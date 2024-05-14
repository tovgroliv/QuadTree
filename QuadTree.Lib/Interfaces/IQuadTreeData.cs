namespace QuadTree.Lib.Interfaces;

/// <summary>
/// Interface for data class to be stored in the QuadTree
/// </summary>
public interface IQuadTreeData
{
	/// <summary>Gets X coordinate of the point to be stored in the quadtree</summary>
	float X { get; }

	/// <summary>Gets Y coordinate of the point to be stored in the quadtree</summary>
	float Y { get; }
}
