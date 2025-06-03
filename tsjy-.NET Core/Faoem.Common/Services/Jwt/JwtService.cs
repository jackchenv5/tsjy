using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Faoem.Common.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Faoem.Common.Services.Jwt;

internal class JwtService(IConfiguration configuration) : IJwtService
{
    private readonly JwtOptions _jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
    private readonly string[] _internalClaimTypes = new[] { "id", "nbf", "exp", "iat", "iss", "aud" };

    public Task<string> GetEncodedJwtAsync(IDictionary<string, object>? claimCollection = null)
    {
        var now = DateTime.Now;

        var subject = new ClaimsIdentity();

        subject.AddClaim(new Claim("id", Guid.NewGuid().ToString("N")));

        var encodedToken = new JwtSecurityTokenHandler().CreateEncodedJwt(
            _jwtOptions.ValidIssuer,
            _jwtOptions.ValidAudience,
            subject,
            now,
            now.AddMinutes(_jwtOptions.TokenLifeTime),
            now,
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.IssuerSigningKey)),
                SecurityAlgorithms.HmacSha256),
            null,
            claimCollection
        );

        return Task.FromResult(encodedToken);
    }

    public async Task<string?> GetRefreshEncodedJwtAsync(string token)
    {
        JwtSecurityToken? jwtToken;

        jwtToken = await ValidateTokenAsync(token, false);

        if (jwtToken == null)
        {
            return default;
        }

        var hasId = jwtToken.Payload.ContainsKey("id");

        if (!hasId)
        {
            return default;
        }

        if (jwtToken.Payload["id"] is not string tokenId)
        {
            return default;
        }

        var now = DateTime.Now;
        var refreshToken = new JwtSecurityTokenHandler().CreateEncodedJwt(
            _jwtOptions.ValidIssuer,
            _jwtOptions.ValidAudience,
            null,
            now,
            now.AddMinutes(_jwtOptions.RefreshTokenLifeTime ?? _jwtOptions.TokenLifeTime * 2),
            now,
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.IssuerSigningKey)),
                SecurityAlgorithms.HmacSha256),
            null,
            new Dictionary<string, object>()
            {
                { "id", Guid.NewGuid().ToString("N") },
                { "tokenId", tokenId }
            }
        );

        return refreshToken;
    }

    private async Task<JwtSecurityToken?> ValidateTokenAsync(string encodedToken, bool validateLifetime = true)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var result = await jwtHandler.ValidateTokenAsync(encodedToken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.IssuerSigningKey)),
            ValidateIssuer = true,
            ValidIssuer = _jwtOptions.ValidIssuer,
            ValidateAudience = true,
            ValidAudience = _jwtOptions.ValidAudience,
            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.FromSeconds(_jwtOptions.ClockSkew)
        });

        if (result.IsValid)
        {
            return result.SecurityToken as JwtSecurityToken;
        }

        return default;
    }

    public async Task<(string, string)> RefreshTokenAsync(string encodedToken, string encodedRefreshToken)
    {
        var token = await ValidateTokenAsync(encodedToken, false);
        var refreshToken = await ValidateTokenAsync(encodedRefreshToken);

        if (token == null || refreshToken == null)
        {
            // token 或 refresh token 任意一个无效，都不允许刷新 token
            return default;
        }

        // 获取不在 _internalClaimTypes 中的 claim
        var claims = token.Claims.Where(claim => !_internalClaimTypes.Contains(claim.Type))
            .ToDictionary(claim => claim.Type, claim => (object)claim.Value);

        var newEncodedToken = await GetEncodedJwtAsync(claims);
        var newEncodedRefreshToken = await GetRefreshEncodedJwtAsync(newEncodedToken);

        return string.IsNullOrEmpty(newEncodedRefreshToken) ? default : (newEncodedToken, newEncodedRefreshToken);
    }
}