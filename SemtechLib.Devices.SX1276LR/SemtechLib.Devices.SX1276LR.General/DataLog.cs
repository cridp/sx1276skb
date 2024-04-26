using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using SemtechLib.Devices.Common.Interfaces;

namespace SemtechLib.Devices.SX1276LR.General
{
	public sealed class DataLog : ILog //, INotifyPropertyChanged
	{
		private FileStream fileStream;

		private StreamWriter streamWriter;

		private bool state;

		private ulong samples;

		private IDevice device;

		private bool enabled;

		private bool isAppend = true;

		private string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		private string fileName = "sx1276LR-Rssi.log";

		private ulong maxSamples = 1000uL;

		private readonly CultureInfo ci = CultureInfo.InvariantCulture;

		public IDevice Device
		{
			set
			{
				if (device == value) return;
				device = (SX1276LR)value;
				device.PropertyChanged += device_PropertyChanged;
			}
		}

		public bool Enabled
		{
			get => enabled;
			set
			{
				if (enabled != value)
				{
					enabled = value;
					OnPropertyChanged("Enabled");
				}
			}
		}

		public bool IsAppend
		{
			get => isAppend;
			set
			{
				isAppend = value;
				OnPropertyChanged("IsAppend");
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

		public event ProgressChangedEventHandler ProgressChanged;

		public event EventHandler Stoped;

		public event PropertyChangedEventHandler PropertyChanged;

		public DataLog()
		{
		}

		public DataLog(IDevice device)
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
            string text = ((SX1276LR)device).RfPaSwitchEnabled == 0 ? "#\tTime\tRSSI" : "#\tTime\tRF_PA RSSI\tRF_IO RSSI";
            streamWriter.WriteLine("#\tSX1276LR data log generated the " + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString());
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
				if (((SX1276LR)device).RfPaSwitchEnabled != 0)
				{
					var text2 = text;
					text = text2 + DateTime.Now.ToString("HH:mm:ss.fff", ci) + "\t" + ((SX1276LR)device).RfPaRssiValue.ToString("F1") + "\t" + ((SX1276LR)device).RfIoRssiValue.ToString("F1");
				}
				else
				{
					text = text + DateTime.Now.ToString("HH:mm:ss.fff", ci) + "\t" + ((SX1276LR)device).RssiValue.ToString("F1");
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

		public void Add(object value)
		{
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
			if (((SX1276LR)device).RfPaSwitchEnabled != 0)
			{
				switch (e.PropertyName)
				{
				case "RfPaRssiValue":
					if (((SX1276LR)device).RfPaSwitchEnabled == 1)
					{
						Update();
					}
					break;
				case "RfIoRssiValue":
					Update();
					break;
				}
			}
			else if ((propertyName = e.PropertyName) != null && propertyName == "RssiValue")
			{
				Update();
			}
		}

		private void OnPropertyChanged(string propName)
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
	}
}
