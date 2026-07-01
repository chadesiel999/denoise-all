using ScopeX.U2.LanguageSupoort;

namespace ScopeX.U2
{
    partial class OtherSettingPage
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
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt5 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt6 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt7 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt8 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt9 = new UserControls.DefaultHighlightPrompt();
            LblLanguage = new UserControls.ScopeXLabel();
            CbxLanguage = new ScopeX.UserControls.SelectComboBox();
            LblTime = new UserControls.ScopeXLabel();
            LblDefault = new UserControls.ScopeXLabel();
            BtnSettingTime = new UserControls.ScopeXIconButton();
            LblChnlColor = new UserControls.ScopeXLabel();
            LblChannel = new UserControls.ScopeXLabel();
            CbxChannel = new ScopeX.UserControls.SelectComboBox();
            BtnRestoreFactory = new UserControls.ScopeXIconButton();
            LblTouch = new UserControls.ScopeXLabel();
            ChkTouch = new UserControls.ScopeXSwitchButton();
            DbxDateTime = new ScopeXDateTimeBox();
            LblTempCheck = new UserControls.ScopeXLabel();
            BtnShowTemperature = new UserControls.ScopeXIconButton();
            LblSysRunTime = new UserControls.ScopeXLabel();
            STBSysRunTime = new UserControls.ScopeXTextBox();
            BtnResetOptionTime = new UserControls.ScopeXIconButton();
            BtnOverOptionTime = new UserControls.ScopeXIconButton();
            scopexLabel2 = new UserControls.ScopeXLabel();
            LblStopMeasure = new UserControls.ScopeXLabel();
            ChkStopMeasure = new UserControls.ScopeXSwitchButton();
            SuspendLayout();
            // 
            // LblLanguage
            // 
            LblLanguage.BackColor = System.Drawing.Color.Empty;
            LblLanguage.BorderColor = System.Drawing.Color.Black;
            LblLanguage.BorderThickness = 0;
            LblLanguage.CornerRadius = 0;
            LblLanguage.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblLanguage.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblLanguage.HighlightPrompt = defaultHighlightPrompt1;
            LblLanguage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblLanguage.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblLanguage.Location = new System.Drawing.Point(5, 164);
            LblLanguage.MultyLineFlag = false;
            LblLanguage.Name = "LblLanguage";
            LblLanguage.Size = new System.Drawing.Size(100, 30);
            LblLanguage.StyleFlags = UserControls.Style.StyleFlag.None;
            LblLanguage.StylizeFlag = true;
            LblLanguage.TabIndex = 19;
            LblLanguage.TabStop = false;
            //LblLanguage.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.OtherSettingPage.LblLanguage"); //多语言"语言";
            LblLanguage.Text = "Language";
            LblLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblLanguage.Token = null;
            // 
            // CbxLanguage
            // 
            CbxLanguage.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxLanguage.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxLanguage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxLanguage.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxLanguage.ForeColor = System.Drawing.Color.White;
            CbxLanguage.Location = new System.Drawing.Point(140, 164);
            CbxLanguage.Name = "CbxLanguage";
            CbxLanguage.Size = new System.Drawing.Size(115, 30);
            CbxLanguage.TabIndex = 38;
            // 
            // LblTime
            // 
            LblTime.BackColor = System.Drawing.Color.Empty;
            LblTime.BorderColor = System.Drawing.Color.Black;
            LblTime.BorderThickness = 0;
            LblTime.CornerRadius = 0;
            LblTime.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblTime.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblTime.HighlightPrompt = defaultHighlightPrompt8;
            LblTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTime.Location = new System.Drawing.Point(5, 68);
            LblTime.MultyLineFlag = false;
            LblTime.Name = "LblTime";
            LblTime.Size = new System.Drawing.Size(120, 30);
            LblTime.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblTime.StylizeFlag = true;
            LblTime.TabIndex = 21;
            LblTime.TabStop = false;
            LblTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.OtherSettingPage.LblTime"); //"时间";"时间";
            LblTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblTime.Token = null;
            // 
            // LblDefault
            // 
            LblDefault.BackColor = System.Drawing.Color.Empty;
            LblDefault.BorderColor = System.Drawing.Color.Black;
            LblDefault.BorderThickness = 0;
            LblDefault.CornerRadius = 0;
            LblDefault.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblDefault.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblDefault.HighlightPrompt = defaultHighlightPrompt9;
            LblDefault.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblDefault.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblDefault.Location = new System.Drawing.Point(5, 213);
            LblDefault.MultyLineFlag = false;
            LblDefault.Name = "LblDefault";
            LblDefault.Size = new System.Drawing.Size(85, 30);
            LblDefault.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblDefault.StylizeFlag = true;
            LblDefault.TabIndex = 22;
            LblDefault.TabStop = false;
            LblDefault.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MoRenSheZhi");
            LblDefault.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblDefault.Token = null;
            // 
            // BtnSettingTime
            // 
            BtnSettingTime.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSettingTime.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSettingTime.BorderThickness = 1;
            BtnSettingTime.CornerRadius = 0;
            BtnSettingTime.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSettingTime.DaskArray = null;
            BtnSettingTime.DropKey = System.Windows.Forms.Keys.Space;
            BtnSettingTime.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnSettingTime.ForeColor = System.Drawing.Color.White;
            BtnSettingTime.Height = 30;
            BtnSettingTime.Icon = null;
            BtnSettingTime.IconOffset = 10;
            BtnSettingTime.IconSize = new System.Drawing.Size(24, 24);
            BtnSettingTime.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSettingTime.IsIndicatorShow = false;
            BtnSettingTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSettingTime.Location = new System.Drawing.Point(480, 68);
            BtnSettingTime.Margin = new System.Windows.Forms.Padding(2);
            BtnSettingTime.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            BtnSettingTime.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSettingTime.MouseInBorderThickness = 0;
            BtnSettingTime.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSettingTime.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSettingTime.Name = "BtnSettingTime";
            BtnSettingTime.PressedBackColor = System.Drawing.Color.Gray;
            BtnSettingTime.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSettingTime.PressedBorderThickness = 0;
            BtnSettingTime.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSettingTime.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSettingTime.Size = new System.Drawing.Size(58, 30);
            BtnSettingTime.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnSettingTime.StylizeFlag = true;
            BtnSettingTime.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSettingTime.SVGPath = "";
            BtnSettingTime.TabIndex = 28;
            BtnSettingTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.OtherSettingPage.BtnSettingTime"); //"设置";
            BtnSettingTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSettingTime.Click += BtnSettingTime_Click;
            // 
            // LblChnlColor
            // 
            LblChnlColor.BackColor = System.Drawing.Color.LightPink;
            LblChnlColor.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblChnlColor.BorderThickness = 0;
            LblChnlColor.CornerRadius = 0;
            LblChnlColor.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblChnlColor.ForeColor = System.Drawing.Color.Black;
            LblChnlColor.HighlightPrompt = defaultHighlightPrompt4;
            LblChnlColor.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblChnlColor.Location = new System.Drawing.Point(240, 23);
            LblChnlColor.MultyLineFlag = false;
            LblChnlColor.Name = "LblChnlColor";
            LblChnlColor.Size = new System.Drawing.Size(286, 30);
            LblChnlColor.StyleFlags = UserControls.Style.StyleFlag.None;
            LblChnlColor.StylizeFlag = true;
            LblChnlColor.TabIndex = 32;
            LblChnlColor.TabStop = false;
            LblChnlColor.Text = "255,182,193";
            LblChnlColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblChnlColor.Token = null;
            LblChnlColor.Click += LblChnlColor_Click;
            // 
            // LblChannel
            // 
            LblChannel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            LblChannel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            LblChannel.BorderThickness = 0;
            LblChannel.CornerRadius = 0;
            LblChannel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblChannel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            LblChannel.HighlightPrompt = defaultHighlightPrompt1;
            LblChannel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblChannel.Location = new System.Drawing.Point(5, 20);
            LblChannel.MultyLineFlag = false;
            LblChannel.Name = "LblChannel";
            LblChannel.Size = new System.Drawing.Size(85, 30);
            LblChannel.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblChannel.StylizeFlag = true;
            LblChannel.TabIndex = 29;
            LblChannel.TabStop = false;
            LblChannel.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TongDaoYanSe");
            LblChannel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblChannel.Token = null;
            // 
            // CbxChannel
            // 
            CbxChannel.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxChannel.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxChannel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxChannel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxChannel.ForeColor = System.Drawing.Color.White;
            CbxChannel.Location = new System.Drawing.Point(140, 20);
            CbxChannel.Name = "CbxChannel";
            CbxChannel.Size = new System.Drawing.Size(75, 30);
            CbxChannel.TabIndex = 37;
            // 
            // BtnRestoreFactory
            // 
            BtnRestoreFactory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            BtnRestoreFactory.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            BtnRestoreFactory.BorderThickness = 1;
            BtnRestoreFactory.CornerRadius = 0;
            BtnRestoreFactory.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnRestoreFactory.DaskArray = null;
            BtnRestoreFactory.DropKey = System.Windows.Forms.Keys.Space;
            BtnRestoreFactory.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnRestoreFactory.ForeColor = System.Drawing.Color.White;
            BtnRestoreFactory.Height = 30;
            BtnRestoreFactory.Icon = null;
            BtnRestoreFactory.IconOffset = 10;
            BtnRestoreFactory.IconSize = new System.Drawing.Size(24, 24);
            BtnRestoreFactory.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            BtnRestoreFactory.IsIndicatorShow = false;
            BtnRestoreFactory.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnRestoreFactory.Location = new System.Drawing.Point(140, 213);
            BtnRestoreFactory.Margin = new System.Windows.Forms.Padding(2);
            BtnRestoreFactory.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            BtnRestoreFactory.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRestoreFactory.MouseInBorderThickness = 0;
            BtnRestoreFactory.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnRestoreFactory.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnRestoreFactory.Name = "BtnRestoreFactory";
            BtnRestoreFactory.PressedBackColor = System.Drawing.Color.Gray;
            BtnRestoreFactory.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRestoreFactory.PressedBorderThickness = 0;
            BtnRestoreFactory.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnRestoreFactory.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnRestoreFactory.Size = new System.Drawing.Size(118, 30);
            BtnRestoreFactory.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnRestoreFactory.StylizeFlag = true;
            BtnRestoreFactory.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnRestoreFactory.SVGPath = "";
            BtnRestoreFactory.TabIndex = 33;
            BtnRestoreFactory.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.OtherSettingPage.BtnRestoreFactory"); // "恢复出厂";
            BtnRestoreFactory.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnRestoreFactory.Click += BtnRestoreFactory_Click;
            // 
            // LblTouch
            // 
            LblTouch.BackColor = System.Drawing.Color.Transparent;
            LblTouch.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblTouch.BorderThickness = 0;
            LblTouch.CornerRadius = 0;
            LblTouch.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblTouch.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblTouch.HighlightPrompt = defaultHighlightPrompt6;
            LblTouch.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTouch.Location = new System.Drawing.Point(5, 260);
            LblTouch.MultyLineFlag = false;
            LblTouch.Name = "LblTouch";
            LblTouch.Size = new System.Drawing.Size(120, 30);
            LblTouch.StyleFlags = UserControls.Style.StyleFlag.None;
            LblTouch.StylizeFlag = true;
            LblTouch.TabIndex = 35;
            LblTouch.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.OtherSettingPage.LblTouch"); // "触摸锁";
            LblTouch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblTouch.Token = null;
            // 
            // ChkTouch
            // 
            ChkTouch.AnimationCount = 8;
            ChkTouch.AnimationFunc = null;
            ChkTouch.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkTouch.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkTouch.BorderThickness = 0;
            ChkTouch.Checked = false;
            ChkTouch.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkTouch.CheckedForeColor = System.Drawing.Color.Black;
            ChkTouch.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkTouch.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.OtherSettingPage.ChkTouch", "CheckedText"); // "开";
            ChkTouch.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkTouch.DropKey = System.Windows.Forms.Keys.Space;
            ChkTouch.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkTouch.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkTouch.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkTouch.Height = 30;
            ChkTouch.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkTouch.Location = new System.Drawing.Point(140, 260);
            ChkTouch.Margin = new System.Windows.Forms.Padding(0);
            ChkTouch.Name = "ChkTouch";
            ChkTouch.Size = new System.Drawing.Size(80, 30);
            ChkTouch.SliderButtonWidth = 30;
            ChkTouch.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkTouch.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkTouch.StylizeFlag = true;
            ChkTouch.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkTouch.TabIndex = 34;
            ChkTouch.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.OtherSettingPage.ChkTouch"); // "关";
            ChkTouch.UseAnimation = true;
            // 
            // LblStopMeasure
            // 
            LblStopMeasure.BackColor = System.Drawing.Color.Transparent;
            LblStopMeasure.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblStopMeasure.BorderThickness = 0;
            LblStopMeasure.CornerRadius = 0;
            LblStopMeasure.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblStopMeasure.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblStopMeasure.HighlightPrompt = defaultHighlightPrompt9;
            LblStopMeasure.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblStopMeasure.Location = new System.Drawing.Point(5, 308);
            LblStopMeasure.MultyLineFlag = false;
            LblStopMeasure.Name = "LblStopMeasure";
            LblStopMeasure.Size = new System.Drawing.Size(135, 30);
            LblStopMeasure.StyleFlags = UserControls.Style.StyleFlag.None;
            LblStopMeasure.StylizeFlag = true;
            LblStopMeasure.TabIndex = 45;
            LblStopMeasure.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.OtherSettingPage.LblStopMeasure");
            LblStopMeasure.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblStopMeasure.Token = null;
            LblStopMeasure.Visible = true;
            // 
            // ChkStopMeasure
            // 
            ChkStopMeasure.AnimationCount = 8;
            ChkStopMeasure.AnimationFunc = null;
            ChkStopMeasure.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkStopMeasure.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkStopMeasure.BorderThickness = 0;
            ChkStopMeasure.Checked = false;
            ChkStopMeasure.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkStopMeasure.CheckedForeColor = System.Drawing.Color.Black;
            ChkStopMeasure.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkStopMeasure.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.OtherSettingPage.ChkStopMeasure", "CheckedText"); // "开";
            ChkStopMeasure.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkStopMeasure.DropKey = System.Windows.Forms.Keys.Space;
            ChkStopMeasure.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkStopMeasure.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkStopMeasure.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkStopMeasure.Height = 30;
            ChkStopMeasure.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkStopMeasure.Location = new System.Drawing.Point(140, 308);
            ChkStopMeasure.Margin = new System.Windows.Forms.Padding(0);
            ChkStopMeasure.Name = "ChkStopMeasure";
            ChkStopMeasure.Size = new System.Drawing.Size(80, 30);
            ChkStopMeasure.SliderButtonWidth = 30;
            ChkStopMeasure.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkStopMeasure.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkStopMeasure.StylizeFlag = true;
            ChkStopMeasure.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkStopMeasure.TabIndex = 34;
            ChkStopMeasure.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.OtherSettingPage.ChkStopMeasure"); // "关";
            ChkStopMeasure.UseAnimation = true;
            // 
            // DbxDateTime
            // 
            DbxDateTime.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            DbxDateTime.CurrentTime = new System.DateTime(2022, 11, 16, 16, 35, 11, 870);
            DbxDateTime.Cursor = System.Windows.Forms.Cursors.IBeam;
            DbxDateTime.Font = new System.Drawing.Font("MiSans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            DbxDateTime.ForeColor = System.Drawing.Color.White;
            DbxDateTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            DbxDateTime.Location = new System.Drawing.Point(140, 68);
            DbxDateTime.Name = "DbxDateTime";
            DbxDateTime.ReadOnly = false;
            DbxDateTime.SelectedColor = System.Drawing.Color.FromArgb(40, 71, 193);
            DbxDateTime.Size = new System.Drawing.Size(334, 30);
            DbxDateTime.StyleFlags = UserControls.Style.StyleFlag.None;
            DbxDateTime.StylizeFlag = true;
            DbxDateTime.TabIndex = 36;
            DbxDateTime.Text = "ScopeXDateTimeBox1";
            // 
            // LblTempCheck
            // 
            LblTempCheck.BackColor = System.Drawing.Color.Transparent;
            LblTempCheck.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblTempCheck.BorderThickness = 0;
            LblTempCheck.CornerRadius = 0;
            LblTempCheck.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblTempCheck.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblTempCheck.HighlightPrompt = defaultHighlightPrompt7;
            LblTempCheck.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTempCheck.Location = new System.Drawing.Point(5, 356);
            LblTempCheck.MultyLineFlag = false;
            LblTempCheck.Name = "LblTempCheck";
            LblTempCheck.Size = new System.Drawing.Size(85, 30);
            LblTempCheck.StyleFlags = UserControls.Style.StyleFlag.None;
            LblTempCheck.StylizeFlag = true;
            LblTempCheck.TabIndex = 37;
            LblTempCheck.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WenDuJianKong");
            LblTempCheck.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblTempCheck.Token = null;
            // 
            // BtnShowTemperature
            // 
            BtnShowTemperature.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowTemperature.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowTemperature.BorderThickness = 1;
            BtnShowTemperature.CornerRadius = 0;
            BtnShowTemperature.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnShowTemperature.DaskArray = null;
            BtnShowTemperature.DropKey = System.Windows.Forms.Keys.Space;
            BtnShowTemperature.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnShowTemperature.ForeColor = System.Drawing.Color.White;
            BtnShowTemperature.Height = 30;
            BtnShowTemperature.Icon = Properties.Resources.Debug;
            BtnShowTemperature.IconOffset = 2;
            BtnShowTemperature.IconSize = new System.Drawing.Size(24, 24);
            BtnShowTemperature.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnShowTemperature.IsIndicatorShow = false;
            BtnShowTemperature.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnShowTemperature.Location = new System.Drawing.Point(140, 356);
            BtnShowTemperature.Margin = new System.Windows.Forms.Padding(2);
            BtnShowTemperature.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowTemperature.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowTemperature.MouseInBorderThickness = 0;
            BtnShowTemperature.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowTemperature.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnShowTemperature.Name = "BtnShowTemperature";
            BtnShowTemperature.PressedBackColor = System.Drawing.Color.Gray;
            BtnShowTemperature.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowTemperature.PressedBorderThickness = 0;
            BtnShowTemperature.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowTemperature.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnShowTemperature.Size = new System.Drawing.Size(75, 30);
            BtnShowTemperature.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnShowTemperature.StylizeFlag = true;
            BtnShowTemperature.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowTemperature.SVGPath = "";
            BtnShowTemperature.TabIndex = 38;
            BtnShowTemperature.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShi");
            BtnShowTemperature.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnShowTemperature.Click += BtnShowTemperature_Click;
            // 
            // scopexLabel1
            // 
            LblSysRunTime.BackColor = System.Drawing.Color.Transparent;
            LblSysRunTime.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblSysRunTime.BorderThickness = 0;
            LblSysRunTime.CornerRadius = 0;
            LblSysRunTime.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSysRunTime.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblSysRunTime.HighlightPrompt = defaultHighlightPrompt8;
            LblSysRunTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSysRunTime.Location = new System.Drawing.Point(5, 116);
            LblSysRunTime.MultyLineFlag = false;
            LblSysRunTime.Name = "LblSysRunTime";
            LblSysRunTime.Size = new System.Drawing.Size(126, 30);
            LblSysRunTime.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSysRunTime.StylizeFlag = true;
            LblSysRunTime.TabIndex = 40;
            LblSysRunTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.OtherSettingPage.LblSysRunTime");//"YunXingShiChang"
            LblSysRunTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSysRunTime.Token = null;
            // 
            // STBSysRunTime
            // 
            STBSysRunTime.AcceptsTab = false;
            STBSysRunTime.AutoShowKeyBoard = false;
            STBSysRunTime.AutoSize = false;
            STBSysRunTime.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            STBSysRunTime.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            STBSysRunTime.BorderThickness = 0;
            STBSysRunTime.CornerRadius = 0;
            STBSysRunTime.Cursor = System.Windows.Forms.Cursors.Hand;
            STBSysRunTime.Enabled = true;
            STBSysRunTime.EnbleSelectBorder = true;
            STBSysRunTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            STBSysRunTime.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            STBSysRunTime.Height = 30;
            STBSysRunTime.HideSelection = true;
            STBSysRunTime.KeyboardVerify = null;
            STBSysRunTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            STBSysRunTime.Lines = new string[] { "00:00:00" };
            STBSysRunTime.Location = new System.Drawing.Point(140, 116);
            STBSysRunTime.MaxLength = 32767;
            STBSysRunTime.Modified = false;
            STBSysRunTime.MouseEnterState = false;
            STBSysRunTime.Multiline = false;
            STBSysRunTime.Name = "STBSysRunTime";
            STBSysRunTime.ProcessCmdKeyFunc = null;
            STBSysRunTime.ReadOnly = true;
            STBSysRunTime.SelectedColor = System.Drawing.Color.FromArgb(0, 157, 255);
            STBSysRunTime.SelectedText = "";
            STBSysRunTime.SelectionLength = 0;
            STBSysRunTime.SelectionStart = 0;
            STBSysRunTime.ShortcutsEnabled = true;
            STBSysRunTime.Size = new System.Drawing.Size(118, 30);
            STBSysRunTime.StyleFlags = UserControls.Style.StyleFlag.None;
            STBSysRunTime.StylizeFlag = true;
            STBSysRunTime.TabIndex = 42;
            STBSysRunTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            STBSysRunTime.UseSystemPasswordChar = false;
            STBSysRunTime.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            STBSysRunTime.WordWrap = true;
            // 
            // BtnResetOptionTime
            // 
            BtnResetOptionTime.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetOptionTime.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetOptionTime.BorderThickness = 1;
            BtnResetOptionTime.CornerRadius = 0;
            BtnResetOptionTime.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnResetOptionTime.DaskArray = null;
            BtnResetOptionTime.DropKey = System.Windows.Forms.Keys.Space;
            BtnResetOptionTime.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnResetOptionTime.ForeColor = System.Drawing.Color.White;
            BtnResetOptionTime.Height = 30;
            BtnResetOptionTime.Icon = null;
            BtnResetOptionTime.IconOffset = 2;
            BtnResetOptionTime.IconSize = new System.Drawing.Size(24, 24);
            BtnResetOptionTime.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnResetOptionTime.IsIndicatorShow = false;
            BtnResetOptionTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnResetOptionTime.Location = new System.Drawing.Point(140, 403);
            BtnResetOptionTime.Margin = new System.Windows.Forms.Padding(2);
            BtnResetOptionTime.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetOptionTime.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetOptionTime.MouseInBorderThickness = 0;
            BtnResetOptionTime.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetOptionTime.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnResetOptionTime.Name = "BtnResetOptionTime";
            BtnResetOptionTime.PressedBackColor = System.Drawing.Color.Gray;
            BtnResetOptionTime.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetOptionTime.PressedBorderThickness = 0;
            BtnResetOptionTime.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetOptionTime.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnResetOptionTime.Size = new System.Drawing.Size(118, 30);
            BtnResetOptionTime.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnResetOptionTime.StylizeFlag = true;
            BtnResetOptionTime.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetOptionTime.SVGPath = "";
            BtnResetOptionTime.TabIndex = 43;
            BtnResetOptionTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhongZhiShiYong");
            BtnResetOptionTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnResetOptionTime.Visible = false;
            BtnResetOptionTime.Click += BtnResetOptionTime_Click;
            // 
            // BtnOverOptionTime
            // 
            BtnOverOptionTime.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOverOptionTime.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOverOptionTime.BorderThickness = 1;
            BtnOverOptionTime.CornerRadius = 0;
            BtnOverOptionTime.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnOverOptionTime.DaskArray = null;
            BtnOverOptionTime.DropKey = System.Windows.Forms.Keys.Space;
            BtnOverOptionTime.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnOverOptionTime.ForeColor = System.Drawing.Color.White;
            BtnOverOptionTime.Height = 30;
            BtnOverOptionTime.Icon = null;
            BtnOverOptionTime.IconOffset = 2;
            BtnOverOptionTime.IconSize = new System.Drawing.Size(24, 24);
            BtnOverOptionTime.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnOverOptionTime.IsIndicatorShow = false;
            BtnOverOptionTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnOverOptionTime.Location = new System.Drawing.Point(212, 403);
            BtnOverOptionTime.Margin = new System.Windows.Forms.Padding(2);
            BtnOverOptionTime.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOverOptionTime.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOverOptionTime.MouseInBorderThickness = 0;
            BtnOverOptionTime.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOverOptionTime.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnOverOptionTime.Name = "BtnOverOptionTime";
            BtnOverOptionTime.PressedBackColor = System.Drawing.Color.Gray;
            BtnOverOptionTime.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOverOptionTime.PressedBorderThickness = 0;
            BtnOverOptionTime.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOverOptionTime.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnOverOptionTime.Size = new System.Drawing.Size(75, 30);
            BtnOverOptionTime.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnOverOptionTime.StylizeFlag = true;
            BtnOverOptionTime.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOverOptionTime.SVGPath = "";
            BtnOverOptionTime.TabIndex = 44;
            BtnOverOptionTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JieShuShiYong");
            BtnOverOptionTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnOverOptionTime.Visible = false;
            BtnOverOptionTime.Click += BtnOverOptionTime_Click;
            // 
            // scopexLabel2
            // 
            scopexLabel2.BackColor = System.Drawing.Color.Transparent;
            scopexLabel2.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            scopexLabel2.BorderThickness = 0;
            scopexLabel2.CornerRadius = 0;
            scopexLabel2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            scopexLabel2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            scopexLabel2.HighlightPrompt = defaultHighlightPrompt9;
            scopexLabel2.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            scopexLabel2.Location = new System.Drawing.Point(5, 403);
            scopexLabel2.MultyLineFlag = false;
            scopexLabel2.Name = "scopexLabel2";
            scopexLabel2.Size = new System.Drawing.Size(85, 30);
            scopexLabel2.StyleFlags = UserControls.Style.StyleFlag.None;
            scopexLabel2.StylizeFlag = true;
            scopexLabel2.TabIndex = 45;
            scopexLabel2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiYongShiJian");
            scopexLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            scopexLabel2.Token = null;
            scopexLabel2.Visible = false;
            // 
            // OtherSettingPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            Controls.Add(scopexLabel2);
            Controls.Add(BtnOverOptionTime);
            Controls.Add(BtnResetOptionTime);
            Controls.Add(STBSysRunTime);
            Controls.Add(LblSysRunTime);
            Controls.Add(BtnShowTemperature);
            Controls.Add(LblTempCheck);
            Controls.Add(DbxDateTime);
            Controls.Add(LblStopMeasure);
            Controls.Add(LblTouch);
            Controls.Add(ChkStopMeasure);
            Controls.Add(ChkTouch);
            Controls.Add(BtnRestoreFactory);
            Controls.Add(LblChnlColor);
            Controls.Add(LblChannel);
            Controls.Add(CbxChannel);
            Controls.Add(BtnSettingTime);
            Controls.Add(LblDefault);
            Controls.Add(LblTime);
            Controls.Add(CbxLanguage);
            Controls.Add(LblLanguage);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "OtherSettingPage";
            Size = new System.Drawing.Size(539, 460);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXLabel LblLanguage;
        private ScopeX.UserControls.SelectComboBox CbxLanguage;
        private ScopeX.UserControls.ScopeXLabel LblTime;
        private ScopeX.UserControls.ScopeXLabel LblDefault;
        private ScopeX.UserControls.ScopeXIconButton BtnSettingTime;
        private ScopeX.UserControls.ScopeXLabel LblChnlColor;
        private ScopeX.UserControls.ScopeXLabel LblChannel;
        private ScopeX.UserControls.SelectComboBox CbxChannel;
        private ScopeX.UserControls.ScopeXIconButton BtnRestoreFactory;
        private ScopeX.UserControls.ScopeXLabel LblTouch;
        private ScopeX.UserControls.ScopeXSwitchButton ChkTouch;
        private ScopeXDateTimeBox DbxDateTime;
        private UserControls.ScopeXLabel LblTempCheck;
        private UserControls.ScopeXIconButton BtnShowTemperature;
        private UserControls.ScopeXLabel LblSysRunTime;
        private UserControls.ScopeXTextBox STBSysRunTime;
        private UserControls.ScopeXIconButton BtnResetOptionTime;
        private UserControls.ScopeXIconButton BtnOverOptionTime;
        private UserControls.ScopeXLabel scopexLabel2;
        private UserControls.ScopeXLabel LblStopMeasure;
        private UserControls.ScopeXSwitchButton ChkStopMeasure;
    }
}
