
namespace ScopeX.U2
{
    partial class PowerAnalysisForm
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
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new UserControls.DefaultHighlightPrompt();
            PnlGeneral = new System.Windows.Forms.Panel();
            BtnAdd = new UserControls.ScopeXIconButton();
            LblCurrentSrc = new UserControls.ScopeXLabel();
            LblVoltageSrc = new UserControls.ScopeXLabel();
            LblMode = new UserControls.ScopeXLabel();
            CbxCurrentSrc = new ScopeX.UserControls.SelectComboBox();
            CbxVoltageSrc = new ScopeX.UserControls.SelectComboBox();
            CbxMode = new ScopeX.UserControls.SelectComboBox();
            TlpOptions = new System.Windows.Forms.TableLayoutPanel();
            PnlGeneral.SuspendLayout();
            TlpOptions.SuspendLayout();
            SuspendLayout();
            // 
            // PnlGeneral
            // 
            PnlGeneral.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            PnlGeneral.Controls.Add(BtnAdd);
            PnlGeneral.Controls.Add(LblCurrentSrc);
            PnlGeneral.Controls.Add(LblVoltageSrc);
            PnlGeneral.Controls.Add(LblMode);
            PnlGeneral.Controls.Add(CbxCurrentSrc);
            PnlGeneral.Controls.Add(CbxVoltageSrc);
            PnlGeneral.Controls.Add(CbxMode);
            PnlGeneral.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            PnlGeneral.Location = new System.Drawing.Point(0, 0);
            PnlGeneral.Margin = new System.Windows.Forms.Padding(0);
            PnlGeneral.Name = "PnlGeneral";
            PnlGeneral.Size = new System.Drawing.Size(539, 174);
            PnlGeneral.TabIndex = 0;
            // 
            // BtnAdd
            // 
            BtnAdd.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAdd.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAdd.BorderThickness = 0;
            BtnAdd.CornerRadius = 0;
            BtnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnAdd.DaskArray = null;
            BtnAdd.DropKey = System.Windows.Forms.Keys.Space;
            BtnAdd.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnAdd.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            BtnAdd.Height = 30;
            BtnAdd.Icon = null;
            BtnAdd.IconOffset = 10;
            BtnAdd.IconSize = new System.Drawing.Size(24, 24);
            BtnAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnAdd.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnAdd.IsIndicatorShow = false;
            BtnAdd.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnAdd.Location = new System.Drawing.Point(390, 124);
            BtnAdd.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAdd.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAdd.MouseInBorderThickness = 0;
            BtnAdd.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAdd.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.PressedBackColor = System.Drawing.Color.Gray;
            BtnAdd.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAdd.PressedBorderThickness = 0;
            BtnAdd.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAdd.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnAdd.Size = new System.Drawing.Size(106, 30);
            BtnAdd.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnAdd.StylizeFlag = true;
            BtnAdd.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAdd.SVGPath = "";
            BtnAdd.TabIndex = 9;
            BtnAdd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnAdd.Click += BtnAdd_Click;
            // 
            // LblCurrentSrc
            // 
            LblCurrentSrc.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblCurrentSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblCurrentSrc.BorderThickness = 0;
            LblCurrentSrc.CornerRadius = 0;
            LblCurrentSrc.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblCurrentSrc.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblCurrentSrc.HighlightPrompt = defaultHighlightPrompt1;
            LblCurrentSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblCurrentSrc.Location = new System.Drawing.Point(208, 96);
            LblCurrentSrc.MultyLineFlag = false;
            LblCurrentSrc.Name = "LblCurrentSrc";
            LblCurrentSrc.Size = new System.Drawing.Size(90, 18);
            LblCurrentSrc.StyleFlags = UserControls.Style.StyleFlag.None;
            LblCurrentSrc.StylizeFlag = true;
            LblCurrentSrc.TabIndex = 7;
            LblCurrentSrc.TabStop = false;
            LblCurrentSrc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblCurrentSrc.Token = null;
            // 
            // LblVoltageSrc
            // 
            LblVoltageSrc.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblVoltageSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblVoltageSrc.BorderThickness = 0;
            LblVoltageSrc.CornerRadius = 0;
            LblVoltageSrc.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblVoltageSrc.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblVoltageSrc.HighlightPrompt = defaultHighlightPrompt2;
            LblVoltageSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblVoltageSrc.Location = new System.Drawing.Point(28, 96);
            LblVoltageSrc.MultyLineFlag = false;
            LblVoltageSrc.Name = "LblVoltageSrc";
            LblVoltageSrc.Size = new System.Drawing.Size(75, 18);
            LblVoltageSrc.StyleFlags = UserControls.Style.StyleFlag.None;
            LblVoltageSrc.StylizeFlag = true;
            LblVoltageSrc.TabIndex = 5;
            LblVoltageSrc.TabStop = false;
            LblVoltageSrc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblVoltageSrc.Token = null;
            // 
            // LblMode
            // 
            LblMode.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblMode.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblMode.BorderThickness = 0;
            LblMode.CornerRadius = 0;
            LblMode.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblMode.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblMode.HighlightPrompt = defaultHighlightPrompt3;
            LblMode.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblMode.Location = new System.Drawing.Point(27, 15);
            LblMode.MultyLineFlag = false;
            LblMode.Name = "LblMode";
            LblMode.Size = new System.Drawing.Size(99, 18);
            LblMode.StyleFlags = UserControls.Style.StyleFlag.None;
            LblMode.StylizeFlag = true;
            LblMode.TabIndex = 2;
            LblMode.TabStop = false;
            LblMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblMode.Token = null;
            // 
            // CbxCurrentSrc
            // 
            CbxCurrentSrc.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxCurrentSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxCurrentSrc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxCurrentSrc.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxCurrentSrc.ForeColor = System.Drawing.Color.White;
            CbxCurrentSrc.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4"
    };
            CbxCurrentSrc.Location = new System.Drawing.Point(208, 124);
            CbxCurrentSrc.Name = "CbxCurrentSrc";
            CbxCurrentSrc.Size = new System.Drawing.Size(106, 30);
            CbxCurrentSrc.TabIndex = 12;
            // 
            // CbxVoltageSrc
            // 
            CbxVoltageSrc.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxVoltageSrc.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxVoltageSrc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxVoltageSrc.DataSource = null;
            CbxVoltageSrc.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxVoltageSrc.ForeColor = System.Drawing.Color.White;
            CbxVoltageSrc.Items = new string[]
    {
    "C1",
    "C2",
    "C3",
    "C4"
    };
            CbxVoltageSrc.Location = new System.Drawing.Point(27, 124);
            CbxVoltageSrc.Name = "CbxVoltageSrc";
            CbxVoltageSrc.Size = new System.Drawing.Size(106, 30);
            CbxVoltageSrc.TabIndex = 11;
            //CbxVoltageSrc.TextChanged += selectTouch2_TextChanged;
            // 
            // CbxMode
            // 
            CbxMode.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxMode.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxMode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxMode.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxMode.ForeColor = System.Drawing.Color.White;
            CbxMode.Items = null;
            CbxMode.Location = new System.Drawing.Point(27, 43);
            CbxMode.Name = "CbxMode";
            CbxMode.Size = new System.Drawing.Size(180, 30);
            CbxMode.TabIndex = 10;
            // 
            // TlpOptions
            // 
            TlpOptions.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            TlpOptions.ColumnCount = 1;
            TlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpOptions.Controls.Add(PnlGeneral, 0, 0);
            TlpOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpOptions.Location = new System.Drawing.Point(2, 45);
            TlpOptions.Name = "TlpOptions";
            TlpOptions.RowCount = 2;
            TlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpOptions.Size = new System.Drawing.Size(436, 525);
            TlpOptions.TabIndex = 0;
            // 
            // PowerAnalysisForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(539, 680);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(TlpOptions);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            HelpLabel = "25";
            IconInterval = 21;
            IconWidth = 26;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PowerAnalysisForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Controls.SetChildIndex(TlpOptions, 0);
            PnlGeneral.ResumeLayout(false);
            TlpOptions.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel PnlGeneral;
        private ScopeX.UserControls.ScopeXIconButton BtnAdd;
        private ScopeX.UserControls.ScopeXIconButton BtnGuide;
        private ScopeX.UserControls.ScopeXSwitchButton ChkActive;
        private ScopeX.UserControls.ScopeXLabel LblCurrentSrc;
        private ScopeX.UserControls.ScopeXLabel LblVoltageSrc;
        private ScopeX.UserControls.ScopeXLabel LblMode;
        private ScopeX.UserControls.SelectComboBox CbxCurrentSrc;
        private ScopeX.UserControls.SelectComboBox CbxVoltageSrc;
        private ScopeX.UserControls.SelectComboBox CbxMode;
        private ScopeX.UserControls.ScopeXLabel LblActive;
        private System.Windows.Forms.TableLayoutPanel TlpOptions;
    }
}