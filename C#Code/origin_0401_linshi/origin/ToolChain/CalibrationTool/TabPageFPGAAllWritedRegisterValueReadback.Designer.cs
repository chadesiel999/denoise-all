
namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPage_FPGAAllWritedRegisterValueReadback
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
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            richTextBox1 = new System.Windows.Forms.RichTextBox();
            panel2 = new System.Windows.Forms.Panel();
            buttonSave = new System.Windows.Forms.Button();
            comboBoxWriteReadSelect = new System.Windows.Forms.ComboBox();
            comboBoxBoardSelect = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            groupBox1 = new System.Windows.Forms.GroupBox();
            panel4 = new System.Windows.Forms.Panel();
            button1 = new System.Windows.Forms.Button();
            richTextBoxMultiRegister = new System.Windows.Forms.RichTextBox();
            label7 = new System.Windows.Forms.Label();
            buttonWriteRegister = new System.Windows.Forms.Button();
            checkBoxWriteRegister_IsAcq = new System.Windows.Forms.CheckBox();
            textBoxWriteRegister_Value = new System.Windows.Forms.TextBox();
            textBoxWriteRegister_Addr = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            buttonReadBack = new System.Windows.Forms.Button();
            tabPage2 = new System.Windows.Forms.TabPage();
            richTextBoxNotSample = new System.Windows.Forms.RichTextBox();
            panel3 = new System.Windows.Forms.Panel();
            buttonCompare = new System.Windows.Forms.Button();
            comboBoxCompareWith = new System.Windows.Forms.ComboBox();
            label6 = new System.Windows.Forms.Label();
            panelHistoryB = new System.Windows.Forms.Panel();
            buttonOpenB = new System.Windows.Forms.Button();
            textBoxHistoryB_FileName = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            buttonOpenA = new System.Windows.Forms.Button();
            textBoxHistoryA_FileName = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            panel4.SuspendLayout();
            tabPage2.SuspendLayout();
            panel3.SuspendLayout();
            panelHistoryB.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(937, 304);
            tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(splitContainer1);
            tabPage1.Location = new System.Drawing.Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(929, 274);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "读取";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(3, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(richTextBox1);
            splitContainer1.Panel1.Controls.Add(panel2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panel1);
            splitContainer1.Size = new System.Drawing.Size(923, 268);
            splitContainer1.SplitterDistance = 134;
            splitContainer1.SplitterWidth = 8;
            splitContainer1.TabIndex = 9;
            // 
            // richTextBox1
            // 
            richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBox1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            richTextBox1.Location = new System.Drawing.Point(0, 30);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new System.Drawing.Size(923, 104);
            richTextBox1.TabIndex = 5;
            richTextBox1.Text = "";
            richTextBox1.WordWrap = false;
            // 
            // panel2
            // 
            panel2.Controls.Add(buttonSave);
            panel2.Controls.Add(comboBoxWriteReadSelect);
            panel2.Controls.Add(comboBoxBoardSelect);
            panel2.Controls.Add(label3);
            panel2.Dock = System.Windows.Forms.DockStyle.Top;
            panel2.Location = new System.Drawing.Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(923, 30);
            panel2.TabIndex = 6;
            // 
            // buttonSave
            // 
            buttonSave.Location = new System.Drawing.Point(497, 3);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new System.Drawing.Size(75, 24);
            buttonSave.TabIndex = 3;
            buttonSave.Text = "保存";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // comboBoxWriteReadSelect
            // 
            comboBoxWriteReadSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxWriteReadSelect.FormattingEnabled = true;
            comboBoxWriteReadSelect.Items.AddRange(new object[] { "写和读", "写", "读" });
            comboBoxWriteReadSelect.Location = new System.Drawing.Point(247, 4);
            comboBoxWriteReadSelect.Name = "comboBoxWriteReadSelect";
            comboBoxWriteReadSelect.Size = new System.Drawing.Size(101, 25);
            comboBoxWriteReadSelect.TabIndex = 2;
            comboBoxWriteReadSelect.SelectedIndexChanged += OnFilterChanged;
            // 
            // comboBoxBoardSelect
            // 
            comboBoxBoardSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxBoardSelect.FormattingEnabled = true;
            comboBoxBoardSelect.Items.AddRange(new object[] { "All", "Pcie Board", "S6 Board", "Proc Board", "AcqBoard1", "AcqBoard2", "AcqBoard3", "AcqBoard4", "AcqBoard5", "AcqBoard6", "AcqBoard7", "AcqBoard8" });
            comboBoxBoardSelect.Location = new System.Drawing.Point(88, 4);
            comboBoxBoardSelect.Name = "comboBoxBoardSelect";
            comboBoxBoardSelect.Size = new System.Drawing.Size(121, 25);
            comboBoxBoardSelect.TabIndex = 1;
            comboBoxBoardSelect.SelectedIndexChanged += OnFilterChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(14, 8);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(68, 17);
            label3.TabIndex = 0;
            label3.Text = "显示范围：";
            // 
            // panel1
            // 
            panel1.Controls.Add(groupBox1);
            panel1.Controls.Add(buttonReadBack);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(923, 126);
            panel1.TabIndex = 3;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(panel4);
            groupBox1.Controls.Add(buttonWriteRegister);
            groupBox1.Controls.Add(checkBoxWriteRegister_IsAcq);
            groupBox1.Controls.Add(textBoxWriteRegister_Value);
            groupBox1.Controls.Add(textBoxWriteRegister_Addr);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
            groupBox1.Location = new System.Drawing.Point(163, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(760, 126);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "测试用-寄存器写";
            // 
            // panel4
            // 
            panel4.Controls.Add(button1);
            panel4.Controls.Add(richTextBoxMultiRegister);
            panel4.Controls.Add(label7);
            panel4.Dock = System.Windows.Forms.DockStyle.Left;
            panel4.Location = new System.Drawing.Point(3, 19);
            panel4.Name = "panel4";
            panel4.Size = new System.Drawing.Size(390, 104);
            panel4.TabIndex = 9;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(243, 32);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(68, 50);
            button1.TabIndex = 8;
            button1.Text = "写多个寄存器";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // richTextBoxMultiRegister
            // 
            richTextBoxMultiRegister.Dock = System.Windows.Forms.DockStyle.Left;
            richTextBoxMultiRegister.Location = new System.Drawing.Point(0, 17);
            richTextBoxMultiRegister.Name = "richTextBoxMultiRegister";
            richTextBoxMultiRegister.Size = new System.Drawing.Size(216, 87);
            richTextBoxMultiRegister.TabIndex = 6;
            richTextBoxMultiRegister.Text = "0x7800,0x00,0\n0x7800,0x01,0\n0x8899,0x20,0";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Dock = System.Windows.Forms.DockStyle.Top;
            label7.Location = new System.Drawing.Point(0, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(387, 17);
            label7.TabIndex = 7;
            label7.Text = "格式为：寄存器地址(16进制),值（16进制）,之后延迟时间(毫秒,10进制)";
            // 
            // buttonWriteRegister
            // 
            buttonWriteRegister.Location = new System.Drawing.Point(676, 35);
            buttonWriteRegister.Name = "buttonWriteRegister";
            buttonWriteRegister.Size = new System.Drawing.Size(67, 50);
            buttonWriteRegister.TabIndex = 5;
            buttonWriteRegister.Text = "写单个寄存器";
            buttonWriteRegister.UseVisualStyleBackColor = true;
            buttonWriteRegister.Click += buttonWriteRegister_Click;
            // 
            // checkBoxWriteRegister_IsAcq
            // 
            checkBoxWriteRegister_IsAcq.AutoSize = true;
            checkBoxWriteRegister_IsAcq.Location = new System.Drawing.Point(585, 80);
            checkBoxWriteRegister_IsAcq.Name = "checkBoxWriteRegister_IsAcq";
            checkBoxWriteRegister_IsAcq.Size = new System.Drawing.Size(75, 21);
            checkBoxWriteRegister_IsAcq.TabIndex = 4;
            checkBoxWriteRegister_IsAcq.Text = "是采集板";
            checkBoxWriteRegister_IsAcq.UseVisualStyleBackColor = true;
            // 
            // textBoxWriteRegister_Value
            // 
            textBoxWriteRegister_Value.Location = new System.Drawing.Point(585, 51);
            textBoxWriteRegister_Value.Name = "textBoxWriteRegister_Value";
            textBoxWriteRegister_Value.Size = new System.Drawing.Size(69, 23);
            textBoxWriteRegister_Value.TabIndex = 3;
            // 
            // textBoxWriteRegister_Addr
            // 
            textBoxWriteRegister_Addr.Location = new System.Drawing.Point(585, 17);
            textBoxWriteRegister_Addr.Name = "textBoxWriteRegister_Addr";
            textBoxWriteRegister_Addr.Size = new System.Drawing.Size(69, 23);
            textBoxWriteRegister_Addr.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(499, 54);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(89, 17);
            label2.TabIndex = 1;
            label2.Text = "寄存器值(0x)：";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(484, 19);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(101, 17);
            label1.TabIndex = 0;
            label1.Text = "寄存器地址(0x)：";
            // 
            // buttonReadBack
            // 
            buttonReadBack.Location = new System.Drawing.Point(14, 6);
            buttonReadBack.Name = "buttonReadBack";
            buttonReadBack.Size = new System.Drawing.Size(100, 39);
            buttonReadBack.TabIndex = 0;
            buttonReadBack.Text = "读取";
            buttonReadBack.UseVisualStyleBackColor = true;
            buttonReadBack.Click += buttonReadBack_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(richTextBoxNotSample);
            tabPage2.Controls.Add(panel3);
            tabPage2.Location = new System.Drawing.Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(3);
            tabPage2.Size = new System.Drawing.Size(929, 274);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "比较";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // richTextBoxNotSample
            // 
            richTextBoxNotSample.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBoxNotSample.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBoxNotSample.Location = new System.Drawing.Point(3, 74);
            richTextBoxNotSample.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            richTextBoxNotSample.Name = "richTextBoxNotSample";
            richTextBoxNotSample.ReadOnly = true;
            richTextBoxNotSample.Size = new System.Drawing.Size(923, 197);
            richTextBoxNotSample.TabIndex = 1;
            richTextBoxNotSample.Text = "";
            richTextBoxNotSample.WordWrap = false;
            // 
            // panel3
            // 
            panel3.Controls.Add(buttonCompare);
            panel3.Controls.Add(comboBoxCompareWith);
            panel3.Controls.Add(label6);
            panel3.Controls.Add(panelHistoryB);
            panel3.Controls.Add(buttonOpenA);
            panel3.Controls.Add(textBoxHistoryA_FileName);
            panel3.Controls.Add(label4);
            panel3.Dock = System.Windows.Forms.DockStyle.Top;
            panel3.Location = new System.Drawing.Point(3, 3);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(923, 71);
            panel3.TabIndex = 0;
            // 
            // buttonCompare
            // 
            buttonCompare.Location = new System.Drawing.Point(690, 8);
            buttonCompare.Name = "buttonCompare";
            buttonCompare.Size = new System.Drawing.Size(77, 46);
            buttonCompare.TabIndex = 8;
            buttonCompare.Text = "比较";
            buttonCompare.UseVisualStyleBackColor = true;
            buttonCompare.Click += buttonCompare_Click;
            // 
            // comboBoxCompareWith
            // 
            comboBoxCompareWith.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxCompareWith.FormattingEnabled = true;
            comboBoxCompareWith.Items.AddRange(new object[] { "当前读回记录", "另一个历史记录" });
            comboBoxCompareWith.Location = new System.Drawing.Point(80, 33);
            comboBoxCompareWith.Name = "comboBoxCompareWith";
            comboBoxCompareWith.Size = new System.Drawing.Size(114, 25);
            comboBoxCompareWith.TabIndex = 6;
            comboBoxCompareWith.SelectedIndexChanged += comboBoxCompareWith_SelectedIndexChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(16, 36);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(68, 17);
            label6.TabIndex = 7;
            label6.Text = "与谁比较：";
            // 
            // panelHistoryB
            // 
            panelHistoryB.Controls.Add(buttonOpenB);
            panelHistoryB.Controls.Add(textBoxHistoryB_FileName);
            panelHistoryB.Controls.Add(label5);
            panelHistoryB.Location = new System.Drawing.Point(231, 32);
            panelHistoryB.Name = "panelHistoryB";
            panelHistoryB.Size = new System.Drawing.Size(420, 30);
            panelHistoryB.TabIndex = 5;
            // 
            // buttonOpenB
            // 
            buttonOpenB.Location = new System.Drawing.Point(337, 4);
            buttonOpenB.Name = "buttonOpenB";
            buttonOpenB.Size = new System.Drawing.Size(77, 25);
            buttonOpenB.TabIndex = 5;
            buttonOpenB.Text = "浏览...";
            buttonOpenB.UseVisualStyleBackColor = true;
            buttonOpenB.Click += buttonOpenB_Click;
            // 
            // textBoxHistoryB_FileName
            // 
            textBoxHistoryB_FileName.Location = new System.Drawing.Point(77, 4);
            textBoxHistoryB_FileName.Name = "textBoxHistoryB_FileName";
            textBoxHistoryB_FileName.ReadOnly = true;
            textBoxHistoryB_FileName.Size = new System.Drawing.Size(253, 23);
            textBoxHistoryB_FileName.TabIndex = 4;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(6, 6);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(76, 17);
            label5.TabIndex = 3;
            label5.Text = "历史记录B：";
            // 
            // buttonOpenA
            // 
            buttonOpenA.Location = new System.Drawing.Point(340, 5);
            buttonOpenA.Name = "buttonOpenA";
            buttonOpenA.Size = new System.Drawing.Size(77, 25);
            buttonOpenA.TabIndex = 2;
            buttonOpenA.Text = "浏览...";
            buttonOpenA.UseVisualStyleBackColor = true;
            buttonOpenA.Click += buttonOpenA_Click;
            // 
            // textBoxHistoryA_FileName
            // 
            textBoxHistoryA_FileName.Location = new System.Drawing.Point(80, 5);
            textBoxHistoryA_FileName.Name = "textBoxHistoryA_FileName";
            textBoxHistoryA_FileName.ReadOnly = true;
            textBoxHistoryA_FileName.Size = new System.Drawing.Size(253, 23);
            textBoxHistoryA_FileName.TabIndex = 1;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(9, 7);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(76, 17);
            label4.TabIndex = 0;
            label4.Text = "历史记录A：";
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // TabPage_FPGAAllWritedRegisterValueReadback
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(tabControl1);
            Name = "TabPage_FPGAAllWritedRegisterValueReadback";
            Size = new System.Drawing.Size(937, 304);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            tabPage2.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panelHistoryB.ResumeLayout(false);
            panelHistoryB.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonOpenA;
        private System.Windows.Forms.TextBox textBoxHistoryA_FileName;
        private System.Windows.Forms.Panel panelHistoryB;
        private System.Windows.Forms.Button buttonOpenB;
        private System.Windows.Forms.TextBox textBoxHistoryB_FileName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxCompareWith;
        private System.Windows.Forms.Button buttonCompare;
        private System.Windows.Forms.RichTextBox richTextBoxNotSample;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.ComboBox comboBoxWriteReadSelect;
        private System.Windows.Forms.ComboBox comboBoxBoardSelect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RichTextBox richTextBoxMultiRegister;
        private System.Windows.Forms.Button buttonWriteRegister;
        private System.Windows.Forms.CheckBox checkBoxWriteRegister_IsAcq;
        private System.Windows.Forms.TextBox textBoxWriteRegister_Value;
        private System.Windows.Forms.TextBox textBoxWriteRegister_Addr;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonReadBack;
        private System.Windows.Forms.Panel panel4;
    }
}
