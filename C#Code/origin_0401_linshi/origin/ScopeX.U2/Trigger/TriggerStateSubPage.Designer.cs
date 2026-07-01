
namespace ScopeX.U2
{
    partial class TriggerStateSubPage
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
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.RadioButtonItem radioButtonItem1 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem2 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new ScopeX.UserControls.DefaultHighlightPrompt();
            LblClkSlope = new ScopeX.UserControls.ScopeXLabel();
            CbxClkSource = new ScopeX.UserControls.ComboBoxEx();
            LblClock = new ScopeX.UserControls.ScopeXLabel();
            LblQulified = new ScopeX.UserControls.ScopeXLabel();
            RdoClkPolarity = new ScopeX.UserControls.UIRadioButtonGroup();
            LblCondition = new ScopeX.UserControls.ScopeXLabel();
            BtnQualified = new ScopeX.UserControls.ScopeXIconButton();
            ChkCondition = new ScopeX.UserControls.ScopeXSwitchButton();
            SuspendLayout();
            // 
            // LblClkSlope
            // 
            LblClkSlope.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblClkSlope.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblClkSlope.BorderThickness = 0;
            LblClkSlope.CornerRadius = 0;
            LblClkSlope.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblClkSlope.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblClkSlope.HighlightPrompt = defaultHighlightPrompt1;
            LblClkSlope.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblClkSlope.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblClkSlope.Location = new System.Drawing.Point(145, 88);
            LblClkSlope.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LblClkSlope.MultyLineFlag = false;
            LblClkSlope.Name = "LblClkSlope";
            LblClkSlope.Size = new System.Drawing.Size(180, 18);
            LblClkSlope.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblClkSlope.StylizeFlag = true;
            LblClkSlope.TabIndex = 6;
            LblClkSlope.TabStop = false;
            LblClkSlope.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiZhongYan");
            LblClkSlope.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblClkSlope.Token = null;
            // 
            // CbxClkSource
            // 
            CbxClkSource.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxClkSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxClkSource.BorderThickness = 0;
            CbxClkSource.CornerRadius = 0;
            CbxClkSource.DataSource = null;
            CbxClkSource.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            CbxClkSource.DropDownHeight = 200;
            CbxClkSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CbxClkSource.DropDownWidth = 85;
            CbxClkSource.DropKey = System.Windows.Forms.Keys.Space;
            CbxClkSource.DroppedDown = false;
            CbxClkSource.FocusColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxClkSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxClkSource.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            CbxClkSource.FormattingEnabled = true;
            CbxClkSource.GetDisPlayName = null;
            CbxClkSource.Height = 30;
            CbxClkSource.ImageMode = false;
            CbxClkSource.ItemHeight = 28;
            CbxClkSource.KeyDropEnble = true;
            CbxClkSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            CbxClkSource.Location = new System.Drawing.Point(10, 112);
            CbxClkSource.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CbxClkSource.MaxDropDownItems = 8;
            CbxClkSource.Name = "CbxClkSource";
            CbxClkSource.RectBtnWidth = 20;
            CbxClkSource.SelectedBackColor = System.Drawing.Color.Blue;
            CbxClkSource.SelectedIndex = -1;
            CbxClkSource.SelectedItem = null;
            CbxClkSource.SelectedText = "";
            CbxClkSource.Size = new System.Drawing.Size(85, 30);
            CbxClkSource.Soreted = false;
            CbxClkSource.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            CbxClkSource.StylizeFlag = true;
            CbxClkSource.TabIndex = 5;
            CbxClkSource.Text = "";
            CbxClkSource.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            CbxClkSource.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            CbxClkSource.SelectedIndexChanged += CbxClkSource_SelectedIndexChanged;
            // 
            // LblClock
            // 
            LblClock.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblClock.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblClock.BorderThickness = 0;
            LblClock.CornerRadius = 0;
            LblClock.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblClock.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblClock.HighlightPrompt = defaultHighlightPrompt2;
            LblClock.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblClock.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblClock.Location = new System.Drawing.Point(10, 88);
            LblClock.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LblClock.MultyLineFlag = false;
            LblClock.Name = "LblClock";
            LblClock.Size = new System.Drawing.Size(85, 18);
            LblClock.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblClock.StylizeFlag = true;
            LblClock.TabIndex = 4;
            LblClock.TabStop = false;
            LblClock.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiZhong");
            LblClock.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblClock.Token = null;
            // 
            // LblQulified
            // 
            LblQulified.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblQulified.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblQulified.BorderThickness = 0;
            LblQulified.CornerRadius = 0;
            LblQulified.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblQulified.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblQulified.HighlightPrompt = defaultHighlightPrompt3;
            LblQulified.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblQulified.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblQulified.Location = new System.Drawing.Point(10, 4);
            LblQulified.MultyLineFlag = false;
            LblQulified.Name = "LblQulified";
            LblQulified.Size = new System.Drawing.Size(180, 18);
            LblQulified.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblQulified.StylizeFlag = true;
            LblQulified.TabIndex = 0;
            LblQulified.TabStop = false;
            LblQulified.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LuoJiZiGeQueRen");
            LblQulified.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblQulified.Token = null;
            // 
            // RdoClkPolarity
            // 
            RdoClkPolarity.BackColor = System.Drawing.Color.Black;
            RdoClkPolarity.BorderColor = System.Drawing.Color.Black;
            RdoClkPolarity.BorderThickness = 0;
            RdoClkPolarity.ButtonBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoClkPolarity.ButtonFont = null;
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShangSheng");
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiaJiang");
            RdoClkPolarity.ButtonItems = (new ScopeX.UserControls.RadioButtonItem[] { radioButtonItem1, radioButtonItem2 });
            RdoClkPolarity.ButtonOffset = 10;
            RdoClkPolarity.ButtonTextColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RdoClkPolarity.ChoosedButtonColor = System.Drawing.Color.FromArgb(0, 157, 255);
            RdoClkPolarity.ChoosedButtonIndex = 0;
            RdoClkPolarity.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoClkPolarity.ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            RdoClkPolarity.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoClkPolarity.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoClkPolarity.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoClkPolarity.Height = 30;
            RdoClkPolarity.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoClkPolarity.Location = new System.Drawing.Point(145, 112);
            RdoClkPolarity.Margin = new System.Windows.Forms.Padding(0);
            RdoClkPolarity.Name = "RdoClkPolarity";
            RdoClkPolarity.Size = new System.Drawing.Size(180, 30);
            RdoClkPolarity.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            RdoClkPolarity.StylizeFlag = true;
            RdoClkPolarity.TabIndex = 7;
            RdoClkPolarity.IndexChanged += RdoClkPolarity_ButtonSelect;
            // 
            // LblCondition
            // 
            LblCondition.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblCondition.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblCondition.BorderThickness = 0;
            LblCondition.CornerRadius = 0;
            LblCondition.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblCondition.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblCondition.HighlightPrompt = defaultHighlightPrompt4;
            LblCondition.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblCondition.Location = new System.Drawing.Point(249, 4);
            LblCondition.MultyLineFlag = false;
            LblCondition.Name = "LblCondition";
            LblCondition.Size = new System.Drawing.Size(85, 18);
            LblCondition.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblCondition.StylizeFlag = true;
            LblCondition.TabIndex = 2;
            LblCondition.TabStop = false;
            LblCondition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DangTiaoJianManZuShi");
            LblCondition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblCondition.Token = null;
            // 
            // BtnQualified
            // 
            BtnQualified.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnQualified.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnQualified.BorderThickness = 0;
            BtnQualified.CornerRadius = 0;
            BtnQualified.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnQualified.DaskArray = null;
            BtnQualified.DropKey = System.Windows.Forms.Keys.Space;
            BtnQualified.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnQualified.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnQualified.Height = 30;
            BtnQualified.Icon = null;
            BtnQualified.IconOffset = 10;
            BtnQualified.IconSize = new System.Drawing.Size(24, 24);
            BtnQualified.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnQualified.IsIndicatorShow = false;
            BtnQualified.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnQualified.Location = new System.Drawing.Point(10, 27);
            BtnQualified.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnQualified.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnQualified.MouseInBorderThickness = 0;
            BtnQualified.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnQualified.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnQualified.Name = "BtnQualified";
            BtnQualified.PressedBackColor = System.Drawing.Color.Gray;
            BtnQualified.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnQualified.PressedBorderThickness = 0;
            BtnQualified.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnQualified.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnQualified.Size = new System.Drawing.Size(180, 30);
            BtnQualified.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnQualified.StylizeFlag = true;
            BtnQualified.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnQualified.SVGPath = "";
            BtnQualified.TabIndex = 1;
            BtnQualified.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DingYiShuRu");
            BtnQualified.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnQualified.Click += BtnDefine_Click;
            // 
            // ChkCondition
            // 
            ChkCondition.AnimationCount = 8;
            ChkCondition.AnimationFunc = null;
            ChkCondition.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkCondition.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkCondition.BorderThickness = 0;
            ChkCondition.Checked = false;
            ChkCondition.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkCondition.CheckedForeColor = System.Drawing.Color.Black;
            ChkCondition.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkCondition.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkCondition.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkCondition.DropKey = System.Windows.Forms.Keys.Space;
            ChkCondition.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkCondition.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkCondition.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkCondition.Height = 30;
            ChkCondition.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkCondition.Location = new System.Drawing.Point(249, 27);
            ChkCondition.Margin = new System.Windows.Forms.Padding(0);
            ChkCondition.Name = "ChkCondition";
            ChkCondition.Size = new System.Drawing.Size(85, 30);
            ChkCondition.SliderButtonWidth = 30;
            ChkCondition.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkCondition.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            ChkCondition.StylizeFlag = true;
            ChkCondition.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            ChkCondition.TabIndex = 3;
            ChkCondition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkCondition.UseAnimation = true;
            ChkCondition.CheckedChangedEvent += ChkMeet_CheckedChanged;
            // 
            // TriggerStateSubPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(ChkCondition);
            Controls.Add(BtnQualified);
            Controls.Add(LblCondition);
            Controls.Add(RdoClkPolarity);
            Controls.Add(LblQulified);
            Controls.Add(LblClock);
            Controls.Add(CbxClkSource);
            Controls.Add(LblClkSlope);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Margin = new System.Windows.Forms.Padding(0);
            Name = "TriggerStateSubPage";
            Size = new System.Drawing.Size(500, 250);
            ResumeLayout(false);
        }

        #endregion
        private ScopeX.UserControls.ScopeXLabel LblClkSlope;
        private ScopeX.UserControls.ScopeXLabel LblClock;
        private ScopeX.UserControls.ComboBoxEx CbxClkSource;
        private ScopeX.UserControls.ScopeXLabel LblQulified;
        private ScopeX.UserControls.UIRadioButtonGroup RdoClkPolarity;
        private ScopeX.UserControls.ScopeXLabel LblCondition;
        private ScopeX.UserControls.ScopeXIconButton BtnQualified;
        private ScopeX.UserControls.ScopeXSwitchButton ChkCondition;
    }
}