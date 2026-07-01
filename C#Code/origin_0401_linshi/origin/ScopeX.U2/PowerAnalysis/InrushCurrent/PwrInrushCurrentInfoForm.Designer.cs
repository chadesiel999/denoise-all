namespace ScopeX.U2
{
    partial class PwrInrushCurrentInfoForm
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
            this.LvInrushCurrent = new ScopeX.UserControls.ScopeXListViewEx();
            this.Parameter = new System.Windows.Forms.ColumnHeader();
            this.Value = new System.Windows.Forms.ColumnHeader();
            this.Mean = new System.Windows.Forms.ColumnHeader();
            this.Max = new System.Windows.Forms.ColumnHeader();
            this.Min = new System.Windows.Forms.ColumnHeader();
            this.ScInrushCurrent = new ScopeX.UserControls.ScopeXScrollContainer();
            this.TmUpdate = new System.Timers.Timer();
            this.ScInrushCurrent.SuspendLayout();
            this.SuspendLayout();
            // 
            // LvInrushCurrent
            // 
            this.LvInrushCurrent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LvInrushCurrent.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LvInrushCurrent.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Parameter,
            this.Value,
            this.Mean,
            this.Max,
            this.Min});
            this.LvInrushCurrent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LvInrushCurrent.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.LvInrushCurrent.FullRowSelect = true;
            this.LvInrushCurrent.GridLines = true;
            this.LvInrushCurrent.GridLinesColor = System.Drawing.Color.Gray;
            this.LvInrushCurrent.HeaderBackColor = System.Drawing.Color.Gray;
            this.LvInrushCurrent.HeaderForeColor = System.Drawing.Color.White;
            this.LvInrushCurrent.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LvInrushCurrent.HideSelection = true;
            this.LvInrushCurrent.IsIndependentWindow = false;
            this.LvInrushCurrent.Location = new System.Drawing.Point(0, 0);
            this.LvInrushCurrent.MultiSelect = false;
            this.LvInrushCurrent.Name = "LvInrushCurrent";
            this.LvInrushCurrent.OwnerDraw = true;
            this.LvInrushCurrent.RowHeight = 23;
            this.LvInrushCurrent.Scrollable = false;
            this.LvInrushCurrent.ScrollContainer = this.ScInrushCurrent;
            this.LvInrushCurrent.SelectedRowColor = System.Drawing.Color.Blue;
            this.LvInrushCurrent.Size = new System.Drawing.Size(272, 67);
            this.LvInrushCurrent.TabIndex = 0;
            this.LvInrushCurrent.TabStop = false;
            this.LvInrushCurrent.Tag = "InrushCurrent";
            this.LvInrushCurrent.UseCompatibleStateImageBehavior = false;
            this.LvInrushCurrent.View = System.Windows.Forms.View.Details;
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
            // ScInrushCurrent
            // 
            this.ScInrushCurrent.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            this.ScInrushCurrent.Controls.Add(this.LvInrushCurrent);
            this.ScInrushCurrent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScInrushCurrent.Location = new System.Drawing.Point(0, 45);
            this.ScInrushCurrent.Name = "ScInrushCurrent";
            this.ScInrushCurrent.ScrollControl = this.LvInrushCurrent;
            this.ScInrushCurrent.ScrollThickness = 6;
            this.ScInrushCurrent.Size = new System.Drawing.Size(718, 163);
            this.ScInrushCurrent.TabIndex = 5;
            // 
            // TmUpdate
            // 
            this.TmUpdate.Enabled = true;
            this.TmUpdate.Interval = 500;
            this.TmUpdate.Elapsed += TmUpdate_Tick;
            // 
            // PwrInrushCurrentInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CanClose = false;
            this.ClientSize = new System.Drawing.Size(720, 210);
            this.ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.ControlBox = false;
            this.Controls.Add(this.ScInrushCurrent);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormOpacity = 95;
            this.HeadHeight = 45;
            this.IconWidth = 26;
            this.IsShowPin = false;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PwrInrushCurrentInfoForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LangYongDianLiu");
            this.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LangYongDianLiu");
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TitleIcon = global::ScopeX.U2.Properties.Resources.MeasureSetting;
            this.TitleIconSize = new System.Drawing.Size(26, 26);
            this.TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.TitleIconWidth = 26;
            this.TitleLableHeight = 39;
            this.ToolIconSize = new System.Drawing.Size(26, 26);
            this.HelpClick += new System.EventHandler(this.PwrInrushCurrentInfoForm_EmbededClick);
            this.TitleIconClick += new System.EventHandler(this.PwrInrushCurrentInfoForm_SettingClick);
            this.Controls.SetChildIndex(this.ScInrushCurrent, 0);
            this.ScInrushCurrent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ScopeX.UserControls.ScopeXListViewEx LvInrushCurrent;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.ColumnHeader Mean;
        private System.Windows.Forms.ColumnHeader Max;
        private System.Windows.Forms.ColumnHeader Min;
        private System.Timers.Timer TmUpdate;
        private ScopeX.UserControls.ScopeXScrollContainer ScInrushCurrent;
    }
}