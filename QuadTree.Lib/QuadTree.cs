using QuadTree.Lib.Entities;
using QuadTree.Lib.Interfaces;
using System.Collections;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuadTree.Lib;

public class QuadTree<T> : IEnumerable<T> where T : IQuadTreeItem
{
	private QuadTreeNode<T> _root;
	private int _nodeCapacity;

	public QuadTree(QuadTreeRect region, int nodeCapacity)
	{
		_root = new QuadTreeNode<T>(region);
		_nodeCapacity = nodeCapacity;
	}

	public QuadTree(QuadTreeRect region) : this(region, 1) { }
	public QuadTree(float centerX, float centerY, float halfSize) : this(new QuadTreeRect(centerX, centerY, halfSize)) { }
	public QuadTree(float centerX, float centerY, float halfSize, int nodeCapacity) : this(new QuadTreeRect(centerX, centerY, halfSize), nodeCapacity) { }

	public void Clear()
	{
		_root.Children = null;
		_root.Elements = new();
	}

	public void Update(T data, float oldX, float oldY)
	{
		UpdateRemove(_root, data, oldX, oldY);
	}

	private void UpdateRemove(QuadTreeNode<T> currentNode, T dataToRemove, float oldX, float oldY)
	{
		if (!currentNode.Bounds.IsPointInside(oldX, oldY))
		{
			return;
		}

		if (currentNode.Children != null)
		{
			var children = currentNode.Children;

			if (children[0].Bounds != null && children[0].Bounds.IsPointInside(oldX, oldY))
			{
				UpdateRemove(children[0], dataToRemove, oldX, oldY);
			}
			else if (children[1].Bounds != null && children[1].Bounds.IsPointInside(oldX, oldY))
			{
				UpdateRemove(children[1], dataToRemove, oldX, oldY);
			}
			else if (children[2].Bounds != null && children[2].Bounds.IsPointInside(oldX, oldY))
			{
				UpdateRemove(children[2], dataToRemove, oldX, oldY);
			}
			else if (children[3].Bounds != null && children[3].Bounds.IsPointInside(oldX, oldY))
			{
				UpdateRemove(children[3], dataToRemove, oldX, oldY);
			}

			if (children[0].Elements.Count() == 0 && children[0].Children == null &&
				children[1].Elements.Count() == 0 && children[1].Children == null &&
				children[2].Elements.Count() == 0 && children[2].Children == null &&
				children[3].Elements.Count() == 0 && children[3].Children == null)
			{
				currentNode.Children = null;
			}
			if (children[0].Elements.Count() + children[1].Elements.Count() + children[2].Elements.Count() + children[3].Elements.Count() < _nodeCapacity &&
				children[0].Children == null && children[1].Children == null && children[2].Children == null && children[3].Children == null)
			{
				children[0].Elements.ForEach(e => currentNode.Elements.Add(e));
				children[1].Elements.ForEach(e => currentNode.Elements.Add(e));
				children[2].Elements.ForEach(e => currentNode.Elements.Add(e));
				children[3].Elements.ForEach(e => currentNode.Elements.Add(e));

				currentNode.Children = null;
			}
		}
		else
		{
			//if (!currentNode.Bounds.IsPointInside(dataToRemove))
			{
				currentNode.Elements.Remove(dataToRemove);
				Insert(dataToRemove);
			}
		}
	}

	public void Remove(T data)
	{
		Remove(_root, data);
	}

	private void Remove(QuadTreeNode<T> currentNode, T dataToRemove)
	{
		if (!currentNode.Bounds.IsPointInside(dataToRemove))
		{
			return;
		}

		if (currentNode.Children != null)
		{
			var children = currentNode.Children;

			if (children[0].Bounds != null && children[0].Bounds.IsPointInside(dataToRemove))
			{
				Remove(children[0], dataToRemove);
			}
			else if (children[1].Bounds != null && children[1].Bounds.IsPointInside(dataToRemove))
			{
				Remove(children[1], dataToRemove);
			}
			else if (children[2].Bounds != null && children[2].Bounds.IsPointInside(dataToRemove))
			{
				Remove(children[2], dataToRemove);
			}
			else if (children[3].Bounds != null && children[3].Bounds.IsPointInside(dataToRemove))
			{
				Remove(children[3], dataToRemove);
			}

			if (children[0].Elements.Count() == 0 && children[0].Children == null &&
				children[1].Elements.Count() == 0 && children[1].Children == null &&
				children[2].Elements.Count() == 0 && children[2].Children == null &&
				children[3].Elements.Count() == 0 && children[3].Children == null)
			{
				currentNode.Children = null;
			}
			if (children[0].Elements.Count() + children[1].Elements.Count() + children[2].Elements.Count() + children[3].Elements.Count() < _nodeCapacity &&
				children[0].Children == null && children[1].Children == null && children[2].Children == null && children[3].Children == null)
			{
				children[0].Elements.ForEach(e => currentNode.Elements.Add(e));
				children[1].Elements.ForEach(e => currentNode.Elements.Add(e));
				children[2].Elements.ForEach(e => currentNode.Elements.Add(e));
				children[3].Elements.ForEach(e => currentNode.Elements.Add(e));

				currentNode.Children = null;
			}
		}
		else
		{
			currentNode.Elements.Remove(dataToRemove);
		}
	}

	public void Insert(T data)
	{
		Insert(_root, data);
	}

	private void Insert(QuadTreeNode<T> currentNode, T dataToInsert)
	{
		if (!currentNode.Bounds.IsPointInside(dataToInsert))
		{
			return;
		}

		if (currentNode.Children == null)
		{
			List<T> elementsToSplite = new();
			currentNode.Elements.Add(dataToInsert);

			if (currentNode.Elements.Count() < _nodeCapacity)
			{
				return;
			}
			else
			{
				elementsToSplite = currentNode.Elements;
				currentNode.Elements = new();
			}

			var bounds = currentNode.Bounds;
			float half = bounds.HalfSize / 2;

			currentNode.Children = [
				new QuadTreeNode<T>(new QuadTreeRect(bounds.CenterX - half, bounds.CenterY - half, half)),
				new QuadTreeNode<T>(new QuadTreeRect(bounds.CenterX + half, bounds.CenterY - half, half)),
				new QuadTreeNode<T>(new QuadTreeRect(bounds.CenterX - half, bounds.CenterY + half, half)),
				new QuadTreeNode<T>(new QuadTreeRect(bounds.CenterX + half, bounds.CenterY + half, half))
			];

			elementsToSplite.ForEach(element =>
			{
				int quad = bounds.GetQuadrantIndex(element);

				Insert(currentNode.Children[quad], element);
			});
		}
		else
		{
			int quad = currentNode.Bounds.GetQuadrantIndex(dataToInsert);

			Insert(currentNode.Children[quad], dataToInsert);
		}
	}

	public delegate void NodeProcessCallback(QuadTreeRect in_bounds);
	public delegate void DataProcessCallback(T in_data);

	public void TraverseNodesAndLeafs(DataProcessCallback? dataProcessCallback, NodeProcessCallback nodeProcessCallback)
	{
		var stack = new Stack<QuadTreeNode<T>>();
		QuadTreeNode<T> current = _root;

		while (true)
		{
			if (current.Children != null)
			{
				foreach (QuadTreeNode<T> node in current.Children)
				{
					nodeProcessCallback?.Invoke(node.Bounds);
				}

				stack.Push(current.Children[2]);
				stack.Push(current.Children[1]);
				stack.Push(current.Children[3]);
				current = current.Children[0];
			}
			else
			{
				if (dataProcessCallback != null)
				{
					current.Elements.ForEach(element =>
					{
						dataProcessCallback?.Invoke(element);
					});
				}

				if (stack.Count > 0)
				{
					current = stack.Pop();
				}
				else
				{
					break;
				}
			}
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		var stack = new Stack<QuadTreeNode<T>>();
		var current = _root;

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
				foreach (var element in current.Elements)
				{
					yield return element;
				}
			}

			if (stack.Count > 0)
			{
				current = stack.Pop();
			}
			else
			{
				break;
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IEnumerable<T> Query(float centerX, float centerY, float halfSize)
	{
		var rect = new QuadTreeRect(centerX, centerY, halfSize);

		var stack = new Stack<QuadTreeNode<T>>();
		var current = _root;

		while (current != null)
		{
			if (current.Children != null)
			{
				var children = current.Children;

				if (children[2].Bounds != null && children[2].Bounds.IsOverlapping(rect))
				{
					stack.Push(children[2]);
				}

				if (children[1].Bounds != null && children[1].Bounds.IsOverlapping(rect))
				{
					stack.Push(children[1]);
				}

				if (children[3].Bounds != null && children[3].Bounds.IsOverlapping(rect))
				{
					stack.Push(children[3]);
				}

				if (children[0].Bounds != null && children[0].Bounds.IsOverlapping(rect))
				{
					stack.Push(children[0]);
				}
			}
			else
			{
				foreach (var element in current.Elements)
				{
					if (rect.IsPointInside(element))
					{
						yield return element;
					}
				}
			}

			if (stack.Count > 0)
			{
				current = stack.Pop();
			}
			else
			{
				break;
			}
		}
	}

	public IEnumerable<T> QueryNeighbours(float x, float y, int radius, int count)
	{
		var squaredRadius = radius * radius;
		var neighbours = Query(x, y, radius)
			.OrderBy(n => GetSquaredDistance(n, x, y))
			.Where(n => GetSquaredDistance(n, x, y) <= squaredRadius)
			.Take(count);

		return neighbours;
	}

	public double GetSquaredDistance(T data, float x, float y)
	{
		return (data.X - x) * (data.X - x) + (data.Y - y) * (data.Y - y);
	}
}
