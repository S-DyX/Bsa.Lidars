using System;
using System.Collections.Generic;
using System.Text;

namespace Bsa.RpLidar.Entities
{
	public class LidarPoint
	{
		/// <summary>
		/// Heading angle of the measurement (Unit : degree)
		/// </summary>
		public float Angle;

		/// <summary>
		/// Measured distance value between the rotating core of the RPLIDAR and the sampling point (Unit : mm)
		/// </summary>
		public float Distance;

		/// <summary>
		/// Quality of the measurement
		/// </summary>
		public ushort Quality;

		/// <summary>
		/// New 360 degree scan indicator
		/// </summary>
		public bool StartFlag;

		public int Flag;
		public bool IsValid => Distance > 0f;

		public override string ToString()
		{
			return $"Angle:{Angle};Distance:{Distance};Quality:{Quality}";
		}
	}
}
