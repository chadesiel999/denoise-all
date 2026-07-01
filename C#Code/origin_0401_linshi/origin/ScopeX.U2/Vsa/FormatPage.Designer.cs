
namespace ScopeX.U2
{
    partial class FormatPage
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
            ScopeX.UserControls.RadioButtonItem radioButtonItem1 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem2 = new ScopeX.UserControls.RadioButtonItem();
            this.TlpVector = new System.Windows.Forms.TableLayoutPanel();
            this.GrpFormat = new ScopeX.UserControls.ScopeXGroupBox();
            this.LblBitPerSym = new ScopeX.UserControls.ScopeXLabel();
            this.BtnBitsPerSym = new ScopeX.UserControls.ScopeXIconButton();
            this.BtnSymbolRate = new ScopeX.UserControls.ScopeXIconButton();
            this.LblSymbolRate = new ScopeX.UserControls.ScopeXLabel();
            this.CbxFormat = new ScopeX.UserControls.ComboBoxEx();
            this.LblFormat = new ScopeX.UserControls.ScopeXLabel();
            this.GrpTiming = new ScopeX.UserControls.ScopeXGroupBox();
            this.LblInterpolation = new ScopeX.UserControls.ScopeXLabel();
            this.RdoInterpolation = new ScopeX.UserControls.UIRadioButtonGroup();
            this.LblTimingEst = new ScopeX.UserControls.ScopeXLabel();
            this.CbxTimingEst = new ScopeX.UserControls.ComboBoxEx();
            this.LblSampPerBaud = new ScopeX.UserControls.ScopeXLabel();
            this.BtnSampPerBaud = new ScopeX.UserControls.ScopeXIconButton();
            this.PnlTriggerType = new System.Windows.Forms.Panel();
            this.CbxTemplate = new ScopeX.UserControls.ComboBoxEx();
            this.LblSource = new ScopeX.UserControls.ScopeXLabel();
            this.LblEnabled = new ScopeX.UserControls.ScopeXLabel();
            this.CbxSource = new ScopeX.UserControls.ComboBoxEx();
            this.LblTemplate = new ScopeX.UserControls.ScopeXLabel();
            this.ChkEnabled = new ScopeX.UserControls.ScopeXSwitchButton();
            this.TlpVector.SuspendLayout();
            this.GrpFormat.SuspendLayout();
            this.GrpTiming.SuspendLayout();
            this.PnlTriggerType.SuspendLayout();
            this.SuspendLayout();
            // 
            // TlpVector
            // 
            this.TlpVector.ColumnCount = 1;
            this.TlpVector.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpVector.Controls.Add(this.GrpFormat, 0, 1);
            this.TlpVector.Controls.Add(this.GrpTiming, 0, 2);
            this.TlpVector.Controls.Add(this.PnlTriggerType, 0, 0);
            this.TlpVector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpVector.Location = new System.Drawing.Point(0, 0);
            this.TlpVector.Name = "TlpVector";
            this.TlpVector.RowCount = 3;
            this.TlpVector.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.TlpVector.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 107F));
            this.TlpVector.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 114F));
            this.TlpVector.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TlpVector.Size = new System.Drawing.Size(480, 311);
            this.TlpVector.TabIndex = 6;
            // 
            // GrpFormat
            // 
            this.GrpFormat.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.GrpFormat.Controls.Add(this.LblBitPerSym);
            this.GrpFormat.Controls.Add(this.BtnBitsPerSym);
            this.GrpFormat.Controls.Add(this.BtnSymbolRate);
            this.GrpFormat.Controls.Add(this.LblSymbolRate);
            this.GrpFormat.Controls.Add(this.CbxFormat);
            this.GrpFormat.Controls.Add(this.LblFormat);
            this.GrpFormat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GrpFormat.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GrpFormat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.GrpFormat.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.GrpFormat.Location = new System.Drawing.Point(3, 77);
            this.GrpFormat.Name = "GrpFormat";
            this.GrpFormat.Size = new System.Drawing.Size(474, 101);
            this.GrpFormat.TabIndex = 22;
            this.GrpFormat.TabStop = false;
            this.GrpFormat.Text = "格式";
            // 
            // LblBitPerSym
            // 
            this.LblBitPerSym.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblBitPerSym.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblBitPerSym.BorderThickness = 0;
            this.LblBitPerSym.CornerRadius = 0;
            this.LblBitPerSym.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblBitPerSym.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblBitPerSym.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblBitPerSym.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblBitPerSym.Location = new System.Drawing.Point(353, 30);
            this.LblBitPerSym.MultyLineFlag = false;
            this.LblBitPerSym.Name = "LblBitPerSym";
            this.LblBitPerSym.Size = new System.Drawing.Size(100, 17);
            this.LblBitPerSym.TabIndex = 10;
            this.LblBitPerSym.TabStop = false;
            this.LblBitPerSym.Text = "Points / Symbol";
            this.LblBitPerSym.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BtnBitsPerSym
            // 
            this.BtnBitsPerSym.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnBitsPerSym.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnBitsPerSym.BorderThickness = 0;
            this.BtnBitsPerSym.CornerRadius = 0;
            this.BtnBitsPerSym.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnBitsPerSym.DaskArray = null;
            this.BtnBitsPerSym.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnBitsPerSym.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnBitsPerSym.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnBitsPerSym.Icon = null;
            this.BtnBitsPerSym.IconOffset = 10;
            this.BtnBitsPerSym.IconSize = new System.Drawing.Size(24, 24);
            this.BtnBitsPerSym.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnBitsPerSym.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnBitsPerSym.Location = new System.Drawing.Point(353, 53);
            this.BtnBitsPerSym.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnBitsPerSym.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnBitsPerSym.MouseInBorderThickness = 0;
            this.BtnBitsPerSym.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnBitsPerSym.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnBitsPerSym.Name = "BtnBitsPerSym";
            this.BtnBitsPerSym.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnBitsPerSym.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnBitsPerSym.PressedBorderThickness = 0;
            this.BtnBitsPerSym.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnBitsPerSym.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnBitsPerSym.Size = new System.Drawing.Size(100, 30);
            this.BtnBitsPerSym.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnBitsPerSym.SVGPath = "";
            this.BtnBitsPerSym.TabIndex = 11;
            this.BtnBitsPerSym.Text = "1";
            this.BtnBitsPerSym.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnBitsPerSym.Click += new System.EventHandler(this.BtnBitsPerSym_Click);
            // 
            // BtnSymbolRate
            // 
            this.BtnSymbolRate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSymbolRate.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSymbolRate.BorderThickness = 0;
            this.BtnSymbolRate.CornerRadius = 0;
            this.BtnSymbolRate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSymbolRate.DaskArray = null;
            this.BtnSymbolRate.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnSymbolRate.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnSymbolRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSymbolRate.Icon = null;
            this.BtnSymbolRate.IconOffset = 10;
            this.BtnSymbolRate.IconSize = new System.Drawing.Size(24, 24);
            this.BtnSymbolRate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnSymbolRate.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnSymbolRate.Location = new System.Drawing.Point(185, 53);
            this.BtnSymbolRate.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSymbolRate.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSymbolRate.MouseInBorderThickness = 0;
            this.BtnSymbolRate.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSymbolRate.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnSymbolRate.Name = "BtnSymbolRate";
            this.BtnSymbolRate.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnSymbolRate.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSymbolRate.PressedBorderThickness = 0;
            this.BtnSymbolRate.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSymbolRate.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnSymbolRate.Size = new System.Drawing.Size(150, 30);
            this.BtnSymbolRate.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSymbolRate.SVGPath = "";
            this.BtnSymbolRate.TabIndex = 12;
            this.BtnSymbolRate.Text = "###.### Hz";
            this.BtnSymbolRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnSymbolRate.Click += new System.EventHandler(this.BtnSymbolRate_Click);
            // 
            // LblSymbolRate
            // 
            this.LblSymbolRate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblSymbolRate.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblSymbolRate.BorderThickness = 0;
            this.LblSymbolRate.CornerRadius = 0;
            this.LblSymbolRate.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblSymbolRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblSymbolRate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblSymbolRate.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblSymbolRate.Location = new System.Drawing.Point(185, 30);
            this.LblSymbolRate.MultyLineFlag = false;
            this.LblSymbolRate.Name = "LblSymbolRate";
            this.LblSymbolRate.Size = new System.Drawing.Size(150, 17);
            this.LblSymbolRate.TabIndex = 7;
            this.LblSymbolRate.TabStop = false;
            this.LblSymbolRate.Text = "符号率";
            this.LblSymbolRate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CbxFormat
            // 
            this.CbxFormat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxFormat.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxFormat.BorderThickness = 0;
            this.CbxFormat.CornerRadius = 0;
            this.CbxFormat.DataSource = null;
            this.CbxFormat.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CbxFormat.DropDownHeight = 1000;
            this.CbxFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxFormat.DropDownWidth = 95;
            this.CbxFormat.DropKey = System.Windows.Forms.Keys.Space;
            this.CbxFormat.DroppedDown = false;
            this.CbxFormat.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxFormat.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CbxFormat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.CbxFormat.FormattingEnabled = true;
            this.CbxFormat.GetDisPlayName = null;
            this.CbxFormat.ItemHeight = 28;
            this.CbxFormat.Items.AddRange(new object[] {
            "边沿",
            "脉宽",
            "视频",
            "码型",
            "状态",
            "超时",
            "建立保持",
            "欠幅",
            "斜率",
            "毛刺",
            "窗口",
            "间隔",
            "多级",
            "串行"});
            this.CbxFormat.KeyDropEnble = true;
            this.CbxFormat.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.CbxFormat.Location = new System.Drawing.Point(27, 53);
            this.CbxFormat.MaxDropDownItems = 20;
            this.CbxFormat.Name = "CbxFormat";
            this.CbxFormat.RectBtnWidth = 20;
            this.CbxFormat.SelectedBackColor = System.Drawing.Color.Blue;
            this.CbxFormat.SelectedIndex = -1;
            this.CbxFormat.SelectedItem = null;
            this.CbxFormat.SelectedText = "";
            this.CbxFormat.Size = new System.Drawing.Size(100, 30);
            this.CbxFormat.Soreted = false;
            this.CbxFormat.TabIndex = 9;
            this.CbxFormat.Text = "";
            this.CbxFormat.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.CbxFormat.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            // 
            // LblFormat
            // 
            this.LblFormat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblFormat.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblFormat.BorderThickness = 0;
            this.LblFormat.CornerRadius = 0;
            this.LblFormat.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblFormat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblFormat.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblFormat.Location = new System.Drawing.Point(27, 30);
            this.LblFormat.MultyLineFlag = false;
            this.LblFormat.Name = "LblFormat";
            this.LblFormat.Size = new System.Drawing.Size(100, 17);
            this.LblFormat.TabIndex = 8;
            this.LblFormat.TabStop = false;
            this.LblFormat.Text = "类型";
            this.LblFormat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GrpTiming
            // 
            this.GrpTiming.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.GrpTiming.Controls.Add(this.LblInterpolation);
            this.GrpTiming.Controls.Add(this.RdoInterpolation);
            this.GrpTiming.Controls.Add(this.LblTimingEst);
            this.GrpTiming.Controls.Add(this.CbxTimingEst);
            this.GrpTiming.Controls.Add(this.LblSampPerBaud);
            this.GrpTiming.Controls.Add(this.BtnSampPerBaud);
            this.GrpTiming.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GrpTiming.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GrpTiming.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.GrpTiming.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.GrpTiming.Location = new System.Drawing.Point(3, 184);
            this.GrpTiming.Name = "GrpTiming";
            this.GrpTiming.Size = new System.Drawing.Size(474, 124);
            this.GrpTiming.TabIndex = 21;
            this.GrpTiming.TabStop = false;
            this.GrpTiming.Text = "定时同步";
            // 
            // LblInterpolation
            // 
            this.LblInterpolation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblInterpolation.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblInterpolation.BorderThickness = 0;
            this.LblInterpolation.CornerRadius = 0;
            this.LblInterpolation.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblInterpolation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblInterpolation.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblInterpolation.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblInterpolation.Location = new System.Drawing.Point(185, 39);
            this.LblInterpolation.MultyLineFlag = false;
            this.LblInterpolation.Name = "LblInterpolation";
            this.LblInterpolation.Size = new System.Drawing.Size(150, 17);
            this.LblInterpolation.TabIndex = 8;
            this.LblInterpolation.TabStop = false;
            this.LblInterpolation.Text = "插值方法";
            this.LblInterpolation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RdoInterpolation
            // 
            this.RdoInterpolation.BackColor = System.Drawing.Color.Black;
            this.RdoInterpolation.BorderColor = System.Drawing.Color.Black;
            this.RdoInterpolation.BorderThickness = 0;
            this.RdoInterpolation.ButtonBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.RdoInterpolation.ButtonFont = null;
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = "线性";
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = "Sinc";
            this.RdoInterpolation.ButtonItems = new ScopeX.UserControls.RadioButtonItem[] {
        radioButtonItem1,
        radioButtonItem2};
            this.RdoInterpolation.ButtonOffset = 10;
            this.RdoInterpolation.ButtonTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.RdoInterpolation.ChoosedButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.RdoInterpolation.ChoosedButtonIndex = 0;
            this.RdoInterpolation.ChoosedButtonTextColor = System.Drawing.Color.Black;
            this.RdoInterpolation.ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.RdoInterpolation.ContentPadding = new System.Windows.Forms.Padding(0);
            this.RdoInterpolation.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.RdoInterpolation.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.RdoInterpolation.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.RdoInterpolation.Location = new System.Drawing.Point(185, 62);
            this.RdoInterpolation.Margin = new System.Windows.Forms.Padding(0);
            this.RdoInterpolation.Name = "RdoInterpolation";
            this.RdoInterpolation.Size = new System.Drawing.Size(150, 30);
            this.RdoInterpolation.TabIndex = 9;
            this.RdoInterpolation.IndexChanged += new System.EventHandler(this.RdoInterpolation_IndexChanged);
            // 
            // LblTimingEst
            // 
            this.LblTimingEst.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblTimingEst.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblTimingEst.BorderThickness = 0;
            this.LblTimingEst.CornerRadius = 0;
            this.LblTimingEst.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblTimingEst.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblTimingEst.Location = new System.Drawing.Point(27, 39);
            this.LblTimingEst.MultyLineFlag = false;
            this.LblTimingEst.Name = "LblTimingEst";
            this.LblTimingEst.Size = new System.Drawing.Size(100, 17);
            this.LblTimingEst.TabIndex = 20;
            this.LblTimingEst.TabStop = false;
            this.LblTimingEst.Text = "定时估计";
            this.LblTimingEst.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CbxTimingEst
            // 
            this.CbxTimingEst.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxTimingEst.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxTimingEst.BorderThickness = 0;
            this.CbxTimingEst.CornerRadius = 0;
            this.CbxTimingEst.DataSource = null;
            this.CbxTimingEst.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CbxTimingEst.DropDownHeight = 200;
            this.CbxTimingEst.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxTimingEst.DropDownWidth = 95;
            this.CbxTimingEst.DropKey = System.Windows.Forms.Keys.Space;
            this.CbxTimingEst.DroppedDown = false;
            this.CbxTimingEst.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxTimingEst.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CbxTimingEst.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.CbxTimingEst.FormattingEnabled = true;
            this.CbxTimingEst.GetDisPlayName = null;
            this.CbxTimingEst.ItemHeight = 28;
            this.CbxTimingEst.Items.AddRange(new object[] {
            "Square",
            "Std dev",
            "EVM",
            "Cross Points"});
            this.CbxTimingEst.KeyDropEnble = true;
            this.CbxTimingEst.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.CbxTimingEst.Location = new System.Drawing.Point(27, 62);
            this.CbxTimingEst.MaxDropDownItems = 8;
            this.CbxTimingEst.Name = "CbxTimingEst";
            this.CbxTimingEst.RectBtnWidth = 20;
            this.CbxTimingEst.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.CbxTimingEst.SelectedIndex = -1;
            this.CbxTimingEst.SelectedItem = null;
            this.CbxTimingEst.SelectedText = "";
            this.CbxTimingEst.Size = new System.Drawing.Size(100, 30);
            this.CbxTimingEst.Soreted = false;
            this.CbxTimingEst.TabIndex = 20;
            this.CbxTimingEst.Text = "";
            this.CbxTimingEst.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.CbxTimingEst.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.CbxTimingEst.SelectedIndexChanged += new System.EventHandler(this.CbxTimingEst_SelectedIndexChanged);
            // 
            // LblSampPerBaud
            // 
            this.LblSampPerBaud.BackColor = System.Drawing.Color.Empty;
            this.LblSampPerBaud.BorderColor = System.Drawing.Color.Black;
            this.LblSampPerBaud.BorderThickness = 0;
            this.LblSampPerBaud.CornerRadius = 0;
            this.LblSampPerBaud.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblSampPerBaud.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblSampPerBaud.Location = new System.Drawing.Point(353, 38);
            this.LblSampPerBaud.MultyLineFlag = false;
            this.LblSampPerBaud.Name = "LblSampPerBaud";
            this.LblSampPerBaud.Size = new System.Drawing.Size(100, 18);
            this.LblSampPerBaud.TabIndex = 24;
            this.LblSampPerBaud.TabStop = false;
            this.LblSampPerBaud.Text = "Samples / Baud";
            this.LblSampPerBaud.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BtnSampPerBaud
            // 
            this.BtnSampPerBaud.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSampPerBaud.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSampPerBaud.BorderThickness = 0;
            this.BtnSampPerBaud.CornerRadius = 0;
            this.BtnSampPerBaud.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSampPerBaud.DaskArray = null;
            this.BtnSampPerBaud.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnSampPerBaud.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnSampPerBaud.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSampPerBaud.Icon = null;
            this.BtnSampPerBaud.IconOffset = 10;
            this.BtnSampPerBaud.IconSize = new System.Drawing.Size(24, 24);
            this.BtnSampPerBaud.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnSampPerBaud.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnSampPerBaud.Location = new System.Drawing.Point(353, 62);
            this.BtnSampPerBaud.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSampPerBaud.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSampPerBaud.MouseInBorderThickness = 0;
            this.BtnSampPerBaud.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSampPerBaud.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnSampPerBaud.Name = "BtnSampPerBaud";
            this.BtnSampPerBaud.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnSampPerBaud.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSampPerBaud.PressedBorderThickness = 0;
            this.BtnSampPerBaud.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSampPerBaud.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnSampPerBaud.Size = new System.Drawing.Size(100, 30);
            this.BtnSampPerBaud.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSampPerBaud.SVGPath = "";
            this.BtnSampPerBaud.TabIndex = 6;
            this.BtnSampPerBaud.Text = "1";
            this.BtnSampPerBaud.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnSampPerBaud.Click += new System.EventHandler(this.BtnSampPerBaud_Click);
            // 
            // PnlTriggerType
            // 
            this.PnlTriggerType.Controls.Add(this.CbxTemplate);
            this.PnlTriggerType.Controls.Add(this.LblSource);
            this.PnlTriggerType.Controls.Add(this.LblEnabled);
            this.PnlTriggerType.Controls.Add(this.CbxSource);
            this.PnlTriggerType.Controls.Add(this.LblTemplate);
            this.PnlTriggerType.Controls.Add(this.ChkEnabled);
            this.PnlTriggerType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlTriggerType.Location = new System.Drawing.Point(0, 0);
            this.PnlTriggerType.Margin = new System.Windows.Forms.Padding(0);
            this.PnlTriggerType.Name = "PnlTriggerType";
            this.PnlTriggerType.Size = new System.Drawing.Size(480, 74);
            this.PnlTriggerType.TabIndex = 1;
            // 
            // CbxTemplate
            // 
            this.CbxTemplate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxTemplate.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxTemplate.BorderThickness = 0;
            this.CbxTemplate.CornerRadius = 0;
            this.CbxTemplate.DataSource = null;
            this.CbxTemplate.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CbxTemplate.DropDownHeight = 1000;
            this.CbxTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxTemplate.DropDownWidth = 95;
            this.CbxTemplate.DropKey = System.Windows.Forms.Keys.Space;
            this.CbxTemplate.DroppedDown = false;
            this.CbxTemplate.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxTemplate.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CbxTemplate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.CbxTemplate.FormattingEnabled = true;
            this.CbxTemplate.GetDisPlayName = null;
            this.CbxTemplate.ItemHeight = 28;
            this.CbxTemplate.Items.AddRange(new object[] {
            "RF",
            "IQ",
            "自定义"});
            this.CbxTemplate.KeyDropEnble = true;
            this.CbxTemplate.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.CbxTemplate.Location = new System.Drawing.Point(353, 27);
            this.CbxTemplate.MaxDropDownItems = 20;
            this.CbxTemplate.Name = "CbxTemplate";
            this.CbxTemplate.RectBtnWidth = 20;
            this.CbxTemplate.SelectedBackColor = System.Drawing.Color.Blue;
            this.CbxTemplate.SelectedIndex = -1;
            this.CbxTemplate.SelectedItem = null;
            this.CbxTemplate.SelectedText = "";
            this.CbxTemplate.Size = new System.Drawing.Size(100, 30);
            this.CbxTemplate.Soreted = false;
            this.CbxTemplate.TabIndex = 3;
            this.CbxTemplate.Text = "";
            this.CbxTemplate.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.CbxTemplate.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.CbxTemplate.SelectedIndexChanged += new System.EventHandler(this.CbxTemplate_SelectedIndexChanged);
            // 
            // LblSource
            // 
            this.LblSource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblSource.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblSource.BorderThickness = 0;
            this.LblSource.CornerRadius = 0;
            this.LblSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblSource.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblSource.Location = new System.Drawing.Point(185, 4);
            this.LblSource.MultyLineFlag = false;
            this.LblSource.Name = "LblSource";
            this.LblSource.Size = new System.Drawing.Size(85, 17);
            this.LblSource.TabIndex = 2;
            this.LblSource.TabStop = false;
            this.LblSource.Text = "信源";
            this.LblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblEnabled
            // 
            this.LblEnabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblEnabled.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblEnabled.BorderThickness = 0;
            this.LblEnabled.CornerRadius = 0;
            this.LblEnabled.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblEnabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblEnabled.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblEnabled.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblEnabled.Location = new System.Drawing.Point(27, 4);
            this.LblEnabled.MultyLineFlag = false;
            this.LblEnabled.Name = "LblEnabled";
            this.LblEnabled.Size = new System.Drawing.Size(75, 17);
            this.LblEnabled.TabIndex = 0;
            this.LblEnabled.TabStop = false;
            this.LblEnabled.Text = "使能";
            this.LblEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CbxSource
            // 
            this.CbxSource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxSource.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxSource.BorderThickness = 0;
            this.CbxSource.CornerRadius = 0;
            this.CbxSource.DataSource = null;
            this.CbxSource.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CbxSource.DropDownHeight = 1000;
            this.CbxSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxSource.DropDownWidth = 95;
            this.CbxSource.DropKey = System.Windows.Forms.Keys.Space;
            this.CbxSource.DroppedDown = false;
            this.CbxSource.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CbxSource.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.CbxSource.FormattingEnabled = true;
            this.CbxSource.GetDisPlayName = null;
            this.CbxSource.ItemHeight = 28;
            this.CbxSource.Items.AddRange(new object[] {
            "边沿",
            "脉宽",
            "视频",
            "码型",
            "状态",
            "超时",
            "建立保持",
            "欠幅",
            "斜率",
            "毛刺",
            "窗口",
            "间隔",
            "多级",
            "串行"});
            this.CbxSource.KeyDropEnble = true;
            this.CbxSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.CbxSource.Location = new System.Drawing.Point(185, 27);
            this.CbxSource.MaxDropDownItems = 20;
            this.CbxSource.Name = "CbxSource";
            this.CbxSource.RectBtnWidth = 20;
            this.CbxSource.SelectedBackColor = System.Drawing.Color.Blue;
            this.CbxSource.SelectedIndex = -1;
            this.CbxSource.SelectedItem = null;
            this.CbxSource.SelectedText = "";
            this.CbxSource.Size = new System.Drawing.Size(85, 30);
            this.CbxSource.Soreted = false;
            this.CbxSource.TabIndex = 3;
            this.CbxSource.Text = "";
            this.CbxSource.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.CbxSource.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            // 
            // LblTemplate
            // 
            this.LblTemplate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblTemplate.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblTemplate.BorderThickness = 0;
            this.LblTemplate.CornerRadius = 0;
            this.LblTemplate.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblTemplate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblTemplate.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblTemplate.Location = new System.Drawing.Point(354, 4);
            this.LblTemplate.MultyLineFlag = false;
            this.LblTemplate.Name = "LblTemplate";
            this.LblTemplate.Size = new System.Drawing.Size(100, 17);
            this.LblTemplate.TabIndex = 2;
            this.LblTemplate.TabStop = false;
            this.LblTemplate.Text = "模板";
            this.LblTemplate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ChkEnabled
            // 
            this.ChkEnabled.AnimationCount = 8;
            this.ChkEnabled.AnimationFunc = null;
            this.ChkEnabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkEnabled.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkEnabled.BorderThickness = 0;
            this.ChkEnabled.Checked = false;
            this.ChkEnabled.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.ChkEnabled.CheckedForeColor = System.Drawing.Color.Black;
            this.ChkEnabled.CheckedSliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkEnabled.CheckedText = "开";
            this.ChkEnabled.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ChkEnabled.DropKey = System.Windows.Forms.Keys.Space;
            this.ChkEnabled.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkEnabled.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ChkEnabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.ChkEnabled.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.ChkEnabled.Location = new System.Drawing.Point(27, 27);
            this.ChkEnabled.Margin = new System.Windows.Forms.Padding(0);
            this.ChkEnabled.Name = "ChkEnabled";
            this.ChkEnabled.Size = new System.Drawing.Size(75, 30);
            this.ChkEnabled.SliderButtonWidth = 30;
            this.ChkEnabled.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkEnabled.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            this.ChkEnabled.TabIndex = 5;
            this.ChkEnabled.Text = "关";
            this.ChkEnabled.UseAnimation = true;
            this.ChkEnabled.CheckedChangedEvent += new System.EventHandler(this.ChkEnabled_CheckedChangedEvent);
            // 
            // FormatPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.Controls.Add(this.TlpVector);
            this.DoubleBuffered = true;
            this.Name = "FormatPage";
            this.Size = new System.Drawing.Size(480, 311);
            this.TlpVector.ResumeLayout(false);
            this.GrpFormat.ResumeLayout(false);
            this.GrpTiming.ResumeLayout(false);
            this.PnlTriggerType.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpVector;
        private ScopeX.UserControls.ScopeXGroupBox GrpFormat;
        private ScopeX.UserControls.ScopeXLabel LblBitPerSym;
        private ScopeX.UserControls.ScopeXIconButton BtnBitsPerSym;
        private ScopeX.UserControls.ScopeXIconButton BtnSymbolRate;
        private ScopeX.UserControls.ScopeXLabel LblSymbolRate;
        private ScopeX.UserControls.ComboBoxEx CbxFormat;
        private ScopeX.UserControls.ScopeXLabel LblFormat;
        private ScopeX.UserControls.ScopeXGroupBox GrpTiming;
        private ScopeX.UserControls.ScopeXLabel LblInterpolation;
        private ScopeX.UserControls.UIRadioButtonGroup RdoInterpolation;
        private ScopeX.UserControls.ScopeXLabel LblTimingEst;
        private ScopeX.UserControls.ComboBoxEx CbxTimingEst;
        private ScopeX.UserControls.ScopeXLabel LblSampPerBaud;
        private ScopeX.UserControls.ScopeXIconButton BtnSampPerBaud;
        private System.Windows.Forms.Panel PnlTriggerType;
        private ScopeX.UserControls.ComboBoxEx CbxTemplate;
        private ScopeX.UserControls.ScopeXLabel LblSource;
        private ScopeX.UserControls.ScopeXLabel LblEnabled;
        private ScopeX.UserControls.ComboBoxEx CbxSource;
        private ScopeX.UserControls.ScopeXLabel LblTemplate;
        private ScopeX.UserControls.ScopeXSwitchButton ChkEnabled;
    }
}
