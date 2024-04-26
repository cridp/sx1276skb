using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SemtechLib.Devices.Common.Events;
using SemtechLib.Devices.Common.Hid;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276LR.Enumerations;
using SemtechLib.Devices.SX1276LR.Events;
using SemtechLib.Devices.SX1276LR.General;
using SemtechLib.General;

namespace SemtechLib.Devices.SX1276LR
{
	public sealed class SX1276LR : IDevice //, INotifyPropertyChanged, IDisposable
	{
		public sealed class IoChangedEventArgs(bool state) : EventArgs
		{
            public bool Sate { get; } = state;
		}
		private const int BR_MIN = 1200;

		private const int BRF_MAX = 250000;

		private const int BRO_MAX = 32768;

		private const int BW_SSB_MAX = 250000;

		private const int FDA_MAX = 200000;

		private const int FDA_MIN = 600;

		private const int FR_BAND_1_MAX = 175000000;

		private const int FR_BAND_1_MIN = 137000000;

		private const int FR_BAND_2_MAX = 525000000;

		private const int FR_BAND_2_MIN = 410000000;

		private const int FR_BAND_3_MAX = 1024000000;

		private const int FR_BAND_3_MIN = 820000000;

		private const byte HID_CMD_ANS_DATA_INDEX = 10;

		private const byte HID_CMD_ANS_DATA_SIZE_INDEX = 9;

		private const byte HID_CMD_ANS_INDEX = 0;

		private const byte HID_CMD_ANS_STAT_INDEX = 0;

		private const byte HID_CMD_ANS_TIME_STAMP_INDEX = 1;

		private const byte HID_CMD_DATA_INDEX = 2;

		private const byte HID_CMD_DATA_SIZE_INDEX = 1;

		private const byte HID_CMD_INDEX = 0;

		private const byte HID_CMD_OPT_INDEX = 0;

		private const int NOISE_ABSOLUTE_ZERO = -174;

		private const int PA_20_DBM_OCP_TRIM_MAX = 240;

		private const int PA_20_DBM_OCP_TRIM_MIN = 150;

		private const int RSSI_OFFSET_HF = -157;

		private const int RSSI_OFFSET_LF = -164;

		private bool _cadDetected;

		private bool _cadDone;

		private string _deviceName;

		private bool _fhssChangeChannel;

		private byte _fifoRxCurrentAddr;

		private bool _headerInfoValid;

		private byte _hopChannel;

		private bool _isPacketHandlerStarted;

		private bool _modemClear;

		private bool _payloadCrcError;

		private decimal _pktRssiValue;

		private sbyte _pktSnrValue;

		private bool _pllTimeout;

		private decimal _rfIoRssiValue = -127.5m;

		private decimal _rfPaRssiValue = -127.5m;

		private decimal _rssiValue;

		private bool _rxDone;

		private byte _rxNbBytes;

		private bool _rxOnGoing;

		private byte _rxPayloadCodingRate = 2;

		private bool _rxPayloadCrcOn;

		private bool _rxTimeout;

		private bool _signalDetected;

		private bool _signalSynchronized;

		private decimal _spectrumRssiValue = -127.5m;

		private bool _test;

		private bool _txDone;

		private bool _validHeader;

		private ushort _validHeaderCnt;

		private ushort _validPacketCnt;

		private bool accessSharedFskReg;

		private bool agcAutoOn;

		private byte agcReferenceLevel = 19;

		private byte agcStep1 = 14;

		private byte agcStep2 = 5;

		private byte agcStep3 = 11;

		private byte agcStep4 = 13;

		private byte agcStep5 = 11;

		private BandEnum band;

		private byte bandwidth = 7;

        private bool bitRateFdevCheckDisbale;

		private bool cadDetectedMask;

		private bool cadDoneMask;

		private byte codingRate = 1;

		private DioMappingEnum dio0Mapping;

		private DioMappingEnum dio1Mapping;

		private DioMappingEnum dio2Mapping;

		private DioMappingEnum dio3Mapping;

		private DioMappingEnum dio4Mapping;

		private DioMappingEnum dio5Mapping;

		private bool fastHopOn;

		private bool fhssChangeChannelMask;

		private byte fifoAddrPtr;

		private byte fifoRxBaseAddr;

		private byte fifoTxBaseAddr;

		private bool firstTransmit;

		private bool forceRxBandLowFrequencyOn;

		private bool forceTxBandLowFrequencyOn;

		private bool frameReceived;

		private bool frameTransmitted;

		private byte freqHoppingPeriod;

		private decimal frequencyRf = 915000000m;

        private bool frequencyRfCheckDisable;

		private decimal frequencyStep = 32000000m / (decimal)Math.Pow(2.0, 19.0);

		private decimal frequencyXo = 32000000m;

		private Version fwVersion = new(0, 0);

		private bool implicitHeaderModeOn;

		private bool isDebugOn;

		private bool isOpen;

		private bool isReceiving;

		private bool lnaBoost;

		private bool lnaBoostPrev = true;

		private LnaGainEnum lnaGain = LnaGainEnum.G1;

		private bool logEnabled;

		private bool lowDatarateOptimize;

		private bool lowFrequencyMode = true;

        private bool lowFrequencyModeOn = true;

		private decimal maxOutputPower = 13.2m;

		private int maxPacketNumber;

		private OperatingModeEnum mode = OperatingModeEnum.Stdby;

		private bool monitor = true;

		private bool ocpOn = true;

		private decimal ocpTrim = 100m;

		private decimal outputPower = 13.2m;

		private bool pa20dBm;

		private byte[] packetBuffer = new byte[256];

        private ILog packetHandlerLog;

		private bool packetModeRxSingle;

		private bool packetModeTx;

        private int packetNumber;

		private bool packetUsePer;

		private PaRampEnum paRamp = PaRampEnum.PaRamp_40;

		private PaSelectEnum paSelect;

		private byte[] payload = [];

		private bool payloadCrcErrorMask;

		private bool payloadCrcOn;

		private byte payloadLength = 14;

		private byte payloadLengthRx = 32;

		private byte payloadMaxLength = 14;

		private List<int> pktLnaValues = [];

        private List<double> pktRssiValues = [];

		private decimal pllBandwidth = 300000m;

		private ushort preambleLength = 12;

		private bool prevAgcAutoOn;

		private LnaGainEnum prevLnaGain;

		private OperatingModeEnum prevMode;

		private bool prevMonitorOn;

		private int prevRfPaSwitchEnabled;

		private RfPaSwitchSelEnum prevRfPaSwitchSel;

		private decimal prevRssiValue = -127.5m;

        private int readLock;

		private RegisterCollection registers;

        private Thread regUpdateThread;

        private bool regUpdateThreadContinue;

        private bool restartRx;

		private int rfPaSwitchEnabled;

		private RfPaSwitchSelEnum rfPaSwitchSel;

		private bool rxDoneMask;

        private bool rxTimeoutMask;

		private int spectrumFreqId;

		private decimal spectrumFreqSpan = 1000000m;

		private bool spectrumOn;

		private int spiSpeed = 2000000;

		private byte spreadingFactor = 7;

		private decimal symbTimeout;

		private bool tcxoInputOn;

		private bool txContinuousModeOn;

		private bool txDoneMask;

		private bool validHeaderMask;

		private Version version = new(0, 0);

        private int writeLock;

		public SX1276LR()
		{
			PropertyChanged += device_PropertyChanged;
			DeviceName = "SX12xxEiger";
			UsbDevice = new HidDevice(1146, 11, DeviceName);
			UsbDevice.Opened += usbDevice_Opened;
			UsbDevice.Closed += usbDevice_Closed;
			Dio0Changed += device_Dio0Changed;
			Dio1Changed += Device_Dio1Changed;
			Dio2Changed += device_Dio2Changed;
			Dio3Changed += device_Dio3Changed;
			Dio4Changed += device_Dio4Changed;
			Dio5Changed += device_Dio5Changed;
			PacketHandlerLog = new PacketLog(this);
			PacketHandlerLog.PropertyChanged += PacketHandlerLog_PropertyChanged;
			PopulateRegisters();
		}

		public delegate void IoChangedEventHandler(object sender, IoChangedEventArgs e);

		public delegate void LimitCheckStatusChangedEventHandler(object sender, LimitCheckStatusEventArg e);

		public event LimitCheckStatusChangedEventHandler BandwidthLimitStatusChanged;

		public event EventHandler Connected;

		public event IoChangedEventHandler Dio0Changed;

		public event IoChangedEventHandler Dio1Changed;

		public event IoChangedEventHandler Dio2Changed;

		public event IoChangedEventHandler Dio3Changed;

		public event IoChangedEventHandler Dio4Changed;

		public event IoChangedEventHandler Dio5Changed;

		public event EventHandler Disconected;

		public event SemtechLib.General.Events.ErrorEventHandler Error;

		public event LimitCheckStatusChangedEventHandler FrequencyRfLimitStatusChanged;

		public event LimitCheckStatusChangedEventHandler OcpTrimLimitStatusChanged;

		public event PacketStatusEventHandler PacketHandlerReceived;

		public event EventHandler PacketHandlerStarted;

		public event EventHandler PacketHandlerStoped;

		public event PacketStatusEventHandler PacketHandlerTransmitted;

		public event PropertyChangedEventHandler PropertyChanged;

		private enum HidCommands
		{
			HID_SK_RESET = 0,
			HID_SK_GET_VERSION = 1,
			HID_SK_GET_NAME = 2,
			HID_SK_GET_ID = 3,
			HID_SK_SET_ID = 4,
			HID_SK_SET_ID_RND = 5,
			HID_SK_FW_UPDATE = 6,
			HID_SK_GET_PIN = 16,
			HID_SK_SET_PIN = 17,
			HID_SK_GET_DIR = 18,
			HID_SK_SET_DIR = 19,
			HID_SK_GET_PINS = 20,
			HID_SK_SM_RESET = 32,
			HID_SK_SM_STEP = 33,
			HID_SK_SM_GET_TIME = 34,
			HID_SK_SM_SET_TIME = 35,
			HID_SK_SM_TRIG_ON_TIME = 36,
			HID_SK_SM_TRIG_ON_PIN = 37,
			HID_EEPROM_READ = 112,
			HID_EEPROM_WRITE = 113,
			HID_DEVICE_READ = 128,
			HID_DEVICE_WRITE = 129,
			HID_DEVICE_GET_COM_INTERFACE = 130,
			HID_DEVICE_SET_COM_INTERFACE = 131,
			HID_DEVICE_GET_COM_INTERFACE_SPEED = 132,
			HID_DEVICE_SET_COM_INTERFACE_SPEED = 133,
			HID_DEVICE_GET_COM_ADDR = 134,
			HID_DEVICE_SET_COM_ADDR = 135,
			HID_DEVICE_INIT = 136,
			HID_DEVICE_RESET = 137,
			HID_DEVICE_GET_BITRATE = 138,
			HID_DEVICE_SET_BITRATE = 139,
			HID_DEVICE_GET_PACKET = 140,
			HID_DEVICE_SET_PACKET = 141,
			HID_DEVICE_GET_PACKET_BUFFER = 142,
			HID_DEVICE_SET_PACKET_BUFFER = 143,
			HID_DEVICE_SEND_TX_PACKET = 144,
			HID_DEVICE_GET_BTN_PACKET = 145,
			HID_DEVICE_SET_BTN_PACKET = 146,
			HID_DEVICE_GET_PN_SEQUENCE = 160,
			HID_DEVICE_SET_PN_SEQUENCE = 161,
			HID_DEVICE_GET_PN_ENABLE = 162,
			HID_DEVICE_SET_PN_ENABLE = 163,
			HID_SK_CMD_NONE = 255
		}

		private enum HidCommandsStatus
		{
			SX_OK,
			SX_ERROR,
			SX_BUSY,
			SX_EMPTY,
			SX_DONE,
			SX_TIMEOUT,
			SX_UNSUPPORTED,
			SX_WAIT,
			SX_CLOSE,
			SX_ACK,
			SX_NACK,
			SX_YES,
			SX_NO
		}

		public bool AccessSharedFskReg
		{
			get => accessSharedFskReg;
			set
			{
				accessSharedFskReg = value;
				OnPropertyChanged(nameof(AccessSharedFskReg));
			}
		}

		public bool AgcAutoOn
		{
			get => agcAutoOn;
			set
			{
				agcAutoOn = value;
				OnPropertyChanged(nameof(AgcAutoOn));
			}
		}

		public int AgcReference => (int)Math.Round(-174.0 + 10.0 * Math.Log10(2.0 * BandwidthHz) + AgcReferenceLevel, MidpointRounding.AwayFromZero);

		public byte AgcReferenceLevel
		{
			get => agcReferenceLevel;
			set
			{
				agcReferenceLevel = value;
				OnPropertyChanged(nameof(AgcReferenceLevel));
				OnPropertyChanged(nameof(AgcReference));
				// OnPropertyChanged("AgcThresh1");
				// OnPropertyChanged("AgcThresh2");
				// OnPropertyChanged("AgcThresh3");
				// OnPropertyChanged("AgcThresh4");
				// OnPropertyChanged("AgcThresh5");
			}
		}

		public byte AgcStep1
		{
			get => agcStep1;
			set
			{
				agcStep1 = value;
				OnPropertyChanged(nameof(AgcStep1));
				OnPropertyChanged(nameof(AgcThresh1));
				// OnPropertyChanged("AgcThresh2");
				// OnPropertyChanged("AgcThresh3");
				// OnPropertyChanged("AgcThresh4");
				// OnPropertyChanged("AgcThresh5");
			}
		}

		public byte AgcStep2
		{
			get => agcStep2;
			set
			{
				agcStep2 = value;
				OnPropertyChanged(nameof(AgcStep2));
				OnPropertyChanged(nameof(AgcThresh2));
				// OnPropertyChanged("AgcThresh3");
				// OnPropertyChanged("AgcThresh4");
				// OnPropertyChanged("AgcThresh5");
			}
		}

		public byte AgcStep3
		{
			get => agcStep3;
			set
			{
				agcStep3 = value;
				OnPropertyChanged(nameof(AgcStep3));
				OnPropertyChanged(nameof(AgcThresh3));
				// OnPropertyChanged("AgcThresh4");
				// OnPropertyChanged("AgcThresh5");
			}
		}

		public byte AgcStep4
		{
			get => agcStep4;
			set
			{
				agcStep4 = value;
				OnPropertyChanged(nameof(AgcStep4));
				OnPropertyChanged(nameof(AgcThresh4));
				// OnPropertyChanged("AgcThresh5");
			}
		}

		public byte AgcStep5
		{
			get => agcStep5;
			set
			{
				agcStep5 = value;
				OnPropertyChanged(nameof(AgcStep5));
				OnPropertyChanged(nameof(AgcThresh5));
			}
		}

		public int AgcThresh1 => AgcReference + agcStep1;

		public int AgcThresh2 => AgcThresh1 + agcStep2;

		public int AgcThresh3 => AgcThresh2 + agcStep3;

		public int AgcThresh4 => AgcThresh3 + agcStep4;

		public int AgcThresh5 => AgcThresh4 + agcStep5;

		public BandEnum Band
		{
			get => band;
			set
			{
				band = value;
				OnPropertyChanged(nameof(Band));
			}
		}

		public byte Bandwidth
		{
			get => bandwidth;
			set
			{
				bandwidth = value;
				OnPropertyChanged(nameof(Bandwidth));
				BandwidthCheck(value);
			}
		}

		public bool CadDetected
		{
			get => _cadDetected;
			set
			{
				if (value == _cadDetected) return;
				_cadDetected = value;
				OnPropertyChanged(nameof(CadDetected));
			}
		}

		public bool CadDetectedMask
		{
			get => cadDetectedMask;
			set
			{
				cadDetectedMask = value;
				OnPropertyChanged(nameof(CadDetectedMask));
			}
		}

		public bool CadDone
		{
			get => _cadDone;
			set
			{
				if (value == _cadDone) return;
				_cadDone = value;
				OnPropertyChanged(nameof(CadDone));
			}
		}

		public bool CadDoneMask
		{
			get => cadDoneMask;
			set
			{
				cadDoneMask = value;
				OnPropertyChanged(nameof(CadDoneMask));
			}
		}

		public byte CodingRate
		{
			get => codingRate;
			set
			{
				codingRate = value;
				OnPropertyChanged(nameof(CodingRate));
			}
		}

		public string DeviceName
		{
			get => _deviceName;
			private set
			{
				if (value == _deviceName) return;
				_deviceName = value;
				OnPropertyChanged(nameof(DeviceName));
			}
		}

		public DioMappingEnum Dio0Mapping
		{
			get => dio0Mapping;
			set
			{
				dio0Mapping = value;
				OnPropertyChanged(nameof(Dio0Mapping));
			}
		}

		public DioMappingEnum Dio1Mapping
		{
			get => dio1Mapping;
			set
			{
				dio1Mapping = value;
				OnPropertyChanged(nameof(Dio1Mapping));
			}
		}

		public DioMappingEnum Dio2Mapping
		{
			get => dio2Mapping;
			set
			{
				dio2Mapping = value;
				OnPropertyChanged(nameof(Dio2Mapping));
			}
		}

		public DioMappingEnum Dio3Mapping
		{
			get => dio3Mapping;
			set
			{
				dio3Mapping = value;
				OnPropertyChanged(nameof(Dio3Mapping));
			}
		}

		public DioMappingEnum Dio4Mapping
		{
			get => dio4Mapping;
			set
			{
				dio4Mapping = value;
				OnPropertyChanged(nameof(Dio4Mapping));
			}
		}

		public DioMappingEnum Dio5Mapping
		{
			get => dio5Mapping;
			set
			{
				dio5Mapping = value;
				OnPropertyChanged(nameof(Dio5Mapping));
			}
		}

		public bool FastHopOn
		{
			get => fastHopOn;
			set
			{
				fastHopOn = value;
				OnPropertyChanged(nameof(FastHopOn));
			}
		}

		public bool FhssChangeChannel
		{
			get => _fhssChangeChannel;
			set
			{
				if (value == _fhssChangeChannel) return;
				_fhssChangeChannel = value;
				OnPropertyChanged(nameof(FhssChangeChannel));
			}
		}

		public bool FhssChangeChannelMask
		{
			get => fhssChangeChannelMask;
			set
			{
				fhssChangeChannelMask = value;
				OnPropertyChanged(nameof(FhssChangeChannelMask));
			}
		}

		public byte FifoAddrPtr
		{
			get => fifoAddrPtr;
			set
			{
				fifoAddrPtr = value;
				OnPropertyChanged(nameof(FifoAddrPtr));
			}
		}

		public byte FifoRxCurrentAddr
		{
			get => _fifoRxCurrentAddr;
			private set
			{
				if (value == _fifoRxCurrentAddr) return;
				_fifoRxCurrentAddr = value;
				OnPropertyChanged(nameof(FifoRxCurrentAddr));
			}
		}

		public bool ForceRxBandLowFrequencyOn
		{
			get => forceRxBandLowFrequencyOn;
			set
			{
				forceRxBandLowFrequencyOn = value;
				OnPropertyChanged(nameof(ForceRxBandLowFrequencyOn));
			}
		}

		public bool ForceTxBandLowFrequencyOn
		{
			get => forceTxBandLowFrequencyOn;
			set
			{
				forceTxBandLowFrequencyOn = value;
				OnPropertyChanged(nameof(ForceTxBandLowFrequencyOn));
			}
		}

		public byte FreqHoppingPeriod
		{
			get => freqHoppingPeriod;
			set
			{
				freqHoppingPeriod = value;
				OnPropertyChanged(nameof(FreqHoppingPeriod));
			}
		}

		public decimal FrequencyRf
		{
			get => frequencyRf;
			set
			{
				frequencyRf = value;
				OnPropertyChanged(nameof(FrequencyRf));
				FrequencyRfCheck(value);
			}
		}

		public decimal FrequencyStep
		{
			get => frequencyStep;
			private set
			{
				frequencyStep = value;
				OnPropertyChanged(nameof(FrequencyStep));
			}
		}

		public decimal FrequencyXo
		{
			get => frequencyXo;
			set
			{
				frequencyXo = value;
				FrequencyStep = frequencyXo / (decimal)Math.Pow(2.0, 19.0);
				FrequencyRf = ((registers["RegFrfMsb"].Value << 16) | (registers["RegFrfMid"].Value << 8) | registers["RegFrfLsb"].Value) * FrequencyStep;
				OnPropertyChanged(nameof(FrequencyXo));
			}
		}

		public Version FwVersion
		{
			get => fwVersion;
			set
			{
				if (fwVersion == value) return;
				fwVersion = value;
				OnPropertyChanged(nameof(FwVersion));
			}
		}

		public bool HeaderInfoValid
		{
			get => _headerInfoValid;
			set
			{
				if (value == _headerInfoValid) return;
				_headerInfoValid = value;
				OnPropertyChanged(nameof(HeaderInfoValid));
			}
		}

		public byte HopChannel
		{
			get => _hopChannel;
			set
			{
				if (value == _hopChannel) return;
				_hopChannel = value;
				OnPropertyChanged(nameof(HopChannel));
			}
		}

		public bool ImplicitHeaderModeOn
		{
			get => implicitHeaderModeOn;
			set
			{
				implicitHeaderModeOn = value;
				OnPropertyChanged(nameof(ImplicitHeaderModeOn));
			}
		}

		public bool IsDebugOn
		{
			get => isDebugOn;
			set
			{
				isDebugOn = value;
				OnPropertyChanged(nameof(IsDebugOn));
			}
		}

        public bool IsOpen
		{
			get => isOpen;
			set
			{
				isOpen = value;
				OnPropertyChanged(nameof(IsOpen));
			}
		}

		public bool IsPacketHandlerStarted
		{
			get => _isPacketHandlerStarted;
			set
			{
				if (value == _isPacketHandlerStarted) return;
				_isPacketHandlerStarted = value;
				OnPropertyChanged(nameof(IsPacketHandlerStarted));
			}
		}

		public bool LnaBoost
		{
			get => lnaBoost;
			set
			{
				lnaBoost = value;
				OnPropertyChanged(nameof(LnaBoost));
			}
		}

		public LnaGainEnum LnaGain
		{
			get => lnaGain;
			set
			{
				lnaGain = value;
				OnPropertyChanged(nameof(LnaGain));
			}
		}

		public bool LogEnabled
		{
			get => logEnabled;
			set
			{
				logEnabled = value;
				OnPropertyChanged(nameof(LogEnabled));
			}
		}

		public bool LowDatarateOptimize
		{
			get => lowDatarateOptimize;
			set
			{
				lowDatarateOptimize = value;
				OnPropertyChanged(nameof(LowDatarateOptimize));
			}
		}

		public bool LowFrequencyModeOn
		{
			get => lowFrequencyModeOn;
			private set
			{
				lowFrequencyModeOn = value;
				OnPropertyChanged(nameof(LowFrequencyModeOn));
			}
		}

		public decimal MaxOutputPower
		{
			get => maxOutputPower;
			set
			{
				maxOutputPower = value;
				OnPropertyChanged(nameof(MaxOutputPower));
			}
		}

		public OperatingModeEnum Mode
		{
			get => mode;
			set
			{
				mode = value;
				OnPropertyChanged(nameof(Mode));
				OnPropertyChanged(nameof(PayloadLength));
			}
		}

		public bool ModemClear
		{
			get => _modemClear;
			set
			{
				if (value == _modemClear) return;
				_modemClear = value;
				OnPropertyChanged(nameof(ModemClear));
			}
		}

		public bool Monitor
		{
			get
			{
				lock (SyncThread)
				{
					return monitor;
				}
			}
			set
			{
				lock (SyncThread)
				{
					monitor = value;
					OnPropertyChanged(nameof(Monitor));
				}
			}
		}

		public bool OcpOn
		{
			get => ocpOn;
			set
			{
				ocpOn = value;
				OnPropertyChanged(nameof(OcpOn));
			}
		}

		public decimal OcpTrim
		{
			get => ocpTrim;
			set
			{
				ocpTrim = value;
				OnPropertyChanged(nameof(OcpTrim));
			}
		}

		public decimal OutputPower
		{
			get => outputPower;
			set
			{
				outputPower = value;
				OnPropertyChanged(nameof(OutputPower));
			}
		}

		public bool Pa20dBm
		{
			get => pa20dBm;
			private set
			{
				pa20dBm = value;
				OnPropertyChanged(nameof(Pa20dBm));
			}
		}

		public ILog PacketHandlerLog
		{
			get => packetHandlerLog;
			private set
			{
				packetHandlerLog = value;
				OnPropertyChanged(nameof(PacketHandlerLog));
			}
		}

		public bool PacketModeRxSingle
		{
			get => packetModeRxSingle;
			set
			{
				packetModeRxSingle = value;
				OnPropertyChanged(nameof(PacketModeRxSingle));
				// OnPropertyChanged("PayloadLength");
			}
		}

		public bool PacketModeTx
		{
			get => packetModeTx;
			set
			{
				packetModeTx = value;
				OnPropertyChanged(nameof(PacketModeTx));
				OnPropertyChanged(nameof(PayloadLength));
			}
		}

		public bool PacketUsePer
		{
			get => packetUsePer;
			set
			{
				packetUsePer = value;
				OnPropertyChanged(nameof(PacketUsePer));
			}
		}

		public PaRampEnum PaRamp
		{
			get => paRamp;
			set
			{
				paRamp = value;
				OnPropertyChanged(nameof(PaRamp));
			}
		}

		public PaSelectEnum PaSelect
		{
			get => paSelect;
			set
			{
				paSelect = value;
				OnPropertyChanged(nameof(PaSelect));
			}
		}

		public byte[] Payload
		{
			get => payload;
			set
			{
				payload = value;
				OnPropertyChanged(nameof(Payload));
				OnPropertyChanged(nameof(PayloadLength));
			}
		}

		public bool PayloadCrcError
		{
			get => _payloadCrcError;
			set
			{
				if (value == _payloadCrcError) return;
				_payloadCrcError = value;
				OnPropertyChanged(nameof(PayloadCrcError));
			}
		}

		public bool PayloadCrcErrorMask
		{
			get => payloadCrcErrorMask;
			set
			{
				payloadCrcErrorMask = value;
				OnPropertyChanged(nameof(PayloadCrcErrorMask));
			}
		}

		public bool PayloadCrcOn
		{
			get => payloadCrcOn;
			set
			{
				payloadCrcOn = value;
				OnPropertyChanged(nameof(PayloadCrcOn));
			}
		}

		public byte PayloadLength
		{
			get
			{
				if (Mode == OperatingModeEnum.Rx || !PacketModeTx)
				{
					return payloadLengthRx;
				}
                const byte b = 0;
                return (byte)(b + (byte)Payload.Length);
			}
			set
			{
				if (Mode == OperatingModeEnum.Rx || !PacketModeTx)
				{
					payloadLengthRx = value;
				}
				else
				{
					payloadLength = value;
				}
				OnPropertyChanged(nameof(PayloadLength));
			}
		}

		public byte PayloadMaxLength
		{
			get => payloadMaxLength;
			set
			{
				payloadMaxLength = value;
				OnPropertyChanged(nameof(PayloadMaxLength));
			}
		}

		public List<int> PktLnaValues
		{
			get => pktLnaValues;
			set
			{
				if (Equals(pktLnaValues, value)) return;
				if (Equals(value, pktLnaValues)) return;
				pktLnaValues = value;
				OnPropertyChanged(nameof(PktLnaValues));
			}
		}

		public decimal PktRssiValue
		{
			get => _pktRssiValue;
			set
			{
				if (value == _pktRssiValue) return;
				_pktRssiValue = value;
				OnPropertyChanged(nameof(PktRssiValue));
			}
		}

		public List<double> PktRssiValues
		{
			get => pktRssiValues;
			set
			{
				if (value == null || pktRssiValues == value) return;
				if (Equals(value, pktRssiValues)) return;
				pktRssiValues = value;
				OnPropertyChanged(nameof(PktRssiValues));
			}
		}

		public sbyte PktSnrValue
		{
			get => _pktSnrValue;
			set
			{
				if (value == _pktSnrValue) return;
				_pktSnrValue = value;
				OnPropertyChanged(nameof(PktSnrValue));
			}
		}

		public decimal PllBandwidth
		{
			get => pllBandwidth;
			private set
			{
				pllBandwidth = value;
				OnPropertyChanged(nameof(PllBandwidth));
			}
		}

		public bool PllTimeout
		{
			get => _pllTimeout;
			set
			{
				if (value == _pllTimeout) return;
				_pllTimeout = value;
				OnPropertyChanged(nameof(PllTimeout));
			}
		}

		public ushort PreambleLength
		{
			get => preambleLength;
			set
			{
				preambleLength = value;
				OnPropertyChanged(nameof(PreambleLength));
			}
		}

		public RegisterCollection Registers
		{
			get => registers;
			set
			{
				registers = value;
				OnPropertyChanged(nameof(Registers));
			}
		}

		public decimal RfIoRssiValue
		{
			get => _rfIoRssiValue;
			set
			{
				if (value == _rfIoRssiValue) return;
				_rfIoRssiValue = value;
				OnPropertyChanged(nameof(RfIoRssiValue));
			}
		}

		public decimal RfPaRssiValue
		{
			get => _rfPaRssiValue;
			set
			{
				if (value == _rfPaRssiValue) return;
				_rfPaRssiValue = value;
				OnPropertyChanged(nameof(RfPaRssiValue));
			}
		}

		public int RfPaSwitchEnabled
		{
			get => rfPaSwitchEnabled;
			set
			{
				lock (SyncThread)
				{
					try
					{
						prevRfPaSwitchEnabled = rfPaSwitchEnabled;
						rfPaSwitchEnabled = value;
						if (prevRfPaSwitchEnabled != rfPaSwitchEnabled)
						{
							if (rfPaSwitchEnabled == 2)
							{
								prevRfPaSwitchSel = rfPaSwitchSel;
							}
							else
							{
								rfPaSwitchSel = prevRfPaSwitchSel;
							}
						}
						_ = rfPaSwitchEnabled;
						OnPropertyChanged(nameof(RfPaSwitchEnabled));
						// OnPropertyChanged("RfPaSwitchSel");
					}
					catch (Exception ex)
					{
						OnError(1, ex.Message);
					}
				}
			}
		}

		public RfPaSwitchSelEnum RfPaSwitchSel
		{
			get => rfPaSwitchSel;
			set
			{
				lock (SyncThread)
				{
					try
					{
						rfPaSwitchSel = value;
						switch (value)
						{
						}
						OnPropertyChanged(nameof(RfPaSwitchSel));
					}
					catch (Exception ex)
					{
						OnError(1, ex.Message);
					}
				}
			}
		}

		public decimal RssiValue
		{
			get => _rssiValue;
			set
			{
				if (value == _rssiValue) return;
				_rssiValue = value;
				OnPropertyChanged(nameof(RssiValue));
			}
		}

		public bool RxDone
		{
			get => _rxDone;
			set
			{
				if (value == _rxDone) return;
				_rxDone = value;
				OnPropertyChanged(nameof(RxDone));
			}
		}

		public bool RxDoneMask
		{
			get => rxDoneMask;
			set
			{
				rxDoneMask = value;
				OnPropertyChanged(nameof(RxDoneMask));
			}
		}

		public byte RxNbBytes
		{
			get => _rxNbBytes;
			set
			{
				if (value == _rxNbBytes) return;
				_rxNbBytes = value;
				OnPropertyChanged(nameof(RxNbBytes));
			}
		}

		public bool RxOnGoing
		{
			get => _rxOnGoing;
			set
			{
				if (value == _rxOnGoing) return;
				_rxOnGoing = value;
				OnPropertyChanged(nameof(RxOnGoing));
			}
		}

		public byte RxPayloadCodingRate
		{
			get => _rxPayloadCodingRate;
			set
			{
				if (value == _rxPayloadCodingRate) return;
				_rxPayloadCodingRate = value;
				OnPropertyChanged(nameof(RxPayloadCodingRate));
			}
		}

		public bool RxPayloadCrcOn
		{
			get => _rxPayloadCrcOn;
			set
			{
				if (value == _rxPayloadCrcOn) return;
				_rxPayloadCrcOn = value;
				OnPropertyChanged(nameof(RxPayloadCrcOn));
			}
		}

		public bool RxTimeout
		{
			get => _rxTimeout;
			set
			{
				if (value == _rxTimeout) return;
				_rxTimeout = value;
				OnPropertyChanged(nameof(RxTimeout));
			}
		}

		public bool RxTimeoutMask
		{
			get => rxTimeoutMask;
			set
			{
				rxTimeoutMask = value;
				OnPropertyChanged(nameof(RxTimeoutMask));
			}
		}

		public bool SignalDetected
		{
			get => _signalDetected;
			set
			{
				if (value == _signalDetected) return;
				_signalDetected = value;
				OnPropertyChanged(nameof(SignalDetected));
			}
		}

		public bool SignalSynchronized
		{
			get => _signalSynchronized;
			set
			{
				if (value == _signalSynchronized) return;
				_signalSynchronized = value;
				OnPropertyChanged(nameof(SignalSynchronized));
			}
		}

		public decimal SpectrumFrequencySpan
		{
			get => spectrumFreqSpan;
			set
			{
				spectrumFreqSpan = value;
				OnPropertyChanged(nameof(SpectrumFrequencySpan));
			}
		}

		public decimal SpectrumRssiValue
		{
			get => _spectrumRssiValue;
			private set
			{
				if (value == _spectrumRssiValue) return;
				_spectrumRssiValue = value;
				OnPropertyChanged(nameof(SpectrumRssiValue));
			}
		}

		public int SPISpeed
		{
			get => spiSpeed;
			set
			{
				spiSpeed = value;
				OnPropertyChanged(nameof(SPISpeed));
			}
		}

		public byte SpreadingFactor
		{
			get => spreadingFactor;
			set
			{
				spreadingFactor = value;
				OnPropertyChanged(nameof(SpreadingFactor));
			}
		}

		public decimal SymbolTime => 1m / SymbolRate;

		public decimal SymbTimeout
		{
			get => symbTimeout;
			set
			{
				symbTimeout = value;
				OnPropertyChanged(nameof(SymbTimeout));
			}
		}

		public bool TcxoInputOn
		{
			get => tcxoInputOn;
			set
			{
				tcxoInputOn = value;
				OnPropertyChanged(nameof(TcxoInputOn));
			}
		}

		public bool Test
		{
			get => _test;
			set
			{
				if (value == _test) return;
				_test = value;
				OnPropertyChanged(nameof(Test));
			}
		}

		public bool TxContinuousModeOn
		{
			get => txContinuousModeOn;
			set
			{
				txContinuousModeOn = value;
				OnPropertyChanged(nameof(TxContinuousModeOn));
			}
		}

		public bool TxDone
		{
			get => _txDone;
			set
			{
				if (value == _txDone) return;
				_txDone = value;
				OnPropertyChanged(nameof(TxDone));
			}
		}

		public bool TxDoneMask
		{
			get => txDoneMask;
			set
			{
				txDoneMask = value;
				OnPropertyChanged(nameof(TxDoneMask));
			}
		}

		public bool ValidHeader
		{
			get => _validHeader;
			set
			{
				if (value == _validHeader) return;
				_validHeader = value;
				OnPropertyChanged(nameof(ValidHeader));
			}
		}

		public ushort ValidHeaderCnt
		{
			get => _validHeaderCnt;
			set
			{
				if (value == _validHeaderCnt) return;
				_validHeaderCnt = value;
				OnPropertyChanged(nameof(ValidHeaderCnt));
			}
		}

		public bool ValidHeaderMask
		{
			get => validHeaderMask;
			set
			{
				validHeaderMask = value;
				OnPropertyChanged(nameof(ValidHeaderMask));
			}
		}

		public ushort ValidPacketCnt
		{
			get => _validPacketCnt;
			set
			{
				if (value == _validPacketCnt) return;
				_validPacketCnt = value;
				OnPropertyChanged(nameof(ValidPacketCnt));
			}
		}

		public Version Version
		{
			get => version;
			set
			{
				if (version == value) return;
				version = value;
				OnPropertyChanged(nameof(Version));
			}
		}

		private double BandwidthHz => Bandwidth switch
		{
			0 => 7812.5, 
			1 => 10416.7, 
			2 => 15625.0, 
			3 => 20833.3, 
			4 => 31250.0, 
			5 => 41666.7, 
			6 => 62500.0, 
			8 => 250000.0, 
			9 => 500000.0, 
			_ => 125000.0, 
		};

		private byte FifoRxBaseAddr
		{
			get => fifoRxBaseAddr;
			set
			{
				fifoRxBaseAddr = value;
				OnPropertyChanged(nameof(FifoRxBaseAddr));
			}
		}

		private byte FifoTxBaseAddr
		{
			get => fifoTxBaseAddr;
			set
			{
				fifoTxBaseAddr = value;
				OnPropertyChanged(nameof(FifoTxBaseAddr));
			}
		}

		private int SpectrumFrequencyId
		{
			get => spectrumFreqId;
			set
			{
				spectrumFreqId = value;
				OnPropertyChanged("SpectrumFreqId");
			}
		}

		private decimal SpectrumFrequencyMax => FrequencyRf + SpectrumFrequencySpan / 2.0m;

		private decimal SpectrumFrequencyMin => FrequencyRf - SpectrumFrequencySpan / 2.0m;

		private decimal SpectrumFrequencyStep => (decimal)BandwidthHz / 3.0m;

		private int SpectrumNbFrequenciesMax => (int)((SpectrumFrequencyMax - SpectrumFrequencyMin) / SpectrumFrequencyStep);

		private bool SpectrumOn
		{
			get => spectrumOn;
			set
			{
				spectrumOn = value;
				if (spectrumOn)
				{
					RfPaSwitchEnabled = 0;
					prevAgcAutoOn = AgcAutoOn;
					SetAgcAutoOn(value: false);
					prevLnaGain = LnaGain;
					SetLnaGain(LnaGainEnum.G1);
					prevMode = Mode;
					SetOperatingMode(OperatingModeEnum.Rx);
					prevMonitorOn = Monitor;
					Monitor = true;
				}
				else
				{
					SetFrequencyRf(FrequencyRf);
					RfPaSwitchEnabled = prevRfPaSwitchEnabled;
					SetLnaGain(prevLnaGain);
					SetAgcAutoOn(prevAgcAutoOn);
					SetOperatingMode(prevMode);
					Monitor = prevMonitorOn;
				}
				OnPropertyChanged(nameof(SpectrumOn));
			}
		}

		private decimal SymbolRate => (decimal)(BandwidthHz / Math.Pow(2.0, SpreadingFactor));

		private object SyncThread { get; } = new();

		private HidDevice UsbDevice { get; }

		public bool Close()
		{
			if (!isOpen && UsbDevice is not { IsOpen: true }) return true;
			UsbDevice.Close();
			isOpen = false;
			return true;
		}

		public void ClrCadDetectedIrq()
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlags"].Value;
					b = (byte)(b & 0xFEu);
					b = (byte)(b | 1u);
					registers["RegIrqFlags"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrCadDoneIrq()
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlags"].Value;
					b = (byte)(b & 0xFBu);
					b = (byte)(b | 4u);
					registers["RegIrqFlags"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrFhssChangeChannelIrq()
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlags"].Value;
					b = (byte)(b & 0xFDu);
					b = (byte)(b | 2u);
					registers["RegIrqFlags"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrPayloadCrcErrorIrq()
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlags"].Value;
					b = (byte)(b & 0xDFu);
					b = (byte)(b | 0x20u);
					registers["RegIrqFlags"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrRxDoneIrq()
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlags"].Value;
					b = (byte)(b & 0xBFu);
					b = (byte)(b | 0x40u);
					registers["RegIrqFlags"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrRxTimeoutIrq()
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlags"].Value;
					b = (byte)(b & 0x7Fu);
					b = (byte)(b | 0x80u);
					registers["RegIrqFlags"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrTxDoneIrq()
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlags"].Value;
					b = (byte)(b & 0xF7u);
					b = (byte)(b | 8u);
					registers["RegIrqFlags"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrValidHeaderIrq()
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlags"].Value;
					b = (byte)(b & 0xEFu);
					b = (byte)(b | 0x10u);
					registers["RegIrqFlags"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void Dispose()
		{
			Close();
		}

		public Version GetVersion()
		{
			byte data = 0;
			if (!Read(66, ref data))
			{
				throw new Exception("Unable to read register RegVersion");
			}
			if (!Read(66, ref data))
			{
				throw new Exception("Unable to read register RegVersion");
			}
			return new Version((data & 0xF0) >> 4, data & 0xF);
		}

		public bool Open()
		{
			try
			{
				Close();
				return UsbDevice.Open();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			return false;
		}

		public void OpenConfig(ref FileStream stream)
		{
			OnError(0, "-");
			using var streamReader = new StreamReader(stream, Encoding.ASCII);
			var num = 1;
			var num2 = 0;
			var text = "";
			try
			{
				while (streamReader.ReadLine() is { } text2)
				{
					if (text2[0] == '#')
					{
						num++;
						continue;
					}
					if (text2[0] != 'R' && text2[0] != 'P' && text2[0] != 'X')
					{
						throw new Exception("At line " + num + ": A configuration line must start either by\n\"#\" for comments\nor a\n\"R\" for the register name.\nor a\n\"P\" for packet settings.\nor a\n\"X\" for crystal frequency.");
					}
					var array = text2.Split('\t');
					if (array.Length != 4)
					{
						if (array.Length != 2)
						{
							throw new Exception("At line " + num + ": The number of columns is " + array.Length + " and it should be 4 or 2.");
						}
						if (array[0] == "PKT")
						{
							text = array[1];
						}
						else
						{
							if (array[0] != "XTAL")
							{
								throw new Exception("At line " + num + ": Invalid Packet or XTAL frequency");
							}
							FrequencyXo = Convert.ToDecimal(array[1]);
						}
					}
					else
					{
						var flag = true;
						foreach (var t in registers)
						{
							if (t.Name != array[1]) continue;
							flag = false;
							break;
						}
						if (flag)
						{
							throw new Exception("At line " + num + ": Invalid register name.");
						}
						if (array[1] != "RegVersion")
						{
							registers[array[1]].Value = Convert.ToByte(array[3], 16);
							num2++;
						}
					}
					num++;
				}
				var array2 = text.Split(';');
				if (array2.Length > 1)
				{
					PacketModeTx = bool.Parse(array2[1]);
				}
				array2 = array2[0].Split(',');
				if (payload != null)
				{
					Array.Resize(ref payload, array2.Length);
				}
				else
				{
					payload = new byte[array2.Length];
				}
				for (var j = 0; j < array2.Length; j++)
				{
					if (array2[j].Length != 0)
					{
						payload[j] = Convert.ToByte(array2[j], 16);
					}
				}
				OnPropertyChanged(nameof(Payload));
				OnPropertyChanged(nameof(PayloadLength));
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				streamReader.Close();
				if (!IsOpen)
				{
					ReadRegisters();
				}
			}
		}

		public void ProcessWinMessage(int msg, IntPtr wParam, IntPtr lParam)
		{
			UsbDevice.ProcessWinMessage(msg, wParam, lParam);
		}

		public bool Read(byte address, ref byte data)
		{
			var array = new byte[1];
			var data2 = array;
			if (!SKDeviceRead(address, ref data2)) return false;
			data = data2[0];
			return true;
		}

		public void ReadIrqFlags()
		{
			lock (SyncThread)
			{
				ReadRegister(registers["RegIrqFlags"]);
				if (RxTimeout && PacketModeRxSingle && IsPacketHandlerStarted)
				{
					Console.WriteLine("IRQ: RxTimeoutIrq [0x{0:X02}] - ReadIrqFlags", registers["RegIrqFlags"].Value);
					ClrRxTimeoutIrq();
					PacketModeRxSingle = false;
					PacketHandlerStop();
				}
				_ = ValidHeader;
				if (!FhssChangeChannel) return;
				Console.WriteLine("IRQ: FhssChangeChannelIrq [0x{0:X02}] - ReadIrqFlags", registers["RegIrqFlags"].Value);
				ClrFhssChangeChannelIrq();
				ReadRegister(registers["RegHopChannel"]);
			}
		}

		public void ReadRegisters(RegisterCollection regs)
		{
			lock (SyncThread)
			{
				try
				{
					readLock++;
					foreach (var reg in regs)
					{
						if (reg.Address != 0 && (reg.Address != 1 || !IsPacketHandlerStarted))
						{
							ReadRegister(reg);
						}
					}
				}
				catch (Exception ex)
				{
					OnError(1, ex.Message);
				}
				finally
				{
					readLock--;
				}
			}
		}

		public void ReadRegisters()
		{
			lock (SyncThread)
			{
				try
				{
					readLock++;
					foreach (var register in registers)
					{
						if (register.Address != 0 && (register.Address != 1 || !IsPacketHandlerStarted))
						{
							ReadRegister(register);
						}
					}
				}
				catch (Exception ex)
				{
					OnError(1, ex.Message);
				}
				finally
				{
					readLock--;
				}
			}
		}

		public void Reset()
		{
			lock (SyncThread)
			{
				try
				{
					var flag = SpectrumOn;
					if (SpectrumOn)
					{
						SpectrumOn = false;
					}
					PacketHandlerStop();
					if (!SKReset())
					{
						throw new Exception("Unable to reset the SK");
					}
					ReadRegisters();
					var num = FrequencyRf;
					SetFrequencyRf(915000000m);
					ImageCalStart();
					SetFrequencyRf(num);
					ImageCalStart();
					SetLoraOn(enable: true);
					SetDefaultValues();
					ReadRegisters();
					RfPaSwitchEnabled = 0;
					RfPaSwitchSel = RfPaSwitchSelEnum.RF_IO_RFIO;
					if (flag)
					{
						SpectrumOn = true;
					}
				}
				catch (Exception ex)
				{
					OnError(1, ex.Message);
				}
			}
		}

		public void SaveConfig(ref FileStream stream)
		{
			OnError(0, "-");
			using var streamWriter = new StreamWriter(stream, Encoding.ASCII);
			try
			{
				streamWriter.WriteLine("#Type\tRegister Name\tAddress[Hex]\tValue[Hex]");
				foreach (var t in registers)
				{
					streamWriter.WriteLine("REG\t{0}\t0x{1:X02}\t0x{2:X02}", t.Name, t.Address, t.Value);
				}
				var text = "";
				if (Payload != null && Payload.Length != 0)
				{
					int j;
					for (j = 0; j < Payload.Length - 1; j++)
					{
						text = text + Payload[j].ToString("X02") + ",";
					}
					text += Payload[j].ToString("X02");
				}
				text += ";";
				text += PacketModeTx;
				streamWriter.WriteLine("PKT\t{0}", text);
				streamWriter.WriteLine("XTAL\t{0}", FrequencyXo);
			}
			finally
			{
				streamWriter.Close();
			}
		}

		public void SetAccessSharedFskReg(bool value)
		{
			try
			{
				var b = (byte)registers["RegOpMode"].Value;
				b = (byte)(b & 0xBFu);
				b |= (byte)(value ? 64u : 0u);
				registers["RegOpMode"].Value = b;
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetAgcAutoOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegModemConfig3"].Value;
					b = (byte)(b & 0xFBu);
					b |= (byte)(value ? 4u : 0u);
					registers["RegModemConfig3"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetAgcReferenceLevel(int value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegAgcRef"].Value;
					b = (byte)(b & 0xC0u);
					b |= (byte)((uint)value & 0x3Fu);
					registers["RegAgcRef"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetAgcStep(byte id, byte value)
		{
			try
			{
				lock (SyncThread)
				{
                    var register = id switch
                    {
                        1 => registers["RegAgcThresh1"],
                        2 or 3 => registers["RegAgcThresh2"],
                        4 or 5 => registers["RegAgcThresh3"],
                        _ => throw new Exception("Invalid AGC step ID!"),
                    };
                    var b = (byte)register.Value;
					switch (id)
					{
					case 1:
						b = (byte)(b & 0xE0u);
						b |= value;
						break;
					case 2:
						b = (byte)(b & 0xFu);
						b |= (byte)(value << 4);
						break;
					case 3:
						b = (byte)(b & 0xF0u);
						b |= (byte)(value & 0xFu);
						break;
					case 4:
						b = (byte)(b & 0xFu);
						b |= (byte)(value << 4);
						break;
					case 5:
						b = (byte)(b & 0xF0u);
						b |= (byte)(value & 0xFu);
						break;
					default:
						throw new Exception("Invalid AGC step ID!");
					}
					register.Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetBand(BandEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegFdevMsb"].Value;
					b = (byte)(b & 0x3Fu);
					b |= (byte)((int)value << 6);
					registers["RegFdevMsb"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetBandwidth(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					if (value < 7 || (value == 7 && (SpreadingFactor == 11 || SpreadingFactor == 12)) || (value == 8 && SpreadingFactor == 12))
					{
						SetLowDatarateOptimize(value: true);
					}
					else
					{
						SetLowDatarateOptimize(value: false);
					}
					var b = (byte)registers["RegModemConfig1"].Value;
					b = (byte)(b & 0xFu);
					b |= (byte)(value << 4);
					registers["RegModemConfig1"].Value = b;
					switch (value)
					{
						case 9 when FrequencyRf >= 640000000m:
							registers["RegTest36"].Value &= 254u;
							registers["RegTest3A"].Value = (registers["RegTest3A"].Value & 0xC0u) | 0x24u;
							break;
						case 9:
							registers["RegTest36"].Value &= 254u;
							registers["RegTest3A"].Value = (registers["RegTest3A"].Value & 0xC0u) | 0x3Fu;
							break;
						default:
							registers["RegTest36"].Value |= 1u;
							ReadRegister(registers["RegTest3A"]);
							break;
					}
					ReadRegister(registers["RegModemConfig2"]);
					ReadRegister(registers["RegSymbTimeoutLsb"]);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetCadDetectedMask(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlagsMask"].Value;
					b = (byte)(b & 0xFEu);
					b |= (byte)(value ? 1u : 0u);
					registers["RegIrqFlagsMask"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetCadDoneMask(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlagsMask"].Value;
					b = (byte)(b & 0xFBu);
					b |= (byte)(value ? 4u : 0u);
					registers["RegIrqFlagsMask"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetCodingRate(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegModemConfig1"].Value;
					b = (byte)(b & 0xF1u);
					b |= (byte)(value << 1);
					registers["RegModemConfig1"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetDioMapping(byte id, DioMappingEnum value)
		{
			try
			{
				lock (SyncThread)
				{
                    var register = id switch
                    {
                        0 or 1 or 2 or 3 => registers["RegDioMapping1"],
                        4 or 5 => registers["RegDioMapping2"],
                        _ => throw new Exception("Invalid DIO ID!"),
                    };
                    uint num = (byte)register.Value;
					switch (id)
					{
					case 0:
						num &= 0x3Fu;
						num |= (uint)((int)value << 6);
						break;
					case 1:
						num &= 0xCFu;
						num |= (uint)((int)value << 4);
						break;
					case 2:
						num &= 0xF3u;
						num |= (uint)((int)value << 2);
						break;
					case 3:
						num &= 0xFCu;
						num |= (uint)(value & DioMappingEnum.DIO_MAP_11);
						break;
					case 4:
						num &= 0x3Fu;
						num |= (uint)((int)value << 6);
						break;
					case 5:
						num &= 0xCFu;
						num |= (uint)((int)value << 4);
						break;
					default:
						throw new Exception("Invalid DIO ID!");
					}
					register.Value = num;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFastHopOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPllHop"].Value;
					b = (byte)(b & 0x7Fu);
					b |= (byte)(value ? 128u : 0u);
					registers["RegPllHop"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFhssChangeChannelMask(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlagsMask"].Value;
					b = (byte)(b & 0xFDu);
					b |= (byte)(value ? 2u : 0u);
					registers["RegIrqFlagsMask"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFifoRxBaseAddr(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegFifoRxBaseAddr"].Value = value;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFifoTxBaseAddr(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegFifoTxBaseAddr"].Value = value;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetForceRxBandLowFrequencyOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegLna"].Value;
					b = (byte)(b & 0xFBu);
					b |= (byte)(value ? 4u : 0u);
					registers["RegLna"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetForceTxBandLowFrequencyOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPaRamp"].Value;
					b = (byte)(b & 0xEFu);
					b |= (byte)(value ? 16u : 0u);
					registers["RegPaRamp"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFreqHoppingPeriod(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegHopPeriod"].Value = value;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFrequencyRf(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					// var b = (byte)registers["RegFrfMsb"].Value;
					// var b2 = (byte)registers["RegFrfMid"].Value;
					// var b3 = (byte)registers["RegFrfLsb"].Value;
					var b = (byte)((long)(value / frequencyStep) >> 16);
					var b2 = (byte)((long)(value / frequencyStep) >> 8);
					var b3 = (byte)(long)(value / frequencyStep);
					frequencyRfCheckDisable = true;
					registers["RegFrfMsb"].Value = b;
					registers["RegFrfMid"].Value = b2;
					frequencyRfCheckDisable = false;
					registers["RegFrfLsb"].Value = b3;
					if (FrequencyRf >= 640000000m)
					{
						SetLowFrequencyModeOn(value: false);
						SetLnaBoost(lnaBoostPrev);
					}
					else
					{
						SetLowFrequencyModeOn(value: true);
						lnaBoostPrev = LnaBoost;
						SetLnaBoost(value: false);
					}
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetImplicitHeaderModeOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegModemConfig1"].Value;
					b = (byte)(b & 0xFEu);
					b |= (byte)(value ? 1u : 0u);
					registers["RegModemConfig1"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetLnaBoost(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegLna"].Value;
					b = (byte)(b & 0xFCu);
					b |= (byte)(value ? 3u : 0u);
					registers["RegLna"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetLnaGain(LnaGainEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegLna"].Value;
					b = (byte)(b & 0x1Fu);
					b |= (byte)((byte)value << 5);
					registers["RegLna"].Value = b;
					ReadRegister(registers["RegLna"]);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetLowDatarateOptimize(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegModemConfig3"].Value;
					b = (byte)(b & 0xF7u);
					b |= (byte)(value ? 8u : 0u);
					registers["RegModemConfig3"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetLowFrequencyModeOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegOpMode"].Value;
					b = (byte)(b & 0xF7u);
					b |= (byte)(value ? 8u : 0u);
					registers["RegOpMode"].Value = b;
					UpdateRegisterTable();
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetMaxOutputPower(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPaConfig"].Value;
					b = (byte)(b & 0x8Fu);
					b |= (byte)(((int)((value - 10.8m) / 0.6m) & 7) << 4);
					registers["RegPaConfig"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetMaxPacketNumber(int value)
		{
			try
			{
				lock (SyncThread)
				{
					maxPacketNumber = value;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetMessage(byte[] value)
		{
			try
			{
				lock (SyncThread)
				{
					Payload = value;
					SetPayloadLength((byte)Payload.Length);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetMessageLength(int value)
		{
			try
			{
				lock (SyncThread)
				{
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetNotificationWindowHandle(IntPtr handle, bool isWpfApplication)
		{
			UsbDevice.RegisterNotification(handle, isWpfApplication);
		}

		public void SetOcpOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegOcp"].Value;
					b = (byte)(b & 0xDFu);
					b |= (byte)(value ? 32u : 0u);
					registers["RegOcp"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetOcpTrim(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegOcp"].Value;
					b = (byte)(b & 0xE0u);
					b = ((value <= 120.0m) ? ((byte)(b | (byte)((byte)((value - 45m) / 5m) & 0xFu))) : (((value <= 120m) || (value > 240.0m)) ? ((byte)(b | 0x1Bu)) : ((byte)(b | (byte)((byte)((value + 30m) / 10m) & 0x1Fu)))));
					registers["RegOcp"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetOperatingMode(OperatingModeEnum value)
		{
			SetOperatingMode(value, isQuiet: false);
		}

		public void SetOutputPower(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPaConfig"].Value;
					b = (byte)(b & 0xF0u);
					if (PaSelect == PaSelectEnum.RFO)
					{
						if (value < MaxOutputPower - 15.0m)
						{
							value = MaxOutputPower - 15.0m;
						}
						if (value > MaxOutputPower)
						{
							value = MaxOutputPower;
						}
						b |= (byte)((uint)(int)(value - MaxOutputPower + 15.0m) & 0xFu);
					}
					else if (!Pa20dBm)
					{
						if (value < 2m)
						{
							value = 2m;
						}
						if (value > 17m)
						{
							value = 17m;
						}
						b |= (byte)((uint)(int)(value - 17.0m + 15.0m) & 0xFu);
					}
					else
					{
						if (value < 5m)
						{
							value = 5m;
						}
						if (value > 20m)
						{
							value = 20m;
						}
						b |= (byte)((uint)(int)(value - 20.0m + 15.0m) & 0xFu);
					}
					registers["RegPaConfig"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPa20dBm(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					if (value == Pa20dBm) return;
					// var b = (byte)registers["RegPaDac"].Value;
					var b = (byte)(value ? 135u : 132u);
					registers["RegPaDac"].Value = b;
					if (value)
					{
						SetPaMode(PaSelectEnum.PA_BOOST);
					}
					ReadRegister(registers["RegPaConfig"]);
					ReadRegister(registers["RegOcp"]);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPacketHandlerLogEnable(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					LogEnabled = value;
				}
			}
			catch (Exception ex)
			{
				LogEnabled = false;
				OnError(1, ex.Message);
			}
		}

		public void SetPacketHandlerStartStop(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					if (value)
					{
						PacketHandlerStart();
					}
					else
					{
						PacketHandlerStop();
					}
				}
			}
			catch (Exception ex)
			{
				PacketHandlerStop();
				OnError(1, ex.Message);
			}
		}

		public void SetPaMode(PaSelectEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPaConfig"].Value;
					b = (byte)(b & 0x7Fu);
					switch (value)
					{
					case PaSelectEnum.PA_BOOST:
						b = (byte)(b | 0x80u);
						break;
					}
					registers["RegPaConfig"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPaRamp(PaRampEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPaRamp"].Value;
					b = (byte)(b & 0xF0u);
					b |= (byte)((byte)value & 0xFu);
					registers["RegPaRamp"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPayloadCrcErrorMask(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlagsMask"].Value;
					b = (byte)(b & 0xDFu);
					b |= (byte)(value ? 32u : 0u);
					registers["RegIrqFlagsMask"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPayloadCrcOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegModemConfig2"].Value;
					b = (byte)(b & 0xFBu);
					b |= (byte)(value ? 4u : 0u);
					registers["RegModemConfig2"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPayloadLength(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegPayloadLength"].Value = value;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPayloadMaxLength(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegPayloadMaxLength"].Value = value;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPllBandwidth(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPll"].Value;
					b = (byte)(b & 0x3Fu);
					b |= (byte)((byte)(value / 75000m - 1m) << 6);
					registers["RegPll"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPreambleLength(int value)
		{
			try
			{
				lock (SyncThread)
				{
					value -= 4;
					registers["RegPreambleMsb"].Value = (byte)(value >> 8);
					registers["RegPreambleLsb"].Value = (byte)value;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetRxDoneMask(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlagsMask"].Value;
					b = (byte)(b & 0xBFu);
					b |= (byte)(value ? 64u : 0u);
					registers["RegIrqFlagsMask"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetRxTimeoutMask(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlagsMask"].Value;
					b = (byte)(b & 0x7Fu);
					b |= (byte)(value ? 128u : 0u);
					registers["RegIrqFlagsMask"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetSpreadingFactor(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					if (Bandwidth < 7 || (Bandwidth == 7 && value is 11 or 12) || (Bandwidth == 8 && value == 12))
					{
						SetLowDatarateOptimize(value: true);
					}
					else
					{
						SetLowDatarateOptimize(value: false);
					}
					var b = (byte)registers["RegModemConfig2"].Value;
					b = (byte)(b & 0xFu);
					b |= (byte)(value << 4);
					registers["RegModemConfig2"].Value = b;
					ReadRegister(registers["RegSymbTimeoutLsb"]);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetSymbTimeout(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegModemConfig2"].Value;
					// var b2 = (byte)registers["RegSymbTimeoutLsb"].Value;
					b = (byte)(b & 0xFCu);
					b |= (byte)(((long)(value / SymbolTime) >> 8) & 3);
					var b2 = (byte)(long)(value / SymbolTime);
					registers["RegModemConfig2"].Value = b;
					registers["RegSymbTimeoutLsb"].Value = b2;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetTcxoInputOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegTcxo"].Value;
					b = (byte)(b & 0xEFu);
					b |= (byte)(value ? 16u : 0u);
					registers["RegTcxo"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetTxDoneMask(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlagsMask"].Value;
					b = (byte)(b & 0xF7u);
					b |= (byte)(value ? 8u : 0u);
					registers["RegIrqFlagsMask"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetValidHeaderMask(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlagsMask"].Value;
					b = (byte)(b & 0xEFu);
					b |= (byte)(value ? 16u : 0u);
					registers["RegIrqFlagsMask"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public bool SKGetId(ref byte id)
		{
			lock (SyncThread)
			{
                const byte command = 3;
                // var hidCommandsStatus = HidCommandsStatus.SX_ERROR;
				var inData = new byte[11];
				var array = new byte[2];
				var outData = array;
				UsbDevice.TxRxCommand(command, outData, ref inData);
				var hidCommandsStatus = (HidCommandsStatus)inData[0];
				_ = inData[1];
				_ = inData[2];
				_ = inData[3];
				_ = inData[4];
				_ = inData[5];
				_ = inData[6];
				_ = inData[7];
				_ = inData[8];
				if (hidCommandsStatus != HidCommandsStatus.SX_OK || inData[9] != 1) return false;
				id = inData[10];
				return true;
			}
		}

		public bool SKGetName()
		{
			lock (SyncThread)
			{
                const byte b = 2;
                var hidCommandsStatus = HidCommandsStatus.SX_ERROR;
				var num = ulong.MaxValue;
				var inData = new byte[25];
				var array = new byte[2];
				var outData = array;
				try
				{
					UsbDevice.TxRxCommand(b, outData, ref inData);
					hidCommandsStatus = (HidCommandsStatus)inData[0];
					num = ((ulong)inData[1] << 56) | ((ulong)inData[2] << 48) | ((ulong)inData[3] << 40) | ((ulong)inData[4] << 32) | ((ulong)inData[5] << 24) | ((ulong)inData[6] << 16) | ((ulong)inData[7] << 8) | inData[8];
					if (hidCommandsStatus == HidCommandsStatus.SX_OK && inData[9] >= 9)
					{
						Array.Copy(inData, 10, inData, 0, 10);
						Array.Resize(ref inData, 9);
						Encoding encoding = new ASCIIEncoding();
						DeviceName = encoding.GetString(inData);
						return true;
					}
					DeviceName = string.Empty;
					return false;
				}
				finally
				{
					Console.WriteLine("{0} ms: {1} with status {2}", num, Enum.GetName(typeof(HidCommands), (HidCommands)b), Enum.GetName(typeof(HidCommandsStatus), hidCommandsStatus));
				}
			}
		}

		public bool SKGetPinDir(byte pinId, ref byte dir)
		{
			lock (SyncThread)
			{
                const byte command = 18;
                // var hidCommandsStatus = HidCommandsStatus.SX_ERROR;
				var inData = new byte[11];
				var outData = new byte[] { 0, 1, pinId };
				UsbDevice.TxRxCommand(command, outData, ref inData);
				var hidCommandsStatus = (HidCommandsStatus)inData[0];
				_ = inData[1];
				_ = inData[2];
				_ = inData[3];
				_ = inData[4];
				_ = inData[5];
				_ = inData[6];
				_ = inData[7];
				_ = inData[8];
				if (hidCommandsStatus != HidCommandsStatus.SX_OK || inData[9] != 1) return false;
				dir = inData[10];
				return true;
			}
		}

		public bool SKGetPinState(byte pinId, ref byte state)
		{
			lock (SyncThread)
			{
                const byte command = 16;
                // var hidCommandsStatus = HidCommandsStatus.SX_ERROR;
				var inData = new byte[11];
				var outData = new byte[] { 0, 1, pinId };
				UsbDevice.TxRxCommand(command, outData, ref inData);
				var hidCommandsStatus = (HidCommandsStatus)inData[0];
				_ = inData[1];
				_ = inData[2];
				_ = inData[3];
				_ = inData[4];
				_ = inData[5];
				_ = inData[6];
				_ = inData[7];
				_ = inData[8];
				if (hidCommandsStatus != HidCommandsStatus.SX_OK || inData[9] != 1) return false;
				state = inData[10];
				return true;
			}
		}

		public bool SKReadEeprom(byte id, byte address, ref byte[] data)
		{
			lock (SyncThread)
			{
                const byte command = 0x70;
                // var hidCommandsStatus = HidCommandsStatus.SX_ERROR;
				var inData = new byte[42];
				var array = new byte[] { 0, 3, 32, id, address };
				for (int i = address; i < data.Length; i += 32)
				{
					array[4] = (byte)i;
					UsbDevice.TxRxCommand(command, array, ref inData);
					var hidCommandsStatus = (HidCommandsStatus)inData[0];
					_ = inData[1];
					_ = inData[2];
					_ = inData[3];
					_ = inData[4];
					_ = inData[5];
					_ = inData[6];
					_ = inData[7];
					_ = inData[8];
					if (hidCommandsStatus != HidCommandsStatus.SX_OK) continue;
					if (32 != inData[9])
					{
						data = null;
						return false;
					}
					Array.Copy(inData, 10, data, i, 32);
				}
				return true;
			}
		}

		public bool SKSetId(byte id)
		{
			lock (SyncThread)
			{
                const byte command = 4;
                // var hidCommandsStatus = HidCommandsStatus.SX_ERROR;
				var inData = new byte[10];
				var outData = new byte[] { 0, 1, id };
				UsbDevice.TxRxCommand(command, outData, ref inData);
				var hidCommandsStatus = (HidCommandsStatus)inData[0];
				_ = inData[1];
				_ = inData[2];
				_ = inData[3];
				_ = inData[4];
				_ = inData[5];
				_ = inData[6];
				_ = inData[7];
				_ = inData[8];
				return hidCommandsStatus == HidCommandsStatus.SX_OK;
			}
		}

		public bool SKSetPinDir(byte pinId, byte dir)
		{
			lock (SyncThread)
			{
                const byte command = 19;
                // var hidCommandsStatus = HidCommandsStatus.SX_ERROR;
				var inData = new byte[10];
				var outData = new byte[] { 0, 2, pinId, dir };
				UsbDevice.TxRxCommand(command, outData, ref inData);
				var hidCommandsStatus = (HidCommandsStatus)inData[0];
				_ = inData[1];
				_ = inData[2];
				_ = inData[3];
				_ = inData[4];
				_ = inData[5];
				_ = inData[6];
				_ = inData[7];
				_ = inData[8];
				return hidCommandsStatus == HidCommandsStatus.SX_OK;
			}
		}

		public bool SKSetRndId(ref byte id)
		{
			lock (SyncThread)
			{
                const byte command = 5;
                // var hidCommandsStatus = HidCommandsStatus.SX_ERROR;
				var inData = new byte[11];
				var array = new byte[2];
				var outData = array;
				UsbDevice.TxRxCommand(command, outData, ref inData);
				var hidCommandsStatus = (HidCommandsStatus)inData[0];
				_ = inData[1];
				_ = inData[2];
				_ = inData[3];
				_ = inData[4];
				_ = inData[5];
				_ = inData[6];
				_ = inData[7];
				_ = inData[8];
				if (hidCommandsStatus != HidCommandsStatus.SX_OK || inData[9] != 1) return false;
				id = inData[10];
				return true;
			}
		}

		public bool SKWriteEeprom(byte id, byte address, byte[] data)
		{
			lock (SyncThread)
			{
                const byte command = 113;
                // var hidCommandsStatus = HidCommandsStatus.SX_ERROR;
				var inData = new byte[10];
				var array = new byte[37];
				array[0] = 0;
				array[1] = 35;
				array[2] = 32;
				array[3] = id;
				for (int i = address; i < data.Length; i += 32)
				{
					array[4] = (byte)i;
					Array.Copy(data, i, array, 5, 32);
					UsbDevice.TxRxCommand(command, array, ref inData);
					var hidCommandsStatus = (HidCommandsStatus)inData[0];
					_ = inData[1];
					_ = inData[2];
					_ = inData[3];
					_ = inData[4];
					_ = inData[5];
					_ = inData[6];
					_ = inData[7];
					_ = inData[8];
					if (hidCommandsStatus != 0)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool Write(byte address, byte data)
		{
			return SKDeviceWrite(address, [data]);
		}

		private void BandwidthCheck(byte value)
		{
			if (FrequencyRf < 175000000m && value >= 8)
			{
				OnBandwidthLimitStatusChanged(LimitCheckStatusEnum.OUT_OF_RANGE, "This bandwidth setting is unsupported in lowest frequency band.");
			}
			else
			{
				OnBandwidthLimitStatusChanged(LimitCheckStatusEnum.OK, "");
			}
		}

		private void ClrAllIrq()
		{
			try
			{
				lock (SyncThread)
				{
					WriteRegister(registers["RegIrqFlags"], byte.MaxValue);
					ReadRegister(registers["RegIrqFlags"]);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		private void device_Dio0Changed(object sender, IoChangedEventArgs e)
		{
			lock (SyncThread)
			{
				if (!IsPacketHandlerStarted || (!e.Sate && !firstTransmit)) return;
				firstTransmit = false;
				ReadRegister(registers["RegIrqFlags"]);
				if (PacketModeTx)
				{
					OnPacketHandlerTransmitted();
					Console.WriteLine("IRQ: TxDoneIrq [0x{0:X02}] - Dio0Tx", registers["RegIrqFlags"].Value);
					ClrAllIrq();
					Console.WriteLine("IRQ: TxDoneIrq [0x{0:X02}] - Dio0Tx", registers["RegIrqFlags"].Value);
					PacketHandlerTransmit();
					return;
				}
				ReadRegister(registers["RegModemStat"]);
				ReadRegister(registers["RegHopChannel"]);
				ReadRegister(registers["RegRxHeaderCntValueMsb"]);
				ReadRegister(registers["RegRxHeaderCntValueLsb"]);
				ReadRegister(registers["RegRxPacketCntValueMsb"]);
				ReadRegister(registers["RegRxPacketCntValueLsb"]);
				ReadRegister(registers["RegRxNbBytes"]);
				ReadRegister(registers["RegFifoRxCurrentAddr"]);
				ReadRegister(registers["RegPktSnrValue"]);
				ReadRegister(registers["RegPktRssiValue"]);
				PacketHandlerReceive();
				Console.WriteLine("IRQ: RxDoneIrq [0x{0:X02}] - Dio0Rx", registers["RegIrqFlags"].Value);
				ClrAllIrq();
				Console.WriteLine("IRQ: RxDoneIrq [0x{0:X02}] - Dio0Rx", registers["RegIrqFlags"].Value);
			}
		}

		private void Device_Dio1Changed(object sender, IoChangedEventArgs e)
        {
            throw new NotSupportedException();
        }

        private void device_Dio2Changed(object sender, IoChangedEventArgs e)
        {
            throw new NotSupportedException();
        }

        private void device_Dio3Changed(object sender, IoChangedEventArgs e)
        {
            throw new NotSupportedException();
        }

        private void device_Dio4Changed(object sender, IoChangedEventArgs e)
        {
            throw new NotSupportedException();
        }

        private void device_Dio5Changed(object sender, IoChangedEventArgs e)
        {
            throw new NotSupportedException();
        }

        private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			string propertyName;
			if ((propertyName = e.PropertyName) == null || propertyName != nameof(Version)) return;
			PopulateRegisters();
			ReadRegisters();
		}

		private void FrequencyRfCheck(decimal value)
		{
			if (frequencyRfCheckDisable) return;
			if (value is < 137000000m or > 175000000m and < 410000000m or > 525000000m and < 820000000m or > 1024000000m)
			{
				var array = new[]
				{
					"[" + 137000000 + ", " + 175000000 + "]",
					"[" + 410000000 + ", " + 525000000 + "]",
					"[" + 820000000 + ", " + 1024000000 + "]"
				};
				OnFrequencyRfLimitStatusChanged(LimitCheckStatusEnum.OUT_OF_RANGE, "The RF frequency is out of range.\nThe valid ranges are:\n" + array[0] + "\n" + array[1] + "\n" + array[2]);
			}
			else
			{
				OnFrequencyRfLimitStatusChanged(LimitCheckStatusEnum.OK, "");
			}
		}

		private void ImageCalStart()
		{
			lock (SyncThread)
			{
				var operatingMode = Mode;
				try
				{
					byte data = 0;
					SetOperatingMode(OperatingModeEnum.Stdby);
					Read(59, ref data);
					Write(59, (byte)(data | 0x40u));
					var now = DateTime.Now;
					var flag = false;
					do
					{
						data = 0;
						Read(59, ref data);
						var now2 = DateTime.Now;
						flag = (now2 - now).TotalMilliseconds >= 1000.0;
					}
					while ((byte)(data & 0x20) == 32 && !flag);
					if (flag)
					{
						throw new Exception("Image calibration timeout.");
					}
				}
				finally
				{
					SetOperatingMode(operatingMode);
				}
			}
		}

		private void OcpTrimCheck(decimal value)
		{
			if (Pa20dBm && value < 150m)
			{
				var array = new[] { "[" + 150 + " mA, " + 240 + " mA]" };
				OnOcpTrimLimitStatusChanged(LimitCheckStatusEnum.OUT_OF_RANGE, "The Overload current protection is out of range.\nThe valid range is:\n" + array[0]);
			}
			else
			{
				OnOcpTrimLimitStatusChanged(LimitCheckStatusEnum.OK, "");
			}
		}

		private void OnBandwidthLimitStatusChanged(LimitCheckStatusEnum status, string message)
		{
            BandwidthLimitStatusChanged?.Invoke(this, new LimitCheckStatusEventArg(status, message));
        }

		private void OnConnected()
		{
            Connected?.Invoke(this, EventArgs.Empty);
        }

		private void OnDio0Changed(bool state)
		{
            Dio0Changed?.Invoke(this, new IoChangedEventArgs(state));
        }

		private void OnDio1Changed(bool state)
		{
            Dio1Changed?.Invoke(this, new IoChangedEventArgs(state));
        }

		private void OnDio2Changed(bool state)
		{
            Dio2Changed?.Invoke(this, new IoChangedEventArgs(state));
        }

		private void OnDio3Changed(bool state)
		{
            Dio3Changed?.Invoke(this, new IoChangedEventArgs(state));
        }

		private void OnDio4Changed(bool state)
		{
            Dio4Changed?.Invoke(this, new IoChangedEventArgs(state));
        }

		private void OnDio5Changed(bool state)
		{
            Dio5Changed?.Invoke(this, new IoChangedEventArgs(state));
        }

		private void OnDisconnected()
		{
            Disconected?.Invoke(this, EventArgs.Empty);
        }

		private void OnError(byte status, string message)
		{
            Error?.Invoke(this, new SemtechLib.General.Events.ErrorEventArgs(status, message));
        }

		private void OnFrequencyRfLimitStatusChanged(LimitCheckStatusEnum status, string message)
		{
            FrequencyRfLimitStatusChanged?.Invoke(this, new LimitCheckStatusEventArg(status, message));
        }

		private void OnOcpTrimLimitStatusChanged(LimitCheckStatusEnum status, string message)
		{
            OcpTrimLimitStatusChanged?.Invoke(this, new LimitCheckStatusEventArg(status, message));
        }

		private void OnPacketHandlerReceived()
		{
			if (PacketHandlerReceived == null) return;
			Console.WriteLine("Pkt#: {0} - RxPkt#: {1}", packetNumber, (registers["RegRxPacketCntValueMsb"].Value << 8) | registers["RegRxPacketCntValueLsb"].Value);
			PacketHandlerReceived(this, new PacketStatusEventArg(packetNumber, maxPacketNumber));
		}

		private void OnPacketHandlerStarted()
		{
            PacketHandlerStarted?.Invoke(this, EventArgs.Empty);
        }

		private void OnPacketHandlerStoped()
		{
            PacketHandlerStoped?.Invoke(this, EventArgs.Empty);
        }

		private void OnPacketHandlerTransmitted()
		{
			if (PacketHandlerTransmitted == null) return;
			Console.WriteLine("Pkt#: {0}", packetNumber);
			PacketHandlerTransmitted(this, new PacketStatusEventArg(packetNumber, maxPacketNumber));
		}

		private void OnPropertyChanged(string propName)
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

		private void packet_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(e.PropertyName);
		}

		private void PacketHandlerLog_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			string propertyName;
			if ((propertyName = e.PropertyName) != null && propertyName == "Enabled")
			{
				LogEnabled = PacketHandlerLog.Enabled;
			}
		}

		private void PacketHandlerReceive()
		{
			lock (SyncThread)
			{
				try
				{
					var data = Array.Empty<byte>();
					SetModeLeds(OperatingModeEnum.Rx);
					if (!PacketModeTx)
					{
						if (PacketModeRxSingle)
						{
							SetFifoAddrPtr(FifoRxBaseAddr);
							data = ((!ImplicitHeaderModeOn) ? new byte[RxNbBytes] : new byte[PayloadLength]);
							IsPacketHandlerStarted = false;
						}
						else if (ImplicitHeaderModeOn)
						{
							SetFifoAddrPtr(FifoRxCurrentAddr);
							data = new byte[PayloadLength];
						}
						else
						{
							SetFifoAddrPtr(FifoRxCurrentAddr);
							data = new byte[RxNbBytes];
						}
					}
					frameReceived = ReadFifo(ref data);
					Payload = data;
					if (!PayloadCrcError)
					{
						packetNumber++;
					}
					OnPacketHandlerReceived();
					pktLnaValues.Clear();
					pktRssiValues.Clear();
					if (!IsPacketHandlerStarted)
					{
						PacketHandlerStop();
					}
					SetModeLeds(OperatingModeEnum.Sleep);
				}
				catch (Exception ex)
				{
					PacketHandlerStop();
					OnError(1, ex.Message);
				}
			}
		}

		private void PacketHandlerStart()
		{
			lock (SyncThread)
			{
				try
				{
					SetModeLeds(OperatingModeEnum.Sleep);
					packetNumber = 0;
					SetOperatingMode(OperatingModeEnum.Sleep, isQuiet: true);
					ReadRegister(registers["RegRxNbBytes"]);
					ReadRegister(registers["RegRxPacketCntValueMsb"]);
					ReadRegister(registers["RegRxPacketCntValueLsb"]);
					ReadRegister(registers["RegPktSnrValue"]);
					ReadRegister(registers["RegPktRssiValue"]);
					ReadRegister(registers["RegFifoRxCurrentAddr"]);
					ReadRegister(registers["RegModemStat"]);
					ReadRegister(registers["RegRxHeaderCntValueMsb"]);
					ReadRegister(registers["RegRxHeaderCntValueLsb"]);
					ReadRegister(registers["RegHopChannel"]);
					if (PacketModeTx)
					{
						if (PayloadLength == 0)
						{
							MessageBox.Show("Message must be at least one byte long", "SX1276SKA-PacketHandler", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							throw new Exception("Message must be at least one byte long");
						}
						SetDioMapping(0, DioMappingEnum.DIO_MAP_01);
					}
					else if (PacketModeRxSingle)
					{
						SetDioMapping(0, DioMappingEnum.DIO_MAP_00);
					}
					else
					{
						SetDioMapping(0, DioMappingEnum.DIO_MAP_00);
						SetFifoAddrPtr(FifoRxBaseAddr);
					}
					frameTransmitted = false;
					frameReceived = false;
					if (PacketModeTx)
					{
						firstTransmit = true;
					}
					else if (PacketModeRxSingle)
					{
						SetOperatingMode(OperatingModeEnum.RxSingle, isQuiet: true);
						OnPacketHandlerReceived();
					}
					else
					{
						SetOperatingMode(OperatingModeEnum.Rx, isQuiet: true);
						OnPacketHandlerReceived();
					}
					PacketHandlerLog.Start();
					IsPacketHandlerStarted = true;
					OnPacketHandlerStarted();
				}
				catch (Exception ex)
				{
					OnError(1, ex.Message);
					PacketHandlerStop();
				}
			}
		}

		private void PacketHandlerStop()
		{
			try
			{
				lock (SyncThread)
				{
					IsPacketHandlerStarted = false;
					PacketHandlerLog.Stop();
					if (!PacketModeTx && PacketModeRxSingle)
					{
						PacketModeRxSingle = false;
						ReadRegister(registers["RegOpMode"]);
						OnPacketHandlerStoped();
					}
					Console.WriteLine("IRQ: AllIrq [0x{0:X02}] - Stop", registers["RegIrqFlags"].Value);
					ClrAllIrq();
					ReadRegister(registers["RegIrqFlags"]);
					SetOperatingMode(Mode);
					frameTransmitted = false;
					frameReceived = false;
					firstTransmit = false;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				OnPacketHandlerStoped();
			}
		}

		private void PacketHandlerTransmit()
		{
			lock (SyncThread)
			{
				try
				{
					SetModeLeds(OperatingModeEnum.Tx);
					if ((maxPacketNumber != 0 && packetNumber >= maxPacketNumber) || !IsPacketHandlerStarted)
					{
						PacketHandlerStop();
						return;
					}
					SetOperatingMode(OperatingModeEnum.Stdby, isQuiet: true);
					Thread.Sleep(100);
					SetFifoAddrPtr(FifoTxBaseAddr);
					packetNumber++;
					if (PacketUsePer)
					{
						if (PayloadLength < 9)
						{
							Array.Resize(ref payload, 9);
						}
						Payload[0] = 0;
						Payload[1] = (byte)(packetNumber >> 24);
						Payload[2] = (byte)(packetNumber >> 16);
						Payload[3] = (byte)(packetNumber >> 8);
						Payload[4] = (byte)packetNumber;
						Payload[5] = 80;
						Payload[6] = 69;
						Payload[7] = 82;
						Payload[8] = (byte)(Payload[0] + Payload[1] + Payload[2] + Payload[3] + Payload[4] + Payload[5] + Payload[6] + Payload[7]);
						OnPropertyChanged(nameof(Payload));
					}
					frameTransmitted = WriteFifo(Payload);
					SetOperatingMode(OperatingModeEnum.Tx, isQuiet: true);
				}
				catch (Exception ex)
				{
					PacketHandlerStop();
					OnError(1, ex.Message);
				}
				finally
				{
					SetModeLeds(OperatingModeEnum.Sleep);
				}
			}
		}

		private void PopulateRegisters()
		{
			if (IsOpen)
			{
				SetLoraOn(enable: true);
				byte data = 0;
				if (!Read(66, ref data))
				{
					throw new Exception("Unable to read register RegVersion");
				}
				if (!Read(66, ref data))
				{
					throw new Exception("Unable to read register RegVersion");
				}
				Version = new Version((data & 0xF0) >> 4, data & 0xF);
			}
			registers = [];
			byte b = 0;
			registers.Add(new Register("RegFifo", b++, 0u, readOnly: true, visible: true));
			registers.Add(new Register("RegOpMode", b++, 137u, readOnly: false, visible: true));
			for (var i = 2; i < 4; i++)
			{
				registers.Add(new Register("RegRes" + b.ToString("X02"), b++, 0u, readOnly: false, visible: true));
			}
			registers.Add(new Register("RegFdevMsb", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegRes05", b++, 82u, readOnly: false, visible: true));
			registers.Add(new Register("RegFrfMsb", b++, 228u, readOnly: false, visible: true));
			registers.Add(new Register("RegFrfMid", b++, 192u, readOnly: false, visible: true));
			registers.Add(new Register("RegFrfLsb", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegPaConfig", b++, 15u, readOnly: false, visible: true));
			registers.Add(new Register("RegPaRamp", b++, 25u, readOnly: false, visible: true));
			registers.Add(new Register("RegOcp", b++, 43u, readOnly: false, visible: true));
			registers.Add(new Register("RegLna", b++, 32u, readOnly: false, visible: true));
			registers.Add(new Register("RegFifoAddrPtr", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegFifoTxBaseAddr", b++, 128u, readOnly: false, visible: true));
			registers.Add(new Register("RegFifoRxBaseAddr", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegFifoRxCurrentAddr", b++, 0u, readOnly: true, visible: true));
			registers.Add(new Register("RegIrqFlagsMask", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegIrqFlags", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegRxNbBytes", b++, 0u, readOnly: true, visible: true));
			registers.Add(new Register("RegRxHeaderCntValueMsb", b++, 0u, readOnly: true, visible: true));
			registers.Add(new Register("RegRxHeaderCntValueLsb", b++, 0u, readOnly: true, visible: true));
			registers.Add(new Register("RegRxPacketCntValueMsb", b++, 0u, readOnly: true, visible: true));
			registers.Add(new Register("RegRxPacketCntValueLsb", b++, 0u, readOnly: true, visible: true));
			registers.Add(new Register("RegModemStat", b++, 0u, readOnly: true, visible: true));
			registers.Add(new Register("RegPktSnrValue", b++, 0u, readOnly: true, visible: true));
			registers.Add(new Register("RegPktRssiValue", b++, 0u, readOnly: true, visible: true));
			registers.Add(new Register("RegRssiValue", b++, 0u, readOnly: true, visible: true));
			registers.Add(new Register("RegHopChannel", b++, 0u, readOnly: true, visible: true));
			registers.Add(new Register("RegModemConfig1", b++, 114u, readOnly: false, visible: true));
			registers.Add(new Register("RegModemConfig2", b++, 112u, readOnly: false, visible: true));
			registers.Add(new Register("RegSymbTimeoutLsb", b++, 100u, readOnly: false, visible: true));
			registers.Add(new Register("RegPreambleMsb", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegPreambleLsb", b++, 8u, readOnly: false, visible: true));
			registers.Add(new Register("RegPayloadLength", b++, 1u, readOnly: false, visible: true));
			registers.Add(new Register("RegMaxPayloadLength", b++, 255u, readOnly: false, visible: true));
			registers.Add(new Register("RegHopPeriod", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegFifoRxByteAddr", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegModemConfig3", b++, 4u, readOnly: false, visible: true));
			for (var j = 39; j < 64; j++)
			{
				registers.Add(new Register("RegTest" + b.ToString("X02"), b++, 0u, readOnly: false, visible: true));
			}
			registers.Add(new Register("RegDioMapping1", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegDioMapping2", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegVersion", b++, 17u, readOnly: false, visible: true));
			for (var k = 67; k < 68; k++)
			{
				registers.Add(new Register("RegTest" + b.ToString("X02"), b++, 0u, readOnly: false, visible: true));
			}
			registers.Add(new Register("RegPllHop", b++, 45u, readOnly: false, visible: true));
			for (var l = 69; l < 75; l++)
			{
				registers.Add(new Register("RegTest" + b.ToString("X02"), b++, 0u, readOnly: false, visible: true));
			}
			registers.Add(new Register("RegTcxo", b++, 9u, readOnly: false, visible: true));
			registers.Add(new Register("RegTest4C", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegPaDac", b++, 132u, readOnly: false, visible: true));
			for (var m = 78; m < 91; m++)
			{
				registers.Add(new Register("RegTest" + b.ToString("X02"), b++, 0u, readOnly: false, visible: true));
			}
			registers.Add(new Register("RegFormerTemp", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegTest5C", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegBitrateFrac", b++, 0u, readOnly: false, visible: true));
			for (var n = 94; n < 97; n++)
			{
				registers.Add(new Register("RegTest" + b.ToString("X02"), b++, 0u, readOnly: false, visible: true));
			}
			registers.Add(new Register("RegAgcRef", b++, 25u, readOnly: false, visible: true));
			registers.Add(new Register("RegAgcThresh1", b++, 12u, readOnly: false, visible: true));
			registers.Add(new Register("RegAgcThresh2", b++, 75u, readOnly: false, visible: true));
			registers.Add(new Register("RegAgcThresh3", b++, 204u, readOnly: false, visible: true));
			for (var num = 101; num < 112; num++)
			{
				registers.Add(new Register("RegTest" + b.ToString("X02"), b++, 0u, readOnly: false, visible: true));
			}
			registers.Add(new Register("RegPll", b++, 208u, readOnly: false, visible: true));
			for (var num2 = 113; num2 < 128; num2++)
			{
				registers.Add(new Register("RegTest" + b.ToString("X02"), b++, 0u, readOnly: false, visible: true));
			}
			foreach (var register in registers)
			{
				register.PropertyChanged += registers_PropertyChanged;
			}
		}

		private bool Read(byte address, ref byte[] data)
		{
			return SKDeviceRead(address, ref data);
		}

		private bool ReadFifo(ref byte[] data)
		{
			var array = new byte[32];
			var num = data.Length;
			var num2 = 0;
			while (num > 0)
			{
				if (num > 32)
				{
					Array.Resize(ref array, 32);
					if (!Read(0, ref array))
					{
						return false;
					}
					Array.Copy(array, 0, data, num2, 32);
					num -= 32;
					num2 += 32;
				}
				else
				{
					Array.Resize(ref array, num);
					if (!Read(0, ref array))
					{
						return false;
					}
					Array.Copy(array, 0, data, num2, num);
					num -= num;
					num2 += num;
				}
			}
			return true;
		}

		private bool ReadRegister(Register r, ref byte data)
		{
			lock (SyncThread)
			{
				try
				{
					readLock++;
					if (IsOpen)
					{
						if (!Read((byte)r.Address, ref data))
						{
							throw new Exception("Unable to read register: " + r.Name);
						}
						r.Value = data;
					}
					else
					{
						UpdateRegisterValue(r);
					}
					return true;
				}
				catch (Exception ex)
				{
					OnError(1, ex.Message);
					return false;
				}
				finally
				{
					readLock--;
				}
			}
		}

		private bool ReadRegister(Register r)
		{
			byte data = 0;
			return ReadRegister(r, ref data);
		}

		private void registers_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			lock (SyncThread)
			{
				if (e.PropertyName != "Value")
				{
					return;
				}
				UpdateRegisterValue((Register)sender);
				if (readLock == 0 && !Write((byte)((Register)sender).Address, (byte)((Register)sender).Value))
				{
					OnError(1, "Unable to write register " + ((Register)sender).Name);
				}

				if (((Register)sender).Name != "RegOpMode") return;
				Console.WriteLine("RegOpMode (PC): 0x{0:X02}", ((Register)sender).Value);
				if (Mode == OperatingModeEnum.Rx || Mode == OperatingModeEnum.RxSingle || Mode == OperatingModeEnum.Cad)
				{
					ReadRegister(registers["RegLna"]);
					ReadRegister(registers["RegRssiValue"]);
				}
				ReadRegister(registers["RegIrqFlags"]);
			}
		}

		private void RegUpdateThread()
		{
			var num = 0;
			byte pinsState = 0;
			while (regUpdateThreadContinue)
			{
				if (!IsOpen)
				{
					Application.DoEvents();
					Thread.Sleep(10);
					continue;
				}
				try
				{
					lock (SyncThread)
					{
						if (SKGetPinsState(ref pinsState))
						{
							OnDio5Changed(state: (pinsState & 0x20) == 32);
							OnDio4Changed(state: (pinsState & 0x10) == 16);
							OnDio3Changed(state: (pinsState & 8) == 8);
							OnDio2Changed(state: (pinsState & 4) == 4);
							OnDio1Changed(state: (pinsState & 2) == 2);
							OnDio0Changed(state: (pinsState & 1) == 1);
						}
						if (!monitor)
						{
							Thread.Sleep(10);
							continue;
						}
						if (num % 10 == 0)
						{
							ReadRegister(registers["RegIrqFlags"]);
							ReadRegister(registers["RegModemStat"]);
							if (RxTimeout && PacketModeRxSingle && IsPacketHandlerStarted)
							{
								PacketHandlerStop();
							}
							byte data = 0;
							if (!Read(1, ref data))
							{
								throw new Exception("Unable to read register: RegOpMode");
							}
							if ((data & 0x80) == 0)
							{
								SetLoraOn(enable: true);
								WriteRegisters();
								ReadRegisters();
								if (IsPacketHandlerStarted && PacketModeTx)
								{
									firstTransmit = true;
									SetDioMapping(0, DioMappingEnum.DIO_MAP_01);
									SetOperatingMode(OperatingModeEnum.Tx);
								}
							}
							if (Mode == OperatingModeEnum.Cad)
							{
								ReadRegister(registers["RegOpMode"]);
							}
							if (Mode == OperatingModeEnum.Rx || (IsPacketHandlerStarted && !PacketModeTx))
							{
								if (isDebugOn && isReceiving)
								{
									ReadRegister(registers["RegLna"]);
								}
								if (!SpectrumOn)
								{
									var num2 = RfPaSwitchEnabled;
									if (num2 == 2)
									{
										RfPaSwitchSel = RfPaSwitchSelEnum.RF_IO_RFIO;
										ReadRegister(registers["RegRssiValue"]);
										RfPaSwitchSel = RfPaSwitchSelEnum.RF_IO_PA_BOOST;
										ReadRegister(registers["RegRssiValue"]);
									}
									else
									{
										ReadRegister(registers["RegRssiValue"]);
									}
								}
								else
								{
									SpectrumProcess();
								}
							}
						}
						if (num >= 200)
						{
							if (restartRx)
							{
								restartRx = false;
								ReadRegister(registers["RegLna"]);
							}
							num = 0;
						}
					}
				}
				catch (Exception ex)
				{
					OnError(1, ex.Message);
				}
				num++;
				Thread.Sleep(1);
			}
		}

		private void SetDefaultValues()
		{
			if (IsOpen)
			{
				if (!Write((byte)registers["RegModemConfig2"].Address, [115, 255]))
				{
					throw new Exception("Unable to write register: " + registers["RegModemConfig2"].Name);
				}
				if (!Write((byte)registers["RegModemConfig3"].Address, 4))
				{
					throw new Exception("Unable to write register: " + registers["RegModemConfig3"].Name);
				}
			}
			else
			{
				registers["RegModemConfig2"].Value = 115u;
				registers["RegSymbTimeoutLsb"].Value = 255u;
				registers["RegModemConfig3"].Value = 4u;
				ReadRegisters();
			}
		}

		private void SetFifoAddrPtr(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegFifoAddrPtr"].Value = value;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		private void SetLoraOn(bool enable)
		{
			if (enable)
			{
				SetOperatingMode(OperatingModeEnum.Sleep);
				byte data = 0;
				if (!Read(1, ref data))
				{
					throw new Exception("Unable to read LoRa mode");
				}
				if (!Write(1, (byte)((data | 0x80u) & 0xF8u)))
				{
					throw new Exception("Unable to write LoRa mode");
				}
				SetOperatingMode(OperatingModeEnum.Stdby);
			}
			else
			{
				SetOperatingMode(OperatingModeEnum.Sleep);
				byte data2 = 0;
				if (!Read(1, ref data2))
				{
					throw new Exception("Unable to read FSK mode");
				}
				if (!Write(1, (byte)(data2 & 0x78u)))
				{
					throw new Exception("Unable to write FSK mode");
				}
				SetOperatingMode(OperatingModeEnum.Stdby);
			}
		}

		private void SetModeLeds(OperatingModeEnum modeN)
		{
			if (Test) return;
			switch (modeN)
			{
				case OperatingModeEnum.Tx:
				case OperatingModeEnum.TxContinuous:
					SKSetPinState(7, 1);
					SKSetPinState(8, 0);
					break;
				case OperatingModeEnum.Rx:
				case OperatingModeEnum.RxSingle:
				case OperatingModeEnum.Cad:
					SKSetPinState(7, 0);
					SKSetPinState(8, 1);
					break;
				default:
					SKSetPinState(6, 1);
					SKSetPinState(7, 1);
					SKSetPinState(8, 1);
					break;
			}
		}

		private void SetOperatingMode(OperatingModeEnum value, bool isQuiet)
		{
			try
			{
				if (value is OperatingModeEnum.Tx or OperatingModeEnum.TxContinuous)
				{
					SKSetPinState(11, 0);
					SKSetPinState(12, 1);
				}
				else
				{
					SKSetPinState(11, 1);
					SKSetPinState(12, 0);
				}
				if (value == OperatingModeEnum.TxContinuous)
				{
					SetTxContinuousOn(value: true);
					value = OperatingModeEnum.Tx;
				}
				else
				{
					SetTxContinuousOn(value: false);
				}
				var b = (byte)registers["RegOpMode"].Value;
				b = (byte)(b & 0xF8u);
				b |= (byte)value;
				if (isQuiet)
				{
					lock (SyncThread)
					{
						if (!Write((byte)registers["RegOpMode"].Address, b))
						{
							throw new Exception("Unable to write register " + registers["RegOpMode"].Name);
						}
						if (Mode is OperatingModeEnum.Rx or OperatingModeEnum.RxSingle or OperatingModeEnum.Cad)
						{
							ReadRegister(registers["RegLna"]);
						}
					}
				}
				else
				{
					registers["RegOpMode"].Value = b;
				}
				Console.WriteLine("RegOpMode (SetOperatingMode): 0x{0:X02}", b);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		private void SetTxContinuousOn(bool value)
		{
			try
			{
				var b = (byte)registers["RegModemConfig2"].Value;
				b = (byte)(b & 0xF7u);
				b |= (byte)(value ? 8u : 0u);
				registers["RegModemConfig2"].Value = b;
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		private bool SKDeviceRead(byte address, ref byte[] data)
		{
			lock (SyncThread)
			{
                const byte command = 128;
                // var hidCommandsStatus = HidCommandsStatus.SX_ERROR;
				var inData = new byte[10 + data.Length];
				var outData = new byte[]
				{
					0,
					(byte)(data.Length + 2),
					(byte)data.Length,
					address
				};
				UsbDevice.TxRxCommand(command, outData, ref inData);
				var hidCommandsStatus = (HidCommandsStatus)inData[0];
				_ = inData[1];
				_ = inData[2];
				_ = inData[3];
				_ = inData[4];
				_ = inData[5];
				_ = inData[6];
				_ = inData[7];
				_ = inData[8];
				if (hidCommandsStatus != HidCommandsStatus.SX_OK || inData[9] != data.Length) return false;
				Array.Copy(inData, 10, data, 0, data.Length);
				return true;
			}
		}

		private bool SKDeviceWrite(byte address, byte[] data)
		{
			lock (SyncThread)
			{
                const byte command = 0x81;
                // var hidCommandsStatus = HidCommandsStatus.SX_ERROR;
				var inData = new byte[10];
				var array = new byte[data.Length + 2 + 2];
				array[0] = 0;
				array[1] = (byte)(data.Length + 2);
				array[2] = (byte)data.Length;
				array[3] = address;
				Array.Copy(data, 0, array, 4, data.Length);
				UsbDevice.TxRxCommand(command, array, ref inData);
				var hidCommandsStatus = (HidCommandsStatus)inData[0];
				_ = inData[1];
				_ = inData[2];
				_ = inData[3];
				_ = inData[4];
				_ = inData[5];
				_ = inData[6];
				_ = inData[7];
				_ = inData[8];
				return hidCommandsStatus == HidCommandsStatus.SX_OK;
			}
		}

		private bool SKGetPinsState(ref byte pinsState)
		{
			lock (SyncThread)
			{
                const byte command = 20;
                // var hidCommandsStatus = HidCommandsStatus.SX_ERROR;
				var inData = new byte[11];
				var array = new byte[2];
				var outData = array;
				UsbDevice.TxRxCommand(command, outData, ref inData);
				var hidCommandsStatus = (HidCommandsStatus)inData[0];
				_ = inData[1];
				_ = inData[2];
				_ = inData[3];
				_ = inData[4];
				_ = inData[5];
				_ = inData[6];
				_ = inData[7];
				_ = inData[8];
				if (hidCommandsStatus != HidCommandsStatus.SX_OK || inData[9] != 1) return false;
				pinsState = inData[10];
				return true;
			}
		}

		private bool SKGetVersion()
		{
			lock (SyncThread)
			{
                const byte b = 1;
                var hidCommandsStatus = HidCommandsStatus.SX_ERROR;
				var num = ulong.MaxValue;
				var inData = new byte[17];
				var array = new byte[2];
				var outData = array;
				try
				{
					UsbDevice.TxRxCommand(b, outData, ref inData);
					hidCommandsStatus = (HidCommandsStatus)inData[0];
					num = ((ulong)inData[1] << 56) | ((ulong)inData[2] << 48) | ((ulong)inData[3] << 40) | ((ulong)inData[4] << 32) | ((ulong)inData[5] << 24) | ((ulong)inData[6] << 16) | ((ulong)inData[7] << 8) | inData[8];
					if (hidCommandsStatus != HidCommandsStatus.SX_OK || inData[9] < 5) return false;
					Array.Copy(inData, 10, inData, 0, inData[9]);
					Array.Resize(ref inData, inData[9]);
					Encoding encoding = new ASCIIEncoding();
					var @string = encoding.GetString(inData);
					fwVersion = @string.Length > 5 ? new Version(@string.Remove(4, 1)) : new Version(@string);
					return true;
				}
				finally
				{
					Console.WriteLine("{0} ms: {1} with status {2}", num, Enum.GetName(typeof(HidCommands), (HidCommands)b), Enum.GetName(typeof(HidCommandsStatus), hidCommandsStatus));
				}
			}
		}

		private bool SKReset()
		{
			lock (SyncThread)
			{
                const byte b = 0;
                var hidCommandsStatus = HidCommandsStatus.SX_ERROR;
				var num = ulong.MaxValue;
				var inData = new byte[10];
				var array = new byte[2];
				var outData = array;
				try
				{
					if (!IsOpen) return false;
					UsbDevice.TxRxCommand(b, outData, ref inData);
					hidCommandsStatus = (HidCommandsStatus)inData[0];
					num = ((ulong)inData[1] << 56) | ((ulong)inData[2] << 48) | ((ulong)inData[3] << 40) | ((ulong)inData[4] << 32) | ((ulong)inData[5] << 24) | ((ulong)inData[6] << 16) | ((ulong)inData[7] << 8) | inData[8];
					return hidCommandsStatus == HidCommandsStatus.SX_OK;
				}
				finally
				{
					Console.WriteLine("{0} ms: {1} with status {2}", num, Enum.GetName(typeof(HidCommands), (HidCommands)b), Enum.GetName(typeof(HidCommandsStatus), hidCommandsStatus));
				}
			}
		}

		private bool SKSetPinState(byte pinId, byte state)
		{
			lock (SyncThread)
			{
                const byte command = 17;
                // var hidCommandsStatus = HidCommandsStatus.SX_ERROR;
				var inData = new byte[10];
				var outData = new byte[] { 0, 2, pinId, state };
				UsbDevice.TxRxCommand(command, outData, ref inData);
				var hidCommandsStatus = (HidCommandsStatus)inData[0];
				_ = inData[1];
				_ = inData[2];
				_ = inData[3];
				_ = inData[4];
				_ = inData[5];
				_ = inData[6];
				_ = inData[7];
				_ = inData[8];
				return hidCommandsStatus == HidCommandsStatus.SX_OK;
			}
		}

		private void SpectrumProcess()
		{
			var num = SpectrumFrequencyMin + SpectrumFrequencyStep * SpectrumFrequencyId;
			var data = (byte)((long)(num / frequencyStep) >> 16);
			var data2 = (byte)((long)(num / frequencyStep) >> 8);
			var data3 = (byte)(long)(num / frequencyStep);
			if (!Write((byte)registers["RegFrfMsb"].Address, data))
			{
				OnError(1, "Unable to write register " + registers["RegFrfMsb"].Name);
			}
			if (!Write((byte)registers["RegFrfMid"].Address, data2))
			{
				OnError(1, "Unable to write register " + registers["RegFrfMid"].Name);
			}
			if (!Write((byte)registers["RegFrfLsb"].Address, data3))
			{
				OnError(1, "Unable to write register " + registers["RegFrfLsb"].Name);
			}
			ReadRegister(registers["RegRssiValue"]);
			SpectrumFrequencyId++;
			if (SpectrumFrequencyId >= SpectrumNbFrequenciesMax)
			{
				SpectrumFrequencyId = 0;
			}
		}

		private void UpdateRegisterTable()
		{
			if (lowFrequencyMode == LowFrequencyModeOn) return;
			lowFrequencyMode = LowFrequencyModeOn;
			ReadRegisters();
		}

		private void UpdateRegisterValue(Register r)
		{
			switch (r.Name)
			{
			case "RegOpMode":
				LowFrequencyModeOn = ((r.Value >> 3) & 1) == 1;
				Console.WriteLine("RegOpMode (URV): 0x{0:X02}", r.Value);
				if ((registers["RegModemConfig2"].Value & 8) == 8 && (r.Value & 7) == 3)
				{
					Mode = OperatingModeEnum.TxContinuous;
				}
				else
				{
					Mode = (OperatingModeEnum)((int)r.Value & 7);
				}
				if (registers["RegPayloadLength"].Value != PayloadLength)
				{
					registers["RegPayloadLength"].Value = PayloadLength;
				}
				lock (SyncThread)
				{
					SetModeLeds(Mode);
					break;
				}
			case "RegFdevMsb":
				Band = (BandEnum)((int)(registers["RegFdevMsb"].Value >> 6) & 3);
				break;
			case "RegFrfMsb":
			case "RegFrfMid":
			case "RegFrfLsb":
				FrequencyRf = ((registers["RegFrfMsb"].Value << 16) | (registers["RegFrfMid"].Value << 8) | registers["RegFrfLsb"].Value) * FrequencyStep;
				BandwidthCheck(Bandwidth);
				break;
			case "RegPaConfig":
				PaSelect = (((r.Value & 0x80) == 128) ? PaSelectEnum.PA_BOOST : PaSelectEnum.RFO);
				if (PaSelect == PaSelectEnum.RFO)
				{
					maxOutputPower = 10.8m + 0.6m * ((r.Value >> 4) & 7u);
					outputPower = MaxOutputPower - (15.0m - (r.Value & 0xFu));
				}
				else if (!Pa20dBm)
				{
					maxOutputPower = 17m;
					outputPower = 17m - (15.0m - (r.Value & 0xFu));
				}
				else
				{
					maxOutputPower = 20m;
					outputPower = 20m - (15.0m - (r.Value & 0xFu));
				}
				OnPropertyChanged(nameof(MaxOutputPower));
				OnPropertyChanged(nameof(OutputPower));
				break;
			case "RegPaRamp":
				ForceTxBandLowFrequencyOn = ((r.Value >> 4) & 1) == 1;
				PaRamp = (PaRampEnum)((int)r.Value & 0xF);
				break;
			case "RegOcp":
				OcpOn = ((r.Value >> 5) & 1) == 1;
                    OcpTrim = (r.Value & 0x1F) switch
                    {
                        <= 15 => 45 + 5 * (r.Value & 0xF),
                        > 15 and <= 27 => -30 + 10 * (r.Value & 0x1F),
                        _ => 240.0m,
                    };
                    if (OcpOn)
				{
					OcpTrimCheck(OcpTrim);
				}
				break;
			case "RegLna":
				LnaGain = (LnaGainEnum)((int)(r.Value >> 5) & 7);
				ForceRxBandLowFrequencyOn = ((r.Value >> 2) & 1) == 1;
				LnaBoost = (r.Value & 3) == 3;
				if (isDebugOn && isReceiving)
				{
					pktLnaValues.Add((int)LnaGain);
				}
				break;
			case "RegFifoAddrPtr":
				FifoAddrPtr = (byte)r.Value;
				break;
			case "RegFifoTxBaseAddr":
				FifoTxBaseAddr = (byte)r.Value;
				break;
			case "RegFifoRxBaseAddr":
				FifoRxBaseAddr = (byte)r.Value;
				break;
			case "RegFifoRxCurrentAddr":
				FifoRxCurrentAddr = (byte)r.Value;
				OnPropertyChanged(nameof(FifoRxCurrentAddr));
				break;
			case "RegIrqFlagsMask":
				RxTimeoutMask = ((r.Value >> 7) & 1) == 1;
				RxDoneMask = ((r.Value >> 6) & 1) == 1;
				PayloadCrcErrorMask = ((r.Value >> 5) & 1) == 1;
				ValidHeaderMask = ((r.Value >> 4) & 1) == 1;
				TxDoneMask = ((r.Value >> 3) & 1) == 1;
				CadDoneMask = ((r.Value >> 2) & 1) == 1;
				FhssChangeChannelMask = ((r.Value >> 1) & 1) == 1;
				CadDetectedMask = (r.Value & 1) == 1;
				break;
			case "RegIrqFlags":
				RxTimeout = ((r.Value >> 7) & 1) == 1;
				OnPropertyChanged(nameof(RxTimeout));
				RxDone = ((r.Value >> 6) & 1) == 1;
				OnPropertyChanged(nameof(RxDone));
				PayloadCrcError = ((r.Value >> 5) & 1) == 1;
				OnPropertyChanged(nameof(PayloadCrcError));
				ValidHeader = ((r.Value >> 4) & 1) == 1;
				OnPropertyChanged(nameof(ValidHeader));
				TxDone = ((r.Value >> 3) & 1) == 1;
				OnPropertyChanged(nameof(TxDone));
				CadDone = ((r.Value >> 2) & 1) == 1;
				OnPropertyChanged(nameof(CadDone));
				FhssChangeChannel = ((r.Value >> 1) & 1) == 1;
				OnPropertyChanged(nameof(FhssChangeChannel));
				CadDetected = (r.Value & 1) == 1;
				OnPropertyChanged(nameof(CadDetected));
				if (isDebugOn && ValidHeader)
				{
					isReceiving = true;
				}
				if (isDebugOn && RxDone)
				{
					isReceiving = false;
				}
				break;
			case "RegRxNbBytes":
				RxNbBytes = (byte)r.Value;
				OnPropertyChanged(nameof(RxNbBytes));
				break;
			case "RegRxHeaderCntValueMsb":
			case "RegRxHeaderCntValueLsb":
				ValidHeaderCnt = (ushort)((registers["RegRxHeaderCntValueMsb"].Value << 8) | registers["RegRxHeaderCntValueLsb"].Value);
				OnPropertyChanged(nameof(ValidHeaderCnt));
				break;
			case "RegRxPacketCntValueMsb":
			case "RegRxPacketCntValueLsb":
				ValidPacketCnt = (ushort)((registers["RegRxPacketCntValueMsb"].Value << 8) | registers["RegRxPacketCntValueLsb"].Value);
				OnPropertyChanged(nameof(ValidPacketCnt));
				break;
			case "RegModemStat":
				RxPayloadCodingRate = (byte)((r.Value & 0xE0) >> 5);
				OnPropertyChanged(nameof(RxPayloadCodingRate));
				ModemClear = ((r.Value >> 4) & 1) == 1;
				OnPropertyChanged(nameof(ModemClear));
				HeaderInfoValid = ((r.Value >> 3) & 1) == 1;
				OnPropertyChanged(nameof(HeaderInfoValid));
				RxOnGoing = ((r.Value >> 2) & 1) == 1;
				OnPropertyChanged(nameof(RxOnGoing));
				SignalSynchronized = ((r.Value >> 1) & 1) == 1;
				OnPropertyChanged(nameof(SignalSynchronized));
				SignalDetected = (r.Value & 1) == 1;
				OnPropertyChanged(nameof(SignalDetected));
				break;
			case "RegPktSnrValue":
			{
				var b = (byte)r.Value;
				if ((b & 0x80) == 128)
				{
					PktSnrValue = (sbyte)(((~b + 1) & 0xFF) >> 2);
					PktSnrValue = (sbyte)(-PktSnrValue);
				}
				else
				{
					PktSnrValue = (sbyte)((b & 0xFF) >> 2);
				}
				if (!isDebugOn)
				{
					if (PktSnrValue < 0)
					{
						if (LowFrequencyModeOn)
						{
							PktRssiValue = -164 + registers["RegPktRssiValue"].Value + (registers["RegPktRssiValue"].Value >> 4) + PktSnrValue;
						}
						else
						{
							PktRssiValue = -157 + registers["RegPktRssiValue"].Value + (registers["RegPktRssiValue"].Value >> 4) + PktSnrValue;
						}
					}
					else if (LowFrequencyModeOn)
					{
						PktRssiValue = -164 + registers["RegPktRssiValue"].Value + (registers["RegPktRssiValue"].Value >> 4);
					}
					else
					{
						PktRssiValue = -157 + registers["RegPktRssiValue"].Value + (registers["RegPktRssiValue"].Value >> 4);
					}
					OnPropertyChanged(nameof(PktRssiValue));
				}
				OnPropertyChanged(nameof(PktSnrValue));
				break;
			}
			case "RegPktRssiValue":
				if (!isDebugOn)
				{
					if (PktSnrValue >= 0)
					{
						if (LowFrequencyModeOn)
						{
							PktRssiValue = -164 + r.Value + (r.Value >> 4);
						}
						else
						{
							PktRssiValue = -157 + r.Value + (r.Value >> 4);
						}
						OnPropertyChanged(nameof(PktRssiValue));
					}
				}
				else
				{
					PktRssiValue = r.Value;
					OnPropertyChanged(nameof(PktRssiValue));
				}
				break;
			case "RegRssiValue":
				prevRssiValue = RssiValue;
				if (!isDebugOn)
				{
					if (LowFrequencyModeOn)
					{
						RssiValue = -164 + r.Value;
					}
					else
					{
						RssiValue = -157 + r.Value;
					}
				}
				else
				{
					RssiValue = r.Value;
				}
				if (RfPaSwitchEnabled != 0)
				{
					switch (RfPaSwitchSel)
					{
						case RfPaSwitchSelEnum.RF_IO_RFIO:
						{
							if (RfPaSwitchEnabled == 1)
							{
								RfPaRssiValue = -127.7m;
							}
							RfIoRssiValue = RssiValue;
							OnPropertyChanged(nameof(RfIoRssiValue));
							break;
						}
						case RfPaSwitchSelEnum.RF_IO_PA_BOOST:
						{
							if (RfPaSwitchEnabled == 1)
							{
								RfIoRssiValue = -127.7m;
							}
							RfPaRssiValue = RssiValue;
							OnPropertyChanged(nameof(RfPaRssiValue));
							break;
						}
					}
				}
				SpectrumRssiValue = RssiValue;
				if (isDebugOn && isReceiving)
				{
					pktRssiValues.Add((double)RssiValue);
				}
				OnPropertyChanged(nameof(RssiValue));
				OnPropertyChanged("SpectrumData");
				break;
			case "RegHopChannel":
				PllTimeout = ((r.Value >> 7) & 1) == 1;
				OnPropertyChanged(nameof(PllTimeout));
				RxPayloadCrcOn = ((r.Value >> 6) & 1) == 1;
				OnPropertyChanged(nameof(RxPayloadCrcOn));
				HopChannel = (byte)(r.Value & 0x3Fu);
				OnPropertyChanged(nameof(HopChannel));
				break;
			case "RegModemConfig1":
				Bandwidth = (byte)((r.Value >> 4) & 0xFu);
				CodingRate = (byte)((r.Value >> 1) & 7u);
				ImplicitHeaderModeOn = (r.Value & 1) == 1;
				OnPropertyChanged(nameof(SymbolRate));
				OnPropertyChanged(nameof(SymbolTime));
				break;
			case "RegModemConfig2":
				SpreadingFactor = (byte)((r.Value >> 4) & 0xFu);
				OnPropertyChanged(nameof(SymbolRate));
				OnPropertyChanged(nameof(SymbolTime));
				TxContinuousModeOn = ((r.Value >> 3) & 1) == 1;
				PayloadCrcOn = ((r.Value >> 2) & 1) == 1;
				SymbTimeout = (((r.Value & 3) << 8) | registers["RegSymbTimeoutLsb"].Value) * SymbolTime;
				break;
			case "RegSymbTimeoutLsb":
				SymbTimeout = (((registers["RegModemConfig2"].Value & 3) << 8) | registers["RegSymbTimeoutLsb"].Value) * SymbolTime;
				break;
			case "RegPreambleMsb":
			case "RegPreambleLsb":
				PreambleLength = (ushort)(((registers["RegPreambleMsb"].Value << 8) | registers["RegPreambleLsb"].Value) + 4);
				break;
			case "RegPayloadLength":
				PayloadLength = (byte)r.Value;
				break;
			case "RegPayloadMaxLength":
				PayloadMaxLength = (byte)r.Value;
				break;
			case "RegHopPeriod":
				FreqHoppingPeriod = (byte)r.Value;
				break;
			case "RegModemConfig3":
				LowDatarateOptimize = ((r.Value >> 3) & 1) == 1;
				AgcAutoOn = ((r.Value >> 2) & 1) == 1;
				break;
			case "RegDioMapping1":
				Dio0Mapping = (DioMappingEnum)((int)(r.Value >> 6) & 3);
				Dio1Mapping = (DioMappingEnum)((int)(r.Value >> 4) & 3);
				Dio2Mapping = (DioMappingEnum)((int)(r.Value >> 2) & 3);
				Dio3Mapping = (DioMappingEnum)((int)r.Value & 3);
				break;
			case "RegDioMapping2":
				Dio4Mapping = (DioMappingEnum)((int)(r.Value >> 6) & 3);
				Dio5Mapping = (DioMappingEnum)((int)(r.Value >> 4) & 3);
				break;
			case "RegVersion":
				Version = new Version((int)((r.Value & 0xF0) >> 4), (int)(r.Value & 0xF));
				break;
			case "RegPllHop":
				FastHopOn = (r.Value & 0x80) == 128;
				break;
			case "RegTcxo":
				TcxoInputOn = (r.Value & 0x10) == 16;
				break;
			case "RegPaDac":
				Pa20dBm = (r.Value & 7) == 7;
				ReadRegister(registers["RegPaConfig"]);
				break;
			case "RegAgcRef":
				AgcReferenceLevel = (byte)(r.Value & 0x3Fu);
				OnPropertyChanged(nameof(AgcReference));
				OnPropertyChanged(nameof(AgcThresh1));
				OnPropertyChanged(nameof(AgcThresh2));
				OnPropertyChanged(nameof(AgcThresh3));
				OnPropertyChanged(nameof(AgcThresh4));
				OnPropertyChanged(nameof(AgcThresh5));
				break;
			case "RegAgcThresh1":
				AgcStep1 = (byte)(r.Value & 0x1Fu);
				OnPropertyChanged(nameof(AgcThresh1));
				OnPropertyChanged(nameof(AgcThresh2));
				OnPropertyChanged(nameof(AgcThresh3));
				OnPropertyChanged(nameof(AgcThresh4));
				OnPropertyChanged(nameof(AgcThresh5));
				break;
			case "RegAgcThresh2":
				AgcStep2 = (byte)(r.Value >> 4);
				AgcStep3 = (byte)(r.Value & 0xFu);
				OnPropertyChanged(nameof(AgcThresh2));
				OnPropertyChanged(nameof(AgcThresh3));
				OnPropertyChanged(nameof(AgcThresh4));
				OnPropertyChanged(nameof(AgcThresh5));
				break;
			case "RegAgcThresh3":
				AgcStep4 = (byte)(r.Value >> 4);
				AgcStep5 = (byte)(r.Value & 0xFu);
				OnPropertyChanged(nameof(AgcThresh4));
				OnPropertyChanged(nameof(AgcThresh5));
				break;
			case "RegPll":
				PllBandwidth = ((r.Value >> 6) + 1) * 75000;
				break;
			case "RegFifoRxByteAddr":
				break;
			}
		}

		private void usbDevice_Closed(object sender, EventArgs e)
		{
			spectrumOn = false;
			isOpen = false;
			regUpdateThreadContinue = false;
			OnDisconnected();
			OnError(0, "-");
		}

		private void usbDevice_Opened(object sender, EventArgs e)
		{
			isOpen = true;
			if (!SKGetVersion())
			{
				OnError(1, "Unable to read SK version.");
				return;
			}
			if (!SKReset())
			{
				OnError(1, "Unable to reset the SK");
				return;
			}
			regUpdateThreadContinue = true;
			regUpdateThread = new Thread(RegUpdateThread);
			regUpdateThread.Start();
			OnConnected();
		}

		private bool Write(byte address, byte[] data)
		{
			return SKDeviceWrite(address, data);
		}

		private bool WriteFifo(byte[] data)
		{
			var array = new byte[32];
			var num = data.Length;
			var num2 = 0;
			while (num > 0)
			{
				if (num > 32)
				{
					Array.Resize(ref array, 32);
					Array.Copy(data, num2, array, 0, 32);
					if (!Write(0, array))
					{
						return false;
					}
					num -= 32;
					num2 += 32;
				}
				else
				{
					Array.Resize(ref array, num);
					Array.Copy(data, num2, array, 0, num);
					if (!Write(0, array))
					{
						return false;
					}
					num -= num;
					num2 += num;
				}
			}
			return true;
		}

		private bool WriteRegister(Register r, byte data)
		{
			lock (SyncThread)
			{
				try
				{
					writeLock++;
					if (!Write((byte)r.Address, data))
					{
						throw new Exception("Unable to read register: " + r.Name);
					}
					return true;
				}
				catch (Exception ex)
				{
					OnError(1, ex.Message);
					return false;
				}
				finally
				{
					writeLock--;
				}
			}
		}

		private void WriteRegisters()
		{
			lock (SyncThread)
			{
				try
				{
					foreach (var register in registers)
					{
						if (register.Address != 0 && !Write((byte)register.Address, (byte)register.Value))
						{
							throw new Exception("Writing register " + register.Name);
						}
					}
				}
				catch (Exception ex)
				{
					OnError(1, ex.Message);
				}
			}
		}
	}
}
