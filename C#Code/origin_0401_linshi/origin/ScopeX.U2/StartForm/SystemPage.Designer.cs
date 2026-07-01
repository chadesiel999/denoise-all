
namespace ScopeX.U2
{
    partial class SystemPage
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
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt5 = new ScopeX.UserControls.DefaultHighlightPrompt();
            this.TlpBody = new System.Windows.Forms.TableLayoutPanel();
            this.LblLogout = new ScopeX.UserControls.ScopeXLabel();
            this.LblRestart = new ScopeX.UserControls.ScopeXLabel();
            this.LblShutDown = new ScopeX.UserControls.ScopeXLabel();
            this.LblClose = new ScopeX.UserControls.ScopeXLabel();
            this.BtnLogout = new ScopeX.UserControls.ScopeXIconButton();
            this.BtnRestart = new ScopeX.UserControls.ScopeXIconButton();
            this.BtnShutDown = new ScopeX.UserControls.ScopeXIconButton();
            this.BtnClose = new ScopeX.UserControls.ScopeXIconButton();
            this.BtnMinimize = new ScopeX.UserControls.ScopeXIconButton();
            this.LblMinimize = new ScopeX.UserControls.ScopeXLabel();
            this.TlpBody.SuspendLayout();
            this.SuspendLayout();
            // 
            // TlpBody
            // 
            this.TlpBody.ColumnCount = 5;
            this.TlpBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TlpBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TlpBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TlpBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TlpBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TlpBody.Controls.Add(this.LblLogout, 4, 1);
            this.TlpBody.Controls.Add(this.LblRestart, 3, 1);
            this.TlpBody.Controls.Add(this.LblShutDown, 2, 1);
            this.TlpBody.Controls.Add(this.LblClose, 1, 1);
            this.TlpBody.Controls.Add(this.BtnLogout, 4, 0);
            this.TlpBody.Controls.Add(this.BtnRestart, 3, 0);
            this.TlpBody.Controls.Add(this.BtnShutDown, 2, 0);
            this.TlpBody.Controls.Add(this.BtnClose, 1, 0);
            this.TlpBody.Controls.Add(this.BtnMinimize, 0, 0);
            this.TlpBody.Controls.Add(this.LblMinimize, 0, 1);
            this.TlpBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpBody.Location = new System.Drawing.Point(0, 0);
            this.TlpBody.Name = "TlpBody";
            this.TlpBody.RowCount = 2;
            this.TlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TlpBody.Size = new System.Drawing.Size(313, 60);
            this.TlpBody.TabIndex = 1;
            // 
            // LblLogout
            // 
            this.LblLogout.BackColor = System.Drawing.Color.Empty;
            this.LblLogout.BorderColor = System.Drawing.Color.Black;
            this.LblLogout.BorderThickness = 0;
            this.LblLogout.CornerRadius = 0;
            this.LblLogout.Dock = System.Windows.Forms.DockStyle.Top;
            this.LblLogout.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblLogout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.LblLogout.HighlightPrompt = defaultHighlightPrompt1;
            this.LblLogout.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblLogout.Location = new System.Drawing.Point(248, 40);
            this.LblLogout.Margin = new System.Windows.Forms.Padding(0);
            this.LblLogout.MultyLineFlag = false;
            this.LblLogout.Name = "LblLogout";
            this.LblLogout.Size = new System.Drawing.Size(65, 16);
            this.LblLogout.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblLogout.StylizeFlag = true;
            this.LblLogout.TabIndex = 9;
            this.LblLogout.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhuXiao");
            this.LblLogout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblLogout.Token = null;
            // 
            // LblRestart
            // 
            this.LblRestart.BackColor = System.Drawing.Color.Empty;
            this.LblRestart.BorderColor = System.Drawing.Color.Black;
            this.LblRestart.BorderThickness = 0;
            this.LblRestart.CornerRadius = 0;
            this.LblRestart.Dock = System.Windows.Forms.DockStyle.Top;
            this.LblRestart.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblRestart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.LblRestart.HighlightPrompt = defaultHighlightPrompt2;
            this.LblRestart.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblRestart.Location = new System.Drawing.Point(186, 40);
            this.LblRestart.Margin = new System.Windows.Forms.Padding(0);
            this.LblRestart.MultyLineFlag = false;
            this.LblRestart.Name = "LblRestart";
            this.LblRestart.Size = new System.Drawing.Size(62, 16);
            this.LblRestart.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblRestart.StylizeFlag = true;
            this.LblRestart.TabIndex = 8;
            this.LblRestart.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhongQi");
            this.LblRestart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblRestart.Token = null;
            // 
            // LblShutDown
            // 
            this.LblShutDown.BackColor = System.Drawing.Color.Empty;
            this.LblShutDown.BorderColor = System.Drawing.Color.Black;
            this.LblShutDown.BorderThickness = 0;
            this.LblShutDown.CornerRadius = 0;
            this.LblShutDown.Dock = System.Windows.Forms.DockStyle.Top;
            this.LblShutDown.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblShutDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.LblShutDown.HighlightPrompt = defaultHighlightPrompt3;
            this.LblShutDown.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblShutDown.Location = new System.Drawing.Point(124, 40);
            this.LblShutDown.Margin = new System.Windows.Forms.Padding(0);
            this.LblShutDown.MultyLineFlag = false;
            this.LblShutDown.Name = "LblShutDown";
            this.LblShutDown.Size = new System.Drawing.Size(62, 16);
            this.LblShutDown.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblShutDown.StylizeFlag = true;
            this.LblShutDown.TabIndex = 7;
            this.LblShutDown.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanJi");
            this.LblShutDown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblShutDown.Token = null;
            // 
            // LblClose
            // 
            this.LblClose.BackColor = System.Drawing.Color.Empty;
            this.LblClose.BorderColor = System.Drawing.Color.Black;
            this.LblClose.BorderThickness = 0;
            this.LblClose.CornerRadius = 0;
            this.LblClose.Dock = System.Windows.Forms.DockStyle.Top;
            this.LblClose.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.LblClose.HighlightPrompt = defaultHighlightPrompt4;
            this.LblClose.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblClose.Location = new System.Drawing.Point(62, 40);
            this.LblClose.Margin = new System.Windows.Forms.Padding(0);
            this.LblClose.MultyLineFlag = false;
            this.LblClose.Name = "LblClose";
            this.LblClose.Size = new System.Drawing.Size(62, 16);
            this.LblClose.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblClose.StylizeFlag = true;
            this.LblClose.TabIndex = 6;
            this.LblClose.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TuiChu");
            this.LblClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblClose.Token = null;
            // 
            // BtnLogout
            // 
            this.BtnLogout.BackColor = System.Drawing.Color.Transparent;
            this.BtnLogout.BorderColor = System.Drawing.Color.Black;
            this.BtnLogout.BorderThickness = 0;
            this.BtnLogout.CornerRadius = 0;
            this.BtnLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnLogout.DaskArray = null;
            this.BtnLogout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnLogout.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnLogout.ForeColor = System.Drawing.Color.Black;
            this.BtnLogout.Height = 40;
            this.BtnLogout.Icon = global::ScopeX.U2.Properties.Resources.Logout;
            this.BtnLogout.IconOffset = 16;
            this.BtnLogout.IconSize = new System.Drawing.Size(28, 28);
            this.BtnLogout.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnLogout.IsIndicatorShow = false;
            this.BtnLogout.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnLogout.Location = new System.Drawing.Point(251, 0);
            this.BtnLogout.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.BtnLogout.MouseinBackColor = System.Drawing.Color.Transparent;
            this.BtnLogout.MouseinBorderColor = System.Drawing.Color.Blue;
            this.BtnLogout.MouseInBorderThickness = 0;
            this.BtnLogout.MouseinForeColor = System.Drawing.Color.Blue;
            this.BtnLogout.MouseinSvgForeColor = System.Drawing.Color.Blue;
            this.BtnLogout.Name = "BtnLogout";
            this.BtnLogout.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnLogout.PressedBorderColor = System.Drawing.Color.Blue;
            this.BtnLogout.PressedBorderThickness = 0;
            this.BtnLogout.PressedForeColor = System.Drawing.Color.Blue;
            this.BtnLogout.PressedSvgForeColor = System.Drawing.Color.Blue;
            this.BtnLogout.Size = new System.Drawing.Size(59, 40);
            this.BtnLogout.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnLogout.StylizeFlag = true;
            this.BtnLogout.SVGForeColor = System.Drawing.Color.Black;
            this.BtnLogout.SVGPath = "";
            this.BtnLogout.TabIndex = 4;
            this.BtnLogout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnLogout.Click += new System.EventHandler(this.BtnLogout_Click);
            // 
            // BtnRestart
            // 
            this.BtnRestart.BackColor = System.Drawing.Color.Transparent;
            this.BtnRestart.BorderColor = System.Drawing.Color.Black;
            this.BtnRestart.BorderThickness = 0;
            this.BtnRestart.CornerRadius = 0;
            this.BtnRestart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnRestart.DaskArray = null;
            this.BtnRestart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnRestart.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnRestart.ForeColor = System.Drawing.Color.Black;
            this.BtnRestart.Height = 40;
            this.BtnRestart.Icon = global::ScopeX.U2.Properties.Resources.Restart;
            this.BtnRestart.IconOffset = 14;
            this.BtnRestart.IconSize = new System.Drawing.Size(28, 28);
            this.BtnRestart.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnRestart.IsIndicatorShow = false;
            this.BtnRestart.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnRestart.Location = new System.Drawing.Point(189, 0);
            this.BtnRestart.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.BtnRestart.MouseinBackColor = System.Drawing.Color.Transparent;
            this.BtnRestart.MouseinBorderColor = System.Drawing.Color.Blue;
            this.BtnRestart.MouseInBorderThickness = 0;
            this.BtnRestart.MouseinForeColor = System.Drawing.Color.Blue;
            this.BtnRestart.MouseinSvgForeColor = System.Drawing.Color.Blue;
            this.BtnRestart.Name = "BtnRestart";
            this.BtnRestart.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnRestart.PressedBorderColor = System.Drawing.Color.Blue;
            this.BtnRestart.PressedBorderThickness = 0;
            this.BtnRestart.PressedForeColor = System.Drawing.Color.Blue;
            this.BtnRestart.PressedSvgForeColor = System.Drawing.Color.Blue;
            this.BtnRestart.Size = new System.Drawing.Size(56, 40);
            this.BtnRestart.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnRestart.StylizeFlag = true;
            this.BtnRestart.SVGForeColor = System.Drawing.Color.Black;
            this.BtnRestart.SVGPath = "";
            this.BtnRestart.TabIndex = 3;
            this.BtnRestart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnRestart.Click += new System.EventHandler(this.BtnRestart_Click);
            // 
            // BtnShutDown
            // 
            this.BtnShutDown.BackColor = System.Drawing.Color.Transparent;
            this.BtnShutDown.BorderColor = System.Drawing.Color.Black;
            this.BtnShutDown.BorderThickness = 0;
            this.BtnShutDown.CornerRadius = 0;
            this.BtnShutDown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnShutDown.DaskArray = null;
            this.BtnShutDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnShutDown.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnShutDown.ForeColor = System.Drawing.Color.Black;
            this.BtnShutDown.Height = 40;
            this.BtnShutDown.Icon = global::ScopeX.U2.Properties.Resources.ShutDown;
            this.BtnShutDown.IconOffset = 14;
            this.BtnShutDown.IconSize = new System.Drawing.Size(28, 28);
            this.BtnShutDown.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnShutDown.IsIndicatorShow = false;
            this.BtnShutDown.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnShutDown.Location = new System.Drawing.Point(127, 0);
            this.BtnShutDown.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.BtnShutDown.MouseinBackColor = System.Drawing.Color.Transparent;
            this.BtnShutDown.MouseinBorderColor = System.Drawing.Color.Blue;
            this.BtnShutDown.MouseInBorderThickness = 0;
            this.BtnShutDown.MouseinForeColor = System.Drawing.Color.Blue;
            this.BtnShutDown.MouseinSvgForeColor = System.Drawing.Color.Blue;
            this.BtnShutDown.Name = "BtnShutDown";
            this.BtnShutDown.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnShutDown.PressedBorderColor = System.Drawing.Color.Blue;
            this.BtnShutDown.PressedBorderThickness = 0;
            this.BtnShutDown.PressedForeColor = System.Drawing.Color.Blue;
            this.BtnShutDown.PressedSvgForeColor = System.Drawing.Color.Blue;
            this.BtnShutDown.Size = new System.Drawing.Size(56, 40);
            this.BtnShutDown.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnShutDown.StylizeFlag = true;
            this.BtnShutDown.SVGForeColor = System.Drawing.Color.Black;
            this.BtnShutDown.SVGPath = "";
            this.BtnShutDown.TabIndex = 2;
            this.BtnShutDown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnShutDown.Click += new System.EventHandler(this.BtnShutDown_Click);
            // 
            // BtnClose
            // 
            this.BtnClose.BackColor = System.Drawing.Color.Transparent;
            this.BtnClose.BorderColor = System.Drawing.Color.Black;
            this.BtnClose.BorderThickness = 0;
            this.BtnClose.CornerRadius = 0;
            this.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnClose.DaskArray = null;
            this.BtnClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnClose.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnClose.ForeColor = System.Drawing.Color.Black;
            this.BtnClose.Height = 40;
            this.BtnClose.Icon = global::ScopeX.U2.Properties.Resources.Close;
            this.BtnClose.IconOffset = 14;
            this.BtnClose.IconSize = new System.Drawing.Size(28, 28);
            this.BtnClose.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnClose.IsIndicatorShow = false;
            this.BtnClose.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnClose.Location = new System.Drawing.Point(65, 0);
            this.BtnClose.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.BtnClose.MouseinBackColor = System.Drawing.Color.Transparent;
            this.BtnClose.MouseinBorderColor = System.Drawing.Color.Blue;
            this.BtnClose.MouseInBorderThickness = 0;
            this.BtnClose.MouseinForeColor = System.Drawing.Color.Blue;
            this.BtnClose.MouseinSvgForeColor = System.Drawing.Color.Blue;
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnClose.PressedBorderColor = System.Drawing.Color.Blue;
            this.BtnClose.PressedBorderThickness = 0;
            this.BtnClose.PressedForeColor = System.Drawing.Color.Blue;
            this.BtnClose.PressedSvgForeColor = System.Drawing.Color.Blue;
            this.BtnClose.Size = new System.Drawing.Size(56, 40);
            this.BtnClose.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnClose.StylizeFlag = true;
            this.BtnClose.SVGForeColor = System.Drawing.Color.Black;
            this.BtnClose.SVGPath = "";
            this.BtnClose.TabIndex = 1;
            this.BtnClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BtnMinimize
            // 
            this.BtnMinimize.BackColor = System.Drawing.Color.Transparent;
            this.BtnMinimize.BorderColor = System.Drawing.Color.Black;
            this.BtnMinimize.BorderThickness = 0;
            this.BtnMinimize.CornerRadius = 0;
            this.BtnMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnMinimize.DaskArray = null;
            this.BtnMinimize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnMinimize.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnMinimize.ForeColor = System.Drawing.Color.Black;
            this.BtnMinimize.Height = 40;
            this.BtnMinimize.Icon = global::ScopeX.U2.Properties.Resources.Minimize;
            this.BtnMinimize.IconOffset = 14;
            this.BtnMinimize.IconSize = new System.Drawing.Size(28, 28);
            this.BtnMinimize.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnMinimize.IsIndicatorShow = false;
            this.BtnMinimize.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnMinimize.Location = new System.Drawing.Point(3, 0);
            this.BtnMinimize.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.BtnMinimize.MouseinBackColor = System.Drawing.Color.Transparent;
            this.BtnMinimize.MouseinBorderColor = System.Drawing.Color.Blue;
            this.BtnMinimize.MouseInBorderThickness = 0;
            this.BtnMinimize.MouseinForeColor = System.Drawing.Color.Blue;
            this.BtnMinimize.MouseinSvgForeColor = System.Drawing.Color.Blue;
            this.BtnMinimize.Name = "BtnMinimize";
            this.BtnMinimize.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnMinimize.PressedBorderColor = System.Drawing.Color.Blue;
            this.BtnMinimize.PressedBorderThickness = 0;
            this.BtnMinimize.PressedForeColor = System.Drawing.Color.Blue;
            this.BtnMinimize.PressedSvgForeColor = System.Drawing.Color.Blue;
            this.BtnMinimize.Size = new System.Drawing.Size(56, 40);
            this.BtnMinimize.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnMinimize.StylizeFlag = true;
            this.BtnMinimize.SVGForeColor = System.Drawing.Color.Black;
            this.BtnMinimize.SVGPath = "";
            this.BtnMinimize.TabIndex = 0;
            this.BtnMinimize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnMinimize.Click += new System.EventHandler(this.BtnMinimize_Click);
            // 
            // LblMinimize
            // 
            this.LblMinimize.BackColor = System.Drawing.Color.Empty;
            this.LblMinimize.BorderColor = System.Drawing.Color.Black;
            this.LblMinimize.BorderThickness = 0;
            this.LblMinimize.CornerRadius = 0;
            this.LblMinimize.Dock = System.Windows.Forms.DockStyle.Top;
            this.LblMinimize.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblMinimize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.LblMinimize.HighlightPrompt = defaultHighlightPrompt5;
            this.LblMinimize.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblMinimize.Location = new System.Drawing.Point(0, 40);
            this.LblMinimize.Margin = new System.Windows.Forms.Padding(0);
            this.LblMinimize.MultyLineFlag = false;
            this.LblMinimize.Name = "LblMinimize";
            this.LblMinimize.Size = new System.Drawing.Size(62, 16);
            this.LblMinimize.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblMinimize.StylizeFlag = true;
            this.LblMinimize.TabIndex = 5;
            this.LblMinimize.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZuiXiaoHua");
            this.LblMinimize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblMinimize.Token = null;
            // 
            // SystemPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TlpBody);
            this.Name = "SystemPage";
            this.Size = new System.Drawing.Size(313, 60);
            this.TlpBody.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel TlpBody;
        private ScopeX.UserControls.ScopeXLabel LblLogout;
        private ScopeX.UserControls.ScopeXLabel LblRestart;
        private ScopeX.UserControls.ScopeXLabel LblShutDown;
        private ScopeX.UserControls.ScopeXLabel LblClose;
        private ScopeX.UserControls.ScopeXIconButton BtnLogout;
        private ScopeX.UserControls.ScopeXIconButton BtnRestart;
        private ScopeX.UserControls.ScopeXIconButton BtnShutDown;
        private ScopeX.UserControls.ScopeXIconButton BtnClose;
        private ScopeX.UserControls.ScopeXIconButton BtnMinimize;
        private ScopeX.UserControls.ScopeXLabel LblMinimize;
    }
}
