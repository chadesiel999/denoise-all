
namespace ScopeX.U2
{
    partial class DebugForm
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
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new ScopeX.UserControls.DefaultHighlightPrompt();
            this.LblDataSource = new ScopeX.UserControls.ScopeXLabel();
            this.CbxDataSource = new ScopeX.UserControls.ComboBoxEx();
            this.SuspendLayout();
            // 
            // LblDataSource
            // 
            this.LblDataSource.BackColor = System.Drawing.Color.Transparent;
            this.LblDataSource.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.LblDataSource.BorderThickness = 0;
            this.LblDataSource.CornerRadius = 0;
            this.LblDataSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblDataSource.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.LblDataSource.HighlightPrompt = defaultHighlightPrompt1;
            this.LblDataSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.LblDataSource.Location = new System.Drawing.Point(22, 54);
            this.LblDataSource.MultyLineFlag = false;
            this.LblDataSource.Name = "LblDataSource";
            this.LblDataSource.Size = new System.Drawing.Size(100, 18);
            this.LblDataSource.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.LblDataSource.StylizeFlag = true;
            this.LblDataSource.TabIndex = 0;
            this.LblDataSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DiaoShiShuJuYuan");
            this.LblDataSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblDataSource.Token = null;
            // 
            // CbxDataSource
            // 
            this.CbxDataSource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxDataSource.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxDataSource.BorderThickness = 0;
            this.CbxDataSource.CornerRadius = 0;
            this.CbxDataSource.DataSource = null;
            this.CbxDataSource.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CbxDataSource.DropDownHeight = 200;
            this.CbxDataSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxDataSource.DropDownWidth = 100;
            this.CbxDataSource.DropKey = System.Windows.Forms.Keys.Space;
            this.CbxDataSource.DroppedDown = false;
            this.CbxDataSource.FocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(54)))), ((int)(((byte)(58)))));
            this.CbxDataSource.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CbxDataSource.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(192)))), ((int)(((byte)(199)))));
            this.CbxDataSource.FormattingEnabled = true;
            this.CbxDataSource.GetDisPlayName = null;
            this.CbxDataSource.Height = 30;
            this.CbxDataSource.ImageMode = false;
            this.CbxDataSource.ItemHeight = 28;
            this.CbxDataSource.Items.AddRange(new object[] {
            "PCIe",
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FangZhen")});
            this.CbxDataSource.KeyDropEnble = true;
            this.CbxDataSource.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            this.CbxDataSource.Location = new System.Drawing.Point(22, 78);
            this.CbxDataSource.MaxDropDownItems = 8;
            this.CbxDataSource.Name = "CbxDataSource";
            this.CbxDataSource.RectBtnWidth = 20;
            this.CbxDataSource.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(157)))), ((int)(((byte)(255)))));
            this.CbxDataSource.SelectedIndex = -1;
            this.CbxDataSource.SelectedItem = null;
            this.CbxDataSource.SelectedText = "";
            this.CbxDataSource.Size = new System.Drawing.Size(100, 30);
            this.CbxDataSource.Soreted = false;
            this.CbxDataSource.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            this.CbxDataSource.StylizeFlag = true;
            this.CbxDataSource.TabIndex = 1;
            this.CbxDataSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CaiJiBan");
            this.CbxDataSource.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.CbxDataSource.VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.CbxDataSource.SelectedIndexChanged += new System.EventHandler(this.CbxDataSource_SelectedIndexChanged);
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CanClose = false;
            this.ClientSize = new System.Drawing.Size(273, 126);
            this.ContentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.ControlBox = false;
            this.Controls.Add(this.LblDataSource);
            this.Controls.Add(this.CbxDataSource);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormOpacity = 95;
            this.HeadBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.IconInterval = 21;
            this.IconWidth = 26;
            this.IsShowPin = false;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DebugForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.StylizeFlag = true;
            this.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DiaoShi");
            this.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DiaoShi");
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ToolIconInterval = 21;
            this.ToolIconSize = new System.Drawing.Size(26, 26);
            this.Controls.SetChildIndex(this.CbxDataSource, 0);
            this.Controls.SetChildIndex(this.LblDataSource, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private ScopeX.UserControls.ScopeXLabel LblDataSource;
        private ScopeX.UserControls.ComboBoxEx CbxDataSource;
    }
}