using Elevator.Challenge.Application.Models;
using Elevator.Challenge.Domain.Entities;
using Elevator.Challenge.Domain.Shared;

namespace Elevator.Challenge.Application.Interfaces;

public interface IElevatorService
{
    IReadOnlyList<ElevatorBase> Elevators { get; }
    Task<Result> CallElevatorAsync(ElevatorRequest request, CancellationToken cancellationToken);
    Task UpdateElevatorsAsync(CancellationToken cancellationToken);
    ElevatorBase GetNearestElevator(int floor);
}