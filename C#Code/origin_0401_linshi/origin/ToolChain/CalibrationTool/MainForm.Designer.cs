
namespace ScopeX.Hardware.Calibration.Tool
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPageAnalogChannel = new System.Windows.Forms.TabPage();
            tabPageCaliData_Channel1 = new TabPageCaliData_Channel();
            tabPageAcqSync = new System.Windows.Forms.TabPage();
            tabPageCaliData_AcqSync1 = new TabPageCaliData_AcqSync();
            tabPageADC_GainPhaseOffset = new System.Windows.Forms.TabPage();
            tabPageCaliData_TiAdc1 = new TabPageCaliData_TiAdc();
            tabPageMiscCaliData = new System.Windows.Forms.TabPage();
            tabPageMiscData1 = new TabPageMiscData();
            tabPageDbiAnalogChannel = new System.Windows.Forms.TabPage();
            tabPageDbiAnalogParams1 = new TabPageDbiAnalogParams();
            tabPageDbiCoefficientsTable = new System.Windows.Forms.TabPage();
            tabPageDbiCoefficientsTable1 = new TabPageDbiCoefficientsTable();
            tabPageDbiLocalOscillators = new System.Windows.Forms.TabPage();
            tabPageDbiLocalOscillators1 = new TabPageDbiLocalOscillators();
            tabPageCommonCoefficientsTable = new System.Windows.Forms.TabPage();
            tabPageCoefficientsTable1 = new TabPageCoefficientsTable();
            tabPageBatchTask = new System.Windows.Forms.TabPage();
            tabPageBatchTask1 = new TabPageBatchTask();
            tabPageMatlab = new System.Windows.Forms.TabPage();
            tabPageMatlabSourceCode1 = new TabPageMatlabSourceCode();
            tabPageScpi = new System.Windows.Forms.TabPage();
            tabPageScpiCmd1 = new TabPageScpiCmd();
            tabPageReadbackRegister = new System.Windows.Forms.TabPage();
            tabPage_fpgaAllWritedRegisterValueReadback1 = new TabPage_FPGAAllWritedRegisterValueReadback();
            tabPageSoftLA = new System.Windows.Forms.TabPage();
            tabPageSoftla1 = new TabPageSoftLA();
            tabPageSystemTools = new System.Windows.Forms.TabPage();
            tabPageSystemTools2 = new TabPageSystemTools();
            tabPageTiadcPhaseGainOffset = new System.Windows.Forms.TabPage();
            tabPageTiadcPhaseOffsetGain1 = new TabPageTiadcPhaseOffsetGain();
            tabPageAnalogChannelParams = new System.Windows.Forms.TabPage();
            tabPageAnalogChannelParams1 = new TabPageAnalogChannelParams();
            tabPageCoefficientParams = new System.Windows.Forms.TabPage();
            tabPageCoefficientParams1 = new TabPageCoefficientParams();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            toolStripMenuItemLoadRemoteUsingData = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItemLoadRemoteCaliFileData = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItemSaveCaliData2LocalFile = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItemLoadLocalCaliFileDataAndUsing = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItemLoadLocalCaliFileDataAndSave2RemoteFile = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItemWriteCaliData2Flash = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItemLoadCaliDataFromFlash = new System.Windows.Forms.ToolStripMenuItem();
            panel1 = new System.Windows.Forms.Panel();
            BtnLoadFromFile = new System.Windows.Forms.Button();
            labelInstrumentState = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            buttonRefresh = new System.Windows.Forms.Button();
            comboBoxVisaResource = new System.Windows.Forms.ComboBox();
            panel10 = new System.Windows.Forms.Panel();
            buttonGotoExePath = new System.Windows.Forms.Button();
            buttonCtrlRegionCtrl = new System.Windows.Forms.Button();
            buttonWaveRegionCtrl = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            buttonConnectInstrument = new System.Windows.Forms.Button();
            panel2 = new System.Windows.Forms.Panel();
            panel3 = new System.Windows.Forms.Panel();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            waveViewer1 = new WaveViewer();
            folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            timer1AutoRefreshCaliData = new System.Windows.Forms.Timer(components);
            tabControl1.SuspendLayout();
            tabPageAnalogChannel.SuspendLayout();
            tabPageAcqSync.SuspendLayout();
            tabPageADC_GainPhaseOffset.SuspendLayout();
            tabPageMiscCaliData.SuspendLayout();
            tabPageDbiAnalogChannel.SuspendLayout();
            tabPageDbiCoefficientsTable.SuspendLayout();
            tabPageDbiLocalOscillators.SuspendLayout();
            tabPageCommonCoefficientsTable.SuspendLayout();
            tabPageBatchTask.SuspendLayout();
            tabPageMatlab.SuspendLayout();
            tabPageScpi.SuspendLayout();
            tabPageReadbackRegister.SuspendLayout();
            tabPageSoftLA.SuspendLayout();
            tabPageSystemTools.SuspendLayout();
            tabPageTiadcPhaseGainOffset.SuspendLayout();
            tabPageAnalogChannelParams.SuspendLayout();
            tabPageCoefficientParams.SuspendLayout();
            statusStrip1.SuspendLayout();
            panel1.SuspendLayout();
            panel10.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPageAnalogChannel);
            tabControl1.Controls.Add(tabPageAcqSync);
            tabControl1.Controls.Add(tabPageADC_GainPhaseOffset);
            tabControl1.Controls.Add(tabPageMiscCaliData);
            tabControl1.Controls.Add(tabPageDbiAnalogChannel);
            tabControl1.Controls.Add(tabPageDbiCoefficientsTable);
            tabControl1.Controls.Add(tabPageDbiLocalOscillators);
            tabControl1.Controls.Add(tabPageCommonCoefficientsTable);
            tabControl1.Controls.Add(tabPageBatchTask);
            tabControl1.Controls.Add(tabPageMatlab);
            tabControl1.Controls.Add(tabPageScpi);
            tabControl1.Controls.Add(tabPageReadbackRegister);
            tabControl1.Controls.Add(tabPageSoftLA);
            tabControl1.Controls.Add(tabPageSystemTools);
            tabControl1.Controls.Add(tabPageTiadcPhaseGainOffset);
            tabControl1.Controls.Add(tabPageAnalogChannelParams);
            tabControl1.Controls.Add(tabPageCoefficientParams);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(1615, 351);
            tabControl1.TabIndex = 0;
            // 
            // tabPageAnalogChannel
            // 
            tabPageAnalogChannel.Controls.Add(tabPageCaliData_Channel1);
            tabPageAnalogChannel.Location = new System.Drawing.Point(4, 26);
            tabPageAnalogChannel.Name = "tabPageAnalogChannel";
            tabPageAnalogChannel.Padding = new System.Windows.Forms.Padding(3);
            tabPageAnalogChannel.Size = new System.Drawing.Size(1607, 321);
            tabPageAnalogChannel.TabIndex = 0;
            tabPageAnalogChannel.Text = "模拟通道";
            tabPageAnalogChannel.UseVisualStyleBackColor = true;
            // 
            // tabPageCaliData_Channel1
            // 
            tabPageCaliData_Channel1.BackColor = System.Drawing.SystemColors.Control;
            tabPageCaliData_Channel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPageCaliData_Channel1.Location = new System.Drawing.Point(3, 3);
            tabPageCaliData_Channel1.Name = "tabPageCaliData_Channel1";
            tabPageCaliData_Channel1.Size = new System.Drawing.Size(1601, 315);
            tabPageCaliData_Channel1.TabIndex = 1;
            // 
            // tabPageAcqSync
            // 
            tabPageAcqSync.Controls.Add(tabPageCaliData_AcqSync1);
            tabPageAcqSync.Location = new System.Drawing.Point(4, 26);
            tabPageAcqSync.Name = "tabPageAcqSync";
            tabPageAcqSync.Padding = new System.Windows.Forms.Padding(3);
            tabPageAcqSync.Size = new System.Drawing.Size(192, 70);
            tabPageAcqSync.TabIndex = 1;
            tabPageAcqSync.Text = "AcqSync";
            tabPageAcqSync.UseVisualStyleBackColor = true;
            // 
            // tabPageCaliData_AcqSync1
            // 
            tabPageCaliData_AcqSync1.BackColor = System.Drawing.SystemColors.Control;
            tabPageCaliData_AcqSync1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPageCaliData_AcqSync1.Location = new System.Drawing.Point(3, 3);
            tabPageCaliData_AcqSync1.Name = "tabPageCaliData_AcqSync1";
            tabPageCaliData_AcqSync1.Size = new System.Drawing.Size(186, 64);
            tabPageCaliData_AcqSync1.TabIndex = 0;
            // 
            // tabPageADC_GainPhaseOffset
            // 
            tabPageADC_GainPhaseOffset.Controls.Add(tabPageCaliData_TiAdc1);
            tabPageADC_GainPhaseOffset.Location = new System.Drawing.Point(4, 26);
            tabPageADC_GainPhaseOffset.Name = "tabPageADC_GainPhaseOffset";
            tabPageADC_GainPhaseOffset.Padding = new System.Windows.Forms.Padding(3);
            tabPageADC_GainPhaseOffset.Size = new System.Drawing.Size(192, 70);
            tabPageADC_GainPhaseOffset.TabIndex = 2;
            tabPageADC_GainPhaseOffset.Text = "ADC-GainPhaseOffset";
            tabPageADC_GainPhaseOffset.UseVisualStyleBackColor = true;
            // 
            // tabPageCaliData_TiAdc1
            // 
            tabPageCaliData_TiAdc1.BackColor = System.Drawing.SystemColors.Control;
            tabPageCaliData_TiAdc1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPageCaliData_TiAdc1.Location = new System.Drawing.Point(3, 3);
            tabPageCaliData_TiAdc1.Name = "tabPageCaliData_TiAdc1";
            tabPageCaliData_TiAdc1.Size = new System.Drawing.Size(186, 64);
            tabPageCaliData_TiAdc1.TabIndex = 0;
            // 
            // tabPageMiscCaliData
            // 
            tabPageMiscCaliData.Controls.Add(tabPageMiscData1);
            tabPageMiscCaliData.Location = new System.Drawing.Point(4, 26);
            tabPageMiscCaliData.Name = "tabPageMiscCaliData";
            tabPageMiscCaliData.Padding = new System.Windows.Forms.Padding(3);
            tabPageMiscCaliData.Size = new System.Drawing.Size(192, 70);
            tabPageMiscCaliData.TabIndex = 9;
            tabPageMiscCaliData.Text = "其他杂项校准数据";
            tabPageMiscCaliData.UseVisualStyleBackColor = true;
            // 
            // tabPageMiscData1
            // 
            tabPageMiscData1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPageMiscData1.Location = new System.Drawing.Point(3, 3);
            tabPageMiscData1.Name = "tabPageMiscData1";
            tabPageMiscData1.Size = new System.Drawing.Size(186, 64);
            tabPageMiscData1.TabIndex = 6;
            // 
            // tabPageDbiAnalogChannel
            // 
            tabPageDbiAnalogChannel.Controls.Add(tabPageDbiAnalogParams1);
            tabPageDbiAnalogChannel.Location = new System.Drawing.Point(4, 26);
            tabPageDbiAnalogChannel.Name = "tabPageDbiAnalogChannel";
            tabPageDbiAnalogChannel.Padding = new System.Windows.Forms.Padding(3);
            tabPageDbiAnalogChannel.Size = new System.Drawing.Size(192, 70);
            tabPageDbiAnalogChannel.TabIndex = 11;
            tabPageDbiAnalogChannel.Text = "Dbi模拟通道校准数据";
            tabPageDbiAnalogChannel.UseVisualStyleBackColor = true;
            // 
            // tabPageDbiAnalogParams1
            // 
            tabPageDbiAnalogParams1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPageDbiAnalogParams1.Location = new System.Drawing.Point(3, 3);
            tabPageDbiAnalogParams1.Name = "tabPageDbiAnalogParams1";
            tabPageDbiAnalogParams1.Size = new System.Drawing.Size(186, 64);
            tabPageDbiAnalogParams1.TabIndex = 0;
            // 
            // tabPageDbiCoefficientsTable
            // 
            tabPageDbiCoefficientsTable.Controls.Add(tabPageDbiCoefficientsTable1);
            tabPageDbiCoefficientsTable.Location = new System.Drawing.Point(4, 26);
            tabPageDbiCoefficientsTable.Name = "tabPageDbiCoefficientsTable";
            tabPageDbiCoefficientsTable.Padding = new System.Windows.Forms.Padding(3);
            tabPageDbiCoefficientsTable.Size = new System.Drawing.Size(192, 70);
            tabPageDbiCoefficientsTable.TabIndex = 12;
            tabPageDbiCoefficientsTable.Text = "Dbi各种系数维护";
            tabPageDbiCoefficientsTable.UseVisualStyleBackColor = true;
            // 
            // tabPageDbiCoefficientsTable1
            // 
            tabPageDbiCoefficientsTable1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPageDbiCoefficientsTable1.Location = new System.Drawing.Point(3, 3);
            tabPageDbiCoefficientsTable1.Name = "tabPageDbiCoefficientsTable1";
            tabPageDbiCoefficientsTable1.Size = new System.Drawing.Size(186, 64);
            tabPageDbiCoefficientsTable1.TabIndex = 0;
            // 
            // tabPageDbiLocalOscillators
            // 
            tabPageDbiLocalOscillators.Controls.Add(tabPageDbiLocalOscillators1);
            tabPageDbiLocalOscillators.Location = new System.Drawing.Point(4, 26);
            tabPageDbiLocalOscillators.Name = "tabPageDbiLocalOscillators";
            tabPageDbiLocalOscillators.Padding = new System.Windows.Forms.Padding(3);
            tabPageDbiLocalOscillators.Size = new System.Drawing.Size(192, 70);
            tabPageDbiLocalOscillators.TabIndex = 15;
            tabPageDbiLocalOscillators.Text = "Dbi本振";
            tabPageDbiLocalOscillators.UseVisualStyleBackColor = true;
            // 
            // tabPageDbiLocalOscillators1
            // 
            tabPageDbiLocalOscillators1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPageDbiLocalOscillators1.Location = new System.Drawing.Point(3, 3);
            tabPageDbiLocalOscillators1.Name = "tabPageDbiLocalOscillators1";
            tabPageDbiLocalOscillators1.Size = new System.Drawing.Size(186, 64);
            tabPageDbiLocalOscillators1.TabIndex = 0;
            // 
            // tabPageCommonCoefficientsTable
            // 
            tabPageCommonCoefficientsTable.Controls.Add(tabPageCoefficientsTable1);
            tabPageCommonCoefficientsTable.Location = new System.Drawing.Point(4, 26);
            tabPageCommonCoefficientsTable.Name = "tabPageCommonCoefficientsTable";
            tabPageCommonCoefficientsTable.Padding = new System.Windows.Forms.Padding(3);
            tabPageCommonCoefficientsTable.Size = new System.Drawing.Size(192, 70);
            tabPageCommonCoefficientsTable.TabIndex = 8;
            tabPageCommonCoefficientsTable.Text = "系数表 维护";
            // 
            // tabPageCoefficientsTable1
            // 
            tabPageCoefficientsTable1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPageCoefficientsTable1.Location = new System.Drawing.Point(3, 3);
            tabPageCoefficientsTable1.Name = "tabPageCoefficientsTable1";
            tabPageCoefficientsTable1.Size = new System.Drawing.Size(186, 64);
            tabPageCoefficientsTable1.TabIndex = 0;
            // 
            // tabPageBatchTask
            // 
            tabPageBatchTask.Controls.Add(tabPageBatchTask1);
            tabPageBatchTask.Location = new System.Drawing.Point(4, 26);
            tabPageBatchTask.Name = "tabPageBatchTask";
            tabPageBatchTask.Padding = new System.Windows.Forms.Padding(3);
            tabPageBatchTask.Size = new System.Drawing.Size(192, 70);
            tabPageBatchTask.TabIndex = 3;
            tabPageBatchTask.Text = "批任务";
            tabPageBatchTask.UseVisualStyleBackColor = true;
            // 
            // tabPageBatchTask1
            // 
            tabPageBatchTask1.BackColor = System.Drawing.SystemColors.Control;
            tabPageBatchTask1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPageBatchTask1.Location = new System.Drawing.Point(3, 3);
            tabPageBatchTask1.Name = "tabPageBatchTask1";
            tabPageBatchTask1.Size = new System.Drawing.Size(186, 64);
            tabPageBatchTask1.TabIndex = 0;
            // 
            // tabPageMatlab
            // 
            tabPageMatlab.Controls.Add(tabPageMatlabSourceCode1);
            tabPageMatlab.Location = new System.Drawing.Point(4, 26);
            tabPageMatlab.Name = "tabPageMatlab";
            tabPageMatlab.Padding = new System.Windows.Forms.Padding(3);
            tabPageMatlab.Size = new System.Drawing.Size(192, 70);
            tabPageMatlab.TabIndex = 4;
            tabPageMatlab.Text = "MATLAB";
            tabPageMatlab.UseVisualStyleBackColor = true;
            // 
            // tabPageMatlabSourceCode1
            // 
            tabPageMatlabSourceCode1.BackColor = System.Drawing.SystemColors.Control;
            tabPageMatlabSourceCode1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPageMatlabSourceCode1.Location = new System.Drawing.Point(3, 3);
            tabPageMatlabSourceCode1.Name = "tabPageMatlabSourceCode1";
            tabPageMatlabSourceCode1.Size = new System.Drawing.Size(186, 64);
            tabPageMatlabSourceCode1.TabIndex = 0;
            // 
            // tabPageScpi
            // 
            tabPageScpi.Controls.Add(tabPageScpiCmd1);
            tabPageScpi.Location = new System.Drawing.Point(4, 26);
            tabPageScpi.Name = "tabPageScpi";
            tabPageScpi.Padding = new System.Windows.Forms.Padding(3);
            tabPageScpi.Size = new System.Drawing.Size(192, 70);
            tabPageScpi.TabIndex = 5;
            tabPageScpi.Text = "Scpi命令控制台";
            tabPageScpi.UseVisualStyleBackColor = true;
            // 
            // tabPageScpiCmd1
            // 
            tabPageScpiCmd1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPageScpiCmd1.Location = new System.Drawing.Point(3, 3);
            tabPageScpiCmd1.Name = "tabPageScpiCmd1";
            tabPageScpiCmd1.Size = new System.Drawing.Size(186, 64);
            tabPageScpiCmd1.TabIndex = 0;
            // 
            // tabPageReadbackRegister
            // 
            tabPageReadbackRegister.Controls.Add(tabPage_fpgaAllWritedRegisterValueReadback1);
            tabPageReadbackRegister.Location = new System.Drawing.Point(4, 26);
            tabPageReadbackRegister.Name = "tabPageReadbackRegister";
            tabPageReadbackRegister.Padding = new System.Windows.Forms.Padding(3);
            tabPageReadbackRegister.Size = new System.Drawing.Size(192, 70);
            tabPageReadbackRegister.TabIndex = 7;
            tabPageReadbackRegister.Text = "读取所有写寄存器的值";
            tabPageReadbackRegister.UseVisualStyleBackColor = true;
            // 
            // tabPage_fpgaAllWritedRegisterValueReadback1
            // 
            tabPage_fpgaAllWritedRegisterValueReadback1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPage_fpgaAllWritedRegisterValueReadback1.Location = new System.Drawing.Point(3, 3);
            tabPage_fpgaAllWritedRegisterValueReadback1.Name = "tabPage_fpgaAllWritedRegisterValueReadback1";
            tabPage_fpgaAllWritedRegisterValueReadback1.Size = new System.Drawing.Size(186, 64);
            tabPage_fpgaAllWritedRegisterValueReadback1.TabIndex = 0;
            // 
            // tabPageSoftLA
            // 
            tabPageSoftLA.Controls.Add(tabPageSoftla1);
            tabPageSoftLA.Location = new System.Drawing.Point(4, 26);
            tabPageSoftLA.Name = "tabPageSoftLA";
            tabPageSoftLA.Padding = new System.Windows.Forms.Padding(3);
            tabPageSoftLA.Size = new System.Drawing.Size(192, 70);
            tabPageSoftLA.TabIndex = 10;
            tabPageSoftLA.Text = "SoftLA";
            tabPageSoftLA.UseVisualStyleBackColor = true;
            // 
            // tabPageSoftla1
            // 
            tabPageSoftla1.Location = new System.Drawing.Point(3, 3);
            tabPageSoftla1.Name = "tabPageSoftla1";
            tabPageSoftla1.Size = new System.Drawing.Size(1194, 315);
            tabPageSoftla1.TabIndex = 0;
            // 
            // tabPageSystemTools
            // 
            tabPageSystemTools.Controls.Add(tabPageSystemTools2);
            tabPageSystemTools.Location = new System.Drawing.Point(4, 26);
            tabPageSystemTools.Name = "tabPageSystemTools";
            tabPageSystemTools.Padding = new System.Windows.Forms.Padding(3);
            tabPageSystemTools.Size = new System.Drawing.Size(1607, 321);
            tabPageSystemTools.TabIndex = 16;
            tabPageSystemTools.Text = "系统工具集";
            tabPageSystemTools.UseVisualStyleBackColor = true;
            // 
            // tabPageSystemTools2
            // 
            tabPageSystemTools2.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPageSystemTools2.Location = new System.Drawing.Point(3, 3);
            tabPageSystemTools2.Name = "tabPageSystemTools2";
            tabPageSystemTools2.Size = new System.Drawing.Size(1601, 315);
            tabPageSystemTools2.TabIndex = 0;
            // 
            // tabPageTiadcPhaseGainOffset
            // 
            tabPageTiadcPhaseGainOffset.Controls.Add(tabPageTiadcPhaseOffsetGain1);
            tabPageTiadcPhaseGainOffset.Location = new System.Drawing.Point(4, 26);
            tabPageTiadcPhaseGainOffset.Name = "tabPageTiadcPhaseGainOffset";
            tabPageTiadcPhaseGainOffset.Size = new System.Drawing.Size(1607, 321);
            tabPageTiadcPhaseGainOffset.TabIndex = 19;
            tabPageTiadcPhaseGainOffset.Text = "Tiadc-GainPhaseOffset";
            tabPageTiadcPhaseGainOffset.UseVisualStyleBackColor = true;
            // 
            // tabPageTiadcPhaseOffsetGain1
            // 
            tabPageTiadcPhaseOffsetGain1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPageTiadcPhaseOffsetGain1.Location = new System.Drawing.Point(0, 0);
            tabPageTiadcPhaseOffsetGain1.Name = "tabPageTiadcPhaseOffsetGain1";
            tabPageTiadcPhaseOffsetGain1.Size = new System.Drawing.Size(1607, 321);
            tabPageTiadcPhaseOffsetGain1.TabIndex = 0;
            // 
            // tabPageAnalogChannelParams
            // 
            tabPageAnalogChannelParams.Controls.Add(tabPageAnalogChannelParams1);
            tabPageAnalogChannelParams.Location = new System.Drawing.Point(4, 26);
            tabPageAnalogChannelParams.Name = "tabPageAnalogChannelParams";
            tabPageAnalogChannelParams.Size = new System.Drawing.Size(1607, 321);
            tabPageAnalogChannelParams.TabIndex = 20;
            tabPageAnalogChannelParams.Text = "AnalogChannelParams";
            tabPageAnalogChannelParams.UseVisualStyleBackColor = true;
            // 
            // tabPageAnalogChannelParams1
            // 
            tabPageAnalogChannelParams1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPageAnalogChannelParams1.Location = new System.Drawing.Point(0, 0);
            tabPageAnalogChannelParams1.Name = "tabPageAnalogChannelParams1";
            tabPageAnalogChannelParams1.Size = new System.Drawing.Size(1607, 321);
            tabPageAnalogChannelParams1.TabIndex = 0;
            // 
            // tabPageCoefficientParams
            // 
            tabPageCoefficientParams.Controls.Add(tabPageCoefficientParams1);
            tabPageCoefficientParams.Location = new System.Drawing.Point(4, 26);
            tabPageCoefficientParams.Name = "tabPageCoefficientParams";
            tabPageCoefficientParams.Size = new System.Drawing.Size(1607, 321);
            tabPageCoefficientParams.TabIndex = 21;
            tabPageCoefficientParams.Text = "Coefficients";
            tabPageCoefficientParams.UseVisualStyleBackColor = true;
            // 
            // tabPageCoefficientParams1
            // 
            tabPageCoefficientParams1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabPageCoefficientParams1.Location = new System.Drawing.Point(0, 0);
            tabPageCoefficientParams1.Name = "tabPageCoefficientParams1";
            tabPageCoefficientParams1.Size = new System.Drawing.Size(1607, 321);
            tabPageCoefficientParams1.TabIndex = 0;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1, toolStripProgressBar1, toolStripDropDownButton1 });
            statusStrip1.Location = new System.Drawing.Point(0, 679);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(1615, 23);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(131, 18);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new System.Drawing.Size(100, 17);
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripMenuItemLoadRemoteUsingData, toolStripMenuItemLoadRemoteCaliFileData, toolStripMenuItemSaveCaliData2LocalFile, toolStripMenuItemLoadLocalCaliFileDataAndUsing, toolStripMenuItemLoadLocalCaliFileDataAndSave2RemoteFile, toolStripMenuItemWriteCaliData2Flash, toolStripMenuItemLoadCaliDataFromFlash });
            toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new System.Drawing.Size(93, 21);
            toolStripDropDownButton1.Text = "校准数据操作";
            // 
            // toolStripMenuItemLoadRemoteUsingData
            // 
            toolStripMenuItemLoadRemoteUsingData.Name = "toolStripMenuItemLoadRemoteUsingData";
            toolStripMenuItemLoadRemoteUsingData.Size = new System.Drawing.Size(268, 22);
            toolStripMenuItemLoadRemoteUsingData.Text = "装载远端运行态校准数据";
            // 
            // toolStripMenuItemLoadRemoteCaliFileData
            // 
            toolStripMenuItemLoadRemoteCaliFileData.Name = "toolStripMenuItemLoadRemoteCaliFileData";
            toolStripMenuItemLoadRemoteCaliFileData.Size = new System.Drawing.Size(268, 22);
            toolStripMenuItemLoadRemoteCaliFileData.Text = "装载远端校准文件数据";
            // 
            // toolStripMenuItemSaveCaliData2LocalFile
            // 
            toolStripMenuItemSaveCaliData2LocalFile.Name = "toolStripMenuItemSaveCaliData2LocalFile";
            toolStripMenuItemSaveCaliData2LocalFile.Size = new System.Drawing.Size(268, 22);
            toolStripMenuItemSaveCaliData2LocalFile.Text = "保存校准数据到本地";
            // 
            // toolStripMenuItemLoadLocalCaliFileDataAndUsing
            // 
            toolStripMenuItemLoadLocalCaliFileDataAndUsing.Name = "toolStripMenuItemLoadLocalCaliFileDataAndUsing";
            toolStripMenuItemLoadLocalCaliFileDataAndUsing.Size = new System.Drawing.Size(268, 22);
            toolStripMenuItemLoadLocalCaliFileDataAndUsing.Text = "装载本地校准数据并生效";
            // 
            // toolStripMenuItemLoadLocalCaliFileDataAndSave2RemoteFile
            // 
            toolStripMenuItemLoadLocalCaliFileDataAndSave2RemoteFile.Name = "toolStripMenuItemLoadLocalCaliFileDataAndSave2RemoteFile";
            toolStripMenuItemLoadLocalCaliFileDataAndSave2RemoteFile.Size = new System.Drawing.Size(268, 22);
            toolStripMenuItemLoadLocalCaliFileDataAndSave2RemoteFile.Text = "装载本地校准数据并保存到远端文件";
            // 
            // toolStripMenuItemWriteCaliData2Flash
            // 
            toolStripMenuItemWriteCaliData2Flash.Name = "toolStripMenuItemWriteCaliData2Flash";
            toolStripMenuItemWriteCaliData2Flash.Size = new System.Drawing.Size(268, 22);
            toolStripMenuItemWriteCaliData2Flash.Text = "烧写校准数据到Flash";
            // 
            // toolStripMenuItemLoadCaliDataFromFlash
            // 
            toolStripMenuItemLoadCaliDataFromFlash.Name = "toolStripMenuItemLoadCaliDataFromFlash";
            toolStripMenuItemLoadCaliDataFromFlash.Size = new System.Drawing.Size(268, 22);
            toolStripMenuItemLoadCaliDataFromFlash.Text = "从FLASH装载校准数据";
            // 
            // panel1
            // 
            panel1.Controls.Add(BtnLoadFromFile);
            panel1.Controls.Add(labelInstrumentState);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(buttonRefresh);
            panel1.Controls.Add(comboBoxVisaResource);
            panel1.Controls.Add(panel10);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(buttonConnectInstrument);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(1615, 35);
            panel1.TabIndex = 2;
            // 
            // BtnLoadFromFile
            // 
            BtnLoadFromFile.Location = new System.Drawing.Point(1229, 4);
            BtnLoadFromFile.Name = "BtnLoadFromFile";
            BtnLoadFromFile.Size = new System.Drawing.Size(121, 28);
            BtnLoadFromFile.TabIndex = 13;
            BtnLoadFromFile.Text = "从文件中装载数据";
            BtnLoadFromFile.UseVisualStyleBackColor = true;
            // 
            // labelInstrumentState
            // 
            labelInstrumentState.AutoSize = true;
            labelInstrumentState.BackColor = System.Drawing.Color.Red;
            labelInstrumentState.Location = new System.Drawing.Point(761, 11);
            labelInstrumentState.Name = "labelInstrumentState";
            labelInstrumentState.Size = new System.Drawing.Size(17, 17);
            labelInstrumentState.TabIndex = 12;
            labelInstrumentState.Text = "×";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(668, 10);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(92, 17);
            label2.TabIndex = 11;
            label2.Text = "仪器连接状态：";
            // 
            // buttonRefresh
            // 
            buttonRefresh.Location = new System.Drawing.Point(473, 5);
            buttonRefresh.Name = "buttonRefresh";
            buttonRefresh.Size = new System.Drawing.Size(70, 28);
            buttonRefresh.TabIndex = 10;
            buttonRefresh.Text = "刷新";
            buttonRefresh.UseVisualStyleBackColor = true;
            buttonRefresh.Click += buttonRefresh_Click;
            // 
            // comboBoxVisaResource
            // 
            comboBoxVisaResource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxVisaResource.FormattingEnabled = true;
            comboBoxVisaResource.Location = new System.Drawing.Point(75, 6);
            comboBoxVisaResource.Name = "comboBoxVisaResource";
            comboBoxVisaResource.Size = new System.Drawing.Size(394, 25);
            comboBoxVisaResource.TabIndex = 9;
            // 
            // panel10
            // 
            panel10.Controls.Add(buttonGotoExePath);
            panel10.Controls.Add(buttonCtrlRegionCtrl);
            panel10.Controls.Add(buttonWaveRegionCtrl);
            panel10.Dock = System.Windows.Forms.DockStyle.Right;
            panel10.Location = new System.Drawing.Point(1364, 0);
            panel10.Name = "panel10";
            panel10.Size = new System.Drawing.Size(251, 35);
            panel10.TabIndex = 4;
            // 
            // buttonGotoExePath
            // 
            buttonGotoExePath.Location = new System.Drawing.Point(5, 4);
            buttonGotoExePath.Name = "buttonGotoExePath";
            buttonGotoExePath.Size = new System.Drawing.Size(55, 28);
            buttonGotoExePath.TabIndex = 4;
            buttonGotoExePath.Text = "转到...";
            buttonGotoExePath.UseVisualStyleBackColor = true;
            buttonGotoExePath.Click += buttonGotoExePath_Click;
            // 
            // buttonCtrlRegionCtrl
            // 
            buttonCtrlRegionCtrl.Location = new System.Drawing.Point(165, 4);
            buttonCtrlRegionCtrl.Name = "buttonCtrlRegionCtrl";
            buttonCtrlRegionCtrl.Size = new System.Drawing.Size(78, 28);
            buttonCtrlRegionCtrl.TabIndex = 1;
            buttonCtrlRegionCtrl.Text = "隐藏校准区";
            buttonCtrlRegionCtrl.UseVisualStyleBackColor = true;
            buttonCtrlRegionCtrl.Click += buttonCtrlRegionCtrl_Click;
            // 
            // buttonWaveRegionCtrl
            // 
            buttonWaveRegionCtrl.Location = new System.Drawing.Point(67, 4);
            buttonWaveRegionCtrl.Name = "buttonWaveRegionCtrl";
            buttonWaveRegionCtrl.Size = new System.Drawing.Size(89, 28);
            buttonWaveRegionCtrl.TabIndex = 0;
            buttonWaveRegionCtrl.Text = "隐藏波形区";
            buttonWaveRegionCtrl.UseVisualStyleBackColor = true;
            buttonWaveRegionCtrl.Click += buttonWaveRegionCtrl_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(68, 17);
            label1.TabIndex = 1;
            label1.Text = "仪器地址：";
            // 
            // buttonConnectInstrument
            // 
            buttonConnectInstrument.Location = new System.Drawing.Point(549, 4);
            buttonConnectInstrument.Name = "buttonConnectInstrument";
            buttonConnectInstrument.Size = new System.Drawing.Size(85, 28);
            buttonConnectInstrument.TabIndex = 0;
            buttonConnectInstrument.Tag = "0";
            buttonConnectInstrument.Text = "连接仪器";
            buttonConnectInstrument.UseVisualStyleBackColor = true;
            buttonConnectInstrument.Click += buttonConnectInstrument_Click;
            // 
            // panel2
            // 
            panel2.Controls.Add(panel3);
            panel2.Controls.Add(panel1);
            panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            panel2.Location = new System.Drawing.Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(1615, 679);
            panel2.TabIndex = 3;
            // 
            // panel3
            // 
            panel3.Controls.Add(splitContainer1);
            panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            panel3.Location = new System.Drawing.Point(0, 35);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(1615, 644);
            panel3.TabIndex = 3;
            // 
            // splitContainer1
            // 
            splitContainer1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.ForeColor = System.Drawing.SystemColors.ControlText;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            splitContainer1.Panel1.Controls.Add(waveViewer1);
            splitContainer1.Panel1MinSize = 10;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            splitContainer1.Panel2.Controls.Add(tabControl1);
            splitContainer1.Panel2MinSize = 10;
            splitContainer1.Size = new System.Drawing.Size(1615, 644);
            splitContainer1.SplitterDistance = 289;
            splitContainer1.TabIndex = 1;
            // 
            // waveViewer1
            // 
            waveViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            waveViewer1.Location = new System.Drawing.Point(0, 0);
            waveViewer1.Name = "waveViewer1";
            waveViewer1.Size = new System.Drawing.Size(1615, 289);
            waveViewer1.TabIndex = 0;
            // 
            // folderBrowserDialog1
            // 
            folderBrowserDialog1.Description = "选择保存的目录";
            // 
            // timer1AutoRefreshCaliData
            // 
            timer1AutoRefreshCaliData.Interval = 500;
            timer1AutoRefreshCaliData.Tick += timer1AutoRefreshCaliData_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1615, 702);
            Controls.Add(panel2);
            Controls.Add(statusStrip1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "DsoFactoryIntegrativeTool";
            FormClosing += MainForm_FormClosing;
            tabControl1.ResumeLayout(false);
            tabPageAnalogChannel.ResumeLayout(false);
            tabPageAcqSync.ResumeLayout(false);
            tabPageADC_GainPhaseOffset.ResumeLayout(false);
            tabPageMiscCaliData.ResumeLayout(false);
            tabPageDbiAnalogChannel.ResumeLayout(false);
            tabPageDbiCoefficientsTable.ResumeLayout(false);
            tabPageDbiLocalOscillators.ResumeLayout(false);
            tabPageCommonCoefficientsTable.ResumeLayout(false);
            tabPageBatchTask.ResumeLayout(false);
            tabPageMatlab.ResumeLayout(false);
            tabPageScpi.ResumeLayout(false);
            tabPageReadbackRegister.ResumeLayout(false);
            tabPageSoftLA.ResumeLayout(false);
            tabPageSystemTools.ResumeLayout(false);
            tabPageTiadcPhaseGainOffset.ResumeLayout(false);
            tabPageAnalogChannelParams.ResumeLayout(false);
            tabPageCoefficientParams.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel10.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel3.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageAnalogChannel;
        private System.Windows.Forms.TabPage tabPageAcqSync;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonConnectInstrument;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Button buttonCtrlRegionCtrl;
        private System.Windows.Forms.Button buttonWaveRegionCtrl;
        private WaveViewer waveViewer1;
        private System.Windows.Forms.TabPage tabPageBatchTask;
        private System.Windows.Forms.TabPage tabPageMatlab;
        private System.Windows.Forms.TabPage tabPageScpi;
        private TabPageCaliData_AcqSync tabPageCaliData_AcqSync1;
        private System.Windows.Forms.TabPage tabPageADC_GainPhaseOffset;
        private TabPageCaliData_TiAdc tabPageCaliData_TiAdc1;
        private TabPageBatchTask tabPageBatchTask1;
        private TabPageMatlabSourceCode tabPageMatlabSourceCode1;
        private TabPageScpiCmd tabPageScpiCmd1;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.ComboBox comboBoxVisaResource;
        private System.Windows.Forms.Button buttonGotoExePath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TabPage tabPageReadbackRegister;
        private TabPage_FPGAAllWritedRegisterValueReadback tabPage_fpgaAllWritedRegisterValueReadback1;
        private TabPageCaliData_Channel tabPageCaliData_Channel1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLoadRemoteUsingData;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLoadRemoteCaliFileData;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSaveCaliData2LocalFile;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLoadLocalCaliFileDataAndUsing;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLoadLocalCaliFileDataAndSave2RemoteFile;
        private System.Windows.Forms.TabPage tabPageCommonCoefficientsTable;
        private TabPageCoefficientsTable tabPageCoefficientsTable1;
        private System.Windows.Forms.TabPage tabPageMiscCaliData;
        private TabPageMiscData tabPageMiscData1;
        private System.Windows.Forms.TabPage tabPageSoftLA;
        private TabPageSoftLA tabPageSoftla1;
        private System.Windows.Forms.TabPage tabPageDbiAnalogChannel;
        private TabPageDbiAnalogParams tabPageDbiAnalogParams1;
        private System.Windows.Forms.TabPage tabPageDbiCoefficientsTable;
        private TabPageDbiCoefficientsTable tabPageDbiCoefficientsTable1;
        private System.Windows.Forms.TabPage tabPageDbiLocalOscillators;
        private TabPageDbiLocalOscillators tabPageDbiLocalOscillators1;
        private System.Windows.Forms.TabPage tabPageSystemTools;
        private TabPageSystemTools tabPageSystemTools1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemWriteCaliData2Flash;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLoadCaliDataFromFlash;
        private TabPageSystemTools tabPageSystemTools2;
        private System.Windows.Forms.Label labelInstrumentState;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer timer1AutoRefreshCaliData;
        private System.Windows.Forms.Button BtnLoadFromFile;
        private System.Windows.Forms.TabPage tabPageTiadcPhaseGainOffset;
        private TabPageTiadcPhaseOffsetGain tabPageTiadcPhaseOffsetGain1;
        private System.Windows.Forms.TabPage tabPageAnalogChannelParams;
        private TabPageAnalogChannelParams tabPageAnalogChannelParams1;
        private System.Windows.Forms.TabPage tabPageCoefficientParams;
        private TabPageCoefficientParams tabPageCoefficientParams1;
    }
}

