using CSharpFunctionalExtensions;
using RockPaperScissorsLizardSpockGame.Application.DTOs;

namespace RockPaperScissorsLizardSpockGame.Application.Interfaces;

public interface IGameService
{
    Task<List<GameChoiceDto>> GetGameChoices(CancellationToken ct);
    Task<GameChoiceDto> GetRandomChoice(CancellationToken ct);
    Task<Result<PlayGameResponse>> PlayGame(int playerChoiceId, CancellationToken ct, string? email = null);
}
