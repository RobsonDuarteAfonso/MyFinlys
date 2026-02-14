using System.Net;
using System.Text.Json;
using MyFinlys.Api.Dtos;

namespace MyFinlys.Api.Middleware;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = new ErrorResponse();

        if (exception is ArgumentException argEx)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.Message = argEx.Message;
            response.ErrorCode = "INVALID_ARGUMENT";
        }
        else if (exception is ArgumentOutOfRangeException outOfRangeEx)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.Message = outOfRangeEx.Message;
            response.ErrorCode = "OUT_OF_RANGE";
        }
        else if (exception is KeyNotFoundException keyEx)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            response.Message = keyEx.Message;
            response.ErrorCode = "NOT_FOUND";
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.Message = "Erro interno do servidor. Por favor, tente novamente.";
            response.ErrorCode = "INTERNAL_SERVER_ERROR";
        }

        response.Timestamp = DateTime.UtcNow;
        return context.Response.WriteAsJsonAsync(response);
    }
}
