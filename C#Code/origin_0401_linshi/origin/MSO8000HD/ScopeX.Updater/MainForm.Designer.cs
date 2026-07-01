
namespace WindowsDSO_Updater
{
	partial class MainForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			StatusStripBar = new System.Windows.Forms.StatusStrip();
			toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			toolStripStatusLabelUsedSecond = new System.Windows.Forms.ToolStripStatusLabel();
			Timer1 = new System.Windows.Forms.Timer(components);
			PanelTitle = new System.Windows.Forms.Panel();
			LableTitle = new System.Windows.Forms.Label();
			panelMain = new System.Windows.Forms.Panel();
			labelLastModifyDate = new System.Windows.Forms.Label();
			CB_AbortVer = new System.Windows.Forms.CheckBox();
			panelTime = new System.Windows.Forms.Panel();
			label3 = new System.Windows.Forms.Label();
			labelActualUsedTime = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			labelCalculateNeedUsingTime = new System.Windows.Forms.Label();
			BtnStartUpdate = new System.Windows.Forms.Button();
			progressBarStep = new System.Windows.Forms.ProgressBar();
			BtnClose = new System.Windows.Forms.Button();
			buttonOpenUpdatePackageFile = new System.Windows.Forms.Button();
			textBoxUpdatePackageFileName = new System.Windows.Forms.TextBox();
			label1 = new System.Windows.Forms.Label();
			StatusStripBar.SuspendLayout();
			PanelTitle.SuspendLayout();
			panelMain.SuspendLayout();
			panelTime.SuspendLayout();
			SuspendLayout();
			// 
			// openFileDialog1
			// 
			openFileDialog1.FileName = "openFileDialog1";
			openFileDialog1.Filter = "更新包|*.upd";
			// 
			// StatusStripBar
			// 
			StatusStripBar.ImageScalingSize = new System.Drawing.Size(20, 20);
			StatusStripBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabelUsedSecond });
			StatusStripBar.Location = new System.Drawing.Point(0, 219);
			StatusStripBar.Name = "StatusStripBar";
			StatusStripBar.Size = new System.Drawing.Size(603, 22);
			StatusStripBar.SizingGrip = false;
			StatusStripBar.TabIndex = 8;
			StatusStripBar.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
			// 
			// toolStripStatusLabelUsedSecond
			// 
			toolStripStatusLabelUsedSecond.Name = "toolStripStatusLabelUsedSecond";
			toolStripStatusLabelUsedSecond.Size = new System.Drawing.Size(0, 17);
			// 
			// Timer1
			// 
			Timer1.Interval = 1000;
			Timer1.Tick += Timer1_Tick;
			// 
			// PanelTitle
			// 
			PanelTitle.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			PanelTitle.BackColor = System.Drawing.Color.FromArgb(216, 240, 236);
			PanelTitle.Controls.Add(LableTitle);
			PanelTitle.Location = new System.Drawing.Point(0, 0);
			PanelTitle.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			PanelTitle.Name = "PanelTitle";
			PanelTitle.Size = new System.Drawing.Size(603, 26);
			PanelTitle.TabIndex = 12;
			PanelTitle.MouseDown += PanelTitle_MouseDown;
			PanelTitle.MouseMove += PanelTitle_MouseMove;
			PanelTitle.MouseUp += PanelTitle_MouseUp;
			// 
			// LableTitle
			// 
			LableTitle.AutoSize = true;
			LableTitle.Location = new System.Drawing.Point(6, 4);
			LableTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			LableTitle.Name = "LableTitle";
			LableTitle.Size = new System.Drawing.Size(68, 17);
			LableTitle.TabIndex = 0;
			LableTitle.Text = "示波器更新";
			// 
			// panelMain
			// 
			panelMain.Controls.Add(labelLastModifyDate);
			panelMain.Controls.Add(CB_AbortVer);
			panelMain.Controls.Add(panelTime);
			panelMain.Controls.Add(BtnStartUpdate);
			panelMain.Controls.Add(progressBarStep);
			panelMain.Controls.Add(BtnClose);
			panelMain.Controls.Add(buttonOpenUpdatePackageFile);
			panelMain.Controls.Add(textBoxUpdatePackageFileName);
			panelMain.Controls.Add(label1);
			panelMain.Location = new System.Drawing.Point(0, 24);
			panelMain.Name = "panelMain";
			panelMain.Size = new System.Drawing.Size(603, 192);
			panelMain.TabIndex = 15;
			// 
			// labelLastModifyDate
			// 
			labelLastModifyDate.AutoSize = true;
			labelLastModifyDate.Location = new System.Drawing.Point(265, 158);
			labelLastModifyDate.Name = "labelLastModifyDate";
			labelLastModifyDate.Size = new System.Drawing.Size(67, 17);
			labelLastModifyDate.TabIndex = 23;
			labelLastModifyDate.Text = "2024.0419";
			labelLastModifyDate.Visible = false;
			// 
			// CB_AbortVer
			// 
			CB_AbortVer.AutoSize = true;
			CB_AbortVer.Checked = true;
			CB_AbortVer.CheckState = System.Windows.Forms.CheckState.Checked;
			CB_AbortVer.Location = new System.Drawing.Point(15, 156);
			CB_AbortVer.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			CB_AbortVer.Name = "CB_AbortVer";
			CB_AbortVer.Size = new System.Drawing.Size(159, 21);
			CB_AbortVer.TabIndex = 22;
			CB_AbortVer.Text = "强制更新，跳过版本检查";
			CB_AbortVer.UseVisualStyleBackColor = true;
			CB_AbortVer.Visible = false;
			// 
			// panelTime
			// 
			panelTime.Controls.Add(label3);
			panelTime.Controls.Add(labelActualUsedTime);
			panelTime.Controls.Add(label2);
			panelTime.Controls.Add(labelCalculateNeedUsingTime);
			panelTime.Location = new System.Drawing.Point(15, 103);
			panelTime.Name = "panelTime";
			panelTime.Size = new System.Drawing.Size(579, 28);
			panelTime.TabIndex = 21;
			panelTime.Visible = false;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(387, 6);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(68, 17);
			label3.TabIndex = 11;
			label3.Text = "已用时间：";
			// 
			// labelActualUsedTime
			// 
			labelActualUsedTime.Location = new System.Drawing.Point(479, 6);
			labelActualUsedTime.Name = "labelActualUsedTime";
			labelActualUsedTime.Size = new System.Drawing.Size(87, 17);
			labelActualUsedTime.TabIndex = 12;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(0, 6);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(92, 17);
			label2.TabIndex = 9;
			label2.Text = "预估需用时间：";
			// 
			// labelCalculateNeedUsingTime
			// 
			labelCalculateNeedUsingTime.Location = new System.Drawing.Point(97, 6);
			labelCalculateNeedUsingTime.Name = "labelCalculateNeedUsingTime";
			labelCalculateNeedUsingTime.Size = new System.Drawing.Size(80, 17);
			labelCalculateNeedUsingTime.TabIndex = 10;
			// 
			// BtnStartUpdate
			// 
			BtnStartUpdate.BackColor = System.Drawing.Color.FromArgb(240, 231, 228);
			BtnStartUpdate.Enabled = false;
			BtnStartUpdate.Location = new System.Drawing.Point(406, 148);
			BtnStartUpdate.Name = "BtnStartUpdate";
			BtnStartUpdate.Size = new System.Drawing.Size(68, 36);
			BtnStartUpdate.TabIndex = 20;
			BtnStartUpdate.Text = "开始更新";
			BtnStartUpdate.UseVisualStyleBackColor = false;
			BtnStartUpdate.Click += BtnStartUpdate_Click;
			// 
			// progressBarStep
			// 
			progressBarStep.ForeColor = System.Drawing.Color.FromArgb(224, 240, 216);
			progressBarStep.Location = new System.Drawing.Point(15, 53);
			progressBarStep.Name = "progressBarStep";
			progressBarStep.Size = new System.Drawing.Size(579, 43);
			progressBarStep.TabIndex = 19;
			// 
			// BtnClose
			// 
			BtnClose.BackColor = System.Drawing.Color.FromArgb(240, 231, 228);
			BtnClose.Location = new System.Drawing.Point(519, 148);
			BtnClose.Name = "BtnClose";
			BtnClose.Size = new System.Drawing.Size(75, 36);
			BtnClose.TabIndex = 18;
			BtnClose.Text = "退出";
			BtnClose.UseVisualStyleBackColor = false;
			BtnClose.Click += BtnClose_Click;
			// 
			// buttonOpenUpdatePackageFile
			// 
			buttonOpenUpdatePackageFile.BackColor = System.Drawing.Color.FromArgb(240, 231, 228);
			buttonOpenUpdatePackageFile.Location = new System.Drawing.Point(519, 8);
			buttonOpenUpdatePackageFile.Name = "buttonOpenUpdatePackageFile";
			buttonOpenUpdatePackageFile.Size = new System.Drawing.Size(75, 35);
			buttonOpenUpdatePackageFile.TabIndex = 17;
			buttonOpenUpdatePackageFile.Text = "浏览...";
			buttonOpenUpdatePackageFile.UseVisualStyleBackColor = false;
			buttonOpenUpdatePackageFile.Click += ButtonOpenUpdatePackageFile_Click;
			// 
			// textBoxUpdatePackageFileName
			// 
			textBoxUpdatePackageFileName.BackColor = System.Drawing.Color.White;
			textBoxUpdatePackageFileName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			textBoxUpdatePackageFileName.Location = new System.Drawing.Point(88, 14);
			textBoxUpdatePackageFileName.Name = "textBoxUpdatePackageFileName";
			textBoxUpdatePackageFileName.ReadOnly = true;
			textBoxUpdatePackageFileName.Size = new System.Drawing.Size(426, 23);
			textBoxUpdatePackageFileName.TabIndex = 16;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(8, 17);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(80, 17);
			label1.TabIndex = 15;
			label1.Text = "更新包文件：";
			// 
			// MainForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(603, 241);
			Controls.Add(panelMain);
			Controls.Add(PanelTitle);
			Controls.Add(StatusStripBar);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			Name = "MainForm";
			StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "示波器升级";
			TopMost = true;
			Load += MainForm_Load;
			KeyPress += MainForm_KeyPress;
			StatusStripBar.ResumeLayout(false);
			StatusStripBar.PerformLayout();
			PanelTitle.ResumeLayout(false);
			PanelTitle.PerformLayout();
			panelMain.ResumeLayout(false);
			panelMain.PerformLayout();
			panelTime.ResumeLayout(false);
			panelTime.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.StatusStrip StatusStripBar;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelUsedSecond;
		private System.Windows.Forms.Timer Timer1;
		private System.Windows.Forms.Panel PanelTitle;
		private System.Windows.Forms.Label LableTitle;
		private System.Windows.Forms.Panel panelMain;
		private System.Windows.Forms.Label labelLastModifyDate;
		private System.Windows.Forms.CheckBox CB_AbortVer;
		private System.Windows.Forms.Panel panelTime;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labelActualUsedTime;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelCalculateNeedUsingTime;
		private System.Windows.Forms.Button BtnStartUpdate;
		private System.Windows.Forms.ProgressBar progressBarStep;
		private System.Windows.Forms.Button BtnClose;
		private System.Windows.Forms.Button buttonOpenUpdatePackageFile;
		private System.Windows.Forms.TextBox textBoxUpdatePackageFileName;
		private System.Windows.Forms.Label label1;
	}
}

