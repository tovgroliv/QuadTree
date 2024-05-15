using QuadTree.Lib.Interfaces;

namespace QuadTree.Lib.Entities;

internal class QuadTreeNode<T> where T : IQuadTreeItem
{
    public QuadTreeRect Bounds;
    public QuadTreeNode<T>[]? Children;
    public List<T> Elements;

    public QuadTreeNode(QuadTreeRect region)
    {
        Bounds = region;
        Children = null;
        Elements = new();
    }
}
