using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SemtechLib.Controls
{
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
	public sealed class ToolStripLed : ToolStripControlHost
	{
		public Led led => Control as Led;

		public bool Checked
		{
			get => led.Checked;
			set => led.Checked = value;
		}

		public Color LedColor
		{
			get => led.LedColor;
			set => led.LedColor = value;
		}

		public ContentAlignment LedAlign
		{
			get => led.LedAlign;
			set => led.LedAlign = value;
		}

		public Size LedSize
		{
			get => led.LedSize;
			set => led.LedSize = value;
		}

		public ToolStripLed()
			: base(new Led())
		{
		}
	}
}
