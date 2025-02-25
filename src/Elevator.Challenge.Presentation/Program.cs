using Elevator.Challenge.Application.Models;
using Elevator.Challenge.Application.Services;
using Elevator.Challenge.Application.Settings;
using Elevator.Challenge.Domain.Entities;
using Elevator.Challenge.Domain.Enums;

var settings = new ElevatorSettings
{
    NumberOfFloors = 10,
    NumberOfElevators = 3,
    MaxPassengers = 20
};

Console.Title = "Elevator Simulation System";
Console.Clear();
Console.WriteLine("Welcome to the Elevator Simulation System");
Console.WriteLine($"Building has {settings.NumberOfElevators} floors and {settings.NumberOfElevators} elevators");
Console.WriteLine("Press Ctrl+C to exit");
Console.WriteLine();

using var cancellationTokenSource = new CancellationTokenSource();
var elevatorService = new ElevatorService(settings);

ShowStatus(elevatorService.Elevators);

while (!cancellationTokenSource.Token.IsCancellationRequested)
{
    try
    {
        var elevatorRequest = GetRequest();

        var callElevatorTask = Task.Run(async () =>
        {
            var elevatorResult =
                await elevatorService.CallElevatorAsync(elevatorRequest, cancellationTokenSource.Token);
            
            if (!elevatorResult.IsSuccess)
            {
                Console.WriteLine(elevatorResult.ErrorMessage);
            }

            return elevatorResult;
        }, cancellationTokenSource.Token);

        var statusUpdateTask = Task.Run(async () =>
        {
            while (!callElevatorTask.IsCompleted)
            {
                ShowStatus(elevatorService.Elevators);
                await Task.Delay(1000); // Update UI every second
            }
        }, cancellationTokenSource.Token);

        await statusUpdateTask;

        var result = await callElevatorTask;


        if (result.IsSuccess)
        {
            ShowStatus(elevatorService.Elevators);
            ShowMessage("Press 'Q' to quit or any other key to make another request.", ConsoleColor.Yellow);

            if (Console.ReadKey().Key == ConsoleKey.Q)
            {
                cancellationTokenSource.Cancel();
                break;
            }
        }
        else
        {
            ShowMessage(result.ErrorMessage, ConsoleColor.Red);
        }
    }
    catch (Exception ex)
    {
        ShowMessage(ex.Message, ConsoleColor.Red);
    }
}

// Handle graceful shutdown
Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    cancellationTokenSource.Cancel();
};

return;

static void ShowMessage(string message , ConsoleColor consoleColor)
{
    Console.ForegroundColor = consoleColor;
    Console.WriteLine();
    Console.WriteLine(message);
    Console.WriteLine();
    Console.ResetColor();
}   

static void ShowStatus(IEnumerable<ElevatorBase> elevators)
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

static ElevatorRequest GetRequest()
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