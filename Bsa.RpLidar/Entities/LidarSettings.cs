namespace Bsa.RpLidar.Entities
{

	public interface ILidarSettings
	{
		int MaxDistance { get; set; }

		string Port { get; set; }

		int BaudRate { get; set; }

		ushort Pwm { get; set; }

		byte Type { get; set; }

		int ElapsedMilliseconds { get; set; }
	}

	public sealed class LidarSettings : ILidarSettings
	{
		public LidarSettings()
		{
			Pwm = 660;
			Type = 4;
			MaxDistance = 25000;
			BaudRate = 115200;
			Port = "Com3";
			ElapsedMilliseconds = 400;
		}
		public int MaxDistance { get; set; }

		public string Port { get; set; }

		public int BaudRate { get; set; }

		public ushort Pwm { get; set; }

		public byte Type { get; set; }

		public int ElapsedMilliseconds { get; set; }

	}
}
