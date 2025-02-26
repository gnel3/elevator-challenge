using Elevator.Challenge.Application;
using Elevator.Challenge.Application.Interfaces;
using Elevator.Challenge.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elevator.Challenge.IntegrationTests;

[TestFixture]
public class ProgramTests
{
    private ServiceProvider _serviceProvider;
    private bool _hasConfigurationBeenLoaded = false;

    [SetUp]
    public void Setup()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        _hasConfigurationBeenLoaded = true;
        
        _serviceProvider = new ServiceCollection()
            .AddApplication(configuration)
            .AddLogging()
            .BuildServiceProvider();
    }
    
    [TearDown]
    public void TearDown()
    {
        if (_hasConfigurationBeenLoaded)
        {
            _serviceProvider.Dispose();
        }
    }

    [Test]
    public void ShouldResolveElevatorService()
    {
        var elevatorService = _serviceProvider.GetRequiredService<IElevatorService>();
        Assert.That(elevatorService, Is.Not.Null);
    }

    [Test]
    public async Task ShouldHandleElevatorRequest()
    {
        var elevatorService = _serviceProvider.GetRequiredService<IElevatorService>();
        var request = new ElevatorRequest(1, 5, 5);
        var result = await elevatorService.CallElevatorAsync(request, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
    }
}