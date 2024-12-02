using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Bsa.RpLidar.Helpers
{
	public static class ByteHelper
	{
		public static string ToStr(this byte[] source)
		{
			var sBuilder = new StringBuilder(256);
			foreach (byte t in source)
			{
				sBuilder.Append(t.ToString("x2"));
			}
			return sBuilder.ToString();
		}

		public static byte[] GetBytes<TStruct>(this TStruct str) where TStruct : struct
		{
			int size = Marshal.SizeOf(str);
			byte[] arr = new byte[size];

			IntPtr ptr = Marshal.AllocHGlobal(size);
			Marshal.StructureToPtr(str, ptr, true);
			Marshal.Copy(ptr, arr, 0, size);
			Marshal.FreeHGlobal(ptr);
			return arr;
		}

		public static List<TStruct> FromBytes<TStruct>(this byte[] arr, int offset = 0) where TStruct : struct
		{
			var value = new TStruct();
			var structureType = value.GetType();
			var size = Marshal.SizeOf(value);

			if (size == 0)
				return null;

			var len = arr.Length / size;
			var result = new List<TStruct>(len);

			while (offset < arr.Length)
			{
				IntPtr ptr = Marshal.AllocHGlobal(size);
				Marshal.Copy(arr, offset, ptr, size);
				value = (TStruct)Marshal.PtrToStructure(ptr, structureType);
				Marshal.FreeHGlobal(ptr);

				result.Add(value);
				offset += size;
			}


			return result;
		}
	
	}
}
