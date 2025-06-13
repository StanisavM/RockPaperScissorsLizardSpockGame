using MediatR;
using RockPaperScissorsLizardSpockGame.Application.DTOs;
using RockPaperScissorsLizardSpockGame.Application.Helpers;
using RockPaperScissorsLizardSpockGame.Application.Interfaces;
using RockPaperScissorsLizardSpockGame.Application.Queries;

namespace RockPaperScissorsLizardSpockGame.Application.Handlers;

public class GetRandomChoiceHandler(IRandomNumberService randomNumberService) : IRequestHandler<GetRandomChoiceQuery, GameChoiceDto>
{
    public async Task<GameChoiceDto> Handle(GetRandomChoiceQuery request, CancellationToken ct)
    {
        // Lazily initialized list of all possible game moves
        IReadOnlyList<GameChoiceDto> choices = GameMovesHelpers.Choices;

        // Get a positive random number from an injected service
        int randomNumber = await randomNumberService.GetRandomNumber(ct);

        // Calculate a valid index in the choices list using modulo to avoid out-of-range
        int index = Math.Abs(randomNumber - 1) % choices.Count;

        // Return the choice at the calculated index
        return choices[index];
    }
}
