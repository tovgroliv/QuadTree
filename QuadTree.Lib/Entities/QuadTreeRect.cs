using QuadTree.Lib.Interfaces;

namespace QuadTree.Lib.Entities;

internal class QuadTreeRect : IQuadTreeRect
{
	public float CenterX { get; set; }
	public float CenterY { get; set; }

	public float HalfSize { get; set; }

	public QuadTreeRect(float centerX, float centerY, float halfSize)
	{
		CenterX = centerX;
		CenterY = centerY;
		HalfSize = halfSize;
	}

	public bool IsPointInside(IQuadTreeItem in_point)
	{
		return Math.Abs(CenterX - in_point.X) <= HalfSize && Math.Abs(CenterY - in_point.Y) <= HalfSize;
	}

	public bool IsPointInside(float x, float y)
	{
		return Math.Abs(CenterX - x) <= HalfSize && Math.Abs(CenterY - y) <= HalfSize;
	}

	public bool IsOverlapping(IQuadTreeRect in_region)
	{
		return Math.Abs(CenterX - in_region.CenterX) <= in_region.HalfSize + HalfSize && Math.Abs(CenterY - in_region.CenterY) <= in_region.HalfSize + HalfSize;
	}

	public bool IsOverlapping(float left, float top, float width, float height)
	{
		return
			CenterX - HalfSize < left + width &&
			CenterX + HalfSize > left &&
			CenterY - HalfSize < top + height &&
			CenterY + HalfSize > top;
	}

	public int GetQuadrantIndex(IQuadTreeItem in_point)
	{
		int quadrant = 0;

		if (in_point.X > CenterX)
		{
			quadrant += 1;
		}

		if (in_point.Y > CenterY)
		{
			quadrant += 2;
		}

		return quadrant;
	}

	public double GetSquaredDistanceOfCenter(float in_x, float in_y)
	{
		return (CenterX - in_x) * (CenterX - in_x) + (CenterY - in_y) * (CenterY - in_y);
	}
}
