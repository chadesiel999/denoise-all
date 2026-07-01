namespace ScopeX.U2
{
    partial class DateTimeBox
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
            this.Tlp = new System.Windows.Forms.TableLayoutPanel();
            this.TbxSeparator4 = new ScopeX.UserControls.ScopeXTextBox();
            this.TbxSeparator3 = new ScopeX.UserControls.ScopeXTextBox();
            this.TbxSeparator = new ScopeX.UserControls.ScopeXTextBox();
            this.TbxSeparator2 = new ScopeX.UserControls.ScopeXTextBox();
            this.TbxSeparator1 = new ScopeX.UserControls.ScopeXTextBox();
            this.TbxSecond = new ScopeX.U2.TextBoxEx();
            this.TbxMinute = new ScopeX.U2.TextBoxEx();
            this.TbxHour = new ScopeX.U2.TextBoxEx();
            this.TbxDay = new ScopeX.U2.TextBoxEx();
            this.TbxMonth = new ScopeX.U2.TextBoxEx();
            this.TbxYear = new ScopeX.U2.TextBoxEx();
            this.Tlp.SuspendLayout();
            this.SuspendLayout();
            // 
            // Tlp
            // 
            this.Tlp.ColumnCount = 11;
            this.Tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.Tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.Tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.Tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.Tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.Tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.Tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.Tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.Tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.Tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.Tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66666F));
            this.Tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.Tlp.Controls.Add(this.TbxSeparator4, 9, 0);
            this.Tlp.Controls.Add(this.TbxSeparator3, 7, 0);
            this.Tlp.Controls.Add(this.TbxSeparator, 5, 0);
            this.Tlp.Controls.Add(this.TbxSeparator2, 3, 0);
            this.Tlp.Controls.Add(this.TbxSeparator1, 1, 0);
            this.Tlp.Controls.Add(this.TbxSecond, 10, 0);
            this.Tlp.Controls.Add(this.TbxMinute, 8, 0);
            this.Tlp.Controls.Add(this.TbxHour, 6, 0);
            this.Tlp.Controls.Add(this.TbxDay, 4, 0);
            this.Tlp.Controls.Add(this.TbxMonth, 2, 0);
            this.Tlp.Controls.Add(this.TbxYear, 0, 0);
            this.Tlp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tlp.Location = new System.Drawing.Point(0, 0);
            this.Tlp.Name = "Tlp";
            this.Tlp.RowCount = 1;
            this.Tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Tlp.Size = new System.Drawing.Size(304, 30);
            this.Tlp.TabIndex = 0;
            // 
            // TbxSeparator4
            // 
            this.TbxSeparator4.AcceptsTab = false;
            this.TbxSeparator4.AutoSize = false;
            this.TbxSeparator4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxSeparator4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxSeparator4.BorderThickness = 0;
            this.TbxSeparator4.CornerRadius = 0;
            this.TbxSeparator4.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.TbxSeparator4.Enabled = true;
            this.TbxSeparator4.EnbleSelectBorder = true;
            this.TbxSeparator4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TbxSeparator4.ForeColor = System.Drawing.Color.White;
            this.TbxSeparator4.Height = 24;
            this.TbxSeparator4.HideSelection = true;
            this.TbxSeparator4.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.TbxSeparator4.Lines = new string[] {
        ":"};
            this.TbxSeparator4.Location = new System.Drawing.Point(251, 3);
            this.TbxSeparator4.MaxLength = 32767;
            this.TbxSeparator4.Modified = false;
            this.TbxSeparator4.MouseEnterState = false;
            this.TbxSeparator4.Multiline = false;
            this.TbxSeparator4.Name = "TbxSeparator4";
            this.TbxSeparator4.ProcessCmdKeyFunc = null;
            this.TbxSeparator4.ReadOnly = true;
            this.TbxSeparator4.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.TbxSeparator4.SelectedText = "";
            this.TbxSeparator4.SelectionLength = 0;
            this.TbxSeparator4.SelectionStart = 0;
            this.TbxSeparator4.ShortcutsEnabled = true;
            this.TbxSeparator4.Size = new System.Drawing.Size(6, 24);
            this.TbxSeparator4.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.TbxSeparator4.StylizeFlag = true;
            this.TbxSeparator4.TabIndex = 36;
            this.TbxSeparator4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TbxSeparator4.UseSystemPasswordChar = false;
            this.TbxSeparator4.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.TbxSeparator4.WordWrap = true;
            // 
            // TbxSeparator3
            // 
            this.TbxSeparator3.AcceptsTab = false;
            this.TbxSeparator3.AutoSize = false;
            this.TbxSeparator3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxSeparator3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxSeparator3.BorderThickness = 0;
            this.TbxSeparator3.CornerRadius = 0;
            this.TbxSeparator3.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.TbxSeparator3.Enabled = true;
            this.TbxSeparator3.EnbleSelectBorder = true;
            this.TbxSeparator3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TbxSeparator3.ForeColor = System.Drawing.Color.White;
            this.TbxSeparator3.Height = 24;
            this.TbxSeparator3.HideSelection = true;
            this.TbxSeparator3.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.TbxSeparator3.Lines = new string[] {
        ":"};
            this.TbxSeparator3.Location = new System.Drawing.Point(199, 3);
            this.TbxSeparator3.MaxLength = 32767;
            this.TbxSeparator3.Modified = false;
            this.TbxSeparator3.MouseEnterState = false;
            this.TbxSeparator3.Multiline = false;
            this.TbxSeparator3.Name = "TbxSeparator3";
            this.TbxSeparator3.ProcessCmdKeyFunc = null;
            this.TbxSeparator3.ReadOnly = true;
            this.TbxSeparator3.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.TbxSeparator3.SelectedText = "";
            this.TbxSeparator3.SelectionLength = 0;
            this.TbxSeparator3.SelectionStart = 0;
            this.TbxSeparator3.ShortcutsEnabled = true;
            this.TbxSeparator3.Size = new System.Drawing.Size(6, 24);
            this.TbxSeparator3.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.TbxSeparator3.StylizeFlag = true;
            this.TbxSeparator3.TabIndex = 35;
            this.TbxSeparator3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TbxSeparator3.UseSystemPasswordChar = false;
            this.TbxSeparator3.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.TbxSeparator3.WordWrap = true;
            // 
            // TbxSeparator
            // 
            this.TbxSeparator.AcceptsTab = false;
            this.TbxSeparator.AutoSize = false;
            this.TbxSeparator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxSeparator.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxSeparator.BorderThickness = 0;
            this.TbxSeparator.CornerRadius = 0;
            this.TbxSeparator.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.TbxSeparator.Enabled = true;
            this.TbxSeparator.EnbleSelectBorder = true;
            this.TbxSeparator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TbxSeparator.ForeColor = System.Drawing.Color.White;
            this.TbxSeparator.Height = 24;
            this.TbxSeparator.HideSelection = true;
            this.TbxSeparator.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.TbxSeparator.Lines = new string[0];
            this.TbxSeparator.Location = new System.Drawing.Point(147, 3);
            this.TbxSeparator.MaxLength = 32767;
            this.TbxSeparator.Modified = false;
            this.TbxSeparator.MouseEnterState = false;
            this.TbxSeparator.Multiline = false;
            this.TbxSeparator.Name = "TbxSeparator";
            this.TbxSeparator.ProcessCmdKeyFunc = null;
            this.TbxSeparator.ReadOnly = true;
            this.TbxSeparator.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.TbxSeparator.SelectedText = "";
            this.TbxSeparator.SelectionLength = 0;
            this.TbxSeparator.SelectionStart = 0;
            this.TbxSeparator.ShortcutsEnabled = true;
            this.TbxSeparator.Size = new System.Drawing.Size(6, 24);
            this.TbxSeparator.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.TbxSeparator.StylizeFlag = true;
            this.TbxSeparator.TabIndex = 34;
            this.TbxSeparator.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TbxSeparator.UseSystemPasswordChar = false;
            this.TbxSeparator.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.TbxSeparator.WordWrap = true;
            // 
            // TbxSeparator2
            // 
            this.TbxSeparator2.AcceptsTab = false;
            this.TbxSeparator2.AutoSize = false;
            this.TbxSeparator2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxSeparator2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxSeparator2.BorderThickness = 0;
            this.TbxSeparator2.CornerRadius = 0;
            this.TbxSeparator2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.TbxSeparator2.Enabled = true;
            this.TbxSeparator2.EnbleSelectBorder = true;
            this.TbxSeparator2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TbxSeparator2.ForeColor = System.Drawing.Color.White;
            this.TbxSeparator2.Height = 24;
            this.TbxSeparator2.HideSelection = true;
            this.TbxSeparator2.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.TbxSeparator2.Lines = new string[] {
        "-"};
            this.TbxSeparator2.Location = new System.Drawing.Point(95, 3);
            this.TbxSeparator2.MaxLength = 32767;
            this.TbxSeparator2.Modified = false;
            this.TbxSeparator2.MouseEnterState = false;
            this.TbxSeparator2.Multiline = false;
            this.TbxSeparator2.Name = "TbxSeparator2";
            this.TbxSeparator2.ProcessCmdKeyFunc = null;
            this.TbxSeparator2.ReadOnly = true;
            this.TbxSeparator2.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.TbxSeparator2.SelectedText = "";
            this.TbxSeparator2.SelectionLength = 0;
            this.TbxSeparator2.SelectionStart = 0;
            this.TbxSeparator2.ShortcutsEnabled = true;
            this.TbxSeparator2.Size = new System.Drawing.Size(6, 24);
            this.TbxSeparator2.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.TbxSeparator2.StylizeFlag = true;
            this.TbxSeparator2.TabIndex = 33;
            this.TbxSeparator2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TbxSeparator2.UseSystemPasswordChar = false;
            this.TbxSeparator2.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.TbxSeparator2.WordWrap = true;
            // 
            // TbxSeparator1
            // 
            this.TbxSeparator1.AcceptsTab = false;
            this.TbxSeparator1.AutoSize = false;
            this.TbxSeparator1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxSeparator1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxSeparator1.BorderThickness = 0;
            this.TbxSeparator1.CornerRadius = 0;
            this.TbxSeparator1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.TbxSeparator1.Enabled = true;
            this.TbxSeparator1.EnbleSelectBorder = true;
            this.TbxSeparator1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TbxSeparator1.ForeColor = System.Drawing.Color.White;
            this.TbxSeparator1.Height = 24;
            this.TbxSeparator1.HideSelection = true;
            this.TbxSeparator1.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.TbxSeparator1.Lines = new string[] {
        "-"};
            this.TbxSeparator1.Location = new System.Drawing.Point(43, 3);
            this.TbxSeparator1.MaxLength = 32767;
            this.TbxSeparator1.Modified = false;
            this.TbxSeparator1.MouseEnterState = false;
            this.TbxSeparator1.Multiline = false;
            this.TbxSeparator1.Name = "TbxSeparator1";
            this.TbxSeparator1.ProcessCmdKeyFunc = null;
            this.TbxSeparator1.ReadOnly = true;
            this.TbxSeparator1.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.TbxSeparator1.SelectedText = "";
            this.TbxSeparator1.SelectionLength = 0;
            this.TbxSeparator1.SelectionStart = 0;
            this.TbxSeparator1.ShortcutsEnabled = true;
            this.TbxSeparator1.Size = new System.Drawing.Size(6, 24);
            this.TbxSeparator1.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.TbxSeparator1.StylizeFlag = true;
            this.TbxSeparator1.TabIndex = 32;
            this.TbxSeparator1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TbxSeparator1.UseSystemPasswordChar = false;
            this.TbxSeparator1.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.TbxSeparator1.WordWrap = true;
            // 
            // TbxSecond
            // 
            this.TbxSecond.AcceptsTab = false;
            this.TbxSecond.AutoSize = false;
            this.TbxSecond.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxSecond.BorderColor = System.Drawing.Color.Black;
            this.TbxSecond.BorderThickness = 0;
            this.TbxSecond.CornerRadius = 0;
            this.TbxSecond.DecLength = 2;
            this.TbxSecond.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TbxSecond.Enabled = true;
            this.TbxSecond.EnbleSelectBorder = false;
            this.TbxSecond.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TbxSecond.ForeColor = System.Drawing.Color.White;
            this.TbxSecond.Height = 24;
            this.TbxSecond.HideSelection = true;
            this.TbxSecond.InputType = ScopeX.U2.TextInputType.NotControl;
            this.TbxSecond.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.TbxSecond.Lines = new string[0];
            this.TbxSecond.Location = new System.Drawing.Point(263, 3);
            this.TbxSecond.MaxLength = 2;
            this.TbxSecond.MaxValue = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.TbxSecond.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.TbxSecond.Modified = false;
            this.TbxSecond.MouseEnterState = false;
            this.TbxSecond.Multiline = false;
            this.TbxSecond.Name = "TbxSecond";
            this.TbxSecond.ProcessCmdKeyFunc = null;
            this.TbxSecond.ReadOnly = false;
            this.TbxSecond.RegexPattern = "";
            this.TbxSecond.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.TbxSecond.SelectedText = "";
            this.TbxSecond.SelectionLength = 0;
            this.TbxSecond.SelectionStart = 0;
            this.TbxSecond.ShortcutsEnabled = true;
            this.TbxSecond.Size = new System.Drawing.Size(38, 24);
            this.TbxSecond.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.TbxSecond.StylizeFlag = true;
            this.TbxSecond.TabIndex = 24;
            this.TbxSecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TbxSecond.UseSystemPasswordChar = false;
            this.TbxSecond.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.TbxSecond.WordWrap = true;
            this.TbxSecond.TextChanged += new System.EventHandler(this.TbxSecond_TextChanged);
            // 
            // TbxMinute
            // 
            this.TbxMinute.AcceptsTab = false;
            this.TbxMinute.AutoSize = false;
            this.TbxMinute.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxMinute.BorderColor = System.Drawing.Color.Black;
            this.TbxMinute.BorderThickness = 0;
            this.TbxMinute.CornerRadius = 0;
            this.TbxMinute.DecLength = 2;
            this.TbxMinute.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TbxMinute.Enabled = true;
            this.TbxMinute.EnbleSelectBorder = false;
            this.TbxMinute.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TbxMinute.ForeColor = System.Drawing.Color.White;
            this.TbxMinute.Height = 24;
            this.TbxMinute.HideSelection = true;
            this.TbxMinute.InputType = ScopeX.U2.TextInputType.NotControl;
            this.TbxMinute.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.TbxMinute.Lines = new string[0];
            this.TbxMinute.Location = new System.Drawing.Point(211, 3);
            this.TbxMinute.MaxLength = 2;
            this.TbxMinute.MaxValue = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.TbxMinute.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.TbxMinute.Modified = false;
            this.TbxMinute.MouseEnterState = false;
            this.TbxMinute.Multiline = false;
            this.TbxMinute.Name = "TbxMinute";
            this.TbxMinute.ProcessCmdKeyFunc = null;
            this.TbxMinute.ReadOnly = false;
            this.TbxMinute.RegexPattern = "";
            this.TbxMinute.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.TbxMinute.SelectedText = "";
            this.TbxMinute.SelectionLength = 0;
            this.TbxMinute.SelectionStart = 0;
            this.TbxMinute.ShortcutsEnabled = true;
            this.TbxMinute.Size = new System.Drawing.Size(34, 24);
            this.TbxMinute.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.TbxMinute.StylizeFlag = true;
            this.TbxMinute.TabIndex = 23;
            this.TbxMinute.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TbxMinute.UseSystemPasswordChar = false;
            this.TbxMinute.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.TbxMinute.WordWrap = true;
            this.TbxMinute.TextChanged += new System.EventHandler(this.TbxMinute_TextChanged);
            this.TbxMinute.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TbxMinute_KeyPress);
            this.TbxMinute.Leave += new System.EventHandler(this.TbxMinute_Leave);
            // 
            // TbxHour
            // 
            this.TbxHour.AcceptsTab = false;
            this.TbxHour.AutoSize = false;
            this.TbxHour.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxHour.BorderColor = System.Drawing.Color.Black;
            this.TbxHour.BorderThickness = 0;
            this.TbxHour.CornerRadius = 0;
            this.TbxHour.DecLength = 2;
            this.TbxHour.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TbxHour.Enabled = true;
            this.TbxHour.EnbleSelectBorder = false;
            this.TbxHour.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TbxHour.ForeColor = System.Drawing.Color.White;
            this.TbxHour.Height = 24;
            this.TbxHour.HideSelection = true;
            this.TbxHour.InputType = ScopeX.U2.TextInputType.NotControl;
            this.TbxHour.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.TbxHour.Lines = new string[0];
            this.TbxHour.Location = new System.Drawing.Point(159, 3);
            this.TbxHour.MaxLength = 2;
            this.TbxHour.MaxValue = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.TbxHour.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.TbxHour.Modified = false;
            this.TbxHour.MouseEnterState = false;
            this.TbxHour.Multiline = false;
            this.TbxHour.Name = "TbxHour";
            this.TbxHour.ProcessCmdKeyFunc = null;
            this.TbxHour.ReadOnly = false;
            this.TbxHour.RegexPattern = "";
            this.TbxHour.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.TbxHour.SelectedText = "";
            this.TbxHour.SelectionLength = 0;
            this.TbxHour.SelectionStart = 0;
            this.TbxHour.ShortcutsEnabled = true;
            this.TbxHour.Size = new System.Drawing.Size(34, 24);
            this.TbxHour.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.TbxHour.StylizeFlag = true;
            this.TbxHour.TabIndex = 22;
            this.TbxHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TbxHour.UseSystemPasswordChar = false;
            this.TbxHour.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.TbxHour.WordWrap = true;
            this.TbxHour.TextChanged += new System.EventHandler(this.TbxHour_TextChanged);
            this.TbxHour.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TbxHour_KeyPress);
            this.TbxHour.Leave += new System.EventHandler(this.TbxHour_Leave);
            // 
            // TbxDay
            // 
            this.TbxDay.AcceptsTab = false;
            this.TbxDay.AutoSize = false;
            this.TbxDay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxDay.BorderColor = System.Drawing.Color.Black;
            this.TbxDay.BorderThickness = 0;
            this.TbxDay.CornerRadius = 0;
            this.TbxDay.DecLength = 2;
            this.TbxDay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TbxDay.Enabled = true;
            this.TbxDay.EnbleSelectBorder = false;
            this.TbxDay.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TbxDay.ForeColor = System.Drawing.Color.White;
            this.TbxDay.Height = 24;
            this.TbxDay.HideSelection = true;
            this.TbxDay.InputType = ScopeX.U2.TextInputType.NotControl;
            this.TbxDay.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.TbxDay.Lines = new string[0];
            this.TbxDay.Location = new System.Drawing.Point(107, 3);
            this.TbxDay.MaxLength = 2;
            this.TbxDay.MaxValue = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.TbxDay.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.TbxDay.Modified = false;
            this.TbxDay.MouseEnterState = false;
            this.TbxDay.Multiline = false;
            this.TbxDay.Name = "TbxDay";
            this.TbxDay.ProcessCmdKeyFunc = null;
            this.TbxDay.ReadOnly = false;
            this.TbxDay.RegexPattern = "";
            this.TbxDay.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.TbxDay.SelectedText = "";
            this.TbxDay.SelectionLength = 0;
            this.TbxDay.SelectionStart = 0;
            this.TbxDay.ShortcutsEnabled = true;
            this.TbxDay.Size = new System.Drawing.Size(34, 24);
            this.TbxDay.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.TbxDay.StylizeFlag = true;
            this.TbxDay.TabIndex = 21;
            this.TbxDay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TbxDay.UseSystemPasswordChar = false;
            this.TbxDay.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.TbxDay.WordWrap = true;
            this.TbxDay.TextChanged += new System.EventHandler(this.TbxDay_TextChanged);
            this.TbxDay.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TbxDay_KeyPress);
            this.TbxDay.Leave += new System.EventHandler(this.TbxDay_Leave);
            // 
            // TbxMonth
            // 
            this.TbxMonth.AcceptsTab = false;
            this.TbxMonth.AutoSize = false;
            this.TbxMonth.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxMonth.BorderColor = System.Drawing.Color.Black;
            this.TbxMonth.BorderThickness = 0;
            this.TbxMonth.CornerRadius = 0;
            this.TbxMonth.DecLength = 2;
            this.TbxMonth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TbxMonth.Enabled = true;
            this.TbxMonth.EnbleSelectBorder = false;
            this.TbxMonth.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TbxMonth.ForeColor = System.Drawing.Color.White;
            this.TbxMonth.Height = 24;
            this.TbxMonth.HideSelection = true;
            this.TbxMonth.InputType = ScopeX.U2.TextInputType.NotControl;
            this.TbxMonth.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.TbxMonth.Lines = new string[0];
            this.TbxMonth.Location = new System.Drawing.Point(55, 3);
            this.TbxMonth.MaxLength = 2;
            this.TbxMonth.MaxValue = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.TbxMonth.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.TbxMonth.Modified = false;
            this.TbxMonth.MouseEnterState = false;
            this.TbxMonth.Multiline = false;
            this.TbxMonth.Name = "TbxMonth";
            this.TbxMonth.ProcessCmdKeyFunc = null;
            this.TbxMonth.ReadOnly = false;
            this.TbxMonth.RegexPattern = "";
            this.TbxMonth.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.TbxMonth.SelectedText = "";
            this.TbxMonth.SelectionLength = 0;
            this.TbxMonth.SelectionStart = 0;
            this.TbxMonth.ShortcutsEnabled = true;
            this.TbxMonth.Size = new System.Drawing.Size(34, 24);
            this.TbxMonth.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.TbxMonth.StylizeFlag = true;
            this.TbxMonth.TabIndex = 20;
            this.TbxMonth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TbxMonth.UseSystemPasswordChar = false;
            this.TbxMonth.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.TbxMonth.WordWrap = true;
            this.TbxMonth.TextChanged += new System.EventHandler(this.TbxMonth_TextChanged);
            this.TbxMonth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TbxMonth_KeyPress);
            this.TbxMonth.Leave += new System.EventHandler(this.TbxMonth_Leave);
            // 
            // TbxYear
            // 
            this.TbxYear.AcceptsTab = false;
            this.TbxYear.AutoSize = false;
            this.TbxYear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.TbxYear.BorderColor = System.Drawing.Color.Black;
            this.TbxYear.BorderThickness = 0;
            this.TbxYear.CornerRadius = 0;
            this.TbxYear.DecLength = 2;
            this.TbxYear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TbxYear.Enabled = true;
            this.TbxYear.EnbleSelectBorder = false;
            this.TbxYear.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TbxYear.ForeColor = System.Drawing.Color.White;
            this.TbxYear.Height = 24;
            this.TbxYear.HideSelection = true;
            this.TbxYear.InputType = ScopeX.U2.TextInputType.NotControl;
            this.TbxYear.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.TbxYear.Lines = new string[0];
            this.TbxYear.Location = new System.Drawing.Point(3, 3);
            this.TbxYear.MaxLength = 4;
            this.TbxYear.MaxValue = new decimal(new int[] {
            2099,
            0,
            0,
            0});
            this.TbxYear.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.TbxYear.Modified = false;
            this.TbxYear.MouseEnterState = false;
            this.TbxYear.Multiline = false;
            this.TbxYear.Name = "TbxYear";
            this.TbxYear.ProcessCmdKeyFunc = null;
            this.TbxYear.ReadOnly = false;
            this.TbxYear.RegexPattern = "";
            this.TbxYear.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.TbxYear.SelectedText = "";
            this.TbxYear.SelectionLength = 0;
            this.TbxYear.SelectionStart = 0;
            this.TbxYear.ShortcutsEnabled = true;
            this.TbxYear.Size = new System.Drawing.Size(34, 24);
            this.TbxYear.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.TbxYear.StylizeFlag = true;
            this.TbxYear.TabIndex = 0;
            this.TbxYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TbxYear.UseSystemPasswordChar = false;
            this.TbxYear.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.TbxYear.WordWrap = true;
            this.TbxYear.TextChanged += new System.EventHandler(this.TbxYear_TextChanged);
            this.TbxYear.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TbxYear_KeyPress);
            this.TbxYear.Leave += new System.EventHandler(this.TbxYear_Leave);
            // 
            // DateTimeBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.Controls.Add(this.Tlp);
            this.Name = "DateTimeBox";
            this.Size = new System.Drawing.Size(304, 30);
            this.Tlp.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel Tlp;
        private TextBoxEx TbxYear;
        private TextBoxEx TbxMonth;
        private TextBoxEx TbxDay;
        private TextBoxEx TbxSecond;
        private TextBoxEx TbxMinute;
        private TextBoxEx TbxHour;
        private ScopeX.UserControls.ScopeXTextBox TbxSeparator1;
        private ScopeX.UserControls.ScopeXTextBox TbxSeparator4;
        private ScopeX.UserControls.ScopeXTextBox TbxSeparator3;
        private ScopeX.UserControls.ScopeXTextBox TbxSeparator;
        private ScopeX.UserControls.ScopeXTextBox TbxSeparator2;
    }
}
