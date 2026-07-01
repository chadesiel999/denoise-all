namespace ScopeX.U2
{
    partial class AuxOutputPage
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
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            UserControls.RadioButtonItem radioButtonItem1 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem2 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem3 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem4 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem5 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem6 = new UserControls.RadioButtonItem();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new UserControls.DefaultHighlightPrompt();
            UserControls.RadioButtonItem radioButtonItem7 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem8 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem9 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem10 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem11 = new UserControls.RadioButtonItem();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt5 = new UserControls.DefaultHighlightPrompt();
            LblAuxOutput = new UserControls.ScopeXLabel();
            RboAuxOutput = new UserControls.UIRadioButtonGroup();
            RboAuxOutPolarity = new UserControls.UIRadioButtonGroup();
            LblAuxOutPolarity = new UserControls.ScopeXLabel();
            LblAuxInput = new UserControls.ScopeXLabel();
            RboAuxInput = new UserControls.UIRadioButtonGroup();
            RboAuxInPolarity = new UserControls.UIRadioButtonGroup();
            LblAuxInPolarity = new UserControls.ScopeXLabel();
            LblAuxInTip = new UserControls.ScopeXLabel();
            SuspendLayout();
            // 
            // LblAuxOutput
            // 
            LblAuxOutput.BackColor = System.Drawing.Color.Empty;
            LblAuxOutput.BorderColor = System.Drawing.Color.Black;
            LblAuxOutput.BorderThickness = 0;
            LblAuxOutput.CornerRadius = 0;
            LblAuxOutput.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblAuxOutput.ForeColor = System.Drawing.Color.FromArgb(234, 234, 234);
            LblAuxOutput.HighlightPrompt = defaultHighlightPrompt1;
            LblAuxOutput.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblAuxOutput.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblAuxOutput.Location = new System.Drawing.Point(12, 191);
            LblAuxOutput.MultyLineFlag = false;
            LblAuxOutput.Name = "LblAuxOutput";
            LblAuxOutput.Size = new System.Drawing.Size(245, 18);
            LblAuxOutput.StyleFlags = UserControls.Style.StyleFlag.None;
            LblAuxOutput.StylizeFlag = true;
            LblAuxOutput.TabIndex = 2;
            LblAuxOutput.TabStop = false;
            LblAuxOutput.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuZhuShuChu");
            LblAuxOutput.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblAuxOutput.Token = null;
            // 
            // RboAuxOutput
            // 
            RboAuxOutput.BackColor = System.Drawing.Color.AliceBlue;
            RboAuxOutput.BorderColor = System.Drawing.Color.AliceBlue;
            RboAuxOutput.BorderThickness = 0;
            RboAuxOutput.ButtonBackColor = System.Drawing.Color.FromArgb(50, 50, 54);
            RboAuxOutput.ButtonFont = null;
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AuxOutputPage.RboAuxOutput.panel1.Btn_0"); //"关闭";
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AuxOutputPage.RboAuxOutput.panel1.Btn_1");//"触发同步";
            radioButtonItem3.Icon = null;
            radioButtonItem3.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem3.Tag = null;
            radioButtonItem3.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AuxOutputPage.RboAuxOutput.panel1.Btn_2");//"AWG触发";
            radioButtonItem4.Icon = null;
            radioButtonItem4.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem4.Tag = null;
            radioButtonItem4.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AuxOutputPage.RboAuxOutput.panel1.Btn_3"); //"通过测试";
            RboAuxOutput.ButtonItems = (new ScopeX.UserControls.RadioButtonItem[] { radioButtonItem1, radioButtonItem2, radioButtonItem3, radioButtonItem4 });
            RboAuxOutput.ButtonOffset = 10;
            RboAuxOutput.ButtonTextColor = System.Drawing.Color.White;
            RboAuxOutput.ChoosedButtonColor = System.Drawing.Color.FromArgb(18, 183, 245);
            RboAuxOutput.ChoosedButtonIndex = 0;
            RboAuxOutput.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RboAuxOutput.ContentBackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            RboAuxOutput.ContentPadding = new System.Windows.Forms.Padding(0);
            RboAuxOutput.FocusBorderColor = System.Drawing.Color.White;
            RboAuxOutput.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RboAuxOutput.ForeColor = System.Drawing.Color.White;
            RboAuxOutput.Height = 30;
            RboAuxOutput.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RboAuxOutput.Location = new System.Drawing.Point(12, 219);
            RboAuxOutput.Name = "RboAuxOutput";
            RboAuxOutput.Size = new System.Drawing.Size(433, 30);
            RboAuxOutput.StyleFlags = UserControls.Style.StyleFlag.None;
            RboAuxOutput.StylizeFlag = true;
            RboAuxOutput.TabIndex = 3;
            RboAuxOutput.IndexChanged += RboAuxOutput_IndexChanged;
            // 
            // RboAuxOutPolarity
            // 
            RboAuxOutPolarity.BackColor = System.Drawing.Color.AliceBlue;
            RboAuxOutPolarity.BorderColor = System.Drawing.Color.AliceBlue;
            RboAuxOutPolarity.BorderThickness = 0;
            RboAuxOutPolarity.ButtonBackColor = System.Drawing.Color.FromArgb(50, 50, 54);
            RboAuxOutPolarity.ButtonFont = null;
            radioButtonItem5.Icon = Properties.Resources.PosPulse;
            radioButtonItem5.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem5.Tag = null;
            radioButtonItem5.Text = "";
            radioButtonItem6.Icon = Properties.Resources.NegPulse;
            radioButtonItem6.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem6.Tag = null;
            radioButtonItem6.Text = "";
            RboAuxOutPolarity.ButtonItems = new UserControls.RadioButtonItem[]
    {
    radioButtonItem5,
    radioButtonItem6
    };
            RboAuxOutPolarity.ButtonOffset = 10;
            RboAuxOutPolarity.ButtonTextColor = System.Drawing.Color.White;
            RboAuxOutPolarity.ChoosedButtonColor = System.Drawing.Color.FromArgb(18, 183, 245);
            RboAuxOutPolarity.ChoosedButtonIndex = 0;
            RboAuxOutPolarity.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RboAuxOutPolarity.ContentBackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            RboAuxOutPolarity.ContentPadding = new System.Windows.Forms.Padding(0);
            RboAuxOutPolarity.FocusBorderColor = System.Drawing.Color.White;
            RboAuxOutPolarity.ForeColor = System.Drawing.Color.White;
            RboAuxOutPolarity.Height = 30;
            RboAuxOutPolarity.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RboAuxOutPolarity.Location = new System.Drawing.Point(12, 314);
            RboAuxOutPolarity.Name = "RboAuxOutPolarity";
            RboAuxOutPolarity.Size = new System.Drawing.Size(160, 30);
            RboAuxOutPolarity.StyleFlags = UserControls.Style.StyleFlag.None;
            RboAuxOutPolarity.StylizeFlag = true;
            RboAuxOutPolarity.TabIndex = 5;
            RboAuxOutPolarity.IndexChanged += RboAuxOutPolarity_IndexChanged;
            // 
            // LblAuxOutPolarity
            // 
            LblAuxOutPolarity.BackColor = System.Drawing.Color.Empty;
            LblAuxOutPolarity.BorderColor = System.Drawing.Color.Black;
            LblAuxOutPolarity.BorderThickness = 0;
            LblAuxOutPolarity.CornerRadius = 0;
            LblAuxOutPolarity.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblAuxOutPolarity.ForeColor = System.Drawing.Color.FromArgb(234, 234, 234);
            LblAuxOutPolarity.HighlightPrompt = defaultHighlightPrompt2;
            LblAuxOutPolarity.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblAuxOutPolarity.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblAuxOutPolarity.Location = new System.Drawing.Point(12, 286);
            LblAuxOutPolarity.MultyLineFlag = false;
            LblAuxOutPolarity.Name = "LblAuxOutPolarity";
            LblAuxOutPolarity.Size = new System.Drawing.Size(160, 18);
            LblAuxOutPolarity.StyleFlags = UserControls.Style.StyleFlag.None;
            LblAuxOutPolarity.StylizeFlag = true;
            LblAuxOutPolarity.TabIndex = 4;
            LblAuxOutPolarity.TabStop = false;
            LblAuxOutPolarity.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AuxOutputPage.LblAuxOutPolarity"); //"输出极性";
            LblAuxOutPolarity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblAuxOutPolarity.Token = null;
            // 
            // LblAuxInput
            // 
            LblAuxInput.BackColor = System.Drawing.Color.Empty;
            LblAuxInput.BorderColor = System.Drawing.Color.Black;
            LblAuxInput.BorderThickness = 0;
            LblAuxInput.CornerRadius = 0;
            LblAuxInput.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblAuxInput.ForeColor = System.Drawing.Color.FromArgb(234, 234, 234);
            LblAuxInput.HighlightPrompt = defaultHighlightPrompt3;
            LblAuxInput.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblAuxInput.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblAuxInput.Location = new System.Drawing.Point(12, 10);
            LblAuxInput.MultyLineFlag = false;
            LblAuxInput.Name = "LblAuxInput";
            LblAuxInput.Size = new System.Drawing.Size(160, 18);
            LblAuxInput.StyleFlags = UserControls.Style.StyleFlag.None;
            LblAuxInput.StylizeFlag = true;
            LblAuxInput.TabIndex = 0;
            LblAuxInput.TabStop = false;
            LblAuxInput.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuZhuShuRu");
            LblAuxInput.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblAuxInput.Token = null;
            // 
            // RboAuxInput
            // 
            RboAuxInput.BackColor = System.Drawing.Color.AliceBlue;
            RboAuxInput.BorderColor = System.Drawing.Color.AliceBlue;
            RboAuxInput.BorderThickness = 0;
            RboAuxInput.ButtonBackColor = System.Drawing.Color.FromArgb(50, 50, 54);
            RboAuxInput.ButtonFont = null;
            radioButtonItem7.Icon = null;
            radioButtonItem7.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem7.Tag = null;
            radioButtonItem7.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AuxOutputPage.RboAuxInput.panel1.Btn_0");// "关闭";
            radioButtonItem8.Icon = null;
            radioButtonItem8.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem8.Tag = null;
            radioButtonItem8.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AuxOutputPage.RboAuxInput.panel1.Btn_1"); //"触发同步";
            radioButtonItem9.Icon = null;
            radioButtonItem9.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem9.Tag = null;
            radioButtonItem9.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AuxOutputPage.RboAuxInput.panel1.Btn_2");//"AWG外触发";
            RboAuxInput.ButtonItems = (new ScopeX.UserControls.RadioButtonItem[] { radioButtonItem7, radioButtonItem8, radioButtonItem9 });
            RboAuxInput.ButtonOffset = 10;
            RboAuxInput.ButtonTextColor = System.Drawing.Color.White;
            RboAuxInput.ChoosedButtonColor = System.Drawing.Color.FromArgb(18, 183, 245);
            RboAuxInput.ChoosedButtonIndex = 0;
            RboAuxInput.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RboAuxInput.ContentBackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            RboAuxInput.ContentPadding = new System.Windows.Forms.Padding(0);
            RboAuxInput.FocusBorderColor = System.Drawing.Color.White;
            RboAuxInput.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RboAuxInput.ForeColor = System.Drawing.Color.White;
            RboAuxInput.Height = 30;
            RboAuxInput.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RboAuxInput.Location = new System.Drawing.Point(12, 33);
            RboAuxInput.Name = "RboAuxInput";
            RboAuxInput.Size = new System.Drawing.Size(433, 30);
            RboAuxInput.StyleFlags = UserControls.Style.StyleFlag.None;
            RboAuxInput.StylizeFlag = true;
            RboAuxInput.TabIndex = 1;
            RboAuxInput.IndexChanged += RboAuxInput_IndexChanged;
            // 
            // RboAuxInPolarity
            // 
            RboAuxInPolarity.BackColor = System.Drawing.Color.AliceBlue;
            RboAuxInPolarity.BorderColor = System.Drawing.Color.AliceBlue;
            RboAuxInPolarity.BorderThickness = 0;
            RboAuxInPolarity.ButtonBackColor = System.Drawing.Color.FromArgb(50, 50, 54);
            RboAuxInPolarity.ButtonFont = null;
            radioButtonItem10.Icon = Properties.Resources.PosPulse;
            radioButtonItem10.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem10.Tag = null;
            radioButtonItem10.Text = "";
            radioButtonItem11.Icon = Properties.Resources.NegPulse;
            radioButtonItem11.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem11.Tag = null;
            radioButtonItem11.Text = "";
            RboAuxInPolarity.ButtonItems = new UserControls.RadioButtonItem[]
    {
    radioButtonItem10,
    radioButtonItem11
    };
            RboAuxInPolarity.ButtonOffset = 10;
            RboAuxInPolarity.ButtonTextColor = System.Drawing.Color.White;
            RboAuxInPolarity.ChoosedButtonColor = System.Drawing.Color.FromArgb(18, 183, 245);
            RboAuxInPolarity.ChoosedButtonIndex = 0;
            RboAuxInPolarity.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RboAuxInPolarity.ContentBackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            RboAuxInPolarity.ContentPadding = new System.Windows.Forms.Padding(0);
            RboAuxInPolarity.FocusBorderColor = System.Drawing.Color.White;
            RboAuxInPolarity.ForeColor = System.Drawing.Color.White;
            RboAuxInPolarity.Height = 26;
            RboAuxInPolarity.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RboAuxInPolarity.Location = new System.Drawing.Point(12, 128);
            RboAuxInPolarity.Name = "RboAuxInPolarity";
            RboAuxInPolarity.Size = new System.Drawing.Size(160, 26);
            RboAuxInPolarity.StyleFlags = UserControls.Style.StyleFlag.None;
            RboAuxInPolarity.StylizeFlag = true;
            RboAuxInPolarity.TabIndex = 7;
            RboAuxInPolarity.IndexChanged += RboAuxInPolarity_IndexChanged;
            // 
            // LblAuxInPolarity
            // 
            LblAuxInPolarity.BackColor = System.Drawing.Color.Empty;
            LblAuxInPolarity.BorderColor = System.Drawing.Color.Black;
            LblAuxInPolarity.BorderThickness = 0;
            LblAuxInPolarity.CornerRadius = 0;
            LblAuxInPolarity.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblAuxInPolarity.ForeColor = System.Drawing.Color.FromArgb(234, 234, 234);
            LblAuxInPolarity.HighlightPrompt = defaultHighlightPrompt4;
            LblAuxInPolarity.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblAuxInPolarity.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblAuxInPolarity.Location = new System.Drawing.Point(12, 100);
            LblAuxInPolarity.MultyLineFlag = false;
            LblAuxInPolarity.Name = "LblAuxInPolarity";
            LblAuxInPolarity.Size = new System.Drawing.Size(160, 18);
            LblAuxInPolarity.StyleFlags = UserControls.Style.StyleFlag.None;
            LblAuxInPolarity.StylizeFlag = true;
            LblAuxInPolarity.TabIndex = 6;
            LblAuxInPolarity.TabStop = false;
            LblAuxInPolarity.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AuxOutputPage.LblAuxInPolarity"); // "输入极性";
            LblAuxInPolarity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblAuxInPolarity.Token = null;
            // 
            // LblAuxInTip
            // 
            LblAuxInTip.BackColor = System.Drawing.Color.Empty;
            LblAuxInTip.BorderColor = System.Drawing.Color.Black;
            LblAuxInTip.BorderThickness = 0;
            LblAuxInTip.CornerRadius = 0;
            LblAuxInTip.Font = new System.Drawing.Font("MiSans", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblAuxInTip.ForeColor = System.Drawing.Color.Khaki;
            LblAuxInTip.HighlightPrompt = defaultHighlightPrompt5;
            LblAuxInTip.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblAuxInTip.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblAuxInTip.Location = new System.Drawing.Point(12, 75);
            LblAuxInTip.MultyLineFlag = false;
            LblAuxInTip.Name = "LblAuxInTip";
            LblAuxInTip.Size = new System.Drawing.Size(400, 18);
            LblAuxInTip.StyleFlags = UserControls.Style.StyleFlag.None;
            LblAuxInTip.StylizeFlag = false;
            LblAuxInTip.TabIndex = 8;
            LblAuxInTip.TabStop = false;
            LblAuxInTip.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TiShi_QingZhengQueJieRuWaiChuFaXinHao_FouZeAWGWuXinHaoShuChu_"); // 此提示需要动态复制，不能直接写死。故而初始化时不需要赋值
            LblAuxInTip.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblAuxInTip.Token = null;
            LblAuxInTip.Visible = false;
            // 
            // AuxOutputPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            Controls.Add(LblAuxInTip);
            Controls.Add(RboAuxInPolarity);
            Controls.Add(LblAuxInPolarity);
            Controls.Add(RboAuxInput);
            Controls.Add(LblAuxInput);
            Controls.Add(RboAuxOutPolarity);
            Controls.Add(LblAuxOutPolarity);
            Controls.Add(RboAuxOutput);
            Controls.Add(LblAuxOutput);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "AuxOutputPage";
            Size = new System.Drawing.Size(459, 372);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXLabel LblAuxOutput;
        private ScopeX.UserControls.UIRadioButtonGroup RboAuxOutput;
        private ScopeX.UserControls.UIRadioButtonGroup RboAuxOutPolarity;
        private ScopeX.UserControls.ScopeXLabel LblAuxOutPolarity;
        private ScopeX.UserControls.ScopeXLabel LblAuxInput;
        private ScopeX.UserControls.UIRadioButtonGroup RboAuxInput;
        private ScopeX.UserControls.UIRadioButtonGroup RboAuxInPolarity;
        private ScopeX.UserControls.ScopeXLabel LblAuxInPolarity;
        private ScopeX.UserControls.ScopeXLabel LblAuxInTip;
    }
}
