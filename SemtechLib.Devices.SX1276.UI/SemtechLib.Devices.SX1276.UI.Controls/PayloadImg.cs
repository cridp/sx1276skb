using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276.UI.Controls
{
	public sealed class PayloadImg : Control
	{
		public new event PaintEventHandler Paint;

		public PayloadImg()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.DoubleBuffer, value: true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
			SetStyle(ControlStyles.UserPaint, value: true);
			SetStyle(ControlStyles.ResizeRedraw, value: true);
			BackColor = Color.Transparent;
			Size = new Size(526, 20);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Paint != null)
			{
				Paint(this, e);
				return;
			}
			base.OnPaint(e);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			Image image = new Bitmap(Width, Height);
			var graphics = Graphics.FromImage(image);
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			var rect = new RectangleF(0f, 0f, Width, Height);
			Brush brush = new SolidBrush(SystemColors.ActiveBorder);
			graphics.DrawLine(new Pen(brush, 2f), rect.Left, rect.Bottom, rect.Right - 138f, rect.Top);
			graphics.DrawLine(new Pen(brush, 2f), rect.Right - 52f, rect.Top, rect.Right, rect.Bottom);
			e.Graphics.DrawImage(image, rect);
		}
	}
}
