using System.ComponentModel;

namespace SemtechLib.General
{
	public sealed class Register : INotifyPropertyChanged
	{
		private uint oldValue;

		private string name;

		private uint address;

		private uint value;

		private bool readOnly;

		private bool visible;

		public string Name
		{
			get => name;
			set
			{
				name = value;
				OnPropertyChanged("Name");
			}
		}

		public uint Address
		{
			get => address;
			set
			{
				address = value;
				OnPropertyChanged("Address");
			}
		}

		public uint Value
		{
			get => value;
			set
			{
				this.value = value;
				OnPropertyChanged("Value");
			}
		}

		public bool ReadOnly
		{
			get => readOnly;
			set
			{
				readOnly = value;
				OnPropertyChanged("ReadOnly");
			}
		}

		public bool Visible
		{
			get => visible;
			set
			{
				visible = value;
				OnPropertyChanged("Visible");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public Register()
		{
			name = "";
			address = 0u;
			value = 0u;
			oldValue = 0u;
		}

		public Register(string name, uint address, uint value)
		{
			this.name = name;
			this.address = address;
			this.value = value;
			oldValue = value;
			readOnly = false;
		}

		public Register(string name, uint address, uint value, bool readOnly, bool visible)
		{
			this.name = name;
			this.address = address;
			this.value = value;
			oldValue = value;
			this.readOnly = readOnly;
			this.visible = visible;
		}

		public bool IsValueChanged()
		{
			return oldValue != value;
		}

		public void ApplyValue()
		{
			oldValue = value;
		}

		private void OnPropertyChanged(string propName)
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
	}
}
