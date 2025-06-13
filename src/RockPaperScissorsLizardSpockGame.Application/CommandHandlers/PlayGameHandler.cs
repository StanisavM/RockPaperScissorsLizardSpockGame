using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RockPaperScissorsLizardSpockGame.Application.Commands;
using RockPaperScissorsLizardSpockGame.Application.DTOs;
using RockPaperScissorsLizardSpockGame.Application.Interfaces;
using RockPaperScissorsLizardSpockGame.Domain.Models;
using RPSLSGame.Domain.Models;

namespace RockPaperScissorsLizardSpockGame.Application.CommandHandlers;
public class PlayGameHandler(IRandomNumberService randomNumberService, ILogger<PlayGameHandler> logger) : IRequestHandler<PlayGameCommand, Result<PlayGameResponse>>
{
    public async Task<Result<PlayGameResponse>> Handle(PlayGameCommand request, CancellationToken ct)
    {
        logger.LogInformation("PlayGameCommand received with player move: {PlayerMove}", request.PlayerMove);

        var randomNumber = await randomNumberService.GetRandomNumber(ct);

        // Get all possible moves and select the computer's move based on random number
        var moves = Enum.GetValues<GameMove>();
        var computerMove = moves[(randomNumber - 1) % moves.Length];

        logger.LogInformation("Computer selected move: {ComputerMove}", computerMove);

        var result = DetermineWinner(request.PlayerMove, computerMove);
        logger.LogInformation("Game result determined: {Result} (Player: {PlayerMove}, Computer: {ComputerMove})", result, request.PlayerMove, computerMove);

        var gameResult = new PlayGameResponse(request.PlayerMove, computerMove, result.ToString().ToLower());

        return Result.Success(gameResult);
    }

    private static GameResult DetermineWinner(GameMove player, GameMove computer)
    {
        if (player == computer)
            return GameResult.Tie;

        bool playerWins = GameRules.DoesMoveWin(player, computer);

        return playerWins ? GameResult.Win : GameResult.Lose;
    }
}
