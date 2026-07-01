
using ScopeX.UserControls;

namespace ScopeX.U2
{
    partial class ScreenSavePage
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
            DefaultHighlightPrompt defaultHighlightPrompt7 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt8 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt9 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt10 = new DefaultHighlightPrompt();
            RadioButtonItem radioButtonItem6 = new RadioButtonItem();
            RadioButtonItem radioButtonItem7 = new RadioButtonItem();
            DefaultHighlightPrompt defaultHighlightPrompt11 = new DefaultHighlightPrompt();
            RadioButtonItem radioButtonItem8 = new RadioButtonItem();
            RadioButtonItem radioButtonItem9 = new RadioButtonItem();
            RadioButtonItem radioButtonItem10 = new RadioButtonItem();
            DefaultHighlightPrompt defaultHighlightPrompt12 = new DefaultHighlightPrompt();
            LabelFileName = new ScopeXLabel();
            LabelFileType = new ScopeXLabel();
            LblTime = new ScopeXLabel();
            LblTimeStamp = new ScopeXLabel();
            LblRegion = new ScopeXLabel();
            RdoRegion = new UIRadioButtonGroup();
            CbxPath = new ComboBoxEx();
            BtnSelectPath = new ScopeXIconButton();
            TbxFileName = new ScopeXTextBox();
            ChkSuffix = new ScopeXSwitchButton();
            ChkTimeStamp = new ScopeXSwitchButton();
            BtnSave = new ScopeXIconButton();
            BtnSaveAndOpen = new ScopeXIconButton();
            LabelPath = new ScopeXLabel();
            RdoColor = new UIRadioButtonGroup();
            LblModel = new ScopeXLabel();
            BtnOpenDir = new ScopeXIconButton();
            CbxFileType = new UserControls.SelectComboBox();
            SuspendLayout();
            // 
            // LabelFileName
            // 
            LabelFileName.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LabelFileName.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LabelFileName.BorderThickness = 0;
            LabelFileName.CornerRadius = 0;
            LabelFileName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LabelFileName.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LabelFileName.HighlightPrompt = defaultHighlightPrompt7;
            LabelFileName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LabelFileName.Location = new System.Drawing.Point(10, 248);
            LabelFileName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            LabelFileName.MultyLineFlag = false;
            LabelFileName.Name = "LabelFileName";
            LabelFileName.Size = new System.Drawing.Size(343, 18);
            LabelFileName.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LabelFileName.StylizeFlag = true;
            LabelFileName.TabIndex = 9;
            LabelFileName.TabStop = false;
            LabelFileName.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MingCheng");
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
            LabelFileType.HighlightPrompt = defaultHighlightPrompt8;
            LabelFileType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LabelFileType.Location = new System.Drawing.Point(10, 82);
            LabelFileType.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            LabelFileType.MultyLineFlag = false;
            LabelFileType.Name = "LabelFileType";
            LabelFileType.Size = new System.Drawing.Size(164, 18);
            LabelFileType.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LabelFileType.StylizeFlag = true;
            LabelFileType.TabIndex = 4;
            LabelFileType.TabStop = false;
            LabelFileType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LeiXing");
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
            LblTime.HighlightPrompt = defaultHighlightPrompt9;
            LblTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTime.Location = new System.Drawing.Point(443, 248);
            LblTime.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            LblTime.MultyLineFlag = false;
            LblTime.Name = "LblTime";
            LblTime.Size = new System.Drawing.Size(120, 18);
            LblTime.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblTime.StylizeFlag = true;
            LblTime.TabIndex = 11;
            LblTime.TabStop = false;
            LblTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("RiQiHouZhui");
            LblTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblTime.Token = null;

            // 
            // LblTimeStamp
            // 
            LblTimeStamp.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblTimeStamp.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblTimeStamp.BorderThickness = 0;
            LblTimeStamp.CornerRadius = 0;
            LblTimeStamp.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblTimeStamp.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblTimeStamp.HighlightPrompt = defaultHighlightPrompt9;
            LblTimeStamp.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTimeStamp.Location = new System.Drawing.Point(298, 82);
            LblTimeStamp.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            LblTimeStamp.MultyLineFlag = false;
            LblTimeStamp.Name = "LblTimeStamp";
            LblTimeStamp.Size = new System.Drawing.Size(120, 18);
            LblTimeStamp.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblTimeStamp.StylizeFlag = true;
            LblTimeStamp.TabIndex = 11;
            LblTimeStamp.TabStop = false;
            LblTimeStamp.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiJianChuo");
            LblTimeStamp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblTimeStamp.Token = null;
            // 
            // LblRegion
            // 
            LblRegion.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblRegion.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblRegion.BorderThickness = 0;
            LblRegion.CornerRadius = 0;
            LblRegion.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblRegion.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblRegion.HighlightPrompt = defaultHighlightPrompt10;
            LblRegion.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblRegion.Location = new System.Drawing.Point(10, 2);
            LblRegion.MultyLineFlag = false;
            LblRegion.Name = "LblRegion";
            LblRegion.Size = new System.Drawing.Size(164, 18);
            LblRegion.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblRegion.StylizeFlag = true;
            LblRegion.TabIndex = 0;
            LblRegion.TabStop = false;
            LblRegion.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuYu");
            LblRegion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblRegion.Token = null;
            // 
            // RdoRegion
            // 
            RdoRegion.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoRegion.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoRegion.BorderThickness = 0;
            RdoRegion.ButtonBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoRegion.ButtonFont = null;
            radioButtonItem6.Icon = null;
            radioButtonItem6.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem6.Tag = null;
            radioButtonItem6.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PingMu");
            radioButtonItem7.Icon = null;
            radioButtonItem7.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem7.Tag = null;
            radioButtonItem7.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhaGe");
            RdoRegion.ButtonItems = new RadioButtonItem[]
    {
    radioButtonItem6,
    radioButtonItem7
    };
            RdoRegion.ButtonOffset = 10;
            RdoRegion.ButtonTextColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RdoRegion.ChoosedButtonColor = System.Drawing.Color.FromArgb(40, 71, 193);
            RdoRegion.ChoosedButtonIndex = 0;
            RdoRegion.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoRegion.ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            RdoRegion.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoRegion.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoRegion.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoRegion.Height = 30;
            RdoRegion.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoRegion.Location = new System.Drawing.Point(10, 30);
            RdoRegion.Name = "RdoRegion";
            RdoRegion.Size = new System.Drawing.Size(164, 30);
            RdoRegion.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            RdoRegion.StylizeFlag = true;
            RdoRegion.TabIndex = 1;
            RdoRegion.IndexChanged += RdoRegion_IndexChanged;
            // 
            // CbxPath
            // 
            CbxPath.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxPath.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxPath.BorderThickness = 0;
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
            CbxPath.Location = new System.Drawing.Point(10, 193);
            CbxPath.MaxDropDownItems = 20;
            CbxPath.Name = "CbxPath";
            CbxPath.RectBtnWidth = 20;
            CbxPath.SelectedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            CbxPath.SelectedIndex = -1;
            CbxPath.SelectedItem = null;
            CbxPath.SelectedText = "";
            CbxPath.Size = new System.Drawing.Size(453, 30);
            CbxPath.Soreted = false;
            CbxPath.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            CbxPath.StylizeFlag = true;
            CbxPath.TabIndex = 7;
            CbxPath.Text = "";
            CbxPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            CbxPath.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
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
            BtnSelectPath.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            BtnSelectPath.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSelectPath.Height = 40;
            BtnSelectPath.Icon = Properties.Resources.FolderBrowser;
            BtnSelectPath.IconOffset = 0;
            BtnSelectPath.IconSize = new System.Drawing.Size(30, 30);
            BtnSelectPath.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSelectPath.IsIndicatorShow = false;
            BtnSelectPath.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSelectPath.Location = new System.Drawing.Point(468, 193);
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
            BtnSelectPath.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnSelectPath.StylizeFlag = false;
            BtnSelectPath.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSelectPath.SVGPath = "";
            BtnSelectPath.TabIndex = 8;
            BtnSelectPath.Tag = "";
            BtnSelectPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            BtnSelectPath.Click += BtnSelectPath_Click;
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
            TbxFileName.Location = new System.Drawing.Point(10, 276);
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
            TbxFileName.Size = new System.Drawing.Size(423, 30);
            TbxFileName.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            TbxFileName.StylizeFlag = true;
            TbxFileName.TabIndex = 10;
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
            ChkSuffix.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkSuffix.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkSuffix.DropKey = System.Windows.Forms.Keys.Space;
            ChkSuffix.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkSuffix.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkSuffix.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkSuffix.Height = 30;
            ChkSuffix.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkSuffix.Location = new System.Drawing.Point(443, 276);
            ChkSuffix.Margin = new System.Windows.Forms.Padding(0);
            ChkSuffix.Name = "ChkSuffix";
            ChkSuffix.Size = new System.Drawing.Size(75, 30);
            ChkSuffix.SliderButtonWidth = 30;
            ChkSuffix.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkSuffix.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            ChkSuffix.StylizeFlag = true;
            ChkSuffix.SwitchShape = ScopeXSwitchButton.Shape.Square;
            ChkSuffix.TabIndex = 12;
            ChkSuffix.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkSuffix.UseAnimation = true;
            ChkSuffix.CheckedChangedEvent += ChkSuffix_CheckedChangedEvent;

            // 
            // ChkSuffix
            // 
            ChkTimeStamp.AnimationCount = 8;
            ChkTimeStamp.AnimationFunc = null;
            ChkTimeStamp.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkTimeStamp.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkTimeStamp.BorderThickness = 0;
            ChkTimeStamp.Checked = false;
            ChkTimeStamp.CheckedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            ChkTimeStamp.CheckedForeColor = System.Drawing.Color.Black;
            ChkTimeStamp.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkTimeStamp.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkTimeStamp.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkTimeStamp.DropKey = System.Windows.Forms.Keys.Space;
            ChkTimeStamp.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkTimeStamp.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkTimeStamp.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkTimeStamp.Height = 30;
            ChkTimeStamp.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkTimeStamp.Location = new System.Drawing.Point(298, 110);
            ChkTimeStamp.Margin = new System.Windows.Forms.Padding(0);
            ChkTimeStamp.Name = "ChkSuffix";
            ChkTimeStamp.Size = new System.Drawing.Size(75, 30);
            ChkTimeStamp.SliderButtonWidth = 30;
            ChkTimeStamp.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkTimeStamp.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            ChkTimeStamp.StylizeFlag = true;
            ChkTimeStamp.SwitchShape = ScopeXSwitchButton.Shape.Square;
            ChkTimeStamp.TabIndex = 12;
            ChkTimeStamp.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkTimeStamp.UseAnimation = true;
            ChkTimeStamp.CheckedChangedEvent += ChkTimeStamp_CheckedChangedEvent;
            // 
            // BtnSave
            // 
            BtnSave.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSave.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSave.BorderThickness = 0;
            BtnSave.CornerRadius = 0;
            BtnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSave.DaskArray = null;
            BtnSave.DropKey = System.Windows.Forms.Keys.Space;
            BtnSave.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnSave.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSave.Height = 30;
            BtnSave.Icon = null;
            BtnSave.IconOffset = 10;
            BtnSave.IconSize = new System.Drawing.Size(24, 24);
            BtnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnSave.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSave.IsIndicatorShow = false;
            BtnSave.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSave.Location = new System.Drawing.Point(298, 338);
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
            BtnSave.Size = new System.Drawing.Size(75, 30);
            BtnSave.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnSave.StylizeFlag = true;
            BtnSave.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSave.SVGPath = "";
            BtnSave.TabIndex = 14;
            BtnSave.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BaoCun");
            BtnSave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSave.Click += BtnSave_Click;
            // 
            // BtnSaveAndOpen
            // 
            BtnSaveAndOpen.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveAndOpen.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveAndOpen.BorderThickness = 0;
            BtnSaveAndOpen.CornerRadius = 0;
            BtnSaveAndOpen.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSaveAndOpen.DaskArray = null;
            BtnSaveAndOpen.DropKey = System.Windows.Forms.Keys.Space;
            BtnSaveAndOpen.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnSaveAndOpen.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveAndOpen.Height = 30;
            BtnSaveAndOpen.Icon = null;
            BtnSaveAndOpen.IconOffset = 10;
            BtnSaveAndOpen.IconSize = new System.Drawing.Size(24, 24);
            BtnSaveAndOpen.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnSaveAndOpen.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSaveAndOpen.IsIndicatorShow = false;
            BtnSaveAndOpen.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSaveAndOpen.Location = new System.Drawing.Point(408, 338);
            BtnSaveAndOpen.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveAndOpen.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveAndOpen.MouseInBorderThickness = 0;
            BtnSaveAndOpen.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveAndOpen.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSaveAndOpen.Name = "BtnSaveAndOpen";
            BtnSaveAndOpen.PressedBackColor = System.Drawing.Color.Gray;
            BtnSaveAndOpen.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveAndOpen.PressedBorderThickness = 0;
            BtnSaveAndOpen.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveAndOpen.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSaveAndOpen.Size = new System.Drawing.Size(120, 30);
            BtnSaveAndOpen.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnSaveAndOpen.StylizeFlag = true;
            BtnSaveAndOpen.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveAndOpen.SVGPath = "";
            BtnSaveAndOpen.TabIndex = 15;
            BtnSaveAndOpen.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BaoCun_TiaoZhuan");
            BtnSaveAndOpen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSaveAndOpen.Click += BtnSaveAndOpen_Click;
            // 
            // LabelPath
            // 
            LabelPath.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LabelPath.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LabelPath.BorderThickness = 0;
            LabelPath.CornerRadius = 0;
            LabelPath.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LabelPath.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LabelPath.HighlightPrompt = defaultHighlightPrompt11;
            LabelPath.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LabelPath.Location = new System.Drawing.Point(10, 165);
            LabelPath.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            LabelPath.MultyLineFlag = false;
            LabelPath.Name = "LabelPath";
            LabelPath.Size = new System.Drawing.Size(343, 18);
            LabelPath.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LabelPath.StylizeFlag = true;
            LabelPath.TabIndex = 6;
            LabelPath.TabStop = false;
            LabelPath.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WeiZhi");
            LabelPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LabelPath.Token = null;
            // 
            // RdoColor
            // 
            RdoColor.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoColor.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoColor.BorderThickness = 0;
            RdoColor.ButtonBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoColor.ButtonFont = null;
            radioButtonItem8.Icon = null;
            radioButtonItem8.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem8.Tag = null;
            radioButtonItem8.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BiaoZhun");
            radioButtonItem9.Icon = null;
            radioButtonItem9.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem9.Tag = null;
            radioButtonItem9.Text =ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("HeiBai");
            radioButtonItem10.Icon = null;
            radioButtonItem10.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem10.Tag = null;
            radioButtonItem10.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FanSe");
            RdoColor.ButtonItems = new RadioButtonItem[]
    {
    radioButtonItem8,
    radioButtonItem9,
    radioButtonItem10
    };
            RdoColor.ButtonOffset = 10;
            RdoColor.ButtonTextColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RdoColor.ChoosedButtonColor = System.Drawing.Color.FromArgb(40, 71, 193);
            RdoColor.ChoosedButtonIndex = 0;
            RdoColor.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoColor.ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            RdoColor.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoColor.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoColor.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoColor.Height = 30;
            RdoColor.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoColor.Location = new System.Drawing.Point(298, 30);
            RdoColor.Name = "RdoColor";
            RdoColor.Size = new System.Drawing.Size(225, 30);
            RdoColor.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            RdoColor.StylizeFlag = true;
            RdoColor.TabIndex = 3;
            RdoColor.IndexChanged += RdoColor_IndexChanged;
            // 
            // LblModel
            // 
            LblModel.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblModel.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblModel.BorderThickness = 0;
            LblModel.CornerRadius = 0;
            LblModel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblModel.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblModel.HighlightPrompt = defaultHighlightPrompt12;
            LblModel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblModel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblModel.Location = new System.Drawing.Point(298, 2);
            LblModel.MultyLineFlag = false;
            LblModel.Name = "LblModel";
            LblModel.Size = new System.Drawing.Size(225, 18);
            LblModel.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblModel.StylizeFlag = true;
            LblModel.TabIndex = 2;
            LblModel.TabStop = false;
            LblModel.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YanSe");
            LblModel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblModel.Token = null;
            // 
            // BtnOpenDir
            // 
            BtnOpenDir.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOpenDir.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOpenDir.BorderThickness = 0;
            BtnOpenDir.CornerRadius = 0;
            BtnOpenDir.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnOpenDir.DaskArray = null;
            BtnOpenDir.DropKey = System.Windows.Forms.Keys.Space;
            BtnOpenDir.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnOpenDir.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOpenDir.Height = 30;
            BtnOpenDir.Icon = Properties.Resources.OpenFolder;
            BtnOpenDir.IconOffset = 5;
            BtnOpenDir.IconSize = new System.Drawing.Size(26, 26);
            BtnOpenDir.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnOpenDir.IsIndicatorShow = false;
            BtnOpenDir.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnOpenDir.Location = new System.Drawing.Point(10, 338);
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
            BtnOpenDir.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnOpenDir.StylizeFlag = true;
            BtnOpenDir.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOpenDir.SVGPath = "";
            BtnOpenDir.TabIndex = 13;
            BtnOpenDir.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DaKaiWenJianSuoZaiWeiZhi");
            BtnOpenDir.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnOpenDir.Click += BtnOpenDir_Click;
            // 
            // CbxFileType
            // 
            CbxFileType.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxFileType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxFileType.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxFileType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxFileType.ForeColor = System.Drawing.Color.White;
            CbxFileType.Location = new System.Drawing.Point(10, 110);
            CbxFileType.Name = "CbxFileType";
            CbxFileType.Size = new System.Drawing.Size(164, 30);
            CbxFileType.TabIndex = 16;
            // 
            // ScreenSavePage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(CbxFileType);
            Controls.Add(BtnOpenDir);
            Controls.Add(RdoColor);
            Controls.Add(LblModel);
            Controls.Add(LabelPath);
            Controls.Add(BtnSaveAndOpen);
            Controls.Add(ChkSuffix);
            Controls.Add(ChkTimeStamp);
            
            Controls.Add(TbxFileName);
            Controls.Add(BtnSave);
            Controls.Add(BtnSelectPath);
            Controls.Add(CbxPath);
            Controls.Add(RdoRegion);
            Controls.Add(LblTime);
            Controls.Add(LblTimeStamp);
            Controls.Add(LabelFileType);
            Controls.Add(LabelFileName);
            Controls.Add(LblRegion);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "ScreenSavePage";
            Size = new System.Drawing.Size(539, 390);
            ResumeLayout(false);
        }

        #endregion
        private ScopeX.UserControls.ScopeXLabel LabelFileName;
        private ScopeX.UserControls.ScopeXLabel LabelFileType;
        private ScopeXLabel LblTime;
        private ScopeXLabel LblTimeStamp;
        private ScopeXLabel LblRegion;
        private UIRadioButtonGroup RdoRegion;
        private ComboBoxEx CbxPath;
        private ScopeXIconButton BtnSelectPath;
        private ScopeXTextBox TbxFileName;
        private ScopeXSwitchButton ChkSuffix;
        private ScopeXSwitchButton ChkTimeStamp;
        private ScopeXIconButton BtnSave;
        private ScopeXIconButton BtnSaveAndOpen;
        private ScopeXLabel LabelPath;
        private UIRadioButtonGroup RdoColor;
        private ScopeXLabel LblModel;
        private ScopeXIconButton BtnOpenDir;
        private UserControls.SelectComboBox CbxFileType;
    }
}
