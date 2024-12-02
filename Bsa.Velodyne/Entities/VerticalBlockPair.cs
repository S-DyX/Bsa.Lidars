using System;
using System.Runtime.InteropServices;

namespace Bsa.Velodyne.Entities
{
	public struct VerticalBlockPair
	{
		public float Azimuth { get; private set; }
		public SinglePoint[] ChannelData { get; private set; }

		internal unsafe VerticalBlockPair(Byte* b, out bool valid)
		{
			RawBlockPair r = *(RawBlockPair*)b;

			valid = (r._Header[0] == 0xFF) && (r._Header[1] == 0xEE);

			this.Azimuth = ((r._Azimuth[1] << 8) | (r._Azimuth[0])) / 100.0f;

			this.ChannelData = new SinglePoint[RawBlockPair._SinglePointsPerVerticalBlock];

			for (int i = 0; i < RawBlockPair._SinglePointsPerVerticalBlock; i++)
				this.ChannelData[i] = new SinglePoint(&r._ChannelData[i * SinglePoint.RawPoint._Size]);
		}


		[StructLayout(LayoutKind.Explicit, Pack = 1)]
		internal unsafe struct RawBlockPair
		{
			/// <summary>
			/// Size of the structure in bytes
			/// </summary>
			public const int _Size = _ChannelDataE;
			public const int _SinglePointsPerVerticalBlock = 32;

			private const int _HeaderE = 2; // index after header E(nd)
			private const int _AzimuthE = 2 + _HeaderE; // index after azimuth E(nd)
			private const int _ChannelDataE = SinglePoint.RawPoint._Size * _SinglePointsPerVerticalBlock + _AzimuthE; // index after channel data E(nd)

			[FieldOffset(0)]
			public fixed byte _Header[2];

			[FieldOffset(_HeaderE)]
			public fixed byte _Azimuth[2];

			[FieldOffset(_AzimuthE)]
			public fixed byte _ChannelData[SinglePoint.RawPoint._Size * _SinglePointsPerVerticalBlock];
		}
	}

	[Serializable]
	public struct SinglePoint
	{
		public readonly float DistanceMM;
		public readonly byte Reflectivity;

		internal unsafe SinglePoint(Byte* b)
		{
			RawPoint r = *(RawPoint*)b;

			// Return Azimuth
			//var distanceMeters = ((r.Distance[1] << 8) | (r.Distance[0])) * 0.002f; // 2 MM increments

			this.DistanceMM = ((r.Distance[1] << 8) | (r.Distance[0])) <<1; // 2 MM increments
			this.Reflectivity = r.Reflectivity;
		}

		internal SinglePoint(float distanceMm, byte reflectivity)
		{
			this.DistanceMM = distanceMm;
			this.Reflectivity = reflectivity;
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1)]
		internal unsafe struct RawPoint
		{
			/// <summary>
			/// Size of the structure in bytes
			/// </summary>
			public const int _Size = 3;

			[FieldOffset(0)]
			public fixed byte Distance[2];

			[FieldOffset(2)]
			public byte Reflectivity;
		};
	}
}
