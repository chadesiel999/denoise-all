namespace ScopeX.U2
{
    partial class ProbeUserCaliPage
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
            components = new System.ComponentModel.Container();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new UserControls.DefaultHighlightPrompt();
            panelHead = new System.Windows.Forms.Panel();
            TlpBody = new System.Windows.Forms.TableLayoutPanel();
            ScCalibItems = new UserControls.ScopeXScrollContainer();
            LblInfo = new UserControls.ScopeXLabel();
            PgBar = new ProgressBar();
            LvCalibItems = new UserControls.ScopeXListViewEx();
            BtnPanel = new System.Windows.Forms.Panel();
            BtnOther = new UserControls.ScopeXIconButton();
            BtnReadCali = new UserControls.ScopeXIconButton();
            BtnClrCali = new UserControls.ScopeXIconButton();
            BtnCloseRef = new UserControls.ScopeXIconButton();
            BtnCancel = new UserControls.ScopeXIconButton();
            BtnCalib = new UserControls.ScopeXIconButton();
            LblSpaceFrst = new UserControls.ScopeXLabel();
            PbWiring = new System.Windows.Forms.PictureBox();
            panelHead.SuspendLayout();
            TlpBody.SuspendLayout();
            ScCalibItems.SuspendLayout();
            BtnPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PbWiring).BeginInit();
            SuspendLayout();
            // 
            // panelHead
            // 
            panelHead.BackColor = System.Drawing.Color.FromArgb(41, 42, 42);
            panelHead.Controls.Add(TlpBody);
            panelHead.Dock = System.Windows.Forms.DockStyle.Fill;
            panelHead.Location = new System.Drawing.Point(0, 0);
            panelHead.Name = "panelHead";
            panelHead.Size = new System.Drawing.Size(700, 480);
            panelHead.TabIndex = 1;
            // 
            // TlpBody
            // 
            TlpBody.BackColor = System.Drawing.Color.Transparent;
            TlpBody.ColumnCount = 1;
            TlpBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TlpBody.Controls.Add(ScCalibItems, 0, 3);
            TlpBody.Controls.Add(BtnPanel, 0, 2);
            TlpBody.Controls.Add(LblSpaceFrst, 0, 0);
            TlpBody.Controls.Add(PbWiring, 0, 1);
            TlpBody.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpBody.Location = new System.Drawing.Point(0, 0);
            TlpBody.Name = "TlpBody";
            TlpBody.RowCount = 4;
            TlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpBody.Size = new System.Drawing.Size(700, 480);
            TlpBody.TabIndex = 6;
            // 
            // ScCalibItems
            // 
            ScCalibItems.AutoScroll = true;
            ScCalibItems.BackColor = System.Drawing.Color.FromArgb(45, 46, 50);
            ScCalibItems.Controls.Add(LblInfo);
            ScCalibItems.Controls.Add(PgBar);
            ScCalibItems.Controls.Add(LvCalibItems);
            ScCalibItems.Dock = System.Windows.Forms.DockStyle.Fill;
            ScCalibItems.Location = new System.Drawing.Point(3, 340);
            ScCalibItems.Name = "ScCalibItems";
            ScCalibItems.ScrollControl = LvCalibItems;
            ScCalibItems.ScrollThickness = 6;
            ScCalibItems.Size = new System.Drawing.Size(694, 137);
            ScCalibItems.TabIndex = 16;
            // 
            // LblInfo
            // 
            LblInfo.AccessibleRole = System.Windows.Forms.AccessibleRole.IpAddress;
            LblInfo.BackColor = System.Drawing.Color.Transparent;
            LblInfo.BorderColor = System.Drawing.Color.Black;
            LblInfo.BorderThickness = 0;
            LblInfo.CornerRadius = 0;
            LblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            LblInfo.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblInfo.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblInfo.HighlightPrompt = defaultHighlightPrompt1;
            LblInfo.IsOmittext = false;
            LblInfo.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblInfo.Location = new System.Drawing.Point(0, 31);
            LblInfo.MultyLineFlag = false;
            LblInfo.Name = "LblInfo";
            LblInfo.Size = new System.Drawing.Size(694, 106);
            LblInfo.StyleFlags = UserControls.Style.StyleFlag.None;
            LblInfo.StylizeFlag = false;
            LblInfo.TabIndex = 17;
            LblInfo.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            LblInfo.Token = null;
            // 
            // PgBar
            // 
            PgBar.BorderColor = System.Drawing.Color.Transparent;
            PgBar.BorderWidth = 0;
            PgBar.Cursor = System.Windows.Forms.Cursors.No;
            PgBar.DescriptionInfo = "";
            PgBar.Dock = System.Windows.Forms.DockStyle.Top;
            PgBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            PgBar.ForeColor = System.Drawing.Color.Black;
            PgBar.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            PgBar.Location = new System.Drawing.Point(0, 0);
            PgBar.MaxValue = 100;
            PgBar.Name = "PgBar";
            PgBar.Opacity = 90;
            PgBar.Size = new System.Drawing.Size(694, 31);
            PgBar.StyleFlags = UserControls.Style.StyleFlag.None;
            PgBar.StylizeFlag = false;
            PgBar.TabIndex = 16;
            PgBar.UseWaitCursor = true;
            PgBar.Value = 0;
            PgBar.ValueBackgroundColor = System.Drawing.Color.WhiteSmoke;
            PgBar.ValueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            PgBar.ValueTxtType = ValueTextType.Percent;
            // 
            // LvCalibItems
            // 
            LvCalibItems.BackColor = System.Drawing.Color.FromArgb(45, 46, 50);
            LvCalibItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            LvCalibItems.Dock = System.Windows.Forms.DockStyle.Fill;
            LvCalibItems.ForeColor = System.Drawing.Color.FromArgb(234, 234, 234);
            LvCalibItems.FullRowSelect = true;
            LvCalibItems.GridLinesColor = System.Drawing.Color.Black;
            LvCalibItems.HeaderBackColor = System.Drawing.Color.Red;
            LvCalibItems.HeaderForeColor = System.Drawing.Color.Silver;
            LvCalibItems.IsIndependentWindow = false;
            LvCalibItems.Location = new System.Drawing.Point(0, 0);
            LvCalibItems.Margin = new System.Windows.Forms.Padding(10, 3, 10, 10);
            LvCalibItems.Name = "LvCalibItems";
            LvCalibItems.OwnerDraw = true;
            LvCalibItems.RowHeight = 20;
            LvCalibItems.ScrollContainer = ScCalibItems;
            LvCalibItems.SelectedRowColor = System.Drawing.Color.MediumSeaGreen;
            LvCalibItems.Size = new System.Drawing.Size(694, 137);
            LvCalibItems.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            LvCalibItems.StylizeFlag = true;
            LvCalibItems.TabIndex = 15;
            LvCalibItems.UseCompatibleStateImageBehavior = false;
            LvCalibItems.View = System.Windows.Forms.View.Details;
            LvCalibItems.Visible = false;
            // 
            // BtnPanel
            // 
            BtnPanel.Controls.Add(BtnOther);
            BtnPanel.Controls.Add(BtnReadCali);
            BtnPanel.Controls.Add(BtnClrCali);
            BtnPanel.Controls.Add(BtnCloseRef);
            BtnPanel.Controls.Add(BtnCancel);
            BtnPanel.Controls.Add(BtnCalib);
            BtnPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnPanel.Location = new System.Drawing.Point(0, 294);
            BtnPanel.Margin = new System.Windows.Forms.Padding(0);
            BtnPanel.Name = "BtnPanel";
            BtnPanel.Size = new System.Drawing.Size(700, 43);
            BtnPanel.TabIndex = 19;
            // 
            // BtnOther
            // 
            BtnOther.Adjustable = false;
            BtnOther.BackColor = System.Drawing.Color.Transparent;
            BtnOther.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOther.BorderThickness = 0;
            BtnOther.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnOther.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnOther.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnOther.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnOther.CornerRadius = 0;
            BtnOther.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnOther.DaskArray = null;
            BtnOther.DoubleClickEnable = true;
            BtnOther.DropKey = System.Windows.Forms.Keys.Space;
            BtnOther.FineEnable = false;
            BtnOther.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnOther.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnOther.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnOther.Height = 35;
            BtnOther.Icon = null;
            BtnOther.IconOffset = 10;
            BtnOther.IconSize = new System.Drawing.Size(24, 24);
            BtnOther.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnOther.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnOther.IsChoosed = false;
            BtnOther.IsIndicatorShow = false;
            BtnOther.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnOther.Location = new System.Drawing.Point(658, 3);
            BtnOther.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOther.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOther.MouseInBorderThickness = 0;
            BtnOther.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOther.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnOther.Name = "BtnOther";
            BtnOther.PressedBackColor = System.Drawing.Color.Gray;
            BtnOther.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOther.PressedBorderThickness = 0;
            BtnOther.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOther.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnOther.Size = new System.Drawing.Size(30, 35);
            BtnOther.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnOther.StylizeFlag = true;
            BtnOther.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOther.SVGPath = "";
            BtnOther.TabIndex = 23;
            BtnOther.Text = "...";
            BtnOther.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnOther.Click += BtnOther_Click;
            // 
            // BtnReadCali
            // 
            BtnReadCali.Adjustable = false;
            BtnReadCali.BackColor = System.Drawing.Color.Transparent;
            BtnReadCali.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnReadCali.BorderThickness = 0;
            BtnReadCali.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnReadCali.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnReadCali.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnReadCali.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnReadCali.CornerRadius = 0;
            BtnReadCali.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnReadCali.DaskArray = null;
            BtnReadCali.DoubleClickEnable = true;
            BtnReadCali.DropKey = System.Windows.Forms.Keys.Space;
            BtnReadCali.FineEnable = false;
            BtnReadCali.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnReadCali.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnReadCali.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnReadCali.Height = 45;
            BtnReadCali.Icon = null;
            BtnReadCali.IconOffset = 10;
            BtnReadCali.IconSize = new System.Drawing.Size(24, 24);
            BtnReadCali.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnReadCali.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnReadCali.IsChoosed = false;
            BtnReadCali.IsIndicatorShow = false;
            BtnReadCali.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnReadCali.Location = new System.Drawing.Point(406, 3);
            BtnReadCali.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnReadCali.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnReadCali.MouseInBorderThickness = 0;
            BtnReadCali.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnReadCali.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnReadCali.Name = "BtnReadCali";
            BtnReadCali.PressedBackColor = System.Drawing.Color.Gray;
            BtnReadCali.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnReadCali.PressedBorderThickness = 0;
            BtnReadCali.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnReadCali.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnReadCali.Size = new System.Drawing.Size(120, 45);
            BtnReadCali.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnReadCali.StylizeFlag = true;
            BtnReadCali.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnReadCali.SVGPath = "";
            BtnReadCali.TabIndex = 22;
            BtnReadCali.Text = "Read Cali Data";
            BtnReadCali.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnReadCali.Click += BtnReadCali_Click;
            // 
            // BtnClrCali
            // 
            BtnClrCali.Adjustable = false;
            BtnClrCali.BackColor = System.Drawing.Color.Transparent;
            BtnClrCali.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnClrCali.BorderThickness = 0;
            BtnClrCali.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnClrCali.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnClrCali.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnClrCali.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnClrCali.CornerRadius = 0;
            BtnClrCali.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnClrCali.DaskArray = null;
            BtnClrCali.DoubleClickEnable = true;
            BtnClrCali.DropKey = System.Windows.Forms.Keys.Space;
            BtnClrCali.FineEnable = false;
            BtnClrCali.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnClrCali.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnClrCali.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnClrCali.Height = 43;
            BtnClrCali.Icon = null;
            BtnClrCali.IconOffset = 10;
            BtnClrCali.IconSize = new System.Drawing.Size(24, 24);
            BtnClrCali.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnClrCali.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnClrCali.IsChoosed = false;
            BtnClrCali.IsIndicatorShow = false;
            BtnClrCali.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnClrCali.Location = new System.Drawing.Point(280, 5);
            BtnClrCali.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnClrCali.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnClrCali.MouseInBorderThickness = 0;
            BtnClrCali.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnClrCali.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnClrCali.Name = "BtnClrCali";
            BtnClrCali.PressedBackColor = System.Drawing.Color.Gray;
            BtnClrCali.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnClrCali.PressedBorderThickness = 0;
            BtnClrCali.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnClrCali.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnClrCali.Size = new System.Drawing.Size(120, 43);
            BtnClrCali.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnClrCali.StylizeFlag = true;
            BtnClrCali.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnClrCali.SVGPath = "";
            BtnClrCali.TabIndex = 21;
            BtnClrCali.Text = "Clear Cali Data";
            BtnClrCali.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnClrCali.Click += BtnClrCali_Click;
            // 
            // BtnCloseRef
            // 
            BtnCloseRef.Adjustable = false;
            BtnCloseRef.BackColor = System.Drawing.Color.Transparent;
            BtnCloseRef.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCloseRef.BorderThickness = 0;
            BtnCloseRef.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCloseRef.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnCloseRef.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCloseRef.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCloseRef.CornerRadius = 0;
            BtnCloseRef.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnCloseRef.DaskArray = null;
            BtnCloseRef.DoubleClickEnable = true;
            BtnCloseRef.DropKey = System.Windows.Forms.Keys.Space;
            BtnCloseRef.FineEnable = false;
            BtnCloseRef.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnCloseRef.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnCloseRef.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnCloseRef.Height = 45;
            BtnCloseRef.Icon = null;
            BtnCloseRef.IconOffset = 10;
            BtnCloseRef.IconSize = new System.Drawing.Size(24, 24);
            BtnCloseRef.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnCloseRef.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCloseRef.IsChoosed = false;
            BtnCloseRef.IsIndicatorShow = false;
            BtnCloseRef.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnCloseRef.Location = new System.Drawing.Point(532, 3);
            BtnCloseRef.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCloseRef.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCloseRef.MouseInBorderThickness = 0;
            BtnCloseRef.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCloseRef.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCloseRef.Name = "BtnCloseRef";
            BtnCloseRef.PressedBackColor = System.Drawing.Color.Gray;
            BtnCloseRef.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCloseRef.PressedBorderThickness = 0;
            BtnCloseRef.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCloseRef.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCloseRef.Size = new System.Drawing.Size(120, 45);
            BtnCloseRef.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnCloseRef.StylizeFlag = true;
            BtnCloseRef.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCloseRef.SVGPath = "";
            BtnCloseRef.TabIndex = 20;
            BtnCloseRef.Text = "Close Square wave";
            BtnCloseRef.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnCloseRef.Click += BtnCloseRef_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Adjustable = false;
            BtnCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            BtnCancel.BackColor = System.Drawing.Color.Transparent;
            BtnCancel.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCancel.BorderThickness = 0;
            BtnCancel.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCancel.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnCancel.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCancel.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCancel.CornerRadius = 0;
            BtnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnCancel.DaskArray = null;
            BtnCancel.DoubleClickEnable = true;
            BtnCancel.DropKey = System.Windows.Forms.Keys.Space;
            BtnCancel.FineEnable = false;
            BtnCancel.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnCancel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnCancel.Height = 42;
            BtnCancel.Icon = null;
            BtnCancel.IconOffset = 10;
            BtnCancel.IconSize = new System.Drawing.Size(24, 24);
            BtnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnCancel.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCancel.IsChoosed = false;
            BtnCancel.IsIndicatorShow = false;
            BtnCancel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnCancel.Location = new System.Drawing.Point(132, 3);
            BtnCancel.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCancel.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCancel.MouseInBorderThickness = 0;
            BtnCancel.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCancel.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.PressedBackColor = System.Drawing.Color.Gray;
            BtnCancel.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCancel.PressedBorderThickness = 0;
            BtnCancel.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCancel.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCancel.Size = new System.Drawing.Size(120, 42);
            BtnCancel.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnCancel.StylizeFlag = true;
            BtnCancel.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCancel.SVGPath = "";
            BtnCancel.TabIndex = 19;
            BtnCancel.Text = "Cancel";
            BtnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // BtnCalib
            // 
            BtnCalib.Adjustable = false;
            BtnCalib.BackColor = System.Drawing.Color.Transparent;
            BtnCalib.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCalib.BorderThickness = 0;
            BtnCalib.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCalib.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnCalib.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCalib.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCalib.CornerRadius = 0;
            BtnCalib.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnCalib.DaskArray = null;
            BtnCalib.DoubleClickEnable = true;
            BtnCalib.DropKey = System.Windows.Forms.Keys.Space;
            BtnCalib.FineEnable = false;
            BtnCalib.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnCalib.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnCalib.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnCalib.Height = 40;
            BtnCalib.Icon = null;
            BtnCalib.IconOffset = 10;
            BtnCalib.IconSize = new System.Drawing.Size(24, 24);
            BtnCalib.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnCalib.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCalib.IsChoosed = false;
            BtnCalib.IsIndicatorShow = false;
            BtnCalib.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnCalib.Location = new System.Drawing.Point(3, 3);
            BtnCalib.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCalib.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCalib.MouseInBorderThickness = 0;
            BtnCalib.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCalib.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCalib.Name = "BtnCalib";
            BtnCalib.PressedBackColor = System.Drawing.Color.Gray;
            BtnCalib.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCalib.PressedBorderThickness = 0;
            BtnCalib.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCalib.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCalib.Size = new System.Drawing.Size(120, 40);
            BtnCalib.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnCalib.StylizeFlag = true;
            BtnCalib.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCalib.SVGPath = "";
            BtnCalib.TabIndex = 18;
            BtnCalib.Text = "Cali";
            BtnCalib.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnCalib.Click += BtnCalib_Click;
            // 
            // LblSpaceFrst
            // 
            LblSpaceFrst.AccessibleRole = System.Windows.Forms.AccessibleRole.IpAddress;
            LblSpaceFrst.BackColor = System.Drawing.Color.Transparent;
            LblSpaceFrst.BorderColor = System.Drawing.Color.Black;
            LblSpaceFrst.BorderThickness = 0;
            LblSpaceFrst.CornerRadius = 0;
            LblSpaceFrst.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSpaceFrst.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblSpaceFrst.HighlightPrompt = defaultHighlightPrompt2;
            LblSpaceFrst.IsOmittext = true;
            LblSpaceFrst.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSpaceFrst.Location = new System.Drawing.Point(10, 3);
            LblSpaceFrst.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            LblSpaceFrst.MultyLineFlag = false;
            LblSpaceFrst.Name = "LblSpaceFrst";
            LblSpaceFrst.Size = new System.Drawing.Size(687, 19);
            LblSpaceFrst.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSpaceFrst.StylizeFlag = true;
            LblSpaceFrst.TabIndex = 1;
            LblSpaceFrst.Text = "占位";
            LblSpaceFrst.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSpaceFrst.Token = null;
            // 
            // PbWiring
            // 
            PbWiring.Cursor = System.Windows.Forms.Cursors.No;
            PbWiring.Location = new System.Drawing.Point(3, 28);
            PbWiring.Name = "PbWiring";
            PbWiring.Size = new System.Drawing.Size(694, 263);
            PbWiring.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            PbWiring.TabIndex = 0;
            PbWiring.TabStop = false;
            // 
            // ProbeUserCaliPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panelHead);
            Name = "ProbeUserCaliPage";
            Size = new System.Drawing.Size(700, 480);
            panelHead.ResumeLayout(false);
            TlpBody.ResumeLayout(false);
            ScCalibItems.ResumeLayout(false);
            ScCalibItems.PerformLayout();
            BtnPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PbWiring).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelHead;
        private System.Windows.Forms.TableLayoutPanel TlpBody;
        private UserControls.ScopeXLabel LblSpaceFrst;
        private System.Windows.Forms.PictureBox PbWiring;
        private UserControls.ScopeXScrollContainer ScCalibItems;
        private UserControls.ScopeXListViewEx LvCalibItems;
        private System.Windows.Forms.Panel BtnPanel;
        private UserControls.ScopeXIconButton BtnCancel;
        private UserControls.ScopeXIconButton BtnCalib;
        private UserControls.ScopeXIconButton BtnCloseRef;
        private UserControls.ScopeXIconButton BtnClrCali;
        private UserControls.ScopeXIconButton scopexIconButton1;
        private UserControls.ScopeXIconButton BtnReadCali;
        private UserControls.ScopeXIconButton scopexIconButton2;
        private UserControls.ScopeXIconButton BtnOther;
        private ProgressBar PgBar;
        private System.Windows.Forms.PictureBox pictureBox1;
        private UserControls.ScopeXLabel LblInfo;
    }
}
