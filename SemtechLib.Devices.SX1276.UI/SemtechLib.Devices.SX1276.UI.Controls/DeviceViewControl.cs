using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SemtechLib.Controls;
using SemtechLib.Devices.Common.Events;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.Devices.SX1276.Events;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;

namespace SemtechLib.Devices.SX1276.UI.Controls
{
	public sealed class DeviceViewControl : UserControl, IDeviceView, IDisposable, INotifyDocumentationChanged
	{
		private delegate void DevicePropertyChangedDelegate(object sender, PropertyChangedEventArgs e);

		private delegate void DevicePacketHandlerStartedDelegate(object sender, EventArgs e);

		private delegate void DevicePacketHandlerStopedDelegate(object sender, EventArgs e);

		private delegate void DevicePacketHandlerTransmittedDelegate(object sender, PacketStatusEventArg e);

		private IContainer components;

		private TabControl tabControl1;

		private TabPage tabCommon;

		private TabPage tabTransmitter;

		private TabPage tabReceiver;

		private TabPage tabIrqMap;

		private TabPage tabPacketHandler;

		private CommonViewControl commonViewControl1;

		private GroupBoxEx gBoxOperatingMode;

		private RadioButton rBtnTransmitter;

		private RadioButton rBtnReceiver;

		private RadioButton rBtnSynthesizerRx;

		private RadioButton rBtnStandby;

		private RadioButton rBtnSleep;

		private TransmitterViewControl transmitterViewControl1;

		private ReceiverViewControl receiverViewControl1;

		private IrqMapViewControl irqMapViewControl1;

		private Led ledModeReady;

		private Label lbModeReady;

		private Label label19;

		private Label label18;

		private Led ledPllLock;

		private Label label17;

		private Led ledTxReady;

		private Led ledRxReady;

		private Led ledSyncAddressMatch;

		private Label label23;

		private Label label22;

		private Led ledPreamble;

		private Label label21;

		private Label label20;

		private Led ledTimeout;

		private Led ledRssi;

		private Led ledFifoOverrun;

		private Led ledFifoLevel;

		private Label label27;

		private Led ledFifoEmpty;

		private Label label26;

		private Label label25;

		private Label label24;

		private Led ledFifoFull;

		private Led ledLowBat;

		private Led ledCrcOk;

		private Led ledPayloadReady;

		private Led ledPacketSent;

		private Label label31;

		private Label label30;

		private Label label29;

		private Label label28;

		private GroupBoxEx gBoxIrqFlags;

		private PacketHandlerView packetHandlerView1;

		private TabPage tabTemperature;

		private TemperatureViewControl temperatureViewControl1;

		private RadioButton rBtnSynthesizerTx;

		private RadioButton rBtnSynthesizer;

		private Led ledFifoNotEmpty;

		private TabPage tabSequencer;

		private SequencerViewControl sequencerViewControl1;

		private SX1276 device;

		public IDevice Device
		{
			get => device;
			set
			{
				if (device != value)
				{
					device = (SX1276)value;
					device.PropertyChanged += device_PropertyChanged;
					device.OcpTrimLimitStatusChanged += device_OcpTrimLimitStatusChanged;
					device.BitrateLimitStatusChanged += device_BitrateLimitStatusChanged;
					device.FdevLimitStatusChanged += device_FdevLimitStatusChanged;
					device.FrequencyRfLimitStatusChanged += device_FrequencyRfLimitStatusChanged;
					device.SyncValueLimitChanged += device_SyncValueLimitChanged;
					device.PacketHandlerStarted += device_PacketHandlerStarted;
					device.PacketHandlerStoped += device_PacketHandlerStoped;
					device.PacketHandlerTransmitted += device_PacketHandlerTransmitted;
					device.PacketHandlerReceived += device_PacketHandlerReceived;
					commonViewControl1.FrequencyXo = device.FrequencyXo;
					commonViewControl1.FrequencyStep = device.FrequencyStep;
					commonViewControl1.ModulationType = device.ModulationType;
					commonViewControl1.ModulationShaping = device.ModulationShaping;
					commonViewControl1.Bitrate = device.Bitrate;
					commonViewControl1.Fdev = device.Fdev;
					commonViewControl1.FrequencyRf = device.FrequencyRf;
					LoadTestPage(device);
				}
			}
		}

		public new bool Enabled
		{
			get => base.Enabled;
			set
			{
				if (base.Enabled != value)
				{
					base.Enabled = value;
				}
			}
		}

		public event SemtechLib.General.Events.ErrorEventHandler Error;

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
			tabControl1 = new TabControl();
			tabCommon = new TabPage();
			commonViewControl1 = new CommonViewControl();
			tabTransmitter = new TabPage();
			transmitterViewControl1 = new TransmitterViewControl();
			tabReceiver = new TabPage();
			receiverViewControl1 = new ReceiverViewControl();
			tabIrqMap = new TabPage();
			irqMapViewControl1 = new IrqMapViewControl();
			tabPacketHandler = new TabPage();
			packetHandlerView1 = new PacketHandlerView();
			tabSequencer = new TabPage();
			sequencerViewControl1 = new SequencerViewControl();
			tabTemperature = new TabPage();
			temperatureViewControl1 = new TemperatureViewControl();
			rBtnSynthesizer = new RadioButton();
			gBoxOperatingMode = new GroupBoxEx();
			rBtnTransmitter = new RadioButton();
			rBtnReceiver = new RadioButton();
			rBtnSynthesizerTx = new RadioButton();
			rBtnSynthesizerRx = new RadioButton();
			rBtnStandby = new RadioButton();
			rBtnSleep = new RadioButton();
			gBoxIrqFlags = new GroupBoxEx();
			ledLowBat = new Led();
			lbModeReady = new Label();
			ledCrcOk = new Led();
			ledRxReady = new Led();
			ledPayloadReady = new Led();
			ledTxReady = new Led();
			ledPacketSent = new Led();
			label17 = new Label();
			label31 = new Label();
			ledPllLock = new Led();
			label30 = new Label();
			label18 = new Label();
			label29 = new Label();
			label19 = new Label();
			label28 = new Label();
			ledModeReady = new Led();
			ledFifoOverrun = new Led();
			ledRssi = new Led();
			ledFifoLevel = new Led();
			ledTimeout = new Led();
			label27 = new Label();
			label20 = new Label();
			ledFifoEmpty = new Led();
			label21 = new Label();
			label26 = new Label();
			ledPreamble = new Led();
			label25 = new Label();
			label22 = new Label();
			label24 = new Label();
			label23 = new Label();
			ledFifoFull = new Led();
			ledSyncAddressMatch = new Led();
			ledFifoNotEmpty = new Led();
			tabControl1.SuspendLayout();
			tabCommon.SuspendLayout();
			tabTransmitter.SuspendLayout();
			tabReceiver.SuspendLayout();
			tabIrqMap.SuspendLayout();
			tabPacketHandler.SuspendLayout();
			tabSequencer.SuspendLayout();
			tabTemperature.SuspendLayout();
			gBoxOperatingMode.SuspendLayout();
			gBoxIrqFlags.SuspendLayout();
			SuspendLayout();
			tabControl1.Controls.Add(tabCommon);
			tabControl1.Controls.Add(tabTransmitter);
			tabControl1.Controls.Add(tabReceiver);
			tabControl1.Controls.Add(tabIrqMap);
			tabControl1.Controls.Add(tabPacketHandler);
			tabControl1.Controls.Add(tabSequencer);
			tabControl1.Controls.Add(tabTemperature);
			tabControl1.Location = new Point(3, 3);
			tabControl1.Name = "tabControl1";
			tabControl1.SelectedIndex = 0;
			tabControl1.Size = new Size(807, 519);
			tabControl1.TabIndex = 0;
			tabCommon.Controls.Add(commonViewControl1);
			tabCommon.Location = new Point(4, 22);
			tabCommon.Name = "tabCommon";
			tabCommon.Padding = new Padding(3);
			tabCommon.Size = new Size(799, 493);
			tabCommon.TabIndex = 0;
			tabCommon.Text = "Common";
			tabCommon.UseVisualStyleBackColor = true;
			commonViewControl1.Band = BandEnum.AUTO;
			commonViewControl1.Bitrate = new decimal([4800, 0, 0, 0]);
//			var commonViewControl = commonViewControl1;
//			var bits = new int[4];
            commonViewControl1.BitrateFrac = new decimal(new int[4]);
			commonViewControl1.FastHopOn = true;
			commonViewControl1.Fdev = new decimal([5002, 0, 0, 0]);
			commonViewControl1.ForceRxBandLowFrequencyOn = true;
			commonViewControl1.ForceTxBandLowFrequencyOn = true;
			commonViewControl1.FrequencyRf = new decimal([915000000, 0, 0, 0]);
			commonViewControl1.FrequencyStep = new decimal([61, 0, 0, 0]);
			commonViewControl1.FrequencyXo = new decimal([32000000, 0, 0, 0]);
			commonViewControl1.Location = new Point(0, 0);
			commonViewControl1.LowBatOn = true;
			commonViewControl1.LowBatTrim = LowBatTrimEnum.Trim1_835;
			commonViewControl1.LowFrequencyModeOn = true;
			commonViewControl1.ModulationShaping = 0;
			commonViewControl1.ModulationType = ModulationTypeEnum.FSK;
			commonViewControl1.Name = "commonViewControl1";
			commonViewControl1.Size = new Size(799, 493);
			commonViewControl1.TabIndex = 0;
			commonViewControl1.TcxoInputOn = true;
			commonViewControl1.FrequencyXoChanged += commonViewControl1_FrequencyXoChanged;
			commonViewControl1.ModulationTypeChanged += commonViewControl1_ModulationTypeChanged;
			commonViewControl1.ModulationShapingChanged += commonViewControl1_ModulationShapingChanged;
			commonViewControl1.BitrateChanged += commonViewControl1_BitrateChanged;
			commonViewControl1.BitrateFracChanged += commonViewControl1_BitrateFracChanged;
			commonViewControl1.FdevChanged += commonViewControl1_FdevChanged;
			commonViewControl1.BandChanged += commonViewControl1_BandChanged;
			commonViewControl1.ForceTxBandLowFrequencyOnChanged += commonViewControl1_ForceTxBandLowFrequencyOnChanged;
			commonViewControl1.ForceRxBandLowFrequencyOnChanged += commonViewControl1_ForceRxBandLowFrequencyOnChanged;
			commonViewControl1.LowFrequencyModeOnChanged += commonViewControl1_LowFrequencyModeOnChanged;
			commonViewControl1.FrequencyRfChanged += commonViewControl1_FrequencyRfChanged;
			commonViewControl1.FastHopOnChanged += commonViewControl1_FastHopOnChanged;
			commonViewControl1.TcxoInputChanged += commonViewControl1_TcxoInputChanged;
			commonViewControl1.RcCalibrationChanged += commonViewControl1_RcCalibrationChanged;
			commonViewControl1.LowBatOnChanged += commonViewControl1_LowBatOnChanged;
			commonViewControl1.LowBatTrimChanged += commonViewControl1_LowBatTrimChanged;
			commonViewControl1.DocumentationChanged += commonViewControl1_DocumentationChanged;
			tabTransmitter.Controls.Add(transmitterViewControl1);
			tabTransmitter.Location = new Point(4, 22);
			tabTransmitter.Name = "tabTransmitter";
			tabTransmitter.Padding = new Padding(3);
			tabTransmitter.Size = new Size(799, 493);
			tabTransmitter.TabIndex = 1;
			tabTransmitter.Text = "Transmitter";
			tabTransmitter.UseVisualStyleBackColor = true;
			transmitterViewControl1.Location = new Point(0, 0);
			transmitterViewControl1.MaxOutputPower = new decimal([132, 0, 0, 65536]);
			transmitterViewControl1.Name = "transmitterViewControl1";
			transmitterViewControl1.OcpOn = true;
			transmitterViewControl1.OcpTrim = new decimal([1000, 0, 0, 65536]);
			transmitterViewControl1.OutputPower = new decimal([132, 0, 0, 65536]);
			transmitterViewControl1.Pa20dBm = false;
			transmitterViewControl1.PaRamp = PaRampEnum.PaRamp_40;
			transmitterViewControl1.PaSelect = PaSelectEnum.RFO;
			transmitterViewControl1.PllBandwidth = new decimal([300000, 0, 0, 0]);
			transmitterViewControl1.Size = new Size(799, 493);
			transmitterViewControl1.TabIndex = 0;
			transmitterViewControl1.PaModeChanged += transmitterViewControl1_PaModeChanged;
			transmitterViewControl1.OutputPowerChanged += transmitterViewControl1_OutputPowerChanged;
			transmitterViewControl1.MaxOutputPowerChanged += transmitterViewControl1_MaxOutputPowerChanged;
			transmitterViewControl1.PaRampChanged += transmitterViewControl1_PaRampChanged;
			transmitterViewControl1.OcpOnChanged += transmitterViewControl1_OcpOnChanged;
			transmitterViewControl1.OcpTrimChanged += transmitterViewControl1_OcpTrimChanged;
			transmitterViewControl1.Pa20dBmChanged += transmitterViewControl1_Pa20dBmChanged;
			transmitterViewControl1.PllBandwidthChanged += transmitterViewControl1_PllBandwidthChanged;
			transmitterViewControl1.DocumentationChanged += transmitterViewControl1_DocumentationChanged;
			tabReceiver.Controls.Add(receiverViewControl1);
			tabReceiver.Location = new Point(4, 22);
			tabReceiver.Name = "tabReceiver";
			tabReceiver.Padding = new Padding(3);
			tabReceiver.Size = new Size(799, 493);
			tabReceiver.TabIndex = 2;
			tabReceiver.Text = "Receiver";
			tabReceiver.UseVisualStyleBackColor = true;
			receiverViewControl1.AfcAutoClearOn = true;
			receiverViewControl1.AfcAutoOn = true;
			receiverViewControl1.AfcRxBw = new decimal([50000, 0, 0, 0]);
			receiverViewControl1.AfcRxBwMax = new decimal([400000, 0, 0, 0]);
			receiverViewControl1.AfcRxBwMin = new decimal([3125, 0, 0, 0]);
			receiverViewControl1.AfcValue = new decimal([0, 0, 0, 65536]);
			receiverViewControl1.AgcAutoOn = true;
			receiverViewControl1.AgcReference = -80;
			receiverViewControl1.AgcReferenceLevel = 19;
			receiverViewControl1.AgcStep1 = 16;
			receiverViewControl1.AgcStep2 = 7;
			receiverViewControl1.AgcStep3 = 11;
			receiverViewControl1.AgcStep4 = 9;
			receiverViewControl1.AgcStep5 = 11;
			receiverViewControl1.AgcThresh1 = 0;
			receiverViewControl1.AgcThresh2 = 0;
			receiverViewControl1.AgcThresh3 = 0;
			receiverViewControl1.AgcThresh4 = 0;
			receiverViewControl1.AgcThresh5 = 0;
			receiverViewControl1.Bitrate = new decimal([4800, 0, 0, 0]);
			receiverViewControl1.BitSyncOn = true;
			receiverViewControl1.DataMode = DataModeEnum.Packet;
			receiverViewControl1.FeiValue = new decimal([0, 0, 0, 65536]);
			receiverViewControl1.FrequencyXo = new decimal([32000000, 0, 0, 0]);
//			var receiverViewControl = receiverViewControl1;
//			var bits2 = new int[4];
            receiverViewControl1.InterPacketRxDelay = new decimal(new int[4]);
			receiverViewControl1.LnaBoost = true;
			receiverViewControl1.Location = new Point(0, 0);
			receiverViewControl1.ModulationType = ModulationTypeEnum.FSK;
			receiverViewControl1.Name = "receiverViewControl1";
//			var receiverViewControl2 = receiverViewControl1;
//			var bits3 = new int[4];
            receiverViewControl1.OokAverageOffset = new decimal(new int[4]);
			receiverViewControl1.OokAverageThreshFilt = OokAverageThreshFiltEnum.COEF_2;
			receiverViewControl1.OokFixedThreshold = 6;
			receiverViewControl1.OokPeakThreshDec = OokPeakThreshDecEnum.EVERY_1_CHIPS_1_TIMES;
			receiverViewControl1.OokPeakThreshStep = new decimal([5, 0, 0, 65536]);
			receiverViewControl1.OokThreshType = OokThreshTypeEnum.Peak;
			receiverViewControl1.PreambleDetectorOn = true;
			receiverViewControl1.PreambleDetectorSize = 1;
			receiverViewControl1.PreambleDetectorTol = 0;
			receiverViewControl1.RestartRxOnCollision = true;
//			var receiverViewControl3 = receiverViewControl1;
//			var bits4 = new int[4];
            receiverViewControl1.RssiCollisionThreshold = new decimal(new int[4]);
//			var receiverViewControl4 = receiverViewControl1;
//			var bits5 = new int[4];
            receiverViewControl1.RssiOffset = new decimal(new int[4]);
			receiverViewControl1.RssiSmoothing = new decimal([2, 0, 0, 0]);
			receiverViewControl1.RssiThreshold = new decimal([116, 0, 0, -2147483648]);
			receiverViewControl1.RssiValue = new decimal([1275, 0, 0, -2147418112]);
			receiverViewControl1.RxBw = new decimal([1890233003, -2135170438, 564688631, 1572864]);
			receiverViewControl1.RxBwMax = new decimal([500000, 0, 0, 0]);
			receiverViewControl1.RxBwMin = new decimal([3906, 0, 0, 0]);
			receiverViewControl1.Size = new Size(799, 493);
			receiverViewControl1.TabIndex = 0;
//			var receiverViewControl5 = receiverViewControl1;
//			var bits6 = new int[4];
            receiverViewControl1.TimeoutRxPreamble = new decimal(new int[4]);
//			var receiverViewControl6 = receiverViewControl1;
//			var bits7 = new int[4];
            receiverViewControl1.TimeoutRxRssi = new decimal(new int[4]);
//			var receiverViewControl7 = receiverViewControl1;
//			var bits8 = new int[4];
            receiverViewControl1.TimeoutSignalSync = new decimal(new int[4]);
			receiverViewControl1.AgcReferenceLevelChanged += receiverViewControl1_AgcReferenceLevelChanged;
			receiverViewControl1.AgcStepChanged += receiverViewControl1_AgcStepChanged;
			receiverViewControl1.LnaGainChanged += receiverViewControl1_LnaGainChanged;
			receiverViewControl1.LnaBoostChanged += receiverViewControl1_LnaBoostChanged;
			receiverViewControl1.RestartRxOnCollisionOnChanged += receiverViewControl1_RestartRxOnCollisionOnChanged;
			receiverViewControl1.RestartRxWithoutPllLockChanged += receiverViewControl1_RestartRxWithoutPllLockChanged;
			receiverViewControl1.RestartRxWithPllLockChanged += receiverViewControl1_RestartRxWithPllLockChanged;
			receiverViewControl1.AfcAutoOnChanged += receiverViewControl1_AfcAutoOnChanged;
			receiverViewControl1.AgcAutoOnChanged += receiverViewControl1_AgcAutoOnChanged;
			receiverViewControl1.RxTriggerChanged += receiverViewControl1_RxTriggerChanged;
			receiverViewControl1.RssiOffsetChanged += receiverViewControl1_RssiOffsetChanged;
			receiverViewControl1.RssiSmoothingChanged += receiverViewControl1_RssiSmoothingChanged;
			receiverViewControl1.RssiCollisionThresholdChanged += receiverViewControl1_RssiCollisionThresholdChanged;
			receiverViewControl1.RssiThreshChanged += receiverViewControl1_RssiThreshChanged;
			receiverViewControl1.RxBwChanged += receiverViewControl1_RxBwChanged;
			receiverViewControl1.AfcRxBwChanged += receiverViewControl1_AfcRxBwChanged;
			receiverViewControl1.BitSyncOnChanged += receiverViewControl1_BitSyncOnChanged;
			receiverViewControl1.OokThreshTypeChanged += receiverViewControl1_OokThreshTypeChanged;
			receiverViewControl1.OokPeakThreshStepChanged += receiverViewControl1_OokPeakThreshStepChanged;
			receiverViewControl1.OokPeakThreshDecChanged += receiverViewControl1_OokPeakThreshDecChanged;
			receiverViewControl1.OokAverageThreshFiltChanged += receiverViewControl1_OokAverageThreshFiltChanged;
			receiverViewControl1.OokAverageBiasChanged += receiverViewControl1_OokAverageBiasChanged;
			receiverViewControl1.OokFixedThreshChanged += receiverViewControl1_OokFixedThreshChanged;
			receiverViewControl1.AgcStartChanged += receiverViewControl1_AgcStartChanged;
			receiverViewControl1.FeiReadChanged += receiverViewControl1_FeiReadChanged;
			receiverViewControl1.AfcAutoClearOnChanged += receiverViewControl1_AfcAutoClearOnChanged;
			receiverViewControl1.AfcClearChanged += receiverViewControl1_AfcClearChanged;
			receiverViewControl1.PreambleDetectorOnChanged += receiverViewControl1_PreambleDetectorOnChanged;
			receiverViewControl1.PreambleDetectorSizeChanged += receiverViewControl1_PreambleDetectorSizeChanged;
			receiverViewControl1.PreambleDetectorTolChanged += receiverViewControl1_PreambleDetectorTolChanged;
			receiverViewControl1.TimeoutRssiChanged += receiverViewControl1_TimeoutRssiChanged;
			receiverViewControl1.TimeoutPreambleChanged += receiverViewControl1_TimeoutPreambleChanged;
			receiverViewControl1.TimeoutSyncWordChanged += receiverViewControl1_TimeoutSyncWordChanged;
			receiverViewControl1.AutoRxRestartDelayChanged += receiverViewControl1_AutoRxRestartDelayChanged;
			receiverViewControl1.DocumentationChanged += receiverViewControl1_DocumentationChanged;
			tabIrqMap.Controls.Add(irqMapViewControl1);
			tabIrqMap.Location = new Point(4, 22);
			tabIrqMap.Name = "tabIrqMap";
			tabIrqMap.Padding = new Padding(3);
			tabIrqMap.Size = new Size(799, 493);
			tabIrqMap.TabIndex = 3;
			tabIrqMap.Text = "IRQ & Map";
			tabIrqMap.UseVisualStyleBackColor = true;
			irqMapViewControl1.BitSyncOn = true;
			irqMapViewControl1.DataMode = DataModeEnum.Packet;
			irqMapViewControl1.FrequencyXo = new decimal([32000000, 0, 0, 0]);
			irqMapViewControl1.Location = new Point(0, 0);
			irqMapViewControl1.MapPreambleDetect = false;
			irqMapViewControl1.Mode = OperatingModeEnum.Stdby;
			irqMapViewControl1.Name = "irqMapViewControl1";
			irqMapViewControl1.Size = new Size(799, 493);
			irqMapViewControl1.TabIndex = 0;
			irqMapViewControl1.DioPreambleIrqOnChanged += irqMapViewControl1_DioPreambleIrqOnChanged;
			irqMapViewControl1.DioMappingChanged += irqMapViewControl1_DioMappingChanged;
			irqMapViewControl1.ClockOutChanged += irqMapViewControl1_ClockOutChanged;
			irqMapViewControl1.DocumentationChanged += irqMapViewControl1_DocumentationChanged;
			tabPacketHandler.Controls.Add(packetHandlerView1);
			tabPacketHandler.Location = new Point(4, 22);
			tabPacketHandler.Name = "tabPacketHandler";
			tabPacketHandler.Padding = new Padding(3);
			tabPacketHandler.Size = new Size(799, 493);
			tabPacketHandler.TabIndex = 4;
			tabPacketHandler.Text = "Packet Handler";
			tabPacketHandler.UseVisualStyleBackColor = true;
			packetHandlerView1.AddressFiltering = AddressFilteringEnum.OFF;
			packetHandlerView1.AutoRestartRxOn = AutoRestartRxEnum.PLL_WAIT_ON;
			packetHandlerView1.BeaconOn = true;
//			var packetHandlerView = packetHandlerView1;
//			var bits9 = new int[4];
            packetHandlerView1.Bitrate = new decimal(new int[4]);
			packetHandlerView1.BitSyncOn = true;
			packetHandlerView1.BroadcastAddress = 0;
			packetHandlerView1.Crc = 0;
			packetHandlerView1.CrcAutoClearOff = false;
			packetHandlerView1.CrcIbmOn = true;
			packetHandlerView1.CrcOn = true;
			packetHandlerView1.DataMode = DataModeEnum.Packet;
			packetHandlerView1.DcFree = DcFreeEnum.OFF;
			packetHandlerView1.FifoFillCondition = FifoFillConditionEnum.OnSyncAddressIrq;
			packetHandlerView1.FifoThreshold = 15;
			packetHandlerView1.IoHomeOn = true;
			packetHandlerView1.IoHomePwrFrameOn = true;
			packetHandlerView1.Location = new Point(0, 0);
			packetHandlerView1.LogEnabled = false;
			packetHandlerView1.MaxPacketNumber = 0;
			packetHandlerView1.Message = [];
			packetHandlerView1.MessageLength = 0;
			packetHandlerView1.Mode = OperatingModeEnum.Stdby;
			packetHandlerView1.Name = "packetHandlerView1";
			packetHandlerView1.NodeAddress = 0;
			packetHandlerView1.NodeAddressRx = 0;
			packetHandlerView1.PacketFormat = PacketFormatEnum.Fixed;
			packetHandlerView1.PacketNumber = 0;
			packetHandlerView1.PayloadLength = 66;
			packetHandlerView1.PreamblePolarity = PreamblePolarityEnum.POLARITY_AA;
			packetHandlerView1.PreambleSize = 3;
			packetHandlerView1.Size = new Size(799, 493);
			packetHandlerView1.StartStop = false;
			packetHandlerView1.SyncOn = true;
			packetHandlerView1.SyncSize = 4;
			packetHandlerView1.SyncValue = [170, 170, 170, 170];
			packetHandlerView1.TabIndex = 0;
			packetHandlerView1.TxStartCondition = true;
			packetHandlerView1.Error += packetHandlerView1_Error;
			packetHandlerView1.DataModeChanged += packetHandlerView1_DataModeChanged;
			packetHandlerView1.PreambleSizeChanged += packetHandlerView1_PreambleSizeChanged;
			packetHandlerView1.AutoRestartRxChanged += packetHandlerView1_AutoRestartRxChanged;
			packetHandlerView1.PreamblePolarityChanged += packetHandlerView1_PreamblePolarityChanged;
			packetHandlerView1.SyncOnChanged += packetHandlerView1_SyncOnChanged;
			packetHandlerView1.FifoFillConditionChanged += packetHandlerView1_FifoFillConditionChanged;
			packetHandlerView1.SyncSizeChanged += packetHandlerView1_SyncSizeChanged;
			packetHandlerView1.SyncValueChanged += packetHandlerView1_SyncValueChanged;
			packetHandlerView1.PacketFormatChanged += packetHandlerView1_PacketFormatChanged;
			packetHandlerView1.DcFreeChanged += packetHandlerView1_DcFreeChanged;
			packetHandlerView1.CrcOnChanged += packetHandlerView1_CrcOnChanged;
			packetHandlerView1.CrcAutoClearOffChanged += packetHandlerView1_CrcAutoClearOffChanged;
			packetHandlerView1.AddressFilteringChanged += packetHandlerView1_AddressFilteringChanged;
			packetHandlerView1.PayloadLengthChanged += packetHandlerView1_PayloadLengthChanged;
			packetHandlerView1.NodeAddressChanged += packetHandlerView1_NodeAddressChanged;
			packetHandlerView1.BroadcastAddressChanged += packetHandlerView1_BroadcastAddressChanged;
			packetHandlerView1.TxStartConditionChanged += packetHandlerView1_TxStartConditionChanged;
			packetHandlerView1.FifoThresholdChanged += packetHandlerView1_FifoThresholdChanged;
			packetHandlerView1.MessageLengthChanged += packetHandlerView1_MessageLengthChanged;
			packetHandlerView1.MessageChanged += packetHandlerView1_MessageChanged;
			packetHandlerView1.StartStopChanged += packetHandlerView1_StartStopChanged;
			packetHandlerView1.MaxPacketNumberChanged += packetHandlerView1_MaxPacketNumberChanged;
			packetHandlerView1.PacketHandlerLogEnableChanged += packetHandlerView1_PacketHandlerLogEnableChanged;
			packetHandlerView1.CrcIbmChanged += packetHandlerView1_CrcIbmChanged;
			packetHandlerView1.IoHomeOnChanged += packetHandlerView1_IoHomeOnChanged;
			packetHandlerView1.IoHomePwrFrameOnChanged += packetHandlerView1_IoHomePwrFrameOnChanged;
			packetHandlerView1.BeaconOnChanged += packetHandlerView1_BeaconOnChanged;
			packetHandlerView1.FillFifoChanged += packetHandlerView1_FillFifoChanged;
			packetHandlerView1.DocumentationChanged += packetHandlerView1_DocumentationChanged;
			tabSequencer.Controls.Add(sequencerViewControl1);
			tabSequencer.Location = new Point(4, 22);
			tabSequencer.Name = "tabSequencer";
			tabSequencer.Size = new Size(799, 493);
			tabSequencer.TabIndex = 6;
			tabSequencer.Text = "Sequencer";
			tabSequencer.UseVisualStyleBackColor = true;
			sequencerViewControl1.FromIdle = FromIdle.TO_RX_ON_TMR1;
			sequencerViewControl1.FromPacketReceived = FromPacketReceived.TO_IDLE;
			sequencerViewControl1.FromReceive = FromReceive.UNUSED_1;
			sequencerViewControl1.FromRxTimeout = FromRxTimeout.TO_RX_RESTART;
			sequencerViewControl1.FromStart = FromStart.TO_LOW_POWER_SELECTION;
			sequencerViewControl1.FromTransmit = FromTransmit.TO_RX;
			sequencerViewControl1.IdleMode = IdleMode.STANDBY;
			sequencerViewControl1.Location = new Point(0, 0);
			sequencerViewControl1.LowPowerSelection = LowPowerSelection.TO_LPM;
			sequencerViewControl1.Name = "sequencerViewControl1";
			sequencerViewControl1.Size = new Size(799, 493);
			sequencerViewControl1.TabIndex = 0;
			sequencerViewControl1.Timer1Coef = 245;
			sequencerViewControl1.Timer1Resolution = TimerResolution.OFF;
			sequencerViewControl1.Timer2Coef = 32;
			sequencerViewControl1.Timer2Resolution = TimerResolution.OFF;
			sequencerViewControl1.SequencerStartChanged += sequencerViewControl1_SequencerStartChanged;
			sequencerViewControl1.SequencerStopChanged += sequencerViewControl1_SequencerStopChanged;
			sequencerViewControl1.IdleModeChanged += sequencerViewControl1_IdleModeChanged;
			sequencerViewControl1.FromStartChanged += sequencerViewControl1_FromStartChanged;
			sequencerViewControl1.LowPowerSelectionChanged += sequencerViewControl1_LowPowerSelectionChanged;
			sequencerViewControl1.FromIdleChanged += sequencerViewControl1_FromIdleChanged;
			sequencerViewControl1.FromTransmitChanged += sequencerViewControl1_FromTransmitChanged;
			sequencerViewControl1.FromReceiveChanged += sequencerViewControl1_FromReceiveChanged;
			sequencerViewControl1.FromRxTimeoutChanged += sequencerViewControl1_FromRxTimeoutChanged;
			sequencerViewControl1.FromPacketReceivedChanged += sequencerViewControl1_FromPacketReceivedChanged;
			sequencerViewControl1.Timer1ResolutionChanged += sequencerViewControl1_Timer1ResolutionChanged;
			sequencerViewControl1.Timer2ResolutionChanged += sequencerViewControl1_Timer2ResolutionChanged;
			sequencerViewControl1.Timer1CoefChanged += sequencerViewControl1_Timer1CoefChanged;
			sequencerViewControl1.Timer2CoefChanged += sequencerViewControl1_Timer2CoefChanged;
			sequencerViewControl1.DocumentationChanged += sequencerViewControl1_DocumentationChanged;
			tabTemperature.Controls.Add(temperatureViewControl1);
			tabTemperature.Location = new Point(4, 22);
			tabTemperature.Name = "tabTemperature";
			tabTemperature.Padding = new Padding(3);
			tabTemperature.Size = new Size(799, 493);
			tabTemperature.TabIndex = 5;
			tabTemperature.Text = "Temperature";
			tabTemperature.UseVisualStyleBackColor = true;
			temperatureViewControl1.AutoImageCalOn = true;
			temperatureViewControl1.ImageCalRunning = false;
			temperatureViewControl1.Location = new Point(0, 0);
			temperatureViewControl1.Mode = OperatingModeEnum.Stdby;
			temperatureViewControl1.Name = "temperatureViewControl1";
			temperatureViewControl1.Size = new Size(799, 493);
			temperatureViewControl1.TabIndex = 0;
			temperatureViewControl1.TempCalDone = false;
			temperatureViewControl1.TempChange = false;
			temperatureViewControl1.TempDelta = new decimal([0, 0, 0, 65536]);
			temperatureViewControl1.TempMeasRunning = false;
			temperatureViewControl1.TempMonitorOff = true;
			temperatureViewControl1.TempThreshold = TempThresholdEnum.THRESH_05;
			temperatureViewControl1.TempValue = new decimal([25, 0, 0, 0]);
			temperatureViewControl1.TempValueRoom = new decimal([250, 0, 0, 65536]);
			temperatureViewControl1.RxCalAutoOnChanged += temperatureViewControl1_RxCalAutoOnChanged;
			temperatureViewControl1.RxCalibrationChanged += temperatureViewControl1_RxCalibrationChanged;
			temperatureViewControl1.TempThresholdChanged += temperatureViewControl1_TempThresholdChanged;
			temperatureViewControl1.TempCalibrateChanged += temperatureViewControl1_TempCalibrateChanged;
			temperatureViewControl1.TempMeasOnChanged += temperatureViewControl1_TempMeasOnChanged;
			temperatureViewControl1.DocumentationChanged += temperatureViewControl1_DocumentationChanged;
			rBtnSynthesizer.AutoSize = true;
			rBtnSynthesizer.Location = new Point(16, 51);
			rBtnSynthesizer.Name = "rBtnSynthesizer";
			rBtnSynthesizer.Size = new Size(79, 17);
			rBtnSynthesizer.TabIndex = 2;
			rBtnSynthesizer.Text = "Synthesizer";
			rBtnSynthesizer.UseVisualStyleBackColor = true;
			rBtnSynthesizer.CheckedChanged += rBtnOperatingMode_CheckedChanged;
			gBoxOperatingMode.Controls.Add(rBtnTransmitter);
			gBoxOperatingMode.Controls.Add(rBtnReceiver);
			gBoxOperatingMode.Controls.Add(rBtnSynthesizerTx);
			gBoxOperatingMode.Controls.Add(rBtnSynthesizerRx);
			gBoxOperatingMode.Controls.Add(rBtnStandby);
			gBoxOperatingMode.Controls.Add(rBtnSleep);
			gBoxOperatingMode.Location = new Point(816, 411);
			gBoxOperatingMode.Name = "gBoxOperatingMode";
			gBoxOperatingMode.Size = new Size(189, 107);
			gBoxOperatingMode.TabIndex = 2;
			gBoxOperatingMode.TabStop = false;
			gBoxOperatingMode.Text = "Operating mode";
			gBoxOperatingMode.MouseEnter += control_MouseEnter;
			gBoxOperatingMode.MouseLeave += control_MouseLeave;
			rBtnTransmitter.AutoSize = true;
			rBtnTransmitter.Location = new Point(94, 51);
			rBtnTransmitter.Name = "rBtnTransmitter";
			rBtnTransmitter.Size = new Size(77, 17);
			rBtnTransmitter.TabIndex = 4;
			rBtnTransmitter.Text = "Transmitter";
			rBtnTransmitter.UseVisualStyleBackColor = true;
			rBtnTransmitter.CheckedChanged += rBtnOperatingMode_CheckedChanged;
			rBtnReceiver.AutoSize = true;
			rBtnReceiver.Location = new Point(94, 80);
			rBtnReceiver.Name = "rBtnReceiver";
			rBtnReceiver.Size = new Size(68, 17);
			rBtnReceiver.TabIndex = 3;
			rBtnReceiver.Text = "Receiver";
			rBtnReceiver.UseVisualStyleBackColor = true;
			rBtnReceiver.CheckedChanged += rBtnOperatingMode_CheckedChanged;
			rBtnSynthesizerTx.AutoSize = true;
			rBtnSynthesizerTx.Location = new Point(16, 51);
			rBtnSynthesizerTx.Name = "rBtnSynthesizerTx";
			rBtnSynthesizerTx.Size = new Size(70, 17);
			rBtnSynthesizerTx.TabIndex = 2;
			rBtnSynthesizerTx.Text = "Synth. Tx";
			rBtnSynthesizerTx.UseVisualStyleBackColor = true;
			rBtnSynthesizerTx.CheckedChanged += rBtnOperatingMode_CheckedChanged;
			rBtnSynthesizerRx.AutoSize = true;
			rBtnSynthesizerRx.Location = new Point(16, 80);
			rBtnSynthesizerRx.Name = "rBtnSynthesizerRx";
			rBtnSynthesizerRx.Size = new Size(71, 17);
			rBtnSynthesizerRx.TabIndex = 2;
			rBtnSynthesizerRx.Text = "Synth. Rx";
			rBtnSynthesizerRx.UseVisualStyleBackColor = true;
			rBtnSynthesizerRx.CheckedChanged += rBtnOperatingMode_CheckedChanged;
			rBtnStandby.AutoSize = true;
			rBtnStandby.Checked = true;
			rBtnStandby.Location = new Point(94, 20);
			rBtnStandby.Name = "rBtnStandby";
			rBtnStandby.Size = new Size(64, 17);
			rBtnStandby.TabIndex = 1;
			rBtnStandby.TabStop = true;
			rBtnStandby.Text = "Standby";
			rBtnStandby.UseVisualStyleBackColor = true;
			rBtnStandby.CheckedChanged += rBtnOperatingMode_CheckedChanged;
			rBtnSleep.AutoSize = true;
			rBtnSleep.Location = new Point(16, 20);
			rBtnSleep.Name = "rBtnSleep";
			rBtnSleep.Size = new Size(52, 17);
			rBtnSleep.TabIndex = 0;
			rBtnSleep.Text = "Sleep";
			rBtnSleep.UseVisualStyleBackColor = true;
			rBtnSleep.CheckedChanged += rBtnOperatingMode_CheckedChanged;
			gBoxIrqFlags.Controls.Add(ledLowBat);
			gBoxIrqFlags.Controls.Add(lbModeReady);
			gBoxIrqFlags.Controls.Add(ledCrcOk);
			gBoxIrqFlags.Controls.Add(ledRxReady);
			gBoxIrqFlags.Controls.Add(ledPayloadReady);
			gBoxIrqFlags.Controls.Add(ledTxReady);
			gBoxIrqFlags.Controls.Add(ledPacketSent);
			gBoxIrqFlags.Controls.Add(label17);
			gBoxIrqFlags.Controls.Add(label31);
			gBoxIrqFlags.Controls.Add(ledPllLock);
			gBoxIrqFlags.Controls.Add(label30);
			gBoxIrqFlags.Controls.Add(label18);
			gBoxIrqFlags.Controls.Add(label29);
			gBoxIrqFlags.Controls.Add(label19);
			gBoxIrqFlags.Controls.Add(label28);
			gBoxIrqFlags.Controls.Add(ledModeReady);
			gBoxIrqFlags.Controls.Add(ledFifoOverrun);
			gBoxIrqFlags.Controls.Add(ledRssi);
			gBoxIrqFlags.Controls.Add(ledFifoLevel);
			gBoxIrqFlags.Controls.Add(ledTimeout);
			gBoxIrqFlags.Controls.Add(label27);
			gBoxIrqFlags.Controls.Add(label20);
			gBoxIrqFlags.Controls.Add(ledFifoEmpty);
			gBoxIrqFlags.Controls.Add(label21);
			gBoxIrqFlags.Controls.Add(label26);
			gBoxIrqFlags.Controls.Add(ledPreamble);
			gBoxIrqFlags.Controls.Add(label25);
			gBoxIrqFlags.Controls.Add(label22);
			gBoxIrqFlags.Controls.Add(label24);
			gBoxIrqFlags.Controls.Add(label23);
			gBoxIrqFlags.Controls.Add(ledFifoFull);
			gBoxIrqFlags.Controls.Add(ledSyncAddressMatch);
			gBoxIrqFlags.Location = new Point(816, 25);
			gBoxIrqFlags.Name = "gBoxIrqFlags";
			gBoxIrqFlags.Size = new Size(189, 380);
			gBoxIrqFlags.TabIndex = 1;
			gBoxIrqFlags.TabStop = false;
			gBoxIrqFlags.Text = "Irq flags";
			gBoxIrqFlags.MouseEnter += control_MouseEnter;
			gBoxIrqFlags.MouseLeave += control_MouseLeave;
			ledLowBat.BackColor = Color.Transparent;
			ledLowBat.LedColor = Color.Green;
			ledLowBat.LedSize = new Size(11, 11);
			ledLowBat.Location = new Point(34, 352);
			ledLowBat.Name = "ledLowBat";
			ledLowBat.Size = new Size(15, 15);
			ledLowBat.TabIndex = 30;
			ledLowBat.Text = "led1";
			ledLowBat.Click += ledLowBat_Click;
			lbModeReady.AutoSize = true;
			lbModeReady.Location = new Point(55, 20);
			lbModeReady.Name = "lbModeReady";
			lbModeReady.Size = new Size(65, 13);
			lbModeReady.TabIndex = 1;
			lbModeReady.Text = "ModeReady";
			ledCrcOk.BackColor = Color.Transparent;
			ledCrcOk.LedColor = Color.Green;
			ledCrcOk.LedSize = new Size(11, 11);
			ledCrcOk.Location = new Point(34, 331);
			ledCrcOk.Name = "ledCrcOk";
			ledCrcOk.Size = new Size(15, 15);
			ledCrcOk.TabIndex = 28;
			ledCrcOk.Text = "led1";
			ledRxReady.BackColor = Color.Transparent;
			ledRxReady.LedColor = Color.Green;
			ledRxReady.LedSize = new Size(11, 11);
			ledRxReady.Location = new Point(34, 40);
			ledRxReady.Name = "ledRxReady";
			ledRxReady.Size = new Size(15, 15);
			ledRxReady.TabIndex = 2;
			ledRxReady.Text = "led1";
			ledPayloadReady.BackColor = Color.Transparent;
			ledPayloadReady.LedColor = Color.Green;
			ledPayloadReady.LedSize = new Size(11, 11);
			ledPayloadReady.Location = new Point(34, 310);
			ledPayloadReady.Name = "ledPayloadReady";
			ledPayloadReady.Size = new Size(15, 15);
			ledPayloadReady.TabIndex = 26;
			ledPayloadReady.Text = "led1";
			ledTxReady.BackColor = Color.Transparent;
			ledTxReady.LedColor = Color.Green;
			ledTxReady.LedSize = new Size(11, 11);
			ledTxReady.Location = new Point(34, 61);
			ledTxReady.Name = "ledTxReady";
			ledTxReady.Size = new Size(15, 15);
			ledTxReady.TabIndex = 4;
			ledTxReady.Text = "led1";
			ledPacketSent.BackColor = Color.Transparent;
			ledPacketSent.LedColor = Color.Green;
			ledPacketSent.LedSize = new Size(11, 11);
			ledPacketSent.Location = new Point(34, 289);
			ledPacketSent.Margin = new Padding(3, 6, 3, 3);
			ledPacketSent.Name = "ledPacketSent";
			ledPacketSent.Size = new Size(15, 15);
			ledPacketSent.TabIndex = 24;
			ledPacketSent.Text = "led1";
			label17.AutoSize = true;
			label17.Location = new Point(55, 83);
			label17.Name = "label17";
			label17.Size = new Size(42, 13);
			label17.TabIndex = 7;
			label17.Text = "PllLock";
			label31.AutoSize = true;
			label31.Location = new Point(55, 290);
			label31.Name = "label31";
			label31.Size = new Size(63, 13);
			label31.TabIndex = 25;
			label31.Text = "PacketSent";
			ledPllLock.BackColor = Color.Transparent;
			ledPllLock.LedColor = Color.Green;
			ledPllLock.LedSize = new Size(11, 11);
			ledPllLock.Location = new Point(34, 82);
			ledPllLock.Margin = new Padding(3, 3, 3, 6);
			ledPllLock.Name = "ledPllLock";
			ledPllLock.Size = new Size(15, 15);
			ledPllLock.TabIndex = 6;
			ledPllLock.Text = "led1";
			label30.AutoSize = true;
			label30.Location = new Point(55, 311);
			label30.Name = "label30";
			label30.Size = new Size(76, 13);
			label30.TabIndex = 27;
			label30.Text = "PayloadReady";
			label18.AutoSize = true;
			label18.Location = new Point(55, 62);
			label18.Name = "label18";
			label18.Size = new Size(50, 13);
			label18.TabIndex = 5;
			label18.Text = "TxReady";
			label29.AutoSize = true;
			label29.Location = new Point(55, 332);
			label29.Name = "label29";
			label29.Size = new Size(37, 13);
			label29.TabIndex = 29;
			label29.Text = "CrcOk";
			label19.AutoSize = true;
			label19.Location = new Point(55, 41);
			label19.Name = "label19";
			label19.Size = new Size(51, 13);
			label19.TabIndex = 3;
			label19.Text = "RxReady";
			label28.AutoSize = true;
			label28.Location = new Point(55, 353);
			label28.Name = "label28";
			label28.Size = new Size(43, 13);
			label28.TabIndex = 31;
			label28.Text = "LowBat";
			ledModeReady.BackColor = Color.Transparent;
			ledModeReady.LedColor = Color.Green;
			ledModeReady.LedSize = new Size(11, 11);
			ledModeReady.Location = new Point(34, 19);
			ledModeReady.Name = "ledModeReady";
			ledModeReady.Size = new Size(15, 15);
			ledModeReady.TabIndex = 0;
			ledModeReady.Text = "Mode Ready";
			ledFifoOverrun.BackColor = Color.Transparent;
			ledFifoOverrun.LedColor = Color.Green;
			ledFifoOverrun.LedSize = new Size(11, 11);
			ledFifoOverrun.Location = new Point(34, 262);
			ledFifoOverrun.Margin = new Padding(3, 3, 3, 6);
			ledFifoOverrun.Name = "ledFifoOverrun";
			ledFifoOverrun.Size = new Size(15, 15);
			ledFifoOverrun.TabIndex = 22;
			ledFifoOverrun.Text = "led1";
			ledFifoOverrun.Click += ledFifoOverrun_Click;
			ledRssi.BackColor = Color.Transparent;
			ledRssi.LedColor = Color.Green;
			ledRssi.LedSize = new Size(11, 11);
			ledRssi.Location = new Point(34, 109);
			ledRssi.Margin = new Padding(3, 6, 3, 3);
			ledRssi.Name = "ledRssi";
			ledRssi.Size = new Size(15, 15);
			ledRssi.TabIndex = 8;
			ledRssi.Text = "led1";
			ledRssi.Click += ledRssi_Click;
			ledFifoLevel.BackColor = Color.Transparent;
			ledFifoLevel.LedColor = Color.Green;
			ledFifoLevel.LedSize = new Size(11, 11);
			ledFifoLevel.Location = new Point(34, 241);
			ledFifoLevel.Name = "ledFifoLevel";
			ledFifoLevel.Size = new Size(15, 15);
			ledFifoLevel.TabIndex = 20;
			ledFifoLevel.Text = "led1";
			ledTimeout.BackColor = Color.Transparent;
			ledTimeout.LedColor = Color.Green;
			ledTimeout.LedSize = new Size(11, 11);
			ledTimeout.Location = new Point(34, 130);
			ledTimeout.Name = "ledTimeout";
			ledTimeout.Size = new Size(15, 15);
			ledTimeout.TabIndex = 10;
			ledTimeout.Text = "led1";
			label27.AutoSize = true;
			label27.Location = new Point(55, 200);
			label27.Name = "label27";
			label27.Size = new Size(40, 13);
			label27.TabIndex = 17;
			label27.Text = "FifoFull";
			label20.AutoSize = true;
			label20.Location = new Point(55, 173);
			label20.Name = "label20";
			label20.Size = new Size(99, 13);
			label20.TabIndex = 15;
			label20.Text = "SyncAddressMatch";
			ledFifoEmpty.BackColor = Color.Transparent;
			ledFifoEmpty.LedColor = Color.Green;
			ledFifoEmpty.LedSize = new Size(11, 11);
			ledFifoEmpty.Location = new Point(34, 220);
			ledFifoEmpty.Name = "ledFifoEmpty";
			ledFifoEmpty.Size = new Size(15, 15);
			ledFifoEmpty.TabIndex = 18;
			ledFifoEmpty.Text = "led1";
			label21.AutoSize = true;
			label21.Location = new Point(55, 152);
			label21.Name = "label21";
			label21.Size = new Size(51, 13);
			label21.TabIndex = 13;
			label21.Text = "Preamble";
			label26.AutoSize = true;
			label26.Location = new Point(55, 221);
			label26.Name = "label26";
			label26.Size = new Size(53, 13);
			label26.TabIndex = 19;
			label26.Text = "FifoEmpty";
			ledPreamble.BackColor = Color.Transparent;
			ledPreamble.LedColor = Color.Green;
			ledPreamble.LedSize = new Size(11, 11);
			ledPreamble.Location = new Point(34, 151);
			ledPreamble.Name = "ledPreamble";
			ledPreamble.Size = new Size(15, 15);
			ledPreamble.TabIndex = 12;
			ledPreamble.Text = "led1";
			ledPreamble.Click += ledPreamble_Click;
			label25.AutoSize = true;
			label25.Location = new Point(55, 242);
			label25.Name = "label25";
			label25.Size = new Size(50, 13);
			label25.TabIndex = 21;
			label25.Text = "FifoLevel";
			label22.AutoSize = true;
			label22.Location = new Point(55, 131);
			label22.Name = "label22";
			label22.Size = new Size(45, 13);
			label22.TabIndex = 11;
			label22.Text = "Timeout";
			label24.AutoSize = true;
			label24.Location = new Point(55, 263);
			label24.Name = "label24";
			label24.Size = new Size(62, 13);
			label24.TabIndex = 23;
			label24.Text = "FifoOverrun";
			label23.AutoSize = true;
			label23.Location = new Point(55, 110);
			label23.Name = "label23";
			label23.Size = new Size(27, 13);
			label23.TabIndex = 9;
			label23.Text = "Rssi";
			ledFifoFull.BackColor = Color.Transparent;
			ledFifoFull.LedColor = Color.Green;
			ledFifoFull.LedSize = new Size(11, 11);
			ledFifoFull.Location = new Point(34, 199);
			ledFifoFull.Margin = new Padding(3, 6, 3, 3);
			ledFifoFull.Name = "ledFifoFull";
			ledFifoFull.Size = new Size(15, 15);
			ledFifoFull.TabIndex = 16;
			ledFifoFull.Text = "led1";
			ledSyncAddressMatch.BackColor = Color.Transparent;
			ledSyncAddressMatch.LedColor = Color.Green;
			ledSyncAddressMatch.LedSize = new Size(11, 11);
			ledSyncAddressMatch.Location = new Point(34, 172);
			ledSyncAddressMatch.Margin = new Padding(3, 3, 3, 6);
			ledSyncAddressMatch.Name = "ledSyncAddressMatch";
			ledSyncAddressMatch.Size = new Size(15, 15);
			ledSyncAddressMatch.TabIndex = 14;
			ledSyncAddressMatch.Text = "led1";
			ledSyncAddressMatch.Click += ledSyncAddressMatch_Click;
			ledFifoNotEmpty.BackColor = Color.Transparent;
			ledFifoNotEmpty.LedColor = Color.Green;
			ledFifoNotEmpty.LedSize = new Size(11, 11);
			ledFifoNotEmpty.Location = new Point(34, 220);
			ledFifoNotEmpty.Name = "ledFifoNotEmpty";
			ledFifoNotEmpty.Size = new Size(15, 15);
			ledFifoNotEmpty.TabIndex = 18;
			ledFifoNotEmpty.Text = "led1";
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(gBoxOperatingMode);
			Controls.Add(tabControl1);
			Controls.Add(gBoxIrqFlags);
			Name = "DeviceViewControl";
			Size = new Size(1008, 525);
			tabControl1.ResumeLayout(false);
			tabCommon.ResumeLayout(false);
			tabTransmitter.ResumeLayout(false);
			tabReceiver.ResumeLayout(false);
			tabIrqMap.ResumeLayout(false);
			tabPacketHandler.ResumeLayout(false);
			tabSequencer.ResumeLayout(false);
			tabTemperature.ResumeLayout(false);
			gBoxOperatingMode.ResumeLayout(false);
			gBoxOperatingMode.PerformLayout();
			gBoxIrqFlags.ResumeLayout(false);
			gBoxIrqFlags.PerformLayout();
			ResumeLayout(false);
		}

		public DeviceViewControl()
		{
			InitializeComponent();
		}

		private void LoadTestPage(SX1276 deviceN)
		{
			try
			{
				if (File.Exists(Application.StartupPath + "\\SemtechLib.Devices.SX1276.Test.dll"))
				{
					var assembly = Assembly.Load(Application.StartupPath + "\\SemtechLib.Devices.SX1276.Test.dll");
					var type = assembly.GetType("SemtechLib.Devices.SX1276.Test.Controls.TestTabPage");
					var obj = Activator.CreateInstance(type);
					type.InvokeMember("SuspendLayout", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null, obj, null);
					SuspendLayout();
					var property = type.GetProperty("Location");
					property.SetValue(obj, new Point(4, 22), null);
					property = type.GetProperty("Name");
					property.SetValue(obj, "tabTest", null);
					property = type.GetProperty("Size");
					property.SetValue(obj, new Size(799, 493), null);
					property = type.GetProperty("TabIndex");
					property.SetValue(obj, 6, null);
					property = type.GetProperty("Text");
					property.SetValue(obj, "R&D Tests", null);
					property = type.GetProperty("UseVisualStyleBackColor");
					property.SetValue(obj, true, null);
					property = type.GetProperty("SX1276");
					property.SetValue(obj, deviceN, null);
					tabControl1.Controls.Add((Control)obj);
					type.InvokeMember("ResumeLayout", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null, obj, [false]);
					ResumeLayout(performLayout: false);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
			case "FrequencyXo":
				commonViewControl1.FrequencyXo = device.FrequencyXo;
				receiverViewControl1.FrequencyXo = device.FrequencyXo;
				irqMapViewControl1.FrequencyXo = device.FrequencyXo;
				break;
			case "FrequencyStep":
				commonViewControl1.FrequencyStep = device.FrequencyStep;
				break;
			case "SpectrumOn":
				if (device.SpectrumOn)
				{
					DisableControls();
					packetHandlerView1.Enabled = false;
				}
				else
				{
					EnableControls();
					packetHandlerView1.Enabled = true;
				}
				break;
			case "ModulationType":
				commonViewControl1.ModulationType = device.ModulationType;
				receiverViewControl1.ModulationType = device.ModulationType;
				break;
			case "ModulationShaping":
				commonViewControl1.ModulationShaping = device.ModulationShaping;
				break;
			case "Mode":
				rBtnSleep.CheckedChanged -= rBtnOperatingMode_CheckedChanged;
				rBtnStandby.CheckedChanged -= rBtnOperatingMode_CheckedChanged;
				rBtnSynthesizerRx.CheckedChanged -= rBtnOperatingMode_CheckedChanged;
				rBtnSynthesizerTx.CheckedChanged -= rBtnOperatingMode_CheckedChanged;
				rBtnReceiver.CheckedChanged -= rBtnOperatingMode_CheckedChanged;
				rBtnTransmitter.CheckedChanged -= rBtnOperatingMode_CheckedChanged;
				switch (device.Mode)
				{
				case OperatingModeEnum.Sleep:
					rBtnSleep.Checked = true;
					break;
				case OperatingModeEnum.Stdby:
					rBtnStandby.Checked = true;
					break;
				case OperatingModeEnum.FsRx:
					rBtnSynthesizerRx.Checked = true;
					break;
				case OperatingModeEnum.FsTx:
					rBtnSynthesizerTx.Checked = true;
					break;
				case OperatingModeEnum.Rx:
					rBtnReceiver.Checked = true;
					break;
				case OperatingModeEnum.Tx:
					rBtnTransmitter.Checked = true;
					break;
				}
				rBtnSleep.CheckedChanged += rBtnOperatingMode_CheckedChanged;
				rBtnStandby.CheckedChanged += rBtnOperatingMode_CheckedChanged;
				rBtnSynthesizerRx.CheckedChanged += rBtnOperatingMode_CheckedChanged;
				rBtnSynthesizerTx.CheckedChanged += rBtnOperatingMode_CheckedChanged;
				rBtnReceiver.CheckedChanged += rBtnOperatingMode_CheckedChanged;
				rBtnTransmitter.CheckedChanged += rBtnOperatingMode_CheckedChanged;
				irqMapViewControl1.Mode = device.Mode;
				packetHandlerView1.Mode = device.Mode;
				temperatureViewControl1.Mode = device.Mode;
				break;
			case "Bitrate":
				commonViewControl1.Bitrate = device.Bitrate;
				receiverViewControl1.Bitrate = device.Bitrate;
				packetHandlerView1.Bitrate = device.Bitrate;
				break;
			case "BitrateFrac":
				commonViewControl1.BitrateFrac = device.BitrateFrac;
				break;
			case "Fdev":
				commonViewControl1.Fdev = device.Fdev;
				break;
			case "Band":
				commonViewControl1.Band = device.Band;
				break;
			case "LowFrequencyModeOn":
				commonViewControl1.LowFrequencyModeOn = device.LowFrequencyModeOn;
				receiverViewControl1.LowFrequencyModeOn = device.LowFrequencyModeOn;
				break;
			case "FrequencyRf":
				commonViewControl1.FrequencyRf = device.FrequencyRf;
				break;
			case "PaSelect":
				transmitterViewControl1.PaSelect = device.PaSelect;
				break;
			case "MaxOutputPower":
				transmitterViewControl1.MaxOutputPower = device.MaxOutputPower;
				break;
			case "OutputPower":
				transmitterViewControl1.OutputPower = device.OutputPower;
				break;
			case "ForceTxBandLowFrequencyOn":
				commonViewControl1.ForceTxBandLowFrequencyOn = device.ForceTxBandLowFrequencyOn;
				break;
			case "PaRamp":
				transmitterViewControl1.PaRamp = device.PaRamp;
				break;
			case "OcpOn":
				transmitterViewControl1.OcpOn = device.OcpOn;
				break;
			case "OcpTrim":
				transmitterViewControl1.OcpTrim = device.OcpTrim;
				break;
			case "Pa20dBm":
				transmitterViewControl1.Pa20dBm = device.Pa20dBm;
				break;
			case "RxBwMin":
				receiverViewControl1.RxBwMin = device.RxBwMin;
				break;
			case "RxBwMax":
				receiverViewControl1.RxBwMax = device.RxBwMax;
				break;
			case "AfcRxBwMin":
				receiverViewControl1.AfcRxBwMin = device.AfcRxBwMin;
				break;
			case "AfcRxBwMax":
				receiverViewControl1.AfcRxBwMax = device.AfcRxBwMax;
				break;
			case "LnaGain":
				receiverViewControl1.LnaGain = device.LnaGain;
				break;
			case "ForceRxBandLowFrequencyOn":
				commonViewControl1.ForceRxBandLowFrequencyOn = device.ForceRxBandLowFrequencyOn;
				break;
			case "LnaBoost":
				receiverViewControl1.LnaBoost = device.LnaBoost;
				break;
			case "RestartRxOnCollision":
				receiverViewControl1.RestartRxOnCollision = device.RestartRxOnCollision;
				break;
			case "AfcAutoOn":
				receiverViewControl1.AfcAutoOn = device.AfcAutoOn;
				break;
			case "AgcAutoOn":
				receiverViewControl1.AgcAutoOn = device.AgcAutoOn;
				break;
			case "RxTrigger":
				receiverViewControl1.RxTrigger = device.RxTrigger;
				break;
			case "RssiOffset":
				receiverViewControl1.RssiOffset = device.RssiOffset;
				break;
			case "RssiSmoothing":
				receiverViewControl1.RssiSmoothing = device.RssiSmoothing;
				break;
			case "RssiCollisionThreshold":
				receiverViewControl1.RssiCollisionThreshold = device.RssiCollisionThreshold;
				break;
			case "RssiThreshold":
				receiverViewControl1.RssiThreshold = device.RssiThreshold;
				break;
			case "RssiValue":
				receiverViewControl1.RssiValue = device.RssiValue;
				break;
			case "RxBw":
				receiverViewControl1.RxBw = device.RxBw;
				break;
			case "AfcRxBw":
				receiverViewControl1.AfcRxBw = device.AfcRxBw;
				break;
			case "BitSyncOn":
				receiverViewControl1.BitSyncOn = device.BitSyncOn;
				irqMapViewControl1.BitSyncOn = device.BitSyncOn;
				packetHandlerView1.BitSyncOn = device.BitSyncOn;
				break;
			case "OokThreshType":
				receiverViewControl1.OokThreshType = device.OokThreshType;
				break;
			case "OokPeakThreshStep":
				receiverViewControl1.OokPeakThreshStep = device.OokPeakThreshStep;
				break;
			case "OokFixedThreshold":
				receiverViewControl1.OokFixedThreshold = device.OokFixedThreshold;
				break;
			case "OokPeakThreshDec":
				receiverViewControl1.OokPeakThreshDec = device.OokPeakThreshDec;
				break;
			case "OokAverageOffset":
				receiverViewControl1.OokAverageOffset = device.OokAverageOffset;
				break;
			case "OokAverageThreshFilt":
				receiverViewControl1.OokAverageThreshFilt = device.OokAverageThreshFilt;
				break;
			case "AfcAutoClearOn":
				receiverViewControl1.AfcAutoClearOn = device.AfcAutoClearOn;
				break;
			case "AfcValue":
				receiverViewControl1.AfcValue = device.AfcValue;
				break;
			case "FeiValue":
				receiverViewControl1.FeiValue = device.FeiValue;
				break;
			case "PreambleDetectorOn":
				receiverViewControl1.PreambleDetectorOn = device.PreambleDetectorOn;
				break;
			case "PreambleDetectorSize":
				receiverViewControl1.PreambleDetectorSize = device.PreambleDetectorSize;
				break;
			case "PreambleDetectorTol":
				receiverViewControl1.PreambleDetectorTol = device.PreambleDetectorTol;
				break;
			case "TimeoutRxRssi":
				receiverViewControl1.TimeoutRxRssi = device.TimeoutRxRssi;
				break;
			case "TimeoutRxPreamble":
				receiverViewControl1.TimeoutRxPreamble = device.TimeoutRxPreamble;
				break;
			case "TimeoutSignalSync":
				receiverViewControl1.TimeoutSignalSync = device.TimeoutSignalSync;
				break;
			case "InterPacketRxDelay":
				receiverViewControl1.InterPacketRxDelay = device.InterPacketRxDelay;
				break;
			case "ClockOut":
				irqMapViewControl1.ClockOut = device.ClockOut;
				break;
			case "Packet":
				packetHandlerView1.DataMode = device.Packet.DataMode;
				packetHandlerView1.PreambleSize = device.Packet.PreambleSize;
				packetHandlerView1.AutoRestartRxOn = device.Packet.AutoRestartRxOn;
				packetHandlerView1.PreamblePolarity = device.Packet.PreamblePolarity;
				packetHandlerView1.SyncOn = device.Packet.SyncOn;
				packetHandlerView1.FifoFillCondition = device.Packet.FifoFillCondition;
				packetHandlerView1.SyncSize = device.Packet.SyncSize;
				packetHandlerView1.SyncValue = device.Packet.SyncValue;
				packetHandlerView1.PacketFormat = device.Packet.PacketFormat;
				packetHandlerView1.DcFree = device.Packet.DcFree;
				packetHandlerView1.CrcOn = device.Packet.CrcOn;
				packetHandlerView1.CrcAutoClearOff = device.Packet.CrcAutoClearOff;
				packetHandlerView1.AddressFiltering = device.Packet.AddressFiltering;
				packetHandlerView1.CrcIbmOn = device.Packet.CrcIbmOn;
				packetHandlerView1.IoHomeOn = device.Packet.IoHomeOn;
				packetHandlerView1.IoHomePwrFrameOn = device.Packet.IoHomePwrFrameOn;
				packetHandlerView1.BeaconOn = device.Packet.BeaconOn;
				packetHandlerView1.PayloadLength = device.Packet.PayloadLength;
				packetHandlerView1.NodeAddress = device.Packet.NodeAddress;
				packetHandlerView1.BroadcastAddress = device.Packet.BroadcastAddress;
				packetHandlerView1.TxStartCondition = device.Packet.TxStartCondition;
				packetHandlerView1.FifoThreshold = device.Packet.FifoThreshold;
				packetHandlerView1.MessageLength = device.Packet.MessageLength;
				packetHandlerView1.Message = device.Packet.Message;
				packetHandlerView1.Crc = device.Packet.Crc;
				break;
			case "PreambleSize":
				packetHandlerView1.PreambleSize = device.Packet.PreambleSize;
				break;
			case "AutoRestartRxOn":
				packetHandlerView1.AutoRestartRxOn = device.Packet.AutoRestartRxOn;
				break;
			case "PreamblePolarity":
				packetHandlerView1.PreamblePolarity = device.Packet.PreamblePolarity;
				break;
			case "SyncOn":
				packetHandlerView1.SyncOn = device.Packet.SyncOn;
				break;
			case "FifoFillCondition":
				packetHandlerView1.FifoFillCondition = device.Packet.FifoFillCondition;
				break;
			case "SyncSize":
				packetHandlerView1.SyncSize = device.Packet.SyncSize;
				break;
			case "SyncValue":
				packetHandlerView1.SyncValue = device.Packet.SyncValue;
				break;
			case "PacketFormat":
				packetHandlerView1.PacketFormat = device.Packet.PacketFormat;
				break;
			case "DcFree":
				packetHandlerView1.DcFree = device.Packet.DcFree;
				break;
			case "CrcOn":
				packetHandlerView1.CrcOn = device.Packet.CrcOn;
				break;
			case "CrcAutoClearOff":
				packetHandlerView1.CrcAutoClearOff = device.Packet.CrcAutoClearOff;
				break;
			case "AddressFiltering":
				packetHandlerView1.AddressFiltering = device.Packet.AddressFiltering;
				break;
			case "CrcIbmOn":
				packetHandlerView1.CrcIbmOn = device.Packet.CrcIbmOn;
				break;
			case "DataMode":
				packetHandlerView1.DataMode = device.Packet.DataMode;
				receiverViewControl1.DataMode = device.Packet.DataMode;
				irqMapViewControl1.DataMode = device.Packet.DataMode;
				break;
			case "IoHomeOn":
				packetHandlerView1.IoHomeOn = device.Packet.IoHomeOn;
				break;
			case "IoHomePwrFrameOn":
				packetHandlerView1.IoHomePwrFrameOn = device.Packet.IoHomePwrFrameOn;
				break;
			case "BeaconOn":
				packetHandlerView1.BeaconOn = device.Packet.BeaconOn;
				break;
			case "PayloadLength":
				packetHandlerView1.PayloadLength = device.Packet.PayloadLength;
				break;
			case "NodeAddress":
				packetHandlerView1.NodeAddress = device.Packet.NodeAddress;
				break;
			case "NodeAddressRx":
				packetHandlerView1.NodeAddressRx = device.Packet.NodeAddressRx;
				break;
			case "BroadcastAddress":
				packetHandlerView1.BroadcastAddress = device.Packet.BroadcastAddress;
				break;
			case "TxStartCondition":
				packetHandlerView1.TxStartCondition = device.Packet.TxStartCondition;
				break;
			case "FifoThreshold":
				packetHandlerView1.FifoThreshold = device.Packet.FifoThreshold;
				break;
			case "MessageLength":
				packetHandlerView1.MessageLength = device.Packet.MessageLength;
				break;
			case "Message":
				packetHandlerView1.Message = device.Packet.Message;
				break;
			case "Crc":
				packetHandlerView1.Crc = device.Packet.Crc;
				break;
			case "LogEnabled":
				packetHandlerView1.LogEnabled = device.Packet.LogEnabled;
				break;
			case "IdleMode":
				sequencerViewControl1.IdleMode = device.IdleMode;
				break;
			case "FromStart":
				sequencerViewControl1.FromStart = device.FromStart;
				break;
			case "LowPowerSelection":
				sequencerViewControl1.LowPowerSelection = device.LowPowerSelection;
				break;
			case "FromIdle":
				sequencerViewControl1.FromIdle = device.FromIdle;
				break;
			case "FromTransmit":
				sequencerViewControl1.FromTransmit = device.FromTransmit;
				break;
			case "FromReceive":
				sequencerViewControl1.FromReceive = device.FromReceive;
				break;
			case "FromRxTimeout":
				sequencerViewControl1.FromRxTimeout = device.FromRxTimeout;
				break;
			case "FromPacketReceived":
				sequencerViewControl1.FromPacketReceived = device.FromPacketReceived;
				break;
			case "Timer1Resolution":
				sequencerViewControl1.Timer1Resolution = device.Timer1Resolution;
				break;
			case "Timer2Resolution":
				sequencerViewControl1.Timer2Resolution = device.Timer2Resolution;
				break;
			case "Timer1Coef":
				sequencerViewControl1.Timer1Coef = device.Timer1Coef;
				break;
			case "Timer2Coef":
				sequencerViewControl1.Timer2Coef = device.Timer2Coef;
				break;
			case "AutoImageCalOn":
				temperatureViewControl1.AutoImageCalOn = device.AutoImageCalOn;
				break;
			case "ImageCalRunning":
				temperatureViewControl1.ImageCalRunning = device.ImageCalRunning;
				break;
			case "TempChange":
				temperatureViewControl1.TempChange = device.TempChange;
				break;
			case "TempThreshold":
				temperatureViewControl1.TempThreshold = device.TempThreshold;
				break;
			case "TempMonitorOff":
				temperatureViewControl1.TempMonitorOff = device.TempMonitorOff;
				break;
			case "TempValue":
				temperatureViewControl1.TempValue = device.TempValue;
				break;
			case "TempValueRoom":
				temperatureViewControl1.TempValueRoom = device.TempValueRoom;
				break;
			case "TempCalDone":
				temperatureViewControl1.TempCalDone = device.TempCalDone;
				break;
			case "TempMeasRunning":
				temperatureViewControl1.TempMeasRunning = device.TempMeasRunning;
				break;
			case "LowBatOn":
				commonViewControl1.LowBatOn = device.LowBatOn;
				break;
			case "LowBatTrim":
				commonViewControl1.LowBatTrim = device.LowBatTrim;
				break;
			case "ModeReady":
				ledModeReady.Checked = device.ModeReady;
				break;
			case "RxReady":
				ledRxReady.Checked = device.RxReady;
				break;
			case "TxReady":
				ledTxReady.Checked = device.TxReady;
				break;
			case "PllLock":
				ledPllLock.Checked = device.PllLock;
				break;
			case "Rssi":
				ledRssi.Checked = device.Rssi;
				break;
			case "Timeout":
				ledTimeout.Checked = device.Timeout;
				break;
			case "PreambleDetect":
				ledPreamble.Checked = device.PreambleDetect;
				break;
			case "SyncAddressMatch":
				ledSyncAddressMatch.Checked = device.SyncAddressMatch;
				break;
			case "FifoFull":
				ledFifoFull.Checked = device.FifoFull;
				break;
			case "FifoEmpty":
				ledFifoEmpty.Checked = device.FifoEmpty;
				break;
			case "FifoLevel":
				ledFifoLevel.Checked = device.FifoLevel;
				break;
			case "FifoOverrun":
				ledFifoOverrun.Checked = device.FifoOverrun;
				break;
			case "PacketSent":
				ledPacketSent.Checked = device.PacketSent;
				break;
			case "PayloadReady":
				ledPayloadReady.Checked = device.PayloadReady;
				break;
			case "CrcOk":
				ledCrcOk.Checked = device.CrcOk;
				break;
			case "LowBat":
				ledLowBat.Checked = device.LowBat;
				break;
			case "Dio0Mapping":
				irqMapViewControl1.Dio0Mapping = device.Dio0Mapping;
				break;
			case "Dio1Mapping":
				irqMapViewControl1.Dio1Mapping = device.Dio1Mapping;
				break;
			case "Dio2Mapping":
				irqMapViewControl1.Dio2Mapping = device.Dio2Mapping;
				break;
			case "Dio3Mapping":
				irqMapViewControl1.Dio3Mapping = device.Dio3Mapping;
				break;
			case "Dio4Mapping":
				irqMapViewControl1.Dio4Mapping = device.Dio4Mapping;
				break;
			case "Dio5Mapping":
				irqMapViewControl1.Dio5Mapping = device.Dio5Mapping;
				break;
			case "MapPreambleDetect":
				irqMapViewControl1.MapPreambleDetect = device.MapPreambleDetect;
				break;
			case "AgcReference":
				receiverViewControl1.AgcReference = device.AgcReference;
				break;
			case "AgcThresh1":
				receiverViewControl1.AgcThresh1 = device.AgcThresh1;
				break;
			case "AgcThresh2":
				receiverViewControl1.AgcThresh2 = device.AgcThresh2;
				break;
			case "AgcThresh3":
				receiverViewControl1.AgcThresh3 = device.AgcThresh3;
				break;
			case "AgcThresh4":
				receiverViewControl1.AgcThresh4 = device.AgcThresh4;
				break;
			case "AgcThresh5":
				receiverViewControl1.AgcThresh5 = device.AgcThresh5;
				break;
			case "AgcReferenceLevel":
				receiverViewControl1.AgcReferenceLevel = device.AgcReferenceLevel;
				break;
			case "AgcStep1":
				receiverViewControl1.AgcStep1 = device.AgcStep1;
				break;
			case "AgcStep2":
				receiverViewControl1.AgcStep2 = device.AgcStep2;
				break;
			case "AgcStep3":
				receiverViewControl1.AgcStep3 = device.AgcStep3;
				break;
			case "AgcStep4":
				receiverViewControl1.AgcStep4 = device.AgcStep4;
				break;
			case "AgcStep5":
				receiverViewControl1.AgcStep5 = device.AgcStep5;
				break;
			case "FastHopOn":
				commonViewControl1.FastHopOn = device.FastHopOn;
				break;
			case "TcxoInputOn":
				commonViewControl1.TcxoInputOn = device.TcxoInputOn;
				break;
			case "PllBandwidth":
				transmitterViewControl1.PllBandwidth = device.PllBandwidth;
				break;
			case "TempDelta":
				temperatureViewControl1.TempDelta = device.TempDelta;
				break;
			case "PngEnabled":
				if (device.PngEnabled)
				{
					DisableControls();
					packetHandlerView1.Enabled = false;
				}
				else
				{
					EnableControls();
					packetHandlerView1.Enabled = true;
				}
				break;
			case "Version":
			case "PngSequence":
				break;
			}
		}

		private void OnDevicePacketHandlerStarted(object sender, EventArgs e)
		{
			DisableControls();
		}

		private void OnDevicePacketHandlerStoped(object sender, EventArgs e)
		{
			EnableControls();
			packetHandlerView1.StartStop = false;
		}

		private void OnDevicePacketHandlerTransmitted(object sender, PacketStatusEventArg e)
		{
			packetHandlerView1.PacketNumber = e.Number;
		}

		private void OnDevicePacketHandlerReceived(object sender, PacketStatusEventArg e)
		{
			packetHandlerView1.PacketNumber = e.Number;
		}

		private void OnError(byte status, string message)
		{
            Error?.Invoke(this, new SemtechLib.General.Events.ErrorEventArgs(status, message));
        }

		public new void Dispose()
		{
			base.Dispose();
		}

		private void DisableControls()
		{
			commonViewControl1.Enabled = false;
			transmitterViewControl1.Enabled = false;
			receiverViewControl1.Enabled = false;
			irqMapViewControl1.Enabled = false;
			temperatureViewControl1.Enabled = false;
			gBoxOperatingMode.Enabled = false;
		}

		private void EnableControls()
		{
			commonViewControl1.Enabled = true;
			transmitterViewControl1.Enabled = true;
			receiverViewControl1.Enabled = true;
			irqMapViewControl1.Enabled = true;
			temperatureViewControl1.Enabled = true;
			gBoxOperatingMode.Enabled = true;
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new DevicePropertyChangedDelegate(OnDevicePropertyChanged), sender, e);
			}
			else
			{
				OnDevicePropertyChanged(sender, e);
			}
		}

		private void device_OcpTrimLimitStatusChanged(object sender, LimitCheckStatusEventArg e)
		{
			transmitterViewControl1.UpdateOcpTrimLimits(e.Status, e.Message);
		}

		private void device_BitrateLimitStatusChanged(object sender, LimitCheckStatusEventArg e)
		{
			commonViewControl1.UpdateBitrateLimits(e.Status, e.Message);
		}

		private void device_FdevLimitStatusChanged(object sender, LimitCheckStatusEventArg e)
		{
			commonViewControl1.UpdateFdevLimits(e.Status, e.Message);
			receiverViewControl1.UpdateRxBwLimits(e.Status, e.Message);
		}

		private void device_FrequencyRfLimitStatusChanged(object sender, LimitCheckStatusEventArg e)
		{
			commonViewControl1.UpdateFrequencyRfLimits(e.Status, e.Message);
		}

		private void device_SyncValueLimitChanged(object sender, LimitCheckStatusEventArg e)
		{
			packetHandlerView1.UpdateSyncValueLimits(e.Status, e.Message);
		}

		private void device_PacketHandlerStarted(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new DevicePacketHandlerStartedDelegate(OnDevicePacketHandlerStarted), sender, e);
			}
			else
			{
				OnDevicePacketHandlerStarted(sender, e);
			}
		}

		private void device_PacketHandlerStoped(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new DevicePacketHandlerStopedDelegate(OnDevicePacketHandlerStoped), sender, e);
			}
			else
			{
				OnDevicePacketHandlerStoped(sender, e);
			}
		}

		private void device_PacketHandlerTransmitted(object sender, PacketStatusEventArg e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new DevicePacketHandlerTransmittedDelegate(OnDevicePacketHandlerTransmitted), sender, e);
			}
			else
			{
				OnDevicePacketHandlerTransmitted(sender, e);
			}
		}

		private void device_PacketHandlerReceived(object sender, PacketStatusEventArg e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new DevicePacketHandlerTransmittedDelegate(OnDevicePacketHandlerTransmitted), sender, e);
			}
			else
			{
				OnDevicePacketHandlerReceived(sender, e);
			}
		}

		private void commonViewControl1_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			OnDocumentationChanged(e);
		}

		private void commonViewControl1_FrequencyXoChanged(object sender, DecimalEventArg e)
		{
			device.FrequencyXo = commonViewControl1.FrequencyXo;
		}

		private void rBtnOperatingMode_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				if (rBtnSleep.Checked)
				{
					device.SetOperatingMode(OperatingModeEnum.Sleep);
				}
				else if (rBtnStandby.Checked)
				{
					device.SetOperatingMode(OperatingModeEnum.Stdby);
				}
				else if (rBtnSynthesizerRx.Checked)
				{
					device.SetOperatingMode(OperatingModeEnum.FsRx);
				}
				else if (rBtnSynthesizerTx.Checked)
				{
					device.SetOperatingMode(OperatingModeEnum.FsTx);
				}
				else if (rBtnReceiver.Checked)
				{
					device.SetOperatingMode(OperatingModeEnum.Rx);
				}
				else if (rBtnTransmitter.Checked)
				{
					device.SetOperatingMode(OperatingModeEnum.Tx);
				}
				irqMapViewControl1.Mode = device.Mode;
				packetHandlerView1.Mode = device.Mode;
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_ModulationTypeChanged(object sender, ModulationTypeEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetModulationType(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_ModulationShapingChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetModulationShaping(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_BitrateChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetBitrate(e.Value);
				receiverViewControl1.Bitrate = device.Bitrate;
				packetHandlerView1.Bitrate = device.Bitrate;
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_BitrateFracChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetBitrateFrac(e.Value);
				receiverViewControl1.Bitrate = device.Bitrate;
				packetHandlerView1.Bitrate = device.Bitrate;
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_FdevChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFdev(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_BandChanged(object sender, BandEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetBand(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_LowFrequencyModeOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetLowFrequencyModeOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_FrequencyRfChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFrequencyRf(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_FastHopOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFastHopOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_TcxoInputChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetTcxoInputOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_RcCalibrationChanged(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.RcCalTrig();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_LowBatOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetLowBatOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_LowBatTrimChanged(object sender, LowBatTrimEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetLowBatTrim(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void transmitterViewControl1_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			OnDocumentationChanged(e);
		}

		private void transmitterViewControl1_PaModeChanged(object sender, PaModeEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPaMode(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void transmitterViewControl1_MaxOutputPowerChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetMaxOutputPower(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void transmitterViewControl1_OutputPowerChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetOutputPower(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_ForceTxBandLowFrequencyOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetForceTxBandLowFrequencyOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void transmitterViewControl1_PaRampChanged(object sender, PaRampEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPaRamp(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void transmitterViewControl1_OcpOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetOcpOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void transmitterViewControl1_OcpTrimChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetOcpTrim(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void transmitterViewControl1_Pa20dBmChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPa20dBm(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void transmitterViewControl1_PllBandwidthChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPllBandwidth(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			OnDocumentationChanged(e);
		}

		private void receiverViewControl1_AgcReferenceLevelChanged(object sender, Int32EventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetAgcReferenceLevel(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_AgcStepChanged(object sender, AgcStepEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetAgcStep(e.Id, e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_LnaGainChanged(object sender, LnaGainEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetLnaGain(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_ForceRxBandLowFrequencyOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetForceRxBandLowFrequencyOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_LnaBoostChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetLnaBoost(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RestartRxOnCollisionOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetRestartRxOnCollisionOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RestartRxWithoutPllLockChanged(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetRestartRxWithoutPllLock();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RestartRxWithPllLockChanged(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetRestartRxWithPllLock();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_AfcAutoOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetAfcAutoOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_AgcAutoOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetAgcAutoOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RxTriggerChanged(object sender, RxTriggerEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetRxTrigger(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RssiOffsetChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetRssiOffset(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RssiSmoothingChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetRssiSmoothing(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RssiCollisionThresholdChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetRssiCollisionThreshold(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RssiThreshChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetRssiThresh(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_DccFastInitOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetDccFastInitOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_DccForceOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetDccForceOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RxBwChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetRxBw(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_AfcRxBwChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetAfcRxBw(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_BitSyncOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetBitSyncOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_OokThreshTypeChanged(object sender, OokThreshTypeEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetOokThreshType(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_OokPeakThreshStepChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetOokPeakThreshStep(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_OokPeakThreshDecChanged(object sender, OokPeakThreshDecEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetOokPeakThreshDec(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_OokAverageThreshFiltChanged(object sender, OokAverageThreshFiltEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetOokAverageThreshFilt(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_OokPeakRecoveryOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetOokPeakRecoveryOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_OokAverageBiasChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetOokAverageBias(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_OokFixedThreshChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetOokFixedThresh(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_BarkerSyncThreshChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetBarkerSyncThresh(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_BarkerSyncLossThreshChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetBarkerSyncLossThresh(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_BarkerTrackingThreshChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetBarkerTrackingThresh(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_AgcStartChanged(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetAgcStart();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_FeiRangeChanged(object sender, FeiRangeEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFeiRange(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_FeiReadChanged(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFeiRead();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_AfcAutoClearOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetAfcAutoClearOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_AfcClearChanged(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetAfcClear();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_PreambleDetectorOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPreambleDetectorOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_PreambleDetectorSizeChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPreambleDetectorSize(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_PreambleDetectorTolChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPreambleDetectorTol(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_TimeoutRssiChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetTimeoutRssi(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_TimeoutPreambleChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetTimeoutPreamble(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_TimeoutSyncWordChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetTimeoutSyncWord(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_AutoRxRestartDelayChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetAutoRxRestartDelay(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void irqMapViewControl1_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			OnDocumentationChanged(e);
		}

		private void irqMapViewControl1_DioMappingChanged(object sender, DioMappingEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetDioMapping(e.Id, e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void irqMapViewControl1_DioPreambleIrqOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetDioPreambleIrqOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void irqMapViewControl1_ClockOutChanged(object sender, ClockOutEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetClockOut(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			OnDocumentationChanged(e);
		}

		private void packetHandlerView1_Error(object sender, SemtechLib.General.Events.ErrorEventArgs e)
		{
			OnError(e.Status, e.Message);
		}

		private void packetHandlerView1_PreambleSizeChanged(object sender, Int32EventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPreambleSize(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_SyncOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetSyncOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_FifoFillConditionChanged(object sender, FifoFillConditionEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFifoFillCondition(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_SyncSizeChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetSyncSize(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_SyncValueChanged(object sender, ByteArrayEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetSyncValue(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_PacketFormatChanged(object sender, PacketFormatEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPacketFormat(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_DcFreeChanged(object sender, DcFreeEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetDcFree(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_CrcOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetCrcOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_CrcAutoClearOffChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetCrcAutoClearOff(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_AddressFilteringChanged(object sender, AddressFilteringEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetAddressFiltering(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_PayloadLengthChanged(object sender, Int16EventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPayloadLength(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_NodeAddressChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetNodeAddress(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_BroadcastAddressChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetBroadcastAddress(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_TxStartConditionChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetTxStartCondition(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_FifoThresholdChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFifoThreshold(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_MessageLengthChanged(object sender, Int32EventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetMessageLength(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_MessageChanged(object sender, ByteArrayEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetMessage(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_StartStopChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPacketHandlerStartStop(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_MaxPacketNumberChanged(object sender, Int32EventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetMaxPacketNumber(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_PacketHandlerLogEnableChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPacketHandlerLogEnable(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			OnDocumentationChanged(e);
		}

		private void sequencerViewControl1_SequencerStartChanged(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetSequencerStart();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_SequencerStopChanged(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetSequencerStop();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_IdleModeChanged(object sender, IdleModeEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetIdleMode(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_FromStartChanged(object sender, FromStartEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFromStart(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_LowPowerSelectionChanged(object sender, LowPowerSelectionEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetLowPowerSelection(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_FromIdleChanged(object sender, FromIdleEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFromIdle(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_FromTransmitChanged(object sender, FromTransmitEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFromTransmit(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_FromReceiveChanged(object sender, FromReceiveEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFromReceive(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_FromRxTimeoutChanged(object sender, FromRxTimeoutEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFromRxTimeout(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_FromPacketReceivedChanged(object sender, FromPacketReceivedEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFromPacketReceived(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_Timer1ResolutionChanged(object sender, TimerResolutionEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetTimer1Resolution(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_Timer2ResolutionChanged(object sender, TimerResolutionEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetTimer2Resolution(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_Timer1CoefChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetTimer1Coef(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_Timer2CoefChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetTimer2Coef(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void temperatureViewControl1_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			OnDocumentationChanged(e);
		}

		private void temperatureViewControl1_RxCalAutoOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetRxCalAutoOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void temperatureViewControl1_RxCalibrationChanged(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.ImageCalStart();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void temperatureViewControl1_TempMeasOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetTempMonitorOff(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void temperatureViewControl1_TempThresholdChanged(object sender, TempThresholdEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetTempThreshold(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void temperatureViewControl1_TempCalibrateChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetTempCalibrate(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
			if (sender == gBoxIrqFlags)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("IrqMap", "Irq flags"));
			}
			else if (sender == gBoxOperatingMode)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "Operating mode"));
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

		private void packetHandlerView1_DataModeChanged(object sender, DataModeEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetDataMode(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_AutoRestartRxChanged(object sender, AutoRestartRxEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetAutoRestartRxOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_PreamblePolarityChanged(object sender, PreamblePolarityEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPreamblePolarity(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_CrcIbmChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetCrcIbmOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_IoHomeOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetIoHomeOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_IoHomePwrFrameOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetIoHomePwrFrameOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_BeaconOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetBeaconOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_FillFifoChanged(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFillFifo();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void ledRssi_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.ClrRssiIrq();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void ledPreamble_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.ClrPreambleDetectIrq();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void ledSyncAddressMatch_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.ClrSyncAddressMatchIrq();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void ledFifoOverrun_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.ClrFifoOverrunIrq();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void ledLowBat_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.ClrLowBatIrq();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}
	/*
		[SpecialName]
		public bool get_Visible()
		{
			return Visible;
		}

		[SpecialName]
		public void set_Visible(bool P_0)
		{
			Visible = P_0;
		}

		[SpecialName]
		public string get_Name()
		{
			return Name;
		}

		[SpecialName]
		public void set_Name(string P_0)
		{
			Name = P_0;
		}

		[SpecialName]
		public int get_TabIndex()
		{
			return TabIndex;
		}

		[SpecialName]
		public void set_TabIndex(int P_0)
		{
			TabIndex = P_0;
		}
		*/
	}
}
