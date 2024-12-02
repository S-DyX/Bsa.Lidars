using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bsa.RpLidar;
using Bsa.RpLidar.Entities;

namespace Bsa.RpLidar.WinForms
{
	public partial class Form1 : Form
	{
		private RpLidarSerialDevice _service;
		private LidarSettings _settings;
		public Form1()
		{
			InitializeComponent();
			_settings = new LidarSettings();
			for (int i = 3; i < 15; i++)
			{
				toolPorts.Items.Add($"Com{i}");
			}

			toolPorts.SelectedIndex = 0;
			_service = new RpLidarSerialDevice(_settings);
			_service.LidarPointGroupScanEvent += _service_LidarPointScanEvent;
		}

		private void _service_LidarPointScanEvent(LidarPointGroup points)
		{
			Render(points.GetPoints().ToList(), Color.Blue);
		}
		private void Render(List<LidarPoint> points, Color colorRed)
		{
			if (points == null || points.Count == 0)
				return;

			var matrixSize = 350;
			var originX = Convert.ToSingle(matrixSize / 2);
			var originY = Convert.ToSingle(matrixSize / 2);
			var origin = new PointF(originX, originY);


			this.BeginInvoke((Action)(() =>
			{

				var minX = double.MaxValue;
				var minY = double.MaxValue;
				var minPos = 0;
				var pm2 = new PictureMatrix(matrixSize, matrixSize);
				for (var pos = 0; pos < points.Count; pos++)
				{
					var data = points[pos];

					var distPixel = data.Distance / 100;
					var rad = (float)(data.Angle * Math.PI / 180.0);
					var endptX = Math.Sin(rad) * (distPixel) + originX;
					var endptY = originY - Math.Cos(rad) * (distPixel);
					if (endptX < minX)
					{
						minPos = pos;
						minY = endptY;
						minX = endptX;
					}
			

					int brightness = (data.Quality << 1) + 128;
					if (brightness > 255) brightness = 255;
					var color = Color.FromArgb(brightness, 0, 0);
					if (endptX > 0 && endptY > 0 && endptY < matrixSize && endptX < matrixSize)
						pm2.SetPixel((int)endptX, (int)endptY, color);
				}
				pm2.SetPixel((int)minX, (int)minY, Color.Green);
				pm2.SetPixel((int)origin.X, (int)origin.Y, Color.Black);
				pictureBoxLidar.Image = pm2.GetImage();
			}));

			this.BeginInvoke((Action)(() =>
			{


				var pointFs = points.Select(x => origin.ToPointF(270, x)).ToList();
				var pm = new PictureMatrix(matrixSize, matrixSize);

				foreach (var pointF in pointFs)
				{
					if (pointF.Y > 0 && pointF.X > 0 && pointF.Y < matrixSize && pointF.X < matrixSize)
						pm.SetPixel((int)pointF.X, (int)pointF.Y, colorRed);
				}

				 
				pm.SetPixel((int)origin.X, (int)origin.Y, Color.Black);
				pictureBoxLidar.Image = pm.GetImage();
			}));
		}
		private void toolStart_Click(object sender, EventArgs e)
		{
			try
			{

				if (toolPorts.SelectedItem != null)
				{
					_settings.Port = toolPorts.SelectedItem.ToString();
				}
				if (!string.IsNullOrEmpty(toolBaudRate.Text))
				{
					_settings.BaudRate = int.Parse(toolBaudRate.Text);
				}

				_service.Start();
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void toolStop_Click(object sender, EventArgs e)
		{
			_service.Stop();
		}
	}
}
