namespace Elevator.Challenge.Application.Models;

public class ElevatorResult
{
    public bool IsSuccess { get; }
    public string ErrorMessage { get; }

    private ElevatorResult(bool isSuccess, string errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static ElevatorResult Success() => new ElevatorResult(true, string.Empty);
    public static ElevatorResult Failure(string errorMessage) => new ElevatorResult(false, errorMessage);
}