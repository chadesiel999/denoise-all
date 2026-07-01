using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ScopeX.U2.BaseControl
{
    partial class SelectComboxGroup
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
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanel1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(1);
            flowLayoutPanel1.MaximumSize = new System.Drawing.Size(361, 30);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(361, 30);
            flowLayoutPanel1.TabIndex = 0;
            flowLayoutPanel1.Paint += btn_OnPaint;
            // 
            // SelectComboxGroup
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(53, 54, 58);
            Controls.Add(flowLayoutPanel1);
            DoubleBuffered = true;
            Margin = new System.Windows.Forms.Padding(0);
            MaximumSize = new System.Drawing.Size(361, 30);
            Name = "SelectComboxGroup";
            Size = new System.Drawing.Size(361, 30);
            Tag = "";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}
