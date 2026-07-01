
namespace ScopeX.U2
{
    partial class AnalogForm
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
            //UserControls.GroupItem groupItem2 = new UserControls.GroupItem();
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
            groupItem1.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.Group0"); // "垂直";
            //groupItem2.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            //groupItem2.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            //groupItem2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            //groupItem2.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            //groupItem2.GroupSize = new System.Drawing.Size(0, 0);
            //groupItem2.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.Group1"); // "输入";
            NbgAnalog.GroupItems = (new UserControls.GroupItem[] { groupItem1/*, groupItem2*/ });
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
            // AnalogForm
            // 
            ActiveBorderVisiable = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(578, 510);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(NbgAnalog);
            DoubleBuffered = true;
            FormOpacity = 95;
            HelpLabel = "13";
            IconInterval = 21;
            IconWidth = 28;
            IndicatorColor = System.Drawing.Color.Lime;
            IndicatorMargin = new System.Windows.Forms.Padding(0, 0, 0, 42);
            IsIndicatorShow = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AnalogForm";
            Padding = new System.Windows.Forms.Padding(1);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            StylizeFlag = true;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MoNiTongDao");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MoNiTongDao");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(28, 26);
            Controls.SetChildIndex(NbgAnalog, 0);
            ResumeLayout(false);
        }

        #endregion
        private ScopeX.UserControls.NavBarGroup NbgAnalog;
    }
}