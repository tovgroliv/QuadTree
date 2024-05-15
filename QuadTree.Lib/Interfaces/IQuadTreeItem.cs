namespace QuadTree.Lib.Interfaces;

public interface IQuadTreeItem
{
    public float X { get; }
    public float Y { get; }

    public double GetSquaredDistance(float x, float y)
    {
        return (X - x) * (X - x) + (Y - y) * (Y - y);
    }
}
