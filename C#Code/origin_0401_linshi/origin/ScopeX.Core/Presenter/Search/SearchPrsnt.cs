using System;
using ScopeX.ComModel;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using ScopeX.Core.Presenter;
using ScopeX.Core.Tools;
using System.Linq;

namespace ScopeX.Core
{
    public class SearchPrsnt : MulticastPrsnt<ISearchView>, ISearchPrsnt
    {
        public static readonly Object Locker = new();

        private protected override SearchModel Model { get; }

        public SearchPrsnt(IDsoPrsnt idp, ISearchView? view, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            Model = mco switch
            {
                ModelCreateOptions.Dependant => DsoModel.Default.Search,
                ModelCreateOptions.Standalone => new(DsoModel.Default.Meas),
                _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
            };
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
            SoftSearch = true;
            Enabled = true;
        }

        public ConcurrentDictionary<Int32, SearchItemPrsnt> ItemPrsntMap = new();

        public Boolean Enabled
        {
            get => Model.Enabled;
            set
            {
                if (!Constants.ENABLE_Search && value)
                {
                    WeakTip.Default.Write("Search", MsgTipId.FunctionDisabled);
                    Model.Enabled = false;
                    return;
                }
                Model.Enabled = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }

        public Boolean Running
        {
            get => Model.Running;
            set
            {
                Model.Running = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }

        public Boolean SoftSearch
        {
            get => Model.SoftSearch;
            set
            {
                Model.SoftSearch = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }


        public Int32 SearchCount
        {
            get => Model.SearchCount;
            private set => Model.SearchCount = value;
        }

        public Boolean RemoveAll()
        {
            foreach (var item in ItemPrsntMap.Values)
            {
                item.Active = false;
            }
            SearchCount = 0;
            return true;
        }


        public Boolean RemoveSearch(Int32 id)
        {
            var result = false;
            if (ItemPrsntMap.ContainsKey(id))
            {
                lock (Locker)
                {
                    if(ItemPrsntMap.ContainsKey(id))
                    {
                        ItemPrsntMap[id].Active = false;
                    }
                    MoveFocus();
                }
                result = true;
            }
            return result;
        }

        public Boolean GetorMakeSearchItemPrsnt(Int32 ID, out SearchItemPrsnt? prsnt, SearchType type = SearchType.Edge)
        {
            Boolean res = false;
            prsnt = null;
            if (!Constants.ENABLE_Lissajous)
            {
                WeakTip.Default.Write("Search", MsgTipId.FunctionDisabled);
                return res;
            }

            if (ItemPrsntMap.ContainsKey(ID))
            {
                prsnt = (SearchItemPrsnt)ItemPrsntMap[ID];
                return true;
            }

            if (SearchCount < Constants.MAX_SEARCH_CNT)
            {
                Model.AddSearch(ID, type, out var item);
                prsnt = new SearchItemPrsnt(Dso, item,this) { Active = false };
                ItemPrsntMap.TryAdd(prsnt.ID, prsnt);
                res = true;
            }
            return res;
        }

        public Boolean SearchItemActive(Int32 ID,Boolean active)
        {
            Boolean res = false;
            SearchItemPrsnt prsnt = null;
            if (!Constants.ENABLE_Lissajous)
            {
                WeakTip.Default.Write("Search", MsgTipId.FunctionDisabled);
                return res;
            }
            if (ItemPrsntMap.ContainsKey(ID))
            {
                prsnt = (SearchItemPrsnt)ItemPrsntMap[ID];
                prsnt.Active = active;
                if(active)
                {
                    SearchCount++;
                    FoucsID = ID;
                    if(TriggerPrsnt.State== SysState.Stop)
                    {
                        prsnt.OnceSearch = true;
                    }
                }
                else
                {

                    SearchCount--;
                    MoveFocus();
                }
                return true;
            }

            return res;
        }

        public Boolean TryMake(SearchType type, [NotNullWhen(true)] out SearchItemPrsnt? prsnt)
        {
            var res = false;
            prsnt = null;
            if (!Constants.ENABLE_Lissajous)
            {
                WeakTip.Default.Write("Search", MsgTipId.FunctionDisabled);
                return res;
            }

            var pt = ItemPrsntMap.FirstOrDefault(p => ((SearchItemPrsnt)p.Value).Active == false);
            if (pt.Value != null)
            {
                prsnt = (SearchItemPrsnt)pt.Value;
                return true;
            }
            if (SearchCount < Constants.MAX_SEARCH_CNT)
            {
                var id = GenerateID();
                Model.AddSearch(id, type, out var itemmodel);
                prsnt = new SearchItemPrsnt(Dso, itemmodel,this);
                prsnt.Type = type;
                ItemPrsntMap.TryAdd(id, prsnt);
                res = true;
            }
            else
            {
                WeakTip.Default.Write("Search", MsgTipId.SearchItemsMaximum);
            }
            return res;
        }

        private Int32 GenerateID()
        {
            Int32 id = 0;
            if (ItemPrsntMap.Count == 0)
            {
                return 0;
            }
            for (var index = id; index < Constants.MAX_SEARCH_CNT; index++)
            {
                if (!ItemPrsntMap.ContainsKey(index))
                {
                    id = index;
                    break;
                }
            }
            return id;
        }

        public void MoveFocus()
        {
            FoucsID = ItemPrsntMap.OrderBy(x => x.Key).FirstOrDefault(x=>x.Value.Active).Value?.ID;
        }


        public Int32? FoucsID
        {
            get
            {
                return Model.FoucsID;
            }
            set
            {
                if(Model.FoucsID != value)
                {
                    Model.FoucsID = value;
                }
            }
        }

        public SearchItemPrsnt? GetFocusdSearchItemPrsnt => ItemPrsntMap.FirstOrDefault(mp => mp.Value.Focused).Value;


        public PageInfoPrsnt InitSearchPageInfo()
        {
            return new PageInfoPrsnt(Dso);
        }

        internal static readonly Dictionary<SearchType, TriggerType> SearchToTrigger = new Dictionary<SearchType, TriggerType>()
        {
            { SearchType.Edge, TriggerType.Edge },
            { SearchType.Pulse, TriggerType.PulseWidth },
            //{ SearchType.Timeout, TriggerType.TimeOut },
            //{ SearchType.Runt, TriggerType.Runt },
            //{ SearchType.Window, TriggerType.Window },
            //{ SearchType.Transition, TriggerType.Transition },
            ////{ SearchType.Logic, null },
            //{ SearchType.SetupHold, TriggerType.SetupHold },
            //{ SearchType.Auto, null},
        };

        internal static readonly Dictionary<TriggerType, SearchType> TriggerToSearch = new Dictionary<TriggerType, SearchType>()
        {
            { TriggerType.Edge, SearchType.Edge },
            { TriggerType.PulseWidth, SearchType.Pulse },
            //{ TriggerType.TimeOut, SearchType.Timeout },
            //{ TriggerType.Runt, SearchType.Runt },
            //{ TriggerType.Window, SearchType.Window },
            //{ TriggerType.Transition, SearchType.Transition },
            //{ TriggerType.SetupHold, SearchType.SetupHold },
            //{ TriggerType.Video, null },
            //{ TriggerType.State, null},
            //{ TriggerType.Glitch, null },
            //{ TriggerType.Interval, null},
            //{ TriggerType.MultiQulified, null },
            //{ TriggerType.Serial, null},
        };

    }

}
