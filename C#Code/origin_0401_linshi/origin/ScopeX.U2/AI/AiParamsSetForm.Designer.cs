namespace ScopeX.U2
{
    partial class AiParamsSetForm
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
            ScopeX.UserControls.GroupItem groupItem3 = new ScopeX.UserControls.GroupItem();
            ScopeX.UserControls.GroupItem groupItem4 = new ScopeX.UserControls.GroupItem();
            ScopeX.UserControls.GroupItem groupItem5 = new ScopeX.UserControls.GroupItem();
            ScopeX.UserControls.GroupItem groupItem6 = new ScopeX.UserControls.GroupItem();
            NbgAiSet = new ScopeX.UserControls.NavBarGroup();
            SuspendLayout();
            // 
            // NbgAiSet
            // 
            NbgAiSet.AssignHelper = "";
            NbgAiSet.BackColor = System.Drawing.Color.FromArgb(39, 41, 48);
            NbgAiSet.CurrentGroupIndex = 0;
            NbgAiSet.Dock = System.Windows.Forms.DockStyle.Fill;
            groupItem1.BackGroundColor = System.Drawing.Color.Empty;
            groupItem1.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem1.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem1.GroupSize = new System.Drawing.Size(0, 0);
            groupItem1.Title = "智能图形图表";
            groupItem2.BackGroundColor = System.Drawing.Color.Empty;
            groupItem2.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem2.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem2.GroupSize = new System.Drawing.Size(0, 0);
            groupItem2.Title = "智能采集";
            groupItem3.BackGroundColor = System.Drawing.Color.Empty;
            groupItem3.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem3.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem3.GroupSize = new System.Drawing.Size(0, 0);
            groupItem3.Title = "智能降噪";
            //groupItem4.BackGroundColor = System.Drawing.Color.Empty;
            //groupItem4.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            //groupItem4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            //groupItem4.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            //groupItem4.GroupSize = new System.Drawing.Size(0, 0);
            //groupItem4.Title = "异常捕获";
            groupItem5.BackGroundColor = System.Drawing.Color.Empty;
            groupItem5.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem5.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem5.GroupSize = new System.Drawing.Size(0, 0);
            groupItem5.Title = "专注捕获";
            groupItem6.BackGroundColor = System.Drawing.Color.Empty;
            groupItem6.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem6.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem6.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem6.GroupSize = new System.Drawing.Size(0, 0);
            groupItem6.Title = "一键设置";
            NbgAiSet.GroupItems = new ScopeX.UserControls.GroupItem[]
    {
    groupItem1,
    groupItem2,
    groupItem3,
    //groupItem4,
    groupItem5,
    groupItem6
    };
            NbgAiSet.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            NbgAiSet.Location = new System.Drawing.Point(0, 45);
            NbgAiSet.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            NbgAiSet.Name = "NbgAiSet";
            NbgAiSet.NavBarColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NbgAiSet.NavBarHeight = 40;
            NbgAiSet.NavForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            NbgAiSet.NavGroupColor = System.Drawing.Color.Empty;
            NbgAiSet.NavGroupHeight = 165;
            NbgAiSet.Size = new System.Drawing.Size(395, 405);
            NbgAiSet.SplitColor = System.Drawing.Color.Empty;
            NbgAiSet.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            NbgAiSet.StylizeFlag = true;
            NbgAiSet.TabIndex = 5;
            // 
            // AiParamsSetForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(389, 472);
            Controls.Add(NbgAiSet);
            ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            Name = "AiParamsSetForm";
            Text = "AiSetForm";
            Title = "AiSetForm";
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            Controls.SetChildIndex(NbgAiSet, 0);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.NavBarGroup NbgAiSet;
    }
}