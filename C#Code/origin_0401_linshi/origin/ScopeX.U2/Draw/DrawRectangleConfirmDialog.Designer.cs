using ScopeX.UserControls;
using Svg;

namespace ScopeX.U2
{
    partial class DrawRectangleConfirmDialog
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
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            BtnHistogram = new ScopeXIconButton();
            BtnZoom = new ScopeXIconButton();
            BtnTrigger = new ScopeXIconButton();
            BtnTriggerNotInterSect = new ScopeXIconButton();
            BtnSaveScreenData= new ScopeXIconButton();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(BtnHistogram, 0, 3);
            tableLayoutPanel1.Controls.Add(BtnZoom, 0, 0);
            tableLayoutPanel1.Controls.Add(BtnTrigger, 0, 1);
            tableLayoutPanel1.Controls.Add(BtnTriggerNotInterSect, 0, 3);
            tableLayoutPanel1.Controls.Add(BtnSaveScreenData, 0, 4);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(1, 1);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(173, 196);
            tableLayoutPanel1.TabIndex = 5;
            // 
            // BtnHistogram
            // 
            BtnHistogram.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnHistogram.IsChoosed = false;
            BtnHistogram.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnHistogram.BorderThickness = 1;
            BtnHistogram.CornerRadius = 0;
            BtnHistogram.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnHistogram.DaskArray = null;
            BtnHistogram.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnHistogram.DropKey = System.Windows.Forms.Keys.Space;
            BtnHistogram.FocusedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnHistogram.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnHistogram.ForeColor = System.Drawing.Color.White;
            BtnHistogram.Height = 48;
            BtnHistogram.Icon = null;
            BtnHistogram.IconOffset = 10;
            BtnHistogram.IconSize = new System.Drawing.Size(24, 24);
            BtnHistogram.IndicatorColor = System.Drawing.Color.Empty;
            BtnHistogram.IsChoosed = false;
            BtnHistogram.IsIndicatorShow = false;
            BtnHistogram.IsInputFocus = false;
            BtnHistogram.Adjustable = false;
            BtnHistogram.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnHistogram.Location = new System.Drawing.Point(0, 144);
            BtnHistogram.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            BtnHistogram.MouseinBackColor = System.Drawing.Color.Transparent;
            BtnHistogram.MouseinBorderColor = System.Drawing.Color.Transparent;
            BtnHistogram.MouseInBorderThickness = 0;
            BtnHistogram.MouseinForeColor = System.Drawing.Color.White;
            BtnHistogram.MouseinSvgForeColor = System.Drawing.Color.Empty;
            BtnHistogram.Name = "BtnHistogram";
            BtnHistogram.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            BtnHistogram.PressedBackColor = System.Drawing.Color.Transparent;
            BtnHistogram.PressedBorderColor = System.Drawing.Color.Transparent;
            BtnHistogram.PressedBorderThickness = 0;
            BtnHistogram.PressedForeColor = System.Drawing.Color.White;
            BtnHistogram.PressedSvgForeColor = System.Drawing.Color.Empty;
            BtnHistogram.Size = new System.Drawing.Size(173, 48);
            BtnHistogram.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnHistogram.StylizeFlag = false;
            BtnHistogram.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnHistogram.SVGPath = "";
            BtnHistogram.Text = "区域直方图";
            BtnHistogram.TabStop = false;
            BtnHistogram.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            BtnHistogram.Click += BtnHistogram_Click;
            // 
            // BtnZoom
            // 
            BtnZoom.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnZoom.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnZoom.BorderThickness = 1;
            BtnZoom.IsChoosed = false;
            BtnZoom.CornerRadius = 0;
            BtnZoom.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnZoom.DaskArray = null;
            BtnZoom.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnZoom.DropKey = System.Windows.Forms.Keys.Space;
            BtnZoom.FocusedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnZoom.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnZoom.ForeColor = System.Drawing.Color.White;
            BtnZoom.Height = 48;
            BtnZoom.Icon = null;
            BtnZoom.IconOffset = 10;
            BtnZoom.IconSize = new System.Drawing.Size(24, 24);
            BtnZoom.IndicatorColor = System.Drawing.Color.Empty;
            BtnZoom.IsChoosed = false;
            BtnZoom.IsIndicatorShow = false;
            BtnZoom.IsInputFocus = false;
            BtnZoom.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnZoom.Location = new System.Drawing.Point(0, 0);
            BtnZoom.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            BtnZoom.MouseinBackColor = System.Drawing.Color.Transparent;
            BtnZoom.MouseinBorderColor = System.Drawing.Color.Transparent;
            BtnZoom.MouseInBorderThickness = 0;
            BtnZoom.MouseinForeColor = System.Drawing.Color.White;
            BtnZoom.MouseinSvgForeColor = System.Drawing.Color.Empty;
            BtnZoom.Name = "BtnZoom";
            BtnZoom.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            BtnZoom.PressedBackColor = System.Drawing.Color.Transparent;
            BtnZoom.PressedBorderColor = System.Drawing.Color.Transparent;
            BtnZoom.PressedBorderThickness = 0;
            BtnZoom.PressedForeColor = System.Drawing.Color.White;
            BtnZoom.PressedSvgForeColor = System.Drawing.Color.Empty;
            BtnZoom.Adjustable = false;
            BtnZoom.Size = new System.Drawing.Size(173, 48);
            BtnZoom.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnZoom.StylizeFlag = false;
            BtnZoom.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnZoom.SVGPath = "";
            BtnZoom.Text = "缩放";
            BtnZoom.TabStop = false;
            BtnZoom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            BtnZoom.Click += BtnZoom_Click;
            // 
            // BtnSaveScreenData
            // 
            BtnSaveScreenData.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveScreenData.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveScreenData.BorderThickness = 1;
            BtnSaveScreenData.IsChoosed = false;
            BtnSaveScreenData.CornerRadius = 0;
            BtnSaveScreenData.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnSaveScreenData.DaskArray = null;
            BtnSaveScreenData.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnSaveScreenData.DropKey = System.Windows.Forms.Keys.Space;
            BtnSaveScreenData.FocusedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSaveScreenData.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnSaveScreenData.ForeColor = System.Drawing.Color.White;
            BtnSaveScreenData.Height = 48;
            BtnSaveScreenData.Icon = null;
            BtnSaveScreenData.IconOffset = 10;
            BtnSaveScreenData.IconSize = new System.Drawing.Size(24, 24);
            BtnSaveScreenData.IndicatorColor = System.Drawing.Color.Empty;
            BtnSaveScreenData.IsChoosed = false;
            BtnSaveScreenData.IsIndicatorShow = false;
            BtnSaveScreenData.IsInputFocus = false;
            BtnSaveScreenData.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSaveScreenData.Location = new System.Drawing.Point(0, 0);
            BtnSaveScreenData.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            BtnSaveScreenData.MouseinBackColor = System.Drawing.Color.Transparent;
            BtnSaveScreenData.MouseinBorderColor = System.Drawing.Color.Transparent;
            BtnSaveScreenData.MouseInBorderThickness = 0;
            BtnSaveScreenData.MouseinForeColor = System.Drawing.Color.White;
            BtnSaveScreenData.MouseinSvgForeColor = System.Drawing.Color.Empty;
            BtnSaveScreenData.Name = "BtnSaveScreenData";
            BtnSaveScreenData.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            BtnSaveScreenData.PressedBackColor = System.Drawing.Color.Transparent;
            BtnSaveScreenData.PressedBorderColor = System.Drawing.Color.Transparent;
            BtnSaveScreenData.PressedBorderThickness = 0;
            BtnSaveScreenData.PressedForeColor = System.Drawing.Color.White;
            BtnSaveScreenData.PressedSvgForeColor = System.Drawing.Color.Empty;
            BtnSaveScreenData.Adjustable = false;
            BtnSaveScreenData.Size = new System.Drawing.Size(173, 48);
            BtnSaveScreenData.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnSaveScreenData.StylizeFlag = false;
            BtnSaveScreenData.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSaveScreenData.SVGPath = "";
            BtnSaveScreenData.Text = "保存数据";
            BtnSaveScreenData.TabStop = false;
            BtnSaveScreenData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            BtnSaveScreenData.Click += BtnSaveScreenData_Click;
            BtnSaveScreenData.Visible = false;
            // 
            // BtnTrigger
            // 
            BtnTrigger.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTrigger.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTrigger.BorderThickness = 1;
            BtnTrigger.IsChoosed = false;
            BtnTrigger.CornerRadius = 0;
            BtnTrigger.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnTrigger.DaskArray = null;
            BtnTrigger.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnTrigger.DropKey = System.Windows.Forms.Keys.Space;
            BtnTrigger.FocusedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTrigger.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnTrigger.ForeColor = System.Drawing.Color.White;
            BtnTrigger.Height = 48;
            BtnTrigger.Icon = null;
            BtnTrigger.IconOffset = 10;
            BtnTrigger.Adjustable = false;
            BtnTrigger.IconSize = new System.Drawing.Size(24, 24);
            BtnTrigger.IndicatorColor = System.Drawing.Color.Empty;
            BtnTrigger.IsChoosed = false;
            BtnTrigger.IsIndicatorShow = false;
            BtnTrigger.IsInputFocus = false;
            BtnTrigger.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnTrigger.Location = new System.Drawing.Point(0, 48);
            BtnTrigger.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            BtnTrigger.MouseinBackColor = System.Drawing.Color.Transparent;
            BtnTrigger.MouseinBorderColor = System.Drawing.Color.Transparent;
            BtnTrigger.MouseInBorderThickness = 0;
            BtnTrigger.MouseinForeColor = System.Drawing.Color.White;
            BtnTrigger.MouseinSvgForeColor = System.Drawing.Color.Empty;
            BtnTrigger.Name = "BtnTrigger";
            BtnTrigger.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            BtnTrigger.PressedBackColor = System.Drawing.Color.Transparent;
            BtnTrigger.PressedBorderColor = System.Drawing.Color.Transparent;
            BtnTrigger.PressedBorderThickness = 0;
            BtnTrigger.PressedForeColor = System.Drawing.Color.White;
            BtnTrigger.PressedSvgForeColor = System.Drawing.Color.Empty;
            BtnTrigger.Size = new System.Drawing.Size(173, 48);
            BtnTrigger.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnTrigger.StylizeFlag = false;
            BtnTrigger.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnTrigger.SVGPath = "";
            BtnTrigger.Text = "区域触发 - 相交";
            BtnTrigger.TabStop= false;
            BtnTrigger.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            BtnTrigger.Click += BtnTrigger_Click;
            // 
            // BtnTriggerNotInterSect
            // 
            BtnTriggerNotInterSect.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTriggerNotInterSect.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTriggerNotInterSect.BorderThickness = 1;
            BtnTriggerNotInterSect.IsChoosed = false;
            BtnTriggerNotInterSect.CornerRadius = 0;
            BtnTriggerNotInterSect.Cursor = System.Windows.Forms.Cursors.Hand;
            BtnTriggerNotInterSect.DaskArray = null;
            BtnTriggerNotInterSect.Dock = System.Windows.Forms.DockStyle.Fill;
            BtnTriggerNotInterSect.DropKey = System.Windows.Forms.Keys.Space;
            BtnTriggerNotInterSect.FocusedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnTriggerNotInterSect.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnTriggerNotInterSect.ForeColor = System.Drawing.Color.White;
            BtnTriggerNotInterSect.Height = 48;
            BtnTriggerNotInterSect.Icon = null;
            BtnTriggerNotInterSect.IconOffset = 10;
            BtnTriggerNotInterSect.IconSize = new System.Drawing.Size(24, 24);
            BtnTriggerNotInterSect.IndicatorColor = System.Drawing.Color.Empty;
            BtnTriggerNotInterSect.IsChoosed = false;
            BtnTriggerNotInterSect.IsIndicatorShow = false;
            BtnTriggerNotInterSect.IsInputFocus = false;
            BtnTriggerNotInterSect.Adjustable = false;
            BtnTriggerNotInterSect.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnTriggerNotInterSect.Location = new System.Drawing.Point(0, 96);
            BtnTriggerNotInterSect.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            BtnTriggerNotInterSect.MouseinBackColor = System.Drawing.Color.Transparent;
            BtnTriggerNotInterSect.MouseinBorderColor = System.Drawing.Color.Transparent;
            BtnTriggerNotInterSect.MouseInBorderThickness = 0;
            BtnTriggerNotInterSect.MouseinForeColor = System.Drawing.Color.White;
            BtnTriggerNotInterSect.MouseinSvgForeColor = System.Drawing.Color.Empty;
            BtnTriggerNotInterSect.Name = "BtnTriggerNotInterSect";
            BtnTriggerNotInterSect.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            BtnTriggerNotInterSect.PressedBackColor = System.Drawing.Color.Transparent;
            BtnTriggerNotInterSect.PressedBorderColor = System.Drawing.Color.Transparent;
            BtnTriggerNotInterSect.PressedBorderThickness = 0;
            BtnTriggerNotInterSect.PressedForeColor = System.Drawing.Color.White;
            BtnTriggerNotInterSect.PressedSvgForeColor = System.Drawing.Color.Empty;
            BtnTriggerNotInterSect.Size = new System.Drawing.Size(173, 48);
            BtnTriggerNotInterSect.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnTriggerNotInterSect.StylizeFlag = false;
            BtnTriggerNotInterSect.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnTriggerNotInterSect.SVGPath = "";
            BtnTriggerNotInterSect.Text = "区域触发 - 不相交";
            BtnTriggerNotInterSect.TabStop = false;
            BtnTriggerNotInterSect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            BtnTriggerNotInterSect.Click += BtnTriggerNotInterSect_Click;
            // 
            // DrawRectangleConfirmDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(175, 210);
            ContentBackColor = System.Drawing.Color.FromArgb(40, 41, 44);
            ControlBox = false;
            Controls.Add(tableLayoutPanel1);
            DoubleBuffered = true;
            ForeColor = System.Drawing.Color.White;
            FormOpacity = 95;
            HelpLabel = "18";
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DrawRectangleConfirmDialog";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Title = "";
            TitleColor = System.Drawing.Color.White;
            TitleIconSize = new System.Drawing.Size(22, 26);
            TitleIconWidth = 22;
            TitleLableHeight = 1;
            ToolIconSize = new System.Drawing.Size(22, 0);
            Controls.SetChildIndex(tableLayoutPanel1, 0);
            tableLayoutPanel1.TabStop = false;
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ScopeXIconButton BtnTrigger;
        private ScopeXIconButton BtnZoom;
        private ScopeXIconButton BtnHistogram;
        private ScopeXIconButton BtnTriggerNotInterSect;
        private ScopeXIconButton BtnSaveScreenData;
    }
}