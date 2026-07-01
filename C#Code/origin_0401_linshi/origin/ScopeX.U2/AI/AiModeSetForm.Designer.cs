namespace ScopeX.U2
{
    partial class AiModeSetForm
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
            ScopeX.UserControls.RadioButtonItem radioButtonItem1 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem2 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem3 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem4 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new ScopeX.UserControls.DefaultHighlightPrompt();
            RdoAiMode = new ScopeX.UserControls.UIRadioButtonGroup();
            LblAiMode = new ScopeX.UserControls.ScopeXLabel();
            SuspendLayout();
            // 
            // RdoAiMode
            // 
            RdoAiMode.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoAiMode.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoAiMode.BorderThickness = 0;
            RdoAiMode.ButtonBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoAiMode.ButtonFont = null;
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = "关闭";
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = "自动";
            radioButtonItem3.Icon = null;
            radioButtonItem3.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem3.Tag = null;
            radioButtonItem3.Text = "询问";
            radioButtonItem4.Icon = null;
            radioButtonItem4.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem4.Tag = null;
            radioButtonItem4.Text = "学习";
            RdoAiMode.ButtonItems = (new ScopeX.UserControls.RadioButtonItem[] { radioButtonItem1, radioButtonItem2, radioButtonItem3, radioButtonItem4 });
            RdoAiMode.ButtonOffset = 10;
            RdoAiMode.ButtonTextColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RdoAiMode.ChoosedButtonColor = System.Drawing.Color.FromArgb(0, 157, 255);
            RdoAiMode.ChoosedButtonIndex = 0;
            RdoAiMode.ChoosedButtonTextColor = System.Drawing.Color.Black;
            RdoAiMode.ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            RdoAiMode.ContentPadding = new System.Windows.Forms.Padding(0);
            RdoAiMode.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            RdoAiMode.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RdoAiMode.Height = 30;
            RdoAiMode.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            RdoAiMode.Location = new System.Drawing.Point(9, 93);
            RdoAiMode.Margin = new System.Windows.Forms.Padding(0);
            RdoAiMode.Name = "RdoAiMode";
            RdoAiMode.Size = new System.Drawing.Size(295, 30);
            RdoAiMode.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            RdoAiMode.StylizeFlag = true;
            RdoAiMode.TabIndex = 63;
            RdoAiMode.IndexChanged += RdoAiMode_IndexChanged;
            // 
            // LblAiMode
            // 
            LblAiMode.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblAiMode.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblAiMode.BorderThickness = 0;
            LblAiMode.CornerRadius = 0;
            LblAiMode.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblAiMode.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblAiMode.HighlightPrompt = defaultHighlightPrompt1;
            LblAiMode.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblAiMode.Location = new System.Drawing.Point(9, 61);
            LblAiMode.Margin = new System.Windows.Forms.Padding(2);
            LblAiMode.MultyLineFlag = false;
            LblAiMode.Name = "LblAiMode";
            LblAiMode.Size = new System.Drawing.Size(295, 30);
            LblAiMode.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblAiMode.StylizeFlag = true;
            LblAiMode.TabIndex = 64;
            LblAiMode.TabStop = false;
            LblAiMode.Text = "智能模式";
            LblAiMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblAiMode.Token = null;
            // 
            // AiModeForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(323, 139);
            Controls.Add(LblAiMode);
            Controls.Add(RdoAiMode);
            ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            Name = "AiModeForm";
            Text = "AI模式";
            Title = "AI模式";
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            Controls.SetChildIndex(RdoAiMode, 0);
            Controls.SetChildIndex(LblAiMode, 0);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.UIRadioButtonGroup RdoAiMode;
        private ScopeX.UserControls.ScopeXLabel LblAiMode;
    }
}