namespace ScopeX.U2
{
    partial class ScreenDetectionPage
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
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt3 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt4 = new UserControls.DefaultHighlightPrompt();
            LblScreen = new UserControls.ScopeXLabel();
            BtnScreenDetection = new UserControls.ScopeXIconButton();
            BtnKeyboardDetection = new UserControls.ScopeXIconButton();
            LblKeyboard = new UserControls.ScopeXLabel();
            LblTouch = new UserControls.ScopeXLabel();
            BtnTouchDetection = new UserControls.ScopeXIconButton();
            LblLed = new UserControls.ScopeXLabel();
            BtnLEDDetection = new UserControls.ScopeXIconButton();
            SuspendLayout();
            // 
            // LblScreen
            // 
            LblScreen.BackColor = System.Drawing.Color.Transparent;
            LblScreen.BorderColor = System.Drawing.Color.Black;
            LblScreen.BorderThickness = 0;
            LblScreen.CornerRadius = 0;
            LblScreen.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblScreen.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblScreen.HighlightPrompt = defaultHighlightPrompt1;
            LblScreen.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblScreen.IsOmittext = true;
            LblScreen.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblScreen.Location = new System.Drawing.Point(10, 20);
            LblScreen.MultyLineFlag = false;
            LblScreen.Name = "LblScreen";
            LblScreen.Size = new System.Drawing.Size(160, 30);
            LblScreen.StyleFlags = UserControls.Style.StyleFlag.None;
            LblScreen.StylizeFlag = true;
            LblScreen.TabIndex = 23;
            LblScreen.TabStop = false;
            LblScreen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            LblScreen.Token = null;
            // 
            // BtnScreenDetection
            // 
            BtnScreenDetection.Adjustable = false;
            BtnScreenDetection.BackColor = System.Drawing.Color.Transparent;
            BtnScreenDetection.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnScreenDetection.BorderThickness = 1;
            BtnScreenDetection.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnScreenDetection.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnScreenDetection.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnScreenDetection.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnScreenDetection.CornerRadius = 0;
            BtnScreenDetection.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnScreenDetection.DaskArray = null;
            BtnScreenDetection.DoubleClickEnable = true;
            BtnScreenDetection.DropKey = System.Windows.Forms.Keys.Space;
            BtnScreenDetection.FineEnable = false;
            BtnScreenDetection.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnScreenDetection.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnScreenDetection.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnScreenDetection.Height = 30;
            BtnScreenDetection.Icon = null;
            BtnScreenDetection.IconOffset = 10;
            BtnScreenDetection.IconSize = new System.Drawing.Size(24, 24);
            BtnScreenDetection.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnScreenDetection.IsChoosed = false;
            BtnScreenDetection.IsIndicatorShow = false;
            BtnScreenDetection.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnScreenDetection.Location = new System.Drawing.Point(180, 20);
            BtnScreenDetection.Margin = new System.Windows.Forms.Padding(2);
            BtnScreenDetection.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnScreenDetection.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnScreenDetection.MouseInBorderThickness = 0;
            BtnScreenDetection.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnScreenDetection.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnScreenDetection.Name = "BtnScreenDetection";
            BtnScreenDetection.PressedBackColor = System.Drawing.Color.Gray;
            BtnScreenDetection.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnScreenDetection.PressedBorderThickness = 0;
            BtnScreenDetection.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnScreenDetection.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnScreenDetection.Size = new System.Drawing.Size(118, 30);
            BtnScreenDetection.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnScreenDetection.StylizeFlag = true;
            BtnScreenDetection.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnScreenDetection.SVGPath = "";
            BtnScreenDetection.TabIndex = 34;
            BtnScreenDetection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnScreenDetection.Click += BtnScreenDetection_Click;
            // 
            // BtnKeyboardDetection
            // 
            BtnKeyboardDetection.Adjustable = false;
            BtnKeyboardDetection.BackColor = System.Drawing.Color.Transparent;
            BtnKeyboardDetection.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnKeyboardDetection.BorderThickness = 1;
            BtnKeyboardDetection.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnKeyboardDetection.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnKeyboardDetection.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnKeyboardDetection.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnKeyboardDetection.CornerRadius = 0;
            BtnKeyboardDetection.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnKeyboardDetection.DaskArray = null;
            BtnKeyboardDetection.DoubleClickEnable = true;
            BtnKeyboardDetection.DropKey = System.Windows.Forms.Keys.Space;
            BtnKeyboardDetection.FineEnable = false;
            BtnKeyboardDetection.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnKeyboardDetection.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnKeyboardDetection.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnKeyboardDetection.Height = 30;
            BtnKeyboardDetection.Icon = null;
            BtnKeyboardDetection.IconOffset = 10;
            BtnKeyboardDetection.IconSize = new System.Drawing.Size(24, 24);
            BtnKeyboardDetection.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnKeyboardDetection.IsChoosed = false;
            BtnKeyboardDetection.IsIndicatorShow = false;
            BtnKeyboardDetection.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnKeyboardDetection.Location = new System.Drawing.Point(180, 120);
            BtnKeyboardDetection.Margin = new System.Windows.Forms.Padding(2);
            BtnKeyboardDetection.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnKeyboardDetection.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnKeyboardDetection.MouseInBorderThickness = 0;
            BtnKeyboardDetection.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnKeyboardDetection.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnKeyboardDetection.Name = "BtnKeyboardDetection";
            BtnKeyboardDetection.PressedBackColor = System.Drawing.Color.Gray;
            BtnKeyboardDetection.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnKeyboardDetection.PressedBorderThickness = 0;
            BtnKeyboardDetection.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnKeyboardDetection.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnKeyboardDetection.Size = new System.Drawing.Size(118, 30);
            BtnKeyboardDetection.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnKeyboardDetection.StylizeFlag = true;
            BtnKeyboardDetection.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnKeyboardDetection.SVGPath = "";
            BtnKeyboardDetection.TabIndex = 36;
            BtnKeyboardDetection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnKeyboardDetection.Click += BtnKeyboardDetection_Click;
            // 
            // LblKeyboard
            // 
            LblKeyboard.BackColor = System.Drawing.Color.Empty;
            LblKeyboard.BorderColor = System.Drawing.Color.Black;
            LblKeyboard.BorderThickness = 0;
            LblKeyboard.CornerRadius = 0;
            LblKeyboard.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblKeyboard.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblKeyboard.HighlightPrompt = defaultHighlightPrompt2;
            LblKeyboard.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblKeyboard.IsOmittext = true;
            LblKeyboard.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblKeyboard.Location = new System.Drawing.Point(10, 120);
            LblKeyboard.MultyLineFlag = false;
            LblKeyboard.Name = "LblKeyboard";
            LblKeyboard.Size = new System.Drawing.Size(160, 30);
            LblKeyboard.StyleFlags = UserControls.Style.StyleFlag.None;
            LblKeyboard.StylizeFlag = true;
            LblKeyboard.TabIndex = 35;
            LblKeyboard.TabStop = false;
            LblKeyboard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            LblKeyboard.Token = null;
            // 
            // LblTouch
            // 
            LblTouch.BackColor = System.Drawing.Color.Empty;
            LblTouch.BorderColor = System.Drawing.Color.Black;
            LblTouch.BorderThickness = 0;
            LblTouch.CornerRadius = 0;
            LblTouch.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblTouch.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblTouch.HighlightPrompt = defaultHighlightPrompt3;
            LblTouch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblTouch.IsOmittext = true;
            LblTouch.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblTouch.Location = new System.Drawing.Point(10, 70);
            LblTouch.MultyLineFlag = false;
            LblTouch.Name = "LblTouch";
            LblTouch.Size = new System.Drawing.Size(160, 30);
            LblTouch.StyleFlags = UserControls.Style.StyleFlag.None;
            LblTouch.StylizeFlag = true;
            LblTouch.TabIndex = 35;
            LblTouch.TabStop = false;
            LblTouch.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            LblTouch.Token = null;
            // 
            // BtnTouchDetection
            // 
            BtnTouchDetection.Adjustable = false;
            BtnTouchDetection.BackColor = System.Drawing.Color.Transparent;
            BtnTouchDetection.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTouchDetection.BorderThickness = 1;
            BtnTouchDetection.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnTouchDetection.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnTouchDetection.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnTouchDetection.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnTouchDetection.CornerRadius = 0;
            BtnTouchDetection.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnTouchDetection.DaskArray = null;
            BtnTouchDetection.DoubleClickEnable = true;
            BtnTouchDetection.DropKey = System.Windows.Forms.Keys.Space;
            BtnTouchDetection.FineEnable = false;
            BtnTouchDetection.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnTouchDetection.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnTouchDetection.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnTouchDetection.Height = 30;
            BtnTouchDetection.Icon = null;
            BtnTouchDetection.IconOffset = 10;
            BtnTouchDetection.IconSize = new System.Drawing.Size(24, 24);
            BtnTouchDetection.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnTouchDetection.IsChoosed = false;
            BtnTouchDetection.IsIndicatorShow = false;
            BtnTouchDetection.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnTouchDetection.Location = new System.Drawing.Point(180, 70);
            BtnTouchDetection.Margin = new System.Windows.Forms.Padding(2);
            BtnTouchDetection.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTouchDetection.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTouchDetection.MouseInBorderThickness = 0;
            BtnTouchDetection.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnTouchDetection.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnTouchDetection.Name = "BtnTouchDetection";
            BtnTouchDetection.PressedBackColor = System.Drawing.Color.Gray;
            BtnTouchDetection.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTouchDetection.PressedBorderThickness = 0;
            BtnTouchDetection.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnTouchDetection.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnTouchDetection.Size = new System.Drawing.Size(118, 30);
            BtnTouchDetection.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnTouchDetection.StylizeFlag = true;
            BtnTouchDetection.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnTouchDetection.SVGPath = "";
            BtnTouchDetection.TabIndex = 36;
            BtnTouchDetection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnTouchDetection.Click += BtnTouchDetection_Click;
            // 
            // LblLed
            // 
            LblLed.BackColor = System.Drawing.Color.Empty;
            LblLed.BorderColor = System.Drawing.Color.Black;
            LblLed.BorderThickness = 0;
            LblLed.CornerRadius = 0;
            LblLed.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblLed.ForeColor = System.Drawing.SystemColors.ButtonFace;
            LblLed.HighlightPrompt = defaultHighlightPrompt4;
            LblLed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblLed.IsOmittext = true;
            LblLed.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblLed.Location = new System.Drawing.Point(10, 170);
            LblLed.MultyLineFlag = false;
            LblLed.Name = "LblLed";
            LblLed.Size = new System.Drawing.Size(160, 30);
            LblLed.StyleFlags = UserControls.Style.StyleFlag.None;
            LblLed.StylizeFlag = true;
            LblLed.TabIndex = 35;
            LblLed.TabStop = false;
            LblLed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            LblLed.Token = null;
            LblLed.Visible = false;
            // 
            // BtnLEDDetection
            // 
            BtnLEDDetection.Adjustable = false;
            BtnLEDDetection.BackColor = System.Drawing.Color.Transparent;
            BtnLEDDetection.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnLEDDetection.BorderThickness = 1;
            BtnLEDDetection.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnLEDDetection.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnLEDDetection.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnLEDDetection.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnLEDDetection.CornerRadius = 0;
            BtnLEDDetection.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnLEDDetection.DaskArray = null;
            BtnLEDDetection.DoubleClickEnable = true;
            BtnLEDDetection.DropKey = System.Windows.Forms.Keys.Space;
            BtnLEDDetection.FineEnable = false;
            BtnLEDDetection.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnLEDDetection.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnLEDDetection.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnLEDDetection.Height = 30;
            BtnLEDDetection.Icon = null;
            BtnLEDDetection.IconOffset = 10;
            BtnLEDDetection.IconSize = new System.Drawing.Size(24, 24);
            BtnLEDDetection.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnLEDDetection.IsChoosed = false;
            BtnLEDDetection.IsIndicatorShow = false;
            BtnLEDDetection.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnLEDDetection.Location = new System.Drawing.Point(180, 170);
            BtnLEDDetection.Margin = new System.Windows.Forms.Padding(2);
            BtnLEDDetection.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnLEDDetection.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnLEDDetection.MouseInBorderThickness = 0;
            BtnLEDDetection.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnLEDDetection.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnLEDDetection.Name = "BtnLEDDetection";
            BtnLEDDetection.PressedBackColor = System.Drawing.Color.Gray;
            BtnLEDDetection.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnLEDDetection.PressedBorderThickness = 0;
            BtnLEDDetection.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnLEDDetection.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnLEDDetection.Size = new System.Drawing.Size(118, 30);
            BtnLEDDetection.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnLEDDetection.StylizeFlag = true;
            BtnLEDDetection.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnLEDDetection.SVGPath = "";
            BtnLEDDetection.TabIndex = 36;
            BtnLEDDetection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnLEDDetection.Visible = false;
            BtnLEDDetection.Click += BtnLEDDetection_Click;
            // 
            // ScreenDetectionPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            Controls.Add(BtnTouchDetection);
            Controls.Add(BtnLEDDetection);
            Controls.Add(BtnKeyboardDetection);
            Controls.Add(LblLed);
            Controls.Add(LblTouch);
            Controls.Add(LblKeyboard);
            Controls.Add(BtnScreenDetection);
            Controls.Add(LblScreen);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "ScreenDetectionPage";
            Size = new System.Drawing.Size(540, 460);
            ResumeLayout(false);
        }

        #endregion

        private UserControls.ScopeXLabel LblScreen;
        private UserControls.ScopeXIconButton BtnScreenDetection;
        private UserControls.ScopeXIconButton BtnKeyboardDetection;
        private UserControls.ScopeXLabel LblKeyboard;
        private UserControls.ScopeXLabel LblTouch;
        private UserControls.ScopeXIconButton BtnTouchDetection;
        private UserControls.ScopeXLabel LblLed;
        private UserControls.ScopeXIconButton BtnLEDDetection;
    }
}