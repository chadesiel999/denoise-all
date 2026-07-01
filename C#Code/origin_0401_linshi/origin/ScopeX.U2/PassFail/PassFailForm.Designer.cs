
namespace ScopeX.U2.PassFail
{
    partial class PassFailForm
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
            NbgAnalog = new UserControls.NavBarGroup();
            SuspendLayout();
            // 
            // NbgAnalog
            // 
            NbgAnalog.AssignHelper = "";
            NbgAnalog.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            NbgAnalog.CurrentGroupIndex = 0;
            NbgAnalog.Dock = System.Windows.Forms.DockStyle.Fill;
            groupItem1.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem1.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem1.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem1.GroupSize = new System.Drawing.Size(0, 0);
            groupItem1.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CaoZuo"); // "操作";
            groupItem2.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem2.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem2.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem2.GroupSize = new System.Drawing.Size(0, 0);
            groupItem2.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BaoCunSheZhi"); 
            NbgAnalog.GroupItems = (new UserControls.GroupItem[] { groupItem1, groupItem2 });
            NbgAnalog.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            NbgAnalog.Location = new System.Drawing.Point(3, 46);
            NbgAnalog.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            NbgAnalog.Name = "NbgAnalog";
            NbgAnalog.NavBarColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NbgAnalog.NavBarHeight = 40;
            NbgAnalog.NavForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            NbgAnalog.NavGroupColor = System.Drawing.Color.FromArgb(41, 42, 45);
            NbgAnalog.NavGroupHeight = 381;
            NbgAnalog.Size = new System.Drawing.Size(572, 461);
            NbgAnalog.SplitColor = System.Drawing.Color.Empty;
            NbgAnalog.StyleFlags = UserControls.Style.StyleFlag.None;
            NbgAnalog.StylizeFlag = true;
            NbgAnalog.TabIndex = 0;
            // 
            // PassFailForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(500, 620);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(NbgAnalog);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HeadHeight = 39;
            HelpLabel = "22";
            IconInterval = 21;
            IconWidth = 26;
            Margin = new System.Windows.Forms.Padding(24, 16, 24, 16);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PassFailForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TongGuoShiBaiCeShi");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TongGuoShiBaiCeShi");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Controls.SetChildIndex(NbgAnalog, 0);
            ResumeLayout(false);
        }

        #endregion
        private ScopeX.UserControls.NavBarGroup NbgAnalog;
    }
}
