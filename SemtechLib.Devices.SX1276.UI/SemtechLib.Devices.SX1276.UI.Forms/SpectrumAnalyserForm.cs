using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SemtechLib.Controls;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.Devices.SX1276.UI.Controls;
using SemtechLib.General;
using ZedGraph;

namespace SemtechLib.Devices.SX1276.UI.Forms
{
	public sealed class SpectrumAnalyserForm : Form
	{
		private delegate void DeviceDataChangedDelegate(object sender, PropertyChangedEventArgs e);

		private IContainer components;

		private Panel panel1;

		private Panel panel2;

		private SpectrumGraphControl graph;

		private NumericUpDownEx nudFreqCenter;

		private NumericUpDownEx nudFreqSpan;

		private NumericUpDownEx nudChanBw;

		private System.Windows.Forms.Label label2;

		private System.Windows.Forms.Label label1;

		private System.Windows.Forms.Label label6;

		private System.Windows.Forms.Label label3;

		private System.Windows.Forms.Label label4;

		private System.Windows.Forms.Label label5;

		private ComboBox cBoxLanGain;

		private System.Windows.Forms.Label label7;

		private PointPairList points;

		private decimal rxBw = 10417m;
        private SX1276 device;

		private decimal FrequencyRf
		{
			get => nudFreqCenter.Value;
			set
			{
				try
				{
					nudFreqCenter.ValueChanged -= nudFreqCenter_ValueChanged;
					var num = (uint)Math.Round(value / device.FrequencyStep, MidpointRounding.AwayFromZero);
					nudFreqCenter.Value = num * device.FrequencyStep;
				}
				catch (Exception)
				{
					nudFreqCenter.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudFreqCenter.ValueChanged += nudFreqCenter_ValueChanged;
				}
			}
		}

		private decimal FrequencySpan
		{
			get => nudFreqSpan.Value;
			set
			{
				try
				{
					nudFreqSpan.ValueChanged -= nudFreqSpan_ValueChanged;
					var num = (uint)Math.Round(value / device.FrequencyStep, MidpointRounding.AwayFromZero);
					nudFreqSpan.Value = num * device.FrequencyStep;
				}
				catch (Exception)
				{
					nudFreqSpan.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudFreqSpan.ValueChanged += nudFreqSpan_ValueChanged;
				}
			}
		}

		private decimal RxBw
		{
			get => rxBw;
			set
			{
				try
				{
					nudChanBw.ValueChanged -= nudChanBw_ValueChanged;
					var mant = 0;
					var exp = 0;
					SX1276.ComputeRxBwMantExp(device.FrequencyXo, device.ModulationType, value, ref mant, ref exp);
					rxBw = SX1276.ComputeRxBw(device.FrequencyXo, device.ModulationType, mant, exp);
					nudChanBw.Value = rxBw;
				}
				catch (Exception)
				{
				}
				finally
				{
					nudChanBw.ValueChanged += nudChanBw_ValueChanged;
				}
			}
		}

		private LnaGainEnum LnaGain
		{
			get => (LnaGainEnum)(cBoxLanGain.SelectedIndex + 1);
			set
			{
				cBoxLanGain.SelectedIndexChanged -= cBoxLanGain_SelectedIndexChanged;
				cBoxLanGain.SelectedIndex = (int)(value - 1);
				cBoxLanGain.SelectedIndexChanged += cBoxLanGain_SelectedIndexChanged;
			}
		}

        public ApplicationSettings AppSettings { get; set; }

        public IDevice Device
		{
			set
			{
				if (device != value)
				{
					device = (SX1276)value;
					device.PropertyChanged += device_PropertyChanged;
					FrequencyRf = device.FrequencyRf;
					RxBw = device.RxBw;
					LnaGain = device.LnaGain;
					UpdatePointsList();
				}
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
			var resources = new ComponentResourceManager(typeof(SpectrumAnalyserForm));
			panel1 = new Panel();
			cBoxLanGain = new ComboBox();
			nudFreqCenter = new NumericUpDownEx();
			nudFreqSpan = new NumericUpDownEx();
			nudChanBw = new NumericUpDownEx();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			panel2 = new Panel();
			graph = new SpectrumGraphControl();
			panel1.SuspendLayout();
            nudFreqCenter.BeginInit();
            nudFreqSpan.BeginInit();
            nudChanBw.BeginInit();
			panel2.SuspendLayout();
			SuspendLayout();
			panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
			panel1.BackColor = Color.Black;
			panel1.BorderStyle = BorderStyle.FixedSingle;
			panel1.Controls.Add(cBoxLanGain);
			panel1.Controls.Add(nudFreqCenter);
			panel1.Controls.Add(nudFreqSpan);
			panel1.Controls.Add(nudChanBw);
			panel1.Controls.Add(label2);
			panel1.Controls.Add(label1);
			panel1.Controls.Add(label6);
			panel1.Controls.Add(label7);
			panel1.Controls.Add(label3);
			panel1.Controls.Add(label4);
			panel1.Controls.Add(label5);
			panel1.Location = new Point(557, 0);
			panel1.Margin = new Padding(0);
			panel1.Name = "panel1";
			panel1.Size = new Size(223, 370);
			panel1.TabIndex = 0;
			cBoxLanGain.FormattingEnabled = true;
			cBoxLanGain.Items.AddRange(new object[6] { "G1", "G2", "G3", "G4", "G5", "G6" });
			cBoxLanGain.Location = new Point(99, 181);
			cBoxLanGain.Name = "cBoxLanGain";
			cBoxLanGain.Size = new Size(98, 21);
			cBoxLanGain.TabIndex = 10;
			cBoxLanGain.SelectedIndexChanged += cBoxLanGain_SelectedIndexChanged;
			nudFreqCenter.Anchor = AnchorStyles.None;
			nudFreqCenter.Increment = new decimal(new int[4] { 61, 0, 0, 0 });
			nudFreqCenter.Location = new Point(99, 103);
			nudFreqCenter.Maximum = new decimal(new int[4] { 1020000000, 0, 0, 0 });
			nudFreqCenter.Minimum = new decimal(new int[4] { 290000000, 0, 0, 0 });
			nudFreqCenter.Name = "nudFreqCenter";
			nudFreqCenter.Size = new Size(98, 20);
			nudFreqCenter.TabIndex = 1;
			nudFreqCenter.ThousandsSeparator = true;
			nudFreqCenter.Value = new decimal(new int[4] { 915000000, 0, 0, 0 });
			nudFreqCenter.ValueChanged += nudFreqCenter_ValueChanged;
			nudFreqSpan.Anchor = AnchorStyles.None;
			nudFreqSpan.Increment = new decimal(new int[4] { 61, 0, 0, 0 });
			nudFreqSpan.Location = new Point(99, 129);
			nudFreqSpan.Maximum = new decimal(new int[4] { 100000000, 0, 0, 0 });
			nudFreqSpan.Name = "nudFreqSpan";
			nudFreqSpan.Size = new Size(98, 20);
			nudFreqSpan.TabIndex = 4;
			nudFreqSpan.ThousandsSeparator = true;
			nudFreqSpan.Value = new decimal(new int[4] { 1000000, 0, 0, 0 });
			nudFreqSpan.ValueChanged += nudFreqSpan_ValueChanged;
			nudChanBw.Anchor = AnchorStyles.None;
			nudChanBw.Location = new Point(99, 155);
			nudChanBw.Maximum = new decimal(new int[4] { 500000, 0, 0, 0 });
			nudChanBw.Minimum = new decimal(new int[4] { 3906, 0, 0, 0 });
			nudChanBw.Name = "nudChanBw";
			nudChanBw.Size = new Size(98, 20);
			nudChanBw.TabIndex = 7;
			nudChanBw.ThousandsSeparator = true;
			nudChanBw.Value = new decimal(new int[4] { 10417, 0, 0, 0 });
			nudChanBw.ValueChanged += nudChanBw_ValueChanged;
			label2.Anchor = AnchorStyles.None;
			label2.AutoSize = true;
			label2.BackColor = Color.Transparent;
			label2.ForeColor = Color.Gray;
			label2.Location = new Point(-2, 133);
			label2.Name = "label2";
			label2.Size = new Size(35, 13);
			label2.TabIndex = 3;
			label2.Text = "Span:";
			label1.Anchor = AnchorStyles.None;
			label1.AutoSize = true;
			label1.BackColor = Color.Transparent;
			label1.ForeColor = Color.Gray;
			label1.Location = new Point(-2, 107);
			label1.Name = "label1";
			label1.Size = new Size(91, 13);
			label1.TabIndex = 0;
			label1.Text = "Center frequency:";
			label6.Anchor = AnchorStyles.None;
			label6.AutoSize = true;
			label6.ForeColor = Color.Gray;
			label6.Location = new Point(203, 159);
			label6.Name = "label6";
			label6.Size = new Size(20, 13);
			label6.TabIndex = 8;
			label6.Text = "Hz";
			label7.Anchor = AnchorStyles.None;
			label7.AutoSize = true;
			label7.BackColor = Color.Transparent;
			label7.ForeColor = Color.Gray;
			label7.Location = new Point(-2, 185);
			label7.Name = "label7";
			label7.Size = new Size(54, 13);
			label7.TabIndex = 9;
			label7.Text = "LNA gain:";
			label3.Anchor = AnchorStyles.None;
			label3.AutoSize = true;
			label3.BackColor = Color.Transparent;
			label3.ForeColor = Color.Gray;
			label3.Location = new Point(-2, 159);
			label3.Name = "label3";
			label3.Size = new Size(101, 13);
			label3.TabIndex = 6;
			label3.Text = "Channel bandwidth:";
			label4.Anchor = AnchorStyles.None;
			label4.AutoSize = true;
			label4.ForeColor = Color.Gray;
			label4.Location = new Point(203, 107);
			label4.Name = "label4";
			label4.Size = new Size(20, 13);
			label4.TabIndex = 2;
			label4.Text = "Hz";
			label5.Anchor = AnchorStyles.None;
			label5.AutoSize = true;
			label5.ForeColor = Color.Gray;
			label5.Location = new Point(203, 133);
			label5.Name = "label5";
			label5.Size = new Size(20, 13);
			label5.TabIndex = 5;
			label5.Text = "Hz";
			panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel2.BorderStyle = BorderStyle.FixedSingle;
			panel2.Controls.Add(graph);
			panel2.Location = new Point(0, 0);
			panel2.Margin = new Padding(0);
			panel2.Name = "panel2";
			panel2.Size = new Size(557, 370);
			panel2.TabIndex = 2;
			graph.Dock = DockStyle.Fill;
			graph.Location = new Point(0, 0);
			graph.Name = "graph";
			graph.Size = new Size(555, 368);
			graph.TabIndex = 0;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(780, 370);
			Controls.Add(panel2);
			Controls.Add(panel1);
			Icon = (Icon)resources.GetObject("$this.Icon");
			Name = "SpectrumAnalyserForm";
			Text = "Spectrum analyser";
			Load += SpectrumAnalyserForm_Load;
			FormClosed += SpectrumAnalyserForm_FormClosed;
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
            nudFreqCenter.EndInit();
            nudFreqSpan.EndInit();
            nudChanBw.EndInit();
			panel2.ResumeLayout(false);
			ResumeLayout(false);
		}

		public SpectrumAnalyserForm()
		{
			InitializeComponent();
		}

		private bool IsFormLocatedInScreen(Form frm, Screen[] screens)
		{
			var upperBound = screens.GetUpperBound(0);
			var result = false;
			for (var i = 0; i <= upperBound; i++)
			{
				if (frm.Left < screens[i].WorkingArea.Left || frm.Top < screens[i].WorkingArea.Top || frm.Left > screens[i].WorkingArea.Right || frm.Top > screens[i].WorkingArea.Bottom)
				{
					result = false;
					continue;
				}
				result = true;
				break;
			}
			return result;
		}

		private void UpdatePointsList()
		{
			var graphPane = graph.PaneList[0];
			graphPane.XAxis.Scale.Max = (double)device.SpectrumFrequencyMax;
			graphPane.XAxis.Scale.Min = (double)device.SpectrumFrequencyMin;
			device.SpectrumFrequencyId = 0;
			points = new PointPairList();
			for (var i = 0; i < device.SpectrumNbFrequenciesMax; i++)
			{
				points.Add(new PointPair((double)(device.SpectrumFrequencyMin + device.SpectrumFrequencyStep * i), -127.5));
			}
			graphPane.CurveList[0] = new LineItem("", points, Color.Yellow, SymbolType.None);
			graphPane.AxisChange();
			graph.Invalidate();
			graph.Refresh();
		}

		private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			try
			{
				switch (e.PropertyName)
				{
				case "FrequencyRf":
					FrequencyRf = device.FrequencyRf;
					UpdatePointsList();
					break;
				case "RxBw":
					RxBw = device.RxBw;
					UpdatePointsList();
					break;
				case "RxBwMin":
					nudChanBw.Minimum = device.RxBwMin;
					break;
				case "RxBwMax":
					nudChanBw.Maximum = device.RxBwMax;
					break;
				case "SpectrumFreqSpan":
					FrequencySpan = device.SpectrumFrequencySpan;
					UpdatePointsList();
					break;
				case "LnaGain":
					LnaGain = device.LnaGain;
					break;
				case "SpectrumData":
					graph.UpdateLineGraph(device.SpectrumFrequencyId, (double)device.SpectrumRssiValue);
					break;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		private void OnError(byte status, string message)
		{
			Refresh();
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new DeviceDataChangedDelegate(OnDevicePropertyChanged), sender, e);
			}
			else
			{
				OnDevicePropertyChanged(sender, e);
			}
		}

		private void SpectrumAnalyserForm_Load(object sender, EventArgs e)
		{
			var value = AppSettings.GetValue("SpectrumAnalyserTop");
			if (value != null)
			{
				try
				{
					Top = int.Parse(value);
				}
				catch
				{
					MessageBox.Show(this, "Error getting Top value.");
				}
			}
			value = AppSettings.GetValue("SpectrumAnalyserLeft");
			if (value != null)
			{
				try
				{
					Left = int.Parse(value);
				}
				catch
				{
					MessageBox.Show(this, "Error getting Left value.");
				}
			}
			var allScreens = Screen.AllScreens;
			if (!IsFormLocatedInScreen(this, allScreens))
			{
				Top = allScreens[0].WorkingArea.Top;
				Left = allScreens[0].WorkingArea.Left;
			}
			device.SpectrumOn = true;
		}

		private void SpectrumAnalyserForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				AppSettings.SetValue("SpectrumAnalyserTop", Top.ToString());
				AppSettings.SetValue("SpectrumAnalyserLeft", Left.ToString());
				device.SpectrumOn = false;
			}
			catch (Exception)
			{
			}
		}

		private void nudFreqCenter_ValueChanged(object sender, EventArgs e)
		{
			FrequencyRf = nudFreqCenter.Value;
			device.SetFrequencyRf(FrequencyRf);
		}

		private void nudFreqSpan_ValueChanged(object sender, EventArgs e)
		{
			FrequencySpan = nudFreqSpan.Value;
			device.SpectrumFrequencySpan = FrequencySpan;
		}

		private void nudChanBw_ValueChanged(object sender, EventArgs e)
		{
			var array = SX1276.ComputeRxBwFreqTable(device.FrequencyXo, device.ModulationType);
			var num = 0;
			var num2 = (int)(nudChanBw.Value - RxBw);
			if (num2 is >= -1 and <= 1)
			{
				num = Array.IndexOf(array, RxBw) - num2;
			}
			else
			{
				var mant = 0;
				var exp = 0;
				var num3 = 0m;
				SX1276.ComputeRxBwMantExp(device.FrequencyXo, device.ModulationType, nudChanBw.Value, ref mant, ref exp);
				num3 = SX1276.ComputeRxBw(device.FrequencyXo, device.ModulationType, mant, exp);
				num = Array.IndexOf(array, num3);
			}
			nudChanBw.ValueChanged -= nudChanBw_ValueChanged;
			nudChanBw.Value = array[num];
			nudChanBw.ValueChanged += nudChanBw_ValueChanged;
			RxBw = nudChanBw.Value;
			device.SetRxBw(RxBw);
		}

		private void cBoxLanGain_SelectedIndexChanged(object sender, EventArgs e)
		{
			device.SetLnaGain(LnaGain);
		}
	}
}
