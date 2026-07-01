
using PdfSharpCore.Drawing;

namespace ScopeX.U2
{
    partial class ProbePage
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
            if (disposing)
            {
                if (Font != null)
                {
                    Font = null;
                }
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
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt52 = new UserControls.DefaultHighlightPrompt();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProbePage));
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt53 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt54 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt55 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt56 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt57 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt58 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt59 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt60 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt61 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt62 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt63 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt64 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt65 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt66 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt67 = new UserControls.DefaultHighlightPrompt();
            UserControls.DefaultHighlightPrompt defaultHighlightPrompt68 = new UserControls.DefaultHighlightPrompt();
            LblProbeMag = new UserControls.ScopeXLabel();
            CbxProbeBtnType = new UserControls.SelectComboBox();
            LblProbeBtn = new UserControls.ScopeXLabel();
            LblInfo = new UserControls.ScopeXLabel();
            PnlInfo = new System.Windows.Forms.Panel();
            TableProbeInfo = new System.Windows.Forms.TableLayoutPanel();
            LblManufacturerName = new UserControls.ScopeXLabel();
            LblManufacturer = new UserControls.ScopeXLabel();
            LblModelName = new UserControls.ScopeXLabel();
            LblModel = new UserControls.ScopeXLabel();
            LblSNName = new UserControls.ScopeXLabel();
            LblSN = new UserControls.ScopeXLabel();
            LblMagName = new UserControls.ScopeXLabel();
            LblMag = new UserControls.ScopeXLabel();
            LblOtherName = new UserControls.ScopeXLabel();
            LblOther = new UserControls.ScopeXLabel();
            LblCustomGain = new UserControls.ScopeXLabel();
            BtnCustomGain = new UserControls.ScopeXIconButton();
            CbxProbeMag = new UserControls.SelectComboBox();
            BtnProbeCali = new UserControls.ScopeXIconButton();
            TbxUnit = new UserControls.ScopeXTextBox();
            LblUnitSelection = new UserControls.ScopeXLabel();
            BtnUnitRatio = new UserControls.ScopeXIconButton();
            LblUnitCvtText = new UserControls.ScopeXLabel();
            BtnProbeDefault = new UserControls.ScopeXIconButton();
            ChkProbeUnitCustom = new UserControls.ScopeXSwitchButton();
            BtnUnitRatioInverted = new UserControls.ScopeXIconButton();
            LblDCBiasSetting = new UserControls.ScopeXLabel();
            BtnDCBiasSetting = new UserControls.ScopeXIconButton();
            PnlInfo.SuspendLayout();
            TableProbeInfo.SuspendLayout();
            SuspendLayout();
            // 
            // LblProbeMag
            // 
            LblProbeMag.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblProbeMag.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblProbeMag.BorderThickness = 0;
            LblProbeMag.CornerRadius = 0;
            LblProbeMag.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblProbeMag.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblProbeMag.HighlightPrompt = defaultHighlightPrompt52;
            LblProbeMag.IsOmittext = true;
            LblProbeMag.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblProbeMag.Location = new System.Drawing.Point(10, 3);
            LblProbeMag.MultyLineFlag = false;
            LblProbeMag.Name = "LblProbeMag";
            LblProbeMag.Size = new System.Drawing.Size(100, 20);
            LblProbeMag.StyleFlags = UserControls.Style.StyleFlag.None;
            LblProbeMag.StylizeFlag = true;
            LblProbeMag.TabIndex = 0;
            LblProbeMag.Text = "探头倍率";
            LblProbeMag.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblProbeMag.Token = null;
            LblProbeMag.ToolTip = true;
            // 
            // CbxProbeBtnType
            // 
            CbxProbeBtnType.AutoSize = true;
            CbxProbeBtnType.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxProbeBtnType.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxProbeBtnType.ComBorderColor = System.Drawing.Color.Blue;
            CbxProbeBtnType.DataSource = (System.Collections.IList)resources.GetObject("CbxProbeBtnType.DataSource");
            CbxProbeBtnType.ExtText = "";
            CbxProbeBtnType.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxProbeBtnType.ForeColor = System.Drawing.Color.White;
            CbxProbeBtnType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            CbxProbeBtnType.Location = new System.Drawing.Point(279, 29);
            CbxProbeBtnType.MaximumSize = new System.Drawing.Size(9999, 9999);
            CbxProbeBtnType.Name = "CbxProbeBtnType";
            CbxProbeBtnType.SelectIndex = -1;
            CbxProbeBtnType.SelectValue = null;
            CbxProbeBtnType.Size = new System.Drawing.Size(181, 30);
            CbxProbeBtnType.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxProbeBtnType.StylizeFlag = true;
            CbxProbeBtnType.TabIndex = 87;
            // 
            // LblProbeBtn
            // 
            LblProbeBtn.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblProbeBtn.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblProbeBtn.BorderThickness = 0;
            LblProbeBtn.CornerRadius = 0;
            LblProbeBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblProbeBtn.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblProbeBtn.HighlightPrompt = defaultHighlightPrompt53;
            LblProbeBtn.IsOmittext = true;
            LblProbeBtn.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblProbeBtn.Location = new System.Drawing.Point(279, 0);
            LblProbeBtn.MultyLineFlag = false;
            LblProbeBtn.Name = "LblProbeBtn";
            LblProbeBtn.Size = new System.Drawing.Size(200, 20);
            LblProbeBtn.StyleFlags = UserControls.Style.StyleFlag.None;
            LblProbeBtn.StylizeFlag = true;
            LblProbeBtn.TabIndex = 8;
            LblProbeBtn.TabStop = false;
            LblProbeBtn.Text = "按键功能";
            LblProbeBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblProbeBtn.Token = null;
            LblProbeBtn.ToolTip = true;
            // 
            // LblInfo
            // 
            LblInfo.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblInfo.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblInfo.BorderThickness = 0;
            LblInfo.CornerRadius = 0;
            LblInfo.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblInfo.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblInfo.HighlightPrompt = defaultHighlightPrompt54;
            LblInfo.IsOmittext = true;
            LblInfo.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblInfo.Location = new System.Drawing.Point(266, 80);
            LblInfo.MultyLineFlag = false;
            LblInfo.Name = "LblInfo";
            LblInfo.Size = new System.Drawing.Size(200, 18);
            LblInfo.StyleFlags = UserControls.Style.StyleFlag.None;
            LblInfo.StylizeFlag = true;
            LblInfo.TabIndex = 13;
            LblInfo.Text = "探头信息";
            LblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblInfo.Token = null;
            LblInfo.ToolTip = true;
            // 
            // PnlInfo
            // 
            PnlInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            PnlInfo.Controls.Add(TableProbeInfo);
            PnlInfo.Location = new System.Drawing.Point(264, 100);
            PnlInfo.Name = "PnlInfo";
            PnlInfo.Size = new System.Drawing.Size(264, 150);
            PnlInfo.TabIndex = 16;
            // 
            // TableProbeInfo
            // 
            TableProbeInfo.ColumnCount = 2;
            TableProbeInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            TableProbeInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            TableProbeInfo.Controls.Add(LblManufacturerName, 0, 0);
            TableProbeInfo.Controls.Add(LblManufacturer, 1, 0);
            TableProbeInfo.Controls.Add(LblModelName, 0, 1);
            TableProbeInfo.Controls.Add(LblModel, 1, 1);
            TableProbeInfo.Controls.Add(LblSNName, 0, 2);
            TableProbeInfo.Controls.Add(LblSN, 1, 2);
            TableProbeInfo.Controls.Add(LblMagName, 0, 3);
            TableProbeInfo.Controls.Add(LblMag, 1, 3);
            TableProbeInfo.Controls.Add(LblOtherName, 0, 4);
            TableProbeInfo.Controls.Add(LblOther, 1, 4);
            TableProbeInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            TableProbeInfo.Location = new System.Drawing.Point(0, 0);
            TableProbeInfo.Margin = new System.Windows.Forms.Padding(0);
            TableProbeInfo.Name = "TableProbeInfo";
            TableProbeInfo.RowCount = 5;
            TableProbeInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            TableProbeInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            TableProbeInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            TableProbeInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            TableProbeInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            TableProbeInfo.Size = new System.Drawing.Size(262, 148);
            TableProbeInfo.TabIndex = 18;
            // 
            // LblManufacturerName
            // 
            LblManufacturerName.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblManufacturerName.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblManufacturerName.BorderThickness = 0;
            LblManufacturerName.CornerRadius = 0;
            LblManufacturerName.Dock = System.Windows.Forms.DockStyle.Fill;
            LblManufacturerName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblManufacturerName.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblManufacturerName.HighlightPrompt = defaultHighlightPrompt55;
            LblManufacturerName.IsOmittext = true;
            LblManufacturerName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblManufacturerName.Location = new System.Drawing.Point(1, 1);
            LblManufacturerName.Margin = new System.Windows.Forms.Padding(1);
            LblManufacturerName.MultyLineFlag = false;
            LblManufacturerName.Name = "LblManufacturerName";
            LblManufacturerName.Size = new System.Drawing.Size(102, 28);
            LblManufacturerName.StyleFlags = UserControls.Style.StyleFlag.None;
            LblManufacturerName.StylizeFlag = true;
            LblManufacturerName.TabIndex = 9;
            LblManufacturerName.TabStop = false;
            LblManufacturerName.Text = "Manufacturer";
            LblManufacturerName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblManufacturerName.Token = null;
            LblManufacturerName.ToolTip = true;
            // 
            // LblManufacturer
            // 
            LblManufacturer.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblManufacturer.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblManufacturer.BorderThickness = 0;
            LblManufacturer.CornerRadius = 0;
            LblManufacturer.Dock = System.Windows.Forms.DockStyle.Fill;
            LblManufacturer.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblManufacturer.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblManufacturer.HighlightPrompt = defaultHighlightPrompt56;
            LblManufacturer.IsOmittext = true;
            LblManufacturer.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblManufacturer.Location = new System.Drawing.Point(105, 1);
            LblManufacturer.Margin = new System.Windows.Forms.Padding(1);
            LblManufacturer.MultyLineFlag = false;
            LblManufacturer.Name = "LblManufacturer";
            LblManufacturer.Size = new System.Drawing.Size(156, 28);
            LblManufacturer.StyleFlags = UserControls.Style.StyleFlag.None;
            LblManufacturer.StylizeFlag = true;
            LblManufacturer.TabIndex = 10;
            LblManufacturer.TabStop = false;
            LblManufacturer.Text = "UNI-T";
            LblManufacturer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblManufacturer.Token = null;
            LblManufacturer.ToolTip = true;
            // 
            // LblModelName
            // 
            LblModelName.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblModelName.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblModelName.BorderThickness = 0;
            LblModelName.CornerRadius = 0;
            LblModelName.Dock = System.Windows.Forms.DockStyle.Fill;
            LblModelName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblModelName.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblModelName.HighlightPrompt = defaultHighlightPrompt57;
            LblModelName.IsOmittext = true;
            LblModelName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblModelName.Location = new System.Drawing.Point(1, 31);
            LblModelName.Margin = new System.Windows.Forms.Padding(1);
            LblModelName.MultyLineFlag = false;
            LblModelName.Name = "LblModelName";
            LblModelName.Size = new System.Drawing.Size(102, 28);
            LblModelName.StyleFlags = UserControls.Style.StyleFlag.None;
            LblModelName.StylizeFlag = true;
            LblModelName.TabIndex = 11;
            LblModelName.TabStop = false;
            LblModelName.Text = "Model";
            LblModelName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblModelName.Token = null;
            LblModelName.ToolTip = true;
            // 
            // LblModel
            // 
            LblModel.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblModel.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblModel.BorderThickness = 0;
            LblModel.CornerRadius = 0;
            LblModel.Dock = System.Windows.Forms.DockStyle.Fill;
            LblModel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblModel.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblModel.HighlightPrompt = defaultHighlightPrompt58;
            LblModel.IsOmittext = true;
            LblModel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblModel.Location = new System.Drawing.Point(105, 31);
            LblModel.Margin = new System.Windows.Forms.Padding(1);
            LblModel.MultyLineFlag = false;
            LblModel.Name = "LblModel";
            LblModel.Size = new System.Drawing.Size(156, 28);
            LblModel.StyleFlags = UserControls.Style.StyleFlag.None;
            LblModel.StylizeFlag = true;
            LblModel.TabIndex = 14;
            LblModel.TabStop = false;
            LblModel.Text = "UT-PA1500";
            LblModel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblModel.Token = null;
            LblModel.ToolTip = true;
            // 
            // LblSNName
            // 
            LblSNName.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblSNName.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblSNName.BorderThickness = 0;
            LblSNName.CornerRadius = 0;
            LblSNName.Dock = System.Windows.Forms.DockStyle.Fill;
            LblSNName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSNName.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblSNName.HighlightPrompt = defaultHighlightPrompt59;
            LblSNName.IsOmittext = true;
            LblSNName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSNName.Location = new System.Drawing.Point(1, 61);
            LblSNName.Margin = new System.Windows.Forms.Padding(1);
            LblSNName.MultyLineFlag = false;
            LblSNName.Name = "LblSNName";
            LblSNName.Size = new System.Drawing.Size(102, 28);
            LblSNName.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            LblSNName.StylizeFlag = true;
            LblSNName.TabIndex = 12;
            LblSNName.TabStop = false;
            LblSNName.Text = "Serial";
            LblSNName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSNName.Token = null;
            LblSNName.ToolTip = true;
            // 
            // LblSN
            // 
            LblSN.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblSN.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblSN.BorderThickness = 0;
            LblSN.CornerRadius = 0;
            LblSN.Dock = System.Windows.Forms.DockStyle.Fill;
            LblSN.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblSN.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblSN.HighlightPrompt = defaultHighlightPrompt60;
            LblSN.IsOmittext = true;
            LblSN.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblSN.Location = new System.Drawing.Point(105, 61);
            LblSN.Margin = new System.Windows.Forms.Padding(1);
            LblSN.MultyLineFlag = false;
            LblSN.Name = "LblSN";
            LblSN.Size = new System.Drawing.Size(156, 28);
            LblSN.StyleFlags = UserControls.Style.StyleFlag.None;
            LblSN.StylizeFlag = true;
            LblSN.TabIndex = 15;
            LblSN.TabStop = false;
            LblSN.Text = "APD2525020020";
            LblSN.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblSN.Token = null;
            LblSN.ToolTip = true;
            // 
            // LblMagName
            // 
            LblMagName.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblMagName.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblMagName.BorderThickness = 0;
            LblMagName.CornerRadius = 0;
            LblMagName.Dock = System.Windows.Forms.DockStyle.Fill;
            LblMagName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblMagName.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblMagName.HighlightPrompt = defaultHighlightPrompt61;
            LblMagName.IsOmittext = true;
            LblMagName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblMagName.Location = new System.Drawing.Point(1, 91);
            LblMagName.Margin = new System.Windows.Forms.Padding(1);
            LblMagName.MultyLineFlag = false;
            LblMagName.Name = "LblMagName";
            LblMagName.Size = new System.Drawing.Size(102, 28);
            LblMagName.StyleFlags = UserControls.Style.StyleFlag.None;
            LblMagName.StylizeFlag = true;
            LblMagName.TabIndex = 13;
            LblMagName.TabStop = false;
            LblMagName.Text = "Magnific";
            LblMagName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblMagName.Token = null;
            LblMagName.ToolTip = true;
            // 
            // LblMag
            // 
            LblMag.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblMag.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblMag.BorderThickness = 0;
            LblMag.CornerRadius = 0;
            LblMag.Dock = System.Windows.Forms.DockStyle.Fill;
            LblMag.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblMag.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblMag.HighlightPrompt = defaultHighlightPrompt62;
            LblMag.IsOmittext = true;
            LblMag.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblMag.Location = new System.Drawing.Point(105, 91);
            LblMag.Margin = new System.Windows.Forms.Padding(1);
            LblMag.MultyLineFlag = false;
            LblMag.Name = "LblMag";
            LblMag.Size = new System.Drawing.Size(156, 28);
            LblMag.StyleFlags = UserControls.Style.StyleFlag.None;
            LblMag.StylizeFlag = true;
            LblMag.TabIndex = 16;
            LblMag.TabStop = false;
            LblMag.Text = "X10";
            LblMag.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblMag.Token = null;
            LblMag.ToolTip = true;
            // 
            // LblOtherName
            // 
            LblOtherName.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblOtherName.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblOtherName.BorderThickness = 0;
            LblOtherName.CornerRadius = 0;
            LblOtherName.Dock = System.Windows.Forms.DockStyle.Fill;
            LblOtherName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblOtherName.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblOtherName.HighlightPrompt = defaultHighlightPrompt63;
            LblOtherName.IsOmittext = true;
            LblOtherName.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblOtherName.Location = new System.Drawing.Point(1, 121);
            LblOtherName.Margin = new System.Windows.Forms.Padding(1);
            LblOtherName.MultyLineFlag = false;
            LblOtherName.Name = "LblOtherName";
            LblOtherName.Size = new System.Drawing.Size(102, 28);
            LblOtherName.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            LblOtherName.StylizeFlag = true;
            LblOtherName.TabIndex = 12;
            LblOtherName.TabStop = false;
            LblOtherName.Text = "Other";
            LblOtherName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblOtherName.Token = null;
            LblOtherName.ToolTip = true;
            // 
            // LblOther
            // 
            LblOther.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblOther.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblOther.BorderThickness = 0;
            LblOther.CornerRadius = 0;
            LblOther.Dock = System.Windows.Forms.DockStyle.Fill;
            LblOther.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblOther.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblOther.HighlightPrompt = defaultHighlightPrompt64;
            LblOther.IsOmittext = true;
            LblOther.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblOther.Location = new System.Drawing.Point(105, 121);
            LblOther.Margin = new System.Windows.Forms.Padding(1);
            LblOther.MultyLineFlag = false;
            LblOther.Name = "LblOther";
            LblOther.Size = new System.Drawing.Size(156, 28);
            LblOther.StyleFlags = UserControls.Style.StyleFlag.None;
            LblOther.StylizeFlag = true;
            LblOther.TabIndex = 15;
            LblOther.TabStop = false;
            LblOther.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblOther.Token = null;
            LblOther.ToolTip = true;
            // 
            // LblCustomGain
            // 
            LblCustomGain.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblCustomGain.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblCustomGain.BorderThickness = 0;
            LblCustomGain.CornerRadius = 0;
            LblCustomGain.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblCustomGain.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblCustomGain.HighlightPrompt = defaultHighlightPrompt65;
            LblCustomGain.IsOmittext = true;
            LblCustomGain.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblCustomGain.Location = new System.Drawing.Point(122, 3);
            LblCustomGain.MultyLineFlag = false;
            LblCustomGain.Name = "LblCustomGain";
            LblCustomGain.Size = new System.Drawing.Size(100, 18);
            LblCustomGain.StyleFlags = UserControls.Style.StyleFlag.None;
            LblCustomGain.StylizeFlag = true;
            LblCustomGain.TabIndex = 20;
            LblCustomGain.TabStop = false;
            LblCustomGain.Text = "自定义倍率";
            LblCustomGain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblCustomGain.Token = null;
            LblCustomGain.ToolTip = true;
            LblCustomGain.Visible = false;
            // 
            // BtnCustomGain
            // 
            BtnCustomGain.Adjustable = false;
            BtnCustomGain.BackColor = System.Drawing.Color.Transparent;
            BtnCustomGain.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCustomGain.BorderThickness = 0;
            BtnCustomGain.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCustomGain.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnCustomGain.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCustomGain.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCustomGain.CornerRadius = 0;
            BtnCustomGain.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnCustomGain.DaskArray = null;
            BtnCustomGain.DoubleClickEnable = true;
            BtnCustomGain.DropKey = System.Windows.Forms.Keys.Space;
            BtnCustomGain.FineEnable = false;
            BtnCustomGain.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnCustomGain.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnCustomGain.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnCustomGain.Height = 30;
            BtnCustomGain.Icon = null;
            BtnCustomGain.IconOffset = 10;
            BtnCustomGain.IconSize = new System.Drawing.Size(24, 24);
            BtnCustomGain.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnCustomGain.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCustomGain.IsChoosed = false;
            BtnCustomGain.IsIndicatorShow = false;
            BtnCustomGain.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnCustomGain.Location = new System.Drawing.Point(122, 29);
            BtnCustomGain.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCustomGain.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCustomGain.MouseInBorderThickness = 0;
            BtnCustomGain.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCustomGain.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCustomGain.Name = "BtnCustomGain";
            BtnCustomGain.PressedBackColor = System.Drawing.Color.Gray;
            BtnCustomGain.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCustomGain.PressedBorderThickness = 0;
            BtnCustomGain.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCustomGain.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCustomGain.Size = new System.Drawing.Size(100, 30);
            BtnCustomGain.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnCustomGain.StylizeFlag = true;
            BtnCustomGain.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCustomGain.SVGPath = "";
            BtnCustomGain.TabIndex = 86;
            BtnCustomGain.Text = "倍率值";
            BtnCustomGain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnCustomGain.Visible = false;
            BtnCustomGain.DoubleClick += BtnCustomGain_Click;
            // 
            // CbxProbeMag
            // 
            CbxProbeMag.AutoSize = true;
            CbxProbeMag.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxProbeMag.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            CbxProbeMag.ComBorderColor = System.Drawing.Color.Blue;
            CbxProbeMag.DataSource = (System.Collections.IList)resources.GetObject("CbxProbeMag.DataSource");
            CbxProbeMag.ExtText = "";
            CbxProbeMag.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CbxProbeMag.ForeColor = System.Drawing.Color.White;
            CbxProbeMag.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            CbxProbeMag.Location = new System.Drawing.Point(10, 29);
            CbxProbeMag.MaximumSize = new System.Drawing.Size(9999, 9999);
            CbxProbeMag.Name = "CbxProbeMag";
            CbxProbeMag.SelectIndex = -1;
            CbxProbeMag.SelectValue = null;
            CbxProbeMag.Size = new System.Drawing.Size(100, 30);
            CbxProbeMag.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            CbxProbeMag.StylizeFlag = true;
            CbxProbeMag.TabIndex = 91;
            CbxProbeMag.SelectedIndexChanged += CbxProbeMag_SelectedIndexChanged;
            // 
            // BtnProbeCali
            // 
            BtnProbeCali.Adjustable = false;
            BtnProbeCali.BackColor = System.Drawing.Color.Transparent;
            BtnProbeCali.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnProbeCali.BorderThickness = 1;
            BtnProbeCali.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnProbeCali.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnProbeCali.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnProbeCali.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnProbeCali.CornerRadius = 0;
            BtnProbeCali.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnProbeCali.DaskArray = null;
            BtnProbeCali.DoubleClickEnable = true;
            BtnProbeCali.DropKey = System.Windows.Forms.Keys.Space;
            BtnProbeCali.FineEnable = false;
            BtnProbeCali.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnProbeCali.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnProbeCali.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnProbeCali.Height = 30;
            BtnProbeCali.Icon = null;
            BtnProbeCali.IconOffset = 10;
            BtnProbeCali.IconSize = new System.Drawing.Size(24, 24);
            BtnProbeCali.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnProbeCali.IsChoosed = false;
            BtnProbeCali.IsIndicatorShow = false;
            BtnProbeCali.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnProbeCali.Location = new System.Drawing.Point(264, 257);
            BtnProbeCali.Margin = new System.Windows.Forms.Padding(2);
            BtnProbeCali.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnProbeCali.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnProbeCali.MouseInBorderThickness = 0;
            BtnProbeCali.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnProbeCali.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnProbeCali.Name = "BtnProbeCali";
            BtnProbeCali.PressedBackColor = System.Drawing.Color.Gray;
            BtnProbeCali.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnProbeCali.PressedBorderThickness = 0;
            BtnProbeCali.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnProbeCali.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnProbeCali.Size = new System.Drawing.Size(100, 30);
            BtnProbeCali.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnProbeCali.StylizeFlag = true;
            BtnProbeCali.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnProbeCali.SVGPath = "";
            BtnProbeCali.TabIndex = 17;
            BtnProbeCali.Text = "自动校正";
            BtnProbeCali.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnProbeCali.MouseClick += BtnProbeCali_Click;
            // 
            // TbxUnit
            // 
            TbxUnit.AcceptsTab = false;
            TbxUnit.Adjustable = false;
            TbxUnit.AutoShowKeyBoard = true;
            TbxUnit.AutoSize = false;
            TbxUnit.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxUnit.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            TbxUnit.BorderThickness = 0;
            TbxUnit.ClickedBorderColor = System.Drawing.Color.White;
            TbxUnit.CornerRadius = 0;
            TbxUnit.Cursor = System.Windows.Forms.Cursors.IBeam;
            TbxUnit.DoubleClickEnable = false;
            TbxUnit.Enabled = true;
            TbxUnit.EnbleSelectBorder = true;
            TbxUnit.FineEnable = false;
            TbxUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbxUnit.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            TbxUnit.Height = 30;
            TbxUnit.HideSelection = true;
            TbxUnit.IsFocusClicked = false;
            TbxUnit.KBMaxCharCount = 2;
            TbxUnit.KeyboardVerify = null;
            TbxUnit.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            TbxUnit.Location = new System.Drawing.Point(122, 100);
            TbxUnit.MaxLength = 2;
            TbxUnit.Modified = false;
            TbxUnit.MouseEnterState = false;
            TbxUnit.Multiline = false;
            TbxUnit.Name = "TbxUnit";
            TbxUnit.ProcessCmdKeyFunc = null;
            TbxUnit.ReadOnly = false;
            TbxUnit.SelectedColor = System.Drawing.Color.FromArgb(0, 157, 255);
            TbxUnit.SelectedText = "";
            TbxUnit.SelectionLength = 0;
            TbxUnit.SelectionStart = 0;
            TbxUnit.ShortcutsEnabled = true;
            TbxUnit.Size = new System.Drawing.Size(100, 30);
            TbxUnit.StyleFlags = UserControls.Style.StyleFlag.None;
            TbxUnit.StylizeFlag = true;
            TbxUnit.TabIndex = 93;
            TbxUnit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            TbxUnit.UseSystemPasswordChar = false;
            TbxUnit.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            TbxUnit.WordWrap = true;
            TbxUnit.TextChanged += TbxUnit_SelectedIndexChanged;
            // 
            // LblUnitSelection
            // 
            LblUnitSelection.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblUnitSelection.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblUnitSelection.BorderThickness = 0;
            LblUnitSelection.CornerRadius = 0;
            LblUnitSelection.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblUnitSelection.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblUnitSelection.HighlightPrompt = defaultHighlightPrompt66;
            LblUnitSelection.IsOmittext = true;
            LblUnitSelection.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblUnitSelection.Location = new System.Drawing.Point(10, 76);
            LblUnitSelection.MultyLineFlag = false;
            LblUnitSelection.Name = "LblUnitSelection";
            LblUnitSelection.Size = new System.Drawing.Size(100, 20);
            LblUnitSelection.StyleFlags = UserControls.Style.StyleFlag.None;
            LblUnitSelection.StylizeFlag = true;
            LblUnitSelection.TabIndex = 92;
            LblUnitSelection.TabStop = false;
            LblUnitSelection.Text = "备用单位";
            LblUnitSelection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblUnitSelection.Token = null;
            LblUnitSelection.ToolTip = true;
            // 
            // BtnUnitRatio
            // 
            BtnUnitRatio.Adjustable = false;
            BtnUnitRatio.BackColor = System.Drawing.Color.Transparent;
            BtnUnitRatio.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnUnitRatio.BorderThickness = 0;
            BtnUnitRatio.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnUnitRatio.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnUnitRatio.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnUnitRatio.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnUnitRatio.CornerRadius = 0;
            BtnUnitRatio.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnUnitRatio.DaskArray = null;
            BtnUnitRatio.DoubleClickEnable = true;
            BtnUnitRatio.DropKey = System.Windows.Forms.Keys.Space;
            BtnUnitRatio.FineEnable = false;
            BtnUnitRatio.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnUnitRatio.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnUnitRatio.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnUnitRatio.Height = 30;
            BtnUnitRatio.Icon = null;
            BtnUnitRatio.IconOffset = 10;
            BtnUnitRatio.IconSize = new System.Drawing.Size(24, 24);
            BtnUnitRatio.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnUnitRatio.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnUnitRatio.IsChoosed = false;
            BtnUnitRatio.IsIndicatorShow = false;
            BtnUnitRatio.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnUnitRatio.Location = new System.Drawing.Point(12, 177);
            BtnUnitRatio.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnUnitRatio.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnUnitRatio.MouseInBorderThickness = 0;
            BtnUnitRatio.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnUnitRatio.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnUnitRatio.Name = "BtnUnitRatio";
            BtnUnitRatio.PressedBackColor = System.Drawing.Color.Gray;
            BtnUnitRatio.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnUnitRatio.PressedBorderThickness = 0;
            BtnUnitRatio.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnUnitRatio.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnUnitRatio.Size = new System.Drawing.Size(98, 30);
            BtnUnitRatio.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnUnitRatio.StylizeFlag = true;
            BtnUnitRatio.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnUnitRatio.SVGPath = "";
            BtnUnitRatio.TabIndex = 95;
            BtnUnitRatio.Text = "比率值";
            BtnUnitRatio.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnUnitRatio.Visible = false;
            BtnUnitRatio.DoubleClick += BtnUnitRatio_Click;
            // 
            // LblUnitCvtText
            // 
            LblUnitCvtText.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblUnitCvtText.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblUnitCvtText.BorderThickness = 0;
            LblUnitCvtText.CornerRadius = 0;
            LblUnitCvtText.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblUnitCvtText.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblUnitCvtText.HighlightPrompt = defaultHighlightPrompt67;
            LblUnitCvtText.IsOmittext = true;
            LblUnitCvtText.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblUnitCvtText.Location = new System.Drawing.Point(12, 148);
            LblUnitCvtText.MultyLineFlag = false;
            LblUnitCvtText.Name = "LblUnitCvtText";
            LblUnitCvtText.Size = new System.Drawing.Size(89, 18);
            LblUnitCvtText.StyleFlags = UserControls.Style.StyleFlag.None;
            LblUnitCvtText.StylizeFlag = true;
            LblUnitCvtText.TabIndex = 94;
            LblUnitCvtText.TabStop = false;
            LblUnitCvtText.Text = "单位比率";
            LblUnitCvtText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblUnitCvtText.Token = null;
            LblUnitCvtText.ToolTip = true;
            LblUnitCvtText.Visible = false;
            // 
            // BtnProbeDefault
            // 
            BtnProbeDefault.Adjustable = false;
            BtnProbeDefault.BackColor = System.Drawing.Color.Transparent;
            BtnProbeDefault.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnProbeDefault.BorderThickness = 1;
            BtnProbeDefault.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnProbeDefault.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnProbeDefault.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnProbeDefault.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnProbeDefault.CornerRadius = 0;
            BtnProbeDefault.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnProbeDefault.DaskArray = null;
            BtnProbeDefault.DoubleClickEnable = true;
            BtnProbeDefault.DropKey = System.Windows.Forms.Keys.Space;
            BtnProbeDefault.FineEnable = false;
            BtnProbeDefault.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnProbeDefault.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnProbeDefault.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnProbeDefault.Height = 30;
            BtnProbeDefault.Icon = null;
            BtnProbeDefault.IconOffset = 10;
            BtnProbeDefault.IconSize = new System.Drawing.Size(24, 24);
            BtnProbeDefault.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnProbeDefault.IsChoosed = false;
            BtnProbeDefault.IsIndicatorShow = false;
            BtnProbeDefault.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnProbeDefault.Location = new System.Drawing.Point(428, 257);
            BtnProbeDefault.Margin = new System.Windows.Forms.Padding(2);
            BtnProbeDefault.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnProbeDefault.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnProbeDefault.MouseInBorderThickness = 0;
            BtnProbeDefault.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnProbeDefault.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnProbeDefault.Name = "BtnProbeDefault";
            BtnProbeDefault.PressedBackColor = System.Drawing.Color.Gray;
            BtnProbeDefault.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnProbeDefault.PressedBorderThickness = 0;
            BtnProbeDefault.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnProbeDefault.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnProbeDefault.Size = new System.Drawing.Size(100, 30);
            BtnProbeDefault.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnProbeDefault.StylizeFlag = true;
            BtnProbeDefault.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnProbeDefault.SVGPath = "";
            BtnProbeDefault.TabIndex = 96;
            BtnProbeDefault.Text = "恢复默认";
            BtnProbeDefault.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnProbeDefault.Click += BtnProbeBtnProbeDefault_Click;
            // 
            // ChkProbeUnitCustom
            // 
            ChkProbeUnitCustom.AnimationCount = 8;
            ChkProbeUnitCustom.AnimationFunc = null;
            ChkProbeUnitCustom.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkProbeUnitCustom.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkProbeUnitCustom.BorderThickness = 0;
            ChkProbeUnitCustom.Checked = false;
            ChkProbeUnitCustom.CheckedBackColor = System.Drawing.Color.FromArgb(0, 157, 255);
            ChkProbeUnitCustom.CheckedForeColor = System.Drawing.Color.Black;
            ChkProbeUnitCustom.CheckedSliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkProbeUnitCustom.CheckedText = "开";
            ChkProbeUnitCustom.Cursor = System.Windows.Forms.Cursors.Hand;
            ChkProbeUnitCustom.DropKey = System.Windows.Forms.Keys.Space;
            ChkProbeUnitCustom.FocusBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            ChkProbeUnitCustom.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            ChkProbeUnitCustom.Height = 30;
            ChkProbeUnitCustom.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            ChkProbeUnitCustom.Location = new System.Drawing.Point(12, 100);
            ChkProbeUnitCustom.Margin = new System.Windows.Forms.Padding(0);
            ChkProbeUnitCustom.Name = "ChkProbeUnitCustom";
            ChkProbeUnitCustom.Size = new System.Drawing.Size(98, 30);
            ChkProbeUnitCustom.SliderButtonWidth = 30;
            ChkProbeUnitCustom.SliderColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ChkProbeUnitCustom.StyleFlags = UserControls.Style.StyleFlag.None;
            ChkProbeUnitCustom.StylizeFlag = true;
            ChkProbeUnitCustom.SwitchShape = UserControls.ScopeXSwitchButton.Shape.Square;
            ChkProbeUnitCustom.TabIndex = 97;
            ChkProbeUnitCustom.Text = "关";
            ChkProbeUnitCustom.UseAnimation = true;
            ChkProbeUnitCustom.Click += ChkProbeUnitCustom_Click;
            // 
            // BtnUnitRatioInverted
            // 
            BtnUnitRatioInverted.Adjustable = false;
            BtnUnitRatioInverted.BackColor = System.Drawing.Color.Transparent;
            BtnUnitRatioInverted.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnUnitRatioInverted.BorderThickness = 0;
            BtnUnitRatioInverted.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnUnitRatioInverted.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnUnitRatioInverted.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnUnitRatioInverted.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnUnitRatioInverted.CornerRadius = 0;
            BtnUnitRatioInverted.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnUnitRatioInverted.DaskArray = null;
            BtnUnitRatioInverted.DoubleClickEnable = true;
            BtnUnitRatioInverted.DropKey = System.Windows.Forms.Keys.Space;
            BtnUnitRatioInverted.FineEnable = false;
            BtnUnitRatioInverted.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnUnitRatioInverted.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnUnitRatioInverted.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnUnitRatioInverted.Height = 30;
            BtnUnitRatioInverted.Icon = null;
            BtnUnitRatioInverted.IconOffset = 10;
            BtnUnitRatioInverted.IconSize = new System.Drawing.Size(24, 24);
            BtnUnitRatioInverted.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnUnitRatioInverted.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnUnitRatioInverted.IsChoosed = false;
            BtnUnitRatioInverted.IsIndicatorShow = false;
            BtnUnitRatioInverted.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnUnitRatioInverted.Location = new System.Drawing.Point(122, 177);
            BtnUnitRatioInverted.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnUnitRatioInverted.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnUnitRatioInverted.MouseInBorderThickness = 0;
            BtnUnitRatioInverted.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnUnitRatioInverted.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnUnitRatioInverted.Name = "BtnUnitRatioInverted";
            BtnUnitRatioInverted.PressedBackColor = System.Drawing.Color.Gray;
            BtnUnitRatioInverted.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnUnitRatioInverted.PressedBorderThickness = 0;
            BtnUnitRatioInverted.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnUnitRatioInverted.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnUnitRatioInverted.Size = new System.Drawing.Size(100, 30);
            BtnUnitRatioInverted.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnUnitRatioInverted.StylizeFlag = true;
            BtnUnitRatioInverted.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnUnitRatioInverted.SVGPath = "";
            BtnUnitRatioInverted.TabIndex = 98;
            BtnUnitRatioInverted.Text = "比率值";
            BtnUnitRatioInverted.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnUnitRatioInverted.Visible = false;
            BtnUnitRatioInverted.DoubleClick += BtnUnitRatioInverted_Click;
            // 
            // LblDCBiasSetting
            // 
            LblDCBiasSetting.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblDCBiasSetting.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            LblDCBiasSetting.BorderThickness = 0;
            LblDCBiasSetting.CornerRadius = 0;
            LblDCBiasSetting.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblDCBiasSetting.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblDCBiasSetting.HighlightPrompt = defaultHighlightPrompt68;
            LblDCBiasSetting.IsOmittext = true;
            LblDCBiasSetting.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblDCBiasSetting.Location = new System.Drawing.Point(12, 228);
            LblDCBiasSetting.MultyLineFlag = false;
            LblDCBiasSetting.Name = "LblDCBiasSetting";
            LblDCBiasSetting.Size = new System.Drawing.Size(100, 18);
            LblDCBiasSetting.StyleFlags = UserControls.Style.StyleFlag.None;
            LblDCBiasSetting.StylizeFlag = true;
            LblDCBiasSetting.TabIndex = 99;
            LblDCBiasSetting.TabStop = false;
            LblDCBiasSetting.Text = "直流偏设置";
            LblDCBiasSetting.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblDCBiasSetting.Token = null;
            LblDCBiasSetting.ToolTip = true;
            LblDCBiasSetting.Visible = false;
            // 
            // BtnDCBiasSetting
            // 
            BtnDCBiasSetting.Adjustable = false;
            BtnDCBiasSetting.BackColor = System.Drawing.Color.Transparent;
            BtnDCBiasSetting.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnDCBiasSetting.BorderThickness = 0;
            BtnDCBiasSetting.ChoosedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnDCBiasSetting.ChoosedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnDCBiasSetting.ChoosedMouseinBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnDCBiasSetting.ChoosedPressedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnDCBiasSetting.CornerRadius = 0;
            BtnDCBiasSetting.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnDCBiasSetting.DaskArray = null;
            BtnDCBiasSetting.DoubleClickEnable = true;
            BtnDCBiasSetting.DropKey = System.Windows.Forms.Keys.Space;
            BtnDCBiasSetting.FineEnable = false;
            BtnDCBiasSetting.FocusedBorderColor = System.Drawing.Color.DeepSkyBlue;
            BtnDCBiasSetting.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnDCBiasSetting.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BtnDCBiasSetting.Height = 30;
            BtnDCBiasSetting.Icon = null;
            BtnDCBiasSetting.IconOffset = 10;
            BtnDCBiasSetting.IconSize = new System.Drawing.Size(24, 24);
            BtnDCBiasSetting.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            BtnDCBiasSetting.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnDCBiasSetting.IsChoosed = false;
            BtnDCBiasSetting.IsIndicatorShow = false;
            BtnDCBiasSetting.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnDCBiasSetting.Location = new System.Drawing.Point(12, 257);
            BtnDCBiasSetting.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnDCBiasSetting.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnDCBiasSetting.MouseInBorderThickness = 0;
            BtnDCBiasSetting.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnDCBiasSetting.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnDCBiasSetting.Name = "BtnDCBiasSetting";
            BtnDCBiasSetting.PressedBackColor = System.Drawing.Color.Gray;
            BtnDCBiasSetting.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnDCBiasSetting.PressedBorderThickness = 0;
            BtnDCBiasSetting.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnDCBiasSetting.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnDCBiasSetting.Size = new System.Drawing.Size(98, 30);
            BtnDCBiasSetting.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnDCBiasSetting.StylizeFlag = true;
            BtnDCBiasSetting.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnDCBiasSetting.SVGPath = "";
            BtnDCBiasSetting.TabIndex = 100;
            BtnDCBiasSetting.TabStop = false;
            BtnDCBiasSetting.Text = "0V";
            BtnDCBiasSetting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnDCBiasSetting.Visible = false;
            BtnDCBiasSetting.Click += BtnDCBiasSetting_Click;
            // 
            // ProbePage
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(BtnDCBiasSetting);
            Controls.Add(LblDCBiasSetting);
            Controls.Add(BtnUnitRatioInverted);
            Controls.Add(ChkProbeUnitCustom);
            Controls.Add(BtnProbeDefault);
            Controls.Add(BtnUnitRatio);
            Controls.Add(LblUnitCvtText);
            Controls.Add(TbxUnit);
            Controls.Add(LblUnitSelection);
            Controls.Add(CbxProbeMag);
            Controls.Add(BtnCustomGain);
            Controls.Add(LblCustomGain);
            Controls.Add(PnlInfo);
            Controls.Add(LblInfo);
            Controls.Add(CbxProbeBtnType);
            Controls.Add(LblProbeBtn);
            Controls.Add(LblProbeMag);
            Controls.Add(BtnProbeCali);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "ProbePage";
            Size = new System.Drawing.Size(539, 303);
            PnlInfo.ResumeLayout(false);
            TableProbeInfo.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ScopeX.UserControls.ScopeXLabel LblProbeMag;
        private ScopeX.UserControls.ScopeXIconButton BtnResetPos;
        private ScopeX.UserControls.ComboBoxEx CbxCoupling;
        private ScopeX.UserControls.ScopeXLabel LblCoupling;
        private ScopeX.UserControls.SelectComboBox CbxProbeBtnType;
        private ScopeX.UserControls.ScopeXLabel LblProbeBtn;
        private ScopeX.UserControls.ComboBoxEx CbxAttenuationType;
        private ScopeX.UserControls.ScopeXLabel LblInfo;
        private System.Windows.Forms.Panel PnlInfo;
        private System.Windows.Forms.TableLayoutPanel TableProbeInfo;
        private ScopeX.UserControls.ScopeXLabel LblMagName;
        private ScopeX.UserControls.ScopeXLabel LblManufacturerName;
        private ScopeX.UserControls.ScopeXLabel ScopeXLabel3;
        private ScopeX.UserControls.ScopeXLabel LblModelName;
        private ScopeX.UserControls.ScopeXLabel LblSNName;
        private ScopeX.UserControls.ScopeXLabel LblOtherName;
        private ScopeX.UserControls.ScopeXLabel ScopeXLabel6;
        private ScopeX.UserControls.ScopeXLabel ScopeXLabel7;
        private ScopeX.UserControls.ScopeXLabel ScopeXLabel8;
        private ScopeX.UserControls.ScopeXLabel LblManufacturer;
        private ScopeX.UserControls.ScopeXLabel LblModel;
        private ScopeX.UserControls.ScopeXLabel LblSN;
        private ScopeX.UserControls.ScopeXLabel LblOther;
        private ScopeX.UserControls.ScopeXLabel LblMag;
        private UserControls.ScopeXLabel LblCustomGain;
        private UserControls.ScopeXIconButton BtnCustomGain;
        private ScopeX.UserControls.SelectComboBox CbxProbeMag;
        private ScopeX.UserControls.ScopeXIconButton BtnProbeCali;
        private UserControls.ScopeXTextBox TbxUnit;
        private UserControls.ScopeXLabel LblUnitSelection;
        private UserControls.ScopeXIconButton BtnUnitRatio;
        private UserControls.ScopeXLabel LblUnitCvtText;
        private UserControls.ScopeXIconButton BtnProbeDefault;
        private UserControls.ScopeXSwitchButton ChkProbeUnitCustom;
        private UserControls.ScopeXIconButton scopexIconButton1;
        private UserControls.ScopeXIconButton BtnUnitRatioInverted;
        private UserControls.ScopeXLabel LblDCBiasSetting;
        private UserControls.ScopeXIconButton BtnDCBiasSetting;
    }
}
