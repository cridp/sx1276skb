using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SemtechLib.Properties;

namespace SemtechLib.Controls.Graph
{
	public sealed class GraphCtrl : UserControl
	{
		public enum eZoomOption
		{
			None,
			ZoomIn,
			ZoomOut,
			AutoScale,
			Hand,
			FullScale
		}

		public enum eGraphType
		{
			Dot,
			Line,
			Bar
		}

		private sealed class GraphGridTypeConverter : TypeConverter
		{
			public override bool GetPropertiesSupported(ITypeDescriptorContext context)
			{
				return true;
			}

			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
			{
				return TypeDescriptor.GetProperties(typeof(GraphGrid));
			}
		}

		[Description("Graph Grid.")]
		[TypeConverter(typeof(GraphGridTypeConverter))]
		[Category("Graph")]
		public sealed class GraphGrid
		{
			public delegate void PropertyChangedEventHandler();

			private int main;

			private int major;

			private int minor;

			private bool showMajor;

			private bool showMinor;

			[Description("Main axe value from which the grid will depends.")]
			public int Main
			{
				get => main;
				set
				{
					main = value;
					PropertyChanged();
				}
			}

			[Description("Major Grid Unit.")]
			public int Major
			{
				get => major;
				set
				{
					major = value;
					PropertyChanged();
				}
			}

			[Description("Minor Grid Unit.")]
			public int Minor
			{
				get => minor;
				set
				{
					minor = value;
					PropertyChanged();
				}
			}

			[Description("Show Major Grid.")]
			public bool ShowMajor
			{
				get => showMajor;
				set
				{
					showMajor = value;
					PropertyChanged();
				}
			}

			[Description("Show Minor Grid.")]
			public bool ShowMinor
			{
				get => showMinor;
				set
				{
					showMinor = value;
					PropertyChanged();
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;

			public override string ToString()
			{
				return main + "; " + major + "; " + minor + "; " + showMajor + "; " + showMinor;
			}
		}

		public sealed class GraphScaleTypeConverter : TypeConverter
		{
			public override bool GetPropertiesSupported(ITypeDescriptorContext context)
			{
				return true;
			}

			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
			{
				return TypeDescriptor.GetProperties(typeof(GraphScale));
			}
		}

		[Category("Graph")]
		[TypeConverter(typeof(GraphScaleTypeConverter))]
		[Description("Graph Scale.")]
		public sealed class GraphScale
		{
			public delegate void PropertyChangedEventHandler();

			private int min;

			private int max;

			private bool show;

			[Description("Minimum Scale value.")]
			public int Min
			{
				get => min;
				set
				{
					min = value;
					PropertyChanged();
				}
			}

			[Description("Maximum Scale value.")]
			public int Max
			{
				get => max;
				set
				{
					max = value;
					PropertyChanged();
				}
			}

			[Description("If true, the scale will be show on the Graph.")]
			public bool Show
			{
				get => show;
				set
				{
					show = value;
					PropertyChanged();
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;

			public override string ToString()
			{
				return max + "; " + min + "; " + show;
			}
		}

		private sealed class GraphFrame
		{
            private int right;
            private int bottom;

            public int Left { get; set; }

            public int Right
			{
				get => right;
				set => right = value;
			}

            public int Top { get; set; }

            public int Bottom
			{
				get => bottom;
				set => bottom = value;
			}

			public int Width => Right - Left;

			public int Height => Top - Bottom;

			public static GraphFrame operator *(GraphFrame gf, float times)
			{
                var graphFrame = new GraphFrame
                {
                    Right = (int)(gf.Right - gf.Width / 2 + times * gf.Width / 2f),
                    Top = (int)(gf.Top - gf.Height / 2 + times * gf.Height / 2f),
                    Left = (int)(gf.Left + gf.Width * (1f - times) / 2f),
                    Bottom = (int)(gf.Bottom + gf.Height * (1f - times) / 2f)
                };
                return graphFrame;
			}
		}

		private sealed class Line
		{
			private Point pt1; // = default(Point);

			private Point pt2; // = default(Point);

			public Point Pt1
			{
				get => pt1;
				set => pt1 = value;
			}

			public Point Pt2
			{
				get => pt2;
				set => pt2 = value;
			}

			public Line(Point pt1, Point pt2)
			{
				this.pt1 = pt1;
				this.pt2 = pt2;
			}
		}

		public sealed class GraphData
		{
			private Color _color;

			private Color _colorGradient;

			private string _text;

			private List<int> _value = new();

			private int _maxWidth = 15;

			private eGraphType _graphType = eGraphType.Line;

			public Color Color
			{
				get => _color;
				set => _color = value;
			}

			public Color ColorGradient
			{
				get
				{
					return _colorGradient == Color.Empty ? _color : _colorGradient;
				}
				set => _colorGradient = value;
			}

			public string Text
			{
				get => _text;
				set => _text = value;
			}

			public List<int> Value
			{
				get => _value;
				set => _value = value;
			}

			public int MaxWidth
			{
				get => _maxWidth;
				set => _maxWidth = value;
			}

			public eGraphType GraphType
			{
				get => _graphType;
				set => _graphType = value;
			}

			public GraphData()
			{
			}

			public GraphData(List<int> value, Color color, eGraphType graphType)
			{
				_value = value;
				_color = color;
				_graphType = graphType;
			}

			public GraphData(List<int> value, Color color, Color colorGradient, eGraphType graphType)
			{
				_value = value;
				_color = color;
				_colorGradient = colorGradient;
				_graphType = graphType;
			}

			public GraphData(List<int> value, Color color, string text, eGraphType graphType)
			{
				_value = value;
				_color = color;
				_text = text;
				_graphType = graphType;
			}

			public GraphData(List<int> value, Color color, Color colorGradient, string text, eGraphType graphType)
			{
				_value = value;
				_color = color;
				_text = text;
				_colorGradient = colorGradient;
				_graphType = graphType;
			}
		}

		[Serializable]
		public sealed class GraphDataCollection : CollectionBase
		{
			public sealed class GraphDataEnumerator : IEnumerator
			{
				private IEnumerator baseEnumerator;

				private IEnumerable temp;

				public GraphData Current => (GraphData)baseEnumerator.Current;

				object IEnumerator.Current => baseEnumerator.Current;

				public GraphDataEnumerator(GraphDataCollection mappings)
				{
					temp = mappings;
					baseEnumerator = temp.GetEnumerator();
				}

				public bool MoveNext()
				{
					return baseEnumerator.MoveNext();
				}

				bool IEnumerator.MoveNext()
				{
					return baseEnumerator.MoveNext();
				}

				public void Reset()
				{
					baseEnumerator.Reset();
				}

				void IEnumerator.Reset()
				{
					baseEnumerator.Reset();
				}
			}

			public GraphData this[int index]
			{
				get => (GraphData)List[index];
				set => List[index] = value;
			}

			public GraphDataCollection()
			{
			}

			public GraphDataCollection(GraphDataCollection value)
			{
				AddRange(value);
			}

			public GraphDataCollection(GraphData[] value)
			{
				AddRange(value);
			}

			public int Add(GraphData value)
			{
				return List.Add(value);
			}

			public void AddRange(GraphData[] value)
			{
				for (var i = 0; i < value.Length; i++)
				{
					Add(value[i]);
				}
			}

			public void AddRange(GraphDataCollection value)
			{
				for (var i = 0; i < value.Count; i++)
				{
					Add(value[i]);
				}
			}

			public bool Contains(GraphData value)
			{
				return List.Contains(value);
			}

			public void CopyTo(GraphData[] array, int index)
			{
				List.CopyTo(array, index);
			}

			public int IndexOf(GraphData value)
			{
				return List.IndexOf(value);
			}

			public void Insert(int index, GraphData value)
			{
				List.Insert(index, value);
			}

			public new GraphDataEnumerator GetEnumerator()
			{
				return new GraphDataEnumerator(this);
			}

			public void Remove(GraphData value)
			{
				Capacity--;
				List.Remove(value);
			}
		}

		private IContainer components;

		private Timer tmrRefresh;

		private CheckBox checkBoxAutoScale;

		private CheckBox checkBoxZoomIn;

		private CheckBox checkBoxZoomOut;

		private CheckBox checkBoxHand;

		private Label labelScaleYmin;

		private Label labelScaleYmax;

		private TextBox textBoxScaleYmax;

		private TextBox textBoxScaleYmin;

		private Label labelSample;

		private ToolTip grphToolTip;

		private float zoomLeft;

		private float zoomRight;

		private float zoomTop;

		private float zoomBottom;

		private int rightOffset;

		private int leftOffset;

		private bool updateData;

		private eGraphType graphType;

		private Brush brushBackground;

		private Brush brushPoint;

		private Pen penFrame;

		private Pen penLine;

		private Color colorBackground;

		private Color colorFrame;

		private Color colorLine;

		private Font fontText = new(FontFamily.GenericSansSerif, 8f);

		private Brush brushText;

		private Color colorText;

		private GraphDataCollection dataCollection = new();

		private int history;

		private bool frameFit;

		private GraphFrame frameFull = new();

		private Rectangle workingArea = default(Rectangle);

		private eZoomOption zoomOption;

		private Point mousePos = default(Point);

		private GraphFrame frameZoom = new();

		private GraphScale scaleX = new();

		private GraphScale scaleY = new();

		private GraphGrid gridX = new();

		private GraphGrid gridY = new();

		private int previousClick = SystemInformation.DoubleClickTime + 1;

		[Description("Color of the Graph Text")]
		[Category("Graph")]
		public Color ColorText
		{
			get => colorText;
			set
			{
				colorText = value;
				brushText = new SolidBrush(colorText);
				labelScaleYmax.ForeColor = colorText;
				textBoxScaleYmax.ForeColor = colorText;
				labelScaleYmin.ForeColor = colorText;
				textBoxScaleYmin.ForeColor = colorText;
				labelSample.ForeColor = colorText;
				updateData = true;
			}
		}

		[Description("Color of the Graph Background")]
		[Category("Graph")]
		public Color ColorBackground
		{
			get => colorBackground;
			set
			{
				colorBackground = value;
				brushBackground = new SolidBrush(colorBackground);
				labelScaleYmax.BackColor = colorBackground;
				textBoxScaleYmax.BackColor = colorBackground;
				labelScaleYmin.BackColor = colorBackground;
				textBoxScaleYmin.BackColor = colorBackground;
				labelSample.BackColor = Color.Transparent;
				updateData = true;
			}
		}

		[Category("Graph")]
		[Description("Color of the Graph Frame")]
		public Color ColorFrame
		{
			get => colorFrame;
			set
			{
				colorFrame = value;
				penFrame = new Pen(colorFrame);
				updateData = true;
			}
		}

		[Description("Color of the Graph Line")]
		[Category("Graph")]
		public Color ColorLine
		{
			get => colorLine;
			set
			{
				colorLine = value;
				penLine = new Pen(colorLine);
				brushPoint = new SolidBrush(colorLine);
				updateData = true;
			}
		}

		[DefaultValue(eGraphType.Dot)]
		[Category("Graph")]
		[Description("Graph Type")]
		public eGraphType Type
		{
			get => graphType;
			set
			{
				graphType = value;
				updateData = true;
			}
		}

		[Description("How many point to keep in hystory")]
		[DefaultValue(100)]
		[Category("Graph")]
		public int History
		{
			get => history;
			set
			{
				history = value;
				foreach (var item in dataCollection)
				{
					if (item.Value.Count > history)
					{
						item.Value.RemoveRange(0, item.Value.Count - history);
					}
				}
			}
		}

		[Description("Let the Frame Fit in the Control Window")]
		[DefaultValue(false)]
		[Category("Graph")]
		public bool FrameFit
		{
			get => frameFit;
			set
			{
				frameFit = value;
				updateData = true;
			}
		}

		[Category("Graph")]
		[Description("Update Rate in milliseconds")]
		[DefaultValue(25)]
		public int UpdateRate
		{
			get => tmrRefresh.Interval;
			set => tmrRefresh.Interval = value;
		}

		[DefaultValue(false)]
		[Category("Graph")]
		[Description("Enable the Autoscale features.")]
		public bool AutoScale
		{
			get
			{
				return zoomOption == eZoomOption.AutoScale;
			}
			set
			{
				if (value)
				{
					zoomOption = eZoomOption.AutoScale;
				}
				else
				{
					zoomOption = eZoomOption.None;
				}
			}
		}

		[DefaultValue(false)]
		[Category("Graph")]
		[Description("Sets the graph to full scale.")]
		public bool FullScale
		{
			get
			{
				return zoomOption == eZoomOption.FullScale;
			}
			set
			{
				if (value)
				{
					zoomOption = eZoomOption.FullScale;
					zoomLeft = 0f;
					zoomRight = 0f;
					zoomTop = 0f;
					zoomBottom = 0f;
				}
				else
				{
					zoomOption = eZoomOption.None;
				}
			}
		}

		[Category("Graph")]
		[Description("Enable the Zoom features.")]
		[DefaultValue(false)]
		public bool Zoom
		{
			get => checkBoxAutoScale.Visible;
			set
			{
				checkBoxAutoScale.Visible = value;
				checkBoxZoomIn.Visible = value;
				checkBoxZoomOut.Visible = value;
				checkBoxHand.Visible = value;
				if (value)
				{
					rightOffset = checkBoxAutoScale.ClientRectangle.Width / 2;
				}
				else
				{
					rightOffset = 0;
				}
				updateData = true;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GraphScale ScaleX
		{
			get => scaleX;
			set
			{
				scaleX = value;
				updateData = true;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GraphScale ScaleY
		{
			get => scaleY;
			set
			{
				scaleY = value;
				updateData = true;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GraphGrid GridX
		{
			get => gridX;
			set
			{
				gridX = value;
				updateData = true;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GraphGrid GridY
		{
			get => gridY;
			set
			{
				gridY = value;
				updateData = true;
			}
		}

		private List<Line> GridLinesX
		{
			get
			{
				var list = new List<Line>();
				var pt = new Point(0, workingArea.Top);
				var pt2 = new Point(0, workingArea.Bottom);
				int num;
				if (GridX.ShowMinor)
				{
					num = GridX.Minor;
				}
				else
				{
					if (!GridX.ShowMajor)
					{
						return list;
					}
					num = GridX.Major;
				}
				int i;
				for (i = GridX.Main; i > FrameZoom.Left; i -= num)
				{
				}
				for (; i < FrameZoom.Left; i += num)
				{
				}
				for (; i < FrameZoom.Right; i += num)
				{
					var num3 = (pt2.X = workingArea.Left + workingArea.Width * (i - FrameZoom.Left) / FrameZoom.Width);
					pt.X = num3;
					list.Add(new Line(pt, pt2));
				}
				return list;
			}
		}

		private List<Line> GridLinesY
		{
			get
			{
				var list = new List<Line>();
				var pt = new Point(workingArea.Left, 0);
				var pt2 = new Point(workingArea.Right, 0);
				int num;
				if (GridY.ShowMinor)
				{
					num = GridY.Minor;
				}
				else
				{
					if (!GridY.ShowMajor)
					{
						return list;
					}
					num = GridY.Major;
				}
				int i;
				for (i = GridY.Main; i > FrameZoom.Bottom; i -= num)
				{
				}
				for (; i < FrameZoom.Bottom; i += num)
				{
				}
				for (; i < FrameZoom.Top; i += num)
				{
					var num3 = (pt2.Y = workingArea.Bottom - workingArea.Height * (i - FrameZoom.Bottom) / FrameZoom.Height);
					pt.Y = num3;
					list.Add(new Line(pt, pt2));
				}
				return list;
			}
		}

		private Rectangle ZoomRect
		{
			get
			{
				var location = default(Point);
				location.X = workingArea.Left + (int)(workingArea.Width * zoomLeft);
				location.Y = workingArea.Top + (int)(workingArea.Height * zoomTop);
				var size = default(Size);
				size.Width = workingArea.Width * FrameZoom.Width / frameFull.Width;
				size.Height = workingArea.Height * FrameZoom.Height / frameFull.Height;
				return new Rectangle(location, size);
			}
		}

		private GraphFrame FrameZoom
		{
			get
			{
                var graphFrame = new GraphFrame
                {
                    Left = frameFull.Left + (int)(frameFull.Width * zoomLeft),
                    Right = frameFull.Right - (int)(frameFull.Width * zoomRight),
                    Top = frameFull.Top - (int)(frameFull.Height * zoomTop),
                    Bottom = frameFull.Bottom + (int)(frameFull.Height * zoomBottom)
                };
                return graphFrame;
			}
			set
			{
				zoomLeft = (value.Left - frameFull.Left) / (float)frameFull.Width;
				zoomRight = (frameFull.Right - value.Right) / (float)frameFull.Width;
				zoomTop = (frameFull.Top - value.Top) / (float)frameFull.Height;
				zoomBottom = (value.Bottom - frameFull.Bottom) / (float)frameFull.Height;
				if (zoomLeft < 0f)
				{
					zoomLeft = 0f;
				}
				if (zoomRight < 0f)
				{
					zoomRight = 0f;
				}
				if (zoomTop < 0f)
				{
					zoomTop = 0f;
				}
				if (zoomBottom < 0f)
				{
					zoomBottom = 0f;
				}
			}
		}

		private Rectangle GraphWindow
		{
			get
			{
				var clientRectangle = ClientRectangle;
				if (!frameFit)
				{
					clientRectangle.Inflate(-5 * clientRectangle.Width / 100 - rightOffset - leftOffset, -5 * clientRectangle.Height / 100);
					var location = new Point(clientRectangle.Location.X, clientRectangle.Location.Y);
					clientRectangle.Location = location;
				}
				return clientRectangle;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			components = new Container();
			var resources = new ComponentResourceManager(typeof(GraphCtrl));
			tmrRefresh = new Timer(components);
			labelScaleYmin = new Label();
			labelScaleYmax = new Label();
			textBoxScaleYmax = new TextBox();
			textBoxScaleYmin = new TextBox();
			labelSample = new Label();
			grphToolTip = new ToolTip(components);
			checkBoxHand = new CheckBox();
			checkBoxZoomOut = new CheckBox();
			checkBoxZoomIn = new CheckBox();
			checkBoxAutoScale = new CheckBox();
			SuspendLayout();
			tmrRefresh.Enabled = true;
			tmrRefresh.Interval = 25;
			tmrRefresh.Tick += tmrRefresh_Tick;
			labelScaleYmin.AutoSize = true;
			labelScaleYmin.Location = new Point(4, 42);
			labelScaleYmin.Name = "labelScaleYmin";
			labelScaleYmin.Size = new Size(23, 13);
			labelScaleYmin.TabIndex = 1;
			labelScaleYmin.Text = "min";
			labelScaleYmin.Click += labelScaleY_Click;
			labelScaleYmax.AutoSize = true;
			labelScaleYmax.Location = new Point(4, 28);
			labelScaleYmax.Name = "labelScaleYmax";
			labelScaleYmax.Size = new Size(26, 13);
			labelScaleYmax.TabIndex = 1;
			labelScaleYmax.Text = "max";
			labelScaleYmax.Click += labelScaleY_Click;
			textBoxScaleYmax.BorderStyle = BorderStyle.None;
			textBoxScaleYmax.Location = new Point(36, 28);
			textBoxScaleYmax.Name = "textBoxScaleYmax";
			textBoxScaleYmax.Size = new Size(47, 13);
			textBoxScaleYmax.TabIndex = 2;
			textBoxScaleYmax.Text = "max";
			textBoxScaleYmax.Visible = false;
			textBoxScaleYmax.KeyPress += textBoxScaleY_KeyPress;
			textBoxScaleYmax.Validated += textBoxScaleY_Validated;
			textBoxScaleYmin.BorderStyle = BorderStyle.None;
			textBoxScaleYmin.Location = new Point(36, 42);
			textBoxScaleYmin.Name = "textBoxScaleYmin";
			textBoxScaleYmin.Size = new Size(47, 13);
			textBoxScaleYmin.TabIndex = 2;
			textBoxScaleYmin.Text = "min";
			textBoxScaleYmin.Visible = false;
			textBoxScaleYmin.KeyPress += textBoxScaleY_KeyPress;
			textBoxScaleYmin.Validated += textBoxScaleY_Validated;
			labelSample.AutoSize = true;
			labelSample.BackColor = Color.Transparent;
			labelSample.Location = new Point(50, 128);
			labelSample.Name = "labelSample";
			labelSample.Size = new Size(42, 13);
			labelSample.TabIndex = 3;
			labelSample.Text = "Sample";
			checkBoxHand.Anchor = AnchorStyles.Right;
			checkBoxHand.Appearance = Appearance.Button;
			checkBoxHand.Image = Resources.Move;
			checkBoxHand.Location = new Point(127, 106);
			checkBoxHand.Name = "checkBoxHand";
			checkBoxHand.Size = new Size(26, 26);
			checkBoxHand.TabIndex = 0;
			checkBoxHand.TextAlign = ContentAlignment.MiddleCenter;
			grphToolTip.SetToolTip(checkBoxHand, "Move:\r\n\r\n-Left Mouse Button Click on the zone you want to move and move the mouse.");
			checkBoxHand.UseVisualStyleBackColor = true;
			checkBoxHand.CheckedChanged += checkBoxZoom_CheckedChanged;
			checkBoxZoomOut.Anchor = AnchorStyles.Right;
			checkBoxZoomOut.Appearance = Appearance.Button;
			checkBoxZoomOut.Image = Resources.ZoomOut;
			checkBoxZoomOut.Location = new Point(127, 80);
			checkBoxZoomOut.Name = "checkBoxZoomOut";
			checkBoxZoomOut.Size = new Size(26, 26);
			checkBoxZoomOut.TabIndex = 0;
			checkBoxZoomOut.TextAlign = ContentAlignment.MiddleCenter;
			grphToolTip.SetToolTip(checkBoxZoomOut, "ZoomOut:\r\n\r\nSelect the button and then each time the graphic is clicked it will zoom out.");
			checkBoxZoomOut.UseVisualStyleBackColor = true;
			checkBoxZoomOut.CheckedChanged += checkBoxZoom_CheckedChanged;
			checkBoxZoomIn.Anchor = AnchorStyles.Right;
			checkBoxZoomIn.Appearance = Appearance.Button;
			checkBoxZoomIn.Image = Resources.ZoomIn;
			checkBoxZoomIn.Location = new Point(127, 54);
			checkBoxZoomIn.Name = "checkBoxZoomIn";
			checkBoxZoomIn.Size = new Size(26, 26);
			checkBoxZoomIn.TabIndex = 0;
			checkBoxZoomIn.TextAlign = ContentAlignment.MiddleCenter;
			grphToolTip.SetToolTip(checkBoxZoomIn, "ZoomIn:\r\n\r\nDraw a rectangle with the Left Mouse button on the graphic zone to zoom");
			checkBoxZoomIn.UseVisualStyleBackColor = true;
			checkBoxZoomIn.CheckedChanged += checkBoxZoom_CheckedChanged;
			checkBoxAutoScale.Anchor = AnchorStyles.Right;
			checkBoxAutoScale.Appearance = Appearance.Button;
			checkBoxAutoScale.Image = Resources.Auto;
			checkBoxAutoScale.Location = new Point(127, 28);
			checkBoxAutoScale.Name = "checkBoxAutoScale";
			checkBoxAutoScale.Size = new Size(26, 26);
			checkBoxAutoScale.TabIndex = 0;
			checkBoxAutoScale.TextAlign = ContentAlignment.MiddleCenter;
			grphToolTip.SetToolTip(checkBoxAutoScale, resources.GetString("checkBoxAutoScale.ToolTip"));
			checkBoxAutoScale.UseVisualStyleBackColor = true;
			checkBoxAutoScale.Click += checkBoxAutoScale_Click;
			checkBoxAutoScale.CheckedChanged += checkBoxZoom_CheckedChanged;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(labelSample);
			Controls.Add(textBoxScaleYmin);
			Controls.Add(textBoxScaleYmax);
			Controls.Add(labelScaleYmax);
			Controls.Add(labelScaleYmin);
			Controls.Add(checkBoxHand);
			Controls.Add(checkBoxZoomOut);
			Controls.Add(checkBoxZoomIn);
			Controls.Add(checkBoxAutoScale);
			Name = "GraphCtrl";
			Size = new Size(155, 161);
			MouseDown += GraphCtrl_MouseDown;
			MouseMove += GraphCtrl_MouseMove;
			Paint += GraphCtrl_Paint;
			MouseUp += GraphCtrl_MouseUp;
			ResumeLayout(false);
			PerformLayout();
		}

		private List<Point> GetPointsZoom(GraphData graphData)
		{
			var list = new List<Point>();
			if (FrameZoom.Height != 0 && FrameZoom.Width != 0)
			{
				var item = default(Point);
				for (var i = 0; i < graphData.Value.Count; i++)
				{
					var num = frameFull.Right - graphData.Value.Count + i;
					item.Y = workingArea.Bottom - workingArea.Height * (graphData.Value[i] - FrameZoom.Bottom) / FrameZoom.Height;
					item.X = workingArea.Right - workingArea.Width * (FrameZoom.Right - num) / FrameZoom.Width;
					list.Add(item);
				}
			}
			return list;
		}

		private List<Point> GetPoints(GraphData graphData)
		{
			var list = new List<Point>();
			if (frameFull.Height != 0 && frameFull.Width != 0)
			{
				var item = default(Point);
				for (var i = 0; i < graphData.Value.Count; i++)
				{
					var num = frameFull.Right - graphData.Value.Count + i;
					item.Y = workingArea.Bottom - workingArea.Height * (graphData.Value[i] - frameFull.Bottom) / frameFull.Height;
					item.X = workingArea.Right - workingArea.Width * (frameFull.Right - num) / (frameFull.Width - 1);
					list.Add(item);
				}
			}
			return list;
		}

		public GraphCtrl()
		{
			InitializeComponent();
			gridX.PropertyChanged += Graph_Changed;
			gridY.PropertyChanged += Graph_Changed;
			scaleX.PropertyChanged += Graph_Changed;
			scaleY.PropertyChanged += Graph_Changed;
			SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.ResizeRedraw, value: true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, value: true);
			updateData = false;
			ColorBackground = Color.Black;
			ColorFrame = Color.Gray;
			ColorLine = Color.White;
			ColorText = Color.Gray;
			Type = eGraphType.Dot;
			History = 100;
			FrameFit = false;
			UpdateRate = 100;
			zoomOption = eZoomOption.None;
			zoomLeft = 0f;
			zoomRight = 0f;
			zoomTop = 0f;
			zoomBottom = 0f;
			ScaleX.Min = 0;
			ScaleX.Max = 100;
			ScaleX.Show = true;
			ScaleY.Min = 0;
			ScaleY.Max = 100;
			ScaleY.Show = true;
			GridX.Main = 0;
			GridX.Major = 25;
			GridX.Minor = 5;
			GridX.ShowMajor = true;
			GridX.ShowMinor = true;
			GridY.Main = 0;
			GridY.Major = 25;
			GridY.Minor = 5;
			GridY.ShowMajor = true;
			GridY.ShowMinor = true;
			Zoom = false;
			leftOffset = 0;
		}

		private void Graph_Changed()
		{
			frameFull.Left = scaleX.Min;
			frameFull.Right = scaleX.Max;
			frameFull.Top = scaleY.Max;
			frameFull.Bottom = scaleY.Min;
			updateData = true;
		}

		private void GraphCtrl_Paint(object sender, PaintEventArgs e)
		{
			if (dataCollection.Count > 0 && dataCollection[0].Value.Count > 0)
			{
				labelSample.Text = dataCollection[0].Value[dataCollection[0].Value.Count - 1].ToString();
			}
			if (zoomOption == eZoomOption.AutoScale && dataCollection[0].Value.Count > 1)
			{
				int num;
				var num2 = (num = dataCollection[0].Value[0]);
				for (var i = 1; i < dataCollection[0].Value.Count; i++)
				{
					if (dataCollection[0].Value[i] < num2)
					{
						num2 = dataCollection[0].Value[i];
					}
					if (dataCollection[0].Value[i] > num)
					{
						num = dataCollection[0].Value[i];
					}
				}
				if (num == num2)
				{
					num++;
					num2--;
				}
				zoomTop = (frameFull.Top - (num + (num - num2) / 10f)) / frameFull.Height;
				zoomBottom = (num2 - (num - num2) / 10f - frameFull.Bottom) / frameFull.Height;
				if (zoomTop < 0f)
				{
					zoomTop = 0f;
				}
				if (zoomBottom < 0f)
				{
					zoomBottom = 0f;
				}
				Graph_Changed();
			}
			e.Graphics.FillRectangle(brushBackground, ClientRectangle);
			workingArea = GraphWindow;
			if (!frameFit)
			{
				if (ScaleY.Show)
				{
					labelScaleYmax.Text = FrameZoom.Top.ToString();
					labelScaleYmin.Text = FrameZoom.Bottom.ToString();
					var location = new Point(0, GraphWindow.Top - labelScaleYmax.Size.Height / 2);
					labelScaleYmax.Location = location;
					textBoxScaleYmax.Location = location;
					location.Y = GraphWindow.Bottom - labelScaleYmin.Size.Height / 2;
					labelScaleYmin.Location = location;
					textBoxScaleYmin.Location = location;
					labelScaleYmax.Visible = true;
					labelScaleYmin.Visible = true;
					leftOffset = Math.Max(labelScaleYmax.ClientRectangle.Width / 2, labelScaleYmin.ClientRectangle.Width / 2);
				}
				else
				{
					labelScaleYmax.Visible = false;
					labelScaleYmin.Visible = false;
					leftOffset = 0;
				}
				if (!ScaleX.Show)
				{
				}
			}
			else
			{
				labelScaleYmax.Visible = false;
				labelScaleYmin.Visible = false;
			}
			var location2 = default(Point);
			location2.X = workingArea.Left + workingArea.Width / 2 - labelSample.ClientRectangle.Width / 2;
			location2.Y = workingArea.Bottom - labelSample.ClientRectangle.Height;
			labelSample.Location = location2;
			e.Graphics.SetClip(workingArea);
			e.Graphics.DrawRectangle(penFrame, workingArea.X, workingArea.Y, workingArea.Width - 1, workingArea.Height - 1);
			penFrame.DashStyle = DashStyle.Dot;
			var gridLinesX = GridLinesX;
			foreach (var item in gridLinesX)
			{
				e.Graphics.DrawLine(penFrame, item.Pt1, item.Pt2);
			}
			gridLinesX = GridLinesY;
			foreach (var item2 in gridLinesX)
			{
				e.Graphics.DrawLine(penFrame, item2.Pt1, item2.Pt2);
			}
			penFrame.DashStyle = DashStyle.Solid;
			var num3 = 0;
			foreach (var item3 in dataCollection)
			{
				if (item3.GraphType == eGraphType.Bar)
				{
					num3++;
				}
			}
			var num4 = 0;
			foreach (var item4 in dataCollection)
			{
				penLine.Color = item4.Color;
				brushPoint = new SolidBrush(item4.Color);
				var pointsZoom = GetPointsZoom(item4);
				switch (item4.GraphType)
				{
				case eGraphType.Dot:
					foreach (var item5 in pointsZoom)
					{
						e.Graphics.FillEllipse(brushPoint, item5.X, item5.Y, 2, 2);
					}
					break;
				case eGraphType.Line:
				{
					for (var j = 0; j < pointsZoom.Count - 1; j++)
					{
						e.Graphics.DrawLine(penLine, pointsZoom[j], pointsZoom[j + 1]);
					}
					break;
				}
				case eGraphType.Bar:
					if (pointsZoom.Count > 0)
					{
						var num5 = workingArea.Bottom - workingArea.Height * -FrameZoom.Bottom / FrameZoom.Height;
						if (pointsZoom[pointsZoom.Count - 1].Y < num5)
						{
							e.Graphics.FillRectangle(brushPoint, new Rectangle(workingArea.Location.X + num4 * (workingArea.Width / num3), pointsZoom[pointsZoom.Count - 1].Y, workingArea.Width / num3, num5 - pointsZoom[pointsZoom.Count - 1].Y));
							break;
						}
						if (pointsZoom[pointsZoom.Count - 1].Y > num5)
						{
							e.Graphics.FillRectangle(brushPoint, new Rectangle(workingArea.Location.X + num4 * (workingArea.Width / num3), num5, workingArea.Width / num3, pointsZoom[pointsZoom.Count - 1].Y - num5));
							break;
						}
						penLine.Color = Color.Blue;
						e.Graphics.DrawRectangle(penLine, workingArea.Location.X + num4 * (workingArea.Width / num3), num5, workingArea.Width / num3, 0.1f);
					}
					break;
				}
				num4++;
			}
			if (FrameZoom.Top != frameFull.Top || FrameZoom.Bottom != frameFull.Bottom || FrameZoom.Left != frameFull.Left || FrameZoom.Right != frameFull.Right)
			{
				var location3 = new Point(workingArea.Location.X + workingArea.Width / 50, workingArea.Location.Y + workingArea.Height / 50);
				workingArea.Inflate(-2 * workingArea.Width / 5, -2 * workingArea.Height / 5);
				workingArea.Location = location3;
				e.Graphics.SetClip(workingArea);
				e.Graphics.FillRectangle(brushBackground, workingArea);
				e.Graphics.DrawRectangle(penLine, workingArea.X, workingArea.Y, workingArea.Width - 1, workingArea.Height - 1);
				num4 = 0;
				foreach (var item6 in dataCollection)
				{
					penLine.Color = item6.Color;
					brushPoint = new SolidBrush(item6.Color);
					var points = GetPoints(item6);
					switch (item6.GraphType)
					{
					case eGraphType.Dot:
						foreach (var item7 in points)
						{
							e.Graphics.FillEllipse(brushPoint, item7.X, item7.Y, 2, 2);
						}
						break;
					case eGraphType.Line:
					{
						for (var k = 0; k < points.Count - 1; k++)
						{
							e.Graphics.DrawLine(penLine, points[k], points[k + 1]);
						}
						break;
					}
					case eGraphType.Bar:
						if (points.Count > 0)
						{
							e.Graphics.FillRectangle(brushPoint, new Rectangle(workingArea.Location.X + num4 * (workingArea.Width / num3), points[points.Count - 1].Y, workingArea.Width / num3, workingArea.Location.Y + workingArea.Height));
						}
						break;
					}
					num4++;
				}
				penFrame.DashStyle = DashStyle.Dot;
				e.Graphics.DrawRectangle(penFrame, ZoomRect);
				penFrame.DashStyle = DashStyle.Solid;
			}
			if (Zoom)
			{
				var num6 = ClientRectangle.Right - (checkBoxAutoScale.Width + 3);
				var num7 = (ClientRectangle.Bottom - ClientRectangle.Top) / 2;
				var location4 = new Point(num6, num7 - checkBoxAutoScale.Height * 2);
				checkBoxAutoScale.Location = location4;
				location4 = new Point(num6, num7 - checkBoxAutoScale.Height);
				checkBoxZoomIn.Location = location4;
				location4 = new Point(num6, num7);
				checkBoxZoomOut.Location = location4;
				location4 = new Point(num6, num7 + checkBoxAutoScale.Height);
				checkBoxHand.Location = location4;
			}
		}

		public void AddData(int series, int data, Color color, eGraphType graphType)
		{
			if (series >= dataCollection.Count)
			{
				dataCollection.Add(new GraphData(new List<int>(data), color, graphType));
			}
			dataCollection[series].Color = color;
			dataCollection[series].Value.Add(data);
			if (dataCollection[series].Value.Count > history)
			{
				dataCollection[series].Value.RemoveRange(0, dataCollection[series].Value.Count - history);
			}
			updateData = true;
		}

		public void ClearData(int series)
		{
			dataCollection[series].Value.Clear();
		}

		private void tmrRefresh_Tick(object sender, EventArgs e)
		{
			if (updateData)
			{
				Invalidate();
			}
			updateData = false;
		}

		private void GraphCtrl_MouseDown(object sender, MouseEventArgs e)
		{
			switch (zoomOption)
			{
			case eZoomOption.ZoomIn:
				if (e.Button == MouseButtons.Left)
				{
					mousePos = e.Location;
				}
				break;
			case eZoomOption.ZoomOut:
				if (e.Button == MouseButtons.Left)
				{
					var graphFrame = FrameZoom;
					graphFrame *= 1.5f;
					FrameZoom = graphFrame;
					updateData = true;
				}
				break;
			case eZoomOption.Hand:
				if (e.Button == MouseButtons.Left)
				{
					mousePos = e.Location;
					frameZoom = FrameZoom;
				}
				break;
			case eZoomOption.None:
			case eZoomOption.AutoScale:
			case eZoomOption.FullScale:
				break;
			}
		}

		private void GraphCtrl_MouseMove(object sender, MouseEventArgs e)
		{
			switch (zoomOption)
			{
			case eZoomOption.ZoomIn:
				if (e.Button == MouseButtons.Left)
				{
					Refresh();
					var graphics = CreateGraphics();
					var point = default(Point);
					var point2 = default(Point);
					point.X = ((mousePos.X < e.X) ? mousePos.X : e.X);
					point2.X = ((mousePos.X >= e.X) ? mousePos.X : e.X);
					point.Y = ((mousePos.Y < e.Y) ? mousePos.Y : e.Y);
					point2.Y = ((mousePos.Y >= e.Y) ? mousePos.Y : e.Y);
					var rect = new Rectangle(point.X, point.Y, point2.X - point.X, point2.Y - point.Y);
					var clip = new Rectangle(point.X, point.Y, point2.X - point.X + 1, point2.Y - point.Y + 1);
					graphics.SetClip(clip);
                        var pen = new Pen(Color.Gray)
                        {
                            DashStyle = DashStyle.Dot
                        };
                        graphics.DrawRectangle(pen, rect);
				}
				break;
			case eZoomOption.Hand:
				if (e.Button == MouseButtons.Left)
				{
					var graphFrame = new GraphFrame();
					workingArea = GraphWindow;
					graphFrame.Left = frameZoom.Left - frameZoom.Width * (e.X - mousePos.X) / workingArea.Width;
					graphFrame.Right = frameZoom.Right - frameZoom.Width * (e.X - mousePos.X) / workingArea.Width;
					graphFrame.Top = frameZoom.Top - frameZoom.Height * (mousePos.Y - e.Y) / workingArea.Height;
					graphFrame.Bottom = frameZoom.Bottom - frameZoom.Height * (mousePos.Y - e.Y) / workingArea.Height;
					if (graphFrame.Left < frameFull.Left)
					{
						graphFrame.Left = frameFull.Left;
						graphFrame.Right = frameFull.Left + frameZoom.Width;
					}
					if (graphFrame.Right > frameFull.Right)
					{
						graphFrame.Right = frameFull.Right;
						graphFrame.Left = frameFull.Right - frameZoom.Width;
					}
					if (graphFrame.Top > frameFull.Top)
					{
						graphFrame.Top = frameFull.Top;
						graphFrame.Bottom = frameFull.Top - frameZoom.Height;
					}
					if (graphFrame.Bottom < frameFull.Bottom)
					{
						graphFrame.Bottom = frameFull.Bottom;
						graphFrame.Top = frameFull.Bottom + frameZoom.Height;
					}
					FrameZoom = graphFrame;
					updateData = true;
				}
				break;
			case eZoomOption.None:
			case eZoomOption.ZoomOut:
			case eZoomOption.AutoScale:
				break;
			}
		}

		private void GraphCtrl_MouseUp(object sender, MouseEventArgs e)
		{
			switch (zoomOption)
			{
			case eZoomOption.ZoomIn:
				if (e.Button == MouseButtons.Left)
				{
					var num = ((mousePos.X < e.X) ? mousePos.X : e.X);
					var num2 = ((mousePos.X >= e.X) ? mousePos.X : e.X);
					var num3 = ((mousePos.Y < e.Y) ? mousePos.Y : e.Y);
					var num4 = ((mousePos.Y >= e.Y) ? mousePos.Y : e.Y);
					if (num == num2 || num3 == num4)
					{
						FrameZoom *= 0.5f;
					}
					else
					{
						var graphFrame = new GraphFrame();
						var graphFrame2 = new GraphFrame();
						workingArea = GraphWindow;
						graphFrame = FrameZoom;
						graphFrame2.Left = graphFrame.Left + graphFrame.Width * (num - workingArea.Left) / workingArea.Width;
						graphFrame2.Right = graphFrame.Right - graphFrame.Width * (workingArea.Right - num2) / workingArea.Width;
						graphFrame2.Top = graphFrame.Top - graphFrame.Height * (num3 - workingArea.Top) / workingArea.Height;
						graphFrame2.Bottom = graphFrame.Bottom + graphFrame.Height * (workingArea.Bottom - num4) / workingArea.Height;
						FrameZoom = graphFrame2;
					}
					updateData = true;
				}
				break;
			case eZoomOption.None:
			case eZoomOption.ZoomOut:
			case eZoomOption.AutoScale:
			case eZoomOption.Hand:
			case eZoomOption.FullScale:
				break;
			}
		}

		private void checkBoxZoom_CheckedChanged(object sender, EventArgs e)
		{
			if (sender == checkBoxAutoScale)
			{
				if (checkBoxAutoScale.Checked)
				{
					checkBoxAutoScale.Image = Resources.AutoSelected;
					checkBoxZoomIn.Image = Resources.ZoomIn;
					checkBoxZoomOut.Image = Resources.ZoomOut;
					checkBoxHand.Image = Resources.Move;
					checkBoxZoomIn.Checked = false;
					checkBoxZoomOut.Checked = false;
					checkBoxHand.Checked = false;
					zoomOption = eZoomOption.AutoScale;
				}
				else
				{
					checkBoxAutoScale.Image = Resources.Auto;
					zoomOption = eZoomOption.None;
				}
			}
			else if (sender == checkBoxZoomIn)
			{
				if (checkBoxZoomIn.Checked)
				{
					checkBoxAutoScale.Image = Resources.Auto;
					checkBoxZoomIn.Image = Resources.ZoomInSelected;
					checkBoxZoomOut.Image = Resources.ZoomOut;
					checkBoxHand.Image = Resources.Move;
					checkBoxAutoScale.Checked = false;
					checkBoxZoomOut.Checked = false;
					checkBoxHand.Checked = false;
					zoomOption = eZoomOption.ZoomIn;
				}
				else
				{
					checkBoxZoomIn.Image = Resources.ZoomIn;
					zoomOption = eZoomOption.None;
				}
			}
			else if (sender == checkBoxZoomOut)
			{
				if (checkBoxZoomOut.Checked)
				{
					checkBoxAutoScale.Image = Resources.Auto;
					checkBoxZoomIn.Image = Resources.ZoomIn;
					checkBoxZoomOut.Image = Resources.ZoomOutSelected;
					checkBoxHand.Image = Resources.Move;
					checkBoxAutoScale.Checked = false;
					checkBoxZoomIn.Checked = false;
					checkBoxHand.Checked = false;
					zoomOption = eZoomOption.ZoomOut;
				}
				else
				{
					checkBoxZoomOut.Image = Resources.ZoomOut;
					zoomOption = eZoomOption.None;
				}
			}
			else if (sender == checkBoxHand)
			{
				if (checkBoxHand.Checked)
				{
					checkBoxAutoScale.Image = Resources.Auto;
					checkBoxZoomIn.Image = Resources.ZoomIn;
					checkBoxZoomOut.Image = Resources.ZoomOut;
					checkBoxHand.Image = Resources.MoveSelected;
					checkBoxAutoScale.Checked = false;
					checkBoxZoomIn.Checked = false;
					checkBoxZoomOut.Checked = false;
					zoomOption = eZoomOption.Hand;
				}
				else
				{
					checkBoxHand.Image = Resources.Move;
					zoomOption = eZoomOption.None;
				}
			}
			Refresh();
		}

		private void checkBoxAutoScale_Click(object sender, EventArgs e)
		{
			var tickCount = Environment.TickCount;
			if (tickCount - previousClick <= SystemInformation.DoubleClickTime)
			{
				checkBoxAutoScale.Image = Resources.AutoSelected;
				checkBoxZoomIn.Image = Resources.ZoomIn;
				checkBoxZoomOut.Image = Resources.ZoomOut;
				checkBoxHand.Image = Resources.Move;
				checkBoxAutoScale.Checked = true;
				zoomOption = eZoomOption.AutoScale;
			}
			else if (checkBoxAutoScale.Checked)
			{
				checkBoxAutoScale.Checked = false;
				checkBoxAutoScale.Image = Resources.Auto;
				zoomOption = eZoomOption.None;
			}
			previousClick = tickCount;
		}

		private void labelScaleY_Click(object sender, EventArgs e)
		{
			var label = (Label)sender;
			var textBox = ((!label.Equals(labelScaleYmax)) ? textBoxScaleYmin : textBoxScaleYmax);
			textBox.Text = label.Text;
			textBox.Visible = true;
			textBox.Focus();
			textBox.SelectAll();
		}

		private void textBoxScaleY_Validated(object sender, EventArgs e)
		{
			var textBox = (TextBox)sender;
			textBox.Visible = false;
			var graphFrame = FrameZoom;
			if (textBox == textBoxScaleYmax)
			{
				graphFrame.Top = Convert.ToInt32(textBox.Text);
			}
			else if (textBox == textBoxScaleYmin)
			{
				graphFrame.Bottom = Convert.ToInt32(textBox.Text);
			}
			FrameZoom = graphFrame;
			updateData = true;
		}

		private void textBoxScaleY_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != '-')
			{
				e.Handled = true;
				if (e.KeyChar == '\r')
				{
					textBoxScaleY_Validated(sender, EventArgs.Empty);
				}
			}
		}
	}
}
