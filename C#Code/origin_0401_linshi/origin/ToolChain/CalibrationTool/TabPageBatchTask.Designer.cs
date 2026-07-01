
namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPageBatchTask
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            panel1 = new System.Windows.Forms.Panel();
            BtnGenerateXml = new System.Windows.Forms.Button();
            buttonReload = new System.Windows.Forms.Button();
            buttonOpenResultFile = new System.Windows.Forms.Button();
            panel5 = new System.Windows.Forms.Panel();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            richTextBoxTaskDescription = new System.Windows.Forms.RichTextBox();
            richTextBoxParameter = new System.Windows.Forms.RichTextBox();
            tabPage2 = new System.Windows.Forms.TabPage();
            richTextBoxCurrTaskTypeParameterDescription = new System.Windows.Forms.RichTextBox();
            tabPage3 = new System.Windows.Forms.TabPage();
            richTextBoxTaskPartHelp = new System.Windows.Forms.RichTextBox();
            panel6 = new System.Windows.Forms.Panel();
            comboBoxTaskParts = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            tabPage4 = new System.Windows.Forms.TabPage();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            dataGridViewFileMessage = new System.Windows.Forms.DataGridView();
            Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dataGridViewMuliXMLFile_InstrumentationInfo = new System.Windows.Forms.DataGridView();
            Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            comboBoxTasks = new System.Windows.Forms.ComboBox();
            buttonCancelTask = new System.Windows.Forms.Button();
            buttonStartTask = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            panel2 = new System.Windows.Forms.Panel();
            labelStep = new System.Windows.Forms.Label();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            panel3 = new System.Windows.Forms.Panel();
            panel4 = new System.Windows.Forms.Panel();
            richTextBoxResultMessage = new System.Windows.Forms.RichTextBox();
            panel7 = new System.Windows.Forms.Panel();
            labelCurrentStepMessage = new System.Windows.Forms.Label();
            labelCurrXmlFileName = new System.Windows.Forms.Label();
            LvStaticInfo = new System.Windows.Forms.ListView();
            ChFile = new System.Windows.Forms.ColumnHeader();
            ChStatic = new System.Windows.Forms.ColumnHeader();
            ChRet = new System.Windows.Forms.ColumnHeader();
            ChMore = new System.Windows.Forms.ColumnHeader();
            panel1.SuspendLayout();
            panel5.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            panel6.SuspendLayout();
            tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewFileMessage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewMuliXMLFile_InstrumentationInfo).BeginInit();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            panel7.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(BtnGenerateXml);
            panel1.Controls.Add(buttonReload);
            panel1.Controls.Add(buttonOpenResultFile);
            panel1.Controls.Add(panel5);
            panel1.Controls.Add(comboBoxTasks);
            panel1.Controls.Add(buttonCancelTask);
            panel1.Controls.Add(buttonStartTask);
            panel1.Controls.Add(label1);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(1113, 244);
            panel1.TabIndex = 0;
            // 
            // BtnGenerateXml
            // 
            BtnGenerateXml.Location = new System.Drawing.Point(801, 8);
            BtnGenerateXml.Name = "BtnGenerateXml";
            BtnGenerateXml.Size = new System.Drawing.Size(75, 23);
            BtnGenerateXml.TabIndex = 8;
            BtnGenerateXml.Text = "生成XML";
            BtnGenerateXml.UseVisualStyleBackColor = true;
            BtnGenerateXml.Click += BtnGenerateXml_Click;
            // 
            // buttonReload
            // 
            buttonReload.Location = new System.Drawing.Point(704, 9);
            buttonReload.Name = "buttonReload";
            buttonReload.Size = new System.Drawing.Size(75, 23);
            buttonReload.TabIndex = 7;
            buttonReload.Text = "重载配置";
            buttonReload.UseVisualStyleBackColor = true;
            buttonReload.Click += buttonReload_Click;
            // 
            // buttonOpenResultFile
            // 
            buttonOpenResultFile.Location = new System.Drawing.Point(575, 10);
            buttonOpenResultFile.Name = "buttonOpenResultFile";
            buttonOpenResultFile.Size = new System.Drawing.Size(107, 23);
            buttonOpenResultFile.TabIndex = 6;
            buttonOpenResultFile.Text = "打开结果文件";
            buttonOpenResultFile.UseVisualStyleBackColor = true;
            buttonOpenResultFile.Visible = false;
            buttonOpenResultFile.Click += buttonOpenResultFile_Click;
            // 
            // panel5
            // 
            panel5.BackColor = System.Drawing.SystemColors.ButtonShadow;
            panel5.Controls.Add(tabControl1);
            panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel5.Location = new System.Drawing.Point(0, 41);
            panel5.Name = "panel5";
            panel5.Size = new System.Drawing.Size(1113, 203);
            panel5.TabIndex = 5;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(1113, 203);
            tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(richTextBoxTaskDescription);
            tabPage1.Controls.Add(richTextBoxParameter);
            tabPage1.Location = new System.Drawing.Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(1105, 173);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "任务描述及当前参数设置";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // richTextBoxTaskDescription
            // 
            richTextBoxTaskDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBoxTaskDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBoxTaskDescription.Location = new System.Drawing.Point(3, 3);
            richTextBoxTaskDescription.Name = "richTextBoxTaskDescription";
            richTextBoxTaskDescription.ReadOnly = true;
            richTextBoxTaskDescription.Size = new System.Drawing.Size(1099, 125);
            richTextBoxTaskDescription.TabIndex = 4;
            richTextBoxTaskDescription.Text = "";
            // 
            // richTextBoxParameter
            // 
            richTextBoxParameter.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBoxParameter.Dock = System.Windows.Forms.DockStyle.Bottom;
            richTextBoxParameter.Location = new System.Drawing.Point(3, 128);
            richTextBoxParameter.Name = "richTextBoxParameter";
            richTextBoxParameter.ReadOnly = true;
            richTextBoxParameter.Size = new System.Drawing.Size(1099, 42);
            richTextBoxParameter.TabIndex = 5;
            richTextBoxParameter.Text = "";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(richTextBoxCurrTaskTypeParameterDescription);
            tabPage2.Location = new System.Drawing.Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(3);
            tabPage2.Size = new System.Drawing.Size(1105, 173);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "当前任务类型参数约定";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // richTextBoxCurrTaskTypeParameterDescription
            // 
            richTextBoxCurrTaskTypeParameterDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBoxCurrTaskTypeParameterDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBoxCurrTaskTypeParameterDescription.Location = new System.Drawing.Point(3, 3);
            richTextBoxCurrTaskTypeParameterDescription.Name = "richTextBoxCurrTaskTypeParameterDescription";
            richTextBoxCurrTaskTypeParameterDescription.ReadOnly = true;
            richTextBoxCurrTaskTypeParameterDescription.Size = new System.Drawing.Size(1099, 167);
            richTextBoxCurrTaskTypeParameterDescription.TabIndex = 5;
            richTextBoxCurrTaskTypeParameterDescription.Text = "";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(richTextBoxTaskPartHelp);
            tabPage3.Controls.Add(panel6);
            tabPage3.Location = new System.Drawing.Point(4, 26);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new System.Windows.Forms.Padding(3);
            tabPage3.Size = new System.Drawing.Size(1105, 173);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "系统定制Task约定";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // richTextBoxTaskPartHelp
            // 
            richTextBoxTaskPartHelp.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBoxTaskPartHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBoxTaskPartHelp.Location = new System.Drawing.Point(3, 36);
            richTextBoxTaskPartHelp.Name = "richTextBoxTaskPartHelp";
            richTextBoxTaskPartHelp.ReadOnly = true;
            richTextBoxTaskPartHelp.Size = new System.Drawing.Size(1099, 134);
            richTextBoxTaskPartHelp.TabIndex = 6;
            richTextBoxTaskPartHelp.Text = "";
            // 
            // panel6
            // 
            panel6.BackColor = System.Drawing.Color.Silver;
            panel6.Controls.Add(comboBoxTaskParts);
            panel6.Controls.Add(label2);
            panel6.Dock = System.Windows.Forms.DockStyle.Top;
            panel6.Location = new System.Drawing.Point(3, 3);
            panel6.Name = "panel6";
            panel6.Size = new System.Drawing.Size(1099, 33);
            panel6.TabIndex = 7;
            // 
            // comboBoxTaskParts
            // 
            comboBoxTaskParts.BackColor = System.Drawing.Color.LightGray;
            comboBoxTaskParts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxTaskParts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            comboBoxTaskParts.FormattingEnabled = true;
            comboBoxTaskParts.Location = new System.Drawing.Point(130, 3);
            comboBoxTaskParts.Name = "comboBoxTaskParts";
            comboBoxTaskParts.Size = new System.Drawing.Size(466, 25);
            comboBoxTaskParts.TabIndex = 0;
            comboBoxTaskParts.SelectedIndexChanged += comboBoxTaskParts_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(3, 8);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(130, 17);
            label2.TabIndex = 1;
            label2.Text = "系统实现的TaskPart：";
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(splitContainer1);
            tabPage4.Location = new System.Drawing.Point(4, 26);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new System.Windows.Forms.Padding(3);
            tabPage4.Size = new System.Drawing.Size(1105, 173);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "当前任务文件版本信息";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(3, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(dataGridViewFileMessage);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(dataGridViewMuliXMLFile_InstrumentationInfo);
            splitContainer1.Size = new System.Drawing.Size(1099, 167);
            splitContainer1.SplitterDistance = 83;
            splitContainer1.TabIndex = 1;
            // 
            // dataGridViewFileMessage
            // 
            dataGridViewFileMessage.AllowUserToAddRows = false;
            dataGridViewFileMessage.AllowUserToDeleteRows = false;
            dataGridViewFileMessage.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dataGridViewFileMessage.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewFileMessage.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewFileMessage.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Column1, Column5, Column6, Column2, Column3, Column4 });
            dataGridViewFileMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            dataGridViewFileMessage.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            dataGridViewFileMessage.Location = new System.Drawing.Point(0, 0);
            dataGridViewFileMessage.Name = "dataGridViewFileMessage";
            dataGridViewFileMessage.ReadOnly = true;
            dataGridViewFileMessage.RowHeadersVisible = false;
            dataGridViewFileMessage.RowTemplate.Height = 25;
            dataGridViewFileMessage.Size = new System.Drawing.Size(1099, 83);
            dataGridViewFileMessage.TabIndex = 0;
            // 
            // Column1
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            Column1.DefaultCellStyle = dataGridViewCellStyle2;
            Column1.HeaderText = "文件名称";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Column1.Width = 300;
            // 
            // Column5
            // 
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            Column5.DefaultCellStyle = dataGridViewCellStyle3;
            Column5.HeaderText = "创建人";
            Column5.Name = "Column5";
            Column5.ReadOnly = true;
            Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column6
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            Column6.DefaultCellStyle = dataGridViewCellStyle4;
            Column6.HeaderText = "最后修改人";
            Column6.Name = "Column6";
            Column6.ReadOnly = true;
            Column6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column2
            // 
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            Column2.DefaultCellStyle = dataGridViewCellStyle5;
            Column2.HeaderText = "最后修改日期";
            Column2.Name = "Column2";
            Column2.ReadOnly = true;
            Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Column2.Width = 160;
            // 
            // Column3
            // 
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            Column3.DefaultCellStyle = dataGridViewCellStyle6;
            Column3.HeaderText = "版本号";
            Column3.Name = "Column3";
            Column3.ReadOnly = true;
            Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column4
            // 
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.White;
            Column4.DefaultCellStyle = dataGridViewCellStyle7;
            Column4.HeaderText = "描述";
            Column4.Name = "Column4";
            Column4.ReadOnly = true;
            Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Column4.Width = 300;
            // 
            // dataGridViewMuliXMLFile_InstrumentationInfo
            // 
            dataGridViewMuliXMLFile_InstrumentationInfo.AllowUserToAddRows = false;
            dataGridViewMuliXMLFile_InstrumentationInfo.AllowUserToDeleteRows = false;
            dataGridViewMuliXMLFile_InstrumentationInfo.AllowUserToResizeRows = false;
            dataGridViewMuliXMLFile_InstrumentationInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewMuliXMLFile_InstrumentationInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Column7, Column8 });
            dataGridViewMuliXMLFile_InstrumentationInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            dataGridViewMuliXMLFile_InstrumentationInfo.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            dataGridViewMuliXMLFile_InstrumentationInfo.Location = new System.Drawing.Point(0, 0);
            dataGridViewMuliXMLFile_InstrumentationInfo.Name = "dataGridViewMuliXMLFile_InstrumentationInfo";
            dataGridViewMuliXMLFile_InstrumentationInfo.RowHeadersVisible = false;
            dataGridViewMuliXMLFile_InstrumentationInfo.RowTemplate.Height = 25;
            dataGridViewMuliXMLFile_InstrumentationInfo.ShowCellErrors = false;
            dataGridViewMuliXMLFile_InstrumentationInfo.ShowEditingIcon = false;
            dataGridViewMuliXMLFile_InstrumentationInfo.Size = new System.Drawing.Size(1099, 80);
            dataGridViewMuliXMLFile_InstrumentationInfo.TabIndex = 0;
            // 
            // Column7
            // 
            Column7.FillWeight = 200F;
            Column7.HeaderText = "仪器名称";
            Column7.Name = "Column7";
            Column7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Column7.Width = 200;
            // 
            // Column8
            // 
            Column8.FillWeight = 300F;
            Column8.HeaderText = "仪器地址";
            Column8.Name = "Column8";
            Column8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Column8.Width = 300;
            // 
            // comboBoxTasks
            // 
            comboBoxTasks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxTasks.FormattingEnabled = true;
            comboBoxTasks.Location = new System.Drawing.Point(68, 8);
            comboBoxTasks.Name = "comboBoxTasks";
            comboBoxTasks.Size = new System.Drawing.Size(246, 25);
            comboBoxTasks.TabIndex = 3;
            comboBoxTasks.SelectedIndexChanged += comboBoxTasks_SelectedIndexChanged;
            // 
            // buttonCancelTask
            // 
            buttonCancelTask.Location = new System.Drawing.Point(452, 9);
            buttonCancelTask.Name = "buttonCancelTask";
            buttonCancelTask.Size = new System.Drawing.Size(75, 23);
            buttonCancelTask.TabIndex = 2;
            buttonCancelTask.Text = "停止";
            buttonCancelTask.UseVisualStyleBackColor = true;
            buttonCancelTask.Click += buttonCancelTask_Click;
            // 
            // buttonStartTask
            // 
            buttonStartTask.Location = new System.Drawing.Point(343, 9);
            buttonStartTask.Name = "buttonStartTask";
            buttonStartTask.Size = new System.Drawing.Size(75, 23);
            buttonStartTask.TabIndex = 1;
            buttonStartTask.Text = "开始";
            buttonStartTask.UseVisualStyleBackColor = true;
            buttonStartTask.Click += buttonStartTask_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(1, 12);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(68, 17);
            label1.TabIndex = 0;
            label1.Text = "任务名称：";
            // 
            // panel2
            // 
            panel2.Controls.Add(labelStep);
            panel2.Controls.Add(progressBar1);
            panel2.Dock = System.Windows.Forms.DockStyle.Top;
            panel2.Location = new System.Drawing.Point(0, 244);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(1113, 46);
            panel2.TabIndex = 1;
            // 
            // labelStep
            // 
            labelStep.AutoSize = true;
            labelStep.BackColor = System.Drawing.Color.Transparent;
            labelStep.Dock = System.Windows.Forms.DockStyle.Fill;
            labelStep.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            labelStep.Location = new System.Drawing.Point(0, 0);
            labelStep.Name = "labelStep";
            labelStep.Size = new System.Drawing.Size(27, 17);
            labelStep.TabIndex = 1;
            labelStep.Text = "1/1";
            // 
            // progressBar1
            // 
            progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            progressBar1.Location = new System.Drawing.Point(0, 0);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(1113, 46);
            progressBar1.TabIndex = 0;
            // 
            // panel3
            // 
            panel3.Controls.Add(panel4);
            panel3.Controls.Add(LvStaticInfo);
            panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            panel3.Location = new System.Drawing.Point(0, 290);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(1113, 289);
            panel3.TabIndex = 2;
            // 
            // panel4
            // 
            panel4.Controls.Add(richTextBoxResultMessage);
            panel4.Controls.Add(panel7);
            panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            panel4.Location = new System.Drawing.Point(0, 0);
            panel4.Name = "panel4";
            panel4.Size = new System.Drawing.Size(648, 289);
            panel4.TabIndex = 0;
            // 
            // richTextBoxResultMessage
            // 
            richTextBoxResultMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBoxResultMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBoxResultMessage.Location = new System.Drawing.Point(0, 40);
            richTextBoxResultMessage.Name = "richTextBoxResultMessage";
            richTextBoxResultMessage.ReadOnly = true;
            richTextBoxResultMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            richTextBoxResultMessage.Size = new System.Drawing.Size(648, 249);
            richTextBoxResultMessage.TabIndex = 1;
            richTextBoxResultMessage.Text = "";
            // 
            // panel7
            // 
            panel7.Controls.Add(labelCurrentStepMessage);
            panel7.Controls.Add(labelCurrXmlFileName);
            panel7.Dock = System.Windows.Forms.DockStyle.Top;
            panel7.Location = new System.Drawing.Point(0, 0);
            panel7.Name = "panel7";
            panel7.Size = new System.Drawing.Size(648, 40);
            panel7.TabIndex = 2;
            // 
            // labelCurrentStepMessage
            // 
            labelCurrentStepMessage.AutoSize = true;
            labelCurrentStepMessage.Dock = System.Windows.Forms.DockStyle.Top;
            labelCurrentStepMessage.Location = new System.Drawing.Point(0, 17);
            labelCurrentStepMessage.Name = "labelCurrentStepMessage";
            labelCurrentStepMessage.Size = new System.Drawing.Size(43, 17);
            labelCurrentStepMessage.TabIndex = 0;
            labelCurrentStepMessage.Text = "label2";
            // 
            // labelCurrXmlFileName
            // 
            labelCurrXmlFileName.AutoSize = true;
            labelCurrXmlFileName.Dock = System.Windows.Forms.DockStyle.Top;
            labelCurrXmlFileName.Location = new System.Drawing.Point(0, 0);
            labelCurrXmlFileName.Name = "labelCurrXmlFileName";
            labelCurrXmlFileName.Size = new System.Drawing.Size(43, 17);
            labelCurrXmlFileName.TabIndex = 0;
            labelCurrXmlFileName.Text = "label3";
            // 
            // LvStaticInfo
            // 
            LvStaticInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { ChFile, ChStatic, ChRet, ChMore });
            LvStaticInfo.Dock = System.Windows.Forms.DockStyle.Right;
            LvStaticInfo.Location = new System.Drawing.Point(648, 0);
            LvStaticInfo.Name = "LvStaticInfo";
            LvStaticInfo.OwnerDraw = true;
            LvStaticInfo.Size = new System.Drawing.Size(465, 289);
            LvStaticInfo.TabIndex = 3;
            LvStaticInfo.UseCompatibleStateImageBehavior = false;
            LvStaticInfo.View = System.Windows.Forms.View.Details;
            // 
            // ChFile
            // 
            ChFile.Text = "FileName";
            ChFile.Width = 200;
            // 
            // ChStatic
            // 
            ChStatic.Text = "Static";
            ChStatic.Width = 100;
            // 
            // ChRet
            // 
            ChRet.Text = "Result";
            ChRet.Width = 100;
            // 
            // ChMore
            // 
            ChMore.Text = "More";
            // 
            // TabPageBatchTask
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "TabPageBatchTask";
            Size = new System.Drawing.Size(1113, 579);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel5.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            tabPage4.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewFileMessage).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewMuliXMLFile_InstrumentationInfo).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel7.ResumeLayout(false);
            panel7.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonCancelTask;
        private System.Windows.Forms.Button buttonStartTask;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RichTextBox richTextBoxResultMessage;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label labelCurrentStepMessage;
        private System.Windows.Forms.ComboBox comboBoxTasks;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.RichTextBox richTextBoxTaskDescription;
        private System.Windows.Forms.Button buttonOpenResultFile;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox richTextBoxParameter;
        private System.Windows.Forms.RichTextBox richTextBoxCurrTaskTypeParameterDescription;
        private System.Windows.Forms.Button buttonReload;
        private System.Windows.Forms.Label labelStep;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.RichTextBox richTextBoxTaskPartHelp;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxTaskParts;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label labelCurrXmlFileName;
        private System.Windows.Forms.ListView LvStaticInfo;
        private System.Windows.Forms.ColumnHeader ChFile;
        private System.Windows.Forms.ColumnHeader ChRet;
        private System.Windows.Forms.ColumnHeader ChMore;
        private System.Windows.Forms.ColumnHeader ChStatic;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.DataGridView dataGridViewFileMessage;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridViewMuliXMLFile_InstrumentationInfo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.Button BtnGenerateXml;
    }
}
