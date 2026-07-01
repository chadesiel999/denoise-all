using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.U2
{
    partial class MsgTipForm
    {

        private ScopeX.UserControls.ScopeXLabel LbMsg;
        private ScopeX.UserControls.ScopeXIconButton BtnYes;
        private ScopeX.UserControls.ScopeXIconButton BtnCancel;

        private void InitializeComponent()
        {
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new ScopeX.UserControls.DefaultHighlightPrompt();
            LbMsg = new ScopeX.UserControls.ScopeXLabel();
            BtnYes = new ScopeX.UserControls.ScopeXIconButton();
            BtnCancel = new ScopeX.UserControls.ScopeXIconButton();
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
            LbMsg.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LbMsg.StylizeFlag = true;
            LbMsg.TabIndex = 1;
            LbMsg.Text = "ScopeXLabel1";
            LbMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            BtnYes.Font = new System.Drawing.Font("MiSans", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnYes.ForeColor = System.Drawing.Color.Black;
            BtnYes.Height = 30;
            BtnYes.Icon = null;
            BtnYes.IconOffset = 10;
            BtnYes.IconSize = new System.Drawing.Size(24, 24);
            BtnYes.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnYes.IsIndicatorShow = false;
            BtnYes.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnYes.Location = new System.Drawing.Point(219, 168);
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
            BtnYes.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnYes.StylizeFlag = true;
            BtnYes.SVGForeColor = System.Drawing.Color.Black;
            BtnYes.SVGPath = "";
            BtnYes.TabIndex = 0;
            BtnYes.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Yes");// "是";
            BtnYes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnYes.Click += BtnYes_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            BtnCancel.BorderColor = System.Drawing.Color.Black;
            BtnCancel.BorderThickness = 1;
            BtnCancel.CornerRadius = 0;
            BtnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnCancel.DaskArray = null;
            BtnCancel.DropKey = System.Windows.Forms.Keys.Space;
            BtnCancel.Font = new System.Drawing.Font("MiSans", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnCancel.ForeColor = System.Drawing.Color.Black;
            BtnCancel.Height = 30;
            BtnCancel.Icon = null;
            BtnCancel.IconOffset = 10;
            BtnCancel.IconSize = new System.Drawing.Size(24, 24);
            BtnCancel.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCancel.IsIndicatorShow = false;
            BtnCancel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnCancel.Location = new System.Drawing.Point(332, 168);
            BtnCancel.MouseinBackColor = System.Drawing.SystemColors.ActiveCaption;
            BtnCancel.MouseinBorderColor = System.Drawing.Color.Blue;
            BtnCancel.MouseInBorderThickness = 1;
            BtnCancel.MouseinForeColor = System.Drawing.Color.Blue;
            BtnCancel.MouseinSvgForeColor = System.Drawing.Color.Blue;
            BtnCancel.Name = "BtnCancel";
            BtnCancel.PressedBackColor = System.Drawing.Color.Gray;
            BtnCancel.PressedBorderColor = System.Drawing.Color.Blue;
            BtnCancel.PressedBorderThickness = 1;
            BtnCancel.PressedForeColor = System.Drawing.Color.Blue;
            BtnCancel.PressedSvgForeColor = System.Drawing.Color.Blue;
            BtnCancel.Size = new System.Drawing.Size(100, 30);
            BtnCancel.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnCancel.StylizeFlag = true;
            BtnCancel.SVGForeColor = System.Drawing.Color.Black;
            BtnCancel.SVGPath = "";
            BtnCancel.TabIndex = 13;
            BtnCancel.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("No");//"否";
            BtnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnCancel.Click += BtnCancel_Click;
            BtnCancel.MouseMove += BtnCancel_MouseMove;
            // 
            // MsgTipForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            ClientSize = new System.Drawing.Size(442, 208);
            ControlBox = false;
            Controls.Add(LbMsg);
            Controls.Add(BtnYes);
            Controls.Add(BtnCancel);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 98;
            HeadHeight = 35;
            IconWidth = 26;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MsgTipForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "MessageForm";
            Title = "MessageForm";
            TitleColor = System.Drawing.Color.FromArgb(234, 234, 234);
            TitleFont = new System.Drawing.Font("MiSans", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleIconSize = new System.Drawing.Size(0, 0);
            TitleIconWidth = 0;
            ToolIconSize = new System.Drawing.Size(26, 26);
            Controls.SetChildIndex(BtnCancel, 0);
            Controls.SetChildIndex(BtnYes, 0);
            Controls.SetChildIndex(LbMsg, 0);
            ResumeLayout(false);
        }
    }
}
