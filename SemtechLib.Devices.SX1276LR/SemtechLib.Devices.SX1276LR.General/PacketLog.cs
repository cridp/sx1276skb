using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SemtechLib.Devices.Common.Events;
using SemtechLib.Devices.Common.Interfaces;

namespace SemtechLib.Devices.SX1276LR.General
{
	public sealed class PacketLog : ILog //, INotifyPropertyChanged
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

		private SX1276LR device;

		private bool enabled;

		private bool isAppend = true;

		private string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		private string fileName = "sx1276-LoRa-pkt.log";

		private ulong maxSamples;

		private readonly CultureInfo ci = CultureInfo.InvariantCulture;

		private bool UsePer;

		public IDevice Device
		{
			set
			{
				if (device == value) return;
				device = (SX1276LR)value;
				device.PropertyChanged += device_PropertyChanged;
				device.PacketHandlerStarted += device_PacketHandlerStarted;
				device.PacketHandlerStoped += device_PacketHandlerStoped;
				device.PacketHandlerTransmitted += device_PacketHandlerTransmitted;
				device.PacketHandlerReceived += device_PacketHandlerReceived;
			}
		}

		public bool Enabled
		{
			get => enabled;
			set
			{
				if (enabled == value) return;
				enabled = value;
				OnPropertyChanged(nameof(Enabled));
			}
		}

		public bool IsAppend
		{
			get => isAppend;
			set
			{
				if (isAppend == value) return;
				isAppend = value;
				OnPropertyChanged(nameof(IsAppend));
			}
		}

		public string Path
		{
			get => path;
			set
			{
				if (path == value) return;
				path = value;
				OnPropertyChanged(nameof(Path));
			}
		}

		public string FileName
		{
			get => fileName;
			set
			{
				if (fileName == value) return;
				fileName = value;
				OnPropertyChanged(nameof(FileName));
			}
		}

		public ulong MaxSamples
		{
			get => maxSamples;
			set
			{
				if (maxSamples == value) return;
				maxSamples = value;
				OnPropertyChanged(nameof(MaxSamples));
			}
		}

		public event ProgressChangedEventHandler ProgressChanged;

		public event EventHandler Stoped;

		public event PropertyChangedEventHandler PropertyChanged;

		public PacketLog()
		{
		}

		public PacketLog(IDevice device)
		{
			Device = device;
		}

		private void OnProgressChanged(int progress)
		{
            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(progress, new object()));
        }

		private void OnStop()
		{
            Stoped?.Invoke(this, EventArgs.Empty);
        }

		private void GenerateFileHeader()
		{
			var text = "";
			streamWriter.WriteLine("#\tSX1276LR LoRa mode packet log generated the " + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString());
			if (device.PacketModeTx)
			{
				text = "#\tTime\tPkt #\tMode\tPayload";
			}
			else
			{
				UsePer = device.PacketUsePer;
				text = (UsePer ? "#\tTime\tPkt #\tMode\tPayload\tCRC\tPkt Rssi [dBm]\tSNR [dB]\tHDR cnt\tPKT cnt\tRx CRC ON\tRx CR\tRx Len\tIRQ\tPER cnt\t" : "#\tTime\tPkt #\tMode\tPayload\tCRC\tPkt Rssi [dBm]\tSNR [dB]\tHDR cnt\tPKT cnt\tRx CRC ON\tRx CR\tRx Len\tIRQ");
			}
			streamWriter.WriteLine(text);
		}

		private void Update()
		{
			if (!Enabled)
			{
				return;
			}
			var text = "\t";
			if (device == null || streamWriter == null || !state)
			{
				return;
			}
			if (samples < maxSamples || maxSamples == 0)
			{
				text = text + DateTime.Now.ToString("HH:mm:ss.fff", ci) + "\t";
				text = text + packetNumber + "\t";
				text += (device.PacketModeTx ? "Tx\t" : "Rx\t");
				if (device.Payload != null && device.Payload.Length != 0)
				{
					int i;
					for (i = 0; i < device.Payload.Length - 1; i++)
					{
						text = text + device.Payload[i].ToString("X02") + "-";
					}
					text = text + device.Payload[i].ToString("X02") + "\t";
				}
				if (!device.PacketModeTx)
				{
					text = (device.ImplicitHeaderModeOn ? ((!device.PayloadCrcOn) ? (text + "OFF\t") : (text + (device.PayloadCrcError ? "KO\t" : "OK\t"))) : ((!device.RxPayloadCrcOn) ? (text + "OFF\t") : (text + (device.PayloadCrcError ? "KO\t" : "OK\t"))));
				}
				text += ((!device.PacketModeTx) ? (device.PktRssiValue.ToString("F1") + "\t") : "\t");
				text = (device.IsDebugOn ? (text + ((!device.PacketModeTx) ? (device.PktSnrValue.ToString("F1") + "\t") : "\t")) : (text + (device.PacketModeTx ? "\t" : ((device.PktSnrValue > 0) ? "> 0\t" : (device.PktSnrValue.ToString("F1") + "\t")))));
				if (!device.PacketModeTx)
				{
					text = text + device.ValidHeaderCnt + "\t";
					text = text + device.ValidPacketCnt + "\t";
					text = text + device.RxPayloadCrcOn + "\t";
					switch (device.RxPayloadCodingRate)
					{
					case 1:
						text += "4/5\t";
						break;
					case 2:
						text += "4/6\t";
						break;
					case 3:
						text += "4/7\t";
						break;
					case 4:
						text += "4/8\t";
						break;
					}
					text = ((!device.ImplicitHeaderModeOn) ? (text + device.RxNbBytes + "\t") : (text + device.PayloadLength + "\t"));
					text = text + device.Registers["RegIrqFlags"].Value.ToString("X02") + "\t";
					if (UsePer)
					{
						if (device.Payload != null)
							text = text + ((device.Payload[1] << 24) | (device.Payload[2] << 16) |
							               (device.Payload[3] << 8) | device.Payload[4]) + "\t";
					}
				}
				if (device.IsDebugOn)
				{
					text += "\tLNA Gain: ";
					foreach (var pktLnaValue in device.PktLnaValues)
					{
						text = text + pktLnaValue + ", ";
					}
                    _ = text.TrimEnd(',');
					text += "\tRssi Values: ";
					foreach (var pktRssiValue in device.PktRssiValues)
					{
						text = text + pktRssiValue.ToString("F1") + ", ";
					}
                    _ = text.TrimEnd(',');
				}
				streamWriter.WriteLine(text);
				if (maxSamples != 0)
				{
					samples++;
					OnProgressChanged((int)(samples * 100m / maxSamples));
				}
				else
				{
					OnProgressChanged(0);
				}
			}
			else
			{
				OnStop();
			}
		}

		public void Start()
		{
			try
			{
				if (!Enabled)
				{
					return;
				}
				if (streamWriter != null)
				{
					streamWriter.Close();
					streamWriter = null;
				}
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream = null;
				}
				if (!IsAppend)
				{
					if (File.Exists(path + "\\" + fileName))
					{
						switch (MessageBox.Show("Packet log file already exists.\nDo you want to replace it?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk))
						{
						case DialogResult.No:
						{
                                    var saveFileDialog = new SaveFileDialog
                                    {
                                        DefaultExt = "*.log",
                                        Filter = "Log Files(*.log)|*.log|AllFiles(*.*)|*.*",
                                        InitialDirectory = device.PacketHandlerLog.Path,
                                        FileName = device.PacketHandlerLog.FileName
                                    };
                                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                                    {
	                                    var array = saveFileDialog.FileName.Split('\\');
	                                    device.PacketHandlerLog.FileName = array[array.Length - 1];
	                                    device.PacketHandlerLog.Path = "";
	                                    int i;
	                                    for (i = 0; i < array.Length - 2; i++)
	                                    {
		                                    var packetHandlerLog = device.PacketHandlerLog;
		                                    packetHandlerLog.Path = packetHandlerLog.Path + array[i] + "\\";
	                                    }
	                                    device.PacketHandlerLog.Path += array[i];
                                    }
                                    break;
						}
						default:
							throw new Exception("Wrong dialog box return code.");
						case DialogResult.Yes:
							break;
						}
					}
					fileStream = new FileStream(path + "\\" + fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
				}
				else
				{
					fileStream = new FileStream(path + "\\" + fileName, FileMode.Append, FileAccess.Write, FileShare.Read);
				}
                streamWriter = new StreamWriter(fileStream, Encoding.ASCII)
                {
                    AutoFlush = true
                };
                GenerateFileHeader();
				samples = 0uL;
				state = true;
			}
			catch (Exception)
			{
				Stop();
                throw;
            }
		}

		public void Stop()
		{
			try
			{
				state = false;
				if (streamWriter != null)
				{
					streamWriter.Close();
					streamWriter = null;
				}
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream = null;
				}
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
