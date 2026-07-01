using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Core;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class VoltmeterPage : UserControl, IVoltmeterView, IStylize
    {
        private Boolean _ArgToCtrl;

        public VoltmeterPage()
        {
            InitializeComponent();
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.FontSize;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

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

        public VoltmeterPrsnt Presenter
        {
            get => (VoltmeterPrsnt)(ParentForm as IVoltmeterView).Presenter;
            set => (ParentForm as IVoltmeterView).Presenter = value;
        }

        //IVoltmeterPrsnt IVoltmeterView.Presenter
        //{
        //    get => Presenter;
        //    set => Presenter = (VoltmeterPrsnt)value;
        //}

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (VoltmeterPrsnt)value;
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.Active):
                    ChkActive.Checked = Presenter.Active;
                    ParentForm?.Close();
                    break;

                case nameof(Presenter.Source):

                    CbxSource.SelectValue = Presenter.Source;
                    LblMode.Enabled = RdoMode.Enabled = !Presenter.EnableMode;
                    RdoMode.ChoosedButtonIndex = (Int32)Presenter.Mode;
                    ChkAutoRange.Visible = !Presenter.IsTriggerSource();
                    LblAutorange.Visible = ChkAutoRange.Visible;

                    //(ParentForm as FloatForm).HeadBackColor = Presenter.DrawColor;
                    //(ParentForm as FloatForm).Refresh();
                    break;

                case nameof(Presenter.Mode):
                    LblMode.Enabled = RdoMode.Enabled = !Presenter.EnableMode;
                    RdoMode.ChoosedButtonIndex = (Int32)Presenter.Mode;
                    break;

                case nameof(Presenter.AutoRange):
                    ChkAutoRange.Checked = Presenter.AutoRange;
                    break;
                case nameof(Presenter.IsStatActive):
                    if (ChkStatistics.Checked != Presenter.IsStatActive)
                    {
                        ChkStatistics.Checked = Presenter.IsStatActive;
                        BtnResetStat.Visible = ChkStatistics.Checked;
                    }
                    break;


            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                ChkActive.Checked = Presenter.Active;

                CbxSource.SelectValue = Presenter.Source;
                ChkAutoRange.Checked = Presenter.AutoRange;
                ChkAutoRange.Visible = !Presenter.IsTriggerSource();
                LblAutorange.Visible = ChkAutoRange.Visible;

                RdoMode.ChoosedButtonIndex = (Int32)Presenter.Mode;
                LblMode.Enabled = RdoMode.Enabled = !Presenter.EnableMode;
                ChkStatistics.Checked = Presenter.IsStatActive;
                BtnResetStat.Visible = ChkStatistics.Checked;
                InitControlIndexes();
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
            LoadSourceList(Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()));
            UpdateView();
        }

        private void LoadSourceList(IEnumerable<ChannelId> sources)
        {
            CbxSource.DataSource = sources.Select(x => new ComboBoxItem(x.ToString(), x)).ToList();
            CbxSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.Source = (ChannelId)CbxSource.SelectValue;
                }
            };


        }

        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Active = ChkActive.Checked;
            }
        }
        private void RdoFigure_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (!_ArgToCtrl)
                {
                    FreshControlIndexes();
                    var item = DsoPrsnt.DefaultDsoPrsnt.Voltmeter;
                    var source = ChannelId.DVM;
                    var figuretype = (MeasItemFigureType)RdoFigure.ChoosedButtonIndex;
                    if (figuretype == MeasItemFigureType.Close)
                    {
                        item.HistgramEnable = false;
                        item.TrackEnable = false;
                        item.TrendEnable = false;
                        return;
                    }

                    var activedprsnt = Program.Oscilloscope.TryGetRange(c => c.Id.IsMath() && c.Active);
                    var amc = activedprsnt.Count;
                    if (amc < 1)//直接打开
                    {
                        AddFigure(item, figuretype);
                        return;
                    }

                    foreach (var prsnt in activedprsnt)
                    {
                        var arg = (prsnt as MathPrsnt).Args;
                        switch (figuretype)
                        {
                            case MeasItemFigureType.Histgram:
                                if (arg is MathHistArg mha && mha.Source == source)
                                {
                                    DsoPrsnt.FocusId = prsnt.Id;//切换到当前channel
                                    return;
                                }
                                break;
                            case MeasItemFigureType.Track:
                                if (arg is MathTrackArg mtka && mtka.Source == source)
                                {
                                    DsoPrsnt.FocusId = prsnt.Id;
                                    return;
                                }
                                break;
                            case MeasItemFigureType.Trend:

                                if (arg is MathTrendArg mtda && mtda.Source == source)
                                {
                                    DsoPrsnt.FocusId = prsnt.Id;
                                    return;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    AddFigure(item, figuretype);
                }
            }
        }


        private void ChkStatistics_CheckedChangedEvent(object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.IsStatActive = ChkStatistics.Checked;
                BtnResetStat.Visible = ChkStatistics.Checked;
            }
        }


        private void BtnResetStat_Click(object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ResetStatistics();
            }
        }

        private void RdoMode_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Mode = (VoltmeterMode)RdoMode.ChoosedButtonIndex;
            }
        }

        private void InitControlIndexes()
        {
            _ArgToCtrl = true;

            var source = ChannelId.DVM;
            List<Int32> indexes = new();
            List<ChannelId> channels = new();
            var activedprsnt = (Program.Oscilloscope.View as DsoForm).Presenter.TryGetRange(c => c.Active && c.Id.IsMath());
            if (activedprsnt.Count < 1)
            {
                indexes.Add((Int32)MeasItemFigureType.Close);
                RdoFigure.ChoosedButtonIndex = (Int32)MeasItemFigureType.Close;
                RdoFigure.SelectIndexes = indexes;
                _ArgToCtrl = false;
                return;
            }

            foreach (var prsnt in activedprsnt)
            {
                var arg = (prsnt as MathPrsnt).Args;
                if (arg is MathHistArg mathhistarg && mathhistarg.Source == source)
                {
                    if (!indexes.Contains((Int32)MeasItemFigureType.Histgram))
                    {
                        indexes.Add((Int32)MeasItemFigureType.Histgram);
                    }
                    if (DsoPrsnt.FocusId == prsnt.Id)
                    {
                        RdoFigure.ChoosedButtonIndex = (Int32)MeasItemFigureType.Histgram;
                    }
                    channels.Add(prsnt.Id);
                }
                else if (arg is MathTrendArg mathtrendarg && mathtrendarg.Source == source)
                {
                    if (!indexes.Contains((Int32)MeasItemFigureType.Trend))
                    {
                        indexes.Add((Int32)MeasItemFigureType.Trend);
                    }
                    if (DsoPrsnt.FocusId == prsnt.Id)
                    {
                        RdoFigure.ChoosedButtonIndex = (Int32)MeasItemFigureType.Trend;
                    }
                    channels.Add(prsnt.Id);
                }
                else if (arg is MathTrackArg mathtrackarg && mathtrackarg.Source == source)
                {
                    if (!indexes.Contains((Int32)MeasItemFigureType.Track))
                    {
                        indexes.Add((Int32)MeasItemFigureType.Track);
                    }
                    if (DsoPrsnt.FocusId == prsnt.Id)
                    {
                        RdoFigure.ChoosedButtonIndex = (Int32)MeasItemFigureType.Track;
                    }
                    channels.Add(prsnt.Id);
                }

            }

            if (indexes.Count == 0)
            {
                indexes.Add((Int32)MeasItemFigureType.Close);
                RdoFigure.ChoosedButtonIndex = (Int32)MeasItemFigureType.Close;
            }
            else
            {
                if (RdoFigure.ChoosedButtonIndex == 0)
                {
                    RdoFigure.ChoosedButtonIndex = indexes.FirstOrDefault();
                    DsoPrsnt.FocusId = channels.FirstOrDefault();
                }
            }
            RdoFigure.SelectIndexes = indexes;

            _ArgToCtrl = false;
        }

        private void FreshControlIndexes()
        {
            if (RdoFigure.ChoosedButtonIndex == (Int32)MeasItemFigureType.Close)
            {
                RdoFigure.SelectIndexes = new List<int> { (Int32)MeasItemFigureType.Close };
            }
            else
            {
                if (RdoFigure.SelectIndexes.Contains((Int32)MeasItemFigureType.Close))
                {
                    RdoFigure.SelectIndexes.Remove((Int32)MeasItemFigureType.Close);
                }
            }
        }

        private void AddFigure(VoltmeterPrsnt prsnt, MeasItemFigureType figureType)
        {
            switch (figureType)
            {
                case Core.MeasItemFigureType.Histgram:
                    prsnt.HistgramEnable = true;
                    if (!prsnt.HistgramEnable)
                    {
                        OpenFailFresh();
                    }
                    break;
                case Core.MeasItemFigureType.Trend:
                    prsnt.TrendEnable = true;
                    if (!prsnt.TrendEnable)
                    {
                        OpenFailFresh();
                    }
                    break;
                case Core.MeasItemFigureType.Track:
                    prsnt.TrackEnable = true;
                    if (!prsnt.TrackEnable)
                    {
                        OpenFailFresh();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 打开新窗口失败刷新控件
        /// </summary>
        private void OpenFailFresh()
        {
            _ArgToCtrl = true;
            if (RdoFigure.SelectIndexes.Count == 0)
            {
                RdoFigure.ChoosedButtonIndex = 0;
            }
            else
            {
                RdoFigure.ChoosedButtonIndex = RdoFigure.SelectIndexes.Last();
            }
            _ArgToCtrl = false;
        }

        private void ChkAutoRange_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AutoRange = ChkAutoRange.Checked;
            }
        }
    }
}