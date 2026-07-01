namespace ScopeX.U2
{
    partial class ManufacturerAdatperPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManufacturerAdatperPage));
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            CbxAdapter = new UserControls.SelectComboBox();
            LblAdapter = new UserControls.ScopeXLabel();
            SuspendLayout();
            // 
            // CbxAdapter
            // 
            CbxAdapter.AutoSize = true;
            CbxAdapter.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxAdapter.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxAdapter.ComBorderColor = System.Drawing.Color.Blue;
            CbxAdapter.DataSource = (System.Collections.IList)resources.GetObject("CbxAdapter.DataSource");
            CbxAdapter.ExtText = "";
            CbxAdapter.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxAdapter.ForeColor = System.Drawing.Color.White;
            CbxAdapter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            CbxAdapter.Location = new System.Drawing.Point(44, 47);
            CbxAdapter.MaximumSize = new System.Drawing.Size(9999, 9999);
            CbxAdapter.Name = "CbxAdapter";
            CbxAdapter.SelectIndex = -1;
            CbxAdapter.SelectValue = null;
            CbxAdapter.Size = new System.Drawing.Size(119, 30);
            CbxAdapter.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxAdapter.StylizeFlag = true;
            CbxAdapter.TabIndex = 13;
            // 
            // LblAdapter
            // 
            LblAdapter.BackColor = System.Drawing.Color.Empty;
            LblAdapter.BorderColor = System.Drawing.Color.Black;
            LblAdapter.BorderThickness = 0;
            LblAdapter.CornerRadius = 0;
            LblAdapter.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblAdapter.ForeColor = System.Drawing.Color.White;
            LblAdapter.HighlightPrompt = defaultHighlightPrompt1;
            LblAdapter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblAdapter.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblAdapter.Location = new System.Drawing.Point(44, 20);
            LblAdapter.MultyLineFlag = false;
            LblAdapter.Name = "LblAdapter";
            LblAdapter.Size = new System.Drawing.Size(192, 21);
            LblAdapter.StyleFlags = UserControls.Style.StyleFlag.None;
            LblAdapter.StylizeFlag = true;
            LblAdapter.TabIndex = 14;
            LblAdapter.TabStop = false;
            LblAdapter.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.ManufacturerAdatperPage.LblAdapter");
            LblAdapter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblAdapter.Token = null;
            // 
            // ManufacturerAdatperPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            Controls.Add(LblAdapter);
            Controls.Add(CbxAdapter);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "ManufacturerAdatperPage";
            Size = new System.Drawing.Size(459, 410);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private UserControls.SelectComboBox CbxAdapter;
        private UserControls.ScopeXLabel LblAdapter;
    }
}
