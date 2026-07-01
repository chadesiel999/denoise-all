using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.U2
{
    partial class MsgBox
    {

        private ScopeX.UserControls.ScopeXLabel LbMsg;
        private ScopeX.UserControls.ScopeXIconButton BtnYes;

        private void InitializeComponent()
        {
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            LbMsg = new UserControls.ScopeXLabel();
            BtnYes = new UserControls.ScopeXIconButton();
            SuspendLayout();
            // 
            // LbMsg
            // 
            LbMsg.BackColor = System.Drawing.Color.Empty;
            LbMsg.BorderColor = System.Drawing.Color.Black;
            LbMsg.BorderThickness = 0;
            LbMsg.CornerRadius = 0;
            LbMsg.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LbMsg.ForeColor = System.Drawing.SystemColors.Control;
            LbMsg.HighlightPrompt = defaultHighlightPrompt1;
            LbMsg.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LbMsg.Location = new System.Drawing.Point(147, 82);
            LbMsg.MultyLineFlag = false;
            LbMsg.Name = "LbMsg";
            LbMsg.IsOmittext = false;
            LbMsg.Size = new System.Drawing.Size(206, 44);
            LbMsg.StyleFlags = UserControls.Style.StyleFlag.None;
            LbMsg.StylizeFlag = true;
            LbMsg.TabIndex = 14;
            LbMsg.Text = "ScopeXLabel1";
            LbMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            LbMsg.Token = null;
            // 
            // BtnYes
            // 
            BtnYes.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            BtnYes.BorderColor = System.Drawing.Color.Black;
            BtnYes.BorderThickness = 1;
            BtnYes.CornerRadius = 0;
            BtnYes.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnYes.DaskArray = null;
            BtnYes.DropKey = System.Windows.Forms.Keys.Space;
            BtnYes.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnYes.ForeColor = System.Drawing.Color.Black;
            BtnYes.Height = 30;
            BtnYes.Icon = null;
            BtnYes.IconOffset = 10;
            BtnYes.IconSize = new System.Drawing.Size(24, 24);
            BtnYes.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnYes.IsIndicatorShow = false;
            BtnYes.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnYes.Location = new System.Drawing.Point(171, 166);
            BtnYes.MouseinBackColor = System.Drawing.SystemColors.ActiveCaption;
            BtnYes.MouseinBorderColor = System.Drawing.Color.Blue;
            BtnYes.MouseInBorderThickness = 1;
            BtnYes.MouseinForeColor = System.Drawing.Color.Blue;
            BtnYes.MouseinSvgForeColor = System.Drawing.Color.Blue;
            BtnYes.Name = "BtnYes";
            BtnYes.PressedBackColor = System.Drawing.Color.Gray;
            BtnYes.PressedBorderColor = System.Drawing.Color.Blue;
            BtnYes.PressedBorderThickness = 1;
            BtnYes.PressedForeColor = System.Drawing.Color.Blue;
            BtnYes.PressedSvgForeColor = System.Drawing.Color.Blue;
            BtnYes.Size = new System.Drawing.Size(100, 30);
            BtnYes.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnYes.StylizeFlag = true;
            BtnYes.SVGForeColor = System.Drawing.Color.Black;
            BtnYes.SVGPath = "";
            BtnYes.TabIndex = 12;
            BtnYes.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QueDing");
            BtnYes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnYes.Click += BtnYes_Click;
            // 
            // MsgBox
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(442, 208);
            ControlBox = false;
            Controls.Add(LbMsg);
            Controls.Add(BtnYes);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 98;
            HeadHeight = 35;
            IconWidth = 26;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MsgBox";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "MessageForm";
            Title = "MessageForm";
            TitleColor = System.Drawing.Color.FromArgb(234, 234, 234);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleIconSize = new System.Drawing.Size(0, 0);
            TitleIconWidth = 0;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Controls.SetChildIndex(BtnYes, 0);
            Controls.SetChildIndex(LbMsg, 0);
            ResumeLayout(false);
        }
    }
}
