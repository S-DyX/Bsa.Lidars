using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

namespace Bsa.RpLidar.WinForms
{
	public static class PmHelper
	{
		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static Tuple<Color[], int[]> BitmapToByteArray(this Bitmap bitmap)
		{
			//var clone = bitmap.Clone() as Bitmap;
			BitmapData bmpdata = null;

			try
			{
				bmpdata = Lock(bitmap);
				var height = bitmap.Height;
				var width = bitmap.Width;
				var stride = bmpdata.Stride;
				int numbytes = bmpdata.Stride * height;
				var step = bmpdata.Stride / width;
				byte[] bytedata = new byte[numbytes];
				var colors = new Color[width * height];
				var gray = new int[width * height];

				IntPtr ptr = bmpdata.Scan0;

				Marshal.Copy(ptr, bytedata, 0, numbytes);
				for (int y = 0; y < height; y++)
				{
					var offset = width * y;
					var strideOffset = stride * y;
					for (int x = 0; x < width; x++)
					{
						var index = strideOffset + x * step;
						var r = bytedata[index + 2];
						var green = bytedata[index + 1];
						var blue = bytedata[index];
						var color = Color.FromArgb(r, green, blue);
						var index0 = offset + x;
						colors[index0] = color;
						gray[index0] = (int)PmHelper.ToGrayD(r, green, blue);
						//if (clone.GetPixel(x, y) != color)
						//{
						//	;
						//}

					}
				}
				return Tuple.Create(colors, gray);
			}
			finally
			{
				if (bmpdata != null)
					bitmap.UnlockBits(bmpdata);
			}

		}

		private static BitmapData Lock(Bitmap bitmap)
		{
			var height = bitmap.Height;
			var width = bitmap.Width;
			return bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
		}

		public static double CalcDispersia(this SortedDictionary<int, int> sortedDictionary, int intervals)
		{
			var n = sortedDictionary.Count / intervals;
			int countPoints = 0;
			int sumKV = 0;
			foreach (var s in sortedDictionary)
			{
				countPoints += s.Value;
				sumKV += s.Key * s.Value;
			}

			var avrgAll = ((float)sumKV) / countPoints;
			double sum = 0;
			//var sb = new StringBuilder();
			for (int i = 0; i < n; i++)
			{
				var count = 0;

				float avrg = 0;
				int max = int.MinValue;
				int min = int.MaxValue;
				for (int j = 0; j < intervals; j++)
				{
					var item = j + i * intervals;
					if (sortedDictionary[item] > 0)
					{
						if (min > item)
							min = item;
						if (max < item)
							max = item;
						//count++;
					}
					count += sortedDictionary[item];
				}
				var avrgLocal = (max + min) / 2;
				var d = Math.Pow((avrgLocal - avrgAll), 2) * count;
				//if (d > 0)
				//{
				//	sb.Append($"interval from:{min} to:{max}. value:{d}, count:{count} ");
				//}

				sum += d;
			}
			var res = sum / countPoints;
			return res;
		}


		public static string GetContent(string url, Encoding encoding, string postData)
		{

			encoding = encoding ?? Encoding.UTF8;
			var request = (HttpWebRequest)WebRequest.Create(url);
			if (!string.IsNullOrEmpty(postData))
			{
				request.Method = "POST";
			}
			request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/7.0)";
			request.Accept = "text/html, application/xhtml+xml, */*";
			request.Headers.Add("Accept-Language", "ru");
			request.ContentType = "application/json";
			request.KeepAlive = true;
			request.AllowAutoRedirect = false;
			request.Timeout = 350000;



			if (!string.IsNullOrEmpty(postData))
			{
				// Write post data
				var dataBytes = encoding.GetBytes(postData);
				request.ContentLength = dataBytes.Length;
				using (var requestStream = request.GetRequestStream())
				{
					requestStream.Write(dataBytes, 0, dataBytes.Length);

				}
				return GetContent(encoding, request);
			}
			else
			{
				return GetContent(encoding, request);
			}




		}

		public static double ToGrayD(this Color pixel)
		{
			return (pixel.R * 0.2989) + (pixel.G * 0.5870) + (pixel.B * 0.1140);
		}
		public static double ToGrayD(byte red, byte green, byte blue)
		{
			return (red * 0.2989) + (green * 0.5870) + (blue * 0.1140);
		}
		public static string GetContent(string url, Encoding encoding, byte[] bytes)
		{
			using (WebClient client = new WebClient())
			{
				//client.Headers.AddAvrg("Content-Type", "application/octet-stream");
				//using (Stream requestStream = client.OpenWrite(url, "POST"))
				//{
				//	stream.CopyTo(requestStream);
				//}
				var result = client.UploadData(url, "POST", bytes);
				return encoding.GetString(result, 0, result.Length);
			}
		}

		private static string GetContent(Encoding encoding, HttpWebRequest request)
		{
			using (var webResponse = (HttpWebResponse)request.GetResponse())
			{
				if (webResponse.Headers["Content-Type"] != null &&
					webResponse.Headers["Content-Type"].ToLower().IndexOf("windows-1251") > 0)
				{
					encoding = Encoding.GetEncoding("windows-1251");
				}
				if (webResponse.Headers["Content-Type"] != null && webResponse.Headers["Content-Type"].ToLower().Contains("utf-8"))
				{
					encoding = Encoding.UTF8;
				}


				using (var stream = webResponse.GetResponseStream())
				{
					if (stream != null)
					{
						using (var streamReader = new StreamReader(stream, encoding))
						{
							var res = string.Empty;
							if (Encoding.UTF8 != encoding)
							{
								var content = streamReader.ReadToEnd();
								var bytes = encoding.GetBytes(content);
								return Encoding.UTF8.GetString(Encoding.Convert(encoding, Encoding.UTF8, bytes));
							}
							return streamReader.ReadToEnd();
						}
					}
				}
			}
			return string.Empty;
		}

		static readonly ImageConverter ImageConverter = new ImageConverter();
		public static byte[] Converter(this System.Drawing.Image x)
		{

			byte[] xByte = (byte[])ImageConverter.ConvertTo(x, typeof(byte[]));
			return xByte;
		}

	
	}
}
