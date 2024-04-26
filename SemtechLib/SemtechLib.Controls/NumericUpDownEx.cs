using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace SemtechLib.Controls
{
	[DesignerCategory("code")]
	public sealed class NumericUpDownEx : NumericUpDown
	{
		private TextBox tBox;

		private Control udBtn;

		private bool mouseOver;

		[Browsable(true)]
		[Category("Mouse")]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public new event EventHandler MouseEnter;

		[EditorBrowsable(EditorBrowsableState.Always)]
		[Browsable(true)]
		[Category("Mouse")]
		public new event EventHandler MouseLeave;

		public NumericUpDownEx()
		{
			tBox = (TextBox)GetPrivateField("upDownEdit");
			if (tBox == null)
			{
				throw new ArgumentNullException(GetType().FullName + ": Can't find internal TextBox field.");
			}
			udBtn = GetPrivateField("upDownButtons");
			if (udBtn == null)
			{
				throw new ArgumentNullException(GetType().FullName + ": Can't find internal UpDown buttons field.");
			}
			tBox.MouseEnter += MouseEnterLeave;
			tBox.MouseLeave += MouseEnterLeave;
			udBtn.MouseEnter += MouseEnterLeave;
			udBtn.MouseLeave += MouseEnterLeave;
			base.MouseEnter += MouseEnterLeave;
			base.MouseLeave += MouseEnterLeave;
		}

		private Control GetPrivateField(string name)
		{
			var field = GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			return (Control)field.GetValue(this);
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
