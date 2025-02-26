namespace Elevator.Challenge.Domain.Shared;

/// <summary>
/// Represents an error with a code and a message.
/// </summary>
public class Error
{
    /// <summary>
    /// Gets a static instance representing no error.
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty);
    
    /// <summary>
    /// Gets the error code.
    /// </summary>
    public string Code { get; }
    
    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }
    
}