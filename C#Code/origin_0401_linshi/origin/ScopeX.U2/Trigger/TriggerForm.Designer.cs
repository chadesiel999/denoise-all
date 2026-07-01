
namespace ScopeX.U2
{
    partial class TriggerForm
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
            UUVerticalSplitLine uuVerticalSplitLine2;
            UserControls.RadioButtonItem radioButtonItem1 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem2 = new UserControls.RadioButtonItem();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new UserControls.DefaultHighlightPrompt();
            UserControls.RadioButtonItem radioButtonItem3 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem4 = new UserControls.RadioButtonItem();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle1 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle1 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle2 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle3 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle2 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle4 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle5 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle6 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new UserControls.DefaultHighlightPrompt();
            TlpTrigger = new System.Windows.Forms.TableLayoutPanel();
            PnlTriggerType = new System.Windows.Forms.Panel();
            BtnForce = new UserControls.ScopeXIconButton();
            RdoMode = new UserControls.UIRadioButtonGroup();
            LblMode = new UserControls.ScopeXLabel();
            CbxTriggerType = new UserControls.SelectComboBox();
            LblTriggerType = new UserControls.ScopeXLabel();
            PnlAssisted = new System.Windows.Forms.Panel();
            RdoHoldoffType = new UserControls.UIRadioButtonGroup();
            NebHoldoff = new UserControls.TouchNeb();
            LblHoldoff = new UserControls.ScopeXLabel();
            BtnResetHoldff = new UserControls.ScopeXIconButton();
            uuVerticalSplitLine2 = new UUVerticalSplitLine(components);
            TlpTrigger.SuspendLayout();
            PnlTriggerType.SuspendLayout();
            PnlAssisted.SuspendLayout();
            SuspendLayout();
            // 
            // uuVerticalSplitLine2
            // 
            uuVerticalSplitLine2.BackColor = System.Drawing.Color.Transparent;
            uuVerticalSplitLine2.DarkColor = System.Drawing.Color.FromArgb(53, 54, 58);
            uuVerticalSplitLine2.LightColor = System.Drawing.Color.White;
            uuVerticalSplitLine2.Location = new System.Drawing.Point(294, 15);
            uuVerticalSplitLine2.Name = "uuVerticalSplitLine2";
            uuVerticalSplitLine2.Size = new System.Drawing.Size(1, 88);
            uuVerticalSplitLine2.TabIndex = 42;
            uuVerticalSplitLine2.Visible = false;
            // 
            // TlpTrigger
            // 
            TlpTrigger.ColumnCount = 1;
            TlpTrigger.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpTrigger.Controls.Add(PnlTriggerType, 0, 0);
            TlpTrigger.Controls.Add(PnlAssisted, 0, 2);
            TlpTrigger.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpTrigger.Location = new System.Drawing.Point(2, 45);
            TlpTrigger.Name = "TlpTrigger";
            TlpTrigger.RowCount = 3;
            TlpTrigger.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            TlpTrigger.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpTrigger.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            TlpTrigger.Size = new System.Drawing.Size(535, 541);
            TlpTrigger.TabIndex = 0;
            // 
            // PnlTriggerType
            // 
            PnlTriggerType.Controls.Add(BtnForce);
            PnlTriggerType.Controls.Add(RdoMode);
            PnlTriggerType.Controls.Add(LblMode);
            PnlTriggerType.Controls.Add(CbxTriggerType);
            PnlTriggerType.Controls.Add(LblTriggerType);
            PnlTriggerType.Dock = System.Windows.Forms.DockStyle.Fill;
            PnlTriggerType.Location = new System.Drawing.Point(0, 0);
            PnlTriggerType.Margin = new System.Windows.Forms.Padding(0);
            PnlTriggerType.Name = "PnlTriggerType";
            PnlTriggerType.Size = new System.Drawing.Size(535, 60);
            PnlTriggerType.TabIndex = 1;
            // 
            // BtnForce
            // 
            BtnForce.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnForce.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnForce.BorderThickness = 0;
            BtnForce.CornerRadius = 0;
            BtnForce.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnForce.DaskArray = null;
            BtnForce.DropKey = System.Windows.Forms.Keys.Space;
            BtnForce.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnForce.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnForce.Height = 30;
            BtnForce.Icon = null;
            BtnForce.IconOffset = 10;
            BtnForce.IconSize = new System.Drawing.Size(24, 24);
            BtnForce.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnForce.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnForce.IsChoosed = false;
            BtnForce.IsIndicatorShow = false;
            BtnForce.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnForce.Location = new System.Drawing.Point(165, 32);
            BtnForce.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnForce.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnForce.MouseInBorderThickness = 0;
            BtnForce.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnForce.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnForce.Name = "BtnForce";
            BtnForce.PressedBackColor = System.Drawing.Color.Gray;
            BtnForce.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnForce.PressedBorderThickness = 0;
            BtnForce.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnForce.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnForce.Size = new System.Drawing.Size(68, 30);
            BtnForce.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnForce.StylizeFlag = true;
            BtnForce.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnForce.SVGPath = "";
            BtnForce.TabIndex = 10;
            BtnForce.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnForce.Click += BtnForce_Click;
            // 
            // RdoMode
            // 
            RdoMode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            RdoMode.BackColor = System.Drawing.Color.Black;
            RdoMode.BorderColor = System.Drawing.Color.Black;
            RdoMode.BorderThickness = 0;
            RdoMode.ButtonBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoMode.ButtonFont = null;
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = "buttonItem";
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = "buttonItem";
            RdoMode.ButtonItems = new UserControls.RadioButtonItem[]
    {
    radioButtonItem1,
    radioButtonItem2
    };
            RdoMode.ButtonOffset = 10;
            RdoMode.ButtonTextColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RdoMode.ChoosedButtonColor = System.Drawing.Color.FromArgb(0, 171, 209);
            RdoMode.ChoosedButtonIndex = 0;
            RdoMode.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoMode.ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            RdoMode.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoMode.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoMode.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoMode.Height = 30;
            RdoMode.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoMode.Location = new System.Drawing.Point(280, 32);
            RdoMode.Margin = new System.Windows.Forms.Padding(0);
            RdoMode.Name = "RdoMode";
            RdoMode.Size = new System.Drawing.Size(245, 30);
            RdoMode.StyleFlags = UserControls.Style.StyleFlag.None;
            RdoMode.StylizeFlag = true;
            RdoMode.TabIndex = 1;
            RdoMode.IndexChanged += RdoMode_IndexChanged;
            // 
            // LblMode
            // 
            LblMode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            LblMode.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblMode.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblMode.BorderThickness = 0;
            LblMode.CornerRadius = 0;
            LblMode.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblMode.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblMode.HighlightPrompt = defaultHighlightPrompt1;
            LblMode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblMode.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblMode.Location = new System.Drawing.Point(280, 4);
            LblMode.MultyLineFlag = false;
            LblMode.Name = "LblMode";
            LblMode.Size = new System.Drawing.Size(245, 18);
            LblMode.StyleFlags = UserControls.Style.StyleFlag.None;
            LblMode.StylizeFlag = true;
            LblMode.TabIndex = 0;
            LblMode.TabStop = false;
            LblMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblMode.Token = null;
            // 
            // CbxTriggerType
            // 
            CbxTriggerType.AutoSize = true;
            CbxTriggerType.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxTriggerType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxTriggerType.ComBorderColor = System.Drawing.Color.Blue;
            CbxTriggerType.DataSource = null;
            CbxTriggerType.ExtText = "";
            CbxTriggerType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxTriggerType.ForeColor = System.Drawing.Color.White;
            CbxTriggerType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            CbxTriggerType.Location = new System.Drawing.Point(10, 32);
            CbxTriggerType.MaximumSize = new System.Drawing.Size(9999, 9999);
            CbxTriggerType.Name = "CbxTriggerType";
            CbxTriggerType.SelectIndex = -1;
            CbxTriggerType.SelectValue = null;
            CbxTriggerType.Size = new System.Drawing.Size(144, 30);
            CbxTriggerType.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxTriggerType.StylizeFlag = true;
            CbxTriggerType.TabIndex = 11;
            CbxTriggerType.SelectedIndexChanged += CbxTriggerType_SelectedIndexChanged;
            // 
            // LblTriggerType
            // 
            LblTriggerType.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblTriggerType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblTriggerType.BorderThickness = 0;
            LblTriggerType.CornerRadius = 0;
            LblTriggerType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblTriggerType.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblTriggerType.HighlightPrompt = defaultHighlightPrompt2;
            LblTriggerType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTriggerType.Location = new System.Drawing.Point(10, 4);
            LblTriggerType.MultyLineFlag = false;
            LblTriggerType.Name = "LblTriggerType";
            LblTriggerType.Size = new System.Drawing.Size(100, 21);
            LblTriggerType.StyleFlags = UserControls.Style.StyleFlag.None;
            LblTriggerType.StylizeFlag = true;
            LblTriggerType.TabIndex = 2;
            LblTriggerType.TabStop = false;
            LblTriggerType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblTriggerType.Token = null;
            // 
            // PnlAssisted
            // 
            PnlAssisted.Controls.Add(uuVerticalSplitLine2);
            PnlAssisted.Controls.Add(RdoHoldoffType);
            PnlAssisted.Controls.Add(NebHoldoff);
            PnlAssisted.Controls.Add(LblHoldoff);
            PnlAssisted.Controls.Add(BtnResetHoldff);
            PnlAssisted.Dock = System.Windows.Forms.DockStyle.Top;
            PnlAssisted.Location = new System.Drawing.Point(0, 431);
            PnlAssisted.Margin = new System.Windows.Forms.Padding(0);
            PnlAssisted.Name = "PnlAssisted";
            PnlAssisted.Size = new System.Drawing.Size(535, 120);
            PnlAssisted.TabIndex = 2;
            // 
            // RdoHoldoffType
            // 
            RdoHoldoffType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            RdoHoldoffType.BackColor = System.Drawing.Color.Black;
            RdoHoldoffType.BorderColor = System.Drawing.Color.Black;
            RdoHoldoffType.BorderThickness = 0;
            RdoHoldoffType.ButtonBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoHoldoffType.ButtonFont = null;
            radioButtonItem3.Icon = null;
            radioButtonItem3.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            radioButtonItem3.Tag = null;
            radioButtonItem3.Text = "buttonItem";
            radioButtonItem4.Icon = null;
            radioButtonItem4.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            radioButtonItem4.Tag = null;
            radioButtonItem4.Text = "buttonItem";
            RdoHoldoffType.ButtonItems = new UserControls.RadioButtonItem[]
    {
    radioButtonItem3,
    radioButtonItem4
    };
            RdoHoldoffType.ButtonOffset = 10;
            RdoHoldoffType.ButtonTextColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RdoHoldoffType.ChoosedButtonColor = System.Drawing.Color.FromArgb(0, 171, 209);
            RdoHoldoffType.ChoosedButtonIndex = 0;
            RdoHoldoffType.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoHoldoffType.ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            RdoHoldoffType.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoHoldoffType.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoHoldoffType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoHoldoffType.Height = 30;
            RdoHoldoffType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoHoldoffType.Location = new System.Drawing.Point(41, 80);
            RdoHoldoffType.Margin = new System.Windows.Forms.Padding(0);
            RdoHoldoffType.Name = "RdoHoldoffType";
            RdoHoldoffType.Size = new System.Drawing.Size(160, 30);
            RdoHoldoffType.StyleFlags = UserControls.Style.StyleFlag.None;
            RdoHoldoffType.StylizeFlag = true;
            RdoHoldoffType.TabIndex = 1;
            RdoHoldoffType.Visible = false;
            RdoHoldoffType.IndexChanged += RdoHoldoffType_IndexChanged;
            // 
            // NebHoldoff
            // 
            NebHoldoff.AddButtonImg = null;
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
            NebHoldoff.AddButtonStyle = buttonStyle1;
            NebHoldoff.AllwaysShowFocusImage = false;
            NebHoldoff.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebHoldoff.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebHoldoff.BorderThickness = 0;
            NebHoldoff.DisableHoldOnInput = false;
            NebHoldoff.DropKey = System.Windows.Forms.Keys.Space;
            NebHoldoff.FocusBoederColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebHoldoff.FocusForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            NebHoldoff.FocusImage = null;
            NebHoldoff.FocusImagePosition = UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebHoldoff.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            NebHoldoff.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            NebHoldoff.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            NebHoldoff.Height = 26;
            NebHoldoff.HoldOnSpeedLevel = 10;
            NebHoldoff.IconWidthProportion = 1F;
            NebHoldoff.Interval = 0.1D;
            NebHoldoff.LanguageKey = null;
            NebHoldoff.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebHoldoff.Location = new System.Drawing.Point(10, 43);
            NebHoldoff.MaxValue = double.MaxValue;
            NebHoldoff.MinValue = double.MinValue;
            NebHoldoff.Name = "NebHoldoff";
            NebHoldoff.Size = new System.Drawing.Size(220, 26);
            NebHoldoff.StringFormatFunc = null;
            NebHoldoff.StyleFlags = UserControls.Style.StyleFlag.None;
            NebHoldoff.StylizeFlag = true;
            NebHoldoff.SubButtonImg = null;
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
            NebHoldoff.SubButtonStyle = buttonStyle2;
            NebHoldoff.TabIndex = 5;
            NebHoldoff.Value = 0D;
            NebHoldoff.Visible = false;

            // 
            // LblHoldoff
            // 
            LblHoldoff.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblHoldoff.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblHoldoff.BorderThickness = 0;
            LblHoldoff.CornerRadius = 0;
            LblHoldoff.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblHoldoff.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblHoldoff.HighlightPrompt = defaultHighlightPrompt3;
            LblHoldoff.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblHoldoff.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblHoldoff.Location = new System.Drawing.Point(10, 13);
            LblHoldoff.MultyLineFlag = false;
            LblHoldoff.Name = "LblHoldoff";
            LblHoldoff.Size = new System.Drawing.Size(160, 18);
            LblHoldoff.StyleFlags = UserControls.Style.StyleFlag.None;
            LblHoldoff.StylizeFlag = true;
            LblHoldoff.TabIndex = 4;
            LblHoldoff.TabStop = false;
            LblHoldoff.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblHoldoff.Token = null;
            LblHoldoff.Visible = false;
            // 
            // BtnResetHoldff
            // 
            BtnResetHoldff.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetHoldff.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetHoldff.BorderThickness = 0;
            BtnResetHoldff.CornerRadius = 0;
            BtnResetHoldff.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnResetHoldff.DaskArray = null;
            BtnResetHoldff.DropKey = System.Windows.Forms.Keys.Space;
            BtnResetHoldff.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnResetHoldff.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetHoldff.Height = 26;
            BtnResetHoldff.Icon = null;
            BtnResetHoldff.IconOffset = 10;
            BtnResetHoldff.IconSize = new System.Drawing.Size(24, 24);
            BtnResetHoldff.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnResetHoldff.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnResetHoldff.IsChoosed = false;
            BtnResetHoldff.IsIndicatorShow = false;
            BtnResetHoldff.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnResetHoldff.Location = new System.Drawing.Point(238, 43);
            BtnResetHoldff.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetHoldff.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetHoldff.MouseInBorderThickness = 0;
            BtnResetHoldff.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetHoldff.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnResetHoldff.Name = "BtnResetHoldff";
            BtnResetHoldff.PressedBackColor = System.Drawing.Color.Gray;
            BtnResetHoldff.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetHoldff.PressedBorderThickness = 0;
            BtnResetHoldff.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetHoldff.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnResetHoldff.Size = new System.Drawing.Size(60, 26);
            BtnResetHoldff.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnResetHoldff.StylizeFlag = true;
            BtnResetHoldff.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetHoldff.SVGPath = "";
            BtnResetHoldff.TabIndex = 6;
            BtnResetHoldff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnResetHoldff.Click += BtnResetHoldff_Click;
            BtnResetHoldff.Visible = false;

            // 
            // TriggerForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(539, 588);
            ContentBackColor = System.Drawing.Color.FromArgb(39, 41, 48);
            ControlBox = false;
            Controls.Add(TlpTrigger);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            HelpLabel = "16";
            IconInterval = 21;
            IconWidth = 28;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TriggerForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(28, 26);
            Controls.SetChildIndex(TlpTrigger, 0);
            TlpTrigger.ResumeLayout(false);
            PnlTriggerType.ResumeLayout(false);
            PnlTriggerType.PerformLayout();
            PnlAssisted.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpTrigger;
        private System.Windows.Forms.Panel PnlTriggerType;
        private ScopeX.UserControls.TouchNeb NebHoldoff;
        private ScopeX.UserControls.ScopeXLabel LblHoldoff;
        private ScopeX.UserControls.ScopeXIconButton BtnResetHoldff;
        private ScopeX.UserControls.UIRadioButtonGroup RdoMode;
        private ScopeX.UserControls.ScopeXLabel LblMode;
       // private ScopeX.UserControls.ComboBoxEx CbxTriggerType;
        private ScopeX.UserControls.ScopeXLabel LblTriggerType;
        private System.Windows.Forms.Panel PnlAssisted;
        private ScopeX.UserControls.UIRadioButtonGroup RdoHoldoffType;
        private UUVerticalSplitLine uuVerticalSplitLine2;
        private ScopeX.UserControls.ScopeXSwitchButton CHKF;
        private ScopeX.UserControls.ScopeXSwitchButton ChkF;
        private ScopeX.UserControls.ScopeXIconButton BtnForce;
        private UserControls.SelectComboBox CbxTriggerType;
    }
}