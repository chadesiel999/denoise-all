
namespace ScopeX.U2
{
    partial class StickyInfo
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
            LblName = new System.Windows.Forms.Label();
            USIFPanel = new ScopeX.UserControls.ScopeXStringIconFlowPanelEx();
            TlpBody = new System.Windows.Forms.TableLayoutPanel();
            LbIndicator = new System.Windows.Forms.Label();
            TlpBody.SuspendLayout();
            SuspendLayout();
            // 
            // LblName
            // 
            LblName.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            LblName.Dock = System.Windows.Forms.DockStyle.Fill;
            LblName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            LblName.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            LblName.Location = new System.Drawing.Point(0, 0);
            LblName.Margin = new System.Windows.Forms.Padding(0);
            LblName.Name = "LblName";
            LblName.Size = new System.Drawing.Size(30, 34);
            LblName.TabIndex = 5;
            LblName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            LblName.Paint += label1_Paint;
            // 
            // USIFPanel
            // 
            USIFPanel.ContentRowHeight = 20;
            USIFPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            USIFPanel.HighlightPrompt = defaultHighlightPrompt1;
            USIFPanel.IconWidth = 20;
            USIFPanel.Location = new System.Drawing.Point(30, 0);
            USIFPanel.Margin = new System.Windows.Forms.Padding(0);
            USIFPanel.Name = "USIFPanel";
            USIFPanel.PromptProperty = null;
            TlpBody.SetRowSpan(USIFPanel, 2);
            USIFPanel.Size = new System.Drawing.Size(118, 38);
            USIFPanel.TabIndex = 6;
            USIFPanel.Token = "";
            // 
            // TlpBody
            // 
            TlpBody.ColumnCount = 2;
            TlpBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            TlpBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpBody.Controls.Add(LblName, 0, 0);
            TlpBody.Controls.Add(USIFPanel, 1, 0);
            TlpBody.Controls.Add(LbIndicator, 0, 1);
            TlpBody.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpBody.Location = new System.Drawing.Point(1, 1);
            TlpBody.Name = "TlpBody";
            TlpBody.RowCount = 2;
            TlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 4F));
            TlpBody.Size = new System.Drawing.Size(148, 38);
            TlpBody.TabIndex = 7;
            // 
            // LbIndicator
            // 
            LbIndicator.AutoSize = true;
            LbIndicator.Dock = System.Windows.Forms.DockStyle.Fill;
            LbIndicator.Location = new System.Drawing.Point(9, 34);
            LbIndicator.Margin = new System.Windows.Forms.Padding(9, 0, 9, 2);
            LbIndicator.Name = "LbIndicator";
            LbIndicator.Size = new System.Drawing.Size(12, 2);
            LbIndicator.TabIndex = 7;
            // 
            // StickyInfo
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlDark;
            Controls.Add(TlpBody);
            Name = "StickyInfo";
            Padding = new System.Windows.Forms.Padding(1);
            Size = new System.Drawing.Size(150, 40);
            TlpBody.ResumeLayout(false);
            TlpBody.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label LblName;
        private ScopeX.UserControls.ScopeXStringIconFlowPanelEx USIFPanel;
        private System.Windows.Forms.TableLayoutPanel TlpBody;
        private System.Windows.Forms.Label LbIndicator;
    }
}
