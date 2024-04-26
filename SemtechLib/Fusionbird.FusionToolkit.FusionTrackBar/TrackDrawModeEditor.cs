using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Fusionbird.FusionToolkit.FusionTrackBar
{
	public sealed class TrackDrawModeEditor : UITypeEditor
	{
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			var trackBarOwnerDrawParts = TrackBarOwnerDrawParts.None;
			if (!(value is TrackBarOwnerDrawParts) || provider == null)
			{
				return value;
			}
			var windowsFormsEditorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if (windowsFormsEditorService == null)
			{
				return value;
			}
            var checkedListBox = new CheckedListBox
            {
                BorderStyle = BorderStyle.None,
                CheckOnClick = true
            };
            checkedListBox.Items.Add("Ticks", (((FusionTrackBar)context.Instance).OwnerDrawParts & TrackBarOwnerDrawParts.Ticks) == TrackBarOwnerDrawParts.Ticks);
			checkedListBox.Items.Add("Thumb", (((FusionTrackBar)context.Instance).OwnerDrawParts & TrackBarOwnerDrawParts.Thumb) == TrackBarOwnerDrawParts.Thumb);
			checkedListBox.Items.Add("Channel", (((FusionTrackBar)context.Instance).OwnerDrawParts & TrackBarOwnerDrawParts.Channel) == TrackBarOwnerDrawParts.Channel);
			windowsFormsEditorService.DropDownControl(checkedListBox);
			var enumerator = checkedListBox.CheckedItems.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
				trackBarOwnerDrawParts |= (TrackBarOwnerDrawParts)Enum.Parse(typeof(TrackBarOwnerDrawParts), objectValue.ToString());
			}
			checkedListBox.Dispose();
			windowsFormsEditorService.CloseDropDown();
			return trackBarOwnerDrawParts;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
	}
}
