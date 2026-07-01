namespace ScopeX.U2;

partial class HistParametersForm
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
        ScContent = new UserControls.ScopeXScrollContainer();
        LvContent = new UserControls.ScopeXListViewEx();
        ItemName = new System.Windows.Forms.ColumnHeader();
        ItemValue = new System.Windows.Forms.ColumnHeader();
        TmUpdate = new System.Timers.Timer();
        ScContent.SuspendLayout();
        SuspendLayout();
        // 
        // ScContent
        // 
        ScContent.BackColor = System.Drawing.Color.FromArgb(54, 54, 54);
        ScContent.Controls.Add(LvContent);
        ScContent.Dock = System.Windows.Forms.DockStyle.Fill;
        ScContent.Location = new System.Drawing.Point(0, 45);
        ScContent.Name = "ScContent";
        ScContent.ScrollControl = LvContent;
        ScContent.ScrollThickness = 6;
        ScContent.Size = new System.Drawing.Size(226, 311);
        ScContent.TabIndex = 5;
        // 
        // LvContent
        // 
        LvContent.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
        LvContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
        LvContent.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { ItemName, ItemValue });
        LvContent.Dock = System.Windows.Forms.DockStyle.Fill;
        LvContent.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        LvContent.ForeColor = System.Drawing.SystemColors.ControlLight;
        LvContent.FullRowSelect = true;
        LvContent.GridLinesColor = System.Drawing.Color.FromArgb(54, 54, 54);
        LvContent.HeaderBackColor = System.Drawing.Color.Gray;
        LvContent.HeaderForeColor = System.Drawing.Color.White;
        LvContent.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
        LvContent.IsIndependentWindow = false;
        LvContent.Location = new System.Drawing.Point(0, 0);
        LvContent.MultiSelect = false;
        LvContent.Name = "LvContent";
        LvContent.OwnerDraw = true;
        LvContent.Scrollable= true;
        LvContent.RowHeight = 23;
        LvContent.ScrollContainer = ScContent;
        LvContent.SelectedRowColor = System.Drawing.Color.Blue;
        LvContent.Size = new System.Drawing.Size(226, 311);
        LvContent.TabIndex = 1;
        LvContent.TabStop = false;
        LvContent.UseCompatibleStateImageBehavior = false;
        LvContent.View = System.Windows.Forms.View.Details;
        LvContent.DrawSubItem += LvContent_DrawSubItem;
        // 
        // ItemName
        // 
        ItemName.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MingCheng");
        ItemName.Width = 200;
        // 
        // ItemValue
        // 
        ItemValue.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Zhi");
        ItemValue.Width = 100;
        // 
        // TmUpdate
        // 
        TmUpdate.Enabled = true;
        TmUpdate.Interval = 500;
        TmUpdate.Elapsed += TmUpdate_Tick;
        // 
        // HistParametersForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        CanClose = false;
        ClientSize = new System.Drawing.Size(400, 400);
        ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
        ControlBox = false;
        Controls.Add(ScContent);
        DoubleBuffered = true;
        Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        FormOpacity = 95;
        HelpLabel = "30";
        IsShowPin = false;
        KeyPreview = true;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "HistParametersForm";
        ShowIcon = false;
        ShowInTaskbar = false;
        StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        Tag = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanShu");
        Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanShu");
        Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanShu");
        TitleColor = System.Drawing.Color.White;
        TitleFont = new System.Drawing.Font("Arial", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        TitleIconSize = new System.Drawing.Size(30, 30);
        TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
        TitleIconWidth = 30;
        TitleLableHeight = 40;
        HelpClick += HistParametersForm_LeftIconClick;
        Controls.SetChildIndex(ScContent, 0);
        ScContent.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private ScopeX.UserControls.ScopeXScrollContainer ScContent;
    private ScopeX.UserControls.ScopeXListViewEx LvContent;
    private System.Windows.Forms.ColumnHeader ItemName;
    private System.Windows.Forms.ColumnHeader ItemValue;
    private System.Timers.Timer TmUpdate;
}
