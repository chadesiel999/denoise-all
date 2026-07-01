using ScopeX.UserControls;

namespace ScopeX.U2
{
    partial class ComponentSettingPage
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
            DefaultHighlightPrompt defaultHighlightPrompt12 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt2 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt8 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt11 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt10 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt9 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt3 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt5 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt1 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt6 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt7 = new DefaultHighlightPrompt();
            DefaultHighlightPrompt defaultHighlightPrompt4 = new DefaultHighlightPrompt();
            StAwg = new Switch();
            LblComponent = new ScopeXLabel();
            scopexLabel1 = new ScopeXLabel();
            scopexLabel7 = new ScopeXLabel();
            StJitter = new Switch();
            scopexLabel2 = new ScopeXLabel();
            StLissajous = new Switch();
            scopexLabel3 = new ScopeXLabel();
            StBus = new Switch();
            scopexLabel4 = new ScopeXLabel();
            StPower = new Switch();
            scopexLabel5 = new ScopeXLabel();
            scopexLabel6 = new ScopeXLabel();
            scopexLabel8 = new ScopeXLabel();
            StSearch = new Switch();
            StPassFail = new Switch();
            StMeasure = new Switch();
            StSegement = new Switch();
            scopexLabel10 = new ScopeXLabel();
            scopexLabel11 = new ScopeXLabel();
            StRef = new Switch();
            scopexLabel12 = new ScopeXLabel();
            StMath = new Switch();
            TlpCursor = new System.Windows.Forms.TableLayoutPanel();
            TlpCursor.SuspendLayout();
            SuspendLayout();
            // 
            // StAwg
            // 
            StAwg.Checked = false;
            StAwg.FalseColor = System.Drawing.Color.Gray;
            StAwg.FalseTextColr = System.Drawing.Color.White;
            StAwg.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            StAwg.Location = new System.Drawing.Point(368, 194);
            StAwg.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            StAwg.Name = "StAwg";
            StAwg.Size = new System.Drawing.Size(90, 30);
            StAwg.StyleFlags = UserControls.Style.StyleFlag.None;
            StAwg.StylizeFlag = true;
            StAwg.SwitchType = SwitchType.Ellipse;
            StAwg.TabIndex = 59;
            StAwg.Texts = new string[] { "已启用", "已禁用" };
            StAwg.TrueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            StAwg.TrueTextColr = System.Drawing.Color.White;
            StAwg.CheckedChanged += StAwg_CheckedChanged;
            // 
            // LblComponent
            // 
            LblComponent.BackColor = System.Drawing.Color.Empty;
            LblComponent.BorderColor = System.Drawing.Color.Black;
            LblComponent.BorderThickness = 0;
            TlpCursor.SetColumnSpan(LblComponent, 6);
            LblComponent.CornerRadius = 0;
            LblComponent.Dock = System.Windows.Forms.DockStyle.Fill;
            LblComponent.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblComponent.ForeColor = System.Drawing.Color.White;
            LblComponent.HighlightPrompt = defaultHighlightPrompt12;
            LblComponent.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            LblComponent.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblComponent.Location = new System.Drawing.Point(0, 0);
            LblComponent.Margin = new System.Windows.Forms.Padding(0);
            LblComponent.MultyLineFlag = false;
            LblComponent.Name = "LblComponent";
            LblComponent.Size = new System.Drawing.Size(459, 38);
            LblComponent.StyleFlags = UserControls.Style.StyleFlag.None;
            LblComponent.StylizeFlag = true;
            LblComponent.TabIndex = 60;
            LblComponent.TabStop = false;
            LblComponent.Text = "可选组件";
            LblComponent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            LblComponent.Token = null;
            // 
            // scopexLabel1
            // 
            scopexLabel1.BackColor = System.Drawing.Color.Empty;
            scopexLabel1.BorderColor = System.Drawing.Color.Black;
            scopexLabel1.BorderThickness = 0;
            scopexLabel1.CornerRadius = 0;
            scopexLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            scopexLabel1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            scopexLabel1.ForeColor = System.Drawing.Color.White;
            scopexLabel1.HighlightPrompt = defaultHighlightPrompt2;
            scopexLabel1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            scopexLabel1.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            scopexLabel1.Location = new System.Drawing.Point(264, 190);
            scopexLabel1.Margin = new System.Windows.Forms.Padding(0);
            scopexLabel1.MultyLineFlag = false;
            scopexLabel1.Name = "scopexLabel1";
            scopexLabel1.Size = new System.Drawing.Size(104, 38);
            scopexLabel1.StyleFlags = UserControls.Style.StyleFlag.None;
            scopexLabel1.StylizeFlag = true;
            scopexLabel1.TabIndex = 62;
            scopexLabel1.TabStop = false;
            scopexLabel1.Text = "AWG";
            scopexLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            scopexLabel1.Token = null;
            // 
            // scopexLabel7
            // 
            scopexLabel7.BackColor = System.Drawing.Color.Empty;
            scopexLabel7.BorderColor = System.Drawing.Color.Black;
            scopexLabel7.BorderThickness = 0;
            scopexLabel7.CornerRadius = 0;
            scopexLabel7.Dock = System.Windows.Forms.DockStyle.Fill;
            scopexLabel7.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            scopexLabel7.ForeColor = System.Drawing.Color.White;
            scopexLabel7.HighlightPrompt = defaultHighlightPrompt8;
            scopexLabel7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            scopexLabel7.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            scopexLabel7.Location = new System.Drawing.Point(30, 190);
            scopexLabel7.Margin = new System.Windows.Forms.Padding(0);
            scopexLabel7.MultyLineFlag = false;
            scopexLabel7.Name = "scopexLabel7";
            scopexLabel7.Size = new System.Drawing.Size(104, 38);
            scopexLabel7.StyleFlags = UserControls.Style.StyleFlag.None;
            scopexLabel7.StylizeFlag = true;
            scopexLabel7.TabIndex = 74;
            scopexLabel7.TabStop = false;
            scopexLabel7.Text = "Jitter";
            scopexLabel7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            scopexLabel7.Token = null;
            // 
            // StJitter
            // 
            StJitter.Checked = false;
            StJitter.FalseColor = System.Drawing.Color.Gray;
            StJitter.FalseTextColr = System.Drawing.Color.White;
            StJitter.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            StJitter.Location = new System.Drawing.Point(134, 194);
            StJitter.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            StJitter.Name = "StJitter";
            StJitter.Size = new System.Drawing.Size(90, 30);
            StJitter.StyleFlags = UserControls.Style.StyleFlag.None;
            StJitter.StylizeFlag = true;
            StJitter.SwitchType = SwitchType.Ellipse;
            StJitter.TabIndex = 73;
            StJitter.Texts = new string[] { "已启用", "已禁用" };
            StJitter.TrueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            StJitter.TrueTextColr = System.Drawing.Color.White;
            StJitter.CheckedChanged += StJitter_CheckedChanged;
            // 
            // scopexLabel2
            // 
            scopexLabel2.BackColor = System.Drawing.Color.Empty;
            scopexLabel2.BorderColor = System.Drawing.Color.Black;
            scopexLabel2.BorderThickness = 0;
            scopexLabel2.CornerRadius = 0;
            scopexLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            scopexLabel2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            scopexLabel2.ForeColor = System.Drawing.Color.White;
            scopexLabel2.HighlightPrompt = defaultHighlightPrompt11;
            scopexLabel2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            scopexLabel2.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            scopexLabel2.Location = new System.Drawing.Point(30, 152);
            scopexLabel2.Margin = new System.Windows.Forms.Padding(0);
            scopexLabel2.MultyLineFlag = false;
            scopexLabel2.Name = "scopexLabel2";
            scopexLabel2.Size = new System.Drawing.Size(104, 38);
            scopexLabel2.StyleFlags = UserControls.Style.StyleFlag.None;
            scopexLabel2.StylizeFlag = true;
            scopexLabel2.TabIndex = 76;
            scopexLabel2.TabStop = false;
            scopexLabel2.Text = "Lissajous";
            scopexLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            scopexLabel2.Token = null;
            // 
            // StLissajous
            // 
            StLissajous.Checked = false;
            StLissajous.FalseColor = System.Drawing.Color.Gray;
            StLissajous.FalseTextColr = System.Drawing.Color.White;
            StLissajous.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            StLissajous.Location = new System.Drawing.Point(134, 156);
            StLissajous.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            StLissajous.Name = "StLissajous";
            StLissajous.Size = new System.Drawing.Size(90, 30);
            StLissajous.StyleFlags = UserControls.Style.StyleFlag.None;
            StLissajous.StylizeFlag = true;
            StLissajous.SwitchType = SwitchType.Ellipse;
            StLissajous.TabIndex = 75;
            StLissajous.Texts = new string[] { "已启用", "已禁用" };
            StLissajous.TrueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            StLissajous.TrueTextColr = System.Drawing.Color.White;
            StLissajous.CheckedChanged += StLissajous_CheckedChanged;
            // 
            // scopexLabel3
            // 
            scopexLabel3.BackColor = System.Drawing.Color.Empty;
            scopexLabel3.BorderColor = System.Drawing.Color.Black;
            scopexLabel3.BorderThickness = 0;
            scopexLabel3.CornerRadius = 0;
            scopexLabel3.Dock = System.Windows.Forms.DockStyle.Fill;
            scopexLabel3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            scopexLabel3.ForeColor = System.Drawing.Color.White;
            scopexLabel3.HighlightPrompt = defaultHighlightPrompt10;
            scopexLabel3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            scopexLabel3.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            scopexLabel3.Location = new System.Drawing.Point(30, 38);
            scopexLabel3.Margin = new System.Windows.Forms.Padding(0);
            scopexLabel3.MultyLineFlag = false;
            scopexLabel3.Name = "scopexLabel3";
            scopexLabel3.Size = new System.Drawing.Size(104, 38);
            scopexLabel3.StyleFlags = UserControls.Style.StyleFlag.None;
            scopexLabel3.StylizeFlag = true;
            scopexLabel3.TabIndex = 80;
            scopexLabel3.TabStop = false;
            scopexLabel3.Text = "BUS";
            scopexLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            scopexLabel3.Token = null;
            // 
            // StBus
            // 
            StBus.Checked = false;
            StBus.FalseColor = System.Drawing.Color.Gray;
            StBus.FalseTextColr = System.Drawing.Color.White;
            StBus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            StBus.Location = new System.Drawing.Point(134, 42);
            StBus.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            StBus.Name = "StBus";
            StBus.Size = new System.Drawing.Size(90, 30);
            StBus.StyleFlags = UserControls.Style.StyleFlag.None;
            StBus.StylizeFlag = true;
            StBus.SwitchType = SwitchType.Ellipse;
            StBus.TabIndex = 79;
            StBus.Texts = new string[] { "已启用", "已禁用" };
            StBus.TrueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            StBus.TrueTextColr = System.Drawing.Color.White;
            StBus.CheckedChanged += StBus_CheckedChanged;
            // 
            // scopexLabel4
            // 
            scopexLabel4.BackColor = System.Drawing.Color.Empty;
            scopexLabel4.BorderColor = System.Drawing.Color.Black;
            scopexLabel4.BorderThickness = 0;
            TlpCursor.SetColumnSpan(scopexLabel4, 2);
            scopexLabel4.CornerRadius = 0;
            scopexLabel4.Dock = System.Windows.Forms.DockStyle.Fill;
            scopexLabel4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            scopexLabel4.ForeColor = System.Drawing.Color.White;
            scopexLabel4.HighlightPrompt = defaultHighlightPrompt9;
            scopexLabel4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            scopexLabel4.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            scopexLabel4.Location = new System.Drawing.Point(0, 228);
            scopexLabel4.Margin = new System.Windows.Forms.Padding(0);
            scopexLabel4.MultyLineFlag = false;
            scopexLabel4.Name = "scopexLabel4";
            scopexLabel4.Size = new System.Drawing.Size(134, 43);
            scopexLabel4.StyleFlags = UserControls.Style.StyleFlag.None;
            scopexLabel4.StylizeFlag = true;
            scopexLabel4.TabIndex = 78;
            scopexLabel4.TabStop = false;
            scopexLabel4.Text = "PowerAnalysis";
            scopexLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            scopexLabel4.Token = null;
            // 
            // StPower
            // 
            StPower.Checked = false;
            StPower.FalseColor = System.Drawing.Color.Gray;
            StPower.FalseTextColr = System.Drawing.Color.White;
            StPower.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            StPower.Location = new System.Drawing.Point(134, 235);
            StPower.Margin = new System.Windows.Forms.Padding(0, 7, 0, 0);
            StPower.Name = "StPower";
            StPower.Size = new System.Drawing.Size(90, 30);
            StPower.StyleFlags = UserControls.Style.StyleFlag.None;
            StPower.StylizeFlag = true;
            StPower.SwitchType = SwitchType.Ellipse;
            StPower.TabIndex = 77;
            StPower.Texts = new string[] { "已启用", "已禁用" };
            StPower.TrueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            StPower.TrueTextColr = System.Drawing.Color.White;
            StPower.CheckedChanged += StPower_CheckedChanged;
            // 
            // scopexLabel5
            // 
            scopexLabel5.BackColor = System.Drawing.Color.Empty;
            scopexLabel5.BorderColor = System.Drawing.Color.Black;
            scopexLabel5.BorderThickness = 0;
            scopexLabel5.CornerRadius = 0;
            scopexLabel5.Dock = System.Windows.Forms.DockStyle.Fill;
            scopexLabel5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            scopexLabel5.ForeColor = System.Drawing.Color.White;
            scopexLabel5.HighlightPrompt = defaultHighlightPrompt3;
            scopexLabel5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            scopexLabel5.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            scopexLabel5.Location = new System.Drawing.Point(264, 114);
            scopexLabel5.Margin = new System.Windows.Forms.Padding(0);
            scopexLabel5.MultyLineFlag = false;
            scopexLabel5.Name = "scopexLabel5";
            scopexLabel5.Size = new System.Drawing.Size(104, 38);
            scopexLabel5.StyleFlags = UserControls.Style.StyleFlag.None;
            scopexLabel5.StylizeFlag = true;
            scopexLabel5.TabIndex = 81;
            scopexLabel5.TabStop = false;
            scopexLabel5.Text = "Search";
            scopexLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            scopexLabel5.Token = null;
            // 
            // scopexLabel6
            // 
            scopexLabel6.BackColor = System.Drawing.Color.Empty;
            scopexLabel6.BorderColor = System.Drawing.Color.Black;
            scopexLabel6.BorderThickness = 0;
            scopexLabel6.CornerRadius = 0;
            scopexLabel6.Dock = System.Windows.Forms.DockStyle.Fill;
            scopexLabel6.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            scopexLabel6.ForeColor = System.Drawing.Color.White;
            scopexLabel6.HighlightPrompt = defaultHighlightPrompt5;
            scopexLabel6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            scopexLabel6.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            scopexLabel6.Location = new System.Drawing.Point(264, 76);
            scopexLabel6.Margin = new System.Windows.Forms.Padding(0);
            scopexLabel6.MultyLineFlag = false;
            scopexLabel6.Name = "scopexLabel6";
            scopexLabel6.Size = new System.Drawing.Size(104, 38);
            scopexLabel6.StyleFlags = UserControls.Style.StyleFlag.None;
            scopexLabel6.StylizeFlag = true;
            scopexLabel6.TabIndex = 82;
            scopexLabel6.TabStop = false;
            scopexLabel6.Text = "P/F";
            scopexLabel6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            scopexLabel6.Token = null;
            // 
            // scopexLabel8
            // 
            scopexLabel8.BackColor = System.Drawing.Color.Empty;
            scopexLabel8.BorderColor = System.Drawing.Color.Black;
            scopexLabel8.BorderThickness = 0;
            scopexLabel8.CornerRadius = 0;
            scopexLabel8.Dock = System.Windows.Forms.DockStyle.Fill;
            scopexLabel8.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            scopexLabel8.ForeColor = System.Drawing.Color.White;
            scopexLabel8.HighlightPrompt = defaultHighlightPrompt1;
            scopexLabel8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            scopexLabel8.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            scopexLabel8.Location = new System.Drawing.Point(264, 152);
            scopexLabel8.Margin = new System.Windows.Forms.Padding(0);
            scopexLabel8.MultyLineFlag = false;
            scopexLabel8.Name = "scopexLabel8";
            scopexLabel8.Size = new System.Drawing.Size(104, 38);
            scopexLabel8.StyleFlags = UserControls.Style.StyleFlag.None;
            scopexLabel8.StylizeFlag = true;
            scopexLabel8.TabIndex = 83;
            scopexLabel8.TabStop = false;
            scopexLabel8.Text = "Measure";
            scopexLabel8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            scopexLabel8.Token = null;
            // 
            // StSearch
            // 
            StSearch.Checked = false;
            StSearch.FalseColor = System.Drawing.Color.Gray;
            StSearch.FalseTextColr = System.Drawing.Color.White;
            StSearch.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            StSearch.Location = new System.Drawing.Point(368, 118);
            StSearch.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            StSearch.Name = "StSearch";
            StSearch.Size = new System.Drawing.Size(90, 30);
            StSearch.StyleFlags = UserControls.Style.StyleFlag.None;
            StSearch.StylizeFlag = true;
            StSearch.SwitchType = SwitchType.Ellipse;
            StSearch.TabIndex = 84;
            StSearch.Texts = new string[] { "已启用", "已禁用" };
            StSearch.TrueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            StSearch.TrueTextColr = System.Drawing.Color.White;
            StSearch.CheckedChanged += StSearch_CheckedChanged;
            // 
            // StPassFail
            // 
            StPassFail.Checked = false;
            StPassFail.FalseColor = System.Drawing.Color.Gray;
            StPassFail.FalseTextColr = System.Drawing.Color.White;
            StPassFail.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            StPassFail.Location = new System.Drawing.Point(368, 80);
            StPassFail.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            StPassFail.Name = "StPassFail";
            StPassFail.Size = new System.Drawing.Size(90, 30);
            StPassFail.StyleFlags = UserControls.Style.StyleFlag.None;
            StPassFail.StylizeFlag = true;
            StPassFail.SwitchType = SwitchType.Ellipse;
            StPassFail.TabIndex = 85;
            StPassFail.Texts = new string[] { "已启用", "已禁用" };
            StPassFail.TrueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            StPassFail.TrueTextColr = System.Drawing.Color.White;
            StPassFail.CheckedChanged += StPassFail_CheckedChanged;
            // 
            // StMeasure
            // 
            StMeasure.Checked = false;
            StMeasure.FalseColor = System.Drawing.Color.Gray;
            StMeasure.FalseTextColr = System.Drawing.Color.White;
            StMeasure.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            StMeasure.Location = new System.Drawing.Point(368, 156);
            StMeasure.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            StMeasure.Name = "StMeasure";
            StMeasure.Size = new System.Drawing.Size(90, 30);
            StMeasure.StyleFlags = UserControls.Style.StyleFlag.None;
            StMeasure.StylizeFlag = true;
            StMeasure.SwitchType = SwitchType.Ellipse;
            StMeasure.TabIndex = 86;
            StMeasure.Texts = new string[] { "已启用", "已禁用" };
            StMeasure.TrueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            StMeasure.TrueTextColr = System.Drawing.Color.White;
            StMeasure.CheckedChanged += StMeasure_CheckedChanged;
            // 
            // StSegement
            // 
            StSegement.Checked = false;
            StSegement.FalseColor = System.Drawing.Color.Gray;
            StSegement.FalseTextColr = System.Drawing.Color.White;
            StSegement.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            StSegement.Location = new System.Drawing.Point(134, 118);
            StSegement.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            StSegement.Name = "StSegement";
            StSegement.Size = new System.Drawing.Size(90, 30);
            StSegement.StyleFlags = UserControls.Style.StyleFlag.None;
            StSegement.StylizeFlag = true;
            StSegement.SwitchType = SwitchType.Ellipse;
            StSegement.TabIndex = 90;
            StSegement.Texts = new string[] { "已启用", "已禁用" };
            StSegement.TrueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            StSegement.TrueTextColr = System.Drawing.Color.White;
            StSegement.CheckedChanged += StSegement_CheckedChanged;
            // 
            // scopexLabel10
            // 
            scopexLabel10.BackColor = System.Drawing.Color.Empty;
            scopexLabel10.BorderColor = System.Drawing.Color.Black;
            scopexLabel10.BorderThickness = 0;
            scopexLabel10.CornerRadius = 0;
            scopexLabel10.Dock = System.Windows.Forms.DockStyle.Fill;
            scopexLabel10.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            scopexLabel10.ForeColor = System.Drawing.Color.White;
            scopexLabel10.HighlightPrompt = defaultHighlightPrompt6;
            scopexLabel10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            scopexLabel10.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            scopexLabel10.Location = new System.Drawing.Point(30, 114);
            scopexLabel10.Margin = new System.Windows.Forms.Padding(0);
            scopexLabel10.MultyLineFlag = false;
            scopexLabel10.Name = "scopexLabel10";
            scopexLabel10.Size = new System.Drawing.Size(104, 38);
            scopexLabel10.StyleFlags = UserControls.Style.StyleFlag.None;
            scopexLabel10.StylizeFlag = true;
            scopexLabel10.TabIndex = 89;
            scopexLabel10.TabStop = false;
            scopexLabel10.Text = "Segement";
            scopexLabel10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            scopexLabel10.Token = null;
            // 
            // scopexLabel11
            // 
            scopexLabel11.BackColor = System.Drawing.Color.Empty;
            scopexLabel11.BorderColor = System.Drawing.Color.Black;
            scopexLabel11.BorderThickness = 0;
            scopexLabel11.CornerRadius = 0;
            scopexLabel11.Dock = System.Windows.Forms.DockStyle.Fill;
            scopexLabel11.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            scopexLabel11.ForeColor = System.Drawing.Color.White;
            scopexLabel11.HighlightPrompt = defaultHighlightPrompt7;
            scopexLabel11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            scopexLabel11.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            scopexLabel11.Location = new System.Drawing.Point(30, 76);
            scopexLabel11.Margin = new System.Windows.Forms.Padding(0);
            scopexLabel11.MultyLineFlag = false;
            scopexLabel11.Name = "scopexLabel11";
            scopexLabel11.Size = new System.Drawing.Size(104, 38);
            scopexLabel11.StyleFlags = UserControls.Style.StyleFlag.None;
            scopexLabel11.StylizeFlag = true;
            scopexLabel11.TabIndex = 91;
            scopexLabel11.TabStop = false;
            scopexLabel11.Text = "Ref";
            scopexLabel11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            scopexLabel11.Token = null;
            // 
            // StRef
            // 
            StRef.Checked = false;
            StRef.FalseColor = System.Drawing.Color.Gray;
            StRef.FalseTextColr = System.Drawing.Color.White;
            StRef.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            StRef.Location = new System.Drawing.Point(134, 80);
            StRef.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            StRef.Name = "StRef";
            StRef.Size = new System.Drawing.Size(90, 30);
            StRef.StyleFlags = UserControls.Style.StyleFlag.None;
            StRef.StylizeFlag = true;
            StRef.SwitchType = SwitchType.Ellipse;
            StRef.TabIndex = 92;
            StRef.Texts = new string[] { "已启用", "已禁用" };
            StRef.TrueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            StRef.TrueTextColr = System.Drawing.Color.White;
            StRef.CheckedChanged += StRef_CheckedChanged;
            // 
            // scopexLabel12
            // 
            scopexLabel12.BackColor = System.Drawing.Color.Empty;
            scopexLabel12.BorderColor = System.Drawing.Color.Black;
            scopexLabel12.BorderThickness = 0;
            scopexLabel12.CornerRadius = 0;
            scopexLabel12.Dock = System.Windows.Forms.DockStyle.Fill;
            scopexLabel12.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            scopexLabel12.ForeColor = System.Drawing.Color.White;
            scopexLabel12.HighlightPrompt = defaultHighlightPrompt4;
            scopexLabel12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            scopexLabel12.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            scopexLabel12.Location = new System.Drawing.Point(264, 38);
            scopexLabel12.Margin = new System.Windows.Forms.Padding(0);
            scopexLabel12.MultyLineFlag = false;
            scopexLabel12.Name = "scopexLabel12";
            scopexLabel12.Size = new System.Drawing.Size(104, 38);
            scopexLabel12.StyleFlags = UserControls.Style.StyleFlag.None;
            scopexLabel12.StylizeFlag = true;
            scopexLabel12.TabIndex = 93;
            scopexLabel12.TabStop = false;
            scopexLabel12.Text = "Math";
            scopexLabel12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            scopexLabel12.Token = null;
            // 
            // StMath
            // 
            StMath.Checked = false;
            StMath.FalseColor = System.Drawing.Color.Gray;
            StMath.FalseTextColr = System.Drawing.Color.White;
            StMath.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            StMath.Location = new System.Drawing.Point(368, 42);
            StMath.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            StMath.Name = "StMath";
            StMath.Size = new System.Drawing.Size(90, 30);
            StMath.StyleFlags = UserControls.Style.StyleFlag.None;
            StMath.StylizeFlag = true;
            StMath.SwitchType = SwitchType.Ellipse;
            StMath.TabIndex = 94;
            StMath.Texts = new string[] { "已启用", "已禁用" };
            StMath.TrueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            StMath.TrueTextColr = System.Drawing.Color.White;
            StMath.CheckedChanged += StMath_CheckedChanged;
            // 
            // TlpCursor
            // 
            TlpCursor.BackColor = System.Drawing.Color.Transparent;
            TlpCursor.ColumnCount = 6;
            TlpCursor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            TlpCursor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TlpCursor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            TlpCursor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            TlpCursor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TlpCursor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            TlpCursor.Controls.Add(LblComponent, 0, 0);
            TlpCursor.Controls.Add(StMeasure, 5, 4);
            TlpCursor.Controls.Add(StMath, 5, 1);
            TlpCursor.Controls.Add(scopexLabel8, 4, 4);
            TlpCursor.Controls.Add(StSearch, 5, 3);
            TlpCursor.Controls.Add(StPassFail, 5, 2);
            TlpCursor.Controls.Add(scopexLabel1, 4, 5);
            TlpCursor.Controls.Add(scopexLabel5, 4, 3);
            TlpCursor.Controls.Add(scopexLabel12, 4, 1);
            TlpCursor.Controls.Add(StAwg, 5, 5);
            TlpCursor.Controls.Add(scopexLabel6, 4, 2);
            TlpCursor.Controls.Add(StSegement, 2, 3);
            TlpCursor.Controls.Add(StRef, 2, 2);
            TlpCursor.Controls.Add(scopexLabel10, 1, 3);
            TlpCursor.Controls.Add(scopexLabel11, 1, 2);
            TlpCursor.Controls.Add(scopexLabel7, 1, 5);
            TlpCursor.Controls.Add(scopexLabel4, 0, 6);
            TlpCursor.Controls.Add(StBus, 2, 1);
            TlpCursor.Controls.Add(scopexLabel3, 1, 1);
            TlpCursor.Controls.Add(StPower, 2, 6);
            TlpCursor.Controls.Add(StJitter, 2, 5);
            TlpCursor.Controls.Add(scopexLabel2, 1, 4);
            TlpCursor.Controls.Add(StLissajous, 2, 4);
            TlpCursor.Dock = System.Windows.Forms.DockStyle.Top;
            TlpCursor.Location = new System.Drawing.Point(0, 0);
            TlpCursor.Margin = new System.Windows.Forms.Padding(0);
            TlpCursor.Name = "TlpCursor";
            TlpCursor.RowCount = 7;
            TlpCursor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2857141F));
            TlpCursor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2857141F));
            TlpCursor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2857141F));
            TlpCursor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2857141F));
            TlpCursor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2857141F));
            TlpCursor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2857141F));
            TlpCursor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2857141F));
            TlpCursor.Size = new System.Drawing.Size(459, 271);
            TlpCursor.TabIndex = 95;
            // 
            // ComponentSettingPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            Controls.Add(TlpCursor);
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Name = "ComponentSettingPage";
            Size = new System.Drawing.Size(459, 410);
            TlpCursor.ResumeLayout(false);
            ResumeLayout(false);
        }
        #endregion
        private Switch StAwg;
        private UUVerticalSplitLine SplitLine;
        private UserControls.ScopeXLabel LblComponent;
        private UserControls.ScopeXLabel scopexLabel1;
        private UserControls.ScopeXLabel scopexLabel7;
        private Switch StJitter;
        private UserControls.ScopeXLabel scopexLabel2;
        private Switch StLissajous;
        private UserControls.ScopeXLabel scopexLabel3;
        private Switch StBus;
        private UserControls.ScopeXLabel scopexLabel4;
        private Switch StPower;
        private UserControls.ScopeXLabel scopexLabel5;
        private UserControls.ScopeXLabel scopexLabel6;
        private UserControls.ScopeXLabel scopexLabel8;
        private Switch StSearch;
        private Switch StPassFail;
        private Switch StMeasure;
        private Switch StSegement;
        private ScopeXLabel scopexLabel10;
        private ScopeXLabel scopexLabel11;
        private Switch StRef;
        private ScopeXLabel scopexLabel12;
        private Switch StMath;
        private System.Windows.Forms.TableLayoutPanel TlpCursor;
    }
}
