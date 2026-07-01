
namespace ScopeX.U2
{
    partial class PwrPSRRInfoForm
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
            LvPSRR = new UserControls.ScopeXListViewEx();
            Parameter = new System.Windows.Forms.ColumnHeader();
            Frequency = new System.Windows.Forms.ColumnHeader();
            Amplitude = new System.Windows.Forms.ColumnHeader();
            PSRR = new System.Windows.Forms.ColumnHeader();
            ScPSRR = new UserControls.ScopeXScrollContainer();
            TmUpdate = new System.Timers.Timer();
            ScPSRR.SuspendLayout();
            SuspendLayout();
            // 
            // LvPSRR
            // 
            LvPSRR.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LvPSRR.BorderStyle = System.Windows.Forms.BorderStyle.None;
            LvPSRR.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { Parameter, Frequency, Amplitude, PSRR });
            LvPSRR.Dock = System.Windows.Forms.DockStyle.Fill;
            LvPSRR.Font = new System.Drawing.Font("MiSans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LvPSRR.ForeColor = System.Drawing.SystemColors.ControlLight;
            LvPSRR.FullRowSelect = true;
            LvPSRR.GridLines = true;
            LvPSRR.GridLinesColor = System.Drawing.Color.Gray;
            LvPSRR.HeaderBackColor = System.Drawing.Color.Gray;
            LvPSRR.HeaderForeColor = System.Drawing.Color.White;
            LvPSRR.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            LvPSRR.HideSelection = true;
            LvPSRR.IsIndependentWindow = false;
            LvPSRR.Location = new System.Drawing.Point(0, 0);
            LvPSRR.MultiSelect = false;
            LvPSRR.Name = "LvPSRR";
            LvPSRR.OwnerDraw = true;
            LvPSRR.RowHeight = 23;
            LvPSRR.ScrollContainer = ScPSRR;
            LvPSRR.Scrollable = true;
            LvPSRR.SelectedRowColor = System.Drawing.Color.Blue;
            LvPSRR.Size = new System.Drawing.Size(530, 312);
            LvPSRR.TabIndex = 0;
            LvPSRR.TabStop = false;
            LvPSRR.Tag = "PowerPSRR";
            LvPSRR.UseCompatibleStateImageBehavior = false;
            LvPSRR.View = System.Windows.Forms.View.Details;
            // 
            // Parameter
            // 
            Parameter.Text = "#";
            Parameter.Width = 50;
            // 
            // Frequency
            // 
            Frequency.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PinLv");
            Frequency.Width = 120;
            // 
            // Amplitude
            // 
            Amplitude.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuDu");
            Amplitude.Width = 120;
            // 
            // Gain
            // 
            PSRR.Text = "PSRR";
            PSRR.Width = 120;
            // 
            // ScPSRR
            // 
            ScPSRR.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ScPSRR.Controls.Add(LvPSRR);
            ScPSRR.Dock = System.Windows.Forms.DockStyle.Fill;
            ScPSRR.Location = new System.Drawing.Point(2, 45);
            ScPSRR.Name = "ScPSRR";
            ScPSRR.ScrollControl = LvPSRR;
            ScPSRR.ScrollThickness = 6;
            ScPSRR.Size = new System.Drawing.Size(530, 312);
            ScPSRR.TabIndex = 0;
            // 
            // TmUpdate
            // 
            TmUpdate.Enabled = true;
            TmUpdate.Interval = 500;
            TmUpdate.Elapsed += TmUpdate_Tick;
            // 
            // PwrPSRRInfoForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            CanClose = false;
            ClientSize = new System.Drawing.Size(534, 359);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(ScPSRR);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            HelpLabel = "25";
            IconWidth = 26;
            IsShowPin = false;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PwrPSRRInfoForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "PSRR";
            Title = "PSRR";
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleIcon = Properties.Resources.MeasureSetting;
            TitleIconSize = new System.Drawing.Size(26, 26);
            TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            TitleIconWidth = 26;
            ToolIconSize = new System.Drawing.Size(26, 26);
            HelpClick += PwrPSRRInfoForm_EmbededClick;
            Controls.SetChildIndex(ScPSRR, 0);
            ScPSRR.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ScopeX.UserControls.ScopeXListViewEx LvPSRR;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Frequency;
        private System.Windows.Forms.ColumnHeader Amplitude;
        private System.Windows.Forms.ColumnHeader PSRR;
        private System.Timers.Timer TmUpdate;
        private ScopeX.UserControls.ScopeXScrollContainer ScPSRR;
    }
}