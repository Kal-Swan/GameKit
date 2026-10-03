using Domain.Entities.Tree;

namespace Core.Tree.Visitors;

public class CountNodeAction : ITreeNodeAction
{
    public int TotalCount { get; private set; }
    public Dictionary<string, int> CountByType { get; } = new();
    public void Visit(ITreeNode node)
    {
        TotalCount++;
        if (!CountByType.TryAdd(node.NodeType, 1))
        {
            CountByType[node.NodeType]++;
        }
    }
}