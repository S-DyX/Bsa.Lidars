using System.Collections.Generic;
using Bsa.Velodyne.Entities;

namespace Bsa.Velodyne
{
	public static class DataMapper
	{
		private static HashSet<int> _BadReturnTypes = new HashSet<int>();
		private static HashSet<int> _BadLidarTypes = new HashSet<int>();

		public static ReturnType ReturnType_(int value)
		{
			switch (value)
			{
				case 0x37:
					return ReturnType.Strongest;
				case 0x38:
					return ReturnType.Last;
				case 0x39:
					return ReturnType.Dual;
				default:
					lock (_BadReturnTypes)
						if (!_BadReturnTypes.Contains(value))
						{
							_BadReturnTypes.Add(value);
							//Logger.WriteWarning(typeof(ReturnType), "Unrecognized ReturnType: " + rt);
						}
					return ReturnType.NAN;
			}
		}

		public static VelodyneModel VelodyneModel_(int value)
		{
			switch (value)
			{
				case 0x21:
					return VelodyneModel.HDL_32E;
				case 0x22:
					return VelodyneModel.VLP_16;
				default:
					lock (_BadReturnTypes)
						if (!_BadLidarTypes.Contains(value))
						{
							_BadLidarTypes.Add(value);
							//Logger.WriteWarning(typeof(VelodyneModel), "Unrecognized VelodyneModel: " + lt);
						}
					return VelodyneModel.NAN;
			}
		}
	}
}
