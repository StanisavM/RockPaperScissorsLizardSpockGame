using MediatR;
using RockPaperScissorsLizardSpockGame.Application.DTOs;

namespace RockPaperScissorsLizardSpockGame.Application.Queries;

public record GetRandomChoiceQuery : IRequest<GameChoiceDto>;