using System;
using System.ComponentModel;

namespace SemtechLib.Devices.SX1276LR.General
{
	public interface ILog : INotifyPropertyChanged
	{
		event ProgressChangedEventHandler ProgressChanged;

		event EventHandler Stoped;

		bool Enabled { get; set; }

		string FileName { get; set; }

		bool IsAppend { get; set; }

		ulong MaxSamples { get; set; }

		string Path { get; set; }

		void Start();

		void Stop();
	}
}
