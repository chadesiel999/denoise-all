namespace ScopeX.Updater.PackageGenerator
{
    partial class FormGenerator
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel9 = new System.Windows.Forms.Panel();
            this.buttonSetSoftwareRelatedPath = new System.Windows.Forms.Button();
            this.textBoxSoftwareRelatedPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.buttonClearAll = new System.Windows.Forms.Button();
            this.panel8 = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.comboBoxProductList = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonSetSaveFileName = new System.Windows.Forms.Button();
            this.textBoxSaveFileName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonGeneratePackage = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.panel3 = new System.Windows.Forms.Panel();
            this.folderBrowserDialogSetRelatedPath = new System.Windows.Forms.FolderBrowserDialog();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.dataGridViewUpdatePackageFiles = new System.Windows.Forms.DataGridView();
            this.ColumnItemName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ColumnButtonSelectFile = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Version = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FirmwareType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.MinDevVer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnCreateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBytes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnButtonDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.panel9.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUpdatePackageFiles)).BeginInit();
            this.SuspendLayout();
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.buttonSetSoftwareRelatedPath);
            this.panel9.Controls.Add(this.textBoxSoftwareRelatedPath);
            this.panel9.Controls.Add(this.label4);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Margin = new System.Windows.Forms.Padding(4);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(1179, 38);
            this.panel9.TabIndex = 1;
            // 
            // buttonSetSoftwareRelatedPath
            // 
            this.buttonSetSoftwareRelatedPath.AutoSize = true;
            this.buttonSetSoftwareRelatedPath.Location = new System.Drawing.Point(914, 3);
            this.buttonSetSoftwareRelatedPath.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonSetSoftwareRelatedPath.Name = "buttonSetSoftwareRelatedPath";
            this.buttonSetSoftwareRelatedPath.Size = new System.Drawing.Size(74, 30);
            this.buttonSetSoftwareRelatedPath.TabIndex = 4;
            this.buttonSetSoftwareRelatedPath.Text = "设置...";
            this.buttonSetSoftwareRelatedPath.UseVisualStyleBackColor = true;
            this.buttonSetSoftwareRelatedPath.Click += new System.EventHandler(this.buttonSetSoftwareRelatedPath_Click);
            // 
            // textBoxSoftwareRelatedPath
            // 
            this.textBoxSoftwareRelatedPath.Location = new System.Drawing.Point(132, 7);
            this.textBoxSoftwareRelatedPath.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxSoftwareRelatedPath.Name = "textBoxSoftwareRelatedPath";
            this.textBoxSoftwareRelatedPath.ReadOnly = true;
            this.textBoxSoftwareRelatedPath.Size = new System.Drawing.Size(776, 23);
            this.textBoxSoftwareRelatedPath.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 10);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "软件相对路径开始于:";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.buttonClearAll);
            this.panel6.Controls.Add(this.panel8);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 480);
            this.panel6.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(1179, 36);
            this.panel6.TabIndex = 1;
            // 
            // buttonClearAll
            // 
            this.buttonClearAll.Location = new System.Drawing.Point(5, 4);
            this.buttonClearAll.Margin = new System.Windows.Forms.Padding(4);
            this.buttonClearAll.Name = "buttonClearAll";
            this.buttonClearAll.Size = new System.Drawing.Size(74, 27);
            this.buttonClearAll.TabIndex = 0;
            this.buttonClearAll.Text = "全部清除";
            this.buttonClearAll.UseVisualStyleBackColor = true;
            this.buttonClearAll.Click += new System.EventHandler(this.buttonClearAll_Click);
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.progressBar1);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel8.Location = new System.Drawing.Point(330, 0);
            this.panel8.Margin = new System.Windows.Forms.Padding(4);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(849, 36);
            this.panel8.TabIndex = 1;
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(0, 0);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(849, 36);
            this.progressBar1.TabIndex = 0;
            // 
            // comboBoxProductList
            // 
            this.comboBoxProductList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProductList.FormattingEnabled = true;
            this.comboBoxProductList.Location = new System.Drawing.Point(46, 1);
            this.comboBoxProductList.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.comboBoxProductList.Name = "comboBoxProductList";
            this.comboBoxProductList.Size = new System.Drawing.Size(250, 25);
            this.comboBoxProductList.TabIndex = 2;
            this.comboBoxProductList.SelectedIndexChanged += new System.EventHandler(this.comboBoxProductList_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(298, 6);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 17);
            this.label3.TabIndex = 1;
            this.label3.Text = "构造需要更新的文件包：";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 4);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "产品";
            // 
            // buttonClose
            // 
            this.buttonClose.AutoSize = true;
            this.buttonClose.Location = new System.Drawing.Point(637, 37);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(199, 38);
            this.buttonClose.TabIndex = 4;
            this.buttonClose.Text = "退出";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonSetSaveFileName
            // 
            this.buttonSetSaveFileName.AutoSize = true;
            this.buttonSetSaveFileName.Location = new System.Drawing.Point(914, 4);
            this.buttonSetSaveFileName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonSetSaveFileName.Name = "buttonSetSaveFileName";
            this.buttonSetSaveFileName.Size = new System.Drawing.Size(74, 30);
            this.buttonSetSaveFileName.TabIndex = 3;
            this.buttonSetSaveFileName.Text = "设置...";
            this.buttonSetSaveFileName.UseVisualStyleBackColor = true;
            this.buttonSetSaveFileName.Click += new System.EventHandler(this.buttonSetSaveFileName_Click);
            // 
            // textBoxSaveFileName
            // 
            this.textBoxSaveFileName.Location = new System.Drawing.Point(84, 8);
            this.textBoxSaveFileName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxSaveFileName.Name = "textBoxSaveFileName";
            this.textBoxSaveFileName.ReadOnly = true;
            this.textBoxSaveFileName.Size = new System.Drawing.Size(825, 23);
            this.textBoxSaveFileName.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 8);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "保存文件为:";
            // 
            // buttonGeneratePackage
            // 
            this.buttonGeneratePackage.AutoSize = true;
            this.buttonGeneratePackage.Location = new System.Drawing.Point(370, 37);
            this.buttonGeneratePackage.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonGeneratePackage.Name = "buttonGeneratePackage";
            this.buttonGeneratePackage.Size = new System.Drawing.Size(199, 38);
            this.buttonGeneratePackage.TabIndex = 0;
            this.buttonGeneratePackage.Text = "生成";
            this.buttonGeneratePackage.UseVisualStyleBackColor = true;
            this.buttonGeneratePackage.Click += new System.EventHandler(this.buttonGeneratePackage_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.buttonClose);
            this.panel3.Controls.Add(this.buttonSetSaveFileName);
            this.panel3.Controls.Add(this.textBoxSaveFileName);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.buttonGeneratePackage);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 516);
            this.panel3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1179, 84);
            this.panel3.TabIndex = 1;
            // 
            // folderBrowserDialogSetRelatedPath
            // 
            this.folderBrowserDialogSetRelatedPath.Description = "选取更新软件所在的目录。缺省情况下，更新软件根据选中的相对路径进行计算。";
            this.folderBrowserDialogSetRelatedPath.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowserDialogSetRelatedPath.ShowNewFolderButton = false;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.comboBoxProductList);
            this.panel5.Controls.Add(this.label3);
            this.panel5.Controls.Add(this.label1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1179, 30);
            this.panel5.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1179, 600);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1179, 516);
            this.panel2.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.panel7);
            this.panel4.Controls.Add(this.panel6);
            this.panel4.Controls.Add(this.panel5);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1179, 516);
            this.panel4.TabIndex = 0;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.dataGridViewUpdatePackageFiles);
            this.panel7.Controls.Add(this.panel9);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(0, 30);
            this.panel7.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(1179, 450);
            this.panel7.TabIndex = 2;
            // 
            // dataGridViewUpdatePackageFiles
            // 
            this.dataGridViewUpdatePackageFiles.AllowUserToAddRows = false;
            this.dataGridViewUpdatePackageFiles.AllowUserToDeleteRows = false;
            this.dataGridViewUpdatePackageFiles.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Blue;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewUpdatePackageFiles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewUpdatePackageFiles.ColumnHeadersHeight = 24;
            this.dataGridViewUpdatePackageFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewUpdatePackageFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnItemName,
            this.ColumnButtonSelectFile,
            this.ColumnFileName,
            this.Version,
            this.FirmwareType,
            this.MinDevVer,
            this.Comment,
            this.ColumnCreateTime,
            this.ColumnBytes,
            this.ColumnButtonDelete});
            this.dataGridViewUpdatePackageFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewUpdatePackageFiles.Location = new System.Drawing.Point(0, 38);
            this.dataGridViewUpdatePackageFiles.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.dataGridViewUpdatePackageFiles.MultiSelect = false;
            this.dataGridViewUpdatePackageFiles.Name = "dataGridViewUpdatePackageFiles";
            this.dataGridViewUpdatePackageFiles.RowHeadersVisible = false;
            this.dataGridViewUpdatePackageFiles.RowHeadersWidth = 51;
            this.dataGridViewUpdatePackageFiles.RowTemplate.Height = 24;
            this.dataGridViewUpdatePackageFiles.Size = new System.Drawing.Size(1179, 412);
            this.dataGridViewUpdatePackageFiles.TabIndex = 0;
            this.dataGridViewUpdatePackageFiles.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewUpdatePackageFiles_CellClick);
            this.dataGridViewUpdatePackageFiles.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewUpdatePackageFiles_CellValueChanged);
            // 
            // ColumnItemName
            // 
            this.ColumnItemName.HeaderText = "部件模块";
            this.ColumnItemName.MinimumWidth = 6;
            this.ColumnItemName.Name = "ColumnItemName";
            this.ColumnItemName.Width = 150;
            // 
            // ColumnButtonSelectFile
            // 
            this.ColumnButtonSelectFile.HeaderText = "浏览";
            this.ColumnButtonSelectFile.MinimumWidth = 6;
            this.ColumnButtonSelectFile.Name = "ColumnButtonSelectFile";
            this.ColumnButtonSelectFile.Width = 40;
            // 
            // ColumnFileName
            // 
            this.ColumnFileName.HeaderText = "文件名称";
            this.ColumnFileName.MinimumWidth = 6;
            this.ColumnFileName.Name = "ColumnFileName";
            this.ColumnFileName.ReadOnly = true;
            this.ColumnFileName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnFileName.Width = 280;
            // 
            // Version
            // 
            this.Version.HeaderText = "固件版本";
            this.Version.MaxInputLength = 12;
            this.Version.MinimumWidth = 6;
            this.Version.Name = "Version";
            this.Version.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Version.Width = 80;
            // 
            // FirmwareType
            // 
            this.FirmwareType.HeaderText = "固件类型";
            this.FirmwareType.Items.AddRange(new object[] {
            "无效",
            "Golden",
            "Application"});
            this.FirmwareType.Name = "FirmwareType";
            this.FirmwareType.ReadOnly = true;
            this.FirmwareType.Width = 150;
            // 
            // MinDevVer
            // 
            this.MinDevVer.HeaderText = "最低驱动版本";
            this.MinDevVer.MaxInputLength = 12;
            this.MinDevVer.MinimumWidth = 6;
            this.MinDevVer.Name = "MinDevVer";
            this.MinDevVer.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.MinDevVer.Width = 110;
            // 
            // Comment
            // 
            this.Comment.HeaderText = "备注(最多255字节)";
            this.Comment.MaxInputLength = 50;
            this.Comment.MinimumWidth = 6;
            this.Comment.Name = "Comment";
            this.Comment.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Comment.ToolTipText = "最多255个字节";
            this.Comment.Width = 300;
            // 
            // ColumnCreateTime
            // 
            this.ColumnCreateTime.HeaderText = "日期时间";
            this.ColumnCreateTime.MaxInputLength = 25;
            this.ColumnCreateTime.MinimumWidth = 6;
            this.ColumnCreateTime.Name = "ColumnCreateTime";
            this.ColumnCreateTime.ReadOnly = true;
            this.ColumnCreateTime.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnCreateTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnCreateTime.Width = 200;
            // 
            // ColumnBytes
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.ColumnBytes.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnBytes.HeaderText = "大小";
            this.ColumnBytes.MaxInputLength = 15;
            this.ColumnBytes.MinimumWidth = 6;
            this.ColumnBytes.Name = "ColumnBytes";
            this.ColumnBytes.ReadOnly = true;
            this.ColumnBytes.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnBytes.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnBytes.Width = 125;
            // 
            // ColumnButtonDelete
            // 
            this.ColumnButtonDelete.HeaderText = "删除";
            this.ColumnButtonDelete.MinimumWidth = 6;
            this.ColumnButtonDelete.Name = "ColumnButtonDelete";
            this.ColumnButtonDelete.Width = 40;
            // 
            // FormGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1179, 600);
            this.Controls.Add(this.panel1);
            this.Name = "FormGenerator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "固件打包工具 ";
            this.Load += new System.EventHandler(this.FormGenerator_Load);
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUpdatePackageFiles)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Button buttonSetSoftwareRelatedPath;
        private System.Windows.Forms.TextBox textBoxSoftwareRelatedPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button buttonClearAll;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ComboBox comboBoxProductList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonSetSaveFileName;
        private System.Windows.Forms.TextBox textBoxSaveFileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonGeneratePackage;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogSetRelatedPath;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.DataGridView dataGridViewUpdatePackageFiles;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColumnItemName;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnButtonSelectFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Version;
        private System.Windows.Forms.DataGridViewComboBoxColumn FirmwareType;
        private System.Windows.Forms.DataGridViewTextBoxColumn MinDevVer;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCreateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnBytes;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnButtonDelete;
    }
}