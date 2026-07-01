
namespace ScopeX.U2
{
    partial class FileForm
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
            
            NbgFile = new UserControls.NavBarGroup();
            SuspendLayout();
            // 
            // NbgFile
            // 
            NbgFile.AssignHelper = "";
            NbgFile.BackColor = System.Drawing.Color.Transparent;
            NbgFile.CurrentGroupIndex = 0;
            NbgFile.Dock = System.Windows.Forms.DockStyle.Fill;
            
            NbgFile.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            NbgFile.Location = new System.Drawing.Point(2, 45);
            NbgFile.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            NbgFile.Name = "NbgFile";
            NbgFile.NavBarColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NbgFile.NavBarHeight = 40;
            NbgFile.NavForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            NbgFile.NavGroupColor = System.Drawing.Color.FromArgb(41, 42, 45);
            NbgFile.NavGroupHeight = 355;
            NbgFile.Size = new System.Drawing.Size(456, 479);
            NbgFile.SplitColor = System.Drawing.Color.Empty;
            NbgFile.StyleFlags = UserControls.Style.StyleFlag.None;
            NbgFile.StylizeFlag = true;
            NbgFile.TabIndex = 0;
            NbgFile.CurrentGroupIndexChanged += NbgFile_CurrentGroupIndexChanged;
            // 
            // FileForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(460, 526);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(NbgFile);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            HelpLabel = "32";
            IconInterval = 21;
            IconWidth = 26;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FileForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            StylizeFlag = true;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CunChu");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CunChu");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Controls.SetChildIndex(NbgFile, 0);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.NavBarGroup NbgFile;
    }
}