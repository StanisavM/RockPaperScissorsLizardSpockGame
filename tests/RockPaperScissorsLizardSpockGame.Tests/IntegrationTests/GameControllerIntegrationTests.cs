using Microsoft.VisualStudio.TestPlatform.TestHost;
using RockPaperScissorsLizardSpockGame.Api.DTOs;
using RockPaperScissorsLizardSpockGame.Application.DTOs;
using RockPaperScissorsLizardSpockGame.Tests.IntegrationTests.Factories;
using System.Net.Http.Json;
namespace RockPaperScissorsLizardSpockGame.IntegrationTests.Controllers;

[Collection("Integration Tests")]
public class GameControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public GameControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _factory.SetupValidResponse();
        _client = _factory.CreateClient();
    }

    [Fact(Skip = "Integration test temporarily disabled due to testhost.deps.json issue.")]
    public async Task GetChoices_ReturnsListOfChoices()
    {
        var response = await _client.GetAsync("/choices");

        response.EnsureSuccessStatusCode();
        var choices = await response.Content.ReadFromJsonAsync<List<GameChoiceDto>>();

        Assert.NotNull(choices);
        Assert.True(choices.Count >= 5);
    }

    [Fact(Skip = "Integration test temporarily disabled due to testhost.deps.json issue.")]
    public async Task GetRandomChoice_ReturnsValidChoice()
    {
        var response = await _client.GetAsync("/choice");

        response.EnsureSuccessStatusCode();
        var choice = await response.Content.ReadFromJsonAsync<GameChoiceDto>();

        Assert.NotNull(choice);
        Assert.True(choice.Id > 0);
    }

    [Fact(Skip = "Integration test temporarily disabled due to testhost.deps.json issue.")]
    public async Task Play_ValidMove_ReturnsGameResult()
    {
        var request = new PlayGameRequest(1, "test@example.com");

        var response = await _client.PostAsJsonAsync("/play", request);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PlayGameResponse>();

        Assert.NotNull(result);
        Assert.Equal(request.Player, (int)result.Player);
        Assert.False(string.IsNullOrEmpty(result.Results));
    }

    [Fact(Skip = "Integration test temporarily disabled due to testhost.deps.json issue.")]
    public async Task GetScoreboard_WithoutEmail_ReturnsGlobalScores()
    {
        var response = await _client.GetAsync("/scoreboard");

        response.EnsureSuccessStatusCode();
        var scores = await response.Content.ReadFromJsonAsync<List<ScoreEntryDto>>();

        Assert.NotNull(scores);
        Assert.True(scores.Count <= 10);
    }

    [Fact(Skip = "Integration test temporarily disabled due to testhost.deps.json issue.")]
    public async Task ResetScoreboard_WithEmail_ClearsUserScores()
    {
        var email = "test@example.com";
        var response = await _client.PostAsync($"/scoreboard/reset?email={email}", null);

        response.EnsureSuccessStatusCode();
        var message = await response.Content.ReadAsStringAsync();

        Assert.Equal("Scoreboard reset.", message);
    }

    public void Dispose()
    {
        _factory?.Dispose();
    }
}
