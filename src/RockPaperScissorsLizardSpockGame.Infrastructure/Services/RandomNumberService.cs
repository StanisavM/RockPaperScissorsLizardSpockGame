using Microsoft.Extensions.Logging;
using RockPaperScissorsLizardSpockGame.Application.Interfaces;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace RockPaperScissorsLizardSpockGame.Application.Services;

public class RandomNumberService(HttpClient httpClient, ILogger<RandomNumberService> logger) : IRandomNumberService
{
    public async Task<int> GetRandomNumber(CancellationToken ct)
    {
        logger.LogInformation("Initiating request for random number...");

        var httpResponse = await httpClient.GetAsync(string.Empty, ct);

        httpResponse.EnsureSuccessStatusCode();

        var result = await httpResponse.Content.ReadFromJsonAsync<RandomNumberResult>(cancellationToken: ct);

        if (result is { RandomNumber: > 0 })
        {
            logger.LogInformation("Random number obtained: {Number}", result.RandomNumber);
            return result.RandomNumber;
        }

        logger.LogWarning("No valid random number received; generating fallback.");
        var fallbackNumber = Random.Shared.Next(1, 100);
        logger.LogInformation("Fallback random number generated: {FallbackNumber}", fallbackNumber);

        return fallbackNumber;
    }
    private record RandomNumberResult([property: JsonPropertyName("random_number")] int RandomNumber);
}
