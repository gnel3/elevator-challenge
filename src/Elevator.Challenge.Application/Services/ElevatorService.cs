using Elevator.Challenge.Application.Interfaces;
using Elevator.Challenge.Application.Models;
using Elevator.Challenge.Application.Settings;
using Elevator.Challenge.Domain.Entities;
using Elevator.Challenge.Domain.Enums;
using Elevator.Challenge.Domain.Errors;
using Elevator.Challenge.Domain.Shared;
using Microsoft.Extensions.Options;

namespace Elevator.Challenge.Application.Services;

public class ElevatorService : IElevatorService
{
    private readonly List<ElevatorBase> _elevators;
    private readonly ElevatorSettings _settings;
    
    public IReadOnlyList<ElevatorBase> Elevators => _elevators.AsReadOnly();

    public ElevatorService(IOptions<ElevatorSettings> settings)
    {
        // Get the actual settings value from the IOptions
        _settings = settings.Value;

        _elevators = Enumerable.Range(1, _settings.NumberOfElevators)
            .Select(id => new PassengerElevator(id, _settings.MaxPassengers))
            .Cast<ElevatorBase>()
            .ToList();
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
            
            var remainingPassengers = request.Passengers;

            while (remainingPassengers > 0)
            {
                var callElevatorTasks = new List<Task>();

                for (var i = 0; i < _elevators.Count && remainingPassengers > 0; i++)
                {
                    var passengersForThisElevator = Math.Min(remainingPassengers, _settings.MaxPassengers);
                    var newRequest = new ElevatorRequest(
                        request.FromFloor,
                        request.ToFloor,
                        passengersForThisElevator);

                    remainingPassengers -= passengersForThisElevator;

                    callElevatorTasks.Add(HandleElevatorRequestAsync(newRequest, cancellationToken));
                }

                await Task.WhenAll(callElevatorTasks);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Elevator.CallElevator", ex.Message));
        }
    }

    public async Task UpdateElevatorsAsync(CancellationToken cancellationToken)
    {
        var tasks = _elevators
            .Where(e => e.Status == Status.Moving)
            .Select(e => e.MoveAsync(cancellationToken));

        await Task.WhenAll(tasks);
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
        await elevator.MoveAsync(cancellationToken);
        elevator.AddPassengers(request.Passengers);
        elevator.AddDestination(request.ToFloor);
        await elevator.MoveAsync(cancellationToken);
        elevator.RemovePassengers(request.Passengers);
    }
}