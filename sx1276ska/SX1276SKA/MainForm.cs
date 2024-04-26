using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SemtechLib.Controls;
using SemtechLib.Devices.Common.Events;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276;
using SemtechLib.Devices.SX1276.Common.UI.Forms;
using SemtechLib.Devices.SX1276.UI.Forms;
using SemtechLib.Devices.SX1276LR;
using SemtechLib.General;
using SemtechLib.General.Interfaces;
using SemtechLib.Properties;

namespace SX1276SKA
{
	public sealed class MainForm : Form
	{
		private delegate void DeviceDataChangedDelegate(object sender, PropertyChangedEventArgs e);

		private delegate void DevicePacketHandlerStartedDelegate(object sender, EventArgs e);

		private delegate void DevicePacketHandlerStopedDelegate(object sender, EventArgs e);

		private delegate void DevicePacketHandlerTransmittedDelegate(object sender, PacketStatusEventArg e);

		private delegate void ConnectedDelegate();

		private delegate void DisconnectedDelegate();

		private delegate void ErrorDelegate(byte status, string message);

		private const string RleaseCandidate = "";

		private const string ApplicationVersion = "";

		private IContainer components;

		private MenuStripEx msMainMenu;

		private ToolStripMenuItem fileToolStripMenuItem;

		private ToolStripMenuItem exitToolStripMenuItem;

		private ToolStripMenuItem helpToolStripMenuItem;

		private ToolStripMenuItem aboutToolStripMenuItem;

		private ToolStripMenuItem connectToolStripMenuItem;

		private ToolStripSeparator mFileSeparator1;

		private ToolStripMenuItem loadToolStripMenuItem;

		private ToolStripMenuItem saveToolStripMenuItem;

		private ToolStripSeparator mFileSeparator2;

		private ToolStripMenuItem saveAsToolStripMenuItem;

		private OpenFileDialog ofConfigFileOpenDlg;

		private SaveFileDialog sfConfigFileSaveDlg;

		private ToolStripStatusLabel tsLblConfigFileName;

		private ToolStripStatusLabel tsLblSeparator2;

		private ToolStripStatusLabel tsLblSeparator3;

		private ToolStripStatusLabel tsLblSeparator4;

		private StatusStrip ssMainStatus;

		private StatusStrip ssMainStatus1;

		private ToolStripMenuItem usersGuideToolStripMenuItem;

		private ToolStripSeparator mHelpSeparator2;

		private ToolStripStatusLabel tsLblStatus;

		private ToolStripSeparator toolStripSeparator3;

		private ToolTip tipMainForm;

		private ToolStripButton tsBtnRefresh;

		private ToolStripMenuItem actionToolStripMenuItem;

		private ToolStripMenuItem refreshToolStripMenuItem;

		private ToolStripContainer toolStripContainer1;

		private ToolStripEx tsActionToolbar;

		private ToolStripLabel toolStripLabel1;

		private ToolStripMenuItem showHelpToolStripMenuItem;

		private ToolStripSeparator mHelpSeparator1;

		private IDeviceView deviceViewControl;

		private ToolStripMenuItem showRegistersToolStripMenuItem;

		private ToolStripButton tsBtnShowRegisters;

		private ToolStripMenuItem resetToolStripMenuItem;

		private ToolStripButton tsBtnReset;

		private ToolStripStatusLabel tsLblSeparator1;

		private ToolStripStatusLabel tsChipVersion;

		private ToolStripMenuItem toolsToolStripMenuItem;

		private ToolStripStatusLabel tsLblChipVersion;

		private ToolStripMenuItem rssiAnalyserToolStripMenuItem;

		private ToolStripMenuItem spectrumAnalyserToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripMenuItem monitorToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator4;

		private ToolStripLabel tsLblMonitor;

		private ToolStripButton tsBtnMonitorOn;

		private ToolStripButton tsBtnMonitorOff;

		private ToolStripMenuItem monitorOffToolStripMenuItem;

		private ToolStripMenuItem monitorOnToolStripMenuItem;

		private ToolStripMenuItem startuptimeToolStripMenuItem;

		private ToolStripButton tsBtnStartupTime;

		private ToolStripStatusLabel tsLblVersion;

		private ToolStripStatusLabel tsVersion;

		private ToolStripStatusLabel tsLblFwVersion;

		private ToolStripStatusLabel tsFwVersion;

		private ToolStripStatusLabel tsLblConnectionStatus;

		private ToolStripLed ledStatus;

		private ToolStripButton tsBtnFwUpdate;

		private ToolStripButton tsBtnOpenFile;

		private ToolStripButton tsBtnSaveFile;

		private ToolStripSeparator tbFileSeparator1;

		private ToolStripButton tsBtnOpenDevice;

		private ToolStripSeparator toolStripSeparator7;

		private ToolStripLabel tsLblModem;

		private ToolStripButton tsBtnModemLoRa;

		private ToolStripButton tsBtnModemFsk;

		private ToolStripSeparator toolStripSeparator5;

		private ToolStripSeparator toolStripSeparator6;

		private ToolStripButton tsBtnShowHelp;

		private ToolStripMenuItem modemToolStripMenuItem;

		private ToolStripMenuItem modemFskToolStripMenuItem;

		private ToolStripMenuItem modemLoRaToolStripMenuItem;

		private ToolStripSeparator tsSeparatorPerModeOn;

		private ToolStripLabel tsLblPerModeOn;

		private ToolStripSeparator tsSeparatorDebugOn;

		private ToolStripLabel tsLblDebugOn;

		private readonly ApplicationSettings appSettings;

		private TestForm frmTest;

		private HelpForm frmHelp;

		private RegistersForm frmRegisters;

		private RssiAnalyserForm frmRssiAnalyser;

		private SpectrumAnalyserForm frmSpectrumAnalyser;

		private Form frmPacketLog;

		private RxTxStartupTimeForm frmStartupTime;

		private IDevice device;

		private readonly List<IDevice> deviceList =
		[
			new SX1276(),
			new SX1276LR()
		];

		private readonly List<IDeviceView> deviceViewList =
		[
			new SemtechLib.Devices.SX1276.UI.Controls.DeviceViewControl(),
			new SemtechLib.Devices.SX1276LR.UI.Controls.DeviceViewControl()
		];

		private FileStream configFileStream;

		private string fskConfigFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		private string fskConfigFileName = "sx1276ska-Fsk.cfg";

		private bool isFskConfigFileOpen;

		private string loRaConfigFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		private string loRaConfigFileName = "sx1276ska-LoRa.cfg";

		private bool isLoRaConfigFileOpen;

		private bool IsLoRaOn = true;

		private bool IsLoRaPacketUsePerOn;

		private bool IsDebugOn;

		private bool AppTestArg { get; }

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

		public string AssemblyVersion
		{
			get
			{
				var executingAssembly = Assembly.GetExecutingAssembly();
				var name = executingAssembly.GetName();
				if (name.Version.ToString() != "")
				{
					return name.Version.ToString();
				}
				return "-.-.-.-";
			}
		}

		protected override void Dispose(bool disposing)
		{
			appSettings.Dispose();
			deviceViewControl.Dispose();
			if (device != null)
			{
				device.Dispose();
			}
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			components = new Container();
			var resources = new ComponentResourceManager(typeof(MainForm));
			ssMainStatus = new StatusStrip();
			tsLblVersion = new ToolStripStatusLabel();
			tsVersion = new ToolStripStatusLabel();
			tsLblSeparator1 = new ToolStripStatusLabel();
			tsLblFwVersion = new ToolStripStatusLabel();
			tsFwVersion = new ToolStripStatusLabel();
			tsBtnFwUpdate = new ToolStripButton();
			tsLblSeparator2 = new ToolStripStatusLabel();
			tsLblChipVersion = new ToolStripStatusLabel();
			tsChipVersion = new ToolStripStatusLabel();
			tsLblSeparator3 = new ToolStripStatusLabel();
			tsLblConfigFileName = new ToolStripStatusLabel();
			tsLblSeparator4 = new ToolStripStatusLabel();
			tsLblConnectionStatus = new ToolStripStatusLabel();
			ledStatus = new ToolStripLed();
			tsLblStatus = new ToolStripStatusLabel();
			ssMainStatus1 = new StatusStrip();
			msMainMenu = new MenuStripEx();
			fileToolStripMenuItem = new ToolStripMenuItem();
			connectToolStripMenuItem = new ToolStripMenuItem();
			mFileSeparator1 = new ToolStripSeparator();
			loadToolStripMenuItem = new ToolStripMenuItem();
			saveToolStripMenuItem = new ToolStripMenuItem();
			saveAsToolStripMenuItem = new ToolStripMenuItem();
			mFileSeparator2 = new ToolStripSeparator();
			exitToolStripMenuItem = new ToolStripMenuItem();
			actionToolStripMenuItem = new ToolStripMenuItem();
			modemToolStripMenuItem = new ToolStripMenuItem();
			modemFskToolStripMenuItem = new ToolStripMenuItem();
			modemLoRaToolStripMenuItem = new ToolStripMenuItem();
			resetToolStripMenuItem = new ToolStripMenuItem();
			refreshToolStripMenuItem = new ToolStripMenuItem();
			showRegistersToolStripMenuItem = new ToolStripMenuItem();
			monitorToolStripMenuItem = new ToolStripMenuItem();
			monitorOffToolStripMenuItem = new ToolStripMenuItem();
			monitorOnToolStripMenuItem = new ToolStripMenuItem();
			startuptimeToolStripMenuItem = new ToolStripMenuItem();
			toolsToolStripMenuItem = new ToolStripMenuItem();
			rssiAnalyserToolStripMenuItem = new ToolStripMenuItem();
			spectrumAnalyserToolStripMenuItem = new ToolStripMenuItem();
			helpToolStripMenuItem = new ToolStripMenuItem();
			showHelpToolStripMenuItem = new ToolStripMenuItem();
			mHelpSeparator1 = new ToolStripSeparator();
			usersGuideToolStripMenuItem = new ToolStripMenuItem();
			mHelpSeparator2 = new ToolStripSeparator();
			aboutToolStripMenuItem = new ToolStripMenuItem();
			tsBtnRefresh = new ToolStripButton();
			toolStripSeparator3 = new ToolStripSeparator();
			ofConfigFileOpenDlg = new OpenFileDialog();
			sfConfigFileSaveDlg = new SaveFileDialog();
			tipMainForm = new ToolTip(components);
			toolStripContainer1 = new ToolStripContainer();
			tsActionToolbar = new ToolStripEx();
			tsBtnOpenFile = new ToolStripButton();
			tsBtnSaveFile = new ToolStripButton();
			tbFileSeparator1 = new ToolStripSeparator();
			tsBtnOpenDevice = new ToolStripButton();
			toolStripSeparator7 = new ToolStripSeparator();
			tsLblModem = new ToolStripLabel();
			tsBtnModemLoRa = new ToolStripButton();
			tsBtnModemFsk = new ToolStripButton();
			toolStripSeparator5 = new ToolStripSeparator();
			tsBtnReset = new ToolStripButton();
			toolStripSeparator1 = new ToolStripSeparator();
			tsBtnStartupTime = new ToolStripButton();
			toolStripSeparator2 = new ToolStripSeparator();
			tsBtnShowRegisters = new ToolStripButton();
			toolStripSeparator4 = new ToolStripSeparator();
			tsLblMonitor = new ToolStripLabel();
			tsBtnMonitorOn = new ToolStripButton();
			tsBtnMonitorOff = new ToolStripButton();
			toolStripSeparator6 = new ToolStripSeparator();
			tsBtnShowHelp = new ToolStripButton();
			tsSeparatorPerModeOn = new ToolStripSeparator();
			tsLblPerModeOn = new ToolStripLabel();
			tsSeparatorDebugOn = new ToolStripSeparator();
			tsLblDebugOn = new ToolStripLabel();
			toolStripLabel1 = new ToolStripLabel();
			ssMainStatus.SuspendLayout();
			ssMainStatus1.SuspendLayout();
			msMainMenu.SuspendLayout();
			toolStripContainer1.BottomToolStripPanel.SuspendLayout();
			toolStripContainer1.TopToolStripPanel.SuspendLayout();
			toolStripContainer1.SuspendLayout();
			tsActionToolbar.SuspendLayout();
			SuspendLayout();
			ssMainStatus.Dock = DockStyle.None;
			ssMainStatus.Items.AddRange(new ToolStripItem[]
			{
				tsLblVersion, tsVersion, tsLblSeparator1, tsLblFwVersion, tsFwVersion, tsBtnFwUpdate, tsLblSeparator2, tsLblChipVersion, tsChipVersion, tsLblSeparator3,
				tsLblConfigFileName, tsLblSeparator4, tsLblConnectionStatus, ledStatus
			});
			ssMainStatus.Location = new Point(0, 22);
			ssMainStatus.Name = "ssMainStatus";
			ssMainStatus.ShowItemToolTips = true;
			ssMainStatus.Size = new Size(1008, 22);
			ssMainStatus.SizingGrip = false;
			ssMainStatus.TabIndex = 3;
			ssMainStatus.Text = "Main status";
			tsLblVersion.Margin = new Padding(3, 3, 0, 3);
			tsLblVersion.Name = "tsLblVersion";
			tsLblVersion.Size = new Size(49, 16);
			tsLblVersion.Text = "Version:";
			tsVersion.AutoSize = false;
			tsVersion.Margin = new Padding(0, 3, 3, 3);
			tsVersion.Name = "tsVersion";
			tsVersion.Size = new Size(48, 16);
			tsVersion.Text = "-";
			tsLblSeparator1.Margin = new Padding(3);
			tsLblSeparator1.Name = "tsLblSeparator1";
			tsLblSeparator1.Size = new Size(10, 16);
			tsLblSeparator1.Text = "|";
			tsLblFwVersion.Margin = new Padding(3, 3, 0, 3);
			tsLblFwVersion.Name = "tsLblFwVersion";
			tsLblFwVersion.Size = new Size(101, 16);
			tsLblFwVersion.Text = "Firmware Version:";
			tsFwVersion.AutoSize = false;
			tsFwVersion.Margin = new Padding(0, 3, 3, 3);
			tsFwVersion.Name = "tsFwVersion";
			tsFwVersion.Size = new Size(48, 16);
			tsFwVersion.Text = "-";
			tsBtnFwUpdate.Name = "tsBtnFwUpdate";
			tsBtnFwUpdate.Size = new Size(23, 20);
			tsLblSeparator2.Margin = new Padding(3);
			tsLblSeparator2.Name = "tsLblSeparator2";
			tsLblSeparator2.Size = new Size(10, 16);
			tsLblSeparator2.Text = "|";
			tsLblChipVersion.Margin = new Padding(3, 3, 0, 3);
			tsLblChipVersion.Name = "tsLblChipVersion";
			tsLblChipVersion.Size = new Size(76, 16);
			tsLblChipVersion.Text = "Chip version:";
			tsChipVersion.AutoSize = false;
			tsChipVersion.Margin = new Padding(0, 3, 3, 3);
			tsChipVersion.Name = "tsChipVersion";
			tsChipVersion.Size = new Size(48, 16);
			tsChipVersion.Text = "-";
			tsLblSeparator3.Margin = new Padding(3);
			tsLblSeparator3.Name = "tsLblSeparator3";
			tsLblSeparator3.Size = new Size(10, 16);
			tsLblSeparator3.Text = "|";
			tsLblConfigFileName.AutoToolTip = true;
			tsLblConfigFileName.DisplayStyle = ToolStripItemDisplayStyle.Text;
			tsLblConfigFileName.Margin = new Padding(3);
			tsLblConfigFileName.Name = "tsLblConfigFileName";
			tsLblConfigFileName.Size = new Size(379, 16);
			tsLblConfigFileName.Spring = true;
			tsLblConfigFileName.Text = "Config File:";
			tsLblConfigFileName.TextAlign = ContentAlignment.MiddleLeft;
			tsLblConfigFileName.ToolTipText = "Shows the active Config file when File-> Open/Save is used";
			tsLblSeparator4.Margin = new Padding(3);
			tsLblSeparator4.Name = "tsLblSeparator4";
			tsLblSeparator4.Size = new Size(10, 16);
			tsLblSeparator4.Text = "|";
			tsLblConnectionStatus.Margin = new Padding(3);
			tsLblConnectionStatus.Name = "tsLblConnectionStatus";
			tsLblConnectionStatus.Size = new Size(106, 16);
			tsLblConnectionStatus.Text = "Connection status:";
			ledStatus.BackColor = Color.Transparent;
			ledStatus.Checked = false;
			ledStatus.LedAlign = ContentAlignment.MiddleCenter;
			ledStatus.LedColor = Color.Green;
			ledStatus.LedSize = new Size(11, 11);
			ledStatus.Margin = new Padding(3);
			ledStatus.Name = "ledStatus";
			ledStatus.Size = new Size(15, 16);
			ledStatus.Text = "Connection status";
			tsLblStatus.Margin = new Padding(3);
			tsLblStatus.Name = "tsLblStatus";
			tsLblStatus.Size = new Size(12, 16);
			tsLblStatus.Text = "-";
			tsLblStatus.TextAlign = ContentAlignment.MiddleLeft;
			tsLblStatus.ToolTipText = "Shows SKA messages.";
			ssMainStatus1.Dock = DockStyle.None;
			ssMainStatus1.Items.AddRange(new ToolStripItem[] { tsLblStatus });
			ssMainStatus1.Location = new Point(0, 0);
			ssMainStatus1.Name = "ssMainStatus1";
			ssMainStatus1.ShowItemToolTips = true;
			ssMainStatus1.Size = new Size(1008, 22);
			ssMainStatus1.SizingGrip = false;
			ssMainStatus1.TabIndex = 3;
			ssMainStatus1.Text = "Main status 1";
			msMainMenu.ClickThrough = true;
			msMainMenu.Dock = DockStyle.None;
			msMainMenu.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, actionToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem });
			msMainMenu.Location = new Point(0, 0);
			msMainMenu.Name = "msMainMenu";
			msMainMenu.Size = new Size(1008, 24);
			msMainMenu.SuppressHighlighting = false;
			msMainMenu.TabIndex = 0;
			msMainMenu.Text = "File";
			fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { connectToolStripMenuItem, mFileSeparator1, loadToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, mFileSeparator2, exitToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new Size(37, 20);
			fileToolStripMenuItem.Text = "&File";
			connectToolStripMenuItem.Image = (Image)resources.GetObject("connectToolStripMenuItem.Image");
			connectToolStripMenuItem.Name = "connectToolStripMenuItem";
			connectToolStripMenuItem.Size = new Size(162, 22);
			connectToolStripMenuItem.Text = "&Connect";
			connectToolStripMenuItem.Visible = false;
			connectToolStripMenuItem.Click += tsBtnOpenDevice_Click;
			mFileSeparator1.Name = "mFileSeparator1";
			mFileSeparator1.Size = new Size(159, 6);
			mFileSeparator1.Visible = false;
			loadToolStripMenuItem.Enabled = false;
			loadToolStripMenuItem.Image = (Image)resources.GetObject("loadToolStripMenuItem.Image");
			loadToolStripMenuItem.Name = "loadToolStripMenuItem";
			loadToolStripMenuItem.Size = new Size(162, 22);
			loadToolStripMenuItem.Text = "&Open Config...";
			loadToolStripMenuItem.Click += loadToolStripMenuItem_Click;
			saveToolStripMenuItem.Enabled = false;
			saveToolStripMenuItem.Image = (Image)resources.GetObject("saveToolStripMenuItem.Image");
			saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			saveToolStripMenuItem.Size = new Size(162, 22);
			saveToolStripMenuItem.Text = "&Save Config";
			saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
			saveAsToolStripMenuItem.Enabled = false;
			saveAsToolStripMenuItem.Image = (Image)resources.GetObject("saveAsToolStripMenuItem.Image");
			saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			saveAsToolStripMenuItem.Size = new Size(162, 22);
			saveAsToolStripMenuItem.Text = "Save Config &As...";
			saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
			mFileSeparator2.Name = "mFileSeparator2";
			mFileSeparator2.Size = new Size(159, 6);
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.Size = new Size(162, 22);
			exitToolStripMenuItem.Text = "&Exit";
			exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
			actionToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { modemToolStripMenuItem, resetToolStripMenuItem, refreshToolStripMenuItem, showRegistersToolStripMenuItem, monitorToolStripMenuItem, startuptimeToolStripMenuItem });
			actionToolStripMenuItem.Name = "actionToolStripMenuItem";
			actionToolStripMenuItem.Size = new Size(54, 20);
			actionToolStripMenuItem.Text = "&Action";
			modemToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { modemFskToolStripMenuItem, modemLoRaToolStripMenuItem });
			modemToolStripMenuItem.Enabled = false;
			modemToolStripMenuItem.Name = "modemToolStripMenuItem";
			modemToolStripMenuItem.Size = new Size(150, 22);
			modemToolStripMenuItem.Text = "M&odem";
			modemFskToolStripMenuItem.Name = "modemFskToolStripMenuItem";
			modemFskToolStripMenuItem.Size = new Size(100, 22);
			modemFskToolStripMenuItem.Text = "FSK";
			modemFskToolStripMenuItem.Click += modemToolStripMenuItem_Click;
			modemLoRaToolStripMenuItem.Checked = true;
			modemLoRaToolStripMenuItem.CheckState = CheckState.Checked;
			modemLoRaToolStripMenuItem.Name = "modemLoRaToolStripMenuItem";
			modemLoRaToolStripMenuItem.Size = new Size(100, 22);
			modemLoRaToolStripMenuItem.Text = "&LoRa";
			modemLoRaToolStripMenuItem.Click += modemToolStripMenuItem_Click;
			resetToolStripMenuItem.Enabled = false;
			resetToolStripMenuItem.Name = "resetToolStripMenuItem";
			resetToolStripMenuItem.Size = new Size(150, 22);
			resetToolStripMenuItem.Text = "R&eset";
			resetToolStripMenuItem.Click += resetToolStripMenuItem_Click;
			refreshToolStripMenuItem.Enabled = false;
			refreshToolStripMenuItem.Image = (Image)resources.GetObject("refreshToolStripMenuItem.Image");
			refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
			refreshToolStripMenuItem.Size = new Size(150, 22);
			refreshToolStripMenuItem.Text = "&Refresh";
			refreshToolStripMenuItem.Click += refreshToolStripMenuItem_Click;
			showRegistersToolStripMenuItem.Enabled = false;
			showRegistersToolStripMenuItem.Name = "showRegistersToolStripMenuItem";
			showRegistersToolStripMenuItem.Size = new Size(150, 22);
			showRegistersToolStripMenuItem.Text = "&Show registers";
			showRegistersToolStripMenuItem.Click += showRegistersToolStripMenuItem_Click;
			monitorToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { monitorOffToolStripMenuItem, monitorOnToolStripMenuItem });
			monitorToolStripMenuItem.Enabled = false;
			monitorToolStripMenuItem.Name = "monitorToolStripMenuItem";
			monitorToolStripMenuItem.Size = new Size(150, 22);
			monitorToolStripMenuItem.Text = "&Monitor";
			monitorOffToolStripMenuItem.Name = "monitorOffToolStripMenuItem";
			monitorOffToolStripMenuItem.Size = new Size(95, 22);
			monitorOffToolStripMenuItem.Text = "OFF";
			monitorOffToolStripMenuItem.Click += monitorToolStripMenuItem_Click;
			monitorOnToolStripMenuItem.Checked = true;
			monitorOnToolStripMenuItem.CheckState = CheckState.Checked;
			monitorOnToolStripMenuItem.Name = "monitorOnToolStripMenuItem";
			monitorOnToolStripMenuItem.Size = new Size(95, 22);
			monitorOnToolStripMenuItem.Text = "&ON";
			monitorOnToolStripMenuItem.Click += monitorToolStripMenuItem_Click;
			startuptimeToolStripMenuItem.Enabled = false;
			startuptimeToolStripMenuItem.Image = (Image)resources.GetObject("startuptimeToolStripMenuItem.Image");
			startuptimeToolStripMenuItem.Name = "startuptimeToolStripMenuItem";
			startuptimeToolStripMenuItem.Size = new Size(150, 22);
			startuptimeToolStripMenuItem.Text = "Startup &time...";
			startuptimeToolStripMenuItem.Click += startuptimeToolStripMenuItem_Click;
			toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { rssiAnalyserToolStripMenuItem, spectrumAnalyserToolStripMenuItem });
			toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			toolsToolStripMenuItem.Size = new Size(48, 20);
			toolsToolStripMenuItem.Text = "Tools";
			rssiAnalyserToolStripMenuItem.Enabled = false;
			rssiAnalyserToolStripMenuItem.Name = "rssiAnalyserToolStripMenuItem";
			rssiAnalyserToolStripMenuItem.Size = new Size(171, 22);
			rssiAnalyserToolStripMenuItem.Text = "RSSI analyser";
			rssiAnalyserToolStripMenuItem.Click += rssiAnalyserToolStripMenuItem_Click;
			spectrumAnalyserToolStripMenuItem.Enabled = false;
			spectrumAnalyserToolStripMenuItem.Name = "spectrumAnalyserToolStripMenuItem";
			spectrumAnalyserToolStripMenuItem.Size = new Size(171, 22);
			spectrumAnalyserToolStripMenuItem.Text = "Spectrum analyser";
			spectrumAnalyserToolStripMenuItem.Click += spectrumAnalyserToolStripMenuItem_Click;
			helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { showHelpToolStripMenuItem, mHelpSeparator1, usersGuideToolStripMenuItem, mHelpSeparator2, aboutToolStripMenuItem });
			helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			helpToolStripMenuItem.Size = new Size(44, 20);
			helpToolStripMenuItem.Text = "&Help";
			showHelpToolStripMenuItem.Enabled = false;
			showHelpToolStripMenuItem.Image = (Image)resources.GetObject("showHelpToolStripMenuItem.Image");
			showHelpToolStripMenuItem.Name = "showHelpToolStripMenuItem";
			showHelpToolStripMenuItem.Size = new Size(231, 22);
			showHelpToolStripMenuItem.Text = "Help";
			showHelpToolStripMenuItem.Click += showHelpToolStripMenuItem_Click;
			mHelpSeparator1.Name = "mHelpSeparator1";
			mHelpSeparator1.Size = new Size(228, 6);
			usersGuideToolStripMenuItem.Name = "usersGuideToolStripMenuItem";
			usersGuideToolStripMenuItem.Size = new Size(231, 22);
			usersGuideToolStripMenuItem.Text = "&User's Guide...";
			usersGuideToolStripMenuItem.Click += usersGuideToolStripMenuItem_Click;
			mHelpSeparator2.Name = "mHelpSeparator2";
			mHelpSeparator2.Size = new Size(228, 6);
			aboutToolStripMenuItem.Image = (Image)resources.GetObject("aboutToolStripMenuItem.Image");
			aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			aboutToolStripMenuItem.Size = new Size(231, 22);
			aboutToolStripMenuItem.Text = "&About SX1276 Evaluation Kit...";
			aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
			tsBtnRefresh.DisplayStyle = ToolStripItemDisplayStyle.Image;
			tsBtnRefresh.Image = (Image)resources.GetObject("tsBtnRefresh.Image");
			tsBtnRefresh.ImageTransparentColor = Color.Magenta;
			tsBtnRefresh.Name = "tsBtnRefresh";
			tsBtnRefresh.Size = new Size(23, 22);
			tsBtnRefresh.Text = "Refresh";
			tsBtnRefresh.Click += refreshToolStripMenuItem_Click;
			toolStripSeparator3.Name = "toolStripSeparator3";
			toolStripSeparator3.Size = new Size(6, 25);
			ofConfigFileOpenDlg.DefaultExt = "*.cfg";
			ofConfigFileOpenDlg.Filter = "Config Files(*.cfg)|*.cfg|AllFiles(*.*)|*.*";
			sfConfigFileSaveDlg.DefaultExt = "*.cfg";
			sfConfigFileSaveDlg.Filter = "Config Files(*.cfg)|*.cfg|AllFiles(*.*)|*.*";
			tipMainForm.ShowAlways = true;
			toolStripContainer1.BottomToolStripPanel.Controls.Add(ssMainStatus1);
			toolStripContainer1.BottomToolStripPanel.Controls.Add(ssMainStatus);
			toolStripContainer1.BottomToolStripPanel.MaximumSize = new Size(0, 44);
			toolStripContainer1.ContentPanel.AutoScroll = true;
			toolStripContainer1.ContentPanel.Size = new Size(1008, 525);
			toolStripContainer1.Dock = DockStyle.Fill;
			toolStripContainer1.LeftToolStripPanelVisible = false;
			toolStripContainer1.Location = new Point(0, 0);
			toolStripContainer1.Name = "toolStripContainer1";
			toolStripContainer1.RightToolStripPanelVisible = false;
			toolStripContainer1.Size = new Size(1008, 618);
			toolStripContainer1.TabIndex = 0;
			toolStripContainer1.Text = "toolStripContainer1";
			toolStripContainer1.TopToolStripPanel.Controls.Add(msMainMenu);
			toolStripContainer1.TopToolStripPanel.Controls.Add(tsActionToolbar);
			toolStripContainer1.TopToolStripPanel.MaximumSize = new Size(0, 50);
			tsActionToolbar.ClickThrough = true;
			tsActionToolbar.Dock = DockStyle.None;
			tsActionToolbar.GripStyle = ToolStripGripStyle.Hidden;
			tsActionToolbar.Items.AddRange(new ToolStripItem[]
			{
				tsBtnOpenFile, tsBtnSaveFile, tbFileSeparator1, tsBtnOpenDevice, toolStripSeparator7, tsLblModem, tsBtnModemLoRa, tsBtnModemFsk, toolStripSeparator5, tsBtnReset,
				toolStripSeparator1, tsBtnRefresh, tsBtnStartupTime, toolStripSeparator2, tsBtnShowRegisters, toolStripSeparator4, tsLblMonitor, tsBtnMonitorOn, tsBtnMonitorOff, toolStripSeparator6,
				tsBtnShowHelp, tsSeparatorPerModeOn, tsLblPerModeOn, tsSeparatorDebugOn, tsLblDebugOn
			});
			tsActionToolbar.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			tsActionToolbar.Location = new Point(3, 24);
			tsActionToolbar.Name = "tsActionToolbar";
			tsActionToolbar.Size = new Size(488, 25);
			tsActionToolbar.SuppressHighlighting = false;
			tsActionToolbar.TabIndex = 2;
			tsActionToolbar.Text = "Action";
			tsBtnOpenFile.DisplayStyle = ToolStripItemDisplayStyle.Image;
			tsBtnOpenFile.Enabled = false;
			tsBtnOpenFile.Image = (Image)resources.GetObject("tsBtnOpenFile.Image");
			tsBtnOpenFile.ImageTransparentColor = Color.Magenta;
			tsBtnOpenFile.Name = "tsBtnOpenFile";
			tsBtnOpenFile.Size = new Size(23, 22);
			tsBtnOpenFile.Text = "Open Config file";
			tsBtnOpenFile.Click += loadToolStripMenuItem_Click;
			tsBtnSaveFile.DisplayStyle = ToolStripItemDisplayStyle.Image;
			tsBtnSaveFile.Enabled = false;
			tsBtnSaveFile.Image = (Image)resources.GetObject("tsBtnSaveFile.Image");
			tsBtnSaveFile.ImageTransparentColor = Color.Magenta;
			tsBtnSaveFile.Name = "tsBtnSaveFile";
			tsBtnSaveFile.Size = new Size(23, 22);
			tsBtnSaveFile.Text = "Save Config file";
			tsBtnSaveFile.Click += saveToolStripMenuItem_Click;
			tbFileSeparator1.Name = "tbFileSeparator1";
			tbFileSeparator1.Size = new Size(6, 25);
			tsBtnOpenDevice.DisplayStyle = ToolStripItemDisplayStyle.Image;
			tsBtnOpenDevice.Image = (Image)resources.GetObject("tsBtnOpenDevice.Image");
			tsBtnOpenDevice.ImageTransparentColor = Color.Magenta;
			tsBtnOpenDevice.Name = "tsBtnOpenDevice";
			tsBtnOpenDevice.Size = new Size(23, 22);
			tsBtnOpenDevice.Text = "Connect";
			tsBtnOpenDevice.Click += tsBtnOpenDevice_Click;
			toolStripSeparator7.Name = "toolStripSeparator7";
			toolStripSeparator7.Size = new Size(6, 25);
			tsLblModem.Name = "tsLblModem";
			tsLblModem.Size = new Size(52, 22);
			tsLblModem.Text = "Modem:";
			tsBtnModemLoRa.Checked = true;
			tsBtnModemLoRa.CheckState = CheckState.Checked;
			tsBtnModemLoRa.DisplayStyle = ToolStripItemDisplayStyle.Text;
			tsBtnModemLoRa.Image = (Image)resources.GetObject("tsBtnModemLoRa.Image");
			tsBtnModemLoRa.ImageTransparentColor = Color.Magenta;
			tsBtnModemLoRa.Name = "tsBtnModemLoRa";
			tsBtnModemLoRa.Size = new Size(37, 22);
			tsBtnModemLoRa.Text = "LoRa";
			tsBtnModemLoRa.ToolTipText = "Enables the SX1276 LoRa modem";
			tsBtnModemLoRa.Click += modemToolStripMenuItem_Click;
			tsBtnModemFsk.DisplayStyle = ToolStripItemDisplayStyle.Text;
			tsBtnModemFsk.Image = (Image)resources.GetObject("tsBtnModemFsk.Image");
			tsBtnModemFsk.ImageTransparentColor = Color.Magenta;
			tsBtnModemFsk.Name = "tsBtnModemFsk";
			tsBtnModemFsk.Size = new Size(30, 22);
			tsBtnModemFsk.Text = "FSK";
			tsBtnModemFsk.ToolTipText = "Enables the SX1276 FSK modem";
			tsBtnModemFsk.Click += modemToolStripMenuItem_Click;
			toolStripSeparator5.Name = "toolStripSeparator5";
			toolStripSeparator5.Size = new Size(6, 25);
			tsBtnReset.DisplayStyle = ToolStripItemDisplayStyle.Text;
			tsBtnReset.Image = (Image)resources.GetObject("tsBtnReset.Image");
			tsBtnReset.ImageTransparentColor = Color.Magenta;
			tsBtnReset.Name = "tsBtnReset";
			tsBtnReset.Size = new Size(39, 22);
			tsBtnReset.Text = "Reset";
			tsBtnReset.Click += resetToolStripMenuItem_Click;
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new Size(6, 25);
			tsBtnStartupTime.DisplayStyle = ToolStripItemDisplayStyle.Image;
			tsBtnStartupTime.Image = (Image)resources.GetObject("tsBtnStartupTime.Image");
			tsBtnStartupTime.ImageTransparentColor = Color.Magenta;
			tsBtnStartupTime.Name = "tsBtnStartupTime";
			tsBtnStartupTime.Size = new Size(23, 22);
			tsBtnStartupTime.Text = "Startup time";
			tsBtnStartupTime.Click += startuptimeToolStripMenuItem_Click;
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new Size(6, 25);
			tsBtnShowRegisters.DisplayStyle = ToolStripItemDisplayStyle.Text;
			tsBtnShowRegisters.Font = new Font("Tahoma", 8.25f, FontStyle.Bold);
			tsBtnShowRegisters.Image = (Image)resources.GetObject("tsBtnShowRegisters.Image");
			tsBtnShowRegisters.ImageTransparentColor = Color.Magenta;
			tsBtnShowRegisters.Name = "tsBtnShowRegisters";
			tsBtnShowRegisters.Size = new Size(33, 22);
			tsBtnShowRegisters.Text = "Reg";
			tsBtnShowRegisters.ToolTipText = "Displays SX1276 raw registers window";
			tsBtnShowRegisters.Click += showRegistersToolStripMenuItem_Click;
			toolStripSeparator4.Name = "toolStripSeparator4";
			toolStripSeparator4.Size = new Size(6, 25);
			tsLblMonitor.Name = "tsLblMonitor";
			tsLblMonitor.Size = new Size(53, 22);
			tsLblMonitor.Text = "Monitor:";
			tsBtnMonitorOn.Checked = true;
			tsBtnMonitorOn.CheckState = CheckState.Checked;
			tsBtnMonitorOn.DisplayStyle = ToolStripItemDisplayStyle.Text;
			tsBtnMonitorOn.Image = (Image)resources.GetObject("tsBtnMonitorOn.Image");
			tsBtnMonitorOn.ImageTransparentColor = Color.Magenta;
			tsBtnMonitorOn.Name = "tsBtnMonitorOn";
			tsBtnMonitorOn.Size = new Size(29, 22);
			tsBtnMonitorOn.Text = "ON";
			tsBtnMonitorOn.ToolTipText = "Enables the SX1276 monitor mode";
			tsBtnMonitorOn.Click += monitorToolStripMenuItem_Click;
			tsBtnMonitorOff.DisplayStyle = ToolStripItemDisplayStyle.Text;
			tsBtnMonitorOff.Image = (Image)resources.GetObject("tsBtnMonitorOff.Image");
			tsBtnMonitorOff.ImageTransparentColor = Color.Magenta;
			tsBtnMonitorOff.Name = "tsBtnMonitorOff";
			tsBtnMonitorOff.Size = new Size(32, 22);
			tsBtnMonitorOff.Text = "OFF";
			tsBtnMonitorOff.ToolTipText = "Disables the SX1276 monitor mode";
			tsBtnMonitorOff.Click += monitorToolStripMenuItem_Click;
			toolStripSeparator6.Name = "toolStripSeparator6";
			toolStripSeparator6.Size = new Size(6, 25);
			tsBtnShowHelp.DisplayStyle = ToolStripItemDisplayStyle.Image;
			tsBtnShowHelp.Enabled = false;
			tsBtnShowHelp.Image = (Image)resources.GetObject("tsBtnShowHelp.Image");
			tsBtnShowHelp.ImageTransparentColor = Color.Magenta;
			tsBtnShowHelp.Name = "tsBtnShowHelp";
			tsBtnShowHelp.Size = new Size(23, 22);
			tsBtnShowHelp.Text = "Help";
			tsBtnShowHelp.Click += showHelpToolStripMenuItem_Click;
			tsSeparatorPerModeOn.Name = "tsSeparatorPerModeOn";
			tsSeparatorPerModeOn.Size = new Size(6, 25);
			tsSeparatorPerModeOn.Visible = false;
			tsLblPerModeOn.ForeColor = Color.Red;
			tsLblPerModeOn.Name = "tsLblPerModeOn";
			tsLblPerModeOn.Size = new Size(82, 22);
			tsLblPerModeOn.Text = "PER mode ON";
			tsLblPerModeOn.Visible = false;
			tsSeparatorDebugOn.Name = "tsSeparatorDebugOn";
			tsSeparatorDebugOn.Size = new Size(6, 25);
			tsSeparatorDebugOn.Visible = false;
			tsLblDebugOn.ForeColor = Color.Red;
			tsLblDebugOn.Name = "tsLblDebugOn";
			tsLblDebugOn.Size = new Size(85, 22);
			tsLblDebugOn.Text = "DBG mode ON";
			tsLblDebugOn.Visible = false;
			toolStripLabel1.Name = "toolStripLabel1";
			toolStripLabel1.Size = new Size(62, 22);
			toolStripLabel1.Text = "Product ID:";
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1008, 618);
			Controls.Add(toolStripContainer1);
			DoubleBuffered = true;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Icon = (Icon)resources.GetObject("$this.Icon");
			KeyPreview = true;
			MainMenuStrip = msMainMenu;
			MaximizeBox = false;
			Name = "MainForm";
			Text = "SX1276 Evaluation Kit";
			FormClosing += Mainform_FormClosing;
			FormClosed += MainForm_FormClosed;
			Load += MainForm_Load;
			KeyDown += Mainform_KeyDown;
			ssMainStatus.ResumeLayout(false);
			ssMainStatus.PerformLayout();
			ssMainStatus1.ResumeLayout(false);
			ssMainStatus1.PerformLayout();
			msMainMenu.ResumeLayout(false);
			msMainMenu.PerformLayout();
			toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
			toolStripContainer1.BottomToolStripPanel.PerformLayout();
			toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			toolStripContainer1.TopToolStripPanel.PerformLayout();
			toolStripContainer1.ResumeLayout(false);
			toolStripContainer1.PerformLayout();
			tsActionToolbar.ResumeLayout(false);
			tsActionToolbar.PerformLayout();
			ResumeLayout(false);
		}

		public MainForm(bool testMode)
		{
			AppTestArg = testMode;
			InitializeComponent();
			deviceList[0].SetNotificationWindowHandle(Handle, isWpfApplication: false);
			deviceList[0].Test = AppTestArg;
			deviceList[0].Error += device_Error;
			deviceList[0].Connected += device_Connected;
			deviceList[0].Disconected += device_Disconected;
			deviceList[0].PropertyChanged += device_PropertyChanged;
			deviceList[0].PacketHandlerStarted += device_PacketHandlerStarted;
			deviceList[0].PacketHandlerStoped += device_PacketHandlerStoped;
			deviceList[1].SetNotificationWindowHandle(Handle, isWpfApplication: false);
			deviceList[1].Test = AppTestArg;
			deviceList[1].Error += device_Error;
			deviceList[1].Connected += device_Connected;
			deviceList[1].Disconected += device_Disconected;
			deviceList[1].PropertyChanged += device_PropertyChanged;
			deviceList[1].PacketHandlerStarted += device_PacketHandlerStarted;
			deviceList[1].PacketHandlerStoped += device_PacketHandlerStoped;
			toolStripContainer1.BottomToolStripPanel.SuspendLayout();
			toolStripContainer1.TopToolStripPanel.SuspendLayout();
			toolStripContainer1.SuspendLayout();
			deviceViewList[0].Name = "sx1276ViewControl";
			((SemtechLib.Devices.SX1276.UI.Controls.DeviceViewControl)deviceViewList[0]).Location = new Point(0, 0);
			((SemtechLib.Devices.SX1276.UI.Controls.DeviceViewControl)deviceViewList[0]).Size = new Size(1008, 525);
			deviceViewList[0].Device = deviceList[0];
			deviceViewList[0].Dock = DockStyle.Fill;
			deviceViewList[0].Enabled = false;
			deviceViewList[0].Visible = true;
			deviceViewList[0].TabIndex = 0;
			deviceViewList[0].Error += deviceViewControl_Error;
			deviceViewList[0].DocumentationChanged += deviceViewControl_DocumentationChanged;
			deviceViewList[1].Name = "sx1276LoRaViewControl";
			((SemtechLib.Devices.SX1276LR.UI.Controls.DeviceViewControl)deviceViewList[1]).Location = new Point(0, 0);
			((SemtechLib.Devices.SX1276LR.UI.Controls.DeviceViewControl)deviceViewList[1]).Size = new Size(1008, 525);
			deviceViewList[1].Device = deviceList[1];
			deviceViewList[1].Dock = DockStyle.Fill;
			deviceViewList[1].Enabled = false;
			deviceViewList[1].Visible = false;
			deviceViewList[1].TabIndex = 0;
			deviceViewList[1].Error += deviceViewControl_Error;
			deviceViewList[1].DocumentationChanged += deviceViewControl_DocumentationChanged;
			toolStripContainer1.ContentPanel.Controls.Add((UserControl)deviceViewList[0]);
			toolStripContainer1.ContentPanel.Controls.Add((UserControl)deviceViewList[1]);
			deviceViewControl = deviceViewList[0];
			toolStripContainer1.BottomToolStripPanel.ResumeLayout(performLayout: false);
			toolStripContainer1.BottomToolStripPanel.PerformLayout();
			toolStripContainer1.TopToolStripPanel.ResumeLayout(performLayout: false);
			toolStripContainer1.TopToolStripPanel.PerformLayout();
			toolStripContainer1.ResumeLayout(performLayout: false);
			toolStripContainer1.PerformLayout();
			try
			{
				appSettings = new ApplicationSettings();
			}
			catch (Exception ex)
			{
				OnError(1, "ERROR: " + ex.Message);
			}
			if (!AppTestArg)
			{
				Text = AssemblyTitle ?? "";
			}
			else
			{
				Text = AssemblyTitle + " - ..::: TEST :::..";
			}
		}

		private void DisableControls()
		{
			if (frmRegisters != null)
			{
				frmRegisters.RegistersFormEnabled = false;
			}
			tsBtnOpenFile.Enabled = false;
			tsBtnSaveFile.Enabled = false;
			loadToolStripMenuItem.Enabled = false;
			saveToolStripMenuItem.Enabled = false;
			saveAsToolStripMenuItem.Enabled = false;
			tsBtnRefresh.Enabled = false;
			tsLblMonitor.Enabled = false;
			tsBtnMonitorOff.Enabled = false;
			tsBtnMonitorOn.Enabled = false;
			refreshToolStripMenuItem.Enabled = false;
			monitorToolStripMenuItem.Enabled = false;
		}

		private void EnableControls()
		{
			if (frmRegisters != null)
			{
				frmRegisters.RegistersFormEnabled = true;
			}
			tsBtnOpenFile.Enabled = true;
			tsBtnSaveFile.Enabled = true;
			loadToolStripMenuItem.Enabled = true;
			saveToolStripMenuItem.Enabled = true;
			saveAsToolStripMenuItem.Enabled = true;
			tsBtnRefresh.Enabled = true;
			tsLblMonitor.Enabled = true;
			tsBtnMonitorOff.Enabled = true;
			tsBtnMonitorOn.Enabled = true;
			refreshToolStripMenuItem.Enabled = true;
			monitorToolStripMenuItem.Enabled = true;
		}

		private void OnError(byte status, string message)
		{
			if (status != 0)
			{
				tsLblStatus.Text = "ERROR: " + message;
			}
			else
			{
				tsLblStatus.Text = message;
			}
			Refresh();
		}

		private void OnConnected()
		{
			try
			{
				OnError(0, "-");
				ledStatus.Checked = device.IsOpen;
				tsBtnOpenDevice.Text = "Disconnect";
				tsBtnOpenDevice.Image = Resources.Connected;
				connectToolStripMenuItem.Text = "Disconnect";
				connectToolStripMenuItem.Image = Resources.Connected;
				device.Reset();
				tsBtnOpenFile.Enabled = true;
				tsBtnSaveFile.Enabled = true;
				loadToolStripMenuItem.Enabled = true;
				saveToolStripMenuItem.Enabled = true;
				saveAsToolStripMenuItem.Enabled = true;
				tsLblModem.Enabled = true;
				tsBtnModemLoRa.Enabled = true;
				tsBtnModemFsk.Enabled = true;
				tsBtnReset.Enabled = true;
				tsBtnRefresh.Enabled = true;
				tsBtnShowRegisters.Enabled = true;
				tsLblMonitor.Enabled = true;
				tsBtnMonitorOn.Enabled = true;
				tsBtnMonitorOff.Enabled = true;
				tsBtnStartupTime.Enabled = true;
				modemToolStripMenuItem.Enabled = true;
				resetToolStripMenuItem.Enabled = true;
				refreshToolStripMenuItem.Enabled = true;
				showRegistersToolStripMenuItem.Enabled = true;
				monitorToolStripMenuItem.Enabled = true;
				startuptimeToolStripMenuItem.Enabled = true;
				rssiAnalyserToolStripMenuItem.Enabled = true;
				spectrumAnalyserToolStripMenuItem.Enabled = true;
				deviceViewControl.Enabled = true;
				if (frmTest != null)
				{
					frmTest.Device = device;
				}
				if (frmRegisters != null)
				{
					frmRegisters.Device = device;
				}
				var text = "";
				text = ((device.FwVersion.Build == 0) ? device.FwVersion.ToString() : $"{device.FwVersion.Major.ToString()}.{device.FwVersion.Minor.ToString()}.B{device.FwVersion.Build.ToString()}");
				tsFwVersion.Text = text;
				tsChipVersion.Text = device.Version.ToString();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		private void OnDisconnected()
		{
			try
			{
				OnError(0, "-");
				ledStatus.Checked = device.IsOpen;
				tsBtnOpenDevice.Text = "Connect";
				tsBtnOpenDevice.Image = Resources.Disconnected;
				connectToolStripMenuItem.Text = "Connect";
				connectToolStripMenuItem.Image = Resources.Disconnected;
				tsBtnOpenFile.Enabled = false;
				tsBtnSaveFile.Enabled = false;
				loadToolStripMenuItem.Enabled = false;
				saveToolStripMenuItem.Enabled = false;
				saveAsToolStripMenuItem.Enabled = false;
				tsLblModem.Enabled = false;
				tsBtnModemLoRa.Enabled = false;
				tsBtnModemFsk.Enabled = false;
				tsBtnReset.Enabled = false;
				tsBtnRefresh.Enabled = false;
				tsBtnShowRegisters.Enabled = false;
				tsLblMonitor.Enabled = false;
				tsBtnMonitorOn.Enabled = false;
				tsBtnMonitorOff.Enabled = false;
				tsBtnStartupTime.Enabled = false;
				modemToolStripMenuItem.Enabled = false;
				resetToolStripMenuItem.Enabled = false;
				refreshToolStripMenuItem.Enabled = false;
				showRegistersToolStripMenuItem.Enabled = false;
				monitorToolStripMenuItem.Enabled = false;
				startuptimeToolStripMenuItem.Enabled = false;
				rssiAnalyserToolStripMenuItem.Enabled = false;
				spectrumAnalyserToolStripMenuItem.Enabled = false;
				showHelpToolStripMenuItem.Enabled = false;
				tsBtnShowHelp.Enabled = false;
				deviceViewControl.Enabled = false;
				if (frmTest != null)
				{
					frmTest.Close();
				}
				if (frmRegisters != null)
				{
					frmRegisters.Close();
				}
				if (frmRssiAnalyser != null)
				{
					frmRssiAnalyser.Close();
				}
				if (frmSpectrumAnalyser != null)
				{
					frmSpectrumAnalyser.Close();
				}
				if (frmStartupTime != null)
				{
					frmStartupTime.Close();
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (device != null)
			{
				device.ProcessWinMessage(m.Msg, m.WParam, m.LParam);
			}
			base.WndProc(ref m);
		}

		private bool IsFormLocatedInScreen(Screen[] screens)
		{
			var upperBound = screens.GetUpperBound(0);
			var result = false;
			for (var i = 0; i <= upperBound; i++)
			{
				if (Left < screens[i].WorkingArea.Left || Top < screens[i].WorkingArea.Top || Left > screens[i].WorkingArea.Right || Top > screens[i].WorkingArea.Bottom)
				{
					result = false;
					continue;
				}
				result = true;
				break;
			}
			return result;
		}

		private void InitializeDevice(bool isLoRaOn)
		{
			IsLoRaOn = isLoRaOn;
			if (device != null)
			{
				tsBtnOpenDevice_Click(tsBtnOpenDevice, EventArgs.Empty);
			}
			if (isLoRaOn)
			{
				startuptimeToolStripMenuItem.Visible = false;
				tsBtnStartupTime.Visible = false;
				toolsToolStripMenuItem.Visible = false;
				deviceViewList[1].Visible = true;
				deviceViewList[0].Visible = false;
				device = deviceList[1];
				deviceViewControl = deviceViewList[1];
			}
			else
			{
				startuptimeToolStripMenuItem.Visible = true;
				tsBtnStartupTime.Visible = true;
				toolsToolStripMenuItem.Visible = true;
				deviceViewList[0].Visible = true;
				deviceViewList[1].Visible = false;
				device = deviceList[0];
				deviceViewControl = deviceViewList[0];
			}
			tsBtnOpenDevice_Click(tsBtnOpenDevice, EventArgs.Empty);
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			try
			{
				tsVersion.Text = $"{Assembly.GetExecutingAssembly().GetName().Version.Major.ToString()}.{Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString()}.{Assembly.GetExecutingAssembly().GetName().Version.Build.ToString()}";
				var value = appSettings.GetValue("Top");
				if (value != null)
				{
					try
					{
						Top = int.Parse(value);
					}
					catch
					{
						MessageBox.Show(this, "Error getting Top value.");
					}
				}
				value = appSettings.GetValue("Left");
				if (value != null)
				{
					try
					{
						Left = int.Parse(value);
					}
					catch
					{
						MessageBox.Show(this, "Error getting Left value.");
					}
				}
				var allScreens = Screen.AllScreens;
				if (!IsFormLocatedInScreen(allScreens))
				{
					Top = allScreens[0].WorkingArea.Top;
					Left = allScreens[0].WorkingArea.Left;
				}
				value = appSettings.GetValue("FskConfigFilePath");
				if (value == null)
				{
					value = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					appSettings.SetValue("FskConfigFilePath", value);
				}
				fskConfigFilePath = value;
				value = appSettings.GetValue("FskConfigFileName");
				if (value == null)
				{
					value = "sx1276ska-Fsk.cfg";
					appSettings.SetValue("FskConfigFileName", value);
				}
				fskConfigFileName = value;
				value = appSettings.GetValue("LoRaConfigFilePath");
				if (value == null)
				{
					value = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					appSettings.SetValue("LoRaConfigFilePath", value);
				}
				loRaConfigFilePath = value;
				value = appSettings.GetValue("LoRaConfigFileName");
				if (value == null)
				{
					value = "sx1276ska-LoRa.cfg";
					appSettings.SetValue("LoRaConfigFileName", value);
				}
				loRaConfigFileName = value;
				value = appSettings.GetValue("IsLoRaOn");
				if (value != null)
				{
					try
					{
						IsLoRaOn = bool.Parse(value);
					}
					catch
					{
						MessageBox.Show(this, "Error getting IsLoRaOn value.");
					}
				}
				value = appSettings.GetValue("IsLoRaPacketUsePerOn");
				if (value != null)
				{
					try
					{
						IsLoRaPacketUsePerOn = bool.Parse(value);
					}
					catch
					{
						MessageBox.Show(this, "Error getting IsLoRaPacketUsePerOn value.");
					}
				}
				value = appSettings.GetValue("IsDebugOn");
				if (value != null)
				{
					try
					{
						IsDebugOn = bool.Parse(value);
					}
					catch
					{
						MessageBox.Show(this, "Error getting IsDebugOn value.");
					}
				}
				tsLblConfigFileName.Text = "Config File: -";
				isFskConfigFileOpen = false;
				isLoRaConfigFileOpen = false;
				if (IsLoRaOn)
				{
					modemFskToolStripMenuItem.Checked = false;
					tsBtnModemFsk.Checked = false;
					modemLoRaToolStripMenuItem.Checked = true;
					tsBtnModemLoRa.Checked = true;
					((SemtechLib.Devices.SX1276LR.UI.Controls.DeviceViewControl)deviceViewList[1]).AppSettings = appSettings;
				}
				else
				{
					modemFskToolStripMenuItem.Checked = true;
					tsBtnModemFsk.Checked = true;
					modemLoRaToolStripMenuItem.Checked = false;
					tsBtnModemLoRa.Checked = false;
				}
				InitializeDevice(IsLoRaOn);
				device.IsDebugOn = IsDebugOn;
				tsLblDebugOn.Visible = IsDebugOn;
				tsSeparatorDebugOn.Visible = IsDebugOn;
				if (IsLoRaOn)
				{
					value = appSettings.GetValue("PacketLogPath");
					if (value == null)
					{
						value = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
						appSettings.SetValue("PacketLogPath", value);
					}
					((SX1276LR)device).PacketHandlerLog.Path = value;
					value = appSettings.GetValue("PacketLogFileName");
					if (value == null)
					{
						value = "sx1276-LoRa-pkt.log";
						appSettings.SetValue("PacketLogFileName", value);
					}
					((SX1276LR)device).PacketHandlerLog.FileName = value;
					value = appSettings.GetValue("PacketLogMaxSamples");
					if (value == null)
					{
						value = "0";
						appSettings.SetValue("PacketLogMaxSamples", value);
					}
					((SX1276LR)device).PacketHandlerLog.MaxSamples = ulong.Parse(value);
					value = appSettings.GetValue("PacketLogIsAppend");
					if (value == null)
					{
						value = true.ToString();
						appSettings.SetValue("PacketLogIsAppend", value);
					}
					((SX1276LR)device).PacketHandlerLog.IsAppend = bool.Parse(value);
					value = appSettings.GetValue("PacketLogEnabled");
					if (value == null)
					{
						value = false.ToString();
						appSettings.SetValue("PacketLogEnabled", value);
					}
					((SX1276LR)device).PacketHandlerLog.Enabled = bool.Parse(value);
					((SX1276LR)device).PacketUsePer = IsLoRaPacketUsePerOn;
					tsLblPerModeOn.Visible = IsLoRaPacketUsePerOn;
					tsSeparatorPerModeOn.Visible = IsLoRaPacketUsePerOn;
				}
				else
				{
					tsLblPerModeOn.Visible = false;
					tsSeparatorPerModeOn.Visible = false;
				}
			}
			catch (Exception ex)
			{
				OnError(1, "ERROR: " + ex.Message);
			}
		}

		private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (device != null)
			{
				device.Close();
			}
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				appSettings.SetValue("Top", Top.ToString());
				appSettings.SetValue("Left", Left.ToString());
				appSettings.SetValue("FskConfigFilePath", fskConfigFilePath);
				appSettings.SetValue("FskConfigFileName", fskConfigFileName);
				appSettings.SetValue("LoRaConfigFilePath", loRaConfigFilePath);
				appSettings.SetValue("LoRaConfigFileName", loRaConfigFileName);
				appSettings.SetValue("IsLoRaOn", IsLoRaOn.ToString());
				appSettings.SetValue("IsLoRaPacketUsePerOn", IsLoRaPacketUsePerOn.ToString());
				appSettings.SetValue("IsDebugOn", IsDebugOn.ToString());
			}
			catch (Exception ex)
			{
				OnError(1, "ERROR: " + ex.Message);
				Refresh();
			}
		}

		private void Mainform_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				e.Handled = true;
				SendKeys.Send("{TAB}");
			}
			else switch (e.KeyData)
			{
				case Keys.N | Keys.Control | Keys.Alt:
				{
					if (tsBtnOpenDevice.Text == "Connect")
					{
						deviceViewControl.Enabled = !deviceViewControl.Enabled;
						if (deviceViewControl.Enabled)
						{
							device.ReadRegisters();
							tsBtnOpenFile.Enabled = true;
							tsBtnSaveFile.Enabled = true;
							loadToolStripMenuItem.Enabled = true;
							saveToolStripMenuItem.Enabled = true;
							saveAsToolStripMenuItem.Enabled = true;
							tsLblModem.Enabled = true;
							tsBtnModemLoRa.Enabled = true;
							tsBtnModemFsk.Enabled = true;
							tsBtnReset.Enabled = true;
							tsBtnRefresh.Enabled = true;
							tsBtnShowRegisters.Enabled = true;
							tsLblMonitor.Enabled = false;
							tsBtnMonitorOff.Enabled = false;
							tsBtnMonitorOn.Enabled = false;
							tsBtnStartupTime.Enabled = true;
							modemToolStripMenuItem.Enabled = true;
							resetToolStripMenuItem.Enabled = true;
							refreshToolStripMenuItem.Enabled = true;
							showRegistersToolStripMenuItem.Enabled = true;
							monitorToolStripMenuItem.Enabled = false;
							startuptimeToolStripMenuItem.Enabled = true;
						}
						else
						{
							tsBtnOpenFile.Enabled = false;
							tsBtnSaveFile.Enabled = false;
							loadToolStripMenuItem.Enabled = false;
							saveToolStripMenuItem.Enabled = false;
							saveAsToolStripMenuItem.Enabled = false;
							tsLblModem.Enabled = false;
							tsBtnModemLoRa.Enabled = false;
							tsBtnModemFsk.Enabled = false;
							tsBtnReset.Enabled = false;
							tsBtnRefresh.Enabled = false;
							tsBtnShowRegisters.Enabled = false;
							tsLblMonitor.Enabled = false;
							tsBtnMonitorOff.Enabled = false;
							tsBtnMonitorOn.Enabled = false;
							tsBtnStartupTime.Enabled = false;
							modemToolStripMenuItem.Enabled = false;
							resetToolStripMenuItem.Enabled = false;
							refreshToolStripMenuItem.Enabled = false;
							showRegistersToolStripMenuItem.Enabled = false;
							monitorToolStripMenuItem.Enabled = false;
							startuptimeToolStripMenuItem.Enabled = false;
						}
					}

					break;
				}
				case Keys.T | Keys.Control | Keys.Alt:
				{
					if (frmTest == null)
					{
						frmTest = new TestForm();
						frmTest.FormClosed += frmTest_FormClosed;
						frmTest.Disposed += frmTest_Disposed;
						frmTest.Device = device;
						frmTest.TestEnabled = false;
					}
					if (!frmTest.TestEnabled)
					{
						frmTest.TestEnabled = true;
						var location = default(Point);
						location.X = Location.X + Width / 2 - frmTest.Width / 2;
						location.Y = Location.Y + Height / 2 - frmTest.Height / 2;
						frmTest.Location = location;
						frmTest.Show();
					}
					else
					{
						frmTest.TestEnabled = false;
						frmTest.Hide();
					}

					break;
				}
				case Keys.P | Keys.Control | Keys.Alt:
				{
					if (IsLoRaOn)
					{
						((SX1276LR)device).PacketUsePer = !((SX1276LR)device).PacketUsePer;
						tsLblPerModeOn.Visible = ((SX1276LR)device).PacketUsePer;
						tsSeparatorPerModeOn.Visible = ((SX1276LR)device).PacketUsePer;
						IsLoRaPacketUsePerOn = ((SX1276LR)device).PacketUsePer;
						appSettings.SetValue("IsLoRaPacketUsePerOn", IsLoRaPacketUsePerOn.ToString());
					}

					break;
				}
				case Keys.D | Keys.Control | Keys.Alt:
					device.IsDebugOn = !device.IsDebugOn;
					tsLblDebugOn.Visible = device.IsDebugOn;
					tsSeparatorDebugOn.Visible = device.IsDebugOn;
					IsDebugOn = device.IsDebugOn;
					appSettings.SetValue("IsDebugOn", IsDebugOn.ToString());
					break;
			}
		}

		private void frmRssiAnalyser_FormClosed(object sender, FormClosedEventArgs e)
		{
			rssiAnalyserToolStripMenuItem.Checked = false;
		}

		private void frmRssiAnalyser_Disposed(object sender, EventArgs e)
		{
			frmRssiAnalyser = null;
		}

		private void frmSpectrumAnalyser_FormClosed(object sender, FormClosedEventArgs e)
		{
			spectrumAnalyserToolStripMenuItem.Checked = false;
		}

		private void frmSpectrumAnalyser_Disposed(object sender, EventArgs e)
		{
			frmSpectrumAnalyser = null;
		}

		private void frmTest_FormClosed(object sender, FormClosedEventArgs e)
		{
		}

		private void frmTest_Disposed(object sender, EventArgs e)
		{
			frmTest = null;
		}

		private void frmHelp_FormClosed(object sender, FormClosedEventArgs e)
		{
			tsBtnShowHelp.Checked = false;
			showHelpToolStripMenuItem.Checked = false;
		}

		private void frmHelp_Disposed(object sender, EventArgs e)
		{
			frmHelp = null;
		}

		private void frmRegisters_FormClosed(object sender, FormClosedEventArgs e)
		{
			tsBtnShowRegisters.Checked = false;
			showRegistersToolStripMenuItem.Checked = false;
		}

		private void frmRegisters_Disposed(object sender, EventArgs e)
		{
			frmRegisters = null;
		}

		private void frmPacketLog_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (device is SX1276)
			{
				device.PropertyChanged -= device_PropertyChanged;
				((SX1276)device).Packet.LogEnabled = false;
				device.PropertyChanged += device_PropertyChanged;
			}
		}

		private void frmPacketLog_Disposed(object sender, EventArgs e)
		{
			frmPacketLog = null;
		}

		private void frmStartupTime_FormClosed(object sender, FormClosedEventArgs e)
		{
			tsBtnStartupTime.Checked = false;
			startuptimeToolStripMenuItem.Checked = false;
		}

		private void frmStartupTime_Disposed(object sender, EventArgs e)
		{
			frmStartupTime = null;
		}

		private void deviceViewControl_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			if (frmHelp != null)
			{
				frmHelp.UpdateDocument(e);
			}
		}

		private void deviceViewControl_Error(object sender, SemtechLib.General.Events.ErrorEventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new ErrorDelegate(OnError), e.Status, e.Message);
			}
			else
			{
				OnError(e.Status, e.Message);
			}
		}

		private void device_Error(object sender, SemtechLib.General.Events.ErrorEventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new ErrorDelegate(OnError), e.Status, e.Message);
			}
			else
			{
				OnError(e.Status, e.Message);
			}
		}

		private void device_Connected(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new ConnectedDelegate(OnConnected), null);
			}
			else
			{
				OnConnected();
			}
		}

		private void device_Disconected(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new DisconnectedDelegate(OnDisconnected), null);
			}
			else
			{
				OnDisconnected();
			}
		}

		private void OnDevicePorpertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
			case "Monitor":
				if (!device.Monitor)
				{
					monitorOffToolStripMenuItem.Checked = true;
					tsBtnMonitorOff.Checked = true;
					monitorOnToolStripMenuItem.Checked = false;
					tsBtnMonitorOn.Checked = false;
				}
				else
				{
					monitorOffToolStripMenuItem.Checked = false;
					tsBtnMonitorOff.Checked = false;
					monitorOnToolStripMenuItem.Checked = true;
					tsBtnMonitorOn.Checked = true;
				}
				break;
			case "SpectrumOn":
				if (device is SX1276 sx1276)
				{
					if (sx1276.SpectrumOn)
					{
						DisableControls();
					}
					else
					{
						EnableControls();
					}
				}
				break;
			case "LogEnabled":
				if (device is SX1276 sx1277)
				{
					if (!sx1277.Packet.LogEnabled)
					{
						if (frmPacketLog != null)
						{
							frmPacketLog.Close();
						}
						break;
					}
					if (frmPacketLog != null)
					{
						frmPacketLog.Close();
					}
					if (frmPacketLog == null)
					{
						frmPacketLog = new PacketLogForm();
						frmPacketLog.FormClosed += frmPacketLog_FormClosed;
						frmPacketLog.Disposed += frmPacketLog_Disposed;
						((PacketLogForm)frmPacketLog).Device = device;
						((PacketLogForm)frmPacketLog).AppSettings = appSettings;
					}
					frmPacketLog.Show();
				}
				else
				{
					_ = device is SX1276LR;
				}
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
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new DeviceDataChangedDelegate(OnDevicePorpertyChanged), sender, e);
			}
			else
			{
				OnDevicePorpertyChanged(sender, e);
			}
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

		private void tsBtnOpenDevice_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;
			tsBtnOpenDevice.Enabled = false;
			connectToolStripMenuItem.Enabled = false;
			Refresh();
			OnError(0, "-");
			Refresh();
			try
			{
				if (tsBtnOpenDevice.Text == "Connect")
				{
					if (!device.Open())
					{
						OnError(1, "Unable to open " + device.DeviceName + " device");
					}
				}
				else if (device != null)
				{
					device.Close();
				}
			}
			catch (Exception ex)
			{
				tsBtnOpenDevice.Text = "Connect";
				tsBtnOpenDevice.Image = Resources.Disconnected;
				connectToolStripMenuItem.Text = "Connect";
				connectToolStripMenuItem.Image = Resources.Disconnected;
				if (device != null)
				{
					device.Close();
				}
				device.ReadRegisters();
				OnError(1, "ERROR: " + ex.Message);
				Refresh();
			}
			finally
			{
				tsBtnOpenDevice.Enabled = true;
				connectToolStripMenuItem.Enabled = true;
				Cursor = Cursors.Default;
			}
		}

		private void loadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OnError(0, "-");
			Validate();
			if (!IsLoRaOn)
			{
				try
				{
					ofConfigFileOpenDlg.InitialDirectory = fskConfigFilePath;
					ofConfigFileOpenDlg.FileName = fskConfigFileName;
					if (ofConfigFileOpenDlg.ShowDialog() == DialogResult.OK)
					{
						var array = ofConfigFileOpenDlg.FileName.Split('\\');
						fskConfigFileName = array[array.Length - 1];
						fskConfigFilePath = "";
						int i;
						for (i = 0; i < array.Length - 2; i++)
						{
							fskConfigFilePath = fskConfigFilePath + array[i] + "\\";
						}
						fskConfigFilePath += array[i];
						configFileStream = new FileStream(fskConfigFilePath + "\\" + fskConfigFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
						device.OpenConfig(ref configFileStream);
						isFskConfigFileOpen = true;
						tsLblConfigFileName.Text = "Config File: " + fskConfigFileName;
						saveToolStripMenuItem.Text = "Save Config \"" + fskConfigFileName + "\"";
					}
					else
					{
						isFskConfigFileOpen = false;
					}
					return;
				}
				catch (Exception ex)
				{
					isFskConfigFileOpen = false;
					OnError(1, "ERROR: " + ex.Message);
					return;
				}
			}
			try
			{
				ofConfigFileOpenDlg.InitialDirectory = loRaConfigFilePath;
				ofConfigFileOpenDlg.FileName = loRaConfigFileName;
				if (ofConfigFileOpenDlg.ShowDialog() == DialogResult.OK)
				{
					var array2 = ofConfigFileOpenDlg.FileName.Split('\\');
					loRaConfigFileName = array2[array2.Length - 1];
					loRaConfigFilePath = "";
					int j;
					for (j = 0; j < array2.Length - 2; j++)
					{
						loRaConfigFilePath = loRaConfigFilePath + array2[j] + "\\";
					}
					loRaConfigFilePath += array2[j];
					configFileStream = new FileStream(loRaConfigFilePath + "\\" + loRaConfigFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
					device.OpenConfig(ref configFileStream);
					isLoRaConfigFileOpen = true;
					tsLblConfigFileName.Text = "Config File: " + loRaConfigFileName;
					saveToolStripMenuItem.Text = "Save Config \"" + loRaConfigFileName + "\"";
				}
				else
				{
					isLoRaConfigFileOpen = false;
				}
			}
			catch (Exception ex2)
			{
				isLoRaConfigFileOpen = false;
				OnError(1, "ERROR: " + ex2.Message);
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Validate();
			try
			{
				OnError(0, "-");
				if (!IsLoRaOn)
				{
					if (isFskConfigFileOpen)
					{
						var dialogResult = MessageBox.Show("Do you want to overwrite the current config file?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
						if (dialogResult == DialogResult.Yes)
						{
							configFileStream = new FileStream(fskConfigFilePath + "\\" + fskConfigFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
							device.SaveConfig(ref configFileStream);
						}
					}
					else
					{
						saveAsToolStripMenuItem_Click(sender, e);
					}
				}
				else if (isLoRaConfigFileOpen)
				{
					var dialogResult2 = MessageBox.Show("Do you want to overwrite the current config file?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if (dialogResult2 == DialogResult.Yes)
					{
						configFileStream = new FileStream(loRaConfigFilePath + "\\" + loRaConfigFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
						device.SaveConfig(ref configFileStream);
					}
				}
				else
				{
					saveAsToolStripMenuItem_Click(sender, e);
				}
			}
			catch (Exception ex)
			{
				OnError(1, "ERROR: " + ex.Message);
			}
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Validate();
			if (!IsLoRaOn)
			{
				try
				{
					OnError(0, "-");
					sfConfigFileSaveDlg.InitialDirectory = fskConfigFilePath;
					sfConfigFileSaveDlg.FileName = fskConfigFileName;
					if (sfConfigFileSaveDlg.ShowDialog() == DialogResult.OK)
					{
						var array = sfConfigFileSaveDlg.FileName.Split('\\');
						fskConfigFileName = array[array.Length - 1];
						fskConfigFilePath = "";
						int i;
						for (i = 0; i < array.Length - 2; i++)
						{
							fskConfigFilePath = fskConfigFilePath + array[i] + "\\";
						}
						fskConfigFilePath += array[i];
						configFileStream = new FileStream(fskConfigFilePath + "\\" + fskConfigFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
						tsLblConfigFileName.Text = "Config File: " + fskConfigFileName;
						saveToolStripMenuItem.Text = "Save Config \"" + fskConfigFileName + "\"";
						device.SaveConfig(ref configFileStream);
						isFskConfigFileOpen = true;
					}
					return;
				}
				catch (Exception ex)
				{
					OnError(1, "ERROR: " + ex.Message);
					isFskConfigFileOpen = false;
					return;
				}
			}
			try
			{
				OnError(0, "-");
				sfConfigFileSaveDlg.InitialDirectory = loRaConfigFilePath;
				sfConfigFileSaveDlg.FileName = loRaConfigFileName;
				if (sfConfigFileSaveDlg.ShowDialog() == DialogResult.OK)
				{
					var array2 = sfConfigFileSaveDlg.FileName.Split('\\');
					loRaConfigFileName = array2[array2.Length - 1];
					loRaConfigFilePath = "";
					int j;
					for (j = 0; j < array2.Length - 2; j++)
					{
						loRaConfigFilePath = loRaConfigFilePath + array2[j] + "\\";
					}
					loRaConfigFilePath += array2[j];
					configFileStream = new FileStream(loRaConfigFilePath + "\\" + loRaConfigFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
					tsLblConfigFileName.Text = "Config File: " + loRaConfigFileName;
					saveToolStripMenuItem.Text = "Save Config \"" + loRaConfigFileName + "\"";
					device.SaveConfig(ref configFileStream);
					isLoRaConfigFileOpen = true;
				}
			}
			catch (Exception ex2)
			{
				OnError(1, "ERROR: " + ex2.Message);
				isLoRaConfigFileOpen = false;
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void modemToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				tsLblConfigFileName.Text = "Config File: -";
				if (sender == modemFskToolStripMenuItem || sender == tsBtnModemFsk)
				{
					modemFskToolStripMenuItem.Checked = true;
					tsBtnModemFsk.Checked = true;
					modemLoRaToolStripMenuItem.Checked = false;
					tsBtnModemLoRa.Checked = false;
					tsLblPerModeOn.Visible = false;
					tsSeparatorPerModeOn.Visible = false;
					InitializeDevice(isLoRaOn: false);
				}
				else
				{
					modemFskToolStripMenuItem.Checked = false;
					tsBtnModemFsk.Checked = false;
					modemLoRaToolStripMenuItem.Checked = true;
					tsBtnModemLoRa.Checked = true;
					tsLblPerModeOn.Visible = IsLoRaPacketUsePerOn;
					tsSeparatorPerModeOn.Visible = IsLoRaPacketUsePerOn;
					InitializeDevice(isLoRaOn: true);
				}
				appSettings.SetValue("IsLoRaOn", IsLoRaOn.ToString());
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

		private void resetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.Reset();
				tsChipVersion.Text = device.Version.ToString();
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

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				device.ReadRegisters();
				tsChipVersion.Text = device.Version.ToString();
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

		private void showRegistersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showRegistersToolStripMenuItem.Checked || tsBtnShowRegisters.Checked)
			{
				showRegistersToolStripMenuItem.Checked = false;
				tsBtnShowRegisters.Checked = false;
				if (frmRegisters != null)
				{
					frmRegisters.Hide();
				}
				if (frmSpectrumAnalyser != null || device.IsPacketHandlerStarted)
				{
					frmRegisters.RegistersFormEnabled = true;
				}
				return;
			}
			showRegistersToolStripMenuItem.Checked = true;
			tsBtnShowRegisters.Checked = true;
			if (frmRegisters == null)
			{
				frmRegisters = new RegistersForm();
				frmRegisters.FormClosed += frmRegisters_FormClosed;
				frmRegisters.Disposed += frmRegisters_Disposed;
				frmRegisters.Device = device;
				frmRegisters.AppSettings = appSettings;
			}
			if (frmSpectrumAnalyser != null || device.IsPacketHandlerStarted)
			{
				frmRegisters.RegistersFormEnabled = false;
			}
			frmRegisters.Show();
		}

		private void monitorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				OnError(0, "-");
				if (sender == monitorOffToolStripMenuItem || sender == tsBtnMonitorOff)
				{
					device.Monitor = false;
				}
				else
				{
					device.Monitor = true;
				}
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

		private void startuptimeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (startuptimeToolStripMenuItem.Checked || tsBtnStartupTime.Checked)
			{
				startuptimeToolStripMenuItem.Checked = false;
				tsBtnStartupTime.Checked = false;
				if (frmStartupTime != null)
				{
					frmStartupTime.Hide();
				}
				return;
			}
			startuptimeToolStripMenuItem.Checked = true;
			tsBtnStartupTime.Checked = true;
			if (frmStartupTime == null)
			{
				frmStartupTime = new RxTxStartupTimeForm();
				frmStartupTime.FormClosed += frmStartupTime_FormClosed;
				frmStartupTime.Disposed += frmStartupTime_Disposed;
				frmStartupTime.Device = device;
				frmStartupTime.AppSettings = appSettings;
			}
			frmStartupTime.Show();
		}

		private void rssiAnalyserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (rssiAnalyserToolStripMenuItem.Checked)
			{
				if (frmRssiAnalyser != null)
				{
					frmRssiAnalyser.Close();
				}
				rssiAnalyserToolStripMenuItem.Checked = false;
				return;
			}
			if (frmSpectrumAnalyser != null)
			{
				frmSpectrumAnalyser.Close();
			}
			if (frmRssiAnalyser == null)
			{
				frmRssiAnalyser = new RssiAnalyserForm();
				frmRssiAnalyser.FormClosed += frmRssiAnalyser_FormClosed;
				frmRssiAnalyser.Disposed += frmRssiAnalyser_Disposed;
				frmRssiAnalyser.Device = device;
				frmRssiAnalyser.AppSettings = appSettings;
			}
			frmRssiAnalyser.Show();
			rssiAnalyserToolStripMenuItem.Checked = true;
		}

		private void spectrumAnalyserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (spectrumAnalyserToolStripMenuItem.Checked)
			{
				if (frmSpectrumAnalyser != null)
				{
					frmSpectrumAnalyser.Close();
				}
				spectrumAnalyserToolStripMenuItem.Checked = false;
				return;
			}
			if (frmRssiAnalyser != null)
			{
				frmRssiAnalyser.Close();
			}
			if (frmSpectrumAnalyser == null)
			{
				frmSpectrumAnalyser = new SpectrumAnalyserForm();
				frmSpectrumAnalyser.FormClosed += frmSpectrumAnalyser_FormClosed;
				frmSpectrumAnalyser.Disposed += frmSpectrumAnalyser_Disposed;
				frmSpectrumAnalyser.Device = device;
				frmSpectrumAnalyser.AppSettings = appSettings;
			}
			frmSpectrumAnalyser.Show();
			spectrumAnalyserToolStripMenuItem.Checked = true;
		}

		private void showHelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showHelpToolStripMenuItem.Checked || tsBtnShowHelp.Checked)
			{
				showHelpToolStripMenuItem.Checked = false;
				tsBtnShowHelp.Checked = false;
				if (frmHelp != null)
				{
					frmHelp.Hide();
				}
				return;
			}
			tsBtnShowHelp.Checked = true;
			if (frmHelp == null)
			{
				frmHelp = new HelpForm();
				frmHelp.FormClosed += frmHelp_FormClosed;
				frmHelp.Disposed += frmHelp_Disposed;
			}
			var location = default(Point);
			location.X = Location.X + Width;
			location.Y = Location.Y;
			frmHelp.Location = location;
			frmHelp.Show();
		}

		private void usersGuideToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (File.Exists(Application.StartupPath + "\\SX1276SKA_usersguide.pdf"))
			{
				Process.Start(Application.StartupPath + "\\SX1276SKA_usersguide.pdf");
			}
			else
			{
				MessageBox.Show("Unable to find the user's guide document!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
            var aboutBox = new AboutBox
            {
                Version = device.Version.ToString(2)
            };
            aboutBox.ShowDialog();
		}
	}
}
