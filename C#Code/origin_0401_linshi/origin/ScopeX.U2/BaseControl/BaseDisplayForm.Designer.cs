
namespace ScopeX.U2
{
    partial class BaseDisplayForm
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
            if(ButtonSource!=null && ButtonSource.Count>0)
            {
                ButtonSource.ForEach(x =>
                {
                    x?.Bitmap?.Dispose();
                });
            }
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
            this.SuspendLayout();
            // 
            // BaseDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ActiveBorderVisiable = false;
            this.BorderThickness = 3;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.HeadHeight = 25;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BaseDisplayForm";
            this.ShowIcon = false;
            this.Text = "BaseWaveForm";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion
    }
}