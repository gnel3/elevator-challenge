namespace Elevator.Challenge.Application.Settings;

public class ElevatorSettings
{
    public int NumberOfElevators { get; init; }
    public int NumberOfFloors { get; init; }
    public int MaxPassengers { get; init; }
    public bool SimulateMovement { get; init; }
}