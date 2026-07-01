
namespace ScopeX.U2
{
    partial class DemoSetForm
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
            NbgDemo = new UserControls.NavBarGroup();
            SuspendLayout();
            // 
            // NbgDemo
            // 
            NbgDemo.AssignHelper = "";
            NbgDemo.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            NbgDemo.CurrentGroupIndex = 0;
            NbgDemo.Dock = System.Windows.Forms.DockStyle.Fill;
            groupItem1.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem1.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem1.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem1.GroupSize = new System.Drawing.Size(0, 0);
            groupItem1.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYuanFenXi");
            groupItem2.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem2.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem2.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem2.GroupSize = new System.Drawing.Size(0, 0);
            groupItem2.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QiTa"); 
            NbgDemo.GroupItems = (new UserControls.GroupItem[] { groupItem1, groupItem2 });
            NbgDemo.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            NbgDemo.Location = new System.Drawing.Point(3, 46);
            NbgDemo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            NbgDemo.Name = "NbgDemo";
            NbgDemo.NavBarColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NbgDemo.NavBarHeight = 40;
            NbgDemo.NavForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            NbgDemo.NavGroupColor = System.Drawing.Color.FromArgb(41, 42, 45);
            NbgDemo.NavGroupHeight = 306;
            NbgDemo.Size = new System.Drawing.Size(494, 388);
            NbgDemo.SplitColor = System.Drawing.Color.Empty;
            NbgDemo.StyleFlags = UserControls.Style.StyleFlag.None;
            NbgDemo.StylizeFlag = true;
            NbgDemo.TabIndex = 5;
            // 
            // DemoSetForm
            // 
            ActiveBorderVisiable = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(500, 437);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(NbgDemo);
            DoubleBuffered = true;
            FormOpacity = 95;
            HelpLabel = "22";
            IconInterval = 21;
            IconWidth = 26;
            IndicatorColor = System.Drawing.Color.Lime;
            IndicatorMargin = new System.Windows.Forms.Padding(0, 0, 0, 42);
            IsIndicatorShow = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DemoSetForm";
            Padding = new System.Windows.Forms.Padding(1);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            StylizeFlag = true;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YanShi");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YanShi");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Controls.SetChildIndex(NbgDemo, 0);
            ResumeLayout(false);
        }

        #endregion

        private UserControls.NavBarGroup NbgDemo;
    }
}
