namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPage_DbiAnalogParams_Common
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
            components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            DgvInfo = new System.Windows.Forms.DataGridView();
            PlControl = new System.Windows.Forms.Panel();
            buttonDefaultRows = new System.Windows.Forms.Button();
            buttonDeleteCurrentRow = new System.Windows.Forms.Button();
            buttonAddNewRow = new System.Windows.Forms.Button();
            textBoxNewKey = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            BtnReadFromOrigin = new System.Windows.Forms.Button();
            buttonSend2Origin = new System.Windows.Forms.Button();
            checkBoxCaliData_TiAdc_AutoSend = new System.Windows.Forms.CheckBox();
            buttonLoadFromOriginFile = new System.Windows.Forms.Button();
            buttonSave2OriginFile = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            comboBoxIncludeOrNotInclude = new System.Windows.Forms.ComboBox();
            labelKeyID = new System.Windows.Forms.Label();
            textBoxKeyIDFilter = new System.Windows.Forms.TextBox();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            richTextBox1 = new System.Windows.Forms.RichTextBox();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)DgvInfo).BeginInit();
            PlControl.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
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
            DgvInfo.MultiSelect = false;
            DgvInfo.Name = "DgvInfo";
            DgvInfo.RowHeadersVisible = false;
            DgvInfo.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            DgvInfo.RowTemplate.Height = 25;
            DgvInfo.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            DgvInfo.Size = new System.Drawing.Size(939, 300);
            DgvInfo.TabIndex = 4;
            DgvInfo.CellEndEdit += DgvInfo_CellEndEdit;
            DgvInfo.CurrentCellDirtyStateChanged += DgvInfo_CurrentCellDirtyStateChanged;
            // 
            // PlControl
            // 
            PlControl.Controls.Add(buttonDefaultRows);
            PlControl.Controls.Add(buttonDeleteCurrentRow);
            PlControl.Controls.Add(buttonAddNewRow);
            PlControl.Controls.Add(textBoxNewKey);
            PlControl.Controls.Add(label1);
            PlControl.Controls.Add(BtnReadFromOrigin);
            PlControl.Controls.Add(buttonSend2Origin);
            PlControl.Controls.Add(checkBoxCaliData_TiAdc_AutoSend);
            PlControl.Controls.Add(buttonLoadFromOriginFile);
            PlControl.Controls.Add(buttonSave2OriginFile);
            PlControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            PlControl.Location = new System.Drawing.Point(0, 476);
            PlControl.Name = "PlControl";
            PlControl.Size = new System.Drawing.Size(939, 65);
            PlControl.TabIndex = 5;
            // 
            // buttonDefaultRows
            // 
            buttonDefaultRows.Location = new System.Drawing.Point(808, 6);
            buttonDefaultRows.Name = "buttonDefaultRows";
            buttonDefaultRows.Size = new System.Drawing.Size(109, 28);
            buttonDefaultRows.TabIndex = 63;
            buttonDefaultRows.Text = "生成全部缺省行";
            buttonDefaultRows.UseVisualStyleBackColor = true;
            buttonDefaultRows.Click += buttonDefaultRows_Click;
            // 
            // buttonDeleteCurrentRow
            // 
            buttonDeleteCurrentRow.Location = new System.Drawing.Point(19, 38);
            buttonDeleteCurrentRow.Name = "buttonDeleteCurrentRow";
            buttonDeleteCurrentRow.Size = new System.Drawing.Size(109, 28);
            buttonDeleteCurrentRow.TabIndex = 62;
            buttonDeleteCurrentRow.Text = "删除当前行";
            buttonDeleteCurrentRow.UseVisualStyleBackColor = true;
            buttonDeleteCurrentRow.Click += buttonDeleteCurrentRow_Click;
            // 
            // buttonAddNewRow
            // 
            buttonAddNewRow.Location = new System.Drawing.Point(673, 7);
            buttonAddNewRow.Name = "buttonAddNewRow";
            buttonAddNewRow.Size = new System.Drawing.Size(109, 28);
            buttonAddNewRow.TabIndex = 61;
            buttonAddNewRow.Text = "添加新行";
            buttonAddNewRow.UseVisualStyleBackColor = true;
            buttonAddNewRow.Click += buttonAddNewRow_Click;
            // 
            // textBoxNewKey
            // 
            textBoxNewKey.Location = new System.Drawing.Point(534, 10);
            textBoxNewKey.Name = "textBoxNewKey";
            textBoxNewKey.Size = new System.Drawing.Size(133, 23);
            textBoxNewKey.TabIndex = 60;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(461, 12);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(68, 17);
            label1.TabIndex = 59;
            label1.Text = "新行关键字";
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
            // panel1
            // 
            panel1.Controls.Add(comboBoxIncludeOrNotInclude);
            panel1.Controls.Add(labelKeyID);
            panel1.Controls.Add(textBoxKeyIDFilter);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(939, 28);
            panel1.TabIndex = 6;
            // 
            // comboBoxIncludeOrNotInclude
            // 
            comboBoxIncludeOrNotInclude.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxIncludeOrNotInclude.FormattingEnabled = true;
            comboBoxIncludeOrNotInclude.Items.AddRange(new object[] { "包含", "不包含" });
            comboBoxIncludeOrNotInclude.Location = new System.Drawing.Point(358, 2);
            comboBoxIncludeOrNotInclude.Name = "comboBoxIncludeOrNotInclude";
            comboBoxIncludeOrNotInclude.Size = new System.Drawing.Size(93, 25);
            comboBoxIncludeOrNotInclude.TabIndex = 62;
            comboBoxIncludeOrNotInclude.SelectedIndexChanged += comboBoxIncludeOrNotInclude_SelectedIndexChanged;
            // 
            // labelKeyID
            // 
            labelKeyID.AutoSize = true;
            labelKeyID.Location = new System.Drawing.Point(3, 5);
            labelKeyID.Name = "labelKeyID";
            labelKeyID.Size = new System.Drawing.Size(102, 17);
            labelKeyID.TabIndex = 59;
            labelKeyID.Text = "KeyID过滤条件：";
            // 
            // textBoxKeyIDFilter
            // 
            textBoxKeyIDFilter.Location = new System.Drawing.Point(106, 2);
            textBoxKeyIDFilter.Name = "textBoxKeyIDFilter";
            textBoxKeyIDFilter.Size = new System.Drawing.Size(246, 23);
            textBoxKeyIDFilter.TabIndex = 60;
            textBoxKeyIDFilter.KeyPress += textBoxKeyIDFilter_KeyPress;
            // 
            // richTextBox1
            // 
            richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBox1.Location = new System.Drawing.Point(0, 0);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new System.Drawing.Size(939, 144);
            richTextBox1.TabIndex = 7;
            richTextBox1.Text = "";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 28);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(DgvInfo);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(richTextBox1);
            splitContainer1.Size = new System.Drawing.Size(939, 448);
            splitContainer1.SplitterDistance = 300;
            splitContainer1.TabIndex = 8;
            // 
            // TabPage_DbiAnalogParams_Common
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Controls.Add(panel1);
            Controls.Add(PlControl);
            Name = "TabPage_DbiAnalogParams_Common";
            Size = new System.Drawing.Size(939, 541);
            ((System.ComponentModel.ISupportInitialize)DgvInfo).EndInit();
            PlControl.ResumeLayout(false);
            PlControl.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView DgvInfo;
        private System.Windows.Forms.Panel PlControl;
        private System.Windows.Forms.Button BtnReadFromOrigin;
        private System.Windows.Forms.Button buttonSend2Origin;
        private System.Windows.Forms.CheckBox checkBoxCaliData_TiAdc_AutoSend;
        private System.Windows.Forms.Button buttonLoadFromOriginFile;
        private System.Windows.Forms.Button buttonSave2OriginFile;
        private System.Windows.Forms.Button buttonAddNewRow;
        private System.Windows.Forms.TextBox textBoxNewKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonDeleteCurrentRow;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelKeyID;
        private System.Windows.Forms.TextBox textBoxKeyIDFilter;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button buttonDefaultRows;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox comboBoxIncludeOrNotInclude;
    }
}
