using Microsoft.Extensions.Logging;
using RockPaperScissorsLizardSpockGame.Application.Interfaces;
using RockPaperScissorsLizardSpockGame.Infrastructure.Helpers;
using System.Text.Json.Serialization;

namespace RockPaperScissorsLizardSpockGame.Application.Services;

/// <summary>
/// Service for retrieving a random number from an external API.
/// If the API response is invalid or deserialization fails, a local random number is generated as a fallback.
/// </summary>
public class RandomNumberService(HttpClient httpClient, ILogger<RandomNumberService> logger) : IRandomNumberService
{
    /// <summary>
    /// Gets a random number from the external service.
    /// If the response is invalid or cannot be deserialized, generates and returns a local random number as a fallback.
    /// </summary>
    /// <param name="ct">Cancellation token for the request.</param>
    /// <returns>A random number from the API, or a locally generated fallback if the API fails.</returns>
    public async Task<int> GetRandomNumber(CancellationToken ct)
    {
        logger.LogInformation("Initiating request for random number...");

        var httpResponse = await httpClient.GetAsync(string.Empty, ct);
        httpResponse.EnsureSuccessStatusCode();

        var result = await HttpContentJsonHelper.SafeReadFromJsonAsync<RandomNumberResult>(httpResponse.Content, logger, ct);

        if (result is { RandomNumber: > 0 })
        {
            logger.LogInformation("Random number obtained: {Number}", result.RandomNumber);
            return result.RandomNumber;
        }

        return GenerateAndLogFallbackNumber(logger);
    }

    /// <summary>
    /// Generates a fallback random number and logs the event.
    /// </summary>
    /// <param name="logger">Logger instance for logging the fallback event.</param>
    /// <returns>A locally generated random number between 1 and 99 (inclusive).</returns>
    private static int GenerateAndLogFallbackNumber(ILogger logger)
    {
        logger.LogWarning("No valid random number received; generating fallback.");
        var fallbackNumber = Random.Shared.Next(1, 100);
        logger.LogInformation("Fallback random number generated: {FallbackNumber}", fallbackNumber);
        return fallbackNumber;
    }

    private record RandomNumberResult([property: JsonPropertyName("random_number")] int RandomNumber);
}