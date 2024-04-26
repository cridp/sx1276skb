using System;

namespace SemtechLib.General
{
	public sealed class PropertyOrderPair : IComparable
	{
		private int order;

        public string Name { get; } = string.Empty;

        public PropertyOrderPair()
		{
		}

		public PropertyOrderPair(string name, int order)
		{
			this.order = order;
			Name = name;
		}

		public int CompareTo(object obj)
		{
			var num = ((PropertyOrderPair)obj).order;
			if (num == order)
			{
				var strB = ((PropertyOrderPair)obj).Name;
				return string.CompareOrdinal(Name, strB);
			}
			if (num > order)
			{
				return -1;
			}
			return 1;
		}
	}
}
