using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using RockPaperScissorsLizardSpockGame.Application.Interfaces;
using RockPaperScissorsLizardSpockGame.Application.Services;
using System.Net;

namespace RockPaperScissorsLizardSpockGame.Tests.ServicesTests
{
    public class RandomNumberServiceTests
    {
        private static RandomNumberService CreateService(HttpResponseMessage response)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            var loggerMock = new Mock<ILogger<RandomNumberService>>();
            return new RandomNumberService(httpClient, loggerMock.Object);
        }

        [Fact]
        public async Task GetRandomNumber_ReturnsNumber_WhenValidResponse()
        {
            // Arrange
            var json = "{\"random_number\":42}";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
            var service = CreateService(response);

            // Act
            var result = await service.GetRandomNumber(CancellationToken.None);

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public async Task GetRandomNumber_ReturnsFallback_WhenInvalidNumber()
        {
            // Arrange: random_number is 0 (invalid)
            var json = "{\"random_number\":0}";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
            var service = CreateService(response);

            // Act
            var result = await service.GetRandomNumber(CancellationToken.None);

            // Assert
            Assert.InRange(result, 1, 99); // Fallback is Random.Shared.Next(1, 100)
        }

        [Fact]
        public async Task GetRandomNumber_ReturnsFallback_WhenNoContent()
        {
            // Arrange: No content
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("", System.Text.Encoding.UTF8, "application/json")
            };
            var service = CreateService(response);

            // Act
            var result = await service.GetRandomNumber(CancellationToken.None);

            // Assert
            Assert.InRange(result, 1, 99);
        }

        [Fact]
        public async Task GetRandomNumber_Throws_WhenHttpError()
        {
            // Arrange: HTTP 500
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var service = CreateService(response);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => service.GetRandomNumber(CancellationToken.None));
        }

        [Fact]
        public async Task GetRandomNumber_ShouldRetryOnFailure()
        {
            // Arrange: always return 500 Internal Server Error
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddHttpClient<IRandomNumberService, RandomNumberService>()
                .ConfigureHttpClient(client => { client.BaseAddress = new Uri("https://fake-api.com/random"); })
                .ConfigurePrimaryHttpMessageHandler(() => mockHandler.Object)
                .AddStandardResilienceHandler();

            var provider = services.BuildServiceProvider();
            var service = provider.GetRequiredService<IRandomNumberService>();

            // Act & Assert: should throw after retries are exhausted
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                service.GetRandomNumber(CancellationToken.None));

            // Assert: 1 initial + 3 retries = 4 total attempts
            mockHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(4),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}