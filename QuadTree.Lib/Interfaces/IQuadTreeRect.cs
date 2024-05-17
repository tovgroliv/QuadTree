using QuadTree.Lib.Entities;

namespace QuadTree.Lib.Interfaces;

public interface IQuadTreeRect
{
	public float CenterX { get; }
	public float CenterY { get; }
	public float HalfSize { get; }

	public bool IsPointInside(IQuadTreeItem in_point);
	public bool IsPointInside(float x, float y);
	public bool IsOverlapping(IQuadTreeRect in_region);
	public bool IsOverlapping(float left, float top, float width, float height);
	public int GetQuadrantIndex(IQuadTreeItem in_point);
	public double GetSquaredDistanceOfCenter(float in_x, float in_y);
}
