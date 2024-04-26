using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using SemtechLib.Controls;
using SemtechLib.Controls.HexBoxCtrl;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.Devices.SX1276.Events;
using SemtechLib.Devices.SX1276.General;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;

namespace SemtechLib.Devices.SX1276.UI.Controls
{
	public sealed class PacketHandlerView : UserControl, INotifyDocumentationChanged
	{
		private IContainer components;

		private Label label1;

		private NumericUpDownEx nudPreambleSize;

		private Label label2;

		private Label label3;

		private Panel pnlSync;

		private RadioButton rBtnSyncOn;

		private RadioButton rBtnSyncOff;

		private Label label4;

		private Panel pnlFifoFillCondition;

		private RadioButton rBtnFifoFillAlways;

		private RadioButton rBtnFifoFillSyncAddress;

		private Label label9;

		private MaskedTextBox tBoxSyncValue;

		private Label label10;

		private Panel pnlPacketFormat;

		private RadioButton rBtnPacketFormatVariable;

		private RadioButton rBtnPacketFormatFixed;

		private Label label11;

		private NumericUpDownEx nudPayloadLength;

		private Label lblPayloadLength;

		private Label label12;

		private Label label17;

		private Panel pnlAddressInPayload;

		private Label label18;

		private Panel pnlAddressFiltering;

		private RadioButton rBtnAddressFilteringOff;

		private RadioButton rBtnAddressFilteringNode;

		private RadioButton rBtnAddressFilteringNodeBroadcast;

		private Label label19;

		private NumericUpDownEx nudNodeAddress;

		private Label lblNodeAddress;

		private Label label20;

		private NumericUpDownEx nudBroadcastAddress;

		private Label lblBroadcastAddress;

		private Label label21;

		private Panel pnlDcFree;

		private RadioButton rBtnDcFreeOff;

		private RadioButton rBtnDcFreeManchester;

		private RadioButton rBtnDcFreeWhitening;

		private Label label22;

		private Panel pnlCrcCalculation;

		private RadioButton rBtnCrcOn;

		private RadioButton rBtnCrcOff;

		private Label label23;

		private Panel pnlCrcAutoClear;

		private RadioButton rBtnCrcAutoClearOn;

		private RadioButton rBtnCrcAutoClearOff;

		private Label label26;

		private Panel pnlTxStart;

		private RadioButton rBtnTxStartFifoLevel;

		private RadioButton rBtnTxStartFifoNotEmpty;

		private Label label27;

		private NumericUpDownEx nudFifoThreshold;

		private GroupBoxEx gBoxPacket;

		private TableLayoutPanel tblPacket;

		private Label label29;

		private Label lblPacketPreamble;

		private Label label30;

		private Label lblPacketSyncValue;

		private Label label31;

		private Label lblPacketLength;

		private Label label32;

		private Panel pnlPacketAddr;

		private Label lblPacketAddr;

		private Label label33;

		private Label lblPayload;

		private Label label34;

		private Panel pnlPacketCrc;

		private Label lblPacketCrc;

		private Led ledPacketCrc;

		private PayloadImg imgPacketMessage;

		private GroupBoxEx gBoxMessage;

		private TableLayoutPanel tblPayloadMessage;

		private Label label35;

		private Label label36;

		private HexBox hexBoxPayload;

		private GroupBoxEx gBoxControl;

		private CheckBox cBtnPacketHandlerStartStop;

		private Label lblPacketsNb;

		private TextBox tBoxPacketsNb;

		private Label lblPacketsRepeatValue;

		private TextBox tBoxPacketsRepeatValue;

		private ErrorProvider errorProvider;

		private TableLayoutPanel tableLayoutPanel1;

		private Panel pnlPayloadLength;

		private TableLayoutPanel tableLayoutPanel2;

		private Panel pnlBroadcastAddress;

		private Panel pnlNodeAddress;

		private GroupBoxEx gBoxDeviceStatus;

		private Label lblOperatingMode;

		private Label label37;

		private Label lblBitSynchroniser;

		private Label lblDataMode;

		private Label label38;

		private Label label39;

		private RadioButton rBtnNodeAddressInPayloadNo;

		private RadioButton rBtnNodeAddressInPayloadYes;

		private CheckBox cBtnLog;

		private Label label13;

		private Label label5;

		private NumericUpDownEx nudSyncSize;

		private Label label6;

		private Panel panel1;

		private RadioButton rBtnPreamblePolarity55;

		private RadioButton rBtnPreamblePolarityAA;

		private Label label7;

		private Panel panel2;

		private RadioButton rBtnCrcCcitt;

		private RadioButton rBtnCrcIbm;

		private Panel panel3;

		private RadioButton rBtnIoHomeOff;

		private RadioButton rBtnIoHomeOn;

		private Panel panel4;

		private RadioButton rBtnIoHomePwrFrameOff;

		private RadioButton rBtnIoHomePwrFrameOn;

		private Panel panel5;

		private RadioButton rBtnBeaconOff;

		private RadioButton rBtnBeaconOn;

		private Label label8;

		private Label label14;

		private Label label15;

		private Label label16;

		private ComboBox cBoxDataMode;

		private Label label24;

		private Button btnFillFifo;

		private Label label25;

		private ComboBox cBoxAutoRestartRxMode;

		private bool inHexPayloadDataChanged;

		private readonly MaskValidationType syncWord = new("69-81-7E-96");

		private readonly MaskValidationType aesWord = new("00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00");

		private OperatingModeEnum mode = OperatingModeEnum.Stdby;

		private decimal bitRate;

		private bool bitSync = true;

		private byte[] syncValue = [105, 129, 126, 150];

		private byte[] message;

		private ushort crc;

		public OperatingModeEnum Mode
		{
			get => mode;
			set
			{
				mode = value;
				UpdateControls();
			}
		}

		public decimal Bitrate
		{
			get => bitRate;
			set
			{
				if (!bitRate.Equals(value))
				{
					bitRate = value;
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

		public DataModeEnum DataMode
		{
			get => (DataModeEnum)cBoxDataMode.SelectedIndex;
			set
			{
				cBoxDataMode.SelectedIndex = (int)value;
				UpdateControls();
			}
		}

		public int PreambleSize
		{
			get => (int)nudPreambleSize.Value;
			set
			{
				nudPreambleSize.Value = value;
                lblPacketPreamble.Text = value switch
                {
                    0 => "",
                    1 => "55",
                    2 => "55-55",
                    3 => "55-55-55",
                    4 => "55-55-55-55",
                    5 => "55-55-55-55-55",
                    _ => "55-55-55-55-...-55",
                };
                if (nudPreambleSize.Value < 2m)
				{
					nudPreambleSize.BackColor = ControlPaint.LightLight(Color.Red);
					errorProvider.SetError(nudPreambleSize, "Preamble size must be greater than 12 bits!");
				}
				else
				{
					nudPreambleSize.BackColor = SystemColors.Window;
					errorProvider.SetError(nudPreambleSize, "");
				}
			}
		}

		public AutoRestartRxEnum AutoRestartRxOn
		{
			get => (AutoRestartRxEnum)cBoxAutoRestartRxMode.SelectedIndex;
			set => cBoxAutoRestartRxMode.SelectedIndex = (int)value;
		}

		public PreamblePolarityEnum PreamblePolarity
		{
			get
			{
				return rBtnPreamblePolarity55.Checked ? PreamblePolarityEnum.POLARITY_55 : PreamblePolarityEnum.POLARITY_AA;
			}
			set
			{
				rBtnPreamblePolarity55.CheckedChanged -= rBtnPreamblePolarity_CheckedChanged;
				rBtnPreamblePolarityAA.CheckedChanged -= rBtnPreamblePolarity_CheckedChanged;
				if (value == PreamblePolarityEnum.POLARITY_55)
				{
					rBtnPreamblePolarity55.Checked = true;
					rBtnPreamblePolarityAA.Checked = false;
				}
				else
				{
					rBtnPreamblePolarity55.Checked = false;
					rBtnPreamblePolarityAA.Checked = true;
				}
				rBtnPreamblePolarity55.CheckedChanged += rBtnPreamblePolarity_CheckedChanged;
				rBtnPreamblePolarityAA.CheckedChanged += rBtnPreamblePolarity_CheckedChanged;
			}
		}

		public bool SyncOn
		{
			get => rBtnSyncOn.Checked;
			set
			{
				rBtnSyncOn.Checked = value;
				rBtnSyncOff.Checked = !value;
				nudSyncSize.Enabled = value;
				tBoxSyncValue.Enabled = value;
				lblPacketSyncValue.Visible = value;
			}
		}

		public FifoFillConditionEnum FifoFillCondition
		{
			get
			{
				return !rBtnFifoFillSyncAddress.Checked ? FifoFillConditionEnum.Allways : FifoFillConditionEnum.OnSyncAddressIrq;
			}
			set
			{
				if (value == FifoFillConditionEnum.OnSyncAddressIrq)
				{
					rBtnFifoFillSyncAddress.Checked = true;
				}
				else
				{
					rBtnFifoFillAlways.Checked = true;
				}
			}
		}

		public byte SyncSize
		{
			get => (byte)nudSyncSize.Value;
			set
			{
				try
				{
					nudSyncSize.Value = value;
					var text = tBoxSyncValue.Text;
                    tBoxSyncValue.Mask = (byte)nudSyncSize.Value switch
                    {
                        1 => "&&",
                        2 => "&&-&&",
                        3 => "&&-&&-&&",
                        4 => "&&-&&-&&-&&",
                        5 => "&&-&&-&&-&&-&&",
                        6 => "&&-&&-&&-&&-&&-&&",
                        7 => "&&-&&-&&-&&-&&-&&-&&",
                        8 => "&&-&&-&&-&&-&&-&&-&&-&&",
                        _ => throw new Exception("Wrong sync word size!"),
                    };
                    tBoxSyncValue.Text = text;
				}
				catch (Exception ex)
				{
					OnError(1, ex.Message);
				}
			}
		}

		public byte[] SyncValue
		{
			get => syncValue;
			set
			{
				syncValue = value;
				try
				{
					tBoxSyncValue.TextChanged -= tBoxSyncValue_TextChanged;
					tBoxSyncValue.MaskInputRejected -= tBoxSyncValue_MaskInputRejected;
					syncWord.ArrayValue = syncValue;
					var label = lblPacketSyncValue;
					var text = (tBoxSyncValue.Text = syncWord.StringValue);
					label.Text = text;
				}
				catch (Exception ex)
				{
					OnError(1, ex.Message);
				}
				finally
				{
					tBoxSyncValue.TextChanged += tBoxSyncValue_TextChanged;
					tBoxSyncValue.MaskInputRejected += tBoxSyncValue_MaskInputRejected;
				}
			}
		}

		public PacketFormatEnum PacketFormat
		{
			get
			{
				if (rBtnPacketFormatVariable.Checked)
				{
					return PacketFormatEnum.Variable;
				}
				return PacketFormatEnum.Fixed;
			}
			set
			{
				nudPayloadLength.Enabled = Mode switch
				{
					OperatingModeEnum.Tx => false,
					OperatingModeEnum.Rx => true,
					_ => false
				};
				if (value == PacketFormatEnum.Variable)
				{
					lblPacketLength.Visible = true;
					rBtnPacketFormatVariable.Checked = true;
				}
				else
				{
					lblPacketLength.Visible = false;
					rBtnPacketFormatFixed.Checked = true;
				}
			}
		}

		public DcFreeEnum DcFree
		{
			get
			{
				if (rBtnDcFreeOff.Checked)
				{
					return DcFreeEnum.OFF;
				}
				if (rBtnDcFreeManchester.Checked)
				{
					return DcFreeEnum.Manchester;
				}
				if (rBtnDcFreeWhitening.Checked)
				{
					return DcFreeEnum.Whitening;
				}
				return DcFreeEnum.OFF;
			}
			set
			{
				switch (value)
				{
				case DcFreeEnum.Manchester:
					rBtnDcFreeManchester.Checked = true;
					break;
				case DcFreeEnum.Whitening:
					rBtnDcFreeWhitening.Checked = true;
					break;
				default:
					rBtnDcFreeOff.Checked = true;
					break;
				}
			}
		}

		public bool CrcOn
		{
			get => rBtnCrcOn.Checked;
			set
			{
				lblPacketCrc.Visible = value;
				rBtnCrcOn.Checked = value;
				rBtnCrcOff.Checked = !value;
			}
		}

		public bool CrcAutoClearOff
		{
			get => rBtnCrcAutoClearOff.Checked;
			set
			{
				rBtnCrcAutoClearOn.Checked = !value;
				rBtnCrcAutoClearOff.Checked = value;
			}
		}

		public AddressFilteringEnum AddressFiltering
		{
			get
			{
				if (rBtnAddressFilteringOff.Checked)
				{
					return AddressFilteringEnum.OFF;
				}
				if (rBtnAddressFilteringNode.Checked)
				{
					return AddressFilteringEnum.Node;
				}
				if (rBtnAddressFilteringNodeBroadcast.Checked)
				{
					return AddressFilteringEnum.NodeBroadcast;
				}
				return AddressFilteringEnum.Reserved;
			}
			set
			{
				switch (value)
				{
				case AddressFilteringEnum.Node:
					rBtnAddressFilteringNode.Checked = true;
					lblPacketAddr.Visible = true;
					nudNodeAddress.Enabled = true;
					lblNodeAddress.Enabled = true;
					nudBroadcastAddress.Enabled = false;
					lblBroadcastAddress.Enabled = false;
					break;
				case AddressFilteringEnum.NodeBroadcast:
					rBtnAddressFilteringNodeBroadcast.Checked = true;
					lblPacketAddr.Visible = true;
					nudNodeAddress.Enabled = true;
					lblNodeAddress.Enabled = true;
					nudBroadcastAddress.Enabled = true;
					lblBroadcastAddress.Enabled = true;
					break;
				case AddressFilteringEnum.Reserved:
					rBtnAddressFilteringNode.Checked = false;
					rBtnAddressFilteringNodeBroadcast.Checked = false;
					rBtnAddressFilteringOff.Checked = false;
					lblPacketAddr.Visible = false;
					nudNodeAddress.Enabled = false;
					lblNodeAddress.Enabled = false;
					nudBroadcastAddress.Enabled = false;
					lblBroadcastAddress.Enabled = false;
					break;
				default:
					rBtnAddressFilteringOff.Checked = true;
					lblPacketAddr.Visible = false;
					nudNodeAddress.Enabled = false;
					lblNodeAddress.Enabled = false;
					nudBroadcastAddress.Enabled = false;
					lblBroadcastAddress.Enabled = false;
					break;
				}
			}
		}

		public short PayloadLength
		{
			get => (short)nudPayloadLength.Value;
			set
			{
				nudPayloadLength.Value = value;
				lblPayloadLength.Text = "0x" + value.ToString("X02");
			}
		}

		public byte NodeAddress
		{
			get => (byte)nudNodeAddress.Value;
			set
			{
				nudNodeAddress.Value = value;
				lblPacketAddr.Text = value.ToString("X02");
				lblNodeAddress.Text = "0x" + value.ToString("X02");
			}
		}

		public byte NodeAddressRx
		{
			get => 0;
			set => lblPacketAddr.Text = value.ToString("X02");
		}

		public byte BroadcastAddress
		{
			get => (byte)nudBroadcastAddress.Value;
			set
			{
				nudBroadcastAddress.Value = value;
				lblBroadcastAddress.Text = "0x" + value.ToString("X02");
			}
		}

		public bool TxStartCondition
		{
			get => rBtnTxStartFifoNotEmpty.Checked;
			set
			{
				rBtnTxStartFifoNotEmpty.Checked = value;
				rBtnTxStartFifoLevel.Checked = !value;
			}
		}

		public byte FifoThreshold
		{
			get => (byte)nudFifoThreshold.Value;
			set => nudFifoThreshold.Value = value;
		}

		public bool CrcIbmOn
		{
			get => rBtnCrcIbm.Checked;
			set
			{
				rBtnCrcIbm.CheckedChanged -= rBtnCrcIbm_CheckedChanged;
				rBtnCrcCcitt.CheckedChanged -= rBtnCrcIbm_CheckedChanged;
				if (value)
				{
					rBtnCrcIbm.Checked = true;
					rBtnCrcCcitt.Checked = false;
				}
				else
				{
					rBtnCrcIbm.Checked = false;
					rBtnCrcCcitt.Checked = true;
				}
				rBtnCrcIbm.CheckedChanged += rBtnCrcIbm_CheckedChanged;
				rBtnCrcCcitt.CheckedChanged += rBtnCrcIbm_CheckedChanged;
			}
		}

		public bool IoHomeOn
		{
			get => rBtnIoHomeOn.Checked;
			set
			{
				rBtnIoHomeOn.CheckedChanged -= rBtnIoHomeOn_CheckedChanged;
				rBtnIoHomeOff.CheckedChanged -= rBtnIoHomeOn_CheckedChanged;
				if (value)
				{
					rBtnIoHomeOn.Checked = true;
					rBtnIoHomeOff.Checked = false;
				}
				else
				{
					rBtnIoHomeOn.Checked = false;
					rBtnIoHomeOff.Checked = true;
				}
				rBtnIoHomeOn.CheckedChanged += rBtnIoHomeOn_CheckedChanged;
				rBtnIoHomeOff.CheckedChanged += rBtnIoHomeOn_CheckedChanged;
			}
		}

		public bool IoHomePwrFrameOn
		{
			get => rBtnIoHomePwrFrameOn.Checked;
			set
			{
				rBtnIoHomePwrFrameOn.CheckedChanged -= rBtnIoHomePwrFrameOn_CheckedChanged;
				rBtnIoHomePwrFrameOff.CheckedChanged -= rBtnIoHomePwrFrameOn_CheckedChanged;
				if (value)
				{
					rBtnIoHomePwrFrameOn.Checked = true;
					rBtnIoHomePwrFrameOff.Checked = false;
				}
				else
				{
					rBtnIoHomePwrFrameOn.Checked = false;
					rBtnIoHomePwrFrameOff.Checked = true;
				}
				rBtnIoHomePwrFrameOn.CheckedChanged += rBtnIoHomePwrFrameOn_CheckedChanged;
				rBtnIoHomePwrFrameOff.CheckedChanged += rBtnIoHomePwrFrameOn_CheckedChanged;
			}
		}

		public bool BeaconOn
		{
			get => rBtnBeaconOn.Checked;
			set
			{
				rBtnBeaconOn.CheckedChanged -= rBtnBeaconOn_CheckedChanged;
				rBtnBeaconOff.CheckedChanged -= rBtnBeaconOn_CheckedChanged;
				if (value)
				{
					rBtnBeaconOn.Checked = true;
					rBtnBeaconOff.Checked = false;
				}
				else
				{
					rBtnBeaconOn.Checked = false;
					rBtnBeaconOff.Checked = true;
				}
				rBtnBeaconOn.CheckedChanged += rBtnBeaconOn_CheckedChanged;
				rBtnBeaconOff.CheckedChanged += rBtnBeaconOn_CheckedChanged;
			}
		}

		public int MessageLength
		{
			get => Convert.ToInt32(lblPacketLength.Text, 16);
			set => lblPacketLength.Text = value.ToString("X02");
		}

		public byte[] Message
		{
			get => message;
			set
			{
				message = value;
				var dynamicByteProvider = hexBoxPayload.ByteProvider as DynamicByteProvider;
				dynamicByteProvider.Bytes.Clear();
				dynamicByteProvider.Bytes.AddRange(value);
				hexBoxPayload.ByteProvider.ApplyChanges();
				hexBoxPayload.Invalidate();
			}
		}

		public ushort Crc
		{
			get => crc;
			set
			{
				crc = value;
				lblPacketCrc.Text = ((value >> 8) & 0xFF).ToString("X02") + "-" + (value & 0xFF).ToString("X02");
			}
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

		public bool LogEnabled
		{
			get => cBtnLog.Checked;
			set => cBtnLog.Checked = value;
		}

		public event ErrorEventHandler Error;

		public event DataModeEventHandler DataModeChanged;

		public event Int32EventHandler PreambleSizeChanged;

		public event AutoRestartRxEventHandler AutoRestartRxChanged;

		public event PreamblePolarityEventHandler PreamblePolarityChanged;

		public event BooleanEventHandler SyncOnChanged;

		public event FifoFillConditionEventHandler FifoFillConditionChanged;

		public event ByteEventHandler SyncSizeChanged;

		public event ByteArrayEventHandler SyncValueChanged;

		public event PacketFormatEventHandler PacketFormatChanged;

		public event DcFreeEventHandler DcFreeChanged;

		public event BooleanEventHandler CrcOnChanged;

		public event BooleanEventHandler CrcAutoClearOffChanged;

		public event AddressFilteringEventHandler AddressFilteringChanged;

		public event Int16EventHandler PayloadLengthChanged;

		public event ByteEventHandler NodeAddressChanged;

		public event ByteEventHandler BroadcastAddressChanged;

		public event BooleanEventHandler TxStartConditionChanged;

		public event ByteEventHandler FifoThresholdChanged;

		public event Int32EventHandler MessageLengthChanged;

		public event ByteArrayEventHandler MessageChanged;

		public event BooleanEventHandler StartStopChanged;

		public event Int32EventHandler MaxPacketNumberChanged;

		public event BooleanEventHandler PacketHandlerLogEnableChanged;

		public event BooleanEventHandler CrcIbmChanged;

		public event BooleanEventHandler IoHomeOnChanged;

		public event BooleanEventHandler IoHomePwrFrameOnChanged;

		public event BooleanEventHandler BeaconOnChanged;

		public event EventHandler FillFifoChanged;

		public event DocumentationChangedEventHandler DocumentationChanged;

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
			tBoxSyncValue = new MaskedTextBox();
			nudPreambleSize = new NumericUpDownEx();
			label12 = new Label();
			label1 = new Label();
			label2 = new Label();
			label18 = new Label();
			label11 = new Label();
			label20 = new Label();
			label10 = new Label();
			label21 = new Label();
			label19 = new Label();
			label27 = new Label();
			label26 = new Label();
			pnlDcFree = new Panel();
			rBtnDcFreeWhitening = new RadioButton();
			rBtnDcFreeManchester = new RadioButton();
			rBtnDcFreeOff = new RadioButton();
			pnlAddressInPayload = new Panel();
			rBtnNodeAddressInPayloadNo = new RadioButton();
			rBtnNodeAddressInPayloadYes = new RadioButton();
			label17 = new Label();
			pnlFifoFillCondition = new Panel();
			rBtnFifoFillAlways = new RadioButton();
			rBtnFifoFillSyncAddress = new RadioButton();
			label4 = new Label();
			pnlSync = new Panel();
			rBtnSyncOff = new RadioButton();
			rBtnSyncOn = new RadioButton();
			label3 = new Label();
			label9 = new Label();
			pnlCrcAutoClear = new Panel();
			rBtnCrcAutoClearOff = new RadioButton();
			rBtnCrcAutoClearOn = new RadioButton();
			label23 = new Label();
			pnlCrcCalculation = new Panel();
			rBtnCrcOff = new RadioButton();
			rBtnCrcOn = new RadioButton();
			label22 = new Label();
			pnlTxStart = new Panel();
			rBtnTxStartFifoNotEmpty = new RadioButton();
			rBtnTxStartFifoLevel = new RadioButton();
			pnlAddressFiltering = new Panel();
			rBtnAddressFilteringNodeBroadcast = new RadioButton();
			rBtnAddressFilteringNode = new RadioButton();
			rBtnAddressFilteringOff = new RadioButton();
			lblNodeAddress = new Label();
			lblPayloadLength = new Label();
			lblBroadcastAddress = new Label();
			pnlPacketFormat = new Panel();
			rBtnPacketFormatFixed = new RadioButton();
			rBtnPacketFormatVariable = new RadioButton();
			tableLayoutPanel1 = new TableLayoutPanel();
			pnlPayloadLength = new Panel();
			nudPayloadLength = new NumericUpDownEx();
			label5 = new Label();
			nudSyncSize = new NumericUpDownEx();
			label6 = new Label();
			panel1 = new Panel();
			rBtnPreamblePolarity55 = new RadioButton();
			rBtnPreamblePolarityAA = new RadioButton();
			label7 = new Label();
			cBoxDataMode = new ComboBox();
			label24 = new Label();
			label25 = new Label();
			cBoxAutoRestartRxMode = new ComboBox();
			pnlNodeAddress = new Panel();
			nudNodeAddress = new NumericUpDownEx();
			pnlBroadcastAddress = new Panel();
			nudBroadcastAddress = new NumericUpDownEx();
			tableLayoutPanel2 = new TableLayoutPanel();
			nudFifoThreshold = new NumericUpDownEx();
			label13 = new Label();
			panel2 = new Panel();
			rBtnCrcCcitt = new RadioButton();
			rBtnCrcIbm = new RadioButton();
			panel3 = new Panel();
			rBtnIoHomeOff = new RadioButton();
			rBtnIoHomeOn = new RadioButton();
			panel4 = new Panel();
			rBtnIoHomePwrFrameOff = new RadioButton();
			rBtnIoHomePwrFrameOn = new RadioButton();
			panel5 = new Panel();
			rBtnBeaconOff = new RadioButton();
			rBtnBeaconOn = new RadioButton();
			label8 = new Label();
			label14 = new Label();
			label15 = new Label();
			label16 = new Label();
			gBoxDeviceStatus = new GroupBoxEx();
			lblOperatingMode = new Label();
			label37 = new Label();
			lblBitSynchroniser = new Label();
			lblDataMode = new Label();
			label38 = new Label();
			label39 = new Label();
			gBoxControl = new GroupBoxEx();
			btnFillFifo = new Button();
			tBoxPacketsNb = new TextBox();
			cBtnLog = new CheckBox();
			cBtnPacketHandlerStartStop = new CheckBox();
			lblPacketsNb = new Label();
			tBoxPacketsRepeatValue = new TextBox();
			lblPacketsRepeatValue = new Label();
			gBoxPacket = new GroupBoxEx();
			imgPacketMessage = new PayloadImg();
			gBoxMessage = new GroupBoxEx();
			tblPayloadMessage = new TableLayoutPanel();
			hexBoxPayload = new HexBox();
			label36 = new Label();
			label35 = new Label();
			tblPacket = new TableLayoutPanel();
			label29 = new Label();
			label30 = new Label();
			label31 = new Label();
			label32 = new Label();
			label33 = new Label();
			label34 = new Label();
			lblPacketPreamble = new Label();
			lblPayload = new Label();
			pnlPacketCrc = new Panel();
			ledPacketCrc = new Led();
			lblPacketCrc = new Label();
			pnlPacketAddr = new Panel();
			lblPacketAddr = new Label();
			lblPacketLength = new Label();
			lblPacketSyncValue = new Label();
			((ISupportInitialize)errorProvider).BeginInit();
			((ISupportInitialize)nudPreambleSize).BeginInit();
			pnlDcFree.SuspendLayout();
			pnlAddressInPayload.SuspendLayout();
			pnlFifoFillCondition.SuspendLayout();
			pnlSync.SuspendLayout();
			pnlCrcAutoClear.SuspendLayout();
			pnlCrcCalculation.SuspendLayout();
			pnlTxStart.SuspendLayout();
			pnlAddressFiltering.SuspendLayout();
			pnlPacketFormat.SuspendLayout();
			tableLayoutPanel1.SuspendLayout();
			pnlPayloadLength.SuspendLayout();
			((ISupportInitialize)nudPayloadLength).BeginInit();
			((ISupportInitialize)nudSyncSize).BeginInit();
			panel1.SuspendLayout();
			pnlNodeAddress.SuspendLayout();
			((ISupportInitialize)nudNodeAddress).BeginInit();
			pnlBroadcastAddress.SuspendLayout();
			((ISupportInitialize)nudBroadcastAddress).BeginInit();
			tableLayoutPanel2.SuspendLayout();
			((ISupportInitialize)nudFifoThreshold).BeginInit();
			panel2.SuspendLayout();
			panel3.SuspendLayout();
			panel4.SuspendLayout();
			panel5.SuspendLayout();
			gBoxDeviceStatus.SuspendLayout();
			gBoxControl.SuspendLayout();
			gBoxPacket.SuspendLayout();
			gBoxMessage.SuspendLayout();
			tblPayloadMessage.SuspendLayout();
			tblPacket.SuspendLayout();
			pnlPacketCrc.SuspendLayout();
			pnlPacketAddr.SuspendLayout();
			SuspendLayout();
			errorProvider.ContainerControl = this;
			tBoxSyncValue.Anchor = AnchorStyles.Left;
			errorProvider.SetIconPadding(tBoxSyncValue, 6);
			tBoxSyncValue.InsertKeyMode = InsertKeyMode.Overwrite;
			tBoxSyncValue.Location = new Point(163, 172);
			tBoxSyncValue.Margin = new Padding(3, 2, 3, 2);
			tBoxSyncValue.Mask = "&&-&&-&&-&&-&&-&&-&&-&&";
			tBoxSyncValue.Name = "tBoxSyncValue";
			tBoxSyncValue.Size = new Size(143, 20);
			tBoxSyncValue.TabIndex = 14;
			tBoxSyncValue.Text = "AAAAAAAAAAAAAAAA";
			tBoxSyncValue.MaskInputRejected += tBoxSyncValue_MaskInputRejected;
			tBoxSyncValue.TypeValidationCompleted += tBoxSyncValue_TypeValidationCompleted;
			tBoxSyncValue.TextChanged += tBoxSyncValue_TextChanged;
			tBoxSyncValue.KeyDown += tBoxSyncValue_KeyDown;
			tBoxSyncValue.MouseEnter += control_MouseEnter;
			tBoxSyncValue.MouseLeave += control_MouseLeave;
			tBoxSyncValue.Validated += tBox_Validated;
			nudPreambleSize.Anchor = AnchorStyles.Left;
			errorProvider.SetIconPadding(nudPreambleSize, 6);
			nudPreambleSize.Location = new Point(163, 27);
			nudPreambleSize.Margin = new Padding(3, 2, 3, 2);
			nudPreambleSize.Maximum = new decimal([65535, 0, 0, 0]);
			nudPreambleSize.Name = "nudPreambleSize";
			nudPreambleSize.Size = new Size(59, 20);
			nudPreambleSize.TabIndex = 1;
			nudPreambleSize.Value = new decimal([3, 0, 0, 0]);
			nudPreambleSize.MouseEnter += control_MouseEnter;
			nudPreambleSize.MouseLeave += control_MouseLeave;
			nudPreambleSize.ValueChanged += nudPreambleSize_ValueChanged;
			label12.Anchor = AnchorStyles.None;
			label12.AutoSize = true;
			label12.Location = new Point(356, 223);
			label12.Name = "label12";
			label12.Size = new Size(32, 13);
			label12.TabIndex = 19;
			label12.Text = "bytes";
			label12.TextAlign = ContentAlignment.MiddleLeft;
			label1.Anchor = AnchorStyles.Left;
			label1.AutoSize = true;
			label1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			label1.Location = new Point(3, 30);
			label1.Name = "label1";
			label1.Size = new Size(75, 13);
			label1.TabIndex = 0;
			label1.Text = "Preamble size:";
			label1.TextAlign = ContentAlignment.MiddleLeft;
			label2.Anchor = AnchorStyles.None;
			label2.AutoSize = true;
			label2.Location = new Point(356, 30);
			label2.Name = "label2";
			label2.Size = new Size(32, 13);
			label2.TabIndex = 2;
			label2.Text = "bytes";
			label2.TextAlign = ContentAlignment.MiddleLeft;
			label18.Anchor = AnchorStyles.Left;
			label18.AutoSize = true;
			label18.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			label18.Location = new Point(3, 29);
			label18.Name = "label18";
			label18.Size = new Size(116, 13);
			label18.TabIndex = 2;
			label18.Text = "Address based filtering:";
			label18.TextAlign = ContentAlignment.MiddleLeft;
			label11.Anchor = AnchorStyles.Left;
			label11.AutoSize = true;
			label11.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			label11.Location = new Point(3, 223);
			label11.Name = "label11";
			label11.Size = new Size(80, 13);
			label11.TabIndex = 17;
			label11.Text = "Payload length:";
			label11.TextAlign = ContentAlignment.MiddleLeft;
			label20.Anchor = AnchorStyles.Left;
			label20.AutoSize = true;
			label20.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			label20.Location = new Point(3, 77);
			label20.Name = "label20";
			label20.Size = new Size(98, 13);
			label20.TabIndex = 5;
			label20.Text = "Broadcast address:";
			label20.TextAlign = ContentAlignment.MiddleLeft;
			label10.Anchor = AnchorStyles.Left;
			label10.AutoSize = true;
			label10.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			label10.Location = new Point(3, 199);
			label10.Name = "label10";
			label10.Size = new Size(76, 13);
			label10.TabIndex = 15;
			label10.Text = "Packet format:";
			label10.TextAlign = ContentAlignment.MiddleLeft;
			label21.Anchor = AnchorStyles.Left;
			label21.AutoSize = true;
			label21.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			label21.Location = new Point(3, 101);
			label21.Name = "label21";
			label21.Size = new Size(46, 13);
			label21.TabIndex = 6;
			label21.Text = "DC-free:";
			label21.TextAlign = ContentAlignment.MiddleLeft;
			label19.Anchor = AnchorStyles.Left;
			label19.AutoSize = true;
			label19.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			label19.Location = new Point(3, 53);
			label19.Name = "label19";
			label19.Size = new Size(76, 13);
			label19.TabIndex = 4;
			label19.Text = "Node address:";
			label19.TextAlign = ContentAlignment.MiddleLeft;
			label27.Anchor = AnchorStyles.Left;
			label27.AutoSize = true;
			label27.Location = new Point(3, 222);
			label27.Name = "label27";
			label27.Size = new Size(83, 13);
			label27.TabIndex = 18;
			label27.Text = "FIFO Threshold:";
			label27.TextAlign = ContentAlignment.MiddleLeft;
			label26.Anchor = AnchorStyles.Left;
			label26.AutoSize = true;
			label26.Location = new Point(3, 198);
			label26.Name = "label26";
			label26.Size = new Size(91, 13);
			label26.TabIndex = 16;
			label26.Text = "Tx start condition:";
			label26.TextAlign = ContentAlignment.MiddleLeft;
			pnlDcFree.Anchor = AnchorStyles.Left;
			pnlDcFree.AutoSize = true;
			pnlDcFree.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlDcFree.Controls.Add(rBtnDcFreeWhitening);
			pnlDcFree.Controls.Add(rBtnDcFreeManchester);
			pnlDcFree.Controls.Add(rBtnDcFreeOff);
			pnlDcFree.Location = new Point(129, 98);
			pnlDcFree.Margin = new Padding(3, 2, 3, 2);
			pnlDcFree.Name = "pnlDcFree";
			pnlDcFree.Size = new Size(217, 20);
			pnlDcFree.TabIndex = 7;
			pnlDcFree.MouseEnter += control_MouseEnter;
			pnlDcFree.MouseLeave += control_MouseLeave;
			rBtnDcFreeWhitening.AutoSize = true;
			rBtnDcFreeWhitening.Location = new Point(141, 3);
			rBtnDcFreeWhitening.Margin = new Padding(3, 0, 3, 0);
			rBtnDcFreeWhitening.Name = "rBtnDcFreeWhitening";
			rBtnDcFreeWhitening.Size = new Size(73, 17);
			rBtnDcFreeWhitening.TabIndex = 2;
			rBtnDcFreeWhitening.Text = "Whitening";
			rBtnDcFreeWhitening.UseVisualStyleBackColor = true;
			rBtnDcFreeWhitening.CheckedChanged += rBtnDcFreeWhitening_CheckedChanged;
			rBtnDcFreeWhitening.MouseEnter += control_MouseEnter;
			rBtnDcFreeWhitening.MouseLeave += control_MouseLeave;
			rBtnDcFreeManchester.AutoSize = true;
			rBtnDcFreeManchester.Location = new Point(54, 3);
			rBtnDcFreeManchester.Margin = new Padding(3, 0, 3, 0);
			rBtnDcFreeManchester.Name = "rBtnDcFreeManchester";
			rBtnDcFreeManchester.Size = new Size(81, 17);
			rBtnDcFreeManchester.TabIndex = 1;
			rBtnDcFreeManchester.Text = "Manchester";
			rBtnDcFreeManchester.UseVisualStyleBackColor = true;
			rBtnDcFreeManchester.CheckedChanged += rBtnDcFreeManchester_CheckedChanged;
			rBtnDcFreeManchester.MouseEnter += control_MouseEnter;
			rBtnDcFreeManchester.MouseLeave += control_MouseLeave;
			rBtnDcFreeOff.AutoSize = true;
			rBtnDcFreeOff.Checked = true;
			rBtnDcFreeOff.Location = new Point(3, 3);
			rBtnDcFreeOff.Margin = new Padding(3, 0, 3, 0);
			rBtnDcFreeOff.Name = "rBtnDcFreeOff";
			rBtnDcFreeOff.Size = new Size(45, 17);
			rBtnDcFreeOff.TabIndex = 0;
			rBtnDcFreeOff.TabStop = true;
			rBtnDcFreeOff.Text = "OFF";
			rBtnDcFreeOff.UseVisualStyleBackColor = true;
			rBtnDcFreeOff.CheckedChanged += rBtnDcFreeOff_CheckedChanged;
			rBtnDcFreeOff.MouseEnter += control_MouseEnter;
			rBtnDcFreeOff.MouseLeave += control_MouseLeave;
			pnlAddressInPayload.Anchor = AnchorStyles.Left;
			pnlAddressInPayload.AutoSize = true;
			pnlAddressInPayload.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlAddressInPayload.Controls.Add(rBtnNodeAddressInPayloadNo);
			pnlAddressInPayload.Controls.Add(rBtnNodeAddressInPayloadYes);
			pnlAddressInPayload.Location = new Point(129, 2);
			pnlAddressInPayload.Margin = new Padding(3, 2, 3, 2);
			pnlAddressInPayload.Name = "pnlAddressInPayload";
			pnlAddressInPayload.Size = new Size(98, 20);
			pnlAddressInPayload.TabIndex = 1;
			pnlAddressInPayload.Visible = false;
			pnlAddressInPayload.MouseEnter += control_MouseEnter;
			pnlAddressInPayload.MouseLeave += control_MouseLeave;
			rBtnNodeAddressInPayloadNo.AutoSize = true;
			rBtnNodeAddressInPayloadNo.Location = new Point(54, 3);
			rBtnNodeAddressInPayloadNo.Margin = new Padding(3, 0, 3, 0);
			rBtnNodeAddressInPayloadNo.Name = "rBtnNodeAddressInPayloadNo";
			rBtnNodeAddressInPayloadNo.Size = new Size(41, 17);
			rBtnNodeAddressInPayloadNo.TabIndex = 1;
			rBtnNodeAddressInPayloadNo.Text = "NO";
			rBtnNodeAddressInPayloadNo.UseVisualStyleBackColor = true;
			rBtnNodeAddressInPayloadNo.MouseEnter += control_MouseEnter;
			rBtnNodeAddressInPayloadNo.MouseLeave += control_MouseLeave;
			rBtnNodeAddressInPayloadYes.AutoSize = true;
			rBtnNodeAddressInPayloadYes.Checked = true;
			rBtnNodeAddressInPayloadYes.Location = new Point(3, 3);
			rBtnNodeAddressInPayloadYes.Margin = new Padding(3, 0, 3, 0);
			rBtnNodeAddressInPayloadYes.Name = "rBtnNodeAddressInPayloadYes";
			rBtnNodeAddressInPayloadYes.Size = new Size(46, 17);
			rBtnNodeAddressInPayloadYes.TabIndex = 0;
			rBtnNodeAddressInPayloadYes.TabStop = true;
			rBtnNodeAddressInPayloadYes.Text = "YES";
			rBtnNodeAddressInPayloadYes.UseVisualStyleBackColor = true;
			rBtnNodeAddressInPayloadYes.MouseEnter += control_MouseEnter;
			rBtnNodeAddressInPayloadYes.MouseLeave += control_MouseLeave;
			label17.Anchor = AnchorStyles.Left;
			label17.AutoSize = true;
			label17.Location = new Point(3, 5);
			label17.Name = "label17";
			label17.Size = new Size(120, 13);
			label17.TabIndex = 0;
			label17.Text = "Add address in payload:";
			label17.TextAlign = ContentAlignment.MiddleLeft;
			label17.Visible = false;
			pnlFifoFillCondition.Anchor = AnchorStyles.Left;
			pnlFifoFillCondition.AutoSize = true;
			pnlFifoFillCondition.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlFifoFillCondition.Controls.Add(rBtnFifoFillAlways);
			pnlFifoFillCondition.Controls.Add(rBtnFifoFillSyncAddress);
			pnlFifoFillCondition.Location = new Point(163, 124);
			pnlFifoFillCondition.Margin = new Padding(3, 2, 3, 2);
			pnlFifoFillCondition.Name = "pnlFifoFillCondition";
			pnlFifoFillCondition.Size = new Size(159, 20);
			pnlFifoFillCondition.TabIndex = 6;
			pnlFifoFillCondition.MouseEnter += control_MouseEnter;
			pnlFifoFillCondition.MouseLeave += control_MouseLeave;
			rBtnFifoFillAlways.AutoSize = true;
			rBtnFifoFillAlways.Location = new Point(98, 3);
			rBtnFifoFillAlways.Margin = new Padding(3, 0, 3, 0);
			rBtnFifoFillAlways.Name = "rBtnFifoFillAlways";
			rBtnFifoFillAlways.Size = new Size(58, 17);
			rBtnFifoFillAlways.TabIndex = 1;
			rBtnFifoFillAlways.Text = "Always";
			rBtnFifoFillAlways.UseVisualStyleBackColor = true;
			rBtnFifoFillAlways.CheckedChanged += rBtnFifoFill_CheckedChanged;
			rBtnFifoFillAlways.MouseEnter += control_MouseEnter;
			rBtnFifoFillAlways.MouseLeave += control_MouseLeave;
			rBtnFifoFillSyncAddress.AutoSize = true;
			rBtnFifoFillSyncAddress.Checked = true;
			rBtnFifoFillSyncAddress.Location = new Point(3, 3);
			rBtnFifoFillSyncAddress.Margin = new Padding(3, 0, 3, 0);
			rBtnFifoFillSyncAddress.Name = "rBtnFifoFillSyncAddress";
			rBtnFifoFillSyncAddress.Size = new Size(89, 17);
			rBtnFifoFillSyncAddress.TabIndex = 0;
			rBtnFifoFillSyncAddress.TabStop = true;
			rBtnFifoFillSyncAddress.Text = "Sync address";
			rBtnFifoFillSyncAddress.UseVisualStyleBackColor = true;
			rBtnFifoFillSyncAddress.CheckedChanged += rBtnFifoFill_CheckedChanged;
			rBtnFifoFillSyncAddress.MouseEnter += control_MouseEnter;
			rBtnFifoFillSyncAddress.MouseLeave += control_MouseLeave;
			label4.Anchor = AnchorStyles.Left;
			label4.AutoSize = true;
			label4.Location = new Point(3, 127);
			label4.Name = "label4";
			label4.Size = new Size(91, 13);
			label4.TabIndex = 5;
			label4.Text = "FIFO fill condition:";
			label4.TextAlign = ContentAlignment.MiddleLeft;
			pnlSync.Anchor = AnchorStyles.Left;
			pnlSync.AutoSize = true;
			pnlSync.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlSync.Controls.Add(rBtnSyncOff);
			pnlSync.Controls.Add(rBtnSyncOn);
			pnlSync.Location = new Point(163, 100);
			pnlSync.Margin = new Padding(3, 2, 3, 2);
			pnlSync.Name = "pnlSync";
			pnlSync.Size = new Size(98, 20);
			pnlSync.TabIndex = 4;
			pnlSync.MouseEnter += control_MouseEnter;
			pnlSync.MouseLeave += control_MouseLeave;
			rBtnSyncOff.AutoSize = true;
			rBtnSyncOff.Location = new Point(50, 3);
			rBtnSyncOff.Margin = new Padding(3, 0, 3, 0);
			rBtnSyncOff.Name = "rBtnSyncOff";
			rBtnSyncOff.Size = new Size(45, 17);
			rBtnSyncOff.TabIndex = 1;
			rBtnSyncOff.Text = "OFF";
			rBtnSyncOff.UseVisualStyleBackColor = true;
			rBtnSyncOff.CheckedChanged += rBtnSyncOn_CheckedChanged;
			rBtnSyncOff.MouseEnter += control_MouseEnter;
			rBtnSyncOff.MouseLeave += control_MouseLeave;
			rBtnSyncOn.AutoSize = true;
			rBtnSyncOn.Checked = true;
			rBtnSyncOn.Location = new Point(3, 3);
			rBtnSyncOn.Margin = new Padding(3, 0, 3, 0);
			rBtnSyncOn.Name = "rBtnSyncOn";
			rBtnSyncOn.Size = new Size(41, 17);
			rBtnSyncOn.TabIndex = 0;
			rBtnSyncOn.TabStop = true;
			rBtnSyncOn.Text = "ON";
			rBtnSyncOn.UseVisualStyleBackColor = true;
			rBtnSyncOn.CheckedChanged += rBtnSyncOn_CheckedChanged;
			rBtnSyncOn.MouseEnter += control_MouseEnter;
			rBtnSyncOn.MouseLeave += control_MouseLeave;
			label3.Anchor = AnchorStyles.Left;
			label3.AutoSize = true;
			label3.Location = new Point(3, 103);
			label3.Name = "label3";
			label3.Size = new Size(60, 13);
			label3.TabIndex = 3;
			label3.Text = "Sync word:";
			label3.TextAlign = ContentAlignment.MiddleLeft;
			label9.Anchor = AnchorStyles.Left;
			label9.AutoSize = true;
			label9.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			label9.Location = new Point(3, 175);
			label9.Name = "label9";
			label9.Size = new Size(89, 13);
			label9.TabIndex = 13;
			label9.Text = "Sync word value:";
			label9.TextAlign = ContentAlignment.MiddleLeft;
			pnlCrcAutoClear.Anchor = AnchorStyles.Left;
			pnlCrcAutoClear.AutoSize = true;
			pnlCrcAutoClear.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlCrcAutoClear.Controls.Add(rBtnCrcAutoClearOff);
			pnlCrcAutoClear.Controls.Add(rBtnCrcAutoClearOn);
			pnlCrcAutoClear.Location = new Point(129, 146);
			pnlCrcAutoClear.Margin = new Padding(3, 2, 3, 2);
			pnlCrcAutoClear.Name = "pnlCrcAutoClear";
			pnlCrcAutoClear.Size = new Size(102, 20);
			pnlCrcAutoClear.TabIndex = 11;
			pnlCrcAutoClear.MouseEnter += control_MouseEnter;
			pnlCrcAutoClear.MouseLeave += control_MouseLeave;
			rBtnCrcAutoClearOff.AutoSize = true;
			rBtnCrcAutoClearOff.Location = new Point(54, 3);
			rBtnCrcAutoClearOff.Margin = new Padding(3, 0, 3, 0);
			rBtnCrcAutoClearOff.Name = "rBtnCrcAutoClearOff";
			rBtnCrcAutoClearOff.Size = new Size(45, 17);
			rBtnCrcAutoClearOff.TabIndex = 1;
			rBtnCrcAutoClearOff.Text = "OFF";
			rBtnCrcAutoClearOff.UseVisualStyleBackColor = true;
			rBtnCrcAutoClearOff.CheckedChanged += rBtnCrcAutoClearOff_CheckedChanged;
			rBtnCrcAutoClearOff.MouseEnter += control_MouseEnter;
			rBtnCrcAutoClearOff.MouseLeave += control_MouseLeave;
			rBtnCrcAutoClearOn.AutoSize = true;
			rBtnCrcAutoClearOn.Checked = true;
			rBtnCrcAutoClearOn.Location = new Point(3, 3);
			rBtnCrcAutoClearOn.Margin = new Padding(3, 0, 3, 0);
			rBtnCrcAutoClearOn.Name = "rBtnCrcAutoClearOn";
			rBtnCrcAutoClearOn.Size = new Size(41, 17);
			rBtnCrcAutoClearOn.TabIndex = 0;
			rBtnCrcAutoClearOn.TabStop = true;
			rBtnCrcAutoClearOn.Text = "ON";
			rBtnCrcAutoClearOn.UseVisualStyleBackColor = true;
			rBtnCrcAutoClearOn.CheckedChanged += rBtnCrcAutoClearOn_CheckedChanged;
			rBtnCrcAutoClearOn.MouseEnter += control_MouseEnter;
			rBtnCrcAutoClearOn.MouseLeave += control_MouseLeave;
			label23.Anchor = AnchorStyles.Left;
			label23.AutoSize = true;
			label23.Location = new Point(3, 149);
			label23.Name = "label23";
			label23.Size = new Size(82, 13);
			label23.TabIndex = 10;
			label23.Text = "CRC auto clear:";
			label23.TextAlign = ContentAlignment.MiddleLeft;
			pnlCrcCalculation.Anchor = AnchorStyles.Left;
			pnlCrcCalculation.AutoSize = true;
			pnlCrcCalculation.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlCrcCalculation.Controls.Add(rBtnCrcOff);
			pnlCrcCalculation.Controls.Add(rBtnCrcOn);
			pnlCrcCalculation.Location = new Point(129, 122);
			pnlCrcCalculation.Margin = new Padding(3, 2, 3, 2);
			pnlCrcCalculation.Name = "pnlCrcCalculation";
			pnlCrcCalculation.Size = new Size(102, 20);
			pnlCrcCalculation.TabIndex = 9;
			pnlCrcCalculation.MouseEnter += control_MouseEnter;
			pnlCrcCalculation.MouseLeave += control_MouseLeave;
			rBtnCrcOff.AutoSize = true;
			rBtnCrcOff.Location = new Point(54, 3);
			rBtnCrcOff.Margin = new Padding(3, 0, 3, 0);
			rBtnCrcOff.Name = "rBtnCrcOff";
			rBtnCrcOff.Size = new Size(45, 17);
			rBtnCrcOff.TabIndex = 1;
			rBtnCrcOff.Text = "OFF";
			rBtnCrcOff.UseVisualStyleBackColor = true;
			rBtnCrcOff.CheckedChanged += rBtnCrcOff_CheckedChanged;
			rBtnCrcOff.MouseEnter += control_MouseEnter;
			rBtnCrcOff.MouseLeave += control_MouseLeave;
			rBtnCrcOn.AutoSize = true;
			rBtnCrcOn.Checked = true;
			rBtnCrcOn.Location = new Point(3, 3);
			rBtnCrcOn.Margin = new Padding(3, 0, 3, 0);
			rBtnCrcOn.Name = "rBtnCrcOn";
			rBtnCrcOn.Size = new Size(41, 17);
			rBtnCrcOn.TabIndex = 0;
			rBtnCrcOn.TabStop = true;
			rBtnCrcOn.Text = "ON";
			rBtnCrcOn.UseVisualStyleBackColor = true;
			rBtnCrcOn.CheckedChanged += rBtnCrcOn_CheckedChanged;
			rBtnCrcOn.MouseEnter += control_MouseEnter;
			rBtnCrcOn.MouseLeave += control_MouseLeave;
			label22.Anchor = AnchorStyles.Left;
			label22.AutoSize = true;
			label22.Location = new Point(3, 125);
			label22.Name = "label22";
			label22.Size = new Size(86, 13);
			label22.TabIndex = 8;
			label22.Text = "CRC calculation:";
			label22.TextAlign = ContentAlignment.MiddleLeft;
			pnlTxStart.Anchor = AnchorStyles.Left;
			pnlTxStart.AutoSize = true;
			pnlTxStart.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlTxStart.Controls.Add(rBtnTxStartFifoNotEmpty);
			pnlTxStart.Controls.Add(rBtnTxStartFifoLevel);
			pnlTxStart.Location = new Point(129, 195);
			pnlTxStart.Margin = new Padding(3, 3, 3, 2);
			pnlTxStart.Name = "pnlTxStart";
			pnlTxStart.Size = new Size(168, 20);
			pnlTxStart.TabIndex = 17;
			pnlTxStart.MouseEnter += control_MouseEnter;
			pnlTxStart.MouseLeave += control_MouseLeave;
			rBtnTxStartFifoNotEmpty.AutoSize = true;
			rBtnTxStartFifoNotEmpty.Checked = true;
			rBtnTxStartFifoNotEmpty.Location = new Point(77, 3);
			rBtnTxStartFifoNotEmpty.Margin = new Padding(3, 0, 3, 0);
			rBtnTxStartFifoNotEmpty.Name = "rBtnTxStartFifoNotEmpty";
			rBtnTxStartFifoNotEmpty.Size = new Size(88, 17);
			rBtnTxStartFifoNotEmpty.TabIndex = 1;
			rBtnTxStartFifoNotEmpty.TabStop = true;
			rBtnTxStartFifoNotEmpty.Text = "FifoNotEmpty";
			rBtnTxStartFifoNotEmpty.UseVisualStyleBackColor = true;
			rBtnTxStartFifoNotEmpty.CheckedChanged += rBtnTxStartFifoNotEmpty_CheckedChanged;
			rBtnTxStartFifoNotEmpty.MouseEnter += control_MouseEnter;
			rBtnTxStartFifoNotEmpty.MouseLeave += control_MouseLeave;
			rBtnTxStartFifoLevel.AutoSize = true;
			rBtnTxStartFifoLevel.Location = new Point(3, 3);
			rBtnTxStartFifoLevel.Margin = new Padding(3, 0, 3, 0);
			rBtnTxStartFifoLevel.Name = "rBtnTxStartFifoLevel";
			rBtnTxStartFifoLevel.Size = new Size(68, 17);
			rBtnTxStartFifoLevel.TabIndex = 0;
			rBtnTxStartFifoLevel.Text = "FifoLevel";
			rBtnTxStartFifoLevel.UseVisualStyleBackColor = true;
			rBtnTxStartFifoLevel.CheckedChanged += rBtnTxStartFifoLevel_CheckedChanged;
			rBtnTxStartFifoLevel.MouseEnter += control_MouseEnter;
			rBtnTxStartFifoLevel.MouseLeave += control_MouseLeave;
			pnlAddressFiltering.Anchor = AnchorStyles.Left;
			pnlAddressFiltering.AutoSize = true;
			pnlAddressFiltering.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlAddressFiltering.Controls.Add(rBtnAddressFilteringNodeBroadcast);
			pnlAddressFiltering.Controls.Add(rBtnAddressFilteringNode);
			pnlAddressFiltering.Controls.Add(rBtnAddressFilteringOff);
			pnlAddressFiltering.Location = new Point(129, 26);
			pnlAddressFiltering.Margin = new Padding(3, 2, 3, 2);
			pnlAddressFiltering.Name = "pnlAddressFiltering";
			pnlAddressFiltering.Size = new Size(228, 20);
			pnlAddressFiltering.TabIndex = 3;
			pnlAddressFiltering.MouseEnter += control_MouseEnter;
			pnlAddressFiltering.MouseLeave += control_MouseLeave;
			rBtnAddressFilteringNodeBroadcast.AutoSize = true;
			rBtnAddressFilteringNodeBroadcast.Location = new Point(111, 3);
			rBtnAddressFilteringNodeBroadcast.Margin = new Padding(3, 0, 3, 0);
			rBtnAddressFilteringNodeBroadcast.Name = "rBtnAddressFilteringNodeBroadcast";
			rBtnAddressFilteringNodeBroadcast.Size = new Size(114, 17);
			rBtnAddressFilteringNodeBroadcast.TabIndex = 2;
			rBtnAddressFilteringNodeBroadcast.Text = "Node or Broadcast";
			rBtnAddressFilteringNodeBroadcast.UseVisualStyleBackColor = true;
			rBtnAddressFilteringNodeBroadcast.CheckedChanged += rBtnAddressFilteringNodeBroadcast_CheckedChanged;
			rBtnAddressFilteringNodeBroadcast.MouseEnter += control_MouseEnter;
			rBtnAddressFilteringNodeBroadcast.MouseLeave += control_MouseLeave;
			rBtnAddressFilteringNode.AutoSize = true;
			rBtnAddressFilteringNode.Location = new Point(54, 3);
			rBtnAddressFilteringNode.Margin = new Padding(3, 0, 3, 0);
			rBtnAddressFilteringNode.Name = "rBtnAddressFilteringNode";
			rBtnAddressFilteringNode.Size = new Size(51, 17);
			rBtnAddressFilteringNode.TabIndex = 1;
			rBtnAddressFilteringNode.Text = "Node";
			rBtnAddressFilteringNode.UseVisualStyleBackColor = true;
			rBtnAddressFilteringNode.CheckedChanged += rBtnAddressFilteringNode_CheckedChanged;
			rBtnAddressFilteringNode.MouseEnter += control_MouseEnter;
			rBtnAddressFilteringNode.MouseLeave += control_MouseLeave;
			rBtnAddressFilteringOff.AutoSize = true;
			rBtnAddressFilteringOff.Checked = true;
			rBtnAddressFilteringOff.Location = new Point(3, 3);
			rBtnAddressFilteringOff.Margin = new Padding(3, 0, 3, 0);
			rBtnAddressFilteringOff.Name = "rBtnAddressFilteringOff";
			rBtnAddressFilteringOff.Size = new Size(45, 17);
			rBtnAddressFilteringOff.TabIndex = 0;
			rBtnAddressFilteringOff.TabStop = true;
			rBtnAddressFilteringOff.Text = "OFF";
			rBtnAddressFilteringOff.UseVisualStyleBackColor = true;
			rBtnAddressFilteringOff.CheckedChanged += rBtnAddressFilteringOff_CheckedChanged;
			rBtnAddressFilteringOff.MouseEnter += control_MouseEnter;
			rBtnAddressFilteringOff.MouseLeave += control_MouseLeave;
			lblNodeAddress.BorderStyle = BorderStyle.Fixed3D;
			lblNodeAddress.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblNodeAddress.Location = new Point(65, 0);
			lblNodeAddress.Name = "lblNodeAddress";
			lblNodeAddress.Size = new Size(59, 20);
			lblNodeAddress.TabIndex = 1;
			lblNodeAddress.Text = "0x00";
			lblNodeAddress.TextAlign = ContentAlignment.MiddleCenter;
			lblNodeAddress.MouseEnter += control_MouseEnter;
			lblNodeAddress.MouseLeave += control_MouseLeave;
			lblPayloadLength.BorderStyle = BorderStyle.Fixed3D;
			lblPayloadLength.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblPayloadLength.Location = new Point(65, 0);
			lblPayloadLength.Name = "lblPayloadLength";
			lblPayloadLength.Size = new Size(59, 20);
			lblPayloadLength.TabIndex = 1;
			lblPayloadLength.Text = "0x00";
			lblPayloadLength.TextAlign = ContentAlignment.MiddleCenter;
			lblPayloadLength.MouseEnter += control_MouseEnter;
			lblPayloadLength.MouseLeave += control_MouseLeave;
			lblBroadcastAddress.BorderStyle = BorderStyle.Fixed3D;
			lblBroadcastAddress.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblBroadcastAddress.Location = new Point(65, 0);
			lblBroadcastAddress.Name = "lblBroadcastAddress";
			lblBroadcastAddress.Size = new Size(59, 20);
			lblBroadcastAddress.TabIndex = 1;
			lblBroadcastAddress.Text = "0x00";
			lblBroadcastAddress.TextAlign = ContentAlignment.MiddleCenter;
			lblBroadcastAddress.MouseEnter += control_MouseEnter;
			lblBroadcastAddress.MouseLeave += control_MouseLeave;
			pnlPacketFormat.Anchor = AnchorStyles.Left;
			pnlPacketFormat.AutoSize = true;
			pnlPacketFormat.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlPacketFormat.Controls.Add(rBtnPacketFormatFixed);
			pnlPacketFormat.Controls.Add(rBtnPacketFormatVariable);
			pnlPacketFormat.Location = new Point(163, 196);
			pnlPacketFormat.Margin = new Padding(3, 2, 3, 2);
			pnlPacketFormat.Name = "pnlPacketFormat";
			pnlPacketFormat.Size = new Size(125, 20);
			pnlPacketFormat.TabIndex = 16;
			pnlPacketFormat.MouseEnter += control_MouseEnter;
			pnlPacketFormat.MouseLeave += control_MouseLeave;
			rBtnPacketFormatFixed.AutoSize = true;
			rBtnPacketFormatFixed.Location = new Point(72, 3);
			rBtnPacketFormatFixed.Margin = new Padding(3, 0, 3, 0);
			rBtnPacketFormatFixed.Name = "rBtnPacketFormatFixed";
			rBtnPacketFormatFixed.Size = new Size(50, 17);
			rBtnPacketFormatFixed.TabIndex = 1;
			rBtnPacketFormatFixed.Text = "Fixed";
			rBtnPacketFormatFixed.UseVisualStyleBackColor = true;
			rBtnPacketFormatFixed.CheckedChanged += rBtnPacketFormat_CheckedChanged;
			rBtnPacketFormatFixed.MouseEnter += control_MouseEnter;
			rBtnPacketFormatFixed.MouseLeave += control_MouseLeave;
			rBtnPacketFormatVariable.AutoSize = true;
			rBtnPacketFormatVariable.Checked = true;
			rBtnPacketFormatVariable.Location = new Point(3, 3);
			rBtnPacketFormatVariable.Margin = new Padding(3, 0, 3, 0);
			rBtnPacketFormatVariable.Name = "rBtnPacketFormatVariable";
			rBtnPacketFormatVariable.Size = new Size(63, 17);
			rBtnPacketFormatVariable.TabIndex = 0;
			rBtnPacketFormatVariable.TabStop = true;
			rBtnPacketFormatVariable.Text = "Variable";
			rBtnPacketFormatVariable.UseVisualStyleBackColor = true;
			rBtnPacketFormatVariable.CheckedChanged += rBtnPacketFormat_CheckedChanged;
			rBtnPacketFormatVariable.MouseEnter += control_MouseEnter;
			rBtnPacketFormatVariable.MouseLeave += control_MouseLeave;
			tableLayoutPanel1.AutoSize = true;
			tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			tableLayoutPanel1.ColumnCount = 3;
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160f));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
			tableLayoutPanel1.Controls.Add(pnlPayloadLength, 1, 9);
			tableLayoutPanel1.Controls.Add(label1, 0, 1);
			tableLayoutPanel1.Controls.Add(pnlPacketFormat, 1, 8);
			tableLayoutPanel1.Controls.Add(label3, 0, 4);
			tableLayoutPanel1.Controls.Add(label4, 0, 5);
			tableLayoutPanel1.Controls.Add(label5, 0, 6);
			tableLayoutPanel1.Controls.Add(label9, 0, 7);
			tableLayoutPanel1.Controls.Add(label10, 0, 8);
			tableLayoutPanel1.Controls.Add(label11, 0, 9);
			tableLayoutPanel1.Controls.Add(pnlFifoFillCondition, 1, 5);
			tableLayoutPanel1.Controls.Add(pnlSync, 1, 4);
			tableLayoutPanel1.Controls.Add(tBoxSyncValue, 1, 7);
			tableLayoutPanel1.Controls.Add(label12, 2, 9);
			tableLayoutPanel1.Controls.Add(nudPreambleSize, 1, 1);
			tableLayoutPanel1.Controls.Add(label2, 2, 1);
			tableLayoutPanel1.Controls.Add(nudSyncSize, 1, 6);
			tableLayoutPanel1.Controls.Add(label6, 2, 6);
			tableLayoutPanel1.Controls.Add(panel1, 1, 3);
			tableLayoutPanel1.Controls.Add(label7, 0, 3);
			tableLayoutPanel1.Controls.Add(cBoxDataMode, 1, 0);
			tableLayoutPanel1.Controls.Add(label24, 0, 0);
			tableLayoutPanel1.Controls.Add(label25, 0, 2);
			tableLayoutPanel1.Controls.Add(cBoxAutoRestartRxMode, 1, 2);
			tableLayoutPanel1.Location = new Point(18, 3);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 10;
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.Size = new Size(391, 242);
			tableLayoutPanel1.TabIndex = 0;
			pnlPayloadLength.Anchor = AnchorStyles.Left;
			pnlPayloadLength.AutoSize = true;
			pnlPayloadLength.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlPayloadLength.Controls.Add(lblPayloadLength);
			pnlPayloadLength.Controls.Add(nudPayloadLength);
			pnlPayloadLength.Location = new Point(163, 220);
			pnlPayloadLength.Margin = new Padding(3, 2, 3, 2);
			pnlPayloadLength.Name = "pnlPayloadLength";
			pnlPayloadLength.Size = new Size(127, 20);
			pnlPayloadLength.TabIndex = 18;
			pnlPayloadLength.MouseEnter += control_MouseEnter;
			pnlPayloadLength.MouseLeave += control_MouseLeave;
			nudPayloadLength.Location = new Point(3, 0);
			nudPayloadLength.Margin = new Padding(3, 0, 3, 0);
			nudPayloadLength.Maximum = new decimal([66, 0, 0, 0]);
			nudPayloadLength.Name = "nudPayloadLength";
			nudPayloadLength.Size = new Size(59, 20);
			nudPayloadLength.TabIndex = 0;
			nudPayloadLength.Value = new decimal([66, 0, 0, 0]);
			nudPayloadLength.MouseEnter += control_MouseEnter;
			nudPayloadLength.MouseLeave += control_MouseLeave;
			nudPayloadLength.ValueChanged += nudPayloadLength_ValueChanged;
			label5.Anchor = AnchorStyles.Left;
			label5.AutoSize = true;
			label5.BackColor = Color.Transparent;
			label5.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			label5.Location = new Point(3, 151);
			label5.Name = "label5";
			label5.Size = new Size(81, 13);
			label5.TabIndex = 7;
			label5.Text = "Sync word size:";
			label5.TextAlign = ContentAlignment.MiddleLeft;
			nudSyncSize.Anchor = AnchorStyles.Left;
			nudSyncSize.Location = new Point(163, 148);
			nudSyncSize.Margin = new Padding(3, 2, 3, 2);
			nudSyncSize.Maximum = new decimal([8, 0, 0, 0]);
			nudSyncSize.Minimum = new decimal([1, 0, 0, 0]);
			nudSyncSize.Name = "nudSyncSize";
			nudSyncSize.Size = new Size(59, 20);
			nudSyncSize.TabIndex = 8;
			nudSyncSize.Value = new decimal([4, 0, 0, 0]);
			nudSyncSize.MouseEnter += control_MouseEnter;
			nudSyncSize.MouseLeave += control_MouseLeave;
			nudSyncSize.ValueChanged += nudSyncSize_ValueChanged;
			label6.Anchor = AnchorStyles.None;
			label6.AutoSize = true;
			label6.Location = new Point(356, 151);
			label6.Name = "label6";
			label6.Size = new Size(32, 13);
			label6.TabIndex = 9;
			label6.Text = "bytes";
			label6.TextAlign = ContentAlignment.MiddleLeft;
			panel1.Anchor = AnchorStyles.Left;
			panel1.AutoSize = true;
			panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel1.Controls.Add(rBtnPreamblePolarity55);
			panel1.Controls.Add(rBtnPreamblePolarityAA);
			panel1.Location = new Point(163, 76);
			panel1.Margin = new Padding(3, 2, 3, 2);
			panel1.Name = "panel1";
			panel1.Size = new Size(110, 20);
			panel1.TabIndex = 4;
			panel1.MouseEnter += control_MouseEnter;
			panel1.MouseLeave += control_MouseLeave;
			rBtnPreamblePolarity55.AutoSize = true;
			rBtnPreamblePolarity55.Location = new Point(59, 1);
			rBtnPreamblePolarity55.Margin = new Padding(3, 0, 3, 0);
			rBtnPreamblePolarity55.Name = "rBtnPreamblePolarity55";
			rBtnPreamblePolarity55.Size = new Size(48, 17);
			rBtnPreamblePolarity55.TabIndex = 1;
			rBtnPreamblePolarity55.Text = "0x55";
			rBtnPreamblePolarity55.UseVisualStyleBackColor = true;
			rBtnPreamblePolarity55.CheckedChanged += rBtnPreamblePolarity_CheckedChanged;
			rBtnPreamblePolarity55.MouseEnter += control_MouseEnter;
			rBtnPreamblePolarity55.MouseLeave += control_MouseLeave;
			rBtnPreamblePolarityAA.AutoSize = true;
			rBtnPreamblePolarityAA.Checked = true;
			rBtnPreamblePolarityAA.Location = new Point(3, 3);
			rBtnPreamblePolarityAA.Margin = new Padding(3, 0, 3, 0);
			rBtnPreamblePolarityAA.Name = "rBtnPreamblePolarityAA";
			rBtnPreamblePolarityAA.Size = new Size(50, 17);
			rBtnPreamblePolarityAA.TabIndex = 0;
			rBtnPreamblePolarityAA.TabStop = true;
			rBtnPreamblePolarityAA.Text = "0xAA";
			rBtnPreamblePolarityAA.UseVisualStyleBackColor = true;
			rBtnPreamblePolarityAA.CheckedChanged += rBtnPreamblePolarity_CheckedChanged;
			rBtnPreamblePolarityAA.MouseEnter += control_MouseEnter;
			rBtnPreamblePolarityAA.MouseLeave += control_MouseLeave;
			label7.Anchor = AnchorStyles.Left;
			label7.AutoSize = true;
			label7.Location = new Point(3, 79);
			label7.Name = "label7";
			label7.Size = new Size(90, 13);
			label7.TabIndex = 3;
			label7.Text = "Preamble polarity:";
			label7.TextAlign = ContentAlignment.MiddleLeft;
			cBoxDataMode.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxDataMode.FormattingEnabled = true;
			cBoxDataMode.Items.AddRange(["Continuous", "Packet"]);
			cBoxDataMode.Location = new Point(163, 2);
			cBoxDataMode.Margin = new Padding(3, 2, 3, 2);
			cBoxDataMode.Name = "cBoxDataMode";
			cBoxDataMode.Size = new Size(121, 21);
			cBoxDataMode.TabIndex = 20;
			cBoxDataMode.SelectedIndexChanged += cBoxDataMode_SelectedIndexChanged;
			label24.Anchor = AnchorStyles.Left;
			label24.AutoSize = true;
			label24.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			label24.Location = new Point(3, 6);
			label24.Name = "label24";
			label24.Size = new Size(62, 13);
			label24.TabIndex = 0;
			label24.Text = "Data mode:";
			label24.TextAlign = ContentAlignment.MiddleLeft;
			label25.Anchor = AnchorStyles.Left;
			label25.AutoSize = true;
			label25.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			label25.Location = new Point(3, 55);
			label25.Name = "label25";
			label25.Size = new Size(109, 13);
			label25.TabIndex = 0;
			label25.Text = "Auto restart Rx mode:";
			label25.TextAlign = ContentAlignment.MiddleLeft;
			cBoxAutoRestartRxMode.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxAutoRestartRxMode.FormattingEnabled = true;
			cBoxAutoRestartRxMode.Items.AddRange(["OFF", "ON, without waiting PLL to re-lock", "ON, wait for PLL to lock"]);
			cBoxAutoRestartRxMode.Location = new Point(163, 51);
			cBoxAutoRestartRxMode.Margin = new Padding(3, 2, 3, 2);
			cBoxAutoRestartRxMode.Name = "cBoxAutoRestartRxMode";
			cBoxAutoRestartRxMode.Size = new Size(187, 21);
			cBoxAutoRestartRxMode.TabIndex = 20;
			cBoxAutoRestartRxMode.SelectedIndexChanged += cBoxAutoRestartRxMode_SelectedIndexChanged;
			pnlNodeAddress.Anchor = AnchorStyles.Left;
			pnlNodeAddress.AutoSize = true;
			pnlNodeAddress.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlNodeAddress.Controls.Add(nudNodeAddress);
			pnlNodeAddress.Controls.Add(lblNodeAddress);
			pnlNodeAddress.Location = new Point(129, 50);
			pnlNodeAddress.Margin = new Padding(3, 2, 3, 2);
			pnlNodeAddress.Name = "pnlNodeAddress";
			pnlNodeAddress.Size = new Size(127, 20);
			pnlNodeAddress.TabIndex = 59;
			pnlNodeAddress.MouseEnter += control_MouseEnter;
			pnlNodeAddress.MouseLeave += control_MouseLeave;
			nudNodeAddress.Location = new Point(0, 0);
			nudNodeAddress.Margin = new Padding(3, 0, 3, 0);
			nudNodeAddress.Maximum = new decimal([255, 0, 0, 0]);
			nudNodeAddress.Name = "nudNodeAddress";
			nudNodeAddress.Size = new Size(59, 20);
			nudNodeAddress.TabIndex = 0;
			nudNodeAddress.MouseEnter += control_MouseEnter;
			nudNodeAddress.MouseLeave += control_MouseLeave;
			nudNodeAddress.ValueChanged += nudNodeAddress_ValueChanged;
			pnlBroadcastAddress.Anchor = AnchorStyles.Left;
			pnlBroadcastAddress.AutoSize = true;
			pnlBroadcastAddress.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlBroadcastAddress.Controls.Add(nudBroadcastAddress);
			pnlBroadcastAddress.Controls.Add(lblBroadcastAddress);
			pnlBroadcastAddress.Location = new Point(129, 74);
			pnlBroadcastAddress.Margin = new Padding(3, 2, 3, 2);
			pnlBroadcastAddress.Name = "pnlBroadcastAddress";
			pnlBroadcastAddress.Size = new Size(127, 20);
			pnlBroadcastAddress.TabIndex = 60;
			pnlBroadcastAddress.MouseEnter += control_MouseEnter;
			pnlBroadcastAddress.MouseLeave += control_MouseLeave;
			nudBroadcastAddress.Location = new Point(0, 0);
			nudBroadcastAddress.Margin = new Padding(3, 0, 3, 0);
			nudBroadcastAddress.Maximum = new decimal([255, 0, 0, 0]);
			nudBroadcastAddress.Name = "nudBroadcastAddress";
			nudBroadcastAddress.Size = new Size(59, 20);
			nudBroadcastAddress.TabIndex = 0;
			nudBroadcastAddress.MouseEnter += control_MouseEnter;
			nudBroadcastAddress.MouseLeave += control_MouseLeave;
			nudBroadcastAddress.ValueChanged += nudBroadcastAddress_ValueChanged;
			tableLayoutPanel2.AutoSize = true;
			tableLayoutPanel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			tableLayoutPanel2.ColumnCount = 3;
			tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
			tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
			tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
			tableLayoutPanel2.Controls.Add(label17, 0, 0);
			tableLayoutPanel2.Controls.Add(pnlBroadcastAddress, 1, 3);
			tableLayoutPanel2.Controls.Add(pnlTxStart, 1, 8);
			tableLayoutPanel2.Controls.Add(label18, 0, 1);
			tableLayoutPanel2.Controls.Add(nudFifoThreshold, 1, 9);
			tableLayoutPanel2.Controls.Add(pnlCrcAutoClear, 1, 6);
			tableLayoutPanel2.Controls.Add(pnlNodeAddress, 1, 2);
			tableLayoutPanel2.Controls.Add(pnlCrcCalculation, 1, 5);
			tableLayoutPanel2.Controls.Add(label19, 0, 2);
			tableLayoutPanel2.Controls.Add(pnlDcFree, 1, 4);
			tableLayoutPanel2.Controls.Add(label20, 0, 3);
			tableLayoutPanel2.Controls.Add(pnlAddressFiltering, 1, 1);
			tableLayoutPanel2.Controls.Add(label21, 0, 4);
			tableLayoutPanel2.Controls.Add(label22, 0, 5);
			tableLayoutPanel2.Controls.Add(label23, 0, 6);
			tableLayoutPanel2.Controls.Add(label26, 0, 8);
			tableLayoutPanel2.Controls.Add(label27, 0, 9);
			tableLayoutPanel2.Controls.Add(pnlAddressInPayload, 1, 0);
			tableLayoutPanel2.Controls.Add(label13, 2, 1);
			tableLayoutPanel2.Controls.Add(panel2, 1, 7);
			tableLayoutPanel2.Controls.Add(panel3, 1, 10);
			tableLayoutPanel2.Controls.Add(panel4, 1, 11);
			tableLayoutPanel2.Controls.Add(panel5, 1, 12);
			tableLayoutPanel2.Controls.Add(label8, 0, 7);
			tableLayoutPanel2.Controls.Add(label14, 0, 10);
			tableLayoutPanel2.Controls.Add(label15, 0, 11);
			tableLayoutPanel2.Controls.Add(label16, 0, 12);
			tableLayoutPanel2.Location = new Point(415, 3);
			tableLayoutPanel2.Name = "tableLayoutPanel2";
			tableLayoutPanel2.RowCount = 13;
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.RowStyles.Add(new RowStyle());
			tableLayoutPanel2.Size = new Size(366, 313);
			tableLayoutPanel2.TabIndex = 1;
			nudFifoThreshold.Anchor = AnchorStyles.Left;
			nudFifoThreshold.Location = new Point(129, 219);
			nudFifoThreshold.Margin = new Padding(3, 2, 3, 2);
			nudFifoThreshold.Maximum = new decimal([128, 0, 0, 0]);
			nudFifoThreshold.Name = "nudFifoThreshold";
			nudFifoThreshold.Size = new Size(59, 20);
			nudFifoThreshold.TabIndex = 19;
			nudFifoThreshold.MouseEnter += control_MouseEnter;
			nudFifoThreshold.MouseLeave += control_MouseLeave;
			nudFifoThreshold.ValueChanged += nudFifoThreshold_ValueChanged;
			label13.Anchor = AnchorStyles.None;
			label13.AutoSize = true;
			label13.Location = new Point(363, 29);
			label13.Name = "label13";
			label13.Size = new Size(0, 13);
			label13.TabIndex = 22;
			label13.TextAlign = ContentAlignment.MiddleLeft;
			panel2.Anchor = AnchorStyles.Left;
			panel2.AutoSize = true;
			panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel2.Controls.Add(rBtnCrcCcitt);
			panel2.Controls.Add(rBtnCrcIbm);
			panel2.Location = new Point(129, 170);
			panel2.Margin = new Padding(3, 2, 3, 2);
			panel2.Name = "panel2";
			panel2.Size = new Size(112, 20);
			panel2.TabIndex = 11;
			panel2.MouseEnter += control_MouseEnter;
			panel2.MouseLeave += control_MouseLeave;
			rBtnCrcCcitt.AutoSize = true;
			rBtnCrcCcitt.Location = new Point(53, 3);
			rBtnCrcCcitt.Margin = new Padding(3, 0, 3, 0);
			rBtnCrcCcitt.Name = "rBtnCrcCcitt";
			rBtnCrcCcitt.Size = new Size(56, 17);
			rBtnCrcCcitt.TabIndex = 1;
			rBtnCrcCcitt.Text = "CCITT";
			rBtnCrcCcitt.UseVisualStyleBackColor = true;
			rBtnCrcCcitt.CheckedChanged += rBtnCrcIbm_CheckedChanged;
			rBtnCrcCcitt.MouseEnter += control_MouseEnter;
			rBtnCrcCcitt.MouseLeave += control_MouseLeave;
			rBtnCrcIbm.AutoSize = true;
			rBtnCrcIbm.Checked = true;
			rBtnCrcIbm.Location = new Point(3, 3);
			rBtnCrcIbm.Margin = new Padding(3, 0, 3, 0);
			rBtnCrcIbm.Name = "rBtnCrcIbm";
			rBtnCrcIbm.Size = new Size(44, 17);
			rBtnCrcIbm.TabIndex = 0;
			rBtnCrcIbm.TabStop = true;
			rBtnCrcIbm.Text = "IBM";
			rBtnCrcIbm.UseVisualStyleBackColor = true;
			rBtnCrcIbm.CheckedChanged += rBtnCrcIbm_CheckedChanged;
			rBtnCrcIbm.MouseEnter += control_MouseEnter;
			rBtnCrcIbm.MouseLeave += control_MouseLeave;
			panel3.Anchor = AnchorStyles.Left;
			panel3.AutoSize = true;
			panel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel3.Controls.Add(rBtnIoHomeOff);
			panel3.Controls.Add(rBtnIoHomeOn);
			panel3.Location = new Point(129, 243);
			panel3.Margin = new Padding(3, 2, 3, 2);
			panel3.Name = "panel3";
			panel3.Size = new Size(102, 20);
			panel3.TabIndex = 11;
			panel3.MouseEnter += control_MouseEnter;
			panel3.MouseLeave += control_MouseLeave;
			rBtnIoHomeOff.AutoSize = true;
			rBtnIoHomeOff.Location = new Point(54, 3);
			rBtnIoHomeOff.Margin = new Padding(3, 0, 3, 0);
			rBtnIoHomeOff.Name = "rBtnIoHomeOff";
			rBtnIoHomeOff.Size = new Size(45, 17);
			rBtnIoHomeOff.TabIndex = 1;
			rBtnIoHomeOff.Text = "OFF";
			rBtnIoHomeOff.UseVisualStyleBackColor = true;
			rBtnIoHomeOff.CheckedChanged += rBtnIoHomeOn_CheckedChanged;
			rBtnIoHomeOff.MouseEnter += control_MouseEnter;
			rBtnIoHomeOff.MouseLeave += control_MouseLeave;
			rBtnIoHomeOn.AutoSize = true;
			rBtnIoHomeOn.Checked = true;
			rBtnIoHomeOn.Location = new Point(3, 3);
			rBtnIoHomeOn.Margin = new Padding(3, 0, 3, 0);
			rBtnIoHomeOn.Name = "rBtnIoHomeOn";
			rBtnIoHomeOn.Size = new Size(41, 17);
			rBtnIoHomeOn.TabIndex = 0;
			rBtnIoHomeOn.TabStop = true;
			rBtnIoHomeOn.Text = "ON";
			rBtnIoHomeOn.UseVisualStyleBackColor = true;
			rBtnIoHomeOn.CheckedChanged += rBtnIoHomeOn_CheckedChanged;
			rBtnIoHomeOn.MouseEnter += control_MouseEnter;
			rBtnIoHomeOn.MouseLeave += control_MouseLeave;
			panel4.Anchor = AnchorStyles.Left;
			panel4.AutoSize = true;
			panel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel4.Controls.Add(rBtnIoHomePwrFrameOff);
			panel4.Controls.Add(rBtnIoHomePwrFrameOn);
			panel4.Location = new Point(129, 267);
			panel4.Margin = new Padding(3, 2, 3, 2);
			panel4.Name = "panel4";
			panel4.Size = new Size(102, 20);
			panel4.TabIndex = 11;
			panel4.MouseEnter += control_MouseEnter;
			panel4.MouseLeave += control_MouseLeave;
			rBtnIoHomePwrFrameOff.AutoSize = true;
			rBtnIoHomePwrFrameOff.Location = new Point(54, 3);
			rBtnIoHomePwrFrameOff.Margin = new Padding(3, 0, 3, 0);
			rBtnIoHomePwrFrameOff.Name = "rBtnIoHomePwrFrameOff";
			rBtnIoHomePwrFrameOff.Size = new Size(45, 17);
			rBtnIoHomePwrFrameOff.TabIndex = 1;
			rBtnIoHomePwrFrameOff.Text = "OFF";
			rBtnIoHomePwrFrameOff.UseVisualStyleBackColor = true;
			rBtnIoHomePwrFrameOff.CheckedChanged += rBtnIoHomePwrFrameOn_CheckedChanged;
			rBtnIoHomePwrFrameOff.MouseEnter += control_MouseEnter;
			rBtnIoHomePwrFrameOff.MouseLeave += control_MouseLeave;
			rBtnIoHomePwrFrameOn.AutoSize = true;
			rBtnIoHomePwrFrameOn.Checked = true;
			rBtnIoHomePwrFrameOn.Location = new Point(3, 3);
			rBtnIoHomePwrFrameOn.Margin = new Padding(3, 0, 3, 0);
			rBtnIoHomePwrFrameOn.Name = "rBtnIoHomePwrFrameOn";
			rBtnIoHomePwrFrameOn.Size = new Size(41, 17);
			rBtnIoHomePwrFrameOn.TabIndex = 0;
			rBtnIoHomePwrFrameOn.TabStop = true;
			rBtnIoHomePwrFrameOn.Text = "ON";
			rBtnIoHomePwrFrameOn.UseVisualStyleBackColor = true;
			rBtnIoHomePwrFrameOn.CheckedChanged += rBtnIoHomePwrFrameOn_CheckedChanged;
			rBtnIoHomePwrFrameOn.MouseEnter += control_MouseEnter;
			rBtnIoHomePwrFrameOn.MouseLeave += control_MouseLeave;
			panel5.Anchor = AnchorStyles.Left;
			panel5.AutoSize = true;
			panel5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel5.Controls.Add(rBtnBeaconOff);
			panel5.Controls.Add(rBtnBeaconOn);
			panel5.Location = new Point(129, 291);
			panel5.Margin = new Padding(3, 2, 3, 2);
			panel5.Name = "panel5";
			panel5.Size = new Size(102, 20);
			panel5.TabIndex = 11;
			panel5.MouseEnter += control_MouseEnter;
			panel5.MouseLeave += control_MouseLeave;
			rBtnBeaconOff.AutoSize = true;
			rBtnBeaconOff.Location = new Point(54, 3);
			rBtnBeaconOff.Margin = new Padding(3, 0, 3, 0);
			rBtnBeaconOff.Name = "rBtnBeaconOff";
			rBtnBeaconOff.Size = new Size(45, 17);
			rBtnBeaconOff.TabIndex = 1;
			rBtnBeaconOff.Text = "OFF";
			rBtnBeaconOff.UseVisualStyleBackColor = true;
			rBtnBeaconOff.CheckedChanged += rBtnBeaconOn_CheckedChanged;
			rBtnBeaconOff.MouseEnter += control_MouseEnter;
			rBtnBeaconOff.MouseLeave += control_MouseLeave;
			rBtnBeaconOn.AutoSize = true;
			rBtnBeaconOn.Checked = true;
			rBtnBeaconOn.Location = new Point(3, 3);
			rBtnBeaconOn.Margin = new Padding(3, 0, 3, 0);
			rBtnBeaconOn.Name = "rBtnBeaconOn";
			rBtnBeaconOn.Size = new Size(41, 17);
			rBtnBeaconOn.TabIndex = 0;
			rBtnBeaconOn.TabStop = true;
			rBtnBeaconOn.Text = "ON";
			rBtnBeaconOn.UseVisualStyleBackColor = true;
			rBtnBeaconOn.CheckedChanged += rBtnBeaconOn_CheckedChanged;
			rBtnBeaconOn.MouseEnter += control_MouseEnter;
			rBtnBeaconOn.MouseLeave += control_MouseLeave;
			label8.Anchor = AnchorStyles.Left;
			label8.AutoSize = true;
			label8.Location = new Point(3, 173);
			label8.Name = "label8";
			label8.Size = new Size(74, 13);
			label8.TabIndex = 10;
			label8.Text = "CRC polynom:";
			label8.TextAlign = ContentAlignment.MiddleLeft;
			label14.Anchor = AnchorStyles.Left;
			label14.AutoSize = true;
			label14.Location = new Point(3, 246);
			label14.Name = "label14";
			label14.Size = new Size(52, 13);
			label14.TabIndex = 10;
			label14.Text = "IO Home:";
			label14.TextAlign = ContentAlignment.MiddleLeft;
			label15.Anchor = AnchorStyles.Left;
			label15.AutoSize = true;
			label15.Location = new Point(3, 270);
			label15.Name = "label15";
			label15.Size = new Size(114, 13);
			label15.TabIndex = 10;
			label15.Text = "IO Home Power frame:";
			label15.TextAlign = ContentAlignment.MiddleLeft;
			label16.Anchor = AnchorStyles.Left;
			label16.AutoSize = true;
			label16.Location = new Point(3, 294);
			label16.Name = "label16";
			label16.Size = new Size(47, 13);
			label16.TabIndex = 10;
			label16.Text = "Beacon:";
			label16.TextAlign = ContentAlignment.MiddleLeft;
			gBoxDeviceStatus.Controls.Add(lblOperatingMode);
			gBoxDeviceStatus.Controls.Add(label37);
			gBoxDeviceStatus.Controls.Add(lblBitSynchroniser);
			gBoxDeviceStatus.Controls.Add(lblDataMode);
			gBoxDeviceStatus.Controls.Add(label38);
			gBoxDeviceStatus.Controls.Add(label39);
			gBoxDeviceStatus.Location = new Point(565, 317);
			gBoxDeviceStatus.Name = "gBoxDeviceStatus";
			gBoxDeviceStatus.Size = new Size(231, 77);
			gBoxDeviceStatus.TabIndex = 3;
			gBoxDeviceStatus.TabStop = false;
			gBoxDeviceStatus.Text = "Device status";
			gBoxDeviceStatus.MouseEnter += control_MouseEnter;
			gBoxDeviceStatus.MouseLeave += control_MouseLeave;
			lblOperatingMode.AutoSize = true;
			lblOperatingMode.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblOperatingMode.Location = new Point(146, 58);
			lblOperatingMode.Margin = new Padding(3);
			lblOperatingMode.Name = "lblOperatingMode";
			lblOperatingMode.Size = new Size(39, 13);
			lblOperatingMode.TabIndex = 5;
			lblOperatingMode.Text = "Sleep";
			lblOperatingMode.TextAlign = ContentAlignment.MiddleLeft;
			label37.AutoSize = true;
			label37.Location = new Point(3, 58);
			label37.Margin = new Padding(3);
			label37.Name = "label37";
			label37.Size = new Size(85, 13);
			label37.TabIndex = 4;
			label37.Text = "Operating mode:";
			label37.TextAlign = ContentAlignment.MiddleLeft;
			lblBitSynchroniser.AutoSize = true;
			lblBitSynchroniser.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblBitSynchroniser.Location = new Point(146, 20);
			lblBitSynchroniser.Margin = new Padding(3);
			lblBitSynchroniser.Name = "lblBitSynchroniser";
			lblBitSynchroniser.Size = new Size(25, 13);
			lblBitSynchroniser.TabIndex = 1;
			lblBitSynchroniser.Text = "ON";
			lblBitSynchroniser.TextAlign = ContentAlignment.MiddleLeft;
			lblDataMode.AutoSize = true;
			lblDataMode.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblDataMode.Location = new Point(146, 39);
			lblDataMode.Margin = new Padding(3);
			lblDataMode.Name = "lblDataMode";
			lblDataMode.Size = new Size(47, 13);
			lblDataMode.TabIndex = 3;
			lblDataMode.Text = "Packet";
			lblDataMode.TextAlign = ContentAlignment.MiddleLeft;
			label38.AutoSize = true;
			label38.Location = new Point(3, 20);
			label38.Margin = new Padding(3);
			label38.Name = "label38";
			label38.Size = new Size(86, 13);
			label38.TabIndex = 0;
			label38.Text = "Bit Synchronizer:";
			label38.TextAlign = ContentAlignment.MiddleLeft;
			label39.AutoSize = true;
			label39.Location = new Point(3, 39);
			label39.Margin = new Padding(3);
			label39.Name = "label39";
			label39.Size = new Size(62, 13);
			label39.TabIndex = 2;
			label39.Text = "Data mode:";
			label39.TextAlign = ContentAlignment.MiddleLeft;
			gBoxControl.Controls.Add(btnFillFifo);
			gBoxControl.Controls.Add(tBoxPacketsNb);
			gBoxControl.Controls.Add(cBtnLog);
			gBoxControl.Controls.Add(cBtnPacketHandlerStartStop);
			gBoxControl.Controls.Add(lblPacketsNb);
			gBoxControl.Controls.Add(tBoxPacketsRepeatValue);
			gBoxControl.Controls.Add(lblPacketsRepeatValue);
			gBoxControl.Location = new Point(565, 394);
			gBoxControl.Name = "gBoxControl";
			gBoxControl.Size = new Size(231, 96);
			gBoxControl.TabIndex = 4;
			gBoxControl.TabStop = false;
			gBoxControl.Text = "Control";
			gBoxControl.MouseEnter += control_MouseEnter;
			gBoxControl.MouseLeave += control_MouseLeave;
			btnFillFifo.Location = new Point(168, 19);
			btnFillFifo.Name = "btnFillFifo";
			btnFillFifo.Size = new Size(57, 23);
			btnFillFifo.TabIndex = 5;
			btnFillFifo.Text = "Fill FIFO";
			btnFillFifo.UseVisualStyleBackColor = true;
			btnFillFifo.Click += btnFillFifo_Click;
			tBoxPacketsNb.Location = new Point(149, 48);
			tBoxPacketsNb.Name = "tBoxPacketsNb";
			tBoxPacketsNb.ReadOnly = true;
			tBoxPacketsNb.Size = new Size(79, 20);
			tBoxPacketsNb.TabIndex = 2;
			tBoxPacketsNb.Text = "0";
			tBoxPacketsNb.TextAlign = HorizontalAlignment.Right;
			cBtnLog.Appearance = Appearance.Button;
			cBtnLog.Location = new Point(87, 19);
			cBtnLog.Name = "cBtnLog";
			cBtnLog.Size = new Size(75, 23);
			cBtnLog.TabIndex = 0;
			cBtnLog.Text = "Log";
			cBtnLog.TextAlign = ContentAlignment.MiddleCenter;
			cBtnLog.UseVisualStyleBackColor = true;
			cBtnLog.CheckedChanged += cBtnLog_CheckedChanged;
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
			lblPacketsNb.TabIndex = 1;
			lblPacketsNb.Text = "Tx Packets:";
			lblPacketsNb.TextAlign = ContentAlignment.MiddleLeft;
			tBoxPacketsRepeatValue.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			tBoxPacketsRepeatValue.Location = new Point(149, 70);
			tBoxPacketsRepeatValue.Name = "tBoxPacketsRepeatValue";
			tBoxPacketsRepeatValue.Size = new Size(79, 20);
			tBoxPacketsRepeatValue.TabIndex = 4;
			tBoxPacketsRepeatValue.Text = "0";
			tBoxPacketsRepeatValue.TextAlign = HorizontalAlignment.Right;
			lblPacketsRepeatValue.AutoSize = true;
			lblPacketsRepeatValue.Location = new Point(3, 73);
			lblPacketsRepeatValue.Name = "lblPacketsRepeatValue";
			lblPacketsRepeatValue.Size = new Size(74, 13);
			lblPacketsRepeatValue.TabIndex = 3;
			lblPacketsRepeatValue.Text = "Repeat value:";
			lblPacketsRepeatValue.TextAlign = ContentAlignment.MiddleLeft;
			gBoxPacket.Controls.Add(imgPacketMessage);
			gBoxPacket.Controls.Add(gBoxMessage);
			gBoxPacket.Controls.Add(tblPacket);
			gBoxPacket.Location = new Point(3, 317);
			gBoxPacket.Margin = new Padding(3, 1, 3, 1);
			gBoxPacket.Name = "gBoxPacket";
			gBoxPacket.Size = new Size(557, 172);
			gBoxPacket.TabIndex = 2;
			gBoxPacket.TabStop = false;
			gBoxPacket.Text = "Packet";
			gBoxPacket.MouseEnter += control_MouseEnter;
			gBoxPacket.MouseLeave += control_MouseLeave;
			imgPacketMessage.BackColor = Color.Transparent;
			imgPacketMessage.Location = new Point(5, 61);
			imgPacketMessage.Margin = new Padding(0);
			imgPacketMessage.Name = "imgPacketMessage";
			imgPacketMessage.Size = new Size(547, 5);
			imgPacketMessage.TabIndex = 1;
			imgPacketMessage.Text = "payloadImg1";
			gBoxMessage.Controls.Add(tblPayloadMessage);
			gBoxMessage.Location = new Point(6, 67);
			gBoxMessage.Margin = new Padding(1);
			gBoxMessage.Name = "gBoxMessage";
			gBoxMessage.Size = new Size(547, 101);
			gBoxMessage.TabIndex = 2;
			gBoxMessage.TabStop = false;
			gBoxMessage.Text = "Message";
			gBoxMessage.MouseEnter += control_MouseEnter;
			gBoxMessage.MouseLeave += control_MouseLeave;
			tblPayloadMessage.AutoSize = true;
			tblPayloadMessage.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			tblPayloadMessage.ColumnCount = 2;
			tblPayloadMessage.ColumnStyles.Add(new ColumnStyle());
			tblPayloadMessage.ColumnStyles.Add(new ColumnStyle());
			tblPayloadMessage.Controls.Add(hexBoxPayload, 0, 1);
			tblPayloadMessage.Controls.Add(label36, 1, 0);
			tblPayloadMessage.Controls.Add(label35, 0, 0);
			tblPayloadMessage.Location = new Point(20, 19);
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
			label36.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label36.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label36.Location = new Point(329, 0);
			label36.Name = "label36";
			label36.Size = new Size(175, 13);
			label36.TabIndex = 1;
			label36.Text = "ASCII";
			label36.TextAlign = ContentAlignment.MiddleCenter;
			label35.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label35.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label35.Location = new Point(3, 0);
			label35.Name = "label35";
			label35.Size = new Size(320, 13);
			label35.TabIndex = 0;
			label35.Text = "HEXADECIMAL";
			label35.TextAlign = ContentAlignment.MiddleCenter;
			tblPacket.AutoSize = true;
			tblPacket.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			tblPacket.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			tblPacket.ColumnCount = 6;
			tblPacket.ColumnStyles.Add(new ColumnStyle());
			tblPacket.ColumnStyles.Add(new ColumnStyle());
			tblPacket.ColumnStyles.Add(new ColumnStyle());
			tblPacket.ColumnStyles.Add(new ColumnStyle());
			tblPacket.ColumnStyles.Add(new ColumnStyle());
			tblPacket.ColumnStyles.Add(new ColumnStyle());
			tblPacket.Controls.Add(label29, 0, 0);
			tblPacket.Controls.Add(label30, 1, 0);
			tblPacket.Controls.Add(label31, 2, 0);
			tblPacket.Controls.Add(label32, 3, 0);
			tblPacket.Controls.Add(label33, 4, 0);
			tblPacket.Controls.Add(label34, 5, 0);
			tblPacket.Controls.Add(lblPacketPreamble, 0, 1);
			tblPacket.Controls.Add(lblPayload, 4, 1);
			tblPacket.Controls.Add(pnlPacketCrc, 5, 1);
			tblPacket.Controls.Add(pnlPacketAddr, 3, 1);
			tblPacket.Controls.Add(lblPacketLength, 2, 1);
			tblPacket.Controls.Add(lblPacketSyncValue, 1, 1);
			tblPacket.Location = new Point(5, 17);
			tblPacket.Margin = new Padding(1);
			tblPacket.Name = "tblPacket";
			tblPacket.RowCount = 2;
			tblPacket.RowStyles.Add(new RowStyle());
			tblPacket.RowStyles.Add(new RowStyle());
			tblPacket.Size = new Size(547, 43);
			tblPacket.TabIndex = 0;
			label29.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label29.Location = new Point(1, 1);
			label29.Margin = new Padding(0);
			label29.Name = "label29";
			label29.Size = new Size(103, 20);
			label29.TabIndex = 0;
			label29.Text = "Preamble";
			label29.TextAlign = ContentAlignment.MiddleCenter;
			label30.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label30.Location = new Point(108, 1);
			label30.Margin = new Padding(0);
			label30.Name = "label30";
			label30.Size = new Size(152, 20);
			label30.TabIndex = 1;
			label30.Text = "Sync";
			label30.TextAlign = ContentAlignment.MiddleCenter;
			label31.BackColor = Color.LightGray;
			label31.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label31.Location = new Point(261, 1);
			label31.Margin = new Padding(0);
			label31.Name = "label31";
			label31.Size = new Size(59, 20);
			label31.TabIndex = 2;
			label31.Text = "Length";
			label31.TextAlign = ContentAlignment.MiddleCenter;
			label32.BackColor = Color.LightGray;
			label32.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label32.Location = new Point(321, 1);
			label32.Margin = new Padding(0);
			label32.Name = "label32";
			label32.Size = new Size(87, 20);
			label32.TabIndex = 3;
			label32.Text = "Node Address";
			label32.TextAlign = ContentAlignment.MiddleCenter;
			label33.BackColor = Color.LightGray;
			label33.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label33.ForeColor = SystemColors.WindowText;
			label33.Location = new Point(409, 1);
			label33.Margin = new Padding(0);
			label33.Name = "label33";
			label33.Size = new Size(85, 20);
			label33.TabIndex = 4;
			label33.Text = "Message";
			label33.TextAlign = ContentAlignment.MiddleCenter;
			label34.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label34.Location = new Point(495, 1);
			label34.Margin = new Padding(0);
			label34.Name = "label34";
			label34.Size = new Size(51, 20);
			label34.TabIndex = 5;
			label34.Text = "CRC";
			label34.TextAlign = ContentAlignment.MiddleCenter;
			lblPacketPreamble.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblPacketPreamble.Location = new Point(1, 22);
			lblPacketPreamble.Margin = new Padding(0);
			lblPacketPreamble.Name = "lblPacketPreamble";
			lblPacketPreamble.Size = new Size(106, 20);
			lblPacketPreamble.TabIndex = 6;
			lblPacketPreamble.Text = "55-55-55-55-...-55";
			lblPacketPreamble.TextAlign = ContentAlignment.MiddleCenter;
			lblPayload.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblPayload.Location = new Point(409, 22);
			lblPayload.Margin = new Padding(0);
			lblPayload.Name = "lblPayload";
			lblPayload.Size = new Size(85, 20);
			lblPayload.TabIndex = 9;
			lblPayload.TextAlign = ContentAlignment.MiddleCenter;
			pnlPacketCrc.Controls.Add(ledPacketCrc);
			pnlPacketCrc.Controls.Add(lblPacketCrc);
			pnlPacketCrc.Location = new Point(495, 22);
			pnlPacketCrc.Margin = new Padding(0);
			pnlPacketCrc.Name = "pnlPacketCrc";
			pnlPacketCrc.Size = new Size(51, 20);
			pnlPacketCrc.TabIndex = 18;
			ledPacketCrc.BackColor = Color.Transparent;
			ledPacketCrc.LedColor = Color.Green;
			ledPacketCrc.LedSize = new Size(11, 11);
			ledPacketCrc.Location = new Point(17, 3);
			ledPacketCrc.Name = "ledPacketCrc";
			ledPacketCrc.Size = new Size(15, 15);
			ledPacketCrc.TabIndex = 1;
			ledPacketCrc.Text = "CRC";
			ledPacketCrc.Visible = false;
			lblPacketCrc.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblPacketCrc.Location = new Point(0, 0);
			lblPacketCrc.Margin = new Padding(0);
			lblPacketCrc.Name = "lblPacketCrc";
			lblPacketCrc.Size = new Size(51, 20);
			lblPacketCrc.TabIndex = 0;
			lblPacketCrc.Text = "XX-XX";
			lblPacketCrc.TextAlign = ContentAlignment.MiddleCenter;
			pnlPacketAddr.Controls.Add(lblPacketAddr);
			pnlPacketAddr.Location = new Point(321, 22);
			pnlPacketAddr.Margin = new Padding(0);
			pnlPacketAddr.Name = "pnlPacketAddr";
			pnlPacketAddr.Size = new Size(87, 20);
			pnlPacketAddr.TabIndex = 11;
			lblPacketAddr.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblPacketAddr.Location = new Point(0, 0);
			lblPacketAddr.Margin = new Padding(0);
			lblPacketAddr.Name = "lblPacketAddr";
			lblPacketAddr.Size = new Size(87, 20);
			lblPacketAddr.TabIndex = 0;
			lblPacketAddr.Text = "00";
			lblPacketAddr.TextAlign = ContentAlignment.MiddleCenter;
			lblPacketLength.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblPacketLength.Location = new Point(261, 22);
			lblPacketLength.Margin = new Padding(0);
			lblPacketLength.Name = "lblPacketLength";
			lblPacketLength.Size = new Size(59, 20);
			lblPacketLength.TabIndex = 8;
			lblPacketLength.Text = "00";
			lblPacketLength.TextAlign = ContentAlignment.MiddleCenter;
			lblPacketSyncValue.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblPacketSyncValue.Location = new Point(108, 22);
			lblPacketSyncValue.Margin = new Padding(0);
			lblPacketSyncValue.Name = "lblPacketSyncValue";
			lblPacketSyncValue.Size = new Size(152, 20);
			lblPacketSyncValue.TabIndex = 7;
			lblPacketSyncValue.Text = "AA-AA-AA-AA-AA-AA-AA-AA";
			lblPacketSyncValue.TextAlign = ContentAlignment.MiddleCenter;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(gBoxDeviceStatus);
			Controls.Add(tableLayoutPanel2);
			Controls.Add(tableLayoutPanel1);
			Controls.Add(gBoxControl);
			Controls.Add(gBoxPacket);
			Name = "PacketHandlerView";
			Size = new Size(799, 493);
			((ISupportInitialize)errorProvider).EndInit();
			((ISupportInitialize)nudPreambleSize).EndInit();
			pnlDcFree.ResumeLayout(false);
			pnlDcFree.PerformLayout();
			pnlAddressInPayload.ResumeLayout(false);
			pnlAddressInPayload.PerformLayout();
			pnlFifoFillCondition.ResumeLayout(false);
			pnlFifoFillCondition.PerformLayout();
			pnlSync.ResumeLayout(false);
			pnlSync.PerformLayout();
			pnlCrcAutoClear.ResumeLayout(false);
			pnlCrcAutoClear.PerformLayout();
			pnlCrcCalculation.ResumeLayout(false);
			pnlCrcCalculation.PerformLayout();
			pnlTxStart.ResumeLayout(false);
			pnlTxStart.PerformLayout();
			pnlAddressFiltering.ResumeLayout(false);
			pnlAddressFiltering.PerformLayout();
			pnlPacketFormat.ResumeLayout(false);
			pnlPacketFormat.PerformLayout();
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			pnlPayloadLength.ResumeLayout(false);
			((ISupportInitialize)nudPayloadLength).EndInit();
			((ISupportInitialize)nudSyncSize).EndInit();
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			pnlNodeAddress.ResumeLayout(false);
			((ISupportInitialize)nudNodeAddress).EndInit();
			pnlBroadcastAddress.ResumeLayout(false);
			((ISupportInitialize)nudBroadcastAddress).EndInit();
			tableLayoutPanel2.ResumeLayout(false);
			tableLayoutPanel2.PerformLayout();
			((ISupportInitialize)nudFifoThreshold).EndInit();
			panel2.ResumeLayout(false);
			panel2.PerformLayout();
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			panel4.ResumeLayout(false);
			panel4.PerformLayout();
			panel5.ResumeLayout(false);
			panel5.PerformLayout();
			gBoxDeviceStatus.ResumeLayout(false);
			gBoxDeviceStatus.PerformLayout();
			gBoxControl.ResumeLayout(false);
			gBoxControl.PerformLayout();
			gBoxPacket.ResumeLayout(false);
			gBoxPacket.PerformLayout();
			gBoxMessage.ResumeLayout(false);
			gBoxMessage.PerformLayout();
			tblPayloadMessage.ResumeLayout(false);
			tblPacket.ResumeLayout(false);
			pnlPacketCrc.ResumeLayout(false);
			pnlPacketAddr.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		public PacketHandlerView()
		{
			InitializeComponent();
			tBoxSyncValue.TextChanged -= tBoxSyncValue_TextChanged;
			tBoxSyncValue.MaskInputRejected -= tBoxSyncValue_MaskInputRejected;
			tBoxSyncValue.ValidatingType = typeof(MaskValidationType);
			tBoxSyncValue.Mask = "&&-&&-&&-&&";
			tBoxSyncValue.TextChanged += tBoxSyncValue_TextChanged;
			tBoxSyncValue.MaskInputRejected += tBoxSyncValue_MaskInputRejected;
			message = [];
			var data = new byte[Message.Length];
			hexBoxPayload.ByteProvider = new DynamicByteProvider(data);
			hexBoxPayload.ByteProvider.Changed += hexBoxPayload_DataChanged;
			hexBoxPayload.ByteProvider.ApplyChanges();
		}

		private void UpdateControls()
		{
			if (DataMode == DataModeEnum.Packet)
			{
				lblDataMode.Text = "Packet";
				if (mode == OperatingModeEnum.Tx || mode == OperatingModeEnum.Rx)
				{
					cBtnPacketHandlerStartStop.Enabled = true;
					tBoxPacketsNb.Enabled = true;
					if (!cBtnPacketHandlerStartStop.Checked)
					{
						tBoxPacketsRepeatValue.Enabled = true;
					}
				}
			}
			else
			{
				lblDataMode.Text = "Continuous";
				cBtnPacketHandlerStartStop.Enabled = false;
				tBoxPacketsNb.Enabled = false;
				tBoxPacketsRepeatValue.Enabled = false;
			}
			switch (Mode)
			{
			case OperatingModeEnum.Sleep:
				lblOperatingMode.Text = "Sleep";
				nudPayloadLength.Enabled = true;
				rBtnNodeAddressInPayloadYes.Enabled = false;
				rBtnNodeAddressInPayloadNo.Enabled = false;
				lblPacketsNb.Visible = false;
				tBoxPacketsNb.Visible = false;
				lblPacketsRepeatValue.Visible = false;
				tBoxPacketsRepeatValue.Visible = false;
				btnFillFifo.Enabled = true;
				break;
			case OperatingModeEnum.Stdby:
				lblOperatingMode.Text = "Standby";
				nudPayloadLength.Enabled = true;
				rBtnNodeAddressInPayloadYes.Enabled = false;
				rBtnNodeAddressInPayloadNo.Enabled = false;
				lblPacketsNb.Visible = false;
				tBoxPacketsNb.Visible = false;
				lblPacketsRepeatValue.Visible = false;
				tBoxPacketsRepeatValue.Visible = false;
				btnFillFifo.Enabled = true;
				break;
			case OperatingModeEnum.FsRx:
				lblOperatingMode.Text = "Synthesize Rx";
				nudPayloadLength.Enabled = true;
				rBtnNodeAddressInPayloadYes.Enabled = false;
				rBtnNodeAddressInPayloadNo.Enabled = false;
				lblPacketsNb.Visible = false;
				tBoxPacketsNb.Visible = false;
				lblPacketsRepeatValue.Visible = false;
				tBoxPacketsRepeatValue.Visible = false;
				btnFillFifo.Enabled = true;
				break;
			case OperatingModeEnum.FsTx:
				lblOperatingMode.Text = "Synthesize Tx";
				nudPayloadLength.Enabled = false;
				rBtnNodeAddressInPayloadYes.Enabled = false;
				rBtnNodeAddressInPayloadNo.Enabled = false;
				lblPacketsNb.Visible = false;
				tBoxPacketsNb.Visible = false;
				lblPacketsRepeatValue.Visible = false;
				tBoxPacketsRepeatValue.Visible = false;
				btnFillFifo.Enabled = true;
				break;
			case OperatingModeEnum.Rx:
				lblOperatingMode.Text = "Receiver";
				nudPayloadLength.Enabled = true;
				rBtnNodeAddressInPayloadYes.Enabled = false;
				rBtnNodeAddressInPayloadNo.Enabled = false;
				lblPacketsNb.Text = "Rx packets:";
				lblPacketsNb.Visible = true;
				tBoxPacketsNb.Visible = true;
				lblPacketsRepeatValue.Visible = false;
				tBoxPacketsRepeatValue.Visible = false;
				btnFillFifo.Enabled = false;
				break;
			case OperatingModeEnum.Tx:
				lblOperatingMode.Text = "Transmitter";
				nudPayloadLength.Enabled = false;
				rBtnNodeAddressInPayloadYes.Enabled = true;
				rBtnNodeAddressInPayloadNo.Enabled = true;
				lblPacketsNb.Text = "Tx Packets:";
				lblPacketsNb.Visible = true;
				tBoxPacketsNb.Visible = true;
				lblPacketsRepeatValue.Visible = true;
				tBoxPacketsRepeatValue.Visible = true;
				btnFillFifo.Enabled = false;
				break;
			}
		}

		private void OnError(byte status, string messageN)
		{
            Error?.Invoke(this, new ErrorEventArgs(status, messageN));
        }

		private void OnDataModeChanged(DataModeEnum value)
		{
            DataModeChanged?.Invoke(this, new DataModeEventArg(value));
        }

		private void OnPreambleSizeChanged(int value)
		{
            PreambleSizeChanged?.Invoke(this, new Int32EventArg(value));
        }

		private void OnAutoRestartRxChanged(AutoRestartRxEnum value)
		{
            AutoRestartRxChanged?.Invoke(this, new AutoRestartRxEventArg(value));
        }

		private void OnPreamblePolarityChanged(PreamblePolarityEnum value)
		{
            PreamblePolarityChanged?.Invoke(this, new PreamblePolarityEventArg(value));
        }

		private void OnFifoFillConditionChanged(FifoFillConditionEnum value)
		{
            FifoFillConditionChanged?.Invoke(this, new FifoFillConditionEventArg(value));
        }

		private void OnSyncOnChanged(bool value)
		{
            SyncOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnSyncSizeChanged(byte value)
		{
            SyncSizeChanged?.Invoke(this, new ByteEventArg(value));
        }

		private void OnSyncValueChanged(byte[] value)
		{
            SyncValueChanged?.Invoke(this, new ByteArrayEventArg(value));
        }

		private void OnPacketFormatChanged(PacketFormatEnum value)
		{
            PacketFormatChanged?.Invoke(this, new PacketFormatEventArg(value));
        }

		private void OnDcFreeChanged(DcFreeEnum value)
		{
            DcFreeChanged?.Invoke(this, new DcFreeEventArg(value));
        }

		private void OnCrcOnChanged(bool value)
		{
            CrcOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnCrcAutoClearOffChanged(bool value)
		{
            CrcAutoClearOffChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnAddressFilteringChanged(AddressFilteringEnum value)
		{
            AddressFilteringChanged?.Invoke(this, new AddressFilteringEventArg(value));
        }

		private void OnPayloadLengthChanged(short value)
		{
            PayloadLengthChanged?.Invoke(this, new Int16EventArg(value));
        }

		private void OnNodeAddressChanged(byte value)
		{
            NodeAddressChanged?.Invoke(this, new ByteEventArg(value));
        }

		private void OnBroadcastAddressChanged(byte value)
		{
            BroadcastAddressChanged?.Invoke(this, new ByteEventArg(value));
        }

		private void OnTxStartConditionChanged(bool value)
		{
            TxStartConditionChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnFifoThresholdChanged(byte value)
		{
            FifoThresholdChanged?.Invoke(this, new ByteEventArg(value));
        }

		private void OnMessageLengthChanged(int value)
		{
            MessageLengthChanged?.Invoke(this, new Int32EventArg(value));
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

		private void OnPacketHandlerLogEnableChanged(bool value)
		{
            PacketHandlerLogEnableChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnCrcIbmChanged(bool value)
		{
            CrcIbmChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnIoHomeOnChanged(bool value)
		{
            IoHomeOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnIoHomePwrFrameOnChanged(bool value)
		{
            IoHomePwrFrameOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnBeaconOnChanged(bool value)
		{
            BeaconOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnFillFifoChanged()
		{
            FillFifoChanged?.Invoke(this, EventArgs.Empty);
        }

		public void UpdateSyncValueLimits(LimitCheckStatusEnum status, string messageN)
		{
			switch (status)
			{
			case LimitCheckStatusEnum.OK:
				tBoxSyncValue.BackColor = SystemColors.Window;
				break;
			case LimitCheckStatusEnum.OUT_OF_RANGE:
				tBoxSyncValue.BackColor = ControlPaint.LightLight(Color.Orange);
				break;
			case LimitCheckStatusEnum.ERROR:
				tBoxSyncValue.BackColor = ControlPaint.LightLight(Color.Red);
				break;
			}
			errorProvider.SetError(tBoxSyncValue, messageN);
		}

		private void cBoxDataMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			DataMode = (DataModeEnum)cBoxDataMode.SelectedIndex;
			OnDataModeChanged(DataMode);
		}

		private void nudPreambleSize_ValueChanged(object sender, EventArgs e)
		{
			PreambleSize = (int)nudPreambleSize.Value;
			OnPreambleSizeChanged(PreambleSize);
		}

		private void cBoxAutoRestartRxMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnAutoRestartRxChanged(AutoRestartRxOn);
		}

		private void rBtnPreamblePolarity_CheckedChanged(object sender, EventArgs e)
		{
			OnPreamblePolarityChanged(PreamblePolarity);
		}

		private void rBtnSyncOn_CheckedChanged(object sender, EventArgs e)
		{
			SyncOn = rBtnSyncOn.Checked;
			OnSyncOnChanged(SyncOn);
		}

		private void rBtnFifoFill_CheckedChanged(object sender, EventArgs e)
		{
			FifoFillCondition = ((!rBtnFifoFillSyncAddress.Checked) ? FifoFillConditionEnum.Allways : FifoFillConditionEnum.OnSyncAddressIrq);
			OnFifoFillConditionChanged(FifoFillCondition);
		}

		private void nudSyncSize_ValueChanged(object sender, EventArgs e)
		{
			SyncSize = (byte)nudSyncSize.Value;
			OnSyncSizeChanged(SyncSize);
		}

		private void tBoxSyncValue_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			OnError(1, "Input rejected at position: " + e.Position.ToString(CultureInfo.CurrentCulture));
		}

		private void tBoxSyncValue_TypeValidationCompleted(object sender, TypeValidationEventArgs e)
		{
			try
			{
				if (e.IsValidInput)
				{
					if (e.ReturnValue is MaskValidationType type)
					{
						SyncValue = type.ArrayValue;
					}
					return;
				}
				SyncValue = MaskValidationType.InvalidMask.ArrayValue;
				throw new Exception("Wrong Sync word entered.  Message: " + e.Message);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		private void tBoxSyncValue_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Shift || e.Control || Uri.IsHexDigit((char)e.KeyData) || e.KeyData is >= Keys.NumPad0 and <= Keys.NumPad9 || e.KeyData == Keys.Back || e.KeyData == Keys.Delete || e.KeyData == Keys.Left || e.KeyData == Keys.Right)
			{
				OnError(0, "-");
				return;
			}
			e.Handled = true;
			e.SuppressKeyPress = true;
		}

		private void tBoxSyncValue_TextChanged(object sender, EventArgs e)
		{
			OnError(0, "-");
			syncWord.StringValue = tBoxSyncValue.Text;
			syncValue = syncWord.ArrayValue;
			lblPacketSyncValue.Text = syncWord.StringValue;
		}

		private void rBtnPacketFormat_CheckedChanged(object sender, EventArgs e)
		{
			PacketFormat = rBtnPacketFormatVariable.Checked ? PacketFormatEnum.Variable : PacketFormatEnum.Fixed;
			OnPacketFormatChanged(PacketFormat);
		}

		private void nudPayloadLength_ValueChanged(object sender, EventArgs e)
		{
			PayloadLength = (byte)nudPayloadLength.Value;
			OnPayloadLengthChanged(PayloadLength);
		}

		private void rBtnAddressFilteringOff_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnAddressFilteringOff.Checked)
			{
				AddressFiltering = AddressFilteringEnum.OFF;
				OnAddressFilteringChanged(AddressFiltering);
			}
		}

		private void rBtnAddressFilteringNode_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnAddressFilteringNode.Checked)
			{
				AddressFiltering = AddressFilteringEnum.Node;
				OnAddressFilteringChanged(AddressFiltering);
			}
		}

		private void rBtnAddressFilteringNodeBroadcast_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnAddressFilteringNodeBroadcast.Checked)
			{
				AddressFiltering = AddressFilteringEnum.NodeBroadcast;
				OnAddressFilteringChanged(AddressFiltering);
			}
		}

		private void nudNodeAddress_ValueChanged(object sender, EventArgs e)
		{
			NodeAddress = (byte)nudNodeAddress.Value;
			OnNodeAddressChanged(NodeAddress);
		}

		private void nudBroadcastAddress_ValueChanged(object sender, EventArgs e)
		{
			BroadcastAddress = (byte)nudBroadcastAddress.Value;
			OnBroadcastAddressChanged(BroadcastAddress);
		}

		private void rBtnDcFreeOff_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnDcFreeOff.Checked)
			{
				DcFree = DcFreeEnum.OFF;
				OnDcFreeChanged(DcFree);
			}
		}

		private void rBtnDcFreeManchester_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnDcFreeManchester.Checked)
			{
				DcFree = DcFreeEnum.Manchester;
				OnDcFreeChanged(DcFree);
			}
		}

		private void rBtnDcFreeWhitening_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnDcFreeWhitening.Checked)
			{
				DcFree = DcFreeEnum.Whitening;
				OnDcFreeChanged(DcFree);
			}
		}

		private void rBtnCrcOn_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnCrcOn.Checked)
			{
				CrcOn = rBtnCrcOn.Checked;
				OnCrcOnChanged(CrcOn);
			}
		}

		private void rBtnCrcOff_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnCrcOff.Checked)
			{
				CrcOn = rBtnCrcOn.Checked;
				OnCrcOnChanged(CrcOn);
			}
		}

		private void rBtnCrcAutoClearOn_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnCrcAutoClearOn.Checked)
			{
				CrcAutoClearOff = rBtnCrcAutoClearOff.Checked;
				OnCrcAutoClearOffChanged(CrcAutoClearOff);
			}
		}

		private void rBtnCrcAutoClearOff_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnCrcAutoClearOff.Checked)
			{
				CrcAutoClearOff = rBtnCrcAutoClearOff.Checked;
				OnCrcAutoClearOffChanged(CrcAutoClearOff);
			}
		}

		private void rBtnCrcIbm_CheckedChanged(object sender, EventArgs e)
		{
			OnCrcIbmChanged(CrcIbmOn);
		}

		private void tBox_Validated(object sender, EventArgs e)
		{
			if (sender == tBoxSyncValue)
			{
				tBoxSyncValue.Text = tBoxSyncValue.Text.ToUpper();
				lblPacketSyncValue.Text = tBoxSyncValue.Text;
				OnSyncValueChanged(SyncValue);
				return;
			}
			var textBox = (TextBox)sender;
			if (textBox.Text != "" && !textBox.Text.StartsWith("0x", ignoreCase: true, null))
			{
				textBox.Text = "0x" + Convert.ToByte(textBox.Text, 16).ToString("X02");
			}
		}

		private void rBtnTxStartFifoLevel_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnTxStartFifoLevel.Checked)
			{
				TxStartCondition = !rBtnTxStartFifoLevel.Checked;
				OnTxStartConditionChanged(TxStartCondition);
			}
		}

		private void rBtnTxStartFifoNotEmpty_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnTxStartFifoNotEmpty.Checked)
			{
				TxStartCondition = !rBtnTxStartFifoLevel.Checked;
				OnTxStartConditionChanged(TxStartCondition);
			}
		}

		private void nudFifoThreshold_ValueChanged(object sender, EventArgs e)
		{
			FifoThreshold = (byte)nudFifoThreshold.Value;
			OnFifoThresholdChanged(FifoThreshold);
		}

		private void rBtnIoHomeOn_CheckedChanged(object sender, EventArgs e)
		{
			OnIoHomeOnChanged(IoHomeOn);
		}

		private void rBtnIoHomePwrFrameOn_CheckedChanged(object sender, EventArgs e)
		{
			OnIoHomePwrFrameOnChanged(IoHomePwrFrameOn);
		}

		private void rBtnBeaconOn_CheckedChanged(object sender, EventArgs e)
		{
			OnBeaconOnChanged(BeaconOn);
		}

		private void hexBoxPayload_DataChanged(object sender, EventArgs e)
		{
			if (!inHexPayloadDataChanged)
			{
				inHexPayloadDataChanged = true;
				if (hexBoxPayload.ByteProvider.Length > 64)
				{
					hexBoxPayload.ByteProvider.DeleteBytes(64L, hexBoxPayload.ByteProvider.Length - 64);
					hexBoxPayload.SelectionStart = 64L;
					hexBoxPayload.SelectionLength = 0L;
				}
				Array.Resize(ref message, (int)hexBoxPayload.ByteProvider.Length);
				for (var i = 0; i < message.Length; i++)
				{
					message[i] = hexBoxPayload.ByteProvider.ReadByte(i);
				}
				OnMessageChanged(Message);
				inHexPayloadDataChanged = false;
			}
		}

		private void cBtnPacketHandlerStartStop_CheckedChanged(object sender, EventArgs e)
		{
			cBtnPacketHandlerStartStop.Text = cBtnPacketHandlerStartStop.Checked ? "Stop" : "Start";
			tableLayoutPanel1.Enabled = !cBtnPacketHandlerStartStop.Checked;
			tableLayoutPanel2.Enabled = !cBtnPacketHandlerStartStop.Checked;
			gBoxPacket.Enabled = !cBtnPacketHandlerStartStop.Checked;
			tBoxPacketsRepeatValue.Enabled = !cBtnPacketHandlerStartStop.Checked;
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

		private void cBtnLog_CheckedChanged(object sender, EventArgs e)
		{
			OnPacketHandlerLogEnableChanged(cBtnLog.Checked);
		}

		private void btnFillFifo_Click(object sender, EventArgs e)
		{
			OnFillFifoChanged();
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
			if (sender == nudPreambleSize)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Preamble size"));
			}
			else if (sender == pnlSync || sender == rBtnSyncOn || sender == rBtnSyncOff)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Sync word"));
			}
			else if (sender == pnlFifoFillCondition || sender == rBtnFifoFillSyncAddress || sender == rBtnFifoFillAlways)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Fifo fill condition"));
			}
			else if (sender == nudSyncSize)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Sync word size"));
			}
			else if (sender == tBoxSyncValue)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Sync word value"));
			}
			else if (sender == pnlPacketFormat || sender == rBtnPacketFormatFixed || sender == rBtnPacketFormatVariable)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Packet format"));
			}
			else if (sender == pnlPayloadLength || sender == nudPayloadLength || sender == lblPayloadLength)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Payload length"));
			}
			else if (sender == pnlAddressInPayload || sender == rBtnNodeAddressInPayloadYes || sender == rBtnNodeAddressInPayloadNo)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Address in payload"));
			}
			else if (sender == pnlAddressFiltering || sender == rBtnAddressFilteringOff || sender == rBtnAddressFilteringNode || sender == rBtnAddressFilteringNodeBroadcast)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Address filtering"));
			}
			else if (sender == pnlNodeAddress || sender == nudNodeAddress || sender == lblNodeAddress)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Node address"));
			}
			else if (sender == pnlBroadcastAddress || sender == nudBroadcastAddress || sender == lblBroadcastAddress)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Broadcast address"));
			}
			else if (sender == pnlDcFree || sender == rBtnDcFreeOff || sender == rBtnDcFreeManchester || sender == rBtnDcFreeWhitening)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Dc free"));
			}
			else if (sender == pnlCrcCalculation || sender == rBtnCrcOn || sender == rBtnCrcOff)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Crc calculation"));
			}
			else if (sender == pnlCrcAutoClear || sender == rBtnCrcAutoClearOn || sender == rBtnCrcAutoClearOff)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Crc auto clear"));
			}
			else if (sender == pnlTxStart || sender == rBtnTxStartFifoLevel || sender == rBtnTxStartFifoNotEmpty)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Tx start"));
			}
			else if (sender == nudFifoThreshold)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Fifo threshold"));
			}
			else if (sender == gBoxControl)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Control"));
			}
			else if (sender == gBoxPacket)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Packet"));
			}
			else if (sender == gBoxMessage)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Message"));
			}
			else if (sender == gBoxDeviceStatus)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "Device status"));
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
	}
}
