namespace ScopeX.U2
{
    partial class CustomDecodeForm
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
            if (ContentControl.Content is ScopeX.Core.IProtocolView view)
            {
                view.Presenter.TryRemoveView(view);
            }
            ContentControl.Content = null;
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
            ContentControl = new DecodeBaseControl();
            SuspendLayout();
            // 
            // ContentControl
            // 
            ContentControl.BackColor = System.Drawing.Color.Transparent;
            ContentControl.Content = null;
            ContentControl.Dock = System.Windows.Forms.DockStyle.Fill;
            ContentControl.Fill = DecodeBaseControl.FillEnum.AutoSize;
            ContentControl.Location = new System.Drawing.Point(2, 45);
            ContentControl.Margin = new System.Windows.Forms.Padding(0);
            ContentControl.Name = "ContentControl";
            ContentControl.Size = new System.Drawing.Size(595, 420);
            ContentControl.TabIndex = 5;
            // 
            // CustomDecodeForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(595, 460);
            ContentBackColor = System.Drawing.Color.FromArgb(39, 41, 48);
            Controls.Add(ContentControl);
            DoubleBuffered = true;
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            HelpLabel = "16";
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "CustomDecodeForm";
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DecodingParameter");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DecodingParameter");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleLableHeight = 32;
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Load += CustomDecodeForm_Load;
            Controls.SetChildIndex(ContentControl, 0);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.U2.DecodeBaseControl ContentControl;
    }
}