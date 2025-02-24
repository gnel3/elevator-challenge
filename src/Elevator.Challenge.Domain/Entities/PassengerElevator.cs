using Elevator.Challenge.Domain.Enums;

namespace Elevator.Challenge.Domain.Entities;

public class PassengerElevator(int id) : ElevatorBase(id)
{
    public override async Task MoveAsync()
    {
        if (DestinationFloors.Count == 0)
        {
            return;
        }

        Status = Status.Moving;
        var nextFloor = DestinationFloors[0];
        CurrentDirection = nextFloor > CurrentFloor ? Direction.Up : Direction.Down;

        // Simulate movement time
        await Task.Delay(Math.Abs(nextFloor - CurrentFloor) * 1000);

        CurrentFloor = nextFloor;
        DestinationFloors.RemoveAt(0);

        if (DestinationFloors.Count == 0)
        {
            Status = Status.Available;
            CurrentDirection = Direction.Idle;
        }
    }

    public override bool CanAddPassengers(int count)
    {
        return CurrentPassengers + count <= MaxPassengers;
    }

    public override void AddPassengers(int count)
    {
        if (!CanAddPassengers(count))
            throw new InvalidOperationException("Elevator capacity exceeded");

        CurrentPassengers += count;
    }

    public override void RemovePassengers(int count)
    {
        if (CurrentPassengers - count < 0)
            throw new InvalidOperationException("Cannot remove more passengers than present");

        CurrentPassengers -= count;
    }

    public override void AddDestination(int floor)
    {
        if (DestinationFloors.Contains(floor)) return;
        
        DestinationFloors.Add(floor);
        DestinationFloors.Sort();
    }
}