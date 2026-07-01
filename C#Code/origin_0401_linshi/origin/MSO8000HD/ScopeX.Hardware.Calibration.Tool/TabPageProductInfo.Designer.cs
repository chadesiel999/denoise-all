
namespace ScopeX.Hardware.Calibration.Tool
{
    partial class TabPageProductInfo
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
            this.textBoxModel = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxManufacturer = new System.Windows.Forms.TextBox();
            this.labelSN = new System.Windows.Forms.Label();
            this.textBoxSN = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxFunCode = new System.Windows.Forms.TextBox();
            this.buttonRead = new System.Windows.Forms.Button();
            this.buttonWrite = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxModel
            // 
            this.textBoxModel.Location = new System.Drawing.Point(124, 24);
            this.textBoxModel.Name = "textBoxModel";
            this.textBoxModel.Size = new System.Drawing.Size(155, 23);
            this.textBoxModel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(69, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Model:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Manufacturer:";
            // 
            // textBoxManufacturer
            // 
            this.textBoxManufacturer.Location = new System.Drawing.Point(124, 53);
            this.textBoxManufacturer.Name = "textBoxManufacturer";
            this.textBoxManufacturer.Size = new System.Drawing.Size(155, 23);
            this.textBoxManufacturer.TabIndex = 2;
            // 
            // labelSN
            // 
            this.labelSN.AutoSize = true;
            this.labelSN.Location = new System.Drawing.Point(90, 85);
            this.labelSN.Name = "labelSN";
            this.labelSN.Size = new System.Drawing.Size(28, 17);
            this.labelSN.TabIndex = 5;
            this.labelSN.Text = "SN:";
            // 
            // textBoxSN
            // 
            this.textBoxSN.Location = new System.Drawing.Point(124, 82);
            this.textBoxSN.Name = "textBoxSN";
            this.textBoxSN.Size = new System.Drawing.Size(155, 23);
            this.textBoxSN.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(50, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "FuncCode:";
            // 
            // textBoxFunCode
            // 
            this.textBoxFunCode.Location = new System.Drawing.Point(124, 111);
            this.textBoxFunCode.Name = "textBoxFunCode";
            this.textBoxFunCode.Size = new System.Drawing.Size(155, 23);
            this.textBoxFunCode.TabIndex = 6;
            // 
            // buttonRead
            // 
            this.buttonRead.Location = new System.Drawing.Point(50, 184);
            this.buttonRead.Name = "buttonRead";
            this.buttonRead.Size = new System.Drawing.Size(75, 23);
            this.buttonRead.TabIndex = 8;
            this.buttonRead.Text = "读取";
            this.buttonRead.UseVisualStyleBackColor = true;
            this.buttonRead.Click += new System.EventHandler(this.buttonRead_Click);
            // 
            // buttonWrite
            // 
            this.buttonWrite.Location = new System.Drawing.Point(206, 184);
            this.buttonWrite.Name = "buttonWrite";
            this.buttonWrite.Size = new System.Drawing.Size(75, 23);
            this.buttonWrite.TabIndex = 9;
            this.buttonWrite.Text = "烧写";
            this.buttonWrite.UseVisualStyleBackColor = true;
            this.buttonWrite.Click += new System.EventHandler(this.buttonWrite_Click);
            // 
            // TabPageProductInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonWrite);
            this.Controls.Add(this.buttonRead);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxFunCode);
            this.Controls.Add(this.labelSN);
            this.Controls.Add(this.textBoxSN);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxManufacturer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxModel);
            this.Name = "TabPageProductInfo";
            this.Size = new System.Drawing.Size(416, 256);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxModel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxManufacturer;
        private System.Windows.Forms.Label labelSN;
        private System.Windows.Forms.TextBox textBoxSN;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxFunCode;
        private System.Windows.Forms.Button buttonRead;
        private System.Windows.Forms.Button buttonWrite;
    }
}
