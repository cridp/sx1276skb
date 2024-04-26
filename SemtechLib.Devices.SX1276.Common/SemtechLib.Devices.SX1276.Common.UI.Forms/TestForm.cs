using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SemtechLib.Controls;
using SemtechLib.Devices.Common.Interfaces;

namespace SemtechLib.Devices.SX1276.Common.UI.Forms
{
	public sealed class TestForm : Form
	{
		private delegate void DeviceDataChangedDelegate(object sender, PropertyChangedEventArgs e);

		private byte currentAddrValue;

		private byte currentDataValue;

		private byte newAddrValue;

		private byte newDataValue;

		private IDevice device;

		private SX1276 device1;

		private SemtechLib.Devices.SX1276LR.SX1276LR device2;

		private bool testEnabled;

		private string previousValue = "";

		private IContainer components;

		private TableLayoutPanel tlRegisters;

		private Button btnWrite;

		private Button btnRead;

		private StatusStrip ssStatus;

		private ToolStripStatusLabel tsLblStatus;

		private Label lblAddress;

		private Label lblDataWrite;

		private TextBox tBoxRegAddress;

		private TextBox tBoxRegValue;

		private GroupBoxEx groupBox3;

		private BackgroundWorker backgroundWorker1;

		private GroupBoxEx groupBox4;

		private Panel pnlRfPaSwitchEnable;

		private RadioButton rBtnRfPaSwitchAuto;

		private RadioButton rBtnRfPaSwitchOff;

		private Label label2;

		private Label label1;

		private Label label4;

		private Label label3;

		private RadioButton rBtnRfPaSwitchManual;

		private PictureBox pBoxRfOut4;

		private PictureBox pBoxRfOut3;

		private PictureBox pBoxRfOut2;

		private PictureBox pBoxRfOut1;

		private Panel pnlRfPaSwitchSel;

		private RadioButton rBtnRfPaSwitchPaIo;

		private RadioButton rBtnRfPaSwitchIoPa;

		private Label label44;

		private Label label5;

		private Label label34;

		private Label label32;

		private Label label43;

		private Label label6;

		private Label label35;

		private Label label37;

		private Label label42;

		private Label label33;

		private Label label38;

		private Label label36;

		private Label label41;

		private Label label31;

		private Label label39;

		private Label label40;

		public IDevice Device
		{
			set
			{
				if (device == value)
				{
					return;
				}

				switch (value)
				{
					case SX1276 sX:
						device = device1 = sX;
						device.PropertyChanged += device_PropertyChanged;
						switch (device1.RfPaSwitchEnabled)
						{
							case 2:
								rBtnRfPaSwitchAuto.Checked = true;
								rBtnRfPaSwitchManual.Checked = false;
								rBtnRfPaSwitchOff.Checked = false;
								break;
							case 1:
								rBtnRfPaSwitchAuto.Checked = false;
								rBtnRfPaSwitchManual.Checked = true;
								rBtnRfPaSwitchOff.Checked = false;
								break;
							case 0:
								rBtnRfPaSwitchAuto.Checked = false;
								rBtnRfPaSwitchManual.Checked = false;
								rBtnRfPaSwitchOff.Checked = true;
								break;
						}

						break;
					case SemtechLib.Devices.SX1276LR.SX1276LR lR:
						device = device2 = lR;
						device.PropertyChanged += device_PropertyChanged;
						switch (device2.RfPaSwitchEnabled)
						{
							case 2:
								rBtnRfPaSwitchAuto.Checked = true;
								rBtnRfPaSwitchManual.Checked = false;
								rBtnRfPaSwitchOff.Checked = false;
								break;
							case 1:
								rBtnRfPaSwitchAuto.Checked = false;
								rBtnRfPaSwitchManual.Checked = true;
								rBtnRfPaSwitchOff.Checked = false;
								break;
							case 0:
								rBtnRfPaSwitchAuto.Checked = false;
								rBtnRfPaSwitchManual.Checked = false;
								rBtnRfPaSwitchOff.Checked = true;
								break;
						}

						break;
				}
			}
		}

		public bool TestEnabled
		{
			get => testEnabled;
			set
			{
				testEnabled = value;
				OnTestEnabledChanged(EventArgs.Empty);
			}
		}

		public event EventHandler TestEnabledChanged;

		public TestForm()
		{
			InitializeComponent();
		}

		private void UpdatePaSwitchSelCheck()
		{
			pBoxRfOut1.Visible = false;
			pBoxRfOut2.Visible = false;
			pBoxRfOut3.Visible = false;
			pBoxRfOut4.Visible = false;
			if (device is SX1276)
			{
				if (device1.Mode == Enumerations.OperatingModeEnum.Tx)
				{
					switch (device1.PaSelect)
					{
					case Enumerations.PaSelectEnum.RFO:
						switch (device1.RfPaSwitchSel)
						{
						case Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO:
							pBoxRfOut2.Visible = true;
							break;
						case Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST:
							pBoxRfOut4.Visible = true;
							break;
						}
						break;
					case Enumerations.PaSelectEnum.PA_BOOST:
						switch (device1.RfPaSwitchSel)
						{
						case Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO:
							pBoxRfOut1.Visible = true;
							break;
						case Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST:
							pBoxRfOut3.Visible = true;
							break;
						}
						break;
					}
				}
				else
				{
					switch (device1.RfPaSwitchSel)
					{
					case Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO:
						pBoxRfOut2.Visible = true;
						break;
					case Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST:
						pBoxRfOut4.Visible = true;
						break;
					}
				}
			}
			else
			{
				if (device is not SX1276LR.SX1276LR)
				{
					return;
				}
				if (device2.Mode == SX1276LR.Enumerations.OperatingModeEnum.Tx)
				{
					switch (device2.PaSelect)
					{
					case SX1276LR.Enumerations.PaSelectEnum.RFO:
						switch (device2.RfPaSwitchSel)
						{
						case SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO:
							pBoxRfOut2.Visible = true;
							break;
						case SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST:
							pBoxRfOut4.Visible = true;
							break;
						}
						break;
					case SX1276LR.Enumerations.PaSelectEnum.PA_BOOST:
						switch (device2.RfPaSwitchSel)
						{
						case SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO:
							pBoxRfOut1.Visible = true;
							break;
						case SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST:
							pBoxRfOut3.Visible = true;
							break;
						}
						break;
					}
				}
				else
				{
					switch (device2.RfPaSwitchSel)
					{
					case SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO:
						pBoxRfOut2.Visible = true;
						break;
					case SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST:
						pBoxRfOut4.Visible = true;
						break;
					}
				}
			}
		}

		private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
			case "Mode":
			case "PaSelect":
				UpdatePaSwitchSelCheck();
				break;
			case "RfPaSwitchSel":
				rBtnRfPaSwitchPaIo.CheckedChanged -= rBtnRfPaSwitchSel_CheckedChanged;
				rBtnRfPaSwitchIoPa.CheckedChanged -= rBtnRfPaSwitchSel_CheckedChanged;
				switch (device)
				{
					case SX1276:
					{
						if (device1.RfPaSwitchEnabled != 2)
						{
							UpdatePaSwitchSelCheck();
							switch (device1.RfPaSwitchSel)
							{
								case Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO:
									rBtnRfPaSwitchPaIo.Checked = true;
									rBtnRfPaSwitchIoPa.Checked = false;
									break;
								case Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST:
									rBtnRfPaSwitchPaIo.Checked = false;
									rBtnRfPaSwitchIoPa.Checked = true;
									break;
							}
						}

						break;
					}
					case SX1276LR.SX1276LR when device2.RfPaSwitchEnabled != 2:
						UpdatePaSwitchSelCheck();
						switch (device2.RfPaSwitchSel)
						{
							case SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO:
								rBtnRfPaSwitchPaIo.Checked = true;
								rBtnRfPaSwitchIoPa.Checked = false;
								break;
							case SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST:
								rBtnRfPaSwitchPaIo.Checked = false;
								rBtnRfPaSwitchIoPa.Checked = true;
								break;
						}

						break;
				}
				rBtnRfPaSwitchPaIo.CheckedChanged += rBtnRfPaSwitchSel_CheckedChanged;
				rBtnRfPaSwitchIoPa.CheckedChanged += rBtnRfPaSwitchSel_CheckedChanged;
				break;
			case "RfPaSwitchEnabled":
				rBtnRfPaSwitchAuto.CheckedChanged -= rBtnRfPaSwitchEnable_CheckedChanged;
				rBtnRfPaSwitchManual.CheckedChanged -= rBtnRfPaSwitchEnable_CheckedChanged;
				rBtnRfPaSwitchOff.CheckedChanged -= rBtnRfPaSwitchEnable_CheckedChanged;
				switch (device)
				{
					case SX1276:
						switch (device1.RfPaSwitchEnabled)
						{
							case 2:
								rBtnRfPaSwitchAuto.Checked = true;
								rBtnRfPaSwitchManual.Checked = false;
								rBtnRfPaSwitchOff.Checked = false;
								break;
							case 1:
								rBtnRfPaSwitchAuto.Checked = false;
								rBtnRfPaSwitchManual.Checked = true;
								rBtnRfPaSwitchOff.Checked = false;
								break;
							case 0:
								rBtnRfPaSwitchAuto.Checked = false;
								rBtnRfPaSwitchManual.Checked = false;
								rBtnRfPaSwitchOff.Checked = true;
								break;
						}

						break;
					case SX1276LR.SX1276LR:
						switch (device2.RfPaSwitchEnabled)
						{
							case 2:
								rBtnRfPaSwitchAuto.Checked = true;
								rBtnRfPaSwitchManual.Checked = false;
								rBtnRfPaSwitchOff.Checked = false;
								break;
							case 1:
								rBtnRfPaSwitchAuto.Checked = false;
								rBtnRfPaSwitchManual.Checked = true;
								rBtnRfPaSwitchOff.Checked = false;
								break;
							case 0:
								rBtnRfPaSwitchAuto.Checked = false;
								rBtnRfPaSwitchManual.Checked = false;
								rBtnRfPaSwitchOff.Checked = true;
								break;
						}

						break;
				}
				pnlRfPaSwitchSel.Enabled = rBtnRfPaSwitchManual.Checked;
				rBtnRfPaSwitchAuto.CheckedChanged += rBtnRfPaSwitchEnable_CheckedChanged;
				rBtnRfPaSwitchManual.CheckedChanged += rBtnRfPaSwitchEnable_CheckedChanged;
				rBtnRfPaSwitchOff.CheckedChanged += rBtnRfPaSwitchEnable_CheckedChanged;
				break;
			}
		}

		private void OnTestEnabledChanged(EventArgs e)
		{
            TestEnabledChanged?.Invoke(this, e);
        }

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new DeviceDataChangedDelegate(OnDevicePropertyChanged), sender, e);
			}
			else
			{
				OnDevicePropertyChanged(sender, e);
			}
		}

		private void TestForm_Load(object sender, EventArgs e)
		{
		}

		private void TestForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			Hide();
			TestEnabled = false;
		}

		private void TestForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				e.Handled = true;
				SendKeys.Send("{TAB}");
			}
			else if (e.KeyData == (Keys.T | Keys.Control | Keys.Alt))
			{
				Hide();
				TestEnabled = false;
			}
		}

		private void TestForm_Activated(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				tsLblStatus.Text = "-";
				Refresh();
			}
			catch (Exception ex)
			{
				tsLblStatus.Text = ex.Message;
			}
			finally
			{
				Refresh();
				Cursor = Cursors.Default;
			}
		}

		private void btnWrite_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				tsLblStatus.Text = "-";
				Refresh();
				var flag = device switch
				{
					SX1276 => device1.Write(newAddrValue, newDataValue),
					SX1276LR.SX1276LR => device2.Write(newAddrValue, newDataValue),
					_ => false
				};
				if (flag)
				{
					currentAddrValue = newAddrValue;
					tBoxRegAddress.ForeColor = SystemColors.WindowText;
					currentDataValue = newDataValue;
					tBoxRegValue.ForeColor = SystemColors.WindowText;
					return;
				}
				throw new Exception("ERROR: Writing command");
			}
			catch (Exception ex)
			{
				tsLblStatus.Text = ex.Message;
			}
			finally
			{
				Refresh();
				Cursor = Cursors.Default;
			}
		}

		private void btnRead_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				tsLblStatus.Text = "-";
				Refresh();
				byte data = 0;
				var flag = device switch
				{
					SX1276 => device1.Read(newAddrValue, ref data),
					SX1276LR.SX1276LR => device2.Read(newAddrValue, ref data),
					_ => false
				};
				switch (flag)
				{
					case true:
						currentAddrValue = newAddrValue;
						tBoxRegAddress.ForeColor = SystemColors.WindowText;
						tBoxRegValue.Text = "0x" + data.ToString("X02");
						currentDataValue = newDataValue = data;
						tBoxRegValue.ForeColor = SystemColors.WindowText;
						return;
					default:
						throw new Exception("ERROR: Reading command");
				}
			}
			catch (Exception ex)
			{
				tsLblStatus.Text = ex.Message;
			}
			finally
			{
				Refresh();
				Cursor = Cursors.Default;
			}
		}

		private void tBox_TextChanged(object sender, EventArgs e)
		{
			var textBox = (TextBox)sender;
			try
			{
				if (textBox == tBoxRegAddress)
				{
					var b = currentAddrValue;
					textBox.ForeColor = textBox.Text != "0x" + b.ToString("X02") ? Color.Red : SystemColors.WindowText;
					if (textBox.Text != "0x")
					{
						newAddrValue = Convert.ToByte(textBox.Text, 16);
					}
				}
				else if (textBox == tBoxRegValue)
				{
					var b2 = currentDataValue;
					textBox.ForeColor = textBox.Text != "0x" + b2.ToString("X02") ? Color.Red : SystemColors.WindowText;
					if (textBox.Text != "0x")
					{
						newDataValue = Convert.ToByte(textBox.Text, 16);
					}
				}
			}
			catch (Exception)
			{
				
			}
		}

		private void txtBox_Enter(object sender, EventArgs e)
		{
			var textBox = (TextBox)sender;
			previousValue = textBox.Text;
		}

		private void txtBox_Validating(object sender, CancelEventArgs e)
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

		private void txtBox_Validated(object sender, EventArgs e)
		{
			var textBox = (TextBox)sender;
			textBox.Text = "0x" + Convert.ToByte(textBox.Text, 16).ToString("X02");
		}

		private void rBtnRfPaSwitchEnable_CheckedChanged(object sender, EventArgs e)
		{
			switch (device)
			{
				case SX1276 when rBtnRfPaSwitchAuto.Checked:
					device1.RfPaSwitchEnabled = 2;
					break;
				case SX1276 when rBtnRfPaSwitchManual.Checked:
					device1.RfPaSwitchEnabled = 1;
					break;
				case SX1276:
					device1.RfPaSwitchEnabled = 0;
					break;
				case SX1276LR.SX1276LR when rBtnRfPaSwitchAuto.Checked:
					device2.RfPaSwitchEnabled = 2;
					break;
				case SX1276LR.SX1276LR when rBtnRfPaSwitchManual.Checked:
					device2.RfPaSwitchEnabled = 1;
					break;
				case SX1276LR.SX1276LR:
					device2.RfPaSwitchEnabled = 0;
					break;
			}

			pnlRfPaSwitchSel.Enabled = rBtnRfPaSwitchManual.Checked;
		}

		private void rBtnRfPaSwitchSel_CheckedChanged(object sender, EventArgs e)
		{
			switch (device)
			{
				case SX1276:
					device1.RfPaSwitchSel = !rBtnRfPaSwitchPaIo.Checked ? Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST : Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO;
					break;
				case SX1276LR.SX1276LR:
					device2.RfPaSwitchSel = !rBtnRfPaSwitchPaIo.Checked ? SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST : SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO;
					break;
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
			var resources = new ComponentResourceManager(typeof(TestForm));
			ssStatus = new StatusStrip();
			tsLblStatus = new ToolStripStatusLabel();
			backgroundWorker1 = new BackgroundWorker();
			groupBox4 = new GroupBoxEx();
			pBoxRfOut4 = new PictureBox();
			pBoxRfOut3 = new PictureBox();
			pBoxRfOut2 = new PictureBox();
			pBoxRfOut1 = new PictureBox();
			pnlRfPaSwitchSel = new Panel();
			rBtnRfPaSwitchPaIo = new RadioButton();
			rBtnRfPaSwitchIoPa = new RadioButton();
			label44 = new Label();
			label5 = new Label();
			label34 = new Label();
			label32 = new Label();
			label43 = new Label();
			label6 = new Label();
			label35 = new Label();
			label37 = new Label();
			label42 = new Label();
			label33 = new Label();
			label38 = new Label();
			label36 = new Label();
			label41 = new Label();
			label31 = new Label();
			label39 = new Label();
			label40 = new Label();
			label4 = new Label();
			label3 = new Label();
			label2 = new Label();
			label1 = new Label();
			pnlRfPaSwitchEnable = new Panel();
			rBtnRfPaSwitchAuto = new RadioButton();
			rBtnRfPaSwitchManual = new RadioButton();
			rBtnRfPaSwitchOff = new RadioButton();
			groupBox3 = new GroupBoxEx();
			btnRead = new Button();
			tlRegisters = new TableLayoutPanel();
			lblAddress = new Label();
			lblDataWrite = new Label();
			tBoxRegAddress = new TextBox();
			tBoxRegValue = new TextBox();
			btnWrite = new Button();
			ssStatus.SuspendLayout();
			groupBox4.SuspendLayout();
			((ISupportInitialize)pBoxRfOut4).BeginInit();
			((ISupportInitialize)pBoxRfOut3).BeginInit();
			((ISupportInitialize)pBoxRfOut2).BeginInit();
			((ISupportInitialize)pBoxRfOut1).BeginInit();
			pnlRfPaSwitchSel.SuspendLayout();
			pnlRfPaSwitchEnable.SuspendLayout();
			groupBox3.SuspendLayout();
			tlRegisters.SuspendLayout();
            SuspendLayout();
			ssStatus.Items.AddRange(new ToolStripItem[] { tsLblStatus });
			ssStatus.Location = new Point(0, 298);
			ssStatus.Name = "ssStatus";
			ssStatus.Size = new Size(319, 22);
			ssStatus.TabIndex = 1;
			ssStatus.Text = "statusStrip1";
			tsLblStatus.Name = "tsLblStatus";
			tsLblStatus.Size = new Size(12, 17);
			tsLblStatus.Text = "-";
			backgroundWorker1.WorkerReportsProgress = true;
			groupBox4.Controls.Add(pBoxRfOut4);
			groupBox4.Controls.Add(pBoxRfOut3);
			groupBox4.Controls.Add(pBoxRfOut2);
			groupBox4.Controls.Add(pBoxRfOut1);
			groupBox4.Controls.Add(pnlRfPaSwitchSel);
			groupBox4.Controls.Add(label44);
			groupBox4.Controls.Add(label5);
			groupBox4.Controls.Add(label34);
			groupBox4.Controls.Add(label32);
			groupBox4.Controls.Add(label43);
			groupBox4.Controls.Add(label6);
			groupBox4.Controls.Add(label35);
			groupBox4.Controls.Add(label37);
			groupBox4.Controls.Add(label42);
			groupBox4.Controls.Add(label33);
			groupBox4.Controls.Add(label38);
			groupBox4.Controls.Add(label36);
			groupBox4.Controls.Add(label41);
			groupBox4.Controls.Add(label31);
			groupBox4.Controls.Add(label39);
			groupBox4.Controls.Add(label40);
			groupBox4.Controls.Add(label4);
			groupBox4.Controls.Add(label3);
			groupBox4.Controls.Add(label2);
			groupBox4.Controls.Add(label1);
			groupBox4.Controls.Add(pnlRfPaSwitchEnable);
			groupBox4.Location = new Point(12, 118);
			groupBox4.Name = "groupBox4";
			groupBox4.Size = new Size(295, 167);
			groupBox4.TabIndex = 4;
			groupBox4.TabStop = false;
			groupBox4.Text = "Antena switch control";
			groupBox4.Visible = false;
			pBoxRfOut4.Image = (Image)resources.GetObject("pBoxRfOut4.Image");
			pBoxRfOut4.Location = new Point(272, 105);
			pBoxRfOut4.Name = "pBoxRfOut4";
			pBoxRfOut4.Size = new Size(16, 16);
			pBoxRfOut4.TabIndex = 21;
			pBoxRfOut4.TabStop = false;
			pBoxRfOut4.Visible = false;
			pBoxRfOut3.Image = (Image)resources.GetObject("pBoxRfOut3.Image");
			pBoxRfOut3.Location = new Point(272, 86);
			pBoxRfOut3.Name = "pBoxRfOut3";
			pBoxRfOut3.Size = new Size(16, 16);
			pBoxRfOut3.TabIndex = 22;
			pBoxRfOut3.TabStop = false;
			pBoxRfOut3.Visible = false;
			pBoxRfOut2.Image = (Image)resources.GetObject("pBoxRfOut2.Image");
			pBoxRfOut2.Location = new Point(272, 67);
			pBoxRfOut2.Name = "pBoxRfOut2";
			pBoxRfOut2.Size = new Size(16, 16);
			pBoxRfOut2.TabIndex = 24;
			pBoxRfOut2.TabStop = false;
			pBoxRfOut1.Image = (Image)resources.GetObject("pBoxRfOut1.Image");
			pBoxRfOut1.Location = new Point(272, 48);
			pBoxRfOut1.Name = "pBoxRfOut1";
			pBoxRfOut1.Size = new Size(16, 16);
			pBoxRfOut1.TabIndex = 23;
			pBoxRfOut1.TabStop = false;
			pBoxRfOut1.Visible = false;
			pnlRfPaSwitchSel.AutoSize = true;
			pnlRfPaSwitchSel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlRfPaSwitchSel.Controls.Add(rBtnRfPaSwitchPaIo);
			pnlRfPaSwitchSel.Controls.Add(rBtnRfPaSwitchIoPa);
			pnlRfPaSwitchSel.Enabled = false;
			pnlRfPaSwitchSel.Location = new Point(70, 47);
			pnlRfPaSwitchSel.Name = "pnlRfPaSwitchSel";
			pnlRfPaSwitchSel.Size = new Size(20, 72);
			pnlRfPaSwitchSel.TabIndex = 4;
			rBtnRfPaSwitchPaIo.AutoSize = true;
			rBtnRfPaSwitchPaIo.Checked = true;
			rBtnRfPaSwitchPaIo.Location = new Point(3, 3);
			rBtnRfPaSwitchPaIo.MinimumSize = new Size(0, 30);
			rBtnRfPaSwitchPaIo.Name = "rBtnRfPaSwitchPaIo";
			rBtnRfPaSwitchPaIo.Size = new Size(14, 30);
			rBtnRfPaSwitchPaIo.TabIndex = 0;
			rBtnRfPaSwitchPaIo.TabStop = true;
			rBtnRfPaSwitchPaIo.UseVisualStyleBackColor = true;
			rBtnRfPaSwitchPaIo.CheckedChanged += rBtnRfPaSwitchSel_CheckedChanged;
			rBtnRfPaSwitchIoPa.AutoSize = true;
			rBtnRfPaSwitchIoPa.Location = new Point(3, 39);
			rBtnRfPaSwitchIoPa.Margin = new Padding(3, 4, 3, 3);
			rBtnRfPaSwitchIoPa.MinimumSize = new Size(0, 30);
			rBtnRfPaSwitchIoPa.Name = "rBtnRfPaSwitchIoPa";
			rBtnRfPaSwitchIoPa.Size = new Size(14, 30);
			rBtnRfPaSwitchIoPa.TabIndex = 0;
			rBtnRfPaSwitchIoPa.UseVisualStyleBackColor = true;
			rBtnRfPaSwitchIoPa.CheckedChanged += rBtnRfPaSwitchSel_CheckedChanged;
			label44.AutoSize = true;
			label44.Location = new Point(96, 107);
			label44.Margin = new Padding(3);
			label44.Name = "label44";
			label44.Size = new Size(22, 13);
			label44.TabIndex = 16;
			label44.Text = "Pin";
			label5.AutoSize = true;
			label5.Location = new Point(96, 50);
			label5.Margin = new Padding(3);
			label5.Name = "label5";
			label5.Size = new Size(22, 13);
			label5.TabIndex = 15;
			label5.Text = "Pin";
			label34.AutoSize = true;
			label34.Location = new Point(194, 69);
			label34.Margin = new Padding(3);
			label34.Name = "label34";
			label34.Size = new Size(25, 13);
			label34.TabIndex = 20;
			label34.Text = "<=>";
			label32.AutoSize = true;
			label32.Location = new Point(124, 69);
			label32.Margin = new Padding(3);
			label32.Name = "label32";
			label32.Size = new Size(32, 13);
			label32.TabIndex = 19;
			label32.Text = "RFIO";
			label43.AutoSize = true;
			label43.Location = new Point(194, 107);
			label43.Margin = new Padding(3);
			label43.Name = "label43";
			label43.Size = new Size(25, 13);
			label43.TabIndex = 18;
			label43.Text = "<=>";
			label6.AutoSize = true;
			label6.Location = new Point(96, 69);
			label6.Margin = new Padding(3);
			label6.Name = "label6";
			label6.Size = new Size(22, 13);
			label6.TabIndex = 17;
			label6.Text = "Pin";
			label35.AutoSize = true;
			label35.Location = new Point(225, 69);
			label35.Margin = new Padding(3);
			label35.Name = "label35";
			label35.Size = new Size(38, 13);
			label35.TabIndex = 7;
			label35.Text = "RF_IO";
			label37.AutoSize = true;
			label37.Location = new Point(96, 88);
			label37.Margin = new Padding(3);
			label37.Name = "label37";
			label37.Size = new Size(22, 13);
			label37.TabIndex = 8;
			label37.Text = "Pin";
			label42.AutoSize = true;
			label42.Location = new Point(225, 107);
			label42.Margin = new Padding(3);
			label42.Name = "label42";
			label42.Size = new Size(41, 13);
			label42.TabIndex = 5;
			label42.Text = "RF_PA";
			label33.AutoSize = true;
			label33.Location = new Point(194, 50);
			label33.Margin = new Padding(3);
			label33.Name = "label33";
			label33.Size = new Size(25, 13);
			label33.TabIndex = 6;
			label33.Text = "<=>";
			label38.AutoSize = true;
			label38.Location = new Point(124, 88);
			label38.Margin = new Padding(3);
			label38.Name = "label38";
			label38.Size = new Size(64, 13);
			label38.TabIndex = 9;
			label38.Text = "PA_BOOST";
			label36.AutoSize = true;
			label36.Location = new Point(225, 50);
			label36.Margin = new Padding(3);
			label36.Name = "label36";
			label36.Size = new Size(41, 13);
			label36.TabIndex = 13;
			label36.Text = "RF_PA";
			label41.AutoSize = true;
			label41.Location = new Point(225, 88);
			label41.Margin = new Padding(3);
			label41.Name = "label41";
			label41.Size = new Size(38, 13);
			label41.TabIndex = 10;
			label41.Text = "RF_IO";
			label31.AutoSize = true;
			label31.Location = new Point(124, 50);
			label31.Margin = new Padding(3);
			label31.Name = "label31";
			label31.Size = new Size(64, 13);
			label31.TabIndex = 11;
			label31.Text = "PA_BOOST";
			label39.AutoSize = true;
			label39.Location = new Point(194, 88);
			label39.Margin = new Padding(3);
			label39.Name = "label39";
			label39.Size = new Size(25, 13);
			label39.TabIndex = 12;
			label39.Text = "<=>";
			label40.AutoSize = true;
			label40.Location = new Point(124, 107);
			label40.Margin = new Padding(3);
			label40.Name = "label40";
			label40.Size = new Size(32, 13);
			label40.TabIndex = 14;
			label40.Text = "RFIO";
			label4.Location = new Point(45, 130);
			label4.Name = "label4";
			label4.Size = new Size(192, 28);
			label4.TabIndex = 3;
			label4.Text = "To be used only on antenna diversity ref design.";
			label3.AutoSize = true;
			label3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label3.Location = new Point(6, 130);
			label3.Name = "label3";
			label3.Size = new Size(38, 13);
			label3.TabIndex = 3;
			label3.Text = "Note:";
			label2.AutoSize = true;
			label2.Location = new Point(6, 77);
			label2.Name = "label2";
			label2.Size = new Size(54, 13);
			label2.TabIndex = 2;
			label2.Text = "Selection:";
			label1.AutoSize = true;
			label1.Location = new Point(6, 24);
			label1.Name = "label1";
			label1.Size = new Size(42, 13);
			label1.TabIndex = 2;
			label1.Text = "Switch:";
			pnlRfPaSwitchEnable.AutoSize = true;
			pnlRfPaSwitchEnable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlRfPaSwitchEnable.Controls.Add(rBtnRfPaSwitchAuto);
			pnlRfPaSwitchEnable.Controls.Add(rBtnRfPaSwitchManual);
			pnlRfPaSwitchEnable.Controls.Add(rBtnRfPaSwitchOff);
			pnlRfPaSwitchEnable.Location = new Point(70, 19);
			pnlRfPaSwitchEnable.Name = "pnlRfPaSwitchEnable";
			pnlRfPaSwitchEnable.Size = new Size(170, 23);
			pnlRfPaSwitchEnable.TabIndex = 1;
			rBtnRfPaSwitchAuto.AutoSize = true;
			rBtnRfPaSwitchAuto.Location = new Point(3, 3);
			rBtnRfPaSwitchAuto.Name = "rBtnRfPaSwitchAuto";
			rBtnRfPaSwitchAuto.Size = new Size(47, 17);
			rBtnRfPaSwitchAuto.TabIndex = 0;
			rBtnRfPaSwitchAuto.Text = "Auto";
			rBtnRfPaSwitchAuto.UseVisualStyleBackColor = true;
			rBtnRfPaSwitchAuto.CheckedChanged += rBtnRfPaSwitchEnable_CheckedChanged;
			rBtnRfPaSwitchManual.AutoSize = true;
			rBtnRfPaSwitchManual.Location = new Point(56, 2);
			rBtnRfPaSwitchManual.Name = "rBtnRfPaSwitchManual";
			rBtnRfPaSwitchManual.Size = new Size(60, 17);
			rBtnRfPaSwitchManual.TabIndex = 0;
			rBtnRfPaSwitchManual.Text = "Manual";
			rBtnRfPaSwitchManual.UseVisualStyleBackColor = true;
			rBtnRfPaSwitchManual.CheckedChanged += rBtnRfPaSwitchEnable_CheckedChanged;
			rBtnRfPaSwitchOff.AutoSize = true;
			rBtnRfPaSwitchOff.Checked = true;
			rBtnRfPaSwitchOff.Location = new Point(122, 3);
			rBtnRfPaSwitchOff.Name = "rBtnRfPaSwitchOff";
			rBtnRfPaSwitchOff.Size = new Size(45, 17);
			rBtnRfPaSwitchOff.TabIndex = 0;
			rBtnRfPaSwitchOff.TabStop = true;
			rBtnRfPaSwitchOff.Text = "OFF";
			rBtnRfPaSwitchOff.UseVisualStyleBackColor = true;
			rBtnRfPaSwitchOff.CheckedChanged += rBtnRfPaSwitchEnable_CheckedChanged;
			groupBox3.Controls.Add(btnRead);
			groupBox3.Controls.Add(tlRegisters);
			groupBox3.Controls.Add(btnWrite);
			groupBox3.Location = new Point(12, 12);
			groupBox3.Name = "groupBox3";
			groupBox3.Size = new Size(295, 100);
			groupBox3.TabIndex = 0;
			groupBox3.TabStop = false;
			groupBox3.Text = "Registers";
			btnRead.Location = new Point(151, 68);
			btnRead.Name = "btnRead";
			btnRead.Size = new Size(65, 23);
			btnRead.TabIndex = 2;
			btnRead.Text = "Read";
			btnRead.UseVisualStyleBackColor = true;
			btnRead.Click += btnRead_Click;
			tlRegisters.AutoSize = true;
			tlRegisters.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			tlRegisters.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			tlRegisters.ColumnCount = 2;
			tlRegisters.ColumnStyles.Add(new ColumnStyle());
			tlRegisters.ColumnStyles.Add(new ColumnStyle());
			tlRegisters.Controls.Add(lblAddress, 0, 0);
			tlRegisters.Controls.Add(lblDataWrite, 1, 0);
			tlRegisters.Controls.Add(tBoxRegAddress, 0, 1);
			tlRegisters.Controls.Add(tBoxRegValue, 1, 1);
			tlRegisters.Location = new Point(75, 19);
			tlRegisters.Name = "tlRegisters";
			tlRegisters.RowCount = 2;
			tlRegisters.RowStyles.Add(new RowStyle());
			tlRegisters.RowStyles.Add(new RowStyle());
			tlRegisters.Size = new Size(145, 43);
			tlRegisters.TabIndex = 0;
			lblAddress.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			lblAddress.Location = new Point(4, 1);
			lblAddress.Name = "lblAddress";
			lblAddress.Size = new Size(65, 20);
			lblAddress.TabIndex = 0;
			lblAddress.Text = "Address";
			lblAddress.TextAlign = ContentAlignment.MiddleCenter;
			lblDataWrite.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			lblDataWrite.Location = new Point(76, 1);
			lblDataWrite.Name = "lblDataWrite";
			lblDataWrite.Size = new Size(65, 20);
			lblDataWrite.TabIndex = 1;
			lblDataWrite.Text = "Data";
			lblDataWrite.TextAlign = ContentAlignment.MiddleCenter;
			tBoxRegAddress.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			tBoxRegAddress.Location = new Point(1, 22);
			tBoxRegAddress.Margin = new Padding(0);
			tBoxRegAddress.MaxLength = 4;
			tBoxRegAddress.Name = "tBoxRegAddress";
			tBoxRegAddress.Size = new Size(71, 20);
			tBoxRegAddress.TabIndex = 2;
			tBoxRegAddress.Text = "0x00";
			tBoxRegAddress.TextAlign = HorizontalAlignment.Center;
			tBoxRegAddress.TextChanged += tBox_TextChanged;
			tBoxRegAddress.Enter += txtBox_Enter;
			tBoxRegAddress.Validating += txtBox_Validating;
			tBoxRegAddress.Validated += txtBox_Validated;
			tBoxRegValue.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			tBoxRegValue.Location = new Point(73, 22);
			tBoxRegValue.Margin = new Padding(0);
			tBoxRegValue.MaxLength = 4;
			tBoxRegValue.Name = "tBoxRegValue";
			tBoxRegValue.Size = new Size(71, 20);
			tBoxRegValue.TabIndex = 3;
			tBoxRegValue.Text = "0x00";
			tBoxRegValue.TextAlign = HorizontalAlignment.Center;
			tBoxRegValue.TextChanged += tBox_TextChanged;
			tBoxRegValue.Enter += txtBox_Enter;
			tBoxRegValue.Validating += txtBox_Validating;
			tBoxRegValue.Validated += txtBox_Validated;
			btnWrite.Location = new Point(79, 68);
			btnWrite.Name = "btnWrite";
			btnWrite.Size = new Size(65, 23);
			btnWrite.TabIndex = 1;
			btnWrite.Text = "Write";
			btnWrite.UseVisualStyleBackColor = true;
			btnWrite.Click += btnWrite_Click;
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(319, 320);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(ssStatus);
			DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MaximizeBox = false;
			MaximumSize = new Size(1200, 1200);
            Name = "TestForm";
            StartPosition = FormStartPosition.Manual;
			Text = "Test";
            Activated += TestForm_Activated;
            FormClosing += TestForm_FormClosing;
            Load += TestForm_Load;
            KeyDown += TestForm_KeyDown;
			ssStatus.ResumeLayout(false);
			ssStatus.PerformLayout();
			groupBox4.ResumeLayout(false);
			groupBox4.PerformLayout();
			((ISupportInitialize)pBoxRfOut4).EndInit();
			((ISupportInitialize)pBoxRfOut3).EndInit();
			((ISupportInitialize)pBoxRfOut2).EndInit();
			((ISupportInitialize)pBoxRfOut1).EndInit();
			pnlRfPaSwitchSel.ResumeLayout(false);
			pnlRfPaSwitchSel.PerformLayout();
			pnlRfPaSwitchEnable.ResumeLayout(false);
			pnlRfPaSwitchEnable.PerformLayout();
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
			tlRegisters.ResumeLayout(false);
			tlRegisters.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
		}
	}
}
