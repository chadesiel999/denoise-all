
namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPageSoftLA
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxCursorAlign = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxDataDisplayFormat = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonOpenProject = new System.Windows.Forms.Button();
            this.buttonCapture = new System.Windows.Forms.Button();
            this.comboBoxGroup = new System.Windows.Forms.ComboBox();
            this.comboBoxDefinedModule = new System.Windows.Forms.ComboBox();
            this.textBoxProject = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.comboBoxCursorAlign);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.comboBoxDataDisplayFormat);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.buttonOpenProject);
            this.panel1.Controls.Add(this.buttonCapture);
            this.panel1.Controls.Add(this.comboBoxGroup);
            this.panel1.Controls.Add(this.comboBoxDefinedModule);
            this.panel1.Controls.Add(this.textBoxProject);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1352, 34);
            this.panel1.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(888, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 17);
            this.label5.TabIndex = 12;
            this.label5.Text = "光标对齐:";
            // 
            // comboBoxCursorAlign
            // 
            this.comboBoxCursorAlign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCursorAlign.FormattingEnabled = true;
            this.comboBoxCursorAlign.Items.AddRange(new object[] {
            "时钟沿",
            "时钟中间"});
            this.comboBoxCursorAlign.Location = new System.Drawing.Point(948, 5);
            this.comboBoxCursorAlign.Name = "comboBoxCursorAlign";
            this.comboBoxCursorAlign.Size = new System.Drawing.Size(83, 25);
            this.comboBoxCursorAlign.TabIndex = 11;
            this.comboBoxCursorAlign.SelectedIndexChanged += new System.EventHandler(this.comboBoxCursorAlign_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1055, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "数据显示进制:";
            // 
            // comboBoxDataDisplayFormat
            // 
            this.comboBoxDataDisplayFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDataDisplayFormat.FormattingEnabled = true;
            this.comboBoxDataDisplayFormat.Items.AddRange(new object[] {
            "2",
            "8",
            "10",
            "16"});
            this.comboBoxDataDisplayFormat.Location = new System.Drawing.Point(1139, 5);
            this.comboBoxDataDisplayFormat.Name = "comboBoxDataDisplayFormat";
            this.comboBoxDataDisplayFormat.Size = new System.Drawing.Size(53, 25);
            this.comboBoxDataDisplayFormat.TabIndex = 9;
            this.comboBoxDataDisplayFormat.SelectedIndexChanged += new System.EventHandler(this.comboBoxDataDisplayFormat_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(536, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "分组：";
            // 
            // buttonOpenProject
            // 
            this.buttonOpenProject.Location = new System.Drawing.Point(233, 5);
            this.buttonOpenProject.Name = "buttonOpenProject";
            this.buttonOpenProject.Size = new System.Drawing.Size(61, 23);
            this.buttonOpenProject.TabIndex = 5;
            this.buttonOpenProject.Text = "打开";
            this.buttonOpenProject.UseVisualStyleBackColor = true;
            this.buttonOpenProject.Click += new System.EventHandler(this.buttonOpenProject_Click);
            // 
            // buttonCapture
            // 
            this.buttonCapture.Location = new System.Drawing.Point(783, 4);
            this.buttonCapture.Name = "buttonCapture";
            this.buttonCapture.Size = new System.Drawing.Size(75, 23);
            this.buttonCapture.TabIndex = 4;
            this.buttonCapture.Text = "抓取";
            this.buttonCapture.UseVisualStyleBackColor = true;
            this.buttonCapture.Click += new System.EventHandler(this.buttonCapture_Click);
            // 
            // comboBoxGroup
            // 
            this.comboBoxGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGroup.FormattingEnabled = true;
            this.comboBoxGroup.Location = new System.Drawing.Point(586, 3);
            this.comboBoxGroup.Name = "comboBoxGroup";
            this.comboBoxGroup.Size = new System.Drawing.Size(151, 25);
            this.comboBoxGroup.TabIndex = 2;
            this.comboBoxGroup.SelectedIndexChanged += new System.EventHandler(this.comboBoxGroup_SelectedIndexChanged);
            // 
            // comboBoxDefinedModule
            // 
            this.comboBoxDefinedModule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDefinedModule.FormattingEnabled = true;
            this.comboBoxDefinedModule.Location = new System.Drawing.Point(339, 3);
            this.comboBoxDefinedModule.Name = "comboBoxDefinedModule";
            this.comboBoxDefinedModule.Size = new System.Drawing.Size(191, 25);
            this.comboBoxDefinedModule.TabIndex = 1;
            this.comboBoxDefinedModule.SelectedIndexChanged += new System.EventHandler(this.comboBoxDefinedModule_SelectedIndexChanged);
            // 
            // textBoxProject
            // 
            this.textBoxProject.Location = new System.Drawing.Point(43, 5);
            this.textBoxProject.Name = "textBoxProject";
            this.textBoxProject.Size = new System.Drawing.Size(183, 23);
            this.textBoxProject.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(300, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "模块：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "工程：";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.Black;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column5,
            this.Column3,
            this.Column4});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.Size = new System.Drawing.Size(448, 554);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting);
            this.dataGridView1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dataGridView1_Scroll);
            // 
            // Column1
            // 
            this.Column1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Column1.Frozen = true;
            this.Column1.HeaderText = "";
            this.Column1.MinimumWidth = 6;
            this.Column1.Name = "Column1";
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column1.Text = "↓";
            this.Column1.Width = 16;
            // 
            // Column2
            // 
            this.Column2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Column2.Frozen = true;
            this.Column2.HeaderText = "";
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column2.Text = "↑";
            this.Column2.Width = 16;
            // 
            // Column5
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            this.Column5.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Column5.Frozen = true;
            this.Column5.HeaderText = "";
            this.Column5.MinimumWidth = 6;
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column5.Width = 16;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "信号";
            this.Column3.MinimumWidth = 6;
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column3.Width = 125;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "数据";
            this.Column4.MinimumWidth = 6;
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column4.Width = 125;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 34);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer1.Size = new System.Drawing.Size(1352, 554);
            this.splitContainer1.SplitterDistance = 448;
            this.splitContainer1.TabIndex = 2;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(900, 554);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.SizeChanged += new System.EventHandler(this.pictureBox1_SizeChanged);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseLeave += new System.EventHandler(this.pictureBox1_MouseLeave);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // TabPageSoftLA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "TabPageSoftLA";
            this.Size = new System.Drawing.Size(1352, 588);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonOpenProject;
        private System.Windows.Forms.Button buttonCapture;
        private System.Windows.Forms.ComboBox comboBoxGroup;
        private System.Windows.Forms.ComboBox comboBoxDefinedModule;
        private System.Windows.Forms.TextBox textBoxProject;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.DataGridViewButtonColumn Column1;
        private System.Windows.Forms.DataGridViewButtonColumn Column2;
        private System.Windows.Forms.DataGridViewButtonColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxDataDisplayFormat;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxCursorAlign;
    }
}
