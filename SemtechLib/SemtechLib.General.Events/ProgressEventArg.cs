using System;

namespace SemtechLib.General.Events
{
	public sealed class ProgressEventArg : EventArgs
	{
        public ulong Progress { get; }

        public ProgressEventArg(ulong progress)
		{
			Progress = progress;
		}
	}
}
