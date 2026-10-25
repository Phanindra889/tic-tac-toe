namespace TicTacToe.Api.Models;

// Row and Column are zero-based (0 to 2). The UI shows them as 1 to 3.
public record Move(int MoveNumber, string Player, int Row, int Column);
