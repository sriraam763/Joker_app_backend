using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Joker_app_backend.MiddleWares;

public class GlobalExceptionHandler: IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception,"An unhandled exception occured:{Message}",exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server error",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Detail = "An unexpected error occurred on the server. Please try again later."
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        // 3. Write JSON response to Angular
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Return true to signal that this exception was handled
        return true;
    }
}