using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Faoem.Common.Services.Jwt;
using Faoem.Common.Services.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.Common.Handlers;

internal class DefaultAuthorizationHandler(IJwtService jwtService, IServiceScopeFactory serviceScopeFactory)
    : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (isAuthenticated)
        {
            if (context.User.Claims.Any(claim => claim.Type == "tokenId"))
            {
                // 使用 refresh token 作为 token 传递
                context.Fail();
                return;
            }

            await HandlePendingRequirementsAsync(context);
        }
        else
        {
            await AutoRefreshTokenAsync(context);
        }
    }

    private async Task HandlePendingRequirementsAsync(AuthorizationHandlerContext context)
    {
        var scope = serviceScopeFactory.CreateScope();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        var permissible = await permissionService.CheckPermissionAsync();
        foreach (var requirement in context.PendingRequirements)
        {
            if (permissible)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }

    private async Task AutoRefreshTokenAsync(AuthorizationHandlerContext context)
    {
        var resource = context.Resource as AuthorizationFilterContext;
        var httpContext = resource?.HttpContext;

        var authorizationHeader = httpContext?.Request.Headers["Authorization"].ToString();
        var xRefreshTokenHeader = httpContext?.Request.Headers["X-Refresh-Token"].ToString();

        if (string.IsNullOrEmpty(authorizationHeader) || string.IsNullOrEmpty(xRefreshTokenHeader))
        {
            context.Fail();
            return;
        }

        string encodedToken;
        string encodedRefreshToken;

        try
        {
            encodedToken = authorizationHeader.Split("Bearer ")[1];
            encodedRefreshToken = xRefreshTokenHeader.Split("Bearer ")[1];
        }
        catch
        {
            context.Fail();
            return;
        }

        if (string.IsNullOrEmpty(encodedToken) || string.IsNullOrEmpty(encodedRefreshToken))
        {
            context.Fail();
            return;
        }

        var (newEncodedToken, newEncodedRefreshToken) =
            await jwtService.RefreshTokenAsync(encodedToken, encodedRefreshToken);

        if (string.IsNullOrEmpty(newEncodedToken) || string.IsNullOrEmpty(newEncodedRefreshToken))
        {
            context.Fail();
            return;
        }

        if (httpContext != null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (tokenHandler.CanReadToken(newEncodedToken))
            {
                var token = tokenHandler.ReadJwtToken(newEncodedToken);
                var claims = token.Claims;
                if (claims is null)
                {
                    context.Fail();
                    return;
                }

                var claimIdentity = new ClaimsIdentity(claims);
                var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
                httpContext.User = claimsPrincipal;
            }
            else
            {
                context.Fail();
            }


            httpContext.Response.Headers["Access-Token"] = newEncodedToken;
            httpContext.Response.Headers["Refresh-Token"] = newEncodedRefreshToken;
        }

        await HandlePendingRequirementsAsync(context);
    }
}