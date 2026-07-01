namespace ScopeX.U2
{
    partial class ArticialIntelligenceInfoStrip
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
            LveMain = new ScopeX.UserControls.ScopeXListViewEx();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            TlpMain = new System.Windows.Forms.TableLayoutPanel();
            LblTitle = new ScopeX.UserControls.ScopeXLabel();
            TimerMain = new System.Windows.Forms.Timer(components);
            TlpMain.SuspendLayout();
            SuspendLayout();
            // 
            // LveMain
            // 
            LveMain.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LveMain.BorderStyle = System.Windows.Forms.BorderStyle.None;
            LveMain.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1 });
            LveMain.Dock = System.Windows.Forms.DockStyle.Fill;
            LveMain.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LveMain.ForeColor = System.Drawing.SystemColors.ControlLight;
            LveMain.FullRowSelect = true;
            LveMain.GridLines = true;
            LveMain.GridLinesColor = System.Drawing.Color.Black;
            LveMain.HeaderBackColor = System.Drawing.Color.Gray;
            LveMain.HeaderForeColor = System.Drawing.Color.White;
            LveMain.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            LveMain.IsIndependentWindow = false;
            LveMain.Location = new System.Drawing.Point(0, 40);
            LveMain.Margin = new System.Windows.Forms.Padding(0);
            LveMain.MultiSelect = false;
            LveMain.Name = "LveMain";
            LveMain.OwnerDraw = true;
            LveMain.RowHeight = 23;
            LveMain.Scrollable = false;
            LveMain.ScrollContainer = null;
            LveMain.SelectedRowColor = System.Drawing.Color.Blue;
            LveMain.Size = new System.Drawing.Size(389, 432);
            LveMain.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            LveMain.StylizeFlag = true;
            LveMain.TabIndex = 7;
            LveMain.TabStop = false;
            LveMain.Tag = "PowerQuality";
            LveMain.UseCompatibleStateImageBehavior = false;
            LveMain.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "提示信息";
            columnHeader1.Width = 200;
            // 
            // TlpMain
            // 
            TlpMain.ColumnCount = 1;
            TlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpMain.Controls.Add(LveMain, 0, 1);
            TlpMain.Controls.Add(LblTitle, 0, 0);
            TlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpMain.Location = new System.Drawing.Point(0, 0);
            TlpMain.Margin = new System.Windows.Forms.Padding(0);
            TlpMain.Name = "TlpMain";
            TlpMain.RowCount = 2;
            TlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            TlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpMain.Size = new System.Drawing.Size(389, 472);
            TlpMain.TabIndex = 8;
            // 
            // LblTitle
            // 
            LblTitle.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblTitle.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblTitle.BorderThickness = 0;
            LblTitle.CornerRadius = 0;
            LblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            LblTitle.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblTitle.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblTitle.HighlightPrompt = defaultHighlightPrompt1;
            LblTitle.IsOmittext = true;
            LblTitle.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTitle.Location = new System.Drawing.Point(0, 0);
            LblTitle.Margin = new System.Windows.Forms.Padding(0);
            LblTitle.MultyLineFlag = false;
            LblTitle.Name = "LblTitle";
            LblTitle.Size = new System.Drawing.Size(389, 40);
            LblTitle.StyleFlags = UserControls.Style.StyleFlag.None;
            LblTitle.StylizeFlag = true;
            LblTitle.TabIndex = 35;
            LblTitle.TabStop = false;
            LblTitle.Text = "AI";
            LblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblTitle.Token = null;
            LblTitle.ToolTip = false;
            // 
            // TimerMain
            // 
            TimerMain.Tick += TimerMain_Tick;
            // 
            // ArticialIntelligenceInfoStrip
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(TlpMain);
            Name = "ArticialIntelligenceInfoStrip";
            Size = new System.Drawing.Size(389, 472);
            TlpMain.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXListViewEx LveMain;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.TableLayoutPanel TlpMain;
        private System.Windows.Forms.Timer TimerMain;
        private UserControls.ScopeXLabel LblTitle;
    }
}
