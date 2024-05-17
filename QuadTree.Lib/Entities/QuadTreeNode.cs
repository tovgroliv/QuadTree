using QuadTree.Lib.Interfaces;

namespace QuadTree.Lib.Entities;

internal class QuadTreeNode<T> : IQuadTreeNode where T : IQuadTreeItem
{
	public IQuadTreeRect Bounds { get; }
	public QuadTreeNode<T>[]? Children;
	public List<T> Elements;

	public QuadTreeNode(IQuadTreeRect region)
	{
		Bounds = region;
		Children = null;
		Elements = new();
	}
}
