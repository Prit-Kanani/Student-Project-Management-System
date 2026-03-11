using Newtonsoft.Json;
using System.Net;

namespace UserService.Exceptions;

public class ExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    // ✅ NOW private is valid
    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        int statusCode = (int)HttpStatusCode.InternalServerError;
        string message = "The API is not working";

        // If the exception inherits from ApiException use its status code and message
        if (ex is ApiException apiEx)
        {
            statusCode = apiEx.StatusCode;
            message = apiEx.Message;
        }

        context.Response.StatusCode = statusCode;

        var errorResponse = new
        {
            StatusCode = statusCode,
            Message = message,
            Status = "ERROR"
        };

        var json = JsonConvert.SerializeObject(errorResponse);
        await context.Response.WriteAsync(json);
    }
}
