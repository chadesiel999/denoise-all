using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ScopeX.Controls.Language;
using ScopeX.Updater;
using ScopeX.Updater.Core;
using static System.Boolean;

namespace WindowsDSO_Updater;

static internal class Program
{
    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        using (new Semaphore(0, 1, "U2OscilloscopeApp", out bool creatednew))
        {
            if (creatednew)
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Application.ThreadException += Application_ThreadException;
                LanguageManger.Instance.Language = LanguageFactory.GetLanguage(Language.简体中文);
                if (!DosModeCheck())
                    return;

                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                bool isfullscreen = false; // 默认使用旧版本
                try
                {
                    string isfullStr = ConfigurationManager.AppSettings["IsFullScreen"];
                    TryParse(isfullStr, out isfullscreen);
                }
                catch (Exception ex)
                {
                    // ignored
                }

                if (isfullscreen)
                {
                    Application.Run(new FullScreenForm());
                }
                else
                {
                    Application.Run(new MainForm());
                }
            }
            else
            {
                MessageBox.Show(@"请先关闭示波器主程序或原有的Update程序！");
            }
        }
    }

    /// <summary>
    ///     DOS模式检查，用以判断是否需要执行静默安装、参数化安装。
    /// </summary>
    /// <returns>是否继续运行</returns>
    static bool DosModeCheck()
    {
        string[] commandArgs = Environment.GetCommandLineArgs();

        List<string> updfiles = commandArgs.Where(c => c.Trim().EndsWith(".upd", StringComparison.OrdinalIgnoreCase)).ToList();
        if (updfiles.Any())
            CommandLineMode.Instance.UpdFilePaths = updfiles;

        string help = commandArgs.FirstOrDefault(c =>
        {
            string temp = c.Replace("-", "").Replace("/", "");
            return temp.Equals("h", StringComparison.OrdinalIgnoreCase) || temp.Equals("help", StringComparison.OrdinalIgnoreCase);
        });

        if (help != null)
        {
            MessageBox.Show(@"优利德固件升级工具

ScopeX.Updater.exe [/h | /help | -h | -help
		/q | /quiet | -q | -quiet | /dl | -disableLed]

命令示例：
    强制禁用部分型号外面版LED状态功能
    ScopeX.Updater.exe /dl 
	静默安装upd固件包：
	ScopeX.Updater.exe /q xxx.upd
	查看命令说明：
	ScopeX.Updater.exe /h
	直接启动界面模式
	ScopeX.Updater.exe ");
            Environment.Exit(0);
            return false;
        }

        string language = commandArgs.FirstOrDefault(c =>
        {
            string temp = c.Replace("-", "").Replace("/", "");
            return temp.StartsWith("l=", StringComparison.OrdinalIgnoreCase) || temp.StartsWith("lang=", StringComparison.OrdinalIgnoreCase);
        });

        if (!string.IsNullOrEmpty(language))
        {
            string lang = language.Replace("-", "").Replace("/", "").Replace("l=", "").Replace("lang=", "").ToLower();
            Language lan = Language.English;
            switch (lang)
            {
                case "zh_cn":
                    lan = Language.简体中文;
                    break;
                case "en":
                default:
                    lan = Language.English;
                    break;
            }
            LanguageManger.Instance.Language = LanguageFactory.GetLanguage(lan);
        }

        string isquitmode = commandArgs.FirstOrDefault(c =>
        {
            string temp = c.Replace("-", "").Replace("/", "");
            return temp.Equals("q", StringComparison.OrdinalIgnoreCase) || temp.Equals("quiet", StringComparison.OrdinalIgnoreCase);
        });
        string isForceDisableOuterLed = commandArgs.FirstOrDefault(c =>
        {
            string temp = c.Replace("-", "").Replace("/", "");
            return temp.Equals("dl", StringComparison.OrdinalIgnoreCase) || temp.Equals("disableLed", StringComparison.OrdinalIgnoreCase);
        });

        if (isquitmode != null)
            CommandLineMode.Instance.IsQuiet = true;
        
        if (isForceDisableOuterLed != null)
            CommandLineMode.Instance.IsForceDisableOuterLed = true;

        if (!CommandLineMode.Instance.IsQuiet)
            return true;

        var result = CommandLineMode.Instance.Install();
        Environment.Exit(result ? 0 : 999);
        return false;
    }

    static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        string msg = "主线程未处理异常：";
        msg += e.Exception?.ToString() ?? "未知";
        LogHelper.WriteLog(msg);
        MessageBox.Show(msg);
        Environment.Exit(999);
    }

    static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        string msg = @"程序域未处理异常：";
        if (e.ExceptionObject is Exception ex)
            msg += ex.ToString();

        LogHelper.WriteLog(msg);
        MessageBox.Show(msg);
        Environment.Exit(999);
    }
}
