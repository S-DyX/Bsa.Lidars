using System;
using System.Collections.Generic;
using System.Text;
using Bsa.RpLidar.Entities;

namespace Bsa.RpLidar.Helpers
{
	public static class CommandHelper
	{
		public static byte GetByte(this Command command)
		{
			return (byte)command;
		}

		public static bool HasResponse(this Command command)
		{
			return command != Command.Stop && command != Command.Reset;
		}
		public static bool GetHasResponse(byte command)
		{
			return command != (byte)Command.Stop && command != (byte)Command.Reset;
		}
		public static int GetSleepInterval(this Command command)
		{
			if (command == Command.Reset || command == Command.Stop)
				return 20;
			return 0;
		}

		public static int GetMustSleep(this byte command)
		{
			return ((Command) command).GetSleepInterval(); 
		}
	}
}
