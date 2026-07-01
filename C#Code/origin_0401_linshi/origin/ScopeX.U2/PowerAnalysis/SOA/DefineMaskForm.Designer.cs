
using System.Windows.Forms;

namespace ScopeX.U2
{
    partial class DefineMaskForm
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            dataGridView1 = new DataGridView();
            BtnAdd = new UserControls.ScopeXIconButton();
            BtnDelete = new UserControls.ScopeXIconButton();
            BtnSave = new UserControls.ScopeXIconButton();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(0, 171, 209);
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            dataGridView1.Location = new System.Drawing.Point(12, 51);
            dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dataGridView1.RowHeadersWidth = 60;
            dataGridView1.Size = new System.Drawing.Size(280, 278);
            dataGridView1.TabIndex = 5;
            dataGridView1.CellClick += DataGridView1_CellClick;
            dataGridView1.RowStateChanged += dataGridView1_RowStateChanged;
            // 
            // BtnAdd
            // 
            BtnAdd.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAdd.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAdd.BorderThickness = 0;
            BtnAdd.CornerRadius = 0;
            BtnAdd.Cursor = Cursors.Hand;
            BtnAdd.DaskArray = null;
            BtnAdd.DropKey = Keys.Space;
            BtnAdd.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnAdd.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAdd.Height = 30;
            BtnAdd.Icon = null;
            BtnAdd.IconOffset = 10;
            BtnAdd.IconSize = new System.Drawing.Size(24, 24);
            BtnAdd.ImeMode = ImeMode.NoControl;
            BtnAdd.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnAdd.IsIndicatorShow = false;
            BtnAdd.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnAdd.Location = new System.Drawing.Point(311, 76);
            BtnAdd.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAdd.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAdd.MouseInBorderThickness = 0;
            BtnAdd.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAdd.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.PressedBackColor = System.Drawing.Color.Gray;
            BtnAdd.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnAdd.PressedBorderThickness = 0;
            BtnAdd.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAdd.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnAdd.Size = new System.Drawing.Size(100, 30);
            BtnAdd.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnAdd.StylizeFlag = true;
            BtnAdd.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnAdd.SVGPath = "";
            BtnAdd.TabIndex = 53;
            BtnAdd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnAdd.Click += BtnAdd_Click;
            // 
            // BtnDelete
            // 
            BtnDelete.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnDelete.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnDelete.BorderThickness = 0;
            BtnDelete.CornerRadius = 0;
            BtnDelete.Cursor = Cursors.Hand;
            BtnDelete.DaskArray = null;
            BtnDelete.DropKey = Keys.Space;
            BtnDelete.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnDelete.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnDelete.Height = 30;
            BtnDelete.Icon = null;
            BtnDelete.IconOffset = 10;
            BtnDelete.IconSize = new System.Drawing.Size(24, 24);
            BtnDelete.ImeMode = ImeMode.NoControl;
            BtnDelete.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnDelete.IsIndicatorShow = false;
            BtnDelete.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnDelete.Location = new System.Drawing.Point(311, 131);
            BtnDelete.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnDelete.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnDelete.MouseInBorderThickness = 0;
            BtnDelete.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnDelete.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnDelete.Name = "BtnDelete";
            BtnDelete.PressedBackColor = System.Drawing.Color.Gray;
            BtnDelete.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnDelete.PressedBorderThickness = 0;
            BtnDelete.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnDelete.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnDelete.Size = new System.Drawing.Size(100, 30);
            BtnDelete.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnDelete.StylizeFlag = true;
            BtnDelete.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnDelete.SVGPath = "";
            BtnDelete.TabIndex = 54;
            BtnDelete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnDelete.Click += BtnDelete_Click;
            // 
            // BtnSave
            // 
            BtnSave.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSave.BorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSave.BorderThickness = 0;
            BtnSave.CornerRadius = 0;
            BtnSave.Cursor = Cursors.Hand;
            BtnSave.DaskArray = null;
            BtnSave.DropKey = Keys.Space;
            BtnSave.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnSave.ForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSave.Height = 30;
            BtnSave.Icon = null;
            BtnSave.IconOffset = 10;
            BtnSave.IconSize = new System.Drawing.Size(24, 24);
            BtnSave.ImeMode = ImeMode.NoControl;
            BtnSave.IndicatorColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnSave.IsIndicatorShow = false;
            BtnSave.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            BtnSave.Location = new System.Drawing.Point(311, 186);
            BtnSave.MouseinBackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSave.MouseinBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSave.MouseInBorderThickness = 0;
            BtnSave.MouseinForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSave.MouseinSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSave.Name = "BtnSave";
            BtnSave.PressedBackColor = System.Drawing.Color.Gray;
            BtnSave.PressedBorderColor = System.Drawing.Color.FromArgb(53, 54, 58);
            BtnSave.PressedBorderThickness = 0;
            BtnSave.PressedForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSave.PressedSvgForeColor = System.Drawing.Color.FromArgb(0, 157, 255);
            BtnSave.Size = new System.Drawing.Size(100, 30);
            BtnSave.StyleFlags = UserControls.Style.StyleFlag.None;
            BtnSave.StylizeFlag = true;
            BtnSave.SVGForeColor = System.Drawing.Color.FromArgb(185, 192, 199);
            BtnSave.SVGPath = "";
            BtnSave.TabIndex = 55;
            BtnSave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            BtnSave.Click += BtnSave_Click;
            // 
            // DefineMaskForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CanClose = false;
            ClientSize = new System.Drawing.Size(439, 339);
            ContentBackColor = System.Drawing.Color.FromArgb(41, 42, 45);
            ControlBox = false;
            Controls.Add(BtnSave);
            Controls.Add(BtnDelete);
            Controls.Add(BtnAdd);
            Controls.Add(dataGridView1);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormOpacity = 95;
            IconWidth = 26;
            IsShowPin = false;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DefineMaskForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            TitleColor = System.Drawing.Color.FromArgb(232, 234, 237);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleIcon = Properties.Resources.MeasureSetting;
            TitleIconSize = new System.Drawing.Size(26, 26);
            TitleIconSizeMode = PictureBoxSizeMode.Zoom;
            TitleIconWidth = 26;
            ToolIconSize = new System.Drawing.Size(26, 26);
            TitleIconClick += DefineMaskForm_SettingClick;
            Controls.SetChildIndex(dataGridView1, 0);
            Controls.SetChildIndex(BtnAdd, 0);
            Controls.SetChildIndex(BtnDelete, 0);
            Controls.SetChildIndex(BtnSave, 0);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private UserControls.ScopeXIconButton BtnGuide;
        private UserControls.ScopeXIconButton scopexIconButton1;
        private UserControls.ScopeXIconButton scopexIconButton2;
        private UserControls.ScopeXIconButton BtnAdd;
        private UserControls.ScopeXIconButton BtnDelete;
        private UserControls.ScopeXIconButton BtnSave;
    }
}