using System;
using System.Collections.Generic;
using System.Text;

namespace Bsa.RpLidar.Entities
{
	public interface IDataResponse
	{
		/// <summary>
		/// <see cref="RpDataType"/>
		/// </summary>
		RpDataType Type { get; }
	}

	public enum RpDataType : byte
	{
		Scan = 0x81,
		GetInfo = 0x04,
		GetHealth = 0x06
	}
}
