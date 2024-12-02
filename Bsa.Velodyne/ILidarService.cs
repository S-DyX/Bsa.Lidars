using System;
using System.Collections.Generic;
using Bsa.Velodyne.Entities;

namespace Bsa.Velodyne
{
	
	public delegate void LidarPointScanEvenHandler(List<LidarPoint> points);
	public interface ILidarService : IDisposable
	{
		event LidarPointScanEvenHandler LidarPointScanEvent; 
		void Start();
		void Stop();
		void Reconnect();

		bool IsConnected();
	}
}
