namespace SemtechLib.General
{
	public sealed class BindingRegister : EditableObject
	{
        public string Name { get; set; }

        public uint Address { get; set; }

        public uint Value { get; set; }

        public bool ReadOnly { get; set; }

        public BindingRegister()
		{
			Name = "";
			Address = 0u;
			Value = 0u;
		}

		public BindingRegister(string name, uint address, uint value)
		{
			Name = name;
			this.Address = address;
			Value = value;
			ReadOnly = false;
		}

		public BindingRegister(string name, uint address, uint value, bool readOnly)
		{
			Name = name;
			this.Address = address;
			Value = value;
			this.ReadOnly = readOnly;
		}
	}
}
