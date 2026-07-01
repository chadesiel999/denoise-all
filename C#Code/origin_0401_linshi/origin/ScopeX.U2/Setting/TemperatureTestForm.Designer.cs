namespace ScopeX.U2
{
    partial class TemperatureTestForm
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
            try
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }
            catch
            {

            }

        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt5 = new UserControls.DefaultHighlightPrompt();
            TlpFigure = new System.Windows.Forms.TableLayoutPanel();
            ScottPlotFormControl = new ScottPlot.FormsPlot();
            panelHead = new System.Windows.Forms.Panel();
            BtnHardDiskTargetTemperature = new UserControls.ScopeXTextBox();
            LblHardDiskTargetTemp = new UserControls.ScopeXLabel();
            BtnTargetTemperature = new UserControls.ScopeXTextBox();
            LblChnnlTargetTemp = new UserControls.ScopeXLabel();
            BtnDiffParam = new UserControls.ScopeXTextBox();
            ScopeXLabel3 = new UserControls.ScopeXLabel();
            BtnIntegralParam = new UserControls.ScopeXTextBox();
            ScopeXLabel2 = new UserControls.ScopeXLabel();
            BtnRadioParam = new UserControls.ScopeXTextBox();
            ScopeXLabel1 = new UserControls.ScopeXLabel();
            TlpFigure.SuspendLayout();
            panelHead.SuspendLayout();
            SuspendLayout();
            // 
            // TlpFigure
            // 
            TlpFigure.ColumnCount = 1;
            TlpFigure.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpFigure.Controls.Add(ScottPlotFormControl, 0, 1);
            TlpFigure.Controls.Add(panelHead, 0, 0);
            TlpFigure.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpFigure.Location = new System.Drawing.Point(1, 46);
            TlpFigure.Name = "TlpFigure";
            TlpFigure.RowCount = 2;
            TlpFigure.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            TlpFigure.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpFigure.Size = new System.Drawing.Size(1798, 806);
            TlpFigure.TabIndex = 5;
            // 
            // ScottPlotFormControl
            // 
            ScottPlotFormControl.BackColor = System.Drawing.Color.Black;
            ScottPlotFormControl.Dock = System.Windows.Forms.DockStyle.Fill;
            ScottPlotFormControl.Location = new System.Drawing.Point(4, 204);
            ScottPlotFormControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            ScottPlotFormControl.Name = "ScottPlotFormControl";
            ScottPlotFormControl.Size = new System.Drawing.Size(1790, 598);
            ScottPlotFormControl.TabIndex = 1;
            // 
            // panelHead
            // 
            panelHead.Controls.Add(BtnHardDiskTargetTemperature);
            panelHead.Controls.Add(LblHardDiskTargetTemp);
            panelHead.Controls.Add(BtnTargetTemperature);
            panelHead.Controls.Add(LblChnnlTargetTemp);
            panelHead.Controls.Add(BtnDiffParam);
            panelHead.Controls.Add(ScopeXLabel3);
            panelHead.Controls.Add(BtnIntegralParam);
            panelHead.Controls.Add(ScopeXLabel2);
            panelHead.Controls.Add(BtnRadioParam);
            panelHead.Controls.Add(ScopeXLabel1);
            panelHead.Dock = System.Windows.Forms.DockStyle.Fill;
            panelHead.Location = new System.Drawing.Point(3, 3);
            panelHead.Name = "panelHead";
            panelHead.Size = new System.Drawing.Size(1792, 194);
            panelHead.TabIndex = 2;
            // 
            // BtnHardDiskTargetTemperature
            // 
            BtnHardDiskTargetTemperature.AcceptsTab = false;
            BtnHardDiskTargetTemperature.AutoShowKeyBoard = false;
            BtnHardDiskTargetTemperature.AutoSize = false;
            BtnHardDiskTargetTemperature.BackColor = System.Drawing.Color.SaddleBrown;
            BtnHardDiskTargetTemperature.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnHardDiskTargetTemperature.BorderThickness = 0;
            BtnHardDiskTargetTemperature.ClickedBorderColor = System.Drawing.Color.White;
            BtnHardDiskTargetTemperature.CornerRadius = 0;
            BtnHardDiskTargetTemperature.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnHardDiskTargetTemperature.Enabled = true;
            BtnHardDiskTargetTemperature.EnbleSelectBorder = true;
            BtnHardDiskTargetTemperature.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnHardDiskTargetTemperature.ForeColor = System.Drawing.Color.Black;
            BtnHardDiskTargetTemperature.Height = 30;
            BtnHardDiskTargetTemperature.HideSelection = true;
            BtnHardDiskTargetTemperature.IsFocusClicked = false;
            BtnHardDiskTargetTemperature.KeyboardVerify = null;
            BtnHardDiskTargetTemperature.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnHardDiskTargetTemperature.Location = new System.Drawing.Point(450, 25);
            BtnHardDiskTargetTemperature.MaxLength = 32767;
            BtnHardDiskTargetTemperature.Modified = false;
            BtnHardDiskTargetTemperature.MouseEnterState = false;
            BtnHardDiskTargetTemperature.Multiline = false;
            BtnHardDiskTargetTemperature.Name = "BtnHardDiskTargetTemperature";
            BtnHardDiskTargetTemperature.ProcessCmdKeyFunc = null;
            BtnHardDiskTargetTemperature.ReadOnly = false;
            BtnHardDiskTargetTemperature.SelectedColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnHardDiskTargetTemperature.SelectedText = "";
            BtnHardDiskTargetTemperature.SelectionLength = 0;
            BtnHardDiskTargetTemperature.SelectionStart = 0;
            BtnHardDiskTargetTemperature.ShortcutsEnabled = false;
            BtnHardDiskTargetTemperature.Size = new System.Drawing.Size(90, 30);
            BtnHardDiskTargetTemperature.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnHardDiskTargetTemperature.StylizeFlag = false;
            BtnHardDiskTargetTemperature.TabIndex = 55;
            BtnHardDiskTargetTemperature.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            BtnHardDiskTargetTemperature.UseSystemPasswordChar = false;
            BtnHardDiskTargetTemperature.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            BtnHardDiskTargetTemperature.WordWrap = true;
            //BtnHardDiskTargetTemperature.Click += BtnHardDiskTargetTemperature_Click;
            // 
            // LblHardDiskTargetTemp
            // 
            LblHardDiskTargetTemp.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblHardDiskTargetTemp.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblHardDiskTargetTemp.BorderThickness = 0;
            LblHardDiskTargetTemp.CornerRadius = 0;
            LblHardDiskTargetTemp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblHardDiskTargetTemp.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblHardDiskTargetTemp.HighlightPrompt = defaultHighlightPrompt1;
            LblHardDiskTargetTemp.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblHardDiskTargetTemp.Location = new System.Drawing.Point(450, 1);
            LblHardDiskTargetTemp.MultyLineFlag = false;
            LblHardDiskTargetTemp.Name = "LblHardDiskTargetTemp";
            LblHardDiskTargetTemp.Size = new System.Drawing.Size(101, 22);
            LblHardDiskTargetTemp.StyleFlags = UserControls.Style.StyleFlag.None;
            LblHardDiskTargetTemp.StylizeFlag = true;
            LblHardDiskTargetTemp.TabIndex = 54;
            LblHardDiskTargetTemp.TabStop = false;
            LblHardDiskTargetTemp.Text = "硬盘目标温度";
            LblHardDiskTargetTemp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblHardDiskTargetTemp.Token = null;
            // 
            // BtnTargetTemperature
            // 
            BtnTargetTemperature.AcceptsTab = false;
            BtnTargetTemperature.AutoShowKeyBoard = false;
            BtnTargetTemperature.AutoSize = false;
            BtnTargetTemperature.BackColor = System.Drawing.Color.Red;
            BtnTargetTemperature.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTargetTemperature.BorderThickness = 0;
            BtnTargetTemperature.ClickedBorderColor = System.Drawing.Color.White;
            BtnTargetTemperature.CornerRadius = 0;
            BtnTargetTemperature.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnTargetTemperature.Enabled = true;
            BtnTargetTemperature.EnbleSelectBorder = true;
            BtnTargetTemperature.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnTargetTemperature.ForeColor = System.Drawing.Color.Black;
            BtnTargetTemperature.Height = 30;
            BtnTargetTemperature.HideSelection = true;
            BtnTargetTemperature.IsFocusClicked = false;
            BtnTargetTemperature.KeyboardVerify = null;
            BtnTargetTemperature.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnTargetTemperature.Location = new System.Drawing.Point(339, 25);
            BtnTargetTemperature.MaxLength = 32767;
            BtnTargetTemperature.Modified = false;
            BtnTargetTemperature.MouseEnterState = false;
            BtnTargetTemperature.Multiline = false;
            BtnTargetTemperature.Name = "BtnTargetTemperature";
            BtnTargetTemperature.ProcessCmdKeyFunc = null;
            BtnTargetTemperature.ReadOnly = false;
            BtnTargetTemperature.SelectedColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnTargetTemperature.SelectedText = "";
            BtnTargetTemperature.SelectionLength = 0;
            BtnTargetTemperature.SelectionStart = 0;
            BtnTargetTemperature.ShortcutsEnabled = true;
            BtnTargetTemperature.Size = new System.Drawing.Size(90, 30);
            BtnTargetTemperature.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnTargetTemperature.StylizeFlag = false;
            BtnTargetTemperature.TabIndex = 26;
            BtnTargetTemperature.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            BtnTargetTemperature.UseSystemPasswordChar = false;
            BtnTargetTemperature.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            BtnTargetTemperature.WordWrap = true;
            //BtnTargetTemperature.Click += BtnTargetTemperature_Click;
            // 
            // LblChnnlTargetTemp
            // 
            LblChnnlTargetTemp.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblChnnlTargetTemp.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblChnnlTargetTemp.BorderThickness = 0;
            LblChnnlTargetTemp.CornerRadius = 0;
            LblChnnlTargetTemp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblChnnlTargetTemp.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblChnnlTargetTemp.HighlightPrompt = defaultHighlightPrompt2;
            LblChnnlTargetTemp.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblChnnlTargetTemp.Location = new System.Drawing.Point(339, 1);
            LblChnnlTargetTemp.MultyLineFlag = false;
            LblChnnlTargetTemp.Name = "LblChnnlTargetTemp";
            LblChnnlTargetTemp.Size = new System.Drawing.Size(101, 22);
            LblChnnlTargetTemp.StyleFlags = UserControls.Style.StyleFlag.None;
            LblChnnlTargetTemp.StylizeFlag = true;
            LblChnnlTargetTemp.TabIndex = 25;
            LblChnnlTargetTemp.TabStop = false;
            LblChnnlTargetTemp.Text = "通道目标温度";
            LblChnnlTargetTemp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblChnnlTargetTemp.Token = null;
            // 
            // BtnDiffParam
            // 
            BtnDiffParam.AcceptsTab = false;
            BtnDiffParam.AutoShowKeyBoard = false;
            BtnDiffParam.AutoSize = false;
            BtnDiffParam.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnDiffParam.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnDiffParam.BorderThickness = 0;
            BtnDiffParam.ClickedBorderColor = System.Drawing.Color.White;
            BtnDiffParam.CornerRadius = 0;
            BtnDiffParam.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnDiffParam.Enabled = true;
            BtnDiffParam.EnbleSelectBorder = true;
            BtnDiffParam.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnDiffParam.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnDiffParam.Height = 30;
            BtnDiffParam.HideSelection = true;
            BtnDiffParam.IsFocusClicked = false;
            BtnDiffParam.KeyboardVerify = null;
            BtnDiffParam.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnDiffParam.Location = new System.Drawing.Point(228, 25);
            BtnDiffParam.MaxLength = 32767;
            BtnDiffParam.Modified = false;
            BtnDiffParam.MouseEnterState = false;
            BtnDiffParam.Multiline = false;
            BtnDiffParam.Name = "BtnDiffParam";
            BtnDiffParam.ProcessCmdKeyFunc = null;
            BtnDiffParam.ReadOnly = false;
            BtnDiffParam.SelectedColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnDiffParam.SelectedText = "";
            BtnDiffParam.SelectionLength = 0;
            BtnDiffParam.SelectionStart = 0;
            BtnDiffParam.ShortcutsEnabled = true;
            BtnDiffParam.Size = new System.Drawing.Size(90, 30);
            BtnDiffParam.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnDiffParam.StylizeFlag = true;
            BtnDiffParam.TabIndex = 24;
            BtnDiffParam.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            BtnDiffParam.UseSystemPasswordChar = false;
            BtnDiffParam.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            BtnDiffParam.WordWrap = true;
            //BtnDiffParam.Click += BtnDiffParam_Click;
            // 
            // ScopeXLabel3
            // 
            ScopeXLabel3.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ScopeXLabel3.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ScopeXLabel3.BorderThickness = 0;
            ScopeXLabel3.CornerRadius = 0;
            ScopeXLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ScopeXLabel3.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ScopeXLabel3.HighlightPrompt = defaultHighlightPrompt3;
            ScopeXLabel3.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ScopeXLabel3.Location = new System.Drawing.Point(228, 1);
            ScopeXLabel3.MultyLineFlag = false;
            ScopeXLabel3.Name = "ScopeXLabel3";
            ScopeXLabel3.Size = new System.Drawing.Size(90, 22);
            ScopeXLabel3.StyleFlags = UserControls.Style.StyleFlag.None;
            ScopeXLabel3.StylizeFlag = true;
            ScopeXLabel3.TabIndex = 23;
            ScopeXLabel3.TabStop = false;
            ScopeXLabel3.Text = "积分系数";
            ScopeXLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            ScopeXLabel3.Token = null;
            // 
            // BtnIntegralParam
            // 
            BtnIntegralParam.AcceptsTab = false;
            BtnIntegralParam.AutoShowKeyBoard = false;
            BtnIntegralParam.AutoSize = false;
            BtnIntegralParam.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnIntegralParam.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnIntegralParam.BorderThickness = 0;
            BtnIntegralParam.ClickedBorderColor = System.Drawing.Color.White;
            BtnIntegralParam.CornerRadius = 0;
            BtnIntegralParam.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnIntegralParam.Enabled = true;
            BtnIntegralParam.EnbleSelectBorder = true;
            BtnIntegralParam.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnIntegralParam.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnIntegralParam.Height = 30;
            BtnIntegralParam.HideSelection = true;
            BtnIntegralParam.IsFocusClicked = false;
            BtnIntegralParam.KeyboardVerify = null;
            BtnIntegralParam.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnIntegralParam.Location = new System.Drawing.Point(117, 25);
            BtnIntegralParam.MaxLength = 32767;
            BtnIntegralParam.Modified = false;
            BtnIntegralParam.MouseEnterState = false;
            BtnIntegralParam.Multiline = false;
            BtnIntegralParam.Name = "BtnIntegralParam";
            BtnIntegralParam.ProcessCmdKeyFunc = null;
            BtnIntegralParam.ReadOnly = false;
            BtnIntegralParam.SelectedColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnIntegralParam.SelectedText = "";
            BtnIntegralParam.SelectionLength = 0;
            BtnIntegralParam.SelectionStart = 0;
            BtnIntegralParam.ShortcutsEnabled = true;
            BtnIntegralParam.Size = new System.Drawing.Size(90, 30);
            BtnIntegralParam.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnIntegralParam.StylizeFlag = true;
            BtnIntegralParam.TabIndex = 22;
            BtnIntegralParam.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            BtnIntegralParam.UseSystemPasswordChar = false;
            BtnIntegralParam.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            BtnIntegralParam.WordWrap = true;
            //BtnIntegralParam.Click += BtnIntegralParam_Click;
            // 
            // ScopeXLabel2
            // 
            ScopeXLabel2.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ScopeXLabel2.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ScopeXLabel2.BorderThickness = 0;
            ScopeXLabel2.CornerRadius = 0;
            ScopeXLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ScopeXLabel2.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ScopeXLabel2.HighlightPrompt = defaultHighlightPrompt4;
            ScopeXLabel2.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ScopeXLabel2.Location = new System.Drawing.Point(117, 1);
            ScopeXLabel2.MultyLineFlag = false;
            ScopeXLabel2.Name = "ScopeXLabel2";
            ScopeXLabel2.Size = new System.Drawing.Size(90, 22);
            ScopeXLabel2.StyleFlags = UserControls.Style.StyleFlag.None;
            ScopeXLabel2.StylizeFlag = true;
            ScopeXLabel2.TabIndex = 21;
            ScopeXLabel2.TabStop = false;
            ScopeXLabel2.Text = "微分系数";
            ScopeXLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            ScopeXLabel2.Token = null;
            // 
            // BtnRadioParam
            // 
            BtnRadioParam.AcceptsTab = false;
            BtnRadioParam.AutoShowKeyBoard = false;
            BtnRadioParam.AutoSize = false;
            BtnRadioParam.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRadioParam.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRadioParam.BorderThickness = 0;
            BtnRadioParam.ClickedBorderColor = System.Drawing.Color.White;
            BtnRadioParam.CornerRadius = 0;
            BtnRadioParam.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnRadioParam.Enabled = true;
            BtnRadioParam.EnbleSelectBorder = true;
            BtnRadioParam.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnRadioParam.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnRadioParam.Height = 30;
            BtnRadioParam.HideSelection = true;
            BtnRadioParam.IsFocusClicked = false;
            BtnRadioParam.KeyboardVerify = null;
            BtnRadioParam.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnRadioParam.Location = new System.Drawing.Point(6, 25);
            BtnRadioParam.MaxLength = 32767;
            BtnRadioParam.Modified = false;
            BtnRadioParam.MouseEnterState = false;
            BtnRadioParam.Multiline = false;
            BtnRadioParam.Name = "BtnRadioParam";
            BtnRadioParam.ProcessCmdKeyFunc = null;
            BtnRadioParam.ReadOnly = false;
            BtnRadioParam.SelectedColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnRadioParam.SelectedText = "";
            BtnRadioParam.SelectionLength = 0;
            BtnRadioParam.SelectionStart = 0;
            BtnRadioParam.ShortcutsEnabled = true;
            BtnRadioParam.Size = new System.Drawing.Size(90, 30);
            BtnRadioParam.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnRadioParam.StylizeFlag = true;
            BtnRadioParam.TabIndex = 20;
            BtnRadioParam.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            BtnRadioParam.UseSystemPasswordChar = false;
            BtnRadioParam.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            BtnRadioParam.WordWrap = true;
            //BtnRadioParam.Click += BtnRadioParam_Click;
            // 
            // ScopeXLabel1
            // 
            ScopeXLabel1.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ScopeXLabel1.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ScopeXLabel1.BorderThickness = 0;
            ScopeXLabel1.CornerRadius = 0;
            ScopeXLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ScopeXLabel1.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ScopeXLabel1.HighlightPrompt = defaultHighlightPrompt5;
            ScopeXLabel1.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ScopeXLabel1.Location = new System.Drawing.Point(6, 1);
            ScopeXLabel1.MultyLineFlag = false;
            ScopeXLabel1.Name = "ScopeXLabel1";
            ScopeXLabel1.Size = new System.Drawing.Size(90, 22);
            ScopeXLabel1.StyleFlags = UserControls.Style.StyleFlag.None;
            ScopeXLabel1.StylizeFlag = true;
            ScopeXLabel1.TabIndex = 19;
            ScopeXLabel1.TabStop = false;
            ScopeXLabel1.Text = "比例系数";
            ScopeXLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            ScopeXLabel1.Token = null;
            // 
            // TemperatureTestForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            CanClose = false;
            ClientSize = new System.Drawing.Size(1800, 853);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(TlpFigure);
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            IconWidth = 26;
            IsShowPin = false;
            Name = "TemperatureTestForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "温度曲线";
            Title = "温度曲线";
            TitleColor = System.Drawing.Color.White;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Controls.SetChildIndex(TlpFigure, 0);
            TlpFigure.ResumeLayout(false);
            panelHead.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpFigure;
        private ScottPlot.FormsPlot ScottPlotFormControl;
        private System.Windows.Forms.Panel panelHead;
        private ScopeX.UserControls.ScopeXTextBox BtnTargetTemperature;
        private ScopeX.UserControls.ScopeXLabel LblChnnlTargetTemp;
        private ScopeX.UserControls.ScopeXTextBox BtnDiffParam;
        private ScopeX.UserControls.ScopeXLabel ScopeXLabel3;
        private ScopeX.UserControls.ScopeXTextBox BtnIntegralParam;
        private ScopeX.UserControls.ScopeXLabel ScopeXLabel2;
        private ScopeX.UserControls.ScopeXTextBox BtnRadioParam;
        private ScopeX.UserControls.ScopeXLabel ScopeXLabel1;
        private UserControls.ScopeXLabel LblHardDiskTargetTemp;
        private UserControls.ScopeXTextBox BtnHardDiskTargetTemperature;
    }
}