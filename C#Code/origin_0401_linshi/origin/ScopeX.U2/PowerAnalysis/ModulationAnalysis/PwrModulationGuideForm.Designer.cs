
namespace ScopeX.U2
{
    partial class PwrModulationGuideForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new ScopeX.UserControls.DefaultHighlightPrompt();
            TlpGuide = new System.Windows.Forms.TableLayoutPanel();
            LblGuide = new ScopeX.UserControls.ScopeXLabel();
            PbGuide = new System.Windows.Forms.PictureBox();
            TlpGuide.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PbGuide).BeginInit();
            SuspendLayout();
            // 
            // TlpGuide
            // 
            TlpGuide.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            TlpGuide.ColumnCount = 2;
            TlpGuide.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.875F));
            TlpGuide.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.125F));
            TlpGuide.Controls.Add(LblGuide, 1, 0);
            TlpGuide.Controls.Add(PbGuide, 0, 0);
            TlpGuide.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpGuide.Location = new System.Drawing.Point(2, 45);
            TlpGuide.Name = "TlpGuide";
            TlpGuide.RowCount = 1;
            TlpGuide.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpGuide.Size = new System.Drawing.Size(724, 254);
            TlpGuide.TabIndex = 0;
            // 
            // LblGuide
            // 
            LblGuide.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblGuide.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblGuide.BorderThickness = 0;
            LblGuide.CornerRadius = 0;
            LblGuide.Dock = System.Windows.Forms.DockStyle.Left;
            LblGuide.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblGuide.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblGuide.HighlightPrompt = defaultHighlightPrompt1;
            LblGuide.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblGuide.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblGuide.Location = new System.Drawing.Point(508, 3);
            LblGuide.MultyLineFlag = false;
            LblGuide.Name = "LblGuide";
            LblGuide.IsOmittext = false;
            LblGuide.Size = new System.Drawing.Size(313, 250);
            LblGuide.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblGuide.StylizeFlag = false;
            LblGuide.TabIndex = 0;
            LblGuide.TabStop = false;
            LblGuide.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ModulationAnalysisDescription");
            LblGuide.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblGuide.Token = null;
            // 
            // PbGuide
            // 
            PbGuide.BackColor = System.Drawing.Color.Black;
            PbGuide.Dock = System.Windows.Forms.DockStyle.Fill;
            PbGuide.Image = Properties.Resources.Modulation;
            PbGuide.Location = new System.Drawing.Point(3, 3);
            PbGuide.Name = "PbGuide";
            PbGuide.Size = new System.Drawing.Size(499, 250);
            PbGuide.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            PbGuide.TabIndex = 84;
            PbGuide.TabStop = false;
            // 
            // PwrModulationGuideForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            CanClose = false;
            ClientSize = new System.Drawing.Size(800, 500);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(TlpGuide);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            HeadHeight = 45;
            IconInterval = 21;
            IconWidth = 26;
            IsShowPin = false;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PwrQualityGuideForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TiaoZhiFenXiLianJieShiYi");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TiaoZhiFenXiLianJieShiYi");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleLableHeight = 39;
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Load += PwrModulationGuideForm_Load;
            Controls.SetChildIndex(TlpGuide, 0);
            TlpGuide.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PbGuide).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpGuide;
        private System.Windows.Forms.PictureBox PbGuide;
        private ScopeX.UserControls.ScopeXLabel LblGuide;
    }
}