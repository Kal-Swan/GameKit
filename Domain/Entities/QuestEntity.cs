using System.Text.Json.Serialization;
using Domain.Entities.Tree;

namespace Domain.Entities;

public class QuestEntity : ITreeNode
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; } = "New Quest";
    public string Description { get; set; } = string.Empty;
    public string EntityType => EntityTypeConstants.Quest;
    
    [JsonIgnore]
    public ITreeNode? Parent { get; set; }
    
    public List<ITreeNode> Children { get; set; } = [];
    public string NodeType => EntityTypeConstants.Quest;
    
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