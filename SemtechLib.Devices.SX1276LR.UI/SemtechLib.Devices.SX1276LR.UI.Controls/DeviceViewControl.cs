using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SemtechLib.Controls;
using SemtechLib.Devices.Common.Events;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276LR.Enumerations;
using SemtechLib.Devices.SX1276LR.Events;
using SemtechLib.Devices.SX1276LR.UI.Forms;
using SemtechLib.General;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;

namespace SemtechLib.Devices.SX1276LR.UI.Controls
{
	public sealed class DeviceViewControl : UserControl, IDeviceView, IDisposable, INotifyDocumentationChanged
	{
		private delegate void DevicePropertyChangedDelegate(object sender, PropertyChangedEventArgs e);

		private delegate void DevicePacketHandlerStartedDelegate(object sender, EventArgs e);

		private delegate void DevicePacketHandlerStopedDelegate(object sender, EventArgs e);

		private delegate void DevicePacketHandlerTransmittedDelegate(object sender, PacketStatusEventArg e);

        private SX1276LR device;

		private IContainer components;

		private TabControl tabControl1;

		private TabPage tabCommon;

		private CommonViewControl commonViewControl1;

		private GroupBoxEx gBoxOperatingMode;

		private RadioButton rBtnReceiver;

		private RadioButton rBtnReceiverSingle;

		private RadioButton rBtnReceiverCad;

		private RadioButton rBtnSynthesizerRx;

		private RadioButton rBtnStandby;

		private RadioButton rBtnSleep;

		private RadioButton rBtnTransmitterContinuous;

		private Led ledRxTimeout;

		private Label lbModeReady;

		private Label label19;

		private Label label18;

		private Led ledValidHeader;

		private Label label17;

		private Led ledPayloadCrcError;

		private Led ledRxDone;

		private Led ledCadDetected;

		private Label label23;

		private Label label22;

		private Led ledFhssChangeChannel;

		private Label label21;

		private Label label20;

		private Led ledCadDone;

		private Led ledTxDone;

		private GroupBoxEx gBoxIrqFlags;

		private RadioButton rBtnSynthesizerTx;

		private RadioButton rBtnSynthesizer;

		private Led ledFifoNotEmpty;

		private Led ledModeReady;

		private TabPage tabLoRa;

		private LoRaViewControl loRaViewControl1;

		private GroupBoxEx groupBoxEx1;

		private Label label42;

		private Led ledSignalDetected;

		private Label label45;

		private Led ledSignalSynchronized;

		private Label label43;

		private Led ledRxOnGoing;

		private Label label41;

		private Led ledHeaderInfoValid;

		private Label label44;

		private Led ledModemClear;

        public ApplicationSettings AppSettings { get; set; }

        public IDevice Device
		{
			get => device;
			set
			{
				if (device != value)
				{
					device = (SX1276LR)value;
					device.PropertyChanged += device_PropertyChanged;
					device.OcpTrimLimitStatusChanged += device_OcpTrimLimitStatusChanged;
					device.FrequencyRfLimitStatusChanged += device_FrequencyRfLimitStatusChanged;
					device.BandwidthLimitStatusChanged += device_BandwidthLimitStatusChanged;
					device.PacketHandlerStarted += device_PacketHandlerStarted;
					device.PacketHandlerStoped += device_PacketHandlerStoped;
					device.PacketHandlerTransmitted += device_PacketHandlerTransmitted;
					device.PacketHandlerReceived += device_PacketHandlerReceived;
					commonViewControl1.FrequencyXo = device.FrequencyXo;
					commonViewControl1.FrequencyStep = device.FrequencyStep;
					commonViewControl1.FrequencyRf = device.FrequencyRf;
					LoadTestPage(device);
				}
			}
		}

		public new bool Enabled
		{
			get => base.Enabled;
			set
			{
				if (base.Enabled != value)
				{
					base.Enabled = value;
				}
			}
		}

		public event SemtechLib.General.Events.ErrorEventHandler Error;

		public event DocumentationChangedEventHandler DocumentationChanged;

		public DeviceViewControl()
		{
			InitializeComponent();
		}

		private void LoadTestPage(SX1276LR deviceN)
		{
			try
			{
				if (File.Exists(Application.StartupPath + "\\SemtechLib.Devices.SX1276LR.Test.dll"))
				{
					var assembly = Assembly.Load(Application.StartupPath + "\\SemtechLib.Devices.SX1276LR.Test.dll");
					var type = assembly.GetType("SemtechLib.Devices.SX1276LR.Test.Controls.TestTabPage");
					var obj = Activator.CreateInstance(type);
					type.InvokeMember("SuspendLayout", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null, obj, null);
					SuspendLayout();
					var property = type.GetProperty("Location");
					property.SetValue(obj, new Point(4, 22), null);
					property = type.GetProperty("Name");
					property.SetValue(obj, "tabTest", null);
					property = type.GetProperty("Size");
					property.SetValue(obj, new Size(799, 493), null);
					property = type.GetProperty("TabIndex");
					property.SetValue(obj, 6, null);
					property = type.GetProperty("Text");
					property.SetValue(obj, "R&D Tests", null);
					property = type.GetProperty("UseVisualStyleBackColor");
					property.SetValue(obj, true, null);
					property = type.GetProperty("SX1276LR");
					property.SetValue(obj, deviceN, null);
					tabControl1.Controls.Add((Control)obj);
					type.InvokeMember("ResumeLayout", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null, obj, [false]);
					ResumeLayout(performLayout: false);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
			case "IsDebugOn":
				loRaViewControl1.IsDebugOn = device.IsDebugOn;
				break;
			case "FrequencyXo":
				commonViewControl1.FrequencyXo = device.FrequencyXo;
				break;
			case "FrequencyStep":
				commonViewControl1.FrequencyStep = device.FrequencyStep;
				break;
			case "SymbolTime":
				loRaViewControl1.SymbolTime = device.SymbolTime;
				break;
			case "Mode":
				rBtnSleep.CheckedChanged -= rBtnOperatingMode_CheckedChanged;
				rBtnStandby.CheckedChanged -= rBtnOperatingMode_CheckedChanged;
				rBtnSynthesizerRx.CheckedChanged -= rBtnOperatingMode_CheckedChanged;
				rBtnSynthesizerTx.CheckedChanged -= rBtnOperatingMode_CheckedChanged;
				rBtnReceiver.CheckedChanged -= rBtnOperatingMode_CheckedChanged;
				switch (device.Mode)
				{
				case OperatingModeEnum.Sleep:
					rBtnSleep.Checked = true;
					break;
				case OperatingModeEnum.Stdby:
					rBtnStandby.Checked = true;
					break;
				case OperatingModeEnum.FsTx:
					rBtnSynthesizerTx.Checked = true;
					break;
				case OperatingModeEnum.FsRx:
					rBtnSynthesizerRx.Checked = true;
					break;
				case OperatingModeEnum.Rx:
					rBtnReceiver.Checked = true;
					break;
				case OperatingModeEnum.RxSingle:
					rBtnReceiverSingle.Checked = true;
					break;
				case OperatingModeEnum.Cad:
					rBtnReceiverCad.Checked = true;
					break;
				}
				rBtnSleep.CheckedChanged += rBtnOperatingMode_CheckedChanged;
				rBtnStandby.CheckedChanged += rBtnOperatingMode_CheckedChanged;
				rBtnSynthesizerRx.CheckedChanged += rBtnOperatingMode_CheckedChanged;
				rBtnSynthesizerTx.CheckedChanged += rBtnOperatingMode_CheckedChanged;
				rBtnReceiver.CheckedChanged += rBtnOperatingMode_CheckedChanged;
				loRaViewControl1.Mode = device.Mode;
				break;
			case "Band":
				commonViewControl1.Band = device.Band;
				break;
			case "LowFrequencyModeOn":
				commonViewControl1.LowFrequencyModeOn = device.LowFrequencyModeOn;
				break;
			case "FrequencyRf":
				commonViewControl1.FrequencyRf = device.FrequencyRf;
				break;
			case "FastHopOn":
				commonViewControl1.FastHopOn = device.FastHopOn;
				break;
			case "PaSelect":
				commonViewControl1.PaSelect = device.PaSelect;
				break;
			case "MaxOutputPower":
				commonViewControl1.MaxOutputPower = device.MaxOutputPower;
				break;
			case "OutputPower":
				commonViewControl1.OutputPower = device.OutputPower;
				break;
			case "ForceTxBandLowFrequencyOn":
				commonViewControl1.ForceTxBandLowFrequencyOn = device.ForceTxBandLowFrequencyOn;
				break;
			case "PaRamp":
				commonViewControl1.PaRamp = device.PaRamp;
				break;
			case "OcpOn":
				commonViewControl1.OcpOn = device.OcpOn;
				break;
			case "OcpTrim":
				commonViewControl1.OcpTrim = device.OcpTrim;
				break;
			case "Pa20dBm":
				commonViewControl1.Pa20dBm = device.Pa20dBm;
				break;
			case "PllBandwidth":
				commonViewControl1.PllBandwidth = device.PllBandwidth;
				break;
			case "LnaGain":
				commonViewControl1.LnaGain = device.LnaGain;
				break;
			case "ForceRxBandLowFrequencyOn":
				commonViewControl1.ForceRxBandLowFrequencyOn = device.ForceRxBandLowFrequencyOn;
				break;
			case "LnaBoost":
				commonViewControl1.LnaBoost = device.LnaBoost;
				break;
			case "AgcReference":
				commonViewControl1.AgcReference = device.AgcReference;
				break;
			case "AgcThresh1":
				commonViewControl1.AgcThresh1 = device.AgcThresh1;
				break;
			case "AgcThresh2":
				commonViewControl1.AgcThresh2 = device.AgcThresh2;
				break;
			case "AgcThresh3":
				commonViewControl1.AgcThresh3 = device.AgcThresh3;
				break;
			case "AgcThresh4":
				commonViewControl1.AgcThresh4 = device.AgcThresh4;
				break;
			case "AgcThresh5":
				commonViewControl1.AgcThresh5 = device.AgcThresh5;
				break;
			case "AgcReferenceLevel":
				commonViewControl1.AgcReferenceLevel = device.AgcReferenceLevel;
				break;
			case "AgcStep1":
				commonViewControl1.AgcStep1 = device.AgcStep1;
				break;
			case "AgcStep2":
				commonViewControl1.AgcStep2 = device.AgcStep2;
				break;
			case "AgcStep3":
				commonViewControl1.AgcStep3 = device.AgcStep3;
				break;
			case "AgcStep4":
				commonViewControl1.AgcStep4 = device.AgcStep4;
				break;
			case "AgcStep5":
				commonViewControl1.AgcStep5 = device.AgcStep5;
				break;
			case "RxTimeout":
				ledRxTimeout.Checked = device.RxTimeout;
				break;
			case "RxDone":
				ledRxDone.Checked = device.RxDone;
				break;
			case "PayloadCrcError":
				ledPayloadCrcError.Checked = device.PayloadCrcError;
				break;
			case "ValidHeader":
				ledValidHeader.Checked = device.ValidHeader;
				break;
			case "TxDone":
				ledTxDone.Checked = device.TxDone;
				break;
			case "CadDone":
				ledCadDone.Checked = device.CadDone;
				break;
			case "FhssChangeChannel":
				ledFhssChangeChannel.Checked = device.FhssChangeChannel;
				break;
			case "CadDetected":
				ledCadDetected.Checked = device.CadDetected;
				break;
			case "RxTimeoutMask":
				loRaViewControl1.RxTimeoutMask = device.RxTimeoutMask;
				break;
			case "RxDoneMask":
				loRaViewControl1.RxDoneMask = device.RxDoneMask;
				break;
			case "PayloadCrcErrorMask":
				loRaViewControl1.PayloadCrcErrorMask = device.PayloadCrcErrorMask;
				break;
			case "ValidHeaderMask":
				loRaViewControl1.ValidHeaderMask = device.ValidHeaderMask;
				break;
			case "TxDoneMask":
				loRaViewControl1.TxDoneMask = device.TxDoneMask;
				break;
			case "CadDoneMask":
				loRaViewControl1.CadDoneMask = device.CadDoneMask;
				break;
			case "FhssChangeChannelMask":
				loRaViewControl1.FhssChangeChannelMask = device.FhssChangeChannelMask;
				break;
			case "CadDetectedMask":
				loRaViewControl1.CadDetectedMask = device.CadDetectedMask;
				break;
			case "ImplicitHeaderModeOn":
				loRaViewControl1.ImplicitHeaderModeOn = device.ImplicitHeaderModeOn;
				break;
			case "AgcAutoOn":
				commonViewControl1.AgcAutoOn = device.AgcAutoOn;
				break;
			case "SymbTimeout":
				loRaViewControl1.SymbTimeout = device.SymbTimeout;
				break;
			case "PayloadCrcOn":
				loRaViewControl1.PayloadCrcOn = device.PayloadCrcOn;
				break;
			case "CodingRate":
				loRaViewControl1.CodingRate = device.CodingRate;
				break;
			case "PayloadLength":
				loRaViewControl1.PayloadLength = device.PayloadLength;
				break;
			case "PreambleLength":
				loRaViewControl1.PreambleLength = device.PreambleLength;
				break;
			case "Bandwidth":
				loRaViewControl1.Bandwidth = device.Bandwidth;
				break;
			case "SpreadingFactor":
				loRaViewControl1.SpreadingFactor = device.SpreadingFactor;
				break;
			case "FreqHoppingPeriod":
				loRaViewControl1.FreqHoppingPeriod = device.FreqHoppingPeriod;
				break;
			case "RxNbBytes":
				loRaViewControl1.RxNbBytes = device.RxNbBytes;
				break;
			case "PllTimeout":
				loRaViewControl1.PllTimeout = device.PllTimeout;
				break;
			case "RxPayloadCrcOn":
				loRaViewControl1.RxPayloadCrcOn = device.RxPayloadCrcOn;
				break;
			case "RxPayloadCodingRate":
				loRaViewControl1.RxPayloadCodingRate = device.RxPayloadCodingRate;
				break;
			case "ValidHeaderCnt":
				loRaViewControl1.ValidHeaderCnt = device.ValidHeaderCnt;
				break;
			case "ValidPacketCnt":
				loRaViewControl1.ValidPacketCnt = device.ValidPacketCnt;
				break;
			case "ModemClear":
				loRaViewControl1.ModemClear = device.ModemClear;
				ledModemClear.Checked = device.ModemClear;
				break;
			case "HeaderInfoValid":
				loRaViewControl1.HeaderInfoValid = device.HeaderInfoValid;
				ledHeaderInfoValid.Checked = device.HeaderInfoValid;
				break;
			case "RxOnGoing":
				loRaViewControl1.RxOnGoing = device.RxOnGoing;
				ledRxOnGoing.Checked = device.RxOnGoing;
				break;
			case "SignalSynchronized":
				loRaViewControl1.SignalSynchronized = device.SignalSynchronized;
				ledSignalSynchronized.Checked = device.SignalSynchronized;
				break;
			case "SignalDetected":
				loRaViewControl1.SignalDetected = device.SignalDetected;
				ledSignalDetected.Checked = device.SignalDetected;
				break;
			case "PktSnrValue":
				loRaViewControl1.PktSnrValue = device.PktSnrValue;
				break;
			case "RssiValue":
				loRaViewControl1.RssiValue = device.RssiValue;
				break;
			case "PktRssiValue":
				loRaViewControl1.PktRssiValue = device.PktRssiValue;
				break;
			case "HopChannel":
				loRaViewControl1.HopChannel = device.HopChannel;
				break;
			case "FifoRxCurrentAddr":
				loRaViewControl1.FifoRxCurrentAddr = device.FifoRxCurrentAddr;
				break;
			case "LowDatarateOptimize":
				loRaViewControl1.LowDatarateOptimize = device.LowDatarateOptimize;
				break;
			case "TcxoInputOn":
				commonViewControl1.TcxoInputOn = device.TcxoInputOn;
				break;
			case "Dio0Mapping":
				commonViewControl1.Dio0Mapping = device.Dio0Mapping;
				break;
			case "Dio1Mapping":
				commonViewControl1.Dio1Mapping = device.Dio1Mapping;
				break;
			case "Dio2Mapping":
				commonViewControl1.Dio2Mapping = device.Dio2Mapping;
				break;
			case "Dio3Mapping":
				commonViewControl1.Dio3Mapping = device.Dio3Mapping;
				break;
			case "Dio4Mapping":
				commonViewControl1.Dio4Mapping = device.Dio4Mapping;
				break;
			case "Dio5Mapping":
				commonViewControl1.Dio5Mapping = device.Dio5Mapping;
				break;
			case "Payload":
				loRaViewControl1.Payload = device.Payload;
				break;
			case "LogEnabled":
				loRaViewControl1.LogEnabled = device.LogEnabled;
				break;
			case "PacketModeTx":
				loRaViewControl1.PacketModeTx = device.PacketModeTx;
				break;
			case "FifoAddrPtr":
			case "FifoTxBaseAddr":
			case "FifoRxBaseAddr":
			case "Version":
			case "PacketModeRxSingle":
				break;
			}
		}

		private void OnDevicePacketHandlerStarted(object sender, EventArgs e)
		{
			DisableControls();
		}

		private void OnDevicePacketHandlerStoped(object sender, EventArgs e)
		{
			EnableControls();
			loRaViewControl1.StartStop = false;
		}

		private void OnDevicePacketHandlerTransmitted(object sender, PacketStatusEventArg e)
		{
			loRaViewControl1.PacketNumber = e.Number;
		}

		private void OnDevicePacketHandlerReceived(object sender, PacketStatusEventArg e)
		{
			loRaViewControl1.PacketNumber = e.Number;
		}

		private void OnError(byte status, string message)
		{
            Error?.Invoke(this, new SemtechLib.General.Events.ErrorEventArgs(status, message));
        }

		public new void Dispose()
		{
			base.Dispose();
		}

		private void DisableControls()
		{
			commonViewControl1.Enabled = false;
			gBoxOperatingMode.Enabled = false;
		}

		private void EnableControls()
		{
			commonViewControl1.Enabled = true;
			gBoxOperatingMode.Enabled = true;
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new DevicePropertyChangedDelegate(OnDevicePropertyChanged), sender, e);
			}
			else
			{
				OnDevicePropertyChanged(sender, e);
			}
		}

		private void device_OcpTrimLimitStatusChanged(object sender, LimitCheckStatusEventArg e)
		{
			commonViewControl1.UpdateOcpTrimLimits(e.Status, e.Message);
		}

		private void device_FrequencyRfLimitStatusChanged(object sender, LimitCheckStatusEventArg e)
		{
			commonViewControl1.UpdateFrequencyRfLimits(e.Status, e.Message);
		}

		private void device_BandwidthLimitStatusChanged(object sender, LimitCheckStatusEventArg e)
		{
			loRaViewControl1.UpdateBandwidthLimits(e.Status, e.Message);
		}

		private void device_PacketHandlerStarted(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new DevicePacketHandlerStartedDelegate(OnDevicePacketHandlerStarted), sender, e);
			}
			else
			{
				OnDevicePacketHandlerStarted(sender, e);
			}
		}

		private void device_PacketHandlerStoped(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new DevicePacketHandlerStopedDelegate(OnDevicePacketHandlerStoped), sender, e);
			}
			else
			{
				OnDevicePacketHandlerStoped(sender, e);
			}
		}

		private void device_PacketHandlerTransmitted(object sender, PacketStatusEventArg e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new DevicePacketHandlerTransmittedDelegate(OnDevicePacketHandlerTransmitted), sender, e);
			}
			else
			{
				OnDevicePacketHandlerTransmitted(sender, e);
			}
		}

		private void device_PacketHandlerReceived(object sender, PacketStatusEventArg e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new DevicePacketHandlerTransmittedDelegate(OnDevicePacketHandlerTransmitted), sender, e);
			}
			else
			{
				OnDevicePacketHandlerReceived(sender, e);
			}
		}

		private void commonViewControl1_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			OnDocumentationChanged(e);
		}

		private void commonViewControl1_FrequencyXoChanged(object sender, DecimalEventArg e)
		{
			device.FrequencyXo = commonViewControl1.FrequencyXo;
		}

		private void rBtnOperatingMode_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				if (rBtnSleep.Checked)
				{
					device.SetOperatingMode(OperatingModeEnum.Sleep);
				}
				else if (rBtnStandby.Checked)
				{
					device.SetOperatingMode(OperatingModeEnum.Stdby);
				}
				else if (rBtnSynthesizerTx.Checked)
				{
					device.SetOperatingMode(OperatingModeEnum.FsTx);
				}
				else if (rBtnTransmitterContinuous.Checked)
				{
					device.PacketModeTx = true;
					device.SetOperatingMode(OperatingModeEnum.TxContinuous);
				}
				else if (rBtnSynthesizerRx.Checked)
				{
					device.SetOperatingMode(OperatingModeEnum.FsRx);
				}
				else if (rBtnReceiver.Checked)
				{
					device.PacketModeRxSingle = false;
					device.PacketModeTx = false;
					device.SetOperatingMode(OperatingModeEnum.Rx);
				}
				else if (rBtnReceiverSingle.Checked)
				{
					device.PacketModeRxSingle = true;
					device.PacketModeTx = false;
					loRaViewControl1.StartStop = true;
				}
				else if (rBtnReceiverCad.Checked)
				{
					device.SetOperatingMode(OperatingModeEnum.Cad);
				}
				loRaViewControl1.Mode = device.Mode;
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_BandChanged(object sender, BandEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetBand(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_LowFrequencyModeOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetLowFrequencyModeOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_FrequencyRfChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFrequencyRf(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_FastHopOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFastHopOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_TcxoInputChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetTcxoInputOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_PaModeChanged(object sender, PaModeEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPaMode(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_MaxOutputPowerChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetMaxOutputPower(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_OutputPowerChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetOutputPower(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_ForceTxBandLowFrequencyOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetForceTxBandLowFrequencyOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_PaRampChanged(object sender, PaRampEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPaRamp(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_OcpOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetOcpOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_OcpTrimChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetOcpTrim(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_Pa20dBmChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPa20dBm(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_PllBandwidthChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPllBandwidth(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_AgcReferenceLevelChanged(object sender, Int32EventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetAgcReferenceLevel(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_AgcStepChanged(object sender, AgcStepEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetAgcStep(e.Id, e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_LnaGainChanged(object sender, LnaGainEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetLnaGain(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_ForceRxBandLowFrequencyOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetForceRxBandLowFrequencyOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_LnaBoostChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetLnaBoost(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_AgcAutoOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetAgcAutoOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_DioMappingChanged(object sender, DioMappingEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetDioMapping(e.Id, e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_ClockOutChanged(object sender, ClockOutEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_Error(object sender, SemtechLib.General.Events.ErrorEventArgs e)
		{
			OnError(e.Status, e.Message);
		}

		private void ledRxTimeout_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.ClrRxTimeoutIrq();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void ledRxDone_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.ClrRxDoneIrq();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void ledPayloadCrcError_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.ClrPayloadCrcErrorIrq();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void ledValidHeader_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.ClrValidHeaderIrq();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void ledTxDone_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.ClrTxDoneIrq();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void ledCadDone_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.ClrCadDoneIrq();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void ledFhssChangeChannel_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.ClrFhssChangeChannelIrq();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void ledCadDetected_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.ClrCadDetectedIrq();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_RxTimeoutMaskChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetRxTimeoutMask(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_RxDoneMaskChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetRxDoneMask(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_PayloadCrcErrorMaskChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPayloadCrcErrorMask(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_ValidHeaderMaskChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetValidHeaderMask(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_TxDoneMaskChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetTxDoneMask(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_CadDoneMaskChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetCadDoneMask(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_FhssChangeChannelMaskChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFhssChangeChannelMask(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_CadDetectedMaskChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetCadDetectedMask(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_ImplicitHeaderModeOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetImplicitHeaderModeOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_SymbTimeoutChanged(object sender, DecimalEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetSymbTimeout(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_PayloadCrcOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPayloadCrcOn(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_CodingRateChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetCodingRate(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_PayloadLengthChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPayloadLength(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_PreambleLengthChanged(object sender, Int32EventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPreambleLength(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_BandwidthChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetBandwidth(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_SpreadingFactorChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetSpreadingFactor(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_FreqHoppingPeriodChanged(object sender, ByteEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetFreqHoppingPeriod(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_MessageChanged(object sender, ByteArrayEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetMessage(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_StartStopChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetPacketHandlerStartStop(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_MaxPacketNumberChanged(object sender, Int32EventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetMaxPacketNumber(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_PacketHandlerLog(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
                var packetLogForm = new PacketLogForm
                {
                    Device = device,
                    AppSettings = AppSettings
                };
                packetLogForm.ShowDialog();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_PacketModeTxChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.PacketModeTx = e.Value;
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void loRaViewControl1_LowDatarateOptimizeChanged(object sender, BooleanEventArg e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.SetLowDatarateOptimize(e.Value);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
			if (sender == gBoxIrqFlags)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("IrqMap", "Irq flags"));
			}
			else if (sender == gBoxOperatingMode)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "Operating mode"));
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
			tabControl1 = new TabControl();
			tabCommon = new TabPage();
			commonViewControl1 = new CommonViewControl();
			tabLoRa = new TabPage();
			loRaViewControl1 = new LoRaViewControl();
			rBtnSynthesizer = new RadioButton();
			gBoxOperatingMode = new GroupBoxEx();
			rBtnTransmitterContinuous = new RadioButton();
			rBtnReceiverSingle = new RadioButton();
			rBtnReceiverCad = new RadioButton();
			rBtnSynthesizerTx = new RadioButton();
			rBtnSynthesizerRx = new RadioButton();
			rBtnStandby = new RadioButton();
			rBtnSleep = new RadioButton();
			rBtnReceiver = new RadioButton();
			gBoxIrqFlags = new GroupBoxEx();
			lbModeReady = new Label();
			ledRxDone = new Led();
			ledPayloadCrcError = new Led();
			label17 = new Label();
			ledValidHeader = new Led();
			label18 = new Label();
			label19 = new Label();
			ledRxTimeout = new Led();
			ledTxDone = new Led();
			ledCadDone = new Led();
			label20 = new Label();
			label21 = new Label();
			ledFhssChangeChannel = new Led();
			label22 = new Label();
			label23 = new Label();
			ledCadDetected = new Led();
			ledFifoNotEmpty = new Led();
			ledModeReady = new Led();
			groupBoxEx1 = new GroupBoxEx();
			label42 = new Label();
			ledSignalDetected = new Led();
			label45 = new Label();
			ledSignalSynchronized = new Led();
			label43 = new Label();
			ledRxOnGoing = new Led();
			label41 = new Label();
			ledHeaderInfoValid = new Led();
			label44 = new Label();
			ledModemClear = new Led();
			tabControl1.SuspendLayout();
			tabCommon.SuspendLayout();
			tabLoRa.SuspendLayout();
			gBoxOperatingMode.SuspendLayout();
			gBoxIrqFlags.SuspendLayout();
			groupBoxEx1.SuspendLayout();
			SuspendLayout();
			tabControl1.Controls.Add(tabCommon);
			tabControl1.Controls.Add(tabLoRa);
			tabControl1.Location = new Point(3, 3);
			tabControl1.Name = "tabControl1";
			tabControl1.SelectedIndex = 0;
			tabControl1.Size = new Size(807, 519);
			tabControl1.TabIndex = 0;
			tabCommon.Controls.Add(commonViewControl1);
			tabCommon.Location = new Point(4, 22);
			tabCommon.Name = "tabCommon";
			tabCommon.Padding = new Padding(3);
			tabCommon.Size = new Size(799, 493);
			tabCommon.TabIndex = 0;
			tabCommon.Text = "Common";
			tabCommon.UseVisualStyleBackColor = true;
			commonViewControl1.AgcAutoOn = true;
			commonViewControl1.AgcReference = 19;
			commonViewControl1.AgcReferenceLevel = 19;
			commonViewControl1.AgcStep1 = 16;
			commonViewControl1.AgcStep2 = 7;
			commonViewControl1.AgcStep3 = 11;
			commonViewControl1.AgcStep4 = 9;
			commonViewControl1.AgcStep5 = 11;
			commonViewControl1.AgcThresh1 = 0;
			commonViewControl1.AgcThresh2 = 0;
			commonViewControl1.AgcThresh3 = 0;
			commonViewControl1.AgcThresh4 = 0;
			commonViewControl1.AgcThresh5 = 0;
			commonViewControl1.Band = BandEnum.AUTO;
			commonViewControl1.Dio0Mapping = DioMappingEnum.DIO_MAP_00;
			commonViewControl1.Dio1Mapping = DioMappingEnum.DIO_MAP_00;
			commonViewControl1.Dio2Mapping = DioMappingEnum.DIO_MAP_00;
			commonViewControl1.Dio3Mapping = DioMappingEnum.DIO_MAP_00;
			commonViewControl1.Dio4Mapping = DioMappingEnum.DIO_MAP_00;
			commonViewControl1.Dio5Mapping = DioMappingEnum.DIO_MAP_00;
			commonViewControl1.FastHopOn = true;
			commonViewControl1.ForceRxBandLowFrequencyOn = true;
			commonViewControl1.ForceTxBandLowFrequencyOn = true;
			commonViewControl1.FrequencyRf = new decimal([915000000, 0, 0, 0]);
			commonViewControl1.FrequencyStep = new decimal([61, 0, 0, 0]);
			commonViewControl1.FrequencyXo = new decimal([32000000, 0, 0, 0]);
			commonViewControl1.LnaBoost = true;
			commonViewControl1.Location = new Point(0, 0);
			commonViewControl1.LowFrequencyModeOn = true;
			commonViewControl1.MaxOutputPower = new decimal([132, 0, 0, 65536]);
			commonViewControl1.Name = "commonViewControl1";
			commonViewControl1.OcpOn = true;
			commonViewControl1.OcpTrim = new decimal([1000, 0, 0, 65536]);
			commonViewControl1.OutputPower = new decimal([132, 0, 0, 65536]);
			commonViewControl1.Pa20dBm = false;
			commonViewControl1.PaRamp = PaRampEnum.PaRamp_40;
			commonViewControl1.PaSelect = PaSelectEnum.RFO;
			commonViewControl1.PllBandwidth = new decimal([300000, 0, 0, 0]);
			commonViewControl1.Size = new Size(799, 493);
			commonViewControl1.TabIndex = 0;
			commonViewControl1.TcxoInputOn = true;
			commonViewControl1.FrequencyXoChanged += commonViewControl1_FrequencyXoChanged;
			commonViewControl1.BandChanged += commonViewControl1_BandChanged;
			commonViewControl1.ForceTxBandLowFrequencyOnChanged += commonViewControl1_ForceTxBandLowFrequencyOnChanged;
			commonViewControl1.ForceRxBandLowFrequencyOnChanged += commonViewControl1_ForceRxBandLowFrequencyOnChanged;
			commonViewControl1.LowFrequencyModeOnChanged += commonViewControl1_LowFrequencyModeOnChanged;
			commonViewControl1.FrequencyRfChanged += commonViewControl1_FrequencyRfChanged;
			commonViewControl1.FastHopOnChanged += commonViewControl1_FastHopOnChanged;
			commonViewControl1.PaModeChanged += commonViewControl1_PaModeChanged;
			commonViewControl1.OutputPowerChanged += commonViewControl1_OutputPowerChanged;
			commonViewControl1.MaxOutputPowerChanged += commonViewControl1_MaxOutputPowerChanged;
			commonViewControl1.PaRampChanged += commonViewControl1_PaRampChanged;
			commonViewControl1.OcpOnChanged += commonViewControl1_OcpOnChanged;
			commonViewControl1.OcpTrimChanged += commonViewControl1_OcpTrimChanged;
			commonViewControl1.Pa20dBmChanged += commonViewControl1_Pa20dBmChanged;
			commonViewControl1.PllBandwidthChanged += commonViewControl1_PllBandwidthChanged;
			commonViewControl1.AgcReferenceLevelChanged += commonViewControl1_AgcReferenceLevelChanged;
			commonViewControl1.AgcStepChanged += commonViewControl1_AgcStepChanged;
			commonViewControl1.LnaGainChanged += commonViewControl1_LnaGainChanged;
			commonViewControl1.LnaBoostChanged += commonViewControl1_LnaBoostChanged;
			commonViewControl1.AgcAutoOnChanged += commonViewControl1_AgcAutoOnChanged;
			commonViewControl1.TcxoInputChanged += commonViewControl1_TcxoInputChanged;
			commonViewControl1.DioMappingChanged += commonViewControl1_DioMappingChanged;
			commonViewControl1.DocumentationChanged += commonViewControl1_DocumentationChanged;
			tabLoRa.Controls.Add(loRaViewControl1);
			tabLoRa.Location = new Point(4, 22);
			tabLoRa.Name = "tabLoRa";
			tabLoRa.Padding = new Padding(3);
			tabLoRa.Size = new Size(799, 493);
			tabLoRa.TabIndex = 1;
			tabLoRa.Text = "LoRa";
			tabLoRa.UseVisualStyleBackColor = true;
			loRaViewControl1.Bandwidth = 7;
			loRaViewControl1.CadDetectedMask = true;
			loRaViewControl1.CadDoneMask = true;
			loRaViewControl1.CodingRate = 1;
			loRaViewControl1.FhssChangeChannelMask = true;
			loRaViewControl1.FreqHoppingPeriod = 0;
			loRaViewControl1.FrequencyXo = new decimal([32000000, 0, 0, 0]);
			loRaViewControl1.ImplicitHeaderModeOn = true;
			loRaViewControl1.Location = new Point(0, 0);
			loRaViewControl1.LogEnabled = false;
			loRaViewControl1.MaxPacketNumber = 0;
			loRaViewControl1.LowDatarateOptimize = true;
			loRaViewControl1.Mode = OperatingModeEnum.Stdby;
			loRaViewControl1.Name = "loRaViewControl1";
			loRaViewControl1.PacketModeTx = false;
			loRaViewControl1.PacketNumber = 0;
			loRaViewControl1.Payload = [];
			loRaViewControl1.PayloadCrcErrorMask = true;
			loRaViewControl1.PayloadCrcOn = false;
			loRaViewControl1.PayloadLength = 14;
			loRaViewControl1.PreambleLength = 8;
			loRaViewControl1.RxDoneMask = true;
			loRaViewControl1.RxTimeoutMask = true;
			loRaViewControl1.Size = new Size(799, 493);
			loRaViewControl1.SpreadingFactor = 7;
			loRaViewControl1.StartStop = false;
			loRaViewControl1.SymbolTime = new decimal([1024, 0, 0, 393216]);
			loRaViewControl1.SymbTimeout = new decimal([1024, 0, 0, 262144]);
			loRaViewControl1.TabIndex = 0;
			loRaViewControl1.TxDoneMask = true;
			loRaViewControl1.ValidHeaderMask = true;
			loRaViewControl1.Error += loRaViewControl1_Error;
			loRaViewControl1.RxTimeoutMaskChanged += loRaViewControl1_RxTimeoutMaskChanged;
			loRaViewControl1.RxDoneMaskChanged += loRaViewControl1_RxDoneMaskChanged;
			loRaViewControl1.PayloadCrcErrorMaskChanged += loRaViewControl1_PayloadCrcErrorMaskChanged;
			loRaViewControl1.ValidHeaderMaskChanged += loRaViewControl1_ValidHeaderMaskChanged;
			loRaViewControl1.TxDoneMaskChanged += loRaViewControl1_TxDoneMaskChanged;
			loRaViewControl1.CadDoneMaskChanged += loRaViewControl1_CadDoneMaskChanged;
			loRaViewControl1.FhssChangeChannelMaskChanged += loRaViewControl1_FhssChangeChannelMaskChanged;
			loRaViewControl1.CadDetectedMaskChanged += loRaViewControl1_CadDetectedMaskChanged;
			loRaViewControl1.ImplicitHeaderModeOnChanged += loRaViewControl1_ImplicitHeaderModeOnChanged;
			loRaViewControl1.SymbTimeoutChanged += loRaViewControl1_SymbTimeoutChanged;
			loRaViewControl1.PayloadCrcOnChanged += loRaViewControl1_PayloadCrcOnChanged;
			loRaViewControl1.CodingRateChanged += loRaViewControl1_CodingRateChanged;
			loRaViewControl1.PayloadLengthChanged += loRaViewControl1_PayloadLengthChanged;
			loRaViewControl1.PreambleLengthChanged += loRaViewControl1_PreambleLengthChanged;
			loRaViewControl1.BandwidthChanged += loRaViewControl1_BandwidthChanged;
			loRaViewControl1.SpreadingFactorChanged += loRaViewControl1_SpreadingFactorChanged;
			loRaViewControl1.FreqHoppingPeriodChanged += loRaViewControl1_FreqHoppingPeriodChanged;
			loRaViewControl1.MessageChanged += loRaViewControl1_MessageChanged;
			loRaViewControl1.StartStopChanged += loRaViewControl1_StartStopChanged;
			loRaViewControl1.MaxPacketNumberChanged += loRaViewControl1_MaxPacketNumberChanged;
			loRaViewControl1.PacketHandlerLog += loRaViewControl1_PacketHandlerLog;
			loRaViewControl1.PacketModeTxChanged += loRaViewControl1_PacketModeTxChanged;
			loRaViewControl1.LowDatarateOptimizeChanged += loRaViewControl1_LowDatarateOptimizeChanged;
			loRaViewControl1.DocumentationChanged += commonViewControl1_DocumentationChanged;
			rBtnSynthesizer.AutoSize = true;
			rBtnSynthesizer.Location = new Point(16, 51);
			rBtnSynthesizer.Name = "rBtnSynthesizer";
			rBtnSynthesizer.Size = new Size(79, 17);
			rBtnSynthesizer.TabIndex = 2;
			rBtnSynthesizer.Text = "Synthesizer";
			rBtnSynthesizer.UseVisualStyleBackColor = true;
			rBtnSynthesizer.CheckedChanged += rBtnOperatingMode_CheckedChanged;
			gBoxOperatingMode.Controls.Add(rBtnTransmitterContinuous);
			gBoxOperatingMode.Controls.Add(rBtnReceiverSingle);
			gBoxOperatingMode.Controls.Add(rBtnReceiverCad);
			gBoxOperatingMode.Controls.Add(rBtnSynthesizerTx);
			gBoxOperatingMode.Controls.Add(rBtnSynthesizerRx);
			gBoxOperatingMode.Controls.Add(rBtnStandby);
			gBoxOperatingMode.Controls.Add(rBtnSleep);
			gBoxOperatingMode.Controls.Add(rBtnReceiver);
			gBoxOperatingMode.Location = new Point(816, 367);
			gBoxOperatingMode.Name = "gBoxOperatingMode";
			gBoxOperatingMode.Size = new Size(189, 151);
			gBoxOperatingMode.TabIndex = 3;
			gBoxOperatingMode.TabStop = false;
			gBoxOperatingMode.Text = "Operating mode";
			gBoxOperatingMode.MouseEnter += control_MouseEnter;
			gBoxOperatingMode.MouseLeave += control_MouseLeave;
			rBtnTransmitterContinuous.AutoSize = true;
			rBtnTransmitterContinuous.Location = new Point(91, 87);
			rBtnTransmitterContinuous.Name = "rBtnTransmitterContinuous";
			rBtnTransmitterContinuous.Size = new Size(92, 17);
			rBtnTransmitterContinuous.TabIndex = 5;
			rBtnTransmitterContinuous.Text = "Tx continuous";
			rBtnTransmitterContinuous.UseVisualStyleBackColor = true;
			rBtnTransmitterContinuous.CheckedChanged += rBtnOperatingMode_CheckedChanged;
			rBtnReceiverSingle.AutoSize = true;
			rBtnReceiverSingle.Location = new Point(12, 110);
			rBtnReceiverSingle.Name = "rBtnReceiverSingle";
			rBtnReceiverSingle.Size = new Size(70, 17);
			rBtnReceiverSingle.TabIndex = 6;
			rBtnReceiverSingle.Text = "Rx Single";
			rBtnReceiverSingle.UseVisualStyleBackColor = true;
			rBtnReceiverSingle.CheckedChanged += rBtnOperatingMode_CheckedChanged;
			rBtnReceiverCad.AutoSize = true;
			rBtnReceiverCad.Location = new Point(91, 110);
			rBtnReceiverCad.Name = "rBtnReceiverCad";
			rBtnReceiverCad.Size = new Size(47, 17);
			rBtnReceiverCad.TabIndex = 7;
			rBtnReceiverCad.Text = "CAD";
			rBtnReceiverCad.UseVisualStyleBackColor = true;
			rBtnReceiverCad.CheckedChanged += rBtnOperatingMode_CheckedChanged;
			rBtnSynthesizerTx.AutoSize = true;
			rBtnSynthesizerTx.Location = new Point(91, 64);
			rBtnSynthesizerTx.Name = "rBtnSynthesizerTx";
			rBtnSynthesizerTx.Size = new Size(70, 17);
			rBtnSynthesizerTx.TabIndex = 3;
			rBtnSynthesizerTx.Text = "Synth. Tx";
			rBtnSynthesizerTx.UseVisualStyleBackColor = true;
			rBtnSynthesizerTx.CheckedChanged += rBtnOperatingMode_CheckedChanged;
			rBtnSynthesizerRx.AutoSize = true;
			rBtnSynthesizerRx.Location = new Point(12, 64);
			rBtnSynthesizerRx.Name = "rBtnSynthesizerRx";
			rBtnSynthesizerRx.Size = new Size(71, 17);
			rBtnSynthesizerRx.TabIndex = 2;
			rBtnSynthesizerRx.Text = "Synth. Rx";
			rBtnSynthesizerRx.UseVisualStyleBackColor = true;
			rBtnSynthesizerRx.CheckedChanged += rBtnOperatingMode_CheckedChanged;
			rBtnStandby.AutoSize = true;
			rBtnStandby.Checked = true;
			rBtnStandby.Location = new Point(91, 41);
			rBtnStandby.Name = "rBtnStandby";
			rBtnStandby.Size = new Size(64, 17);
			rBtnStandby.TabIndex = 1;
			rBtnStandby.TabStop = true;
			rBtnStandby.Text = "Standby";
			rBtnStandby.UseVisualStyleBackColor = true;
			rBtnStandby.CheckedChanged += rBtnOperatingMode_CheckedChanged;
			rBtnSleep.AutoSize = true;
			rBtnSleep.Location = new Point(12, 41);
			rBtnSleep.Name = "rBtnSleep";
			rBtnSleep.Size = new Size(52, 17);
			rBtnSleep.TabIndex = 0;
			rBtnSleep.Text = "Sleep";
			rBtnSleep.UseVisualStyleBackColor = true;
			rBtnSleep.CheckedChanged += rBtnOperatingMode_CheckedChanged;
			rBtnReceiver.AutoSize = true;
			rBtnReceiver.Location = new Point(12, 87);
			rBtnReceiver.Name = "rBtnReceiver";
			rBtnReceiver.Size = new Size(38, 17);
			rBtnReceiver.TabIndex = 4;
			rBtnReceiver.Text = "Rx";
			rBtnReceiver.UseVisualStyleBackColor = true;
			rBtnReceiver.CheckedChanged += rBtnOperatingMode_CheckedChanged;
			gBoxIrqFlags.Controls.Add(lbModeReady);
			gBoxIrqFlags.Controls.Add(ledRxDone);
			gBoxIrqFlags.Controls.Add(ledPayloadCrcError);
			gBoxIrqFlags.Controls.Add(label17);
			gBoxIrqFlags.Controls.Add(ledValidHeader);
			gBoxIrqFlags.Controls.Add(label18);
			gBoxIrqFlags.Controls.Add(label19);
			gBoxIrqFlags.Controls.Add(ledRxTimeout);
			gBoxIrqFlags.Controls.Add(ledTxDone);
			gBoxIrqFlags.Controls.Add(ledCadDone);
			gBoxIrqFlags.Controls.Add(label20);
			gBoxIrqFlags.Controls.Add(label21);
			gBoxIrqFlags.Controls.Add(ledFhssChangeChannel);
			gBoxIrqFlags.Controls.Add(label22);
			gBoxIrqFlags.Controls.Add(label23);
			gBoxIrqFlags.Controls.Add(ledCadDetected);
			gBoxIrqFlags.Location = new Point(816, 25);
			gBoxIrqFlags.Name = "gBoxIrqFlags";
			gBoxIrqFlags.Size = new Size(189, 188);
			gBoxIrqFlags.TabIndex = 1;
			gBoxIrqFlags.TabStop = false;
			gBoxIrqFlags.Text = "Irq flags";
			gBoxIrqFlags.MouseEnter += control_MouseEnter;
			gBoxIrqFlags.MouseLeave += control_MouseLeave;
			lbModeReady.AutoSize = true;
			lbModeReady.Location = new Point(55, 20);
			lbModeReady.Name = "lbModeReady";
			lbModeReady.Size = new Size(58, 13);
			lbModeReady.TabIndex = 1;
			lbModeReady.Text = "RxTimeout";
			ledRxDone.BackColor = Color.Transparent;
			ledRxDone.LedColor = Color.Green;
			ledRxDone.LedSize = new Size(11, 11);
			ledRxDone.Location = new Point(34, 40);
			ledRxDone.Name = "ledRxDone";
			ledRxDone.Size = new Size(15, 15);
			ledRxDone.TabIndex = 2;
			ledRxDone.Text = "Rx done";
			ledRxDone.Click += ledRxDone_Click;
			ledPayloadCrcError.BackColor = Color.Transparent;
			ledPayloadCrcError.LedColor = Color.Green;
			ledPayloadCrcError.LedSize = new Size(11, 11);
			ledPayloadCrcError.Location = new Point(34, 61);
			ledPayloadCrcError.Name = "ledPayloadCrcError";
			ledPayloadCrcError.Size = new Size(15, 15);
			ledPayloadCrcError.TabIndex = 4;
			ledPayloadCrcError.Text = "Payload CRC error";
			ledPayloadCrcError.Click += ledPayloadCrcError_Click;
			label17.AutoSize = true;
			label17.Location = new Point(55, 83);
			label17.Name = "label17";
			label17.Size = new Size(65, 13);
			label17.TabIndex = 7;
			label17.Text = "ValidHeader";
			ledValidHeader.BackColor = Color.Transparent;
			ledValidHeader.LedColor = Color.Green;
			ledValidHeader.LedSize = new Size(11, 11);
			ledValidHeader.Location = new Point(34, 82);
			ledValidHeader.Name = "ledValidHeader";
			ledValidHeader.Size = new Size(15, 15);
			ledValidHeader.TabIndex = 6;
			ledValidHeader.Text = "Valid header";
			ledValidHeader.Click += ledValidHeader_Click;
			label18.AutoSize = true;
			label18.Location = new Point(55, 62);
			label18.Name = "label18";
			label18.Size = new Size(83, 13);
			label18.TabIndex = 5;
			label18.Text = "PayloadCrcError";
			label19.AutoSize = true;
			label19.Location = new Point(55, 41);
			label19.Name = "label19";
			label19.Size = new Size(46, 13);
			label19.TabIndex = 3;
			label19.Text = "RxDone";
			ledRxTimeout.BackColor = Color.Transparent;
			ledRxTimeout.LedColor = Color.Green;
			ledRxTimeout.LedSize = new Size(11, 11);
			ledRxTimeout.Location = new Point(34, 19);
			ledRxTimeout.Name = "ledRxTimeout";
			ledRxTimeout.Size = new Size(15, 15);
			ledRxTimeout.TabIndex = 0;
			ledRxTimeout.Text = "Rx timeout";
			ledRxTimeout.Click += ledRxTimeout_Click;
			ledTxDone.BackColor = Color.Transparent;
			ledTxDone.LedColor = Color.Green;
			ledTxDone.LedSize = new Size(11, 11);
			ledTxDone.Location = new Point(34, 103);
			ledTxDone.Name = "ledTxDone";
			ledTxDone.Size = new Size(15, 15);
			ledTxDone.TabIndex = 8;
			ledTxDone.Text = "Tx done";
			ledTxDone.Click += ledTxDone_Click;
			ledCadDone.BackColor = Color.Transparent;
			ledCadDone.LedColor = Color.Green;
			ledCadDone.LedSize = new Size(11, 11);
			ledCadDone.Location = new Point(34, 124);
			ledCadDone.Name = "ledCadDone";
			ledCadDone.Size = new Size(15, 15);
			ledCadDone.TabIndex = 10;
			ledCadDone.Text = "CAD done";
			ledCadDone.Click += ledCadDone_Click;
			label20.AutoSize = true;
			label20.Location = new Point(55, 167);
			label20.Name = "label20";
			label20.Size = new Size(70, 13);
			label20.TabIndex = 15;
			label20.Text = "CadDetected";
			label21.AutoSize = true;
			label21.Location = new Point(55, 146);
			label21.Name = "label21";
			label21.Size = new Size(105, 13);
			label21.TabIndex = 13;
			label21.Text = "FhssChangeChannel";
			ledFhssChangeChannel.BackColor = Color.Transparent;
			ledFhssChangeChannel.LedColor = Color.Green;
			ledFhssChangeChannel.LedSize = new Size(11, 11);
			ledFhssChangeChannel.Location = new Point(34, 145);
			ledFhssChangeChannel.Name = "ledFhssChangeChannel";
			ledFhssChangeChannel.Size = new Size(15, 15);
			ledFhssChangeChannel.TabIndex = 12;
			ledFhssChangeChannel.Text = "FHSS change channel";
			ledFhssChangeChannel.Click += ledFhssChangeChannel_Click;
			label22.AutoSize = true;
			label22.Location = new Point(55, 125);
			label22.Name = "label22";
			label22.Size = new Size(52, 13);
			label22.TabIndex = 11;
			label22.Text = "CadDone";
			label23.AutoSize = true;
			label23.Location = new Point(55, 104);
			label23.Name = "label23";
			label23.Size = new Size(45, 13);
			label23.TabIndex = 9;
			label23.Text = "TxDone";
			ledCadDetected.BackColor = Color.Transparent;
			ledCadDetected.LedColor = Color.Green;
			ledCadDetected.LedSize = new Size(11, 11);
			ledCadDetected.Location = new Point(34, 166);
			ledCadDetected.Name = "ledCadDetected";
			ledCadDetected.Size = new Size(15, 15);
			ledCadDetected.TabIndex = 14;
			ledCadDetected.Text = "CAD detected";
			ledCadDetected.Click += ledCadDetected_Click;
			ledFifoNotEmpty.BackColor = Color.Transparent;
			ledFifoNotEmpty.LedColor = Color.Green;
			ledFifoNotEmpty.LedSize = new Size(11, 11);
			ledFifoNotEmpty.Location = new Point(34, 220);
			ledFifoNotEmpty.Name = "ledFifoNotEmpty";
			ledFifoNotEmpty.Size = new Size(15, 15);
			ledFifoNotEmpty.TabIndex = 18;
			ledFifoNotEmpty.Text = "led1";
			ledModeReady.BackColor = Color.Transparent;
			ledModeReady.LedColor = Color.Green;
			ledModeReady.LedSize = new Size(11, 11);
			ledModeReady.Location = new Point(34, 19);
			ledModeReady.Name = "ledModeReady";
			ledModeReady.Size = new Size(15, 15);
			ledModeReady.TabIndex = 0;
			ledModeReady.Text = "Rx timeout";
			groupBoxEx1.Controls.Add(label42);
			groupBoxEx1.Controls.Add(ledSignalDetected);
			groupBoxEx1.Controls.Add(label45);
			groupBoxEx1.Controls.Add(ledSignalSynchronized);
			groupBoxEx1.Controls.Add(label43);
			groupBoxEx1.Controls.Add(ledRxOnGoing);
			groupBoxEx1.Controls.Add(label41);
			groupBoxEx1.Controls.Add(ledHeaderInfoValid);
			groupBoxEx1.Controls.Add(label44);
			groupBoxEx1.Controls.Add(ledModemClear);
			groupBoxEx1.Location = new Point(816, 219);
			groupBoxEx1.Name = "groupBoxEx1";
			groupBoxEx1.Size = new Size(189, 142);
			groupBoxEx1.TabIndex = 2;
			groupBoxEx1.TabStop = false;
			groupBoxEx1.Text = "Modem status";
			label42.AutoSize = true;
			label42.Location = new Point(58, 28);
			label42.Name = "label42";
			label42.Size = new Size(68, 13);
			label42.TabIndex = 1;
			label42.Text = "Modem clear";
			ledSignalDetected.BackColor = Color.Transparent;
			ledSignalDetected.LedColor = Color.Green;
			ledSignalDetected.LedSize = new Size(11, 11);
			ledSignalDetected.Location = new Point(34, 111);
			ledSignalDetected.Name = "ledSignalDetected";
			ledSignalDetected.Size = new Size(15, 15);
			ledSignalDetected.TabIndex = 8;
			ledSignalDetected.Text = "Signal detected";
			label45.AutoSize = true;
			label45.Location = new Point(58, 112);
			label45.Name = "label45";
			label45.Size = new Size(81, 13);
			label45.TabIndex = 9;
			label45.Text = "Signal detected";
			ledSignalSynchronized.BackColor = Color.Transparent;
			ledSignalSynchronized.LedColor = Color.Green;
			ledSignalSynchronized.LedSize = new Size(11, 11);
			ledSignalSynchronized.Location = new Point(34, 90);
			ledSignalSynchronized.Name = "ledSignalSynchronized";
			ledSignalSynchronized.Size = new Size(15, 15);
			ledSignalSynchronized.TabIndex = 6;
			ledSignalSynchronized.Text = "Signal synchronized";
			label43.AutoSize = true;
			label43.Location = new Point(58, 91);
			label43.Name = "label43";
			label43.Size = new Size(101, 13);
			label43.TabIndex = 7;
			label43.Text = "Signal synchronized";
			ledRxOnGoing.BackColor = Color.Transparent;
			ledRxOnGoing.LedColor = Color.Green;
			ledRxOnGoing.LedSize = new Size(11, 11);
			ledRxOnGoing.Location = new Point(34, 69);
			ledRxOnGoing.Name = "ledRxOnGoing";
			ledRxOnGoing.Size = new Size(15, 15);
			ledRxOnGoing.TabIndex = 4;
			ledRxOnGoing.Text = "Rx on going";
			label41.AutoSize = true;
			label41.Location = new Point(58, 70);
			label41.Name = "label41";
			label41.Size = new Size(64, 13);
			label41.TabIndex = 5;
			label41.Text = "Rx on going";
			ledHeaderInfoValid.BackColor = Color.Transparent;
			ledHeaderInfoValid.LedColor = Color.Green;
			ledHeaderInfoValid.LedSize = new Size(11, 11);
			ledHeaderInfoValid.Location = new Point(34, 48);
			ledHeaderInfoValid.Name = "ledHeaderInfoValid";
			ledHeaderInfoValid.Size = new Size(15, 15);
			ledHeaderInfoValid.TabIndex = 2;
			ledHeaderInfoValid.Text = "Header info valid";
			label44.AutoSize = true;
			label44.Location = new Point(58, 49);
			label44.Name = "label44";
			label44.Size = new Size(87, 13);
			label44.TabIndex = 3;
			label44.Text = "Header info valid";
			ledModemClear.BackColor = Color.Transparent;
			ledModemClear.LedColor = Color.Green;
			ledModemClear.LedSize = new Size(11, 11);
			ledModemClear.Location = new Point(34, 27);
			ledModemClear.Name = "ledModemClear";
			ledModemClear.Size = new Size(15, 15);
			ledModemClear.TabIndex = 0;
			ledModemClear.Text = "Modem clear";
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(groupBoxEx1);
			Controls.Add(gBoxOperatingMode);
			Controls.Add(tabControl1);
			Controls.Add(gBoxIrqFlags);
			Name = "DeviceViewControl";
			Size = new Size(1008, 525);
			tabControl1.ResumeLayout(false);
			tabCommon.ResumeLayout(false);
			tabLoRa.ResumeLayout(false);
			gBoxOperatingMode.ResumeLayout(false);
			gBoxOperatingMode.PerformLayout();
			gBoxIrqFlags.ResumeLayout(false);
			gBoxIrqFlags.PerformLayout();
			groupBoxEx1.ResumeLayout(false);
			groupBoxEx1.PerformLayout();
			ResumeLayout(false);
		}
	/*
		[SpecialName]
		public bool get_Visible()
		{
			return Visible;
		}

		[SpecialName]
		public void set_Visible(bool P_0)
		{
			Visible = P_0;
		}

		[SpecialName]
		public string get_Name()
		{
			return Name;
		}

		[SpecialName]
		public void set_Name(string P_0)
		{
			Name = P_0;
		}

		[SpecialName]
		public int get_TabIndex()
		{
			return TabIndex;
		}

		[SpecialName]
		public void set_TabIndex(int P_0)
		{
			TabIndex = P_0;
		}
		*/
	}
}
