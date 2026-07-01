
namespace ScopeX.U2
{
    partial class UserCodeForm
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
            BtnCancel = new UserControls.ScopeXIconButton();
            BtnExceteRepeat = new UserControls.ScopeXIconButton();
            GpxCodeEdit = new UserControls.ScopeXGroupBox();
            RtbxCodeEditor = new System.Windows.Forms.RichTextBox();
            GpxResult = new UserControls.ScopeXGroupBox();
            RtbxResult = new System.Windows.Forms.RichTextBox();
            BtnSetWorkFolder = new UserControls.ScopeXIconButton();
            BtnSaveToFile = new UserControls.ScopeXIconButton();
            BtnImport = new UserControls.ScopeXIconButton();
            TimerUpdate = new System.Timers.Timer();
            BtnExceteOneTime = new UserControls.ScopeXIconButton();
            GpxCodeEdit.SuspendLayout();
            GpxResult.SuspendLayout();
            SuspendLayout();
            // 
            // BtnCancel
            // 
            BtnCancel.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCancel.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCancel.BorderThickness = 0;
            BtnCancel.CornerRadius = 0;
            BtnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnCancel.DaskArray = null;
            BtnCancel.DropKey = System.Windows.Forms.Keys.Space;
            BtnCancel.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCancel.Height = 30;
            BtnCancel.Icon = null;
            BtnCancel.IconOffset = 10;
            BtnCancel.IconSize = new System.Drawing.Size(24, 24);
            BtnCancel.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCancel.IsIndicatorShow = false;
            BtnCancel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnCancel.Location = new System.Drawing.Point(752, 654);
            BtnCancel.Margin = new System.Windows.Forms.Padding(4);
            BtnCancel.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCancel.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCancel.MouseInBorderThickness = 0;
            BtnCancel.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCancel.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.PressedBackColor = System.Drawing.Color.Gray;
            BtnCancel.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnCancel.PressedBorderThickness = 0;
            BtnCancel.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCancel.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnCancel.Size = new System.Drawing.Size(120, 30);
            BtnCancel.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnCancel.StylizeFlag = true;
            BtnCancel.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnCancel.SVGPath = "";
            BtnCancel.TabIndex = 7;
            BtnCancel.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("UserCodeForm.BtnCancel"); // "停止运行";
            BtnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // BtnExceteRepeat
            // 
            BtnExceteRepeat.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnExceteRepeat.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnExceteRepeat.BorderThickness = 0;
            BtnExceteRepeat.CornerRadius = 0;
            BtnExceteRepeat.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnExceteRepeat.DaskArray = null;
            BtnExceteRepeat.DropKey = System.Windows.Forms.Keys.Space;
            BtnExceteRepeat.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnExceteRepeat.Height = 30;
            BtnExceteRepeat.Icon = null;
            BtnExceteRepeat.IconOffset = 10;
            BtnExceteRepeat.IconSize = new System.Drawing.Size(24, 24);
            BtnExceteRepeat.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnExceteRepeat.IsIndicatorShow = false;
            BtnExceteRepeat.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnExceteRepeat.Location = new System.Drawing.Point(610, 654);
            BtnExceteRepeat.Margin = new System.Windows.Forms.Padding(4);
            BtnExceteRepeat.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnExceteRepeat.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnExceteRepeat.MouseInBorderThickness = 0;
            BtnExceteRepeat.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnExceteRepeat.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnExceteRepeat.Name = "BtnExceteRepeat";
            BtnExceteRepeat.PressedBackColor = System.Drawing.Color.Gray;
            BtnExceteRepeat.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnExceteRepeat.PressedBorderThickness = 0;
            BtnExceteRepeat.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnExceteRepeat.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnExceteRepeat.Size = new System.Drawing.Size(120, 30);
            BtnExceteRepeat.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnExceteRepeat.StylizeFlag = true;
            BtnExceteRepeat.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnExceteRepeat.SVGPath = "";
            BtnExceteRepeat.TabIndex = 6;
            BtnExceteRepeat.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("UserCodeForm.BtnExceteRepeat"); // "循环运行";
            BtnExceteRepeat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnExceteRepeat.Click += BtnExcuteRepeat_Click;
            // 
            // GpxCodeEdit
            // 
            GpxCodeEdit.BackColor = System.Drawing.Color.Transparent;
            GpxCodeEdit.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            GpxCodeEdit.BorderThickness = 1U;
            GpxCodeEdit.Controls.Add(RtbxCodeEditor);
            GpxCodeEdit.Dock = System.Windows.Forms.DockStyle.Top;
            GpxCodeEdit.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            GpxCodeEdit.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            GpxCodeEdit.Location = new System.Drawing.Point(1, 46);
            GpxCodeEdit.Name = "GpxCodeEdit";
            GpxCodeEdit.Size = new System.Drawing.Size(822, 333);
            GpxCodeEdit.StyleFlags = UserControls.Style.StyleFlag.None;
            GpxCodeEdit.StylizeFlag = true;
            GpxCodeEdit.TabIndex = 0;
            GpxCodeEdit.TabStop = false;
            GpxCodeEdit.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("UserCodeForm.GpxCodeEdit"); // "代码";
            // 
            // RtbxCodeEditor
            // 
            RtbxCodeEditor.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            RtbxCodeEditor.BorderStyle = System.Windows.Forms.BorderStyle.None;
            RtbxCodeEditor.DetectUrls = false;
            RtbxCodeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            RtbxCodeEditor.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RtbxCodeEditor.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RtbxCodeEditor.Location = new System.Drawing.Point(3, 17);
            RtbxCodeEditor.Name = "RtbxCodeEditor";
            RtbxCodeEditor.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            RtbxCodeEditor.ShortcutsEnabled = false;
            RtbxCodeEditor.Size = new System.Drawing.Size(816, 313);
            RtbxCodeEditor.TabIndex = 0;
            RtbxCodeEditor.TabStop = false;
            RtbxCodeEditor.Text = "";
            RtbxCodeEditor.VScroll += RtbxCodeEditor_VScroll;
            // 
            // GpxResult
            // 
            GpxResult.BackColor = System.Drawing.Color.Transparent;
            GpxResult.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            GpxResult.BorderThickness = 1U;
            GpxResult.Controls.Add(RtbxResult);
            GpxResult.Dock = System.Windows.Forms.DockStyle.Top;
            GpxResult.ForeColor = System.Drawing.Color.FromArgb(232, 234, 237);
            GpxResult.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            GpxResult.Location = new System.Drawing.Point(1, 379);
            GpxResult.Name = "GpxResult";
            GpxResult.Size = new System.Drawing.Size(822, 256);
            GpxResult.StyleFlags = UserControls.Style.StyleFlag.None;
            GpxResult.StylizeFlag = true;
            GpxResult.TabIndex = 1;
            GpxResult.TabStop = false;
            GpxResult.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("UserCodeForm.GpxResult"); // "运行结果";
            // 
            // RtbxResult
            // 
            RtbxResult.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            RtbxResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            RtbxResult.DetectUrls = false;
            RtbxResult.Dock = System.Windows.Forms.DockStyle.Fill;
            RtbxResult.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RtbxResult.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            RtbxResult.Location = new System.Drawing.Point(3, 17);
            RtbxResult.Name = "RtbxResult";
            RtbxResult.ReadOnly = true;
            RtbxResult.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            RtbxResult.ShortcutsEnabled = false;
            RtbxResult.Size = new System.Drawing.Size(816, 236);
            RtbxResult.TabIndex = 0;
            RtbxResult.TabStop = false;
            RtbxResult.Text = "";
            // 
            // BtnSetWorkFolder
            // 
            BtnSetWorkFolder.Anchor = System.Windows.Forms.AnchorStyles.Left;
            BtnSetWorkFolder.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSetWorkFolder.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSetWorkFolder.BorderThickness = 0;
            BtnSetWorkFolder.CornerRadius = 0;
            BtnSetWorkFolder.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSetWorkFolder.DaskArray = null;
            BtnSetWorkFolder.DropKey = System.Windows.Forms.Keys.Space;
            BtnSetWorkFolder.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSetWorkFolder.Height = 30;
            BtnSetWorkFolder.Icon = null;
            BtnSetWorkFolder.IconOffset = 10;
            BtnSetWorkFolder.IconSize = new System.Drawing.Size(24, 24);
            BtnSetWorkFolder.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSetWorkFolder.IsIndicatorShow = false;
            BtnSetWorkFolder.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSetWorkFolder.Location = new System.Drawing.Point(10, 654);
            BtnSetWorkFolder.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSetWorkFolder.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSetWorkFolder.MouseInBorderThickness = 0;
            BtnSetWorkFolder.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSetWorkFolder.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSetWorkFolder.Name = "BtnSetWorkFolder";
            BtnSetWorkFolder.PressedBackColor = System.Drawing.Color.Gray;
            BtnSetWorkFolder.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSetWorkFolder.PressedBorderThickness = 0;
            BtnSetWorkFolder.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSetWorkFolder.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSetWorkFolder.Size = new System.Drawing.Size(182, 30);
            BtnSetWorkFolder.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnSetWorkFolder.StylizeFlag = true;
            BtnSetWorkFolder.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSetWorkFolder.SVGPath = "";
            BtnSetWorkFolder.TabIndex = 2;
            BtnSetWorkFolder.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("UserCodeForm.BtnSetWorkFolder"); // "设置工作目录";
            BtnSetWorkFolder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSetWorkFolder.Click += BtnSetWorkFolder_Click;
            // 
            // BtnSaveToFile
            // 
            BtnSaveToFile.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveToFile.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveToFile.BorderThickness = 0;
            BtnSaveToFile.CornerRadius = 0;
            BtnSaveToFile.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSaveToFile.DaskArray = null;
            BtnSaveToFile.DropKey = System.Windows.Forms.Keys.Space;
            BtnSaveToFile.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveToFile.Height = 30;
            BtnSaveToFile.Icon = null;
            BtnSaveToFile.IconOffset = 10;
            BtnSaveToFile.IconSize = new System.Drawing.Size(24, 24);
            BtnSaveToFile.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSaveToFile.IsIndicatorShow = false;
            BtnSaveToFile.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSaveToFile.Location = new System.Drawing.Point(346, 654);
            BtnSaveToFile.Margin = new System.Windows.Forms.Padding(4);
            BtnSaveToFile.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveToFile.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveToFile.MouseInBorderThickness = 0;
            BtnSaveToFile.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveToFile.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSaveToFile.Name = "BtnSaveToFile";
            BtnSaveToFile.PressedBackColor = System.Drawing.Color.Gray;
            BtnSaveToFile.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveToFile.PressedBorderThickness = 0;
            BtnSaveToFile.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveToFile.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSaveToFile.Size = new System.Drawing.Size(110, 30);
            BtnSaveToFile.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnSaveToFile.StylizeFlag = true;
            BtnSaveToFile.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveToFile.SVGPath = "";
            BtnSaveToFile.TabIndex = 4;
            BtnSaveToFile.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("UserCodeForm.BtnSaveToFile"); // "保存代码";
            BtnSaveToFile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSaveToFile.Click += BtnSaveToFile_Click;
            // 
            // BtnImport
            // 
            BtnImport.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnImport.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnImport.BorderThickness = 0;
            BtnImport.CornerRadius = 0;
            BtnImport.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnImport.DaskArray = null;
            BtnImport.DropKey = System.Windows.Forms.Keys.Space;
            BtnImport.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnImport.Height = 30;
            BtnImport.Icon = null;
            BtnImport.IconOffset = 10;
            BtnImport.IconSize = new System.Drawing.Size(24, 24);
            BtnImport.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnImport.IsIndicatorShow = false;
            BtnImport.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnImport.Location = new System.Drawing.Point(214, 654);
            BtnImport.Margin = new System.Windows.Forms.Padding(4);
            BtnImport.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnImport.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnImport.MouseInBorderThickness = 0;
            BtnImport.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnImport.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnImport.Name = "BtnImport";
            BtnImport.PressedBackColor = System.Drawing.Color.Gray;
            BtnImport.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnImport.PressedBorderThickness = 0;
            BtnImport.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnImport.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnImport.Size = new System.Drawing.Size(110, 30);
            BtnImport.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnImport.StylizeFlag = true;
            BtnImport.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnImport.SVGPath = "";
            BtnImport.TabIndex = 3;
            BtnImport.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("UserCodeForm.BtnImport"); // "导入代码";
            BtnImport.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnImport.Click += BtnImport_Click;
            // 
            // TimerUpdate
            // 
            TimerUpdate.Interval = 50;
            TimerUpdate.Elapsed += TimerUpdate_Tick;
            // 
            // BtnExceteOneTime
            // 
            BtnExceteOneTime.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnExceteOneTime.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnExceteOneTime.BorderThickness = 0;
            BtnExceteOneTime.CornerRadius = 0;
            BtnExceteOneTime.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnExceteOneTime.DaskArray = null;
            BtnExceteOneTime.DropKey = System.Windows.Forms.Keys.Space;
            BtnExceteOneTime.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnExceteOneTime.Height = 30;
            BtnExceteOneTime.Icon = null;
            BtnExceteOneTime.IconOffset = 10;
            BtnExceteOneTime.IconSize = new System.Drawing.Size(24, 24);
            BtnExceteOneTime.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnExceteOneTime.IsIndicatorShow = false;
            BtnExceteOneTime.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnExceteOneTime.Location = new System.Drawing.Point(478, 654);
            BtnExceteOneTime.Margin = new System.Windows.Forms.Padding(4);
            BtnExceteOneTime.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnExceteOneTime.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnExceteOneTime.MouseInBorderThickness = 0;
            BtnExceteOneTime.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnExceteOneTime.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnExceteOneTime.Name = "BtnExceteOneTime";
            BtnExceteOneTime.PressedBackColor = System.Drawing.Color.Gray;
            BtnExceteOneTime.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnExceteOneTime.PressedBorderThickness = 0;
            BtnExceteOneTime.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnExceteOneTime.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnExceteOneTime.Size = new System.Drawing.Size(110, 30);
            BtnExceteOneTime.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnExceteOneTime.StylizeFlag = true;
            BtnExceteOneTime.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnExceteOneTime.SVGPath = "";
            BtnExceteOneTime.TabIndex = 5;
            BtnExceteOneTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("UserCodeForm.BtnExceteOneTime"); // "运行一次";
            BtnExceteOneTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnExceteOneTime.Click += BtnExceteOneTime_Click;
            // 
            // UserCodeForm
            // 
            ActiveBorderColor = System.Drawing.Color.FromArgb(11, 39, 133);
            ActiveBorderVisiable = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(900, 696);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(BtnExceteOneTime);
            Controls.Add(BtnSaveToFile);
            Controls.Add(BtnImport);
            Controls.Add(BtnSetWorkFolder);
            Controls.Add(GpxResult);
            Controls.Add(GpxCodeEdit);
            Controls.Add(BtnCancel);
            Controls.Add(BtnExceteRepeat);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            HelpLabel = "20";
            IconWidth = 26;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "UserCodeForm";
            Padding = new System.Windows.Forms.Padding(1);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("UserCodeForm"); // "代码编辑器";
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("UserCodeForm", "Title"); // "代码编辑器";
            TitleColor = System.Drawing.Color.FromArgb(234, 234, 234);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ToolIconSize = new System.Drawing.Size(26, 26);
            Controls.SetChildIndex(BtnExceteRepeat, 0);
            Controls.SetChildIndex(BtnCancel, 0);
            Controls.SetChildIndex(GpxCodeEdit, 0);
            Controls.SetChildIndex(GpxResult, 0);
            Controls.SetChildIndex(BtnSetWorkFolder, 0);
            Controls.SetChildIndex(BtnImport, 0);
            Controls.SetChildIndex(BtnSaveToFile, 0);
            Controls.SetChildIndex(BtnExceteOneTime, 0);
            GpxCodeEdit.ResumeLayout(false);
            GpxResult.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXIconButton BtnCancel;
        private ScopeX.UserControls.ScopeXIconButton BtnExceteRepeat;
        private System.Windows.Forms.RichTextBox RtbxCodeEditor;
        private ScopeX.UserControls.ScopeXGroupBox GpxCodeEdit;
        private ScopeX.UserControls.ScopeXGroupBox GpxResult;
        private System.Windows.Forms.RichTextBox RtbxResult;
        private ScopeX.UserControls.ScopeXIconButton BtnSetWorkFolder;
        private ScopeX.UserControls.ScopeXIconButton BtnSaveToFile;
        private ScopeX.UserControls.ScopeXIconButton BtnImport;
        private System.Timers.Timer TimerUpdate;
        private ScopeX.UserControls.ScopeXIconButton BtnExceteOneTime;
        private ScopeX.UserControls.ScopeXIconButton btnSelectMatlabType0;
    }
}