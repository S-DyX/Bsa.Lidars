using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Bsa.RpLidar.Entities;

namespace Bsa.RpLidar.WinForms
{
	public static class PointHelpers
	{
		public static LidarAngleSide FindByAngle(this List<LidarAngleSide> agnleSides, float angle)
		{
			foreach (var lidarAngleSide in agnleSides)
			{
				if (lidarAngleSide.CompareAngle(angle))
				{
					return lidarAngleSide;
				}
			}

			return null;
		}
		public static bool CompareAngle(this LidarAngleSide settingsAngleSide, float angle)
		{
			if (settingsAngleSide.From > settingsAngleSide.To)
			{
				if ((settingsAngleSide.From < angle && angle <= 360) ||
				    (angle >= 0 && settingsAngleSide.To >= angle))
				{
					return true;
				}
				else
				{
					return false;
				}
			}


			return settingsAngleSide.From < angle && settingsAngleSide.To >= angle;
		}
		public static double DegreeToRadian(this double angle)
		{
			return Math.PI * angle / 180.0;
		}
		public static double DegreeToRadian(this float angle)
		{
			return Math.PI * angle / 180.0;
		}
		public static double RadianToDegree(this double angle)
		{
			return angle * (180.0 / Math.PI);
		}
		public static PointF ToPointF(PointF origin, float rotation, float angle, float distance)
		{
			double angleRadian = (angle + rotation).DegreeToRadian();
			double dblX = origin.X + Math.Cos(angleRadian) * distance;
			double dblY = origin.Y + Math.Sin(angleRadian) * distance;

			float x = Convert.ToSingle(dblX);
			float y = Convert.ToSingle(dblY);
			var pointF = new PointF(x, y);
			return pointF;
		}

		public static PointF ToPointF(this PointF origin, float rotation, LidarPoint lidarPoint)
		{
			var distance = lidarPoint.Distance;
			if (distance > 0)
				distance = distance / 100;
			if (distance > 500)
				;
			var pointF = ToPointF(origin, rotation, lidarPoint.Angle, distance);
			return pointF;
		}

	
	}
}
