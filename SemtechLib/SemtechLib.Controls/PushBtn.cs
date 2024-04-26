using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemtechLib.Controls
{
	public sealed class PushBtn : Control
	{
		private ContentAlignment controlAlign = ContentAlignment.MiddleCenter;

		private Size itemSize = default(Size);

		private int recessDepth = 1;

		private int bevelHeight = 1;

		private int bevelDepth;

		private bool dome;

		private Color buttonColor = Color.Aqua;

		private Color backgroudColor = Color.GhostWhite;

		private Color borderColor = Color.Black;

		private LinearGradientBrush edgeBrush;

		private Blend edgeBlend;

		private Color edgeColor1;

		private Color edgeColor2;

		private int edgeWidth = 1;

		private int buttonPressOffset;

		private float lightAngle = 50f;

		private Color cColor = Color.White;

		private bool gotFocus;

		private GraphicsPath bpath;

		private GraphicsPath gpath;

		[Description("Indicates how the LED should be aligned")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		[Category("Appearance")]
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
					result.X = (int)(Size.Width / 2.0 - itemSize.Width / 2.0);
					result.Y = 0;
					return result;
				case ContentAlignment.TopRight:
					result.X = Size.Width - itemSize.Width;
					result.Y = 0;
					return result;
				case ContentAlignment.MiddleLeft:
					result.X = 0;
					result.Y = (int)(Size.Height / 2.0 - itemSize.Height / 2.0);
					return result;
				case ContentAlignment.MiddleCenter:
					result.X = (int)(Size.Width / 2.0 - itemSize.Width / 2.0);
					result.Y = (int)(Size.Height / 2.0 - itemSize.Height / 2.0);
					return result;
				case ContentAlignment.MiddleRight:
					result.X = Size.Width - itemSize.Width;
					result.Y = (int)(Size.Height / 2.0 - itemSize.Height / 2.0);
					return result;
				case ContentAlignment.BottomLeft:
					result.X = 0;
					result.Y = Size.Height - itemSize.Height;
					return result;
				case ContentAlignment.BottomCenter:
					result.X = (int)(Width / 2.0 - itemSize.Width / 2.0);
					result.Y = Size.Height - itemSize.Height;
					return result;
				case ContentAlignment.BottomRight:
					result.X = Size.Width - itemSize.Width;
					result.Y = Size.Height - itemSize.Height;
					return result;
				default:
					result.X = 0;
					result.Y = 0;
					return result;
				}
			}
		}

		private int RecessDepth
		{
			get => recessDepth;
			set
			{
				if (value < 0)
				{
					recessDepth = 0;
				}
				else if (value > 15)
				{
					recessDepth = 15;
				}
				else
				{
					recessDepth = value;
				}
				Invalidate();
			}
		}

		private int BevelHeight
		{
			get => bevelHeight;
			set
			{
				if (value < 0)
				{
					bevelHeight = 0;
				}
				else
				{
					bevelHeight = value;
				}
				Invalidate();
			}
		}

		private int BevelDepth
		{
			get => bevelDepth;
			set
			{
				if (value < 0)
				{
					bevelDepth = 0;
				}
				else
				{
					bevelDepth = value;
				}
				Invalidate();
			}
		}

		private bool Dome
		{
			get => dome;
			set
			{
				dome = value;
				Invalidate();
			}
		}

		public new event PaintEventHandler Paint;

		public PushBtn()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.DoubleBuffer, value: true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
			SetStyle(ControlStyles.UserPaint, value: true);
			SetStyle(ControlStyles.ResizeRedraw, value: true);
			BackColor = Color.Transparent;
			Size = new Size(23, 23);
			MouseDown += mouseDown;
			MouseUp += mouseUp;
			Enter += weGotFocus;
			Leave += weLostFocus;
			KeyDown += keyDown;
			KeyUp += keyUp;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Paint != null)
			{
				Paint(this, e);
				return;
			}
			base.OnPaint(e);
			if (!Enabled)
			{
				buttonColor = ControlPaint.Light(SystemColors.InactiveCaption);
				backgroudColor = SystemColors.InactiveCaption;
				borderColor = SystemColors.InactiveBorder;
			}
			else
			{
				buttonColor = Color.Aqua;
				backgroudColor = Color.GhostWhite;
				borderColor = Color.Black;
			}
			edgeColor1 = ControlPaint.Light(buttonColor);
			edgeColor2 = ControlPaint.Dark(buttonColor);
			var g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			itemSize.Width = Size.Width * 80 / 100;
			itemSize.Height = Size.Height * 80 / 100;
			var num = Size.Width * 10 / 100;
			var rect = new Rectangle(PosFromAlignment.X + 2, PosFromAlignment.Y + 2, itemSize.Width - 4, itemSize.Height - 4);
			var rect2 = new Rectangle(PosFromAlignment.X - num, PosFromAlignment.Y - num, itemSize.Width + num * 2 - 1, itemSize.Height + num * 2 - 1);
			edgeWidth = GetEdgeWidth(rect);
			FillBackground(g, rect2);
			g.DrawRectangle(new Pen(new SolidBrush(borderColor)), rect2);
			if (RecessDepth > 0)
			{
				DrawRecess(ref g, ref rect);
			}
			DrawEdges(g, ref rect);
			ShrinkShape(ref g, ref rect, edgeWidth);
			DrawButton(g, rect);
		}

		private void FillBackground(Graphics g, Rectangle rect)
		{
			var rect2 = rect;
			rect2.Inflate(1, 1);
            var solidBrush = new SolidBrush(Color.FromKnownColor(KnownColor.Transparent))
            {
                Color = backgroudColor
            };
            g.FillRectangle(solidBrush, rect2);
			solidBrush.Dispose();
		}

		private void DrawRecess(ref Graphics g, ref Rectangle recessRect)
		{
			var linearGradientBrush = new LinearGradientBrush(recessRect, ControlPaint.Dark(backgroudColor), ControlPaint.LightLight(backgroudColor), GetLightAngle(50f));
            var blend = new Blend
            {
                Positions = new float[6] { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f },
                Factors = new float[6] { 0.2f, 0.2f, 0.4f, 0.4f, 1f, 1f }
            };
            linearGradientBrush.Blend = blend;
			var rect = recessRect;
			ShrinkShape(ref g, ref rect, 1);
			FillShape(g, linearGradientBrush, rect);
			ShrinkShape(ref g, ref recessRect, recessDepth);
		}

		private void DrawEdges(Graphics g, ref Rectangle edgeRect)
		{
			ShrinkShape(ref g, ref edgeRect, 1);
			var rect = edgeRect;
			rect.Inflate(1, 1);
			edgeBrush = new LinearGradientBrush(rect, edgeColor1, edgeColor2, GetLightAngle(lightAngle));
            edgeBlend = new Blend
            {
                Positions = new float[6] { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f },
                Factors = new float[6] { 0f, 0f, 0.2f, 0.4f, 1f, 1f }
            };
            edgeBrush.Blend = edgeBlend;
			FillShape(g, edgeBrush, edgeRect);
		}

		private void DrawButton(Graphics g, Rectangle buttonRect)
		{
			BuildGraphicsPath(buttonRect);
            var pathGradientBrush = new PathGradientBrush(bpath)
            {
                SurroundColors = new Color[1] { buttonColor }
            };
            buttonRect.Offset(buttonPressOffset, buttonPressOffset);
			if (bevelHeight > 0)
			{
				var rectangle = buttonRect;
				rectangle.Inflate(1, 1);
				pathGradientBrush.CenterPoint = new PointF(buttonRect.X + buttonRect.Width / 8 + buttonPressOffset, buttonRect.Y + buttonRect.Height / 8 + buttonPressOffset);
				pathGradientBrush.CenterColor = cColor;
				FillShape(g, pathGradientBrush, buttonRect);
				ShrinkShape(ref g, ref buttonRect, bevelHeight);
			}
			if (bevelDepth > 0)
			{
				DrawInnerBevel(g, buttonRect, bevelDepth, buttonColor);
				ShrinkShape(ref g, ref buttonRect, bevelDepth);
			}
			pathGradientBrush.CenterColor = buttonColor;
			if (dome)
			{
				pathGradientBrush.CenterColor = cColor;
				pathGradientBrush.CenterPoint = new PointF(buttonRect.X + buttonRect.Width / 8 + buttonPressOffset, buttonRect.Y + buttonRect.Height / 8 + buttonPressOffset);
			}
			FillShape(g, pathGradientBrush, buttonRect);
			if (gotFocus)
			{
				DrawFocus(g, buttonRect);
			}
		}

		private void BuildGraphicsPath(Rectangle buttonRect)
		{
			bpath = new GraphicsPath();
			AddShape(rect: new Rectangle(buttonRect.X - 1, buttonRect.Y - 1, buttonRect.Width + 2, buttonRect.Height + 2), gpath: bpath);
			AddShape(bpath, buttonRect);
		}

		private void SetClickableRegion()
		{
			gpath = new GraphicsPath();
			gpath.AddEllipse(ClientRectangle);
			Region = new Region(gpath);
		}

		private void FillShape(Graphics g, object brush, Rectangle rect)
		{
			if (brush.GetType().ToString() == "System.Drawing.Drawing2D.LinearGradientBrush")
			{
				g.FillEllipse((LinearGradientBrush)brush, rect);
			}
			else if (brush.GetType().ToString() == "System.Drawing.Drawing2D.PathGradientBrush")
			{
				g.FillEllipse((PathGradientBrush)brush, rect);
			}
		}

		private void AddShape(GraphicsPath gpath, Rectangle rect)
		{
			gpath.AddEllipse(rect);
		}

		private void DrawShape(Graphics g, Pen pen, Rectangle rect)
		{
			g.DrawEllipse(pen, rect);
		}

		private void ShrinkShape(ref Graphics g, ref Rectangle rect, int amount)
		{
			rect.Inflate(-amount, -amount);
		}

		private void DrawFocus(Graphics g, Rectangle rect)
		{
			rect.Inflate(-2, -2);
            var pen = new Pen(Color.Black)
            {
                DashStyle = DashStyle.Dot
            };
            DrawShape(g, pen, rect);
		}

		private void DrawInnerBevel(Graphics g, Rectangle rect, int depth, Color buttonColor)
		{
			var color = ControlPaint.LightLight(buttonColor);
			var color2 = ControlPaint.Dark(buttonColor);
            var blend = new Blend
            {
                Positions = new float[6] { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f },
                Factors = new float[6] { 0.2f, 0.4f, 0.6f, 0.6f, 1f, 1f }
            };
            var rect2 = rect;
			rect2.Inflate(1, 1);
            var linearGradientBrush = new LinearGradientBrush(rect2, color2, color, GetLightAngle(50f))
            {
                Blend = blend
            };
            FillShape(g, linearGradientBrush, rect);
		}

		private float GetLightAngle(float angle)
		{
			var num = 1f - Width / (float)Height;
			return angle - 15f * num;
		}

		private int GetEdgeWidth(Rectangle rect)
		{
			if ((rect.Width < 50) | (rect.Height < 50))
			{
				return 1;
			}
			return 2;
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
			lightAngle = 230f;
			buttonPressOffset = 1;
			Invalidate();
		}

		private void buttonUp()
		{
			lightAngle = 50f;
			buttonPressOffset = 0;
			Invalidate();
		}

		private void keyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode.ToString() == "Space")
			{
				buttonDown();
			}
		}

		private void keyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode.ToString() == "Space")
			{
				buttonUp();
			}
		}

		private void weGotFocus(object sender, EventArgs e)
		{
			gotFocus = true;
			Invalidate();
		}

		private void weLostFocus(object sender, EventArgs e)
		{
			gotFocus = false;
			buttonUp();
			Invalidate();
		}
	}
}
