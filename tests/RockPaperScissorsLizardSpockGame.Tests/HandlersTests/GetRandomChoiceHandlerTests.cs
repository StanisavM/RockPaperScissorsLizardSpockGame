using Moq;
using RockPaperScissorsLizardSpockGame.Application.DTOs;
using RockPaperScissorsLizardSpockGame.Application.Handlers;
using RockPaperScissorsLizardSpockGame.Application.Helpers;
using RockPaperScissorsLizardSpockGame.Application.Interfaces;
using RockPaperScissorsLizardSpockGame.Application.Queries;

namespace RockPaperScissorsLizardSpockGame.Tests.Handlers;

public class GetRandomChoiceHandlerTests
{
    private readonly Mock<IRandomNumberService> _mockRandomService;
    private readonly GetRandomChoiceHandler _handler;
    private readonly IReadOnlyList<GameChoiceDto> _choices;

    public GetRandomChoiceHandlerTests()
    {
        _mockRandomService = new Mock<IRandomNumberService>();
        _handler = new GetRandomChoiceHandler(_mockRandomService.Object);
        _choices = GameMovesHelpers.Choices;
    }

    [Fact]
    public async Task Handle_IfValidRandomNumberReturned_ReturnsCorrectChoice()
    {
        // Arrange
        int index = 1;
        int randomNumber = index + 1;
        _mockRandomService.Setup(x => x.GetRandomNumber(It.IsAny<CancellationToken>()))
            .ReturnsAsync(randomNumber);

        var expected = _choices[index];

        // Act
        var result = await _handler.Handle(new GetRandomChoiceQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(expected.Id, result.Id);
        Assert.Equal(expected.Name, result.Name);
    }

    [Fact]
    public async Task Handle_IfLargeRandomNumberReturned_UsesModuloToWrapAround()
    {
        // Arrange
        int randomNumber = 999;
        int expectedIndex = (randomNumber - 1) % _choices.Count;
        var expected = _choices[expectedIndex];

        _mockRandomService.Setup(x => x.GetRandomNumber(It.IsAny<CancellationToken>()))
            .ReturnsAsync(randomNumber);

        // Act
        var result = await _handler.Handle(new GetRandomChoiceQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(expected.Id, result.Id);
        Assert.Equal(expected.Name, result.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task Handle_IfZeroOrNegativeRandomNumberReturned_StillReturnsValidChoice(int randomNumber)
    {
        // Arrange
        int index = Math.Abs(randomNumber - 1) % _choices.Count;
        var expected = _choices[index];

        _mockRandomService.Setup(x => x.GetRandomNumber(It.IsAny<CancellationToken>()))
            .ReturnsAsync(randomNumber);

        // Act
        var result = await _handler.Handle(new GetRandomChoiceQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(expected.Id, result.Id);
        Assert.Equal(expected.Name, result.Name);
    }

    [Fact]
    public async Task Handle_ReturnedChoice_IsInDefinedGameMovesList()
    {
        // Arrange
        _mockRandomService.Setup(x => x.GetRandomNumber(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        var result = await _handler.Handle(new GetRandomChoiceQuery(), CancellationToken.None);

        // Assert
        Assert.Contains(_choices, c => c.Id == result.Id && c.Name == result.Name);
    }

    [Fact]
    public async Task Handle_IfMultipleConsecutiveCalls_ReturnDifferentChoicesOverTime()
    {
        // Arrange
        var randomNumbers = new Queue<int>(new[] { 1, 2, 3, 4, 5 });

        _mockRandomService.Setup(x => x.GetRandomNumber(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => randomNumbers.Dequeue());

        var results = new List<GameChoiceDto>();

        // Act
        for (int i = 0; i < 5; i++)
        {
            var choice = await _handler.Handle(new GetRandomChoiceQuery(), CancellationToken.None);
            results.Add(choice);
        }

        // Assert
        Assert.Equal(5, results.Select(c => c.Id).Distinct().Count());
    }
}
