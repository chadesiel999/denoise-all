
using System.Linq;
using System.Windows.Forms;

namespace ScopeX.U2
{
    partial class NormalViewSettingPage
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
            UserControls.RadioButtonItem radioButtonItem7 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem8 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem9 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem10 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem11 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem12 = new UserControls.RadioButtonItem();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt9 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt10 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt11 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt12 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt13 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt14 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt15 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt16 = new UserControls.DefaultHighlightPrompt();
            RdoWaveStyle = new UserControls.UIRadioButtonGroup();
            RdoVertLabelPos = new UserControls.UIRadioButtonGroup();
            RdoHorzLabelPos = new UserControls.UIRadioButtonGroup();
            LblGridIntensity = new UserControls.ScopeXLabel();
            LblGridStyle = new UserControls.ScopeXLabel();
            LblWaveIntensity = new UserControls.ScopeXLabel();
            LblWaveStyle = new UserControls.ScopeXLabel();
            LblHorzLabelPos = new UserControls.ScopeXLabel();
            LblVertLabelPos = new UserControls.ScopeXLabel();
            ChkTickLabel = new UserControls.ScopeXSwitchButton();
            LblTickLabel = new UserControls.ScopeXLabel();
            LblPersist = new UserControls.ScopeXLabel();
            UtbGridIntensity = new UserTrackBar();
            UtbWaveIntensity = new UserTrackBar();
            CbxGridStyle = new ScopeX.UserControls.SelectComboBox();
            CbxPersist = new ScopeX.UserControls.SelectComboBox();
            SuspendLayout();
            // 
            // RdoWaveStyle
            // 
            RdoWaveStyle.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            RdoWaveStyle.BackColor = System.Drawing.Color.Black;
            RdoWaveStyle.BorderColor = System.Drawing.Color.Black;
            RdoWaveStyle.BorderThickness = 0;
            RdoWaveStyle.ButtonBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoWaveStyle.ButtonFont = null;
            radioButtonItem7.Icon = null;
            radioButtonItem7.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem7.Tag = null;
            radioButtonItem7.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiLiang");
            radioButtonItem8.Icon = null;
            radioButtonItem8.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem8.Tag = null;
            radioButtonItem8.Text =ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Dian");
            RdoWaveStyle.ButtonItems = new UserControls.RadioButtonItem[]
    {
    radioButtonItem7,
    radioButtonItem8
    };
            RdoWaveStyle.ButtonOffset = 10;
            RdoWaveStyle.ButtonTextColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RdoWaveStyle.ChoosedButtonColor = System.Drawing.Color.FromArgb(40, 71, 193);
            RdoWaveStyle.ChoosedButtonIndex = 0;
            RdoWaveStyle.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoWaveStyle.ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            RdoWaveStyle.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoWaveStyle.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoWaveStyle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoWaveStyle.Height = 30;
            RdoWaveStyle.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoWaveStyle.Location = new System.Drawing.Point(10, 262);
            RdoWaveStyle.Name = "RdoWaveStyle";
            RdoWaveStyle.Size = new System.Drawing.Size(180, 30);
            RdoWaveStyle.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            RdoWaveStyle.StylizeFlag = true;
            RdoWaveStyle.TabIndex = 13;
            RdoWaveStyle.IndexChanged += RdoWaveStyle_IndexChanged;
            // 
            // RdoVertLabelPos
            // 
            RdoVertLabelPos.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            RdoVertLabelPos.BackColor = System.Drawing.Color.Black;
            RdoVertLabelPos.BorderColor = System.Drawing.Color.Black;
            RdoVertLabelPos.BorderThickness = 0;
            RdoVertLabelPos.ButtonBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoVertLabelPos.ButtonFont = null;
            radioButtonItem9.Icon = null;
            radioButtonItem9.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem9.Tag = null;
            radioButtonItem9.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Shang");
            radioButtonItem10.Icon = null;
            radioButtonItem10.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem10.Tag = null;
            radioButtonItem10.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Xia");
            RdoVertLabelPos.ButtonItems = new UserControls.RadioButtonItem[]
    {
    radioButtonItem9,
    radioButtonItem10
    };
            RdoVertLabelPos.ButtonOffset = 10;
            RdoVertLabelPos.ButtonTextColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RdoVertLabelPos.ChoosedButtonColor = System.Drawing.Color.FromArgb(40, 71, 193);
            RdoVertLabelPos.ChoosedButtonIndex = 0;
            RdoVertLabelPos.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoVertLabelPos.ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            RdoVertLabelPos.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoVertLabelPos.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoVertLabelPos.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoVertLabelPos.Height = 30;
            RdoVertLabelPos.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoVertLabelPos.Location = new System.Drawing.Point(250, 114);
            RdoVertLabelPos.Name = "RdoVertLabelPos";
            RdoVertLabelPos.Size = new System.Drawing.Size(180, 30);
            RdoVertLabelPos.StyleFlags = UserControls.Style.StyleFlag.None;
            RdoVertLabelPos.StylizeFlag = true;
            RdoVertLabelPos.TabIndex = 7;
            RdoVertLabelPos.IndexChanged += RdoVertTickLabel_IndexChanged;
            // 
            // RdoHorzLabelPos
            // 
            RdoHorzLabelPos.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            RdoHorzLabelPos.BackColor = System.Drawing.Color.Black;
            RdoHorzLabelPos.BorderColor = System.Drawing.Color.Black;
            RdoHorzLabelPos.BorderThickness = 0;
            RdoHorzLabelPos.ButtonBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoHorzLabelPos.ButtonFont = null;
            radioButtonItem11.Icon = null;
            radioButtonItem11.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem11.Tag = null;
            radioButtonItem11.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Zuo");
            radioButtonItem12.Icon = null;
            radioButtonItem12.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem12.Tag = null;
            radioButtonItem12.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("You_Right");
            RdoHorzLabelPos.ButtonItems = new UserControls.RadioButtonItem[]
    {
    radioButtonItem11,
    radioButtonItem12
    };
            RdoHorzLabelPos.ButtonOffset = 10;
            RdoHorzLabelPos.ButtonTextColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RdoHorzLabelPos.ChoosedButtonColor = System.Drawing.Color.FromArgb(40, 71, 193);
            RdoHorzLabelPos.ChoosedButtonIndex = 0;
            RdoHorzLabelPos.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoHorzLabelPos.ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            RdoHorzLabelPos.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoHorzLabelPos.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoHorzLabelPos.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoHorzLabelPos.Height = 30;
            RdoHorzLabelPos.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoHorzLabelPos.Location = new System.Drawing.Point(10, 114);
            RdoHorzLabelPos.Name = "RdoHorzLabelPos";
            RdoHorzLabelPos.Size = new System.Drawing.Size(180, 30);
            RdoHorzLabelPos.StyleFlags = UserControls.Style.StyleFlag.None;
            RdoHorzLabelPos.StylizeFlag = true;
            RdoHorzLabelPos.TabIndex = 5;
            RdoHorzLabelPos.IndexChanged += RdoHorzTickLabel_IndexChanged;
            // 
            // LblGridIntensity
            // 
            LblGridIntensity.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblGridIntensity.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblGridIntensity.BorderThickness = 0;
            LblGridIntensity.CornerRadius = 0;
            LblGridIntensity.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblGridIntensity.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblGridIntensity.HighlightPrompt = defaultHighlightPrompt9;
            LblGridIntensity.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblGridIntensity.Location = new System.Drawing.Point(250, 160);
            LblGridIntensity.MultyLineFlag = false;
            LblGridIntensity.Name = "LblGridIntensity";
            LblGridIntensity.Size = new System.Drawing.Size(180, 18);
            LblGridIntensity.StyleFlags = UserControls.Style.StyleFlag.None;
            LblGridIntensity.StylizeFlag = true;
            LblGridIntensity.TabIndex = 10;
            LblGridIntensity.TabStop = false;
            LblGridIntensity.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WangGeLiangDu");
            LblGridIntensity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblGridIntensity.Token = null;
            // 
            // LblGridStyle
            // 
            LblGridStyle.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblGridStyle.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblGridStyle.BorderThickness = 0;
            LblGridStyle.CornerRadius = 0;
            LblGridStyle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblGridStyle.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblGridStyle.HighlightPrompt = defaultHighlightPrompt10;
            LblGridStyle.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblGridStyle.Location = new System.Drawing.Point(10, 160);
            LblGridStyle.MultyLineFlag = false;
            LblGridStyle.Name = "LblGridStyle";
            LblGridStyle.Size = new System.Drawing.Size(180, 18);
            LblGridStyle.StyleFlags = UserControls.Style.StyleFlag.None;
            LblGridStyle.StylizeFlag = true;
            LblGridStyle.TabIndex = 8;
            LblGridStyle.TabStop = false;
            LblGridStyle.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WangGeYangShi");
            LblGridStyle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblGridStyle.Token = null;
            // 
            // LblWaveIntensity
            // 
            LblWaveIntensity.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblWaveIntensity.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblWaveIntensity.BorderThickness = 0;
            LblWaveIntensity.CornerRadius = 0;
            LblWaveIntensity.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblWaveIntensity.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblWaveIntensity.HighlightPrompt = defaultHighlightPrompt11;
            LblWaveIntensity.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblWaveIntensity.Location = new System.Drawing.Point(250, 234);
            LblWaveIntensity.MultyLineFlag = false;
            LblWaveIntensity.Name = "LblWaveIntensity";
            LblWaveIntensity.Size = new System.Drawing.Size(180, 18);
            LblWaveIntensity.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblWaveIntensity.StylizeFlag = true;
            LblWaveIntensity.TabIndex = 14;
            LblWaveIntensity.TabStop = false;
            LblWaveIntensity.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BoXingLiangDu");
            LblWaveIntensity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblWaveIntensity.Token = null;
            // 
            // LblWaveStyle
            // 
            LblWaveStyle.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblWaveStyle.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblWaveStyle.BorderThickness = 0;
            LblWaveStyle.CornerRadius = 0;
            LblWaveStyle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblWaveStyle.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblWaveStyle.HighlightPrompt = defaultHighlightPrompt12;
            LblWaveStyle.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblWaveStyle.Location = new System.Drawing.Point(10, 234);
            LblWaveStyle.MultyLineFlag = false;
            LblWaveStyle.Name = "LblWaveStyle";
            LblWaveStyle.Size = new System.Drawing.Size(180, 18);
            LblWaveStyle.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblWaveStyle.StylizeFlag = true;
            LblWaveStyle.TabIndex = 12;
            LblWaveStyle.TabStop = false;
            LblWaveStyle.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BoXingYangShi");
            LblWaveStyle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblWaveStyle.Token = null;
            // 
            // LblHorzLabelPos
            // 
            LblHorzLabelPos.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblHorzLabelPos.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblHorzLabelPos.BorderThickness = 0;
            LblHorzLabelPos.CornerRadius = 0;
            LblHorzLabelPos.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblHorzLabelPos.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblHorzLabelPos.HighlightPrompt = defaultHighlightPrompt13;
            LblHorzLabelPos.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblHorzLabelPos.Location = new System.Drawing.Point(10, 86);
            LblHorzLabelPos.MultyLineFlag = false;
            LblHorzLabelPos.Name = "LblHorzLabelPos";
            LblHorzLabelPos.Size = new System.Drawing.Size(180, 18);
            LblHorzLabelPos.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblHorzLabelPos.StylizeFlag = true;
            LblHorzLabelPos.TabIndex = 4;
            LblHorzLabelPos.TabStop = false;
            LblHorzLabelPos.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChuiZhiBiaoJiWeiZhi");
            LblHorzLabelPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblHorzLabelPos.Token = null;
            // 
            // LblVertLabelPos
            // 
            LblVertLabelPos.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblVertLabelPos.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblVertLabelPos.BorderThickness = 0;
            LblVertLabelPos.CornerRadius = 0;
            LblVertLabelPos.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblVertLabelPos.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblVertLabelPos.HighlightPrompt = defaultHighlightPrompt14;
            LblVertLabelPos.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblVertLabelPos.Location = new System.Drawing.Point(250, 86);
            LblVertLabelPos.MultyLineFlag = false;
            LblVertLabelPos.Name = "LblVertLabelPos";
            LblVertLabelPos.Size = new System.Drawing.Size(180, 18);
            LblVertLabelPos.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblVertLabelPos.StylizeFlag = true;
            LblVertLabelPos.TabIndex = 6;
            LblVertLabelPos.TabStop = false;
            LblVertLabelPos.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuiPingBiaoJiWeiZhi");
            LblVertLabelPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblVertLabelPos.Token = null;
            // 
            // ChkTickLabel
            // 
            ChkTickLabel.AnimationCount = 8;
            ChkTickLabel.AnimationFunc = null;
            ChkTickLabel.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkTickLabel.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkTickLabel.BorderThickness = 0;
            ChkTickLabel.Checked = false;
            ChkTickLabel.CheckedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            ChkTickLabel.CheckedForeColor = System.Drawing.Color.Black;
            ChkTickLabel.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkTickLabel.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkTickLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkTickLabel.DropKey = System.Windows.Forms.Keys.Space;
            ChkTickLabel.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkTickLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkTickLabel.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkTickLabel.Height = 30;
            ChkTickLabel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkTickLabel.Location = new System.Drawing.Point(10, 34);
            ChkTickLabel.Margin = new System.Windows.Forms.Padding(0);
            ChkTickLabel.Name = "ChkTickLabel";
            ChkTickLabel.Size = new System.Drawing.Size(75, 30);
            ChkTickLabel.SliderButtonWidth = 30;
            ChkTickLabel.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkTickLabel.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            ChkTickLabel.StylizeFlag = true;
            ChkTickLabel.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            ChkTickLabel.TabIndex = 1;
            ChkTickLabel.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkTickLabel.UseAnimation = true;
            ChkTickLabel.CheckedChangedEvent += ChkTickLabel_CheckedChangedEvent;
            // 
            // LblTickLabel
            // 
            LblTickLabel.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblTickLabel.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblTickLabel.BorderThickness = 0;
            LblTickLabel.CornerRadius = 0;
            LblTickLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblTickLabel.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblTickLabel.HighlightPrompt = defaultHighlightPrompt15;
            LblTickLabel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTickLabel.Location = new System.Drawing.Point(10, 6);
            LblTickLabel.MultyLineFlag = false;
            LblTickLabel.Name = "LblTickLabel";
            LblTickLabel.Size = new System.Drawing.Size(140, 18);
            LblTickLabel.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblTickLabel.StylizeFlag = true;
            LblTickLabel.TabIndex = 0;
            LblTickLabel.TabStop = false;
            LblTickLabel.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShiBiaoJi");
            LblTickLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblTickLabel.Token = null;
            // 
            // LblPersist
            // 
            LblPersist.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblPersist.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblPersist.BorderThickness = 0;
            LblPersist.CornerRadius = 0;
            LblPersist.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblPersist.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblPersist.HighlightPrompt = defaultHighlightPrompt16;
            LblPersist.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblPersist.Location = new System.Drawing.Point(250, 6);
            LblPersist.MultyLineFlag = false;
            LblPersist.Name = "LblPersist";
            LblPersist.Size = new System.Drawing.Size(137, 18);
            LblPersist.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblPersist.StylizeFlag = true;
            LblPersist.TabIndex = 2;
            LblPersist.TabStop = false;
            LblPersist.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YuHui");
            LblPersist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblPersist.Token = null;
            // 
            // UtbGridIntensity
            // 
            UtbGridIntensity.DcimalDigits = 0;
            UtbGridIntensity.IsShowTips = true;
            UtbGridIntensity.LineColor = System.Drawing.Color.FromArgb(80, 80, 80);
            UtbGridIntensity.LineWidth = 10F;
            UtbGridIntensity.Location = new System.Drawing.Point(250, 188);
            UtbGridIntensity.MaxValue = 100F;
            UtbGridIntensity.MinValue = 0F;
            UtbGridIntensity.Name = "UtbGridIntensity";
            UtbGridIntensity.Size = new System.Drawing.Size(180, 30);
            UtbGridIntensity.TabIndex = 11;
            UtbGridIntensity.Text = "userTrackBar1";
            UtbGridIntensity.TipsFormat = null;
            UtbGridIntensity.Value = 0F;
            UtbGridIntensity.ValueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            // 
            // UtbWaveIntensity
            // 
            UtbWaveIntensity.DcimalDigits = 0;
            UtbWaveIntensity.IsShowTips = true;
            UtbWaveIntensity.LineColor = System.Drawing.Color.FromArgb(80, 80, 80);
            UtbWaveIntensity.LineWidth = 10F;
            UtbWaveIntensity.Location = new System.Drawing.Point(250, 262);
            UtbWaveIntensity.MaxValue = 100F;
            UtbWaveIntensity.MinValue = 0F;
            UtbWaveIntensity.Name = "UtbWaveIntensity";
            UtbWaveIntensity.Size = new System.Drawing.Size(180, 30);
            UtbWaveIntensity.TabIndex = 15;
            UtbWaveIntensity.Text = "userTrackBar2";
            UtbWaveIntensity.TipsFormat = null;
            UtbWaveIntensity.Value = 0F;
            UtbWaveIntensity.ValueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            // 
            // CbxGridStyle
            // 
            CbxGridStyle.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxGridStyle.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxGridStyle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxGridStyle.DataSource = null;
            CbxGridStyle.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxGridStyle.ForeColor = System.Drawing.Color.White;
            CbxGridStyle.Items= new string[] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Man"), ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JianDan"), ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Wu") };
            CbxGridStyle.Location = new System.Drawing.Point(10, 188);
            CbxGridStyle.Name = "CbxGridStyle";
            CbxGridStyle.Size = new System.Drawing.Size(180, 30);
            CbxGridStyle.TabIndex = 16;
            CbxGridStyle.SelectedIndexChanged += CbxGridStyle_SelectedIndexChanged;
            // 
            // CbxPersist
            // 
            CbxPersist.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxPersist.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxPersist.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxPersist.DataSource = null;
            CbxPersist.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxPersist.ForeColor = System.Drawing.Color.White;
            CbxPersist.Items = new string[] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBi"), ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZiDong")/*"自动"*/, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("NormalViewSettingPage_WuXian")/*"无限"*/ };
            CbxPersist.Location = new System.Drawing.Point(250, 34);
            CbxPersist.Name = "CbxPersist";
            CbxPersist.Size = new System.Drawing.Size(137, 30);
            CbxPersist.TabIndex = 17;
            CbxPersist.SelectedIndexChanged += CbxPersist_SelectedIndexChanged;
            // 
            // NormalViewSettingPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(CbxPersist);
            Controls.Add(CbxGridStyle);
            Controls.Add(UtbWaveIntensity);
            Controls.Add(UtbGridIntensity);
            Controls.Add(LblPersist);
            Controls.Add(ChkTickLabel);
            Controls.Add(LblTickLabel);
            Controls.Add(RdoWaveStyle);
            Controls.Add(RdoVertLabelPos);
            Controls.Add(RdoHorzLabelPos);
            Controls.Add(LblGridIntensity);
            Controls.Add(LblGridStyle);
            Controls.Add(LblWaveIntensity);
            Controls.Add(LblWaveStyle);
            Controls.Add(LblHorzLabelPos);
            Controls.Add(LblVertLabelPos);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "NormalViewSettingPage";
            Size = new System.Drawing.Size(440, 311);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.UIRadioButtonGroup RdoWaveStyle;
        private ScopeX.UserControls.UIRadioButtonGroup RdoVertLabelPos;
        private ScopeX.UserControls.UIRadioButtonGroup RdoHorzLabelPos;
        private ScopeX.UserControls.ScopeXLabel LblGridIntensity;
        private ScopeX.UserControls.ScopeXLabel LblGridStyle;
        private ScopeX.UserControls.ScopeXLabel LblWaveIntensity;
        private ScopeX.UserControls.ScopeXLabel LblWaveStyle;
        private ScopeX.UserControls.ScopeXLabel LblHorzLabelPos;
        private ScopeX.UserControls.ScopeXLabel LblVertLabelPos;
        private ScopeX.UserControls.ScopeXSwitchButton ChkTickLabel;
        private ScopeX.UserControls.ScopeXLabel LblTickLabel;
        private ScopeX.UserControls.ScopeXLabel LblPersist;
        private UserTrackBar UtbGridIntensity;
        private UserTrackBar UtbWaveIntensity;
        private ScopeX.UserControls.SelectComboBox CbxGridStyle;
        private ScopeX.UserControls.SelectComboBox CbxPersist;
    }
}
