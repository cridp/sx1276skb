using System;
using System.Collections.Generic;
using System.ComponentModel;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.General
{
	public sealed class Packet : INotifyPropertyChanged
	{
		private const ushort PolynomeCcitt = 4129;

		private const ushort PolynomeIbm = 32773;

		public const int MaxFifoSize = 66;

		private readonly byte[] PayloadMinLength = new byte[8] { 1, 1, 2, 2, 1, 1, 2, 2 };

		private readonly byte[] PayloadMaxLength = new byte[8] { 66, 66, 66, 66, 64, 65, 65, 50 };

		private readonly byte[] MessageMaxLength = new byte[8] { 66, 65, 65, 64, 64, 64, 64, 48 };

		private readonly byte[] MessageLengthOffset = new byte[8] { 0, 1, 1, 2, 0, 1, 1, 2 };
        private int preambleSize = 3;

		private AutoRestartRxEnum autoRestartRxOn;

		private PreamblePolarityEnum preamblePolarity;

		private bool syncOn = true;

		private FifoFillConditionEnum fifoFillCondition;

		private byte syncSize = 4;

		private byte[] syncValue = new byte[4] { 105, 129, 126, 150 };

		private PacketFormatEnum packetFormat = PacketFormatEnum.Variable;

		private DcFreeEnum dcFree;

		private bool crcOn = true;

		private bool crcAutoClearOff;

		private AddressFilteringEnum addressFiltering;

		private bool crcIbmOn = true;

		private DataModeEnum dataMode;

		private bool ioHomeOn;

		private bool ioHomePwrFrameOn;

		private bool beaconOn;

		private short payloadLength = 66;

		private short payloadLengthRx = 66;

		private byte nodeAddress;

		private byte nodeAddressRx;

		private byte broadcastAddress;

		private bool txStartCondition = true;

		private byte fifoThreshold = 15;

		private byte[] message = new byte[0];

		private decimal rssi = -127.5m;

		private bool logEnabled;

		private byte maxLengthIndex;

        public OperatingModeEnum Mode { get; set; } = OperatingModeEnum.Stdby;

        public int PreambleSize
		{
			get => preambleSize;
			set
			{
				preambleSize = value;
				OnPropertyChanged("PreambleSize");
			}
		}

		public AutoRestartRxEnum AutoRestartRxOn
		{
			get => autoRestartRxOn;
			set
			{
				autoRestartRxOn = value;
				OnPropertyChanged("AutoRestartRxOn");
			}
		}

		public PreamblePolarityEnum PreamblePolarity
		{
			get => preamblePolarity;
			set
			{
				preamblePolarity = value;
				OnPropertyChanged("PreamblePolarity");
			}
		}

		public bool SyncOn
		{
			get => syncOn;
			set
			{
				syncOn = value;
				OnPropertyChanged("SyncOn");
			}
		}

		public FifoFillConditionEnum FifoFillCondition
		{
			get => fifoFillCondition;
			set
			{
				fifoFillCondition = value;
				OnPropertyChanged("FifoFillCondition");
			}
		}

		public byte SyncSize
		{
			get => syncSize;
			set
			{
				syncSize = value;
				Array.Resize(ref syncValue, syncSize);
				OnPropertyChanged("SyncSize");
			}
		}

		public byte[] SyncValue
		{
			get => syncValue;
			set
			{
				syncValue = value;
				OnPropertyChanged("SyncValue");
			}
		}

		public PacketFormatEnum PacketFormat
		{
			get => packetFormat;
			set
			{
				packetFormat = value;
				OnPropertyChanged("PacketFormat");
				OnPropertyChanged("Crc");
			}
		}

		public DcFreeEnum DcFree
		{
			get => dcFree;
			set
			{
				dcFree = value;
				OnPropertyChanged("DcFree");
			}
		}

		public bool CrcOn
		{
			get => crcOn;
			set
			{
				crcOn = value;
				OnPropertyChanged("CrcOn");
				OnPropertyChanged("Crc");
			}
		}

		public bool CrcAutoClearOff
		{
			get => crcAutoClearOff;
			set
			{
				crcAutoClearOff = value;
				OnPropertyChanged("CrcAutoClearOff");
			}
		}

		public AddressFilteringEnum AddressFiltering
		{
			get => addressFiltering;
			set
			{
				addressFiltering = value;
				OnPropertyChanged("AddressFiltering");
				OnPropertyChanged("PayloadLength");
				OnPropertyChanged("MessageLength");
				OnPropertyChanged("Crc");
			}
		}

		public bool CrcIbmOn
		{
			get => crcIbmOn;
			set
			{
				crcIbmOn = value;
				OnPropertyChanged("CrcIbmOn");
				OnPropertyChanged("Crc");
			}
		}

		public DataModeEnum DataMode
		{
			get => dataMode;
			set
			{
				dataMode = value;
				OnPropertyChanged("DataMode");
				OnPropertyChanged("Mode");
			}
		}

		public bool IoHomeOn
		{
			get => ioHomeOn;
			set
			{
				ioHomeOn = value;
				OnPropertyChanged("IoHomeOn");
			}
		}

		public bool IoHomePwrFrameOn
		{
			get => ioHomePwrFrameOn;
			set
			{
				ioHomePwrFrameOn = value;
				OnPropertyChanged("IoHomePwrFrameOn");
			}
		}

		public bool BeaconOn
		{
			get => beaconOn;
			set
			{
				beaconOn = value;
				OnPropertyChanged("BeaconOn");
			}
		}

		public short PayloadLength
		{
			get
			{
				if (Mode == OperatingModeEnum.Rx)
				{
					return payloadLengthRx;
				}
				byte b = 0;
				if (PacketFormat == PacketFormatEnum.Variable)
				{
					b++;
				}
				if (AddressFiltering != 0)
				{
					b++;
				}
				return (byte)(b + (byte)Message.Length);
			}
			set
			{
				if (Mode == OperatingModeEnum.Rx)
				{
					payloadLengthRx = value;
				}
				else
				{
					payloadLength = value;
				}
				OnPropertyChanged("PayloadLength");
			}
		}

		public byte NodeAddress
		{
			get => nodeAddress;
			set
			{
				nodeAddress = value;
				OnPropertyChanged("NodeAddress");
				OnPropertyChanged("Crc");
			}
		}

		public byte NodeAddressRx
		{
			get => nodeAddressRx;
			set
			{
				nodeAddressRx = value;
				OnPropertyChanged("NodeAddressRx");
			}
		}

		public byte BroadcastAddress
		{
			get => broadcastAddress;
			set
			{
				broadcastAddress = value;
				OnPropertyChanged("BroadcastAddress");
			}
		}

		public bool TxStartCondition
		{
			get => txStartCondition;
			set
			{
				txStartCondition = value;
				OnPropertyChanged("TxStartCondition");
			}
		}

		public byte FifoThreshold
		{
			get => fifoThreshold;
			set
			{
				fifoThreshold = value;
				OnPropertyChanged("FifoThreshold");
			}
		}

		public byte MessageLength
		{
			get
			{
				byte b = 0;
				if (AddressFiltering != 0)
				{
					b++;
				}
				return (byte)(b + (byte)Message.Length);
			}
		}

		public byte[] Message
		{
			get => message;
			set
			{
				message = value;
				OnPropertyChanged("Message");
				OnPropertyChanged("PayloadLength");
				OnPropertyChanged("MessageLength");
				OnPropertyChanged("Crc");
			}
		}

		public ushort Crc
		{
			get
			{
				var array = new byte[0];
				var num = 0;
				var num2 = 0;
				if (PacketFormat == PacketFormatEnum.Variable)
				{
					Array.Resize(ref array, ++num);
					array[num2++] = MessageLength;
				}
				if (AddressFiltering != 0)
				{
					Array.Resize(ref array, ++num);
					if (Mode == OperatingModeEnum.Rx)
					{
						array[num2++] = NodeAddressRx;
					}
					else
					{
						array[num2++] = NodeAddress;
					}
				}
				num += Message.Length;
				Array.Resize(ref array, num);
				for (var i = 0; i < Message.Length; i++)
				{
					array[num2 + i] = Message[i];
				}
				return ComputeCrc(array);
			}
		}

		public decimal Rssi
		{
			get => rssi;
			set
			{
				rssi = value;
				OnPropertyChanged("Rssi");
			}
		}

		public bool LogEnabled
		{
			get => logEnabled;
			set
			{
				logEnabled = value;
				OnPropertyChanged("LogEnabled");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void UpdatePayloadLengthMaxMin()
		{
			maxLengthIndex = (byte)((PacketFormat == PacketFormatEnum.Variable) ? 2u : 0u);
			maxLengthIndex |= (byte)((AddressFiltering != 0) ? 1 : 0);
			if (Message.Length > MessageMaxLength[maxLengthIndex])
			{
				Array.Resize(ref message, MessageMaxLength[maxLengthIndex]);
				OnPropertyChanged("Message");
				OnPropertyChanged("MessageLength");
				OnPropertyChanged("PayloadLength");
				OnPropertyChanged("Crc");
			}
		}

		private ushort ComputeCcittCrc(ushort crc, byte data)
		{
			for (var i = 0; i < 8; i++)
			{
				if (((uint)((crc & 0x8000) >> 8) ^ (data & 0x80u)) != 0)
				{
					crc <<= 1;
					crc = (ushort)(crc ^ 0x1021u);
				}
				else
				{
					crc <<= 1;
				}
				data <<= 1;
			}
			return crc;
		}

		private ushort ComputeIbmCrc(ushort crc, byte data)
		{
			for (var i = 0; i < 8; i++)
			{
				if (((uint)((crc & 0x8000) >> 8) ^ (data & 0x80u)) != 0)
				{
					crc <<= 1;
					crc = (ushort)(crc ^ 0x8005u);
				}
				else
				{
					crc <<= 1;
				}
				data <<= 1;
			}
			return crc;
		}

		public ushort ComputeCrc(byte[] packet)
		{
			var num = (ushort)((!ioHomeOn && !ioHomePwrFrameOn) ? ((!crcIbmOn || ioHomeOn || ioHomePwrFrameOn) ? 7439 : ushort.MaxValue) : 0);
			for (var i = 0; i < packet.Length; i++)
			{
				if (ioHomeOn)
				{
					packet[i] = (byte)((ulong)((((packet[i] * 2050L) & 0x22110) | ((packet[i] * 32800L) & 0x88440)) * 65793) >> 16);
				}
				num = ((!crcIbmOn || ioHomeOn || ioHomePwrFrameOn) ? ComputeCcittCrc(num, packet[i]) : ComputeIbmCrc(num, packet[i]));
			}
			if (ioHomeOn || ioHomePwrFrameOn || crcIbmOn)
			{
				return num;
			}
			return (ushort)(~num);
		}

		public byte[] ToArray()
		{
			var list = new List<byte>();
			if (PacketFormat == PacketFormatEnum.Variable)
			{
				list.Add(MessageLength);
			}
			if (AddressFiltering != 0)
			{
				list.Add(NodeAddress);
			}
			foreach (var t in Message)
			{
				list.Add(t);
			}
			return list.ToArray();
		}

		public void SetSaveData(string data)
		{
			var array = data.Split(';');
			if (array.Length != 5)
			{
				return;
			}
			array = array[4].Split(',');
			if (message != null)
			{
				Array.Resize(ref message, array.Length);
			}
			else
			{
				message = new byte[array.Length];
			}
			for (var i = 0; i < array.Length; i++)
			{
				if (array[i].Length != 0)
				{
					message[i] = Convert.ToByte(array[i], 16);
				}
			}
			OnPropertyChanged("Message");
			OnPropertyChanged("MessageLength");
			OnPropertyChanged("PayloadLength");
			OnPropertyChanged("Crc");
			UpdatePayloadLengthMaxMin();
		}

		public string GetSaveData()
		{
			var text = ((Mode == OperatingModeEnum.Tx) ? true.ToString() : false.ToString());
			text += ";";
			text += ((AddressFiltering != 0) ? true.ToString() : false.ToString());
			text += ";";
			text = text + payloadLength + ";";
			text = text + nodeAddress + ";";
			if (message != null && message.Length != 0)
			{
				int i;
				for (i = 0; i < message.Length - 1; i++)
				{
					text = text + message[i].ToString("X02") + ",";
				}
				text += message[i].ToString("X02");
			}
			return text;
		}

		private void OnPropertyChanged(string propName)
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
	}
}
