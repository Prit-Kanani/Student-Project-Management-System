using Comman.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace ProjectService.Exceptions;

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

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = "The API is not working";

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
