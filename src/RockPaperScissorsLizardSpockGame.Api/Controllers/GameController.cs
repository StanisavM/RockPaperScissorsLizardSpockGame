using Microsoft.AspNetCore.Mvc;
using RockPaperScissorsLizardSpockGame.Api.DTOs;
using RockPaperScissorsLizardSpockGame.Application.DTOs;
using RockPaperScissorsLizardSpockGame.Application.Interfaces;

namespace RockPaperScissorsLizardSpockGame.Api.Controllers
{
    /// <summary>
    /// API controller for managing Rock-Paper-Scissors-Lizard-Spock game operations,
    /// including retrieving choices, playing rounds, and handling the scoreboard.
    /// </summary>
    [ApiController]
    [Route("")]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly IGameService _gameService;
        private readonly IScoreboardService _scoreboardService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging controller operations.</param>
        /// <param name="gameService">Service for handling game logic and operations.</param>
        /// <param name="scoreboardService">Service for managing scoreboard data and operations.</param>
        public GameController(ILogger<GameController> logger, IGameService gameService, IScoreboardService scoreboardService)
        {
            _logger = logger;
            _gameService = gameService;
            _scoreboardService = scoreboardService;
        }

        /// <summary>
        /// Retrieves all the valid moves available in the Rock-Paper-Scissors-Lizard-Spock game.
        /// </summary>
        /// <param name="ct">Cancellation token to abort the operation if needed.</param>
        /// <returns>A JSON array containing all possible game moves.</returns>
        /// <response code="200">Returns the list of all game moves.</response>
        [HttpGet("choices")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<GameChoiceDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChoices(CancellationToken ct = default)
        {
            _logger.LogInformation("Request received: Fetching list of game choices.");
            var choices = await _gameService.GetGameChoices(ct);
            _logger.LogInformation("Returning {Count} choices.", choices?.Count() ?? 0);
            return Ok(choices);
        }

        /// <summary>
        /// Retrieves a single, randomly selected move from the list of valid game moves.
        /// </summary>
        /// <param name="ct">Cancellation token to cancel the request if necessary.</param>
        /// <returns>A randomly selected game move.</returns>
        /// <response code="200">Returns the random game move selected.</response>
        [HttpGet("choice")]
        [ProducesResponseType(typeof(GameChoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetRandomChoice(CancellationToken ct = default)
        {
            _logger.LogInformation("Request received: Getting random game choice.");
            var randomChoice = await _gameService.GetRandomChoice(ct);
            _logger.LogInformation("Random choice selected: {@RandomChoice}", randomChoice);
            return Ok(randomChoice);
        }

        /// <summary>
        /// Plays a round of the game using the player's chosen move and returns the result.
        /// PlayGameResponse includes players moves, results, as well as random FunFact for this game.
        /// </summary>
        /// <param name="request">An object containing the player's selected move.</param>
        /// <param name="ct">Cancellation token to cancel the request if needed.</param>
        /// <returns>The outcome of the game round, including both player and computer moves.</returns>
        /// <response code="200">Returns the game result when the round completes successfully.</response>
        /// <response code="400">Returned if the player's move is invalid or the request is malformed.</response>
        [HttpPost("play")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PlayGameResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Play([FromBody] PlayGameRequest request, CancellationToken ct = default)
        {
            _logger.LogInformation("Received play request with player moveId: {PlayerMove} and email: {Email}", request.Player, request.Email);

            var playResult = await _gameService.PlayGame(request.Player, ct, request.Email);

            if (playResult.IsSuccess)
            {
                _logger.LogInformation("Round played successfully: Player {PlayerMove}, Email:{Email}, Computer {ComputerMove}, Outcome {Result}",
                    playResult.Value.Player, request.Email, playResult.Value.Computer, playResult.Value.Results);
                return Ok(playResult.Value);
            }

            _logger.LogWarning("Play game request failed: {Error}", playResult.Error);
            return BadRequest(playResult.Error);
        }

        /// <summary>
        /// Retrieves the 10 most recent game results for the specified user if email is provided, or 10 most recent global game results if email is not provided.
        /// </summary>
        /// <param name="email">The email address used to identify the user.</param>
        /// <param name="ct"></param>
        /// <returns>A JSON array containing up to 10 of the most recent game results.</returns>
        /// <response code="200">Returns the list of recent game results for the user if email provided, or global scoreboard.</response>
        /// <response code="500">Returned if there is an unexpected error accessing the scoreboard data.</response>
        [HttpGet("scoreboard")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<ScoreEntryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetScoreboard([FromQuery] string? email = null, CancellationToken ct = default)
        {
            // TODO: Secure this endpoint with JWT Bearer authentication.
            // Currently, the `email` is provided via query parameter to identify the user.
            // Improve this so that:
            // - This endpoint should require a valid JWT Bearer token.
            // - The email should be extracted from the token claims (e.g., user identity).
            // - The authenticated user should only be allowed to fetch their own scoreboard.
            // - Admin users (based on role/claim in JWT) may be allowed to access any user's scoreboard.
            var scores = await _scoreboardService.GetRecentEntries(email, ct:ct);
            return Ok(scores.Select(s => new ScoreEntryDto(s.PlayerEmail, s.PlayerMove, s.ComputerMove, s.Result, s.PlayedAt)));
        }

        /// <summary>
        /// Resets the scoreboard for the specified user by deleting all saved game results.
        /// </summary>
        /// <param name="email">The email address used to identify the user whose scoreboard should be cleared.</param>
        /// <param name="ct"></param>
        /// <returns>A confirmation message indicating the scoreboard has been reset.</returns>
        /// <response code="200">Indicates the scoreboard was successfully reset.</response>
        /// <response code="400">Returned if the email is null, empty, or invalid.</response>
        /// <response code="500">Returned if there is an unexpected error while resetting the scoreboard.</response>
        [HttpPost("scoreboard/reset")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetScoreboard([FromQuery] string email, CancellationToken ct = default)
        {
            // TODO: Secure this endpoint with JWT Bearer authentication.
            // Currently, email field is provided in the request body to identify the user.
            // Improve this so that:
            // - This endpoint should require a valid JWT Bearer token.
            // - The email should be extracted from the token claims instead of being passed explicitly.
            // - The authenticated user should only be allowed to reset their own scoreboard.
            // - Admin users (based on role/claim in JWT) may be allowed to reset any user's scoreboard.
            await _scoreboardService.ResetScoreboardAsync(email);
            return Ok("Scoreboard reset.");
        }
    }
}
