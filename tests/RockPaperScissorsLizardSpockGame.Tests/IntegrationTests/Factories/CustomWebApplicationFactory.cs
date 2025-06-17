using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RockPaperScissorsLizardSpockGame.Application.Interfaces;
using RockPaperScissorsLizardSpockGame.Application.Services;
using RockPaperScissorsLizardSpockGame.Infrastructure;
using RockPaperScissorsLizardSpockGame.Infrastructure.Services;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace RockPaperScissorsLizardSpockGame.Tests.IntegrationTests.Factories;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private WireMockServer _wireMockServer;
    private readonly string _testDbPath = Path.Combine(Path.GetTempPath(), "test-scoreboard.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _wireMockServer = WireMockServer.Start(9090);

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var testConfig = new Dictionary<string, string>
            {
                { "RandomNumberService:Url", "http://localhost:9090/random" },
                { "LiteDb:DbName", _testDbPath },
                { "LiteDb:CollectionName", "scores" },
                { "AllowedOrigins:0", "http://localhost" }
            };

            config.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddHttpClient<IRandomNumberService, RandomNumberService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:9090/random");
            });

            // Ensure IScoreboardService uses the real LiteDB implementation with the test DB path
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IScoreboardService));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddScoped<IScoreboardService>(_ =>
               new LiteDBScoreboardService(
                   Options.Create(new LiteDbSettings
                   {
                       DbName = _testDbPath,
                       CollectionName = "scores"
                   })
               )
           );
        });
    }

    public void SetupValidResponse()
    {
        _wireMockServer?.Reset();

        _wireMockServer?.Given(
            Request.Create().WithPath("/random").UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithBody(_ =>
                {
                    var randomValue = new Random().Next(1, 100);
                    var responseJson = JsonSerializer.Serialize(new { random_number = randomValue });
                    return responseJson;
                })
                .WithHeader("Content-Type", "application/json")
        );

        Thread.Sleep(500);
    }

    public void SetupServiceUnavailable()
    {
        _wireMockServer?.Reset();

        _wireMockServer?.Given(
            Request.Create().WithPath("/random").UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(503)
                .WithBody("Service Unavailable")
                .WithHeader("Content-Type", "text/plain")
        );
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _wireMockServer?.Stop();
            _wireMockServer?.Dispose();

            // Clean up test LiteDB
            if (File.Exists(_testDbPath))
            {
                File.Delete(_testDbPath);
            }
        }

        base.Dispose(disposing);
    }
}
