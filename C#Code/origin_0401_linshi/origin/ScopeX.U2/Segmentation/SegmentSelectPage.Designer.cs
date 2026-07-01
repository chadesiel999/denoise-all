namespace ScopeX.U2
{
    partial class SegmentSelectPage
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
            this.CbxViewMode = new ScopeX.UserControls.ComboBoxEx();
            this.LblViewMode = new ScopeX.UserControls.ScopeXLabel();
            this.LblFramesSelected = new ScopeX.UserControls.ScopeXLabel();
            this.BtnFramesSelected = new ScopeX.UserControls.ScopeXIconButton();
            this.SuspendLayout();
            // 
            // CbxViewMode
            // 
            this.CbxViewMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxViewMode.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxViewMode.BorderThickness = 0;
            this.CbxViewMode.CornerRadius = 0;
            this.CbxViewMode.DataSource = null;
            this.CbxViewMode.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CbxViewMode.DropDownHeight = 200;
            this.CbxViewMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxViewMode.DropDownWidth = 111;
            this.CbxViewMode.DropKey = System.Windows.Forms.Keys.Space;
            this.CbxViewMode.DroppedDown = false;
            this.CbxViewMode.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(52)))), ((int)(((byte)(56)))));
            this.CbxViewMode.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CbxViewMode.ForeColor = System.Drawing.Color.Silver;
            this.CbxViewMode.FormattingEnabled = true;
            this.CbxViewMode.GetDisPlayName = null;
            this.CbxViewMode.Height = 34;
            this.CbxViewMode.ImageMode = false;
            this.CbxViewMode.ItemHeight = 25;
            this.CbxViewMode.Items.AddRange(new object[] {
            "45°",
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DuiDie"),
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DieJia"),
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PinJie")});
            this.CbxViewMode.KeyDropEnble = true;
            this.CbxViewMode.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.CbxViewMode.Location = new System.Drawing.Point(284, 32);
            this.CbxViewMode.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.CbxViewMode.MaxDropDownItems = 8;
            this.CbxViewMode.Name = "CbxViewMode";
            this.CbxViewMode.RectBtnWidth = 20;
            this.CbxViewMode.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(183)))), ((int)(((byte)(245)))));
            this.CbxViewMode.SelectedIndex = -1;
            this.CbxViewMode.SelectedItem = null;
            this.CbxViewMode.SelectedText = "";
            this.CbxViewMode.Size = new System.Drawing.Size(111, 34);
            this.CbxViewMode.Soreted = false;
            this.CbxViewMode.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.CbxViewMode.StylizeFlag = true;
            this.CbxViewMode.TabIndex = 51;
            this.CbxViewMode.Text = "45°";
            this.CbxViewMode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.CbxViewMode.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.CbxViewMode.SelectedIndexChanged += new System.EventHandler(this.CbxViewMode_SelectedIndexChanged);
            // 
            // LblViewMode
            // 
            this.LblViewMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblViewMode.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblViewMode.BorderThickness = 0;
            this.LblViewMode.CornerRadius = 0;
            this.LblViewMode.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblViewMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblViewMode.HighlightPrompt = defaultHighlightPrompt1;
            this.LblViewMode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblViewMode.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblViewMode.Location = new System.Drawing.Point(284, 9);
            this.LblViewMode.MultyLineFlag = false;
            this.LblViewMode.Name = "LblViewMode";
            this.LblViewMode.Size = new System.Drawing.Size(111, 18);
            this.LblViewMode.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblViewMode.StylizeFlag = true;
            this.LblViewMode.TabIndex = 50;
            this.LblViewMode.TabStop = false;
            this.LblViewMode.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShiLeiXing");
            this.LblViewMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblViewMode.Token = null;
            // 
            // LblFramesSelected
            // 
            this.LblFramesSelected.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblFramesSelected.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblFramesSelected.BorderThickness = 0;
            this.LblFramesSelected.CornerRadius = 0;
            this.LblFramesSelected.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblFramesSelected.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblFramesSelected.HighlightPrompt = defaultHighlightPrompt2;
            this.LblFramesSelected.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblFramesSelected.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblFramesSelected.Location = new System.Drawing.Point(42, 9);
            this.LblFramesSelected.MultyLineFlag = false;
            this.LblFramesSelected.Name = "LblFramesSelected";
            this.LblFramesSelected.Size = new System.Drawing.Size(178, 18);
            this.LblFramesSelected.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblFramesSelected.StylizeFlag = true;
            this.LblFramesSelected.TabIndex = 53;
            this.LblFramesSelected.TabStop = false;
            this.LblFramesSelected.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhengXuanZe");
            this.LblFramesSelected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblFramesSelected.Token = null;
            // 
            // BtnFramesSelected
            // 
            this.BtnFramesSelected.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.BtnFramesSelected.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnFramesSelected.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnFramesSelected.BorderThickness = 1;
            this.BtnFramesSelected.CornerRadius = 0;
            this.BtnFramesSelected.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnFramesSelected.DaskArray = null;
            this.BtnFramesSelected.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnFramesSelected.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnFramesSelected.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnFramesSelected.Height = 34;
            this.BtnFramesSelected.Icon = null;
            this.BtnFramesSelected.IconOffset = 10;
            this.BtnFramesSelected.IconSize = new System.Drawing.Size(24, 24);
            this.BtnFramesSelected.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnFramesSelected.IsIndicatorShow = false;
            this.BtnFramesSelected.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnFramesSelected.Location = new System.Drawing.Point(42, 32);
            this.BtnFramesSelected.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnFramesSelected.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnFramesSelected.MouseInBorderThickness = 1;
            this.BtnFramesSelected.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnFramesSelected.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnFramesSelected.Name = "BtnFramesSelected";
            this.BtnFramesSelected.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnFramesSelected.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnFramesSelected.PressedBorderThickness = 1;
            this.BtnFramesSelected.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnFramesSelected.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnFramesSelected.Size = new System.Drawing.Size(178, 34);
            this.BtnFramesSelected.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnFramesSelected.StylizeFlag = true;
            this.BtnFramesSelected.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnFramesSelected.SVGPath = "";
            this.BtnFramesSelected.TabIndex = 54;
            this.BtnFramesSelected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnFramesSelected.Click += new System.EventHandler(this.BtnFramesSelected_Click);
            // 
            // SegmentSelectPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.Controls.Add(this.BtnFramesSelected);
            this.Controls.Add(this.LblFramesSelected);
            this.Controls.Add(this.CbxViewMode);
            this.Controls.Add(this.LblViewMode);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SegmentSelectPage";
            this.Size = new System.Drawing.Size(435, 180);
            this.Load += new System.EventHandler(this.FragmentSelectPage_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ScopeX.UserControls.ComboBoxEx CbxViewMode;
        private ScopeX.UserControls.ScopeXLabel LblViewMode;
        private ScopeX.UserControls.ScopeXLabel LblFramesSelected;
        private ScopeX.UserControls.ScopeXIconButton BtnFramesSelected;
    }
}
