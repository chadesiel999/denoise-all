using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.U2
{
    partial class FileBrowserForm
    {
        // <summary>
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
            SuspendLayout();
            // 
            // TbFolderPath
            // 
            TbFolderPath.BackColor = System.Drawing.Color.FromArgb(61, 62, 69);
            TbFolderPath.BorderColor = System.Drawing.Color.FromArgb(67, 69, 76);
            TbFolderPath.BorderThickness = 1;
            TbFolderPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbFolderPath.ForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            TbFolderPath.SelectedColor = System.Drawing.Color.FromArgb(40, 71, 193);
            TbFolderPath.TabIndex = 1;
            // 
            // DirectoryPage
            // 
            DirectoryPage.BackColor = System.Drawing.Color.FromArgb(41, 43, 50);
            DirectoryPage.TabIndex = 3;
            // 
            // BtnCancel
            // 
            BtnCancel.BackColor = System.Drawing.Color.FromArgb(61, 62, 69);
            BtnCancel.BorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            BtnCancel.BorderThickness = 0;
            BtnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnCancel.ForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnCancel.Height = 30;
            BtnCancel.MouseinBackColor = System.Drawing.Color.FromArgb(54, 54, 54);
            BtnCancel.MouseinBorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            BtnCancel.MouseInBorderThickness = 0;
            BtnCancel.MouseinForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnCancel.MouseinSvgForeColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCancel.PressedBorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            BtnCancel.PressedBorderThickness = 0;
            BtnCancel.PressedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnCancel.PressedSvgForeColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnCancel.Size = new System.Drawing.Size(83, 30);
            BtnCancel.SVGForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnCancel.TabIndex = 9;
            // 
            // BtnYes
            // 
            BtnYes.BackColor = System.Drawing.Color.FromArgb(61, 62, 69);
            BtnYes.BorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            BtnYes.BorderThickness = 0;
            BtnYes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            BtnYes.ForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnYes.Height = 30;
            BtnYes.MouseinBackColor = System.Drawing.Color.FromArgb(54, 54, 54);
            BtnYes.MouseinBorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            BtnYes.MouseInBorderThickness = 0;
            BtnYes.MouseinForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnYes.MouseinSvgForeColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnYes.PressedBorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            BtnYes.PressedBorderThickness = 0;
            BtnYes.PressedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnYes.PressedSvgForeColor = System.Drawing.Color.FromArgb(40, 71, 193);
            BtnYes.Size = new System.Drawing.Size(84, 30);
            BtnYes.SVGForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            BtnYes.TabIndex = 8;
            // 
            // TbChoosedFolderName
            // 
            TbChoosedFolderName.BackColor = System.Drawing.Color.FromArgb(61, 62, 69);
            TbChoosedFolderName.BorderColor = System.Drawing.Color.FromArgb(61, 62, 69);
            TbChoosedFolderName.BorderThickness = 1;
            TbChoosedFolderName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TbChoosedFolderName.ForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            TbChoosedFolderName.Height = 30;
            TbChoosedFolderName.SelectedColor = System.Drawing.Color.FromArgb(40, 71, 193);
            TbChoosedFolderName.Size = new System.Drawing.Size(580, 30);
            TbChoosedFolderName.TabIndex = 6;
            // 
            // LblChoosedFile
            // 
            LblChoosedFile.BackColor = System.Drawing.Color.FromArgb(42, 42, 42);
            LblChoosedFile.BorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            LblChoosedFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblChoosedFile.ForeColor = System.Drawing.Color.FromArgb(234, 234, 234);
            LblChoosedFile.Size = new System.Drawing.Size(98, 30);
            LblChoosedFile.TabIndex = 5;
            // 
            // CmbFileType
            // 
            CmbFileType.BackColor = System.Drawing.Color.FromArgb(61, 62, 69);
            CmbFileType.BorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            CmbFileType.FocusColor = System.Drawing.Color.FromArgb(54, 54, 54);
            CmbFileType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CmbFileType.ForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            CmbFileType.Height = 30;
            CmbFileType.ItemHeight = 35;
            CmbFileType.SelectedBackColor = System.Drawing.Color.FromArgb(40, 71, 193);
            CmbFileType.Size = new System.Drawing.Size(178, 30);
            CmbFileType.TabIndex = 7;
            // 
            // FilePage
            // 
            FilePage.BackColor = System.Drawing.Color.FromArgb(41, 43, 50);
            FilePage.TabIndex = 4;
            // 
            // ScopeXIconButton1
            // 
            ScopeXIconButton1.BackColor = System.Drawing.Color.FromArgb(42, 42, 42);
            ScopeXIconButton1.BorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            ScopeXIconButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ScopeXIconButton1.ForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            ScopeXIconButton1.MouseinBackColor = System.Drawing.Color.FromArgb(54, 54, 54);
            ScopeXIconButton1.MouseinBorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            ScopeXIconButton1.MouseInBorderThickness = 0;
            ScopeXIconButton1.MouseinForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            ScopeXIconButton1.MouseinSvgForeColor = System.Drawing.Color.FromArgb(40, 71, 193);
            ScopeXIconButton1.PressedBorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            ScopeXIconButton1.PressedBorderThickness = 0;
            ScopeXIconButton1.PressedForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            ScopeXIconButton1.PressedSvgForeColor = System.Drawing.Color.FromArgb(40, 71, 193);
            ScopeXIconButton1.SVGForeColor = System.Drawing.Color.FromArgb(192, 192, 192);
            ScopeXIconButton1.TabIndex = 2;
            // 
            // LblFolderPath
            // 
            LblFolderPath.BackColor = System.Drawing.Color.FromArgb(42, 42, 42);
            LblFolderPath.BorderColor = System.Drawing.Color.FromArgb(54, 54, 54);
            LblFolderPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            LblFolderPath.ForeColor = System.Drawing.Color.FromArgb(234, 234, 234);
            LblFolderPath.Location = new System.Drawing.Point(12, 55);
            LblFolderPath.Size = new System.Drawing.Size(84, 30);
            LblFolderPath.TabIndex = 0;
            // 
            // FileBrowserForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            BorderThickness = 2;
            CanEditFileName = true;
            ClientSize = new System.Drawing.Size(897, 605);
            ContentBackColor = System.Drawing.Color.FromArgb(42, 42, 42);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            HeadBackColor = System.Drawing.Color.FromArgb(50, 55, 65);
            HeadHeight = 45;
            IconInterval = 21;
            IconSideDistance = 10;
            IconWidth = 26;
            KeyPreview = true;
            Name = "FileBrowserForm";
            TitleColor = System.Drawing.Color.FromArgb(234, 234, 234);
            TitleFont = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TitleLableHeight = 39;
            ToolIconInterval = 21;
            ToolIconSize = new System.Drawing.Size(26, 26);
            ResumeLayout(false);
        }

        #endregion
    }
}
