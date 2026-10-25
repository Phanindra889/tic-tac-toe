using TicTacToe.Api.Models;

namespace TicTacToe.Tests;

public class GameTests
{
    // Small helper: plays a list of (row, column) moves, alternating X and O.
    private static Game PlayMoves(params (int Row, int Column)[] moves)
    {
        var game = new Game(GameMode.TwoPlayer);

        foreach (var move in moves)
        {
            game.MakeMove(game.CurrentPlayer, move.Row, move.Column);
        }

        return game;
    }

    [Fact]
    public void NewGame_StartsEmpty_WithXToPlay()
    {
        var game = new Game(GameMode.TwoPlayer);

        Assert.All(game.Board, cell => Assert.Null(cell));
        Assert.Equal("X", game.CurrentPlayer);
        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Empty(game.Moves);
        Assert.False(game.CanUndo);
    }

    [Fact]
    public void ValidMove_PlacesMarkAndAddsToHistory()
    {
        var game = new Game(GameMode.TwoPlayer);

        game.MakeMove("X", 1, 2);

        Assert.Equal("X", game.Board[5]);
        var move = Assert.Single(game.Moves);
        Assert.Equal(new Move(1, "X", 1, 2), move);
    }

    [Fact]
    public void ValidMove_SwitchesTurn()
    {
        var game = new Game(GameMode.TwoPlayer);

        game.MakeMove("X", 0, 0);
        Assert.Equal("O", game.CurrentPlayer);

        game.MakeMove("O", 1, 1);
        Assert.Equal("X", game.CurrentPlayer);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 3)]
    [InlineData(3, 3)]
    public void MoveOutsideBoard_IsRejected(int row, int column)
    {
        var game = new Game(GameMode.TwoPlayer);

        Assert.Throws<InvalidMoveException>(() => game.MakeMove("X", row, column));
        Assert.Empty(game.Moves);
    }

    [Fact]
    public void MoveOnOccupiedCell_IsRejected_AndTurnDoesNotChange()
    {
        var game = PlayMoves((0, 0));

        Assert.Throws<InvalidMoveException>(() => game.MakeMove("O", 0, 0));
        Assert.Equal("O", game.CurrentPlayer);
        Assert.Single(game.Moves);
    }

    [Fact]
    public void MoveByWrongPlayer_IsRejected()
    {
        var game = new Game(GameMode.TwoPlayer);

        Assert.Throws<InvalidMoveException>(() => game.MakeMove("O", 0, 0));
        Assert.Equal("X", game.CurrentPlayer);
    }

    [Fact]
    public void RowWin_IsDetected()
    {
        // X fills the top row, O plays in the middle row.
        var game = PlayMoves((0, 0), (1, 0), (0, 1), (1, 1), (0, 2));

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal("X", game.Winner);
        Assert.Equal([0, 1, 2], game.WinningCells);
    }

    [Fact]
    public void ColumnWin_IsDetected()
    {
        // O fills the middle column.
        var game = PlayMoves((0, 0), (0, 1), (2, 0), (1, 1), (1, 2), (2, 1));

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal("O", game.Winner);
        Assert.Equal([1, 4, 7], game.WinningCells);
    }

    [Fact]
    public void DiagonalWin_IsDetected()
    {
        var game = PlayMoves((0, 0), (0, 1), (1, 1), (0, 2), (2, 2));

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal("X", game.Winner);
        Assert.Equal([0, 4, 8], game.WinningCells);
    }

    [Fact]
    public void AntiDiagonalWin_IsDetected()
    {
        var game = PlayMoves((0, 2), (0, 0), (1, 1), (0, 1), (2, 0));

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal([2, 4, 6], game.WinningCells);
    }

    [Fact]
    public void FullBoardWithNoWinner_IsDraw()
    {
        // X | O | X
        // X | O | O
        // O | X | X
        var game = PlayMoves((0, 0), (0, 1), (0, 2), (1, 1), (1, 0), (1, 2), (2, 1), (2, 0), (2, 2));

        Assert.Equal(GameStatus.Draw, game.Status);
        Assert.Null(game.Winner);
        Assert.Empty(game.WinningCells);
    }

    [Fact]
    public void MoveAfterGameIsWon_IsRejected()
    {
        var game = PlayMoves((0, 0), (1, 0), (0, 1), (1, 1), (0, 2));

        Assert.Throws<InvalidMoveException>(() => game.MakeMove("O", 2, 2));
        Assert.Equal(5, game.Moves.Count);
    }

    [Fact]
    public void Undo_InTwoPlayerMode_RemovesOnlyLastMove()
    {
        var game = PlayMoves((0, 0), (1, 1));

        game.UndoLastMove();

        Assert.Null(game.Board[4]);
        Assert.Equal("X", game.Board[0]);
        Assert.Single(game.Moves);
        Assert.Equal("O", game.CurrentPlayer);
    }

    [Fact]
    public void Undo_InComputerMode_RemovesComputerAndHumanMoves()
    {
        var game = new Game(GameMode.Computer);
        game.MakeMove("X", 0, 0);
        game.MakeMove("O", 1, 1);

        game.UndoLastMove();

        Assert.Empty(game.Moves);
        Assert.All(game.Board, cell => Assert.Null(cell));
        Assert.Equal("X", game.CurrentPlayer);
    }

    [Fact]
    public void Undo_WithNoMoves_IsRejected()
    {
        var game = new Game(GameMode.TwoPlayer);

        Assert.Throws<InvalidMoveException>(() => game.UndoLastMove());
    }

    [Fact]
    public void Undo_AfterGameIsOver_IsRejected()
    {
        var game = PlayMoves((0, 0), (1, 0), (0, 1), (1, 1), (0, 2));

        Assert.False(game.CanUndo);
        Assert.Throws<InvalidMoveException>(() => game.UndoLastMove());
        Assert.Equal(GameStatus.Won, game.Status);
    }

    [Fact]
    public void Reset_ClearsEverything()
    {
        var game = PlayMoves((0, 0), (1, 0), (0, 1), (1, 1), (0, 2));
        var originalId = game.Id;

        game.Reset();

        Assert.Equal(originalId, game.Id);
        Assert.All(game.Board, cell => Assert.Null(cell));
        Assert.Empty(game.Moves);
        Assert.Equal("X", game.CurrentPlayer);
        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Null(game.Winner);
        Assert.Empty(game.WinningCells);
    }
}
