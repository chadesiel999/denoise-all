
namespace ScopeX.U2
{
    partial class AcquireForm
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
            NbgAcquire = new UserControls.NavBarGroup();
            SuspendLayout();
            // 
            // NbgAcquire
            // 
            NbgAcquire.AssignHelper = "";
            NbgAcquire.BackColor = System.Drawing.Color.FromArgb(39, 41, 48);
            NbgAcquire.CurrentGroupIndex = 0;
            NbgAcquire.Dock = System.Windows.Forms.DockStyle.Fill;
            groupItem1.BackGroundColor = System.Drawing.Color.Empty;
            groupItem1.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem1.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem1.GroupSize = new System.Drawing.Size(0, 0);
            groupItem1.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SheZhi");
            //groupItem2.BackGroundColor = System.Drawing.Color.Empty;
            //groupItem2.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            //groupItem2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            //groupItem2.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            //groupItem2.GroupSize = new System.Drawing.Size(0, 0);
            //groupItem2.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShunXuMoShi");
            NbgAcquire.GroupItems = (new UserControls.GroupItem[] { groupItem1/*, groupItem2 */});
            NbgAcquire.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            NbgAcquire.Location = new System.Drawing.Point(2, 45);
            NbgAcquire.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            NbgAcquire.Name = "NbgAcquire";
            NbgAcquire.NavBarColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NbgAcquire.NavBarHeight = 40;
            NbgAcquire.NavForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            NbgAcquire.NavGroupColor = System.Drawing.Color.Empty;
            NbgAcquire.NavGroupHeight = 229;
            NbgAcquire.Size = new System.Drawing.Size(532, 311);
            NbgAcquire.SplitColor = System.Drawing.Color.Empty;
            NbgAcquire.StyleFlags = UserControls.Style.StyleFlag.None;
            NbgAcquire.StylizeFlag = true;
            NbgAcquire.TabIndex = 0;
            // 
            // AcquireForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(536, 358);
            ContentBackColor = System.Drawing.Color.FromArgb(39, 41, 48);
            ControlBox = false;
            Controls.Add(NbgAcquire);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            HelpLabel = "15";
            IconInterval = 21;
            IconWidth = 26;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AcquireForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            StylizeFlag = true;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CaiJiSheZhi");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CaiJiSheZhi");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Controls.SetChildIndex(NbgAcquire, 0);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.NavBarGroup NbgAcquire;
    }
}