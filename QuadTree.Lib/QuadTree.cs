namespace QuadTree.Lib;

public class Quadtree<T> where T : IQuadTreeObject
{
	private Rectangle _bounds;
	private int _maxObjects;
	private List<Quadtree<T>> _children;
	private List<T> _objects;

	public Quadtree(Rectangle bounds, int maxObjects)
	{
		_bounds = bounds;
		_maxObjects = maxObjects;
		_children = new List<Quadtree<T>>();
		_objects = new List<T>();
	}

	public void Insert(T obj)
	{
		if (!_bounds.Contains(obj))
		{
			throw new ArgumentException("Object is not within the bounds of the quadtree");
		}

		if (_children.Count == 0)
		{
			_objects.Add(obj);

			if (_objects.Count > _maxObjects)
			{
				Subdivide();
			}
		}
		else
		{
			int index = (int)GetChildIndex(obj);
			_children[index].Insert(obj);
		}
	}

	public void Remove(T obj)
	{
		if (!_bounds.Contains(obj))
		{
			throw new ArgumentException("Object is not within the bounds of the quadtree");
		}

		if (_children.Count == 0)
		{
			_objects.Remove(obj);
		}
		else
		{
			int index = (int)GetChildIndex(obj);
			_children[index].Remove(obj);

			if (_children[index]._objects.Count == 0)
			{
				_children.RemoveAt(index);
			}
		}
	}

	public void Update(T obj)
	{
		if (!_bounds.Contains(obj))
		{
			throw new ArgumentException("Object is not within the bounds of the quadtree");
		}

		if (_children.Count == 0)
		{
			if (_objects.Contains(obj))
			{
				// Object is already in the quadtree, so its position has not changed
			}
			else
			{
				// Object is not in the quadtree, so it must be added
				Insert(obj);
			}
		}
		else
		{
			int index = (int)GetChildIndex(obj);

			if (_children[index]._objects.Contains(obj))
			{
				// Object is in the correct child quadtree, so its position has not changed
			}
			else
			{
				// Object is not in the correct child quadtree, so it must be removed from the old child and added to the new child
				_children[index].Remove(obj);
				Insert(obj);
			}
		}
	}

	public List<T> Query(Rectangle bounds)
	{
		List<T> foundObjects = new List<T>();

		if (_bounds.Intersects(bounds))
		{
			foreach (T obj in _objects)
			{
				if (bounds.Contains(obj))
				{
					foundObjects.Add(obj);
				}
			}

			foreach (Quadtree<T> child in _children)
			{
				foundObjects.AddRange(child.Query(bounds));
			}
		}

		return foundObjects;
	}

	private void Subdivide()
	{
		Rectangle nw = new Rectangle(_bounds.X, _bounds.Y, _bounds.Width / 2, _bounds.Height / 2);
		Rectangle ne = new Rectangle(_bounds.X + _bounds.Width / 2, _bounds.Y, _bounds.Width / 2, _bounds.Height / 2);
		Rectangle sw = new Rectangle(_bounds.X, _bounds.Y + _bounds.Height / 2, _bounds.Width / 2, _bounds.Height / 2);
		Rectangle se = new Rectangle(_bounds.X + _bounds.Width / 2, _bounds.Y + _bounds.Height / 2, _bounds.Width / 2, _bounds.Height / 2);

		_children.Add(new Quadtree<T>(nw, _maxObjects));
		_children.Add(new Quadtree<T>(ne, _maxObjects));
		_children.Add(new Quadtree<T>(sw, _maxObjects));
		_children.Add(new Quadtree<T>(se, _maxObjects));

		foreach (T obj in _objects)
		{
			int index = (int)GetChildIndex(obj);
			_children[index].Insert(obj);
		}

		_objects.Clear();
	}

	private QuadTreeIndexEnum GetChildIndex(T obj)
	{
		QuadTreeIndexEnum index = QuadTreeIndexEnum.None;

		double verticalMidpoint = _bounds.X + (_bounds.Width / 2);
		double horizontalMidpoint = _bounds.Y + (_bounds.Height / 2);

		bool left = obj is Rectangle ? ((Rectangle)(object)obj).X < verticalMidpoint : ((Point)(object)obj).X < verticalMidpoint;
		bool top = obj is Rectangle ? ((Rectangle)(object)obj).Y < horizontalMidpoint : ((Point)(object)obj).Y < horizontalMidpoint;

		if (left)
		{
			if (top)
			{
				index = QuadTreeIndexEnum.NE;
			}
			else
			{
				index = QuadTreeIndexEnum.SE;
			}
		}
		else
		{
			if (top)
			{
				index = QuadTreeIndexEnum.NW;
			}
			else
			{
				index = QuadTreeIndexEnum.SW;
			}
		}

		return index;
	}
}
