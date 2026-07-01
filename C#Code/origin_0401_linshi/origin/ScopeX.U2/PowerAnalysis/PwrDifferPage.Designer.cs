
namespace ScopeX.U2
{
    partial class PwrDifferPage
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.RadioButtonItem radioButtonItem1 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem2 = new ScopeX.UserControls.RadioButtonItem();
            this.LblDifferSrcType = new ScopeX.UserControls.ScopeXLabel();
            this.RdoDifferSrc = new ScopeX.UserControls.UIRadioButtonGroup();
            this.BtnShowDifferWfm = new ScopeX.UserControls.ScopeXIconButton();
            this.SuspendLayout();
            // 
            // LblDifferSrcType
            // 
            this.LblDifferSrcType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblDifferSrcType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblDifferSrcType.BorderThickness = 0;
            this.LblDifferSrcType.CornerRadius = 0;
            this.LblDifferSrcType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblDifferSrcType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblDifferSrcType.HighlightPrompt = defaultHighlightPrompt1;
            this.LblDifferSrcType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblDifferSrcType.Location = new System.Drawing.Point(10, 6);
            this.LblDifferSrcType.MultyLineFlag = false;
            this.LblDifferSrcType.Name = "LblDifferSrcType";
            this.LblDifferSrcType.Size = new System.Drawing.Size(160, 17);
            this.LblDifferSrcType.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblDifferSrcType.StylizeFlag = true;
            this.LblDifferSrcType.TabIndex = 0;
            this.LblDifferSrcType.TabStop = false;
            this.LblDifferSrcType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WeiFenYuan");
            this.LblDifferSrcType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblDifferSrcType.Token = null;
            // 
            // RdoDifferSrc
            // 
            this.RdoDifferSrc.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.RdoDifferSrc.BackColor = System.Drawing.Color.Black;
            this.RdoDifferSrc.BorderColor = System.Drawing.Color.Black;
            this.RdoDifferSrc.BorderThickness = 0;
            this.RdoDifferSrc.ButtonBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.RdoDifferSrc.ButtonFont = null;
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaYuan");
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianLiuYuan");
            this.RdoDifferSrc.ButtonItems = new ScopeX.UserControls.RadioButtonItem[] {
        radioButtonItem1,
        radioButtonItem2};
            this.RdoDifferSrc.ButtonOffset = 10;
            this.RdoDifferSrc.ButtonTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.RdoDifferSrc.ChoosedButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.RdoDifferSrc.ChoosedButtonIndex = 0;
            this.RdoDifferSrc.ChoosedButtonTextColor = System.Drawing.Color.Black;
            this.RdoDifferSrc.ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.RdoDifferSrc.ContentPadding = new System.Windows.Forms.Padding(0);
            this.RdoDifferSrc.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.RdoDifferSrc.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.RdoDifferSrc.Height = 30;
            this.RdoDifferSrc.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.RdoDifferSrc.Location = new System.Drawing.Point(10, 29);
            this.RdoDifferSrc.Name = "RdoDifferSrc";
            this.RdoDifferSrc.Size = new System.Drawing.Size(160, 30);
            this.RdoDifferSrc.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.RdoDifferSrc.StylizeFlag = true;
            this.RdoDifferSrc.TabIndex = 1;
            this.RdoDifferSrc.IndexChanged += new System.EventHandler(this.RdoDifferSrc_IndexChanged);
            // 
            // BtnShowDifferWfm
            // 
            this.BtnShowDifferWfm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnShowDifferWfm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnShowDifferWfm.BorderThickness = 0;
            this.BtnShowDifferWfm.CornerRadius = 0;
            this.BtnShowDifferWfm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnShowDifferWfm.DaskArray = null;
            this.BtnShowDifferWfm.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnShowDifferWfm.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnShowDifferWfm.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnShowDifferWfm.Height = 30;
            this.BtnShowDifferWfm.Icon = null;
            this.BtnShowDifferWfm.IconOffset = 10;
            this.BtnShowDifferWfm.IconSize = new System.Drawing.Size(24, 24);
            this.BtnShowDifferWfm.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnShowDifferWfm.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnShowDifferWfm.IsIndicatorShow = false;
            this.BtnShowDifferWfm.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnShowDifferWfm.Location = new System.Drawing.Point(235, 29);
            this.BtnShowDifferWfm.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnShowDifferWfm.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnShowDifferWfm.MouseInBorderThickness = 0;
            this.BtnShowDifferWfm.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnShowDifferWfm.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnShowDifferWfm.Name = "BtnShowDifferWfm";
            this.BtnShowDifferWfm.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnShowDifferWfm.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnShowDifferWfm.PressedBorderThickness = 0;
            this.BtnShowDifferWfm.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnShowDifferWfm.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnShowDifferWfm.Size = new System.Drawing.Size(110, 30);
            this.BtnShowDifferWfm.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnShowDifferWfm.StylizeFlag = true;
            this.BtnShowDifferWfm.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnShowDifferWfm.SVGPath = "";
            this.BtnShowDifferWfm.TabIndex = 2;
            this.BtnShowDifferWfm.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShiWeiFenBoXing");
            this.BtnShowDifferWfm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnShowDifferWfm.Click += new System.EventHandler(this.BtnShowDifferWfm_Click);
            // 
            // PwrDifferPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.Controls.Add(this.BtnShowDifferWfm);
            this.Controls.Add(this.LblDifferSrcType);
            this.Controls.Add(this.RdoDifferSrc);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "PwrDifferPage";
            this.Size = new System.Drawing.Size(450, 78);
            this.ResumeLayout(false);

        }

        #endregion

        private ScopeX.UserControls.ScopeXLabel LblDifferSrcType;
        private ScopeX.UserControls.UIRadioButtonGroup RdoDifferSrc;
        private ScopeX.UserControls.ScopeXIconButton BtnShowDifferWfm;
    }
}
