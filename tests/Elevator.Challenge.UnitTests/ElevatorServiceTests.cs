using Elevator.Challenge.Application.Models;
using Elevator.Challenge.Application.Services;
using Elevator.Challenge.Application.Settings;
using Elevator.Challenge.Domain.Enums;
using Elevator.Challenge.Domain.Errors;
using Microsoft.Extensions.Options;
using Moq;

namespace Elevator.Challenge.UnitTests;

[TestFixture]
public class ElevatorServiceTests
{
    private ElevatorService _elevatorService;
    private Mock<IOptions<ElevatorSettings>> _mockSettings;
    
    [SetUp]
    public void Setup()
    {
        _mockSettings = new Mock<IOptions<ElevatorSettings>>();
        _mockSettings.Setup(s => s.Value).Returns(new ElevatorSettings
        {
            NumberOfElevators = 2,
            MaxPassengers = 10,
            NumberOfFloors = 10
        });

        _elevatorService = new ElevatorService(_mockSettings.Object);
    }
    
    [Test]
    public async Task CallElevatorAsync_ShouldHandleConcurrentRequests()
    {
        // Arrange
        var request1 = new ElevatorRequest(1, 5, 5);
        var request2 = new ElevatorRequest(2, 6, 5);
        var cancellationToken = CancellationToken.None;

        // Act
        var task1 = _elevatorService.CallElevatorAsync(request1, cancellationToken);
        var task2 = _elevatorService.CallElevatorAsync(request2, cancellationToken);

        var results = await Task.WhenAll(task1, task2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(results[0].IsSuccess, Is.True);
            Assert.That(results[1].IsSuccess, Is.True);
        });
    }

    [Test]
    public async Task CallElevatorAsync_ShouldReturnSuccess_WhenRequestIsValid()
    {
        // Arrange
        var request = new ElevatorRequest(1, 5, 5);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _elevatorService.CallElevatorAsync(request, cancellationToken);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task CallElevatorAsync_ShouldReturnFailure_WhenRequestHasInvalidFloor()
    {
        // Arrange
        var request = new ElevatorRequest(0, 5, 5); // Invalid floor
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _elevatorService.CallElevatorAsync(request, cancellationToken);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(DomainErrors.Elevator.CheckFloor));
        });
    }

    [Test]
    public async Task CallElevatorAsync_ShouldReturnFailure_WhenRequestHasInvalidPassengers()
    {
        // Arrange
        var request = new ElevatorRequest(1, 5, 0); // Invalid passengers
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _elevatorService.CallElevatorAsync(request, cancellationToken);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(DomainErrors.Elevator.CheckPassengers));
        });
    }
    
    [Test]
    public async Task CallElevatorAsync_ShouldReturnSuccess_WhenRequestExceedsMaxPassengers()   
    {
        // Arrange
        var request = new ElevatorRequest(1, 5, 15); // Exceeds max passengers
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _elevatorService.CallElevatorAsync(request, cancellationToken);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
    }
    
    [Test]
    public async Task MoveAsync_ShouldHandleMultipleDestinations()
    {
        // Arrange
        var elevator = _elevatorService.Elevators.First();
        elevator.AddDestination(3);
        elevator.AddDestination(5);
        var cancellationToken = CancellationToken.None;

        // Act
        await elevator.MoveAsync(cancellationToken);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(elevator.CurrentFloor, Is.EqualTo(3));
            Assert.That(elevator.Status, Is.EqualTo(Status.Moving));
        });

        await elevator.MoveAsync(cancellationToken);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(elevator.CurrentFloor, Is.EqualTo(5));
            Assert.That(elevator.Status, Is.EqualTo(Status.Available));
        });
    }
}