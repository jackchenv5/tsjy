using Faoem.Common.DbContexts;
using Faoem.Common.Dtos;
using Faoem.Common.Exceptions;
using Faoem.Common.Extensions;
using Faoem.Common.Inputs;
using Faoem.Common.Models;
using Faoem.Common.Options;
using Faoem.Common.Services.Email;
using Faoem.Common.Services.Jwt;
using Faoem.Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Faoem.Common.Services.User;

internal class UserService(
    CommonDbContext commonDbContext,
    IHttpContextAccessor httpContextAccessor,
    IJwtService jwtService,
    IConfiguration configuration,
    IEmailService emailService
) : IUserService
{
    public async Task<UserDto> LoginAsync(LoginInput loginInput)
    {
        var user = await commonDbContext.Users.FirstOrDefaultAsync(
            u => u.LowerUsername == loginInput.Username.ToLower());
        if (user == null)
        {
            throw new AppException("Invalid username or password.", StatusCodes.Status401Unauthorized);
        }

        var passwordHash = UserUtils.GetPasswordHash(loginInput.Password, user.Salt);
        if (passwordHash != user.PasswordHash)
        {
            throw new AppException("Invalid username or password.", StatusCodes.Status401Unauthorized);
        }

        return await IssueTokenAsync(user);
    }

    public async Task<UserDto> CaptchaLoginAsync(CaptchaInput captchaInput)
    {
        var lowerEmail = captchaInput.Email.ToLower();
        var validDomain = await emailService.ValidDomainAsync(captchaInput.Email);
        if (!validDomain)
        {
            throw new AppException("The email domain is not allowed.", StatusCodes.Status400BadRequest);
        }

        var captcha = await commonDbContext.Captchas
            .OrderBy(c => c.ExpireTime)
            .LastOrDefaultAsync(c => c.LowerEmail == lowerEmail);

        if (captcha is null)
        {
            throw new AppException("Invalid email or captcha.", StatusCodes.Status401Unauthorized);
        }

        if (captcha.Code != captchaInput.Code)
        {
            throw new AppException("Invalid email or captcha.", StatusCodes.Status401Unauthorized);
        }

        if (captcha.ExpireTime < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        {
            commonDbContext.Captchas.Remove(captcha);
            await commonDbContext.SaveChangesAsync();

            throw new AppException("The captcha is expired.", StatusCodes.Status401Unauthorized);
        }

        var user = await commonDbContext.Users.FirstOrDefaultAsync(u => u.LowerEmail == lowerEmail);

        if (user is null)
        {
            var salt = Guid.NewGuid().ToString("N");
            user = new Models.User
            {
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Email = captcha.Email,
                Salt = salt,
                PasswordHash = UserUtils.GetPasswordHash(Guid.NewGuid().ToString("N"), salt),
                Username = captcha.Email
            };
            await commonDbContext.Users.AddAsync(user);
        }

        var userDto = await IssueTokenAsync(user);

        commonDbContext.Captchas.Remove(captcha);
        await commonDbContext.SaveChangesAsync();

        return userDto;
    }

    public async Task GetCaptchaAsync(EmailInput emailInput)
    {
        var lowerEmail = emailInput.Email.ToLower();

        var validDomain = await emailService.ValidDomainAsync(emailInput.Email);
        if (!validDomain)
        {
            throw new AppException("The email domain is not allowed.", StatusCodes.Status400BadRequest);
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var captcha = await commonDbContext.Captchas
            .AnyAsync(c => c.LowerEmail == lowerEmail && c.ExpireTime > now);
        if (captcha)
        {
            throw new AppException("The captcha is already sent.", StatusCodes.Status400BadRequest);
        }

        var captchaOptions = configuration.GetSection("Captcha").Get<CaptchaOptions>() ?? new CaptchaOptions();


        var code = new Random().Next(100000, 999999);
        var expireTime = captchaOptions.LifeTime * 60 + now;
        var newCaptcha = new Captcha
        {
            Code = code,
            Email = emailInput.Email,
            ExpireTime = expireTime
        };

        var mimeMessage = new MimeMessage();
        mimeMessage.To.Add(new MailboxAddress(emailInput.Email, emailInput.Email));
        mimeMessage.Subject = "Your Captcha Code";
        mimeMessage.Body = new TextPart("plain")
        {
            Text = $"Your captcha code is {code}. It will expire in {captchaOptions.LifeTime} minutes.",
        };

        await emailService.SendAsync(mimeMessage);

        await commonDbContext.Captchas.AddAsync(newCaptcha);
        await commonDbContext.SaveChangesAsync();
    }

    private async Task<UserDto> IssueTokenAsync(Models.User user)
    {
        user.LastLogin = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        await commonDbContext.SaveChangesAsync();

        var encodedToken = await jwtService.GetEncodedJwtAsync(new Dictionary<string, object>()
        {
            { "uid", user.Id.ToString() }
        });

        var encodedRefreshToken = await jwtService.GetRefreshEncodedJwtAsync(encodedToken);

        if (httpContextAccessor.HttpContext == null)
        {
            return user.ToUserDto();
        }

        httpContextAccessor.HttpContext.Response.Headers["Access-Token"] = encodedToken;
        httpContextAccessor.HttpContext.Response.Headers["Refresh-Token"] = encodedRefreshToken;

        return user.ToUserDto();
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        var claims = httpContextAccessor.HttpContext?.User.Claims;
        var userIdString = claims?.FirstOrDefault(c => c.Type == "uid")?.Value;

        if (userIdString is null)
        {
            return default;
        }

        if (!long.TryParse(userIdString, out var userId))
        {
            return default;
        }

        var user = await commonDbContext.Users.FindAsync(userId);

        return user?.ToUserDto();
    }

    public async Task<PagedDto<UserDto>> GetUserAsync(int pageIndex = 1, int pageSize = 20)
    {
        var pagedDto = new PagedDto<UserDto>();
        pagedDto.Total = await commonDbContext.Users.CountAsync();
        pagedDto.Items = await commonDbContext.Users
            .OrderBy(u => u.Id)
            .Select(u => u.ToUserDto())
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return pagedDto;
    }

    public async Task<UserDto?> GetUserAsync(long userId)
    {
        var user = await commonDbContext.Users.FindAsync(userId);

        return user?.ToUserDto();
    }

    public async Task<UserDto> AddUserAsync(UserInput userInput)
    {
        var lowerUsername = userInput.Username.ToLower();
        if (await commonDbContext.Users.AnyAsync(u => u.LowerUsername == lowerUsername))
        {
            throw new AppException("The username is in used.", 400);
        }

        if (string.IsNullOrEmpty(userInput.Password))
        {
            throw new AppException("The password hash can not be empty.", 400);
        }

        var salt = Guid.NewGuid().ToString("N");
        var user = new Models.User
        {
            Username = userInput.Username,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            FullName = userInput.FullName,
            LastLogin = 0,
            PasswordHash = UserUtils.GetPasswordHash(userInput.Password, salt),
            Salt = salt
        };

        await commonDbContext.AddAsync(user);
        await commonDbContext.SaveChangesAsync();

        return user.ToUserDto();
    }

    public async Task UpdateUserAsync(long userId, UserInput userInput)
    {
        var user = await commonDbContext.Users.FindAsync(userId);
        if (user is null)
        {
            throw new AppException("The user is not found.", 404);
        }

        var currentUser = await GetCurrentUserAsync();
        if (currentUser is null)
        {
            throw new AppException("Your account is not authenticated.", 401);
        }

        var lowerUsername = userInput.Username.ToLower();

        if (user.LowerUsername == "sysadmin")
        {
            if (currentUser.LowerUsername != "sysadmin")
            {
                throw new AppException("You are not allowed to modify the sysadmin account.", 403);
            }

            if (lowerUsername != "sysadmin")
            {
                throw new AppException("You are not allowed to modify the username of sysadmin account.", 403);
            }

            user.FullName = userInput.FullName;
            if (userInput.Password is not null)
            {
                user.PasswordHash = UserUtils.GetPasswordHash(userInput.Password, user.Salt);
            }
        }
        else
        {
            user.Username = userInput.Username;
            user.FullName = userInput.FullName;
            if (!string.IsNullOrEmpty(userInput.Password))
            {
                user.PasswordHash = UserUtils.GetPasswordHash(userInput.Password, user.Salt);
            }
        }

        await commonDbContext.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(long userId)
    {
        var user = await commonDbContext.Users.FindAsync(userId);

        if (user is null)
        {
            throw new AppException("The user is not found.", 404);
        }

        if (user.LowerUsername == "sysadmin")
        {
            throw new AppException("You are not allowed to delete the sysadmin account.", 403);
        }

        commonDbContext.Users.Remove(user);
        await commonDbContext.SaveChangesAsync();
    }

    public async Task UpdateUserInfoAsync(long userId, UserInfoInput input)
    {
        var user = await commonDbContext.Users.FindAsync(userId);
        if (user is null)
        {
            throw new AppException("The user is not found.", 404);
        }

        user.FullName = input.FullName;
        await commonDbContext.SaveChangesAsync();
    }

    public async Task UpdatePasswordAsync(long userId, PasswordInput input)
    {
        var user = await commonDbContext.Users.FindAsync(userId);
        if (user is null)
        {
            throw new AppException("The user is not found.", 404);
        }

        if (user.PasswordHash != UserUtils.GetPasswordHash(input.Password, user.Salt))
        {
            throw new AppException("The old password is incorrect.", 403);
        }

        if (input.NewPassword != input.ConfirmNewPassword)
        {
            throw new AppException("The new password and confirm password are not the same.", 400);
        }

        user.PasswordHash = UserUtils.GetPasswordHash(input.NewPassword, user.Salt);
        await commonDbContext.SaveChangesAsync();
    }
}