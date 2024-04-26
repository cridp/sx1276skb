using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276.UI.Forms
{
	public sealed class TemperatureCalibrationForm : Form
	{
		private IContainer components;

		private Label label1;

		private NumericUpDown nudTempRoom;

		private Label label2;

		private Label label3;

		private Button btnOk;

		public decimal TempValueRoom
		{
			get => nudTempRoom.Value;
			set => nudTempRoom.Value = value;
		}

		public TemperatureCalibrationForm()
		{
			InitializeComponent();
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
			var resources = new ComponentResourceManager(typeof(TemperatureCalibrationForm));
			label1 = new Label();
			nudTempRoom = new NumericUpDown();
			label2 = new Label();
			label3 = new Label();
			btnOk = new Button();
			((ISupportInitialize)nudTempRoom).BeginInit();
			SuspendLayout();
			label1.AutoSize = true;
			label1.Location = new Point(12, 77);
			label1.Name = "label1";
			label1.Size = new Size(125, 13);
			label1.TabIndex = 1;
			label1.Text = "Actual room temperature:";
			nudTempRoom.Location = new Point(143, 73);
			nudTempRoom.Maximum = new decimal(new int[4] { 85, 0, 0, 0 });
			nudTempRoom.Minimum = new decimal(new int[4] { 40, 0, 0, -2147483648 });
			nudTempRoom.Name = "nudTempRoom";
			nudTempRoom.Size = new Size(39, 20);
			nudTempRoom.TabIndex = 2;
			nudTempRoom.Value = new decimal(new int[4] { 25, 0, 0, 0 });
			label2.AutoSize = true;
			label2.Location = new Point(188, 77);
			label2.Name = "label2";
			label2.Size = new Size(18, 13);
			label2.TabIndex = 3;
			label2.Text = "°C";
			label3.AutoSize = true;
			label3.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label3.Location = new Point(13, 10);
			label3.MaximumSize = new Size(238, 0);
			label3.Name = "label3";
			label3.Size = new Size(218, 51);
			label3.TabIndex = 0;
			label3.Text = "Please enter the actual room temperature measured on an auxiliary thermometer!";
			label3.TextAlign = ContentAlignment.MiddleCenter;
			btnOk.DialogResult = DialogResult.OK;
			btnOk.Location = new Point(85, 99);
			btnOk.Name = "btnOk";
			btnOk.Size = new Size(75, 23);
			btnOk.TabIndex = 4;
			btnOk.Text = "OK";
			btnOk.UseVisualStyleBackColor = true;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(244, 133);
			Controls.Add(btnOk);
			Controls.Add(nudTempRoom);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(label3);
			DoubleBuffered = true;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Icon = (Icon)resources.GetObject("$this.Icon");
			KeyPreview = true;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "TemperatureCalibrationForm";
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterParent;
			Text = "Temperature Calibration";
			((ISupportInitialize)nudTempRoom).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
