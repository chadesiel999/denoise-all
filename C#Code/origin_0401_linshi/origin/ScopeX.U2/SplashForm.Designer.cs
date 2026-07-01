namespace ScopeX.U2
{
    partial class SplashForm
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
            LblMessage = new UserControls.ScopeXLabel();
            GifLb = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // LblMessage
            // 
            LblMessage.BackColor = System.Drawing.Color.Empty;
            LblMessage.BorderColor = System.Drawing.Color.Black;
            LblMessage.BorderThickness = 0;
            LblMessage.CornerRadius = 0;
            LblMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            LblMessage.Font = new System.Drawing.Font("MiSans Normal", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblMessage.ForeColor = System.Drawing.Color.White;
            LblMessage.HighlightPrompt = defaultHighlightPrompt1;
            LblMessage.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblMessage.Location = new System.Drawing.Point(0, 340);
            LblMessage.MultyLineFlag = false;
            LblMessage.Name = "LblMessage";
            LblMessage.Size = new System.Drawing.Size(331, 30);
            LblMessage.StyleFlags = UserControls.Style.StyleFlag.None;
            LblMessage.StylizeFlag = false;
            LblMessage.TabIndex = 1;
            LblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblMessage.Token = null;
            // 
            // GifLb
            // 
            GifLb.Dock = System.Windows.Forms.DockStyle.Fill;
            GifLb.Location = new System.Drawing.Point(0, 0);
            GifLb.Name = "GifLb";
            GifLb.Size = new System.Drawing.Size(331, 340);
            GifLb.TabIndex = 2;
            // 
            // SplashForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(331, 370);
            Controls.Add(GifLb);
            Controls.Add(LblMessage);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Margin = new System.Windows.Forms.Padding(4);
            Name = "SplashForm";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXLabel LblMessage;
        private System.Windows.Forms.Label GifLb;
    }
}