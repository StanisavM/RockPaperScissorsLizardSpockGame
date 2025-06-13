using MediatR;
using RockPaperScissorsLizardSpockGame.Application.DTOs;

namespace RockPaperScissorsLizardSpockGame.Application.Queries;

public record GetChoicesQuery : IRequest<List<GameChoiceDto>>;