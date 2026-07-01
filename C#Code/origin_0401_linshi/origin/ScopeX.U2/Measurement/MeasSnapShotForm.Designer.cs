
using System.Drawing;

namespace ScopeX.U2
{
    partial class MeasSnapShotForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MeasSnapShotForm));
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            LvContent = new ListViewEx();
            CbxSource = new UserControls.SelectComboBox();
            TmUpdate = new System.Timers.Timer();
            TlpSnapshot = new System.Windows.Forms.Panel();
            PlSource = new System.Windows.Forms.Panel();
            LblTip = new UserControls.ScopeXLabel();
            TlpSnapshot.SuspendLayout();
            PlSource.SuspendLayout();
            SuspendLayout();
            // 
            // LvContent
            // 
            LvContent.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            LvContent.EnbleSelect = false;
            LvContent.GridColor = Color.FromArgb(54, 54, 54);
            LvContent.GridLine = true;
            LvContent.GridWidth = 1;
            LvContent.HeaderBackColor = Color.Gray;
            LvContent.HeaderFont = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point);
            LvContent.HeaderForeColor = Color.White;
            LvContent.HeaderHeight = 0;
            LvContent.HeaderTextAlignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            LvContent.HeaderVisility = false;
            LvContent.ItemHeight = 41;
            LvContent.Location = new Point(0, 0);
            LvContent.Margin = new System.Windows.Forms.Padding(0);
            LvContent.Name = "LvContent";
            LvContent.Orientation = ListViewEx.ColorOrientation.Horizontal;
            LvContent.SelectedBackColor = Color.Blue;
            LvContent.SelectedForeColor = Color.White;
            LvContent.SelectedIndex = -1;
            LvContent.Size = new Size(1104, 328);
            LvContent.TabIndex = 1;
            LvContent.TabStop = false;
            LvContent.TextAlignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            // 
            // CbxSource
            // 
            CbxSource.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            CbxSource.BackColor = Color.FromArgb(53, 54, 58);
            CbxSource.BorderColor = Color.FromArgb(53, 54, 58);
            CbxSource.DataSource = (System.Collections.IList)resources.GetObject("CbxSource.DataSource");
            CbxSource.ExtText = "";
            CbxSource.Font = new Font("Microsoft YaHei UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            CbxSource.ForeColor = Color.White;
            CbxSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            CbxSource.Items = new string[]
     {
    "C1",
    "C2",
    "C3",
    "C4"
     };
            CbxSource.Location = new Point(974, 17);
            CbxSource.MaximumSize = new Size(99999, 99999);
            CbxSource.Name = "CbxSource";
            CbxSource.SelectIndex = 0;
            CbxSource.SelectValue = 0;
            CbxSource.Size = new Size(110, 30);
            CbxSource.TabIndex = 3;
            // 
            // TmUpdate
            // 
            TmUpdate.Enabled = false;
            TmUpdate.Interval = 500;
            TmUpdate.Elapsed += TmUpdate_Tick;
            // 
            // TlpSnapshot
            // 
            TlpSnapshot.Controls.Add(LvContent);
            TlpSnapshot.Controls.Add(PlSource);
            TlpSnapshot.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpSnapshot.Location = new Point(2, 45);
            TlpSnapshot.Margin = new System.Windows.Forms.Padding(0);
            TlpSnapshot.Name = "TlpSnapshot";
            TlpSnapshot.Size = new Size(1104, 396);
            TlpSnapshot.TabIndex = 6;
            // 
            // PlSource
            // 
            PlSource.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PlSource.BackColor = Color.Transparent;
            PlSource.Controls.Add(LblTip);
            PlSource.Controls.Add(CbxSource);
            PlSource.Location = new Point(0, 320);
            PlSource.Margin = new System.Windows.Forms.Padding(0);
            PlSource.Name = "PlSource";
            PlSource.Size = new Size(1104, 68);
            PlSource.TabIndex = 2;
            // 
            // LblTip
            // 
            LblTip.BackColor = Color.Transparent;
            LblTip.BorderColor = Color.FromArgb(53, 54, 58);
            LblTip.BorderThickness = 0;
            LblTip.CornerRadius = 0;
            LblTip.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point);
            LblTip.ForeColor = Color.Red;
            LblTip.HighlightPrompt = defaultHighlightPrompt1;
            LblTip.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTip.Location = new Point(6, 28);
            LblTip.MultyLineFlag = false;
            LblTip.Name = "LblTip";
            LblTip.Size = new Size(400, 25);
            LblTip.StyleFlags = UserControls.Style.StyleFlag.None;
            LblTip.StylizeFlag = false;
            LblTip.TabIndex = 48;
            LblTip.TabStop = false;
            LblTip.TextAlign = ContentAlignment.MiddleLeft;
            LblTip.Token = null;
            // 
            // MeasSnapShotForm
            // 
            AutoScaleDimensions = new SizeF(8F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            CanClose = false;
            ClientSize = new Size(1250, 445);
            ContentBackColor = Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(TlpSnapshot);
            DoubleBuffered = true;
            Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            FormOpacity = 85;
            HeadBackColor = Color.FromArgb(50, 55, 65);
            IsShowPin = false;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MeasSnapShotForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            StylizeFlag = true;
            TitleColor = Color.White;
            TitleFont = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point);
            TitleIcon = Properties.Resources.MeasureSnapshot;
            TitleIconSize = new Size(30, 30);
            TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            TitleIconWidth = 30;
            HelpClick += MeasSnapShotForm_LeftIconClick;
            Move += MeasSnapShotForm_Move;
            Controls.SetChildIndex(TlpSnapshot, 0);
            TlpSnapshot.ResumeLayout(false);
            PlSource.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        //private ScopeX.UserControls.ComboBoxEx CbxSource;
        private System.Timers.Timer TmUpdate;
        private ScopeX.U2.ListViewEx LvContent;
        private System.Windows.Forms.Panel TlpSnapshot;
        private System.Windows.Forms.Panel PlSource;
        private ScopeX.UserControls.SelectComboBox CbxSource;
        private UserControls.ScopeXLabel LblTip;
    }
}