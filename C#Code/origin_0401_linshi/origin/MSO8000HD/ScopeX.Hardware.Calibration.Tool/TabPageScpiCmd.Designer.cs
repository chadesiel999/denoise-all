
namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPageScpiCmd
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.richTextBoxReadBackResult = new System.Windows.Forms.RichTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonQuery = new System.Windows.Forms.Button();
            this.buttonClearReadbackResult = new System.Windows.Forms.Button();
            this.buttonRead = new System.Windows.Forms.Button();
            this.textBoxSCPICMD = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonSend = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBoxSelectResourceName = new System.Windows.Forms.TextBox();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxVisaResource = new System.Windows.Forms.ComboBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.richTextBoxReadBackMessage = new System.Windows.Forms.RichTextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.buttonLocalSCPIReadBack = new System.Windows.Forms.Button();
            this.buttonLocalSCPISend = new System.Windows.Forms.Button();
            this.comboBoxLocalScpiParam = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.richTextBoxLocalCmdDescription = new System.Windows.Forms.RichTextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.textBoxLocalCmdSourceCode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxDefinedCmdList = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(850, 369);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(842, 339);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.richTextBoxReadBackResult);
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(842, 339);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "测试";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // richTextBoxReadBackResult
            // 
            this.richTextBoxReadBackResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxReadBackResult.Location = new System.Drawing.Point(3, 165);
            this.richTextBoxReadBackResult.Name = "richTextBoxReadBackResult";
            this.richTextBoxReadBackResult.Size = new System.Drawing.Size(836, 171);
            this.richTextBoxReadBackResult.TabIndex = 9;
            this.richTextBoxReadBackResult.Text = "";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.buttonQuery);
            this.panel2.Controls.Add(this.buttonClearReadbackResult);
            this.panel2.Controls.Add(this.buttonRead);
            this.panel2.Controls.Add(this.textBoxSCPICMD);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.buttonSend);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 87);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(836, 78);
            this.panel2.TabIndex = 11;
            // 
            // buttonQuery
            // 
            this.buttonQuery.Location = new System.Drawing.Point(174, 43);
            this.buttonQuery.Name = "buttonQuery";
            this.buttonQuery.Size = new System.Drawing.Size(76, 29);
            this.buttonQuery.TabIndex = 11;
            this.buttonQuery.Text = "查询";
            this.buttonQuery.UseVisualStyleBackColor = true;
            this.buttonQuery.Click += new System.EventHandler(this.buttonQuery_Click);
            // 
            // buttonClearReadbackResult
            // 
            this.buttonClearReadbackResult.Location = new System.Drawing.Point(274, 43);
            this.buttonClearReadbackResult.Name = "buttonClearReadbackResult";
            this.buttonClearReadbackResult.Size = new System.Drawing.Size(148, 29);
            this.buttonClearReadbackResult.TabIndex = 10;
            this.buttonClearReadbackResult.Text = "清除所有读回结果";
            this.buttonClearReadbackResult.UseVisualStyleBackColor = true;
            this.buttonClearReadbackResult.Click += new System.EventHandler(this.buttonClearReadbackResult_Click);
            // 
            // buttonRead
            // 
            this.buttonRead.Location = new System.Drawing.Point(91, 43);
            this.buttonRead.Name = "buttonRead";
            this.buttonRead.Size = new System.Drawing.Size(76, 29);
            this.buttonRead.TabIndex = 9;
            this.buttonRead.Text = "读取";
            this.buttonRead.UseVisualStyleBackColor = true;
            this.buttonRead.Click += new System.EventHandler(this.buttonRead_Click);
            // 
            // textBoxSCPICMD
            // 
            this.textBoxSCPICMD.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxSCPICMD.Location = new System.Drawing.Point(0, 17);
            this.textBoxSCPICMD.Name = "textBoxSCPICMD";
            this.textBoxSCPICMD.Size = new System.Drawing.Size(836, 23);
            this.textBoxSCPICMD.TabIndex = 7;
            this.textBoxSCPICMD.Text = "*IDN?";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "SCPI命令：";
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(9, 42);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(76, 29);
            this.buttonSend.TabIndex = 8;
            this.buttonSend.Text = "发送";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBoxSelectResourceName);
            this.panel1.Controls.Add(this.buttonRefresh);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.comboBoxVisaResource);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(836, 84);
            this.panel1.TabIndex = 10;
            // 
            // textBoxSelectResourceName
            // 
            this.textBoxSelectResourceName.Location = new System.Drawing.Point(0, 50);
            this.textBoxSelectResourceName.Name = "textBoxSelectResourceName";
            this.textBoxSelectResourceName.ReadOnly = true;
            this.textBoxSelectResourceName.Size = new System.Drawing.Size(422, 23);
            this.textBoxSelectResourceName.TabIndex = 9;
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(428, 20);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(76, 29);
            this.buttonRefresh.TabIndex = 8;
            this.buttonRefresh.Text = "刷新";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "系统中现有Visa资源：";
            // 
            // comboBoxVisaResource
            // 
            this.comboBoxVisaResource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVisaResource.FormattingEnabled = true;
            this.comboBoxVisaResource.Location = new System.Drawing.Point(0, 20);
            this.comboBoxVisaResource.Name = "comboBoxVisaResource";
            this.comboBoxVisaResource.Size = new System.Drawing.Size(422, 25);
            this.comboBoxVisaResource.TabIndex = 6;
            this.comboBoxVisaResource.SelectedIndexChanged += new System.EventHandler(this.comboBoxVisaResource_SelectedIndexChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.richTextBoxReadBackMessage);
            this.tabPage3.Controls.Add(this.panel5);
            this.tabPage3.Controls.Add(this.panel4);
            this.tabPage3.Controls.Add(this.panel3);
            this.tabPage3.Location = new System.Drawing.Point(4, 26);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(842, 339);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "本仪器特殊控制";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // richTextBoxReadBackMessage
            // 
            this.richTextBoxReadBackMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxReadBackMessage.Location = new System.Drawing.Point(3, 175);
            this.richTextBoxReadBackMessage.Name = "richTextBoxReadBackMessage";
            this.richTextBoxReadBackMessage.Size = new System.Drawing.Size(836, 161);
            this.richTextBoxReadBackMessage.TabIndex = 8;
            this.richTextBoxReadBackMessage.Text = "";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.buttonLocalSCPIReadBack);
            this.panel5.Controls.Add(this.buttonLocalSCPISend);
            this.panel5.Controls.Add(this.comboBoxLocalScpiParam);
            this.panel5.Controls.Add(this.label4);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(3, 138);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(836, 37);
            this.panel5.TabIndex = 9;
            // 
            // buttonLocalSCPIReadBack
            // 
            this.buttonLocalSCPIReadBack.Location = new System.Drawing.Point(708, 6);
            this.buttonLocalSCPIReadBack.Name = "buttonLocalSCPIReadBack";
            this.buttonLocalSCPIReadBack.Size = new System.Drawing.Size(75, 23);
            this.buttonLocalSCPIReadBack.TabIndex = 11;
            this.buttonLocalSCPIReadBack.Text = "读取";
            this.buttonLocalSCPIReadBack.UseVisualStyleBackColor = true;
            this.buttonLocalSCPIReadBack.Click += new System.EventHandler(this.buttonLocalSCPIReadBack_Click);
            // 
            // buttonLocalSCPISend
            // 
            this.buttonLocalSCPISend.Location = new System.Drawing.Point(626, 5);
            this.buttonLocalSCPISend.Name = "buttonLocalSCPISend";
            this.buttonLocalSCPISend.Size = new System.Drawing.Size(75, 23);
            this.buttonLocalSCPISend.TabIndex = 10;
            this.buttonLocalSCPISend.Text = "发送";
            this.buttonLocalSCPISend.UseVisualStyleBackColor = true;
            this.buttonLocalSCPISend.Click += new System.EventHandler(this.buttonLocalSCPISend_Click);
            // 
            // comboBoxLocalScpiParam
            // 
            this.comboBoxLocalScpiParam.FormattingEnabled = true;
            this.comboBoxLocalScpiParam.Location = new System.Drawing.Point(42, 5);
            this.comboBoxLocalScpiParam.Name = "comboBoxLocalScpiParam";
            this.comboBoxLocalScpiParam.Size = new System.Drawing.Size(578, 25);
            this.comboBoxLocalScpiParam.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "参数：";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.richTextBoxLocalCmdDescription);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(3, 46);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(836, 92);
            this.panel4.TabIndex = 3;
            // 
            // richTextBoxLocalCmdDescription
            // 
            this.richTextBoxLocalCmdDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxLocalCmdDescription.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxLocalCmdDescription.Name = "richTextBoxLocalCmdDescription";
            this.richTextBoxLocalCmdDescription.ReadOnly = true;
            this.richTextBoxLocalCmdDescription.Size = new System.Drawing.Size(836, 92);
            this.richTextBoxLocalCmdDescription.TabIndex = 0;
            this.richTextBoxLocalCmdDescription.Text = "";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.textBoxLocalCmdSourceCode);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.comboBoxDefinedCmdList);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(836, 43);
            this.panel3.TabIndex = 2;
            // 
            // textBoxLocalCmdSourceCode
            // 
            this.textBoxLocalCmdSourceCode.Location = new System.Drawing.Point(388, 11);
            this.textBoxLocalCmdSourceCode.Name = "textBoxLocalCmdSourceCode";
            this.textBoxLocalCmdSourceCode.ReadOnly = true;
            this.textBoxLocalCmdSourceCode.Size = new System.Drawing.Size(395, 23);
            this.textBoxLocalCmdSourceCode.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 17);
            this.label3.TabIndex = 1;
            this.label3.Text = "控制：";
            // 
            // comboBoxDefinedCmdList
            // 
            this.comboBoxDefinedCmdList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDefinedCmdList.FormattingEnabled = true;
            this.comboBoxDefinedCmdList.Location = new System.Drawing.Point(55, 11);
            this.comboBoxDefinedCmdList.Name = "comboBoxDefinedCmdList";
            this.comboBoxDefinedCmdList.Size = new System.Drawing.Size(327, 25);
            this.comboBoxDefinedCmdList.TabIndex = 0;
            this.comboBoxDefinedCmdList.SelectedIndexChanged += new System.EventHandler(this.comboBoxDefinedCmdList_SelectedIndexChanged);
            // 
            // TabPageScpiCmd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "TabPageScpiCmd";
            this.Size = new System.Drawing.Size(850, 369);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBoxSCPICMD;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBoxReadBackResult;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBoxSelectResourceName;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxVisaResource;
        private System.Windows.Forms.Button buttonRead;
        private System.Windows.Forms.Button buttonClearReadbackResult;
        private System.Windows.Forms.Button buttonQuery;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxDefinedCmdList;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox textBoxLocalCmdSourceCode;
        private System.Windows.Forms.RichTextBox richTextBoxLocalCmdDescription;
        private System.Windows.Forms.RichTextBox richTextBoxReadBackMessage;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button buttonLocalSCPIReadBack;
        private System.Windows.Forms.Button buttonLocalSCPISend;
        private System.Windows.Forms.ComboBox comboBoxLocalScpiParam;
        private System.Windows.Forms.Label label4;
    }
}
