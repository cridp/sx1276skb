using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemtechLib.Controls
{
	public sealed class TempCtrl : Control
	{
		public sealed class RangeTypeConverter : TypeConverter
		{
			public override bool GetPropertiesSupported(ITypeDescriptorContext context)
			{
				return true;
			}

			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
			{
				return TypeDescriptor.GetProperties(typeof(Ranges));
			}
		}

		[TypeConverter(typeof(RangeTypeConverter))]
		[Category("Behavior")]
		[Description("Range.")]
		public sealed class Ranges
		{
			public delegate void PropertyChangedEventHandler();

			private double min;

			private double max;

			[Description("Minimum value.")]
			public double Min
			{
				get => min;
				set
				{
					min = value;
                    PropertyChanged?.Invoke();
                }
			}

			[Description("Maximum value.")]
			public double Max
			{
				get => max;
				set
				{
					max = value;
                    PropertyChanged?.Invoke();
                }
			}

			public event PropertyChangedEventHandler PropertyChanged;

			public Ranges()
			{
				min = -40.0;
				Max = 365.0;
			}

			public Ranges(double min, double max)
			{
				this.min = min;
				this.max = max;
			}

			public override string ToString()
			{
				return max + "; " + min;
			}
		}

		private bool enableTransparentBackground;

		private bool requiresRedraw;

		private Image backgroundImg;

		private RectangleF rectBackgroundImg;

		private Ranges range = new();

		private double value = 25.0;

		private bool drawTics = true;

		private int smallTicFreq = 5;

		private int largeTicFreq = 10;

		private Color colorFore;

		private Color colorBack;

		private Color colorScale;

		private Color colorScaleText;

		private Color colorOutline;

		private Color colorBackground;

		private Pen forePen;

		private Pen scalePen;

		private Pen outlinePen;

		private SolidBrush blackBrush;

		private SolidBrush fillBrush;

		private LinearGradientBrush bulbBrush;

		private Font fntText = new("Arial", 10f, FontStyle.Bold);

		private StringFormat strfmtText = new();

		private RectangleF rectCylinder;

		private RectangleF rectBulb;

		private PointF pointCenter;

		private float fTmpWidth;

		private float fRange;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Ranges Range
		{
			get => range;
			set
			{
				range = value;
				requiresRedraw = true;
				Invalidate();
			}
		}

		public double Value
		{
			get => value;
			set
			{
				this.value = value;
				if (value > range.Max)
				{
					this.value = range.Max;
				}
				if (value < range.Min)
				{
					this.value = range.Min;
				}
				Invalidate();
			}
		}

		public bool DrawTics
		{
			get => drawTics;
			set
			{
				drawTics = value;
				requiresRedraw = true;
				Invalidate();
			}
		}

		public int SmallTicFreq
		{
			get => smallTicFreq;
			set
			{
				smallTicFreq = value;
				requiresRedraw = true;
				Invalidate();
			}
		}

		public int LargeTicFreq
		{
			get => largeTicFreq;
			set
			{
				largeTicFreq = value;
				requiresRedraw = true;
				Invalidate();
			}
		}

		[Description("Enables or Disables Transparent Background color. Note: Enabling this will reduce the performance and may make the control flicker.")]
		[DefaultValue(false)]
		public bool EnableTransparentBackground
		{
			get => enableTransparentBackground;
			set
			{
				enableTransparentBackground = value;
				SetStyle(ControlStyles.OptimizedDoubleBuffer, !enableTransparentBackground);
				requiresRedraw = true;
				Refresh();
			}
		}

		public new event PaintEventHandler Paint;

		public TempCtrl()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, value: true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
			SetStyle(ControlStyles.UserPaint, value: true);
			SetStyle(ControlStyles.ResizeRedraw, value: true);
			Size = new Size(75, 253);
			ForeColor = Color.Red;
			colorFore = ForeColor;
			colorBack = SystemColors.Control;
			colorScale = Color.FromArgb(0, 0, 0);
			colorScaleText = Color.FromArgb(0, 0, 0);
			colorOutline = Color.FromArgb(64, 0, 0);
			colorBackground = Color.FromKnownColor(KnownColor.Transparent);
			EnabledChanged += TempCtrl_EnabledChanged;
			SizeChanged += TempCtrl_SizeChanged;
		}

		private void TempCtrl_EnabledChanged(object sender, EventArgs e)
		{
			requiresRedraw = true;
			Refresh();
		}

		private void TempCtrl_SizeChanged(object sender, EventArgs e)
		{
			requiresRedraw = true;
			Refresh();
		}

		private Color OffsetColor(Color color, short offset)
		{
			byte b = 0;
			byte b2 = 0;
			byte b3 = 0;
			var val = offset;
			var val2 = offset;
			var val3 = offset;
			if (offset < -255 || offset > 255)
			{
				return color;
			}
			b = color.R;
			b2 = color.G;
			b3 = color.B;
			if (offset > 0)
			{
				if (b + offset > 255)
				{
					val = (short)(255 - b);
				}
				if (b2 + offset > 255)
				{
					val2 = (short)(255 - b2);
				}
				if (b3 + offset > 255)
				{
					val3 = (short)(255 - b3);
				}
				offset = Math.Min(Math.Min(val, val2), val3);
			}
			else
			{
				if (b + offset < 0)
				{
					val = (short)(-b);
				}
				if (b2 + offset < 0)
				{
					val2 = (short)(-b2);
				}
				if (b3 + offset < 0)
				{
					val3 = (short)(-b3);
				}
				offset = Math.Max(Math.Max(val, val2), val3);
			}
			return Color.FromArgb(color.A, b + offset, b2 + offset, b3 + offset);
		}

		private void FillCylinder(Graphics g, RectangleF ctrl, Brush fillBrush, Color outlineColor)
		{
			var rect = ctrl with { Y = ctrl.Y - 5f, Height = 5f };
			var rect2 = ctrl with { Y = ctrl.Bottom - 5f, Height = 5f };
			var pen = new Pen(outlineColor);
			var graphicsPath = new GraphicsPath();
			graphicsPath.AddArc(rect, 0f, 180f);
			graphicsPath.AddArc(rect2, 180f, -180f);
			graphicsPath.CloseFigure();
			g.FillPath(fillBrush, graphicsPath);
			g.DrawPath(pen, graphicsPath);
			graphicsPath.Reset();
			graphicsPath.AddEllipse(rect);
			g.FillPath(fillBrush, graphicsPath);
			g.DrawPath(pen, graphicsPath);
		}

		private double Fahrenheit2Celsius(double fahrenheit)
		{
			return (fahrenheit - 32.0) / 1.8;
		}

		private double Celsius2Fahrenheit(double celsius)
		{
			return celsius * 1.8 + 32.0;
		}

		private void DrawBulb(Graphics g, RectangleF rect, bool enabled)
		{
			g.FillEllipse(bulbBrush, rectBulb);
			g.DrawEllipse(outlinePen, rectBulb);
		}

		private void DrawCylinder(Graphics g, RectangleF rect, bool enabled)
		{
			FillCylinder(g, rectCylinder, fillBrush, colorOutline);
		}

		private void DrawValue(Graphics g, RectangleF rect, bool enabled)
		{
			if (!enabled) return;
			fRange = (float)(Range.Max - Range.Min);
			var num = (float)value;
			if (Range.Min < 0.0)
			{
				num += Math.Abs((int)Range.Min);
			}
			if (num > 0f)
			{
				var num2 = num / fRange * 100f;
				var num3 = rectCylinder.Height / 100f * num2;
				var ctrl = new RectangleF(rectCylinder.Left, rectCylinder.Bottom - num3, rectCylinder.Width, num3);
				FillCylinder(g, ctrl, bulbBrush, colorOutline);
			}
			g.DrawString(layoutRectangle: new RectangleF(pointCenter.X + 10f, rectBulb.Bottom + 5f, 70f, 20f), s: value.ToString("0 [°C]"), font: fntText, brush: blackBrush, format: strfmtText);
			g.DrawString(layoutRectangle: new RectangleF(pointCenter.X - 80f, rectBulb.Bottom + 5f, 70f, 20f), s: Celsius2Fahrenheit(value).ToString("0 [°F]"), font: fntText, brush: blackBrush, format: strfmtText);
		}

		private void DrawTicks(Graphics g, RectangleF rect, bool enabled)
		{
			if (!drawTics) return;
			fRange = (float)(Range.Max - Range.Min);
			var font = new Font("Arial", 7f);
			var stringFormat = new StringFormat
			{
				Alignment = StringAlignment.Far,
				LineAlignment = StringAlignment.Center
			};
			var num = rectCylinder.Height / fRange;
			var num2 = num * largeTicFreq;
			var num3 = (long)Range.Max;
			for (var num4 = rectCylinder.Top; num4 <= rectCylinder.Bottom; num4 += num2)
			{
				g.DrawLine(pt1: new Point((int)rectCylinder.Right + 3, (int)num4), pt2: new Point((int)rectCylinder.Right + 10, (int)num4), pen: scalePen);
				g.DrawString(point: new PointF(rectCylinder.Right + 30f, num4), s: num3.ToString(), font: font, brush: blackBrush, format: stringFormat);
				num3 -= largeTicFreq;
			}
			num2 = num * smallTicFreq;
			for (var num5 = rectCylinder.Top; num5 <= rectCylinder.Bottom; num5 += num2)
			{
				g.DrawLine(pt1: new Point((int)rectCylinder.Right + 3, (int)num5), pt2: new Point((int)rectCylinder.Right + 8, (int)num5), pen: scalePen);
			}
			var num6 = Celsius2Fahrenheit(Range.Max);
			var num7 = (int)(num6 % 10.0);
			if (num7 != 0)
			{
				num7 = 10 - num7;
			}
			num6 -= num7;
			fRange = (float)(Celsius2Fahrenheit(Range.Max) - Celsius2Fahrenheit(Range.Min));
			num = rectCylinder.Height / fRange;
			num2 = num * largeTicFreq;
			num3 = (long)Celsius2Fahrenheit(Range.Min);
			for (var num8 = rectCylinder.Bottom; num8 >= rectCylinder.Top; num8 -= num2)
			{
				g.DrawLine(pt1: new Point((int)rectCylinder.Left - 10, (int)num8), pt2: new Point((int)rectCylinder.Left - 3, (int)num8), pen: scalePen);
				g.DrawString(point: new PointF(rectCylinder.Left - 15f, num8), s: num3.ToString(), font: font, brush: blackBrush, format: stringFormat);
				num3 += largeTicFreq;
			}
			num2 = num * smallTicFreq;
			for (var num9 = rectCylinder.Bottom; num9 >= rectCylinder.Top; num9 -= num2)
			{
				g.DrawLine(pt1: new Point((int)rectCylinder.Left - 8, (int)num9), pt2: new Point((int)rectCylinder.Left - 3, (int)num9), pen: scalePen);
			}
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
			DrawValue(graphics, rect, Enabled);
			e.Graphics.DrawImage(image, rect);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			if (!enableTransparentBackground)
			{
				base.OnPaintBackground(e);
			}
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			e.Graphics.FillRectangle(new SolidBrush(Color.Transparent), new RectangleF(0f, 0f, Width, Height));
			if (backgroundImg == null || requiresRedraw)
			{
				backgroundImg = new Bitmap(Width, Height);
				var graphics = Graphics.FromImage(backgroundImg);
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				rectBackgroundImg = new RectangleF(0f, 0f, Width, Height);
				pointCenter = new PointF(rectBackgroundImg.Left + rectBackgroundImg.Width / 2f, rectBackgroundImg.Top + rectBackgroundImg.Height / 2f);
				fTmpWidth = rectBackgroundImg.Width / 5f;
				rectBulb = new RectangleF(pointCenter.X - fTmpWidth, rectBackgroundImg.Bottom - (fTmpWidth * 2f + 25f), fTmpWidth * 2f, fTmpWidth * 2f);
				rectCylinder = new RectangleF(pointCenter.X - fTmpWidth / 2f, rectBackgroundImg.Top + (drawTics ? 25 : 10), fTmpWidth, rectBulb.Top - rectBackgroundImg.Top - (drawTics ? 20 : 5));
				if (!Enabled)
				{
					colorFore = SystemColors.ControlDark;
					colorScale = SystemColors.GrayText;
					colorScaleText = SystemColors.GrayText;
					colorOutline = SystemColors.ControlDark;
				}
				else
				{
					colorFore = ForeColor;
					colorScale = Color.FromArgb(0, 0, 0);
					colorScaleText = Color.FromArgb(0, 0, 0);
					colorOutline = Color.FromArgb(64, 0, 0);
				}
				forePen = new Pen(colorFore);
				scalePen = new Pen(colorScale);
				outlinePen = new Pen(colorOutline);
				blackBrush = new SolidBrush(colorScaleText);
				fillBrush = new SolidBrush(colorBack);
				bulbBrush = new LinearGradientBrush(rectBulb, OffsetColor(colorFore, 55), OffsetColor(colorFore, -55), LinearGradientMode.Horizontal);
				strfmtText.Alignment = StringAlignment.Center;
				strfmtText.LineAlignment = StringAlignment.Center;
				DrawBulb(graphics, rectBackgroundImg, Enabled);
				DrawCylinder(graphics, rectBackgroundImg, Enabled);
				graphics.DrawEllipse(rect: rectCylinder with { Y = rectCylinder.Y - 5f, Height = 5f }, pen: outlinePen);
				DrawTicks(graphics, rectBackgroundImg, Enabled);
				requiresRedraw = false;
			}
			e.Graphics.DrawImage(backgroundImg, rectBackgroundImg);
		}
	}
}
