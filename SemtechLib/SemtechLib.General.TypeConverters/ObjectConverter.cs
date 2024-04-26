using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace SemtechLib.General.TypeConverters
{
	public sealed class ObjectConverter : TypeConverter
	{
		private Type objectType;

		private ObjectConverter(Type ObjectType)
		{
			objectType = ObjectType;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor) && value.GetType() == objectType)
			{
				var constructor = objectType.GetConstructor(new Type[0]);
				if (constructor != null)
				{
					return new InstanceDescriptor(constructor, new object[0], isComplete: false);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
