using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Core.Tools;
using System.Threading;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class MeasItemCfgPage : UserControl, IMeasView, IStylize
    {
        private Boolean _ArgToCtrl;

        public readonly Int32 PxIndex;

        public MeasItemCfgPage(Int32 pxIndex = 0)
        {
            InitializeComponent();
            PxIndex = pxIndex;
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

        public MeasPrsnt Presenter
        {
            get => (MeasPrsnt)(ParentForm as IMeasView).Presenter;
            set => (ParentForm as IMeasView).Presenter = value;
        }

        IMeasPrsnt IView<IMeasPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (MeasPrsnt)value;
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            var item = Presenter[PxIndex];
            switch (propertyName)
            {
                case nameof(item.Active):
                    ChkActive.Checked = item.Active;
                    //if (!item.Active)
                    //{
                    //    RemoveAllFigure(item.Id);
                    //}
                    break;
                case nameof(item.Source):
                    //CbxSource.SetItemText(item.Source);
                    CbxSource.SelectValue = item.Source;
                    //CbxSource.ForeColor = item.SourceColor;
                    //(ParentForm as FloatForm).HeadBackColor = Presenter[PxIndex].DrawColor;
                    break;
                case nameof(item.Source2nd):
                    //CbxSource2nd.SetItemText(item.Source2nd);
                    CbxSource2nd.SelectValue = item.Source2nd;
                    //CbxSource2nd.ForeColor = item.Source2ndColor;
                    break;
                case nameof(item.Name):
                    BtnName.Text = MeasureApp.Default.MeasCandidates[Presenter[PxIndex].Name].Text;
                    ChangeControlState();
                    break;
                case nameof(Presenter.Indicator):
                    ChkIndicator.Checked = Presenter.Indicator == PxIndex + 1;
                    break;
                case nameof(item.HistgramEnable):
                case nameof(item.TrackEnable):
                case nameof(item.TrendEnable):
                    UpdateControlIndexes();
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;

                ChkActive.Checked = Presenter[PxIndex].Active;
                if (!ChkActive.Checked)
                {
                    LblActive.Visible = ChkActive.Visible = RdoFigure.Visible = LblFigure.Visible = Presenter[PxIndex].MeasureType == MeasureType.Single;
                    BtnOK.Visible = !LblActive.Visible;
                }
                if (MeasureApp.Default.MeasCandidates.ContainsKey(Presenter[PxIndex].Name))
                {
                    BtnName.Text = MeasureApp.Default.MeasCandidates[Presenter[PxIndex].Name].Text;
                }
                else
                {
                    BtnName.Text = Presenter[PxIndex].Name;
                }
                //CbxSource.SetItemText(Presenter[PxIndex].Source);
                CbxSource.SelectValue = Presenter[PxIndex].Source;

                //CbxSource.ForeColor = Presenter[_PxIndex].SourceColor;
                //CbxSource2nd.SetItemText(Presenter[PxIndex].Source2nd);
                CbxSource2nd.SelectValue = Presenter[PxIndex].Source2nd;
                //CbxSource2nd.ForeColor = Presenter[_PxIndex].Source2ndColor;

                ChkIndicator.Checked = Presenter.Indicator == PxIndex + 1;

                if (Presenter[PxIndex].MeasureType == MeasureType.Composite)
                {
                    CbxSource.Width += 100;
                    CbxOperation.Location = new Point(CbxOperation.Location.X + 120, CbxOperation.Location.Y);
                    CbxOperation.Width = 50;
                    CbxSource2nd.Location = new Point(CbxSource2nd.Location.X + 150, CbxSource2nd.Location.Y);
                    CbxSource2nd.Width += 100;
                    if (!ChkActive.Visible)
                    {
                        LblSource.Location = new Point(LblSource.Location.X, LblSource.Location.Y - 50);
                        CbxSource.Location = new Point(CbxSource.Location.X, CbxSource.Location.Y - 50);
                        CbxOperation.Location = new Point(CbxOperation.Location.X, CbxOperation.Location.Y - 50);
                        CbxSource2nd.Location = new Point(CbxSource2nd.Location.X, CbxSource2nd.Location.Y - 50);
                        BtnOK.Location = new Point(BtnOK.Location.X, BtnOK.Location.Y - 50);
                        this.Height -= 100;
                    }
                }

                ChangeControlState();

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
            //!!!Patch: Digital channel source are appended to measure source list
            var srcs = Program.Oscilloscope.FindIdentities(c => c.Id.IsAnalog() || (c.Active && (c.Id.IsReference() /*|| c.Id.IsMath()*/))).OrderBy(x => (Int32)x);
            //if (Presenter.Dso.TryGetChannel(ChannelId.D0, out var cp))
            //{
            //    var dp = (DigitalPrsnt)cp;
            //    for (Int32 i = 0; i < dp.BitLength; i++)
            //    {
            //        if (dp.GetActiveAt(i))
            //        {
            //            srcs.Add(ChannelId.D0 + i);
            //        }
            //    }
            //}
            LoadSourceList(srcs);
            UpdateView();
            InitControlIndexes();
        }

        private void LoadSourceList(IEnumerable<ChannelId> sources)
        {
            if (Presenter[PxIndex].MeasureType == MeasureType.Single)
            {
                var exitsource = DsoPrsnt.DefaultDsoPrsnt.Measure.SelectedItems.Where((x, i) => x.Active && i != PxIndex && x.Name == Presenter[PxIndex].Name).Select(x => x.Source);//防止重复测量
                CbxSource.DataSource = sources.Where(x => !exitsource.Contains(x)).Select(x => new ComboBoxItem(x.ToString(), x)).ToList();
                CbxSource.SelectValue = Presenter[PxIndex].Source;
                CbxSource.SelectedIndexChanged += (_, _) =>
                {
                    if (!_ArgToCtrl)
                    {
                        Presenter[PxIndex].Source = (ChannelId)CbxSource.SelectValue;
                        //CbxSource.ForeColor = Presenter[_PxIndex].SourceColor;
                        ChangedLblIndicatorVisible();
                    }
                };

                CbxSource2nd.DataSource = sources.Select(x => new ComboBoxItem(x.ToString(), x)).ToList();
                CbxSource2nd.SelectValue = Presenter[PxIndex].Source2nd;
                CbxSource2nd.SelectedIndexChanged += (_, _) =>
                {
                    if (!_ArgToCtrl)
                    {
                        Presenter[PxIndex].Source2nd = (ChannelId)CbxSource2nd.SelectValue;
                    }
                };
            }
            else if (Presenter[PxIndex].MeasureType == MeasureType.Composite)
            {
                var src1 = Presenter.SelectedItems?.Where(x => x.Active && x.MeasureType == MeasureType.Single).Select(x => x.Id).ToList();
                var src2 = Presenter.SelectedItems?.Where(x => x.Active && x.MeasureType == MeasureType.Single).Select(x => x.Id).ToList();
                if (Presenter[PxIndex].Active && Presenter[PxIndex].Source.IsMeasure() && !src1.Contains(Presenter[PxIndex].Source))
                {
                    src1.Add(Presenter[PxIndex].Source);
                    src1 = src1.OrderBy(x => x).ToList();
                }

                CbxSource.DataSource = src1.Select(x => new ComboBoxItem(x.ToString(), x) { Enabled = Presenter.SelectedItems[x - ChannelId.P1].Active && Presenter.SelectedItems[x - ChannelId.P1].MeasureType == MeasureType.Single }).ToList();
                CbxSource.SelectValue = Presenter[PxIndex].Source.IsMeasure() ? Presenter[PxIndex].Source : src1.FirstOrDefault();
                CbxSource.SelectedIndexChanged += (_, _) =>
                {
                    if (!_ArgToCtrl)
                    {
                        Presenter[PxIndex].Source = (ChannelId)CbxSource.SelectValue;
                    }
                };
                if (Presenter[PxIndex].Active && Presenter[PxIndex].Source2nd.IsMeasure() && !src2.Contains(Presenter[PxIndex].Source2nd))
                {
                    src2.Add(Presenter[PxIndex].Source2nd);
                    src2 = src2.OrderBy(x => x).ToList();
                }
                CbxSource2nd.DataSource = src2.Select(x => new ComboBoxItem(x.ToString(), x) { Enabled = Presenter.SelectedItems[x - ChannelId.P1].Active && Presenter.SelectedItems[x - ChannelId.P1].MeasureType == MeasureType.Single }).ToList();
                CbxSource2nd.SelectValue = Presenter[PxIndex].Source2nd.IsMeasure() ? Presenter[PxIndex].Source2nd : src2.FirstOrDefault(x => x != (ChannelId)CbxSource.SelectValue);
                CbxSource2nd.SelectedIndexChanged += (_, _) =>
                {
                    if (!_ArgToCtrl)
                    {
                        Presenter[PxIndex].Source2nd = (ChannelId)CbxSource2nd.SelectValue;
                    }
                };
                CbxOperation.DataSource = Enum.GetValues(typeof(MeasureOperator))
                                       .Cast<MeasureOperator>()
                                       .Select(x => new ComboBoxItem(x.GetDescription(), x)).ToList();
                CbxOperation.SelectValue = Presenter[PxIndex].Operation;
            }
        }

        private readonly String[] _DualSrcMeasItems =
        {
            "Delay@lv",
            "Phase@lv",
            "Setup",
            "Hold",
            "Crossing"
        };

        private void ChangeControlState()
        {
            LblName.Visible = BtnName.Visible = Presenter[PxIndex].MeasureType == MeasureType.Single;
            LblThrold.Visible = BtnThrold.Visible = Presenter[PxIndex].Name.Contains("@lv") && Presenter[PxIndex].MeasureType == MeasureType.Single;

            BtnResetStat.Visible = Presenter[PxIndex].Active && Presenter.IsStatActive;
            BtnResetStat.Location = LblThrold.Visible ? new Point(BtnName.Location.X + BtnName.Width - BtnResetStat.Width, CbxSource.Location.Y) : new Point(LblName.Location.X, CbxSource.Location.Y);

            LblSourceTip.Visible = CbxSource2nd.Visible = Presenter[PxIndex].Dualsrc && Presenter[PxIndex].MeasureType == MeasureType.Single;
            if (Presenter[PxIndex].MeasureType == MeasureType.Composite)
            {
                CbxSource.Visible = CbxSource2nd.Visible = CbxOperation.Visible = true;
                BtnResetStat.Width += 42;
                BtnResetStat.Location = new Point(BtnResetStat.Location.X + 42, BtnResetStat.Location.Y - 90);
            }

            //<Remark>更改人：彭博 创建日期：2023/12/7 17:01:00  原因：当通道未打开时，将隐藏指示器控件 </Remark>
            ChangedLblIndicatorVisible();
        }

        public void ChangedLblIndicatorVisible()
        {
            if (Presenter[PxIndex].MeasureType == MeasureType.Single)
            {
                var (xpos, ypos) = Presenter.GetIndicator(PxIndex);

                DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Presenter[PxIndex].Source, out IChnlPrsnt chnlPrsnt);
                DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Presenter[PxIndex].Source2nd, out IChnlPrsnt chnlPrsnt2);
                if (chnlPrsnt != null && (CbxSource2nd.Visible ? chnlPrsnt2 != null : true))
                {
                    if (!chnlPrsnt.Active && (CbxSource2nd.Visible ? !chnlPrsnt2.Active : true))
                    {
                        if (xpos == null && ypos == null)
                        {
                            //<Remark>更改人：彭博 创建日期：2023/12/7 17:01:00  原因：当通道未打开时，将隐藏指示器控件 </Remark>
                            //LblIndicator.Visible = ChkIndicator.Visible = false;
                            LblIndicator.Visible = ChkIndicator.Visible = false;
                            if (ChkIndicator.Checked)
                            {
                                ChkIndicator.Checked = false;
                            }
                        }
                    }
                    else if (((ChannelId)CbxSource.SelectValue).IsReference() || (CbxSource2nd.Visible && ((ChannelId)CbxSource2nd.SelectValue).IsReference()))
                    {
                        Presenter.Indicator = 0;
                        LblIndicator.Visible = ChkIndicator.Visible = false;
                    }
                    else
                    {
                        LblIndicator.Visible = ChkIndicator.Visible = Presenter.GetIndicatorStates(Presenter[PxIndex].Name);
                    }
                }
                else
                {
                    LblIndicator.Visible = ChkIndicator.Visible = false;
                    if (ChkIndicator.Checked)
                    {
                        ChkIndicator.Checked = false;
                    }
                }
            }
            else if (Presenter[PxIndex].MeasureType == MeasureType.Composite)
            {
                LblIndicator.Visible = false;
                ChkIndicator.Visible = false;
            }
        }

        private void ChkActive_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter[PxIndex].Active = ChkActive.Checked;
                ParentForm?.Close();
            }
        }


        private void CbxOperation_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Presenter[PxIndex].Operation = (MeasureOperator)CbxOperation.SelectValue;
        }

        private void BtnName_Click(Object sender, EventArgs e)
        {
            MeasSelectionForm msf = new(Presenter[PxIndex].Name, Presenter[PxIndex].Source, (n, s) =>
            {
                var mi = MeasureApp.Default.Presenter.SelectedItems.FirstOrDefault(o => o.Active && o.Id != Presenter[PxIndex].Id && o.Name == n && o.Source == s);
                if (mi != null)
                {
                    WeakTip.Default.Write("Measure", MsgTipId.MeasuerLabelExisted);
                    return false;
                }

                Presenter[PxIndex].Name = n;
                Presenter[PxIndex].Source = s;
                Presenter[PxIndex].Active = true;
                return true;
            })
            {
                Anchor = AnchorStyles.Top,
                StartPosition = FormStartPosition.CenterScreen,
            };
            //msf.Owner = this.FindForm();
            (ParentForm as FloatForm).CanClose = false;
            Thread.Sleep(100);
            msf.ShowDialog();
            (ParentForm as FloatForm).CanClose = true;
            var srcs = Program.Oscilloscope.FindIdentities(c => c.Id.IsAnalog() || (c.Active && (c.Id.IsReference() /*|| c.Id.IsMath()*/))).OrderBy(x => (Int32)x);
            LoadSourceList(srcs);
        }

        private void BtnOK_Click(Object sender, EventArgs e)
        {
            Presenter[PxIndex].Active = true;
            Presenter[PxIndex].Source = (ChannelId)CbxSource.SelectValue;
            Presenter[PxIndex].Source2nd = (ChannelId)CbxSource2nd.SelectValue;
            Presenter[PxIndex].Operation = (MeasureOperator)CbxOperation.SelectValue;
            (this.ParentForm as MeasItemCfgForm).DialogResult = DialogResult.OK;
        }

        private void BtnResetStat_Click(Object sender, EventArgs e)
        {
            Presenter.ResetStat(PxIndex);
        }

        private void ChkIndicator_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (ChkIndicator.Checked)
            {
                Presenter.Indicator = PxIndex + 1;
            }
            else
            {
                Presenter.Indicator = 0;
            }
        }

        private void BtnThrold_Click(Object sender, EventArgs e)
        {
            var rlf = new MeasRefLevelForm(PxIndex)
            {
                Presenter = Presenter,
            };

            rlf.Presenter.TryAddView(rlf);

            (ParentForm as FloatForm).CanClose = false;
            rlf.ShowDialogByEvent();
            (ParentForm as FloatForm).CanClose = true;
        }

        private void RdoFigure_IndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                FreshControlIndexes();
                var item = Presenter[PxIndex];
                var source = (ChannelId)Enum.Parse(typeof(ChannelId), $"P{PxIndex + 1}");
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

        private void AddFigure(MeasItemPrsnt item, MeasItemFigureType figureType)
        {
            switch (figureType)
            {
                case Core.MeasItemFigureType.Histgram:
                    item.HistgramEnable = true;
                    if (!item.HistgramEnable)
                    {
                        OpenFailFresh();
                    }
                    break;
                case Core.MeasItemFigureType.Trend:
                    item.TrendEnable = true;
                    if (!item.TrendEnable)
                    {
                        OpenFailFresh();
                    }
                    break;
                case Core.MeasItemFigureType.Track:
                    item.TrackEnable = true;
                    if (!item.TrackEnable)
                    {
                        OpenFailFresh();
                    }
                    break;
                default:
                    break;
            }
        }

        private void UpdateControlIndexes()
        {
            _ArgToCtrl = true;
            var selectindex = new List<Int32>();
            var item = Presenter[PxIndex];
            if (item.HistgramEnable)
            {
                selectindex.Add((Int32)MeasItemFigureType.Histgram);
            }
            if (item.TrendEnable)
            {
                selectindex.Add((Int32)MeasItemFigureType.Trend);
            }
            if (item.TrackEnable)
            {
                selectindex.Add((Int32)MeasItemFigureType.Track);
            }
            if (selectindex.Count < 1)
            {
                selectindex.Add((Int32)MeasItemFigureType.Close);
                RdoFigure.ChoosedButtonIndex = (Int32)MeasItemFigureType.Close;
            }
            else
                RdoFigure.ChoosedButtonIndex = selectindex.LastOrDefault();

            RdoFigure.SelectIndexes = selectindex;
            _ArgToCtrl = false;
        }

        private void InitControlIndexes()
        {
            _ArgToCtrl = true;

            var source = (ChannelId)Enum.Parse(typeof(ChannelId), $"P{PxIndex + 1}");
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
