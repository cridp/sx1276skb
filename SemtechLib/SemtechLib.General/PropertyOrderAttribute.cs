using System;

namespace SemtechLib.General
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class PropertyOrderAttribute : Attribute
	{
        public int Order { get; }

        public PropertyOrderAttribute()
		{
		}

		public PropertyOrderAttribute(int order)
		{
			Order = order;
		}
	}
}
