namespace TicTacToe.Api.Models;

public class Scoreboard
{
    public int XWins { get; private set; }
    public int OWins { get; private set; }
    public int Draws { get; private set; }

    public void RecordResult(Game game)
    {
        if (game.Status == GameStatus.Draw)
        {
            Draws++;
        }
        else if (game.Winner == "X")
        {
            XWins++;
        }
        else if (game.Winner == "O")
        {
            OWins++;
        }
    }

    public void Reset()
    {
        XWins = 0;
        OWins = 0;
        Draws = 0;
    }
}
