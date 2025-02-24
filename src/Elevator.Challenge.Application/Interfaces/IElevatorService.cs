using Elevator.Challenge.Domain.Entities;

namespace Elevator.Challenge.Application.Interfaces;

public interface IElevatorService
{
    IReadOnlyList<ElevatorBase> Elevators { get; }
    Task CallElevatorAsync(int fromFloor, int toFloor, int passengers);
    Task UpdateElevatorsAsync();
    ElevatorBase GetNearestElevator(int floor);
}