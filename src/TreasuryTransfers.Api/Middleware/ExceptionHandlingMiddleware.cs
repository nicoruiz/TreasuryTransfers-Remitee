using System.Net;
using System.Text.Json;
using TreasuryTransfers.Domain.Exceptions;

namespace TreasuryTransfers.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

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

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, code) = exception switch
        {
            ValidationException => (HttpStatusCode.BadRequest, "VALIDATION_ERROR"),
            NotFoundException => (HttpStatusCode.NotFound, "NOT_FOUND"),
            _ => (HttpStatusCode.InternalServerError, "INTERNAL_ERROR")
        };

        var message = statusCode == HttpStatusCode.InternalServerError
            ? "An unexpected error occurred."
            : exception.Message;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = JsonSerializer.Serialize(new { code, message });
        await context.Response.WriteAsync(response);
    }
}
