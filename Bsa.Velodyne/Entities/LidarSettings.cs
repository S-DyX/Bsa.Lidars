using System.Collections.Generic;

namespace Bsa.Velodyne.Entities
{
	public interface ILidarSettings
	{
		int Port { get; set; }
		List<LaserPitch> LaserPitches { get; set; }
		int ReceiveTimeout { get; set; }
		int ReceiveBufferSize { get; set; }

		int ElapsedMilliseconds { get; set; }
	}
	public sealed class VeloVp16LidarSettings : ILidarSettings
	{
		public VeloVp16LidarSettings()
		{
			Port = 2368;
			ReceiveBufferSize = 4096;
			ReceiveTimeout = 250;
			ElapsedMilliseconds = 200;
			var laserPitch = new float[16]
			{
				-15,
				1,
				-13,
				3,
				-11,
				5,
				-9,
				7,
				-7,
				9,
				-5,
				11,
				-3,
				13,
				-1,
				15
			};
			LaserPitches = new List<LaserPitch>();
			foreach (var f in laserPitch)
			{
				LaserPitches.Add(new LaserPitch() { Angle = f });
			}
		}


		public int Port { get; set; }
		public List<LaserPitch> LaserPitches { get; set; }
		public int ReceiveTimeout { get; set; }
		public int ReceiveBufferSize { get; set; }
		public int ElapsedMilliseconds { get; set; }
	}
}
