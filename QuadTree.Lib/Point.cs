namespace QuadTree.Lib;

public struct Point : IQuadTreeObject
{
	public double X { get; set; }
	public double Y { get; set; }

	public Point(double x, double y)
	{
		X = x;
		Y = y;
	}
}
