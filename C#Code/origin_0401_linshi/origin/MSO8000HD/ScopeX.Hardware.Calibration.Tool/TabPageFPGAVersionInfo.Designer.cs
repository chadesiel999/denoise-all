
namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPageFPGAVersionInfo
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonReadBack = new System.Windows.Forms.Button();
            this.richTextBoxFPGA_ContentVersionInfo = new System.Windows.Forms.RichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.richTextBoxFPGAVersionInfoAtFlash = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonReadBack);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 255);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(580, 38);
            this.panel1.TabIndex = 0;
            // 
            // buttonReadBack
            // 
            this.buttonReadBack.Location = new System.Drawing.Point(290, 6);
            this.buttonReadBack.Name = "buttonReadBack";
            this.buttonReadBack.Size = new System.Drawing.Size(70, 27);
            this.buttonReadBack.TabIndex = 0;
            this.buttonReadBack.Text = "读取";
            this.buttonReadBack.UseVisualStyleBackColor = true;
            this.buttonReadBack.Click += new System.EventHandler(this.buttonReadBack_Click);
            // 
            // richTextBoxFPGA_ContentVersionInfo
            // 
            this.richTextBoxFPGA_ContentVersionInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxFPGA_ContentVersionInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxFPGA_ContentVersionInfo.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxFPGA_ContentVersionInfo.Name = "richTextBoxFPGA_ContentVersionInfo";
            this.richTextBoxFPGA_ContentVersionInfo.ReadOnly = true;
            this.richTextBoxFPGA_ContentVersionInfo.Size = new System.Drawing.Size(580, 127);
            this.richTextBoxFPGA_ContentVersionInfo.TabIndex = 1;
            this.richTextBoxFPGA_ContentVersionInfo.Text = "";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.richTextBoxFPGA_ContentVersionInfo);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.richTextBoxFPGAVersionInfoAtFlash);
            this.splitContainer1.Size = new System.Drawing.Size(580, 255);
            this.splitContainer1.SplitterDistance = 127;
            this.splitContainer1.TabIndex = 2;
            // 
            // richTextBoxFPGAVersionInfoAtFlash
            // 
            this.richTextBoxFPGAVersionInfoAtFlash.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxFPGAVersionInfoAtFlash.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxFPGAVersionInfoAtFlash.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxFPGAVersionInfoAtFlash.Name = "richTextBoxFPGAVersionInfoAtFlash";
            this.richTextBoxFPGAVersionInfoAtFlash.ReadOnly = true;
            this.richTextBoxFPGAVersionInfoAtFlash.Size = new System.Drawing.Size(580, 124);
            this.richTextBoxFPGAVersionInfoAtFlash.TabIndex = 4;
            this.richTextBoxFPGAVersionInfoAtFlash.Text = "";
            // 
            // TabPageFPGAVersionInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "TabPageFPGAVersionInfo";
            this.Size = new System.Drawing.Size(580, 293);
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonReadBack;
        private System.Windows.Forms.RichTextBox richTextBoxFPGA_ContentVersionInfo;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox richTextBoxFPGAVersionInfoAtFlash;
    }
}
