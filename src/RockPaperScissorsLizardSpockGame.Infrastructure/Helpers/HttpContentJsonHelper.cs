using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace RockPaperScissorsLizardSpockGame.Infrastructure.Helpers
{
    public static class HttpContentJsonHelper
    {
        /// <summary>
        /// Safely reads and deserializes JSON content, returning default(T) if deserialization fails.
        /// </summary>
        public static async Task<T?> SafeReadFromJsonAsync<T>(
            HttpContent content,
            ILogger logger,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Failed to deserialize JSON content.");
                return default;
            }
            catch (NotSupportedException ex)
            {
                logger.LogWarning(ex, "Content type not supported for JSON deserialization.");
                return default;
            }
        }
    }
}