using System.Reflection;
using System.Text;
using Faoem.Common.DbContexts;
using Faoem.Common.Handlers;
using Faoem.Common.Options;
using Faoem.Common.Services.Email;
using Faoem.Common.Services.Jwt;
using Faoem.Common.Services.Menu;
using Faoem.Common.Services.Permission;
using Faoem.Common.Services.Role;
using Faoem.Common.Services.RoleMenu;
using Faoem.Common.Services.RolePermission;
using Faoem.Common.Services.Setting;
using Faoem.Common.Services.User;
using Faoem.Common.Services.UserRole;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Faoem.Common.Extensions;

public static class CommonExtension
{
    public static IServiceCollection AddCommon(this IServiceCollection services, IConfiguration configuration)
    {
        // 配置数据库相关服务
        services.AddSqliteDbContext<CommonDbContext, SqliteCommonDbContext>(configuration);
        services.AddMySqlDbContext<CommonDbContext, MySqlCommonDbContext>(configuration);

        // 配置 HttpContextAccessor
        services.AddHttpContextAccessor();

        // 配置 SettingService
        services.AddScoped<ISettingService, SettingService>();

        // 配置认证服务
        var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.IssuerSigningKey)),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:ValidAudience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(jwtOptions.ClockSkew)
                };
            });

        // 配置 JwtService
        services.AddSingleton<IJwtService, JwtService>();

        // 配置 UserService
        services.AddScoped<IUserService, UserService>();

        // 配置 RoleService
        services.AddScoped<IRoleService, RoleService>();

        // 配置 UserRoleService
        services.AddScoped<IUserRoleService, UserRoleService>();

        services.AddScoped<IRolePermissionService, RolePermissionService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IRoleMenuService, RoleMenuService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IEmailService, EmailService>();

        // 配置授权处理程序
        services.TryAddSingleton<IAuthorizationHandler, DefaultAuthorizationHandler>();

        // 启用全局授权处理
        services.Configure<MvcOptions>(options => options.Filters.Add(new AuthorizeFilter()));

        return services;
    }

    public static WebApplication ConfigureCommon(this WebApplication app)
    {
        // 自动创建 Data 目录
        if (app.Environment.IsDevelopment())
        {
            var dir = Directory.GetCurrentDirectory();
            var dataDir = Path.Combine(dir, "Data");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
        }
        else
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (dir is not null)
            {
                var dataDir = Path.Combine(dir, "Data");
                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);
                }
            }
        }

        // 迁移数据库
        using var scope = app.Services.CreateScope();
        var commonContext = scope.ServiceProvider.GetRequiredService<CommonDbContext>();
        commonContext.Database.Migrate();

        // 添加种子数据
        commonContext.AddSeedData();

        // 配置权限数据
        app.MapPermission();

        app.UpdateSysAdminPermission();

        app.AddDefaultMenu();

        app.AddDefaultSettings();

        // 自定义 Swagger
        app.UseCustomSwagger();

        // 配置认证和授权中间件
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}