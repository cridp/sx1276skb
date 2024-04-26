using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SemtechLib.Controls;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.Devices.SX1276.Events;
using SemtechLib.Devices.SX1276.UI.Forms;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;

namespace SemtechLib.Devices.SX1276.UI.Controls
{
	public sealed class TemperatureViewControl : UserControl, INotifyDocumentationChanged
	{
		private OperatingModeEnum mode = OperatingModeEnum.Stdby;

		private bool tempCalDone;
        private decimal tempDelta; // = 0.0m;

		private IContainer components;

		private Button btnCalibrate;

		private TempCtrl thermometerCtrl;

		private Led ledTempMeasRunning;

		private Label lblMeasuring;

		private GroupBoxEx gBoxIQCalibration;

		private Label lblSensitivityBoost;

		private Panel pnlSensitivityBoost;

		private RadioButton rBtnRxCalAutoOff;

		private RadioButton rBtnRxCalAutoOn;

		private Label label1;

		private Button btnRxCalibration;

		private Label label2;

		private Led ledRxCalOnGoing;

		private GroupBoxEx gBoxTemperature;

		private Led ledTempStatHigher;

		private Label label3;

		private Label label4;

		private Label label5;

		private Panel panel2;

		private RadioButton rBtnTempMeasOff;

		private RadioButton rBtnTempMeasOn;

		private Label label16;

		private ComboBox cBoxTempThreshold;

		private Panel panel1;

		private Label label6;

		private Label label54;

		private Label lblTempDelta;

		public OperatingModeEnum Mode
		{
			get => mode;
			set
			{
				mode = value;
				switch (mode)
				{
				case OperatingModeEnum.FsTx:
				case OperatingModeEnum.Tx:
				case OperatingModeEnum.FsRx:
				case OperatingModeEnum.Rx:
					btnCalibrate.Enabled = true;
					panel1.Enabled = true;
					if (TempCalDone)
					{
						thermometerCtrl.Enabled = true;
					}
					break;
				case OperatingModeEnum.Sleep:
				case OperatingModeEnum.Stdby:
					btnCalibrate.Enabled = false;
					panel1.Enabled = false;
					thermometerCtrl.Enabled = false;
					break;
				}
			}
		}

		public bool AutoImageCalOn
		{
			get => rBtnRxCalAutoOn.Checked;
			set
			{
				rBtnRxCalAutoOn.CheckedChanged -= rBtnRxCalAutoOn_CheckedChanged;
				rBtnRxCalAutoOff.CheckedChanged -= rBtnRxCalAutoOn_CheckedChanged;
				if (value)
				{
					rBtnRxCalAutoOn.Checked = true;
					rBtnRxCalAutoOff.Checked = false;
				}
				else
				{
					rBtnRxCalAutoOn.Checked = false;
					rBtnRxCalAutoOff.Checked = true;
				}
				rBtnRxCalAutoOn.CheckedChanged += rBtnRxCalAutoOn_CheckedChanged;
				rBtnRxCalAutoOff.CheckedChanged += rBtnRxCalAutoOn_CheckedChanged;
			}
		}

		public bool ImageCalRunning
		{
			get => ledRxCalOnGoing.Checked;
			set => ledRxCalOnGoing.Checked = value;
		}

		public bool TempChange
		{
			get => ledTempStatHigher.Checked;
			set => ledTempStatHigher.Checked = value;
		}

		public TempThresholdEnum TempThreshold
		{
			get => (TempThresholdEnum)cBoxTempThreshold.SelectedIndex;
			set
			{
				cBoxTempThreshold.SelectedIndexChanged -= cBoxTempThreshold_SelectedIndexChanged;
				cBoxTempThreshold.SelectedIndex = (int)value;
				cBoxTempThreshold.SelectedIndexChanged += cBoxTempThreshold_SelectedIndexChanged;
			}
		}

		public bool TempMonitorOff
		{
			get => rBtnTempMeasOff.Checked;
			set
			{
				rBtnTempMeasOn.CheckedChanged -= rBtnTempMeasOn_CheckedChanged;
				rBtnTempMeasOff.CheckedChanged -= rBtnTempMeasOn_CheckedChanged;
				if (value)
				{
					rBtnTempMeasOn.Checked = false;
					rBtnTempMeasOff.Checked = true;
				}
				else
				{
					rBtnTempMeasOn.Checked = true;
					rBtnTempMeasOff.Checked = false;
				}
				rBtnTempMeasOn.CheckedChanged += rBtnTempMeasOn_CheckedChanged;
				rBtnTempMeasOff.CheckedChanged += rBtnTempMeasOn_CheckedChanged;
			}
		}

		public bool TempMeasRunning
		{
			get => ledTempMeasRunning.Checked;
			set => ledTempMeasRunning.Checked = value;
		}

		public decimal TempValue
		{
			get => (decimal)thermometerCtrl.Value;
			set => thermometerCtrl.Value = (double)value;
		}

		public bool TempCalDone
		{
			get => tempCalDone;
			set
			{
				tempCalDone = value;
				thermometerCtrl.Enabled = value;
			}
		}

        public decimal TempValueRoom { get; set; } = 25.0m;

        public decimal TempDelta
		{
			get => tempDelta;
			set
			{
				tempDelta = value;
				lblTempDelta.Text = tempDelta.ToString("N0");
			}
		}

		public event BooleanEventHandler RxCalAutoOnChanged;

		public event EventHandler RxCalibrationChanged;

		public event TempThresholdEventHandler TempThresholdChanged;

		public event DecimalEventHandler TempCalibrateChanged;

		public event BooleanEventHandler TempMeasOnChanged;

		public event DocumentationChangedEventHandler DocumentationChanged;

		public TemperatureViewControl()
		{
			InitializeComponent();
		}

		private void OnRxCalAutoOnChanged(bool value)
		{
            RxCalAutoOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnRxCalibrationChanged()
		{
            RxCalibrationChanged?.Invoke(this, EventArgs.Empty);
        }

		private void OnTempThresholdChanged(TempThresholdEnum value)
		{
            TempThresholdChanged?.Invoke(this, new TempThresholdEventArg(value));
        }

		private void OnTempCalibrateChanged(decimal value)
		{
            TempCalibrateChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnTempMeasOnChanged(bool value)
		{
            TempMeasOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void rBtnRxCalAutoOn_CheckedChanged(object sender, EventArgs e)
		{
			AutoImageCalOn = rBtnRxCalAutoOn.Checked;
			OnRxCalAutoOnChanged(AutoImageCalOn);
		}

		private void btnRxCalibration_Click(object sender, EventArgs e)
		{
			OnRxCalibrationChanged();
		}

		private void cBoxTempThreshold_SelectedIndexChanged(object sender, EventArgs e)
		{
			TempThreshold = (TempThresholdEnum)cBoxTempThreshold.SelectedIndex;
			OnTempThresholdChanged(TempThreshold);
		}

		private void rBtnTempMeasOn_CheckedChanged(object sender, EventArgs e)
		{
			TempMonitorOff = rBtnTempMeasOff.Checked;
			OnTempMeasOnChanged(TempMonitorOff);
		}

		private void btnTempCalibrate_Click(object sender, EventArgs e)
		{
            using var temperatureCalibrationForm = new TemperatureCalibrationForm();
            temperatureCalibrationForm.TempValueRoom = TempValueRoom;
            if (temperatureCalibrationForm.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			try
			{
				Cursor = Cursors.WaitCursor;
				TempValueRoom = temperatureCalibrationForm.TempValueRoom;
				OnTempCalibrateChanged(TempValueRoom);
			}
			catch (Exception)
			{
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
			if (sender == thermometerCtrl)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Temperature", "Thermometer"));
			}
			else if (sender == btnCalibrate)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Temperature", "Calibrate"));
			}
			else if (sender == ledTempMeasRunning || sender == lblMeasuring)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Temperature", "Measure running"));
			}
		}

		private void control_MouseLeave(object sender, EventArgs e)
		{
			OnDocumentationChanged(new DocumentationChangedEventArgs(".", "Overview"));
		}

		private void OnDocumentationChanged(DocumentationChangedEventArgs e)
		{
            DocumentationChanged?.Invoke(this, e);
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
			panel1 = new Panel();
			thermometerCtrl = new TempCtrl();
			gBoxTemperature = new GroupBoxEx();
			lblMeasuring = new Label();
			ledTempMeasRunning = new Led();
			btnCalibrate = new Button();
			label16 = new Label();
			cBoxTempThreshold = new ComboBox();
			ledTempStatHigher = new Led();
			label3 = new Label();
			label4 = new Label();
			label5 = new Label();
			panel2 = new Panel();
			rBtnTempMeasOff = new RadioButton();
			rBtnTempMeasOn = new RadioButton();
			gBoxIQCalibration = new GroupBoxEx();
			label6 = new Label();
			ledRxCalOnGoing = new Led();
			btnRxCalibration = new Button();
			label54 = new Label();
			label2 = new Label();
			label1 = new Label();
			lblTempDelta = new Label();
			lblSensitivityBoost = new Label();
			pnlSensitivityBoost = new Panel();
			rBtnRxCalAutoOff = new RadioButton();
			rBtnRxCalAutoOn = new RadioButton();
			panel1.SuspendLayout();
			gBoxTemperature.SuspendLayout();
			panel2.SuspendLayout();
			gBoxIQCalibration.SuspendLayout();
			pnlSensitivityBoost.SuspendLayout();
			SuspendLayout();
			panel1.Controls.Add(thermometerCtrl);
			panel1.Location = new Point(364, 3);
			panel1.Name = "panel1";
			panel1.Size = new Size(432, 487);
			panel1.TabIndex = 7;
			thermometerCtrl.BackColor = Color.Transparent;
			thermometerCtrl.DrawTics = true;
			thermometerCtrl.Enabled = false;
			thermometerCtrl.ForeColor = Color.Red;
			thermometerCtrl.LargeTicFreq = 10;
			thermometerCtrl.Location = new Point(142, 26);
			thermometerCtrl.Name = "thermometerCtrl";
			thermometerCtrl.Range.Max = 90.0;
			thermometerCtrl.Range.Min = -40.0;
			thermometerCtrl.Size = new Size(148, 434);
			thermometerCtrl.SmallTicFreq = 5;
			thermometerCtrl.TabIndex = 0;
			thermometerCtrl.Text = "Thermometer";
			thermometerCtrl.Value = 25.0;
			thermometerCtrl.MouseEnter += control_MouseEnter;
			thermometerCtrl.MouseLeave += control_MouseLeave;
			gBoxTemperature.Controls.Add(lblMeasuring);
			gBoxTemperature.Controls.Add(ledTempMeasRunning);
			gBoxTemperature.Controls.Add(btnCalibrate);
			gBoxTemperature.Controls.Add(label16);
			gBoxTemperature.Controls.Add(cBoxTempThreshold);
			gBoxTemperature.Controls.Add(ledTempStatHigher);
			gBoxTemperature.Controls.Add(label3);
			gBoxTemperature.Controls.Add(label4);
			gBoxTemperature.Controls.Add(label5);
			gBoxTemperature.Controls.Add(panel2);
			gBoxTemperature.Location = new Point(3, 244);
			gBoxTemperature.Name = "gBoxTemperature";
			gBoxTemperature.Size = new Size(355, 147);
			gBoxTemperature.TabIndex = 6;
			gBoxTemperature.TabStop = false;
			gBoxTemperature.Text = "Temperature";
			lblMeasuring.AutoSize = true;
			lblMeasuring.Location = new Point(6, 72);
			lblMeasuring.Name = "lblMeasuring";
			lblMeasuring.Size = new Size(59, 13);
			lblMeasuring.TabIndex = 1;
			lblMeasuring.Text = "Measuring:";
			lblMeasuring.TextAlign = ContentAlignment.MiddleLeft;
			lblMeasuring.MouseEnter += control_MouseEnter;
			lblMeasuring.MouseLeave += control_MouseLeave;
			ledTempMeasRunning.BackColor = Color.Transparent;
			ledTempMeasRunning.LedColor = Color.Green;
			ledTempMeasRunning.LedSize = new Size(11, 11);
			ledTempMeasRunning.Location = new Point(164, 71);
			ledTempMeasRunning.Name = "ledTempMeasRunning";
			ledTempMeasRunning.Size = new Size(15, 15);
			ledTempMeasRunning.TabIndex = 2;
			ledTempMeasRunning.Text = "Measuring";
			ledTempMeasRunning.MouseEnter += control_MouseEnter;
			ledTempMeasRunning.MouseLeave += control_MouseLeave;
			btnCalibrate.Location = new Point(164, 42);
			btnCalibrate.Name = "btnCalibrate";
			btnCalibrate.Size = new Size(75, 23);
			btnCalibrate.TabIndex = 0;
			btnCalibrate.Text = "Calibrate";
			btnCalibrate.UseVisualStyleBackColor = true;
			btnCalibrate.Click += btnTempCalibrate_Click;
			btnCalibrate.MouseEnter += control_MouseEnter;
			btnCalibrate.MouseLeave += control_MouseLeave;
			label16.AutoSize = true;
			label16.Location = new Point(294, 96);
			label16.Name = "label16";
			label16.Size = new Size(18, 13);
			label16.TabIndex = 9;
			label16.Text = "°C";
			cBoxTempThreshold.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxTempThreshold.FormattingEnabled = true;
			cBoxTempThreshold.Items.AddRange(["5", "10", "15", "20"]);
			cBoxTempThreshold.Location = new Point(164, 92);
			cBoxTempThreshold.Name = "cBoxTempThreshold";
			cBoxTempThreshold.Size = new Size(124, 21);
			cBoxTempThreshold.TabIndex = 8;
			cBoxTempThreshold.SelectedIndexChanged += cBoxTempThreshold_SelectedIndexChanged;
			ledTempStatHigher.BackColor = Color.Transparent;
			ledTempStatHigher.LedColor = Color.Green;
			ledTempStatHigher.LedSize = new Size(11, 11);
			ledTempStatHigher.Location = new Point(164, 121);
			ledTempStatHigher.Name = "ledTempStatHigher";
			ledTempStatHigher.Size = new Size(15, 15);
			ledTempStatHigher.TabIndex = 7;
			ledTempStatHigher.Text = "led1";
			label3.AutoSize = true;
			label3.Location = new Point(6, 123);
			label3.Name = "label3";
			label3.Size = new Size(149, 13);
			label3.TabIndex = 3;
			label3.Text = "Change higher than threshold:";
			label4.AutoSize = true;
			label4.Location = new Point(6, 97);
			label4.Name = "label4";
			label4.Size = new Size(57, 13);
			label4.TabIndex = 3;
			label4.Text = "Threshold:";
			label5.AutoSize = true;
			label5.Location = new Point(6, 21);
			label5.Name = "label5";
			label5.Size = new Size(45, 13);
			label5.TabIndex = 3;
			label5.Text = "Monitor:";
			panel2.AutoSize = true;
			panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel2.Controls.Add(rBtnTempMeasOff);
			panel2.Controls.Add(rBtnTempMeasOn);
			panel2.Location = new Point(164, 19);
			panel2.Name = "panel2";
			panel2.Size = new Size(93, 17);
			panel2.TabIndex = 4;
			rBtnTempMeasOff.AutoSize = true;
			rBtnTempMeasOff.Location = new Point(45, 0);
			rBtnTempMeasOff.Margin = new Padding(3, 0, 3, 0);
			rBtnTempMeasOff.Name = "rBtnTempMeasOff";
			rBtnTempMeasOff.Size = new Size(45, 17);
			rBtnTempMeasOff.TabIndex = 1;
			rBtnTempMeasOff.Text = "OFF";
			rBtnTempMeasOff.UseVisualStyleBackColor = true;
			rBtnTempMeasOff.CheckedChanged += rBtnTempMeasOn_CheckedChanged;
			rBtnTempMeasOn.AutoSize = true;
			rBtnTempMeasOn.Checked = true;
			rBtnTempMeasOn.Location = new Point(3, 0);
			rBtnTempMeasOn.Margin = new Padding(3, 0, 3, 0);
			rBtnTempMeasOn.Name = "rBtnTempMeasOn";
			rBtnTempMeasOn.Size = new Size(41, 17);
			rBtnTempMeasOn.TabIndex = 0;
			rBtnTempMeasOn.TabStop = true;
			rBtnTempMeasOn.Text = "ON";
			rBtnTempMeasOn.UseVisualStyleBackColor = true;
			rBtnTempMeasOn.CheckedChanged += rBtnTempMeasOn_CheckedChanged;
			gBoxIQCalibration.Controls.Add(label6);
			gBoxIQCalibration.Controls.Add(ledRxCalOnGoing);
			gBoxIQCalibration.Controls.Add(btnRxCalibration);
			gBoxIQCalibration.Controls.Add(label54);
			gBoxIQCalibration.Controls.Add(label2);
			gBoxIQCalibration.Controls.Add(label1);
			gBoxIQCalibration.Controls.Add(lblTempDelta);
			gBoxIQCalibration.Controls.Add(lblSensitivityBoost);
			gBoxIQCalibration.Controls.Add(pnlSensitivityBoost);
			gBoxIQCalibration.Location = new Point(3, 102);
			gBoxIQCalibration.Name = "gBoxIQCalibration";
			gBoxIQCalibration.Size = new Size(355, 136);
			gBoxIQCalibration.TabIndex = 6;
			gBoxIQCalibration.TabStop = false;
			gBoxIQCalibration.Text = "IQ calibration";
			label6.AutoSize = true;
			label6.Location = new Point(6, 96);
			label6.Name = "label6";
			label6.Size = new Size(93, 26);
			label6.TabIndex = 20;
			label6.Text = "Temperature delta\r\n( Actual - Former ):";
			ledRxCalOnGoing.BackColor = Color.Transparent;
			ledRxCalOnGoing.LedColor = Color.Green;
			ledRxCalOnGoing.LedSize = new Size(11, 11);
			ledRxCalOnGoing.Location = new Point(164, 71);
			ledRxCalOnGoing.Name = "ledRxCalOnGoing";
			ledRxCalOnGoing.Size = new Size(15, 15);
			ledRxCalOnGoing.TabIndex = 7;
			ledRxCalOnGoing.Text = "led1";
			btnRxCalibration.Location = new Point(164, 42);
			btnRxCalibration.Name = "btnRxCalibration";
			btnRxCalibration.Size = new Size(75, 23);
			btnRxCalibration.TabIndex = 5;
			btnRxCalibration.Text = "Calibrate";
			btnRxCalibration.UseVisualStyleBackColor = true;
			btnRxCalibration.Click += btnRxCalibration_Click;
			label54.AutoSize = true;
			label54.BackColor = Color.Transparent;
			label54.Location = new Point(294, 103);
			label54.Name = "label54";
			label54.Size = new Size(18, 13);
			label54.TabIndex = 19;
			label54.Text = "°C";
			label54.TextAlign = ContentAlignment.MiddleCenter;
			label2.AutoSize = true;
			label2.Location = new Point(6, 73);
			label2.Name = "label2";
			label2.Size = new Size(90, 13);
			label2.TabIndex = 3;
			label2.Text = "Calibration status:";
			label1.AutoSize = true;
			label1.Location = new Point(6, 47);
			label1.Name = "label1";
			label1.Size = new Size(59, 13);
			label1.TabIndex = 3;
			label1.Text = "Calibration:";
			lblTempDelta.BackColor = Color.Transparent;
			lblTempDelta.BorderStyle = BorderStyle.Fixed3D;
			lblTempDelta.Location = new Point(164, 99);
			lblTempDelta.Margin = new Padding(3);
			lblTempDelta.Name = "lblTempDelta";
			lblTempDelta.Size = new Size(124, 20);
			lblTempDelta.TabIndex = 18;
			lblTempDelta.Text = "0";
			lblTempDelta.TextAlign = ContentAlignment.MiddleCenter;
			lblSensitivityBoost.AutoSize = true;
			lblSensitivityBoost.Location = new Point(6, 21);
			lblSensitivityBoost.Name = "lblSensitivityBoost";
			lblSensitivityBoost.Size = new Size(32, 13);
			lblSensitivityBoost.TabIndex = 3;
			lblSensitivityBoost.Text = "Auto:";
			pnlSensitivityBoost.AutoSize = true;
			pnlSensitivityBoost.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlSensitivityBoost.Controls.Add(rBtnRxCalAutoOff);
			pnlSensitivityBoost.Controls.Add(rBtnRxCalAutoOn);
			pnlSensitivityBoost.Enabled = true;
			pnlSensitivityBoost.Location = new Point(164, 19);
			pnlSensitivityBoost.Name = "pnlSensitivityBoost";
			pnlSensitivityBoost.Size = new Size(93, 17);
			pnlSensitivityBoost.TabIndex = 4;
			rBtnRxCalAutoOff.AutoSize = true;
			rBtnRxCalAutoOff.Location = new Point(45, 0);
			rBtnRxCalAutoOff.Margin = new Padding(3, 0, 3, 0);
			rBtnRxCalAutoOff.Name = "rBtnRxCalAutoOff";
			rBtnRxCalAutoOff.Size = new Size(45, 17);
			rBtnRxCalAutoOff.TabIndex = 1;
			rBtnRxCalAutoOff.Text = "OFF";
			rBtnRxCalAutoOff.UseVisualStyleBackColor = true;
			rBtnRxCalAutoOff.CheckedChanged += rBtnRxCalAutoOn_CheckedChanged;
			rBtnRxCalAutoOn.AutoSize = true;
			rBtnRxCalAutoOn.Checked = true;
			rBtnRxCalAutoOn.Location = new Point(3, 0);
			rBtnRxCalAutoOn.Margin = new Padding(3, 0, 3, 0);
			rBtnRxCalAutoOn.Name = "rBtnRxCalAutoOn";
			rBtnRxCalAutoOn.Size = new Size(41, 17);
			rBtnRxCalAutoOn.TabIndex = 0;
			rBtnRxCalAutoOn.TabStop = true;
			rBtnRxCalAutoOn.Text = "ON";
			rBtnRxCalAutoOn.UseVisualStyleBackColor = true;
			rBtnRxCalAutoOn.CheckedChanged += rBtnRxCalAutoOn_CheckedChanged;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(panel1);
			Controls.Add(gBoxTemperature);
			Controls.Add(gBoxIQCalibration);
			Name = "TemperatureViewControl";
			Size = new Size(799, 493);
			panel1.ResumeLayout(false);
			gBoxTemperature.ResumeLayout(false);
			gBoxTemperature.PerformLayout();
			panel2.ResumeLayout(false);
			panel2.PerformLayout();
			gBoxIQCalibration.ResumeLayout(false);
			gBoxIQCalibration.PerformLayout();
			pnlSensitivityBoost.ResumeLayout(false);
			pnlSensitivityBoost.PerformLayout();
			ResumeLayout(false);
		}
	}
}
