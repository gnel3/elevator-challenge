namespace Elevator.Challenge.Domain.Entities;

public abstract class Entity
{
    public Guid Id { get; private set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
    }
    
    protected Entity(Guid id)
    {
        Id = Guid.NewGuid();
    }
}