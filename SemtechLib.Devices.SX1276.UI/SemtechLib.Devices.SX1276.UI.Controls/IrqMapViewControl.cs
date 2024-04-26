using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using SemtechLib.Controls;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.Devices.SX1276.Events;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;

namespace SemtechLib.Devices.SX1276.UI.Controls
{
	public sealed class IrqMapViewControl : UserControl, INotifyDocumentationChanged
	{
		private decimal frequencyXo = 32000000m;

		private OperatingModeEnum mode = OperatingModeEnum.Undefined;

		private DataModeEnum dataMode = DataModeEnum.Packet;

		private bool bitSync = true;

		private IContainer components;

		private ErrorProvider errorProvider;

		private Label label1;

		private Label label2;

		private Label label3;

		private Label label4;

		private Label label5;

		private Label label6;

		private Label label7;

		private Label lblOperatingMode;

		private ComboBox cBoxDio5Mapping;

		private ComboBox cBoxDio4Mapping;

		private ComboBox cBoxDio3Mapping;

		private ComboBox cBoxDio2Mapping;

		private ComboBox cBoxDio1Mapping;

		private ComboBox cBoxDio0Mapping;

		private Label lblDataMode;

		private Label label13;

		private Label label15;

		private ComboBox cBoxClockOut;

		private Label label16;

		private Label lblBitSynchroniser;

		private Label label8;

		private GroupBoxEx gBoxDeviceStatus;

		private GroupBoxEx gBoxClockOut;

		private GroupBoxEx gBoxDioMapping;

		private GroupBoxEx gBoxDioSettings;

		private Panel pnlPreambleIrq;

		private RadioButton rBtnPreambleIrqOff;

		private RadioButton rBtnPreambleIrqOn;

		private Label label9;

		public decimal FrequencyXo
		{
			get => frequencyXo;
			set
			{
				var clockOut = (int)ClockOut;
				frequencyXo = value;
				cBoxClockOut.Items.Clear();
				for (var num = 1; num <= 32; num <<= 1)
				{
					cBoxClockOut.Items.Add(Math.Round(frequencyXo / num, MidpointRounding.AwayFromZero).ToString(CultureInfo.CurrentCulture));
				}
				cBoxClockOut.Items.Add("RC");
				cBoxClockOut.Items.Add("OFF");
				ClockOut = (ClockOutEnum)clockOut;
			}
		}

		public OperatingModeEnum Mode
		{
			get => mode;
			set
			{
				if (mode != value)
				{
					mode = value;
					PopulateDioCbox();
					switch (mode)
					{
					case OperatingModeEnum.Sleep:
						lblOperatingMode.Text = "Sleep";
						break;
					case OperatingModeEnum.Stdby:
						lblOperatingMode.Text = "Standby";
						break;
					case OperatingModeEnum.FsRx:
						lblOperatingMode.Text = "Synthesizer Rx";
						break;
					case OperatingModeEnum.FsTx:
						lblOperatingMode.Text = "Synthesizer Tx";
						break;
					case OperatingModeEnum.Rx:
						lblOperatingMode.Text = "Receiver";
						break;
					case OperatingModeEnum.Tx:
						lblOperatingMode.Text = "Transmitter";
						break;
					}
				}
			}
		}

		public DataModeEnum DataMode
		{
			get => dataMode;
			set
			{
				dataMode = value;
				PopulateDioCbox();
				switch (dataMode)
				{
				case DataModeEnum.Packet:
					lblDataMode.Text = "Packet";
					break;
				case DataModeEnum.Continuous:
					lblDataMode.Text = "Continuous";
					break;
				}
			}
		}

		public bool BitSyncOn
		{
			get => bitSync;
			set
			{
				bitSync = value;
				lblBitSynchroniser.Text = bitSync ? "ON" : "OFF";
			}
		}

		public DioMappingEnum Dio0Mapping
		{
			get => (DioMappingEnum)cBoxDio0Mapping.SelectedIndex;
			set
			{
				try
				{
					cBoxDio0Mapping.SelectedIndexChanged -= cBoxDio0Mapping_SelectedIndexChanged;
					cBoxDio0Mapping.SelectedIndex = (int)value;
					cBoxDio0Mapping.SelectedIndexChanged += cBoxDio0Mapping_SelectedIndexChanged;
				}
				catch
				{
					cBoxDio0Mapping.SelectedIndexChanged += cBoxDio0Mapping_SelectedIndexChanged;
				}
			}
		}

		public DioMappingEnum Dio1Mapping
		{
			get => (DioMappingEnum)cBoxDio1Mapping.SelectedIndex;
			set
			{
				try
				{
					cBoxDio1Mapping.SelectedIndexChanged -= cBoxDio1Mapping_SelectedIndexChanged;
					cBoxDio1Mapping.SelectedIndex = (int)value;
					cBoxDio1Mapping.SelectedIndexChanged += cBoxDio1Mapping_SelectedIndexChanged;
				}
				catch
				{
					cBoxDio1Mapping.SelectedIndexChanged += cBoxDio1Mapping_SelectedIndexChanged;
				}
			}
		}

		public DioMappingEnum Dio2Mapping
		{
			get => (DioMappingEnum)cBoxDio2Mapping.SelectedIndex;
			set
			{
				try
				{
					cBoxDio2Mapping.SelectedIndexChanged -= cBoxDio2Mapping_SelectedIndexChanged;
					cBoxDio2Mapping.SelectedIndex = (int)value;
					cBoxDio2Mapping.SelectedIndexChanged += cBoxDio2Mapping_SelectedIndexChanged;
				}
				catch
				{
					cBoxDio2Mapping.SelectedIndexChanged += cBoxDio2Mapping_SelectedIndexChanged;
				}
			}
		}

		public DioMappingEnum Dio3Mapping
		{
			get => (DioMappingEnum)cBoxDio3Mapping.SelectedIndex;
			set
			{
				try
				{
					cBoxDio3Mapping.SelectedIndexChanged -= cBoxDio3Mapping_SelectedIndexChanged;
					cBoxDio3Mapping.SelectedIndex = (int)value;
					cBoxDio3Mapping.SelectedIndexChanged += cBoxDio3Mapping_SelectedIndexChanged;
				}
				catch
				{
					cBoxDio3Mapping.SelectedIndexChanged += cBoxDio3Mapping_SelectedIndexChanged;
				}
			}
		}

		public DioMappingEnum Dio4Mapping
		{
			get => (DioMappingEnum)cBoxDio4Mapping.SelectedIndex;
			set
			{
				try
				{
					cBoxDio4Mapping.SelectedIndexChanged -= cBoxDio4Mapping_SelectedIndexChanged;
					cBoxDio4Mapping.SelectedIndex = (int)value;
					cBoxDio4Mapping.SelectedIndexChanged += cBoxDio4Mapping_SelectedIndexChanged;
				}
				catch
				{
					cBoxDio4Mapping.SelectedIndexChanged += cBoxDio4Mapping_SelectedIndexChanged;
				}
			}
		}

		public DioMappingEnum Dio5Mapping
		{
			get => (DioMappingEnum)cBoxDio5Mapping.SelectedIndex;
			set
			{
				try
				{
					cBoxDio5Mapping.SelectedIndexChanged -= cBoxDio5Mapping_SelectedIndexChanged;
					cBoxDio5Mapping.SelectedIndex = (int)value;
					cBoxDio5Mapping.SelectedIndexChanged += cBoxDio5Mapping_SelectedIndexChanged;
				}
				catch
				{
					cBoxDio5Mapping.SelectedIndexChanged += cBoxDio5Mapping_SelectedIndexChanged;
				}
			}
		}

		public bool MapPreambleDetect
		{
			get => rBtnPreambleIrqOn.Checked;
			set
			{
				rBtnPreambleIrqOn.CheckedChanged -= rBtnPreambleIrq_CheckedChanged;
				rBtnPreambleIrqOff.CheckedChanged -= rBtnPreambleIrq_CheckedChanged;
				if (value)
				{
					rBtnPreambleIrqOn.Checked = true;
					rBtnPreambleIrqOff.Checked = false;
				}
				else
				{
					rBtnPreambleIrqOn.Checked = false;
					rBtnPreambleIrqOff.Checked = true;
				}
				PopulateDioCbox();
				rBtnPreambleIrqOn.CheckedChanged += rBtnPreambleIrq_CheckedChanged;
				rBtnPreambleIrqOff.CheckedChanged += rBtnPreambleIrq_CheckedChanged;
			}
		}

		public ClockOutEnum ClockOut
		{
			get => (ClockOutEnum)cBoxClockOut.SelectedIndex;
			set
			{
				try
				{
					cBoxClockOut.SelectedIndexChanged -= cBoxClockOut_SelectedIndexChanged;
					cBoxClockOut.SelectedIndex = (int)value;
					cBoxClockOut.SelectedIndexChanged += cBoxClockOut_SelectedIndexChanged;
				}
				catch
				{
					cBoxClockOut.SelectedIndexChanged += cBoxClockOut_SelectedIndexChanged;
				}
			}
		}

		public event BooleanEventHandler DioPreambleIrqOnChanged;

		public event DioMappingEventHandler DioMappingChanged;

		public event ClockOutEventHandler ClockOutChanged;

		public event DocumentationChangedEventHandler DocumentationChanged;

		public IrqMapViewControl()
		{
			InitializeComponent();
		}

		private void PopulateDioCbox()
		{
			var array = new int[6]
			{
				(int)Dio0Mapping,
				(int)Dio1Mapping,
				(int)Dio2Mapping,
				(int)Dio3Mapping,
				(int)Dio4Mapping,
				(int)Dio5Mapping
			};
			cBoxDio0Mapping.Items.Clear();
			cBoxDio1Mapping.Items.Clear();
			cBoxDio2Mapping.Items.Clear();
			cBoxDio3Mapping.Items.Clear();
			cBoxDio4Mapping.Items.Clear();
			cBoxDio5Mapping.Items.Clear();
			switch (dataMode)
			{
			case DataModeEnum.Packet:
				switch (mode)
				{
				case OperatingModeEnum.Sleep:
					cBoxDio0Mapping.Items.AddRange(["-", "-", "-", "-"]);
					cBoxDio1Mapping.Items.AddRange(["FifoLevel", "FifoEmpty", "FifoFull", "-"]);
					cBoxDio2Mapping.Items.AddRange(["FifoFull", "-", "FifoFull", "FifoFull"]);
					cBoxDio3Mapping.Items.AddRange(["FifoEmpty", "-", "FifoEmpty", "FifoEmpty"]);
					cBoxDio4Mapping.Items.AddRange(["-", "-", "-", "-"]);
					cBoxDio5Mapping.Items.AddRange(["ClkOut", "-", "-", "-"]);
					break;
				case OperatingModeEnum.Stdby:
					cBoxDio0Mapping.Items.AddRange(["-", "-", "-", "LowBat"]);
					cBoxDio1Mapping.Items.AddRange(["FifoLevel", "FifoEmpty", "FifoFull", "-"]);
					cBoxDio2Mapping.Items.AddRange(["FifoFull", "-", "FifoFull", "FifoFull"]);
					cBoxDio3Mapping.Items.AddRange(["FifoEmpty", "-", "FifoEmpty", "FifoEmpty"]);
					cBoxDio4Mapping.Items.AddRange(["LowBat", "-", "-", "-"]);
					cBoxDio5Mapping.Items.AddRange(["ClkOut", "-", "-", "ModeReady"]);
					break;
				case OperatingModeEnum.FsTx:
				case OperatingModeEnum.FsRx:
					cBoxDio0Mapping.Items.AddRange(["-", "-", "-", "LowBat"]);
					cBoxDio1Mapping.Items.AddRange(["FifoLevel", "FifoEmpty", "FifoFull", "-"]);
					cBoxDio2Mapping.Items.AddRange(["FifoFull", "-", "FifoFull", "FifoFull"]);
					cBoxDio3Mapping.Items.AddRange(["FifoEmpty", "-", "FifoEmpty", "FifoEmpty"]);
					cBoxDio4Mapping.Items.AddRange(["LowBat", "PllLock", "-", "-"]);
					cBoxDio5Mapping.Items.AddRange(["ClkOut", "PllLock", "-", "ModeReady"]);
					break;
				case OperatingModeEnum.Rx:
					if (MapPreambleDetect)
					{
						cBoxDio0Mapping.Items.AddRange(["PayloadReady", "CrcOk", "-", "LowBat"]);
						cBoxDio1Mapping.Items.AddRange(["FifoLevel", "FifoEmpty", "FifoFull", "-"]);
						cBoxDio2Mapping.Items.AddRange(["FifoFull", "RxReady", "Timeout", "SyncAddress"]);
						cBoxDio3Mapping.Items.AddRange(["FifoEmpty", "-", "FifoEmpty", "FifoEmpty"]);
						cBoxDio4Mapping.Items.AddRange(["LowBat", "PllLock", "Timeout", "Preamble"]);
						cBoxDio5Mapping.Items.AddRange(["ClkOut", "PllLock", "Data", "ModeReady"]);
					}
					else
					{
						cBoxDio0Mapping.Items.AddRange(["PayloadReady", "CrcOk", "-", "LowBat"]);
						cBoxDio1Mapping.Items.AddRange(["FifoLevel", "FifoEmpty", "FifoFull", "-"]);
						cBoxDio2Mapping.Items.AddRange(["FifoFull", "RxReady", "Timeout", "SyncAddress"]);
						cBoxDio3Mapping.Items.AddRange(["FifoEmpty", "-", "FifoEmpty", "FifoEmpty"]);
						cBoxDio4Mapping.Items.AddRange(["LowBat", "PllLock", "Timeout", "Rssi"]);
						cBoxDio5Mapping.Items.AddRange(["ClkOut", "PllLock", "Data", "ModeReady"]);
					}
					break;
				case OperatingModeEnum.Tx:
					cBoxDio0Mapping.Items.AddRange(["PacketSent", "-", "-", "LowBat"]);
					cBoxDio1Mapping.Items.AddRange(["FifoLevel", "FifoEmpty", "FifoFull", "-"]);
					cBoxDio2Mapping.Items.AddRange(["FifoFull", "-", "FifoFull", "FifoFull"]);
					cBoxDio3Mapping.Items.AddRange(["FifoEmpty", "TxReady", "FifoEmpty", "FifoEmpty"]);
					cBoxDio4Mapping.Items.AddRange(["LowBat", "PllLock", "-", "-"]);
					cBoxDio5Mapping.Items.AddRange(["ClkOut", "PllLock", "Data", "ModeReady"]);
					break;
				}
				break;
			case DataModeEnum.Continuous:
				switch (mode)
				{
				case OperatingModeEnum.Sleep:
					cBoxDio0Mapping.Items.AddRange(["-", "-", "-", "-"]);
					cBoxDio1Mapping.Items.AddRange(["-", "-", "-", "-"]);
					cBoxDio2Mapping.Items.AddRange(["-", "-", "-", "-"]);
					cBoxDio3Mapping.Items.AddRange(["-", "-", "-", "-"]);
					cBoxDio4Mapping.Items.AddRange(["-", "-", "-", "-"]);
					cBoxDio5Mapping.Items.AddRange(["ClkOut", "-", "-", "-"]);
					break;
				case OperatingModeEnum.Stdby:
					cBoxDio0Mapping.Items.AddRange(["-", "-", "-", "-"]);
					cBoxDio1Mapping.Items.AddRange(["-", "-", "-", "-"]);
					cBoxDio2Mapping.Items.AddRange(["-", "-", "-", "-"]);
					cBoxDio3Mapping.Items.AddRange(["-", "-", "-", "LowBat"]);
					cBoxDio4Mapping.Items.AddRange(["-", "-", "-", "ModeReady"]);
					cBoxDio5Mapping.Items.AddRange(["ClkOut", "-", "-", "ModeReady"]);
					break;
				case OperatingModeEnum.FsTx:
				case OperatingModeEnum.FsRx:
					cBoxDio0Mapping.Items.AddRange(["-", "-", "-", "-"]);
					cBoxDio1Mapping.Items.AddRange(["-", "-", "-", "-"]);
					cBoxDio2Mapping.Items.AddRange(["-", "-", "-", "-"]);
					cBoxDio3Mapping.Items.AddRange(["-", "-", "-", "LowBat"]);
					cBoxDio4Mapping.Items.AddRange(["LowBat", "PllLock", "-", "ModeReady"]);
					cBoxDio5Mapping.Items.AddRange(["ClkOut", "PllLock", "-", "ModeReady"]);
					break;
				case OperatingModeEnum.Rx:
					if (MapPreambleDetect)
					{
						cBoxDio0Mapping.Items.AddRange(["SyncAddress", "Preamble", "RxReady", "-"]);
						cBoxDio1Mapping.Items.AddRange(["Dclk", "Preamble", "-", "-"]);
						cBoxDio2Mapping.Items.AddRange(["Data", "Data", "Data", "Data"]);
						cBoxDio3Mapping.Items.AddRange(["Timeout", "Preamble", "-", "LowBat"]);
						cBoxDio4Mapping.Items.AddRange(["LowBat", "PllLock", "Timeout", "ModeReady"]);
						cBoxDio5Mapping.Items.AddRange(["ClkOut", "PllLock", "Preamble", "ModeReady"]);
					}
					else
					{
						cBoxDio0Mapping.Items.AddRange(["SyncAddress", "Rssi", "RxReady", "-"]);
						cBoxDio1Mapping.Items.AddRange(["Dclk", "Rssi", "-", "-"]);
						cBoxDio2Mapping.Items.AddRange(["Data", "Data", "Data", "Data"]);
						cBoxDio3Mapping.Items.AddRange(["Timeout", "Rssi", "-", "LowBat"]);
						cBoxDio4Mapping.Items.AddRange(["LowBat", "PllLock", "Timeout", "ModeReady"]);
						cBoxDio5Mapping.Items.AddRange(["ClkOut", "PllLock", "Rssi", "ModeReady"]);
					}
					break;
				case OperatingModeEnum.Tx:
					cBoxDio0Mapping.Items.AddRange(["TxReady", "-", "-", "-"]);
					cBoxDio1Mapping.Items.AddRange(["Dclk", "-", "-", "-"]);
					cBoxDio2Mapping.Items.AddRange(["Data", "Data", "Data", "Data"]);
					cBoxDio3Mapping.Items.AddRange(["-", "-", "-", "LowBat"]);
					cBoxDio4Mapping.Items.AddRange(["LowBat", "PllLock", "-", "ModeReady"]);
					cBoxDio5Mapping.Items.AddRange(["ClkOut", "PllLock", "-", "ModeReady"]);
					break;
				}
				break;
			}
			Dio0Mapping = (DioMappingEnum)array[0];
			Dio1Mapping = (DioMappingEnum)array[1];
			Dio2Mapping = (DioMappingEnum)array[2];
			Dio3Mapping = (DioMappingEnum)array[3];
			Dio4Mapping = (DioMappingEnum)array[4];
			Dio5Mapping = (DioMappingEnum)array[5];
		}

		private void OnDioMappingChanged(byte id, DioMappingEnum value)
		{
            DioMappingChanged?.Invoke(this, new DioMappingEventArg(id, value));
        }

		private void OnClockOutChanged(ClockOutEnum value)
		{
            ClockOutChanged?.Invoke(this, new ClockOutEventArg(value));
        }

		private void OnDioPreambleIrqOnChanged(bool value)
		{
            DioPreambleIrqOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void cBoxDio0Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged(0, (DioMappingEnum)cBoxDio0Mapping.SelectedIndex);
		}

		private void cBoxDio1Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged(1, (DioMappingEnum)cBoxDio1Mapping.SelectedIndex);
		}

		private void cBoxDio2Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged(2, (DioMappingEnum)cBoxDio2Mapping.SelectedIndex);
		}

		private void cBoxDio3Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged(3, (DioMappingEnum)cBoxDio3Mapping.SelectedIndex);
		}

		private void cBoxDio4Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged(4, (DioMappingEnum)cBoxDio4Mapping.SelectedIndex);
		}

		private void cBoxDio5Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged(5, (DioMappingEnum)cBoxDio5Mapping.SelectedIndex);
		}

		private void cBoxClockOut_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnClockOutChanged((ClockOutEnum)cBoxClockOut.SelectedIndex);
		}

		private void rBtnPreambleIrq_CheckedChanged(object sender, EventArgs e)
		{
			MapPreambleDetect = rBtnPreambleIrqOn.Checked;
			OnDioPreambleIrqOnChanged(MapPreambleDetect);
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
			if (sender == gBoxDeviceStatus)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "Device status"));
			}
			else if (sender == gBoxDioMapping)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Irq mapping", "DIO mapping"));
			}
			else if (sender == gBoxClockOut)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Irq mapping", "Clock out"));
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
			components = new Container();
			errorProvider = new ErrorProvider(components);
			gBoxClockOut = new GroupBoxEx();
			cBoxClockOut = new ComboBox();
			label15 = new Label();
			label16 = new Label();
			gBoxDioMapping = new GroupBoxEx();
			cBoxDio5Mapping = new ComboBox();
			cBoxDio4Mapping = new ComboBox();
			label2 = new Label();
			label7 = new Label();
			cBoxDio3Mapping = new ComboBox();
			cBoxDio0Mapping = new ComboBox();
			label3 = new Label();
			cBoxDio1Mapping = new ComboBox();
			label4 = new Label();
			cBoxDio2Mapping = new ComboBox();
			label5 = new Label();
			label6 = new Label();
			gBoxDioSettings = new GroupBoxEx();
			pnlPreambleIrq = new Panel();
			rBtnPreambleIrqOff = new RadioButton();
			rBtnPreambleIrqOn = new RadioButton();
			label9 = new Label();
			gBoxDeviceStatus = new GroupBoxEx();
			lblBitSynchroniser = new Label();
			lblOperatingMode = new Label();
			label13 = new Label();
			label1 = new Label();
			label8 = new Label();
			lblDataMode = new Label();
			((ISupportInitialize)errorProvider).BeginInit();
			gBoxClockOut.SuspendLayout();
			gBoxDioMapping.SuspendLayout();
			gBoxDioSettings.SuspendLayout();
			pnlPreambleIrq.SuspendLayout();
			gBoxDeviceStatus.SuspendLayout();
			SuspendLayout();
			errorProvider.ContainerControl = this;
			gBoxClockOut.Controls.Add(cBoxClockOut);
			gBoxClockOut.Controls.Add(label15);
			gBoxClockOut.Controls.Add(label16);
			gBoxClockOut.Location = new Point(253, 386);
			gBoxClockOut.Name = "gBoxClockOut";
			gBoxClockOut.Size = new Size(293, 45);
			gBoxClockOut.TabIndex = 2;
			gBoxClockOut.TabStop = false;
			gBoxClockOut.Text = "Clock out";
			gBoxClockOut.MouseEnter += control_MouseEnter;
			gBoxClockOut.MouseLeave += control_MouseLeave;
			cBoxClockOut.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxClockOut.FormattingEnabled = true;
			cBoxClockOut.Location = new Point(162, 19);
			cBoxClockOut.Name = "cBoxClockOut";
			cBoxClockOut.Size = new Size(100, 21);
			cBoxClockOut.TabIndex = 1;
			cBoxClockOut.SelectedIndexChanged += cBoxClockOut_SelectedIndexChanged;
			label15.AutoSize = true;
			label15.Location = new Point(5, 23);
			label15.Name = "label15";
			label15.Size = new Size(60, 13);
			label15.TabIndex = 0;
			label15.Text = "Frequency:";
			label15.TextAlign = ContentAlignment.MiddleLeft;
			label16.AutoSize = true;
			label16.Location = new Point(268, 23);
			label16.Name = "label16";
			label16.Size = new Size(20, 13);
			label16.TabIndex = 2;
			label16.Text = "Hz";
			label16.TextAlign = ContentAlignment.MiddleLeft;
			gBoxDioMapping.Controls.Add(cBoxDio5Mapping);
			gBoxDioMapping.Controls.Add(cBoxDio4Mapping);
			gBoxDioMapping.Controls.Add(label2);
			gBoxDioMapping.Controls.Add(label7);
			gBoxDioMapping.Controls.Add(cBoxDio3Mapping);
			gBoxDioMapping.Controls.Add(cBoxDio0Mapping);
			gBoxDioMapping.Controls.Add(label3);
			gBoxDioMapping.Controls.Add(cBoxDio1Mapping);
			gBoxDioMapping.Controls.Add(label4);
			gBoxDioMapping.Controls.Add(cBoxDio2Mapping);
			gBoxDioMapping.Controls.Add(label5);
			gBoxDioMapping.Controls.Add(label6);
			gBoxDioMapping.Location = new Point(253, 201);
			gBoxDioMapping.Name = "gBoxDioMapping";
			gBoxDioMapping.Size = new Size(293, 179);
			gBoxDioMapping.TabIndex = 1;
			gBoxDioMapping.TabStop = false;
			gBoxDioMapping.Text = "DIO mapping";
			gBoxDioMapping.MouseEnter += control_MouseEnter;
			gBoxDioMapping.MouseLeave += control_MouseLeave;
			cBoxDio5Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxDio5Mapping.FormattingEnabled = true;
			cBoxDio5Mapping.Location = new Point(162, 19);
			cBoxDio5Mapping.Name = "cBoxDio5Mapping";
			cBoxDio5Mapping.Size = new Size(100, 21);
			cBoxDio5Mapping.TabIndex = 1;
			cBoxDio5Mapping.SelectedIndexChanged += cBoxDio5Mapping_SelectedIndexChanged;
			cBoxDio4Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxDio4Mapping.FormattingEnabled = true;
			cBoxDio4Mapping.Location = new Point(162, 46);
			cBoxDio4Mapping.Name = "cBoxDio4Mapping";
			cBoxDio4Mapping.Size = new Size(100, 21);
			cBoxDio4Mapping.TabIndex = 3;
			cBoxDio4Mapping.SelectedIndexChanged += cBoxDio4Mapping_SelectedIndexChanged;
			label2.AutoSize = true;
			label2.Location = new Point(6, 23);
			label2.Margin = new Padding(3);
			label2.Name = "label2";
			label2.Size = new Size(35, 13);
			label2.TabIndex = 0;
			label2.Text = "DIO5:";
			label2.TextAlign = ContentAlignment.MiddleLeft;
			label7.AutoSize = true;
			label7.Location = new Point(6, 158);
			label7.Margin = new Padding(3);
			label7.Name = "label7";
			label7.Size = new Size(35, 13);
			label7.TabIndex = 10;
			label7.Text = "DIO0:";
			label7.TextAlign = ContentAlignment.MiddleLeft;
			cBoxDio3Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxDio3Mapping.FormattingEnabled = true;
			cBoxDio3Mapping.Location = new Point(162, 73);
			cBoxDio3Mapping.Name = "cBoxDio3Mapping";
			cBoxDio3Mapping.Size = new Size(100, 21);
			cBoxDio3Mapping.TabIndex = 5;
			cBoxDio3Mapping.SelectedIndexChanged += cBoxDio3Mapping_SelectedIndexChanged;
			cBoxDio0Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxDio0Mapping.FormattingEnabled = true;
			cBoxDio0Mapping.Location = new Point(162, 154);
			cBoxDio0Mapping.Name = "cBoxDio0Mapping";
			cBoxDio0Mapping.Size = new Size(100, 21);
			cBoxDio0Mapping.TabIndex = 11;
			cBoxDio0Mapping.SelectedIndexChanged += cBoxDio0Mapping_SelectedIndexChanged;
			label3.AutoSize = true;
			label3.Location = new Point(6, 50);
			label3.Margin = new Padding(3);
			label3.Name = "label3";
			label3.Size = new Size(35, 13);
			label3.TabIndex = 2;
			label3.Text = "DIO4:";
			label3.TextAlign = ContentAlignment.MiddleLeft;
			cBoxDio1Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxDio1Mapping.FormattingEnabled = true;
			cBoxDio1Mapping.Location = new Point(162, 127);
			cBoxDio1Mapping.Name = "cBoxDio1Mapping";
			cBoxDio1Mapping.Size = new Size(100, 21);
			cBoxDio1Mapping.TabIndex = 9;
			cBoxDio1Mapping.SelectedIndexChanged += cBoxDio1Mapping_SelectedIndexChanged;
			label4.AutoSize = true;
			label4.Location = new Point(6, 77);
			label4.Margin = new Padding(3);
			label4.Name = "label4";
			label4.Size = new Size(35, 13);
			label4.TabIndex = 4;
			label4.Text = "DIO3:";
			label4.TextAlign = ContentAlignment.MiddleLeft;
			cBoxDio2Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxDio2Mapping.FormattingEnabled = true;
			cBoxDio2Mapping.Location = new Point(162, 100);
			cBoxDio2Mapping.Name = "cBoxDio2Mapping";
			cBoxDio2Mapping.Size = new Size(100, 21);
			cBoxDio2Mapping.TabIndex = 7;
			cBoxDio2Mapping.SelectedIndexChanged += cBoxDio2Mapping_SelectedIndexChanged;
			label5.AutoSize = true;
			label5.Location = new Point(6, 104);
			label5.Margin = new Padding(3);
			label5.Name = "label5";
			label5.Size = new Size(35, 13);
			label5.TabIndex = 6;
			label5.Text = "DIO2:";
			label5.TextAlign = ContentAlignment.MiddleLeft;
			label6.AutoSize = true;
			label6.Location = new Point(6, 131);
			label6.Margin = new Padding(3);
			label6.Name = "label6";
			label6.Size = new Size(35, 13);
			label6.TabIndex = 8;
			label6.Text = "DIO1:";
			label6.TextAlign = ContentAlignment.MiddleLeft;
			gBoxDioSettings.Controls.Add(pnlPreambleIrq);
			gBoxDioSettings.Controls.Add(label9);
			gBoxDioSettings.Location = new Point(253, 145);
			gBoxDioSettings.Name = "gBoxDioSettings";
			gBoxDioSettings.Size = new Size(293, 50);
			gBoxDioSettings.TabIndex = 0;
			gBoxDioSettings.TabStop = false;
			gBoxDioSettings.Text = "DIO settings";
			gBoxDioSettings.MouseEnter += control_MouseEnter;
			gBoxDioSettings.MouseLeave += control_MouseLeave;
			pnlPreambleIrq.AutoSize = true;
			pnlPreambleIrq.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlPreambleIrq.Controls.Add(rBtnPreambleIrqOff);
			pnlPreambleIrq.Controls.Add(rBtnPreambleIrqOn);
			pnlPreambleIrq.Location = new Point(162, 19);
			pnlPreambleIrq.Name = "pnlPreambleIrq";
			pnlPreambleIrq.Size = new Size(102, 20);
			pnlPreambleIrq.TabIndex = 3;
			rBtnPreambleIrqOff.AutoSize = true;
			rBtnPreambleIrqOff.Location = new Point(54, 3);
			rBtnPreambleIrqOff.Margin = new Padding(3, 0, 3, 0);
			rBtnPreambleIrqOff.Name = "rBtnPreambleIrqOff";
			rBtnPreambleIrqOff.Size = new Size(45, 17);
			rBtnPreambleIrqOff.TabIndex = 1;
			rBtnPreambleIrqOff.Text = "OFF";
			rBtnPreambleIrqOff.UseVisualStyleBackColor = true;
			rBtnPreambleIrqOff.CheckedChanged += rBtnPreambleIrq_CheckedChanged;
			rBtnPreambleIrqOn.AutoSize = true;
			rBtnPreambleIrqOn.Checked = true;
			rBtnPreambleIrqOn.Location = new Point(3, 3);
			rBtnPreambleIrqOn.Margin = new Padding(3, 0, 3, 0);
			rBtnPreambleIrqOn.Name = "rBtnPreambleIrqOn";
			rBtnPreambleIrqOn.Size = new Size(41, 17);
			rBtnPreambleIrqOn.TabIndex = 0;
			rBtnPreambleIrqOn.TabStop = true;
			rBtnPreambleIrqOn.Text = "ON";
			rBtnPreambleIrqOn.UseVisualStyleBackColor = true;
			rBtnPreambleIrqOn.CheckedChanged += rBtnPreambleIrq_CheckedChanged;
			label9.AutoSize = true;
			label9.Location = new Point(6, 24);
			label9.Name = "label9";
			label9.Size = new Size(76, 13);
			label9.TabIndex = 2;
			label9.Text = "Preamble IRQ:";
			gBoxDeviceStatus.Controls.Add(lblBitSynchroniser);
			gBoxDeviceStatus.Controls.Add(lblOperatingMode);
			gBoxDeviceStatus.Controls.Add(label13);
			gBoxDeviceStatus.Controls.Add(label1);
			gBoxDeviceStatus.Controls.Add(label8);
			gBoxDeviceStatus.Controls.Add(lblDataMode);
			gBoxDeviceStatus.Location = new Point(253, 62);
			gBoxDeviceStatus.Name = "gBoxDeviceStatus";
			gBoxDeviceStatus.Size = new Size(293, 77);
			gBoxDeviceStatus.TabIndex = 0;
			gBoxDeviceStatus.TabStop = false;
			gBoxDeviceStatus.Text = "Device status";
			gBoxDeviceStatus.MouseEnter += control_MouseEnter;
			gBoxDeviceStatus.MouseLeave += control_MouseLeave;
			lblBitSynchroniser.AutoSize = true;
			lblBitSynchroniser.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblBitSynchroniser.Location = new Point(159, 19);
			lblBitSynchroniser.Margin = new Padding(3);
			lblBitSynchroniser.Name = "lblBitSynchroniser";
			lblBitSynchroniser.Size = new Size(25, 13);
			lblBitSynchroniser.TabIndex = 1;
			lblBitSynchroniser.Text = "ON";
			lblBitSynchroniser.TextAlign = ContentAlignment.MiddleLeft;
			lblOperatingMode.AutoSize = true;
			lblOperatingMode.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblOperatingMode.Location = new Point(159, 57);
			lblOperatingMode.Margin = new Padding(3);
			lblOperatingMode.Name = "lblOperatingMode";
			lblOperatingMode.Size = new Size(39, 13);
			lblOperatingMode.TabIndex = 5;
			lblOperatingMode.Text = "Sleep";
			lblOperatingMode.TextAlign = ContentAlignment.MiddleLeft;
			label13.AutoSize = true;
			label13.Location = new Point(6, 38);
			label13.Margin = new Padding(3);
			label13.Name = "label13";
			label13.Size = new Size(62, 13);
			label13.TabIndex = 2;
			label13.Text = "Data mode:";
			label13.TextAlign = ContentAlignment.MiddleLeft;
			label1.AutoSize = true;
			label1.Location = new Point(6, 57);
			label1.Margin = new Padding(3);
			label1.Name = "label1";
			label1.Size = new Size(85, 13);
			label1.TabIndex = 4;
			label1.Text = "Operating mode:";
			label1.TextAlign = ContentAlignment.MiddleLeft;
			label8.AutoSize = true;
			label8.Location = new Point(6, 19);
			label8.Margin = new Padding(3);
			label8.Name = "label8";
			label8.Size = new Size(86, 13);
			label8.TabIndex = 0;
			label8.Text = "Bit Synchronizer:";
			label8.TextAlign = ContentAlignment.MiddleLeft;
			lblDataMode.AutoSize = true;
			lblDataMode.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblDataMode.Location = new Point(159, 38);
			lblDataMode.Margin = new Padding(3);
			lblDataMode.Name = "lblDataMode";
			lblDataMode.Size = new Size(47, 13);
			lblDataMode.TabIndex = 3;
			lblDataMode.Text = "Packet";
			lblDataMode.TextAlign = ContentAlignment.MiddleLeft;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(gBoxClockOut);
			Controls.Add(gBoxDioMapping);
			Controls.Add(gBoxDioSettings);
			Controls.Add(gBoxDeviceStatus);
			Name = "IrqMapViewControl";
			Size = new Size(799, 493);
			((ISupportInitialize)errorProvider).EndInit();
			gBoxClockOut.ResumeLayout(false);
			gBoxClockOut.PerformLayout();
			gBoxDioMapping.ResumeLayout(false);
			gBoxDioMapping.PerformLayout();
			gBoxDioSettings.ResumeLayout(false);
			gBoxDioSettings.PerformLayout();
			pnlPreambleIrq.ResumeLayout(false);
			pnlPreambleIrq.PerformLayout();
			gBoxDeviceStatus.ResumeLayout(false);
			gBoxDeviceStatus.PerformLayout();
			ResumeLayout(false);
		}
	}
}
