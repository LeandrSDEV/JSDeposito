using JSDeposito.Core.Exceptions;
using System.Net;
using System.Security;
using System.Text.Json;

namespace JSDeposito.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(
                ex,
                "Erro não tratado | Path: {Path} | Method: {Method}",
                context.Request.Path,
                context.Request.Method
            );

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex,
            "Erro não tratado | Path: {Path} | Method: {Method}",
            context.Request.Path,
            context.Request.Method);

        var statusCode = ex switch
        {
            SecurityException => HttpStatusCode.Forbidden,
            NotFoundException => HttpStatusCode.NotFound,
            BusinessException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };

        var response = new
        {
            error = ex.GetType().Name,
            message = ex.Message,
            traceId = context.TraceIdentifier
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}
