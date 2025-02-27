using Elevator.Challenge.Domain.Enums;
using Elevator.Challenge.Domain.Shared;

namespace Elevator.Challenge.Domain.Entities;

/// <summary>
/// Represents the base class for an elevator.
/// </summary>
public abstract class ElevatorBase : Entity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ElevatorBase"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the elevator.</param>
    /// <param name="maxPassengers">The maximum number of passengers the elevator can hold.</param>
    protected ElevatorBase(int id, int maxPassengers) : base(id)
    {
        MaxPassengers = maxPassengers;
    }

    /// <summary>
    /// Gets or sets the current floor of the elevator.
    /// </summary>
    public int CurrentFloor { get; protected set; } = 1;

    /// <summary>
    /// Gets or sets the current direction of the elevator.
    /// </summary>
    public Direction CurrentDirection { get; protected set; } = Direction.Idle;

    /// <summary>
    /// Gets or sets the status of the elevator.
    /// </summary>
    public Status Status { get; protected set; } = Status.Available;

    /// <summary>
    /// Gets or sets the current number of passengers in the elevator.
    /// </summary>
    public int CurrentPassengers { get; protected set; } = 0;

    /// <summary>
    /// Gets the maximum number of passengers the elevator can hold.
    /// </summary>
    public int MaxPassengers { get; protected set; }

    /// <summary>
    /// Gets the list of destination floors for the elevator.
    /// </summary>
    public List<int> DestinationFloors { get; protected set; } = [];

    /// <summary>
    /// Moves the elevator asynchronously.
    /// </summary>
    /// <param name="simulateMovement">Indicates whether to simulate movement.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public abstract Task MoveAsync(bool simulateMovement, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether passengers can be added to the elevator.
    /// </summary>
    /// <param name="count">The number of passengers to add.</param>
    /// <returns><c>true</c> if passengers can be added; otherwise, <c>false</c>.</returns>
    public abstract bool CanAddPassengers(int count);

    /// <summary>
    /// Adds passengers to the elevator.
    /// </summary>
    /// <param name="count">The number of passengers to add.</param>
    /// <returns>A result indicating success or failure.</returns>
    public abstract Result AddPassengers(int count);

    /// <summary>
    /// Removes passengers from the elevator.
    /// </summary>
    /// <param name="count">The number of passengers to remove.</param>
    /// <returns>A result indicating success or failure.</returns>
    public abstract Result RemovePassengers(int count);

    /// <summary>
    /// Adds a destination floor to the elevator.
    /// </summary>
    /// <param name="floor">The floor to add.</param>
    public abstract void AddDestination(int floor);
}