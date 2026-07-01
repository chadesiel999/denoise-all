namespace ScopeX.U2
{
    partial class TempControlForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle7 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle19 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle20 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle21 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle8 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle22 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle23 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle24 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt14 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt15 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt16 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt13 = new ScopeX.UserControls.DefaultHighlightPrompt();
            TlpMain = new System.Windows.Forms.TableLayoutPanel();
            PlFansCtrl = new System.Windows.Forms.Panel();
            NebFanSpeed = new ScopeX.UserControls.TouchNeb();
            LblFanSpeed = new ScopeX.UserControls.ScopeXLabel();
            CbxFansName = new ScopeX.UserControls.ComboBoxEx();
            LblFanName = new ScopeX.UserControls.ScopeXLabel();
            LblAutoCfgFanEnable = new ScopeX.UserControls.ScopeXLabel();
            ChkAutoCtrlFansEnable = new ScopeX.UserControls.ScopeXSwitchButton();
            PlTempInfo = new System.Windows.Forms.Panel();
            ScTempInfo = new ScopeX.UserControls.ScopeXScrollContainer();
            LvTempInfo = new ScopeX.UserControls.ScopeXListViewEx();
            Index = new System.Windows.Forms.ColumnHeader();
            Parameter = new System.Windows.Forms.ColumnHeader();
            Temp = new System.Windows.Forms.ColumnHeader();
            TimerUpdateTemp = new System.Windows.Forms.Timer(components);
            LblAutoCali = new ScopeX.UserControls.ScopeXLabel();
            ChkAutoCali = new ScopeX.UserControls.ScopeXSwitchButton();
            TlpMain.SuspendLayout();
            PlFansCtrl.SuspendLayout();
            PlTempInfo.SuspendLayout();
            ScTempInfo.SuspendLayout();
            SuspendLayout();
            // 
            // TlpMain
            // 
            TlpMain.ColumnCount = 1;
            //TlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            TlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            //TlpMain.Controls.Add(PlFansCtrl, 0, 0);
            TlpMain.Controls.Add(PlTempInfo, 0, 0);
            TlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpMain.Location = new System.Drawing.Point(0, 45);
            TlpMain.Name = "TlpMain";
            TlpMain.RowCount = 1;
            TlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TlpMain.Size = new System.Drawing.Size(550, 352);
            TlpMain.TabIndex = 6;
            // 
            // PlFansCtrl
            // 
            PlFansCtrl.Controls.Add(LblAutoCali);
            PlFansCtrl.Controls.Add(ChkAutoCali);
            PlFansCtrl.Controls.Add(NebFanSpeed);
            PlFansCtrl.Controls.Add(LblFanSpeed);
            PlFansCtrl.Controls.Add(CbxFansName);
            PlFansCtrl.Controls.Add(LblFanName);
            PlFansCtrl.Controls.Add(LblAutoCfgFanEnable);
            PlFansCtrl.Controls.Add(ChkAutoCtrlFansEnable);
            PlFansCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            PlFansCtrl.Location = new System.Drawing.Point(0, 0);
            PlFansCtrl.Margin = new System.Windows.Forms.Padding(0);
            PlFansCtrl.Name = "PlFansCtrl";
            PlFansCtrl.Size = new System.Drawing.Size(230, 352);
            PlFansCtrl.TabIndex = 3;
            // 
            // NebFanSpeed
            // 
            NebFanSpeed.AddButtonImg = null;
            buttonBaseStyle19.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle19.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle19.BorderThickness = 0;
            buttonBaseStyle19.ForeColor = System.Drawing.Color.White;
            buttonStyle7.MouseClickStyle = buttonBaseStyle19;
            buttonBaseStyle20.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle20.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle20.BorderThickness = 0;
            buttonBaseStyle20.ForeColor = System.Drawing.Color.White;
            buttonStyle7.MouseInStyle = buttonBaseStyle20;
            buttonBaseStyle21.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle21.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle21.BorderThickness = 0;
            buttonBaseStyle21.ForeColor = System.Drawing.Color.White;
            buttonStyle7.NomalStyle = buttonBaseStyle21;
            NebFanSpeed.AddButtonStyle = buttonStyle7;
            NebFanSpeed.AllwaysShowFocusImage = false;
            NebFanSpeed.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebFanSpeed.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebFanSpeed.BorderThickness = 0;
            NebFanSpeed.DisableHoldOnInput = false;
            NebFanSpeed.DropKey = System.Windows.Forms.Keys.Space;
            NebFanSpeed.FocusBoederColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebFanSpeed.FocusForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            NebFanSpeed.FocusImage = null;
            NebFanSpeed.FocusImagePosition = ScopeX.UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebFanSpeed.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            NebFanSpeed.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            NebFanSpeed.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            NebFanSpeed.Height = 30;
            NebFanSpeed.HoldOnSpeedLevel = 5;
            NebFanSpeed.IconWidthProportion = 1F;
            NebFanSpeed.Interval = 1D;
            NebFanSpeed.LanguageKey = null;
            NebFanSpeed.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebFanSpeed.Location = new System.Drawing.Point(12, 234);
            NebFanSpeed.MaxValue = 100D;
            NebFanSpeed.MinValue = 0D;
            NebFanSpeed.Name = "NebFanSpeed";
            NebFanSpeed.Size = new System.Drawing.Size(188, 30);
            NebFanSpeed.StringFormatFunc = null;
            NebFanSpeed.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            NebFanSpeed.StylizeFlag = true;
            NebFanSpeed.SubButtonImg = null;
            buttonBaseStyle22.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle22.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle22.BorderThickness = 0;
            buttonBaseStyle22.ForeColor = System.Drawing.Color.White;
            buttonStyle8.MouseClickStyle = buttonBaseStyle22;
            buttonBaseStyle23.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle23.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle23.BorderThickness = 0;
            buttonBaseStyle23.ForeColor = System.Drawing.Color.White;
            buttonStyle8.MouseInStyle = buttonBaseStyle23;
            buttonBaseStyle24.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle24.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle24.BorderThickness = 0;
            buttonBaseStyle24.ForeColor = System.Drawing.Color.White;
            buttonStyle8.NomalStyle = buttonBaseStyle24;
            NebFanSpeed.SubButtonStyle = buttonStyle8;
            NebFanSpeed.TabIndex = 17;
            NebFanSpeed.Value = 0D;
            // 
            // LblFanSpeed
            // 
            LblFanSpeed.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblFanSpeed.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblFanSpeed.BorderThickness = 0;
            LblFanSpeed.CornerRadius = 0;
            LblFanSpeed.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblFanSpeed.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblFanSpeed.HighlightPrompt = defaultHighlightPrompt14;
            LblFanSpeed.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblFanSpeed.Location = new System.Drawing.Point(12, 210);
            LblFanSpeed.MultyLineFlag = false;
            LblFanSpeed.Name = "LblFanSpeed";
            LblFanSpeed.Size = new System.Drawing.Size(188, 18);
            LblFanSpeed.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblFanSpeed.StylizeFlag = true;
            LblFanSpeed.TabIndex = 16;
            LblFanSpeed.Text = "风扇转速";
            LblFanSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblFanSpeed.Token = null;
            // 
            // CbxFansName
            // 
            CbxFansName.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxFansName.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxFansName.BorderThickness = 0;
            CbxFansName.ContainerBackColor = null;
            CbxFansName.ContainerForeColor = null;
            CbxFansName.CornerRadius = 0;
            CbxFansName.DataSource = null;
            CbxFansName.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            CbxFansName.DropDownHeight = 200;
            CbxFansName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CbxFansName.DropDownWidth = 106;
            CbxFansName.DropKey = System.Windows.Forms.Keys.Space;
            CbxFansName.DroppedDown = false;
            CbxFansName.FocusColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxFansName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxFansName.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            CbxFansName.FormattingEnabled = true;
            CbxFansName.GetDisPlayName = null;
            CbxFansName.Height = 30;
            CbxFansName.ImageMode = false;
            CbxFansName.ItemHeight = 28;
            CbxFansName.KeyDropEnble = true;
            CbxFansName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            CbxFansName.Location = new System.Drawing.Point(12, 177);
            CbxFansName.MaxDropDownItems = 8;
            CbxFansName.Name = "CbxFansName";
            CbxFansName.RectBtnWidth = 20;
            CbxFansName.SelectedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            CbxFansName.SelectedIndex = -1;
            CbxFansName.SelectedItem = null;
            CbxFansName.SelectedText = "";
            CbxFansName.Size = new System.Drawing.Size(106, 30);
            CbxFansName.Soreted = false;
            CbxFansName.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            CbxFansName.StylizeFlag = true;
            CbxFansName.TabIndex = 15;
            CbxFansName.Text = "正常";
            CbxFansName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            CbxFansName.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            // 
            // LblFanName
            // 
            LblFanName.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblFanName.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblFanName.BorderThickness = 0;
            LblFanName.CornerRadius = 0;
            LblFanName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblFanName.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblFanName.HighlightPrompt = defaultHighlightPrompt15;
            LblFanName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblFanName.Location = new System.Drawing.Point(12, 153);
            LblFanName.MultyLineFlag = false;
            LblFanName.Name = "LblFanName";
            LblFanName.Size = new System.Drawing.Size(106, 18);
            LblFanName.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblFanName.StylizeFlag = true;
            LblFanName.TabIndex = 14;
            LblFanName.Text = "风扇类型";
            LblFanName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblFanName.Token = null;
            // 
            // LblAutoCfgFanEnable
            // 
            LblAutoCfgFanEnable.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblAutoCfgFanEnable.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblAutoCfgFanEnable.BorderThickness = 0;
            LblAutoCfgFanEnable.CornerRadius = 0;
            LblAutoCfgFanEnable.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblAutoCfgFanEnable.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblAutoCfgFanEnable.HighlightPrompt = defaultHighlightPrompt16;
            LblAutoCfgFanEnable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblAutoCfgFanEnable.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblAutoCfgFanEnable.Location = new System.Drawing.Point(12, 19);
            LblAutoCfgFanEnable.MultyLineFlag = false;
            LblAutoCfgFanEnable.Name = "LblAutoCfgFanEnable";
            LblAutoCfgFanEnable.Size = new System.Drawing.Size(106, 18);
            LblAutoCfgFanEnable.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblAutoCfgFanEnable.StylizeFlag = true;
            LblAutoCfgFanEnable.TabIndex = 12;
            LblAutoCfgFanEnable.TabStop = false;
            LblAutoCfgFanEnable.Text = "风扇转速自动调整";
            LblAutoCfgFanEnable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblAutoCfgFanEnable.Token = null;
            // 
            // ChkAutoCtrlFansEnable
            // 
            ChkAutoCtrlFansEnable.AnimationCount = 8;
            ChkAutoCtrlFansEnable.AnimationFunc = null;
            ChkAutoCtrlFansEnable.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkAutoCtrlFansEnable.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkAutoCtrlFansEnable.BorderThickness = 0;
            ChkAutoCtrlFansEnable.Checked = false;
            ChkAutoCtrlFansEnable.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkAutoCtrlFansEnable.CheckedForeColor = System.Drawing.Color.Black;
            ChkAutoCtrlFansEnable.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkAutoCtrlFansEnable.CheckedText = "开";
            ChkAutoCtrlFansEnable.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkAutoCtrlFansEnable.DropKey = System.Windows.Forms.Keys.Space;
            ChkAutoCtrlFansEnable.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkAutoCtrlFansEnable.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkAutoCtrlFansEnable.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkAutoCtrlFansEnable.Height = 30;
            ChkAutoCtrlFansEnable.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkAutoCtrlFansEnable.Location = new System.Drawing.Point(12, 42);
            ChkAutoCtrlFansEnable.Margin = new System.Windows.Forms.Padding(0);
            ChkAutoCtrlFansEnable.Name = "ChkAutoCtrlFansEnable";
            ChkAutoCtrlFansEnable.Size = new System.Drawing.Size(106, 30);
            ChkAutoCtrlFansEnable.SliderButtonWidth = 30;
            ChkAutoCtrlFansEnable.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkAutoCtrlFansEnable.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            ChkAutoCtrlFansEnable.StylizeFlag = true;
            ChkAutoCtrlFansEnable.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            ChkAutoCtrlFansEnable.TabIndex = 13;
            ChkAutoCtrlFansEnable.Text = "关";
            ChkAutoCtrlFansEnable.UseAnimation = true;
            ChkAutoCtrlFansEnable.Click += ChkAutoCtrlFansEnable_Click;
            // 
            // PlTempInfo
            // 
            PlTempInfo.Controls.Add(ScTempInfo);
            PlTempInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            PlTempInfo.Location = new System.Drawing.Point(233, 3);
            PlTempInfo.Name = "PlTempInfo";
            PlTempInfo.Size = new System.Drawing.Size(314, 346);
            PlTempInfo.TabIndex = 4;
            // 
            // ScTempInfo
            // 
            ScTempInfo.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ScTempInfo.Controls.Add(LvTempInfo);
            ScTempInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            ScTempInfo.Location = new System.Drawing.Point(0, 0);
            ScTempInfo.Name = "ScTempInfo";
            ScTempInfo.ScrollControl = LvTempInfo;
            ScTempInfo.ScrollThickness = 6;
            ScTempInfo.Size = new System.Drawing.Size(314, 346);
            ScTempInfo.TabIndex = 1;
            // 
            // LvTempInfo
            // 
            LvTempInfo.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LvTempInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            LvTempInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { Index, Parameter, Temp });
            LvTempInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            LvTempInfo.ForeColor = System.Drawing.SystemColors.ControlLight;
            LvTempInfo.FullRowSelect = true;
            LvTempInfo.GridLinesColor = System.Drawing.Color.Gray;
            LvTempInfo.HeaderBackColor = System.Drawing.Color.Gray;
            LvTempInfo.HeaderForeColor = System.Drawing.Color.White;
            LvTempInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            LvTempInfo.HideSelection = true;
            LvTempInfo.IsIndependentWindow = false;
            LvTempInfo.Location = new System.Drawing.Point(0, 0);
            LvTempInfo.MultiSelect = false;
            LvTempInfo.Name = "LvTempInfo";
            LvTempInfo.OwnerDraw = true;
            LvTempInfo.RowHeight = 30;
            LvTempInfo.ScrollContainer = ScTempInfo;
            LvTempInfo.SelectedRowColor = System.Drawing.Color.Blue;
            LvTempInfo.Size = new System.Drawing.Size(314, 346);
            LvTempInfo.TabIndex = 4;
            LvTempInfo.TabStop = false;
            LvTempInfo.Tag = "";
            LvTempInfo.UseCompatibleStateImageBehavior = false;
            LvTempInfo.View = System.Windows.Forms.View.Details;
            // 
            // Index
            // 
            Index.Text = "";
            Index.Width = 50;
            // 
            // Parameter
            // 
            Parameter.Text = "名称";
            Parameter.Width = 120;
            // 
            // Temp
            // 
            Temp.Text = "温度值";
            Temp.Width = 120;
            // 
            // TimerUpdateTemp
            // 
            TimerUpdateTemp.Interval = 1000;
            TimerUpdateTemp.Tick += TimerUpdateTemp_Tick;
            // 
            // LblAutoCali
            // 
            LblAutoCali.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblAutoCali.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblAutoCali.BorderThickness = 0;
            LblAutoCali.CornerRadius = 0;
            LblAutoCali.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblAutoCali.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblAutoCali.HighlightPrompt = defaultHighlightPrompt13;
            LblAutoCali.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblAutoCali.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblAutoCali.Location = new System.Drawing.Point(12, 87);
            LblAutoCali.MultyLineFlag = false;
            LblAutoCali.Name = "LblAutoCali";
            LblAutoCali.Size = new System.Drawing.Size(106, 18);
            LblAutoCali.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblAutoCali.StylizeFlag = true;
            LblAutoCali.TabIndex = 18;
            LblAutoCali.TabStop = false;
            LblAutoCali.Text = "系统自动校准";
            LblAutoCali.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblAutoCali.Token = null;
            // 
            // ChkAutoCali
            // 
            ChkAutoCali.AnimationCount = 8;
            ChkAutoCali.AnimationFunc = null;
            ChkAutoCali.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkAutoCali.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkAutoCali.BorderThickness = 0;
            ChkAutoCali.Checked = false;
            ChkAutoCali.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkAutoCali.CheckedForeColor = System.Drawing.Color.Black;
            ChkAutoCali.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkAutoCali.CheckedText = "开";
            ChkAutoCali.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkAutoCali.DropKey = System.Windows.Forms.Keys.Space;
            ChkAutoCali.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkAutoCali.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkAutoCali.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkAutoCali.Height = 30;
            ChkAutoCali.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkAutoCali.Location = new System.Drawing.Point(12, 110);
            ChkAutoCali.Margin = new System.Windows.Forms.Padding(0);
            ChkAutoCali.Name = "ChkAutoCali";
            ChkAutoCali.Size = new System.Drawing.Size(106, 30);
            ChkAutoCali.SliderButtonWidth = 30;
            ChkAutoCali.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkAutoCali.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            ChkAutoCali.StylizeFlag = true;
            ChkAutoCali.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            ChkAutoCali.TabIndex = 19;
            ChkAutoCali.Text = "关";
            ChkAutoCali.UseAnimation = true;
            ChkAutoCali.Click += ChkAutoCali_Click;
            // 
            // TempControlForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            ClientSize = new System.Drawing.Size(550, 397);
            ContentBackColor = System.Drawing.Color.FromArgb(39, 41, 48);
            Controls.Add(TlpMain);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            Name = "TempControlForm";
            Text = "温度检测";
            Title = "温度检测";
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormClosed += TempControlForm_FormClosed;
            Load += TempControlForm_Load;
            Controls.SetChildIndex(TlpMain, 0);
            TlpMain.ResumeLayout(false);
            PlFansCtrl.ResumeLayout(false);
            PlTempInfo.ResumeLayout(false);
            ScTempInfo.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel TlpMain;
        private System.Windows.Forms.Panel PlFansCtrl;
        private ScopeX.UserControls.ScopeXLabel LblAutoCfgFanEnable;
        private ScopeX.UserControls.ScopeXSwitchButton ChkAutoCtrlFansEnable;
        private ScopeX.UserControls.ComboBoxEx CbxFansName;
        private ScopeX.UserControls.ScopeXLabel LblFanName;
        private ScopeX.UserControls.TouchNeb NebFanSpeed;
        public ScopeX.UserControls.ScopeXLabel LblFanSpeed;
        private System.Windows.Forms.Timer TimerUpdateTemp;
        private ScopeX.UserControls.ScopeXListViewEx LvTempInfo;
        private System.Windows.Forms.ColumnHeader Index;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Temp;
        private System.Windows.Forms.Panel PlTempInfo;
        private ScopeX.UserControls.ScopeXScrollContainer ScTempInfo;
        private ScopeX.UserControls.ScopeXLabel LblAutoCali;
        private ScopeX.UserControls.ScopeXSwitchButton ChkAutoCali;
    }
}