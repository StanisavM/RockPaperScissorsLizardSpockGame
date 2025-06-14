using RockPaperScissorsLizardSpockGame.Domain.Models;

namespace RockPaperScissorsLizardSpockGame.Application.DTOs;

public record ScoreEntryDto(string PlayerEmail, GameMove PlayerMove, GameMove ComputerMove, string Result, DateTime PlayedAt);
