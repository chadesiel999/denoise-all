
namespace ScopeX.U2
{
    partial class PwrEfficiencyPage
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PwrEfficiencyPage));
            UserControls.RadioButtonItem radioButtonItem1 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem2 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem3 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem4 = new UserControls.RadioButtonItem();
            ChkActive = new UserControls.ScopeXSwitchButton();
            LblActive = new UserControls.ScopeXLabel();
            LblInVoltageSrc = new UserControls.ScopeXLabel();
            LblInCurrentSrc = new UserControls.ScopeXLabel();
            LblOutVoltageSrc = new UserControls.ScopeXLabel();
            LblOutCurrentSrc = new UserControls.ScopeXLabel();
            CbxInVoltageSrc = new UserControls.SelectComboBox();
            CbxOutVoltageSrc = new UserControls.SelectComboBox();
            CbxInCurrentSrc = new UserControls.SelectComboBox();
            CbxOutCurrentSrc = new UserControls.SelectComboBox();
            BtnShowPower1 = new UserControls.ScopeXIconButton();
            BtnShowPower2 = new UserControls.ScopeXIconButton();
            BtnGuide = new UserControls.ScopeXIconButton();
            BtnResultTable = new UserControls.ScopeXIconButton();
            LblInputType = new UserControls.ScopeXLabel();
            RdoInputType = new UserControls.UIRadioButtonGroup();
            LblOutputType = new UserControls.ScopeXLabel();
            RdoOutputType = new UserControls.UIRadioButtonGroup();
            SuspendLayout();
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
            ChkActive.Location = new System.Drawing.Point(19, 40);
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
            // LblInVoltageSrc
            // 
            LblInVoltageSrc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            LblInVoltageSrc.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            LblInVoltageSrc.BorderThickness = 0;
            LblInVoltageSrc.CornerRadius = 0;
            LblInVoltageSrc.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblInVoltageSrc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            LblInVoltageSrc.HighlightPrompt = defaultHighlightPrompt1;
            LblInVoltageSrc.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblInVoltageSrc.IsOmittext = true;
            LblInVoltageSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblInVoltageSrc.Location = new System.Drawing.Point(196, 16);
            LblInVoltageSrc.MultyLineFlag = false;
            LblInVoltageSrc.Name = "LblInVoltageSrc";
            LblInVoltageSrc.Size = new System.Drawing.Size(140, 18);
            LblInVoltageSrc.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblInVoltageSrc.StylizeFlag = true;
            LblInVoltageSrc.TabIndex = 4;
            LblInVoltageSrc.TabStop = false;
            LblInVoltageSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuRuDuanDianYaYuan");
            LblInVoltageSrc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblInVoltageSrc.Token = null;
            // 
            // LblInCurrentSrc
            // 
            LblInCurrentSrc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            LblInCurrentSrc.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            LblInCurrentSrc.BorderThickness = 0;
            LblInCurrentSrc.CornerRadius = 0;
            LblInCurrentSrc.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblInCurrentSrc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            LblInCurrentSrc.HighlightPrompt = defaultHighlightPrompt2;
            LblInCurrentSrc.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblInCurrentSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblInCurrentSrc.Location = new System.Drawing.Point(373, 16);
            LblInCurrentSrc.MultyLineFlag = false;
            LblInCurrentSrc.Name = "LblInCurrentSrc";
            LblInCurrentSrc.Size = new System.Drawing.Size(140, 18);
            LblInCurrentSrc.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblInCurrentSrc.StylizeFlag = true;
            LblInCurrentSrc.TabIndex = 0;
            LblInCurrentSrc.TabStop = false;
            LblInCurrentSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuRuDuanDianLiuYuan");
            LblInCurrentSrc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblInCurrentSrc.Token = null;
            // 
            // LblOutVoltageSrc
            // 
            LblOutVoltageSrc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            LblOutVoltageSrc.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            LblOutVoltageSrc.BorderThickness = 0;
            LblOutVoltageSrc.CornerRadius = 0;
            LblOutVoltageSrc.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblOutVoltageSrc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            LblOutVoltageSrc.HighlightPrompt = defaultHighlightPrompt3;
            LblOutVoltageSrc.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblOutVoltageSrc.IsOmittext = true;
            LblOutVoltageSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblOutVoltageSrc.Location = new System.Drawing.Point(196, 148);
            LblOutVoltageSrc.MultyLineFlag = false;
            LblOutVoltageSrc.Name = "LblOutVoltageSrc";
            LblOutVoltageSrc.Size = new System.Drawing.Size(140, 18);
            LblOutVoltageSrc.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblOutVoltageSrc.StylizeFlag = true;
            LblOutVoltageSrc.TabIndex = 6;
            LblOutVoltageSrc.TabStop = false;
            LblOutVoltageSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuChuDuanDianYaYuan");
            LblOutVoltageSrc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblOutVoltageSrc.Token = null;
            // 
            // LblOutCurrentSrc
            // 
            LblOutCurrentSrc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            LblOutCurrentSrc.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            LblOutCurrentSrc.BorderThickness = 0;
            LblOutCurrentSrc.CornerRadius = 0;
            LblOutCurrentSrc.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblOutCurrentSrc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            LblOutCurrentSrc.HighlightPrompt = defaultHighlightPrompt4;
            LblOutCurrentSrc.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblOutCurrentSrc.IsOmittext = true;
            LblOutCurrentSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblOutCurrentSrc.Location = new System.Drawing.Point(373, 148);
            LblOutCurrentSrc.MultyLineFlag = false;
            LblOutCurrentSrc.Name = "LblOutCurrentSrc";
            LblOutCurrentSrc.Size = new System.Drawing.Size(140, 18);
            LblOutCurrentSrc.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblOutCurrentSrc.StylizeFlag = true;
            LblOutCurrentSrc.TabIndex = 2;
            LblOutCurrentSrc.TabStop = false;
            LblOutCurrentSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuChuDuanDianLiuYuan");
            LblOutCurrentSrc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblOutCurrentSrc.Token = null;
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
            CbxInVoltageSrc.Location = new System.Drawing.Point(196, 40);
            CbxInVoltageSrc.Name = "CbxInVoltageSrc";
            CbxInVoltageSrc.SelectIndex = 0;
            CbxInVoltageSrc.SelectValue = 0;
            CbxInVoltageSrc.Size = new System.Drawing.Size(140, 30);
            CbxInVoltageSrc.TabIndex = 83; CbxInVoltageSrc.SelectedIndexChanged += CbxInVoltageSrc_SelectedIndexChanged;
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
            CbxOutVoltageSrc.Location = new System.Drawing.Point(196, 176);
            CbxOutVoltageSrc.Name = "CbxOutVoltageSrc";
            CbxOutVoltageSrc.Size = new System.Drawing.Size(140, 30);
            CbxOutVoltageSrc.TabIndex = 83; CbxOutVoltageSrc.SelectedIndexChanged += CbxOutVoltageSrc_SelectedIndexChanged;
            // 
            // CbxInCurrentSrc
            // 
            CbxInCurrentSrc.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxInCurrentSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxInCurrentSrc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxInCurrentSrc.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxInCurrentSrc.ForeColor = System.Drawing.Color.White;
            CbxInCurrentSrc.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4"
    };
            CbxInCurrentSrc.Location = new System.Drawing.Point(373, 40);
            CbxInCurrentSrc.Name = "CbxInCurrentSrc";
            CbxInCurrentSrc.SelectIndex = 0;
            CbxInCurrentSrc.SelectValue = 0;
            CbxInCurrentSrc.Size = new System.Drawing.Size(140, 30);
            CbxInCurrentSrc.TabIndex = 83; CbxInCurrentSrc.SelectedIndexChanged += CbxInCurrentSrc_SelectedIndexChanged;
            // 
            // CbxOutCurrentSrc
            // 
            CbxOutCurrentSrc.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxOutCurrentSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxOutCurrentSrc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxOutCurrentSrc.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxOutCurrentSrc.ForeColor = System.Drawing.Color.White;
            CbxOutCurrentSrc.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4"
    };
            CbxOutCurrentSrc.Location = new System.Drawing.Point(373, 176);
            CbxOutCurrentSrc.Name = "CbxOutCurrentSrc";
            CbxOutCurrentSrc.Size = new System.Drawing.Size(140, 30);
            CbxOutCurrentSrc.TabIndex = 83; CbxOutCurrentSrc.SelectedIndexChanged += CbxOutCurrentSrc_SelectedIndexChanged;
            // 
            // BtnShowPower1
            // 
            BtnShowPower1.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowPower1.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowPower1.BorderThickness = 0;
            BtnShowPower1.CornerRadius = 0;
            BtnShowPower1.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnShowPower1.DaskArray = null;
            BtnShowPower1.DropKey = System.Windows.Forms.Keys.Space;
            BtnShowPower1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnShowPower1.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowPower1.Height = 30;
            BtnShowPower1.Icon = null;
            BtnShowPower1.IconOffset = 10;
            BtnShowPower1.IconSize = new System.Drawing.Size(24, 24);
            BtnShowPower1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnShowPower1.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnShowPower1.IsIndicatorShow = false;
            BtnShowPower1.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnShowPower1.Location = new System.Drawing.Point(19, 284);
            BtnShowPower1.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowPower1.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowPower1.MouseInBorderThickness = 0;
            BtnShowPower1.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowPower1.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnShowPower1.Name = "BtnShowPower1";
            BtnShowPower1.PressedBackColor = System.Drawing.Color.Gray;
            BtnShowPower1.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowPower1.PressedBorderThickness = 0;
            BtnShowPower1.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowPower1.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnShowPower1.Size = new System.Drawing.Size(140, 30);
            BtnShowPower1.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnShowPower1.StylizeFlag = true;
            BtnShowPower1.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowPower1.SVGPath = "";
            BtnShowPower1.TabIndex = 52;
            BtnShowPower1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuRuGongLvTu");
            BtnShowPower1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnShowPower1.Click += BtnShowPower1_Click;
            // 
            // BtnShowPower2
            // 
            BtnShowPower2.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowPower2.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowPower2.BorderThickness = 0;
            BtnShowPower2.CornerRadius = 0;
            BtnShowPower2.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnShowPower2.DaskArray = null;
            BtnShowPower2.DropKey = System.Windows.Forms.Keys.Space;
            BtnShowPower2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnShowPower2.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowPower2.Height = 30;
            BtnShowPower2.Icon = null;
            BtnShowPower2.IconOffset = 10;
            BtnShowPower2.IconSize = new System.Drawing.Size(24, 24);
            BtnShowPower2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnShowPower2.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnShowPower2.IsIndicatorShow = false;
            BtnShowPower2.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnShowPower2.Location = new System.Drawing.Point(196, 284);
            BtnShowPower2.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowPower2.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowPower2.MouseInBorderThickness = 0;
            BtnShowPower2.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowPower2.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnShowPower2.Name = "BtnShowPower2";
            BtnShowPower2.PressedBackColor = System.Drawing.Color.Gray;
            BtnShowPower2.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnShowPower2.PressedBorderThickness = 0;
            BtnShowPower2.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowPower2.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnShowPower2.Size = new System.Drawing.Size(140, 30);
            BtnShowPower2.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnShowPower2.StylizeFlag = true;
            BtnShowPower2.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnShowPower2.SVGPath = "";
            BtnShowPower2.TabIndex = 52;
            BtnShowPower2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuChuGongLvTu");
            BtnShowPower2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnShowPower2.Click += BtnShowPower2_Click;
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
            BtnGuide.Location = new System.Drawing.Point(19, 176);
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
            BtnResultTable.Location = new System.Drawing.Point(373, 284);
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
            // LblInputType
            // 
            LblInputType.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblInputType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblInputType.BorderThickness = 0;
            LblInputType.CornerRadius = 0;
            LblInputType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblInputType.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblInputType.HighlightPrompt = defaultHighlightPrompt4;
            LblInputType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblInputType.IsOmittext = true;
            LblInputType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblInputType.Location = new System.Drawing.Point(373, 90);
            LblInputType.MultyLineFlag = false;
            LblInputType.Name = "LblInputType";
            LblInputType.Size = new System.Drawing.Size(140, 18);
            LblInputType.StyleFlags = UserControls.Style.StyleFlag.None;
            LblInputType.StylizeFlag = true;
            LblInputType.TabIndex = 2;
            LblInputType.TabStop = false;
            LblInputType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuRuLeiXing");
            LblInputType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblInputType.Token = null;
            // 
            // RdoInputType
            // 
            RdoInputType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            RdoInputType.BackColor = System.Drawing.Color.Black;
            RdoInputType.BorderColor = System.Drawing.Color.Black;
            RdoInputType.BorderThickness = 0;
            RdoInputType.ButtonBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoInputType.ButtonFont = null;
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = "AC";
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = "DC";
            RdoInputType.ButtonItems = new UserControls.RadioButtonItem[]
    {
    radioButtonItem1,
    radioButtonItem2
    };
            RdoInputType.ButtonOffset = 10;
            RdoInputType.ButtonTextColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RdoInputType.ChoosedButtonColor = System.Drawing.Color.FromArgb(0, 157, 255);
            RdoInputType.ChoosedButtonIndex = 0;
            RdoInputType.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoInputType.ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            RdoInputType.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoInputType.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoInputType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoInputType.Height = 30;
            RdoInputType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoInputType.Location = new System.Drawing.Point(373, 118);
            RdoInputType.Margin = new System.Windows.Forms.Padding(0);
            RdoInputType.Name = "RdoInputType";
            RdoInputType.Size = new System.Drawing.Size(140, 30);
            RdoInputType.StyleFlags = UserControls.Style.StyleFlag.None;
            RdoInputType.StylizeFlag = true;
            RdoInputType.TabIndex = 63;
            RdoInputType.IndexChanged += RdoInputType_IndexChanged;
            // 
            // LblOutputType
            // 
            LblOutputType.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblOutputType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblOutputType.BorderThickness = 0;
            LblOutputType.CornerRadius = 0;
            LblOutputType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblOutputType.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblOutputType.HighlightPrompt = defaultHighlightPrompt4;
            LblOutputType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblOutputType.IsOmittext = true;
            LblOutputType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblOutputType.Location = new System.Drawing.Point(19, 90);
            LblOutputType.MultyLineFlag = false;
            LblOutputType.Name = "LblOutputType";
            LblOutputType.Size = new System.Drawing.Size(140, 18);
            LblOutputType.StyleFlags = UserControls.Style.StyleFlag.None;
            LblOutputType.StylizeFlag = true;
            LblOutputType.TabIndex = 2;
            LblOutputType.TabStop = false;
            LblOutputType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuChuLeiXing");
            LblOutputType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblOutputType.Token = null;
            // 
            // RdoOutputType
            // 
            RdoOutputType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            RdoOutputType.BackColor = System.Drawing.Color.Black;
            RdoOutputType.BorderColor = System.Drawing.Color.Black;
            RdoOutputType.BorderThickness = 0;
            RdoOutputType.ButtonBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoOutputType.ButtonFont = null;
            radioButtonItem3.Icon = null;
            radioButtonItem3.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem3.Tag = null;
            radioButtonItem3.Text = "AC";
            radioButtonItem4.Icon = null;
            radioButtonItem4.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem4.Tag = null;
            radioButtonItem4.Text = "DC";
            RdoOutputType.ButtonItems = new UserControls.RadioButtonItem[]
    {
    radioButtonItem3,
    radioButtonItem4
    };
            RdoOutputType.ButtonOffset = 10;
            RdoOutputType.ButtonTextColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RdoOutputType.ChoosedButtonColor = System.Drawing.Color.FromArgb(0, 157, 255);
            RdoOutputType.ChoosedButtonIndex = 0;
            RdoOutputType.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoOutputType.ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            RdoOutputType.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoOutputType.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoOutputType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoOutputType.Height = 30;
            RdoOutputType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoOutputType.Location = new System.Drawing.Point(19, 118);
            RdoOutputType.Margin = new System.Windows.Forms.Padding(0);
            RdoOutputType.Name = "RdoOutputType";
            RdoOutputType.Size = new System.Drawing.Size(140, 30);
            RdoOutputType.StyleFlags = UserControls.Style.StyleFlag.None;
            RdoOutputType.StylizeFlag = true;
            RdoOutputType.TabIndex = 63;
            RdoOutputType.IndexChanged += RdoOutputType_IndexChanged;
            // 
            // PwrEfficiencyPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(LblActive);
            Controls.Add(ChkActive);
            Controls.Add(LblInVoltageSrc);
            Controls.Add(CbxInVoltageSrc);
            Controls.Add(LblInCurrentSrc);
            Controls.Add(CbxInCurrentSrc);
            Controls.Add(LblOutVoltageSrc);
            Controls.Add(CbxOutVoltageSrc);
            Controls.Add(LblOutCurrentSrc);
            Controls.Add(CbxOutCurrentSrc);
            Controls.Add(BtnShowPower1);
            Controls.Add(BtnShowPower2);
            Controls.Add(BtnGuide);
            Controls.Add(BtnResultTable);
            Controls.Add(LblInputType);
            Controls.Add(RdoInputType);
            Controls.Add(LblOutputType);
            Controls.Add(RdoOutputType);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "PwrEfficiencyPage";
            Size = new System.Drawing.Size(539, 444);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXLabel LblActive;
        private ScopeX.UserControls.ScopeXSwitchButton ChkActive;
        private ScopeX.UserControls.ScopeXLabel LblInVoltageSrc;
        private ScopeX.UserControls.SelectComboBox CbxInVoltageSrc;
        private ScopeX.UserControls.ScopeXLabel LblInCurrentSrc;
        private ScopeX.UserControls.SelectComboBox CbxInCurrentSrc;
        private ScopeX.UserControls.ScopeXLabel LblOutVoltageSrc;
        private ScopeX.UserControls.SelectComboBox CbxOutVoltageSrc;
        private ScopeX.UserControls.ScopeXLabel LblOutCurrentSrc;
        private ScopeX.UserControls.SelectComboBox CbxOutCurrentSrc;
        private ScopeX.UserControls.ScopeXIconButton BtnShowPower1;
        private ScopeX.UserControls.ScopeXIconButton BtnShowPower2;
        private ScopeX.UserControls.ScopeXIconButton BtnGuide;
        private ScopeX.UserControls.ScopeXIconButton BtnResultTable;
        private ScopeX.UserControls.ScopeXLabel LblInputType;
        private ScopeX.UserControls.UIRadioButtonGroup RdoInputType;
        private ScopeX.UserControls.ScopeXLabel LblOutputType;
        private ScopeX.UserControls.UIRadioButtonGroup RdoOutputType;
    }
}
