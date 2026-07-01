namespace ScopeX.U2
{
    partial class PwrTransientPage
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
            ScopeX.UserControls.RadioButtonItem radioButtonItem1 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem2 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new ScopeX.UserControls.DefaultHighlightPrompt();
            this.LblTransientOpt = new ScopeX.UserControls.ScopeXLabel();
            this.RdoTransientOpt = new ScopeX.UserControls.UIRadioButtonGroup();
            this.LblStableOutputV = new ScopeX.UserControls.ScopeXLabel();
            this.LblOvershootV = new ScopeX.UserControls.ScopeXLabel();
            this.BtnStableOutputV = new ScopeX.UserControls.ScopeXIconButton();
            this.BtnOvershootV = new ScopeX.UserControls.ScopeXIconButton();
            this.SuspendLayout();
            // 
            // LblTransientOpt
            // 
            this.LblTransientOpt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblTransientOpt.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblTransientOpt.BorderThickness = 0;
            this.LblTransientOpt.CornerRadius = 0;
            this.LblTransientOpt.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblTransientOpt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblTransientOpt.HighlightPrompt = defaultHighlightPrompt1;
            this.LblTransientOpt.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblTransientOpt.Location = new System.Drawing.Point(10, 6);
            this.LblTransientOpt.MultyLineFlag = false;
            this.LblTransientOpt.Name = "LblTransientOpt";
            this.LblTransientOpt.Size = new System.Drawing.Size(160, 18);
            this.LblTransientOpt.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblTransientOpt.StylizeFlag = true;
            this.LblTransientOpt.TabIndex = 0;
            this.LblTransientOpt.TabStop = false;
            this.LblTransientOpt.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Yuan");
            this.LblTransientOpt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblTransientOpt.Token = null;
            // 
            // RdoTransientOpt
            // 
            this.RdoTransientOpt.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.RdoTransientOpt.BackColor = System.Drawing.Color.Black;
            this.RdoTransientOpt.BorderColor = System.Drawing.Color.Black;
            this.RdoTransientOpt.BorderThickness = 0;
            this.RdoTransientOpt.ButtonBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.RdoTransientOpt.ButtonFont = null;
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaYuan");
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianLiuYuan");
            this.RdoTransientOpt.ButtonItems = new ScopeX.UserControls.RadioButtonItem[] {
        radioButtonItem1,
        radioButtonItem2};
            this.RdoTransientOpt.ButtonOffset = 10;
            this.RdoTransientOpt.ButtonTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.RdoTransientOpt.ChoosedButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.RdoTransientOpt.ChoosedButtonIndex = 0;
            this.RdoTransientOpt.ChoosedButtonTextColor = System.Drawing.Color.Black;
            this.RdoTransientOpt.ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.RdoTransientOpt.ContentPadding = new System.Windows.Forms.Padding(0);
            this.RdoTransientOpt.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.RdoTransientOpt.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.RdoTransientOpt.Height = 30;
            this.RdoTransientOpt.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.RdoTransientOpt.Location = new System.Drawing.Point(10, 29);
            this.RdoTransientOpt.Name = "RdoTransientOpt";
            this.RdoTransientOpt.Size = new System.Drawing.Size(160, 30);
            this.RdoTransientOpt.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.RdoTransientOpt.StylizeFlag = true;
            this.RdoTransientOpt.TabIndex = 1;
            // 
            // LblStableOutputV
            // 
            this.LblStableOutputV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblStableOutputV.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblStableOutputV.BorderThickness = 0;
            this.LblStableOutputV.CornerRadius = 0;
            this.LblStableOutputV.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblStableOutputV.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblStableOutputV.HighlightPrompt = defaultHighlightPrompt2;
            this.LblStableOutputV.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblStableOutputV.Location = new System.Drawing.Point(10, 86);
            this.LblStableOutputV.MultyLineFlag = false;
            this.LblStableOutputV.Name = "LblStableOutputV";
            this.LblStableOutputV.Size = new System.Drawing.Size(88, 18);
            this.LblStableOutputV.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblStableOutputV.StylizeFlag = true;
            this.LblStableOutputV.TabIndex = 2;
            this.LblStableOutputV.TabStop = false;
            this.LblStableOutputV.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WenDingShuChuDianYa");
            this.LblStableOutputV.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblStableOutputV.Token = null;
            // 
            // LblOvershootV
            // 
            this.LblOvershootV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblOvershootV.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblOvershootV.BorderThickness = 0;
            this.LblOvershootV.CornerRadius = 0;
            this.LblOvershootV.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblOvershootV.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblOvershootV.HighlightPrompt = defaultHighlightPrompt3;
            this.LblOvershootV.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblOvershootV.Location = new System.Drawing.Point(132, 86);
            this.LblOvershootV.MultyLineFlag = false;
            this.LblOvershootV.Name = "LblOvershootV";
            this.LblOvershootV.Size = new System.Drawing.Size(83, 18);
            this.LblOvershootV.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblOvershootV.StylizeFlag = true;
            this.LblOvershootV.TabIndex = 4;
            this.LblOvershootV.TabStop = false;
            this.LblOvershootV.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaGuoChong");
            this.LblOvershootV.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblOvershootV.Token = null;
            // 
            // BtnStableOutputV
            // 
            this.BtnStableOutputV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnStableOutputV.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnStableOutputV.BorderThickness = 0;
            this.BtnStableOutputV.CornerRadius = 0;
            this.BtnStableOutputV.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnStableOutputV.DaskArray = null;
            this.BtnStableOutputV.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnStableOutputV.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnStableOutputV.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnStableOutputV.Height = 30;
            this.BtnStableOutputV.Icon = null;
            this.BtnStableOutputV.IconOffset = 10;
            this.BtnStableOutputV.IconSize = new System.Drawing.Size(24, 24);
            this.BtnStableOutputV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnStableOutputV.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnStableOutputV.IsIndicatorShow = false;
            this.BtnStableOutputV.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnStableOutputV.Location = new System.Drawing.Point(10, 110);
            this.BtnStableOutputV.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnStableOutputV.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnStableOutputV.MouseInBorderThickness = 0;
            this.BtnStableOutputV.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnStableOutputV.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnStableOutputV.Name = "BtnStableOutputV";
            this.BtnStableOutputV.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnStableOutputV.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnStableOutputV.PressedBorderThickness = 0;
            this.BtnStableOutputV.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnStableOutputV.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnStableOutputV.Size = new System.Drawing.Size(88, 30);
            this.BtnStableOutputV.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnStableOutputV.StylizeFlag = true;
            this.BtnStableOutputV.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnStableOutputV.SVGPath = "";
            this.BtnStableOutputV.TabIndex = 3;
            this.BtnStableOutputV.Text = "0mV";
            this.BtnStableOutputV.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BtnOvershootV
            // 
            this.BtnOvershootV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnOvershootV.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnOvershootV.BorderThickness = 0;
            this.BtnOvershootV.CornerRadius = 0;
            this.BtnOvershootV.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnOvershootV.DaskArray = null;
            this.BtnOvershootV.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnOvershootV.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnOvershootV.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnOvershootV.Height = 30;
            this.BtnOvershootV.Icon = null;
            this.BtnOvershootV.IconOffset = 10;
            this.BtnOvershootV.IconSize = new System.Drawing.Size(24, 24);
            this.BtnOvershootV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOvershootV.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnOvershootV.IsIndicatorShow = false;
            this.BtnOvershootV.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnOvershootV.Location = new System.Drawing.Point(132, 110);
            this.BtnOvershootV.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnOvershootV.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnOvershootV.MouseInBorderThickness = 0;
            this.BtnOvershootV.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnOvershootV.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnOvershootV.Name = "BtnOvershootV";
            this.BtnOvershootV.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnOvershootV.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnOvershootV.PressedBorderThickness = 0;
            this.BtnOvershootV.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnOvershootV.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnOvershootV.Size = new System.Drawing.Size(83, 30);
            this.BtnOvershootV.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnOvershootV.StylizeFlag = true;
            this.BtnOvershootV.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnOvershootV.SVGPath = "";
            this.BtnOvershootV.TabIndex = 5;
            this.BtnOvershootV.Text = "10%";
            this.BtnOvershootV.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PwrTransientPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.Controls.Add(this.BtnOvershootV);
            this.Controls.Add(this.BtnStableOutputV);
            this.Controls.Add(this.LblOvershootV);
            this.Controls.Add(this.LblStableOutputV);
            this.Controls.Add(this.LblTransientOpt);
            this.Controls.Add(this.RdoTransientOpt);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "PwrTransientPage";
            this.Size = new System.Drawing.Size(295, 156);
            this.ResumeLayout(false);

        }

        #endregion

        private ScopeX.UserControls.ScopeXLabel LblTransientOpt;
        private ScopeX.UserControls.UIRadioButtonGroup RdoTransientOpt;
        private ScopeX.UserControls.ScopeXLabel LblStableOutputV;
        private ScopeX.UserControls.ScopeXLabel LblOvershootV;
        private ScopeX.UserControls.ScopeXIconButton BtnStableOutputV;
        private ScopeX.UserControls.ScopeXIconButton BtnOvershootV;
    }
}
