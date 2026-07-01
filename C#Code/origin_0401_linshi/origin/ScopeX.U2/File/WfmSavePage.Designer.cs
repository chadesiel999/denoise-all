
using ScopeX.UserControls;

namespace ScopeX.U2
{
    partial class WfmSavePage
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
            DefaultHighlightPrompt defaultHighlightPrompt1 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt2 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt3 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt4 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt5 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt6 = new DefaultHighlightPrompt();
            BtnSelectPath = new ScopeXIconButton();
            LabelPath = new ScopeXLabel();
            CbxPath = new ComboBoxEx();
            LabelFileName = new ScopeXLabel();
            LabelFileType = new ScopeXLabel();
            CbxFileType = new SelectComboBox();
            LabelSource = new ScopeXLabel();
            CbxSource = new SelectComboBox();
            BtnSave = new ScopeXIconButton();
            LblTime = new ScopeXLabel();
            ChkSuffix = new ScopeXSwitchButton();
            TbxFileName = new ScopeXTextBox();
            BtnSaveAndRecall = new ScopeXIconButton();
            BtnOpenDir = new ScopeXIconButton();
            LabelWfmTxtFormat = new ScopeXLabel();
            CbxWfmTxtFormat = new UserControls.SelectComboBox();
            SuspendLayout();
            // 
            // BtnSelectPath
            // 
            BtnSelectPath.BackColor = System.Drawing.Color.Transparent;
            BtnSelectPath.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSelectPath.BorderThickness = 0;
            BtnSelectPath.CornerRadius = 0;
            BtnSelectPath.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSelectPath.DaskArray = null;
            BtnSelectPath.DropKey = System.Windows.Forms.Keys.Space;
            BtnSelectPath.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnSelectPath.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnSelectPath.Height = 40;
            BtnSelectPath.Icon = Properties.Resources.FolderBrowser;
            BtnSelectPath.IconOffset = 0;
            BtnSelectPath.IconSize = new System.Drawing.Size(30, 30);
            BtnSelectPath.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSelectPath.IsChoosed = false;
            BtnSelectPath.IsIndicatorShow = false;
            BtnSelectPath.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSelectPath.Location = new System.Drawing.Point(378, 102);
            BtnSelectPath.MouseinBackColor = System.Drawing.Color.Transparent;
            BtnSelectPath.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSelectPath.MouseInBorderThickness = 0;
            BtnSelectPath.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSelectPath.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSelectPath.Name = "BtnSelectPath";
            BtnSelectPath.PressedBackColor = System.Drawing.Color.Transparent;
            BtnSelectPath.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSelectPath.PressedBorderThickness = 0;
            BtnSelectPath.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSelectPath.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSelectPath.Size = new System.Drawing.Size(40, 40);
            BtnSelectPath.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnSelectPath.StylizeFlag = false;
            BtnSelectPath.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSelectPath.SVGPath = "";
            BtnSelectPath.TabIndex = 8;
            BtnSelectPath.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSelectPath.Click += BtnSelectPath_Click;
            // 
            // LabelPath
            // 
            LabelPath.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LabelPath.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LabelPath.BorderThickness = 0;
            LabelPath.CornerRadius = 0;
            LabelPath.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LabelPath.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LabelPath.HighlightPrompt = defaultHighlightPrompt1;
            LabelPath.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LabelPath.Location = new System.Drawing.Point(10, 74);
            LabelPath.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            LabelPath.MultyLineFlag = false;
            LabelPath.Name = "LabelPath";
            LabelPath.Size = new System.Drawing.Size(343, 18);
            LabelPath.StyleFlags = UserControls.Style.StyleFlag.None;
            LabelPath.StylizeFlag = true;
            LabelPath.TabIndex = 6;
            LabelPath.TabStop = false;
            LabelPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LabelPath.Token = null;
            // 
            // CbxPath
            // 
            CbxPath.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxPath.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxPath.BorderThickness = 0;
            CbxPath.ContainerBackColor = null;
            CbxPath.ContainerForeColor = null;
            CbxPath.CornerRadius = 0;
            CbxPath.DataSource = null;
            CbxPath.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            CbxPath.DropDownHeight = 200;
            CbxPath.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CbxPath.DropDownWidth = 343;
            CbxPath.DropKey = System.Windows.Forms.Keys.Space;
            CbxPath.DroppedDown = false;
            CbxPath.FocusColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxPath.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxPath.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            CbxPath.GetDisPlayName = null;
            CbxPath.Height = 30;
            CbxPath.ImageMode = false;
            CbxPath.ItemHeight = 28;
            CbxPath.KeyDropEnble = true;
            CbxPath.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            CbxPath.Location = new System.Drawing.Point(10, 102);
            CbxPath.MaxDropDownItems = 8;
            CbxPath.Name = "CbxPath";
            CbxPath.RectBtnWidth = 20;
            CbxPath.SelectedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            CbxPath.SelectedIndex = -1;
            CbxPath.SelectedItem = null;
            CbxPath.SelectedText = "";
            CbxPath.Size = new System.Drawing.Size(363, 30);
            CbxPath.Soreted = false;
            CbxPath.StyleFlags = UserControls.Style.StyleFlag.None;
            CbxPath.StylizeFlag = true;
            CbxPath.TabIndex = 7;
            CbxPath.Text = "";
            CbxPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            CbxPath.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            // 
            // LabelFileName
            // 
            LabelFileName.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LabelFileName.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LabelFileName.BorderThickness = 0;
            LabelFileName.CornerRadius = 0;
            LabelFileName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LabelFileName.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LabelFileName.HighlightPrompt = defaultHighlightPrompt2;
            LabelFileName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LabelFileName.Location = new System.Drawing.Point(10, 150);
            LabelFileName.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            LabelFileName.MultyLineFlag = false;
            LabelFileName.Name = "LabelFileName";
            LabelFileName.Size = new System.Drawing.Size(343, 18);
            LabelFileName.StyleFlags = UserControls.Style.StyleFlag.None;
            LabelFileName.StylizeFlag = true;
            LabelFileName.TabIndex = 9;
            LabelFileName.TabStop = false;
            LabelFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LabelFileName.Token = null;
            // 
            // LabelFileType
            // 
            LabelFileType.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LabelFileType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LabelFileType.BorderThickness = 0;
            LabelFileType.CornerRadius = 0;
            LabelFileType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LabelFileType.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LabelFileType.HighlightPrompt = defaultHighlightPrompt3;
            LabelFileType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LabelFileType.Location = new System.Drawing.Point(10, 2);
            LabelFileType.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            LabelFileType.MultyLineFlag = false;
            LabelFileType.Name = "LabelFileType";
            LabelFileType.Size = new System.Drawing.Size(103, 18);
            LabelFileType.StyleFlags = UserControls.Style.StyleFlag.None;
            LabelFileType.StylizeFlag = true;
            LabelFileType.TabIndex = 0;
            LabelFileType.TabStop = false;
            LabelFileType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LabelFileType.Token = null;
            // 
            // CbxFileType
            // 
            CbxFileType.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxFileType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxFileType.DataSource = null;
            CbxFileType.ExtText = "";
            CbxFileType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxFileType.ForeColor = System.Drawing.Color.White;
            CbxFileType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            CbxFileType.Location = new System.Drawing.Point(10, 30);
            CbxFileType.MaximumSize = new System.Drawing.Size(99999, 99999);
            CbxFileType.Name = "CbxFileType";
            CbxFileType.SelectIndex = -1;
            CbxFileType.SelectValue = null;
            CbxFileType.Size = new System.Drawing.Size(140, 30);
            CbxFileType.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxFileType.StylizeFlag = true;
            CbxFileType.TabIndex = 16;
            // 
            // LabelSource
            // 
            LabelSource.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LabelSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LabelSource.BorderThickness = 0;
            LabelSource.CornerRadius = 0;
            LabelSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LabelSource.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LabelSource.HighlightPrompt = defaultHighlightPrompt4;
            LabelSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LabelSource.Location = new System.Drawing.Point(418, 2);
            LabelSource.Margin = new System.Windows.Forms.Padding(2);
            LabelSource.MultyLineFlag = false;
            LabelSource.Name = "LabelSource";
            LabelSource.Size = new System.Drawing.Size(103, 18);
            LabelSource.StyleFlags = UserControls.Style.StyleFlag.None;
            LabelSource.StylizeFlag = true;
            LabelSource.TabIndex = 4;
            LabelSource.TabStop = false;
            LabelSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LabelSource.Token = null;
            // 
            // CbxSource
            // 
            CbxSource.AutoSize = true;
            CbxSource.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.ComBorderColor = System.Drawing.Color.Blue;
            CbxSource.DataSource = null;
            CbxSource.ExtText = "";
            CbxSource.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxSource.ForeColor = System.Drawing.Color.White;
            CbxSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            CbxSource.Location = new System.Drawing.Point(418, 30);
            CbxSource.MaximumSize = new System.Drawing.Size(99999, 99999);
            CbxSource.Name = "CbxSource";
            CbxSource.SelectIndex = -1;
            CbxSource.SelectValue = null;
            CbxSource.Size = new System.Drawing.Size(103, 30);
            CbxSource.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxSource.StylizeFlag = true;
            CbxSource.TabIndex = 17;
            // 
            // BtnSave
            // 
            BtnSave.BackColor = System.Drawing.Color.Transparent;
            BtnSave.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSave.BorderThickness = 0;
            BtnSave.CornerRadius = 0;
            BtnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSave.DaskArray = null;
            BtnSave.DropKey = System.Windows.Forms.Keys.Space;
            BtnSave.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnSave.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnSave.Height = 30;
            BtnSave.Icon = null;
            BtnSave.IconOffset = 10;
            BtnSave.IconSize = new System.Drawing.Size(24, 24);
            BtnSave.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSave.IsChoosed = false;
            BtnSave.IsIndicatorShow = false;
            BtnSave.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSave.Location = new System.Drawing.Point(248, 246);
            BtnSave.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSave.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSave.MouseInBorderThickness = 0;
            BtnSave.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSave.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSave.Name = "BtnSave";
            BtnSave.PressedBackColor = System.Drawing.Color.Gray;
            BtnSave.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSave.PressedBorderThickness = 0;
            BtnSave.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSave.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSave.Size = new System.Drawing.Size(90, 30);
            BtnSave.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnSave.StylizeFlag = true;
            BtnSave.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSave.SVGPath = "";
            BtnSave.TabIndex = 14;
            BtnSave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSave.Click += BtnSave_Click;
            // 
            // LblTime
            // 
            LblTime.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblTime.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblTime.BorderThickness = 0;
            LblTime.CornerRadius = 0;
            LblTime.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblTime.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblTime.HighlightPrompt = defaultHighlightPrompt5;
            LblTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTime.Location = new System.Drawing.Point(378, 150);
            LblTime.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            LblTime.MultyLineFlag = false;
            LblTime.Name = "LblTime";
            LblTime.Size = new System.Drawing.Size(100, 18);
            LblTime.StyleFlags = UserControls.Style.StyleFlag.None;
            LblTime.StylizeFlag = true;
            LblTime.TabIndex = 11;
            LblTime.TabStop = false;
            LblTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblTime.Token = null;
            // 
            // ChkSuffix
            // 
            ChkSuffix.AnimationCount = 8;
            ChkSuffix.AnimationFunc = null;
            ChkSuffix.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkSuffix.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkSuffix.BorderThickness = 0;
            ChkSuffix.Checked = false;
            ChkSuffix.CheckedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            ChkSuffix.CheckedForeColor = System.Drawing.Color.Black;
            ChkSuffix.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkSuffix.CheckedText = "开";
            ChkSuffix.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkSuffix.DropKey = System.Windows.Forms.Keys.Space;
            ChkSuffix.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkSuffix.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkSuffix.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkSuffix.Height = 30;
            ChkSuffix.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkSuffix.Location = new System.Drawing.Point(378, 178);
            ChkSuffix.Margin = new System.Windows.Forms.Padding(0);
            ChkSuffix.Name = "ChkSuffix";
            ChkSuffix.Size = new System.Drawing.Size(100, 30);
            ChkSuffix.SliderButtonWidth = 30;
            ChkSuffix.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkSuffix.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkSuffix.StylizeFlag = true;
            ChkSuffix.SwitchShape = ScopeXSwitchButton.Shape.Square;
            ChkSuffix.TabIndex = 12;
            ChkSuffix.Text = "关";
            ChkSuffix.UseAnimation = true;
            ChkSuffix.CheckedChangedEvent += ChkSuffix_CheckedChangedEvent;
            // 
            // TbxFileName
            // 
            TbxFileName.AcceptsTab = false;
            TbxFileName.AutoShowKeyBoard = true;
            TbxFileName.AutoSize = false;
            TbxFileName.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxFileName.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxFileName.BorderThickness = 0;
            TbxFileName.CornerRadius = 0;
            TbxFileName.Cursor = System.Windows.Forms.Cursors.IBeam;
            TbxFileName.Enabled = true;
            TbxFileName.EnbleSelectBorder = true;
            TbxFileName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbxFileName.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            TbxFileName.Height = 30;
            TbxFileName.HideSelection = true;
            TbxFileName.KeyboardVerify = null;
            TbxFileName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TbxFileName.Location = new System.Drawing.Point(10, 178);
            TbxFileName.MaxLength = 32767;
            TbxFileName.Modified = false;
            TbxFileName.MouseEnterState = false;
            TbxFileName.Multiline = false;
            TbxFileName.Name = "TbxFileName";
            TbxFileName.ProcessCmdKeyFunc = null;
            TbxFileName.ReadOnly = false;
            TbxFileName.SelectedColor = System.Drawing.Color.FromArgb(40, 71, 193);
            TbxFileName.SelectedText = "";
            TbxFileName.SelectionLength = 0;
            TbxFileName.SelectionStart = 0;
            TbxFileName.ShortcutsEnabled = true;
            TbxFileName.Size = new System.Drawing.Size(363, 30);
            TbxFileName.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxFileName.StylizeFlag = true;
            TbxFileName.TabIndex = 10;
            TbxFileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            TbxFileName.UseSystemPasswordChar = false;
            TbxFileName.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            TbxFileName.WordWrap = true;
            TbxFileName.TextChanged += TbxFileName_TextChanged;
            // 
            // BtnSaveAndRecall
            // 
            BtnSaveAndRecall.BackColor = System.Drawing.Color.Transparent;
            BtnSaveAndRecall.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveAndRecall.BorderThickness = 0;
            BtnSaveAndRecall.CornerRadius = 0;
            BtnSaveAndRecall.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSaveAndRecall.DaskArray = null;
            BtnSaveAndRecall.DropKey = System.Windows.Forms.Keys.Space;
            BtnSaveAndRecall.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnSaveAndRecall.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnSaveAndRecall.Height = 30;
            BtnSaveAndRecall.Icon = null;
            BtnSaveAndRecall.IconOffset = 10;
            BtnSaveAndRecall.IconSize = new System.Drawing.Size(24, 24);
            BtnSaveAndRecall.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSaveAndRecall.IsChoosed = false;
            BtnSaveAndRecall.IsIndicatorShow = false;
            BtnSaveAndRecall.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSaveAndRecall.Location = new System.Drawing.Point(408, 246);
            BtnSaveAndRecall.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveAndRecall.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveAndRecall.MouseInBorderThickness = 0;
            BtnSaveAndRecall.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveAndRecall.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSaveAndRecall.Name = "BtnSaveAndRecall";
            BtnSaveAndRecall.PressedBackColor = System.Drawing.Color.Gray;
            BtnSaveAndRecall.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveAndRecall.PressedBorderThickness = 0;
            BtnSaveAndRecall.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveAndRecall.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSaveAndRecall.Size = new System.Drawing.Size(120, 30);
            BtnSaveAndRecall.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnSaveAndRecall.StylizeFlag = true;
            BtnSaveAndRecall.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveAndRecall.SVGPath = "";
            BtnSaveAndRecall.TabIndex = 15;
            BtnSaveAndRecall.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSaveAndRecall.Click += BtnSaveAndRecall_Click;
            BtnSaveAndRecall.Visible=false;
            // 
            // BtnOpenDir
            // 
            BtnOpenDir.BackColor = System.Drawing.Color.Transparent;
            BtnOpenDir.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOpenDir.BorderThickness = 0;
            BtnOpenDir.CornerRadius = 0;
            BtnOpenDir.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnOpenDir.DaskArray = null;
            BtnOpenDir.DropKey = System.Windows.Forms.Keys.Space;
            BtnOpenDir.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnOpenDir.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnOpenDir.Height = 30;
            BtnOpenDir.Icon = Properties.Resources.OpenFolder;
            BtnOpenDir.IconOffset = 5;
            BtnOpenDir.IconSize = new System.Drawing.Size(26, 26);
            BtnOpenDir.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnOpenDir.IsChoosed = false;
            BtnOpenDir.IsIndicatorShow = false;
            BtnOpenDir.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnOpenDir.Location = new System.Drawing.Point(10, 246);
            BtnOpenDir.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 5);
            BtnOpenDir.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOpenDir.MouseInBorderThickness = 0;
            BtnOpenDir.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOpenDir.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnOpenDir.Name = "BtnOpenDir";
            BtnOpenDir.PressedBackColor = System.Drawing.Color.Gray;
            BtnOpenDir.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOpenDir.PressedBorderThickness = 0;
            BtnOpenDir.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOpenDir.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnOpenDir.Size = new System.Drawing.Size(204, 30);
            BtnOpenDir.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnOpenDir.StylizeFlag = true;
            BtnOpenDir.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOpenDir.SVGPath = "";
            BtnOpenDir.TabIndex = 13;
            BtnOpenDir.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnOpenDir.Click += BtnOpenDic_Click;
            // 
            // LabelWfmTxtFormat
            // 
            LabelWfmTxtFormat.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LabelWfmTxtFormat.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LabelWfmTxtFormat.BorderThickness = 0;
            LabelWfmTxtFormat.CornerRadius = 0;
            LabelWfmTxtFormat.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LabelWfmTxtFormat.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LabelWfmTxtFormat.HighlightPrompt = defaultHighlightPrompt6;
            LabelWfmTxtFormat.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LabelWfmTxtFormat.Location = new System.Drawing.Point(220, 2);
            LabelWfmTxtFormat.Margin = new System.Windows.Forms.Padding(2);
            LabelWfmTxtFormat.MultyLineFlag = false;
            LabelWfmTxtFormat.Name = "LabelWfmTxtFormat";
            LabelWfmTxtFormat.Size = new System.Drawing.Size(103, 18);
            LabelWfmTxtFormat.StyleFlags = UserControls.Style.StyleFlag.None;
            LabelWfmTxtFormat.StylizeFlag = true;
            LabelWfmTxtFormat.TabIndex = 2;
            LabelWfmTxtFormat.TabStop = false;
            LabelWfmTxtFormat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LabelWfmTxtFormat.Token = null;
            // 
            // CbxWfmTxtFormat
            // 
            CbxWfmTxtFormat.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxWfmTxtFormat.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxWfmTxtFormat.DataSource = null;
            CbxWfmTxtFormat.ExtText = "";
            CbxWfmTxtFormat.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxWfmTxtFormat.ForeColor = System.Drawing.Color.White;
            CbxWfmTxtFormat.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            CbxWfmTxtFormat.Location = new System.Drawing.Point(220, 30);
            CbxWfmTxtFormat.MaximumSize = new System.Drawing.Size(99999, 99999);
            CbxWfmTxtFormat.Name = "CbxWfmTxtFormat";
            CbxWfmTxtFormat.SelectIndex = -1;
            CbxWfmTxtFormat.SelectValue = null;
            CbxWfmTxtFormat.Size = new System.Drawing.Size(103, 30);
            CbxWfmTxtFormat.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxWfmTxtFormat.StylizeFlag = true;
            CbxWfmTxtFormat.TabIndex = 18;
            // 
            // WfmSavePage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(CbxWfmTxtFormat);
            Controls.Add(LabelWfmTxtFormat);
            Controls.Add(BtnOpenDir);
            Controls.Add(BtnSaveAndRecall);
            Controls.Add(TbxFileName);
            Controls.Add(ChkSuffix);
            Controls.Add(BtnSave);
            Controls.Add(LabelSource);
            Controls.Add(CbxSource);
            Controls.Add(CbxFileType);
            Controls.Add(CbxPath);
            Controls.Add(LabelFileType);
            Controls.Add(LblTime);
            Controls.Add(LabelFileName);
            Controls.Add(LabelPath);
            Controls.Add(BtnSelectPath);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "WfmSavePage";
            Size = new System.Drawing.Size(539, 390);
            ResumeLayout(false);
        }

        #endregion

        private ScopeXIconButton BtnSelectPath;
        private ScopeXLabel LabelPath;
        private ComboBoxEx CbxPath;
        private ScopeXLabel LabelFileName;
        private ScopeXLabel LabelFileType;
        private ScopeXLabel LabelSource;
        private ScopeXIconButton BtnSave;
        private ScopeXLabel LblTime;
        private ScopeXSwitchButton ChkSuffix;
        private ScopeXTextBox TbxFileName;
        private ScopeXIconButton BtnSaveAndRecall;
        private ScopeXIconButton BtnOpenDir;
        private ScopeXLabel LabelWfmTxtFormat;
        //private ComboBoxEx CbxWfmTxtFormat;
        private SelectComboBox CbxFileType;
        private SelectComboBox CbxSource;
        private SelectComboBox CbxWfmTxtFormat;
    }
}
