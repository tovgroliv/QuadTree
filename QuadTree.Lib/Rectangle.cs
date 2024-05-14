namespace QuadTree.Lib;

public class Rectangle : IQuadTreeObject
{
	public float X { get; set; }
	public float Y { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }

	public Rectangle(float x, float y, int width, int height)
	{
		X = x;
		Y = y;
		Width = width;
		Height = height;
	}

	public bool Contains(IQuadTreeObject rect)
	{
		if (rect is Rectangle)
		{
			return Contains((Rectangle)rect);
		}
		if (rect is Point)
		{
			return Contains((Point)rect);
		}

		throw new Exception();
	}

	private bool Contains(Rectangle rect)
	{
		return X <= rect.X && X + Width >= rect.X + rect.Width && Y <= rect.Y && Y + Height >= rect.Y + rect.Height;
	}

	private bool Contains(Point point)
	{
		return X <= point.X && X + Width >= point.X && Y <= point.Y && Y + Height >= point.Y;
	}

	public bool Intersects(Rectangle rect)
	{
		return X < rect.X + rect.Width && X + Width > rect.X && Y < rect.Y + rect.Height && Y + Height > rect.Y;
	}
}
