using System;
using System.Runtime.InteropServices;


namespace ScopeX.Hardware.Calibration.Tool
{
    public class ImportOutFuncs
    {
		#region //Windows API
		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);//
		[DllImport("user32.dll")]
		public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool BRePaint);

		public const int GWL_STYLE = -16;//窗口样式
								  //窗口风格
		public const int WS_CAPTION = 0x00C00000;//创建一个有标题框的窗口（包括WS_BORDER风格）
		public const int WS_THICKFRAME = 0x00040000;//创建一个具有厚边框的窗口，可以通过厚边框来改变窗口大小
		public const int WS_SYSMENU = 0X00080000;//创建一个在标题条上带有窗口菜单的窗口，必须同时设定WS_CAPTION风格
		[DllImport("user32")]
		public static extern int GetWindowLong(System.IntPtr hwnd, int nIndex);

		[DllImport("user32")]
		public static extern int SetWindowLong(System.IntPtr hwnd, int index, int newLong);

		#endregion
	}
}
