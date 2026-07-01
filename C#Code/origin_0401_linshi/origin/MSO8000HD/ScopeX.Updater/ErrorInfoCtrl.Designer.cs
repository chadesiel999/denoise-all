namespace ScopeX.Updater
{
	partial class ErrorInfoCtrl
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
			panel1 = new System.Windows.Forms.Panel();
			LbFace = new System.Windows.Forms.Label();
			panelMsg = new System.Windows.Forms.Panel();
			BtnReadInfo = new System.Windows.Forms.Button();
			LbFilePath = new System.Windows.Forms.Label();
			LbMsg = new System.Windows.Forms.Label();
			BtnClose = new System.Windows.Forms.Button();
			panel1.SuspendLayout();
			panelMsg.SuspendLayout();
			SuspendLayout();
			// 
			// panel1
			// 
			panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
			panel1.Controls.Add(LbFace);
			panel1.Location = new System.Drawing.Point(1, -1);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(110, 193);
			panel1.TabIndex = 16;
			// 
			// LbFace
			// 
			LbFace.AutoSize = true;
			LbFace.Font = new System.Drawing.Font("微软雅黑", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			LbFace.Location = new System.Drawing.Point(25, 60);
			LbFace.Name = "LbFace";
			LbFace.Size = new System.Drawing.Size(60, 64);
			LbFace.TabIndex = 16;
			LbFace.Text = ":(";
			// 
			// panelMsg
			// 
			panelMsg.Controls.Add(BtnReadInfo);
			panelMsg.Controls.Add(LbFilePath);
			panelMsg.Controls.Add(LbMsg);
			panelMsg.Controls.Add(panel1);
			panelMsg.Controls.Add(BtnClose);
			panelMsg.Dock = System.Windows.Forms.DockStyle.Fill;
			panelMsg.Location = new System.Drawing.Point(0, 0);
			panelMsg.Name = "panelMsg";
			panelMsg.Size = new System.Drawing.Size(603, 192);
			panelMsg.TabIndex = 1;
			// 
			// BtnReadInfo
			// 
			BtnReadInfo.BackColor = System.Drawing.Color.FromArgb(240, 231, 228);
			BtnReadInfo.Location = new System.Drawing.Point(215, 140);
			BtnReadInfo.Name = "BtnReadInfo";
			BtnReadInfo.Size = new System.Drawing.Size(75, 36);
			BtnReadInfo.TabIndex = 19;
			BtnReadInfo.Text = "日志文件";
			BtnReadInfo.UseVisualStyleBackColor = false;
			BtnReadInfo.Click += BtnReadInfo_Click;
			// 
			// LbFilePath
			// 
			LbFilePath.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			LbFilePath.Location = new System.Drawing.Point(127, 60);
			LbFilePath.Name = "LbFilePath";
			LbFilePath.Size = new System.Drawing.Size(455, 78);
			LbFilePath.TabIndex = 18;
			LbFilePath.Text = "FilePath";
			// 
			// LbMsg
			// 
			LbMsg.AutoSize = true;
			LbMsg.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			LbMsg.Location = new System.Drawing.Point(127, 29);
			LbMsg.Name = "LbMsg";
			LbMsg.Size = new System.Drawing.Size(163, 20);
			LbMsg.TabIndex = 17;
			LbMsg.Text = "更新出错，日志已导出：";
			// 
			// BtnClose
			// 
			BtnClose.BackColor = System.Drawing.Color.FromArgb(240, 231, 228);
			BtnClose.Location = new System.Drawing.Point(348, 140);
			BtnClose.Name = "BtnClose";
			BtnClose.Size = new System.Drawing.Size(75, 36);
			BtnClose.TabIndex = 14;
			BtnClose.Text = "退出";
			BtnClose.UseVisualStyleBackColor = false;
			BtnClose.Click += BtnClose_Click;
			// 
			// ErrorInfoCtrl
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			Controls.Add(panelMsg);
			Name = "ErrorInfoCtrl";
			Size = new System.Drawing.Size(603, 192);
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			panelMsg.ResumeLayout(false);
			panelMsg.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label LbFace;
		private System.Windows.Forms.Panel panelMsg;
		private System.Windows.Forms.Label LbFilePath;
		private System.Windows.Forms.Label LbMsg;
		private System.Windows.Forms.Button BtnClose;
		private System.Windows.Forms.Button BtnReadInfo;
	}
}
