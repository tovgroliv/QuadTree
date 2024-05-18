using QuadTree.Lib;
using QuadTree.Lib.Interfaces;
using System.ComponentModel;
using System.Diagnostics;
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

		public float SpeedX { get; set; } = 10;
		public float SpeedY { get; set; } = 10;

		public float X { get; set; }
		public float Y { get; set; }
		public IQuadTreeNode ParentNode { get; set; }

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


	QuadTree<Item> m_quadtree;

	private List<Item> m_neighbour_data = null;

	public Random m_rnd = new Random(DateTime.Now.Millisecond);

	bool _work = true;

	int _width = 500;

	public MainWindow()
	{
		InitializeComponent();

		_width = (int)cMainCanvas.Width;

		m_quadtree = new QuadTree<Item>((float)_width / 2, (float)_width / 2, (float)_width / 2, 4);

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

			lock (m_quadtree)
			{
				foreach (var item in m_quadtree.ToList())
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

					m_quadtree.Update(item);
				}
				foreach (var item in m_quadtree.ToList())
				{
					if (_work)
					{
						Application.Current.Dispatcher.Invoke(new Action(() =>
						{
							item.UpdateMarker();
							cRectCanvas.Children.Clear();
							m_quadtree.TraverseNodesAndLeafs(null, DrawQuadTreeNode);

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

								m_neighbour_data = m_quadtree.QueryNeighbours((float)current_pos.X, (float)current_pos.Y, distance, count).ToList();

								foreach (Item data in m_neighbour_data)
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
		m_quadtree.TraverseNodesAndLeafs(DrawQuadTreeLeaf, DrawQuadTreeNode);
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
		m_quadtree.Clear();
		cMainCanvas.Children.Clear();
		cRectCanvas.Children.Clear();
	}

	private void Canvas_MouseMove(object sender, MouseEventArgs e)
	{
		current_pos = e.GetPosition(cMainCanvas);
	}

	private void ClearNeighbourMarkers()
	{
		if (m_neighbour_data != null)
		{
			foreach (Item data in m_neighbour_data)
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
		foreach (Item data in m_quadtree)
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

		for (int i = 0; i < random; i++)
		{
			Item data = new Item((float)(m_rnd.NextDouble() * cMainCanvas.Width), (float)(m_rnd.NextDouble() * cMainCanvas.Height));

			m_quadtree.Insert(data);
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