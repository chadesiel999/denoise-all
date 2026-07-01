
namespace ScopeX.U2
{
    partial class MathTrackSubPage
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
            CbxSource = new ScopeX.UserControls.ComboBoxEx();
            LblSource = new ScopeX.UserControls.ScopeXLabel();
            BtnMeasItem = new ScopeX.UserControls.ScopeXIconButton();
            SuspendLayout();
            // 
            // CbxSource
            // 
            CbxSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CbxSource.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.BorderThickness = 0;
            CbxSource.CornerRadius = 0;
            CbxSource.DataSource = null;
            CbxSource.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            CbxSource.DropDownHeight = 200;
            CbxSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CbxSource.DropDownWidth = 100;
            CbxSource.DropKey = System.Windows.Forms.Keys.Space;
            CbxSource.DroppedDown = false;
            CbxSource.FocusColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxSource.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            CbxSource.FormattingEnabled = true;
            CbxSource.GetDisPlayName = null;
            CbxSource.Height = 30;
            CbxSource.ImageMode = false;
            CbxSource.ItemHeight = 28;
            CbxSource.Items.AddRange(new object[] { "P1", "P2", "P3", "P4", "P5", "P6", "P7", "P8", "P9", "P10" });
            CbxSource.KeyDropEnble = true;
            CbxSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            CbxSource.Location = new System.Drawing.Point(10, 31);
            CbxSource.MaxDropDownItems = 8;
            CbxSource.Name = "CbxSource";
            CbxSource.RectBtnWidth = 20;
            CbxSource.SelectedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            CbxSource.SelectedIndex = -1;
            CbxSource.SelectedItem = null;
            CbxSource.SelectedText = "";
            CbxSource.Size = new System.Drawing.Size(100, 30);
            CbxSource.Soreted = false;
            CbxSource.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            CbxSource.StylizeFlag = true;
            CbxSource.TabIndex = 1;
            CbxSource.Text = "";
            CbxSource.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            CbxSource.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            CbxSource.Visible = false;
            CbxSource.SelectedIndexChanged += CbxSource_SelectedIndexChanged;
            // 
            // LblSource
            // 
            LblSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            LblSource.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblSource.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblSource.BorderThickness = 0;
            LblSource.CornerRadius = 0;
            LblSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSource.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblSource.HighlightPrompt = defaultHighlightPrompt1;
            LblSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSource.Location = new System.Drawing.Point(10, 3);
            LblSource.MultyLineFlag = false;
            LblSource.Name = "LblSource";
            LblSource.Size = new System.Drawing.Size(100, 18);
            LblSource.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblSource.StylizeFlag = true;
            LblSource.TabIndex = 0;
            LblSource.TabStop = false;
            LblSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Yuan");
            LblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSource.Token = null;
            LblSource.Visible = false;
            // 
            // BtnMeasItem
            // 
            BtnMeasItem.Anchor = System.Windows.Forms.AnchorStyles.Left;
            BtnMeasItem.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnMeasItem.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnMeasItem.BorderThickness = 0;
            BtnMeasItem.CornerRadius = 0;
            BtnMeasItem.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnMeasItem.DaskArray = null;
            BtnMeasItem.DropKey = System.Windows.Forms.Keys.Space;
            BtnMeasItem.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnMeasItem.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnMeasItem.Height = 30;
            BtnMeasItem.Icon = null;
            BtnMeasItem.IconOffset = 10;
            BtnMeasItem.IconSize = new System.Drawing.Size(24, 24);
            BtnMeasItem.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnMeasItem.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnMeasItem.IsIndicatorShow = false;
            BtnMeasItem.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnMeasItem.Location = new System.Drawing.Point(10, 31);
            BtnMeasItem.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnMeasItem.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnMeasItem.MouseInBorderThickness = 0;
            BtnMeasItem.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnMeasItem.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnMeasItem.Name = "BtnMeasItem";
            BtnMeasItem.PressedBackColor = System.Drawing.Color.Gray;
            BtnMeasItem.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnMeasItem.PressedBorderThickness = 0;
            BtnMeasItem.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnMeasItem.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnMeasItem.Size = new System.Drawing.Size(157, 30);
            BtnMeasItem.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnMeasItem.StylizeFlag = true;
            BtnMeasItem.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnMeasItem.SVGPath = "";
            BtnMeasItem.TabIndex = 2;
            BtnMeasItem.Text = "P1";
            BtnMeasItem.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnMeasItem.Click += BtnMeasItem_Click;
            // 
            // MathTrackSubPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(BtnMeasItem);
            Controls.Add(CbxSource);
            Controls.Add(LblSource);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "MathTrackSubPage";
            Size = new System.Drawing.Size(400, 85);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ComboBoxEx CbxSource;
        private ScopeX.UserControls.ScopeXLabel LblSource;
        private ScopeX.UserControls.ScopeXIconButton BtnMeasItem;
    }
}
