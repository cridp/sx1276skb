using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276.General;
using SemtechLib.General;
using SemtechLib.General.Events;

namespace SemtechLib.Devices.SX1276.UI.Forms
{
	public sealed class PacketLogForm : Form
	{
		private delegate void DeviceDataChangedDelegate(object sender, PropertyChangedEventArgs e);

		private readonly int tickStart = Environment.TickCount;
        private IDevice device;
        private string previousValue;

		private IContainer components;

		private GroupBox groupBox5;

		private Button btnLogBrowseFile;

		private ProgressBar pBarLog;

		private TableLayoutPanel tableLayoutPanel3;

		private TextBox tBoxLogMaxSamples;

		private Label lblCommandsLogMaxSamples;

		private CheckBox cBtnLogOnOff;

		private Button btnClose;

		private SaveFileDialog sfLogSaveFileDlg;

        public ApplicationSettings AppSettings { get; set; }

        public IDevice Device
		{
			set
			{
				if (device != value)
				{
					device = value;
					Log.Device = device;
				}
			}
		}

		private PacketLog Log { get; } = new();

        public PacketLogForm()
		{
			InitializeComponent();
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

		private void OnError(byte status, string message)
		{
			Refresh();
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

		private void log_ProgressChanged(object sender, ProgressEventArg e)
		{
			if (InvokeRequired)
			{
				BeginInvoke((MethodInvoker)delegate
				{
					pBarLog.Value = (int)e.Progress;
				}, null);
			}
			else
			{
				pBarLog.Value = (int)e.Progress;
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

		private void PacketLogForm_Load(object sender, EventArgs e)
		{
			try
			{
				var value = AppSettings.GetValue("PacketLogTop");
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
				value = AppSettings.GetValue("PacketLogLeft");
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
				value = AppSettings.GetValue("PacketLogPath");
				if (value == null)
				{
					value = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					AppSettings.SetValue("PacketLogPath", value);
				}
				Log.Path = value;
				value = AppSettings.GetValue("PacketLogFileName");
				if (value == null)
				{
					value = "sx1276-pkt.log";
					AppSettings.SetValue("PacketLogFileName", value);
				}
				Log.FileName = value;
				value = AppSettings.GetValue("PacketLogMaxSamples");
				if (value == null)
				{
					value = "1000";
					AppSettings.SetValue("PacketLogMaxSamples", value);
				}
				Log.MaxSamples = ulong.Parse(value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		private void PacketLogForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				AppSettings.SetValue("PacketLogTop", Top.ToString());
				AppSettings.SetValue("PacketLogLeft", Left.ToString());
				AppSettings.SetValue("PacketLogPath", Log.Path);
				AppSettings.SetValue("PacketLogFileName", Log.FileName);
				AppSettings.SetValue("PacketLogMaxSamples", Log.MaxSamples.ToString());
			}
			catch (Exception)
			{
			}
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
						var packetLog = Log;
						packetLog.Path = packetLog.Path + array[i] + "\\";
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

		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
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
			var resources = new ComponentResourceManager(typeof(PacketLogForm));
			groupBox5 = new GroupBox();
			btnLogBrowseFile = new Button();
			pBarLog = new ProgressBar();
			tableLayoutPanel3 = new TableLayoutPanel();
			tBoxLogMaxSamples = new TextBox();
			lblCommandsLogMaxSamples = new Label();
			cBtnLogOnOff = new CheckBox();
			btnClose = new Button();
			sfLogSaveFileDlg = new SaveFileDialog();
			groupBox5.SuspendLayout();
			tableLayoutPanel3.SuspendLayout();
			SuspendLayout();
			groupBox5.Controls.Add(btnLogBrowseFile);
			groupBox5.Controls.Add(pBarLog);
			groupBox5.Controls.Add(tableLayoutPanel3);
			groupBox5.Controls.Add(cBtnLogOnOff);
			groupBox5.Location = new Point(12, 12);
			groupBox5.Name = "groupBox5";
			groupBox5.Size = new Size(209, 103);
			groupBox5.TabIndex = 4;
			groupBox5.TabStop = false;
			groupBox5.Text = "Log control";
			btnLogBrowseFile.Location = new Point(15, 70);
			btnLogBrowseFile.Name = "btnLogBrowseFile";
			btnLogBrowseFile.Size = new Size(75, 23);
			btnLogBrowseFile.TabIndex = 2;
			btnLogBrowseFile.Text = "Browse...";
			btnLogBrowseFile.UseVisualStyleBackColor = true;
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
			lblCommandsLogMaxSamples.Location = new Point(3, 0);
			lblCommandsLogMaxSamples.Name = "lblCommandsLogMaxSamples";
			lblCommandsLogMaxSamples.Size = new Size(85, 23);
			lblCommandsLogMaxSamples.TabIndex = 0;
			lblCommandsLogMaxSamples.Text = "Max samples:";
			lblCommandsLogMaxSamples.TextAlign = ContentAlignment.MiddleLeft;
			cBtnLogOnOff.Appearance = Appearance.Button;
			cBtnLogOnOff.CheckAlign = ContentAlignment.MiddleCenter;
			cBtnLogOnOff.Location = new Point(119, 70);
			cBtnLogOnOff.Name = "cBtnLogOnOff";
			cBtnLogOnOff.Size = new Size(75, 23);
			cBtnLogOnOff.TabIndex = 3;
			cBtnLogOnOff.Text = "Start";
			cBtnLogOnOff.TextAlign = ContentAlignment.MiddleCenter;
			cBtnLogOnOff.UseVisualStyleBackColor = true;
			cBtnLogOnOff.CheckedChanged += cBtnLogOnOff_CheckedChanged;
			btnClose.Location = new Point(79, 121);
			btnClose.Name = "btnClose";
			btnClose.Size = new Size(75, 23);
			btnClose.TabIndex = 2;
			btnClose.Text = "Close";
			btnClose.UseVisualStyleBackColor = true;
			btnClose.Click += btnClose_Click;
			sfLogSaveFileDlg.DefaultExt = "*.log";
			sfLogSaveFileDlg.Filter = "Log Files(*.log)|*.log|AllFiles(*.*)|*.*";
			AcceptButton = btnClose;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(233, 155);
			Controls.Add(btnClose);
			Controls.Add(groupBox5);
			DoubleBuffered = true;
			Icon = (Icon)resources.GetObject("$this.Icon");
			KeyPreview = true;
			MaximizeBox = false;
			Name = "PacketLogForm";
			Opacity = 0.9;
			Text = "Packet Log";
			FormClosed += PacketLogForm_FormClosed;
			Load += PacketLogForm_Load;
			groupBox5.ResumeLayout(false);
			groupBox5.PerformLayout();
			tableLayoutPanel3.ResumeLayout(false);
			tableLayoutPanel3.PerformLayout();
			ResumeLayout(false);
		}
	}
}
