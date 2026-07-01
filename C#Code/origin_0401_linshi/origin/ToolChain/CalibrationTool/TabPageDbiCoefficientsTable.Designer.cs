
namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPageDbiCoefficientsTable
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
            label5 = new System.Windows.Forms.Label();
            richTextBoxUsingCaliData = new System.Windows.Forms.RichTextBox();
            label4 = new System.Windows.Forms.Label();
            richTextBoxFileContent = new System.Windows.Forms.RichTextBox();
            panel1 = new System.Windows.Forms.Panel();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            panel2 = new System.Windows.Forms.Panel();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            buttonReSendAll = new System.Windows.Forms.Button();
            panelSubBandSelect = new System.Windows.Forms.Panel();
            comboBoxSubband = new System.Windows.Forms.ComboBox();
            label8 = new System.Windows.Forms.Label();
            panelChannelSelect = new System.Windows.Forms.Panel();
            comboBoxChannel = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            panelFilterbandModeSelect = new System.Windows.Forms.Panel();
            label6 = new System.Windows.Forms.Label();
            comboBoxFilterbandMode = new System.Windows.Forms.ComboBox();
            comboBoxBandMode = new System.Windows.Forms.ComboBox();
            label7 = new System.Windows.Forms.Label();
            buttonReadFile = new System.Windows.Forms.Button();
            comboBoxCaliType = new System.Windows.Forms.ComboBox();
            buttonSelectFile = new System.Windows.Forms.Button();
            textBox1 = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panelSubBandSelect.SuspendLayout();
            panelChannelSelect.SuspendLayout();
            panelFilterbandModeSelect.SuspendLayout();
            SuspendLayout();
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
            richTextBoxUsingCaliData.Size = new System.Drawing.Size(232, 595);
            richTextBoxUsingCaliData.TabIndex = 2;
            richTextBoxUsingCaliData.Text = "";
            richTextBoxUsingCaliData.VScroll += richTextBoxUsingCaliData_VScroll;
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
            richTextBoxFileContent.Size = new System.Drawing.Size(218, 595);
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
            panel1.Size = new System.Drawing.Size(218, 20);
            panel1.TabIndex = 0;
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
            splitContainer2.Size = new System.Drawing.Size(458, 615);
            splitContainer2.SplitterDistance = 218;
            splitContainer2.SplitterWidth = 8;
            splitContainer2.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Controls.Add(label5);
            panel2.Dock = System.Windows.Forms.DockStyle.Top;
            panel2.Location = new System.Drawing.Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(232, 20);
            panel2.TabIndex = 1;
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
            splitContainer1.Panel1.Controls.Add(buttonReSendAll);
            splitContainer1.Panel1.Controls.Add(panelSubBandSelect);
            splitContainer1.Panel1.Controls.Add(panelChannelSelect);
            splitContainer1.Panel1.Controls.Add(panelFilterbandModeSelect);
            splitContainer1.Panel1.Controls.Add(comboBoxBandMode);
            splitContainer1.Panel1.Controls.Add(label7);
            splitContainer1.Panel1.Controls.Add(buttonReadFile);
            splitContainer1.Panel1.Controls.Add(comboBoxCaliType);
            splitContainer1.Panel1.Controls.Add(buttonSelectFile);
            splitContainer1.Panel1.Controls.Add(textBox1);
            splitContainer1.Panel1.Controls.Add(label1);
            splitContainer1.Panel1.Controls.Add(label3);
            splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new System.Drawing.Size(962, 615);
            splitContainer1.SplitterDistance = 500;
            splitContainer1.TabIndex = 9;
            // 
            // buttonReSendAll
            // 
            buttonReSendAll.Location = new System.Drawing.Point(92, 197);
            buttonReSendAll.Name = "buttonReSendAll";
            buttonReSendAll.Size = new System.Drawing.Size(104, 26);
            buttonReSendAll.TabIndex = 27;
            buttonReSendAll.Text = "重发相关系数表";
            buttonReSendAll.UseVisualStyleBackColor = true;
            buttonReSendAll.Click += buttonReSendAll_Click;
            // 
            // panelSubBandSelect
            // 
            panelSubBandSelect.Controls.Add(comboBoxSubband);
            panelSubBandSelect.Controls.Add(label8);
            panelSubBandSelect.Location = new System.Drawing.Point(289, 68);
            panelSubBandSelect.Name = "panelSubBandSelect";
            panelSubBandSelect.Size = new System.Drawing.Size(152, 32);
            panelSubBandSelect.TabIndex = 26;
            // 
            // comboBoxSubband
            // 
            comboBoxSubband.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxSubband.FormattingEnabled = true;
            comboBoxSubband.Items.AddRange(new object[] { "子带1", "子带2", "子带3", "子带4" });
            comboBoxSubband.Location = new System.Drawing.Point(56, 2);
            comboBoxSubband.Name = "comboBoxSubband";
            comboBoxSubband.Size = new System.Drawing.Size(75, 25);
            comboBoxSubband.TabIndex = 1;
            comboBoxSubband.SelectedIndexChanged += comboBoxSubband_SelectedIndexChanged;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(10, 5);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(44, 17);
            label8.TabIndex = 3;
            label8.Text = "子带：";
            // 
            // panelChannelSelect
            // 
            panelChannelSelect.Controls.Add(comboBoxChannel);
            panelChannelSelect.Controls.Add(label2);
            panelChannelSelect.Location = new System.Drawing.Point(289, 35);
            panelChannelSelect.Name = "panelChannelSelect";
            panelChannelSelect.Size = new System.Drawing.Size(152, 32);
            panelChannelSelect.TabIndex = 25;
            // 
            // comboBoxChannel
            // 
            comboBoxChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxChannel.FormattingEnabled = true;
            comboBoxChannel.Location = new System.Drawing.Point(56, 2);
            comboBoxChannel.Name = "comboBoxChannel";
            comboBoxChannel.Size = new System.Drawing.Size(75, 25);
            comboBoxChannel.TabIndex = 1;
            comboBoxChannel.SelectedIndexChanged += comboBoxChannel_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(10, 5);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(44, 17);
            label2.TabIndex = 3;
            label2.Text = "通道：";
            // 
            // panelFilterbandModeSelect
            // 
            panelFilterbandModeSelect.Controls.Add(label6);
            panelFilterbandModeSelect.Controls.Add(comboBoxFilterbandMode);
            panelFilterbandModeSelect.Location = new System.Drawing.Point(-1, 66);
            panelFilterbandModeSelect.Name = "panelFilterbandModeSelect";
            panelFilterbandModeSelect.Size = new System.Drawing.Size(258, 31);
            panelFilterbandModeSelect.TabIndex = 24;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(-1, 5);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(104, 17);
            label6.TabIndex = 22;
            label6.Text = "滤波器通过模式：";
            // 
            // comboBoxFilterbandMode
            // 
            comboBoxFilterbandMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxFilterbandMode.FormattingEnabled = true;
            comboBoxFilterbandMode.Items.AddRange(new object[] { "低通", "带通" });
            comboBoxFilterbandMode.Location = new System.Drawing.Point(103, 3);
            comboBoxFilterbandMode.Name = "comboBoxFilterbandMode";
            comboBoxFilterbandMode.Size = new System.Drawing.Size(113, 25);
            comboBoxFilterbandMode.TabIndex = 23;
            comboBoxFilterbandMode.SelectedIndexChanged += comboBoxFilterbandMode_SelectedIndexChanged;
            // 
            // comboBoxBandMode
            // 
            comboBoxBandMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxBandMode.FormattingEnabled = true;
            comboBoxBandMode.Items.AddRange(new object[] { "20GHz", "其它" });
            comboBoxBandMode.Location = new System.Drawing.Point(102, 36);
            comboBoxBandMode.Name = "comboBoxBandMode";
            comboBoxBandMode.Size = new System.Drawing.Size(176, 25);
            comboBoxBandMode.TabIndex = 21;
            comboBoxBandMode.SelectedIndexChanged += comboBoxBandMode_SelectedIndexChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(11, 38);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(92, 17);
            label7.TabIndex = 20;
            label7.Text = "带宽独占模式：";
            // 
            // buttonReadFile
            // 
            buttonReadFile.Location = new System.Drawing.Point(92, 136);
            buttonReadFile.Name = "buttonReadFile";
            buttonReadFile.Size = new System.Drawing.Size(104, 26);
            buttonReadFile.TabIndex = 7;
            buttonReadFile.Text = "装载并生效";
            buttonReadFile.UseVisualStyleBackColor = true;
            buttonReadFile.Click += buttonReadFile_Click;
            // 
            // comboBoxCaliType
            // 
            comboBoxCaliType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxCaliType.FormattingEnabled = true;
            comboBoxCaliType.Items.AddRange(new object[] { "InterpolationCoefficients", "LocalOscillatorCoefficients", "AntiImageCoefficients", "FractionaryDelayCoefficients", "OverlapPhaseFreqDelayCoefficients", "TiAdc", "AmpFreqCoefficients", "PhaseFreqCoefficients", "MultiRadioInterpolationCoefficients" });
            comboBoxCaliType.Location = new System.Drawing.Point(101, 6);
            comboBoxCaliType.Name = "comboBoxCaliType";
            comboBoxCaliType.Size = new System.Drawing.Size(319, 25);
            comboBoxCaliType.TabIndex = 0;
            comboBoxCaliType.SelectedIndexChanged += comboBoxCaliType_SelectedIndexChanged;
            // 
            // buttonSelectFile
            // 
            buttonSelectFile.Location = new System.Drawing.Point(424, 103);
            buttonSelectFile.Name = "buttonSelectFile";
            buttonSelectFile.Size = new System.Drawing.Size(65, 25);
            buttonSelectFile.TabIndex = 6;
            buttonSelectFile.Text = "选取..";
            buttonSelectFile.UseVisualStyleBackColor = true;
            buttonSelectFile.Click += buttonSelectFile_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new System.Drawing.Point(102, 103);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new System.Drawing.Size(316, 23);
            textBox1.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(34, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(68, 17);
            label1.TabIndex = 2;
            label1.Text = "系数类型：";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(16, 104);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(92, 17);
            label3.TabIndex = 4;
            label3.Text = "系数文本文件：";
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "系数文本文件|*.txt";
            // 
            // TabPageDbiCoefficientsTable
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Name = "TabPageDbiCoefficientsTable";
            Size = new System.Drawing.Size(962, 615);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panelSubBandSelect.ResumeLayout(false);
            panelSubBandSelect.PerformLayout();
            panelChannelSelect.ResumeLayout(false);
            panelChannelSelect.PerformLayout();
            panelFilterbandModeSelect.ResumeLayout(false);
            panelFilterbandModeSelect.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox richTextBoxUsingCaliData;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox richTextBoxFileContent;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonReadFile;
        private System.Windows.Forms.ComboBox comboBoxCaliType;
        private System.Windows.Forms.Button buttonSelectFile;
        private System.Windows.Forms.ComboBox comboBoxChannel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ComboBox comboBoxBandMode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panelFilterbandModeSelect;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxFilterbandMode;
        private System.Windows.Forms.Panel panelChannelSelect;
        private System.Windows.Forms.Panel panelSubBandSelect;
        private System.Windows.Forms.ComboBox comboBoxSubband;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonReSendAll;
    }
}
