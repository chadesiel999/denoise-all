using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Core;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class CymometerPage : UserControl, ICymometerView, IStylize
    {
        private Boolean _ArgToCtrl;

        public CymometerPage()
        {
            InitializeComponent();
            CbxSource.Visible = false;
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

        public CymometerPrsnt Presenter
        {
            get => (CymometerPrsnt)(ParentForm as ICymometerView).Presenter;
            set => (ParentForm as ICymometerView).Presenter = value;
        }

        //ICymometerPrsnt ICymometerView.Presenter
        //{
        //    get => Presenter;
        //    set => Presenter = (CymometerPrsnt)value;
        //}

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (CymometerPrsnt)value;
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
                case nameof(Presenter.ShowPeriod):
                    RdoSwitch.ChoosedButtonIndex = Presenter.ShowPeriod ? 1 : 0;
                    break;
                case nameof(Presenter.Source):
                    //CbxSource.SelectedIndex = (Int32)Presenter.Source;
                    //(ParentForm as FloatForm).HeadBackColor = Presenter.DrawColor;
                    //(ParentForm as FloatForm).Refresh();
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
                RdoSwitch.ChoosedButtonIndex = Presenter.ShowPeriod ? 1 : 0;
                RdoSwitch.SelectIndexes = new List<Int32> { RdoSwitch.ChoosedButtonIndex };
                //CbxSource.SelectedIndex = (Int32)Presenter.Source;
                CbxSource.SelectValue = (Int32)Presenter.Source;
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
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            UpdateView();
        }

        private void LoadSourceList(IEnumerable<ChannelId> sources)
        {

            CbxSource.DataSource = sources.Select(x => new ComboBoxItem(x.ToString(), x, null)).ToList();
            CbxSource.SelectValue = Presenter.Source;
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
        private void RdoSwitch_IndexChanged(object sender, System.EventArgs e)
        {
            switch (RdoSwitch.ChoosedButtonIndex)
            {
                case 0:
                default:
                    Presenter.ShowPeriod = false;
                    RdoSwitch.SelectIndexes = new List<Int32> { RdoSwitch.ChoosedButtonIndex };
                    break;
                case 1:
                    Presenter.ShowPeriod = true;
                    RdoSwitch.SelectIndexes = new List<Int32> { RdoSwitch.ChoosedButtonIndex };
                    break;
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
        private void InitControlIndexes()
        {
            _ArgToCtrl = true;

            var source = ChannelId.CYM;
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

        private void RdoFigure_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (!_ArgToCtrl)
                {
                    FreshControlIndexes();
                    var item = DsoPrsnt.DefaultDsoPrsnt.Cymometer;
                    var source = ChannelId.CYM;
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

        private void AddFigure(CymometerPrsnt prsnt, MeasItemFigureType figureType)
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
    }
}
