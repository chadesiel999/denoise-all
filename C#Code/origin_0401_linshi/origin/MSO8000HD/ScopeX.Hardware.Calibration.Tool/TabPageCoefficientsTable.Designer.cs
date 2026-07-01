
namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPageCoefficientsTable
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
            comboBoxCaliType = new System.Windows.Forms.ComboBox();
            comboBoxChannel = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            textBox1 = new System.Windows.Forms.TextBox();
            buttonSelectFile = new System.Windows.Forms.Button();
            buttonReadFile = new System.Windows.Forms.Button();
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            richTextBoxFileContent = new System.Windows.Forms.RichTextBox();
            panel1 = new System.Windows.Forms.Panel();
            label4 = new System.Windows.Forms.Label();
            richTextBoxUsingCaliData = new System.Windows.Forms.RichTextBox();
            panel2 = new System.Windows.Forms.Panel();
            label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // comboBoxCaliType
            // 
            comboBoxCaliType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxCaliType.FormattingEnabled = true;
            comboBoxCaliType.Items.AddRange(new object[] { "TiAdc", "Interpolation", "AFC" });
            comboBoxCaliType.Location = new System.Drawing.Point(93, 10);
            comboBoxCaliType.Name = "comboBoxCaliType";
            comboBoxCaliType.Size = new System.Drawing.Size(230, 25);
            comboBoxCaliType.TabIndex = 0;
            comboBoxCaliType.SelectedIndexChanged += OnSelectedItemChanged;
            // 
            // comboBoxChannel
            // 
            comboBoxChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxChannel.FormattingEnabled = true;
            comboBoxChannel.Location = new System.Drawing.Point(379, 10);
            comboBoxChannel.Name = "comboBoxChannel";
            comboBoxChannel.Size = new System.Drawing.Size(65, 25);
            comboBoxChannel.TabIndex = 1;
            comboBoxChannel.SelectedIndexChanged += OnSelectedItemChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(19, 13);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(68, 17);
            label1.TabIndex = 2;
            label1.Text = "系数类型：";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(329, 13);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(44, 17);
            label2.TabIndex = 3;
            label2.Text = "通道：";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(8, 55);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(92, 17);
            label3.TabIndex = 4;
            label3.Text = "系数文本文件：";
            // 
            // textBox1
            // 
            textBox1.Location = new System.Drawing.Point(93, 55);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new System.Drawing.Size(307, 23);
            textBox1.TabIndex = 5;
            // 
            // buttonSelectFile
            // 
            buttonSelectFile.Location = new System.Drawing.Point(416, 54);
            buttonSelectFile.Name = "buttonSelectFile";
            buttonSelectFile.Size = new System.Drawing.Size(75, 25);
            buttonSelectFile.TabIndex = 6;
            buttonSelectFile.Text = "选取..";
            buttonSelectFile.UseVisualStyleBackColor = true;
            buttonSelectFile.Click += buttonSelectFile_Click;
            // 
            // buttonReadFile
            // 
            buttonReadFile.Location = new System.Drawing.Point(84, 93);
            buttonReadFile.Name = "buttonReadFile";
            buttonReadFile.Size = new System.Drawing.Size(104, 26);
            buttonReadFile.TabIndex = 7;
            buttonReadFile.Text = "装载并生效";
            buttonReadFile.UseVisualStyleBackColor = true;
            buttonReadFile.Click += buttonReadFile_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "系数文本文件|*.txt";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(buttonReadFile);
            splitContainer1.Panel1.Controls.Add(comboBoxCaliType);
            splitContainer1.Panel1.Controls.Add(buttonSelectFile);
            splitContainer1.Panel1.Controls.Add(comboBoxChannel);
            splitContainer1.Panel1.Controls.Add(textBox1);
            splitContainer1.Panel1.Controls.Add(label1);
            splitContainer1.Panel1.Controls.Add(label3);
            splitContainer1.Panel1.Controls.Add(label2);
            splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new System.Drawing.Size(924, 318);
            splitContainer1.SplitterDistance = 500;
            splitContainer1.TabIndex = 8;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.Location = new System.Drawing.Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(richTextBoxFileContent);
            splitContainer2.Panel1.Controls.Add(panel1);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(richTextBoxUsingCaliData);
            splitContainer2.Panel2.Controls.Add(panel2);
            splitContainer2.Size = new System.Drawing.Size(420, 318);
            splitContainer2.SplitterDistance = 200;
            splitContainer2.SplitterWidth = 8;
            splitContainer2.TabIndex = 0;
            // 
            // richTextBoxFileContent
            // 
            richTextBoxFileContent.BackColor = System.Drawing.Color.White;
            richTextBoxFileContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBoxFileContent.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBoxFileContent.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            richTextBoxFileContent.ForeColor = System.Drawing.Color.Black;
            richTextBoxFileContent.Location = new System.Drawing.Point(0, 20);
            richTextBoxFileContent.Name = "richTextBoxFileContent";
            richTextBoxFileContent.ReadOnly = true;
            richTextBoxFileContent.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            richTextBoxFileContent.Size = new System.Drawing.Size(200, 298);
            richTextBoxFileContent.TabIndex = 1;
            richTextBoxFileContent.Text = "";
            richTextBoxFileContent.VScroll += richTextBoxFileContent_VScroll;
            // 
            // panel1
            // 
            panel1.Controls.Add(label4);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(200, 20);
            panel1.TabIndex = 0;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = System.Windows.Forms.DockStyle.Top;
            label4.Location = new System.Drawing.Point(0, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(104, 17);
            label4.TabIndex = 0;
            label4.Text = "当前文件的数据：";
            // 
            // richTextBoxUsingCaliData
            // 
            richTextBoxUsingCaliData.BackColor = System.Drawing.Color.Black;
            richTextBoxUsingCaliData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBoxUsingCaliData.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBoxUsingCaliData.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            richTextBoxUsingCaliData.ForeColor = System.Drawing.Color.White;
            richTextBoxUsingCaliData.Location = new System.Drawing.Point(0, 20);
            richTextBoxUsingCaliData.Name = "richTextBoxUsingCaliData";
            richTextBoxUsingCaliData.ReadOnly = true;
            richTextBoxUsingCaliData.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            richTextBoxUsingCaliData.Size = new System.Drawing.Size(212, 298);
            richTextBoxUsingCaliData.TabIndex = 2;
            richTextBoxUsingCaliData.Text = "";
            richTextBoxUsingCaliData.VScroll += richTextBoxUsingCaliData_VScroll;
            // 
            // panel2
            // 
            panel2.Controls.Add(label5);
            panel2.Dock = System.Windows.Forms.DockStyle.Top;
            panel2.Location = new System.Drawing.Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(212, 20);
            panel2.TabIndex = 1;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = System.Windows.Forms.DockStyle.Top;
            label5.Location = new System.Drawing.Point(0, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(104, 17);
            label5.TabIndex = 0;
            label5.Text = "在用的校准数据：";
            // 
            // TabPageCoefficientsTable
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Name = "TabPageCoefficientsTable";
            Size = new System.Drawing.Size(924, 318);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxCaliType;
        private System.Windows.Forms.ComboBox comboBoxChannel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonSelectFile;
        private System.Windows.Forms.Button buttonReadFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox richTextBoxFileContent;
        private System.Windows.Forms.RichTextBox richTextBoxUsingCaliData;
    }
}
