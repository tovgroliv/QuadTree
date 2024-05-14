namespace QuadTree.Lib;

public class Rectangle
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

	public bool Contains(Rectangle rect)
	{
		return X <= rect.X && X + Width >= rect.X + rect.Width && Y <= rect.Y && Y + Height >= rect.Y + rect.Height;
	}

	public bool Contains(Point point)
	{
		return X <= point.X && X + Width >= point.X && Y <= point.Y && Y + Height >= point.Y;
	}

	public bool Intersects(Rectangle rect)
	{
		return X < rect.X + rect.Width && X + Width > rect.X && Y < rect.Y + rect.Height && Y + Height > rect.Y;
	}
}
