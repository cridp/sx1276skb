using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Fusionbird.FusionToolkit.FusionTrackBar
{
	public class FusionTrackBar : TrackBar
	{
		private Rectangle ChannelBounds;
        private Rectangle ThumbBounds;

		private int ThumbState;

        [DefaultValue(typeof(TrackBarOwnerDrawParts), "None")]
        [Description("Gets/sets the trackbar parts that will be OwnerDrawn.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Editor(typeof(TrackDrawModeEditor), typeof(UITypeEditor))]
        public TrackBarOwnerDrawParts OwnerDrawParts { get; set; }

        public event EventHandler<TrackBarDrawItemEventArgs> DrawChannel;

		public event EventHandler<TrackBarDrawItemEventArgs> DrawThumb;

		public event EventHandler<TrackBarDrawItemEventArgs> DrawTicks;

		public FusionTrackBar()
		{
			SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 20)
			{
				m.Result = IntPtr.Zero;
				return;
			}
			base.WndProc(ref m);
			if (m.Msg != 8270)
			{
				return;
			}
			var nMHDR = (NativeMethods.NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.NMHDR));
			if (nMHDR.code != -12)
			{
				return;
			}
			Marshal.StructureToPtr((object)nMHDR, m.LParam, fDeleteOld: false);
			var nMCUSTOMDRAW = (NativeMethods.NMCUSTOMDRAW)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.NMCUSTOMDRAW));
			if (nMCUSTOMDRAW.dwDrawStage == NativeMethods.CustomDrawDrawStage.CDDS_PREPAINT)
			{
				var graphics = Graphics.FromHdc(nMCUSTOMDRAW.hdc);
				var paintEventArgs = new PaintEventArgs(graphics, Bounds);
				paintEventArgs.Graphics.TranslateTransform(-Left, -Top);
				InvokePaintBackground(Parent, paintEventArgs);
				InvokePaint(Parent, paintEventArgs);
				var solidBrush = new SolidBrush(BackColor);
				paintEventArgs.Graphics.FillRectangle(solidBrush, Bounds);
				solidBrush.Dispose();
				paintEventArgs.Graphics.ResetTransform();
				paintEventArgs.Dispose();
				graphics.Dispose();
				var result = new IntPtr(48);
				m.Result = result;
			}
			else if (nMCUSTOMDRAW.dwDrawStage == NativeMethods.CustomDrawDrawStage.CDDS_POSTPAINT)
			{
				OnDrawTicks(nMCUSTOMDRAW.hdc);
				OnDrawChannel(nMCUSTOMDRAW.hdc);
				OnDrawThumb(nMCUSTOMDRAW.hdc);
			}
			else
			{
				if (nMCUSTOMDRAW.dwDrawStage != NativeMethods.CustomDrawDrawStage.CDDS_ITEMPREPAINT)
				{
					return;
				}
				if (nMCUSTOMDRAW.dwItemSpec.ToInt32() == 2)
				{
					ThumbBounds = nMCUSTOMDRAW.rc.ToRectangle();
					if (Enabled)
					{
						if (nMCUSTOMDRAW.uItemState == NativeMethods.CustomDrawItemState.CDIS_SELECTED)
						{
							ThumbState = 3;
						}
						else
						{
							ThumbState = 1;
						}
					}
					else
					{
						ThumbState = 5;
					}
					OnDrawThumb(nMCUSTOMDRAW.hdc);
				}
				else if (nMCUSTOMDRAW.dwItemSpec.ToInt32() == 3)
				{
					ChannelBounds = nMCUSTOMDRAW.rc.ToRectangle();
					OnDrawChannel(nMCUSTOMDRAW.hdc);
				}
				else if (nMCUSTOMDRAW.dwItemSpec.ToInt32() == 1)
				{
					OnDrawTicks(nMCUSTOMDRAW.hdc);
				}
				var result = new IntPtr(4);
				m.Result = result;
			}
		}

		private void DrawHorizontalTicks(Graphics g, Color color)
		{
			var num = Maximum / TickFrequency - 1;
			var pen = new Pen(color);
			var rectangleF = new RectangleF(ChannelBounds.Left + ThumbBounds.Width / 2, ThumbBounds.Top - 5, 0f, 3f);
			var rectangleF2 = new RectangleF(ChannelBounds.Right - ThumbBounds.Width / 2 - 1, ThumbBounds.Top - 5, 0f, 3f);
			var num2 = (rectangleF2.Right - rectangleF.Left) / (num + 1);
			if (TickStyle != TickStyle.BottomRight)
			{
				g.DrawLine(pen, rectangleF.Left, rectangleF.Top, rectangleF.Right, rectangleF.Bottom);
				g.DrawLine(pen, rectangleF2.Left, rectangleF2.Top, rectangleF2.Right, rectangleF2.Bottom);
				var rectangleF3 = rectangleF;
				rectangleF3.Height -= 1f;
				rectangleF3.Offset(num2, 1f);
				var num3 = num - 1;
				for (var i = 0; i <= num3; i++)
				{
					g.DrawLine(pen, rectangleF3.Left, rectangleF3.Top, rectangleF3.Left, rectangleF3.Bottom);
					rectangleF3.Offset(num2, 0f);
				}
			}
			rectangleF.Offset(0f, ThumbBounds.Height + 6);
			rectangleF2.Offset(0f, ThumbBounds.Height + 6);
			if (TickStyle != TickStyle.TopLeft)
			{
				g.DrawLine(pen, rectangleF.Left, rectangleF.Top, rectangleF.Left, rectangleF.Bottom);
				g.DrawLine(pen, rectangleF2.Left, rectangleF2.Top, rectangleF2.Left, rectangleF2.Bottom);
				var rectangleF3 = rectangleF;
				rectangleF3.Height -= 1f;
				rectangleF3.Offset(num2, 0f);
				var num4 = num - 1;
				for (var j = 0; j <= num4; j++)
				{
					g.DrawLine(pen, rectangleF3.Left, rectangleF3.Top, rectangleF3.Left, rectangleF3.Bottom);
					rectangleF3.Offset(num2, 0f);
				}
			}
			pen.Dispose();
		}

		private void DrawVerticalTicks(Graphics g, Color color)
		{
			var num = Maximum / TickFrequency - 1;
			var pen = new Pen(color);
			var rectangleF = new RectangleF(ThumbBounds.Left - 5, ChannelBounds.Bottom - ThumbBounds.Height / 2 - 1, 3f, 0f);
			var rectangleF2 = new RectangleF(ThumbBounds.Left - 5, ChannelBounds.Top + ThumbBounds.Height / 2, 3f, 0f);
			var num2 = (rectangleF2.Bottom - rectangleF.Top) / (num + 1);
			if (TickStyle != TickStyle.BottomRight)
			{
				g.DrawLine(pen, rectangleF.Left, rectangleF.Top, rectangleF.Right, rectangleF.Bottom);
				g.DrawLine(pen, rectangleF2.Left, rectangleF2.Top, rectangleF2.Right, rectangleF2.Bottom);
				var rectangleF3 = rectangleF;
				rectangleF3.Width -= 1f;
				rectangleF3.Offset(1f, num2);
				var num3 = num - 1;
				for (var i = 0; i <= num3; i++)
				{
					g.DrawLine(pen, rectangleF3.Left, rectangleF3.Top, rectangleF3.Right, rectangleF3.Bottom);
					rectangleF3.Offset(0f, num2);
				}
			}
			rectangleF.Offset(ThumbBounds.Width + 6, 0f);
			rectangleF2.Offset(ThumbBounds.Width + 6, 0f);
			if (TickStyle != TickStyle.TopLeft)
			{
				g.DrawLine(pen, rectangleF.Left, rectangleF.Top, rectangleF.Right, rectangleF.Bottom);
				g.DrawLine(pen, rectangleF2.Left, rectangleF2.Top, rectangleF2.Right, rectangleF2.Bottom);
				var rectangleF3 = rectangleF;
				rectangleF3.Width -= 1f;
				rectangleF3.Offset(0f, num2);
				var num4 = num - 1;
				for (var j = 0; j <= num4; j++)
				{
					g.DrawLine(pen, rectangleF3.Left, rectangleF3.Top, rectangleF3.Right, rectangleF3.Bottom);
					rectangleF3.Offset(0f, num2);
				}
			}
			pen.Dispose();
		}

		private void DrawPointerDown(Graphics g)
		{
			var array = new Point[6]
			{
				new(ThumbBounds.Left + ThumbBounds.Width / 2, ThumbBounds.Bottom - 1),
				new(ThumbBounds.Left, ThumbBounds.Bottom - ThumbBounds.Width / 2 - 1),
				ThumbBounds.Location,
				new(ThumbBounds.Right - 1, ThumbBounds.Top),
				new(ThumbBounds.Right - 1, ThumbBounds.Bottom - ThumbBounds.Width / 2 - 1),
				new(ThumbBounds.Left + ThumbBounds.Width / 2, ThumbBounds.Bottom - 1)
			};
			var graphicsPath = new GraphicsPath();
			graphicsPath.AddLines(array);
			var region2 = (g.Clip = new Region(graphicsPath));
			if (ThumbState == 3 || !Enabled)
			{
				ControlPaint.DrawButton(g, ThumbBounds, ButtonState.All);
			}
			else
			{
				g.Clear(SystemColors.Control);
			}
			g.ResetClip();
			region2.Dispose();
			graphicsPath.Dispose();
			var points = new Point[4]
			{
				array[0],
				array[1],
				array[2],
				array[3]
			};
			g.DrawLines(SystemPens.ControlLightLight, points);
			points = new Point[3]
			{
				array[3],
				array[4],
				array[5]
			};
			g.DrawLines(SystemPens.ControlDarkDark, points);
			array[0].Offset(0, -1);
			array[1].Offset(1, 0);
			array[2].Offset(1, 1);
			array[3].Offset(-1, 1);
			array[4].Offset(-1, 0);
			ref var reference = ref array[5];
			reference = array[0];
			points = new Point[4]
			{
				array[0],
				array[1],
				array[2],
				array[3]
			};
			g.DrawLines(SystemPens.ControlLight, points);
			points = new Point[3]
			{
				array[3],
				array[4],
				array[5]
			};
			g.DrawLines(SystemPens.ControlDark, points);
		}

		private void DrawPointerLeft(Graphics g)
		{
			var array = new Point[6]
			{
				new(ThumbBounds.Left, ThumbBounds.Top + ThumbBounds.Height / 2),
				new(ThumbBounds.Left + ThumbBounds.Height / 2, ThumbBounds.Top),
				new(ThumbBounds.Right - 1, ThumbBounds.Top),
				new(ThumbBounds.Right - 1, ThumbBounds.Bottom - 1),
				new(ThumbBounds.Left + ThumbBounds.Height / 2, ThumbBounds.Bottom - 1),
				new(ThumbBounds.Left, ThumbBounds.Top + ThumbBounds.Height / 2)
			};
			var graphicsPath = new GraphicsPath();
			graphicsPath.AddLines(array);
			var region2 = (g.Clip = new Region(graphicsPath));
			if (ThumbState == 3 || !Enabled)
			{
				ControlPaint.DrawButton(g, ThumbBounds, ButtonState.All);
			}
			else
			{
				g.Clear(SystemColors.Control);
			}
			g.ResetClip();
			region2.Dispose();
			graphicsPath.Dispose();
			var points = new Point[3]
			{
				array[0],
				array[1],
				array[2]
			};
			g.DrawLines(SystemPens.ControlLightLight, points);
			points = new Point[4]
			{
				array[2],
				array[3],
				array[4],
				array[5]
			};
			g.DrawLines(SystemPens.ControlDarkDark, points);
			array[0].Offset(1, 0);
			array[1].Offset(0, 1);
			array[2].Offset(-1, 1);
			array[3].Offset(-1, -1);
			array[4].Offset(0, -1);
			ref var reference = ref array[5];
			reference = array[0];
			points = new Point[3]
			{
				array[0],
				array[1],
				array[2]
			};
			g.DrawLines(SystemPens.ControlLight, points);
			points = new Point[4]
			{
				array[2],
				array[3],
				array[4],
				array[5]
			};
			g.DrawLines(SystemPens.ControlDark, points);
		}

		private void DrawPointerRight(Graphics g)
		{
			var array = new Point[6]
			{
				new(ThumbBounds.Left, ThumbBounds.Bottom - 1),
				new(ThumbBounds.Left, ThumbBounds.Top),
				new(ThumbBounds.Right - ThumbBounds.Height / 2 - 1, ThumbBounds.Top),
				new(ThumbBounds.Right - 1, ThumbBounds.Top + ThumbBounds.Height / 2),
				new(ThumbBounds.Right - ThumbBounds.Height / 2 - 1, ThumbBounds.Bottom - 1),
				new(ThumbBounds.Left, ThumbBounds.Bottom - 1)
			};
			var graphicsPath = new GraphicsPath();
			graphicsPath.AddLines(array);
			var region2 = (g.Clip = new Region(graphicsPath));
			if (ThumbState == 3 || !Enabled)
			{
				ControlPaint.DrawButton(g, ThumbBounds, ButtonState.All);
			}
			else
			{
				g.Clear(SystemColors.Control);
			}
			g.ResetClip();
			region2.Dispose();
			graphicsPath.Dispose();
			var points = new Point[4]
			{
				array[0],
				array[1],
				array[2],
				array[3]
			};
			g.DrawLines(SystemPens.ControlLightLight, points);
			points = new Point[3]
			{
				array[3],
				array[4],
				array[5]
			};
			g.DrawLines(SystemPens.ControlDarkDark, points);
			array[0].Offset(1, -1);
			array[1].Offset(1, 1);
			array[2].Offset(0, 1);
			array[3].Offset(-1, 0);
			array[4].Offset(0, -1);
			ref var reference = ref array[5];
			reference = array[0];
			points = new Point[4]
			{
				array[0],
				array[1],
				array[2],
				array[3]
			};
			g.DrawLines(SystemPens.ControlLight, points);
			points = new Point[3]
			{
				array[3],
				array[4],
				array[5]
			};
			g.DrawLines(SystemPens.ControlDark, points);
		}

		private void DrawPointerUp(Graphics g)
		{
			var array = new Point[6]
			{
				new(ThumbBounds.Left, ThumbBounds.Bottom - 1),
				new(ThumbBounds.Left, ThumbBounds.Top + ThumbBounds.Width / 2),
				new(ThumbBounds.Left + ThumbBounds.Width / 2, ThumbBounds.Top),
				new(ThumbBounds.Right - 1, ThumbBounds.Top + ThumbBounds.Width / 2),
				new(ThumbBounds.Right - 1, ThumbBounds.Bottom - 1),
				new(ThumbBounds.Left, ThumbBounds.Bottom - 1)
			};
			var graphicsPath = new GraphicsPath();
			graphicsPath.AddLines(array);
			var region2 = (g.Clip = new Region(graphicsPath));
			if (ThumbState == 3 || !Enabled)
			{
				ControlPaint.DrawButton(g, ThumbBounds, ButtonState.All);
			}
			else
			{
				g.Clear(SystemColors.Control);
			}
			g.ResetClip();
			region2.Dispose();
			graphicsPath.Dispose();
			var points = new Point[3]
			{
				array[0],
				array[1],
				array[2]
			};
			g.DrawLines(SystemPens.ControlLightLight, points);
			points = new Point[4]
			{
				array[2],
				array[3],
				array[4],
				array[5]
			};
			g.DrawLines(SystemPens.ControlDarkDark, points);
			array[0].Offset(1, -1);
			array[1].Offset(1, 0);
			array[2].Offset(0, 1);
			array[3].Offset(-1, 0);
			array[4].Offset(-1, -1);
			ref var reference = ref array[5];
			reference = array[0];
			points = new Point[3]
			{
				array[0],
				array[1],
				array[2]
			};
			g.DrawLines(SystemPens.ControlLight, points);
			points = new Point[4]
			{
				array[2],
				array[3],
				array[4],
				array[5]
			};
			g.DrawLines(SystemPens.ControlDark, points);
		}

		protected virtual void OnDrawTicks(IntPtr hdc)
		{
			var graphics = Graphics.FromHdc(hdc);
			if ((OwnerDrawParts & TrackBarOwnerDrawParts.Ticks) == TrackBarOwnerDrawParts.Ticks && !DesignMode)
			{
				var e = new TrackBarDrawItemEventArgs(graphics, ClientRectangle, (TrackBarItemState)ThumbState);
                DrawTicks?.Invoke(this, e);
            }
			else
			{
				if (TickStyle == TickStyle.None || ThumbBounds.Equals(Rectangle.Empty))
				{
					return;
				}
				var color = Color.Black;
				if (VisualStyleRenderer.IsSupported)
				{
					var visualStyleRenderer = new VisualStyleRenderer("TRACKBAR", 9, ThumbState);
					color = visualStyleRenderer.GetColor(ColorProperty.TextColor);
				}
				if (Orientation == Orientation.Horizontal)
				{
					DrawHorizontalTicks(graphics, color);
				}
				else
				{
					DrawVerticalTicks(graphics, color);
				}
			}
			graphics.Dispose();
		}

		protected virtual void OnDrawThumb(IntPtr hdc)
		{
			var graphics = Graphics.FromHdc(hdc);
			graphics.Clip = new Region(ThumbBounds);
			if ((OwnerDrawParts & TrackBarOwnerDrawParts.Thumb) == TrackBarOwnerDrawParts.Thumb && !DesignMode)
			{
				var e = new TrackBarDrawItemEventArgs(graphics, ThumbBounds, (TrackBarItemState)ThumbState);
                DrawThumb?.Invoke(this, e);
            }
			else
			{
				var trackBarParts = NativeMethods.TrackBarParts.TKP_THUMB;
				if (ThumbBounds.Equals(Rectangle.Empty))
				{
					return;
				}
				switch (TickStyle)
				{
				case TickStyle.None:
				case TickStyle.BottomRight:
					trackBarParts = ((Orientation != 0) ? NativeMethods.TrackBarParts.TKP_THUMBRIGHT : NativeMethods.TrackBarParts.TKP_THUMBBOTTOM);
					break;
				case TickStyle.TopLeft:
					trackBarParts = ((Orientation != 0) ? NativeMethods.TrackBarParts.TKP_THUMBLEFT : NativeMethods.TrackBarParts.TKP_THUMBTOP);
					break;
				case TickStyle.Both:
					trackBarParts = ((Orientation != 0) ? NativeMethods.TrackBarParts.TKP_THUMBVERT : NativeMethods.TrackBarParts.TKP_THUMB);
					break;
				}
				if (VisualStyleRenderer.IsSupported)
				{
					var visualStyleRenderer = new VisualStyleRenderer("TRACKBAR", (int)trackBarParts, ThumbState);
					visualStyleRenderer.DrawBackground(graphics, ThumbBounds);
					graphics.ResetClip();
					graphics.Dispose();
					return;
				}
				switch (trackBarParts)
				{
				case NativeMethods.TrackBarParts.TKP_THUMBBOTTOM:
					DrawPointerDown(graphics);
					break;
				case NativeMethods.TrackBarParts.TKP_THUMBTOP:
					DrawPointerUp(graphics);
					break;
				case NativeMethods.TrackBarParts.TKP_THUMBLEFT:
					DrawPointerLeft(graphics);
					break;
				case NativeMethods.TrackBarParts.TKP_THUMBRIGHT:
					DrawPointerRight(graphics);
					break;
				default:
					if (ThumbState == 3 || !Enabled)
					{
						ControlPaint.DrawButton(graphics, ThumbBounds, ButtonState.All);
					}
					else
					{
						graphics.FillRectangle(SystemBrushes.Control, ThumbBounds);
					}
					ControlPaint.DrawBorder3D(graphics, ThumbBounds, Border3DStyle.Raised);
					break;
				}
			}
			graphics.ResetClip();
			graphics.Dispose();
		}

		protected virtual void OnDrawChannel(IntPtr hdc)
		{
			var graphics = Graphics.FromHdc(hdc);
			if ((OwnerDrawParts & TrackBarOwnerDrawParts.Channel) == TrackBarOwnerDrawParts.Channel && !DesignMode)
			{
				var e = new TrackBarDrawItemEventArgs(graphics, ChannelBounds, (TrackBarItemState)ThumbState);
                DrawChannel?.Invoke(this, e);
            }
			else
			{
				if (ChannelBounds.Equals(Rectangle.Empty))
				{
					return;
				}
				if (VisualStyleRenderer.IsSupported)
				{
					var visualStyleRenderer = new VisualStyleRenderer("TRACKBAR", 1, 1);
					visualStyleRenderer.DrawBackground(graphics, ChannelBounds);
					graphics.ResetClip();
					graphics.Dispose();
					return;
				}
				ControlPaint.DrawBorder3D(graphics, ChannelBounds, Border3DStyle.Sunken);
			}
			graphics.Dispose();
		}
	}
}
