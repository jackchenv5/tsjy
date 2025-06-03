using Faoem.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Faoem.Common.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not AppException appException)
        {
            return;
        }

        context.HttpContext.Response.StatusCode = appException.StatusCode;
        context.Result = new JsonResult(new { appException.Message });
        context.ExceptionHandled = true;
    }
}