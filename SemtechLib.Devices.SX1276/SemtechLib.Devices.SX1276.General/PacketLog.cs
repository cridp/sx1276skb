using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using SemtechLib.Devices.Common.Events;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.General.Events;

namespace SemtechLib.Devices.SX1276.General
{
	public sealed class PacketLog : INotifyPropertyChanged
	{
		public enum PacketHandlerModeEnum
		{
			IDLE,
			RX,
			TX
		}

		private FileStream fileStream;

		private StreamWriter streamWriter;

		private bool state;

		private ulong samples;

		private int packetNumber;

		private int maxPacketNumber;

		private IDevice device;

		private string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		private string fileName = "sx1276-pkt.log";

		private ulong maxSamples = 1000uL;

		private readonly CultureInfo ci = CultureInfo.InvariantCulture;

		public IDevice Device
		{
			set
			{
				if (device != value)
				{
					device = value;
					device.PropertyChanged += device_PropertyChanged;
					device.PacketHandlerStarted += device_PacketHandlerStarted;
					device.PacketHandlerStoped += device_PacketHandlerStoped;
					device.PacketHandlerTransmitted += device_PacketHandlerTransmitted;
					device.PacketHandlerReceived += device_PacketHandlerReceived;
				}
			}
		}

		public string Path
		{
			get => path;
			set
			{
				path = value;
				OnPropertyChanged("Path");
			}
		}

		public string FileName
		{
			get => fileName;
			set
			{
				fileName = value;
				OnPropertyChanged("FileName");
			}
		}

		public ulong MaxSamples
		{
			get => maxSamples;
			set
			{
				maxSamples = value;
				OnPropertyChanged("MaxSamples");
			}
		}

		public event ProgressEventHandler ProgressChanged;

		public event EventHandler Stoped;

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnProgressChanged(ulong progress)
		{
            ProgressChanged?.Invoke(this, new ProgressEventArg(progress));
        }

		private void OnStop()
		{
            Stoped?.Invoke(this, EventArgs.Empty);
        }

		private void GenerateFileHeader()
		{
            string text = "#\tTime\tMode\tRssi\tPkt Max\tPkt #\tPreamble Size\tSync\tLength\tNode Address\tMessage\tCRC";
            streamWriter.WriteLine("#\tSX1276 packet log generated the " + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString());
			streamWriter.WriteLine(text);
		}

		private void Update()
		{
			var text = "\t";
			if (device == null || !state)
			{
				return;
			}
			if (samples < maxSamples || maxSamples == 0)
			{
				text = text + DateTime.Now.ToString("HH:mm:ss.fff", ci) + "\t";
				text += ((((SX1276)device).Mode == OperatingModeEnum.Tx) ? "Tx\t" : ((((SX1276)device).Mode == OperatingModeEnum.Rx) ? "Rx\t" : "\t"));
				text += ((((SX1276)device).Mode == OperatingModeEnum.Rx) ? (((SX1276)device).Packet.Rssi.ToString("F1") + "\t") : "\t");
				text = text + maxPacketNumber + "\t";
				text = text + packetNumber + "\t";
				text = text + ((SX1276)device).Packet.PreambleSize + "\t";
				var maskValidationType = new MaskValidationType(((SX1276)device).Packet.SyncValue);
				text = text + maskValidationType.StringValue + "\t";
				text = text + ((SX1276)device).Packet.MessageLength.ToString("X02") + "\t";
				text = ((((SX1276)device).Mode != OperatingModeEnum.Rx) ? (text + ((((SX1276)device).Packet.AddressFiltering != 0) ? ((SX1276)device).Packet.NodeAddress.ToString("X02") : "")) : (text + ((((SX1276)device).Packet.AddressFiltering != 0) ? ((SX1276)device).Packet.NodeAddressRx.ToString("X02") : "")));
				text += "\t";
				if (((SX1276)device).Packet.Message != null && ((SX1276)device).Packet.Message.Length != 0)
				{
					int i;
					for (i = 0; i < ((SX1276)device).Packet.Message.Length - 1; i++)
					{
						text = text + ((SX1276)device).Packet.Message[i].ToString("X02") + "-";
					}
					text = text + ((SX1276)device).Packet.Message[i].ToString("X02") + "\t";
				}
				text += (((SX1276)device).Packet.CrcOn ? ((((SX1276)device).Packet.Crc >> 8).ToString("X02") + "-" + (((SX1276)device).Packet.Crc & 0xFF).ToString("X02") + "\t") : "\t");
				streamWriter.WriteLine(text);
				if (maxSamples != 0)
				{
					samples++;
					OnProgressChanged((ulong)(samples * 100m / maxSamples));
				}
				else
				{
					OnProgressChanged(0uL);
				}
			}
			else
			{
				OnStop();
			}
		}

		public void Start()
		{
			fileStream = new FileStream(path + "\\" + fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
			streamWriter = new StreamWriter(fileStream, Encoding.ASCII);
			GenerateFileHeader();
			samples = 0uL;
			state = true;
		}

		public void Stop()
		{
			try
			{
				state = false;
				streamWriter.Close();
			}
			catch (Exception)
			{
			}
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			string propertyName;
			if ((propertyName = e.PropertyName) != null)
			{
				_ = propertyName == "RssiValue";
			}
		}

		private void device_PacketHandlerStarted(object sender, EventArgs e)
		{
		}

		private void device_PacketHandlerStoped(object sender, EventArgs e)
		{
		}

		private void device_PacketHandlerTransmitted(object sender, PacketStatusEventArg e)
		{
			maxPacketNumber = e.Max;
			packetNumber = e.Number;
			Update();
		}

		private void device_PacketHandlerReceived(object sender, PacketStatusEventArg e)
		{
			maxPacketNumber = e.Max;
			packetNumber = e.Number;
			Update();
		}

		private void OnPropertyChanged(string propName)
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
	}
}
