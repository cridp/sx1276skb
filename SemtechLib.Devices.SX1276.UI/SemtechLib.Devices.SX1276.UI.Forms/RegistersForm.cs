using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276.UI.Controls;
using SemtechLib.General;

namespace SemtechLib.Devices.SX1276.UI.Forms
{
	public sealed class RegistersForm : Form, INotifyPropertyChanged
	{
        private IDevice device;

		private bool registersFormEnabled;

		private IContainer components;

		private RegisterTableControl registerTableControl1;

		private StatusStrip statusStrip1;

		private ToolStripStatusLabel ssLblStatus;

		private Panel panel1;

        public ApplicationSettings AppSettings { get; set; }

        public IDevice Device
		{
			set
			{
				try
				{
					device = value;
					device.PropertyChanged += device_PropertyChanged;
					registerTableControl1.Registers = device.Registers;
					device.ReadRegisters();
				}
				catch (Exception ex)
				{
					OnError(1, ex.Message);
				}
			}
		}

		public bool RegistersFormEnabled
		{
			get => registersFormEnabled;
			set
			{
				registersFormEnabled = value;
				panel1.Enabled = value;
				OnPropertyChanged("RegistersFormEnabled");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public RegistersForm()
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
			if (status != 0)
			{
				ssLblStatus.Text = "ERROR: " + message;
			}
			else
			{
				ssLblStatus.Text = message;
			}
			Refresh();
		}

		private void RegistersForm_Load(object sender, EventArgs e)
		{
			var value = AppSettings.GetValue("RegistersTop");
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
			value = AppSettings.GetValue("RegistersLeft");
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
			if (IsFormLocatedInScreen(this, allScreens)) return;
			Top = allScreens[0].WorkingArea.Top;
			Left = allScreens[0].WorkingArea.Left;
		}

		private void RegistersForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				AppSettings.SetValue("RegistersTop", Top.ToString());
				AppSettings.SetValue("RegistersLeft", Left.ToString());
			}
			catch (Exception)
			{
			}
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
			case "Registers":
			case "Version":
				registerTableControl1.Registers = device.Registers;
				break;
			}
		}

		private void OnPropertyChanged(string propName)
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
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
			var resources = new ComponentResourceManager(typeof(RegistersForm));
			statusStrip1 = new StatusStrip();
			ssLblStatus = new ToolStripStatusLabel();
			panel1 = new Panel();
			registerTableControl1 = new RegisterTableControl();
			statusStrip1.SuspendLayout();
			panel1.SuspendLayout();
			SuspendLayout();
			statusStrip1.Items.AddRange(new ToolStripItem[1] { ssLblStatus });
			statusStrip1.Location = new Point(0, 244);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Size = new Size(292, 22);
			statusStrip1.TabIndex = 1;
			statusStrip1.Text = "statusStrip1";
			ssLblStatus.Name = "ssLblStatus";
			ssLblStatus.Size = new Size(11, 17);
			ssLblStatus.Text = "-";
			panel1.AutoSize = true;
			panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel1.Controls.Add(registerTableControl1);
			panel1.Dock = DockStyle.Fill;
			panel1.Location = new Point(0, 0);
			panel1.Name = "panel1";
			panel1.Size = new Size(292, 244);
			panel1.TabIndex = 0;
			registerTableControl1.AutoSize = true;
			registerTableControl1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			registerTableControl1.Location = new Point(3, 3);
			registerTableControl1.Name = "registerTableControl1";
			registerTableControl1.Size = new Size(208, 25);
			registerTableControl1.Split = 4u;
			registerTableControl1.TabIndex = 0;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			ClientSize = new Size(292, 266);
			Controls.Add(panel1);
			Controls.Add(statusStrip1);
			DoubleBuffered = true;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Icon = (Icon)resources.GetObject("$this.Icon");
			KeyPreview = true;
			MaximizeBox = false;
			Name = "RegistersForm";
			Text = "SX1276 Registers display";
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
