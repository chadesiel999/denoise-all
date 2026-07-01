namespace ScopeX.U2
{
    partial class SaveDataForm
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
            PgBar = new ProgressBar();
            SuspendLayout();
            // 
            // PgBar
            // 
            PgBar.BorderColor = System.Drawing.Color.Transparent;
            PgBar.BorderWidth = 0;
            PgBar.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            PgBar.DescriptionInfo = "";
            PgBar.Dock = System.Windows.Forms.DockStyle.Fill;
            PgBar.Font = new System.Drawing.Font("MiSans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            PgBar.ForeColor = System.Drawing.Color.Black;
            PgBar.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            PgBar.Location = new System.Drawing.Point(0, 0);
            PgBar.MaxValue = 100;
            PgBar.Name = "PgBar";
            PgBar.Opacity = 90;
            PgBar.Size = new System.Drawing.Size(530, 35);
            PgBar.StyleFlags = UserControls.Style.StyleFlag.None;
            PgBar.StylizeFlag = true;
            PgBar.TabIndex = 0;
            PgBar.UseWaitCursor = true;
            PgBar.Value = 10;
            PgBar.ValueBackgroundColor = System.Drawing.Color.WhiteSmoke;
            PgBar.ValueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            PgBar.ValueTxtType = ValueTextType.Percent;
            // 
            // SaveDataForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(530, 35);
            Controls.Add(PgBar);
            Cursor = System.Windows.Forms.Cursors.WaitCursor;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SaveDataForm";
            Opacity = 0.9D;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            UseWaitCursor = true;
            ResumeLayout(false);
        }

        #endregion

        private ProgressBar PgBar;
    }
}