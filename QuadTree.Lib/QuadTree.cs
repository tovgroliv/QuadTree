using System.Collections;
using System.Linq;
using QuadTree.Lib.Entities;
using QuadTree.Lib.Interfaces;

namespace QuadTree.Lib;

public class QuadTree<T> : IEnumerable<T> where T : IQuadTreeData
{
	private QuadTreeNode<T> m_root;
	private int m_bucket_capacity;
	private int m_node_count;

	public QuadTree(QuadTreeRegion in_region, int in_bucket_capacity)
	{
		m_root = new QuadTreeNode<T>(in_region);
		m_bucket_capacity = in_bucket_capacity;
		m_node_count = 0;
	}

	public QuadTree(QuadTreeRegion in_region) : this(in_region, 1)
	{

	}

	public QuadTree(float in_center_x, float in_center_y, float in_half_size) : this(new QuadTreeRegion(in_center_x, in_center_y, in_half_size))
	{
	}

	public QuadTree(float in_center_x, float in_center_y, float in_half_size, int in_bucket_size) : this(new QuadTreeRegion(in_center_x, in_center_y, in_half_size), in_bucket_size)
	{
	}

	public void Clear()
	{
		m_root.Children = null;
		m_root.Data = null;
	}
	public void Update(T obj)
	{
		//if (this.Count() == 0)
		//{
		//	Insert(obj);
		//}
		//else
		//{
		//	Remove(obj);
		//	Insert(obj);

		//}
	}
	//public void Remove(T in_data)
	//{
	//	var node = new QuadTreeLeaf<T>(in_data);

	//	Find(m_root, node);
	//}

	//private QuadTreeNode<T>? Find(IQuadTreeData node_to_remove)
	//{
	//	QuadTreeNode<T> current = m_root;

	//	while (current != null)
	//	{
	//		if (current.Children != null)
	//		{
	//			QuadTreeNode<T>[] children = current.Children;

	//			if (children[2].Bounds != null && children[2].Bounds.IsPointInside(node_to_remove))
	//			{
	//				current = children[2];
	//				continue;
	//			}

	//			if (children[1].Bounds != null && children[1].Bounds.IsPointInside(node_to_remove))
	//			{
	//				current = children[1];
	//				continue;
	//			}

	//			if (children[3].Bounds != null && children[3].Bounds.IsPointInside(node_to_remove))
	//			{
	//				current = children[3];
	//				continue;
	//			}

	//			if (children[0].Bounds != null && children[0].Bounds.IsPointInside(node_to_remove))
	//			{
	//				current = children[0];
	//				continue;
	//			}
	//		}
	//		else
	//		{
	//			QuadTreeLeaf<T> node = current.Data;

	//			while (node != null)
	//			{
	//				if (node.Data.X == node_to_remove.X && node.Data.Y == node_to_remove.Y)
	//					yield return node.Data;

	//				node = node.Next;
	//			}

	//			if (node != null)
	//			{
					
	//			}
	//		}
	//	}
	//}

	public void Insert(T in_data)
	{
		var node = new QuadTreeLeaf<T>(in_data);

		Insert(m_root, node);
	}

	private void Insert(QuadTreeNode<T> in_current_node, QuadTreeLeaf<T> in_node_to_insert)
	{
		// check if point ot insert is inside -> if it is not then it can not be child of this node
		if (!in_current_node.Bounds.IsPointInside(in_node_to_insert.Data))
			return;

		List<QuadTreeLeaf<T>> nodes_to_insert = null;
		int quadrant;

		// if this node is leaf
		if (in_current_node.Children == null)
		{
			in_current_node.Data.Add(in_node_to_insert);
			if (in_current_node.Data.Count() < m_bucket_capacity)
			{
				return;
			}
			else
			{
				// current node needs to be splitted
				nodes_to_insert = in_current_node.Data;

				// remove data
				in_current_node.Data = new();
			}
		}
		else
		{
			// move downward on the tree following the apropriate quadrant
			quadrant = in_current_node.Bounds.GetQuadrantIndex(in_node_to_insert.Data);

			Insert(in_current_node.Children[quadrant], in_node_to_insert);

			return;
		}

		// subdivide current node
		QuadTreeRegion bounds = in_current_node.Bounds;
		float half = bounds.HalfSize / 2;
		var subdivision = new QuadTreeNode<T>[4] {
			new QuadTreeNode<T>(new QuadTreeRegion(bounds.CenterX - half, bounds.CenterY - half, half)),
			new QuadTreeNode<T>(new QuadTreeRegion(bounds.CenterX + half, bounds.CenterY - half, half )),
			new QuadTreeNode<T>(new QuadTreeRegion(bounds.CenterX - half, bounds.CenterY + half, half )),
			new QuadTreeNode<T>(new QuadTreeRegion(bounds.CenterX + half, bounds.CenterY + half, half ))
		};

		// insert node
		quadrant = bounds.GetQuadrantIndex(in_node_to_insert.Data);

		Insert(subdivision[quadrant], in_node_to_insert);

		// insert nodes from the splitted node
		foreach (var node in nodes_to_insert)
		{
			quadrant = bounds.GetQuadrantIndex(node.Data);

			Insert(subdivision[quadrant], node);
		}

		in_current_node.Children = subdivision;

	}

	public delegate void NodeProcessCallback(QuadTreeRegion in_bounds);
	public delegate void DataProcessCallback(T in_data);

	public void TraverseNodesAndLeafs(DataProcessCallback in_data_process_callback, NodeProcessCallback in_node_process_callback)
	{
		var stack = new Stack<QuadTreeNode<T>>();
		QuadTreeNode<T> current = m_root;

		while (true)
		{
			if (current.Children != null)
			{
				if (in_node_process_callback != null)
				{
					foreach (QuadTreeNode<T> node in current.Children)
						in_node_process_callback(node.Bounds);
				}

				stack.Push(current.Children[2]);
				stack.Push(current.Children[1]);
				stack.Push(current.Children[3]);
				current = current.Children[0];
			}
			else
			{
				var nodes = current.Data;

				nodes.ForEach(node =>
				{
					in_data_process_callback(node.Data);
				});

				if (stack.Count > 0)
					current = stack.Pop();
				else
					break;
			}
		}
	}

	public IEnumerable<T> Query(float in_left, float in_top, float in_width, float in_height)
	{
		Stack<QuadTreeNode<T>> stack = new Stack<QuadTreeNode<T>>();
		QuadTreeNode<T> current = m_root;

		while (current != null)
		{
			if (current.Children != null)
			{
				QuadTreeNode<T>[] children = current.Children;

				if (children[2].Bounds != null && children[2].Bounds.IsOverlapping(in_left, in_top, in_width, in_height))
					stack.Push(children[2]);

				if (children[1].Bounds != null && children[1].Bounds.IsOverlapping(in_left, in_top, in_width, in_height))
					stack.Push(children[1]);

				if (children[3].Bounds != null && children[3].Bounds.IsOverlapping(in_left, in_top, in_width, in_height))
					stack.Push(children[3]);

				if (children[0].Bounds != null && children[0].Bounds.IsOverlapping(in_left, in_top, in_width, in_height))
					stack.Push(children[0]);
			}
			else
			{
				var nodes = current.Data;

				foreach (var node in nodes)
				{
					if (node.Data.X > in_left && node.Data.X < in_left + in_width && node.Data.Y > in_top && node.Data.Y < in_top + in_height)
						yield return node.Data;
				}
			}

			if (stack.Count > 0)
				current = stack.Pop();
			else
				break;
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		Stack<QuadTreeNode<T>> stack = new Stack<QuadTreeNode<T>>();
		QuadTreeNode<T> current = m_root;

		while (current != null)
		{
			if (current.Children != null)
			{
				stack.Push(current.Children[2]);
				stack.Push(current.Children[1]);
				stack.Push(current.Children[3]);
				stack.Push(current.Children[0]);
			}
			else
			{
				var nodes = current.Data;

				foreach (var node in nodes)
				{
					yield return node.Data;
				}
			}

			if (stack.Count > 0)
				current = stack.Pop();
			else
				break;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public List<T> QueryNeighbours(float in_x, float in_y, int in_neighbours_count)
	{
		QuadTreeRegion neighbour_region = new QuadTreeRegion(in_x, in_y, 0);
		Stack<QuadTreeNode<T>> stack = new Stack<QuadTreeNode<T>>();
		QuadTreeNode<T> current;

		List<T> neighbours = new List<T>();
		double[] neighbour_distances = new double[in_neighbours_count];
		double neighbours_worst_distance = 0;
		int neighbours_worst_index = 0;

		// set root node as current
		current = m_root;

		while (current != null)
		{

			// move downwards if this node has child nodes
			if (current.Children != null)
			{
				// store regions in the stack and continue with the closest region
				double closest_region_distance;
				int closest_region_index;

				// find closest region
				closest_region_index = 0;
				closest_region_distance = current.Children[0].Bounds.GetSquaredDistanceOfCenter(in_x, in_y);

				for (int i = 0; i < 4; i++)
				{
					double distance = current.Children[i].Bounds.GetSquaredDistanceOfCenter(in_x, in_y);
					if (distance < closest_region_distance)
					{
						closest_region_distance = distance;
						closest_region_index = i;
					}
				}

				// store regions
				for (int i = 0; i < 4; i++)
				{
					if (i == closest_region_index)
						continue;

					// if the neighbor region is defined then store only the overlapping regions, otherwise store all regions
					if (neighbour_region.HalfSize == 0 || current.Children[i].Bounds.IsOverlapping(neighbour_region))
						stack.Push(current.Children[i]);
				}

				// continue processing with the closest	region
				current = current.Children[closest_region_index];
			}
			else
			{
				foreach (var current_leaf_entry in current.Data)
				{
					// calculate distance (squared)
					double squared_distance = current_leaf_entry.GetSquaredDistance(in_x, in_y);

					if (current.Data != null)
					{
						// simply store data point if the list is not full
						if (neighbours.Count < in_neighbours_count)
						{
							if (neighbours.Count == 0)
							{
								neighbours_worst_distance = squared_distance;
								neighbours_worst_index = 0;
							}
							else
							{
								if (squared_distance > neighbours_worst_distance)
								{
									neighbours_worst_distance = squared_distance;
									neighbours_worst_index = neighbours.Count;
								}
							}

							// add this item to the neighbours list
							neighbour_distances[neighbours.Count] = squared_distance;
							neighbours.Add(current_leaf_entry.Data);

							// if the required number of neighbour is found store the worst distance in the region
							if (neighbours.Count == in_neighbours_count)
							{
								neighbour_region.HalfSize = (float)Math.Sqrt(neighbours_worst_distance);
							}
						}
						else
						{
							// list is full, store only when this item is closer than the worst item (largest distance) in the list
							if (squared_distance < neighbours_worst_distance)
							{
								// replace worst element
								neighbour_distances[neighbours_worst_index] = squared_distance;
								neighbours[neighbours_worst_index] = current_leaf_entry.Data;

								// find the current worst element
								neighbours_worst_index = 0;
								neighbours_worst_distance = neighbour_distances[0];
								for (int i = 1; i < in_neighbours_count; i++)
								{
									if (neighbour_distances[i] > neighbours_worst_distance)
									{
										neighbours_worst_distance = neighbour_distances[i];
										neighbours_worst_index = i;
									}
								}

								neighbour_region.HalfSize = (float)Math.Sqrt(neighbours_worst_distance);
							}
						}
					}
				}

				// get new element from the stack or exit if no more element to investigate
				do
				{
					if (stack.Count > 0)
					{
						current = stack.Pop();
					}
					else
					{
						current = null;
						break;
					}

					// if the neighbour region is know skip all elements with a non-overlapping region
				} while (neighbour_region.HalfSize > 0 && !current.Bounds.IsOverlapping(neighbour_region));


			}
		}

		return neighbours;
	}
}

