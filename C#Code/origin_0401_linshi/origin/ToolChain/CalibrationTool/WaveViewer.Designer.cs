
namespace ScopeX.Hardware.Calibration.Tool
{
    partial class WaveViewer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            tabControl2 = new System.Windows.Forms.TabControl();
            tabPage4 = new System.Windows.Forms.TabPage();
            dataGridViewWaveDataView_Channel = new System.Windows.Forms.DataGridView();
            Column21 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column23 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            Column22 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            panel12 = new System.Windows.Forms.Panel();
            buttonSaveChannelData = new System.Windows.Forms.Button();
            numericUpDownWaveChannelExtractNum = new System.Windows.Forms.NumericUpDown();
            label3 = new System.Windows.Forms.Label();
            buttonChannel_ClearAll = new System.Windows.Forms.Button();
            buttonChannel_SelectAll = new System.Windows.Forms.Button();
            tabPage5 = new System.Windows.Forms.TabPage();
            dataGridViewWaveDataView_Adc = new System.Windows.Forms.DataGridView();
            dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column24 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            dataGridViewButtonColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            panel13 = new System.Windows.Forms.Panel();
            comboBoxADC_Channel = new System.Windows.Forms.ComboBox();
            comboBoxAdcDotLineMode = new System.Windows.Forms.ComboBox();
            numericUpDownAdcZoomCount = new System.Windows.Forms.NumericUpDown();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            buttonAdc_ClearAll = new System.Windows.Forms.Button();
            buttonAdc_SelectAll = new System.Windows.Forms.Button();
            buttonADCRunStop = new System.Windows.Forms.Button();
            colorDialog1 = new System.Windows.Forms.ColorDialog();
            timer1 = new System.Windows.Forms.Timer(components);
            panel1 = new System.Windows.Forms.Panel();
            panel3 = new System.Windows.Forms.Panel();
            labelCursorY1 = new System.Windows.Forms.Label();
            labelCursorY2 = new System.Windows.Forms.Label();
            labelCursorX2 = new System.Windows.Forms.Label();
            labelCursorX1 = new System.Windows.Forms.Label();
            panel4 = new System.Windows.Forms.Panel();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            panel2 = new System.Windows.Forms.Panel();
            richTextBoxCursorResult = new System.Windows.Forms.RichTextBox();
            buttonCursorReset = new System.Windows.Forms.Button();
            checkBoxCursor = new System.Windows.Forms.CheckBox();
            folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            BtnSaveCoreData = new System.Windows.Forms.Button();
            tabControl2.SuspendLayout();
            tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewWaveDataView_Channel).BeginInit();
            panel12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownWaveChannelExtractNum).BeginInit();
            tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewWaveDataView_Adc).BeginInit();
            panel13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownAdcZoomCount).BeginInit();
            panel1.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl2
            // 
            tabControl2.Controls.Add(tabPage4);
            tabControl2.Controls.Add(tabPage5);
            tabControl2.Dock = System.Windows.Forms.DockStyle.Left;
            tabControl2.Location = new System.Drawing.Point(0, 0);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new System.Drawing.Size(294, 419);
            tabControl2.TabIndex = 2;
            tabControl2.SelectedIndexChanged += tabControl2_SelectedIndexChanged;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(dataGridViewWaveDataView_Channel);
            tabPage4.Controls.Add(panel12);
            tabPage4.Location = new System.Drawing.Point(4, 26);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new System.Windows.Forms.Padding(3);
            tabPage4.Size = new System.Drawing.Size(286, 389);
            tabPage4.TabIndex = 0;
            tabPage4.Text = "通道数据";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // dataGridViewWaveDataView_Channel
            // 
            dataGridViewWaveDataView_Channel.AllowUserToAddRows = false;
            dataGridViewWaveDataView_Channel.AllowUserToDeleteRows = false;
            dataGridViewWaveDataView_Channel.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dataGridViewWaveDataView_Channel.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewWaveDataView_Channel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewWaveDataView_Channel.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Column21, Column23, Column22 });
            dataGridViewWaveDataView_Channel.Dock = System.Windows.Forms.DockStyle.Fill;
            dataGridViewWaveDataView_Channel.EnableHeadersVisualStyles = false;
            dataGridViewWaveDataView_Channel.Location = new System.Drawing.Point(3, 3);
            dataGridViewWaveDataView_Channel.Name = "dataGridViewWaveDataView_Channel";
            dataGridViewWaveDataView_Channel.RowHeadersVisible = false;
            dataGridViewWaveDataView_Channel.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewWaveDataView_Channel.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewWaveDataView_Channel.RowTemplate.Height = 23;
            dataGridViewWaveDataView_Channel.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            dataGridViewWaveDataView_Channel.Size = new System.Drawing.Size(280, 323);
            dataGridViewWaveDataView_Channel.TabIndex = 2;
            dataGridViewWaveDataView_Channel.CellDoubleClick += dataGridViewWaveDataView_Channel_CellDoubleClick;
            dataGridViewWaveDataView_Channel.CurrentCellDirtyStateChanged += dataGridViewWaveDataView_Channel_CurrentCellDirtyStateChanged;
            dataGridViewWaveDataView_Channel.RowEnter += dataGridViewWaveDataView_Channel_RowEnter;
            // 
            // Column21
            // 
            Column21.HeaderText = "通道";
            Column21.Name = "Column21";
            Column21.ReadOnly = true;
            Column21.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            Column21.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column23
            // 
            Column23.HeaderText = "选中";
            Column23.Name = "Column23";
            Column23.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            Column23.Width = 50;
            // 
            // Column22
            // 
            Column22.HeaderText = "颜色";
            Column22.Name = "Column22";
            Column22.ReadOnly = true;
            Column22.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            Column22.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Column22.Width = 50;
            // 
            // panel12
            // 
            panel12.Controls.Add(buttonSaveChannelData);
            panel12.Controls.Add(numericUpDownWaveChannelExtractNum);
            panel12.Controls.Add(label3);
            panel12.Controls.Add(buttonChannel_ClearAll);
            panel12.Controls.Add(buttonChannel_SelectAll);
            panel12.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel12.Location = new System.Drawing.Point(3, 326);
            panel12.Name = "panel12";
            panel12.Size = new System.Drawing.Size(280, 60);
            panel12.TabIndex = 1;
            // 
            // buttonSaveChannelData
            // 
            buttonSaveChannelData.Location = new System.Drawing.Point(136, 34);
            buttonSaveChannelData.Name = "buttonSaveChannelData";
            buttonSaveChannelData.Size = new System.Drawing.Size(75, 23);
            buttonSaveChannelData.TabIndex = 10;
            buttonSaveChannelData.Tag = "false";
            buttonSaveChannelData.Text = "保存数据";
            buttonSaveChannelData.UseVisualStyleBackColor = true;
            buttonSaveChannelData.Click += buttonSaveChannelData_Click;
            // 
            // numericUpDownWaveChannelExtractNum
            // 
            numericUpDownWaveChannelExtractNum.Location = new System.Drawing.Point(149, 7);
            numericUpDownWaveChannelExtractNum.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            numericUpDownWaveChannelExtractNum.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownWaveChannelExtractNum.Name = "numericUpDownWaveChannelExtractNum";
            numericUpDownWaveChannelExtractNum.Size = new System.Drawing.Size(62, 23);
            numericUpDownWaveChannelExtractNum.TabIndex = 9;
            numericUpDownWaveChannelExtractNum.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownWaveChannelExtractNum.ValueChanged += ReadrawAtStopMode;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(91, 8);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(59, 17);
            label3.TabIndex = 8;
            label3.Text = "抽点倍数:";
            // 
            // buttonChannel_ClearAll
            // 
            buttonChannel_ClearAll.Location = new System.Drawing.Point(5, 33);
            buttonChannel_ClearAll.Name = "buttonChannel_ClearAll";
            buttonChannel_ClearAll.Size = new System.Drawing.Size(75, 23);
            buttonChannel_ClearAll.TabIndex = 2;
            buttonChannel_ClearAll.Tag = "false";
            buttonChannel_ClearAll.Text = "清除选中";
            buttonChannel_ClearAll.UseVisualStyleBackColor = true;
            buttonChannel_ClearAll.Click += onChannelSelectCtrl;
            // 
            // buttonChannel_SelectAll
            // 
            buttonChannel_SelectAll.Location = new System.Drawing.Point(5, 5);
            buttonChannel_SelectAll.Name = "buttonChannel_SelectAll";
            buttonChannel_SelectAll.Size = new System.Drawing.Size(75, 23);
            buttonChannel_SelectAll.TabIndex = 1;
            buttonChannel_SelectAll.Tag = "true";
            buttonChannel_SelectAll.Text = "全部选中";
            buttonChannel_SelectAll.UseVisualStyleBackColor = true;
            buttonChannel_SelectAll.Click += onChannelSelectCtrl;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(dataGridViewWaveDataView_Adc);
            tabPage5.Controls.Add(panel13);
            tabPage5.Location = new System.Drawing.Point(4, 26);
            tabPage5.Name = "tabPage5";
            tabPage5.Padding = new System.Windows.Forms.Padding(3);
            tabPage5.Size = new System.Drawing.Size(286, 389);
            tabPage5.TabIndex = 1;
            tabPage5.Text = "ADC数据";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // dataGridViewWaveDataView_Adc
            // 
            dataGridViewWaveDataView_Adc.AllowUserToAddRows = false;
            dataGridViewWaveDataView_Adc.AllowUserToDeleteRows = false;
            dataGridViewWaveDataView_Adc.AllowUserToResizeRows = false;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dataGridViewWaveDataView_Adc.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dataGridViewWaveDataView_Adc.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewWaveDataView_Adc.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { dataGridViewCheckBoxColumn1, Column24, dataGridViewButtonColumn1 });
            dataGridViewWaveDataView_Adc.Dock = System.Windows.Forms.DockStyle.Fill;
            dataGridViewWaveDataView_Adc.EnableHeadersVisualStyles = false;
            dataGridViewWaveDataView_Adc.Location = new System.Drawing.Point(3, 3);
            dataGridViewWaveDataView_Adc.Name = "dataGridViewWaveDataView_Adc";
            dataGridViewWaveDataView_Adc.RowHeadersVisible = false;
            dataGridViewWaveDataView_Adc.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewWaveDataView_Adc.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewWaveDataView_Adc.RowTemplate.Height = 23;
            dataGridViewWaveDataView_Adc.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            dataGridViewWaveDataView_Adc.Size = new System.Drawing.Size(280, 323);
            dataGridViewWaveDataView_Adc.TabIndex = 3;
            dataGridViewWaveDataView_Adc.CellDoubleClick += dataGridViewWaveDataView_Adc_CellDoubleClick;
            dataGridViewWaveDataView_Adc.CurrentCellDirtyStateChanged += dataGridViewWaveDataView_Adc_CurrentCellDirtyStateChanged;
            dataGridViewWaveDataView_Adc.RowEnter += dataGridViewWaveDataView_Adc_RowEnter;
            // 
            // dataGridViewCheckBoxColumn1
            // 
            dataGridViewCheckBoxColumn1.HeaderText = "ADC及Core";
            dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            dataGridViewCheckBoxColumn1.ReadOnly = true;
            dataGridViewCheckBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            dataGridViewCheckBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column24
            // 
            Column24.HeaderText = "选中";
            Column24.Name = "Column24";
            Column24.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            Column24.Width = 50;
            // 
            // dataGridViewButtonColumn1
            // 
            dataGridViewButtonColumn1.HeaderText = "颜色";
            dataGridViewButtonColumn1.Name = "dataGridViewButtonColumn1";
            dataGridViewButtonColumn1.ReadOnly = true;
            dataGridViewButtonColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            dataGridViewButtonColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dataGridViewButtonColumn1.Width = 50;
            // 
            // panel13
            // 
            panel13.Controls.Add(BtnSaveCoreData);
            panel13.Controls.Add(comboBoxADC_Channel);
            panel13.Controls.Add(comboBoxAdcDotLineMode);
            panel13.Controls.Add(numericUpDownAdcZoomCount);
            panel13.Controls.Add(label2);
            panel13.Controls.Add(label1);
            panel13.Controls.Add(buttonAdc_ClearAll);
            panel13.Controls.Add(buttonAdc_SelectAll);
            panel13.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel13.Location = new System.Drawing.Point(3, 326);
            panel13.Name = "panel13";
            panel13.Size = new System.Drawing.Size(280, 60);
            panel13.TabIndex = 0;
            // 
            // comboBoxADC_Channel
            // 
            comboBoxADC_Channel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxADC_Channel.FormattingEnabled = true;
            comboBoxADC_Channel.Location = new System.Drawing.Point(192, 4);
            comboBoxADC_Channel.Name = "comboBoxADC_Channel";
            comboBoxADC_Channel.Size = new System.Drawing.Size(61, 25);
            comboBoxADC_Channel.TabIndex = 9;
            // 
            // comboBoxAdcDotLineMode
            // 
            comboBoxAdcDotLineMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxAdcDotLineMode.FormattingEnabled = true;
            comboBoxAdcDotLineMode.Items.AddRange(new object[] { "线", "点" });
            comboBoxAdcDotLineMode.Location = new System.Drawing.Point(139, 5);
            comboBoxAdcDotLineMode.Name = "comboBoxAdcDotLineMode";
            comboBoxAdcDotLineMode.Size = new System.Drawing.Size(44, 25);
            comboBoxAdcDotLineMode.TabIndex = 8;
            comboBoxAdcDotLineMode.SelectedIndexChanged += ReadrawAtStopMode;
            // 
            // numericUpDownAdcZoomCount
            // 
            numericUpDownAdcZoomCount.Location = new System.Drawing.Point(139, 33);
            numericUpDownAdcZoomCount.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            numericUpDownAdcZoomCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownAdcZoomCount.Name = "numericUpDownAdcZoomCount";
            numericUpDownAdcZoomCount.Size = new System.Drawing.Size(44, 23);
            numericUpDownAdcZoomCount.TabIndex = 7;
            numericUpDownAdcZoomCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownAdcZoomCount.ValueChanged += ReadrawAtStopMode;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(81, 34);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(59, 17);
            label2.TabIndex = 6;
            label2.Text = "放大倍数:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(80, 8);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(59, 17);
            label1.TabIndex = 5;
            label1.Text = "点线模式:";
            // 
            // buttonAdc_ClearAll
            // 
            buttonAdc_ClearAll.Location = new System.Drawing.Point(8, 31);
            buttonAdc_ClearAll.Name = "buttonAdc_ClearAll";
            buttonAdc_ClearAll.Size = new System.Drawing.Size(70, 23);
            buttonAdc_ClearAll.TabIndex = 4;
            buttonAdc_ClearAll.Tag = "false";
            buttonAdc_ClearAll.Text = "清除选中";
            buttonAdc_ClearAll.UseVisualStyleBackColor = true;
            buttonAdc_ClearAll.Click += onAdcSelectCtrl;
            // 
            // buttonAdc_SelectAll
            // 
            buttonAdc_SelectAll.Location = new System.Drawing.Point(8, 5);
            buttonAdc_SelectAll.Name = "buttonAdc_SelectAll";
            buttonAdc_SelectAll.Size = new System.Drawing.Size(69, 23);
            buttonAdc_SelectAll.TabIndex = 3;
            buttonAdc_SelectAll.Tag = "true";
            buttonAdc_SelectAll.Text = "全部选中";
            buttonAdc_SelectAll.UseVisualStyleBackColor = true;
            buttonAdc_SelectAll.Click += onAdcSelectCtrl;
            // 
            // buttonADCRunStop
            // 
            buttonADCRunStop.BackColor = System.Drawing.Color.Tomato;
            buttonADCRunStop.Location = new System.Drawing.Point(6, 7);
            buttonADCRunStop.Name = "buttonADCRunStop";
            buttonADCRunStop.Size = new System.Drawing.Size(97, 53);
            buttonADCRunStop.TabIndex = 9;
            buttonADCRunStop.Tag = "stop";
            buttonADCRunStop.Text = "Stopped";
            buttonADCRunStop.UseVisualStyleBackColor = false;
            buttonADCRunStop.Click += buttonADCRunStop_Click;
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // panel1
            // 
            panel1.Controls.Add(panel3);
            panel1.Controls.Add(panel2);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(294, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(617, 419);
            panel1.TabIndex = 4;
            // 
            // panel3
            // 
            panel3.Controls.Add(labelCursorY1);
            panel3.Controls.Add(labelCursorY2);
            panel3.Controls.Add(labelCursorX2);
            panel3.Controls.Add(labelCursorX1);
            panel3.Controls.Add(panel4);
            panel3.Controls.Add(pictureBox1);
            panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            panel3.Location = new System.Drawing.Point(0, 0);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(617, 352);
            panel3.TabIndex = 2;
            // 
            // labelCursorY1
            // 
            labelCursorY1.BackColor = System.Drawing.Color.Black;
            labelCursorY1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            labelCursorY1.Image = Properties.Resources.Arrow_Y1;
            labelCursorY1.Location = new System.Drawing.Point(0, 50);
            labelCursorY1.Margin = new System.Windows.Forms.Padding(0);
            labelCursorY1.Name = "labelCursorY1";
            labelCursorY1.Size = new System.Drawing.Size(21, 15);
            labelCursorY1.TabIndex = 11;
            labelCursorY1.Visible = false;
            labelCursorY1.MouseDown += onCursorYMouseDown;
            labelCursorY1.MouseMove += onCursorYMouseMove;
            labelCursorY1.MouseUp += onCursorYMouseUp;
            // 
            // labelCursorY2
            // 
            labelCursorY2.BackColor = System.Drawing.Color.Black;
            labelCursorY2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            labelCursorY2.Image = Properties.Resources.Arrow_Y2;
            labelCursorY2.Location = new System.Drawing.Point(-1, 92);
            labelCursorY2.Margin = new System.Windows.Forms.Padding(0);
            labelCursorY2.Name = "labelCursorY2";
            labelCursorY2.Size = new System.Drawing.Size(21, 15);
            labelCursorY2.TabIndex = 10;
            labelCursorY2.Visible = false;
            labelCursorY2.MouseDown += onCursorYMouseDown;
            labelCursorY2.MouseMove += onCursorYMouseMove;
            labelCursorY2.MouseUp += onCursorYMouseUp;
            // 
            // labelCursorX2
            // 
            labelCursorX2.BackColor = System.Drawing.Color.Black;
            labelCursorX2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            labelCursorX2.Image = Properties.Resources.Arrow_X2;
            labelCursorX2.Location = new System.Drawing.Point(75, 0);
            labelCursorX2.Margin = new System.Windows.Forms.Padding(0);
            labelCursorX2.Name = "labelCursorX2";
            labelCursorX2.Size = new System.Drawing.Size(15, 21);
            labelCursorX2.TabIndex = 9;
            labelCursorX2.Visible = false;
            labelCursorX2.MouseDown += onCursorXMouseDown;
            labelCursorX2.MouseMove += onCursorXMouseMove;
            labelCursorX2.MouseUp += onCursorXMouseUp;
            // 
            // labelCursorX1
            // 
            labelCursorX1.BackColor = System.Drawing.Color.Black;
            labelCursorX1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            labelCursorX1.Image = Properties.Resources.Arrow_X1;
            labelCursorX1.Location = new System.Drawing.Point(36, 0);
            labelCursorX1.Margin = new System.Windows.Forms.Padding(0);
            labelCursorX1.Name = "labelCursorX1";
            labelCursorX1.Size = new System.Drawing.Size(15, 21);
            labelCursorX1.TabIndex = 8;
            labelCursorX1.Visible = false;
            labelCursorX1.MouseDown += onCursorXMouseDown;
            labelCursorX1.MouseMove += onCursorXMouseMove;
            labelCursorX1.MouseUp += onCursorXMouseUp;
            // 
            // panel4
            // 
            panel4.Dock = System.Windows.Forms.DockStyle.Right;
            panel4.Location = new System.Drawing.Point(613, 0);
            panel4.Name = "panel4";
            panel4.Size = new System.Drawing.Size(4, 352);
            panel4.TabIndex = 2;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = System.Drawing.Color.Black;
            pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            pictureBox1.Location = new System.Drawing.Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(617, 352);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            pictureBox1.Resize += ReadrawAtStopMode;
            // 
            // panel2
            // 
            panel2.Controls.Add(richTextBoxCursorResult);
            panel2.Controls.Add(buttonCursorReset);
            panel2.Controls.Add(buttonADCRunStop);
            panel2.Controls.Add(checkBoxCursor);
            panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel2.Location = new System.Drawing.Point(0, 352);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(617, 67);
            panel2.TabIndex = 0;
            // 
            // richTextBoxCursorResult
            // 
            richTextBoxCursorResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBoxCursorResult.Location = new System.Drawing.Point(199, 8);
            richTextBoxCursorResult.Name = "richTextBoxCursorResult";
            richTextBoxCursorResult.ReadOnly = true;
            richTextBoxCursorResult.Size = new System.Drawing.Size(393, 52);
            richTextBoxCursorResult.TabIndex = 10;
            richTextBoxCursorResult.Text = "";
            richTextBoxCursorResult.Visible = false;
            // 
            // buttonCursorReset
            // 
            buttonCursorReset.Location = new System.Drawing.Point(118, 34);
            buttonCursorReset.Name = "buttonCursorReset";
            buttonCursorReset.Size = new System.Drawing.Size(42, 25);
            buttonCursorReset.TabIndex = 2;
            buttonCursorReset.Text = "复位";
            buttonCursorReset.UseVisualStyleBackColor = true;
            buttonCursorReset.Visible = false;
            buttonCursorReset.Click += buttonCursorReset_Click;
            // 
            // checkBoxCursor
            // 
            checkBoxCursor.AutoSize = true;
            checkBoxCursor.Location = new System.Drawing.Point(118, 9);
            checkBoxCursor.Name = "checkBoxCursor";
            checkBoxCursor.Size = new System.Drawing.Size(75, 21);
            checkBoxCursor.TabIndex = 0;
            checkBoxCursor.Text = "显示光标";
            checkBoxCursor.UseVisualStyleBackColor = true;
            checkBoxCursor.CheckedChanged += checkBoxCursor_CheckedChanged;
            // 
            // folderBrowserDialog1
            // 
            folderBrowserDialog1.Description = "选择文件保存的路径";
            // 
            // BtnSaveCoreData
            // 
            BtnSaveCoreData.Location = new System.Drawing.Point(192, 33);
            BtnSaveCoreData.Name = "BtnSaveCoreData";
            BtnSaveCoreData.Size = new System.Drawing.Size(61, 23);
            BtnSaveCoreData.TabIndex = 10;
            BtnSaveCoreData.Text = "保存";
            BtnSaveCoreData.UseVisualStyleBackColor = true;
            BtnSaveCoreData.Click += BtnSaveCoreData_Click;
            // 
            // WaveViewer
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panel1);
            Controls.Add(tabControl2);
            Name = "WaveViewer";
            Size = new System.Drawing.Size(911, 419);
            tabControl2.ResumeLayout(false);
            tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewWaveDataView_Channel).EndInit();
            panel12.ResumeLayout(false);
            panel12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownWaveChannelExtractNum).EndInit();
            tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewWaveDataView_Adc).EndInit();
            panel13.ResumeLayout(false);
            panel13.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownAdcZoomCount).EndInit();
            panel1.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.DataGridView dataGridViewWaveDataView_Channel;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Button buttonChannel_ClearAll;
        private System.Windows.Forms.Button buttonChannel_SelectAll;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.DataGridView dataGridViewWaveDataView_Adc;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.Button buttonAdc_ClearAll;
        private System.Windows.Forms.Button buttonAdc_SelectAll;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column21;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column23;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column22;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewCheckBoxColumn1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column24;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewButtonColumn1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxAdcDotLineMode;
        private System.Windows.Forms.NumericUpDown numericUpDownAdcZoomCount;
        private System.Windows.Forms.Button buttonADCRunStop;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.NumericUpDown numericUpDownWaveChannelExtractNum;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonSaveChannelData;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ComboBox comboBoxADC_Channel;
        private System.Windows.Forms.CheckBox checkBoxCursor;
        private System.Windows.Forms.Label labelCursorX1;
        private System.Windows.Forms.Label labelCursorX2;
        private System.Windows.Forms.Label labelCursorY1;
        private System.Windows.Forms.Label labelCursorY2;
        private System.Windows.Forms.Button buttonCursorReset;
        private System.Windows.Forms.RichTextBox richTextBoxCursorResult;
        private System.Windows.Forms.Button BtnSaveCoreData;
    }
}
