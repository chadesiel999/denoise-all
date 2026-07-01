
namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPageMatlabSourceCode
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
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.richTextBoxMatlabSourceCode_SourceCode = new System.Windows.Forms.RichTextBox();
            this.panel9 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxMatlabSourceCode_PrePosProcessType = new System.Windows.Forms.ComboBox();
            this.buttonMatlabSourceCode_LoadFromFile = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.buttonMatlabSourceCode_RunStopStatus = new System.Windows.Forms.Button();
            this.buttonMatlabSourceCode_SaveToFile = new System.Windows.Forms.Button();
            this.richTextBoxMatlabSourceCode_Result = new System.Windows.Forms.RichTextBox();
            this.panelFigure = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.checkBoxAutoStop = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.BackColor = System.Drawing.Color.LightGray;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            this.splitContainer2.Panel1MinSize = 50;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panelFigure);
            this.splitContainer2.Panel2MinSize = 50;
            this.splitContainer2.Size = new System.Drawing.Size(599, 402);
            this.splitContainer2.SplitterDistance = 500;
            this.splitContainer2.TabIndex = 1;
            // 
            // splitContainer3
            // 
            this.splitContainer3.BackColor = System.Drawing.Color.LightGray;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.richTextBoxMatlabSourceCode_SourceCode);
            this.splitContainer3.Panel1.Controls.Add(this.panel9);
            this.splitContainer3.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.richTextBoxMatlabSourceCode_Result);
            this.splitContainer3.Size = new System.Drawing.Size(500, 402);
            this.splitContainer3.SplitterDistance = 245;
            this.splitContainer3.TabIndex = 0;
            // 
            // richTextBoxMatlabSourceCode_SourceCode
            // 
            this.richTextBoxMatlabSourceCode_SourceCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxMatlabSourceCode_SourceCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxMatlabSourceCode_SourceCode.Location = new System.Drawing.Point(0, 32);
            this.richTextBoxMatlabSourceCode_SourceCode.Name = "richTextBoxMatlabSourceCode_SourceCode";
            this.richTextBoxMatlabSourceCode_SourceCode.Size = new System.Drawing.Size(500, 176);
            this.richTextBoxMatlabSourceCode_SourceCode.TabIndex = 1;
            this.richTextBoxMatlabSourceCode_SourceCode.Text = "";
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.label8);
            this.panel9.Controls.Add(this.comboBoxMatlabSourceCode_PrePosProcessType);
            this.panel9.Controls.Add(this.buttonMatlabSourceCode_LoadFromFile);
            this.panel9.Controls.Add(this.label7);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(500, 32);
            this.panel9.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(143, 6);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 17);
            this.label8.TabIndex = 4;
            this.label8.Text = "前后处理类型";
            // 
            // comboBoxMatlabSourceCode_PrePosProcessType
            // 
            this.comboBoxMatlabSourceCode_PrePosProcessType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMatlabSourceCode_PrePosProcessType.FormattingEnabled = true;
            this.comboBoxMatlabSourceCode_PrePosProcessType.Location = new System.Drawing.Point(229, 3);
            this.comboBoxMatlabSourceCode_PrePosProcessType.Name = "comboBoxMatlabSourceCode_PrePosProcessType";
            this.comboBoxMatlabSourceCode_PrePosProcessType.Size = new System.Drawing.Size(272, 25);
            this.comboBoxMatlabSourceCode_PrePosProcessType.TabIndex = 3;
            this.comboBoxMatlabSourceCode_PrePosProcessType.SelectedIndexChanged += new System.EventHandler(this.comboBoxMatlabSourceCode_PrePosProcessType_SelectedIndexChanged);
            // 
            // buttonMatlabSourceCode_LoadFromFile
            // 
            this.buttonMatlabSourceCode_LoadFromFile.Location = new System.Drawing.Point(87, 3);
            this.buttonMatlabSourceCode_LoadFromFile.Name = "buttonMatlabSourceCode_LoadFromFile";
            this.buttonMatlabSourceCode_LoadFromFile.Size = new System.Drawing.Size(50, 23);
            this.buttonMatlabSourceCode_LoadFromFile.TabIndex = 1;
            this.buttonMatlabSourceCode_LoadFromFile.Text = "装载";
            this.buttonMatlabSourceCode_LoadFromFile.UseVisualStyleBackColor = true;
            this.buttonMatlabSourceCode_LoadFromFile.Click += new System.EventHandler(this.buttonMatlabSourceCode_LoadFromFile_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 17);
            this.label7.TabIndex = 0;
            this.label7.Text = "Matlab源代码";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBoxAutoStop);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.numericUpDown1);
            this.panel1.Controls.Add(this.buttonMatlabSourceCode_RunStopStatus);
            this.panel1.Controls.Add(this.buttonMatlabSourceCode_SaveToFile);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 208);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(500, 37);
            this.panel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(138, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "刷新频率(mS):";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(225, 8);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(67, 23);
            this.numericUpDown1.TabIndex = 3;
            this.numericUpDown1.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // buttonMatlabSourceCode_RunStopStatus
            // 
            this.buttonMatlabSourceCode_RunStopStatus.Location = new System.Drawing.Point(55, 2);
            this.buttonMatlabSourceCode_RunStopStatus.Name = "buttonMatlabSourceCode_RunStopStatus";
            this.buttonMatlabSourceCode_RunStopStatus.Size = new System.Drawing.Size(78, 31);
            this.buttonMatlabSourceCode_RunStopStatus.TabIndex = 2;
            this.buttonMatlabSourceCode_RunStopStatus.Tag = "stop";
            this.buttonMatlabSourceCode_RunStopStatus.Text = "Stopped";
            this.buttonMatlabSourceCode_RunStopStatus.UseVisualStyleBackColor = true;
            this.buttonMatlabSourceCode_RunStopStatus.Click += new System.EventHandler(this.buttonMatlabSourceCode_RunStopStatus_Click);
            // 
            // buttonMatlabSourceCode_SaveToFile
            // 
            this.buttonMatlabSourceCode_SaveToFile.Location = new System.Drawing.Point(3, 6);
            this.buttonMatlabSourceCode_SaveToFile.Name = "buttonMatlabSourceCode_SaveToFile";
            this.buttonMatlabSourceCode_SaveToFile.Size = new System.Drawing.Size(50, 23);
            this.buttonMatlabSourceCode_SaveToFile.TabIndex = 2;
            this.buttonMatlabSourceCode_SaveToFile.Text = "保存";
            this.buttonMatlabSourceCode_SaveToFile.UseVisualStyleBackColor = true;
            this.buttonMatlabSourceCode_SaveToFile.Click += new System.EventHandler(this.buttonMatlabSourceCode_SaveToFile_Click);
            // 
            // richTextBoxMatlabSourceCode_Result
            // 
            this.richTextBoxMatlabSourceCode_Result.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxMatlabSourceCode_Result.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxMatlabSourceCode_Result.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxMatlabSourceCode_Result.Name = "richTextBoxMatlabSourceCode_Result";
            this.richTextBoxMatlabSourceCode_Result.Size = new System.Drawing.Size(500, 153);
            this.richTextBoxMatlabSourceCode_Result.TabIndex = 2;
            this.richTextBoxMatlabSourceCode_Result.Text = "";
            // 
            // panelFigure
            // 
            this.panelFigure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFigure.Location = new System.Drawing.Point(0, 0);
            this.panelFigure.Name = "panelFigure";
            this.panelFigure.Size = new System.Drawing.Size(95, 402);
            this.panelFigure.TabIndex = 0;
            this.panelFigure.SizeChanged += new System.EventHandler(this.panelFigure_SizeChanged);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Matla源代码文件|*.m";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Matla源代码文件|*.m";
            // 
            // checkBoxAutoStop
            // 
            this.checkBoxAutoStop.AutoSize = true;
            this.checkBoxAutoStop.Location = new System.Drawing.Point(310, 9);
            this.checkBoxAutoStop.Name = "checkBoxAutoStop";
            this.checkBoxAutoStop.Size = new System.Drawing.Size(147, 21);
            this.checkBoxAutoStop.TabIndex = 5;
            this.checkBoxAutoStop.Text = "执行完该次后自动停止";
            this.checkBoxAutoStop.UseVisualStyleBackColor = true;
            // 
            // TabPageMatlabSourceCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer2);
            this.Name = "TabPageMatlabSourceCode";
            this.Size = new System.Drawing.Size(599, 402);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.RichTextBox richTextBoxMatlabSourceCode_SourceCode;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBoxMatlabSourceCode_PrePosProcessType;
        private System.Windows.Forms.Button buttonMatlabSourceCode_SaveToFile;
        private System.Windows.Forms.Button buttonMatlabSourceCode_LoadFromFile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panelFigure;
        private System.Windows.Forms.RichTextBox richTextBoxMatlabSourceCode_Result;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonMatlabSourceCode_RunStopStatus;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.CheckBox checkBoxAutoStop;
    }
}
