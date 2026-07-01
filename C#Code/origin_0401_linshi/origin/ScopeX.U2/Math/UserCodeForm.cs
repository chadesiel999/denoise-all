using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using EventBus;
using ScopeX.Controls.Common.Helper;
using ScopeX.Controls.Common.Structs;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.U2.BaseControl;
using ScopeX.U2.LanguageSupoort;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class UserCodeForm :FlashBorderForm
    {
        public UserCodeForm(MathUserProgramArg userProgramArg)
        {
            InitializeComponent();
            _UserProgramArg = userProgramArg;
            Title = $"{_UserProgramArg.UserProgramType} {Title}";
            this.HelpClick += (_, _) =>
            {
                var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(UserCodeForm)));
            };

            if (!MatlabTools.IsMatlabInstalled())
            {
                WeakTip.Default.Write("Matlab", MsgTipId.MatlabUninstalled, false, "", 2);
                BtnExceteRepeat.Enabled = false;
                BtnExceteOneTime.Enabled = false;
                BtnCancel.Enabled = false;

            }

        }

        private readonly MathUserProgramArg _UserProgramArg;

        //public MathPrsnt Presenter
        //{
        //    get;
        //    set;
        //}

        //IBadge IView<IBadge>.Presenter
        //{
        //    get => Presenter;
        //    set => Presenter = (MathPrsnt)value;
        //}

        //public void UpdateView(object presenter, string propertyName)
        //{

        //}

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Escape)
            {
                Close();
                return;
            }
            base.OnKeyPress(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            TimerUpdate.Start();

            _UserProgramArg.ReadDataFromFile(_UserProgramArg.CodeFileFullPath, out string code);
            if (!String.IsNullOrWhiteSpace(code))
            {
                RtbxCodeEditor.Text = code;
            }

            Stylize();
#if SaveLanguage
            // LanguageFactory.CacheFormLanguageControls(this);
#endif
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (TimerUpdate != null)
            {
                TimerUpdate.Stop();
                TimerUpdate.Elapsed -= TimerUpdate_Tick;
                TimerUpdate.Enabled = false;
            }
            base.OnHandleDestroyed(e);
        }
        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
            ActiveBorderColor = Color.DeepSkyBlue;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams paras = base.CreateParams;
                paras.ExStyle |= 0x02000000;
                return paras;
            }
        }

        private void BtnSetWorkFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.InitialDirectory = _UserProgramArg.WorkFolder;

            //using DirectoryBrowserForm fbd = new(_UserProgramArg.WorkFolder);
            //fbd.StartPositionEx = fbd.CalculateWindowPosition();
            //if (fbd.ShowDialogByEvent() == DialogResult.Yes)
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _UserProgramArg.WorkFolder = dialog.SelectedPath;//fbd.ChoosedDirPath;
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            switch (_UserProgramArg.UserProgramType)
            {
                case UserProgramType.Matlab:
                    dialog.Filter = "m(*.m)|*.m";
                    break;
                case UserProgramType.JavaScript:
                    break;
                case UserProgramType.VbScript:
                    break;
                case UserProgramType.CPlusPlus:
                    break;
                case UserProgramType.Excel:
                    break;
                case UserProgramType.Close:
                    break;
                default:
                    break;
            }

            dialog.SetWindowInCenter();
            dialog.InitialDirectory = _UserProgramArg.WorkFolder;
            //FileBrowserForm fbf = FileBrowserForm.Instance;
            //fbf.CanEditFileName = true;
            //fbf.SetFileFilter(_UserProgramArg.CurFileExtension);
            //fbf.SetPath(_UserProgramArg.WorkFolder);

            //if (fbf.ShowDialogByEvent() == DialogResult.Yes && !String.IsNullOrWhiteSpace(fbf.FullFileName))
            if (dialog.ShowDialog() == DialogResult.OK && !String.IsNullOrWhiteSpace(dialog.FileName))
            {
                if (_UserProgramArg.ReadDataFromFile(dialog.FileName, out string code))
                {
                    RtbxCodeEditor.Text = code;
                    WeakTip.Default.Write("File", MsgTipId.ReadingSuccess);
                }
                else
                {
                    WeakTip.Default.Write("File", MsgTipId.ReadingFailed);
                }
            }
            //  fbf.CanEditFileName = false;
        }

        private void BtnSaveToFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            switch (_UserProgramArg.UserProgramType)
            {
                case UserProgramType.Matlab:
                    dialog.Filter = "m(*.m)|*.m";
                    break;
                case UserProgramType.JavaScript:
                    break;
                case UserProgramType.VbScript:
                    break;
                case UserProgramType.CPlusPlus:
                    break;
                case UserProgramType.Excel:
                    break;
                case UserProgramType.Close:
                    break;
                default:
                    break;
            }

            dialog.SetWindowInCenter();
            dialog.InitialDirectory = _UserProgramArg.WorkFolder;

            //FileBrowserForm fbf = FileBrowserForm.Instance;
            //fbf.CanEditFileName = true;
            //fbf.SetFileFilter(_UserProgramArg.CurFileExtension);
            //fbf.SetPath(_UserProgramArg.WorkFolder);

            //if (fbf.ShowDialogByEvent() == DialogResult.Yes && !String.IsNullOrWhiteSpace(fbf.FullFileName))
            if (dialog.ShowDialog() == DialogResult.OK && !String.IsNullOrWhiteSpace(dialog.FileName))
            {
                if (_UserProgramArg.SaveDataToFile(dialog.FileName, RtbxCodeEditor.Text))
                {
                    WeakTip.Default.Write("File", MsgTipId.SavingSuccess, false, Path.GetDirectoryName(dialog.FileName));
                }
                else
                {
                    WeakTip.Default.Write("File", MsgTipId.SavingFailed);
                }
            }
            //fbf.CanEditFileName = false;
        }

        private void BtnExcuteRepeat_Click(object sender, EventArgs e)
        {
            _UserProgramArg.RunState = RunStateType.Repeat;
            _UserProgramArg.ProgramCode = _UserProgramArg.curExecuteCode;
            _UserProgramArg.SaveDataToFile(_UserProgramArg.CodeFileFullPath, RtbxCodeEditor.Text);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            _UserProgramArg.RunState = RunStateType.Stop;
        }

        private void TimerUpdate_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(TickEvent));
            }
            else
            {
                TickEvent();
            }
            void TickEvent()
            {
                if (RtbxResult.Text != _UserProgramArg.ProgramResult)
                {
                    RtbxResult.Text = _UserProgramArg.ProgramResult;
                }
            }
        }

        private void BtnExceteOneTime_Click(object sender, EventArgs e)
        {
            _UserProgramArg.RunState = RunStateType.Single;
            _UserProgramArg.ProgramCode = _UserProgramArg.curExecuteCode;
            _UserProgramArg.SaveDataToFile(_UserProgramArg.CodeFileFullPath, RtbxCodeEditor.Text);
        }

        //private static void SaveDataToFile(string fileName, string data)
        //{
        //    StreamWriter sw = new(fileName);
        //    sw.Write(data);
        //    sw.Flush();
        //    sw.Close();
        //}

        //private string ReadDataFromFile(string fileName)
        //{
        //    if (!File.Exists(fileName))
        //    {
        //        return _UserProgramArg.CurDefaultCode;
        //    }

        //    StreamReader sr = new(fileName);
        //    String data = sr.ReadToEnd();
        //    sr.Close();
        //    return data;
        //}

        private void RtbxCodeEditor_VScroll(object sender, EventArgs e)
        {
            NativeMethods.SendMessage(this.RtbxCodeEditor.Handle, 0x0114, 1, 1);
        }
    }
}
