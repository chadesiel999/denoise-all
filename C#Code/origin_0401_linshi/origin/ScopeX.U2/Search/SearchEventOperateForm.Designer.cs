
namespace ScopeX.U2
{
    partial class SearchEventOperateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ScopeX.UserControls.GroupItem groupItem1 = new ScopeX.UserControls.GroupItem();
            ScopeX.UserControls.GroupItem groupItem2 = new ScopeX.UserControls.GroupItem();
            NbgSearchEvent = new ScopeX.UserControls.NavBarGroup();
            SuspendLayout();
            // 
            // NbgSearchEvent
            // 
            NbgSearchEvent.AssignHelper = "";
            NbgSearchEvent.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            NbgSearchEvent.CurrentGroupIndex = 0;
            NbgSearchEvent.Dock = System.Windows.Forms.DockStyle.Fill;
            groupItem1.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem1.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem1.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem1.GroupSize = new System.Drawing.Size(0, 0);
            groupItem1.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YaoCaiQuDeCaoZuo");
            groupItem2.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem2.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem2.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem2.GroupSize = new System.Drawing.Size(0, 0);
            groupItem2.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BaoCunPeiZhi");
            NbgSearchEvent.GroupItems = (new ScopeX.UserControls.GroupItem[] { groupItem1, groupItem2 });
            NbgSearchEvent.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            NbgSearchEvent.Location = new System.Drawing.Point(2, 45);
            NbgSearchEvent.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            NbgSearchEvent.Name = "NbgSearchEvent";
            NbgSearchEvent.NavBarColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NbgSearchEvent.NavBarHeight = 40;
            NbgSearchEvent.NavForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            NbgSearchEvent.NavGroupColor = System.Drawing.Color.FromArgb(41, 42, 45);
            NbgSearchEvent.NavGroupHeight = 381;
            NbgSearchEvent.Size = new System.Drawing.Size(574, 463);
            NbgSearchEvent.SplitColor = System.Drawing.Color.Empty;
            NbgSearchEvent.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            NbgSearchEvent.StylizeFlag = true;
            NbgSearchEvent.TabIndex = 0;
            // 
            // SearchEventOperateForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(578, 510);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(NbgSearchEvent);
            DoubleBuffered = true;
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            HeadHeight = 45;
            IconInterval = 21;
            IconWidth = 28;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SearchEventOperateForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            StylizeFlag = true;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiJianCaoZuo");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiJianCaoZuo");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleLableHeight = 39;
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(28, 26);
            Controls.SetChildIndex(NbgSearchEvent, 0);
            ResumeLayout(false);
        }

        #endregion
        private ScopeX.UserControls.NavBarGroup NbgSearchEvent;
    }
}