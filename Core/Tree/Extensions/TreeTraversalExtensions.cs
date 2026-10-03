using Core.Tree.Visitors;
using Domain.Entities.Tree;

namespace Core.Tree.Extensions;

public static class TreeTraversalExtensions
{
    public static void Traverse(this ITreeNode root, ITreeNodeAction nodeAction)
    {
        nodeAction.Visit(root);
        foreach (var child in root.Children)
        {
            child.Traverse(nodeAction);
        }
    }
}