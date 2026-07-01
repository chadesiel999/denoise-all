
using ScopeX.U2.BaseControl;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2.Search
{
    partial class SearchInfoForm
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
            TlpSnapshot = new System.Windows.Forms.Panel();
            LvSearch = new ScopeXListViewEx();
            Index = new System.Windows.Forms.ColumnHeader();
            Type = new System.Windows.Forms.ColumnHeader();
            Position = new System.Windows.Forms.ColumnHeader();
            Delta = new System.Windows.Forms.ColumnHeader();
            Description = new System.Windows.Forms.ColumnHeader();
            ImageListPad = new System.Windows.Forms.ImageList(components);
            TmUpdate = new System.Timers.Timer();
            TlpSnapshot.SuspendLayout();
            SuspendLayout();
            // 
            // TlpSnapshot
            // 
            TlpSnapshot.Controls.Add(LvSearch);
            TlpSnapshot.Controls.Add(PageControl);
            TlpSnapshot.Dock = System.Windows.Forms.DockStyle.Fill;
            TlpSnapshot.Location = new System.Drawing.Point(2, 45);
            TlpSnapshot.Margin = new System.Windows.Forms.Padding(0);
            TlpSnapshot.Name = "TlpSnapshot";
            TlpSnapshot.Size = new System.Drawing.Size(615, 290);
            TlpSnapshot.TabIndex = 6;
            // 
            // LvSearch
            // 
            LvSearch.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LvSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            LvSearch.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { Index, Type, Position, Delta, Description });
            LvSearch.Dock = System.Windows.Forms.DockStyle.Top;
            LvSearch.ForeColor = System.Drawing.SystemColors.ControlLight;
            LvSearch.FullRowSelect = true;
            LvSearch.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            LvSearch.MultiSelect = false;
            LvSearch.Name = "LvSearch";
            LvSearch.Scrollable = false;
            LvSearch.Size = new System.Drawing.Size(615, 290);
            LvSearch.SmallImageList = ImageListPad;
            LvSearch.TabIndex = 0;
            LvSearch.TabStop = false;
            LvSearch.UseCompatibleStateImageBehavior = false;
            LvSearch.View = System.Windows.Forms.View.Details;
            LvSearch.SelectedIndexChanged += LvSearch_SelectedIndexChanged;
            // 
            // Index
            // 
            Index.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BianHao");
            Index.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            Index.Width = 75;
            // 
            // Type
            // 
            Type.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LeiXing");
            Type.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            Type.Width = 100;
            // 
            // Position
            // 
            Position.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WeiZhi");
            Position.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            Position.Width = 120;
            // 
            // Delta
            // 
            Delta.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChaZhi");
            Delta.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            Delta.Width = 120;
            // 
            // Description
            // 
            Description.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MiaoShu");
            Description.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            Description.Width = 200;
            //
            //PageControl
            //
            PageControl.Size = new System.Drawing.Size(520, 30);
            PageControl.Location = new System.Drawing.Point(0, 321);
            // 
            // ImageListPad
            // 
            ImageListPad.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            ImageListPad.ImageSize = new System.Drawing.Size(1, 30);
            ImageListPad.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // TmUpdate
            // 
            TmUpdate.Enabled = true;
            TmUpdate.Interval = 500;
            TmUpdate.Elapsed += TmUpdate_Tick;
            // 
            // SearchInfoForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            CanClose = false;
            ClientSize = new System.Drawing.Size(620, 410);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(TlpSnapshot);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            HeadHeight = 45;
            IconInterval = 21;
            IconWidth = 28;
            IsShowClose = true;
            IsShowPin = false;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SearchInfoForm";
            StyleFlags = StyleFlag.FontSize;
            StylizeFlag = true;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SouSuoShiJianLieBiao");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SouSuoShiJianLieBiao");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleLableHeight = 39;
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(28, 26);
            HelpClick += MeasSnapShotForm_LeftIconClick;
            ToolClick += MeasSnapShotForm_ToolIconClick;
            Controls.SetChildIndex(TlpSnapshot, 0);
            ResumeLayout(false);
        }


        #endregion


        private System.Windows.Forms.Panel TlpSnapshot;
        private ScopeXListViewEx LvSearch;
        private System.Windows.Forms.ColumnHeader Index;
        private System.Windows.Forms.ColumnHeader Type;
        private System.Timers.Timer TmUpdate;
        private System.Windows.Forms.ImageList ImageListPad;
        private System.Windows.Forms.ColumnHeader Position;
        private System.Windows.Forms.ColumnHeader Delta;
        private System.Windows.Forms.ColumnHeader Description;
        private ScopeX.U2.BaseControl.PageControl PageControl;
    }
}
