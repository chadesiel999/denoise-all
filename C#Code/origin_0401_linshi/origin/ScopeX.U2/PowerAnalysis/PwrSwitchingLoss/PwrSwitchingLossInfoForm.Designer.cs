
namespace ScopeX.U2
{
    partial class PwrSwitchingLossInfoForm
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
            LvSwitchingLoss = new UserControls.ScopeXListViewEx();
            Parameter = new System.Windows.Forms.ColumnHeader();
            Value = new System.Windows.Forms.ColumnHeader();
            Mean = new System.Windows.Forms.ColumnHeader();
            Max = new System.Windows.Forms.ColumnHeader();
            Min = new System.Windows.Forms.ColumnHeader();
            ScSwitchingLoss = new UserControls.ScopeXScrollContainer();
            TmUpdate = new System.Timers.Timer();
            ScSwitchingLoss.SuspendLayout();
            SuspendLayout();
            // 
            // LvSwitchingLoss
            // 
            LvSwitchingLoss.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LvSwitchingLoss.BorderStyle = System.Windows.Forms.BorderStyle.None;
            LvSwitchingLoss.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { Parameter, Value, Mean, Max, Min });
            LvSwitchingLoss.Dock = System.Windows.Forms.DockStyle.Fill;
            LvSwitchingLoss.Font = new System.Drawing.Font("MiSans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LvSwitchingLoss.ForeColor = System.Drawing.SystemColors.ControlLight;
            LvSwitchingLoss.FullRowSelect = true;
            LvSwitchingLoss.GridLines = true;
            LvSwitchingLoss.GridLinesColor = System.Drawing.Color.Gray;
            LvSwitchingLoss.HeaderBackColor = System.Drawing.Color.Gray;
            LvSwitchingLoss.HeaderForeColor = System.Drawing.Color.White;
            LvSwitchingLoss.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            LvSwitchingLoss.HideSelection = true;
            LvSwitchingLoss.IsIndependentWindow = false;
            LvSwitchingLoss.Location = new System.Drawing.Point(0, 0);
            LvSwitchingLoss.MultiSelect = false;
            LvSwitchingLoss.Name = "LvSwitchingLoss";
            LvSwitchingLoss.OwnerDraw = true;
            LvSwitchingLoss.RowHeight = 23;
            LvSwitchingLoss.ScrollContainer = ScSwitchingLoss;
            LvSwitchingLoss.SelectedRowColor = System.Drawing.Color.Blue;
            LvSwitchingLoss.Size = new System.Drawing.Size(718, 263);
            LvSwitchingLoss.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            LvSwitchingLoss.StylizeFlag = true;
            LvSwitchingLoss.TabIndex = 0;
            LvSwitchingLoss.TabStop = false;
            LvSwitchingLoss.Tag = "SwitchingLoss";
            LvSwitchingLoss.UseCompatibleStateImageBehavior = false;
            LvSwitchingLoss.View = System.Windows.Forms.View.Details;
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
            // ScSwitchingLoss
            // 
            ScSwitchingLoss.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ScSwitchingLoss.Controls.Add(LvSwitchingLoss);
            ScSwitchingLoss.Dock = System.Windows.Forms.DockStyle.Fill;
            ScSwitchingLoss.Location = new System.Drawing.Point(0, 45);
            ScSwitchingLoss.Name = "ScSwitchingLoss";
            ScSwitchingLoss.ScrollControl = LvSwitchingLoss;
            ScSwitchingLoss.ScrollThickness = 6;
            ScSwitchingLoss.Size = new System.Drawing.Size(718, 263);
            ScSwitchingLoss.TabIndex = 0;
            // 
            // TmUpdate
            // 
            TmUpdate.Enabled = true;
            TmUpdate.Interval = 500;
            TmUpdate.Elapsed += TmUpdate_Tick;
            // 
            // PwrSwitchingLossInfoForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CanClose = false;
            ClientSize = new System.Drawing.Size(720, 310);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(ScSwitchingLoss);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            IconWidth = 26;
            IsShowPin = false;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PwrSwitchingLossInfoForm";
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
            HelpClick += PwrSwitchingLossInfoForm_EmbededClick;
            TitleIconClick += PwrSwitchingLossInfoForm_SettingClick;
            Controls.SetChildIndex(ScSwitchingLoss, 0);
            ScSwitchingLoss.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXListViewEx LvSwitchingLoss;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Mean;
        private System.Windows.Forms.ColumnHeader Max;
        private System.Windows.Forms.ColumnHeader Min;
        private System.Timers.Timer TmUpdate;
        private ScopeX.UserControls.ScopeXScrollContainer ScSwitchingLoss;
        private System.Windows.Forms.ColumnHeader Value;
    }
}