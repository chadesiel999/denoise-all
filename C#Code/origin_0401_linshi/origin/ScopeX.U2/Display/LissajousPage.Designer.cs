
namespace ScopeX.U2
{
    partial class LissajousPage
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
            CbxSourceX = new ScopeX.UserControls.SelectComboBox();
            this.LblSourceY = new ScopeX.UserControls.ScopeXLabel();
            this.LblSourceX = new ScopeX.UserControls.ScopeXLabel();
            CbxSourceY = new ScopeX.UserControls.SelectComboBox();
            this.BtnAddLissa = new ScopeX.UserControls.ScopeXIconButton();
            this.SuspendLayout();
            // 
            // CbxSourceX
            // 
            CbxSourceX.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSourceX.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSourceX.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxSourceX.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxSourceX.ForeColor = System.Drawing.Color.White;
            CbxSourceX.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4"
    };
            CbxSourceX.Location = new System.Drawing.Point(23, 36);
            CbxSourceX.Name = "CbxSourceX";
            CbxSourceX.Size = new System.Drawing.Size(85, 30);
            CbxSourceX.TabIndex = 5;
            CbxSourceX.SelectedIndexChanged += CbxSourceX_SelectedIndexChanged;
            // 
            // LblSourceY
            // 
            this.LblSourceY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblSourceY.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblSourceY.BorderThickness = 0;
            this.LblSourceY.CornerRadius = 0;
            this.LblSourceY.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblSourceY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblSourceY.HighlightPrompt = defaultHighlightPrompt1;
            this.LblSourceY.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblSourceY.Location = new System.Drawing.Point(152, 8);
            this.LblSourceY.MultyLineFlag = false;
            this.LblSourceY.Name = "LblSourceY";
            this.LblSourceY.Size = new System.Drawing.Size(85, 18);
            this.LblSourceY.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblSourceY.StylizeFlag = true;
            this.LblSourceY.TabIndex = 2;
            this.LblSourceY.TabStop = false;
            this.LblSourceY.Text = "Y";
            this.LblSourceY.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblSourceY.Token = null;
            // 
            // LblSourceX
            // 
            this.LblSourceX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblSourceX.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblSourceX.BorderThickness = 0;
            this.LblSourceX.CornerRadius = 0;
            this.LblSourceX.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblSourceX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblSourceX.HighlightPrompt = defaultHighlightPrompt2;
            this.LblSourceX.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblSourceX.Location = new System.Drawing.Point(23, 8);
            this.LblSourceX.MultyLineFlag = false;
            this.LblSourceX.Name = "LblSourceX";
            this.LblSourceX.Size = new System.Drawing.Size(85, 18);
            this.LblSourceX.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblSourceX.StylizeFlag = true;
            this.LblSourceX.TabIndex = 0;
            this.LblSourceX.TabStop = false;
            this.LblSourceX.Text = "X";
            this.LblSourceX.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblSourceX.Token = null;
            // 
            // CbxSourceY
            // 
            CbxSourceY.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSourceY.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSourceY.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxSourceY.DataSource = null;
            CbxSourceY.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxSourceY.ForeColor = System.Drawing.Color.White;
            CbxSourceY.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4"
    };
            CbxSourceY.Location = new System.Drawing.Point(152, 36);
            CbxSourceY.Name = "CbxSourceY";
            CbxSourceY.Size = new System.Drawing.Size(85, 30);
            CbxSourceY.TabIndex = 6;
            CbxSourceY.SelectedIndexChanged += CbxSourceY_SelectedIndexChanged;
            // 
            // BtnAddLissa
            // 
            this.BtnAddLissa.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnAddLissa.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnAddLissa.BorderThickness = 1;
            this.BtnAddLissa.CornerRadius = 0;
            this.BtnAddLissa.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnAddLissa.DaskArray = null;
            this.BtnAddLissa.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnAddLissa.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnAddLissa.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnAddLissa.Height = 30;
            this.BtnAddLissa.Icon = null;
            this.BtnAddLissa.IconOffset = 10;
            this.BtnAddLissa.IconSize = new System.Drawing.Size(24, 24);
            this.BtnAddLissa.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnAddLissa.IsIndicatorShow = false;
            this.BtnAddLissa.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnAddLissa.Location = new System.Drawing.Point(279, 36);
            this.BtnAddLissa.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnAddLissa.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnAddLissa.MouseInBorderThickness = 0;
            this.BtnAddLissa.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnAddLissa.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnAddLissa.Name = "BtnAddLissa";
            this.BtnAddLissa.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnAddLissa.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnAddLissa.PressedBorderThickness = 0;
            this.BtnAddLissa.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnAddLissa.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnAddLissa.Size = new System.Drawing.Size(85, 30);
            this.BtnAddLissa.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnAddLissa.StylizeFlag = true;
            this.BtnAddLissa.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnAddLissa.SVGPath = "";
            this.BtnAddLissa.TabIndex = 4;
            this.BtnAddLissa.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TianJia");
            this.BtnAddLissa.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnAddLissa.Click += new System.EventHandler(this.BtnAddLissa_Click);
            // 
            // LissajousPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.Controls.Add(this.CbxSourceX);
            this.Controls.Add(this.LblSourceY);
            this.Controls.Add(this.LblSourceX);
            this.Controls.Add(this.CbxSourceY);
            this.Controls.Add(this.BtnAddLissa);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "LissajousPage";
            this.Size = new System.Drawing.Size(389, 85);
            this.ResumeLayout(false);

        }

        #endregion

        //private ScopeX.UserControls.ComboBoxEx CbxSourceX;
        private ScopeX.UserControls.ScopeXLabel LblSourceY;
        private ScopeX.UserControls.ScopeXLabel LblSourceX;
        //private ScopeX.UserControls.ComboBoxEx CbxSourceY;
        private ScopeX.UserControls.ScopeXIconButton BtnAddLissa;
        private ScopeX.UserControls.SelectComboBox CbxSourceX;
        private ScopeX.UserControls.SelectComboBox CbxSourceY;
    }
}
