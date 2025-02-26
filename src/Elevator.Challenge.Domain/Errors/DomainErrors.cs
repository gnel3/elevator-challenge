using Elevator.Challenge.Domain.Shared;

namespace Elevator.Challenge.Domain.Errors;

public static class DomainErrors
{
    public static class Elevator
    {
        public static readonly Error CheckFloor = new Error(
            "Elevator.CheckFloor", 
            "Floor numbers is not valid");
        
        public static readonly Error CheckPassengers = new Error(
            "Elevator.CheckPassengers", 
            "At least one passenger is required");
    }
}