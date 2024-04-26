using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SemtechLib.Controls
{
	[DesignerCategory("code")]
	public sealed class GroupBoxEx : GroupBox
	{
		private bool mouseOver;

		[EditorBrowsable(EditorBrowsableState.Always)]
		[Category("Mouse")]
		[Browsable(true)]
		public new event EventHandler MouseEnter;

		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Category("Mouse")]
		public new event EventHandler MouseLeave;

		public GroupBoxEx()
		{
			base.MouseEnter += MouseEnterLeave;
			base.MouseLeave += MouseEnterLeave;
		}

		private void MouseEnterLeave(object sender, EventArgs e)
		{
			var rectangle = RectangleToScreen(ClientRectangle);
			var mousePosition = MousePosition;
			var flag = rectangle.Contains(mousePosition);
			if (!(mouseOver ^ flag))
			{
				return;
			}
			mouseOver = flag;
			if (mouseOver)
			{
                MouseEnter?.Invoke(this, EventArgs.Empty);
            }
			else
            {
                MouseLeave?.Invoke(this, EventArgs.Empty);
            }
        }
	}
}
