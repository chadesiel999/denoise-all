using ScopeX.UserControls;
using System.Windows.Forms;

namespace ScopeX.U2
{
    partial class ArticialIntelligenceSetInfoStrip
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
            components = new System.ComponentModel.Container();
            TimerMain = new Timer(components);
            columnHeader1 = new ColumnHeader();
            LblTitle = new Label();
            LblClose = new Label();
            LveMain = new ScopeXListViewEx();
            SuspendLayout();
            // 
            // TimerMain
            // 
            TimerMain.Tick += TimerMain_Tick;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "提示信息";
            columnHeader1.Width = 220;
            // 
            // LblTitle
            // 
            LblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            LblTitle.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblTitle.CausesValidation = false;
            LblTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            LblTitle.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            LblTitle.Location = new System.Drawing.Point(0, 0);
            LblTitle.Margin = new Padding(0);
            LblTitle.Name = "LblTitle";
            LblTitle.Padding = new Padding(8, 0, 0, 0);
            LblTitle.Size = new System.Drawing.Size(198, 36);
            LblTitle.TabIndex = 35;
            LblTitle.Text = "AI 智能设置";
            LblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblClose
            // 
            LblClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            LblClose.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LblClose.Cursor = Cursors.Hand;
            LblClose.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            LblClose.ForeColor = System.Drawing.Color.FromArgb(180, 180, 180);
            LblClose.Location = new System.Drawing.Point(198, 0);
            LblClose.Margin = new Padding(0);
            LblClose.Name = "LblClose";
            LblClose.Size = new System.Drawing.Size(36, 36);
            LblClose.TabIndex = 36;
            LblClose.Text = "✕";
            LblClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            LblClose.Click += LblClose_Click;
            // 
            // LveMain
            // 
            LveMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LveMain.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LveMain.BorderStyle = BorderStyle.None;
            LveMain.Columns.AddRange(new ColumnHeader[] { columnHeader1 });
            LveMain.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LveMain.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            LveMain.FullRowSelect = true;
            LveMain.GridLines = false;
            LveMain.GridLinesColor = System.Drawing.Color.FromArgb(60, 60, 60);
            LveMain.HeaderBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LveMain.HeaderForeColor = System.Drawing.Color.FromArgb(180, 180, 180);
            LveMain.HeaderStyle = ColumnHeaderStyle.None;
            LveMain.IsIndependentWindow = false;
            LveMain.Location = new System.Drawing.Point(7, 40);
            LveMain.Margin = new Padding(0);
            LveMain.MultiSelect = false;
            LveMain.Name = "LveMain";
            LveMain.OwnerDraw = true;
            LveMain.RowHeight = 24;
            LveMain.Scrollable = true;
            LveMain.ScrollContainer = null;
            LveMain.SelectedRowColor = System.Drawing.Color.FromArgb(0, 120, 215);
            LveMain.Size = new System.Drawing.Size(220, 100);
            LveMain.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            LveMain.StylizeFlag = true;
            LveMain.TabIndex = 7;
            LveMain.TabStop = false;
            LveMain.Tag = "PowerQuality";
            LveMain.UseCompatibleStateImageBehavior = false;
            LveMain.View = View.Details;
            // 
            // ArticialIntelligenceSetInfoStrip
            //   
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(LblClose);
            Controls.Add(LblTitle);
            Controls.Add(LveMain);
            ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            MinimumSize = new System.Drawing.Size(234, 80);
            Name = "ArticialIntelligenceSetInfoStrip";
            Size = new System.Drawing.Size(234, 140);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXListViewEx LveMain;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Timer TimerMain;
        private System.Windows.Forms.Label LblTitle;
        private System.Windows.Forms.Label LblClose;
    }
}
