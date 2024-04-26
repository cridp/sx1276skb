using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SemtechLib.General.Interfaces;

namespace SX1276SKA
{
	public sealed class HelpForm : Form
	{
		private readonly string docPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName) + "\\Doc";

		private IContainer components;

		private WebBrowser docViewer;

		public HelpForm()
		{
			InitializeComponent();
			if (File.Exists(docPath + "\\overview.html"))
			{
				docViewer.Navigate(docPath + "\\overview.html");
			}
		}

		public void UpdateDocument(DocumentationChangedEventArgs e)
		{
			var text = docPath + "\\" + e.DocFolder + "\\" + e.DocName + ".html";
			if (File.Exists(text))
			{
				docViewer.Navigate(text);
			}
			else if (File.Exists(docPath + "\\overview.html"))
			{
				docViewer.Navigate(docPath + "\\overview.html");
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			var resources = new ComponentResourceManager(typeof(HelpForm));
			docViewer = new WebBrowser();
			SuspendLayout();
			docViewer.AllowWebBrowserDrop = false;
			docViewer.Dock = DockStyle.Fill;
			docViewer.IsWebBrowserContextMenuEnabled = false;
			docViewer.Location = new Point(0, 0);
			docViewer.MinimumSize = new Size(20, 20);
			docViewer.Name = "docViewer";
			docViewer.Size = new Size(292, 594);
			docViewer.TabIndex = 2;
			docViewer.TabStop = false;
			docViewer.Url = new Uri("", UriKind.Relative);
			docViewer.WebBrowserShortcutsEnabled = false;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(292, 594);
			Controls.Add(docViewer);
			Icon = (Icon)resources.GetObject("$this.Icon");
			Name = "HelpForm";
			StartPosition = FormStartPosition.Manual;
			Text = "HelpForm";
			ResumeLayout(false);
		}
	}
}
