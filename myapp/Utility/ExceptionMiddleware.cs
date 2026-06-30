using TraineeManagement.myapp.Models;
using TraineeManagement.myapp.Exceptions;

namespace TraineeManagement.myapp.Utility
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Triggers if GetSafePath detects a hacker or directory traversal attack
                _logger.LogWarning(ex, "Security violation caught in middleware.");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { message = "Access denied: Invalid file operations." });
            }
            catch (OperationCanceledException ex)
            {
                // Triggers if CancellationToken stops a file upload or download mid-execution
                _logger.LogInformation(ex, "A file operation task was cancelled by the user or system.");
                context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
                await context.Response.WriteAsJsonAsync(new { message = "The operation was aborted." });
            }
            catch (FileNotFoundException ex)
            {
                // Triggers if a file is requested but missing from the local disk folder
                _logger.LogWarning(ex, "Requested disk file could not be located.");
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { message = "The requested file asset does not exist on this server." });
            }
            catch (ArgumentException ex)
            {
                // Triggers if bad validation data is processed (like an invalid MIME type or extension)
                _logger.LogWarning(ex, "Validation failure caught in service layer.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
            catch(BusinessValidationException ex)
            {
                //custom exception is used so that no unauthorised person can signup as trainee or mentor
                _logger.LogWarning(ex, "Business rule Validation failed");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new {message = ex.Message});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "An unexpected error occurred. Please try again later.",
                    //  THE FIX: Read the inner exception directly right here
                    detail = ex.InnerException != null ? ex.InnerException.Message : ex.Message
                });
            }
        }
    }
}