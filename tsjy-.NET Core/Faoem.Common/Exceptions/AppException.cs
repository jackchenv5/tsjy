using Microsoft.AspNetCore.Http;

namespace Faoem.Common.Exceptions;

public class AppException : Exception
{
    public int StatusCode { get; set; } = StatusCodes.Status500InternalServerError;

    public AppException(string message) : base(message)
    {
    }

    public AppException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}