namespace ScopeX.U2;

partial class MeasureForm
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
            ScopeX.UserControls.GroupItem groupItem1 = new ScopeX.UserControls.GroupItem();
            ScopeX.UserControls.GroupItem groupItem2 = new ScopeX.UserControls.GroupItem();
            this.NbgMeasure = new ScopeX.UserControls.NavBarGroup();
            this.SuspendLayout();
            // 
            // NbgMeasure
            // 
            this.NbgMeasure.AssignHelper = "";
            this.NbgMeasure.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.NbgMeasure.CurrentGroupIndex = 0;
            this.NbgMeasure.Dock = System.Windows.Forms.DockStyle.Fill;
            groupItem1.BackGroundColor = System.Drawing.Color.Empty;
            groupItem1.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            groupItem1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem1.FontColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            groupItem1.GroupSize = new System.Drawing.Size(0, 0);
            groupItem1.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanShuSheZhi");
            groupItem2.BackGroundColor = System.Drawing.Color.Empty;
            groupItem2.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            groupItem2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem2.FontColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            groupItem2.GroupSize = new System.Drawing.Size(0, 0);
            groupItem2.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YunSuanSheZhi");
            this.NbgMeasure.GroupItems = new ScopeX.UserControls.GroupItem[] {
        groupItem1,
        groupItem2};
            this.NbgMeasure.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.NbgMeasure.Location = new System.Drawing.Point(0, 40);
            this.NbgMeasure.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.NbgMeasure.Name = "NbgMeasure";
            this.NbgMeasure.NavBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.NbgMeasure.NavBarHeight = 40;
            this.NbgMeasure.NavForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.NbgMeasure.NavGroupColor = System.Drawing.Color.Empty;
            this.NbgMeasure.NavGroupHeight = 353;
            this.NbgMeasure.Size = new System.Drawing.Size(571, 433);
            this.NbgMeasure.SplitColor = System.Drawing.Color.Empty;
            this.NbgMeasure.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.NbgMeasure.StylizeFlag = true;
            this.NbgMeasure.TabIndex = 5;
            // 
            // MeasureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 473);
            this.ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.ControlBox = false;
            this.Controls.Add(this.NbgMeasure);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormOpacity = 95;
            this.IconInterval = 21;
            this.IconWidth = 28;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MeasureForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.StylizeFlag = true;
            this.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanShuCeLiang");
            this.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanShuCeLiang");
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TitleIcon = global::ScopeX.U2.Properties.Resources.Measurement;
            this.TitleIconSize = new System.Drawing.Size(30, 30);
            this.TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.TitleIconWidth = 30;
            this.TitleLableHeight = 40;
            this.ToolIconInterval = 21;
            this.ToolIconSize = new System.Drawing.Size(28, 26);
            this.Controls.SetChildIndex(this.NbgMeasure, 0);
            this.ResumeLayout(false);

    }

    #endregion

    private ScopeX.UserControls.NavBarGroup NbgMeasure;
}
