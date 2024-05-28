using QuadTree.Lib;
using QuadTree.Lib.Interfaces;

class Item(float x, float y) : IQuadTreeItem
{
	public float X { get; set; } = x;
	public float Y { get; set; } = y;
	public IQuadTreeNode? ParentNode { get; set; }
}

class Program
{
	static void Main(string[] args)
	{
		var halfWidth = 500;
		// create a quad tree with node size equal to 10
		var quadTree = new QuadTree<Item>(0, 0, halfWidth, 10, 10);

		Random random = new Random();
		// insert 1000 random items
		for (int i = 0; i < 1000; i++)
		{
			quadTree.Insert(new Item(random.Next(-halfWidth, halfWidth), random.Next(-halfWidth, halfWidth)));
		}

		var iterator = 0;

		// find neighbors near the zero point within a radius of 100 and contain a maximum number of 20
		Console.WriteLine("\nNeighbors near the point\n");
		iterator = 0;
		var neighbours = quadTree.QueryNeighbours(0, 0, 100, 20);
		neighbours.ToList().ForEach(n => Console.WriteLine($"{++iterator}. Neighbour - x:{n.X} y:{n.Y}"));

		// get neighbors in current node
		Console.WriteLine("\nNeighbors in current node\n");
		iterator = 0;
		var item = quadTree.First();
		quadTree.GetOnNode(item).ToList().ForEach(n => Console.WriteLine($"{++iterator}. Neighbour - x:{n.X} y:{n.Y}"));
	}
}
