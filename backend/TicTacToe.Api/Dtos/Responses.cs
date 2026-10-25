using TicTacToe.Api.Models;

namespace TicTacToe.Api.Dtos;

public record ScoreboardResponse(int XWins, int OWins, int Draws)
{
    public static ScoreboardResponse From(Scoreboard scoreboard)
    {
        return new ScoreboardResponse(scoreboard.XWins, scoreboard.OWins, scoreboard.Draws);
    }
}

// Everything the frontend needs to draw the screen.
public record GameResponse(
    Guid Id,
    GameMode Mode,
    string?[] Board,
    string CurrentPlayer,
    GameStatus Status,
    string? Winner,
    int[] WinningCells,
    List<Move> Moves,
    bool CanUndo,
    ScoreboardResponse Scoreboard)
{
    // Copies the arrays so the response can't change if the game changes later.
    public static GameResponse From(Game game, Scoreboard scoreboard)
    {
        return new GameResponse(
            game.Id,
            game.Mode,
            game.Board.ToArray(),
            game.CurrentPlayer,
            game.Status,
            game.Winner,
            game.WinningCells.ToArray(),
            game.Moves.ToList(),
            game.CanUndo,
            ScoreboardResponse.From(scoreboard));
    }
}
