using System;
using System.Collections.Generic;
using System.Text;

namespace Bsa.RpLidar.Entities
{
	public enum RobotSideType
	{
		Unknown = 0,
		Left = 1,
		Right = 2,
		Back = 4,
		Forward = 8,
		Side = 16,
		Bottom = 32,
		Top = 64,
		Center = 128,
	}
	public static class RobotSideTypeHelper
	{
		public static RobotSideType GetOppositeSide(this RobotSideType sideType)
		{
			switch (sideType)
			{
				case RobotSideType.Back:
					return RobotSideType.Forward;
				case RobotSideType.Forward:
					return RobotSideType.Back;
				case RobotSideType.Left:
					return RobotSideType.Right;
				case RobotSideType.Right:
					return RobotSideType.Left;
				default:
					var result = sideType == RobotSideType.Unknown;
					if (sideType.HasFlag(RobotSideType.Forward))
					{
						//result = result | RobotSideType.Back;
					}

					break;
			}

			return RobotSideType.Unknown;
		}

	}
}
