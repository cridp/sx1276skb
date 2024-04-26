using System;

namespace SemtechLib.General
{
	public sealed class BindingRegisterList : BindingCollectionBase
	{
		public BindingRegister this[int Index]
		{
			get => List[Index] as BindingRegister;
			set => List[Index] = value;
		}

		protected override Type ElementType => typeof(BindingRegister);

		public int Add(BindingRegister Item)
		{
			return List.Add(Item);
		}
	}
}
