using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using RockPaperScissorsLizardSpockGame.Application.Interfaces;
using RockPaperScissorsLizardSpockGame.Application.Services;
using RockPaperScissorsLizardSpockGame.Infrastructure.Configuration;

namespace RockPaperScissorsLizardSpockGame.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    // Extension method to register infrastructure-related services into DI container
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind AppSettings from configuration
        var appSettings = new AppSettings();
        configuration.Bind(appSettings);

        // Validate AppSettings using FluentValidation
        var validator = new AppSettingsValidator();
        var validationResult = validator.Validate(appSettings);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new InvalidOperationException($"AppSettings validation failed: {errors}");
        }

        // Register configuration sections for DI (optional, for IOptions<T> usage)
        services.Configure<LiteDbSettings>(configuration.GetSection("LiteDb"));
        services.Configure<RandomNumberServiceSettings>(configuration.GetSection("RandomNumberService"));

        // Register HttpClient for IRandomNumberService with base address set from config
        services.AddHttpClient<IRandomNumberService, RandomNumberService>(client =>
        {
            client.BaseAddress = new Uri(appSettings.RandomNumberService.Url);
        })
        // Add resilience policies using Polly to make HTTP calls more robust
        .AddResilienceHandler("randomNumberPolicy", builder =>
        {
            // Retry policy: retry up to 3 times with exponential backoff on non-success HTTP responses
            builder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .HandleResult(response => !response.IsSuccessStatusCode)
            });

            // Timeout policy: cancel requests if they take longer than 5 seconds
            builder.AddTimeout(TimeSpan.FromSeconds(5));

            // Circuit breaker policy: breaks circuit for 15 seconds if failure threshold is reached
            builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                SamplingDuration = TimeSpan.FromSeconds(30),  // Duration to measure failure rate
                MinimumThroughput = 2,                         // Minimum number of requests before evaluating failures
                BreakDuration = TimeSpan.FromSeconds(15)      // Duration circuit remains open after breaking
            });
        });
    }
}