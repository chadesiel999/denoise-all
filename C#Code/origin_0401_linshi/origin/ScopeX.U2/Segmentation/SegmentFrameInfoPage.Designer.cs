namespace ScopeX.U2
{
    partial class SegmentFrameInfoPage
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
            this.BtnFramesInfo = new ScopeX.UserControls.ScopeXIconButton();
            this.SuspendLayout();
            // 
            // BtnFramesInfo
            // 
            this.BtnFramesInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnFramesInfo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.BtnFramesInfo.BorderThickness = 1;
            this.BtnFramesInfo.CornerRadius = 0;
            this.BtnFramesInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnFramesInfo.DaskArray = new int[] {
        4,
        4};
            this.BtnFramesInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnFramesInfo.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnFramesInfo.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnFramesInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnFramesInfo.Height = 45;
            this.BtnFramesInfo.Icon = null;
            this.BtnFramesInfo.IconOffset = 10;
            this.BtnFramesInfo.IconSize = new System.Drawing.Size(24, 24);
            this.BtnFramesInfo.IndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnFramesInfo.IsIndicatorShow = false;
            this.BtnFramesInfo.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnFramesInfo.Location = new System.Drawing.Point(0, 0);
            this.BtnFramesInfo.Margin = new System.Windows.Forms.Padding(0);
            this.BtnFramesInfo.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnFramesInfo.MouseinBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.BtnFramesInfo.MouseInBorderThickness = 1;
            this.BtnFramesInfo.MouseinForeColor = System.Drawing.Color.White;
            this.BtnFramesInfo.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnFramesInfo.Name = "BtnFramesInfo";
            this.BtnFramesInfo.PressedBackColor = System.Drawing.Color.Gray;
            this.BtnFramesInfo.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.BtnFramesInfo.PressedBorderThickness = 1;
            this.BtnFramesInfo.PressedForeColor = System.Drawing.Color.White;
            this.BtnFramesInfo.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnFramesInfo.Size = new System.Drawing.Size(62, 45);
            this.BtnFramesInfo.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnFramesInfo.StylizeFlag = true;
            this.BtnFramesInfo.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnFramesInfo.SVGPath = "";
            this.BtnFramesInfo.TabIndex = 55;
            this.BtnFramesInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnFramesInfo.Click += new System.EventHandler(this.BtnFramesInfo_Click);
            // 
            // SegmentFrameInfoPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.Controls.Add(this.BtnFramesInfo);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SegmentFrameInfoPage";
            this.Size = new System.Drawing.Size(62, 45);
            this.ResumeLayout(false);

        }

        #endregion

        private ScopeX.UserControls.ScopeXIconButton BtnFramesInfo;
    }
}
