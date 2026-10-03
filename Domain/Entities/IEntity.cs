namespace Domain.Entities;

public interface IEntity
{
    Guid Id { get; }
    string Name { get; set; }
    string EntityType { get; }
}