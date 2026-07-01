
namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPageDbiLocalOscillators
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
            this.buttonCaliData_LoadDefualtValue_Channel = new System.Windows.Forms.Button();
            this.comboBoxCaliDataChannelSelectedChannel = new System.Windows.Forms.ComboBox();
            this.buttonCaliData_Channel_Send = new System.Windows.Forms.Button();
            this.checkBoxCaliData_ChannelAutoSend = new System.Windows.Forms.CheckBox();
            this.buttonCaliData_Channel_SaveToFile = new System.Windows.Forms.Button();
            this.buttonCaliData_Channel_LoadFromFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonCaliData_LoadDefualtValue_Channel);
            this.panel1.Controls.Add(this.comboBoxCaliDataChannelSelectedChannel);
            this.panel1.Controls.Add(this.buttonCaliData_Channel_Send);
            this.panel1.Controls.Add(this.checkBoxCaliData_ChannelAutoSend);
            this.panel1.Controls.Add(this.buttonCaliData_Channel_SaveToFile);
            this.panel1.Controls.Add(this.buttonCaliData_Channel_LoadFromFile);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(155, 362);
            this.panel1.TabIndex = 0;
            // 
            // buttonCaliData_LoadDefualtValue_Channel
            // 
            this.buttonCaliData_LoadDefualtValue_Channel.Location = new System.Drawing.Point(14, 190);
            this.buttonCaliData_LoadDefualtValue_Channel.Name = "buttonCaliData_LoadDefualtValue_Channel";
            this.buttonCaliData_LoadDefualtValue_Channel.Size = new System.Drawing.Size(126, 28);
            this.buttonCaliData_LoadDefualtValue_Channel.TabIndex = 19;
            this.buttonCaliData_LoadDefualtValue_Channel.Text = "装载缺省值";
            this.buttonCaliData_LoadDefualtValue_Channel.UseVisualStyleBackColor = true;
            this.buttonCaliData_LoadDefualtValue_Channel.Click += new System.EventHandler(this.buttonCaliData_LoadDefualtValue_Channel_Click);
            // 
            // comboBoxCaliDataChannelSelectedChannel
            // 
            this.comboBoxCaliDataChannelSelectedChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCaliDataChannelSelectedChannel.FormattingEnabled = true;
            this.comboBoxCaliDataChannelSelectedChannel.Items.AddRange(new object[] {
            "CH1",
            "CH2",
            "CH3",
            "CH4"});
            this.comboBoxCaliDataChannelSelectedChannel.Location = new System.Drawing.Point(75, 9);
            this.comboBoxCaliDataChannelSelectedChannel.Name = "comboBoxCaliDataChannelSelectedChannel";
            this.comboBoxCaliDataChannelSelectedChannel.Size = new System.Drawing.Size(65, 25);
            this.comboBoxCaliDataChannelSelectedChannel.TabIndex = 18;
            this.comboBoxCaliDataChannelSelectedChannel.SelectedIndexChanged += new System.EventHandler(this.comboBoxCaliDataChannelSelectedChannel_SelectedIndexChanged);
            // 
            // buttonCaliData_Channel_Send
            // 
            this.buttonCaliData_Channel_Send.Location = new System.Drawing.Point(14, 67);
            this.buttonCaliData_Channel_Send.Name = "buttonCaliData_Channel_Send";
            this.buttonCaliData_Channel_Send.Size = new System.Drawing.Size(126, 28);
            this.buttonCaliData_Channel_Send.TabIndex = 17;
            this.buttonCaliData_Channel_Send.Text = "生效";
            this.buttonCaliData_Channel_Send.UseVisualStyleBackColor = true;
            this.buttonCaliData_Channel_Send.Click += new System.EventHandler(this.buttonCaliData_Channel_Send_Click);
            // 
            // checkBoxCaliData_ChannelAutoSend
            // 
            this.checkBoxCaliData_ChannelAutoSend.AutoSize = true;
            this.checkBoxCaliData_ChannelAutoSend.Location = new System.Drawing.Point(14, 40);
            this.checkBoxCaliData_ChannelAutoSend.Name = "checkBoxCaliData_ChannelAutoSend";
            this.checkBoxCaliData_ChannelAutoSend.Size = new System.Drawing.Size(75, 21);
            this.checkBoxCaliData_ChannelAutoSend.TabIndex = 16;
            this.checkBoxCaliData_ChannelAutoSend.Text = "自动生效";
            this.checkBoxCaliData_ChannelAutoSend.UseVisualStyleBackColor = true;
            // 
            // buttonCaliData_Channel_SaveToFile
            // 
            this.buttonCaliData_Channel_SaveToFile.Location = new System.Drawing.Point(14, 149);
            this.buttonCaliData_Channel_SaveToFile.Name = "buttonCaliData_Channel_SaveToFile";
            this.buttonCaliData_Channel_SaveToFile.Size = new System.Drawing.Size(126, 28);
            this.buttonCaliData_Channel_SaveToFile.TabIndex = 15;
            this.buttonCaliData_Channel_SaveToFile.Text = "保存到文件";
            this.buttonCaliData_Channel_SaveToFile.UseVisualStyleBackColor = true;
            this.buttonCaliData_Channel_SaveToFile.Click += new System.EventHandler(this.buttonCaliData_Channel_SaveToFile_Click);
            // 
            // buttonCaliData_Channel_LoadFromFile
            // 
            this.buttonCaliData_Channel_LoadFromFile.Location = new System.Drawing.Point(14, 108);
            this.buttonCaliData_Channel_LoadFromFile.Name = "buttonCaliData_Channel_LoadFromFile";
            this.buttonCaliData_Channel_LoadFromFile.Size = new System.Drawing.Size(126, 28);
            this.buttonCaliData_Channel_LoadFromFile.TabIndex = 14;
            this.buttonCaliData_Channel_LoadFromFile.Text = "从文件装载";
            this.buttonCaliData_Channel_LoadFromFile.UseVisualStyleBackColor = true;
            this.buttonCaliData_Channel_LoadFromFile.Click += new System.EventHandler(this.buttonCaliData_Channel_LoadFromFile_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 13;
            this.label2.Text = "通道选择：";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(155, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(1017, 362);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "CmdIndexChar";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "控制字";
            this.Column2.Name = "Column2";
            // 
            // TabPageDbiLocalOscillators
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.Name = "TabPageDbiLocalOscillators";
            this.Size = new System.Drawing.Size(1172, 362);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonCaliData_LoadDefualtValue_Channel;
        private System.Windows.Forms.ComboBox comboBoxCaliDataChannelSelectedChannel;
        private System.Windows.Forms.Button buttonCaliData_Channel_Send;
        private System.Windows.Forms.CheckBox checkBoxCaliData_ChannelAutoSend;
        private System.Windows.Forms.Button buttonCaliData_Channel_SaveToFile;
        private System.Windows.Forms.Button buttonCaliData_Channel_LoadFromFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
    }
}
