
namespace ScopeX.U2
{
    partial class PanelManageForm
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
            PanelContent = new UserControls.ScrollPanel();
            ScrollContainer = new UserControls.ScopeXScrollContainer();
            ScrollContainer.SuspendLayout();
            SuspendLayout();
            // 
            // PanelContent
            // 
            PanelContent.AutoScroll = true;
            PanelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            PanelContent.Location = new System.Drawing.Point(0, 0);
            PanelContent.Margin = new System.Windows.Forms.Padding(0);
            PanelContent.Name = "PanelContent";
            PanelContent.Padding = new System.Windows.Forms.Padding(3);
            PanelContent.ScrollContainer = ScrollContainer;
            PanelContent.Size = new System.Drawing.Size(271, 470);
            PanelContent.TabIndex = 5;
            // 
            // ScrollContainer
            // 
            ScrollContainer.BackColor = System.Drawing.Color.FromArgb(41, 43, 50);
            ScrollContainer.Controls.Add(PanelContent);
            ScrollContainer.Dock = System.Windows.Forms.DockStyle.Top;
            ScrollContainer.Location = new System.Drawing.Point(2, 35);
            ScrollContainer.Name = "ScrollContainer";
            ScrollContainer.ScrollControl = PanelContent;
            ScrollContainer.ScrollThickness = 6;
            ScrollContainer.Size = new System.Drawing.Size(271, 470);
            ScrollContainer.TabIndex = 6;
            // 
            // PanelManageForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(275, 500);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 43, 50);
            Controls.Add(ScrollContainer);
            FormOpacity = 90;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            HeadHeight = 35;
            IconSideDistance = 4;
            IconWidth = 19;
            Margin = new System.Windows.Forms.Padding(8);
            Name = "PanelManageForm";
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GongNengJieGuoMianBan");
            TitleColor = System.Drawing.Color.FromArgb(234, 234, 234);
            TitleFont = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            TitleIconSize = new System.Drawing.Size(0, 0);
            TitleIconWidth = 0;
            TitleLableHeight = 40;
            ToolIconSize = new System.Drawing.Size(19, 19);
            Controls.SetChildIndex(ScrollContainer, 0);
            ScrollContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScrollPanel PanelContent;
        private ScopeX.UserControls.ScopeXScrollContainer ScrollContainer;
    }
}