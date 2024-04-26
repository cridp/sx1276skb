using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace SemtechLib.Devices.SX1276.General
{
	public sealed class DataLog : ILog, INotifyPropertyChanged
	{
		private FileStream fileStream;

		private StreamWriter streamWriter;

		private bool state;

		private ulong samples;

		private SX1276 device;

		private string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		private string fileName = "sx1276-Rssi.log";

		private ulong maxSamples = 1000uL;

		private readonly CultureInfo ci = CultureInfo.InvariantCulture;

		public SX1276 Device
		{
			set
			{
				if (device != value)
				{
					device = value;
					device.PropertyChanged += device_PropertyChanged;
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

		public event ProgressChangedEventHandler ProgressChanged;

		public event EventHandler Stoped;

		public event PropertyChangedEventHandler PropertyChanged;

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
            string text = device.RfPaSwitchEnabled == 0 ? "#\tTime\tRSSI" : "#\tTime\tRF_PA RSSI\tRF_IO RSSI";
            streamWriter.WriteLine("#\tSX1276 data log generated the " + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString());
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
				if (device.RfPaSwitchEnabled != 0)
				{
					var text2 = text;
					text = text2 + DateTime.Now.ToString("HH:mm:ss.fff", ci) + "\t" + device.RfPaRssiValue.ToString("F1") + "\t" + device.RfIoRssiValue.ToString("F1");
				}
				else
				{
					text = text + DateTime.Now.ToString("HH:mm:ss.fff", ci) + "\t" + device.RssiValue.ToString("F1");
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
			if (device.RfPaSwitchEnabled != 0)
			{
				switch (e.PropertyName)
				{
				case "RfPaRssiValue":
					if (device.RfPaSwitchEnabled == 1)
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
