using Domain.Entities.Tree;

namespace Core.Tree.Visitors;

public interface ITreeNodeAction
{
    void Visit(ITreeNode node);
}