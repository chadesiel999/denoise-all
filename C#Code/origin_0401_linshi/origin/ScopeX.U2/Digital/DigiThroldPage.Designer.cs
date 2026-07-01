
namespace ScopeX.U2
{
    partial class DigiThroldPage
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle1 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle1 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle2 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle3 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle2 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle4 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle5 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle6 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle3 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle7 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle8 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle9 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonStyle buttonStyle4 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle10 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle11 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle buttonBaseStyle12 = new ScopeX.UserControls.ScopeXNumericEditBox.ButtonBaseStyle();
            this.LblThrold = new System.Windows.Forms.Label();
            this.LblFamily = new System.Windows.Forms.Label();
            CbxFamily = new ScopeX.UserControls.SelectComboBox();
            this.NebThreshold = new ScopeX.UserControls.TouchNeb();
            this.LblGroup = new System.Windows.Forms.Label();
            CbxGroup = new ScopeX.UserControls.SelectComboBox();
            this.NebHysteresis = new ScopeX.UserControls.TouchNeb();
            this.LblHyst = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LblThrold
            // 
            this.LblThrold.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblThrold.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblThrold.Location = new System.Drawing.Point(162, 143);
            this.LblThrold.Name = "LblThrold";
            this.LblThrold.Size = new System.Drawing.Size(205, 20);
            this.LblThrold.TabIndex = 4;
            this.LblThrold.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YuZhiDianPing");
            this.LblThrold.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblFamily
            // 
            this.LblFamily.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblFamily.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblFamily.Location = new System.Drawing.Point(162, 76);
            this.LblFamily.Name = "LblFamily";
            this.LblFamily.Size = new System.Drawing.Size(205, 20);
            this.LblFamily.TabIndex = 2;
            this.LblFamily.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiLie");
            this.LblFamily.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
           // 
            // CbxFamily
            // 
            CbxFamily.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxFamily.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxFamily.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxFamily.Font = new System.Drawing.Font("MiSans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxFamily.ForeColor = System.Drawing.Color.White;
            CbxFamily.Items = new string[]
    {
    "TTL",
    "CMOS5000",
    "CMOS3300",
    "CMOS2500",
    "CMOS1800",
    "ECL",
    "PECL",
    "LVDS",
    "USER"
    };
            CbxFamily.Location = new System.Drawing.Point(162, 104);
            CbxFamily.Name = "CbxFamily";
            CbxFamily.Size = new System.Drawing.Size(205, 30);
            CbxFamily.TabIndex = 9;
            CbxFamily.SelectedIndexChanged += CbxFamily_SelectedIndexChanged;
            // 
            // NebThreshold
            // 
            this.NebThreshold.AddButtonImg = null;
            buttonBaseStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(164)))), ((int)(((byte)(220)))));
            buttonBaseStyle1.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle1.BorderThickness = 0;
            buttonBaseStyle1.ForeColor = System.Drawing.Color.White;
            buttonStyle1.MouseClickStyle = buttonBaseStyle1;
            buttonBaseStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(164)))), ((int)(((byte)(220)))));
            buttonBaseStyle2.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle2.BorderThickness = 0;
            buttonBaseStyle2.ForeColor = System.Drawing.Color.White;
            buttonStyle1.MouseInStyle = buttonBaseStyle2;
            buttonBaseStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(183)))), ((int)(((byte)(245)))));
            buttonBaseStyle3.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle3.BorderThickness = 0;
            buttonBaseStyle3.ForeColor = System.Drawing.Color.White;
            buttonStyle1.NomalStyle = buttonBaseStyle3;
            this.NebThreshold.AddButtonStyle = buttonStyle1;
            this.NebThreshold.AllwaysShowFocusImage = false;
            this.NebThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.NebThreshold.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.NebThreshold.BorderThickness = 0;
            this.NebThreshold.DisableHoldOnInput = false;
            this.NebThreshold.DropKey = System.Windows.Forms.Keys.Space;
            this.NebThreshold.FocusBoederColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.NebThreshold.FocusForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.NebThreshold.FocusImage = null;
            this.NebThreshold.FocusImagePosition = ScopeX.UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            this.NebThreshold.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.NebThreshold.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.NebThreshold.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.NebThreshold.Height = 30;
            this.NebThreshold.HoldOnSpeedLevel = 10;
            this.NebThreshold.IconWidthProportion = 1F;
            this.NebThreshold.Interval = 0.1D;
            this.NebThreshold.LanguageKey = null;
            this.NebThreshold.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            this.NebThreshold.Location = new System.Drawing.Point(162, 171);
            this.NebThreshold.MaxValue = 1.7976931348623157E+308D;
            this.NebThreshold.MinValue = -1.7976931348623157E+308D;
            this.NebThreshold.Name = "NebThreshold";
            this.NebThreshold.Size = new System.Drawing.Size(205, 30);
            this.NebThreshold.StringFormatFunc = null;
            this.NebThreshold.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.NebThreshold.StylizeFlag = true;
            this.NebThreshold.SubButtonImg = null;
            buttonBaseStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(164)))), ((int)(((byte)(220)))));
            buttonBaseStyle4.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle4.BorderThickness = 0;
            buttonBaseStyle4.ForeColor = System.Drawing.Color.White;
            buttonStyle2.MouseClickStyle = buttonBaseStyle4;
            buttonBaseStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(164)))), ((int)(((byte)(220)))));
            buttonBaseStyle5.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle5.BorderThickness = 0;
            buttonBaseStyle5.ForeColor = System.Drawing.Color.White;
            buttonStyle2.MouseInStyle = buttonBaseStyle5;
            buttonBaseStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(183)))), ((int)(((byte)(245)))));
            buttonBaseStyle6.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle6.BorderThickness = 0;
            buttonBaseStyle6.ForeColor = System.Drawing.Color.White;
            buttonStyle2.NomalStyle = buttonBaseStyle6;
            this.NebThreshold.SubButtonStyle = buttonStyle2;
            this.NebThreshold.TabIndex = 5;
            this.NebThreshold.Value = 0D;
            // 
            // LblGroup
            // 
            this.LblGroup.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblGroup.Location = new System.Drawing.Point(30, 76);
            this.LblGroup.Name = "LblGroup";
            this.LblGroup.Size = new System.Drawing.Size(100, 20);
            this.LblGroup.TabIndex = 0;
            this.LblGroup.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianPingZu");
            this.LblGroup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
           // 
            // CbxGroup
            // 
            CbxGroup.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxGroup.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxGroup.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxGroup.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxGroup.ForeColor = System.Drawing.Color.White;
            CbxGroup.Location = new System.Drawing.Point(30, 104);
            CbxGroup.Name = "CbxGroup";
            CbxGroup.Size = new System.Drawing.Size(99, 30);
            CbxGroup.TabIndex = 8;
            CbxGroup.SelectedIndexChanged += CbxGroup_SelectedIndexChanged;
            // 
            // NebHysteresis
            // 
            this.NebHysteresis.AddButtonImg = null;
            buttonBaseStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(164)))), ((int)(((byte)(220)))));
            buttonBaseStyle7.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle7.BorderThickness = 0;
            buttonBaseStyle7.ForeColor = System.Drawing.Color.White;
            buttonStyle3.MouseClickStyle = buttonBaseStyle7;
            buttonBaseStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(164)))), ((int)(((byte)(220)))));
            buttonBaseStyle8.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle8.BorderThickness = 0;
            buttonBaseStyle8.ForeColor = System.Drawing.Color.White;
            buttonStyle3.MouseInStyle = buttonBaseStyle8;
            buttonBaseStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(183)))), ((int)(((byte)(245)))));
            buttonBaseStyle9.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle9.BorderThickness = 0;
            buttonBaseStyle9.ForeColor = System.Drawing.Color.White;
            buttonStyle3.NomalStyle = buttonBaseStyle9;
            this.NebHysteresis.AddButtonStyle = buttonStyle3;
            this.NebHysteresis.AllwaysShowFocusImage = false;
            this.NebHysteresis.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.NebHysteresis.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.NebHysteresis.BorderThickness = 0;
            this.NebHysteresis.DisableHoldOnInput = false;
            this.NebHysteresis.DropKey = System.Windows.Forms.Keys.Space;
            this.NebHysteresis.FocusBoederColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.NebHysteresis.FocusForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.NebHysteresis.FocusImage = null;
            this.NebHysteresis.FocusImagePosition = ScopeX.UserControls.ScopeXNumericEditBox.FocusImagePositionEnum.Left;
            this.NebHysteresis.FocusImageRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.NebHysteresis.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.NebHysteresis.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.NebHysteresis.Height = 30;
            this.NebHysteresis.HoldOnSpeedLevel = 10;
            this.NebHysteresis.IconWidthProportion = 1F;
            this.NebHysteresis.Interval = 0.1D;
            this.NebHysteresis.LanguageKey = null;
            this.NebHysteresis.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            this.NebHysteresis.Location = new System.Drawing.Point(162, 238);
            this.NebHysteresis.MaxValue = 1.7976931348623157E+308D;
            this.NebHysteresis.MinValue = -1.7976931348623157E+308D;
            this.NebHysteresis.Name = "NebHysteresis";
            this.NebHysteresis.Size = new System.Drawing.Size(205, 30);
            this.NebHysteresis.StringFormatFunc = null;
            this.NebHysteresis.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.NebHysteresis.StylizeFlag = true;
            this.NebHysteresis.SubButtonImg = null;
            buttonBaseStyle10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(164)))), ((int)(((byte)(220)))));
            buttonBaseStyle10.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle10.BorderThickness = 0;
            buttonBaseStyle10.ForeColor = System.Drawing.Color.White;
            buttonStyle4.MouseClickStyle = buttonBaseStyle10;
            buttonBaseStyle11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(164)))), ((int)(((byte)(220)))));
            buttonBaseStyle11.BorderColor = System.Drawing.Color.Green;
            buttonBaseStyle11.BorderThickness = 0;
            buttonBaseStyle11.ForeColor = System.Drawing.Color.White;
            buttonStyle4.MouseInStyle = buttonBaseStyle11;
            buttonBaseStyle12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(183)))), ((int)(((byte)(245)))));
            buttonBaseStyle12.BorderColor = System.Drawing.Color.Transparent;
            buttonBaseStyle12.BorderThickness = 0;
            buttonBaseStyle12.ForeColor = System.Drawing.Color.White;
            buttonStyle4.NomalStyle = buttonBaseStyle12;
            this.NebHysteresis.SubButtonStyle = buttonStyle4;
            this.NebHysteresis.TabIndex = 7;
            this.NebHysteresis.Value = 0D;
            this.NebHysteresis.Visible = false;
            // 
            // LblHyst
            // 
            this.LblHyst.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblHyst.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.LblHyst.Location = new System.Drawing.Point(162, 210);
            this.LblHyst.Name = "LblHyst";
            this.LblHyst.Size = new System.Drawing.Size(205, 20);
            this.LblHyst.TabIndex = 6;
            this.LblHyst.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChiZhi");
            this.LblHyst.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblHyst.Visible = false;
            // 
            // DigiThroldPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.Controls.Add(this.LblGroup);
            this.Controls.Add(this.CbxGroup);
            this.Controls.Add(this.LblHyst);
            this.Controls.Add(this.LblThrold);
            this.Controls.Add(this.LblFamily);
            this.Controls.Add(this.CbxFamily);
            this.Controls.Add(this.NebHysteresis);
            this.Controls.Add(this.NebThreshold);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "DigiThroldPage";
            this.Size = new System.Drawing.Size(398, 371);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LblThrold;
        private System.Windows.Forms.Label LblFamily;
        private System.Windows.Forms.Label LblGroup;
        private ScopeX.UserControls.SelectComboBox CbxGroup;
        private ScopeX.UserControls.SelectComboBox CbxFamily;
        private ScopeX.UserControls.TouchNeb NebThreshold;
        private ScopeX.UserControls.TouchNeb NebHysteresis;
        private System.Windows.Forms.Label LblHyst;
    }
}
