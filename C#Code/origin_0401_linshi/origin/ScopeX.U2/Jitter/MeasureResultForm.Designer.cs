namespace ScopeX.U2
{
    partial class MeasureResultForm
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
            LvMeasureResult = new UserControls.ScopeXListViewEx();
            Parameter = new System.Windows.Forms.ColumnHeader();
            Value = new System.Windows.Forms.ColumnHeader();
            Detail = new System.Windows.Forms.ColumnHeader();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // LvMeasureResult
            // 
            LvMeasureResult.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            LvMeasureResult.HeaderBackColor = System.Drawing.Color.FromArgb(38, 38, 46);
            LvMeasureResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            LvMeasureResult.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { Parameter, Value, Detail });
            LvMeasureResult.Dock = System.Windows.Forms.DockStyle.Fill;
            LvMeasureResult.ForeColor = System.Drawing.SystemColors.ControlLight;
            LvMeasureResult.FullRowSelect = true;
            LvMeasureResult.GridLines = true;
            LvMeasureResult.GridLinesColor = System.Drawing.Color.Gray;
            LvMeasureResult.HeaderBackColor = System.Drawing.Color.Gray;
            LvMeasureResult.HeaderForeColor = System.Drawing.Color.White;
            LvMeasureResult.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            LvMeasureResult.HideSelection = true;
            LvMeasureResult.IsIndependentWindow = false;
            LvMeasureResult.Location = new System.Drawing.Point(3, 3);
            LvMeasureResult.MultiSelect = false;
            LvMeasureResult.Name = "LvMeasureResult";
            LvMeasureResult.OwnerDraw = true;
            LvMeasureResult.RowHeight = 23;
            LvMeasureResult.ScrollContainer = null;
            LvMeasureResult.SelectedRowColor = System.Drawing.Color.Blue;
            LvMeasureResult.Size = new System.Drawing.Size(480, 330);
            LvMeasureResult.TabIndex = 7;
            LvMeasureResult.TabStop = false;
            LvMeasureResult.Tag = "";
            LvMeasureResult.UseCompatibleStateImageBehavior = false;
            LvMeasureResult.View = System.Windows.Forms.View.Details;
            // 
            // Parameter
            // 
            Parameter.Width = 150;
            // 
            // Value
            // 
            Value.Width = 100;
            // 
            // Detail
            // 
            Detail.Width = 176;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(LvMeasureResult, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(2, 45);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            tableLayoutPanel1.Size = new System.Drawing.Size(496, 335);
            tableLayoutPanel1.TabIndex = 10;
            // 
            // MeasureResultForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderThickness = 2;
            CanClose = false;
            ClientSize = new System.Drawing.Size(550, 340);
            Controls.Add(tableLayoutPanel1);
            HelpLabel = "27";
            IconWidth = 26;
            IsShowPin = false;
            Name = "MeasureResultForm";
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            ToolIconSize = new System.Drawing.Size(26, 26);
            HelpClick += MeasureResultForm_HelpClick;
            Controls.SetChildIndex(tableLayoutPanel1, 0);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ScopeX.UserControls.ScopeXListViewEx LvMeasureResult;
        private System.Windows.Forms.ColumnHeader Parameter;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.ColumnHeader Detail;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}