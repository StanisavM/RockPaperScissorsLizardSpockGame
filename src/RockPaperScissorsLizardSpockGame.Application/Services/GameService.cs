using CSharpFunctionalExtensions;
using MediatR;
using RockPaperScissorsLizardSpockGame.Application.Commands;
using RockPaperScissorsLizardSpockGame.Application.DTOs;
using RockPaperScissorsLizardSpockGame.Application.Interfaces;
using RockPaperScissorsLizardSpockGame.Application.Queries;
using RockPaperScissorsLizardSpockGame.Domain.Models;

namespace RockPaperScissorsLizardSpockGame.Application.Services;

public class GameService(IMediator mediator) : IGameService
{
    private readonly IMediator _mediator = mediator;
    public async Task<Result<PlayGameResponse>> PlayGame(int playerChoiceId, CancellationToken ct, string? email = null) => await _mediator.Send(new PlayGameCommand((GameMove)playerChoiceId, email), ct);

    public async Task<List<GameChoiceDto>> GetGameChoices(CancellationToken ct) => await _mediator.Send(new GetChoicesQuery(), ct);

    public async Task<GameChoiceDto> GetRandomChoice(CancellationToken ct) => await _mediator.Send(new GetRandomChoiceQuery(), ct);
}
