using Elevator.Challenge.Application.Models;
using Elevator.Challenge.Domain.Entities;
using Elevator.Challenge.Domain.Enums;

namespace Elevator.Challenge.Presentation.Services;

public static class ConsoleDisplayService
{
    public static void ShowMessage(string message, ConsoleColor consoleColor)
    {
        Console.ForegroundColor = consoleColor;
        Console.WriteLine();
        Console.WriteLine(message);
        Console.WriteLine();
        Console.ResetColor();
    }

    public static void ShowStatus(IEnumerable<ElevatorBase> elevators)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== Elevator Status ===");
        foreach (var elevator in elevators)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Elevator #{elevator.Id}");
            Console.ResetColor();
            Console.WriteLine($"Current Floor: {elevator.CurrentFloor}");
            Console.ForegroundColor = elevator.Status switch
            {
                Status.Available => ConsoleColor.Green,
                Status.Moving => ConsoleColor.Blue,
                Status.Stopped => ConsoleColor.Yellow,
                _ => ConsoleColor.Red
            };
            Console.WriteLine($"Status: {elevator.Status}");
            Console.ResetColor();
            Console.WriteLine($"Direction: {elevator.CurrentDirection}");
            Console.WriteLine($"Passengers: {elevator.CurrentPassengers}/{elevator.MaxPassengers}");
            Console.WriteLine($"Destinations: {string.Join(", ", elevator.DestinationFloors)}");
            Console.WriteLine("===========================");
        }
    }

    public static ElevatorRequest GetRequest()
    {
        Console.Write("Enter the floor number where the elevator is called from: ");

        if (!int.TryParse(Console.ReadLine(), out var fromFloor))
        {
            Console.WriteLine("Please enter a valid floor number");
        }

        Console.Write("Enter the floor number where the elevator should go to: ");

        if (!int.TryParse(Console.ReadLine(), out var toFloor))
        {
            Console.WriteLine("Please enter a valid floor number");
        }

        Console.Write("Enter the number of passengers on the elevator: ");

        if (!int.TryParse(Console.ReadLine(), out var passengers))
        {
            Console.WriteLine("Please enter a valid floor number");
        }

        return new ElevatorRequest(fromFloor, toFloor, passengers);
    }
}