using Bsa.Velodyne.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Bsa.Velodyne
{
	public sealed class VeloLidarSerialDevice : ILidarService
	{
		private readonly ILidarSettings _settings;
		private readonly ILocalLogger _localLogger;
		private bool _isRun;
		private UdpClient _client;
		private Thread _task;
		private readonly object _sync = new object();

		private List<LaserPitch> LaserPitch;
		private Dictionary<int, LaserPitch> _laserPitchDict = new Dictionary<int, LaserPitch>();
		public VeloLidarSerialDevice(ILidarSettings settings, ILocalLogger localLogger = null)
		{
			_settings = settings;
			if (settings.LaserPitches == null)
				throw new InvalidDataException("settings.LaserPitches can not be null");
			LaserPitch = settings.LaserPitches;
			foreach (var settingsLaserPitch in settings.LaserPitches)
			{
				_laserPitchDict[(int)settingsLaserPitch.Angle] = settingsLaserPitch;
			}
			_localLogger = localLogger;

		}


		public void Dispose()
		{
			Stop();
		}

		public void Start()
		{
			var serializeObject = "laser=on";
			ChangeSettings(serializeObject);
			lock (_sync)
			{
				if (!_isRun)
				{
					_isRun = true;
					InitConnection();
					_task = new Thread(ProcessMessages);
					_task.Start();
				}
			}
		}

		private void InitConnection()
		{
			_client = new UdpClient(new IPEndPoint(IPAddress.Any, _settings.Port));
			_client.Client.ReceiveTimeout = _settings.ReceiveTimeout; // .25 Seconds
			_client.Client.ReceiveBufferSize = 4096;
		}

		private readonly object _lock = new object();
		private unsafe void ProcessMessages()
		{
			var sw = new Stopwatch();
			sw.Start();
			var points = new List<LidarPoint>();
			while (_isRun)
			{
				try
				{
					if (!IsConnected())
						Reconnect();
					IPEndPoint iep = null;
					var data = _client.Receive(ref iep);


					if (data.Length == RawPacket._Size)
					{
						Packet p;
						bool valid;

						fixed (byte* b = data)
						{
							p = new Packet(b, out valid);
						}


						if (valid)
						{
							foreach (var verticalBlockPair in p.Blocks)
							{
								var azimuth = verticalBlockPair.Azimuth;
								var len = verticalBlockPair.ChannelData.Length;
								for (int i = 0; i < len; i++)
								{
									var distance = verticalBlockPair.ChannelData[i];
									var index = i;
									if (index > 15)
										index -= 16;
									points.Add(new LidarPoint()
									{
										Distance = (int)distance.DistanceMM,
										Angle = azimuth,
										HorizontalAngle = LaserPitch[index].Angle,
										Quality = distance.Reflectivity,
									});

								}
							}

						}
						if (sw.ElapsedMilliseconds > _settings.ElapsedMilliseconds)
						{

							sw.Restart();
							lock (_lock)
							{
								ScanProcess(points);
							}
							points.Clear();
						}

					}
				}
				catch (Exception e)
				{
					_localLogger?.Error(e.Message, e);
				}
			}
		}
		private void ScanProcess(List<LidarPoint> points)
		{
			if (!points.Any())
				return;
			if (!_isRun)
				return;

			if (LidarPointScanEvent != null)
			{
				LidarPointScanEvent.Invoke(points);
			}

		}

		public void Stop()
		{
			var serializeObject = "laser=off";
			ChangeSettings(serializeObject);
			lock (_sync)
			{
				_isRun = false;
				_client?.Close();
				_client = null;
				_task = null;
			}
		}

		public void Reconnect()
		{
			if (IsConnected())
			{
				var client = _client;
				_client = null;
				client.Close();
			}
			InitConnection();
		}

		public bool IsConnected()
		{
			return _client?.Client?.Connected ?? false;
		}

		private void ChangeSettings(string serializeObject)
		{
			//using (var wclnt = new MyWebClient(2000))
			//{
			//	wclnt.Headers["Content-type"] = " application/x-www-form-urlencoded";
			//	var address = $"http://{_settings.Ip}/cgi/setting";
			//	var ret = wclnt.UploadString(address, "POST", serializeObject);
			//}
		}

		public event LidarPointScanEvenHandler LidarPointScanEvent;
	}
}
