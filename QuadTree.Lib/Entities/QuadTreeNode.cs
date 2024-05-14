using QuadTree.Lib.Interfaces;

namespace QuadTree.Lib.Entities;

internal class QuadTreeNode<T> where T : IQuadTreeData
{
	public QuadTreeRegion Bounds;
	public QuadTreeNode<T>[] Children;
	public List<QuadTreeLeaf<T>> Data;

	public QuadTreeNode(QuadTreeRegion in_region)
	{
		Bounds = in_region;
		Children = null;
		Data = new();
	}
}
