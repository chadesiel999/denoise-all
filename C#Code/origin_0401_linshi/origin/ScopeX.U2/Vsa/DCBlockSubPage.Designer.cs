
namespace ScopeX.U2
{
    partial class DCBlockSubPage
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
            ScopeX.UserControls.RadioButtonItem radioButtonItem1 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem2 = new ScopeX.UserControls.RadioButtonItem();
            this.LblDCBlockMode = new ScopeX.UserControls.ScopeXLabel();
            this.RdoDCBlockMode = new ScopeX.UserControls.UIRadioButtonGroup();
            this.GrpDCBlock = new ScopeX.UserControls.ScopeXGroupBox();
            this.SuspendLayout();
            // 
            // LblDCBlockMode
            // 
            this.LblDCBlockMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.LblDCBlockMode.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblDCBlockMode.BorderThickness = 0;
            this.LblDCBlockMode.CornerRadius = 0;
            this.LblDCBlockMode.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblDCBlockMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblDCBlockMode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LblDCBlockMode.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblDCBlockMode.Location = new System.Drawing.Point(27, 39);
            this.LblDCBlockMode.MultyLineFlag = false;
            this.LblDCBlockMode.Name = "LblDCBlockMode";
            this.LblDCBlockMode.Size = new System.Drawing.Size(150, 17);
            this.LblDCBlockMode.TabIndex = 10;
            this.LblDCBlockMode.TabStop = false;
            this.LblDCBlockMode.Text = "模式";
            this.LblDCBlockMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RdoDCBlockMode
            // 
            this.RdoDCBlockMode.BackColor = System.Drawing.Color.Black;
            this.RdoDCBlockMode.BorderColor = System.Drawing.Color.Black;
            this.RdoDCBlockMode.BorderThickness = 0;
            this.RdoDCBlockMode.ButtonBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.RdoDCBlockMode.ButtonFont = null;
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = "模拟";
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = "数字";
            this.RdoDCBlockMode.ButtonItems = new ScopeX.UserControls.RadioButtonItem[] {
        radioButtonItem1,
        radioButtonItem2};
            this.RdoDCBlockMode.ButtonOffset = 10;
            this.RdoDCBlockMode.ButtonTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.RdoDCBlockMode.ChoosedButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.RdoDCBlockMode.ChoosedButtonIndex = 0;
            this.RdoDCBlockMode.ChoosedButtonTextColor = System.Drawing.Color.Black;
            this.RdoDCBlockMode.ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.RdoDCBlockMode.ContentPadding = new System.Windows.Forms.Padding(0);
            this.RdoDCBlockMode.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.RdoDCBlockMode.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.RdoDCBlockMode.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.RdoDCBlockMode.Location = new System.Drawing.Point(27, 62);
            this.RdoDCBlockMode.Margin = new System.Windows.Forms.Padding(0);
            this.RdoDCBlockMode.Name = "RdoDCBlockMode";
            this.RdoDCBlockMode.Size = new System.Drawing.Size(150, 30);
            this.RdoDCBlockMode.TabIndex = 11;
            this.RdoDCBlockMode.IndexChanged += new System.EventHandler(this.RdoDCBlockMode_IndexChanged);
            // 
            // GrpDCBlock
            // 
            this.GrpDCBlock.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.GrpDCBlock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GrpDCBlock.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GrpDCBlock.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.GrpDCBlock.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.GrpDCBlock.Location = new System.Drawing.Point(0, 0);
            this.GrpDCBlock.Name = "GrpDCBlock";
            this.GrpDCBlock.Size = new System.Drawing.Size(450, 150);
            this.GrpDCBlock.TabIndex = 25;
            this.GrpDCBlock.TabStop = false;
            this.GrpDCBlock.Text = "隔直设置";
            // 
            // DCBlockPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.Controls.Add(this.LblDCBlockMode);
            this.Controls.Add(this.RdoDCBlockMode);
            this.Controls.Add(this.GrpDCBlock);
            this.DoubleBuffered = true;
            this.Name = "DCBlockPage";
            this.Size = new System.Drawing.Size(450, 150);
            this.ResumeLayout(false);

        }

        #endregion

        private ScopeX.UserControls.ScopeXLabel LblDCBlockMode;
        private ScopeX.UserControls.UIRadioButtonGroup RdoDCBlockMode;
        private ScopeX.UserControls.ScopeXGroupBox GrpDCBlock;
    }
}
