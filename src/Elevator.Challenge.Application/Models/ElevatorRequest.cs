namespace Elevator.Challenge.Application.Models;

public class ElevatorRequest(int fromFloor, int toFloor, int passengers)
{
    public int FromFloor { get; set; } = fromFloor;
    public int ToFloor { get; set; } = toFloor;
    public int Passengers { get; set; } = passengers;
}