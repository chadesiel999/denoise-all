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
using System.Windows.Forms;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class WfmSavePage : UserControl, IFileView, IStylize
    {
        private Boolean _ArgToCtrl;

        public WfmSavePage()
        {
            InitializeComponent();
            InitControlsText();
        }

        private void InitControlsText()
        {
            LabelPath.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WeiZhi");
            LabelFileName.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MingCheng");
            LabelFileType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BoXingGeShi");
            LabelSource.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Yuan");
            BtnSave.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BaoCun");
            LblTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("RiQiHouZhui");
            ChkSuffix.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkSuffix.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            BtnSaveAndRecall.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BaoCun_HuiDiao");
            BtnOpenDir.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DaKaiWenJianSuoZaiWeiZhi");
            LabelWfmTxtFormat.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WenBenGeShi");
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
            get;
            set;
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
                    //fix bug 6993,ljw 25.3.21
                    //CbxPath.Text = Presenter.WfmPath;
                    CbxPath.DataSource = new List<string>() { Presenter.WfmPath };
                    CbxPath.SelectedIndex = 0;
                    break;
                case nameof(Presenter.WfmSource):
                    //CbxSource.SelectedIndex = (Int32)Presenter.WfmSource;
                    //CbxSource.SetItemValue(Presenter.WfmSource);
                    CbxSource.SelectValue = Presenter.WfmSource;
                    Presenter.FileName = Presenter.MakeDefaultFileName(Presenter.WfmPath, FilePrsnt.GetWfmFileExtName(Presenter.WfmFormat));
                    break;
                case nameof(Presenter.IfAppendDatetime):
                    ChkSuffix.Checked = Presenter.IfAppendDatetime;
                    TbxFileName.Text = Presenter.FileName;
                    break;
                case nameof(Presenter.FileName):
                    TbxFileName.Text = Presenter.FileName;
                    break;
                case nameof(Presenter.WfmFormat):
                    //CbxFileType.SelectedIndex = (Int32)Presenter.WfmFormat;
                    CbxFileType.SelectValue = Presenter.WfmFormat;
                    BtnSaveAndRecall.Visible = Presenter.WfmFormat == WfmFormat.Binary || Presenter.WfmFormat == WfmFormat.CSV;
                    BtnSaveAndRecall.Visible =false;
                    LabelWfmTxtFormat.Visible = CbxWfmTxtFormat.Visible = Presenter.WfmFormat == WfmFormat.Text;
                    Presenter.FileName = Presenter.MakeDefaultFileName(Presenter.WfmPath, FilePrsnt.GetWfmFileExtName(Presenter.WfmFormat));
                    LoadSourece();
                    //if (Presenter.WfmFormat == WfmFormat.Binary)
                    //{
                    //    LoadSourceList(Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()));
                    //}
                    //else
                    //{
                    //    LoadSourceList(Program.Oscilloscope.FindIdentities(c => c.Id.IsAnalog() || (c.Active && c.Id.IsMath())), Presenter.WfmFormat == WfmFormat.DAT);
                    //}
                    break;
                case nameof(Presenter.WfmTxtFormat):
                    //CbxWfmTxtFormat.SelectedIndex = (Int32)Presenter.WfmTxtFormat;
                    CbxWfmTxtFormat.SelectValue = Presenter.WfmTxtFormat;
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                //CbxSource.SelectedIndex = (Int32)Presenter.WfmSource;
                //CbxSource.SetItemValue(Presenter.WfmSource);
                CbxSource.SelectValue = Presenter.WfmSource;

                //fix bug 6993,ljw 25.3.21
                //CbxPath.Text = Presenter.WfmPath;
                CbxPath.DataSource = new List<string>() { Presenter.WfmPath };
                CbxPath.SelectedIndex = 0;
                ChkSuffix.Checked = Presenter.IfAppendDatetime;
                TbxFileName.Text = Presenter.FileName;

                //CbxFileType.SelectedIndex = (Int32)Presenter.WfmFormat;
                CbxFileType.SelectValue = Presenter.WfmFormat;

                BtnSaveAndRecall.Visible = Presenter.WfmFormat == WfmFormat.Binary || Presenter.WfmFormat == WfmFormat.CSV;
                BtnSaveAndRecall.Visible = false;
                LabelWfmTxtFormat.Visible = CbxWfmTxtFormat.Visible = Presenter.WfmFormat == WfmFormat.Text;
                //CbxWfmTxtFormat.SelectedIndex = (Int32)Presenter.WfmTxtFormat;
                CbxWfmTxtFormat.SelectValue = Presenter.WfmTxtFormat;
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
            Presenter.FileName = Presenter.MakeDefaultFileName(Presenter.WfmPath, FilePrsnt.GetWfmFileExtName(Presenter.WfmFormat));
            LoadWfmFormatList(Enum.GetValues<WfmFormat>());
            LoadSourece();
            //if (Presenter.WfmFormat == WfmFormat.Binary)
            //{
            //    LoadSourceList(Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()));
            //}
            //else
            //{
            //    //LoadSourceList(Enum.GetValues<ChannelId>().Where(x => x.IsAnalog() || x.IsMath()), true);
            //    LoadSourceList(Program.Oscilloscope.FindIdentities(c => c.Id.IsAnalog() || (c.Active && c.Id.IsMath())), Presenter.WfmFormat == WfmFormat.DAT);
            //}
            LoadWfmTxtFormat(Enum.GetValues<TxtFormat>());
        }

        private void LoadSourece()
        {
            var source = Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()).ToList();
            if (Presenter.WfmFormat != WfmFormat.Binary)
            {
                var mprsnt = Program.Oscilloscope.TryGetRange(c => c.Id.IsBaseMath() && c.Active);
                mprsnt.ForEach(p =>
                {
                    if (p is MathPrsnt mp)
                    {
                        if (mp.Args.Type == MathType.Binary)
                        {
                            source.Add(mp.Id);
                        }
                    }
                });
            }
            var activedigital = Program.Oscilloscope.TryGetRange(cp => cp.Id.IsDigital() && cp.Active).ToList();
            var hasdigital = Presenter.WfmFormat == WfmFormat.DAT && activedigital.Count > 0;
            LoadSourceList(source, hasdigital);
        }

        private void LoadSourceList(IEnumerable<ChannelId> sources, Boolean hasDigital = false)
        {
            var src = sources.Select(x => new ComboBoxItem(x.ToString(), x, null)).ToList();

            if (hasDigital)
            {
                src.Add(new ComboBoxItem("DIGITAL", ChannelId.D0, null));
            }
            else
            {
                if (Presenter.WfmSource.IsDigital())
                    Presenter.WfmSource = sources.FirstOrDefault();
            }
            if (Presenter.WfmFormat == WfmFormat.CSV)
            {
                src.Add(new ComboBoxItem("All", "All", null));
            }
            CbxSource.DataSource = src;
            CbxSource.SelectValue = Presenter.WfmSource;
            CbxSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    if (CbxSource.SelectValue.ToString() != "All")
                    {
                        Presenter.WfmSource = (ChannelId)CbxSource.SelectValue;
                    }
                }
            };
        }

        private void LoadWfmFormatList(IEnumerable<WfmFormat> formats)
        {
            CbxFileType.DataSource = formats.Where(x => x != WfmFormat.BSV).Select(x => new ComboBoxItem(x.ToString() + $"(.{x.GetAlias()})", x, null)).ToList();
            CbxFileType.SelectValue = Presenter.WfmFormat;
            CbxFileType.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.WfmFormat = (WfmFormat)CbxFileType.SelectValue;
                }
            };
        }

        private void LoadWfmTxtFormat(IEnumerable<TxtFormat> formats)
        {
            CbxWfmTxtFormat.DataSource = formats.Select(x => new ComboBoxItem(x.ToString(), x, null)).ToList();
            CbxWfmTxtFormat.SelectValue = Presenter.WfmTxtFormat;
            CbxWfmTxtFormat.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.WfmTxtFormat = (TxtFormat)CbxWfmTxtFormat.SelectValue;
                }
            };
        }

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
                if (ParentForm is FloatForm floatForm)
                {
                    floatForm.CanClose = true;
                }
            }
        }

        private void TbxFileName_TextChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.FileName = TbxFileName.Text;
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
            if (Presenter.WfmSource.IsAnalog())
            {
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Presenter.WfmSource, out var prsnt))
                {
                    if (!prsnt.Active)
                    {
                        WeakTip.Default.Write("File", MsgTipId.FileSaveCheck);
                        return;
                    }
                }
            }
            var res = 0;
            if (Presenter.WfmFormat == WfmFormat.CSV && CbxSource.SelectValue.ToString() == "All")
            {
                var sources = new List<ChannelId>();
                foreach (ComboBoxItem item in CbxSource.DataSource)
                {
                    if (item.Item2 == "All")
                        continue;
                    sources.Add((ChannelId)item.Item2);
                }
                sources.Sort();
                if (sources.Contains(DsoPrsnt.FocusId))
                {
                    sources.Remove(DsoPrsnt.FocusId);
                    sources.Insert(0, DsoPrsnt.FocusId);
                }
                res = FilePrsnt.SaveAllWaveformByCsv(Presenter.WfmPath, Presenter.FileName, Presenter.IfAppendDatetime, Presenter.WfmTxtFormat, sources: sources.ToArray());
            }
            else
            {
                res = FilePrsnt.SaveWaveformEx(Presenter.WfmPath, Presenter.FileName, Presenter.WfmFormat, Presenter.WfmSource, Presenter.IfAppendDatetime, Presenter.WfmTxtFormat);
            }
            if (res == 1)
            {
                Presenter.FileName = Presenter.MakeDefaultFileName(Presenter.WfmPath, FilePrsnt.GetWfmFileExtName(Presenter.WfmFormat));
                WeakTip.Default.Close();
                ((DsoForm)Program.Oscilloscope.View).CloseWeakTipForm();

                WeakTip.Default.Write("File", MsgTipId.SavingSuccess, false, Presenter.WfmPath);
            }
            else if (res == 0)
            {
                WeakTip.Default.Close();
                ((DsoForm)Program.Oscilloscope.View).CloseWeakTipForm();
                WeakTip.Default.Write("File", MsgTipId.SavingFailed);
            }
            else//res==2
            {
                //unsave
            }
        }

        private void BtnOpenDic_Click(Object sender, EventArgs e)
        {
            var filename = ExplorerExtension.GetLatestFile(Presenter.WfmPath, "." + Presenter.WfmFormat.GetAlias());
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

        private void BtnSaveAndRecall_Click(Object sender, EventArgs e)
        {
            if (Presenter.WfmSource.IsAnalog())
            {
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Presenter.WfmSource, out var prsnt))
                {
                    if (!prsnt.Active)
                    {
                        WeakTip.Default.Write("File", MsgTipId.FileSaveCheck);
                        return;
                    }
                }
            }
            var res = -1;
            if (Presenter.WfmFormat == WfmFormat.CSV && CbxSource.SelectValue.ToString() == "All")
            {
                var sources = new List<ChannelId>();
                foreach (ComboBoxItem item in CbxSource.DataSource)
                {
                    if (item.Item2 == "All")
                        continue;
                    sources.Add((ChannelId)item.Item2);
                }
                sources.Sort();
                if (sources.Contains(DsoPrsnt.FocusId))
                {
                    sources.Remove(DsoPrsnt.FocusId);
                    sources.Insert(0, DsoPrsnt.FocusId);
                }
                res = FilePrsnt.SaveAllWaveformByCsv(Presenter.WfmPath, Presenter.FileName, Presenter.IfAppendDatetime, Presenter.WfmTxtFormat, sources: sources.ToArray());
            }
            else
            {
                res = FilePrsnt.SaveWaveformEx(Presenter.WfmPath, Presenter.FileName, Presenter.WfmFormat, Presenter.WfmSource, Presenter.IfAppendDatetime, Presenter.WfmTxtFormat);
            }
            if (res == 1)
            {
                var fullpath = Presenter.WfmPath + "\\" + Presenter.FileName + "." + Presenter.WfmFormat.GetAlias();
                foreach (var id in ChannelIdExt.GetReferences())
                {
                    Program.Oscilloscope.TryGetChannel(id, out var cp);
                    if (cp is null || !cp.Active)
                    {
                        ReferencePrsnt rprsnt = null;
                        FileInfo fileInfo = new FileInfo(fullpath);
                        if (fileInfo.Extension == "." + WfmFormat.Binary.GetAlias())
                        {
                            if (ReferencePrsnt.TryRead(id, Presenter.Dso, fullpath, ref rprsnt))
                            {
                                Program.Oscilloscope.AddChannel(id, rprsnt);
                                rprsnt.Active = true;
                                Presenter.FileName = Presenter.MakeDefaultFileName(Presenter.WfmPath, FilePrsnt.GetWfmFileExtName(Presenter.WfmFormat));
                                return;
                            }
                        }
                        if (fileInfo.Extension == "." + WfmFormat.CSV.GetAlias())
                        {
                            if (ReferencePrsnt.TryReadSVG(id, Presenter.Dso, fullpath, ref rprsnt))
                            {
                                Program.Oscilloscope.AddChannel(id, rprsnt);
                                rprsnt.Active = true;
                                Presenter.FileName = Presenter.MakeDefaultFileName(Presenter.WfmPath, FilePrsnt.GetWfmFileExtName(Presenter.WfmFormat));
                                return;
                            }
                        }
                        WeakTip.Default.Write("REF", MsgTipId.RefFileError1);
                    }
                }
                WeakTip.Default.Write("Ref", MsgTipId.NoMoreRefChannels);
                Presenter.FileName = Presenter.MakeDefaultFileName(Presenter.WfmPath, FilePrsnt.GetWfmFileExtName(Presenter.WfmFormat));
            }
            else if (res == 0)
            {
                WeakTip.Default.Write("File", MsgTipId.SavingFailed);
            }
            else//res==2
            {
                //unsave
            }
        }
    }
}
