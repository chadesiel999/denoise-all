
namespace ScopeX.U2
{
    partial class WeakTipForm
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
            components = new System.ComponentModel.Container();
            PnlLeft = new System.Windows.Forms.Panel();
            PnlBottom = new System.Windows.Forms.Panel();
            PnlRight = new System.Windows.Forms.Panel();
            PnlTop = new System.Windows.Forms.Panel();
            TmUpdate = new System.Timers.Timer();
            BtnOpenFile = new UserControls.ScopeXIconButton();
            LblMsg = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // PnlLeft
            // 
            PnlLeft.BackColor = System.Drawing.Color.Gray;
            PnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            PnlLeft.Location = new System.Drawing.Point(0, 0);
            PnlLeft.Name = "PnlLeft";
            PnlLeft.Size = new System.Drawing.Size(1, 41);
            PnlLeft.TabIndex = 1;
            // 
            // PnlBottom
            // 
            PnlBottom.BackColor = System.Drawing.Color.Gray;
            PnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            PnlBottom.Location = new System.Drawing.Point(1, 40);
            PnlBottom.Name = "PnlBottom";
            PnlBottom.Size = new System.Drawing.Size(505, 1);
            PnlBottom.TabIndex = 2;
            // 
            // PnlRight
            // 
            PnlRight.BackColor = System.Drawing.Color.Gray;
            PnlRight.Dock = System.Windows.Forms.DockStyle.Right;
            PnlRight.Location = new System.Drawing.Point(505, 0);
            PnlRight.Name = "PnlRight";
            PnlRight.Size = new System.Drawing.Size(1, 40);
            PnlRight.TabIndex = 3;
            // 
            // PnlTop
            // 
            PnlTop.BackColor = System.Drawing.Color.Gray;
            PnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            PnlTop.Location = new System.Drawing.Point(1, 0);
            PnlTop.Name = "PnlTop";
            PnlTop.Size = new System.Drawing.Size(504, 1);
            PnlTop.TabIndex = 4;
            // 
            // TmUpdate
            // 
            TmUpdate.Interval = 1000;
            TmUpdate.Elapsed += TmUpdate_Tick;
            // 
            // BtnOpenFile
            // 
            BtnOpenFile.BackColor = System.Drawing.Color.Transparent;
            BtnOpenFile.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOpenFile.BorderThickness = 0;
            BtnOpenFile.CornerRadius = 0;
            BtnOpenFile.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnOpenFile.DaskArray = null;
            BtnOpenFile.Dock = System.Windows.Forms.DockStyle.Right;
            BtnOpenFile.DropKey = System.Windows.Forms.Keys.Space;
            BtnOpenFile.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnOpenFile.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOpenFile.Height = 39;
            BtnOpenFile.Icon = Properties.Resources.OpenFolder;
            BtnOpenFile.IconOffset = 0;
            BtnOpenFile.IconSize = new System.Drawing.Size(30, 30);
            BtnOpenFile.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnOpenFile.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnOpenFile.IsIndicatorShow = false;
            BtnOpenFile.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnOpenFile.Location = new System.Drawing.Point(470, 1);
            BtnOpenFile.MouseinBackColor = System.Drawing.Color.Transparent;
            BtnOpenFile.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOpenFile.MouseInBorderThickness = 0;
            BtnOpenFile.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOpenFile.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnOpenFile.Name = "BtnOpenFile";
            BtnOpenFile.PressedBackColor = System.Drawing.Color.Transparent;
            BtnOpenFile.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOpenFile.PressedBorderThickness = 0;
            BtnOpenFile.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOpenFile.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnOpenFile.Size = new System.Drawing.Size(35, 39);
            BtnOpenFile.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnOpenFile.StylizeFlag = false;
            BtnOpenFile.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOpenFile.SVGPath = "";
            BtnOpenFile.TabIndex = 12;
            BtnOpenFile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnOpenFile.Click += BtnOpenFile_Click;
            // 
            // LblMsg
            // 
            LblMsg.AutoEllipsis = true;
            LblMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            LblMsg.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblMsg.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblMsg.Location = new System.Drawing.Point(1, 1);
            LblMsg.Margin = new System.Windows.Forms.Padding(0);
            LblMsg.Name = "LblMsg";
            LblMsg.Size = new System.Drawing.Size(469, 39);
            LblMsg.TabIndex = 13;
            LblMsg.Text = "Message";
            LblMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WeakTipForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoScroll = true;
            AutoSize = true;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(506, 41);
            Controls.Add(LblMsg);
            Controls.Add(BtnOpenFile);
            Controls.Add(PnlTop);
            Controls.Add(PnlRight);
            Controls.Add(PnlBottom);
            Controls.Add(PnlLeft);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Name = "WeakTipForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "TipMsgForm";
            Load += WeakTipForm_Load;
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Panel PnlLeft;
        private System.Windows.Forms.Panel PnlBottom;
        private System.Windows.Forms.Panel PnlRight;
        private System.Windows.Forms.Panel PnlTop;
        private System.Timers.Timer TmUpdate;
        private ScopeX.UserControls.ScopeXIconButton BtnOpenFile;
        private System.Windows.Forms.Label LblMsg;
    }
}