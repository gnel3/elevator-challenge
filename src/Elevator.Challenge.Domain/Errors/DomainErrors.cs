using Elevator.Challenge.Domain.Shared;

namespace Elevator.Challenge.Domain.Errors;

/// <summary>
/// Contains domain-specific errors for the elevator system.
/// </summary>
public static class DomainErrors
{
    /// <summary>
    /// Contains errors related to elevator operations.
    /// </summary>
    public static class Elevator
    {
        public static readonly Error CheckFloor = new(
            "Elevator.CheckFloor",
            "Floor numbers is not valid");
        
        public static readonly Error CheckPassengers = new(
            "Elevator.CheckPassengers",
            "At least one passenger is required");

        public static readonly Error MaxPassengersExceeded = new(
            "Elevator.MaxPassengersExceeded",
            "Elevator capacity exceeded");

        public static readonly Error RemovePassengers = new(
            "Elevator.RemovePassengers",
            "Cannot remove more passengers than present");
    }
}