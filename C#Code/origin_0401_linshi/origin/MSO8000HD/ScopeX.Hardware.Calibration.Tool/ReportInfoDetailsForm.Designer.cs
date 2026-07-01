namespace ScopeX.Hardware.Calibration.Tool
{
    partial class ReportInfoDetailsForm
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.ChNum = new System.Windows.Forms.ColumnHeader();
            this.ChFunc = new System.Windows.Forms.ColumnHeader();
            this.ChStepInfo = new System.Windows.Forms.ColumnHeader();
            this.ChRst = new System.Windows.Forms.ColumnHeader();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.CbOnlyFailed = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ChNum,
            this.ChFunc,
            this.ChStepInfo,
            this.ChRst});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(805, 329);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // ChNum
            // 
            this.ChNum.Text = "序号";
            // 
            // ChFunc
            // 
            this.ChFunc.Text = "执行方法名";
            this.ChFunc.Width = 300;
            // 
            // ChStepInfo
            // 
            this.ChStepInfo.Text = "档位信息";
            this.ChStepInfo.Width = 300;
            // 
            // ChRst
            // 
            this.ChRst.Text = "结果";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 490);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(805, 26);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(171, 21);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(0, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 21);
            this.label1.TabIndex = 2;
            this.label1.Text = "详细信息：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 373);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(805, 117);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // CbOnlyFailed
            // 
            this.CbOnlyFailed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CbOnlyFailed.AutoSize = true;
            this.CbOnlyFailed.Location = new System.Drawing.Point(711, 6);
            this.CbOnlyFailed.Name = "CbOnlyFailed";
            this.CbOnlyFailed.Size = new System.Drawing.Size(87, 21);
            this.CbOnlyFailed.TabIndex = 5;
            this.CbOnlyFailed.Text = "只显示失败";
            this.CbOnlyFailed.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.CbOnlyFailed);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 329);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(805, 44);
            this.panel1.TabIndex = 6;
            // 
            // ReportInfoDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 516);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.listView1);
            this.Name = "ReportInfoDetailsForm";
            this.Text = "ReportInfoDetailsForm";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader ChNum;
        private System.Windows.Forms.ColumnHeader ChFunc;
        private System.Windows.Forms.ColumnHeader ChStepInfo;
        private System.Windows.Forms.ColumnHeader ChRst;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.CheckBox CbOnlyFailed;
        private System.Windows.Forms.Panel panel1;
    }
}