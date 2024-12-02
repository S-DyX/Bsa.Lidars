using System;
using System.Collections.Generic;
using System.Text;

namespace Bsa.RpLidar.Entities
{
	internal struct RplidarPayloadMotorPwm
	{
		internal ushort pwm_value;
	}

	internal struct RplidarPayloadExpressScan
	{
		internal byte working_mode;
		internal ushort working_flags;
		internal ushort param;
	}

	/// <summary>
	/// 4+128
	/// </summary>
	public sealed class RplidarResponseUltraCapsuleMeasurementNodes
	{
		public byte s_checksum_1; // see [s_checksum_1]
		public byte s_checksum_2; // see [s_checksum_1]
		public ushort start_angle_sync_q6;
		public uint[] ultra_cabins;
		public bool hasBit;
	}
}
