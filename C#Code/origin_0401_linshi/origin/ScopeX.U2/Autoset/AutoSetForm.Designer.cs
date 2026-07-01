
namespace ScopeX.U2
{
    partial class AutoSetForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoSetForm));
            this.UpcWait = new ScopeX.UserControls.Controls.ProcessBars.ScopeXProcessCircle();
            this.SuspendLayout();
            // 
            // UpcWait
            // 
            this.UpcWait.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.UpcWait, "UpcWait");
            this.UpcWait.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(122)))), ((int)(((byte)(222)))));
            this.UpcWait.Name = "UpcWait";
            this.UpcWait.ShowValue = false;
            this.UpcWait.TextVisible = false;
            this.UpcWait.TextWidth = 290;
            this.UpcWait.UseWaitCursor = true;
            // 
            // AutoSetForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ControlBox = false;
            this.Controls.Add(this.UpcWait);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoSetForm";
            this.Opacity = 0.8D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.UseWaitCursor = true;
            this.ResumeLayout(false);

        }

        #endregion

        private ScopeX.UserControls.Controls.ProcessBars.ScopeXProcessCircle UpcWait;
    }
}