using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SemtechLib.Controls;
using SemtechLib.Controls.HexBoxCtrl;
using SemtechLib.Devices.SX1276LR.Enumerations;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;

namespace SemtechLib.Devices.SX1276LR.UI.Controls
{
	public sealed class LoRaViewControl : UserControl, INotifyDocumentationChanged
	{
		private bool inHexPayloadDataChanged;
        private decimal symbolTime;

		private OperatingModeEnum mode = OperatingModeEnum.Stdby;

		private byte[] payload;

		private IContainer components;

		private Label label20;

		private Panel panel6;

		private Label label21;

		private Label label22;

		private Panel panel7;

		private Label label23;

		private Label label24;

		private Label label25;

		private Label label26;

		private Label label27;

		private Label label28;

		private ErrorProvider errorProvider;

		private Label label30;

		private Label lblListenResolRx;

		private GroupBoxEx gBoxIrqMask;

		private Panel panel10;

		private RadioButton rBtnCadDetectedMaskOff;

		private RadioButton rBtnCadDetectedMaskOn;

		private Label label7;

		private Panel panel9;

		private RadioButton rBtnFhssChangeChannelMaskOff;

		private RadioButton rBtnFhssChangeChannelMaskOn;

		private Label label6;

		private Panel panel8;

		private RadioButton rBtnCadDoneMaskOff;

		private RadioButton rBtnCadDoneMaskOn;

		private Label label5;

		private Panel panel5;

		private RadioButton rBtnTxDoneMaskOff;

		private RadioButton rBtnTxDoneMaskOn;

		private Label label4;

		private Panel panel3;

		private RadioButton rBtnValidHeaderMaskOff;

		private RadioButton rBtnValidHeaderMaskOn;

		private Label label3;

		private Panel panel2;

		private RadioButton rBtnPayloadCrcErrorMaskOff;

		private RadioButton rBtnPayloadCrcErrorMaskOn;

		private Label label2;

		private Panel panel1;

		private RadioButton rBtnRxDoneMaskOff;

		private RadioButton rBtnRxDoneMaskOn;

		private Label label1;

		private Panel panel4;

		private RadioButton rBtnRxTimeoutMaskOff;

		private RadioButton rBtnRxTimeoutMaskOn;

		private Label label10;

		private Panel panel11;

		private RadioButton rBtnImplicitHeaderOff;

		private RadioButton rBtnImplicitHeaderOn;

		private Label label8;

		private Label label11;

		private Label label9;

		private NumericUpDownEx nudSymbTimeout;

		private Panel panel12;

		private RadioButton rBtnPayloadCrcOff;

		private RadioButton rBtnPayloadCrcOn;

		private Label label12;

		private ComboBox cBoxCodingRate;

		private Label label15;

		private Label label16;

		private Label label17;

		private NumericUpDownEx nudPayloadLength;

		private Label label19;

		private NumericUpDownEx nudPreambleLength;

		private Label label29;

		private ComboBox cBoxBandwidth;

		private Label label32;

		private Label label31;

		private ComboBox cBoxSpreadingFactor;

		private Label label33;

		private Label label35;

		private NumericUpDownEx nudFreqHoppingPeriod;

		private Label label34;

		private Led ledRxPayloadCrcOn;

		private Label label39;

		private Label lblRxPayloadCodingRate;

		private Label label37;

		private Label lblRxNbBytes;

		private Label label38;

		private Label lblRxValidHeaderCnt;

		private Label label18;

		private Label lblRxPacketCnt;

		private Label label40;

		private GroupBoxEx groupBoxEx1;

		private Label label42;

		private Led ledSignalDetected;

		private Label label45;

		private Led ledSignalSynchronized;

		private Label label43;

		private Led ledRxOnGoing;

		private Label label41;

		private Led ledHeaderInfoValid;

		private Label label44;

		private Led ledModemClear;

		private Label lblPacketSnr;

		private Label label46;

		private Label lblPacketRssi;

		private Label label47;

		private Label lblRssiValue;

		private Label label48;

		private Label lblHopChannel;

		private Label label49;

		private Label lblFifoRxCurrentAddr;

		private Label label50;

		private GroupBoxEx gBoxControl;

		private TextBox tBoxPacketsNb;

		private Button btnLog;

		private CheckBox cBtnPacketHandlerStartStop;

		private Label lblPacketsNb;

		private TextBox tBoxPacketsRepeatValue;

		private Label lblPacketsRepeatValue;

		private GroupBoxEx gBoxMessage;

		private TableLayoutPanel tblPayloadMessage;

		private HexBox hexBoxPayload;

		private Label label51;

		private Label label52;

		private Label label53;

		private TableLayoutPanel pnlPacketStatus;

		private TableLayoutPanel pnlHeaderInfo;

		private Panel pnlPacketStatusHeaderName;

		private Label lblPacketStatusHeaderName;

		private Panel pnlRxHeaderInfoHeader;

		private Label lblRxHeaderInfoHeaderName;

		private Label lblPllTimeout;

		private Led ledPllTimeout;

		private GroupBox gBoxSettings;

		private Panel pnlPacketMode;

		private RadioButton rBtnPacketModeRx;

		private RadioButton rBtnPacketModeTx;

		private Led ledLogEnabled;

		private Label label13;

		private Panel panel13;

		private RadioButton rBtnLowDatarateOptimizeOff;

		private RadioButton rBtnLowDatarateOptimizeOn;

        public bool IsDebugOn { get; set; }

        public decimal FrequencyXo { get; set; } = 32000000m;

        public decimal SymbolTime
		{
			get => symbolTime;
			set
			{
				symbolTime = value;
				nudSymbTimeout.Increment = symbolTime;
				nudSymbTimeout.Maximum = symbolTime * 1023m;
			}
		}

		public OperatingModeEnum Mode
		{
			get => mode;
			set
			{
				mode = value;
				UpdateControls();
			}
		}

		public bool RxTimeoutMask
		{
			get => rBtnRxTimeoutMaskOn.Checked;
			set
			{
				rBtnRxTimeoutMaskOn.CheckedChanged -= rBtnRxTimeoutMask_CheckedChanged;
				rBtnRxTimeoutMaskOff.CheckedChanged -= rBtnRxTimeoutMask_CheckedChanged;
				if (value)
				{
					rBtnRxTimeoutMaskOn.Checked = true;
					rBtnRxTimeoutMaskOff.Checked = false;
				}
				else
				{
					rBtnRxTimeoutMaskOn.Checked = false;
					rBtnRxTimeoutMaskOff.Checked = true;
				}
				rBtnRxTimeoutMaskOn.CheckedChanged += rBtnRxTimeoutMask_CheckedChanged;
				rBtnRxTimeoutMaskOff.CheckedChanged += rBtnRxTimeoutMask_CheckedChanged;
			}
		}

		public bool RxDoneMask
		{
			get => rBtnRxDoneMaskOn.Checked;
			set
			{
				rBtnRxDoneMaskOn.CheckedChanged -= rBtnRxDoneMask_CheckedChanged;
				rBtnRxDoneMaskOff.CheckedChanged -= rBtnRxDoneMask_CheckedChanged;
				if (value)
				{
					rBtnRxDoneMaskOn.Checked = true;
					rBtnRxDoneMaskOff.Checked = false;
				}
				else
				{
					rBtnRxDoneMaskOn.Checked = false;
					rBtnRxDoneMaskOff.Checked = true;
				}
				rBtnRxDoneMaskOn.CheckedChanged += rBtnRxDoneMask_CheckedChanged;
				rBtnRxDoneMaskOff.CheckedChanged += rBtnRxDoneMask_CheckedChanged;
			}
		}

		public bool PayloadCrcErrorMask
		{
			get => rBtnPayloadCrcErrorMaskOn.Checked;
			set
			{
				rBtnPayloadCrcErrorMaskOn.CheckedChanged -= rBtnPayloadCrcErrorMask_CheckedChanged;
				rBtnPayloadCrcErrorMaskOff.CheckedChanged -= rBtnPayloadCrcErrorMask_CheckedChanged;
				if (value)
				{
					rBtnPayloadCrcErrorMaskOn.Checked = true;
					rBtnPayloadCrcErrorMaskOff.Checked = false;
				}
				else
				{
					rBtnPayloadCrcErrorMaskOn.Checked = false;
					rBtnPayloadCrcErrorMaskOff.Checked = true;
				}
				rBtnPayloadCrcErrorMaskOn.CheckedChanged += rBtnPayloadCrcErrorMask_CheckedChanged;
				rBtnPayloadCrcErrorMaskOff.CheckedChanged += rBtnPayloadCrcErrorMask_CheckedChanged;
			}
		}

		public bool ValidHeaderMask
		{
			get => rBtnValidHeaderMaskOn.Checked;
			set
			{
				rBtnValidHeaderMaskOn.CheckedChanged -= rBtnValidHeaderMask_CheckedChanged;
				rBtnValidHeaderMaskOff.CheckedChanged -= rBtnValidHeaderMask_CheckedChanged;
				if (value)
				{
					rBtnValidHeaderMaskOn.Checked = true;
					rBtnValidHeaderMaskOff.Checked = false;
				}
				else
				{
					rBtnValidHeaderMaskOn.Checked = false;
					rBtnValidHeaderMaskOff.Checked = true;
				}
				rBtnValidHeaderMaskOn.CheckedChanged += rBtnValidHeaderMask_CheckedChanged;
				rBtnValidHeaderMaskOff.CheckedChanged += rBtnValidHeaderMask_CheckedChanged;
			}
		}

		public bool TxDoneMask
		{
			get => rBtnTxDoneMaskOn.Checked;
			set
			{
				rBtnTxDoneMaskOn.CheckedChanged -= rBtnTxDoneMask_CheckedChanged;
				rBtnTxDoneMaskOff.CheckedChanged -= rBtnTxDoneMask_CheckedChanged;
				if (value)
				{
					rBtnTxDoneMaskOn.Checked = true;
					rBtnTxDoneMaskOff.Checked = false;
				}
				else
				{
					rBtnTxDoneMaskOn.Checked = false;
					rBtnTxDoneMaskOff.Checked = true;
				}
				rBtnTxDoneMaskOn.CheckedChanged += rBtnTxDoneMask_CheckedChanged;
				rBtnTxDoneMaskOff.CheckedChanged += rBtnTxDoneMask_CheckedChanged;
			}
		}

		public bool CadDoneMask
		{
			get => rBtnCadDoneMaskOn.Checked;
			set
			{
				rBtnCadDoneMaskOn.CheckedChanged -= rBtnCadDoneMask_CheckedChanged;
				rBtnCadDoneMaskOff.CheckedChanged -= rBtnCadDoneMask_CheckedChanged;
				if (value)
				{
					rBtnCadDoneMaskOn.Checked = true;
					rBtnCadDoneMaskOff.Checked = false;
				}
				else
				{
					rBtnCadDoneMaskOn.Checked = false;
					rBtnCadDoneMaskOff.Checked = true;
				}
				rBtnCadDoneMaskOn.CheckedChanged += rBtnCadDoneMask_CheckedChanged;
				rBtnCadDoneMaskOff.CheckedChanged += rBtnCadDoneMask_CheckedChanged;
			}
		}

		public bool FhssChangeChannelMask
		{
			get => rBtnFhssChangeChannelMaskOn.Checked;
			set
			{
				rBtnFhssChangeChannelMaskOn.CheckedChanged -= rBtnFhssChangeChannelMask_CheckedChanged;
				rBtnFhssChangeChannelMaskOff.CheckedChanged -= rBtnFhssChangeChannelMask_CheckedChanged;
				if (value)
				{
					rBtnFhssChangeChannelMaskOn.Checked = true;
					rBtnFhssChangeChannelMaskOff.Checked = false;
				}
				else
				{
					rBtnFhssChangeChannelMaskOn.Checked = false;
					rBtnFhssChangeChannelMaskOff.Checked = true;
				}
				rBtnFhssChangeChannelMaskOn.CheckedChanged += rBtnFhssChangeChannelMask_CheckedChanged;
				rBtnFhssChangeChannelMaskOff.CheckedChanged += rBtnFhssChangeChannelMask_CheckedChanged;
			}
		}

		public bool CadDetectedMask
		{
			get => rBtnCadDetectedMaskOn.Checked;
			set
			{
				rBtnCadDetectedMaskOn.CheckedChanged -= rBtnCadDetectedMask_CheckedChanged;
				rBtnCadDetectedMaskOff.CheckedChanged -= rBtnCadDetectedMask_CheckedChanged;
				if (value)
				{
					rBtnCadDetectedMaskOn.Checked = true;
					rBtnCadDetectedMaskOff.Checked = false;
				}
				else
				{
					rBtnCadDetectedMaskOn.Checked = false;
					rBtnCadDetectedMaskOff.Checked = true;
				}
				rBtnCadDetectedMaskOn.CheckedChanged += rBtnCadDetectedMask_CheckedChanged;
				rBtnCadDetectedMaskOff.CheckedChanged += rBtnCadDetectedMask_CheckedChanged;
			}
		}

		public bool ImplicitHeaderModeOn
		{
			get => rBtnImplicitHeaderOn.Checked;
			set
			{
				rBtnImplicitHeaderOn.CheckedChanged -= rBtnImplicitHeader_CheckedChanged;
				rBtnImplicitHeaderOff.CheckedChanged -= rBtnImplicitHeader_CheckedChanged;
				if (value)
				{
					rBtnImplicitHeaderOn.Checked = true;
					rBtnImplicitHeaderOff.Checked = false;
					lblRxHeaderInfoHeaderName.Visible = false;
					pnlRxHeaderInfoHeader.Visible = false;
					pnlHeaderInfo.Visible = false;
				}
				else
				{
					rBtnImplicitHeaderOn.Checked = false;
					rBtnImplicitHeaderOff.Checked = true;
					lblRxHeaderInfoHeaderName.Visible = true;
					pnlRxHeaderInfoHeader.Visible = true;
					pnlHeaderInfo.Visible = true;
				}
				rBtnImplicitHeaderOn.CheckedChanged += rBtnImplicitHeader_CheckedChanged;
				rBtnImplicitHeaderOff.CheckedChanged += rBtnImplicitHeader_CheckedChanged;
			}
		}

		public decimal SymbTimeout
		{
			get => nudSymbTimeout.Value;
			set
			{
				try
				{
					nudSymbTimeout.ValueChanged -= nudSymbTimeout_ValueChanged;
					nudSymbTimeout.BackColor = SystemColors.Window;
					var num = (uint)Math.Round(value / SymbolTime, MidpointRounding.AwayFromZero);
					nudSymbTimeout.Value = num * SymbolTime;
				}
				catch (Exception)
				{
					nudSymbTimeout.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudSymbTimeout.ValueChanged += nudSymbTimeout_ValueChanged;
				}
			}
		}

		public bool PayloadCrcOn
		{
			get => rBtnPayloadCrcOn.Checked;
			set
			{
				rBtnPayloadCrcOn.CheckedChanged -= rBtnPayloadCrc_CheckedChanged;
				rBtnPayloadCrcOff.CheckedChanged -= rBtnPayloadCrc_CheckedChanged;
				if (value)
				{
					rBtnPayloadCrcOn.Checked = true;
					rBtnPayloadCrcOff.Checked = false;
				}
				else
				{
					rBtnPayloadCrcOn.Checked = false;
					rBtnPayloadCrcOff.Checked = true;
				}
				rBtnPayloadCrcOn.CheckedChanged += rBtnPayloadCrc_CheckedChanged;
				rBtnPayloadCrcOff.CheckedChanged += rBtnPayloadCrc_CheckedChanged;
			}
		}

		public byte CodingRate
		{
			get => (byte)(cBoxCodingRate.SelectedIndex + 1);
			set
			{
				cBoxCodingRate.SelectedIndexChanged -= cBoxCodingRate_SelectedIndexChanged;
				cBoxCodingRate.SelectedIndex = value - 1;
				cBoxCodingRate.SelectedIndexChanged += cBoxCodingRate_SelectedIndexChanged;
			}
		}

		public byte PayloadLength
		{
			get => (byte)nudPayloadLength.Value;
			set
			{
				try
				{
					nudPayloadLength.ValueChanged -= nudPayloadLength_ValueChanged;
					nudPayloadLength.BackColor = SystemColors.Window;
					nudPayloadLength.Value = value;
				}
				catch (Exception)
				{
					nudPayloadLength.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudPayloadLength.ValueChanged += nudPayloadLength_ValueChanged;
				}
			}
		}

		public int PreambleLength
		{
			get => (int)nudPreambleLength.Value;
			set
			{
				try
				{
					nudPreambleLength.ValueChanged -= nudPreambleLength_ValueChanged;
					nudPreambleLength.BackColor = SystemColors.Window;
					nudPreambleLength.Value = value;
				}
				catch (Exception)
				{
					nudPreambleLength.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudPreambleLength.ValueChanged += nudPreambleLength_ValueChanged;
				}
			}
		}

		public byte Bandwidth
		{
			get => (byte)cBoxBandwidth.SelectedIndex;
			set
			{
				cBoxBandwidth.SelectedIndexChanged -= cBoxBandwidth_SelectedIndexChanged;
				cBoxBandwidth.SelectedIndex = value;
				cBoxBandwidth.SelectedIndexChanged += cBoxBandwidth_SelectedIndexChanged;
			}
		}

		public byte SpreadingFactor
		{
			get => (byte)(cBoxSpreadingFactor.SelectedIndex + 7);
			set
			{
				try
				{
					cBoxSpreadingFactor.SelectedIndexChanged -= cBoxSpreadingFactor_SelectedIndexChanged;
					cBoxSpreadingFactor.SelectedIndex = value - 7;
				}
				catch (Exception)
				{
				}
				finally
				{
					cBoxSpreadingFactor.SelectedIndexChanged += cBoxSpreadingFactor_SelectedIndexChanged;
				}
			}
		}

		public byte FreqHoppingPeriod
		{
			get => (byte)nudFreqHoppingPeriod.Value;
			set
			{
				try
				{
					nudFreqHoppingPeriod.ValueChanged -= nudFreqHoppingPeriod_ValueChanged;
					nudFreqHoppingPeriod.BackColor = SystemColors.Window;
					nudFreqHoppingPeriod.Value = value;
				}
				catch (Exception)
				{
					nudFreqHoppingPeriod.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudFreqHoppingPeriod.ValueChanged += nudFreqHoppingPeriod_ValueChanged;
				}
			}
		}

		public byte RxNbBytes
		{
			set => lblRxNbBytes.Text = value.ToString();
		}

		public bool PllTimeout
		{
			set => ledPllTimeout.Checked = value;
		}

		public bool RxPayloadCrcOn
		{
			set => ledRxPayloadCrcOn.Checked = value;
		}

		public byte RxPayloadCodingRate
		{
			set
			{
				var text = "-";
				switch (value)
				{
				case 1:
					text = "4/5";
					break;
				case 2:
					text = "4/6";
					break;
				case 3:
					text = "4/7";
					break;
				case 4:
					text = "4/8";
					break;
				}
				lblRxPayloadCodingRate.Text = text;
			}
		}

		public ushort ValidHeaderCnt
		{
			set => lblRxValidHeaderCnt.Text = value.ToString();
		}

		public ushort ValidPacketCnt
		{
			set => lblRxPacketCnt.Text = value.ToString();
		}

		public bool ModemClear
		{
			set => ledModemClear.Checked = value;
		}

		public bool HeaderInfoValid
		{
			set => ledHeaderInfoValid.Checked = value;
		}

		public bool RxOnGoing
		{
			set => ledRxOnGoing.Checked = value;
		}

		public bool SignalSynchronized
		{
			set => ledSignalDetected.Checked = value;
		}

		public bool SignalDetected
		{
			set => ledSignalDetected.Checked = value;
		}

		public sbyte PktSnrValue
		{
			set
			{
				if (value > 0 && !IsDebugOn)
				{
					lblPacketSnr.Text = "> 0";
				}
				else
				{
					lblPacketSnr.Text = value.ToString();
				}
			}
		}

		public decimal RssiValue
		{
			set => lblRssiValue.Text = value.ToString("###.0");
		}

		public decimal PktRssiValue
		{
			set => lblPacketRssi.Text = value.ToString("###.0");
		}

		public byte HopChannel
		{
			set => lblHopChannel.Text = value.ToString();
		}

		public byte FifoRxCurrentAddr
		{
			set => lblFifoRxCurrentAddr.Text = value.ToString();
		}

		public byte[] Payload
		{
			get => payload;
			set
			{
				payload = value;
				var dynamicByteProvider = hexBoxPayload.ByteProvider as DynamicByteProvider;
				dynamicByteProvider.Bytes.Clear();
				dynamicByteProvider.Bytes.AddRange(value);
				hexBoxPayload.ByteProvider.ApplyChanges();
				hexBoxPayload.UpdateScrollSize();
				hexBoxPayload.Invalidate();
			}
		}

		public bool LogEnabled
		{
			get => ledLogEnabled.Checked;
			set => ledLogEnabled.Checked = value;
		}

		public bool StartStop
		{
			get => cBtnPacketHandlerStartStop.Checked;
			set => cBtnPacketHandlerStartStop.Checked = value;
		}

		public int PacketNumber
		{
			get => Convert.ToInt32(tBoxPacketsNb.Text);
			set => tBoxPacketsNb.Text = value.ToString();
		}

		public int MaxPacketNumber
		{
			get => Convert.ToInt32(tBoxPacketsRepeatValue.Text);
			set => tBoxPacketsRepeatValue.Text = value.ToString();
		}

		public bool PacketModeTx
		{
			get => rBtnPacketModeTx.Checked;
			set
			{
				rBtnPacketModeTx.CheckedChanged -= rBtnPacketMode_CheckedChanged;
				rBtnPacketModeRx.CheckedChanged -= rBtnPacketMode_CheckedChanged;
				if (value)
				{
					rBtnPacketModeTx.Checked = true;
					rBtnPacketModeRx.Checked = false;
				}
				else
				{
					rBtnPacketModeTx.Checked = false;
					rBtnPacketModeRx.Checked = true;
				}
				UpdateControls();
				rBtnPacketModeTx.CheckedChanged += rBtnPacketMode_CheckedChanged;
				rBtnPacketModeRx.CheckedChanged += rBtnPacketMode_CheckedChanged;
			}
		}

		public bool LowDatarateOptimize
		{
			get => rBtnLowDatarateOptimizeOn.Checked;
			set
			{
				rBtnLowDatarateOptimizeOn.CheckedChanged -= rBtnLowDatarateOptimize_CheckedChanged;
				rBtnLowDatarateOptimizeOff.CheckedChanged -= rBtnLowDatarateOptimize_CheckedChanged;
				if (value)
				{
					rBtnLowDatarateOptimizeOn.Checked = true;
					rBtnLowDatarateOptimizeOff.Checked = false;
				}
				else
				{
					rBtnLowDatarateOptimizeOn.Checked = false;
					rBtnLowDatarateOptimizeOff.Checked = true;
				}
				rBtnLowDatarateOptimizeOn.CheckedChanged += rBtnLowDatarateOptimize_CheckedChanged;
				rBtnLowDatarateOptimizeOff.CheckedChanged += rBtnLowDatarateOptimize_CheckedChanged;
			}
		}

		public event ErrorEventHandler Error;

		public event BooleanEventHandler RxTimeoutMaskChanged;

		public event BooleanEventHandler RxDoneMaskChanged;

		public event BooleanEventHandler PayloadCrcErrorMaskChanged;

		public event BooleanEventHandler ValidHeaderMaskChanged;

		public event BooleanEventHandler TxDoneMaskChanged;

		public event BooleanEventHandler CadDoneMaskChanged;

		public event BooleanEventHandler FhssChangeChannelMaskChanged;

		public event BooleanEventHandler CadDetectedMaskChanged;

		public event BooleanEventHandler ImplicitHeaderModeOnChanged;

		public event DecimalEventHandler SymbTimeoutChanged;

		public event BooleanEventHandler PayloadCrcOnChanged;

		public event ByteEventHandler CodingRateChanged;

		public event ByteEventHandler PayloadLengthChanged;

		public event Int32EventHandler PreambleLengthChanged;

		public event ByteEventHandler BandwidthChanged;

		public event ByteEventHandler SpreadingFactorChanged;

		public event ByteEventHandler FreqHoppingPeriodChanged;

		public event ByteArrayEventHandler MessageChanged;

		public event BooleanEventHandler StartStopChanged;

		public event Int32EventHandler MaxPacketNumberChanged;

		public event EventHandler PacketHandlerLog;

		public event BooleanEventHandler PacketModeTxChanged;

		public event BooleanEventHandler LowDatarateOptimizeChanged;

		public event DocumentationChangedEventHandler DocumentationChanged;

		public LoRaViewControl()
		{
			InitializeComponent();
			payload = [];
			var data = new byte[Payload.Length];
			hexBoxPayload.ByteProvider = new DynamicByteProvider(data);
			hexBoxPayload.ByteProvider.Changed += hexBoxPayload_DataChanged;
			hexBoxPayload.ByteProvider.ApplyChanges();
		}

		private void UpdateControls()
		{
			if (mode == OperatingModeEnum.Sleep || mode == OperatingModeEnum.Stdby)
			{
				cBtnPacketHandlerStartStop.Enabled = true;
				tBoxPacketsNb.Enabled = true;
				if (!cBtnPacketHandlerStartStop.Checked)
				{
					tBoxPacketsRepeatValue.Enabled = true;
					pnlPacketMode.Enabled = true;
				}
			}
			gBoxControl.Enabled = false;
			switch (Mode)
			{
			case OperatingModeEnum.Sleep:
			case OperatingModeEnum.Stdby:
				gBoxControl.Enabled = true;
				if (rBtnPacketModeRx.Checked)
				{
					nudPayloadLength.Enabled = true;
					lblPacketsNb.Text = "Rx packets:";
				}
				else
				{
					nudPayloadLength.Enabled = false;
					lblPacketsNb.Text = "Tx Packets:";
				}
				lblPacketsNb.Visible = true;
				tBoxPacketsNb.Visible = true;
				if (rBtnPacketModeRx.Checked)
				{
					lblPacketsRepeatValue.Visible = false;
					tBoxPacketsRepeatValue.Visible = false;
				}
				else
				{
					lblPacketsRepeatValue.Visible = true;
					tBoxPacketsRepeatValue.Visible = true;
				}
				break;
			case OperatingModeEnum.FsTx:
				nudPayloadLength.Enabled = false;
				lblPacketsNb.Visible = false;
				tBoxPacketsNb.Visible = false;
				lblPacketsRepeatValue.Visible = false;
				tBoxPacketsRepeatValue.Visible = false;
				break;
			case OperatingModeEnum.Tx:
			case OperatingModeEnum.TxContinuous:
				nudPayloadLength.Enabled = false;
				lblPacketsNb.Visible = false;
				tBoxPacketsNb.Visible = false;
				lblPacketsRepeatValue.Visible = false;
				tBoxPacketsRepeatValue.Visible = false;
				break;
			case OperatingModeEnum.FsRx:
				nudPayloadLength.Enabled = true;
				lblPacketsNb.Visible = false;
				tBoxPacketsNb.Visible = false;
				lblPacketsRepeatValue.Visible = false;
				tBoxPacketsRepeatValue.Visible = false;
				break;
			case OperatingModeEnum.Rx:
			case OperatingModeEnum.RxSingle:
			case OperatingModeEnum.Cad:
				nudPayloadLength.Enabled = true;
				lblPacketsNb.Visible = false;
				tBoxPacketsNb.Visible = false;
				lblPacketsRepeatValue.Visible = false;
				tBoxPacketsRepeatValue.Visible = false;
				break;
			}
		}

		private void OnError(byte status, string message)
		{
            Error?.Invoke(this, new ErrorEventArgs(status, message));
        }

		private void OnRxTimeoutMaskChanged(bool value)
		{
            RxTimeoutMaskChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnRxDoneMaskChanged(bool value)
		{
            RxDoneMaskChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnPayloadCrcErrorMaskChanged(bool value)
		{
            PayloadCrcErrorMaskChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnValidHeaderMaskChanged(bool value)
		{
            ValidHeaderMaskChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnTxDoneMaskChanged(bool value)
		{
            TxDoneMaskChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnCadDoneMaskChanged(bool value)
		{
            CadDoneMaskChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnFhssChangeChannelMaskChanged(bool value)
		{
            FhssChangeChannelMaskChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnCadDetectedMaskChanged(bool value)
		{
            CadDetectedMaskChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnImplicitHeaderChanged(bool value)
		{
            ImplicitHeaderModeOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnSymbTimeoutChanged(decimal value)
		{
            SymbTimeoutChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnPayloadCrcOnChanged(bool value)
		{
            PayloadCrcOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnCodingRateChanged(byte value)
		{
            CodingRateChanged?.Invoke(this, new ByteEventArg(value));
        }

		private void OnPayloadLengthChanged(byte value)
		{
            PayloadLengthChanged?.Invoke(this, new ByteEventArg(value));
        }

		private void OnPreambleLengthChanged(int value)
		{
            PreambleLengthChanged?.Invoke(this, new Int32EventArg(value));
        }

		private void OnBandwidthChanged(byte value)
		{
            BandwidthChanged?.Invoke(this, new ByteEventArg(value));
        }

		private void OnSpreadingFactorChanged(byte value)
		{
            SpreadingFactorChanged?.Invoke(this, new ByteEventArg(value));
        }

		private void OnFreqHoppingPeriodChanged(byte value)
		{
            FreqHoppingPeriodChanged?.Invoke(this, new ByteEventArg(value));
        }

		private void OnMessageChanged(byte[] value)
		{
            MessageChanged?.Invoke(this, new ByteArrayEventArg(value));
        }

		private void OnStartStopChanged(bool value)
		{
            StartStopChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnMaxPacketNumberChanged(int value)
		{
            MaxPacketNumberChanged?.Invoke(this, new Int32EventArg(value));
        }

		private void OnPacketHandlerLog()
		{
            PacketHandlerLog?.Invoke(this, EventArgs.Empty);
        }

		private void OnPacketModeTxChanged(bool value)
		{
            PacketModeTxChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnLowDatarateOptimizeChanged(bool value)
		{
            LowDatarateOptimizeChanged?.Invoke(this, new BooleanEventArg(value));
        }

		public void UpdateBandwidthLimits(LimitCheckStatusEnum status, string message)
		{
			errorProvider.SetError(cBoxBandwidth, message);
		}

		private void rBtnRxTimeoutMask_CheckedChanged(object sender, EventArgs e)
		{
			RxTimeoutMask = rBtnRxTimeoutMaskOn.Checked;
			OnRxTimeoutMaskChanged(RxTimeoutMask);
		}

		private void rBtnRxDoneMask_CheckedChanged(object sender, EventArgs e)
		{
			RxDoneMask = rBtnRxDoneMaskOn.Checked;
			OnRxDoneMaskChanged(RxDoneMask);
		}

		private void rBtnPayloadCrcErrorMask_CheckedChanged(object sender, EventArgs e)
		{
			PayloadCrcErrorMask = rBtnPayloadCrcErrorMaskOn.Checked;
			OnPayloadCrcErrorMaskChanged(PayloadCrcErrorMask);
		}

		private void rBtnValidHeaderMask_CheckedChanged(object sender, EventArgs e)
		{
			ValidHeaderMask = rBtnValidHeaderMaskOn.Checked;
			OnValidHeaderMaskChanged(ValidHeaderMask);
		}

		private void rBtnTxDoneMask_CheckedChanged(object sender, EventArgs e)
		{
			TxDoneMask = rBtnTxDoneMaskOn.Checked;
			OnTxDoneMaskChanged(TxDoneMask);
		}

		private void rBtnCadDoneMask_CheckedChanged(object sender, EventArgs e)
		{
			CadDoneMask = rBtnCadDoneMaskOn.Checked;
			OnCadDoneMaskChanged(CadDoneMask);
		}

		private void rBtnFhssChangeChannelMask_CheckedChanged(object sender, EventArgs e)
		{
			FhssChangeChannelMask = rBtnFhssChangeChannelMaskOn.Checked;
			OnFhssChangeChannelMaskChanged(FhssChangeChannelMask);
		}

		private void rBtnCadDetectedMask_CheckedChanged(object sender, EventArgs e)
		{
			CadDetectedMask = rBtnCadDetectedMaskOn.Checked;
			OnCadDetectedMaskChanged(CadDetectedMask);
		}

		private void rBtnImplicitHeader_CheckedChanged(object sender, EventArgs e)
		{
			ImplicitHeaderModeOn = rBtnImplicitHeaderOn.Checked;
			OnImplicitHeaderChanged(ImplicitHeaderModeOn);
		}

		private void nudSymbTimeout_ValueChanged(object sender, EventArgs e)
		{
			SymbTimeout = nudSymbTimeout.Value;
			OnSymbTimeoutChanged(SymbTimeout);
		}

		private void rBtnPayloadCrc_CheckedChanged(object sender, EventArgs e)
		{
			PayloadCrcOn = rBtnPayloadCrcOn.Checked;
			OnPayloadCrcOnChanged(PayloadCrcOn);
		}

		private void cBoxCodingRate_SelectedIndexChanged(object sender, EventArgs e)
		{
			CodingRate = (byte)(cBoxCodingRate.SelectedIndex + 1);
			OnCodingRateChanged(CodingRate);
		}

		private void nudPayloadLength_ValueChanged(object sender, EventArgs e)
		{
			PayloadLength = (byte)nudPayloadLength.Value;
			OnPayloadLengthChanged(PayloadLength);
		}

		private void nudPreambleLength_ValueChanged(object sender, EventArgs e)
		{
			PreambleLength = (int)nudPreambleLength.Value;
			OnPreambleLengthChanged(PreambleLength);
		}

		private void cBoxBandwidth_SelectedIndexChanged(object sender, EventArgs e)
		{
			Bandwidth = (byte)cBoxBandwidth.SelectedIndex;
			OnBandwidthChanged(Bandwidth);
		}

		private void cBoxSpreadingFactor_SelectedIndexChanged(object sender, EventArgs e)
		{
			SpreadingFactor = (byte)(cBoxSpreadingFactor.SelectedIndex + 7);
			OnSpreadingFactorChanged(SpreadingFactor);
		}

		private void nudFreqHoppingPeriod_ValueChanged(object sender, EventArgs e)
		{
			FreqHoppingPeriod = (byte)nudFreqHoppingPeriod.Value;
			OnFreqHoppingPeriodChanged(FreqHoppingPeriod);
		}

		private void hexBoxPayload_DataChanged(object sender, EventArgs e)
		{
			if (!inHexPayloadDataChanged)
			{
				inHexPayloadDataChanged = true;
				if (hexBoxPayload.ByteProvider.Length > 255)
				{
					hexBoxPayload.ByteProvider.DeleteBytes(255L, hexBoxPayload.ByteProvider.Length - 255);
					hexBoxPayload.SelectionStart = 255L;
					hexBoxPayload.SelectionLength = 0L;
				}
				Array.Resize(ref payload, (int)hexBoxPayload.ByteProvider.Length);
				for (var i = 0; i < payload.Length; i++)
				{
					payload[i] = hexBoxPayload.ByteProvider.ReadByte(i);
				}
				OnMessageChanged(Payload);
				inHexPayloadDataChanged = false;
			}
		}

		private void cBtnPacketHandlerStartStop_CheckedChanged(object sender, EventArgs e)
		{
			cBtnPacketHandlerStartStop.Text = cBtnPacketHandlerStartStop.Checked ? "Stop" : "Start";
			gBoxSettings.Enabled = !cBtnPacketHandlerStartStop.Checked;
			gBoxIrqMask.Enabled = !cBtnPacketHandlerStartStop.Checked;
			hexBoxPayload.ReadOnly = cBtnPacketHandlerStartStop.Checked;
			tBoxPacketsRepeatValue.Enabled = !cBtnPacketHandlerStartStop.Checked;
			pnlPacketMode.Enabled = !cBtnPacketHandlerStartStop.Checked;
			btnLog.Enabled = !cBtnPacketHandlerStartStop.Checked;
			try
			{
				MaxPacketNumber = Convert.ToInt32(tBoxPacketsRepeatValue.Text);
			}
			catch
			{
				MaxPacketNumber = 0;
				tBoxPacketsRepeatValue.Text = "0";
				OnError(1, "Wrong max packet value! Value has been reseted.");
			}
			OnMaxPacketNumberChanged(MaxPacketNumber);
			OnStartStopChanged(cBtnPacketHandlerStartStop.Checked);
		}

		private void btnLog_Click(object sender, EventArgs e)
		{
			OnPacketHandlerLog();
		}

		private void rBtnPacketMode_CheckedChanged(object sender, EventArgs e)
		{
			PacketModeTx = rBtnPacketModeTx.Checked;
			OnPacketModeTxChanged(PacketModeTx);
		}

		private void rBtnLowDatarateOptimize_CheckedChanged(object sender, EventArgs e)
		{
			LowDatarateOptimize = rBtnLowDatarateOptimizeOn.Checked;
			OnLowDatarateOptimizeChanged(LowDatarateOptimize);
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
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
			label20 = new Label();
			panel6 = new Panel();
			label21 = new Label();
			label22 = new Label();
			panel7 = new Panel();
			label23 = new Label();
			label24 = new Label();
			label25 = new Label();
			label26 = new Label();
			label27 = new Label();
			label28 = new Label();
			errorProvider = new ErrorProvider(components);
			nudPreambleLength = new NumericUpDownEx();
			cBoxBandwidth = new ComboBox();
			lblListenResolRx = new Label();
			label30 = new Label();
			panel11 = new Panel();
			rBtnImplicitHeaderOff = new RadioButton();
			rBtnImplicitHeaderOn = new RadioButton();
			label8 = new Label();
			label9 = new Label();
			label11 = new Label();
			label12 = new Label();
			panel12 = new Panel();
			rBtnPayloadCrcOff = new RadioButton();
			rBtnPayloadCrcOn = new RadioButton();
			cBoxCodingRate = new ComboBox();
			label15 = new Label();
			label16 = new Label();
			label17 = new Label();
			label19 = new Label();
			label29 = new Label();
			label31 = new Label();
			label32 = new Label();
			label33 = new Label();
			cBoxSpreadingFactor = new ComboBox();
			label34 = new Label();
			label35 = new Label();
			lblRxNbBytes = new Label();
			label38 = new Label();
			label18 = new Label();
			lblRxValidHeaderCnt = new Label();
			label40 = new Label();
			lblRxPacketCnt = new Label();
			label46 = new Label();
			lblPacketSnr = new Label();
			label47 = new Label();
			lblPacketRssi = new Label();
			label48 = new Label();
			lblRssiValue = new Label();
			label49 = new Label();
			lblHopChannel = new Label();
			label50 = new Label();
			lblFifoRxCurrentAddr = new Label();
			pnlPacketStatus = new TableLayoutPanel();
			lblRxPayloadCodingRate = new Label();
			label37 = new Label();
			label39 = new Label();
			pnlHeaderInfo = new TableLayoutPanel();
			lblPllTimeout = new Label();
			ledRxPayloadCrcOn = new Led();
			ledPllTimeout = new Led();
			pnlRxHeaderInfoHeader = new Panel();
			lblRxHeaderInfoHeaderName = new Label();
			pnlPacketStatusHeaderName = new Panel();
			lblPacketStatusHeaderName = new Label();
			gBoxSettings = new GroupBox();
			nudSymbTimeout = new NumericUpDownEx();
			label13 = new Label();
			panel13 = new Panel();
			rBtnLowDatarateOptimizeOff = new RadioButton();
			rBtnLowDatarateOptimizeOn = new RadioButton();
			nudFreqHoppingPeriod = new NumericUpDownEx();
			nudPayloadLength = new NumericUpDownEx();
			gBoxMessage = new GroupBoxEx();
			tblPayloadMessage = new TableLayoutPanel();
			hexBoxPayload = new HexBox();
			label51 = new Label();
			label52 = new Label();
			gBoxControl = new GroupBoxEx();
			ledLogEnabled = new Led();
			pnlPacketMode = new Panel();
			rBtnPacketModeRx = new RadioButton();
			rBtnPacketModeTx = new RadioButton();
			tBoxPacketsNb = new TextBox();
			btnLog = new Button();
			cBtnPacketHandlerStartStop = new CheckBox();
			lblPacketsNb = new Label();
			tBoxPacketsRepeatValue = new TextBox();
			lblPacketsRepeatValue = new Label();
			groupBoxEx1 = new GroupBoxEx();
			label53 = new Label();
			label42 = new Label();
			ledSignalDetected = new Led();
			label45 = new Label();
			ledSignalSynchronized = new Led();
			label43 = new Label();
			ledRxOnGoing = new Led();
			label41 = new Label();
			ledHeaderInfoValid = new Led();
			label44 = new Label();
			ledModemClear = new Led();
			gBoxIrqMask = new GroupBoxEx();
			panel10 = new Panel();
			rBtnCadDetectedMaskOff = new RadioButton();
			rBtnCadDetectedMaskOn = new RadioButton();
			label7 = new Label();
			panel9 = new Panel();
			rBtnFhssChangeChannelMaskOff = new RadioButton();
			rBtnFhssChangeChannelMaskOn = new RadioButton();
			label6 = new Label();
			panel8 = new Panel();
			rBtnCadDoneMaskOff = new RadioButton();
			rBtnCadDoneMaskOn = new RadioButton();
			label5 = new Label();
			panel5 = new Panel();
			rBtnTxDoneMaskOff = new RadioButton();
			rBtnTxDoneMaskOn = new RadioButton();
			label4 = new Label();
			panel3 = new Panel();
			rBtnValidHeaderMaskOff = new RadioButton();
			rBtnValidHeaderMaskOn = new RadioButton();
			label3 = new Label();
			panel2 = new Panel();
			rBtnPayloadCrcErrorMaskOff = new RadioButton();
			rBtnPayloadCrcErrorMaskOn = new RadioButton();
			label2 = new Label();
			panel1 = new Panel();
			rBtnRxDoneMaskOff = new RadioButton();
			rBtnRxDoneMaskOn = new RadioButton();
			label1 = new Label();
			panel4 = new Panel();
			rBtnRxTimeoutMaskOff = new RadioButton();
			rBtnRxTimeoutMaskOn = new RadioButton();
			label10 = new Label();
			((ISupportInitialize)errorProvider).BeginInit();
			((ISupportInitialize)nudPreambleLength).BeginInit();
			panel11.SuspendLayout();
			panel12.SuspendLayout();
			pnlPacketStatus.SuspendLayout();
			pnlHeaderInfo.SuspendLayout();
			gBoxSettings.SuspendLayout();
			((ISupportInitialize)nudSymbTimeout).BeginInit();
			panel13.SuspendLayout();
			((ISupportInitialize)nudFreqHoppingPeriod).BeginInit();
			((ISupportInitialize)nudPayloadLength).BeginInit();
			gBoxMessage.SuspendLayout();
			tblPayloadMessage.SuspendLayout();
			gBoxControl.SuspendLayout();
			pnlPacketMode.SuspendLayout();
			groupBoxEx1.SuspendLayout();
			gBoxIrqMask.SuspendLayout();
			panel10.SuspendLayout();
			panel9.SuspendLayout();
			panel8.SuspendLayout();
			panel5.SuspendLayout();
			panel3.SuspendLayout();
			panel2.SuspendLayout();
			panel1.SuspendLayout();
			panel4.SuspendLayout();
			SuspendLayout();
			label20.Location = new Point(0, 0);
			label20.Name = "label20";
			label20.Size = new Size(100, 23);
			label20.TabIndex = 0;
			panel6.AutoSize = true;
			panel6.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel6.Location = new Point(165, 85);
			panel6.Name = "panel6";
			panel6.Size = new Size(98, 17);
			panel6.TabIndex = 1;
			label21.AutoSize = true;
			label21.Location = new Point(8, 114);
			label21.Name = "label21";
			label21.Size = new Size(105, 13);
			label21.TabIndex = 3;
			label21.Text = "Listen resolution idle:";
			label22.AutoSize = true;
			label22.Location = new Point(295, 112);
			label22.Name = "label22";
			label22.Size = new Size(18, 13);
			label22.TabIndex = 5;
			label22.Text = "µs";
			panel7.Location = new Point(0, 0);
			panel7.Name = "panel7";
			panel7.Size = new Size(200, 100);
			panel7.TabIndex = 0;
			label23.AutoSize = true;
			label23.Location = new Point(8, 167);
			label23.Name = "label23";
			label23.Size = new Size(72, 13);
			label23.TabIndex = 9;
			label23.Text = "Listen criteria:";
			label24.AutoSize = true;
			label24.Location = new Point(8, 217);
			label24.Name = "label24";
			label24.Size = new Size(59, 13);
			label24.TabIndex = 11;
			label24.Text = "Listen end:";
			label25.AutoSize = true;
			label25.Location = new Point(295, 245);
			label25.Name = "label25";
			label25.Size = new Size(20, 13);
			label25.TabIndex = 15;
			label25.Text = "ms";
			label26.AutoSize = true;
			label26.Location = new Point(295, 271);
			label26.Name = "label26";
			label26.Size = new Size(20, 13);
			label26.TabIndex = 18;
			label26.Text = "ms";
			label27.AutoSize = true;
			label27.Location = new Point(8, 245);
			label27.Name = "label27";
			label27.Size = new Size(79, 13);
			label27.TabIndex = 13;
			label27.Text = "Listen idle time:";
			label28.AutoSize = true;
			label28.Location = new Point(8, 270);
			label28.Name = "label28";
			label28.Size = new Size(76, 13);
			label28.TabIndex = 16;
			label28.Text = "Listen Rx time:";
			errorProvider.ContainerControl = this;
			errorProvider.SetIconPadding(nudPreambleLength, 6);
			nudPreambleLength.Location = new Point(368, 23);
			nudPreambleLength.Maximum = new decimal([65539, 0, 0, 0]);
			nudPreambleLength.Minimum = new decimal([4, 0, 0, 0]);
			nudPreambleLength.Name = "nudPreambleLength";
			nudPreambleLength.Size = new Size(124, 20);
			nudPreambleLength.TabIndex = 14;
			nudPreambleLength.Value = new decimal([12, 0, 0, 0]);
			nudPreambleLength.ValueChanged += nudPreambleLength_ValueChanged;
			cBoxBandwidth.DropDownStyle = ComboBoxStyle.DropDownList;
			errorProvider.SetIconPadding(cBoxBandwidth, 30);
			cBoxBandwidth.Items.AddRange(["7.8125", "10.4167", "15.625", "20.8333", "31.25", "41.6667", "62.5", "125", "250", "500"]);
			cBoxBandwidth.Location = new Point(99, 77);
			cBoxBandwidth.Name = "cBoxBandwidth";
			cBoxBandwidth.Size = new Size(124, 21);
			cBoxBandwidth.TabIndex = 5;
			cBoxBandwidth.SelectedIndexChanged += cBoxBandwidth_SelectedIndexChanged;
			lblListenResolRx.AutoSize = true;
			lblListenResolRx.Location = new Point(8, 141);
			lblListenResolRx.Name = "lblListenResolRx";
			lblListenResolRx.Size = new Size(102, 13);
			lblListenResolRx.TabIndex = 6;
			lblListenResolRx.Text = "Listen resolution Rx:";
			label30.AutoSize = true;
			label30.Location = new Point(295, 139);
			label30.Name = "label30";
			label30.Size = new Size(18, 13);
			label30.TabIndex = 8;
			label30.Text = "µs";
			panel11.AutoSize = true;
			panel11.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel11.Controls.Add(rBtnImplicitHeaderOff);
			panel11.Controls.Add(rBtnImplicitHeaderOn);
			panel11.Location = new Point(368, 50);
			panel11.Name = "panel11";
			panel11.Size = new Size(102, 20);
			panel11.TabIndex = 17;
			rBtnImplicitHeaderOff.AutoSize = true;
			rBtnImplicitHeaderOff.Location = new Point(54, 3);
			rBtnImplicitHeaderOff.Margin = new Padding(3, 0, 3, 0);
			rBtnImplicitHeaderOff.Name = "rBtnImplicitHeaderOff";
			rBtnImplicitHeaderOff.Size = new Size(45, 17);
			rBtnImplicitHeaderOff.TabIndex = 1;
			rBtnImplicitHeaderOff.Text = "OFF";
			rBtnImplicitHeaderOff.UseVisualStyleBackColor = true;
			rBtnImplicitHeaderOff.CheckedChanged += rBtnImplicitHeader_CheckedChanged;
			rBtnImplicitHeaderOn.AutoSize = true;
			rBtnImplicitHeaderOn.Checked = true;
			rBtnImplicitHeaderOn.Location = new Point(3, 3);
			rBtnImplicitHeaderOn.Margin = new Padding(3, 0, 3, 0);
			rBtnImplicitHeaderOn.Name = "rBtnImplicitHeaderOn";
			rBtnImplicitHeaderOn.Size = new Size(41, 17);
			rBtnImplicitHeaderOn.TabIndex = 0;
			rBtnImplicitHeaderOn.TabStop = true;
			rBtnImplicitHeaderOn.Text = "ON";
			rBtnImplicitHeaderOn.UseVisualStyleBackColor = true;
			rBtnImplicitHeaderOn.CheckedChanged += rBtnImplicitHeader_CheckedChanged;
			label8.AutoSize = true;
			label8.Location = new Point(276, 54);
			label8.Name = "label8";
			label8.Size = new Size(78, 13);
			label8.TabIndex = 16;
			label8.Text = "Implicit header:";
			label9.AutoSize = true;
			label9.Location = new Point(5, 108);
			label9.Name = "label9";
			label9.Size = new Size(60, 13);
			label9.TabIndex = 7;
			label9.Text = "Rx timeout:";
			label11.AutoSize = true;
			label11.Location = new Point(229, 108);
			label11.Name = "label11";
			label11.Size = new Size(12, 13);
			label11.TabIndex = 9;
			label11.Text = "s";
			label12.AutoSize = true;
			label12.Location = new Point(276, 108);
			label12.Name = "label12";
			label12.Size = new Size(73, 13);
			label12.TabIndex = 21;
			label12.Text = "Payload CRC:";
			panel12.AutoSize = true;
			panel12.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel12.Controls.Add(rBtnPayloadCrcOff);
			panel12.Controls.Add(rBtnPayloadCrcOn);
			panel12.Location = new Point(368, 104);
			panel12.Name = "panel12";
			panel12.Size = new Size(102, 20);
			panel12.TabIndex = 22;
			rBtnPayloadCrcOff.AutoSize = true;
			rBtnPayloadCrcOff.Location = new Point(54, 3);
			rBtnPayloadCrcOff.Margin = new Padding(3, 0, 3, 0);
			rBtnPayloadCrcOff.Name = "rBtnPayloadCrcOff";
			rBtnPayloadCrcOff.Size = new Size(45, 17);
			rBtnPayloadCrcOff.TabIndex = 1;
			rBtnPayloadCrcOff.Text = "OFF";
			rBtnPayloadCrcOff.UseVisualStyleBackColor = true;
			rBtnPayloadCrcOff.CheckedChanged += rBtnPayloadCrc_CheckedChanged;
			rBtnPayloadCrcOn.AutoSize = true;
			rBtnPayloadCrcOn.Checked = true;
			rBtnPayloadCrcOn.Location = new Point(3, 3);
			rBtnPayloadCrcOn.Margin = new Padding(3, 0, 3, 0);
			rBtnPayloadCrcOn.Name = "rBtnPayloadCrcOn";
			rBtnPayloadCrcOn.Size = new Size(41, 17);
			rBtnPayloadCrcOn.TabIndex = 0;
			rBtnPayloadCrcOn.TabStop = true;
			rBtnPayloadCrcOn.Text = "ON";
			rBtnPayloadCrcOn.UseVisualStyleBackColor = true;
			rBtnPayloadCrcOn.CheckedChanged += rBtnPayloadCrc_CheckedChanged;
			cBoxCodingRate.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxCodingRate.Items.AddRange(["4/5", "4/6", "4/7", "4/8"]);
			cBoxCodingRate.Location = new Point(99, 50);
			cBoxCodingRate.Name = "cBoxCodingRate";
			cBoxCodingRate.Size = new Size(124, 21);
			cBoxCodingRate.TabIndex = 3;
			cBoxCodingRate.SelectedIndexChanged += cBoxCodingRate_SelectedIndexChanged;
			label15.AutoSize = true;
			label15.Location = new Point(5, 54);
			label15.Name = "label15";
			label15.Size = new Size(64, 13);
			label15.TabIndex = 2;
			label15.Text = "Coding rate:";
			label16.AutoSize = true;
			label16.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			label16.Location = new Point(276, 81);
			label16.Name = "label16";
			label16.Size = new Size(80, 13);
			label16.TabIndex = 18;
			label16.Text = "Payload length:";
			label16.TextAlign = ContentAlignment.MiddleLeft;
			label17.AutoSize = true;
			label17.Location = new Point(498, 81);
			label17.Name = "label17";
			label17.Size = new Size(32, 13);
			label17.TabIndex = 20;
			label17.Text = "bytes";
			label17.TextAlign = ContentAlignment.MiddleLeft;
			label19.AutoSize = true;
			label19.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			label19.Location = new Point(276, 27);
			label19.Name = "label19";
			label19.Size = new Size(86, 13);
			label19.TabIndex = 13;
			label19.Text = "Preamble length:";
			label19.TextAlign = ContentAlignment.MiddleLeft;
			label29.AutoSize = true;
			label29.Location = new Point(498, 27);
			label29.Name = "label29";
			label29.Size = new Size(44, 13);
			label29.TabIndex = 15;
			label29.Text = "symbols";
			label29.TextAlign = ContentAlignment.MiddleLeft;
			label31.AutoSize = true;
			label31.Location = new Point(5, 81);
			label31.Name = "label31";
			label31.Size = new Size(60, 13);
			label31.TabIndex = 4;
			label31.Text = "Bandwidth:";
			label32.AutoSize = true;
			label32.Location = new Point(229, 81);
			label32.Name = "label32";
			label32.Size = new Size(26, 13);
			label32.TabIndex = 6;
			label32.Text = "kHz";
			label33.AutoSize = true;
			label33.Location = new Point(5, 27);
			label33.Name = "label33";
			label33.Size = new Size(88, 13);
			label33.TabIndex = 0;
			label33.Text = "Spreading factor:";
			cBoxSpreadingFactor.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxSpreadingFactor.Items.AddRange(["SF7", "SF8", "SF9", "SF10", "SF11", "SF12"]);
			cBoxSpreadingFactor.Location = new Point(99, 23);
			cBoxSpreadingFactor.Name = "cBoxSpreadingFactor";
			cBoxSpreadingFactor.Size = new Size(124, 21);
			cBoxSpreadingFactor.TabIndex = 1;
			cBoxSpreadingFactor.SelectedIndexChanged += cBoxSpreadingFactor_SelectedIndexChanged;
			label34.AutoSize = true;
			label34.Location = new Point(229, 170);
			label34.Name = "label34";
			label34.Size = new Size(76, 13);
			label34.TabIndex = 12;
			label34.Text = "symbol periods";
			label34.TextAlign = ContentAlignment.MiddleLeft;
			label34.Visible = false;
			label35.AutoSize = true;
			label35.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			label35.Location = new Point(5, 163);
			label35.MaximumSize = new Size(94, 0);
			label35.Name = "label35";
			label35.Size = new Size(80, 26);
			label35.TabIndex = 10;
			label35.Text = "Frequency hopping period:";
			label35.TextAlign = ContentAlignment.MiddleLeft;
			label35.Visible = false;
			lblRxNbBytes.Anchor = AnchorStyles.None;
			lblRxNbBytes.BorderStyle = BorderStyle.Fixed3D;
			lblRxNbBytes.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblRxNbBytes.Location = new Point(407, 37);
			lblRxNbBytes.Margin = new Padding(3);
			lblRxNbBytes.Name = "lblRxNbBytes";
			lblRxNbBytes.Size = new Size(59, 20);
			lblRxNbBytes.TabIndex = 9;
			lblRxNbBytes.Text = "-";
			lblRxNbBytes.TextAlign = ContentAlignment.MiddleCenter;
			label38.Anchor = AnchorStyles.None;
			label38.AutoSize = true;
			label38.Location = new Point(392, 4);
			label38.Margin = new Padding(3);
			label38.MaximumSize = new Size(90, 0);
			label38.MinimumSize = new Size(90, 0);
			label38.Name = "label38";
			label38.Size = new Size(90, 26);
			label38.TabIndex = 4;
			label38.Text = "Number of bytes received";
			label38.TextAlign = ContentAlignment.MiddleCenter;
			label18.Anchor = AnchorStyles.None;
			label18.AutoSize = true;
			label18.Location = new Point(4, 4);
			label18.Margin = new Padding(3);
			label18.MaximumSize = new Size(90, 0);
			label18.MinimumSize = new Size(90, 0);
			label18.Name = "label18";
			label18.Size = new Size(90, 26);
			label18.TabIndex = 0;
			label18.Text = "Received valid header count";
			label18.TextAlign = ContentAlignment.MiddleCenter;
			lblRxValidHeaderCnt.Anchor = AnchorStyles.None;
			lblRxValidHeaderCnt.BorderStyle = BorderStyle.Fixed3D;
			lblRxValidHeaderCnt.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblRxValidHeaderCnt.Location = new Point(19, 37);
			lblRxValidHeaderCnt.Margin = new Padding(3);
			lblRxValidHeaderCnt.Name = "lblRxValidHeaderCnt";
			lblRxValidHeaderCnt.Size = new Size(59, 20);
			lblRxValidHeaderCnt.TabIndex = 5;
			lblRxValidHeaderCnt.Text = "-";
			lblRxValidHeaderCnt.TextAlign = ContentAlignment.MiddleCenter;
			label40.Anchor = AnchorStyles.None;
			label40.AutoSize = true;
			label40.Location = new Point(100, 4);
			label40.Margin = new Padding(3);
			label40.MaximumSize = new Size(90, 0);
			label40.MinimumSize = new Size(90, 0);
			label40.Name = "label40";
			label40.Size = new Size(90, 26);
			label40.TabIndex = 1;
			label40.Text = "Received valid packet count";
			label40.TextAlign = ContentAlignment.MiddleCenter;
			lblRxPacketCnt.Anchor = AnchorStyles.None;
			lblRxPacketCnt.BorderStyle = BorderStyle.Fixed3D;
			lblRxPacketCnt.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblRxPacketCnt.Location = new Point(115, 37);
			lblRxPacketCnt.Margin = new Padding(3);
			lblRxPacketCnt.Name = "lblRxPacketCnt";
			lblRxPacketCnt.Size = new Size(59, 20);
			lblRxPacketCnt.TabIndex = 7;
			lblRxPacketCnt.Text = "-";
			lblRxPacketCnt.TextAlign = ContentAlignment.MiddleCenter;
			label46.Anchor = AnchorStyles.None;
			label46.AutoSize = true;
			label46.Location = new Point(292, 4);
			label46.Margin = new Padding(3);
			label46.MaximumSize = new Size(90, 0);
			label46.MinimumSize = new Size(90, 0);
			label46.Name = "label46";
			label46.Size = new Size(90, 26);
			label46.TabIndex = 3;
			label46.Text = "Received packet SNR [dB]";
			label46.TextAlign = ContentAlignment.MiddleCenter;
			lblPacketSnr.Anchor = AnchorStyles.None;
			lblPacketSnr.BorderStyle = BorderStyle.Fixed3D;
			lblPacketSnr.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblPacketSnr.Location = new Point(307, 37);
			lblPacketSnr.Margin = new Padding(3);
			lblPacketSnr.Name = "lblPacketSnr";
			lblPacketSnr.Size = new Size(59, 20);
			lblPacketSnr.TabIndex = 9;
			lblPacketSnr.Text = "-";
			lblPacketSnr.TextAlign = ContentAlignment.MiddleCenter;
			label47.Anchor = AnchorStyles.None;
			label47.AutoSize = true;
			label47.Location = new Point(388, 4);
			label47.Margin = new Padding(3);
			label47.MaximumSize = new Size(90, 0);
			label47.MinimumSize = new Size(90, 0);
			label47.Name = "label47";
			label47.Size = new Size(90, 26);
			label47.TabIndex = 4;
			label47.Text = "Received packet RSSI [dBm]";
			label47.TextAlign = ContentAlignment.MiddleCenter;
			lblPacketRssi.Anchor = AnchorStyles.None;
			lblPacketRssi.BorderStyle = BorderStyle.Fixed3D;
			lblPacketRssi.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblPacketRssi.Location = new Point(403, 37);
			lblPacketRssi.Margin = new Padding(3);
			lblPacketRssi.Name = "lblPacketRssi";
			lblPacketRssi.Size = new Size(59, 20);
			lblPacketRssi.TabIndex = 10;
			lblPacketRssi.Text = "-";
			lblPacketRssi.TextAlign = ContentAlignment.MiddleCenter;
			label48.Anchor = AnchorStyles.None;
			label48.AutoSize = true;
			label48.Location = new Point(486, 4);
			label48.Margin = new Padding(3);
			label48.MaximumSize = new Size(90, 0);
			label48.MinimumSize = new Size(90, 0);
			label48.Name = "label48";
			label48.Size = new Size(90, 26);
			label48.TabIndex = 5;
			label48.Text = "Current RSSI value [dBm]";
			label48.TextAlign = ContentAlignment.MiddleCenter;
			lblRssiValue.Anchor = AnchorStyles.None;
			lblRssiValue.BorderStyle = BorderStyle.Fixed3D;
			lblRssiValue.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblRssiValue.Location = new Point(502, 37);
			lblRssiValue.Margin = new Padding(3);
			lblRssiValue.Name = "lblRssiValue";
			lblRssiValue.Size = new Size(59, 20);
			lblRssiValue.TabIndex = 11;
			lblRssiValue.Text = "-";
			lblRssiValue.TextAlign = ContentAlignment.MiddleCenter;
			label49.Anchor = AnchorStyles.None;
			label49.AutoSize = true;
			label49.Location = new Point(4, 4);
			label49.Margin = new Padding(3);
			label49.MaximumSize = new Size(90, 0);
			label49.MinimumSize = new Size(90, 0);
			label49.Name = "label49";
			label49.Size = new Size(90, 26);
			label49.TabIndex = 0;
			label49.Text = "Current hopping channel";
			label49.TextAlign = ContentAlignment.MiddleCenter;
			lblHopChannel.Anchor = AnchorStyles.None;
			lblHopChannel.BorderStyle = BorderStyle.Fixed3D;
			lblHopChannel.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblHopChannel.Location = new Point(19, 37);
			lblHopChannel.Margin = new Padding(3);
			lblHopChannel.Name = "lblHopChannel";
			lblHopChannel.Size = new Size(59, 20);
			lblHopChannel.TabIndex = 6;
			lblHopChannel.Text = "-";
			lblHopChannel.TextAlign = ContentAlignment.MiddleCenter;
			label50.Anchor = AnchorStyles.None;
			label50.AutoSize = true;
			label50.Location = new Point(196, 4);
			label50.Margin = new Padding(3);
			label50.MaximumSize = new Size(90, 0);
			label50.MinimumSize = new Size(90, 0);
			label50.Name = "label50";
			label50.Size = new Size(90, 26);
			label50.TabIndex = 2;
			label50.Text = "Rx databuffer address";
			label50.TextAlign = ContentAlignment.MiddleCenter;
			lblFifoRxCurrentAddr.Anchor = AnchorStyles.None;
			lblFifoRxCurrentAddr.BorderStyle = BorderStyle.Fixed3D;
			lblFifoRxCurrentAddr.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblFifoRxCurrentAddr.Location = new Point(211, 37);
			lblFifoRxCurrentAddr.Margin = new Padding(3);
			lblFifoRxCurrentAddr.Name = "lblFifoRxCurrentAddr";
			lblFifoRxCurrentAddr.Size = new Size(59, 20);
			lblFifoRxCurrentAddr.TabIndex = 8;
			lblFifoRxCurrentAddr.Text = "-";
			lblFifoRxCurrentAddr.TextAlign = ContentAlignment.MiddleCenter;
			pnlPacketStatus.AutoSize = true;
			pnlPacketStatus.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlPacketStatus.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			pnlPacketStatus.ColumnCount = 6;
			pnlPacketStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			pnlPacketStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			pnlPacketStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			pnlPacketStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			pnlPacketStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			pnlPacketStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			pnlPacketStatus.Controls.Add(label49, 0, 0);
			pnlPacketStatus.Controls.Add(lblHopChannel, 0, 1);
			pnlPacketStatus.Controls.Add(lblRssiValue, 5, 1);
			pnlPacketStatus.Controls.Add(lblPacketRssi, 4, 1);
			pnlPacketStatus.Controls.Add(label48, 5, 0);
			pnlPacketStatus.Controls.Add(label47, 4, 0);
			pnlPacketStatus.Controls.Add(lblPacketSnr, 3, 1);
			pnlPacketStatus.Controls.Add(label46, 3, 0);
			pnlPacketStatus.Controls.Add(lblRxPacketCnt, 1, 1);
			pnlPacketStatus.Controls.Add(label50, 2, 0);
			pnlPacketStatus.Controls.Add(label40, 1, 0);
			pnlPacketStatus.Controls.Add(lblFifoRxCurrentAddr, 2, 1);
			pnlPacketStatus.Location = new Point(108, 326);
			pnlPacketStatus.Name = "pnlPacketStatus";
			pnlPacketStatus.RowCount = 2;
			pnlPacketStatus.RowStyles.Add(new RowStyle());
			pnlPacketStatus.RowStyles.Add(new RowStyle());
			pnlPacketStatus.Size = new Size(583, 61);
			pnlPacketStatus.TabIndex = 7;
			lblRxPayloadCodingRate.Anchor = AnchorStyles.None;
			lblRxPayloadCodingRate.BorderStyle = BorderStyle.Fixed3D;
			lblRxPayloadCodingRate.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblRxPayloadCodingRate.Location = new Point(310, 37);
			lblRxPayloadCodingRate.Margin = new Padding(3);
			lblRxPayloadCodingRate.Name = "lblRxPayloadCodingRate";
			lblRxPayloadCodingRate.Size = new Size(59, 20);
			lblRxPayloadCodingRate.TabIndex = 8;
			lblRxPayloadCodingRate.Text = "-";
			lblRxPayloadCodingRate.TextAlign = ContentAlignment.MiddleCenter;
			label37.Anchor = AnchorStyles.None;
			label37.AutoSize = true;
			label37.Location = new Point(295, 4);
			label37.Margin = new Padding(3);
			label37.MaximumSize = new Size(90, 0);
			label37.MinimumSize = new Size(90, 0);
			label37.Name = "label37";
			label37.Size = new Size(90, 26);
			label37.TabIndex = 3;
			label37.Text = "Rx payload coding rate";
			label37.TextAlign = ContentAlignment.MiddleCenter;
			label39.Anchor = AnchorStyles.None;
			label39.AutoSize = true;
			label39.Location = new Point(198, 10);
			label39.Margin = new Padding(3);
			label39.MaximumSize = new Size(90, 0);
			label39.MinimumSize = new Size(90, 0);
			label39.Name = "label39";
			label39.Size = new Size(90, 13);
			label39.TabIndex = 2;
			label39.Text = "Rx payload CRC";
			label39.TextAlign = ContentAlignment.MiddleCenter;
			pnlHeaderInfo.AutoSize = true;
			pnlHeaderInfo.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlHeaderInfo.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			pnlHeaderInfo.ColumnCount = 5;
			pnlHeaderInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
			pnlHeaderInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
			pnlHeaderInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
			pnlHeaderInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
			pnlHeaderInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
			pnlHeaderInfo.Controls.Add(label37, 3, 0);
			pnlHeaderInfo.Controls.Add(lblRxPayloadCodingRate, 3, 1);
			pnlHeaderInfo.Controls.Add(lblPllTimeout, 1, 0);
			pnlHeaderInfo.Controls.Add(ledRxPayloadCrcOn, 2, 1);
			pnlHeaderInfo.Controls.Add(ledPllTimeout, 1, 1);
			pnlHeaderInfo.Controls.Add(label39, 2, 0);
			pnlHeaderInfo.Controls.Add(label18, 0, 0);
			pnlHeaderInfo.Controls.Add(lblRxValidHeaderCnt, 0, 1);
			pnlHeaderInfo.Controls.Add(label38, 4, 0);
			pnlHeaderInfo.Controls.Add(lblRxNbBytes, 4, 1);
			pnlHeaderInfo.Location = new Point(156, 245);
			pnlHeaderInfo.Name = "pnlHeaderInfo";
			pnlHeaderInfo.RowCount = 2;
			pnlHeaderInfo.RowStyles.Add(new RowStyle());
			pnlHeaderInfo.RowStyles.Add(new RowStyle());
			pnlHeaderInfo.Size = new Size(486, 61);
			pnlHeaderInfo.TabIndex = 4;
			lblPllTimeout.Anchor = AnchorStyles.None;
			lblPllTimeout.AutoSize = true;
			lblPllTimeout.Location = new Point(101, 10);
			lblPllTimeout.Margin = new Padding(3);
			lblPllTimeout.MaximumSize = new Size(90, 0);
			lblPllTimeout.MinimumSize = new Size(90, 0);
			lblPllTimeout.Name = "lblPllTimeout";
			lblPllTimeout.Size = new Size(90, 13);
			lblPllTimeout.TabIndex = 1;
			lblPllTimeout.Text = "PLL timeout";
			lblPllTimeout.TextAlign = ContentAlignment.MiddleCenter;
			ledRxPayloadCrcOn.Anchor = AnchorStyles.None;
			ledRxPayloadCrcOn.BackColor = Color.Transparent;
			ledRxPayloadCrcOn.LedColor = Color.Green;
			ledRxPayloadCrcOn.LedSize = new Size(11, 11);
			ledRxPayloadCrcOn.Location = new Point(235, 39);
			ledRxPayloadCrcOn.Name = "ledRxPayloadCrcOn";
			ledRxPayloadCrcOn.Size = new Size(15, 15);
			ledRxPayloadCrcOn.TabIndex = 7;
			ledRxPayloadCrcOn.Text = "Rx payload CRC on";
			ledPllTimeout.Anchor = AnchorStyles.None;
			ledPllTimeout.BackColor = Color.Transparent;
			ledPllTimeout.LedColor = Color.Green;
			ledPllTimeout.LedSize = new Size(11, 11);
			ledPllTimeout.Location = new Point(138, 39);
			ledPllTimeout.Name = "ledPllTimeout";
			ledPllTimeout.Size = new Size(15, 15);
			ledPllTimeout.TabIndex = 6;
			ledPllTimeout.Text = "PLL timeout";
			pnlRxHeaderInfoHeader.BorderStyle = BorderStyle.FixedSingle;
			pnlRxHeaderInfoHeader.Location = new Point(85, 234);
			pnlRxHeaderInfoHeader.Name = "pnlRxHeaderInfoHeader";
			pnlRxHeaderInfoHeader.Size = new Size(710, 1);
			pnlRxHeaderInfoHeader.TabIndex = 3;
			lblRxHeaderInfoHeaderName.AutoSize = true;
			lblRxHeaderInfoHeaderName.Location = new Point(3, 228);
			lblRxHeaderInfoHeaderName.Name = "lblRxHeaderInfoHeaderName";
			lblRxHeaderInfoHeaderName.Size = new Size(76, 13);
			lblRxHeaderInfoHeaderName.TabIndex = 2;
			lblRxHeaderInfoHeaderName.Text = "Rx header info";
			pnlPacketStatusHeaderName.BorderStyle = BorderStyle.FixedSingle;
			pnlPacketStatusHeaderName.Location = new Point(85, 315);
			pnlPacketStatusHeaderName.Name = "pnlPacketStatusHeaderName";
			pnlPacketStatusHeaderName.Size = new Size(710, 1);
			pnlPacketStatusHeaderName.TabIndex = 6;
			lblPacketStatusHeaderName.AutoSize = true;
			lblPacketStatusHeaderName.Location = new Point(3, 309);
			lblPacketStatusHeaderName.Name = "lblPacketStatusHeaderName";
			lblPacketStatusHeaderName.Size = new Size(72, 13);
			lblPacketStatusHeaderName.TabIndex = 5;
			lblPacketStatusHeaderName.Text = "Packet status";
			gBoxSettings.Controls.Add(label33);
			gBoxSettings.Controls.Add(nudSymbTimeout);
			gBoxSettings.Controls.Add(label9);
			gBoxSettings.Controls.Add(label11);
			gBoxSettings.Controls.Add(label8);
			gBoxSettings.Controls.Add(panel11);
			gBoxSettings.Controls.Add(label35);
			gBoxSettings.Controls.Add(label13);
			gBoxSettings.Controls.Add(label12);
			gBoxSettings.Controls.Add(label19);
			gBoxSettings.Controls.Add(panel13);
			gBoxSettings.Controls.Add(panel12);
			gBoxSettings.Controls.Add(nudFreqHoppingPeriod);
			gBoxSettings.Controls.Add(label34);
			gBoxSettings.Controls.Add(nudPreambleLength);
			gBoxSettings.Controls.Add(label15);
			gBoxSettings.Controls.Add(label29);
			gBoxSettings.Controls.Add(label31);
			gBoxSettings.Controls.Add(nudPayloadLength);
			gBoxSettings.Controls.Add(cBoxCodingRate);
			gBoxSettings.Controls.Add(label16);
			gBoxSettings.Controls.Add(label32);
			gBoxSettings.Controls.Add(label17);
			gBoxSettings.Controls.Add(cBoxBandwidth);
			gBoxSettings.Controls.Add(cBoxSpreadingFactor);
			gBoxSettings.Location = new Point(3, 3);
			gBoxSettings.Name = "gBoxSettings";
			gBoxSettings.Size = new Size(548, 225);
			gBoxSettings.TabIndex = 0;
			gBoxSettings.TabStop = false;
			gBoxSettings.Text = "Settings";
			nudSymbTimeout.DecimalPlaces = 6;
			nudSymbTimeout.Increment = new decimal([1024, 0, 0, 393216]);
			nudSymbTimeout.Location = new Point(99, 104);
			nudSymbTimeout.Maximum = new decimal([1047552, 0, 0, 393216]);
			nudSymbTimeout.Name = "nudSymbTimeout";
			nudSymbTimeout.Size = new Size(124, 20);
			nudSymbTimeout.TabIndex = 8;
			nudSymbTimeout.ThousandsSeparator = true;
			nudSymbTimeout.Value = new decimal([1024, 0, 0, 262144]);
			nudSymbTimeout.ValueChanged += nudSymbTimeout_ValueChanged;
			label13.AutoSize = true;
			label13.Location = new Point(5, 131);
			label13.MaximumSize = new Size(94, 0);
			label13.Name = "label13";
			label13.Size = new Size(72, 26);
			label13.TabIndex = 21;
			label13.Text = "Low datarate optimize:";
			panel13.AutoSize = true;
			panel13.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel13.Controls.Add(rBtnLowDatarateOptimizeOff);
			panel13.Controls.Add(rBtnLowDatarateOptimizeOn);
			panel13.Location = new Point(99, 134);
			panel13.Name = "panel13";
			panel13.Size = new Size(102, 20);
			panel13.TabIndex = 22;
			rBtnLowDatarateOptimizeOff.AutoSize = true;
			rBtnLowDatarateOptimizeOff.Location = new Point(54, 3);
			rBtnLowDatarateOptimizeOff.Margin = new Padding(3, 0, 3, 0);
			rBtnLowDatarateOptimizeOff.Name = "rBtnLowDatarateOptimizeOff";
			rBtnLowDatarateOptimizeOff.Size = new Size(45, 17);
			rBtnLowDatarateOptimizeOff.TabIndex = 1;
			rBtnLowDatarateOptimizeOff.Text = "OFF";
			rBtnLowDatarateOptimizeOff.UseVisualStyleBackColor = true;
			rBtnLowDatarateOptimizeOff.CheckedChanged += rBtnLowDatarateOptimize_CheckedChanged;
			rBtnLowDatarateOptimizeOn.AutoSize = true;
			rBtnLowDatarateOptimizeOn.Checked = true;
			rBtnLowDatarateOptimizeOn.Location = new Point(3, 3);
			rBtnLowDatarateOptimizeOn.Margin = new Padding(3, 0, 3, 0);
			rBtnLowDatarateOptimizeOn.Name = "rBtnLowDatarateOptimizeOn";
			rBtnLowDatarateOptimizeOn.Size = new Size(41, 17);
			rBtnLowDatarateOptimizeOn.TabIndex = 0;
			rBtnLowDatarateOptimizeOn.TabStop = true;
			rBtnLowDatarateOptimizeOn.Text = "ON";
			rBtnLowDatarateOptimizeOn.UseVisualStyleBackColor = true;
			rBtnLowDatarateOptimizeOn.CheckedChanged += rBtnLowDatarateOptimize_CheckedChanged;
			nudFreqHoppingPeriod.Location = new Point(99, 166);
			nudFreqHoppingPeriod.Maximum = new decimal([255, 0, 0, 0]);
			nudFreqHoppingPeriod.Name = "nudFreqHoppingPeriod";
			nudFreqHoppingPeriod.Size = new Size(124, 20);
			nudFreqHoppingPeriod.TabIndex = 11;
			nudFreqHoppingPeriod.Visible = false;
			nudFreqHoppingPeriod.ValueChanged += nudFreqHoppingPeriod_ValueChanged;
			nudPayloadLength.Location = new Point(368, 77);
			nudPayloadLength.Maximum = new decimal([256, 0, 0, 0]);
			nudPayloadLength.Name = "nudPayloadLength";
			nudPayloadLength.Size = new Size(124, 20);
			nudPayloadLength.TabIndex = 19;
			nudPayloadLength.Value = new decimal([14, 0, 0, 0]);
			nudPayloadLength.ValueChanged += nudPayloadLength_ValueChanged;
			gBoxMessage.Controls.Add(tblPayloadMessage);
			gBoxMessage.Location = new Point(3, 393);
			gBoxMessage.Name = "gBoxMessage";
			gBoxMessage.Size = new Size(528, 97);
			gBoxMessage.TabIndex = 8;
			gBoxMessage.TabStop = false;
			gBoxMessage.Text = "Message";
			tblPayloadMessage.AutoSize = true;
			tblPayloadMessage.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			tblPayloadMessage.ColumnCount = 2;
			tblPayloadMessage.ColumnStyles.Add(new ColumnStyle());
			tblPayloadMessage.ColumnStyles.Add(new ColumnStyle());
			tblPayloadMessage.Controls.Add(hexBoxPayload, 0, 1);
			tblPayloadMessage.Controls.Add(label51, 1, 0);
			tblPayloadMessage.Controls.Add(label52, 0, 0);
			tblPayloadMessage.Location = new Point(11, 16);
			tblPayloadMessage.Name = "tblPayloadMessage";
			tblPayloadMessage.RowCount = 2;
			tblPayloadMessage.RowStyles.Add(new RowStyle());
			tblPayloadMessage.RowStyles.Add(new RowStyle());
			tblPayloadMessage.Size = new Size(507, 79);
			tblPayloadMessage.TabIndex = 0;
			hexBoxPayload.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			tblPayloadMessage.SetColumnSpan(hexBoxPayload, 2);
			hexBoxPayload.Font = new Font("Courier New", 8.25f);
			hexBoxPayload.LineInfoDigits = 2;
			hexBoxPayload.LineInfoForeColor = Color.Empty;
			hexBoxPayload.Location = new Point(3, 16);
			hexBoxPayload.Name = "hexBoxPayload";
			hexBoxPayload.ShadowSelectionColor = Color.FromArgb(100, 60, 188, 255);
			hexBoxPayload.Size = new Size(501, 60);
			hexBoxPayload.StringViewVisible = true;
			hexBoxPayload.TabIndex = 2;
			hexBoxPayload.UseFixedBytesPerLine = true;
			hexBoxPayload.VScrollBarVisible = true;
			label51.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label51.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label51.Location = new Point(329, 0);
			label51.Name = "label51";
			label51.Size = new Size(175, 13);
			label51.TabIndex = 1;
			label51.Text = "ASCII";
			label51.TextAlign = ContentAlignment.MiddleCenter;
			label52.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label52.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label52.Location = new Point(3, 0);
			label52.Name = "label52";
			label52.Size = new Size(320, 13);
			label52.TabIndex = 0;
			label52.Text = "HEXADECIMAL";
			label52.TextAlign = ContentAlignment.MiddleCenter;
			gBoxControl.Controls.Add(ledLogEnabled);
			gBoxControl.Controls.Add(pnlPacketMode);
			gBoxControl.Controls.Add(tBoxPacketsNb);
			gBoxControl.Controls.Add(btnLog);
			gBoxControl.Controls.Add(cBtnPacketHandlerStartStop);
			gBoxControl.Controls.Add(lblPacketsNb);
			gBoxControl.Controls.Add(tBoxPacketsRepeatValue);
			gBoxControl.Controls.Add(lblPacketsRepeatValue);
			gBoxControl.Location = new Point(537, 393);
			gBoxControl.Name = "gBoxControl";
			gBoxControl.Size = new Size(259, 97);
			gBoxControl.TabIndex = 9;
			gBoxControl.TabStop = false;
			gBoxControl.Text = "Packet Control";
			ledLogEnabled.BackColor = Color.Transparent;
			ledLogEnabled.LedColor = Color.Green;
			ledLogEnabled.LedSize = new Size(11, 11);
			ledLogEnabled.Location = new Point(143, 23);
			ledLogEnabled.Name = "ledLogEnabled";
			ledLogEnabled.Size = new Size(15, 15);
			ledLogEnabled.TabIndex = 2;
			ledLogEnabled.Text = "Log status";
			pnlPacketMode.AutoSize = true;
			pnlPacketMode.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlPacketMode.Controls.Add(rBtnPacketModeRx);
			pnlPacketMode.Controls.Add(rBtnPacketModeTx);
			pnlPacketMode.Location = new Point(168, 19);
			pnlPacketMode.Name = "pnlPacketMode";
			pnlPacketMode.Size = new Size(87, 20);
			pnlPacketMode.TabIndex = 3;
			rBtnPacketModeRx.AutoSize = true;
			rBtnPacketModeRx.Checked = true;
			rBtnPacketModeRx.Location = new Point(46, 3);
			rBtnPacketModeRx.Margin = new Padding(3, 0, 3, 0);
			rBtnPacketModeRx.Name = "rBtnPacketModeRx";
			rBtnPacketModeRx.Size = new Size(38, 17);
			rBtnPacketModeRx.TabIndex = 1;
			rBtnPacketModeRx.TabStop = true;
			rBtnPacketModeRx.Text = "Rx";
			rBtnPacketModeRx.UseVisualStyleBackColor = true;
			rBtnPacketModeRx.CheckedChanged += rBtnPacketMode_CheckedChanged;
			rBtnPacketModeTx.AutoSize = true;
			rBtnPacketModeTx.Location = new Point(3, 3);
			rBtnPacketModeTx.Margin = new Padding(3, 0, 3, 0);
			rBtnPacketModeTx.Name = "rBtnPacketModeTx";
			rBtnPacketModeTx.Size = new Size(37, 17);
			rBtnPacketModeTx.TabIndex = 0;
			rBtnPacketModeTx.Text = "Tx";
			rBtnPacketModeTx.UseVisualStyleBackColor = true;
			rBtnPacketModeTx.CheckedChanged += rBtnPacketMode_CheckedChanged;
			tBoxPacketsNb.Location = new Point(149, 48);
			tBoxPacketsNb.Name = "tBoxPacketsNb";
			tBoxPacketsNb.ReadOnly = true;
			tBoxPacketsNb.Size = new Size(79, 20);
			tBoxPacketsNb.TabIndex = 5;
			tBoxPacketsNb.Text = "0";
			tBoxPacketsNb.TextAlign = HorizontalAlignment.Right;
			btnLog.Location = new Point(87, 19);
			btnLog.Name = "btnLog";
			btnLog.Size = new Size(75, 23);
			btnLog.TabIndex = 1;
			btnLog.Text = "Log";
			btnLog.Click += btnLog_Click;
			cBtnPacketHandlerStartStop.Appearance = Appearance.Button;
			cBtnPacketHandlerStartStop.Location = new Point(6, 19);
			cBtnPacketHandlerStartStop.Name = "cBtnPacketHandlerStartStop";
			cBtnPacketHandlerStartStop.Size = new Size(75, 23);
			cBtnPacketHandlerStartStop.TabIndex = 0;
			cBtnPacketHandlerStartStop.Text = "Start";
			cBtnPacketHandlerStartStop.TextAlign = ContentAlignment.MiddleCenter;
			cBtnPacketHandlerStartStop.UseVisualStyleBackColor = true;
			cBtnPacketHandlerStartStop.CheckedChanged += cBtnPacketHandlerStartStop_CheckedChanged;
			lblPacketsNb.AutoSize = true;
			lblPacketsNb.Location = new Point(3, 51);
			lblPacketsNb.Name = "lblPacketsNb";
			lblPacketsNb.Size = new Size(64, 13);
			lblPacketsNb.TabIndex = 4;
			lblPacketsNb.Text = "Tx Packets:";
			lblPacketsNb.TextAlign = ContentAlignment.MiddleLeft;
			tBoxPacketsRepeatValue.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			tBoxPacketsRepeatValue.Location = new Point(149, 70);
			tBoxPacketsRepeatValue.Name = "tBoxPacketsRepeatValue";
			tBoxPacketsRepeatValue.Size = new Size(79, 20);
			tBoxPacketsRepeatValue.TabIndex = 7;
			tBoxPacketsRepeatValue.Text = "0";
			tBoxPacketsRepeatValue.TextAlign = HorizontalAlignment.Right;
			lblPacketsRepeatValue.AutoSize = true;
			lblPacketsRepeatValue.Location = new Point(3, 73);
			lblPacketsRepeatValue.Name = "lblPacketsRepeatValue";
			lblPacketsRepeatValue.Size = new Size(74, 13);
			lblPacketsRepeatValue.TabIndex = 6;
			lblPacketsRepeatValue.Text = "Repeat value:";
			lblPacketsRepeatValue.TextAlign = ContentAlignment.MiddleLeft;
			groupBoxEx1.Controls.Add(label53);
			groupBoxEx1.Controls.Add(label42);
			groupBoxEx1.Controls.Add(ledSignalDetected);
			groupBoxEx1.Controls.Add(label45);
			groupBoxEx1.Controls.Add(ledSignalSynchronized);
			groupBoxEx1.Controls.Add(label43);
			groupBoxEx1.Controls.Add(ledRxOnGoing);
			groupBoxEx1.Controls.Add(label41);
			groupBoxEx1.Controls.Add(ledHeaderInfoValid);
			groupBoxEx1.Controls.Add(label44);
			groupBoxEx1.Controls.Add(ledModemClear);
			groupBoxEx1.Location = new Point(863, 241);
			groupBoxEx1.Name = "groupBoxEx1";
			groupBoxEx1.Size = new Size(135, 139);
			groupBoxEx1.TabIndex = 28;
			groupBoxEx1.TabStop = false;
			groupBoxEx1.Text = "Modem status";
			groupBoxEx1.Visible = false;
			label53.AutoSize = true;
			label53.Location = new Point(30, 16);
			label53.Name = "label53";
			label53.Size = new Size(102, 13);
			label53.TabIndex = 1;
			label53.Text = "Searching Preamble";
			label42.AutoSize = true;
			label42.Location = new Point(30, 34);
			label42.Name = "label42";
			label42.Size = new Size(68, 13);
			label42.TabIndex = 1;
			label42.Text = "Modem clear";
			ledSignalDetected.BackColor = Color.Transparent;
			ledSignalDetected.LedColor = Color.Green;
			ledSignalDetected.LedSize = new Size(11, 11);
			ledSignalDetected.Location = new Point(6, 117);
			ledSignalDetected.Name = "ledSignalDetected";
			ledSignalDetected.Size = new Size(15, 15);
			ledSignalDetected.TabIndex = 2;
			ledSignalDetected.Text = "Signal detected";
			label45.AutoSize = true;
			label45.Location = new Point(30, 118);
			label45.Name = "label45";
			label45.Size = new Size(81, 13);
			label45.TabIndex = 3;
			label45.Text = "Signal detected";
			ledSignalSynchronized.BackColor = Color.Transparent;
			ledSignalSynchronized.LedColor = Color.Green;
			ledSignalSynchronized.LedSize = new Size(11, 11);
			ledSignalSynchronized.Location = new Point(6, 96);
			ledSignalSynchronized.Name = "ledSignalSynchronized";
			ledSignalSynchronized.Size = new Size(15, 15);
			ledSignalSynchronized.TabIndex = 2;
			ledSignalSynchronized.Text = "Signal synchronized";
			label43.AutoSize = true;
			label43.Location = new Point(30, 97);
			label43.Name = "label43";
			label43.Size = new Size(101, 13);
			label43.TabIndex = 3;
			label43.Text = "Signal synchronized";
			ledRxOnGoing.BackColor = Color.Transparent;
			ledRxOnGoing.LedColor = Color.Green;
			ledRxOnGoing.LedSize = new Size(11, 11);
			ledRxOnGoing.Location = new Point(6, 75);
			ledRxOnGoing.Name = "ledRxOnGoing";
			ledRxOnGoing.Size = new Size(15, 15);
			ledRxOnGoing.TabIndex = 2;
			ledRxOnGoing.Text = "Rx on going";
			label41.AutoSize = true;
			label41.Location = new Point(30, 76);
			label41.Name = "label41";
			label41.Size = new Size(64, 13);
			label41.TabIndex = 3;
			label41.Text = "Rx on going";
			ledHeaderInfoValid.BackColor = Color.Transparent;
			ledHeaderInfoValid.LedColor = Color.Green;
			ledHeaderInfoValid.LedSize = new Size(11, 11);
			ledHeaderInfoValid.Location = new Point(6, 54);
			ledHeaderInfoValid.Name = "ledHeaderInfoValid";
			ledHeaderInfoValid.Size = new Size(15, 15);
			ledHeaderInfoValid.TabIndex = 2;
			ledHeaderInfoValid.Text = "Header info valid";
			label44.AutoSize = true;
			label44.Location = new Point(30, 55);
			label44.Name = "label44";
			label44.Size = new Size(87, 13);
			label44.TabIndex = 3;
			label44.Text = "Header info valid";
			ledModemClear.BackColor = Color.Transparent;
			ledModemClear.LedColor = Color.Green;
			ledModemClear.LedSize = new Size(11, 11);
			ledModemClear.Location = new Point(6, 33);
			ledModemClear.Name = "ledModemClear";
			ledModemClear.Size = new Size(15, 15);
			ledModemClear.TabIndex = 0;
			ledModemClear.Text = "Modem clear";
			gBoxIrqMask.Controls.Add(panel10);
			gBoxIrqMask.Controls.Add(label7);
			gBoxIrqMask.Controls.Add(panel9);
			gBoxIrqMask.Controls.Add(label6);
			gBoxIrqMask.Controls.Add(panel8);
			gBoxIrqMask.Controls.Add(label5);
			gBoxIrqMask.Controls.Add(panel5);
			gBoxIrqMask.Controls.Add(label4);
			gBoxIrqMask.Controls.Add(panel3);
			gBoxIrqMask.Controls.Add(label3);
			gBoxIrqMask.Controls.Add(panel2);
			gBoxIrqMask.Controls.Add(label2);
			gBoxIrqMask.Controls.Add(panel1);
			gBoxIrqMask.Controls.Add(label1);
			gBoxIrqMask.Controls.Add(panel4);
			gBoxIrqMask.Controls.Add(label10);
			gBoxIrqMask.Location = new Point(557, 3);
			gBoxIrqMask.Name = "gBoxIrqMask";
			gBoxIrqMask.Size = new Size(239, 225);
			gBoxIrqMask.TabIndex = 1;
			gBoxIrqMask.TabStop = false;
			gBoxIrqMask.Text = "IRQ mask";
			gBoxIrqMask.MouseEnter += control_MouseEnter;
			gBoxIrqMask.MouseLeave += control_MouseLeave;
			panel10.AutoSize = true;
			panel10.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel10.Controls.Add(rBtnCadDetectedMaskOff);
			panel10.Controls.Add(rBtnCadDetectedMaskOn);
			panel10.Location = new Point(128, 198);
			panel10.Name = "panel10";
			panel10.Size = new Size(102, 20);
			panel10.TabIndex = 15;
			rBtnCadDetectedMaskOff.AutoSize = true;
			rBtnCadDetectedMaskOff.Location = new Point(54, 3);
			rBtnCadDetectedMaskOff.Margin = new Padding(3, 0, 3, 0);
			rBtnCadDetectedMaskOff.Name = "rBtnCadDetectedMaskOff";
			rBtnCadDetectedMaskOff.Size = new Size(45, 17);
			rBtnCadDetectedMaskOff.TabIndex = 1;
			rBtnCadDetectedMaskOff.Text = "OFF";
			rBtnCadDetectedMaskOff.UseVisualStyleBackColor = true;
			rBtnCadDetectedMaskOff.CheckedChanged += rBtnCadDetectedMask_CheckedChanged;
			rBtnCadDetectedMaskOn.AutoSize = true;
			rBtnCadDetectedMaskOn.Checked = true;
			rBtnCadDetectedMaskOn.Location = new Point(3, 3);
			rBtnCadDetectedMaskOn.Margin = new Padding(3, 0, 3, 0);
			rBtnCadDetectedMaskOn.Name = "rBtnCadDetectedMaskOn";
			rBtnCadDetectedMaskOn.Size = new Size(41, 17);
			rBtnCadDetectedMaskOn.TabIndex = 0;
			rBtnCadDetectedMaskOn.TabStop = true;
			rBtnCadDetectedMaskOn.Text = "ON";
			rBtnCadDetectedMaskOn.UseVisualStyleBackColor = true;
			rBtnCadDetectedMaskOn.CheckedChanged += rBtnCadDetectedMask_CheckedChanged;
			label7.AutoSize = true;
			label7.Location = new Point(6, 202);
			label7.Name = "label7";
			label7.Size = new Size(77, 13);
			label7.TabIndex = 14;
			label7.Text = "CAD detected:";
			panel9.AutoSize = true;
			panel9.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel9.Controls.Add(rBtnFhssChangeChannelMaskOff);
			panel9.Controls.Add(rBtnFhssChangeChannelMaskOn);
			panel9.Location = new Point(128, 172);
			panel9.Name = "panel9";
			panel9.Size = new Size(102, 20);
			panel9.TabIndex = 13;
			rBtnFhssChangeChannelMaskOff.AutoSize = true;
			rBtnFhssChangeChannelMaskOff.Location = new Point(54, 3);
			rBtnFhssChangeChannelMaskOff.Margin = new Padding(3, 0, 3, 0);
			rBtnFhssChangeChannelMaskOff.Name = "rBtnFhssChangeChannelMaskOff";
			rBtnFhssChangeChannelMaskOff.Size = new Size(45, 17);
			rBtnFhssChangeChannelMaskOff.TabIndex = 1;
			rBtnFhssChangeChannelMaskOff.Text = "OFF";
			rBtnFhssChangeChannelMaskOff.UseVisualStyleBackColor = true;
			rBtnFhssChangeChannelMaskOff.CheckedChanged += rBtnFhssChangeChannelMask_CheckedChanged;
			rBtnFhssChangeChannelMaskOn.AutoSize = true;
			rBtnFhssChangeChannelMaskOn.Checked = true;
			rBtnFhssChangeChannelMaskOn.Location = new Point(3, 3);
			rBtnFhssChangeChannelMaskOn.Margin = new Padding(3, 0, 3, 0);
			rBtnFhssChangeChannelMaskOn.Name = "rBtnFhssChangeChannelMaskOn";
			rBtnFhssChangeChannelMaskOn.Size = new Size(41, 17);
			rBtnFhssChangeChannelMaskOn.TabIndex = 0;
			rBtnFhssChangeChannelMaskOn.TabStop = true;
			rBtnFhssChangeChannelMaskOn.Text = "ON";
			rBtnFhssChangeChannelMaskOn.UseVisualStyleBackColor = true;
			rBtnFhssChangeChannelMaskOn.CheckedChanged += rBtnFhssChangeChannelMask_CheckedChanged;
			label6.AutoSize = true;
			label6.Location = new Point(6, 176);
			label6.Name = "label6";
			label6.Size = new Size(118, 13);
			label6.TabIndex = 12;
			label6.Text = "FHSS change channel:";
			panel8.AutoSize = true;
			panel8.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel8.Controls.Add(rBtnCadDoneMaskOff);
			panel8.Controls.Add(rBtnCadDoneMaskOn);
			panel8.Location = new Point(128, 146);
			panel8.Name = "panel8";
			panel8.Size = new Size(102, 20);
			panel8.TabIndex = 11;
			rBtnCadDoneMaskOff.AutoSize = true;
			rBtnCadDoneMaskOff.Location = new Point(54, 3);
			rBtnCadDoneMaskOff.Margin = new Padding(3, 0, 3, 0);
			rBtnCadDoneMaskOff.Name = "rBtnCadDoneMaskOff";
			rBtnCadDoneMaskOff.Size = new Size(45, 17);
			rBtnCadDoneMaskOff.TabIndex = 1;
			rBtnCadDoneMaskOff.Text = "OFF";
			rBtnCadDoneMaskOff.UseVisualStyleBackColor = true;
			rBtnCadDoneMaskOff.CheckedChanged += rBtnCadDoneMask_CheckedChanged;
			rBtnCadDoneMaskOn.AutoSize = true;
			rBtnCadDoneMaskOn.Checked = true;
			rBtnCadDoneMaskOn.Location = new Point(3, 3);
			rBtnCadDoneMaskOn.Margin = new Padding(3, 0, 3, 0);
			rBtnCadDoneMaskOn.Name = "rBtnCadDoneMaskOn";
			rBtnCadDoneMaskOn.Size = new Size(41, 17);
			rBtnCadDoneMaskOn.TabIndex = 0;
			rBtnCadDoneMaskOn.TabStop = true;
			rBtnCadDoneMaskOn.Text = "ON";
			rBtnCadDoneMaskOn.UseVisualStyleBackColor = true;
			rBtnCadDoneMaskOn.CheckedChanged += rBtnCadDoneMask_CheckedChanged;
			label5.AutoSize = true;
			label5.Location = new Point(6, 150);
			label5.Name = "label5";
			label5.Size = new Size(59, 13);
			label5.TabIndex = 10;
			label5.Text = "CAD done:";
			panel5.AutoSize = true;
			panel5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel5.Controls.Add(rBtnTxDoneMaskOff);
			panel5.Controls.Add(rBtnTxDoneMaskOn);
			panel5.Location = new Point(128, 120);
			panel5.Name = "panel5";
			panel5.Size = new Size(102, 20);
			panel5.TabIndex = 9;
			rBtnTxDoneMaskOff.AutoSize = true;
			rBtnTxDoneMaskOff.Location = new Point(54, 3);
			rBtnTxDoneMaskOff.Margin = new Padding(3, 0, 3, 0);
			rBtnTxDoneMaskOff.Name = "rBtnTxDoneMaskOff";
			rBtnTxDoneMaskOff.Size = new Size(45, 17);
			rBtnTxDoneMaskOff.TabIndex = 1;
			rBtnTxDoneMaskOff.Text = "OFF";
			rBtnTxDoneMaskOff.UseVisualStyleBackColor = true;
			rBtnTxDoneMaskOff.CheckedChanged += rBtnTxDoneMask_CheckedChanged;
			rBtnTxDoneMaskOn.AutoSize = true;
			rBtnTxDoneMaskOn.Checked = true;
			rBtnTxDoneMaskOn.Location = new Point(3, 3);
			rBtnTxDoneMaskOn.Margin = new Padding(3, 0, 3, 0);
			rBtnTxDoneMaskOn.Name = "rBtnTxDoneMaskOn";
			rBtnTxDoneMaskOn.Size = new Size(41, 17);
			rBtnTxDoneMaskOn.TabIndex = 0;
			rBtnTxDoneMaskOn.TabStop = true;
			rBtnTxDoneMaskOn.Text = "ON";
			rBtnTxDoneMaskOn.UseVisualStyleBackColor = true;
			rBtnTxDoneMaskOn.CheckedChanged += rBtnTxDoneMask_CheckedChanged;
			label4.AutoSize = true;
			label4.Location = new Point(6, 124);
			label4.Name = "label4";
			label4.Size = new Size(49, 13);
			label4.TabIndex = 8;
			label4.Text = "Tx done:";
			panel3.AutoSize = true;
			panel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel3.Controls.Add(rBtnValidHeaderMaskOff);
			panel3.Controls.Add(rBtnValidHeaderMaskOn);
			panel3.Location = new Point(128, 94);
			panel3.Name = "panel3";
			panel3.Size = new Size(102, 20);
			panel3.TabIndex = 7;
			rBtnValidHeaderMaskOff.AutoSize = true;
			rBtnValidHeaderMaskOff.Location = new Point(54, 3);
			rBtnValidHeaderMaskOff.Margin = new Padding(3, 0, 3, 0);
			rBtnValidHeaderMaskOff.Name = "rBtnValidHeaderMaskOff";
			rBtnValidHeaderMaskOff.Size = new Size(45, 17);
			rBtnValidHeaderMaskOff.TabIndex = 1;
			rBtnValidHeaderMaskOff.Text = "OFF";
			rBtnValidHeaderMaskOff.UseVisualStyleBackColor = true;
			rBtnValidHeaderMaskOff.CheckedChanged += rBtnValidHeaderMask_CheckedChanged;
			rBtnValidHeaderMaskOn.AutoSize = true;
			rBtnValidHeaderMaskOn.Checked = true;
			rBtnValidHeaderMaskOn.Location = new Point(3, 3);
			rBtnValidHeaderMaskOn.Margin = new Padding(3, 0, 3, 0);
			rBtnValidHeaderMaskOn.Name = "rBtnValidHeaderMaskOn";
			rBtnValidHeaderMaskOn.Size = new Size(41, 17);
			rBtnValidHeaderMaskOn.TabIndex = 0;
			rBtnValidHeaderMaskOn.TabStop = true;
			rBtnValidHeaderMaskOn.Text = "ON";
			rBtnValidHeaderMaskOn.UseVisualStyleBackColor = true;
			rBtnValidHeaderMaskOn.CheckedChanged += rBtnValidHeaderMask_CheckedChanged;
			label3.AutoSize = true;
			label3.Location = new Point(6, 98);
			label3.Name = "label3";
			label3.Size = new Size(69, 13);
			label3.TabIndex = 6;
			label3.Text = "Valid header:";
			panel2.AutoSize = true;
			panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel2.Controls.Add(rBtnPayloadCrcErrorMaskOff);
			panel2.Controls.Add(rBtnPayloadCrcErrorMaskOn);
			panel2.Location = new Point(128, 68);
			panel2.Name = "panel2";
			panel2.Size = new Size(102, 20);
			panel2.TabIndex = 5;
			rBtnPayloadCrcErrorMaskOff.AutoSize = true;
			rBtnPayloadCrcErrorMaskOff.Location = new Point(54, 3);
			rBtnPayloadCrcErrorMaskOff.Margin = new Padding(3, 0, 3, 0);
			rBtnPayloadCrcErrorMaskOff.Name = "rBtnPayloadCrcErrorMaskOff";
			rBtnPayloadCrcErrorMaskOff.Size = new Size(45, 17);
			rBtnPayloadCrcErrorMaskOff.TabIndex = 1;
			rBtnPayloadCrcErrorMaskOff.Text = "OFF";
			rBtnPayloadCrcErrorMaskOff.UseVisualStyleBackColor = true;
			rBtnPayloadCrcErrorMaskOff.CheckedChanged += rBtnPayloadCrcErrorMask_CheckedChanged;
			rBtnPayloadCrcErrorMaskOn.AutoSize = true;
			rBtnPayloadCrcErrorMaskOn.Checked = true;
			rBtnPayloadCrcErrorMaskOn.Location = new Point(3, 3);
			rBtnPayloadCrcErrorMaskOn.Margin = new Padding(3, 0, 3, 0);
			rBtnPayloadCrcErrorMaskOn.Name = "rBtnPayloadCrcErrorMaskOn";
			rBtnPayloadCrcErrorMaskOn.Size = new Size(41, 17);
			rBtnPayloadCrcErrorMaskOn.TabIndex = 0;
			rBtnPayloadCrcErrorMaskOn.TabStop = true;
			rBtnPayloadCrcErrorMaskOn.Text = "ON";
			rBtnPayloadCrcErrorMaskOn.UseVisualStyleBackColor = true;
			rBtnPayloadCrcErrorMaskOn.CheckedChanged += rBtnPayloadCrcErrorMask_CheckedChanged;
			label2.AutoSize = true;
			label2.Location = new Point(6, 72);
			label2.Name = "label2";
			label2.Size = new Size(97, 13);
			label2.TabIndex = 4;
			label2.Text = "Payload CRC error:";
			panel1.AutoSize = true;
			panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel1.Controls.Add(rBtnRxDoneMaskOff);
			panel1.Controls.Add(rBtnRxDoneMaskOn);
			panel1.Location = new Point(128, 42);
			panel1.Name = "panel1";
			panel1.Size = new Size(102, 20);
			panel1.TabIndex = 3;
			rBtnRxDoneMaskOff.AutoSize = true;
			rBtnRxDoneMaskOff.Location = new Point(54, 3);
			rBtnRxDoneMaskOff.Margin = new Padding(3, 0, 3, 0);
			rBtnRxDoneMaskOff.Name = "rBtnRxDoneMaskOff";
			rBtnRxDoneMaskOff.Size = new Size(45, 17);
			rBtnRxDoneMaskOff.TabIndex = 1;
			rBtnRxDoneMaskOff.Text = "OFF";
			rBtnRxDoneMaskOff.UseVisualStyleBackColor = true;
			rBtnRxDoneMaskOff.CheckedChanged += rBtnRxDoneMask_CheckedChanged;
			rBtnRxDoneMaskOn.AutoSize = true;
			rBtnRxDoneMaskOn.Checked = true;
			rBtnRxDoneMaskOn.Location = new Point(3, 3);
			rBtnRxDoneMaskOn.Margin = new Padding(3, 0, 3, 0);
			rBtnRxDoneMaskOn.Name = "rBtnRxDoneMaskOn";
			rBtnRxDoneMaskOn.Size = new Size(41, 17);
			rBtnRxDoneMaskOn.TabIndex = 0;
			rBtnRxDoneMaskOn.TabStop = true;
			rBtnRxDoneMaskOn.Text = "ON";
			rBtnRxDoneMaskOn.UseVisualStyleBackColor = true;
			rBtnRxDoneMaskOn.CheckedChanged += rBtnRxDoneMask_CheckedChanged;
			label1.AutoSize = true;
			label1.Location = new Point(6, 46);
			label1.Name = "label1";
			label1.Size = new Size(50, 13);
			label1.TabIndex = 2;
			label1.Text = "Rx done:";
			panel4.AutoSize = true;
			panel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel4.Controls.Add(rBtnRxTimeoutMaskOff);
			panel4.Controls.Add(rBtnRxTimeoutMaskOn);
			panel4.Location = new Point(128, 16);
			panel4.Name = "panel4";
			panel4.Size = new Size(102, 20);
			panel4.TabIndex = 1;
			rBtnRxTimeoutMaskOff.AutoSize = true;
			rBtnRxTimeoutMaskOff.Location = new Point(54, 3);
			rBtnRxTimeoutMaskOff.Margin = new Padding(3, 0, 3, 0);
			rBtnRxTimeoutMaskOff.Name = "rBtnRxTimeoutMaskOff";
			rBtnRxTimeoutMaskOff.Size = new Size(45, 17);
			rBtnRxTimeoutMaskOff.TabIndex = 1;
			rBtnRxTimeoutMaskOff.Text = "OFF";
			rBtnRxTimeoutMaskOff.UseVisualStyleBackColor = true;
			rBtnRxTimeoutMaskOff.CheckedChanged += rBtnRxTimeoutMask_CheckedChanged;
			rBtnRxTimeoutMaskOn.AutoSize = true;
			rBtnRxTimeoutMaskOn.Checked = true;
			rBtnRxTimeoutMaskOn.Location = new Point(3, 3);
			rBtnRxTimeoutMaskOn.Margin = new Padding(3, 0, 3, 0);
			rBtnRxTimeoutMaskOn.Name = "rBtnRxTimeoutMaskOn";
			rBtnRxTimeoutMaskOn.Size = new Size(41, 17);
			rBtnRxTimeoutMaskOn.TabIndex = 0;
			rBtnRxTimeoutMaskOn.TabStop = true;
			rBtnRxTimeoutMaskOn.Text = "ON";
			rBtnRxTimeoutMaskOn.UseVisualStyleBackColor = true;
			rBtnRxTimeoutMaskOn.CheckedChanged += rBtnRxTimeoutMask_CheckedChanged;
			label10.AutoSize = true;
			label10.Location = new Point(6, 20);
			label10.Name = "label10";
			label10.Size = new Size(60, 13);
			label10.TabIndex = 0;
			label10.Text = "Rx timeout:";
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(pnlPacketStatusHeaderName);
			Controls.Add(lblPacketStatusHeaderName);
			Controls.Add(pnlRxHeaderInfoHeader);
			Controls.Add(lblRxHeaderInfoHeaderName);
			Controls.Add(pnlHeaderInfo);
			Controls.Add(pnlPacketStatus);
			Controls.Add(gBoxMessage);
			Controls.Add(gBoxControl);
			Controls.Add(groupBoxEx1);
			Controls.Add(gBoxIrqMask);
			Controls.Add(gBoxSettings);
			Name = "LoRaViewControl";
			Size = new Size(799, 493);
			((ISupportInitialize)errorProvider).EndInit();
			((ISupportInitialize)nudPreambleLength).EndInit();
			panel11.ResumeLayout(false);
			panel11.PerformLayout();
			panel12.ResumeLayout(false);
			panel12.PerformLayout();
			pnlPacketStatus.ResumeLayout(false);
			pnlPacketStatus.PerformLayout();
			pnlHeaderInfo.ResumeLayout(false);
			pnlHeaderInfo.PerformLayout();
			gBoxSettings.ResumeLayout(false);
			gBoxSettings.PerformLayout();
			((ISupportInitialize)nudSymbTimeout).EndInit();
			panel13.ResumeLayout(false);
			panel13.PerformLayout();
			((ISupportInitialize)nudFreqHoppingPeriod).EndInit();
			((ISupportInitialize)nudPayloadLength).EndInit();
			gBoxMessage.ResumeLayout(false);
			gBoxMessage.PerformLayout();
			tblPayloadMessage.ResumeLayout(false);
			gBoxControl.ResumeLayout(false);
			gBoxControl.PerformLayout();
			pnlPacketMode.ResumeLayout(false);
			pnlPacketMode.PerformLayout();
			groupBoxEx1.ResumeLayout(false);
			groupBoxEx1.PerformLayout();
			gBoxIrqMask.ResumeLayout(false);
			gBoxIrqMask.PerformLayout();
			panel10.ResumeLayout(false);
			panel10.PerformLayout();
			panel9.ResumeLayout(false);
			panel9.PerformLayout();
			panel8.ResumeLayout(false);
			panel8.PerformLayout();
			panel5.ResumeLayout(false);
			panel5.PerformLayout();
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			panel2.ResumeLayout(false);
			panel2.PerformLayout();
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			panel4.ResumeLayout(false);
			panel4.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
