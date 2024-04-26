using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SemtechLib.Controls;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276.General;
using SemtechLib.Devices.SX1276.UI.Controls;
using SemtechLib.General;
using ZedGraph;

namespace SemtechLib.Devices.SX1276.UI.Forms
{
	public sealed class RssiAnalyserForm : Form
	{
		private delegate void DeviceDataChangedDelegate(object sender, PropertyChangedEventArgs e);

		private readonly int tickStart = Environment.TickCount;
        private SX1276 device;
        private double time;

		private string previousValue;

		private IContainer components;

		private Panel panel1;

		private System.Windows.Forms.Label label1;

		private System.Windows.Forms.Label label8;

		private System.Windows.Forms.Label label3;

		private System.Windows.Forms.Label label2;

		private Panel panel2;

		private RssiGraphControl graph;

		private NumericUpDownEx nudRssiThresh;

		private System.Windows.Forms.Label label55;

		private GroupBoxEx groupBox5;

		private Button btnLogBrowseFile;

		private ProgressBar pBarLog;

		private TableLayoutPanel tableLayoutPanel3;

		private TextBox tBoxLogMaxSamples;

		private System.Windows.Forms.Label lblCommandsLogMaxSamples;

		private CheckBox cBtnLogOnOff;

		private SaveFileDialog sfLogSaveFileDlg;

		private System.Windows.Forms.Label label9;

		private System.Windows.Forms.Label label7;

		private System.Windows.Forms.Label label5;

		private System.Windows.Forms.Label label4;

        public ApplicationSettings AppSettings { get; set; }

        public IDevice Device
		{
			set
			{
				if (device != value)
				{
					device = (SX1276)value;
					Log.Device = device;
					device.PropertyChanged += device_PropertyChanged;
					CreateThreshold();
					nudRssiThresh.Value = device.RssiThreshold;
					UpdateThreshold((double)nudRssiThresh.Value);
				}
			}
		}

		private DataLog Log { get; } = new();

        public RssiAnalyserForm()
		{
			InitializeComponent();
			graph.MouseWheel += graph_MouseWheel;
			Log.PropertyChanged += log_PropertyChanged;
			Log.Stoped += log_Stoped;
			Log.ProgressChanged += log_ProgressChanged;
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

		private void CreateThreshold()
		{
			var graphPane = graph.PaneList[0];
			var num = 0.0;
			var lineObj = new LineObj(Color.Green, 0.0, num, 1.0, num)
			{
				Location =
				{
					CoordinateFrame = CoordType.XChartFractionYScale
				},
				IsVisible = true
			};
			graphPane.GraphObjList.Add(lineObj);
			graphPane.AxisChange();
			graph.Invalidate();
		}

		private void UpdateThreshold(double thres)
		{
			var graphPane = graph.PaneList[0];
			(graphPane.GraphObjList[0] as LineObj).Location.Y = thres;
			(graphPane.GraphObjList[0] as LineObj).Location.Y1 = thres;
			if (thres < graphPane.YAxis.Scale.Max && thres > graphPane.YAxis.Scale.Min)
			{
				(graphPane.GraphObjList[0] as LineObj).IsVisible = true;
			}
			else
			{
				(graphPane.GraphObjList[0] as LineObj).IsVisible = false;
			}
			graphPane.AxisChange();
			graph.Invalidate();
		}

		private void UpdateProgressBarStyle()
		{
			if (Log.MaxSamples == 0 && cBtnLogOnOff.Checked)
			{
				pBarLog.Style = ProgressBarStyle.Marquee;
			}
			else
			{
				pBarLog.Style = ProgressBarStyle.Continuous;
			}
		}

		private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
			case "RssiValue":
				if (device.RfPaSwitchEnabled == 0)
				{
					time = (Environment.TickCount - tickStart) / 1000.0;
					graph.UpdateLineGraph(time, (double)device.RssiValue);
				}
				break;
			case "RfPaSwitchEnabled":
				label9.Visible = device.RfPaSwitchEnabled != 0;
				label7.Visible = device.RfPaSwitchEnabled != 0;
				label5.Visible = device.RfPaSwitchEnabled != 0;
				label4.Visible = device.RfPaSwitchEnabled != 0;
				label2.Visible = device.RfPaSwitchEnabled == 0;
				label8.Visible = device.RfPaSwitchEnabled == 0;
				break;
			case "RfPaRssiValue":
				if (device.RfPaSwitchEnabled == 1)
				{
					time = (Environment.TickCount - tickStart) / 1000.0;
					graph.UpdateLineGraph(1, time, (double)device.RfPaRssiValue);
					graph.UpdateLineGraph(2, time, (double)device.RfIoRssiValue);
				}
				break;
			case "RfIoRssiValue":
				if (device.RfPaSwitchEnabled != 0)
				{
					time = (Environment.TickCount - tickStart) / 1000.0;
					graph.UpdateLineGraph(1, time, (double)device.RfPaRssiValue);
					graph.UpdateLineGraph(2, time, (double)device.RfIoRssiValue);
				}
				break;
			case "RssiThreshold":
				nudRssiThresh.Value = device.RssiThreshold;
				break;
			}
			UpdateThreshold((double)nudRssiThresh.Value);
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

		private void tBoxLogMaxSamples_Enter(object sender, EventArgs e)
		{
			previousValue = tBoxLogMaxSamples.Text;
		}

		private void tBoxLogMaxSamples_Validating(object sender, CancelEventArgs e)
		{
			try
			{
				Convert.ToUInt64(tBoxLogMaxSamples.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\rInput Format: " + 0uL + " - " + ulong.MaxValue.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				tBoxLogMaxSamples.Text = previousValue;
			}
		}

		private void tBoxLogMaxSamples_Validated(object sender, EventArgs e)
		{
			Log.MaxSamples = ulong.Parse(tBoxLogMaxSamples.Text);
		}

		private void log_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
			case "MaxSamples":
				tBoxLogMaxSamples.Text = Log.MaxSamples.ToString();
				break;
			case "Path":
				break;
			case "FileName":
				break;
			}
		}

		private void log_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke((MethodInvoker)delegate
				{
					pBarLog.Value = e.ProgressPercentage;
				}, null);
			}
			else
			{
				pBarLog.Value = e.ProgressPercentage;
			}
		}

		private void log_Stoped(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke((MethodInvoker)delegate
				{
					cBtnLogOnOff.Checked = false;
					cBtnLogOnOff.Text = "Start";
					tBoxLogMaxSamples.Enabled = true;
					btnLogBrowseFile.Enabled = true;
					Log.Stop();
					UpdateProgressBarStyle();
				}, null);
			}
			else
			{
				cBtnLogOnOff.Checked = false;
				cBtnLogOnOff.Text = "Start";
				tBoxLogMaxSamples.Enabled = true;
				btnLogBrowseFile.Enabled = true;
				Log.Stop();
				UpdateProgressBarStyle();
			}
		}

		private void RssiAnalyserForm_Load(object sender, EventArgs e)
		{
			try
			{
				var value = AppSettings.GetValue("RssiAnalyserTop");
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
				value = AppSettings.GetValue("RssiAnalyserLeft");
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
				value = AppSettings.GetValue("LogPath");
				if (value == null)
				{
					value = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					AppSettings.SetValue("LogPath", value);
				}
				Log.Path = value;
				value = AppSettings.GetValue("LogFileName");
				if (value == null)
				{
					value = "sx1276-Rssi.log";
					AppSettings.SetValue("LogFileName", value);
				}
				Log.FileName = value;
				value = AppSettings.GetValue("LogMaxSamples");
				if (value == null)
				{
					value = "1000";
					AppSettings.SetValue("LogMaxSamples", value);
				}
				Log.MaxSamples = ulong.Parse(value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		private void RssiAnalyserForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				AppSettings.SetValue("RssiAnalyserTop", Top.ToString());
				AppSettings.SetValue("RssiAnalyserLeft", Left.ToString());
				AppSettings.SetValue("LogPath", Log.Path);
				AppSettings.SetValue("LogFileName", Log.FileName);
				AppSettings.SetValue("LogMaxSamples", Log.MaxSamples.ToString());
			}
			catch (Exception)
			{
			}
		}

		private void nudRssiThresh_ValueChanged(object sender, EventArgs e)
		{
			device.SetRssiThresh(nudRssiThresh.Value);
		}

		private void graph_MouseWheel(object sender, MouseEventArgs e)
		{
			UpdateThreshold((double)nudRssiThresh.Value);
		}

		private void btnLogBrowseFile_Click(object sender, EventArgs e)
		{
			OnError(0, "-");
			try
			{
				sfLogSaveFileDlg.InitialDirectory = Log.Path;
				sfLogSaveFileDlg.FileName = Log.FileName;
				if (sfLogSaveFileDlg.ShowDialog() == DialogResult.OK)
				{
					var array = sfLogSaveFileDlg.FileName.Split('\\');
					Log.FileName = array[array.Length - 1];
					Log.Path = "";
					int i;
					for (i = 0; i < array.Length - 2; i++)
					{
						var dataLog = Log;
						dataLog.Path = dataLog.Path + array[i] + "\\";
					}
					Log.Path += array[i];
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		private void cBtnLogOnOff_CheckedChanged(object sender, EventArgs e)
		{
			OnError(0, "-");
			try
			{
				if (cBtnLogOnOff.Checked)
				{
					cBtnLogOnOff.Text = "Stop";
					tBoxLogMaxSamples.Enabled = false;
					btnLogBrowseFile.Enabled = false;
					Log.Start();
				}
				else
				{
					cBtnLogOnOff.Text = "Start";
					tBoxLogMaxSamples.Enabled = true;
					btnLogBrowseFile.Enabled = true;
					Log.Stop();
				}
			}
			catch (Exception ex)
			{
				cBtnLogOnOff.Checked = false;
				cBtnLogOnOff.Text = "Start";
				tBoxLogMaxSamples.Enabled = true;
				btnLogBrowseFile.Enabled = true;
				Log.Stop();
				OnError(1, ex.Message);
			}
			finally
			{
				UpdateProgressBarStyle();
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
			var resources = new ComponentResourceManager(typeof(RssiAnalyserForm));
			panel1 = new Panel();
			groupBox5 = new GroupBoxEx();
			btnLogBrowseFile = new Button();
			pBarLog = new ProgressBar();
			tableLayoutPanel3 = new TableLayoutPanel();
			tBoxLogMaxSamples = new TextBox();
			lblCommandsLogMaxSamples = new System.Windows.Forms.Label();
			cBtnLogOnOff = new CheckBox();
			nudRssiThresh = new NumericUpDownEx();
			label55 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			panel2 = new Panel();
			graph = new RssiGraphControl();
			sfLogSaveFileDlg = new SaveFileDialog();
			panel1.SuspendLayout();
			groupBox5.SuspendLayout();
			tableLayoutPanel3.SuspendLayout();
			((ISupportInitialize)nudRssiThresh).BeginInit();
			panel2.SuspendLayout();
			SuspendLayout();
			panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
			panel1.BackColor = Color.Black;
			panel1.BorderStyle = BorderStyle.FixedSingle;
			panel1.Controls.Add(groupBox5);
			panel1.Controls.Add(nudRssiThresh);
			panel1.Controls.Add(label55);
			panel1.Controls.Add(label3);
			panel1.Controls.Add(label1);
			panel1.Controls.Add(label9);
			panel1.Controls.Add(label7);
			panel1.Controls.Add(label5);
			panel1.Controls.Add(label4);
			panel1.Controls.Add(label2);
			panel1.Controls.Add(label8);
			panel1.Location = new Point(553, 0);
			panel1.Margin = new Padding(0);
			panel1.Name = "panel1";
			panel1.Size = new Size(223, 366);
			panel1.TabIndex = 1;
			groupBox5.BackColor = Color.Transparent;
			groupBox5.Controls.Add(btnLogBrowseFile);
			groupBox5.Controls.Add(pBarLog);
			groupBox5.Controls.Add(tableLayoutPanel3);
			groupBox5.Controls.Add(cBtnLogOnOff);
			groupBox5.ForeColor = Color.Gray;
			groupBox5.Location = new Point(9, 196);
			groupBox5.Name = "groupBox5";
			groupBox5.Size = new Size(209, 103);
			groupBox5.TabIndex = 8;
			groupBox5.TabStop = false;
			groupBox5.Text = "Log control";
			btnLogBrowseFile.BackColor = SystemColors.Control;
			btnLogBrowseFile.ForeColor = SystemColors.ControlText;
			btnLogBrowseFile.Location = new Point(15, 70);
			btnLogBrowseFile.Name = "btnLogBrowseFile";
			btnLogBrowseFile.Size = new Size(75, 23);
			btnLogBrowseFile.TabIndex = 2;
			btnLogBrowseFile.Text = "Browse...";
			btnLogBrowseFile.UseVisualStyleBackColor = false;
			btnLogBrowseFile.Click += btnLogBrowseFile_Click;
			pBarLog.Location = new Point(15, 51);
			pBarLog.Name = "pBarLog";
			pBarLog.Size = new Size(179, 13);
			pBarLog.Step = 1;
			pBarLog.Style = ProgressBarStyle.Continuous;
			pBarLog.TabIndex = 1;
			tableLayoutPanel3.AutoSize = true;
			tableLayoutPanel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			tableLayoutPanel3.ColumnCount = 2;
			tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
			tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
			tableLayoutPanel3.Controls.Add(tBoxLogMaxSamples, 1, 0);
			tableLayoutPanel3.Controls.Add(lblCommandsLogMaxSamples, 0, 0);
			tableLayoutPanel3.Location = new Point(15, 19);
			tableLayoutPanel3.Name = "tableLayoutPanel3";
			tableLayoutPanel3.RowCount = 1;
			tableLayoutPanel3.RowStyles.Add(new RowStyle());
			tableLayoutPanel3.Size = new Size(179, 26);
			tableLayoutPanel3.TabIndex = 0;
			tBoxLogMaxSamples.Location = new Point(94, 3);
			tBoxLogMaxSamples.Name = "tBoxLogMaxSamples";
			tBoxLogMaxSamples.Size = new Size(82, 20);
			tBoxLogMaxSamples.TabIndex = 1;
			tBoxLogMaxSamples.Text = "1000";
			tBoxLogMaxSamples.TextAlign = HorizontalAlignment.Center;
			tBoxLogMaxSamples.Enter += tBoxLogMaxSamples_Enter;
			tBoxLogMaxSamples.Validating += tBoxLogMaxSamples_Validating;
			tBoxLogMaxSamples.Validated += tBoxLogMaxSamples_Validated;
			lblCommandsLogMaxSamples.ForeColor = Color.Gray;
			lblCommandsLogMaxSamples.Location = new Point(3, 0);
			lblCommandsLogMaxSamples.Name = "lblCommandsLogMaxSamples";
			lblCommandsLogMaxSamples.Size = new Size(85, 23);
			lblCommandsLogMaxSamples.TabIndex = 0;
			lblCommandsLogMaxSamples.Text = "Max samples:";
			lblCommandsLogMaxSamples.TextAlign = ContentAlignment.MiddleLeft;
			cBtnLogOnOff.Appearance = Appearance.Button;
			cBtnLogOnOff.BackColor = SystemColors.Control;
			cBtnLogOnOff.CheckAlign = ContentAlignment.MiddleCenter;
			cBtnLogOnOff.ForeColor = SystemColors.ControlText;
			cBtnLogOnOff.Location = new Point(119, 70);
			cBtnLogOnOff.Name = "cBtnLogOnOff";
			cBtnLogOnOff.Size = new Size(75, 23);
			cBtnLogOnOff.TabIndex = 3;
			cBtnLogOnOff.Text = "Start";
			cBtnLogOnOff.TextAlign = ContentAlignment.MiddleCenter;
			cBtnLogOnOff.UseVisualStyleBackColor = false;
			cBtnLogOnOff.CheckedChanged += cBtnLogOnOff_CheckedChanged;
			nudRssiThresh.Anchor = AnchorStyles.None;
			nudRssiThresh.DecimalPlaces = 1;
			nudRssiThresh.Increment = new decimal(new int[4] { 5, 0, 0, 65536 });
			nudRssiThresh.Location = new Point(117, 171);
			nudRssiThresh.Margin = new Padding(0);
//			var numericUpDownEx = nudRssiThresh;
//			var bits = new int[4];
            nudRssiThresh.Maximum = new decimal(new int[4]);
			nudRssiThresh.Minimum = new decimal(new int[4] { 1275, 0, 0, -2147418112 });
			nudRssiThresh.Name = "nudRssiThresh";
			nudRssiThresh.Size = new Size(60, 20);
			nudRssiThresh.TabIndex = 7;
			nudRssiThresh.ThousandsSeparator = true;
			nudRssiThresh.Value = new decimal(new int[4] { 114, 0, 0, -2147483648 });
			nudRssiThresh.ValueChanged += nudRssiThresh_ValueChanged;
			label55.Anchor = AnchorStyles.None;
			label55.AutoSize = true;
			label55.BackColor = Color.Transparent;
			label55.ForeColor = Color.Gray;
			label55.Location = new Point(6, 171);
			label55.Margin = new Padding(0);
			label55.Name = "label55";
			label55.Size = new Size(54, 13);
			label55.TabIndex = 6;
			label55.Text = "Threshold";
			label55.TextAlign = ContentAlignment.MiddleCenter;
			label3.Anchor = AnchorStyles.None;
			label3.AutoSize = true;
			label3.ForeColor = Color.Green;
			label3.Location = new Point(6, 90);
			label3.Name = "label3";
			label3.Size = new Size(82, 13);
			label3.TabIndex = 2;
			label3.Text = "RSSI Threshold";
			label1.Anchor = AnchorStyles.None;
			label1.BackColor = Color.Green;
			label1.Location = new Point(117, 95);
			label1.Margin = new Padding(3, 3, 0, 3);
			label1.Name = "label1";
			label1.Size = new Size(25, 2);
			label1.TabIndex = 3;
			label1.Text = "label7";
			label9.Anchor = AnchorStyles.None;
			label9.AutoSize = true;
			label9.ForeColor = Color.Aqua;
			label9.Location = new Point(6, 29);
			label9.Margin = new Padding(3);
			label9.Name = "label9";
			label9.Size = new Size(69, 13);
			label9.TabIndex = 0;
			label9.Text = "RF_PA RSSI";
			label9.Visible = false;
			label7.Anchor = AnchorStyles.None;
			label7.BackColor = Color.Aqua;
			label7.Location = new Point(117, 34);
			label7.Margin = new Padding(3);
			label7.Name = "label7";
			label7.Size = new Size(25, 2);
			label7.TabIndex = 1;
			label7.Text = "label7";
			label7.Visible = false;
			label5.Anchor = AnchorStyles.None;
			label5.AutoSize = true;
			label5.ForeColor = Color.Yellow;
			label5.Location = new Point(6, 48);
			label5.Margin = new Padding(3);
			label5.Name = "label5";
			label5.Size = new Size(66, 13);
			label5.TabIndex = 0;
			label5.Text = "RF_IO RSSI";
			label5.Visible = false;
			label4.Anchor = AnchorStyles.None;
			label4.BackColor = Color.Yellow;
			label4.Location = new Point(117, 53);
			label4.Margin = new Padding(3);
			label4.Name = "label4";
			label4.Size = new Size(25, 2);
			label4.TabIndex = 1;
			label4.Text = "label7";
			label4.Visible = false;
			label2.Anchor = AnchorStyles.None;
			label2.AutoSize = true;
			label2.ForeColor = Color.Red;
			label2.Location = new Point(6, 67);
			label2.Margin = new Padding(3);
			label2.Name = "label2";
			label2.Size = new Size(32, 13);
			label2.TabIndex = 0;
			label2.Text = "RSSI";
			label8.Anchor = AnchorStyles.None;
			label8.BackColor = Color.Red;
			label8.Location = new Point(117, 72);
			label8.Margin = new Padding(3);
			label8.Name = "label8";
			label8.Size = new Size(25, 2);
			label8.TabIndex = 1;
			label8.Text = "label7";
			panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel2.BorderStyle = BorderStyle.FixedSingle;
			panel2.Controls.Add(graph);
			panel2.Location = new Point(0, 0);
			panel2.Margin = new Padding(0);
			panel2.Name = "panel2";
			panel2.Size = new Size(553, 366);
			panel2.TabIndex = 0;
			graph.Dock = DockStyle.Fill;
			graph.Location = new Point(0, 0);
			graph.Name = "graph";
			graph.Size = new Size(551, 364);
			graph.TabIndex = 0;
			sfLogSaveFileDlg.DefaultExt = "*.log";
			sfLogSaveFileDlg.Filter = "Log Files(*.log)|*.log|AllFiles(*.*)|*.*";
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(776, 366);
			Controls.Add(panel2);
			Controls.Add(panel1);
			Icon = (Icon)resources.GetObject("$this.Icon");
			Name = "RssiAnalyserForm";
			Text = "Rssi analyser";
			FormClosed += RssiAnalyserForm_FormClosed;
			Load += RssiAnalyserForm_Load;
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			groupBox5.ResumeLayout(false);
			groupBox5.PerformLayout();
			tableLayoutPanel3.ResumeLayout(false);
			tableLayoutPanel3.PerformLayout();
			((ISupportInitialize)nudRssiThresh).EndInit();
			panel2.ResumeLayout(false);
			ResumeLayout(false);
		}
	}
}
