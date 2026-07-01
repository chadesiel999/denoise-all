
namespace ScopeX.U2
{
    partial class GeneralDigtalSettingPage
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
            ScopeX.UserControls.GroupItem groupItem1 = new ScopeX.UserControls.GroupItem();
            ScopeX.UserControls.GroupItem groupItem2 = new ScopeX.UserControls.GroupItem();
            ScopeX.UserControls.GroupItem groupItem3 = new ScopeX.UserControls.GroupItem();
            ScopeX.UserControls.GroupItem groupItem4 = new ScopeX.UserControls.GroupItem();
            this.NbgVsa = new ScopeX.UserControls.NavBarGroup();
            this.SuspendLayout();
            // 
            // NbgVsa
            // 
            this.NbgVsa.AssignHelper = "";
            this.NbgVsa.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.NbgVsa.CurrentGroupIndex = 0;
            this.NbgVsa.Dock = System.Windows.Forms.DockStyle.Fill;
            groupItem1.BackGroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            groupItem1.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            groupItem1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem1.FontColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            groupItem1.GroupSize = new System.Drawing.Size(0, 0);
            groupItem1.Title = "调制参数";
            groupItem2.BackGroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            groupItem2.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            groupItem2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem2.FontColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            groupItem2.GroupSize = new System.Drawing.Size(0, 0);
            groupItem2.Title = "频率和带宽参数";
            groupItem3.BackGroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            groupItem3.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            groupItem3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem3.FontColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            groupItem3.GroupSize = new System.Drawing.Size(0, 0);
            groupItem3.Title = "均衡";
            groupItem4.BackGroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            groupItem4.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            groupItem4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem4.FontColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            groupItem4.GroupSize = new System.Drawing.Size(0, 0);
            groupItem4.Title = "算法选择";
            this.NbgVsa.GroupItems = new ScopeX.UserControls.GroupItem[] {
        groupItem1,
        groupItem2,
        groupItem3,
        groupItem4};
            this.NbgVsa.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.NbgVsa.Location = new System.Drawing.Point(0, 0);
            this.NbgVsa.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.NbgVsa.Name = "NbgVsa";
            this.NbgVsa.NavBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.NbgVsa.NavBarHeight = 40;
            this.NbgVsa.NavForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.NbgVsa.NavGroupColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.NbgVsa.NavGroupHeight = 387;
            this.NbgVsa.Size = new System.Drawing.Size(480, 547);
            this.NbgVsa.SplitColor = System.Drawing.Color.Empty;
            this.NbgVsa.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.NbgVsa.StylizeFlag = false;
            this.NbgVsa.TabIndex = 7;
            // 
            // GeneralDigtalSettingPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.NbgVsa);
            this.Name = "GeneralDigtalSettingPage";
            this.Size = new System.Drawing.Size(480, 547);
            this.ResumeLayout(false);

        }

        #endregion

        private ScopeX.UserControls.NavBarGroup NbgVsa;
    }
}
