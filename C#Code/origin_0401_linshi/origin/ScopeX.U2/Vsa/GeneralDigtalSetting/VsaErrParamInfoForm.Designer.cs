
namespace ScopeX.U2
{
    partial class VsaErrParamInfoForm
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
            this.LvQuality = new ScopeX.UserControls.ScopeXListViewEx();
            this.Parameter = new System.Windows.Forms.ColumnHeader();
            this.Value = new System.Windows.Forms.ColumnHeader();
            this.Mean = new System.Windows.Forms.ColumnHeader();
            this.Max = new System.Windows.Forms.ColumnHeader();
            this.Min = new System.Windows.Forms.ColumnHeader();
            this.TmUpdate = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // LvQuality
            // 
            this.LvQuality.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LvQuality.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LvQuality.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Parameter,
            this.Value,
            this.Mean,
            this.Max,
            this.Min});
            this.LvQuality.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LvQuality.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.LvQuality.FullRowSelect = true;
            this.LvQuality.GridLines = true;
            this.LvQuality.GridLinesColor = System.Drawing.Color.Black;
            this.LvQuality.HeaderBackColor = System.Drawing.Color.Gray;
            this.LvQuality.HeaderForeColor = System.Drawing.Color.White;
            this.LvQuality.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LvQuality.IsIndependentWindow = false;
            this.LvQuality.Location = new System.Drawing.Point(0, 45);
            this.LvQuality.MultiSelect = false;
            this.LvQuality.Name = "LvQuality";
            this.LvQuality.OwnerDraw = true;
            this.LvQuality.RowHeight = 23;
            this.LvQuality.Scrollable = false;            
            this.LvQuality.SelectedRowColor = System.Drawing.Color.Blue;
            this.LvQuality.Size = new System.Drawing.Size(525, 270);
            this.LvQuality.TabIndex = 0;
            this.LvQuality.TabStop = false;
            this.LvQuality.Tag = "PowerQuality";
            this.LvQuality.UseCompatibleStateImageBehavior = false;
            this.LvQuality.View = System.Windows.Forms.View.Details;
            // 
            // Parameter
            // 
            this.Parameter.Text = "";
            this.Parameter.Width = 150;
            // 
            // Value
            // 
            this.Value.Text = "值";
            this.Value.Width = 120;
            // 
            // Mean
            // 
            this.Mean.Text = "平均值";
            this.Mean.Width = 85;
            // 
            // Max
            // 
            this.Max.Text = "最大值";
            this.Max.Width = 85;
            // 
            // Min
            // 
            this.Min.Text = "最小值";
            this.Min.Width = 85;
            // 
            // TmUpdate
            // 
            this.TmUpdate.Enabled = true;
            this.TmUpdate.Interval = 500;
            this.TmUpdate.Tick += new System.EventHandler(this.TmUpdate_Tick);
            // 
            // VsaErrParamInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CanClose = false;
            this.ClientSize = new System.Drawing.Size(525, 315);
            this.ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.ControlBox = false;
            this.Controls.Add(this.LvQuality);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormOpacity = 95;
            this.HeadHeight = 45;
            this.IsShowClose = false;
            this.IsShowPin = false;
            this.FixedToolIconInfos[2].Icon = global::ScopeX.U2.Properties.Resources.FormEmbed;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VsaErrParamInfoForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "符号误差表";
            this.Title = "符号误差表";
            this.TitleColor = System.Drawing.Color.White;
            this.TitleFont = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TitleIconSize = new System.Drawing.Size(30, 30);
            this.TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.TitleIconWidth = 30;
            this.TitleLableHeight = 39;
            this.HelpClick += new System.EventHandler(this.PwrQualityInfoForm_LeftIconClick);
            this.Controls.SetChildIndex(this.LvQuality, 0);
            this.ResumeLayout(false);

        }

        #endregion
        private ScopeX.UserControls.ScopeXListViewEx LvQuality;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.ColumnHeader Mean;
        private System.Windows.Forms.ColumnHeader Max;
        private System.Windows.Forms.ColumnHeader Min;
        private System.Windows.Forms.Timer TmUpdate;
    }
}