using ScopeX.UserControls;

namespace ScopeX.U2
{
    partial class PwrOnOffTimePage
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
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle11 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle31 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle32 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle33 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle12 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle34 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle35 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle36 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new ScopeX.UserControls.DefaultHighlightPrompt();
            LblActive = new ScopeX.UserControls.ScopeXLabel();
            ChkActive = new ScopeX.UserControls.ScopeXSwitchButton();
            LblOutVoltageSrc = new ScopeX.UserControls.ScopeXLabel();
            LblInVoltageSrc = new ScopeX.UserControls.ScopeXLabel();
            CbxOutVoltageSrc = new ScopeX.UserControls.SelectComboBox();
            CbxInVoltageSrc = new ScopeX.UserControls.SelectComboBox();
            LblInputPeakVoltage = new UserControls.ScopeXLabel();
            BtnInputPeakVoltage = new UserControls.ScopeXIconButton();
            LblOutputPeakVoltage = new UserControls.ScopeXLabel();
            BtnOutputPeakVoltage = new UserControls.ScopeXIconButton();
            LblConvertType = new ScopeX.UserControls.ScopeXLabel();
            CbxConvertType = new ScopeX.UserControls.SelectComboBox();
            LblTestType = new ScopeX.UserControls.ScopeXLabel();
            CbxTestType = new ScopeX.UserControls.SelectComboBox();
            LblAcquisitionTime = new UserControls.ScopeXLabel();
            BtnAcquisitionTime = new UserControls.ScopeXIconButton();
            BtnRun = new UserControls.ScopeXIconButton();
            BtnGuide = new ScopeX.UserControls.ScopeXIconButton();
            BtnResultTable = new ScopeX.UserControls.ScopeXIconButton();
            
            SuspendLayout();
            // 
            // LblActive
            // 
            LblActive.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblActive.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblActive.BorderThickness = 0;
            LblActive.CornerRadius = 0;
            LblActive.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblActive.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblActive.HighlightPrompt = defaultHighlightPrompt3;
            LblActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblActive.Location = new System.Drawing.Point(19, 16);
            LblActive.MultyLineFlag = false;
            LblActive.Name = "LblActive";
            LblActive.Size = new System.Drawing.Size(140, 18);
            LblActive.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblActive.StylizeFlag = true;
            LblActive.TabIndex = 75;
            LblActive.TabStop = false;
            LblActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShi");
            LblActive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblActive.Token = null;
            // 
            // ChkActive
            // 
            ChkActive.AnimationCount = 8;
            ChkActive.AnimationFunc = null;
            ChkActive.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkActive.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkActive.BorderThickness = 0;
            ChkActive.Checked = false;
            ChkActive.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkActive.CheckedForeColor = System.Drawing.Color.Black;
            ChkActive.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkActive.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkActive.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkActive.DropKey = System.Windows.Forms.Keys.Space;
            ChkActive.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkActive.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkActive.Height = 30;
            ChkActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkActive.Location = new System.Drawing.Point(19, 44);
            ChkActive.Margin = new System.Windows.Forms.Padding(0);
            ChkActive.Name = "ChkActive";
            ChkActive.Size = new System.Drawing.Size(75, 30);
            ChkActive.SliderButtonWidth = 30;
            ChkActive.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkActive.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            ChkActive.StylizeFlag = true;
            ChkActive.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            ChkActive.TabIndex = 74;
            ChkActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkActive.UseAnimation = true;
            ChkActive.CheckedChangedEvent += ChkActive_CheckedChangedEvent;
            // 
            // LblInVoltageSrc
            // 
            LblInVoltageSrc.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblInVoltageSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblInVoltageSrc.BorderThickness = 0;
            LblInVoltageSrc.CornerRadius = 0;
            LblInVoltageSrc.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblInVoltageSrc.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblInVoltageSrc.HighlightPrompt = defaultHighlightPrompt2;
            LblInVoltageSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblInVoltageSrc.Location = new System.Drawing.Point(196, 16);
            LblInVoltageSrc.MultyLineFlag = false;
            LblInVoltageSrc.Name = "LblInVoltageSrc";
            LblInVoltageSrc.Size = new System.Drawing.Size(140, 18);
            LblInVoltageSrc.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblInVoltageSrc.StylizeFlag = true;
            LblInVoltageSrc.TabIndex = 53;
            LblInVoltageSrc.TabStop = false;
            LblInVoltageSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuRuDianYaYuan");
            LblInVoltageSrc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblInVoltageSrc.Token = null;
            // 
            // CbxInVoltageSrc
            // 
            CbxInVoltageSrc.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxInVoltageSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxInVoltageSrc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxInVoltageSrc.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxInVoltageSrc.ForeColor = System.Drawing.Color.White;
            CbxInVoltageSrc.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4"
    };
            CbxInVoltageSrc.Location = new System.Drawing.Point(196, 44);
            CbxInVoltageSrc.Name = "CbxInVoltageSrc";
            CbxInVoltageSrc.Size = new System.Drawing.Size(140, 30);
            CbxInVoltageSrc.TabIndex = 94;
            // 
            // LblOutVoltageSrc
            // 
            LblOutVoltageSrc.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblOutVoltageSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblOutVoltageSrc.BorderThickness = 0;
            LblOutVoltageSrc.CornerRadius = 0;
            LblOutVoltageSrc.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblOutVoltageSrc.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblOutVoltageSrc.HighlightPrompt = defaultHighlightPrompt1;
            LblOutVoltageSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblOutVoltageSrc.Location = new System.Drawing.Point(373, 16);
            LblOutVoltageSrc.MultyLineFlag = false;
            LblOutVoltageSrc.Name = "LblOutVoltageSrc";
            LblOutVoltageSrc.Size = new System.Drawing.Size(140, 18);
            LblOutVoltageSrc.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblOutVoltageSrc.StylizeFlag = true;
            LblOutVoltageSrc.TabIndex = 55;
            LblOutVoltageSrc.TabStop = false;
            LblOutVoltageSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuChuDianYaYuan");
            LblOutVoltageSrc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblOutVoltageSrc.Token = null;
            // 
            // CbxOutVoltageSrc
            // 
            CbxOutVoltageSrc.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxOutVoltageSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxOutVoltageSrc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxOutVoltageSrc.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxOutVoltageSrc.ForeColor = System.Drawing.Color.White;
            CbxOutVoltageSrc.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4"
    };
            CbxOutVoltageSrc.Location = new System.Drawing.Point(373, 44);
            CbxOutVoltageSrc.Name = "CbxOutVoltageSrc";
            CbxOutVoltageSrc.Size = new System.Drawing.Size(140, 30);
            CbxOutVoltageSrc.TabIndex = 95;
            // 
            // LblTransType
            // 
            LblConvertType.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblConvertType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblConvertType.BorderThickness = 0;
            LblConvertType.CornerRadius = 0;
            LblConvertType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblConvertType.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblConvertType.HighlightPrompt = defaultHighlightPrompt2;
            LblConvertType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblConvertType.Location = new System.Drawing.Point(19, 224);
            LblConvertType.MultyLineFlag = false;
            LblConvertType.Name = "LblConvertType";
            LblConvertType.Size = new System.Drawing.Size(140, 18);
            LblConvertType.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblConvertType.StylizeFlag = true;
            LblConvertType.TabIndex = 53;
            LblConvertType.TabStop = false;
            LblConvertType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhuanHuanLeiXing");
            LblConvertType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblConvertType.Token = null;
            // 
            // CbxConvertType
            // 
            CbxConvertType.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxConvertType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxConvertType.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxConvertType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxConvertType.ForeColor = System.Drawing.Color.White;
            CbxConvertType.Location = new System.Drawing.Point(19, 252);
            CbxConvertType.Name = "CbxConvertType";
            CbxConvertType.Size = new System.Drawing.Size(140, 30);
            CbxConvertType.TabIndex = 94;
            // 
            // LblTestType
            // 
            LblTestType.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblTestType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblTestType.BorderThickness = 0;
            LblTestType.CornerRadius = 0;
            LblTestType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblTestType.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblTestType.HighlightPrompt = defaultHighlightPrompt2;
            LblTestType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTestType.Location = new System.Drawing.Point(373, 120);
            LblTestType.MultyLineFlag = false;
            LblTestType.Name = "LblTestType";
            LblTestType.Size = new System.Drawing.Size(140, 18);
            LblTestType.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblTestType.StylizeFlag = true;
            LblTestType.TabIndex = 53;
            LblTestType.TabStop = false;
            LblTestType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CeShiLeiXing");
            LblTestType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblTestType.Token = null;
            // 
            // CbxTestType
            // 
            CbxTestType.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxTestType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxTestType.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxTestType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxTestType.ForeColor = System.Drawing.Color.White;
            CbxTestType.Location = new System.Drawing.Point(373, 148);
            CbxTestType.Name = "CbxTestType";
            CbxTestType.Size = new System.Drawing.Size(140, 30);
            CbxTestType.TabIndex = 94;
            // 
            // LblInputPeakVoltage
            // 
            LblInputPeakVoltage.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblInputPeakVoltage.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblInputPeakVoltage.BorderThickness = 0;
            LblInputPeakVoltage.CornerRadius = 0;
            LblInputPeakVoltage.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblInputPeakVoltage.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblInputPeakVoltage.HighlightPrompt = defaultHighlightPrompt3;
            LblInputPeakVoltage.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblInputPeakVoltage.Location = new System.Drawing.Point(19, 120);
            LblInputPeakVoltage.MultyLineFlag = false;
            LblInputPeakVoltage.Name = "LblInputPeakVoltage";
            LblInputPeakVoltage.Size = new System.Drawing.Size(140, 18);
            LblInputPeakVoltage.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblInputPeakVoltage.StylizeFlag = true;
            LblInputPeakVoltage.TabIndex = 75;
            LblInputPeakVoltage.TabStop = false;
            LblInputPeakVoltage.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuRuFengZhiDianYa");
            LblInputPeakVoltage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblInputPeakVoltage.Token = null;
            // 
            // BtnInputPeakVoltage
            // 
            BtnInputPeakVoltage.BackColor = System.Drawing.Color.Transparent;
            BtnInputPeakVoltage.BorderColor = System.Drawing.Color.Black;
            BtnInputPeakVoltage.BorderThickness = 1;
            BtnInputPeakVoltage.CornerRadius = 0;
            BtnInputPeakVoltage.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnInputPeakVoltage.DaskArray = null;
            BtnInputPeakVoltage.DropKey = System.Windows.Forms.Keys.Space;
            BtnInputPeakVoltage.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnInputPeakVoltage.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnInputPeakVoltage.Height = 23;
            BtnInputPeakVoltage.Icon = null;
            BtnInputPeakVoltage.IconOffset = 10;
            BtnInputPeakVoltage.IconSize = new System.Drawing.Size(24, 24);
            BtnInputPeakVoltage.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnInputPeakVoltage.IsChoosed = false;
            BtnInputPeakVoltage.IsIndicatorShow = false;
            BtnInputPeakVoltage.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnInputPeakVoltage.Location = new System.Drawing.Point(19, 148);
            BtnInputPeakVoltage.MouseinBackColor = System.Drawing.Color.Transparent;
            BtnInputPeakVoltage.MouseinBorderColor = System.Drawing.Color.Blue;
            BtnInputPeakVoltage.MouseInBorderThickness = 1;
            BtnInputPeakVoltage.MouseinForeColor = System.Drawing.Color.Blue;
            BtnInputPeakVoltage.MouseinSvgForeColor = System.Drawing.Color.Blue;
            BtnInputPeakVoltage.Name = "BtnThreshold";
            BtnInputPeakVoltage.PressedBackColor = System.Drawing.Color.Gray;
            BtnInputPeakVoltage.PressedBorderColor = System.Drawing.Color.Blue;
            BtnInputPeakVoltage.PressedBorderThickness = 1;
            BtnInputPeakVoltage.PressedForeColor = System.Drawing.Color.Blue;
            BtnInputPeakVoltage.PressedSvgForeColor = System.Drawing.Color.Blue;
            BtnInputPeakVoltage.Size = new System.Drawing.Size(140, 30);
            BtnInputPeakVoltage.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            BtnInputPeakVoltage.StylizeFlag = true;
            BtnInputPeakVoltage.SVGForeColor = System.Drawing.Color.Black;
            BtnInputPeakVoltage.SVGPath = "";
            BtnInputPeakVoltage.TabIndex = 2;
            BtnInputPeakVoltage.Text = "BtnInputPeakVoltage";
            BtnInputPeakVoltage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnInputPeakVoltage.DoubleClick += BtnInPeakVoltage_Click;
            // 
            // LblOutputPeakVoltage
            // 
            LblOutputPeakVoltage.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblOutputPeakVoltage.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblOutputPeakVoltage.BorderThickness = 0;
            LblOutputPeakVoltage.CornerRadius = 0;
            LblOutputPeakVoltage.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblOutputPeakVoltage.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblOutputPeakVoltage.HighlightPrompt = defaultHighlightPrompt3;
            LblOutputPeakVoltage.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblOutputPeakVoltage.Location = new System.Drawing.Point(196, 120);
            LblOutputPeakVoltage.MultyLineFlag = false;
            LblOutputPeakVoltage.Name = "LblOutputPeakVoltage";
            LblOutputPeakVoltage.Size = new System.Drawing.Size(140, 18);
            LblOutputPeakVoltage.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblOutputPeakVoltage.StylizeFlag = true;
            LblOutputPeakVoltage.TabIndex = 75;
            LblOutputPeakVoltage.TabStop = false;
            LblOutputPeakVoltage.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuChuFengZhiDianYa");
            LblOutputPeakVoltage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblOutputPeakVoltage.Token = null;
            // 
            // BtnOutputPeakVoltage
            // 
            BtnOutputPeakVoltage.BackColor = System.Drawing.Color.Transparent;
            BtnOutputPeakVoltage.BorderColor = System.Drawing.Color.Black;
            BtnOutputPeakVoltage.BorderThickness = 1;
            BtnOutputPeakVoltage.CornerRadius = 0;
            BtnOutputPeakVoltage.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnOutputPeakVoltage.DaskArray = null;
            BtnOutputPeakVoltage.DropKey = System.Windows.Forms.Keys.Space;
            BtnOutputPeakVoltage.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnOutputPeakVoltage.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnOutputPeakVoltage.Height = 23;
            BtnOutputPeakVoltage.Icon = null;
            BtnOutputPeakVoltage.IconOffset = 10;
            BtnOutputPeakVoltage.IconSize = new System.Drawing.Size(24, 24);
            BtnOutputPeakVoltage.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnOutputPeakVoltage.IsChoosed = false;
            BtnOutputPeakVoltage.IsIndicatorShow = false;
            BtnOutputPeakVoltage.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnOutputPeakVoltage.Location = new System.Drawing.Point(196, 148);
            BtnOutputPeakVoltage.MouseinBackColor = System.Drawing.Color.Transparent;
            BtnOutputPeakVoltage.MouseinBorderColor = System.Drawing.Color.Blue;
            BtnOutputPeakVoltage.MouseInBorderThickness = 1;
            BtnOutputPeakVoltage.MouseinForeColor = System.Drawing.Color.Blue;
            BtnOutputPeakVoltage.MouseinSvgForeColor = System.Drawing.Color.Blue;
            BtnOutputPeakVoltage.Name = "BtnThreshold";
            BtnOutputPeakVoltage.PressedBackColor = System.Drawing.Color.Gray;
            BtnOutputPeakVoltage.PressedBorderColor = System.Drawing.Color.Blue;
            BtnOutputPeakVoltage.PressedBorderThickness = 1;
            BtnOutputPeakVoltage.PressedForeColor = System.Drawing.Color.Blue;
            BtnOutputPeakVoltage.PressedSvgForeColor = System.Drawing.Color.Blue;
            BtnOutputPeakVoltage.Size = new System.Drawing.Size(140, 30);
            BtnOutputPeakVoltage.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            BtnOutputPeakVoltage.StylizeFlag = true;
            BtnOutputPeakVoltage.SVGForeColor = System.Drawing.Color.Black;
            BtnOutputPeakVoltage.SVGPath = "";
            BtnOutputPeakVoltage.TabIndex = 2;
            BtnOutputPeakVoltage.Text = "BtnOutputPeakVoltage";
            BtnOutputPeakVoltage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnOutputPeakVoltage.DoubleClick += BtnOutPeakVoltage_Click;
            // 
            // LblAcquisitionTime
            // 
            LblAcquisitionTime.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblAcquisitionTime.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblAcquisitionTime.BorderThickness = 0;
            LblAcquisitionTime.CornerRadius = 0;
            LblAcquisitionTime.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblAcquisitionTime.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblAcquisitionTime.HighlightPrompt = defaultHighlightPrompt3;
            LblAcquisitionTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblAcquisitionTime.Location = new System.Drawing.Point(196, 224);
            LblAcquisitionTime.MultyLineFlag = false;
            LblAcquisitionTime.Name = "LblAcquisitionTime";
            LblAcquisitionTime.Size = new System.Drawing.Size(140, 18);
            LblAcquisitionTime.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblAcquisitionTime.StylizeFlag = true;
            LblAcquisitionTime.TabIndex = 75;
            LblAcquisitionTime.TabStop = false;
            LblAcquisitionTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CaiJiShiJian");
            LblAcquisitionTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblAcquisitionTime.Token = null;
            // 
            // BtnAcquisitionTime
            // 
            BtnAcquisitionTime.BackColor = System.Drawing.Color.Transparent;
            BtnAcquisitionTime.BorderColor = System.Drawing.Color.Black;
            BtnAcquisitionTime.BorderThickness = 1;
            BtnAcquisitionTime.CornerRadius = 0;
            BtnAcquisitionTime.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnAcquisitionTime.DaskArray = null;
            BtnAcquisitionTime.DropKey = System.Windows.Forms.Keys.Space;
            BtnAcquisitionTime.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnAcquisitionTime.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnAcquisitionTime.Height = 23;
            BtnAcquisitionTime.Icon = null;
            BtnAcquisitionTime.IconOffset = 10;
            BtnAcquisitionTime.IconSize = new System.Drawing.Size(24, 24);
            BtnAcquisitionTime.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnAcquisitionTime.IsChoosed = false;
            BtnAcquisitionTime.IsIndicatorShow = false;
            BtnAcquisitionTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnAcquisitionTime.Location = new System.Drawing.Point(196, 252);
            BtnAcquisitionTime.MouseinBackColor = System.Drawing.Color.Transparent;
            BtnAcquisitionTime.MouseinBorderColor = System.Drawing.Color.Blue;
            BtnAcquisitionTime.MouseInBorderThickness = 1;
            BtnAcquisitionTime.MouseinForeColor = System.Drawing.Color.Blue;
            BtnAcquisitionTime.MouseinSvgForeColor = System.Drawing.Color.Blue;
            BtnAcquisitionTime.Name = "BtnAcquisitionTime";
            BtnAcquisitionTime.PressedBackColor = System.Drawing.Color.Gray;
            BtnAcquisitionTime.PressedBorderColor = System.Drawing.Color.Blue;
            BtnAcquisitionTime.PressedBorderThickness = 1;
            BtnAcquisitionTime.PressedForeColor = System.Drawing.Color.Blue;
            BtnAcquisitionTime.PressedSvgForeColor = System.Drawing.Color.Blue;
            BtnAcquisitionTime.Size = new System.Drawing.Size(140, 30);
            BtnAcquisitionTime.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            BtnAcquisitionTime.StylizeFlag = true;
            BtnAcquisitionTime.SVGForeColor = System.Drawing.Color.Black;
            BtnAcquisitionTime.SVGPath = "";
            BtnAcquisitionTime.TabIndex = 2;
            BtnAcquisitionTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnAcquisitionTime.DoubleClick += BtnAcquisitionTime_Click;
            // 
            // BtnRun
            // 
            BtnRun.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRun.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRun.BorderThickness = 0;
            BtnRun.CornerRadius = 0;
            BtnRun.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnRun.DaskArray = null;
            BtnRun.DropKey = System.Windows.Forms.Keys.Space;
            BtnRun.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnRun.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnRun.Height = 30;
            BtnRun.Icon = null;
            BtnRun.IconOffset = 10;
            BtnRun.IconSize = new System.Drawing.Size(24, 24);
            BtnRun.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnRun.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnRun.IsIndicatorShow = false;
            BtnRun.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnRun.Location = new System.Drawing.Point(196, 338);
            BtnRun.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRun.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRun.MouseInBorderThickness = 0;
            BtnRun.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnRun.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnRun.Name = "BtnRun";
            BtnRun.PressedBackColor = System.Drawing.Color.Gray;
            BtnRun.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRun.PressedBorderThickness = 0;
            BtnRun.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnRun.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnRun.Size = new System.Drawing.Size(140, 30);
            BtnRun.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnRun.StylizeFlag = true;
            BtnRun.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnRun.SVGPath = "";
            BtnRun.TabIndex = 52;
            BtnRun.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YunXing");
            BtnRun.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnRun.Click += BtnRun_Click;
            // 
            // BtnGuide
            // 
            BtnGuide.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnGuide.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnGuide.BorderThickness = 0;
            BtnGuide.CornerRadius = 0;
            BtnGuide.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnGuide.DaskArray = null;
            BtnGuide.DropKey = System.Windows.Forms.Keys.Space;
            BtnGuide.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnGuide.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnGuide.Height = 30;
            BtnGuide.Icon = null;
            BtnGuide.IconOffset = 10;
            BtnGuide.IconSize = new System.Drawing.Size(24, 24);
            BtnGuide.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnGuide.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnGuide.IsIndicatorShow = false;
            BtnGuide.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnGuide.Location = new System.Drawing.Point(373, 252);
            BtnGuide.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnGuide.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnGuide.MouseInBorderThickness = 0;
            BtnGuide.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnGuide.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnGuide.Name = "BtnGuide";
            BtnGuide.PressedBackColor = System.Drawing.Color.Gray;
            BtnGuide.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnGuide.PressedBorderThickness = 0;
            BtnGuide.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnGuide.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnGuide.Size = new System.Drawing.Size(140, 30);
            BtnGuide.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnGuide.StylizeFlag = true;
            BtnGuide.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnGuide.SVGPath = "";
            BtnGuide.TabIndex = 52;
            BtnGuide.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XinHaoLianJieShiYi");
            BtnGuide.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnGuide.Click += BtnGuide_Click;
            // 
            // BtnResultTable
            // 
            BtnResultTable.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResultTable.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResultTable.BorderThickness = 0;
            BtnResultTable.CornerRadius = 0;
            BtnResultTable.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnResultTable.DaskArray = null;
            BtnResultTable.DropKey = System.Windows.Forms.Keys.Space;
            BtnResultTable.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnResultTable.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResultTable.Height = 30;
            BtnResultTable.Icon = null;
            BtnResultTable.IconOffset = 10;
            BtnResultTable.IconSize = new System.Drawing.Size(24, 24);
            BtnResultTable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnResultTable.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnResultTable.IsIndicatorShow = false;
            BtnResultTable.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnResultTable.Location = new System.Drawing.Point(19, 338);
            BtnResultTable.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResultTable.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResultTable.MouseInBorderThickness = 0;
            BtnResultTable.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResultTable.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnResultTable.Name = "BtnResultTable";
            BtnResultTable.PressedBackColor = System.Drawing.Color.Gray;
            BtnResultTable.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResultTable.PressedBorderThickness = 0;
            BtnResultTable.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResultTable.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnResultTable.Size = new System.Drawing.Size(140, 30);
            BtnResultTable.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnResultTable.StylizeFlag = true;
            BtnResultTable.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResultTable.SVGPath = "";
            BtnResultTable.TabIndex = 93;
            BtnResultTable.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JieGuoBiao");
            BtnResultTable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnResultTable.Click += BtnResultTable_Click;
            // 
            // PwrOnOffTimePage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(LblActive);
            Controls.Add(ChkActive);
            Controls.Add(LblOutVoltageSrc);
            Controls.Add(LblInVoltageSrc);
            Controls.Add(CbxOutVoltageSrc);
            Controls.Add(CbxInVoltageSrc);
            Controls.Add(LblConvertType);
            Controls.Add(CbxConvertType);
            Controls.Add(LblTestType);
            Controls.Add(CbxTestType);
            Controls.Add(LblAcquisitionTime);
            Controls.Add(BtnAcquisitionTime);
            Controls.Add(LblInputPeakVoltage);
            Controls.Add(BtnInputPeakVoltage);
            Controls.Add(LblOutputPeakVoltage);
            Controls.Add(BtnOutputPeakVoltage);
            Controls.Add(BtnRun);
            Controls.Add(BtnResultTable);
            Controls.Add(BtnGuide);
            Name = "PwrOnOffTimePage";
            Size = new System.Drawing.Size(539, 444);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXLabel LblActive;
        private ScopeX.UserControls.ScopeXSwitchButton ChkActive;
        private ScopeX.UserControls.ScopeXLabel LblInVoltageSrc;
        private ScopeX.UserControls.SelectComboBox CbxInVoltageSrc;
        private ScopeX.UserControls.ScopeXLabel LblOutVoltageSrc;
        private ScopeX.UserControls.SelectComboBox CbxOutVoltageSrc;
        private ScopeX.UserControls.ScopeXLabel LblConvertType;
        private ScopeX.UserControls.SelectComboBox CbxConvertType;
        private ScopeX.UserControls.ScopeXLabel LblTestType;
        private ScopeX.UserControls.SelectComboBox CbxTestType;
        private ScopeX.UserControls.ScopeXLabel LblAcquisitionTime;
        private ScopeX.UserControls.ScopeXIconButton BtnAcquisitionTime;
        private ScopeX.UserControls.ScopeXLabel LblInputPeakVoltage;
        private ScopeX.UserControls.ScopeXIconButton BtnInputPeakVoltage;
        private ScopeX.UserControls.ScopeXLabel LblOutputPeakVoltage;
        private ScopeX.UserControls.ScopeXIconButton BtnOutputPeakVoltage;
        private ScopeX.UserControls.ScopeXIconButton BtnRun;
        private ScopeX.UserControls.ScopeXIconButton BtnGuide;
        private ScopeX.UserControls.ScopeXIconButton BtnResultTable;
    }
}
