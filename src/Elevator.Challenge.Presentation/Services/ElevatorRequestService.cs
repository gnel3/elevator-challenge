using Elevator.Challenge.Application.Interfaces;
using Elevator.Challenge.Application.Models;
using Elevator.Challenge.Domain.Shared;

namespace Elevator.Challenge.Presentation.Services;

public static class ElevatorRequestService
{
    /// <summary>
    /// Handles an elevator request by calling the elevator and updating the status display.
    /// </summary>
    /// <param name="elevatorService">The elevator service to handle the request.</param>
    /// <param name="elevatorRequestFromUserInput">The elevator request from user input.</param>
    /// <param name="cancellationToken">The cancellation token to handle task cancellation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the elevator call.</returns>
     public static async Task<Result> HandleElevatorRequest(
        IElevatorService elevatorService,
        ElevatorRequest elevatorRequestFromUserInput,
        CancellationToken cancellationToken)
    {
        Console.Clear();

        var callElevatorTask = Task.Run(async () => await elevatorService.CallElevatorAsync(
            elevatorRequestFromUserInput,
            cancellationToken), cancellationToken);

        // Task to update the status display while the elevator is being called
        var statusUpdateTask = Task.Run(() =>
        {
            while (!callElevatorTask.IsCompleted)
            {
                ConsoleDisplayService.DisplayStatus(elevatorService.Elevators, true);
            }
            
            return Task.CompletedTask;
        }, cancellationToken);

        await statusUpdateTask;

        // Return the result of the elevator call
        return await callElevatorTask;
    }
}