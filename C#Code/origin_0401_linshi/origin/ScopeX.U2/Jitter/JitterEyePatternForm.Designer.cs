namespace ScopeX.U2
{
    partial class JitterEyePatternForm
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
            Plot = new ScottPlot.FormsPlot();
            SuspendLayout();
            // 
            // Plot
            // 
            Plot.BackColor = System.Drawing.Color.Black;
            Plot.Dock = System.Windows.Forms.DockStyle.Fill;
            Plot.Location = new System.Drawing.Point(2, 45);
            Plot.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Plot.Name = "Plot";
            Plot.Size = new System.Drawing.Size(471, 303);
            Plot.TabIndex = 5;
            // 
            // JitterEyePatternForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(475, 350);
            Controls.Add(Plot);
            HelpLabel = "26";
            IconWidth = 26;
            Name = "JitterEyePatternForm";
            StylizeFlag = true;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Jitter_YanTu");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Jitter_YanTu");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ToolIconSize = new System.Drawing.Size(26, 26);
            HelpClick += JitterEyePatternForm_EmbededClick;
            Controls.SetChildIndex(Plot, 0);
            ResumeLayout(false);
        }

        #endregion

        private ScottPlot.FormsPlot Plot;
    }
}