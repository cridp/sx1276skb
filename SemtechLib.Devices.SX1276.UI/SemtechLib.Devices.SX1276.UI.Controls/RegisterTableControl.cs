using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SemtechLib.General;

namespace SemtechLib.Devices.SX1276.UI.Controls
{
	public sealed class RegisterTableControl : UserControl
	{
		private delegate void RegisterPropertyChangedDelegate(object sender, PropertyChangedEventArgs e);

		private IContainer components;

		private ErrorProvider errorProvider;

		private Timer tmrChangeValidated;

		private readonly int LABEL_SIZE_WIDTH = 150;

		private readonly int LABEL_SIZE_HEIGHT = 20;

		private TableLayoutPanel panel;

		private Label label;

		private List<Label> labelList;

		private int tabIndex;

		private int invisibleCnt;

		private RegisterCollection registers;

		private uint split = 1u;

		private string previousValue = "";

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public RegisterCollection Registers
		{
			get => registers;
			set
			{
				registers = value;
				foreach (var item in value)
				{
					item.PropertyChanged += register_PropertyChanged;
				}
				BuildTableHeader();
				BuildTable();
			}
		}

		[DefaultValue(1)]
		public uint Split
		{
			get => split;
			set
			{
				split = value == 0 ? 1u : value;
				BuildTable();
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
			components = new Container();
			errorProvider = new ErrorProvider(components);
			tmrChangeValidated = new Timer(components);
			((ISupportInitialize)errorProvider).BeginInit();
			SuspendLayout();
			errorProvider.ContainerControl = this;
			tmrChangeValidated.Interval = 5000;
			tmrChangeValidated.Tick += tmrChangeValidated_Tick;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			Name = "RegisterTableControl";
			Size = new Size(0, 0);
			((ISupportInitialize)errorProvider).EndInit();
			ResumeLayout(false);
		}

		public RegisterTableControl()
		{
			InitializeComponent();
			registers = [];
			BuildTableHeader();
		}

		private void BuildTableHeader()
		{
			if (panel != null)
			{
				Controls.Remove(panel);
			}
			panel = new TableLayoutPanel();
			labelList = [];
			panel.SuspendLayout();
		}

		private void AddHeaderLabel(int col, int row)
		{
			for (var i = 0; i < 3; i++)
			{
                label = new Label
                {
                    AutoSize = false,
                    Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0),
                    TextAlign = ContentAlignment.MiddleCenter,
                    TabIndex = tabIndex++
                };
                labelList.Add(label);
			}
			labelList[col].Text = "Register";
			labelList[col].Size = new Size(LABEL_SIZE_WIDTH, LABEL_SIZE_HEIGHT);
			labelList[col + 1].Text = "Addr";
			labelList[col + 1].Size = new Size(39, 20);
			labelList[col + 2].Text = "Value";
			labelList[col + 2].Size = new Size(39, 20);
			panel.Controls.Add(labelList[col], col, row);
			panel.Controls.Add(labelList[col + 1], col + 1, row);
			panel.Controls.Add(labelList[col + 2], col + 2, row);
		}

		private void BuildTable()
		{
			panel.Padding = new Padding(0, 0, 0, 0);
			panel.AutoSize = true;
			panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			panel.Location = new Point(0, 0);
			panel.TabIndex = tabIndex++;
			AddHeaderLabel(0, 0);
			var num = 0;
			var num2 = 1;
			invisibleCnt = 0;
			foreach (var register in registers)
			{
				if (!register.Visible)
				{
					invisibleCnt++;
				}
			}
			foreach (var t in registers)
			{
				if (!t.Visible)
				{
					continue;
				}
				label = new Label
				{
					AutoSize = false
				};
				Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
				label.TextAlign = ContentAlignment.MiddleLeft;
				label.TabIndex = tabIndex++;
				label.Margin = new Padding(0);
				label.Size = new Size(LABEL_SIZE_WIDTH, LABEL_SIZE_HEIGHT);
				label.Text = t.Name;
				panel.Controls.Add(label, num, num2);
				label = new Label
				{
					AutoSize = false,
					Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0),
					TextAlign = ContentAlignment.MiddleCenter,
					TabIndex = tabIndex++,
					Margin = new Padding(0),
					Size = new Size(39, 20),
					Text = "0x" + t.Address.ToString("X02")
				};
				panel.Controls.Add(label, num + 1, num2);
				var textBox = new TextBox
				{
					AutoSize = false,
					Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0),
					TextAlign = HorizontalAlignment.Center,
					MaxLength = 4,
					TabIndex = tabIndex++,
					Tag = "0x" + t.Address.ToString("X02"),
					Margin = new Padding(0),
					Size = new Size(45, 20),
					Text = "0x" + t.Value.ToString("X02"),
					ReadOnly = t.ReadOnly
				};
				textBox.Validated += tBox_Validated;
				textBox.Enter += tBox_Enter;
				textBox.Validating += tBox_Validating;
				textBox.TextChanged += tBox_TextChanged;
				panel.Controls.Add(textBox, num + 2, num2++);
				if (num2 > (registers.Count - invisibleCnt) / split)
				{
					num2 = 1;
					num += 3;
					if (num < split * 3 || (registers.Count - invisibleCnt) % split != 0)
					{
						AddHeaderLabel(num, 0);
					}
				}
			}
			panel.ResumeLayout(performLayout: false);
			panel.PerformLayout();
			Controls.Add(panel);
		}

		private int GetIndexFromTextBox(TextBox tBox)
		{
			var row = 0;
			var num = 0;
			foreach (Control control in panel.Controls)
			{
				if (control is TextBox && control == tBox)
				{
					num = panel.GetColumn(control);
					row = panel.GetRow(control);
					break;
				}
			}
			var labelN = (Label)panel.GetControlFromPosition(num - 2, row);
			return registers.IndexOf(registers[labelN.Text]);
		}

		private void OnRegisterPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "Value")
			{
				return;
			}
			foreach (Control control in panel.Controls)
			{
				if (control is not TextBox textBox)
				{
					continue;
				}

				if (Convert.ToUInt32((string)textBox.Tag, 16) != ((Register)sender).Address) continue;
				if (textBox.Text != "0x" + ((Register)sender).Value.ToString("X02"))
				{
					textBox.ForeColor = Color.Red;
				}
				textBox.Text = "0x" + ((Register)sender).Value.ToString("X02");
				break;
			}
		}

		private void register_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new RegisterPropertyChangedDelegate(OnRegisterPropertyChanged), sender, e);
			}
			else
			{
				OnRegisterPropertyChanged(sender, e);
			}
		}

		private void tBox_TextChanged(object sender, EventArgs e)
		{
			var textBox = (TextBox)sender;
			try
			{
				if (textBox.Text != "0x" + registers[GetIndexFromTextBox(textBox)].Value.ToString("X02"))
				{
					textBox.ForeColor = Color.Red;
				}
				else
				{
					tmrChangeValidated.Enabled = true;
				}
				if (textBox.Text != "0x")
				{
				}
			}
			catch (Exception)
			{
			}
		}

		private void tBox_Enter(object sender, EventArgs e)
		{
			var textBox = (TextBox)sender;
			previousValue = textBox.Text;
		}

		private void tBox_Validating(object sender, CancelEventArgs e)
		{
			var textBox = (TextBox)sender;
			byte b = 0;
			var b2 = byte.MaxValue;
			try
			{
				Convert.ToByte(textBox.Text, 16);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\rInput Format: Hex 0x" + b.ToString("X02") + " - 0x" + b2.ToString("X02"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				textBox.Text = previousValue;
			}
		}

		private void tBox_Validated(object sender, EventArgs e)
		{
			var textBox = (TextBox)sender;
			var b = Convert.ToByte(textBox.Text, 16);
			if (!textBox.Text.StartsWith("0x", ignoreCase: true, null))
			{
				textBox.Text = "0x" + b.ToString("X02");
			}
			if (registers[GetIndexFromTextBox(textBox)].Value != b)
			{
				registers[GetIndexFromTextBox(textBox)].Value = b;
				tmrChangeValidated.Enabled = true;
			}
		}

		private void tmrChangeValidated_Tick(object sender, EventArgs e)
		{
			tmrChangeValidated.Enabled = false;
			foreach (Control control in panel.Controls)
			{
                if (control is TextBox textBox && registers[GetIndexFromTextBox(textBox)].Value == Convert.ToByte(textBox.Text, 16))
                {
                    textBox.ForeColor = SystemColors.WindowText;
                }
            }
		}
	}
}
