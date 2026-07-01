
using PdfSharpCore.Drawing;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    partial class VerticalPage
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
            if (disposing)
            {
                if (Font != null)
                {
                    Font = null;
                }
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
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt5 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt6 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt7 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt8 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt9 = new UserControls.DefaultHighlightPrompt();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle1 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle1 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle2 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle3 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle2 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle4 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle5 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle6 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt10 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt11 = new UserControls.DefaultHighlightPrompt();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle3 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle7 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle8 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle9 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle4 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle10 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle11 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle12 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            TbxUnit = new UserControls.ScopeXTextBox();
            LblLabelVisiblity = new UserControls.ScopeXLabel();
            ChkActive = new UserControls.ScopeXSwitchButton();
            ChkBandLimit = new UserControls.SelectComboBox();
            LblActive = new UserControls.ScopeXLabel();
            LblInvert = new UserControls.ScopeXLabel();
            LblbandLimit = new UserControls.ScopeXLabel();
            TbxLabel = new UserControls.ScopeXTextBox();
            ChkInvert = new UserControls.ScopeXSwitchButton();
            LblCoupling = new UserControls.ScopeXLabel();
            LblScale = new UserControls.ScopeXLabel();
            LblLabel = new UserControls.ScopeXLabel();
            BtnResetPos = new UserControls.ScopeXIconButton();
            BtnBias = new UserControls.ScopeXIconButton();
            LblPosition = new UserControls.ScopeXLabel();
            BtnResetBias = new UserControls.ScopeXIconButton();
            LblBias = new UserControls.ScopeXLabel();
            NebScale = new UserControls.TouchNeb();
            CbxCoupling = new UserControls.SelectComboBox();
            BtnPosition = new UserControls.ScopeXIconButton();
            ChkIndependentWindow = new UserControls.ScopeXSwitchButton();
            LblAmplitudeSelection = new UserControls.ScopeXLabel();
            ChkAmplitude = new UserControls.ScopeXSwitchButton();
            ChkLabelVisiblity = new UserControls.ScopeXSwitchButton();
            LblIndependentWindow = new UserControls.ScopeXLabel();
            LblChannelDelay = new UserControls.ScopeXLabel();
            NebChannelDelay = new UserControls.TouchNeb();
            BtnCopyToOtherChannel = new UserControls.ScopeXIconButton();
            ChkOtherChannelSelect = new UserControls.SelectComboBox();
            SuspendLayout();
            // 
            // TbxUnit
            // 
            TbxUnit.AcceptsTab = false;
            TbxUnit.Adjustable = false;
            TbxUnit.AutoShowKeyBoard = true;
            TbxUnit.AutoSize = false;
            TbxUnit.BackColor = Color.FromArgb(53, 54, 58);
            TbxUnit.BorderColor = Color.FromArgb(53, 54, 58);
            TbxUnit.BorderThickness = 0;
            TbxUnit.ClickedBorderColor = Color.White;
            TbxUnit.CornerRadius = 0;
            TbxUnit.Cursor = Cursors.IBeam;
            TbxUnit.DoubleClickEnable = false;
            TbxUnit.Enabled = true;
            TbxUnit.EnbleSelectBorder = true;
            TbxUnit.FineEnable = false;
            TbxUnit.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point);
            TbxUnit.ForeColor = Color.FromArgb(185, 192, 199);
            TbxUnit.Height = 30;
            TbxUnit.HideSelection = true;
            TbxUnit.IsFocusClicked = false;
            TbxUnit.KBMaxCharCount = int.MaxValue;
            TbxUnit.KeyboardVerify = null;
            TbxUnit.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TbxUnit.Location = new Point(331, 176);
            TbxUnit.MaxLength = 32767;
            TbxUnit.Modified = false;
            TbxUnit.MouseEnterState = false;
            TbxUnit.Multiline = false;
            TbxUnit.Name = "TbxUnit";
            TbxUnit.ProcessCmdKeyFunc = null;
            TbxUnit.ReadOnly = false;
            TbxUnit.SelectedColor = Color.FromArgb(0, 157, 255);
            TbxUnit.SelectedText = "";
            TbxUnit.SelectionLength = 0;
            TbxUnit.SelectionStart = 0;
            TbxUnit.ShortcutsEnabled = true;
            TbxUnit.Size = new Size(200, 30);
            TbxUnit.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxUnit.StylizeFlag = true;
            TbxUnit.TabIndex = 16;
            TbxUnit.TextAlign = HorizontalAlignment.Left;
            TbxUnit.UseSystemPasswordChar = false;
            TbxUnit.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            TbxUnit.Visible = false;
            TbxUnit.WordWrap = true;
            TbxUnit.TextChanged += TbxUnit_SelectedIndexChanged;
            // 
            // LblLabelVisiblity
            // 
            LblLabelVisiblity.BackColor = Color.FromArgb(41, 42, 45);
            LblLabelVisiblity.BorderColor = Color.FromArgb(53, 54, 58);
            LblLabelVisiblity.BorderThickness = 0;
            LblLabelVisiblity.CornerRadius = 0;
            LblLabelVisiblity.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LblLabelVisiblity.ForeColor = Color.FromArgb(232, 234, 237);
            LblLabelVisiblity.HighlightPrompt = defaultHighlightPrompt1;
            LblLabelVisiblity.IsOmittext = true;
            LblLabelVisiblity.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblLabelVisiblity.Location = new Point(441, 222);
            LblLabelVisiblity.MultyLineFlag = false;
            LblLabelVisiblity.Name = "LblLabelVisiblity";
            LblLabelVisiblity.Size = new Size(110, 21);
            LblLabelVisiblity.StyleFlags = UserControls.Style.StyleFlag.None;
            LblLabelVisiblity.StylizeFlag = true;
            LblLabelVisiblity.TabIndex = 15;
            LblLabelVisiblity.TabStop = false;
            LblLabelVisiblity.TextAlign = ContentAlignment.MiddleLeft;
            LblLabelVisiblity.Token = null;
            LblLabelVisiblity.ToolTip = true;
            // 
            // ChkActive
            // 
            ChkActive.AnimationCount = 8;
            ChkActive.AnimationFunc = null;
            ChkActive.BackColor = Color.FromArgb(53, 54, 58);
            ChkActive.BorderColor = Color.FromArgb(53, 54, 58);
            ChkActive.BorderThickness = 0;
            ChkActive.Checked = false;
            ChkActive.CheckedBackColor = Color.FromArgb(0, 157, 255);
            ChkActive.CheckedForeColor = Color.Black;
            ChkActive.CheckedSliderColor = Color.FromArgb(232, 234, 237);
            ChkActive.CheckedText = "开";
            ChkActive.Cursor = Cursors.Hand;
            ChkActive.DropKey = Keys.Space;
            ChkActive.FocusBorderColor = Color.FromArgb(53, 54, 58);
            ChkActive.ForeColor = Color.FromArgb(185, 192, 199);
            ChkActive.Height = 30;
            ChkActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkActive.Location = new Point(10, 28);
            ChkActive.Margin = new Padding(0);
            ChkActive.Name = "ChkActive";
            ChkActive.Size = new Size(75, 30);
            ChkActive.SliderButtonWidth = 30;
            ChkActive.SliderColor = Color.FromArgb(232, 234, 237);
            ChkActive.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkActive.StylizeFlag = true;
            ChkActive.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkActive.TabIndex = 1;
            ChkActive.Text = "关";
            ChkActive.UseAnimation = true;
            ChkActive.CheckedChangedEvent += ChkActive_CheckedChangedEvent;
            // 
            // ChkBandLimit
            // 
            ChkBandLimit.AutoSize = true;
            ChkBandLimit.BackColor = Color.FromArgb(53, 54, 58);
            ChkBandLimit.BorderColor = Color.FromArgb(53, 54, 58);
            ChkBandLimit.ComBorderColor = Color.Blue;
            ChkBandLimit.DataSource = null;
            ChkBandLimit.ExtText = "";
            ChkBandLimit.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point);
            ChkBandLimit.ForeColor = Color.White;
            ChkBandLimit.ImeMode = ImeMode.NoControl;
            ChkBandLimit.Location = new Point(331, 102);
            ChkBandLimit.MaximumSize = new Size(99999, 99999);
            ChkBandLimit.Name = "ChkBandLimit";
            ChkBandLimit.SelectIndex = -1;
            ChkBandLimit.SelectValue = null;
            ChkBandLimit.Size = new Size(200, 30);
            ChkBandLimit.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            ChkBandLimit.StylizeFlag = true;
            ChkBandLimit.TabIndex = 23;
            // 
            // LblActive
            // 
            LblActive.BackColor = Color.FromArgb(41, 42, 45);
            LblActive.BorderColor = Color.FromArgb(53, 54, 58);
            LblActive.BorderThickness = 0;
            LblActive.CornerRadius = 0;
            LblActive.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LblActive.ForeColor = Color.FromArgb(232, 234, 237);
            LblActive.HighlightPrompt = defaultHighlightPrompt2;
            LblActive.ImeMode = ImeMode.NoControl;
            LblActive.IsOmittext = true;
            LblActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblActive.Location = new Point(10, 0);
            LblActive.MultyLineFlag = false;
            LblActive.Name = "LblActive";
            LblActive.Size = new Size(75, 24);
            LblActive.StyleFlags = UserControls.Style.StyleFlag.None;
            LblActive.StylizeFlag = true;
            LblActive.TabIndex = 0;
            LblActive.TabStop = false;
            LblActive.TextAlign = ContentAlignment.MiddleLeft;
            LblActive.Token = null;
            LblActive.ToolTip = true;
            // 
            // LblInvert
            // 
            LblInvert.BackColor = Color.FromArgb(41, 42, 45);
            LblInvert.BorderColor = Color.FromArgb(53, 54, 58);
            LblInvert.BorderThickness = 0;
            LblInvert.CornerRadius = 0;
            LblInvert.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LblInvert.ForeColor = Color.FromArgb(232, 234, 237);
            LblInvert.HighlightPrompt = defaultHighlightPrompt3;
            LblInvert.ImeMode = ImeMode.NoControl;
            LblInvert.IsOmittext = true;
            LblInvert.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblInvert.Location = new Point(117, 0);
            LblInvert.MultyLineFlag = false;
            LblInvert.Name = "LblInvert";
            LblInvert.Size = new Size(75, 21);
            LblInvert.StyleFlags = UserControls.Style.StyleFlag.None;
            LblInvert.StylizeFlag = true;
            LblInvert.TabIndex = 2;
            LblInvert.TabStop = false;
            LblInvert.TextAlign = ContentAlignment.MiddleLeft;
            LblInvert.Token = null;
            LblInvert.ToolTip = true;
            LblInvert.Visible = false;
            // 
            // LblbandLimit
            // 
            LblbandLimit.BackColor = Color.FromArgb(41, 42, 45);
            LblbandLimit.BorderColor = Color.FromArgb(53, 54, 58);
            LblbandLimit.BorderThickness = 0;
            LblbandLimit.CornerRadius = 0;
            LblbandLimit.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LblbandLimit.ForeColor = Color.FromArgb(232, 234, 237);
            LblbandLimit.HighlightPrompt = defaultHighlightPrompt4;
            LblbandLimit.IsOmittext = true;
            LblbandLimit.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblbandLimit.Location = new Point(331, 74);
            LblbandLimit.MultyLineFlag = false;
            LblbandLimit.Name = "LblbandLimit";
            LblbandLimit.Size = new Size(200, 21);
            LblbandLimit.StyleFlags = UserControls.Style.StyleFlag.None;
            LblbandLimit.StylizeFlag = true;
            LblbandLimit.TabIndex = 10;
            LblbandLimit.TabStop = false;
            LblbandLimit.TextAlign = ContentAlignment.MiddleLeft;
            LblbandLimit.Token = null;
            LblbandLimit.ToolTip = true;
            // 
            // TbxLabel
            // 
            TbxLabel.AcceptsTab = false;
            TbxLabel.Adjustable = false;
            TbxLabel.AutoShowKeyBoard = true;
            TbxLabel.AutoSize = false;
            TbxLabel.BackColor = Color.FromArgb(53, 54, 58);
            TbxLabel.BorderColor = Color.FromArgb(53, 54, 58);
            TbxLabel.BorderThickness = 0;
            TbxLabel.ClickedBorderColor = Color.White;
            TbxLabel.CornerRadius = 0;
            TbxLabel.Cursor = Cursors.IBeam;
            TbxLabel.DoubleClickEnable = false;
            TbxLabel.Enabled = true;
            TbxLabel.EnbleSelectBorder = true;
            TbxLabel.FineEnable = false;
            TbxLabel.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            TbxLabel.ForeColor = Color.FromArgb(185, 192, 199);
            TbxLabel.Height = 30;
            TbxLabel.HideSelection = true;
            TbxLabel.IsFocusClicked = false;
            TbxLabel.KBMaxCharCount = int.MaxValue;
            TbxLabel.KeyboardVerify = null;
            TbxLabel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TbxLabel.Location = new Point(331, 250);
            TbxLabel.MaxLength = 32767;
            TbxLabel.Modified = false;
            TbxLabel.MouseEnterState = false;
            TbxLabel.Multiline = false;
            TbxLabel.Name = "TbxLabel";
            TbxLabel.ProcessCmdKeyFunc = null;
            TbxLabel.ReadOnly = false;
            TbxLabel.SelectedColor = Color.FromArgb(0, 157, 255);
            TbxLabel.SelectedText = "";
            TbxLabel.SelectionLength = 0;
            TbxLabel.SelectionStart = 0;
            TbxLabel.ShortcutsEnabled = true;
            TbxLabel.Size = new Size(100, 30);
            TbxLabel.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxLabel.StylizeFlag = true;
            TbxLabel.TabIndex = 21;
            TbxLabel.TextAlign = HorizontalAlignment.Left;
            TbxLabel.UseSystemPasswordChar = false;
            TbxLabel.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            TbxLabel.WordWrap = true;
            TbxLabel.TextChanged += TbxLabel_SelectedIndexChanged;
            // 
            // ChkInvert
            // 
            ChkInvert.AnimationCount = 8;
            ChkInvert.AnimationFunc = null;
            ChkInvert.BackColor = Color.FromArgb(53, 54, 58);
            ChkInvert.BorderColor = Color.FromArgb(53, 54, 58);
            ChkInvert.BorderThickness = 0;
            ChkInvert.Checked = false;
            ChkInvert.CheckedBackColor = Color.FromArgb(0, 157, 255);
            ChkInvert.CheckedForeColor = Color.Black;
            ChkInvert.CheckedSliderColor = Color.FromArgb(232, 234, 237);
            ChkInvert.CheckedText = "开";
            ChkInvert.Cursor = Cursors.Hand;
            ChkInvert.DropKey = Keys.Space;
            ChkInvert.FocusBorderColor = Color.FromArgb(53, 54, 58);
            ChkInvert.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            ChkInvert.ForeColor = Color.FromArgb(185, 192, 199);
            ChkInvert.Height = 30;
            ChkInvert.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkInvert.Location = new Point(117, 28);
            ChkInvert.Margin = new Padding(0);
            ChkInvert.MinimumSize = new Size(1, 1);
            ChkInvert.Name = "ChkInvert";
            ChkInvert.Size = new Size(75, 30);
            ChkInvert.SliderButtonWidth = 30;
            ChkInvert.SliderColor = Color.FromArgb(232, 234, 237);
            ChkInvert.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkInvert.StylizeFlag = true;
            ChkInvert.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkInvert.TabIndex = 3;
            ChkInvert.Text = "关";
            ChkInvert.UseAnimation = true;
            ChkInvert.CheckedChangedEvent += ChkInvert_CheckedChangedEvent;
            ChkInvert.Visible=false;
            // 
            // LblCoupling
            // 
            LblCoupling.BackColor = Color.FromArgb(41, 42, 45);
            LblCoupling.BorderColor = Color.FromArgb(53, 54, 58);
            LblCoupling.BorderThickness = 0;
            LblCoupling.CornerRadius = 0;
            LblCoupling.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LblCoupling.ForeColor = Color.FromArgb(232, 234, 237);
            LblCoupling.HighlightPrompt = defaultHighlightPrompt5;
            LblCoupling.IsOmittext = true;
            LblCoupling.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblCoupling.Location = new Point(331, 0);
            LblCoupling.MultyLineFlag = false;
            LblCoupling.Name = "LblCoupling";
            LblCoupling.Size = new Size(200, 24);
            LblCoupling.StyleFlags = UserControls.Style.StyleFlag.None;
            LblCoupling.StylizeFlag = true;
            LblCoupling.TabIndex = 6;
            LblCoupling.TabStop = false;
            LblCoupling.TextAlign = ContentAlignment.MiddleLeft;
            LblCoupling.Token = null;
            LblCoupling.ToolTip = true;
            // 
            // LblScale
            // 
            LblScale.BackColor = Color.FromArgb(41, 42, 45);
            LblScale.BorderColor = Color.FromArgb(53, 54, 58);
            LblScale.BorderThickness = 0;
            LblScale.CornerRadius = 0;
            LblScale.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LblScale.ForeColor = Color.FromArgb(232, 234, 237);
            LblScale.HighlightPrompt = defaultHighlightPrompt6;
            LblScale.ImeMode = ImeMode.NoControl;
            LblScale.IsOmittext = true;
            LblScale.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblScale.Location = new Point(10, 74);
            LblScale.MultyLineFlag = false;
            LblScale.Name = "LblScale";
            LblScale.Size = new Size(211, 21);
            LblScale.StyleFlags = UserControls.Style.StyleFlag.None;
            LblScale.StylizeFlag = true;
            LblScale.TabIndex = 8;
            LblScale.TabStop = false;
            LblScale.TextAlign = ContentAlignment.MiddleLeft;
            LblScale.Token = null;
            LblScale.ToolTip = true;
            // 
            // LblLabel
            // 
            LblLabel.BackColor = Color.FromArgb(41, 42, 45);
            LblLabel.BorderColor = Color.FromArgb(53, 54, 58);
            LblLabel.BorderThickness = 0;
            LblLabel.CornerRadius = 0;
            LblLabel.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LblLabel.ForeColor = Color.FromArgb(232, 234, 237);
            LblLabel.HighlightPrompt = defaultHighlightPrompt7;
            LblLabel.IsOmittext = true;
            LblLabel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblLabel.Location = new Point(331, 222);
            LblLabel.MultyLineFlag = false;
            LblLabel.Name = "LblLabel";
            LblLabel.Size = new Size(100, 21);
            LblLabel.StyleFlags = UserControls.Style.StyleFlag.None;
            LblLabel.StylizeFlag = true;
            LblLabel.TabIndex = 20;
            LblLabel.TabStop = false;
            LblLabel.TextAlign = ContentAlignment.MiddleLeft;
            LblLabel.Token = null;
            LblLabel.ToolTip = true;
            // 
            // BtnResetPos
            // 
            BtnResetPos.Adjustable = false;
            BtnResetPos.BackColor = Color.Transparent;
            BtnResetPos.BorderColor = Color.FromArgb(53, 54, 58);
            BtnResetPos.BorderThickness = 0;
            BtnResetPos.ChoosedBackColor = Color.FromArgb(40, 71, 193);
            BtnResetPos.ChoosedForeColor = Color.FromArgb(192, 192, 192);
            BtnResetPos.ChoosedMouseinBackColor = Color.FromArgb(40, 71, 193);
            BtnResetPos.ChoosedPressedBackColor = Color.FromArgb(40, 71, 193);
            BtnResetPos.CornerRadius = 0;
            BtnResetPos.Cursor = Cursors.Hand;
            BtnResetPos.DaskArray = null;
            BtnResetPos.DoubleClickEnable = true;
            BtnResetPos.DropKey = Keys.Space;
            BtnResetPos.FineEnable = true;
            BtnResetPos.FocusedBorderColor = Color.DeepSkyBlue;
            BtnResetPos.ForeColor = SystemColors.ActiveCaptionText;
            BtnResetPos.Height = 30;
            BtnResetPos.Icon = null;
            BtnResetPos.IconOffset = 10;
            BtnResetPos.IconSize = new Size(24, 24);
            BtnResetPos.ImeMode = ImeMode.NoControl;
            BtnResetPos.IndicatorColor = Color.FromArgb(40, 71, 193);
            BtnResetPos.IsChoosed = false;
            BtnResetPos.IsIndicatorShow = false;
            BtnResetPos.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnResetPos.Location = new Point(227, 176);
            BtnResetPos.MouseinBackColor = Color.FromArgb(53, 54, 58);
            BtnResetPos.MouseinBorderColor = Color.FromArgb(53, 54, 58);
            BtnResetPos.MouseInBorderThickness = 0;
            BtnResetPos.MouseinForeColor = Color.FromArgb(185, 192, 199);
            BtnResetPos.MouseinSvgForeColor = Color.FromArgb(0, 157, 255);
            BtnResetPos.Name = "BtnResetPos";
            BtnResetPos.PressedBackColor = Color.Gray;
            BtnResetPos.PressedBorderColor = Color.FromArgb(53, 54, 58);
            BtnResetPos.PressedBorderThickness = 0;
            BtnResetPos.PressedForeColor = Color.FromArgb(185, 192, 199);
            BtnResetPos.PressedSvgForeColor = Color.FromArgb(0, 157, 255);
            BtnResetPos.Size = new Size(70, 30);
            BtnResetPos.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnResetPos.StylizeFlag = true;
            BtnResetPos.SVGForeColor = Color.FromArgb(185, 192, 199);
            BtnResetPos.SVGPath = "";
            BtnResetPos.TabIndex = 14;
            BtnResetPos.TextAlign = ContentAlignment.MiddleCenter;
            BtnResetPos.Click += BtnResetPos_Click;
            // 
            // BtnBias
            // 
            BtnBias.Adjustable = false;
            BtnBias.BackColor = Color.Transparent;
            BtnBias.BorderColor = Color.FromArgb(67, 69, 76);
            BtnBias.BorderThickness = 1;
            BtnBias.ChoosedBackColor = Color.FromArgb(40, 71, 193);
            BtnBias.ChoosedForeColor = Color.FromArgb(192, 192, 192);
            BtnBias.ChoosedMouseinBackColor = Color.FromArgb(40, 71, 193);
            BtnBias.ChoosedPressedBackColor = Color.FromArgb(40, 71, 193);
            BtnBias.CornerRadius = 0;
            BtnBias.Cursor = Cursors.Hand;
            BtnBias.DaskArray = null;
            BtnBias.DoubleClickEnable = true;
            BtnBias.DropKey = Keys.Space;
            BtnBias.FineEnable = true;
            BtnBias.FocusedBorderColor = Color.DeepSkyBlue;
            BtnBias.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnBias.ForeColor = SystemColors.ActiveCaptionText;
            BtnBias.Height = 30;
            BtnBias.Icon = null;
            BtnBias.IconOffset = 10;
            BtnBias.IconSize = new Size(24, 24);
            BtnBias.ImeMode = ImeMode.NoControl;
            BtnBias.IndicatorColor = Color.FromArgb(40, 71, 193);
            BtnBias.IsChoosed = false;
            BtnBias.IsIndicatorShow = false;
            BtnBias.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnBias.Location = new Point(10, 250);
            BtnBias.MouseinBackColor = Color.FromArgb(53, 54, 58);
            BtnBias.MouseinBorderColor = Color.FromArgb(53, 54, 58);
            BtnBias.MouseInBorderThickness = 0;
            BtnBias.MouseinForeColor = Color.FromArgb(185, 192, 199);
            BtnBias.MouseinSvgForeColor = Color.FromArgb(0, 157, 255);
            BtnBias.Name = "BtnBias";
            BtnBias.PressedBackColor = Color.Gray;
            BtnBias.PressedBorderColor = Color.FromArgb(53, 54, 58);
            BtnBias.PressedBorderThickness = 0;
            BtnBias.PressedForeColor = Color.FromArgb(185, 192, 199);
            BtnBias.PressedSvgForeColor = Color.FromArgb(0, 157, 255);
            BtnBias.Size = new Size(211, 30);
            BtnBias.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnBias.StylizeFlag = true;
            BtnBias.SVGForeColor = Color.FromArgb(185, 192, 199);
            BtnBias.SVGPath = "";
            BtnBias.TabIndex = 18;
            BtnBias.Text = "0mV";
            BtnBias.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LblPosition
            // 
            LblPosition.BackColor = Color.FromArgb(41, 42, 45);
            LblPosition.BorderColor = Color.FromArgb(53, 54, 58);
            LblPosition.BorderThickness = 0;
            LblPosition.CornerRadius = 0;
            LblPosition.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LblPosition.ForeColor = Color.FromArgb(232, 234, 237);
            LblPosition.HighlightPrompt = defaultHighlightPrompt8;
            LblPosition.IsOmittext = true;
            LblPosition.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblPosition.Location = new Point(10, 148);
            LblPosition.MultyLineFlag = false;
            LblPosition.Name = "LblPosition";
            LblPosition.Size = new Size(211, 21);
            LblPosition.StyleFlags = UserControls.Style.StyleFlag.None;
            LblPosition.StylizeFlag = true;
            LblPosition.TabIndex = 12;
            LblPosition.TabStop = false;
            LblPosition.TextAlign = ContentAlignment.MiddleLeft;
            LblPosition.Token = null;
            LblPosition.ToolTip = true;
            // 
            // BtnResetBias
            // 
            BtnResetBias.Adjustable = false;
            BtnResetBias.BackColor = Color.Transparent;
            BtnResetBias.BorderColor = Color.FromArgb(53, 54, 58);
            BtnResetBias.BorderThickness = 0;
            BtnResetBias.ChoosedBackColor = Color.FromArgb(40, 71, 193);
            BtnResetBias.ChoosedForeColor = Color.FromArgb(192, 192, 192);
            BtnResetBias.ChoosedMouseinBackColor = Color.FromArgb(40, 71, 193);
            BtnResetBias.ChoosedPressedBackColor = Color.FromArgb(40, 71, 193);
            BtnResetBias.CornerRadius = 0;
            BtnResetBias.Cursor = Cursors.Hand;
            BtnResetBias.DaskArray = null;
            BtnResetBias.DoubleClickEnable = true;
            BtnResetBias.DropKey = Keys.Space;
            BtnResetBias.FineEnable = true;
            BtnResetBias.FocusedBorderColor = Color.DeepSkyBlue;
            BtnResetBias.ForeColor = SystemColors.ActiveCaptionText;
            BtnResetBias.Height = 30;
            BtnResetBias.Icon = null;
            BtnResetBias.IconOffset = 10;
            BtnResetBias.IconSize = new Size(24, 24);
            BtnResetBias.ImeMode = ImeMode.NoControl;
            BtnResetBias.IndicatorColor = Color.FromArgb(40, 71, 193);
            BtnResetBias.IsChoosed = false;
            BtnResetBias.IsIndicatorShow = false;
            BtnResetBias.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnResetBias.Location = new Point(227, 250);
            BtnResetBias.MouseinBackColor = Color.FromArgb(53, 54, 58);
            BtnResetBias.MouseinBorderColor = Color.FromArgb(53, 54, 58);
            BtnResetBias.MouseInBorderThickness = 0;
            BtnResetBias.MouseinForeColor = Color.FromArgb(185, 192, 199);
            BtnResetBias.MouseinSvgForeColor = Color.FromArgb(0, 157, 255);
            BtnResetBias.Name = "BtnResetBias";
            BtnResetBias.PressedBackColor = Color.Gray;
            BtnResetBias.PressedBorderColor = Color.FromArgb(53, 54, 58);
            BtnResetBias.PressedBorderThickness = 0;
            BtnResetBias.PressedForeColor = Color.FromArgb(185, 192, 199);
            BtnResetBias.PressedSvgForeColor = Color.FromArgb(0, 157, 255);
            BtnResetBias.Size = new Size(70, 30);
            BtnResetBias.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnResetBias.StylizeFlag = true;
            BtnResetBias.SVGForeColor = Color.FromArgb(185, 192, 199);
            BtnResetBias.SVGPath = "";
            BtnResetBias.TabIndex = 19;
            BtnResetBias.TextAlign = ContentAlignment.MiddleCenter;
            BtnResetBias.Click += BtnResetBias_Click;
            // 
            // LblBias
            // 
            LblBias.BackColor = Color.FromArgb(41, 42, 45);
            LblBias.BorderColor = Color.FromArgb(53, 54, 58);
            LblBias.BorderThickness = 0;
            LblBias.CornerRadius = 0;
            LblBias.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LblBias.ForeColor = Color.FromArgb(232, 234, 237);
            LblBias.HighlightPrompt = defaultHighlightPrompt9;
            LblBias.ImeMode = ImeMode.NoControl;
            LblBias.IsOmittext = true;
            LblBias.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblBias.Location = new Point(10, 222);
            LblBias.MultyLineFlag = false;
            LblBias.Name = "LblBias";
            LblBias.Size = new Size(211, 21);
            LblBias.StyleFlags = UserControls.Style.StyleFlag.None;
            LblBias.StylizeFlag = true;
            LblBias.TabIndex = 17;
            LblBias.TabStop = false;
            LblBias.TextAlign = ContentAlignment.MiddleLeft;
            LblBias.Token = null;
            LblBias.ToolTip = true;
            // 
            // NebScale
            // 
            NebScale.AddButtonImg = Properties.Resources.WaveVerticalRoomOut;
            buttonBaseStyle1.BackColor = Color.FromArgb(0, 157, 255);
            buttonBaseStyle1.BorderColor = Color.Transparent;
            buttonBaseStyle1.BorderThickness = 0;
            buttonBaseStyle1.ForeColor = Color.White;
            buttonStyle1.MouseClickStyle = buttonBaseStyle1;
            buttonBaseStyle2.BackColor = Color.FromArgb(0, 157, 255);
            buttonBaseStyle2.BorderColor = Color.Green;
            buttonBaseStyle2.BorderThickness = 0;
            buttonBaseStyle2.ForeColor = Color.White;
            buttonStyle1.MouseInStyle = buttonBaseStyle2;
            buttonBaseStyle3.BackColor = Color.FromArgb(0, 157, 255);
            buttonBaseStyle3.BorderColor = Color.Transparent;
            buttonBaseStyle3.BorderThickness = 0;
            buttonBaseStyle3.ForeColor = Color.White;
            buttonStyle1.NomalStyle = buttonBaseStyle3;
            NebScale.AddButtonStyle = buttonStyle1;
            NebScale.Adjustable = false;
            NebScale.AllwaysShowFocusImage = false;
            NebScale.BackColor = Color.FromArgb(53, 54, 58);
            NebScale.BorderColor = Color.FromArgb(53, 54, 58);
            NebScale.BorderThickness = 0;
            NebScale.ClickedBorderColor = Color.White;
            NebScale.DisableHoldOnInput = true;
            NebScale.DoubleClickEnable = false;
            NebScale.DropKey = Keys.Space;
            NebScale.FineEnable = false;
            NebScale.FocusBoederColor = Color.FromArgb(53, 54, 58);
            NebScale.FocusForeColor = Color.FromArgb(0, 157, 255);
            NebScale.FocusImage = null;
            NebScale.FocusImagePosition = UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebScale.FocusImageRect = new Rectangle(0, 0, 0, 0);
            NebScale.ForeColor = Color.FromArgb(185, 192, 199);
            NebScale.Height = 30;
            NebScale.HoldOnSpeedLevel = 5;
            NebScale.IconWidthProportion = 1F;
            NebScale.Interval = 0.1D;
            NebScale.IsFocusClicked = false;
            NebScale.LanguageKey = null;
            NebScale.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebScale.Location = new Point(10, 102);
            NebScale.MaxValue = double.MaxValue;
            NebScale.MinValue = double.MinValue;
            NebScale.Name = "NebScale";
            NebScale.Size = new Size(211, 30);
            NebScale.StringFormatFunc = null;
            NebScale.StyleFlags = UserControls.Style.StyleFlag.None;
            NebScale.StylizeFlag = true;
            NebScale.SubButtonImg = Properties.Resources.WaveVerticalRoomIn;
            buttonBaseStyle4.BackColor = Color.FromArgb(0, 157, 255);
            buttonBaseStyle4.BorderColor = Color.Transparent;
            buttonBaseStyle4.BorderThickness = 0;
            buttonBaseStyle4.ForeColor = Color.White;
            buttonStyle2.MouseClickStyle = buttonBaseStyle4;
            buttonBaseStyle5.BackColor = Color.FromArgb(0, 157, 255);
            buttonBaseStyle5.BorderColor = Color.Green;
            buttonBaseStyle5.BorderThickness = 0;
            buttonBaseStyle5.ForeColor = Color.White;
            buttonStyle2.MouseInStyle = buttonBaseStyle5;
            buttonBaseStyle6.BackColor = Color.FromArgb(0, 157, 255);
            buttonBaseStyle6.BorderColor = Color.Transparent;
            buttonBaseStyle6.BorderThickness = 0;
            buttonBaseStyle6.ForeColor = Color.White;
            buttonStyle2.NomalStyle = buttonBaseStyle6;
            NebScale.SubButtonStyle = buttonStyle2;
            NebScale.TabIndex = 9;
            NebScale.Value = 0D;
            // 
            // CbxCoupling
            // 
            CbxCoupling.AutoSize = true;
            CbxCoupling.BackColor = Color.FromArgb(53, 54, 58);
            CbxCoupling.BorderColor = Color.FromArgb(53, 54, 58);
            CbxCoupling.ComBorderColor = Color.Blue;
            CbxCoupling.DataSource = null;
            CbxCoupling.ExtText = "";
            CbxCoupling.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point);
            CbxCoupling.ForeColor = Color.White;
            CbxCoupling.ImeMode = ImeMode.NoControl;
            CbxCoupling.Location = new Point(331, 28);
            CbxCoupling.MaximumSize = new Size(99999, 99999);
            CbxCoupling.Name = "CbxCoupling";
            CbxCoupling.SelectIndex = -1;
            CbxCoupling.SelectValue = null;
            CbxCoupling.Size = new Size(200, 30);
            CbxCoupling.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxCoupling.StylizeFlag = true;
            CbxCoupling.Enabled = false;
            CbxCoupling.TabIndex = 22;

            // 
            // BtnPosition
            // 
            BtnPosition.Adjustable = false;
            BtnPosition.BackColor = Color.Transparent;
            BtnPosition.BorderColor = Color.FromArgb(67, 69, 76);
            BtnPosition.BorderThickness = 1;
            BtnPosition.ChoosedBackColor = Color.FromArgb(40, 71, 193);
            BtnPosition.ChoosedForeColor = Color.FromArgb(192, 192, 192);
            BtnPosition.ChoosedMouseinBackColor = Color.FromArgb(40, 71, 193);
            BtnPosition.ChoosedPressedBackColor = Color.FromArgb(40, 71, 193);
            BtnPosition.CornerRadius = 0;
            BtnPosition.Cursor = Cursors.Hand;
            BtnPosition.DaskArray = null;
            BtnPosition.DoubleClickEnable = true;
            BtnPosition.DropKey = Keys.Space;
            BtnPosition.FineEnable = true;
            BtnPosition.FocusedBorderColor = Color.DeepSkyBlue;
            BtnPosition.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnPosition.ForeColor = SystemColors.ActiveCaptionText;
            BtnPosition.Height = 30;
            BtnPosition.Icon = null;
            BtnPosition.IconOffset = 10;
            BtnPosition.IconSize = new Size(24, 24);
            BtnPosition.ImeMode = ImeMode.NoControl;
            BtnPosition.IndicatorColor = Color.FromArgb(40, 71, 193);
            BtnPosition.IsChoosed = false;
            BtnPosition.IsIndicatorShow = false;
            BtnPosition.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnPosition.Location = new Point(10, 176);
            BtnPosition.MouseinBackColor = Color.FromArgb(53, 54, 58);
            BtnPosition.MouseinBorderColor = Color.FromArgb(53, 54, 58);
            BtnPosition.MouseInBorderThickness = 0;
            BtnPosition.MouseinForeColor = Color.FromArgb(185, 192, 199);
            BtnPosition.MouseinSvgForeColor = Color.FromArgb(0, 157, 255);
            BtnPosition.Name = "BtnPosition";
            BtnPosition.PressedBackColor = Color.Gray;
            BtnPosition.PressedBorderColor = Color.FromArgb(53, 54, 58);
            BtnPosition.PressedBorderThickness = 0;
            BtnPosition.PressedForeColor = Color.FromArgb(185, 192, 199);
            BtnPosition.PressedSvgForeColor = Color.FromArgb(0, 157, 255);
            BtnPosition.Size = new Size(211, 30);
            BtnPosition.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnPosition.StylizeFlag = true;
            BtnPosition.SVGForeColor = Color.FromArgb(185, 192, 199);
            BtnPosition.SVGPath = "";
            BtnPosition.TabIndex = 13;
            BtnPosition.Text = "0";
            BtnPosition.TextAlign = ContentAlignment.MiddleCenter;
            BtnPosition.DoubleClick += BtnPosition_DoubleClick;
            // 
            // ChkIndependentWindow
            // 
            ChkIndependentWindow.AnimationCount = 8;
            ChkIndependentWindow.AnimationFunc = null;
            ChkIndependentWindow.BackColor = Color.FromArgb(53, 54, 58);
            ChkIndependentWindow.BorderColor = Color.FromArgb(53, 54, 58);
            ChkIndependentWindow.BorderThickness = 1;
            ChkIndependentWindow.Checked = false;
            ChkIndependentWindow.CheckedBackColor = Color.FromArgb(0, 157, 255);
            ChkIndependentWindow.CheckedForeColor = Color.Black;
            ChkIndependentWindow.CheckedSliderColor = Color.FromArgb(232, 234, 237);
            ChkIndependentWindow.CheckedText = "开";
            ChkIndependentWindow.Cursor = Cursors.Hand;
            ChkIndependentWindow.DropKey = Keys.Space;
            ChkIndependentWindow.FocusBorderColor = Color.FromArgb(53, 54, 58);
            ChkIndependentWindow.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            ChkIndependentWindow.ForeColor = Color.FromArgb(185, 192, 199);
            ChkIndependentWindow.Height = 30;
            ChkIndependentWindow.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkIndependentWindow.Location = new Point(224, 28);
            ChkIndependentWindow.Margin = new Padding(0);
            ChkIndependentWindow.Name = "ChkIndependentWindow";
            ChkIndependentWindow.Size = new Size(75, 30);
            ChkIndependentWindow.SliderButtonWidth = 32;
            ChkIndependentWindow.SliderColor = Color.FromArgb(232, 234, 237);
            ChkIndependentWindow.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkIndependentWindow.StylizeFlag = true;
            ChkIndependentWindow.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkIndependentWindow.TabIndex = 5;
            ChkIndependentWindow.Text = "关";
            ChkIndependentWindow.UseAnimation = true;
            ChkIndependentWindow.Visible = false;
            ChkIndependentWindow.CheckedChangedEvent += ChkIndependentWindow_CheckedChangedEvent;
            // 
            // LblAmplitudeSelection
            // 
            LblAmplitudeSelection.BackColor = Color.FromArgb(41, 42, 45);
            LblAmplitudeSelection.BorderColor = Color.FromArgb(53, 54, 58);
            LblAmplitudeSelection.BorderThickness = 0;
            LblAmplitudeSelection.CornerRadius = 0;
            LblAmplitudeSelection.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LblAmplitudeSelection.ForeColor = Color.FromArgb(232, 234, 237);
            LblAmplitudeSelection.HighlightPrompt = defaultHighlightPrompt1;
            LblAmplitudeSelection.IsOmittext = true;
            LblAmplitudeSelection.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblAmplitudeSelection.Location = new Point(227, 74);
            LblAmplitudeSelection.MultyLineFlag = false;
            LblAmplitudeSelection.Name = "LblAmplitudeSelection";
            LblAmplitudeSelection.Size = new Size(200, 21);
            LblAmplitudeSelection.StyleFlags = UserControls.Style.StyleFlag.None;
            LblAmplitudeSelection.StylizeFlag = true;
            LblAmplitudeSelection.TabIndex = 15;
            LblAmplitudeSelection.TabStop = false;
            LblAmplitudeSelection.TextAlign = ContentAlignment.MiddleLeft;
            LblAmplitudeSelection.Token = null;
            LblAmplitudeSelection.ToolTip = true;
            // 
            // ChkAmplitude
            // 
            ChkAmplitude.AnimationCount = 8;
            ChkAmplitude.AnimationFunc = null;
            ChkAmplitude.BackColor = Color.FromArgb(53, 54, 58);
            ChkAmplitude.BorderColor = Color.FromArgb(53, 54, 58);
            ChkAmplitude.BorderThickness = 0;
            ChkAmplitude.Checked = false;
            ChkAmplitude.CheckedBackColor = Color.FromArgb(0, 157, 255);
            ChkAmplitude.CheckedForeColor = Color.Black;
            ChkAmplitude.CheckedSliderColor = Color.FromArgb(232, 234, 237);
            ChkAmplitude.CheckedText = "开";
            ChkAmplitude.Cursor = Cursors.Hand;
            ChkAmplitude.DropKey = Keys.Space;
            ChkAmplitude.FocusBorderColor = Color.FromArgb(53, 54, 58);
            ChkAmplitude.ForeColor = Color.FromArgb(185, 192, 199);
            ChkAmplitude.Height = 30;
            ChkAmplitude.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkAmplitude.Location = new Point(227, 102);
            ChkAmplitude.Margin = new Padding(0);
            ChkAmplitude.Name = "ChkAmplitude";
            ChkAmplitude.Size = new Size(75, 30);
            ChkAmplitude.SliderButtonWidth = 30;
            ChkAmplitude.SliderColor = Color.FromArgb(232, 234, 237);
            ChkAmplitude.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkAmplitude.StylizeFlag = true;
            ChkAmplitude.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkAmplitude.TabIndex = 18;
            ChkAmplitude.Text = "关";
            ChkAmplitude.UseAnimation = true;
            ChkAmplitude.CheckedChangedEvent += ChkAmplitude_CheckedChanged;
            // 
            // ChkLabelVisiblity
            // 
            ChkLabelVisiblity.AnimationCount = 8;
            ChkLabelVisiblity.AnimationFunc = null;
            ChkLabelVisiblity.BackColor = Color.FromArgb(53, 54, 58);
            ChkLabelVisiblity.BorderColor = Color.FromArgb(53, 54, 58);
            ChkLabelVisiblity.BorderThickness = 0;
            ChkLabelVisiblity.Checked = false;
            ChkLabelVisiblity.CheckedBackColor = Color.FromArgb(0, 157, 255);
            ChkLabelVisiblity.CheckedForeColor = Color.Black;
            ChkLabelVisiblity.CheckedSliderColor = Color.FromArgb(232, 234, 237);
            ChkLabelVisiblity.CheckedText = "开";
            ChkLabelVisiblity.Cursor = Cursors.Hand;
            ChkLabelVisiblity.DropKey = Keys.Space;
            ChkLabelVisiblity.FocusBorderColor = Color.FromArgb(53, 54, 58);
            ChkLabelVisiblity.ForeColor = Color.FromArgb(185, 192, 199);
            ChkLabelVisiblity.Height = 30;
            ChkLabelVisiblity.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkLabelVisiblity.Location = new Point(441, 250);
            ChkLabelVisiblity.Margin = new Padding(0);
            ChkLabelVisiblity.Name = "ChkLabelVisiblity";
            ChkLabelVisiblity.Size = new Size(85, 30);
            ChkLabelVisiblity.SliderButtonWidth = 30;
            ChkLabelVisiblity.SliderColor = Color.FromArgb(232, 234, 237);
            ChkLabelVisiblity.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkLabelVisiblity.StylizeFlag = true;
            ChkLabelVisiblity.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkLabelVisiblity.TabIndex = 18;
            ChkLabelVisiblity.Text = "关";
            ChkLabelVisiblity.UseAnimation = true;
            ChkLabelVisiblity.CheckedChangedEvent += ChkLabelVisiblity_CheckedChangedEvent;
            // 
            // LblIndependentWindow
            // 
            LblIndependentWindow.BackColor = Color.FromArgb(41, 42, 45);
            LblIndependentWindow.BorderColor = Color.FromArgb(53, 54, 58);
            LblIndependentWindow.BorderThickness = 0;
            LblIndependentWindow.CornerRadius = 0;
            LblIndependentWindow.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LblIndependentWindow.ForeColor = Color.FromArgb(232, 234, 237);
            LblIndependentWindow.HighlightPrompt = defaultHighlightPrompt10;
            LblIndependentWindow.ImeMode = ImeMode.NoControl;
            LblIndependentWindow.IsOmittext = true;
            LblIndependentWindow.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblIndependentWindow.Location = new Point(224, 0);
            LblIndependentWindow.MultyLineFlag = false;
            LblIndependentWindow.Name = "LblIndependentWindow";
            LblIndependentWindow.Size = new Size(75, 21);
            LblIndependentWindow.StyleFlags = UserControls.Style.StyleFlag.None;
            LblIndependentWindow.StylizeFlag = true;
            LblIndependentWindow.TabIndex = 4;
            LblIndependentWindow.TabStop = false;
            LblIndependentWindow.TextAlign = ContentAlignment.MiddleLeft;
            LblIndependentWindow.Token = null;
            LblIndependentWindow.ToolTip = true;
            LblIndependentWindow.Visible = false;
            // 
            // LblChannelDelay
            // 
            LblChannelDelay.BackColor = Color.FromArgb(41, 42, 45);
            LblChannelDelay.BorderColor = Color.FromArgb(53, 54, 58);
            LblChannelDelay.BorderThickness = 0;
            LblChannelDelay.CornerRadius = 0;
            LblChannelDelay.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LblChannelDelay.ForeColor = Color.FromArgb(232, 234, 237);
            LblChannelDelay.HighlightPrompt = defaultHighlightPrompt11;
            LblChannelDelay.ImeMode = ImeMode.NoControl;
            LblChannelDelay.IsOmittext = true;
            LblChannelDelay.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblChannelDelay.Location = new Point(10, 298);
            LblChannelDelay.MultyLineFlag = false;
            LblChannelDelay.Name = "LblChannelDelay";
            LblChannelDelay.Size = new Size(211, 21);
            LblChannelDelay.StyleFlags = UserControls.Style.StyleFlag.None;
            LblChannelDelay.StylizeFlag = true;
            LblChannelDelay.TabIndex = 22;
            LblChannelDelay.TabStop = false;
            LblChannelDelay.TextAlign = ContentAlignment.MiddleLeft;
            LblChannelDelay.Token = null;
            LblChannelDelay.ToolTip = true;
            LblChannelDelay.Visible = false;
            // 
            // NebChannelDelay
            // 
            NebChannelDelay.AddButtonImg = null;
            buttonBaseStyle7.BackColor = Color.FromArgb(0, 157, 255);
            buttonBaseStyle7.BorderColor = Color.Transparent;
            buttonBaseStyle7.BorderThickness = 0;
            buttonBaseStyle7.ForeColor = Color.White;
            buttonStyle3.MouseClickStyle = buttonBaseStyle7;
            buttonBaseStyle8.BackColor = Color.FromArgb(0, 157, 255);
            buttonBaseStyle8.BorderColor = Color.Green;
            buttonBaseStyle8.BorderThickness = 0;
            buttonBaseStyle8.ForeColor = Color.White;
            buttonStyle3.MouseInStyle = buttonBaseStyle8;
            buttonBaseStyle9.BackColor = Color.FromArgb(0, 157, 255);
            buttonBaseStyle9.BorderColor = Color.Transparent;
            buttonBaseStyle9.BorderThickness = 0;
            buttonBaseStyle9.ForeColor = Color.White;
            buttonStyle3.NomalStyle = buttonBaseStyle9;
            NebChannelDelay.AddButtonStyle = buttonStyle3;
            NebChannelDelay.Adjustable = false;
            NebChannelDelay.AllwaysShowFocusImage = false;
            NebChannelDelay.BackColor = Color.FromArgb(53, 54, 58);
            NebChannelDelay.BorderColor = Color.FromArgb(53, 54, 58);
            NebChannelDelay.BorderThickness = 0;
            NebChannelDelay.ClickedBorderColor = Color.White;
            NebChannelDelay.DisableHoldOnInput = false;
            NebChannelDelay.DoubleClickEnable = false;
            NebChannelDelay.DropKey = Keys.Space;
            NebChannelDelay.FineEnable = false;
            NebChannelDelay.FocusBoederColor = Color.FromArgb(53, 54, 58);
            NebChannelDelay.FocusForeColor = Color.FromArgb(0, 157, 255);
            NebChannelDelay.FocusImage = null;
            NebChannelDelay.FocusImagePosition = UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebChannelDelay.FocusImageRect = new Rectangle(0, 0, 0, 0);
            NebChannelDelay.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            NebChannelDelay.ForeColor = Color.FromArgb(185, 192, 199);
            NebChannelDelay.Height = 30;
            NebChannelDelay.HoldOnSpeedLevel = 10;
            NebChannelDelay.IconWidthProportion = 1F;
            NebChannelDelay.Interval = 1E-08D;
            NebChannelDelay.IsFocusClicked = false;
            NebChannelDelay.LanguageKey = null;
            NebChannelDelay.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebChannelDelay.Location = new Point(10, 326);
            NebChannelDelay.MaxValue = double.MaxValue;
            NebChannelDelay.MinValue = double.MinValue;
            NebChannelDelay.Name = "NebChannelDelay";
            NebChannelDelay.Size = new Size(211, 30);
            NebChannelDelay.StringFormatFunc = null;
            NebChannelDelay.StyleFlags = UserControls.Style.StyleFlag.None;
            NebChannelDelay.StylizeFlag = true;
            NebChannelDelay.SubButtonImg = null;
            buttonBaseStyle10.BackColor = Color.FromArgb(0, 157, 255);
            buttonBaseStyle10.BorderColor = Color.Transparent;
            buttonBaseStyle10.BorderThickness = 0;
            buttonBaseStyle10.ForeColor = Color.White;
            buttonStyle4.MouseClickStyle = buttonBaseStyle10;
            buttonBaseStyle11.BackColor = Color.FromArgb(0, 157, 255);
            buttonBaseStyle11.BorderColor = Color.Green;
            buttonBaseStyle11.BorderThickness = 0;
            buttonBaseStyle11.ForeColor = Color.White;
            buttonStyle4.MouseInStyle = buttonBaseStyle11;
            buttonBaseStyle12.BackColor = Color.FromArgb(0, 157, 255);
            buttonBaseStyle12.BorderColor = Color.Transparent;
            buttonBaseStyle12.BorderThickness = 0;
            buttonBaseStyle12.ForeColor = Color.White;
            buttonStyle4.NomalStyle = buttonBaseStyle12;
            NebChannelDelay.SubButtonStyle = buttonStyle4;
            NebChannelDelay.TabIndex = 25;
            NebChannelDelay.Value = 0D;
            NebChannelDelay.Visible = false;
            NebChannelDelay.Enabled = false;
            // 
            // BtnCopyToOtherChannel
            // 
            BtnCopyToOtherChannel.Adjustable = false;
            BtnCopyToOtherChannel.BackColor = Color.Transparent;
            BtnCopyToOtherChannel.BorderColor = Color.FromArgb(67, 69, 76);
            BtnCopyToOtherChannel.BorderThickness = 1;
            BtnCopyToOtherChannel.ChoosedBackColor = Color.FromArgb(40, 71, 193);
            BtnCopyToOtherChannel.ChoosedForeColor = Color.FromArgb(192, 192, 192);
            BtnCopyToOtherChannel.ChoosedMouseinBackColor = Color.FromArgb(40, 71, 193);
            BtnCopyToOtherChannel.ChoosedPressedBackColor = Color.FromArgb(40, 71, 193);
            BtnCopyToOtherChannel.CornerRadius = 0;
            BtnCopyToOtherChannel.Cursor = Cursors.Hand;
            BtnCopyToOtherChannel.DaskArray = null;
            BtnCopyToOtherChannel.DoubleClickEnable = true;
            BtnCopyToOtherChannel.DropKey = Keys.Space;
            BtnCopyToOtherChannel.FineEnable = true;
            BtnCopyToOtherChannel.FocusedBorderColor = Color.DeepSkyBlue;
            BtnCopyToOtherChannel.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnCopyToOtherChannel.ForeColor = SystemColors.ActiveCaptionText;
            BtnCopyToOtherChannel.Height = 30;
            BtnCopyToOtherChannel.Icon = null;
            BtnCopyToOtherChannel.IconOffset = 10;
            BtnCopyToOtherChannel.IconSize = new Size(24, 24);
            BtnCopyToOtherChannel.ImeMode = ImeMode.NoControl;
            BtnCopyToOtherChannel.IndicatorColor = Color.FromArgb(40, 71, 193);
            BtnCopyToOtherChannel.IsChoosed = false;
            BtnCopyToOtherChannel.IsIndicatorShow = false;
            BtnCopyToOtherChannel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnCopyToOtherChannel.Location = new Point(330, 326);
            BtnCopyToOtherChannel.MouseinBackColor = Color.FromArgb(53, 54, 58);
            BtnCopyToOtherChannel.MouseinBorderColor = Color.FromArgb(53, 54, 58);
            BtnCopyToOtherChannel.MouseInBorderThickness = 0;
            BtnCopyToOtherChannel.MouseinForeColor = Color.FromArgb(185, 192, 199);
            BtnCopyToOtherChannel.MouseinSvgForeColor = Color.FromArgb(0, 157, 255);
            BtnCopyToOtherChannel.Name = "BtnCopyToOtherChannel";
            BtnCopyToOtherChannel.PressedBackColor = Color.Gray;
            BtnCopyToOtherChannel.PressedBorderColor = Color.FromArgb(53, 54, 58);
            BtnCopyToOtherChannel.PressedBorderThickness = 0;
            BtnCopyToOtherChannel.PressedForeColor = Color.FromArgb(185, 192, 199);
            BtnCopyToOtherChannel.PressedSvgForeColor = Color.FromArgb(0, 157, 255);
            BtnCopyToOtherChannel.Size = new Size(80, 30);
            BtnCopyToOtherChannel.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnCopyToOtherChannel.StylizeFlag = true;
            BtnCopyToOtherChannel.SVGForeColor = Color.FromArgb(185, 192, 199);
            BtnCopyToOtherChannel.SVGPath = "";
            BtnCopyToOtherChannel.TabIndex = 26;
            BtnCopyToOtherChannel.Text = "0";
            BtnCopyToOtherChannel.TextAlign = ContentAlignment.MiddleCenter;
            BtnCopyToOtherChannel.Click += BtnCopyToOtherChannel_Click;
            // 
            // ChkOtherChannelSelect
            // 
            ChkOtherChannelSelect.AutoSize = true;
            ChkOtherChannelSelect.BackColor = Color.FromArgb(53, 54, 58);
            ChkOtherChannelSelect.BorderColor = Color.FromArgb(53, 54, 58);
            ChkOtherChannelSelect.ComBorderColor = Color.Blue;
            ChkOtherChannelSelect.DataSource = null;
            ChkOtherChannelSelect.ExtText = "";
            ChkOtherChannelSelect.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point);
            ChkOtherChannelSelect.ForeColor = Color.White;
            ChkOtherChannelSelect.ImeMode = ImeMode.NoControl;
            ChkOtherChannelSelect.Location = new Point(420, 326);
            ChkOtherChannelSelect.MaximumSize = new Size(99999, 99999);
            ChkOtherChannelSelect.Name = "ChkOtherChannelSelect";
            ChkOtherChannelSelect.SelectIndex = -1;
            ChkOtherChannelSelect.SelectValue = null;
            ChkOtherChannelSelect.Size = new Size(110, 30);
            ChkOtherChannelSelect.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            ChkOtherChannelSelect.StylizeFlag = true;
            ChkOtherChannelSelect.TabIndex = 27;
            // 
            // VerticalPage
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(41, 42, 45);
            Controls.Add(NebChannelDelay);
            Controls.Add(ChkIndependentWindow);
            Controls.Add(LblIndependentWindow);
            Controls.Add(NebScale);
            Controls.Add(TbxUnit);
            Controls.Add(ChkLabelVisiblity);
            Controls.Add(CbxCoupling);
            Controls.Add(ChkBandLimit);
            Controls.Add(LblActive);
            Controls.Add(LblInvert);
            Controls.Add(LblbandLimit);
            Controls.Add(TbxLabel);
            Controls.Add(LblCoupling);
            Controls.Add(LblScale);
            Controls.Add(LblLabel);
            Controls.Add(BtnPosition);
            Controls.Add(BtnResetPos);
            Controls.Add(BtnBias);
            Controls.Add(LblPosition);
            Controls.Add(LblChannelDelay);
            Controls.Add(BtnResetBias);
            Controls.Add(LblBias);
            Controls.Add(ChkActive);
            Controls.Add(ChkInvert);
            Controls.Add(LblAmplitudeSelection);
            Controls.Add(ChkAmplitude);
            Controls.Add(BtnCopyToOtherChannel);
            Controls.Add(ChkOtherChannelSelect);
            DoubleBuffered = true;
            Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Name = "VerticalPage";
            Size = new Size(549, 364);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ScopeX.UserControls.ScopeXSwitchButton ChkActive;
        private ScopeX.UserControls.ScopeXSwitchButton ChkInvert;
        private ScopeX.UserControls.ScopeXTextBox TbxUnit;
        private ScopeX.UserControls.ScopeXLabel LblLabelVisiblity;
        private ScopeX.UserControls.SelectComboBox ChkBandLimit;
        private ScopeX.UserControls.ScopeXLabel LblActive;
        private ScopeX.UserControls.ScopeXLabel LblInvert;
        private ScopeX.UserControls.ScopeXLabel LblbandLimit;
        private ScopeX.UserControls.ScopeXTextBox TbxLabel;
        private ScopeX.UserControls.ScopeXLabel LblCoupling;
        private ScopeX.UserControls.ScopeXLabel LblScale;
        private ScopeX.UserControls.ScopeXLabel LblLabel;
        private ScopeX.UserControls.ScopeXIconButton BtnResetPos;
        private ScopeX.UserControls.ScopeXIconButton BtnBias;
        private ScopeX.UserControls.ScopeXLabel LblPosition;
        private ScopeX.UserControls.ScopeXIconButton BtnResetBias;
        private ScopeX.UserControls.ScopeXLabel LblBias;
        private ScopeX.UserControls.TouchNeb NebScale;
        private ScopeX.UserControls.SelectComboBox CbxCoupling;
        private ScopeX.UserControls.ScopeXIconButton BtnPosition;
        private ScopeX.UserControls.ScopeXSwitchButton ChkIndependentWindow;
        private ScopeX.UserControls.ScopeXLabel LblIndependentWindow;
        private UserControls.ScopeXLabel LblChannelDelay;
        private UserControls.TouchNeb NebChannelDelay;
        private ScopeX.UserControls.ScopeXLabel LblAmplitudeSelection;
        private ScopeX.UserControls.ScopeXSwitchButton ChkAmplitude;
        private ScopeX.UserControls.ScopeXSwitchButton ChkLabelVisiblity;
        private ScopeX.UserControls.ScopeXIconButton BtnCopyToOtherChannel;
        private ScopeX.UserControls.SelectComboBox ChkOtherChannelSelect;
    }
}
