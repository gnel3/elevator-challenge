namespace Elevator.Challenge.Domain.Entities;

/// <summary>
/// Represents the base class for all entities in the domain.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    protected Entity(int id) => Id = id;

    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    public int Id { get; private set; }
}