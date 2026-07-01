using System.Windows.Forms;

namespace ScopeX.U2
{
    partial class EyePatternParameterForm
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
            LvEyePatternParameter = new ScopeX.UserControls.ScopeXListViewEx();
            Parameter = new System.Windows.Forms.ColumnHeader();
            Value = new System.Windows.Forms.ColumnHeader();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // LvEyePatternParameter
            // 
            LvEyePatternParameter.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LvEyePatternParameter.BorderStyle = System.Windows.Forms.BorderStyle.None;
            LvEyePatternParameter.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { Parameter, Value });
            LvEyePatternParameter.Dock = System.Windows.Forms.DockStyle.Fill;
            LvEyePatternParameter.ForeColor = System.Drawing.SystemColors.ControlLight;
            LvEyePatternParameter.FullRowSelect = true;
            LvEyePatternParameter.GridLines = true;
            LvEyePatternParameter.GridLinesColor = System.Drawing.Color.Gray;
            LvEyePatternParameter.HeaderBackColor = System.Drawing.Color.Gray;
            LvEyePatternParameter.HeaderForeColor = System.Drawing.Color.White;
            LvEyePatternParameter.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            LvEyePatternParameter.HideSelection = true;
            LvEyePatternParameter.IsIndependentWindow = false;
            LvEyePatternParameter.Location = new System.Drawing.Point(3, 3);
            LvEyePatternParameter.MultiSelect = false;
            LvEyePatternParameter.Name = "LvEyePatternParameter";
            LvEyePatternParameter.OwnerDraw = true;
            LvEyePatternParameter.RowHeight = 23;
            LvEyePatternParameter.ScrollContainer = null;
            LvEyePatternParameter.SelectedRowColor = System.Drawing.Color.Blue;
            LvEyePatternParameter.Size = new System.Drawing.Size(340, 248);
            LvEyePatternParameter.TabIndex = 8;
            LvEyePatternParameter.TabStop = false;
            LvEyePatternParameter.Tag = "";
            LvEyePatternParameter.UseCompatibleStateImageBehavior = false;
            LvEyePatternParameter.View = System.Windows.Forms.View.Details;
            // 
            // Parameter
            // 
            Parameter.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CeLiangXiang");
            Parameter.Width = 100;
            // 
            // Value
            // 
            Value.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DangQianZhi");
            Value.Width = 180;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(LvEyePatternParameter, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(2, 45);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            tableLayoutPanel1.Size = new System.Drawing.Size(355, 250);
            tableLayoutPanel1.TabIndex = 10;
            // 
            // EyePatternParameterForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderThickness = 2;
            CanClose = false;
            ClientSize = new System.Drawing.Size(360, 300);
            Controls.Add(tableLayoutPanel1);
            IconWidth = 26;
            IsShowPin = false;
            Name = "EyePatternParameterForm";
            StylizeFlag = true;
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YanTuCanShu");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YanTuCanShu");
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ToolIconSize = new System.Drawing.Size(26, 26);
            HelpClick += EyePatternParameterForm_HelpClick;
            Controls.SetChildIndex(tableLayoutPanel1, 0);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXListViewEx LvEyePatternParameter;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}