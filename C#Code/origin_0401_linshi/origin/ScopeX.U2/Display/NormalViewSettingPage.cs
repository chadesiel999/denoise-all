// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/20</date>

namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Core;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="NormalViewSettingPage" />.
    /// </summary>
    public partial class NormalViewSettingPage : UserControl, IDisplayView, IStylize
    {
        /// <summary>
        /// Defines the _ArgToCtrl.
        /// </summary>
        private Boolean _ArgToCtrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalViewSettingPage"/> class.
        /// </summary>
        public NormalViewSettingPage(Boolean visible)
        {
            InitializeComponent();
            LblPersist.Visible = CbxPersist.Visible = CbxPersist.Visible = visible;
            if (Program.Oscilloscope.Timebase.IsScan)
            {
                LblPersist.Visible = CbxPersist.Visible  = false;
            }
        }

        /// <summary>
        /// Gets or sets the Presenter.
        /// </summary>
        public DisplayPrsnt Presenter { get => (DisplayPrsnt)(ParentForm as IDisplayView).Presenter; set => (ParentForm as IDisplayView).Presenter = value; }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        /// <summary>
        /// Gets the DesignMode.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the Presenter.
        /// </summary>
        IDisplayPrsnt IView<IDisplayPrsnt>.Presenter { get => Presenter; set => Presenter = (DisplayPrsnt)value; }

        /// <summary>
        /// The Refresh.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        /// <summary>
        /// The UpdateView.
        /// </summary>
        /// <param name="prsnt">The presenter<see cref="Object"/>.</param>
        /// <param name="propertyName">The propertyName<see cref="String"/>.</param>
        public void UpdateView(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.DrawMode):
                    RdoWaveStyle.ChoosedButtonIndex = (Int32)Presenter.DrawMode;
                    break;
                case nameof(Presenter.GridStyle):
                    //CbxGridStyle.SelectedIndex = (Int32)Presenter.GridStyle;
                    CbxGridStyle.SelectValue = (Int32)Presenter.GridStyle;
                    break;
                case nameof(Presenter.AxisTickVisible):
                    ChkTickLabel.Checked = Presenter.AxisTickVisible;
                    break;
                case nameof(Presenter.XAxisTickBottom):
                    RdoVertLabelPos.ChoosedButtonIndex = Presenter.XAxisTickBottom ? 1 : 0;
                    break;
                case nameof(Presenter.YAxisTickRight):
                    RdoHorzLabelPos.ChoosedButtonIndex = Presenter.YAxisTickRight ? 1 : 0;
                    break;
                case nameof(Presenter.GridIntensity):
                    UtbGridIntensity.Value = Presenter.GridIntensity;
                    break;
                case nameof(Presenter.WfmIntensity):
                    UtbWaveIntensity.Value = Presenter.WfmIntensity;
                    break;
                case nameof(Presenter.Persist):
                    if (DsoPrsnt.DefaultDsoPrsnt.Timebase.StorageMode != AnaChnlStorageMode.Fast)
                    {
                        if (Presenter.Persist == WfmPersist.Auto)
                        {
                            Presenter.Persist = WfmPersist.Close;
                            CbxPersist.SelectIndex = 0;
                        }
                        else
                        {
                            if (Presenter.Persist == WfmPersist.Close)
                            {
                                CbxPersist.SelectIndex = 0;
                            }
                            else if (Presenter.Persist == WfmPersist.Infinity)
                            {
                                CbxPersist.SelectIndex = 1;
                            }
                        }

                    }
                    else
                    {
                        CbxPersist.SelectIndex = (Int32)Presenter.Persist;
                    }
                    break;
                case nameof(DsoPrsnt.DefaultDsoPrsnt.Timebase.StorageMode):
                    if (DsoPrsnt.DefaultDsoPrsnt.Timebase.StorageMode != AnaChnlStorageMode.Fast)
                    {
                        CbxPersist.Items = new string[] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBi"), ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("NormalViewSettingPage_WuXian")/*"无限"*/};

                        if (Presenter.Persist == WfmPersist.Auto)
                        {
                            Presenter.Persist = WfmPersist.Close;
                            CbxPersist.SelectIndex = 0;
                        }
                        else
                        {
                            if (Presenter.Persist == WfmPersist.Infinity)
                            {
                                CbxPersist.SelectIndex = 1;
                            }
                        }
                    }
                    else //开启快采显示自动余辉
                    {
                        CbxPersist.Items = new String[] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBi"), ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZiDong")/*"自动"*/, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("NormalViewSettingPage_WuXian")/*"无限"*/ };
                        CbxPersist.SelectIndex = (Int32)Presenter.Persist;
                    }
                    break;
                case nameof(Program.Oscilloscope.Timebase.IsScan):
                    if (Program.Oscilloscope.Timebase.IsScan)
                    {
                        LblPersist.Visible = CbxPersist.Visible = false;
                    }
                    else
                    {
                        LblPersist.Visible = CbxPersist.Visible = true;
                    }
                    break;
                default:
                    break;
            }
            _ArgToCtrl = false;
        }

        /// <summary>
        /// The OnLoad.
        /// </summary>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitOnLoad();
            UpdateView();
        }

        /// <summary>
        /// The UpdateView.
        /// </summary>
        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                RdoWaveStyle.ChoosedButtonIndex = (Int32)Presenter.DrawMode;
                //CbxGridStyle.SelectedIndex = (Int32)Presenter.GridStyle;
                CbxGridStyle.SelectValue = (Int32)Presenter.GridStyle;
                ChkTickLabel.Checked = Presenter.AxisTickVisible;
                RdoHorzLabelPos.ChoosedButtonIndex = Presenter.YAxisTickRight ? 1 : 0;
                RdoVertLabelPos.ChoosedButtonIndex = Presenter.XAxisTickBottom ? 1 : 0;
                UtbWaveIntensity.Value = Presenter.WfmIntensity;
                UtbGridIntensity.Value = Presenter.GridIntensity;
                //DSO模式自动切换为关闭余辉并隐藏
                if (DsoPrsnt.DefaultDsoPrsnt.Timebase.StorageMode != AnaChnlStorageMode.Fast)
                {
                    CbxPersist.Items = new string[] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBi"), ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("NormalViewSettingPage_WuXian")/*"无限"*/ };

                    if (Presenter.Persist == WfmPersist.Auto)
                    {
                        Presenter.Persist = WfmPersist.Close;
                        CbxPersist.SelectIndex = 0;
                    }
                    else
                    {
                        if (Presenter.Persist == WfmPersist.Close)
                        {
                            CbxPersist.SelectIndex = 0;
                        }
                        else if (Presenter.Persist == WfmPersist.Infinity)
                        {
                            CbxPersist.SelectIndex = 1;
                        }
                    }
                }
                else //开启快采显示自动余辉
                {
                    CbxPersist.Items = new String[] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuanBi"), ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZiDong")/*"自动"*/, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("NormalViewSettingPage_WuXian")/*"无限"*/ };
                    CbxPersist.SelectIndex = (Int32)Presenter.Persist;
                }
                _ArgToCtrl = false;
            }
        }

        /// <summary>
        /// The CbxGridStyle_SelectedIndexChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        //private void CbxGridStyle_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.GridStyle = (GridType)CbxGridStyle.SelectedIndex;
        //    }
        //}

        /// <summary>
        /// The CbxPersist_SelectedIndexChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        //private void CbxPersist_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.Persist = (WfmPersist)CbxPersist.SelectedIndex;
        //    }
        //}

        /// <summary>
        /// The ChkTickLabel_CheckedChangedEvent.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void ChkTickLabel_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AxisTickVisible = ChkTickLabel.Checked;
            }
        }

        /// <summary>
        /// The InitOnLoad.
        /// </summary>
        private void InitOnLoad()
        {
            UtbGridIntensity.ValueChanged += (_, _) => { Presenter.GridIntensity = (Int32)UtbGridIntensity.Value; };
            UtbWaveIntensity.ValueChanged += (_, _) => { Presenter.WfmIntensity = (Int32)UtbWaveIntensity.Value; };
        }

        /// <summary>
        /// The RdoHorzTickLabel_IndexChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void RdoHorzTickLabel_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.YAxisTickRight = RdoHorzLabelPos.ChoosedButtonIndex != 0;
            }
        }

        /// <summary>
        /// The RdoVertTickLabel_IndexChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void RdoVertTickLabel_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.XAxisTickBottom = RdoVertLabelPos.ChoosedButtonIndex != 0;
            }
        }

        /// <summary>
        /// The RdoWaveStyle_IndexChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void RdoWaveStyle_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.DrawMode = (WfmDrawMode)RdoWaveStyle.ChoosedButtonIndex;
            }
        }
        private void CbxGridStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.GridStyle = (GridType)CbxGridStyle.SelectValue;
            }
        }

        private void CbxPersist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (DsoPrsnt.DefaultDsoPrsnt.Timebase.StorageMode != AnaChnlStorageMode.Fast)
                {
                    WfmPersist temp = (WfmPersist)CbxPersist.SelectIndex;
                    Presenter.Persist = temp == WfmPersist.Auto ? WfmPersist.Infinity : temp;
                }
                else
                {
                    Presenter.Persist = (WfmPersist)(CbxPersist.SelectIndex);
                }
            }
        }
    }
}
