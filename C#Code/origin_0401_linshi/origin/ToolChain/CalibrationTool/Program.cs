using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Hardware.Calibration.Tool.Base;
namespace ScopeX.Hardware.Calibration.Tool
{
    static class Program
    {
        internal static MainForm? OurMainForm;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            BaseHelper.InitDomainClass(Assembly.GetExecutingAssembly());
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

            //获得当前登录的Windows用户标示
            WindowsPrincipal principal = new(WindowsIdentity.GetCurrent());
            if (Debugger.IsAttached || principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                //如果是管理员，则直接运行
                BaseHelper.InitSystemInfo();
                OurMainForm = new MainForm();
                Application.Run(OurMainForm);
            }
            else
            {
                //创建启动对象
                ProcessStartInfo startInfo = new();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Application.ExecutablePath;
                //设置启动动作,确保以管理员身份运行
                startInfo.Verb = "runas";
                try
                {
                    Process.Start(startInfo);
                }
                catch
                {
                    return;
                }
                Application.Exit();
            }
            
        }
    }
}
