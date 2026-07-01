
namespace ScopeX.U2
{
    partial class PwrHarmonicGuidePage
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
            TlpGuide = new System.Windows.Forms.TableLayoutPanel();
            LblGuide = new UserControls.ScopeXLabel();
            PbGuide = new System.Windows.Forms.PictureBox();
            TlpGuide.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PbGuide).BeginInit();
            SuspendLayout();
            // 
            // TlpGuide
            // 
            TlpGuide.AutoSize = true;
            TlpGuide.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            TlpGuide.ColumnCount = 1;
            TlpGuide.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpGuide.Controls.Add(LblGuide, 0, 1);
            TlpGuide.Controls.Add(PbGuide, 0, 0);
            TlpGuide.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpGuide.Location = new System.Drawing.Point(0, 0);
            TlpGuide.Margin = new System.Windows.Forms.Padding(0);
            TlpGuide.Name = "TlpGuide";
            TlpGuide.RowCount = 2;
            TlpGuide.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            TlpGuide.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            TlpGuide.Size = new System.Drawing.Size(445, 342);
            TlpGuide.TabIndex = 1;
            // 
            // LblGuide
            // 
            LblGuide.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblGuide.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblGuide.BorderThickness = 0;
            LblGuide.CornerRadius = 0;
            LblGuide.Dock = System.Windows.Forms.DockStyle.Fill;
            LblGuide.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblGuide.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblGuide.HighlightPrompt = defaultHighlightPrompt1;
            LblGuide.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblGuide.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblGuide.Location = new System.Drawing.Point(3, 150);
            LblGuide.MultyLineFlag = false;
            LblGuide.Name = "LblGuide";
            LblGuide.ToolTip = false;
            LblGuide.IsOmittext = false;
            LblGuide.Size = new System.Drawing.Size(439, 120);
            LblGuide.StyleFlags = UserControls.Style.StyleFlag.None;
            LblGuide.StylizeFlag = false;
            LblGuide.TabIndex = 0;
            LblGuide.TabStop = false;
            LblGuide.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblGuide.Token = null;
            // 
            // PbGuide
            // 
            PbGuide.Dock = System.Windows.Forms.DockStyle.Fill;
            PbGuide.Image = Properties.Resources.Harmonics;
            PbGuide.Location = new System.Drawing.Point(3, 3);
            PbGuide.Name = "PbGuide";
            PbGuide.Size = new System.Drawing.Size(439, 216);
            PbGuide.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            PbGuide.TabIndex = 1;
            PbGuide.TabStop = false;
            // 
            // PwrHarmonicGuidePage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(TlpGuide);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "PwrHarmonicGuidePage";
            Size = new System.Drawing.Size(445, 350);
            TlpGuide.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PbGuide).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ScopeX.UserControls.ScopeXLabel LblRefFreq;
        private ScopeX.UserControls.UIRadioButtonGroup RdoRefFreq;
        private ScopeX.UserControls.ScopeXLabel LblStatistic;
        private ScopeX.UserControls.ScopeXSwitchButton ChkStatistic;
        private ScopeX.UserControls.ScopeXIconButton BtnResetStatistic;
        private ScopeX.UserControls.ScopeXIconButton BtnShowPowerWfm;
        private System.Windows.Forms.TableLayoutPanel TlpGuide;
        private ScopeX.UserControls.ScopeXLabel LblGuide;
        private System.Windows.Forms.PictureBox PbGuide;
    }
}
