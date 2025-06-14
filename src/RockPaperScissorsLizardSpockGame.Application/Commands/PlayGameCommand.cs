using MediatR;
using RockPaperScissorsLizardSpockGame.Application.DTOs;
using RockPaperScissorsLizardSpockGame.Domain.Models;
using CSharpFunctionalExtensions;

namespace RockPaperScissorsLizardSpockGame.Application.Commands;

public record PlayGameCommand(GameMove PlayerMove, string? Email = null) : IRequest<Result<PlayGameResponse>>;