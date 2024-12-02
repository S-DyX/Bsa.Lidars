using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bsa.RpLidar.Entities
{
	public sealed class LidarPointGroup : IEnumerable<LidarPointGroupItem>
	{
		public double X { get; set; }
		public double Y { get; set; }

		public LidarPointGroup(double x, double y)
		{
			X = x;
			Y = y;
		}

		public ILidarSettings Settings { get; set; }

		private Dictionary<int, LidarPointGroupItem> _dictionary = new Dictionary<int, LidarPointGroupItem>(3600);

		public LidarPointGroupItem this[float angle]
		{
			get
			{
				if (angle < 0)
					angle = 360 + angle;
				var anglei = (int)(angle * 10);
				if (_dictionary.ContainsKey(anglei))
				{
					return _dictionary[anglei];
				}

				return null;
			}
		}

		public LidarPointGroupItem this[int angle]
		{
			get
			{
				if (angle < 0)
					angle = 360 + angle;
				var anglei = angle * 10;
				if (_dictionary.ContainsKey(anglei))
				{
					return _dictionary[anglei];
				}

				return null;
			}
		}

		public void Add(LidarPoint point)
		{
			if (point == null || !point.IsValid)
				return;

			var angle = (int)(point.Angle * 10);
			if (_dictionary.ContainsKey(angle))
			{
				var lidarPointGroupItem = _dictionary[angle];
				if (lidarPointGroupItem.Distance > point.Distance)
				{
					lidarPointGroupItem.Distance = point.Distance;
					lidarPointGroupItem.Count++;
				}
			}
			else
			{
				_dictionary.Add(angle, new LidarPointGroupItem()
				{
					Angle = (int)point.Angle,
					OriginalAngle = point.Angle,
					Distance = point.Distance,
					Count = 1,

				});
			}
		}


		public int Count => _dictionary.Count;

		public double Compare(LidarPointGroup group)
		{
			double distance = 0;
			var dictF = new Dictionary<int, int>();
			var points = _dictionary.Values.ToList();
			var lidarPointGroupItems = @group.Items.ToList();
			var intersectCount = 0;
			var pointsCount = points.Count;
			for (var index = 0; index < pointsCount; index++)
			{
				var point = points[index];

				for (var i = 0; i < lidarPointGroupItems.Count; i++)
				{
					if (dictF.ContainsKey(i))
						continue;

					var second = lidarPointGroupItems[i];
					if (Math.Abs(second.Distance - point.Distance) < 50)
					{
						dictF[i] = index;
						intersectCount++;
						break;
					}
				}
			}
			if (intersectCount > 300)
			{
				return 1;
			}
			//if (intersectCount > 50)
			//{
			//	return .3;
			//}



			distance = (double)intersectCount / points.Count;
			return distance;
		}

		public List<LidarPoint> GetPoints()
		{
			var result = new List<LidarPoint>();
			foreach (var point in _dictionary.Values)
			{
				result.Add(new LidarPoint()
				{
					Angle = point.OriginalAngle,
					Distance = point.Distance
				});
			}

			return result;
		}

		public List<LidarPoint> Filter(ILocalLogger logger)
		{
			for (int i = 0; i <= 360; i++)
			{
				if (!_dictionary.ContainsKey(i))
					_dictionary[i] = null;
			}
			var points = _dictionary.Values.ToList();
			var result = new List<LidarPoint>();
			for (var index = 0; index < points.Count; index++)
			{
				var point = points[index];
				if (point == null)
					continue;


				var found = false;
				for (int i = index + 1; i < points.Count && i < 5; i++)
				{
					var pointNext = points[i];
					if (pointNext != null && Math.Abs(pointNext.Distance - point.Distance) < 300)
					{
						result.Add(new LidarPoint()
						{
							Angle = point.Angle,
							Distance = point.Distance
						});
						logger?.Warn($"Backward found near point: {point.ToString()}, {pointNext}");
						found = true;
						break;
					}

				}

				if (!found)
				{
					for (int i = index - 1; i > -5; i--)
					{
						if (i < 0)
							i = 360 + i;
						if (points.Count >= i)
							continue;
						var pointNext = points[i];
						if (pointNext != null && Math.Abs(pointNext.Distance - point.Distance) < 300)
						{
							result.Add(new LidarPoint() { Angle = point.Angle, Distance = point.Distance });
							logger?.Warn($"Backward found near point: {point.ToString()}, {pointNext}");
							found = true;
							break;
						}
					}
				}
				if (!found)
				{
					logger?.Warn($"Filtered point: {point.ToString()}");
				}



			}

			return result;
		}
		public void AddRange(IEnumerable<LidarPoint> points)
		{
			foreach (var lidarPoint in points)
			{
				Add(lidarPoint);
			}
		}

		public IEnumerable<LidarPointGroupItem> Items => _dictionary.Values;
		public IEnumerator<LidarPointGroupItem> GetEnumerator()
		{
			return _dictionary.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
	public sealed class LidarPointGroupItem
	{
		public int Angle { get; set; }
		public float OriginalAngle { get; set; }
		public float Distance { get; set; }

		public int Count { get; set; }

		public RobotSideType Side;

		public double? Cathetus;

		public bool CanIgnore;
		public override string ToString()
		{
			return $"{Angle};{Distance};{Count}";
		}
	}
}
