using Microsoft.VisualBasic.FileIO;
using System.Windows.Forms;

namespace ScopeX.U2
{
    partial class DetailsForm
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
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt1 = new ScopeX.UserControls.DefaultHighlightPrompt();
            ScopeX.UserControls.DefaultHighlightPrompt defaultHighlightPrompt2 = new ScopeX.UserControls.DefaultHighlightPrompt();
            tableLayoutPanel1 = new TableLayoutPanel();
            LblFpga = new ScopeX.UserControls.ScopeXLabel();
            LblModel = new ScopeX.UserControls.ScopeXLabel();
            RTBFpgaInfo = new RichTextBox();
            ScOption = new ScopeX.UserControls.ScopeXScrollContainer();
            LvOption = new ScopeX.UserControls.ScopeXListViewEx();
            tableLayoutPanel1.SuspendLayout();
            ScOption.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(LblFpga, 0, 0);
            tableLayoutPanel1.Controls.Add(LblModel, 0, 2);
            tableLayoutPanel1.Controls.Add(RTBFpgaInfo, 0, 1);
            tableLayoutPanel1.Controls.Add(ScOption, 0, 3);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(2, 40);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            tableLayoutPanel1.Size = new System.Drawing.Size(551, 558);
            tableLayoutPanel1.TabIndex = 5;
            // 
            // LblFpga
            // 
            LblFpga.AccessibleRole = AccessibleRole.IpAddress;
            LblFpga.BackColor = System.Drawing.Color.Empty;
            LblFpga.BorderColor = System.Drawing.Color.Black;
            LblFpga.BorderThickness = 0;
            LblFpga.CornerRadius = 0;
            LblFpga.Dock = DockStyle.Bottom;
            LblFpga.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblFpga.ForeColor = System.Drawing.Color.White;
            LblFpga.HighlightPrompt = defaultHighlightPrompt1;
            LblFpga.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblFpga.Location = new System.Drawing.Point(10, 16);
            LblFpga.Margin = new Padding(10, 3, 3, 3);
            LblFpga.MultyLineFlag = false;
            LblFpga.Name = "LblFpga";
            LblFpga.Size = new System.Drawing.Size(538, 16);
            LblFpga.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblFpga.StylizeFlag = true;
            LblFpga.TabIndex = 15;
            LblFpga.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuJianXinXi_");
            LblFpga.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            LblFpga.Token = null;
            // 
            // LblModel
            // 
            LblModel.AccessibleRole = AccessibleRole.IpAddress;
            LblModel.BackColor = System.Drawing.Color.Empty;
            LblModel.BorderColor = System.Drawing.Color.Black;
            LblModel.BorderThickness = 0;
            LblModel.CornerRadius = 0;
            LblModel.Dock = DockStyle.Bottom;
            LblModel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblModel.ForeColor = System.Drawing.Color.White;
            LblModel.HighlightPrompt = defaultHighlightPrompt2;
            LblModel.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            LblModel.Location = new System.Drawing.Point(10, 197);
            LblModel.Margin = new Padding(10, 3, 3, 3);
            LblModel.MultyLineFlag = false;
            LblModel.Name = "LblModel";
            LblModel.Size = new System.Drawing.Size(538, 16);
            LblModel.StyleFlags = ScopeX.UserControls.Style.StyleFlag.None;
            LblModel.StylizeFlag = true;
            LblModel.TabIndex = 14;
            LblModel.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YiJiaZaiMoKuai_");
            LblModel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            LblModel.Token = null;
            // 
            // RTBFpgaInfo
            // 
            RTBFpgaInfo.BackColor = System.Drawing.Color.FromArgb(52, 52, 52);
            RTBFpgaInfo.BorderStyle = BorderStyle.None;
            RTBFpgaInfo.Dock = DockStyle.Fill;
            RTBFpgaInfo.ForeColor = System.Drawing.Color.White;
            RTBFpgaInfo.Location = new System.Drawing.Point(10, 38);
            RTBFpgaInfo.Margin = new Padding(10, 3, 10, 10);
            RTBFpgaInfo.Name = "RTBFpgaInfo";
            RTBFpgaInfo.ReadOnly = true;
            RTBFpgaInfo.Size = new System.Drawing.Size(531, 133);
            RTBFpgaInfo.TabIndex = 16;
            RTBFpgaInfo.Text = "";
            // 
            // ScOption
            // 
            ScOption.BackColor = System.Drawing.Color.FromArgb(45, 46, 50);
            tableLayoutPanel1.SetColumnSpan(ScOption, 2);
            ScOption.Controls.Add(LvOption);
            ScOption.Dock = DockStyle.Fill;
            ScOption.Location = new System.Drawing.Point(3, 219);
            ScOption.Name = "ScOption";
            ScOption.ScrollControl = LvOption;
            ScOption.ScrollThickness = 6;
            ScOption.Size = new System.Drawing.Size(545, 336);
            ScOption.TabIndex = 16;
            // 
            // LvOption
            // 
            LvOption.BackColor = System.Drawing.Color.FromArgb(45, 46, 50);
            LvOption.BorderStyle = BorderStyle.None;
            LvOption.Dock = DockStyle.Fill;
            LvOption.ForeColor = System.Drawing.Color.FromArgb(234, 234, 234);
            LvOption.FullRowSelect = true;
            LvOption.GridLinesColor = System.Drawing.Color.Black;
            LvOption.HeaderBackColor = System.Drawing.Color.Red;
            LvOption.HeaderForeColor = System.Drawing.Color.Silver;
            LvOption.IsIndependentWindow = false;
            LvOption.Location = new System.Drawing.Point(0, 0);
            LvOption.Margin = new Padding(10, 3, 10, 10);
            LvOption.Name = "LvOption";
            LvOption.OwnerDraw = true;
            LvOption.RowHeight = 20;
            LvOption.ScrollContainer = ScOption;
            LvOption.SelectedRowColor = System.Drawing.Color.MediumSeaGreen;
            LvOption.Size = new System.Drawing.Size(545, 336);
            LvOption.TabIndex = 13;
            LvOption.UseCompatibleStateImageBehavior = false;
            LvOption.View = View.Details;
            // 
            // DetailsForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            CanClose = false;
            ClientSize = new System.Drawing.Size(555, 600);
            Controls.Add(tableLayoutPanel1);
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            IsShowPin = false;
            Name = "DetailsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiangXiXinXi_");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiangXiXinXi_");
            Controls.SetChildIndex(tableLayoutPanel1, 0);
            tableLayoutPanel1.ResumeLayout(false);
            ScOption.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ScopeX.UserControls.ScopeXLabel LblModel;
        private ScopeX.UserControls.ScopeXLabel ScopeXLabel1;
        private ScopeX.UserControls.ScopeXScrollContainer ScOption;
        private ScopeX.UserControls.ScopeXLabel LblFpga;
        private RichTextBox RTBFpgaInfo;
        private ScopeX.UserControls.ScopeXListViewEx LvOption;
    }
}