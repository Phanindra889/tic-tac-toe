using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Dtos;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/scoreboard")]
public class ScoreboardController : ControllerBase
{
    private readonly GameService _gameService;

    public ScoreboardController(GameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet]
    public ActionResult<ScoreboardResponse> GetScoreboard()
    {
        return Ok(_gameService.GetScoreboard());
    }

    [HttpPost("reset")]
    public ActionResult<ScoreboardResponse> ResetScoreboard()
    {
        return Ok(_gameService.ResetScoreboard());
    }
}
