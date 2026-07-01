
namespace ScopeX.U2
{
    partial class PwrDifferInfoForm
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
            this.LvDiffer = new ScopeX.UserControls.ScopeXListViewEx();
            this.Parameter = new System.Windows.Forms.ColumnHeader();
            this.MaxSlewRate = new System.Windows.Forms.ColumnHeader();
            this.MinSlewRate = new System.Windows.Forms.ColumnHeader();
            this.ScDiffer = new ScopeX.UserControls.ScopeXScrollContainer();
            this.TmUpdate = new System.Timers.Timer();
            this.ScDiffer.SuspendLayout();
            this.SuspendLayout();
            // 
            // LvDiffer
            // 
            this.LvDiffer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LvDiffer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LvDiffer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Parameter,
            this.MaxSlewRate,
            this.MinSlewRate});
            this.LvDiffer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LvDiffer.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.LvDiffer.FullRowSelect = true;
            this.LvDiffer.GridLines = true;
            this.LvDiffer.GridLinesColor = System.Drawing.Color.Gray;
            this.LvDiffer.HeaderBackColor = System.Drawing.Color.Gray;
            this.LvDiffer.HeaderForeColor = System.Drawing.Color.White;
            this.LvDiffer.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LvDiffer.HideSelection = true;
            this.LvDiffer.IsIndependentWindow = false;
            this.LvDiffer.Location = new System.Drawing.Point(0, 0);
            this.LvDiffer.MultiSelect = false;
            this.LvDiffer.Name = "LvDiffer";
            this.LvDiffer.OwnerDraw = true;
            this.LvDiffer.RowHeight = 23;
            this.LvDiffer.Scrollable = false;
            this.LvDiffer.ScrollContainer = this.ScDiffer;
            this.LvDiffer.SelectedRowColor = System.Drawing.Color.Blue;
            this.LvDiffer.Size = new System.Drawing.Size(370, 69);
            this.LvDiffer.TabIndex = 0;
            this.LvDiffer.TabStop = false;
            this.LvDiffer.Tag = "Differ";
            this.LvDiffer.UseCompatibleStateImageBehavior = false;
            this.LvDiffer.View = System.Windows.Forms.View.Details;
            // 
            // Parameter
            // 
            this.Parameter.Text = "";
            this.Parameter.Width = 125;
            // 
            // MaxSlewRate
            // 
            this.MaxSlewRate.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZuiDaZhi");
            this.MaxSlewRate.Width = 125;
            // 
            // MinSlewRate
            // 
            this.MinSlewRate.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZuiXiaoZhi");
            this.MinSlewRate.Width = 125;
            // 
            // ScDiffer
            // 
            this.ScDiffer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.ScDiffer.Controls.Add(this.LvDiffer);
            this.ScDiffer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScDiffer.Location = new System.Drawing.Point(0, 45);
            this.ScDiffer.Name = "ScDiffer";
            this.ScDiffer.ScrollControl = this.LvDiffer;
            this.ScDiffer.ScrollThickness = 6;
            this.ScDiffer.Size = new System.Drawing.Size(376, 75);
            this.ScDiffer.TabIndex = 5;
            // 
            // TmUpdate
            // 
            this.TmUpdate.Enabled = true;
            this.TmUpdate.Interval = 500;
            this.TmUpdate.Elapsed += TmUpdate_Tick;
            // 
            // PwrDifferInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CanClose = false;
            this.ClientSize = new System.Drawing.Size(376, 120);
            this.ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.ControlBox = false;
            this.Controls.Add(this.ScDiffer);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormOpacity = 95;
            this.HeadHeight = 45;
            this.IconWidth = 26;
            this.IsShowPin = false;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PwrDifferInfoForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BoXingWeiFen");
            this.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BoXingWeiFen");
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TitleIcon = global::ScopeX.U2.Properties.Resources.MeasureSetting;
            this.TitleIconSize = new System.Drawing.Size(26, 26);
            this.TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.TitleIconWidth = 26;
            this.TitleLableHeight = 39;
            this.ToolIconSize = new System.Drawing.Size(26, 26);
            this.HelpClick += new System.EventHandler(this.PwrDifferInfoForm_EmbededClick);
            this.TitleIconClick += new System.EventHandler(this.PwrDifferInfoForm_SettingClick);
            this.Controls.SetChildIndex(this.ScDiffer, 0);
            this.ScDiffer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private ScopeX.UserControls.ScopeXListViewEx LvDiffer;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader MaxSlewRate;
        private System.Windows.Forms.ColumnHeader MinSlewRate;
        private System.Timers.Timer TmUpdate;
        private ScopeX.UserControls.ScopeXScrollContainer ScDiffer;
    }
}