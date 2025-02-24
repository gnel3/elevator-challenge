namespace Elevator.Challenge.Domain.Entities;

public abstract class Entity(int id)
{
    public int Id { get; private set; } = id;
}