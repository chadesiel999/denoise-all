
namespace ScopeX.U2
{
    partial class PwrHarmonicInfoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            ImageListPad = new System.Windows.Forms.ImageList(components);
            LvHarmonicTable = new ListViewEx();
            TmUpdate = new System.Timers.Timer();
            TlpHarmonic = new System.Windows.Forms.TableLayoutPanel();
            LvDistortion = new ListViewEx();
            TlpHarmonic.SuspendLayout();
            SuspendLayout();
            // 
            // ImageListPad
            // 
            ImageListPad.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            ImageListPad.ImageSize = new System.Drawing.Size(1, 24);
            ImageListPad.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // LvHarmonicTable
            // 
            LvHarmonicTable.Columns.AddRange(new ListViewEx.ColumnInfo[]
            {
                new ListViewEx.ColumnInfo("",50),
                new ListViewEx.ColumnInfo(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PinLv_Hz_"),150),
                new ListViewEx.ColumnInfo(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LiangZhi___"),150),
                new ListViewEx.ColumnInfo(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LiangZhiJunFangGen"),150)
                {
                    Tag = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LiangZhiJunFangGen"),
                },
                new ListViewEx.ColumnInfo(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiangWei_°_"),150),
            });
            LvHarmonicTable.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;
            LvHarmonicTable.TextAlignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            LvHarmonicTable.BackColor = new System.Drawing.Color[] { System.Drawing.Color.FromArgb(41, 42, 45) };
            LvHarmonicTable.ForeColor = new System.Drawing.Color[] { System.Drawing.SystemColors.ControlLight };
            LvHarmonicTable.Dock = System.Windows.Forms.DockStyle.Fill;
            LvHarmonicTable.EnbleSelect = false;
            LvHarmonicTable.GridColor = System.Drawing.Color.Gray;
            LvHarmonicTable.GridLine = true;
            LvHarmonicTable.GridWidth = 1;
            LvHarmonicTable.HeaderBackColor = System.Drawing.Color.Gray;
            LvHarmonicTable.HeaderFont = new System.Drawing.Font("MiSans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LvHarmonicTable.Font = new System.Drawing.Font("MiSans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LvHarmonicTable.HeaderForeColor = System.Drawing.Color.White;
            LvHarmonicTable.HeaderHeight = 30;
            LvHarmonicTable.HeaderTextAlignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            LvHarmonicTable.HeaderVisility = true;
            LvHarmonicTable.ItemHeight = 23;
            LvHarmonicTable.Location = new System.Drawing.Point(3, 23);
            LvHarmonicTable.Name = "LvHarmonicTable";
            LvHarmonicTable.Orientation = ListViewEx.ColorOrientation.Horizontal;
            LvHarmonicTable.SelectedBackColor = System.Drawing.Color.Blue;
            LvHarmonicTable.SelectedForeColor = System.Drawing.Color.White;
            LvHarmonicTable.SelectedIndex = -1;
            LvHarmonicTable.Size = new System.Drawing.Size(620, 408);
            LvHarmonicTable.TabIndex = 0;
            LvHarmonicTable.TabStop = false;
            LvHarmonicTable.TextAlignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            // 
            // TmUpdate
            // 
            TmUpdate.Enabled = true;
            TmUpdate.Interval = 500;
            TmUpdate.Elapsed += TmUpdate_Tick;
            // 
            // TlpHarmonic
            // 
            TlpHarmonic.BackColor = System.Drawing.Color.Transparent;
            TlpHarmonic.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TlpHarmonic.Controls.Add(LvDistortion);
            TlpHarmonic.Controls.Add(LvHarmonicTable);
            TlpHarmonic.Dock = System.Windows.Forms.DockStyle.Top;
            TlpHarmonic.Location = new System.Drawing.Point(0, 45);
            TlpHarmonic.Name = "TlpHarmonic";
            TlpHarmonic.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            TlpHarmonic.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 333F));
            TlpHarmonic.Size = new System.Drawing.Size(435, 434);
            TlpHarmonic.TabIndex = 0;
            TlpHarmonic.Tag = "Harmonic";
            // 
            // LvDistortion
            // 
            LvDistortion.Columns.AddRange(new ListViewEx.ColumnInfo[]
            {
                new ListViewEx.ColumnInfo("", 150),
                new ListViewEx.ColumnInfo("",120),
            });
            LvDistortion.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
            LvDistortion.TextAlignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            LvDistortion.BackColor = new System.Drawing.Color[] { System.Drawing.Color.FromArgb(41, 42, 45) };
            LvDistortion.ForeColor = new System.Drawing.Color[] { System.Drawing.SystemColors.ControlLight };
            LvDistortion.Dock = System.Windows.Forms.DockStyle.Fill;
            LvDistortion.EnbleSelect = false;
            LvDistortion.GridColor = System.Drawing.Color.Gray;
            LvDistortion.GridLine = true;
            LvDistortion.GridWidth = 1;
            LvDistortion.HeaderFont = new System.Drawing.Font("MiSans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LvDistortion.Font = new System.Drawing.Font("MiSans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LvDistortion.HeaderBackColor = System.Drawing.Color.Gray;
            LvDistortion.HeaderFont = new System.Drawing.Font("MiSans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LvDistortion.HeaderForeColor = System.Drawing.Color.White;
            LvDistortion.HeaderHeight = 0;
            LvDistortion.HeaderTextAlignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            LvDistortion.HeaderVisility = false;
            LvDistortion.ItemHeight = 23;
            LvDistortion.Location = new System.Drawing.Point(3, 3);
            LvDistortion.Name = "LvDistortion";
            LvDistortion.Orientation = ListViewEx.ColorOrientation.Horizontal;
            LvDistortion.SelectedBackColor = System.Drawing.Color.Blue;
            LvDistortion.SelectedForeColor = System.Drawing.Color.White;
            LvDistortion.SelectedIndex = -1;
            LvDistortion.Size = new System.Drawing.Size(429, 14);
            LvDistortion.TabIndex = 0;
            LvDistortion.TabStop = false;
            LvDistortion.TextAlignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            // 
            // PwrHarmonicInfoForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CanClose = false;
            ClientSize = new System.Drawing.Size(680, 485);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(TlpHarmonic);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            HelpLabel = "25";
            IconWidth = 26;
            IsShowPin = false;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PwrHarmonicInfoForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XieBoFenXi");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XieBoFenXi");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleIcon = Properties.Resources.MeasureSetting;
            TitleIconSize = new System.Drawing.Size(26, 26);
            TitleIconSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            TitleIconWidth = 26;
            ToolIconSize = new System.Drawing.Size(26, 26);
            HelpClick += PwrHarmonicInfoForm_EmbededClick;
            //TitleIconClick += PwrHarmonicInfoForm_SettingClick;
            Controls.SetChildIndex(TlpHarmonic, 0);
            TlpHarmonic.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.ImageList ImageListPad;
        private System.Timers.Timer TmUpdate;
        private ScopeX.U2.ListViewEx LvDistortion;
        private ScopeX.U2.ListViewEx LvHarmonicTable;
        private System.Windows.Forms.TableLayoutPanel TlpHarmonic;
    }
}