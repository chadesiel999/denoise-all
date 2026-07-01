
namespace ScopeX.U2
{
    partial class PwrDifferGuideForm
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
            this.TlpGuide = new System.Windows.Forms.TableLayoutPanel();
            this.LblGuide = new ScopeX.UserControls.ScopeXLabel();
            this.PbGuide = new System.Windows.Forms.PictureBox();
            this.TlpGuide.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PbGuide)).BeginInit();
            this.SuspendLayout();
            // 
            // TlpGuide
            // 
            this.TlpGuide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.TlpGuide.ColumnCount = 2;
            this.TlpGuide.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 69.875F));
            this.TlpGuide.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.125F));
            this.TlpGuide.Controls.Add(this.LblGuide, 1, 0);
            this.TlpGuide.Controls.Add(this.PbGuide, 0, 0);
            this.TlpGuide.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpGuide.Location = new System.Drawing.Point(0, 45);
            this.TlpGuide.Name = "TlpGuide";
            this.TlpGuide.RowCount = 1;
            this.TlpGuide.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpGuide.Size = new System.Drawing.Size(728, 256);
            this.TlpGuide.TabIndex = 0;
            // 
            // LblGuide
            // 
            this.LblGuide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblGuide.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblGuide.BorderThickness = 0;
            this.LblGuide.CornerRadius = 0;
            this.LblGuide.Dock = System.Windows.Forms.DockStyle.Left;
            this.LblGuide.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblGuide.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblGuide.HighlightPrompt = defaultHighlightPrompt1;
            this.LblGuide.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblGuide.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblGuide.Location = new System.Drawing.Point(511, 3);
            this.LblGuide.MultyLineFlag = true;
            this.LblGuide.Name = "LblGuide";
            this.LblGuide.Size = new System.Drawing.Size(214, 250);
            this.LblGuide.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblGuide.StylizeFlag = false;
            this.LblGuide.TabIndex = 0;
            this.LblGuide.TabStop = false;
            this.LblGuide.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BoXingWeiFenFenXi_CeLiangKaiGuanJingTiGuanZaiKaiGuanGuoChengZhongDianYaHuoDianLiuBoXingDeBianHuaLv__n_nCeLiangBuZou__n1_JiangChaFenTanZhen_DuanYuJingTiGuanDeLouJiXiangLian__n2_ChaFenTanZhen_DuanLianJieDaoJingTiGuanDeYuanJi__n3_") +
    ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JiangDianLiuTanTouYuJingTiGuanDeYuanJiXiangLian");
            this.LblGuide.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblGuide.Token = null;
            // 
            // PbGuide
            // 
            this.PbGuide.BackColor = System.Drawing.Color.Black;
            this.PbGuide.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PbGuide.Image = global::ScopeX.U2.Properties.Resources.SlewRate;
            this.PbGuide.Location = new System.Drawing.Point(3, 3);
            this.PbGuide.Name = "PbGuide";
            this.PbGuide.Size = new System.Drawing.Size(502, 250);
            this.PbGuide.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PbGuide.TabIndex = 84;
            this.PbGuide.TabStop = false;
            // 
            // PwrDifferGuideForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CanClose = false;
            this.ClientSize = new System.Drawing.Size(728, 301);
            this.ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.ControlBox = false;
            this.Controls.Add(this.TlpGuide);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.HeadBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.HeadHeight = 45;
            this.IconInterval = 21;
            this.IconWidth = 26;
            this.IsShowPin = false;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PwrDifferGuideForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BoXingWeiFenLianJieShiYi");
            this.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BoXingWeiFenLianJieShiYi");
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TitleLableHeight = 39;
            this.ToolIconInterval = 21;
            this.ToolIconSize = new System.Drawing.Size(26, 26);
            this.Controls.SetChildIndex(this.TlpGuide, 0);
            this.TlpGuide.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PbGuide)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel TlpGuide;
        private System.Windows.Forms.PictureBox PbGuide;
        private ScopeX.UserControls.ScopeXLabel LblGuide;
    }
}