using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SemtechLib.Controls;
using SemtechLib.Devices.SX1276LR.Enumerations;
using SemtechLib.Devices.SX1276LR.Events;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;

namespace SemtechLib.Devices.SX1276LR.UI.Controls
{
	public sealed class CommonViewControl : UserControl, INotifyDocumentationChanged
	{
		private decimal maxOutputPower = 13.2m;

		private decimal outputPower = 13.2m;

		private decimal ocpTrim = 100m;

		private decimal pllBandwidth;

		private int agcReference = 19;

		private int agcThresh1;

		private int agcThresh2;

		private int agcThresh3;

		private int agcThresh4;

		private int agcThresh5;

		private IContainer components;

		private NumericUpDownEx nudFrequencyXo;

		private NumericUpDownEx nudFrequencyRf;

		private Label label1;

		private Label label9;

		private Label label14;

		private Label lblRcOscillatorCalStat;

		private Label label13;

		private Label label20;

		private Panel panel6;

		private Label label21;

		private Label label22;

		private Panel panel7;

		private Label label23;

		private Label label24;

		private Label label25;

		private Label label26;

		private Label label27;

		private Label label28;

		private ErrorProvider errorProvider;

		private Label label30;

		private Label lblListenResolRx;

		private GroupBoxEx gBoxGeneral;

		private Panel panel1;

		private RadioButton rBtnTcxoInputOff;

		private RadioButton rBtnTcxoInputOn;

		private Label label3;

		private Panel panel5;

		private RadioButton rBtnFastHopOff;

		private RadioButton rBtnFastHopOn;

		private GroupBoxEx gBoxRxSettings;

		private Panel panel3;

		private RadioButton rBtnLnaBoostOff;

		private RadioButton rBtnLnaBoostOn;

		private Label label2;

		private Label label4;

		private Label lblAgcReference;

		private Label label48;

		private Label label49;

		private Label label50;

		private Label label51;

		private Label label52;

		private Label lblLnaGain1;

		private Label label53;

		private Panel panel2;

		private RadioButton rBtnLnaGain1;

		private RadioButton rBtnLnaGain2;

		private RadioButton rBtnLnaGain3;

		private RadioButton rBtnLnaGain4;

		private RadioButton rBtnLnaGain5;

		private RadioButton rBtnLnaGain6;

		private Label lblLnaGain2;

		private Label lblLnaGain3;

		private Label lblLnaGain4;

		private Label lblLnaGain5;

		private Label lblLnaGain6;

		private Label lblAgcThresh1;

		private Label lblAgcThresh2;

		private Label lblAgcThresh3;

		private Label lblAgcThresh4;

		private Label lblAgcThresh5;

		private Label label47;

		private NumericUpDownEx nudPllBandwidth;

		private Label label5;

		private Label label8;

		private Panel panel4;

		private RadioButton rBtnOcpOff;

		private RadioButton rBtnOcpOn;

		private Label label10;

		private NumericUpDownEx nudOcpTrim;

		private Label suffixOCPtrim;

		private NumericUpDownEx nudOutputPower;

		private Label suffixOutputPower;

		private GroupBoxEx gBoxTxSettings;

		private ComboBox cBoxPaRamp;

		private Panel pnlPaSelect;

		private RadioButton rBtnRfPa;

		private RadioButton rBtnRfo;

		private Label suffixPAramp;

		private Label label12;

		private Label label17;

		private Panel panel9;

		private RadioButton rBtnAgcAutoOff;

		private RadioButton rBtnAgcAutoOn;

		private GroupBoxEx gBoxAgc;

		private Label label15;

		private Label label16;

		private Label label29;

		private Label label31;

		private Label label32;

		private Label label33;

		private Label label34;

		private Label label46;

		private Label label59;

		private Label label60;

		private Label label61;

		private Label label62;

		private NumericUpDown nudAgcStep5;

		private NumericUpDown nudAgcStep4;

		private NumericUpDownEx nudAgcReferenceLevel;

		private NumericUpDown nudAgcStep3;

		private NumericUpDown nudAgcStep1;

		private NumericUpDown nudAgcStep2;

		private Label label19;

		private Label label18;

		private GroupBox groupBox1;

		private TableLayoutPanel tableLayoutPanel1;

		private ComboBox cBoxDio4Mapping;

		private ComboBox cBoxDio3Mapping;

		private ComboBox cBoxDio2Mapping;

		private ComboBox cBoxDio1Mapping;

		private ComboBox cBoxDio0Mapping;

		private Label label54;

		private Label label55;

		private Label label56;

		private Label label57;

		private Label label58;

		private Label label35;

		private ComboBox cBoxDio5Mapping;

		private NumericUpDownEx nudMaxOutputPower;

		private Label label7;

		private Label label6;

		private ComboBox cBoxBand;

		private Label label11;

		private Label label36;

		private Panel panel8;

		private RadioButton rBtnLowFrequencyModeOff;

		private RadioButton rBtnLowFrequencyModeOn;

		private Label label37;

		private Panel panel10;

		private RadioButton rBtnForceRxBandLowFrequencyOff;

		private RadioButton rBtnForceRxBandLowFrequencyOn;

		private Label label38;

		private Panel panel11;

		private RadioButton rBtnForceTxBandLowFrequencyOff;

		private RadioButton rBtnForceTxBandLowFrequencyOn;

		private Label label39;

		private Panel panel12;

		private GroupBoxEx gBoxOptioanl;

		private Panel pnlPa20dBm;

		private RadioButton rBtnPa20dBmOff;

		private RadioButton rBtnPa20dBmOn;

		private Label lblPa20dBm;

		public decimal FrequencyXo
		{
			get => nudFrequencyXo.Value;
			set
			{
				nudFrequencyXo.ValueChanged -= nudFrequencyXo_ValueChanged;
				nudFrequencyXo.Value = value;
				nudFrequencyXo.ValueChanged += nudFrequencyXo_ValueChanged;
			}
		}

		public decimal FrequencyStep
		{
			get => nudFrequencyRf.Increment;
			set => nudFrequencyRf.Increment = value;
		}

		public BandEnum Band
		{
			get => (BandEnum)cBoxBand.SelectedIndex;
			set
			{
				cBoxBand.SelectedIndexChanged -= cBoxBand_SelectedIndexChanged;
				cBoxBand.SelectedIndex = (int)value;
				cBoxBand.SelectedIndexChanged += cBoxBand_SelectedIndexChanged;
			}
		}

		public bool ForceTxBandLowFrequencyOn
		{
			get => rBtnForceTxBandLowFrequencyOn.Checked;
			set
			{
				rBtnForceTxBandLowFrequencyOn.CheckedChanged -= rBtnForceTxBandLowFrequency_CheckedChanged;
				rBtnForceTxBandLowFrequencyOff.CheckedChanged -= rBtnForceTxBandLowFrequency_CheckedChanged;
				if (value)
				{
					rBtnForceTxBandLowFrequencyOn.Checked = true;
					rBtnForceTxBandLowFrequencyOff.Checked = false;
				}
				else
				{
					rBtnForceTxBandLowFrequencyOn.Checked = false;
					rBtnForceTxBandLowFrequencyOff.Checked = true;
				}
				rBtnForceTxBandLowFrequencyOn.CheckedChanged += rBtnForceTxBandLowFrequency_CheckedChanged;
				rBtnForceTxBandLowFrequencyOff.CheckedChanged += rBtnForceTxBandLowFrequency_CheckedChanged;
			}
		}

		public bool ForceRxBandLowFrequencyOn
		{
			get => rBtnForceRxBandLowFrequencyOn.Checked;
			set
			{
				rBtnForceRxBandLowFrequencyOn.CheckedChanged -= rBtnForceRxBandLowFrequency_CheckedChanged;
				rBtnForceRxBandLowFrequencyOff.CheckedChanged -= rBtnForceRxBandLowFrequency_CheckedChanged;
				if (value)
				{
					rBtnForceRxBandLowFrequencyOn.Checked = true;
					rBtnForceRxBandLowFrequencyOff.Checked = false;
				}
				else
				{
					rBtnForceRxBandLowFrequencyOn.Checked = false;
					rBtnForceRxBandLowFrequencyOff.Checked = true;
				}
				rBtnForceRxBandLowFrequencyOn.CheckedChanged += rBtnForceRxBandLowFrequency_CheckedChanged;
				rBtnForceRxBandLowFrequencyOff.CheckedChanged += rBtnForceRxBandLowFrequency_CheckedChanged;
			}
		}

		public bool LowFrequencyModeOn
		{
			get => rBtnLowFrequencyModeOn.Checked;
			set
			{
				rBtnLowFrequencyModeOn.CheckedChanged -= rBtnLowFrequencyMode_CheckedChanged;
				rBtnLowFrequencyModeOff.CheckedChanged -= rBtnLowFrequencyMode_CheckedChanged;
				if (value)
				{
					rBtnLowFrequencyModeOn.Checked = true;
					rBtnLowFrequencyModeOff.Checked = false;
					rBtnLnaBoostOn.Enabled = false;
					rBtnLnaBoostOff.Enabled = false;
				}
				else
				{
					rBtnLowFrequencyModeOn.Checked = false;
					rBtnLowFrequencyModeOff.Checked = true;
					rBtnLnaBoostOn.Enabled = true;
					rBtnLnaBoostOff.Enabled = true;
				}
				rBtnLowFrequencyModeOn.CheckedChanged += rBtnLowFrequencyMode_CheckedChanged;
				rBtnLowFrequencyModeOff.CheckedChanged += rBtnLowFrequencyMode_CheckedChanged;
			}
		}

		public decimal FrequencyRf
		{
			get => nudFrequencyRf.Value;
			set
			{
				try
				{
					nudFrequencyRf.ValueChanged -= nudFrequencyRf_ValueChanged;
					nudFrequencyRf.BackColor = SystemColors.Window;
					var num = (uint)Math.Round(value / FrequencyStep, MidpointRounding.AwayFromZero);
					nudFrequencyRf.Value = num * FrequencyStep;
				}
				catch (Exception)
				{
					nudFrequencyRf.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudFrequencyRf.ValueChanged += nudFrequencyRf_ValueChanged;
				}
			}
		}

		public bool FastHopOn
		{
			get => rBtnFastHopOn.Checked;
			set
			{
				rBtnFastHopOn.CheckedChanged -= rBtnFastHop_CheckedChanged;
				rBtnFastHopOff.CheckedChanged -= rBtnFastHop_CheckedChanged;
				if (value)
				{
					rBtnFastHopOn.Checked = true;
					rBtnFastHopOff.Checked = false;
				}
				else
				{
					rBtnFastHopOn.Checked = false;
					rBtnFastHopOff.Checked = true;
				}
				rBtnFastHopOn.CheckedChanged += rBtnFastHop_CheckedChanged;
				rBtnFastHopOff.CheckedChanged += rBtnFastHop_CheckedChanged;
			}
		}

		public bool TcxoInputOn
		{
			get => rBtnTcxoInputOn.Checked;
			set
			{
				rBtnTcxoInputOn.CheckedChanged -= rBtnTcxoInput_CheckedChanged;
				rBtnTcxoInputOff.CheckedChanged -= rBtnTcxoInput_CheckedChanged;
				rBtnTcxoInputOn.Checked = value;
				rBtnTcxoInputOff.Checked = !value;
				rBtnTcxoInputOn.CheckedChanged += rBtnTcxoInput_CheckedChanged;
				rBtnTcxoInputOff.CheckedChanged += rBtnTcxoInput_CheckedChanged;
			}
		}

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
				catch (Exception)
				{
				}
				finally
				{
					nudPllBandwidth.ValueChanged += nudPllBandwidth_ValueChanged;
				}
			}
		}

		public int AgcReference
		{
			get => agcReference;
			set
			{
				agcReference = value;
				lblAgcReference.Text = value.ToString();
			}
		}

		public int AgcThresh1
		{
			get => agcThresh1;
			set
			{
				agcThresh1 = value;
				lblAgcThresh1.Text = value.ToString();
			}
		}

		public int AgcThresh2
		{
			get => agcThresh2;
			set
			{
				agcThresh2 = value;
				lblAgcThresh2.Text = value.ToString();
			}
		}

		public int AgcThresh3
		{
			get => agcThresh3;
			set
			{
				agcThresh3 = value;
				lblAgcThresh3.Text = value.ToString();
			}
		}

		public int AgcThresh4
		{
			get => agcThresh4;
			set
			{
				agcThresh4 = value;
				lblAgcThresh4.Text = value.ToString();
			}
		}

		public int AgcThresh5
		{
			get => agcThresh5;
			set
			{
				agcThresh5 = value;
				lblAgcThresh5.Text = value.ToString();
			}
		}

		public int AgcReferenceLevel
		{
			get => (int)nudAgcReferenceLevel.Value;
			set
			{
				try
				{
					nudAgcReferenceLevel.ValueChanged -= nudAgcReferenceLevel_ValueChanged;
					nudAgcReferenceLevel.Value = value;
					nudAgcReferenceLevel.ValueChanged += nudAgcReferenceLevel_ValueChanged;
				}
				catch (Exception)
				{
					nudAgcReferenceLevel.ValueChanged += nudAgcReferenceLevel_ValueChanged;
				}
			}
		}

		public byte AgcStep1
		{
			get => (byte)nudAgcStep1.Value;
			set
			{
				try
				{
					nudAgcStep1.ValueChanged -= nudAgcStep_ValueChanged;
					nudAgcStep1.Value = value;
					nudAgcStep1.ValueChanged += nudAgcStep_ValueChanged;
				}
				catch (Exception)
				{
					nudAgcStep1.ValueChanged += nudAgcStep_ValueChanged;
				}
			}
		}

		public byte AgcStep2
		{
			get => (byte)nudAgcStep2.Value;
			set
			{
				try
				{
					nudAgcStep2.ValueChanged -= nudAgcStep_ValueChanged;
					nudAgcStep2.Value = value;
					nudAgcStep2.ValueChanged += nudAgcStep_ValueChanged;
				}
				catch (Exception)
				{
					nudAgcStep2.ValueChanged += nudAgcStep_ValueChanged;
				}
			}
		}

		public byte AgcStep3
		{
			get => (byte)nudAgcStep3.Value;
			set
			{
				try
				{
					nudAgcStep3.ValueChanged -= nudAgcStep_ValueChanged;
					nudAgcStep3.Value = value;
					nudAgcStep3.ValueChanged += nudAgcStep_ValueChanged;
				}
				catch (Exception)
				{
					nudAgcStep3.ValueChanged += nudAgcStep_ValueChanged;
				}
			}
		}

		public byte AgcStep4
		{
			get => (byte)nudAgcStep4.Value;
			set
			{
				try
				{
					nudAgcStep4.ValueChanged -= nudAgcStep_ValueChanged;
					nudAgcStep4.Value = value;
					nudAgcStep4.ValueChanged += nudAgcStep_ValueChanged;
				}
				catch (Exception)
				{
					nudAgcStep4.ValueChanged += nudAgcStep_ValueChanged;
				}
			}
		}

		public byte AgcStep5
		{
			get => (byte)nudAgcStep5.Value;
			set
			{
				try
				{
					nudAgcStep5.ValueChanged -= nudAgcStep_ValueChanged;
					nudAgcStep5.Value = value;
					nudAgcStep5.ValueChanged += nudAgcStep_ValueChanged;
				}
				catch (Exception)
				{
					nudAgcStep5.ValueChanged += nudAgcStep_ValueChanged;
				}
			}
		}

		public LnaGainEnum LnaGain
		{
			private get
			{
				if (rBtnLnaGain1.Checked)
				{
					return LnaGainEnum.G1;
				}
				if (rBtnLnaGain2.Checked)
				{
					return LnaGainEnum.G2;
				}
				if (rBtnLnaGain3.Checked)
				{
					return LnaGainEnum.G3;
				}
				if (rBtnLnaGain4.Checked)
				{
					return LnaGainEnum.G4;
				}
				if (rBtnLnaGain5.Checked)
				{
					return LnaGainEnum.G5;
				}
				return rBtnLnaGain6.Checked ? LnaGainEnum.G6 : LnaGainEnum.G1;
			}
			set
			{
				rBtnLnaGain1.CheckedChanged -= rBtnLnaGain_CheckedChanged;
				rBtnLnaGain2.CheckedChanged -= rBtnLnaGain_CheckedChanged;
				rBtnLnaGain3.CheckedChanged -= rBtnLnaGain_CheckedChanged;
				rBtnLnaGain4.CheckedChanged -= rBtnLnaGain_CheckedChanged;
				rBtnLnaGain5.CheckedChanged -= rBtnLnaGain_CheckedChanged;
				rBtnLnaGain6.CheckedChanged -= rBtnLnaGain_CheckedChanged;
				switch (value)
				{
				case LnaGainEnum.G1:
					rBtnLnaGain1.Checked = true;
					rBtnLnaGain2.Checked = false;
					rBtnLnaGain3.Checked = false;
					rBtnLnaGain4.Checked = false;
					rBtnLnaGain5.Checked = false;
					rBtnLnaGain6.Checked = false;
					lblLnaGain1.BackColor = Color.LightSteelBlue;
					lblLnaGain2.BackColor = Color.Transparent;
					lblLnaGain3.BackColor = Color.Transparent;
					lblLnaGain4.BackColor = Color.Transparent;
					lblLnaGain5.BackColor = Color.Transparent;
					lblLnaGain6.BackColor = Color.Transparent;
					break;
				case LnaGainEnum.G2:
					rBtnLnaGain1.Checked = false;
					rBtnLnaGain2.Checked = true;
					rBtnLnaGain3.Checked = false;
					rBtnLnaGain4.Checked = false;
					rBtnLnaGain5.Checked = false;
					rBtnLnaGain6.Checked = false;
					lblLnaGain1.BackColor = Color.Transparent;
					lblLnaGain2.BackColor = Color.LightSteelBlue;
					lblLnaGain3.BackColor = Color.Transparent;
					lblLnaGain4.BackColor = Color.Transparent;
					lblLnaGain5.BackColor = Color.Transparent;
					lblLnaGain6.BackColor = Color.Transparent;
					break;
				case LnaGainEnum.G3:
					rBtnLnaGain1.Checked = false;
					rBtnLnaGain2.Checked = false;
					rBtnLnaGain3.Checked = true;
					rBtnLnaGain4.Checked = false;
					rBtnLnaGain5.Checked = false;
					rBtnLnaGain6.Checked = false;
					lblLnaGain1.BackColor = Color.Transparent;
					lblLnaGain2.BackColor = Color.Transparent;
					lblLnaGain3.BackColor = Color.LightSteelBlue;
					lblLnaGain4.BackColor = Color.Transparent;
					lblLnaGain5.BackColor = Color.Transparent;
					lblLnaGain6.BackColor = Color.Transparent;
					break;
				case LnaGainEnum.G4:
					rBtnLnaGain1.Checked = false;
					rBtnLnaGain2.Checked = false;
					rBtnLnaGain3.Checked = false;
					rBtnLnaGain4.Checked = true;
					rBtnLnaGain5.Checked = false;
					rBtnLnaGain6.Checked = false;
					lblLnaGain1.BackColor = Color.Transparent;
					lblLnaGain2.BackColor = Color.Transparent;
					lblLnaGain3.BackColor = Color.Transparent;
					lblLnaGain4.BackColor = Color.LightSteelBlue;
					lblLnaGain5.BackColor = Color.Transparent;
					lblLnaGain6.BackColor = Color.Transparent;
					break;
				case LnaGainEnum.G5:
					rBtnLnaGain1.Checked = false;
					rBtnLnaGain2.Checked = false;
					rBtnLnaGain3.Checked = false;
					rBtnLnaGain4.Checked = false;
					rBtnLnaGain5.Checked = true;
					rBtnLnaGain6.Checked = false;
					lblLnaGain1.BackColor = Color.Transparent;
					lblLnaGain2.BackColor = Color.Transparent;
					lblLnaGain3.BackColor = Color.Transparent;
					lblLnaGain4.BackColor = Color.Transparent;
					lblLnaGain5.BackColor = Color.LightSteelBlue;
					lblLnaGain6.BackColor = Color.Transparent;
					break;
				case LnaGainEnum.G6:
					rBtnLnaGain1.Checked = false;
					rBtnLnaGain2.Checked = false;
					rBtnLnaGain3.Checked = false;
					rBtnLnaGain4.Checked = false;
					rBtnLnaGain5.Checked = false;
					rBtnLnaGain6.Checked = true;
					lblLnaGain1.BackColor = Color.Transparent;
					lblLnaGain2.BackColor = Color.Transparent;
					lblLnaGain3.BackColor = Color.Transparent;
					lblLnaGain4.BackColor = Color.Transparent;
					lblLnaGain5.BackColor = Color.Transparent;
					lblLnaGain6.BackColor = Color.LightSteelBlue;
					break;
				}
				rBtnLnaGain1.CheckedChanged += rBtnLnaGain_CheckedChanged;
				rBtnLnaGain2.CheckedChanged += rBtnLnaGain_CheckedChanged;
				rBtnLnaGain3.CheckedChanged += rBtnLnaGain_CheckedChanged;
				rBtnLnaGain4.CheckedChanged += rBtnLnaGain_CheckedChanged;
				rBtnLnaGain5.CheckedChanged += rBtnLnaGain_CheckedChanged;
				rBtnLnaGain6.CheckedChanged += rBtnLnaGain_CheckedChanged;
			}
		}

		public bool LnaBoost
		{
			get => rBtnLnaBoostOn.Checked;
			set
			{
				rBtnLnaBoostOn.CheckedChanged -= rBtnLnaBoost_CheckedChanged;
				rBtnLnaBoostOff.CheckedChanged -= rBtnLnaBoost_CheckedChanged;
				if (value)
				{
					rBtnLnaBoostOn.Checked = true;
					rBtnLnaBoostOff.Checked = false;
				}
				else
				{
					rBtnLnaBoostOn.Checked = false;
					rBtnLnaBoostOff.Checked = true;
				}
				rBtnLnaBoostOn.CheckedChanged += rBtnLnaBoost_CheckedChanged;
				rBtnLnaBoostOff.CheckedChanged += rBtnLnaBoost_CheckedChanged;
			}
		}

		public bool AgcAutoOn
		{
			get => rBtnAgcAutoOn.Checked;
			set
			{
				rBtnAgcAutoOn.CheckedChanged -= rBtnAgcAutoOn_CheckedChanged;
				rBtnAgcAutoOff.CheckedChanged -= rBtnAgcAutoOn_CheckedChanged;
				if (value)
				{
					rBtnAgcAutoOn.Checked = true;
					rBtnAgcAutoOff.Checked = false;
					rBtnLnaGain1.Enabled = false;
					rBtnLnaGain2.Enabled = false;
					rBtnLnaGain3.Enabled = false;
					rBtnLnaGain4.Enabled = false;
					rBtnLnaGain5.Enabled = false;
					rBtnLnaGain6.Enabled = false;
				}
				else
				{
					rBtnAgcAutoOn.Checked = false;
					rBtnAgcAutoOff.Checked = true;
					rBtnLnaGain1.Enabled = true;
					rBtnLnaGain2.Enabled = true;
					rBtnLnaGain3.Enabled = true;
					rBtnLnaGain4.Enabled = true;
					rBtnLnaGain5.Enabled = true;
					rBtnLnaGain6.Enabled = true;
				}
				rBtnAgcAutoOn.CheckedChanged += rBtnAgcAutoOn_CheckedChanged;
				rBtnAgcAutoOff.CheckedChanged += rBtnAgcAutoOn_CheckedChanged;
			}
		}

		public DioMappingEnum Dio0Mapping
		{
			get => (DioMappingEnum)cBoxDio0Mapping.SelectedIndex;
			set
			{
				try
				{
					cBoxDio0Mapping.SelectedIndexChanged -= cBoxDio0Mapping_SelectedIndexChanged;
					cBoxDio0Mapping.SelectedIndex = (int)value;
					cBoxDio0Mapping.SelectedIndexChanged += cBoxDio0Mapping_SelectedIndexChanged;
				}
				catch
				{
					cBoxDio0Mapping.SelectedIndexChanged += cBoxDio0Mapping_SelectedIndexChanged;
				}
			}
		}

		public DioMappingEnum Dio1Mapping
		{
			get => (DioMappingEnum)cBoxDio1Mapping.SelectedIndex;
			set
			{
				try
				{
					cBoxDio1Mapping.SelectedIndexChanged -= cBoxDio1Mapping_SelectedIndexChanged;
					cBoxDio1Mapping.SelectedIndex = (int)value;
					cBoxDio1Mapping.SelectedIndexChanged += cBoxDio1Mapping_SelectedIndexChanged;
				}
				catch
				{
					cBoxDio1Mapping.SelectedIndexChanged += cBoxDio1Mapping_SelectedIndexChanged;
				}
			}
		}

		public DioMappingEnum Dio2Mapping
		{
			get => (DioMappingEnum)cBoxDio2Mapping.SelectedIndex;
			set
			{
				try
				{
					cBoxDio2Mapping.SelectedIndexChanged -= cBoxDio2Mapping_SelectedIndexChanged;
					cBoxDio2Mapping.SelectedIndex = (int)value;
					cBoxDio2Mapping.SelectedIndexChanged += cBoxDio2Mapping_SelectedIndexChanged;
				}
				catch
				{
					cBoxDio2Mapping.SelectedIndexChanged += cBoxDio2Mapping_SelectedIndexChanged;
				}
			}
		}

		public DioMappingEnum Dio3Mapping
		{
			get => (DioMappingEnum)cBoxDio3Mapping.SelectedIndex;
			set
			{
				try
				{
					cBoxDio3Mapping.SelectedIndexChanged -= cBoxDio3Mapping_SelectedIndexChanged;
					cBoxDio3Mapping.SelectedIndex = (int)value;
					cBoxDio3Mapping.SelectedIndexChanged += cBoxDio3Mapping_SelectedIndexChanged;
				}
				catch
				{
					cBoxDio3Mapping.SelectedIndexChanged += cBoxDio3Mapping_SelectedIndexChanged;
				}
			}
		}

		public DioMappingEnum Dio4Mapping
		{
			get => (DioMappingEnum)cBoxDio4Mapping.SelectedIndex;
			set
			{
				try
				{
					cBoxDio4Mapping.SelectedIndexChanged -= cBoxDio4Mapping_SelectedIndexChanged;
					cBoxDio4Mapping.SelectedIndex = (int)value;
					cBoxDio4Mapping.SelectedIndexChanged += cBoxDio4Mapping_SelectedIndexChanged;
				}
				catch
				{
					cBoxDio4Mapping.SelectedIndexChanged += cBoxDio4Mapping_SelectedIndexChanged;
				}
			}
		}

		public DioMappingEnum Dio5Mapping
		{
			get => (DioMappingEnum)cBoxDio5Mapping.SelectedIndex;
			set
			{
				try
				{
					cBoxDio5Mapping.SelectedIndexChanged -= cBoxDio5Mapping_SelectedIndexChanged;
					cBoxDio5Mapping.SelectedIndex = (int)value;
					cBoxDio5Mapping.SelectedIndexChanged += cBoxDio5Mapping_SelectedIndexChanged;
				}
				catch
				{
					cBoxDio5Mapping.SelectedIndexChanged += cBoxDio5Mapping_SelectedIndexChanged;
				}
			}
		}

		public event DecimalEventHandler FrequencyXoChanged;

		public event BandEventHandler BandChanged;

		public event BooleanEventHandler ForceTxBandLowFrequencyOnChanged;

		public event BooleanEventHandler ForceRxBandLowFrequencyOnChanged;

		public event BooleanEventHandler LowFrequencyModeOnChanged;

		public event DecimalEventHandler FrequencyRfChanged;

		public event BooleanEventHandler FastHopOnChanged;

		public event PaModeEventHandler PaModeChanged;

		public event DecimalEventHandler OutputPowerChanged;

		public event DecimalEventHandler MaxOutputPowerChanged;

		public event PaRampEventHandler PaRampChanged;

		public event BooleanEventHandler OcpOnChanged;

		public event DecimalEventHandler OcpTrimChanged;

		public event BooleanEventHandler Pa20dBmChanged;

		public event DecimalEventHandler PllBandwidthChanged;

		public event Int32EventHandler AgcReferenceLevelChanged;

		public event AgcStepEventHandler AgcStepChanged;

		public event LnaGainEventHandler LnaGainChanged;

		public event BooleanEventHandler LnaBoostChanged;

		public event BooleanEventHandler AgcAutoOnChanged;

		public event BooleanEventHandler TcxoInputChanged;

		public event DioMappingEventHandler DioMappingChanged;

		public event DocumentationChangedEventHandler DocumentationChanged;

		public CommonViewControl()
		{
			InitializeComponent();
		}

		private void OnFrequencyXoChanged(decimal value)
		{
            FrequencyXoChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnBandChanged(BandEnum value)
		{
            BandChanged?.Invoke(this, new BandEventArg(value));
        }

		private void OnForceTxBandLowFrequencyOnChanged(bool value)
		{
            ForceTxBandLowFrequencyOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnForceRxBandLowFrequencyOnChanged(bool value)
		{
            ForceRxBandLowFrequencyOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnLowFrequencyModeOnChanged(bool value)
		{
            LowFrequencyModeOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnFrequencyRfChanged(decimal value)
		{
            FrequencyRfChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnFastHopOnChanged(bool value)
		{
            FastHopOnChanged?.Invoke(this, new BooleanEventArg(value));
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

		private void OnAgcReferenceLevelChanged(int value)
		{
            AgcReferenceLevelChanged?.Invoke(this, new Int32EventArg(value));
        }

		private void OnAgcStepChanged(byte id, byte value)
		{
            AgcStepChanged?.Invoke(this, new AgcStepEventArg(id, value));
        }

		private void OnLnaGainChanged(LnaGainEnum value)
		{
            LnaGainChanged?.Invoke(this, new LnaGainEventArg(value));
        }

		private void OnLnaBoostChanged(bool value)
		{
            LnaBoostChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnAgcAutoOnChanged(bool value)
		{
            AgcAutoOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnTcxoInputChanged(bool value)
		{
            TcxoInputChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnDioMappingChanged(byte id, DioMappingEnum value)
		{
            DioMappingChanged?.Invoke(this, new DioMappingEventArg(id, value));
        }

		public void UpdateFrequencyRfLimits(LimitCheckStatusEnum status, string message)
		{
			switch (status)
			{
			case LimitCheckStatusEnum.OK:
				nudFrequencyRf.BackColor = SystemColors.Window;
				break;
			case LimitCheckStatusEnum.OUT_OF_RANGE:
				nudFrequencyRf.BackColor = ControlPaint.LightLight(Color.Orange);
				break;
			case LimitCheckStatusEnum.ERROR:
				nudFrequencyRf.BackColor = ControlPaint.LightLight(Color.Red);
				break;
			}
			errorProvider.SetError(nudFrequencyRf, message);
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

		private void nudFrequencyXo_ValueChanged(object sender, EventArgs e)
		{
			FrequencyXo = nudFrequencyXo.Value;
			OnFrequencyXoChanged(FrequencyXo);
		}

		private void cBoxBand_SelectedIndexChanged(object sender, EventArgs e)
		{
			Band = (BandEnum)cBoxBand.SelectedIndex;
			OnBandChanged(Band);
		}

		private void rBtnForceTxBandLowFrequency_CheckedChanged(object sender, EventArgs e)
		{
			ForceTxBandLowFrequencyOn = rBtnForceTxBandLowFrequencyOn.Checked;
			OnForceTxBandLowFrequencyOnChanged(ForceTxBandLowFrequencyOn);
		}

		private void rBtnForceRxBandLowFrequency_CheckedChanged(object sender, EventArgs e)
		{
			ForceRxBandLowFrequencyOn = rBtnForceRxBandLowFrequencyOn.Checked;
			OnForceRxBandLowFrequencyOnChanged(ForceRxBandLowFrequencyOn);
		}

		private void rBtnLowFrequencyMode_CheckedChanged(object sender, EventArgs e)
		{
			LowFrequencyModeOn = rBtnLowFrequencyModeOn.Checked;
			OnLowFrequencyModeOnChanged(LowFrequencyModeOn);
		}

		private void nudFrequencyRf_ValueChanged(object sender, EventArgs e)
		{
			FrequencyRf = nudFrequencyRf.Value;
			OnFrequencyRfChanged(FrequencyRf);
		}

		private void rBtnFastHop_CheckedChanged(object sender, EventArgs e)
		{
			FastHopOn = rBtnFastHopOn.Checked;
			OnFastHopOnChanged(FastHopOn);
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
				switch (nudOcpTrim.Value)
				{
					case <= 120.0m:
						nudOcpTrim.Value = 45.0m + 5.0m * (num2 + num3);
						break;
					case > 120m and <= 240.0m:
						nudOcpTrim.Value = -30.0m + 10.0m * (num2 + num3);
						break;
					default:
						nudOcpTrim.Value = 240.0m;
						break;
				}
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

		private void nudAgcReferenceLevel_ValueChanged(object sender, EventArgs e)
		{
			AgcReferenceLevel = (int)nudAgcReferenceLevel.Value;
			OnAgcReferenceLevelChanged(AgcReferenceLevel);
		}

		private void nudAgcStep_ValueChanged(object sender, EventArgs e)
		{
		}

		private void rBtnLnaGain_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnLnaGain1.Checked)
			{
				LnaGain = LnaGainEnum.G1;
			}
			else if (rBtnLnaGain2.Checked)
			{
				LnaGain = LnaGainEnum.G2;
			}
			else if (rBtnLnaGain3.Checked)
			{
				LnaGain = LnaGainEnum.G3;
			}
			else if (rBtnLnaGain4.Checked)
			{
				LnaGain = LnaGainEnum.G4;
			}
			else if (rBtnLnaGain5.Checked)
			{
				LnaGain = LnaGainEnum.G5;
			}
			else if (rBtnLnaGain6.Checked)
			{
				LnaGain = LnaGainEnum.G6;
			}
			else
			{
				LnaGain = LnaGainEnum.G1;
			}
			OnLnaGainChanged(LnaGain);
		}

		private void rBtnLnaBoost_CheckedChanged(object sender, EventArgs e)
		{
			LnaBoost = rBtnLnaBoostOn.Checked;
			OnLnaBoostChanged(LnaBoost);
		}

		private void rBtnAgcAutoOn_CheckedChanged(object sender, EventArgs e)
		{
			AgcAutoOn = rBtnAgcAutoOn.Checked;
			OnAgcAutoOnChanged(AgcAutoOn);
		}

		private void rBtnTcxoInput_CheckedChanged(object sender, EventArgs e)
		{
			OnTcxoInputChanged(TcxoInputOn);
		}

		private void cBoxDio0Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged(0, (DioMappingEnum)cBoxDio0Mapping.SelectedIndex);
		}

		private void cBoxDio1Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged(1, (DioMappingEnum)cBoxDio1Mapping.SelectedIndex);
		}

		private void cBoxDio2Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged(2, (DioMappingEnum)cBoxDio2Mapping.SelectedIndex);
		}

		private void cBoxDio3Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged(3, (DioMappingEnum)cBoxDio3Mapping.SelectedIndex);
		}

		private void cBoxDio4Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged(4, (DioMappingEnum)cBoxDio4Mapping.SelectedIndex);
		}

		private void cBoxDio5Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged(5, (DioMappingEnum)cBoxDio5Mapping.SelectedIndex);
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
			if (sender == gBoxGeneral)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "General"));
			}
			if (sender == gBoxTxSettings)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Transmitter", "Tx Settings"));
			}
			else if (sender == gBoxRxSettings)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Receiver", "Rx Settings"));
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
			label20 = new Label();
			panel6 = new Panel();
			label21 = new Label();
			label22 = new Label();
			panel7 = new Panel();
			label23 = new Label();
			label24 = new Label();
			label25 = new Label();
			label26 = new Label();
			label27 = new Label();
			label28 = new Label();
			errorProvider = new ErrorProvider(components);
			lblListenResolRx = new Label();
			label30 = new Label();
			groupBox1 = new GroupBox();
			tableLayoutPanel1 = new TableLayoutPanel();
			cBoxDio4Mapping = new ComboBox();
			cBoxDio3Mapping = new ComboBox();
			cBoxDio2Mapping = new ComboBox();
			cBoxDio1Mapping = new ComboBox();
			cBoxDio0Mapping = new ComboBox();
			label54 = new Label();
			label55 = new Label();
			label56 = new Label();
			label57 = new Label();
			label58 = new Label();
			label35 = new Label();
			cBoxDio5Mapping = new ComboBox();
			gBoxRxSettings = new GroupBoxEx();
			panel12 = new Panel();
			panel9 = new Panel();
			rBtnAgcAutoOff = new RadioButton();
			rBtnAgcAutoOn = new RadioButton();
			label17 = new Label();
			label2 = new Label();
			panel3 = new Panel();
			rBtnLnaBoostOff = new RadioButton();
			rBtnLnaBoostOn = new RadioButton();
			label4 = new Label();
			lblAgcReference = new Label();
			label48 = new Label();
			label49 = new Label();
			label50 = new Label();
			label51 = new Label();
			label52 = new Label();
			lblLnaGain1 = new Label();
			label53 = new Label();
			panel2 = new Panel();
			rBtnLnaGain1 = new RadioButton();
			rBtnLnaGain2 = new RadioButton();
			rBtnLnaGain3 = new RadioButton();
			rBtnLnaGain4 = new RadioButton();
			rBtnLnaGain5 = new RadioButton();
			rBtnLnaGain6 = new RadioButton();
			lblLnaGain2 = new Label();
			lblLnaGain3 = new Label();
			lblLnaGain4 = new Label();
			lblLnaGain5 = new Label();
			lblLnaGain6 = new Label();
			lblAgcThresh1 = new Label();
			lblAgcThresh2 = new Label();
			lblAgcThresh3 = new Label();
			lblAgcThresh4 = new Label();
			lblAgcThresh5 = new Label();
			label47 = new Label();
			gBoxAgc = new GroupBoxEx();
			label15 = new Label();
			label16 = new Label();
			label29 = new Label();
			label31 = new Label();
			label32 = new Label();
			label33 = new Label();
			label34 = new Label();
			label46 = new Label();
			label59 = new Label();
			label60 = new Label();
			label61 = new Label();
			label62 = new Label();
			nudAgcStep5 = new NumericUpDown();
			nudAgcStep4 = new NumericUpDown();
			nudAgcReferenceLevel = new NumericUpDownEx();
			nudAgcStep3 = new NumericUpDown();
			nudAgcStep1 = new NumericUpDown();
			nudAgcStep2 = new NumericUpDown();
			gBoxTxSettings = new GroupBoxEx();
			pnlPa20dBm = new Panel();
			rBtnPa20dBmOff = new RadioButton();
			rBtnPa20dBmOn = new RadioButton();
			lblPa20dBm = new Label();
			nudMaxOutputPower = new NumericUpDownEx();
			label7 = new Label();
			label6 = new Label();
			nudPllBandwidth = new NumericUpDownEx();
			panel4 = new Panel();
			rBtnOcpOff = new RadioButton();
			rBtnOcpOn = new RadioButton();
			label19 = new Label();
			label10 = new Label();
			label5 = new Label();
			label8 = new Label();
			nudOcpTrim = new NumericUpDownEx();
			suffixOCPtrim = new Label();
			label18 = new Label();
			cBoxPaRamp = new ComboBox();
			pnlPaSelect = new Panel();
			rBtnRfPa = new RadioButton();
			rBtnRfo = new RadioButton();
			nudOutputPower = new NumericUpDownEx();
			suffixOutputPower = new Label();
			suffixPAramp = new Label();
			label12 = new Label();
			gBoxGeneral = new GroupBoxEx();
			gBoxOptioanl = new GroupBoxEx();
			panel10 = new Panel();
			rBtnForceRxBandLowFrequencyOff = new RadioButton();
			rBtnForceRxBandLowFrequencyOn = new RadioButton();
			label38 = new Label();
			cBoxBand = new ComboBox();
			panel8 = new Panel();
			rBtnLowFrequencyModeOff = new RadioButton();
			rBtnLowFrequencyModeOn = new RadioButton();
			label36 = new Label();
			label37 = new Label();
			label11 = new Label();
			panel11 = new Panel();
			rBtnForceTxBandLowFrequencyOff = new RadioButton();
			rBtnForceTxBandLowFrequencyOn = new RadioButton();
			panel5 = new Panel();
			rBtnFastHopOff = new RadioButton();
			rBtnFastHopOn = new RadioButton();
			label39 = new Label();
			label3 = new Label();
			panel1 = new Panel();
			rBtnTcxoInputOff = new RadioButton();
			rBtnTcxoInputOn = new RadioButton();
			nudFrequencyXo = new NumericUpDownEx();
			label9 = new Label();
			label1 = new Label();
			label13 = new Label();
			lblRcOscillatorCalStat = new Label();
			label14 = new Label();
			nudFrequencyRf = new NumericUpDownEx();
			((ISupportInitialize)errorProvider).BeginInit();
			groupBox1.SuspendLayout();
			tableLayoutPanel1.SuspendLayout();
			gBoxRxSettings.SuspendLayout();
			panel12.SuspendLayout();
			panel9.SuspendLayout();
			panel3.SuspendLayout();
			panel2.SuspendLayout();
			gBoxAgc.SuspendLayout();
			((ISupportInitialize)nudAgcStep5).BeginInit();
			((ISupportInitialize)nudAgcStep4).BeginInit();
			((ISupportInitialize)nudAgcReferenceLevel).BeginInit();
			((ISupportInitialize)nudAgcStep3).BeginInit();
			((ISupportInitialize)nudAgcStep1).BeginInit();
			((ISupportInitialize)nudAgcStep2).BeginInit();
			gBoxTxSettings.SuspendLayout();
			pnlPa20dBm.SuspendLayout();
			((ISupportInitialize)nudMaxOutputPower).BeginInit();
			((ISupportInitialize)nudPllBandwidth).BeginInit();
			panel4.SuspendLayout();
			((ISupportInitialize)nudOcpTrim).BeginInit();
			pnlPaSelect.SuspendLayout();
			((ISupportInitialize)nudOutputPower).BeginInit();
			gBoxGeneral.SuspendLayout();
			gBoxOptioanl.SuspendLayout();
			panel10.SuspendLayout();
			panel8.SuspendLayout();
			panel11.SuspendLayout();
			panel5.SuspendLayout();
			panel1.SuspendLayout();
			((ISupportInitialize)nudFrequencyXo).BeginInit();
			((ISupportInitialize)nudFrequencyRf).BeginInit();
			SuspendLayout();
			label20.Location = new Point(0, 0);
			label20.Name = "label20";
			label20.Size = new Size(100, 23);
			label20.TabIndex = 0;
			panel6.AutoSize = true;
			panel6.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel6.Location = new Point(165, 85);
			panel6.Name = "panel6";
			panel6.Size = new Size(98, 17);
			panel6.TabIndex = 1;
			label21.AutoSize = true;
			label21.Location = new Point(8, 114);
			label21.Name = "label21";
			label21.Size = new Size(105, 13);
			label21.TabIndex = 3;
			label21.Text = "Listen resolution idle:";
			label22.AutoSize = true;
			label22.Location = new Point(295, 112);
			label22.Name = "label22";
			label22.Size = new Size(18, 13);
			label22.TabIndex = 5;
			label22.Text = "µs";
			panel7.Location = new Point(0, 0);
			panel7.Name = "panel7";
			panel7.Size = new Size(200, 100);
			panel7.TabIndex = 0;
			label23.AutoSize = true;
			label23.Location = new Point(8, 167);
			label23.Name = "label23";
			label23.Size = new Size(72, 13);
			label23.TabIndex = 9;
			label23.Text = "Listen criteria:";
			label24.AutoSize = true;
			label24.Location = new Point(8, 217);
			label24.Name = "label24";
			label24.Size = new Size(59, 13);
			label24.TabIndex = 11;
			label24.Text = "Listen end:";
			label25.AutoSize = true;
			label25.Location = new Point(295, 245);
			label25.Name = "label25";
			label25.Size = new Size(20, 13);
			label25.TabIndex = 15;
			label25.Text = "ms";
			label26.AutoSize = true;
			label26.Location = new Point(295, 271);
			label26.Name = "label26";
			label26.Size = new Size(20, 13);
			label26.TabIndex = 18;
			label26.Text = "ms";
			label27.AutoSize = true;
			label27.Location = new Point(8, 245);
			label27.Name = "label27";
			label27.Size = new Size(79, 13);
			label27.TabIndex = 13;
			label27.Text = "Listen idle time:";
			label28.AutoSize = true;
			label28.Location = new Point(8, 270);
			label28.Name = "label28";
			label28.Size = new Size(76, 13);
			label28.TabIndex = 16;
			label28.Text = "Listen Rx time:";
			errorProvider.ContainerControl = this;
			lblListenResolRx.AutoSize = true;
			lblListenResolRx.Location = new Point(8, 141);
			lblListenResolRx.Name = "lblListenResolRx";
			lblListenResolRx.Size = new Size(102, 13);
			lblListenResolRx.TabIndex = 6;
			lblListenResolRx.Text = "Listen resolution Rx:";
			label30.AutoSize = true;
			label30.Location = new Point(295, 139);
			label30.Name = "label30";
			label30.Size = new Size(18, 13);
			label30.TabIndex = 8;
			label30.Text = "µs";
			groupBox1.Controls.Add(tableLayoutPanel1);
			groupBox1.Location = new Point(3, 402);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new Size(793, 81);
			groupBox1.TabIndex = 14;
			groupBox1.TabStop = false;
			groupBox1.Text = "DIO mapping";
			tableLayoutPanel1.AutoSize = true;
			tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			tableLayoutPanel1.ColumnCount = 6;
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			tableLayoutPanel1.Controls.Add(cBoxDio4Mapping, 1, 1);
			tableLayoutPanel1.Controls.Add(cBoxDio3Mapping, 2, 1);
			tableLayoutPanel1.Controls.Add(cBoxDio2Mapping, 3, 1);
			tableLayoutPanel1.Controls.Add(cBoxDio1Mapping, 4, 1);
			tableLayoutPanel1.Controls.Add(cBoxDio0Mapping, 5, 1);
			tableLayoutPanel1.Controls.Add(label54, 5, 0);
			tableLayoutPanel1.Controls.Add(label55, 4, 0);
			tableLayoutPanel1.Controls.Add(label56, 3, 0);
			tableLayoutPanel1.Controls.Add(label57, 2, 0);
			tableLayoutPanel1.Controls.Add(label58, 1, 0);
			tableLayoutPanel1.Controls.Add(label35, 0, 0);
			tableLayoutPanel1.Controls.Add(cBoxDio5Mapping, 0, 1);
			tableLayoutPanel1.Location = new Point(6, 19);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 2;
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.Size = new Size(781, 49);
			tableLayoutPanel1.TabIndex = 29;
			cBoxDio4Mapping.Anchor = AnchorStyles.None;
			cBoxDio4Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxDio4Mapping.FormattingEnabled = true;
			cBoxDio4Mapping.Items.AddRange(["CadDetected", "PllLock", "PllLock", "-"]);
			cBoxDio4Mapping.Location = new Point(133, 24);
			cBoxDio4Mapping.Name = "cBoxDio4Mapping";
			cBoxDio4Mapping.Size = new Size(122, 21);
			cBoxDio4Mapping.TabIndex = 0;
			cBoxDio4Mapping.SelectedIndexChanged += cBoxDio4Mapping_SelectedIndexChanged;
			cBoxDio3Mapping.Anchor = AnchorStyles.None;
			cBoxDio3Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxDio3Mapping.FormattingEnabled = true;
			cBoxDio3Mapping.Items.AddRange(["CadDone", "ValidHeader", "PayloadCrcError", "-"]);
			cBoxDio3Mapping.Location = new Point(262, 24);
			cBoxDio3Mapping.Name = "cBoxDio3Mapping";
			cBoxDio3Mapping.Size = new Size(122, 21);
			cBoxDio3Mapping.TabIndex = 0;
			cBoxDio3Mapping.SelectedIndexChanged += cBoxDio3Mapping_SelectedIndexChanged;
			cBoxDio2Mapping.Anchor = AnchorStyles.None;
			cBoxDio2Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxDio2Mapping.FormattingEnabled = true;
			cBoxDio2Mapping.Items.AddRange(["FhssChangeChannel", "FhssChangeChannel", "FhssChangeChannel", "-"]);
			cBoxDio2Mapping.Location = new Point(391, 24);
			cBoxDio2Mapping.Name = "cBoxDio2Mapping";
			cBoxDio2Mapping.Size = new Size(122, 21);
			cBoxDio2Mapping.TabIndex = 0;
			cBoxDio2Mapping.SelectedIndexChanged += cBoxDio2Mapping_SelectedIndexChanged;
			cBoxDio1Mapping.Anchor = AnchorStyles.None;
			cBoxDio1Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxDio1Mapping.FormattingEnabled = true;
			cBoxDio1Mapping.Items.AddRange(["RxTimeout", "FhssChangeChannel", "CadDetected", "-"]);
			cBoxDio1Mapping.Location = new Point(520, 24);
			cBoxDio1Mapping.Name = "cBoxDio1Mapping";
			cBoxDio1Mapping.Size = new Size(122, 21);
			cBoxDio1Mapping.TabIndex = 0;
			cBoxDio1Mapping.SelectedIndexChanged += cBoxDio1Mapping_SelectedIndexChanged;
			cBoxDio0Mapping.Anchor = AnchorStyles.None;
			cBoxDio0Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxDio0Mapping.FormattingEnabled = true;
			cBoxDio0Mapping.Items.AddRange(["RxDone", "TxDone", "CadDone", "-"]);
			cBoxDio0Mapping.Location = new Point(651, 24);
			cBoxDio0Mapping.Name = "cBoxDio0Mapping";
			cBoxDio0Mapping.Size = new Size(123, 21);
			cBoxDio0Mapping.TabIndex = 0;
			cBoxDio0Mapping.SelectedIndexChanged += cBoxDio0Mapping_SelectedIndexChanged;
			label54.Anchor = AnchorStyles.None;
			label54.AutoSize = true;
			label54.Location = new Point(697, 4);
			label54.Margin = new Padding(3);
			label54.Name = "label54";
			label54.Size = new Size(32, 13);
			label54.TabIndex = 1;
			label54.Text = "DIO0";
			label54.TextAlign = ContentAlignment.MiddleCenter;
			label55.Anchor = AnchorStyles.None;
			label55.AutoSize = true;
			label55.Location = new Point(565, 4);
			label55.Margin = new Padding(3);
			label55.Name = "label55";
			label55.Size = new Size(32, 13);
			label55.TabIndex = 1;
			label55.Text = "DIO1";
			label55.TextAlign = ContentAlignment.MiddleCenter;
			label56.Anchor = AnchorStyles.None;
			label56.AutoSize = true;
			label56.Location = new Point(436, 4);
			label56.Margin = new Padding(3);
			label56.Name = "label56";
			label56.Size = new Size(32, 13);
			label56.TabIndex = 1;
			label56.Text = "DIO2";
			label56.TextAlign = ContentAlignment.MiddleCenter;
			label57.Anchor = AnchorStyles.None;
			label57.AutoSize = true;
			label57.Location = new Point(307, 4);
			label57.Margin = new Padding(3);
			label57.Name = "label57";
			label57.Size = new Size(32, 13);
			label57.TabIndex = 1;
			label57.Text = "DIO3";
			label57.TextAlign = ContentAlignment.MiddleCenter;
			label58.Anchor = AnchorStyles.None;
			label58.AutoSize = true;
			label58.Location = new Point(178, 4);
			label58.Margin = new Padding(3);
			label58.Name = "label58";
			label58.Size = new Size(32, 13);
			label58.TabIndex = 1;
			label58.Text = "DIO4";
			label58.TextAlign = ContentAlignment.MiddleCenter;
			label35.Anchor = AnchorStyles.None;
			label35.AutoSize = true;
			label35.Location = new Point(49, 4);
			label35.Margin = new Padding(3);
			label35.Name = "label35";
			label35.Size = new Size(32, 13);
			label35.TabIndex = 1;
			label35.Text = "DIO5";
			label35.TextAlign = ContentAlignment.MiddleCenter;
			cBoxDio5Mapping.Anchor = AnchorStyles.None;
			cBoxDio5Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxDio5Mapping.FormattingEnabled = true;
			cBoxDio5Mapping.Items.AddRange(["ModeReady", "ClkOut", "ClkOut", "-"]);
			cBoxDio5Mapping.Location = new Point(4, 24);
			cBoxDio5Mapping.Name = "cBoxDio5Mapping";
			cBoxDio5Mapping.Size = new Size(122, 21);
			cBoxDio5Mapping.TabIndex = 0;
			cBoxDio5Mapping.SelectedIndexChanged += cBoxDio5Mapping_SelectedIndexChanged;
			gBoxRxSettings.Controls.Add(panel12);
			gBoxRxSettings.Controls.Add(label4);
			gBoxRxSettings.Controls.Add(lblAgcReference);
			gBoxRxSettings.Controls.Add(label48);
			gBoxRxSettings.Controls.Add(label49);
			gBoxRxSettings.Controls.Add(label50);
			gBoxRxSettings.Controls.Add(label51);
			gBoxRxSettings.Controls.Add(label52);
			gBoxRxSettings.Controls.Add(lblLnaGain1);
			gBoxRxSettings.Controls.Add(label53);
			gBoxRxSettings.Controls.Add(panel2);
			gBoxRxSettings.Controls.Add(lblLnaGain2);
			gBoxRxSettings.Controls.Add(lblLnaGain3);
			gBoxRxSettings.Controls.Add(lblLnaGain4);
			gBoxRxSettings.Controls.Add(lblLnaGain5);
			gBoxRxSettings.Controls.Add(lblLnaGain6);
			gBoxRxSettings.Controls.Add(lblAgcThresh1);
			gBoxRxSettings.Controls.Add(lblAgcThresh2);
			gBoxRxSettings.Controls.Add(lblAgcThresh3);
			gBoxRxSettings.Controls.Add(lblAgcThresh4);
			gBoxRxSettings.Controls.Add(lblAgcThresh5);
			gBoxRxSettings.Controls.Add(label47);
			gBoxRxSettings.Location = new Point(3, 293);
			gBoxRxSettings.Name = "gBoxRxSettings";
			gBoxRxSettings.Size = new Size(793, 103);
			gBoxRxSettings.TabIndex = 8;
			gBoxRxSettings.TabStop = false;
			gBoxRxSettings.Text = "Rx settings";
			panel12.AutoSize = true;
			panel12.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel12.Controls.Add(panel9);
			panel12.Controls.Add(label17);
			panel12.Controls.Add(label2);
			panel12.Controls.Add(panel3);
			panel12.Location = new Point(264, 28);
			panel12.Name = "panel12";
			panel12.Size = new Size(264, 46);
			panel12.TabIndex = 7;
			panel9.AutoSize = true;
			panel9.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel9.Controls.Add(rBtnAgcAutoOff);
			panel9.Controls.Add(rBtnAgcAutoOn);
			panel9.Location = new Point(163, 3);
			panel9.Name = "panel9";
			panel9.Size = new Size(98, 17);
			panel9.TabIndex = 23;
			rBtnAgcAutoOff.AutoSize = true;
			rBtnAgcAutoOff.Location = new Point(50, 0);
			rBtnAgcAutoOff.Margin = new Padding(3, 0, 3, 0);
			rBtnAgcAutoOff.Name = "rBtnAgcAutoOff";
			rBtnAgcAutoOff.Size = new Size(45, 17);
			rBtnAgcAutoOff.TabIndex = 1;
			rBtnAgcAutoOff.Text = "OFF";
			rBtnAgcAutoOff.UseVisualStyleBackColor = true;
			rBtnAgcAutoOff.CheckedChanged += rBtnAgcAutoOn_CheckedChanged;
			rBtnAgcAutoOn.AutoSize = true;
			rBtnAgcAutoOn.Checked = true;
			rBtnAgcAutoOn.Location = new Point(3, 0);
			rBtnAgcAutoOn.Margin = new Padding(3, 0, 3, 0);
			rBtnAgcAutoOn.Name = "rBtnAgcAutoOn";
			rBtnAgcAutoOn.Size = new Size(41, 17);
			rBtnAgcAutoOn.TabIndex = 0;
			rBtnAgcAutoOn.TabStop = true;
			rBtnAgcAutoOn.Text = "ON";
			rBtnAgcAutoOn.UseVisualStyleBackColor = true;
			rBtnAgcAutoOn.CheckedChanged += rBtnAgcAutoOn_CheckedChanged;
			label17.AutoSize = true;
			label17.Location = new Point(3, 5);
			label17.Name = "label17";
			label17.Size = new Size(56, 13);
			label17.TabIndex = 22;
			label17.Text = "AGC auto:";
			label2.AutoSize = true;
			label2.Location = new Point(3, 28);
			label2.Name = "label2";
			label2.Size = new Size(60, 13);
			label2.TabIndex = 0;
			label2.Text = "LNA boost:";
			panel3.AutoSize = true;
			panel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel3.Controls.Add(rBtnLnaBoostOff);
			panel3.Controls.Add(rBtnLnaBoostOn);
			panel3.Location = new Point(163, 26);
			panel3.Name = "panel3";
			panel3.Size = new Size(98, 17);
			panel3.TabIndex = 23;
			rBtnLnaBoostOff.AutoSize = true;
			rBtnLnaBoostOff.Location = new Point(50, 0);
			rBtnLnaBoostOff.Margin = new Padding(3, 0, 3, 0);
			rBtnLnaBoostOff.Name = "rBtnLnaBoostOff";
			rBtnLnaBoostOff.Size = new Size(45, 17);
			rBtnLnaBoostOff.TabIndex = 1;
			rBtnLnaBoostOff.Text = "OFF";
			rBtnLnaBoostOff.UseVisualStyleBackColor = true;
			rBtnLnaBoostOff.CheckedChanged += rBtnLnaBoost_CheckedChanged;
			rBtnLnaBoostOn.AutoSize = true;
			rBtnLnaBoostOn.Checked = true;
			rBtnLnaBoostOn.Location = new Point(3, 0);
			rBtnLnaBoostOn.Margin = new Padding(3, 0, 3, 0);
			rBtnLnaBoostOn.Name = "rBtnLnaBoostOn";
			rBtnLnaBoostOn.Size = new Size(41, 17);
			rBtnLnaBoostOn.TabIndex = 0;
			rBtnLnaBoostOn.TabStop = true;
			rBtnLnaBoostOn.Text = "ON";
			rBtnLnaBoostOn.UseVisualStyleBackColor = true;
			rBtnLnaBoostOn.CheckedChanged += rBtnLnaBoost_CheckedChanged;
			label4.BackColor = Color.Transparent;
			label4.Location = new Point(167, 75);
			label4.Name = "label4";
			label4.Size = new Size(42, 13);
			label4.TabIndex = 6;
			label4.Text = "Gain";
			label4.TextAlign = ContentAlignment.MiddleCenter;
			label4.Visible = false;
			lblAgcReference.BackColor = Color.Transparent;
			lblAgcReference.Location = new Point(124, 32);
			lblAgcReference.Margin = new Padding(0, 0, 0, 3);
			lblAgcReference.Name = "lblAgcReference";
			lblAgcReference.Size = new Size(100, 13);
			lblAgcReference.TabIndex = 7;
			lblAgcReference.Text = "-80";
			lblAgcReference.TextAlign = ContentAlignment.MiddleCenter;
			lblAgcReference.Visible = false;
			label48.BackColor = Color.Transparent;
			label48.Location = new Point(124, 16);
			label48.Margin = new Padding(0, 0, 0, 3);
			label48.Name = "label48";
			label48.Size = new Size(100, 13);
			label48.TabIndex = 0;
			label48.Text = "Reference";
			label48.TextAlign = ContentAlignment.MiddleCenter;
			label48.Visible = false;
			label49.BackColor = Color.Transparent;
			label49.Location = new Point(224, 16);
			label49.Margin = new Padding(0, 0, 0, 3);
			label49.Name = "label49";
			label49.Size = new Size(100, 13);
			label49.TabIndex = 1;
			label49.Text = "Threshold 1";
			label49.TextAlign = ContentAlignment.MiddleCenter;
			label49.Visible = false;
			label50.BackColor = Color.Transparent;
			label50.Location = new Point(324, 16);
			label50.Margin = new Padding(0, 0, 0, 3);
			label50.Name = "label50";
			label50.Size = new Size(100, 13);
			label50.TabIndex = 2;
			label50.Text = "Threshold 2";
			label50.TextAlign = ContentAlignment.MiddleCenter;
			label50.Visible = false;
			label51.BackColor = Color.Transparent;
			label51.Location = new Point(424, 16);
			label51.Margin = new Padding(0, 0, 0, 3);
			label51.Name = "label51";
			label51.Size = new Size(100, 13);
			label51.TabIndex = 3;
			label51.Text = "Threshold 3";
			label51.TextAlign = ContentAlignment.MiddleCenter;
			label51.Visible = false;
			label52.BackColor = Color.Transparent;
			label52.Location = new Point(524, 16);
			label52.Margin = new Padding(0, 0, 0, 3);
			label52.Name = "label52";
			label52.Size = new Size(100, 13);
			label52.TabIndex = 4;
			label52.Text = "Threshold 4";
			label52.TextAlign = ContentAlignment.MiddleCenter;
			label52.Visible = false;
			lblLnaGain1.BackColor = Color.LightSteelBlue;
			lblLnaGain1.BorderStyle = BorderStyle.Fixed3D;
			lblLnaGain1.Location = new Point(174, 48);
			lblLnaGain1.Margin = new Padding(0, 0, 0, 3);
			lblLnaGain1.Name = "lblLnaGain1";
			lblLnaGain1.Size = new Size(100, 20);
			lblLnaGain1.TabIndex = 14;
			lblLnaGain1.Text = "G1";
			lblLnaGain1.TextAlign = ContentAlignment.MiddleCenter;
			lblLnaGain1.Visible = false;
			label53.BackColor = Color.Transparent;
			label53.Location = new Point(624, 16);
			label53.Margin = new Padding(0, 0, 0, 3);
			label53.Name = "label53";
			label53.Size = new Size(100, 13);
			label53.TabIndex = 5;
			label53.Text = "Threshold 5";
			label53.TextAlign = ContentAlignment.MiddleCenter;
			label53.Visible = false;
			panel2.AutoSize = true;
			panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel2.Controls.Add(rBtnLnaGain1);
			panel2.Controls.Add(rBtnLnaGain2);
			panel2.Controls.Add(rBtnLnaGain3);
			panel2.Controls.Add(rBtnLnaGain4);
			panel2.Controls.Add(rBtnLnaGain5);
			panel2.Controls.Add(rBtnLnaGain6);
			panel2.Location = new Point(215, 75);
			panel2.Name = "panel2";
			panel2.Size = new Size(521, 13);
			panel2.TabIndex = 22;
			panel2.Visible = false;
			rBtnLnaGain1.AutoSize = true;
			rBtnLnaGain1.Checked = true;
			rBtnLnaGain1.Location = new Point(3, 0);
			rBtnLnaGain1.Margin = new Padding(3, 0, 3, 0);
			rBtnLnaGain1.Name = "rBtnLnaGain1";
			rBtnLnaGain1.Size = new Size(14, 13);
			rBtnLnaGain1.TabIndex = 0;
			rBtnLnaGain1.TabStop = true;
			rBtnLnaGain1.UseVisualStyleBackColor = true;
			rBtnLnaGain1.CheckedChanged += rBtnLnaGain_CheckedChanged;
			rBtnLnaGain2.AutoSize = true;
			rBtnLnaGain2.Location = new Point(102, 0);
			rBtnLnaGain2.Margin = new Padding(3, 0, 3, 0);
			rBtnLnaGain2.Name = "rBtnLnaGain2";
			rBtnLnaGain2.Size = new Size(14, 13);
			rBtnLnaGain2.TabIndex = 1;
			rBtnLnaGain2.UseVisualStyleBackColor = true;
			rBtnLnaGain2.CheckedChanged += rBtnLnaGain_CheckedChanged;
			rBtnLnaGain3.AutoSize = true;
			rBtnLnaGain3.Location = new Point(203, 0);
			rBtnLnaGain3.Margin = new Padding(3, 0, 3, 0);
			rBtnLnaGain3.Name = "rBtnLnaGain3";
			rBtnLnaGain3.Size = new Size(14, 13);
			rBtnLnaGain3.TabIndex = 2;
			rBtnLnaGain3.UseVisualStyleBackColor = true;
			rBtnLnaGain3.CheckedChanged += rBtnLnaGain_CheckedChanged;
			rBtnLnaGain4.AutoSize = true;
			rBtnLnaGain4.Location = new Point(303, 0);
			rBtnLnaGain4.Margin = new Padding(3, 0, 3, 0);
			rBtnLnaGain4.Name = "rBtnLnaGain4";
			rBtnLnaGain4.Size = new Size(14, 13);
			rBtnLnaGain4.TabIndex = 3;
			rBtnLnaGain4.UseVisualStyleBackColor = true;
			rBtnLnaGain4.CheckedChanged += rBtnLnaGain_CheckedChanged;
			rBtnLnaGain5.AutoSize = true;
			rBtnLnaGain5.Location = new Point(404, 0);
			rBtnLnaGain5.Margin = new Padding(3, 0, 3, 0);
			rBtnLnaGain5.Name = "rBtnLnaGain5";
			rBtnLnaGain5.Size = new Size(14, 13);
			rBtnLnaGain5.TabIndex = 4;
			rBtnLnaGain5.UseVisualStyleBackColor = true;
			rBtnLnaGain5.CheckedChanged += rBtnLnaGain_CheckedChanged;
			rBtnLnaGain6.AutoSize = true;
			rBtnLnaGain6.Location = new Point(504, 0);
			rBtnLnaGain6.Margin = new Padding(3, 0, 3, 0);
			rBtnLnaGain6.Name = "rBtnLnaGain6";
			rBtnLnaGain6.Size = new Size(14, 13);
			rBtnLnaGain6.TabIndex = 5;
			rBtnLnaGain6.UseVisualStyleBackColor = true;
			rBtnLnaGain6.CheckedChanged += rBtnLnaGain_CheckedChanged;
			lblLnaGain2.BackColor = Color.Transparent;
			lblLnaGain2.BorderStyle = BorderStyle.Fixed3D;
			lblLnaGain2.Location = new Point(274, 48);
			lblLnaGain2.Margin = new Padding(0, 0, 0, 3);
			lblLnaGain2.Name = "lblLnaGain2";
			lblLnaGain2.Size = new Size(100, 20);
			lblLnaGain2.TabIndex = 15;
			lblLnaGain2.Text = "G2";
			lblLnaGain2.TextAlign = ContentAlignment.MiddleCenter;
			lblLnaGain2.Visible = false;
			lblLnaGain3.BackColor = Color.Transparent;
			lblLnaGain3.BorderStyle = BorderStyle.Fixed3D;
			lblLnaGain3.Location = new Point(374, 48);
			lblLnaGain3.Margin = new Padding(0, 0, 0, 3);
			lblLnaGain3.Name = "lblLnaGain3";
			lblLnaGain3.Size = new Size(100, 20);
			lblLnaGain3.TabIndex = 16;
			lblLnaGain3.Text = "G3";
			lblLnaGain3.TextAlign = ContentAlignment.MiddleCenter;
			lblLnaGain3.Visible = false;
			lblLnaGain4.BackColor = Color.Transparent;
			lblLnaGain4.BorderStyle = BorderStyle.Fixed3D;
			lblLnaGain4.Location = new Point(474, 48);
			lblLnaGain4.Margin = new Padding(0, 0, 0, 3);
			lblLnaGain4.Name = "lblLnaGain4";
			lblLnaGain4.Size = new Size(100, 20);
			lblLnaGain4.TabIndex = 17;
			lblLnaGain4.Text = "G4";
			lblLnaGain4.TextAlign = ContentAlignment.MiddleCenter;
			lblLnaGain4.Visible = false;
			lblLnaGain5.BackColor = Color.Transparent;
			lblLnaGain5.BorderStyle = BorderStyle.Fixed3D;
			lblLnaGain5.Location = new Point(574, 48);
			lblLnaGain5.Margin = new Padding(0, 0, 0, 3);
			lblLnaGain5.Name = "lblLnaGain5";
			lblLnaGain5.Size = new Size(100, 20);
			lblLnaGain5.TabIndex = 18;
			lblLnaGain5.Text = "G5";
			lblLnaGain5.TextAlign = ContentAlignment.MiddleCenter;
			lblLnaGain5.Visible = false;
			lblLnaGain6.BackColor = Color.Transparent;
			lblLnaGain6.BorderStyle = BorderStyle.Fixed3D;
			lblLnaGain6.Location = new Point(674, 48);
			lblLnaGain6.Margin = new Padding(0, 0, 0, 3);
			lblLnaGain6.Name = "lblLnaGain6";
			lblLnaGain6.Size = new Size(100, 20);
			lblLnaGain6.TabIndex = 19;
			lblLnaGain6.Text = "G6";
			lblLnaGain6.TextAlign = ContentAlignment.MiddleCenter;
			lblLnaGain6.Visible = false;
			lblAgcThresh1.BackColor = Color.Transparent;
			lblAgcThresh1.Location = new Point(224, 32);
			lblAgcThresh1.Margin = new Padding(0, 0, 0, 3);
			lblAgcThresh1.Name = "lblAgcThresh1";
			lblAgcThresh1.Size = new Size(100, 13);
			lblAgcThresh1.TabIndex = 8;
			lblAgcThresh1.Text = "0";
			lblAgcThresh1.TextAlign = ContentAlignment.MiddleCenter;
			lblAgcThresh1.Visible = false;
			lblAgcThresh2.BackColor = Color.Transparent;
			lblAgcThresh2.Location = new Point(324, 32);
			lblAgcThresh2.Margin = new Padding(0, 0, 0, 3);
			lblAgcThresh2.Name = "lblAgcThresh2";
			lblAgcThresh2.Size = new Size(100, 13);
			lblAgcThresh2.TabIndex = 9;
			lblAgcThresh2.Text = "0";
			lblAgcThresh2.TextAlign = ContentAlignment.MiddleCenter;
			lblAgcThresh2.Visible = false;
			lblAgcThresh3.BackColor = Color.Transparent;
			lblAgcThresh3.Location = new Point(424, 32);
			lblAgcThresh3.Margin = new Padding(0, 0, 0, 3);
			lblAgcThresh3.Name = "lblAgcThresh3";
			lblAgcThresh3.Size = new Size(100, 13);
			lblAgcThresh3.TabIndex = 10;
			lblAgcThresh3.Text = "0";
			lblAgcThresh3.TextAlign = ContentAlignment.MiddleCenter;
			lblAgcThresh3.Visible = false;
			lblAgcThresh4.BackColor = Color.Transparent;
			lblAgcThresh4.Location = new Point(524, 32);
			lblAgcThresh4.Margin = new Padding(0, 0, 0, 3);
			lblAgcThresh4.Name = "lblAgcThresh4";
			lblAgcThresh4.Size = new Size(100, 13);
			lblAgcThresh4.TabIndex = 11;
			lblAgcThresh4.Text = "0";
			lblAgcThresh4.TextAlign = ContentAlignment.MiddleCenter;
			lblAgcThresh4.Visible = false;
			lblAgcThresh5.BackColor = Color.Transparent;
			lblAgcThresh5.Location = new Point(624, 32);
			lblAgcThresh5.Margin = new Padding(0, 0, 0, 3);
			lblAgcThresh5.Name = "lblAgcThresh5";
			lblAgcThresh5.Size = new Size(100, 13);
			lblAgcThresh5.TabIndex = 12;
			lblAgcThresh5.Text = "0";
			lblAgcThresh5.TextAlign = ContentAlignment.MiddleCenter;
			lblAgcThresh5.Visible = false;
			label47.AutoSize = true;
			label47.BackColor = Color.Transparent;
			label47.Location = new Point(723, 32);
			label47.Margin = new Padding(0);
			label47.Name = "label47";
			label47.Size = new Size(64, 13);
			label47.TabIndex = 13;
			label47.Text = "-> Pin [dBm]";
			label47.TextAlign = ContentAlignment.MiddleLeft;
			label47.Visible = false;
			gBoxAgc.Controls.Add(label15);
			gBoxAgc.Controls.Add(label16);
			gBoxAgc.Controls.Add(label29);
			gBoxAgc.Controls.Add(label31);
			gBoxAgc.Controls.Add(label32);
			gBoxAgc.Controls.Add(label33);
			gBoxAgc.Controls.Add(label34);
			gBoxAgc.Controls.Add(label46);
			gBoxAgc.Controls.Add(label59);
			gBoxAgc.Controls.Add(label60);
			gBoxAgc.Controls.Add(label61);
			gBoxAgc.Controls.Add(label62);
			gBoxAgc.Controls.Add(nudAgcStep5);
			gBoxAgc.Controls.Add(nudAgcStep4);
			gBoxAgc.Controls.Add(nudAgcReferenceLevel);
			gBoxAgc.Controls.Add(nudAgcStep3);
			gBoxAgc.Controls.Add(nudAgcStep1);
			gBoxAgc.Controls.Add(nudAgcStep2);
			gBoxAgc.Location = new Point(3, 489);
			gBoxAgc.Name = "gBoxAgc";
			gBoxAgc.Size = new Size(515, 98);
			gBoxAgc.TabIndex = 13;
			gBoxAgc.TabStop = false;
			gBoxAgc.Text = "AGC";
			gBoxAgc.Visible = false;
			label15.AutoSize = true;
			label15.BackColor = Color.Transparent;
			label15.Location = new Point(2, 23);
			label15.Name = "label15";
			label15.Size = new Size(89, 13);
			label15.TabIndex = 2;
			label15.Text = "Reference Level:";
			label16.AutoSize = true;
			label16.BackColor = Color.Transparent;
			label16.Location = new Point(244, 23);
			label16.Name = "label16";
			label16.Size = new Size(89, 13);
			label16.TabIndex = 8;
			label16.Text = "Threshold step 1:";
			label29.AutoSize = true;
			label29.BackColor = Color.Transparent;
			label29.Location = new Point(2, 49);
			label29.Name = "label29";
			label29.Size = new Size(89, 13);
			label29.TabIndex = 11;
			label29.Text = "Threshold step 2:";
			label31.AutoSize = true;
			label31.BackColor = Color.Transparent;
			label31.Location = new Point(244, 49);
			label31.Name = "label31";
			label31.Size = new Size(89, 13);
			label31.TabIndex = 14;
			label31.Text = "Threshold step 3:";
			label32.AutoSize = true;
			label32.BackColor = Color.Transparent;
			label32.Location = new Point(2, 75);
			label32.Name = "label32";
			label32.Size = new Size(89, 13);
			label32.TabIndex = 17;
			label32.Text = "Threshold step 4:";
			label33.AutoSize = true;
			label33.BackColor = Color.Transparent;
			label33.Location = new Point(244, 75);
			label33.Name = "label33";
			label33.Size = new Size(89, 13);
			label33.TabIndex = 20;
			label33.Text = "Threshold step 5:";
			label34.AutoSize = true;
			label34.BackColor = Color.Transparent;
			label34.Location = new Point(223, 23);
			label34.Name = "label34";
			label34.Size = new Size(20, 13);
			label34.TabIndex = 4;
			label34.Text = "dB";
			label46.AutoSize = true;
			label46.BackColor = Color.Transparent;
			label46.Location = new Point(465, 23);
			label46.Name = "label46";
			label46.Size = new Size(20, 13);
			label46.TabIndex = 10;
			label46.Text = "dB";
			label59.AutoSize = true;
			label59.BackColor = Color.Transparent;
			label59.Location = new Point(223, 49);
			label59.Name = "label59";
			label59.Size = new Size(20, 13);
			label59.TabIndex = 13;
			label59.Text = "dB";
			label60.AutoSize = true;
			label60.BackColor = Color.Transparent;
			label60.Location = new Point(465, 49);
			label60.Name = "label60";
			label60.Size = new Size(20, 13);
			label60.TabIndex = 16;
			label60.Text = "dB";
			label61.AutoSize = true;
			label61.BackColor = Color.Transparent;
			label61.Location = new Point(223, 75);
			label61.Name = "label61";
			label61.Size = new Size(20, 13);
			label61.TabIndex = 19;
			label61.Text = "dB";
			label62.AutoSize = true;
			label62.BackColor = Color.Transparent;
			label62.Location = new Point(465, 75);
			label62.Name = "label62";
			label62.Size = new Size(20, 13);
			label62.TabIndex = 22;
			label62.Text = "dB";
			nudAgcStep5.Location = new Point(361, 71);
			nudAgcStep5.Maximum = new decimal([15, 0, 0, 0]);
			nudAgcStep5.Name = "nudAgcStep5";
			nudAgcStep5.Size = new Size(98, 20);
			nudAgcStep5.TabIndex = 21;
			nudAgcStep5.Value = new decimal([11, 0, 0, 0]);
			nudAgcStep5.ValueChanged += nudAgcStep_ValueChanged;
			nudAgcStep4.Location = new Point(119, 71);
			nudAgcStep4.Maximum = new decimal([15, 0, 0, 0]);
			nudAgcStep4.Name = "nudAgcStep4";
			nudAgcStep4.Size = new Size(98, 20);
			nudAgcStep4.TabIndex = 18;
			nudAgcStep4.Value = new decimal([9, 0, 0, 0]);
			nudAgcStep4.ValueChanged += nudAgcStep_ValueChanged;
			nudAgcReferenceLevel.Location = new Point(119, 19);
			nudAgcReferenceLevel.Maximum = new decimal([63, 0, 0, 0]);
			nudAgcReferenceLevel.Name = "nudAgcReferenceLevel";
			nudAgcReferenceLevel.Size = new Size(98, 20);
			nudAgcReferenceLevel.TabIndex = 3;
			nudAgcReferenceLevel.ThousandsSeparator = true;
			nudAgcReferenceLevel.Value = new decimal([19, 0, 0, 0]);
			nudAgcReferenceLevel.ValueChanged += nudAgcReferenceLevel_ValueChanged;
			nudAgcStep3.Location = new Point(361, 45);
			nudAgcStep3.Maximum = new decimal([15, 0, 0, 0]);
			nudAgcStep3.Name = "nudAgcStep3";
			nudAgcStep3.Size = new Size(98, 20);
			nudAgcStep3.TabIndex = 15;
			nudAgcStep3.Value = new decimal([11, 0, 0, 0]);
			nudAgcStep3.ValueChanged += nudAgcStep_ValueChanged;
			nudAgcStep1.Location = new Point(361, 19);
			nudAgcStep1.Maximum = new decimal([31, 0, 0, 0]);
			nudAgcStep1.Name = "nudAgcStep1";
			nudAgcStep1.Size = new Size(98, 20);
			nudAgcStep1.TabIndex = 9;
			nudAgcStep1.Value = new decimal([16, 0, 0, 0]);
			nudAgcStep1.ValueChanged += nudAgcStep_ValueChanged;
			nudAgcStep2.Location = new Point(119, 45);
			nudAgcStep2.Maximum = new decimal([15, 0, 0, 0]);
			nudAgcStep2.Name = "nudAgcStep2";
			nudAgcStep2.Size = new Size(98, 20);
			nudAgcStep2.TabIndex = 12;
			nudAgcStep2.Value = new decimal([7, 0, 0, 0]);
			nudAgcStep2.ValueChanged += nudAgcStep_ValueChanged;
			gBoxTxSettings.Controls.Add(pnlPa20dBm);
			gBoxTxSettings.Controls.Add(lblPa20dBm);
			gBoxTxSettings.Controls.Add(nudMaxOutputPower);
			gBoxTxSettings.Controls.Add(label7);
			gBoxTxSettings.Controls.Add(label6);
			gBoxTxSettings.Controls.Add(nudPllBandwidth);
			gBoxTxSettings.Controls.Add(panel4);
			gBoxTxSettings.Controls.Add(label19);
			gBoxTxSettings.Controls.Add(label10);
			gBoxTxSettings.Controls.Add(label5);
			gBoxTxSettings.Controls.Add(label8);
			gBoxTxSettings.Controls.Add(nudOcpTrim);
			gBoxTxSettings.Controls.Add(suffixOCPtrim);
			gBoxTxSettings.Controls.Add(label18);
			gBoxTxSettings.Controls.Add(cBoxPaRamp);
			gBoxTxSettings.Controls.Add(pnlPaSelect);
			gBoxTxSettings.Controls.Add(nudOutputPower);
			gBoxTxSettings.Controls.Add(suffixOutputPower);
			gBoxTxSettings.Controls.Add(suffixPAramp);
			gBoxTxSettings.Controls.Add(label12);
			gBoxTxSettings.Location = new Point(3, 111);
			gBoxTxSettings.Name = "gBoxTxSettings";
			gBoxTxSettings.Size = new Size(793, 176);
			gBoxTxSettings.TabIndex = 9;
			gBoxTxSettings.TabStop = false;
			gBoxTxSettings.Text = "Tx settings";
			pnlPa20dBm.AutoSize = true;
			pnlPa20dBm.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlPa20dBm.Controls.Add(rBtnPa20dBmOff);
			pnlPa20dBm.Controls.Add(rBtnPa20dBmOn);
			pnlPa20dBm.Location = new Point(164, 150);
			pnlPa20dBm.Name = "pnlPa20dBm";
			pnlPa20dBm.Size = new Size(102, 20);
			pnlPa20dBm.TabIndex = 7;
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
			lblPa20dBm.Location = new Point(6, 154);
			lblPa20dBm.Name = "lblPa20dBm";
			lblPa20dBm.Size = new Size(144, 13);
			lblPa20dBm.TabIndex = 8;
			lblPa20dBm.Text = "+20 dBm on pin PA_BOOST:";
			nudMaxOutputPower.DecimalPlaces = 1;
			nudMaxOutputPower.Increment = new decimal([6, 0, 0, 65536]);
			nudMaxOutputPower.Location = new Point(164, 98);
			nudMaxOutputPower.Maximum = new decimal([15, 0, 0, 0]);
			nudMaxOutputPower.Minimum = new decimal([108, 0, 0, 65536]);
			nudMaxOutputPower.Name = "nudMaxOutputPower";
			nudMaxOutputPower.Size = new Size(124, 20);
			nudMaxOutputPower.TabIndex = 4;
			nudMaxOutputPower.ThousandsSeparator = true;
			nudMaxOutputPower.Value = new decimal([132, 0, 0, 65536]);
			nudMaxOutputPower.ValueChanged += nudMaxOutputPower_ValueChanged;
			label7.AutoSize = true;
			label7.Location = new Point(6, 102);
			label7.Name = "label7";
			label7.Size = new Size(119, 13);
			label7.TabIndex = 6;
			label7.Text = "Maximum output power:";
			label6.AutoSize = true;
			label6.Location = new Point(294, 102);
			label6.Name = "label6";
			label6.Size = new Size(28, 13);
			label6.TabIndex = 5;
			label6.Text = "dBm";
			nudPllBandwidth.Increment = new decimal([75000, 0, 0, 0]);
			nudPllBandwidth.Location = new Point(542, 72);
			nudPllBandwidth.Maximum = new decimal([300000, 0, 0, 0]);
			nudPllBandwidth.Minimum = new decimal([75000, 0, 0, 0]);
			nudPllBandwidth.Name = "nudPllBandwidth";
			nudPllBandwidth.Size = new Size(124, 20);
			nudPllBandwidth.TabIndex = 2;
			nudPllBandwidth.ThousandsSeparator = true;
			nudPllBandwidth.Value = new decimal([300000, 0, 0, 0]);
			nudPllBandwidth.ValueChanged += nudPllBandwidth_ValueChanged;
			panel4.AutoSize = true;
			panel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel4.Controls.Add(rBtnOcpOff);
			panel4.Controls.Add(rBtnOcpOn);
			panel4.Location = new Point(542, 98);
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
			label19.AutoSize = true;
			label19.Location = new Point(376, 102);
			label19.Name = "label19";
			label19.Size = new Size(139, 13);
			label19.TabIndex = 1;
			label19.Text = "Overload current protection:";
			label10.AutoSize = true;
			label10.Location = new Point(376, 129);
			label10.Name = "label10";
			label10.Size = new Size(130, 13);
			label10.TabIndex = 1;
			label10.Text = "Overload current trimming:";
			label5.AutoSize = true;
			label5.Location = new Point(376, 76);
			label5.Name = "label5";
			label5.Size = new Size(81, 13);
			label5.TabIndex = 1;
			label5.Text = "PLL bandwidth:";
			label8.AutoSize = true;
			label8.Location = new Point(672, 76);
			label8.Name = "label8";
			label8.Size = new Size(20, 13);
			label8.TabIndex = 3;
			label8.Text = "Hz";
			errorProvider.SetIconPadding(nudOcpTrim, 30);
			nudOcpTrim.Location = new Point(542, 125);
			nudOcpTrim.Maximum = new decimal([240, 0, 0, 0]);
			nudOcpTrim.Minimum = new decimal([45, 0, 0, 0]);
			nudOcpTrim.Name = "nudOcpTrim";
			nudOcpTrim.Size = new Size(124, 20);
			nudOcpTrim.TabIndex = 2;
			nudOcpTrim.ThousandsSeparator = true;
			nudOcpTrim.Value = new decimal([100, 0, 0, 0]);
			nudOcpTrim.ValueChanged += nudOcpTrim_ValueChanged;
			suffixOCPtrim.AutoSize = true;
			suffixOCPtrim.Location = new Point(672, 129);
			suffixOCPtrim.Name = "suffixOCPtrim";
			suffixOCPtrim.Size = new Size(22, 13);
			suffixOCPtrim.TabIndex = 3;
			suffixOCPtrim.Text = "mA";
			label18.AutoSize = true;
			label18.Location = new Point(6, 128);
			label18.Name = "label18";
			label18.Size = new Size(74, 13);
			label18.TabIndex = 3;
			label18.Text = "Output power:";
			cBoxPaRamp.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxPaRamp.Items.AddRange(
            [
                "3400", "2000", "1000", "500", "250", "125", "100", "62", "50", "40",
				"31", "25", "20", "15", "12", "10"
			]);
			cBoxPaRamp.Location = new Point(164, 71);
			cBoxPaRamp.Name = "cBoxPaRamp";
			cBoxPaRamp.Size = new Size(124, 21);
			cBoxPaRamp.TabIndex = 2;
			cBoxPaRamp.SelectedIndexChanged += cBoxPaRamp_SelectedIndexChanged;
			pnlPaSelect.AutoSize = true;
			pnlPaSelect.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			pnlPaSelect.Controls.Add(rBtnRfPa);
			pnlPaSelect.Controls.Add(rBtnRfo);
			pnlPaSelect.Location = new Point(266, 19);
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
			nudOutputPower.DecimalPlaces = 1;
			nudOutputPower.Location = new Point(164, 124);
			nudOutputPower.Maximum = new decimal([132, 0, 0, 65536]);
			nudOutputPower.Minimum = new decimal([18, 0, 0, -2147418112]);
			nudOutputPower.Name = "nudOutputPower";
			nudOutputPower.Size = new Size(124, 20);
			nudOutputPower.TabIndex = 0;
			nudOutputPower.ThousandsSeparator = true;
			nudOutputPower.Value = new decimal([132, 0, 0, 65536]);
			nudOutputPower.ValueChanged += nudOutputPower_ValueChanged;
			suffixOutputPower.AutoSize = true;
			suffixOutputPower.Location = new Point(294, 128);
			suffixOutputPower.Name = "suffixOutputPower";
			suffixOutputPower.Size = new Size(28, 13);
			suffixOutputPower.TabIndex = 1;
			suffixOutputPower.Text = "dBm";
			suffixPAramp.AutoSize = true;
			suffixPAramp.Location = new Point(294, 75);
			suffixPAramp.Name = "suffixPAramp";
			suffixPAramp.Size = new Size(18, 13);
			suffixPAramp.TabIndex = 3;
			suffixPAramp.Text = "µs";
			label12.AutoSize = true;
			label12.Location = new Point(6, 75);
			label12.Name = "label12";
			label12.Size = new Size(50, 13);
			label12.TabIndex = 1;
			label12.Text = "PA ramp:";
			gBoxGeneral.Controls.Add(gBoxOptioanl);
			gBoxGeneral.Controls.Add(panel1);
			gBoxGeneral.Controls.Add(nudFrequencyXo);
			gBoxGeneral.Controls.Add(label9);
			gBoxGeneral.Controls.Add(label1);
			gBoxGeneral.Controls.Add(label13);
			gBoxGeneral.Controls.Add(lblRcOscillatorCalStat);
			gBoxGeneral.Controls.Add(label14);
			gBoxGeneral.Controls.Add(nudFrequencyRf);
			gBoxGeneral.Location = new Point(3, 3);
			gBoxGeneral.Name = "gBoxGeneral";
			gBoxGeneral.Size = new Size(793, 102);
			gBoxGeneral.TabIndex = 0;
			gBoxGeneral.TabStop = false;
			gBoxGeneral.Text = "General";
			gBoxGeneral.MouseEnter += control_MouseEnter;
			gBoxGeneral.MouseLeave += control_MouseLeave;
			gBoxOptioanl.Controls.Add(panel10);
			gBoxOptioanl.Controls.Add(label38);
			gBoxOptioanl.Controls.Add(cBoxBand);
			gBoxOptioanl.Controls.Add(panel8);
			gBoxOptioanl.Controls.Add(label36);
			gBoxOptioanl.Controls.Add(label37);
			gBoxOptioanl.Controls.Add(label11);
			gBoxOptioanl.Controls.Add(panel11);
			gBoxOptioanl.Controls.Add(panel5);
			gBoxOptioanl.Controls.Add(label39);
			gBoxOptioanl.Controls.Add(label3);
			gBoxOptioanl.Location = new Point(474, 76);
			gBoxOptioanl.Name = "gBoxOptioanl";
			gBoxOptioanl.Size = new Size(338, 154);
			gBoxOptioanl.TabIndex = 7;
			gBoxOptioanl.TabStop = false;
			gBoxOptioanl.Text = "Optional";
			gBoxOptioanl.Visible = false;
			panel10.AutoSize = true;
			panel10.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel10.Controls.Add(rBtnForceRxBandLowFrequencyOff);
			panel10.Controls.Add(rBtnForceRxBandLowFrequencyOn);
			panel10.Location = new Point(173, 99);
			panel10.Name = "panel10";
			panel10.Size = new Size(102, 23);
			panel10.TabIndex = 9;
			rBtnForceRxBandLowFrequencyOff.AutoSize = true;
			rBtnForceRxBandLowFrequencyOff.Location = new Point(54, 3);
			rBtnForceRxBandLowFrequencyOff.Name = "rBtnForceRxBandLowFrequencyOff";
			rBtnForceRxBandLowFrequencyOff.Size = new Size(45, 17);
			rBtnForceRxBandLowFrequencyOff.TabIndex = 1;
			rBtnForceRxBandLowFrequencyOff.Text = "OFF";
			rBtnForceRxBandLowFrequencyOff.UseVisualStyleBackColor = true;
			rBtnForceRxBandLowFrequencyOff.CheckedChanged += rBtnForceRxBandLowFrequency_CheckedChanged;
			rBtnForceRxBandLowFrequencyOn.AutoSize = true;
			rBtnForceRxBandLowFrequencyOn.Checked = true;
			rBtnForceRxBandLowFrequencyOn.Location = new Point(3, 3);
			rBtnForceRxBandLowFrequencyOn.Name = "rBtnForceRxBandLowFrequencyOn";
			rBtnForceRxBandLowFrequencyOn.Size = new Size(41, 17);
			rBtnForceRxBandLowFrequencyOn.TabIndex = 0;
			rBtnForceRxBandLowFrequencyOn.TabStop = true;
			rBtnForceRxBandLowFrequencyOn.Text = "ON";
			rBtnForceRxBandLowFrequencyOn.UseVisualStyleBackColor = true;
			rBtnForceRxBandLowFrequencyOn.CheckedChanged += rBtnForceRxBandLowFrequency_CheckedChanged;
			label38.AutoSize = true;
			label38.Location = new Point(7, 104);
			label38.Name = "label38";
			label38.Size = new Size(149, 13);
			label38.TabIndex = 8;
			label38.Text = "Force Rx band low frequency:";
			cBoxBand.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxBand.FormattingEnabled = true;
			cBoxBand.Items.AddRange(["Auto", "820-1024", "410-525", "137-175"]);
			cBoxBand.Location = new Point(173, 22);
			cBoxBand.Name = "cBoxBand";
			cBoxBand.Size = new Size(124, 21);
			cBoxBand.TabIndex = 25;
			cBoxBand.SelectedIndexChanged += cBoxBand_SelectedIndexChanged;
			panel8.AutoSize = true;
			panel8.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel8.Controls.Add(rBtnLowFrequencyModeOff);
			panel8.Controls.Add(rBtnLowFrequencyModeOn);
			panel8.Location = new Point(173, 123);
			panel8.Name = "panel8";
			panel8.Size = new Size(102, 23);
			panel8.TabIndex = 28;
			rBtnLowFrequencyModeOff.AutoSize = true;
			rBtnLowFrequencyModeOff.Location = new Point(54, 3);
			rBtnLowFrequencyModeOff.Name = "rBtnLowFrequencyModeOff";
			rBtnLowFrequencyModeOff.Size = new Size(45, 17);
			rBtnLowFrequencyModeOff.TabIndex = 1;
			rBtnLowFrequencyModeOff.Text = "OFF";
			rBtnLowFrequencyModeOff.UseVisualStyleBackColor = true;
			rBtnLowFrequencyModeOff.CheckedChanged += rBtnLowFrequencyMode_CheckedChanged;
			rBtnLowFrequencyModeOn.AutoSize = true;
			rBtnLowFrequencyModeOn.Checked = true;
			rBtnLowFrequencyModeOn.Location = new Point(3, 3);
			rBtnLowFrequencyModeOn.Name = "rBtnLowFrequencyModeOn";
			rBtnLowFrequencyModeOn.Size = new Size(41, 17);
			rBtnLowFrequencyModeOn.TabIndex = 0;
			rBtnLowFrequencyModeOn.TabStop = true;
			rBtnLowFrequencyModeOn.Text = "ON";
			rBtnLowFrequencyModeOn.UseVisualStyleBackColor = true;
			rBtnLowFrequencyModeOn.CheckedChanged += rBtnLowFrequencyMode_CheckedChanged;
			label36.AutoSize = true;
			label36.Location = new Point(303, 26);
			label36.Name = "label36";
			label36.Size = new Size(29, 13);
			label36.TabIndex = 26;
			label36.Text = "MHz";
			label37.AutoSize = true;
			label37.Location = new Point(7, 128);
			label37.Name = "label37";
			label37.Size = new Size(109, 13);
			label37.TabIndex = 27;
			label37.Text = "Low frequency mode:";
			label11.AutoSize = true;
			label11.Location = new Point(15, 25);
			label11.Name = "label11";
			label11.Size = new Size(35, 13);
			label11.TabIndex = 24;
			label11.Text = "Band:";
			panel11.AutoSize = true;
			panel11.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel11.Controls.Add(rBtnForceTxBandLowFrequencyOff);
			panel11.Controls.Add(rBtnForceTxBandLowFrequencyOn);
			panel11.Location = new Point(173, 49);
			panel11.Name = "panel11";
			panel11.Size = new Size(102, 23);
			panel11.TabIndex = 10;
			rBtnForceTxBandLowFrequencyOff.AutoSize = true;
			rBtnForceTxBandLowFrequencyOff.Location = new Point(54, 3);
			rBtnForceTxBandLowFrequencyOff.Name = "rBtnForceTxBandLowFrequencyOff";
			rBtnForceTxBandLowFrequencyOff.Size = new Size(45, 17);
			rBtnForceTxBandLowFrequencyOff.TabIndex = 1;
			rBtnForceTxBandLowFrequencyOff.Text = "OFF";
			rBtnForceTxBandLowFrequencyOff.UseVisualStyleBackColor = true;
			rBtnForceTxBandLowFrequencyOff.CheckedChanged += rBtnForceTxBandLowFrequency_CheckedChanged;
			rBtnForceTxBandLowFrequencyOn.AutoSize = true;
			rBtnForceTxBandLowFrequencyOn.Checked = true;
			rBtnForceTxBandLowFrequencyOn.Location = new Point(3, 3);
			rBtnForceTxBandLowFrequencyOn.Name = "rBtnForceTxBandLowFrequencyOn";
			rBtnForceTxBandLowFrequencyOn.Size = new Size(41, 17);
			rBtnForceTxBandLowFrequencyOn.TabIndex = 0;
			rBtnForceTxBandLowFrequencyOn.TabStop = true;
			rBtnForceTxBandLowFrequencyOn.Text = "ON";
			rBtnForceTxBandLowFrequencyOn.UseVisualStyleBackColor = true;
			rBtnForceTxBandLowFrequencyOn.CheckedChanged += rBtnForceTxBandLowFrequency_CheckedChanged;
			panel5.AutoSize = true;
			panel5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel5.Controls.Add(rBtnFastHopOff);
			panel5.Controls.Add(rBtnFastHopOn);
			panel5.Location = new Point(173, 76);
			panel5.Name = "panel5";
			panel5.Size = new Size(98, 17);
			panel5.TabIndex = 23;
			rBtnFastHopOff.AutoSize = true;
			rBtnFastHopOff.Location = new Point(50, 0);
			rBtnFastHopOff.Margin = new Padding(3, 0, 3, 0);
			rBtnFastHopOff.Name = "rBtnFastHopOff";
			rBtnFastHopOff.Size = new Size(45, 17);
			rBtnFastHopOff.TabIndex = 1;
			rBtnFastHopOff.Text = "OFF";
			rBtnFastHopOff.UseVisualStyleBackColor = true;
			rBtnFastHopOff.CheckedChanged += rBtnFastHop_CheckedChanged;
			rBtnFastHopOn.AutoSize = true;
			rBtnFastHopOn.Checked = true;
			rBtnFastHopOn.Location = new Point(3, 0);
			rBtnFastHopOn.Margin = new Padding(3, 0, 3, 0);
			rBtnFastHopOn.Name = "rBtnFastHopOn";
			rBtnFastHopOn.Size = new Size(41, 17);
			rBtnFastHopOn.TabIndex = 0;
			rBtnFastHopOn.TabStop = true;
			rBtnFastHopOn.Text = "ON";
			rBtnFastHopOn.UseVisualStyleBackColor = true;
			rBtnFastHopOn.CheckedChanged += rBtnFastHop_CheckedChanged;
			label39.AutoSize = true;
			label39.Location = new Point(15, 54);
			label39.Name = "label39";
			label39.Size = new Size(148, 13);
			label39.TabIndex = 7;
			label39.Text = "Force Tx band low frequency:";
			label3.AutoSize = true;
			label3.Location = new Point(15, 78);
			label3.Name = "label3";
			label3.Size = new Size(71, 13);
			label3.TabIndex = 22;
			label3.Text = "Fast hopping:";
			panel1.AutoSize = true;
			panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel1.Controls.Add(rBtnTcxoInputOff);
			panel1.Controls.Add(rBtnTcxoInputOn);
			panel1.Location = new Point(545, 54);
			panel1.Name = "panel1";
			panel1.Size = new Size(122, 20);
			panel1.TabIndex = 1;
			rBtnTcxoInputOff.AutoSize = true;
			rBtnTcxoInputOff.Location = new Point(63, 3);
			rBtnTcxoInputOff.Margin = new Padding(3, 0, 3, 0);
			rBtnTcxoInputOff.Name = "rBtnTcxoInputOff";
			rBtnTcxoInputOff.Size = new Size(56, 17);
			rBtnTcxoInputOff.TabIndex = 1;
			rBtnTcxoInputOff.Text = "Crystal";
			rBtnTcxoInputOff.UseVisualStyleBackColor = true;
			rBtnTcxoInputOff.CheckedChanged += rBtnTcxoInput_CheckedChanged;
			rBtnTcxoInputOn.AutoSize = true;
			rBtnTcxoInputOn.Checked = true;
			rBtnTcxoInputOn.Location = new Point(3, 3);
			rBtnTcxoInputOn.Margin = new Padding(3, 0, 3, 0);
			rBtnTcxoInputOn.Name = "rBtnTcxoInputOn";
			rBtnTcxoInputOn.Size = new Size(54, 17);
			rBtnTcxoInputOn.TabIndex = 0;
			rBtnTcxoInputOn.TabStop = true;
			rBtnTcxoInputOn.Text = "TCXO";
			rBtnTcxoInputOn.UseVisualStyleBackColor = true;
			rBtnTcxoInputOn.CheckedChanged += rBtnTcxoInput_CheckedChanged;
			nudFrequencyXo.Location = new Point(545, 28);
			nudFrequencyXo.Maximum = new decimal([32000000, 0, 0, 0]);
			nudFrequencyXo.Minimum = new decimal([26000000, 0, 0, 0]);
			nudFrequencyXo.Name = "nudFrequencyXo";
			nudFrequencyXo.Size = new Size(124, 20);
			nudFrequencyXo.TabIndex = 1;
			nudFrequencyXo.ThousandsSeparator = true;
			nudFrequencyXo.Value = new decimal([32000000, 0, 0, 0]);
			nudFrequencyXo.ValueChanged += nudFrequencyXo_ValueChanged;
			label9.AutoSize = true;
			label9.Location = new Point(675, 32);
			label9.Name = "label9";
			label9.Size = new Size(20, 13);
			label9.TabIndex = 2;
			label9.Text = "Hz";
			label1.AutoSize = true;
			label1.Location = new Point(379, 32);
			label1.Name = "label1";
			label1.Size = new Size(78, 13);
			label1.TabIndex = 0;
			label1.Text = "XO Frequency:";
			label13.AutoSize = true;
			label13.Location = new Point(297, 45);
			label13.Name = "label13";
			label13.Size = new Size(20, 13);
			label13.TabIndex = 2;
			label13.Text = "Hz";
			lblRcOscillatorCalStat.AutoSize = true;
			lblRcOscillatorCalStat.Location = new Point(379, 58);
			lblRcOscillatorCalStat.Name = "lblRcOscillatorCalStat";
			lblRcOscillatorCalStat.Size = new Size(96, 13);
			lblRcOscillatorCalStat.TabIndex = 5;
			lblRcOscillatorCalStat.Text = "XO input selection:";
			label14.AutoSize = true;
			label14.Location = new Point(9, 45);
			label14.Name = "label14";
			label14.Size = new Size(74, 13);
			label14.TabIndex = 0;
			label14.Text = "RF frequency:";
			errorProvider.SetIconPadding(nudFrequencyRf, 30);
			nudFrequencyRf.Increment = new decimal([61, 0, 0, 0]);
			nudFrequencyRf.Location = new Point(167, 41);
			nudFrequencyRf.Maximum = new decimal([2040000000, 0, 0, 0]);
			nudFrequencyRf.Minimum = new decimal([100000000, 0, 0, 0]);
			nudFrequencyRf.Name = "nudFrequencyRf";
			nudFrequencyRf.Size = new Size(124, 20);
			nudFrequencyRf.TabIndex = 1;
			nudFrequencyRf.ThousandsSeparator = true;
			nudFrequencyRf.Value = new decimal([915000000, 0, 0, 0]);
			nudFrequencyRf.ValueChanged += nudFrequencyRf_ValueChanged;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(groupBox1);
			Controls.Add(gBoxRxSettings);
			Controls.Add(gBoxAgc);
			Controls.Add(gBoxTxSettings);
			Controls.Add(gBoxGeneral);
			Name = "CommonViewControl";
			Size = new Size(799, 493);
			((ISupportInitialize)errorProvider).EndInit();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			gBoxRxSettings.ResumeLayout(false);
			gBoxRxSettings.PerformLayout();
			panel12.ResumeLayout(false);
			panel12.PerformLayout();
			panel9.ResumeLayout(false);
			panel9.PerformLayout();
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			panel2.ResumeLayout(false);
			panel2.PerformLayout();
			gBoxAgc.ResumeLayout(false);
			gBoxAgc.PerformLayout();
			((ISupportInitialize)nudAgcStep5).EndInit();
			((ISupportInitialize)nudAgcStep4).EndInit();
			((ISupportInitialize)nudAgcReferenceLevel).EndInit();
			((ISupportInitialize)nudAgcStep3).EndInit();
			((ISupportInitialize)nudAgcStep1).EndInit();
			((ISupportInitialize)nudAgcStep2).EndInit();
			gBoxTxSettings.ResumeLayout(false);
			gBoxTxSettings.PerformLayout();
			pnlPa20dBm.ResumeLayout(false);
			pnlPa20dBm.PerformLayout();
			((ISupportInitialize)nudMaxOutputPower).EndInit();
			((ISupportInitialize)nudPllBandwidth).EndInit();
			panel4.ResumeLayout(false);
			panel4.PerformLayout();
			((ISupportInitialize)nudOcpTrim).EndInit();
			pnlPaSelect.ResumeLayout(false);
			pnlPaSelect.PerformLayout();
			((ISupportInitialize)nudOutputPower).EndInit();
			gBoxGeneral.ResumeLayout(false);
			gBoxGeneral.PerformLayout();
			gBoxOptioanl.ResumeLayout(false);
			gBoxOptioanl.PerformLayout();
			panel10.ResumeLayout(false);
			panel10.PerformLayout();
			panel8.ResumeLayout(false);
			panel8.PerformLayout();
			panel11.ResumeLayout(false);
			panel11.PerformLayout();
			panel5.ResumeLayout(false);
			panel5.PerformLayout();
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			((ISupportInitialize)nudFrequencyXo).EndInit();
			((ISupportInitialize)nudFrequencyRf).EndInit();
			ResumeLayout(false);
		}
	}
}
