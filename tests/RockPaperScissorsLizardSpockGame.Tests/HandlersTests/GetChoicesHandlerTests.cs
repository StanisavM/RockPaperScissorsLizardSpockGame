using RockPaperScissorsLizardSpockGame.Application.Handlers;
using RockPaperScissorsLizardSpockGame.Application.Helpers;
using RockPaperScissorsLizardSpockGame.Application.Queries;

namespace RockPaperScissorsLizardSpockGame.Tests.HandlersTests;

public class GetChoicesHandlerTests
{

    [Fact]
    public async Task Handle_WhenCalled_ReturnsAllExpectedChoices()
    {
        // Arrange
        var handler = new GetChoicesHandler();
        var query = new GetChoicesQuery();
        var cancellationToken = CancellationToken.None;

        var expectedChoices = GameMovesHelpers.Choices.ToList();

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedChoices.Count, result.Count);

        foreach (var expected in expectedChoices)
        {
            Assert.Contains(result, actual =>
                actual.Id == expected.Id &&
                actual.Name == expected.Name);
        }
    }
}

