
namespace ScopeX.U2
{
    partial class TriggerSustainTimeSubPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TriggerSustainTimeSubPage));
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new UserControls.DefaultHighlightPrompt();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle1 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle1 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle2 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle3 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle2 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle4 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle5 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle6 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new UserControls.DefaultHighlightPrompt();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle3 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle7 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle8 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle9 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle4 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle10 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle11 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle12 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new UserControls.DefaultHighlightPrompt();
            CbxCondition = new UserControls.SelectComboBox();
            LblCondition = new UserControls.ScopeXLabel();
            BtnSetting = new UserControls.ScopeXIconButton();
            LblSetting = new UserControls.ScopeXLabel();
            NebLowerWidth = new UserControls.TouchNeb();
            LblLowerWidth = new UserControls.ScopeXLabel();
            NebUpperWidth = new UserControls.TouchNeb();
            LblUpperWidth = new UserControls.ScopeXLabel();
            SuspendLayout();
            // 
            // CbxCondition
            // 
            CbxCondition.AutoSize = true;
            CbxCondition.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxCondition.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxCondition.ComBorderColor = System.Drawing.Color.Blue;
            CbxCondition.DataSource = (System.Collections.IList)resources.GetObject("CbxCondition.DataSource");
            CbxCondition.ExtText = "";
            CbxCondition.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxCondition.ForeColor = System.Drawing.Color.White;
            CbxCondition.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            CbxCondition.Items = new string[]
    {
    ">",
    "<",
    "[ ... ]"
    };
            CbxCondition.Location = new System.Drawing.Point(290, 40);
            CbxCondition.MaximumSize = new System.Drawing.Size(9999, 9999);
            CbxCondition.Name = "CbxCondition";
            CbxCondition.SelectIndex = 0;
            CbxCondition.SelectValue = 0;
            CbxCondition.Size = new System.Drawing.Size(85, 30);
            CbxCondition.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxCondition.StylizeFlag = true;
            CbxCondition.TabIndex = 22;
            CbxCondition.SelectedIndexChanged += CbxCondition_SelectedIndexChanged;
            // 
            // LblCondition
            // 
            LblCondition.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblCondition.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblCondition.BorderThickness = 0;
            LblCondition.CornerRadius = 0;
            LblCondition.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblCondition.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblCondition.HighlightPrompt = defaultHighlightPrompt1;
            LblCondition.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblCondition.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblCondition.Location = new System.Drawing.Point(290, 12);
            LblCondition.MultyLineFlag = false;
            LblCondition.Name = "LblCondition";
            LblCondition.Size = new System.Drawing.Size(90, 18);
            LblCondition.StyleFlags = UserControls.Style.StyleFlag.None;
            LblCondition.StylizeFlag = true;
            LblCondition.TabIndex = 9;
            LblCondition.TabStop = false;
            LblCondition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblCondition.Token = null;
            // 
            // BtnSetting
            // 
            BtnSetting.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSetting.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSetting.BorderThickness = 0;
            BtnSetting.CornerRadius = 0;
            BtnSetting.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSetting.DaskArray = null;
            BtnSetting.DropKey = System.Windows.Forms.Keys.Space;
            BtnSetting.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnSetting.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnSetting.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSetting.Height = 30;
            BtnSetting.Icon = null;
            BtnSetting.IconOffset = 10;
            BtnSetting.IconSize = new System.Drawing.Size(24, 24);
            BtnSetting.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSetting.IsChoosed = false;
            BtnSetting.IsIndicatorShow = false;
            BtnSetting.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSetting.Location = new System.Drawing.Point(10, 40);
            BtnSetting.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSetting.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSetting.MouseInBorderThickness = 0;
            BtnSetting.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSetting.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSetting.Name = "BtnSetting";
            BtnSetting.PressedBackColor = System.Drawing.Color.Gray;
            BtnSetting.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSetting.PressedBorderThickness = 0;
            BtnSetting.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSetting.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSetting.Size = new System.Drawing.Size(180, 30);
            BtnSetting.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnSetting.StylizeFlag = true;
            BtnSetting.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSetting.SVGPath = "";
            BtnSetting.TabIndex = 12;
            BtnSetting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSetting.Click += BtnSetting_Click;
            // 
            // LblSetting
            // 
            LblSetting.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblSetting.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblSetting.BorderThickness = 0;
            LblSetting.CornerRadius = 0;
            LblSetting.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSetting.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblSetting.HighlightPrompt = defaultHighlightPrompt2;
            LblSetting.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblSetting.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSetting.Location = new System.Drawing.Point(10, 12);
            LblSetting.MultyLineFlag = false;
            LblSetting.Name = "LblSetting";
            LblSetting.Size = new System.Drawing.Size(180, 18);
            LblSetting.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSetting.StylizeFlag = true;
            LblSetting.TabIndex = 11;
            LblSetting.TabStop = false;
            LblSetting.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSetting.Token = null;
            LblSetting.Visible = false;
            // 
            // NebLowerWidth
            // 
            NebLowerWidth.AddButtonImg = null;
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
            NebLowerWidth.AddButtonStyle = buttonStyle1;
            NebLowerWidth.AllwaysShowFocusImage = false;
            NebLowerWidth.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebLowerWidth.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebLowerWidth.BorderThickness = 0;
            NebLowerWidth.ClickedBorderColor = System.Drawing.Color.White;
            NebLowerWidth.DisableHoldOnInput = false;
            NebLowerWidth.DropKey = System.Windows.Forms.Keys.Space;
            NebLowerWidth.FocusBoederColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebLowerWidth.FocusForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            NebLowerWidth.FocusImage = null;
            NebLowerWidth.FocusImagePosition = UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebLowerWidth.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            NebLowerWidth.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            NebLowerWidth.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            NebLowerWidth.Height = 29;
            NebLowerWidth.HoldOnSpeedLevel = 10;
            NebLowerWidth.IconWidthProportion = 1F;
            NebLowerWidth.Interval = 0.1D;
            NebLowerWidth.IsFocusClicked = false;
            NebLowerWidth.LanguageKey = null;
            NebLowerWidth.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebLowerWidth.Location = new System.Drawing.Point(12, 130);
            NebLowerWidth.MaxValue = double.MaxValue;
            NebLowerWidth.MinValue = double.MinValue;
            NebLowerWidth.Name = "NebLowerWidth";
            NebLowerWidth.Size = new System.Drawing.Size(238, 29);
            NebLowerWidth.StringFormatFunc = null;
            NebLowerWidth.StyleFlags = UserControls.Style.StyleFlag.None;
            NebLowerWidth.StylizeFlag = true;
            NebLowerWidth.SubButtonImg = null;
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
            NebLowerWidth.SubButtonStyle = buttonStyle2;
            NebLowerWidth.TabIndex = 15;
            NebLowerWidth.Text = "0mV";
            NebLowerWidth.Value = 0D;
            // 
            // LblLowerWidth
            // 
            LblLowerWidth.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblLowerWidth.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblLowerWidth.BorderThickness = 0;
            LblLowerWidth.CornerRadius = 0;
            LblLowerWidth.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblLowerWidth.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblLowerWidth.HighlightPrompt = defaultHighlightPrompt3;
            LblLowerWidth.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblLowerWidth.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblLowerWidth.Location = new System.Drawing.Point(12, 102);
            LblLowerWidth.MultyLineFlag = false;
            LblLowerWidth.Name = "LblLowerWidth";
            LblLowerWidth.Size = new System.Drawing.Size(220, 18);
            LblLowerWidth.StyleFlags = UserControls.Style.StyleFlag.None;
            LblLowerWidth.StylizeFlag = true;
            LblLowerWidth.TabIndex = 14;
            LblLowerWidth.TabStop = false;
            LblLowerWidth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblLowerWidth.Token = null;
            // 
            // NebUpperWidth
            // 
            NebUpperWidth.AddButtonImg = null;
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
            NebUpperWidth.AddButtonStyle = buttonStyle3;
            NebUpperWidth.AllwaysShowFocusImage = false;
            NebUpperWidth.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebUpperWidth.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebUpperWidth.BorderThickness = 0;
            NebUpperWidth.ClickedBorderColor = System.Drawing.Color.White;
            NebUpperWidth.DisableHoldOnInput = false;
            NebUpperWidth.DropKey = System.Windows.Forms.Keys.Space;
            NebUpperWidth.FocusBoederColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebUpperWidth.FocusForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            NebUpperWidth.FocusImage = null;
            NebUpperWidth.FocusImagePosition = UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebUpperWidth.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            NebUpperWidth.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            NebUpperWidth.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            NebUpperWidth.Height = 29;
            NebUpperWidth.HoldOnSpeedLevel = 10;
            NebUpperWidth.IconWidthProportion = 1F;
            NebUpperWidth.Interval = 0.1D;
            NebUpperWidth.IsFocusClicked = false;
            NebUpperWidth.LanguageKey = null;
            NebUpperWidth.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebUpperWidth.Location = new System.Drawing.Point(285, 130);
            NebUpperWidth.MaxValue = double.MaxValue;
            NebUpperWidth.MinValue = double.MinValue;
            NebUpperWidth.Name = "NebUpperWidth";
            NebUpperWidth.Size = new System.Drawing.Size(238, 29);
            NebUpperWidth.StringFormatFunc = null;
            NebUpperWidth.StyleFlags = UserControls.Style.StyleFlag.None;
            NebUpperWidth.StylizeFlag = true;
            NebUpperWidth.SubButtonImg = null;
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
            NebUpperWidth.SubButtonStyle = buttonStyle4;
            NebUpperWidth.TabIndex = 21;
            NebUpperWidth.Text = "0mV";
            NebUpperWidth.Value = 0D;
            // 
            // LblUpperWidth
            // 
            LblUpperWidth.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblUpperWidth.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblUpperWidth.BorderThickness = 0;
            LblUpperWidth.CornerRadius = 0;
            LblUpperWidth.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblUpperWidth.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblUpperWidth.HighlightPrompt = defaultHighlightPrompt4;
            LblUpperWidth.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblUpperWidth.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblUpperWidth.Location = new System.Drawing.Point(285, 102);
            LblUpperWidth.MultyLineFlag = false;
            LblUpperWidth.Name = "LblUpperWidth";
            LblUpperWidth.Size = new System.Drawing.Size(220, 18);
            LblUpperWidth.StyleFlags = UserControls.Style.StyleFlag.None;
            LblUpperWidth.StylizeFlag = true;
            LblUpperWidth.TabIndex = 20;
            LblUpperWidth.TabStop = false;
            LblUpperWidth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblUpperWidth.Token = null;
            // 
            // TriggerSustainTimeSubPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(NebUpperWidth);
            Controls.Add(LblUpperWidth);
            Controls.Add(NebLowerWidth);
            Controls.Add(LblLowerWidth);
            Controls.Add(BtnSetting);
            Controls.Add(LblSetting);
            Controls.Add(CbxCondition);
            Controls.Add(LblCondition);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Margin = new System.Windows.Forms.Padding(0);
            Name = "TriggerSustainTimeSubPage";
            Size = new System.Drawing.Size(550, 250);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        //private ScopeX.UserControls.ComboBoxEx CbxCondition;
        private ScopeX.UserControls.ScopeXLabel LblCondition;
        private ScopeX.UserControls.ScopeXIconButton BtnSetting;
        private ScopeX.UserControls.ScopeXLabel LblSetting;
        private ScopeX.UserControls.TouchNeb NebLowerWidth;
        private ScopeX.UserControls.ScopeXLabel LblLowerWidth;
        private ScopeX.UserControls.TouchNeb NebUpperWidth;
        private ScopeX.UserControls.ScopeXLabel LblUpperWidth;
        private ScopeX.UserControls.SelectComboBox CbxCondition;
    }
}