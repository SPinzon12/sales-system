using System.Net;
using System.Text.Json;

namespace SalesSystem.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Error no controlado en la solicitud {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        var response = context.Response;
        response.ContentType = "application/json";

        var (statusCode, code, message) = GetErrorDetails(exception);

        response.StatusCode = statusCode;

        var result = JsonSerializer.Serialize(new
        {
            codigo = code,
            mensaje = message,
            path = context.Request.Path,
            metodo = context.Request.Method,
            timestamp = DateTime.UtcNow.ToString("o")
        });

        await response.WriteAsync(result);
    }

    private static (int statusCode, string code, string message) GetErrorDetails(Exception exception)
    {
        return exception switch
        {
            ArgumentException => ((int)HttpStatusCode.BadRequest, "VALIDACION_ERROR", exception.Message),
            InvalidOperationException => ((int)HttpStatusCode.BadRequest, "OPERACION_INVALIDA", exception.Message),
            KeyNotFoundException => ((int)HttpStatusCode.NotFound, "RECURSO_NO_ENCONTRADO", exception.Message),
            UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized, "NO_AUTORIZADO", exception.Message),
            _ => ((int)HttpStatusCode.InternalServerError, "ERROR_INTERNO", "Error interno del servidor")
        };
    }
}
