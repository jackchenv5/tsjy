using System.Text.Json.Serialization;
using Faoem.Common.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Faoem.Common.Extensions;

public static class ControllerExtension
{
    public static IMvcBuilder Configure(this IMvcBuilder builder)
    {
        builder.Services.Configure<MvcOptions>(options => options.Filters.Add<ExceptionFilter>());

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        return builder;
    }
}