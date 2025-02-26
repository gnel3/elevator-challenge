using Elevator.Challenge.Application.Models;
using Elevator.Challenge.Application.Services;
using Elevator.Challenge.Application.Settings;
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
}