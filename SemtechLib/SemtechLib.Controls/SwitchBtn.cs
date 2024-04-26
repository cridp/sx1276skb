using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemtechLib.Controls
{
	public sealed class SwitchBtn : Control
	{
		private bool _checked;

		private ContentAlignment controlAlign = ContentAlignment.MiddleCenter;

		private Size itemSize = default(Size);

		[Description("Indicates whether the component is in the checked state")]
		[Category("Appearance")]
		[DefaultValue(false)]
		private bool Checked
		{
			get => _checked;
			set
			{
				_checked = value;
				Invalidate();
			}
		}

		[Category("Appearance")]
		[Description("Indicates how the LED should be aligned")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		public ContentAlignment ControlAlign
		{
			get => controlAlign;
			set
			{
				controlAlign = value;
				Invalidate();
			}
		}

		private Point PosFromAlignment
		{
			get
			{
				var result = default(Point);
				switch (controlAlign)
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

		private Size ItemSize
		{
			get => itemSize;
			set
			{
				itemSize = value;
				Invalidate();
			}
		}

		public new event PaintEventHandler Paint;

		public SwitchBtn()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.DoubleBuffer, value: true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
			SetStyle(ControlStyles.UserPaint, value: true);
			SetStyle(ControlStyles.ResizeRedraw, value: true);
			BackColor = Color.Transparent;
			Width = 15;
			Height = 25;
			itemSize.Width = 10;
			itemSize.Height = 23;
			MouseDown += mouseDown;
			MouseUp += mouseUp;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Paint != null)
			{
				Paint(this, e);
				return;
			}
			base.OnPaint(e);
			if (Enabled)
			{
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, 0)), PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(150, 150, 150)), PosFromAlignment.X + 2, PosFromAlignment.Y + 5, itemSize.Width - 4, itemSize.Height - 10);
				if (Checked)
				{
					e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0)), PosFromAlignment.X + 3, PosFromAlignment.Y + 6, itemSize.Width - 6, itemSize.Height - 16);
				}
				else
				{
					e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0)), PosFromAlignment.X + 3, PosFromAlignment.Y + 10, itemSize.Width - 6, itemSize.Height - 16);
				}
			}
			else
			{
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, 120, 120)), PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(150, 150, 150)), PosFromAlignment.X + 2, PosFromAlignment.Y + 5, itemSize.Width - 4, itemSize.Height - 10);
				if (Checked)
				{
					e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 100, 100)), PosFromAlignment.X + 3, PosFromAlignment.Y + 6, itemSize.Width - 6, itemSize.Height - 16);
				}
				else
				{
					e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 100, 100)), PosFromAlignment.X + 3, PosFromAlignment.Y + 10, itemSize.Width - 6, itemSize.Height - 16);
				}
			}
		}

		private void mouseDown(object sender, MouseEventArgs e)
		{
			buttonDown();
		}

		private void mouseUp(object sender, MouseEventArgs e)
		{
			buttonUp();
		}

		private void buttonDown()
		{
			Invalidate();
		}

		private void buttonUp()
		{
			Checked = !Checked;
			Invalidate();
		}
	}
}
