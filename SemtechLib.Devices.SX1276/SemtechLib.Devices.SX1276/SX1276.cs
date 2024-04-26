using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SemtechLib.Devices.Common.Events;
using SemtechLib.Devices.Common.Hid;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.Devices.SX1276.Events;
using SemtechLib.Devices.SX1276.General;
using SemtechLib.General;

namespace SemtechLib.Devices.SX1276
{
	public sealed class SX1276 : IDevice //, INotifyPropertyChanged, IDisposable
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

		private const int NOISE_FIGURE_HF = 6;

		private const int NOISE_FIGURE_LF = 4;

		private const int PA_20_DBM_OCP_TRIM_MAX = 240;

		private const int PA_20_DBM_OCP_TRIM_MIN = 150;

		private const int RSSI_OFFSET_HF = -156;

		private const int RSSI_OFFSET_LF = -159;

		private readonly byte[] FifoData = new byte[66];

		private bool _crcOk;

		private string _deviceName;

		private bool _fifoEmpty;

		private bool _fifoFull;

		private bool _fifoLevel;

		private bool _fifoOverrun;

		private bool _imageCalRunning;

		private bool _isPacketHandlerStarted;

		private ILog _log;

		private bool _modeReady;

		private bool _packetSent;

		private bool _payloadReady;

		private bool _pllLock;

		private bool _preambleDetect;

		private decimal _rfIoRssiValue = -127.5m;

		private decimal _rfPaRssiValue = -127.5m;

		private bool _rssi;

		private decimal _rssiValue = -127.5m;

		private bool _rxReady;

		private decimal _spectrumRssiValue = -127.5m;

		private bool _syncAddressMatch;

		private bool _tempChange;

		private bool _tempMeasRunning;

		private decimal _tempValue = 165.0m;

		private bool _test;

		private bool _timeout;

		private bool _txReady;

		private bool afcAutoClearOn;

		private bool afcAutoOn;

		private decimal afcRxBw = 25000m;

		private decimal afcValue; // = 0.0m;

		private bool agcAutoOn = true;

		private byte agcReferenceLevel = 19;

		private byte agcStep1 = 14;

		private byte agcStep2 = 5;

		private byte agcStep3 = 11;

		private byte agcStep4 = 13;

		private byte agcStep5 = 11;

		private bool autoImageCalOn = true;

		private BandEnum band;

		private decimal bitrate = 4800m;

        private bool bitRateFdevCheckDisbale;

		private decimal bitrateFrac;

		private bool bitSyncOn = true;

		private ClockOutEnum clockOut = ClockOutEnum.CLOCK_OUT_111;

		private DioMappingEnum dio0Mapping;

		private DioMappingEnum dio1Mapping;

		private DioMappingEnum dio2Mapping;

		private DioMappingEnum dio3Mapping;

		private DioMappingEnum dio4Mapping;

		private DioMappingEnum dio5Mapping;

		private bool fastHopOn;

		private decimal fdev = 5000m;

		private decimal feiValue; // = 0.0m;

		private bool firstTransmit;

		private bool forceRxBandLowFrequencyOn;

		private bool forceTxBandLowFrequencyOn;

		private decimal formerTemp;

		private bool frameReceived;

		private bool frameTransmitted;

		private decimal frequencyRf = 915000000m;

        private bool frequencyRfCheckDisable;

		private decimal frequencyStep = 32000000m / (decimal)Math.Pow(2.0, 19.0);

		private decimal frequencyXo = 32000000m;

		private FromIdle fromIdle = FromIdle.TO_RX_ON_TMR1;

		private FromPacketReceived fromPacketReceived;

		private FromReceive fromReceive;

		private FromRxTimeout fromRxTimeout;

		private FromStart fromStart;

		private FromTransmit fromTransmit = FromTransmit.TO_RX;

        private Version fwVersion = new(0, 0);

		private IdleMode idleMode;

		private decimal interPacketRxDelay;

        private bool isDebugOn;

		private bool isOpen;

		private bool lnaBoost;

		private bool lnaBoostPrev;

		private LnaGainEnum lnaGain = LnaGainEnum.G1;

        private bool lowBatOn;

		private LowBatTrimEnum lowBatTrim = LowBatTrimEnum.Trim1_835;

		private bool lowFrequencyMode = true;

		private bool lowFrequencyModeOn = true;

		private LowPowerSelection lowPowerSelection = LowPowerSelection.TO_LPM;

		private bool mapPreambleDetect;

		private decimal maxOutputPower = 13.2m;

		private int maxPacketNumber;

		private OperatingModeEnum mode = OperatingModeEnum.Stdby;

		private byte modulationShaping;

        private ModulationTypeEnum modulationType;

		private bool monitor = true;

		private bool ocpOn = true;

		private decimal ocpTrim = 100m;

		private decimal ookAverageOffset; // = 0.0m;

		private OokAverageThreshFiltEnum ookAverageThreshFilt = OokAverageThreshFiltEnum.COEF_2;

        private byte ookFixedThreshold = 6;

		private OokPeakThreshDecEnum ookPeakThreshDec;

		private decimal ookPeakThreshStep = 0.5m;

		private OokThreshTypeEnum ookThreshType = OokThreshTypeEnum.Peak;

		private decimal outputPower = 13.2m;

		private bool pa20dBm;

		private Packet packet;

        private int packetNumber;

		private PaRampEnum paRamp = PaRampEnum.PaRamp_40;

		private PaSelectEnum paSelect;

		private decimal pllBandwidth = 300000m;

		private bool pngEnabled;

		private bool preambleDetectorOn = true;

		private byte preambleDetectorSize = 2;

		private byte preambleDetectorTol = 10;

		private bool prevAgcAutoOn;

		private LnaGainEnum prevLnaGain;

		private OperatingModeEnum prevMode;

		private ModulationTypeEnum prevModulationType;

		private bool prevMonitorOn;

        private OperatingModeEnum prevPngOpMode = OperatingModeEnum.Stdby;

		private int prevRfPaSwitchEnabled;

		private RfPaSwitchSelEnum prevRfPaSwitchSel;

		private decimal prevRssiThresh;

        private decimal prevRssiValue = -127.5m;

        private int readLock;

		private RegisterCollection registers;

        private Thread regUpdateThread;

        private bool regUpdateThreadContinue;

        private bool restartRx;

		private bool restartRxOnCollision;

		private int rfPaSwitchEnabled;

		private RfPaSwitchSelEnum rfPaSwitchSel;

		private decimal rssiCollisionThreshold = 10m;

		private decimal rssiOffset; // = 0.0m;

		private decimal rssiSmoothing = 8.0m;

		private decimal rssiThreshold = -127.5m;

		private decimal rxBw = 10416m;

		private RxTriggerEnum rxTriger;

		private bool SequencerStarted;

		private int spectrumFreqId;

		private decimal spectrumFreqSpan = 1000000m;

		private bool spectrumOn;

		private int spiSpeed = 2000000;

		private bool tcxoInputOn;

        private bool tempCalDone;

		private decimal tempDelta;

		private bool tempMonitorOff;

        private TempThresholdEnum tempThreshold = TempThresholdEnum.THRESH_10;

        private decimal tempValueRoom = 25.0m;

		private decimal timeoutRxPreamble;

		private decimal timeoutRxRssi;

		private decimal timeoutSignalSync;

		private byte timer1Coef = 245;

		private TimerResolution timer1Resolution;

		private byte timer2Coef = 32;

		private TimerResolution timer2Resolution;

		private Version version = new(0, 0);

        private int writeLock;

		public SX1276()
		{
			PropertyChanged += Device_PropertyChanged;
			DeviceName = "SX12xxEiger";
			UsbDevice = new HidDevice(1146, 11, DeviceName);
			UsbDevice.Opened += UsbDevice_Opened;
			UsbDevice.Closed += UsbDevice_Closed;
			Dio0Changed += Device_Dio0Changed;
			Dio1Changed += Device_Dio1Changed;
			Dio2Changed += Device_Dio2Changed;
			Dio3Changed += Device_Dio3Changed;
			Dio4Changed += Device_Dio4Changed;
			Dio5Changed += Device_Dio5Changed;
			PopulateRegisters();
		}

		public delegate void IoChangedEventHandler(object sender, IoChangedEventArgs e);

		public delegate void LimitCheckStatusChangedEventHandler(object sender, LimitCheckStatusEventArg e);

		public event LimitCheckStatusChangedEventHandler BitrateLimitStatusChanged;

		public event EventHandler Connected;

		public event IoChangedEventHandler Dio0Changed;

		public event IoChangedEventHandler Dio1Changed;

		public event IoChangedEventHandler Dio2Changed;

		public event IoChangedEventHandler Dio3Changed;

		public event IoChangedEventHandler Dio4Changed;

		public event IoChangedEventHandler Dio5Changed;

		public event EventHandler Disconected;

		public event SemtechLib.General.Events.ErrorEventHandler Error;

		public event LimitCheckStatusChangedEventHandler FdevLimitStatusChanged;

		public event LimitCheckStatusChangedEventHandler FrequencyRfLimitStatusChanged;

		public event LimitCheckStatusChangedEventHandler OcpTrimLimitStatusChanged;

		public event PacketStatusEventHandler PacketHandlerReceived;

		public event EventHandler PacketHandlerStarted;

		public event EventHandler PacketHandlerStoped;

		public event PacketStatusEventHandler PacketHandlerTransmitted;

		public event PropertyChangedEventHandler PropertyChanged;

		public event LimitCheckStatusChangedEventHandler SyncValueLimitChanged;

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

		public bool AfcAutoClearOn
		{
			get => afcAutoClearOn;
			set
			{
				afcAutoClearOn = value;
				OnPropertyChanged(nameof(AfcAutoClearOn));
			}
		}

		public bool AfcAutoOn
		{
			get => afcAutoOn;
			set
			{
				afcAutoOn = value;
				OnPropertyChanged(nameof(AfcAutoOn));
			}
		}

		public decimal AfcRxBw
		{
			get => afcRxBw;
			set
			{
				afcRxBw = value;
				OnPropertyChanged(nameof(AfcRxBw));
			}
		}

		public decimal AfcRxBwMax => ComputeRxBwMax();

		public decimal AfcRxBwMin => ComputeRxBwMin();

		public decimal AfcValue
		{
			get => afcValue;
			set
			{
				afcValue = value;
				OnPropertyChanged(nameof(AfcValue));
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

		public int AgcReference => (int)Math.Round(-174.0 + 10.0 * Math.Log10((double)(2m * RxBw)) + AgcReferenceLevel, MidpointRounding.AwayFromZero);

		public byte AgcReferenceLevel
		{
			get => agcReferenceLevel;
			set
			{
				agcReferenceLevel = value;
				OnPropertyChanged(nameof(AgcReferenceLevel));
				OnPropertyChanged(nameof(AgcReference));
				// OnPropertyChanged("AgcReferenceLevel");
				// OnPropertyChanged("AgcReference");
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
				// OnPropertyChanged("AgcStep1");
				// OnPropertyChanged("AgcThresh1");
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
				// OnPropertyChanged("AgcStep2");
				// OnPropertyChanged("AgcThresh2");
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
				// OnPropertyChanged("AgcStep3");
				// OnPropertyChanged("AgcThresh3");
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
				// OnPropertyChanged("AgcStep4");
				// OnPropertyChanged("AgcThresh4");
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
				// OnPropertyChanged("AgcStep5");
				// OnPropertyChanged("AgcThresh5");
			}
		}

		public int AgcThresh1 => AgcReference + agcStep1;

		public int AgcThresh2 => AgcThresh1 + agcStep2;

		public int AgcThresh3 => AgcThresh2 + agcStep3;

		public int AgcThresh4 => AgcThresh3 + agcStep4;

		public int AgcThresh5 => AgcThresh4 + agcStep5;

		public bool AutoImageCalOn
		{
			get => autoImageCalOn;
			set
			{
				autoImageCalOn = value;
				OnPropertyChanged(nameof(AutoImageCalOn));
			}
		}

		public BandEnum Band
		{
			get => band;
			set
			{
				band = value;
				OnPropertyChanged(nameof(Band));
			}
		}

		public decimal Bitrate
		{
			get => bitrate;
			set
			{
				bitrate = value;
				BitrateFdevCheck(value, fdev);
				OnPropertyChanged(nameof(Bitrate));
				OnPropertyChanged(nameof(Tbit));
			}
		}

		public decimal BitrateFrac
		{
			get => bitrateFrac;
			set
			{
				bitrateFrac = value;
				OnPropertyChanged(nameof(BitrateFrac));
			}
		}

		public bool BitSyncOn
		{
			get => bitSyncOn;
			set
			{
				bitSyncOn = value;
				OnPropertyChanged(nameof(BitSyncOn));
			}
		}

		public ClockOutEnum ClockOut
		{
			get => clockOut;
			set
			{
				clockOut = value;
				OnPropertyChanged(nameof(ClockOut));
			}
		}

		public bool CrcOk
		{
			get => _crcOk;
			set
			{
				if (value == _crcOk) return;
				_crcOk = value;
				OnPropertyChanged(nameof(CrcOk));
			}
		}

		public string DeviceName
		{
			get => _deviceName;
			set
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

		public decimal Fdev
		{
			get => fdev;
			set
			{
				fdev = value;
				BitrateFdevCheck(bitrate, value);
				OnPropertyChanged(nameof(Fdev));
			}
		}

		public decimal FeiValue
		{
			get => feiValue;
			set
			{
				feiValue = value;
				OnPropertyChanged(nameof(FeiValue));
			}
		}

		public bool FifoEmpty
		{
			get => _fifoEmpty;
			set
			{
				if (value == _fifoEmpty) return;
				_fifoEmpty = value;
				OnPropertyChanged(nameof(FifoEmpty));
			}
		}

		public bool FifoFull
		{
			get => _fifoFull;
			set
			{
				if (value == _fifoFull) return;
				_fifoFull = value;
				OnPropertyChanged(nameof(FifoFull));
			}
		}

		public bool FifoLevel
		{
			get => _fifoLevel;
			set
			{
				if (value == _fifoLevel) return;
				_fifoLevel = value;
				OnPropertyChanged(nameof(FifoLevel));
			}
		}

		public bool FifoOverrun
		{
			get => _fifoOverrun;
			set
			{
				if (value == _fifoOverrun) return;
				_fifoOverrun = value;
				OnPropertyChanged(nameof(FifoOverrun));
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

		public decimal FormerTemp
		{
			get => formerTemp;
			set
			{
				formerTemp = value;
				OnPropertyChanged(nameof(FormerTemp));
			}
		}

		public decimal FrequencyRf
		{
			get => frequencyRf;
			set
			{
				frequencyRf = value;
				OnPropertyChanged(nameof(FrequencyRf));
				OnPropertyChanged(nameof(SpectrumFrequencyMax));
				OnPropertyChanged(nameof(SpectrumFrequencyMin));
				FrequencyRfCheck(value);
			}
		}

		public decimal FrequencyStep
		{
			get => frequencyStep;
			set
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
				Fdev = ((registers["RegFdevMsb"].Value << 8) | registers["RegFdevLsb"].Value) * FrequencyStep;
				Bitrate = frequencyXo / (((registers["RegBitrateMsb"].Value << 8) | registers["RegBitrateLsb"].Value) + registers["RegBitrateFrac"].Value / 16.0m);
				OnPropertyChanged(nameof(FrequencyXo));
				// var num = 0;
				RxBw = ComputeRxBw(mant: ((registers["RegRxBw"].Value & 0x18) >> 3) switch
				{
					0u => 16, 
					1u => 20, 
					2u => 24, 
					_ => throw new Exception(message: "Invalid RxBwMant parameter"), 
				}, frequencyXo: value, mod: modulationType, exp: (int)(registers["RegRxBw"].Value & 7));
				// num = 0;
				AfcRxBw = ComputeRxBw(mant: ((registers["RegAfcBw"].Value & 0x18) >> 3) switch
				{
					0u => 16, 
					1u => 20, 
					2u => 24, 
					_ => throw new Exception(message: "Invalid RxBwMant parameter"), 
				}, frequencyXo: value, mod: modulationType, exp: (int)(registers["RegAfcBw"].Value & 7));
			}
		}

		public FromIdle FromIdle
		{
			get => fromIdle;
			set
			{
				fromIdle = value;
				OnPropertyChanged(nameof(FromIdle));
			}
		}

		public FromPacketReceived FromPacketReceived
		{
			get => fromPacketReceived;
			set
			{
				fromPacketReceived = value;
				OnPropertyChanged(nameof(FromPacketReceived));
			}
		}

		public FromReceive FromReceive
		{
			get => fromReceive;
			set
			{
				fromReceive = value;
				OnPropertyChanged(nameof(FromReceive));
			}
		}

		public FromRxTimeout FromRxTimeout
		{
			get => fromRxTimeout;
			set
			{
				fromRxTimeout = value;
				OnPropertyChanged(nameof(FromRxTimeout));
			}
		}

		public FromStart FromStart
		{
			get => fromStart;
			set
			{
				fromStart = value;
				OnPropertyChanged(nameof(FromStart));
			}
		}

		public FromTransmit FromTransmit
		{
			get => fromTransmit;
			set
			{
				fromTransmit = value;
				OnPropertyChanged(nameof(FromTransmit));
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

		public IdleMode IdleMode
		{
			get => idleMode;
			set
			{
				idleMode = value;
				OnPropertyChanged(nameof(IdleMode));
			}
		}

		public bool ImageCalRunning
		{
			get => _imageCalRunning;
			set
			{
				if (value == _imageCalRunning) return;
				_imageCalRunning = value;
				OnPropertyChanged(nameof(ImageCalRunning));
			}
		}

		public decimal InterPacketRxDelay
		{
			get => interPacketRxDelay;
			set
			{
				interPacketRxDelay = value;
				OnPropertyChanged(nameof(InterPacketRxDelay));
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
			private set
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

		public ILog Log
		{
			get => _log;
			set
			{
				if (Equals(value, _log)) return;
				_log = value;
				OnPropertyChanged(nameof(Log));
			}
		}

		public bool LowBat { get; private set; }

		public bool LowBatOn
		{
			get => lowBatOn;
			set
			{
				lowBatOn = value;
				OnPropertyChanged(nameof(LowBatOn));
			}
		}

		public LowBatTrimEnum LowBatTrim
		{
			get => lowBatTrim;
			set
			{
				lowBatTrim = value;
				OnPropertyChanged(nameof(LowBatTrim));
			}
		}

		public bool LowFrequencyModeOn
		{
			get => lowFrequencyModeOn;
			set
			{
				lowFrequencyModeOn = value;
				OnPropertyChanged(nameof(LowFrequencyModeOn));
			}
		}

		public LowPowerSelection LowPowerSelection
		{
			get => lowPowerSelection;
			set
			{
				lowPowerSelection = value;
				OnPropertyChanged(nameof(LowPowerSelection));
			}
		}

		public bool MapPreambleDetect
		{
			get => mapPreambleDetect;
			private set
			{
				mapPreambleDetect = value;
				OnPropertyChanged(nameof(MapPreambleDetect));
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
				OnPropertyChanged("DataMode");
			}
		}

		public bool ModeReady
		{
			get => _modeReady;
			set
			{
				if (value == _modeReady) return;
				_modeReady = value;
				OnPropertyChanged(nameof(ModeReady));
			}
		}

		public byte ModulationShaping
		{
			get => modulationShaping;
			set
			{
				modulationShaping = value;
				OnPropertyChanged(nameof(ModulationShaping));
			}
		}

		public ModulationTypeEnum ModulationType
		{
			get => modulationType;
			set
			{
				modulationType = value;
				OnPropertyChanged(nameof(ModulationType));
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

		public decimal OokAverageOffset
		{
			get => ookAverageOffset;
			set
			{
				ookAverageOffset = value;
				OnPropertyChanged(nameof(OokAverageOffset));
			}
		}

		public OokAverageThreshFiltEnum OokAverageThreshFilt
		{
			get => ookAverageThreshFilt;
			set
			{
				ookAverageThreshFilt = value;
				OnPropertyChanged(nameof(OokAverageThreshFilt));
			}
		}

        public byte OokFixedThreshold
		{
			get => ookFixedThreshold;
			private set
			{
				ookFixedThreshold = value;
				OnPropertyChanged(nameof(OokFixedThreshold));
			}
		}

		public OokPeakThreshDecEnum OokPeakThreshDec
		{
			get => ookPeakThreshDec;
			set
			{
				ookPeakThreshDec = value;
				OnPropertyChanged(nameof(OokPeakThreshDec));
			}
		}

		public decimal OokPeakThreshStep
		{
			get => ookPeakThreshStep;
			set
			{
				ookPeakThreshStep = value;
				OnPropertyChanged(nameof(OokPeakThreshStep));
			}
		}

		public OokThreshTypeEnum OokThreshType
		{
			get => ookThreshType;
			set
			{
				ookThreshType = value;
				OnPropertyChanged(nameof(OokThreshType));
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
			set
			{
				pa20dBm = value;
				OnPropertyChanged(nameof(Pa20dBm));
			}
		}

		public Packet Packet
		{
			get => packet;
			set
			{
				packet = value;
				packet.PropertyChanged += packet_PropertyChanged;
				OnPropertyChanged(nameof(Packet));
			}
		}

		public bool PacketSent
		{
			get => _packetSent;
			set
			{
				if (value == _packetSent) return;
				_packetSent = value;
				OnPropertyChanged(nameof(PacketSent));
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

		public bool PayloadReady
		{
			get => _payloadReady;
			set
			{
				if (value == _payloadReady) return;
				_payloadReady = value;
				OnPropertyChanged(nameof(PayloadReady));
			}
		}

		public decimal PllBandwidth
		{
			get => pllBandwidth;
			set
			{
				pllBandwidth = value;
				OnPropertyChanged(nameof(PllBandwidth));
			}
		}

		public bool PllLock
		{
			get => _pllLock;
			set
			{
				if (value == _pllLock) return;
				_pllLock = value;
				OnPropertyChanged(nameof(PllLock));
			}
		}

		public bool PngEnabled
		{
			get => pngEnabled;
			set
			{
				pngEnabled = value;
				if (value)
				{
					prevPngOpMode = Mode;
					SetOperatingMode(OperatingModeEnum.Tx);
				}
				else
				{
					SetOperatingMode(prevPngOpMode);
				}
				OnPropertyChanged(nameof(PngEnabled));
			}
		}

        public PnSequence PngSequence
		{
			get => Png.Pn;
			set
			{
				Png.Pn = value;
				OnPropertyChanged(nameof(PngSequence));
			}
		}

		public bool PreambleDetect
		{
			get => _preambleDetect;
			set
			{
				if (value == _preambleDetect) return;
				_preambleDetect = value;
				OnPropertyChanged(nameof(PreambleDetect));
			}
		}

		public bool PreambleDetectorOn
		{
			get => preambleDetectorOn;
			set
			{
				preambleDetectorOn = value;
				OnPropertyChanged(nameof(PreambleDetectorOn));
			}
		}

		public byte PreambleDetectorSize
		{
			get => preambleDetectorSize;
			set
			{
				preambleDetectorSize = value;
				OnPropertyChanged(nameof(PreambleDetectorSize));
			}
		}

		public byte PreambleDetectorTol
		{
			get => preambleDetectorTol;
			set
			{
				preambleDetectorTol = value;
				OnPropertyChanged(nameof(PreambleDetectorTol));
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

		public bool RestartRxOnCollision
		{
			get => restartRxOnCollision;
			set
			{
				restartRxOnCollision = value;
				OnPropertyChanged(nameof(RestartRxOnCollision));
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
                            case RfPaSwitchSelEnum.RF_IO_RFIO:
                                break;
                            case RfPaSwitchSelEnum.RF_IO_PA_BOOST:
                                break;
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

		public bool Rssi
		{
			get => _rssi;
			set
			{
				if (value == _rssi) return;
				_rssi = value;
				OnPropertyChanged(nameof(Rssi));
			}
		}

		public decimal RssiCollisionThreshold
		{
			get => rssiCollisionThreshold;
			set
			{
				rssiCollisionThreshold = value;
				OnPropertyChanged(nameof(RssiCollisionThreshold));
			}
		}

		public decimal RssiOffset
		{
			get => rssiOffset;
			set
			{
				rssiOffset = value;
				OnPropertyChanged(nameof(RssiOffset));
			}
		}

		public decimal RssiSmoothing
		{
			get => rssiSmoothing;
			set
			{
				rssiSmoothing = value;
				OnPropertyChanged(nameof(RssiSmoothing));
			}
		}

		public decimal RssiThreshold
		{
			get => rssiThreshold;
			set
			{
				rssiThreshold = value;
				OnPropertyChanged(nameof(RssiThreshold));
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

		public decimal RxBw
		{
			get => rxBw;
			set
			{
				rxBw = value;
				OnPropertyChanged(nameof(RxBw));
				OnPropertyChanged(nameof(SpectrumFrequencyStep));
				OnPropertyChanged(nameof(AgcReference));
			}
		}

		public decimal RxBwMax => ComputeRxBwMax();

		public decimal RxBwMin => ComputeRxBwMin();

		public bool RxReady
		{
			get => _rxReady;
			set
			{
				if (value == _rxReady) return;
				_rxReady = value;
				OnPropertyChanged(nameof(RxReady));
			}
		}

		public RxTriggerEnum RxTrigger
		{
			get => rxTriger;
			set
			{
				rxTriger = value;
				OnPropertyChanged(nameof(RxTrigger));
			}
		}

		public int SpectrumFrequencyId
		{
			get => spectrumFreqId;
			set
			{
				spectrumFreqId = value;
				OnPropertyChanged(nameof(SpectrumFrequencyId));
			}
		}

		public decimal SpectrumFrequencyMax => FrequencyRf + SpectrumFrequencySpan / 2.0m;

		public decimal SpectrumFrequencyMin => FrequencyRf - SpectrumFrequencySpan / 2.0m;

		public decimal SpectrumFrequencySpan
		{
			get => spectrumFreqSpan;
			set
			{
				spectrumFreqSpan = value;
				OnPropertyChanged(nameof(SpectrumFrequencySpan));
				OnPropertyChanged(nameof(SpectrumFrequencyMax));
				OnPropertyChanged(nameof(SpectrumFrequencyMin));
			}
		}

		public decimal SpectrumFrequencyStep => RxBw / 3.0m;

		public int SpectrumNbFrequenciesMax => (int)((SpectrumFrequencyMax - SpectrumFrequencyMin) / SpectrumFrequencyStep);

		public bool SpectrumOn
		{
			get => spectrumOn;
			set
			{
				spectrumOn = value;
				if (spectrumOn)
				{
					RfPaSwitchEnabled = 0;
					prevRssiThresh = RssiThreshold;
					SetRssiThresh(-127.5m);
					prevModulationType = ModulationType;
					SetModulationType(ModulationTypeEnum.OOK);
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
					SetRssiThresh(prevRssiThresh);
					SetModulationType(prevModulationType);
					SetLnaGain(prevLnaGain);
					SetAgcAutoOn(prevAgcAutoOn);
					SetOperatingMode(prevMode);
					Monitor = prevMonitorOn;
				}
				OnPropertyChanged(nameof(SpectrumOn));
			}
		}

		public decimal SpectrumRssiValue
		{
			get => _spectrumRssiValue;
			set
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

		public bool SyncAddressMatch
		{
			get => _syncAddressMatch;
			set
			{
				if (value == _syncAddressMatch) return;
				_syncAddressMatch = value;
				OnPropertyChanged(nameof(SyncAddressMatch));
			}
		}

		public decimal Tbit => 1m / Bitrate;

		public bool TcxoInputOn
		{
			get => tcxoInputOn;
			set
			{
				tcxoInputOn = value;
				OnPropertyChanged(nameof(TcxoInputOn));
			}
		}

        public bool TempCalDone
		{
			get => tempCalDone;
			set
			{
				tempCalDone = value;
				OnPropertyChanged(nameof(TempCalDone));
			}
		}

		public bool TempChange
		{
			get => _tempChange;
			set
			{
				if (value == _tempChange) return;
				_tempChange = value;
				OnPropertyChanged(nameof(TempChange));
			}
		}

		public decimal TempDelta
		{
			get => tempDelta;
			set
			{
				tempDelta = value;
				OnPropertyChanged(nameof(TempDelta));
			}
		}

		public bool TempMeasRunning
		{
			get => _tempMeasRunning;
			private set
			{
				if (value == _tempMeasRunning) return;
				_tempMeasRunning = value;
				OnPropertyChanged(nameof(TempMeasRunning));
			}
		}

		public bool TempMonitorOff
		{
			get => tempMonitorOff;
			set
			{
				tempMonitorOff = value;
				OnPropertyChanged(nameof(TempMonitorOff));
			}
		}

		public TempThresholdEnum TempThreshold
		{
			get => tempThreshold;
			set
			{
				tempThreshold = value;
				OnPropertyChanged(nameof(TempThreshold));
			}
		}

		public decimal TempValue
		{
			get => _tempValue;
			set
			{
				if (value == _tempValue) return;
				_tempValue = value;
				OnPropertyChanged(nameof(TempValue));
			}
		}

		public decimal TempValueRoom
		{
			get => tempValueRoom;
			set
			{
				tempValueRoom = value;
				OnPropertyChanged(nameof(TempValueRoom));
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

		public bool Timeout
		{
			get => _timeout;
			set
			{
				if (value == _timeout) return;
				_timeout = value;
				OnPropertyChanged(nameof(Timeout));
			}
		}

		public decimal TimeoutRxPreamble
		{
			get => timeoutRxPreamble;
			set
			{
				timeoutRxPreamble = value;
				OnPropertyChanged(nameof(TimeoutRxPreamble));
			}
		}

		public decimal TimeoutRxRssi
		{
			get => timeoutRxRssi;
			set
			{
				timeoutRxRssi = value;
				OnPropertyChanged(nameof(TimeoutRxRssi));
			}
		}

		public decimal TimeoutSignalSync
		{
			get => timeoutSignalSync;
			set
			{
				timeoutSignalSync = value;
				OnPropertyChanged(nameof(TimeoutSignalSync));
			}
		}

		public byte Timer1Coef
		{
			get => timer1Coef;
			set
			{
				timer1Coef = value;
				OnPropertyChanged(nameof(Timer1Coef));
			}
		}

		public TimerResolution Timer1Resolution
		{
			get => timer1Resolution;
			set
			{
				timer1Resolution = value;
				OnPropertyChanged(nameof(Timer1Resolution));
			}
		}

		public byte Timer2Coef
		{
			get => timer2Coef;
			set
			{
				timer2Coef = value;
				OnPropertyChanged(nameof(Timer2Coef));
			}
		}

		public TimerResolution Timer2Resolution
		{
			get => timer2Resolution;
			set
			{
				timer2Resolution = value;
				OnPropertyChanged(nameof(Timer2Resolution));
			}
		}

		public bool TxReady
		{
			get => _txReady;
			set
			{
				if (value == _txReady) return;
				_txReady = value;
				OnPropertyChanged(nameof(TxReady));
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

		private decimal[] OoPeakThreshStepTable { get; } = [0.5m, 1.0m, 1.5m, 2.0m, 3.0m, 4.0m, 5.0m, 6.0m];

		private PseudoNoiseGenerator Png { get; set; } = new();

		private object SyncThread { get; } = new();

		private decimal TempValueCal { get; set; } = 165.0m;

		private HidDevice UsbDevice { get; }

		public static decimal ComputeRxBw(decimal frequencyXo, ModulationTypeEnum mod, int mant, int exp)
		{
			if (mod == ModulationTypeEnum.FSK)
			{
				return frequencyXo / (mant * (decimal)Math.Pow(2.0, exp + 2));
			}
			return frequencyXo / (mant * (decimal)Math.Pow(2.0, exp + 3));
		}

		public static decimal[] ComputeRxBwFreqTable(decimal frequencyXo, ModulationTypeEnum mod)
		{
			var array = new decimal[24];
			// var num = 0;
			// var num2 = 0;
			var num3 = 0;
			for (var num = 0; num < 8; num++)
			{
				for (var num2 = 16; num2 <= 24; num2 += 4)
				{
					if (mod == ModulationTypeEnum.FSK)
					{
						ref var reference = ref array[num3++];
						reference = frequencyXo / (num2 * (decimal)Math.Pow(2.0, num + 2));
					}
					else
					{
						ref var reference2 = ref array[num3++];
						reference2 = frequencyXo / (num2 * (decimal)Math.Pow(2.0, num + 3));
					}
				}
			}
			return array;
		}

		public static void ComputeRxBwMantExp(decimal frequencyXo, ModulationTypeEnum mod, decimal value, ref int mant, ref int exp)
		{
			// var num = 0;
			// var num2 = 0;
			// var num3 = 0m;
			var num4 = 10000000m;
			for (var num = 0; num < 8; num++)
			{
				for (var num2 = 16; num2 <= 24; num2 += 4)
				{
					var num3 = ((mod != 0) ? (frequencyXo / (num2 * (decimal)Math.Pow(2.0, num + 3))) : (frequencyXo / (num2 * (decimal)Math.Pow(2.0, num + 2))));
					if (Math.Abs(num3 - value) >= num4) continue;
					num4 = Math.Abs(num3 - value);
					mant = num2;
					exp = num;
				}
			}
		}

		public bool Close()
		{
			if (!isOpen && UsbDevice is not { IsOpen: true }) return true;
			UsbDevice.Close();
			isOpen = false;
			return true;
		}

		public void ClrFifoOverrunIrq()
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlags2"].Value;
					b = (byte)(b & 0xEFu);
					b = (byte)(b | 0x10u);
					registers["RegIrqFlags2"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrLowBatIrq()
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlags2"].Value;
					b = (byte)(b & 0xFEu);
					b = (byte)(b | 1u);
					registers["RegIrqFlags2"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrPreambleDetectIrq()
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlags1"].Value;
					b = (byte)(b & 0xFDu);
					b = (byte)(b | 2u);
					registers["RegIrqFlags1"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrRssiIrq()
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlags1"].Value;
					b = (byte)(b & 0xF7u);
					b = (byte)(b | 8u);
					registers["RegIrqFlags1"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrSyncAddressMatchIrq()
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegIrqFlags1"].Value;
					b = (byte)(b & 0xFEu);
					b = (byte)(b | 1u);
					registers["RegIrqFlags1"].Value = b;
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

		public void ImageCalStart()
		{
			lock (SyncThread)
			{
				var operatingMode = Mode;
				try
				{
					byte data = 0;
					SetOperatingMode(OperatingModeEnum.Stdby);
					ReadRegister(registers["RegImageCal"], ref data);
					WriteRegister(registers["RegImageCal"], (byte)(data | 0x40u));
					ImageCalRunning = false;
					OnPropertyChanged("ImageCalRunning");
					var now = DateTime.Now;
					var flag = false;
					do
					{
						data = 0;
						ReadRegister(registers["RegImageCal"], ref data);
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
					ReadRegister(registers["RegTemp"]);
					ReadRegister(registers["RegFormerTemp"]);
					SetOperatingMode(operatingMode);
				}
			}
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
			var saveData = "";
			try
			{
				while (streamReader.ReadLine() is { } text)
				{
					if (text[0] == '#')
					{
						num++;
						continue;
					}
					if (text[0] != 'R' && text[0] != 'P' && text[0] != 'X')
					{
						throw new Exception("At line " + num + ": A configuration line must start either by\n\"#\" for comments\nor a\n\"R\" for the register name.\nor a\n\"P\" for packet settings.\nor a\n\"X\" for crystal frequency.");
					}
					var array = text.Split('\t');
					if (array.Length != 4)
					{
						if (array.Length != 2)
						{
							throw new Exception("At line " + num + ": The number of columns is " + array.Length + " and it should be 4 or 2.");
						}
						if (array[0] == "PKT")
						{
							saveData = array[1];
						}
						else
						{
							if (array[0] != "XTAL")
							{
								throw new Exception("At line " + num + ": Invalid Packet or XTAL freuqncy");
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
				packet.SetSaveData(saveData);
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

		public void RcCalTrig()
		{
			lock (SyncThread)
			{
				byte data = 0;
				if (Mode == OperatingModeEnum.Stdby)
				{
					ReadRegister(registers["RegOsc"], ref data);
					WriteRegister(registers["RegOsc"], (byte)(data | 8u));
					var now = DateTime.Now;
					var flag = false;
					do
					{
						data = 0;
						ReadRegister(registers["RegOsc"], ref data);
						var now2 = DateTime.Now;
						flag = (now2 - now).TotalMilliseconds >= 1000.0;
					}
					while ((byte)(data & 8) == 8 && !flag);
					if (flag)
					{
						throw new Exception("RC oscillator calibration timeout.");
					}
					return;
				}
				MessageBox.Show("The chip must be in Standby mode in order to calibrate the RC oscillator!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				throw new Exception("The chip must be in Standby mode in order to calibrate the RC oscillator!");
			}
		}

		public bool Read(byte address, ref byte data)
		{
			var array = new byte[1];
			var data2 = array;
			if (!SKDeviceRead(address, ref data2)) return false;
			data = data2[0];
			return true;
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
					tempCalDone = false;
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
					SetLoraOn(enable: false);
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
				streamWriter.WriteLine("PKT\t{0}", packet.GetSaveData());
				streamWriter.WriteLine("XTAL\t{0}", FrequencyXo);
			}
			finally
			{
				streamWriter.Close();
			}
		}

		public void SetAddressFiltering(AddressFilteringEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPacketConfig1"].Value;
					b = (byte)(b & 0xF9u);
					b |= (byte)(((byte)value & 3) << 1);
					registers["RegPacketConfig1"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetAfcAutoClearOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegAfcFei"].Value;
					b = (byte)(b & 0xFEu);
					b |= (byte)(value ? 1u : 0u);
					registers["RegAfcFei"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetAfcAutoOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegRxConfig"].Value;
					b = (byte)(b & 0xEFu);
					b |= (byte)(value ? 16u : 0u);
					registers["RegRxConfig"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetAfcClear()
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegAfcFei"].Value;
					b = (byte)(b | 2u);
					registers["RegAfcFei"].Value = b;
					ReadRegister(registers["RegAfcMsb"]);
					ReadRegister(registers["RegAfcLsb"]);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetAfcRxBw(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegAfcBw"].Value;
					b = (byte)(b & 0xE0u);
					var exp = 0;
					var mant = 0;
					ComputeRxBwMantExp(frequencyXo, ModulationType, value, ref mant, ref exp);
					b = mant switch
					{
						16 => (byte)(b | (byte)((uint)exp & 7u)), 
						20 => (byte)(b | (byte)(8u | ((uint)exp & 7u))), 
						24 => (byte)(b | (byte)(0x10u | ((uint)exp & 7u))), 
						_ => throw new Exception("Invalid RxBwMant parameter"), 
					};
					registers["RegAfcBw"].Value = b;
				}
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
					var b = (byte)registers["RegRxConfig"].Value;
					b = (byte)(b & 0xF7u);
					b |= (byte)(value ? 8u : 0u);
					registers["RegRxConfig"].Value = b;
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

		public void SetAgcStart()
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegAfcFei"].Value;
					b = (byte)(b | 0x10u);
					registers["RegAfcFei"].Value = b;
					ReadRegister(registers["RegLna"]);
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
                    Register register = id switch
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

		public void SetAutoRestartRxOn(AutoRestartRxEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegSyncConfig"].Value;
					b = (byte)(b & 0x3Fu);
					b |= (byte)((byte)value << 6);
					registers["RegSyncConfig"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetAutoRxRestartDelay(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegRxDelay"].Value = (uint)Math.Round(value / 1000m / (4m * Tbit), MidpointRounding.AwayFromZero);
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

		public void SetBarkerSyncLossThresh(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegRes18"].Value = (byte)(value & 0x7Fu);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetBarkerSyncThresh(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegRes17"].Value = (byte)(value & 0x7Fu);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetBarkerTrackingThresh(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegRes19"].Value = (byte)(value & 0x7Fu);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetBeaconOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPacketConfig2"].Value;
					b = (byte)(b & 0xF7u);
					b |= (byte)(value ? 8u : 0u);
					registers["RegPacketConfig2"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetBitrate(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					byte value2;
					byte value3;
					if (ModulationType == ModulationTypeEnum.FSK)
					{
						value2 = (byte)((long)Math.Round(frequencyXo / value - BitrateFrac / 16m, MidpointRounding.AwayFromZero) >> 8);
						value3 = (byte)(long)Math.Round(frequencyXo / value - BitrateFrac / 16m, MidpointRounding.AwayFromZero);
					}
					else
					{
						value2 = (byte)((long)Math.Round(frequencyXo / value, MidpointRounding.AwayFromZero) >> 8);
						value3 = (byte)(long)Math.Round(frequencyXo / value, MidpointRounding.AwayFromZero);
					}
					bitRateFdevCheckDisbale = true;
					registers["RegBitrateMsb"].Value = value2;
					bitRateFdevCheckDisbale = false;
					registers["RegBitrateLsb"].Value = value3;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetBitrateFrac(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegBitrateFrac"].Value;
					b = (byte)(b & 0xF0u);
					b |= (byte)((byte)value & 0xFu);
					registers["RegBitrateFrac"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetBitSyncOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegOokPeak"].Value;
					b = (byte)(b & 0xDFu);
					b |= (byte)(value ? 32u : 0u);
					registers["RegOokPeak"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetBroadcastAddress(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegBroadcastAdrs"].Value = value;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetClockOut(ClockOutEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var value2 = registers["RegOsc"].Value;
					value2 &= 0xF8u;
					value2 |= (uint)(value & ClockOutEnum.CLOCK_OUT_111);
					registers["RegOsc"].Value = value2;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetCrcAutoClearOff(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPacketConfig1"].Value;
					b = (byte)(b & 0xF7u);
					b |= (byte)(value ? 8u : 0u);
					registers["RegPacketConfig1"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetCrcIbmOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPacketConfig1"].Value;
					b = (byte)(b & 0xFEu);
					b |= (byte)(value ? 1u : 0u);
					registers["RegPacketConfig1"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetCrcOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPacketConfig1"].Value;
					b = (byte)(b & 0xEFu);
					b |= (byte)(value ? 16u : 0u);
					registers["RegPacketConfig1"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetDataMode(DataModeEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPacketConfig2"].Value;
					b = (byte)(b & 0xBFu);
					b |= (byte)((byte)value << 6);
					registers["RegPacketConfig2"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetDccFastInitOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegRxBw"].Value;
					b = (byte)(b & 0xBFu);
					b |= (byte)(value ? 64u : 0u);
					registers["RegRxBw"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetDccForceOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegRxBw"].Value;
					b = (byte)(b & 0xDFu);
					b |= (byte)(value ? 32u : 0u);
					registers["RegRxBw"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetDcFree(DcFreeEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPacketConfig1"].Value;
					b = (byte)(b & 0x9Fu);
					b |= (byte)(((byte)value & 3) << 5);
					registers["RegPacketConfig1"].Value = b;
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
                    Register register = id switch
                    {
                        0 or 1 or 2 or 3 => registers["RegDioMapping1"],
                        4 or 5 => registers["RegDioMapping2"],
                        _ => throw new Exception(message: "Invalid DIO ID!"),
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

		public void SetDioPreambleIrqOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegDioMapping2"].Value;
					b = (byte)(b & 0xFEu);
					b |= (byte)(value ? 1u : 0u);
					registers["RegDioMapping2"].Value = b;
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

		public void SetFdev(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					// var b = (byte)(registers["RegFdevMsb"].Value & 0x3Fu);
					// var b2 = (byte)registers["RegFdevLsb"].Value;
					var b = (byte)((long)(value / frequencyStep) >> 8);
					var b2 = (byte)(long)(value / frequencyStep);
					bitRateFdevCheckDisbale = true;
					registers["RegFdevMsb"].Value = (byte)(b & 0x3Fu);
					bitRateFdevCheckDisbale = false;
					registers["RegFdevLsb"].Value = b2;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFeiRange(FeiRangeEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegAfcFei"].Value;
					b = (byte)(b & 0xF7u);
					b |= (byte)((byte)value << 3);
					registers["RegAfcFei"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFeiRead()
		{
			try
			{
				lock (SyncThread)
				{
					ReadRegister(registers["RegFeiMsb"]);
					ReadRegister(registers["RegFeiLsb"]);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFifoFillCondition(FifoFillConditionEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegSyncConfig"].Value;
					b = (byte)(b & 0xF7u);
					b |= (byte)((byte)value << 3);
					registers["RegSyncConfig"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFifoThreshold(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegFifoThresh"].Value;
					b = (byte)(b & 0x80u);
					b |= (byte)(value & 0x7Fu);
					registers["RegFifoThresh"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFillFifo()
		{
			try
			{
				lock (SyncThread)
				{
					WriteFifo(packet.ToArray());
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

		public void SetFromIdle(FromIdle value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegSeqConfig1"].Value;
					b = (byte)(b & 0xFDu);
					b |= (byte)((byte)value << 1);
					registers["RegSeqConfig1"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFromPacketReceived(FromPacketReceived value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegSeqConfig2"].Value;
					b = (byte)(b & 0xF8u);
					b |= (byte)value;
					registers["RegSeqConfig2"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFromReceive(FromReceive value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegSeqConfig2"].Value;
					b = (byte)(b & 0x1Fu);
					b |= (byte)((byte)value << 5);
					registers["RegSeqConfig2"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFromRxTimeout(FromRxTimeout value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegSeqConfig2"].Value;
					b = (byte)(b & 0xE7u);
					b |= (byte)((byte)value << 3);
					registers["RegSeqConfig2"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFromStart(FromStart value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegSeqConfig1"].Value;
					b = (byte)(b & 0xE7u);
					b |= (byte)((byte)value << 3);
					registers["RegSeqConfig1"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFromTransmit(FromTransmit value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegSeqConfig1"].Value;
					b = (byte)(b & 0xFEu);
					b |= (byte)value;
					registers["RegSeqConfig1"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetIdleMode(IdleMode value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegSeqConfig1"].Value;
					b = (byte)(b & 0xDFu);
					b |= (byte)((byte)value << 5);
					registers["RegSeqConfig1"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetIoHomeOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPacketConfig2"].Value;
					b = (byte)(b & 0xDFu);
					b |= (byte)(value ? 32u : 0u);
					registers["RegPacketConfig2"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetIoHomePwrFrameOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPacketConfig2"].Value;
					b = (byte)(b & 0xEFu);
					b |= (byte)(value ? 16u : 0u);
					registers["RegPacketConfig2"].Value = b;
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

		public void SetLowBatOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegLowBat"].Value;
					b = (byte)(b & 0xF7u);
					b |= (byte)(value ? 8u : 0u);
					registers["RegLowBat"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetLowBatTrim(LowBatTrimEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegLowBat"].Value;
					b = (byte)(b & 0xF8u);
					b |= (byte)value;
					registers["RegLowBat"].Value = b;
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

		public void SetLowPowerSelection(LowPowerSelection value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegSeqConfig1"].Value;
					b = (byte)(b & 0xFBu);
					b |= (byte)((byte)value << 2);
					registers["RegSeqConfig1"].Value = b;
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
					packet.Message = value;
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

		public void SetModulationShaping(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPaRamp"].Value;
					b = (byte)(b & 0x9Fu);
					b |= (byte)(value << 5);
					registers["RegPaRamp"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetModulationType(ModulationTypeEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegOpMode"].Value;
					b = (byte)(b & 0x9Fu);
					b |= (byte)((byte)value << 5);
					registers["RegOpMode"].Value = b;
					ReadRegister(registers["RegBitrateMsb"]);
					ReadRegister(registers["RegBitrateLsb"]);
					ReadRegister(registers["RegBitrateFrac"]);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetNodeAddress(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegNodeAdrs"].Value = value;
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

		public void SetOokAverageBias(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegOokAvg"].Value;
					b = (byte)(b & 0xF3u);
					b |= (byte)((byte)(value / 2m) << 2);
					registers["RegOokAvg"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetOokAverageThreshFilt(OokAverageThreshFiltEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegOokAvg"].Value;
					b = (byte)(b & 0xFCu);
					b |= (byte)((byte)value & 3u);
					registers["RegOokAvg"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetOokFixedThresh(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegOokFix"].Value = value;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetOokPeakRecoveryOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegOokAvg"].Value;
					b = (byte)(b & 0xEFu);
					b |= (byte)(value ? 16u : 0u);
					registers["RegOokAvg"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetOokPeakThreshDec(OokPeakThreshDecEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegOokAvg"].Value;
					b = (byte)(b & 0x1Fu);
					b |= (byte)(((byte)value & 7) << 5);
					registers["RegOokAvg"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetOokPeakThreshStep(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegOokPeak"].Value;
					b = (byte)(b & 0xF8u);
					b |= (byte)((byte)Array.IndexOf(OoPeakThreshStepTable, value) & 7u);
					registers["RegOokPeak"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetOokThreshType(OokThreshTypeEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegOokPeak"].Value;
					b = (byte)(b & 0xE7u);
					b |= (byte)(((byte)value & 3) << 3);
					registers["RegOokPeak"].Value = b;
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

		public void SetPacketFormat(PacketFormatEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPacketConfig1"].Value;
					b = (byte)(b & 0x7Fu);
					b |= (byte)((value == PacketFormatEnum.Variable) ? 128u : 0u);
					registers["RegPacketConfig1"].Value = b;
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
					packet.LogEnabled = value;
				}
			}
			catch (Exception ex)
			{
				packet.LogEnabled = false;
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

		public void SetPayloadLength(short value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegPacketConfig2"].Value = registers["RegPacketConfig2"].Value | (byte)((uint)(value >> 8) & 7u);
					registers["RegPayloadLength"].Value = (byte)value;
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

		public void SetPreambleDetectorOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPreambleDetect"].Value;
					b = (byte)(b & 0x7Fu);
					b |= (byte)(value ? 128u : 0u);
					registers["RegPreambleDetect"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPreambleDetectorSize(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPreambleDetect"].Value;
					b = (byte)(b & 0x9Fu);
					b |= (byte)(value - 1 << 5);
					registers["RegPreambleDetect"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPreambleDetectorTol(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegPreambleDetect"].Value;
					b = (byte)(b & 0xE0u);
					b |= (byte)(value & 0x1Fu);
					registers["RegPreambleDetect"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPreamblePolarity(PreamblePolarityEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegSyncConfig"].Value;
					b = (byte)(b & 0xDFu);
					b |= (byte)((byte)value << 5);
					registers["RegSyncConfig"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPreambleSize(int value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegPreambleMsb"].Value = (byte)(value >> 8);
					registers["RegPreambleLsb"].Value = (byte)value;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetRestartRxOnCollisionOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegRxConfig"].Value;
					b = (byte)(b & 0x7Fu);
					b |= (byte)(value ? 128u : 0u);
					registers["RegRxConfig"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetRestartRxWithoutPllLock()
		{
			try
			{
				lock (SyncThread)
				{
					byte data = 0;
					ReadRegister(registers["RegRxConfig"], ref data);
					WriteRegister(registers["RegRxConfig"], (byte)(data | 0x40u));
					restartRx = true;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetRestartRxWithPllLock()
		{
			try
			{
				lock (SyncThread)
				{
					byte data = 0;
					ReadRegister(registers["RegRxConfig"], ref data);
					WriteRegister(registers["RegRxConfig"], (byte)(data | 0x20u));
					restartRx = true;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetRssiCollisionThreshold(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegRssiCollision"].Value = (uint)value;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetRssiOffset(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					lock (SyncThread)
					{
						var b = (byte)registers["RegRssiConfig"].Value;
						b = (byte)(b & 7u);
						var b2 = (sbyte)value;
						if (b2 < 0)
						{
							b2 = (sbyte)(~b2 & 0x1F);
							b2++;
							b2 = (sbyte)(-b2);
						}
						b |= (byte)((b2 & 0x1F) << 3);
						registers["RegRssiConfig"].Value = b;
						ReadRegister(registers["RegRssiValue"]);
					}
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetRssiSmoothing(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					lock (SyncThread)
					{
						var b = (byte)registers["RegRssiConfig"].Value;
						b = (byte)(b & 0xF8u);
						b |= (byte)((uint)(int)(Math.Log((double)value, 2.0) - 1.0) & 7u);
						registers["RegRssiConfig"].Value = b;
					}
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetRssiThresh(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegRssiThresh"].Value = (uint)(-value * 2m);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetRxBw(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegRxBw"].Value;
					b = (byte)(b & 0xE0u);
					var exp = 0;
					var mant = 0;
					ComputeRxBwMantExp(frequencyXo, ModulationType, value, ref mant, ref exp);
					b = mant switch
					{
						16 => (byte)(b | (byte)((uint)exp & 7u)),
						20 => (byte)(b | (byte)(8u | ((uint)exp & 7u))),
						24 => (byte)(b | (byte)(0x10u | ((uint)exp & 7u))),
						_ => throw new Exception("Invalid RxBwMant parameter")
					};

					registers["RegRxBw"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetRxCalAutoOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegImageCal"].Value;
					b = (byte)(b & 0x7Fu);
					b |= (byte)(value ? 128u : 0u);
					registers["RegImageCal"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetRxTrigger(RxTriggerEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegRxConfig"].Value;
					b = (byte)(b & 0xF8u);
					switch (value)
					{
					case RxTriggerEnum.RX_TRIGER_001:
						b = (byte)(b | 1u);
						break;
					case RxTriggerEnum.RX_TRIGER_110:
						b = (byte)(b | 6u);
						break;
					case RxTriggerEnum.RX_TRIGER_111:
						b = (byte)(b | 7u);
						break;
					}
					registers["RegRxConfig"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetSequencerStart()
		{
			try
			{
				lock (SyncThread)
				{
					SequencerStarted = true;
					WriteRegister(registers["RegSeqConfig1"], (byte)((registers["RegSeqConfig1"].Value & 0x3Fu) | 0x80u));
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetSequencerStop()
		{
			try
			{
				lock (SyncThread)
				{
					SequencerStarted = false;
					WriteRegister(registers["RegSeqConfig1"], (byte)((registers["RegSeqConfig1"].Value & 0x3Fu) | 0x40u));
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetSyncOn(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegSyncConfig"].Value;
					b = (byte)(b & 0xEFu);
					b |= (byte)(value ? 16u : 0u);
					registers["RegSyncConfig"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetSyncSize(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegSyncConfig"].Value;
					b = (byte)(b & 0xF8u);
					b = ((Mode != OperatingModeEnum.Rx || !packet.IoHomeOn) ? ((byte)(b | (byte)((uint)(value - 1) & 7u))) : ((byte)(b | (byte)(value & 7u))));
					registers["RegSyncConfig"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetSyncValue(byte[] value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegSyncValue1"].Address;
					for (var i = 0; i < value.Length; i++)
					{
						registers[b + i].Value = value[i];
					}
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

		public void SetTempCalibrate(decimal tempRoomValue)
		{
			lock (SyncThread)
			{
				if (TempMonitorOff || Mode == 0 || Mode == OperatingModeEnum.Stdby) return;
				TempCalDone = false;
				TempValueRoom = tempRoomValue;
				byte data = 0;
				ReadRegister(registers["RegTemp"], ref data);
				TempValueCal = (byte)(data & 0x7Fu);
				if ((data & 0x80) == 128)
				{
					TempValueCal *= -1m;
				}
				ReadRegister(registers["RegTemp"]);
				ReadRegister(registers["RegFormerTemp"]);
				TempCalDone = true;
			}
		}

		public void SetTempMonitorOff(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegImageCal"].Value;
					b = (byte)(b & 0xFEu);
					b = (byte)(b | ((!value) ? 1u : 0u));
					registers["RegImageCal"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetTempThreshold(TempThresholdEnum value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegImageCal"].Value;
					b = (byte)(b & 0xF9u);
					b |= (byte)((byte)value << 1);
					registers["RegImageCal"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetTimeoutPreamble(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegRxTimeout2"].Value = (uint)Math.Round(value / 1000m / (16m * Tbit), MidpointRounding.AwayFromZero);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetTimeoutRssi(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegRxTimeout1"].Value = (uint)Math.Round(value / 1000m / (16m * Tbit), MidpointRounding.AwayFromZero);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetTimeoutSyncWord(decimal value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegRxTimeout3"].Value = (uint)Math.Round(value / 1000m / (16m * Tbit), MidpointRounding.AwayFromZero);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetTimer1Coef(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegTimer1Coef"].Value = value;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetTimer1Resolution(TimerResolution value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegTimerResol"].Value;
					b = (byte)(b & 0xF3u);
					b |= (byte)((byte)value << 2);
					registers["RegTimerResol"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetTimer2Coef(byte value)
		{
			try
			{
				lock (SyncThread)
				{
					registers["RegTimer2Coef"].Value = value;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetTimer2Resolution(TimerResolution value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegTimerResol"].Value;
					b = (byte)(b & 0xFCu);
					b |= (byte)value;
					registers["RegTimerResol"].Value = b;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetTxStartCondition(bool value)
		{
			try
			{
				lock (SyncThread)
				{
					var b = (byte)registers["RegFifoThresh"].Value;
					b = (byte)(b & 0x7Fu);
					b |= (byte)(value ? 128u : 0u);
					registers["RegFifoThresh"].Value = b;
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
				const byte command = 0x3;
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
				const byte command = 0x2;
				var hidCommandsStatus = HidCommandsStatus.SX_ERROR;
				var num = ulong.MaxValue;
				var inData = new byte[25];
				var array = new byte[2];
				var outData = array;
				try
				{
					UsbDevice.TxRxCommand(command, outData, ref inData);
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
					Console.WriteLine("{0} ms: {1} with status {2}", num, Enum.GetName(typeof(HidCommands), (HidCommands)command), Enum.GetName(typeof(HidCommandsStatus), hidCommandsStatus));
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
					if (hidCommandsStatus == HidCommandsStatus.SX_OK)
					{
						if (32 != inData[9])
						{
							data = null;
							return false;
						}
						Array.Copy(inData, 10, data, i, 32);
					}
				}
				return true;
			}
		}

		public bool SKSetId(byte id)
		{
			lock (SyncThread)
			{
				const byte command = 0x4;
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
				const byte command = 0x5;
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
                const byte command = 0x71;
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

		public void WriteRegisters()
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

		private void BitrateFdevCheck(decimal bitRate, decimal fdevN)
		{
			var num = 250000m;
			var num2 = 1200m;
			if (bitRateFdevCheckDisbale)
			{
				return;
			}
			if (modulationType == ModulationTypeEnum.OOK)
			{
				num = 32768m;
			}
			if (bitRate < num2 || bitRate > num)
			{
				OnBitrateLimitStatusChanged(LimitCheckStatusEnum.OUT_OF_RANGE, "The bitrate is out of range.\nThe valid range is [" + num2 + ", " + num + "]");
			}
			else
			{
				OnBitrateLimitStatusChanged(LimitCheckStatusEnum.OK, "");
			}
			if (modulationType != ModulationTypeEnum.OOK)
			{
				if (fdevN < 600m || fdev > 200000m)
				{
					OnFdevLimitStatusChanged(LimitCheckStatusEnum.OUT_OF_RANGE, "The frequency deviation is out of range.\nThe valid range is [" + 600 + ", " + 200000 + "]");
				}
				else if (fdevN + bitRate / 2m > 250000m)
				{
					OnFdevLimitStatusChanged(LimitCheckStatusEnum.ERROR, "The single sided band width has been exceeded.\n Fdev + ( Bitrate / 2 ) > " + 250000 + " Hz");
				}
				else
				{
					var num3 = 2.0m * fdevN / bitRate;
					if (num3 is >= 0.4969m and <= 10.0m)
					{
						OnFdevLimitStatusChanged(LimitCheckStatusEnum.OK, "");
					}
					else
					{
						OnFdevLimitStatusChanged(LimitCheckStatusEnum.ERROR, "The modulation index is out of range.\nThe valid range is [0.5, 10]");
					}
				}
			}
			else
			{
				OnFdevLimitStatusChanged(LimitCheckStatusEnum.OK, "");
			}
		}

		private decimal ComputeRxBwMax()
		{
			if (ModulationType == ModulationTypeEnum.FSK)
			{
				return FrequencyXo / (16m * (decimal)Math.Pow(2.0, 2.0));
			}
			return FrequencyXo / (16m * (decimal)Math.Pow(2.0, 3.0));
		}

		private decimal ComputeRxBwMin()
		{
			if (ModulationType == ModulationTypeEnum.FSK)
			{
				return FrequencyXo / (24m * (decimal)Math.Pow(2.0, 9.0));
			}
			return FrequencyXo / (24m * (decimal)Math.Pow(2.0, 10.0));
		}

		private void Device_Dio0Changed(object sender, IoChangedEventArgs e)
		{
			lock (SyncThread)
			{
				if (!IsPacketHandlerStarted || (!e.Sate && !firstTransmit)) return;
				firstTransmit = false;
				switch (Mode)
				{
					case OperatingModeEnum.Tx:
						OnPacketHandlerTransmitted();
						PacketHandlerTransmit();
						break;
					case OperatingModeEnum.Rx:
						PacketHandlerReceive();
						break;
				}
			}
		}

		private void Device_Dio1Changed(object sender, IoChangedEventArgs e)
        {
            throw new NotSupportedException();
        }

        private void Device_Dio2Changed(object sender, IoChangedEventArgs e)
        {
            throw new NotSupportedException();
        }

        private void Device_Dio3Changed(object sender, IoChangedEventArgs e)
        {
            throw new NotSupportedException();
        }

        private void Device_Dio4Changed(object sender, IoChangedEventArgs e)
        {
            throw new NotSupportedException();
        }

        private void Device_Dio5Changed(object sender, IoChangedEventArgs e)
        {
            throw new NotSupportedException();
        }

		private void Device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
			case "Version":
				PopulateRegisters();
				ReadRegisters();
				break;
			case "RxReady":
				_ = RxReady;
				break;
			}
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

		private void OnBitrateLimitStatusChanged(LimitCheckStatusEnum status, string message)
		{
            BitrateLimitStatusChanged?.Invoke(this, new LimitCheckStatusEventArg(status, message));
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

		private void OnFdevLimitStatusChanged(LimitCheckStatusEnum status, string message)
		{
            FdevLimitStatusChanged?.Invoke(this, new LimitCheckStatusEventArg(status, message));
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
            PacketHandlerReceived?.Invoke(this, new PacketStatusEventArg(packetNumber, maxPacketNumber));
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
            PacketHandlerTransmitted?.Invoke(this, new PacketStatusEventArg(packetNumber, maxPacketNumber));
        }

		private void OnPropertyChanged(string propName)
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

		private void OnSyncValueLimitChanged(LimitCheckStatusEnum status, string message)
		{
            SyncValueLimitChanged?.Invoke(this, new LimitCheckStatusEventArg(status, message));
        }

		private void packet_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(e.PropertyName);
		}

		private void PacketHandlerReceive()
		{
			lock (SyncThread)
			{
				try
				{
					SetModeLeds(OperatingModeEnum.Rx);
					byte data = 0;
					ReadRegister(registers["RegRssiValue"], ref data);
					packet.Rssi = -(decimal)data / 2.0m;
					frameReceived = ReceiveRfData(out var buffer);
					switch (packet.PacketFormat)
					{
						case PacketFormatEnum.Fixed when packet.AddressFiltering != 0:
							packet.NodeAddressRx = buffer[0];
							Array.Copy(buffer, 1, buffer, 0, buffer.Length - 1);
							Array.Resize(ref buffer, packet.PayloadLength - 1);
							break;
						case PacketFormatEnum.Fixed:
							Array.Resize(ref buffer, packet.PayloadLength);
							break;
						case PacketFormatEnum.Variable:
						{
							int num = buffer[0];
							Array.Copy(buffer, 1, buffer, 0, buffer.Length - 1);
							Array.Resize(ref buffer, num);
							if (packet.AddressFiltering != 0)
							{
								num--;
								packet.NodeAddressRx = buffer[0];
								Array.Copy(buffer, 1, buffer, 0, buffer.Length - 1);
								Array.Resize(ref buffer, num);
							}

							break;
						}
					}
					packet.Message = buffer;
					packetNumber++;
					OnPacketHandlerReceived();
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
					SetDataMode(DataModeEnum.Packet);
					if (Mode == OperatingModeEnum.Tx)
					{
						if (packet.MessageLength == 0)
						{
							MessageBox.Show("Message must be at least one byte long", "SX1276SKA-PacketHandler", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							throw new Exception("Message must be at least one byte long");
						}
					}
					else
					{
						_ = Mode;
						_ = 5;
					}
					frameTransmitted = false;
					frameReceived = false;
					if (Mode == OperatingModeEnum.Tx)
					{
						SetOperatingMode(OperatingModeEnum.Tx, isQuiet: true);
						firstTransmit = true;
					}
					else
					{
						SetOperatingMode(OperatingModeEnum.Rx, isQuiet: true);
						OnPacketHandlerReceived();
					}
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
					frameTransmitted = TransmitRfData(packet.ToArray());
					packetNumber++;
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
				byte data = 0;
				if (!Read(0x42, ref data))
				{
					throw new Exception("Unable to read register RegVersion");
				}
				if (!Read(0x42, ref data))
				{
					throw new Exception("Unable to read register RegVersion");
				}
				Version = new Version((data & 0xF0) >> 4, data & 0xF);
			}
			registers = [];
			byte b = 0;
			registers.Add(new Register("RegFifo", b++, 0u, readOnly: true, visible: true));
			registers.Add(new Register("RegOpMode", b++, 9u, readOnly: false, visible: true));
			registers.Add(new Register("RegBitrateMsb", b++, 26u, readOnly: false, visible: true));
			registers.Add(new Register("RegBitrateLsb", b++, 11u, readOnly: false, visible: true));
			registers.Add(new Register("RegFdevMsb", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegFdevLsb", b++, 82u, readOnly: false, visible: true));
			registers.Add(new Register("RegFrfMsb", b++, 228u, readOnly: false, visible: true));
			registers.Add(new Register("RegFrfMid", b++, 192u, readOnly: false, visible: true));
			registers.Add(new Register("RegFrfLsb", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegPaConfig", b++, 15u, readOnly: false, visible: true));
			registers.Add(new Register("RegPaRamp", b++, 25u, readOnly: false, visible: true));
			registers.Add(new Register("RegOcp", b++, 43u, readOnly: false, visible: true));
			registers.Add(new Register("RegLna", b++, 32u, readOnly: false, visible: true));
			registers.Add(new Register("RegRxConfig", b++, 8u, readOnly: false, visible: true));
			registers.Add(new Register("RegRssiConfig", b++, 2u, readOnly: false, visible: true));
			registers.Add(new Register("RegRssiCollision", b++, 10u, readOnly: false, visible: true));
			registers.Add(new Register("RegRssiThresh", b++, 255u, readOnly: false, visible: true));
			registers.Add(new Register("RegRssiValue", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegRxBw", b++, 21u, readOnly: false, visible: true));
			registers.Add(new Register("RegAfcBw", b++, 11u, readOnly: false, visible: true));
			registers.Add(new Register("RegOokPeak", b++, 40u, readOnly: false, visible: true));
			registers.Add(new Register("RegOokFix", b++, 12u, readOnly: false, visible: true));
			registers.Add(new Register("RegOokAvg", b++, 18u, readOnly: false, visible: true));
			registers.Add(new Register("RegRes17", b++, 71u, readOnly: false, visible: true));
			registers.Add(new Register("RegRes18", b++, 50u, readOnly: false, visible: true));
			registers.Add(new Register("RegRes19", b++, 62u, readOnly: false, visible: true));
			registers.Add(new Register("RegAfcFei", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegAfcMsb", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegAfcLsb", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegFeiMsb", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegFeiLsb", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegPreambleDetect", b++, 0x40u, readOnly: false, visible: true));
			registers.Add(new Register("RegRxTimeout1", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegRxTimeout2", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegRxTimeout3", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegRxDelay", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegOsc", b++, 5u, readOnly: false, visible: true));
			registers.Add(new Register("RegPreambleMsb", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegPreambleLsb", b++, 3u, readOnly: false, visible: true));
			registers.Add(new Register("RegSyncConfig", b++, 147u, readOnly: false, visible: true));
			registers.Add(new Register("RegSyncValue1", b++, 85u, readOnly: false, visible: true));
			registers.Add(new Register("RegSyncValue2", b++, 85u, readOnly: false, visible: true));
			registers.Add(new Register("RegSyncValue3", b++, 85u, readOnly: false, visible: true));
			registers.Add(new Register("RegSyncValue4", b++, 85u, readOnly: false, visible: true));
			registers.Add(new Register("RegSyncValue5", b++, 85u, readOnly: false, visible: true));
			registers.Add(new Register("RegSyncValue6", b++, 85u, readOnly: false, visible: true));
			registers.Add(new Register("RegSyncValue7", b++, 85u, readOnly: false, visible: true));
			registers.Add(new Register("RegSyncValue8", b++, 85u, readOnly: false, visible: true));
			registers.Add(new Register("RegPacketConfig1", b++, 144u, readOnly: false, visible: true));
			registers.Add(new Register("RegPacketConfig2", b++, 64u, readOnly: false, visible: true));
			registers.Add(new Register("RegPayloadLength", b++, 64u, readOnly: false, visible: true));
			registers.Add(new Register("RegNodeAdrs", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegBroadcastAdrs", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegFifoThresh", b++, 15u, readOnly: false, visible: true));
			registers.Add(new Register("RegSeqConfig1", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegSeqConfig2", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegTimerResol", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegTimer1Coef", b++, 245u, readOnly: false, visible: true));
			registers.Add(new Register("RegTimer2Coef", b++, 32u, readOnly: false, visible: true));
			registers.Add(new Register("RegImageCal", b++, 130u, readOnly: false, visible: true));
			registers.Add(new Register("RegTemp", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegLowBat", b++, 2u, readOnly: false, visible: true));
			registers.Add(new Register("RegIrqFlags1", b++, 128u, readOnly: false, visible: true));
			registers.Add(new Register("RegIrqFlags2", b++, 64u, readOnly: false, visible: true));
			registers.Add(new Register("RegDioMapping1", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegDioMapping2", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegVersion", b++, 17u, readOnly: false, visible: true));
			for (var i = 67; i < 68; i++)
			{
				registers.Add(new Register("RegTest" + b.ToString("X02"), b++, 0u, readOnly: false, visible: true));
			}
			registers.Add(new Register("RegPllHop", b++, 45u, readOnly: false, visible: true));
			for (var j = 69; j < 75; j++)
			{
				registers.Add(new Register("RegTest" + b.ToString("X02"), b++, 0u, readOnly: false, visible: true));
			}
			registers.Add(new Register("RegTcxo", b++, 9u, readOnly: false, visible: true));
			registers.Add(new Register("RegTest4C", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegPaDac", b++, 132u, readOnly: false, visible: true));
			for (var k = 78; k < 91; k++)
			{
				registers.Add(new Register("RegTest" + b.ToString("X02"), b++, 0u, readOnly: false, visible: true));
			}
			registers.Add(new Register("RegFormerTemp", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegTest5C", b++, 0u, readOnly: false, visible: true));
			registers.Add(new Register("RegBitrateFrac", b++, 0u, readOnly: false, visible: true));
			for (var l = 94; l < 97; l++)
			{
				registers.Add(new Register("RegTest" + b.ToString("X02"), b++, 0u, readOnly: false, visible: true));
			}
			registers.Add(new Register("RegAgcRef", b++, 25u, readOnly: false, visible: true));
			registers.Add(new Register("RegAgcThresh1", b++, 12u, readOnly: false, visible: true));
			registers.Add(new Register("RegAgcThresh2", b++, 75u, readOnly: false, visible: true));
			registers.Add(new Register("RegAgcThresh3", b++, 204u, readOnly: false, visible: true));
			for (var m = 101; m < 112; m++)
			{
				registers.Add(new Register("RegTest" + b.ToString("X02"), b++, 0u, readOnly: false, visible: true));
			}
			registers.Add(new Register("RegPll", b++, 208u, readOnly: false, visible: true));
			for (var n = 113; n < 128; n++)
			{
				registers.Add(new Register("RegTest" + b.ToString("X02"), b++, 0u, readOnly: false, visible: true));
			}
			foreach (var register in registers)
			{
				register.PropertyChanged += Registers_PropertyChanged;
			}
			Packet = new Packet();
		}

		private void PreambleCheck()
        {
            throw new NotSupportedException();
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

		private void ReadIrqFlags()
		{
			ReadRegister(registers["RegIrqFlags1"]);
			ReadRegister(registers["RegIrqFlags2"]);
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

		private bool ReceiveRfData(out byte[] buffer)
		{
			lock (SyncThread)
			{
				// var flag = false;
				SetOperatingMode(OperatingModeEnum.Stdby, isQuiet: true);
				buffer = FifoData;
				var flag = ReadFifo(ref buffer);
				SetOperatingMode(OperatingModeEnum.Rx, isQuiet: true);
				return flag;
			}
		}

		private void Registers_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
				if (((Register)sender).Name == "RegOpMode")
				{
					if (Mode == OperatingModeEnum.Rx)
					{
						ReadRegister(registers["RegLna"]);
						ReadRegister(registers["RegFeiMsb"]);
						ReadRegister(registers["RegFeiLsb"]);
						ReadRegister(registers["RegAfcMsb"]);
						ReadRegister(registers["RegAfcLsb"]);
						ReadRegister(registers["RegRssiValue"]);
					}
					ReadIrqFlags();
				}
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
							ReadIrqFlags();
							if (SequencerStarted)
							{
								ReadRegister(registers["RegOpMode"]);
							}
							switch (Mode)
							{
								case OperatingModeEnum.Rx when !SpectrumOn:
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

									break;
								}
								case OperatingModeEnum.Rx:
									SpectrumProcess();
									break;
								case OperatingModeEnum.Tx when PngEnabled && !FifoFull:
									WriteFifo([Png.NextByte()]);
									break;
							}
							if (!TempMonitorOff && TempCalDone && Mode != 0 && Mode != OperatingModeEnum.Stdby)
							{
								TempMeasRunning = false;
								OnPropertyChanged("TempMeasRunning");
							}
						}
						if (num >= 200)
						{
							if (restartRx)
							{
								restartRx = false;
								ReadRegister(registers["RegLna"]);
								ReadRegister(registers["RegFeiMsb"]);
								ReadRegister(registers["RegFeiLsb"]);
								ReadRegister(registers["RegAfcMsb"]);
								ReadRegister(registers["RegAfcLsb"]);
							}
							if (!TempMonitorOff && TempCalDone && Mode != 0 && Mode != OperatingModeEnum.Stdby)
							{
								TempMeasRunning = true;
								OnPropertyChanged("TempMeasRunning");
								ReadRegister(registers["RegImageCal"]);
								ReadRegister(registers["RegTemp"]);
								ReadRegister(registers["RegFormerTemp"]);
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
			TempCalDone = false;
			if (!IsOpen)
			{
				ReadRegisters();
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
					SKSetPinState(7, 1);
					SKSetPinState(8, 0);
					break;
				case OperatingModeEnum.Rx:
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
				if (value == OperatingModeEnum.Tx)
				{
					SKSetPinState(11, 0);
					SKSetPinState(12, 1);
				}
				else
				{
					SKSetPinState(11, 1);
					SKSetPinState(12, 0);
				}
				var b = (byte)registers["RegOpMode"].Value;
				b = (byte)(b & 0xF8u);
				b |= (byte)value;
				if (!isQuiet)
				{
					registers["RegOpMode"].Value = b;
					return;
				}
				lock (SyncThread)
				{
					if (!Write((byte)registers["RegOpMode"].Address, b))
					{
						throw new Exception("Unable to write register " + registers["RegOpMode"].Name);
					}
					if (packet.IoHomeOn)
					{
						SetSyncSize(packet.SyncSize);
					}

					if (Mode != OperatingModeEnum.Rx) return;
					ReadRegister(registers["RegLna"]);
					ReadRegister(registers["RegFeiMsb"]);
					ReadRegister(registers["RegFeiLsb"]);
					ReadRegister(registers["RegAfcMsb"]);
					ReadRegister(registers["RegAfcLsb"]);
				}
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
                const byte command = 0x80;
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
				const byte command = 0x14;
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
				byte b = 1;
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
				byte b = 0;
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
				const byte command = 0x11;
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
			SetRestartRxWithPllLock();
			ReadRegister(registers["RegRssiValue"]);
			SpectrumFrequencyId++;
			if (SpectrumFrequencyId >= SpectrumNbFrequenciesMax)
			{
				SpectrumFrequencyId = 0;
			}
		}

		private void SyncValueCheck(byte[] value)
		{
			var num = 0;
			if (value == null)
			{
				num++;
			}
			else if (value[0] == 0)
			{
				num++;
			}
			if (num != 0)
			{
				OnSyncValueLimitChanged(LimitCheckStatusEnum.ERROR, "First sync word byte must be different of 0!");
			}
			else
			{
				OnSyncValueLimitChanged(LimitCheckStatusEnum.OK, "");
			}
		}

		private bool TransmitRfData(byte[] buffer)
		{
			lock (SyncThread)
			{
				// var flag = false;
				SetOperatingMode(OperatingModeEnum.Stdby, isQuiet: true);
				Thread.Sleep(100);
				var flag = WriteFifo(buffer);
				SetOperatingMode(OperatingModeEnum.Tx, isQuiet: true);
				return flag;
			}
		}

		private void UpdateReceiverData()
		{
			OnPropertyChanged("RxBwMin");
			OnPropertyChanged("RxBwMax");
            rxBw = ((registers["RegRxBw"].Value & 0x18) >> 3) switch
            {
                0u => ComputeRxBw(frequencyXo, modulationType, 16, (int)(registers["RegRxBw"].Value & 7)),
                1u => ComputeRxBw(frequencyXo, modulationType, 20, (int)(registers["RegRxBw"].Value & 7)),
                2u => ComputeRxBw(frequencyXo, modulationType, 24, (int)(registers["RegRxBw"].Value & 7)),
                _ => throw new Exception("Invalid RxBwMant parameter"),
            };
            OnPropertyChanged("RxBw");
			OnPropertyChanged("AfcRxBwMin");
			OnPropertyChanged("AfcRxBwMax");
            afcRxBw = ((registers["RegAfcBw"].Value & 0x18) >> 3) switch
            {
                0u => ComputeRxBw(frequencyXo, modulationType, 16, (int)(registers["RegAfcBw"].Value & 7)),
                1u => ComputeRxBw(frequencyXo, modulationType, 20, (int)(registers["RegAfcBw"].Value & 7)),
                2u => ComputeRxBw(frequencyXo, modulationType, 24, (int)(registers["RegAfcBw"].Value & 7)),
                _ => throw new Exception("Invalid RxBwMant parameter"),
            };
            OnPropertyChanged("AfcRxBw");
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
			{
				if ((r.Value & 0x60) == 64 || (r.Value & 0x60) == 96)
				{
					r.Value &= 159u;
				}
				ModulationType = (ModulationTypeEnum)((int)(r.Value >> 5) & 3);
				UpdateReceiverData();
				BitrateFdevCheck(bitrate, fdev);
				LowFrequencyModeOn = ((r.Value >> 3) & 1) == 1;
				var b2 = (byte)(r.Value & 7u);
				if (b2 > 5)
				{
					b2 = 0;
				}
				Mode = (OperatingModeEnum)b2;
				if (packet.Mode != Mode)
				{
					packet.Mode = Mode;
				}
				if ((((registers["RegPacketConfig2"].Value & 7) << 8) | registers["RegPayloadLength"].Value) != packet.PayloadLength)
				{
					registers["RegPacketConfig2"].Value = registers["RegPacketConfig2"].Value | (byte)((uint)(packet.PayloadLength >> 8) & 7u);
					registers["RegPayloadLength"].Value = (byte)packet.PayloadLength;
				}
				lock (SyncThread)
				{
					SetModeLeds(Mode);
					break;
				}
			}
			case "RegBitrateMsb":
			case "RegBitrateLsb":
				if (((registers["RegBitrateMsb"].Value << 8) | registers["RegBitrateLsb"].Value) == 0)
				{
					registers["RegBitrateLsb"].Value = 1u;
				}
				if (ModulationType == ModulationTypeEnum.FSK)
				{
					Bitrate = frequencyXo / (((registers["RegBitrateMsb"].Value << 8) | registers["RegBitrateLsb"].Value) + registers["RegBitrateFrac"].Value / 16.0m);
				}
				else
				{
					Bitrate = frequencyXo / ((registers["RegBitrateMsb"].Value << 8) | registers["RegBitrateLsb"].Value);
				}
				break;
			case "RegFdevMsb":
			case "RegFdevLsb":
				Band = (BandEnum)((int)(registers["RegFdevMsb"].Value >> 6) & 3);
				Fdev = (((registers["RegFdevMsb"].Value & 0x3F) << 8) | registers["RegFdevLsb"].Value) * FrequencyStep;
				break;
			case "RegFrfMsb":
			case "RegFrfMid":
			case "RegFrfLsb":
				FrequencyRf = ((registers["RegFrfMsb"].Value << 16) | (registers["RegFrfMid"].Value << 8) | registers["RegFrfLsb"].Value) * FrequencyStep;
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
				OnPropertyChanged("MaxOutputPower");
				OnPropertyChanged("OutputPower");
				break;
			case "RegPaRamp":
				ModulationShaping = (byte)((r.Value >> 5) & 3u);
				ForceTxBandLowFrequencyOn = ((r.Value >> 4) & 1) == 1;
				PaRamp = (PaRampEnum)((int)r.Value & 0xF);
				break;
			case "RegOcp":
				OcpOn = ((r.Value >> 5) & 1) == 1;
				OcpTrim = (r.Value & 0x1F) switch
				{
					<= 15 => 45 + 5 * (r.Value & 0xF),
					> 15 and <= 27 => -30 + 10 * (r.Value & 0x1F),
					_ => 240.0m
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
				break;
			case "RegRxConfig":
				RestartRxOnCollision = ((r.Value >> 7) & 1) == 1;
				AfcAutoOn = ((r.Value >> 4) & 1) == 1;
				AgcAutoOn = ((r.Value >> 3) & 1) == 1;
                    RxTrigger = (r.Value & 7) switch
                    {
                        1u => RxTriggerEnum.RX_TRIGER_001,
                        6u => RxTriggerEnum.RX_TRIGER_110,
                        7u => RxTriggerEnum.RX_TRIGER_111,
                        _ => RxTriggerEnum.RX_TRIGER_000,
                    };
                    break;
			case "RegRssiConfig":
			{
				var b = (sbyte)(r.Value >> 3);
				if ((b & 0x10) == 16)
				{
					b = (sbyte)(~b & 0x1F);
					b++;
					b = (sbyte)(-b);
				}
				RssiOffset = b;
				RssiSmoothing = (decimal)Math.Pow(2.0, (r.Value & 7) + 1);
				break;
			}
			case "RegRssiCollision":
				RssiCollisionThreshold = r.Value;
				break;
			case "RegRssiThresh":
				RssiThreshold = -(decimal)r.Value / 2.0m;
				break;
			case "RegRssiValue":
				prevRssiValue = RssiValue;
				RssiValue = -(decimal)r.Value / 2.0m;
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
							OnPropertyChanged("RfIoRssiValue");
							break;
						}
						case RfPaSwitchSelEnum.RF_IO_PA_BOOST:
						{
							if (RfPaSwitchEnabled == 1)
							{
								RfIoRssiValue = -127.7m;
							}
							RfPaRssiValue = RssiValue;
							OnPropertyChanged("RfPaRssiValue");
							break;
						}
					}
				}
				SpectrumRssiValue = RssiValue;
				OnPropertyChanged("RssiValue");
				OnPropertyChanged("SpectrumData");
				break;
			case "RegRxBw":
			{
				// var num2 = 0;
				rxBw = ComputeRxBw(mant: ((r.Value & 0x18) >> 3) switch
				{
					0u => 16, 
					1u => 20, 
					2u => 24, 
					_ => throw new Exception("Invalid RxBwMant parameter"), 
				}, frequencyXo: frequencyXo, mod: modulationType, exp: (int)(r.Value & 7));
				OnPropertyChanged("RxBwMin");
				OnPropertyChanged("RxBwMax");
				OnPropertyChanged("RxBw");
				OnPropertyChanged("AgcReference");
				OnPropertyChanged("AgcThresh1");
				OnPropertyChanged("AgcThresh2");
				OnPropertyChanged("AgcThresh3");
				OnPropertyChanged("AgcThresh4");
				OnPropertyChanged("AgcThresh5");
				break;
			}
			case "RegAfcBw":
			{
				// var num2 = 0;
				afcRxBw = ComputeRxBw(mant: ((r.Value & 0x18) >> 3) switch
				{
					0u => 16, 
					1u => 20, 
					2u => 24, 
					_ => throw new Exception("Invalid RxBwMant parameter"), 
				}, frequencyXo: frequencyXo, mod: modulationType, exp: (int)(r.Value & 7));
				OnPropertyChanged("AfcRxBwMin");
				OnPropertyChanged("AfcRxBwMax");
				OnPropertyChanged("AfcRxBw");
				break;
			}
			case "RegOokPeak":
				BitSyncOn = ((r.Value >> 5) & 1) == 1;
				OokThreshType = (OokThreshTypeEnum)((int)(r.Value >> 3) & 3);
				OokPeakThreshStep = OoPeakThreshStepTable[r.Value & 7];
				break;
			case "RegOokFix":
				OokFixedThreshold = (byte)r.Value;
				break;
			case "RegOokAvg":
				OokPeakThreshDec = (OokPeakThreshDecEnum)((int)(r.Value >> 5) & 7);
				OokAverageOffset = ((r.Value >> 2) & 3) * 2;
				OokAverageThreshFilt = (OokAverageThreshFiltEnum)((int)r.Value & 3);
				break;
			case "RegAfcFei":
				AfcAutoClearOn = (r.Value & 1) == 1;
				break;
			case "RegAfcMsb":
			case "RegAfcLsb":
				AfcValue = (short)((registers["RegAfcMsb"].Value << 8) | registers["RegAfcLsb"].Value) * FrequencyStep;
				break;
			case "RegFeiMsb":
			case "RegFeiLsb":
				FeiValue = (short)((registers["RegFeiMsb"].Value << 8) | registers["RegFeiLsb"].Value) * FrequencyStep;
				break;
			case "RegPreambleDetect":
				PreambleDetectorOn = ((r.Value >> 7) & 1) == 1;
				PreambleDetectorSize = (byte)(((r.Value >> 5) & 3) + 1);
				PreambleDetectorTol = (byte)(r.Value & 0x1Fu);
				break;
			case "RegRxTimeout1":
				TimeoutRxRssi = r.Value * (16m * Tbit) * 1000m;
				break;
			case "RegRxTimeout2":
				TimeoutRxPreamble = r.Value * (16m * Tbit) * 1000m;
				break;
			case "RegRxTimeout3":
				TimeoutSignalSync = r.Value * (16m * Tbit) * 1000m;
				break;
			case "RegRxDelay":
				InterPacketRxDelay = r.Value * (4m * Tbit) * 1000m;
				break;
			case "RegOsc":
				ClockOut = (ClockOutEnum)((int)r.Value & 7);
				break;
			case "RegPreambleMsb":
			case "RegPreambleLsb":
				packet.PreambleSize = (int)((registers["RegPreambleMsb"].Value << 8) | registers["RegPreambleLsb"].Value);
				break;
			case "RegSyncConfig":
				packet.AutoRestartRxOn = (AutoRestartRxEnum)((int)(r.Value >> 6) & 3);
				packet.PreamblePolarity = (PreamblePolarityEnum)((int)(r.Value >> 5) & 1);
				packet.SyncOn = ((r.Value >> 4) & 1) == 1;
				packet.FifoFillCondition = (FifoFillConditionEnum)((int)(r.Value >> 3) & 1);
				if (Mode == OperatingModeEnum.Rx && packet.IoHomeOn)
				{
					packet.SyncSize = (byte)(r.Value & 7u);
				}
				else
				{
					packet.SyncSize = (byte)((r.Value & 7) + 1);
				}
				UpdateSyncValue();
				break;
			case "RegSyncValue1":
			case "RegSyncValue2":
			case "RegSyncValue3":
			case "RegSyncValue4":
			case "RegSyncValue5":
			case "RegSyncValue6":
			case "RegSyncValue7":
			case "RegSyncValue8":
				UpdateSyncValue();
				break;
			case "RegPacketConfig1":
				packet.PacketFormat = ((((r.Value >> 7) & 1) == 1) ? PacketFormatEnum.Variable : PacketFormatEnum.Fixed);
				packet.DcFree = (DcFreeEnum)((int)(r.Value >> 5) & 3);
				packet.CrcOn = ((r.Value >> 4) & 1) == 1;
				packet.CrcAutoClearOff = ((r.Value >> 3) & 1) == 1;
				packet.AddressFiltering = (AddressFilteringEnum)((int)(r.Value >> 1) & 3);
				packet.CrcIbmOn = (r.Value & 1) == 1;
				break;
			case "RegPacketConfig2":
				packet.DataMode = (DataModeEnum)((int)(r.Value >> 6) & 1);
				packet.IoHomeOn = ((r.Value >> 5) & 1) == 1;
				if (Mode == OperatingModeEnum.Rx && packet.IoHomeOn)
				{
					packet.SyncSize = (byte)(Registers["RegSyncConfig"].Value & 7u);
				}
				else
				{
					packet.SyncSize = (byte)((Registers["RegSyncConfig"].Value & 7) + 1);
				}
				UpdateSyncValue();
				packet.IoHomePwrFrameOn = ((r.Value >> 4) & 1) == 1;
				packet.BeaconOn = ((r.Value >> 3) & 1) == 1;
				packet.PayloadLength = (short)((packet.PayloadLength & 0xFF) | (int)((r.Value & 7) << 8));
				OnPropertyChanged("Crc");
				break;
			case "RegPayloadLength":
				packet.PayloadLength = (short)((packet.PayloadLength & 0x700) | (int)r.Value);
				break;
			case "RegNodeAdrs":
				packet.NodeAddress = (byte)r.Value;
				break;
			case "RegBroadcastAdrs":
				packet.BroadcastAddress = (byte)r.Value;
				break;
			case "RegFifoThresh":
				packet.TxStartCondition = ((r.Value >> 7) & 1) == 1;
				packet.FifoThreshold = (byte)(r.Value & 0x7Fu);
				break;
			case "RegSeqConfig1":
				IdleMode = (IdleMode)((int)(r.Value >> 5) & 1);
				FromStart = (FromStart)((int)(r.Value >> 3) & 3);
				LowPowerSelection = (LowPowerSelection)((int)(r.Value >> 2) & 1);
				FromIdle = (FromIdle)((int)(r.Value >> 1) & 1);
				FromTransmit = (FromTransmit)((int)r.Value & 1);
				break;
			case "RegSeqConfig2":
				FromReceive = (FromReceive)((int)(r.Value >> 5) & 7);
				FromRxTimeout = (FromRxTimeout)((int)(r.Value >> 3) & 3);
				FromPacketReceived = (FromPacketReceived)((int)r.Value & 7);
				break;
			case "RegTimerResol":
				Timer1Resolution = (TimerResolution)((int)(r.Value >> 2) & 3);
				Timer2Resolution = (TimerResolution)((int)r.Value & 3);
				break;
			case "RegTimer1Coef":
				Timer1Coef = (byte)r.Value;
				break;
			case "RegTimer2Coef":
				Timer2Coef = (byte)r.Value;
				break;
			case "RegImageCal":
				AutoImageCalOn = ((r.Value >> 7) & 1) == 1;
				ImageCalRunning = ((r.Value >> 5) & 1) == 0;
				OnPropertyChanged("ImageCalRunning");
				TempChange = ((r.Value >> 3) & 1) == 1;
				OnPropertyChanged("TempChange");
				TempThreshold = (TempThresholdEnum)((r.Value & 6) >> 1);
				TempMonitorOff = (r.Value & 1) == 1;
				break;
			case "RegTemp":
				TempValue = (byte)(r.Value & 0x7Fu);
				if ((r.Value & 0x80) == 128)
				{
					TempValue *= -1m;
				}
				tempDelta = TempValue - formerTemp;
				OnPropertyChanged("TempDelta");
				TempValue += tempValueRoom - TempValueCal;
				OnPropertyChanged("TempValue");
				break;
			case "RegLowBat":
				LowBatOn = ((r.Value >> 3) & 1) == 1;
				LowBatTrim = (LowBatTrimEnum)((int)r.Value & 7);
				break;
			case "RegIrqFlags1":
			{
				ModeReady = ((r.Value >> 7) & 1) == 1;
				OnPropertyChanged("ModeReady");
				var flag = ((r.Value >> 6) & 1) == 1;
				if (!RxReady && flag)
				{
					restartRx = true;
				}
				RxReady = flag;
				OnPropertyChanged("RxReady");
				TxReady = ((r.Value >> 5) & 1) == 1;
				OnPropertyChanged("TxReady");
				PllLock = ((r.Value >> 4) & 1) == 1;
				OnPropertyChanged("PllLock");
				Rssi = ((r.Value >> 3) & 1) == 1;
				OnPropertyChanged("Rssi");
				Timeout = ((r.Value >> 2) & 1) == 1;
				OnPropertyChanged("Timeout");
				PreambleDetect = ((r.Value >> 1) & 1) == 1;
				OnPropertyChanged("PreambleDetect");
				SyncAddressMatch = (r.Value & 1) == 1;
				OnPropertyChanged("SyncAddressMatch");
				break;
			}
			case "RegIrqFlags2":
				FifoFull = ((r.Value >> 7) & 1) == 1;
				OnPropertyChanged("FifoFull");
				FifoEmpty = ((r.Value >> 6) & 1) == 1;
				OnPropertyChanged("FifoEmpty");
				FifoLevel = ((r.Value >> 5) & 1) == 1;
				OnPropertyChanged("FifoLevel");
				FifoOverrun = ((r.Value >> 4) & 1) == 1;
				OnPropertyChanged("FifoOverrun");
				PacketSent = ((r.Value >> 3) & 1) == 1;
				OnPropertyChanged("PacketSent");
				PayloadReady = ((r.Value >> 2) & 1) == 1;
				OnPropertyChanged("PayloadReady");
				CrcOk = ((r.Value >> 1) & 1) == 1;
				OnPropertyChanged("CrcOk");
				LowBat = (r.Value & 1) == 1;
				OnPropertyChanged("LowBat");
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
				MapPreambleDetect = (r.Value & 1) == 1;
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
			case "RegFormerTemp":
			{
				formerTemp = (byte)(r.Value & 0x7Fu);
				if ((r.Value & 0x80) == 128)
				{
					formerTemp *= -1m;
				}
				decimal num = (byte)(registers["RegTemp"].Value & 0x7Fu);
				if ((registers["RegTemp"].Value & 0x80) == 128)
				{
					num *= -1m;
				}
				tempDelta = num - formerTemp;
				OnPropertyChanged("TempDelta");
				OnPropertyChanged("FormerTemp");
				break;
			}
			case "RegBitrateFrac":
				BitrateFrac = r.Value & 0xFu;
				if (ModulationType == ModulationTypeEnum.FSK)
				{
					Bitrate = frequencyXo / (((registers["RegBitrateMsb"].Value << 8) | registers["RegBitrateLsb"].Value) + registers["RegBitrateFrac"].Value / 16.0m);
				}
				else
				{
					Bitrate = frequencyXo / ((registers["RegBitrateMsb"].Value << 8) | registers["RegBitrateLsb"].Value);
				}
				break;
			case "RegAgcRef":
				AgcReferenceLevel = (byte)(r.Value & 0x3Fu);
				OnPropertyChanged("AgcReference");
				OnPropertyChanged("AgcThresh1");
				OnPropertyChanged("AgcThresh2");
				OnPropertyChanged("AgcThresh3");
				OnPropertyChanged("AgcThresh4");
				OnPropertyChanged("AgcThresh5");
				break;
			case "RegAgcThresh1":
				AgcStep1 = (byte)(r.Value & 0x1Fu);
				OnPropertyChanged("AgcThresh1");
				OnPropertyChanged("AgcThresh2");
				OnPropertyChanged("AgcThresh3");
				OnPropertyChanged("AgcThresh4");
				OnPropertyChanged("AgcThresh5");
				break;
			case "RegAgcThresh2":
				AgcStep2 = (byte)(r.Value >> 4);
				AgcStep3 = (byte)(r.Value & 0xFu);
				OnPropertyChanged("AgcThresh2");
				OnPropertyChanged("AgcThresh3");
				OnPropertyChanged("AgcThresh4");
				OnPropertyChanged("AgcThresh5");
				break;
			case "RegAgcThresh3":
				AgcStep4 = (byte)(r.Value >> 4);
				AgcStep5 = (byte)(r.Value & 0xFu);
				OnPropertyChanged("AgcThresh4");
				OnPropertyChanged("AgcThresh5");
				break;
			case "RegPll":
				PllBandwidth = ((r.Value >> 6) + 1) * 75000;
				break;
			case "RegRes17":
			case "RegRes18":
			case "RegRes19":
				break;
			}
		}

		private void UpdateSyncValue()
		{
			var address = (int)registers["RegSyncValue1"].Address;
			for (var i = 0; i < packet.SyncValue.Length; i++)
			{
				packet.SyncValue[i] = (byte)registers[address + i].Value;
			}
			SyncValueCheck(packet.SyncValue);
			OnPropertyChanged("SyncValue");
		}

		private void UsbDevice_Closed(object sender, EventArgs e)
		{
			spectrumOn = false;
			isOpen = false;
			regUpdateThreadContinue = false;
			OnDisconnected();
			OnError(0, "-");
		}

		private void UsbDevice_Opened(object sender, EventArgs e)
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
    }
}
