using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RockPaperScissorsLizardSpockGame.Api.DTOs;
using RockPaperScissorsLizardSpockGame.Application.DTOs;
using RockPaperScissorsLizardSpockGame.Application.Interfaces;

namespace RockPaperScissorsLizardSpockGame.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly IGameService _gameService;

        public GameController(ILogger<GameController> logger, IGameService gameService)
        {
            _logger = logger;
            _gameService = gameService;
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
        public async Task<IActionResult> GetRandomChoice(CancellationToken ct = default)
        {
            _logger.LogInformation("Request received: Getting random game choice.");
            var randomChoice = await _gameService.GetRandomChoice(ct);
            _logger.LogInformation("Random choice selected: {@RandomChoice}", randomChoice);
            return Ok(randomChoice);
        }

        /// <summary>
        /// Plays a round of the game using the player's chosen move and returns the result.
        /// </summary>
        /// <param name="request">An object containing the player's selected move.</param>
        /// <param name="ct">Cancellation token to cancel the request if needed.</param>
        /// <returns>The outcome of the game round, including both player and computer moves.</returns>
        /// <response code="200">Returns the game result when the round completes successfully.</response>
        /// <response code="400">Returned if the player's move is invalid or the request is malformed.</response>
        [HttpPost("play")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PlayGameResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Play([FromBody] PlayGameRequest request, CancellationToken ct = default)
        {
            _logger.LogInformation("Received play request with player move ID: {PlayerMove}", request.Player);

            var playResult = await _gameService.PlayGame(request.Player, ct);

            if (playResult.IsSuccess)
            {
                _logger.LogInformation("Round played successfully: Player {PlayerMove}, Computer {ComputerMove}, Outcome {Result}",
                    playResult.Value.Player, playResult.Value.Computer, playResult.Value.Results);
                return Ok(playResult.Value);
            }

            _logger.LogWarning("Play game request failed: {Error}", playResult.Error);
            return BadRequest(playResult.Error);
        }
    }
}
