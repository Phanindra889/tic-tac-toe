using TicTacToe.Api.Dtos;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services;

// Keeps all games and the scoreboard in memory.
// It is registered as a singleton, so the data lives as long as the API is running.
public class GameService
{
    private readonly Dictionary<Guid, Game> _games = new();
    private readonly Scoreboard _scoreboard = new();

    // One simple lock is enough for a local app and keeps requests from stepping on each other.
    private readonly object _lock = new();

    public GameResponse CreateGame(GameMode mode)
    {
        lock (_lock)
        {
            var game = new Game(mode);
            _games[game.Id] = game;
            return GameResponse.From(game, _scoreboard);
        }
    }

    public GameResponse GetGame(Guid id)
    {
        lock (_lock)
        {
            return GameResponse.From(FindGame(id), _scoreboard);
        }
    }

    public GameResponse MakeMove(Guid id, string player, int row, int column)
    {
        lock (_lock)
        {
            var game = FindGame(id);

            // In computer mode the human is always X.
            if (game.Mode == GameMode.Computer && player != "X")
            {
                throw new InvalidMoveException("In computer mode you play as X.");
            }

            game.MakeMove(player, row, column);

            if (game.Mode == GameMode.Computer && game.Status == GameStatus.InProgress)
            {
                var cell = ComputerPlayer.ChooseCell(game.Board);
                game.MakeMove("O", cell / Game.BoardSize, cell % Game.BoardSize);
            }

            // MakeMove throws if the game was already over, so if it is over now,
            // it finished during this request. That way each game is counted only once.
            if (game.Status != GameStatus.InProgress)
            {
                _scoreboard.RecordResult(game);
            }

            return GameResponse.From(game, _scoreboard);
        }
    }

    public GameResponse Undo(Guid id)
    {
        lock (_lock)
        {
            var game = FindGame(id);
            game.UndoLastMove();
            return GameResponse.From(game, _scoreboard);
        }
    }

    public GameResponse ResetGame(Guid id)
    {
        lock (_lock)
        {
            var game = FindGame(id);
            game.Reset();
            return GameResponse.From(game, _scoreboard);
        }
    }

    public ScoreboardResponse GetScoreboard()
    {
        lock (_lock)
        {
            return ScoreboardResponse.From(_scoreboard);
        }
    }

    public ScoreboardResponse ResetScoreboard()
    {
        lock (_lock)
        {
            _scoreboard.Reset();
            return ScoreboardResponse.From(_scoreboard);
        }
    }

    private Game FindGame(Guid id)
    {
        if (!_games.TryGetValue(id, out var game))
        {
            throw new GameNotFoundException(id);
        }

        return game;
    }
}
