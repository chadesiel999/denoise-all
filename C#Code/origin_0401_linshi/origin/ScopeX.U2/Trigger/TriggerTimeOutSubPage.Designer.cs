
namespace ScopeX.U2
{
    partial class TriggerTimeOutSubPage
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
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle3 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle7 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle8 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle9 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle4 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle10 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle11 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle12 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            LblPolarity = new UserControls.ScopeXLabel();
            RdoPolarity = new UserControls.UIRadioButtonGroup();
            LblSource = new UserControls.ScopeXLabel();
            LblPosition = new UserControls.ScopeXLabel();
            CbxSource = new ScopeX.UserControls.SelectComboBox();
            LblDurationps = new UserControls.ScopeXLabel();
            NebPositon = new UserControls.TouchNeb();
            NebDuration = new UserControls.TouchNeb();
            BtnResetPosition = new UserControls.ScopeXIconButton();
            SuspendLayout();
            // 
            // LblPolarity
            // 
            LblPolarity.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblPolarity.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblPolarity.BorderThickness = 0;
            LblPolarity.CornerRadius = 0;
            LblPolarity.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblPolarity.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblPolarity.HighlightPrompt = defaultHighlightPrompt1;
            LblPolarity.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblPolarity.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblPolarity.Location = new System.Drawing.Point(145, 4);
            LblPolarity.MultyLineFlag = false;
            LblPolarity.Name = "LblPolarity";
            LblPolarity.Size = new System.Drawing.Size(275, 21);
            LblPolarity.StyleFlags = UserControls.Style.StyleFlag.None;
            LblPolarity.StylizeFlag = true;
            LblPolarity.TabIndex = 2;
            LblPolarity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblPolarity.Token = null;
            // 
            // RdoPolarity
            // 
            RdoPolarity.BackColor = System.Drawing.Color.Black;
            RdoPolarity.BorderColor = System.Drawing.Color.Black;
            RdoPolarity.BorderThickness = 0;
            RdoPolarity.ButtonBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoPolarity.ButtonFont = null;
            RdoPolarity.ButtonOffset = 10;
            UserControls.RadioButtonItem radioButtonItem1 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem2 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem3 = new UserControls.RadioButtonItem();
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShangShengYan");
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiaJiangYan");
            radioButtonItem3.Icon = null;
            radioButtonItem3.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem3.Tag = null;
            radioButtonItem3.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("RenYiYan");
            RdoPolarity.ButtonItems = (new UserControls.RadioButtonItem[] { radioButtonItem1, radioButtonItem2, radioButtonItem3 });
            RdoPolarity.ButtonTextColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RdoPolarity.ChoosedButtonColor = System.Drawing.Color.FromArgb(0, 157, 255);
            RdoPolarity.ChoosedButtonIndex = 0;
            RdoPolarity.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoPolarity.ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            RdoPolarity.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoPolarity.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoPolarity.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoPolarity.Height = 30;
            RdoPolarity.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoPolarity.Location = new System.Drawing.Point(145, 32);
            RdoPolarity.Margin = new System.Windows.Forms.Padding(0);
            RdoPolarity.Name = "RdoPolarity";
            RdoPolarity.Size = new System.Drawing.Size(171, 30);
            RdoPolarity.StyleFlags = UserControls.Style.StyleFlag.None;
            RdoPolarity.StylizeFlag = true;
            RdoPolarity.TabIndex = 3;
            RdoPolarity.IndexChanged += RdoPolarity_IndexChanged;
            // 
            // LblSource
            // 
            LblSource.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblSource.BorderThickness = 0;
            LblSource.CornerRadius = 0;
            LblSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSource.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblSource.HighlightPrompt = defaultHighlightPrompt2;
            LblSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSource.Location = new System.Drawing.Point(10, 4);
            LblSource.MultyLineFlag = false;
            LblSource.Name = "LblSource";
            LblSource.Size = new System.Drawing.Size(85, 18);
            LblSource.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSource.StylizeFlag = true;
            LblSource.TabIndex = 0;
            LblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSource.Token = null;
            // 
            // LblPosition
            // 
            LblPosition.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblPosition.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblPosition.BorderThickness = 0;
            LblPosition.CornerRadius = 0;
            LblPosition.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblPosition.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblPosition.HighlightPrompt = defaultHighlightPrompt3;
            LblPosition.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblPosition.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblPosition.Location = new System.Drawing.Point(10, 88);
            LblPosition.MultyLineFlag = false;
            LblPosition.Name = "LblPosition";
            LblPosition.Size = new System.Drawing.Size(220, 18);
            LblPosition.StyleFlags = UserControls.Style.StyleFlag.None;
            LblPosition.StylizeFlag = true;
            LblPosition.TabIndex = 4;
            LblPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblPosition.Token = null;
            // 
            // CbxSource
            // 
            CbxSource.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxSource.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxSource.ForeColor = System.Drawing.Color.White;
            CbxSource.Location = new System.Drawing.Point(10, 32);
            CbxSource.Name = "CbxSource";
            CbxSource.Size = new System.Drawing.Size(85, 30);
            CbxSource.TabIndex = 9;
            CbxSource.SelectedIndexChanged += this.CbxSource_SelectedIndexChanged;
            // 
            // LblDurationps
            // 
            LblDurationps.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblDurationps.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblDurationps.BorderThickness = 0;
            LblDurationps.CornerRadius = 0;
            LblDurationps.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblDurationps.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblDurationps.HighlightPrompt = defaultHighlightPrompt4;
            LblDurationps.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblDurationps.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblDurationps.Location = new System.Drawing.Point(10, 169);
            LblDurationps.MultyLineFlag = false;
            LblDurationps.Name = "LblDurationps";
            LblDurationps.Size = new System.Drawing.Size(220, 18);
            LblDurationps.StyleFlags = UserControls.Style.StyleFlag.None;
            LblDurationps.StylizeFlag = true;
            LblDurationps.TabIndex = 7;
            LblDurationps.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblDurationps.Token = null;
            // 
            // NebPositon
            // 
            NebPositon.AddButtonImg = null;
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
            NebPositon.AddButtonStyle = buttonStyle1;
            NebPositon.AllwaysShowFocusImage = false;
            NebPositon.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebPositon.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebPositon.BorderThickness = 0;
            NebPositon.DisableHoldOnInput = false;
            NebPositon.DropKey = System.Windows.Forms.Keys.Space;
            NebPositon.FocusBoederColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebPositon.FocusForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            NebPositon.FocusImage = null;
            NebPositon.FocusImagePosition = UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebPositon.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            NebPositon.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            NebPositon.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            NebPositon.Height = 30;
            NebPositon.HoldOnSpeedLevel = 10;
            NebPositon.IconWidthProportion = 1F;
            NebPositon.Interval = 0.1D;
            NebPositon.LanguageKey = null;
            NebPositon.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebPositon.Location = new System.Drawing.Point(10, 116);
            NebPositon.Name = "NebPositon";
            NebPositon.Size = new System.Drawing.Size(220, 30);
            NebPositon.StringFormatFunc = null;
            NebPositon.StyleFlags = UserControls.Style.StyleFlag.None;
            NebPositon.StylizeFlag = true;
            NebPositon.SubButtonImg = null;
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
            NebPositon.SubButtonStyle = buttonStyle2;
            NebPositon.TabIndex = 5;
            NebPositon.Text = "0mV";
            NebPositon.Value = 0D;
            // 
            // NebDuration
            // 
            NebDuration.AddButtonImg = null;
            buttonBaseStyle7.BackColor = System.Drawing.Color.FromArgb(16, 164, 220);
            buttonBaseStyle7.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle7.BorderThickness = 0;
            buttonBaseStyle7.ForeColor = System.Drawing.Color.White;
            buttonStyle3.MouseClickStyle = buttonBaseStyle7;
            buttonBaseStyle8.BackColor = System.Drawing.Color.FromArgb(16, 164, 220);
            buttonBaseStyle8.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle8.BorderThickness = 0;
            buttonBaseStyle8.ForeColor = System.Drawing.Color.White;
            buttonStyle3.MouseInStyle = buttonBaseStyle8;
            buttonBaseStyle9.BackColor = System.Drawing.Color.FromArgb(18, 183, 245);
            buttonBaseStyle9.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle9.BorderThickness = 0;
            buttonBaseStyle9.ForeColor = System.Drawing.Color.White;
            buttonStyle3.NomalStyle = buttonBaseStyle9;
            NebDuration.AddButtonStyle = buttonStyle3;
            NebDuration.AllwaysShowFocusImage = false;
            NebDuration.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebDuration.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebDuration.BorderThickness = 0;
            NebDuration.DisableHoldOnInput = false;
            NebDuration.DropKey = System.Windows.Forms.Keys.Space;
            NebDuration.FocusBoederColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebDuration.FocusForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            NebDuration.FocusImage = null;
            NebDuration.FocusImagePosition = UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebDuration.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            NebDuration.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            NebDuration.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            NebDuration.Height = 30;
            NebDuration.HoldOnSpeedLevel = 10;
            NebDuration.IconWidthProportion = 1F;
            NebDuration.Interval = 0.1D;
            NebDuration.LanguageKey = null;
            NebDuration.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebDuration.Location = new System.Drawing.Point(10, 197);
            NebDuration.Name = "NebDuration";
            NebDuration.Size = new System.Drawing.Size(220, 30);
            NebDuration.StringFormatFunc = null;
            NebDuration.StyleFlags = UserControls.Style.StyleFlag.None;
            NebDuration.StylizeFlag = true;
            NebDuration.SubButtonImg = null;
            buttonBaseStyle10.BackColor = System.Drawing.Color.FromArgb(16, 164, 220);
            buttonBaseStyle10.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle10.BorderThickness = 0;
            buttonBaseStyle10.ForeColor = System.Drawing.Color.White;
            buttonStyle4.MouseClickStyle = buttonBaseStyle10;
            buttonBaseStyle11.BackColor = System.Drawing.Color.FromArgb(16, 164, 220);
            buttonBaseStyle11.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle11.BorderThickness = 0;
            buttonBaseStyle11.ForeColor = System.Drawing.Color.White;
            buttonStyle4.MouseInStyle = buttonBaseStyle11;
            buttonBaseStyle12.BackColor = System.Drawing.Color.FromArgb(18, 183, 245);
            buttonBaseStyle12.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle12.BorderThickness = 0;
            buttonBaseStyle12.ForeColor = System.Drawing.Color.White;
            buttonStyle4.NomalStyle = buttonBaseStyle12;
            NebDuration.SubButtonStyle = buttonStyle4;
            NebDuration.TabIndex = 8;
            NebDuration.Text = "0mV";
            NebDuration.Value = 0D;
            // 
            // BtnResetPosition
            // 
            BtnResetPosition.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetPosition.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetPosition.BorderThickness = 0;
            BtnResetPosition.CornerRadius = 0;
            BtnResetPosition.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnResetPosition.DaskArray = null;
            BtnResetPosition.DropKey = System.Windows.Forms.Keys.Space;
            BtnResetPosition.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnResetPosition.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetPosition.Height = 30;
            BtnResetPosition.Icon = null;
            BtnResetPosition.IconOffset = 10;
            BtnResetPosition.IconSize = new System.Drawing.Size(24, 24);
            BtnResetPosition.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnResetPosition.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnResetPosition.IsIndicatorShow = false;
            BtnResetPosition.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnResetPosition.Location = new System.Drawing.Point(238, 116);
            BtnResetPosition.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetPosition.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetPosition.MouseInBorderThickness = 0;
            BtnResetPosition.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetPosition.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnResetPosition.Name = "BtnResetPosition";
            BtnResetPosition.PressedBackColor = System.Drawing.Color.Gray;
            BtnResetPosition.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetPosition.PressedBorderThickness = 0;
            BtnResetPosition.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetPosition.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnResetPosition.Size = new System.Drawing.Size(60, 30);
            BtnResetPosition.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnResetPosition.StylizeFlag = true;
            BtnResetPosition.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetPosition.SVGPath = "";
            BtnResetPosition.TabIndex = 6;
            BtnResetPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnResetPosition.Click += BtnResetPosition_Click;
            // 
            // TriggerTimeOutSubPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(BtnResetPosition);
            Controls.Add(NebDuration);
            Controls.Add(NebPositon);
            Controls.Add(LblDurationps);
            Controls.Add(CbxSource);
            Controls.Add(LblPolarity);
            Controls.Add(RdoPolarity);
            Controls.Add(LblSource);
            Controls.Add(LblPosition);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Margin = new System.Windows.Forms.Padding(0);
            Name = "TriggerTimeOutSubPage";
            Size = new System.Drawing.Size(500, 250);
            ResumeLayout(false);
        }

        #endregion
        private ScopeX.UserControls.ScopeXLabel LblPolarity;
        private ScopeX.UserControls.UIRadioButtonGroup RdoPolarity;
        private ScopeX.UserControls.ScopeXLabel LblSource;
        private ScopeX.UserControls.ScopeXLabel LblPosition;
        //private ScopeX.UserControls.ComboBoxEx CbxSource;
        private ScopeX.UserControls.ScopeXLabel LblDurationps;
        private ScopeX.UserControls.TouchNeb NebPositon;
        private ScopeX.UserControls.TouchNeb NebDuration;
        private ScopeX.UserControls.ScopeXIconButton BtnResetPosition;
        private ScopeX.UserControls.SelectComboBox CbxSource;
    }
}