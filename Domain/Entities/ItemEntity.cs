using System.Text.Json.Serialization;
using Domain.Entities.Tree;

namespace Domain.Entities;

public class ItemEntity : ITreeNode
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "New Item";
    public string EntityType => EntityTypeConstants.Item;

    public int Value { get; set; }
    public string Description { get; set; } = string.Empty;
    
    [JsonIgnore]
    public ITreeNode? Parent { get; set; }
    public List<ITreeNode> Children { get; set; } = [];
    public string NodeType => EntityTypeConstants.Item;
    public void AddChild(ITreeNode child, int? position)
    {
        child.Parent?.RemoveChild(child);

        if (position is not null)
        {
            Children.Insert(position.Value, child);
        }
        else
        {
            Children.Add(child);
        }
        
        child.Parent = this;
    }

    public void RemoveChild(ITreeNode child)
    {
        if(Children.Remove(child))
        {
            child.Parent = null;
        }
    }
}