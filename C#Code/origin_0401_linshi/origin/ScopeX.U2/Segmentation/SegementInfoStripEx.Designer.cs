namespace ScopeX.U2
{
    partial class SegementInfoStripEx
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
            TlpFragment = new System.Windows.Forms.TableLayoutPanel();
            UpcCollectState = new ScopeXProcessEllipseEx();
            TlpFragment.SuspendLayout();
            SuspendLayout();
            // 
            // TlpFragment
            // 
            TlpFragment.BackColor = System.Drawing.SystemColors.WindowFrame;
            TlpFragment.ColumnCount = 1;
            TlpFragment.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpFragment.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TlpFragment.Controls.Add(UpcCollectState, 0, 0);
            TlpFragment.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpFragment.Location = new System.Drawing.Point(0, 0);
            TlpFragment.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            TlpFragment.Name = "TlpFragment";
            TlpFragment.RowCount = 1;
            TlpFragment.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TlpFragment.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TlpFragment.Size = new System.Drawing.Size(259, 100);
            TlpFragment.TabIndex = 1;
            // 
            // UpcCollectState
            // 
            UpcCollectState.BackEllipseColor = System.Drawing.Color.FromArgb(80, 85, 95);
            UpcCollectState.EllipseBackColor = System.Drawing.Color.FromArgb(41, 43, 50);
            UpcCollectState.FirstColumnWidth = 70;
            UpcCollectState.Font = new System.Drawing.Font("MiSans", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            UpcCollectState.ForeColor = System.Drawing.Color.Black;
            UpcCollectState.HeaderBackColor = System.Drawing.Color.FromArgb(61, 62, 69);
            UpcCollectState.HeaderForeColor = System.Drawing.Color.White;
            UpcCollectState.HeaderHeight = 30;
            UpcCollectState.InnerEllipseColor = System.Drawing.Color.Silver;
            UpcCollectState.IsShowInnerEllipseBorder = false;
            UpcCollectState.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Ignore;
            UpcCollectState.Location = new System.Drawing.Point(3, 3);
            UpcCollectState.MaxValue = 100;
            UpcCollectState.Name = "UpcCollectState";
            UpcCollectState.ShowType = UserControls.ShowType.Ring;
            UpcCollectState.Size = new System.Drawing.Size(253, 94);
            UpcCollectState.StyleFlags = UserControls.Style.StyleFlag.None;
            UpcCollectState.StylizeFlag = true;
            UpcCollectState.TabIndex = 0;
            UpcCollectState.Text = "顺序模式";
            UpcCollectState.Value = 25;
            UpcCollectState.ValueColor = System.Drawing.Color.FromArgb(0, 171, 209);
            UpcCollectState.ValueInfoBackColor = System.Drawing.Color.FromArgb(41, 43, 50);
            UpcCollectState.ValueInfoColor = System.Drawing.Color.White;
            UpcCollectState.ValueMargin = 0;
            UpcCollectState.ValueType = UserControls.ProcessBarValueType.Percent;
            UpcCollectState.ValueWidth = 12;
            UpcCollectState.Click += UpcCollectState_Click;
            UpcCollectState.MouseDown += UpcCollectState_MouseDown;
            UpcCollectState.MouseMove += UpcCollectState_MouseMove;
            UpcCollectState.MouseUp += UpcCollectState_MouseUp;
            // 
            // SegementInfoStripEx
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            Controls.Add(TlpFragment);
            Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            Name = "SegementInfoStripEx";
            Size = new System.Drawing.Size(259, 100);
            TlpFragment.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpFragment;
        private ScopeXProcessEllipseEx UpcCollectState;
    }
}
