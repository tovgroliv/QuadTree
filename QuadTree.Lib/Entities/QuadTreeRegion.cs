using QuadTree.Lib.Interfaces;

namespace QuadTree.Lib.Entities;

/// <summary>
/// Class for storing rectangular (AABB) region. The region is stored as the center point and half size (width and height). The region must have the same width and height.
/// </summary>
public class QuadTreeRegion
{
	/// <summary>X coordinate of the center point of the region</summary>
	public float CenterX;

	/// <summary>Y coordinate of the center point of the region</summary>
	public float CenterY;

	/// <summary>Half size (Width and height) of the region</summary>
	public float HalfSize;

	/// <summary>
	/// Constructor from float values
	/// </summary>
	/// <param name="in_center_x">X coordinate of the center point of the region</param>
	/// <param name="in_center_y">Y coordinate of the center point of the region</param>
	/// <param name="in_half_size">Half size (Width and height) of the region</param>
	public QuadTreeRegion(float in_center_x, float in_center_y, float in_half_size)
	{
		CenterX = in_center_x;
		CenterY = in_center_y;
		HalfSize = in_half_size;
	}

	/// <summary>
	/// Checks if the given quadtree data (point) is inside the region.
	/// </summary>
	/// <param name="in_point">Quadtree data containing the coordinates to check</param>
	/// <returns>True if point is inside the region</returns>
	public bool IsPointInside(IQuadTreeData in_point)
	{
		return (Math.Abs(CenterX - in_point.X) <= HalfSize) && (Math.Abs(CenterY - in_point.Y) <= HalfSize);
	}

	/// <summary>
	/// Checks if two regions are overlapping
	/// </summary>
	/// <param name="in_region">Other region to check</param>
	/// <returns>True if regions are overlapping</returns>
	public bool IsOverlapping(QuadTreeRegion in_region)
	{
		return (Math.Abs(CenterX - in_region.CenterX) <= (in_region.HalfSize + HalfSize)) && (Math.Abs(CenterY - in_region.CenterY) <= (in_region.HalfSize + HalfSize));
	}

	/// <summary>
	/// Checks if two regions are overlapping. The other region is defined using it's left, top, width, height parameters
	/// </summary>
	/// <param name="in_left">Left X coordinate of the region</param>
	/// <param name="in_top">Top Y coordinate of the region</param>
	/// <param name="in_width">Width of the region</param>
	/// <param name="in_height">Height of the region</param>
	/// <returns></returns>
	public bool IsOverlapping(float in_left, float in_top, float in_width, float in_height)
	{
		return (CenterX - HalfSize < in_left + in_width) &&
						(CenterX + HalfSize > in_left) &&
						(CenterY - HalfSize < in_top + in_height) &&
						(CenterY + HalfSize > in_top);
	}

	/// <summary>
	/// Gets index of the quadrant where the given tree data (point) is located
	/// 
	/// The quadrant numbering:
	/// ---------
	/// | 0 | 1 |
	/// ---------
	/// | 2 | 3 |
	/// ---------
	/// </summary>
	/// <param name="in_point">Tree data (point) to check</param>
	/// <returns>Index of the quadrant [0..3]</returns>
	public int GetQuadrantIndex(IQuadTreeData in_point)
	{
		int quadrant = 0;

		if (in_point.X > CenterX)
			quadrant += 1;

		if (in_point.Y > CenterY)
			quadrant += 2;

		return quadrant;
	}

	/// <summary>
	/// Gets the square of the Euclidean distance of the specified point from the center of the region
	/// </summary>
	/// <param name="in_x">Point X coordinate</param>
	/// <param name="in_y">Point Y coordinate</param>
	/// <returns>Square of the Euclidean distance</returns>
	public double GetSquaredDistanceOfCenter(float in_x, float in_y)
	{
		return (CenterX - in_x) * (CenterX - in_x) + (CenterY - in_y) * (CenterY - in_y);
	}
}
