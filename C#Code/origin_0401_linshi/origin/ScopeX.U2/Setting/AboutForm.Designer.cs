using System.Windows.Forms;

namespace ScopeX.U2
{
    partial class AboutForm
    {

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt5 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt6 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt7 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt8 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt9 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt10 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt11 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt12 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt13 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt14 = new UserControls.DefaultHighlightPrompt();
            TlpBody = new TableLayoutPanel();
            LblHardWare = new UserControls.ScopeXLabel();
            scopexLabel1 = new UserControls.ScopeXLabel();
            LblOption = new UserControls.ScopeXLabel();
            LblType = new UserControls.ScopeXLabel();
            LblSoftWareVersion = new UserControls.ScopeXLabel();
            LblFireWare = new UserControls.ScopeXLabel();
            LblSN = new UserControls.ScopeXLabel();
            PbLogo = new PictureBox();
            Lbl1 = new UserControls.ScopeXLabel();
            Lbl2 = new UserControls.ScopeXLabel();
            Lbl3 = new UserControls.ScopeXLabel();
            Lbl4 = new UserControls.ScopeXLabel();
            Lbl5 = new UserControls.ScopeXLabel();
            LblDate = new UserControls.ScopeXLabel();
            ScOption = new UserControls.ScopeXScrollContainer();
            LvOption = new UserControls.ScopeXListViewEx();
            BtnInfo = new Button();
            panel1 = new Panel();
            LblOptionUsingTime = new UserControls.ScopeXLabel();
            BtnRemoveLiscense = new UserControls.ScopeXIconButton();
            BtnInstallLicense = new UserControls.ScopeXIconButton();
            TlpBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PbLogo).BeginInit();
            ScOption.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // TlpBody
            // 
            TlpBody.BackColor = System.Drawing.Color.Transparent;
            TlpBody.ColumnCount = 2;
            TlpBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 23.0988045F));
            TlpBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 76.9012F));
            TlpBody.Controls.Add(LblHardWare, 1, 4);
            TlpBody.Controls.Add(scopexLabel1, 0, 4);
            TlpBody.Controls.Add(LblOption, 0, 7);
            TlpBody.Controls.Add(LblType, 1, 1);
            TlpBody.Controls.Add(LblSoftWareVersion, 1, 2);
            TlpBody.Controls.Add(LblFireWare, 1, 3);
            TlpBody.Controls.Add(LblSN, 1, 5);
            TlpBody.Controls.Add(PbLogo, 0, 0);
            TlpBody.Controls.Add(Lbl1, 0, 1);
            TlpBody.Controls.Add(Lbl2, 0, 2);
            TlpBody.Controls.Add(Lbl3, 0, 3);
            TlpBody.Controls.Add(Lbl4, 0, 5);
            TlpBody.Controls.Add(Lbl5, 0, 6);
            TlpBody.Controls.Add(LblDate, 1, 6);
            TlpBody.Controls.Add(ScOption, 0, 8);
            TlpBody.Controls.Add(BtnInfo, 1, 0);
            TlpBody.Controls.Add(panel1, 1, 7);
            TlpBody.Dock = DockStyle.Fill;
            TlpBody.Location = new System.Drawing.Point(3, 46);
            TlpBody.Name = "TlpBody";
            TlpBody.RowCount = 9;
            TlpBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            TlpBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            TlpBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            TlpBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            TlpBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            TlpBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            TlpBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            TlpBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            TlpBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            TlpBody.Size = new System.Drawing.Size(754, 561);
            TlpBody.TabIndex = 5;
            // 
            // LblHardWare
            // 
            LblHardWare.AccessibleRole = AccessibleRole.IpAddress;
            LblHardWare.BackColor = System.Drawing.Color.Empty;
            LblHardWare.BorderColor = System.Drawing.Color.Black;
            LblHardWare.BorderThickness = 0;
            LblHardWare.CornerRadius = 0;
            LblHardWare.Dock = DockStyle.Fill;
            LblHardWare.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblHardWare.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblHardWare.HighlightPrompt = defaultHighlightPrompt1;
            LblHardWare.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblHardWare.Location = new System.Drawing.Point(177, 103);
            LblHardWare.MultyLineFlag = false;
            LblHardWare.Name = "LblHardWare";
            LblHardWare.Size = new System.Drawing.Size(574, 19);
            LblHardWare.StyleFlags = UserControls.Style.StyleFlag.None;
            LblHardWare.StylizeFlag = true;
            LblHardWare.TabIndex = 21;
            LblHardWare.Text = "0.0.0.0";
            LblHardWare.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblHardWare.Token = null;
            // 
            // scopexLabel1
            // 
            scopexLabel1.AccessibleRole = AccessibleRole.IpAddress;
            scopexLabel1.BackColor = System.Drawing.Color.Empty;
            scopexLabel1.BorderColor = System.Drawing.Color.Black;
            scopexLabel1.BorderThickness = 0;
            scopexLabel1.CornerRadius = 0;
            scopexLabel1.Dock = DockStyle.Fill;
            scopexLabel1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            scopexLabel1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            scopexLabel1.HighlightPrompt = defaultHighlightPrompt2;
            scopexLabel1.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            scopexLabel1.Location = new System.Drawing.Point(10, 103);
            scopexLabel1.Margin = new Padding(10, 3, 3, 3);
            scopexLabel1.MultyLineFlag = false;
            scopexLabel1.Name = "scopexLabel1";
            scopexLabel1.Size = new System.Drawing.Size(161, 19);
            scopexLabel1.StyleFlags = UserControls.Style.StyleFlag.None;
            scopexLabel1.StylizeFlag = true;
            scopexLabel1.TabIndex = 20;
            scopexLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            scopexLabel1.Token = null;
            // 
            // LblOption
            // 
            LblOption.AccessibleRole = AccessibleRole.IpAddress;
            LblOption.BackColor = System.Drawing.Color.Empty;
            LblOption.BorderColor = System.Drawing.Color.Black;
            LblOption.BorderThickness = 0;
            LblOption.CornerRadius = 0;
            LblOption.Dock = DockStyle.Fill;
            LblOption.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblOption.ForeColor = System.Drawing.Color.White;
            LblOption.HighlightPrompt = defaultHighlightPrompt3;
            LblOption.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblOption.Location = new System.Drawing.Point(10, 178);
            LblOption.Margin = new Padding(10, 3, 3, 3);
            LblOption.MultyLineFlag = false;
            LblOption.Name = "LblOption";
            LblOption.Size = new System.Drawing.Size(161, 29);
            LblOption.StyleFlags = UserControls.Style.StyleFlag.None;
            LblOption.StylizeFlag = true;
            LblOption.TabIndex = 14;
            LblOption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblOption.Token = null;
            LblOption.DoubleClick += LblOption_DoubleClick;
            // 
            // LblType
            // 
            LblType.AccessibleRole = AccessibleRole.IpAddress;
            LblType.BackColor = System.Drawing.Color.Empty;
            LblType.BorderColor = System.Drawing.Color.Black;
            LblType.BorderThickness = 0;
            LblType.CornerRadius = 0;
            LblType.Dock = DockStyle.Fill;
            LblType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblType.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblType.HighlightPrompt = defaultHighlightPrompt4;
            LblType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblType.Location = new System.Drawing.Point(177, 28);
            LblType.MultyLineFlag = false;
            LblType.Name = "LblType";
            LblType.Size = new System.Drawing.Size(574, 19);
            LblType.StyleFlags = UserControls.Style.StyleFlag.None;
            LblType.StylizeFlag = true;
            LblType.TabIndex = 7;
            LblType.Text = "LblType";
            LblType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblType.Token = null;
            // 
            // LblSoftWareVersion
            // 
            LblSoftWareVersion.AccessibleRole = AccessibleRole.IpAddress;
            LblSoftWareVersion.BackColor = System.Drawing.Color.Empty;
            LblSoftWareVersion.BorderColor = System.Drawing.Color.Black;
            LblSoftWareVersion.BorderThickness = 0;
            LblSoftWareVersion.CornerRadius = 0;
            LblSoftWareVersion.Dock = DockStyle.Fill;
            LblSoftWareVersion.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSoftWareVersion.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblSoftWareVersion.HighlightPrompt = defaultHighlightPrompt5;
            LblSoftWareVersion.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSoftWareVersion.Location = new System.Drawing.Point(177, 53);
            LblSoftWareVersion.MultyLineFlag = false;
            LblSoftWareVersion.Name = "LblSoftWareVersion";
            LblSoftWareVersion.Size = new System.Drawing.Size(574, 19);
            LblSoftWareVersion.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSoftWareVersion.StylizeFlag = true;
            LblSoftWareVersion.TabIndex = 8;
            LblSoftWareVersion.Text = "0.0.0.0";
            LblSoftWareVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSoftWareVersion.Token = null;
            // 
            // LblFireWare
            // 
            LblFireWare.AccessibleRole = AccessibleRole.IpAddress;
            LblFireWare.BackColor = System.Drawing.Color.Empty;
            LblFireWare.BorderColor = System.Drawing.Color.Black;
            LblFireWare.BorderThickness = 0;
            LblFireWare.CornerRadius = 0;
            LblFireWare.Dock = DockStyle.Fill;
            LblFireWare.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblFireWare.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblFireWare.HighlightPrompt = defaultHighlightPrompt6;
            LblFireWare.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblFireWare.Location = new System.Drawing.Point(177, 78);
            LblFireWare.MultyLineFlag = false;
            LblFireWare.Name = "LblFireWare";
            LblFireWare.Size = new System.Drawing.Size(574, 19);
            LblFireWare.StyleFlags = UserControls.Style.StyleFlag.None;
            LblFireWare.StylizeFlag = true;
            LblFireWare.TabIndex = 9;
            LblFireWare.Text = "0.0.0.0";
            LblFireWare.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblFireWare.Token = null;
            // 
            // LblSN
            // 
            LblSN.AccessibleRole = AccessibleRole.IpAddress;
            LblSN.BackColor = System.Drawing.Color.Empty;
            LblSN.BorderColor = System.Drawing.Color.Black;
            LblSN.BorderThickness = 0;
            LblSN.CornerRadius = 0;
            LblSN.Dock = DockStyle.Fill;
            LblSN.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSN.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblSN.HighlightPrompt = defaultHighlightPrompt7;
            LblSN.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSN.Location = new System.Drawing.Point(177, 128);
            LblSN.MultyLineFlag = false;
            LblSN.Name = "LblSN";
            LblSN.Size = new System.Drawing.Size(574, 19);
            LblSN.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSN.StylizeFlag = true;
            LblSN.TabIndex = 10;
            LblSN.Text = "LblSN";
            LblSN.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSN.Token = null;
            // 
            // PbLogo
            // 
            PbLogo.Dock = DockStyle.Fill;
            PbLogo.Location = new System.Drawing.Point(3, 3);
            PbLogo.Name = "PbLogo";
            PbLogo.Size = new System.Drawing.Size(168, 19);
            PbLogo.SizeMode = PictureBoxSizeMode.CenterImage;
            PbLogo.TabIndex = 0;
            PbLogo.TabStop = false;
            // 
            // Lbl1
            // 
            Lbl1.AccessibleRole = AccessibleRole.IpAddress;
            Lbl1.BackColor = System.Drawing.Color.Transparent;
            Lbl1.BorderColor = System.Drawing.Color.Black;
            Lbl1.BorderThickness = 0;
            Lbl1.CornerRadius = 0;
            Lbl1.Dock = DockStyle.Fill;
            Lbl1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Lbl1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            Lbl1.HighlightPrompt = defaultHighlightPrompt8;
            Lbl1.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            Lbl1.Location = new System.Drawing.Point(10, 28);
            Lbl1.Margin = new Padding(10, 3, 3, 3);
            Lbl1.MultyLineFlag = false;
            Lbl1.Name = "Lbl1";
            Lbl1.Size = new System.Drawing.Size(161, 19);
            Lbl1.StyleFlags = UserControls.Style.StyleFlag.None;
            Lbl1.StylizeFlag = true;
            Lbl1.TabIndex = 1;
            Lbl1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            Lbl1.Token = null;
            // 
            // Lbl2
            // 
            Lbl2.AccessibleRole = AccessibleRole.IpAddress;
            Lbl2.BackColor = System.Drawing.Color.Empty;
            Lbl2.BorderColor = System.Drawing.Color.Black;
            Lbl2.BorderThickness = 0;
            Lbl2.CornerRadius = 0;
            Lbl2.Dock = DockStyle.Fill;
            Lbl2.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Lbl2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            Lbl2.HighlightPrompt = defaultHighlightPrompt9;
            Lbl2.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            Lbl2.Location = new System.Drawing.Point(10, 53);
            Lbl2.Margin = new Padding(10, 3, 3, 3);
            Lbl2.MultyLineFlag = false;
            Lbl2.Name = "Lbl2";
            Lbl2.Size = new System.Drawing.Size(161, 19);
            Lbl2.StyleFlags = UserControls.Style.StyleFlag.None;
            Lbl2.StylizeFlag = true;
            Lbl2.TabIndex = 2;
            Lbl2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            Lbl2.Token = null;
            // 
            // Lbl3
            // 
            Lbl3.AccessibleRole = AccessibleRole.IpAddress;
            Lbl3.BackColor = System.Drawing.Color.Empty;
            Lbl3.BorderColor = System.Drawing.Color.Black;
            Lbl3.BorderThickness = 0;
            Lbl3.CornerRadius = 0;
            Lbl3.Dock = DockStyle.Fill;
            Lbl3.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Lbl3.ForeColor = System.Drawing.SystemColors.ButtonFace;
            Lbl3.HighlightPrompt = defaultHighlightPrompt10;
            Lbl3.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            Lbl3.Location = new System.Drawing.Point(10, 78);
            Lbl3.Margin = new Padding(10, 3, 3, 3);
            Lbl3.MultyLineFlag = false;
            Lbl3.Name = "Lbl3";
            Lbl3.Size = new System.Drawing.Size(161, 19);
            Lbl3.StyleFlags = UserControls.Style.StyleFlag.None;
            Lbl3.StylizeFlag = true;
            Lbl3.TabIndex = 3;
            Lbl3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            Lbl3.Token = null;
            // 
            // Lbl4
            // 
            Lbl4.AccessibleRole = AccessibleRole.IpAddress;
            Lbl4.BackColor = System.Drawing.Color.Empty;
            Lbl4.BorderColor = System.Drawing.Color.Black;
            Lbl4.BorderThickness = 0;
            Lbl4.CornerRadius = 0;
            Lbl4.Dock = DockStyle.Fill;
            Lbl4.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Lbl4.ForeColor = System.Drawing.SystemColors.ButtonFace;
            Lbl4.HighlightPrompt = defaultHighlightPrompt11;
            Lbl4.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            Lbl4.Location = new System.Drawing.Point(10, 128);
            Lbl4.Margin = new Padding(10, 3, 3, 3);
            Lbl4.MultyLineFlag = false;
            Lbl4.Name = "Lbl4";
            Lbl4.Size = new System.Drawing.Size(161, 19);
            Lbl4.StyleFlags = UserControls.Style.StyleFlag.None;
            Lbl4.StylizeFlag = true;
            Lbl4.TabIndex = 4;
            Lbl4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            Lbl4.Token = null;
            // 
            // Lbl5
            // 
            Lbl5.AccessibleRole = AccessibleRole.IpAddress;
            Lbl5.BackColor = System.Drawing.Color.Empty;
            Lbl5.BorderColor = System.Drawing.Color.Black;
            Lbl5.BorderThickness = 0;
            Lbl5.CornerRadius = 0;
            Lbl5.Dock = DockStyle.Fill;
            Lbl5.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Lbl5.ForeColor = System.Drawing.SystemColors.ButtonFace;
            Lbl5.HighlightPrompt = defaultHighlightPrompt12;
            Lbl5.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            Lbl5.Location = new System.Drawing.Point(10, 153);
            Lbl5.Margin = new Padding(10, 3, 3, 3);
            Lbl5.MultyLineFlag = false;
            Lbl5.Name = "Lbl5";
            Lbl5.Size = new System.Drawing.Size(161, 19);
            Lbl5.StyleFlags = UserControls.Style.StyleFlag.None;
            Lbl5.StylizeFlag = true;
            Lbl5.TabIndex = 5;
            Lbl5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            Lbl5.Token = null;
            Lbl5.Visible = false;
            // 
            // LblDate
            // 
            LblDate.AccessibleRole = AccessibleRole.IpAddress;
            LblDate.BackColor = System.Drawing.Color.Empty;
            LblDate.BorderColor = System.Drawing.Color.Black;
            LblDate.BorderThickness = 0;
            LblDate.CornerRadius = 0;
            LblDate.Dock = DockStyle.Fill;
            LblDate.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblDate.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblDate.HighlightPrompt = defaultHighlightPrompt13;
            LblDate.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblDate.Location = new System.Drawing.Point(177, 153);
            LblDate.MultyLineFlag = false;
            LblDate.Name = "LblDate";
            LblDate.Size = new System.Drawing.Size(574, 19);
            LblDate.StyleFlags = UserControls.Style.StyleFlag.None;
            LblDate.StylizeFlag = true;
            LblDate.TabIndex = 6;
            LblDate.Text = "LblDate";
            LblDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblDate.Token = null;
            LblDate.Visible = false;
            // 
            // ScOption
            // 
            ScOption.BackColor = System.Drawing.Color.FromArgb(45, 46, 50);
            TlpBody.SetColumnSpan(ScOption, 2);
            ScOption.Controls.Add(LvOption);
            ScOption.Dock = DockStyle.Fill;
            ScOption.Location = new System.Drawing.Point(3, 213);
            ScOption.Name = "ScOption";
            ScOption.ScrollControl = LvOption;
            ScOption.ScrollThickness = 6;
            ScOption.Size = new System.Drawing.Size(748, 345);
            ScOption.TabIndex = 16;
            // 
            // LvOption
            // 
            LvOption.BackColor = System.Drawing.Color.FromArgb(45, 46, 50);
            LvOption.BorderStyle = BorderStyle.None;
            LvOption.Dock = DockStyle.Fill;
            LvOption.ForeColor = System.Drawing.Color.FromArgb(234, 234, 234);
            LvOption.FullRowSelect = true;
            LvOption.GridLinesColor = System.Drawing.Color.Black;
            LvOption.HeaderBackColor = System.Drawing.Color.Red;
            LvOption.HeaderForeColor = System.Drawing.Color.Silver;
            LvOption.IsIndependentWindow = false;
            LvOption.Location = new System.Drawing.Point(0, 0);
            LvOption.Margin = new Padding(10, 3, 10, 10);
            LvOption.Name = "LvOption";
            LvOption.OwnerDraw = true;
            LvOption.RowHeight = 20;
            LvOption.ScrollContainer = ScOption;
            LvOption.SelectedRowColor = System.Drawing.Color.MediumSeaGreen;
            LvOption.Size = new System.Drawing.Size(748, 345);
            LvOption.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            LvOption.StylizeFlag = true;
            LvOption.TabIndex = 15;
            LvOption.UseCompatibleStateImageBehavior = false;
            LvOption.View = View.Details;
            LvOption.SelectedIndexChanged += LvOption_SelectedIndexChanged;
            // 
            // BtnInfo
            // 
            BtnInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            BtnInfo.Location = new System.Drawing.Point(177, 3);
            BtnInfo.Name = "BtnInfo";
            BtnInfo.Size = new System.Drawing.Size(75, 19);
            BtnInfo.TabIndex = 17;
            BtnInfo.UseVisualStyleBackColor = true;
            BtnInfo.Click += BtnInfo_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(LblOptionUsingTime);
            panel1.Controls.Add(BtnRemoveLiscense);
            panel1.Controls.Add(BtnInstallLicense);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(174, 175);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(580, 35);
            panel1.TabIndex = 19;
            // 
            // LblOptionUsingTime
            // 
            LblOptionUsingTime.AccessibleRole = AccessibleRole.IpAddress;
            LblOptionUsingTime.BackColor = System.Drawing.Color.Transparent;
            LblOptionUsingTime.BorderColor = System.Drawing.Color.Black;
            LblOptionUsingTime.BorderThickness = 0;
            LblOptionUsingTime.CornerRadius = 0;
            LblOptionUsingTime.Font = new System.Drawing.Font("MiSans", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblOptionUsingTime.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblOptionUsingTime.HighlightPrompt = defaultHighlightPrompt14;
            LblOptionUsingTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblOptionUsingTime.Location = new System.Drawing.Point(282, 3);
            LblOptionUsingTime.Margin = new Padding(10, 3, 3, 3);
            LblOptionUsingTime.MultyLineFlag = false;
            LblOptionUsingTime.Name = "LblOptionUsingTime";
            LblOptionUsingTime.Size = new System.Drawing.Size(290, 29);
            LblOptionUsingTime.StyleFlags = UserControls.Style.StyleFlag.None;
            LblOptionUsingTime.StylizeFlag = true;
            LblOptionUsingTime.TabIndex = 20;
            LblOptionUsingTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            LblOptionUsingTime.Token = null;
            // 
            // BtnRemoveLiscense
            // 
            BtnRemoveLiscense.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRemoveLiscense.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRemoveLiscense.BorderThickness = 0;
            BtnRemoveLiscense.CornerRadius = 0;
            BtnRemoveLiscense.Cursor = Cursors.Hand;
            BtnRemoveLiscense.DaskArray = null;
            BtnRemoveLiscense.DropKey = Keys.Space;
            BtnRemoveLiscense.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnRemoveLiscense.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnRemoveLiscense.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnRemoveLiscense.Height = 29;
            BtnRemoveLiscense.Icon = null;
            BtnRemoveLiscense.IconOffset = 10;
            BtnRemoveLiscense.IconSize = new System.Drawing.Size(24, 24);
            BtnRemoveLiscense.ImeMode = ImeMode.NoControl;
            BtnRemoveLiscense.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnRemoveLiscense.IsChoosed = false;
            BtnRemoveLiscense.IsIndicatorShow = false;
            BtnRemoveLiscense.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnRemoveLiscense.Location = new System.Drawing.Point(132, 3);
            BtnRemoveLiscense.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRemoveLiscense.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRemoveLiscense.MouseInBorderThickness = 0;
            BtnRemoveLiscense.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnRemoveLiscense.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnRemoveLiscense.Name = "BtnRemoveLiscense";
            BtnRemoveLiscense.PressedBackColor = System.Drawing.Color.Gray;
            BtnRemoveLiscense.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRemoveLiscense.PressedBorderThickness = 0;
            BtnRemoveLiscense.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnRemoveLiscense.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnRemoveLiscense.Size = new System.Drawing.Size(140, 29);
            BtnRemoveLiscense.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnRemoveLiscense.StylizeFlag = true;
            BtnRemoveLiscense.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnRemoveLiscense.SVGPath = "";
            BtnRemoveLiscense.TabIndex = 19;
            BtnRemoveLiscense.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnRemoveLiscense.Visible = false;
            BtnRemoveLiscense.Click += BtnRemoveLiscense_Click;
            // 
            // BtnInstallLicense
            // 
            BtnInstallLicense.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnInstallLicense.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnInstallLicense.BorderThickness = 0;
            BtnInstallLicense.CornerRadius = 0;
            BtnInstallLicense.Cursor = Cursors.Hand;
            BtnInstallLicense.DaskArray = null;
            BtnInstallLicense.DropKey = Keys.Space;
            BtnInstallLicense.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnInstallLicense.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnInstallLicense.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnInstallLicense.Height = 29;
            BtnInstallLicense.Icon = null;
            BtnInstallLicense.IconOffset = 10;
            BtnInstallLicense.IconSize = new System.Drawing.Size(24, 24);
            BtnInstallLicense.ImeMode = ImeMode.NoControl;
            BtnInstallLicense.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnInstallLicense.IsChoosed = false;
            BtnInstallLicense.IsIndicatorShow = false;
            BtnInstallLicense.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnInstallLicense.Location = new System.Drawing.Point(3, 3);
            BtnInstallLicense.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnInstallLicense.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnInstallLicense.MouseInBorderThickness = 0;
            BtnInstallLicense.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnInstallLicense.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnInstallLicense.Name = "BtnInstallLicense";
            BtnInstallLicense.PressedBackColor = System.Drawing.Color.Gray;
            BtnInstallLicense.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnInstallLicense.PressedBorderThickness = 0;
            BtnInstallLicense.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnInstallLicense.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnInstallLicense.Size = new System.Drawing.Size(120, 29);
            BtnInstallLicense.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnInstallLicense.StylizeFlag = true;
            BtnInstallLicense.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnInstallLicense.SVGPath = "";
            BtnInstallLicense.TabIndex = 18;
            BtnInstallLicense.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnInstallLicense.Click += BtnInstallLicense_Click;
            // 
            // AboutForm
            // 
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            CanClose = false;
            ClientSize = new System.Drawing.Size(780, 610);
            Controls.Add(TlpBody);
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            IsShowPin = false;
            Name = "AboutForm";
            TitleIcon = Properties.Resources.Info;
            TitleIconSize = new System.Drawing.Size(32, 32);
            TitleIconSizeMode = PictureBoxSizeMode.Zoom;
            TitleIconWidth = 32;
            Controls.SetChildIndex(TlpBody, 0);
            TlpBody.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PbLogo).EndInit();
            ScOption.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        private TableLayoutPanel TlpBody;
        private ScopeX.UserControls.ScopeXLabel LblType;
        private ScopeX.UserControls.ScopeXLabel LblSoftWareVersion;
        private ScopeX.UserControls.ScopeXLabel LblFireWare;
        private ScopeX.UserControls.ScopeXLabel LblSN;
        private PictureBox PbLogo;
        private ScopeX.UserControls.ScopeXLabel Lbl1;
        private ScopeX.UserControls.ScopeXLabel Lbl2;
        private ScopeX.UserControls.ScopeXLabel Lbl3;
        private ScopeX.UserControls.ScopeXLabel Lbl4;
        private ScopeX.UserControls.ScopeXLabel Lbl5;
        private ScopeX.UserControls.ScopeXLabel LblDate;
        private ScopeX.UserControls.ScopeXListViewEx LvOption;
        private ScopeX.UserControls.ScopeXLabel LblOption;
        private System.ComponentModel.IContainer components;
        private ScopeX.UserControls.ScopeXScrollContainer ScOption;
        private Button BtnInfo;
        private UserControls.ScopeXIconButton BtnInstallLicense;
        private Panel panel1;
        private UserControls.ScopeXIconButton BtnRemoveLiscense;
        private UserControls.ScopeXLabel LblOptionUsingTime;
        private UserControls.ScopeXLabel scopexLabel1;
        private UserControls.ScopeXLabel LblHardWare;
    }
}
