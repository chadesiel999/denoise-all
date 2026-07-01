namespace ScopeX.U2
{
    partial class PwrSlewRateInfoForm
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
            components = new System.ComponentModel.Container();
            LvSlewRate = new UserControls.ScopeXListViewEx();
            Parameter = new System.Windows.Forms.ColumnHeader();
            Value = new System.Windows.Forms.ColumnHeader();
            Mean = new System.Windows.Forms.ColumnHeader();
            Max = new System.Windows.Forms.ColumnHeader();
            Min = new System.Windows.Forms.ColumnHeader();
            ScSlewRate = new UserControls.ScopeXScrollContainer();
            TmUpdate = new System.Timers.Timer();
            ScSlewRate.SuspendLayout();
            SuspendLayout();
            // 
            // LvSlewRate
            // 
            LvSlewRate.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LvSlewRate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            LvSlewRate.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { Parameter, Value, Mean, Max, Min });
            LvSlewRate.Dock = System.Windows.Forms.DockStyle.Fill;
            LvSlewRate.Font = new System.Drawing.Font("MiSans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LvSlewRate.ForeColor = System.Drawing.SystemColors.ControlLight;
            LvSlewRate.FullRowSelect = true;
            LvSlewRate.GridLines = true;
            LvSlewRate.GridLinesColor = System.Drawing.Color.Gray;
            LvSlewRate.HeaderBackColor = System.Drawing.Color.Gray;
            LvSlewRate.HeaderForeColor = System.Drawing.Color.White;
            LvSlewRate.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            LvSlewRate.HideSelection = true;
            LvSlewRate.IsIndependentWindow = false;
            LvSlewRate.Location = new System.Drawing.Point(0, 0);
            LvSlewRate.MultiSelect = false;
            LvSlewRate.Name = "LvSlewRate";
            LvSlewRate.OwnerDraw = true;
            LvSlewRate.RowHeight = 23;
            LvSlewRate.ScrollContainer = ScSlewRate;
            LvSlewRate.SelectedRowColor = System.Drawing.Color.Blue;
            LvSlewRate.Size = new System.Drawing.Size(725, 440);
            LvSlewRate.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            LvSlewRate.StylizeFlag = true;
            LvSlewRate.TabIndex = 0;
            LvSlewRate.TabStop = false;
            LvSlewRate.Tag = "SlewRate";
            LvSlewRate.UseCompatibleStateImageBehavior = false;
            LvSlewRate.View = System.Windows.Forms.View.Details;
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
            // ScSlewRate
            // 
            ScSlewRate.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ScSlewRate.Controls.Add(LvSlewRate);
            ScSlewRate.Dock = System.Windows.Forms.DockStyle.Fill;
            ScSlewRate.Location = new System.Drawing.Point(0, 45);
            ScSlewRate.Name = "ScSlewRate";
            ScSlewRate.ScrollControl = LvSlewRate;
            ScSlewRate.ScrollThickness = 6;
            ScSlewRate.Size = new System.Drawing.Size(725, 440);
            ScSlewRate.TabIndex = 0;
            // 
            // TmUpdate
            // 
            TmUpdate.Enabled = true;
            TmUpdate.Interval = 500;
            TmUpdate.Elapsed += TmUpdate_Tick;
            // 
            // PwrSlewRateInfoForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CanClose = false;
            ClientSize = new System.Drawing.Size(730, 450);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(ScSlewRate);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            IconWidth = 26;
            IsShowPin = false;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PwrSlewRateInfoForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleIcon = Properties.Resources.MeasureSetting;
            TitleIconSize = new System.Drawing.Size(26, 26);
            TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            TitleIconWidth = 26;
            ToolIconSize = new System.Drawing.Size(26, 26);
            HelpClick += PwrSlewRateInfoForm_EmbededClick;
            TitleIconClick += PwrSlewRateInfoForm_SettingClick;
            Controls.SetChildIndex(ScSlewRate, 0);
            ScSlewRate.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXListViewEx LvSlewRate;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Mean;
        private System.Windows.Forms.ColumnHeader Max;
        private System.Windows.Forms.ColumnHeader Min;
        private System.Timers.Timer TmUpdate;
        private ScopeX.UserControls.ScopeXScrollContainer ScSlewRate;
        private System.Windows.Forms.ColumnHeader Value;
    }
}