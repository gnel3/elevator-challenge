using Elevator.Challenge.Domain.Enums;
using Elevator.Challenge.Domain.Errors;
using Elevator.Challenge.Domain.Shared;

namespace Elevator.Challenge.Domain.Entities;

/// <summary>
/// Represents a passenger elevator with specific functionality for moving, adding, and removing passengers.
/// </summary>
public class PassengerElevator : ElevatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PassengerElevator"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the elevator.</param>
    /// <param name="maxPassengers">The maximum number of passengers the elevator can hold.</param>
    public PassengerElevator(int id, int maxPassengers) : base(id, maxPassengers)
    {
    }

    public override async Task MoveAsync(bool simulateMovement, CancellationToken cancellationToken)
    {
        try
        {
            if (DestinationFloors.Count == 0)
            {
                return;
            }

            var nextFloor = DestinationFloors[0];

            CurrentDirection = nextFloor > CurrentFloor ? Direction.Up : Direction.Down;
            Status = Status.Moving;

            // Simulate movement by delaying the task
            if (simulateMovement)
            {
                await Task.Delay(Math.Abs(nextFloor - CurrentFloor) * 1000, cancellationToken);
            }

            // Check if the operation has been canceled
            cancellationToken.ThrowIfCancellationRequested();

            CurrentFloor = nextFloor;
            DestinationFloors.RemoveAt(0);

            if (DestinationFloors.Count == 0)
            {
                Status = Status.Available;
                CurrentDirection = Direction.Idle;
            }
        }
        catch (OperationCanceledException)
        {
            // Handle operation cancellation
            Status = Status.Available;
            CurrentDirection = Direction.Idle;
        }
        catch (Exception)
        {
            Status = Status.Available;
            CurrentDirection = Direction.Idle;
        }
    }

    public override bool CanAddPassengers(int count)
    {
        return CurrentPassengers + count <= MaxPassengers;
    }

    public override Result AddPassengers(int count)
    {
        if (!CanAddPassengers(count))
        {
            return Result.Failure(DomainErrors.Elevator.MaxPassengersExceeded);
        }

        CurrentPassengers += count;

        return Result.Success();
    }

    public override Result RemovePassengers(int count)
    {
        if (CurrentPassengers - count < 0)
        {
            return Result.Failure(DomainErrors.Elevator.RemovePassengers);
        }

        CurrentPassengers -= count;

        return Result.Success();
    }

    public override void AddDestination(int floor)
    {
        if (DestinationFloors.Contains(floor))
        {
            return;
        }

        DestinationFloors.Add(floor);
        DestinationFloors.Sort();
    }
}