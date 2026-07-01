
namespace ScopeX.U2
{
    partial class StatusToolStrip
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.timeLabel = new ScopeX.UserControls.ScopeXLabel();
            this.dateLabel = new ScopeX.UserControls.ScopeXLabel();
            this.BtnNet = new ScopeX.UserControls.ScopeXIconButton();
            this.BtnTransform = new ScopeX.UserControls.ScopeXIconButton();
            this.BtnUsb = new ScopeX.UserControls.ScopeXIconButton();
            this.BtnVoice = new ScopeX.UserControls.ScopeXIconButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Controls.Add(this.timeLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dateLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.BtnUsb, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.BtnTransform, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.BtnNet, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.BtnVoice, 4, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(160, 115);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // timeLabel
            // 
            this.timeLabel.BackColor = System.Drawing.Color.Empty;
            this.timeLabel.BorderColor = System.Drawing.Color.Black;
            this.timeLabel.BorderThickness = 0;
            this.tableLayoutPanel1.SetColumnSpan(this.timeLabel, 5);
            this.timeLabel.CornerRadius = 0;
            this.timeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.timeLabel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.timeLabel.Location = new System.Drawing.Point(3, 3);
            this.timeLabel.MultyLineFlag = false;
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(154, 31);
            this.timeLabel.TabIndex = 0;
            this.timeLabel.Text = "ScopeXLabel1";
            this.timeLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // dateLabel
            // 
            this.dateLabel.BackColor = System.Drawing.Color.Empty;
            this.dateLabel.BorderColor = System.Drawing.Color.Black;
            this.dateLabel.BorderThickness = 0;
            this.tableLayoutPanel1.SetColumnSpan(this.dateLabel, 5);
            this.dateLabel.CornerRadius = 0;
            this.dateLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dateLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.dateLabel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.dateLabel.Location = new System.Drawing.Point(3, 40);
            this.dateLabel.MultyLineFlag = false;
            this.dateLabel.Name = "dateLabel";
            this.dateLabel.Size = new System.Drawing.Size(154, 31);
            this.dateLabel.TabIndex = 1;
            this.dateLabel.Text = "ScopeXLabel2";
            this.dateLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // BtnNet
            // 
            this.BtnNet.BackColor = System.Drawing.Color.Transparent;
            this.BtnNet.BorderColor = System.Drawing.Color.Black;
            this.BtnNet.BorderThickness = 0;
            this.BtnNet.CornerRadius = 0;
            this.BtnNet.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnNet.DaskArray = null;
            this.BtnNet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnNet.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnNet.ForeColor = System.Drawing.Color.Black;
            this.BtnNet.Icon = null;
            this.BtnNet.IconOffset = 1;
            this.BtnNet.IconSize = new System.Drawing.Size(28, 28);
            this.BtnNet.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnNet.Location = new System.Drawing.Point(90, 79);
            this.BtnNet.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.BtnNet.MouseinBackColor = System.Drawing.Color.Transparent;
            this.BtnNet.MouseinBorderColor = System.Drawing.Color.Blue;
            this.BtnNet.MouseInBorderThickness = 0;
            this.BtnNet.MouseinForeColor = System.Drawing.Color.Blue;
            this.BtnNet.MouseinSvgForeColor = System.Drawing.Color.White;
            this.BtnNet.Name = "BtnNet";
            this.BtnNet.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnNet.PressedBorderColor = System.Drawing.Color.Blue;
            this.BtnNet.PressedBorderThickness = 0;
            this.BtnNet.PressedForeColor = System.Drawing.Color.Blue;
            this.BtnNet.PressedSvgForeColor = System.Drawing.Color.White;
            this.BtnNet.Size = new System.Drawing.Size(30, 31);
            this.BtnNet.SVGForeColor = System.Drawing.Color.White;
            this.BtnNet.SVGPath = global::ScopeX.U2.Properties.Resources.StatusToolStripNetSvg;
            this.BtnNet.TabIndex = 3;
            this.BtnNet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BtnTransform
            // 
            this.BtnTransform.BackColor = System.Drawing.Color.Transparent;
            this.BtnTransform.BorderColor = System.Drawing.Color.Black;
            this.BtnTransform.BorderThickness = 0;
            this.BtnTransform.CornerRadius = 0;
            this.BtnTransform.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnTransform.DaskArray = null;
            this.BtnTransform.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnTransform.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnTransform.ForeColor = System.Drawing.Color.Black;
            this.BtnTransform.Icon = null;
            this.BtnTransform.IconOffset = 1;
            this.BtnTransform.IconSize = new System.Drawing.Size(28, 28);
            this.BtnTransform.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnTransform.Location = new System.Drawing.Point(55, 79);
            this.BtnTransform.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.BtnTransform.MouseinBackColor = System.Drawing.Color.Transparent;
            this.BtnTransform.MouseinBorderColor = System.Drawing.Color.Blue;
            this.BtnTransform.MouseInBorderThickness = 0;
            this.BtnTransform.MouseinForeColor = System.Drawing.Color.Blue;
            this.BtnTransform.MouseinSvgForeColor = System.Drawing.Color.White;
            this.BtnTransform.Name = "BtnTransform";
            this.BtnTransform.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnTransform.PressedBorderColor = System.Drawing.Color.Blue;
            this.BtnTransform.PressedBorderThickness = 0;
            this.BtnTransform.PressedForeColor = System.Drawing.Color.Blue;
            this.BtnTransform.PressedSvgForeColor = System.Drawing.Color.White;
            this.BtnTransform.Size = new System.Drawing.Size(30, 31);
            this.BtnTransform.SVGForeColor = System.Drawing.Color.White;
            this.BtnTransform.SVGPath = global::ScopeX.U2.Properties.Resources.StatusToolStripTransformSvg;
            this.BtnTransform.TabIndex = 2;
            this.BtnTransform.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BtnUsb
            // 
            this.BtnUsb.BackColor = System.Drawing.Color.Transparent;
            this.BtnUsb.BorderColor = System.Drawing.Color.Black;
            this.BtnUsb.BorderThickness = 0;
            this.BtnUsb.CornerRadius = 0;
            this.BtnUsb.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnUsb.DaskArray = null;
            this.BtnUsb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnUsb.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnUsb.ForeColor = System.Drawing.Color.Black;
            this.BtnUsb.Icon = null;
            this.BtnUsb.IconOffset = 1;
            this.BtnUsb.IconSize = new System.Drawing.Size(28, 28);
            this.BtnUsb.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnUsb.Location = new System.Drawing.Point(20, 79);
            this.BtnUsb.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.BtnUsb.MouseinBackColor = System.Drawing.Color.Transparent;
            this.BtnUsb.MouseinBorderColor = System.Drawing.Color.Blue;
            this.BtnUsb.MouseInBorderThickness = 0;
            this.BtnUsb.MouseinForeColor = System.Drawing.Color.Blue;
            this.BtnUsb.MouseinSvgForeColor = System.Drawing.Color.White;
            this.BtnUsb.Name = "BtnUsb";
            this.BtnUsb.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnUsb.PressedBorderColor = System.Drawing.Color.Blue;
            this.BtnUsb.PressedBorderThickness = 0;
            this.BtnUsb.PressedForeColor = System.Drawing.Color.Blue;
            this.BtnUsb.PressedSvgForeColor = System.Drawing.Color.White;
            this.BtnUsb.Size = new System.Drawing.Size(30, 31);
            this.BtnUsb.SVGForeColor = System.Drawing.Color.White;
            this.BtnUsb.SVGPath = global::ScopeX.U2.Properties.Resources.StatusToolStripUsbSvg;
            this.BtnUsb.TabIndex = 1;
            this.BtnUsb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BtnVoice
            // 
            this.BtnVoice.BackColor = System.Drawing.Color.Transparent;
            this.BtnVoice.BorderColor = System.Drawing.Color.Black;
            this.BtnVoice.BorderThickness = 0;
            this.BtnVoice.CornerRadius = 0;
            this.BtnVoice.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnVoice.DaskArray = null;
            this.BtnVoice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnVoice.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnVoice.ForeColor = System.Drawing.Color.Black;
            this.BtnVoice.Icon = null;
            this.BtnVoice.IconOffset = 1;
            this.BtnVoice.IconSize = new System.Drawing.Size(28, 28);
            this.BtnVoice.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnVoice.Location = new System.Drawing.Point(125, 79);
            this.BtnVoice.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.BtnVoice.MouseinBackColor = System.Drawing.Color.Transparent;
            this.BtnVoice.MouseinBorderColor = System.Drawing.Color.Blue;
            this.BtnVoice.MouseInBorderThickness = 0;
            this.BtnVoice.MouseinForeColor = System.Drawing.Color.Blue;
            this.BtnVoice.MouseinSvgForeColor = System.Drawing.Color.White;
            this.BtnVoice.Name = "BtnVoice";
            this.BtnVoice.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnVoice.PressedBorderColor = System.Drawing.Color.Blue;
            this.BtnVoice.PressedBorderThickness = 0;
            this.BtnVoice.PressedForeColor = System.Drawing.Color.Blue;
            this.BtnVoice.PressedSvgForeColor = System.Drawing.Color.White;
            this.BtnVoice.Size = new System.Drawing.Size(30, 31);
            this.BtnVoice.SVGForeColor = System.Drawing.Color.White;
            this.BtnVoice.SVGPath = global::ScopeX.U2.Properties.Resources.StatusToolStripVoiceOffSvg;
            this.BtnVoice.TabIndex = 0;
            this.BtnVoice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // StatusToolStrip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "StatusToolStrip";
            this.Size = new System.Drawing.Size(160, 115);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ScopeX.UserControls.ScopeXLabel timeLabel;
        private ScopeX.UserControls.ScopeXLabel dateLabel;
        private ScopeX.UserControls.ScopeXIconButton BtnNet;
        private ScopeX.UserControls.ScopeXIconButton BtnTransform;
        private ScopeX.UserControls.ScopeXIconButton BtnUsb;
        private ScopeX.UserControls.ScopeXIconButton BtnVoice;
    }
}
