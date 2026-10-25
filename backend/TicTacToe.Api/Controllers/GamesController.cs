using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Dtos;
using TicTacToe.Api.Models;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly GameService _gameService;

    public GamesController(GameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost]
    public ActionResult<GameResponse> CreateGame(CreateGameRequest request)
    {
        var game = _gameService.CreateGame(request.Mode);
        return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<GameResponse> GetGame(Guid id)
    {
        return HandleErrors(() => _gameService.GetGame(id));
    }

    [HttpPost("{id:guid}/moves")]
    public ActionResult<GameResponse> MakeMove(Guid id, MoveRequest request)
    {
        return HandleErrors(() => _gameService.MakeMove(id, request.Player, request.Row, request.Column));
    }

    [HttpPost("{id:guid}/undo")]
    public ActionResult<GameResponse> Undo(Guid id)
    {
        return HandleErrors(() => _gameService.Undo(id));
    }

    [HttpPost("{id:guid}/reset")]
    public ActionResult<GameResponse> ResetGame(Guid id)
    {
        return HandleErrors(() => _gameService.ResetGame(id));
    }

    // Turns our game exceptions into the right HTTP status codes.
    private ActionResult<GameResponse> HandleErrors(Func<GameResponse> action)
    {
        try
        {
            return Ok(action());
        }
        catch (GameNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidMoveException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
