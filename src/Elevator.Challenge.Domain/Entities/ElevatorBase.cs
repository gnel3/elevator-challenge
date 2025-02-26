using Elevator.Challenge.Domain.Enums;

namespace Elevator.Challenge.Domain.Entities;

public abstract class ElevatorBase(int id, int maxPassengers) : Entity(id)
{
    public int CurrentFloor { get; protected set; } = 1;
    public Direction CurrentDirection { get; protected set; } = Direction.Idle;
    public Status Status { get; protected set; } = Status.Available;
    public int CurrentPassengers { get; protected set; } = 0;
    public int MaxPassengers { get; protected set; } = maxPassengers;
    public List<int> DestinationFloors { get; protected set; } = new();

    public abstract Task MoveAsync(bool simulateMovement, CancellationToken cancellationToken);
    public abstract bool CanAddPassengers(int count);
    public abstract void AddPassengers(int count);
    public abstract void RemovePassengers(int count);
    public abstract void AddDestination(int floor);
}