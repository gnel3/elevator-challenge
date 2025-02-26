using Elevator.Challenge.Application.Models;
using Elevator.Challenge.Domain.Entities;
using Elevator.Challenge.Domain.Enums;

namespace Elevator.Challenge.Presentation.Services;

public static class ConsoleDisplayService
{
    /// <summary>
    /// Displays a message in the specified console color.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="consoleColor">The color to use for the message.</param>
    public static void DisplayMessage(string message, ConsoleColor consoleColor)
    {
        Console.ForegroundColor = consoleColor;
        Console.WriteLine();
        Console.WriteLine(message);
        Console.WriteLine();
        Console.ResetColor();
    }

    /// <summary>
    /// Displays the status of all elevators.
    /// </summary>
    /// <param name="elevators">The list of elevators to display the status for.</param>
    public static void DisplayStatus(IEnumerable<ElevatorBase> elevators)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;
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
            Console.ForegroundColor = elevator.CurrentDirection switch
            {
                Direction.Down => ConsoleColor.Cyan,
                Direction.Up => ConsoleColor.Cyan,
                Direction.Idle => ConsoleColor.White,
                _ => ConsoleColor.Red
            };
            Console.WriteLine($"Direction: {elevator.CurrentDirection}");
            Console.ResetColor();
            Console.WriteLine($"Passengers: {elevator.CurrentPassengers}/{elevator.MaxPassengers}");
            Console.WriteLine($"Destinations: {string.Join(", ", elevator.DestinationFloors)}");
            Console.WriteLine("===========================");
        }
    }

    /// <summary>
    /// Gets an elevator request from user input.
    /// </summary>
    /// <returns>An ElevatorRequest object containing the user's input.</returns>
    public static ElevatorRequest GetElevatorRequestFromUserInput()
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