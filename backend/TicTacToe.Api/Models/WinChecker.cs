namespace TicTacToe.Api.Models;

// The board is stored as 9 cells, numbered like this:
//
//   0 | 1 | 2
//   ---------
//   3 | 4 | 5
//   ---------
//   6 | 7 | 8
//
public static class WinChecker
{
    private static readonly int[][] WinningLines =
    [
        [0, 1, 2], [3, 4, 5], [6, 7, 8], // rows
        [0, 3, 6], [1, 4, 7], [2, 5, 8], // columns
        [0, 4, 8], [2, 4, 6]             // diagonals
    ];

    // Returns the three winning cell indexes, or null if nobody has won yet.
    public static int[]? FindWinningLine(string?[] board)
    {
        foreach (var line in WinningLines)
        {
            var first = board[line[0]];

            if (first != null && first == board[line[1]] && first == board[line[2]])
            {
                return line;
            }
        }

        return null;
    }
}
