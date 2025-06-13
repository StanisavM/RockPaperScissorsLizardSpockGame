using System.Net;
using System.Text.Json;

namespace RockPaperScissorsLizardSpockGame.Api.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
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
            HttpRequestException => ((int)HttpStatusCode.ServiceUnavailable, "We're unable to connect to a required external service. Please try again shortly."),
            _ => ((int)HttpStatusCode.InternalServerError, "Something went wrong. Please contact support if the problem persists.")
        };

        response.StatusCode = statusCode;
        var error = new { message };

        return response.WriteAsync(JsonSerializer.Serialize(error));
    }
}
