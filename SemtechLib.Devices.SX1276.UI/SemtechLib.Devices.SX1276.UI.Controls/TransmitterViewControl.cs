using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SemtechLib.Controls;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.Devices.SX1276.Events;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;

namespace SemtechLib.Devices.SX1276.UI.Controls
{
	public sealed class TransmitterViewControl : UserControl, INotifyDocumentationChanged
	{
		private decimal maxOutputPower = 13.2m;

		private decimal outputPower = 13.2m;

		private decimal ocpTrim = 100m;

		private decimal pllBandwidth;

		private IContainer components;

		private NumericUpDownEx nudOutputPower;

		private NumericUpDownEx nudOcpTrim;

		private ComboBox cBoxPaRamp;

		private Panel panel4;

		private RadioButton rBtnOcpOff;

		private RadioButton rBtnOcpOn;

		private Panel pnlPaSelect;

		private RadioButton rBtnRfPa;

		private RadioButton rBtnRfo;

		private Label suffixOutputPower;

		private Label suffixPAramp;

		private Label suffixOCPtrim;

		private Label label3;

		private Label label5;

		private ErrorProvider errorProvider;

		private GroupBoxEx gBoxPowerAmplifier;

		private GroupBoxEx gBoxOverloadCurrentProtection;

		private GroupBoxEx gBoxOutputPower;

		private GroupBoxEx groupBoxEx1;

		private NumericUpDownEx nudPllBandwidth;

		private Label label4;

		private Label label2;

		private NumericUpDownEx nudMaxOutputPower;

		private Label label7;

		private Label label6;

		private Label label1;

		private Panel pnlPa20dBm;

		private RadioButton rBtnPa20dBmOff;

		private RadioButton rBtnPa20dBmOn;

		private Label lblPa20dBm;

		public PaSelectEnum PaSelect
		{
			get
			{
				if (rBtnRfo.Checked)
				{
					return PaSelectEnum.RFO;
				}
				return rBtnRfPa.Checked ? PaSelectEnum.PA_BOOST : PaSelectEnum.RFO;
			}
			set
			{
				rBtnRfo.CheckedChanged -= rBtnPaControl_CheckedChanged;
				rBtnRfPa.CheckedChanged -= rBtnPaControl_CheckedChanged;
				switch (value)
				{
				case PaSelectEnum.RFO:
					rBtnRfo.Checked = true;
					rBtnRfPa.Checked = false;
					nudMaxOutputPower.Enabled = true;
					break;
				case PaSelectEnum.PA_BOOST:
					rBtnRfo.Checked = false;
					rBtnRfPa.Checked = true;
					nudMaxOutputPower.Enabled = false;
					break;
				}
				rBtnRfo.CheckedChanged += rBtnPaControl_CheckedChanged;
				rBtnRfPa.CheckedChanged += rBtnPaControl_CheckedChanged;
			}
		}

		public decimal MaxOutputPower
		{
			get => maxOutputPower;
			set
			{
				try
				{
					nudMaxOutputPower.ValueChanged -= nudMaxOutputPower_ValueChanged;
					nudMaxOutputPower.BackColor = SystemColors.Window;
					if (PaSelect == PaSelectEnum.RFO)
					{
						nudMaxOutputPower.Maximum = 15.0m;
						nudMaxOutputPower.Minimum = 10.8m;
						var num = (ushort)((uint)(int)((value - 10.8m) / 0.6m) & 7u);
						maxOutputPower = 10.8m + 0.6m * num;
						nudMaxOutputPower.Value = maxOutputPower;
					}
					else if (!Pa20dBm)
					{
						maxOutputPower = 17.0m;
						nudMaxOutputPower.Maximum = maxOutputPower;
						nudMaxOutputPower.Minimum = maxOutputPower;
						nudMaxOutputPower.Value = maxOutputPower;
					}
					else
					{
						maxOutputPower = 20.0m;
						nudMaxOutputPower.Maximum = maxOutputPower;
						nudMaxOutputPower.Minimum = maxOutputPower;
						nudMaxOutputPower.Value = maxOutputPower;
					}
				}
				catch (Exception)
				{
					nudMaxOutputPower.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudMaxOutputPower.ValueChanged += nudMaxOutputPower_ValueChanged;
				}
			}
		}

		public decimal OutputPower
		{
			get => outputPower; // nudOutputPower.Value;
			set
			{
				try
				{
					nudOutputPower.ValueChanged -= nudOutputPower_ValueChanged;
					nudOutputPower.BackColor = SystemColors.Window;
					nudOutputPower.Maximum = MaxOutputPower;
					nudOutputPower.Minimum = MaxOutputPower - 15.0m;
					if (PaSelect == PaSelectEnum.RFO)
					{
						var num = (ushort)((uint)(int)(value - MaxOutputPower + 15.0m) & 0xFu);
						outputPower = MaxOutputPower - (15.0m - num);
						nudOutputPower.Value = outputPower;
					}
					else if (!Pa20dBm)
					{
						var num2 = (ushort)((uint)(int)(value - 17m + 15.0m) & 0xFu);
						outputPower = 17m - (15.0m - num2);
						nudOutputPower.Value = outputPower;
					}
					else
					{
						var num3 = (ushort)((uint)(int)(value - 20m + 15.0m) & 0xFu);
						outputPower = 20m - (15.0m - num3);
						nudOutputPower.Value = outputPower;
					}
				}
				catch (Exception)
				{
					nudOutputPower.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudOutputPower.ValueChanged += nudOutputPower_ValueChanged;
				}
			}
		}

		public PaRampEnum PaRamp
		{
			get => (PaRampEnum)cBoxPaRamp.SelectedIndex;
			set
			{
				cBoxPaRamp.SelectedIndexChanged -= cBoxPaRamp_SelectedIndexChanged;
				cBoxPaRamp.SelectedIndex = (int)value;
				cBoxPaRamp.SelectedIndexChanged += cBoxPaRamp_SelectedIndexChanged;
			}
		}

		public bool OcpOn
		{
			get => rBtnOcpOn.Checked;
			set
			{
				rBtnOcpOn.CheckedChanged -= rBtnOcpOn_CheckedChanged;
				rBtnOcpOff.CheckedChanged -= rBtnOcpOn_CheckedChanged;
				if (value)
				{
					rBtnOcpOn.Checked = true;
					rBtnOcpOff.Checked = false;
				}
				else
				{
					rBtnOcpOn.Checked = false;
					rBtnOcpOff.Checked = true;
				}
				rBtnOcpOn.CheckedChanged += rBtnOcpOn_CheckedChanged;
				rBtnOcpOff.CheckedChanged += rBtnOcpOn_CheckedChanged;
			}
		}

		public decimal OcpTrim
		{
			get => ocpTrim;
			set
			{
				try
				{
					nudOcpTrim.ValueChanged -= nudOcpTrim_ValueChanged;
					switch (value)
					{
						case <= 120.0m:
						{
							var num = (ushort)Math.Round((value - 45.0m) / 5.0m, MidpointRounding.AwayFromZero);
							ocpTrim = 45.0m + 5.0m * num;
							break;
						}
						case > 120m and <= 240.0m:
						{
							var num = (ushort)Math.Round((value + 30.0m) / 10.0m, MidpointRounding.AwayFromZero);
							ocpTrim = -30.0m + 10.0m * num;
							break;
						}
						default:
						{
							ushort num = 27;
							ocpTrim = -30.0m + 10.0m * num;
							break;
						}
					}
					nudOcpTrim.Value = ocpTrim;
					nudOcpTrim.ValueChanged += nudOcpTrim_ValueChanged;
				}
				catch (Exception)
				{
					nudOcpTrim.BackColor = ControlPaint.LightLight(Color.Red);
					nudOcpTrim.ValueChanged += nudOcpTrim_ValueChanged;
				}
			}
		}

		public bool Pa20dBm
		{
			get => rBtnPa20dBmOn.Checked;
			set
			{
				rBtnPa20dBmOn.CheckedChanged -= rBtnPa20dBm_CheckedChanged;
				rBtnPa20dBmOff.CheckedChanged -= rBtnPa20dBm_CheckedChanged;
				if (value)
				{
					rBtnPa20dBmOn.Checked = true;
					rBtnPa20dBmOff.Checked = false;
					pnlPaSelect.Enabled = false;
				}
				else
				{
					rBtnPa20dBmOn.Checked = false;
					rBtnPa20dBmOff.Checked = true;
					pnlPaSelect.Enabled = true;
				}
				rBtnPa20dBmOn.CheckedChanged += rBtnPa20dBm_CheckedChanged;
				rBtnPa20dBmOff.CheckedChanged += rBtnPa20dBm_CheckedChanged;
			}
		}

		public decimal PllBandwidth
		{
			get => pllBandwidth;
			set
			{
				try
				{
					nudPllBandwidth.ValueChanged -= nudPllBandwidth_ValueChanged;
					var num = (ushort)(value / 75000m - 1m);
					nudPllBandwidth.Value = (pllBandwidth = (num + 1) * 75000);
				}
				finally
				{
					nudPllBandwidth.ValueChanged += nudPllBandwidth_ValueChanged;
				}
			}
		}

		public event PaModeEventHandler PaModeChanged;

		public event DecimalEventHandler OutputPowerChanged;

		public event DecimalEventHandler MaxOutputPowerChanged;

		public event PaRampEventHandler PaRampChanged;

		public event BooleanEventHandler OcpOnChanged;

		public event DecimalEventHandler OcpTrimChanged;

		public event BooleanEventHandler Pa20dBmChanged;

		public event DecimalEventHandler PllBandwidthChanged;

		public event DocumentationChangedEventHandler DocumentationChanged;

		public TransmitterViewControl()
		{
			InitializeComponent();
		}

		private void OnPaModeChanged(PaSelectEnum value)
		{
            PaModeChanged?.Invoke(this, new PaModeEventArg(value));
        }

		private void OnMaxOutputPowerChanged(decimal value)
		{
            MaxOutputPowerChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnOutputPowerChanged(decimal value)
		{
            OutputPowerChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnPaRampChanged(PaRampEnum value)
		{
            PaRampChanged?.Invoke(this, new PaRampEventArg(value));
        }

		private void OnOcpOnChanged(bool value)
		{
            OcpOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnOcpTrimChanged(decimal value)
		{
            OcpTrimChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnPa20dBmChanged(bool value)
		{
            Pa20dBmChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnPllBandwidthChanged(decimal value)
		{
            PllBandwidthChanged?.Invoke(this, new DecimalEventArg(value));
        }

		public void UpdateOcpTrimLimits(LimitCheckStatusEnum status, string message)
		{
			switch (status)
			{
			case LimitCheckStatusEnum.OK:
				nudOcpTrim.BackColor = SystemColors.Window;
				break;
			case LimitCheckStatusEnum.OUT_OF_RANGE:
				nudOcpTrim.BackColor = ControlPaint.LightLight(Color.Orange);
				break;
			case LimitCheckStatusEnum.ERROR:
				nudOcpTrim.BackColor = ControlPaint.LightLight(Color.Red);
				break;
			}
			errorProvider.SetError(nudOcpTrim, message);
		}

		private void rBtnPaControl_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnRfo.Checked)
			{
				PaSelect = PaSelectEnum.RFO;
			}
			else if (rBtnRfPa.Checked)
			{
				PaSelect = PaSelectEnum.PA_BOOST;
			}
			OnPaModeChanged(PaSelect);
		}

		private void nudMaxOutputPower_ValueChanged(object sender, EventArgs e)
		{
			MaxOutputPower = nudMaxOutputPower.Value;
			OnMaxOutputPowerChanged(MaxOutputPower);
		}

		private void nudOutputPower_ValueChanged(object sender, EventArgs e)
		{
			OutputPower = nudOutputPower.Value;
			OnOutputPowerChanged(OutputPower);
		}

		private void rBtnPa20dBm_CheckedChanged(object sender, EventArgs e)
		{
			Pa20dBm = rBtnPa20dBmOn.Checked;
			OnPa20dBmChanged(Pa20dBm);
		}

		private void cBoxPaRamp_SelectedIndexChanged(object sender, EventArgs e)
		{
			PaRamp = (PaRampEnum)cBoxPaRamp.SelectedIndex;
			OnPaRampChanged(PaRamp);
		}

		private void rBtnOcpOn_CheckedChanged(object sender, EventArgs e)
		{
			OcpOn = rBtnOcpOn.Checked;
			OnOcpOnChanged(OcpOn);
		}

		private void nudOcpTrim_ValueChanged(object sender, EventArgs e)
		{
			int num;
			int num2;
			int num3;
			switch (nudOcpTrim.Value)
			{
				case <= 120.0m:
					num = (int)Math.Round((OcpTrim - 45.0m) / 5.0m, MidpointRounding.AwayFromZero);
					num2 = (int)Math.Round((nudOcpTrim.Value - 45.0m) / 5.0m, MidpointRounding.AwayFromZero);
					num3 = (int)(nudOcpTrim.Value - OcpTrim);
					break;
				case > 120m and <= 240.0m:
					num = (int)Math.Round((OcpTrim + 30.0m) / 10.0m, MidpointRounding.AwayFromZero);
					num2 = (int)Math.Round((nudOcpTrim.Value + 30.0m) / 10.0m, MidpointRounding.AwayFromZero);
					num3 = (int)(nudOcpTrim.Value - OcpTrim);
					break;
				default:
					num = (int)Math.Round(27m, MidpointRounding.AwayFromZero);
					num2 = (int)Math.Round((nudOcpTrim.Value + 30.0m) / 10.0m, MidpointRounding.AwayFromZero);
					num3 = (int)(nudOcpTrim.Value - 240.0m);
					break;
			}
			if (num3 is >= -1 and <= 1 && num == num2)
			{
				nudOcpTrim.ValueChanged -= nudOcpTrim_ValueChanged;
				nudOcpTrim.Value = nudOcpTrim.Value switch
				{
					<= 120.0m => 45.0m + 5.0m * (num2 + num3),
					> 120m and <= 240.0m => -30.0m + 10.0m * (num2 + num3),
					_ => 240.0m
				};
				nudOcpTrim.ValueChanged += nudOcpTrim_ValueChanged;
			}
			OcpTrim = nudOcpTrim.Value;
			OnOcpTrimChanged(OcpTrim);
		}

		private void nudPllBandwidth_ValueChanged(object sender, EventArgs e)
		{
			var num = (int)(PllBandwidth / 75000m - 1m);
			var num2 = (int)(nudPllBandwidth.Value / 75000m - 1m);
			var num3 = (int)(nudPllBandwidth.Value - PllBandwidth);
			nudPllBandwidth.ValueChanged -= nudPllBandwidth_ValueChanged;
			if (num == 0)
			{
				num3 = 0;
			}
			if (num3 is >= 0 and <= 1)
			{
				nudPllBandwidth.Value = (num2 + num3 + 1) * 75000;
			}
			else
			{
				nudPllBandwidth.Value = (num2 + 1) * 75000;
			}
			nudPllBandwidth.ValueChanged += nudPllBandwidth_ValueChanged;
			PllBandwidth = nudPllBandwidth.Value;
			OnPllBandwidthChanged(PllBandwidth);
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
			if (sender == gBoxPowerAmplifier)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Transmitter", "Power amplifier"));
			}
			else if (sender == gBoxOutputPower)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Transmitter", "Output power"));
			}
			else if (sender == gBoxOverloadCurrentProtection)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Transmitter", "Overload current protection"));
			}
		}

		private void control_MouseLeave(object sender, EventArgs e)
		{
			OnDocumentationChanged(new DocumentationChangedEventArgs(".", "Overview"));
		}

		private void OnDocumentationChanged(DocumentationChangedEventArgs e)
		{
            DocumentationChanged?.Invoke(this, e);
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
			nudOcpTrim = new NumericUpDownEx();
			gBoxOverloadCurrentProtection = new GroupBoxEx();
			panel4 = new Panel();
			rBtnOcpOff = new RadioButton();
			rBtnOcpOn = new RadioButton();
			label5 = new Label();
			suffixOCPtrim = new Label();
			gBoxOutputPower = new GroupBoxEx();
			pnlPa20dBm = new Panel();
			rBtnPa20dBmOff = new RadioButton();
			rBtnPa20dBmOn = new RadioButton();
			lblPa20dBm = new Label();
			nudMaxOutputPower = new NumericUpDownEx();
			label7 = new Label();
			nudOutputPower = new NumericUpDownEx();
			label6 = new Label();
			label1 = new Label();
			suffixOutputPower = new Label();
			gBoxPowerAmplifier = new GroupBoxEx();
			cBoxPaRamp = new ComboBox();
			pnlPaSelect = new Panel();
			rBtnRfPa = new RadioButton();
			rBtnRfo = new RadioButton();
			suffixPAramp = new Label();
			label3 = new Label();
			groupBoxEx1 = new GroupBoxEx();
			nudPllBandwidth = new NumericUpDownEx();
			label4 = new Label();
			label2 = new Label();
			((ISupportInitialize)errorProvider).BeginInit();
			((ISupportInitialize)nudOcpTrim).BeginInit();
			gBoxOverloadCurrentProtection.SuspendLayout();
			panel4.SuspendLayout();
			gBoxOutputPower.SuspendLayout();
			pnlPa20dBm.SuspendLayout();
			((ISupportInitialize)nudMaxOutputPower).BeginInit();
			((ISupportInitialize)nudOutputPower).BeginInit();
			gBoxPowerAmplifier.SuspendLayout();
			pnlPaSelect.SuspendLayout();
			groupBoxEx1.SuspendLayout();
			((ISupportInitialize)nudPllBandwidth).BeginInit();
			SuspendLayout();
			errorProvider.ContainerControl = this;
			errorProvider.SetIconPadding(nudOcpTrim, 30);
			nudOcpTrim.Location = new Point(192, 45);
			nudOcpTrim.Maximum = new decimal([240, 0, 0, 0]);
			nudOcpTrim.Minimum = new decimal([45, 0, 0, 0]);
			nudOcpTrim.Name = "nudOcpTrim";
			nudOcpTrim.Size = new Size(124, 20);
			nudOcpTrim.TabIndex = 2;
			nudOcpTrim.ThousandsSeparator = true;
			nudOcpTrim.Value = new decimal([100, 0, 0, 0]);
			nudOcpTrim.ValueChanged += nudOcpTrim_ValueChanged;
			gBoxOverloadCurrentProtection.Controls.Add(panel4);
			gBoxOverloadCurrentProtection.Controls.Add(label5);
			gBoxOverloadCurrentProtection.Controls.Add(nudOcpTrim);
			gBoxOverloadCurrentProtection.Controls.Add(suffixOCPtrim);
			gBoxOverloadCurrentProtection.Location = new Point(217, 301);
			gBoxOverloadCurrentProtection.Name = "gBoxOverloadCurrentProtection";
			gBoxOverloadCurrentProtection.Size = new Size(364, 69);
			gBoxOverloadCurrentProtection.TabIndex = 2;
			gBoxOverloadCurrentProtection.TabStop = false;
			gBoxOverloadCurrentProtection.Text = "Overload current protection";
			gBoxOverloadCurrentProtection.MouseEnter += control_MouseEnter;
			gBoxOverloadCurrentProtection.MouseLeave += control_MouseLeave;
			panel4.AutoSize = true;
			panel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel4.Controls.Add(rBtnOcpOff);
			panel4.Controls.Add(rBtnOcpOn);
			panel4.Location = new Point(192, 19);
			panel4.Name = "panel4";
			panel4.Size = new Size(102, 20);
			panel4.TabIndex = 0;
			rBtnOcpOff.AutoSize = true;
			rBtnOcpOff.Location = new Point(54, 3);
			rBtnOcpOff.Margin = new Padding(3, 0, 3, 0);
			rBtnOcpOff.Name = "rBtnOcpOff";
			rBtnOcpOff.Size = new Size(45, 17);
			rBtnOcpOff.TabIndex = 1;
			rBtnOcpOff.Text = "OFF";
			rBtnOcpOff.UseVisualStyleBackColor = true;
			rBtnOcpOff.CheckedChanged += rBtnOcpOn_CheckedChanged;
			rBtnOcpOn.AutoSize = true;
			rBtnOcpOn.Checked = true;
			rBtnOcpOn.Location = new Point(3, 3);
			rBtnOcpOn.Margin = new Padding(3, 0, 3, 0);
			rBtnOcpOn.Name = "rBtnOcpOn";
			rBtnOcpOn.Size = new Size(41, 17);
			rBtnOcpOn.TabIndex = 0;
			rBtnOcpOn.TabStop = true;
			rBtnOcpOn.Text = "ON";
			rBtnOcpOn.UseVisualStyleBackColor = true;
			rBtnOcpOn.CheckedChanged += rBtnOcpOn_CheckedChanged;
			label5.AutoSize = true;
			label5.Location = new Point(6, 49);
			label5.Name = "label5";
			label5.Size = new Size(52, 13);
			label5.TabIndex = 1;
			label5.Text = "Trimming:";
			suffixOCPtrim.AutoSize = true;
			suffixOCPtrim.Location = new Point(322, 49);
			suffixOCPtrim.Name = "suffixOCPtrim";
			suffixOCPtrim.Size = new Size(22, 13);
			suffixOCPtrim.TabIndex = 3;
			suffixOCPtrim.Text = "mA";
			gBoxOutputPower.Controls.Add(pnlPa20dBm);
			gBoxOutputPower.Controls.Add(lblPa20dBm);
			gBoxOutputPower.Controls.Add(nudMaxOutputPower);
			gBoxOutputPower.Controls.Add(label7);
			gBoxOutputPower.Controls.Add(nudOutputPower);
			gBoxOutputPower.Controls.Add(label6);
			gBoxOutputPower.Controls.Add(label1);
			gBoxOutputPower.Controls.Add(suffixOutputPower);
			gBoxOutputPower.Location = new Point(217, 194);
			gBoxOutputPower.Name = "gBoxOutputPower";
			gBoxOutputPower.Size = new Size(364, 101);
			gBoxOutputPower.TabIndex = 1;
			gBoxOutputPower.TabStop = false;
			gBoxOutputPower.Text = "Output power";
			gBoxOutputPower.MouseEnter += control_MouseEnter;
			gBoxOutputPower.MouseLeave += control_MouseLeave;
			pnlPa20dBm.AutoSize = true;
			pnlPa20dBm.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlPa20dBm.Controls.Add(rBtnPa20dBmOff);
			pnlPa20dBm.Controls.Add(rBtnPa20dBmOn);
			pnlPa20dBm.Location = new Point(192, 71);
			pnlPa20dBm.Name = "pnlPa20dBm";
			pnlPa20dBm.Size = new Size(102, 20);
			pnlPa20dBm.TabIndex = 4;
			rBtnPa20dBmOff.AutoSize = true;
			rBtnPa20dBmOff.Checked = true;
			rBtnPa20dBmOff.Location = new Point(54, 3);
			rBtnPa20dBmOff.Margin = new Padding(3, 0, 3, 0);
			rBtnPa20dBmOff.Name = "rBtnPa20dBmOff";
			rBtnPa20dBmOff.Size = new Size(45, 17);
			rBtnPa20dBmOff.TabIndex = 1;
			rBtnPa20dBmOff.TabStop = true;
			rBtnPa20dBmOff.Text = "OFF";
			rBtnPa20dBmOff.UseVisualStyleBackColor = true;
			rBtnPa20dBmOff.CheckedChanged += rBtnPa20dBm_CheckedChanged;
			rBtnPa20dBmOn.AutoSize = true;
			rBtnPa20dBmOn.Location = new Point(3, 3);
			rBtnPa20dBmOn.Margin = new Padding(3, 0, 3, 0);
			rBtnPa20dBmOn.Name = "rBtnPa20dBmOn";
			rBtnPa20dBmOn.Size = new Size(41, 17);
			rBtnPa20dBmOn.TabIndex = 0;
			rBtnPa20dBmOn.Text = "ON";
			rBtnPa20dBmOn.UseVisualStyleBackColor = true;
			rBtnPa20dBmOn.CheckedChanged += rBtnPa20dBm_CheckedChanged;
			lblPa20dBm.AutoSize = true;
			lblPa20dBm.Location = new Point(6, 75);
			lblPa20dBm.Name = "lblPa20dBm";
			lblPa20dBm.Size = new Size(144, 13);
			lblPa20dBm.TabIndex = 5;
			lblPa20dBm.Text = "+20 dBm on pin PA_BOOST:";
			nudMaxOutputPower.DecimalPlaces = 1;
			nudMaxOutputPower.Increment = new decimal([6, 0, 0, 65536]);
			nudMaxOutputPower.Location = new Point(192, 19);
			nudMaxOutputPower.Maximum = new decimal([15, 0, 0, 0]);
			nudMaxOutputPower.Minimum = new decimal([108, 0, 0, 65536]);
			nudMaxOutputPower.Name = "nudMaxOutputPower";
			nudMaxOutputPower.Size = new Size(124, 20);
			nudMaxOutputPower.TabIndex = 0;
			nudMaxOutputPower.ThousandsSeparator = true;
			nudMaxOutputPower.Value = new decimal([132, 0, 0, 65536]);
			nudMaxOutputPower.ValueChanged += nudMaxOutputPower_ValueChanged;
			label7.AutoSize = true;
			label7.Location = new Point(6, 23);
			label7.Name = "label7";
			label7.Size = new Size(119, 13);
			label7.TabIndex = 1;
			label7.Text = "Maximum output power:";
			nudOutputPower.DecimalPlaces = 1;
			nudOutputPower.Location = new Point(192, 45);
			nudOutputPower.Maximum = new decimal([132, 0, 0, 65536]);
			nudOutputPower.Minimum = new decimal([18, 0, 0, -2147418112]);
			nudOutputPower.Name = "nudOutputPower";
			nudOutputPower.Size = new Size(124, 20);
			nudOutputPower.TabIndex = 0;
			nudOutputPower.ThousandsSeparator = true;
			nudOutputPower.Value = new decimal([132, 0, 0, 65536]);
			nudOutputPower.ValueChanged += nudOutputPower_ValueChanged;
			label6.AutoSize = true;
			label6.Location = new Point(322, 23);
			label6.Name = "label6";
			label6.Size = new Size(28, 13);
			label6.TabIndex = 1;
			label6.Text = "dBm";
			label1.AutoSize = true;
			label1.Location = new Point(6, 49);
			label1.Name = "label1";
			label1.Size = new Size(74, 13);
			label1.TabIndex = 1;
			label1.Text = "Output power:";
			suffixOutputPower.AutoSize = true;
			suffixOutputPower.Location = new Point(322, 49);
			suffixOutputPower.Name = "suffixOutputPower";
			suffixOutputPower.Size = new Size(28, 13);
			suffixOutputPower.TabIndex = 1;
			suffixOutputPower.Text = "dBm";
			gBoxPowerAmplifier.Controls.Add(cBoxPaRamp);
			gBoxPowerAmplifier.Controls.Add(pnlPaSelect);
			gBoxPowerAmplifier.Controls.Add(suffixPAramp);
			gBoxPowerAmplifier.Controls.Add(label3);
			gBoxPowerAmplifier.Location = new Point(217, 69);
			gBoxPowerAmplifier.Name = "gBoxPowerAmplifier";
			gBoxPowerAmplifier.Size = new Size(364, 119);
			gBoxPowerAmplifier.TabIndex = 0;
			gBoxPowerAmplifier.TabStop = false;
			gBoxPowerAmplifier.Text = "Power Amplifier";
			gBoxPowerAmplifier.MouseEnter += control_MouseEnter;
			gBoxPowerAmplifier.MouseLeave += control_MouseLeave;
			cBoxPaRamp.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxPaRamp.Items.AddRange(
            [
                "3400", "2000", "1000", "500", "250", "125", "100", "62", "50", "40",
				"31", "25", "20", "15", "12", "10"
			]);
			cBoxPaRamp.Location = new Point(192, 94);
			cBoxPaRamp.Name = "cBoxPaRamp";
			cBoxPaRamp.Size = new Size(124, 21);
			cBoxPaRamp.TabIndex = 2;
			cBoxPaRamp.SelectedIndexChanged += cBoxPaRamp_SelectedIndexChanged;
			pnlPaSelect.AutoSize = true;
			pnlPaSelect.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlPaSelect.Controls.Add(rBtnRfPa);
			pnlPaSelect.Controls.Add(rBtnRfo);
			pnlPaSelect.Location = new Point(65, 19);
			pnlPaSelect.Name = "pnlPaSelect";
			pnlPaSelect.Size = new Size(203, 46);
			pnlPaSelect.TabIndex = 0;
			rBtnRfPa.AutoSize = true;
			rBtnRfPa.Location = new Point(3, 26);
			rBtnRfPa.Name = "rBtnRfPa";
			rBtnRfPa.Size = new Size(197, 17);
			rBtnRfPa.TabIndex = 1;
			rBtnRfPa.Text = "PA1 -> Transmits on pin PA_BOOST";
			rBtnRfPa.UseVisualStyleBackColor = true;
			rBtnRfPa.CheckedChanged += rBtnPaControl_CheckedChanged;
			rBtnRfo.AutoSize = true;
			rBtnRfo.Checked = true;
			rBtnRfo.Location = new Point(3, 3);
			rBtnRfo.Name = "rBtnRfo";
			rBtnRfo.Size = new Size(162, 17);
			rBtnRfo.TabIndex = 0;
			rBtnRfo.TabStop = true;
			rBtnRfo.Text = "PA0 -> Transmits on pin RFO";
			rBtnRfo.UseVisualStyleBackColor = true;
			rBtnRfo.CheckedChanged += rBtnPaControl_CheckedChanged;
			suffixPAramp.AutoSize = true;
			suffixPAramp.Location = new Point(322, 98);
			suffixPAramp.Name = "suffixPAramp";
			suffixPAramp.Size = new Size(18, 13);
			suffixPAramp.TabIndex = 3;
			suffixPAramp.Text = "µs";
			label3.AutoSize = true;
			label3.Location = new Point(6, 98);
			label3.Name = "label3";
			label3.Size = new Size(50, 13);
			label3.TabIndex = 1;
			label3.Text = "PA ramp:";
			groupBoxEx1.Controls.Add(nudPllBandwidth);
			groupBoxEx1.Controls.Add(label4);
			groupBoxEx1.Controls.Add(label2);
			groupBoxEx1.Location = new Point(217, 376);
			groupBoxEx1.Name = "groupBoxEx1";
			groupBoxEx1.Size = new Size(364, 48);
			groupBoxEx1.TabIndex = 3;
			groupBoxEx1.TabStop = false;
			groupBoxEx1.Text = "PLL bandwidth";
			nudPllBandwidth.Increment = new decimal([75000, 0, 0, 0]);
			nudPllBandwidth.Location = new Point(192, 19);
			nudPllBandwidth.Maximum = new decimal([300000, 0, 0, 0]);
			nudPllBandwidth.Minimum = new decimal([75000, 0, 0, 0]);
			nudPllBandwidth.Name = "nudPllBandwidth";
			nudPllBandwidth.Size = new Size(124, 20);
			nudPllBandwidth.TabIndex = 2;
			nudPllBandwidth.ThousandsSeparator = true;
			nudPllBandwidth.Value = new decimal([300000, 0, 0, 0]);
			nudPllBandwidth.ValueChanged += nudPllBandwidth_ValueChanged;
			label4.AutoSize = true;
			label4.Location = new Point(6, 23);
			label4.Name = "label4";
			label4.Size = new Size(29, 13);
			label4.TabIndex = 1;
			label4.Text = "PLL:";
			label2.AutoSize = true;
			label2.Location = new Point(322, 23);
			label2.Name = "label2";
			label2.Size = new Size(20, 13);
			label2.TabIndex = 3;
			label2.Text = "Hz";
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(groupBoxEx1);
			Controls.Add(gBoxOverloadCurrentProtection);
			Controls.Add(gBoxOutputPower);
			Controls.Add(gBoxPowerAmplifier);
			Name = "TransmitterViewControl";
			Size = new Size(799, 493);
			((ISupportInitialize)errorProvider).EndInit();
			((ISupportInitialize)nudOcpTrim).EndInit();
			gBoxOverloadCurrentProtection.ResumeLayout(false);
			gBoxOverloadCurrentProtection.PerformLayout();
			panel4.ResumeLayout(false);
			panel4.PerformLayout();
			gBoxOutputPower.ResumeLayout(false);
			gBoxOutputPower.PerformLayout();
			pnlPa20dBm.ResumeLayout(false);
			pnlPa20dBm.PerformLayout();
			((ISupportInitialize)nudMaxOutputPower).EndInit();
			((ISupportInitialize)nudOutputPower).EndInit();
			gBoxPowerAmplifier.ResumeLayout(false);
			gBoxPowerAmplifier.PerformLayout();
			pnlPaSelect.ResumeLayout(false);
			pnlPaSelect.PerformLayout();
			groupBoxEx1.ResumeLayout(false);
			groupBoxEx1.PerformLayout();
			((ISupportInitialize)nudPllBandwidth).EndInit();
			ResumeLayout(false);
		}
	}
}
