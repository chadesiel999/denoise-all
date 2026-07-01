
namespace ScopeX.U2
{
    partial class DsoBtmStrip
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
            TlpLv1 = new System.Windows.Forms.TableLayoutPanel();
            BtnAwg2 = new System.Windows.Forms.Button();
            BtnAwg1 = new System.Windows.Forms.Button();
            BtnDigital = new System.Windows.Forms.Button();
            BtnMath = new System.Windows.Forms.Button();
            BtnRadio = new System.Windows.Forms.Button();
            ChnlInfoPanel = new BadgeInfoPanel();
            BtnReference = new System.Windows.Forms.Button();
            BtnBus = new System.Windows.Forms.Button();
            TlpLv1.SuspendLayout();
            SuspendLayout();
            // 
            // TlpLv1
            // 
            TlpLv1.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            TlpLv1.ColumnCount = 7;
            TlpLv1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpLv1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            TlpLv1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TlpLv1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TlpLv1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TlpLv1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TlpLv1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TlpLv1.Controls.Add(BtnAwg2, 5, 1);
            TlpLv1.Controls.Add(BtnAwg1, 5, 0);
            TlpLv1.Controls.Add(BtnDigital, 3, 0);
            TlpLv1.Controls.Add(BtnMath, 1, 0);
            TlpLv1.Controls.Add(BtnRadio, 6, 0);
            TlpLv1.Controls.Add(ChnlInfoPanel, 0, 0);
            TlpLv1.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpLv1.Location = new System.Drawing.Point(0, 0);
            TlpLv1.Name = "TlpLv1";
            TlpLv1.RowCount = 2;
            TlpLv1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpLv1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpLv1.Size = new System.Drawing.Size(1080, 115);
            TlpLv1.TabIndex = 0;
            // 
            // BtnAwg2
            // 
            BtnAwg2.BackColor = System.Drawing.Color.FromArgb(72, 77, 85);
            BtnAwg2.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnAwg2.FlatAppearance.BorderSize = 0;
            BtnAwg2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            BtnAwg2.Font = new System.Drawing.Font("Arial Narrow", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnAwg2.Location = new System.Drawing.Point(949, 62);
            BtnAwg2.Margin = new System.Windows.Forms.Padding(1);
            BtnAwg2.Name = "BtnAwg2";
            BtnAwg2.Size = new System.Drawing.Size(64, 52);
            BtnAwg2.TabIndex = 6;
            BtnAwg2.Text = "G2";
            BtnAwg2.UseVisualStyleBackColor = false;
            BtnAwg2.Click += BtnAwg2_Click;
            BtnAwg2.Paint += Btn_Paint;
            BtnAwg2.MouseDown += btnMouseDown;
            BtnAwg2.MouseUp += btnMouseUp;
            // 
            // BtnAwg1
            // 
            BtnAwg1.BackColor = System.Drawing.Color.FromArgb(72, 77, 85);
            BtnAwg1.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnAwg1.FlatAppearance.BorderSize = 0;
            BtnAwg1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            BtnAwg1.Font = new System.Drawing.Font("Arial Narrow", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnAwg1.Location = new System.Drawing.Point(949, 1);
            BtnAwg1.Margin = new System.Windows.Forms.Padding(1);
            BtnAwg1.Name = "BtnAwg1";
            BtnAwg1.Size = new System.Drawing.Size(64, 59);
            BtnAwg1.TabIndex = 5;
            BtnAwg1.Text = "G1";
            BtnAwg1.UseVisualStyleBackColor = false;
            BtnAwg1.Click += BtnAwg1_Click;
            BtnAwg1.Paint += Btn_Paint;
            BtnAwg1.MouseDown += btnMouseDown;
            BtnAwg1.MouseUp += btnMouseUp;
            // 
            // BtnDigital
            // 
            BtnDigital.BackColor = System.Drawing.Color.FromArgb(72, 77, 85);
            BtnDigital.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnDigital.FlatAppearance.BorderSize = 0;
            BtnDigital.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            BtnDigital.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnDigital.Location = new System.Drawing.Point(883, 1);
            BtnDigital.Margin = new System.Windows.Forms.Padding(1);
            BtnDigital.Name = "BtnDigital";
            TlpLv1.SetRowSpan(BtnDigital, 2);
            BtnDigital.Size = new System.Drawing.Size(64, 113);
            BtnDigital.TabIndex = 3;
            BtnDigital.UseVisualStyleBackColor = false;
            BtnDigital.Click += BtnDigital_Click;
            BtnDigital.Paint += Btn_Paint;
            BtnDigital.MouseDown += btnMouseDown;
            BtnDigital.MouseUp += btnMouseUp;
            // 
            // BtnMath
            // 
            BtnMath.BackColor = System.Drawing.Color.FromArgb(72, 77, 85);
            BtnMath.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnMath.FlatAppearance.BorderSize = 0;
            BtnMath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            BtnMath.Location = new System.Drawing.Point(813, 1);
            BtnMath.Margin = new System.Windows.Forms.Padding(1);
            BtnMath.Name = "BtnMath";
            TlpLv1.SetRowSpan(BtnMath, 2);
            BtnMath.Size = new System.Drawing.Size(68, 113);
            BtnMath.TabIndex = 1;
            BtnMath.UseVisualStyleBackColor = false;
            BtnMath.Click += BtnMath_Click;
            BtnMath.Paint += Btn_Paint;
            BtnMath.MouseDown += btnMouseDown;
            BtnMath.MouseUp += btnMouseUp;
            // 
            // BtnRadio
            // 
            BtnRadio.BackColor = System.Drawing.Color.FromArgb(72, 77, 85);
            BtnRadio.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnRadio.FlatAppearance.BorderSize = 0;
            BtnRadio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            BtnRadio.Location = new System.Drawing.Point(1015, 1);
            BtnRadio.Margin = new System.Windows.Forms.Padding(1);
            BtnRadio.Name = "BtnRadio";
            TlpLv1.SetRowSpan(BtnRadio, 2);
            BtnRadio.Size = new System.Drawing.Size(64, 113);
            BtnRadio.TabIndex = 7;
            BtnRadio.UseVisualStyleBackColor = false;
            BtnRadio.Click += BtnRF_Click;
            BtnRadio.Paint += Btn_Paint;
            BtnRadio.MouseDown += btnMouseDown;
            BtnRadio.MouseUp += btnMouseUp;
            // 
            // ChnlInfoPanel
            // 
            ChnlInfoPanel.BackColor = System.Drawing.Color.FromArgb(33, 33, 40);
            ChnlInfoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            ChnlInfoPanel.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChnlInfoPanel.ForeColor = System.Drawing.Color.Black;
            ChnlInfoPanel.Location = new System.Drawing.Point(0, 0);
            ChnlInfoPanel.Margin = new System.Windows.Forms.Padding(0);
            ChnlInfoPanel.Name = "ChnlInfoPanel";
            TlpLv1.SetRowSpan(ChnlInfoPanel, 2);
            ChnlInfoPanel.ScrollArrowBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ChnlInfoPanel.ScrollArrowMouseInBackColor = System.Drawing.Color.Transparent;
            ChnlInfoPanel.ScrollArrowWidth = 39;
            ChnlInfoPanel.Size = new System.Drawing.Size(812, 115);
            ChnlInfoPanel.TabIndex = 0;
            // 
            // BtnReference
            // 
            BtnReference.BackColor = System.Drawing.Color.FromArgb(72, 77, 85);
            BtnReference.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnReference.FlatAppearance.BorderSize = 0;
            BtnReference.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            BtnReference.Location = new System.Drawing.Point(747, 1);
            BtnReference.Margin = new System.Windows.Forms.Padding(1);
            BtnReference.Name = "BtnReference";
            BtnReference.Size = new System.Drawing.Size(68, 113);
            BtnReference.TabIndex = 2;
            BtnReference.UseVisualStyleBackColor = false;
            BtnReference.Click += BtnReference_Click;
            BtnReference.Paint += Btn_Paint;
            BtnReference.MouseDown += btnMouseDown;
            BtnReference.MouseUp += btnMouseUp;
            // 
            // BtnBus
            // 
            BtnBus.BackColor = System.Drawing.Color.FromArgb(72, 77, 85);
            BtnBus.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnBus.FlatAppearance.BorderSize = 0;
            BtnBus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            BtnBus.Location = new System.Drawing.Point(883, 1);
            BtnBus.Margin = new System.Windows.Forms.Padding(1);
            BtnBus.Name = "BtnBus";
            BtnBus.Size = new System.Drawing.Size(64, 113);
            BtnBus.TabIndex = 4;
            BtnBus.UseVisualStyleBackColor = false;
            BtnBus.Click += BtnBus_Click;
            BtnBus.Paint += Btn_Paint;
            BtnBus.MouseDown += btnMouseDown;
            BtnBus.MouseUp += btnMouseUp;
            // 
            // DsoBtmStrip
            // 
            AllowDrop = true;
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(33, 33, 40);
            Controls.Add(TlpLv1);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ForeColor = System.Drawing.Color.White;
            Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            Name = "DsoBtmStrip";
            Size = new System.Drawing.Size(1080, 115);
            TlpLv1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel TlpLv1;
        private BadgeInfoPanel ChnlInfoPanel;
        internal System.Windows.Forms.Button BtnMath;
        internal System.Windows.Forms.Button BtnReference;
        internal System.Windows.Forms.Button BtnBus;
        internal System.Windows.Forms.Button BtnDigital;
        internal System.Windows.Forms.Button BtnAwg1;
        internal System.Windows.Forms.Button BtnRadio;
        internal System.Windows.Forms.Button BtnAwg2;
    }
}
