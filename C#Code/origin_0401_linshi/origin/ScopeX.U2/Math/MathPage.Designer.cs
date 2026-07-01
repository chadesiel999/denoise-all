
using ScopeX.UserControls;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    partial class MathPage
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
            System.Windows.Forms.Panel PnlVertical;
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle9 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle25 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle26 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle27 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle10 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle28 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle29 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle30 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle11 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle31 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle32 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle33 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle12 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle34 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle35 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle36 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt9 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt10 = new UserControls.DefaultHighlightPrompt();
            System.Windows.Forms.Panel PnlHead;
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt11 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt12 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt13 = new UserControls.DefaultHighlightPrompt();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle13 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle37 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle38 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle39 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle14 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle40 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle41 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle42 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle15 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle43 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle44 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle45 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle16 = new UserControls.ScopeXNumericEditBox.ButtonStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle46 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle47 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle48 = new UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt14 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt15 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt16 = new UserControls.DefaultHighlightPrompt();
            BtnResetVPos = new UserControls.ScopeXIconButton();
            NebVPos = new UserControls.TouchNeb();
            NebVScale = new UserControls.TouchNeb();
            LblVScale = new UserControls.ScopeXLabel();
            LblVPos = new UserControls.ScopeXLabel();
            ChkIndependentWindow = new UserControls.ScopeXSwitchButton();
            LblIndependentWindow = new UserControls.ScopeXLabel();
            ChkActive = new UserControls.ScopeXSwitchButton();
            CbxCalcType = new ScopeX.UserControls.SelectComboBox();
            ChkLabelVisiblity = new ScopeX.UserControls.ScopeXSwitchButton();
            LblCalcType = new UserControls.ScopeXLabel();
            LblActive = new UserControls.ScopeXLabel();
            TlpMath = new System.Windows.Forms.TableLayoutPanel();
            PnlHorizon = new System.Windows.Forms.Panel();
            BtnResetHPos = new UserControls.ScopeXIconButton();
            NebHPos = new UserControls.TouchNeb();
            NebHScale = new UserControls.TouchNeb();
            LblHPos = new UserControls.ScopeXLabel();
            LblHScale = new UserControls.ScopeXLabel();
            PnlTail = new System.Windows.Forms.Panel();
            ChkUnit = new System.Windows.Forms.CheckBox();
            TbxUnit = new UserControls.ScopeXTextBox();
            TbxLabel = new UserControls.ScopeXTextBox();
            LblLabelVisiblity = new UserControls.ScopeXLabel();
            LblLabel = new UserControls.ScopeXLabel();
            PnlVertical = new System.Windows.Forms.Panel();
            PnlHead = new System.Windows.Forms.Panel();
            PnlVertical.SuspendLayout();
            PnlHead.SuspendLayout();
            TlpMath.SuspendLayout();
            PnlHorizon.SuspendLayout();
            PnlTail.SuspendLayout();
            SuspendLayout();
            // 
            // PnlVertical
            // 
            PnlVertical.AutoSize = true;
            PnlVertical.Controls.Add(BtnResetVPos);
            PnlVertical.Controls.Add(NebVPos);
            PnlVertical.Controls.Add(NebVScale);
            PnlVertical.Controls.Add(LblVScale);
            PnlVertical.Controls.Add(LblVPos);
            PnlVertical.Dock = System.Windows.Forms.DockStyle.Fill;
            PnlVertical.Location = new System.Drawing.Point(0, 299);
            PnlVertical.Name = "PnlVertical";
            PnlVertical.Padding = new System.Windows.Forms.Padding(3);
            PnlVertical.Size = new System.Drawing.Size(480, 60);
            PnlVertical.TabIndex = 2;
            // 
            // BtnResetVPos
            // 
            BtnResetVPos.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetVPos.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetVPos.BorderThickness = 0;
            BtnResetVPos.CornerRadius = 0;
            BtnResetVPos.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnResetVPos.DaskArray = null;
            BtnResetVPos.DropKey = System.Windows.Forms.Keys.Space;
            BtnResetVPos.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnResetVPos.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetVPos.Height = 26;
            BtnResetVPos.Icon = null;
            BtnResetVPos.IconOffset = 10;
            BtnResetVPos.IconSize = new System.Drawing.Size(24, 24);
            BtnResetVPos.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnResetVPos.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnResetVPos.IsIndicatorShow = false;
            BtnResetVPos.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnResetVPos.Location = new System.Drawing.Point(464, 30);
            BtnResetVPos.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetVPos.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetVPos.MouseInBorderThickness = 0;
            BtnResetVPos.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetVPos.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnResetVPos.Name = "BtnResetVPos";
            BtnResetVPos.PressedBackColor = System.Drawing.Color.Gray;
            BtnResetVPos.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetVPos.PressedBorderThickness = 0;
            BtnResetVPos.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetVPos.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnResetVPos.Size = new System.Drawing.Size(55, 26);
            BtnResetVPos.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnResetVPos.StylizeFlag = true;
            BtnResetVPos.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetVPos.SVGPath = "";
            BtnResetVPos.TabIndex = 24;
            BtnResetVPos.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.PnlVertical.BtnResetVPos"); // "复位";
            BtnResetVPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnResetVPos.Click += BtnResetVPos_Click;
            // 
            // NebVPos
            // 
            NebVPos.AddButtonImg = null;
            buttonBaseStyle25.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle25.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle25.BorderThickness = 0;
            buttonBaseStyle25.ForeColor = System.Drawing.Color.White;
            buttonStyle9.MouseClickStyle = buttonBaseStyle25;
            buttonBaseStyle26.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle26.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle26.BorderThickness = 0;
            buttonBaseStyle26.ForeColor = System.Drawing.Color.White;
            buttonStyle9.MouseInStyle = buttonBaseStyle26;
            buttonBaseStyle27.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle27.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle27.BorderThickness = 0;
            buttonBaseStyle27.ForeColor = System.Drawing.Color.White;
            buttonStyle9.NomalStyle = buttonBaseStyle27;
            NebVPos.AddButtonStyle = buttonStyle9;
            NebVPos.AllwaysShowFocusImage = false;
            NebVPos.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebVPos.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebVPos.BorderThickness = 0;
            NebVPos.DisableHoldOnInput = false;
            NebVPos.DropKey = System.Windows.Forms.Keys.Space;
            NebVPos.FocusBoederColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebVPos.FocusForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            NebVPos.FocusImage = null;
            NebVPos.FocusImagePosition = UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebVPos.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            NebVPos.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            NebVPos.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            NebVPos.Height = 26;
            NebVPos.HoldOnSpeedLevel = 10;
            NebVPos.IconWidthProportion = 1F;
            NebVPos.Interval = 0.1D;
            NebVPos.LanguageKey = null;
            NebVPos.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebVPos.Location = new System.Drawing.Point(234, 30);
            NebVPos.Name = "NebVPos";
            NebVPos.Size = new System.Drawing.Size(220, 26);
            NebVPos.StringFormatFunc = null;
            NebVPos.StyleFlags = UserControls.Style.StyleFlag.None;
            NebVPos.StylizeFlag = true;
            NebVPos.SubButtonImg = null;
            buttonBaseStyle28.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle28.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle28.BorderThickness = 0;
            buttonBaseStyle28.ForeColor = System.Drawing.Color.White;
            buttonStyle10.MouseClickStyle = buttonBaseStyle28;
            buttonBaseStyle29.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle29.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle29.BorderThickness = 0;
            buttonBaseStyle29.ForeColor = System.Drawing.Color.White;
            buttonStyle10.MouseInStyle = buttonBaseStyle29;
            buttonBaseStyle30.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle30.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle30.BorderThickness = 0;
            buttonBaseStyle30.ForeColor = System.Drawing.Color.White;
            buttonStyle10.NomalStyle = buttonBaseStyle30;
            NebVPos.SubButtonStyle = buttonStyle10;
            NebVPos.TabIndex = 23;
            NebVPos.Value = 0D;
            // 
            // NebVScale
            // 
            NebVScale.AddButtonImg = null;
            buttonBaseStyle31.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle31.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle31.BorderThickness = 0;
            buttonBaseStyle31.ForeColor = System.Drawing.Color.White;
            buttonStyle11.MouseClickStyle = buttonBaseStyle31;
            buttonBaseStyle32.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle32.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle32.BorderThickness = 0;
            buttonBaseStyle32.ForeColor = System.Drawing.Color.White;
            buttonStyle11.MouseInStyle = buttonBaseStyle32;
            buttonBaseStyle33.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle33.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle33.BorderThickness = 0;
            buttonBaseStyle33.ForeColor = System.Drawing.Color.White;
            buttonStyle11.NomalStyle = buttonBaseStyle33;
            NebVScale.AddButtonStyle = buttonStyle11;
            NebVScale.AllwaysShowFocusImage = false;
            NebVScale.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebVScale.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebVScale.BorderThickness = 0;
            NebVScale.DisableHoldOnInput = false;
            NebVScale.DropKey = System.Windows.Forms.Keys.Space;
            NebVScale.FocusBoederColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebVScale.FocusForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            NebVScale.FocusImage = null;
            NebVScale.FocusImagePosition = UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebVScale.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            NebVScale.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            NebVScale.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            NebVScale.Height = 26;
            NebVScale.HoldOnSpeedLevel = 10;
            NebVScale.IconWidthProportion = 1F;
            NebVScale.Interval = 0.1D;
            NebVScale.LanguageKey = null;
            NebVScale.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebVScale.Location = new System.Drawing.Point(10, 30);
            NebVScale.Name = "NebVScale";
            NebVScale.Size = new System.Drawing.Size(210, 26);
            NebVScale.StringFormatFunc = null;
            NebVScale.StyleFlags = UserControls.Style.StyleFlag.None;
            NebVScale.StylizeFlag = true;
            NebVScale.SubButtonImg = null;
            buttonBaseStyle34.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle34.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle34.BorderThickness = 0;
            buttonBaseStyle34.ForeColor = System.Drawing.Color.White;
            buttonStyle12.MouseClickStyle = buttonBaseStyle34;
            buttonBaseStyle35.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle35.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle35.BorderThickness = 0;
            buttonBaseStyle35.ForeColor = System.Drawing.Color.White;
            buttonStyle12.MouseInStyle = buttonBaseStyle35;
            buttonBaseStyle36.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle36.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle36.BorderThickness = 0;
            buttonBaseStyle36.ForeColor = System.Drawing.Color.White;
            buttonStyle12.NomalStyle = buttonBaseStyle36;
            NebVScale.SubButtonStyle = buttonStyle12;
            NebVScale.TabIndex = 21;
            NebVScale.Value = 0D;
            // 
            // LblVScale
            // 
            LblVScale.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblVScale.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblVScale.BorderThickness = 0;
            LblVScale.CornerRadius = 0;
            LblVScale.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblVScale.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblVScale.HighlightPrompt = defaultHighlightPrompt9;
            LblVScale.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblVScale.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblVScale.Location = new System.Drawing.Point(10, 2);
            LblVScale.MultyLineFlag = false;
            LblVScale.Name = "LblVScale";
            LblVScale.Size = new System.Drawing.Size(170, 19);
            LblVScale.StyleFlags = UserControls.Style.StyleFlag.None;
            LblVScale.StylizeFlag = true;
            LblVScale.TabIndex = 20;
            LblVScale.TabStop = false;
            LblVScale.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.PnlVertical.LblVScale"); // "垂直刻度";
            LblVScale.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblVScale.Token = null;
            // 
            // LblVPos
            // 
            LblVPos.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblVPos.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblVPos.BorderThickness = 0;
            LblVPos.CornerRadius = 0;
            LblVPos.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblVPos.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblVPos.HighlightPrompt = defaultHighlightPrompt10;
            LblVPos.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblVPos.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblVPos.Location = new System.Drawing.Point(234, 2);
            LblVPos.MultyLineFlag = false;
            LblVPos.Name = "LblVPos";
            LblVPos.Size = new System.Drawing.Size(170, 19);
            LblVPos.StyleFlags = UserControls.Style.StyleFlag.None;
            LblVPos.StylizeFlag = true;
            LblVPos.TabIndex = 22;
            LblVPos.TabStop = false;
            LblVPos.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.PnlVertical.LblVPos"); // "垂直位置";
            LblVPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblVPos.Token = null;
            // 
            // PnlHead
            // 
            PnlHead.AutoSize = true;
            PnlHead.Controls.Add(ChkIndependentWindow);
            PnlHead.Controls.Add(LblIndependentWindow);
            PnlHead.Controls.Add(ChkActive);
            PnlHead.Controls.Add(CbxCalcType);
            PnlHead.Controls.Add(LblCalcType);
            PnlHead.Controls.Add(LblActive);
            PnlHead.Dock = System.Windows.Forms.DockStyle.Fill;
            PnlHead.Location = new System.Drawing.Point(6, 6);
            PnlHead.Name = "PnlHead";
            PnlHead.Padding = new System.Windows.Forms.Padding(3);
            PnlHead.Size = new System.Drawing.Size(480, 59);
            PnlHead.TabIndex = 0;
            // 
            // ChkIndependentWindow
            // 
            ChkIndependentWindow.AnimationCount = 8;
            ChkIndependentWindow.AnimationFunc = null;
            ChkIndependentWindow.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkIndependentWindow.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkIndependentWindow.BorderThickness = 1;
            ChkIndependentWindow.Checked = false;
            ChkIndependentWindow.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkIndependentWindow.CheckedForeColor = System.Drawing.Color.Black;
            ChkIndependentWindow.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkIndependentWindow.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.PnlHead.ChkIndependentWindow", "CheckedText"); // "开";
            ChkIndependentWindow.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkIndependentWindow.DropKey = System.Windows.Forms.Keys.Space;
            ChkIndependentWindow.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkIndependentWindow.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkIndependentWindow.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkIndependentWindow.Height = 26;
            ChkIndependentWindow.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkIndependentWindow.Location = new System.Drawing.Point(155, 30);
            ChkIndependentWindow.Margin = new System.Windows.Forms.Padding(0);
            ChkIndependentWindow.Name = "ChkIndependentWindow";
            ChkIndependentWindow.Size = new System.Drawing.Size(75, 26);
            ChkIndependentWindow.SliderButtonWidth = 32;
            ChkIndependentWindow.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkIndependentWindow.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkIndependentWindow.StylizeFlag = true;
            ChkIndependentWindow.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkIndependentWindow.TabIndex = 5;
            ChkIndependentWindow.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.PnlHead.ChkIndependentWindow"); // "关";
            ChkIndependentWindow.UseAnimation = true;
            ChkIndependentWindow.Visible = false;
            ChkIndependentWindow.CheckedChangedEvent += ChkIndependentWindow_CheckedChangedEvent;
            // 
            // LblIndependentWindow
            // 
            LblIndependentWindow.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblIndependentWindow.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblIndependentWindow.BorderThickness = 0;
            LblIndependentWindow.CornerRadius = 0;
            LblIndependentWindow.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblIndependentWindow.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblIndependentWindow.HighlightPrompt = defaultHighlightPrompt11;
            LblIndependentWindow.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblIndependentWindow.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblIndependentWindow.Location = new System.Drawing.Point(155, 2);
            LblIndependentWindow.MultyLineFlag = false;
            LblIndependentWindow.Name = "LblIndependentWindow";
            LblIndependentWindow.Size = new System.Drawing.Size(90, 19);
            LblIndependentWindow.StyleFlags = UserControls.Style.StyleFlag.None;
            LblIndependentWindow.StylizeFlag = true;
            LblIndependentWindow.TabIndex = 4;
            LblIndependentWindow.TabStop = false;
            LblIndependentWindow.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.PnlHead.LblIndependentWindow"); // "独立窗口";
            LblIndependentWindow.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblIndependentWindow.Token = null;
            LblIndependentWindow.Visible = false;
            // 
            // ChkActive
            // 
            ChkActive.AnimationCount = 8;
            ChkActive.AnimationFunc = null;
            ChkActive.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkActive.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkActive.BorderThickness = 1;
            ChkActive.Checked = true;
            ChkActive.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkActive.CheckedForeColor = System.Drawing.Color.Black;
            ChkActive.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkActive.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.PnlHead.ChkActive", "CheckedText"); // "开";
            ChkActive.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkActive.DropKey = System.Windows.Forms.Keys.Space;
            ChkActive.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkActive.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkActive.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkActive.Height = 26;
            ChkActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkActive.Location = new System.Drawing.Point(10, 30);
            ChkActive.Margin = new System.Windows.Forms.Padding(0);
            ChkActive.Name = "ChkActive";
            ChkActive.Size = new System.Drawing.Size(75, 26);
            ChkActive.SliderButtonWidth = 32;
            ChkActive.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkActive.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkActive.StylizeFlag = true;
            ChkActive.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkActive.TabIndex = 1;
            ChkActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.PnlHead.ChkActive"); // "关";
            ChkActive.UseAnimation = true;
            ChkActive.CheckedChangedEvent += ChkActive_CheckedChangedEvent;
            // 
            // CbxCalcType
            // 
            CbxCalcType.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxCalcType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxCalcType.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxCalcType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxCalcType.ForeColor = System.Drawing.Color.White;
            CbxCalcType.Location = new System.Drawing.Point(351, 30);
            CbxCalcType.Name = "CbxCalcType";
            CbxCalcType.Size = new System.Drawing.Size(170, 30);
            CbxCalcType.TabIndex = 6;
            // 
            // LblCalcType
            // 
            LblCalcType.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblCalcType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblCalcType.BorderThickness = 0;
            LblCalcType.CornerRadius = 0;
            LblCalcType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblCalcType.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblCalcType.HighlightPrompt = defaultHighlightPrompt12;
            LblCalcType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblCalcType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblCalcType.Location = new System.Drawing.Point(351, 2);
            LblCalcType.MultyLineFlag = false;
            LblCalcType.Name = "LblCalcType";
            LblCalcType.Size = new System.Drawing.Size(170, 19);
            LblCalcType.StyleFlags = UserControls.Style.StyleFlag.None;
            LblCalcType.StylizeFlag = true;
            LblCalcType.TabIndex = 2;
            LblCalcType.TabStop = false;
            LblCalcType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.PnlHead.LblCalcType"); // "运算类型";
            LblCalcType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblCalcType.Token = null;
            // 
            // LblActive
            // 
            LblActive.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblActive.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblActive.BorderThickness = 0;
            LblActive.CornerRadius = 0;
            LblActive.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblActive.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblActive.HighlightPrompt = defaultHighlightPrompt13;
            LblActive.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblActive.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblActive.Location = new System.Drawing.Point(10, 2);
            LblActive.MultyLineFlag = false;
            LblActive.Name = "LblActive";
            LblActive.Size = new System.Drawing.Size(75, 19);
            LblActive.StyleFlags = UserControls.Style.StyleFlag.None;
            LblActive.StylizeFlag = true;
            LblActive.TabIndex = 0;
            LblActive.TabStop = false;
            LblActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.PnlHead.LblActive"); // "显示";
            LblActive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblActive.Token = null;
            // 
            // TlpMath
            // 
            TlpMath.ColumnCount = 1;
            TlpMath.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpMath.Controls.Add(PnlVertical, 0, 2);
            TlpMath.Controls.Add(PnlHead, 0, 0);
            TlpMath.Controls.Add(PnlHorizon, 0, 3);
            TlpMath.Controls.Add(PnlTail, 0, 4);
            TlpMath.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpMath.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TlpMath.Location = new System.Drawing.Point(0, 0);
            TlpMath.Name = "TlpMath";
            TlpMath.Padding = new System.Windows.Forms.Padding(3);
            TlpMath.RowCount = 5;
            TlpMath.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpMath.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpMath.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpMath.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpMath.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpMath.Size = new System.Drawing.Size(492, 492);
            TlpMath.TabIndex = 0;
            // 
            // PnlHorizon
            // 
            PnlHorizon.AutoSize = true;
            PnlHorizon.Controls.Add(BtnResetHPos);
            PnlHorizon.Controls.Add(NebHPos);
            PnlHorizon.Controls.Add(NebHScale);
            PnlHorizon.Controls.Add(LblHPos);
            PnlHorizon.Controls.Add(LblHScale);
            PnlHorizon.Dock = System.Windows.Forms.DockStyle.Fill;
            PnlHorizon.Location = new System.Drawing.Point(6, 365);
            PnlHorizon.Name = "PnlHorizon";
            PnlHorizon.Size = new System.Drawing.Size(480, 57);
            PnlHorizon.TabIndex = 3;
            // 
            // BtnResetHPos
            // 
            BtnResetHPos.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetHPos.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetHPos.BorderThickness = 0;
            BtnResetHPos.CornerRadius = 0;
            BtnResetHPos.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnResetHPos.DaskArray = null;
            BtnResetHPos.DropKey = System.Windows.Forms.Keys.Space;
            BtnResetHPos.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnResetHPos.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetHPos.Height = 26;
            BtnResetHPos.Icon = null;
            BtnResetHPos.IconOffset = 10;
            BtnResetHPos.IconSize = new System.Drawing.Size(24, 24);
            BtnResetHPos.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnResetHPos.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnResetHPos.IsIndicatorShow = false;
            BtnResetHPos.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnResetHPos.Location = new System.Drawing.Point(464, 30);
            BtnResetHPos.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetHPos.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetHPos.MouseInBorderThickness = 0;
            BtnResetHPos.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetHPos.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnResetHPos.Name = "BtnResetHPos";
            BtnResetHPos.PressedBackColor = System.Drawing.Color.Gray;
            BtnResetHPos.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnResetHPos.PressedBorderThickness = 0;
            BtnResetHPos.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetHPos.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnResetHPos.Size = new System.Drawing.Size(55, 26);
            BtnResetHPos.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnResetHPos.StylizeFlag = true;
            BtnResetHPos.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnResetHPos.SVGPath = "";
            BtnResetHPos.TabIndex = 34;
            BtnResetHPos.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.PnlHorizon.BtnResetHPos"); // "复位";
            BtnResetHPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnResetHPos.Click += BtnResetHPos_Click;
            // 
            // NebHPos
            // 
            NebHPos.AddButtonImg = null;
            buttonBaseStyle37.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle37.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle37.BorderThickness = 0;
            buttonBaseStyle37.ForeColor = System.Drawing.Color.White;
            buttonStyle13.MouseClickStyle = buttonBaseStyle37;
            buttonBaseStyle38.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle38.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle38.BorderThickness = 0;
            buttonBaseStyle38.ForeColor = System.Drawing.Color.White;
            buttonStyle13.MouseInStyle = buttonBaseStyle38;
            buttonBaseStyle39.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle39.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle39.BorderThickness = 0;
            buttonBaseStyle39.ForeColor = System.Drawing.Color.White;
            buttonStyle13.NomalStyle = buttonBaseStyle39;
            NebHPos.AddButtonStyle = buttonStyle13;
            NebHPos.AllwaysShowFocusImage = false;
            NebHPos.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebHPos.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebHPos.BorderThickness = 0;
            NebHPos.DisableHoldOnInput = false;
            NebHPos.DropKey = System.Windows.Forms.Keys.Space;
            NebHPos.FocusBoederColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebHPos.FocusForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            NebHPos.FocusImage = null;
            NebHPos.FocusImagePosition = UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebHPos.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            NebHPos.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            NebHPos.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            NebHPos.Height = 26;
            NebHPos.HoldOnSpeedLevel = 10;
            NebHPos.IconWidthProportion = 1F;
            NebHPos.Interval = 0.1D;
            NebHPos.LanguageKey = null;
            NebHPos.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebHPos.Location = new System.Drawing.Point(234, 30);
            NebHPos.Name = "NebHPos";
            NebHPos.Size = new System.Drawing.Size(220, 26);
            NebHPos.StringFormatFunc = null;
            NebHPos.StyleFlags = UserControls.Style.StyleFlag.None;
            NebHPos.StylizeFlag = true;
            NebHPos.SubButtonImg = null;
            buttonBaseStyle40.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle40.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle40.BorderThickness = 0;
            buttonBaseStyle40.ForeColor = System.Drawing.Color.White;
            buttonStyle14.MouseClickStyle = buttonBaseStyle40;
            buttonBaseStyle41.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle41.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle41.BorderThickness = 0;
            buttonBaseStyle41.ForeColor = System.Drawing.Color.White;
            buttonStyle14.MouseInStyle = buttonBaseStyle41;
            buttonBaseStyle42.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle42.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle42.BorderThickness = 0;
            buttonBaseStyle42.ForeColor = System.Drawing.Color.White;
            buttonStyle14.NomalStyle = buttonBaseStyle42;
            NebHPos.SubButtonStyle = buttonStyle14;
            NebHPos.TabIndex = 33;
            NebHPos.Value = 0D;
            // 
            // NebHScale
            // 
            NebHScale.AddButtonImg = null;
            buttonBaseStyle43.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle43.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle43.BorderThickness = 0;
            buttonBaseStyle43.ForeColor = System.Drawing.Color.White;
            buttonStyle15.MouseClickStyle = buttonBaseStyle43;
            buttonBaseStyle44.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle44.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle44.BorderThickness = 0;
            buttonBaseStyle44.ForeColor = System.Drawing.Color.White;
            buttonStyle15.MouseInStyle = buttonBaseStyle44;
            buttonBaseStyle45.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle45.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle45.BorderThickness = 0;
            buttonBaseStyle45.ForeColor = System.Drawing.Color.White;
            buttonStyle15.NomalStyle = buttonBaseStyle45;
            NebHScale.AddButtonStyle = buttonStyle15;
            NebHScale.AllwaysShowFocusImage = false;
            NebHScale.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebHScale.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebHScale.BorderThickness = 0;
            NebHScale.DisableHoldOnInput = false;
            NebHScale.DropKey = System.Windows.Forms.Keys.Space;
            NebHScale.FocusBoederColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NebHScale.FocusForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            NebHScale.FocusImage = null;
            NebHScale.FocusImagePosition = UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            NebHScale.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            NebHScale.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            NebHScale.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            NebHScale.Height = 26;
            NebHScale.HoldOnSpeedLevel = 10;
            NebHScale.IconWidthProportion = 1F;
            NebHScale.Interval = 0.1D;
            NebHScale.LanguageKey = null;
            NebHScale.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            NebHScale.Location = new System.Drawing.Point(9, 30);
            NebHScale.Name = "NebHScale";
            NebHScale.Size = new System.Drawing.Size(210, 26);
            NebHScale.StringFormatFunc = null;
            NebHScale.StyleFlags = UserControls.Style.StyleFlag.None;
            NebHScale.StylizeFlag = true;
            NebHScale.SubButtonImg = null;
            buttonBaseStyle46.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle46.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle46.BorderThickness = 0;
            buttonBaseStyle46.ForeColor = System.Drawing.Color.White;
            buttonStyle16.MouseClickStyle = buttonBaseStyle46;
            buttonBaseStyle47.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle47.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle47.BorderThickness = 0;
            buttonBaseStyle47.ForeColor = System.Drawing.Color.White;
            buttonStyle16.MouseInStyle = buttonBaseStyle47;
            buttonBaseStyle48.BackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            buttonBaseStyle48.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle48.BorderThickness = 0;
            buttonBaseStyle48.ForeColor = System.Drawing.Color.White;
            buttonStyle16.NomalStyle = buttonBaseStyle48;
            NebHScale.SubButtonStyle = buttonStyle16;
            NebHScale.TabIndex = 31;
            NebHScale.Value = 0D;
            // 
            // LblHPos
            // 
            LblHPos.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblHPos.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblHPos.BorderThickness = 0;
            LblHPos.CornerRadius = 0;
            LblHPos.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblHPos.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblHPos.HighlightPrompt = defaultHighlightPrompt14;
            LblHPos.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblHPos.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblHPos.Location = new System.Drawing.Point(234, 2);
            LblHPos.MultyLineFlag = false;
            LblHPos.Name = "LblHPos";
            LblHPos.Size = new System.Drawing.Size(170, 19);
            LblHPos.StyleFlags = UserControls.Style.StyleFlag.None;
            LblHPos.StylizeFlag = true;
            LblHPos.TabIndex = 32;
            LblHPos.TabStop = false;
            LblHPos.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.PnlHorizon.LblHPos"); // "水平位置";
            LblHPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblHPos.Token = null;
            // 
            // LblHScale
            // 
            LblHScale.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblHScale.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblHScale.BorderThickness = 0;
            LblHScale.CornerRadius = 0;
            LblHScale.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblHScale.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblHScale.HighlightPrompt = defaultHighlightPrompt15;
            LblHScale.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblHScale.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblHScale.Location = new System.Drawing.Point(9, 2);
            LblHScale.MultyLineFlag = false;
            LblHScale.Name = "LblHScale";
            LblHScale.Size = new System.Drawing.Size(170, 19);
            LblHScale.StyleFlags = UserControls.Style.StyleFlag.None;
            LblHScale.StylizeFlag = true;
            LblHScale.TabIndex = 30;
            LblHScale.TabStop = false;
            LblHScale.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.PnlHorizon.LblHScale"); // "水平刻度";
            LblHScale.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblHScale.Token = null;
            // 
            // PnlTail
            // 
            PnlTail.AutoSize = true;
            PnlTail.Controls.Add(ChkUnit);
            PnlTail.Controls.Add(TbxUnit);
            PnlTail.Controls.Add(TbxLabel);
            PnlTail.Controls.Add(LblLabel);
            //PnlTail.Controls.Add(LblLabelVisiblity);
            PnlTail.Controls.Add(ChkLabelVisiblity);
            PnlTail.Dock = System.Windows.Forms.DockStyle.Fill;
            PnlTail.Location = new System.Drawing.Point(0, 428);
            PnlTail.Name = "PnlTail";
            PnlTail.Size = new System.Drawing.Size(480, 58);
            PnlTail.TabIndex = 4;
            // 
            // ChkUnit
            // 
            ChkUnit.AutoSize = true;
            ChkUnit.Font = new System.Drawing.Font("MiSans", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ChkUnit.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkUnit.Location = new System.Drawing.Point(331, 4);
            ChkUnit.Name = "ChkUnit";
            ChkUnit.Size = new System.Drawing.Size(86, 19);
            ChkUnit.TabIndex = 36;
            ChkUnit.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.PnlTail.ChkUnit"); // "自定义单位";
            ChkUnit.UseVisualStyleBackColor = true;
            ChkUnit.CheckedChanged += CbxUnit_Click;
            // 
            // TbxUnit
            // 
            TbxUnit.AcceptsTab = false;
            TbxUnit.AutoShowKeyBoard = true;
            TbxUnit.AutoSize = false;
            TbxUnit.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxUnit.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxUnit.BorderThickness = 0;
            TbxUnit.CornerRadius = 0;
            TbxUnit.Cursor = System.Windows.Forms.Cursors.IBeam;
            TbxUnit.Enabled = false;
            TbxUnit.EnbleSelectBorder = true;
            TbxUnit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbxUnit.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            TbxUnit.Height = 23;
            TbxUnit.HideSelection = true;
            TbxUnit.KeyboardVerify = null;
            TbxUnit.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TbxUnit.Location = new System.Drawing.Point(331, 32);
            TbxUnit.MaxLength = 32767;
            TbxUnit.Modified = false;
            TbxUnit.MouseEnterState = false;
            TbxUnit.Multiline = false;
            TbxUnit.Name = "TbxUnit";
            TbxUnit.ProcessCmdKeyFunc = null;
            TbxUnit.ReadOnly = false;
            TbxUnit.SelectedColor = System.Drawing.Color.FromArgb(0, 157, 255);
            TbxUnit.SelectedText = "";
            TbxUnit.SelectionLength = 0;
            TbxUnit.SelectionStart = 0;
            TbxUnit.ShortcutsEnabled = true;
            TbxUnit.Size = new System.Drawing.Size(170, 23);
            TbxUnit.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxUnit.StylizeFlag = true;
            TbxUnit.TabIndex = 37;
            TbxUnit.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            TbxUnit.UseSystemPasswordChar = false;
            TbxUnit.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            TbxUnit.WordWrap = true;
            TbxUnit.TextChanged += TbxUnit_TextChanged;
            // 
            // TbxLabel
            // 
            TbxLabel.AcceptsTab = false;
            TbxLabel.AutoShowKeyBoard = true;
            TbxLabel.AutoSize = false;
            TbxLabel.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxLabel.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxLabel.BorderThickness = 0;
            TbxLabel.CornerRadius = 0;
            TbxLabel.Cursor = System.Windows.Forms.Cursors.IBeam;
            TbxLabel.Enabled = true;
            TbxLabel.EnbleSelectBorder = true;
            TbxLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbxLabel.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            TbxLabel.Height = 23;
            TbxLabel.HideSelection = true;
            TbxLabel.KeyboardVerify = null;
            TbxLabel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TbxLabel.Location = new System.Drawing.Point(9, 32);
            TbxLabel.MaxLength = 32767;
            TbxLabel.Modified = false;
            TbxLabel.MouseEnterState = false;
            TbxLabel.Multiline = false;
            TbxLabel.Name = "TbxLabel";
            TbxLabel.ProcessCmdKeyFunc = null;
            TbxLabel.ReadOnly = false;
            TbxLabel.SelectedColor = System.Drawing.Color.FromArgb(0, 157, 255);
            TbxLabel.SelectedText = "";
            TbxLabel.SelectionLength = 0;
            TbxLabel.SelectionStart = 0;
            TbxLabel.ShortcutsEnabled = true;
            TbxLabel.Size = new System.Drawing.Size(175, 23);
            TbxLabel.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxLabel.StylizeFlag = true;
            TbxLabel.TabIndex = 35;
            TbxLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            TbxLabel.UseSystemPasswordChar = false;
            TbxLabel.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            TbxLabel.WordWrap = true;
            TbxLabel.TextChanged += TbxLabel_TextChanged;
            // 
            // LblLabel
            // 
            LblLabel.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblLabel.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblLabel.BorderThickness = 0;
            LblLabel.CornerRadius = 0;
            LblLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblLabel.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblLabel.HighlightPrompt = defaultHighlightPrompt16;
            LblLabel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblLabel.Location = new System.Drawing.Point(10, 4);
            LblLabel.MultyLineFlag = false;
            LblLabel.Name = "LblLabel";
            LblLabel.Size = new System.Drawing.Size(175, 19);
            LblLabel.StyleFlags = UserControls.Style.StyleFlag.None;
            LblLabel.StylizeFlag = true;
            LblLabel.TabIndex = 34;
            LblLabel.TabStop = false;
            LblLabel.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.PnlTail.LblLabel"); // "标签";
            LblLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblLabel.Token = null;
            // 
            // LblLabelVisiblity
            // 
            LblLabelVisiblity.BackColor = Color.FromArgb(41, 42, 45);
            LblLabelVisiblity.BorderColor = Color.FromArgb(53, 54, 58);
            LblLabelVisiblity.BorderThickness = 0;
            LblLabelVisiblity.CornerRadius = 0;
            LblLabelVisiblity.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LabelVisiblity"); // "打开标签";
            LblLabelVisiblity.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LblLabelVisiblity.ForeColor = Color.FromArgb(232, 234, 237);
            LblLabelVisiblity.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblLabelVisiblity.Location = new Point(195, 4);
            LblLabelVisiblity.MultyLineFlag = false;
            LblLabelVisiblity.Name = "LblUnitSelection";
            LblLabelVisiblity.Size = new Size(120, 20);
            LblLabelVisiblity.StyleFlags = UserControls.Style.StyleFlag.None;
            LblLabelVisiblity.StylizeFlag = true;
            LblLabelVisiblity.TabIndex = 15;
            LblLabelVisiblity.TabStop = false;
            LblLabelVisiblity.TextAlign = ContentAlignment.MiddleLeft;
            LblLabelVisiblity.Token = null;
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
            ChkLabelVisiblity.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkLabelVisiblity.Cursor = Cursors.Hand;
            ChkLabelVisiblity.DropKey = Keys.Space;
            ChkLabelVisiblity.FocusBorderColor = Color.FromArgb(53, 54, 58);
            ChkLabelVisiblity.ForeColor = Color.FromArgb(185, 192, 199);
            ChkLabelVisiblity.Height = 30;
            ChkLabelVisiblity.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkLabelVisiblity.Location = new Point(195, 32);
            ChkLabelVisiblity.Margin = new Padding(0);
            ChkLabelVisiblity.Name = "ChkAmplitude";
            ChkLabelVisiblity.Size = new Size(75, 30);
            ChkLabelVisiblity.SliderButtonWidth = 30;
            ChkLabelVisiblity.SliderColor = Color.FromArgb(232, 234, 237);
            ChkLabelVisiblity.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkLabelVisiblity.StylizeFlag = true;
            ChkLabelVisiblity.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkLabelVisiblity.TabIndex = 18;
            ChkLabelVisiblity.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan"); //  "关";
            ChkLabelVisiblity.UseAnimation = true;
            ChkLabelVisiblity.CheckedChangedEvent += ChkLabelVisiblity_CheckedChangedEvent;
            // 
            // MathPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(TlpMath);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "MathPage";
            Size = new System.Drawing.Size(539, 520);
            PnlVertical.ResumeLayout(false);
            PnlHead.ResumeLayout(false);
            TlpMath.ResumeLayout(false);
            TlpMath.PerformLayout();
            PnlHorizon.ResumeLayout(false);
            PnlTail.ResumeLayout(false);
            PnlTail.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpMath;
        private System.Windows.Forms.Panel PnlTail;
        private ScopeX.UserControls.SelectComboBox CbxCalcType;
        private ScopeX.UserControls.ScopeXLabel LblCalcType;
        private ScopeX.UserControls.ScopeXLabel LblActive;
        private ScopeX.UserControls.ScopeXLabel LblHPos;
        private ScopeX.UserControls.ScopeXLabel LblHScale;
        private ScopeX.UserControls.ScopeXLabel LblVPos;
        private ScopeX.UserControls.ScopeXTextBox TbxLabel;
        private ScopeX.UserControls.ScopeXLabel LblLabelVisiblity;
        private ScopeX.UserControls.ScopeXTextBox TbxUnit;
        private System.Windows.Forms.CheckBox ChkUnit;
        private ScopeX.UserControls.ScopeXSwitchButton ChkActive;
        private ScopeX.UserControls.TouchNeb NebVScale;
        private ScopeX.UserControls.TouchNeb NebVPos;
        private ScopeX.UserControls.TouchNeb NebHPos;
        private ScopeX.UserControls.TouchNeb NebHScale;
        private ScopeX.UserControls.ScopeXLabel LblVScale;
        private ScopeX.UserControls.ScopeXLabel LblLabel;
        private ScopeX.UserControls.ScopeXIconButton BtnResetVPos;
        private ScopeX.UserControls.ScopeXIconButton BtnResetHPos;
        private ScopeX.UserControls.ScopeXSwitchButton ChkIndependentWindow;
        private ScopeX.UserControls.ScopeXSwitchButton ChkLabelVisiblity;
        private ScopeX.UserControls.ScopeXLabel LblIndependentWindow;
        private System.Windows.Forms.Panel PnlHorizon;
    }
}
