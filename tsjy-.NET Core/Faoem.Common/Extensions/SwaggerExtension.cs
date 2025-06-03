using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Faoem.Common.Extensions;

internal static class SwaggerExtension
{
    internal static WebApplication UseCustomSwagger(this WebApplication app, bool developmentOnly = true,
        string routePrefix = "api")
    {
        if (developmentOnly && !app.Environment.IsDevelopment())
        {
            return app;
        }

        app.UseSwagger(options => { options.RouteTemplate = $"{routePrefix}/{{documentName}}/swagger.json"; });
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/{routePrefix}/v1/swagger.json", "API V1");
            options.RoutePrefix = routePrefix;
        });

        return app;
    }
}