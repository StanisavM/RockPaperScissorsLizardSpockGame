using System.Net;
using System.Text.Json;
namespace RockPaperScissorsLizardSpockGame.Api.Middlewares;

/// <summary>
/// Middleware that handles unhandled exceptions during HTTP request processing
/// and returns standardized JSON error responses.
/// </summary>
/// <param name="next">The next middleware in the request pipeline.</param>
/// <param name="logger">The logger used to log exception details.</param>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    /// <summary>
    /// Processes an HTTP request, handling any unhandled exceptions and formatting error responses.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while handling the request.");
            await FormatErrorResponseAsync(context, ex);
        }
    }
    private static Task FormatErrorResponseAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;
        var rootEx = exception.GetBaseException();

        var (statusCode, message) = rootEx switch
        {
            ArgumentException => ((int)HttpStatusCode.BadRequest, rootEx.Message),
            HttpRequestException => ((int)HttpStatusCode.ServiceUnavailable,
                "We're unable to connect to a required external service. Please try again shortly."),
            LiteDB.LiteException => ((int)HttpStatusCode.InternalServerError,
                "A database error occurred while processing your request. Please try again later."),
            _ => ((int)HttpStatusCode.InternalServerError,
                "Something went wrong. Please contact support if the problem persists.")
        };

        response.StatusCode = statusCode;
        var error = new { message };

        return response.WriteAsync(JsonSerializer.Serialize(error));
    }
}
