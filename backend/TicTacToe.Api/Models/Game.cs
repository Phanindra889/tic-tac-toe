namespace TicTacToe.Api.Models;

// Holds the state of one game and enforces the rules.
// It knows nothing about HTTP or the scoreboard.
public class Game
{
    public const int BoardSize = 3;

    public Guid Id { get; } = Guid.NewGuid();
    public GameMode Mode { get; }

    // null means the cell is empty, otherwise "X" or "O".
    public string?[] Board { get; private set; } = new string?[BoardSize * BoardSize];

    public string CurrentPlayer { get; private set; } = "X";
    public GameStatus Status { get; private set; } = GameStatus.InProgress;
    public string? Winner { get; private set; }
    public int[] WinningCells { get; private set; } = [];
    public List<Move> Moves { get; } = new();

    // We chose "Option A": once a game is finished, undo is disabled,
    // so the scoreboard result for that game is final.
    public bool CanUndo => Status == GameStatus.InProgress && Moves.Count > 0;

    public Game(GameMode mode)
    {
        Mode = mode;
    }

    public void MakeMove(string player, int row, int column)
    {
        if (Status != GameStatus.InProgress)
        {
            throw new InvalidMoveException("The game is already over. Start a new game to keep playing.");
        }

        if (player != CurrentPlayer)
        {
            throw new InvalidMoveException($"It is {CurrentPlayer}'s turn, not {player}'s.");
        }

        if (row < 0 || row >= BoardSize || column < 0 || column >= BoardSize)
        {
            throw new InvalidMoveException("That move is outside the board.");
        }

        var index = row * BoardSize + column;

        if (Board[index] != null)
        {
            throw new InvalidMoveException("That cell is already taken.");
        }

        Board[index] = player;
        Moves.Add(new Move(Moves.Count + 1, player, row, column));

        UpdateStatus();
    }

    public void UndoLastMove()
    {
        if (!CanUndo)
        {
            throw new InvalidMoveException("There is no move to undo.");
        }

        var removedMove = RemoveLastMove();

        // Against the computer, undo takes back the computer's move (O)
        // and the human's move (X) before it, so it is X's turn again.
        if (Mode == GameMode.Computer && removedMove.Player == "O" && Moves.Count > 0)
        {
            RemoveLastMove();
        }

        UpdateStatus();
    }

    public void Reset()
    {
        Board = new string?[BoardSize * BoardSize];
        Moves.Clear();
        CurrentPlayer = "X";
        Status = GameStatus.InProgress;
        Winner = null;
        WinningCells = [];
    }

    private Move RemoveLastMove()
    {
        var lastMove = Moves[^1];
        Moves.RemoveAt(Moves.Count - 1);
        Board[lastMove.Row * BoardSize + lastMove.Column] = null;
        return lastMove;
    }

    // Works out the status from the board, so it is always correct
    // after a move or an undo.
    private void UpdateStatus()
    {
        var winningLine = WinChecker.FindWinningLine(Board);

        if (winningLine != null)
        {
            Status = GameStatus.Won;
            Winner = Board[winningLine[0]];
            WinningCells = winningLine;
        }
        else if (Moves.Count == Board.Length)
        {
            Status = GameStatus.Draw;
            Winner = null;
            WinningCells = [];
        }
        else
        {
            Status = GameStatus.InProgress;
            Winner = null;
            WinningCells = [];
        }

        // X always goes first, so an even number of moves means it's X's turn.
        CurrentPlayer = Moves.Count % 2 == 0 ? "X" : "O";
    }
}
