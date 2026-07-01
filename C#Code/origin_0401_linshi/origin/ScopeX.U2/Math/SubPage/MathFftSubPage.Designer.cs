
using ScopeX.MathExt;

namespace ScopeX.U2
{
    partial class MathFftSubPage
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
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt6 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt7 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt8 = new UserControls.DefaultHighlightPrompt();
            CbxResultType = new ScopeX.UserControls.SelectComboBox();
            LblResultType = new UserControls.ScopeXLabel();
            LblNumber = new UserControls.ScopeXLabel();
            CbxWindowType = new ScopeX.UserControls.SelectComboBox();
            LblWindowType = new UserControls.ScopeXLabel();
            CbxSource = new ScopeX.UserControls.SelectComboBox();
            LblSource = new UserControls.ScopeXLabel();
            LblPeakMarks = new UserControls.ScopeXLabel();
            ChkPeakMarks = new UserControls.ScopeXSwitchButton();
            GrpFrequncy = new UserControls.ScopeXGroupBox();
            NebFrequencyValueSpan = new UserControls.TouchNeb();
            NebFrequencyValueCenter = new UserControls.TouchNeb();
            LblSpan = new UserControls.ScopeXLabel();
            LblCenter = new UserControls.ScopeXLabel();
            LblUnit = new UserControls.ScopeXLabel();
            CbxUnit = new ScopeX.UserControls.SelectComboBox();
            CbxNumber = new ScopeX.UserControls.SelectComboBox();
            BtnConfig = new ScopeX.UserControls.ScopeXIconButton();
            GrpFrequncy.SuspendLayout();
            SuspendLayout();
			// 
            // CbxResultType
            // 
            CbxResultType.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxResultType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxResultType.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxResultType.DataSource = null;
            CbxResultType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxResultType.ForeColor = System.Drawing.Color.White;
            CbxResultType.Items = new string[] {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage($"FFTResultOpt_{FFTResultOpt.Ampltd}"), // 幅度谱
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage($"FFTResultOpt_{FFTResultOpt.Power}"),// 功率谱
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage($"FFTResultOpt_{FFTResultOpt.Psd}"),// Psd
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage($"FFTResultOpt_{FFTResultOpt.Real}"),// 实部
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage($"FFTResultOpt_{FFTResultOpt.Imaginary}"),// 虚部
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage($"FFTResultOpt_{FFTResultOpt.Phase}") // 相位谱
            };
            CbxResultType.Location = new System.Drawing.Point(155, 35);
            CbxResultType.Name = "CbxResultType";
            CbxResultType.Size = new System.Drawing.Size(150, 30);
            CbxResultType.TabIndex = 24;
            CbxResultType.SelectedIndexChanged += CbxResultType_SelectedIndexChanged;
            // 
            // LblResultType
            // 
            LblResultType.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblResultType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblResultType.BorderThickness = 0;
            LblResultType.CornerRadius = 0;
            LblResultType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblResultType.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblResultType.HighlightPrompt = defaultHighlightPrompt1;
            LblResultType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblResultType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblResultType.Location = new System.Drawing.Point(155, 7);
            LblResultType.MultyLineFlag = false;
            LblResultType.Name = "LblResultType";
            LblResultType.Size = new System.Drawing.Size(122, 22);
            LblResultType.StyleFlags = UserControls.Style.StyleFlag.None;
            LblResultType.StylizeFlag = true;
            LblResultType.TabIndex = 8;
            LblResultType.TabStop = false;
            LblResultType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathFftSubPage.LblResultType"); // "输出类型";
            LblResultType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblResultType.Token = null;
            // 
            // LblNumber
            // 
            LblNumber.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblNumber.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblNumber.BorderThickness = 0;
            LblNumber.CornerRadius = 0;
            LblNumber.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblNumber.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblNumber.HighlightPrompt = defaultHighlightPrompt2;
            LblNumber.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblNumber.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblNumber.Location = new System.Drawing.Point(155, 78);
            LblNumber.MultyLineFlag = false;
            LblNumber.Name = "LblNumber";
            LblNumber.Size = new System.Drawing.Size(115, 22);
            LblNumber.StyleFlags = UserControls.Style.StyleFlag.None;
            LblNumber.StylizeFlag = true;
            LblNumber.TabIndex = 6;
            LblNumber.TabStop = false;
            LblNumber.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathFftSubPage.LblNumber"); // "点数";
            LblNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblNumber.Token = null;
            // 
            // CbxWindowType
            // 
            CbxWindowType.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxWindowType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxWindowType.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxWindowType.DataSource = null;
            CbxWindowType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxWindowType.ForeColor = System.Drawing.Color.White;
            CbxWindowType.Items=new string[] {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage($"WindowType_{WindowType.Rectangle}"), // "矩形窗",
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage($"WindowType_{WindowType.Hann}"), // "汉宁窗", 
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage($"WindowType_{WindowType.Hamming}"), // "汉明窗", 
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage($"WindowType_{WindowType.Blackman}"), // "布莱克曼窗", 
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage($"WindowType_{WindowType.Flattop}"), // "平顶窗" 
            };
          
            CbxWindowType.Location = new System.Drawing.Point(10, 106);
            CbxWindowType.Name = "CbxWindowType";
            CbxWindowType.Size = new System.Drawing.Size(120, 30);
            CbxWindowType.TabIndex = 26;
            CbxWindowType.SelectedIndexChanged += CbxWindowType_SelectedIndexChanged;
            // 
            // LblWindowType
            // 
            LblWindowType.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblWindowType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblWindowType.BorderThickness = 0;
            LblWindowType.CornerRadius = 0;
            LblWindowType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblWindowType.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblWindowType.HighlightPrompt = defaultHighlightPrompt3;
            LblWindowType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblWindowType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblWindowType.Location = new System.Drawing.Point(10, 78);
            LblWindowType.MultyLineFlag = false;
            LblWindowType.Name = "LblWindowType";
            LblWindowType.Size = new System.Drawing.Size(100, 22);
            LblWindowType.StyleFlags = UserControls.Style.StyleFlag.None;
            LblWindowType.StylizeFlag = true;
            LblWindowType.TabIndex = 2;
            LblWindowType.TabStop = false;
            LblWindowType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathFftSubPage.LblWindowType"); // "窗函数";
            LblWindowType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblWindowType.Token = null;
            // 
            // CbxSource
            // 
            CbxSource.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxSource.DataSource = null;
            CbxSource.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxSource.ForeColor = System.Drawing.Color.White;
            CbxSource.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4",
    "R1",
    "R2",
    "R3",
    "R4",
    "M1",
    "M2",
    "M3",
    "M4",
    "M5",
    "M6",
    "M7",
    "M8"
    };
            CbxSource.Location = new System.Drawing.Point(10, 35);
            CbxSource.Name = "CbxSource";
            CbxSource.Size = new System.Drawing.Size(120, 30);
            CbxSource.TabIndex = 23;
            // 
            // LblSource
            // 
            LblSource.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblSource.BorderThickness = 0;
            LblSource.CornerRadius = 0;
            LblSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSource.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblSource.HighlightPrompt = defaultHighlightPrompt4;
            LblSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSource.Location = new System.Drawing.Point(10, 7);
            LblSource.MultyLineFlag = false;
            LblSource.Name = "LblSource";
            LblSource.Size = new System.Drawing.Size(100, 22);
            LblSource.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSource.StylizeFlag = true;
            LblSource.TabIndex = 0;
            LblSource.TabStop = false;
            LblSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathFftSubPage.LblSource"); // "源";
            LblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSource.Token = null;
            // 
            // LblPeakMarks
            // 
            LblPeakMarks.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblPeakMarks.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblPeakMarks.BorderThickness = 0;
            LblPeakMarks.CornerRadius = 0;
            LblPeakMarks.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblPeakMarks.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblPeakMarks.HighlightPrompt = defaultHighlightPrompt5;
            LblPeakMarks.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblPeakMarks.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblPeakMarks.Location = new System.Drawing.Point(392, 72);
            LblPeakMarks.MultyLineFlag = false;
            LblPeakMarks.Name = "LblPeakMarks";
            LblPeakMarks.Size = new System.Drawing.Size(73, 22);
            LblPeakMarks.StyleFlags = UserControls.Style.StyleFlag.None;
            LblPeakMarks.StylizeFlag = true;
            LblPeakMarks.TabIndex = 10;
            LblPeakMarks.TabStop = false;
            LblPeakMarks.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathFftSubPage.LblPeakMarks"); // "峰值标记";
            LblPeakMarks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblPeakMarks.Token = null;
            LblPeakMarks.Visible = false;
            // 
            // ChkPeakMarks
            // 
            ChkPeakMarks.AnimationCount = 8;
            ChkPeakMarks.AnimationFunc = null;
            ChkPeakMarks.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkPeakMarks.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkPeakMarks.BorderThickness = 0;
            ChkPeakMarks.Checked = false;
            ChkPeakMarks.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkPeakMarks.CheckedForeColor = System.Drawing.Color.Black;
            ChkPeakMarks.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkPeakMarks.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathFftSubPage.ChkPeakMarks","CheckedText"); // "开";
            ChkPeakMarks.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkPeakMarks.DropKey = System.Windows.Forms.Keys.Space;
            ChkPeakMarks.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkPeakMarks.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkPeakMarks.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkPeakMarks.Height = 30;
            ChkPeakMarks.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkPeakMarks.Location = new System.Drawing.Point(392, 106);
            ChkPeakMarks.Margin = new System.Windows.Forms.Padding(0);
            ChkPeakMarks.Name = "ChkPeakMarks";
            ChkPeakMarks.Size = new System.Drawing.Size(75, 30);
            ChkPeakMarks.SliderButtonWidth = 30;
            ChkPeakMarks.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkPeakMarks.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkPeakMarks.StylizeFlag = true;
            ChkPeakMarks.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkPeakMarks.TabIndex = 11;
            ChkPeakMarks.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathFftSubPage.ChkPeakMarks"); // "关";
            ChkPeakMarks.UseAnimation = true;
            ChkPeakMarks.Visible = false;
            // 
            // GrpFrequncy
            // 
            GrpFrequncy.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            GrpFrequncy.BorderThickness = 1U;
            GrpFrequncy.Controls.Add(NebFrequencyValueSpan);
            GrpFrequncy.Controls.Add(NebFrequencyValueCenter);
            GrpFrequncy.Controls.Add(LblSpan);
            GrpFrequncy.Controls.Add(LblCenter);
            GrpFrequncy.Font = new System.Drawing.Font("MiSans", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            GrpFrequncy.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            GrpFrequncy.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            GrpFrequncy.Location = new System.Drawing.Point(-3, 146);
            GrpFrequncy.Name = "GrpFrequncy";
            GrpFrequncy.Size = new System.Drawing.Size(437, 116);
            GrpFrequncy.StyleFlags = UserControls.Style.StyleFlag.None;
            GrpFrequncy.StylizeFlag = false;
            GrpFrequncy.TabIndex = 21;
            GrpFrequncy.TabStop = false;
            GrpFrequncy.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathFftSubPage.GrpFrequncy"); // "频率范围";
            // 
            // NebFrequencyValueSpan
            // 
            NebFrequencyValueSpan.AddButtonImg = null;
            buttonBaseStyle1.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle1.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle1.BorderThickness = 0;
            buttonBaseStyle1.ForeColor = System.Drawing.Color.White;
            buttonStyle1.MouseClickStyle = buttonBaseStyle1;
            buttonBaseStyle2.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle2.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle2.BorderThickness = 0;
            buttonBaseStyle2.ForeColor = System.Drawing.Color.White;
            buttonStyle1.MouseInStyle = buttonBaseStyle2;
            buttonBaseStyle3.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle3.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle3.BorderThickness = 0;
            buttonBaseStyle3.ForeColor = System.Drawing.Color.White;
            buttonStyle1.NomalStyle = buttonBaseStyle3;
            NebFrequencyValueSpan.AddButtonStyle = buttonStyle1;
            NebFrequencyValueSpan.AllwaysShowFocusImage = false;
            NebFrequencyValueSpan.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebFrequencyValueSpan.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebFrequencyValueSpan.BorderThickness = 0;
            NebFrequencyValueSpan.DisableHoldOnInput = false;
            NebFrequencyValueSpan.DropKey = System.Windows.Forms.Keys.Space;
            NebFrequencyValueSpan.FocusBoederColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebFrequencyValueSpan.FocusForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            NebFrequencyValueSpan.FocusImage = null;
            NebFrequencyValueSpan.FocusImagePosition = UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebFrequencyValueSpan.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            NebFrequencyValueSpan.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            NebFrequencyValueSpan.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            NebFrequencyValueSpan.Height = 30;
            NebFrequencyValueSpan.HoldOnSpeedLevel = 10;
            NebFrequencyValueSpan.IconWidthProportion = 1F;
            NebFrequencyValueSpan.Interval = 0.1D;
            NebFrequencyValueSpan.LanguageKey = null;
            NebFrequencyValueSpan.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebFrequencyValueSpan.Location = new System.Drawing.Point(160, 73);
            NebFrequencyValueSpan.MaxValue = double.MaxValue;
            NebFrequencyValueSpan.MinValue = double.MinValue;
            NebFrequencyValueSpan.Name = "NebFrequencyValueSpan";
            NebFrequencyValueSpan.Size = new System.Drawing.Size(264, 30);
            NebFrequencyValueSpan.StringFormatFunc = null;
            NebFrequencyValueSpan.StyleFlags = UserControls.Style.StyleFlag.None;
            NebFrequencyValueSpan.StylizeFlag = true;
            NebFrequencyValueSpan.SubButtonImg = null;
            buttonBaseStyle4.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle4.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle4.BorderThickness = 0;
            buttonBaseStyle4.ForeColor = System.Drawing.Color.White;
            buttonStyle2.MouseClickStyle = buttonBaseStyle4;
            buttonBaseStyle5.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle5.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle5.BorderThickness = 0;
            buttonBaseStyle5.ForeColor = System.Drawing.Color.White;
            buttonStyle2.MouseInStyle = buttonBaseStyle5;
            buttonBaseStyle6.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle6.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle6.BorderThickness = 0;
            buttonBaseStyle6.ForeColor = System.Drawing.Color.White;
            buttonStyle2.NomalStyle = buttonBaseStyle6;
            NebFrequencyValueSpan.SubButtonStyle = buttonStyle2;
            NebFrequencyValueSpan.TabIndex = 23;
            NebFrequencyValueSpan.Value = 0D;
            // 
            // NebFrequencyValueCenter
            // 
            NebFrequencyValueCenter.AddButtonImg = null;
            buttonBaseStyle7.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle7.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle7.BorderThickness = 0;
            buttonBaseStyle7.ForeColor = System.Drawing.Color.White;
            buttonStyle3.MouseClickStyle = buttonBaseStyle7;
            buttonBaseStyle8.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle8.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle8.BorderThickness = 0;
            buttonBaseStyle8.ForeColor = System.Drawing.Color.White;
            buttonStyle3.MouseInStyle = buttonBaseStyle8;
            buttonBaseStyle9.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle9.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle9.BorderThickness = 0;
            buttonBaseStyle9.ForeColor = System.Drawing.Color.White;
            buttonStyle3.NomalStyle = buttonBaseStyle9;
            NebFrequencyValueCenter.AddButtonStyle = buttonStyle3;
            NebFrequencyValueCenter.AllwaysShowFocusImage = false;
            NebFrequencyValueCenter.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebFrequencyValueCenter.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebFrequencyValueCenter.BorderThickness = 0;
            NebFrequencyValueCenter.DisableHoldOnInput = false;
            NebFrequencyValueCenter.DropKey = System.Windows.Forms.Keys.Space;
            NebFrequencyValueCenter.FocusBoederColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebFrequencyValueCenter.FocusForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            NebFrequencyValueCenter.FocusImage = null;
            NebFrequencyValueCenter.FocusImagePosition = UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebFrequencyValueCenter.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            NebFrequencyValueCenter.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            NebFrequencyValueCenter.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            NebFrequencyValueCenter.Height = 30;
            NebFrequencyValueCenter.HoldOnSpeedLevel = 10;
            NebFrequencyValueCenter.IconWidthProportion = 1F;
            NebFrequencyValueCenter.Interval = 0.1D;
            NebFrequencyValueCenter.LanguageKey = null;
            NebFrequencyValueCenter.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebFrequencyValueCenter.Location = new System.Drawing.Point(160, 29);
            NebFrequencyValueCenter.MaxValue = double.MaxValue;
            NebFrequencyValueCenter.MinValue = double.MinValue;
            NebFrequencyValueCenter.Name = "NebFrequencyValueCenter";
            NebFrequencyValueCenter.Size = new System.Drawing.Size(264, 30);
            NebFrequencyValueCenter.StringFormatFunc = null;
            NebFrequencyValueCenter.StyleFlags = UserControls.Style.StyleFlag.None;
            NebFrequencyValueCenter.StylizeFlag = true;
            NebFrequencyValueCenter.SubButtonImg = null;
            buttonBaseStyle10.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle10.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle10.BorderThickness = 0;
            buttonBaseStyle10.ForeColor = System.Drawing.Color.White;
            buttonStyle4.MouseClickStyle = buttonBaseStyle10;
            buttonBaseStyle11.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle11.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle11.BorderThickness = 0;
            buttonBaseStyle11.ForeColor = System.Drawing.Color.White;
            buttonStyle4.MouseInStyle = buttonBaseStyle11;
            buttonBaseStyle12.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle12.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle12.BorderThickness = 0;
            buttonBaseStyle12.ForeColor = System.Drawing.Color.White;
            buttonStyle4.NomalStyle = buttonBaseStyle12;
            NebFrequencyValueCenter.SubButtonStyle = buttonStyle4;
            NebFrequencyValueCenter.TabIndex = 22;
            NebFrequencyValueCenter.Value = 0D;
            // 
            // LblSpan
            // 
            LblSpan.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblSpan.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblSpan.BorderThickness = 0;
            LblSpan.CornerRadius = 0;
            LblSpan.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSpan.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblSpan.HighlightPrompt = defaultHighlightPrompt6;
            LblSpan.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblSpan.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSpan.Location = new System.Drawing.Point(51, 73);
            LblSpan.MultyLineFlag = false;
            LblSpan.Name = "LblSpan";
            LblSpan.Size = new System.Drawing.Size(115, 22);
            LblSpan.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSpan.StylizeFlag = true;
            LblSpan.TabIndex = 6;
            LblSpan.TabStop = false;
            LblSpan.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathFftSubPage.GrpFrequncy.LblSpan"); // "跨度";
            LblSpan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSpan.Token = null;
            // 
            // LblCenter
            // 
            LblCenter.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblCenter.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblCenter.BorderThickness = 0;
            LblCenter.CornerRadius = 0;
            LblCenter.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblCenter.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblCenter.HighlightPrompt = defaultHighlightPrompt7;
            LblCenter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblCenter.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblCenter.Location = new System.Drawing.Point(51, 29);
            LblCenter.MultyLineFlag = false;
            LblCenter.Name = "LblCenter";
            LblCenter.Size = new System.Drawing.Size(135, 22);
            LblCenter.StyleFlags = UserControls.Style.StyleFlag.None;
            LblCenter.StylizeFlag = true;
            LblCenter.TabIndex = 6;
            LblCenter.TabStop = false;
            LblCenter.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathFftSubPage.GrpFrequncy.LblCenter"); // "中心频率";
            LblCenter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblCenter.Token = null;
            // 
            // LblUnit
            // 
            LblUnit.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblUnit.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblUnit.BorderThickness = 0;
            LblUnit.CornerRadius = 0;
            LblUnit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblUnit.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblUnit.HighlightPrompt = defaultHighlightPrompt8;
            LblUnit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblUnit.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblUnit.Location = new System.Drawing.Point(331, 7);
            LblUnit.MultyLineFlag = false;
            LblUnit.Name = "LblUnit";
            LblUnit.Size = new System.Drawing.Size(115, 22);
            LblUnit.StyleFlags = UserControls.Style.StyleFlag.None;
            LblUnit.StylizeFlag = true;
            LblUnit.TabIndex = 8;
            LblUnit.TabStop = false;
            LblUnit.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathFftSubPage.LblUnit"); // "单位";
            LblUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblUnit.Token = null;
            // 
            // CbxUnit
            // 
            CbxUnit.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxUnit.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxUnit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxUnit.DataSource = null;
            CbxUnit.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxUnit.ForeColor = System.Drawing.Color.White;
            CbxUnit.Items = new string[]
    {
    "Vrms",
    "dBm",
    "dBμW",
    "dBmV",
    "dBμV",
    "dBmA",
    "dBμA"
    };
            CbxUnit.Location = new System.Drawing.Point(331, 35);
            CbxUnit.Name = "CbxUnit";
            CbxUnit.Size = new System.Drawing.Size(100, 30);
            CbxUnit.TabIndex = 25;
            CbxUnit.SelectedIndexChanged += CbxUnit_SelectedIndexChanged;
            // 
            // CbxNumber
            // 
            CbxNumber.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxNumber.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxNumber.DataSource = null;
            CbxNumber.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxNumber.ForeColor = System.Drawing.Color.White;
            CbxNumber.Items = null;
            CbxNumber.Location = new System.Drawing.Point(155, 106);
            CbxNumber.Name = "CbxNumber";
            CbxNumber.Size = new System.Drawing.Size(100, 30);
            CbxNumber.TabIndex = 27;
            // 
            // BtnConfig
            // 
            BtnConfig.Anchor = System.Windows.Forms.AnchorStyles.Left;
            BtnConfig.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnConfig.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnConfig.BorderThickness = 0;
            BtnConfig.CornerRadius = 0;
            BtnConfig.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnConfig.DaskArray = null;
            BtnConfig.DropKey = System.Windows.Forms.Keys.Space;
            BtnConfig.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnConfig.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnConfig.Height = 30;
            BtnConfig.Icon = null;
            BtnConfig.IconOffset = 10;
            BtnConfig.IconSize = new System.Drawing.Size(24, 24);
            BtnConfig.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnConfig.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnConfig.IsIndicatorShow = false;
            BtnConfig.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnConfig.Location = new System.Drawing.Point(330, 90);
            BtnConfig.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnConfig.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnConfig.MouseInBorderThickness = 0;
            BtnConfig.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnConfig.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnConfig.Name = "BtnConfig";
            BtnConfig.PressedBackColor = System.Drawing.Color.Gray;
            BtnConfig.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnConfig.PressedBorderThickness = 0;
            BtnConfig.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnConfig.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnConfig.Size = new System.Drawing.Size(98, 30);
            BtnConfig.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnConfig.StylizeFlag = true;
            BtnConfig.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnConfig.SVGPath = "";
            BtnConfig.TabIndex = 22;
            BtnConfig.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuWei");
            BtnConfig.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnConfig.Visible = true;
            BtnConfig.Click += BtnConfig_Click;
            // 
            // MathFftSubPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(CbxNumber);
            Controls.Add(GrpFrequncy);
            Controls.Add(ChkPeakMarks);
            Controls.Add(LblPeakMarks);
            Controls.Add(CbxUnit);
            Controls.Add(LblUnit);
            Controls.Add(CbxResultType);
            Controls.Add(LblResultType);
            Controls.Add(BtnConfig);
            Controls.Add(LblNumber);
            Controls.Add(CbxWindowType);
            Controls.Add(LblWindowType);
            Controls.Add(CbxSource);
            Controls.Add(LblSource);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "MathFftSubPage";
            Padding = new System.Windows.Forms.Padding(3);
            Size = new System.Drawing.Size(480, 271);
            GrpFrequncy.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.SelectComboBox CbxResultType;
        private ScopeX.UserControls.ScopeXLabel LblResultType;
        private ScopeX.UserControls.ScopeXLabel LblNumber;
        private ScopeX.UserControls.SelectComboBox CbxWindowType;
        private ScopeX.UserControls.ScopeXLabel LblWindowType;
        private ScopeX.UserControls.SelectComboBox CbxSource;
        private ScopeX.UserControls.ScopeXLabel LblSource;
        private ScopeX.UserControls.ScopeXLabel LblPeakMarks;
        private ScopeX.UserControls.ScopeXSwitchButton ChkPeakMarks;
        private ScopeX.UserControls.ScopeXGroupBox GrpFrequncy;
        private ScopeX.UserControls.UIRadioButtonGroup uiRadioButtonGroup2;
        private ScopeX.UserControls.UIRadioButtonGroup RdoFrequencyType;
        private ScopeX.UserControls.TouchNeb touchNeb1;
        private ScopeX.UserControls.TouchNeb NebVScale;
        private ScopeX.UserControls.TouchNeb NebFrequencyValueSpan;
        private ScopeX.UserControls.TouchNeb NebFrequencyValueCenter;
        private ScopeX.UserControls.ScopeXLabel LblSpan;
        private ScopeX.UserControls.ScopeXLabel LblCenter;
        private ScopeX.UserControls.ScopeXLabel LblUnit;
        private ScopeX.UserControls.SelectComboBox CbxUnit;
        private ScopeX.UserControls.SelectComboBox CbxNumber;
        private ScopeX.UserControls.ScopeXLabel LblConfig;
        private ScopeX.UserControls.ScopeXIconButton BtnConfig;
    }
}
