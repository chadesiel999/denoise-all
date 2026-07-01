
using ScopeX.UserControls;

namespace ScopeX.U2
{
    partial class LongStorageWfmSavePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LongStorageWfmSavePage));
            DefaultHighlightPrompt defaultHighlightPrompt1 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt2 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt3 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt4 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt5 = new DefaultHighlightPrompt();
            RadioButtonItem radioButtonItem1 = new RadioButtonItem();
            RadioButtonItem radioButtonItem2 = new RadioButtonItem();
            RadioButtonItem radioButtonItem3 = new RadioButtonItem();
            RadioButtonItem radioButtonItem4 = new RadioButtonItem();
            BtnSaveOriginData = new ScopeXIconButton();
            CbxSaveFormat = new SelectComboBox();
            LblSaveFormat = new ScopeXLabel();
            BtnSelectPath = new ScopeXIconButton();
            LabelPath = new ScopeXLabel();
            LabelFileName = new ScopeXLabel();
            LblTime = new ScopeXLabel();
            CbxPath = new ComboBoxEx();
            LabelSource = new ScopeXLabel();
            ChkSuffix = new ScopeXSwitchButton();
            TbxFileName = new ScopeXTextBox();
            BtnOpenDir = new ScopeXIconButton();
            RdoSource = new UIRadioButtonGroup();
            SuspendLayout();
            // 
            // BtnSaveOriginData
            // 
            BtnSaveOriginData.BackColor = System.Drawing.Color.Transparent;
            BtnSaveOriginData.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveOriginData.BorderThickness = 0;
            BtnSaveOriginData.CornerRadius = 0;
            BtnSaveOriginData.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSaveOriginData.DaskArray = null;
            BtnSaveOriginData.DropKey = System.Windows.Forms.Keys.Space;
            BtnSaveOriginData.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnSaveOriginData.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnSaveOriginData.Height = 30;
            BtnSaveOriginData.Icon = null;
            BtnSaveOriginData.IconOffset = 10;
            BtnSaveOriginData.IconSize = new System.Drawing.Size(24, 24);
            BtnSaveOriginData.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSaveOriginData.IsChoosed = false;
            BtnSaveOriginData.IsIndicatorShow = false;
            BtnSaveOriginData.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSaveOriginData.Location = new System.Drawing.Point(337, 246);
            BtnSaveOriginData.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveOriginData.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveOriginData.MouseInBorderThickness = 0;
            BtnSaveOriginData.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveOriginData.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSaveOriginData.Name = "BtnSaveOriginData";
            BtnSaveOriginData.PressedBackColor = System.Drawing.Color.Gray;
            BtnSaveOriginData.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveOriginData.PressedBorderThickness = 0;
            BtnSaveOriginData.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveOriginData.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSaveOriginData.Size = new System.Drawing.Size(160, 30);
            BtnSaveOriginData.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnSaveOriginData.StylizeFlag = true;
            BtnSaveOriginData.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveOriginData.SVGPath = "";
            BtnSaveOriginData.TabIndex = 19;
            BtnSaveOriginData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSaveOriginData.Click += BtnSaveOriginData_Click;
            // 
            // CbxSaveFormat
            // 
            CbxSaveFormat.AutoSize = true;
            CbxSaveFormat.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSaveFormat.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSaveFormat.ComBorderColor = System.Drawing.Color.Blue;
            CbxSaveFormat.DataSource = (System.Collections.IList)resources.GetObject("CbxSaveFormat.DataSource");
            CbxSaveFormat.ExtText = "";
            CbxSaveFormat.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxSaveFormat.ForeColor = System.Drawing.Color.White;
            CbxSaveFormat.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            CbxSaveFormat.Location = new System.Drawing.Point(284, 31);
            CbxSaveFormat.MaximumSize = new System.Drawing.Size(99999, 99999);
            CbxSaveFormat.Name = "CbxSaveFormat";
            CbxSaveFormat.SelectIndex = -1;
            CbxSaveFormat.SelectValue = null;
            CbxSaveFormat.Size = new System.Drawing.Size(133, 30);
            CbxSaveFormat.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxSaveFormat.StylizeFlag = true;
            CbxSaveFormat.TabIndex = 20;
            // 
            // LblSaveFormat
            // 
            LblSaveFormat.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblSaveFormat.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblSaveFormat.BorderThickness = 0;
            LblSaveFormat.CornerRadius = 0;
            LblSaveFormat.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSaveFormat.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblSaveFormat.HighlightPrompt = defaultHighlightPrompt1;
            LblSaveFormat.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSaveFormat.Location = new System.Drawing.Point(284, 4);
            LblSaveFormat.Margin = new System.Windows.Forms.Padding(2);
            LblSaveFormat.MultyLineFlag = false;
            LblSaveFormat.Name = "LblSaveFormat";
            LblSaveFormat.Size = new System.Drawing.Size(103, 18);
            LblSaveFormat.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSaveFormat.StylizeFlag = true;
            LblSaveFormat.TabIndex = 21;
            LblSaveFormat.TabStop = false;
            LblSaveFormat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSaveFormat.Token = null;
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
            BtnSelectPath.Location = new System.Drawing.Point(422, 102);
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
            LabelPath.HighlightPrompt = defaultHighlightPrompt2;
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
            // LabelFileName
            // 
            LabelFileName.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LabelFileName.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LabelFileName.BorderThickness = 0;
            LabelFileName.CornerRadius = 0;
            LabelFileName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LabelFileName.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LabelFileName.HighlightPrompt = defaultHighlightPrompt3;
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
            // LblTime
            // 
            LblTime.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblTime.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblTime.BorderThickness = 0;
            LblTime.CornerRadius = 0;
            LblTime.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblTime.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblTime.HighlightPrompt = defaultHighlightPrompt4;
            LblTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTime.Location = new System.Drawing.Point(422, 150);
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
            CbxPath.Size = new System.Drawing.Size(403, 30);
            CbxPath.Soreted = false;
            CbxPath.StyleFlags = UserControls.Style.StyleFlag.None;
            CbxPath.StylizeFlag = true;
            CbxPath.TabIndex = 7;
            CbxPath.Text = "";
            CbxPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            CbxPath.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            // 
            // LabelSource
            // 
            LabelSource.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LabelSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LabelSource.BorderThickness = 0;
            LabelSource.CornerRadius = 0;
            LabelSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LabelSource.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LabelSource.HighlightPrompt = defaultHighlightPrompt5;
            LabelSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LabelSource.Location = new System.Drawing.Point(10, 4);
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
            ChkSuffix.Location = new System.Drawing.Point(422, 178);
            ChkSuffix.Margin = new System.Windows.Forms.Padding(0);
            ChkSuffix.Name = "ChkSuffix";
            ChkSuffix.Size = new System.Drawing.Size(80, 30);
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
            TbxFileName.Size = new System.Drawing.Size(402, 30);
            TbxFileName.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxFileName.StylizeFlag = true;
            TbxFileName.TabIndex = 10;
            TbxFileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            TbxFileName.UseSystemPasswordChar = false;
            TbxFileName.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            TbxFileName.WordWrap = true;
            TbxFileName.TextChanged += TbxFileName_TextChanged;
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
            // RdoSource
            // 
            RdoSource.BackColor = System.Drawing.Color.AliceBlue;
            RdoSource.BorderColor = System.Drawing.Color.AliceBlue;
            RdoSource.BorderThickness = 0;
            RdoSource.ButtonBackColor = System.Drawing.Color.FromArgb(50, 50, 54);
            RdoSource.ButtonFont = null;
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = "C1";
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = "C2";
            radioButtonItem3.Icon = null;
            radioButtonItem3.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem3.Tag = null;
            radioButtonItem3.Text = "C3";
            radioButtonItem4.Icon = null;
            radioButtonItem4.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem4.Tag = null;
            radioButtonItem4.Text = "C4";
            RdoSource.ButtonItems = new RadioButtonItem[]
    {
    radioButtonItem1,
    radioButtonItem2,
    radioButtonItem3,
    radioButtonItem4
    };
            RdoSource.ButtonOffset = 10;
            RdoSource.ButtonTextColor = System.Drawing.Color.White;
            RdoSource.ChoosedButtonColor = System.Drawing.Color.FromArgb(18, 183, 245);
            RdoSource.ChoosedButtonIndex = 0;
            RdoSource.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoSource.ContentBackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            RdoSource.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoSource.FocusBorderColor = System.Drawing.Color.White;
            RdoSource.ForeColor = System.Drawing.Color.White;
            RdoSource.Height = 35;
            RdoSource.IsMutiSelect = true;
            RdoSource.IsRemoveSelected = true;
            RdoSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoSource.Location = new System.Drawing.Point(10, 32);
            RdoSource.Name = "RdoSource";
            RdoSource.Size = new System.Drawing.Size(255, 35);
            RdoSource.StyleFlags = UserControls.Style.StyleFlag.None;
            RdoSource.StylizeFlag = true;
            RdoSource.TabIndex = 22;
            // 
            // LongStorageWfmSavePage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(RdoSource);
            Controls.Add(LblSaveFormat);
            Controls.Add(CbxSaveFormat);
            Controls.Add(BtnSaveOriginData);
            Controls.Add(BtnOpenDir);
            Controls.Add(TbxFileName);
            Controls.Add(ChkSuffix);
            Controls.Add(LabelSource);
            Controls.Add(CbxPath);
            Controls.Add(LblTime);
            Controls.Add(LabelFileName);
            Controls.Add(LabelPath);
            Controls.Add(BtnSelectPath);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "LongStorageWfmSavePage";
            Size = new System.Drawing.Size(539, 390);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ScopeXIconButton BtnSaveOriginData;
        private SelectComboBox CbxSaveFormat;
        private ScopeXLabel LblSaveFormat;
        private ScopeXIconButton BtnSelectPath;
        private ScopeXLabel LabelPath;
        private ScopeXLabel LabelFileName;
        private ScopeXLabel LblTime;
        private ComboBoxEx CbxPath;
        private ScopeXLabel LabelSource;
        private ScopeXSwitchButton ChkSuffix;
        private ScopeXTextBox TbxFileName;
        private ScopeXIconButton BtnOpenDir;
        private UIRadioButtonGroup RdoSource;
    }
}
