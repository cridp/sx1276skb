using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using SemtechLib.Controls;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.Devices.SX1276.Events;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;

namespace SemtechLib.Devices.SX1276.UI.Controls
{
	public sealed class ReceiverViewControl : UserControl, INotifyDocumentationChanged
	{
		private IContainer components;

		private ErrorProvider errorProvider;

		private Label suffixOOKfixed;

		private Label suffixOOKstep;

		private Label suffixAFCRxBw;

		private Label suffixRxBw;

		private Label lblOokFixed;

		private Label lblOokCutoff;

		private Label lblOokDec;

		private Label lblOokStep;

		private Label lblOokType;

		private Label lblAfcRxBw;

		private Label lblRxBw;

		private NumericUpDownEx nudRxFilterBw;

		private NumericUpDownEx nudRxFilterBwAfc;

		private Panel panel5;

		private RadioButton rBtnAgcAutoOff;

		private RadioButton rBtnAgcAutoOn;

		private Label lblAgcThresh5;

		private Label lblAgcThresh4;

		private Label lblAgcThresh3;

		private Label lblAgcThresh2;

		private Label lblAgcThresh1;

		private Label lblLnaGain6;

		private Label lblLnaGain5;

		private Label lblLnaGain4;

		private Label lblLnaGain3;

		private Label lblLnaGain2;

		private Label lblLnaGain1;

		private Label lblAgcReference;

		private Panel panel6;

		private RadioButton rBtnLnaGain1;

		private RadioButton rBtnLnaGain2;

		private RadioButton rBtnLnaGain3;

		private RadioButton rBtnLnaGain4;

		private RadioButton rBtnLnaGain5;

		private RadioButton rBtnLnaGain6;

		private Label label47;

		private Label label53;

		private Label label52;

		private Label label51;

		private Label label50;

		private Label label49;

		private Label label48;

		private Label label54;

		private Label label55;

		private Label lblRssiValue;

		private Label label56;

		private Label label13;

		private NumericUpDownEx nudRssiThresh;

		private ComboBox cBoxOokThreshType;

		private NumericUpDownEx nudOokPeakThreshStep;

		private ComboBox cBoxOokAverageThreshFilt;

		private ComboBox cBoxOokPeakThreshDec;

		private NumericUpDownEx nudOokFixedThresh;

		private Label label10;

		private Label lblFeiValue;

		private Button btnAfcClear;

		private Label lblAfcValue;

		private Panel panel9;

		private RadioButton rBtnAfcAutoOff;

		private RadioButton rBtnAfcAutoOn;

		private Panel panel8;

		private RadioButton rBtnAfcAutoClearOff;

		private RadioButton rBtnAfcAutoClearOn;

		private Label label20;

		private Label label19;

		private Button btnFeiRead;

		private Label label22;

		private Label label12;

		private NumericUpDownEx nudTimeoutPreamble;

		private NumericUpDownEx nudAutoRxRestartDelay;

		private Label label15;

		private Label label11;

		private Label label14;

		private Label label9;

		private GroupBoxEx gBoxRssi;

		private GroupBoxEx gBoxAfc;

		private GroupBoxEx gBoxDemodulator;

		private GroupBoxEx gBoxRxBw;

		private GroupBoxEx gBoxLnaSettings;

		private Label label17;

		private Label label18;

		private Button btnRestartRxWithoutPllLock;

		private Label label2;

		private Label label5;

		private Panel panel4;

		private RadioButton rBtnRestartRxOnCollisionOff;

		private RadioButton rBtnRestartRxOnCollisionOn;

		private NumericUpDownEx nudRssiSmoothing;

		private Label label25;

		private NumericUpDownEx nudRssiOffset;

		private Label label24;

		private NumericUpDownEx nudRssiCollisionThreshold;

		private Label label26;

		private Label label28;

		private Panel panel13;

		private RadioButton rBtnBitSyncOff;

		private RadioButton rBtnBitSyncOn;

		private NumericUpDownEx nudOokAverageOffset;

		private Label label30;

		private GroupBoxEx gBoxTimeout;

		private NumericUpDownEx nudTimeoutSyncWord;

		private Label label35;

		private Label label27;

		private NumericUpDownEx nudTimeoutRssi;

		private Label label37;

		private Label label36;

		private Button btnRestartRxWithPllLock;

		private GroupBoxEx gBoxRxConfig;

		private Label label16;

		private Label label23;

		private Label label38;

		private Label label39;

		private Button btnAgcStart;

		private Label label40;

		private GroupBoxEx gBoxPreamble;

		private NumericUpDownEx nudPreambleDetectorTol;

		private Label label41;

		private NumericUpDownEx nudPreambleDetectorSize;

		private Label label44;

		private Panel panel1;

		private RadioButton rBtnPreambleDetectorOff;

		private RadioButton rBtnPreambleDetectorOn;

		private Label label57;

		private Label label58;

		private Label label42;

		private Panel pnlHorizontalSeparator;

		private Label label1;

		private Panel panel2;

		private GroupBoxEx gBoxAgc;

		private Label label4;

		private Label label6;

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

		private Panel panel3;

		private RadioButton rBtnLnaBoostOff;

		private RadioButton rBtnLnaBoostOn;

		private ComboBox cBoxRxTrigger;

		private Label label3;
        private decimal bitRate = 4800m;

		private bool lowFrequencyModeOn = true;

		private int agcReference = 19;

		private int agcThresh1;

		private int agcThresh2;

		private int agcThresh3;

		private int agcThresh4;

		private int agcThresh5;

		private decimal rssiSmoothing = 16m;

		private decimal rssiValue = -127.5m;

		private decimal rxBw = 10417m;

		private decimal afcRxBw = 50000m;

		private decimal ookPeakThreshStep = 0.5m;

		private decimal afcValue; // = 0.0m;

		private decimal feiValue; // = 0.0m;

        public decimal FrequencyXo { get; set; } = 32000000m;

        public DataModeEnum DataMode { get; set; } = DataModeEnum.Packet;

        public ModulationTypeEnum ModulationType { get; set; }

        public decimal Bitrate
		{
			get => bitRate;
			set
			{
				if (bitRate == value)
				{
					return;
				}
				var ookAverageThreshFilt = (int)OokAverageThreshFilt;
				cBoxOokAverageThreshFilt.Items.Clear();
				for (var num = 32; num >= 2; num /= 2)
				{
					if (num != 16)
					{
						cBoxOokAverageThreshFilt.Items.Add(Math.Round(value / (decimal)(num * Math.PI)).ToString(CultureInfo.CurrentCulture));
					}
				}
				OokAverageThreshFilt = (OokAverageThreshFiltEnum)ookAverageThreshFilt;
				try
				{
					nudTimeoutRssi.ValueChanged -= nudTimeoutRssi_ValueChanged;
					decimal num2 = (uint)Math.Round(nudTimeoutRssi.Value / 1000m / (16m / bitRate), MidpointRounding.AwayFromZero);
					nudTimeoutRssi.Maximum = 255m * (16m / value) * 1000m;
					nudTimeoutRssi.Increment = nudTimeoutRssi.Maximum / 255m;
					nudTimeoutRssi.Value = num2 * (16m / value) * 1000m;
				}
				catch (Exception)
				{
				}
				finally
				{
					nudTimeoutRssi.ValueChanged += nudTimeoutRssi_ValueChanged;
				}
				try
				{
					nudTimeoutPreamble.ValueChanged -= nudTimeoutPreamble_ValueChanged;
					decimal num3 = (uint)Math.Round(nudTimeoutPreamble.Value / 1000m / (16m / bitRate), MidpointRounding.AwayFromZero);
					nudTimeoutPreamble.Maximum = 255m * (16m / value) * 1000m;
					nudTimeoutPreamble.Increment = nudTimeoutPreamble.Maximum / 255m;
					nudTimeoutPreamble.Value = num3 * (16m / value) * 1000m;
				}
				catch (Exception)
				{
				}
				finally
				{
					nudTimeoutPreamble.ValueChanged += nudTimeoutPreamble_ValueChanged;
				}
				try
				{
					nudTimeoutSyncWord.ValueChanged -= nudTimeoutSyncWord_ValueChanged;
					decimal num4 = (uint)Math.Round(nudTimeoutSyncWord.Value / 1000m / (16m / bitRate), MidpointRounding.AwayFromZero);
					nudTimeoutSyncWord.Maximum = 255m * (16m / value) * 1000m;
					nudTimeoutSyncWord.Increment = nudTimeoutSyncWord.Maximum / 255m;
					nudTimeoutSyncWord.Value = num4 * (16m / value) * 1000m;
				}
				catch
				{
				}
				finally
				{
					nudTimeoutSyncWord.ValueChanged += nudTimeoutSyncWord_ValueChanged;
				}
				try
				{
					nudAutoRxRestartDelay.ValueChanged -= nudAutoRxRestartDelay_ValueChanged;
					decimal num5 = (uint)Math.Round(nudAutoRxRestartDelay.Value / 1000m / (4m / bitRate), MidpointRounding.AwayFromZero);
					nudAutoRxRestartDelay.Maximum = 255m * (4m / value) * 1000m;
					nudAutoRxRestartDelay.Increment = nudAutoRxRestartDelay.Maximum / 255m;
					nudAutoRxRestartDelay.Value = num5 * (4m / value) * 1000m;
				}
				catch
				{
				}
				finally
				{
					nudAutoRxRestartDelay.ValueChanged += nudAutoRxRestartDelay_ValueChanged;
				}
				bitRate = value;
			}
		}

		public bool LowFrequencyModeOn
		{
			get => lowFrequencyModeOn;
			set
			{
				if (value)
				{
					rBtnLnaBoostOn.Enabled = false;
					rBtnLnaBoostOff.Enabled = false;
				}
				else
				{
					rBtnLnaBoostOn.Enabled = true;
					rBtnLnaBoostOff.Enabled = true;
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

		public decimal RxBwMin
		{
			get => nudRxFilterBw.Minimum;
			set => nudRxFilterBw.Minimum = value;
		}

		public decimal RxBwMax
		{
			get => nudRxFilterBw.Maximum;
			set => nudRxFilterBw.Maximum = value;
		}

		public decimal AfcRxBwMin
		{
			get => nudRxFilterBwAfc.Minimum;
			set => nudRxFilterBwAfc.Minimum = value;
		}

		public decimal AfcRxBwMax
		{
			get => nudRxFilterBwAfc.Maximum;
			set => nudRxFilterBwAfc.Maximum = value;
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

		public bool RestartRxOnCollision
		{
			get => rBtnRestartRxOnCollisionOn.Checked;
			set
			{
				rBtnRestartRxOnCollisionOn.CheckedChanged -= rBtnRestartRxOnCollisionOn_CheckedChanged;
				rBtnRestartRxOnCollisionOff.CheckedChanged -= rBtnRestartRxOnCollisionOn_CheckedChanged;
				if (value)
				{
					rBtnRestartRxOnCollisionOn.Checked = true;
					rBtnRestartRxOnCollisionOff.Checked = false;
				}
				else
				{
					rBtnRestartRxOnCollisionOn.Checked = false;
					rBtnRestartRxOnCollisionOff.Checked = true;
				}
				rBtnRestartRxOnCollisionOn.CheckedChanged += rBtnRestartRxOnCollisionOn_CheckedChanged;
				rBtnRestartRxOnCollisionOff.CheckedChanged += rBtnRestartRxOnCollisionOn_CheckedChanged;
			}
		}

		public bool AfcAutoOn
		{
			get => rBtnAfcAutoOn.Checked;
			set
			{
				rBtnAfcAutoOn.CheckedChanged -= rBtnAfcAutoOn_CheckedChanged;
				rBtnAfcAutoOff.CheckedChanged -= rBtnAfcAutoOn_CheckedChanged;
				if (value)
				{
					rBtnAfcAutoOn.Checked = true;
					rBtnAfcAutoOff.Checked = false;
				}
				else
				{
					rBtnAfcAutoOn.Checked = false;
					rBtnAfcAutoOff.Checked = true;
				}
				rBtnAfcAutoOn.CheckedChanged += rBtnAfcAutoOn_CheckedChanged;
				rBtnAfcAutoOff.CheckedChanged += rBtnAfcAutoOn_CheckedChanged;
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
					btnAgcStart.Enabled = true;
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
					btnAgcStart.Enabled = false;
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

		public RxTriggerEnum RxTrigger
		{
			get => (RxTriggerEnum)cBoxRxTrigger.SelectedIndex;
			set
			{
				cBoxRxTrigger.SelectedIndexChanged -= cBoxRxTrigger_SelectedIndexChanged;
				cBoxRxTrigger.SelectedIndex = (int)value;
				cBoxRxTrigger.SelectedIndexChanged += cBoxRxTrigger_SelectedIndexChanged;
			}
		}

		public decimal RssiOffset
		{
			get => nudRssiOffset.Value;
			set
			{
				try
				{
					nudRssiOffset.ValueChanged -= nudRssiOffset_ValueChanged;
					nudRssiOffset.Value = value;
				}
				finally
				{
					nudRssiOffset.ValueChanged += nudRssiOffset_ValueChanged;
				}
			}
		}

		public decimal RssiSmoothing
		{
			get => rssiSmoothing;
			set
			{
				try
				{
					nudRssiSmoothing.ValueChanged -= nudRssiSmoothing_ValueChanged;
					var num = (ushort)Math.Log((double)value, 2.0);
					rssiSmoothing = (decimal)Math.Pow(2.0, num);
					nudRssiSmoothing.Value = rssiSmoothing;
				}
				finally
				{
					nudRssiSmoothing.ValueChanged += nudRssiSmoothing_ValueChanged;
				}
			}
		}

		public decimal RssiCollisionThreshold
		{
			get => nudRssiCollisionThreshold.Value;
			set
			{
				try
				{
					nudRssiCollisionThreshold.ValueChanged -= nudRssiCollisionThreshold_ValueChanged;
					nudRssiCollisionThreshold.Value = value;
				}
				finally
				{
					nudRssiCollisionThreshold.ValueChanged += nudRssiCollisionThreshold_ValueChanged;
				}
			}
		}

		public decimal RssiThreshold
		{
			get => nudRssiThresh.Value;
			set
			{
				try
				{
					nudRssiThresh.ValueChanged -= nudRssiThresh_ValueChanged;
					nudRssiThresh.Value = value;
					nudRssiThresh.ValueChanged += nudRssiThresh_ValueChanged;
				}
				catch (Exception)
				{
					nudRssiThresh.ValueChanged += nudRssiThresh_ValueChanged;
				}
			}
		}

		public decimal RssiValue
		{
			get => rssiValue;
			set
			{
				rssiValue = value;
				lblRssiValue.Text = value.ToString("###.0");
			}
		}

		public decimal RxBw
		{
			get => rxBw;
			set
			{
				try
				{
					nudRxFilterBw.ValueChanged -= nudRxFilterBw_ValueChanged;
					var mant = 0;
					var exp = 0;
					SX1276.ComputeRxBwMantExp(FrequencyXo, ModulationType, value, ref mant, ref exp);
					rxBw = SX1276.ComputeRxBw(FrequencyXo, ModulationType, mant, exp);
					nudRxFilterBw.Value = rxBw;
					nudRxFilterBw.ValueChanged += nudRxFilterBw_ValueChanged;
				}
				catch (Exception)
				{
					nudRxFilterBw.ValueChanged += nudRxFilterBw_ValueChanged;
				}
			}
		}

		public decimal AfcRxBw
		{
			get => afcRxBw;
			set
			{
				try
				{
					nudRxFilterBwAfc.ValueChanged -= nudRxFilterBwAfc_ValueChanged;
					var mant = 0;
					var exp = 0;
					SX1276.ComputeRxBwMantExp(FrequencyXo, ModulationType, value, ref mant, ref exp);
					afcRxBw = SX1276.ComputeRxBw(FrequencyXo, ModulationType, mant, exp);
					nudRxFilterBwAfc.Value = afcRxBw;
					nudRxFilterBwAfc.ValueChanged += nudRxFilterBwAfc_ValueChanged;
				}
				catch (Exception)
				{
					nudRxFilterBwAfc.ValueChanged += nudRxFilterBwAfc_ValueChanged;
				}
			}
		}

		public bool BitSyncOn
		{
			get => rBtnBitSyncOn.Checked;
			set
			{
				rBtnBitSyncOn.CheckedChanged -= rBtnBitSyncOn_CheckedChanged;
				rBtnBitSyncOff.CheckedChanged -= rBtnBitSyncOn_CheckedChanged;
				if (value)
				{
					rBtnBitSyncOn.Checked = true;
					rBtnBitSyncOff.Checked = false;
				}
				else
				{
					rBtnBitSyncOn.Checked = false;
					rBtnBitSyncOff.Checked = true;
				}
				rBtnBitSyncOn.CheckedChanged += rBtnBitSyncOn_CheckedChanged;
				rBtnBitSyncOff.CheckedChanged += rBtnBitSyncOn_CheckedChanged;
			}
		}

		public OokThreshTypeEnum OokThreshType
		{
			get => (OokThreshTypeEnum)cBoxOokThreshType.SelectedIndex;
			set
			{
				cBoxOokThreshType.SelectedIndexChanged -= cBoxOokThreshType_SelectedIndexChanged;
				switch (value)
				{
				case OokThreshTypeEnum.Fixed:
					cBoxOokThreshType.SelectedIndex = 0;
					nudOokPeakThreshStep.Enabled = false;
					cBoxOokPeakThreshDec.Enabled = false;
					cBoxOokAverageThreshFilt.Enabled = false;
					nudOokFixedThresh.Enabled = true;
					break;
				case OokThreshTypeEnum.Peak:
					cBoxOokThreshType.SelectedIndex = 1;
					nudOokPeakThreshStep.Enabled = true;
					cBoxOokPeakThreshDec.Enabled = true;
					cBoxOokAverageThreshFilt.Enabled = false;
					nudOokFixedThresh.Enabled = true;
					break;
				case OokThreshTypeEnum.Average:
					cBoxOokThreshType.SelectedIndex = 2;
					nudOokPeakThreshStep.Enabled = false;
					cBoxOokPeakThreshDec.Enabled = false;
					cBoxOokAverageThreshFilt.Enabled = true;
					nudOokFixedThresh.Enabled = false;
					break;
				default:
					cBoxOokThreshType.SelectedIndex = -1;
					break;
				}
				cBoxOokThreshType.SelectedIndexChanged += cBoxOokThreshType_SelectedIndexChanged;
			}
		}

		public decimal OokPeakThreshStep
		{
			get => ookPeakThreshStep;
			set
			{
				try
				{
					nudOokPeakThreshStep.ValueChanged -= nudOokPeakThreshStep_ValueChanged;
					var array = new decimal[8] { 0.5m, 1.0m, 1.5m, 2.0m, 3.0m, 4.0m, 5.0m, 6.0m };
					var num = Array.IndexOf(array, value);
					ookPeakThreshStep = array[num];
					nudOokPeakThreshStep.Value = ookPeakThreshStep;
					nudOokPeakThreshStep.ValueChanged += nudOokPeakThreshStep_ValueChanged;
				}
				catch (Exception)
				{
					nudOokPeakThreshStep.ValueChanged += nudOokPeakThreshStep_ValueChanged;
				}
			}
		}

		public OokPeakThreshDecEnum OokPeakThreshDec
		{
			get => (OokPeakThreshDecEnum)cBoxOokPeakThreshDec.SelectedIndex;
			set
			{
				cBoxOokPeakThreshDec.SelectedIndexChanged -= cBoxOokPeakThreshDec_SelectedIndexChanged;
				cBoxOokPeakThreshDec.SelectedIndex = (int)value;
				cBoxOokPeakThreshDec.SelectedIndexChanged += cBoxOokPeakThreshDec_SelectedIndexChanged;
			}
		}

		public decimal OokAverageOffset
		{
			get => nudOokAverageOffset.Value;
			set
			{
				try
				{
					nudOokAverageOffset.ValueChanged -= nudOokAverageOffset_ValueChanged;
					nudOokAverageOffset.Value = value;
				}
				finally
				{
					nudOokAverageOffset.ValueChanged += nudOokAverageOffset_ValueChanged;
				}
			}
		}

		public OokAverageThreshFiltEnum OokAverageThreshFilt
		{
			get => (OokAverageThreshFiltEnum)cBoxOokAverageThreshFilt.SelectedIndex;
			set
			{
				cBoxOokAverageThreshFilt.SelectedIndexChanged -= cBoxOokAverageThreshFilt_SelectedIndexChanged;
				cBoxOokAverageThreshFilt.SelectedIndex = (int)value;
				cBoxOokAverageThreshFilt.SelectedIndexChanged += cBoxOokAverageThreshFilt_SelectedIndexChanged;
			}
		}

		public byte OokFixedThreshold
		{
			get => (byte)nudOokFixedThresh.Value;
			set
			{
				try
				{
					nudOokFixedThresh.ValueChanged -= nudOokFixedThresh_ValueChanged;
					nudOokFixedThresh.Value = value;
					nudOokFixedThresh.ValueChanged += nudOokFixedThresh_ValueChanged;
				}
				catch (Exception)
				{
					nudOokFixedThresh.ValueChanged += nudOokFixedThresh_ValueChanged;
				}
			}
		}

		public bool AfcAutoClearOn
		{
			get => rBtnAfcAutoClearOn.Checked;
			set
			{
				rBtnAfcAutoClearOn.CheckedChanged -= rBtnAfcAutoClearOn_CheckedChanged;
				rBtnAfcAutoClearOff.CheckedChanged -= rBtnAfcAutoClearOn_CheckedChanged;
				if (value)
				{
					rBtnAfcAutoClearOn.Checked = true;
					rBtnAfcAutoClearOff.Checked = false;
				}
				else
				{
					rBtnAfcAutoClearOn.Checked = false;
					rBtnAfcAutoClearOff.Checked = true;
				}
				rBtnAfcAutoClearOn.CheckedChanged += rBtnAfcAutoClearOn_CheckedChanged;
				rBtnAfcAutoClearOff.CheckedChanged += rBtnAfcAutoClearOn_CheckedChanged;
			}
		}

		public decimal AfcValue
		{
			get => afcValue;
			set
			{
				afcValue = value;
				lblAfcValue.Text = afcValue.ToString("N0");
			}
		}

		public decimal FeiValue
		{
			get => feiValue;
			set
			{
				feiValue = value;
				lblFeiValue.Text = feiValue.ToString("N0");
			}
		}

		public bool PreambleDetectorOn
		{
			get => rBtnPreambleDetectorOn.Checked;
			set
			{
				rBtnPreambleDetectorOn.CheckedChanged -= rBtnPreambleDetectorOn_CheckedChanged;
				rBtnPreambleDetectorOff.CheckedChanged -= rBtnPreambleDetectorOn_CheckedChanged;
				if (value)
				{
					rBtnPreambleDetectorOn.Checked = true;
					rBtnPreambleDetectorOff.Checked = false;
				}
				else
				{
					rBtnPreambleDetectorOn.Checked = false;
					rBtnPreambleDetectorOff.Checked = true;
				}
				rBtnPreambleDetectorOn.CheckedChanged += rBtnPreambleDetectorOn_CheckedChanged;
				rBtnPreambleDetectorOff.CheckedChanged += rBtnPreambleDetectorOn_CheckedChanged;
			}
		}

		public byte PreambleDetectorSize
		{
			get => (byte)nudPreambleDetectorSize.Value;
			set
			{
				try
				{
					nudPreambleDetectorSize.ValueChanged -= nudPreambleDetectorSize_ValueChanged;
					nudPreambleDetectorSize.Value = value;
				}
				finally
				{
					nudPreambleDetectorSize.ValueChanged += nudPreambleDetectorSize_ValueChanged;
				}
			}
		}

		public byte PreambleDetectorTol
		{
			get => (byte)nudPreambleDetectorTol.Value;
			set
			{
				try
				{
					nudPreambleDetectorTol.ValueChanged -= nudPreambleDetectorTol_ValueChanged;
					nudPreambleDetectorTol.Value = value;
				}
				finally
				{
					nudPreambleDetectorTol.ValueChanged += nudPreambleDetectorTol_ValueChanged;
				}
			}
		}

		public decimal TimeoutRxRssi
		{
			get => nudTimeoutRssi.Value;
			set
			{
				try
				{
					nudTimeoutRssi.ValueChanged -= nudTimeoutRssi_ValueChanged;
					var num = (uint)Math.Round(value / 1000m / (16m / Bitrate), MidpointRounding.AwayFromZero);
					nudTimeoutRssi.Value = num * (16m / Bitrate) * 1000m;
					nudTimeoutRssi.ValueChanged += nudTimeoutRssi_ValueChanged;
				}
				catch (Exception)
				{
					nudTimeoutRssi.ValueChanged += nudTimeoutRssi_ValueChanged;
				}
			}
		}

		public decimal TimeoutRxPreamble
		{
			get => nudTimeoutPreamble.Value;
			set
			{
				try
				{
					nudTimeoutPreamble.ValueChanged -= nudTimeoutPreamble_ValueChanged;
					var num = (uint)Math.Round(value / 1000m / (16m / Bitrate), MidpointRounding.AwayFromZero);
					nudTimeoutPreamble.Value = num * (16m / Bitrate) * 1000m;
					nudTimeoutPreamble.ValueChanged += nudTimeoutPreamble_ValueChanged;
				}
				catch (Exception)
				{
					nudTimeoutPreamble.ValueChanged += nudTimeoutPreamble_ValueChanged;
				}
			}
		}

		public decimal TimeoutSignalSync
		{
			get => nudTimeoutSyncWord.Value;
			set
			{
				try
				{
					nudTimeoutSyncWord.ValueChanged -= nudTimeoutSyncWord_ValueChanged;
					var num = (uint)Math.Round(value / 1000m / (16m / Bitrate), MidpointRounding.AwayFromZero);
					nudTimeoutSyncWord.Value = num * (16m / Bitrate) * 1000m;
					nudTimeoutSyncWord.ValueChanged += nudTimeoutSyncWord_ValueChanged;
				}
				catch (Exception)
				{
					nudTimeoutSyncWord.ValueChanged += nudTimeoutSyncWord_ValueChanged;
				}
			}
		}

		public decimal InterPacketRxDelay
		{
			get => nudAutoRxRestartDelay.Value;
			set
			{
				try
				{
					nudAutoRxRestartDelay.ValueChanged -= nudAutoRxRestartDelay_ValueChanged;
					var num = (uint)Math.Round(value / 1000m / (4m / Bitrate), MidpointRounding.AwayFromZero);
					nudAutoRxRestartDelay.Value = num * (4m / Bitrate) * 1000m;
					nudAutoRxRestartDelay.ValueChanged += nudAutoRxRestartDelay_ValueChanged;
				}
				catch (Exception)
				{
					nudAutoRxRestartDelay.ValueChanged += nudAutoRxRestartDelay_ValueChanged;
				}
			}
		}

		public event Int32EventHandler AgcReferenceLevelChanged;

		public event AgcStepEventHandler AgcStepChanged;

		public event LnaGainEventHandler LnaGainChanged;

		public event BooleanEventHandler LnaBoostChanged;

		public event BooleanEventHandler RestartRxOnCollisionOnChanged;

		public event EventHandler RestartRxWithoutPllLockChanged;

		public event EventHandler RestartRxWithPllLockChanged;

		public event BooleanEventHandler AfcAutoOnChanged;

		public event BooleanEventHandler AgcAutoOnChanged;

		public event RxTriggerEventHandler RxTriggerChanged;

		public event DecimalEventHandler RssiOffsetChanged;

		public event DecimalEventHandler RssiSmoothingChanged;

		public event DecimalEventHandler RssiCollisionThresholdChanged;

		public event DecimalEventHandler RssiThreshChanged;

		public event DecimalEventHandler RxBwChanged;

		public event DecimalEventHandler AfcRxBwChanged;

		public event BooleanEventHandler BitSyncOnChanged;

		public event OokThreshTypeEventHandler OokThreshTypeChanged;

		public event DecimalEventHandler OokPeakThreshStepChanged;

		public event OokPeakThreshDecEventHandler OokPeakThreshDecChanged;

		public event OokAverageThreshFiltEventHandler OokAverageThreshFiltChanged;

		public event DecimalEventHandler OokAverageBiasChanged;

		public event ByteEventHandler OokFixedThreshChanged;

		public event EventHandler AgcStartChanged;

		public event EventHandler FeiReadChanged;

		public event BooleanEventHandler AfcAutoClearOnChanged;

		public event EventHandler AfcClearChanged;

		public event BooleanEventHandler PreambleDetectorOnChanged;

		public event ByteEventHandler PreambleDetectorSizeChanged;

		public event ByteEventHandler PreambleDetectorTolChanged;

		public event DecimalEventHandler TimeoutRssiChanged;

		public event DecimalEventHandler TimeoutPreambleChanged;

		public event DecimalEventHandler TimeoutSyncWordChanged;

		public event DecimalEventHandler AutoRxRestartDelayChanged;

		public event DocumentationChangedEventHandler DocumentationChanged;

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
			nudPreambleDetectorTol = new NumericUpDownEx();
			nudPreambleDetectorSize = new NumericUpDownEx();
			cBoxRxTrigger = new ComboBox();
			nudTimeoutRssi = new NumericUpDownEx();
			nudAutoRxRestartDelay = new NumericUpDownEx();
			nudTimeoutSyncWord = new NumericUpDownEx();
			nudTimeoutPreamble = new NumericUpDownEx();
			nudRxFilterBwAfc = new NumericUpDownEx();
			nudRxFilterBw = new NumericUpDownEx();
			nudOokAverageOffset = new NumericUpDownEx();
			cBoxOokThreshType = new ComboBox();
			nudOokPeakThreshStep = new NumericUpDownEx();
			nudOokFixedThresh = new NumericUpDownEx();
			cBoxOokPeakThreshDec = new ComboBox();
			cBoxOokAverageThreshFilt = new ComboBox();
			nudRssiOffset = new NumericUpDownEx();
			nudRssiSmoothing = new NumericUpDownEx();
			nudRssiCollisionThreshold = new NumericUpDownEx();
			lblRssiValue = new Label();
			nudRssiThresh = new NumericUpDownEx();
			panel2 = new Panel();
			gBoxAgc = new GroupBoxEx();
			label4 = new Label();
			label39 = new Label();
			label6 = new Label();
			label29 = new Label();
			label31 = new Label();
			label32 = new Label();
			label16 = new Label();
			label33 = new Label();
			label34 = new Label();
			label46 = new Label();
			btnAgcStart = new Button();
			panel5 = new Panel();
			rBtnAgcAutoOff = new RadioButton();
			rBtnAgcAutoOn = new RadioButton();
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
			gBoxTimeout = new GroupBoxEx();
			label35 = new Label();
			label15 = new Label();
			label37 = new Label();
			label27 = new Label();
			label11 = new Label();
			label36 = new Label();
			label14 = new Label();
			label9 = new Label();
			gBoxPreamble = new GroupBoxEx();
			label41 = new Label();
			label42 = new Label();
			label44 = new Label();
			panel1 = new Panel();
			rBtnPreambleDetectorOff = new RadioButton();
			rBtnPreambleDetectorOn = new RadioButton();
			label57 = new Label();
			label58 = new Label();
			gBoxRxBw = new GroupBoxEx();
			lblAfcRxBw = new Label();
			lblRxBw = new Label();
			suffixAFCRxBw = new Label();
			suffixRxBw = new Label();
			gBoxRxConfig = new GroupBoxEx();
			label38 = new Label();
			label5 = new Label();
			btnRestartRxWithPllLock = new Button();
			label3 = new Label();
			btnRestartRxWithoutPllLock = new Button();
			panel4 = new Panel();
			rBtnRestartRxOnCollisionOff = new RadioButton();
			rBtnRestartRxOnCollisionOn = new RadioButton();
			label26 = new Label();
			gBoxDemodulator = new GroupBoxEx();
			pnlHorizontalSeparator = new Panel();
			label30 = new Label();
			label1 = new Label();
			lblOokType = new Label();
			lblOokStep = new Label();
			label28 = new Label();
			lblOokDec = new Label();
			lblOokCutoff = new Label();
			lblOokFixed = new Label();
			suffixOOKstep = new Label();
			label40 = new Label();
			suffixOOKfixed = new Label();
			panel13 = new Panel();
			rBtnBitSyncOff = new RadioButton();
			rBtnBitSyncOn = new RadioButton();
			gBoxRssi = new GroupBoxEx();
			label23 = new Label();
			label17 = new Label();
			label54 = new Label();
			label24 = new Label();
			label55 = new Label();
			label25 = new Label();
			label56 = new Label();
			gBoxAfc = new GroupBoxEx();
			label19 = new Label();
			btnFeiRead = new Button();
			panel8 = new Panel();
			rBtnAfcAutoClearOff = new RadioButton();
			rBtnAfcAutoClearOn = new RadioButton();
			lblFeiValue = new Label();
			label12 = new Label();
			label20 = new Label();
			label18 = new Label();
			label10 = new Label();
			btnAfcClear = new Button();
			lblAfcValue = new Label();
			label22 = new Label();
			panel9 = new Panel();
			rBtnAfcAutoOff = new RadioButton();
			rBtnAfcAutoOn = new RadioButton();
			gBoxLnaSettings = new GroupBoxEx();
			panel3 = new Panel();
			rBtnLnaBoostOff = new RadioButton();
			rBtnLnaBoostOn = new RadioButton();
			label2 = new Label();
			label13 = new Label();
			lblAgcReference = new Label();
			label48 = new Label();
			label49 = new Label();
			label50 = new Label();
			label51 = new Label();
			label52 = new Label();
			lblLnaGain1 = new Label();
			label53 = new Label();
			panel6 = new Panel();
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
			((ISupportInitialize)errorProvider).BeginInit();
			((ISupportInitialize)nudPreambleDetectorTol).BeginInit();
			((ISupportInitialize)nudPreambleDetectorSize).BeginInit();
			((ISupportInitialize)nudTimeoutRssi).BeginInit();
			((ISupportInitialize)nudAutoRxRestartDelay).BeginInit();
			((ISupportInitialize)nudTimeoutSyncWord).BeginInit();
			((ISupportInitialize)nudTimeoutPreamble).BeginInit();
			((ISupportInitialize)nudRxFilterBwAfc).BeginInit();
			((ISupportInitialize)nudRxFilterBw).BeginInit();
			((ISupportInitialize)nudOokAverageOffset).BeginInit();
			((ISupportInitialize)nudOokPeakThreshStep).BeginInit();
			((ISupportInitialize)nudOokFixedThresh).BeginInit();
			((ISupportInitialize)nudRssiOffset).BeginInit();
			((ISupportInitialize)nudRssiSmoothing).BeginInit();
			((ISupportInitialize)nudRssiCollisionThreshold).BeginInit();
			((ISupportInitialize)nudRssiThresh).BeginInit();
			panel2.SuspendLayout();
			gBoxAgc.SuspendLayout();
			panel5.SuspendLayout();
			((ISupportInitialize)nudAgcStep5).BeginInit();
			((ISupportInitialize)nudAgcStep4).BeginInit();
			((ISupportInitialize)nudAgcReferenceLevel).BeginInit();
			((ISupportInitialize)nudAgcStep3).BeginInit();
			((ISupportInitialize)nudAgcStep1).BeginInit();
			((ISupportInitialize)nudAgcStep2).BeginInit();
			gBoxTimeout.SuspendLayout();
			gBoxPreamble.SuspendLayout();
			panel1.SuspendLayout();
			gBoxRxBw.SuspendLayout();
			gBoxRxConfig.SuspendLayout();
			panel4.SuspendLayout();
			gBoxDemodulator.SuspendLayout();
			panel13.SuspendLayout();
			gBoxRssi.SuspendLayout();
			gBoxAfc.SuspendLayout();
			panel8.SuspendLayout();
			panel9.SuspendLayout();
			gBoxLnaSettings.SuspendLayout();
			panel3.SuspendLayout();
			panel6.SuspendLayout();
			SuspendLayout();
			errorProvider.ContainerControl = this;
			errorProvider.SetIconPadding(nudPreambleDetectorTol, 30);
			nudPreambleDetectorTol.Location = new Point(120, 65);
			nudPreambleDetectorTol.Maximum = new decimal([31, 0, 0, 0]);
			nudPreambleDetectorTol.Name = "nudPreambleDetectorTol";
			nudPreambleDetectorTol.Size = new Size(98, 20);
			nudPreambleDetectorTol.TabIndex = 6;
			nudPreambleDetectorTol.ThousandsSeparator = true;
			nudPreambleDetectorTol.ValueChanged += nudPreambleDetectorTol_ValueChanged;
			errorProvider.SetIconPadding(nudPreambleDetectorSize, 30);
			nudPreambleDetectorSize.Location = new Point(120, 40);
			nudPreambleDetectorSize.Maximum = new decimal([4, 0, 0, 0]);
			nudPreambleDetectorSize.Minimum = new decimal([1, 0, 0, 0]);
			nudPreambleDetectorSize.Name = "nudPreambleDetectorSize";
			nudPreambleDetectorSize.Size = new Size(98, 20);
			nudPreambleDetectorSize.TabIndex = 6;
			nudPreambleDetectorSize.ThousandsSeparator = true;
			nudPreambleDetectorSize.Value = new decimal([1, 0, 0, 0]);
			nudPreambleDetectorSize.ValueChanged += nudPreambleDetectorSize_ValueChanged;
			cBoxRxTrigger.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxRxTrigger.FormattingEnabled = true;
			errorProvider.SetIconPadding(cBoxRxTrigger, 30);
			cBoxRxTrigger.Items.AddRange(["None", "RSSI", "Preamble", "RSSI & Preamble"]);
			cBoxRxTrigger.Location = new Point(125, 97);
			cBoxRxTrigger.Name = "cBoxRxTrigger";
			cBoxRxTrigger.Size = new Size(102, 21);
			cBoxRxTrigger.TabIndex = 22;
			cBoxRxTrigger.SelectedIndexChanged += cBoxRxTrigger_SelectedIndexChanged;
			nudTimeoutRssi.DecimalPlaces = 3;
			errorProvider.SetIconPadding(nudTimeoutRssi, 30);
			nudTimeoutRssi.Location = new Point(125, 19);
			nudTimeoutRssi.Maximum = new decimal([850, 0, 0, 0]);
			nudTimeoutRssi.Name = "nudTimeoutRssi";
			nudTimeoutRssi.Size = new Size(98, 20);
			nudTimeoutRssi.TabIndex = 3;
			nudTimeoutRssi.ThousandsSeparator = true;
			nudTimeoutRssi.ValueChanged += nudTimeoutRssi_ValueChanged;
			nudAutoRxRestartDelay.DecimalPlaces = 3;
			errorProvider.SetIconPadding(nudAutoRxRestartDelay, 30);
			nudAutoRxRestartDelay.Location = new Point(125, 97);
			nudAutoRxRestartDelay.Maximum = new decimal([850, 0, 0, 0]);
			nudAutoRxRestartDelay.Name = "nudAutoRxRestartDelay";
			nudAutoRxRestartDelay.Size = new Size(98, 20);
			nudAutoRxRestartDelay.TabIndex = 3;
			nudAutoRxRestartDelay.ThousandsSeparator = true;
			nudAutoRxRestartDelay.ValueChanged += nudAutoRxRestartDelay_ValueChanged;
			nudTimeoutSyncWord.DecimalPlaces = 3;
			errorProvider.SetIconPadding(nudTimeoutSyncWord, 30);
			nudTimeoutSyncWord.Location = new Point(125, 71);
			nudTimeoutSyncWord.Maximum = new decimal([850, 0, 0, 0]);
			nudTimeoutSyncWord.Name = "nudTimeoutSyncWord";
			nudTimeoutSyncWord.Size = new Size(98, 20);
			nudTimeoutSyncWord.TabIndex = 6;
			nudTimeoutSyncWord.ThousandsSeparator = true;
			nudTimeoutSyncWord.ValueChanged += nudTimeoutSyncWord_ValueChanged;
			nudTimeoutPreamble.DecimalPlaces = 3;
			errorProvider.SetIconPadding(nudTimeoutPreamble, 30);
			nudTimeoutPreamble.Location = new Point(125, 45);
			nudTimeoutPreamble.Maximum = new decimal([850, 0, 0, 0]);
			nudTimeoutPreamble.Name = "nudTimeoutPreamble";
			nudTimeoutPreamble.Size = new Size(98, 20);
			nudTimeoutPreamble.TabIndex = 6;
			nudTimeoutPreamble.ThousandsSeparator = true;
			nudTimeoutPreamble.ValueChanged += nudTimeoutPreamble_ValueChanged;
			errorProvider.SetIconPadding(nudRxFilterBwAfc, 30);
			nudRxFilterBwAfc.Location = new Point(120, 42);
			nudRxFilterBwAfc.Maximum = new decimal([400000, 0, 0, 0]);
			nudRxFilterBwAfc.Minimum = new decimal([3125, 0, 0, 0]);
			nudRxFilterBwAfc.Name = "nudRxFilterBwAfc";
			nudRxFilterBwAfc.Size = new Size(98, 20);
			nudRxFilterBwAfc.TabIndex = 4;
			nudRxFilterBwAfc.ThousandsSeparator = true;
			nudRxFilterBwAfc.Value = new decimal([50000, 0, 0, 0]);
			nudRxFilterBwAfc.ValueChanged += nudRxFilterBwAfc_ValueChanged;
			errorProvider.SetIconPadding(nudRxFilterBw, 25);
			nudRxFilterBw.Location = new Point(120, 17);
			nudRxFilterBw.Maximum = new decimal([500000, 0, 0, 0]);
			nudRxFilterBw.Minimum = new decimal([3906, 0, 0, 0]);
			nudRxFilterBw.Name = "nudRxFilterBw";
			nudRxFilterBw.Size = new Size(98, 20);
			nudRxFilterBw.TabIndex = 4;
			nudRxFilterBw.ThousandsSeparator = true;
			nudRxFilterBw.Value = new decimal([10417, 0, 0, 0]);
			nudRxFilterBw.ValueChanged += nudRxFilterBw_ValueChanged;
			errorProvider.SetIconPadding(nudOokAverageOffset, 30);
			nudOokAverageOffset.Increment = new decimal([2, 0, 0, 0]);
			nudOokAverageOffset.Location = new Point(125, 188);
			nudOokAverageOffset.Maximum = new decimal([6, 0, 0, 0]);
			nudOokAverageOffset.Name = "nudOokAverageOffset";
			nudOokAverageOffset.Size = new Size(98, 20);
			nudOokAverageOffset.TabIndex = 3;
			nudOokAverageOffset.ThousandsSeparator = true;
			nudOokAverageOffset.ValueChanged += nudOokAverageOffset_ValueChanged;
			cBoxOokThreshType.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxOokThreshType.FormattingEnabled = true;
			errorProvider.SetIconPadding(cBoxOokThreshType, 30);
			cBoxOokThreshType.Items.AddRange(["Fixed", "Peak", "Average"]);
			cBoxOokThreshType.Location = new Point(125, 57);
			cBoxOokThreshType.Name = "cBoxOokThreshType";
			cBoxOokThreshType.Size = new Size(102, 21);
			cBoxOokThreshType.TabIndex = 1;
			cBoxOokThreshType.SelectedIndexChanged += cBoxOokThreshType_SelectedIndexChanged;
			nudOokPeakThreshStep.DecimalPlaces = 1;
			errorProvider.SetIconPadding(nudOokPeakThreshStep, 30);
			nudOokPeakThreshStep.Increment = new decimal([5, 0, 0, 65536]);
			nudOokPeakThreshStep.Location = new Point(125, 83);
			nudOokPeakThreshStep.Maximum = new decimal([60, 0, 0, 65536]);
			nudOokPeakThreshStep.Minimum = new decimal([5, 0, 0, 65536]);
			nudOokPeakThreshStep.Name = "nudOokPeakThreshStep";
			nudOokPeakThreshStep.Size = new Size(98, 20);
			nudOokPeakThreshStep.TabIndex = 3;
			nudOokPeakThreshStep.ThousandsSeparator = true;
			nudOokPeakThreshStep.Value = new decimal([5, 0, 0, 65536]);
			nudOokPeakThreshStep.ValueChanged += nudOokPeakThreshStep_ValueChanged;
			nudOokPeakThreshStep.Validating += nudOokPeakThreshStep_Validating;
			errorProvider.SetIconPadding(nudOokFixedThresh, 30);
			nudOokFixedThresh.Location = new Point(125, 109);
			nudOokFixedThresh.Maximum = new decimal([255, 0, 0, 0]);
			nudOokFixedThresh.Name = "nudOokFixedThresh";
			nudOokFixedThresh.Size = new Size(98, 20);
			nudOokFixedThresh.TabIndex = 10;
			nudOokFixedThresh.ThousandsSeparator = true;
			nudOokFixedThresh.Value = new decimal([6, 0, 0, 0]);
			nudOokFixedThresh.ValueChanged += nudOokFixedThresh_ValueChanged;
			cBoxOokPeakThreshDec.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxOokPeakThreshDec.FormattingEnabled = true;
			errorProvider.SetIconPadding(cBoxOokPeakThreshDec, 30);
			cBoxOokPeakThreshDec.Items.AddRange(["1x per chip", "1x every 2 chips", "1x every 4 chips", "1x every 8 chips", "2x per chip", "4x per chip", "8x per chip", "16x per chip"]);
			cBoxOokPeakThreshDec.Location = new Point(125, 135);
			cBoxOokPeakThreshDec.Name = "cBoxOokPeakThreshDec";
			cBoxOokPeakThreshDec.Size = new Size(102, 21);
			cBoxOokPeakThreshDec.TabIndex = 6;
			cBoxOokPeakThreshDec.SelectedIndexChanged += cBoxOokPeakThreshDec_SelectedIndexChanged;
			cBoxOokAverageThreshFilt.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxOokAverageThreshFilt.FormattingEnabled = true;
			errorProvider.SetIconPadding(cBoxOokAverageThreshFilt, 30);
			cBoxOokAverageThreshFilt.Items.AddRange(["Bitrate / 32π", "Bitrate / 8π", "Bitrate / 4π", "Bitrate / 2π"]);
			cBoxOokAverageThreshFilt.Location = new Point(125, 161);
			cBoxOokAverageThreshFilt.Name = "cBoxOokAverageThreshFilt";
			cBoxOokAverageThreshFilt.Size = new Size(102, 21);
			cBoxOokAverageThreshFilt.TabIndex = 8;
			cBoxOokAverageThreshFilt.SelectedIndexChanged += cBoxOokAverageThreshFilt_SelectedIndexChanged;
			errorProvider.SetIconPadding(nudRssiOffset, 30);
			nudRssiOffset.Location = new Point(125, 19);
			nudRssiOffset.Maximum = new decimal([15, 0, 0, 0]);
			nudRssiOffset.Minimum = new decimal([16, 0, 0, -2147483648]);
			nudRssiOffset.Name = "nudRssiOffset";
			nudRssiOffset.Size = new Size(98, 20);
			nudRssiOffset.TabIndex = 3;
			nudRssiOffset.ThousandsSeparator = true;
			nudRssiOffset.ValueChanged += nudRssiOffset_ValueChanged;
			errorProvider.SetIconPadding(nudRssiSmoothing, 30);
			nudRssiSmoothing.Location = new Point(125, 45);
			nudRssiSmoothing.Maximum = new decimal([256, 0, 0, 0]);
			nudRssiSmoothing.Minimum = new decimal([2, 0, 0, 0]);
			nudRssiSmoothing.Name = "nudRssiSmoothing";
			nudRssiSmoothing.Size = new Size(98, 20);
			nudRssiSmoothing.TabIndex = 3;
			nudRssiSmoothing.ThousandsSeparator = true;
			nudRssiSmoothing.Value = new decimal([2, 0, 0, 0]);
			nudRssiSmoothing.ValueChanged += nudRssiSmoothing_ValueChanged;
			errorProvider.SetIconPadding(nudRssiCollisionThreshold, 30);
			nudRssiCollisionThreshold.Location = new Point(125, 42);
			nudRssiCollisionThreshold.Maximum = new decimal([255, 0, 0, 0]);
			nudRssiCollisionThreshold.Name = "nudRssiCollisionThreshold";
			nudRssiCollisionThreshold.Size = new Size(98, 20);
			nudRssiCollisionThreshold.TabIndex = 3;
			nudRssiCollisionThreshold.ThousandsSeparator = true;
			nudRssiCollisionThreshold.ValueChanged += nudRssiCollisionThreshold_ValueChanged;
			lblRssiValue.BackColor = Color.Transparent;
			lblRssiValue.BorderStyle = BorderStyle.Fixed3D;
			errorProvider.SetIconPadding(lblRssiValue, 30);
			lblRssiValue.Location = new Point(125, 97);
			lblRssiValue.Margin = new Padding(3);
			lblRssiValue.Name = "lblRssiValue";
			lblRssiValue.Size = new Size(98, 20);
			lblRssiValue.TabIndex = 15;
			lblRssiValue.Text = "0";
			lblRssiValue.TextAlign = ContentAlignment.MiddleCenter;
			nudRssiThresh.DecimalPlaces = 1;
			errorProvider.SetIconPadding(nudRssiThresh, 30);
			nudRssiThresh.Increment = new decimal([5, 0, 0, 65536]);
			nudRssiThresh.Location = new Point(125, 71);
			// var numericUpDownEx = nudRssiThresh;
			// var bits = new int[4];
			nudRssiThresh.Maximum = new decimal(new int[4]);
			nudRssiThresh.Minimum = new decimal([1275, 0, 0, -2147418112]);
			nudRssiThresh.Name = "nudRssiThresh";
			nudRssiThresh.Size = new Size(98, 20);
			nudRssiThresh.TabIndex = 11;
			nudRssiThresh.ThousandsSeparator = true;
			nudRssiThresh.Value = new decimal([80, 0, 0, -2147483648]);
			nudRssiThresh.ValueChanged += nudRssiThresh_ValueChanged;
			panel2.Controls.Add(gBoxAgc);
			panel2.Controls.Add(gBoxTimeout);
			panel2.Controls.Add(gBoxPreamble);
			panel2.Controls.Add(gBoxRxBw);
			panel2.Controls.Add(gBoxRxConfig);
			panel2.Controls.Add(gBoxDemodulator);
			panel2.Controls.Add(gBoxRssi);
			panel2.Controls.Add(gBoxAfc);
			panel2.Location = new Point(0, 0);
			panel2.Margin = new Padding(0);
			panel2.Name = "panel2";
			panel2.Size = new Size(799, 384);
			panel2.TabIndex = 8;
			gBoxAgc.Controls.Add(label4);
			gBoxAgc.Controls.Add(label39);
			gBoxAgc.Controls.Add(label6);
			gBoxAgc.Controls.Add(label29);
			gBoxAgc.Controls.Add(label31);
			gBoxAgc.Controls.Add(label32);
			gBoxAgc.Controls.Add(label16);
			gBoxAgc.Controls.Add(label33);
			gBoxAgc.Controls.Add(label34);
			gBoxAgc.Controls.Add(label46);
			gBoxAgc.Controls.Add(btnAgcStart);
			gBoxAgc.Controls.Add(panel5);
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
			gBoxAgc.Location = new Point(3, 74);
			gBoxAgc.Name = "gBoxAgc";
			gBoxAgc.Size = new Size(260, 217);
			gBoxAgc.TabIndex = 7;
			gBoxAgc.TabStop = false;
			gBoxAgc.Text = "AGC";
			label4.AutoSize = true;
			label4.BackColor = Color.Transparent;
			label4.Location = new Point(3, 69);
			label4.Name = "label4";
			label4.Size = new Size(89, 13);
			label4.TabIndex = 2;
			label4.Text = "Reference Level:";
			label39.AutoSize = true;
			label39.BackColor = Color.Transparent;
			label39.Location = new Point(3, 18);
			label39.Name = "label39";
			label39.Size = new Size(32, 13);
			label39.TabIndex = 0;
			label39.Text = "AGC:";
			label6.AutoSize = true;
			label6.BackColor = Color.Transparent;
			label6.Location = new Point(3, 94);
			label6.Name = "label6";
			label6.Size = new Size(89, 13);
			label6.TabIndex = 8;
			label6.Text = "Threshold step 1:";
			label29.AutoSize = true;
			label29.BackColor = Color.Transparent;
			label29.Location = new Point(3, 120);
			label29.Name = "label29";
			label29.Size = new Size(89, 13);
			label29.TabIndex = 11;
			label29.Text = "Threshold step 2:";
			label31.AutoSize = true;
			label31.BackColor = Color.Transparent;
			label31.Location = new Point(3, 146);
			label31.Name = "label31";
			label31.Size = new Size(89, 13);
			label31.TabIndex = 14;
			label31.Text = "Threshold step 3:";
			label32.AutoSize = true;
			label32.BackColor = Color.Transparent;
			label32.Location = new Point(3, 172);
			label32.Name = "label32";
			label32.Size = new Size(89, 13);
			label32.TabIndex = 17;
			label32.Text = "Threshold step 4:";
			label16.AutoSize = true;
			label16.Location = new Point(3, 44);
			label16.Name = "label16";
			label16.Size = new Size(56, 13);
			label16.TabIndex = 8;
			label16.Text = "AGC auto:";
			label33.AutoSize = true;
			label33.BackColor = Color.Transparent;
			label33.Location = new Point(3, 198);
			label33.Name = "label33";
			label33.Size = new Size(89, 13);
			label33.TabIndex = 20;
			label33.Text = "Threshold step 5:";
			label34.AutoSize = true;
			label34.BackColor = Color.Transparent;
			label34.Location = new Point(224, 69);
			label34.Name = "label34";
			label34.Size = new Size(20, 13);
			label34.TabIndex = 4;
			label34.Text = "dB";
			label46.AutoSize = true;
			label46.BackColor = Color.Transparent;
			label46.Location = new Point(224, 94);
			label46.Name = "label46";
			label46.Size = new Size(20, 13);
			label46.TabIndex = 10;
			label46.Text = "dB";
			btnAgcStart.Location = new Point(120, 13);
			btnAgcStart.Name = "btnAgcStart";
			btnAgcStart.Size = new Size(98, 23);
			btnAgcStart.TabIndex = 19;
			btnAgcStart.Text = "Start";
			btnAgcStart.UseVisualStyleBackColor = true;
			btnAgcStart.Click += btnAgcStart_Click;
			panel5.AutoSize = true;
			panel5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel5.Controls.Add(rBtnAgcAutoOff);
			panel5.Controls.Add(rBtnAgcAutoOn);
			panel5.Location = new Point(120, 42);
			panel5.Name = "panel5";
			panel5.Size = new Size(98, 17);
			panel5.TabIndex = 21;
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
			label59.AutoSize = true;
			label59.BackColor = Color.Transparent;
			label59.Location = new Point(224, 120);
			label59.Name = "label59";
			label59.Size = new Size(20, 13);
			label59.TabIndex = 13;
			label59.Text = "dB";
			label60.AutoSize = true;
			label60.BackColor = Color.Transparent;
			label60.Location = new Point(224, 146);
			label60.Name = "label60";
			label60.Size = new Size(20, 13);
			label60.TabIndex = 16;
			label60.Text = "dB";
			label61.AutoSize = true;
			label61.BackColor = Color.Transparent;
			label61.Location = new Point(224, 172);
			label61.Name = "label61";
			label61.Size = new Size(20, 13);
			label61.TabIndex = 19;
			label61.Text = "dB";
			label62.AutoSize = true;
			label62.BackColor = Color.Transparent;
			label62.Location = new Point(224, 198);
			label62.Name = "label62";
			label62.Size = new Size(20, 13);
			label62.TabIndex = 22;
			label62.Text = "dB";
			nudAgcStep5.Location = new Point(120, 194);
			nudAgcStep5.Maximum = new decimal([15, 0, 0, 0]);
			nudAgcStep5.Name = "nudAgcStep5";
			nudAgcStep5.Size = new Size(98, 20);
			nudAgcStep5.TabIndex = 21;
			nudAgcStep5.Value = new decimal([11, 0, 0, 0]);
			nudAgcStep5.ValueChanged += nudAgcStep_ValueChanged;
			nudAgcStep4.Location = new Point(120, 168);
			nudAgcStep4.Maximum = new decimal([15, 0, 0, 0]);
			nudAgcStep4.Name = "nudAgcStep4";
			nudAgcStep4.Size = new Size(98, 20);
			nudAgcStep4.TabIndex = 18;
			nudAgcStep4.Value = new decimal([9, 0, 0, 0]);
			nudAgcStep4.ValueChanged += nudAgcStep_ValueChanged;
			nudAgcReferenceLevel.Location = new Point(120, 65);
			nudAgcReferenceLevel.Maximum = new decimal([63, 0, 0, 0]);
			nudAgcReferenceLevel.Name = "nudAgcReferenceLevel";
			nudAgcReferenceLevel.Size = new Size(98, 20);
			nudAgcReferenceLevel.TabIndex = 3;
			nudAgcReferenceLevel.ThousandsSeparator = true;
			nudAgcReferenceLevel.Value = new decimal([19, 0, 0, 0]);
			nudAgcReferenceLevel.ValueChanged += nudAgcReferenceLevel_ValueChanged;
			nudAgcStep3.Location = new Point(120, 142);
			nudAgcStep3.Maximum = new decimal([15, 0, 0, 0]);
			nudAgcStep3.Name = "nudAgcStep3";
			nudAgcStep3.Size = new Size(98, 20);
			nudAgcStep3.TabIndex = 15;
			nudAgcStep3.Value = new decimal([11, 0, 0, 0]);
			nudAgcStep3.ValueChanged += nudAgcStep_ValueChanged;
			nudAgcStep1.Location = new Point(120, 90);
			nudAgcStep1.Maximum = new decimal([31, 0, 0, 0]);
			nudAgcStep1.Name = "nudAgcStep1";
			nudAgcStep1.Size = new Size(98, 20);
			nudAgcStep1.TabIndex = 9;
			nudAgcStep1.Value = new decimal([16, 0, 0, 0]);
			nudAgcStep1.ValueChanged += nudAgcStep_ValueChanged;
			nudAgcStep2.Location = new Point(120, 116);
			nudAgcStep2.Maximum = new decimal([15, 0, 0, 0]);
			nudAgcStep2.Name = "nudAgcStep2";
			nudAgcStep2.Size = new Size(98, 20);
			nudAgcStep2.TabIndex = 12;
			nudAgcStep2.Value = new decimal([7, 0, 0, 0]);
			nudAgcStep2.ValueChanged += nudAgcStep_ValueChanged;
			gBoxTimeout.Controls.Add(nudTimeoutRssi);
			gBoxTimeout.Controls.Add(nudAutoRxRestartDelay);
			gBoxTimeout.Controls.Add(nudTimeoutSyncWord);
			gBoxTimeout.Controls.Add(label35);
			gBoxTimeout.Controls.Add(nudTimeoutPreamble);
			gBoxTimeout.Controls.Add(label15);
			gBoxTimeout.Controls.Add(label37);
			gBoxTimeout.Controls.Add(label27);
			gBoxTimeout.Controls.Add(label11);
			gBoxTimeout.Controls.Add(label36);
			gBoxTimeout.Controls.Add(label14);
			gBoxTimeout.Controls.Add(label9);
			gBoxTimeout.Location = new Point(535, 244);
			gBoxTimeout.Name = "gBoxTimeout";
			gBoxTimeout.Size = new Size(261, 121);
			gBoxTimeout.TabIndex = 4;
			gBoxTimeout.TabStop = false;
			gBoxTimeout.Text = "Timeout";
			gBoxTimeout.MouseEnter += control_MouseEnter;
			gBoxTimeout.MouseLeave += control_MouseLeave;
			label35.AutoSize = true;
			label35.BackColor = Color.Transparent;
			label35.Location = new Point(229, 75);
			label35.Name = "label35";
			label35.Size = new Size(20, 13);
			label35.TabIndex = 7;
			label35.Text = "ms";
			label15.AutoSize = true;
			label15.BackColor = Color.Transparent;
			label15.Location = new Point(229, 49);
			label15.Name = "label15";
			label15.Size = new Size(20, 13);
			label15.TabIndex = 7;
			label15.Text = "ms";
			label37.AutoSize = true;
			label37.BackColor = Color.Transparent;
			label37.Location = new Point(229, 101);
			label37.Name = "label37";
			label37.Size = new Size(20, 13);
			label37.TabIndex = 4;
			label37.Text = "ms";
			label27.AutoSize = true;
			label27.Location = new Point(7, 75);
			label27.Name = "label27";
			label27.Size = new Size(64, 13);
			label27.TabIndex = 5;
			label27.Text = "Signal sync:";
			label11.AutoSize = true;
			label11.BackColor = Color.Transparent;
			label11.Location = new Point(229, 23);
			label11.Name = "label11";
			label11.Size = new Size(20, 13);
			label11.TabIndex = 4;
			label11.Text = "ms";
			label36.AutoSize = true;
			label36.Location = new Point(7, 101);
			label36.Name = "label36";
			label36.Size = new Size(111, 13);
			label36.TabIndex = 2;
			label36.Text = "Inter packet Rx delay:";
			label14.AutoSize = true;
			label14.Location = new Point(7, 49);
			label14.Name = "label14";
			label14.Size = new Size(54, 13);
			label14.TabIndex = 5;
			label14.Text = "Preamble:";
			label9.AutoSize = true;
			label9.Location = new Point(7, 23);
			label9.Name = "label9";
			label9.Size = new Size(35, 13);
			label9.TabIndex = 2;
			label9.Text = "RSSI:";
			gBoxPreamble.Controls.Add(nudPreambleDetectorTol);
			gBoxPreamble.Controls.Add(label41);
			gBoxPreamble.Controls.Add(nudPreambleDetectorSize);
			gBoxPreamble.Controls.Add(label42);
			gBoxPreamble.Controls.Add(label44);
			gBoxPreamble.Controls.Add(panel1);
			gBoxPreamble.Controls.Add(label57);
			gBoxPreamble.Controls.Add(label58);
			gBoxPreamble.Location = new Point(3, 294);
			gBoxPreamble.Name = "gBoxPreamble";
			gBoxPreamble.Size = new Size(260, 88);
			gBoxPreamble.TabIndex = 4;
			gBoxPreamble.TabStop = false;
			gBoxPreamble.Text = "Preamble detetction";
			gBoxPreamble.MouseEnter += control_MouseEnter;
			gBoxPreamble.MouseLeave += control_MouseLeave;
			label41.AutoSize = true;
			label41.BackColor = Color.Transparent;
			label41.Location = new Point(224, 69);
			label41.Name = "label41";
			label41.Size = new Size(27, 13);
			label41.TabIndex = 7;
			label41.Text = "chip";
			label42.AutoSize = true;
			label42.BackColor = Color.Transparent;
			label42.Location = new Point(224, 44);
			label42.Name = "label42";
			label42.Size = new Size(27, 13);
			label42.TabIndex = 7;
			label42.Text = "byte";
			label44.AutoSize = true;
			label44.Location = new Point(7, 69);
			label44.Name = "label44";
			label44.Size = new Size(79, 13);
			label44.TabIndex = 5;
			label44.Text = "Error tolerance:";
			panel1.AutoSize = true;
			panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel1.Controls.Add(rBtnPreambleDetectorOff);
			panel1.Controls.Add(rBtnPreambleDetectorOn);
			panel1.Location = new Point(120, 18);
			panel1.Name = "panel1";
			panel1.Size = new Size(98, 17);
			panel1.TabIndex = 6;
			rBtnPreambleDetectorOff.AutoSize = true;
			rBtnPreambleDetectorOff.Location = new Point(50, 0);
			rBtnPreambleDetectorOff.Margin = new Padding(3, 0, 3, 0);
			rBtnPreambleDetectorOff.Name = "rBtnPreambleDetectorOff";
			rBtnPreambleDetectorOff.Size = new Size(45, 17);
			rBtnPreambleDetectorOff.TabIndex = 1;
			rBtnPreambleDetectorOff.Text = "OFF";
			rBtnPreambleDetectorOff.UseVisualStyleBackColor = true;
			rBtnPreambleDetectorOff.CheckedChanged += rBtnPreambleDetectorOn_CheckedChanged;
			rBtnPreambleDetectorOn.AutoSize = true;
			rBtnPreambleDetectorOn.Checked = true;
			rBtnPreambleDetectorOn.Location = new Point(3, 0);
			rBtnPreambleDetectorOn.Margin = new Padding(3, 0, 3, 0);
			rBtnPreambleDetectorOn.Name = "rBtnPreambleDetectorOn";
			rBtnPreambleDetectorOn.Size = new Size(41, 17);
			rBtnPreambleDetectorOn.TabIndex = 0;
			rBtnPreambleDetectorOn.TabStop = true;
			rBtnPreambleDetectorOn.Text = "ON";
			rBtnPreambleDetectorOn.UseVisualStyleBackColor = true;
			rBtnPreambleDetectorOn.CheckedChanged += rBtnPreambleDetectorOn_CheckedChanged;
			label57.AutoSize = true;
			label57.Location = new Point(7, 44);
			label57.Name = "label57";
			label57.Size = new Size(30, 13);
			label57.TabIndex = 5;
			label57.Text = "Size:";
			label58.AutoSize = true;
			label58.Location = new Point(7, 20);
			label58.Name = "label58";
			label58.Size = new Size(56, 13);
			label58.TabIndex = 2;
			label58.Text = "Detection:";
			gBoxRxBw.Controls.Add(lblAfcRxBw);
			gBoxRxBw.Controls.Add(lblRxBw);
			gBoxRxBw.Controls.Add(suffixAFCRxBw);
			gBoxRxBw.Controls.Add(nudRxFilterBwAfc);
			gBoxRxBw.Controls.Add(suffixRxBw);
			gBoxRxBw.Controls.Add(nudRxFilterBw);
			gBoxRxBw.Location = new Point(3, 2);
			gBoxRxBw.Name = "gBoxRxBw";
			gBoxRxBw.Size = new Size(260, 66);
			gBoxRxBw.TabIndex = 0;
			gBoxRxBw.TabStop = false;
			gBoxRxBw.Text = "Bandwidth";
			gBoxRxBw.MouseEnter += control_MouseEnter;
			gBoxRxBw.MouseLeave += control_MouseLeave;
			lblAfcRxBw.AutoSize = true;
			lblAfcRxBw.Location = new Point(3, 44);
			lblAfcRxBw.Name = "lblAfcRxBw";
			lblAfcRxBw.Size = new Size(104, 13);
			lblAfcRxBw.TabIndex = 3;
			lblAfcRxBw.Text = "AFC filter bandwidth:";
			lblRxBw.AutoSize = true;
			lblRxBw.Location = new Point(3, 18);
			lblRxBw.Name = "lblRxBw";
			lblRxBw.Size = new Size(97, 13);
			lblRxBw.TabIndex = 3;
			lblRxBw.Text = "Rx filter bandwidth:";
			suffixAFCRxBw.AutoSize = true;
			suffixAFCRxBw.Location = new Point(224, 46);
			suffixAFCRxBw.Name = "suffixAFCRxBw";
			suffixAFCRxBw.Size = new Size(20, 13);
			suffixAFCRxBw.TabIndex = 5;
			suffixAFCRxBw.Text = "Hz";
			suffixRxBw.AutoSize = true;
			suffixRxBw.Location = new Point(224, 21);
			suffixRxBw.Name = "suffixRxBw";
			suffixRxBw.Size = new Size(20, 13);
			suffixRxBw.TabIndex = 5;
			suffixRxBw.Text = "Hz";
			gBoxRxConfig.Controls.Add(cBoxRxTrigger);
			gBoxRxConfig.Controls.Add(label38);
			gBoxRxConfig.Controls.Add(label5);
			gBoxRxConfig.Controls.Add(btnRestartRxWithPllLock);
			gBoxRxConfig.Controls.Add(label3);
			gBoxRxConfig.Controls.Add(btnRestartRxWithoutPllLock);
			gBoxRxConfig.Controls.Add(panel4);
			gBoxRxConfig.Controls.Add(nudRssiCollisionThreshold);
			gBoxRxConfig.Controls.Add(label26);
			gBoxRxConfig.Location = new Point(269, 258);
			gBoxRxConfig.Name = "gBoxRxConfig";
			gBoxRxConfig.Size = new Size(261, 124);
			gBoxRxConfig.TabIndex = 4;
			gBoxRxConfig.TabStop = false;
			gBoxRxConfig.Text = "Rx startup control";
			gBoxRxConfig.MouseEnter += control_MouseEnter;
			gBoxRxConfig.MouseLeave += control_MouseLeave;
			label38.AutoSize = true;
			label38.BackColor = Color.Transparent;
			label38.Location = new Point(229, 46);
			label38.Name = "label38";
			label38.Size = new Size(20, 13);
			label38.TabIndex = 12;
			label38.Text = "dB";
			label38.TextAlign = ContentAlignment.MiddleCenter;
			label5.AutoSize = true;
			label5.Location = new Point(3, 21);
			label5.Name = "label5";
			label5.Size = new Size(115, 13);
			label5.TabIndex = 5;
			label5.Text = "Restart Rx on collision:";
			btnRestartRxWithPllLock.Location = new Point(133, 68);
			btnRestartRxWithPllLock.Name = "btnRestartRxWithPllLock";
			btnRestartRxWithPllLock.Size = new Size(89, 23);
			btnRestartRxWithPllLock.TabIndex = 19;
			btnRestartRxWithPllLock.Text = "Rx Restart PLL";
			btnRestartRxWithPllLock.UseVisualStyleBackColor = true;
			btnRestartRxWithPllLock.Click += btnRestartRxWithPllLock_Click;
			label3.AutoSize = true;
			label3.Location = new Point(3, 101);
			label3.Name = "label3";
			label3.Size = new Size(55, 13);
			label3.TabIndex = 8;
			label3.Text = "Rx trigger:";
			btnRestartRxWithoutPllLock.Location = new Point(38, 68);
			btnRestartRxWithoutPllLock.Name = "btnRestartRxWithoutPllLock";
			btnRestartRxWithoutPllLock.Size = new Size(89, 23);
			btnRestartRxWithoutPllLock.TabIndex = 19;
			btnRestartRxWithoutPllLock.Text = " Rx Restart";
			btnRestartRxWithoutPllLock.UseVisualStyleBackColor = true;
			btnRestartRxWithoutPllLock.Click += btnRestartRxWithoutPllLock_Click;
			panel4.AutoSize = true;
			panel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel4.Controls.Add(rBtnRestartRxOnCollisionOff);
			panel4.Controls.Add(rBtnRestartRxOnCollisionOn);
			panel4.Location = new Point(125, 19);
			panel4.Name = "panel4";
			panel4.Size = new Size(98, 17);
			panel4.TabIndex = 6;
			rBtnRestartRxOnCollisionOff.AutoSize = true;
			rBtnRestartRxOnCollisionOff.Location = new Point(50, 0);
			rBtnRestartRxOnCollisionOff.Margin = new Padding(3, 0, 3, 0);
			rBtnRestartRxOnCollisionOff.Name = "rBtnRestartRxOnCollisionOff";
			rBtnRestartRxOnCollisionOff.Size = new Size(45, 17);
			rBtnRestartRxOnCollisionOff.TabIndex = 1;
			rBtnRestartRxOnCollisionOff.Text = "OFF";
			rBtnRestartRxOnCollisionOff.UseVisualStyleBackColor = true;
			rBtnRestartRxOnCollisionOff.CheckedChanged += rBtnRestartRxOnCollisionOn_CheckedChanged;
			rBtnRestartRxOnCollisionOn.AutoSize = true;
			rBtnRestartRxOnCollisionOn.Checked = true;
			rBtnRestartRxOnCollisionOn.Location = new Point(3, 0);
			rBtnRestartRxOnCollisionOn.Margin = new Padding(3, 0, 3, 0);
			rBtnRestartRxOnCollisionOn.Name = "rBtnRestartRxOnCollisionOn";
			rBtnRestartRxOnCollisionOn.Size = new Size(41, 17);
			rBtnRestartRxOnCollisionOn.TabIndex = 0;
			rBtnRestartRxOnCollisionOn.TabStop = true;
			rBtnRestartRxOnCollisionOn.Text = "ON";
			rBtnRestartRxOnCollisionOn.UseVisualStyleBackColor = true;
			rBtnRestartRxOnCollisionOn.CheckedChanged += rBtnRestartRxOnCollisionOn_CheckedChanged;
			label26.AutoSize = true;
			label26.Location = new Point(3, 46);
			label26.Name = "label26";
			label26.Size = new Size(94, 13);
			label26.TabIndex = 2;
			label26.Text = "Collision threshold:";
			gBoxDemodulator.Controls.Add(pnlHorizontalSeparator);
			gBoxDemodulator.Controls.Add(nudOokAverageOffset);
			gBoxDemodulator.Controls.Add(label30);
			gBoxDemodulator.Controls.Add(cBoxOokThreshType);
			gBoxDemodulator.Controls.Add(label1);
			gBoxDemodulator.Controls.Add(lblOokType);
			gBoxDemodulator.Controls.Add(lblOokStep);
			gBoxDemodulator.Controls.Add(label28);
			gBoxDemodulator.Controls.Add(lblOokDec);
			gBoxDemodulator.Controls.Add(lblOokCutoff);
			gBoxDemodulator.Controls.Add(lblOokFixed);
			gBoxDemodulator.Controls.Add(suffixOOKstep);
			gBoxDemodulator.Controls.Add(label40);
			gBoxDemodulator.Controls.Add(suffixOOKfixed);
			gBoxDemodulator.Controls.Add(nudOokPeakThreshStep);
			gBoxDemodulator.Controls.Add(nudOokFixedThresh);
			gBoxDemodulator.Controls.Add(panel13);
			gBoxDemodulator.Controls.Add(cBoxOokPeakThreshDec);
			gBoxDemodulator.Controls.Add(cBoxOokAverageThreshFilt);
			gBoxDemodulator.Location = new Point(535, 20);
			gBoxDemodulator.Name = "gBoxDemodulator";
			gBoxDemodulator.Size = new Size(261, 218);
			gBoxDemodulator.TabIndex = 2;
			gBoxDemodulator.TabStop = false;
			gBoxDemodulator.Text = "Demodulator";
			gBoxDemodulator.MouseEnter += control_MouseEnter;
			gBoxDemodulator.MouseLeave += control_MouseLeave;
			pnlHorizontalSeparator.BorderStyle = BorderStyle.FixedSingle;
			pnlHorizontalSeparator.Location = new Point(49, 47);
			pnlHorizontalSeparator.Name = "pnlHorizontalSeparator";
			pnlHorizontalSeparator.Size = new Size(206, 1);
			pnlHorizontalSeparator.TabIndex = 12;
			label30.AutoSize = true;
			label30.Location = new Point(6, 192);
			label30.Name = "label30";
			label30.Size = new Size(58, 13);
			label30.TabIndex = 2;
			label30.Text = "Avg offset:";
			label1.AutoSize = true;
			label1.Location = new Point(6, 41);
			label1.Name = "label1";
			label1.Size = new Size(30, 13);
			label1.TabIndex = 0;
			label1.Text = "OOK";
			lblOokType.AutoSize = true;
			lblOokType.Location = new Point(6, 61);
			lblOokType.Name = "lblOokType";
			lblOokType.Size = new Size(80, 13);
			lblOokType.TabIndex = 0;
			lblOokType.Text = "Threshold type:";
			lblOokStep.AutoSize = true;
			lblOokStep.Location = new Point(6, 87);
			lblOokStep.Name = "lblOokStep";
			lblOokStep.Size = new Size(104, 13);
			lblOokStep.TabIndex = 2;
			lblOokStep.Text = "Peak threshold step:";
			label28.AutoSize = true;
			label28.Location = new Point(6, 21);
			label28.Name = "label28";
			label28.Size = new Size(84, 13);
			label28.TabIndex = 8;
			label28.Text = "Bit synchronizer:";
			lblOokDec.AutoSize = true;
			lblOokDec.Location = new Point(6, 139);
			lblOokDec.Name = "lblOokDec";
			lblOokDec.Size = new Size(108, 13);
			lblOokDec.TabIndex = 5;
			lblOokDec.Text = "Peak threshold decr.:";
			lblOokCutoff.AutoSize = true;
			lblOokCutoff.Location = new Point(6, 165);
			lblOokCutoff.Name = "lblOokCutoff";
			lblOokCutoff.Size = new Size(105, 13);
			lblOokCutoff.TabIndex = 7;
			lblOokCutoff.Text = "Avg threshold cutoff:";
			lblOokFixed.AutoSize = true;
			lblOokFixed.Location = new Point(6, 113);
			lblOokFixed.Name = "lblOokFixed";
			lblOokFixed.Size = new Size(81, 13);
			lblOokFixed.TabIndex = 9;
			lblOokFixed.Text = "Fixed threshold:";
			suffixOOKstep.AutoSize = true;
			suffixOOKstep.BackColor = Color.Transparent;
			suffixOOKstep.Location = new Point(229, 87);
			suffixOOKstep.Name = "suffixOOKstep";
			suffixOOKstep.Size = new Size(20, 13);
			suffixOOKstep.TabIndex = 4;
			suffixOOKstep.Text = "dB";
			label40.AutoSize = true;
			label40.BackColor = Color.Transparent;
			label40.Location = new Point(229, 192);
			label40.Name = "label40";
			label40.Size = new Size(20, 13);
			label40.TabIndex = 11;
			label40.Text = "dB";
			suffixOOKfixed.AutoSize = true;
			suffixOOKfixed.BackColor = Color.Transparent;
			suffixOOKfixed.Location = new Point(229, 122);
			suffixOOKfixed.Name = "suffixOOKfixed";
			suffixOOKfixed.Size = new Size(20, 13);
			suffixOOKfixed.TabIndex = 11;
			suffixOOKfixed.Text = "dB";
			panel13.AutoSize = true;
			panel13.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel13.Controls.Add(rBtnBitSyncOff);
			panel13.Controls.Add(rBtnBitSyncOn);
			panel13.Location = new Point(125, 19);
			panel13.Name = "panel13";
			panel13.Size = new Size(98, 17);
			panel13.TabIndex = 7;
			rBtnBitSyncOff.AutoSize = true;
			rBtnBitSyncOff.Location = new Point(50, 0);
			rBtnBitSyncOff.Margin = new Padding(3, 0, 3, 0);
			rBtnBitSyncOff.Name = "rBtnBitSyncOff";
			rBtnBitSyncOff.Size = new Size(45, 17);
			rBtnBitSyncOff.TabIndex = 1;
			rBtnBitSyncOff.Text = "OFF";
			rBtnBitSyncOff.UseVisualStyleBackColor = true;
			rBtnBitSyncOff.CheckedChanged += rBtnBitSyncOn_CheckedChanged;
			rBtnBitSyncOn.AutoSize = true;
			rBtnBitSyncOn.Checked = true;
			rBtnBitSyncOn.Location = new Point(3, 0);
			rBtnBitSyncOn.Margin = new Padding(3, 0, 3, 0);
			rBtnBitSyncOn.Name = "rBtnBitSyncOn";
			rBtnBitSyncOn.Size = new Size(41, 17);
			rBtnBitSyncOn.TabIndex = 0;
			rBtnBitSyncOn.TabStop = true;
			rBtnBitSyncOn.Text = "ON";
			rBtnBitSyncOn.UseVisualStyleBackColor = true;
			rBtnBitSyncOn.CheckedChanged += rBtnBitSyncOn_CheckedChanged;
			gBoxRssi.Controls.Add(label23);
			gBoxRssi.Controls.Add(label17);
			gBoxRssi.Controls.Add(label54);
			gBoxRssi.Controls.Add(nudRssiOffset);
			gBoxRssi.Controls.Add(label24);
			gBoxRssi.Controls.Add(label55);
			gBoxRssi.Controls.Add(label25);
			gBoxRssi.Controls.Add(nudRssiSmoothing);
			gBoxRssi.Controls.Add(label56);
			gBoxRssi.Controls.Add(lblRssiValue);
			gBoxRssi.Controls.Add(nudRssiThresh);
			gBoxRssi.Location = new Point(269, 127);
			gBoxRssi.Name = "gBoxRssi";
			gBoxRssi.Size = new Size(261, 125);
			gBoxRssi.TabIndex = 4;
			gBoxRssi.TabStop = false;
			gBoxRssi.Text = "RSSI";
			gBoxRssi.MouseEnter += control_MouseEnter;
			gBoxRssi.MouseLeave += control_MouseLeave;
			label23.AutoSize = true;
			label23.BackColor = Color.Transparent;
			label23.Location = new Point(229, 23);
			label23.Name = "label23";
			label23.Size = new Size(20, 13);
			label23.TabIndex = 12;
			label23.Text = "dB";
			label23.TextAlign = ContentAlignment.MiddleCenter;
			label17.AutoSize = true;
			label17.BackColor = Color.Transparent;
			label17.Location = new Point(229, 75);
			label17.Name = "label17";
			label17.Size = new Size(28, 13);
			label17.TabIndex = 12;
			label17.Text = "dBm";
			label17.TextAlign = ContentAlignment.MiddleCenter;
			label54.AutoSize = true;
			label54.BackColor = Color.Transparent;
			label54.Location = new Point(229, 101);
			label54.Name = "label54";
			label54.Size = new Size(28, 13);
			label54.TabIndex = 17;
			label54.Text = "dBm";
			label54.TextAlign = ContentAlignment.MiddleCenter;
			label24.AutoSize = true;
			label24.Location = new Point(3, 23);
			label24.Name = "label24";
			label24.Size = new Size(38, 13);
			label24.TabIndex = 2;
			label24.Text = "Offset:";
			label55.AutoSize = true;
			label55.BackColor = Color.Transparent;
			label55.Location = new Point(3, 75);
			label55.Margin = new Padding(0);
			label55.Name = "label55";
			label55.Size = new Size(57, 13);
			label55.TabIndex = 10;
			label55.Text = "Threshold:";
			label55.TextAlign = ContentAlignment.MiddleCenter;
			label25.AutoSize = true;
			label25.Location = new Point(3, 49);
			label25.Name = "label25";
			label25.Size = new Size(60, 13);
			label25.TabIndex = 2;
			label25.Text = "Smoothing:";
			label56.AutoSize = true;
			label56.BackColor = Color.Transparent;
			label56.Location = new Point(3, 101);
			label56.Margin = new Padding(0);
			label56.Name = "label56";
			label56.Size = new Size(37, 13);
			label56.TabIndex = 13;
			label56.Text = "Value:";
			label56.TextAlign = ContentAlignment.MiddleCenter;
			gBoxAfc.Controls.Add(label19);
			gBoxAfc.Controls.Add(btnFeiRead);
			gBoxAfc.Controls.Add(panel8);
			gBoxAfc.Controls.Add(lblFeiValue);
			gBoxAfc.Controls.Add(label12);
			gBoxAfc.Controls.Add(label20);
			gBoxAfc.Controls.Add(label18);
			gBoxAfc.Controls.Add(label10);
			gBoxAfc.Controls.Add(btnAfcClear);
			gBoxAfc.Controls.Add(lblAfcValue);
			gBoxAfc.Controls.Add(label22);
			gBoxAfc.Controls.Add(panel9);
			gBoxAfc.Location = new Point(269, 3);
			gBoxAfc.Name = "gBoxAfc";
			gBoxAfc.Size = new Size(261, 118);
			gBoxAfc.TabIndex = 3;
			gBoxAfc.TabStop = false;
			gBoxAfc.Text = "AFC";
			gBoxAfc.MouseEnter += control_MouseEnter;
			gBoxAfc.MouseLeave += control_MouseLeave;
			label19.AutoSize = true;
			label19.BackColor = Color.Transparent;
			label19.Location = new Point(3, 44);
			label19.Name = "label19";
			label19.Size = new Size(80, 13);
			label19.TabIndex = 5;
			label19.Text = "AFC auto clear:";
			btnFeiRead.Location = new Point(78, 90);
			btnFeiRead.Name = "btnFeiRead";
			btnFeiRead.Size = new Size(41, 23);
			btnFeiRead.TabIndex = 16;
			btnFeiRead.Text = "Read";
			btnFeiRead.UseVisualStyleBackColor = true;
			btnFeiRead.Click += btnFeiMeasure_Click;
			panel8.AutoSize = true;
			panel8.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel8.Controls.Add(rBtnAfcAutoClearOff);
			panel8.Controls.Add(rBtnAfcAutoClearOn);
			panel8.Location = new Point(125, 42);
			panel8.Name = "panel8";
			panel8.Size = new Size(98, 17);
			panel8.TabIndex = 6;
			rBtnAfcAutoClearOff.AutoSize = true;
			rBtnAfcAutoClearOff.Location = new Point(50, 0);
			rBtnAfcAutoClearOff.Margin = new Padding(3, 0, 3, 0);
			rBtnAfcAutoClearOff.Name = "rBtnAfcAutoClearOff";
			rBtnAfcAutoClearOff.Size = new Size(45, 17);
			rBtnAfcAutoClearOff.TabIndex = 1;
			rBtnAfcAutoClearOff.Text = "OFF";
			rBtnAfcAutoClearOff.UseVisualStyleBackColor = true;
			rBtnAfcAutoClearOff.CheckedChanged += rBtnAfcAutoClearOn_CheckedChanged;
			rBtnAfcAutoClearOn.AutoSize = true;
			rBtnAfcAutoClearOn.Checked = true;
			rBtnAfcAutoClearOn.Location = new Point(3, 0);
			rBtnAfcAutoClearOn.Margin = new Padding(3, 0, 3, 0);
			rBtnAfcAutoClearOn.Name = "rBtnAfcAutoClearOn";
			rBtnAfcAutoClearOn.Size = new Size(41, 17);
			rBtnAfcAutoClearOn.TabIndex = 0;
			rBtnAfcAutoClearOn.TabStop = true;
			rBtnAfcAutoClearOn.Text = "ON";
			rBtnAfcAutoClearOn.UseVisualStyleBackColor = true;
			rBtnAfcAutoClearOn.CheckedChanged += rBtnAfcAutoClearOn_CheckedChanged;
			lblFeiValue.BackColor = Color.Transparent;
			lblFeiValue.BorderStyle = BorderStyle.Fixed3D;
			lblFeiValue.Location = new Point(125, 91);
			lblFeiValue.Margin = new Padding(3);
			lblFeiValue.Name = "lblFeiValue";
			lblFeiValue.Size = new Size(98, 20);
			lblFeiValue.TabIndex = 17;
			lblFeiValue.Text = "0";
			lblFeiValue.TextAlign = ContentAlignment.MiddleLeft;
			label12.AutoSize = true;
			label12.BackColor = Color.Transparent;
			label12.Location = new Point(3, 95);
			label12.Name = "label12";
			label12.Size = new Size(26, 13);
			label12.TabIndex = 15;
			label12.Text = "FEI:";
			label12.TextAlign = ContentAlignment.MiddleCenter;
			label20.AutoSize = true;
			label20.Location = new Point(3, 21);
			label20.Name = "label20";
			label20.Size = new Size(54, 13);
			label20.TabIndex = 8;
			label20.Text = "AFC auto:";
			label18.AutoSize = true;
			label18.BackColor = Color.Transparent;
			label18.Location = new Point(229, 69);
			label18.Name = "label18";
			label18.Size = new Size(20, 13);
			label18.TabIndex = 14;
			label18.Text = "Hz";
			label10.AutoSize = true;
			label10.BackColor = Color.Transparent;
			label10.Location = new Point(229, 95);
			label10.Name = "label10";
			label10.Size = new Size(20, 13);
			label10.TabIndex = 19;
			label10.Text = "Hz";
			btnAfcClear.Location = new Point(78, 64);
			btnAfcClear.Name = "btnAfcClear";
			btnAfcClear.Size = new Size(41, 23);
			btnAfcClear.TabIndex = 11;
			btnAfcClear.Text = "Clear";
			btnAfcClear.UseVisualStyleBackColor = true;
			btnAfcClear.Click += btnAfcClear_Click;
			lblAfcValue.BackColor = Color.Transparent;
			lblAfcValue.BorderStyle = BorderStyle.Fixed3D;
			lblAfcValue.Location = new Point(125, 65);
			lblAfcValue.Margin = new Padding(3);
			lblAfcValue.Name = "lblAfcValue";
			lblAfcValue.Size = new Size(98, 20);
			lblAfcValue.TabIndex = 12;
			lblAfcValue.Text = "0";
			lblAfcValue.TextAlign = ContentAlignment.MiddleLeft;
			label22.AutoSize = true;
			label22.BackColor = Color.Transparent;
			label22.Location = new Point(3, 69);
			label22.Name = "label22";
			label22.Size = new Size(30, 13);
			label22.TabIndex = 9;
			label22.Text = "AFC:";
			label22.TextAlign = ContentAlignment.MiddleCenter;
			panel9.AutoSize = true;
			panel9.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel9.Controls.Add(rBtnAfcAutoOff);
			panel9.Controls.Add(rBtnAfcAutoOn);
			panel9.Location = new Point(125, 19);
			panel9.Name = "panel9";
			panel9.Size = new Size(98, 17);
			panel9.TabIndex = 7;
			rBtnAfcAutoOff.AutoSize = true;
			rBtnAfcAutoOff.Location = new Point(50, 0);
			rBtnAfcAutoOff.Margin = new Padding(3, 0, 3, 0);
			rBtnAfcAutoOff.Name = "rBtnAfcAutoOff";
			rBtnAfcAutoOff.Size = new Size(45, 17);
			rBtnAfcAutoOff.TabIndex = 1;
			rBtnAfcAutoOff.Text = "OFF";
			rBtnAfcAutoOff.UseVisualStyleBackColor = true;
			rBtnAfcAutoOff.CheckedChanged += rBtnAfcAutoOn_CheckedChanged;
			rBtnAfcAutoOn.AutoSize = true;
			rBtnAfcAutoOn.Checked = true;
			rBtnAfcAutoOn.Location = new Point(3, 0);
			rBtnAfcAutoOn.Margin = new Padding(3, 0, 3, 0);
			rBtnAfcAutoOn.Name = "rBtnAfcAutoOn";
			rBtnAfcAutoOn.Size = new Size(41, 17);
			rBtnAfcAutoOn.TabIndex = 0;
			rBtnAfcAutoOn.TabStop = true;
			rBtnAfcAutoOn.Text = "ON";
			rBtnAfcAutoOn.UseVisualStyleBackColor = true;
			rBtnAfcAutoOn.CheckedChanged += rBtnAfcAutoOn_CheckedChanged;
			gBoxLnaSettings.Controls.Add(panel3);
			gBoxLnaSettings.Controls.Add(label2);
			gBoxLnaSettings.Controls.Add(label13);
			gBoxLnaSettings.Controls.Add(lblAgcReference);
			gBoxLnaSettings.Controls.Add(label48);
			gBoxLnaSettings.Controls.Add(label49);
			gBoxLnaSettings.Controls.Add(label50);
			gBoxLnaSettings.Controls.Add(label51);
			gBoxLnaSettings.Controls.Add(label52);
			gBoxLnaSettings.Controls.Add(lblLnaGain1);
			gBoxLnaSettings.Controls.Add(label53);
			gBoxLnaSettings.Controls.Add(panel6);
			gBoxLnaSettings.Controls.Add(lblLnaGain2);
			gBoxLnaSettings.Controls.Add(lblLnaGain3);
			gBoxLnaSettings.Controls.Add(lblLnaGain4);
			gBoxLnaSettings.Controls.Add(lblLnaGain5);
			gBoxLnaSettings.Controls.Add(lblLnaGain6);
			gBoxLnaSettings.Controls.Add(lblAgcThresh1);
			gBoxLnaSettings.Controls.Add(lblAgcThresh2);
			gBoxLnaSettings.Controls.Add(lblAgcThresh3);
			gBoxLnaSettings.Controls.Add(lblAgcThresh4);
			gBoxLnaSettings.Controls.Add(lblAgcThresh5);
			gBoxLnaSettings.Controls.Add(label47);
			gBoxLnaSettings.Location = new Point(3, 387);
			gBoxLnaSettings.Name = "gBoxLnaSettings";
			gBoxLnaSettings.Size = new Size(793, 103);
			gBoxLnaSettings.TabIndex = 7;
			gBoxLnaSettings.TabStop = false;
			gBoxLnaSettings.Text = "Lna settings";
			gBoxLnaSettings.MouseEnter += control_MouseEnter;
			gBoxLnaSettings.MouseLeave += control_MouseLeave;
			panel3.AutoSize = true;
			panel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel3.Controls.Add(rBtnLnaBoostOff);
			panel3.Controls.Add(rBtnLnaBoostOn);
			panel3.Location = new Point(71, 50);
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
			label2.AutoSize = true;
			label2.Location = new Point(6, 52);
			label2.Name = "label2";
			label2.Size = new Size(60, 13);
			label2.TabIndex = 0;
			label2.Text = "LNA boost:";
			label13.BackColor = Color.Transparent;
			label13.Location = new Point(167, 75);
			label13.Name = "label13";
			label13.Size = new Size(42, 13);
			label13.TabIndex = 6;
			label13.Text = "Gain";
			label13.TextAlign = ContentAlignment.MiddleCenter;
			lblAgcReference.BackColor = Color.Transparent;
			lblAgcReference.Location = new Point(124, 32);
			lblAgcReference.Margin = new Padding(0, 0, 0, 3);
			lblAgcReference.Name = "lblAgcReference";
			lblAgcReference.Size = new Size(100, 13);
			lblAgcReference.TabIndex = 7;
			lblAgcReference.Text = "-80";
			lblAgcReference.TextAlign = ContentAlignment.MiddleCenter;
			label48.BackColor = Color.Transparent;
			label48.Location = new Point(124, 16);
			label48.Margin = new Padding(0, 0, 0, 3);
			label48.Name = "label48";
			label48.Size = new Size(100, 13);
			label48.TabIndex = 0;
			label48.Text = "Reference";
			label48.TextAlign = ContentAlignment.MiddleCenter;
			label49.BackColor = Color.Transparent;
			label49.Location = new Point(224, 16);
			label49.Margin = new Padding(0, 0, 0, 3);
			label49.Name = "label49";
			label49.Size = new Size(100, 13);
			label49.TabIndex = 1;
			label49.Text = "Threshold 1";
			label49.TextAlign = ContentAlignment.MiddleCenter;
			label50.BackColor = Color.Transparent;
			label50.Location = new Point(324, 16);
			label50.Margin = new Padding(0, 0, 0, 3);
			label50.Name = "label50";
			label50.Size = new Size(100, 13);
			label50.TabIndex = 2;
			label50.Text = "Threshold 2";
			label50.TextAlign = ContentAlignment.MiddleCenter;
			label51.BackColor = Color.Transparent;
			label51.Location = new Point(424, 16);
			label51.Margin = new Padding(0, 0, 0, 3);
			label51.Name = "label51";
			label51.Size = new Size(100, 13);
			label51.TabIndex = 3;
			label51.Text = "Threshold 3";
			label51.TextAlign = ContentAlignment.MiddleCenter;
			label52.BackColor = Color.Transparent;
			label52.Location = new Point(524, 16);
			label52.Margin = new Padding(0, 0, 0, 3);
			label52.Name = "label52";
			label52.Size = new Size(100, 13);
			label52.TabIndex = 4;
			label52.Text = "Threshold 4";
			label52.TextAlign = ContentAlignment.MiddleCenter;
			lblLnaGain1.BackColor = Color.LightSteelBlue;
			lblLnaGain1.BorderStyle = BorderStyle.Fixed3D;
			lblLnaGain1.Location = new Point(174, 48);
			lblLnaGain1.Margin = new Padding(0, 0, 0, 3);
			lblLnaGain1.Name = "lblLnaGain1";
			lblLnaGain1.Size = new Size(100, 20);
			lblLnaGain1.TabIndex = 14;
			lblLnaGain1.Text = "G1";
			lblLnaGain1.TextAlign = ContentAlignment.MiddleCenter;
			label53.BackColor = Color.Transparent;
			label53.Location = new Point(624, 16);
			label53.Margin = new Padding(0, 0, 0, 3);
			label53.Name = "label53";
			label53.Size = new Size(100, 13);
			label53.TabIndex = 5;
			label53.Text = "Threshold 5";
			label53.TextAlign = ContentAlignment.MiddleCenter;
			panel6.AutoSize = true;
			panel6.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel6.Controls.Add(rBtnLnaGain1);
			panel6.Controls.Add(rBtnLnaGain2);
			panel6.Controls.Add(rBtnLnaGain3);
			panel6.Controls.Add(rBtnLnaGain4);
			panel6.Controls.Add(rBtnLnaGain5);
			panel6.Controls.Add(rBtnLnaGain6);
			panel6.Location = new Point(215, 75);
			panel6.Name = "panel6";
			panel6.Size = new Size(521, 13);
			panel6.TabIndex = 22;
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
			lblLnaGain3.BackColor = Color.Transparent;
			lblLnaGain3.BorderStyle = BorderStyle.Fixed3D;
			lblLnaGain3.Location = new Point(374, 48);
			lblLnaGain3.Margin = new Padding(0, 0, 0, 3);
			lblLnaGain3.Name = "lblLnaGain3";
			lblLnaGain3.Size = new Size(100, 20);
			lblLnaGain3.TabIndex = 16;
			lblLnaGain3.Text = "G3";
			lblLnaGain3.TextAlign = ContentAlignment.MiddleCenter;
			lblLnaGain4.BackColor = Color.Transparent;
			lblLnaGain4.BorderStyle = BorderStyle.Fixed3D;
			lblLnaGain4.Location = new Point(474, 48);
			lblLnaGain4.Margin = new Padding(0, 0, 0, 3);
			lblLnaGain4.Name = "lblLnaGain4";
			lblLnaGain4.Size = new Size(100, 20);
			lblLnaGain4.TabIndex = 17;
			lblLnaGain4.Text = "G4";
			lblLnaGain4.TextAlign = ContentAlignment.MiddleCenter;
			lblLnaGain5.BackColor = Color.Transparent;
			lblLnaGain5.BorderStyle = BorderStyle.Fixed3D;
			lblLnaGain5.Location = new Point(574, 48);
			lblLnaGain5.Margin = new Padding(0, 0, 0, 3);
			lblLnaGain5.Name = "lblLnaGain5";
			lblLnaGain5.Size = new Size(100, 20);
			lblLnaGain5.TabIndex = 18;
			lblLnaGain5.Text = "G5";
			lblLnaGain5.TextAlign = ContentAlignment.MiddleCenter;
			lblLnaGain6.BackColor = Color.Transparent;
			lblLnaGain6.BorderStyle = BorderStyle.Fixed3D;
			lblLnaGain6.Location = new Point(674, 48);
			lblLnaGain6.Margin = new Padding(0, 0, 0, 3);
			lblLnaGain6.Name = "lblLnaGain6";
			lblLnaGain6.Size = new Size(100, 20);
			lblLnaGain6.TabIndex = 19;
			lblLnaGain6.Text = "G6";
			lblLnaGain6.TextAlign = ContentAlignment.MiddleCenter;
			lblAgcThresh1.BackColor = Color.Transparent;
			lblAgcThresh1.Location = new Point(224, 32);
			lblAgcThresh1.Margin = new Padding(0, 0, 0, 3);
			lblAgcThresh1.Name = "lblAgcThresh1";
			lblAgcThresh1.Size = new Size(100, 13);
			lblAgcThresh1.TabIndex = 8;
			lblAgcThresh1.Text = "0";
			lblAgcThresh1.TextAlign = ContentAlignment.MiddleCenter;
			lblAgcThresh2.BackColor = Color.Transparent;
			lblAgcThresh2.Location = new Point(324, 32);
			lblAgcThresh2.Margin = new Padding(0, 0, 0, 3);
			lblAgcThresh2.Name = "lblAgcThresh2";
			lblAgcThresh2.Size = new Size(100, 13);
			lblAgcThresh2.TabIndex = 9;
			lblAgcThresh2.Text = "0";
			lblAgcThresh2.TextAlign = ContentAlignment.MiddleCenter;
			lblAgcThresh3.BackColor = Color.Transparent;
			lblAgcThresh3.Location = new Point(424, 32);
			lblAgcThresh3.Margin = new Padding(0, 0, 0, 3);
			lblAgcThresh3.Name = "lblAgcThresh3";
			lblAgcThresh3.Size = new Size(100, 13);
			lblAgcThresh3.TabIndex = 10;
			lblAgcThresh3.Text = "0";
			lblAgcThresh3.TextAlign = ContentAlignment.MiddleCenter;
			lblAgcThresh4.BackColor = Color.Transparent;
			lblAgcThresh4.Location = new Point(524, 32);
			lblAgcThresh4.Margin = new Padding(0, 0, 0, 3);
			lblAgcThresh4.Name = "lblAgcThresh4";
			lblAgcThresh4.Size = new Size(100, 13);
			lblAgcThresh4.TabIndex = 11;
			lblAgcThresh4.Text = "0";
			lblAgcThresh4.TextAlign = ContentAlignment.MiddleCenter;
			lblAgcThresh5.BackColor = Color.Transparent;
			lblAgcThresh5.Location = new Point(624, 32);
			lblAgcThresh5.Margin = new Padding(0, 0, 0, 3);
			lblAgcThresh5.Name = "lblAgcThresh5";
			lblAgcThresh5.Size = new Size(100, 13);
			lblAgcThresh5.TabIndex = 12;
			lblAgcThresh5.Text = "0";
			lblAgcThresh5.TextAlign = ContentAlignment.MiddleCenter;
			label47.AutoSize = true;
			label47.BackColor = Color.Transparent;
			label47.Location = new Point(723, 32);
			label47.Margin = new Padding(0);
			label47.Name = "label47";
			label47.Size = new Size(64, 13);
			label47.TabIndex = 13;
			label47.Text = "-> Pin [dBm]";
			label47.TextAlign = ContentAlignment.MiddleLeft;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(gBoxLnaSettings);
			Controls.Add(panel2);
			Name = "ReceiverViewControl";
			Size = new Size(799, 493);
			((ISupportInitialize)errorProvider).EndInit();
			((ISupportInitialize)nudPreambleDetectorTol).EndInit();
			((ISupportInitialize)nudPreambleDetectorSize).EndInit();
			((ISupportInitialize)nudTimeoutRssi).EndInit();
			((ISupportInitialize)nudAutoRxRestartDelay).EndInit();
			((ISupportInitialize)nudTimeoutSyncWord).EndInit();
			((ISupportInitialize)nudTimeoutPreamble).EndInit();
			((ISupportInitialize)nudRxFilterBwAfc).EndInit();
			((ISupportInitialize)nudRxFilterBw).EndInit();
			((ISupportInitialize)nudOokAverageOffset).EndInit();
			((ISupportInitialize)nudOokPeakThreshStep).EndInit();
			((ISupportInitialize)nudOokFixedThresh).EndInit();
			((ISupportInitialize)nudRssiOffset).EndInit();
			((ISupportInitialize)nudRssiSmoothing).EndInit();
			((ISupportInitialize)nudRssiCollisionThreshold).EndInit();
			((ISupportInitialize)nudRssiThresh).EndInit();
			panel2.ResumeLayout(false);
			gBoxAgc.ResumeLayout(false);
			gBoxAgc.PerformLayout();
			panel5.ResumeLayout(false);
			panel5.PerformLayout();
			((ISupportInitialize)nudAgcStep5).EndInit();
			((ISupportInitialize)nudAgcStep4).EndInit();
			((ISupportInitialize)nudAgcReferenceLevel).EndInit();
			((ISupportInitialize)nudAgcStep3).EndInit();
			((ISupportInitialize)nudAgcStep1).EndInit();
			((ISupportInitialize)nudAgcStep2).EndInit();
			gBoxTimeout.ResumeLayout(false);
			gBoxTimeout.PerformLayout();
			gBoxPreamble.ResumeLayout(false);
			gBoxPreamble.PerformLayout();
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			gBoxRxBw.ResumeLayout(false);
			gBoxRxBw.PerformLayout();
			gBoxRxConfig.ResumeLayout(false);
			gBoxRxConfig.PerformLayout();
			panel4.ResumeLayout(false);
			panel4.PerformLayout();
			gBoxDemodulator.ResumeLayout(false);
			gBoxDemodulator.PerformLayout();
			panel13.ResumeLayout(false);
			panel13.PerformLayout();
			gBoxRssi.ResumeLayout(false);
			gBoxRssi.PerformLayout();
			gBoxAfc.ResumeLayout(false);
			gBoxAfc.PerformLayout();
			panel8.ResumeLayout(false);
			panel8.PerformLayout();
			panel9.ResumeLayout(false);
			panel9.PerformLayout();
			gBoxLnaSettings.ResumeLayout(false);
			gBoxLnaSettings.PerformLayout();
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			panel6.ResumeLayout(false);
			panel6.PerformLayout();
			ResumeLayout(false);
		}

		public ReceiverViewControl()
		{
			InitializeComponent();
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

		private void OnRestartRxOnCollisionOnChanged(bool value)
		{
            RestartRxOnCollisionOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnRestartRxWithoutPllLockChanged()
		{
            RestartRxWithoutPllLockChanged?.Invoke(this, EventArgs.Empty);
        }

		private void OnRestartRxWithPllLockChanged()
		{
            RestartRxWithPllLockChanged?.Invoke(this, EventArgs.Empty);
        }

		private void OnAfcAutoOnChanged(bool value)
		{
            AfcAutoOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnAgcAutoOnChanged(bool value)
		{
            AgcAutoOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnRxTriggerChanged(RxTriggerEnum value)
		{
            RxTriggerChanged?.Invoke(this, new RxTriggerEventArg(value));
        }

		private void OnRssiOffsetChanged(decimal value)
		{
            RssiOffsetChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnRssiSmoothingChanged(decimal value)
		{
            RssiSmoothingChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnRssiCollisionThresholdChanged(decimal value)
		{
            RssiCollisionThresholdChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnRssiThreshChanged(decimal value)
		{
            RssiThreshChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnRxBwChanged(decimal value)
		{
            RxBwChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnAfcRxBwChanged(decimal value)
		{
            AfcRxBwChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnBitSyncOnChanged(bool value)
		{
            BitSyncOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnOokThreshTypeChanged(OokThreshTypeEnum value)
		{
            OokThreshTypeChanged?.Invoke(this, new OokThreshTypeEventArg(value));
        }

		private void OnOokPeakThreshStepChanged(decimal value)
		{
            OokPeakThreshStepChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnOokPeakThreshDecChanged(OokPeakThreshDecEnum value)
		{
            OokPeakThreshDecChanged?.Invoke(this, new OokPeakThreshDecEventArg(value));
        }

		private void OnOokAverageBiasChanged(decimal value)
		{
            OokAverageBiasChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnOokAverageThreshFiltChanged(OokAverageThreshFiltEnum value)
		{
            OokAverageThreshFiltChanged?.Invoke(this, new OokAverageThreshFiltEventArg(value));
        }

		private void OnOokFixedThreshChanged(byte value)
		{
            OokFixedThreshChanged?.Invoke(this, new ByteEventArg(value));
        }

		private void OnAgcStartChanged()
		{
            AgcStartChanged?.Invoke(this, EventArgs.Empty);
        }

		private void OnFeiReadChanged()
		{
            FeiReadChanged?.Invoke(this, EventArgs.Empty);
        }

		private void OnAfcAutoClearOnChanged(bool value)
		{
            AfcAutoClearOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnAfcClearChanged()
		{
            AfcClearChanged?.Invoke(this, EventArgs.Empty);
        }

		private void OnPreambleDetectorOnChanged(bool value)
		{
            PreambleDetectorOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnPreambleDetectorSizeChanged(byte value)
		{
            PreambleDetectorSizeChanged?.Invoke(this, new ByteEventArg(value));
        }

		private void OnPreambleDetectorTolChanged(byte value)
		{
            PreambleDetectorTolChanged?.Invoke(this, new ByteEventArg(value));
        }

		private void OnTimeoutRssiChanged(decimal value)
		{
            TimeoutRssiChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnTimeoutPreambleChanged(decimal value)
		{
            TimeoutPreambleChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnTimeoutSyncWordChanged(decimal value)
		{
            TimeoutSyncWordChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnAutoRxRestartDelayChanged(decimal value)
		{
            AutoRxRestartDelayChanged?.Invoke(this, new DecimalEventArg(value));
        }

		public void UpdateRxBwLimits(LimitCheckStatusEnum status, string message)
		{
			switch (status)
			{
			case LimitCheckStatusEnum.OK:
				nudRxFilterBw.BackColor = SystemColors.Window;
				break;
			case LimitCheckStatusEnum.OUT_OF_RANGE:
				nudRxFilterBw.BackColor = ControlPaint.LightLight(Color.Orange);
				break;
			case LimitCheckStatusEnum.ERROR:
				nudRxFilterBw.BackColor = ControlPaint.LightLight(Color.Red);
				break;
			}
			errorProvider.SetError(nudRxFilterBw, message);
		}

		private void nudAgcReferenceLevel_ValueChanged(object sender, EventArgs e)
		{
			AgcReferenceLevel = (int)nudAgcReferenceLevel.Value;
			OnAgcReferenceLevelChanged(AgcReferenceLevel);
		}

		private void nudAgcStep_ValueChanged(object sender, EventArgs e)
		{
			byte value = 0;
			byte id = 0;
			if (sender == nudAgcStep1)
			{
				var b2 = (AgcStep1 = (byte)nudAgcStep1.Value);
				value = b2;
				id = 1;
			}
			else if (sender == nudAgcStep2)
			{
				var b4 = (AgcStep2 = (byte)nudAgcStep2.Value);
				value = b4;
				id = 2;
			}
			else if (sender == nudAgcStep3)
			{
				var b6 = (AgcStep3 = (byte)nudAgcStep3.Value);
				value = b6;
				id = 3;
			}
			else if (sender == nudAgcStep4)
			{
				var b8 = (AgcStep4 = (byte)nudAgcStep4.Value);
				value = b8;
				id = 4;
			}
			else if (sender == nudAgcStep5)
			{
				var b10 = (AgcStep5 = (byte)nudAgcStep5.Value);
				value = b10;
				id = 5;
			}
			OnAgcStepChanged(id, value);
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

		private void rBtnRestartRxOnCollisionOn_CheckedChanged(object sender, EventArgs e)
		{
			RestartRxOnCollision = rBtnRestartRxOnCollisionOn.Checked;
			OnRestartRxOnCollisionOnChanged(RestartRxOnCollision);
		}

		private void btnRestartRxWithoutPllLock_Click(object sender, EventArgs e)
		{
			OnRestartRxWithoutPllLockChanged();
		}

		private void btnRestartRxWithPllLock_Click(object sender, EventArgs e)
		{
			OnRestartRxWithPllLockChanged();
		}

		private void rBtnAfcAutoOn_CheckedChanged(object sender, EventArgs e)
		{
			AfcAutoOn = rBtnAfcAutoOn.Checked;
			OnAfcAutoOnChanged(AfcAutoOn);
		}

		private void rBtnAgcAutoOn_CheckedChanged(object sender, EventArgs e)
		{
			AgcAutoOn = rBtnAgcAutoOn.Checked;
			OnAgcAutoOnChanged(AgcAutoOn);
		}

		private void cBoxRxTrigger_SelectedIndexChanged(object sender, EventArgs e)
		{
			RxTrigger = (RxTriggerEnum)cBoxRxTrigger.SelectedIndex;
			OnRxTriggerChanged(RxTrigger);
		}

		private void nudRssiOffset_ValueChanged(object sender, EventArgs e)
		{
			RssiOffset = nudRssiOffset.Value;
			OnRssiOffsetChanged(RssiOffset);
		}

		private void nudRssiSmoothing_ValueChanged(object sender, EventArgs e)
		{
			var num = (int)Math.Log((double)RssiSmoothing, 2.0);
			var num2 = (int)Math.Log((double)nudRssiSmoothing.Value, 2.0);
			var num3 = (int)(nudRssiSmoothing.Value - RssiSmoothing);
			nudRssiSmoothing.ValueChanged -= nudRssiSmoothing_ValueChanged;
			if (num == 0)
			{
				num3 = 0;
			}
			if (num3 is >= 0 and <= 1)
			{
				nudRssiSmoothing.Value = (decimal)Math.Pow(2.0, num2 + num3);
			}
			else
			{
				nudRssiSmoothing.Value = (decimal)Math.Pow(2.0, num2);
			}
			nudRssiSmoothing.ValueChanged += nudRssiSmoothing_ValueChanged;
			RssiSmoothing = nudRssiSmoothing.Value;
			OnRssiSmoothingChanged(RssiSmoothing);
		}

		private void nudRssiCollisionThreshold_ValueChanged(object sender, EventArgs e)
		{
			RssiCollisionThreshold = nudRssiCollisionThreshold.Value;
			OnRssiCollisionThresholdChanged(RssiCollisionThreshold);
		}

		private void nudRssiThresh_ValueChanged(object sender, EventArgs e)
		{
			RssiThreshold = nudRssiThresh.Value;
			OnRssiThreshChanged(RssiThreshold);
		}

		private void nudRxFilterBw_ValueChanged(object sender, EventArgs e)
		{
			var array = SX1276.ComputeRxBwFreqTable(FrequencyXo, ModulationType);
			var num = 0;
			var num2 = (int)(nudRxFilterBw.Value - RxBw);
			if (num2 is >= -1 and <= 1)
			{
				num = Array.IndexOf(array, RxBw) - num2;
			}
			else
			{
				var mant = 0;
				var exp = 0;
				var num3 = 0m;
				SX1276.ComputeRxBwMantExp(FrequencyXo, ModulationType, nudRxFilterBw.Value, ref mant, ref exp);
				num3 = SX1276.ComputeRxBw(FrequencyXo, ModulationType, mant, exp);
				num = Array.IndexOf(array, num3);
			}
			nudRxFilterBw.ValueChanged -= nudRxFilterBw_ValueChanged;
			nudRxFilterBw.Value = array[num];
			nudRxFilterBw.ValueChanged += nudRxFilterBw_ValueChanged;
			RxBw = nudRxFilterBw.Value;
			OnRxBwChanged(RxBw);
		}

		private void nudRxFilterBwAfc_ValueChanged(object sender, EventArgs e)
		{
			var array = SX1276.ComputeRxBwFreqTable(FrequencyXo, ModulationType);
			var num = 0;
			var num2 = (int)(nudRxFilterBwAfc.Value - AfcRxBw);
			if (num2 is >= -1 and <= 1)
			{
				num = Array.IndexOf(array, AfcRxBw) - num2;
			}
			else
			{
				var mant = 0;
				var exp = 0;
				var num3 = 0m;
				SX1276.ComputeRxBwMantExp(FrequencyXo, ModulationType, nudRxFilterBwAfc.Value, ref mant, ref exp);
				num3 = SX1276.ComputeRxBw(FrequencyXo, ModulationType, mant, exp);
				num = Array.IndexOf(array, num3);
			}
			nudRxFilterBwAfc.ValueChanged -= nudRxFilterBwAfc_ValueChanged;
			nudRxFilterBwAfc.Value = array[num];
			nudRxFilterBwAfc.ValueChanged += nudRxFilterBwAfc_ValueChanged;
			AfcRxBw = nudRxFilterBwAfc.Value;
			OnAfcRxBwChanged(AfcRxBw);
		}

		private void rBtnBitSyncOn_CheckedChanged(object sender, EventArgs e)
		{
			BitSyncOn = rBtnBitSyncOn.Checked;
			OnBitSyncOnChanged(BitSyncOn);
		}

		private void cBoxOokThreshType_SelectedIndexChanged(object sender, EventArgs e)
		{
			OokThreshType = (OokThreshTypeEnum)cBoxOokThreshType.SelectedIndex;
			OnOokThreshTypeChanged(OokThreshType);
		}

		private void nudOokPeakThreshStep_Validating(object sender, CancelEventArgs e)
		{
			_ = nudOokPeakThreshStep.Value < 2m;
		}

		private void nudOokPeakThreshStep_ValueChanged(object sender, EventArgs e)
		{
			try
			{
				nudOokPeakThreshStep.ValueChanged -= nudOokPeakThreshStep_ValueChanged;
				var array = new decimal[8] { 0.5m, 1.0m, 1.5m, 2.0m, 3.0m, 4.0m, 5.0m, 6.0m };
				var num = 0;
				var num2 = nudOokPeakThreshStep.Value - OokPeakThreshStep;
				var num3 = 10000000m;
				for (var i = 0; i < 8; i++)
				{
					if (Math.Abs(nudOokPeakThreshStep.Value - array[i]) < num3)
					{
						num3 = Math.Abs(nudOokPeakThreshStep.Value - array[i]);
						num = i;
					}
				}
				if (num3 / Math.Abs(num2) == 1m && num2 >= 0.5m)
				{
					if (num2 > 0m)
					{
						nudOokPeakThreshStep.Value += nudOokPeakThreshStep.Increment;
					}
					else
					{
						nudOokPeakThreshStep.Value -= nudOokPeakThreshStep.Increment;
					}
					num = Array.IndexOf(array, nudOokPeakThreshStep.Value);
				}
				nudOokPeakThreshStep.Value = array[num];
				nudOokPeakThreshStep.ValueChanged += nudOokPeakThreshStep_ValueChanged;
				OokPeakThreshStep = nudOokPeakThreshStep.Value;
				OnOokPeakThreshStepChanged(OokPeakThreshStep);
			}
			catch
			{
				nudOokPeakThreshStep.ValueChanged += nudOokPeakThreshStep_ValueChanged;
			}
		}

		private void cBoxOokPeakThreshDec_SelectedIndexChanged(object sender, EventArgs e)
		{
			OokPeakThreshDec = (OokPeakThreshDecEnum)cBoxOokPeakThreshDec.SelectedIndex;
			OnOokPeakThreshDecChanged(OokPeakThreshDec);
		}

		private void cBoxOokAverageThreshFilt_SelectedIndexChanged(object sender, EventArgs e)
		{
			OokAverageThreshFilt = (OokAverageThreshFiltEnum)cBoxOokAverageThreshFilt.SelectedIndex;
			OnOokAverageThreshFiltChanged(OokAverageThreshFilt);
		}

		private void nudOokAverageOffset_ValueChanged(object sender, EventArgs e)
		{
			OokAverageOffset = nudOokAverageOffset.Value;
			OnOokAverageBiasChanged(OokAverageOffset);
		}

		private void nudOokFixedThresh_ValueChanged(object sender, EventArgs e)
		{
			OokFixedThreshold = (byte)nudOokFixedThresh.Value;
			OnOokFixedThreshChanged(OokFixedThreshold);
		}

		private void btnAgcStart_Click(object sender, EventArgs e)
		{
			OnAgcStartChanged();
		}

		private void btnFeiMeasure_Click(object sender, EventArgs e)
		{
			OnFeiReadChanged();
		}

		private void rBtnAfcAutoClearOn_CheckedChanged(object sender, EventArgs e)
		{
			AfcAutoClearOn = rBtnAfcAutoClearOn.Checked;
			OnAfcAutoClearOnChanged(AfcAutoClearOn);
		}

		private void btnAfcClear_Click(object sender, EventArgs e)
		{
			OnAfcClearChanged();
		}

		private void rBtnPreambleDetectorOn_CheckedChanged(object sender, EventArgs e)
		{
			PreambleDetectorOn = rBtnPreambleDetectorOn.Checked;
			OnPreambleDetectorOnChanged(PreambleDetectorOn);
		}

		private void nudPreambleDetectorSize_ValueChanged(object sender, EventArgs e)
		{
			PreambleDetectorSize = (byte)nudPreambleDetectorSize.Value;
			OnPreambleDetectorSizeChanged(PreambleDetectorSize);
		}

		private void nudPreambleDetectorTol_ValueChanged(object sender, EventArgs e)
		{
			PreambleDetectorTol = (byte)nudPreambleDetectorTol.Value;
			OnPreambleDetectorTolChanged(PreambleDetectorTol);
		}

		private void nudTimeoutRssi_ValueChanged(object sender, EventArgs e)
		{
			TimeoutRxRssi = nudTimeoutRssi.Value;
			OnTimeoutRssiChanged(TimeoutRxRssi);
		}

		private void nudTimeoutPreamble_ValueChanged(object sender, EventArgs e)
		{
			TimeoutRxPreamble = nudTimeoutPreamble.Value;
			OnTimeoutPreambleChanged(TimeoutRxPreamble);
		}

		private void nudTimeoutSyncWord_ValueChanged(object sender, EventArgs e)
		{
			TimeoutSignalSync = nudTimeoutSyncWord.Value;
			OnTimeoutSyncWordChanged(TimeoutSignalSync);
		}

		private void nudAutoRxRestartDelay_ValueChanged(object sender, EventArgs e)
		{
			InterPacketRxDelay = nudAutoRxRestartDelay.Value;
			OnAutoRxRestartDelayChanged(InterPacketRxDelay);
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
			if (sender == gBoxRxBw)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Receiver", "Bandwidth"));
			}
			else if (sender == gBoxDemodulator)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Receiver", "Demodulator"));
			}
			else if (sender == gBoxAfc)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Receiver", "Afc"));
			}
			else if (sender == gBoxRssi)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Receiver", "Rssi"));
			}
			else if (sender == gBoxLnaSettings)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Receiver", "Lna"));
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
	}
}
