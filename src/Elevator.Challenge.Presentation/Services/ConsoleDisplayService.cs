using Elevator.Challenge.Application.Models;
using Elevator.Challenge.Domain.Entities;
using Elevator.Challenge.Domain.Enums;
using Elevator.Challenge.Presentation.Extensions;

namespace Elevator.Challenge.Presentation.Services;

public static class ConsoleDisplayService
{
    /// <summary>
    /// This needs to change to match the number of lines displayed for each elevator status in DisplayStatus.
    /// </summary>
    private const int StatusHeightSetting = 7;

    /// <summary>
    /// Displays a message in the specified console color.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="consoleColor">The color to use for the message.</param>
    /// <param name="statusHeight">The height of the status display.</param>
    public static void DisplayMessage(string message, ConsoleColor consoleColor, int statusHeight = 0)
    {
        var cursorTop = statusHeight + 1;
        if (cursorTop >= Console.BufferHeight)
        {
            cursorTop = Console.BufferHeight - 1;
        }
        
        Console.SetCursorPosition(0, cursorTop);
        Console.ForegroundColor = consoleColor;
        ConsoleExtensions.WritePadded(message);
        Console.ResetColor();
    }

    /// <summary>
    /// Calculates the height required to display the status of all elevators.
    /// </summary>
    /// <param name="elevatorCount">The number of elevators.</param>
    /// <returns>The height required to display the status of all elevators.</returns>
    public static int CalculateStatusHeight(int elevatorCount)
    {
        // Start the iteration height at 2 because of the welcome message and status message
        var statusHeight = 2;
        
        // Add the height of the status display for each elevator
        statusHeight += elevatorCount * StatusHeightSetting;
        
        return statusHeight;
    }

    /// <summary>
    /// Displays the status of each elevator in the console, including its ID, current floor, status, direction,
    /// number of passengers, and destination floors.
    /// </summary>
    /// <param name="elevators">The collection of elevators to display the status for.</param>
    /// <param name="addProgress">Indicates whether to add progress information to the display.</param>
    public static void DisplayStatus(IEnumerable<ElevatorBase> elevators, bool addProgress = false)
    {
        Console.SetCursorPosition(0, 0);
        ConsoleExtensions.WritePadded("Welcome to the Elevator Simulation System");
        Console.SetCursorPosition(0, 1);
        ConsoleExtensions.WritePadded("Press Ctrl+C to exit");
        Console.SetCursorPosition(0, 2);
        Console.ForegroundColor = ConsoleColor.White;
        ConsoleExtensions.WritePadded("=== Elevator Status ===");
        
        // Start the iteration height at 2 because of the welcome message and status message
        var iterationHeight = 2;
        
        foreach (var elevator in elevators)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(0, iterationHeight + 1);
            ConsoleExtensions.WritePadded($"Elevator #{elevator.Id}");
            Console.ResetColor();
            Console.SetCursorPosition(0, iterationHeight + 2);
            ConsoleExtensions.WritePadded($"Current Floor: {elevator.CurrentFloor}");
            Console.ForegroundColor = elevator.Status switch
            {
                Status.Available => ConsoleColor.Green,
                Status.Moving => ConsoleColor.Blue,
                Status.Stopped => ConsoleColor.Yellow,
                _ => ConsoleColor.Red
            };
            Console.SetCursorPosition(0, iterationHeight + 3);
            ConsoleExtensions.WritePadded($"Status: {elevator.Status}");
            Console.ResetColor();
            Console.ForegroundColor = elevator.CurrentDirection switch
            {
                Direction.Down => ConsoleColor.Cyan,
                Direction.Up => ConsoleColor.Cyan,
                Direction.Idle => ConsoleColor.White,
                _ => ConsoleColor.Red
            };
            Console.SetCursorPosition(0, iterationHeight + 4);
            ConsoleExtensions.WritePadded($"Direction: {elevator.CurrentDirection}");
            Console.ResetColor();
            Console.SetCursorPosition(0, iterationHeight + 5);
            ConsoleExtensions.WritePadded($"Passengers: {elevator.CurrentPassengers}/{elevator.MaxPassengers}");
            Console.SetCursorPosition(0, iterationHeight + 6);
            ConsoleExtensions.WritePadded($"Destinations: {string.Join(", ", elevator.DestinationFloors)}");
            Console.SetCursorPosition(0, iterationHeight + 7);
            ConsoleExtensions.WritePadded("===========================");

            iterationHeight += 7;
        }

        Console.SetCursorPosition(0, iterationHeight + 1);
        
        if (addProgress)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleExtensions.WritePadded("Working...");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            ConsoleExtensions.WritePadded("Waiting for input...");
        }

        Console.ResetColor();
    }

    
    /// <summary>
    /// Gets an elevator request from user input.
    /// </summary>
    /// <param name="statusHeight">The height of the status display.</param>
    /// <returns>An ElevatorRequest object containing the user's input.</returns>
    public static ElevatorRequest GetElevatorRequestFromUserInput(int statusHeight)
    {
        int fromFloor;
        while (true)
        {
            Console.SetCursorPosition(0, statusHeight + 2);
            DisplayRequestMessage("Enter the floor number where the elevator is called ", "from", ": ");
            if (int.TryParse(Console.ReadLine(), out fromFloor))
            {
                break;
            }
            ConsoleExtensions.WritePadded("Please enter a valid floor number.");
        }

        int toFloor;
        while (true)
        {
            Console.SetCursorPosition(0, statusHeight + 3);
            DisplayRequestMessage("Enter the floor number where the elevator should go ", "to", ": ");
            if (int.TryParse(Console.ReadLine(), out toFloor))
            {
                break;
            }
            ConsoleExtensions.WritePadded("Please enter a valid floor number.");
        }

        int passengers;
        while (true)
        {
            Console.SetCursorPosition(0, statusHeight + 4);
            DisplayRequestMessage("Enter the number of", " passengers ", "on the elevator: ");
            if (int.TryParse(Console.ReadLine(), out passengers))
            {
                break;
            }
            ConsoleExtensions.WritePadded("Please enter a valid number of passengers.");
        }

        return new ElevatorRequest(fromFloor, toFloor, passengers);
    }
    
    /// <summary>
    /// Displays a request message with highlighted parts.
    /// </summary>
    /// <param name="firstPart">The first part of the message.</param>
    /// <param name="highlightedPart">The highlighted part of the message.</param>
    /// <param name="lastPart">The last part of the message.</param>
    private static void DisplayRequestMessage(string firstPart, string highlightedPart, string lastPart)
    {
        Console.Write(firstPart);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(highlightedPart);
        Console.ResetColor();
        Console.Write(lastPart);
    }
}