
namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPageCaliData_AcqSync
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            panel7 = new System.Windows.Forms.Panel();
            richTextBoxAdc5200SyncWindows = new System.Windows.Forms.RichTextBox();
            dataGridViewCaliData_AcqSync = new System.Windows.Forms.DataGridView();
            panel8 = new System.Windows.Forms.Panel();
            buttonRead5200AdcWindow = new System.Windows.Forms.Button();
            comboBoxCaliDataChannelSelectedChannel = new System.Windows.Forms.ComboBox();
            labelTitle = new System.Windows.Forms.Label();
            buttonCaliData_LoadDefualtValue_AcqSync = new System.Windows.Forms.Button();
            buttonCaliData_AcqSync_Send = new System.Windows.Forms.Button();
            checkBoxCaliData_AcqSync_AutoSend = new System.Windows.Forms.CheckBox();
            buttonCaliData_AcqSyncLoadFromFile = new System.Windows.Forms.Button();
            buttonCaliData_AcqSyncSaveToFile = new System.Windows.Forms.Button();
            Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            SampleClock10G = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewCaliData_AcqSync).BeginInit();
            panel8.SuspendLayout();
            SuspendLayout();
            // 
            // panel7
            // 
            panel7.Controls.Add(richTextBoxAdc5200SyncWindows);
            panel7.Controls.Add(dataGridViewCaliData_AcqSync);
            panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            panel7.Location = new System.Drawing.Point(0, 0);
            panel7.Name = "panel7";
            panel7.Size = new System.Drawing.Size(1074, 379);
            panel7.TabIndex = 6;
            // 
            // richTextBoxAdc5200SyncWindows
            // 
            richTextBoxAdc5200SyncWindows.Dock = System.Windows.Forms.DockStyle.Left;
            richTextBoxAdc5200SyncWindows.Location = new System.Drawing.Point(607, 0);
            richTextBoxAdc5200SyncWindows.Name = "richTextBoxAdc5200SyncWindows";
            richTextBoxAdc5200SyncWindows.ReadOnly = true;
            richTextBoxAdc5200SyncWindows.Size = new System.Drawing.Size(475, 379);
            richTextBoxAdc5200SyncWindows.TabIndex = 1;
            richTextBoxAdc5200SyncWindows.Text = "";
            // 
            // dataGridViewCaliData_AcqSync
            // 
            dataGridViewCaliData_AcqSync.AllowUserToAddRows = false;
            dataGridViewCaliData_AcqSync.AllowUserToDeleteRows = false;
            dataGridViewCaliData_AcqSync.AllowUserToResizeRows = false;
            dataGridViewCaliData_AcqSync.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dataGridViewCaliData_AcqSync.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCaliData_AcqSync.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCaliData_AcqSync.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Column11, SampleClock10G, Column12, Column13, Column14, Column15, Column16 });
            dataGridViewCaliData_AcqSync.Dock = System.Windows.Forms.DockStyle.Left;
            dataGridViewCaliData_AcqSync.EnableHeadersVisualStyles = false;
            dataGridViewCaliData_AcqSync.Location = new System.Drawing.Point(0, 0);
            dataGridViewCaliData_AcqSync.MultiSelect = false;
            dataGridViewCaliData_AcqSync.Name = "dataGridViewCaliData_AcqSync";
            dataGridViewCaliData_AcqSync.RowHeadersVisible = false;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCaliData_AcqSync.RowsDefaultCellStyle = dataGridViewCellStyle2;
            dataGridViewCaliData_AcqSync.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCaliData_AcqSync.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCaliData_AcqSync.RowTemplate.Height = 25;
            dataGridViewCaliData_AcqSync.Size = new System.Drawing.Size(607, 379);
            dataGridViewCaliData_AcqSync.TabIndex = 0;
            dataGridViewCaliData_AcqSync.CellEndEdit += onCaliData_AcqSync_NeedRefreshGrid;
            // 
            // panel8
            // 
            panel8.Controls.Add(buttonRead5200AdcWindow);
            panel8.Controls.Add(comboBoxCaliDataChannelSelectedChannel);
            panel8.Controls.Add(labelTitle);
            panel8.Controls.Add(buttonCaliData_LoadDefualtValue_AcqSync);
            panel8.Controls.Add(buttonCaliData_AcqSync_Send);
            panel8.Controls.Add(checkBoxCaliData_AcqSync_AutoSend);
            panel8.Controls.Add(buttonCaliData_AcqSyncLoadFromFile);
            panel8.Controls.Add(buttonCaliData_AcqSyncSaveToFile);
            panel8.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel8.Location = new System.Drawing.Point(0, 379);
            panel8.Name = "panel8";
            panel8.Size = new System.Drawing.Size(1074, 48);
            panel8.TabIndex = 5;
            // 
            // buttonRead5200AdcWindow
            // 
            buttonRead5200AdcWindow.Location = new System.Drawing.Point(730, 12);
            buttonRead5200AdcWindow.Name = "buttonRead5200AdcWindow";
            buttonRead5200AdcWindow.Size = new System.Drawing.Size(176, 28);
            buttonRead5200AdcWindow.TabIndex = 16;
            buttonRead5200AdcWindow.Text = "读取5200ADC的同步窗口串";
            buttonRead5200AdcWindow.UseVisualStyleBackColor = true;
            buttonRead5200AdcWindow.Click += buttonRead5200AdcWindow_Click;
            // 
            // comboBoxCaliDataChannelSelectedChannel
            // 
            comboBoxCaliDataChannelSelectedChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxCaliDataChannelSelectedChannel.FormattingEnabled = true;
            comboBoxCaliDataChannelSelectedChannel.Items.AddRange(new object[] { "采集板1", "采集板2", "采集板3", "采集板4", "采集板5", "采集板6", "采集板7", "采集板8" });
            comboBoxCaliDataChannelSelectedChannel.Location = new System.Drawing.Point(80, 14);
            comboBoxCaliDataChannelSelectedChannel.Name = "comboBoxCaliDataChannelSelectedChannel";
            comboBoxCaliDataChannelSelectedChannel.Size = new System.Drawing.Size(78, 25);
            comboBoxCaliDataChannelSelectedChannel.TabIndex = 15;
            comboBoxCaliDataChannelSelectedChannel.SelectedIndexChanged += comboBoxCaliDataChannelSelectedChannel_SelectedIndexChanged;
            // 
            // labelTitle
            // 
            labelTitle.AutoSize = true;
            labelTitle.Location = new System.Drawing.Point(8, 18);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new System.Drawing.Size(80, 17);
            labelTitle.TabIndex = 14;
            labelTitle.Text = "采集板选择：";
            // 
            // buttonCaliData_LoadDefualtValue_AcqSync
            // 
            buttonCaliData_LoadDefualtValue_AcqSync.Location = new System.Drawing.Point(614, 13);
            buttonCaliData_LoadDefualtValue_AcqSync.Name = "buttonCaliData_LoadDefualtValue_AcqSync";
            buttonCaliData_LoadDefualtValue_AcqSync.Size = new System.Drawing.Size(85, 28);
            buttonCaliData_LoadDefualtValue_AcqSync.TabIndex = 13;
            buttonCaliData_LoadDefualtValue_AcqSync.Text = "装载缺省值";
            buttonCaliData_LoadDefualtValue_AcqSync.UseVisualStyleBackColor = true;
            buttonCaliData_LoadDefualtValue_AcqSync.Click += buttonCaliData_LoadDefualtValue_AcqSync_Click;
            // 
            // buttonCaliData_AcqSync_Send
            // 
            buttonCaliData_AcqSync_Send.Location = new System.Drawing.Point(295, 12);
            buttonCaliData_AcqSync_Send.Name = "buttonCaliData_AcqSync_Send";
            buttonCaliData_AcqSync_Send.Size = new System.Drawing.Size(85, 28);
            buttonCaliData_AcqSync_Send.TabIndex = 11;
            buttonCaliData_AcqSync_Send.Text = "生效";
            buttonCaliData_AcqSync_Send.UseVisualStyleBackColor = true;
            buttonCaliData_AcqSync_Send.Click += buttonCaliData_AcqSync_Send_Click;
            // 
            // checkBoxCaliData_AcqSync_AutoSend
            // 
            checkBoxCaliData_AcqSync_AutoSend.AutoSize = true;
            checkBoxCaliData_AcqSync_AutoSend.Location = new System.Drawing.Point(207, 18);
            checkBoxCaliData_AcqSync_AutoSend.Name = "checkBoxCaliData_AcqSync_AutoSend";
            checkBoxCaliData_AcqSync_AutoSend.Size = new System.Drawing.Size(75, 21);
            checkBoxCaliData_AcqSync_AutoSend.TabIndex = 10;
            checkBoxCaliData_AcqSync_AutoSend.Text = "自动生效";
            checkBoxCaliData_AcqSync_AutoSend.UseVisualStyleBackColor = true;
            // 
            // buttonCaliData_AcqSyncLoadFromFile
            // 
            buttonCaliData_AcqSyncLoadFromFile.Location = new System.Drawing.Point(404, 12);
            buttonCaliData_AcqSyncLoadFromFile.Name = "buttonCaliData_AcqSyncLoadFromFile";
            buttonCaliData_AcqSyncLoadFromFile.Size = new System.Drawing.Size(85, 28);
            buttonCaliData_AcqSyncLoadFromFile.TabIndex = 2;
            buttonCaliData_AcqSyncLoadFromFile.Text = "从文件装载";
            buttonCaliData_AcqSyncLoadFromFile.UseVisualStyleBackColor = true;
            buttonCaliData_AcqSyncLoadFromFile.Click += buttonCaliData_AcqSyncLoadFromFile_Click;
            // 
            // buttonCaliData_AcqSyncSaveToFile
            // 
            buttonCaliData_AcqSyncSaveToFile.Location = new System.Drawing.Point(511, 12);
            buttonCaliData_AcqSyncSaveToFile.Name = "buttonCaliData_AcqSyncSaveToFile";
            buttonCaliData_AcqSyncSaveToFile.Size = new System.Drawing.Size(85, 28);
            buttonCaliData_AcqSyncSaveToFile.TabIndex = 3;
            buttonCaliData_AcqSyncSaveToFile.Text = "保存到文件";
            buttonCaliData_AcqSyncSaveToFile.UseVisualStyleBackColor = true;
            buttonCaliData_AcqSyncSaveToFile.Click += buttonCaliData_AcqSyncSaveToFile_Click;
            // 
            // Column11
            // 
            Column11.HeaderText = "Adc序号";
            Column11.Name = "Column11";
            Column11.ReadOnly = true;
            Column11.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SampleClock10G
            // 
            SampleClock10G.HeaderText = "SampleClock10G";
            SampleClock10G.Name = "SampleClock10G";
            SampleClock10G.Width = 150;
            // 
            // Column12
            // 
            Column12.HeaderText = "SampleClock20G";
            Column12.Name = "Column12";
            Column12.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Column12.Width = 150;
            // 
            // Column13
            // 
            Column13.HeaderText = "SyncReset";
            Column13.Name = "Column13";
            Column13.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column14
            // 
            Column14.HeaderText = "RM";
            Column14.Name = "Column14";
            Column14.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column15
            // 
            Column15.HeaderText = "Serdes";
            Column15.Name = "Column15";
            Column15.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column16
            // 
            Column16.HeaderText = "WriteEnable";
            Column16.Name = "Column16";
            Column16.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // TabPageCaliData_AcqSync
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panel7);
            Controls.Add(panel8);
            Name = "TabPageCaliData_AcqSync";
            Size = new System.Drawing.Size(1074, 427);
            panel7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewCaliData_AcqSync).EndInit();
            panel8.ResumeLayout(false);
            panel8.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.DataGridView dataGridViewCaliData_AcqSync;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Button buttonCaliData_LoadDefualtValue_AcqSync;
        private System.Windows.Forms.Button buttonCaliData_AcqSync_Send;
        private System.Windows.Forms.CheckBox checkBoxCaliData_AcqSync_AutoSend;
        private System.Windows.Forms.Button buttonCaliData_AcqSyncLoadFromFile;
        private System.Windows.Forms.Button buttonCaliData_AcqSyncSaveToFile;
        private System.Windows.Forms.ComboBox comboBoxCaliDataChannelSelectedChannel;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.RichTextBox richTextBoxAdc5200SyncWindows;
        private System.Windows.Forms.Button buttonRead5200AdcWindow;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn SampleClock10G;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column16;
    }
}
