namespace ScopeX.U2
{
    partial class ScopeXCheckButton
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
            if(disposing)
            {
                if (Font != null)
                {
                    Font = null;
                }
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
            this.Tlp = new System.Windows.Forms.TableLayoutPanel();
            this.BtnCheck = new ScopeX.UserControls.ScopeXIconButton();
            this.BtnMain = new ScopeX.UserControls.ScopeXIconButton();
            this.Tlp.SuspendLayout();
            this.SuspendLayout();
            // 
            // Tlp
            // 
            this.Tlp.ColumnCount = 2;
            this.Tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.Tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.Tlp.Controls.Add(this.BtnCheck, 0, 0);
            this.Tlp.Controls.Add(this.BtnMain, 1, 0);
            this.Tlp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tlp.Location = new System.Drawing.Point(0, 0);
            this.Tlp.Name = "Tlp";
            this.Tlp.RowCount = 1;
            this.Tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Tlp.Size = new System.Drawing.Size(120, 30);
            this.Tlp.TabIndex = 1;
            // 
            // BtnCheck
            // 
            this.BtnCheck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnCheck.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnCheck.BorderThickness = 0;
            this.BtnCheck.CornerRadius = 0;
            this.BtnCheck.DaskArray = null;
            this.BtnCheck.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnCheck.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnCheck.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnCheck.ForeColor = System.Drawing.Color.White;
            this.BtnCheck.Height = 30;
            this.BtnCheck.Icon = global::ScopeX.U2.Properties.Resources.Check;
            this.BtnCheck.IconOffset = 10;
            this.BtnCheck.IconSize = new System.Drawing.Size(20, 20);
            this.BtnCheck.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnCheck.Location = new System.Drawing.Point(0, 0);
            this.BtnCheck.Margin = new System.Windows.Forms.Padding(0);
            this.BtnCheck.MouseinBackColor = System.Drawing.Color.Transparent;
            this.BtnCheck.MouseinBorderColor = System.Drawing.Color.Transparent;
            this.BtnCheck.MouseInBorderThickness = 0;
            this.BtnCheck.MouseinForeColor = System.Drawing.Color.Transparent;
            this.BtnCheck.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnCheck.Name = "BtnCheck";
            this.BtnCheck.PressedBackColor = System.Drawing.Color.Transparent;
            this.BtnCheck.PressedBorderColor = System.Drawing.Color.Transparent;
            this.BtnCheck.PressedBorderThickness = 0;
            this.BtnCheck.PressedForeColor = System.Drawing.Color.Transparent;
            this.BtnCheck.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnCheck.Size = new System.Drawing.Size(30, 30);
            this.BtnCheck.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnCheck.StylizeFlag = true;
            this.BtnCheck.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnCheck.SVGPath = "";
            this.BtnCheck.TabIndex = 48;
            this.BtnCheck.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BtnMain
            // 
            this.BtnMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnMain.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.BtnMain.BorderThickness = 0;
            this.BtnMain.CornerRadius = 0;
            this.BtnMain.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnMain.DaskArray = null;
            this.BtnMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnMain.DropKey = System.Windows.Forms.Keys.Space;
            this.BtnMain.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnMain.ForeColor = System.Drawing.Color.White;
            this.BtnMain.Height = 30;
            this.BtnMain.Icon = null;
            this.BtnMain.IconOffset = 10;
            this.BtnMain.IconSize = new System.Drawing.Size(24, 24);
            this.BtnMain.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.BtnMain.Location = new System.Drawing.Point(30, 0);
            this.BtnMain.Margin = new System.Windows.Forms.Padding(0);
            this.BtnMain.MouseinBackColor = System.Drawing.Color.Transparent;
            this.BtnMain.MouseinBorderColor = System.Drawing.Color.Transparent;
            this.BtnMain.MouseInBorderThickness = 0;
            this.BtnMain.MouseinForeColor = System.Drawing.Color.Transparent;
            this.BtnMain.MouseinSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnMain.Name = "BtnMain";
            this.BtnMain.PressedBackColor = System.Drawing.Color.Transparent;
            this.BtnMain.PressedBorderColor = System.Drawing.Color.Transparent;
            this.BtnMain.PressedBorderThickness = 0;
            this.BtnMain.PressedForeColor = System.Drawing.Color.Transparent;
            this.BtnMain.PressedSvgForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.BtnMain.Size = new System.Drawing.Size(90, 30);
            this.BtnMain.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.BtnMain.StylizeFlag = true;
            this.BtnMain.SVGForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.BtnMain.SVGPath = "";
            this.BtnMain.TabIndex = 47;
            this.BtnMain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UCCheckButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.Controls.Add(this.Tlp);
            this.Name = "UCCheckButton";
            this.Size = new System.Drawing.Size(120, 30);
            this.Tlp.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel Tlp;
        private ScopeX.UserControls.ScopeXIconButton BtnMain;
        private ScopeX.UserControls.ScopeXIconButton BtnCheck;
    }
}
