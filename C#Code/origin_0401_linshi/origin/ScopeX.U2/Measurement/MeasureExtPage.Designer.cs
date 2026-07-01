namespace ScopeX.U2;

partial class MeasureExtPage
{
    /// <summary> 
    /// 必需的设计器变量。
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// 清理所有正在使用的资源。
    /// </summary>
    /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region 组件设计器生成的代码

    /// <summary> 
    /// 设计器支持所需的方法 - 不要修改
    /// 使用代码编辑器修改此方法的内容。
    /// </summary>
    private void InitializeComponent()
    {
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new ScopeX.UserControls.DefaultHighlightPrompt();
            this.CbxSource1 = new ScopeX.UserControls.ComboBoxEx();
            this.BtnOperator = new ScopeX.UserControls.ScopeXIconButton();
            this.LblOperator1 = new ScopeX.UserControls.ScopeXLabel();
            this.CbxSource2 = new ScopeX.UserControls.ComboBoxEx();
            this.LblEqu1 = new ScopeX.UserControls.ScopeXLabel();
            this.CbxDestination = new ScopeX.UserControls.ComboBoxEx();
            this.SuspendLayout();
            // 
            // CbxSource1
            // 
            this.CbxSource1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxSource1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxSource1.BorderThickness = 0;
            this.CbxSource1.CornerRadius = 0;
            this.CbxSource1.DataSource = null;
            this.CbxSource1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CbxSource1.DropDownHeight = 200;
            this.CbxSource1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxSource1.DropDownWidth = 65;
            this.CbxSource1.DropKey = System.Windows.Forms.Keys.Space;
            this.CbxSource1.DroppedDown = false;
            this.CbxSource1.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxSource1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CbxSource1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.CbxSource1.FormattingEnabled = true;
            this.CbxSource1.GetDisPlayName = null;
            this.CbxSource1.Height = 30;
            this.CbxSource1.ImageMode = false;
            this.CbxSource1.ItemHeight = 28;
            this.CbxSource1.KeyDropEnble = true;
            this.CbxSource1.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.CbxSource1.Location = new System.Drawing.Point(132, 68);
            this.CbxSource1.MaxDropDownItems = 8;
            this.CbxSource1.Name = "CbxSource1";
            this.CbxSource1.RectBtnWidth = 20;
            this.CbxSource1.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.CbxSource1.SelectedIndex = -1;
            this.CbxSource1.SelectedItem = null;
            this.CbxSource1.SelectedText = "";
            this.CbxSource1.Size = new System.Drawing.Size(65, 30);
            this.CbxSource1.Soreted = false;
            this.CbxSource1.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.CbxSource1.StylizeFlag = true;
            this.CbxSource1.TabIndex = 0;
            this.CbxSource1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBi");
            this.CbxSource1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.CbxSource1.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            // 
            // BtnOperator
            // 
            this.BtnOperator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnOperator.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnOperator.BorderThickness = 0;
            this.BtnOperator.CornerRadius = 0;
            this.BtnOperator.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnOperator.DaskArray = null;
            this.BtnOperator.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnOperator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnOperator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnOperator.Height = 30;
            this.BtnOperator.Icon = null;
            this.BtnOperator.IconOffset = 10;
            this.BtnOperator.IconSize = new System.Drawing.Size(24, 24);
            this.BtnOperator.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOperator.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnOperator.IsIndicatorShow = false;
            this.BtnOperator.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnOperator.Location = new System.Drawing.Point(205, 68);
            this.BtnOperator.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnOperator.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnOperator.MouseInBorderThickness = 0;
            this.BtnOperator.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnOperator.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnOperator.Name = "BtnOperator";
            this.BtnOperator.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnOperator.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnOperator.PressedBorderThickness = 0;
            this.BtnOperator.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnOperator.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnOperator.Size = new System.Drawing.Size(85, 30);
            this.BtnOperator.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnOperator.StylizeFlag = true;
            this.BtnOperator.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnOperator.SVGPath = "";
            this.BtnOperator.TabIndex = 3;
            this.BtnOperator.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Wu");
            this.BtnOperator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnOperator.Click += new System.EventHandler(this.BtnOperator_Click);
            // 
            // LblOperator1
            // 
            this.LblOperator1.BackColor = System.Drawing.Color.Empty;
            this.LblOperator1.BorderColor = System.Drawing.Color.Black;
            this.LblOperator1.BorderThickness = 0;
            this.LblOperator1.CornerRadius = 0;
            this.LblOperator1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblOperator1.ForeColor = System.Drawing.Color.White;
            this.LblOperator1.HighlightPrompt = defaultHighlightPrompt1;
            this.LblOperator1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblOperator1.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblOperator1.Location = new System.Drawing.Point(205, 40);
            this.LblOperator1.MultyLineFlag = false;
            this.LblOperator1.Name = "LblOperator1";
            this.LblOperator1.Size = new System.Drawing.Size(85, 18);
            this.LblOperator1.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblOperator1.StylizeFlag = false;
            this.LblOperator1.TabIndex = 2;
            this.LblOperator1.TabStop = false;
            this.LblOperator1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CaoZuo");
            this.LblOperator1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblOperator1.Token = null;
            // 
            // CbxSource2
            // 
            this.CbxSource2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxSource2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxSource2.BorderThickness = 0;
            this.CbxSource2.CornerRadius = 0;
            this.CbxSource2.DataSource = null;
            this.CbxSource2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CbxSource2.DropDownHeight = 200;
            this.CbxSource2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxSource2.DropDownWidth = 65;
            this.CbxSource2.DropKey = System.Windows.Forms.Keys.Space;
            this.CbxSource2.DroppedDown = false;
            this.CbxSource2.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxSource2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CbxSource2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.CbxSource2.FormattingEnabled = true;
            this.CbxSource2.GetDisPlayName = null;
            this.CbxSource2.Height = 30;
            this.CbxSource2.ImageMode = false;
            this.CbxSource2.ItemHeight = 28;
            this.CbxSource2.KeyDropEnble = true;
            this.CbxSource2.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.CbxSource2.Location = new System.Drawing.Point(296, 68);
            this.CbxSource2.MaxDropDownItems = 8;
            this.CbxSource2.Name = "CbxSource2";
            this.CbxSource2.RectBtnWidth = 20;
            this.CbxSource2.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.CbxSource2.SelectedIndex = -1;
            this.CbxSource2.SelectedItem = null;
            this.CbxSource2.SelectedText = "";
            this.CbxSource2.Size = new System.Drawing.Size(65, 30);
            this.CbxSource2.Soreted = false;
            this.CbxSource2.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.CbxSource2.StylizeFlag = true;
            this.CbxSource2.TabIndex = 1;
            this.CbxSource2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBi");
            this.CbxSource2.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.CbxSource2.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            // 
            // LblEqu1
            // 
            this.LblEqu1.BackColor = System.Drawing.Color.Empty;
            this.LblEqu1.BorderColor = System.Drawing.Color.Black;
            this.LblEqu1.BorderThickness = 0;
            this.LblEqu1.CornerRadius = 0;
            this.LblEqu1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblEqu1.ForeColor = System.Drawing.Color.White;
            this.LblEqu1.HighlightPrompt = defaultHighlightPrompt2;
            this.LblEqu1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblEqu1.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblEqu1.Location = new System.Drawing.Point(98, 68);
            this.LblEqu1.MultyLineFlag = false;
            this.LblEqu1.Name = "LblEqu1";
            this.LblEqu1.Size = new System.Drawing.Size(28, 30);
            this.LblEqu1.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblEqu1.StylizeFlag = false;
            this.LblEqu1.TabIndex = 4;
            this.LblEqu1.TabStop = false;
            this.LblEqu1.Text = "=";
            this.LblEqu1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblEqu1.Token = null;
            // 
            // CbxDestination
            // 
            this.CbxDestination.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxDestination.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxDestination.BorderThickness = 0;
            this.CbxDestination.CornerRadius = 0;
            this.CbxDestination.DataSource = null;
            this.CbxDestination.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CbxDestination.DropDownHeight = 200;
            this.CbxDestination.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxDestination.DropDownWidth = 65;
            this.CbxDestination.DropKey = System.Windows.Forms.Keys.Space;
            this.CbxDestination.DroppedDown = false;
            this.CbxDestination.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxDestination.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CbxDestination.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.CbxDestination.FormattingEnabled = true;
            this.CbxDestination.GetDisPlayName = null;
            this.CbxDestination.Height = 30;
            this.CbxDestination.ImageMode = false;
            this.CbxDestination.ItemHeight = 28;
            this.CbxDestination.Items.AddRange(new object[] {
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBi"),
            "P1",
            "P2",
            "P3",
            "P4",
            "P5",
            "P6",
            "P7",
            "P8"});
            this.CbxDestination.KeyDropEnble = true;
            this.CbxDestination.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.CbxDestination.Location = new System.Drawing.Point(27, 68);
            this.CbxDestination.MaxDropDownItems = 8;
            this.CbxDestination.Name = "CbxDestination";
            this.CbxDestination.RectBtnWidth = 20;
            this.CbxDestination.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.CbxDestination.SelectedIndex = -1;
            this.CbxDestination.SelectedItem = null;
            this.CbxDestination.SelectedText = "";
            this.CbxDestination.Size = new System.Drawing.Size(65, 30);
            this.CbxDestination.Soreted = false;
            this.CbxDestination.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.CbxDestination.StylizeFlag = true;
            this.CbxDestination.TabIndex = 0;
            this.CbxDestination.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBi");
            this.CbxDestination.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.CbxDestination.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.CbxDestination.SelectedIndexChanged += new System.EventHandler(this.CbxDestination_SelectedIndexChanged);
            // 
            // MeasureExtPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.Controls.Add(this.LblEqu1);
            this.Controls.Add(this.LblOperator1);
            this.Controls.Add(this.CbxSource2);
            this.Controls.Add(this.CbxDestination);
            this.Controls.Add(this.CbxSource1);
            this.Controls.Add(this.BtnOperator);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "MeasureExtPage";
            this.Size = new System.Drawing.Size(388, 168);
            this.ResumeLayout(false);

    }

    #endregion

    private ScopeX.UserControls.ComboBoxEx CbxSource1;
    private ScopeX.UserControls.ScopeXIconButton BtnOperator;
    private ScopeX.UserControls.ScopeXLabel LblOperator1;
    private ScopeX.UserControls.ComboBoxEx CbxSource2;
    private ScopeX.UserControls.ScopeXLabel LblEqu1;
    private ScopeX.UserControls.ComboBoxEx CbxDestination;
}
