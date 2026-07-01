
using System.Reflection.Metadata;

namespace ScopeX.U2
{
    partial class PwrModulationInfoForm
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
            LvModulation = new UserControls.ScopeXListViewEx();
            Parameter = new System.Windows.Forms.ColumnHeader();
            Value = new System.Windows.Forms.ColumnHeader();
            Mean = new System.Windows.Forms.ColumnHeader();
            Max = new System.Windows.Forms.ColumnHeader();
            Min = new System.Windows.Forms.ColumnHeader();
            ScModulation = new UserControls.ScopeXScrollContainer();
            TmUpdate = new System.Timers.Timer();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ScModulation.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // LvQuality
            // 
            LvModulation.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LvModulation.BorderStyle = System.Windows.Forms.BorderStyle.None;
            LvModulation.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { Parameter, Value, Mean, Max, Min });
            LvModulation.Dock = System.Windows.Forms.DockStyle.Fill;
            LvModulation.Font = new System.Drawing.Font("MiSans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LvModulation.ForeColor = System.Drawing.SystemColors.ControlLight;
            LvModulation.FullRowSelect = true;
            LvModulation.GridLines = true;
            LvModulation.GridLinesColor = System.Drawing.Color.Gray;
            LvModulation.HeaderBackColor = System.Drawing.Color.Gray;
            LvModulation.HeaderForeColor = System.Drawing.Color.White;
            LvModulation.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            LvModulation.HideSelection = true;
            LvModulation.IsIndependentWindow = false;
            LvModulation.Location = new System.Drawing.Point(0, 0);
            LvModulation.MultiSelect = false;
            LvModulation.Name = "LvQuality";
            LvModulation.OwnerDraw = true;
            LvModulation.RowHeight = 23;
            LvModulation.ScrollContainer = ScModulation;
            LvModulation.SelectedRowColor = System.Drawing.Color.Blue;
            LvModulation.Size = new System.Drawing.Size(620, 291);
            LvModulation.TabIndex = 0;
            LvModulation.TabStop = false;
            LvModulation.Tag = "PowerQuality";
            LvModulation.UseCompatibleStateImageBehavior = false;
            LvModulation.View = System.Windows.Forms.View.Details;
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
            ScModulation.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ScModulation.Controls.Add(LvModulation);
            ScModulation.Dock = System.Windows.Forms.DockStyle.Fill;
            ScModulation.Location = new System.Drawing.Point(3, 3);
            ScModulation.Name = "ScQuality";
            ScModulation.ScrollControl = LvModulation;
            ScModulation.ScrollThickness = 6;
            ScModulation.Size = new System.Drawing.Size(620, 291);
            ScModulation.TabIndex = 0;
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
            tableLayoutPanel1.Controls.Add(ScModulation, 0, 0);
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
            ScModulation.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ScopeX.UserControls.ScopeXListViewEx LvModulation;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.ColumnHeader Mean;
        private System.Windows.Forms.ColumnHeader Max;
        private System.Windows.Forms.ColumnHeader Min;
        private System.Timers.Timer TmUpdate;
        private ScopeX.UserControls.ScopeXScrollContainer ScModulation;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}