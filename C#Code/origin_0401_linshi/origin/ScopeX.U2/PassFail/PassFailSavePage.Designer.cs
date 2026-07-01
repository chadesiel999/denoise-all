
namespace ScopeX.U2.PassFail
{
    partial class PassFailSavePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PassFailSavePage));
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt5 = new UserControls.DefaultHighlightPrompt();
            TbxFileName = new UserControls.ScopeXTextBox();
            ChkSuffix = new UserControls.ScopeXSwitchButton();
            CbxFileType = new UserControls.SelectComboBox();
            CbxPath = new UserControls.ComboBoxEx();
            LabelFileType = new UserControls.ScopeXLabel();
            LblTime = new UserControls.ScopeXLabel();
            LabelFileName = new UserControls.ScopeXLabel();
            LabelPath = new UserControls.ScopeXLabel();
            BtnSelectPath = new UserControls.ScopeXIconButton();
            CbxPicType = new UserControls.SelectComboBox();
            LabelPicType = new UserControls.ScopeXLabel();
            SuspendLayout();
            // 
            // TbxFileName
            // 
            TbxFileName.AcceptsTab = false;
            TbxFileName.Adjustable = false;
            TbxFileName.AutoShowKeyBoard = true;
            TbxFileName.AutoSize = false;
            TbxFileName.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxFileName.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxFileName.BorderThickness = 0;
            TbxFileName.ClickedBorderColor = System.Drawing.Color.White;
            TbxFileName.CornerRadius = 0;
            TbxFileName.Cursor = System.Windows.Forms.Cursors.IBeam;
            TbxFileName.DoubleClickEnable = false;
            TbxFileName.Enabled = true;
            TbxFileName.EnbleSelectBorder = true;
            TbxFileName.FineEnable = false;
            TbxFileName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbxFileName.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            TbxFileName.Height = 30;
            TbxFileName.HideSelection = true;
            TbxFileName.IsFocusClicked = false;
            TbxFileName.KeyboardVerify = null;
            TbxFileName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TbxFileName.Location = new System.Drawing.Point(35, 188);
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
            TbxFileName.TabIndex = 22;
            TbxFileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            TbxFileName.UseSystemPasswordChar = false;
            TbxFileName.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            TbxFileName.WordWrap = true;
            TbxFileName.TextChanged += TbxFileName_TextChanged;
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
            ChkSuffix.Location = new System.Drawing.Point(403, 188);
            ChkSuffix.Margin = new System.Windows.Forms.Padding(0);
            ChkSuffix.Name = "ChkSuffix";
            ChkSuffix.Size = new System.Drawing.Size(100, 30);
            ChkSuffix.SliderButtonWidth = 30;
            ChkSuffix.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkSuffix.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkSuffix.StylizeFlag = true;
            ChkSuffix.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkSuffix.TabIndex = 24;
            ChkSuffix.Text = "关";
            ChkSuffix.UseAnimation = true;
            ChkSuffix.CheckedChangedEvent += ChkSuffix_CheckedChangedEvent;
            // 
            // CbxFileType
            // 
            CbxFileType.AutoSize = true;
            CbxFileType.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxFileType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxFileType.ComBorderColor = System.Drawing.Color.Blue;
            CbxFileType.DataSource = (System.Collections.IList)resources.GetObject("CbxFileType.DataSource");
            CbxFileType.ExtText = "";
            CbxFileType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxFileType.ForeColor = System.Drawing.Color.White;
            CbxFileType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            CbxFileType.Location = new System.Drawing.Point(35, 40);
            CbxFileType.MaximumSize = new System.Drawing.Size(99999, 99999);
            CbxFileType.Name = "CbxFileType";
            CbxFileType.SelectIndex = -1;
            CbxFileType.SelectValue = null;
            CbxFileType.Size = new System.Drawing.Size(140, 30);
            CbxFileType.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxFileType.StylizeFlag = true;
            CbxFileType.TabIndex = 25;
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
            CbxPath.DropDownWidth = 363;
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
            CbxPath.Location = new System.Drawing.Point(35, 112);
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
            CbxPath.TabIndex = 19;
            CbxPath.Text = "";
            CbxPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            CbxPath.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            // 
            // LabelFileType
            // 
            LabelFileType.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LabelFileType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LabelFileType.BorderThickness = 0;
            LabelFileType.CornerRadius = 0;
            LabelFileType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LabelFileType.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LabelFileType.HighlightPrompt = defaultHighlightPrompt1;
            LabelFileType.IsOmittext = true;
            LabelFileType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LabelFileType.Location = new System.Drawing.Point(35, 12);
            LabelFileType.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            LabelFileType.MultyLineFlag = false;
            LabelFileType.Name = "LabelFileType";
            LabelFileType.Size = new System.Drawing.Size(103, 18);
            LabelFileType.StyleFlags = UserControls.Style.StyleFlag.None;
            LabelFileType.StylizeFlag = true;
            LabelFileType.TabIndex = 17;
            LabelFileType.TabStop = false;
            LabelFileType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LabelFileType.Token = null;
            // 
            // LblTime
            // 
            LblTime.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblTime.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblTime.BorderThickness = 0;
            LblTime.CornerRadius = 0;
            LblTime.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblTime.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblTime.HighlightPrompt = defaultHighlightPrompt2;
            LblTime.IsOmittext = true;
            LblTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTime.Location = new System.Drawing.Point(403, 160);
            LblTime.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            LblTime.MultyLineFlag = false;
            LblTime.Name = "LblTime";
            LblTime.Size = new System.Drawing.Size(100, 18);
            LblTime.StyleFlags = UserControls.Style.StyleFlag.None;
            LblTime.StylizeFlag = true;
            LblTime.TabIndex = 23;
            LblTime.TabStop = false;
            LblTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblTime.Token = null;
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
            LabelFileName.IsOmittext = true;
            LabelFileName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LabelFileName.Location = new System.Drawing.Point(35, 160);
            LabelFileName.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            LabelFileName.MultyLineFlag = false;
            LabelFileName.Name = "LabelFileName";
            LabelFileName.Size = new System.Drawing.Size(343, 18);
            LabelFileName.StyleFlags = UserControls.Style.StyleFlag.None;
            LabelFileName.StylizeFlag = true;
            LabelFileName.TabIndex = 21;
            LabelFileName.TabStop = false;
            LabelFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LabelFileName.Token = null;
            // 
            // LabelPath
            // 
            LabelPath.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LabelPath.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LabelPath.BorderThickness = 0;
            LabelPath.CornerRadius = 0;
            LabelPath.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LabelPath.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LabelPath.HighlightPrompt = defaultHighlightPrompt4;
            LabelPath.IsOmittext = true;
            LabelPath.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LabelPath.Location = new System.Drawing.Point(35, 84);
            LabelPath.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            LabelPath.MultyLineFlag = false;
            LabelPath.Name = "LabelPath";
            LabelPath.Size = new System.Drawing.Size(343, 18);
            LabelPath.StyleFlags = UserControls.Style.StyleFlag.None;
            LabelPath.StylizeFlag = true;
            LabelPath.TabIndex = 18;
            LabelPath.TabStop = false;
            LabelPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LabelPath.Token = null;
            // 
            // BtnSelectPath
            // 
            BtnSelectPath.Adjustable = false;
            BtnSelectPath.BackColor = System.Drawing.Color.Transparent;
            BtnSelectPath.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSelectPath.BorderThickness = 0;
            BtnSelectPath.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSelectPath.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnSelectPath.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSelectPath.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSelectPath.CornerRadius = 0;
            BtnSelectPath.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSelectPath.DaskArray = null;
            BtnSelectPath.DoubleClickEnable = true;
            BtnSelectPath.DropKey = System.Windows.Forms.Keys.Space;
            BtnSelectPath.FineEnable = false;
            BtnSelectPath.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
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
            BtnSelectPath.Location = new System.Drawing.Point(403, 108);
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
            BtnSelectPath.TabIndex = 20;
            BtnSelectPath.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSelectPath.Click += BtnSelectPath_Click;
            // 
            // CbxPicType
            // 
            CbxPicType.AutoSize = true;
            CbxPicType.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxPicType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxPicType.ComBorderColor = System.Drawing.Color.Blue;
            CbxPicType.DataSource = (System.Collections.IList)resources.GetObject("CbxPicType.DataSource");
            CbxPicType.ExtText = "";
            CbxPicType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxPicType.ForeColor = System.Drawing.Color.White;
            CbxPicType.Location = new System.Drawing.Point(258, 40);
            CbxPicType.MaximumSize = new System.Drawing.Size(9999, 9999);
            CbxPicType.Name = "CbxPicType";
            CbxPicType.SelectIndex = -1;
            CbxPicType.SelectValue = null;
            CbxPicType.Size = new System.Drawing.Size(164, 30);
            CbxPicType.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxPicType.StylizeFlag = true;
            CbxPicType.TabIndex = 27;
            // 
            // LabelPicType
            // 
            LabelPicType.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LabelPicType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LabelPicType.BorderThickness = 0;
            LabelPicType.CornerRadius = 0;
            LabelPicType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LabelPicType.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LabelPicType.HighlightPrompt = defaultHighlightPrompt5;
            LabelPicType.IsOmittext = true;
            LabelPicType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LabelPicType.Location = new System.Drawing.Point(258, 12);
            LabelPicType.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            LabelPicType.MultyLineFlag = false;
            LabelPicType.Name = "LabelPicType";
            LabelPicType.Size = new System.Drawing.Size(164, 18);
            LabelPicType.StyleFlags = UserControls.Style.StyleFlag.None;
            LabelPicType.StylizeFlag = true;
            LabelPicType.TabIndex = 26;
            LabelPicType.TabStop = false;
            LabelPicType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LabelPicType.Token = null;
            // 
            // PassFailSavePage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(CbxPicType);
            Controls.Add(LabelPicType);
            Controls.Add(TbxFileName);
            Controls.Add(ChkSuffix);
            Controls.Add(CbxFileType);
            Controls.Add(CbxPath);
            Controls.Add(LabelFileType);
            Controls.Add(LblTime);
            Controls.Add(LabelFileName);
            Controls.Add(LabelPath);
            Controls.Add(BtnSelectPath);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "PassFailSavePage";
            Size = new System.Drawing.Size(539, 600);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private UserControls.ScopeXTextBox TbxFileName;
        private UserControls.ScopeXSwitchButton ChkSuffix;
        private UserControls.SelectComboBox CbxFileType;
        private UserControls.ComboBoxEx CbxPath;
        private UserControls.ScopeXLabel LabelFileType;
        private UserControls.ScopeXLabel LblTime;
        private UserControls.ScopeXLabel LabelFileName;
        private UserControls.ScopeXLabel LabelPath;
        private UserControls.ScopeXIconButton BtnSelectPath;
        private UserControls.SelectComboBox selectComboBox1;
        private UserControls.ScopeXLabel scopexLabel1;
        private UserControls.SelectComboBox CbxPicype;
        private UserControls.SelectComboBox CbxPicType;
        private UserControls.ScopeXLabel LabelPicType;
    }
}
