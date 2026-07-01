using EventBus;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Structs;
using ScopeX.Core.Presenter;
using ScopeX.Core;

namespace ScopeX.U2.Search
{
    public class SearchApp :ISearchView, IDisposable
    {
        public static SearchApp Default
        {
            get;
            internal set;
        }

        public ConcurrentDictionary<Int32, SearchInfo> InfoControls = new ConcurrentDictionary<Int32, SearchInfo>();
        public SearchForm SearchForm
        {
            get;
            set;
        }


        private SearchItemPrsnt _FoucsItem;
        public SearchItemPrsnt FoucsItem => Presenter?.GetFocusdSearchItemPrsnt;

        public SearchPrsnt Presenter { get; set; }

        ISearchPrsnt IView<ISearchPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (SearchPrsnt)value;
        }

        public ConcurrentDictionary<Int32, SearchItemPrsnt> Items
        {
            get { return Presenter.ItemPrsntMap; }
        }

        public PageInfoPrsnt PageInfo { get; private set; }

        public SearchApp(SearchPrsnt prsnt)
        {
            if (prsnt != null)
            {
                Presenter = prsnt;
                Presenter.TryAddView(this);
                PageInfo = Presenter.InitSearchPageInfo();
            }
        }


        public Int64 NavigationIndex { get; private set; } = -1;

        public void TriggerTypeChanged()
        {
            if (ItemForm != null && !ItemForm.IsDisposed)
            {
                ItemForm?.JudgeCopyFromTriggerEnable();
            }
        }

        public void AddSearchInfo(SearchItemPrsnt itemprsnt)
        {
            var info = new SearchInfo(itemprsnt);
            InfoControls.TryAdd(itemprsnt.ID, info);
            foreach (var item in Presenter.ItemPrsntMap)
            {
                if (InfoControls.ContainsKey(item.Key))
                {
                    InfoControls[item.Key].Visible = true;
                }
                else
                {
                    InfoControls.TryAdd(item.Key, new SearchInfo(item.Value));
                }
            }
        }

        public Boolean RemoveSearchInfo(Int32 id)
        {
            var result = InfoControls.TryRemove(id, out var info);
            if (info != null)
            {
                info.Visible = false;
                info.Dispose();
            }
            return result;
        }

        public SearchItemForm MakeItemForm(SearchItemPrsnt itemPrsnt)
        {
            if (this.ItemForm == null || this.ItemForm.IsDisposed)
            {
                var sf = new SearchItemForm()
                {
                    Presenter = itemPrsnt,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Text = itemPrsnt.Name
                };
                sf.Presenter.TryAddView(sf);
                sf.FormClosed += (_, _) =>
                {
                    this.ItemForm = null;
                };
                this.ItemForm = sf;
                return sf;
            }
            else
            {
                this.ItemForm?.Reload(itemPrsnt);
            }

            return this.ItemForm;
        }

        public Form InfoForm
        {
            get;
            private set;
        }

        public SearchItemForm ItemForm
        {
            get;
            private set;
        }



        public void ShowInfoForm(SearchItemPrsnt prsnt)
        {
            if (InfoForm == null || (InfoForm as SearchInfoForm).IsDisposed)
            {
                var sif = new SearchInfoForm(prsnt, PageInfo)
                {
                    Anchor = AnchorStyles.Top,
                    Location = new(100, 100),
                };
                InfoForm = sif;
            }

            if (InfoForm != null && !InfoForm.Visible && !(InfoForm as SearchInfoForm).IsDataTableFiguer)
            {
                EventBroker.Instance.GetEvent<FormEventArgs>().Publish(this, new FormEventArgs() { Current = InfoForm, Type = FormType.InfoForm });
            }
        }

        public void HideInfoForm()
        {
            if (InfoForm != null && !(InfoForm as SearchInfoForm).IsClosed)
            {
                InfoForm?.Close();
            }
            InfoForm = null;
        }

        public void SystemStateChanged()
        {
            if (Presenter == null)
            {
                return;
            }
            if (FoucsItem != null && InfoControls.Count > 0 && InfoControls.Keys.Contains(FoucsItem.ID))
            {
                InfoControls[FoucsItem.ID].UpdateView(null, null);
            }
        }

        public void UpdateNavigationIndex(Int64 index)
        {
            if (NavigationIndex != index)
            {
                NavigationIndex = index;

                if (NavigationIndex >= FoucsItem.ResultCount)
                {
                    NavigationIndex = FoucsItem.ResultCount - 1;
                }
                else if (NavigationIndex < 0)
                {
                    NavigationIndex = 0;
                }

                Navigation();
            }
        }

        public void NavigationPreviousIndex()
        {
            if (TriggerPrsnt.State == SysState.Stop && FoucsItem != null && FoucsItem.ResultCount > 0)
            {
                NavigationIndex--;
                if (NavigationIndex >= FoucsItem.ResultCount)
                {
                    NavigationIndex = FoucsItem.ResultCount - 2;
                }
                else if (NavigationIndex < 0)
                {
                    NavigationIndex = 0;
                }

                Navigation();
            }
        }

        public void NavigationNextIndex()
        {
            if (TriggerPrsnt.State == SysState.Stop && FoucsItem != null && FoucsItem.ResultCount > 0)
            {
                NavigationIndex++;
                if (NavigationIndex >= FoucsItem.ResultCount)
                {
                    NavigationIndex = FoucsItem.ResultCount - 1;
                }
                if (NavigationIndex < 0)
                {
                    NavigationIndex = 1;
                }

                Navigation();
            }
        }

        private void Navigation()//zoom打开和移动
        {
            //if (FoucsItem.EventEnable && InfoForm != null && InfoForm is SearchInfoForm form && form.IsDataTableFiguer)
            //{
            //    form.FormIndependent();
            //}

            if (!Program.Oscilloscope.Timebase.IsZoom)
            {
                Program.Oscilloscope.Timebase.IsZoom = true;
            }
            Program.Oscilloscope.Timebase.ZoomScaleY = 1;
            var result = FoucsItem.GetSearchResults();
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(FoucsItem.Source, out var chprsnt);
            var tmbinfo = (chprsnt.Sampling as TimebasePrsnt).GetCurrentTmbInfo();
            if (chprsnt != null && result != null && result.GetLength(1) > 0)
            {
                var tmb = FoucsItem.TransToTmb(result[0, NavigationIndex]);
                if (DsoPrsnt.DefaultDsoPrsnt.Timebase.MinPosition > tmb)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Timebase.PositionByus = DsoPrsnt.DefaultDsoPrsnt.Timebase.MinPosition;
                    var offset = ComModel.Constants.MAX_XPOS_IDX / 2.0f + (tmb - DsoPrsnt.DefaultDsoPrsnt.Timebase.PositionByus)* Program.Oscilloscope.Timebase.PosIdxPerDiv/ Program.Oscilloscope.Timebase.ScaleByus;
                    var minzommcenterX = ComModel.Constants.MAX_XPOS_IDX * Program.Oscilloscope.Timebase.ZoomScaleX / 2.0f;
                    offset = offset < minzommcenterX ? minzommcenterX : offset;
                    Program.Oscilloscope.Timebase.ZoomCenterX = offset;
                }
                else
                {
                    Program.Oscilloscope.Timebase.ZoomCenterX = (Int32)ComModel.Constants.MAX_XPOS_IDX / 2;
                    DsoPrsnt.DefaultDsoPrsnt.Timebase.PositionByus = tmb;
                }
                //var min = (chprsnt.Sampling as Time).CalcPosIndex();
            }
            if (InfoForm != null && PageInfo != null)
            {
                (InfoForm as SearchInfoForm).PageControlFirst = false;
                (InfoForm as SearchInfoForm).NeedUpdate = true;
            }
        }

        public void UpdateView(Object presenter, String propertyName)
        {
            if ((Program.Oscilloscope.View as DsoForm).InvokeRequired)
            {
                (Program.Oscilloscope.View as DsoForm).BeginInvoke(new Action<Object, String>(Update), new[] { presenter, propertyName });
            }
            else
            {
                Update(presenter, propertyName);
            }
        }

        public void Update(object prsnt, string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Presenter.SearchCount):
                    if (InfoControls.Count != Presenter.SearchCount)
                    {
                        var names = Presenter.ItemPrsntMap.Where(x => x.Value.Active).Select(x => x.Key).Except(InfoControls.Keys);
                        //添加
                        foreach (var name in names)
                        {
                            if (!InfoControls.Keys.Contains(name))
                            {
                                AddSearchInfo(Presenter.ItemPrsntMap[name]);
                            }
                        }

                        names = InfoControls.Keys.Except(Presenter.ItemPrsntMap.Where(x => x.Value.Active).Select(x => x.Key));
                        //移除
                        foreach (var name in names)
                        {
                            RemoveSearchInfo(name);
                        }

                        if (InfoControls.Count == 0)
                        {
                            HideInfoForm();
                            //this.FoucsItem = null;
                        }
                    }
                    break;
                case nameof(Presenter.FoucsID):
                    if (Presenter.FoucsID.HasValue)
                    {
                        if (Presenter.FoucsID.Value != FoucsItem?.ID)
                        {
                            Presenter.ItemPrsntMap[Presenter.FoucsID.Value].Focused = true;
                            foreach (var item in Presenter.ItemPrsntMap.Values)
                            {
                                if (item.ID != Presenter.FoucsID.Value)
                                {
                                    item.Focused = false;
                                }
                            }
                            if (ItemForm != null && !ItemForm.IsDisposed && PageInfo != null)
                            {
                                ItemForm.Reload(FoucsItem);
                            }
                            if (InfoForm != null && !InfoForm.IsDisposed && PageInfo != null)
                            {
                                var lastId = (InfoForm as SearchInfoForm)?.SearchItem.ID;
                                if (lastId != null)
                                {
                                    (InfoForm as SearchInfoForm).SearchItem = Presenter.ItemPrsntMap[Presenter.FoucsID.Value];
                                    Presenter.ItemPrsntMap[lastId.Value].EventEnable = false;
                                    Presenter.ItemPrsntMap[Presenter.FoucsID.Value].EventEnable = true;
                                    FoucsItem.IsNewResult = true;
                                    (InfoForm as SearchInfoForm).PageControlFirst = true;
                                    (InfoForm as SearchInfoForm).NeedUpdate = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        ItemForm?.Close();
                        ItemForm = null;
                        InfoForm?.Close();
                        InfoForm = null;
                    }

                    break;
            }
        }

        public void Dispose()
        {
            this.Presenter?.TryRemoveView(this);
        }
    }
}
