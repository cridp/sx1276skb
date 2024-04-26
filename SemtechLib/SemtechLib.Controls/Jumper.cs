using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemtechLib.Controls
{
	public sealed class Jumper : Control
	{
		private bool _checked;

		private ContentAlignment jumperAlign = ContentAlignment.MiddleCenter;

		private Size itemSize = default(Size);

		[DefaultValue(false)]
		[Category("Appearance")]
		[Description("Indicates whether the component is in the checked state")]
		public bool Checked
		{
			get => _checked;
			set
			{
				_checked = value;
				Invalidate();
			}
		}

		[Description("Indicates how the Jumper should be aligned")]
		[Category("Appearance")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		public ContentAlignment JumperAlign
		{
			get => jumperAlign;
			set
			{
				jumperAlign = value;
				Invalidate();
			}
		}

		private Point PosFromAlignment
		{
			get
			{
				var result = default(Point);
				switch (jumperAlign)
				{
				case ContentAlignment.TopLeft:
					result.X = 0;
					result.Y = 0;
					return result;
				case ContentAlignment.TopCenter:
					result.X = (int)(Width / 2.0 - itemSize.Width / 2.0);
					result.Y = 0;
					return result;
				case ContentAlignment.TopRight:
					result.X = Width - itemSize.Width;
					result.Y = 0;
					return result;
				case ContentAlignment.MiddleLeft:
					result.X = 0;
					result.Y = (int)(Height / 2.0 - itemSize.Height / 2.0);
					return result;
				case ContentAlignment.MiddleCenter:
					result.X = (int)(Width / 2.0 - itemSize.Width / 2.0);
					result.Y = (int)(Height / 2.0 - itemSize.Height / 2.0);
					return result;
				case ContentAlignment.MiddleRight:
					result.X = Width - itemSize.Width;
					result.Y = (int)(Height / 2.0 - itemSize.Height / 2.0);
					return result;
				case ContentAlignment.BottomLeft:
					result.X = 0;
					result.Y = Height - itemSize.Height;
					return result;
				case ContentAlignment.BottomCenter:
					result.X = (int)(Width / 2.0 - itemSize.Width / 2.0);
					result.Y = Height - itemSize.Height;
					return result;
				case ContentAlignment.BottomRight:
					result.X = Width - itemSize.Width;
					result.Y = Height - itemSize.Height;
					return result;
				default:
					result.X = 0;
					result.Y = 0;
					return result;
				}
			}
		}

		public new event PaintEventHandler Paint;

		public Jumper()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.DoubleBuffer, value: true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
			SetStyle(ControlStyles.UserPaint, value: true);
			SetStyle(ControlStyles.ResizeRedraw, value: true);
			BackColor = Color.Transparent;
			Size = new Size(19, 35);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Paint != null)
			{
				Paint(this, e);
				return;
			}
			base.OnPaint(e);
			itemSize.Width = Size.Width * 66 / 100;
			itemSize.Height = Size.Height * 92 / 100;
			if (Enabled)
			{
				var size = new Size(itemSize.Width * 40 / 100, itemSize.Width * 40 / 100);
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(150, 150, 150)), PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0)), PosFromAlignment.X + itemSize.Width / 2 - size.Width / 2, PosFromAlignment.Y + itemSize.Height / 4 - size.Height / 2, size.Width, size.Height);
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0)), PosFromAlignment.X + itemSize.Width / 2 - size.Width / 2, PosFromAlignment.Y + itemSize.Height / 2 - size.Height / 2, size.Width, size.Height);
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0)), PosFromAlignment.X + itemSize.Width / 2 - size.Width / 2, PosFromAlignment.Y + 3 * (itemSize.Height / 4) - size.Height / 2, size.Width, size.Height);
				if (Checked)
				{
					e.Graphics.FillRectangle(new SolidBrush(ForeColor), PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, 3 * (itemSize.Height / 5));
				}
				else
				{
					e.Graphics.FillRectangle(new SolidBrush(ForeColor), PosFromAlignment.X, PosFromAlignment.Y + 2 * (itemSize.Height / 5), itemSize.Width, 3 * (itemSize.Height / 5));
				}
			}
			else
			{
				var size2 = new Size(itemSize.Width * 40 / 100, itemSize.Width * 40 / 100);
				e.Graphics.FillRectangle(new SolidBrush(SystemColors.InactiveCaption), PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
				e.Graphics.FillRectangle(new SolidBrush(SystemColors.InactiveBorder), PosFromAlignment.X + itemSize.Width / 2 - size2.Width / 2, PosFromAlignment.Y + itemSize.Height / 4 - size2.Height / 2, size2.Width, size2.Height);
				e.Graphics.FillRectangle(new SolidBrush(SystemColors.InactiveBorder), PosFromAlignment.X + itemSize.Width / 2 - size2.Width / 2, PosFromAlignment.Y + itemSize.Height / 2 - size2.Height / 2, size2.Width, size2.Height);
				e.Graphics.FillRectangle(new SolidBrush(SystemColors.InactiveBorder), PosFromAlignment.X + itemSize.Width / 2 - size2.Width / 2, PosFromAlignment.Y + 3 * (itemSize.Height / 4) - size2.Height / 2, size2.Width, size2.Height);
			}
		}
	}
}
