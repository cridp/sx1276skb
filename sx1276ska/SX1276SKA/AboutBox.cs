using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SX1276SKA.Properties;

namespace SX1276SKA
{
	internal sealed class AboutBox : Form
	{
		private IContainer components;

		private TableLayoutPanel tableLayoutPanel;

		private PictureBox logoPictureBox;

		private Label labelProductName;

		private Label labelVersion;

		private Label labelCopyright;

		private Label labelCompanyName;

		private TextBox textBoxDescription;

		private Button okButton;

		private Label lblVersion;

		public string Version
		{
			get => lblVersion.Text;
			set => lblVersion.Text = $"Device version: {value}";
		}

		private string AssemblyTitle
		{
			get
			{
				var customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), inherit: false);
				if (customAttributes.Length <= 0)
					return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
				var assemblyTitleAttribute = (AssemblyTitleAttribute)customAttributes[0];
				return assemblyTitleAttribute.Title != "" ? assemblyTitleAttribute.Title : Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

		private string AssemblyDescription
		{
			get
			{
				var customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), inherit: false);
				return customAttributes.Length == 0 ? "" : ((AssemblyDescriptionAttribute)customAttributes[0]).Description;
			}
		}

		private string AssemblyProduct
		{
			get
			{
				var customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), inherit: false);
				return customAttributes.Length == 0 ? "" : ((AssemblyProductAttribute)customAttributes[0]).Product;
			}
		}

		private string AssemblyCopyright
		{
			get
			{
				var customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), inherit: false);
				return customAttributes.Length == 0 ? "" : ((AssemblyCopyrightAttribute)customAttributes[0]).Copyright;
			}
		}

		private string AssemblyCompany
		{
			get
			{
				var customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), inherit: false);
				return customAttributes.Length == 0 ? "" : ((AssemblyCompanyAttribute)customAttributes[0]).Company;
			}
		}

		public AboutBox()
		{
			InitializeComponent();
			Text = $"About {AssemblyTitle}";
			labelProductName.Text = AssemblyProduct;
			var executingAssembly = Assembly.GetExecutingAssembly();
			var version = executingAssembly.GetName().Version;
			labelVersion.Text = $"Version {version.Major.ToString()}.{version.Minor.ToString()}.{version.Build.ToString()}";
			labelCopyright.Text = AssemblyCopyright;
			labelCompanyName.Text = AssemblyCompany;
			textBoxDescription.Text = AssemblyDescription;
			lblVersion.Text = $"Device version: ";
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
			tableLayoutPanel = new TableLayoutPanel();
			logoPictureBox = new PictureBox();
			labelProductName = new Label();
			labelVersion = new Label();
			labelCopyright = new Label();
			labelCompanyName = new Label();
			textBoxDescription = new TextBox();
			okButton = new Button();
			lblVersion = new Label();
			tableLayoutPanel.SuspendLayout();
			((ISupportInitialize)logoPictureBox).BeginInit();
			SuspendLayout();
			tableLayoutPanel.ColumnCount = 2;
			tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35.90909f));
			tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64.09091f));
			tableLayoutPanel.Controls.Add(logoPictureBox, 0, 0);
			tableLayoutPanel.Controls.Add(labelProductName, 1, 0);
			tableLayoutPanel.Controls.Add(labelVersion, 1, 1);
			tableLayoutPanel.Controls.Add(labelCopyright, 1, 3);
			tableLayoutPanel.Controls.Add(labelCompanyName, 1, 4);
			tableLayoutPanel.Controls.Add(textBoxDescription, 1, 5);
			tableLayoutPanel.Controls.Add(okButton, 1, 6);
			tableLayoutPanel.Controls.Add(lblVersion, 1, 2);
			tableLayoutPanel.Dock = DockStyle.Fill;
			tableLayoutPanel.Location = new Point(9, 9);
			tableLayoutPanel.Name = "tableLayoutPanel";
			tableLayoutPanel.RowCount = 7;
			tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));
			tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));
			tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));
			tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));
			tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));
			tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40f));
			tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10f));
			tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
			tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
			tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
			tableLayoutPanel.Size = new Size(440, 265);
			tableLayoutPanel.TabIndex = 0;
			logoPictureBox.BackgroundImage = Resources.sx1276_pr;
			logoPictureBox.BackgroundImageLayout = ImageLayout.Center;
			logoPictureBox.Dock = DockStyle.Fill;
			logoPictureBox.InitialImage = null;
			logoPictureBox.Location = new Point(3, 3);
			logoPictureBox.Name = "logoPictureBox";
			tableLayoutPanel.SetRowSpan(logoPictureBox, 7);
			logoPictureBox.Size = new Size(151, 259);
			logoPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
			logoPictureBox.TabIndex = 12;
			logoPictureBox.TabStop = false;
			labelProductName.Dock = DockStyle.Fill;
			labelProductName.Location = new Point(163, 0);
			labelProductName.Margin = new Padding(6, 0, 3, 0);
			labelProductName.MaximumSize = new Size(0, 17);
			labelProductName.Name = "labelProductName";
			labelProductName.Size = new Size(274, 17);
			labelProductName.TabIndex = 0;
			labelProductName.Text = "ProductID Name";
			labelProductName.TextAlign = ContentAlignment.MiddleLeft;
			labelVersion.Dock = DockStyle.Fill;
			labelVersion.Location = new Point(163, 26);
			labelVersion.Margin = new Padding(6, 0, 3, 0);
			labelVersion.MaximumSize = new Size(0, 17);
			labelVersion.Name = "labelVersion";
			labelVersion.Size = new Size(274, 17);
			labelVersion.TabIndex = 1;
			labelVersion.Text = "Version";
			labelVersion.TextAlign = ContentAlignment.MiddleLeft;
			labelCopyright.Dock = DockStyle.Fill;
			labelCopyright.Location = new Point(163, 78);
			labelCopyright.Margin = new Padding(6, 0, 3, 0);
			labelCopyright.MaximumSize = new Size(0, 17);
			labelCopyright.Name = "labelCopyright";
			labelCopyright.Size = new Size(274, 17);
			labelCopyright.TabIndex = 2;
			labelCopyright.Text = "Copyright";
			labelCopyright.TextAlign = ContentAlignment.MiddleLeft;
			labelCompanyName.Dock = DockStyle.Fill;
			labelCompanyName.Location = new Point(163, 104);
			labelCompanyName.Margin = new Padding(6, 0, 3, 0);
			labelCompanyName.MaximumSize = new Size(0, 17);
			labelCompanyName.Name = "labelCompanyName";
			labelCompanyName.Size = new Size(274, 17);
			labelCompanyName.TabIndex = 3;
			labelCompanyName.Text = "Company Name";
			labelCompanyName.TextAlign = ContentAlignment.MiddleLeft;
			textBoxDescription.Dock = DockStyle.Fill;
			textBoxDescription.Location = new Point(163, 133);
			textBoxDescription.Margin = new Padding(6, 3, 3, 3);
			textBoxDescription.Multiline = true;
			textBoxDescription.Name = "textBoxDescription";
			textBoxDescription.ReadOnly = true;
			textBoxDescription.ScrollBars = ScrollBars.Both;
			textBoxDescription.Size = new Size(274, 100);
			textBoxDescription.TabIndex = 4;
			textBoxDescription.TabStop = false;
			textBoxDescription.Text = "Description";
			okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			okButton.DialogResult = DialogResult.Cancel;
			okButton.Location = new Point(362, 239);
			okButton.Name = "okButton";
			okButton.Size = new Size(75, 23);
			okButton.TabIndex = 5;
			okButton.Text = "&OK";
			lblVersion.Dock = DockStyle.Fill;
			lblVersion.Location = new Point(163, 52);
			lblVersion.Margin = new Padding(6, 0, 3, 0);
			lblVersion.MaximumSize = new Size(0, 17);
			lblVersion.Name = "lblVersion";
			lblVersion.Size = new Size(274, 17);
			lblVersion.TabIndex = 1;
			lblVersion.Text = "Device Version";
			lblVersion.TextAlign = ContentAlignment.MiddleLeft;
			AcceptButton = okButton;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(458, 283);
			Controls.Add(tableLayoutPanel);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "AboutBox";
			Padding = new Padding(9);
			ShowIcon = false;
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterParent;
			Text = "AboutBox";
			tableLayoutPanel.ResumeLayout(false);
			tableLayoutPanel.PerformLayout();
			((ISupportInitialize)logoPictureBox).EndInit();
			ResumeLayout(false);
		}
	}
}
