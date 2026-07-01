namespace ScopeX.U2
{
    partial class OtherWfmPage
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
            this.ChkPhaseVSTime = new System.Windows.Forms.CheckBox();
            this.ChkFrequencyVSTime = new System.Windows.Forms.CheckBox();
            this.ChkAmpVSTime = new System.Windows.Forms.CheckBox();
            this.ChkPhaseSpec = new ScopeX.UserControls.ScopeXSwitchButton();
            this.LblPhaseSpec = new ScopeX.UserControls.ScopeXLabel();
            this.ChkWaterfall = new ScopeX.UserControls.ScopeXSwitchButton();
            this.LblWatefall = new ScopeX.UserControls.ScopeXLabel();
            this.GrpModulation = new ScopeX.UserControls.ScopeXGroupBox();
            this.GrpModulation.SuspendLayout();
            this.SuspendLayout();
            // 
            // ChkPhaseVSTime
            // 
            this.ChkPhaseVSTime.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ChkPhaseVSTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkPhaseVSTime.Location = new System.Drawing.Point(309, 44);
            this.ChkPhaseVSTime.Name = "ChkPhaseVSTime";
            this.ChkPhaseVSTime.Size = new System.Drawing.Size(100, 30);
            this.ChkPhaseVSTime.TabIndex = 2;
            this.ChkPhaseVSTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiangWeiDuiShiJian");
            this.ChkPhaseVSTime.UseVisualStyleBackColor = true;
            this.ChkPhaseVSTime.Click += new System.EventHandler(this.ChkPhaseVSTime_Click);
            // 
            // ChkFrequencyVSTime
            // 
            this.ChkFrequencyVSTime.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ChkFrequencyVSTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkFrequencyVSTime.Location = new System.Drawing.Point(172, 44);
            this.ChkFrequencyVSTime.Name = "ChkFrequencyVSTime";
            this.ChkFrequencyVSTime.Size = new System.Drawing.Size(100, 30);
            this.ChkFrequencyVSTime.TabIndex = 1;
            this.ChkFrequencyVSTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PinLvDuiShiJian");
            this.ChkFrequencyVSTime.UseVisualStyleBackColor = true;
            this.ChkFrequencyVSTime.Click += new System.EventHandler(this.ChkFrequencyVSTime_Click);
            // 
            // ChkAmpVSTime
            // 
            this.ChkAmpVSTime.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ChkAmpVSTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkAmpVSTime.Location = new System.Drawing.Point(35, 44);
            this.ChkAmpVSTime.Name = "ChkAmpVSTime";
            this.ChkAmpVSTime.Size = new System.Drawing.Size(100, 30);
            this.ChkAmpVSTime.TabIndex = 0;
            this.ChkAmpVSTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuDuDuiShiJian");
            this.ChkAmpVSTime.UseVisualStyleBackColor = true;
            this.ChkAmpVSTime.Click += new System.EventHandler(this.ChkAmpVSTime_Click);
            // 
            // ChkPhaseSpec
            // 
            this.ChkPhaseSpec.AnimationCount = 8;
            this.ChkPhaseSpec.AnimationFunc = null;
            this.ChkPhaseSpec.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkPhaseSpec.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkPhaseSpec.BorderThickness = 0;
            this.ChkPhaseSpec.Checked = false;
            this.ChkPhaseSpec.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.ChkPhaseSpec.CheckedForeColor = System.Drawing.Color.Black;
            this.ChkPhaseSpec.CheckedSliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkPhaseSpec.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            this.ChkPhaseSpec.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ChkPhaseSpec.DropKey = System.Windows.Forms.Keys.Space;
            this.ChkPhaseSpec.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkPhaseSpec.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.ChkPhaseSpec.Height = 30;
            this.ChkPhaseSpec.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.ChkPhaseSpec.Location = new System.Drawing.Point(102, 6);
            this.ChkPhaseSpec.Name = "ChkPhaseSpec";
            this.ChkPhaseSpec.Size = new System.Drawing.Size(75, 30);
            this.ChkPhaseSpec.SliderButtonWidth = 30;
            this.ChkPhaseSpec.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkPhaseSpec.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.ChkPhaseSpec.StylizeFlag = false;
            this.ChkPhaseSpec.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            this.ChkPhaseSpec.TabIndex = 1;
            this.ChkPhaseSpec.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            this.ChkPhaseSpec.UseAnimation = true;
            this.ChkPhaseSpec.CheckedChangedEvent += new System.EventHandler(this.ChkPhaseVSFrequency_Click);
            // 
            // LblPhaseSpec
            // 
            this.LblPhaseSpec.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblPhaseSpec.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblPhaseSpec.BorderThickness = 0;
            this.LblPhaseSpec.CornerRadius = 0;
            this.LblPhaseSpec.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblPhaseSpec.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblPhaseSpec.HighlightPrompt = defaultHighlightPrompt1;
            this.LblPhaseSpec.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblPhaseSpec.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblPhaseSpec.Location = new System.Drawing.Point(10, 6);
            this.LblPhaseSpec.MultyLineFlag = false;
            this.LblPhaseSpec.Name = "LblPhaseSpec";
            this.LblPhaseSpec.Size = new System.Drawing.Size(75, 30);
            this.LblPhaseSpec.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblPhaseSpec.StylizeFlag = false;
            this.LblPhaseSpec.TabIndex = 0;
            this.LblPhaseSpec.TabStop = false;
            this.LblPhaseSpec.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiangWeiPu");
            this.LblPhaseSpec.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LblPhaseSpec.Token = null;
            // 
            // ChkWaterfall
            // 
            this.ChkWaterfall.AnimationCount = 8;
            this.ChkWaterfall.AnimationFunc = null;
            this.ChkWaterfall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkWaterfall.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkWaterfall.BorderThickness = 0;
            this.ChkWaterfall.Checked = false;
            this.ChkWaterfall.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.ChkWaterfall.CheckedForeColor = System.Drawing.Color.Black;
            this.ChkWaterfall.CheckedSliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkWaterfall.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            this.ChkWaterfall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ChkWaterfall.DropKey = System.Windows.Forms.Keys.Space;
            this.ChkWaterfall.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.ChkWaterfall.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.ChkWaterfall.Height = 30;
            this.ChkWaterfall.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.ChkWaterfall.Location = new System.Drawing.Point(319, 6);
            this.ChkWaterfall.Name = "ChkWaterfall";
            this.ChkWaterfall.Size = new System.Drawing.Size(75, 30);
            this.ChkWaterfall.SliderButtonWidth = 30;
            this.ChkWaterfall.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.ChkWaterfall.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.ChkWaterfall.StylizeFlag = false;
            this.ChkWaterfall.SwitchShape = ScopeX.UserControls.ScopeXSwitchButton.Shape.Square;
            this.ChkWaterfall.TabIndex = 4;
            this.ChkWaterfall.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            this.ChkWaterfall.UseAnimation = true;
            this.ChkWaterfall.CheckedChangedEvent += new System.EventHandler(this.ChkThreeD_Click);
            // 
            // LblWatefall
            // 
            this.LblWatefall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblWatefall.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblWatefall.BorderThickness = 0;
            this.LblWatefall.CornerRadius = 0;
            this.LblWatefall.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblWatefall.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblWatefall.HighlightPrompt = defaultHighlightPrompt2;
            this.LblWatefall.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblWatefall.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblWatefall.Location = new System.Drawing.Point(227, 6);
            this.LblWatefall.MultyLineFlag = false;
            this.LblWatefall.Name = "LblWatefall";
            this.LblWatefall.Size = new System.Drawing.Size(75, 30);
            this.LblWatefall.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblWatefall.StylizeFlag = false;
            this.LblWatefall.TabIndex = 3;
            this.LblWatefall.TabStop = false;
            this.LblWatefall.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PuBuTu");
            this.LblWatefall.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LblWatefall.Token = null;
            // 
            // GrpModulation
            // 
            this.GrpModulation.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.GrpModulation.BorderThickness = ((uint)(1u));
            this.GrpModulation.Controls.Add(this.ChkPhaseVSTime);
            this.GrpModulation.Controls.Add(this.ChkAmpVSTime);
            this.GrpModulation.Controls.Add(this.ChkFrequencyVSTime);
            this.GrpModulation.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GrpModulation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.GrpModulation.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.GrpModulation.Location = new System.Drawing.Point(10, 63);
            this.GrpModulation.Name = "GrpModulation";
            this.GrpModulation.Size = new System.Drawing.Size(438, 106);
            this.GrpModulation.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.GrpModulation.StylizeFlag = true;
            this.GrpModulation.TabIndex = 5;
            this.GrpModulation.TabStop = false;
            this.GrpModulation.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DiaoZhiBoXing");
            // 
            // OtherWfmPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.Controls.Add(this.GrpModulation);
            this.Controls.Add(this.ChkWaterfall);
            this.Controls.Add(this.LblWatefall);
            this.Controls.Add(this.ChkPhaseSpec);
            this.Controls.Add(this.LblPhaseSpec);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "OtherWfmPage";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(465, 184);
            this.GrpModulation.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox ChkPhaseVSTime;
        private System.Windows.Forms.CheckBox ChkFrequencyVSTime;
        private System.Windows.Forms.CheckBox ChkAmpVSTime;
        private ScopeX.UserControls.ScopeXSwitchButton ChkPhaseSpec;
        private ScopeX.UserControls.ScopeXLabel LblPhaseSpec;
        private ScopeX.UserControls.ScopeXSwitchButton ChkWaterfall;
        private ScopeX.UserControls.ScopeXLabel LblWatefall;
        private ScopeX.UserControls.ScopeXGroupBox GrpModulation;
    }
}
