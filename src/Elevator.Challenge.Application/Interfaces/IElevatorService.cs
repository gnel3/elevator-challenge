using Elevator.Challenge.Application.Models;
using Elevator.Challenge.Domain.Entities;
using Elevator.Challenge.Domain.Shared;

namespace Elevator.Challenge.Application.Interfaces;

/// <summary>
/// Interface for elevator service operations.
/// </summary>
public interface IElevatorService
{
    /// <summary>
    /// Gets the readonly list of all elevators.
    /// </summary>
    IReadOnlyList<ElevatorBase> Elevators { get; }

    /// <summary>
    /// Calls an elevator based on the specified request.
    /// </summary>
    /// <param name="request">The elevator request containing the floor and passenger information.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the elevator call.</returns>
    Task<Result> CallElevatorAsync(ElevatorRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the nearest elevator to the specified floor.
    /// </summary>
    /// <param name="floor">The floor number to find the nearest elevator to.</param>
    /// <returns>The nearest elevator to the specified floor.</returns>
    ElevatorBase GetNearestElevator(int floor);
}