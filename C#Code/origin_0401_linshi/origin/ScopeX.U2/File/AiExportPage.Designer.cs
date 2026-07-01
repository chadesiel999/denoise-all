//using ScopeX.UserControls;
//using ScopeX.UserControls.Style;
//using System.Drawing;
//using System.Windows.Forms;

//namespace ScopeX.U2
//{
//    partial class AiExportPage
//    {
//        private System.ComponentModel.IContainer components = null;

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && (components != null))
//            {
//                components.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        #region 组件设计器生成的代码

//        private void InitializeComponent()
//        {
//            this.lblChannel = new ScopeX.UserControls.ScopeXLabel();
//            this.cmbChannel = new ScopeX.UserControls.ComboBoxEx();
//            this.lblFileName = new ScopeX.UserControls.ScopeXLabel();
//            this.txtFileName = new ScopeX.UserControls.ScopeXTextBox();
//            this.lblAuthor = new ScopeX.UserControls.ScopeXLabel();
//            this.txtAuthor = new ScopeX.UserControls.ScopeXTextBox();
//            this.lblCount = new ScopeX.UserControls.ScopeXLabel();
//            this.numCount = new System.Windows.Forms.NumericUpDown();
//            this.lblInterval = new ScopeX.UserControls.ScopeXLabel();
//            this.numInterval = new System.Windows.Forms.NumericUpDown();
//            this.lblMs = new ScopeX.UserControls.ScopeXLabel();

//            // --- 新增控件 ---
//            this.lblFormat = new ScopeX.UserControls.ScopeXLabel();
//            this.cmbFormat = new ScopeX.UserControls.ComboBoxEx();
//            // ----------------

//            this.lblTag = new ScopeX.UserControls.ScopeXLabel();
//            this.cmbTag = new ScopeX.UserControls.ComboBoxEx();
//            this.lblUsage = new ScopeX.UserControls.ScopeXLabel();
//            this.cmbUsage = new ScopeX.UserControls.ComboBoxEx();
//            this.lblNote = new ScopeX.UserControls.ScopeXLabel();
//            this.txtNote = new ScopeX.UserControls.ScopeXTextBox();
//            this.btnStart = new ScopeX.UserControls.ScopeXIconButton();
//            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
//            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).BeginInit();
//            this.SuspendLayout();

//            // 基础颜色定义
//            Color ctrlBackColor = Color.FromArgb(53, 54, 58);
//            Color foreColor = Color.FromArgb(185, 192, 199);
//            Color labelColor = Color.FromArgb(232, 234, 237);
//            Color highlightColor = Color.FromArgb(40, 71, 193);

//            // 布局参数
//            int labelX = 20;
//            int ctrlX = 100;
//            int startY = 15;
//            int stepY = 32;
//            int currY = startY;

//            // 1. Channel
//            this.lblChannel.AutoSize = true;
//            this.lblChannel.BackColor = System.Drawing.Color.Transparent;
//            this.lblChannel.Font = new System.Drawing.Font("Arial", 9F);
//            this.lblChannel.ForeColor = labelColor;
//            this.lblChannel.Location = new System.Drawing.Point(labelX, currY + 5);
//            this.lblChannel.Name = "lblChannel";
//            this.lblChannel.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XuanZeTongDao");

//            this.cmbChannel.BackColor = ctrlBackColor;
//            this.cmbChannel.BorderColor = ctrlBackColor;
//            this.cmbChannel.BorderThickness = 0;
//            this.cmbChannel.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
//            this.cmbChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
//            this.cmbChannel.ForeColor = foreColor;
//            this.cmbChannel.ItemHeight = 24;
//            this.cmbChannel.Items.AddRange(new object[] { "CH1", "CH2", "CH3", "CH4" });
//            this.cmbChannel.Location = new System.Drawing.Point(ctrlX, currY);
//            this.cmbChannel.Name = "cmbChannel";
//            this.cmbChannel.SelectedBackColor = highlightColor;
//            this.cmbChannel.Size = new System.Drawing.Size(200, 30);
//            this.cmbChannel.TabIndex = 1;
//            currY += stepY;

//            // 2. FileName
//            this.lblFileName.AutoSize = true;
//            this.lblFileName.BackColor = System.Drawing.Color.Transparent;
//            this.lblFileName.Font = new System.Drawing.Font("Arial", 9F);
//            this.lblFileName.ForeColor = labelColor;
//            this.lblFileName.Location = new System.Drawing.Point(labelX, currY + 5);
//            this.lblFileName.Name = "lblFileName";
//            this.lblFileName.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WenJianQianZhui");

//            this.txtFileName.AutoShowKeyBoard = false;
//            this.txtFileName.BackColor = ctrlBackColor;
//            this.txtFileName.BorderColor = ctrlBackColor;
//            this.txtFileName.BorderThickness = 0;
//            this.txtFileName.CornerRadius = 0;
//            this.txtFileName.ForeColor = foreColor;
//            this.txtFileName.Location = new System.Drawing.Point(ctrlX, currY);
//            this.txtFileName.Name = "txtFileName";
//            this.txtFileName.ReadOnly = false;
//            this.txtFileName.Size = new System.Drawing.Size(200, 30);
//            this.txtFileName.TabIndex = 3;
//            this.txtFileName.Text = "WaveData";
//            this.txtFileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;

//            currY += stepY;

//            // 3. Author
//            this.lblAuthor.AutoSize = true;
//            this.lblAuthor.BackColor = System.Drawing.Color.Transparent;
//            this.lblAuthor.Font = new System.Drawing.Font("Arial", 9F);
//            this.lblAuthor.ForeColor = labelColor;
//            this.lblAuthor.Location = new System.Drawing.Point(labelX, currY + 5);
//            this.lblAuthor.Name = "lblAuthor";
//            this.lblAuthor.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CaiJiRenYuan");

//            this.txtAuthor.AutoShowKeyBoard = false;
//            this.txtAuthor.BackColor = ctrlBackColor;
//            this.txtAuthor.BorderColor = ctrlBackColor;
//            this.txtAuthor.BorderThickness = 0;
//            this.txtAuthor.CornerRadius = 0;
//            this.txtAuthor.ForeColor = foreColor;
//            this.txtAuthor.Location = new System.Drawing.Point(ctrlX, currY);
//            this.txtAuthor.Name = "txtAuthor";
//            this.txtAuthor.ReadOnly = false;
//            this.txtAuthor.Size = new System.Drawing.Size(200, 30);
//            this.txtAuthor.TabIndex = 5;
//            this.txtAuthor.Text = "UESTC";
//            this.txtAuthor.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;

//            currY += stepY;

//            // 4. Count
//            this.lblCount.AutoSize = true;
//            this.lblCount.BackColor = System.Drawing.Color.Transparent;
//            this.lblCount.Font = new System.Drawing.Font("Arial", 9F);
//            this.lblCount.ForeColor = labelColor;
//            this.lblCount.Location = new System.Drawing.Point(labelX, currY + 5);
//            this.lblCount.Name = "lblCount";
//            this.lblCount.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CaiJiZhenShu");

//            this.numCount.BackColor = ctrlBackColor;
//            this.numCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
//            this.numCount.ForeColor = foreColor;
//            this.numCount.Location = new System.Drawing.Point(ctrlX, currY + 2);
//            this.numCount.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
//            this.numCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
//            this.numCount.Name = "numCount";
//            this.numCount.Size = new System.Drawing.Size(200, 21);
//            this.numCount.TabIndex = 7;
//            this.numCount.Value = new decimal(new int[] { 10, 0, 0, 0 });

//            currY += stepY;

//            // 5. Interval
//            this.lblInterval.AutoSize = true;
//            this.lblInterval.BackColor = System.Drawing.Color.Transparent;
//            this.lblInterval.Font = new System.Drawing.Font("Arial", 9F);
//            this.lblInterval.ForeColor = labelColor;
//            this.lblInterval.Location = new System.Drawing.Point(labelX, currY + 5);
//            this.lblInterval.Name = "lblInterval";
//            this.lblInterval.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CaiJiJianGe");

//            this.numInterval.BackColor = ctrlBackColor;
//            this.numInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
//            this.numInterval.ForeColor = foreColor;
//            this.numInterval.Location = new System.Drawing.Point(ctrlX, currY + 2);
//            this.numInterval.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
//            this.numInterval.Name = "numInterval";
//            this.numInterval.Size = new System.Drawing.Size(160, 21);
//            this.numInterval.TabIndex = 9;
//            this.numInterval.Value = new decimal(new int[] { 20, 0, 0, 0 });

//            this.lblMs.AutoSize = true;
//            this.lblMs.BackColor = System.Drawing.Color.Transparent;
//            this.lblMs.Font = new System.Drawing.Font("Arial", 9F);
//            this.lblMs.ForeColor = labelColor;
//            this.lblMs.Location = new System.Drawing.Point(ctrlX + 165, currY + 5);
//            this.lblMs.Name = "lblMs";
//            this.lblMs.Text = "ms";

//            currY += stepY;

//            // --- 6. [NEW] File Format ---
//            this.lblFormat.AutoSize = true;
//            this.lblFormat.BackColor = System.Drawing.Color.Transparent;
//            this.lblFormat.Font = new System.Drawing.Font("Arial", 9F);
//            this.lblFormat.ForeColor = labelColor;
//            this.lblFormat.Location = new System.Drawing.Point(labelX, currY + 5);
//            this.lblFormat.Name = "lblFormat";
//            this.lblFormat.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WenJianGeShi"); // "文件格式:"

//            this.cmbFormat.BackColor = ctrlBackColor;
//            this.cmbFormat.BorderColor = ctrlBackColor;
//            this.cmbFormat.BorderThickness = 0;
//            this.cmbFormat.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
//            this.cmbFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
//            this.cmbFormat.ForeColor = foreColor;
//            this.cmbFormat.ItemHeight = 24;
//            // 添加选项：0=Bin+Json, 1=Txt
//            this.cmbFormat.Items.AddRange(new object[] {
//                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GeShiBin"), // "Binary (.bin+.json)"
//                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GeShiTxt")  // "Text (.txt)"
//            });
//            this.cmbFormat.Location = new System.Drawing.Point(ctrlX, currY);
//            this.cmbFormat.Name = "cmbFormat";
//            this.cmbFormat.SelectedBackColor = highlightColor;
//            this.cmbFormat.Size = new System.Drawing.Size(200, 30);
//            this.cmbFormat.TabIndex = 10; // 注意TabIndex顺序

//            currY += stepY;
//            // ----------------------------

//            // 7. Tag
//            this.lblTag.AutoSize = true;
//            this.lblTag.BackColor = System.Drawing.Color.Transparent;
//            this.lblTag.Font = new System.Drawing.Font("Arial", 9F);
//            this.lblTag.ForeColor = labelColor;
//            this.lblTag.Location = new System.Drawing.Point(labelX, currY + 5);
//            this.lblTag.Name = "lblTag";
//            this.lblTag.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YangBenBiaoQian");

//            this.cmbTag.BackColor = ctrlBackColor;
//            this.cmbTag.BorderColor = ctrlBackColor;
//            this.cmbTag.BorderThickness = 0;
//            this.cmbTag.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
//            this.cmbTag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
//            this.cmbTag.ForeColor = foreColor;
//            this.cmbTag.ItemHeight = 24;
//            this.cmbTag.Items.AddRange(new object[] { "Normal", "Anomaly", "Noise", "Signal_A", "Signal_B" });
//            this.cmbTag.Location = new System.Drawing.Point(ctrlX, currY);
//            this.cmbTag.Name = "cmbTag";
//            this.cmbTag.SelectedBackColor = highlightColor;
//            this.cmbTag.Size = new System.Drawing.Size(200, 30);
//            this.cmbTag.TabIndex = 12;
//            this.cmbTag.Text = "Normal";

//            currY += stepY;

//            // 8. Usage
//            this.lblUsage.AutoSize = true;
//            this.lblUsage.BackColor = System.Drawing.Color.Transparent;
//            this.lblUsage.Font = new System.Drawing.Font("Arial", 9F);
//            this.lblUsage.ForeColor = labelColor;
//            this.lblUsage.Location = new System.Drawing.Point(labelX, currY + 5);
//            this.lblUsage.Name = "lblUsage";
//            this.lblUsage.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuJuYongTu");

//            this.cmbUsage.BackColor = ctrlBackColor;
//            this.cmbUsage.BorderColor = ctrlBackColor;
//            this.cmbUsage.BorderThickness = 0;
//            this.cmbUsage.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
//            this.cmbUsage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
//            this.cmbUsage.ForeColor = foreColor;
//            this.cmbUsage.ItemHeight = 24;
//            this.cmbUsage.Items.AddRange(new object[] {
//                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XunLianJi"),
//                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CeShiJi"),
//                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YanZhengJi")
//            });
//            this.cmbUsage.Location = new System.Drawing.Point(ctrlX, currY);
//            this.cmbUsage.Name = "cmbUsage";
//            this.cmbUsage.SelectedBackColor = highlightColor;
//            this.cmbUsage.Size = new System.Drawing.Size(200, 30);
//            this.cmbUsage.TabIndex = 14;

//            currY += stepY;

//            // 9. Note
//            this.lblNote.AutoSize = true;
//            this.lblNote.BackColor = System.Drawing.Color.Transparent;
//            this.lblNote.Font = new System.Drawing.Font("Arial", 9F);
//            this.lblNote.ForeColor = labelColor;
//            this.lblNote.Location = new System.Drawing.Point(labelX, currY + 5);
//            this.lblNote.Name = "lblNote";
//            this.lblNote.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiYanBeiZhu");

//            this.txtNote.AutoShowKeyBoard = false;
//            this.txtNote.BackColor = ctrlBackColor;
//            this.txtNote.BorderColor = ctrlBackColor;
//            this.txtNote.BorderThickness = 0;
//            this.txtNote.CornerRadius = 0;
//            this.txtNote.ForeColor = foreColor;
//            this.txtNote.Location = new System.Drawing.Point(ctrlX, currY);
//            this.txtNote.Multiline = true;
//            this.txtNote.Name = "txtNote";
//            this.txtNote.ReadOnly = false;
//            this.txtNote.Size = new System.Drawing.Size(200, 50);
//            this.txtNote.TabIndex = 16;
//            this.txtNote.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Wu");
//            this.txtNote.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;

//            currY += 65;

//            // Button
//            this.btnStart.BackColor = ctrlBackColor;
//            this.btnStart.BorderColor = ctrlBackColor;
//            this.btnStart.BorderThickness = 1;
//            this.btnStart.CornerRadius = 0;
//            this.btnStart.Cursor = System.Windows.Forms.Cursors.Hand;
//            this.btnStart.ForeColor = foreColor;
//            // 此时坐标约为 337，在 480 的高度内没问题
//            this.btnStart.Location = new System.Drawing.Point(ctrlX, currY);
//            this.btnStart.MouseinBackColor = System.Drawing.Color.FromArgb(70, 70, 75);
//            this.btnStart.Name = "btnStart";
//            this.btnStart.PressedBackColor = highlightColor;
//            this.btnStart.PressedForeColor = System.Drawing.Color.White;
//            this.btnStart.Size = new System.Drawing.Size(200, 38);
//            this.btnStart.TabIndex = 17;
//            this.btnStart.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("KaiShiCaiJiDaoChu");
//            this.btnStart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
//            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);

//            // AiExportPage
//            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
//            this.AutoScroll = true;

//            // 添加控件 (记得添加 Format 相关的)
//            this.Controls.Add(this.btnStart);
//            this.Controls.Add(this.txtNote);
//            this.Controls.Add(this.lblNote);
//            this.Controls.Add(this.cmbUsage);
//            this.Controls.Add(this.lblUsage);
//            this.Controls.Add(this.cmbTag);
//            this.Controls.Add(this.lblTag);
//            this.Controls.Add(this.cmbFormat); // [New]
//            this.Controls.Add(this.lblFormat); // [New]
//            this.Controls.Add(this.lblMs);
//            this.Controls.Add(this.numInterval);
//            this.Controls.Add(this.lblInterval);
//            this.Controls.Add(this.numCount);
//            this.Controls.Add(this.lblCount);
//            this.Controls.Add(this.txtAuthor);
//            this.Controls.Add(this.lblAuthor);
//            this.Controls.Add(this.txtFileName);
//            this.Controls.Add(this.lblFileName);
//            this.Controls.Add(this.cmbChannel);
//            this.Controls.Add(this.lblChannel);

//            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
//            this.ForeColor = foreColor;
//            this.Name = "AiExportPage";
//            this.Size = new System.Drawing.Size(350, 480);
//            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
//            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).EndInit();
//            this.ResumeLayout(false);
//            this.PerformLayout();
//        }

//        #endregion

//        // ... 原有声明 ...
//        private ScopeX.UserControls.ScopeXLabel lblChannel;
//        private ScopeX.UserControls.ComboBoxEx cmbChannel;
//        private ScopeX.UserControls.ScopeXLabel lblFileName;
//        private ScopeX.UserControls.ScopeXTextBox txtFileName;
//        private ScopeX.UserControls.ScopeXLabel lblAuthor;
//        private ScopeX.UserControls.ScopeXTextBox txtAuthor;
//        private ScopeX.UserControls.ScopeXLabel lblCount;
//        private System.Windows.Forms.NumericUpDown numCount;
//        private ScopeX.UserControls.ScopeXLabel lblInterval;
//        private System.Windows.Forms.NumericUpDown numInterval;
//        private ScopeX.UserControls.ScopeXLabel lblMs;

//        // --- 新增声明 ---
//        private ScopeX.UserControls.ScopeXLabel lblFormat;
//        private ScopeX.UserControls.ComboBoxEx cmbFormat;
//        // ----------------

//        private ScopeX.UserControls.ScopeXLabel lblTag;
//        private ScopeX.UserControls.ComboBoxEx cmbTag;
//        private ScopeX.UserControls.ScopeXLabel lblUsage;
//        private ScopeX.UserControls.ComboBoxEx cmbUsage;
//        private ScopeX.UserControls.ScopeXLabel lblNote;
//        private ScopeX.UserControls.ScopeXTextBox txtNote;
//        private ScopeX.UserControls.ScopeXIconButton btnStart;
//    }
//}
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    partial class AiExportPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        private void InitializeComponent()
        {
            this.lblChannel = new ScopeX.UserControls.ScopeXLabel();
            this.cmbChannel = new ScopeX.UserControls.ComboBoxEx();
            this.lblFileName = new ScopeX.UserControls.ScopeXLabel();
            this.txtFileName = new ScopeX.UserControls.ScopeXTextBox();
            this.lblAuthor = new ScopeX.UserControls.ScopeXLabel();
            this.txtAuthor = new ScopeX.UserControls.ScopeXTextBox();
            this.lblCount = new ScopeX.UserControls.ScopeXLabel();
            this.numCount = new System.Windows.Forms.NumericUpDown();
            this.lblInterval = new ScopeX.UserControls.ScopeXLabel();
            this.numInterval = new System.Windows.Forms.NumericUpDown();
            this.lblMs = new ScopeX.UserControls.ScopeXLabel();
            this.lblFormat = new ScopeX.UserControls.ScopeXLabel();
            this.cmbFormat = new ScopeX.UserControls.ComboBoxEx();
            this.lblTag = new ScopeX.UserControls.ScopeXLabel();
            this.cmbTag = new ScopeX.UserControls.ComboBoxEx();
            this.lblUsage = new ScopeX.UserControls.ScopeXLabel();
            this.cmbUsage = new ScopeX.UserControls.ComboBoxEx();
            this.lblNote = new ScopeX.UserControls.ScopeXLabel();
            this.txtNote = new ScopeX.UserControls.ScopeXTextBox();
            this.btnStart = new ScopeX.UserControls.ScopeXIconButton();
            this.btnaddtype = new ScopeX.UserControls.ScopeXIconButton();
            this.cmbtype = new ScopeX.UserControls.ComboBoxEx();
            this.btndeletetype = new ScopeX.UserControls.ScopeXIconButton();
            this.lbltype = new ScopeX.UserControls.ScopeXLabel();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // lblChannel
            // 
            this.lblChannel.AutoSize = true;
            this.lblChannel.BackColor = System.Drawing.Color.Transparent;
            this.lblChannel.Font = new System.Drawing.Font("Arial", 9F);
            this.lblChannel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.lblChannel.Location = new System.Drawing.Point(20, 20);
            this.lblChannel.Name = "lblChannel";
            this.lblChannel.Size = new System.Drawing.Size(60, 15);
            this.lblChannel.TabIndex = 0;
            this.lblChannel.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XuanZeTongDao");
            // 
            // cmbChannel
            // 
            this.cmbChannel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cmbChannel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cmbChannel.BorderThickness = 0;
            this.cmbChannel.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChannel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.cmbChannel.ItemHeight = 24;
            this.cmbChannel.Items.AddRange(new object[] {
            "CH1",
            "CH2",
            "CH3",
            "CH4"});
            this.cmbChannel.Location = new System.Drawing.Point(100, 15);
            this.cmbChannel.Name = "cmbChannel";
            this.cmbChannel.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.cmbChannel.Size = new System.Drawing.Size(200, 30);
            this.cmbChannel.TabIndex = 1;
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.BackColor = System.Drawing.Color.Transparent;
            this.lblFileName.Font = new System.Drawing.Font("Arial", 9F);
            this.lblFileName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.lblFileName.Location = new System.Drawing.Point(20, 52);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(60, 15);
            this.lblFileName.TabIndex = 2;
            this.lblFileName.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WenJianQianZhui");
            // 
            // txtFileName
            // 
            this.txtFileName.AutoShowKeyBoard = false;
            this.txtFileName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.txtFileName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.txtFileName.BorderThickness = 0;
            this.txtFileName.CornerRadius = 0;
            this.txtFileName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.txtFileName.Location = new System.Drawing.Point(100, 47);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.ReadOnly = false;
            this.txtFileName.Size = new System.Drawing.Size(200, 30);
            this.txtFileName.TabIndex = 3;
            this.txtFileName.Text = "WaveData";
            this.txtFileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.BackColor = System.Drawing.Color.Transparent;
            this.lblAuthor.Font = new System.Drawing.Font("Arial", 9F);
            this.lblAuthor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.lblAuthor.Location = new System.Drawing.Point(20, 84);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(60, 15);
            this.lblAuthor.TabIndex = 4;
            this.lblAuthor.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CaiJiRenYuan");
            // 
            // txtAuthor
            // 
            this.txtAuthor.AutoShowKeyBoard = false;
            this.txtAuthor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.txtAuthor.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.txtAuthor.BorderThickness = 0;
            this.txtAuthor.CornerRadius = 0;
            this.txtAuthor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.txtAuthor.Location = new System.Drawing.Point(100, 79);
            this.txtAuthor.Name = "txtAuthor";
            this.txtAuthor.ReadOnly = false;
            this.txtAuthor.Size = new System.Drawing.Size(200, 30);
            this.txtAuthor.TabIndex = 5;
            this.txtAuthor.Text = "UESTC";
            this.txtAuthor.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.BackColor = System.Drawing.Color.Transparent;
            this.lblCount.Font = new System.Drawing.Font("Arial", 9F);
            this.lblCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.lblCount.Location = new System.Drawing.Point(20, 116);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(60, 15);
            this.lblCount.TabIndex = 6;
            this.lblCount.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CaiJiZhenShu");
            // 
            // numCount
            // 
            this.numCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.numCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.numCount.Location = new System.Drawing.Point(100, 113);
            this.numCount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCount.Name = "numCount";
            this.numCount.Size = new System.Drawing.Size(200, 21);
            this.numCount.TabIndex = 7;
            this.numCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lblInterval
            // 
            this.lblInterval.AutoSize = true;
            this.lblInterval.BackColor = System.Drawing.Color.Transparent;
            this.lblInterval.Font = new System.Drawing.Font("Arial", 9F);
            this.lblInterval.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.lblInterval.Location = new System.Drawing.Point(20, 148);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(60, 15);
            this.lblInterval.TabIndex = 8;
            this.lblInterval.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CaiJiJianGe");
            // 
            // numInterval
            // 
            this.numInterval.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.numInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numInterval.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.numInterval.Location = new System.Drawing.Point(100, 145);
            this.numInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numInterval.Name = "numInterval";
            this.numInterval.Size = new System.Drawing.Size(160, 21);
            this.numInterval.TabIndex = 9;
            this.numInterval.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // lblMs
            // 
            this.lblMs.AutoSize = true;
            this.lblMs.BackColor = System.Drawing.Color.Transparent;
            this.lblMs.Font = new System.Drawing.Font("Arial", 9F);
            this.lblMs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.lblMs.Location = new System.Drawing.Point(265, 148);
            this.lblMs.Name = "lblMs";
            this.lblMs.Size = new System.Drawing.Size(24, 15);
            this.lblMs.TabIndex = 10;
            this.lblMs.Text = "ms";
            // 
            // lblFormat
            // 
            this.lblFormat.AutoSize = true;
            this.lblFormat.BackColor = System.Drawing.Color.Transparent;
            this.lblFormat.Font = new System.Drawing.Font("Arial", 9F);
            this.lblFormat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.lblFormat.Location = new System.Drawing.Point(20, 180);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(60, 15);
            this.lblFormat.TabIndex = 11;
            this.lblFormat.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WenJianGeShi");
            // 
            // cmbFormat
            // 
            this.cmbFormat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cmbFormat.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cmbFormat.BorderThickness = 0;
            this.cmbFormat.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFormat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.cmbFormat.ItemHeight = 24;
            this.cmbFormat.Items.AddRange(new object[] {
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GeShiBin"),
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GeShiTxt")});
            this.cmbFormat.Location = new System.Drawing.Point(100, 177);
            this.cmbFormat.Name = "cmbFormat";
            this.cmbFormat.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.cmbFormat.Size = new System.Drawing.Size(200, 30);
            this.cmbFormat.TabIndex = 12;
            // 
            // lblTag
            // 
            this.lblTag.AutoSize = true;
            this.lblTag.BackColor = System.Drawing.Color.Transparent;
            this.lblTag.Font = new System.Drawing.Font("Arial", 9F);
            this.lblTag.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.lblTag.Location = new System.Drawing.Point(20, 212);
            this.lblTag.Name = "lblTag";
            this.lblTag.Size = new System.Drawing.Size(60, 15);
            this.lblTag.TabIndex = 13;
            this.lblTag.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YangBenBiaoQian");
            // 
            // cmbTag
            // 
            this.cmbTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cmbTag.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cmbTag.BorderThickness = 0;
            this.cmbTag.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbTag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTag.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.cmbTag.ItemHeight = 24;
            this.cmbTag.Items.AddRange(new object[] {
            "Normal",
            "Anomaly",
            "Noise",
            "Signal_A",
            "Signal_B"});
            this.cmbTag.Location = new System.Drawing.Point(100, 209);
            this.cmbTag.Name = "cmbTag";
            this.cmbTag.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.cmbTag.Size = new System.Drawing.Size(200, 30);
            this.cmbTag.TabIndex = 14;
            this.cmbTag.Text = "Normal";
            // 
            // lblUsage
            // 
            this.lblUsage.AutoSize = true;
            this.lblUsage.BackColor = System.Drawing.Color.Transparent;
            this.lblUsage.Font = new System.Drawing.Font("Arial", 9F);
            this.lblUsage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.lblUsage.Location = new System.Drawing.Point(20, 244);
            this.lblUsage.Name = "lblUsage";
            this.lblUsage.Size = new System.Drawing.Size(60, 15);
            this.lblUsage.TabIndex = 15;
            this.lblUsage.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuJuYongTu");
            // 
            // cmbUsage
            // 
            this.cmbUsage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cmbUsage.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cmbUsage.BorderThickness = 0;
            this.cmbUsage.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbUsage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUsage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.cmbUsage.ItemHeight = 24;
            this.cmbUsage.Items.AddRange(new object[] {
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XunLianJi"),
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CeShiJi"),
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YanZhengJi")});
            this.cmbUsage.Location = new System.Drawing.Point(100, 241);
            this.cmbUsage.Name = "cmbUsage";
            this.cmbUsage.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.cmbUsage.Size = new System.Drawing.Size(200, 30);
            this.cmbUsage.TabIndex = 16;
            // 
            // lblNote
            // 
            this.lblNote.AutoSize = true;
            this.lblNote.BackColor = System.Drawing.Color.Transparent;
            this.lblNote.Font = new System.Drawing.Font("Arial", 9F);
            this.lblNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.lblNote.Location = new System.Drawing.Point(20, 276);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(60, 15);
            this.lblNote.TabIndex = 17;
            this.lblNote.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiYanBeiZhu");
            // 
            // txtNote
            // 
            this.txtNote.AutoShowKeyBoard = false;
            this.txtNote.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.txtNote.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.txtNote.BorderThickness = 0;
            this.txtNote.CornerRadius = 0;
            this.txtNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.txtNote.Location = new System.Drawing.Point(100, 273);
            this.txtNote.Multiline = true;
            this.txtNote.Name = "txtNote";
            this.txtNote.ReadOnly = false;
            this.txtNote.Size = new System.Drawing.Size(200, 30);
            this.txtNote.TabIndex = 18;
            this.txtNote.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Wu");
            this.txtNote.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.btnStart.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.btnStart.BorderThickness = 1;
            this.btnStart.CornerRadius = 0;
            this.btnStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.btnStart.Location = new System.Drawing.Point(100, 305);
            this.btnStart.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(75)))));
            this.btnStart.Name = "btnStart";
            this.btnStart.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.btnStart.PressedForeColor = System.Drawing.Color.White;
            this.btnStart.Size = new System.Drawing.Size(100, 30);
            this.btnStart.TabIndex = 19;
            this.btnStart.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("KaiShiCaiJiDaoChu");
            this.btnStart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnaddtype
            // 
            this.btnaddtype.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.btnaddtype.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.btnaddtype.BorderThickness = 1;
            this.btnaddtype.CornerRadius = 0;
            this.btnaddtype.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnaddtype.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.btnaddtype.Location = new System.Drawing.Point(210, 305);
            this.btnaddtype.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(75)))));
            this.btnaddtype.Name = "btnaddtype";
            this.btnaddtype.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.btnaddtype.PressedForeColor = System.Drawing.Color.White;
            this.btnaddtype.Size = new System.Drawing.Size(80, 30);
            this.btnaddtype.TabIndex = 20;
            this.btnaddtype.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("RuKu");
            this.btnaddtype.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnaddtype.Click += new System.EventHandler(this.btnaddtype_Click);
            // 
            // cmbtype
            // 
            this.cmbtype.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cmbtype.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.cmbtype.BorderThickness = 0;
            this.cmbtype.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbtype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbtype.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.cmbtype.ItemHeight = 24;
            this.cmbtype.Items.AddRange(new object[] {
            "Normal",
            "Anomaly",
            "Noise",
            "Signal_A",
            "Signal_B"});
            this.cmbtype.Location = new System.Drawing.Point(100, 337);
            this.cmbtype.Name = "cmbTag";
            this.cmbtype.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.cmbtype.Size = new System.Drawing.Size(200, 30);
            this.cmbtype.TabIndex = 21;
            this.cmbtype.Text = "Normal";
            // 
            // lbltype
            // 
            this.lbltype.AutoSize = true;
            this.lbltype.BackColor = System.Drawing.Color.Transparent;
            this.lbltype.Font = new System.Drawing.Font("Arial", 9F);
            this.lbltype.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(234)))), ((int)(((byte)(237)))));
            this.lbltype.Location = new System.Drawing.Point(20, 340);
            this.lbltype.Name = "lblNote";
            this.lbltype.Size = new System.Drawing.Size(60, 15);
            this.lbltype.TabIndex = 22;
            this.lbltype.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XinHaoLeiXing");
            // 
            // btndeletetype
            // 
            this.btndeletetype.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.btndeletetype.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.btndeletetype.BorderThickness = 1;
            this.btndeletetype.CornerRadius = 0;
            this.btndeletetype.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btndeletetype.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.btndeletetype.Location = new System.Drawing.Point(100, 369);
            this.btndeletetype.MouseinBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(75)))));
            this.btndeletetype.Name = "btndeletetype";
            this.btndeletetype.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(71)))), ((int)(((byte)(193)))));
            this.btndeletetype.PressedForeColor = System.Drawing.Color.White;
            this.btndeletetype.Size = new System.Drawing.Size(100, 30);
            this.btndeletetype.TabIndex = 23;
            this.btndeletetype.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShanChu");
            this.btndeletetype.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btndeletetype.Click += new System.EventHandler(this.btndeletetype_Click);
            // 
            // AiExportPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.AutoScroll = true;
            this.Controls.Add(this.lbltype);
            this.Controls.Add(this.btndeletetype);
            this.Controls.Add(this.cmbtype);
            this.Controls.Add(this.btnaddtype);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtNote);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.cmbUsage);
            this.Controls.Add(this.lblUsage);
            this.Controls.Add(this.cmbTag);
            this.Controls.Add(this.lblTag);
            this.Controls.Add(this.cmbFormat);
            this.Controls.Add(this.lblFormat);
            this.Controls.Add(this.lblMs);
            this.Controls.Add(this.numInterval);
            this.Controls.Add(this.lblInterval);
            this.Controls.Add(this.numCount);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.txtAuthor);
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.cmbChannel);
            this.Controls.Add(this.lblChannel);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.Name = "AiExportPage";
            this.Size = new System.Drawing.Size(350, 450);
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private ScopeX.UserControls.ScopeXLabel lblChannel;
        private ScopeX.UserControls.ComboBoxEx cmbChannel;
        private ScopeX.UserControls.ScopeXLabel lblFileName;
        private ScopeX.UserControls.ScopeXTextBox txtFileName;
        private ScopeX.UserControls.ScopeXLabel lblAuthor;
        private ScopeX.UserControls.ScopeXTextBox txtAuthor;
        private ScopeX.UserControls.ScopeXLabel lblCount;
        private System.Windows.Forms.NumericUpDown numCount;
        private ScopeX.UserControls.ScopeXLabel lblInterval;
        private System.Windows.Forms.NumericUpDown numInterval;
        private ScopeX.UserControls.ScopeXLabel lblMs;
        private ScopeX.UserControls.ScopeXLabel lblFormat;
        private ScopeX.UserControls.ComboBoxEx cmbFormat;
        private ScopeX.UserControls.ScopeXLabel lblTag;
        private ScopeX.UserControls.ComboBoxEx cmbTag;
        private ScopeX.UserControls.ScopeXLabel lblUsage;
        private ScopeX.UserControls.ComboBoxEx cmbUsage;
        private ScopeX.UserControls.ScopeXLabel lblNote;
        private ScopeX.UserControls.ScopeXTextBox txtNote;
        private ScopeX.UserControls.ScopeXIconButton btnStart;
        private ScopeX.UserControls.ScopeXIconButton btnaddtype;
        private ScopeX.UserControls.ComboBoxEx cmbtype;
        private ScopeX.UserControls.ScopeXIconButton btndeletetype;
        private ScopeX.UserControls.ScopeXLabel lbltype;
    }
}