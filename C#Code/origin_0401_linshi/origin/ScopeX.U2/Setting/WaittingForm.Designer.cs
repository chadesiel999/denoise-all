namespace ScopeX.U2
{
    partial class WaittingForm
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
            this.LoadCircle = new ScopeX.U2.UestcLoadingCircle();
            this.SuspendLayout();
            // 
            // LoadCircle
            // 
            this.LoadCircle.Active = false;
            this.LoadCircle.InnerCircleRadius = 50;
            this.LoadCircle.Location = new System.Drawing.Point(900, 480);
            this.LoadCircle.Name = "LoadCircle";
            this.LoadCircle.NumberSpoke = 12;
            this.LoadCircle.OuterCircleRadius = 25;
            this.LoadCircle.RotationSpeedms = 250;
            this.LoadCircle.Size = new System.Drawing.Size(120, 120);
            this.LoadCircle.SpokeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(171)))), ((int)(((byte)(209)))));
            this.LoadCircle.SpokeThickness = 6;
            this.LoadCircle.StylePreset = ScopeX.U2.UestcLoadingCircle.StylePresets.MacOSX;
            this.LoadCircle.TabIndex = 0;
            // 
            // WaittingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.Controls.Add(this.LoadCircle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "WaittingForm";
            this.Opacity = 0.9D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WaittingForm";
            this.ResumeLayout(false);

        }

        #endregion

        private UestcLoadingCircle LoadCircle;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}