
using System.Reflection.Metadata;

namespace ScopeX.U2
{
    partial class PwrQualityInfoForm
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
            LvQuality = new UserControls.ScopeXListViewEx();
            Parameter = new System.Windows.Forms.ColumnHeader();
            Value = new System.Windows.Forms.ColumnHeader();
            Mean = new System.Windows.Forms.ColumnHeader();
            Max = new System.Windows.Forms.ColumnHeader();
            Min = new System.Windows.Forms.ColumnHeader();
            ScQuality = new UserControls.ScopeXScrollContainer();
            TmUpdate = new System.Timers.Timer();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ScQuality.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // LvQuality
            // 
            LvQuality.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LvQuality.BorderStyle = System.Windows.Forms.BorderStyle.None;
            LvQuality.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { Parameter, Value, Mean, Max, Min });
            LvQuality.Dock = System.Windows.Forms.DockStyle.Fill;
            LvQuality.Font = new System.Drawing.Font("MiSans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LvQuality.ForeColor = System.Drawing.SystemColors.ControlLight;
            LvQuality.FullRowSelect = true;
            LvQuality.GridLines = true;
            LvQuality.GridLinesColor = System.Drawing.Color.Gray;
            LvQuality.HeaderBackColor = System.Drawing.Color.Gray;
            LvQuality.HeaderForeColor = System.Drawing.Color.White;
            LvQuality.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            LvQuality.HideSelection = true;
            LvQuality.IsIndependentWindow = false;
            LvQuality.Location = new System.Drawing.Point(0, 0);
            LvQuality.MultiSelect = false;
            LvQuality.Name = "LvQuality";
            LvQuality.OwnerDraw = true;
            LvQuality.RowHeight = 23;
            LvQuality.ScrollContainer = ScQuality;
            LvQuality.SelectedRowColor = System.Drawing.Color.Blue;
            LvQuality.Size = new System.Drawing.Size(620, 291);
            LvQuality.TabIndex = 0;
            LvQuality.TabStop = false;
            LvQuality.Tag = "PowerQuality";
            LvQuality.UseCompatibleStateImageBehavior = false;
            LvQuality.View = System.Windows.Forms.View.Details;
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
            // ScQuality
            // 
            ScQuality.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ScQuality.Controls.Add(LvQuality);
            ScQuality.Dock = System.Windows.Forms.DockStyle.Fill;
            ScQuality.Location = new System.Drawing.Point(3, 3);
            ScQuality.Name = "ScQuality";
            ScQuality.ScrollControl = LvQuality;
            ScQuality.ScrollThickness = 6;
            ScQuality.Size = new System.Drawing.Size(620, 291);
            ScQuality.TabIndex = 0;
            // 
            // TmUpdate
            // 
            TmUpdate.Enabled = true;
            TmUpdate.Interval = 500;
            TmUpdate.Elapsed += TmUpdate_Tick;
          
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(ScQuality, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(2, 45);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            tableLayoutPanel1.Size = new System.Drawing.Size(626, 347);
            tableLayoutPanel1.TabIndex = 2;
            tableLayoutPanel1.SizeChanged += tableLayoutPanel1_SizeChanged;
            // 
            // PwrQualityInfoForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            CanClose = false;
            ClientSize = new System.Drawing.Size(680, 394);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(tableLayoutPanel1);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            IconWidth = 26;
            IsShowPin = false;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PwrQualityInfoForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleIcon = Properties.Resources.MeasureSetting;
            TitleIconSize = new System.Drawing.Size(26, 26);
            TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            TitleIconWidth = 26;
            ToolIconSize = new System.Drawing.Size(26, 26);
            HelpClick += PwrQualityInfoForm_EmbededClick;
            Controls.SetChildIndex(tableLayoutPanel1, 0);
            ScQuality.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ScopeX.UserControls.ScopeXListViewEx LvQuality;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.ColumnHeader Mean;
        private System.Windows.Forms.ColumnHeader Max;
        private System.Windows.Forms.ColumnHeader Min;
        private System.Timers.Timer TmUpdate;
        private ScopeX.UserControls.ScopeXScrollContainer ScQuality;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}