using System;

namespace SemtechLib.General.Events
{
	public sealed class ErrorEventArgs : EventArgs
	{
        private string message;

        public byte Status { get; }

        public string Message => message;

		public ErrorEventArgs(byte status, string message)
		{
			Status = status;
			this.message = message;
		}
	}
}
