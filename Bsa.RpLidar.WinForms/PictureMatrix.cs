using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Bsa.RpLidar.WinForms
{
	public sealed class PictureMatrix
	{
		private int[] _matrix;
		private Color[] _matrixColors;

		private readonly SortedDictionary<int, List<Point>> _dictionary = new SortedDictionary<int, List<Point>>();

		public int Height;
		public int Width;
		public int Step { get; private set; }

		public PictureMatrix(Bitmap photo)
			: this(photo, 1)
		{
		}
		public PictureMatrix(int width, int height)
		{
			Height = height;
			Width = width;
			Step = 1;
			_matrix = new int[Height * Width];
			_matrixColors = new Color[Height * Width];
		}

		protected PictureMatrix(int[] matrixGray, Color[] matrixColors)
		{
			_matrix = matrixGray;
			_matrixColors = matrixColors;
		}
		public PictureMatrix(Bitmap photo, int step)
		{
			if (step <= 0)
				step = 1;
			Step = step;
			Height = photo.Height / Step;
			Width = photo.Width / Step;
			_matrix = new int[Height * Width];
			_matrixColors = new Color[Height * Width];
			//Fill(photo);
			Fast(photo);
		}

		private void Fast(Bitmap photo)
		{
			var data = photo.BitmapToByteArray();
			_matrixColors = data.Item1;
			_matrix = data.Item2;
			//for (int i = 0; i < _matrixColors.Length; i++)
			//{
			//	var grayD = (int)_matrixColors[i].ToGrayD();
			//	_matrix[i] = grayD;
			//}
		}

		private void Fill(Bitmap photo)
		{
			for (int y = 0; y < Height; y++)
			{
				var offset = Width * y;
				var yval = y * Step;
				for (int x = 0; x < Width; x++)
				{
					var color = photo.GetPixel(x * Step, yval);
					var index0 = offset + x;
					_matrixColors[index0] = color;
					var grayD = (int)color.ToGrayD();
					_matrix[index0] = grayD;
				}
			}
		}

		public PictureMatrix(PictureMatrix photo, int step)
		{
			if (step <= 0)
				step = 1;
			Step = step;
			Height = photo.Height / Step;
			Width = photo.Width / Step;
			_matrix = new int[Height * Width];
			_matrixColors = new Color[Height * Width];
			for (int y = 0; y < Height; y++)
			{
				var offset = Width * y;
				var localY = y * Step;
				for (int x = 0; x < Width; x++)
				{
					var color = photo.GetPixel(x * Step, localY);
					var index0 = offset + x;
					_matrixColors[index0] = color;
					var grayD = (int)color.ToGrayD();
					_matrix[index0] = grayD;
				}
			}
		}

		public PictureMatrix(PictureMatrix photo, Rectangle rect, double prop)
		{
			Step = 1;
			Height = (int)(photo.Height * prop);
			Width = (int)(photo.Width * prop);
			_matrix = new int[Height * Width];
			_matrixColors = new Color[Height * Width];
			for (int y = rect.Y; y < rect.Height + rect.Y; y++)
			{
				var offset = Width * y;
				var localY = y * Step;
				for (int x = rect.X; x < rect.X + rect.Width; x++)
				{
					var color = photo.GetPixel(x * Step, localY);
					var index0 = offset + x;
					_matrixColors[index0] = color;
					var grayD = (int)color.ToGrayD();
					_matrix[index0] = grayD;
				}
			}
		}



		public Point GetOriginalXy(int x, int y)
		{
			return new Point(Step * x, Step * y);
		}

		public Bitmap GetImage()
		{
			var result = new Bitmap(Width, Height);
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					result.SetPixel(x, y, GetPixel(x, y));
				}
			}
			return result;
		}

		public PictureMatrix GetSubMatrix(int x, int y, int width, int height)
		{
			var pm = new PictureMatrix(width, height);
			var localX = 0;
			var localY = 0;
			var xSize = x + width;
			var ySize = y + height;


			for (int j = y; j < this.Height && j < ySize; j++)
			{
				if (localY >= height)
					break;
				for (int i = x; i < this.Width && i < xSize; i++)
				{
					if (localX >= width)
						break;
					var pixel = this.GetPixel(i, j);
					pm.SetPixel(localX, localY, pixel);
					localX++;
				}
				localY++;

			}
			return pm;
		}


		private void AddPoints(int grayD, int x, int y)
		{
			var points = new List<Point>();
			if (!_dictionary.ContainsKey(grayD))
			{
				_dictionary.Add(grayD, points);
			}
			else
			{
				points = _dictionary[grayD];
			}
			points.Add(new Point(x, y));
		}

		public void SetPixel(int x, int y, Color color)
		{
			if (x >= Width)
				throw new ArgumentOutOfRangeException($"{x}>={Width}");
			if (y >= Height)
				throw new ArgumentOutOfRangeException($"{y}>={Height}");
			var index0 = Width * y + x;
			_matrixColors[index0] = color;
			_matrix[index0] = (int)color.ToGrayD();
		}

		public void SetPixel(int index, Color color)
		{
			if (index >= _matrixColors.Length)
				throw new ArgumentOutOfRangeException($"{index}>={_matrixColors.Length}");
			_matrixColors[index] = color;
			_matrix[index] = (int)color.ToGrayD();
		}

		public int Size
		{
			get { return _matrixColors.Length; }
		}

		public int GetGrayPixel(int x, int y)
		{
			if (x >= Width)
				throw new ArgumentOutOfRangeException($"{x}>={Width}");
			if (y >= Height)
				throw new ArgumentOutOfRangeException($"{y}>={Height}");
			var index0 = Width * y + x;
			if (index0 >= Size)
				throw new ArgumentOutOfRangeException($"{index0}>={Size}");
			return _matrix[index0];
		}

		public int GetGrayPixel(int index)
		{
			if (index >= _matrix.Length)
				throw new ArgumentOutOfRangeException($"{index}>={_matrix.Length}");
			return _matrix[index];
		}

		public Color GetPixel(int index)
		{
			return _matrixColors[index];
		}

		public Point GetPointByIndex(int index)
		{
			if (index < 0)
				return new Point(-1, -1);
			if (index > _matrix.Length)
				return new Point(-1, -1);
			var y = index / Width;
			var x = index - Width * y;
			return new Point(x, y);
		}
		public int GetIndex(int x, int y)
		{
			if (x < 0)
				return -1;
			if (x >= Width)
				return -1;
			return Width * y + x;
		}

		public int GetOffset(int y)
		{
			if (y >= Height)
				return -1;
			if (y < 0)
				return -1;

			return Width * y;
		}

		public Color GetPixel(int x, int y)
		{
			if (x >= Width)
				throw new ArgumentOutOfRangeException($"{x}>={Width}");
			if (y >= Height)
				throw new ArgumentOutOfRangeException($"{y}>={Height}");
			var index0 = Width * y + x;
			if (index0 >= _matrixColors.Length)
				throw new ArgumentOutOfRangeException($"{index0}>={_matrixColors.Length}");
			return _matrixColors[index0];
		}



		public PictureMatrix Clone()
		{
			var matrix = new int[Height * Width];
			var matrixColors = new Color[Height * Width];
			_matrix.CopyTo(matrix, 0);
			_matrixColors.CopyTo(matrixColors, 0);
			var result = new PictureMatrix(matrix, matrixColors);
			result.Width = Width;
			result.Height = Height;
			return result;

		}
	}
}
