using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    partial class PwrInrushCurrentPage
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
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new ScopeX.UserControls.DefaultHighlightPrompt();
            UserControls.RadioButtonItem radioButtonItem1 = new UserControls.RadioButtonItem();
            UserControls.RadioButtonItem radioButtonItem2 = new UserControls.RadioButtonItem();
            LblActive = new ScopeX.UserControls.ScopeXLabel();
            ChkActive = new ScopeX.UserControls.ScopeXSwitchButton();
            LblCurrentSrc = new ScopeX.UserControls.ScopeXLabel();
            CbxCurrentSrc = new ScopeX.UserControls.SelectComboBox();
            LblPeakCurrent = new UserControls.ScopeXLabel();
            BtnPeakCurrent = new UserControls.ScopeXIconButton();
            BtnSingleRun = new UserControls.ScopeXIconButton();
            BtnSingleRun = new UserControls.ScopeXIconButton();
            BtnResultTable = new ScopeX.UserControls.ScopeXIconButton();
            BtnGuide = new ScopeX.UserControls.ScopeXIconButton();
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
            LblActive.HighlightPrompt = defaultHighlightPrompt4;
            LblActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblActive.Location = new System.Drawing.Point(19, 16);
            LblActive.MultyLineFlag = false;
            LblActive.Name = "LblActive";
            LblActive.Size = new System.Drawing.Size(75, 18);
            LblActive.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblActive.StylizeFlag = true;
            LblActive.TabIndex = 75;
            LblActive.TabStop = false;
            LblActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShi");
            LblActive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblActive.Token = null;
            // 
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
            // LblCurrentSrc
            // 
            LblCurrentSrc.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblCurrentSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblCurrentSrc.BorderThickness = 0;
            LblCurrentSrc.CornerRadius = 0;
            LblCurrentSrc.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblCurrentSrc.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblCurrentSrc.HighlightPrompt = defaultHighlightPrompt2;
            LblCurrentSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblCurrentSrc.Location = new System.Drawing.Point(196, 16);
            LblCurrentSrc.MultyLineFlag = false;
            LblCurrentSrc.Name = "LblCurrentSrc";
            LblCurrentSrc.Size = new System.Drawing.Size(140, 18);
            LblCurrentSrc.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblCurrentSrc.StylizeFlag = true;
            LblCurrentSrc.TabIndex = 55;
            LblCurrentSrc.TabStop = false;
            LblCurrentSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianLiuYuan");
            LblCurrentSrc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblCurrentSrc.Token = null;
            // 
            // CbxCurrentSrc
            // 
            CbxCurrentSrc.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxCurrentSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxCurrentSrc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxCurrentSrc.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxCurrentSrc.ForeColor = System.Drawing.Color.White;
            CbxCurrentSrc.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4"
    };
            CbxCurrentSrc.Location = new System.Drawing.Point(196, 44);
            CbxCurrentSrc.Name = "CbxCurrentSrc";
            CbxCurrentSrc.SelectValue = null;
            CbxCurrentSrc.Size = new System.Drawing.Size(140, 30);
            CbxCurrentSrc.TabIndex = 77;
            // 
            // LblPeakCurrent
            // 
            LblPeakCurrent.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblPeakCurrent.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblPeakCurrent.BorderThickness = 0;
            LblPeakCurrent.CornerRadius = 0;
            LblPeakCurrent.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblPeakCurrent.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblPeakCurrent.HighlightPrompt = defaultHighlightPrompt4;
            LblPeakCurrent.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblPeakCurrent.Location = new System.Drawing.Point(373, 16);
            LblPeakCurrent.MultyLineFlag = false;
            LblPeakCurrent.Name = "LblPeakCurrent";
            LblPeakCurrent.Size = new System.Drawing.Size(140, 18);
            LblPeakCurrent.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblPeakCurrent.StylizeFlag = true;
            LblPeakCurrent.TabIndex = 75;
            LblPeakCurrent.TabStop = false;
            LblPeakCurrent.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FengZhiDianLiu");
            LblPeakCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblPeakCurrent.Token = null;
            // 
            // BtnPeakCurrent
            // 
            BtnPeakCurrent.BackColor = System.Drawing.Color.Transparent;
            BtnPeakCurrent.BorderColor = System.Drawing.Color.Black;
            BtnPeakCurrent.BorderThickness = 1;
            BtnPeakCurrent.CornerRadius = 0;
            BtnPeakCurrent.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnPeakCurrent.DaskArray = null;
            BtnPeakCurrent.DropKey = System.Windows.Forms.Keys.Space;
            BtnPeakCurrent.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnPeakCurrent.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnPeakCurrent.Height = 23;
            BtnPeakCurrent.Icon = null;
            BtnPeakCurrent.IconOffset = 10;
            BtnPeakCurrent.IconSize = new System.Drawing.Size(24, 24);
            BtnPeakCurrent.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnPeakCurrent.IsChoosed = false;
            BtnPeakCurrent.IsIndicatorShow = false;
            BtnPeakCurrent.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnPeakCurrent.Location = new System.Drawing.Point(373, 44);
            BtnPeakCurrent.MouseinBackColor = System.Drawing.Color.Transparent;
            BtnPeakCurrent.MouseinBorderColor = System.Drawing.Color.Blue;
            BtnPeakCurrent.MouseInBorderThickness = 1;
            BtnPeakCurrent.MouseinForeColor = System.Drawing.Color.Blue;
            BtnPeakCurrent.MouseinSvgForeColor = System.Drawing.Color.Blue;
            BtnPeakCurrent.Name = "BtnThreshold";
            BtnPeakCurrent.PressedBackColor = System.Drawing.Color.Gray;
            BtnPeakCurrent.PressedBorderColor = System.Drawing.Color.Blue;
            BtnPeakCurrent.PressedBorderThickness = 1;
            BtnPeakCurrent.PressedForeColor = System.Drawing.Color.Blue;
            BtnPeakCurrent.PressedSvgForeColor = System.Drawing.Color.Blue;
            BtnPeakCurrent.Size = new System.Drawing.Size(140, 30);
            BtnPeakCurrent.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            BtnPeakCurrent.StylizeFlag = true;
            BtnPeakCurrent.SVGForeColor = System.Drawing.Color.Black;
            BtnPeakCurrent.SVGPath = "";
            BtnPeakCurrent.TabIndex = 2;
            BtnPeakCurrent.Text = "BtnPeakCurrent";
            BtnPeakCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnPeakCurrent.DoubleClick += BtnPeakCurrent_Click;
            
            // 
            // BtnSingleRun
            // 
            BtnSingleRun.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSingleRun.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSingleRun.BorderThickness = 0;
            BtnSingleRun.CornerRadius = 0;
            BtnSingleRun.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSingleRun.DaskArray = null;
            BtnSingleRun.DropKey = System.Windows.Forms.Keys.Space;
            BtnSingleRun.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnSingleRun.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSingleRun.Height = 30;
            BtnSingleRun.Icon = null;
            BtnSingleRun.IconOffset = 10;
            BtnSingleRun.IconSize = new System.Drawing.Size(24, 24);
            BtnSingleRun.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnSingleRun.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSingleRun.IsIndicatorShow = false;
            BtnSingleRun.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSingleRun.Location = new System.Drawing.Point(196, 254);
            BtnSingleRun.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSingleRun.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSingleRun.MouseInBorderThickness = 0;
            BtnSingleRun.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSingleRun.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSingleRun.Name = "BtnSingleRun";
            BtnSingleRun.PressedBackColor = System.Drawing.Color.Gray;
            BtnSingleRun.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSingleRun.PressedBorderThickness = 0;
            BtnSingleRun.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSingleRun.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSingleRun.Size = new System.Drawing.Size(140, 30);
            BtnSingleRun.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnSingleRun.StylizeFlag = true;
            BtnSingleRun.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSingleRun.SVGPath = "";
            BtnSingleRun.TabIndex = 70;
            BtnSingleRun.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YunXing");
            BtnSingleRun.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSingleRun.Click += BtnSingleRun_Click;
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
            BtnResultTable.Location = new System.Drawing.Point(19, 148);
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
            BtnResultTable.TabIndex = 70;
            BtnResultTable.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JieGuoBiao");
            BtnResultTable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnResultTable.Click += BtnResultTable_Click;
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
            BtnGuide.Location = new System.Drawing.Point(196, 148);
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
            // PowerQualityPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            Controls.Add(LblActive);
            Controls.Add(ChkActive);
            Controls.Add(LblCurrentSrc);
            Controls.Add(CbxCurrentSrc);
            Controls.Add(LblPeakCurrent);
            Controls.Add(BtnPeakCurrent);
            Controls.Add(BtnSingleRun);
            Controls.Add(BtnResultTable);
            Controls.Add(BtnGuide);
            Name = "PowerQualityPage";
            Size = new System.Drawing.Size(539, 444);
            ResumeLayout(false);
        }

        #endregion
        private ScopeX.UserControls.ScopeXLabel LblActive;
        private ScopeX.UserControls.ScopeXSwitchButton ChkActive;
        private ScopeX.UserControls.ScopeXLabel LblCurrentSrc;
        private ScopeX.UserControls.SelectComboBox CbxCurrentSrc;
        private ScopeX.UserControls.ScopeXLabel LblPeakCurrent;
        private ScopeX.UserControls.ScopeXIconButton BtnPeakCurrent;
        private ScopeX.UserControls.ScopeXIconButton BtnSingleRun;
        private ScopeX.UserControls.ScopeXIconButton BtnResultTable;
        private ScopeX.UserControls.ScopeXIconButton BtnGuide;
    }
}
