namespace ScopeX.U2.Demo
{
    partial class PowerAnalysisPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PowerAnalysisPage));
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            CbxMode = new UserControls.SelectComboBox();
            LblType = new UserControls.ScopeXLabel();
            BtnAdd = new UserControls.ScopeXIconButton();
            SuspendLayout();
            // 
            // CbxMode
            // 
            CbxMode.AutoSize = true;
            CbxMode.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxMode.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxMode.ComBorderColor = System.Drawing.Color.Blue;
            CbxMode.DataSource = (System.Collections.IList)resources.GetObject("CbxMode.DataSource");
            CbxMode.ExtText = "";
            CbxMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxMode.ForeColor = System.Drawing.Color.White;
            CbxMode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            CbxMode.Location = new System.Drawing.Point(22, 60);
            CbxMode.MaximumSize = new System.Drawing.Size(99999, 99999);
            CbxMode.Name = "CbxMode";
            CbxMode.SelectIndex = -1;
            CbxMode.SelectValue = null;
            CbxMode.Size = new System.Drawing.Size(200, 30);
            CbxMode.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxMode.StylizeFlag = true;
            CbxMode.TabIndex = 24;
            // 
            // LblType
            // 
            LblType.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblType.BorderThickness = 0;
            LblType.CornerRadius = 0;
            LblType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblType.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblType.HighlightPrompt = defaultHighlightPrompt1;
            LblType.IsOmittext = true;
            LblType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblType.Location = new System.Drawing.Point(22, 32);
            LblType.MultyLineFlag = false;
            LblType.Name = "LblType";
            LblType.Size = new System.Drawing.Size(200, 24);
            LblType.StyleFlags = UserControls.Style.StyleFlag.None;
            LblType.StylizeFlag = true;
            LblType.TabIndex = 23;
            LblType.TabStop = false;
            LblType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblType.Token = null;
            // 
            // BtnAdd
            // 
            BtnAdd.Adjustable = false;
            BtnAdd.BackColor = System.Drawing.Color.Transparent;
            BtnAdd.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAdd.BorderThickness = 0;
            BtnAdd.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnAdd.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnAdd.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnAdd.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnAdd.CornerRadius = 0;
            BtnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnAdd.DaskArray = null;
            BtnAdd.DoubleClickEnable = true;
            BtnAdd.DropKey = System.Windows.Forms.Keys.Space;
            BtnAdd.FineEnable = false;
            BtnAdd.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnAdd.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnAdd.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnAdd.Height = 30;
            BtnAdd.Icon = null;
            BtnAdd.IconOffset = 10;
            BtnAdd.IconSize = new System.Drawing.Size(24, 24);
            BtnAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnAdd.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnAdd.IsChoosed = false;
            BtnAdd.IsIndicatorShow = false;
            BtnAdd.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnAdd.Location = new System.Drawing.Point(280, 60);
            BtnAdd.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAdd.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAdd.MouseInBorderThickness = 0;
            BtnAdd.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAdd.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.PressedBackColor = System.Drawing.Color.Gray;
            BtnAdd.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAdd.PressedBorderThickness = 0;
            BtnAdd.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAdd.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnAdd.Size = new System.Drawing.Size(106, 30);
            BtnAdd.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnAdd.StylizeFlag = true;
            BtnAdd.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAdd.SVGPath = "";
            BtnAdd.TabIndex = 25;
            BtnAdd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnAdd.Click += BtnAdd_Click;
            // 
            // PowerAnalysisPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(BtnAdd);
            Controls.Add(CbxMode);
            Controls.Add(LblType);
            Name = "PowerAnalysisPage";
            Size = new System.Drawing.Size(494, 290);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private UserControls.SelectComboBox CbxMode;
        private UserControls.ScopeXLabel LblType;
        private UserControls.ScopeXIconButton BtnAdd;
    }
}
