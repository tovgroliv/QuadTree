using QuadTree.Lib;
using QuadTree.Lib.Interfaces;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuadTree.TestApp;

public partial class MainWindow : Window
{
	public class Item : IQuadTreeItem
	{
		public Ellipse Marker;

		public float SpeedX { get; set; } = 0;
		public float SpeedY { get; set; } = 0;

		public float X { get; set; }
		public float Y { get; set; }
		public IQuadTreeNode? ParentNode { get; set; }

		public Item(float in_x, float in_y)
		{
			var random = new Random();
			SpeedX *= Math.Sign(random.Next(-1, 1) == 0 ? 1 : -1);
			SpeedY *= Math.Sign(random.Next(-1, 1) == 0 ? 1 : -1);

			Marker = new Ellipse()
			{
				Width = 5,
				Height = 5,
				Fill = Brushes.Green
			};

			Marker.SetValue(Canvas.LeftProperty, in_x - 2.5);
			Marker.SetValue(Canvas.TopProperty, in_y - 2.5);
			Marker.SetValue(Panel.ZIndexProperty, 1);
			Marker.IsHitTestVisible = false;

			X = in_x;
			Y = in_y;
		}

		public void UpdateMarker()
		{
			Marker.SetValue(Canvas.LeftProperty, X - 2.5);
			Marker.SetValue(Canvas.TopProperty, Y - 2.5);
		}
	}


	private QuadTree<Item> _quadTree;

	private List<Item> _neighbourData = new();

	public Random _random = new Random(DateTime.Now.Millisecond);

	bool _work = true;

	int _width = 500;

	public MainWindow()
	{
		InitializeComponent();

		_width = (int)cMainCanvas.Width;

		_quadTree = new QuadTree<Item>((float)_width / 2, (float)_width / 2, (float)_width / 2, 4, 10);

		this.Closing += (s,e) => _work = false;

		Thread moveThread = new Thread(() => Moving());
		moveThread.IsBackground = true;
		moveThread.Start();
	}

	private void Moving()
	{
		while (_work)
		{
			Thread.Sleep(40);

			lock (_quadTree)
			{
				foreach (var item in _quadTree.ToList())
				{
					var oldX = item.X;
					var oldY = item.Y;
					item.X = Math.Clamp(item.X + item.SpeedX, 0, (float)_width);
					item.Y = Math.Clamp(item.Y + item.SpeedY, 0, (float)_width);

					if (item.X == 0 || item.X == _width)
					{
						item.SpeedX *= -1;
					}
					if (item.Y == 0 || item.Y == _width)
					{
						item.SpeedY *= -1;
					}

					_quadTree.Update(item);
				}
				foreach (var item in _quadTree.ToList())
				{
					if (_work)
					{
						Application.Current.Dispatcher.Invoke(new Action(() =>
						{
							//item.UpdateMarker();
							//cRectCanvas.Children.Clear();
							//_quadTree.TraverseNodesAndLeafs(null, DrawQuadTreeNode);

							if (cbNeighbourSearch.IsChecked == true)
							{
								ClearNeighbourMarkers();

								var checkDistance = int.TryParse(distanceSearchField.Text, out var distance);
								var checkCount = int.TryParse(countSearchField.Text, out var count);

								if (!checkDistance)
								{
									distance = 50;
								}

								if (!checkCount)
								{
									count = 10;
								}

								_neighbourData = _quadTree.QueryNeighbours((float)current_pos.X, (float)current_pos.Y, distance, count).ToList();

								foreach (Item data in _neighbourData)
								{
									data.Marker.Fill = Brushes.Cyan;
								}
							}
						}));
					}
					else
					{
						return;
					}
				}
			}
		}
	}

	Point current_pos;

	private void DrawQuadtree()
	{
		_quadTree.TraverseNodesAndLeafs(DrawQuadTreeLeaf, DrawQuadTreeNode);
		coundLabel.Text = $"{cMainCanvas.Children.Count}";
	}

	private void DrawQuadTreeNode(IQuadTreeRect in_bounds)
	{
		var rect = new Rectangle()
		{
			Width = in_bounds.HalfSize * 2 + 1,
			Height = in_bounds.HalfSize * 2 + 1,
			Stroke = Brushes.LightCoral,
			SnapsToDevicePixels = true
		};

		rect.SetValue(Canvas.LeftProperty, (double)(in_bounds.CenterX - in_bounds.HalfSize));
		rect.SetValue(Canvas.TopProperty, (double)(in_bounds.CenterY - in_bounds.HalfSize));
		rect.SetValue(Panel.ZIndexProperty, 0);
		cRectCanvas.Children.Add(rect);

	}

	private void DrawQuadTreeLeaf(Item in_data)
	{
		cMainCanvas.Children.Add((Ellipse)in_data.Marker);
	}

	private void ClearButton_Click(object sender, RoutedEventArgs e)
	{
		_quadTree.Clear();
		cMainCanvas.Children.Clear();
		cRectCanvas.Children.Clear();
	}

	private void Canvas_MouseMove(object sender, MouseEventArgs e)
	{
		current_pos = e.GetPosition(cMainCanvas);
	}

	private void ClearNeighbourMarkers()
	{
		if (_neighbourData != null)
		{
			foreach (Item data in _neighbourData)
			{
				data.Marker.Fill = Brushes.Green;
			}
		}
	}

	private void CheckBox_Click(object sender, RoutedEventArgs e)
	{
		if (((CheckBox)sender).IsChecked == false)
		{
			ClearNeighbourMarkers();
		}
	}

	private void ClearRegionMarkers()
	{
		foreach (Item data in _quadTree)
		{
			data.Marker.Fill = Brushes.Green;
		}
	}

	private void DeselectButton_Click(object sender, RoutedEventArgs e)
	{
		ClearRegionMarkers();
	}

	private void AddRandomButton_Click(object sender, RoutedEventArgs e)
	{

		var check = int.TryParse(countRandomField.Text, out var random);

		if (!check)
		{
			random = 10;
		}

		//for (int i = 0; i < random; i++)
		//{
		//	Item data = new Item((float)(_random.NextDouble() * cMainCanvas.Width), (float)(_random.NextDouble() * cMainCanvas.Height));

		//	_quadTree.Insert(data);
		//}

		for (int x = 0; x < 20; x++)
		{
			for (int y = 0; y < 20; y++)
			{
				Item data = new Item(x * 10, y * 10);

				_quadTree.Insert(data);
			}
		}

		//coundLabel.Text = $"{m_quadtree.Count()}";

		cMainCanvas.Children.Clear();
		cRectCanvas.Children.Clear();
		DrawQuadtree();

	}

	private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		Process.Start(new ProcessStartInfo("cmd", $"/c start {e.Uri.OriginalString}") { CreateNoWindow = true });
	}
}