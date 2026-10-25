using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services;

// A simple computer opponent that always plays as O.
public static class ComputerPlayer
{
    private const int Center = 4;
    private static readonly int[] Corners = [0, 2, 6, 8];

    // Picks a cell index (0 to 8) using this priority:
    // 1. Win if possible
    // 2. Block X from winning
    // 3. Take the center
    // 4. Take a corner
    // 5. Take any empty cell
    public static int ChooseCell(string?[] board)
    {
        var winningCell = FindWinningCell(board, "O");
        if (winningCell != null)
        {
            return winningCell.Value;
        }

        var blockingCell = FindWinningCell(board, "X");
        if (blockingCell != null)
        {
            return blockingCell.Value;
        }

        if (board[Center] == null)
        {
            return Center;
        }

        foreach (var corner in Corners)
        {
            if (board[corner] == null)
            {
                return corner;
            }
        }

        return Array.IndexOf(board, null);
    }

    // Tries each empty cell and checks if playing there would win the game for this player.
    private static int? FindWinningCell(string?[] board, string player)
    {
        for (var i = 0; i < board.Length; i++)
        {
            if (board[i] != null)
            {
                continue;
            }

            var testBoard = (string?[])board.Clone();
            testBoard[i] = player;

            if (WinChecker.FindWinningLine(testBoard) != null)
            {
                return i;
            }
        }

        return null;
    }
}
