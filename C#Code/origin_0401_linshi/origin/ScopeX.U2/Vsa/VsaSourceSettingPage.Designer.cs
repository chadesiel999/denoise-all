
namespace ScopeX.U2
{
    partial class VsaSourceSettingPage
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
            this.CbxTemplate = new ScopeX.UserControls.ComboBoxEx();
            this.LblSourceType = new ScopeX.UserControls.ScopeXLabel();
            this.LblEnabled = new ScopeX.UserControls.ScopeXLabel();
            this.CbxSignalType = new ScopeX.UserControls.ComboBoxEx();
            this.LblTemplate = new ScopeX.UserControls.ScopeXLabel();
            this.ChkEnabled = new ScopeX.UserControls.ScopeXSwitchButton();
            this.lblSource1 = new ScopeX.UserControls.ScopeXLabel();
            this.cbxSource1 = new ScopeX.UserControls.ComboBoxEx();
            this.lblSource2 = new ScopeX.UserControls.ScopeXLabel();
            this.cbxSource2 = new ScopeX.UserControls.ComboBoxEx();
            this.lblDemod = new ScopeX.UserControls.ScopeXLabel();
            this.cbxDemod = new ScopeX.UserControls.ComboBoxEx();
            this.SuspendLayout();
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
            this.CbxTemplate.Height = 30;
            this.CbxTemplate.ItemHeight = 28;
            this.CbxTemplate.Items.AddRange(new object[] {
            "RF",
            "IQ"});
            this.CbxTemplate.KeyDropEnble = true;
            this.CbxTemplate.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.CbxTemplate.Location = new System.Drawing.Point(185, 26);
            this.CbxTemplate.MaxDropDownItems = 20;
            this.CbxTemplate.Name = "CbxTemplate";
            this.CbxTemplate.RectBtnWidth = 20;
            this.CbxTemplate.SelectedBackColor = System.Drawing.Color.Blue;
            this.CbxTemplate.SelectedIndex = 0;
            this.CbxTemplate.SelectedItem = null;
            this.CbxTemplate.SelectedText = "";
            this.CbxTemplate.Size = new System.Drawing.Size(100, 30);
            this.CbxTemplate.Soreted = false;
            this.CbxTemplate.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.CbxTemplate.StylizeFlag = false;
            this.CbxTemplate.TabIndex = 9;
            this.CbxTemplate.Text = "RF";
            this.CbxTemplate.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.CbxTemplate.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.CbxTemplate.SelectedIndexChanged += new System.EventHandler(this.CbxTemplate_SelectedIndexChanged);
            // 
            // LblSourceType
            // 
            this.LblSourceType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblSourceType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblSourceType.BorderThickness = 0;
            this.LblSourceType.CornerRadius = 0;
            this.LblSourceType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblSourceType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblSourceType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblSourceType.Location = new System.Drawing.Point(21, 74);
            this.LblSourceType.MultyLineFlag = false;
            this.LblSourceType.Name = "LblSourceType";
            this.LblSourceType.Size = new System.Drawing.Size(100, 17);
            this.LblSourceType.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblSourceType.StylizeFlag = false;
            this.LblSourceType.TabIndex = 7;
            this.LblSourceType.TabStop = false;
            this.LblSourceType.Text = "信号类型";
            this.LblSourceType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.LblEnabled.Location = new System.Drawing.Point(21, 3);
            this.LblEnabled.MultyLineFlag = false;
            this.LblEnabled.Name = "LblEnabled";
            this.LblEnabled.Size = new System.Drawing.Size(75, 17);
            this.LblEnabled.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblEnabled.StylizeFlag = false;
            this.LblEnabled.TabIndex = 6;
            this.LblEnabled.TabStop = false;
            this.LblEnabled.Text = "使能";
            this.LblEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CbxSignalType
            // 
            this.CbxSignalType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxSignalType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxSignalType.BorderThickness = 0;
            this.CbxSignalType.CornerRadius = 0;
            this.CbxSignalType.DataSource = null;
            this.CbxSignalType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CbxSignalType.DropDownHeight = 1000;
            this.CbxSignalType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxSignalType.DropDownWidth = 95;
            this.CbxSignalType.DropKey = System.Windows.Forms.Keys.Space;
            this.CbxSignalType.DroppedDown = false;
            this.CbxSignalType.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxSignalType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CbxSignalType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.CbxSignalType.FormattingEnabled = true;
            this.CbxSignalType.GetDisPlayName = null;
            this.CbxSignalType.Height = 30;
            this.CbxSignalType.ItemHeight = 28;
            this.CbxSignalType.Items.AddRange(new object[] {
            "通用数字解调",
            "蓝牙信号",
            "802.11ad",
            "LTE",
            "5GNR",
            "OFDM"});
            this.CbxSignalType.KeyDropEnble = true;
            this.CbxSignalType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.CbxSignalType.Location = new System.Drawing.Point(21, 97);
            this.CbxSignalType.MaxDropDownItems = 20;
            this.CbxSignalType.Name = "CbxSignalType";
            this.CbxSignalType.RectBtnWidth = 20;
            this.CbxSignalType.SelectedBackColor = System.Drawing.Color.Blue;
            this.CbxSignalType.SelectedIndex = 0;
            this.CbxSignalType.SelectedItem = "通用数字解调";
            this.CbxSignalType.SelectedText = "";
            this.CbxSignalType.Size = new System.Drawing.Size(100, 30);
            this.CbxSignalType.Soreted = false;
            this.CbxSignalType.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.CbxSignalType.StylizeFlag = false;
            this.CbxSignalType.TabIndex = 10;
            this.CbxSignalType.Text = "通用数字解调";
            this.CbxSignalType.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.CbxSignalType.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.CbxSignalType.SelectedIndexChanged += new System.EventHandler(this.CbxSignalType_SelectedIndexChanged);
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
            this.LblTemplate.Location = new System.Drawing.Point(186, 3);
            this.LblTemplate.MultyLineFlag = false;
            this.LblTemplate.Name = "LblTemplate";
            this.LblTemplate.Size = new System.Drawing.Size(100, 17);
            this.LblTemplate.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblTemplate.StylizeFlag = false;
            this.LblTemplate.TabIndex = 8;
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
            this.ChkEnabled.Height = 30;
            this.ChkEnabled.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.ChkEnabled.Location = new System.Drawing.Point(21, 26);
            this.ChkEnabled.Margin = new System.Windows.Forms.Padding(0);
            this.ChkEnabled.Name = "ChkEnabled";
            this.ChkEnabled.Size = new System.Drawing.Size(75, 30);
            this.ChkEnabled.SliderButtonWidth = 30;
            this.ChkEnabled.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkEnabled.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.ChkEnabled.StylizeFlag = false;
            this.ChkEnabled.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            this.ChkEnabled.TabIndex = 11;
            this.ChkEnabled.Text = "关";
            this.ChkEnabled.UseAnimation = true;
            // 
            // lblSource1
            // 
            this.lblSource1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.lblSource1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.lblSource1.BorderThickness = 0;
            this.lblSource1.CornerRadius = 0;
            this.lblSource1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblSource1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.lblSource1.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.lblSource1.Location = new System.Drawing.Point(359, 3);
            this.lblSource1.MultyLineFlag = false;
            this.lblSource1.Name = "lblSource1";
            this.lblSource1.Size = new System.Drawing.Size(85, 17);
            this.lblSource1.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.lblSource1.StylizeFlag = false;
            this.lblSource1.TabIndex = 12;
            this.lblSource1.TabStop = false;
            this.lblSource1.Text = "信源1";
            this.lblSource1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbxSource1
            // 
            this.cbxSource1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cbxSource1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cbxSource1.BorderThickness = 0;
            this.cbxSource1.CornerRadius = 0;
            this.cbxSource1.DataSource = null;
            this.cbxSource1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxSource1.DropDownHeight = 1000;
            this.cbxSource1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSource1.DropDownWidth = 95;
            this.cbxSource1.DropKey = System.Windows.Forms.Keys.Space;
            this.cbxSource1.DroppedDown = false;
            this.cbxSource1.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cbxSource1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cbxSource1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.cbxSource1.FormattingEnabled = true;
            this.cbxSource1.GetDisPlayName = null;
            this.cbxSource1.Height = 30;
            this.cbxSource1.ItemHeight = 28;
            this.cbxSource1.Items.AddRange(new object[] {
            "通用数字解调",
            "蓝牙信号",
            "802.11ad",
            "LTE",
            "5GNR",
            "OFDM"});
            this.cbxSource1.KeyDropEnble = true;
            this.cbxSource1.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.cbxSource1.Location = new System.Drawing.Point(359, 26);
            this.cbxSource1.MaxDropDownItems = 20;
            this.cbxSource1.Name = "cbxSource1";
            this.cbxSource1.RectBtnWidth = 20;
            this.cbxSource1.SelectedBackColor = System.Drawing.Color.Blue;
            this.cbxSource1.SelectedIndex = 0;
            this.cbxSource1.SelectedItem = "通用数字解调";
            this.cbxSource1.SelectedText = "";
            this.cbxSource1.Size = new System.Drawing.Size(85, 30);
            this.cbxSource1.Soreted = false;
            this.cbxSource1.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.cbxSource1.StylizeFlag = false;
            this.cbxSource1.TabIndex = 13;
            this.cbxSource1.Text = "通用数字解调";
            this.cbxSource1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.cbxSource1.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            // 
            // lblSource2
            // 
            this.lblSource2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.lblSource2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.lblSource2.BorderThickness = 0;
            this.lblSource2.CornerRadius = 0;
            this.lblSource2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblSource2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.lblSource2.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.lblSource2.Location = new System.Drawing.Point(359, 74);
            this.lblSource2.MultyLineFlag = false;
            this.lblSource2.Name = "lblSource2";
            this.lblSource2.Size = new System.Drawing.Size(85, 17);
            this.lblSource2.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.lblSource2.StylizeFlag = false;
            this.lblSource2.TabIndex = 14;
            this.lblSource2.TabStop = false;
            this.lblSource2.Text = "信源2";
            this.lblSource2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSource2.Visible = false;
            // 
            // cbxSource2
            // 
            this.cbxSource2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cbxSource2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cbxSource2.BorderThickness = 0;
            this.cbxSource2.CornerRadius = 0;
            this.cbxSource2.DataSource = null;
            this.cbxSource2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxSource2.DropDownHeight = 1000;
            this.cbxSource2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSource2.DropDownWidth = 95;
            this.cbxSource2.DropKey = System.Windows.Forms.Keys.Space;
            this.cbxSource2.DroppedDown = false;
            this.cbxSource2.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cbxSource2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cbxSource2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.cbxSource2.FormattingEnabled = true;
            this.cbxSource2.GetDisPlayName = null;
            this.cbxSource2.Height = 30;
            this.cbxSource2.ItemHeight = 28;
            this.cbxSource2.Items.AddRange(new object[] {
            "通用数字解调",
            "蓝牙信号",
            "802.11ad",
            "LTE",
            "5GNR",
            "OFDM"});
            this.cbxSource2.KeyDropEnble = true;
            this.cbxSource2.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.cbxSource2.Location = new System.Drawing.Point(359, 97);
            this.cbxSource2.MaxDropDownItems = 20;
            this.cbxSource2.Name = "cbxSource2";
            this.cbxSource2.RectBtnWidth = 20;
            this.cbxSource2.SelectedBackColor = System.Drawing.Color.Blue;
            this.cbxSource2.SelectedIndex = 1;
            this.cbxSource2.SelectedItem = "蓝牙信号";
            this.cbxSource2.SelectedText = "";
            this.cbxSource2.Size = new System.Drawing.Size(85, 30);
            this.cbxSource2.Soreted = false;
            this.cbxSource2.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.cbxSource2.StylizeFlag = false;
            this.cbxSource2.TabIndex = 15;
            this.cbxSource2.Text = "蓝牙信号";
            this.cbxSource2.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.cbxSource2.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.cbxSource2.Visible = false;
            // 
            // lblDemod
            // 
            this.lblDemod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.lblDemod.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.lblDemod.BorderThickness = 0;
            this.lblDemod.CornerRadius = 0;
            this.lblDemod.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblDemod.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.lblDemod.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.lblDemod.Location = new System.Drawing.Point(186, 74);
            this.lblDemod.MultyLineFlag = false;
            this.lblDemod.Name = "lblDemod";
            this.lblDemod.Size = new System.Drawing.Size(100, 17);
            this.lblDemod.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.lblDemod.StylizeFlag = false;
            this.lblDemod.TabIndex = 16;
            this.lblDemod.TabStop = false;
            this.lblDemod.Text = "解调图形";
            this.lblDemod.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbxDemod
            // 
            this.cbxDemod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cbxDemod.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cbxDemod.BorderThickness = 0;
            this.cbxDemod.CornerRadius = 0;
            this.cbxDemod.DataSource = null;
            this.cbxDemod.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxDemod.DropDownHeight = 1000;
            this.cbxDemod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDemod.DropDownWidth = 95;
            this.cbxDemod.DropKey = System.Windows.Forms.Keys.Space;
            this.cbxDemod.DroppedDown = false;
            this.cbxDemod.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cbxDemod.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cbxDemod.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.cbxDemod.FormattingEnabled = true;
            this.cbxDemod.GetDisPlayName = null;
            this.cbxDemod.Height = 30;
            this.cbxDemod.ItemHeight = 28;
            this.cbxDemod.Items.AddRange(new object[] {
            "IQ时域图",
            "星座图",
            "眼图",
            "矢量图",
            "符号表",
            "符号误差表",
            "相位误差时间图",
            "幅度误差时间图"});
            this.cbxDemod.KeyDropEnble = true;
            this.cbxDemod.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.cbxDemod.Location = new System.Drawing.Point(186, 97);
            this.cbxDemod.MaxDropDownItems = 20;
            this.cbxDemod.Name = "cbxDemod";
            this.cbxDemod.RectBtnWidth = 20;
            this.cbxDemod.SelectedBackColor = System.Drawing.Color.Blue;
            this.cbxDemod.SelectedIndex = 0;
            this.cbxDemod.SelectedItem = "IQ时域图";
            this.cbxDemod.SelectedText = "";
            this.cbxDemod.Size = new System.Drawing.Size(100, 30);
            this.cbxDemod.Soreted = false;
            this.cbxDemod.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.cbxDemod.StylizeFlag = false;
            this.cbxDemod.TabIndex = 17;
            this.cbxDemod.Text = "IQ时域图";
            this.cbxDemod.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.cbxDemod.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            // 
            // VsaSourceSettingPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lblDemod);
            this.Controls.Add(this.cbxDemod);
            this.Controls.Add(this.lblSource2);
            this.Controls.Add(this.cbxSource2);
            this.Controls.Add(this.lblSource1);
            this.Controls.Add(this.cbxSource1);
            this.Controls.Add(this.CbxTemplate);
            this.Controls.Add(this.LblSourceType);
            this.Controls.Add(this.LblEnabled);
            this.Controls.Add(this.CbxSignalType);
            this.Controls.Add(this.LblTemplate);
            this.Controls.Add(this.ChkEnabled);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "VsaSourceSettingPage";
            this.Size = new System.Drawing.Size(480, 140);
            this.ResumeLayout(false);

        }

        #endregion

        private ScopeX.UserControls.ComboBoxEx CbxTemplate;
        private ScopeX.UserControls.ScopeXLabel LblSourceType;
        private ScopeX.UserControls.ScopeXLabel LblEnabled;
        private ScopeX.UserControls.ComboBoxEx CbxSignalType;
        private ScopeX.UserControls.ScopeXLabel LblTemplate;
        private ScopeX.UserControls.ScopeXSwitchButton ChkEnabled;
        private ScopeX.UserControls.ScopeXLabel lblSource1;
        private ScopeX.UserControls.ComboBoxEx cbxSource1;
        private ScopeX.UserControls.ScopeXLabel lblSource2;
        private ScopeX.UserControls.ComboBoxEx cbxSource2;
        private ScopeX.UserControls.ScopeXLabel lblDemod;
        private ScopeX.UserControls.ComboBoxEx cbxDemod;
    }
}
