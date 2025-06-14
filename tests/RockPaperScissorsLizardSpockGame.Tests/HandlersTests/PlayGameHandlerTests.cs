using Microsoft.Extensions.Logging;
using Moq;
using RockPaperScissorsLizardSpockGame.Application.CommandHandlers;
using RockPaperScissorsLizardSpockGame.Application.Commands;
using RockPaperScissorsLizardSpockGame.Application.DTOs;
using RockPaperScissorsLizardSpockGame.Application.Interfaces;
using RockPaperScissorsLizardSpockGame.Domain.Models;

public class PlayGameHandlerTests
{
    private readonly Mock<IRandomNumberService> _mockRandomNumberService;
    private readonly Mock<IScoreboardService> _mockScoreboardService;
    private readonly Mock<ILogger<PlayGameHandler>> _mockLogger;
    private readonly PlayGameHandler _handler;

    public PlayGameHandlerTests()
    {
        _mockRandomNumberService = new Mock<IRandomNumberService>();
        _mockScoreboardService = new Mock<IScoreboardService>();
        _mockLogger = new Mock<ILogger<PlayGameHandler>>();
        _handler = new PlayGameHandler(_mockRandomNumberService.Object, _mockLogger.Object, _mockScoreboardService.Object);
    }

    [Theory]
    [InlineData(GameMove.Rock, GameMove.Scissors, "win")]
    [InlineData(GameMove.Rock, GameMove.Lizard, "win")]
    [InlineData(GameMove.Paper, GameMove.Rock, "win")]
    [InlineData(GameMove.Paper, GameMove.Spock, "win")]
    [InlineData(GameMove.Scissors, GameMove.Paper, "win")]
    [InlineData(GameMove.Scissors, GameMove.Lizard, "win")]
    [InlineData(GameMove.Lizard, GameMove.Spock, "win")]
    [InlineData(GameMove.Lizard, GameMove.Paper, "win")]
    [InlineData(GameMove.Spock, GameMove.Rock, "win")]
    [InlineData(GameMove.Spock, GameMove.Scissors, "win")]

    [InlineData(GameMove.Rock, GameMove.Paper, "lose")]
    [InlineData(GameMove.Rock, GameMove.Spock, "lose")]
    [InlineData(GameMove.Paper, GameMove.Scissors, "lose")]
    [InlineData(GameMove.Paper, GameMove.Lizard, "lose")]
    [InlineData(GameMove.Scissors, GameMove.Rock, "lose")]
    [InlineData(GameMove.Scissors, GameMove.Spock, "lose")]
    [InlineData(GameMove.Lizard, GameMove.Rock, "lose")]
    [InlineData(GameMove.Lizard, GameMove.Scissors, "lose")]
    [InlineData(GameMove.Spock, GameMove.Paper, "lose")]
    [InlineData(GameMove.Spock, GameMove.Lizard, "lose")]

    [InlineData(GameMove.Rock, GameMove.Rock, "tie")]
    [InlineData(GameMove.Paper, GameMove.Paper, "tie")]
    [InlineData(GameMove.Scissors, GameMove.Scissors, "tie")]
    [InlineData(GameMove.Lizard, GameMove.Lizard, "tie")]
    [InlineData(GameMove.Spock, GameMove.Spock, "tie")]
    public async Task Handle_IfPlayerAndComputerMakeMoves_ReturnsExpectedResult(GameMove player, GameMove computer, string expectedResult)
    {
        // Arrange
        _mockRandomNumberService.Setup(x => x.GetRandomNumber(It.IsAny<CancellationToken>()))
            .ReturnsAsync((int)computer);
        var command = new PlayGameCommand(player);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedResult, result.Value.Results);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100)]
    public async Task Handle_InvalidRandomNumber_WrapsIndexCorrectly(int random)
    {
        // Arrange
        _mockRandomNumberService.Setup(x => x.GetRandomNumber(It.IsAny<CancellationToken>()))
            .ReturnsAsync(random);
        var command = new PlayGameCommand(GameMove.Rock);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.IsType<PlayGameResponse>(result.Value);
    }

    [Fact]
    public async Task Handle_AlwaysReturnsDefinedGameResult()
    {
        // Arrange
        _mockRandomNumberService.Setup(x => x.GetRandomNumber(It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);
        var command = new PlayGameCommand(GameMove.Paper);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Contains(result.Value.Results, new[] { "win", "lose", "tie" });
    }
}
