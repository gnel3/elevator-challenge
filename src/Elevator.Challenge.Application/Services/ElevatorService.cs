using Elevator.Challenge.Application.Interfaces;
using Elevator.Challenge.Application.Models;
using Elevator.Challenge.Application.Settings;
using Elevator.Challenge.Domain.Entities;
using Elevator.Challenge.Domain.Enums;
using Elevator.Challenge.Domain.Errors;
using Elevator.Challenge.Domain.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Elevator.Challenge.Application.Services;

public class ElevatorService : IElevatorService
{
    private readonly List<ElevatorBase> _elevators = [];
    private readonly ElevatorSettings _settings = new();
    private readonly ILogger<ElevatorService> _logger;

    public IReadOnlyList<ElevatorBase> Elevators => _elevators.AsReadOnly();

    public ElevatorService(IOptions<ElevatorSettings> settings, ILogger<ElevatorService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        try
        {
            // Get the actual settings value from the IOptions
            _settings = settings.Value;

            // Initialize the list of elevators based on the settings and use the range for ids
            _elevators = Enumerable.Range(1, _settings.NumberOfElevators)
                .Select(id => new PassengerElevator(id, _settings.MaxPassengers))
                .Cast<ElevatorBase>()
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing ElevatorService");
        }
    }

    public async Task<Result> CallElevatorAsync(ElevatorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.FromFloor < 1 || request.FromFloor > _settings.NumberOfFloors ||
                request.ToFloor < 1 || request.ToFloor > _settings.NumberOfFloors)
            {
                return Result.Failure(DomainErrors.Elevator.CheckFloor);
            }

            if (request.Passengers < 1)
            {
                return Result.Failure(DomainErrors.Elevator.CheckPassengers);
            }

            // If there are more passengers than the max allowed, split them across multiple elevators
            if (request.Passengers <= _settings.MaxPassengers)
            {
                await HandleElevatorRequestAsync(request, cancellationToken);
            }
            else
            {
                var remainingPassengers = request.Passengers;

                // Handle the elevator request for the specified number of passengers
                while (remainingPassengers > 0)
                {
                    var callElevatorTasks = new List<Task>();

                    // Loop through the elevators and call them based on the number of passengers and elevator count
                    // If all elevators are busy, continue to run all tasks in the list, and come back to this loop
                    // for the remaining passengers
                    for (var i = 0; i < _elevators.Count && remainingPassengers > 0; i++)
                    {
                        var passengersForThisElevator = Math.Min(remainingPassengers, _settings.MaxPassengers);

                        var additionalRequest = new ElevatorRequest(
                            request.FromFloor,
                            request.ToFloor,
                            passengersForThisElevator);

                        remainingPassengers -= passengersForThisElevator;

                        // Add the task to the list so multiple elevators can be called at the same time
                        callElevatorTasks.Add(HandleElevatorRequestAsync(additionalRequest, cancellationToken));
                    }

                    await Task.WhenAll(callElevatorTasks);
                }
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling elevator");
            return Result.Failure(new Error("Elevator.CallElevator", ex.Message));
        }
    }

    public ElevatorBase GetNearestElevator(int floor)
    {
        return _elevators
            .Where(e => e.Status == Status.Available)
            .OrderBy(e => Math.Abs(e.CurrentFloor - floor))
            .First();
    }

    private async Task HandleElevatorRequestAsync(ElevatorRequest request, CancellationToken cancellationToken)
    {
        var elevator = GetNearestElevator(request.FromFloor);
        elevator.AddDestination(request.FromFloor);
        await elevator.MoveAsync(_settings.SimulateMovement, cancellationToken);
        elevator.AddPassengers(request.Passengers);
        elevator.AddDestination(request.ToFloor);
        await elevator.MoveAsync(_settings.SimulateMovement, cancellationToken);
        elevator.RemovePassengers(request.Passengers);
    }
}