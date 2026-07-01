
using ScopeX.UserControls;

namespace ScopeX.U2
{
    partial class SettingPage
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
            this.components = new System.ComponentModel.Container();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt6 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt7 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt8 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt9 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt10 = new ScopeX.UserControls.DefaultHighlightPrompt();
            this.TbxFileName = new ScopeX.UserControls.ScopeXTextBox();
            this.ChkSuffix = new ScopeX.UserControls.ScopeXSwitchButton();
            this.BtnSave = new ScopeX.UserControls.ScopeXIconButton();
            this.CbxPath = new ScopeX.UserControls.ComboBoxEx();
            this.LblTime = new ScopeX.UserControls.ScopeXLabel();
            this.LblFileName = new ScopeX.UserControls.ScopeXLabel();
            this.BtnSelectPath = new ScopeX.UserControls.ScopeXIconButton();
            this.BtnSaveAndOpen = new ScopeX.UserControls.ScopeXIconButton();
            this.BtnUndoDefault = new ScopeX.UserControls.ScopeXIconButton();
            this.BtnLoadDefault = new ScopeX.UserControls.ScopeXIconButton();
            this.CbxFile = new ScopeX.UserControls.ComboBoxEx();
            this.BtnSelectFile = new ScopeX.UserControls.ScopeXIconButton();
            this.LblSaver = new ScopeX.UserControls.ScopeXLabel();
            //this.SplitLine = new ScopeX.U2.UUVerticalSplitLine(this.components);
            //this.uuVerticalSplitLine1 = new ScopeX.U2.UUVerticalSplitLine(this.components);
            this.LblReader = new ScopeX.UserControls.ScopeXLabel();
            this.BtnOpenDir = new ScopeX.UserControls.ScopeXIconButton();
            this.LabelPath = new ScopeX.UserControls.ScopeXLabel();
            this.SuspendLayout();
            // 
            // TbxFileName
            // 
            this.TbxFileName.AcceptsTab = false;
            this.TbxFileName.AutoShowKeyBoard = true;
            this.TbxFileName.AutoSize = false;
            this.TbxFileName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxFileName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxFileName.BorderThickness = 0;
            this.TbxFileName.CornerRadius = 0;
            this.TbxFileName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.TbxFileName.Enabled = true;
            this.TbxFileName.EnbleSelectBorder = true;
            this.TbxFileName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TbxFileName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.TbxFileName.Height = 30;
            this.TbxFileName.HideSelection = true;
            this.TbxFileName.KeyboardVerify = null;
            this.TbxFileName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.TbxFileName.Lines = new string[0];
            this.TbxFileName.Location = new System.Drawing.Point(46, 125);
            this.TbxFileName.MaxLength = 32767;
            this.TbxFileName.Modified = false;
            this.TbxFileName.MouseEnterState = false;
            this.TbxFileName.Multiline = false;
            this.TbxFileName.Name = "TbxFileName";
            this.TbxFileName.ProcessCmdKeyFunc = null;
            this.TbxFileName.ReadOnly = false;
            this.TbxFileName.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.TbxFileName.SelectedText = "";
            this.TbxFileName.SelectionLength = 0;
            this.TbxFileName.SelectionStart = 0;
            this.TbxFileName.ShortcutsEnabled = true;
            this.TbxFileName.Size = new System.Drawing.Size(369, 30);
            this.TbxFileName.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.TbxFileName.StylizeFlag = true;
            this.TbxFileName.TabIndex = 6;
            this.TbxFileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.TbxFileName.UseSystemPasswordChar = false;
            this.TbxFileName.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.TbxFileName.WordWrap = true;
            this.TbxFileName.TextChanged += new System.EventHandler(this.TbxFileName_TextChanged);
            // 
            // ChkSuffix
            // 
            this.ChkSuffix.AnimationCount = 8;
            this.ChkSuffix.AnimationFunc = null;
            this.ChkSuffix.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkSuffix.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkSuffix.BorderThickness = 0;
            this.ChkSuffix.Checked = false;
            this.ChkSuffix.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.ChkSuffix.CheckedForeColor = System.Drawing.Color.Black;
            this.ChkSuffix.CheckedSliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkSuffix.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            this.ChkSuffix.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ChkSuffix.DropKey = System.Windows.Forms.Keys.Space;
            this.ChkSuffix.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkSuffix.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ChkSuffix.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.ChkSuffix.Height = 30;
            this.ChkSuffix.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.ChkSuffix.Location = new System.Drawing.Point(421, 125);
            this.ChkSuffix.Margin = new System.Windows.Forms.Padding(0);
            this.ChkSuffix.Name = "ChkSuffix";
            this.ChkSuffix.Size = new System.Drawing.Size(75, 30);
            this.ChkSuffix.SliderButtonWidth = 30;
            this.ChkSuffix.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkSuffix.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.ChkSuffix.StylizeFlag = true;
            this.ChkSuffix.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            this.ChkSuffix.TabIndex = 8;
            this.ChkSuffix.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            this.ChkSuffix.UseAnimation = true;
            this.ChkSuffix.CheckedChangedEvent += new System.EventHandler(this.ChkSuffix_CheckedChangedEvent);
            // 
            // BtnSave
            // 
            this.BtnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSave.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSave.BorderThickness = 1;
            this.BtnSave.CornerRadius = 0;
            this.BtnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSave.DaskArray = null;
            this.BtnSave.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnSave.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSave.Height = 30;
            this.BtnSave.Icon = null;
            this.BtnSave.IconOffset = 10;
            this.BtnSave.IconSize = new System.Drawing.Size(24, 24);
            this.BtnSave.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnSave.IsIndicatorShow = false;
            this.BtnSave.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnSave.Location = new System.Drawing.Point(270, 184);
            this.BtnSave.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSave.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSave.MouseInBorderThickness = 0;
            this.BtnSave.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSave.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnSave.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSave.PressedBorderThickness = 0;
            this.BtnSave.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSave.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnSave.Size = new System.Drawing.Size(75, 30);
            this.BtnSave.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnSave.StylizeFlag = true;
            this.BtnSave.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSave.SVGPath = "";
            this.BtnSave.TabIndex = 10;
            this.BtnSave.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BaoCun");
            this.BtnSave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // CbxPath
            // 
            this.CbxPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxPath.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxPath.BorderThickness = 0;
            this.CbxPath.CornerRadius = 0;
            this.CbxPath.DataSource = null;
            this.CbxPath.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.CbxPath.DropDownHeight = 200;
            this.CbxPath.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxPath.DropDownWidth = 309;
            this.CbxPath.DropKey = System.Windows.Forms.Keys.Space;
            this.CbxPath.DroppedDown = false;
            this.CbxPath.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxPath.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CbxPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.CbxPath.GetDisPlayName = null;
            this.CbxPath.Height = 30;
            this.CbxPath.ImageMode = false;
            this.CbxPath.ItemHeight = 28;
            this.CbxPath.KeyDropEnble = true;
            this.CbxPath.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.CbxPath.Location = new System.Drawing.Point(46, 55);
            this.CbxPath.MaxDropDownItems = 8;
            this.CbxPath.Name = "CbxPath";
            this.CbxPath.RectBtnWidth = 20;
            this.CbxPath.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.CbxPath.SelectedIndex = -1;
            this.CbxPath.SelectedItem = null;
            this.CbxPath.SelectedText = "";
            this.CbxPath.Size = new System.Drawing.Size(369, 30);
            this.CbxPath.Soreted = false;
            this.CbxPath.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.CbxPath.StylizeFlag = true;
            this.CbxPath.TabIndex = 3;
            this.CbxPath.Text = "";
            this.CbxPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.CbxPath.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            // 
            // LblTime
            // 
            this.LblTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblTime.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblTime.BorderThickness = 0;
            this.LblTime.CornerRadius = 0;
            this.LblTime.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblTime.HighlightPrompt = defaultHighlightPrompt6;
            this.LblTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblTime.Location = new System.Drawing.Point(421, 97);
            this.LblTime.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.LblTime.MultyLineFlag = false;
            this.LblTime.Name = "LblTime";
            this.LblTime.Size = new System.Drawing.Size(120, 18);
            this.LblTime.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblTime.StylizeFlag = true;
            this.LblTime.TabIndex = 7;
            this.LblTime.TabStop = false;
            this.LblTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("RiQiHouZhui");
            this.LblTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblTime.Token = null;
            // 
            // LblFileName
            // 
            this.LblFileName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblFileName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblFileName.BorderThickness = 0;
            this.LblFileName.CornerRadius = 0;
            this.LblFileName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblFileName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblFileName.HighlightPrompt = defaultHighlightPrompt7;
            this.LblFileName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblFileName.Location = new System.Drawing.Point(46, 97);
            this.LblFileName.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.LblFileName.MultyLineFlag = false;
            this.LblFileName.Name = "LblFileName";
            this.LblFileName.Size = new System.Drawing.Size(309, 18);
            this.LblFileName.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblFileName.StylizeFlag = true;
            this.LblFileName.TabIndex = 5;
            this.LblFileName.TabStop = false;
            this.LblFileName.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MingCheng");
            this.LblFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblFileName.Token = null;
            // 
            // BtnSelectPath
            // 
            this.BtnSelectPath.BackColor = System.Drawing.Color.Transparent;
            this.BtnSelectPath.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSelectPath.BorderThickness = 0;
            this.BtnSelectPath.CornerRadius = 0;
            this.BtnSelectPath.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSelectPath.DaskArray = null;
            this.BtnSelectPath.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnSelectPath.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnSelectPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSelectPath.Height = 30;
            this.BtnSelectPath.Icon = global::ScopeX.U2.Properties.Resources.FolderBrowser;
            this.BtnSelectPath.IconOffset = 0;
            this.BtnSelectPath.IconSize = new System.Drawing.Size(24, 24);
            this.BtnSelectPath.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnSelectPath.IsIndicatorShow = false;
            this.BtnSelectPath.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnSelectPath.Location = new System.Drawing.Point(421, 55);
            this.BtnSelectPath.MouseinBackColor = System.Drawing.Color.Transparent;
            this.BtnSelectPath.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSelectPath.MouseInBorderThickness = 0;
            this.BtnSelectPath.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSelectPath.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnSelectPath.Name = "BtnSelectPath";
            this.BtnSelectPath.PressedBackColor = System.Drawing.Color.Transparent;
            this.BtnSelectPath.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSelectPath.PressedBorderThickness = 0;
            this.BtnSelectPath.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSelectPath.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnSelectPath.Size = new System.Drawing.Size(30, 30);
            this.BtnSelectPath.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnSelectPath.StylizeFlag = false;
            this.BtnSelectPath.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSelectPath.SVGPath = "";
            this.BtnSelectPath.TabIndex = 4;
            this.BtnSelectPath.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnSelectPath.Click += new System.EventHandler(this.BtnSelectPath_Click);
            // 
            // BtnSaveAndOpen
            // 
            this.BtnSaveAndOpen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSaveAndOpen.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSaveAndOpen.BorderThickness = 1;
            this.BtnSaveAndOpen.CornerRadius = 0;
            this.BtnSaveAndOpen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSaveAndOpen.DaskArray = null;
            this.BtnSaveAndOpen.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnSaveAndOpen.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnSaveAndOpen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSaveAndOpen.Height = 30;
            this.BtnSaveAndOpen.Icon = null;
            this.BtnSaveAndOpen.IconOffset = 10;
            this.BtnSaveAndOpen.IconSize = new System.Drawing.Size(24, 24);
            this.BtnSaveAndOpen.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnSaveAndOpen.IsIndicatorShow = false;
            this.BtnSaveAndOpen.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnSaveAndOpen.Location = new System.Drawing.Point(380, 184);
            this.BtnSaveAndOpen.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSaveAndOpen.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSaveAndOpen.MouseInBorderThickness = 0;
            this.BtnSaveAndOpen.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSaveAndOpen.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnSaveAndOpen.Name = "BtnSaveAndOpen";
            this.BtnSaveAndOpen.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnSaveAndOpen.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSaveAndOpen.PressedBorderThickness = 0;
            this.BtnSaveAndOpen.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSaveAndOpen.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnSaveAndOpen.Size = new System.Drawing.Size(116, 30);
            this.BtnSaveAndOpen.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnSaveAndOpen.StylizeFlag = true;
            this.BtnSaveAndOpen.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSaveAndOpen.SVGPath = "";
            this.BtnSaveAndOpen.TabIndex = 11;
            this.BtnSaveAndOpen.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BaoCun_TiaoZhuan");
            this.BtnSaveAndOpen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnSaveAndOpen.Click += new System.EventHandler(this.BtnSaveAndOpen_Click);
            // 
            // BtnUndoDefault
            // 
            this.BtnUndoDefault.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnUndoDefault.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnUndoDefault.BorderThickness = 1;
            this.BtnUndoDefault.CornerRadius = 0;
            this.BtnUndoDefault.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnUndoDefault.DaskArray = null;
            this.BtnUndoDefault.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnUndoDefault.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnUndoDefault.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnUndoDefault.Height = 30;
            this.BtnUndoDefault.Icon = null;
            this.BtnUndoDefault.IconOffset = 10;
            this.BtnUndoDefault.IconSize = new System.Drawing.Size(24, 24);
            this.BtnUndoDefault.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnUndoDefault.IsIndicatorShow = false;
            this.BtnUndoDefault.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnUndoDefault.Location = new System.Drawing.Point(290, 316);
            this.BtnUndoDefault.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnUndoDefault.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnUndoDefault.MouseInBorderThickness = 0;
            this.BtnUndoDefault.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnUndoDefault.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnUndoDefault.Name = "BtnUndoDefault";
            this.BtnUndoDefault.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnUndoDefault.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnUndoDefault.PressedBorderThickness = 0;
            this.BtnUndoDefault.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnUndoDefault.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnUndoDefault.Size = new System.Drawing.Size(80, 30);
            this.BtnUndoDefault.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnUndoDefault.StylizeFlag = true;
            this.BtnUndoDefault.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnUndoDefault.SVGPath = "";
            this.BtnUndoDefault.TabIndex = 17;
            this.BtnUndoDefault.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CheXiaoMoRenSheZhi");
            this.BtnUndoDefault.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnUndoDefault.Visible = false;
            this.BtnUndoDefault.Click += new System.EventHandler(this.BtnUndoDefault_Click);
            // 
            // BtnLoadDefault
            // 
            this.BtnLoadDefault.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnLoadDefault.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnLoadDefault.BorderThickness = 1;
            this.BtnLoadDefault.CornerRadius = 0;
            this.BtnLoadDefault.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnLoadDefault.DaskArray = null;
            this.BtnLoadDefault.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnLoadDefault.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnLoadDefault.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnLoadDefault.Height = 30;
            this.BtnLoadDefault.Icon = null;
            this.BtnLoadDefault.IconOffset = 10;
            this.BtnLoadDefault.IconSize = new System.Drawing.Size(24, 24);
            this.BtnLoadDefault.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnLoadDefault.IsIndicatorShow = false;
            this.BtnLoadDefault.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnLoadDefault.Location = new System.Drawing.Point(46, 316);
            this.BtnLoadDefault.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnLoadDefault.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnLoadDefault.MouseInBorderThickness = 0;
            this.BtnLoadDefault.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnLoadDefault.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnLoadDefault.Name = "BtnLoadDefault";
            this.BtnLoadDefault.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnLoadDefault.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnLoadDefault.PressedBorderThickness = 0;
            this.BtnLoadDefault.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnLoadDefault.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnLoadDefault.Size = new System.Drawing.Size(80, 30);
            this.BtnLoadDefault.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnLoadDefault.StylizeFlag = true;
            this.BtnLoadDefault.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnLoadDefault.SVGPath = "";
            this.BtnLoadDefault.TabIndex = 16;
            this.BtnLoadDefault.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MoRenSheZhi");
            this.BtnLoadDefault.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnLoadDefault.Visible = false;
            this.BtnLoadDefault.Click += new System.EventHandler(this.BtnLoadDefault_Click);
            // 
            // CbxFile
            // 
            this.CbxFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxFile.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxFile.BorderThickness = 0;
            this.CbxFile.CornerRadius = 0;
            this.CbxFile.DataSource = null;
            this.CbxFile.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.CbxFile.DropDownHeight = 200;
            this.CbxFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxFile.DropDownWidth = 309;
            this.CbxFile.DropKey = System.Windows.Forms.Keys.Space;
            this.CbxFile.DroppedDown = false;
            this.CbxFile.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxFile.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CbxFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.CbxFile.GetDisPlayName = null;
            this.CbxFile.Height = 30;
            this.CbxFile.ImageMode = false;
            this.CbxFile.ItemHeight = 28;
            this.CbxFile.KeyDropEnble = true;
            this.CbxFile.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.CbxFile.Location = new System.Drawing.Point(46, 263);
            this.CbxFile.MaxDropDownItems = 8;
            this.CbxFile.Name = "CbxFile";
            this.CbxFile.RectBtnWidth = 20;
            this.CbxFile.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.CbxFile.SelectedIndex = -1;
            this.CbxFile.SelectedItem = null;
            this.CbxFile.SelectedText = "";
            this.CbxFile.Size = new System.Drawing.Size(369, 30);
            this.CbxFile.Soreted = false;
            this.CbxFile.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.CbxFile.StylizeFlag = true;
            this.CbxFile.TabIndex = 14;
            this.CbxFile.Text = "";
            this.CbxFile.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.CbxFile.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            // 
            // BtnSelectFile
            // 
            this.BtnSelectFile.BackColor = System.Drawing.Color.Transparent;
            this.BtnSelectFile.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSelectFile.BorderThickness = 0;
            this.BtnSelectFile.CornerRadius = 0;
            this.BtnSelectFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSelectFile.DaskArray = null;
            this.BtnSelectFile.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnSelectFile.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnSelectFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSelectFile.Height = 30;
            this.BtnSelectFile.Icon = global::ScopeX.U2.Properties.Resources.OpenFile;
            this.BtnSelectFile.IconOffset = 0;
            this.BtnSelectFile.IconSize = new System.Drawing.Size(30, 28);
            this.BtnSelectFile.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnSelectFile.IsIndicatorShow = false;
            this.BtnSelectFile.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnSelectFile.Location = new System.Drawing.Point(421, 263);
            this.BtnSelectFile.MouseinBackColor = System.Drawing.Color.Transparent;
            this.BtnSelectFile.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSelectFile.MouseInBorderThickness = 0;
            this.BtnSelectFile.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSelectFile.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnSelectFile.Name = "BtnSelectFile";
            this.BtnSelectFile.PressedBackColor = System.Drawing.Color.Transparent;
            this.BtnSelectFile.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSelectFile.PressedBorderThickness = 0;
            this.BtnSelectFile.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSelectFile.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnSelectFile.Size = new System.Drawing.Size(30, 30);
            this.BtnSelectFile.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnSelectFile.StylizeFlag = false;
            this.BtnSelectFile.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSelectFile.SVGPath = "";
            this.BtnSelectFile.TabIndex = 15;
            this.BtnSelectFile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnSelectFile.Click += new System.EventHandler(this.BtnSelectFile_Click);
            // 
            // LblSaver
            // 
            this.LblSaver.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblSaver.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblSaver.BorderThickness = 0;
            this.LblSaver.CornerRadius = 0;
            this.LblSaver.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblSaver.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblSaver.HighlightPrompt = defaultHighlightPrompt8;
            this.LblSaver.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblSaver.Location = new System.Drawing.Point(10, 6);
            this.LblSaver.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.LblSaver.MultyLineFlag = false;
            this.LblSaver.Name = "LblSaver";
            this.LblSaver.Size = new System.Drawing.Size(50, 18);
            this.LblSaver.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblSaver.StylizeFlag = true;
            this.LblSaver.TabIndex = 0;
            this.LblSaver.TabStop = false;
            this.LblSaver.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BaoCun");
            this.LblSaver.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblSaver.Token = null;
            // 
            // SplitLine
            // 
            //this.SplitLine.BackColor = System.Drawing.Color.Transparent;
            //this.SplitLine.DarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            //this.SplitLine.ForeColor = System.Drawing.Color.White;
            //this.SplitLine.LightColor = System.Drawing.Color.White;
            //this.SplitLine.Location = new System.Drawing.Point(66, 15);
            //this.SplitLine.Name = "SplitLine";
            //this.SplitLine.Size = new System.Drawing.Size(380, 1);
            //this.SplitLine.TabIndex = 1;
            // 
            // uuVerticalSplitLine1
            // 
            //this.uuVerticalSplitLine1.BackColor = System.Drawing.Color.Transparent;
            //this.uuVerticalSplitLine1.DarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            //this.uuVerticalSplitLine1.ForeColor = System.Drawing.Color.White;
            //this.uuVerticalSplitLine1.LightColor = System.Drawing.Color.White;
            //this.uuVerticalSplitLine1.Location = new System.Drawing.Point(66, 231);
            //this.uuVerticalSplitLine1.Name = "uuVerticalSplitLine1";
            //this.uuVerticalSplitLine1.Size = new System.Drawing.Size(380, 1);
            //this.uuVerticalSplitLine1.TabIndex = 13;
            // 
            // LblReader
            // 
            this.LblReader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblReader.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblReader.BorderThickness = 0;
            this.LblReader.CornerRadius = 0;
            this.LblReader.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblReader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblReader.HighlightPrompt = defaultHighlightPrompt9;
            this.LblReader.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblReader.Location = new System.Drawing.Point(10, 222);
            this.LblReader.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.LblReader.MultyLineFlag = false;
            this.LblReader.Name = "LblReader";
            this.LblReader.Size = new System.Drawing.Size(70, 18);
            this.LblReader.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblReader.StylizeFlag = true;
            this.LblReader.TabIndex = 12;
            this.LblReader.TabStop = false;
            this.LblReader.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DuQu");
            this.LblReader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblReader.Token = null;
            // 
            // BtnOpenDir
            // 
            this.BtnOpenDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnOpenDir.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnOpenDir.BorderThickness = 0;
            this.BtnOpenDir.CornerRadius = 0;
            this.BtnOpenDir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnOpenDir.DaskArray = null;
            this.BtnOpenDir.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnOpenDir.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnOpenDir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnOpenDir.Height = 30;
            this.BtnOpenDir.Icon = global::ScopeX.U2.Properties.Resources.OpenFolder;
            this.BtnOpenDir.IconOffset = 5;
            this.BtnOpenDir.IconSize = new System.Drawing.Size(26, 26);
            this.BtnOpenDir.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnOpenDir.IsIndicatorShow = false;
            this.BtnOpenDir.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnOpenDir.Location = new System.Drawing.Point(46, 184);
            this.BtnOpenDir.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(5)))));
            this.BtnOpenDir.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnOpenDir.MouseInBorderThickness = 0;
            this.BtnOpenDir.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnOpenDir.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnOpenDir.Name = "BtnOpenDir";
            this.BtnOpenDir.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnOpenDir.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnOpenDir.PressedBorderThickness = 0;
            this.BtnOpenDir.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnOpenDir.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnOpenDir.Size = new System.Drawing.Size(204, 30);
            this.BtnOpenDir.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnOpenDir.StylizeFlag = true;
            this.BtnOpenDir.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnOpenDir.SVGPath = "";
            this.BtnOpenDir.TabIndex = 9;
            this.BtnOpenDir.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DaKaiWenJianSuoZaiWeiZhi");
            this.BtnOpenDir.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnOpenDir.Click += new System.EventHandler(this.BtnOpenDir_Click);
            // 
            // LabelPath
            // 
            this.LabelPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LabelPath.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LabelPath.BorderThickness = 0;
            this.LabelPath.CornerRadius = 0;
            this.LabelPath.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LabelPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LabelPath.HighlightPrompt = defaultHighlightPrompt10;
            this.LabelPath.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LabelPath.Location = new System.Drawing.Point(46, 27);
            this.LabelPath.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.LabelPath.MultyLineFlag = false;
            this.LabelPath.Name = "LabelPath";
            this.LabelPath.Size = new System.Drawing.Size(309, 18);
            this.LabelPath.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LabelPath.StylizeFlag = true;
            this.LabelPath.TabIndex = 2;
            this.LabelPath.TabStop = false;
            this.LabelPath.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WeiZhi");
            this.LabelPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LabelPath.Token = null;
            // 
            // SettingPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.Controls.Add(this.LabelPath);
            this.Controls.Add(this.BtnOpenDir);
            //this.Controls.Add(this.uuVerticalSplitLine1);
            this.Controls.Add(this.LblReader);
            //this.Controls.Add(this.SplitLine);
            this.Controls.Add(this.LblSaver);
            this.Controls.Add(this.BtnUndoDefault);
            this.Controls.Add(this.BtnSaveAndOpen);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.ChkSuffix);
            this.Controls.Add(this.BtnLoadDefault);
            this.Controls.Add(this.TbxFileName);
            this.Controls.Add(this.CbxFile);
            this.Controls.Add(this.CbxPath);
            this.Controls.Add(this.BtnSelectFile);
            this.Controls.Add(this.LblFileName);
            this.Controls.Add(this.BtnSelectPath);
            this.Controls.Add(this.LblTime);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WeiRuanYaHei"), 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "SettingPage";
            this.Size = new System.Drawing.Size(539, 356);
            this.ResumeLayout(false);

        }

        #endregion

        private ScopeXTextBox TbxFileName;
        private ScopeXSwitchButton ChkSuffix;
        private ScopeXIconButton BtnSave;
        private ComboBoxEx CbxPath;
        private ScopeXLabel LblTime;
        private ScopeXLabel LblFileName;
        private ScopeXIconButton BtnSelectPath;
        private ScopeXIconButton BtnSaveAndOpen;
        private ScopeXIconButton BtnUndoDefault;
        private ScopeXIconButton BtnLoadDefault;
        private ComboBoxEx CbxFile;
        private ScopeXIconButton BtnSelectFile;
        private ScopeXLabel LblSaver;
        //private UUVerticalSplitLine SplitLine;
        //private UUVerticalSplitLine uuVerticalSplitLine1;
        private ScopeXLabel LblReader;
        private ScopeXIconButton BtnOpenDir;
        private ScopeXLabel LabelPath;
    }
}
