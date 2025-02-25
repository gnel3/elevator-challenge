using Elevator.Challenge.Application.Models;
using Elevator.Challenge.Domain.Entities;

namespace Elevator.Challenge.Application.Interfaces;

public interface IElevatorService
{
    IReadOnlyList<ElevatorBase> Elevators { get; }
    Task CallElevatorAsync(ElevatorRequest request);
    Task UpdateElevatorsAsync();
    ElevatorBase GetNearestElevator(int floor);
}