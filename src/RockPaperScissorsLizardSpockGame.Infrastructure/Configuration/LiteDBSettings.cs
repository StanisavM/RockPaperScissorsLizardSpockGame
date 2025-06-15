using FluentValidation;

namespace RockPaperScissorsLizardSpockGame.Infrastructure;

/// <summary>
/// Represents configuration settings for LiteDB database access.
/// </summary>
public class LiteDbSettings
{
    public string DbName { get; set; } = default!;
    public string CollectionName { get; set; } = default!;
    public class LiteDbSettingsValidator : AbstractValidator<LiteDbSettings>
    {
        public LiteDbSettingsValidator()
        {
            RuleFor(x => x.DbName).NotEmpty().WithMessage("LiteDb:DbName is required.");
            RuleFor(x => x.CollectionName).NotEmpty().WithMessage("LiteDb:CollectionName is required.");
        }
    }
}