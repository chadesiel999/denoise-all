// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using ScopeX.ComModel;


    internal class TriggerMultiQualifiedModel : TriggerModel
    {
        public override String Name => TriggerType.MultiQulified.ToString();

        private List<(TriggerModel Node, TriggerPathwayInfo Pathway)> Events
        {
            get;
        } = new();

        public Int32 Count => Events.Count;

        public void AddEvent(TriggerModel node)
        {
            Events.Add(new(node, new()));
        }

        public void RemoveEvent(Int32 index)
        {
            if (index >= 0 && index < Events.Count)
            {
                Events.RemoveAt(index);
            }
        }

        public void ClearEvent()
        {
            Events.Clear();
        }

        public (TriggerModel Node, TriggerPathwayInfo Pathway) this[Int32 index]
        {
            get => Events[index];
            set => Events[index] = value;
        }

        public Int32 FindIndex(Predicate<TriggerModel> match)
        {
            return Events.FindIndex((o) => match(o.Node));
        }

        public override void LeapPosIndex()
        {
            Events.ForEach((o) => o.Node.LeapPosIndex());
        }
    }

    internal class TriggerPathwayInfo : INotifyPropertyChanged
    {
        private DelayOpt _DelayType = DelayOpt.Time;
        public DelayOpt DelayType
        {
            get => _DelayType;
            set
            {
                if (value != _DelayType)
                {
                    _DelayType = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int64 _DurationByps = 10;
        public Int64 DurationByps
        {
            get => _DurationByps;
            set
            {
                if (value > MaxDuration)
                {
                    value = MaxDuration;
                }
                else if (value < MinDuration)
                {
                    value = MinDuration;
                }

                if (value != _DurationByps)
                {
                    _DurationByps = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int64 MaxDuration
        {
            get;
            init;
        } = Constants.MAX_MULTISTAGE_TIME_PS;

        public Int64 MinDuration
        {
            get;
            init;
        } = Constants.MIN_MULTISTAGE_TIME_PS;

        private Int32 _EventCounts = Constants.MIN_MULTISTAGE_EVENT;
        public Int32 EventCounts
        {
            get => _EventCounts;
            set
            {
                if (value > MaxEventCounts)
                {
                    value = MaxEventCounts;
                }
                else if (value < MinEventCounts)
                {
                    value = MinEventCounts;
                }

                if (value != _EventCounts)
                {
                    _EventCounts = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int32 MaxEventCounts
        {
            get;
            init;
        } = Constants.MAX_MULTISTAGE_EVENT;

        public Int32 MinEventCounts
        {
            get;
            init;
        } = Constants.MIN_MULTISTAGE_EVENT;


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
