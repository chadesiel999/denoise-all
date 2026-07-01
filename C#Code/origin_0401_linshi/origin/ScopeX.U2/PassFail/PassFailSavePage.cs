// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/3/23</date>

using ScopeX.Core;

namespace ScopeX.U2.PassFail
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.MathExt;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;
    using ScopeX.Controls.Common.Structs;
    using ScopeX.UserControls;
    using static ScopeX.UserControls.SelectComboBox;

    public partial class PassFailSavePage : UserControl, IPassFailView, IStylize
    {
        private Boolean _ArgToCtrl;

        public PassFailSavePage()
        {
            InitializeComponent();
            InitControlsText();
        }
        private void InitControlsText()
        {
            LabelPath.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WeiZhi");
            LabelFileName.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MingCheng");
            LabelFileType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BoXingGeShi");
            LblTime.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("RiQiHouZhui");
            ChkSuffix.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            ChkSuffix.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            LabelPicType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JiePingGeShi");
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

        public PassFailPrsnt Presenter
        {
            get => Program.Oscilloscope.PassFail;
            set => (ParentForm as IPassFailView).Presenter = value;
        }

        IPassFailPrsnt IView<IPassFailPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (PassFailPrsnt)value;
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
                case nameof(Presenter.SavePath):
                    CbxPath.Text = Presenter.SavePath;
                    break;
                case nameof(Presenter.IfAppendDatetime):
                    ChkSuffix.Checked = Presenter.IfAppendDatetime;
                    TbxFileName.Text = Presenter.FileName;
                    break;
                case nameof(Presenter.FileName):
                    TbxFileName.Text = Presenter.FileName;
                    break;
                case nameof(Presenter.WfmFormat):
                    CbxFileType.SelectValue = Presenter.WfmFormat;
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                CbxPath.Text = Presenter.SavePath;
                ChkSuffix.Checked = Presenter.IfAppendDatetime;
                TbxFileName.Text = Presenter.FileName;
                CbxFileType.SelectValue = Presenter.WfmFormat;
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
            UpdateView();
            LoadWfmFormatList(Enum.GetValues<WfmFormat>());
            LoadPicFormatList(Enum.GetValues<PicFormat>());
        }
        private void LoadWfmFormatList(IEnumerable<WfmFormat> formats)
        {
            CbxFileType.DataSource = formats.Where(x => x != WfmFormat.BSV && x != WfmFormat.Text && x != WfmFormat.Matlab && x != WfmFormat.TSV && x != WfmFormat.DAT && x != WfmFormat.Excel).Select(x => new ComboBoxItem(x.ToString() + $"(.{x.GetAlias()})", x, null)).ToList();
            CbxFileType.SelectValue = Presenter.WfmFormat;
            CbxFileType.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.WfmFormat = (WfmFormat)CbxFileType.SelectValue;
                }
            };
        }
        private void LoadPicFormatList(IEnumerable<PicFormat> formats)
        {
            CbxPicType.DataSource = formats.Where(x => x != PicFormat.Tiff && x != PicFormat.Gif).Select(x => new ComboBoxItem(x.ToString() + $"({FilePrsnt.GetPicFileExtName(x)})", x, null)).ToList();
            CbxPicType.SelectValue = Presenter.PicFormat;
            CbxPicType.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.PicFormat = (PicFormat)CbxPicType.SelectValue;
                }
            };
        }
        private void BtnSelectPath_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.InitialDirectory = Presenter.SavePath;

                (ParentForm as FloatForm).CanClose = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Presenter.SavePath = dialog.SelectedPath;
                }
                if (ParentForm is FloatForm floatForm)
                {
                    floatForm.CanClose = true;
                }
            }
        }

        private void TbxFileName_TextChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.FileName = TbxFileName.Text;
            }
        }

        private void ChkSuffix_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.IfAppendDatetime = ChkSuffix.Checked;
            }
        }
    }
}
