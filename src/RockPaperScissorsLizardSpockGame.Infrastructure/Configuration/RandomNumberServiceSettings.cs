using FluentValidation;

namespace RockPaperScissorsLizardSpockGame.Infrastructure.Configuration;

/// <summary>
/// Represents configuration settings for the external random number service.
/// </summary>
public class RandomNumberServiceSettings
{
    /// <summary>
    /// The URL of the random number service endpoint.
    /// </summary>
    public string Url { get; set; } = default!;
    public class RandomNumberServiceSettingsValidator : AbstractValidator<RandomNumberServiceSettings>
    {
        public RandomNumberServiceSettingsValidator()
        {
            RuleFor(x => x.Url).NotEmpty().WithMessage("RandomNumberService:Url is required.");
        }
    }
}