namespace ScopeX.U2
{
    partial class PwrRDSonInfoForm
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
            this.LvRDSon = new ScopeX.UserControls.ScopeXListViewEx();
            this.Parameter = new System.Windows.Forms.ColumnHeader();
            this.Value = new System.Windows.Forms.ColumnHeader();
            this.Mean = new System.Windows.Forms.ColumnHeader();
            this.Max = new System.Windows.Forms.ColumnHeader();
            this.Min = new System.Windows.Forms.ColumnHeader();
            this.ScRDSon = new ScopeX.UserControls.ScopeXScrollContainer();
            this.TmUpdate = new System.Timers.Timer();
            this.ScRDSon.SuspendLayout();
            this.SuspendLayout();
            // 
            // LvRDSon
            // 
            this.LvRDSon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LvRDSon.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LvRDSon.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Parameter,
            this.Value,
            this.Mean,
            this.Max,
            this.Min});
            this.LvRDSon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LvRDSon.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.LvRDSon.FullRowSelect = true;
            this.LvRDSon.GridLines = true;
            this.LvRDSon.GridLinesColor = System.Drawing.Color.Gray;
            this.LvRDSon.HeaderBackColor = System.Drawing.Color.Gray;
            this.LvRDSon.HeaderForeColor = System.Drawing.Color.White;
            this.LvRDSon.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LvRDSon.HideSelection = true;
            this.LvRDSon.IsIndependentWindow = false;
            this.LvRDSon.Location = new System.Drawing.Point(0, 0);
            this.LvRDSon.MultiSelect = false;
            this.LvRDSon.Name = "LvRdson";
            this.LvRDSon.OwnerDraw = true;
            this.LvRDSon.RowHeight = 23;
            this.LvRDSon.Scrollable = false;
            this.LvRDSon.ScrollContainer = this.ScRDSon;
            this.LvRDSon.SelectedRowColor = System.Drawing.Color.Blue;
            this.LvRDSon.Size = new System.Drawing.Size(272, 105);
            this.LvRDSon.TabIndex = 0;
            this.LvRDSon.TabStop = false;
            this.LvRDSon.Tag = "Rdson";
            this.LvRDSon.UseCompatibleStateImageBehavior = false;
            this.LvRDSon.View = System.Windows.Forms.View.Details;
            // 
            // Parameter
            // 
            Parameter.Text = "";
            Parameter.Width = 220;
            // 
            // Value
            // 
            Value.Width = 120;
            // 
            // Mean
            // 
            Mean.Width = 120;
            // 
            // Max
            // 
            Max.Width = 120;
            // 
            // Min
            // 
            Min.Width = 120;
            // 
            // ScRDSon
            // 
            this.ScRDSon.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            this.ScRDSon.Controls.Add(this.LvRDSon);
            this.ScRDSon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScRDSon.Location = new System.Drawing.Point(0, 45);
            this.ScRDSon.Name = "ScRdson";
            this.ScRDSon.ScrollControl = this.LvRDSon;
            this.ScRDSon.ScrollThickness = 6;
            this.ScRDSon.Size = new System.Drawing.Size(718, 105);
            this.ScRDSon.TabIndex = 5;
            // 
            // TmUpdate
            // 
            this.TmUpdate.Enabled = true;
            this.TmUpdate.Interval = 500;
            this.TmUpdate.Elapsed += TmUpdate_Tick;
            // 
            // PwrRDSonInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CanClose = false;
            this.ClientSize = new System.Drawing.Size(720, 110);
            this.ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.ControlBox = false;
            this.Controls.Add(this.ScRDSon);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormOpacity = 95;
            this.HeadHeight = 45;
            this.IconWidth = 26;
            this.IsShowPin = false;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PwrRDSonInfoForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Rds(on)";
            this.Title = "Rds(on)";
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TitleIcon = global::ScopeX.U2.Properties.Resources.MeasureSetting;
            this.TitleIconSize = new System.Drawing.Size(26, 26);
            this.TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.TitleIconWidth = 26;
            this.TitleLableHeight = 39;
            this.ToolIconSize = new System.Drawing.Size(26, 26);
            this.HelpClick += new System.EventHandler(this.PwrRDSonInfoForm_EmbededClick);
            this.TitleIconClick += new System.EventHandler(this.PwrRDSonInfoForm_SettingClick);
            this.Controls.SetChildIndex(this.ScRDSon, 0);
            this.ScRDSon.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ScopeX.UserControls.ScopeXListViewEx LvRDSon;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.ColumnHeader Mean;
        private System.Windows.Forms.ColumnHeader Max;
        private System.Windows.Forms.ColumnHeader Min;
        private System.Timers.Timer TmUpdate;
        private ScopeX.UserControls.ScopeXScrollContainer ScRDSon;
    }
}