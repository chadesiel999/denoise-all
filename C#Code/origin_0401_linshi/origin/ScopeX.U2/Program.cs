global using ScopeX.U2.LanguageSupoort;
using EventBus;
using Microsoft.Win32;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Hardware;
using ScopeX.Core.Tools;
using ScopeX.U2.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;

namespace ScopeX.U2
{
    internal static class Program
    {
        public static DsoPrsnt Oscilloscope;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Try to create a kernel object with the specified name
            using (new Semaphore(0, 1, "U2OscilloscopeApp", out var creatednew))
            {
                if (creatednew)
                {
                    System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                    Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    // 启用捕获未处理异常的模式，使得程序尽量不要崩溃。
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                    Application.SetHighDpiMode(HighDpiMode.SystemAware);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    Application.ThreadException += Application_ThreadException;
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                    //SystemEvents.SessionEnding += new SessionEndingEventHandler(formShutdown.SystemEvents_SessionEnding);
                    //SystemEvents.SessionEnding += SystemEvents_SessionEnding;
                    LoadLanguage();//加载语言类型
                    //获得当前登录的Windows用户标示
                    WindowsPrincipal principal = new(WindowsIdentity.GetCurrent());
                    if (Debugger.IsAttached || principal.IsInRole(WindowsBuiltInRole.Administrator))
                    {
                        Run();  //如果是管理员，则直接运行
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

        private static Boolean LoadLanguage()
        {
            Language lang = Language.简体中文;
            if (AppConfig.GetIntance() == null || AppConfig.GetIntance()!.LANGUAGEID == null)
            {
                lang = CultureInfo.InstalledUICulture.Name switch
                {
                    "en-US" => Language.English,
                    "zh-CN" => Language.简体中文,
                    _ => Language.简体中文
                };

                if (AppConfig.GetIntance() != null)
                {
                    AppConfig.GetIntance()!.LANGUAGEID = (Int32)lang;
                }
            }
            Controls.Language.LanguageManger.Instance.Language = LanguageFactory.GetLanguage(AppConfig.GetIntance() != null ? (Language)AppConfig.GetIntance()!.LANGUAGEID!.Value : lang);

            return true;
        }

        private static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            // FilePrsnt.SaveSetting(Environment.CurrentDirectory, "LastSettings", false, true);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e == null || e.ExceptionObject == null)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs("应用程序域未处理异常：无异常信息", LogLevel.Warn));
            }
            else
            {
                Exception ex = e.ExceptionObject as Exception;
                if (ex != null)
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(ex.Message, LogLevel.Warn));
                }
                else
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs("应用程序域未处理异常：异常类型错误", LogLevel.Warn));
                }
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (e == null || e.Exception == null)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs("应用程序线程未处理异常：无异常信息", LogLevel.Warn));
            }
            else
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(e.Exception.Message, LogLevel.Warn));
            }
        }
        private static Dictionary<String, long> ElapsedMilliseconds = new Dictionary<String, long>();
        private static void Run()
        {
            //配置风格管理器的风格配置项
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.StyleConfig = AppStyleConfig.Singleton;
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.EnableStlize = Constants.ENABLE_STYLE;

            var dsoform = new DsoForm();
            //Oscilloscope = new DsoPrsnt(dsoform, PlatformUIManager.Default.Platform.ProductType);
            Oscilloscope = new DsoPrsnt(dsoform,ProductType.B24_AI20G);
            var dsoapp = new DsoAppContent(dsoform, new SplashForm());
            String type = String.Empty;
            var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\AboutInfo.xml");
            if (System.IO.File.Exists(filepath))
            {
                using (var sm = System.IO.File.OpenRead(filepath))
                {
                    XmlReader reader = XmlReader.Create(sm);
                    reader.ReadToFollowing("AboutInfo");

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Type")
                        {

                            reader.Read();
                            if (reader.NodeType == XmlNodeType.Text)
                            {
                                type = reader.Value.Trim();
                                break;
                            }
                        }
                    }
                    reader.Close();
                }
            }
            if (!String.IsNullOrEmpty(type))
            {
                Oscilloscope.SetProductModel(type);
            }

            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs("开始启动,版本号:" + Application.ProductVersion, LogLevel.Info));

            Application.Run(dsoapp);

            Oscilloscope.Close();
        }
        private static async Task<AsyncTaskResult> MatlabEngineInitAsync(String mark)
        {
            return await Task.Run(() =>
            {
                DateTime dt = DateTime.Now;
                if (Constants.ENABLE_MATLAB)
                {
                    MathExt.MatlabEngine.AsyncInit();
                }
                return new AsyncTaskResult() { Success = true, Mark = mark, ErrorMsg = "", UsedMilliseconds = DateTime.Now.Subtract(dt).TotalMilliseconds };
            }
            );
        }

        private static async Task<AsyncTaskResult> SystemInformationCheck(String mark)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                    {
                        var regkey = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\OEMInformation", true);
                        if (regkey != null)
                        {
                            using (regkey)
                            {
                                var modelobj = regkey.GetValue("Model");
                                if (modelobj != null)
                                {
                                    var modelstr = modelobj.ToString();
                                    String md = Oscilloscope.GetProductModel();
                                    if (!md.Equals(modelstr, StringComparison.OrdinalIgnoreCase))
                                    {
                                        regkey.SetValue("Model", md);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(ex, LogLevel.Error));
                    return new AsyncTaskResult() { Success = false, Mark = mark, ErrorMsg = ex.Message };
                }
                return new AsyncTaskResult() { Success = true, Mark = mark, ErrorMsg = "" };
            });
        }

        private static async Task<AsyncTaskResult> HardwareOpenAsync(String mark)
        {
            return await Task.Run(() =>
            {
                try
                {
                    bool succ = Oscilloscope.Open("", Constants.BOARD_ATTACHED ? DataSourceOpt.PCIe : DataSourceOpt.Simulator);
                    return new AsyncTaskResult() { Success = succ, Mark = mark, ErrorMsg = "" };
                }
                catch (Exception ex)
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(ex, LogLevel.Error));
                    return new AsyncTaskResult() { Success = false, Mark = mark, ErrorMsg = ex.Message };
                }
            }
            );
        }

        public static void InitApp(Object arg)
        {
            DsoPrsnt.KeyBoardLockEnable = true;

            Oscilloscope.ErrorMsgBox = ErrorMsgBox;

            var startup = (SplashForm)arg;

            startup.Invoke(new Action<String>((o) => startup.ShowMessge(o)), ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProgramCS_StartingBaseService"));// "正在初始化基础服务......"

            #region 开启异步Task，并行 键盘检测、GPU编译、SCPI Manager、MatlabEngineInit、硬件上电等5个任务
            List<Task<AsyncTaskResult>> asyncTasks = new List<Task<AsyncTaskResult>>();

            if (Oscilloscope.View is DsoForm dsoform)
            {
                //Async task1:DsoFormInitOnLoad
                asyncTasks.Add(dsoform.InitAsync(startup, "DsoFormInitOnLoad"));
                //Async task2:StartScpiManager
                asyncTasks.Add(dsoform.StartScpiManagerAsync("StartScpiManager"));
            }
            //Async task3:MatlabEngineInitAsync
            asyncTasks.Add(MatlabEngineInitAsync("MatlabEngineInit"));
            //Async task4:keyboard

            if (PlatformUIManager.Default.Platform.Attribute.SupportKeyBoard)
                asyncTasks.Add(KeyboardLed.Default.InitAsync("keyboard"));

            //Async task5:HardwareOpen
            asyncTasks.Add(HardwareOpenAsync("HardwareOpen"));

            // 系统信息同步
            // asyncTasks.Add(SystemInformationCheck("SystemInfoSync"));

            if (!Task.WaitAll(asyncTasks.ToArray(), 2 * 60 * 1000))
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"异步初始化超时120S", LogLevel.Info));


            OptionManager.Default.SetModel();

            var errorTasks = asyncTasks.Where(o => !o.Result.Success);
            bool bContinue = true;
            foreach (var task in errorTasks)
            {
                switch (task.Result.Mark)
                {
                    case "keyboard":
                        break;
                    case "DsoFormInitOnLoad":
                        break;
                    case "HardwareOpen":
                        bContinue = false;
                        break;
                    default:
                        break;
                }
            }
            #endregion

            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(ExportHdFuncs.GetAllFPGAVersionInfo(), LogLevel.Info));
            if (Constants.BOARD_ATTACHED == false || (Constants.BOARD_ATTACHED == true && bContinue))
            {
                String msg = "";
                if (Constants.BOARD_ATTACHED == false || VersionManager.CheckVersion(out msg))
                {
                    startup.Invoke(new Action<String>((o) => startup.ShowMessge(o)), msg);
                    Thread.Sleep(3000);


                    Oscilloscope.Run();
                    startup.Invoke(new Action(() =>
                    {
                        startup.Close();
                    }));

                    if (Oscilloscope.View is DsoForm dsofm)
                    {
                        dsofm.InitChannelActive();
                        dsofm.LoadLastSettingFunction();
                    }
                    DsoPrsnt.KeyBoardLockEnable = false;
                }
                else
                {
                    startup.Invoke(new Action(() =>
                    {
                        ExportHdFuncs.ConfigLed(ErrorCode.ErrorType);
                        StrongTipEventArgs arg = new StrongTipEventArgs("MsgTipId.Error", "MsgTipId.CheckVersion", MessageType.Error, ErrorCode.ErrorType.GetDescription(), msg);
                        MsgBox msgTipForm = new MsgBox(arg);
                        msgTipForm.TopMost = true;
                        msgTipForm.Owner = startup;
                        msgTipForm.ShowDialog();
                    }));
                    if (Oscilloscope.View is DsoForm form)
                    {
                        form.Invoke(new Action(() =>
                        {
                            form.DsoClosing();
                        }));
                    }
                    Application.Exit();
                }
            }
            else
            {
                startup.Invoke(new Action(() =>
                {
                    String errorstr = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProgramCS_StartError");
                    if (ErrorCode.ErrorType == ErrorType.S_ADC_Tap_Max_Min_00072 || ErrorCode.ErrorType == ErrorType.S_ADC_Tap_Zero_00073)
                    {
                        errorstr = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProgramCS_S00072_S00073_Error");
                    }

                    ExportHdFuncs.ConfigLed(ErrorCode.ErrorType);
                    MsgBox msgTipForm = new MsgBox();
                    msgTipForm.Message = errorstr + ErrorCode.ErrorType.GetDescription(); // "启动异常，请查看错误日志！错误码："
                    msgTipForm.Title = "Error";
                    msgTipForm.TopMost = true;
                    msgTipForm.Owner = startup;
                    msgTipForm.ShowDialog();
                }));
                startup.Invoke(new Action<String>((o) => startup.ShowMessge(o)), "Fail!");
                if (Oscilloscope.View is DsoForm form)
                {
                    form.Invoke(new Action(() =>
                    {
                        form.DsoClosing();
                    }));
                }
                Application.Exit();
            }
        }

        private static Boolean CheckVersion()
        {
            if (Constants.ENABLE_VERSION == false)
                return true;

            var versiondictionary = Core.Hardware.ExportHdFuncs.TryTakeHardwareVersionInfo();
            foreach (var item in versiondictionary)
            {
                if (CheckVersionByType(item.Key, item.Value) == false)
                    return false;
            }
            return true;
        }

        private static bool CheckVersionByType(HardwareVersionItem type, HardwareVersionInfo? value)
        {
            if (value?.Major < 0)
            {
                return false;
            }
            if (value?.Major == 0)
            {
                if (value?.Minor < 3)
                {
                    return false;
                }
                if (value?.Minor == 3 && value?.Build < 7)
                {
                    return false;
                }
            }
            return true;
        }
        private static int ErrorMsgBoxCount = 0;//临时解决
        #region 多屏显示
        public static Int32 GetScreenIdx(Form form)
        {
            Int32 i;
            for (i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].Bounds.Contains(form.Location))
                {
                    break;
                }
            }
            return i;
        }

        public static Int32 GetScreenCount()
        {
            return Screen.AllScreens.Length;
        }

        public static void MoveToScreen(Form form, Int32 index)
        {
            Rectangle rect;
            if (index >= 0)
            {
                rect = Screen.AllScreens[index].Bounds;
            }
            else
            {
                rect = Screen.AllScreens.First((scr) => scr.Primary).Bounds;
            }

            var oldrect = form.Bounds;

            form.Top = (rect.Height - form.Height) / 2 + rect.Y;
            form.Left = (rect.Width - form.Width) / 2 + rect.X;

            foreach (var owned in form.OwnedForms)
            {
                var hr = (owned.Top - oldrect.Top) / (Double)oldrect.Height;
                var top = (Int32)(rect.Top + hr * rect.Height);
                if (top + owned.Height > rect.Bottom)
                {
                    top = rect.Bottom - owned.Height;
                    if (top < rect.Top)
                    {
                        top = rect.Top;
                    }
                }
                owned.Top = top;

                var wr = (owned.Left - oldrect.Left) / (Double)oldrect.Width;
                var left = (Int32)(rect.Left + wr * rect.Width);
                if (left + owned.Width > rect.Right)
                {
                    left = rect.Right - owned.Width;
                    if (left < rect.Left)
                    {
                        left = rect.Left;
                    }
                }
                owned.Left = left;
            }
        }
        #endregion
        private static void ErrorMsgBox(Int32 errorType)
        {
            //if (Oscilloscope.View is DsoForm dsoform1 && ErrorMsgBoxCount < 1)
            //{
            //    ErrorMsgBoxCount++;
            //    dsoform1.Invoke(new Action(() =>
            //    {
            //        MsgBox msgTipForm = new MsgBox();
            //        msgTipForm.Message = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ProgramCS_ACQError");//  "采集错误，请重启软件！"; // "启动异常，请查看错误日志！错误码："
            //        msgTipForm.Title = "Error";
            //        msgTipForm.TopMost = true;
            //        msgTipForm.Owner = dsoform1;
            //        msgTipForm.ShowDialog();
            //    }));
            //}
            //StrongTip.Default.Show(MsgTipId.Error, MsgTipId.AppClose, MessageType.Error);
        }
    }

    public class DsoAppContent : ApplicationContext
    {
        private readonly Form _MainForm;

        public DsoAppContent(Form mainForm, Form flashForm)
          : base(mainForm)
        {
            _MainForm = mainForm;

            //First set the SplashForm to use as context
            MainForm = flashForm;
        }

        protected override void OnMainFormClosed(object sender, EventArgs e)
        {
            if (sender is SplashForm)
            {
                //Secondly set the DsoForm to use as context
                MainForm = _MainForm;
                MainForm.Visible = true;
                MainForm.Activate();
            }
            else
            {
                base.OnMainFormClosed(sender, e);
            }
        }
    }
    public class AsyncTaskResult
    {
        public bool Success { get; set; }
        public String Mark { get; set; }
        public String ErrorMsg { get; set; }
        public double UsedMilliseconds { get; set; }
    }
}
