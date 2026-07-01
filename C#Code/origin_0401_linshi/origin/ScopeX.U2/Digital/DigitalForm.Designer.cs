
namespace ScopeX.U2
{
    partial class DigitalForm
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
            UserControls.GroupItem groupItem1 = new UserControls.GroupItem();
            UserControls.GroupItem groupItem2 = new UserControls.GroupItem();
            NbgDigital = new UserControls.NavBarGroup();
            SuspendLayout();
            // 
            // NbgDigital
            // 
            NbgDigital.AssignHelper = "";
            NbgDigital.CurrentGroupIndex = 0;
            NbgDigital.Dock = System.Windows.Forms.DockStyle.Fill;
            groupItem1.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem1.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem1.Font = new System.Drawing.Font("Arial", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem1.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem1.GroupSize = new System.Drawing.Size(0, 0);
            groupItem1.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShiSheZhi");
            groupItem2.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem2.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem2.Font = new System.Drawing.Font("Arial", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem2.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem2.GroupSize = new System.Drawing.Size(0, 0);
            groupItem2.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianPingSheZhi");
            NbgDigital.GroupItems = (new UserControls.GroupItem[] { groupItem1, groupItem2 });
            NbgDigital.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            NbgDigital.Location = new System.Drawing.Point(3, 46);
            NbgDigital.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            NbgDigital.Name = "NbgDigital";
            NbgDigital.NavBarColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NbgDigital.NavBarHeight = 40;
            NbgDigital.NavForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            NbgDigital.NavGroupColor = System.Drawing.Color.FromArgb(41, 42, 45);
            NbgDigital.NavGroupHeight = 206;
            NbgDigital.Size = new System.Drawing.Size(434, 288);
            NbgDigital.SplitColor = System.Drawing.Color.Empty;
            NbgDigital.StyleFlags = UserControls.Style.StyleFlag.None;
            NbgDigital.StylizeFlag = true;
            NbgDigital.TabIndex = 0;
            // 
            // DigitalForm
            // 
            ActiveBorderVisiable = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(440, 337);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(NbgDigital);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HelpLabel = "23";
            IconInterval = 21;
            IconWidth = 26;
            IsIndicatorShow = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DigitalForm";
            Padding = new System.Windows.Forms.Padding(1);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("_LuoJiTongDao");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("_LuoJiTongDao");
            TitleColor = System.Drawing.Color.FromArgb(234, 234, 234);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Controls.SetChildIndex(NbgDigital, 0);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.NavBarGroup NbgDigital;
    }
}