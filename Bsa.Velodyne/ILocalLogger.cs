using System;

namespace Bsa.Velodyne
{
	public interface ILocalLogger
	{
		void Warn(string message);
		void Error(string message);
		void Error(string message, Exception ex);
		void Info(string message);

		void Debug(string message);
		 

	}
}
