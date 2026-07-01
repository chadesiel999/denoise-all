using NPOI.OpenXmlFormats.Spreadsheet;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ScopeX.Core
{
    internal class SearchModel :INotifyPropertyChanged
    {
        public static readonly Object Locker = new();

        private readonly ConcurrentDictionary<Int64, SearchItemModel> _Items = new();
        public ConcurrentDictionary<Int64, SearchItemModel> Items
        {
            get
            {
                return _Items;
            }
        }
        public SearchModel(MeasureModel mm)
        {
            _Meas = mm;
            _Items = new();
        }

        private SysState _LastTriggerState = SysState.Reset;

        protected readonly MeasureModel _Meas;

        private Boolean _Enabled = false;
        public Boolean Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled != value)
                {
                    _Enabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _Running = false;
        public Boolean Running
        {
            get => _Running;
            set
            {
                if (_Running != value)
                {
                    _Running = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _SoftSearch = false;
        public Boolean SoftSearch
        {
            get => _SoftSearch;
            set
            {
                if (_SoftSearch != value)
                {
                    _SoftSearch = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<String> OnceSearchItemName
        {
            get => Items.Values.Where(x => x.OnceSearch).Select(x => x.Name).ToList();
        }

        private Int32 _SearchCount = 0;
        public Int32 SearchCount
        {
            get => _SearchCount;
            set
            {
                if (_SearchCount != value)
                {
                    _SearchCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32? _FoucsID = null;

        public Int32? FoucsID
        {
            get
            {
                return _FoucsID;
            }
            set
            {
                if (_FoucsID != value)
                {
                    _FoucsID = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean AddSearch(Int32 id, SearchType type, [NotNullWhen(true)] out SearchItemModel searchItemModel)
        {
            //if (SearchCount >= Constants.MAX_SEARCH_CNT)
            //{
            //    WeakTip.Default.Write("Search", MsgTipId.GreatethanMax);
            //    searchItemModel = null;
            //    return false;
            //}
            lock (Locker)
            {
                SearchItemModel searchItem = new SearchItemModel(AssignSearchName(id));
                searchItem.OnceSearch = true;
                searchItem.DrawColor = Tools.ColorLookup.MakeRandColor();
                //searchItem.Active = true;
                searchItemModel = searchItem;

                switch (type)
                {
                    case SearchType.Edge:
                        searchItem.SearchTypeModel = new SearchEdgeModel();
                        break;
                    case SearchType.Pulse:
                        searchItem.SearchTypeModel = new SearchPulseModel();
                        break;
                    case SearchType.Timeout:
                        searchItem.SearchTypeModel = new SearchTimeoutModel();
                        break;
                    case SearchType.Runt:
                        searchItem.SearchTypeModel = new SearchRuntModel();
                        break;
                    case SearchType.Window:
                        searchItem.SearchTypeModel = new SearchWindowModel();
                        break;
                    case SearchType.Transition:
                        searchItem.SearchTypeModel = new SearchTransitionModel();
                        break;
                    case SearchType.Pattern:
                        break;
                    case SearchType.SetupHold:
                        searchItem.SearchTypeModel = new SearchSetupHoldModel();
                        break;
                    case SearchType.Auto:
                        break;
                    default:
                        return false;
                }

                _Items.TryAdd(searchItem.Id, searchItem);
            }
            return true;
        }

        public Boolean RemoveAll()
        {
            _Items.Clear();
            SearchCount = 0;
            return true;
        }

        public Boolean RemoveSearch(Int64 id)
        {
            if (_Items.ContainsKey(id))
            {
                lock (Locker)
                {
                    _Items.TryRemove(id, out var deletedItem);
                    SearchCount--;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private String AssignSearchName(Int32 ID)
        {
            //Int32 nameIndex = 0;
            //String name = "Search";

            //do
            //{
            //    nameIndex++;
            //    name = "Search" + nameIndex.ToString();
            //} while (_Items.ContainsKey(nameIndex));

            return $"Search{ID}";
        }

        public Boolean Take(CancellationToken ct)
        {
            try
            {
                Dictionary<Int64, (Double[,] Result, Int32 ResultCount)> results = new();
                lock (Locker)
                {
                    if(_LastTriggerState== SysState.Auto&& TriggerPrsnt.State == SysState.Stop)
                    {
                        foreach (var item in _Items.Values)
                        {
                            item.OnceSearch = true;
                        }
                    }

                    if (TriggerPrsnt.State == SysState.Stop && OnceSearchItemName.Count == 0)
                        return false;

                    _LastTriggerState = TriggerPrsnt.State;

                    if (_SoftSearch)
                    {
                        foreach (var item in _Items)
                        {
                            if (item.Value == null || !item.Value.Active)
                            {
                                continue;
                            }

                            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(item.Value.Source, out var prsnt);
                            if (TriggerPrsnt.State!= SysState.Stop||OnceSearchItemName.Contains(item.Value.Name))
                            {
                                List<Double> searchResults = Search(item.Value);
                                if (prsnt != null && prsnt.Sampling is TimebasePrsnt tmb && tmb != null && prsnt.Pack != null && prsnt.Pack.Properties != null)
                                {
                                    item.Value.SearchResultTmbInfo = tmb.GetCurrentTmbInfo();
                                    item.Value.ResultPrefix = prsnt.Pack.Properties.TmbUnit.Prefix;
                                    item.Value.ResultTmbUnitName = prsnt.Pack.Properties.TmbUnit.Name;
                                    if (prsnt != null && prsnt.VuDatabase != null && prsnt.VuDatabase.Current != null)
                                    {
                                        item.Value.ZoomRatio = prsnt.VuDatabase.Current.ZoomRatio;
                                        item.Value.Start = prsnt.VuDatabase.Current.Start;
                                    }
                                }
                                Double[,] packBuffer = new Double[1, searchResults.Count];
                                if (searchResults.Count > 0)
                                    for (Int32 i = 0; i < packBuffer.GetLength(0); i++)
                                    {
                                        for (Int32 j = 0; j < packBuffer.GetLength(1); j++)
                                        {
                                            packBuffer[i, j] = searchResults[j];
                                        }
                                    }
                                results.Add(item.Key, (packBuffer, packBuffer.GetLength(1)));
                            }
                        }
                    }
                    else
                    {
                        if (Enabled)
                        {
                            if (Hd.Search?.TryTakeResult(out var finds) ?? false)
                            {
                                results = finds;
                            }
                        }
                    }

                    foreach (var item in results)
                    {
                        if (_Items.ContainsKey(item.Key))
                        {
                            if (OnceSearchItemName.Count == 0 || OnceSearchItemName.Contains(_Items[item.Key].Name))
                            {
                                _Items[item.Key].SearchTypeModel.ResultPack = item.Value;
                                _Items[item.Key].ResultCount = item.Value.ResultCount;
                                _Items[item.Key].OnceSearch = false;
                                _Items[item.Key].IsNewResult = true;
                            }
                        }
                    }

                }

                return true;
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"{e.Message}\n{e.StackTrace}", EventBus.LogLevel.Debug));
                return false;
            }

            return false;
        }

        public List<Double> Search(SearchItemModel searchItemModel)
        {
            if (DsoPrsnt.DefaultDsoPrsnt.IsScan)
                return new List<Double>();

            Double reflevel = 0;
            Double threshold = 0;
            Double threshold2nd = 0;
            CompareCondition condition = CompareCondition.NotCare;

            String measureName = "";
            switch (searchItemModel.Type)
            {
                case SearchType.Edge:
                {
                    SearchEdgeModel searchEdgeModel = (SearchEdgeModel)searchItemModel.SearchTypeModel;
                    if (searchEdgeModel.Slope == EdgeSlope.Rise)
                    {
                        measureName = "PEdge";
                    }
                    else if (searchEdgeModel.Slope == EdgeSlope.Fall)
                    {
                        measureName = "NEdge";
                    }
                    else
                    {
                        measureName = "Edge";
                    }

                    reflevel = searchEdgeModel.CompPosition;
                    threshold = searchEdgeModel.CompPosition;
                    condition = CompareCondition.NotCare;
                }
                break;
                case SearchType.Pulse:
                {
                    SearchPulseModel searchPulseModel = (SearchPulseModel)searchItemModel.SearchTypeModel;
                    if (searchPulseModel.Polarity == PulsePolarity.Positive)
                    {
                        measureName = "PWidth";
                    }
                    else if (searchPulseModel.Polarity == PulsePolarity.Negative)
                    {
                        measureName = "NWidth";
                    }

                    switch (searchPulseModel.Condition)
                    {
                        case PulseCondition.GreaterThan:
                            condition = CompareCondition.GreaterThan;
                            threshold = Quantity.ConvertByPrefix(searchPulseModel.WidthByps, Prefix.Empty, Prefix.Tera);
                            break;
                        case PulseCondition.LessThan:
                            condition = CompareCondition.LessThan;
                            threshold = Quantity.ConvertByPrefix(searchPulseModel.UpperWidthByps, Prefix.Empty, Prefix.Tera);
                            break;
                        case PulseCondition.Equal:
                            condition = CompareCondition.InRange;
                            threshold = Quantity.ConvertByPrefix(searchPulseModel.WidthByps, Prefix.Empty, Prefix.Tera);
                            threshold2nd = Quantity.ConvertByPrefix(searchPulseModel.UpperWidthByps, Prefix.Empty, Prefix.Tera);
                            break;
                        case PulseCondition.NotEqual:
                            condition = CompareCondition.OutRange;
                            threshold = Quantity.ConvertByPrefix(searchPulseModel.WidthByps, Prefix.Empty, Prefix.Tera);
                            threshold2nd = Quantity.ConvertByPrefix(searchPulseModel.UpperWidthByps, Prefix.Empty, Prefix.Tera);
                            break;
                        default:
                            condition = CompareCondition.NotCare;
                            break;
                    }
                    reflevel = searchPulseModel.CompPosition;
                }
                break;
                case SearchType.Timeout:
                    break;
                case SearchType.Runt:
                    break;
                case SearchType.Window:
                    break;
                case SearchType.Transition:
                    break;
                case SearchType.Pattern:
                    break;
                case SearchType.SetupHold:
                    break;
                case SearchType.Auto:
                    break;
                default:
                    return new List<Double>();
            }

            var (x, y) = _Meas.Calc.GetSearch(new MeasureItemModel(measureName, searchItemModel.Source), reflevel, MeasureGate.Screen);

            if ((y != null && x != null) && y.Count <= x.Count)
            {
                List<Double> result = new();

                switch (condition)
                {
                    case CompareCondition.GreaterThan:
                    {
                        for (Int32 i = 0; i < y.Count; i++)
                        {
                            if (y[i] >= threshold)
                            {
                                result.Add(x[i]);
                            }
                        }
                        return result;
                    }
                    case CompareCondition.LessThan:
                    {
                        for (Int32 i = 0; i < y.Count; i++)
                        {
                            if (y[i] <= threshold)
                            {
                                result.Add(x[i]);
                            }
                        }
                        return result;
                    }
                    case CompareCondition.InRange:
                    {
                        var max = Math.Max(threshold, threshold2nd);
                        var min = Math.Min(threshold, threshold2nd);

                        for (Int32 i = 0; i < y.Count; i++)
                        {
                            if (y[i] < max && y[i] > min)
                            {
                                result.Add(x[i]);
                            }
                        }
                        return result;
                    }
                    case CompareCondition.OutRange:
                    {
                        var max = Math.Max(threshold, threshold2nd);
                        var min = Math.Max(threshold, threshold2nd);

                        for (Int32 i = 0; i < y.Count; i++)
                        {
                            if (y[i] > max && y[i] < min)
                            {
                                result.Add(x[i]);
                            }
                        }
                        return result;
                    }
                    case CompareCondition.NotCare:
                    {
                        for (Int32 i = 0; i < y.Count; i++)
                        {
                            result.Add(x[i]);
                        }
                        return result;
                    }
                    default:
                        break;
                }
            }
            return new List<Double>();
        }

        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
