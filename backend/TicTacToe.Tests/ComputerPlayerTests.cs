using TicTacToe.Api.Services;

namespace TicTacToe.Tests;

public class ComputerPlayerTests
{
    // Builds a board from a 9-character string, e.g. "XO-------".
    // '-' means an empty cell.
    private static string?[] Board(string cells)
    {
        return cells.Select(c => c == '-' ? null : c.ToString()).ToArray();
    }

    [Fact]
    public void TakesWinningMove_WhenOCanWin()
    {
        // O can win at cell 5, X could also win at cell 2. Winning comes first.
        var board = Board("XX-OO-X--");

        Assert.Equal(5, ComputerPlayer.ChooseCell(board));
    }

    [Fact]
    public void BlocksX_WhenXIsAboutToWin()
    {
        var board = Board("XX--O----");

        Assert.Equal(2, ComputerPlayer.ChooseCell(board));
    }

    [Fact]
    public void TakesCenter_WhenNothingToWinOrBlock()
    {
        var board = Board("X--------");

        Assert.Equal(4, ComputerPlayer.ChooseCell(board));
    }

    [Fact]
    public void TakesCorner_WhenCenterIsTaken()
    {
        var board = Board("----X----");

        Assert.Equal(0, ComputerPlayer.ChooseCell(board));
    }

    [Fact]
    public void TakesAnyCell_WhenCenterAndCornersAreTaken()
    {
        // X | O | X
        // - | X | -
        // O | X | O
        // Only cells 3 and 5 are free, and neither wins or blocks.
        var board = Board("XOX-X-OXO");

        Assert.Equal(3, ComputerPlayer.ChooseCell(board));
    }
}
