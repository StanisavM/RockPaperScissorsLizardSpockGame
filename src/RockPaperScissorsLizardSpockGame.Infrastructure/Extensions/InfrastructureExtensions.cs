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
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var appSettings = new AppSettings();
        configuration.GetSection("AppSettings").Bind(appSettings);

        var validator = new AppSettingsValidator();
        var validationResult = validator.Validate(appSettings);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new InvalidOperationException($"AppSettings validation failed: {errors}");
        }

        services.Configure<LiteDbSettings>(configuration.GetSection("AppSettings:LiteDb"));
        services.Configure<RandomNumberServiceSettings>(configuration.GetSection("AppSettings:RandomNumberService"));

        services.AddHttpClient<IRandomNumberService, RandomNumberService>(client =>
        {
            client.BaseAddress = new Uri(appSettings.RandomNumberService.Url);
        })
        .AddResilienceHandler("randomNumberPolicy", builder =>
        {
            builder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .HandleResult(response => !response.IsSuccessStatusCode)
            });

            builder.AddTimeout(TimeSpan.FromSeconds(5));

            builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                SamplingDuration = TimeSpan.FromSeconds(30),  // Duration to measure failure rate
                MinimumThroughput = 2,                         // Minimum number of requests before evaluating failures
                BreakDuration = TimeSpan.FromSeconds(15)      // Duration circuit remains open after breaking
            });
        });
    }
}