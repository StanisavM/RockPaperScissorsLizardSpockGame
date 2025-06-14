using RockPaperScissorsLizardSpockGame.Domain.Models;

namespace RockPaperScissorsLizardSpockGame.Application.DTOs;

public record PlayGameResponse(GameMove Player, GameMove Computer, string Results, string? FunFact = null);
