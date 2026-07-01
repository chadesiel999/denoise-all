namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPageCoefficientParams
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
            TlpMain = new System.Windows.Forms.TableLayoutPanel();
            TlpData = new System.Windows.Forms.TableLayoutPanel();
            DgvFiltrate = new System.Windows.Forms.DataGridView();
            FiltrateSelect = new System.Windows.Forms.DataGridViewComboBoxColumn();
            DgvInfo = new System.Windows.Forms.DataGridView();
            CoefficentsType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            TlpFileData = new System.Windows.Forms.TableLayoutPanel();
            richTextBoxFileContent = new System.Windows.Forms.RichTextBox();
            LblFileData = new System.Windows.Forms.Label();
            TlpOriginData = new System.Windows.Forms.TableLayoutPanel();
            LblOriginData = new System.Windows.Forms.Label();
            richTextBoxUsingCaliData = new System.Windows.Forms.RichTextBox();
            PlControl = new System.Windows.Forms.Panel();
            BtnApply = new System.Windows.Forms.Button();
            BtnReadFromLocalFile = new System.Windows.Forms.Button();
            BtnReadFromOrigin = new System.Windows.Forms.Button();
            buttonSend2Origin = new System.Windows.Forms.Button();
            buttonCaliData_LoadFromFile = new System.Windows.Forms.Button();
            buttonCaliData_SaveToFile = new System.Windows.Forms.Button();
            TlpMain.SuspendLayout();
            TlpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DgvFiltrate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DgvInfo).BeginInit();
            TlpFileData.SuspendLayout();
            TlpOriginData.SuspendLayout();
            PlControl.SuspendLayout();
            SuspendLayout();
            // 
            // TlpMain
            // 
            TlpMain.ColumnCount = 1;
            TlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpMain.Controls.Add(TlpData, 0, 0);
            TlpMain.Controls.Add(PlControl, 0, 1);
            TlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpMain.Location = new System.Drawing.Point(0, 0);
            TlpMain.Name = "TlpMain";
            TlpMain.RowCount = 2;
            TlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            TlpMain.Size = new System.Drawing.Size(968, 535);
            TlpMain.TabIndex = 0;
            // 
            // TlpData
            // 
            TlpData.ColumnCount = 4;
            TlpData.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            TlpData.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpData.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            TlpData.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            TlpData.Controls.Add(DgvFiltrate, 0, 0);
            TlpData.Controls.Add(DgvInfo, 1, 0);
            TlpData.Controls.Add(TlpFileData, 2, 0);
            TlpData.Controls.Add(TlpOriginData, 2, 0);
            TlpData.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpData.Location = new System.Drawing.Point(3, 3);
            TlpData.Name = "TlpData";
            TlpData.RowCount = 1;
            TlpData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpData.Size = new System.Drawing.Size(962, 449);
            TlpData.TabIndex = 0;
            // 
            // DgvFiltrate
            // 
            DgvFiltrate.AllowUserToAddRows = false;
            DgvFiltrate.AllowUserToDeleteRows = false;
            DgvFiltrate.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            DgvFiltrate.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            DgvFiltrate.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvFiltrate.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { FiltrateSelect });
            DgvFiltrate.Dock = System.Windows.Forms.DockStyle.Fill;
            DgvFiltrate.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DgvFiltrate.Location = new System.Drawing.Point(0, 0);
            DgvFiltrate.Margin = new System.Windows.Forms.Padding(0);
            DgvFiltrate.Name = "DgvFiltrate";
            DgvFiltrate.RowHeadersVisible = false;
            DgvFiltrate.RowTemplate.Height = 25;
            DgvFiltrate.Size = new System.Drawing.Size(200, 449);
            DgvFiltrate.TabIndex = 6;
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
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            DgvInfo.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            DgvInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { CoefficentsType });
            DgvInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            DgvInfo.Location = new System.Drawing.Point(203, 3);
            DgvInfo.Name = "DgvInfo";
            DgvInfo.RowHeadersVisible = false;
            DgvInfo.RowTemplate.Height = 25;
            DgvInfo.Size = new System.Drawing.Size(356, 443);
            DgvInfo.TabIndex = 1;
            DgvInfo.CellClick += DgvInfo_CellClick;
            // 
            // CoefficentsType
            // 
            CoefficentsType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            CoefficentsType.HeaderText = "CoefficentsType";
            CoefficentsType.Name = "CoefficentsType";
            CoefficentsType.ReadOnly = true;
            // 
            // TlpFileData
            // 
            TlpFileData.ColumnCount = 1;
            TlpFileData.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpFileData.Controls.Add(richTextBoxFileContent, 0, 1);
            TlpFileData.Controls.Add(LblFileData, 0, 0);
            TlpFileData.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpFileData.Location = new System.Drawing.Point(765, 3);
            TlpFileData.Name = "TlpFileData";
            TlpFileData.RowCount = 2;
            TlpFileData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            TlpFileData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpFileData.Size = new System.Drawing.Size(194, 443);
            TlpFileData.TabIndex = 2;
            // 
            // richTextBoxFileContent
            // 
            richTextBoxFileContent.BackColor = System.Drawing.Color.White;
            richTextBoxFileContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBoxFileContent.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBoxFileContent.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            richTextBoxFileContent.ForeColor = System.Drawing.Color.Black;
            richTextBoxFileContent.Location = new System.Drawing.Point(3, 33);
            richTextBoxFileContent.Name = "richTextBoxFileContent";
            richTextBoxFileContent.ReadOnly = true;
            richTextBoxFileContent.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            richTextBoxFileContent.Size = new System.Drawing.Size(188, 407);
            richTextBoxFileContent.TabIndex = 2;
            richTextBoxFileContent.Text = "";
            // 
            // LblFileData
            // 
            LblFileData.Dock = System.Windows.Forms.DockStyle.Fill;
            LblFileData.Location = new System.Drawing.Point(3, 0);
            LblFileData.Name = "LblFileData";
            LblFileData.Size = new System.Drawing.Size(188, 30);
            LblFileData.TabIndex = 3;
            LblFileData.Text = "本地数据";
            LblFileData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TlpOriginData
            // 
            TlpOriginData.ColumnCount = 1;
            TlpOriginData.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpOriginData.Controls.Add(LblOriginData, 0, 0);
            TlpOriginData.Controls.Add(richTextBoxUsingCaliData, 0, 1);
            TlpOriginData.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpOriginData.Location = new System.Drawing.Point(565, 3);
            TlpOriginData.Name = "TlpOriginData";
            TlpOriginData.RowCount = 2;
            TlpOriginData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            TlpOriginData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpOriginData.Size = new System.Drawing.Size(194, 443);
            TlpOriginData.TabIndex = 3;
            // 
            // LblOriginData
            // 
            LblOriginData.Dock = System.Windows.Forms.DockStyle.Fill;
            LblOriginData.Location = new System.Drawing.Point(3, 0);
            LblOriginData.Name = "LblOriginData";
            LblOriginData.Size = new System.Drawing.Size(188, 30);
            LblOriginData.TabIndex = 4;
            LblOriginData.Text = "远端数据";
            LblOriginData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // richTextBoxUsingCaliData
            // 
            richTextBoxUsingCaliData.BackColor = System.Drawing.Color.Black;
            richTextBoxUsingCaliData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBoxUsingCaliData.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBoxUsingCaliData.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            richTextBoxUsingCaliData.ForeColor = System.Drawing.Color.White;
            richTextBoxUsingCaliData.Location = new System.Drawing.Point(3, 33);
            richTextBoxUsingCaliData.Name = "richTextBoxUsingCaliData";
            richTextBoxUsingCaliData.ReadOnly = true;
            richTextBoxUsingCaliData.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            richTextBoxUsingCaliData.Size = new System.Drawing.Size(188, 407);
            richTextBoxUsingCaliData.TabIndex = 3;
            richTextBoxUsingCaliData.Text = "";
            // 
            // PlControl
            // 
            PlControl.Controls.Add(BtnApply);
            PlControl.Controls.Add(BtnReadFromLocalFile);
            PlControl.Controls.Add(BtnReadFromOrigin);
            PlControl.Controls.Add(buttonSend2Origin);
            PlControl.Controls.Add(buttonCaliData_LoadFromFile);
            PlControl.Controls.Add(buttonCaliData_SaveToFile);
            PlControl.Dock = System.Windows.Forms.DockStyle.Fill;
            PlControl.Location = new System.Drawing.Point(3, 458);
            PlControl.Name = "PlControl";
            PlControl.Size = new System.Drawing.Size(962, 74);
            PlControl.TabIndex = 1;
            // 
            // BtnApply
            // 
            BtnApply.Location = new System.Drawing.Point(343, 38);
            BtnApply.Name = "BtnApply";
            BtnApply.Size = new System.Drawing.Size(142, 28);
            BtnApply.TabIndex = 59;
            BtnApply.Text = "生效文件数据";
            BtnApply.UseVisualStyleBackColor = true;
            BtnApply.Click += BtnApply_Click;
            // 
            // BtnReadFromLocalFile
            // 
            BtnReadFromLocalFile.Location = new System.Drawing.Point(343, 7);
            BtnReadFromLocalFile.Name = "BtnReadFromLocalFile";
            BtnReadFromLocalFile.Size = new System.Drawing.Size(142, 28);
            BtnReadFromLocalFile.TabIndex = 58;
            BtnReadFromLocalFile.Text = "读取本地文件";
            BtnReadFromLocalFile.UseVisualStyleBackColor = true;
            BtnReadFromLocalFile.Click += BtnReadFromLocalFile_Click;
            // 
            // BtnReadFromOrigin
            // 
            BtnReadFromOrigin.Location = new System.Drawing.Point(77, 38);
            BtnReadFromOrigin.Name = "BtnReadFromOrigin";
            BtnReadFromOrigin.Size = new System.Drawing.Size(85, 28);
            BtnReadFromOrigin.TabIndex = 57;
            BtnReadFromOrigin.Text = "从远端读取";
            BtnReadFromOrigin.UseVisualStyleBackColor = true;
            BtnReadFromOrigin.Click += BtnReadFromOrigin_Click;
            // 
            // buttonSend2Origin
            // 
            buttonSend2Origin.Location = new System.Drawing.Point(77, 7);
            buttonSend2Origin.Name = "buttonSend2Origin";
            buttonSend2Origin.Size = new System.Drawing.Size(85, 28);
            buttonSend2Origin.TabIndex = 56;
            buttonSend2Origin.Text = "发送到远端";
            buttonSend2Origin.UseVisualStyleBackColor = true;
            buttonSend2Origin.Click += buttonSend2Origin_Click;
            // 
            // buttonCaliData_LoadFromFile
            // 
            buttonCaliData_LoadFromFile.Location = new System.Drawing.Point(194, 38);
            buttonCaliData_LoadFromFile.Name = "buttonCaliData_LoadFromFile";
            buttonCaliData_LoadFromFile.Size = new System.Drawing.Size(109, 28);
            buttonCaliData_LoadFromFile.TabIndex = 54;
            buttonCaliData_LoadFromFile.Text = "从远端文件装载";
            buttonCaliData_LoadFromFile.UseVisualStyleBackColor = true;
            buttonCaliData_LoadFromFile.Click += buttonCaliData_LoadFromFile_Click;
            // 
            // buttonCaliData_SaveToFile
            // 
            buttonCaliData_SaveToFile.Location = new System.Drawing.Point(194, 7);
            buttonCaliData_SaveToFile.Name = "buttonCaliData_SaveToFile";
            buttonCaliData_SaveToFile.Size = new System.Drawing.Size(109, 28);
            buttonCaliData_SaveToFile.TabIndex = 55;
            buttonCaliData_SaveToFile.Text = "保存到远端文件";
            buttonCaliData_SaveToFile.UseVisualStyleBackColor = true;
            buttonCaliData_SaveToFile.Click += buttonCaliData_SaveToFile_Click;
            // 
            // TabPageCoefficientParams
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(TlpMain);
            Name = "TabPageCoefficientParams";
            Size = new System.Drawing.Size(968, 535);
            TlpMain.ResumeLayout(false);
            TlpData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DgvFiltrate).EndInit();
            ((System.ComponentModel.ISupportInitialize)DgvInfo).EndInit();
            TlpFileData.ResumeLayout(false);
            TlpOriginData.ResumeLayout(false);
            PlControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpMain;
        private System.Windows.Forms.TableLayoutPanel TlpData;
        private System.Windows.Forms.Panel PlControl;
        private System.Windows.Forms.DataGridView DgvInfo;
        private System.Windows.Forms.RichTextBox richTextBoxFileContent;
        private System.Windows.Forms.RichTextBox richTextBoxUsingCaliData;
        private System.Windows.Forms.TableLayoutPanel TlpFileData;
        private System.Windows.Forms.TableLayoutPanel TlpOriginData;
        private System.Windows.Forms.Label LblFileData;
        private System.Windows.Forms.Label LblOriginData;
        private System.Windows.Forms.Button BtnReadFromOrigin;
        private System.Windows.Forms.Button buttonSend2Origin;
        private System.Windows.Forms.Button buttonCaliData_LoadFromFile;
        private System.Windows.Forms.Button buttonCaliData_SaveToFile;
        private System.Windows.Forms.Button BtnReadFromLocalFile;
        private System.Windows.Forms.Button BtnApply;
        private System.Windows.Forms.DataGridView DgvFiltrate;
        private System.Windows.Forms.DataGridViewComboBoxColumn FiltrateSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn CoefficentsType;
    }
}
