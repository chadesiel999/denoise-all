namespace ScopeX.U2
{
    partial class CustomFormulaForm
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomFormulaForm));
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            BtnBackSpace = new UserControls.ScopeXIconButton();
            BtnAccept = new UserControls.ScopeXIconButton();
            BtnCancel = new UserControls.ScopeXIconButton();
            BtnClear = new UserControls.ScopeXIconButton();
            ImgSelection = new System.Windows.Forms.ImageList(components);
            LvFunctions = new UserControls.ScrollPanel();
            ScFunctions = new UserControls.ScopeXScrollContainer();
            RtbxEditor = new System.Windows.Forms.RichTextBox();
            TlpEditor = new System.Windows.Forms.TableLayoutPanel();
            LblDescription = new UserControls.ScopeXLabel();
            RdoSource = new UserControls.UIRadioButtonGroup();
            BtnLoadFormula = new UserControls.ScopeXIconButton();
            BtnSaveFormula = new UserControls.ScopeXIconButton();
            ScSource = new UserControls.ScopeXScrollContainer();
            LvSources = new UserControls.ScrollPanel();
            ScNumberics = new UserControls.ScopeXScrollContainer();
            LvNumberics = new UserControls.ScrollPanel();
            ToolTip = new System.Windows.Forms.ToolTip(components);
            ScFunctions.SuspendLayout();
            TlpEditor.SuspendLayout();
            ScSource.SuspendLayout();
            ScNumberics.SuspendLayout();
            SuspendLayout();
            // 
            // BtnBackSpace
            // 
            BtnBackSpace.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnBackSpace.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnBackSpace.BorderThickness = 0;
            BtnBackSpace.CornerRadius = 0;
            BtnBackSpace.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnBackSpace.DaskArray = null;
            BtnBackSpace.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnBackSpace.DropKey = System.Windows.Forms.Keys.Space;
            BtnBackSpace.Font = new System.Drawing.Font("MiSans", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnBackSpace.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnBackSpace.Height = 26;
            BtnBackSpace.Icon = null;
            BtnBackSpace.IconOffset = 10;
            BtnBackSpace.IconSize = new System.Drawing.Size(24, 24);
            BtnBackSpace.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnBackSpace.IsIndicatorShow = false;
            BtnBackSpace.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnBackSpace.Location = new System.Drawing.Point(420, 123);
            BtnBackSpace.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            BtnBackSpace.MouseinBackColor = System.Drawing.Color.Chocolate;
            BtnBackSpace.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnBackSpace.MouseInBorderThickness = 1;
            BtnBackSpace.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnBackSpace.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnBackSpace.Name = "BtnBackSpace";
            BtnBackSpace.PressedBackColor = System.Drawing.Color.DarkOrange;
            BtnBackSpace.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnBackSpace.PressedBorderThickness = 1;
            BtnBackSpace.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnBackSpace.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnBackSpace.Size = new System.Drawing.Size(44, 26);
            BtnBackSpace.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            BtnBackSpace.StylizeFlag = false;
            BtnBackSpace.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnBackSpace.SVGPath = "";
            BtnBackSpace.TabIndex = 5;
            BtnBackSpace.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CustomFormulaForm.TlpEditor.BtnBackSpace"); // "删除";
            BtnBackSpace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnBackSpace.Click += BtnBackspace_Click;
            // 
            // BtnAccept
            // 
            BtnAccept.BackColor = System.Drawing.Color.FromArgb(54, 54, 54);
            BtnAccept.BorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            BtnAccept.BorderThickness = 0;
            BtnAccept.CornerRadius = 0;
            BtnAccept.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnAccept.DaskArray = null;
            BtnAccept.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnAccept.DropKey = System.Windows.Forms.Keys.Space;
            BtnAccept.Font = new System.Drawing.Font("MiSans", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnAccept.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAccept.Height = 27;
            BtnAccept.Icon = null;
            BtnAccept.IconOffset = 10;
            BtnAccept.IconSize = new System.Drawing.Size(24, 24);
            BtnAccept.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnAccept.IsIndicatorShow = false;
            BtnAccept.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnAccept.Location = new System.Drawing.Point(726, 643);
            BtnAccept.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            BtnAccept.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAccept.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAccept.MouseInBorderThickness = 1;
            BtnAccept.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAccept.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnAccept.Name = "BtnAccept";
            BtnAccept.PressedBackColor = System.Drawing.Color.Gray;
            BtnAccept.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAccept.PressedBorderThickness = 1;
            BtnAccept.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAccept.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnAccept.Size = new System.Drawing.Size(80, 27);
            BtnAccept.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            BtnAccept.StylizeFlag = false;
            BtnAccept.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAccept.SVGPath = "";
            BtnAccept.TabIndex = 10;
            BtnAccept.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CustomFormulaForm.TlpEditor.BtnAccept"); // "确定";
            BtnAccept.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnAccept.Click += BtnAccept_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.BackColor = System.Drawing.Color.FromArgb(54, 54, 54);
            BtnCancel.BorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            BtnCancel.BorderThickness = 0;
            BtnCancel.CornerRadius = 0;
            BtnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnCancel.DaskArray = null;
            BtnCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnCancel.DropKey = System.Windows.Forms.Keys.Space;
            BtnCancel.Font = new System.Drawing.Font("MiSans", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnCancel.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCancel.Height = 27;
            BtnCancel.Icon = null;
            BtnCancel.IconOffset = 10;
            BtnCancel.IconSize = new System.Drawing.Size(24, 24);
            BtnCancel.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCancel.IsIndicatorShow = false;
            BtnCancel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnCancel.Location = new System.Drawing.Point(816, 643);
            BtnCancel.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            BtnCancel.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCancel.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCancel.MouseInBorderThickness = 1;
            BtnCancel.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCancel.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.PressedBackColor = System.Drawing.Color.Gray;
            BtnCancel.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCancel.PressedBorderThickness = 1;
            BtnCancel.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCancel.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCancel.Size = new System.Drawing.Size(83, 27);
            BtnCancel.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            BtnCancel.StylizeFlag = false;
            BtnCancel.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCancel.SVGPath = "";
            BtnCancel.TabIndex = 11;
            BtnCancel.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CustomFormulaForm.TlpEditor.BtnCancel"); // "取消";
            BtnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // BtnClear
            // 
            BtnClear.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnClear.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnClear.BorderThickness = 0;
            BtnClear.CornerRadius = 0;
            BtnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnClear.DaskArray = null;
            BtnClear.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnClear.DropKey = System.Windows.Forms.Keys.Space;
            BtnClear.Font = new System.Drawing.Font("MiSans", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnClear.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnClear.Height = 26;
            BtnClear.Icon = null;
            BtnClear.IconOffset = 10;
            BtnClear.IconSize = new System.Drawing.Size(24, 24);
            BtnClear.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnClear.IsIndicatorShow = false;
            BtnClear.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnClear.Location = new System.Drawing.Point(366, 123);
            BtnClear.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            BtnClear.MouseinBackColor = System.Drawing.Color.Maroon;
            BtnClear.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnClear.MouseInBorderThickness = 1;
            BtnClear.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnClear.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnClear.Name = "BtnClear";
            BtnClear.PressedBackColor = System.Drawing.Color.FromArgb(192, 0, 0);
            BtnClear.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnClear.PressedBorderThickness = 1;
            BtnClear.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnClear.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnClear.Size = new System.Drawing.Size(44, 26);
            BtnClear.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            BtnClear.StylizeFlag = false;
            BtnClear.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnClear.SVGPath = "";
            BtnClear.TabIndex = 4;
            BtnClear.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CustomFormulaForm.TlpEditor.BtnClear"); // "清除";
            BtnClear.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnClear.Click += BtnClear_Click;
            // 
            // ImgSelection
            // 
            ImgSelection.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            ImgSelection.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("ImgSelection.ImageStream");
            ImgSelection.TransparentColor = System.Drawing.Color.Transparent;
            ImgSelection.Images.SetKeyName(0, "And.png");
            ImgSelection.Images.SetKeyName(1, "Comma.png");
            ImgSelection.Images.SetKeyName(2, "Dot.png");
            ImgSelection.Images.SetKeyName(3, "E.png");
            ImgSelection.Images.SetKeyName(4, "MathAbs.png");
            ImgSelection.Images.SetKeyName(5, "MathAdd.png");
            ImgSelection.Images.SetKeyName(6, "MathAverage.png");
            ImgSelection.Images.SetKeyName(7, "MathCommonmode.png");
            ImgSelection.Images.SetKeyName(8, "MathConv.png");
            ImgSelection.Images.SetKeyName(9, "MathCorr.png");
            ImgSelection.Images.SetKeyName(10, "MathCos.png");
            ImgSelection.Images.SetKeyName(11, "MathDer.png");
            ImgSelection.Images.SetKeyName(12, "MathDiv.png");
            ImgSelection.Images.SetKeyName(13, "MathEXP.png");
            ImgSelection.Images.SetKeyName(14, "MathExp10.png");
            ImgSelection.Images.SetKeyName(15, "MathGt.png");
            ImgSelection.Images.SetKeyName(16, "MathInt.png");
            ImgSelection.Images.SetKeyName(17, "MathLB.png");
            ImgSelection.Images.SetKeyName(18, "MathLn.png");
            ImgSelection.Images.SetKeyName(19, "MathLog10.png");
            ImgSelection.Images.SetKeyName(20, "MathLt.png");
            ImgSelection.Images.SetKeyName(21, "MathMultpl.png");
            ImgSelection.Images.SetKeyName(22, "MathRB.png");
            ImgSelection.Images.SetKeyName(23, "MathSin.png");
            ImgSelection.Images.SetKeyName(24, "MathSqroot.png");
            ImgSelection.Images.SetKeyName(25, "MathSquare.png");
            ImgSelection.Images.SetKeyName(26, "MathSub.png");
            ImgSelection.Images.SetKeyName(27, "MathTan.png");
            ImgSelection.Images.SetKeyName(28, "Not.png");
            ImgSelection.Images.SetKeyName(29, "Number0.png");
            ImgSelection.Images.SetKeyName(30, "Number1.png");
            ImgSelection.Images.SetKeyName(31, "Number2.png");
            ImgSelection.Images.SetKeyName(32, "Number3.png");
            ImgSelection.Images.SetKeyName(33, "Number4.png");
            ImgSelection.Images.SetKeyName(34, "Number5.png");
            ImgSelection.Images.SetKeyName(35, "Number6.png");
            ImgSelection.Images.SetKeyName(36, "Number7.png");
            ImgSelection.Images.SetKeyName(37, "Number8.png");
            ImgSelection.Images.SetKeyName(38, "Number9.png");
            ImgSelection.Images.SetKeyName(39, "Or.png");
            ImgSelection.Images.SetKeyName(40, "Pi.png");
            ImgSelection.Images.SetKeyName(41, "Subvector.png");
            ImgSelection.Images.SetKeyName(42, "Xor.png");
            ImgSelection.Images.SetKeyName(43, "MathAcos.png");
            ImgSelection.Images.SetKeyName(44, "MathAsin.png");
            ImgSelection.Images.SetKeyName(45, "MathAtan.png");
            ImgSelection.Images.SetKeyName(46, "MathCosh.png");
            ImgSelection.Images.SetKeyName(47, "MathDegrees.png");
            ImgSelection.Images.SetKeyName(48, "MathMax.png");
            ImgSelection.Images.SetKeyName(49, "MathMin.png");
            ImgSelection.Images.SetKeyName(50, "MathRadians.png");
            ImgSelection.Images.SetKeyName(51, "MathReciprocal.png");
            ImgSelection.Images.SetKeyName(52, "MathTanh.png");
            ImgSelection.Images.SetKeyName(53, "MathSinh.png");
            ImgSelection.Images.SetKeyName(54, "Interpolate.png");
            ImgSelection.Images.SetKeyName(55, "Mod.png");
            // 
            // LvFunctions
            // 
            LvFunctions.AutoScroll = true;
            LvFunctions.Dock = System.Windows.Forms.DockStyle.Fill;
            LvFunctions.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            LvFunctions.Location = new System.Drawing.Point(0, 0);
            LvFunctions.Name = "LvFunctions";
            LvFunctions.ScrollContainer = ScFunctions;
            LvFunctions.Size = new System.Drawing.Size(589, 311);
            LvFunctions.TabIndex = 0;
            // 
            // ScFunctions
            // 
            ScFunctions.BackColor = System.Drawing.Color.FromArgb(54, 54, 54);
            TlpEditor.SetColumnSpan(ScFunctions, 6);
            ScFunctions.Controls.Add(LvFunctions);
            ScFunctions.Dock = System.Windows.Forms.DockStyle.Fill;
            ScFunctions.Location = new System.Drawing.Point(3, 326);
            ScFunctions.Name = "ScFunctions";
            ScFunctions.ScrollControl = LvFunctions;
            ScFunctions.ScrollThickness = 6;
            ScFunctions.Size = new System.Drawing.Size(589, 311);
            ScFunctions.TabIndex = 8;
            // 
            // RtbxEditor
            // 
            RtbxEditor.BackColor = System.Drawing.Color.FromArgb(54, 54, 54);
            RtbxEditor.BorderStyle = System.Windows.Forms.BorderStyle.None;
            TlpEditor.SetColumnSpan(RtbxEditor, 5);
            RtbxEditor.DetectUrls = false;
            RtbxEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            RtbxEditor.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RtbxEditor.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RtbxEditor.ImeMode = System.Windows.Forms.ImeMode.Disable;
            RtbxEditor.Location = new System.Drawing.Point(3, 3);
            RtbxEditor.Name = "RtbxEditor";
            RtbxEditor.ReadOnly = true;
            RtbxEditor.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Horizontal;
            RtbxEditor.ShortcutsEnabled = false;
            RtbxEditor.Size = new System.Drawing.Size(463, 114);
            RtbxEditor.TabIndex = 0;
            RtbxEditor.TabStop = false;
            RtbxEditor.Text = "C1+C2";
            // 
            // TlpEditor
            // 
            TlpEditor.ColumnCount = 9;
            TlpEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28F));
            TlpEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6F));
            TlpEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6F));
            TlpEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6F));
            TlpEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6F));
            TlpEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            TlpEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            TlpEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            TlpEditor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            TlpEditor.Controls.Add(LblDescription, 5, 0);
            TlpEditor.Controls.Add(RdoSource, 6, 1);
            TlpEditor.Controls.Add(RtbxEditor, 0, 0);
            TlpEditor.Controls.Add(BtnClear, 3, 1);
            TlpEditor.Controls.Add(BtnBackSpace, 4, 1);
            TlpEditor.Controls.Add(BtnLoadFormula, 1, 1);
            TlpEditor.Controls.Add(BtnSaveFormula, 2, 1);
            TlpEditor.Controls.Add(BtnCancel, 8, 4);
            TlpEditor.Controls.Add(BtnAccept, 7, 4);
            TlpEditor.Controls.Add(ScSource, 0, 2);
            TlpEditor.Controls.Add(ScFunctions, 0, 3);
            TlpEditor.Controls.Add(ScNumberics, 6, 3);
            TlpEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpEditor.Location = new System.Drawing.Point(1, 46);
            TlpEditor.Name = "TlpEditor";
            TlpEditor.RowCount = 5;
            TlpEditor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            TlpEditor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            TlpEditor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            TlpEditor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            TlpEditor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            TlpEditor.Size = new System.Drawing.Size(904, 673);
            TlpEditor.TabIndex = 0;
            // 
            // LblDescription
            // 
            LblDescription.BackColor = System.Drawing.Color.FromArgb(54, 54, 54);
            LblDescription.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblDescription.BorderThickness = 0;
            TlpEditor.SetColumnSpan(LblDescription, 4);
            LblDescription.CornerRadius = 0;
            LblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            LblDescription.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblDescription.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblDescription.HighlightPrompt = defaultHighlightPrompt1;
            LblDescription.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblDescription.Location = new System.Drawing.Point(472, 3);
            LblDescription.MultyLineFlag = false;
            LblDescription.Name = "LblDescription";
            LblDescription.IsOmittext = false;
            LblDescription.Size = new System.Drawing.Size(429, 114);
            LblDescription.StyleFlags = UserControls.Style.StyleFlag.None;
            LblDescription.StylizeFlag = false;
            LblDescription.TabIndex = 1;
            LblDescription.TabStop = false;
            LblDescription.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CustomFormulaForm.TlpEditor.LblDescription"); // "选择所需操作符和操作数构造公式，自带左括号的操作符需自行补全右括号。";
            LblDescription.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            LblDescription.Token = null;
            // 
            // RdoSource
            // 
            RdoSource.BackColor = System.Drawing.Color.Black;
            RdoSource.BorderColor = System.Drawing.Color.Black;
            RdoSource.BorderThickness = 0;
            RdoSource.ButtonBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoSource.ButtonFont = null;
            RdoSource.ButtonOffset = 10;
            RdoSource.ButtonTextColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RdoSource.ChoosedButtonColor = System.Drawing.Color.FromArgb(0, 157, 255);
            RdoSource.ChoosedButtonIndex = 0;
            RdoSource.ChoosedButtonTextColor = System.Drawing.Color.Black;
            TlpEditor.SetColumnSpan(RdoSource, 3);
            RdoSource.ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            RdoSource.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoSource.Dock = System.Windows.Forms.DockStyle.Right;
            RdoSource.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoSource.Font = new System.Drawing.Font("MiSans", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoSource.Height = 26;
            RdoSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoSource.Location = new System.Drawing.Point(598, 123);
            RdoSource.Name = "RdoSource";
            RdoSource.Size = new System.Drawing.Size(303, 26);
            RdoSource.StyleFlags = UserControls.Style.StyleFlag.None;
            RdoSource.StylizeFlag = false;
            RdoSource.TabIndex = 6;
            RdoSource.IndexChanged += RdoSource_IndexChanged;
            // 
            // BtnLoadFormula
            // 
            BtnLoadFormula.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnLoadFormula.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnLoadFormula.BorderThickness = 0;
            BtnLoadFormula.CornerRadius = 0;
            BtnLoadFormula.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnLoadFormula.DaskArray = null;
            BtnLoadFormula.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnLoadFormula.DropKey = System.Windows.Forms.Keys.Space;
            BtnLoadFormula.Font = new System.Drawing.Font("MiSans", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnLoadFormula.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnLoadFormula.Height = 26;
            BtnLoadFormula.Icon = null;
            BtnLoadFormula.IconOffset = 10;
            BtnLoadFormula.IconSize = new System.Drawing.Size(24, 24);
            BtnLoadFormula.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnLoadFormula.IsIndicatorShow = false;
            BtnLoadFormula.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnLoadFormula.Location = new System.Drawing.Point(258, 123);
            BtnLoadFormula.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            BtnLoadFormula.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnLoadFormula.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnLoadFormula.MouseInBorderThickness = 1;
            BtnLoadFormula.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnLoadFormula.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnLoadFormula.Name = "BtnLoadFormula";
            BtnLoadFormula.PressedBackColor = System.Drawing.Color.Gray;
            BtnLoadFormula.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnLoadFormula.PressedBorderThickness = 1;
            BtnLoadFormula.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnLoadFormula.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnLoadFormula.Size = new System.Drawing.Size(44, 26);
            BtnLoadFormula.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            BtnLoadFormula.StylizeFlag = false;
            BtnLoadFormula.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnLoadFormula.SVGPath = "";
            BtnLoadFormula.TabIndex = 2;
            BtnLoadFormula.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CustomFormulaForm.TlpEditor.BtnLoadFormula"); // "加载";
            BtnLoadFormula.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnLoadFormula.Click += BtnLoadFormula_Click;
            // 
            // BtnSaveFormula
            // 
            BtnSaveFormula.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveFormula.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveFormula.BorderThickness = 0;
            BtnSaveFormula.CornerRadius = 0;
            BtnSaveFormula.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSaveFormula.DaskArray = null;
            BtnSaveFormula.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnSaveFormula.DropKey = System.Windows.Forms.Keys.Space;
            BtnSaveFormula.Font = new System.Drawing.Font("MiSans", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnSaveFormula.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveFormula.Height = 26;
            BtnSaveFormula.Icon = null;
            BtnSaveFormula.IconOffset = 10;
            BtnSaveFormula.IconSize = new System.Drawing.Size(24, 24);
            BtnSaveFormula.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSaveFormula.IsIndicatorShow = false;
            BtnSaveFormula.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSaveFormula.Location = new System.Drawing.Point(312, 123);
            BtnSaveFormula.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            BtnSaveFormula.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveFormula.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveFormula.MouseInBorderThickness = 1;
            BtnSaveFormula.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveFormula.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSaveFormula.Name = "BtnSaveFormula";
            BtnSaveFormula.PressedBackColor = System.Drawing.Color.Gray;
            BtnSaveFormula.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveFormula.PressedBorderThickness = 1;
            BtnSaveFormula.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveFormula.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSaveFormula.Size = new System.Drawing.Size(44, 26);
            BtnSaveFormula.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            BtnSaveFormula.StylizeFlag = false;
            BtnSaveFormula.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveFormula.SVGPath = "";
            BtnSaveFormula.TabIndex = 3;
            BtnSaveFormula.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CustomFormulaForm.TlpEditor.BtnSaveFormula"); // "保存";
            BtnSaveFormula.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSaveFormula.Click += BtnSaveFormula_Click;
            // 
            // ScSource
            // 
            ScSource.BackColor = System.Drawing.Color.FromArgb(54, 54, 54);
            TlpEditor.SetColumnSpan(ScSource, 9);
            ScSource.Controls.Add(LvSources);
            ScSource.Dock = System.Windows.Forms.DockStyle.Fill;
            ScSource.Location = new System.Drawing.Point(3, 155);
            ScSource.Name = "ScSource";
            ScSource.ScrollControl = LvSources;
            ScSource.ScrollThickness = 6;
            ScSource.Size = new System.Drawing.Size(898, 165);
            ScSource.TabIndex = 7;
            // 
            // LvSources
            // 
            LvSources.AutoScroll = true;
            LvSources.Dock = System.Windows.Forms.DockStyle.Fill;
            LvSources.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            LvSources.Location = new System.Drawing.Point(0, 0);
            LvSources.Name = "LvSources";
            LvSources.ScrollContainer = ScSource;
            LvSources.Size = new System.Drawing.Size(898, 165);
            LvSources.TabIndex = 0;
            // 
            // ScNumberics
            // 
            ScNumberics.BackColor = System.Drawing.Color.FromArgb(54, 54, 54);
            TlpEditor.SetColumnSpan(ScNumberics, 3);
            ScNumberics.Controls.Add(LvNumberics);
            ScNumberics.Dock = System.Windows.Forms.DockStyle.Fill;
            ScNumberics.Location = new System.Drawing.Point(598, 326);
            ScNumberics.Name = "ScNumberics";
            ScNumberics.ScrollControl = LvNumberics;
            ScNumberics.ScrollThickness = 6;
            ScNumberics.Size = new System.Drawing.Size(303, 311);
            ScNumberics.TabIndex = 9;
            // 
            // LvNumberics
            // 
            LvNumberics.AutoScroll = true;
            LvNumberics.Dock = System.Windows.Forms.DockStyle.Fill;
            LvNumberics.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            LvNumberics.Location = new System.Drawing.Point(0, 0);
            LvNumberics.Name = "LvNumberics";
            LvNumberics.ScrollContainer = ScNumberics;
            LvNumberics.Size = new System.Drawing.Size(303, 311);
            LvNumberics.TabIndex = 0;
            // 
            // ToolTip
            // 
            ToolTip.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ToolTip.ForeColor = System.Drawing.Color.FromArgb(210, 210, 210);
            ToolTip.OwnerDraw = true;
            ToolTip.ShowAlways = true;
            // 
            // CustomFormulaForm
            // 
            ActiveBorderColor = System.Drawing.Color.FromArgb(11, 39, 133);
            ActiveBorderVisiable = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ClientSize = new System.Drawing.Size(906, 720);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(TlpEditor);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HelpLabel = "20";
            IconInterval = 21;
            IconWidth = 26;
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(650, 500);
            Name = "CustomFormulaForm";
            Padding = new System.Windows.Forms.Padding(1);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CustomFormulaForm"); // "公式编辑器";
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CustomFormulaForm", "Title"); // "公式编辑器";
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Controls.SetChildIndex(TlpEditor, 0);
            ScFunctions.ResumeLayout(false);
            TlpEditor.ResumeLayout(false);
            ScSource.ResumeLayout(false);
            ScNumberics.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXIconButton BtnBackSpace;
        private ScopeX.UserControls.ScopeXIconButton BtnAccept;
        private ScopeX.UserControls.ScopeXIconButton BtnCancel;
        private ScopeX.UserControls.ScopeXIconButton BtnClear;


        private System.Windows.Forms.ImageList ImgSelection;
        private ScopeX.UserControls.ScrollPanel LvFunctions;
        private System.Windows.Forms.RichTextBox RtbxEditor;
        private System.Windows.Forms.TableLayoutPanel TlpEditor;
        private ScopeX.UserControls.ScrollPanel LvSources;
        private ScopeX.UserControls.ScopeXIconButton BtnLoadFormula;
        private ScopeX.UserControls.ScopeXIconButton BtnSaveFormula;
        private ScopeX.UserControls.UIRadioButtonGroup RdoSource;
        private ScopeX.UserControls.ScrollPanel LvNumberics;
        private ScopeX.UserControls.ScopeXScrollContainer ScSource;
        private ScopeX.UserControls.ScopeXScrollContainer ScFunctions;
        private ScopeX.UserControls.ScopeXScrollContainer ScNumberics;
        private System.Windows.Forms.ToolTip ToolTip;
        private ScopeX.UserControls.ScopeXLabel LblDescription;
    }


}