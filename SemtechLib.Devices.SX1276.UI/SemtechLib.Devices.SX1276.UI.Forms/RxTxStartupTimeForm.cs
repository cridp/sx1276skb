using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SemtechLib.Controls;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.General;

namespace SemtechLib.Devices.SX1276.UI.Forms
{
	public sealed class RxTxStartupTimeForm : Form
	{
		private delegate void DeviceDataChangedDelegate(object sender, PropertyChangedEventArgs e);

		private const double Tana = 2E-05;

		private string unit = "";

		private double txTsTr;

		private double rxTsRe;

		private double rxTsRssi;

		private double rxTsReTsRssi;
        private SX1276 device;

		private double Tcf;

		private IContainer components;

		private GroupBoxEx gBoxTxStartupTime;

		private Label lblTsTrUnit;

		private Label lblTsTrValue;

		private Label lblTsTr;

		private GroupBoxEx gBoxRxStartupTime;

		private Label label4;

		private Label label3;

		private Label lblAgcOnValue;

		private Label lblAfcOnValue;

		private Label lblAfcOn;

		private Label lblAgcOn;

		private Label lblTsReUnit;

		private Label lblTsReValue;

		private Label lblTsRe;

		private Label label9;

		private Label label6;

		private Label lblTsReTsRssiUnit;

		private Label lblTsRssiUnit;

		private Label label8;

		private Label label5;

		private Label lblTsReTsRssiValue;

		private Label lblTsRssiValue;

		private Label label7;

		private Label label2;

		private Label label10;

		private Label label1;

		private Panel pnlHorizontalSeparator;

		private double TxTsTr
		{
			get => txTsTr;
			set
			{
				txTsTr = value;
				unit = "s";
				lblTsTrValue.Text = EngineeringNotation.ToString(txTsTr, ref unit);
				lblTsTrUnit.Text = unit;
			}
		}

		private double RxTsRe
		{
			get => rxTsRe;
			set
			{
				rxTsRe = value;
				unit = "s";
				lblTsReValue.Text = EngineeringNotation.ToString(rxTsRe, ref unit);
				lblTsReUnit.Text = unit;
			}
		}

		private double RxTsRssi
		{
			get => rxTsRssi;
			set
			{
				rxTsRssi = value;
				unit = "s";
				lblTsRssiValue.Text = EngineeringNotation.ToString(rxTsRssi, ref unit);
				lblTsRssiUnit.Text = unit;
			}
		}

		private double RxTsReTsRssi
		{
			get => rxTsReTsRssi;
			set
			{
				rxTsReTsRssi = value;
				unit = "s";
				lblTsReTsRssiValue.Text = EngineeringNotation.ToString(rxTsReTsRssi, ref unit);
				lblTsReTsRssiUnit.Text = unit;
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
					ComputeStartupTimmings();
				}
			}
		}

		public RxTxStartupTimeForm()
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

		private void ComputeStartupTimmings()
		{
			if (device.ModulationType == ModulationTypeEnum.OOK)
			{
				Tcf = 34.0 / (4.0 * (double)device.RxBw);
				TxTsTr = 5E-06 + 0.5 * (double)device.Tbit;
			}
			else
			{
				Tcf = 21.0 / (4.0 * (double)device.RxBw);
				var num = 4E-05;
				switch ((byte)device.PaRamp)
				{
				case 0:
					num = 0.0034;
					break;
				case 1:
					num = 0.002;
					break;
				case 2:
					num = 0.001;
					break;
				case 3:
					num = 0.0005;
					break;
				case 4:
					num = 0.00025;
					break;
				case 5:
					num = 0.000125;
					break;
				case 6:
					num = 0.0001;
					break;
				case 7:
					num = 6.2E-05;
					break;
				case 8:
					num = 5E-05;
					break;
				case 9:
					num = 4E-05;
					break;
				case 10:
					num = 3.1E-05;
					break;
				case 11:
					num = 2.5E-05;
					break;
				case 12:
					num = 2E-05;
					break;
				case 13:
					num = 1.5E-05;
					break;
				case 14:
					num = 1.2E-05;
					break;
				case 15:
					num = 1E-05;
					break;
				}
				TxTsTr = 5E-06 + 1.25 * num + 0.5 * (double)device.Tbit;
			}
			lblAgcOnValue.Text = (device.AgcAutoOn ? "ON" : "OFF");
			lblAfcOnValue.Text = (device.AfcAutoOn ? "ON" : "OFF");
			if (!device.AfcAutoOn)
			{
				var mant = 0;
				var exp = 0;
				SX1276.ComputeRxBwMantExp(device.FrequencyXo, device.ModulationType, device.RxBw, ref mant, ref exp);
				var num2 = 2.0 * (double)device.FrequencyXo / mant;
				var num3 = 1.0 / num2;
				var num4 = 1.0 / (4.0 * (double)device.RxBw);
				if (device.RxBw <= 160000m)
				{
					RxTsRe = 2E-05 + 12.0 * num3 + 24.0 * num4;
				}
				else
				{
					RxTsRe = 2E-05 + 76.0 * num3 + 24.0 * num4;
				}
				RxTsRssi = (double)device.RssiSmoothing * num4;
				RxTsReTsRssi = RxTsRe + RxTsRssi;
			}
			else
			{
				var mant2 = 0;
				var exp2 = 0;
				SX1276.ComputeRxBwMantExp(device.FrequencyXo, device.ModulationType, device.AfcRxBw, ref mant2, ref exp2);
				var num5 = 2.0 * (double)device.FrequencyXo / mant2;
				var num6 = 1.0 / num5;
				var num7 = 1.0 / (4.0 * (double)device.AfcRxBw);
				if (device.AfcRxBw <= 160000m)
				{
					RxTsRe = 2E-05 + 12.0 * num6 + 24.0 * num7;
				}
				else
				{
					RxTsRe = 2E-05 + 76.0 * num6 + 24.0 * num7;
				}
				RxTsRssi = (double)device.RssiSmoothing * num7;
				RxTsReTsRssi = RxTsRe + RxTsRssi;
			}
		}

		private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
			case "FrequencyXo":
			case "Bitrate":
			case "PaRamp":
			case "RxBw":
			case "AfcRxBw":
			case "DccFastInitOn":
			case "DccForceOn":
			case "AgcAutoOn":
			case "AfcAutoOn":
				ComputeStartupTimmings();
				break;
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

		private void RxTxStartupTimeForm_Load(object sender, EventArgs e)
		{
			try
			{
				var value = AppSettings.GetValue("RxTxStartupTimeTop");
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
				value = AppSettings.GetValue("RxTxStartupTimeLeft");
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
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		private void RxTxStartupTimeForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				AppSettings.SetValue("RxTxStartupTimeTop", Top.ToString());
				AppSettings.SetValue("RxTxStartupTimeLeft", Left.ToString());
			}
			catch (Exception)
			{
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
			var resources = new ComponentResourceManager(typeof(RxTxStartupTimeForm));
			label4 = new Label();
			label3 = new Label();
			gBoxRxStartupTime = new GroupBoxEx();
			lblTsReUnit = new Label();
			lblTsReValue = new Label();
			lblTsRe = new Label();
			lblAgcOnValue = new Label();
			lblAfcOnValue = new Label();
			lblAfcOn = new Label();
			lblAgcOn = new Label();
			gBoxTxStartupTime = new GroupBoxEx();
			lblTsTrUnit = new Label();
			lblTsTrValue = new Label();
			lblTsTr = new Label();
			label1 = new Label();
			lblTsRssiValue = new Label();
			lblTsRssiUnit = new Label();
			label2 = new Label();
			label5 = new Label();
			label6 = new Label();
			label7 = new Label();
			label8 = new Label();
			label9 = new Label();
			label10 = new Label();
			lblTsReTsRssiValue = new Label();
			lblTsReTsRssiUnit = new Label();
			pnlHorizontalSeparator = new Panel();
			gBoxRxStartupTime.SuspendLayout();
			gBoxTxStartupTime.SuspendLayout();
			SuspendLayout();
			label4.AutoSize = true;
			label4.Location = new Point(60, 264);
			label4.Name = "label4";
			label4.Size = new Size(191, 13);
			label4.TabIndex = 6;
			label4.Text = "See drawings section 4.2 of datasheet.";
			label4.Visible = false;
			label3.AutoSize = true;
			label3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label3.Location = new Point(16, 264);
			label3.Name = "label3";
			label3.Size = new Size(38, 13);
			label3.TabIndex = 5;
			label3.Text = "Note:";
			label3.Visible = false;
			gBoxRxStartupTime.Controls.Add(pnlHorizontalSeparator);
			gBoxRxStartupTime.Controls.Add(label9);
			gBoxRxStartupTime.Controls.Add(label6);
			gBoxRxStartupTime.Controls.Add(lblTsReTsRssiUnit);
			gBoxRxStartupTime.Controls.Add(lblTsRssiUnit);
			gBoxRxStartupTime.Controls.Add(lblTsReUnit);
			gBoxRxStartupTime.Controls.Add(label8);
			gBoxRxStartupTime.Controls.Add(label5);
			gBoxRxStartupTime.Controls.Add(lblTsReTsRssiValue);
			gBoxRxStartupTime.Controls.Add(lblTsRssiValue);
			gBoxRxStartupTime.Controls.Add(lblTsReValue);
			gBoxRxStartupTime.Controls.Add(label7);
			gBoxRxStartupTime.Controls.Add(label2);
			gBoxRxStartupTime.Controls.Add(label10);
			gBoxRxStartupTime.Controls.Add(label1);
			gBoxRxStartupTime.Controls.Add(lblTsRe);
			gBoxRxStartupTime.Controls.Add(lblAgcOnValue);
			gBoxRxStartupTime.Controls.Add(lblAfcOnValue);
			gBoxRxStartupTime.Controls.Add(lblAfcOn);
			gBoxRxStartupTime.Controls.Add(lblAgcOn);
			gBoxRxStartupTime.Location = new Point(12, 81);
			gBoxRxStartupTime.Name = "gBoxRxStartupTime";
			gBoxRxStartupTime.Size = new Size(324, 180);
			gBoxRxStartupTime.TabIndex = 4;
			gBoxRxStartupTime.TabStop = false;
			gBoxRxStartupTime.Text = "Rx startup time";
			lblTsReUnit.AutoSize = true;
			lblTsReUnit.Location = new Point(275, 68);
			lblTsReUnit.Margin = new Padding(1);
			lblTsReUnit.MinimumSize = new Size(40, 20);
			lblTsReUnit.Name = "lblTsReUnit";
			lblTsReUnit.Size = new Size(40, 20);
			lblTsReUnit.TabIndex = 29;
			lblTsReUnit.Text = "μs";
			lblTsReUnit.TextAlign = ContentAlignment.MiddleLeft;
			lblTsReValue.AutoSize = true;
			lblTsReValue.Location = new Point(193, 68);
			lblTsReValue.Margin = new Padding(1);
			lblTsReValue.MinimumSize = new Size(80, 20);
			lblTsReValue.Name = "lblTsReValue";
			lblTsReValue.Size = new Size(80, 20);
			lblTsReValue.TabIndex = 28;
			lblTsReValue.Text = "0.000";
			lblTsReValue.TextAlign = ContentAlignment.MiddleRight;
			lblTsRe.AutoSize = true;
			lblTsRe.Location = new Point(4, 68);
			lblTsRe.Margin = new Padding(1);
			lblTsRe.MinimumSize = new Size(0, 20);
			lblTsRe.Name = "lblTsRe";
			lblTsRe.Size = new Size(45, 20);
			lblTsRe.TabIndex = 27;
			lblTsRe.Text = "TS_RE:";
			lblTsRe.TextAlign = ContentAlignment.MiddleLeft;
			lblAgcOnValue.AutoSize = true;
			lblAgcOnValue.Location = new Point(193, 17);
			lblAgcOnValue.Margin = new Padding(1);
			lblAgcOnValue.MinimumSize = new Size(80, 20);
			lblAgcOnValue.Name = "lblAgcOnValue";
			lblAgcOnValue.Size = new Size(80, 20);
			lblAgcOnValue.TabIndex = 25;
			lblAgcOnValue.Text = "OFF";
			lblAgcOnValue.TextAlign = ContentAlignment.MiddleRight;
			lblAfcOnValue.AutoSize = true;
			lblAfcOnValue.Location = new Point(193, 39);
			lblAfcOnValue.Margin = new Padding(1);
			lblAfcOnValue.MinimumSize = new Size(80, 20);
			lblAfcOnValue.Name = "lblAfcOnValue";
			lblAfcOnValue.Size = new Size(80, 20);
			lblAfcOnValue.TabIndex = 25;
			lblAfcOnValue.Text = "OFF";
			lblAfcOnValue.TextAlign = ContentAlignment.MiddleRight;
			lblAfcOn.AutoSize = true;
			lblAfcOn.Location = new Point(4, 39);
			lblAfcOn.Margin = new Padding(1);
			lblAfcOn.MinimumSize = new Size(0, 20);
			lblAfcOn.Name = "lblAfcOn";
			lblAfcOn.Size = new Size(30, 20);
			lblAfcOn.TabIndex = 24;
			lblAfcOn.Text = "AFC:";
			lblAfcOn.TextAlign = ContentAlignment.MiddleLeft;
			lblAgcOn.AutoSize = true;
			lblAgcOn.Location = new Point(4, 17);
			lblAgcOn.Margin = new Padding(1);
			lblAgcOn.MinimumSize = new Size(0, 20);
			lblAgcOn.Name = "lblAgcOn";
			lblAgcOn.Size = new Size(32, 20);
			lblAgcOn.TabIndex = 24;
			lblAgcOn.Text = "AGC:";
			lblAgcOn.TextAlign = ContentAlignment.MiddleLeft;
			gBoxTxStartupTime.Controls.Add(lblTsTrUnit);
			gBoxTxStartupTime.Controls.Add(lblTsTrValue);
			gBoxTxStartupTime.Controls.Add(lblTsTr);
			gBoxTxStartupTime.Location = new Point(12, 12);
			gBoxTxStartupTime.Name = "gBoxTxStartupTime";
			gBoxTxStartupTime.Size = new Size(324, 63);
			gBoxTxStartupTime.TabIndex = 4;
			gBoxTxStartupTime.TabStop = false;
			gBoxTxStartupTime.Text = "Tx startup time";
			lblTsTrUnit.AutoSize = true;
			lblTsTrUnit.Location = new Point(275, 27);
			lblTsTrUnit.Margin = new Padding(1);
			lblTsTrUnit.MinimumSize = new Size(40, 20);
			lblTsTrUnit.Name = "lblTsTrUnit";
			lblTsTrUnit.Size = new Size(40, 20);
			lblTsTrUnit.TabIndex = 26;
			lblTsTrUnit.Text = "μs";
			lblTsTrUnit.TextAlign = ContentAlignment.MiddleLeft;
			lblTsTrValue.AutoSize = true;
			lblTsTrValue.Location = new Point(193, 27);
			lblTsTrValue.Margin = new Padding(1);
			lblTsTrValue.MinimumSize = new Size(80, 20);
			lblTsTrValue.Name = "lblTsTrValue";
			lblTsTrValue.Size = new Size(80, 20);
			lblTsTrValue.TabIndex = 25;
			lblTsTrValue.Text = "0.000";
			lblTsTrValue.TextAlign = ContentAlignment.MiddleRight;
			lblTsTr.AutoSize = true;
			lblTsTr.Location = new Point(4, 27);
			lblTsTr.Margin = new Padding(1);
			lblTsTr.MinimumSize = new Size(0, 20);
			lblTsTr.Name = "lblTsTr";
			lblTsTr.Size = new Size(45, 20);
			lblTsTr.TabIndex = 24;
			lblTsTr.Text = "TS_TR:";
			lblTsTr.TextAlign = ContentAlignment.MiddleLeft;
			label1.AutoSize = true;
			label1.Location = new Point(4, 90);
			label1.Margin = new Padding(1);
			label1.MinimumSize = new Size(0, 20);
			label1.Name = "label1";
			label1.Size = new Size(55, 20);
			label1.TabIndex = 27;
			label1.Text = "TS_RSSI:";
			label1.TextAlign = ContentAlignment.MiddleLeft;
			lblTsRssiValue.AutoSize = true;
			lblTsRssiValue.Location = new Point(193, 90);
			lblTsRssiValue.Margin = new Padding(1);
			lblTsRssiValue.MinimumSize = new Size(80, 20);
			lblTsRssiValue.Name = "lblTsRssiValue";
			lblTsRssiValue.Size = new Size(80, 20);
			lblTsRssiValue.TabIndex = 28;
			lblTsRssiValue.Text = "0.000";
			lblTsRssiValue.TextAlign = ContentAlignment.MiddleRight;
			lblTsRssiUnit.AutoSize = true;
			lblTsRssiUnit.Location = new Point(275, 90);
			lblTsRssiUnit.Margin = new Padding(1);
			lblTsRssiUnit.MinimumSize = new Size(40, 20);
			lblTsRssiUnit.Name = "lblTsRssiUnit";
			lblTsRssiUnit.Size = new Size(40, 20);
			lblTsRssiUnit.TabIndex = 29;
			lblTsRssiUnit.Text = "μs";
			lblTsRssiUnit.TextAlign = ContentAlignment.MiddleLeft;
			label2.AutoSize = true;
			label2.Location = new Point(4, 134);
			label2.Margin = new Padding(1);
			label2.MinimumSize = new Size(0, 20);
			label2.Name = "label2";
			label2.Size = new Size(43, 20);
			label2.TabIndex = 27;
			label2.Text = "TS_FS:";
			label2.TextAlign = ContentAlignment.MiddleLeft;
			label5.AutoSize = true;
			label5.Location = new Point(193, 134);
			label5.Margin = new Padding(1);
			label5.MinimumSize = new Size(80, 20);
			label5.Name = "label5";
			label5.Size = new Size(80, 20);
			label5.TabIndex = 28;
			label5.Text = "60.000";
			label5.TextAlign = ContentAlignment.MiddleRight;
			label6.AutoSize = true;
			label6.Location = new Point(275, 134);
			label6.Margin = new Padding(1);
			label6.MinimumSize = new Size(40, 20);
			label6.Name = "label6";
			label6.Size = new Size(40, 20);
			label6.TabIndex = 29;
			label6.Text = "μs";
			label6.TextAlign = ContentAlignment.MiddleLeft;
			label7.AutoSize = true;
			label7.Location = new Point(4, 156);
			label7.Margin = new Padding(1);
			label7.MinimumSize = new Size(0, 20);
			label7.Name = "label7";
			label7.Size = new Size(150, 20);
			label7.TabIndex = 27;
			label7.Text = "TS_OSC (depends on crystal):";
			label7.TextAlign = ContentAlignment.MiddleLeft;
			label8.AutoSize = true;
			label8.Location = new Point(193, 156);
			label8.Margin = new Padding(1);
			label8.MinimumSize = new Size(80, 20);
			label8.Name = "label8";
			label8.Size = new Size(80, 20);
			label8.TabIndex = 28;
			label8.Text = "250.000";
			label8.TextAlign = ContentAlignment.MiddleRight;
			label9.AutoSize = true;
			label9.Location = new Point(275, 156);
			label9.Margin = new Padding(1);
			label9.MinimumSize = new Size(40, 20);
			label9.Name = "label9";
			label9.Size = new Size(40, 20);
			label9.TabIndex = 29;
			label9.Text = "μs";
			label9.TextAlign = ContentAlignment.MiddleLeft;
			label10.AutoSize = true;
			label10.Location = new Point(4, 112);
			label10.Margin = new Padding(1);
			label10.MinimumSize = new Size(0, 20);
			label10.Name = "label10";
			label10.Size = new Size(139, 20);
			label10.TabIndex = 27;
			label10.Text = "TS_RE + TS_RSSI (+/-5%):";
			label10.TextAlign = ContentAlignment.MiddleLeft;
			lblTsReTsRssiValue.AutoSize = true;
			lblTsReTsRssiValue.Location = new Point(193, 112);
			lblTsReTsRssiValue.Margin = new Padding(1);
			lblTsReTsRssiValue.MinimumSize = new Size(80, 20);
			lblTsReTsRssiValue.Name = "lblTsReTsRssiValue";
			lblTsReTsRssiValue.Size = new Size(80, 20);
			lblTsReTsRssiValue.TabIndex = 28;
			lblTsReTsRssiValue.Text = "0.000";
			lblTsReTsRssiValue.TextAlign = ContentAlignment.MiddleRight;
			lblTsReTsRssiUnit.AutoSize = true;
			lblTsReTsRssiUnit.Location = new Point(275, 112);
			lblTsReTsRssiUnit.Margin = new Padding(1);
			lblTsReTsRssiUnit.MinimumSize = new Size(40, 20);
			lblTsReTsRssiUnit.Name = "lblTsReTsRssiUnit";
			lblTsReTsRssiUnit.Size = new Size(40, 20);
			lblTsReTsRssiUnit.TabIndex = 29;
			lblTsReTsRssiUnit.Text = "μs";
			lblTsReTsRssiUnit.TextAlign = ContentAlignment.MiddleLeft;
			pnlHorizontalSeparator.BorderStyle = BorderStyle.FixedSingle;
			pnlHorizontalSeparator.Location = new Point(7, 63);
			pnlHorizontalSeparator.Name = "pnlHorizontalSeparator";
			pnlHorizontalSeparator.Size = new Size(311, 1);
			pnlHorizontalSeparator.TabIndex = 30;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(347, 286);
			Controls.Add(label4);
			Controls.Add(label3);
			Controls.Add(gBoxRxStartupTime);
			Controls.Add(gBoxTxStartupTime);
			DoubleBuffered = true;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Icon = (Icon)resources.GetObject("$this.Icon");
			KeyPreview = true;
			MaximizeBox = false;
			Name = "RxTxStartupTimeForm";
			Text = "RxTx Startup Time";
			FormClosed += RxTxStartupTimeForm_FormClosed;
			Load += RxTxStartupTimeForm_Load;
			gBoxRxStartupTime.ResumeLayout(false);
			gBoxRxStartupTime.PerformLayout();
			gBoxTxStartupTime.ResumeLayout(false);
			gBoxTxStartupTime.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
