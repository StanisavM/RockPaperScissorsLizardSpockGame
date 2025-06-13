using RockPaperScissorsLizardSpockGame.Application.DTOs;
using RockPaperScissorsLizardSpockGame.Domain.Models;

namespace RockPaperScissorsLizardSpockGame.Application.Helpers;

public static class GameMovesHelpers
{
    public static IReadOnlyList<GameChoiceDto> Choices => _choices.Value;

    private static readonly Lazy<IReadOnlyList<GameChoiceDto>> _choices = new(() =>
        Enum.GetValues<GameMove>()
            .Select(move => new GameChoiceDto((int)move, move.ToString().ToLowerInvariant()))
            .ToList()
            .AsReadOnly());
}
