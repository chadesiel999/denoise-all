namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPageTiadcPhaseOffsetGain
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            TlpMain = new System.Windows.Forms.TableLayoutPanel();
            TlpInfo = new System.Windows.Forms.TableLayoutPanel();
            TlpParamsInfo = new System.Windows.Forms.TableLayoutPanel();
            DgvInfo = new System.Windows.Forms.DataGridView();
            TlpGodParams = new System.Windows.Forms.TableLayoutPanel();
            DgvVersionAndDate = new System.Windows.Forms.DataGridView();
            GodVersion = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ItemVersion = new System.Windows.Forms.DataGridViewComboBoxColumn();
            CalcDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            DgvGodParams = new System.Windows.Forms.DataGridView();
            TlpControl = new System.Windows.Forms.TableLayoutPanel();
            DgvError = new System.Windows.Forms.DataGridView();
            DgvFiltrate = new System.Windows.Forms.DataGridView();
            FiltrateSelect = new System.Windows.Forms.DataGridViewComboBoxColumn();
            PlControl = new System.Windows.Forms.Panel();
            BtnReadFromOrigin = new System.Windows.Forms.Button();
            GpbAutoCali = new System.Windows.Forms.GroupBox();
            LblGainThrehold = new System.Windows.Forms.Label();
            numericUpDownGainLimit = new System.Windows.Forms.NumericUpDown();
            label1 = new System.Windows.Forms.Label();
            numericUpDownPhaseLimit = new System.Windows.Forms.NumericUpDown();
            LblOffsetThrehold = new System.Windows.Forms.Label();
            numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            LblPhaseThrehold = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            numericUpDownOverTimeOfMin = new System.Windows.Forms.NumericUpDown();
            label7 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            GpbCalcError = new System.Windows.Forms.GroupBox();
            tb_Avg = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            LblSampleFrequency = new System.Windows.Forms.Label();
            textBoxTotalADCSamplingRadioByGSPS = new System.Windows.Forms.TextBox();
            label15 = new System.Windows.Forms.Label();
            textBoxSignalFreqByMHz = new System.Windows.Forms.TextBox();
            BtnAutoCali = new System.Windows.Forms.Button();
            buttonCalcError = new System.Windows.Forms.Button();
            buttonCaliData_TiAdc_Send = new System.Windows.Forms.Button();
            checkBoxCaliData_TiAdc_AutoSend = new System.Windows.Forms.CheckBox();
            buttonCaliData_TiAdc_LoadFromFile = new System.Windows.Forms.Button();
            buttonCaliData_TiAdc_SaveToFile = new System.Windows.Forms.Button();
            TlpMain.SuspendLayout();
            TlpInfo.SuspendLayout();
            TlpParamsInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DgvInfo).BeginInit();
            TlpGodParams.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DgvVersionAndDate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DgvGodParams).BeginInit();
            TlpControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DgvError).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DgvFiltrate).BeginInit();
            PlControl.SuspendLayout();
            GpbAutoCali.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownGainLimit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownPhaseLimit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownOverTimeOfMin).BeginInit();
            GpbCalcError.SuspendLayout();
            SuspendLayout();
            // 
            // TlpMain
            // 
            TlpMain.ColumnCount = 1;
            TlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpMain.Controls.Add(TlpInfo, 0, 0);
            TlpMain.Controls.Add(PlControl, 0, 1);
            TlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpMain.Location = new System.Drawing.Point(0, 0);
            TlpMain.Margin = new System.Windows.Forms.Padding(0);
            TlpMain.Name = "TlpMain";
            TlpMain.RowCount = 2;
            TlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            TlpMain.Size = new System.Drawing.Size(1394, 458);
            TlpMain.TabIndex = 0;
            // 
            // TlpInfo
            // 
            TlpInfo.ColumnCount = 2;
            TlpInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            TlpInfo.Controls.Add(TlpParamsInfo, 0, 0);
            TlpInfo.Controls.Add(TlpControl, 1, 0);
            TlpInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpInfo.Location = new System.Drawing.Point(3, 3);
            TlpInfo.Name = "TlpInfo";
            TlpInfo.RowCount = 1;
            TlpInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpInfo.Size = new System.Drawing.Size(1388, 372);
            TlpInfo.TabIndex = 1;
            // 
            // TlpParamsInfo
            // 
            TlpParamsInfo.ColumnCount = 1;
            TlpParamsInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpParamsInfo.Controls.Add(DgvInfo, 0, 0);
            TlpParamsInfo.Controls.Add(TlpGodParams, 0, 1);
            TlpParamsInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpParamsInfo.Location = new System.Drawing.Point(0, 0);
            TlpParamsInfo.Margin = new System.Windows.Forms.Padding(0);
            TlpParamsInfo.Name = "TlpParamsInfo";
            TlpParamsInfo.RowCount = 2;
            TlpParamsInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpParamsInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            TlpParamsInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TlpParamsInfo.Size = new System.Drawing.Size(988, 372);
            TlpParamsInfo.TabIndex = 2;
            // 
            // DgvInfo
            // 
            DgvInfo.AllowUserToAddRows = false;
            DgvInfo.AllowUserToDeleteRows = false;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            DgvInfo.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            DgvInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            DgvInfo.Location = new System.Drawing.Point(3, 3);
            DgvInfo.Name = "DgvInfo";
            DgvInfo.RowHeadersVisible = false;
            DgvInfo.RowTemplate.Height = 25;
            DgvInfo.Size = new System.Drawing.Size(982, 316);
            DgvInfo.TabIndex = 0;
            DgvInfo.CellValueChanged += DgvInfo_CellValueChanged;
            DgvInfo.CurrentCellDirtyStateChanged += DgvInfo_CurrentCellDirtyStateChanged;
            // 
            // TlpGodParams
            // 
            TlpGodParams.ColumnCount = 2;
            TlpGodParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpGodParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            TlpGodParams.Controls.Add(DgvVersionAndDate, 1, 0);
            TlpGodParams.Controls.Add(DgvGodParams, 0, 0);
            TlpGodParams.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpGodParams.Location = new System.Drawing.Point(0, 322);
            TlpGodParams.Margin = new System.Windows.Forms.Padding(0);
            TlpGodParams.Name = "TlpGodParams";
            TlpGodParams.RowCount = 1;
            TlpGodParams.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpGodParams.Size = new System.Drawing.Size(988, 50);
            TlpGodParams.TabIndex = 1;
            // 
            // DgvVersionAndDate
            // 
            DgvVersionAndDate.AllowUserToAddRows = false;
            DgvVersionAndDate.AllowUserToDeleteRows = false;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            DgvVersionAndDate.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            DgvVersionAndDate.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvVersionAndDate.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { GodVersion, ItemVersion, CalcDate });
            DgvVersionAndDate.Dock = System.Windows.Forms.DockStyle.Fill;
            DgvVersionAndDate.Location = new System.Drawing.Point(638, 0);
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
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            DgvGodParams.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            DgvGodParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvGodParams.Dock = System.Windows.Forms.DockStyle.Fill;
            DgvGodParams.Location = new System.Drawing.Point(0, 0);
            DgvGodParams.Margin = new System.Windows.Forms.Padding(0);
            DgvGodParams.Name = "DgvGodParams";
            DgvGodParams.RowHeadersVisible = false;
            DgvGodParams.RowTemplate.Height = 25;
            DgvGodParams.Size = new System.Drawing.Size(638, 50);
            DgvGodParams.TabIndex = 1;
            DgvGodParams.CellValueChanged += DgvGodParams_CellValueChanged;
            DgvGodParams.CurrentCellDirtyStateChanged += DgvGodParams_CurrentCellDirtyStateChanged;
            // 
            // TlpControl
            // 
            TlpControl.ColumnCount = 1;
            TlpControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TlpControl.Controls.Add(DgvError, 0, 1);
            TlpControl.Controls.Add(DgvFiltrate, 0, 0);
            TlpControl.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpControl.Location = new System.Drawing.Point(991, 3);
            TlpControl.Name = "TlpControl";
            TlpControl.RowCount = 2;
            TlpControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TlpControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TlpControl.Size = new System.Drawing.Size(394, 366);
            TlpControl.TabIndex = 3;
            // 
            // DgvError
            // 
            DgvError.AllowUserToAddRows = false;
            DgvError.AllowUserToDeleteRows = false;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            DgvError.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
            DgvError.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvError.Dock = System.Windows.Forms.DockStyle.Fill;
            DgvError.Location = new System.Drawing.Point(3, 186);
            DgvError.Name = "DgvError";
            DgvError.RowHeadersVisible = false;
            DgvError.RowTemplate.Height = 25;
            DgvError.Size = new System.Drawing.Size(388, 177);
            DgvError.TabIndex = 1;
            // 
            // DgvFiltrate
            // 
            DgvFiltrate.AllowUserToAddRows = false;
            DgvFiltrate.AllowUserToDeleteRows = false;
            DgvFiltrate.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            DgvFiltrate.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            DgvFiltrate.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvFiltrate.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { FiltrateSelect });
            DgvFiltrate.Dock = System.Windows.Forms.DockStyle.Fill;
            DgvFiltrate.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DgvFiltrate.Location = new System.Drawing.Point(0, 0);
            DgvFiltrate.Margin = new System.Windows.Forms.Padding(0);
            DgvFiltrate.Name = "DgvFiltrate";
            DgvFiltrate.RowHeadersVisible = false;
            DgvFiltrate.RowTemplate.Height = 25;
            DgvFiltrate.Size = new System.Drawing.Size(394, 183);
            DgvFiltrate.TabIndex = 5;
            DgvFiltrate.CellClick += DgvFiltrate_CellClick;
            DgvFiltrate.CellValueChanged += DgvFiltrate_CellValueChanged;
            DgvFiltrate.CurrentCellDirtyStateChanged += DgvFiltrate_CurrentCellDirtyStateChanged;
            // 
            // FiltrateSelect
            // 
            FiltrateSelect.HeaderText = "FiltrateSelect";
            FiltrateSelect.Name = "FiltrateSelect";
            // 
            // PlControl
            // 
            PlControl.Controls.Add(BtnReadFromOrigin);
            PlControl.Controls.Add(GpbAutoCali);
            PlControl.Controls.Add(GpbCalcError);
            PlControl.Controls.Add(BtnAutoCali);
            PlControl.Controls.Add(buttonCalcError);
            PlControl.Controls.Add(buttonCaliData_TiAdc_Send);
            PlControl.Controls.Add(checkBoxCaliData_TiAdc_AutoSend);
            PlControl.Controls.Add(buttonCaliData_TiAdc_LoadFromFile);
            PlControl.Controls.Add(buttonCaliData_TiAdc_SaveToFile);
            PlControl.Dock = System.Windows.Forms.DockStyle.Fill;
            PlControl.Location = new System.Drawing.Point(0, 378);
            PlControl.Margin = new System.Windows.Forms.Padding(0);
            PlControl.Name = "PlControl";
            PlControl.Size = new System.Drawing.Size(1394, 80);
            PlControl.TabIndex = 1;
            // 
            // BtnReadFromOrigin
            // 
            BtnReadFromOrigin.Location = new System.Drawing.Point(163, 40);
            BtnReadFromOrigin.Name = "BtnReadFromOrigin";
            BtnReadFromOrigin.Size = new System.Drawing.Size(85, 28);
            BtnReadFromOrigin.TabIndex = 53;
            BtnReadFromOrigin.Text = "从远端读取";
            BtnReadFromOrigin.UseVisualStyleBackColor = true;
            BtnReadFromOrigin.Click += BtnReadFromOrigin_Click;
            // 
            // GpbAutoCali
            // 
            GpbAutoCali.Controls.Add(LblGainThrehold);
            GpbAutoCali.Controls.Add(numericUpDownGainLimit);
            GpbAutoCali.Controls.Add(label1);
            GpbAutoCali.Controls.Add(numericUpDownPhaseLimit);
            GpbAutoCali.Controls.Add(LblOffsetThrehold);
            GpbAutoCali.Controls.Add(numericUpDown1);
            GpbAutoCali.Controls.Add(LblPhaseThrehold);
            GpbAutoCali.Controls.Add(label5);
            GpbAutoCali.Controls.Add(numericUpDownOverTimeOfMin);
            GpbAutoCali.Controls.Add(label7);
            GpbAutoCali.Controls.Add(label4);
            GpbAutoCali.Location = new System.Drawing.Point(930, 0);
            GpbAutoCali.Name = "GpbAutoCali";
            GpbAutoCali.Size = new System.Drawing.Size(461, 74);
            GpbAutoCali.TabIndex = 52;
            GpbAutoCali.TabStop = false;
            GpbAutoCali.Text = "自动校准参数设置";
            // 
            // LblGainThrehold
            // 
            LblGainThrehold.AutoSize = true;
            LblGainThrehold.Location = new System.Drawing.Point(15, 21);
            LblGainThrehold.Name = "LblGainThrehold";
            LblGainThrehold.Size = new System.Drawing.Size(92, 17);
            LblGainThrehold.TabIndex = 40;
            LblGainThrehold.Text = "增益误差门限：";
            // 
            // numericUpDownGainLimit
            // 
            numericUpDownGainLimit.Location = new System.Drawing.Point(113, 19);
            numericUpDownGainLimit.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownGainLimit.Name = "numericUpDownGainLimit";
            numericUpDownGainLimit.Size = new System.Drawing.Size(64, 23);
            numericUpDownGainLimit.TabIndex = 41;
            numericUpDownGainLimit.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(183, 48);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(52, 17);
            label1.TabIndex = 50;
            label1.Text = "(万分之)";
            // 
            // numericUpDownPhaseLimit
            // 
            numericUpDownPhaseLimit.Location = new System.Drawing.Point(364, 19);
            numericUpDownPhaseLimit.Maximum = new decimal(new int[] { 50000, 0, 0, 0 });
            numericUpDownPhaseLimit.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownPhaseLimit.Name = "numericUpDownPhaseLimit";
            numericUpDownPhaseLimit.Size = new System.Drawing.Size(64, 23);
            numericUpDownPhaseLimit.TabIndex = 42;
            numericUpDownPhaseLimit.Value = new decimal(new int[] { 500, 0, 0, 0 });
            // 
            // LblOffsetThrehold
            // 
            LblOffsetThrehold.AutoSize = true;
            LblOffsetThrehold.Location = new System.Drawing.Point(15, 48);
            LblOffsetThrehold.Name = "LblOffsetThrehold";
            LblOffsetThrehold.Size = new System.Drawing.Size(92, 17);
            LblOffsetThrehold.TabIndex = 49;
            LblOffsetThrehold.Text = "偏置误差门限：";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new System.Drawing.Point(113, 46);
            numericUpDown1.Maximum = new decimal(new int[] { 50000, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new System.Drawing.Size(64, 23);
            numericUpDown1.TabIndex = 48;
            numericUpDown1.Value = new decimal(new int[] { 500, 0, 0, 0 });
            // 
            // LblPhaseThrehold
            // 
            LblPhaseThrehold.AutoSize = true;
            LblPhaseThrehold.Location = new System.Drawing.Point(264, 21);
            LblPhaseThrehold.Name = "LblPhaseThrehold";
            LblPhaseThrehold.Size = new System.Drawing.Size(92, 17);
            LblPhaseThrehold.TabIndex = 43;
            LblPhaseThrehold.Text = "相位误差门限：";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(185, 19);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(52, 17);
            label5.TabIndex = 46;
            label5.Text = "(万分之)";
            // 
            // numericUpDownOverTimeOfMin
            // 
            numericUpDownOverTimeOfMin.Location = new System.Drawing.Point(365, 45);
            numericUpDownOverTimeOfMin.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownOverTimeOfMin.Name = "numericUpDownOverTimeOfMin";
            numericUpDownOverTimeOfMin.Size = new System.Drawing.Size(64, 23);
            numericUpDownOverTimeOfMin.TabIndex = 45;
            numericUpDownOverTimeOfMin.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(434, 21);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(19, 17);
            label7.TabIndex = 47;
            label7.Text = "fS";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(264, 48);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(92, 17);
            label4.TabIndex = 44;
            label4.Text = "最大迭代次数：";
            // 
            // GpbCalcError
            // 
            GpbCalcError.Controls.Add(tb_Avg);
            GpbCalcError.Controls.Add(label2);
            GpbCalcError.Controls.Add(LblSampleFrequency);
            GpbCalcError.Controls.Add(textBoxTotalADCSamplingRadioByGSPS);
            GpbCalcError.Controls.Add(label15);
            GpbCalcError.Controls.Add(textBoxSignalFreqByMHz);
            GpbCalcError.Location = new System.Drawing.Point(633, 3);
            GpbCalcError.Name = "GpbCalcError";
            GpbCalcError.Size = new System.Drawing.Size(291, 75);
            GpbCalcError.TabIndex = 51;
            GpbCalcError.TabStop = false;
            GpbCalcError.Text = "误差计算参数设置";
            // 
            // tb_Avg
            // 
            tb_Avg.Location = new System.Drawing.Point(221, 18);
            tb_Avg.Name = "tb_Avg";
            tb_Avg.Size = new System.Drawing.Size(36, 23);
            tb_Avg.TabIndex = 39;
            tb_Avg.Text = "1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(180, 24);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(35, 17);
            label2.TabIndex = 38;
            label2.Text = "平均:";
            // 
            // LblSampleFrequency
            // 
            LblSampleFrequency.AutoSize = true;
            LblSampleFrequency.Location = new System.Drawing.Point(6, 24);
            LblSampleFrequency.Name = "LblSampleFrequency";
            LblSampleFrequency.Size = new System.Drawing.Size(109, 17);
            LblSampleFrequency.TabIndex = 34;
            LblSampleFrequency.Text = "单路采样率(GSPS):";
            // 
            // textBoxTotalADCSamplingRadioByGSPS
            // 
            textBoxTotalADCSamplingRadioByGSPS.Location = new System.Drawing.Point(121, 18);
            textBoxTotalADCSamplingRadioByGSPS.Name = "textBoxTotalADCSamplingRadioByGSPS";
            textBoxTotalADCSamplingRadioByGSPS.Size = new System.Drawing.Size(36, 23);
            textBoxTotalADCSamplingRadioByGSPS.TabIndex = 35;
            textBoxTotalADCSamplingRadioByGSPS.Text = "40";
            textBoxTotalADCSamplingRadioByGSPS.TextChanged += textBoxTotalADCSamplingRadioByGSPS_TextChanged;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(6, 51);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(94, 17);
            label15.TabIndex = 36;
            label15.Text = "信号频率(MHz):";
            // 
            // textBoxSignalFreqByMHz
            // 
            textBoxSignalFreqByMHz.Location = new System.Drawing.Point(103, 48);
            textBoxSignalFreqByMHz.Name = "textBoxSignalFreqByMHz";
            textBoxSignalFreqByMHz.Size = new System.Drawing.Size(54, 23);
            textBoxSignalFreqByMHz.TabIndex = 37;
            textBoxSignalFreqByMHz.Text = "100";
            // 
            // BtnAutoCali
            // 
            BtnAutoCali.Location = new System.Drawing.Point(479, 41);
            BtnAutoCali.Name = "BtnAutoCali";
            BtnAutoCali.Size = new System.Drawing.Size(95, 28);
            BtnAutoCali.TabIndex = 39;
            BtnAutoCali.Text = "自动校准";
            BtnAutoCali.UseVisualStyleBackColor = true;
            // 
            // buttonCalcError
            // 
            buttonCalcError.Location = new System.Drawing.Point(479, 8);
            buttonCalcError.Name = "buttonCalcError";
            buttonCalcError.Size = new System.Drawing.Size(95, 28);
            buttonCalcError.TabIndex = 38;
            buttonCalcError.Text = "计算误差";
            buttonCalcError.UseVisualStyleBackColor = true;
            buttonCalcError.Click += buttonCalcError_Click;
            // 
            // buttonCaliData_TiAdc_Send
            // 
            buttonCaliData_TiAdc_Send.Location = new System.Drawing.Point(163, 9);
            buttonCaliData_TiAdc_Send.Name = "buttonCaliData_TiAdc_Send";
            buttonCaliData_TiAdc_Send.Size = new System.Drawing.Size(85, 28);
            buttonCaliData_TiAdc_Send.TabIndex = 15;
            buttonCaliData_TiAdc_Send.Text = "发送到远端";
            buttonCaliData_TiAdc_Send.UseVisualStyleBackColor = true;
            buttonCaliData_TiAdc_Send.Click += buttonCaliData_TiAdc_Send_Click;
            // 
            // checkBoxCaliData_TiAdc_AutoSend
            // 
            checkBoxCaliData_TiAdc_AutoSend.AutoSize = true;
            checkBoxCaliData_TiAdc_AutoSend.Location = new System.Drawing.Point(19, 14);
            checkBoxCaliData_TiAdc_AutoSend.Name = "checkBoxCaliData_TiAdc_AutoSend";
            checkBoxCaliData_TiAdc_AutoSend.Size = new System.Drawing.Size(111, 21);
            checkBoxCaliData_TiAdc_AutoSend.TabIndex = 14;
            checkBoxCaliData_TiAdc_AutoSend.Text = "自动发送到远端";
            checkBoxCaliData_TiAdc_AutoSend.UseVisualStyleBackColor = true;
            // 
            // buttonCaliData_TiAdc_LoadFromFile
            // 
            buttonCaliData_TiAdc_LoadFromFile.Location = new System.Drawing.Point(307, 40);
            buttonCaliData_TiAdc_LoadFromFile.Name = "buttonCaliData_TiAdc_LoadFromFile";
            buttonCaliData_TiAdc_LoadFromFile.Size = new System.Drawing.Size(109, 28);
            buttonCaliData_TiAdc_LoadFromFile.TabIndex = 12;
            buttonCaliData_TiAdc_LoadFromFile.Text = "从远端文件装载";
            buttonCaliData_TiAdc_LoadFromFile.UseVisualStyleBackColor = true;
            buttonCaliData_TiAdc_LoadFromFile.Click += buttonCaliData_TiAdc_LoadFromFile_Click;
            // 
            // buttonCaliData_TiAdc_SaveToFile
            // 
            buttonCaliData_TiAdc_SaveToFile.Location = new System.Drawing.Point(307, 9);
            buttonCaliData_TiAdc_SaveToFile.Name = "buttonCaliData_TiAdc_SaveToFile";
            buttonCaliData_TiAdc_SaveToFile.Size = new System.Drawing.Size(109, 28);
            buttonCaliData_TiAdc_SaveToFile.TabIndex = 13;
            buttonCaliData_TiAdc_SaveToFile.Text = "保存到远端文件";
            buttonCaliData_TiAdc_SaveToFile.UseVisualStyleBackColor = true;
            buttonCaliData_TiAdc_SaveToFile.Click += buttonCaliData_TiAdc_SaveToFile_Click;
            // 
            // TabPageTiadcPhaseOffsetGain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(TlpMain);
            Name = "TabPageTiadcPhaseOffsetGain";
            Size = new System.Drawing.Size(1394, 458);
            TlpMain.ResumeLayout(false);
            TlpInfo.ResumeLayout(false);
            TlpParamsInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DgvInfo).EndInit();
            TlpGodParams.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DgvVersionAndDate).EndInit();
            ((System.ComponentModel.ISupportInitialize)DgvGodParams).EndInit();
            TlpControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DgvError).EndInit();
            ((System.ComponentModel.ISupportInitialize)DgvFiltrate).EndInit();
            PlControl.ResumeLayout(false);
            PlControl.PerformLayout();
            GpbAutoCali.ResumeLayout(false);
            GpbAutoCali.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownGainLimit).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownPhaseLimit).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownOverTimeOfMin).EndInit();
            GpbCalcError.ResumeLayout(false);
            GpbCalcError.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpMain;
        private System.Windows.Forms.Panel PlControl;
        private System.Windows.Forms.DataGridView DgvInfo;
        private System.Windows.Forms.TableLayoutPanel TlpInfo;
        private System.Windows.Forms.Button buttonCaliData_TiAdc_Send;
        private System.Windows.Forms.CheckBox checkBoxCaliData_TiAdc_AutoSend;
        private System.Windows.Forms.Button buttonCaliData_TiAdc_LoadFromFile;
        private System.Windows.Forms.Button buttonCaliData_TiAdc_SaveToFile;
        private System.Windows.Forms.DataGridView DgvError;
        private System.Windows.Forms.TextBox textBoxSignalFreqByMHz;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxTotalADCSamplingRadioByGSPS;
        private System.Windows.Forms.Label LblSampleFrequency;
        private System.Windows.Forms.Button buttonCalcError;
        private System.Windows.Forms.Button BtnAutoCali;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDownOverTimeOfMin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label LblPhaseThrehold;
        private System.Windows.Forms.NumericUpDown numericUpDownPhaseLimit;
        private System.Windows.Forms.NumericUpDown numericUpDownGainLimit;
        private System.Windows.Forms.Label LblGainThrehold;
        private System.Windows.Forms.Label LblOffsetThrehold;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox GpbCalcError;
        private System.Windows.Forms.GroupBox GpbAutoCali;
        private System.Windows.Forms.Button BtnReadFromOrigin;
        private System.Windows.Forms.TableLayoutPanel TlpParamsInfo;
        private System.Windows.Forms.DataGridView DgvGodParams;
        private System.Windows.Forms.TableLayoutPanel TlpGodParams;
        private System.Windows.Forms.DataGridView DgvVersionAndDate;
        private System.Windows.Forms.DataGridViewComboBoxColumn GodVersion;
        private System.Windows.Forms.DataGridViewComboBoxColumn ItemVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn CalcDate;
        private System.Windows.Forms.TableLayoutPanel TlpControl;
        private System.Windows.Forms.DataGridView DgvFiltrate;
        private System.Windows.Forms.DataGridViewComboBoxColumn FiltrateSelect;
        private System.Windows.Forms.TextBox tb_Avg;
        private System.Windows.Forms.Label label2;
    }
}
