using Elevator.Challenge.Application;
using Elevator.Challenge.Application.Interfaces;
using Elevator.Challenge.Presentation.Extensions;
using Elevator.Challenge.Presentation.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

IElevatorService elevatorService;

try
{
    // Build the configuration from appsettings.json
    var configurationRoot = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

    // Set up dependency injection
    var serviceProvider = new ServiceCollection()
        .AddApplication(configurationRoot)
        .AddLogging()
        .BuildServiceProvider();

    // Get the elevator service from the service provider
    elevatorService = serviceProvider.GetRequiredService<IElevatorService>();

    // Create a cancellation token source to handle graceful shutdowns
    using var cancellationTokenSource = new CancellationTokenSource();

    // Handle Ctrl+C to cancel the simulation
    Console.CancelKeyPress += (_, e) =>
    {
        e.Cancel = true;
        cancellationTokenSource.Cancel();
    };

    // Set console properties
    Console.CursorVisible = false;
    Console.Title = "Elevator Simulation System";

    // Calculate the status height required to display all elevators
    var statusHeight = ConsoleDisplayService.CalculateStatusHeight(elevatorService.Elevators.Count);
    if (statusHeight > Console.WindowHeight)
    {
        Console.Clear();
        ConsoleDisplayService.DisplayMessage(
            "The console window is too small to display the status. Please resize the window.",
            ConsoleColor.Red, statusHeight);
        return;
    }

    // Display initial status of elevators
    ConsoleDisplayService.DisplayStatus(elevatorService.Elevators);

    // Main loop to handle elevator requests
    while (!cancellationTokenSource.Token.IsCancellationRequested)
    {
        try
        {
            var elevatorRequestFromUserInput = ConsoleDisplayService.GetElevatorRequestFromUserInput(statusHeight);

            Console.Clear();

            var callElevatorTask = Task.Run(async () =>
            {
                var elevatorResult =
                    await elevatorService.CallElevatorAsync(elevatorRequestFromUserInput, cancellationTokenSource.Token);

                if (!elevatorResult.IsFailure)
                {
                    return elevatorResult;
                }
                
                Console.SetCursorPosition(0, statusHeight + 1);
                ConsoleExtensions.WritePadded(elevatorResult.Error.Message);

                return elevatorResult;
            }, cancellationTokenSource.Token);

            var statusUpdateTask = Task.Run(async () =>
            {
                while (!callElevatorTask.IsCompleted)
                {
                    ConsoleDisplayService.DisplayStatus(elevatorService.Elevators);
                    await Task.Delay(500);
                }
            }, cancellationTokenSource.Token);

            await statusUpdateTask;

            var result = await callElevatorTask;
            if (result.IsSuccess)
            {
                ConsoleDisplayService.DisplayStatus(elevatorService.Elevators);
                ConsoleDisplayService.DisplayMessage("Press 'Q' to quit or any other key to make another request.",
                    ConsoleColor.Yellow, statusHeight);

                if (Console.ReadKey().Key == ConsoleKey.Q)
                {
                    cancellationTokenSource.Cancel();
                    break;
                }
            }
            else
            {
                ConsoleDisplayService.DisplayMessage(result.Error.Message, ConsoleColor.Red, statusHeight);
            }
        }
        catch (Exception ex)
        {
            ConsoleDisplayService.DisplayMessage(ex.Message, ConsoleColor.Red, statusHeight);
        }
    }
}
catch (Exception ex)
{
    Console.Clear();
    ConsoleDisplayService.DisplayMessage(ex.Message, ConsoleColor.Red);
    return;
}

// Handle unhandled exceptions
AppDomain.CurrentDomain.UnhandledException += (_, e) =>
{
    Console.Clear();
    ConsoleDisplayService.DisplayMessage(
        $"Fatal Error: {(e.ExceptionObject as Exception)?.Message}",
        ConsoleColor.Red);
};