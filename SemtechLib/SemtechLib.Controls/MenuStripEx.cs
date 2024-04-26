using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SemtechLib.Controls
{
	public sealed class MenuStripEx : MenuStrip
	{
        private bool suppressHighlighting = true;

        [Category("Extended")]
        [DefaultValue("false")]
        public bool ClickThrough { get; set; }

        [DefaultValue("true")]
		[Category("Extended")]
		public bool SuppressHighlighting
		{
			get => suppressHighlighting;
			set => suppressHighlighting = value;
		}

		protected override void WndProc(ref Message m)
		{
			if ((long)m.Msg == 512 && suppressHighlighting && !TopLevelControl.ContainsFocus) return;
			base.WndProc(ref m);
			if ((long)m.Msg == 33 && ClickThrough && m.Result == (IntPtr)2L)
			{
				m.Result = (IntPtr)1L;
			}
		}
	}
}
