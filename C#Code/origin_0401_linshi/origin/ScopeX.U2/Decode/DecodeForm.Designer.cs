
using System.Windows.Forms;
using Vulkan;

namespace ScopeX.U2
{
    partial class DecodeForm
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
            if (_DecodeView != null)
                Presenter?.TryRemoveView(_DecodeView);
            DbcDecode.Content = null;
            if (disposing && (components != null))
            {
                Presenter?.TryRemoveView(this);
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
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new UserControls.DefaultHighlightPrompt();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle1 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle1 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle2 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle3 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle2 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle4 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle5 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle6 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt5 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt6 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt7 = new UserControls.DefaultHighlightPrompt();
            UUVerticalSplitLine uuVerticalSplitLine1;
            LblBusType = new UserControls.ScopeXLabel();
            CbxBusType = new ScopeX.UserControls.SelectComboBox();
            DbcDecode = new DecodeBaseControl();
            LblDecodeType = new UserControls.ScopeXLabel();
            LblDisplay = new UserControls.ScopeXLabel();
            BtnDisplay = new UserControls.ScopeXSwitchButton();
            LblMark = new UserControls.ScopeXLabel();
            LblLabelVisiblity = new UserControls.ScopeXLabel();
            TbxMark = new UserControls.ScopeXTextBox();
            EbxPosition = new UserControls.TouchNeb();
            BtnResetPos = new UserControls.ScopeXIconButton();
            LblPosition = new UserControls.ScopeXLabel();
            TlpDeocode = new System.Windows.Forms.TableLayoutPanel();
            PnlHeader = new System.Windows.Forms.Panel();
            PnlButtom = new System.Windows.Forms.Panel();
            CbxDecodeType = new ScopeX.UserControls.SelectComboBox();
            ChkIndependentWindow = new UserControls.ScopeXSwitchButton();
            ChkLabelVisiblity = new ScopeX.UserControls.ScopeXSwitchButton();
            LblIndependentWindow = new UserControls.ScopeXLabel();
            LblEvent = new UserControls.ScopeXLabel();
            ChkEvent = new UserControls.ScopeXSwitchButton();
            uuVerticalSplitLine1 = new UUVerticalSplitLine(components);
            TlpDeocode.SuspendLayout();
            PnlHeader.SuspendLayout();
            SuspendLayout();
            // 
            // LblBusType
            // 
            LblBusType.BackColor = System.Drawing.Color.Empty;
            LblBusType.BorderColor = System.Drawing.Color.Black;
            LblBusType.BorderThickness = 0;
            LblBusType.CornerRadius = 0;
            LblBusType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblBusType.HighlightPrompt = defaultHighlightPrompt1;
            LblBusType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblBusType.Location = new System.Drawing.Point(10, 10);
            LblBusType.MultyLineFlag = false;
            LblBusType.Name = "LblBusType";
            LblBusType.Size = new System.Drawing.Size(90, 20);
            LblBusType.StyleFlags = UserControls.Style.StyleFlag.None;
            LblBusType.StylizeFlag = true;
            LblBusType.TabIndex = 9;
            LblBusType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XieYiXuanZe");
            LblBusType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblBusType.Token = null;
            // 
            // CbxBusType
            // 
            CbxBusType.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxBusType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxBusType.DataSource = null;
            CbxBusType.ExtText = "";
            CbxBusType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxBusType.ForeColor = System.Drawing.Color.White;
            CbxBusType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            CbxBusType.Location = new System.Drawing.Point(10, 38);
            CbxBusType.MaximumSize = new System.Drawing.Size(99999, 99999);
            CbxBusType.Name = "CbxBusType";
            CbxBusType.SelectIndex = -1;
            CbxBusType.SelectValue = null;
            CbxBusType.Size = new System.Drawing.Size(130, 30);
            CbxBusType.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxBusType.StylizeFlag = true;
            CbxBusType.TabIndex = 20;
            // 
            // uuVerticalSplitLine1
            // 
            uuVerticalSplitLine1.BackColor = System.Drawing.Color.Transparent;
            uuVerticalSplitLine1.DarkColor = System.Drawing.Color.FromArgb(53, 54, 58);
            uuVerticalSplitLine1.LightColor = System.Drawing.Color.White;
            uuVerticalSplitLine1.Location = new System.Drawing.Point(10, 75);
            uuVerticalSplitLine1.Name = "uuVerticalSplitLine1";
            uuVerticalSplitLine1.Size = new System.Drawing.Size(575, 1);
            uuVerticalSplitLine1.TabIndex = 3;
            // 
            // DbcDecode
            // 
            DbcDecode.BackColor = System.Drawing.Color.Transparent;
            DbcDecode.Content = null;
            DbcDecode.Fill = DecodeBaseControl.FillEnum.AutoHeight;
            //DbcDecode.Controls.Add(uuVerticalSplitLine1);
            DbcDecode.Location = new System.Drawing.Point(10, 175);
            DbcDecode.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            DbcDecode.Name = "DbcDecode";
            DbcDecode.Size = new System.Drawing.Size(600, 410);
            DbcDecode.TabIndex = 12;
            DbcDecode.TabStop = false;
            // 
            // LblDecodeType
            // 
            LblDecodeType.BackColor = System.Drawing.Color.Empty;
            LblDecodeType.BorderColor = System.Drawing.Color.Black;
            LblDecodeType.BorderThickness = 0;
            LblDecodeType.CornerRadius = 0;
            LblDecodeType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblDecodeType.HighlightPrompt = defaultHighlightPrompt2;
            LblDecodeType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblDecodeType.Location = new System.Drawing.Point(330, 0);
            LblDecodeType.MultyLineFlag = false;
            LblDecodeType.Name = "LblDecodeType";
            LblDecodeType.Size = new System.Drawing.Size(130, 20);
            LblDecodeType.StyleFlags = UserControls.Style.StyleFlag.None;
            LblDecodeType.StylizeFlag = true;
            LblDecodeType.TabIndex = 9;
            LblDecodeType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShiGeShi");
            LblDecodeType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblDecodeType.Token = null;
            // 
            // LblDisplay
            // 
            LblDisplay.BackColor = System.Drawing.Color.Empty;
            LblDisplay.BorderColor = System.Drawing.Color.Black;
            LblDisplay.BorderThickness = 0;
            LblDisplay.CornerRadius = 0;
            LblDisplay.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblDisplay.HighlightPrompt = defaultHighlightPrompt3;
            LblDisplay.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblDisplay.Location = new System.Drawing.Point(355, 10);
            LblDisplay.MultyLineFlag = false;
            LblDisplay.Name = "LblDisplay";
            LblDisplay.Size = new System.Drawing.Size(80, 20);
            LblDisplay.StyleFlags = UserControls.Style.StyleFlag.None;
            LblDisplay.StylizeFlag = true;
            LblDisplay.TabIndex = 9;
            LblDisplay.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZongXianXianShi");
            LblDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblDisplay.Token = null;
            // 
            // BtnDisplay
            // 
            BtnDisplay.AnimationCount = 8;
            BtnDisplay.AnimationFunc = null;
            BtnDisplay.BackColor = System.Drawing.Color.FromArgb(150, 150, 150);
            BtnDisplay.BorderColor = System.Drawing.Color.Black;
            BtnDisplay.BorderThickness = 1;
            BtnDisplay.Checked = false;
            BtnDisplay.CheckedBackColor = System.Drawing.Color.FromArgb(18, 183, 245);
            BtnDisplay.CheckedForeColor = System.Drawing.Color.Black;
            BtnDisplay.CheckedSliderColor = System.Drawing.Color.Silver;
            BtnDisplay.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            BtnDisplay.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnDisplay.DropKey = System.Windows.Forms.Keys.Space;
            BtnDisplay.FocusBorderColor = System.Drawing.SystemColors.Control;
            BtnDisplay.ForeColor = System.Drawing.SystemColors.ControlText;
            BtnDisplay.Height = 30;
            BtnDisplay.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnDisplay.Location = new System.Drawing.Point(355, 38);
            BtnDisplay.Name = "BtnDisplay";
            BtnDisplay.Size = new System.Drawing.Size(80, 30);
            BtnDisplay.SliderButtonWidth = 24;
            BtnDisplay.SliderColor = System.Drawing.Color.Silver;
            BtnDisplay.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnDisplay.StylizeFlag = true;
            BtnDisplay.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            BtnDisplay.TabIndex = 13;
            BtnDisplay.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            BtnDisplay.UseAnimation = true;
            // 
            // LblMark
            // 
            LblMark.BackColor = System.Drawing.Color.Empty;
            LblMark.BorderColor = System.Drawing.Color.Black;
            LblMark.BorderThickness = 0;
            LblMark.CornerRadius = 0;
            LblMark.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblMark.HighlightPrompt = defaultHighlightPrompt4;
            LblMark.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblMark.Location = new System.Drawing.Point(10, 68);
            LblMark.MultyLineFlag = false;
            LblMark.Name = "LblMark";
            LblMark.Size = new System.Drawing.Size(113, 20);
            LblMark.StyleFlags = UserControls.Style.StyleFlag.None;
            LblMark.StylizeFlag = true;
            LblMark.TabIndex = 9;
            LblMark.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BiaoQian");
            LblMark.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblMark.Token = null;
            // 
            // TbxMark
            // 
            TbxMark.AcceptsTab = false;
            TbxMark.AutoShowKeyBoard = true;
            TbxMark.AutoSize = false;
            TbxMark.BackColor = System.Drawing.SystemColors.ActiveBorder;
            TbxMark.BorderColor = System.Drawing.Color.Black;
            TbxMark.BorderThickness = 1;
            TbxMark.CornerRadius = 0;
            TbxMark.Cursor = System.Windows.Forms.Cursors.IBeam;
            TbxMark.Enabled = true;
            TbxMark.EnbleSelectBorder = false;
            TbxMark.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbxMark.ForeColor = System.Drawing.SystemColors.WindowText;
            TbxMark.Height = 30;
            TbxMark.HideSelection = true;
            TbxMark.KeyboardVerify = null;
            TbxMark.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TbxMark.Lines = new string[]
    {
    "ScopeXTextBox1"
    };
            TbxMark.Location = new System.Drawing.Point(10,96);
            TbxMark.Margin = new System.Windows.Forms.Padding(0);
            TbxMark.MaxLength = 32767;
            TbxMark.Modified = false;
            TbxMark.MouseEnterState = false;
            TbxMark.Multiline = false;
            TbxMark.Name = "TbxMark";
            TbxMark.ProcessCmdKeyFunc = null;
            TbxMark.ReadOnly = false;
            TbxMark.SelectedColor = System.Drawing.Color.FromArgb(18, 183, 245);
            TbxMark.SelectedText = "";
            TbxMark.SelectionLength = 0;
            TbxMark.SelectionStart = 0;
            TbxMark.ShortcutsEnabled = true;
            TbxMark.Size = new System.Drawing.Size(136, 30);
            TbxMark.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxMark.StylizeFlag = true;
            TbxMark.TabIndex = 14;
            TbxMark.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            TbxMark.UseSystemPasswordChar = false;
            TbxMark.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            TbxMark.WordWrap = true;
            TbxMark.TextChanged += TbxMark_TextChanged;
            // 
            // EbxPosition
            // 
            EbxPosition.AddButtonImg = null;
            buttonBaseStyle1.BackColor = System.Drawing.Color.FromArgb(16, 164, 220);
            buttonBaseStyle1.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle1.BorderThickness = 0;
            buttonBaseStyle1.ForeColor = System.Drawing.Color.White;
            buttonStyle1.MouseClickStyle = buttonBaseStyle1;
            buttonBaseStyle2.BackColor = System.Drawing.Color.FromArgb(16, 164, 220);
            buttonBaseStyle2.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle2.BorderThickness = 0;
            buttonBaseStyle2.ForeColor = System.Drawing.Color.White;
            buttonStyle1.MouseInStyle = buttonBaseStyle2;
            buttonBaseStyle3.BackColor = System.Drawing.Color.FromArgb(18, 183, 245);
            buttonBaseStyle3.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle3.BorderThickness = 0;
            buttonBaseStyle3.ForeColor = System.Drawing.Color.White;
            buttonStyle1.NomalStyle = buttonBaseStyle3;
            EbxPosition.AddButtonStyle = buttonStyle1;
            EbxPosition.AllwaysShowFocusImage = false;
            EbxPosition.BorderColor = System.Drawing.Color.Black;
            EbxPosition.BorderThickness = 1;
            EbxPosition.DisableHoldOnInput = false;
            EbxPosition.DropKey = System.Windows.Forms.Keys.Space;
            EbxPosition.FocusBoederColor = System.Drawing.SystemColors.Control;
            EbxPosition.FocusForeColor = System.Drawing.Color.Blue;
            EbxPosition.FocusImage = null;
            EbxPosition.FocusImagePosition = UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            EbxPosition.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            EbxPosition.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            EbxPosition.Height = 30;
            EbxPosition.HoldOnSpeedLevel = 5;
            EbxPosition.IconWidthProportion = 1F;
            EbxPosition.Interval = 0.1D;
            EbxPosition.LanguageKey = null;
            EbxPosition.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            EbxPosition.Location = new System.Drawing.Point(10, 28);
            EbxPosition.Margin = new System.Windows.Forms.Padding(0);
            EbxPosition.MaxValue = double.MaxValue;
            EbxPosition.MinValue = double.MinValue;
            EbxPosition.Name = "EbxPosition";
            EbxPosition.Size = new System.Drawing.Size(213, 30);
            EbxPosition.StringFormatFunc = null;
            EbxPosition.StyleFlags = UserControls.Style.StyleFlag.None;
            EbxPosition.StylizeFlag = true;
            EbxPosition.SubButtonImg = null;
            buttonBaseStyle4.BackColor = System.Drawing.Color.FromArgb(16, 164, 220);
            buttonBaseStyle4.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle4.BorderThickness = 0;
            buttonBaseStyle4.ForeColor = System.Drawing.Color.White;
            buttonStyle2.MouseClickStyle = buttonBaseStyle4;
            buttonBaseStyle5.BackColor = System.Drawing.Color.FromArgb(16, 164, 220);
            buttonBaseStyle5.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle5.BorderThickness = 0;
            buttonBaseStyle5.ForeColor = System.Drawing.Color.White;
            buttonStyle2.MouseInStyle = buttonBaseStyle5;
            buttonBaseStyle6.BackColor = System.Drawing.Color.FromArgb(18, 183, 245);
            buttonBaseStyle6.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle6.BorderThickness = 0;
            buttonBaseStyle6.ForeColor = System.Drawing.Color.White;
            buttonStyle2.NomalStyle = buttonBaseStyle6;
            EbxPosition.SubButtonStyle = buttonStyle2;
            EbxPosition.TabIndex = 16;
            EbxPosition.Value = 0D;
            // 
            // BtnResetPos
            // 
            BtnResetPos.BackColor = System.Drawing.Color.Transparent;
            BtnResetPos.BorderColor = System.Drawing.Color.Black;
            BtnResetPos.BorderThickness = 1;
            BtnResetPos.CornerRadius = 0;
            BtnResetPos.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnResetPos.DaskArray = null;
            BtnResetPos.DropKey = System.Windows.Forms.Keys.Space;
            BtnResetPos.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnResetPos.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnResetPos.Height = 30;
            BtnResetPos.Icon = null;
            BtnResetPos.IconOffset = 10;
            BtnResetPos.IconSize = new System.Drawing.Size(24, 24);
            BtnResetPos.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnResetPos.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnResetPos.IsChoosed = false;
            BtnResetPos.IsIndicatorShow = false;
            BtnResetPos.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnResetPos.Location = new System.Drawing.Point(235, 28);
            BtnResetPos.MouseinBackColor = System.Drawing.Color.Transparent;
            BtnResetPos.MouseinBorderColor = System.Drawing.SystemColors.Control;
            BtnResetPos.MouseInBorderThickness = 1;
            BtnResetPos.MouseinForeColor = System.Drawing.Color.Black;
            BtnResetPos.MouseinSvgForeColor = System.Drawing.Color.Blue;
            BtnResetPos.Name = "BtnResetPos";
            BtnResetPos.PressedBackColor = System.Drawing.Color.Gray;
            BtnResetPos.PressedBorderColor = System.Drawing.SystemColors.Control;
            BtnResetPos.PressedBorderThickness = 1;
            BtnResetPos.PressedForeColor = System.Drawing.Color.Black;
            BtnResetPos.PressedSvgForeColor = System.Drawing.Color.Blue;
            BtnResetPos.Size = new System.Drawing.Size(80, 30);
            BtnResetPos.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnResetPos.StylizeFlag = true;
            BtnResetPos.SVGForeColor = System.Drawing.Color.Black;
            BtnResetPos.SVGPath = "";
            BtnResetPos.TabIndex = 17;
            BtnResetPos.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SheWei0");
            BtnResetPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LblPosition
            // 
            LblPosition.BackColor = System.Drawing.Color.Empty;
            LblPosition.BorderColor = System.Drawing.Color.Black;
            LblPosition.BorderThickness = 0;
            LblPosition.CornerRadius = 0;
            LblPosition.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblPosition.HighlightPrompt = defaultHighlightPrompt5;
            LblPosition.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblPosition.Location = new System.Drawing.Point(10, 0);
            LblPosition.MultyLineFlag = false;
            LblPosition.Name = "LblPosition";
            LblPosition.Size = new System.Drawing.Size(167, 20);
            LblPosition.StyleFlags = UserControls.Style.StyleFlag.None;
            LblPosition.StylizeFlag = true;
            LblPosition.TabIndex = 15;
            LblPosition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WeiYi");
            LblPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblPosition.Token = null;
            // 
            // LblLabelVisiblity
            // 
            LblLabelVisiblity.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblLabelVisiblity.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblLabelVisiblity.BorderThickness = 0;
            LblLabelVisiblity.CornerRadius = 0;
            LblLabelVisiblity.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LabelVisiblity"); // "打开标签";
            LblLabelVisiblity.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblLabelVisiblity.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblLabelVisiblity.HighlightPrompt = defaultHighlightPrompt1;
            LblLabelVisiblity.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblLabelVisiblity.Location = new System.Drawing.Point(153, 68);
            LblLabelVisiblity.MultyLineFlag = false;
            LblLabelVisiblity.Name = "LblUnitSelection";
            LblLabelVisiblity.Size = new System.Drawing.Size(120, 20);
            LblLabelVisiblity.StyleFlags = UserControls.Style.StyleFlag.None;
            LblLabelVisiblity.StylizeFlag = true;
            LblLabelVisiblity.TabIndex = 15;
            LblLabelVisiblity.TabStop = false;
            LblLabelVisiblity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblLabelVisiblity.Token = null;
            // 
            // ChkLabelVisiblity
            // 
            ChkLabelVisiblity.AnimationCount = 8;
            ChkLabelVisiblity.AnimationFunc = null;
            ChkLabelVisiblity.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkLabelVisiblity.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkLabelVisiblity.BorderThickness = 0;
            ChkLabelVisiblity.Checked = false;
            ChkLabelVisiblity.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkLabelVisiblity.CheckedForeColor = System.Drawing.Color.Black;
            ChkLabelVisiblity.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkLabelVisiblity.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkLabelVisiblity.Cursor = Cursors.Hand;
            ChkLabelVisiblity.DropKey = Keys.Space;
            ChkLabelVisiblity.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkLabelVisiblity.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkLabelVisiblity.Height = 30;
            ChkLabelVisiblity.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkLabelVisiblity.Location = new System.Drawing.Point(153, 96);
            ChkLabelVisiblity.Margin = new Padding(0);
            ChkLabelVisiblity.Name = "ChkAmplitude";
            ChkLabelVisiblity.Size = new System.Drawing.Size(75, 30);
            ChkLabelVisiblity.SliderButtonWidth = 30;
            ChkLabelVisiblity.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkLabelVisiblity.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkLabelVisiblity.StylizeFlag = true;
            ChkLabelVisiblity.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkLabelVisiblity.TabIndex = 18;
            ChkLabelVisiblity.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan"); //  "关";
            ChkLabelVisiblity.UseAnimation = true;
            ChkLabelVisiblity.CheckedChangedEvent += ChkLabelVisiblity_CheckedChangedEvent;
            // 
            // TlpDeocode
            // 
            TlpDeocode.ColumnCount = 1;
            TlpDeocode.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TlpDeocode.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TlpDeocode.Controls.Add(PnlHeader, 0, 0);
            TlpDeocode.Controls.Add(DbcDecode, 0, 1);
            TlpDeocode.Controls.Add(PnlButtom, 0, 2);
            TlpDeocode.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpDeocode.Location = new System.Drawing.Point(3, 46);
            TlpDeocode.Name = "TlpDeocode";
            TlpDeocode.RowCount = 3;
            TlpDeocode.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpDeocode.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpDeocode.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpDeocode.Size = new System.Drawing.Size(528, 581);
            TlpDeocode.TabIndex = 19;
            // 
            // PnlHeader
            // 
            PnlHeader.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PnlHeader.BackColor = System.Drawing.Color.Transparent;
            //PnlHeader.Controls.Add(CbxDecodeType);
            PnlHeader.Controls.Add(ChkIndependentWindow);
            PnlHeader.Controls.Add(LblIndependentWindow);
            PnlHeader.Controls.Add(uuVerticalSplitLine1);
            //PnlHeader.Controls.Add(BtnResetPos);
            PnlHeader.Controls.Add(LblEvent);
            PnlHeader.Controls.Add(LblDisplay);
            PnlHeader.Controls.Add(ChkEvent);
            PnlHeader.Controls.Add(BtnDisplay);
            // PnlHeader.Controls.Add(LblMark);
            //PnlHeader.Controls.Add(TbxMark);
            PnlHeader.Controls.Add(LblBusType);
            PnlHeader.Controls.Add(CbxBusType);
            // PnlHeader.Controls.Add(LblDecodeType);
            //PnlHeader.Controls.Add(LblPosition);
            //PnlHeader.Controls.Add(EbxPosition);
            PnlHeader.Location = new System.Drawing.Point(0, 0);
            PnlHeader.Margin = new System.Windows.Forms.Padding(0);
            PnlHeader.Name = "PnlHeader";
            PnlHeader.Size = new System.Drawing.Size(528, 85);
            PnlHeader.TabIndex = 0;
            //
            //PnlButtom
            //
            //PnlButtom.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            PnlButtom.BackColor = System.Drawing.Color.Transparent;
            PnlButtom.Controls.Add(CbxDecodeType);
            PnlButtom.Controls.Add(BtnResetPos);
            PnlButtom.Controls.Add(LblMark);
            PnlButtom.Controls.Add(TbxMark);
            PnlButtom.Controls.Add(LblDecodeType);
            PnlButtom.Controls.Add(LblPosition);
            PnlButtom.Controls.Add(EbxPosition);
            //PnlButtom.Controls.Add(LblLabelVisiblity);
            PnlButtom.Controls.Add(ChkLabelVisiblity);
            PnlButtom.Location = new System.Drawing.Point(0, 420);
            PnlButtom.Margin = new System.Windows.Forms.Padding(0);
            PnlButtom.Name = "PnlButtom";
            PnlButtom.Size = new System.Drawing.Size(594, 140);
            PnlButtom.TabIndex = 0;
            // 
            // CbxDecodeType
            // 
            CbxDecodeType.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxDecodeType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxDecodeType.DataSource = null;
            CbxDecodeType.ExtText = "";
            CbxDecodeType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxDecodeType.ForeColor = System.Drawing.Color.White;
            CbxDecodeType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            CbxDecodeType.Location = new System.Drawing.Point(330, 28);
            CbxDecodeType.MaximumSize = new System.Drawing.Size(99999, 99999);
            CbxDecodeType.Name = "CbxDecodeType";
            CbxDecodeType.SelectIndex = -1;
            CbxDecodeType.SelectValue = null;
            CbxDecodeType.Size = new System.Drawing.Size(130, 30);
            CbxDecodeType.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxDecodeType.StylizeFlag = true;
            CbxDecodeType.TabIndex = 21;
            // 
            // ChkIndependentWindow
            // 
            ChkIndependentWindow.AnimationCount = 8;
            ChkIndependentWindow.AnimationFunc = null;
            ChkIndependentWindow.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkIndependentWindow.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkIndependentWindow.BorderThickness = 1;
            ChkIndependentWindow.Checked = false;
            ChkIndependentWindow.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkIndependentWindow.CheckedForeColor = System.Drawing.Color.Black;
            ChkIndependentWindow.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkIndependentWindow.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkIndependentWindow.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkIndependentWindow.DropKey = System.Windows.Forms.Keys.Space;
            ChkIndependentWindow.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkIndependentWindow.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkIndependentWindow.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkIndependentWindow.Height = 30;
            ChkIndependentWindow.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkIndependentWindow.Location = new System.Drawing.Point(200, 38);
            ChkIndependentWindow.Margin = new System.Windows.Forms.Padding(0);
            ChkIndependentWindow.Name = "ChkIndependentWindow";
            ChkIndependentWindow.Size = new System.Drawing.Size(80, 30);
            ChkIndependentWindow.SliderButtonWidth = 32;
            ChkIndependentWindow.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkIndependentWindow.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkIndependentWindow.StylizeFlag = true;
            ChkIndependentWindow.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkIndependentWindow.TabIndex = 19;
            ChkIndependentWindow.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkIndependentWindow.UseAnimation = true;
            ChkIndependentWindow.CheckedChangedEvent += ChkIndependentWindow_CheckedChangedEvent;
            // 
            // LblIndependentWindow
            // 
            LblIndependentWindow.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblIndependentWindow.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblIndependentWindow.BorderThickness = 0;
            LblIndependentWindow.CornerRadius = 0;
            LblIndependentWindow.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblIndependentWindow.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblIndependentWindow.HighlightPrompt = defaultHighlightPrompt6;
            LblIndependentWindow.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblIndependentWindow.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblIndependentWindow.Location = new System.Drawing.Point(200, 10);
            LblIndependentWindow.MultyLineFlag = false;
            LblIndependentWindow.Name = "LblIndependentWindow";
            LblIndependentWindow.Size = new System.Drawing.Size(90, 22);
            LblIndependentWindow.StyleFlags = UserControls.Style.StyleFlag.None;
            LblIndependentWindow.StylizeFlag = true;
            LblIndependentWindow.TabIndex = 18;
            LblIndependentWindow.TabStop = false;
            LblIndependentWindow.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DuLiChuangKou");
            LblIndependentWindow.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblIndependentWindow.Token = null;
            // 
            // LblEvent
            // 
            LblEvent.BackColor = System.Drawing.Color.Empty;
            LblEvent.BorderColor = System.Drawing.Color.Black;
            LblEvent.BorderThickness = 0;
            LblEvent.CornerRadius = 0;
            LblEvent.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblEvent.HighlightPrompt = defaultHighlightPrompt7;
            LblEvent.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblEvent.Location = new System.Drawing.Point(505, 10);
            LblEvent.MultyLineFlag = false;
            LblEvent.Name = "LblEvent";
            LblEvent.Size = new System.Drawing.Size(80, 20);
            LblEvent.StyleFlags = UserControls.Style.StyleFlag.None;
            LblEvent.StylizeFlag = true;
            LblEvent.TabIndex = 9;
            LblEvent.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiJianLieBiao");
            LblEvent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblEvent.Token = null;
            // 
            // ChkEvent
            // 
            ChkEvent.AnimationCount = 8;
            ChkEvent.AnimationFunc = null;
            ChkEvent.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkEvent.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkEvent.BorderThickness = 1;
            ChkEvent.Checked = false;
            ChkEvent.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkEvent.CheckedForeColor = System.Drawing.Color.Black;
            ChkEvent.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkEvent.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkEvent.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkEvent.DropKey = System.Windows.Forms.Keys.Space;
            ChkEvent.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkEvent.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkEvent.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkEvent.Height = 30;
            ChkEvent.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkEvent.Location = new System.Drawing.Point(505, 38);
            ChkEvent.Margin = new System.Windows.Forms.Padding(0);
            ChkEvent.Name = "ChkIndependentWindow";
            ChkEvent.Size = new System.Drawing.Size(80, 30);
            ChkEvent.SliderButtonWidth = 32;
            ChkEvent.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkEvent.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkEvent.StylizeFlag = true;
            ChkEvent.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkEvent.TabIndex = 19;
            ChkEvent.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkEvent.UseAnimation = true;
            // 
            // DecodeForm
            // 
            ActiveBorderVisiable = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ActiveBorder;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(600, 680);
            ContentBackColor = System.Drawing.SystemColors.ActiveBorder;
            Controls.Add(TlpDeocode);
            ForeColor = System.Drawing.SystemColors.ControlText;
            FormOpacity = 95;
            HelpLabel = "17";
            IsIndicatorShow = true;
            Margin = new System.Windows.Forms.Padding(5);
            Name = "DecodeForm";
            Padding = new System.Windows.Forms.Padding(1);
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZongXian");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZongXian");
            TitleColor = System.Drawing.SystemColors.ActiveBorder;
            //TitleLableHeight = 1;
            Controls.SetChildIndex(TlpDeocode, 0);
            TlpDeocode.ResumeLayout(false);
            PnlHeader.ResumeLayout(false);
            ResumeLayout(false);
        }


        #endregion

        private ScopeX.UserControls.ScopeXLabel LblBusType;
        private ScopeX.UserControls.SelectComboBox CbxBusType;
        private DecodeBaseControl DbcDecode;
        private ScopeX.UserControls.ScopeXLabel LblDecodeType;
        private ScopeX.UserControls.ScopeXLabel LblDisplay;
        private ScopeX.UserControls.ScopeXSwitchButton BtnDisplay;
        private ScopeX.UserControls.ScopeXLabel LblMark;
        private ScopeX.UserControls.ScopeXLabel LblLabelVisiblity;
        private ScopeX.UserControls.ScopeXTextBox TbxMark;
        private ScopeX.UserControls.TouchNeb EbxPosition;
        private ScopeX.UserControls.ScopeXIconButton BtnResetPos;
        private ScopeX.UserControls.ScopeXLabel LblPosition;
        private System.Windows.Forms.TableLayoutPanel TlpDeocode;
        private System.Windows.Forms.Panel PnlHeader;
        private System.Windows.Forms.Panel PnlButtom;
        private ScopeX.UserControls.ScopeXLabel LblEvent;
        private ScopeX.UserControls.ScopeXSwitchButton ChkEvent;
        private ScopeX.UserControls.ScopeXSwitchButton ChkIndependentWindow;
        private ScopeX.UserControls.ScopeXSwitchButton ChkLabelVisiblity;
        private ScopeX.UserControls.ScopeXLabel LblIndependentWindow;
        private ScopeX.UserControls.SelectComboBox CbxDecodeType;
    }
}