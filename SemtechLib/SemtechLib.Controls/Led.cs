using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemtechLib.Controls
{
	public sealed class Led : Control
	{
		private bool _checked;

		private Color ledColor = Color.Green;

		private ContentAlignment ledAlign = ContentAlignment.MiddleCenter;

		private Size itemSize = new(11, 11);

		[Category("Appearance")]
		[DefaultValue(false)]
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

		[Category("Appearance")]
		[Description("Indicates the color of the LED")]
		public Color LedColor
		{
			get => ledColor;
			set
			{
				ledColor = value;
				Invalidate();
			}
		}

		[DefaultValue(ContentAlignment.MiddleCenter)]
		[Category("Appearance")]
		[Description("Indicates how the LED should be aligned")]
		public ContentAlignment LedAlign
		{
			get => ledAlign;
			set
			{
				ledAlign = value;
				Invalidate();
			}
		}

		[Category("Layout")]
		[Description("Sets the size of the led")]
		public Size LedSize
		{
			get => itemSize;
			set
			{
				itemSize = value;
				Invalidate();
			}
		}

		private Point PosFromAlignment
		{
			get
			{
				var result = default(Point);
				switch (ledAlign)
				{
				case ContentAlignment.TopLeft:
					result.X = 1;
					result.Y = 1;
					return result;
				case ContentAlignment.TopCenter:
					result.X = (int)(Width / 2.0 - itemSize.Width / 2.0);
					result.Y = 1;
					return result;
				case ContentAlignment.TopRight:
					result.X = Width - itemSize.Width - 1;
					result.Y = 1;
					return result;
				case ContentAlignment.MiddleLeft:
					result.X = 1;
					result.Y = (int)(Height / 2.0 - itemSize.Height / 2.0);
					return result;
				case ContentAlignment.MiddleCenter:
					result.X = (int)(Width / 2.0 - itemSize.Width / 2.0);
					result.Y = (int)(Height / 2.0 - itemSize.Height / 2.0);
					return result;
				case ContentAlignment.MiddleRight:
					result.X = Width - itemSize.Width - 1;
					result.Y = (int)(Height / 2.0 - itemSize.Height / 2.0);
					return result;
				case ContentAlignment.BottomLeft:
					result.X = 0;
					result.Y = Height - itemSize.Height - 1;
					return result;
				case ContentAlignment.BottomCenter:
					result.X = (int)(Width / 2.0 - itemSize.Width / 2.0);
					result.Y = Height - itemSize.Height - 1;
					return result;
				case ContentAlignment.BottomRight:
					result.X = Width - itemSize.Width - 1;
					result.Y = Height - itemSize.Height - 1;
					return result;
				default:
					result.X = 0;
					result.Y = 0;
					return result;
				}
			}
		}

		public new event PaintEventHandler Paint;

		public Led()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.DoubleBuffer, value: true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
			SetStyle(ControlStyles.UserPaint, value: true);
			SetStyle(ControlStyles.ResizeRedraw, value: true);
			BackColor = Color.Transparent;
			Size = new Size(15, 15);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Paint != null)
			{
				Paint(this, e);
				return;
			}
			base.OnPaint(e);
			var num = 1f - Width / (float)Height;
			var angle = 50f - 15f * num;
			var rectangle = new Rectangle(PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
			var linearGradientBrush = new LinearGradientBrush(rectangle, ControlPaint.Dark(Parent.BackColor), ControlPaint.LightLight(Parent.BackColor), angle);
            var blend = new Blend
            {
                Positions = new float[6] { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f },
                Factors = new float[6] { 0.2f, 0.2f, 0.4f, 0.4f, 1f, 1f }
            };
            linearGradientBrush.Blend = blend;
			var rect = rectangle;
			rect.Inflate(1, 1);
			e.Graphics.FillEllipse(linearGradientBrush, rect);
			if (Enabled)
			{
				if (Checked)
				{
					e.Graphics.FillEllipse(new SolidBrush(ControlPaint.Light(ledColor)), PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
				}
				else
				{
					e.Graphics.FillEllipse(new SolidBrush(ControlPaint.Dark(ledColor)), PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
				}
			}
			var linearGradientBrush2 = new LinearGradientBrush(rectangle, Color.FromArgb(150, 255, 255, 255), Color.Transparent, angle);
			var linearGradientBrush3 = new LinearGradientBrush(rectangle, Color.FromArgb(100, 255, 255, 255), Color.FromArgb(100, 255, 255, 255), angle);
            var blend2 = new Blend
            {
                Positions = new float[6] { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f },
                Factors = new float[6] { 0.2f, 0.2f, 0.4f, 0.4f, 1f, 1f }
            };
            linearGradientBrush2.Blend = blend2;
			linearGradientBrush3.Blend = blend2;
			e.Graphics.FillEllipse(linearGradientBrush3, PosFromAlignment.X + itemSize.Width * 13 / 100, PosFromAlignment.Y + itemSize.Height * 13 / 100, itemSize.Width * 40 / 100, itemSize.Height * 40 / 100);
			e.Graphics.FillEllipse(linearGradientBrush2, new Rectangle(PosFromAlignment, itemSize));
		}
	}
}
