using LiteDB;
using Microsoft.Extensions.Options;
using RockPaperScissorsLizardSpockGame.Application.Interfaces;
using RockPaperScissorsLizardSpockGame.Domain.Models;

namespace RockPaperScissorsLizardSpockGame.Infrastructure.Services;

public class LiteDBScoreboardService : IScoreboardService
{
    // Introduced LiteDB as a lightweight local database to persist game results.

    private readonly ILiteCollection<ScoreEntry> _collection;

    public LiteDBScoreboardService(IOptions<LiteDbSettings> settings)
    {
        var db = new LiteDatabase(settings.Value.DbName);
        _collection = db.GetCollection<ScoreEntry>(settings.Value.CollectionName);
        _collection.EnsureIndex(x => x.PlayerEmail);
    }

    public Task AddEntryAsync(ScoreEntry entry, CancellationToken ct = default)
    {
        _collection.Insert(entry);
        return Task.CompletedTask;
    }

    public Task<List<ScoreEntry>> GetRecentEntries(string? email, int limit = 10, CancellationToken ct = default)
    {
        // Query all entries if no email provided; otherwise filter by email
        var query = string.IsNullOrWhiteSpace(email)
            ? _collection.Query()
            : _collection.Query().Where(x => x.PlayerEmail == email);

        var entries = query
            .OrderByDescending(x => x.PlayedAt)
            .Limit(limit)
            .ToList();

        return Task.FromResult(entries);
    }

    public Task ResetScoreboardAsync(string email, CancellationToken ct = default)
    {
        _collection.DeleteMany(x => x.PlayerEmail == email);
        return Task.CompletedTask;
    }
}