using TicTacToe.Api.Models;

namespace TicTacToe.Api.Dtos;

public record CreateGameRequest(GameMode Mode);

// The game id comes from the URL: POST /api/games/{id}/moves
public record MoveRequest(string Player, int Row, int Column);
