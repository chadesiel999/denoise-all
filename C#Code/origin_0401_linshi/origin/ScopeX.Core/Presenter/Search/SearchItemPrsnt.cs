using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    public class SearchItemPrsnt : MulticastPrsnt<ISearchItemView>, ISearchItemPrsnt
    {
        internal SearchItemPrsnt(IDsoPrsnt idp, SearchItemModel m, SearchPrsnt prsnt, ISearchItemView? view = null) : base(idp)
        {
            Parent = prsnt;
            ItemModel = m;
            ItemModel.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
            _Type = Model.Type;
            _SearchTypePrsnt = GetSearchPrsnt(Model.Type);
        }

        private SearchPrsnt Parent { get; set; }

        private protected SearchItemModel ItemModel { get; }

        private protected override SearchItemModel Model => ItemModel;

        public Boolean Focused
        {
            get => Model.Focused;
            set
            {
                Model.Focused = value;
                //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }


        public Boolean Active
        {
            get => Model.Active;
            set
            {
                if (Model.Active != value)
                {
                    Model.Active = value;
                    Model.SearchTypeModel.ResultPack = null;
                    Parent?.SearchItemActive(ID, value);
                }
            }
        }

        public Boolean Visible
        {
            get => Model.Visible;
            set
            {
                Model.Visible = value;
            }
        }

        public List<String> KeyInfos
        {
            get
            {
                return Model.KeyInfos;
            }
        }

        public Boolean EventEnable
        {
            get => Model.EventEnable;
            set
            {
                Model.EventEnable = value;
                //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }

        public String Name
        {
            get => Model.Name;
            set => Model.Name = value;
        }

        public ChannelId Source
        {
            get => Model.Source;
            set
            {
                Model.Source = value;
                //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }

        public Boolean OnceSearch
        {
            get => Model.OnceSearch;
            set
            {
                Model.OnceSearch = value;
            }
        }

        public Int32 ResultCount
        {
            get => Model.ResultCount;
            set => Model.ResultCount = value;
        }

        public Int32 ID
        {
            get => Model.Id;
        }

        public Color DrawColor
        {
            get => Model.DrawColor;
            set => Model.DrawColor = value;
        }

        private ISearchTypePrsnt _SearchTypePrsnt;
        public ISearchTypePrsnt SearchTypePrsnt
        {
            get
            {
                lock (this)
                {
                    if (_Type == Model.Type)
                    {
                        return _SearchTypePrsnt;
                    }
                    if (_SearchTypePrsnt is ITriggerPrsnt tp)
                    {
                        tp.DisposeEvent();
                    }

                    _SearchTypePrsnt = GetSearchPrsnt(Model.Type);
                    _Type = Model.Type;
                    return _SearchTypePrsnt;
                }
            }
        }

        public String GetTypeDescription()
        {
            String des = $"Search.{_Type}.";
            switch (_Type)
            {
                default:
                case SearchType.Edge:
                    des += (SearchTypePrsnt as SearchEdgePrsnt).Slope;
                    break;
                case SearchType.Pulse:
                    des += (SearchTypePrsnt as SearchPulsePrsnt).Polarity;
                    break;
            }
            return des;
        }

        private SearchType _Type;
        public SearchType Type
        {
            get => Model.Type;
            set
            {
                lock (this)
                {
                    if (Model.Type != value)
                    {
                        Model.Type = value;
                        if (_SearchTypePrsnt is ITriggerPrsnt tp)
                        {
                            tp.DisposeEvent();
                        }
                        _SearchTypePrsnt = GetSearchPrsnt(value);
                        _Type = Model.Type;
                        //Hardware.HdCmdFactory.Push(HdCmd.Search);
                    }
                }
            }
        }

        private ISearchTypePrsnt GetSearchPrsnt(SearchType type)
        {
            switch (type)
            {
                case SearchType.Edge:
                    return new SearchEdgePrsnt(Dso, (SearchEdgeModel)Model.SearchTypeModel);
                case SearchType.Pulse:
                    return new SearchPulsePrsnt(Dso, (SearchPulseModel)Model.SearchTypeModel);
                case SearchType.Timeout:
                    return new SearchTimeoutPrsnt(Dso, (SearchTimeoutModel)Model.SearchTypeModel);
                case SearchType.Runt:
                    return new SearchRuntPrsnt(Dso, (SearchRuntModel)Model.SearchTypeModel);
                case SearchType.Window:
                    return new SearchWindowPrsnt(Dso, (SearchWindowModel)Model.SearchTypeModel);
                case SearchType.Transition:
                    return new SearchTransitionPrsnt(Dso, (SearchTransitionModel)Model.SearchTypeModel);
                case SearchType.Pattern:
                    break;
                case SearchType.SetupHold:
                    return new SearchSetupHoldPrsnt(Dso, (SearchSetupHoldModel)Model.SearchTypeModel);
                case SearchType.Auto:
                    break;
                default:
                    break;
            }
            return new SearchEdgePrsnt(Dso, (SearchEdgeModel)Model.SearchTypeModel);
        }

        public void SetModelValue(String name, Object value)
        {
            Type type = Model.SearchTypeModel.GetType();

            while (type != null)
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                // 设置字段的新值
                foreach (FieldInfo field in fields)
                {
                    if (field.Name == name)
                    {
                        field.SetValue(Model.SearchTypeModel, value);
                        OnPropertyChanged(this, new PropertyChangedEventArgs(name));
                        return;
                    }
                }

                PropertyInfo property = type.GetProperty(name);
                if (property != null)
                {
                    property.SetValue(Model.SearchTypeModel, value);
                    OnPropertyChanged(this, new PropertyChangedEventArgs(name));
                    break;
                }

                if (type != type.BaseType)
                {
                    type = type.BaseType;
                }
                else
                {
                    break;
                }

            }
        }
        public Boolean IsNewResult { get => Model.IsNewResult; set => Model.IsNewResult = value; }
        public (Double PositionByus, Double PosIndex, Double Scale, Double PosIdxPerDiv) SearchResultTmbInfo { get => Model.SearchResultTmbInfo; }
        public Prefix ResultPrefix { get => Model.ResultPrefix; }
        public String ResultTmbUnitName { get => Model.ResultTmbUnitName; }
        public Double ZoomRatio { get => Model.ZoomRatio; }
        public Double Start { get => Model.Start; }

        public Double[,]? GetSearchResults()
        {
            if (Model.SearchTypeModel.ResultPack != null)
            {
                var (result, resultCount) = Model.SearchTypeModel.ResultPack!.Value;
                return result;
            }
            return null;
        }

        public List<SearchItemResult>? GetSearchResultsByTmb()
        {
            var results = new List<SearchItemResult>();
            var buffer = Model.SearchTypeModel.ResultPack!.Value.Result;
            if (buffer == null)
            {
                return null;
            }
            var source = DsoModel.Default.AnalogChnls.FirstOrDefault(x => x.Id == Source);
            if (source == null || source.Pack == null)
            {
                return null;
            }
            Double delta = TransToTmb(buffer[0, 0]);
            var index = 0;
            for (Int32 ii = 0; ii < buffer.GetLongLength(0); ii++)
            {
                for (Int32 jj = 0; jj < buffer.GetLongLength(1); jj++)
                {
                    var position = TransToTmb(buffer[ii, jj]);
                    delta = position - delta;
                    var deltabys = Quantity.ConvertByPrefix(delta, ResultPrefix, Prefix.Empty).ToString("E13");
                    var positionbys = Quantity.ConvertByPrefix(position, ResultPrefix, Prefix.Empty).ToString("E13");
                    results.Add(new SearchItemResult(index, Type, positionbys, deltabys, Type));
                    delta = position;
                    index++;
                }
            }

            return results;
        }

        public Double TransToTmb(Double position)
        {
            Double value = position / ZoomRatio + Start;

            return (value - SearchResultTmbInfo.PosIndex) * SearchResultTmbInfo.Scale / SearchResultTmbInfo.PosIdxPerDiv;
        }

        public Boolean ReadFromTrigger()
        {
            try
            {
                var flag = false;
                if (SearchPrsnt.TriggerToSearch.ContainsKey(TriggerModel.Type) == false)
                    return false;

                if (SearchPrsnt.TriggerToSearch[TriggerModel.Type] == Type)
                {
                    flag = SearchTypePrsnt?.ReadFromTrigger() == true;
                    OnPropertyChanged(this, new PropertyChangedEventArgs(String.Empty));
                    return flag;
                }

                Type = (SearchType)SearchPrsnt.TriggerToSearch[TriggerModel.Type];
                flag = SearchTypePrsnt?.ReadFromTrigger() == true;
                OnPropertyChanged(this, new PropertyChangedEventArgs(String.Empty));
                return flag;
            }
            catch (Exception e)
            {

                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"{e.Message}\n{e.StackTrace}", EventBus.LogLevel.Debug));
                return false;
            }
        }

        public Boolean SetToTrigger()
        {
            try
            {
                if (SearchPrsnt.SearchToTrigger.ContainsKey(Type) == false)
                    return false;
                if (SearchPrsnt.SearchToTrigger[Type] != TriggerPrsnt.Type)
                {
                    TriggerPrsnt.GetOrMakeTrigger(Dso, SearchPrsnt.SearchToTrigger[Type]);
                    SearchTypePrsnt?.LoadTriggerPrsnt();
                }
                return SearchTypePrsnt?.SetToTrigger() == true;
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"{e.Message}\n{e.StackTrace}", EventBus.LogLevel.Debug));
                return false;
            }
        }

        public record SearchItemResult(Int32 Index, SearchType Type, String PositionBys, String DeltaBys, SearchType Description);
    }
}
