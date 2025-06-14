namespace RockPaperScissorsLizardSpockGame.Api.DTOs;

/// <summary>
/// Represents a request to play a round of the game, including the player's move and optional email address.
/// </summary>
/// <param name="Player">The integer identifier of the player's chosen move.</param>
/// <param name="Email">The optional email address of the player for tracking scores.</param>
public record PlayGameRequest(int Player, string? Email);
