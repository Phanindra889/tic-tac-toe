using TicTacToe.Api.Models;
using TicTacToe.Api.Services;

namespace TicTacToe.Tests;

public class GameServiceTests
{
    private readonly GameService _service = new();

    // X wins on the top row in a two player game.
    private void PlayXWin(Guid gameId)
    {
        _service.MakeMove(gameId, "X", 0, 0);
        _service.MakeMove(gameId, "O", 1, 0);
        _service.MakeMove(gameId, "X", 0, 1);
        _service.MakeMove(gameId, "O", 1, 1);
        _service.MakeMove(gameId, "X", 0, 2);
    }

    [Fact]
    public void CreateGame_ReturnsFreshGame()
    {
        var game = _service.CreateGame(GameMode.TwoPlayer);

        Assert.NotEqual(Guid.Empty, game.Id);
        Assert.Equal(GameMode.TwoPlayer, game.Mode);
        Assert.Equal(9, game.Board.Length);
        Assert.Equal(GameStatus.InProgress, game.Status);
    }

    [Fact]
    public void GetGame_WithUnknownId_Throws()
    {
        Assert.Throws<GameNotFoundException>(() => _service.GetGame(Guid.NewGuid()));
    }

    [Fact]
    public void ComputerMode_ComputerRepliesAfterHumanMove()
    {
        var game = _service.CreateGame(GameMode.Computer);

        var result = _service.MakeMove(game.Id, "X", 0, 0);

        Assert.Equal(2, result.Moves.Count);
        Assert.Equal("O", result.Moves[1].Player);
        Assert.Equal("O", result.Board[4]); // computer takes the center
        Assert.Equal("X", result.CurrentPlayer);
    }

    [Fact]
    public void ComputerMode_HumanCannotPlayAsO()
    {
        var game = _service.CreateGame(GameMode.Computer);

        Assert.Throws<InvalidMoveException>(() => _service.MakeMove(game.Id, "O", 0, 0));
    }

    [Fact]
    public void ComputerMode_ComputerDoesNotMoveAfterGameEnds()
    {
        var game = _service.CreateGame(GameMode.Computer);

        // Keep playing the first empty cell until the game ends (the computer will win).
        var state = _service.GetGame(game.Id);
        while (state.Status == GameStatus.InProgress)
        {
            var cell = Array.IndexOf(state.Board, null);
            state = _service.MakeMove(game.Id, "X", cell / 3, cell % 3);
        }

        var movesWhenFinished = state.Moves.Count;
        Assert.Throws<InvalidMoveException>(() => _service.MakeMove(game.Id, "X", 0, 0));
        Assert.Equal(movesWhenFinished, _service.GetGame(game.Id).Moves.Count);
    }

    [Fact]
    public void ComputerMode_UndoRemovesBothMoves()
    {
        var game = _service.CreateGame(GameMode.Computer);
        _service.MakeMove(game.Id, "X", 0, 0);

        var result = _service.Undo(game.Id);

        Assert.Empty(result.Moves);
        Assert.Equal("X", result.CurrentPlayer);
        Assert.False(result.CanUndo);
    }

    [Fact]
    public void Scoreboard_IsUpdatedWhenGameIsWon()
    {
        var game = _service.CreateGame(GameMode.TwoPlayer);

        PlayXWin(game.Id);

        var scoreboard = _service.GetScoreboard();
        Assert.Equal(1, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }

    [Fact]
    public void Scoreboard_IsUpdatedOnlyOncePerGame()
    {
        var game = _service.CreateGame(GameMode.TwoPlayer);
        PlayXWin(game.Id);

        // Extra moves and undo after the game is over are rejected.
        Assert.Throws<InvalidMoveException>(() => _service.MakeMove(game.Id, "O", 2, 2));
        Assert.Throws<InvalidMoveException>(() => _service.Undo(game.Id));

        Assert.Equal(1, _service.GetScoreboard().XWins);
    }

    [Fact]
    public void Scoreboard_CountsDraws()
    {
        var game = _service.CreateGame(GameMode.TwoPlayer);
        (int, int)[] moves = [(0, 0), (0, 1), (0, 2), (1, 1), (1, 0), (1, 2), (2, 1), (2, 0), (2, 2)];

        foreach (var (row, column) in moves)
        {
            var current = _service.GetGame(game.Id).CurrentPlayer;
            _service.MakeMove(game.Id, current, row, column);
        }

        Assert.Equal(1, _service.GetScoreboard().Draws);
    }

    [Fact]
    public void ResetGame_KeepsScoreboard()
    {
        var game = _service.CreateGame(GameMode.TwoPlayer);
        PlayXWin(game.Id);

        var result = _service.ResetGame(game.Id);

        Assert.Empty(result.Moves);
        Assert.Equal(GameStatus.InProgress, result.Status);
        Assert.Equal(1, result.Scoreboard.XWins);
    }

    [Fact]
    public void ResetScoreboard_SetsAllCountsToZero()
    {
        var game = _service.CreateGame(GameMode.TwoPlayer);
        PlayXWin(game.Id);

        var scoreboard = _service.ResetScoreboard();

        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }
}
