using Microsoft.OpenApi.Models;
using RockPaperScissorsLizardSpockGame.Api.Middlewares;
using RockPaperScissorsLizardSpockGame.Application.Commands;
using RockPaperScissorsLizardSpockGame.Application.Interfaces;
using RockPaperScissorsLizardSpockGame.Application.Queries;
using RockPaperScissorsLizardSpockGame.Application.Services;
using RockPaperScissorsLizardSpockGame.Infrastructure.Configuration;
using RockPaperScissorsLizardSpockGame.Infrastructure.Extensions;
using RockPaperScissorsLizardSpockGame.Infrastructure.Services;
using Serilog;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

try
{
    Log.Information("Starting web host");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddInfrastructure(builder.Configuration);

    var appSettings = new AppSettings();
    builder.Configuration.GetSection("AppSettings").Bind(appSettings);

    builder.Services.AddConfiguredCors(appSettings!);
    builder.Services.AddSwaggerGen(c =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);

        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Rock Paper Scissors Lizard Spock API",
            Version = "v1",
            Description = "API for playing Rock Paper Scissors Lizard Spock game."
        });
    });
    builder.Services.AddHttpClient<IRandomNumberService, RandomNumberService>();
    builder.Services.AddScoped<IScoreboardService, LiteDBScoreboardService>();
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(GetChoicesQuery).Assembly);
        cfg.RegisterServicesFromAssembly(typeof(GetRandomChoiceQuery).Assembly);
        cfg.RegisterServicesFromAssembly(typeof(PlayGameCommand).Assembly);
    });

    builder.Services.AddScoped<IGameService, GameService>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors("AllowSpecificOrigin");
    app.UseHttpsRedirection();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
