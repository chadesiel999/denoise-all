namespace ScopeX.U2
{
    partial class SegmentChooseForm
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
            ScFrame = new ScopeX.UserControls.ScopeXScrollContainer();
            LvFrame = new ScopeX.UserControls.ScrollPanel();
            ScFrame.SuspendLayout();
            SuspendLayout();
            // 
            // ScFrame
            // 
            ScFrame.BackColor = System.Drawing.Color.FromArgb(54, 54, 54);
            ScFrame.Controls.Add(LvFrame);
            ScFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            ScFrame.Location = new System.Drawing.Point(2, 45);
            ScFrame.Name = "ScFrame";
            ScFrame.ScrollControl = LvFrame;
            ScFrame.ScrollThickness = 6;
            ScFrame.Size = new System.Drawing.Size(546, 293);
            ScFrame.TabIndex = 9;
            // 
            // LvFrame
            // 
            LvFrame.AutoScroll = true;
            LvFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            LvFrame.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            LvFrame.Location = new System.Drawing.Point(0, 0);
            LvFrame.Name = "LvFrame";
            LvFrame.ScrollContainer = ScFrame;
            LvFrame.Size = new System.Drawing.Size(546, 293);
            LvFrame.TabIndex = 0;
            // 
            // SegmentChooseForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(550, 340);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(ScFrame);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            HeadHeight = 45;
            IconInterval = 21;
            IconWidth = 26;
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SegmentChooseForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            StylizeFlag = true;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XuanZeZhengXu");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XuanZeZhengXu");
            TitleColor = System.Drawing.Color.FromArgb(234, 234, 234);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleLableHeight = 39;
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Load += FragementChooseForm_Load;
            Controls.SetChildIndex(ScFrame, 0);
            ScFrame.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXScrollContainer ScFrame;
        private ScopeX.UserControls.ScrollPanel LvFrame;
    }
}