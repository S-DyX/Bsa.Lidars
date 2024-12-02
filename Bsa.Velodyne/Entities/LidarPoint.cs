namespace Bsa.Velodyne.Entities
{
	public class LidarPoint
	{
		/// <summary>
		/// Heading angle of the measurement (Unit : degree)
		/// </summary>
		public float Angle;

		/// <summary>
		/// Угол отклонения по горизонту
		/// </summary>
		public float HorizontalAngle;

		/// <summary>
		/// Measured distance value between the rotating core of the RPLIDAR and the sampling point (Unit : mm)
		/// </summary>
		public int Distance;

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

		public double X { get; set; }
		public double Y { get; set; }
		public override string ToString()
		{
			return $"Angle:{Angle};Distance:{Distance};Quality:{Quality}";
		}
	}
}
