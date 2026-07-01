using ScopeX.U2.Properties;

namespace ScopeX.U2
{
    partial class PwrTransientInfoForm
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
            this.components = new System.ComponentModel.Container();
            this.LvTransient = new ScopeX.UserControls.ScopeXListViewEx();
            this.Parameter = new System.Windows.Forms.ColumnHeader();
            this.Value = new System.Windows.Forms.ColumnHeader();
            this.ScTransient = new ScopeX.UserControls.ScopeXScrollContainer();
            this.TmUpdate = new System.Timers.Timer();
            this.ScTransient.SuspendLayout();
            this.SuspendLayout();
            // 
            // LvTransient
            // 
            this.LvTransient.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LvTransient.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LvTransient.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Parameter,
            this.Value});
            this.LvTransient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LvTransient.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.LvTransient.FullRowSelect = true;
            this.LvTransient.GridLines = true;
            this.LvTransient.GridLinesColor = System.Drawing.Color.Gray;
            this.LvTransient.HeaderBackColor = System.Drawing.Color.Gray;
            this.LvTransient.HeaderForeColor = System.Drawing.Color.White;
            this.LvTransient.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LvTransient.IsIndependentWindow = false;
            this.LvTransient.Location = new System.Drawing.Point(0, 0);
            this.LvTransient.MultiSelect = false;
            this.LvTransient.Name = "LvTransient";
            this.LvTransient.OwnerDraw = true;
            this.LvTransient.RowHeight = 23;
            this.LvTransient.ScrollContainer = this.ScTransient;
            this.LvTransient.SelectedRowColor = System.Drawing.Color.Blue;
            this.LvTransient.Size = new System.Drawing.Size(400, 57);
            this.LvTransient.TabIndex = 0;
            this.LvTransient.TabStop = false;
            this.LvTransient.Tag = "Transient";
            this.LvTransient.UseCompatibleStateImageBehavior = false;
            this.LvTransient.View = System.Windows.Forms.View.Details;
            // 
            // Parameter
            // 
            this.Parameter.Text = "";
            this.Parameter.Width = 150;
            // 
            // Value
            // 
            this.Value.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Zhi");
            this.Value.Width = 125;
            // 
            // ScTransient
            // 
            this.ScTransient.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.ScTransient.Controls.Add(this.LvTransient);
            this.ScTransient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScTransient.Location = new System.Drawing.Point(0, 45);
            this.ScTransient.Name = "ScTransient";
            this.ScTransient.ScrollControl = this.LvTransient;
            this.ScTransient.ScrollThickness = 6;
            this.ScTransient.Size = new System.Drawing.Size(406, 63);
            this.ScTransient.TabIndex = 0;
            // 
            // TmUpdate
            // 
            this.TmUpdate.Enabled = true;
            this.TmUpdate.Interval = 500;
            // 
            // PwrTransientInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CanClose = false;
            this.ClientSize = new System.Drawing.Size(406, 108);
            this.ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.ControlBox = false;
            this.Controls.Add(this.ScTransient);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormOpacity = 95;
            this.HeadHeight = 45;
            this.IconWidth = 26;
            this.IsShowPin = false;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PwrTransientInfoForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShunTaiXiangYing");
            this.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShunTaiXiangYing");
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TitleIcon = global::ScopeX.U2.Properties.Resources.MeasureSetting;
            this.TitleIconSize = new System.Drawing.Size(26, 26);
            this.TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.TitleIconWidth = 26;
            this.TitleLableHeight = 45;
            this.ToolIconSize = new System.Drawing.Size(26, 26);
            this.HelpClick += new System.EventHandler(this.PwrTransientInfoForm_EmbededClick);
            this.TitleIconClick += new System.EventHandler(this.PwrTransientInfoForm_SettingClick);
            this.Controls.SetChildIndex(this.ScTransient, 0);
            this.ScTransient.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ScopeX.UserControls.ScopeXListViewEx LvTransient;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Timers.Timer TmUpdate;
        private ScopeX.UserControls.ScopeXScrollContainer ScTransient;
    }
}