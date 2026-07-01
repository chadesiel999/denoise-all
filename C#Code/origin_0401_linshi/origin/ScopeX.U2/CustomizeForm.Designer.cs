using ScopeX.UserControls;
using System.Drawing;

namespace ScopeX.U2
{
    partial class CustomizeForm
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
            LblSetting = new UserControls.ScopeXLabel();
            CbxSetting=new SelectComboBox();
            BtnRun = new ScopeXIconButton();
            // 
            // CbxSetting
            // 
            CbxSetting.AutoSize = true;
            CbxSetting.BackColor = Color.FromArgb(53, 54, 58);
            CbxSetting.BorderColor = Color.FromArgb(53, 54, 58);
            CbxSetting.ComBorderColor = Color.Blue;
            CbxSetting.DataSource = null;
            CbxSetting.ExtText = "";
            CbxSetting.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point);
            CbxSetting.ForeColor = Color.White;
            CbxSetting.Location = new Point(10, 88);
            CbxSetting.MaximumSize = new Size(99999, 99999);
            CbxSetting.Name = "CbxCoupling";
            CbxSetting.SelectIndex = -1;
            CbxSetting.SelectValue = null;
            CbxSetting.Size = new Size(180, 30);
            CbxSetting.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxSetting.StylizeFlag = true;
            CbxSetting.TabIndex = 22;
            // 
            // LblActive
            // 
            LblSetting.BackColor = Color.FromArgb(41, 42, 45);
            LblSetting.BorderColor = Color.FromArgb(53, 54, 58);
            LblSetting.BorderThickness = 0;
            LblSetting.CornerRadius = 0;
            LblSetting.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LblSetting.ForeColor = Color.FromArgb(232, 234, 237);
            LblSetting.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSetting.Location = new Point(10, 60);
            LblSetting.MultyLineFlag = false;
            LblSetting.Name = "LblSetting";
            LblSetting.Size = new Size(110, 24);
            LblSetting.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZiDingYi");
            LblSetting.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSetting.StylizeFlag = true;
            LblSetting.TabIndex = 0;
            LblSetting.TabStop = false;
            LblSetting.TextAlign = ContentAlignment.MiddleLeft;
            LblSetting.Visible = true;
            LblSetting.Token = null;

            // 
            // BtnRun
            // 
            BtnRun.Adjustable = false;
            BtnRun.BackColor = Color.Transparent;
            BtnRun.BorderColor = Color.FromArgb(53, 54, 58);
            BtnRun.BorderThickness = 0;
            BtnRun.ChoosedBackColor = Color.FromArgb(40, 71, 193);
            BtnRun.ChoosedForeColor = Color.FromArgb(192, 192, 192);
            BtnRun.ChoosedMouseinBackColor = Color.FromArgb(40, 71, 193);
            BtnRun.ChoosedPressedBackColor = Color.FromArgb(40, 71, 193);
            BtnRun.CornerRadius = 0;
            BtnRun.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnRun.DaskArray = null;
            BtnRun.DoubleClickEnable = true;
            BtnRun.DropKey = System.Windows.Forms.Keys.Space;
            BtnRun.FineEnable = false;
            BtnRun.FocusedBorderColor = Color.DeepSkyBlue;
            BtnRun.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnRun.ForeColor = SystemColors.ActiveCaptionText;
            BtnRun.Height = 26;
            BtnRun.Icon = null;
            BtnRun.IconOffset = 10;
            BtnRun.IconSize = new Size(24, 24);
            BtnRun.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnRun.IndicatorColor = Color.FromArgb(40, 71, 193);
            BtnRun.IsChoosed = false;
            BtnRun.IsIndicatorShow = false;
            BtnRun.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnRun.Location = new Point(207, 88);
            BtnRun.MouseinBackColor = Color.FromArgb(53, 54, 58);
            BtnRun.MouseinBorderColor = Color.FromArgb(53, 54, 58);
            BtnRun.MouseInBorderThickness = 0;
            BtnRun.MouseinForeColor = Color.FromArgb(185, 192, 199);
            BtnRun.MouseinSvgForeColor = Color.FromArgb(0, 157, 255);
            BtnRun.Name = "BtnRun";
            BtnRun.PressedBackColor = Color.Gray;
            BtnRun.PressedBorderColor = Color.FromArgb(53, 54, 58);
            BtnRun.PressedBorderThickness = 0;
            BtnRun.PressedForeColor = Color.FromArgb(185, 192, 199);
            BtnRun.PressedSvgForeColor = Color.FromArgb(0, 157, 255);
            BtnRun.Size = new Size(110, 26);
            BtnRun.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnRun.StylizeFlag = true;
            BtnRun.SVGForeColor = Color.FromArgb(185, 192, 199);
            BtnRun.SVGPath = "";
            BtnRun.TabIndex = 23;
            BtnRun.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhiXing");
            BtnRun.TextAlign = ContentAlignment.MiddleCenter;
            BtnRun.Click += BtnRun_Click;

            ActiveBorderVisiable = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(578, 510);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
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
            Name = "CustomizeForm";
            Padding = new System.Windows.Forms.Padding(1);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            StylizeFlag = true;
            Controls.Add(BtnRun);
            Controls.Add(LblSetting);
            Controls.Add(CbxSetting);
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZiDingYiChuangKou");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZiDingYiChuangKou");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(28, 26);
            ResumeLayout(false);

    }

        private ScopeX.UserControls.ScopeXLabel LblSetting;
        private ScopeX.UserControls.SelectComboBox CbxSetting;
        private ScopeXIconButton BtnRun;

        #endregion
    }
}