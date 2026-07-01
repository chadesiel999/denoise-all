namespace ScopeX.U2
{
    partial class AutosetSettingForm
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
            BtnOnePeriod = new ScopeX.UserControls.ScopeXIconButton();
            BtnNPeriod = new ScopeX.UserControls.ScopeXIconButton();
            BtnRise = new ScopeX.UserControls.ScopeXIconButton();
            BtnFall = new ScopeX.UserControls.ScopeXIconButton();
            BtnSetting = new ScopeX.UserControls.ScopeXIconButton();
            Timer = new System.Timers.Timer();
            SuspendLayout();
            // 
            // BtnOnePeriod
            // 
            BtnOnePeriod.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOnePeriod.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOnePeriod.BorderThickness = 0;
            BtnOnePeriod.CornerRadius = 5;
            BtnOnePeriod.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnOnePeriod.DaskArray = null;
            BtnOnePeriod.DropKey = System.Windows.Forms.Keys.Space;
            BtnOnePeriod.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnOnePeriod.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOnePeriod.Height = 35;
            BtnOnePeriod.Icon = Properties.Resources.OnePeriod;
            BtnOnePeriod.IconOffset = 0;
            BtnOnePeriod.IconSize = new System.Drawing.Size(90, 35);
            BtnOnePeriod.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnOnePeriod.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnOnePeriod.IsIndicatorShow = false;
            BtnOnePeriod.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnOnePeriod.Location = new System.Drawing.Point(12, 12);
            BtnOnePeriod.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOnePeriod.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOnePeriod.MouseInBorderThickness = 0;
            BtnOnePeriod.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOnePeriod.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnOnePeriod.Name = "BtnOnePeriod";
            BtnOnePeriod.PressedBackColor = System.Drawing.Color.Gray;
            BtnOnePeriod.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnOnePeriod.PressedBorderThickness = 0;
            BtnOnePeriod.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOnePeriod.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnOnePeriod.Size = new System.Drawing.Size(90, 35);
            BtnOnePeriod.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnOnePeriod.StylizeFlag = true;
            BtnOnePeriod.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnOnePeriod.SVGPath = "";
            BtnOnePeriod.TabIndex = 15;
            BtnOnePeriod.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnOnePeriod.Click += BtnOnePeriod_Click;
            // 
            // BtnNPeriod
            // 
            BtnNPeriod.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnNPeriod.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnNPeriod.BorderThickness = 0;
            BtnNPeriod.CornerRadius = 5;
            BtnNPeriod.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnNPeriod.DaskArray = null;
            BtnNPeriod.DropKey = System.Windows.Forms.Keys.Space;
            BtnNPeriod.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnNPeriod.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnNPeriod.Height = 35;
            BtnNPeriod.Icon = Properties.Resources.NPeriod;
            BtnNPeriod.IconOffset = 0;
            BtnNPeriod.IconSize = new System.Drawing.Size(90, 35);
            BtnNPeriod.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnNPeriod.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnNPeriod.IsIndicatorShow = false;
            BtnNPeriod.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnNPeriod.Location = new System.Drawing.Point(113, 12);
            BtnNPeriod.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnNPeriod.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnNPeriod.MouseInBorderThickness = 0;
            BtnNPeriod.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnNPeriod.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnNPeriod.Name = "BtnNPeriod";
            BtnNPeriod.PressedBackColor = System.Drawing.Color.Gray;
            BtnNPeriod.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnNPeriod.PressedBorderThickness = 0;
            BtnNPeriod.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnNPeriod.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnNPeriod.Size = new System.Drawing.Size(90, 35);
            BtnNPeriod.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnNPeriod.StylizeFlag = true;
            BtnNPeriod.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnNPeriod.SVGPath = "";
            BtnNPeriod.TabIndex = 16;
            BtnNPeriod.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DuoZhouQi");
            BtnNPeriod.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnNPeriod.Click += BtnNPeriod_Click;
            // 
            // BtnRise
            // 
            BtnRise.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRise.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRise.BorderThickness = 0;
            BtnRise.CornerRadius = 5;
            BtnRise.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnRise.DaskArray = null;
            BtnRise.DropKey = System.Windows.Forms.Keys.Space;
            BtnRise.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnRise.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnRise.Height = 35;
            BtnRise.Icon = Properties.Resources.RisingEdge;
            BtnRise.IconOffset = 0;
            BtnRise.IconSize = new System.Drawing.Size(90, 35);
            BtnRise.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnRise.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnRise.IsIndicatorShow = false;
            BtnRise.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnRise.Location = new System.Drawing.Point(214, 12);
            BtnRise.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRise.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRise.MouseInBorderThickness = 0;
            BtnRise.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnRise.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnRise.Name = "BtnRise";
            BtnRise.PressedBackColor = System.Drawing.Color.Gray;
            BtnRise.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnRise.PressedBorderThickness = 0;
            BtnRise.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnRise.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnRise.Size = new System.Drawing.Size(90, 35);
            BtnRise.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnRise.StylizeFlag = true;
            BtnRise.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnRise.SVGPath = "";
            BtnRise.TabIndex = 17;
            BtnRise.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnRise.Click += BtnRise_Click;
            // 
            // BtnFall
            // 
            BtnFall.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnFall.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnFall.BorderThickness = 0;
            BtnFall.CornerRadius = 5;
            BtnFall.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnFall.DaskArray = null;
            BtnFall.DropKey = System.Windows.Forms.Keys.Space;
            BtnFall.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnFall.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnFall.Height = 35;
            BtnFall.Icon = Properties.Resources.FallingEdge;
            BtnFall.IconOffset = 0;
            BtnFall.IconSize = new System.Drawing.Size(90, 35);
            BtnFall.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnFall.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnFall.IsIndicatorShow = false;
            BtnFall.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnFall.Location = new System.Drawing.Point(315, 12);
            BtnFall.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnFall.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnFall.MouseInBorderThickness = 0;
            BtnFall.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnFall.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnFall.Name = "BtnFall";
            BtnFall.PressedBackColor = System.Drawing.Color.Gray;
            BtnFall.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnFall.PressedBorderThickness = 0;
            BtnFall.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnFall.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnFall.Size = new System.Drawing.Size(90, 35);
            BtnFall.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnFall.StylizeFlag = true;
            BtnFall.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnFall.SVGPath = "";
            BtnFall.TabIndex = 18;
            BtnFall.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnFall.Click += BtnFall_Click;
            // 
            // BtnSetting
            // 
            BtnSetting.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSetting.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSetting.BorderThickness = 0;
            BtnSetting.CornerRadius = 5;
            BtnSetting.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSetting.DaskArray = null;
            BtnSetting.DropKey = System.Windows.Forms.Keys.Space;
            BtnSetting.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnSetting.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSetting.Height = 35;
            BtnSetting.Icon = Properties.Resources.AutosetSetting;
            BtnSetting.IconOffset = 0;
            BtnSetting.IconSize = new System.Drawing.Size(90, 35);
            BtnSetting.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnSetting.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSetting.IsIndicatorShow = false;
            BtnSetting.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSetting.Location = new System.Drawing.Point(416, 12);
            BtnSetting.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSetting.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSetting.MouseInBorderThickness = 0;
            BtnSetting.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSetting.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSetting.Name = "BtnSetting";
            BtnSetting.PressedBackColor = System.Drawing.Color.Gray;
            BtnSetting.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSetting.PressedBorderThickness = 0;
            BtnSetting.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSetting.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSetting.Size = new System.Drawing.Size(90, 35);
            BtnSetting.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            BtnSetting.StylizeFlag = true;
            BtnSetting.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSetting.SVGPath = "";
            BtnSetting.TabIndex = 19;
            BtnSetting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSetting.Click += BtnSetting_Click;
            // 
            // Timer
            // 
            Timer.Interval = 1000;
            Timer.Elapsed += Timer_Tick;
            // 
            // AutosetSettingForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CanClose = false;
            ClientSize = new System.Drawing.Size(518, 62);
            Controls.Add(BtnSetting);
            Controls.Add(BtnFall);
            Controls.Add(BtnRise);
            Controls.Add(BtnNPeriod);
            Controls.Add(BtnOnePeriod);
            FormOpacity = 80;
            HeadHeight = 0;
            IsShowClose = false;
            IsShowPin = false;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AutosetSettingForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            StylizeFlag = true;
            Text = "AutosetSettingForm";
            Title = "AutosetSettingForm";
            Controls.SetChildIndex(BtnOnePeriod, 0);
            Controls.SetChildIndex(BtnNPeriod, 0);
            Controls.SetChildIndex(BtnRise, 0);
            Controls.SetChildIndex(BtnFall, 0);
            Controls.SetChildIndex(BtnSetting, 0);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXIconButton BtnOnePeriod;
        private ScopeX.UserControls.ScopeXIconButton BtnNPeriod;
        private ScopeX.UserControls.ScopeXIconButton BtnRise;
        private ScopeX.UserControls.ScopeXIconButton BtnFall;
        private ScopeX.UserControls.ScopeXIconButton BtnSetting;
        private System.Timers.Timer Timer;
    }
}