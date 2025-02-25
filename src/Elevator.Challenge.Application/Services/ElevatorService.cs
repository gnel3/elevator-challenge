using Elevator.Challenge.Application.Interfaces;
using Elevator.Challenge.Application.Models;
using Elevator.Challenge.Application.Settings;
using Elevator.Challenge.Domain.Entities;
using Elevator.Challenge.Domain.Enums;

namespace Elevator.Challenge.Application.Services;

public class ElevatorService : IElevatorService
{
    private readonly List<ElevatorBase> _elevators;
    public IReadOnlyList<ElevatorBase> Elevators => _elevators.AsReadOnly();

    public ElevatorService(ElevatorSettings settings)
    {
        _elevators = Enumerable.Range(1, settings.NumberOfElevators)
            .Select(id => new PassengerElevator(id, settings.MaxPassengers))
            .Cast<ElevatorBase>()
            .ToList();
    }

    public async Task CallElevatorAsync(ElevatorRequest request)
    {
        var elevator = GetNearestElevator(request.FromFloor);
        
        if (!elevator.CanAddPassengers(request.Passengers))
            throw new InvalidOperationException("Too many passengers for a single elevator");

        elevator.AddDestination(request.FromFloor);
        await elevator.MoveAsync();
        
        elevator.AddPassengers(request.Passengers);
        elevator.AddDestination(request.ToFloor);
        await elevator.MoveAsync();
        
        elevator.RemovePassengers(request.Passengers);
    }

    public async Task UpdateElevatorsAsync()
    {
        var tasks = _elevators
            .Where(e => e.Status == Status.Moving)
            .Select(e => e.MoveAsync());
        
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