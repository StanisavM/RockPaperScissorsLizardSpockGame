using MediatR;
using RockPaperScissorsLizardSpockGame.Application.DTOs;
using RockPaperScissorsLizardSpockGame.Application.Helpers;
using RockPaperScissorsLizardSpockGame.Application.Queries;

namespace RockPaperScissorsLizardSpockGame.Application.Handlers;

public class GetChoicesHandler : IRequestHandler<GetChoicesQuery, List<GameChoiceDto>>
{
    public async Task<List<GameChoiceDto>> Handle(GetChoicesQuery request, CancellationToken ct) 
        => await Task.FromResult(GameMovesHelpers.Choices.ToList());
}
