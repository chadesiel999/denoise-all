
namespace ScopeX.U2
{
    partial class VoltmeterInfo
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
            this.components = new System.ComponentModel.Container();
            this.LblTitle = new System.Windows.Forms.Label();
            this.LblSource = new System.Windows.Forms.Label();
            this.TlpDVM = new System.Windows.Forms.TableLayoutPanel();
            this.LblInfo = new System.Windows.Forms.Label();
            this.LblCoupling = new System.Windows.Forms.Label();
            this.TmUpdate = new System.Timers.Timer();
            this.TlpDVM.SuspendLayout();
            this.SuspendLayout();
            // 
            // LblTitle
            // 
            this.LblTitle.AutoSize = true;
            this.LblTitle.BackColor = System.Drawing.SystemColors.Control;
            this.LblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblTitle.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblTitle.Location = new System.Drawing.Point(0, 0);
            this.LblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.LblTitle.Name = "LblTitle";
            this.LblTitle.Size = new System.Drawing.Size(39, 28);
            this.LblTitle.TabIndex = 0;
            this.LblTitle.Text = "DVM";
            this.LblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblTitle.Click += new System.EventHandler(this.LblHeader_Click);
            // 
            // LblSource
            // 
            this.LblSource.BackColor = System.Drawing.SystemColors.Control;
            this.LblSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblSource.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblSource.Location = new System.Drawing.Point(39, 0);
            this.LblSource.Margin = new System.Windows.Forms.Padding(0);
            this.LblSource.Name = "LblSource";
            this.LblSource.Size = new System.Drawing.Size(81, 28);
            this.LblSource.TabIndex = 1;
            this.LblSource.Text = "C1";
            this.LblSource.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LblSource.Click += new System.EventHandler(this.LblHeader_Click);
            // 
            // TlpDVM
            // 
            this.TlpDVM.BackColor = System.Drawing.SystemColors.ControlDark;
            this.TlpDVM.ColumnCount = 2;
            this.TlpDVM.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.TlpDVM.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpDVM.Controls.Add(this.LblInfo, 0, 2);
            this.TlpDVM.Controls.Add(this.LblTitle, 0, 0);
            this.TlpDVM.Controls.Add(this.LblSource, 1, 0);
            this.TlpDVM.Controls.Add(this.LblCoupling, 1, 0);
            this.TlpDVM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpDVM.Location = new System.Drawing.Point(0, 0);
            this.TlpDVM.Name = "TlpDVM";
            this.TlpDVM.RowCount = 3;
            this.TlpDVM.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.TlpDVM.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.TlpDVM.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpDVM.Size = new System.Drawing.Size(120, 110);
            this.TlpDVM.TabIndex = 5;
            // 
            // LblInfo
            // 
            this.LblInfo.AutoSize = true;
            this.LblInfo.BackColor = System.Drawing.SystemColors.ControlDark;
            this.TlpDVM.SetColumnSpan(this.LblInfo, 2);
            this.LblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblInfo.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblInfo.Location = new System.Drawing.Point(0, 50);
            this.LblInfo.Margin = new System.Windows.Forms.Padding(0);
            this.LblInfo.Name = "LblInfo";
            this.LblInfo.Size = new System.Drawing.Size(120, 60);
            this.LblInfo.TabIndex = 2;
            this.LblInfo.Text = "1.00 V";
            this.LblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LblCoupling
            // 
            this.LblCoupling.AutoSize = true;
            this.TlpDVM.SetColumnSpan(this.LblCoupling, 2);
            this.LblCoupling.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblCoupling.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblCoupling.Location = new System.Drawing.Point(0, 28);
            this.LblCoupling.Margin = new System.Windows.Forms.Padding(0);
            this.LblCoupling.Name = "LblCoupling";
            this.LblCoupling.Size = new System.Drawing.Size(120, 22);
            this.LblCoupling.TabIndex = 3;
            this.LblCoupling.Text = "AC";
            this.LblCoupling.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TmUpdate
            // 
            this.TmUpdate.Enabled = true;
            this.TmUpdate.Interval = 1000;
            this.TmUpdate.Elapsed += TmUpdate_Tick;
            // 
            // DVMInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TlpDVM);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "DVMInfo";
            this.Size = new System.Drawing.Size(120, 110);
            this.Click += new System.EventHandler(this.DVMInfo_Click);
            this.TlpDVM.ResumeLayout(false);
            this.TlpDVM.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LblTitle;
        private System.Windows.Forms.Label LblSource;
        private System.Windows.Forms.TableLayoutPanel TlpDVM;
        private System.Windows.Forms.Label LblInfo;
        private System.Timers.Timer TmUpdate;
        private System.Windows.Forms.Label LblCoupling;
    }
}
