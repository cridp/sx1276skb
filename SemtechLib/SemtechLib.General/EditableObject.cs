#define TRACE
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace SemtechLib.General
{
	public class EditableObject : IEditableObject
	{
		private sealed class NotCopied
		{
            public static NotCopied Value { get; } = new();
        }

        private object[] originalValues;

        private BindingCollectionBase Collection { get; set; }

        private bool PendingInsert => Collection.pendingInsert == this;

		private bool IsEdit => originalValues != null;

		internal void SetCollection(BindingCollectionBase CollectionN)
		{
			Collection = CollectionN;
		}

		void IEditableObject.BeginEdit()
		{
			Trace.WriteLine("BeginEdit");
			if (IsEdit)
			{
				return;
			}
			var properties = TypeDescriptor.GetProperties(this, null);
			var array = new object[properties.Count];
			for (var i = 0; i < properties.Count; i++)
			{
				array[i] = NotCopied.Value;
				var propertyDescriptor = properties[i];
				if (propertyDescriptor.PropertyType.IsSubclassOf(typeof(ValueType)))
				{
					array[i] = propertyDescriptor.GetValue(this);
					continue;
				}
				var value = propertyDescriptor.GetValue(this);
				if (value == null)
				{
					array[i] = null;
				}
				else if (value is not IList && value is ICloneable cloneable)
				{
					array[i] = cloneable.Clone();
				}
			}
			originalValues = array;
		}

		void IEditableObject.CancelEdit()
		{
			Trace.WriteLine("CancelEdit");
			if (!IsEdit)
			{
				return;
			}
			if (PendingInsert)
			{
				((IList)Collection).Remove(this);
			}
			var properties = TypeDescriptor.GetProperties(this, null);
			for (var i = 0; i < properties.Count; i++)
			{
				if (originalValues[i] is not NotCopied)
				{
					properties[i].SetValue(this, originalValues[i]);
				}
			}
			originalValues = null;
		}

		void IEditableObject.EndEdit()
		{
			Trace.WriteLine("EndEdit");
			if (IsEdit)
			{
				if (PendingInsert)
				{
					Collection.pendingInsert = null;
				}
				originalValues = null;
			}
		}
	}
}
