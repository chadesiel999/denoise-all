namespace ScopeX.Updater
{
    partial class FullScreenForm
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
            progressBar = new System.Windows.Forms.ProgressBar();
            lb_UsingTime_des = new System.Windows.Forms.Label();
            lb_EstTime_Des = new System.Windows.Forms.Label();
            lb_EstTime = new System.Windows.Forms.Label();
            panelTime = new System.Windows.Forms.FlowLayoutPanel();
            lb_UsingTime = new System.Windows.Forms.Label();
            lb_Title = new System.Windows.Forms.Label();
            StatusStripBar = new System.Windows.Forms.StatusStrip();
            Timer1 = new System.Windows.Forms.Timer(components);
            button1 = new System.Windows.Forms.Button();
            MSG_panle = new System.Windows.Forms.FlowLayoutPanel();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            groupBox1 = new System.Windows.Forms.GroupBox();
            panelTime.SuspendLayout();
            MSG_panle.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            progressBar.Location = new System.Drawing.Point(12, 275);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(1763, 50);
            progressBar.TabIndex = 0;
            progressBar.Value = 50;
            // 
            // lb_UsingTime_des
            // 
            lb_UsingTime_des.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lb_UsingTime_des.AutoSize = true;
            lb_UsingTime_des.Location = new System.Drawing.Point(830, 0);
            lb_UsingTime_des.Margin = new System.Windows.Forms.Padding(700, 0, 4, 0);
            lb_UsingTime_des.Name = "lb_UsingTime_des";
            lb_UsingTime_des.Size = new System.Drawing.Size(84, 20);
            lb_UsingTime_des.TabIndex = 13;
            lb_UsingTime_des.Text = "已用时间：";
            // 
            // lb_EstTime_Des
            // 
            lb_EstTime_Des.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lb_EstTime_Des.AutoSize = true;
            lb_EstTime_Des.Location = new System.Drawing.Point(4, 0);
            lb_EstTime_Des.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lb_EstTime_Des.Name = "lb_EstTime_Des";
            lb_EstTime_Des.Size = new System.Drawing.Size(114, 20);
            lb_EstTime_Des.TabIndex = 12;
            lb_EstTime_Des.Text = "预估需用时间：";
            // 
            // lb_EstTime
            // 
            lb_EstTime.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lb_EstTime.AutoSize = true;
            lb_EstTime.Location = new System.Drawing.Point(126, 0);
            lb_EstTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lb_EstTime.Name = "lb_EstTime";
            lb_EstTime.Size = new System.Drawing.Size(0, 20);
            lb_EstTime.TabIndex = 12;
            // 
            // panelTime
            // 
            panelTime.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panelTime.Controls.Add(lb_EstTime_Des);
            panelTime.Controls.Add(lb_EstTime);
            panelTime.Controls.Add(lb_UsingTime_des);
            panelTime.Controls.Add(lb_UsingTime);
            panelTime.Location = new System.Drawing.Point(12, 331);
            panelTime.Name = "panelTime";
            panelTime.Size = new System.Drawing.Size(1763, 28);
            panelTime.TabIndex = 14;
            // 
            // lb_UsingTime
            // 
            lb_UsingTime.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lb_UsingTime.AutoSize = true;
            lb_UsingTime.Location = new System.Drawing.Point(922, 0);
            lb_UsingTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lb_UsingTime.Name = "lb_UsingTime";
            lb_UsingTime.Size = new System.Drawing.Size(0, 20);
            lb_UsingTime.TabIndex = 12;
            // 
            // lb_Title
            // 
            lb_Title.AutoSize = true;
            lb_Title.Location = new System.Drawing.Point(12, 9);
            lb_Title.Margin = new System.Windows.Forms.Padding(700, 0, 4, 0);
            lb_Title.Name = "lb_Title";
            lb_Title.Size = new System.Drawing.Size(0, 20);
            lb_Title.TabIndex = 13;
            // 
            // StatusStripBar
            // 
            StatusStripBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            StatusStripBar.Location = new System.Drawing.Point(0, 646);
            StatusStripBar.Name = "StatusStripBar";
            StatusStripBar.Size = new System.Drawing.Size(1787, 22);
            StatusStripBar.TabIndex = 16;
            StatusStripBar.Text = "statusStrip1";
            // 
            // Timer1
            // 
            Timer1.Tick += Timer1_Tick;
            // 
            // button1
            // 
            button1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            button1.Location = new System.Drawing.Point(1737, 0);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(50, 29);
            button1.TabIndex = 17;
            button1.Text = "X";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // MSG_panle
            // 
            MSG_panle.AutoScroll = true;
            MSG_panle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            MSG_panle.Controls.Add(flowLayoutPanel1);
            MSG_panle.Dock = System.Windows.Forms.DockStyle.Fill;
            MSG_panle.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            MSG_panle.Location = new System.Drawing.Point(3, 23);
            MSG_panle.Margin = new System.Windows.Forms.Padding(0);
            MSG_panle.Name = "MSG_panle";
            MSG_panle.Size = new System.Drawing.Size(901, 252);
            MSG_panle.TabIndex = 18;
            MSG_panle.WrapContents = false;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.AutoScrollMargin = new System.Drawing.Size(10, 10);
            flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(0, 159);
            flowLayoutPanel1.TabIndex = 18;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.Controls.Add(MSG_panle);
            groupBox1.Location = new System.Drawing.Point(868, 378);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(907, 278);
            groupBox1.TabIndex = 19;
            groupBox1.TabStop = false;
            groupBox1.Text = "消息：";
            groupBox1.Visible = false;
            // 
            // FullScreenForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1787, 668);
            ControlBox = false;
            Controls.Add(groupBox1);
            Controls.Add(button1);
            Controls.Add(StatusStripBar);
            Controls.Add(panelTime);
            Controls.Add(lb_Title);
            Controls.Add(progressBar);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            ImeMode = System.Windows.Forms.ImeMode.Disable;
            Name = "FullScreenForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "FullScreenForm";
            TopMost = true;
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Load += FullScreenForm_Load;
            Shown += FullScreenForm_Shown;
            panelTime.ResumeLayout(false);
            panelTime.PerformLayout();
            MSG_panle.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lb_UsingTime_des;
        private System.Windows.Forms.Label lb_EstTime_Des;
        private System.Windows.Forms.Label lb_EstTime;
        private System.Windows.Forms.FlowLayoutPanel panelTime;
        private System.Windows.Forms.Label lb_UsingTime;
        private System.Windows.Forms.Label lb_Title;
        private System.Windows.Forms.StatusStrip StatusStripBar;
        private System.Windows.Forms.Timer Timer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.FlowLayoutPanel MSG_panle;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}