using System;
using System.Collections.Generic;
using System.Text;

namespace Bsa.RpLidar.Entities
{
	public sealed class LidarAngleSide
	{
		public LidarAngleSide()
		{
			Id = $"{Guid.NewGuid()}";
			MaxDistance = 16000;
		}
		
		public string Id;
		public float From;
		public float To;

		public RobotSideType Side;
		public float Distance;
		public float MaxDistance;

		public override string ToString()
		{
			return $"Lidar_{Side}_{From}-{To},{Id}";
		}
	}
}
