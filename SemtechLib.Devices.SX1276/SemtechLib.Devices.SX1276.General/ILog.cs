using System;
using System.ComponentModel;

namespace SemtechLib.Devices.SX1276.General
{
	public interface ILog : INotifyPropertyChanged
	{
		string Path { get; set; }

		string FileName { get; set; }

		ulong MaxSamples { get; set; }

		event ProgressChangedEventHandler ProgressChanged;

		event EventHandler Stoped;

		void Start();

		void Stop();
	}
}
