namespace QuadTree.Lib.Interfaces;

public interface IQuadTreeItem
{
	public float X { get; }
	public float Y { get; }

	public IQuadTreeNode? ParentNode { get; set; }
}
