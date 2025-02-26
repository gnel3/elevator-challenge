namespace Elevator.Challenge.Application.Models;

/// <summary>
/// Represents a request to call an elevator.
/// </summary>
public class ElevatorRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ElevatorRequest"/> class.
    /// </summary>
    /// <param name="fromFloor">The floor from which the elevator is requested.</param>
    /// <param name="toFloor">The floor to which the elevator is requested to go.</param>
    /// <param name="passengers">The number of passengers requesting the elevator.</param>
    public ElevatorRequest(int fromFloor, int toFloor, int passengers)
    {
        FromFloor = fromFloor;
        ToFloor = toFloor;
        Passengers = passengers;
    }

    /// <summary>
    /// Gets the floor from which the elevator is requested.
    /// </summary>
    public int FromFloor { get; }

    /// <summary>
    /// Gets the floor to which the elevator is requested to go.
    /// </summary>
    public int ToFloor { get; }

    /// <summary>
    /// Gets the number of passengers requesting the elevator.
    /// </summary>
    public int Passengers { get; }
}