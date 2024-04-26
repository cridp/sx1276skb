using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemtechLib.Controls.HexBoxCtrl
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public sealed class BuiltInContextMenu : Component
	{
		private HexBox _hexBox;

		private ContextMenuStrip _contextMenuStrip;

		private ToolStripMenuItem _cutToolStripMenuItem;

		private ToolStripMenuItem _copyToolStripMenuItem;

		private ToolStripMenuItem _pasteToolStripMenuItem;

		private ToolStripMenuItem _selectAllToolStripMenuItem;
        private string _cutMenuItemText;
        private string _selectAllMenuItemText;
        private Image _copyMenuItemImage;
        private Image _selectAllMenuItemImage;

        [Category("BuiltIn-ContextMenu")]
        [DefaultValue(null)]
        [Localizable(true)]
        public string CopyMenuItemText { get; set; }

        [DefaultValue(null)]
		[Localizable(true)]
		[Category("BuiltIn-ContextMenu")]
		public string CutMenuItemText
		{
			get => _cutMenuItemText;
			set => _cutMenuItemText = value;
		}

        [Localizable(true)]
        [DefaultValue(null)]
        [Category("BuiltIn-ContextMenu")]
        public string PasteMenuItemText { get; set; }

        [Localizable(true)]
		[Category("BuiltIn-ContextMenu")]
		[DefaultValue(null)]
		public string SelectAllMenuItemText
		{
			get => _selectAllMenuItemText;
			set => _selectAllMenuItemText = value;
		}

		internal string CutMenuItemTextInternal
		{
			get
			{
				if (string.IsNullOrEmpty(CutMenuItemText))
				{
					return "Cut";
				}
				return CutMenuItemText;
			}
		}

		internal string CopyMenuItemTextInternal
		{
			get
			{
				if (string.IsNullOrEmpty(CopyMenuItemText))
				{
					return "Copy";
				}
				return CopyMenuItemText;
			}
		}

		internal string PasteMenuItemTextInternal
		{
			get
			{
				if (string.IsNullOrEmpty(PasteMenuItemText))
				{
					return "Paste";
				}
				return PasteMenuItemText;
			}
		}

		internal string SelectAllMenuItemTextInternal
		{
			get
			{
				if (string.IsNullOrEmpty(SelectAllMenuItemText))
				{
					return "SelectAll";
				}
				return SelectAllMenuItemText;
			}
		}

        [DefaultValue(null)]
        [Category("BuiltIn-ContextMenu")]
        public Image CutMenuItemImage { get; set; }

        [Category("BuiltIn-ContextMenu")]
		[DefaultValue(null)]
		public Image CopyMenuItemImage
		{
			get => _copyMenuItemImage;
			set => _copyMenuItemImage = value;
		}

        [DefaultValue(null)]
        [Category("BuiltIn-ContextMenu")]
        public Image PasteMenuItemImage { get; set; }

        [Category("BuiltIn-ContextMenu")]
		[DefaultValue(null)]
		public Image SelectAllMenuItemImage
		{
			get => _selectAllMenuItemImage;
			set => _selectAllMenuItemImage = value;
		}

		internal BuiltInContextMenu(HexBox hexBox)
		{
			_hexBox = hexBox;
			_hexBox.ByteProviderChanged += HexBox_ByteProviderChanged;
		}

		private void HexBox_ByteProviderChanged(object sender, EventArgs e)
		{
			CheckBuiltInContextMenu();
		}

		private void CheckBuiltInContextMenu()
		{
			if (!Util.DesignMode)
			{
				if (_contextMenuStrip == null)
				{
					var contextMenuStrip = new ContextMenuStrip();
					_cutToolStripMenuItem = new ToolStripMenuItem(CutMenuItemTextInternal, CutMenuItemImage, CutMenuItem_Click);
					contextMenuStrip.Items.Add(_cutToolStripMenuItem);
					_copyToolStripMenuItem = new ToolStripMenuItem(CopyMenuItemTextInternal, CopyMenuItemImage, CopyMenuItem_Click);
					contextMenuStrip.Items.Add(_copyToolStripMenuItem);
					_pasteToolStripMenuItem = new ToolStripMenuItem(PasteMenuItemTextInternal, PasteMenuItemImage, PasteMenuItem_Click);
					contextMenuStrip.Items.Add(_pasteToolStripMenuItem);
					contextMenuStrip.Items.Add(new ToolStripSeparator());
					_selectAllToolStripMenuItem = new ToolStripMenuItem(SelectAllMenuItemTextInternal, SelectAllMenuItemImage, SelectAllMenuItem_Click);
					contextMenuStrip.Items.Add(_selectAllToolStripMenuItem);
					contextMenuStrip.Opening += BuildInContextMenuStrip_Opening;
					_contextMenuStrip = contextMenuStrip;
				}
				if (_hexBox.ByteProvider == null && _hexBox.ContextMenuStrip != null)
				{
					_hexBox.ContextMenuStrip = null;
				}
				else if (_hexBox.ByteProvider != null && _hexBox.ContextMenuStrip == null)
				{
					_hexBox.ContextMenuStrip = _contextMenuStrip;
				}
			}
		}

		private void BuildInContextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			_cutToolStripMenuItem.Enabled = _hexBox.CanCut();
			_copyToolStripMenuItem.Enabled = _hexBox.CanCopy();
			_pasteToolStripMenuItem.Enabled = _hexBox.CanPaste();
			_selectAllToolStripMenuItem.Enabled = _hexBox.CanSelectAll();
		}

		private void CutMenuItem_Click(object sender, EventArgs e)
		{
			_hexBox.Copy();
		}

		private void CopyMenuItem_Click(object sender, EventArgs e)
		{
			_hexBox.Copy();
		}

		private void PasteMenuItem_Click(object sender, EventArgs e)
		{
			_hexBox.Copy();
		}

		private void SelectAllMenuItem_Click(object sender, EventArgs e)
		{
			_hexBox.SelectAll();
		}
	}
}
