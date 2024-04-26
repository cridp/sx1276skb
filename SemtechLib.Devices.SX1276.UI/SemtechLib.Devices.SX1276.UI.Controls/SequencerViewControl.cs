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
	public sealed class SequencerViewControl : UserControl, INotifyDocumentationChanged
	{
		private IContainer components;

		private ErrorProvider errorProvider;

		private Button btnSequencerStop;

		private Button btnSequencerStart;

		private Label label1;

		private Label label22;

		private ComboBox cBoxTimer1Resolution;

		private ComboBox cBoxFromPacketReceived;

		private ComboBox cBoxFromRxTimeout;

		private ComboBox cBoxFromReceive;

		private ComboBox cBoxFromTransmit;

		private ComboBox cBoxFromIdle;

		private ComboBox cBoxSeqLowPowerState;

		private ComboBox cBoxFromStart;

		private ComboBox cBoxSeqLowPowerMode;

		private Label label3;

		private Label label2;

		private ComboBox cBoxSeqTimer2Resolution;

		private NumericUpDownEx nudSeqTimer2Coefficient;

		private NumericUpDownEx nudSeqTimer1Coefficient;

		private Label label5;

		private Label label4;

		private Label label7;

		private Label label9;

		private Label label8;

		private Label label6;

		private Label lblTimer2Value;

		private Label lblTimer1Value;

		private Label label12;

		private Label label11;

		private Label label20;

		private Label label19;

		private Label label18;

		private Label label17;

		private Label label16;

		private Label label15;

		private Label label14;

		private Label label13;

		private Label label10;

		private Led ledSequencerModeStatus;

		private ToolTip toolTip1;

		private decimal timer1Resol = 0.064m;

		private decimal timer2Resol = 0.064m;

		public IdleMode IdleMode
		{
			get => (IdleMode)cBoxSeqLowPowerMode.SelectedIndex;
			set
			{
				cBoxSeqLowPowerMode.SelectedIndexChanged -= cBoxSeqLowPowerMode_SelectedIndexChanged;
				cBoxSeqLowPowerMode.SelectedIndex = (int)value;
				cBoxSeqLowPowerMode.SelectedIndexChanged += cBoxSeqLowPowerMode_SelectedIndexChanged;
			}
		}

		public FromStart FromStart
		{
			get => (FromStart)cBoxFromStart.SelectedIndex;
			set
			{
				cBoxFromStart.SelectedIndexChanged -= cBoxFromStart_SelectedIndexChanged;
				cBoxFromStart.SelectedIndex = (int)value;
				cBoxFromStart.SelectedIndexChanged += cBoxFromStart_SelectedIndexChanged;
			}
		}

		public LowPowerSelection LowPowerSelection
		{
			get => (LowPowerSelection)cBoxSeqLowPowerState.SelectedIndex;
			set
			{
				cBoxSeqLowPowerState.SelectedIndexChanged -= cBoxSeqLowPowerState_SelectedIndexChanged;
				cBoxSeqLowPowerState.SelectedIndex = (int)value;
				cBoxSeqLowPowerState.SelectedIndexChanged += cBoxSeqLowPowerState_SelectedIndexChanged;
			}
		}

		public FromIdle FromIdle
		{
			get => (FromIdle)cBoxFromIdle.SelectedIndex;
			set
			{
				cBoxFromIdle.SelectedIndexChanged -= cBoxFromIdle_SelectedIndexChanged;
				cBoxFromIdle.SelectedIndex = (int)value;
				cBoxFromIdle.SelectedIndexChanged += cBoxFromIdle_SelectedIndexChanged;
			}
		}

		public FromTransmit FromTransmit
		{
			get => (FromTransmit)cBoxFromTransmit.SelectedIndex;
			set
			{
				cBoxFromTransmit.SelectedIndexChanged -= cBoxFromTransmit_SelectedIndexChanged;
				cBoxFromTransmit.SelectedIndex = (int)value;
				cBoxFromTransmit.SelectedIndexChanged += cBoxFromTransmit_SelectedIndexChanged;
			}
		}

		public FromReceive FromReceive
		{
			get => (FromReceive)cBoxFromReceive.SelectedIndex;
			set
			{
				cBoxFromReceive.SelectedIndexChanged -= cBoxFromReceive_SelectedIndexChanged;
				cBoxFromReceive.SelectedIndex = (int)value;
				cBoxFromReceive.SelectedIndexChanged += cBoxFromReceive_SelectedIndexChanged;
			}
		}

		public FromRxTimeout FromRxTimeout
		{
			get => (FromRxTimeout)cBoxFromRxTimeout.SelectedIndex;
			set
			{
				cBoxFromRxTimeout.SelectedIndexChanged -= cBoxFromRxTimeout_SelectedIndexChanged;
				cBoxFromRxTimeout.SelectedIndex = (int)value;
				cBoxFromRxTimeout.SelectedIndexChanged += cBoxFromRxTimeout_SelectedIndexChanged;
			}
		}

		public FromPacketReceived FromPacketReceived
		{
			get => (FromPacketReceived)cBoxFromPacketReceived.SelectedIndex;
			set
			{
				cBoxFromPacketReceived.SelectedIndexChanged -= cBoxFromPacketReceived_SelectedIndexChanged;
				cBoxFromPacketReceived.SelectedIndex = (int)value;
				cBoxFromPacketReceived.SelectedIndexChanged += cBoxFromPacketReceived_SelectedIndexChanged;
			}
		}

		public TimerResolution Timer1Resolution
		{
			get => (TimerResolution)cBoxTimer1Resolution.SelectedIndex;
			set
			{
				cBoxTimer1Resolution.SelectedIndexChanged -= cBoxTimer1Resolution_SelectedIndexChanged;
				cBoxTimer1Resolution.SelectedIndex = (int)value;
				switch (value)
				{
				case TimerResolution.Res000064:
					timer1Resol = 0.064m;
					nudSeqTimer1Coefficient.Enabled = true;
					break;
				case TimerResolution.Res004100:
					timer1Resol = 4.1m;
					nudSeqTimer1Coefficient.Enabled = true;
					break;
				case TimerResolution.Res262000:
					timer1Resol = 262m;
					nudSeqTimer1Coefficient.Enabled = true;
					break;
				default:
					nudSeqTimer1Coefficient.Enabled = false;
					break;
				}
				lblTimer1Value.Text = value == TimerResolution.OFF ? "OFF" : ((int)(timer1Resol * nudSeqTimer1Coefficient.Value * 1000.0m)).ToString();
				if (Timer2Resolution != 0 && value == TimerResolution.OFF)
				{
					errorProvider.SetError(lblTimer2Value, "When Timer2 is enabled the Timer1 must be enabled also.");
				}
				else
				{
					errorProvider.SetError(lblTimer2Value, "");
				}
				cBoxTimer1Resolution.SelectedIndexChanged += cBoxTimer1Resolution_SelectedIndexChanged;
			}
		}

		public TimerResolution Timer2Resolution
		{
			get => (TimerResolution)cBoxSeqTimer2Resolution.SelectedIndex;
			set
			{
				cBoxSeqTimer2Resolution.SelectedIndexChanged -= cBoxSeqTimer2Resolution_SelectedIndexChanged;
				cBoxSeqTimer2Resolution.SelectedIndex = (int)value;
				switch (value)
				{
				case TimerResolution.Res000064:
					timer2Resol = 0.064m;
					nudSeqTimer2Coefficient.Enabled = true;
					break;
				case TimerResolution.Res004100:
					timer2Resol = 4.1m;
					nudSeqTimer2Coefficient.Enabled = true;
					break;
				case TimerResolution.Res262000:
					timer2Resol = 262m;
					nudSeqTimer2Coefficient.Enabled = true;
					break;
				default:
					nudSeqTimer2Coefficient.Enabled = false;
					break;
				}
				lblTimer2Value.Text = value == TimerResolution.OFF ? "OFF" : ((int)(timer2Resol * nudSeqTimer2Coefficient.Value * 1000.0m)).ToString();
				if (Timer1Resolution == TimerResolution.OFF && value != 0)
				{
					errorProvider.SetError(lblTimer2Value, "When Timer2 is enabled the Timer1 must be enabled also.");
				}
				else
				{
					errorProvider.SetError(lblTimer2Value, "");
				}
				cBoxSeqTimer2Resolution.SelectedIndexChanged += cBoxSeqTimer2Resolution_SelectedIndexChanged;
			}
		}

		public byte Timer1Coef
		{
			get => (byte)nudSeqTimer1Coefficient.Value;
			set
			{
				try
				{
					nudSeqTimer1Coefficient.ValueChanged -= nudSeqTimer1Coefficient_ValueChanged;
					nudSeqTimer1Coefficient.Value = value;
					lblTimer1Value.Text = Timer1Resolution == TimerResolution.OFF ? "OFF" : ((int)(timer1Resol * nudSeqTimer1Coefficient.Value * 1000.0m)).ToString();
				}
				catch (Exception)
				{
				}
				finally
				{
					nudSeqTimer1Coefficient.ValueChanged += nudSeqTimer1Coefficient_ValueChanged;
				}
			}
		}

		public byte Timer2Coef
		{
			get => (byte)nudSeqTimer2Coefficient.Value;
			set
			{
				try
				{
					nudSeqTimer2Coefficient.ValueChanged -= nudSeqTimer2Coefficient_ValueChanged;
					nudSeqTimer2Coefficient.Value = value;
					lblTimer2Value.Text = Timer2Resolution == TimerResolution.OFF ? "OFF" : ((int)(timer2Resol * nudSeqTimer2Coefficient.Value * 1000.0m)).ToString();
				}
				catch (Exception)
				{
				}
				finally
				{
					nudSeqTimer2Coefficient.ValueChanged += nudSeqTimer2Coefficient_ValueChanged;
				}
			}
		}

		public event EventHandler SequencerStartChanged;

		public event EventHandler SequencerStopChanged;

		public event IdleModeEventHandler IdleModeChanged;

		public event FromStartEventHandler FromStartChanged;

		public event LowPowerSelectionEventHandler LowPowerSelectionChanged;

		public event FromIdleEventHandler FromIdleChanged;

		public event FromTransmitEventHandler FromTransmitChanged;

		public event FromReceiveEventHandler FromReceiveChanged;

		public event FromRxTimeoutEventHandler FromRxTimeoutChanged;

		public event FromPacketReceivedEventHandler FromPacketReceivedChanged;

		public event TimerResolutionEventHandler Timer1ResolutionChanged;

		public event TimerResolutionEventHandler Timer2ResolutionChanged;

		public event ByteEventHandler Timer1CoefChanged;

		public event ByteEventHandler Timer2CoefChanged;

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
			var resources = new ComponentResourceManager(typeof(SequencerViewControl));
			errorProvider = new ErrorProvider(components);
			lblTimer2Value = new Label();
			btnSequencerStop = new Button();
			btnSequencerStart = new Button();
			label1 = new Label();
			label22 = new Label();
			cBoxTimer1Resolution = new ComboBox();
			cBoxSeqLowPowerMode = new ComboBox();
			cBoxFromStart = new ComboBox();
			cBoxSeqLowPowerState = new ComboBox();
			cBoxFromIdle = new ComboBox();
			cBoxFromTransmit = new ComboBox();
			cBoxFromReceive = new ComboBox();
			cBoxFromRxTimeout = new ComboBox();
			cBoxFromPacketReceived = new ComboBox();
			cBoxSeqTimer2Resolution = new ComboBox();
			label2 = new Label();
			label3 = new Label();
			label4 = new Label();
			label5 = new Label();
			label6 = new Label();
			label7 = new Label();
			label8 = new Label();
			label9 = new Label();
			lblTimer1Value = new Label();
			label11 = new Label();
			label12 = new Label();
			label10 = new Label();
			label13 = new Label();
			label14 = new Label();
			label15 = new Label();
			label16 = new Label();
			label17 = new Label();
			label18 = new Label();
			label19 = new Label();
			label20 = new Label();
			nudSeqTimer2Coefficient = new NumericUpDownEx();
			nudSeqTimer1Coefficient = new NumericUpDownEx();
			ledSequencerModeStatus = new Led();
			toolTip1 = new ToolTip(components);
			((ISupportInitialize)errorProvider).BeginInit();
			((ISupportInitialize)nudSeqTimer2Coefficient).BeginInit();
			((ISupportInitialize)nudSeqTimer1Coefficient).BeginInit();
			SuspendLayout();
			errorProvider.ContainerControl = this;
			lblTimer2Value.BackColor = Color.Transparent;
			lblTimer2Value.BorderStyle = BorderStyle.Fixed3D;
			errorProvider.SetIconPadding(lblTimer2Value, 30);
			lblTimer2Value.Location = new Point(502, 391);
			lblTimer2Value.Margin = new Padding(3);
			lblTimer2Value.Name = "lblTimer2Value";
			lblTimer2Value.Size = new Size(98, 20);
			lblTimer2Value.TabIndex = 35;
			lblTimer2Value.Text = "0";
			lblTimer2Value.TextAlign = ContentAlignment.MiddleLeft;
			btnSequencerStop.Location = new Point(447, 75);
			btnSequencerStop.Name = "btnSequencerStop";
			btnSequencerStop.Size = new Size(89, 23);
			btnSequencerStop.TabIndex = 2;
			btnSequencerStop.Text = "Stop";
			btnSequencerStop.UseVisualStyleBackColor = true;
			btnSequencerStop.Click += btnSequencerStop_Click;
			btnSequencerStart.Location = new Point(352, 75);
			btnSequencerStart.Name = "btnSequencerStart";
			btnSequencerStart.Size = new Size(89, 23);
			btnSequencerStart.TabIndex = 1;
			btnSequencerStart.Text = "Start";
			btnSequencerStart.UseVisualStyleBackColor = true;
			btnSequencerStart.Click += btnSequencerStart_Click;
			label1.AutoSize = true;
			label1.Location = new Point(194, 320);
			label1.Margin = new Padding(3);
			label1.Name = "label1";
			label1.Size = new Size(90, 13);
			label1.TabIndex = 19;
			label1.Text = "Timer 1 resolution";
			label22.AutoSize = true;
			label22.Location = new Point(307, 343);
			label22.Name = "label22";
			label22.Size = new Size(18, 13);
			label22.TabIndex = 22;
			label22.Text = "µs";
			cBoxTimer1Resolution.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxTimer1Resolution.FormattingEnabled = true;
			cBoxTimer1Resolution.Items.AddRange(["OFF", "64", "4'100", "262'000"]);
			cBoxTimer1Resolution.Location = new Point(177, 339);
			cBoxTimer1Resolution.Name = "cBoxTimer1Resolution";
			cBoxTimer1Resolution.Size = new Size(124, 21);
			cBoxTimer1Resolution.TabIndex = 21;
			cBoxTimer1Resolution.SelectedIndexChanged += cBoxTimer1Resolution_SelectedIndexChanged;
			cBoxSeqLowPowerMode.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxSeqLowPowerMode.FormattingEnabled = true;
			cBoxSeqLowPowerMode.Items.AddRange(["Standby", "Sleep"]);
			cBoxSeqLowPowerMode.Location = new Point(352, 104);
			cBoxSeqLowPowerMode.Name = "cBoxSeqLowPowerMode";
			cBoxSeqLowPowerMode.Size = new Size(124, 21);
			cBoxSeqLowPowerMode.TabIndex = 4;
			cBoxSeqLowPowerMode.SelectedIndexChanged += cBoxSeqLowPowerMode_SelectedIndexChanged;
			cBoxFromStart.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxFromStart.FormattingEnabled = true;
			cBoxFromStart.Items.AddRange(["To LowPowerSelection ", "To Rx", "To Tx", "To Tx on FifoLevel"]);
			cBoxFromStart.Location = new Point(352, 131);
			cBoxFromStart.Name = "cBoxFromStart";
			cBoxFromStart.Size = new Size(124, 21);
			cBoxFromStart.TabIndex = 6;
			cBoxFromStart.SelectedIndexChanged += cBoxFromStart_SelectedIndexChanged;
			cBoxSeqLowPowerState.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxSeqLowPowerState.FormattingEnabled = true;
			cBoxSeqLowPowerState.Items.AddRange(["Sequencer OFF", "Idle"]);
			cBoxSeqLowPowerState.Location = new Point(352, 158);
			cBoxSeqLowPowerState.Name = "cBoxSeqLowPowerState";
			cBoxSeqLowPowerState.Size = new Size(124, 21);
			cBoxSeqLowPowerState.TabIndex = 8;
			cBoxSeqLowPowerState.SelectedIndexChanged += cBoxSeqLowPowerState_SelectedIndexChanged;
			cBoxFromIdle.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxFromIdle.FormattingEnabled = true;
			cBoxFromIdle.Items.AddRange(["To Tx", "To Rx"]);
			cBoxFromIdle.Location = new Point(352, 185);
			cBoxFromIdle.Name = "cBoxFromIdle";
			cBoxFromIdle.Size = new Size(124, 21);
			cBoxFromIdle.TabIndex = 10;
			cBoxFromIdle.SelectedIndexChanged += cBoxFromIdle_SelectedIndexChanged;
			cBoxFromTransmit.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxFromTransmit.FormattingEnabled = true;
			cBoxFromTransmit.Items.AddRange(["To LowPowerSelection", "To Rx"]);
			cBoxFromTransmit.Location = new Point(352, 212);
			cBoxFromTransmit.Name = "cBoxFromTransmit";
			cBoxFromTransmit.Size = new Size(124, 21);
			cBoxFromTransmit.TabIndex = 12;
			cBoxFromTransmit.SelectedIndexChanged += cBoxFromTransmit_SelectedIndexChanged;
			cBoxFromReceive.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxFromReceive.FormattingEnabled = true;
			cBoxFromReceive.Items.AddRange(["Unused", "To PacketReceived on PayloadReady", "To LowPowerSelection on PayloadReady", "To PacketReceived on CrcOk", "To Sequencer OFF on RSSI", "To Sequencer OFF on SyncAddress", "To Sequencer OFF on PreambleDetect", "Unused"]);
			cBoxFromReceive.Location = new Point(352, 239);
			cBoxFromReceive.Name = "cBoxFromReceive";
			cBoxFromReceive.Size = new Size(124, 21);
			cBoxFromReceive.TabIndex = 14;
			cBoxFromReceive.SelectedIndexChanged += cBoxFromReceive_SelectedIndexChanged;
			cBoxFromRxTimeout.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxFromRxTimeout.FormattingEnabled = true;
			cBoxFromRxTimeout.Items.AddRange(["To Rx on RxRestart", "To Tx", "To LowPowerSelection", "To Sequencer OFF"]);
			cBoxFromRxTimeout.Location = new Point(352, 266);
			cBoxFromRxTimeout.Name = "cBoxFromRxTimeout";
			cBoxFromRxTimeout.Size = new Size(124, 21);
			cBoxFromRxTimeout.TabIndex = 16;
			cBoxFromRxTimeout.SelectedIndexChanged += cBoxFromRxTimeout_SelectedIndexChanged;
			cBoxFromPacketReceived.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxFromPacketReceived.FormattingEnabled = true;
			cBoxFromPacketReceived.Items.AddRange(["To Sequencer OFF", "To Tx on FifoEmpty", "To LowPowerSelection", "To Rx via FsRx on frequency change", "To Rx"]);
			cBoxFromPacketReceived.Location = new Point(352, 293);
			cBoxFromPacketReceived.Name = "cBoxFromPacketReceived";
			cBoxFromPacketReceived.Size = new Size(124, 21);
			cBoxFromPacketReceived.TabIndex = 18;
			cBoxFromPacketReceived.SelectedIndexChanged += cBoxFromPacketReceived_SelectedIndexChanged;
			cBoxSeqTimer2Resolution.DropDownStyle = ComboBoxStyle.DropDownList;
			cBoxSeqTimer2Resolution.FormattingEnabled = true;
			cBoxSeqTimer2Resolution.Items.AddRange(["OFF", "64", "4'100", "262'000"]);
			cBoxSeqTimer2Resolution.Location = new Point(177, 391);
			cBoxSeqTimer2Resolution.Name = "cBoxSeqTimer2Resolution";
			cBoxSeqTimer2Resolution.Size = new Size(124, 21);
			cBoxSeqTimer2Resolution.TabIndex = 30;
			cBoxSeqTimer2Resolution.SelectedIndexChanged += cBoxSeqTimer2Resolution_SelectedIndexChanged;
			label2.AutoSize = true;
			label2.Location = new Point(307, 395);
			label2.Name = "label2";
			label2.Size = new Size(18, 13);
			label2.TabIndex = 31;
			label2.Text = "µs";
			label3.AutoSize = true;
			label3.Location = new Point(194, 372);
			label3.Margin = new Padding(3);
			label3.Name = "label3";
			label3.Size = new Size(90, 13);
			label3.TabIndex = 28;
			label3.Text = "Timer 2 resolution";
			label4.AutoSize = true;
			label4.Location = new Point(367, 320);
			label4.Margin = new Padding(3);
			label4.Name = "label4";
			label4.Size = new Size(94, 13);
			label4.TabIndex = 20;
			label4.Text = "Timer 1 coefficient";
			label5.AutoSize = true;
			label5.Location = new Point(367, 372);
			label5.Margin = new Padding(3);
			label5.Name = "label5";
			label5.Size = new Size(94, 13);
			label5.TabIndex = 29;
			label5.Text = "Timer 2 coefficient";
			label6.AutoSize = true;
			label6.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label6.Location = new Point(331, 343);
			label6.Name = "label6";
			label6.Size = new Size(15, 13);
			label6.TabIndex = 23;
			label6.Text = "X";
			label7.AutoSize = true;
			label7.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label7.Location = new Point(331, 395);
			label7.Name = "label7";
			label7.Size = new Size(15, 13);
			label7.TabIndex = 32;
			label7.Text = "X";
			label8.AutoSize = true;
			label8.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label8.Location = new Point(482, 343);
			label8.Name = "label8";
			label8.Size = new Size(14, 13);
			label8.TabIndex = 25;
			label8.Text = "=";
			label9.AutoSize = true;
			label9.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label9.Location = new Point(482, 395);
			label9.Name = "label9";
			label9.Size = new Size(14, 13);
			label9.TabIndex = 34;
			label9.Text = "=";
			lblTimer1Value.BackColor = Color.Transparent;
			lblTimer1Value.BorderStyle = BorderStyle.Fixed3D;
			lblTimer1Value.Location = new Point(502, 339);
			lblTimer1Value.Margin = new Padding(3);
			lblTimer1Value.Name = "lblTimer1Value";
			lblTimer1Value.Size = new Size(98, 20);
			lblTimer1Value.TabIndex = 26;
			lblTimer1Value.Text = "0";
			lblTimer1Value.TextAlign = ContentAlignment.MiddleLeft;
			label11.AutoSize = true;
			label11.Location = new Point(606, 343);
			label11.Name = "label11";
			label11.Size = new Size(18, 13);
			label11.TabIndex = 27;
			label11.Text = "µs";
			label12.AutoSize = true;
			label12.Location = new Point(606, 395);
			label12.Name = "label12";
			label12.Size = new Size(18, 13);
			label12.TabIndex = 36;
			label12.Text = "µs";
			label10.AutoSize = true;
			label10.Location = new Point(174, 80);
			label10.Name = "label10";
			label10.Size = new Size(62, 13);
			label10.TabIndex = 0;
			label10.Text = "Sequencer:";
			label13.AutoSize = true;
			label13.Location = new Point(174, 107);
			label13.Name = "label13";
			label13.Size = new Size(56, 13);
			label13.TabIndex = 3;
			label13.Text = "Idle mode:";
			label14.AutoSize = true;
			label14.Location = new Point(174, 134);
			label14.Name = "label14";
			label14.Size = new Size(102, 13);
			label14.TabIndex = 5;
			label14.Text = "Transition from start:";
			label15.AutoSize = true;
			label15.Location = new Point(174, 161);
			label15.Name = "label15";
			label15.Size = new Size(107, 13);
			label15.TabIndex = 7;
			label15.Text = "Low power selection:";
			label16.AutoSize = true;
			label16.Location = new Point(174, 188);
			label16.Name = "label16";
			label16.Size = new Size(98, 13);
			label16.TabIndex = 9;
			label16.Text = "Transition from idle:";
			label17.AutoSize = true;
			label17.Location = new Point(174, 215);
			label17.Name = "label17";
			label17.Size = new Size(118, 13);
			label17.TabIndex = 11;
			label17.Text = "Transition from transmit:";
			label18.AutoSize = true;
			label18.Location = new Point(174, 242);
			label18.Name = "label18";
			label18.Size = new Size(117, 13);
			label18.TabIndex = 13;
			label18.Text = "Transition from receive:";
			label19.AutoSize = true;
			label19.Location = new Point(174, 269);
			label19.Name = "label19";
			label19.Size = new Size(132, 13);
			label19.TabIndex = 15;
			label19.Text = "Transition from Rx timeout:";
			label20.AutoSize = true;
			label20.Location = new Point(174, 296);
			label20.Name = "label20";
			label20.Size = new Size(159, 13);
			label20.TabIndex = 17;
			label20.Text = "Transition from packet received:";
			nudSeqTimer2Coefficient.Location = new Point(352, 391);
			nudSeqTimer2Coefficient.Maximum = new decimal([255, 0, 0, 0]);
			nudSeqTimer2Coefficient.Name = "nudSeqTimer2Coefficient";
			nudSeqTimer2Coefficient.Size = new Size(124, 20);
			nudSeqTimer2Coefficient.TabIndex = 33;
			nudSeqTimer2Coefficient.ThousandsSeparator = true;
			nudSeqTimer2Coefficient.ValueChanged += nudSeqTimer2Coefficient_ValueChanged;
			nudSeqTimer1Coefficient.Location = new Point(352, 339);
			nudSeqTimer1Coefficient.Maximum = new decimal([255, 0, 0, 0]);
			nudSeqTimer1Coefficient.Name = "nudSeqTimer1Coefficient";
			nudSeqTimer1Coefficient.Size = new Size(124, 20);
			nudSeqTimer1Coefficient.TabIndex = 24;
			nudSeqTimer1Coefficient.ThousandsSeparator = true;
			nudSeqTimer1Coefficient.ValueChanged += nudSeqTimer1Coefficient_ValueChanged;
			ledSequencerModeStatus.BackColor = Color.Transparent;
			ledSequencerModeStatus.LedColor = Color.Green;
			ledSequencerModeStatus.LedSize = new Size(11, 11);
			ledSequencerModeStatus.Location = new Point(542, 79);
			ledSequencerModeStatus.Name = "ledSequencerModeStatus";
			ledSequencerModeStatus.Size = new Size(15, 15);
			ledSequencerModeStatus.TabIndex = 37;
			ledSequencerModeStatus.Text = "Sequencer mode status";
			toolTip1.SetToolTip(ledSequencerModeStatus, resources.GetString("ledSequencerModeStatus.ToolTip"));
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(ledSequencerModeStatus);
			Controls.Add(lblTimer2Value);
			Controls.Add(btnSequencerStop);
			Controls.Add(lblTimer1Value);
			Controls.Add(btnSequencerStart);
			Controls.Add(nudSeqTimer2Coefficient);
			Controls.Add(nudSeqTimer1Coefficient);
			Controls.Add(label5);
			Controls.Add(label4);
			Controls.Add(label3);
			Controls.Add(label20);
			Controls.Add(label19);
			Controls.Add(label18);
			Controls.Add(label17);
			Controls.Add(label16);
			Controls.Add(label15);
			Controls.Add(label14);
			Controls.Add(label13);
			Controls.Add(label10);
			Controls.Add(label1);
			Controls.Add(label2);
			Controls.Add(label7);
			Controls.Add(label9);
			Controls.Add(label8);
			Controls.Add(label6);
			Controls.Add(label12);
			Controls.Add(label11);
			Controls.Add(label22);
			Controls.Add(cBoxFromPacketReceived);
			Controls.Add(cBoxFromRxTimeout);
			Controls.Add(cBoxFromReceive);
			Controls.Add(cBoxFromTransmit);
			Controls.Add(cBoxFromIdle);
			Controls.Add(cBoxSeqLowPowerState);
			Controls.Add(cBoxFromStart);
			Controls.Add(cBoxSeqLowPowerMode);
			Controls.Add(cBoxSeqTimer2Resolution);
			Controls.Add(cBoxTimer1Resolution);
			Name = "SequencerViewControl";
			Size = new Size(799, 493);
			((ISupportInitialize)errorProvider).EndInit();
			((ISupportInitialize)nudSeqTimer2Coefficient).EndInit();
			((ISupportInitialize)nudSeqTimer1Coefficient).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		public SequencerViewControl()
		{
			InitializeComponent();
		}

		private void OnSequencerStartChanged()
		{
            SequencerStartChanged?.Invoke(this, EventArgs.Empty);
        }

		private void OnSequencerStopChanged()
		{
            SequencerStopChanged?.Invoke(this, EventArgs.Empty);
        }

		private void OnIdleModeChanged(IdleMode value)
		{
            IdleModeChanged?.Invoke(this, new IdleModeEventArg(value));
        }

		private void OnFromStartChanged(FromStart value)
		{
            FromStartChanged?.Invoke(this, new FromStartEventArg(value));
        }

		private void OnLowPowerSelectionChanged(LowPowerSelection value)
		{
            LowPowerSelectionChanged?.Invoke(this, new LowPowerSelectionEventArg(value));
        }

		private void OnFromIdleChanged(FromIdle value)
		{
            FromIdleChanged?.Invoke(this, new FromIdleEventArg(value));
        }

		private void OnFromTransmitChanged(FromTransmit value)
		{
            FromTransmitChanged?.Invoke(this, new FromTransmitEventArg(value));
        }

		private void OnFromReceiveChanged(FromReceive value)
		{
            FromReceiveChanged?.Invoke(this, new FromReceiveEventArg(value));
        }

		private void OnFromRxTimeoutChanged(FromRxTimeout value)
		{
            FromRxTimeoutChanged?.Invoke(this, new FromRxTimeoutEventArg(value));
        }

		private void OnFromPacketReceivedChanged(FromPacketReceived value)
		{
            FromPacketReceivedChanged?.Invoke(this, new FromPacketReceivedEventArg(value));
        }

		private void OnTimer1ResolutionChanged(TimerResolution value)
		{
            Timer1ResolutionChanged?.Invoke(this, new TimerResolutionEventArg(value));
        }

		private void OnTimer2ResolutionChanged(TimerResolution value)
		{
            Timer2ResolutionChanged?.Invoke(this, new TimerResolutionEventArg(value));
        }

		private void OnTimer1CoefChanged(byte value)
		{
            Timer1CoefChanged?.Invoke(this, new ByteEventArg(value));
        }

		private void OnTimer2CoefChanged(byte value)
		{
            Timer2CoefChanged?.Invoke(this, new ByteEventArg(value));
        }

		private void btnSequencerStart_Click(object sender, EventArgs e)
		{
			ledSequencerModeStatus.Checked = true;
			OnSequencerStartChanged();
		}

		private void btnSequencerStop_Click(object sender, EventArgs e)
		{
			ledSequencerModeStatus.Checked = false;
			OnSequencerStopChanged();
		}

		private void cBoxSeqLowPowerMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			IdleMode = (IdleMode)cBoxSeqLowPowerMode.SelectedIndex;
			OnIdleModeChanged(IdleMode);
		}

		private void cBoxFromStart_SelectedIndexChanged(object sender, EventArgs e)
		{
			FromStart = (FromStart)cBoxFromStart.SelectedIndex;
			OnFromStartChanged(FromStart);
		}

		private void cBoxSeqLowPowerState_SelectedIndexChanged(object sender, EventArgs e)
		{
			LowPowerSelection = (LowPowerSelection)cBoxSeqLowPowerState.SelectedIndex;
			OnLowPowerSelectionChanged(LowPowerSelection);
		}

		private void cBoxFromIdle_SelectedIndexChanged(object sender, EventArgs e)
		{
			FromIdle = (FromIdle)cBoxFromIdle.SelectedIndex;
			OnFromIdleChanged(FromIdle);
		}

		private void cBoxFromTransmit_SelectedIndexChanged(object sender, EventArgs e)
		{
			FromTransmit = (FromTransmit)cBoxFromTransmit.SelectedIndex;
			OnFromTransmitChanged(FromTransmit);
		}

		private void cBoxFromReceive_SelectedIndexChanged(object sender, EventArgs e)
		{
			FromReceive = (FromReceive)cBoxFromReceive.SelectedIndex;
			OnFromReceiveChanged(FromReceive);
		}

		private void cBoxFromRxTimeout_SelectedIndexChanged(object sender, EventArgs e)
		{
			FromRxTimeout = (FromRxTimeout)cBoxFromRxTimeout.SelectedIndex;
			OnFromRxTimeoutChanged(FromRxTimeout);
		}

		private void cBoxFromPacketReceived_SelectedIndexChanged(object sender, EventArgs e)
		{
			FromPacketReceived = (FromPacketReceived)cBoxFromPacketReceived.SelectedIndex;
			OnFromPacketReceivedChanged(FromPacketReceived);
		}

		private void cBoxTimer1Resolution_SelectedIndexChanged(object sender, EventArgs e)
		{
			Timer1Resolution = (TimerResolution)cBoxTimer1Resolution.SelectedIndex;
			OnTimer1ResolutionChanged(Timer1Resolution);
		}

		private void cBoxSeqTimer2Resolution_SelectedIndexChanged(object sender, EventArgs e)
		{
			Timer2Resolution = (TimerResolution)cBoxSeqTimer2Resolution.SelectedIndex;
			OnTimer2ResolutionChanged(Timer2Resolution);
		}

		private void nudSeqTimer1Coefficient_ValueChanged(object sender, EventArgs e)
		{
			Timer1Coef = (byte)nudSeqTimer1Coefficient.Value;
			OnTimer1CoefChanged(Timer1Coef);
		}

		private void nudSeqTimer2Coefficient_ValueChanged(object sender, EventArgs e)
		{
			Timer2Coef = (byte)nudSeqTimer2Coefficient.Value;
			OnTimer2CoefChanged(Timer2Coef);
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
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
