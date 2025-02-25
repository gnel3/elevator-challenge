using Elevator.Challenge.Application.Interfaces;
using Elevator.Challenge.Application.Models;
using Elevator.Challenge.Application.Settings;
using Elevator.Challenge.Domain.Entities;
using Elevator.Challenge.Domain.Enums;

namespace Elevator.Challenge.Application.Services;

public class ElevatorService : IElevatorService
{
    private readonly List<ElevatorBase> _elevators;
    
    private readonly ElevatorSettings _settings;
    
    public IReadOnlyList<ElevatorBase> Elevators => _elevators.AsReadOnly();

    public ElevatorService(ElevatorSettings settings)
    {
        _settings = settings;
        
        _elevators = Enumerable.Range(1, settings.NumberOfElevators)
            .Select(id => new PassengerElevator(id, settings.MaxPassengers))
            .Cast<ElevatorBase>()
            .ToList();
    }

    public async Task<ElevatorResult> CallElevatorAsync(ElevatorRequest request, CancellationToken cancellationToken)
    {
        if (request.FromFloor < 1 || request.FromFloor > _settings.NumberOfFloors || 
            request.ToFloor < 1 || request.ToFloor > _settings.NumberOfFloors)
        {
            return ElevatorResult.Failure($"Floor numbers must be between 1 and {_settings.NumberOfFloors}");
        }
        
        if (request.Passengers <= 0 || request.Passengers > _settings.MaxPassengers)
        {
            return ElevatorResult.Failure($"Number of passengers must be between 1 and {_settings.MaxPassengers}");
        }

        var elevator = GetNearestElevator(request.FromFloor);
        
        if (!elevator.CanAddPassengers(request.Passengers))
        {
            return ElevatorResult.Failure("Too many passengers for a single elevator");
        }

        elevator.AddDestination(request.FromFloor);
        await elevator.MoveAsync(cancellationToken);
        
        elevator.AddPassengers(request.Passengers);
        elevator.AddDestination(request.ToFloor);
        await elevator.MoveAsync(cancellationToken);
        
        elevator.RemovePassengers(request.Passengers);
        
        return ElevatorResult.Success();
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
}