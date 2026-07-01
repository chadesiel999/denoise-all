namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPageAiAnalogParams
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
            TlpMain = new System.Windows.Forms.TableLayoutPanel();
            PlControl = new System.Windows.Forms.Panel();
            BtnReadFromOrigin = new System.Windows.Forms.Button();
            buttonSend2Origin = new System.Windows.Forms.Button();
            checkBoxCaliData_TiAdc_AutoSend = new System.Windows.Forms.CheckBox();
            buttonLoadFromOriginFile = new System.Windows.Forms.Button();
            buttonSave2OriginFile = new System.Windows.Forms.Button();
            DgvInfo = new System.Windows.Forms.DataGridView();
            TlpMain.SuspendLayout();
            PlControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DgvInfo).BeginInit();
            SuspendLayout();
            // 
            // TlpMain
            // 
            TlpMain.ColumnCount = 1;
            TlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpMain.Controls.Add(PlControl, 0, 1);
            TlpMain.Controls.Add(DgvInfo, 0, 0);
            TlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpMain.Location = new System.Drawing.Point(0, 0);
            TlpMain.Name = "TlpMain";
            TlpMain.RowCount = 2;
            TlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            TlpMain.Size = new System.Drawing.Size(990, 565);
            TlpMain.TabIndex = 0;
            // 
            // PlControl
            // 
            PlControl.Controls.Add(BtnReadFromOrigin);
            PlControl.Controls.Add(buttonSend2Origin);
            PlControl.Controls.Add(checkBoxCaliData_TiAdc_AutoSend);
            PlControl.Controls.Add(buttonLoadFromOriginFile);
            PlControl.Controls.Add(buttonSave2OriginFile);
            PlControl.Dock = System.Windows.Forms.DockStyle.Fill;
            PlControl.Location = new System.Drawing.Point(3, 488);
            PlControl.Name = "PlControl";
            PlControl.Size = new System.Drawing.Size(984, 74);
            PlControl.TabIndex = 5;
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
            // DgvInfo
            // 
            DgvInfo.AllowUserToAddRows = false;
            DgvInfo.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            DgvInfo.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            DgvInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            DgvInfo.Location = new System.Drawing.Point(0, 0);
            DgvInfo.Margin = new System.Windows.Forms.Padding(0);
            DgvInfo.Name = "DgvInfo";
            DgvInfo.RowHeadersVisible = false;
            DgvInfo.RowTemplate.Height = 25;
            DgvInfo.Size = new System.Drawing.Size(990, 485);
            DgvInfo.TabIndex = 4;
            DgvInfo.CellValueChanged += DgvInfo_CellValueChanged;
            DgvInfo.CurrentCellDirtyStateChanged += DgvInfo_CurrentCellDirtyStateChanged;
            // 
            // TabPageAiAnalogParams
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(TlpMain);
            Name = "TabPageAiAnalogParams";
            Size = new System.Drawing.Size(990, 565);
            TlpMain.ResumeLayout(false);
            PlControl.ResumeLayout(false);
            PlControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DgvInfo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpMain;
        private System.Windows.Forms.Panel PlControl;
        private System.Windows.Forms.Button BtnReadFromOrigin;
        private System.Windows.Forms.Button buttonSend2Origin;
        private System.Windows.Forms.CheckBox checkBoxCaliData_TiAdc_AutoSend;
        private System.Windows.Forms.Button buttonLoadFromOriginFile;
        private System.Windows.Forms.Button buttonSave2OriginFile;
        private System.Windows.Forms.DataGridView DgvInfo;
    }
}
