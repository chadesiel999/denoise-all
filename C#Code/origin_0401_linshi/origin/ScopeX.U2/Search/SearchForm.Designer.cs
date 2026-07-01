
namespace ScopeX.U2.Search
{
    partial class SearchForm
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
            BtnAddSearch = new ScopeX.UserControls.ScopeXIconButton();
            ChkSoftSearch = new System.Windows.Forms.CheckBox();
            ChkActive = new ScopeX.UserControls.ScopeXSwitchButton();
            ChkResultTable = new ScopeX.UserControls.ScopeXSwitchButton();
            LblActive = new ScopeX.UserControls.ScopeXLabel();
            LblResultTable = new ScopeX.UserControls.ScopeXLabel();
            SuspendLayout();
            // 
            // BtnAddSearch
            // 
            BtnAddSearch.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            BtnAddSearch.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAddSearch.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAddSearch.BorderThickness = 0;
            BtnAddSearch.CornerRadius = 0;
            BtnAddSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnAddSearch.DaskArray = null;
            BtnAddSearch.DropKey = System.Windows.Forms.Keys.Space;
            BtnAddSearch.Font = new System.Drawing.Font("Arial", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnAddSearch.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAddSearch.Height = 30;
            BtnAddSearch.Icon = null;
            BtnAddSearch.IconOffset = 10;
            BtnAddSearch.IconSize = new System.Drawing.Size(24, 24);
            BtnAddSearch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnAddSearch.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnAddSearch.IsIndicatorShow = false;
            BtnAddSearch.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnAddSearch.Location = new System.Drawing.Point(12, 129);
            BtnAddSearch.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAddSearch.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAddSearch.MouseInBorderThickness = 0;
            BtnAddSearch.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAddSearch.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnAddSearch.Name = "BtnAddSearch";
            BtnAddSearch.PressedBackColor = System.Drawing.Color.Gray;
            BtnAddSearch.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAddSearch.PressedBorderThickness = 0;
            BtnAddSearch.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAddSearch.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnAddSearch.Size = new System.Drawing.Size(256, 30);
            BtnAddSearch.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnAddSearch.StylizeFlag = true;
            BtnAddSearch.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAddSearch.SVGPath = "";
            BtnAddSearch.TabIndex = 3;
            BtnAddSearch.Text = "+";
            BtnAddSearch.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnAddSearch.Click += BtnAddSearch_Click;
            // 
            // ChkSoftSearch
            // 
            ChkSoftSearch.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            ChkSoftSearch.AutoSize = true;
            ChkSoftSearch.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkSoftSearch.Location = new System.Drawing.Point(157, 88);
            ChkSoftSearch.Name = "ChkSoftSearch";
            ChkSoftSearch.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            ChkSoftSearch.Size = new System.Drawing.Size(75, 21);
            ChkSoftSearch.TabIndex = 2;
            ChkSoftSearch.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("RuanJianSouSuo");
            ChkSoftSearch.UseVisualStyleBackColor = true;
            ChkSoftSearch.Click += ChkSoftSearch_Click;
            ChkSoftSearch.Visible = false;
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
            ChkActive.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkActive.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkActive.Height = 30;
            ChkActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkActive.Location = new System.Drawing.Point(12, 79);
            ChkActive.Margin = new System.Windows.Forms.Padding(0);
            ChkActive.Name = "ChkActive";
            ChkActive.Size = new System.Drawing.Size(75, 30);
            ChkActive.SliderButtonWidth = 30;
            ChkActive.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkActive.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            ChkActive.StylizeFlag = true;
            ChkActive.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            ChkActive.TabIndex = 1;
            ChkActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkActive.UseAnimation = true;
            ChkActive.CheckedChangedEvent += ChkActive_CheckedChangedEvent;
            // 
            // ChkResultTable
            // 
            ChkResultTable.AnimationCount = 8;
            ChkResultTable.AnimationFunc = null;
            ChkResultTable.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkResultTable.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkResultTable.BorderThickness = 0;
            ChkResultTable.Checked = false;
            ChkResultTable.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkResultTable.CheckedForeColor = System.Drawing.Color.Black;
            ChkResultTable.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkResultTable.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkResultTable.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkResultTable.DropKey = System.Windows.Forms.Keys.Space;
            ChkResultTable.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkResultTable.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkResultTable.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkResultTable.Height = 30;
            ChkResultTable.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkResultTable.Location = new System.Drawing.Point(162, 79);
            ChkResultTable.Margin = new System.Windows.Forms.Padding(0);
            ChkResultTable.Name = "ChkResultTable";
            ChkResultTable.Size = new System.Drawing.Size(75, 30);
            ChkResultTable.SliderButtonWidth = 30;
            ChkResultTable.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkResultTable.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            ChkResultTable.StylizeFlag = true;
            ChkResultTable.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            ChkResultTable.TabIndex = 1;
            ChkResultTable.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkResultTable.UseAnimation = true;
            ChkResultTable.CheckedChangedEvent += ChkResultTable_CheckedChangedEvent;
            // 
            // LblActive
            // 
            LblActive.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblActive.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblActive.BorderThickness = 0;
            LblActive.CornerRadius = 0;
            LblActive.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblActive.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblActive.HighlightPrompt = defaultHighlightPrompt1;
            LblActive.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblActive.Location = new System.Drawing.Point(12, 51);
            LblActive.MultyLineFlag = false;
            LblActive.Name = "LblActive";
            LblActive.Size = new System.Drawing.Size(75, 17);
            LblActive.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblActive.StylizeFlag = true;
            LblActive.TabIndex = 14;
            LblActive.TabStop = false;
            LblActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SouSuo");
            LblActive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblActive.Token = null;
            // 
            // LblResultTable
            // 
            LblResultTable.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblResultTable.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblResultTable.BorderThickness = 0;
            LblResultTable.CornerRadius = 0;
            LblResultTable.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblResultTable.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblResultTable.HighlightPrompt = defaultHighlightPrompt1;
            LblResultTable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblResultTable.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblResultTable.Location = new System.Drawing.Point(162, 51);
            LblResultTable.MultyLineFlag = false;
            LblResultTable.Name = "LblResultTable";
            LblResultTable.Size = new System.Drawing.Size(75, 17);
            LblResultTable.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblResultTable.StylizeFlag = true;
            LblResultTable.TabIndex = 14;
            LblResultTable.TabStop = false;
            LblResultTable.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SouSuoShiJianLieBiao");
            LblResultTable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblResultTable.Token = null;
            // 
            // SearchForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(280, 171);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(LblActive);
            Controls.Add(ChkActive);
            Controls.Add(LblResultTable);
            Controls.Add(ChkResultTable);
            Controls.Add(ChkSoftSearch);
            Controls.Add(BtnAddSearch);
            DoubleBuffered = true;
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            HeadHeight = 45;
            IconInterval = 21;
            IconSideDistance = 3;
            IconWidth = 28;
            Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SearchForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BoXingSouSuo");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BoXingSouSuo");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleLableHeight = 39;
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(28, 26);
            Controls.SetChildIndex(BtnAddSearch, 0);
            Controls.SetChildIndex(ChkSoftSearch, 0);
            Controls.SetChildIndex(ChkActive, 0);
            Controls.SetChildIndex(LblActive, 0);
            Controls.SetChildIndex(ChkResultTable, 0);
            Controls.SetChildIndex(LblResultTable, 0);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion


        private ScopeX.UserControls.ScopeXIconButton BtnAddSearch;
        private System.Windows.Forms.CheckBox ChkSoftSearch;
        private ScopeX.UserControls.ScopeXSwitchButton ChkActive;
        private ScopeX.UserControls.ScopeXSwitchButton ChkResultTable;
        private ScopeX.UserControls.ScopeXLabel LblActive;
        private ScopeX.UserControls.ScopeXLabel LblResultTable;
    }
}
