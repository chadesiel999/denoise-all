
namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPageValueSetting
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
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.buttonReadback = new System.Windows.Forms.Button();
            this.buttonSend = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonAllOpen = new System.Windows.Forms.Button();
            this.buttonAllClose = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(0, 0);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(844, 200);
            this.checkedListBox1.TabIndex = 0;
            // 
            // buttonReadback
            // 
            this.buttonReadback.Location = new System.Drawing.Point(215, 3);
            this.buttonReadback.Name = "buttonReadback";
            this.buttonReadback.Size = new System.Drawing.Size(75, 35);
            this.buttonReadback.TabIndex = 1;
            this.buttonReadback.Text = "读取";
            this.buttonReadback.UseVisualStyleBackColor = true;
            this.buttonReadback.Click += new System.EventHandler(this.buttonReadback_Click);
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(480, 2);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(75, 36);
            this.buttonSend.TabIndex = 2;
            this.buttonSend.Text = "设置";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonReadback);
            this.panel1.Controls.Add(this.buttonSend);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 246);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(958, 40);
            this.panel1.TabIndex = 3;
            // 
            // buttonAllOpen
            // 
            this.buttonAllOpen.Location = new System.Drawing.Point(21, 71);
            this.buttonAllOpen.Name = "buttonAllOpen";
            this.buttonAllOpen.Size = new System.Drawing.Size(75, 35);
            this.buttonAllOpen.TabIndex = 4;
            this.buttonAllOpen.Text = "全部打开";
            this.buttonAllOpen.UseVisualStyleBackColor = true;
            this.buttonAllOpen.Click += new System.EventHandler(this.buttonAllOpen_Click);
            // 
            // buttonAllClose
            // 
            this.buttonAllClose.Location = new System.Drawing.Point(21, 23);
            this.buttonAllClose.Name = "buttonAllClose";
            this.buttonAllClose.Size = new System.Drawing.Size(75, 35);
            this.buttonAllClose.TabIndex = 3;
            this.buttonAllClose.Text = "全部关闭";
            this.buttonAllClose.UseVisualStyleBackColor = false;
            this.buttonAllClose.Click += new System.EventHandler(this.buttonAllClose_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.checkedListBox1);
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(958, 246);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.buttonCopy);
            this.panel2.Controls.Add(this.buttonAllOpen);
            this.panel2.Controls.Add(this.buttonAllClose);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(844, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(114, 200);
            this.panel2.TabIndex = 1;
            // 
            // buttonCopy
            // 
            this.buttonCopy.Location = new System.Drawing.Point(6, 162);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(104, 35);
            this.buttonCopy.TabIndex = 5;
            this.buttonCopy.Text = "拷贝到剪贴板";
            this.buttonCopy.UseVisualStyleBackColor = true;
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
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
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(958, 42);
            this.dataGridView1.TabIndex = 0;
            // 
            // Column1
            // 
            this.Column1.Frozen = true;
            this.Column1.HeaderText = "变量名称";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column1.Width = 200;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "值";
            this.Column2.Name = "Column2";
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // TabPageValueSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "TabPageValueSetting";
            this.Size = new System.Drawing.Size(958, 286);
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button buttonReadback;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonAllOpen;
        private System.Windows.Forms.Button buttonAllClose;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonCopy;
    }
}
