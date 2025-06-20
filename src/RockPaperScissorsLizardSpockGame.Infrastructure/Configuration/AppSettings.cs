﻿using FluentValidation;

namespace RockPaperScissorsLizardSpockGame.Infrastructure.Configuration;

/// <summary>
/// Root settings class for infrastructure configuration.
/// </summary>
public class AppSettings
{
    public LiteDbSettings LiteDb { get; set; } = new();
    public RandomNumberServiceSettings RandomNumberService { get; set; } = new();
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}

public class AppSettingsValidator : AbstractValidator<AppSettings>
{
    public AppSettingsValidator()
    {
        RuleFor(x => x.LiteDb).SetValidator(new LiteDbSettings.LiteDbSettingsValidator());
        RuleFor(x => x.RandomNumberService).SetValidator(new RandomNumberServiceSettings.RandomNumberServiceSettingsValidator());
        RuleFor(x => x.AllowedOrigins).NotEmpty().WithMessage("AllowedOrigins configuration must not be empty.");
    }
}