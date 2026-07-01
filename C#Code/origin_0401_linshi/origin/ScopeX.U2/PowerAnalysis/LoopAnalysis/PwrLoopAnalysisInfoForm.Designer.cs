
namespace ScopeX.U2
{
    partial class PwrLoopAnalysisInfoForm
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
            LvLoopAnalysis = new UserControls.ScopeXListViewEx();
            Parameter = new System.Windows.Forms.ColumnHeader();
            Frequency = new System.Windows.Forms.ColumnHeader();
            Amplitude = new System.Windows.Forms.ColumnHeader();
            Gain = new System.Windows.Forms.ColumnHeader();
            Phase = new System.Windows.Forms.ColumnHeader();
            ScLoopAnalysis = new UserControls.ScopeXScrollContainer();
            TmUpdate = new System.Timers.Timer();
            ScLoopAnalysis.SuspendLayout();
            SuspendLayout();
            // 
            // LvLoopAnalysis
            // 
            LvLoopAnalysis.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LvLoopAnalysis.BorderStyle = System.Windows.Forms.BorderStyle.None;
            LvLoopAnalysis.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { Parameter, Frequency, Amplitude, Gain, Phase });
            LvLoopAnalysis.Font = new System.Drawing.Font("MiSans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LvLoopAnalysis.ForeColor = System.Drawing.SystemColors.ControlLight;
            LvLoopAnalysis.FullRowSelect = true;
            LvLoopAnalysis.GridLines = true;
            LvLoopAnalysis.GridLinesColor = System.Drawing.Color.Gray;
            LvLoopAnalysis.HeaderBackColor = System.Drawing.Color.Gray;
            LvLoopAnalysis.HeaderForeColor = System.Drawing.Color.White;
            LvLoopAnalysis.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            LvLoopAnalysis.HideSelection = true;
            LvLoopAnalysis.IsIndependentWindow = false;
            LvLoopAnalysis.Location = new System.Drawing.Point(0, 0);
            LvLoopAnalysis.MultiSelect = false;
            LvLoopAnalysis.Name = "LvLoopAnalysis";
            LvLoopAnalysis.OwnerDraw = true;
            LvLoopAnalysis.RowHeight = 23;
            LvLoopAnalysis.ScrollContainer = ScLoopAnalysis;
            LvLoopAnalysis.SelectedRowColor = System.Drawing.Color.Blue;
            LvLoopAnalysis.Size = new System.Drawing.Size(525, 305);
            LvLoopAnalysis.TabIndex = 0;
            LvLoopAnalysis.TabStop = false;
            LvLoopAnalysis.Tag = "PowerLoopAnalysis";
            LvLoopAnalysis.UseCompatibleStateImageBehavior = false;
            LvLoopAnalysis.View = System.Windows.Forms.View.Details;
            LvLoopAnalysis.SizeChanged += LvLoopAnalysis_SizeChanged;
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
            Gain.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZengYi");
            Gain.Width = 120;
            // 
            // Phase
            // 
            Phase.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiangWei");
            Phase.Width = 120;
            // 
            // ScLoopAnalysis
            // 
            ScLoopAnalysis.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ScLoopAnalysis.Controls.Add(LvLoopAnalysis);
            ScLoopAnalysis.Dock = System.Windows.Forms.DockStyle.Fill;
            ScLoopAnalysis.Location = new System.Drawing.Point(2, 45);
            ScLoopAnalysis.Name = "ScLoopAnalysis";
            ScLoopAnalysis.ScrollControl = LvLoopAnalysis;
            ScLoopAnalysis.ScrollThickness = 6;
            ScLoopAnalysis.Size = new System.Drawing.Size(530, 312);
            ScLoopAnalysis.TabIndex = 0;
            ScLoopAnalysis.HorizontalScroll.Enabled = false;
            ScLoopAnalysis.SizeChanged += ScLoopAnalysis_SizeChanged;
            // 
            // TmUpdate
            // 
            TmUpdate.Enabled = true;
            TmUpdate.Interval = 500;
            TmUpdate.Elapsed += TmUpdate_Tick;
            // 
            // PwrLoopAnalysisInfoForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            CanClose = false;
            ClientSize = new System.Drawing.Size(534, 359);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(ScLoopAnalysis);
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
            Name = "PwrLoopAnalysisInfoForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("HuanLuFenXi");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("HuanLuFenXi");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleIcon = Properties.Resources.MeasureSetting;
            TitleIconSize = new System.Drawing.Size(26, 26);
            TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            TitleIconWidth = 26;
            ToolIconSize = new System.Drawing.Size(26, 26);
            HelpClick += PwrLoopAnalysisInfoForm_EmbededClick;
            Controls.SetChildIndex(ScLoopAnalysis, 0);
            ScLoopAnalysis.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ScopeX.UserControls.ScopeXListViewEx LvLoopAnalysis;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Frequency;
        private System.Windows.Forms.ColumnHeader Amplitude;
        private System.Windows.Forms.ColumnHeader Gain;
        private System.Windows.Forms.ColumnHeader Phase;
        private System.Timers.Timer TmUpdate;
        private ScopeX.UserControls.ScopeXScrollContainer ScLoopAnalysis;
    }
}