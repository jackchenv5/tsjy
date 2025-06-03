using Faoem.Common.Dtos;
using Faoem.Common.Inputs;

namespace Faoem.Common.Services.User;

public interface IUserService
{
    public Task<UserDto> LoginAsync(LoginInput loginInput);
    public Task<UserDto> CaptchaLoginAsync(CaptchaInput captchaInput);
    public Task GetCaptchaAsync(EmailInput emailInput);
    public Task<UserDto?> GetCurrentUserAsync();
    public Task<PagedDto<UserDto>> GetUserAsync(int pageIndex = 1, int pageSize = 20);
    public Task<UserDto?> GetUserAsync(long userId);
    public Task<UserDto> AddUserAsync(UserInput userInput);
    public Task UpdateUserAsync(long userId, UserInput userInput);
    public Task DeleteUserAsync(long userId);
    public Task UpdateUserInfoAsync(long userId, UserInfoInput input);
    public Task UpdatePasswordAsync(long userId, PasswordInput input);
}