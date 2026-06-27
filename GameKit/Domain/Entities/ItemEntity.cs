namespace GameKit.Domain.Entities;

public class ItemEntity : IEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; }
    public string EntityType => "Item";
    
    public int Value { get; set; }
    public string Description { get; set; } = string.Empty;
}