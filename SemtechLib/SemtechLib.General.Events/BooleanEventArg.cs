using System;

namespace SemtechLib.General.Events
{
	public sealed class BooleanEventArg : EventArgs
	{
        public bool Value { get; }

        public BooleanEventArg(bool value)
		{
			Value = value;
		}
	}
}
