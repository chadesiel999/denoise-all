namespace ScopeX.U2
{
    partial class PwrOnOffTimeInfoForm
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
            LvOnOffTime = new UserControls.ScopeXListViewEx();
            Parameter = new System.Windows.Forms.ColumnHeader();
            Value = new System.Windows.Forms.ColumnHeader();
            Mean = new System.Windows.Forms.ColumnHeader();
            Max = new System.Windows.Forms.ColumnHeader();
            Min = new System.Windows.Forms.ColumnHeader();
            ScOnOffTime = new UserControls.ScopeXScrollContainer();
            TmUpdate = new System.Timers.Timer();
            ScOnOffTime.SuspendLayout();
            SuspendLayout();
            // 
            // LvOnOffTime
            // 
            LvOnOffTime.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LvOnOffTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
            LvOnOffTime.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { Parameter, Value, Mean, Max, Min });
            LvOnOffTime.Dock = System.Windows.Forms.DockStyle.Fill;
            LvOnOffTime.Font = new System.Drawing.Font("MiSans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LvOnOffTime.ForeColor = System.Drawing.SystemColors.ControlLight;
            LvOnOffTime.FullRowSelect = true;
            LvOnOffTime.GridLines = true;
            LvOnOffTime.GridLinesColor = System.Drawing.Color.Gray;
            LvOnOffTime.HeaderBackColor = System.Drawing.Color.Gray;
            LvOnOffTime.HeaderForeColor = System.Drawing.Color.White;
            LvOnOffTime.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            LvOnOffTime.HideSelection = true;
            LvOnOffTime.IsIndependentWindow = false;
            LvOnOffTime.Location = new System.Drawing.Point(0, 0);
            LvOnOffTime.MultiSelect = false;
            LvOnOffTime.Name = "LvOnOffTime";
            LvOnOffTime.OwnerDraw = true;
            LvOnOffTime.RowHeight = 23;
            LvOnOffTime.ScrollContainer = ScOnOffTime;
            LvOnOffTime.SelectedRowColor = System.Drawing.Color.Blue;
            LvOnOffTime.Size = new System.Drawing.Size(718, 83);
            LvOnOffTime.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            LvOnOffTime.StylizeFlag = true;
            LvOnOffTime.TabIndex = 0;
            LvOnOffTime.TabStop = false;
            LvOnOffTime.Tag = "OnOffTime";
            LvOnOffTime.UseCompatibleStateImageBehavior = false;
            LvOnOffTime.View = System.Windows.Forms.View.Details;
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
            // ScOnOffTime
            // 
            ScOnOffTime.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ScOnOffTime.Controls.Add(LvOnOffTime);
            ScOnOffTime.Dock = System.Windows.Forms.DockStyle.Fill;
            ScOnOffTime.Location = new System.Drawing.Point(0, 45);
            ScOnOffTime.Name = "ScOnOffTime";
            ScOnOffTime.ScrollControl = LvOnOffTime;
            ScOnOffTime.ScrollThickness = 6;
            ScOnOffTime.Size = new System.Drawing.Size(718, 83);
            ScOnOffTime.TabIndex = 0;
            // 
            // TmUpdate
            // 
            TmUpdate.Enabled = true;
            TmUpdate.Interval = 500;
            TmUpdate.Elapsed += TmUpdate_Tick;
            // 
            // PwrOnOffTimeInfoForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CanClose = false;
            ClientSize = new System.Drawing.Size(720, 130);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(ScOnOffTime);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            IconWidth = 26;
            IsShowPin = false;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PwrOnOffTimeInfoForm";
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
            HelpClick += PwrOnOffTimeInfoForm_EmbededClick;
            TitleIconClick += PwrOnOffTimeInfoForm_SettingClick;
            Controls.SetChildIndex(ScOnOffTime, 0);
            ScOnOffTime.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXListViewEx LvOnOffTime;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Mean;
        private System.Windows.Forms.ColumnHeader Max;
        private System.Windows.Forms.ColumnHeader Min;
        private System.Timers.Timer TmUpdate;
        private ScopeX.UserControls.ScopeXScrollContainer ScOnOffTime;
        private System.Windows.Forms.ColumnHeader Value;
    }
}