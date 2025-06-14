using RockPaperScissorsLizardSpockGame.Domain.Models;

namespace RockPaperScissorsLizardSpockGame.Application.Interfaces;

public interface IScoreboardService
{
    Task AddEntryAsync(ScoreEntry entry, CancellationToken ct = default);
    Task<List<ScoreEntry>> GetRecentEntries(string? email = null, int limit = 10, CancellationToken ct = default);
    Task ResetScoreboardAsync(string email, CancellationToken ct = default);
}
