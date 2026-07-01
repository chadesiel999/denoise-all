using System;
using System.IO;
using System.Windows.Forms;

namespace ScopeX.Updater
{
	public partial class ErrorInfoCtrl : UserControl
	{
		private string fileFullPath;
		public ErrorInfoCtrl(string msg, string? logPath)
		{
			InitializeComponent();
			LbMsg.Text = $"{msg}";
			if (logPath != null && string.IsNullOrWhiteSpace(logPath) || !File.Exists(logPath))
			{
				BtnReadInfo.Visible = false;
				LbFilePath.Text = "";
			}
			LbMsg.Text = $"{msg},日志已导出:";
			LbFilePath.Text = logPath;
			fileFullPath = logPath;
		}

		private void BtnClose_Click(object sender, EventArgs e)
		{
			Environment.Exit(0);
		}

		private void BtnReadInfo_Click(object sender, EventArgs e)
		{
			System.Diagnostics.ProcessStartInfo psi = new
			System.Diagnostics.ProcessStartInfo("Explorer.exe");
			psi.Arguments = "/e,/select," + fileFullPath; System.Diagnostics.Process.Start(psi);
		}
	}
}
