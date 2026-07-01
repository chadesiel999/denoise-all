namespace ScopeX.U2
{
    partial class MultiDomainForm
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
            NbgRadio = new ScopeX.UserControls.NavBarGroup();
            SuspendLayout();
            // 
            // NbgRadio
            // 
            NbgRadio.AssignHelper = "";
            NbgRadio.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            NbgRadio.CurrentGroupIndex = 0;
            NbgRadio.Dock = System.Windows.Forms.DockStyle.Fill;
            groupItem1.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem1.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem1.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem1.GroupSize = new System.Drawing.Size(0, 0);
            groupItem1.Title = "显示设置";
            groupItem2.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem2.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem2.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem2.GroupSize = new System.Drawing.Size(0, 0);
            groupItem2.Title = "参数设置";
            NbgRadio.GroupItems = new ScopeX.UserControls.GroupItem[]
    {
    groupItem1,
    groupItem2
    };
            NbgRadio.Location = new System.Drawing.Point(0, 45);
            NbgRadio.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            NbgRadio.Name = "NbgRadio";
            NbgRadio.NavBarColor = System.Drawing.Color.FromArgb(53, 54, 58);
            NbgRadio.NavBarHeight = 40;
            NbgRadio.NavForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            NbgRadio.NavGroupColor = System.Drawing.Color.FromArgb(41, 42, 45);
            NbgRadio.NavGroupHeight = 272;
            NbgRadio.Size = new System.Drawing.Size(800, 352);
            NbgRadio.SplitColor = System.Drawing.Color.Empty;
            NbgRadio.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            NbgRadio.StylizeFlag = true;
            NbgRadio.TabIndex = 5;
            // 
            // MultiDomainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 397);
            Controls.Add(NbgRadio);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "MultiDomainForm";
            Text = "多域分析";
            Title = "多域分析";
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Controls.SetChildIndex(NbgRadio, 0);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.NavBarGroup NbgRadio;
    }
}