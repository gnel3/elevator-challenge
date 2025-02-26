using Elevator.Challenge.Application;
using Elevator.Challenge.Application.Interfaces;
using Elevator.Challenge.Presentation.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

IElevatorService elevatorService;

try
{
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
}
catch (Exception ex)
{
    ConsoleDisplayService.DisplayMessage(ex.Message, ConsoleColor.Red);
    return;
}

Console.Title = "Elevator Simulation System";
Console.Clear();
Console.WriteLine("Welcome to the Elevator Simulation System");
Console.WriteLine("Press Ctrl+C to exit");
Console.WriteLine();

// Create a cancellation token source to handle graceful shutdowns
using var cancellationTokenSource = new CancellationTokenSource();

// Handle Ctrl+C to cancel the simulation
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cancellationTokenSource.Cancel();
};

// Display initial status of elevators
ConsoleDisplayService.DisplayStatus(elevatorService.Elevators);

while (!cancellationTokenSource.Token.IsCancellationRequested)
{
    try
    {
        var elevatorRequest = ConsoleDisplayService.GetElevatorRequestFromUserInput();

        var callElevatorTask = Task.Run(async () =>
        {
            var elevatorResult =
                await elevatorService.CallElevatorAsync(elevatorRequest, cancellationTokenSource.Token);
            
            if (elevatorResult.IsFailure)
            {
                Console.WriteLine(elevatorResult.Error.Message);
            }

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
            ConsoleDisplayService.DisplayMessage("Press 'Q' to quit or any other key to make another request.", ConsoleColor.Yellow);

            if (Console.ReadKey().Key == ConsoleKey.Q)
            {
                cancellationTokenSource.Cancel();
                break;
            }
        }
        else
        {
            ConsoleDisplayService.DisplayMessage(result.Error.Message, ConsoleColor.Red);
        }
    }
    catch (Exception ex)
    {
        ConsoleDisplayService.DisplayMessage(ex.Message, ConsoleColor.Red);
    }
}