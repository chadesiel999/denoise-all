
namespace ScopeX.U2
{
    partial class CascadeSettingForm
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
            UserControls.GroupItem groupItem1 = new UserControls.GroupItem();
            UserControls.GroupItem groupItem2 = new UserControls.GroupItem();
            NbgCascade = new UserControls.NavBarGroup();
            SuspendLayout();
            // 
            // NbgCascade
            // 
            NbgCascade.AssignHelper = "";
            NbgCascade.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            NbgCascade.CurrentGroupIndex = 0;
            NbgCascade.Dock = System.Windows.Forms.DockStyle.Fill;
            groupItem1.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem1.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem1.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem1.GroupSize = new System.Drawing.Size(0, 0);
            groupItem1.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiJian");
            groupItem2.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem2.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem2.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem2.GroupSize = new System.Drawing.Size(0, 0);
            groupItem2.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YanChi");
            NbgCascade.GroupItems = (new UserControls.GroupItem[] { groupItem1, groupItem2 });
            NbgCascade.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            NbgCascade.Location = new System.Drawing.Point(2, 45);
            NbgCascade.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            NbgCascade.Name = "NbgCascade";
            NbgCascade.NavBarColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NbgCascade.NavBarHeight = 40;
            NbgCascade.NavForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            NbgCascade.NavGroupColor = System.Drawing.Color.FromArgb(41, 42, 45);
            NbgCascade.NavGroupHeight = 281;
            NbgCascade.Size = new System.Drawing.Size(517, 363);
            NbgCascade.SplitColor = System.Drawing.Color.Empty;
            NbgCascade.StyleFlags = UserControls.Style.StyleFlag.None;
            NbgCascade.StylizeFlag = true;
            NbgCascade.TabIndex = 0;
            // 
            // CascadeSettingForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(521, 410);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(NbgCascade);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            HelpLabel = "16";
            IconInterval = 21;
            IconWidth = 26;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CascadeSettingForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PeiZhi");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PeiZhi");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Controls.SetChildIndex(NbgCascade, 0);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.NavBarGroup NbgCascade;
    }
}
