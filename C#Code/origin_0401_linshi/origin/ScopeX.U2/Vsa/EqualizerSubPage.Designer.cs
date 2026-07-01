
namespace ScopeX.U2
{
    partial class EqualizerSubPage
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
            this.BtnGradient = new ScopeX.UserControls.ScopeXIconButton();
            this.LblGradient = new ScopeX.UserControls.ScopeXLabel();
            this.LblSymLength = new ScopeX.UserControls.ScopeXLabel();
            this.BtnSymLength = new ScopeX.UserControls.ScopeXIconButton();
            this.LblTapLength = new ScopeX.UserControls.ScopeXLabel();
            this.BtnTapLength = new ScopeX.UserControls.ScopeXIconButton();
            this.GrpEqualizer = new ScopeX.UserControls.ScopeXGroupBox();
            this.GrpEqualizer.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnGradient
            // 
            this.BtnGradient.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnGradient.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnGradient.BorderThickness = 0;
            this.BtnGradient.CornerRadius = 0;
            this.BtnGradient.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnGradient.DaskArray = null;
            this.BtnGradient.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnGradient.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnGradient.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnGradient.Icon = null;
            this.BtnGradient.IconOffset = 10;
            this.BtnGradient.IconSize = new System.Drawing.Size(24, 24);
            this.BtnGradient.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnGradient.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnGradient.Location = new System.Drawing.Point(27, 74);
            this.BtnGradient.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnGradient.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnGradient.MouseInBorderThickness = 0;
            this.BtnGradient.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnGradient.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnGradient.Name = "BtnGradient";
            this.BtnGradient.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnGradient.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnGradient.PressedBorderThickness = 0;
            this.BtnGradient.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnGradient.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnGradient.Size = new System.Drawing.Size(100, 30);
            this.BtnGradient.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnGradient.SVGPath = "";
            this.BtnGradient.TabIndex = 14;
            this.BtnGradient.Text = "1E-3";
            this.BtnGradient.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnGradient.Click += new System.EventHandler(this.BtnGradient_Click);
            // 
            // LblGradient
            // 
            this.LblGradient.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblGradient.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblGradient.BorderThickness = 0;
            this.LblGradient.CornerRadius = 0;
            this.LblGradient.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblGradient.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblGradient.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblGradient.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblGradient.Location = new System.Drawing.Point(27, 51);
            this.LblGradient.MultyLineFlag = false;
            this.LblGradient.Name = "LblGradient";
            this.LblGradient.Size = new System.Drawing.Size(100, 17);
            this.LblGradient.TabIndex = 13;
            this.LblGradient.TabStop = false;
            this.LblGradient.Text = "收敛阈值";
            this.LblGradient.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblSymLength
            // 
            this.LblSymLength.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblSymLength.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblSymLength.BorderThickness = 0;
            this.LblSymLength.CornerRadius = 0;
            this.LblSymLength.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblSymLength.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblSymLength.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblSymLength.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblSymLength.Location = new System.Drawing.Point(147, 51);
            this.LblSymLength.MultyLineFlag = false;
            this.LblSymLength.Name = "LblSymLength";
            this.LblSymLength.Size = new System.Drawing.Size(100, 17);
            this.LblSymLength.TabIndex = 13;
            this.LblSymLength.TabStop = false;
            this.LblSymLength.Text = "符号个数";
            this.LblSymLength.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BtnSymLength
            // 
            this.BtnSymLength.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSymLength.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSymLength.BorderThickness = 0;
            this.BtnSymLength.CornerRadius = 0;
            this.BtnSymLength.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSymLength.DaskArray = null;
            this.BtnSymLength.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnSymLength.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnSymLength.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSymLength.Icon = null;
            this.BtnSymLength.IconOffset = 10;
            this.BtnSymLength.IconSize = new System.Drawing.Size(24, 24);
            this.BtnSymLength.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnSymLength.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnSymLength.Location = new System.Drawing.Point(147, 74);
            this.BtnSymLength.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSymLength.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSymLength.MouseInBorderThickness = 0;
            this.BtnSymLength.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSymLength.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnSymLength.Name = "BtnSymLength";
            this.BtnSymLength.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnSymLength.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnSymLength.PressedBorderThickness = 0;
            this.BtnSymLength.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSymLength.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnSymLength.Size = new System.Drawing.Size(100, 30);
            this.BtnSymLength.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnSymLength.SVGPath = "";
            this.BtnSymLength.TabIndex = 14;
            this.BtnSymLength.Text = "100";
            this.BtnSymLength.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnSymLength.Click += new System.EventHandler(this.BtnSymLength_Click);
            // 
            // LblTapLength
            // 
            this.LblTapLength.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblTapLength.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblTapLength.BorderThickness = 0;
            this.LblTapLength.CornerRadius = 0;
            this.LblTapLength.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblTapLength.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblTapLength.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblTapLength.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblTapLength.Location = new System.Drawing.Point(267, 51);
            this.LblTapLength.MultyLineFlag = false;
            this.LblTapLength.Name = "LblTapLength";
            this.LblTapLength.Size = new System.Drawing.Size(100, 17);
            this.LblTapLength.TabIndex = 13;
            this.LblTapLength.TabStop = false;
            this.LblTapLength.Text = "抽头数";
            this.LblTapLength.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BtnTapLength
            // 
            this.BtnTapLength.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnTapLength.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnTapLength.BorderThickness = 0;
            this.BtnTapLength.CornerRadius = 0;
            this.BtnTapLength.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnTapLength.DaskArray = null;
            this.BtnTapLength.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnTapLength.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnTapLength.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnTapLength.Icon = null;
            this.BtnTapLength.IconOffset = 10;
            this.BtnTapLength.IconSize = new System.Drawing.Size(24, 24);
            this.BtnTapLength.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnTapLength.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnTapLength.Location = new System.Drawing.Point(267, 74);
            this.BtnTapLength.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnTapLength.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnTapLength.MouseInBorderThickness = 0;
            this.BtnTapLength.MouseinForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnTapLength.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnTapLength.Name = "BtnTapLength";
            this.BtnTapLength.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnTapLength.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnTapLength.PressedBorderThickness = 0;
            this.BtnTapLength.PressedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnTapLength.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnTapLength.Size = new System.Drawing.Size(100, 30);
            this.BtnTapLength.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnTapLength.SVGPath = "";
            this.BtnTapLength.TabIndex = 14;
            this.BtnTapLength.Text = "10";
            this.BtnTapLength.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnTapLength.Click += new System.EventHandler(this.BtnTapLength_Click);
            // 
            // GrpEqualizer
            // 
            this.GrpEqualizer.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.GrpEqualizer.Controls.Add(this.LblTapLength);
            this.GrpEqualizer.Controls.Add(this.BtnTapLength);
            this.GrpEqualizer.Controls.Add(this.LblSymLength);
            this.GrpEqualizer.Controls.Add(this.BtnGradient);
            this.GrpEqualizer.Controls.Add(this.BtnSymLength);
            this.GrpEqualizer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GrpEqualizer.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GrpEqualizer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.GrpEqualizer.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.GrpEqualizer.Location = new System.Drawing.Point(0, 0);
            this.GrpEqualizer.Name = "GrpEqualizer";
            this.GrpEqualizer.Size = new System.Drawing.Size(450, 150);
            this.GrpEqualizer.TabIndex = 24;
            this.GrpEqualizer.TabStop = false;
            this.GrpEqualizer.Text = "均衡设置";
            // 
            // EqualizerPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.Controls.Add(this.LblGradient);
            this.Controls.Add(this.GrpEqualizer);
            this.DoubleBuffered = true;
            this.Name = "EqualizerPage";
            this.Size = new System.Drawing.Size(450, 150);
            this.GrpEqualizer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ScopeX.UserControls.ScopeXIconButton BtnGradient;
        private ScopeX.UserControls.ScopeXLabel LblGradient;
        private ScopeX.UserControls.ScopeXLabel LblSymLength;
        private ScopeX.UserControls.ScopeXIconButton BtnSymLength;
        private ScopeX.UserControls.ScopeXLabel LblTapLength;
        private ScopeX.UserControls.ScopeXIconButton BtnTapLength;
        private ScopeX.UserControls.ScopeXGroupBox GrpEqualizer;
    }
}
