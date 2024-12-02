using System.Runtime.InteropServices;

namespace Bsa.Velodyne.Entities
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	internal unsafe struct RawPacket
	{
		/// <summary>
		/// Size of the structure in bytes
		/// </summary>
		public const int _Size = _FactoryE;
		public const int _VerticalBlockPairsPerPacket = 12;

		// private const int _HeaderE = 42; // UDP Will Filter This Heading OUT
		private const int _DataBlockE = VerticalBlockPair.RawBlockPair._Size * _VerticalBlockPairsPerPacket; // index after data block E(nd)
		private const int _TimeStampE = 4 + _DataBlockE; // index after time stamp E(nd)
		private const int _FactoryE = 2 + _TimeStampE; // index after factory E(nd)

		// [FieldOffset(0)]
		// public fixed byte _Header[42]; // UDP Will Filter This Heading OUT

		[FieldOffset(0)]
		public fixed byte _DataBlocks[VerticalBlockPair.RawBlockPair._Size * _VerticalBlockPairsPerPacket];

		[FieldOffset(_DataBlockE)]
		public fixed byte _TimeStamp[4];

		[FieldOffset(_TimeStampE)]
		public fixed byte _Factory[2];
	};
}
