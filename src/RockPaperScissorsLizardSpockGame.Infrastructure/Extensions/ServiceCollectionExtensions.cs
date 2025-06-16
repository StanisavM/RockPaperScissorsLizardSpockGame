using Microsoft.Extensions.DependencyInjection;
using RockPaperScissorsLizardSpockGame.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguredCors(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", policy =>
            {
                policy.WithOrigins("https://codechallenge.boohma.com")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        return services;
    }
}