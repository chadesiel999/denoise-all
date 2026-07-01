using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebSocketSharp.NetCore;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class LongStorageWfmSavePage : UserControl, IFileView, IStylize
    {
        private Boolean _ArgToCtrl;

        public LongStorageWfmSavePage()
        {
            InitializeComponent();
            InitControlsText();
        }

        private void InitControlsText()
        {
            LabelPath.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WeiZhi");
            LabelFileName.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MingCheng");
            LabelSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Yuan");
            LblTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("RiQiHouZhui");
            ChkSuffix.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkSuffix.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            BtnOpenDir.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DaKaiWenJianSuoZaiWeiZhi");
            LblSaveFormat.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WenJianGeShi");
            BtnSaveOriginData.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BaoCunShenCunChuShuJu");
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

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
                case nameof(Presenter.WfmPath):
                    //fix bug 6993,ljw 25.4.14
                    //CbxPath.Text = Presenter.WfmPath;
                    CbxPath.DataSource = new List<string>() { Presenter.WfmPath };
                    CbxPath.SelectedIndex = 0;
                    BeginInvoke(() => LoadFileFormatList());
                    break;
                case nameof(Presenter.WfmSource):
                    //CbxSource.SelectedIndex = (Int32)Presenter.WfmSource;
                    //CbxSource.SetItemValue(Presenter.WfmSource);
                    //CbxSource.SelectValue = Presenter.WfmSource;
                    Presenter.LongStroageFileName = Presenter.MakeDefaultFileName(Presenter.WfmPath, FilePrsnt.GetWfmFileExtName(Presenter.LongWfmFormat));
                    break;
                case nameof(Presenter.IfAppendDatetime):
                    ChkSuffix.Checked = Presenter.IfAppendDatetime;
                    TbxFileName.Text = Presenter.LongStroageFileName;
                    break;
                case nameof(Presenter.LongStroageFileName):
                    TbxFileName.Text = Presenter.LongStroageFileName;
                    break;
                case nameof(Presenter.LongWfmFormat):

                    if (Presenter.LongWfmFormat == WfmFormat.Binary)
                    {
                        LoadSourceList(Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()));
                    }
                    LoadFileFormatList();
                    CbxSaveFormat.SelectValue = Presenter.LongWfmFormat;
                    break;
                case nameof(Presenter.WfmTxtFormat):
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                //CbxSource.SelectValue = Presenter.WfmSource;

                //fix bug 6993,ljw 25.4.14
                //CbxPath.Text = Presenter.WfmPath;
                CbxPath.DataSource = new List<string>() { Presenter.WfmPath };
                CbxPath.SelectedIndex = 0;
                ChkSuffix.Checked = Presenter.IfAppendDatetime;
                TbxFileName.Text = Presenter.LongStroageFileName;


                _ArgToCtrl = false;
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitOnLoad();
            UpdateView();
        }

        private void InitOnLoad()
        {
            if (!Directory.Exists(Presenter.WfmPath))
            {
                Presenter.WfmPath = Constants.WFM_DEF_PATH;
                WeakTip.Default.Write("WfmPath", MsgTipId.FilePathNotExist, false, "", 2);
            }
            //Presenter.FileName = Presenter.MakeDefaultFileName(Presenter.WfmPath, Presenter.WfmFormat.GetAlias(), Presenter.WfmSource.ToString()[..1]);
            Presenter.LongStroageFileName = Program.Oscilloscope.SaveDataSoure.MakeDefaultFileName(Presenter.WfmPath, FilePrsnt.GetWfmFileExtName(Presenter.LongWfmFormat), "_C");

            LoadSourceList(Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()));
            LoadFileFormatList();
            InitControlIndexes();
        }

        private void InitControlIndexes()
        {
            RdoSource.SelectIndexes = new List<Int32>() { 0 };
        }

        private void LoadSourceList(IEnumerable<ChannelId> sources, Boolean hasDigital = false)
        {
            var src = sources.Select(x => new ComboBoxItem(x.ToString(), x, null)).ToList();

            if (hasDigital)
            {
                src.Add(new ComboBoxItem("DIGITAL", ChannelId.D0, null));
            }
            //CbxSource.DataSource = src;
            //CbxSource.SelectValue = Presenter.WfmSource;
            //CbxSource.SelectedIndexChanged += (_, _) =>
            //{
            //    if (!_ArgToCtrl)
            //    {
            //        Presenter.WfmSource = (ChannelId)CbxSource.SelectValue;
            //    }
            //};
        }

        private void LoadFileFormatList()
        {
            var formats = Enum.GetValues<WfmFormat>().ToList();
            //因文本格式下文件较大，故只检测在最大存储深度下是否支持文本格式
            if (Program.Oscilloscope.Timebase.StorageWaveDotsCnt > 501_000_000)//500M
            {
                if (!Program.Oscilloscope.SaveDataSoure.CheckIsSupportTxtFormat(Presenter.WfmPath))
                {
                    Presenter.LongWfmFormat = WfmFormat.Binary;
                    formats.Remove(WfmFormat.Text);
                }
            }

            if (Presenter.LongWfmFormat != WfmFormat.Binary && Presenter.LongWfmFormat != WfmFormat.Text)
            {
                Presenter.LongWfmFormat = WfmFormat.Binary;
            }
            CbxSaveFormat.DataSource = formats.Where(x => x == WfmFormat.Binary || x == WfmFormat.Text).Select(x => new ComboBoxItem(x.ToString() + $"(.{x.GetAlias()})", x, null)).ToList();
            CbxSaveFormat.SelectValue = Presenter.LongWfmFormat;
            CbxSaveFormat.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.LongWfmFormat = (WfmFormat)CbxSaveFormat.SelectValue;
                }
            };
        }

        private List<Int32> _SelectSource = new List<Int32>() { 0 };

        private void BtnSelectPath_Click(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.InitialDirectory = Presenter.WfmPath;

                // using var fbd = new DirectoryBrowserForm(Presenter.WfmPath);
                //fbd.StartPositionEx = fbd.CalculateWindowPosition();
                (ParentForm as FloatForm).CanClose = false;
                //if (fbd.ShowDialogByPosition() == DialogResult.Yes)
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Presenter.WfmPath = dialog.SelectedPath;//fbd.ChoosedDirPath;
                }
                (ParentForm as FloatForm).CanClose = true;
            }
        }

        private void TbxFileName_TextChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.LongStroageFileName = TbxFileName.Text;
            }
        }

        private void ChkSuffix_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.IfAppendDatetime = ChkSuffix.Checked;
            }
        }

        private void BtnOpenDic_Click(Object sender, EventArgs e)
        {
            var filename = ExplorerExtension.GetLatestFile(Presenter.WfmPath, "." + Presenter.LongWfmFormat.GetAlias());
            ParentForm.Owner = (Program.Oscilloscope.View as DsoForm);
            if (String.IsNullOrEmpty(filename))
            {
                ExplorerExtension.ExplorerDic(Presenter.WfmPath);
            }
            else
            {
                ExplorerExtension.ExplorerFile(filename);
            }
            ParentForm?.Close();
        }


        private void BtnSaveOriginData_Click(object sender, EventArgs e)
        {
            if (RdoSource.SelectIndexes.Count == 0)
            {
                WeakTip.Default.Write("File", MsgTipId.FileSaveCheck);
                return;
            }

            List<ChannelId> selectsource = new List<ChannelId>();
            RdoSource.SelectIndexes.ForEach(index => selectsource.Add((ChannelId)index));

            foreach (var source in selectsource)
            {
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(source, out var prsnt))
                {
                    if (!prsnt.Active)
                    {
                        WeakTip.Default.Write("File", MsgTipId.FileSaveCheck);
                        return;
                    }
                }
                else
                {
                    WeakTip.Default.Write("File", MsgTipId.FileSaveCheck);
                    return;
                }
            }

            Program.Oscilloscope.SaveDataSoure.DataSources = selectsource;

            if (!Program.Oscilloscope.SaveDataSoure.IsPrepareSuccess())
            {
                return;
            }
            Cursor = Cursors.Default;
            ParentForm.Close();
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.SAVEDATASOURCE);
        }

    }
}
