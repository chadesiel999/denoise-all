namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPageAnalogChannelParams
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            TlpGodInfo = new System.Windows.Forms.TableLayoutPanel();
            TlpGodParams = new System.Windows.Forms.TableLayoutPanel();
            DgvVersionAndDate = new System.Windows.Forms.DataGridView();
            GodVersion = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ItemVersion = new System.Windows.Forms.DataGridViewComboBoxColumn();
            CalcDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            DgvGodParams = new System.Windows.Forms.DataGridView();
            PlControl = new System.Windows.Forms.Panel();
            BtnReadFromOrigin = new System.Windows.Forms.Button();
            buttonSend2Origin = new System.Windows.Forms.Button();
            checkBoxCaliData_TiAdc_AutoSend = new System.Windows.Forms.CheckBox();
            buttonLoadFromOriginFile = new System.Windows.Forms.Button();
            buttonSave2OriginFile = new System.Windows.Forms.Button();
            TlpInfo = new System.Windows.Forms.TableLayoutPanel();
            DgvFiltrate = new System.Windows.Forms.DataGridView();
            FiltrateSelect = new System.Windows.Forms.DataGridViewComboBoxColumn();
            DgvInfo = new System.Windows.Forms.DataGridView();
            TlpGodInfo.SuspendLayout();
            TlpGodParams.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DgvVersionAndDate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DgvGodParams).BeginInit();
            PlControl.SuspendLayout();
            TlpInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DgvFiltrate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DgvInfo).BeginInit();
            SuspendLayout();
            // 
            // TlpGodInfo
            // 
            TlpGodInfo.ColumnCount = 1;
            TlpGodInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpGodInfo.Controls.Add(TlpGodParams, 0, 0);
            TlpGodInfo.Controls.Add(PlControl, 0, 2);
            TlpGodInfo.Controls.Add(TlpInfo, 0, 1);
            TlpGodInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpGodInfo.Location = new System.Drawing.Point(0, 0);
            TlpGodInfo.Name = "TlpGodInfo";
            TlpGodInfo.RowCount = 3;
            TlpGodInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            TlpGodInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpGodInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            TlpGodInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TlpGodInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TlpGodInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TlpGodInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TlpGodInfo.Size = new System.Drawing.Size(1033, 564);
            TlpGodInfo.TabIndex = 0;
            // 
            // TlpGodParams
            // 
            TlpGodParams.ColumnCount = 2;
            TlpGodParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpGodParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            TlpGodParams.Controls.Add(DgvVersionAndDate, 1, 0);
            TlpGodParams.Controls.Add(DgvGodParams, 0, 0);
            TlpGodParams.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpGodParams.Location = new System.Drawing.Point(0, 0);
            TlpGodParams.Margin = new System.Windows.Forms.Padding(0);
            TlpGodParams.Name = "TlpGodParams";
            TlpGodParams.RowCount = 1;
            TlpGodParams.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpGodParams.Size = new System.Drawing.Size(1033, 50);
            TlpGodParams.TabIndex = 2;
            // 
            // DgvVersionAndDate
            // 
            DgvVersionAndDate.AllowUserToAddRows = false;
            DgvVersionAndDate.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            DgvVersionAndDate.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            DgvVersionAndDate.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvVersionAndDate.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { GodVersion, ItemVersion, CalcDate });
            DgvVersionAndDate.Dock = System.Windows.Forms.DockStyle.Fill;
            DgvVersionAndDate.Location = new System.Drawing.Point(683, 0);
            DgvVersionAndDate.Margin = new System.Windows.Forms.Padding(0);
            DgvVersionAndDate.Name = "DgvVersionAndDate";
            DgvVersionAndDate.RowHeadersVisible = false;
            DgvVersionAndDate.RowTemplate.Height = 25;
            DgvVersionAndDate.Size = new System.Drawing.Size(350, 50);
            DgvVersionAndDate.TabIndex = 2;
            DgvVersionAndDate.CellValueChanged += DgvVersionAndDate_CellValueChanged;
            DgvVersionAndDate.CurrentCellDirtyStateChanged += DgvVersionAndDate_CurrentCellDirtyStateChanged;
            // 
            // GodVersion
            // 
            GodVersion.HeaderText = "GodVersion";
            GodVersion.Name = "GodVersion";
            GodVersion.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            GodVersion.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ItemVersion
            // 
            ItemVersion.HeaderText = "ItemVersion";
            ItemVersion.Name = "ItemVersion";
            ItemVersion.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            ItemVersion.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // CalcDate
            // 
            CalcDate.HeaderText = "CalcDate";
            CalcDate.Name = "CalcDate";
            CalcDate.Width = 140;
            // 
            // DgvGodParams
            // 
            DgvGodParams.AllowUserToAddRows = false;
            DgvGodParams.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            DgvGodParams.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            DgvGodParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvGodParams.Dock = System.Windows.Forms.DockStyle.Fill;
            DgvGodParams.Location = new System.Drawing.Point(0, 0);
            DgvGodParams.Margin = new System.Windows.Forms.Padding(0);
            DgvGodParams.Name = "DgvGodParams";
            DgvGodParams.RowHeadersVisible = false;
            DgvGodParams.RowTemplate.Height = 25;
            DgvGodParams.Size = new System.Drawing.Size(683, 50);
            DgvGodParams.TabIndex = 1;
            DgvGodParams.CellValueChanged += DgvGodParams_CellValueChanged;
            DgvGodParams.CurrentCellDirtyStateChanged += DgvGodParams_CurrentCellDirtyStateChanged;
            // 
            // PlControl
            // 
            PlControl.Controls.Add(BtnReadFromOrigin);
            PlControl.Controls.Add(buttonSend2Origin);
            PlControl.Controls.Add(checkBoxCaliData_TiAdc_AutoSend);
            PlControl.Controls.Add(buttonLoadFromOriginFile);
            PlControl.Controls.Add(buttonSave2OriginFile);
            PlControl.Dock = System.Windows.Forms.DockStyle.Fill;
            PlControl.Location = new System.Drawing.Point(3, 487);
            PlControl.Name = "PlControl";
            PlControl.Size = new System.Drawing.Size(1027, 74);
            PlControl.TabIndex = 4;
            // 
            // BtnReadFromOrigin
            // 
            BtnReadFromOrigin.Location = new System.Drawing.Point(163, 38);
            BtnReadFromOrigin.Name = "BtnReadFromOrigin";
            BtnReadFromOrigin.Size = new System.Drawing.Size(85, 28);
            BtnReadFromOrigin.TabIndex = 58;
            BtnReadFromOrigin.Text = "从远端读取";
            BtnReadFromOrigin.UseVisualStyleBackColor = true;
            BtnReadFromOrigin.Click += BtnReadFromOrigin_Click;
            // 
            // buttonSend2Origin
            // 
            buttonSend2Origin.Location = new System.Drawing.Point(163, 7);
            buttonSend2Origin.Name = "buttonSend2Origin";
            buttonSend2Origin.Size = new System.Drawing.Size(85, 28);
            buttonSend2Origin.TabIndex = 57;
            buttonSend2Origin.Text = "发送到远端";
            buttonSend2Origin.UseVisualStyleBackColor = true;
            buttonSend2Origin.Click += buttonSend2Origin_Click;
            // 
            // checkBoxCaliData_TiAdc_AutoSend
            // 
            checkBoxCaliData_TiAdc_AutoSend.AutoSize = true;
            checkBoxCaliData_TiAdc_AutoSend.Location = new System.Drawing.Point(19, 12);
            checkBoxCaliData_TiAdc_AutoSend.Name = "checkBoxCaliData_TiAdc_AutoSend";
            checkBoxCaliData_TiAdc_AutoSend.Size = new System.Drawing.Size(111, 21);
            checkBoxCaliData_TiAdc_AutoSend.TabIndex = 56;
            checkBoxCaliData_TiAdc_AutoSend.Text = "自动发送到远端";
            checkBoxCaliData_TiAdc_AutoSend.UseVisualStyleBackColor = true;
            // 
            // buttonLoadFromOriginFile
            // 
            buttonLoadFromOriginFile.Location = new System.Drawing.Point(280, 38);
            buttonLoadFromOriginFile.Name = "buttonLoadFromOriginFile";
            buttonLoadFromOriginFile.Size = new System.Drawing.Size(109, 28);
            buttonLoadFromOriginFile.TabIndex = 54;
            buttonLoadFromOriginFile.Text = "从远端文件装载";
            buttonLoadFromOriginFile.UseVisualStyleBackColor = true;
            buttonLoadFromOriginFile.Click += buttonLoadFromOriginFile_Click;
            // 
            // buttonSave2OriginFile
            // 
            buttonSave2OriginFile.Location = new System.Drawing.Point(280, 7);
            buttonSave2OriginFile.Name = "buttonSave2OriginFile";
            buttonSave2OriginFile.Size = new System.Drawing.Size(109, 28);
            buttonSave2OriginFile.TabIndex = 55;
            buttonSave2OriginFile.Text = "保存到远端文件";
            buttonSave2OriginFile.UseVisualStyleBackColor = true;
            buttonSave2OriginFile.Click += buttonSave2OriginFile_Click;
            // 
            // TlpInfo
            // 
            TlpInfo.ColumnCount = 2;
            TlpInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            TlpInfo.Controls.Add(DgvFiltrate, 1, 0);
            TlpInfo.Controls.Add(DgvInfo, 0, 0);
            TlpInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpInfo.Location = new System.Drawing.Point(0, 50);
            TlpInfo.Margin = new System.Windows.Forms.Padding(0);
            TlpInfo.Name = "TlpInfo";
            TlpInfo.RowCount = 1;
            TlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpInfo.Size = new System.Drawing.Size(1033, 434);
            TlpInfo.TabIndex = 5;
            // 
            // DgvFiltrate
            // 
            DgvFiltrate.AllowUserToAddRows = false;
            DgvFiltrate.AllowUserToDeleteRows = false;
            DgvFiltrate.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            DgvFiltrate.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            DgvFiltrate.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvFiltrate.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { FiltrateSelect });
            DgvFiltrate.Dock = System.Windows.Forms.DockStyle.Fill;
            DgvFiltrate.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DgvFiltrate.Location = new System.Drawing.Point(833, 0);
            DgvFiltrate.Margin = new System.Windows.Forms.Padding(0);
            DgvFiltrate.Name = "DgvFiltrate";
            DgvFiltrate.RowHeadersVisible = false;
            DgvFiltrate.RowTemplate.Height = 25;
            DgvFiltrate.Size = new System.Drawing.Size(200, 434);
            DgvFiltrate.TabIndex = 4;
            DgvFiltrate.CellClick += DgvFiltrate_CellClick;
            DgvFiltrate.CellValueChanged += DgvFiltrate_CellValueChanged;
            DgvFiltrate.CurrentCellDirtyStateChanged += DgvFiltrate_CurrentCellDirtyStateChanged;
            // 
            // FiltrateSelect
            // 
            FiltrateSelect.HeaderText = "FiltrateSelect";
            FiltrateSelect.Name = "FiltrateSelect";
            // 
            // DgvInfo
            // 
            DgvInfo.AllowUserToAddRows = false;
            DgvInfo.AllowUserToDeleteRows = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            DgvInfo.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            DgvInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            DgvInfo.Location = new System.Drawing.Point(0, 0);
            DgvInfo.Margin = new System.Windows.Forms.Padding(0);
            DgvInfo.Name = "DgvInfo";
            DgvInfo.RowHeadersVisible = false;
            DgvInfo.RowTemplate.Height = 25;
            DgvInfo.Size = new System.Drawing.Size(833, 434);
            DgvInfo.TabIndex = 3;
            DgvInfo.CellValueChanged += DgvInfo_CellValueChanged;
            DgvInfo.CurrentCellDirtyStateChanged += DgvInfo_CurrentCellDirtyStateChanged;
            // 
            // TabPageAnalogChannelParams
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(TlpGodInfo);
            Name = "TabPageAnalogChannelParams";
            Size = new System.Drawing.Size(1033, 564);
            TlpGodInfo.ResumeLayout(false);
            TlpGodParams.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DgvVersionAndDate).EndInit();
            ((System.ComponentModel.ISupportInitialize)DgvGodParams).EndInit();
            PlControl.ResumeLayout(false);
            PlControl.PerformLayout();
            TlpInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DgvFiltrate).EndInit();
            ((System.ComponentModel.ISupportInitialize)DgvInfo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpGodInfo;
        private System.Windows.Forms.TableLayoutPanel TlpGodParams;
        private System.Windows.Forms.DataGridView DgvVersionAndDate;
        private System.Windows.Forms.DataGridView DgvGodParams;
        private System.Windows.Forms.DataGridView DgvInfo;
        private System.Windows.Forms.Panel PlControl;
        private System.Windows.Forms.Button BtnReadFromOrigin;
        private System.Windows.Forms.Button buttonSend2Origin;
        private System.Windows.Forms.CheckBox checkBoxCaliData_TiAdc_AutoSend;
        private System.Windows.Forms.Button buttonLoadFromOriginFile;
        private System.Windows.Forms.Button buttonSave2OriginFile;
        private System.Windows.Forms.TableLayoutPanel TlpInfo;
        private System.Windows.Forms.DataGridView DgvFiltrate;
        private System.Windows.Forms.DataGridViewComboBoxColumn GodVersion;
        private System.Windows.Forms.DataGridViewComboBoxColumn ItemVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn CalcDate;
        private System.Windows.Forms.DataGridViewComboBoxColumn FiltrateSelect;
    }
}
