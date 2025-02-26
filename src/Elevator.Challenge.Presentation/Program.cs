using Elevator.Challenge.Application;
using Elevator.Challenge.Application.Interfaces;
using Elevator.Challenge.Presentation.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

IElevatorService elevatorService;

try
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

    var serviceProvider = new ServiceCollection()
        .AddApplication(configuration)
        .BuildServiceProvider();

    elevatorService = serviceProvider.GetRequiredService<IElevatorService>();
}
catch (Exception ex)
{
    ConsoleDisplayService.ShowMessage(ex.Message, ConsoleColor.Red);
    return;
}

Console.Title = "Elevator Simulation System";
Console.Clear();
Console.WriteLine("Welcome to the Elevator Simulation System");
Console.WriteLine("Press Ctrl+C to exit");
Console.WriteLine();

using var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    cancellationTokenSource.Cancel();
};

ConsoleDisplayService.ShowStatus(elevatorService.Elevators);

while (!cancellationTokenSource.Token.IsCancellationRequested)
{
    try
    {
        var elevatorRequest = ConsoleDisplayService.GetRequest();

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
                ConsoleDisplayService.ShowStatus(elevatorService.Elevators);
                await Task.Delay(1000);
            }
        }, cancellationTokenSource.Token);

        await statusUpdateTask;

        var result = await callElevatorTask;
        if (result.IsSuccess)
        {
            ConsoleDisplayService.ShowStatus(elevatorService.Elevators);
            ConsoleDisplayService.ShowMessage("Press 'Q' to quit or any other key to make another request.", ConsoleColor.Yellow);

            if (Console.ReadKey().Key == ConsoleKey.Q)
            {
                cancellationTokenSource.Cancel();
                break;
            }
        }
        else
        {
            ConsoleDisplayService.ShowMessage(result.Error.Message, ConsoleColor.Red);
        }
    }
    catch (Exception ex)
    {
        ConsoleDisplayService.ShowMessage(ex.Message, ConsoleColor.Red);
    }
}