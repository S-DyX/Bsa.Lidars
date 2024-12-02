using System;
using System.Collections.Generic;
using System.Text;
using Bsa.RpLidar.Entities;

namespace Bsa.RpLidar
{ 

	public delegate void LidarPointGroupScanEvenHandler(LidarPointGroup points);
	public delegate void LidarPointScanEvenHandler(List<LidarPoint> points);
	public interface ILidarService : IDisposable
	{
		event LidarPointScanEvenHandler LidarPointScanEvent;
		event LidarPointGroupScanEvenHandler LidarPointGroupScanEvent;
		void Start();
		void Stop();

	}
}
