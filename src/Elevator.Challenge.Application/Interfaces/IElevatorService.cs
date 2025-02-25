using Elevator.Challenge.Application.Models;
using Elevator.Challenge.Domain.Entities;

namespace Elevator.Challenge.Application.Interfaces;

public interface IElevatorService
{
    IReadOnlyList<ElevatorBase> Elevators { get; }
    Task<ElevatorResult> CallElevatorAsync(ElevatorRequest request, CancellationToken cancellationToken);
    Task UpdateElevatorsAsync(CancellationToken cancellationToken);
    ElevatorBase GetNearestElevator(int floor);
}