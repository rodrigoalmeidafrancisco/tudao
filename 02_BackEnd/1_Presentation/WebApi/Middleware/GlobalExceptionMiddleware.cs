using System.Net;
using System.Text.Json;
using Shared.DTOs;

namespace WebApi.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ArgumentException argEx => CreateErrorResponse(
                HttpStatusCode.BadRequest,
                "Invalid argument",
                argEx.Message),

            InvalidOperationException invOpEx => CreateErrorResponse(
                HttpStatusCode.BadRequest,
                "Invalid operation",
                invOpEx.Message),

            UnauthorizedAccessException => CreateErrorResponse(
                HttpStatusCode.Unauthorized,
                "Unauthorized",
                "You are not authorized to access this resource"),

            KeyNotFoundException => CreateErrorResponse(
                HttpStatusCode.NotFound,
                "Resource not found",
                "The requested resource was not found"),

            _ => CreateErrorResponse(
                HttpStatusCode.InternalServerError,
                "Internal server error",
                _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred")
        };

        context.Response.StatusCode = (int)response.StatusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response.ApiResponse, options);
        await context.Response.WriteAsync(json);
    }

    private static (HttpStatusCode StatusCode, ApiResponse<object> ApiResponse) CreateErrorResponse(
        HttpStatusCode statusCode,
        string message,
        string detail)
    {
        var response = ApiResponse<object>.ErrorResponse(message, [detail]);
        return (statusCode, response);
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
