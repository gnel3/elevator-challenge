using Elevator.Challenge.Application.Interfaces;
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

    public async Task CallElevatorAsync(int fromFloor, int toFloor, int passengers)
    {
        var elevator = GetNearestElevator(fromFloor);
        
        if (!elevator.CanAddPassengers(passengers))
            throw new InvalidOperationException("Too many passengers for a single elevator");

        elevator.AddDestination(fromFloor);
        await elevator.MoveAsync();
        
        elevator.AddPassengers(passengers);
        elevator.AddDestination(toFloor);
        await elevator.MoveAsync();
        
        elevator.RemovePassengers(passengers);
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