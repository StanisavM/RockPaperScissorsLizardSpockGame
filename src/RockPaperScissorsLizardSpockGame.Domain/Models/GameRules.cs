using RockPaperScissorsLizardSpockGame.Domain.Models;

namespace RPSLSGame.Domain.Models;

public static class GameRules
{
    // Maps each move to the set of moves it can defeat
    private static readonly Dictionary<GameMove, HashSet<GameMove>> _winningMoves = new()
    {
        { GameMove.Rock, new HashSet<GameMove> { GameMove.Scissors, GameMove.Lizard } },
        { GameMove.Paper, new HashSet<GameMove> { GameMove.Rock, GameMove.Spock } },
        { GameMove.Scissors, new HashSet<GameMove> { GameMove.Paper, GameMove.Lizard } },
        { GameMove.Lizard, new HashSet<GameMove> { GameMove.Spock, GameMove.Paper } },
        { GameMove.Spock, new HashSet<GameMove> { GameMove.Rock, GameMove.Scissors } }
    };

    // Expose an immutable view of the wining moves dictionary
    private static IReadOnlyDictionary<GameMove, HashSet<GameMove>> WinningMoves => _winningMoves;

    /// <summary>
    /// Determines whether the attacker's move beats the defender's move
    /// according to the predefined game rules.
    /// </summary>
    /// <param name="attacker">The move made by the attacking player.</param>
    /// <param name="defender">The move made by the defending player.</param>
    /// <returns>True if the attacker’s move defeats the defender’s move; otherwise, false.</returns>
    public static bool DoesMoveWin(GameMove attacker, GameMove defender) => WinningMoves.TryGetValue(attacker, out var defeats) && defeats.Contains(defender);
}