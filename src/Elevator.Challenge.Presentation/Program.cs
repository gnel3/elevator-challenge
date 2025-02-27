using Elevator.Challenge.Application;
using Elevator.Challenge.Application.Interfaces;
using Elevator.Challenge.Presentation.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
    var elevatorService = serviceProvider.GetRequiredService<IElevatorService>();

    // Create a cancellation token source to handle graceful shutdowns
    using var cancellationTokenSource = new CancellationTokenSource();
    
    var cancellationToken = cancellationTokenSource.Token;

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
    
    // Main task to handle elevator requests so that using CancelKeyPress works to cancel the task
    var mainTask = Task.Run(async () =>
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var elevatorRequestFromUserInput = ConsoleDisplayService.GetElevatorRequestFromUserInput(statusHeight);

            var elevatorRequestTask = Task.Run(async () => await ElevatorRequestService.HandleElevatorRequest(
                elevatorService,
                elevatorRequestFromUserInput,
                cancellationToken), cancellationToken);
            
            var elevatorRequestResult = await elevatorRequestTask;
            if (elevatorRequestResult.IsSuccess)
            {
                ConsoleDisplayService.DisplayStatus(elevatorService.Elevators);
                continue;
            }

            ConsoleDisplayService.DisplayMessage(elevatorRequestResult.Error.Message, ConsoleColor.Red,
                statusHeight + 1);
        }
    }, cancellationToken);

    // Wait for the main task to complete or for the cancellation token to be triggered
    await Task.WhenAny(mainTask, Task.Delay(Timeout.Infinite, cancellationToken));
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