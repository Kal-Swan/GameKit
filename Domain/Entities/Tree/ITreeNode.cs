using System.Text.Json.Serialization;

namespace Domain.Entities.Tree;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ItemEntity), EntityTypeConstants.Item)]
[JsonDerivedType(typeof(QuestEntity), EntityTypeConstants.Quest)]
public interface ITreeNode : IEntity
{
    ITreeNode? Parent { get; set; }
    List<ITreeNode> Children { get; }
    string NodeType { get; }
    void AddChild(ITreeNode child, int? position = null);
    void RemoveChild(ITreeNode child);
}