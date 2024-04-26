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
	public sealed class CommonViewControl : UserControl, INotifyDocumentationChanged
	{
		private IContainer components;

		private Button btnRcCalibration;

		private NumericUpDownEx nudFrequencyXo;

		private ComboBox cBoxLowBatTrim;

		private NumericUpDownEx nudBitrate;

		private NumericUpDownEx nudFrequencyRf;

		private NumericUpDownEx nudFdev;

		private Panel panel4;

		private RadioButton rBtnLowBatOff;

		private RadioButton rBtnLowBatOn;

		private Label label1;

		private Panel panel2;

		private RadioButton rBtnModulationTypeOok;

		private RadioButton rBtnModulationTypeFsk;

		private Panel panel3;

		private RadioButton rBtnModulationShaping11;

		private RadioButton rBtnModulationShaping10;

		private RadioButton rBtnModulationShaping01;

		private RadioButton rBtnModulationShapingOff;

		private Label label5;

		private Label label7;

		private Label label6;

		private Label label10;

		private Label label9;

		private Label label14;

		private Label label16;

		private Label lblRcOscillatorCalStat;

		private Label lblRcOscillatorCal;

		private Label label15;

		private Label label13;

		private Label label17;

		private Label label11;

		private Label label8;

		private Label label12;

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

		private GroupBoxEx gBoxOscillators;

		private GroupBoxEx gBoxModulation;

		private GroupBoxEx gBoxGeneral;

		private GroupBoxEx gBoxBatteryManagement;

		private NumericUpDownEx nudBitrateFrac;

		private Label label2;

		private Panel panel1;

		private RadioButton rBtnTcxoInputOff;

		private RadioButton rBtnTcxoInputOn;

		private Label label3;

		private Panel panel5;

		private RadioButton rBtnFastHopOff;

		private RadioButton rBtnFastHopOn;

		private Panel panel8;

		private RadioButton rBtnLowFrequencyModeOff;

		private RadioButton rBtnLowFrequencyModeOn;

		private ComboBox cBoxBand;

		private Label label19;

		private Label label18;

		private Label label4;

		private GroupBoxEx groupBoxEx1;

		private Panel panel10;

		private RadioButton rBtnForceRxBandLowFrequencyOff;

		private RadioButton rBtnForceRxBandLowFrequencyOn;

		private Label label31;

		private Panel panel9;

		private RadioButton rBtnForceTxBandLowFrequencyOff;

		private RadioButton rBtnForceTxBandLowFrequencyOn;

		private Label label29;

		private GroupBoxEx groupBoxEx2;

		private decimal bitRate = 4800m;

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
			set
			{
				nudFrequencyRf.Increment = value;
				nudFdev.Increment = value;
			}
		}

		public ModulationTypeEnum ModulationType
		{
			get
			{
				if (rBtnModulationTypeFsk.Checked)
				{
					return ModulationTypeEnum.FSK;
				}
				return rBtnModulationTypeOok.Checked ? ModulationTypeEnum.OOK : ModulationTypeEnum.FSK;
			}
			set
			{
				rBtnModulationTypeFsk.CheckedChanged -= rBtnModulationType_CheckedChanged;
				rBtnModulationTypeOok.CheckedChanged -= rBtnModulationType_CheckedChanged;
				switch (value)
				{
				case ModulationTypeEnum.FSK:
					rBtnModulationTypeFsk.Checked = true;
					rBtnModulationTypeOok.Checked = false;
					rBtnModulationShapingOff.Text = "OFF";
					rBtnModulationShaping01.Text = "Gaussian filter, BT = 1.0";
					rBtnModulationShaping10.Text = "Gaussian filter, BT = 0.5";
					rBtnModulationShaping11.Text = "Gaussian filter, BT = 0.3";
					rBtnModulationShaping11.Visible = true;
					nudBitrateFrac.Enabled = true;
					break;
				case ModulationTypeEnum.OOK:
					rBtnModulationTypeFsk.Checked = false;
					rBtnModulationTypeOok.Checked = true;
					rBtnModulationShapingOff.Text = "OFF";
					rBtnModulationShaping01.Text = "Filtering with fCutOff = BR";
					rBtnModulationShaping10.Text = "Filtering with fCutOff = 2 * BR";
					rBtnModulationShaping11.Text = "Unused";
					rBtnModulationShaping11.Visible = false;
					nudBitrateFrac.Enabled = false;
					break;
				}
				rBtnModulationTypeFsk.CheckedChanged += rBtnModulationType_CheckedChanged;
				rBtnModulationTypeOok.CheckedChanged += rBtnModulationType_CheckedChanged;
			}
		}

		public byte ModulationShaping
		{
			get
			{
				if (rBtnModulationShapingOff.Checked)
				{
					return 0;
				}
				if (rBtnModulationShaping01.Checked)
				{
					return 1;
				}
				return rBtnModulationShaping10.Checked ? (byte)2 : (byte)3;
			}
			set
			{
				rBtnModulationShapingOff.CheckedChanged -= rBtnModulationShaping_CheckedChanged;
				rBtnModulationShaping01.CheckedChanged -= rBtnModulationShaping_CheckedChanged;
				rBtnModulationShaping10.CheckedChanged -= rBtnModulationShaping_CheckedChanged;
				rBtnModulationShaping11.CheckedChanged -= rBtnModulationShaping_CheckedChanged;
				switch (value)
				{
				case 0:
					rBtnModulationShapingOff.Checked = true;
					rBtnModulationShaping01.Checked = false;
					rBtnModulationShaping10.Checked = false;
					rBtnModulationShaping11.Checked = false;
					break;
				case 1:
					rBtnModulationShapingOff.Checked = false;
					rBtnModulationShaping01.Checked = true;
					rBtnModulationShaping10.Checked = false;
					rBtnModulationShaping11.Checked = false;
					break;
				case 2:
					rBtnModulationShapingOff.Checked = false;
					rBtnModulationShaping01.Checked = false;
					rBtnModulationShaping10.Checked = true;
					rBtnModulationShaping11.Checked = false;
					break;
				case 3:
					rBtnModulationShapingOff.Checked = false;
					rBtnModulationShaping01.Checked = false;
					rBtnModulationShaping10.Checked = false;
					rBtnModulationShaping11.Checked = true;
					break;
				}
				rBtnModulationShapingOff.CheckedChanged += rBtnModulationShaping_CheckedChanged;
				rBtnModulationShaping01.CheckedChanged += rBtnModulationShaping_CheckedChanged;
				rBtnModulationShaping10.CheckedChanged += rBtnModulationShaping_CheckedChanged;
				rBtnModulationShaping11.CheckedChanged += rBtnModulationShaping_CheckedChanged;
			}
		}

		public decimal Bitrate
		{
			get => bitRate;
			set
			{
				try
				{
					nudBitrate.ValueChanged -= nudBitrate_ValueChanged;
					var num = (ushort)Math.Round(FrequencyXo / value - BitrateFrac / 16m, MidpointRounding.AwayFromZero);
					bitRate = Math.Round(FrequencyXo / (num + BitrateFrac / 16m), MidpointRounding.AwayFromZero);
					nudBitrate.Value = bitRate;
				}
				catch (Exception)
				{
					nudBitrate.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudBitrate.ValueChanged += nudBitrate_ValueChanged;
				}
			}
		}

		public decimal BitrateFrac
		{
			get => nudBitrateFrac.Value;
			set
			{
				try
				{
					nudBitrateFrac.ValueChanged -= nudBitrateFrac_ValueChanged;
					nudBitrateFrac.Value = value;
				}
				catch (Exception)
				{
					nudBitrateFrac.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudBitrateFrac.ValueChanged += nudBitrateFrac_ValueChanged;
				}
			}
		}

		public decimal Fdev
		{
			get => nudFdev.Value;
			set
			{
				try
				{
					nudFdev.ValueChanged -= nudFdev_ValueChanged;
					var num = (ushort)Math.Round(value / FrequencyStep, MidpointRounding.AwayFromZero);
					nudFdev.Value = num * FrequencyStep;
				}
				catch (Exception)
				{
					nudFdev.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudFdev.ValueChanged += nudFdev_ValueChanged;
				}
			}
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
				}
				else
				{
					rBtnLowFrequencyModeOn.Checked = false;
					rBtnLowFrequencyModeOff.Checked = true;
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

		public bool LowBatOn
		{
			get => rBtnLowBatOn.Checked;
			set
			{
				rBtnLowBatOn.CheckedChanged -= rBtnLowBatOn_CheckedChanged;
				rBtnLowBatOff.CheckedChanged -= rBtnLowBatOn_CheckedChanged;
				if (value)
				{
					rBtnLowBatOn.Checked = true;
					rBtnLowBatOff.Checked = false;
				}
				else
				{
					rBtnLowBatOn.Checked = false;
					rBtnLowBatOff.Checked = true;
				}
				rBtnLowBatOn.CheckedChanged += rBtnLowBatOn_CheckedChanged;
				rBtnLowBatOff.CheckedChanged += rBtnLowBatOn_CheckedChanged;
			}
		}

		public LowBatTrimEnum LowBatTrim
		{
			get => (LowBatTrimEnum)cBoxLowBatTrim.SelectedIndex;
			set
			{
				cBoxLowBatTrim.SelectedIndexChanged -= cBoxLowBatTrim_SelectedIndexChanged;
				cBoxLowBatTrim.SelectedIndex = (int)value;
				cBoxLowBatTrim.SelectedIndexChanged += cBoxLowBatTrim_SelectedIndexChanged;
			}
		}

		public event DecimalEventHandler FrequencyXoChanged;

		public event ModulationTypeEventHandler ModulationTypeChanged;

		public event ByteEventHandler ModulationShapingChanged;

		public event DecimalEventHandler BitrateChanged;

		public event DecimalEventHandler BitrateFracChanged;

		public event DecimalEventHandler FdevChanged;

		public event BandEventHandler BandChanged;

		public event BooleanEventHandler ForceTxBandLowFrequencyOnChanged;

		public event BooleanEventHandler ForceRxBandLowFrequencyOnChanged;

		public event BooleanEventHandler LowFrequencyModeOnChanged;

		public event DecimalEventHandler FrequencyRfChanged;

		public event BooleanEventHandler FastHopOnChanged;

		public event BooleanEventHandler TcxoInputChanged;

		public event EventHandler RcCalibrationChanged;

		public event BooleanEventHandler LowBatOnChanged;

		public event LowBatTrimEventHandler LowBatTrimChanged;

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
			nudBitrate = new NumericUpDownEx();
			nudFdev = new NumericUpDownEx();
			nudFrequencyRf = new NumericUpDownEx();
			lblListenResolRx = new Label();
			label30 = new Label();
			gBoxBatteryManagement = new GroupBoxEx();
			panel4 = new Panel();
			rBtnLowBatOff = new RadioButton();
			rBtnLowBatOn = new RadioButton();
			label17 = new Label();
			label15 = new Label();
			label16 = new Label();
			cBoxLowBatTrim = new ComboBox();
			gBoxOscillators = new GroupBoxEx();
			panel1 = new Panel();
			rBtnTcxoInputOff = new RadioButton();
			rBtnTcxoInputOn = new RadioButton();
			nudFrequencyXo = new NumericUpDownEx();
			label9 = new Label();
			btnRcCalibration = new Button();
			label1 = new Label();
			lblRcOscillatorCalStat = new Label();
			lblRcOscillatorCal = new Label();
			gBoxModulation = new GroupBoxEx();
			panel2 = new Panel();
			rBtnModulationTypeOok = new RadioButton();
			rBtnModulationTypeFsk = new RadioButton();
			label6 = new Label();
			label5 = new Label();
			panel3 = new Panel();
			rBtnModulationShaping11 = new RadioButton();
			rBtnModulationShaping10 = new RadioButton();
			rBtnModulationShaping01 = new RadioButton();
			rBtnModulationShapingOff = new RadioButton();
			gBoxGeneral = new GroupBoxEx();
			label3 = new Label();
			panel5 = new Panel();
			rBtnFastHopOff = new RadioButton();
			rBtnFastHopOn = new RadioButton();
			nudBitrateFrac = new NumericUpDownEx();
			label12 = new Label();
			label8 = new Label();
			label11 = new Label();
			label13 = new Label();
			label14 = new Label();
			label2 = new Label();
			label10 = new Label();
			label7 = new Label();
			panel8 = new Panel();
			rBtnLowFrequencyModeOff = new RadioButton();
			rBtnLowFrequencyModeOn = new RadioButton();
			cBoxBand = new ComboBox();
			label19 = new Label();
			label18 = new Label();
			label4 = new Label();
			groupBoxEx1 = new GroupBoxEx();
			panel10 = new Panel();
			rBtnForceRxBandLowFrequencyOff = new RadioButton();
			rBtnForceRxBandLowFrequencyOn = new RadioButton();
			label31 = new Label();
			panel9 = new Panel();
			rBtnForceTxBandLowFrequencyOff = new RadioButton();
			rBtnForceTxBandLowFrequencyOn = new RadioButton();
			label29 = new Label();
			groupBoxEx2 = new GroupBoxEx();
			((ISupportInitialize)errorProvider).BeginInit();
			((ISupportInitialize)nudBitrate).BeginInit();
			((ISupportInitialize)nudFdev).BeginInit();
			((ISupportInitialize)nudFrequencyRf).BeginInit();
			gBoxBatteryManagement.SuspendLayout();
			panel4.SuspendLayout();
			gBoxOscillators.SuspendLayout();
			panel1.SuspendLayout();
			((ISupportInitialize)nudFrequencyXo).BeginInit();
			gBoxModulation.SuspendLayout();
			panel2.SuspendLayout();
			panel3.SuspendLayout();
			gBoxGeneral.SuspendLayout();
			panel5.SuspendLayout();
			((ISupportInitialize)nudBitrateFrac).BeginInit();
			panel8.SuspendLayout();
			groupBoxEx1.SuspendLayout();
			panel10.SuspendLayout();
			panel9.SuspendLayout();
			groupBoxEx2.SuspendLayout();
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
			errorProvider.SetIconPadding(nudBitrate, 30);
			nudBitrate.Location = new Point(164, 68);
			nudBitrate.Maximum = new decimal([603774, 0, 0, 0]);
			nudBitrate.Minimum = new decimal([600, 0, 0, 0]);
			nudBitrate.Name = "nudBitrate";
			nudBitrate.Size = new Size(124, 20);
			nudBitrate.TabIndex = 4;
			nudBitrate.ThousandsSeparator = true;
			nudBitrate.Value = new decimal([4800, 0, 0, 0]);
			nudBitrate.ValueChanged += nudBitrate_ValueChanged;
			errorProvider.SetIconPadding(nudFdev, 30);
			nudFdev.Increment = new decimal([61, 0, 0, 0]);
			nudFdev.Location = new Point(164, 119);
			nudFdev.Maximum = new decimal([300000, 0, 0, 0]);
			nudFdev.Name = "nudFdev";
			nudFdev.Size = new Size(124, 20);
			nudFdev.TabIndex = 8;
			nudFdev.ThousandsSeparator = true;
			nudFdev.Value = new decimal([5005, 0, 0, 0]);
			nudFdev.ValueChanged += nudFdev_ValueChanged;
			errorProvider.SetIconPadding(nudFrequencyRf, 30);
			nudFrequencyRf.Increment = new decimal([61, 0, 0, 0]);
			nudFrequencyRf.Location = new Point(164, 19);
			nudFrequencyRf.Maximum = new decimal([2040000000, 0, 0, 0]);
			nudFrequencyRf.Minimum = new decimal([100000000, 0, 0, 0]);
			nudFrequencyRf.Name = "nudFrequencyRf";
			nudFrequencyRf.Size = new Size(124, 20);
			nudFrequencyRf.TabIndex = 1;
			nudFrequencyRf.ThousandsSeparator = true;
			nudFrequencyRf.Value = new decimal([915000000, 0, 0, 0]);
			nudFrequencyRf.ValueChanged += nudFrequencyRf_ValueChanged;
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
			gBoxBatteryManagement.Controls.Add(panel4);
			gBoxBatteryManagement.Controls.Add(label17);
			gBoxBatteryManagement.Controls.Add(label15);
			gBoxBatteryManagement.Controls.Add(label16);
			gBoxBatteryManagement.Controls.Add(cBoxLowBatTrim);
			gBoxBatteryManagement.Location = new Point(222, 410);
			gBoxBatteryManagement.Name = "gBoxBatteryManagement";
			gBoxBatteryManagement.Size = new Size(355, 80);
			gBoxBatteryManagement.TabIndex = 5;
			gBoxBatteryManagement.TabStop = false;
			gBoxBatteryManagement.Text = "Battery management";
			gBoxBatteryManagement.MouseEnter += control_MouseEnter;
			gBoxBatteryManagement.MouseLeave += control_MouseLeave;
			panel4.AutoSize = true;
			panel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel4.Controls.Add(rBtnLowBatOff);
			panel4.Controls.Add(rBtnLowBatOn);
			panel4.Location = new Point(164, 19);
			panel4.Name = "panel4";
			panel4.Size = new Size(102, 20);
			panel4.TabIndex = 1;
			rBtnLowBatOff.AutoSize = true;
			rBtnLowBatOff.Location = new Point(54, 3);
			rBtnLowBatOff.Margin = new Padding(3, 0, 3, 0);
			rBtnLowBatOff.Name = "rBtnLowBatOff";
			rBtnLowBatOff.Size = new Size(45, 17);
			rBtnLowBatOff.TabIndex = 1;
			rBtnLowBatOff.Text = "OFF";
			rBtnLowBatOff.UseVisualStyleBackColor = true;
			rBtnLowBatOn.AutoSize = true;
			rBtnLowBatOn.Checked = true;
			rBtnLowBatOn.Location = new Point(3, 3);
			rBtnLowBatOn.Margin = new Padding(3, 0, 3, 0);
			rBtnLowBatOn.Name = "rBtnLowBatOn";
			rBtnLowBatOn.Size = new Size(41, 17);
			rBtnLowBatOn.TabIndex = 0;
			rBtnLowBatOn.TabStop = true;
			rBtnLowBatOn.Text = "ON";
			rBtnLowBatOn.UseVisualStyleBackColor = true;
			label17.AutoSize = true;
			label17.Location = new Point(6, 48);
			label17.Name = "label17";
			label17.Size = new Size(130, 13);
			label17.TabIndex = 2;
			label17.Text = "Low battery threshold trim:";
			label15.AutoSize = true;
			label15.Location = new Point(6, 24);
			label15.Name = "label15";
			label15.Size = new Size(107, 13);
			label15.TabIndex = 0;
			label15.Text = "Low battery detector:";
			label16.AutoSize = true;
			label16.Location = new Point(294, 49);
			label16.Name = "label16";
			label16.Size = new Size(14, 13);
			label16.TabIndex = 4;
			label16.Text = "V";
			cBoxLowBatTrim.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxLowBatTrim.FormattingEnabled = true;
			cBoxLowBatTrim.Items.AddRange(["1.695", "1.764", "1.835", "1.905", "1.976", "2.045", "2.116", "2.185"]);
			cBoxLowBatTrim.Location = new Point(164, 45);
			cBoxLowBatTrim.Name = "cBoxLowBatTrim";
			cBoxLowBatTrim.Size = new Size(124, 21);
			cBoxLowBatTrim.TabIndex = 3;
			cBoxLowBatTrim.SelectedIndexChanged += cBoxLowBatTrim_SelectedIndexChanged;
			gBoxOscillators.Controls.Add(panel1);
			gBoxOscillators.Controls.Add(nudFrequencyXo);
			gBoxOscillators.Controls.Add(label9);
			gBoxOscillators.Controls.Add(btnRcCalibration);
			gBoxOscillators.Controls.Add(label1);
			gBoxOscillators.Controls.Add(lblRcOscillatorCalStat);
			gBoxOscillators.Controls.Add(lblRcOscillatorCal);
			gBoxOscillators.Location = new Point(222, 304);
			gBoxOscillators.Name = "gBoxOscillators";
			gBoxOscillators.Size = new Size(355, 100);
			gBoxOscillators.TabIndex = 3;
			gBoxOscillators.TabStop = false;
			gBoxOscillators.Text = "Oscillators";
			gBoxOscillators.MouseEnter += control_MouseEnter;
			gBoxOscillators.MouseLeave += control_MouseLeave;
			panel1.AutoSize = true;
			panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel1.Controls.Add(rBtnTcxoInputOff);
			panel1.Controls.Add(rBtnTcxoInputOn);
			panel1.Location = new Point(164, 45);
			panel1.Name = "panel1";
			panel1.Size = new Size(122, 23);
			panel1.TabIndex = 1;
			rBtnTcxoInputOff.AutoSize = true;
			rBtnTcxoInputOff.Location = new Point(63, 3);
			rBtnTcxoInputOff.Name = "rBtnTcxoInputOff";
			rBtnTcxoInputOff.Size = new Size(56, 17);
			rBtnTcxoInputOff.TabIndex = 1;
			rBtnTcxoInputOff.Text = "Crystal";
			rBtnTcxoInputOff.UseVisualStyleBackColor = true;
			rBtnTcxoInputOff.CheckedChanged += rBtnTcxoInput_CheckedChanged;
			rBtnTcxoInputOn.AutoSize = true;
			rBtnTcxoInputOn.Checked = true;
			rBtnTcxoInputOn.Location = new Point(3, 3);
			rBtnTcxoInputOn.Name = "rBtnTcxoInputOn";
			rBtnTcxoInputOn.Size = new Size(54, 17);
			rBtnTcxoInputOn.TabIndex = 0;
			rBtnTcxoInputOn.TabStop = true;
			rBtnTcxoInputOn.Text = "TCXO";
			rBtnTcxoInputOn.UseVisualStyleBackColor = true;
			rBtnTcxoInputOn.CheckedChanged += rBtnTcxoInput_CheckedChanged;
			nudFrequencyXo.Location = new Point(164, 19);
			nudFrequencyXo.Maximum = new decimal([32000000, 0, 0, 0]);
			nudFrequencyXo.Minimum = new decimal([26000000, 0, 0, 0]);
			nudFrequencyXo.Name = "nudFrequencyXo";
			nudFrequencyXo.Size = new Size(124, 20);
			nudFrequencyXo.TabIndex = 1;
			nudFrequencyXo.ThousandsSeparator = true;
			nudFrequencyXo.Value = new decimal([32000000, 0, 0, 0]);
			nudFrequencyXo.ValueChanged += nudFrequencyXo_ValueChanged;
			label9.AutoSize = true;
			label9.Location = new Point(294, 23);
			label9.Name = "label9";
			label9.Size = new Size(20, 13);
			label9.TabIndex = 2;
			label9.Text = "Hz";
			btnRcCalibration.Location = new Point(164, 71);
			btnRcCalibration.Name = "btnRcCalibration";
			btnRcCalibration.Size = new Size(75, 23);
			btnRcCalibration.TabIndex = 4;
			btnRcCalibration.Text = "Calibrate";
			btnRcCalibration.UseVisualStyleBackColor = true;
			btnRcCalibration.Click += btnRcCalibration_Click;
			label1.AutoSize = true;
			label1.Location = new Point(6, 23);
			label1.Name = "label1";
			label1.Size = new Size(78, 13);
			label1.TabIndex = 0;
			label1.Text = "XO Frequency:";
			lblRcOscillatorCalStat.AutoSize = true;
			lblRcOscillatorCalStat.Location = new Point(6, 47);
			lblRcOscillatorCalStat.Name = "lblRcOscillatorCalStat";
			lblRcOscillatorCalStat.Size = new Size(96, 13);
			lblRcOscillatorCalStat.TabIndex = 5;
			lblRcOscillatorCalStat.Text = "XO input selection:";
			lblRcOscillatorCal.AutoSize = true;
			lblRcOscillatorCal.Location = new Point(6, 76);
			lblRcOscillatorCal.Name = "lblRcOscillatorCal";
			lblRcOscillatorCal.Size = new Size(120, 13);
			lblRcOscillatorCal.TabIndex = 3;
			lblRcOscillatorCal.Text = "RC oscillator calibration:";
			gBoxModulation.Controls.Add(panel2);
			gBoxModulation.Controls.Add(label6);
			gBoxModulation.Controls.Add(label5);
			gBoxModulation.Controls.Add(panel3);
			gBoxModulation.Location = new Point(222, 154);
			gBoxModulation.Name = "gBoxModulation";
			gBoxModulation.Size = new Size(355, 144);
			gBoxModulation.TabIndex = 2;
			gBoxModulation.TabStop = false;
			gBoxModulation.Text = "Modulation";
			gBoxModulation.MouseEnter += control_MouseEnter;
			gBoxModulation.MouseLeave += control_MouseLeave;
			panel2.AutoSize = true;
			panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel2.Controls.Add(rBtnModulationTypeOok);
			panel2.Controls.Add(rBtnModulationTypeFsk);
			panel2.Location = new Point(164, 19);
			panel2.Name = "panel2";
			panel2.Size = new Size(105, 23);
			panel2.TabIndex = 1;
			rBtnModulationTypeOok.AutoSize = true;
			rBtnModulationTypeOok.Location = new Point(54, 3);
			rBtnModulationTypeOok.Name = "rBtnModulationTypeOok";
			rBtnModulationTypeOok.Size = new Size(48, 17);
			rBtnModulationTypeOok.TabIndex = 1;
			rBtnModulationTypeOok.Text = "OOK";
			rBtnModulationTypeOok.UseVisualStyleBackColor = true;
			rBtnModulationTypeOok.CheckedChanged += rBtnModulationType_CheckedChanged;
			rBtnModulationTypeFsk.AutoSize = true;
			rBtnModulationTypeFsk.Checked = true;
			rBtnModulationTypeFsk.Location = new Point(3, 3);
			rBtnModulationTypeFsk.Name = "rBtnModulationTypeFsk";
			rBtnModulationTypeFsk.Size = new Size(45, 17);
			rBtnModulationTypeFsk.TabIndex = 0;
			rBtnModulationTypeFsk.TabStop = true;
			rBtnModulationTypeFsk.Text = "FSK";
			rBtnModulationTypeFsk.UseVisualStyleBackColor = true;
			rBtnModulationTypeFsk.CheckedChanged += rBtnModulationType_CheckedChanged;
			label6.AutoSize = true;
			label6.Location = new Point(6, 53);
			label6.Name = "label6";
			label6.Size = new Size(102, 13);
			label6.TabIndex = 2;
			label6.Text = "Modulation shaping:";
			label5.AutoSize = true;
			label5.Location = new Point(6, 24);
			label5.Name = "label5";
			label5.Size = new Size(62, 13);
			label5.TabIndex = 0;
			label5.Text = "Modulation:";
			panel3.AutoSize = true;
			panel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel3.Controls.Add(rBtnModulationShaping11);
			panel3.Controls.Add(rBtnModulationShaping10);
			panel3.Controls.Add(rBtnModulationShaping01);
			panel3.Controls.Add(rBtnModulationShapingOff);
			panel3.Location = new Point(164, 48);
			panel3.Name = "panel3";
			panel3.Size = new Size(144, 92);
			panel3.TabIndex = 3;
			rBtnModulationShaping11.AutoSize = true;
			rBtnModulationShaping11.Location = new Point(3, 72);
			rBtnModulationShaping11.Name = "rBtnModulationShaping11";
			rBtnModulationShaping11.Size = new Size(138, 17);
			rBtnModulationShaping11.TabIndex = 3;
			rBtnModulationShaping11.Text = "Gaussian filter, BT = 0.3";
			rBtnModulationShaping11.UseVisualStyleBackColor = true;
			rBtnModulationShaping11.CheckedChanged += rBtnModulationShaping_CheckedChanged;
			rBtnModulationShaping10.AutoSize = true;
			rBtnModulationShaping10.Location = new Point(3, 49);
			rBtnModulationShaping10.Name = "rBtnModulationShaping10";
			rBtnModulationShaping10.Size = new Size(138, 17);
			rBtnModulationShaping10.TabIndex = 2;
			rBtnModulationShaping10.Text = "Gaussian filter, BT = 0.5";
			rBtnModulationShaping10.UseVisualStyleBackColor = true;
			rBtnModulationShaping10.CheckedChanged += rBtnModulationShaping_CheckedChanged;
			rBtnModulationShaping01.AutoSize = true;
			rBtnModulationShaping01.Location = new Point(3, 26);
			rBtnModulationShaping01.Name = "rBtnModulationShaping01";
			rBtnModulationShaping01.Size = new Size(138, 17);
			rBtnModulationShaping01.TabIndex = 1;
			rBtnModulationShaping01.Text = "Gaussian filter, BT = 1.0";
			rBtnModulationShaping01.UseVisualStyleBackColor = true;
			rBtnModulationShaping01.CheckedChanged += rBtnModulationShaping_CheckedChanged;
			rBtnModulationShapingOff.AutoSize = true;
			rBtnModulationShapingOff.Checked = true;
			rBtnModulationShapingOff.Location = new Point(3, 3);
			rBtnModulationShapingOff.Name = "rBtnModulationShapingOff";
			rBtnModulationShapingOff.Size = new Size(45, 17);
			rBtnModulationShapingOff.TabIndex = 0;
			rBtnModulationShapingOff.TabStop = true;
			rBtnModulationShapingOff.Text = "OFF";
			rBtnModulationShapingOff.UseVisualStyleBackColor = true;
			rBtnModulationShapingOff.CheckedChanged += rBtnModulationShaping_CheckedChanged;
			gBoxGeneral.Controls.Add(label3);
			gBoxGeneral.Controls.Add(panel5);
			gBoxGeneral.Controls.Add(nudBitrateFrac);
			gBoxGeneral.Controls.Add(nudBitrate);
			gBoxGeneral.Controls.Add(label12);
			gBoxGeneral.Controls.Add(label8);
			gBoxGeneral.Controls.Add(label11);
			gBoxGeneral.Controls.Add(label13);
			gBoxGeneral.Controls.Add(label14);
			gBoxGeneral.Controls.Add(label2);
			gBoxGeneral.Controls.Add(label10);
			gBoxGeneral.Controls.Add(label7);
			gBoxGeneral.Controls.Add(nudFdev);
			gBoxGeneral.Controls.Add(nudFrequencyRf);
			gBoxGeneral.Location = new Point(222, 3);
			gBoxGeneral.Name = "gBoxGeneral";
			gBoxGeneral.Size = new Size(355, 145);
			gBoxGeneral.TabIndex = 0;
			gBoxGeneral.TabStop = false;
			gBoxGeneral.Text = "General";
			gBoxGeneral.MouseEnter += control_MouseEnter;
			gBoxGeneral.MouseLeave += control_MouseLeave;
			label3.AutoSize = true;
			label3.Location = new Point(6, 47);
			label3.Name = "label3";
			label3.Size = new Size(71, 13);
			label3.TabIndex = 22;
			label3.Text = "Fast hopping:";
			panel5.AutoSize = true;
			panel5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel5.Controls.Add(rBtnFastHopOff);
			panel5.Controls.Add(rBtnFastHopOn);
			panel5.Location = new Point(164, 45);
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
			nudBitrateFrac.Location = new Point(164, 94);
			nudBitrateFrac.Maximum = new decimal([15, 0, 0, 0]);
			nudBitrateFrac.Name = "nudBitrateFrac";
			nudBitrateFrac.Size = new Size(124, 20);
			nudBitrateFrac.TabIndex = 4;
			nudBitrateFrac.ThousandsSeparator = true;
			nudBitrateFrac.ValueChanged += nudBitrateFrac_ValueChanged;
			label12.AutoSize = true;
			label12.Location = new Point(137, 123);
			label12.Name = "label12";
			label12.Size = new Size(21, 13);
			label12.TabIndex = 7;
			label12.Text = "+/-";
			label8.AutoSize = true;
			label8.Location = new Point(294, 72);
			label8.Name = "label8";
			label8.Size = new Size(24, 13);
			label8.TabIndex = 5;
			label8.Text = "bps";
			label11.AutoSize = true;
			label11.Location = new Point(294, 123);
			label11.Name = "label11";
			label11.Size = new Size(20, 13);
			label11.TabIndex = 9;
			label11.Text = "Hz";
			label13.AutoSize = true;
			label13.Location = new Point(294, 23);
			label13.Name = "label13";
			label13.Size = new Size(20, 13);
			label13.TabIndex = 2;
			label13.Text = "Hz";
			label14.AutoSize = true;
			label14.Location = new Point(6, 23);
			label14.Name = "label14";
			label14.Size = new Size(74, 13);
			label14.TabIndex = 0;
			label14.Text = "RF frequency:";
			label2.AutoSize = true;
			label2.Location = new Point(6, 98);
			label2.Name = "label2";
			label2.Size = new Size(92, 13);
			label2.TabIndex = 3;
			label2.Text = "Bitrate fine tuning:";
			label10.AutoSize = true;
			label10.Location = new Point(6, 123);
			label10.Name = "label10";
			label10.Size = new Size(34, 13);
			label10.TabIndex = 6;
			label10.Text = "Fdev:";
			label7.AutoSize = true;
			label7.Location = new Point(6, 72);
			label7.Name = "label7";
			label7.Size = new Size(40, 13);
			label7.TabIndex = 3;
			label7.Text = "Bitrate:";
			panel8.AutoSize = true;
			panel8.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel8.Controls.Add(rBtnLowFrequencyModeOff);
			panel8.Controls.Add(rBtnLowFrequencyModeOn);
			panel8.Location = new Point(164, 46);
			panel8.Name = "panel8";
			panel8.Size = new Size(102, 23);
			panel8.TabIndex = 1;
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
			cBoxBand.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxBand.FormattingEnabled = true;
			cBoxBand.Items.AddRange(["Auto", "820-1024", "410-525", "137-175"]);
			cBoxBand.Location = new Point(164, 19);
			cBoxBand.Name = "cBoxBand";
			cBoxBand.Size = new Size(124, 21);
			cBoxBand.TabIndex = 3;
			cBoxBand.SelectedIndexChanged += cBoxBand_SelectedIndexChanged;
			label19.AutoSize = true;
			label19.Location = new Point(6, 51);
			label19.Name = "label19";
			label19.Size = new Size(109, 13);
			label19.TabIndex = 0;
			label19.Text = "Low frequency mode:";
			label18.AutoSize = true;
			label18.Location = new Point(6, 22);
			label18.Name = "label18";
			label18.Size = new Size(35, 13);
			label18.TabIndex = 2;
			label18.Text = "Band:";
			label4.AutoSize = true;
			label4.Location = new Point(294, 23);
			label4.Name = "label4";
			label4.Size = new Size(29, 13);
			label4.TabIndex = 4;
			label4.Text = "MHz";
			groupBoxEx1.Controls.Add(panel10);
			groupBoxEx1.Controls.Add(label31);
			groupBoxEx1.Controls.Add(panel9);
			groupBoxEx1.Controls.Add(label29);
			groupBoxEx1.Location = new Point(583, 154);
			groupBoxEx1.Name = "groupBoxEx1";
			groupBoxEx1.Size = new Size(355, 78);
			groupBoxEx1.TabIndex = 6;
			groupBoxEx1.TabStop = false;
			groupBoxEx1.Text = "Low frequency band";
			groupBoxEx1.Visible = false;
			panel10.AutoSize = true;
			panel10.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel10.Controls.Add(rBtnForceRxBandLowFrequencyOff);
			panel10.Controls.Add(rBtnForceRxBandLowFrequencyOn);
			panel10.Location = new Point(164, 48);
			panel10.Name = "panel10";
			panel10.Size = new Size(102, 23);
			panel10.TabIndex = 1;
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
			label31.AutoSize = true;
			label31.Location = new Point(6, 53);
			label31.Name = "label31";
			label31.Size = new Size(53, 13);
			label31.TabIndex = 0;
			label31.Text = "Force Rx:";
			panel9.AutoSize = true;
			panel9.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel9.Controls.Add(rBtnForceTxBandLowFrequencyOff);
			panel9.Controls.Add(rBtnForceTxBandLowFrequencyOn);
			panel9.Location = new Point(164, 19);
			panel9.Name = "panel9";
			panel9.Size = new Size(102, 23);
			panel9.TabIndex = 1;
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
			label29.AutoSize = true;
			label29.Location = new Point(6, 24);
			label29.Name = "label29";
			label29.Size = new Size(52, 13);
			label29.TabIndex = 0;
			label29.Text = "Force Tx:";
			groupBoxEx2.Controls.Add(cBoxBand);
			groupBoxEx2.Controls.Add(panel8);
			groupBoxEx2.Controls.Add(label4);
			groupBoxEx2.Controls.Add(label18);
			groupBoxEx2.Controls.Add(label19);
			groupBoxEx2.Location = new Point(583, 238);
			groupBoxEx2.Name = "groupBoxEx2";
			groupBoxEx2.Size = new Size(355, 83);
			groupBoxEx2.TabIndex = 7;
			groupBoxEx2.TabStop = false;
			groupBoxEx2.Text = "Band";
			groupBoxEx2.Visible = false;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(groupBoxEx2);
			Controls.Add(groupBoxEx1);
			Controls.Add(gBoxBatteryManagement);
			Controls.Add(gBoxOscillators);
			Controls.Add(gBoxModulation);
			Controls.Add(gBoxGeneral);
			Name = "CommonViewControl";
			Size = new Size(799, 493);
			((ISupportInitialize)errorProvider).EndInit();
			((ISupportInitialize)nudBitrate).EndInit();
			((ISupportInitialize)nudFdev).EndInit();
			((ISupportInitialize)nudFrequencyRf).EndInit();
			gBoxBatteryManagement.ResumeLayout(false);
			gBoxBatteryManagement.PerformLayout();
			panel4.ResumeLayout(false);
			panel4.PerformLayout();
			gBoxOscillators.ResumeLayout(false);
			gBoxOscillators.PerformLayout();
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			((ISupportInitialize)nudFrequencyXo).EndInit();
			gBoxModulation.ResumeLayout(false);
			gBoxModulation.PerformLayout();
			panel2.ResumeLayout(false);
			panel2.PerformLayout();
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			gBoxGeneral.ResumeLayout(false);
			gBoxGeneral.PerformLayout();
			panel5.ResumeLayout(false);
			panel5.PerformLayout();
			((ISupportInitialize)nudBitrateFrac).EndInit();
			panel8.ResumeLayout(false);
			panel8.PerformLayout();
			groupBoxEx1.ResumeLayout(false);
			groupBoxEx1.PerformLayout();
			panel10.ResumeLayout(false);
			panel10.PerformLayout();
			panel9.ResumeLayout(false);
			panel9.PerformLayout();
			groupBoxEx2.ResumeLayout(false);
			groupBoxEx2.PerformLayout();
			ResumeLayout(false);
		}

		public CommonViewControl()
		{
			InitializeComponent();
		}

		private void OnFrequencyXoChanged(decimal value)
		{
            FrequencyXoChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnModulationTypeChanged(ModulationTypeEnum value)
		{
            ModulationTypeChanged?.Invoke(this, new ModulationTypeEventArg(value));
        }

		private void OnModulationShapingChanged(byte value)
		{
            ModulationShapingChanged?.Invoke(this, new ByteEventArg(value));
        }

		private void OnBitrateChanged(decimal value)
		{
            BitrateChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnBitrateFracChanged(decimal value)
		{
            BitrateFracChanged?.Invoke(this, new DecimalEventArg(value));
        }

		private void OnFdevChanged(decimal value)
		{
            FdevChanged?.Invoke(this, new DecimalEventArg(value));
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

		private void OnTcxoInputChanged(bool value)
		{
            TcxoInputChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnRcCalibrationChanged()
		{
            RcCalibrationChanged?.Invoke(this, EventArgs.Empty);
        }

		private void OnLowBatOnChanged(bool value)
		{
            LowBatOnChanged?.Invoke(this, new BooleanEventArg(value));
        }

		private void OnLowBatTrimChanged(LowBatTrimEnum value)
		{
            LowBatTrimChanged?.Invoke(this, new LowBatTrimEventArg(value));
        }

		public void UpdateBitrateLimits(LimitCheckStatusEnum status, string message)
		{
			switch (status)
			{
			case LimitCheckStatusEnum.OK:
				nudBitrate.BackColor = SystemColors.Window;
				break;
			case LimitCheckStatusEnum.OUT_OF_RANGE:
				nudBitrate.BackColor = ControlPaint.LightLight(Color.Orange);
				break;
			case LimitCheckStatusEnum.ERROR:
				nudBitrate.BackColor = ControlPaint.LightLight(Color.Red);
				break;
			}
			errorProvider.SetError(nudBitrate, message);
		}

		public void UpdateFdevLimits(LimitCheckStatusEnum status, string message)
		{
			switch (status)
			{
			case LimitCheckStatusEnum.OK:
				nudFdev.BackColor = SystemColors.Window;
				break;
			case LimitCheckStatusEnum.OUT_OF_RANGE:
				nudFdev.BackColor = ControlPaint.LightLight(Color.Orange);
				break;
			case LimitCheckStatusEnum.ERROR:
				nudFdev.BackColor = ControlPaint.LightLight(Color.Red);
				break;
			}
			errorProvider.SetError(nudFdev, message);
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

		private void nudFrequencyXo_ValueChanged(object sender, EventArgs e)
		{
			FrequencyXo = nudFrequencyXo.Value;
			OnFrequencyXoChanged(FrequencyXo);
		}

		private void rBtnModulationType_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnModulationTypeFsk.Checked)
			{
				ModulationType = ModulationTypeEnum.FSK;
			}
			else if (rBtnModulationTypeOok.Checked)
			{
				ModulationType = ModulationTypeEnum.OOK;
			}
			else
			{
				ModulationType = ModulationTypeEnum.Reserved2;
			}
			OnModulationTypeChanged(ModulationType);
		}

		private void rBtnModulationShaping_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnModulationShapingOff.Checked)
			{
				ModulationShaping = 0;
			}
			else if (rBtnModulationShaping01.Checked)
			{
				ModulationShaping = 1;
			}
			else if (rBtnModulationShaping10.Checked)
			{
				ModulationShaping = 2;
			}
			else if (rBtnModulationShaping11.Checked)
			{
				ModulationShaping = 3;
			}
			OnModulationShapingChanged(ModulationShaping);
		}

		private void nudBitrate_ValueChanged(object sender, EventArgs e)
		{
			_ = (int)Math.Round(FrequencyXo / Bitrate - BitrateFrac / 16m, MidpointRounding.AwayFromZero);
			var num = (int)Math.Round(FrequencyXo / nudBitrate.Value - BitrateFrac / 16m, MidpointRounding.AwayFromZero);
			var num2 = (int)(nudBitrate.Value - Bitrate);
			nudBitrate.ValueChanged -= nudBitrate_ValueChanged;
			nudBitrate.Value = num2 is >= -1 and <= 1 ? Math.Round(FrequencyXo / (num - num2 + BitrateFrac / 16m), MidpointRounding.AwayFromZero) : Math.Round(FrequencyXo / (num + BitrateFrac / 16m), MidpointRounding.AwayFromZero);
			nudBitrate.ValueChanged += nudBitrate_ValueChanged;
			Bitrate = nudBitrate.Value;
			OnBitrateChanged(Bitrate);
		}

		private void nudBitrateFrac_ValueChanged(object sender, EventArgs e)
		{
			BitrateFrac = nudBitrateFrac.Value;
			OnBitrateFracChanged(BitrateFrac);
		}

		private void nudFdev_ValueChanged(object sender, EventArgs e)
		{
			Fdev = nudFdev.Value;
			OnFdevChanged(Fdev);
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

		private void rBtnTcxoInput_CheckedChanged(object sender, EventArgs e)
		{
			OnTcxoInputChanged(TcxoInputOn);
		}

		private void btnRcCalibration_Click(object sender, EventArgs e)
		{
			OnRcCalibrationChanged();
		}

		private void rBtnLowBatOn_CheckedChanged(object sender, EventArgs e)
		{
			LowBatOn = rBtnLowBatOn.Checked;
			OnLowBatOnChanged(LowBatOn);
		}

		private void cBoxLowBatTrim_SelectedIndexChanged(object sender, EventArgs e)
		{
			LowBatTrim = (LowBatTrimEnum)cBoxLowBatTrim.SelectedIndex;
			OnLowBatTrimChanged(LowBatTrim);
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
			if (sender == gBoxGeneral)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "General"));
			}
			else if (sender == gBoxModulation)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "Modulation"));
			}
			else if (sender == gBoxOscillators)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "Oscillators"));
			}
			else if (sender == gBoxBatteryManagement)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "Battery management"));
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
