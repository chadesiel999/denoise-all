
using System.Linq;
using ScopeX.Core;

namespace ScopeX.U2
{
    partial class TriggerSerialSubPage
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
            if (this.ContentControl != null)
                this.ContentControl.Content = null;

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
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new UserControls.DefaultHighlightPrompt();
            TlpSerial = new System.Windows.Forms.TableLayoutPanel();
            HeaderPanel = new System.Windows.Forms.Panel();
            LblCondition = new UserControls.ScopeXLabel();
            LblSerialType = new UserControls.ScopeXLabel();
            CbxCondition = new ScopeX.UserControls.SelectComboBox();
            CbxSerialType = new ScopeX.UserControls.SelectComboBox();
            ContentControl = new DecodeBaseControl();
            PCustom = new System.Windows.Forms.Panel();
            BtnCustom = new UserControls.ScopeXIconButton();
            TlpSerial.SuspendLayout();
            HeaderPanel.SuspendLayout();
            PCustom.SuspendLayout();
            SuspendLayout();
            // 
            // TlpSerial
            // 
            TlpSerial.ColumnCount = 1;
            TlpSerial.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpSerial.Controls.Add(HeaderPanel, 0, 0);
            TlpSerial.Controls.Add(ContentControl, 0, 1);
            TlpSerial.Controls.Add(PCustom, 0, 2);
            TlpSerial.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpSerial.Location = new System.Drawing.Point(0, 0);
            TlpSerial.Margin = new System.Windows.Forms.Padding(0);
            TlpSerial.Name = "TlpSerial";
            TlpSerial.RowCount = 3;
            TlpSerial.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            TlpSerial.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TlpSerial.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            TlpSerial.Size = new System.Drawing.Size(500, 573);
            TlpSerial.TabIndex = 0;
            // 
            // HeaderPanel
            // 
            HeaderPanel.Controls.Add(LblCondition);
            HeaderPanel.Controls.Add(LblSerialType);
            HeaderPanel.Controls.Add(CbxCondition);
            HeaderPanel.Controls.Add(CbxSerialType);
            HeaderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            HeaderPanel.Location = new System.Drawing.Point(0, 0);
            HeaderPanel.Margin = new System.Windows.Forms.Padding(0);
            HeaderPanel.Name = "HeaderPanel";
            HeaderPanel.Size = new System.Drawing.Size(500, 66);
            HeaderPanel.TabIndex = 0;
            // 
            // LblCondition
            // 
            LblCondition.BackColor = System.Drawing.Color.Empty;
            LblCondition.BorderColor = System.Drawing.Color.Black;
            LblCondition.BorderThickness = 0;
            LblCondition.CornerRadius = 0;
            LblCondition.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblCondition.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            LblCondition.HighlightPrompt = defaultHighlightPrompt1;
            LblCondition.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblCondition.Location = new System.Drawing.Point(249, 2);
            LblCondition.MultyLineFlag = false;
            LblCondition.Name = "LblCondition";
            LblCondition.Size = new System.Drawing.Size(183, 21);
            LblCondition.StyleFlags = UserControls.Style.StyleFlag.None;
            LblCondition.StylizeFlag = true;
            LblCondition.TabIndex = 2;
            LblCondition.TabStop = false;
            LblCondition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblCondition.Token = null;
            // 
            // LblSerialType
            // 
            LblSerialType.BackColor = System.Drawing.Color.Empty;
            LblSerialType.BorderColor = System.Drawing.Color.Black;
            LblSerialType.BorderThickness = 0;
            LblSerialType.CornerRadius = 0;
            LblSerialType.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSerialType.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            LblSerialType.HighlightPrompt = defaultHighlightPrompt2;
            LblSerialType.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSerialType.Location = new System.Drawing.Point(10, 2);
            LblSerialType.MultyLineFlag = false;
            LblSerialType.Name = "LblSerialType";
            LblSerialType.Size = new System.Drawing.Size(183, 21);
            LblSerialType.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSerialType.StylizeFlag = true;
            LblSerialType.TabIndex = 0;
            LblSerialType.TabStop = false;
            LblSerialType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSerialType.Token = null;
            // 
            // CbxCondition
            // 
            CbxCondition.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxCondition.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxCondition.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxCondition.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxCondition.ForeColor = System.Drawing.Color.White;
            CbxCondition.Location = new System.Drawing.Point(249, 30);
            CbxCondition.Name = "CbxCondition";
            CbxCondition.Size = new System.Drawing.Size(285, 30);
            CbxCondition.TabIndex = 5;
            // 
            // CbxSerialType
            // 
            CbxSerialType.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSerialType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxSerialType.BorderStyle = System.Windows.Forms.BorderStyle.None;
            CbxSerialType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxSerialType.ForeColor = System.Drawing.Color.White;
            CbxSerialType.Location = new System.Drawing.Point(10, 30);
            CbxSerialType.Name = "CbxSerialType";
            CbxSerialType.Size = new System.Drawing.Size(183, 30);
            CbxSerialType.TabIndex = 4;
            // 
            // ContentControl
            // 
            ContentControl.BackColor = System.Drawing.Color.Transparent;
            ContentControl.Content = null;
            ContentControl.Dock = System.Windows.Forms.DockStyle.Fill;
            ContentControl.Fill = DecodeBaseControl.FillEnum.AutoSize;
            ContentControl.Location = new System.Drawing.Point(0, 66);
            ContentControl.Margin = new System.Windows.Forms.Padding(0);
            ContentControl.Name = "ContentControl";
            ContentControl.Size = new System.Drawing.Size(500, 255);
            ContentControl.TabIndex = 1;
            ContentControl.Text = "busBaseControl1";
            // 
            // PCustom
            // 
            PCustom.Controls.Add(BtnCustom);
            PCustom.Dock = System.Windows.Forms.DockStyle.Fill;
            PCustom.Location = new System.Drawing.Point(0, 321);
            PCustom.Margin = new System.Windows.Forms.Padding(0);
            PCustom.Name = "PCustom";
            PCustom.Size = new System.Drawing.Size(500, 252);
            PCustom.TabIndex = 2;
            // 
            // BtnCustom
            // 
            BtnCustom.BackColor = System.Drawing.Color.Transparent;
            BtnCustom.BorderColor = System.Drawing.Color.Black;
            BtnCustom.BorderThickness = 1;
            BtnCustom.CornerRadius = 0;
            BtnCustom.DaskArray = null;
            BtnCustom.DropKey = System.Windows.Forms.Keys.Space;
            BtnCustom.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCustom.Height = 27;
            BtnCustom.Icon = null;
            BtnCustom.IconOffset = 10;
            BtnCustom.IconSize = new System.Drawing.Size(24, 24);
            BtnCustom.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCustom.IsIndicatorShow = false;
            BtnCustom.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnCustom.Location = new System.Drawing.Point(10, 27);
            BtnCustom.Margin = new System.Windows.Forms.Padding(0);
            BtnCustom.MouseinBackColor = System.Drawing.Color.Transparent;
            BtnCustom.MouseinBorderColor = System.Drawing.Color.Blue;
            BtnCustom.MouseInBorderThickness = 1;
            BtnCustom.MouseinForeColor = System.Drawing.Color.Blue;
            BtnCustom.MouseinSvgForeColor = System.Drawing.Color.Blue;
            BtnCustom.Name = "BtnCustom";
            BtnCustom.PressedBackColor = System.Drawing.Color.Gray;
            BtnCustom.PressedBorderColor = System.Drawing.Color.Blue;
            BtnCustom.PressedBorderThickness = 1;
            BtnCustom.PressedForeColor = System.Drawing.Color.Blue;
            BtnCustom.PressedSvgForeColor = System.Drawing.Color.Blue;
            BtnCustom.Size = new System.Drawing.Size(183, 27);
            BtnCustom.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnCustom.StylizeFlag = false;
            BtnCustom.SVGForeColor = System.Drawing.Color.Black;
            BtnCustom.SVGPath = "";
            BtnCustom.TabIndex = 0;
            BtnCustom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnCustom.Click += BtnCustom_Click;
            // 
            // TriggerSerialSubPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(TlpSerial);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Margin = new System.Windows.Forms.Padding(0);
            Name = "TriggerSerialSubPage";
            Size = new System.Drawing.Size(539, 573);
            TlpSerial.ResumeLayout(false);
            HeaderPanel.ResumeLayout(false);
            PCustom.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpSerial;
        private System.Windows.Forms.Panel HeaderPanel;
        private ScopeX.UserControls.ScopeXLabel LblSerialType;
        private ScopeX.UserControls.ScopeXLabel LblCondition;
        //private ScopeX.UserControls.ComboBoxEx CbxCondition;
        private ScopeX.U2.DecodeBaseControl ContentControl;
        private System.Windows.Forms.Panel PCustom;
        private ScopeX.UserControls.ScopeXIconButton BtnCustom;
        private ScopeX.UserControls.SelectComboBox CbxCondition;
        private ScopeX.UserControls.SelectComboBox CbxSerialType;
    }
}
