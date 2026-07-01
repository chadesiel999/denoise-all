using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using System.Collections.Generic;

namespace ScopeX.U2
{
    public partial class SettingPage : UserControl, IFileView, IStylize
    {
        private Boolean _ArgToCtrl;

        public SettingPage()
        {
            InitializeComponent();
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        private readonly String _TempFile = "TempSettingFile";

        protected new Boolean DesignMode
        {
            get
            {
                Boolean rtnflag = false;
#if DEBUG
                rtnflag = DesignTimeHelper.InDesignMode(this);
#endif
                return rtnflag;
            }
        }

        public FilePrsnt Presenter
        {
            get => (FilePrsnt)(ParentForm as IFileView).Presenter;
            set => (ParentForm as IFileView).Presenter = value;
        }

        IFilePrsnt IView<IFilePrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (FilePrsnt)value;
        }

        /// <summary>
        /// 生成文件名
        /// </summary>
        /// <returns></returns>
        //private String MakeDefaultFileName()
        //{
        //    if (Presenter.IfAppendDatetime)
        //        return Presenter.DefaultPrefixName + FilePrsnt.GetDateTimeString();
        //    else
        //    {
        //        var result = new System.IO.DirectoryInfo(Presenter.SettingSavePath)
        //            .GetFiles($"*.u2", System.IO.SearchOption.TopDirectoryOnly)
        //            .Where(x => Regex.IsMatch(x.Name, $"^{Presenter.DefaultPrefixName}[0-9]{"{3}"}.u2$", RegexOptions.IgnoreCase));

        //        return Presenter.DefaultPrefixName + String.Format("{0:D3}", result.Count());
        //    }
        //}

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        public void UpdateView(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.SettingSavePath):
                    //fix bug 6993,ljw 25.4.14
                    //CbxPath.Text = Presenter.SettingSavePath;
                    CbxPath.DataSource = new List<string>() { Presenter.SettingSavePath };
                    CbxPath.SelectedIndex = 0;
                    break;
                case nameof(Presenter.IfAppendDatetime):
                    ChkSuffix.Checked = Presenter.IfAppendDatetime;
                    TbxFileName.Text = Presenter.FileName;
                    break;
                case nameof(Presenter.FileName):
                    TbxFileName.Text = Presenter.FileName;
                    break;
                case nameof(Presenter.SettingLoadFullPath):
                    //fix bug 6993,ljw 25.4.14
                    //CbxFile.Text = Presenter.SettingLoadFullPath;
                    CbxPath.DataSource = new List<String>() { Presenter.SettingLoadFullPath };
                    CbxPath.SelectedIndex = 0;
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                //fix bug 6993,ljw 25.4.14
                //CbxPath.Text = Presenter.SettingSavePath;
                CbxPath.DataSource = new List<String>() { Presenter.SettingSavePath };
                CbxPath.SelectedIndex = 0;
                ChkSuffix.Checked = Presenter.IfAppendDatetime;
                TbxFileName.Text = Presenter.FileName;
                //fix bug 6993,ljw 25.4.14
                //CbxFile.Text = Presenter.SettingLoadFullPath;
                CbxFile.DataSource = new List<String>() { Presenter.SettingLoadFullPath };
                CbxFile.SelectedIndex = 0;
                _ArgToCtrl = false;
            }

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Directory.Exists(Presenter.SettingSavePath))
            {
                Presenter.WfmPath = Constants.SET_DEF_PATH;
                Presenter.SettingSavePath = Constants.SET_DEF_PATH;
                WeakTip.Default.Write("SettingSavePath", MsgTipId.FilePathNotExist, false, "", 2);
            }
            UpdateView();
        }

        private void BtnSelectPath_Click(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.InitialDirectory = Presenter.SettingSavePath;

                // using var fbd = new DirectoryBrowserForm(Presenter.SettingSavePath);
                //fbd.StartPositionEx = fbd.CalculateWindowPosition();
                (ParentForm as FloatForm).CanClose = false;
                // if (fbd.ShowDialogByPosition() == DialogResult.Yes)
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Presenter.SettingSavePath = dialog.SelectedPath;//fbd.ChoosedDirPath;
                }
                (ParentForm as FloatForm).CanClose = true;
            }
        }

        private void ChkSuffix_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.IfAppendDatetime = ChkSuffix.Checked;
            }
        }

        private void BtnSave_Click(Object sender, EventArgs e)
        {
            var res = FilePrsnt.SaveSettingEx(Presenter.SettingSavePath, Presenter.FileName, Presenter.IfAppendDatetime);
            if (res == 1)
            {
                Presenter.FileName = Presenter.MakeDefaultFileName(Presenter.SettingSavePath, ".set");
                WeakTip.Default.Write("File", MsgTipId.SavingSuccess, false, Presenter.SettingSavePath);
            }
            else if (res == 0)
            {
                WeakTip.Default.Write("File", MsgTipId.SavingFailed);
            }
            else//res=2
            {
                //unsave
            }
        }

        private void TbxFileName_TextChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.FileName = TbxFileName.Text;
            }
        }

        private void BtnOpenDir_Click(Object sender, EventArgs e)
        {
            var filename = ExplorerExtension.GetLatestFile(Presenter.SettingSavePath);
            if (String.IsNullOrEmpty(filename))
            {
                ExplorerExtension.ExplorerDic(Presenter.SettingSavePath);
            }
            else
            {
                ExplorerExtension.ExplorerFile(filename);
            }
            ParentForm?.Close();
        }

        private void BtnSaveAndOpen_Click(Object sender, EventArgs e)
        {
            var res = FilePrsnt.SaveSettingEx(Presenter.SettingSavePath, Presenter.FileName, Presenter.IfAppendDatetime);
            if (res == 1)
            {
                var filename = ExplorerExtension.GetLatestFile(Presenter.SettingSavePath);
                if (String.IsNullOrEmpty(filename))
                {
                    ExplorerExtension.ExplorerDic(Presenter.WfmPath);
                }
                else
                {
                    ExplorerExtension.ExplorerFile(filename);
                }
                Presenter.FileName = Presenter.MakeDefaultFileName(Presenter.SettingSavePath, ".set");
            }
            else if (res == 0)
            {
                WeakTip.Default.Write("File", MsgTipId.SavingFailed);
            }
            else//res=2
            {
                //unsave
            }
        }

        private enum SetType
        {
            set
        };

        private void BtnSelectFile_Click(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                //FileBrowserForm rdform = FileBrowserForm.Instance;
                //rdform.SetFileFilter(Enum.GetValues<SetType>().Where(x => x == SetType.set));
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "set(*.set)|*.set";
                dialog.SetWindowInCenter();
                String path = Presenter.SettingLoadFullPath;
                if (System.IO.File.Exists(Presenter.SettingLoadFullPath))
                {
                    path = Path.GetDirectoryName(path);
                }
                dialog.InitialDirectory = path;
                //rdform.SetPath(path);
                (ParentForm as FloatForm).CanClose = false;
                // if (rdform.ShowDialogByEvent() == DialogResult.Yes)
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (!StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.SwitchSettingFile, MessageType.Warning))
                    {
                        return;
                    }

                    Presenter.SettingLoadFullPath = dialog.FileName;// rdform.FullFileName;
                    if (FilePrsnt.LoadSetting(Presenter.SettingLoadFullPath, true))
                    {
                        (Program.Oscilloscope.View as DsoForm).InitBadge();
                        WeakTip.Default.Write("File", MsgTipId.ReadingSuccess);
                    }
                    else
                    {
                        WeakTip.Default.Write("File", MsgTipId.ReadingFailed);
                    }
                }
                if (ParentForm != null)
                {
                    (ParentForm as FloatForm).CanClose = true;
                }
            }
        }

        private void BtnLoadDefault_Click(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (!StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.RestoreFactoryDefault, MessageType.Warning))
                {
                    return;
                }

                var filename = Presenter.SettingSavePath + "\\" + _TempFile + ".set";
                if (System.IO.File.Exists(filename))
                {
                    if (!FilePrsnt.DeleteFile(filename))
                    {
                        WeakTip.Default.Write("File", MsgTipId.FileOccupied);
                        return;
                    }
                }
                FilePrsnt.SaveSetting(Presenter.SettingSavePath, _TempFile, false);//恢复出厂设置前 先保存一个临时文件 用于撤销
                (Program.Oscilloscope.View as DsoForm).Activate();
                if (FilePrsnt.LoadDefSetting())
                {
                    (Program.Oscilloscope.View as DsoForm).DefaultInit();
                    BtnUndoDefault.Visible = true;
                }
                else
                {
                    WeakTip.Default.Write("Default", MsgTipId.DefaultingFailed, false, "", 2);
                    BtnUndoDefault.Visible = false;
                }
            }
        }

        private void BtnUndoDefault_Click(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (!StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.UndoFactoryDefault, MessageType.Asking))
                {
                    return;
                }

                var filename = Presenter.SettingSavePath + "\\" + _TempFile + ".set";
                if (!System.IO.File.Exists(filename))
                {
                    WeakTip.Default.Write("File", MsgTipId.UndoFactoryDefaultErr);
                    BtnUndoDefault.Visible = false;
                    return;
                }

                var res = FilePrsnt.LoadSetting(filename);//读取临时文件

                if (res)
                {
                    (Program.Oscilloscope.View as DsoForm).InitBadge();
                }
                else
                {
                    WeakTip.Default.Write("File", MsgTipId.ReadingFailed);
                }

                if (!FilePrsnt.DeleteFile(filename))
                {
                    WeakTip.Default.Write("File", MsgTipId.FileOccupied);
                }//删除临时文件
                BtnUndoDefault.Visible = false;
            }
        }
    }
}
