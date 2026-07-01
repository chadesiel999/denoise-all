
namespace ScopeX.U2
{
    partial class PwrEfficiencyInfoForm
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
            LvEfficiency = new ScopeX.UserControls.ScopeXListViewEx();
            Parameter = new System.Windows.Forms.ColumnHeader();
            Value = new System.Windows.Forms.ColumnHeader();
            Mean = new System.Windows.Forms.ColumnHeader();
            Max = new System.Windows.Forms.ColumnHeader();
            Min = new System.Windows.Forms.ColumnHeader();
            ScEfficiency = new ScopeX.UserControls.ScopeXScrollContainer();
            TmUpdate = new System.Timers.Timer();
            ScEfficiency.SuspendLayout();
            SuspendLayout();
            // 
            // LvEfficiency
            // 
            LvEfficiency.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            LvEfficiency.BorderStyle = System.Windows.Forms.BorderStyle.None;
            LvEfficiency.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            Parameter,
            Value,
            Mean,
            Max,
            Min,});
            LvEfficiency.Dock = System.Windows.Forms.DockStyle.Fill;
            LvEfficiency.ForeColor = System.Drawing.SystemColors.ControlLight;
            LvEfficiency.GridLines = true;
            LvEfficiency.GridLinesColor = System.Drawing.Color.Gray;
            LvEfficiency.HeaderBackColor = System.Drawing.Color.Gray;
            LvEfficiency.HeaderForeColor = System.Drawing.Color.White;
            LvEfficiency.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            LvEfficiency.HideSelection = true;
            LvEfficiency.IsIndependentWindow = false;
            LvEfficiency.Location = new System.Drawing.Point(0, 0);
            LvEfficiency.MultiSelect = false;
            LvEfficiency.Name = "LvEfficiency";
            LvEfficiency.OwnerDraw = true;
            LvEfficiency.RowHeight = 23;
            LvEfficiency.Scrollable = false;
            LvEfficiency.ScrollContainer = ScEfficiency;
            LvEfficiency.SelectedRowColor = System.Drawing.Color.Blue;
            LvEfficiency.Size = new System.Drawing.Size(718, 133);
            LvEfficiency.TabIndex = 0;
            LvEfficiency.TabStop = false;
            LvEfficiency.Tag = "PowerEfficency";
            LvEfficiency.UseCompatibleStateImageBehavior = false;
            LvEfficiency.View = System.Windows.Forms.View.Details;
            // 
            // Parameter
            // 
            Parameter.Text = "";
            Parameter.Width = 160;
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
            // ScEfficiency
            // 
            ScEfficiency.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ScEfficiency.Controls.Add(LvEfficiency);
            ScEfficiency.Dock = System.Windows.Forms.DockStyle.Fill;
            ScEfficiency.Location = new System.Drawing.Point(3, 3);
            ScEfficiency.Name = "ScEfficiency";
            ScEfficiency.ScrollControl = LvEfficiency;
            ScEfficiency.ScrollThickness = 6;
            ScEfficiency.Size = new System.Drawing.Size(718, 133);
            ScEfficiency.TabIndex = 0;
            // 
            // TmUpdate
            // 
            TmUpdate.Enabled = true;
            TmUpdate.Interval = 500;
            TmUpdate.Elapsed += TmUpdate_Tick;
            // 
            // PwrEfficiencyInfoForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CanClose = false;
            ClientSize = new System.Drawing.Size(720, 180);
            ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            ControlBox = false;
            Controls.Add(ScEfficiency);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HeadHeight = 45;
            IconWidth = 26;
            IsShowPin = false;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PwrEfficiencyInfoForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYuanXiaoLv");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYuanXiaoLv");
            TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleIcon = global::ScopeX.U2.Properties.Resources.MeasureSetting;
            TitleIconSize = new System.Drawing.Size(26, 26);
            TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            TitleIconWidth = 26;
            TitleLableHeight = 39;
            ToolIconSize = new System.Drawing.Size(26, 26);
            HelpClick += new System.EventHandler(PwrEfficiencyInfoForm_EmbededClick);
            TitleIconClick += new System.EventHandler(PwrEfficiencyInfoForm_SettingClick);
            Controls.SetChildIndex(ScEfficiency, 0);
            ScEfficiency.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion
        private ScopeX.UserControls.ScopeXListViewEx LvEfficiency;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.ColumnHeader Mean;
        private System.Windows.Forms.ColumnHeader Max;
        private System.Windows.Forms.ColumnHeader Min;
        private System.Timers.Timer TmUpdate;
        private ScopeX.UserControls.ScopeXScrollContainer ScEfficiency;
    }
}