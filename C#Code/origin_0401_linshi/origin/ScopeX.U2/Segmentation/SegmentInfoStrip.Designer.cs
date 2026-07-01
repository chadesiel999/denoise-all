namespace ScopeX.U2
{
    partial class SegmentInfoStrip
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
            UpcCollectState = new UserControls.ScopeXProcessEllipse();
            TlpFragment = new System.Windows.Forms.TableLayoutPanel();
            LblTitle = new UserControls.ScopeXLabel();
            lblCount = new UserControls.ScopeXLabel();
            TlpFragment.SuspendLayout();
            SuspendLayout();
            // 
            // UpcCollectState
            // 
            UpcCollectState.BackColor = System.Drawing.Color.Transparent;
            UpcCollectState.BackEllipseColor = System.Drawing.Color.FromArgb(80, 85, 95);
            UpcCollectState.Dock = System.Windows.Forms.DockStyle.Fill;
            UpcCollectState.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            UpcCollectState.ForeColor = System.Drawing.Color.Black;
            UpcCollectState.InnerEllipseColor = System.Drawing.Color.Silver;
            UpcCollectState.IsShowInnerEllipseBorder = false;
            UpcCollectState.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            UpcCollectState.Location = new System.Drawing.Point(1, 28);
            UpcCollectState.Margin = new System.Windows.Forms.Padding(1, 0, 1, 1);
            UpcCollectState.MaxValue = 100;
            UpcCollectState.Name = "UpcCollectState";
            UpcCollectState.ShowType = UserControls.ShowType.Ring;
            UpcCollectState.Size = new System.Drawing.Size(73, 71);
            UpcCollectState.StyleFlags = UserControls.Style.StyleFlag.None;
            UpcCollectState.StylizeFlag = true;
            UpcCollectState.TabIndex = 1;
            UpcCollectState.Value = 25;
            UpcCollectState.ValueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            UpcCollectState.ValueMargin = 0;
            UpcCollectState.ValueType = UserControls.ProcessBarValueType.Percent;
            UpcCollectState.ValueWidth = 12;
            UpcCollectState.Click += UpcCollectState_Click;
            // 
            // TlpFragment
            // 
            TlpFragment.BackColor = System.Drawing.SystemColors.WindowFrame;
            TlpFragment.ColumnCount = 2;
            TlpFragment.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            TlpFragment.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpFragment.Controls.Add(LblTitle, 0, 0);
            TlpFragment.Controls.Add(lblCount, 1, 1);
            TlpFragment.Controls.Add(UpcCollectState, 0, 1);
            TlpFragment.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpFragment.Location = new System.Drawing.Point(0, 0);
            TlpFragment.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            TlpFragment.Name = "TlpFragment";
            TlpFragment.RowCount = 2;
            TlpFragment.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            TlpFragment.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpFragment.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TlpFragment.Size = new System.Drawing.Size(259, 100);
            TlpFragment.TabIndex = 0;
            // 
            // LblTitle
            // 
            LblTitle.BackColor = System.Drawing.Color.FromArgb(61, 62, 69);
            LblTitle.BorderColor = System.Drawing.Color.Black;
            LblTitle.BorderThickness = 0;
            TlpFragment.SetColumnSpan(LblTitle, 2);
            LblTitle.CornerRadius = 0;
            LblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            LblTitle.Enabled = false;
            LblTitle.Font = new System.Drawing.Font("MiSans", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblTitle.ForeColor = System.Drawing.Color.White;
            LblTitle.HighlightPrompt = defaultHighlightPrompt1;
            LblTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblTitle.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTitle.Location = new System.Drawing.Point(1, 1);
            LblTitle.Margin = new System.Windows.Forms.Padding(1);
            LblTitle.MultyLineFlag = false;
            LblTitle.Name = "LblTitle";
            LblTitle.Size = new System.Drawing.Size(257, 26);
            LblTitle.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            LblTitle.StylizeFlag = true;
            LblTitle.TabIndex = 57;
            LblTitle.TabStop = false;
            LblTitle.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShunXuMoShi");
            LblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblTitle.Token = null;
            // 
            // lblCount
            // 
            lblCount.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            lblCount.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            lblCount.BorderThickness = 0;
            lblCount.CornerRadius = 0;
            lblCount.Dock = System.Windows.Forms.DockStyle.Fill;
            lblCount.Font = new System.Drawing.Font("MiSans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblCount.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            lblCount.HighlightPrompt = defaultHighlightPrompt2;
            lblCount.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            lblCount.Location = new System.Drawing.Point(75, 28);
            lblCount.Margin = new System.Windows.Forms.Padding(0, 0, 1, 1);
            lblCount.MultyLineFlag = false;
            lblCount.Name = "lblCount";
            lblCount.Size = new System.Drawing.Size(183, 71);
            lblCount.StyleFlags = UserControls.Style.StyleFlag.None;
            lblCount.StylizeFlag = false;
            lblCount.TabIndex = 56;
            lblCount.TabStop = false;
            lblCount.Text = "41861133 / 41861133";
            lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblCount.Token = null;
            // 
            // SegmentInfoStrip
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(TlpFragment);
            Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            Name = "SegmentInfoStrip";
            Size = new System.Drawing.Size(259, 100);
            TlpFragment.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel TlpFragment;
        private UserControls.ScopeXProcessEllipse UpcCollectState;
        private UserControls.ScopeXLabel lblCount;
        private UserControls.ScopeXLabel LblTitle;
    }
}
