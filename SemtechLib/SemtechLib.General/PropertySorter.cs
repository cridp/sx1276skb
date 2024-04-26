using System;
using System.Collections;
using System.ComponentModel;

namespace SemtechLib.General
{
	public sealed class PropertySorter : ExpandableObjectConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			var properties = TypeDescriptor.GetProperties(value, attributes);
			var arrayList = new ArrayList();
			foreach (PropertyDescriptor item in properties)
			{
				var attribute = item.Attributes[typeof(PropertyOrderAttribute)];
				if (attribute != null)
				{
					var propertyOrderAttribute = (PropertyOrderAttribute)attribute;
					arrayList.Add(new PropertyOrderPair(item.Name, propertyOrderAttribute.Order));
				}
				else
				{
					arrayList.Add(new PropertyOrderPair(item.Name, 0));
				}
			}
			arrayList.Sort();
			var arrayList2 = new ArrayList();
			foreach (PropertyOrderPair item2 in arrayList)
			{
				arrayList2.Add(item2.Name);
			}
			return properties.Sort((string[])arrayList2.ToArray(typeof(string)));
		}
	}
}
