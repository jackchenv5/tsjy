namespace Faoem.Common.Services.Jwt;

public interface IJwtService
{
    public Task<string> GetEncodedJwtAsync(IDictionary<string, object>? claimCollection = null);
    public Task<string?> GetRefreshEncodedJwtAsync(string token);
    public Task<(string, string)> RefreshTokenAsync(string encodedToken, string encodedRefreshToken);
}