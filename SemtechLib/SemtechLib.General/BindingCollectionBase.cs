using System;
using System.Collections;
using System.ComponentModel;

namespace SemtechLib.General
{
	public abstract class BindingCollectionBase : IBindingList, IList, ICollection, IEnumerable
	{
		private ArrayList list;

		internal object pendingInsert;

		private ListChangedEventHandler listChanged;

		public int Count => list.Count;

		protected IList List => this;

		protected virtual Type ElementType => typeof(object);

		bool IBindingList.AllowEdit => true;

		bool IBindingList.AllowNew => true;

		bool IBindingList.AllowRemove => true;

		bool IBindingList.SupportsChangeNotification => true;

		bool IBindingList.SupportsSearching => false;

		bool IBindingList.SupportsSorting => false;

		bool IBindingList.IsSorted => false;

		ListSortDirection IBindingList.SortDirection => throw new NotSupportedException();

		PropertyDescriptor IBindingList.SortProperty => throw new NotSupportedException();

		bool ICollection.IsSynchronized => list.IsSynchronized;

		object ICollection.SyncRoot => list.SyncRoot;

		object IList.this[int index]
		{
			get
			{
				if (index < 0 || index >= list.Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				return list[index];
			}
			set
			{
				if (index < 0 || index >= list.Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				OnValidate(value);
				var obj = list[index];
				OnSet(index, obj, value);
				list[index] = value;
				try
				{
					OnSetComplete(index, obj, value);
				}
				catch
				{
					list[index] = obj;
					throw;
				}
                listChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemChanged, index));
            }
		}

		bool IList.IsFixedSize => list.IsFixedSize;

		bool IList.IsReadOnly => list.IsReadOnly;

		event ListChangedEventHandler IBindingList.ListChanged
		{
			add => listChanged = (ListChangedEventHandler)Delegate.Combine(listChanged, value);
			remove => listChanged = (ListChangedEventHandler)Delegate.Remove(listChanged, value);
		}

		protected BindingCollectionBase()
		{
			list = [];
			pendingInsert = null;
		}

		public void Clear()
		{
			OnClear();
			foreach (var t in list)
			{
				((EditableObject)t).SetCollection(null);
			}
			list.Clear();
			pendingInsert = null;
			OnClearComplete();
            listChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= list.Count)
			{
				throw new ArgumentOutOfRangeException();
			}
			var obj = list[index];
			OnValidate(obj);
			OnRemove(index, obj);
			((EditableObject)list[index]).SetCollection(null);
			if (pendingInsert == obj)
			{
				pendingInsert = null;
			}
			list.RemoveAt(index);
			OnRemoveComplete(index, obj);
            listChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
        }

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		protected virtual object CreateInstance()
		{
			return Activator.CreateInstance(ElementType);
		}

		object IBindingList.AddNew()
		{
			if (pendingInsert != null)
			{
				((IEditableObject)pendingInsert).CancelEdit();
			}
			var obj = CreateInstance();
			((IList)this).Add(obj);
			pendingInsert = obj;
			return obj;
		}

		void IBindingList.AddIndex(PropertyDescriptor property)
		{
			throw new NotSupportedException();
		}

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			throw new NotSupportedException();
		}

		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
			throw new NotSupportedException();
		}

		void IBindingList.RemoveSort()
		{
			throw new NotSupportedException();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			list.CopyTo(array, index);
		}

		int IList.Add(object value)
		{
			OnValidate(value);
			OnInsert(list.Count, value);
			var num = list.Add(value);
			try
			{
				OnInsertComplete(num, value);
			}
			catch
			{
				list.RemoveAt(num);
				throw;
			}
			((EditableObject)value).SetCollection(this);
            listChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemAdded, num));
            return num;
		}

		bool IList.Contains(object value)
		{
			return list.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return list.IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			if (index < 0 || index > list.Count)
			{
				throw new ArgumentOutOfRangeException();
			}
			OnValidate(value);
			OnInsert(index, value);
			list.Insert(index, value);
			try
			{
				OnInsertComplete(index, value);
			}
			catch
			{
				list.RemoveAt(index);
				throw;
			}
			((EditableObject)value).SetCollection(this);
            listChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemAdded, index));
        }

		void IList.Remove(object value)
		{
			OnValidate(value);
			var num = list.IndexOf(value);
			if (num < 0)
			{
				throw new ArgumentException();
			}
			OnRemove(num, value);
			list.RemoveAt(num);
			OnRemoveComplete(num, value);
            listChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, num));
        }

		protected virtual void OnClear()
		{
		}

		protected virtual void OnClearComplete()
		{
		}

		protected virtual void OnInsert(int index, object value)
		{
		}

		protected virtual void OnInsertComplete(int index, object value)
		{
		}

		protected virtual void OnRemove(int index, object value)
		{
		}

		protected virtual void OnRemoveComplete(int index, object value)
		{
		}

		protected virtual void OnSet(int index, object oldValue, object newValue)
		{
		}

		protected virtual void OnSetComplete(int index, object oldValue, object newValue)
		{
		}

		protected virtual void OnValidate(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
		}
	}
}
