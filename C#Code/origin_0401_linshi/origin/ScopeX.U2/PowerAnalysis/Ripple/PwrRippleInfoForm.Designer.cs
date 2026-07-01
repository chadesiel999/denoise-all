
namespace ScopeX.U2
{
    partial class PwrRippleInfoForm
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
            LvRipple = new UserControls.ScopeXListViewEx();
            Parameter = new System.Windows.Forms.ColumnHeader();
            Value = new System.Windows.Forms.ColumnHeader();
            Mean = new System.Windows.Forms.ColumnHeader();
            Max = new System.Windows.Forms.ColumnHeader();
            Min = new System.Windows.Forms.ColumnHeader();
            ScRipple = new UserControls.ScopeXScrollContainer();
            TmUpdate = new System.Timers.Timer();
            ScRipple.SuspendLayout();
            SuspendLayout();
            // 
            // LvRipple
            // 
            LvRipple.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LvRipple.BorderStyle = System.Windows.Forms.BorderStyle.None;
            LvRipple.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { Parameter, Value, Mean, Max, Min });
            LvRipple.Dock = System.Windows.Forms.DockStyle.Fill;
            LvRipple.Font = new System.Drawing.Font("MiSans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LvRipple.ForeColor = System.Drawing.SystemColors.ControlLight;
            LvRipple.FullRowSelect = true;
            LvRipple.GridLines = true;
            LvRipple.GridLinesColor = System.Drawing.Color.Gray;
            LvRipple.HeaderBackColor = System.Drawing.Color.Gray;
            LvRipple.HeaderForeColor = System.Drawing.Color.White;
            LvRipple.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            LvRipple.HideSelection = true;
            LvRipple.IsIndependentWindow = false;
            LvRipple.Location = new System.Drawing.Point(0, 0);
            LvRipple.MultiSelect = false;
            LvRipple.Name = "LvRipple";
            LvRipple.OwnerDraw = true;
            LvRipple.RowHeight = 23;
            LvRipple.ScrollContainer = ScRipple;
            LvRipple.SelectedRowColor = System.Drawing.Color.Blue;
            LvRipple.Size = new System.Drawing.Size(638, 158);
            LvRipple.StyleFlags = UserControls.Style.StyleFlag.FontSize;
            LvRipple.StylizeFlag = true;
            LvRipple.TabIndex = 0;
            LvRipple.TabStop = false;
            LvRipple.Tag = "Ripple";
            LvRipple.UseCompatibleStateImageBehavior = false;
            LvRipple.View = System.Windows.Forms.View.Details;
            // 
            // Parameter
            // 
            Parameter.Text = "";
            Parameter.Width = 150;
            // 
            // Value
            // 
            Value.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Zhi");
            Value.Width = 120;
            // 
            // Mean
            // 
            Mean.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PingJunZhi");
            Mean.Width = 120;
            // 
            // Max
            // 
            Max.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZuiDaZhi");
            Max.Width = 120;
            // 
            // Min
            // 
            Min.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZuiXiaoZhi");
            Min.Width = 120;
            // 
            // ScRipple
            // 
            ScRipple.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ScRipple.Controls.Add(LvRipple);
            ScRipple.Dock = System.Windows.Forms.DockStyle.Fill;
            ScRipple.Location = new System.Drawing.Point(1, 46);
            ScRipple.Name = "ScRipple";
            ScRipple.ScrollControl = LvRipple;
            ScRipple.ScrollThickness = 6;
            ScRipple.Size = new System.Drawing.Size(638, 158);
            ScRipple.TabIndex = 5;
            // 
            // TmUpdate
            // 
            TmUpdate.Enabled = true;
            TmUpdate.Interval = 500;
            TmUpdate.Elapsed += TmUpdate_Tick;
            // 
            // PwrRippleInfoForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CanClose = false;
            ClientSize = new System.Drawing.Size(640, 205);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(ScRipple);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HeadHeight = 45;
            IconWidth = 26;
            IsShowPin = false;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PwrRippleInfoForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WenBoFenXi");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WenBoFenXi");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleIcon = Properties.Resources.MeasureSetting;
            TitleIconSize = new System.Drawing.Size(26, 26);
            TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            TitleIconWidth = 26;
            TitleLableHeight = 39;
            ToolIconSize = new System.Drawing.Size(26, 26);
            HelpClick += PwrRippleInfoForm_EmbededClick;
            TitleIconClick += PwrRippleInfoForm_SettingClick;
            Controls.SetChildIndex(ScRipple, 0);
            ScRipple.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ScopeX.UserControls.ScopeXListViewEx LvRipple;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.ColumnHeader Mean;
        private System.Windows.Forms.ColumnHeader Max;
        private System.Windows.Forms.ColumnHeader Min;
        private System.Timers.Timer TmUpdate;
        private ScopeX.UserControls.ScopeXScrollContainer ScRipple;
    }
}
