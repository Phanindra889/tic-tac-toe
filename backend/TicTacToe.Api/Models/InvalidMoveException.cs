namespace TicTacToe.Api.Models;

// Thrown when a player tries to do something the rules don't allow.
// The controller turns this into a 400 Bad Request.
public class InvalidMoveException : Exception
{
    public InvalidMoveException(string message) : base(message)
    {
    }
}
