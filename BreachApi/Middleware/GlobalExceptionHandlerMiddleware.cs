using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace BreachApi.Middleware;

/// <summary>
/// Global exception handler middleware for centralized error handling and logging
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Processes the HTTP request and handles any exceptions that occur
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing request {RequestPath}", 
                context.Request.Path);
            
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles the exception and returns an appropriate HTTP response
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="exception">The exception that occurred</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            error = new
            {
                message = GetErrorMessage(exception),
                type = exception.GetType().Name,
                timestamp = DateTime.UtcNow
            }
        };

        context.Response.StatusCode = GetStatusCode(exception);

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    /// <summary>
    /// Gets an appropriate error message based on the exception type
    /// </summary>
    /// <param name="exception">The exception</param>
    /// <returns>A user-friendly error message</returns>
    private static string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            InvalidOperationException => "An operation failed to complete successfully.",
            ArgumentException => "Invalid input provided.",
            UnauthorizedAccessException => "Access denied.",
            _ => "An unexpected error occurred. Please try again later."
        };
    }

    /// <summary>
    /// Gets the appropriate HTTP status code based on the exception type
    /// </summary>
    /// <param name="exception">The exception</param>
    /// <returns>The HTTP status code</returns>
    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ArgumentException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            InvalidOperationException => (int)HttpStatusCode.InternalServerError,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }
} 