using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.General;

namespace SemtechLib.Devices.SX1276LR.UI.Forms
{
	public sealed class PacketLogForm : Form
	{
		private delegate void DeviceDataChangedDelegate(object sender, PropertyChangedEventArgs e);

        private IDevice device;

		private IContainer components;

		private Button btnLogBrowseFile;

		private Button btnClose;

		private SaveFileDialog sfLogSaveFileDlg;

		private Label label1;

		private Label lblFileName;

		private Label label3;

		private RadioButton rBtnFileModeAppendOn;

		private RadioButton rBtnFileModeAppendOff;

		private Panel panel1;

		private Panel panel13;

		private RadioButton rBtnLogOff;

		private RadioButton rBtnLogOn;

		private Label label2;

        public ApplicationSettings AppSettings { get; set; }

        public IDevice Device
		{
			set
			{
				if (device != value)
				{
					device = value;
					((SX1276LR)device).PacketHandlerLog.PropertyChanged += log_PropertyChanged;
					rBtnLogOn.Checked = ((SX1276LR)device).PacketHandlerLog.Enabled;
					rBtnLogOff.Checked = !((SX1276LR)device).PacketHandlerLog.Enabled;
					rBtnFileModeAppendOn.Checked = ((SX1276LR)device).PacketHandlerLog.IsAppend;
					rBtnFileModeAppendOff.Checked = !((SX1276LR)device).PacketHandlerLog.IsAppend;
					lblFileName.Text = ((SX1276LR)device).PacketHandlerLog.FileName;
				}
			}
		}

		public PacketLogForm()
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

		private void OnError(byte status, string message)
		{
			Refresh();
		}

		private void log_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
			case "FileName":
				lblFileName.Text = ((SX1276LR)device).PacketHandlerLog.FileName;
				break;
			case "Path":
				break;
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
				rBtnLogOn.Checked = ((SX1276LR)device).PacketHandlerLog.Enabled;
				rBtnLogOff.Checked = !((SX1276LR)device).PacketHandlerLog.Enabled;
				rBtnFileModeAppendOn.Checked = ((SX1276LR)device).PacketHandlerLog.IsAppend;
				rBtnFileModeAppendOff.Checked = !((SX1276LR)device).PacketHandlerLog.IsAppend;
				lblFileName.Text = ((SX1276LR)device).PacketHandlerLog.FileName;
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
				AppSettings.SetValue("PacketLogPath", ((SX1276LR)device).PacketHandlerLog.Path);
				AppSettings.SetValue("PacketLogFileName", ((SX1276LR)device).PacketHandlerLog.FileName);
				AppSettings.SetValue("PacketLogMaxSamples", ((SX1276LR)device).PacketHandlerLog.MaxSamples.ToString());
				AppSettings.SetValue("PacketLogIsAppend", ((SX1276LR)device).PacketHandlerLog.IsAppend.ToString());
				AppSettings.SetValue("PacketLogEnabled", ((SX1276LR)device).PacketHandlerLog.Enabled.ToString());
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
				sfLogSaveFileDlg.InitialDirectory = ((SX1276LR)device).PacketHandlerLog.Path;
				sfLogSaveFileDlg.FileName = ((SX1276LR)device).PacketHandlerLog.FileName;
				if (sfLogSaveFileDlg.ShowDialog() == DialogResult.OK)
				{
					var array = sfLogSaveFileDlg.FileName.Split('\\');
					((SX1276LR)device).PacketHandlerLog.FileName = array[array.Length - 1];
					((SX1276LR)device).PacketHandlerLog.Path = "";
					int i;
					for (i = 0; i < array.Length - 2; i++)
					{
						var packetHandlerLog = ((SX1276LR)device).PacketHandlerLog;
						packetHandlerLog.Path = packetHandlerLog.Path + array[i] + "\\";
					}
					((SX1276LR)device).PacketHandlerLog.Path += array[i];
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void rBtnFileModeAppendOn_CheckedChanged(object sender, EventArgs e)
		{
			((SX1276LR)device).PacketHandlerLog.IsAppend = rBtnFileModeAppendOn.Checked;
		}

		private void rBtnFileModeAppendOff_CheckedChanged(object sender, EventArgs e)
		{
			((SX1276LR)device).PacketHandlerLog.IsAppend = rBtnFileModeAppendOn.Checked;
		}

		private void rBtnLogOn_CheckedChanged(object sender, EventArgs e)
		{
			((SX1276LR)device).PacketHandlerLog.Enabled = rBtnLogOn.Checked;
		}

		private void rBtnLogOff_CheckedChanged(object sender, EventArgs e)
		{
			((SX1276LR)device).PacketHandlerLog.Enabled = rBtnLogOn.Checked;
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
			btnLogBrowseFile = new Button();
			btnClose = new Button();
			sfLogSaveFileDlg = new SaveFileDialog();
			label1 = new Label();
			lblFileName = new Label();
			label3 = new Label();
			rBtnFileModeAppendOn = new RadioButton();
			rBtnFileModeAppendOff = new RadioButton();
			panel1 = new Panel();
			panel13 = new Panel();
			rBtnLogOff = new RadioButton();
			rBtnLogOn = new RadioButton();
			label2 = new Label();
			panel1.SuspendLayout();
			panel13.SuspendLayout();
			SuspendLayout();
			btnLogBrowseFile.Location = new Point(318, 34);
			btnLogBrowseFile.Name = "btnLogBrowseFile";
			btnLogBrowseFile.Size = new Size(75, 23);
			btnLogBrowseFile.TabIndex = 4;
			btnLogBrowseFile.Text = "Browse...";
			btnLogBrowseFile.UseVisualStyleBackColor = true;
			btnLogBrowseFile.Click += btnLogBrowseFile_Click;
			btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btnClose.Location = new Point(166, 117);
			btnClose.Name = "btnClose";
			btnClose.Size = new Size(75, 23);
			btnClose.TabIndex = 7;
			btnClose.Text = "OK";
			btnClose.UseVisualStyleBackColor = true;
			btnClose.Click += btnClose_Click;
			sfLogSaveFileDlg.DefaultExt = "*.log";
			sfLogSaveFileDlg.Filter = "Log Files(*.log)|*.log|AllFiles(*.*)|*.*";
			label1.AutoSize = true;
			label1.Location = new Point(11, 39);
			label1.Name = "label1";
			label1.Size = new Size(26, 13);
			label1.TabIndex = 2;
			label1.Text = "File:";
			lblFileName.BorderStyle = BorderStyle.Fixed3D;
			lblFileName.Location = new Point(66, 35);
			lblFileName.Name = "lblFileName";
			lblFileName.Size = new Size(246, 20);
			lblFileName.TabIndex = 3;
			lblFileName.Text = "-";
			lblFileName.TextAlign = ContentAlignment.MiddleLeft;
			label3.AutoSize = true;
			label3.Location = new Point(11, 75);
			label3.Name = "label3";
			label3.Size = new Size(37, 13);
			label3.TabIndex = 5;
			label3.Text = "Mode:";
			rBtnFileModeAppendOn.AutoSize = true;
			rBtnFileModeAppendOn.Checked = true;
			rBtnFileModeAppendOn.Location = new Point(3, 3);
			rBtnFileModeAppendOn.Name = "rBtnFileModeAppendOn";
			rBtnFileModeAppendOn.Size = new Size(126, 17);
			rBtnFileModeAppendOn.TabIndex = 0;
			rBtnFileModeAppendOn.TabStop = true;
			rBtnFileModeAppendOn.Text = "Append to current file";
			rBtnFileModeAppendOn.UseVisualStyleBackColor = true;
			rBtnFileModeAppendOn.CheckedChanged += rBtnFileModeAppendOn_CheckedChanged;
			rBtnFileModeAppendOff.AutoSize = true;
			rBtnFileModeAppendOff.Location = new Point(3, 26);
			rBtnFileModeAppendOff.Name = "rBtnFileModeAppendOff";
			rBtnFileModeAppendOff.Size = new Size(144, 17);
			rBtnFileModeAppendOff.TabIndex = 1;
			rBtnFileModeAppendOff.Text = "Create new file each time";
			rBtnFileModeAppendOff.UseVisualStyleBackColor = true;
			rBtnFileModeAppendOff.CheckedChanged += rBtnFileModeAppendOff_CheckedChanged;
			panel1.AutoSize = true;
			panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel1.Controls.Add(rBtnFileModeAppendOn);
			panel1.Controls.Add(rBtnFileModeAppendOff);
			panel1.Location = new Point(66, 58);
			panel1.Name = "panel1";
			panel1.Size = new Size(150, 46);
			panel1.TabIndex = 6;
			panel13.AutoSize = true;
			panel13.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel13.Controls.Add(rBtnLogOff);
			panel13.Controls.Add(rBtnLogOn);
			panel13.Location = new Point(66, 12);
			panel13.Name = "panel13";
			panel13.Size = new Size(102, 20);
			panel13.TabIndex = 1;
			rBtnLogOff.AutoSize = true;
			rBtnLogOff.Checked = true;
			rBtnLogOff.Location = new Point(54, 3);
			rBtnLogOff.Margin = new Padding(3, 0, 3, 0);
			rBtnLogOff.Name = "rBtnLogOff";
			rBtnLogOff.Size = new Size(45, 17);
			rBtnLogOff.TabIndex = 1;
			rBtnLogOff.TabStop = true;
			rBtnLogOff.Text = "OFF";
			rBtnLogOff.UseVisualStyleBackColor = true;
			rBtnLogOff.CheckedChanged += rBtnLogOff_CheckedChanged;
			rBtnLogOn.AutoSize = true;
			rBtnLogOn.Location = new Point(3, 3);
			rBtnLogOn.Margin = new Padding(3, 0, 3, 0);
			rBtnLogOn.Name = "rBtnLogOn";
			rBtnLogOn.Size = new Size(41, 17);
			rBtnLogOn.TabIndex = 0;
			rBtnLogOn.Text = "ON";
			rBtnLogOn.UseVisualStyleBackColor = true;
			rBtnLogOn.CheckedChanged += rBtnLogOn_CheckedChanged;
			label2.AutoSize = true;
			label2.Location = new Point(11, 16);
			label2.Name = "label2";
			label2.Size = new Size(28, 13);
			label2.TabIndex = 0;
			label2.Text = "Log:";
			AcceptButton = btnClose;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(406, 152);
			Controls.Add(panel13);
			Controls.Add(panel1);
			Controls.Add(lblFileName);
			Controls.Add(label3);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(btnLogBrowseFile);
			Controls.Add(btnClose);
			DoubleBuffered = true;
			Icon = (Icon)resources.GetObject("$this.Icon");
			KeyPreview = true;
			MaximizeBox = false;
			Name = "PacketLogForm";
			Opacity = 0.9;
			Text = "Packet log settings";
			FormClosed += PacketLogForm_FormClosed;
			Load += PacketLogForm_Load;
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			panel13.ResumeLayout(false);
			panel13.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
