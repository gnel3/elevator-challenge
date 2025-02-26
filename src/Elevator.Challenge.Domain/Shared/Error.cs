namespace Elevator.Challenge.Domain.Shared;

public class Error(string code, string message)
{
    public static readonly Error None = new Error(string.Empty, string.Empty);
    public string Code { get; } = code;
    public string Message { get; } = message;
}