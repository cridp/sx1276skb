using System.Windows.Forms;

namespace SemtechLib.Controls
{
	internal sealed class VProgressBar : ProgressBar
	{
		protected override CreateParams CreateParams
		{
			get
			{
				var createParams = base.CreateParams;
				createParams.Style |= 4;
				return createParams;
			}
		}
	}
}
