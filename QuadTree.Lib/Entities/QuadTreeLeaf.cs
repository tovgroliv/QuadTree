using QuadTree.Lib.Interfaces;

namespace QuadTree.Lib.Entities;

internal class QuadTreeLeaf<T> where T : IQuadTreeData
{
	public T Data;

	public QuadTreeLeaf(T in_data)
	{
		Data = in_data;
	}

	public double GetSquaredDistance(float in_x, float in_y)
	{
		return (Data.X - in_x) * (Data.X - in_x) + (Data.Y - in_y) * (Data.Y - in_y);
	}
}
