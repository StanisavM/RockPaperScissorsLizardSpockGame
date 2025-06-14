using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using RockPaperScissorsLizardSpockGame.Application.Commands;
using RockPaperScissorsLizardSpockGame.Application.DTOs;
using RockPaperScissorsLizardSpockGame.Application.Helpers;
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
        var index = Math.Abs((randomNumber - 1) % moves.Length);
        var computerMove = moves[index];

        logger.LogInformation("Raw random number: {RandomNumber}, Computed index: {Index}, Computer selected move: {ComputerMove}",randomNumber, index, computerMove);

        var result = DetermineWinner(request.PlayerMove, computerMove);
        logger.LogInformation("Game result determined: {Result} (Player: {PlayerMove}, Computer: {ComputerMove})", result, request.PlayerMove, computerMove);

        var gameResult = new PlayGameResponse(request.PlayerMove, computerMove, result.ToString().ToLower(), PlayGameResponseHelpers.GetRandomFunFact());

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
