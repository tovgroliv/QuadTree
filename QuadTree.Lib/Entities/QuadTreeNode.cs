using QuadTree.Lib.Interfaces;

namespace QuadTree.Lib.Entities;

internal class QuadTreeNode<T> : IQuadTreeNode where T : IQuadTreeItem
{
	public int Level { get; }
	public IQuadTreeRect Bounds { get; }
	public QuadTreeNode<T>[]? Children;
	public List<T> Elements;

	public QuadTreeNode(IQuadTreeRect region, int level)
	{
		Bounds = region;
		Children = null;
		Elements = new();
		Level = level;
	}
}
