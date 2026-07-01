namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPageDbiDiscardDots
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            TlpMain = new System.Windows.Forms.TableLayoutPanel();
            DgvSubband = new System.Windows.Forms.DataGridView();
            TxbSubband = new System.Windows.Forms.DataGridViewTextBoxColumn();
            TxbDiscardBefore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            TxbDiscardAfter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            BtnCalcDiscardDots = new System.Windows.Forms.DataGridViewButtonColumn();
            TxbInitPhase = new System.Windows.Forms.DataGridViewTextBoxColumn();
            BtnCalcInitPhase = new System.Windows.Forms.DataGridViewButtonColumn();
            TxbSignalFreq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            TxbDiifPhase = new System.Windows.Forms.DataGridViewTextBoxColumn();
            TxbLocalFeq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            BtnCalcLocalCoe = new System.Windows.Forms.DataGridViewButtonColumn();
            PlControl = new System.Windows.Forms.Panel();
            BtnInitPhaseDiffFile = new System.Windows.Forms.Button();
            BtnSavePhaseDiff = new System.Windows.Forms.Button();
            TbxCoeLength = new System.Windows.Forms.TextBox();
            LblCoeLength = new System.Windows.Forms.Label();
            BtnSlectSourceAddress = new System.Windows.Forms.Button();
            BtnLoadFromFile = new System.Windows.Forms.Button();
            BtnSaveToFile = new System.Windows.Forms.Button();
            BtnLoadCfg = new System.Windows.Forms.Button();
            BtnSaveCfg = new System.Windows.Forms.Button();
            LblSubBandCnt = new System.Windows.Forms.Label();
            NudSubbandCnt = new System.Windows.Forms.NumericUpDown();
            TxbParallelRoads = new System.Windows.Forms.TextBox();
            LblParallelRoads = new System.Windows.Forms.Label();
            TxbSampleFreq = new System.Windows.Forms.TextBox();
            LblSampleFreq = new System.Windows.Forms.Label();
            BtnRefreshSingleAddress = new System.Windows.Forms.Button();
            CbxVisaResource = new System.Windows.Forms.ComboBox();
            LblSourceAddress = new System.Windows.Forms.Label();
            CbxChnlSelect = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            BtnSend = new System.Windows.Forms.Button();
            ChkAutoSend = new System.Windows.Forms.CheckBox();
            RtbInfo = new System.Windows.Forms.RichTextBox();
            TlpMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DgvSubband).BeginInit();
            PlControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NudSubbandCnt).BeginInit();
            SuspendLayout();
            // 
            // TlpMain
            // 
            TlpMain.ColumnCount = 1;
            TlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpMain.Controls.Add(DgvSubband, 0, 0);
            TlpMain.Controls.Add(PlControl, 0, 2);
            TlpMain.Controls.Add(RtbInfo, 0, 1);
            TlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpMain.Location = new System.Drawing.Point(0, 0);
            TlpMain.Name = "TlpMain";
            TlpMain.RowCount = 3;
            TlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            TlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            TlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            TlpMain.Size = new System.Drawing.Size(1592, 596);
            TlpMain.TabIndex = 0;
            // 
            // DgvSubband
            // 
            DgvSubband.AllowUserToAddRows = false;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            DgvSubband.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            DgvSubband.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvSubband.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { TxbSubband, TxbDiscardBefore, TxbDiscardAfter, BtnCalcDiscardDots, TxbInitPhase, BtnCalcInitPhase, TxbSignalFreq, TxbDiifPhase, TxbLocalFeq, BtnCalcLocalCoe });
            DgvSubband.Dock = System.Windows.Forms.DockStyle.Fill;
            DgvSubband.Location = new System.Drawing.Point(3, 3);
            DgvSubband.Name = "DgvSubband";
            DgvSubband.RowTemplate.Height = 25;
            DgvSubband.Size = new System.Drawing.Size(1586, 192);
            DgvSubband.TabIndex = 0;
            DgvSubband.CellContentClick += DgvSubband_CellContentClick;
            DgvSubband.CellEndEdit += DgvSubband_CellEndEdit;
            // 
            // TxbSubband
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            TxbSubband.DefaultCellStyle = dataGridViewCellStyle6;
            TxbSubband.HeaderText = "子带";
            TxbSubband.Name = "TxbSubband";
            TxbSubband.ReadOnly = true;
            TxbSubband.Width = 60;
            // 
            // TxbDiscardBefore
            // 
            TxbDiscardBefore.HeaderText = "混频前丢点数";
            TxbDiscardBefore.Name = "TxbDiscardBefore";
            TxbDiscardBefore.Width = 120;
            // 
            // TxbDiscardAfter
            // 
            TxbDiscardAfter.HeaderText = "混频后丢点数";
            TxbDiscardAfter.Name = "TxbDiscardAfter";
            TxbDiscardAfter.Width = 120;
            // 
            // BtnCalcDiscardDots
            // 
            BtnCalcDiscardDots.HeaderText = "计算丢点并下发";
            BtnCalcDiscardDots.Name = "BtnCalcDiscardDots";
            BtnCalcDiscardDots.Width = 120;
            // 
            // TxbInitPhase
            // 
            TxbInitPhase.HeaderText = "初相";
            TxbInitPhase.Name = "TxbInitPhase";
            // 
            // BtnCalcInitPhase
            // 
            BtnCalcInitPhase.HeaderText = "仅计算初相";
            BtnCalcInitPhase.Name = "BtnCalcInitPhase";
            BtnCalcInitPhase.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            BtnCalcInitPhase.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // TxbSignalFreq
            // 
            TxbSignalFreq.HeaderText = "信号频点(MHz)";
            TxbSignalFreq.Name = "TxbSignalFreq";
            TxbSignalFreq.Width = 200;
            // 
            // TxbDiifPhase
            // 
            TxbDiifPhase.HeaderText = "当前子带与上一子带的相位差";
            TxbDiifPhase.Name = "TxbDiifPhase";
            TxbDiifPhase.Width = 200;
            // 
            // TxbLocalFeq
            // 
            TxbLocalFeq.HeaderText = "本振频率(GHz)";
            TxbLocalFeq.Name = "TxbLocalFeq";
            TxbLocalFeq.Width = 120;
            // 
            // BtnCalcLocalCoe
            // 
            BtnCalcLocalCoe.HeaderText = "生成本振系数并下发";
            BtnCalcLocalCoe.Name = "BtnCalcLocalCoe";
            BtnCalcLocalCoe.Width = 150;
            // 
            // PlControl
            // 
            PlControl.Controls.Add(BtnInitPhaseDiffFile);
            PlControl.Controls.Add(BtnSavePhaseDiff);
            PlControl.Controls.Add(TbxCoeLength);
            PlControl.Controls.Add(LblCoeLength);
            PlControl.Controls.Add(BtnSlectSourceAddress);
            PlControl.Controls.Add(BtnLoadFromFile);
            PlControl.Controls.Add(BtnSaveToFile);
            PlControl.Controls.Add(BtnLoadCfg);
            PlControl.Controls.Add(BtnSaveCfg);
            PlControl.Controls.Add(LblSubBandCnt);
            PlControl.Controls.Add(NudSubbandCnt);
            PlControl.Controls.Add(TxbParallelRoads);
            PlControl.Controls.Add(LblParallelRoads);
            PlControl.Controls.Add(TxbSampleFreq);
            PlControl.Controls.Add(LblSampleFreq);
            PlControl.Controls.Add(BtnRefreshSingleAddress);
            PlControl.Controls.Add(CbxVisaResource);
            PlControl.Controls.Add(LblSourceAddress);
            PlControl.Controls.Add(CbxChnlSelect);
            PlControl.Controls.Add(label2);
            PlControl.Controls.Add(BtnSend);
            PlControl.Controls.Add(ChkAutoSend);
            PlControl.Dock = System.Windows.Forms.DockStyle.Fill;
            PlControl.Location = new System.Drawing.Point(3, 498);
            PlControl.Name = "PlControl";
            PlControl.Size = new System.Drawing.Size(1586, 95);
            PlControl.TabIndex = 1;
            // 
            // BtnInitPhaseDiffFile
            // 
            BtnInitPhaseDiffFile.Location = new System.Drawing.Point(1404, 12);
            BtnInitPhaseDiffFile.Name = "BtnInitPhaseDiffFile";
            BtnInitPhaseDiffFile.Size = new System.Drawing.Size(112, 28);
            BtnInitPhaseDiffFile.TabIndex = 41;
            BtnInitPhaseDiffFile.Text = "初始化相位差文件";
            BtnInitPhaseDiffFile.UseVisualStyleBackColor = true;
            BtnInitPhaseDiffFile.Click += BtnInitPhaseDiffFile_Click;
            // 
            // BtnSavePhaseDiff
            // 
            BtnSavePhaseDiff.Location = new System.Drawing.Point(1404, 45);
            BtnSavePhaseDiff.Name = "BtnSavePhaseDiff";
            BtnSavePhaseDiff.Size = new System.Drawing.Size(112, 28);
            BtnSavePhaseDiff.TabIndex = 40;
            BtnSavePhaseDiff.Text = "扫频并保存相位差";
            BtnSavePhaseDiff.UseVisualStyleBackColor = true;
            BtnSavePhaseDiff.Click += BtnSavePhaseDiff_Click;
            // 
            // TbxCoeLength
            // 
            TbxCoeLength.Location = new System.Drawing.Point(1071, 50);
            TbxCoeLength.Name = "TbxCoeLength";
            TbxCoeLength.Size = new System.Drawing.Size(56, 23);
            TbxCoeLength.TabIndex = 39;
            // 
            // LblCoeLength
            // 
            LblCoeLength.AutoSize = true;
            LblCoeLength.Location = new System.Drawing.Point(1071, 18);
            LblCoeLength.Name = "LblCoeLength";
            LblCoeLength.Size = new System.Drawing.Size(56, 17);
            LblCoeLength.TabIndex = 38;
            LblCoeLength.Text = "系数长度";
            // 
            // BtnSlectSourceAddress
            // 
            BtnSlectSourceAddress.Location = new System.Drawing.Point(654, 12);
            BtnSlectSourceAddress.Name = "BtnSlectSourceAddress";
            BtnSlectSourceAddress.Size = new System.Drawing.Size(70, 28);
            BtnSlectSourceAddress.TabIndex = 37;
            BtnSlectSourceAddress.Text = "选择";
            BtnSlectSourceAddress.UseVisualStyleBackColor = true;
            BtnSlectSourceAddress.Click += BtnSlectSourceAddress_Click;
            // 
            // BtnLoadFromFile
            // 
            BtnLoadFromFile.Location = new System.Drawing.Point(131, 48);
            BtnLoadFromFile.Name = "BtnLoadFromFile";
            BtnLoadFromFile.Size = new System.Drawing.Size(85, 28);
            BtnLoadFromFile.TabIndex = 29;
            BtnLoadFromFile.Text = "从文件装载";
            BtnLoadFromFile.UseVisualStyleBackColor = true;
            BtnLoadFromFile.Click += BtnLoadFromFile_Click;
            // 
            // BtnSaveToFile
            // 
            BtnSaveToFile.Location = new System.Drawing.Point(265, 48);
            BtnSaveToFile.Name = "BtnSaveToFile";
            BtnSaveToFile.Size = new System.Drawing.Size(85, 28);
            BtnSaveToFile.TabIndex = 30;
            BtnSaveToFile.Text = "保存到文件";
            BtnSaveToFile.UseVisualStyleBackColor = true;
            BtnSaveToFile.Click += BtnSaveToFile_Click;
            // 
            // BtnLoadCfg
            // 
            BtnLoadCfg.Location = new System.Drawing.Point(1163, 12);
            BtnLoadCfg.Name = "BtnLoadCfg";
            BtnLoadCfg.Size = new System.Drawing.Size(85, 28);
            BtnLoadCfg.TabIndex = 26;
            BtnLoadCfg.Text = "加载配置";
            BtnLoadCfg.UseVisualStyleBackColor = true;
            BtnLoadCfg.Click += BtnLoadCfg_Click;
            // 
            // BtnSaveCfg
            // 
            BtnSaveCfg.Location = new System.Drawing.Point(1163, 47);
            BtnSaveCfg.Name = "BtnSaveCfg";
            BtnSaveCfg.Size = new System.Drawing.Size(85, 28);
            BtnSaveCfg.TabIndex = 25;
            BtnSaveCfg.Text = "保存配置";
            BtnSaveCfg.UseVisualStyleBackColor = true;
            BtnSaveCfg.Click += BtnSaveCfg_Click;
            // 
            // LblSubBandCnt
            // 
            LblSubBandCnt.AutoSize = true;
            LblSubBandCnt.Location = new System.Drawing.Point(982, 18);
            LblSubBandCnt.Name = "LblSubBandCnt";
            LblSubBandCnt.Size = new System.Drawing.Size(56, 17);
            LblSubBandCnt.TabIndex = 24;
            LblSubBandCnt.Text = "子带个数";
            // 
            // NudSubbandCnt
            // 
            NudSubbandCnt.Location = new System.Drawing.Point(982, 51);
            NudSubbandCnt.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            NudSubbandCnt.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NudSubbandCnt.Name = "NudSubbandCnt";
            NudSubbandCnt.Size = new System.Drawing.Size(56, 23);
            NudSubbandCnt.TabIndex = 23;
            NudSubbandCnt.Value = new decimal(new int[] { 3, 0, 0, 0 });
            NudSubbandCnt.ValueChanged += NudSubbandCnt_ValueChanged;
            // 
            // TxbParallelRoads
            // 
            TxbParallelRoads.Location = new System.Drawing.Point(889, 50);
            TxbParallelRoads.Name = "TxbParallelRoads";
            TxbParallelRoads.Size = new System.Drawing.Size(56, 23);
            TxbParallelRoads.TabIndex = 22;
            // 
            // LblParallelRoads
            // 
            LblParallelRoads.AutoSize = true;
            LblParallelRoads.Location = new System.Drawing.Point(889, 18);
            LblParallelRoads.Name = "LblParallelRoads";
            LblParallelRoads.Size = new System.Drawing.Size(56, 17);
            LblParallelRoads.TabIndex = 21;
            LblParallelRoads.Text = "并行路数";
            // 
            // TxbSampleFreq
            // 
            TxbSampleFreq.Location = new System.Drawing.Point(765, 50);
            TxbSampleFreq.Name = "TxbSampleFreq";
            TxbSampleFreq.Size = new System.Drawing.Size(92, 23);
            TxbSampleFreq.TabIndex = 20;
            // 
            // LblSampleFreq
            // 
            LblSampleFreq.AutoSize = true;
            LblSampleFreq.Location = new System.Drawing.Point(765, 18);
            LblSampleFreq.Name = "LblSampleFreq";
            LblSampleFreq.Size = new System.Drawing.Size(94, 17);
            LblSampleFreq.TabIndex = 19;
            LblSampleFreq.Text = "总采样率(GSPS)";
            // 
            // BtnRefreshSingleAddress
            // 
            BtnRefreshSingleAddress.Location = new System.Drawing.Point(559, 12);
            BtnRefreshSingleAddress.Name = "BtnRefreshSingleAddress";
            BtnRefreshSingleAddress.Size = new System.Drawing.Size(70, 28);
            BtnRefreshSingleAddress.TabIndex = 18;
            BtnRefreshSingleAddress.Text = "刷新";
            BtnRefreshSingleAddress.UseVisualStyleBackColor = true;
            BtnRefreshSingleAddress.Click += BtnRefreshSingleAddress_Click;
            // 
            // CbxVisaResource
            // 
            CbxVisaResource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CbxVisaResource.FormattingEnabled = true;
            CbxVisaResource.Location = new System.Drawing.Point(471, 51);
            CbxVisaResource.Name = "CbxVisaResource";
            CbxVisaResource.Size = new System.Drawing.Size(253, 25);
            CbxVisaResource.TabIndex = 17;
            // 
            // LblSourceAddress
            // 
            LblSourceAddress.AutoSize = true;
            LblSourceAddress.Location = new System.Drawing.Point(471, 18);
            LblSourceAddress.Name = "LblSourceAddress";
            LblSourceAddress.Size = new System.Drawing.Size(68, 17);
            LblSourceAddress.TabIndex = 16;
            LblSourceAddress.Text = "信号源地址";
            // 
            // CbxChnlSelect
            // 
            CbxChnlSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CbxChnlSelect.FormattingEnabled = true;
            CbxChnlSelect.Items.AddRange(new object[] { "CH1", "CH2", "CH3", "CH4" });
            CbxChnlSelect.Location = new System.Drawing.Point(10, 51);
            CbxChnlSelect.Name = "CbxChnlSelect";
            CbxChnlSelect.Size = new System.Drawing.Size(65, 25);
            CbxChnlSelect.TabIndex = 15;
            CbxChnlSelect.SelectedIndexChanged += CbxChnlSelect_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(10, 18);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(68, 17);
            label2.TabIndex = 14;
            label2.Text = "通道选择：";
            // 
            // BtnSend
            // 
            BtnSend.Location = new System.Drawing.Point(1285, 47);
            BtnSend.Name = "BtnSend";
            BtnSend.Size = new System.Drawing.Size(85, 28);
            BtnSend.TabIndex = 13;
            BtnSend.Text = "生效";
            BtnSend.UseVisualStyleBackColor = true;
            BtnSend.Click += BtnSend_Click;
            // 
            // ChkAutoSend
            // 
            ChkAutoSend.AutoSize = true;
            ChkAutoSend.Location = new System.Drawing.Point(1285, 17);
            ChkAutoSend.Name = "ChkAutoSend";
            ChkAutoSend.Size = new System.Drawing.Size(75, 21);
            ChkAutoSend.TabIndex = 12;
            ChkAutoSend.Text = "自动生效";
            ChkAutoSend.UseVisualStyleBackColor = true;
            // 
            // RtbInfo
            // 
            RtbInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            RtbInfo.Location = new System.Drawing.Point(3, 201);
            RtbInfo.Name = "RtbInfo";
            RtbInfo.Size = new System.Drawing.Size(1586, 291);
            RtbInfo.TabIndex = 2;
            RtbInfo.Text = "";
            // 
            // TabPageDbiDiscardDots
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(TlpMain);
            Name = "TabPageDbiDiscardDots";
            Size = new System.Drawing.Size(1592, 596);
            TlpMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DgvSubband).EndInit();
            PlControl.ResumeLayout(false);
            PlControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NudSubbandCnt).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpMain;
        private System.Windows.Forms.DataGridView DgvSubband;
        private System.Windows.Forms.Panel PlControl;
        private System.Windows.Forms.Button BtnSend;
        private System.Windows.Forms.CheckBox ChkAutoSend;
        private System.Windows.Forms.ComboBox CbxChnlSelect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BtnRefreshSingleAddress;
        private System.Windows.Forms.ComboBox CbxVisaResource;
        private System.Windows.Forms.Label LblSourceAddress;
        private System.Windows.Forms.Label LblSampleFreq;
        private System.Windows.Forms.TextBox TxbSampleFreq;
        private System.Windows.Forms.TextBox TxbParallelRoads;
        private System.Windows.Forms.Label LblParallelRoads;
        private System.Windows.Forms.NumericUpDown NudSubbandCnt;
        private System.Windows.Forms.Label LblSubBandCnt;
        private System.Windows.Forms.Button BtnSaveCfg;
        private System.Windows.Forms.Button BtnLoadCfg;
        private System.Windows.Forms.Button BtnLoadFromFile;
        private System.Windows.Forms.Button BtnSaveToFile;
        private System.Windows.Forms.Button BtnSlectSourceAddress;
        private System.Windows.Forms.RichTextBox RtbInfo;
        private System.Windows.Forms.TextBox TbxCoeLength;
        private System.Windows.Forms.Label LblCoeLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn TxbSubband;
        private System.Windows.Forms.DataGridViewTextBoxColumn TxbDiscardBefore;
        private System.Windows.Forms.DataGridViewTextBoxColumn TxbDiscardAfter;
        private System.Windows.Forms.DataGridViewButtonColumn BtnCalcDiscardDots;
        private System.Windows.Forms.DataGridViewTextBoxColumn TxbInitPhase;
        private System.Windows.Forms.DataGridViewButtonColumn BtnCalcInitPhase;
        private System.Windows.Forms.DataGridViewTextBoxColumn TxbSignalFreq;
        private System.Windows.Forms.DataGridViewTextBoxColumn TxbDiifPhase;
        private System.Windows.Forms.DataGridViewTextBoxColumn TxbLocalFeq;
        private System.Windows.Forms.DataGridViewButtonColumn BtnCalcLocalCoe;
        private System.Windows.Forms.Button BtnSavePhaseDiff;
        private System.Windows.Forms.Button BtnInitPhaseDiffFile;
    }
}
