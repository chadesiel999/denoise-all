
namespace ScopeX.U2.PassFail
{
    partial class PassFailInfoForm
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
            LvPassFail = new ListViewNF();
            Parameter = new System.Windows.Forms.ColumnHeader();
            Value = new System.Windows.Forms.ColumnHeader();
            TmUpdate = new System.Timers.Timer();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // LvPassFail
            // 
            LvPassFail.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            LvPassFail.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LvPassFail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            LvPassFail.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { Parameter, Value });
            LvPassFail.ForeColor = System.Drawing.SystemColors.ControlLight;
            LvPassFail.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            LvPassFail.HideSelection = true;
            LvPassFail.Location = new System.Drawing.Point(0, 0);
            LvPassFail.Margin = new System.Windows.Forms.Padding(0);
            LvPassFail.MultiSelect = false;
            LvPassFail.Name = "LvPassFail";
            LvPassFail.Scrollable = false;
            LvPassFail.Size = new System.Drawing.Size(400, 421);
            LvPassFail.TabIndex = 0;
            LvPassFail.TabStop = false;
            LvPassFail.UseCompatibleStateImageBehavior = false;
            LvPassFail.View = System.Windows.Forms.View.Details;
            // 
            // Parameter
            // 
            Parameter.Text = "Parameter";
            Parameter.Width = 200;
            // 
            // Value
            // 
            Value.Text = "Value";
            Value.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            Value.Width = 150;
            // 
            // TmUpdate
            // 
            TmUpdate.Enabled = false;
            TmUpdate.Interval = 1000;
            TmUpdate.Elapsed += TmUpdate_Tick;
            
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(LvPassFail, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(2, 45);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            tableLayoutPanel1.Size = new System.Drawing.Size(400, 471);
            tableLayoutPanel1.TabIndex = 12;
            // 
            // PassFailInfoForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            CanClose = false;
            ClientSize = new System.Drawing.Size(420, 518);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(tableLayoutPanel1);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HelpLabel = "22";
            IconWidth = 26;
            IsShowPin = false;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PassFailInfoForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleIcon = Properties.Resources.MeasureSetting;
            TitleIconSize = new System.Drawing.Size(26, 26);
            TitleIconWidth = 26;
            ToolIconSize = new System.Drawing.Size(26, 26);
            HelpClick += PassFailInfoForm_EmbededClick;
            TitleIconClick += PassFailInfoForm_SettingClick;
            Controls.SetChildIndex(tableLayoutPanel1, 0);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ListViewNF LvPassFail;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Timers.Timer TmUpdate;
        private System.Windows.Forms.ImageList ImageListPad;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
