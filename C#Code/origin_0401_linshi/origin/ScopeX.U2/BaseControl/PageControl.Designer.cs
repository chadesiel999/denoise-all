using System.Drawing;
using ScopeX.U2.Properties;

namespace ScopeX.U2.BaseControl
{
    partial class PageControl
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
            BtnFirstPage = new UserControls.ScopeXIconButton();
            BtnPreviousPage = new UserControls.ScopeXIconButton();
            LblCurrentPageNum = new UserControls.ScopeXLabel();
            BtnNextPage = new UserControls.ScopeXIconButton();
            BtnLastPage = new UserControls.ScopeXIconButton();
            SuspendLayout();
            // 
            // BtnFirstPage
            // 
            BtnFirstPage.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            BtnFirstPage.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnFirstPage.BorderThickness = 1;
            BtnFirstPage.CornerRadius = 0;
            BtnFirstPage.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnFirstPage.DaskArray = null;
            BtnFirstPage.DropKey = System.Windows.Forms.Keys.Space;
            BtnFirstPage.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnFirstPage.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnFirstPage.Height = 30;
            BtnFirstPage.Icon = Resources.First_Enable;
            BtnFirstPage.IconOffset = 21;
            BtnFirstPage.IconSize = new System.Drawing.Size(28, 28);
            BtnFirstPage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnFirstPage.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnFirstPage.IsIndicatorShow = false;
            BtnFirstPage.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnFirstPage.Location = new System.Drawing.Point(310, 2);
            BtnFirstPage.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnFirstPage.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnFirstPage.MouseInBorderThickness = 0;
            BtnFirstPage.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnFirstPage.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnFirstPage.Name = "BtnFirstPage";
            BtnFirstPage.PressedBackColor = System.Drawing.Color.Gray;
            BtnFirstPage.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnFirstPage.PressedBorderThickness = 0;
            BtnFirstPage.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnFirstPage.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnFirstPage.Size = new System.Drawing.Size(50, 31);
            BtnFirstPage.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnFirstPage.StylizeFlag = true;
            BtnFirstPage.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnFirstPage.SVGPath = "";
            BtnFirstPage.TabIndex = 34;
            BtnFirstPage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnFirstPage.Click += Btn_Click;
            // 
            // BtnPreviousPage
            // 
            BtnPreviousPage.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            BtnPreviousPage.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnPreviousPage.BorderThickness = 1;
            BtnPreviousPage.CornerRadius = 0;
            BtnPreviousPage.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnPreviousPage.DaskArray = null;
            BtnPreviousPage.DropKey = System.Windows.Forms.Keys.Space;
            BtnPreviousPage.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnPreviousPage.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnPreviousPage.Height = 30;
            BtnPreviousPage.Icon = Resources.Previous_Enable;
            BtnPreviousPage.IconOffset = 21;
            BtnPreviousPage.IconSize = new System.Drawing.Size(28, 28);
            BtnPreviousPage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnPreviousPage.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnPreviousPage.IsIndicatorShow = false;
            BtnPreviousPage.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnPreviousPage.Location = new System.Drawing.Point(350, 2);
            BtnPreviousPage.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnPreviousPage.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnPreviousPage.MouseInBorderThickness = 0;
            BtnPreviousPage.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnPreviousPage.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnPreviousPage.Name = "BtnPreviousPage";
            BtnPreviousPage.PressedBackColor = System.Drawing.Color.Gray;
            BtnPreviousPage.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnPreviousPage.PressedBorderThickness = 0;
            BtnPreviousPage.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnPreviousPage.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnPreviousPage.Size = new System.Drawing.Size(50, 31);
            BtnPreviousPage.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnPreviousPage.StylizeFlag = true;
            BtnPreviousPage.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnPreviousPage.SVGPath = "";
            BtnPreviousPage.TabIndex = 34;
            BtnPreviousPage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnPreviousPage.Click += Btn_Click;
            // 
            // TbxCurrentPageNum
            // 
            LblCurrentPageNum.AutoSize = true;
            LblCurrentPageNum.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblCurrentPageNum.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblCurrentPageNum.BorderThickness = 1;
            LblCurrentPageNum.CornerRadius = 0;
            LblCurrentPageNum.Cursor = System.Windows.Forms.Cursors.IBeam;
            LblCurrentPageNum.Enabled = true;
            LblCurrentPageNum.Font =  new Font("MiSans", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            LblCurrentPageNum.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            LblCurrentPageNum.Height = 30;
            LblCurrentPageNum.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblCurrentPageNum.Location = new System.Drawing.Point(385,7);
            LblCurrentPageNum.Name = "TbxCurrentPageNum";
            LblCurrentPageNum.Size = new System.Drawing.Size(50, 28);
            LblCurrentPageNum.StyleFlags = UserControls.Style.StyleFlag.None;
            LblCurrentPageNum.StylizeFlag = true;
            LblCurrentPageNum.TabIndex = 35;
            LblCurrentPageNum.TextAlign =  ContentAlignment.MiddleCenter;
            LblCurrentPageNum.Click += TbxCurrentPageNum_Click;
            // 
            // BtnNextPage
            // 
            BtnNextPage.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            BtnNextPage.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnNextPage.BorderThickness = 1;
            BtnNextPage.CornerRadius = 0;
            BtnNextPage.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnNextPage.DaskArray = null;
            BtnNextPage.DropKey = System.Windows.Forms.Keys.Space;
            BtnNextPage.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnNextPage.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnNextPage.Height = 30;
            BtnNextPage.Icon = Resources.Next_Enable;
            BtnNextPage.IconOffset = 21;
            BtnNextPage.IconSize = new System.Drawing.Size(28, 28);
            BtnNextPage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnNextPage.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnNextPage.IsIndicatorShow = false;
            BtnNextPage.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnNextPage.Location = new System.Drawing.Point(440,2);
            BtnNextPage.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnNextPage.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnNextPage.MouseInBorderThickness = 0;
            BtnNextPage.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnNextPage.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnNextPage.Name = "BtnNextPage";
            BtnNextPage.PressedBackColor = System.Drawing.Color.Gray;
            BtnNextPage.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnNextPage.PressedBorderThickness = 0;
            BtnNextPage.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnNextPage.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnNextPage.Size = new System.Drawing.Size(50, 31);
            BtnNextPage.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnNextPage.StylizeFlag = true;
            BtnNextPage.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnNextPage.SVGPath = "";
            BtnNextPage.TabIndex = 34;
            BtnNextPage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnNextPage.Click += Btn_Click;
            // 
            // BtnLastPage
            // 
            BtnLastPage.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            BtnLastPage.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnLastPage.BorderThickness = 1;
            BtnLastPage.CornerRadius = 0;
            BtnLastPage.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnLastPage.DaskArray = null;
            BtnLastPage.DropKey = System.Windows.Forms.Keys.Space;
            BtnLastPage.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnLastPage.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnLastPage.Height = 30;
            BtnLastPage.Icon = Resources.Last_Enable;
            BtnLastPage.IconOffset = 21;
            BtnLastPage.IconSize = new System.Drawing.Size(28, 28);
            BtnLastPage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnLastPage.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnLastPage.IsIndicatorShow = false;
            BtnLastPage.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnLastPage.Location = new System.Drawing.Point(470, 2);
            BtnLastPage.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnLastPage.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnLastPage.MouseInBorderThickness = 0;
            BtnLastPage.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnLastPage.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnLastPage.Name = "BtnLastPage";
            BtnLastPage.PressedBackColor = System.Drawing.Color.Gray;
            BtnLastPage.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnLastPage.PressedBorderThickness = 0;
            BtnLastPage.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnLastPage.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnLastPage.Size = new System.Drawing.Size(50, 31);
            BtnLastPage.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnLastPage.StylizeFlag = true;
            BtnLastPage.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnLastPage.SVGPath = "";
            BtnLastPage.TabIndex = 34;
            BtnLastPage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnLastPage.Click += Btn_Click;
            // 
            // PageControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ForeColor = System.Drawing.Color.White;
            MaximumSize = new System.Drawing.Size(2000, 30);
            MinimumSize = new System.Drawing.Size(0, 30);
            Controls.Add(BtnFirstPage);
            Controls.Add(BtnPreviousPage);
            Controls.Add(LblCurrentPageNum);
            Controls.Add(BtnNextPage);
            Controls.Add(BtnLastPage);
            Name = "PageControl";
            Font= new Font("MiSans", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            Size = new System.Drawing.Size(500, 30);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXIconButton BtnFirstPage;
        private ScopeX.UserControls.ScopeXIconButton BtnPreviousPage;
        private ScopeX.UserControls.ScopeXLabel LblCurrentPageNum;
        private ScopeX.UserControls.ScopeXIconButton BtnNextPage;
        private ScopeX.UserControls.ScopeXIconButton BtnLastPage;
    }
}
