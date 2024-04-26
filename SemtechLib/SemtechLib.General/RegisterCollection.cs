using System;
using System.Collections;

namespace SemtechLib.General
{
	public sealed class RegisterCollection : CollectionBase
	{
		public sealed class RegisterEnumerator : IEnumerator
		{
			private readonly IEnumerator baseEnumerator;
			
			private readonly IEnumerable temp;

			public Register Current => (Register)baseEnumerator.Current;

			object IEnumerator.Current => baseEnumerator.Current;

			public RegisterEnumerator(RegisterCollection mappings)
			{
				temp = mappings;
				baseEnumerator = temp.GetEnumerator();
			}

			public bool MoveNext()
			{
				return baseEnumerator.MoveNext();
			}

			bool IEnumerator.MoveNext()
			{
				return baseEnumerator.MoveNext();
			}

			public void Reset()
			{
				baseEnumerator.Reset();
			}

			void IEnumerator.Reset()
			{
				baseEnumerator.Reset();
			}
		}

		public Register this[int index]
		{
			get => (Register)List[index];
			set => List[index] = value;
		}

		public Register this[string name]
		{
			get
			{
				foreach (Register item in List)
				{
					if (item.Name == name)
					{
						return item;
					}
				}
				return null;
			}
			set
			{
				foreach (Register item in List)
				{
					if (item.Name == name)
					{
						List[(int)item.Address] = value;
					}
				}
			}
		}

		public event EventHandler DataInserted;

		public RegisterCollection()
		{
		}

		public RegisterCollection(RegisterCollection value)
		{
			AddRange(value);
		}

		public RegisterCollection(Register[] value)
		{
			AddRange(value);
		}

		public int Add(Register value)
		{
			return List.Add(value);
		}

		private void AddRange(Register[] value)
		{
			foreach (var t in value)
			{
				Add(t);
			}
		}

		private void AddRange(RegisterCollection value)
		{
			foreach (var t in value)
			{
				Add(t);
			}
		}

		public bool Contains(Register value)
		{
			return List.Contains(value);
		}

		public void CopyTo(Register[] array, int index)
		{
			List.CopyTo(array, index);
		}

		public int IndexOf(Register value)
		{
			return List.IndexOf(value);
		}

		public void Insert(int index, Register value)
		{
			List.Insert(index, value);
		}

		public new RegisterEnumerator GetEnumerator()
		{
			return new RegisterEnumerator(this);
		}

		public void Remove(Register value)
		{
			try
			{
				List.Remove(value);
				Capacity--;
			}
			catch (Exception)
			{
			}
		}

		public void RemoveRange(Register[] value)
		{
			foreach (var t in value)
			{
				Remove(t);
			}
		}

		public void RemoveRange(RegisterCollection value)
		{
			foreach (var t in value)
			{
				Remove(t);
			}
		}

		protected override void OnInsert(int index, object value)
		{
			base.OnInsert(index, value);
            DataInserted?.Invoke(this, EventArgs.Empty);
        }
	}
}
