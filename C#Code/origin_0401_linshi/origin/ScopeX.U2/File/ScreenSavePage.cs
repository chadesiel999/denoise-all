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
using System.Threading;
using System.Windows.Forms;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class ScreenSavePage : UserControl, IFileView, IStylize
    {
        private Boolean _ArgToCtrl;

        public ScreenSavePage()
        {
            InitializeComponent();
        }

        private void LoadPicFormatList(IEnumerable<PicFormat> formats)
        {
            CbxFileType.DataSource = formats.Select(x => new ComboBoxItem(x.ToString() + $"({FilePrsnt.GetPicFileExtName(x)})", x, null)).ToList();
            CbxFileType.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.PicFormat = (PicFormat)CbxFileType.SelectValue;
                }
            };
            //CbxFileType.DataSource = formats.Select(x => new KeyValuePair<String, PicFormat>(x.ToString() + $"({FilePrsnt.GetPicFileExtName(x)})", x)).ToList();
            //CbxFileType.DisplayMember = "Key";
            //CbxFileType.ValueMember = "Value";

            //CbxFileType.SelectedIndexChanged += (_, _) =>
            //{
            //    if (!_ArgToCtrl)
            //    {
            //        Presenter.PicFormat = (PicFormat)CbxFileType.SelectedIndex;
            //    }
            //};
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

        /// <summary>
        /// 生成文件名
        /// </summary>
        /// <returns></returns>
        //private String MakeDefaultFileName()
        //{
        //    if (Presenter.IfAppendDatetime)
        //        return Presenter.DefaultPrefixName + DateTime.Now.ToString("yyyyMMddHHmmss");
        //    else
        //    {
        //        var ext = Presenter.PicFormat.GetFileTypeDescription().Extension[0];
        //        var result = new System.IO.DirectoryInfo(Presenter.PicPath)
        //            .GetFiles($"*.{ext}", System.IO.SearchOption.TopDirectoryOnly)
        //            .Where(x => Regex.IsMatch(x.Name, $"^{Presenter.DefaultPrefixName}[0-9]{"{3}"}.{ext}$", RegexOptions.IgnoreCase));

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
                case nameof(Presenter.PicPath):
                    //fix bug 6993,ljw 25.4.14
                    //CbxPath.Text = Presenter.PicPath;
                    CbxPath.DataSource = new List<string>() { Presenter.PicPath };
                    CbxPath.SelectedIndex = 0;
                    break;
                case nameof(Presenter.PicRegion):
                    RdoRegion.ChoosedButtonIndex = Presenter.PicRegion == PicArea.Application ? 0 : 1;
                    break;
                case nameof(Presenter.IfAppendDatetime):
                    ChkSuffix.Checked = Presenter.IfAppendDatetime;
                    TbxFileName.Text = Presenter.FileName;
                    break;
                case nameof(Presenter.FileName):
                    TbxFileName.Text = Presenter.FileName;
                    break;
                case nameof(Presenter.PicFormat):
                    //CbxFileType.SelectedIndex = (Int32)Presenter.PicFormat;
                    CbxFileType.SelectValue = Presenter.PicFormat;
                    Presenter.FileName = Presenter.MakeDefaultFileName(Presenter.PicPath, FilePrsnt.GetPicFileExtName(Presenter.PicFormat));
                    break;
                case nameof(Presenter.PicColor):
                    RdoColor.ChoosedButtonIndex = (Int32)Presenter.PicColor;
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                RdoRegion.ChoosedButtonIndex = Presenter.PicRegion == PicArea.Application ? 0 : 1;
                //fix bug 6993,ljw 25.4.14
                //CbxPath.Text = Presenter.PicPath;
                CbxPath.DataSource = new List<string>() { Presenter.PicPath };
                CbxPath.SelectedIndex = 0;
                ChkSuffix.Checked = Presenter.IfAppendDatetime;
                ChkTimeStamp.Checked = FilePrsnt.IsTimestamp;
                TbxFileName.Text = Presenter.FileName;
                //CbxFileType.SelectedIndex = (Int32)Presenter.PicFormat;
                CbxFileType.SelectValue = Presenter.PicFormat;
                RdoColor.ChoosedButtonIndex = (Int32)Presenter.PicColor;
                _ArgToCtrl = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Directory.Exists(Presenter.PicPath))
            {
                Presenter.WfmPath = Constants.PIC_DEF_PATH;
                WeakTip.Default.Write("PicPath", MsgTipId.FilePathNotExist, false, "", 2);
            }
            LoadPicFormatList(Enum.GetValues<PicFormat>());
            UpdateView();
        }

        private void RdoRegion_IndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.PicRegion = RdoRegion.ChoosedButtonIndex == 0 ? PicArea.Application : PicArea.Window;
            }
        }

        private void RdoColor_IndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.PicColor = (PicColor)RdoColor.ChoosedButtonIndex;
            }
        }

        private void BtnSelectPath_Click(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.InitialDirectory = Presenter.PicPath;

                //using var fbd = new DirectoryBrowserForm(Presenter.PicPath);

                //fbd.StartPositionEx = fbd.CalculateWindowPosition();
                (ParentForm as FloatForm).CanClose = false;
                //if (fbd.ShowDialogByPosition() == DialogResult.Yes)
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Presenter.PicPath = dialog.SelectedPath;//fbd.ChoosedDirPath;
                }
                (ParentForm as FloatForm).CanClose = false;
            }
        }

        private void BtnSave_Click(Object sender, EventArgs e)
        {
            var picpath = Presenter.PicPath;
            var filename = Presenter.FileName;
            var picformat = Presenter.PicFormat;
            var picregion = Presenter.PicRegion;
            var appenddatetime = Presenter.IfAppendDatetime;
            var piccolor = Presenter.PicColor;
            //ParentForm.Visible = false;
            (ParentForm as FloatForm).Close();
            (Program.Oscilloscope.View as DsoForm).Activate();
            var res = FilePrsnt.SaveImageEx(picpath, filename, picformat, picregion, appenddatetime, piccolor, true, true);
            if (res == 1)
            {
                WeakTip.Default.Write("File", MsgTipId.SavingSuccess, false, picpath);
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

        private void ChkSuffix_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.IfAppendDatetime = ChkSuffix.Checked;
            }
        }
        private void ChkTimeStamp_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                FilePrsnt.IsTimestamp = ChkTimeStamp.Checked;
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
            var filename = ExplorerExtension.GetLatestFile(Presenter.PicPath, FilePrsnt.GetPicFileExtName(Presenter.PicFormat));
            if (String.IsNullOrEmpty(filename))
            {
                ExplorerExtension.ExplorerDic(Presenter.PicPath);
            }
            else
            {
                ExplorerExtension.ExplorerFile(filename);
            }
            ParentForm?.Close();
        }

        private void BtnSaveAndOpen_Click(Object sender, EventArgs e)
        {
            var picpath = Presenter.PicPath;
            var filename = Presenter.FileName;
            var picformat = Presenter.PicFormat;
            var picregion = Presenter.PicRegion;
            var appenddatetime = Presenter.IfAppendDatetime;
            var piccolor = Presenter.PicColor;
            ParentForm.Visible = false;
            var res = FilePrsnt.SaveImageEx(picpath, filename, picformat, picregion, appenddatetime, piccolor, true);
            if (res == 1)
            {
                Thread.Sleep(200);
                var latestfilename = ExplorerExtension.GetLatestFile(picpath, FilePrsnt.GetPicFileExtName(picformat));

                if (String.IsNullOrEmpty(latestfilename))
                {
                    ExplorerExtension.ExplorerDic(picpath);
                }
                else
                {
                    ExplorerExtension.ExplorerFile(latestfilename);
                }
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
