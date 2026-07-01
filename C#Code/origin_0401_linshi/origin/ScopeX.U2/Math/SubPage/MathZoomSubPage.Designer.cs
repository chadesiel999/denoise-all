
namespace ScopeX.U2
{
    partial class MathZoomSubPage
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
            CbxSource = new ScopeX.UserControls.ComboBoxEx();
            LblSource = new ScopeX.UserControls.ScopeXLabel();
            SuspendLayout();
            // 
            // CbxSource
            // 
            CbxSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CbxSource.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.BorderThickness = 0;
            CbxSource.CornerRadius = 0;
            CbxSource.DataSource = null;
            CbxSource.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            CbxSource.DropDownHeight = 200;
            CbxSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CbxSource.DropDownWidth = 100;
            CbxSource.DropKey = System.Windows.Forms.Keys.Space;
            CbxSource.DroppedDown = false;
            CbxSource.FocusColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxSource.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            CbxSource.FormattingEnabled = true;
            CbxSource.GetDisPlayName = null;
            CbxSource.Height = 30;
            CbxSource.ImageMode = false;
            CbxSource.ItemHeight = 28;
            CbxSource.Items.AddRange(new object[] { "C1", "C2", "C3", "C4", "R1", "R2", "R3", "R4" });
            CbxSource.KeyDropEnble = true;
            CbxSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            CbxSource.Location = new System.Drawing.Point(10, 43);
            CbxSource.MaxDropDownItems = 8;
            CbxSource.Name = "CbxSource";
            CbxSource.RectBtnWidth = 20;
            CbxSource.SelectedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            CbxSource.SelectedIndex = -1;
            CbxSource.SelectedItem = null;
            CbxSource.SelectedText = "";
            CbxSource.Size = new System.Drawing.Size(100, 30);
            CbxSource.Soreted = false;
            CbxSource.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            CbxSource.StylizeFlag = true;
            CbxSource.TabIndex = 1;
            CbxSource.Text = "";
            CbxSource.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            CbxSource.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            CbxSource.SelectedIndexChanged += CbxSource_SelectedIndexChanged;
            CbxSource.Click += CbxSource_Click;
            // 
            // LblSource
            // 
            LblSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            LblSource.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblSource.BorderThickness = 0;
            LblSource.CornerRadius = 0;
            LblSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSource.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblSource.HighlightPrompt = defaultHighlightPrompt1;
            LblSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSource.Location = new System.Drawing.Point(10, 15);
            LblSource.MultyLineFlag = false;
            LblSource.Name = "LblSource";
            LblSource.Size = new System.Drawing.Size(100, 18);
            LblSource.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblSource.StylizeFlag = true;
            LblSource.TabIndex = 0;
            LblSource.TabStop = false;
            LblSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Yuan");
            LblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSource.Token = null;
            // 
            // MathZoomSubPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(CbxSource);
            Controls.Add(LblSource);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "MathZoomSubPage";
            Padding = new System.Windows.Forms.Padding(3);
            Size = new System.Drawing.Size(252, 101);
            ResumeLayout(false);
        }

        #endregion
        private ScopeX.UserControls.ComboBoxEx CbxSource;
        private ScopeX.UserControls.ScopeXLabel LblSource;
    }
}
