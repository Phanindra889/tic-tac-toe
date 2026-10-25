namespace TicTacToe.Api.Models;

// The controller turns this into a 404 Not Found.
public class GameNotFoundException : Exception
{
    public GameNotFoundException(Guid id) : base($"Game {id} was not found.")
    {
    }
}
