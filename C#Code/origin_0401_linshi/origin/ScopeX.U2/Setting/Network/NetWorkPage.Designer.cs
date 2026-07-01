using System;
using System.Diagnostics;

namespace ScopeX.U2
{
    partial class NetWorkPage
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            UserControls.RadioButtonItem radioButtonItem1 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem2 = new UserControls.RadioButtonItem();
            IPAddress ipAddress1 = new IPAddress();
            IPAddress ipAddress2 = new IPAddress();
            IPAddress ipAddress3 = new IPAddress();
            IPAddress ipAddress4 = new IPAddress();
            IPAddress ipAddress5 = new IPAddress();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt5 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt6 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt7 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt8 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt9 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt10 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt11 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt12 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt13 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt14 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt15 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt16 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt17 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt18 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt19 = new UserControls.DefaultHighlightPrompt();
            TcmNetWork = new UserControls.TabControlMenu();
            TpgNetWork = new UserControls.TabControlMenuPage();
            RdoGetIpMethod = new UserControls.UIRadioButtonGroup();
            TbxDNSBackup = new IPTextBox();
            TbxDNSMain = new IPTextBox();
            TbxGateway = new IPTextBox();
            TbxIPMask = new IPTextBox();
            TbxIPAdress = new IPTextBox();
            CbxNetworkAdapter = new UserControls.ComboBoxEx();
            RdoNetWork = new UserControls.UIRadioButtonGroup();
            LblNetWorkStatus = new UserControls.ScopeXLabel();
            //BtnSetIpAuto = new UserControls.ScopeXIconButton();
            LblNetWorkCardName = new UserControls.ScopeXLabel();
            BtnSettingIP = new UserControls.ScopeXIconButton();
            LblGetIPAddressMethod = new UserControls.ScopeXLabel();
            LblIPAdress = new UserControls.ScopeXLabel();
            LblIPMask = new UserControls.ScopeXLabel();
            LblDNSBackup = new UserControls.ScopeXLabel();
            LblGateway = new UserControls.ScopeXLabel();
            TbxMAC = new UserControls.ScopeXTextBox();
            LblDNSMain = new UserControls.ScopeXLabel();
            LblMAC = new UserControls.ScopeXLabel();
            TpgUSB = new UserControls.TabControlMenuPage();
            TbxVisaAddress = new UserControls.ScopeXTextBox();
            LblSerialNum = new UserControls.ScopeXLabel();
            LblProductID = new UserControls.ScopeXLabel();
            LblVendorID = new UserControls.ScopeXLabel();
            LblVADDR = new UserControls.ScopeXLabel();
            LblSN = new UserControls.ScopeXLabel();
            LblPID = new UserControls.ScopeXLabel();
            LblVID = new UserControls.ScopeXLabel();
            TpgWebServer = new UserControls.TabControlMenuPage();
            webWaittingLabel = new System.Windows.Forms.Label();
            ChkWebStatus = new UserControls.ScopeXSwitchButton();
            LblServer = new UserControls.ScopeXLabel();
            BtnWebStatus = new UserControls.ScopeXIconButton();
            LblStatus = new UserControls.ScopeXLabel();
            TbxPort = new UserControls.ScopeXTextBox();
            LblPort = new UserControls.ScopeXLabel();
            TcmNetWork.SuspendLayout();
            TpgNetWork.SuspendLayout();
            TpgUSB.SuspendLayout();
            TpgWebServer.SuspendLayout();
            SuspendLayout();
            // 
            // TcmNetWork
            // 
            TcmNetWork.BackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            TcmNetWork.BorderColor = System.Drawing.Color.FromArgb(41, 42, 46);
            TcmNetWork.Controls.Add(TpgNetWork);
            //TcmNetWork.Controls.Add(TpgUSB);
            //TcmNetWork.Controls.Add(TpgWebServer);
            TcmNetWork.Dock = System.Windows.Forms.DockStyle.Fill;
            TcmNetWork.HotTrack = true;
            TcmNetWork.ItemSize = new System.Drawing.Size(35, 100);
            TcmNetWork.LanguageKey = null;
            TcmNetWork.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TcmNetWork.Location = new System.Drawing.Point(0, 0);
            TcmNetWork.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            TcmNetWork.Multiline = true;
            TcmNetWork.Name = "TcmNetWork";
            TcmNetWork.Padding = new System.Drawing.Point(0, 0);
            TcmNetWork.PanelBackColor = System.Drawing.Color.FromArgb(0, 171, 209);
            TcmNetWork.PanelNomalBackColor = System.Drawing.Color.FromArgb(46, 47, 40);
            TcmNetWork.SelectedIndex = 0;
            TcmNetWork.Size = new System.Drawing.Size(459, 493);
            TcmNetWork.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            TcmNetWork.TabIndex = 0;
            TcmNetWork.TextDirection = UserControls.TabControlMenu.Direction.Horizontal;
            // 
            // TpgNetWork
            // 
            TpgNetWork.BackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            TpgNetWork.Controls.Add(RdoGetIpMethod);
            TpgNetWork.Controls.Add(TbxDNSBackup);
            TpgNetWork.Controls.Add(TbxDNSMain);
            TpgNetWork.Controls.Add(TbxGateway);
            TpgNetWork.Controls.Add(TbxIPMask);
            TpgNetWork.Controls.Add(TbxIPAdress);
            TpgNetWork.Controls.Add(CbxNetworkAdapter);
            TpgNetWork.Controls.Add(RdoNetWork);
            TpgNetWork.Controls.Add(LblNetWorkStatus);
            //TpgNetWork.Controls.Add(BtnSetIpAuto);
            TpgNetWork.Controls.Add(LblNetWorkCardName);
            TpgNetWork.Controls.Add(BtnSettingIP);
            TpgNetWork.Controls.Add(LblGetIPAddressMethod);
            TpgNetWork.Controls.Add(LblIPAdress);
            TpgNetWork.Controls.Add(LblIPMask);
            TpgNetWork.Controls.Add(LblDNSBackup);
            TpgNetWork.Controls.Add(LblGateway);
            TpgNetWork.Controls.Add(TbxMAC);
            TpgNetWork.Controls.Add(LblDNSMain);
            TpgNetWork.Controls.Add(LblMAC);
            TpgNetWork.Dock = System.Windows.Forms.DockStyle.Fill;
            TpgNetWork.Font = new System.Drawing.Font("MiSans", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TpgNetWork.ForeColor = System.Drawing.Color.White;
            TpgNetWork.HeaderColor = System.Drawing.Color.Black;
            TpgNetWork.HeaderFont = new System.Drawing.Font("MiSans", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TpgNetWork.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TpgNetWork.Location = new System.Drawing.Point(1, 35);
            TpgNetWork.Margin = new System.Windows.Forms.Padding(0);
            TpgNetWork.Name = "TpgNetWork";
            TpgNetWork.Size = new System.Drawing.Size(458, 492);
            TpgNetWork.TabIndex = 0;
            TpgNetWork.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgNetWork");
            // 
            // RdoGetIpMethod
            // 
            RdoGetIpMethod.BackColor = System.Drawing.Color.AliceBlue;
            RdoGetIpMethod.BorderColor = System.Drawing.Color.AliceBlue;
            RdoGetIpMethod.BorderThickness = 0;
            RdoGetIpMethod.ButtonBackColor = System.Drawing.Color.FromArgb(50, 50, 54);
            RdoGetIpMethod.ButtonFont = null;
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgNetWork.RdoGetIpMethod.panel1.Btn_0"); // "自动";
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgNetWork.RdoGetIpMethod.panel1.Btn_1"); // "手动";
            RdoGetIpMethod.ButtonItems = new ScopeX.UserControls.RadioButtonItem[] {
        radioButtonItem1,
        radioButtonItem2};
            RdoGetIpMethod.ButtonOffset = 10;
            RdoGetIpMethod.ButtonTextColor = System.Drawing.Color.White;
            RdoGetIpMethod.ChoosedButtonColor = System.Drawing.Color.FromArgb(18, 183, 245);
            RdoGetIpMethod.ChoosedButtonIndex = 0;
            RdoGetIpMethod.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoGetIpMethod.ContentBackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            RdoGetIpMethod.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoGetIpMethod.FocusBorderColor = System.Drawing.Color.White;
            RdoGetIpMethod.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoGetIpMethod.ForeColor = System.Drawing.Color.White;
            RdoGetIpMethod.Height = 30;
            RdoGetIpMethod.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoGetIpMethod.Location = new System.Drawing.Point(216, 106);
            RdoGetIpMethod.Name = "RdoGetIpMethod";
            RdoGetIpMethod.Size = new System.Drawing.Size(250, 30);
            RdoGetIpMethod.StyleFlags = UserControls.Style.StyleFlag.None;
            RdoGetIpMethod.StylizeFlag = true;
            RdoGetIpMethod.TabIndex = 20;
            // 
            // TbxDNSBackup
            // 
            TbxDNSBackup.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxDNSBackup.Cursor = System.Windows.Forms.Cursors.Default;
            TbxDNSBackup.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbxDNSBackup.ForeColor = System.Drawing.Color.White;
            ipAddress1.IP1 = "0";
            ipAddress1.IP2 = "0";
            ipAddress1.IP3 = "0";
            ipAddress1.IP4 = "0";
            TbxDNSBackup.IPAddress = ipAddress1;
            TbxDNSBackup.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            TbxDNSBackup.Location = new System.Drawing.Point(216, 321);
            TbxDNSBackup.Name = "TbxDNSBackup";
            TbxDNSBackup.SelectedColor = System.Drawing.Color.FromArgb(40, 71, 193);
            TbxDNSBackup.Size = new System.Drawing.Size(250, 30);
            TbxDNSBackup.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxDNSBackup.StylizeFlag = true;
            TbxDNSBackup.TabIndex = 15;
            TbxDNSBackup.Text = "0.0.0.0";
            TbxDNSBackup.Visible = false;
            // 
            // TbxDNSMain
            // 
            TbxDNSMain.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxDNSMain.Cursor = System.Windows.Forms.Cursors.Default;
            TbxDNSMain.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbxDNSMain.ForeColor = System.Drawing.Color.White;
            ipAddress2.IP1 = "0";
            ipAddress2.IP2 = "0";
            ipAddress2.IP3 = "0";
            ipAddress2.IP4 = "0";
            TbxDNSMain.IPAddress = ipAddress2;
            TbxDNSMain.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            TbxDNSMain.Location = new System.Drawing.Point(216, 278);
            TbxDNSMain.Name = "TbxDNSMain";
            TbxDNSMain.SelectedColor = System.Drawing.Color.FromArgb(40, 71, 193);
            TbxDNSMain.Size = new System.Drawing.Size(250, 30);
            TbxDNSMain.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxDNSMain.StylizeFlag = true;
            TbxDNSMain.TabIndex = 13;
            TbxDNSMain.Text = "0.0.0.0";
            TbxDNSMain.Visible = false;
            // 
            // TbxGateway
            // 
            TbxGateway.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxGateway.Cursor = System.Windows.Forms.Cursors.Default;
            TbxGateway.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbxGateway.ForeColor = System.Drawing.Color.White;
            ipAddress3.IP1 = "0";
            ipAddress3.IP2 = "0";
            ipAddress3.IP3 = "0";
            ipAddress3.IP4 = "0";
            TbxGateway.IPAddress = ipAddress3;
            TbxGateway.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            TbxGateway.Location = new System.Drawing.Point(216, 235);
            TbxGateway.Name = "TbxGateway";
            TbxGateway.SelectedColor = System.Drawing.Color.FromArgb(40, 71, 193);
            TbxGateway.Size = new System.Drawing.Size(250, 30);
            TbxGateway.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxGateway.StylizeFlag = true;
            TbxGateway.TabIndex = 11;
            TbxGateway.Text = "0.0.0.0";
            TbxGateway.Visible = false;
            // 
            // TbxIPMask
            // 
            TbxIPMask.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxIPMask.Cursor = System.Windows.Forms.Cursors.Default;
            TbxIPMask.Font = new System.Drawing.Font("MiSans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbxIPMask.ForeColor = System.Drawing.Color.White;
            ipAddress4.IP1 = "0";
            ipAddress4.IP2 = "0";
            ipAddress4.IP3 = "0";
            ipAddress4.IP4 = "0";
            TbxIPMask.IPAddress = ipAddress4;
            TbxIPMask.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            TbxIPMask.Location = new System.Drawing.Point(216, 192);
            TbxIPMask.Name = "TbxIPMask";
            TbxIPMask.SelectedColor = System.Drawing.Color.FromArgb(40, 71, 193);
            TbxIPMask.Size = new System.Drawing.Size(250, 30);
            TbxIPMask.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxIPMask.StylizeFlag = true;
            TbxIPMask.TabIndex = 9;
            TbxIPMask.Text = "0.0.0.0";
            // 
            // TbxIPAdress
            // 
            TbxIPAdress.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxIPAdress.Cursor = System.Windows.Forms.Cursors.Default;
            TbxIPAdress.Font = new System.Drawing.Font("MiSans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbxIPAdress.ForeColor = System.Drawing.Color.White;
            ipAddress5.IP1 = "0";
            ipAddress5.IP2 = "0";
            ipAddress5.IP3 = "0";
            ipAddress5.IP4 = "0";
            TbxIPAdress.IPAddress = ipAddress5;
            TbxIPAdress.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            TbxIPAdress.Location = new System.Drawing.Point(216, 149);
            TbxIPAdress.Name = "TbxIPAdress";
            TbxIPAdress.SelectedColor = System.Drawing.Color.FromArgb(40, 71, 193);
            TbxIPAdress.Size = new System.Drawing.Size(250, 30);
            TbxIPAdress.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxIPAdress.StylizeFlag = true;
            TbxIPAdress.TabIndex = 7;
            TbxIPAdress.Text = "0.0.0.0";
            // 
            // CbxNetworkAdapter
            // 
            CbxNetworkAdapter.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxNetworkAdapter.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxNetworkAdapter.BorderThickness = 0;
            CbxNetworkAdapter.CornerRadius = 0;
            CbxNetworkAdapter.DataSource = null;
            CbxNetworkAdapter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            CbxNetworkAdapter.DropDownHeight = 200;
            CbxNetworkAdapter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CbxNetworkAdapter.DropDownWidth = 250;
            CbxNetworkAdapter.DropKey = System.Windows.Forms.Keys.Space;
            CbxNetworkAdapter.DroppedDown = false;
            CbxNetworkAdapter.FocusColor = System.Drawing.Color.FromArgb(51, 52, 56);
            CbxNetworkAdapter.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxNetworkAdapter.ForeColor = System.Drawing.Color.Silver;
            CbxNetworkAdapter.FormattingEnabled = true;
            CbxNetworkAdapter.GetDisPlayName = null;
            CbxNetworkAdapter.Height = 30;
            CbxNetworkAdapter.ImageMode = false;
            CbxNetworkAdapter.ItemHeight = 40;
            CbxNetworkAdapter.KeyDropEnble = true;
            CbxNetworkAdapter.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            CbxNetworkAdapter.Location = new System.Drawing.Point(216, 63);
            CbxNetworkAdapter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CbxNetworkAdapter.MaxDropDownItems = 8;
            CbxNetworkAdapter.Name = "CbxNetworkAdapter";
            CbxNetworkAdapter.RectBtnWidth = 20;
            CbxNetworkAdapter.SelectedBackColor = System.Drawing.Color.FromArgb(18, 183, 245);
            CbxNetworkAdapter.SelectedIndex = -1;
            CbxNetworkAdapter.SelectedItem = null;
            CbxNetworkAdapter.SelectedText = "";
            CbxNetworkAdapter.Size = new System.Drawing.Size(250, 30);
            CbxNetworkAdapter.Soreted = false;
            CbxNetworkAdapter.StyleFlags = UserControls.Style.StyleFlag.None;
            CbxNetworkAdapter.StylizeFlag = true;
            CbxNetworkAdapter.TabIndex = 3;
            CbxNetworkAdapter.Text = "";
            CbxNetworkAdapter.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            CbxNetworkAdapter.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            CbxNetworkAdapter.SelectedIndexChanged += CbxNetworkAdapter_SelectedIndexChanged;
            // 
            // RdoNetWork
            // 
            RdoNetWork.BackColor = System.Drawing.Color.AliceBlue;
            RdoNetWork.BorderColor = System.Drawing.Color.AliceBlue;
            RdoNetWork.BorderThickness = 0;
            RdoNetWork.ButtonBackColor = System.Drawing.Color.FromArgb(50, 50, 54);
            RdoNetWork.ButtonFont = null;
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgNetWork.RdoNetWork.panel1.Btn_0"); // "已连接";
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgNetWork.RdoNetWork.panel1.Btn_1"); // "所有";
            RdoNetWork.ButtonItems = new ScopeX.UserControls.RadioButtonItem[] {
        radioButtonItem1,
        radioButtonItem2};
            RdoNetWork.ButtonOffset = 10;
            RdoNetWork.ButtonTextColor = System.Drawing.Color.White;
            RdoNetWork.ChoosedButtonColor = System.Drawing.Color.FromArgb(18, 183, 245);
            RdoNetWork.ChoosedButtonIndex = 0;
            RdoNetWork.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoNetWork.ContentBackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            RdoNetWork.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoNetWork.FocusBorderColor = System.Drawing.Color.White;
            RdoNetWork.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoNetWork.ForeColor = System.Drawing.Color.White;
            RdoNetWork.Height = 30;
            RdoNetWork.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoNetWork.Location = new System.Drawing.Point(216, 20);
            RdoNetWork.Name = "RdoNetWork";
            RdoNetWork.Size = new System.Drawing.Size(250, 30);
            RdoNetWork.StyleFlags = UserControls.Style.StyleFlag.None;
            RdoNetWork.StylizeFlag = true;
            RdoNetWork.TabIndex = 1;
            RdoNetWork.IndexChanged += RdoNetWork_IndexChanged;
            // 
            // LblNetWorkStatus
            // 
            LblNetWorkStatus.BackColor = System.Drawing.Color.Empty;
            LblNetWorkStatus.BorderColor = System.Drawing.Color.Black;
            LblNetWorkStatus.BorderThickness = 0;
            LblNetWorkStatus.CornerRadius = 0;
            LblNetWorkStatus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblNetWorkStatus.ForeColor = System.Drawing.Color.White;
            LblNetWorkStatus.HighlightPrompt = defaultHighlightPrompt1;
            LblNetWorkStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblNetWorkStatus.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblNetWorkStatus.Location = new System.Drawing.Point(16, 20);
            LblNetWorkStatus.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblNetWorkStatus.MultyLineFlag = false;
            LblNetWorkStatus.Name = "LblNetWorkStatus";
            LblNetWorkStatus.Size = new System.Drawing.Size(160, 30);
            LblNetWorkStatus.StyleFlags = UserControls.Style.StyleFlag.None;
            LblNetWorkStatus.StylizeFlag = true;
            LblNetWorkStatus.TabIndex = 0;
            LblNetWorkStatus.TabStop = false;
            LblNetWorkStatus.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgNetWork.LblNetWorkStatus"); // "网络选择";
            LblNetWorkStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblNetWorkStatus.Token = null;
            //// 
            //// BtnSetIpAuto
            //// 
            //BtnSetIpAuto.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            //BtnSetIpAuto.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            //BtnSetIpAuto.BorderThickness = 1;
            //BtnSetIpAuto.CornerRadius = 0;
            //BtnSetIpAuto.Cursor = System.Windows.Forms.Cursors.Hand;
            //BtnSetIpAuto.DaskArray = null;
            //BtnSetIpAuto.DropKey = System.Windows.Forms.Keys.Space;
            //BtnSetIpAuto.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            //BtnSetIpAuto.ForeColor = System.Drawing.Color.White;
            //BtnSetIpAuto.Height = 30;
            //BtnSetIpAuto.Icon = null;
            //BtnSetIpAuto.IconOffset = 10;
            //BtnSetIpAuto.IconSize = new System.Drawing.Size(24, 24);
            //BtnSetIpAuto.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            //BtnSetIpAuto.IsIndicatorShow = false;
            //BtnSetIpAuto.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            //BtnSetIpAuto.Location = new System.Drawing.Point(311, 408);
            //BtnSetIpAuto.Margin = new System.Windows.Forms.Padding(2);
            //BtnSetIpAuto.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            //BtnSetIpAuto.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            //BtnSetIpAuto.MouseInBorderThickness = 0;
            //BtnSetIpAuto.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            //BtnSetIpAuto.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            //BtnSetIpAuto.Name = "BtnSetIpAuto";
            //BtnSetIpAuto.PressedBackColor = System.Drawing.Color.Gray;
            //BtnSetIpAuto.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            //BtnSetIpAuto.PressedBorderThickness = 0;
            //BtnSetIpAuto.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            //BtnSetIpAuto.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            //BtnSetIpAuto.Size = new System.Drawing.Size(85, 30);
            //BtnSetIpAuto.StyleFlags = UserControls.Style.StyleFlag.None;
            //BtnSetIpAuto.StylizeFlag = true;
            //BtnSetIpAuto.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            //BtnSetIpAuto.SVGPath = "";
            //BtnSetIpAuto.TabIndex = 19;
            //BtnSetIpAuto.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgNetWork.BtnSetIpAuto"); // "自动获取";
            //BtnSetIpAuto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //BtnSetIpAuto.Click += BtnSetIpAuto_Click;
            // 
            // LblNetWorkCardName
            // 
            LblNetWorkCardName.BackColor = System.Drawing.Color.Empty;
            LblNetWorkCardName.BorderColor = System.Drawing.Color.Black;
            LblNetWorkCardName.BorderThickness = 0;
            LblNetWorkCardName.CornerRadius = 0;
            LblNetWorkCardName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblNetWorkCardName.ForeColor = System.Drawing.Color.White;
            LblNetWorkCardName.HighlightPrompt = defaultHighlightPrompt2;
            LblNetWorkCardName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblNetWorkCardName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblNetWorkCardName.Location = new System.Drawing.Point(16, 63);
            LblNetWorkCardName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblNetWorkCardName.MultyLineFlag = false;
            LblNetWorkCardName.Name = "LblNetWorkCardName";
            LblNetWorkCardName.Size = new System.Drawing.Size(160, 30);
            LblNetWorkCardName.StyleFlags = UserControls.Style.StyleFlag.None;
            LblNetWorkCardName.StylizeFlag = true;
            LblNetWorkCardName.TabIndex = 2;
            LblNetWorkCardName.TabStop = false;
            LblNetWorkCardName.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgNetWork.LblNetWorkCardName"); // "网络名称";
            LblNetWorkCardName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblNetWorkCardName.Token = null;
            // 
            // BtnSettingIP
            // 
            BtnSettingIP.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSettingIP.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSettingIP.BorderThickness = 1;
            BtnSettingIP.CornerRadius = 0;
            BtnSettingIP.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSettingIP.DaskArray = null;
            BtnSettingIP.DropKey = System.Windows.Forms.Keys.Space;
            BtnSettingIP.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnSettingIP.ForeColor = System.Drawing.Color.White;
            BtnSettingIP.Height = 30;
            BtnSettingIP.Icon = null;
            BtnSettingIP.IconOffset = 10;
            BtnSettingIP.IconSize = new System.Drawing.Size(24, 24);
            BtnSettingIP.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSettingIP.IsIndicatorShow = false;
            BtnSettingIP.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSettingIP.Location = new System.Drawing.Point(216, 300);
            BtnSettingIP.Margin = new System.Windows.Forms.Padding(2);
            BtnSettingIP.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSettingIP.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSettingIP.MouseInBorderThickness = 0;
            BtnSettingIP.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSettingIP.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSettingIP.Name = "BtnSettingIP";
            BtnSettingIP.PressedBackColor = System.Drawing.Color.Gray;
            BtnSettingIP.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSettingIP.PressedBorderThickness = 0;
            BtnSettingIP.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSettingIP.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSettingIP.Size = new System.Drawing.Size(250, 30);
            BtnSettingIP.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnSettingIP.StylizeFlag = true;
            BtnSettingIP.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSettingIP.SVGPath = "";
            BtnSettingIP.TabIndex = 18;
            BtnSettingIP.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgNetWork.BtnSettingIP"); // "设置  IP";
            BtnSettingIP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSettingIP.Click += BtnSettingIP_Click;
            // 
            // LblGetIPAddressMethod
            // 
            LblGetIPAddressMethod.BackColor = System.Drawing.Color.Empty;
            LblGetIPAddressMethod.BorderColor = System.Drawing.Color.Black;
            LblGetIPAddressMethod.BorderThickness = 0;
            LblGetIPAddressMethod.CornerRadius = 0;
            LblGetIPAddressMethod.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblGetIPAddressMethod.ForeColor = System.Drawing.Color.White;
            LblGetIPAddressMethod.HighlightPrompt = defaultHighlightPrompt3;
            LblGetIPAddressMethod.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblGetIPAddressMethod.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblGetIPAddressMethod.Location = new System.Drawing.Point(16, 106);
            LblGetIPAddressMethod.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblGetIPAddressMethod.MultyLineFlag = false;
            LblGetIPAddressMethod.Name = "LblGetIPAddressMethod";
            LblGetIPAddressMethod.Size = new System.Drawing.Size(200, 30);
            LblGetIPAddressMethod.StyleFlags = UserControls.Style.StyleFlag.None;
            LblGetIPAddressMethod.StylizeFlag = true;
            LblGetIPAddressMethod.TabIndex = 4;
            LblGetIPAddressMethod.TabStop = false;
            LblGetIPAddressMethod.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgNetWork.LblGetIPAddressMethod"); // "IP获取方式";
            LblGetIPAddressMethod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblGetIPAddressMethod.Token = null;
            // 
            // LblIPAdress
            // 
            LblIPAdress.BackColor = System.Drawing.Color.Empty;
            LblIPAdress.BorderColor = System.Drawing.Color.Black;
            LblIPAdress.BorderThickness = 0;
            LblIPAdress.CornerRadius = 0;
            LblIPAdress.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblIPAdress.ForeColor = System.Drawing.Color.White;
            LblIPAdress.HighlightPrompt = defaultHighlightPrompt4;
            LblIPAdress.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblIPAdress.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblIPAdress.Location = new System.Drawing.Point(16, 149);
            LblIPAdress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblIPAdress.MultyLineFlag = false;
            LblIPAdress.Name = "LblIPAdress";
            LblIPAdress.Size = new System.Drawing.Size(160, 30);
            LblIPAdress.StyleFlags = UserControls.Style.StyleFlag.None;
            LblIPAdress.StylizeFlag = true;
            LblIPAdress.TabIndex = 6;
            LblIPAdress.TabStop = false;
            LblIPAdress.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgNetWork.LblIPAdress"); // "IP地址";
            LblIPAdress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblIPAdress.Token = null;
            // 
            // LblIPMask
            // 
            LblIPMask.BackColor = System.Drawing.Color.Empty;
            LblIPMask.BorderColor = System.Drawing.Color.Black;
            LblIPMask.BorderThickness = 0;
            LblIPMask.CornerRadius = 0;
            LblIPMask.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblIPMask.ForeColor = System.Drawing.Color.White;
            LblIPMask.HighlightPrompt = defaultHighlightPrompt5;
            LblIPMask.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblIPMask.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblIPMask.Location = new System.Drawing.Point(16, 192);
            LblIPMask.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblIPMask.MultyLineFlag = false;
            LblIPMask.Name = "LblIPMask";
            LblIPMask.Size = new System.Drawing.Size(160, 30);
            LblIPMask.StyleFlags = UserControls.Style.StyleFlag.None;
            LblIPMask.StylizeFlag = true;
            LblIPMask.TabIndex = 8;
            LblIPMask.TabStop = false;
            LblIPMask.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgNetWork.LblIPMask"); // "子网掩码";
            LblIPMask.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblIPMask.Token = null;
            // 
            // LblDNSBackup
            // 
            LblDNSBackup.BackColor = System.Drawing.Color.Empty;
            LblDNSBackup.BorderColor = System.Drawing.Color.Black;
            LblDNSBackup.BorderThickness = 0;
            LblDNSBackup.CornerRadius = 0;
            LblDNSBackup.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblDNSBackup.ForeColor = System.Drawing.Color.White;
            LblDNSBackup.HighlightPrompt = defaultHighlightPrompt6;
            LblDNSBackup.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblDNSBackup.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblDNSBackup.Location = new System.Drawing.Point(16, 321);
            LblDNSBackup.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblDNSBackup.MultyLineFlag = false;
            LblDNSBackup.Name = "LblDNSBackup";
            LblDNSBackup.Size = new System.Drawing.Size(160, 30);
            LblDNSBackup.StyleFlags = UserControls.Style.StyleFlag.None;
            LblDNSBackup.StylizeFlag = true;
            LblDNSBackup.TabIndex = 14;
            LblDNSBackup.TabStop = false;
            LblDNSBackup.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgNetWork.LblDNSBackup"); // "备用DNS";
            LblDNSBackup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblDNSBackup.Token = null;
            LblDNSBackup.Visible = false;
            // 
            // LblGateway
            // 
            LblGateway.BackColor = System.Drawing.Color.Empty;
            LblGateway.BorderColor = System.Drawing.Color.Black;
            LblGateway.BorderThickness = 0;
            LblGateway.CornerRadius = 0;
            LblGateway.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblGateway.ForeColor = System.Drawing.Color.White;
            LblGateway.HighlightPrompt = defaultHighlightPrompt7;
            LblGateway.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblGateway.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblGateway.Location = new System.Drawing.Point(16, 235);
            LblGateway.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblGateway.MultyLineFlag = false;
            LblGateway.Name = "LblGateway";
            LblGateway.Size = new System.Drawing.Size(160, 30);
            LblGateway.StyleFlags = UserControls.Style.StyleFlag.None;
            LblGateway.StylizeFlag = true;
            LblGateway.TabIndex = 10;
            LblGateway.TabStop = false;
            LblGateway.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgNetWork.LblGateway"); // "网关";
            LblGateway.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblGateway.Token = null;
            LblGateway.Visible = false;
            // 
            // TbxMAC
            // 
            TbxMAC.AcceptsTab = false;
            TbxMAC.AutoShowKeyBoard = true;
            TbxMAC.AutoSize = false;
            TbxMAC.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxMAC.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxMAC.BorderThickness = 0;
            TbxMAC.CornerRadius = 0;
            TbxMAC.Cursor = System.Windows.Forms.Cursors.IBeam;
            TbxMAC.Enabled = true;
            TbxMAC.EnbleSelectBorder = true;
            TbxMAC.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbxMAC.ForeColor = System.Drawing.Color.White;
            TbxMAC.Height = 30;
            TbxMAC.HideSelection = true;
            TbxMAC.KeyboardVerify = null;
            TbxMAC.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TbxMAC.Lines = new string[0];
            TbxMAC.Location = new System.Drawing.Point(216, 235);// new System.Drawing.Point(216, 364);
            TbxMAC.MaxLength = 32767;
            TbxMAC.Modified = false;
            TbxMAC.MouseEnterState = false;
            TbxMAC.Multiline = false;
            TbxMAC.Name = "TbxMAC";
            TbxMAC.ProcessCmdKeyFunc = null;
            TbxMAC.ReadOnly = true;
            TbxMAC.SelectedColor = System.Drawing.Color.FromArgb(0, 157, 255);
            TbxMAC.SelectedText = "";
            TbxMAC.SelectionLength = 0;
            TbxMAC.SelectionStart = 0;
            TbxMAC.ShortcutsEnabled = true;
            TbxMAC.Size = new System.Drawing.Size(250, 30);
            TbxMAC.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxMAC.StylizeFlag = true;
            TbxMAC.TabIndex = 17;
            TbxMAC.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            TbxMAC.UseSystemPasswordChar = false;
            TbxMAC.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            TbxMAC.WordWrap = true;
            // 
            // LblDNSMain
            // 
            LblDNSMain.BackColor = System.Drawing.Color.Empty;
            LblDNSMain.BorderColor = System.Drawing.Color.Black;
            LblDNSMain.BorderThickness = 0;
            LblDNSMain.CornerRadius = 0;
            LblDNSMain.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblDNSMain.ForeColor = System.Drawing.Color.White;
            LblDNSMain.HighlightPrompt = defaultHighlightPrompt8;
            LblDNSMain.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblDNSMain.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblDNSMain.Location = new System.Drawing.Point(16, 278);
            LblDNSMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblDNSMain.MultyLineFlag = false;
            LblDNSMain.Name = "LblDNSMain";
            LblDNSMain.Size = new System.Drawing.Size(160, 30);
            LblDNSMain.StyleFlags = UserControls.Style.StyleFlag.None;
            LblDNSMain.StylizeFlag = true;
            LblDNSMain.TabIndex = 12;
            LblDNSMain.TabStop = false;
            LblDNSMain.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgNetWork.LblDNSMain"); // "主DNS";
            LblDNSMain.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblDNSMain.Token = null;
            LblDNSMain.Visible = false;
            // 
            // LblMAC
            // 
            LblMAC.BackColor = System.Drawing.Color.Empty;
            LblMAC.BorderColor = System.Drawing.Color.Black;
            LblMAC.BorderThickness = 0;
            LblMAC.CornerRadius = 0;
            LblMAC.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblMAC.ForeColor = System.Drawing.Color.White;
            LblMAC.HighlightPrompt = defaultHighlightPrompt9;
            LblMAC.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblMAC.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblMAC.Location = new System.Drawing.Point(16, 235);// new System.Drawing.Point(16, 364);
            LblMAC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblMAC.MultyLineFlag = false;
            LblMAC.Name = "LblMAC";
            LblMAC.Size = new System.Drawing.Size(160, 30);
            LblMAC.StyleFlags = UserControls.Style.StyleFlag.None;
            LblMAC.StylizeFlag = true;
            LblMAC.TabIndex = 16;
            LblMAC.TabStop = false;
            LblMAC.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgNetWork.LblMAC"); // "MAC地址";
            LblMAC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblMAC.Token = null;
            // 
            // TpgUSB
            // 
            TpgUSB.BackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            TpgUSB.Controls.Add(TbxVisaAddress);
            TpgUSB.Controls.Add(LblSerialNum);
            TpgUSB.Controls.Add(LblProductID);
            TpgUSB.Controls.Add(LblVendorID);
            TpgUSB.Controls.Add(LblVADDR);
            TpgUSB.Controls.Add(LblSN);
            TpgUSB.Controls.Add(LblPID);
            TpgUSB.Controls.Add(LblVID);
            TpgUSB.Dock = System.Windows.Forms.DockStyle.Fill;
            TpgUSB.Font = new System.Drawing.Font("MiSans", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TpgUSB.ForeColor = System.Drawing.Color.White;
            TpgUSB.HeaderColor = System.Drawing.Color.White;
            TpgUSB.HeaderFont = new System.Drawing.Font("MiSans", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TpgUSB.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TpgUSB.Location = new System.Drawing.Point(1, 35);
            TpgUSB.Margin = new System.Windows.Forms.Padding(0);
            TpgUSB.Name = "TpgUSB";
            TpgUSB.Size = new System.Drawing.Size(458, 492);
            TpgUSB.TabIndex = 1;
            TpgUSB.Text = "USB";
            // 
            // TbxVisaAddress
            // 
            TbxVisaAddress.AcceptsTab = false;
            TbxVisaAddress.AutoShowKeyBoard = true;
            TbxVisaAddress.AutoSize = false;
            TbxVisaAddress.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxVisaAddress.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxVisaAddress.BorderThickness = 0;
            TbxVisaAddress.CornerRadius = 0;
            TbxVisaAddress.Cursor = System.Windows.Forms.Cursors.IBeam;
            TbxVisaAddress.Enabled = true;
            TbxVisaAddress.EnbleSelectBorder = true;
            TbxVisaAddress.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbxVisaAddress.ForeColor = System.Drawing.Color.White;
            TbxVisaAddress.Height = 30;
            TbxVisaAddress.HideSelection = true;
            TbxVisaAddress.KeyboardVerify = null;
            TbxVisaAddress.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TbxVisaAddress.Lines = new string[] {
        "USB0::0x5656::0x0857::{C044111}::INSTR" };
            TbxVisaAddress.Location = new System.Drawing.Point(151, 136);
            TbxVisaAddress.MaxLength = 32767;
            TbxVisaAddress.Modified = false;
            TbxVisaAddress.MouseEnterState = false;
            TbxVisaAddress.Multiline = false;
            TbxVisaAddress.Name = "TbxVisaAddress";
            TbxVisaAddress.ProcessCmdKeyFunc = null;
            TbxVisaAddress.ReadOnly = true;
            TbxVisaAddress.SelectedColor = System.Drawing.Color.FromArgb(0, 157, 255);
            TbxVisaAddress.SelectedText = "";
            TbxVisaAddress.SelectionLength = 0;
            TbxVisaAddress.SelectionStart = 0;
            TbxVisaAddress.ShortcutsEnabled = true;
            TbxVisaAddress.Size = new System.Drawing.Size(209, 30);
            TbxVisaAddress.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxVisaAddress.StylizeFlag = true;
            TbxVisaAddress.TabIndex = 58;
            TbxVisaAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            TbxVisaAddress.UseSystemPasswordChar = false;
            TbxVisaAddress.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            TbxVisaAddress.Visible = false;
            TbxVisaAddress.WordWrap = true;
            // 
            // LblSerialNum
            // 
            LblSerialNum.BackColor = System.Drawing.Color.Empty;
            LblSerialNum.BorderColor = System.Drawing.Color.Black;
            LblSerialNum.BorderThickness = 0;
            LblSerialNum.CornerRadius = 0;
            LblSerialNum.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSerialNum.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblSerialNum.HighlightPrompt = defaultHighlightPrompt10;
            LblSerialNum.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblSerialNum.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSerialNum.Location = new System.Drawing.Point(151, 98);
            LblSerialNum.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblSerialNum.MultyLineFlag = false;
            LblSerialNum.Name = "LblSerialNum";
            LblSerialNum.Size = new System.Drawing.Size(209, 30);
            LblSerialNum.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSerialNum.StylizeFlag = true;
            LblSerialNum.TabIndex = 57;
            LblSerialNum.TabStop = false;
            LblSerialNum.Text = "C044111";
            LblSerialNum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSerialNum.Token = null;
            // 
            // LblProductID
            // 
            LblProductID.BackColor = System.Drawing.Color.Empty;
            LblProductID.BorderColor = System.Drawing.Color.Black;
            LblProductID.BorderThickness = 0;
            LblProductID.CornerRadius = 0;
            LblProductID.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblProductID.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblProductID.HighlightPrompt = defaultHighlightPrompt11;
            LblProductID.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblProductID.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblProductID.Location = new System.Drawing.Point(151, 60);
            LblProductID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblProductID.MultyLineFlag = false;
            LblProductID.Name = "LblProductID";
            LblProductID.Size = new System.Drawing.Size(209, 30);
            LblProductID.StyleFlags = UserControls.Style.StyleFlag.None;
            LblProductID.StylizeFlag = true;
            LblProductID.TabIndex = 56;
            LblProductID.TabStop = false;
            LblProductID.Text = "0x359（857）";
            LblProductID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblProductID.Token = null;
            // 
            // LblVendorID
            // 
            LblVendorID.BackColor = System.Drawing.Color.Empty;
            LblVendorID.BorderColor = System.Drawing.Color.Black;
            LblVendorID.BorderThickness = 0;
            LblVendorID.CornerRadius = 0;
            LblVendorID.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblVendorID.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblVendorID.HighlightPrompt = defaultHighlightPrompt12;
            LblVendorID.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblVendorID.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblVendorID.Location = new System.Drawing.Point(151, 22);
            LblVendorID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblVendorID.MultyLineFlag = false;
            LblVendorID.Name = "LblVendorID";
            LblVendorID.Size = new System.Drawing.Size(209, 30);
            LblVendorID.StyleFlags = UserControls.Style.StyleFlag.None;
            LblVendorID.StylizeFlag = true;
            LblVendorID.TabIndex = 55;
            LblVendorID.TabStop = false;
            LblVendorID.Text = "0x1618（5656）";
            LblVendorID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblVendorID.Token = null;
            // 
            // LblVADDR
            // 
            LblVADDR.BackColor = System.Drawing.Color.Empty;
            LblVADDR.BorderColor = System.Drawing.Color.Black;
            LblVADDR.BorderThickness = 0;
            LblVADDR.CornerRadius = 0;
            LblVADDR.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblVADDR.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblVADDR.HighlightPrompt = defaultHighlightPrompt13;
            LblVADDR.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblVADDR.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblVADDR.Location = new System.Drawing.Point(16, 136);
            LblVADDR.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblVADDR.MultyLineFlag = false;
            LblVADDR.Name = "LblVADDR";
            LblVADDR.Size = new System.Drawing.Size(97, 30);
            LblVADDR.StyleFlags = UserControls.Style.StyleFlag.None;
            LblVADDR.StylizeFlag = true;
            LblVADDR.TabIndex = 54;
            LblVADDR.TabStop = false;
            LblVADDR.Text = "Visa Address";
            LblVADDR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblVADDR.Token = null;
            LblVADDR.Visible = false;
            // 
            // LblSN
            // 
            LblSN.BackColor = System.Drawing.Color.Empty;
            LblSN.BorderColor = System.Drawing.Color.Black;
            LblSN.BorderThickness = 0;
            LblSN.CornerRadius = 0;
            LblSN.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSN.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblSN.HighlightPrompt = defaultHighlightPrompt14;
            LblSN.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblSN.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSN.Location = new System.Drawing.Point(16, 98);
            LblSN.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblSN.MultyLineFlag = false;
            LblSN.Name = "LblSN";
            LblSN.Size = new System.Drawing.Size(97, 30);
            LblSN.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSN.StylizeFlag = true;
            LblSN.TabIndex = 53;
            LblSN.TabStop = false;
            LblSN.Text = "SN";
            LblSN.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblSN.Token = null;
            // 
            // LblPID
            // 
            LblPID.BackColor = System.Drawing.Color.Empty;
            LblPID.BorderColor = System.Drawing.Color.Black;
            LblPID.BorderThickness = 0;
            LblPID.CornerRadius = 0;
            LblPID.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblPID.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblPID.HighlightPrompt = defaultHighlightPrompt15;
            LblPID.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblPID.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblPID.Location = new System.Drawing.Point(16, 60);
            LblPID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblPID.MultyLineFlag = false;
            LblPID.Name = "LblPID";
            LblPID.Size = new System.Drawing.Size(97, 30);
            LblPID.StyleFlags = UserControls.Style.StyleFlag.None;
            LblPID.StylizeFlag = true;
            LblPID.TabIndex = 52;
            LblPID.TabStop = false;
            LblPID.Text = "PID";
            LblPID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblPID.Token = null;
            // 
            // LblVID
            // 
            LblVID.BackColor = System.Drawing.Color.Empty;
            LblVID.BorderColor = System.Drawing.Color.Black;
            LblVID.BorderThickness = 0;
            LblVID.CornerRadius = 0;
            LblVID.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblVID.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblVID.HighlightPrompt = defaultHighlightPrompt16;
            LblVID.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblVID.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblVID.Location = new System.Drawing.Point(16, 22);
            LblVID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblVID.MultyLineFlag = false;
            LblVID.Name = "LblVID";
            LblVID.Size = new System.Drawing.Size(97, 30);
            LblVID.StyleFlags = UserControls.Style.StyleFlag.None;
            LblVID.StylizeFlag = true;
            LblVID.TabIndex = 51;
            LblVID.TabStop = false;
            LblVID.Text = "VID";
            LblVID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblVID.Token = null;
            // 
            // TpgWebServer
            // 
            TpgWebServer.BackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            TpgWebServer.Controls.Add(webWaittingLabel);
            TpgWebServer.Controls.Add(ChkWebStatus);
            TpgWebServer.Controls.Add(LblServer);
            TpgWebServer.Controls.Add(BtnWebStatus);
            TpgWebServer.Controls.Add(LblStatus);
            TpgWebServer.Controls.Add(TbxPort);
            TpgWebServer.Controls.Add(LblPort);
            TpgWebServer.Dock = System.Windows.Forms.DockStyle.Fill;
            TpgWebServer.Font = new System.Drawing.Font("MiSans", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TpgWebServer.ForeColor = System.Drawing.Color.White;
            TpgWebServer.HeaderColor = System.Drawing.Color.White;
            TpgWebServer.HeaderFont = new System.Drawing.Font("MiSans", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TpgWebServer.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TpgWebServer.Location = new System.Drawing.Point(1, 35);
            TpgWebServer.Margin = new System.Windows.Forms.Padding(0);
            TpgWebServer.Name = "TpgWebServer";
            TpgWebServer.Size = new System.Drawing.Size(458, 492);
            TpgWebServer.TabIndex = 2;
            TpgWebServer.Text = "WebServer";
            // 
            // ChkWebStatus
            // 
            ChkWebStatus.AnimationCount = 8;
            ChkWebStatus.AnimationFunc = null;
            ChkWebStatus.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkWebStatus.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkWebStatus.BorderThickness = 1;
            ChkWebStatus.Checked = false;
            ChkWebStatus.CheckedBackColor = System.Drawing.Color.FromArgb(18, 183, 245);
            ChkWebStatus.CheckedForeColor = System.Drawing.Color.Black;
            ChkWebStatus.CheckedSliderColor = System.Drawing.Color.Silver;
            ChkWebStatus.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkWebStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkWebStatus.DropKey = System.Windows.Forms.Keys.Space;
            ChkWebStatus.FocusBorderColor = System.Drawing.SystemColors.Control;
            ChkWebStatus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkWebStatus.ForeColor = System.Drawing.Color.White;
            ChkWebStatus.Height = 30;
            ChkWebStatus.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkWebStatus.Location = new System.Drawing.Point(123, 111);
            ChkWebStatus.Margin = new System.Windows.Forms.Padding(0);
            ChkWebStatus.MinimumSize = new System.Drawing.Size(1, 1);
            ChkWebStatus.Name = "ChkWebStatus";
            ChkWebStatus.Size = new System.Drawing.Size(75, 30);
            ChkWebStatus.SliderButtonWidth = 23;
            ChkWebStatus.SliderColor = System.Drawing.Color.Silver;
            ChkWebStatus.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkWebStatus.StylizeFlag = true;
            ChkWebStatus.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkWebStatus.TabIndex = 64;
            ChkWebStatus.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgWebServer.ChkWebStatus"); // "关";
            ChkWebStatus.UseAnimation = false;
            ChkWebStatus.CheckedChangedEvent += ChkWebStatus_CheckedChangedEvent;
            // 
            // LblServer
            // 
            LblServer.BackColor = System.Drawing.Color.Empty;
            LblServer.BorderColor = System.Drawing.Color.Black;
            LblServer.BorderThickness = 0;
            LblServer.CornerRadius = 0;
            LblServer.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblServer.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblServer.HighlightPrompt = defaultHighlightPrompt17;
            LblServer.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblServer.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblServer.Location = new System.Drawing.Point(16, 111);
            LblServer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblServer.MultyLineFlag = false;
            LblServer.Name = "LblServer";
            LblServer.Size = new System.Drawing.Size(71, 30);
            LblServer.StyleFlags = UserControls.Style.StyleFlag.None;
            LblServer.StylizeFlag = true;
            LblServer.TabIndex = 63;
            LblServer.TabStop = false;
            LblServer.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgWebServer.LblServer"); // "服务器";
            LblServer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblServer.Token = null;
            // 
            // BtnWebStatus
            // 
            BtnWebStatus.BackColor = System.Drawing.Color.FromArgb(54, 54, 54);
            BtnWebStatus.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnWebStatus.BorderThickness = 1;
            BtnWebStatus.CornerRadius = 15;
            BtnWebStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnWebStatus.DaskArray = null;
            BtnWebStatus.DropKey = System.Windows.Forms.Keys.Space;
            BtnWebStatus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnWebStatus.ForeColor = System.Drawing.Color.Empty;
            BtnWebStatus.Height = 30;
            BtnWebStatus.Icon = Properties.Resources.OFF;
            BtnWebStatus.IconOffset = 0;
            BtnWebStatus.IconSize = new System.Drawing.Size(20, 20);
            BtnWebStatus.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnWebStatus.IsIndicatorShow = false;
            BtnWebStatus.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnWebStatus.Location = new System.Drawing.Point(123, 22);
            BtnWebStatus.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtnWebStatus.MouseinBackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            BtnWebStatus.MouseinBorderColor = System.Drawing.Color.Empty;
            BtnWebStatus.MouseInBorderThickness = 0;
            BtnWebStatus.MouseinForeColor = System.Drawing.Color.Empty;
            BtnWebStatus.MouseinSvgForeColor = System.Drawing.Color.Empty;
            BtnWebStatus.Name = "BtnWebStatus";
            BtnWebStatus.PressedBackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            BtnWebStatus.PressedBorderColor = System.Drawing.Color.Empty;
            BtnWebStatus.PressedBorderThickness = 0;
            BtnWebStatus.PressedForeColor = System.Drawing.Color.Empty;
            BtnWebStatus.PressedSvgForeColor = System.Drawing.Color.Empty;
            BtnWebStatus.Size = new System.Drawing.Size(20, 30);
            BtnWebStatus.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnWebStatus.StylizeFlag = false;
            BtnWebStatus.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnWebStatus.SVGPath = "";
            BtnWebStatus.TabIndex = 62;
            BtnWebStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LblStatus
            // 
            LblStatus.BackColor = System.Drawing.Color.Empty;
            LblStatus.BorderColor = System.Drawing.Color.Black;
            LblStatus.BorderThickness = 0;
            LblStatus.CornerRadius = 0;
            LblStatus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblStatus.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblStatus.HighlightPrompt = defaultHighlightPrompt18;
            LblStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblStatus.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblStatus.Location = new System.Drawing.Point(16, 22);
            LblStatus.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblStatus.MultyLineFlag = false;
            LblStatus.Name = "LblStatus";
            LblStatus.Size = new System.Drawing.Size(71, 30);
            LblStatus.StyleFlags = UserControls.Style.StyleFlag.None;
            LblStatus.StylizeFlag = true;
            LblStatus.TabIndex = 61;
            LblStatus.TabStop = false;
            LblStatus.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgWebServer.LblStatus"); // "状态";
            LblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblStatus.Token = null;
            // 
            // TbxPort
            // 
            TbxPort.AcceptsTab = false;
            TbxPort.AutoShowKeyBoard = true;
            TbxPort.AutoSize = false;
            TbxPort.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxPort.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxPort.BorderThickness = 0;
            TbxPort.CornerRadius = 0;
            TbxPort.Cursor = System.Windows.Forms.Cursors.IBeam;
            TbxPort.Enabled = true;
            TbxPort.EnbleSelectBorder = true;
            TbxPort.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbxPort.ForeColor = System.Drawing.Color.White;
            TbxPort.Height = 30;
            TbxPort.HideSelection = true;
            TbxPort.KeyboardVerify = null;
            TbxPort.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TbxPort.Lines = new string[] {
        "80" };
            TbxPort.Location = new System.Drawing.Point(124, 62);
            TbxPort.MaxLength = 32767;
            TbxPort.Modified = false;
            TbxPort.MouseEnterState = false;
            TbxPort.Multiline = false;
            TbxPort.Name = "TbxPort";
            TbxPort.ProcessCmdKeyFunc = null;
            TbxPort.ReadOnly = false;
            TbxPort.SelectedColor = System.Drawing.Color.FromArgb(0, 157, 255);
            TbxPort.SelectedText = "";
            TbxPort.SelectionLength = 0;
            TbxPort.SelectionStart = 0;
            TbxPort.ShortcutsEnabled = true;
            TbxPort.Size = new System.Drawing.Size(74, 30);
            TbxPort.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxPort.StylizeFlag = true;
            TbxPort.TabIndex = 56;
            TbxPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            TbxPort.UseSystemPasswordChar = false;
            TbxPort.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            TbxPort.WordWrap = true;
            // 
            // LblPort
            // 
            LblPort.BackColor = System.Drawing.Color.Empty;
            LblPort.BorderColor = System.Drawing.Color.Black;
            LblPort.BorderThickness = 0;
            LblPort.CornerRadius = 0;
            LblPort.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblPort.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblPort.HighlightPrompt = defaultHighlightPrompt19;
            LblPort.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblPort.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblPort.Location = new System.Drawing.Point(16, 62);
            LblPort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LblPort.MultyLineFlag = false;
            LblPort.Name = "LblPort";
            LblPort.Size = new System.Drawing.Size(71, 30);
            LblPort.StyleFlags = UserControls.Style.StyleFlag.None;
            LblPort.StylizeFlag = true;
            LblPort.TabIndex = 55;
            LblPort.TabStop = false;
            LblPort.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.NetWorkPage.TcmNetWork.TpgWebServer.LblPort"); // "端口号";
            LblPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblPort.Token = null;
            // 
            // webWaittingLabel
            // 
            webWaittingLabel.AutoSize = false;
            webWaittingLabel.Image = Properties.Resources.waitting;
            webWaittingLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            webWaittingLabel.Location = new System.Drawing.Point(226, 106);
            webWaittingLabel.Name = "webWaittingLabel";
            webWaittingLabel.Size = new System.Drawing.Size(42, 42);
            webWaittingLabel.TabIndex = 65;
            webWaittingLabel.Text = " ";
            webWaittingLabel.Visible = false;
            // 
            // NetWorkPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            Controls.Add(TcmNetWork);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "NetWorkPage";
            Size = new System.Drawing.Size(539, 493);
            TcmNetWork.ResumeLayout(false);
            TpgNetWork.ResumeLayout(false);
            TpgUSB.ResumeLayout(false);
            TpgWebServer.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private ScopeX.UserControls.TabControlMenu TcmNetWork;
        private ScopeX.UserControls.TabControlMenuPage TpgNetWork;
        private ScopeX.UserControls.ScopeXLabel LblNetWorkCardName;
        private ScopeX.UserControls.ScopeXLabel LblNetWorkStatus;
        private ScopeX.UserControls.UIRadioButtonGroup RdoNetWork;
        private ScopeX.UserControls.ScopeXLabel LblDNSBackup;
        private ScopeX.UserControls.ScopeXIconButton BtnSettingIP;
        private ScopeX.UserControls.ScopeXLabel LblMAC;
        private ScopeX.UserControls.ScopeXLabel LblDNSMain;
        private ScopeX.UserControls.ScopeXLabel LblGateway;
        private ScopeX.UserControls.ScopeXLabel LblIPMask;
        private ScopeX.UserControls.ScopeXLabel LblIPAdress;
        private ScopeX.UserControls.ScopeXLabel LblGetIPAddressMethod;
        private ScopeX.UserControls.ScopeXTextBox TbxMAC;
        private ScopeX.UserControls.TabControlMenuPage TpgUSB;
        private ScopeX.UserControls.ScopeXTextBox TbxVisaAddress;
        private ScopeX.UserControls.ScopeXLabel LblSerialNum;
        private ScopeX.UserControls.ScopeXLabel LblProductID;
        private ScopeX.UserControls.ScopeXLabel LblVendorID;
        private ScopeX.UserControls.ScopeXLabel LblVADDR;
        private ScopeX.UserControls.ScopeXLabel LblSN;
        private ScopeX.UserControls.ScopeXLabel LblPID;
        private ScopeX.UserControls.ScopeXLabel LblVID;
        private ScopeX.UserControls.TabControlMenuPage TpgWebServer;
        private ScopeX.UserControls.ScopeXTextBox TbxPort;
        private ScopeX.UserControls.ScopeXLabel LblPort;
        private ScopeX.UserControls.ScopeXSwitchButton ChkWebStatus;
        private ScopeX.UserControls.ScopeXLabel LblServer;
        private ScopeX.UserControls.ScopeXIconButton BtnWebStatus;
        private ScopeX.UserControls.ScopeXLabel LblStatus;
        private ScopeX.UserControls.ComboBoxEx CbxNetworkAdapter;
        private IPTextBox TbxDNSBackup;
        private IPTextBox TbxDNSMain;
        private IPTextBox TbxGateway;
        private IPTextBox TbxIPMask;
        private IPTextBox TbxIPAdress;
        private System.Windows.Forms.Label webWaittingLabel;
        private UserControls.UIRadioButtonGroup RdoGetIpMethod;
    }
}
