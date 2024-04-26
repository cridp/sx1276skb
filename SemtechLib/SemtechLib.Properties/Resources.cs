using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace SemtechLib.Properties
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[CompilerGenerated]
	[DebuggerNonUserCode]
	public class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static ResourceManager ResourceManager
		{
			get
			{
				if (ReferenceEquals(resourceMan, null))
				{
					var resourceManager = new ResourceManager("SemtechLib.Properties.Resources", typeof(Resources).Assembly);
					resourceMan = resourceManager;
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static CultureInfo Culture
		{
			get => resourceCulture;
			set => resourceCulture = value;
		}

		public static Bitmap About
		{
			get
			{
				var @object = ResourceManager.GetObject("About", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap Auto
		{
			get
			{
				var @object = ResourceManager.GetObject("Auto", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap AutoSelected
		{
			get
			{
				var @object = ResourceManager.GetObject("AutoSelected", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap Burn
		{
			get
			{
				var @object = ResourceManager.GetObject("Burn", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap Connected
		{
			get
			{
				var @object = ResourceManager.GetObject("Connected", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap CopyHS
		{
			get
			{
				var @object = ResourceManager.GetObject("CopyHS", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap CutHS
		{
			get
			{
				var @object = ResourceManager.GetObject("CutHS", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap Disconnected
		{
			get
			{
				var @object = ResourceManager.GetObject("Disconnected", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap ExpirationHS
		{
			get
			{
				var @object = ResourceManager.GetObject("ExpirationHS", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap graphhs
		{
			get
			{
				var @object = ResourceManager.GetObject("graphhs", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap Help
		{
			get
			{
				var @object = ResourceManager.GetObject("Help", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap Move
		{
			get
			{
				var @object = ResourceManager.GetObject("Move", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap MoveSelected
		{
			get
			{
				var @object = ResourceManager.GetObject("MoveSelected", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap openHS
		{
			get
			{
				var @object = ResourceManager.GetObject("openHS", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap PasteHS
		{
			get
			{
				var @object = ResourceManager.GetObject("PasteHS", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap Refresh
		{
			get
			{
				var @object = ResourceManager.GetObject("Refresh", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap RefreshDocViewHS
		{
			get
			{
				var @object = ResourceManager.GetObject("RefreshDocViewHS", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap SaveAllHS
		{
			get
			{
				var @object = ResourceManager.GetObject("SaveAllHS", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap saveHS
		{
			get
			{
				var @object = ResourceManager.GetObject("saveHS", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Icon semtech
		{
			get
			{
				var @object = ResourceManager.GetObject("semtech", resourceCulture);
				return (Icon)@object;
			}
		}

		public static Bitmap semtech_16_16
		{
			get
			{
				var @object = ResourceManager.GetObject("semtech_16_16", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap Tune
		{
			get
			{
				var @object = ResourceManager.GetObject("Tune", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap ZoomIn
		{
			get
			{
				var @object = ResourceManager.GetObject("ZoomIn", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap ZoomInSelected
		{
			get
			{
				var @object = ResourceManager.GetObject("ZoomInSelected", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap ZoomOut
		{
			get
			{
				var @object = ResourceManager.GetObject("ZoomOut", resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap ZoomOutSelected
		{
			get
			{
				var @object = ResourceManager.GetObject("ZoomOutSelected", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal Resources()
		{
		}
	}
}
