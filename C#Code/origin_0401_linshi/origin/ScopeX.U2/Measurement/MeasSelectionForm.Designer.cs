
namespace ScopeX.U2
{
    partial class MeasSelectionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MeasSelectionForm));
            TcmParameters = new UserControls.TabControlMenu();
            TpgVertical = new UserControls.TabControlMenuPage();
            TpgHorizontal = new UserControls.TabControlMenuPage();
            TpgOther = new UserControls.TabControlMenuPage();
            PbxSelectedIcon = new System.Windows.Forms.PictureBox();
            LblSelectedName = new UserControls.ScopeXLabel();
            LblSelectedDescription = new UserControls.ScopeXLabel();
            BtnSelect = new UserControls.ScopeXIconButton();
            BtnClose = new UserControls.ScopeXIconButton();
            LblSource = new UserControls.ScopeXLabel();
            CbxSource = new BaseControl.SelectComboxGroup();
            TcmParameters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PbxSelectedIcon).BeginInit();
            SuspendLayout();
            // 
            // TcmParameters
            // 
            TcmParameters.Alignment = System.Windows.Forms.TabAlignment.Left;
            TcmParameters.BackColor = System.Drawing.Color.FromArgb(41, 42, 46);
            TcmParameters.BorderColor = System.Drawing.Color.FromArgb(41, 42, 46);
            TcmParameters.Controls.Add(TpgVertical);
            TcmParameters.Controls.Add(TpgHorizontal);
            TcmParameters.Controls.Add(TpgOther);
            TcmParameters.Dock = System.Windows.Forms.DockStyle.Top;
            TcmParameters.ItemSize = new System.Drawing.Size(45, 90);
            TcmParameters.LanguageKey = null;
            TcmParameters.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TcmParameters.Location = new System.Drawing.Point(3, 46);
            TcmParameters.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            TcmParameters.Multiline = true;
            TcmParameters.Name = "TcmParameters";
            TcmParameters.Padding = new System.Drawing.Point(0, 0);
            TcmParameters.PanelBackColor = System.Drawing.Color.FromArgb(0, 171, 209);
            TcmParameters.PanelNomalBackColor = System.Drawing.Color.FromArgb(38, 38, 46);
            TcmParameters.SelectedIndex = 0;
            TcmParameters.Size = new System.Drawing.Size(1136, 275);
            TcmParameters.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            TcmParameters.TabIndex = 0;
            TcmParameters.TextDirection = UserControls.TabControlMenu.Direction.Vertical;
            // 
            // TpgVertical
            // 
            TpgVertical.BackColor = System.Drawing.Color.FromArgb(50, 50, 54);
            TpgVertical.Dock = System.Windows.Forms.DockStyle.Fill;
            TpgVertical.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TpgVertical.ForeColor = System.Drawing.Color.White;
            TpgVertical.HeaderColor = System.Drawing.Color.Black;
            TpgVertical.HeaderFont = new System.Drawing.Font("MiSans", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TpgVertical.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TpgVertical.Location = new System.Drawing.Point(46, 0);
            TpgVertical.Margin = new System.Windows.Forms.Padding(0);
            TpgVertical.Name = "TpgVertical";
            TpgVertical.Size = new System.Drawing.Size(1090, 274);
            TpgVertical.TabIndex = 1;
            // 
            // TpgHorizontal
            // 
            TpgHorizontal.BackColor = System.Drawing.Color.FromArgb(50, 50, 54);
            TpgHorizontal.Dock = System.Windows.Forms.DockStyle.Fill;
            TpgHorizontal.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TpgHorizontal.ForeColor = System.Drawing.Color.White;
            TpgHorizontal.HeaderColor = System.Drawing.Color.White;
            TpgHorizontal.HeaderFont = new System.Drawing.Font("MiSans", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TpgHorizontal.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TpgHorizontal.Location = new System.Drawing.Point(46, 0);
            TpgHorizontal.Margin = new System.Windows.Forms.Padding(0);
            TpgHorizontal.Name = "TpgHorizontal";
            TpgHorizontal.Size = new System.Drawing.Size(1090, 274);
            TpgHorizontal.TabIndex = 0;
            // 
            // TpgOther
            // 
            TpgOther.BackColor = System.Drawing.Color.FromArgb(50, 50, 54);
            TpgOther.Dock = System.Windows.Forms.DockStyle.Fill;
            TpgOther.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TpgOther.ForeColor = System.Drawing.Color.White;
            TpgOther.HeaderColor = System.Drawing.Color.White;
            TpgOther.HeaderFont = new System.Drawing.Font("MiSans", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TpgOther.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TpgOther.Location = new System.Drawing.Point(46, 0);
            TpgOther.Margin = new System.Windows.Forms.Padding(0);
            TpgOther.Name = "TpgOther";
            TpgOther.Size = new System.Drawing.Size(1090, 274);
            TpgOther.TabIndex = 2;
            // 
            // PbxSelectedIcon
            // 
            PbxSelectedIcon.BackColor = System.Drawing.Color.Transparent;
            PbxSelectedIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            PbxSelectedIcon.Image = Properties.Resources.ImageUtility;
            PbxSelectedIcon.Location = new System.Drawing.Point(47, 348);
            PbxSelectedIcon.Name = "PbxSelectedIcon";
            PbxSelectedIcon.Size = new System.Drawing.Size(226, 149);
            PbxSelectedIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            PbxSelectedIcon.TabIndex = 20;
            PbxSelectedIcon.TabStop = false;
            // 
            // LblSelectedName
            // 
            LblSelectedName.BackColor = System.Drawing.Color.Empty;
            LblSelectedName.BorderColor = System.Drawing.Color.Black;
            LblSelectedName.BorderThickness = 0;
            LblSelectedName.CornerRadius = 0;
            LblSelectedName.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSelectedName.ForeColor = System.Drawing.Color.White;
            LblSelectedName.HighlightPrompt = defaultHighlightPrompt1;
            LblSelectedName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSelectedName.Location = new System.Drawing.Point(279, 348);
            LblSelectedName.MultyLineFlag = false;
            LblSelectedName.Name = "LblSelectedName";
            LblSelectedName.Size = new System.Drawing.Size(200, 23);
            LblSelectedName.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSelectedName.StylizeFlag = true;
            LblSelectedName.TabIndex = 1;
            LblSelectedName.TabStop = false;
            LblSelectedName.Text = "Name";
            LblSelectedName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSelectedName.Token = null;
            // 
            // LblSelectedDescription
            // 
            LblSelectedDescription.BackColor = System.Drawing.Color.Empty;
            LblSelectedDescription.BorderColor = System.Drawing.Color.Black;
            LblSelectedDescription.BorderThickness = 0;
            LblSelectedDescription.CornerRadius = 0;
            LblSelectedDescription.AutoSize = false;
            LblSelectedDescription.Font = new System.Drawing.Font("MiSans", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSelectedDescription.ForeColor = System.Drawing.Color.White;
            LblSelectedDescription.HighlightPrompt = defaultHighlightPrompt2;
            LblSelectedDescription.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblSelectedDescription.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSelectedDescription.Location = new System.Drawing.Point(295, 377);
            LblSelectedDescription.MultyLineFlag = true;
            LblSelectedDescription.Name = "LblSelectedDescription";
            LblSelectedDescription.IsOmittext = false;
            LblSelectedDescription.Size = new System.Drawing.Size(480, 120);
            LblSelectedDescription.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSelectedDescription.StylizeFlag = false;
            LblSelectedDescription.TabIndex = 2;
            LblSelectedDescription.TabStop = false;
            LblSelectedDescription.Text = "Description";
            LblSelectedDescription.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            LblSelectedDescription.Token = null;
            // 
            // BtnSelect
            // 
            BtnSelect.BackColor = System.Drawing.Color.Transparent;
            BtnSelect.BorderColor = System.Drawing.Color.Transparent;
            BtnSelect.BorderThickness = 0;
            BtnSelect.CornerRadius = 0;
            BtnSelect.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSelect.DaskArray = null;
            BtnSelect.DropKey = System.Windows.Forms.Keys.Space;
            BtnSelect.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnSelect.Height = 30;
            BtnSelect.Icon = null;
            BtnSelect.IconOffset = 10;
            BtnSelect.IconSize = new System.Drawing.Size(24, 24);
            BtnSelect.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSelect.IsChoosed = false;
            BtnSelect.IsIndicatorShow = false;
            BtnSelect.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSelect.Location = new System.Drawing.Point(912, 467);
            BtnSelect.MouseinBackColor = System.Drawing.Color.Green;
            BtnSelect.MouseinBorderColor = System.Drawing.Color.Transparent;
            BtnSelect.MouseInBorderThickness = 1;
            BtnSelect.MouseinForeColor = System.Drawing.Color.White;
            BtnSelect.MouseinSvgForeColor = System.Drawing.Color.Transparent;
            BtnSelect.Name = "BtnSelect";
            BtnSelect.PressedBackColor = System.Drawing.Color.FromArgb(0, 192, 0);
            BtnSelect.PressedBorderColor = System.Drawing.Color.Transparent;
            BtnSelect.PressedBorderThickness = 0;
            BtnSelect.PressedForeColor = System.Drawing.Color.White;
            BtnSelect.PressedSvgForeColor = System.Drawing.Color.White;
            BtnSelect.Size = new System.Drawing.Size(85, 30);
            BtnSelect.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnSelect.StylizeFlag = true;
            BtnSelect.SVGForeColor = System.Drawing.Color.Black;
            BtnSelect.SVGPath = "";
            BtnSelect.TabIndex = 5;
            BtnSelect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSelect.Click += BtnItem_Selected;
            // 
            // BtnClose
            // 
            BtnClose.BackColor = System.Drawing.Color.Transparent;
            BtnClose.BorderColor = System.Drawing.Color.Black;
            BtnClose.BorderThickness = 0;
            BtnClose.CornerRadius = 0;
            BtnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnClose.DaskArray = null;
            BtnClose.DropKey = System.Windows.Forms.Keys.Space;
            BtnClose.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnClose.Height = 30;
            BtnClose.Icon = null;
            BtnClose.IconOffset = 10;
            BtnClose.IconSize = new System.Drawing.Size(24, 24);
            BtnClose.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnClose.IsChoosed = false;
            BtnClose.IsIndicatorShow = false;
            BtnClose.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnClose.Location = new System.Drawing.Point(1045, 467);
            BtnClose.MouseinBackColor = System.Drawing.Color.FromArgb(51, 52, 56);
            BtnClose.MouseinBorderColor = System.Drawing.Color.Transparent;
            BtnClose.MouseInBorderThickness = 1;
            BtnClose.MouseinForeColor = System.Drawing.Color.White;
            BtnClose.MouseinSvgForeColor = System.Drawing.Color.White;
            BtnClose.Name = "BtnClose";
            BtnClose.PressedBackColor = System.Drawing.Color.Gray;
            BtnClose.PressedBorderColor = System.Drawing.Color.Transparent;
            BtnClose.PressedBorderThickness = 0;
            BtnClose.PressedForeColor = System.Drawing.Color.White;
            BtnClose.PressedSvgForeColor = System.Drawing.Color.Blue;
            BtnClose.Size = new System.Drawing.Size(85, 30);
            BtnClose.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnClose.StylizeFlag = true;
            BtnClose.SVGForeColor = System.Drawing.Color.Black;
            BtnClose.SVGPath = "";
            BtnClose.TabIndex = 6;
            BtnClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnClose.Click += BtnClose_Click;
            // 
            // LblSource
            // 
            LblSource.BackColor = System.Drawing.Color.Empty;
            LblSource.BorderColor = System.Drawing.Color.Black;
            LblSource.BorderThickness = 0;
            LblSource.CornerRadius = 0;
            LblSource.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSource.ForeColor = System.Drawing.Color.White;
            LblSource.HighlightPrompt = defaultHighlightPrompt3;
            LblSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSource.Location = new System.Drawing.Point(802, 348);
            LblSource.MultyLineFlag = false;
            LblSource.Name = "LblSource";
            LblSource.Size = new System.Drawing.Size(200, 23);
            LblSource.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSource.StylizeFlag = true;
            LblSource.TabIndex = 3;
            LblSource.TabStop = false;
            LblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSource.Token = null;
            // 
            // CbxSource
            // 
            CbxSource.AutoSize = true;
            CbxSource.BackColor = System.Drawing.Color.DimGray;
            CbxSource.CbbItem = new string[] { "" };
            CbxSource.Location = new System.Drawing.Point(802, 377);
            CbxSource.Margin = new System.Windows.Forms.Padding(0);
            CbxSource.MaximumSize = new System.Drawing.Size(361, 30);
            CbxSource.MinimumSize = new System.Drawing.Size(361, 30);
            CbxSource.Name = "CbxSource";
            CbxSource.MainItem = new string[] { "C1", "C2", "C3", "C4" };
            CbxSource.SelectedIndex = 0;
            CbxSource.Size = new System.Drawing.Size(361, 30);
            CbxSource.TabIndex = 29;
            CbxSource.Tag = "";
            // 
            // MeasSelectionForm
            // 
            ActiveBorderColor = System.Drawing.Color.FromArgb(50, 55, 65);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            CanClose = false;
            ClientSize = new System.Drawing.Size(1215, 517);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(CbxSource);
            Controls.Add(LblSource);
            Controls.Add(BtnClose);
            Controls.Add(BtnSelect);
            Controls.Add(LblSelectedDescription);
            Controls.Add(LblSelectedName);
            Controls.Add(PbxSelectedIcon);
            Controls.Add(TcmParameters);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            HelpLabel = "18";
            IconWidth = 26;
            IsShowPin = false;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MeasSelectionForm";
            Padding = new System.Windows.Forms.Padding(1);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            TitleColor = System.Drawing.Color.White;
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleIcon = Properties.Resources.MeasureCustom;
            TitleIconSize = new System.Drawing.Size(26, 26);
            TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            TitleIconWidth = 26;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Controls.SetChildIndex(TcmParameters, 0);
            Controls.SetChildIndex(PbxSelectedIcon, 0);
            Controls.SetChildIndex(LblSelectedName, 0);
            Controls.SetChildIndex(LblSelectedDescription, 0);
            Controls.SetChildIndex(BtnSelect, 0);
            Controls.SetChildIndex(BtnClose, 0);
            Controls.SetChildIndex(LblSource, 0);
            Controls.SetChildIndex(CbxSource, 0);
            TcmParameters.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PbxSelectedIcon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ScopeX.UserControls.TabControlMenu TcmParameters;
        private ScopeX.UserControls.TabControlMenuPage TpgHorizontal;
        private ScopeX.UserControls.TabControlMenuPage TpgVertical;
        private ScopeX.UserControls.TabControlMenuPage TpgOther;
        private System.Windows.Forms.PictureBox PbxSelectedIcon;
        private ScopeX.UserControls.ScopeXLabel LblSelectedName;
        private ScopeX.UserControls.ScopeXLabel LblSelectedDescription;
        private ScopeX.UserControls.ScopeXIconButton BtnSelect;
        private ScopeX.UserControls.ScopeXIconButton BtnClose;
        private ScopeX.UserControls.ScopeXLabel LblSource;
        private BaseControl.SelectComboxGroup CbxSource;
    }
}