namespace ScopeX.U2;

partial class MeasStatisticForm
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
            this.ScMeasure = new ScopeX.UserControls.ScopeXScrollContainer();
            this.LvMeasure = new ScopeX.UserControls.ScopeXListViewEx();
            this.Index = new System.Windows.Forms.ColumnHeader();
            this.Parameter = new System.Windows.Forms.ColumnHeader();
            this.Mean = new System.Windows.Forms.ColumnHeader();
            this.Value = new System.Windows.Forms.ColumnHeader();
            this.Max = new System.Windows.Forms.ColumnHeader();
            this.Min = new System.Windows.Forms.ColumnHeader();
            this.Sigma = new System.Windows.Forms.ColumnHeader();
            this.Population = new System.Windows.Forms.ColumnHeader();
            this.TmUpdate = new System.Timers.Timer();
            this.ScMeasure.SuspendLayout();
            this.SuspendLayout();
            // 
            // ScMeasure
            // 
            this.ScMeasure.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.ScMeasure.Controls.Add(this.LvMeasure);
            this.ScMeasure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScMeasure.Location = new System.Drawing.Point(0, 45);
            this.ScMeasure.Name = "ScMeasure";
            this.ScMeasure.ScrollControl = this.LvMeasure;
            this.ScMeasure.ScrollThickness = 6;
            this.ScMeasure.Size = new System.Drawing.Size(959, 275);
            this.ScMeasure.TabIndex = 0;
            // 
            // LvMeasure
            // 
            this.LvMeasure.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LvMeasure.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LvMeasure.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Index,
            this.Parameter,
            this.Mean,
            this.Value,
            this.Max,
            this.Min,
            this.Sigma,
            this.Population});
            this.LvMeasure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LvMeasure.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.LvMeasure.FullRowSelect = true;
            this.LvMeasure.GridLinesColor = System.Drawing.Color.Gray;
            this.LvMeasure.HeaderBackColor = System.Drawing.Color.Gray;
            this.LvMeasure.HeaderForeColor = System.Drawing.Color.White;
            this.LvMeasure.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LvMeasure.HideSelection = true;
            this.LvMeasure.IsIndependentWindow = false;
            this.LvMeasure.Location = new System.Drawing.Point(0, 0);
            this.LvMeasure.MultiSelect = false;
            this.LvMeasure.Name = "LvMeasure";
            this.LvMeasure.OwnerDraw = true;
            this.LvMeasure.RowHeight = 30;
            this.LvMeasure.ScrollContainer = this.ScMeasure;
            this.LvMeasure.SelectedRowColor = System.Drawing.Color.Blue;
            this.LvMeasure.Size = new System.Drawing.Size(953, 269);
            this.LvMeasure.TabIndex = 0;
            this.LvMeasure.TabStop = false;
            this.LvMeasure.Tag = "";
            this.LvMeasure.UseCompatibleStateImageBehavior = false;
            this.LvMeasure.View = System.Windows.Forms.View.Details;
            this.LvMeasure.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.LvMeasure_DrawSubItem);
            this.LvMeasure.Click += new System.EventHandler(this.LvMeasure_Click);
            // 
            // Index
            // 
            this.Index.Text = "";
            this.Index.Width = 50;
            // 
            // Parameter
            // 
            this.Parameter.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MingCheng");
            this.Parameter.Width = 200;
            // 
            // Mean
            // 
            this.Mean.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PingJunZhi");
            this.Mean.Width = 120;
            // 
            // Value
            // 
            this.Value.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DangQianZhi");
            this.Value.Width = 120;
            // 
            // Max
            // 
            this.Max.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZuiDaZhi");
            this.Max.Width = 120;
            // 
            // Min
            // 
            this.Min.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZuiXiaoZhi");
            this.Min.Width = 120;
            // 
            // Sigma
            // 
            this.Sigma.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BiaoZhunCha");
            this.Sigma.Width = 120;
            // 
            // Population
            // 
            this.Population.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CiShu");
            this.Population.Width = 100;
            // 
            // TmUpdate
            // 
            this.TmUpdate.Enabled = true;
            this.TmUpdate.Interval = 500;
            this.TmUpdate.Elapsed += TmUpdate_Tick;
            // 
            // MeasStatisticForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CanClose = false;
            this.ClientSize = new System.Drawing.Size(959, 320);
            this.ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.ControlBox = false;
            this.Controls.Add(this.ScMeasure);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormOpacity = 95;
            this.HeadHeight = 45;
            this.IconInterval = 21;
            this.IconWidth = 26;
            this.IsShowPin = false;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MeasStatisticForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanShuCeLiang");
            this.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanShuCeLiang");
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TitleIconSize = new System.Drawing.Size(26, 26);
            this.TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.TitleIconWidth = 26;
            this.TitleLableHeight = 45;
            this.ToolIconInterval = 21;
            this.ToolIconSize = new System.Drawing.Size(26, 26);
            this.HelpClick += new System.EventHandler(this.MeasStatisticForm_EmbededClick);
            this.Controls.SetChildIndex(this.ScMeasure, 0);
            this.ScMeasure.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private ScopeX.UserControls.ScopeXScrollContainer ScMeasure;
    private ScopeX.UserControls.ScopeXListViewEx LvMeasure;
    private System.Windows.Forms.ColumnHeader Index;
    private System.Windows.Forms.ColumnHeader Parameter;
    private System.Windows.Forms.ColumnHeader Mean;
    private System.Windows.Forms.ColumnHeader Value;
    private System.Windows.Forms.ColumnHeader Max;
    private System.Timers.Timer TmUpdate;
    private System.Windows.Forms.ColumnHeader Min;
    private System.Windows.Forms.ColumnHeader Sigma;
    private System.Windows.Forms.ColumnHeader Population;
}
