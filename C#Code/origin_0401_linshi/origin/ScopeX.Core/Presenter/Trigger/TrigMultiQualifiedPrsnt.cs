// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using ScopeX.ComModel;

    /// <summary>
    /// Defines the <see cref="TrigMultiQualifiedPrsnt" />.
    /// </summary>
    public class TrigMultiQualifiedPrsnt : TriggerPrsnt
    {
        /// <summary>
        /// Defines the QualifiedType.
        /// </summary>
        public readonly ImmutableList<TriggerType> QualifiedType = new List<TriggerType>()
        {
            TriggerType.Edge,
            TriggerType.PulseWidth,
            TriggerType.Glitch,
            TriggerType.Transition,
            TriggerType.Runt,
            TriggerType.Window,
            TriggerType.TimeOut,

        }.ToImmutableList();

        /// <summary>
        /// Initializes a new instance of the <see cref="TrigMultiQualifiedPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView?"/>.</param>
        public TrigMultiQualifiedPrsnt(IDsoPrsnt idp, ITriggerView? view = null) : base(idp, view)
        {
            _Model = (TriggerMultiQualifiedModel)DsoModel.Default.GetTriggerModel(TriggerType.MultiQulified);
            LoadEvent();
        }

        /// <summary>
        /// 重载参数
        /// </summary>
        public override void LoadEvent()
        {
            if (Model != null)
            {
                Model.PropertyChanged += OnPropertyChanged;
            }
        }

        /// <summary>
        /// 切换类型，注销事件
        /// </summary>
        public override void DisposeEvent()
        {
            if (Model != null)
            {
                Model.PropertyChanged -= OnPropertyChanged;
            }
        }

        /// <summary>
        /// Gets the Count.
        /// </summary>
        public Int32 Count => Model.Count;

        /// <summary>
        /// Gets or sets the PosIndex.
        /// </summary>
        public override Double PosIndex
        {
            get => 0;
            set { }
        }

        /// <summary>
        /// Gets the Model.
        /// </summary>
        private protected override TriggerMultiQualifiedModel Model => (TriggerMultiQualifiedModel)_Model!;

        /// <summary>
        /// The AddEvent.
        /// </summary>
        /// <param name="tt">The tt<see cref="TriggerType"/>.</param>
        /// <returns>The <see cref="Boolean"/>.</returns>
        public Boolean AddEvent(TriggerType tt)
        {
            switch (tt)
            {
                case TriggerType.Edge:
                    Model.AddEvent(new TriggerEdgeModel());
                    return true;
                case TriggerType.PulseWidth:
                    Model.AddEvent(new TriggerWidthModel());
                    return true;
                case TriggerType.Glitch:
                    Model.AddEvent(new TriggerGlitchModel());
                    return true;
                case TriggerType.Transition:
                    Model.AddEvent(new TriggerTransModel());
                    return true;
                case TriggerType.Runt:
                    Model.AddEvent(new TriggerRuntModel());
                    return true;
                case TriggerType.Window:
                    Model.AddEvent(new TriggerWindowModel());
                    return true;
                case TriggerType.TimeOut:
                    Model.AddEvent(new TriggerTimeOutModel());
                    return true;
            }
            return false;
        }

        /// <summary>
        /// The ClearEvent.
        /// </summary>
        public void ClearEvent()
        {
            Model.ClearEvent();
        }

        /// <summary>
        /// The GetEvent.
        /// </summary>
        /// <param name="index">The index<see cref="Int32"/>.</param>
        /// <param name="vu">The vu<see cref="ITriggerView"/>.</param>
        /// <returns>The <see cref="TriggerPrsnt"/>.</returns>
        public TriggerPrsnt GetEvent(Int32 index, ITriggerView vu)
        {
            return Model[index].Node switch
            {
                TriggerEdgeModel tem => new TrigEdgePrsnt(Dso, vu, tem),
                TriggerGlitchModel tgm => new TrigGlitchPrsnt(Dso, vu, tgm),
                TriggerWidthModel tpwm => new TrigWidthPrsnt(Dso, vu, tpwm),
                TriggerTransModel ttm => new TrigTransPrsnt(Dso, vu, ttm),
                TriggerRuntModel trm => new TrigRuntPrsnt(Dso, vu, trm),
                TriggerWindowModel twm => new TrigWindowPrsnt(Dso, vu, twm),
                TriggerTimeOutModel ttom => new TrigTimeOutPrsnt(Dso, vu, ttom),
                _ => throw new InvalidCastException(),
            };
        }

        /// <summary>
        /// The GetEventType.
        /// </summary>
        /// <param name="index">The index<see cref="Int32"/>.</param>
        /// <returns>The <see cref="TriggerType"/>.</returns>
        public TriggerType GetEventType(Int32 index)
        {
            return Model[index].Node switch
            {
                TriggerEdgeModel => TriggerType.Edge,
                TriggerGlitchModel => TriggerType.Glitch,
                TriggerWidthModel => TriggerType.PulseWidth,
                TriggerTransModel => TriggerType.Transition,
                TriggerRuntModel => TriggerType.Runt,
                TriggerWindowModel => TriggerType.Window,
                TriggerTimeOutModel => TriggerType.TimeOut,
                _ => throw new InvalidCastException(),
            };
        }

        public Int32 GetEventTypeIndex(Int32 index)
        {
            return Model[index].Node switch
            {
                TriggerEdgeModel => 0,
                TriggerGlitchModel => 2,
                TriggerWidthModel => 1,
                TriggerTransModel => 3,
                TriggerRuntModel => 4,
                TriggerWindowModel => 5,
                TriggerTimeOutModel => 6,
                _ => throw new InvalidCastException(),
            };
        }


        /// <summary>
        /// The GetPathway.
        /// </summary>
        /// <param name="index">The index<see cref="Int32"/>.</param>
        /// <param name="vu">The vu<see cref="ITriggerView"/>.</param>
        /// <returns>The <see cref="TrigPathwayPrsnt"/>.</returns>
        public TrigPathwayPrsnt GetPathway(Int32 index, ITriggerView vu)
        {
            return new(Dso, vu, Model[index].Pathway);
        }

        /// <summary>
        /// The RemoveEvent.
        /// </summary>
        /// <param name="index">The index<see cref="Int32"/>.</param>
        public void RemoveEvent(Int32 index)
        {
            if (index >= 0 && index < Count)
            {
                Model.RemoveEvent(index);
            }
        }

        /// <summary>
        /// The ResetPosIndex.
        /// </summary>
        public override void ResetPosIndex()
        {
            ;
        }

        /// <summary>
        /// The SetEvent.
        /// </summary>
        /// <param name="index">The index<see cref="Int32"/>.</param>
        /// <param name="tt">The tt<see cref="TriggerType"/>.</param>
        /// <returns>The <see cref="Boolean"/>.</returns>
        public Boolean SetEvent(Int32 index, TriggerType tt)
        {
            if (index >= 0 && index < Count)
            {
                if (Model[index].Node.Name != tt.ToString())
                {
                    switch (tt)
                    {
                        case TriggerType.Edge:
                            Model[index] = (new TriggerEdgeModel(), Model[index].Pathway);
                            break;
                        case TriggerType.PulseWidth:
                            Model[index] = (new TriggerWidthModel(), Model[index].Pathway);
                            break;
                        case TriggerType.Glitch:
                            Model[index] = (new TriggerGlitchModel(), Model[index].Pathway);
                            break;
                        case TriggerType.Transition:
                            Model[index] = (new TriggerTransModel(), Model[index].Pathway);
                            break;
                        case TriggerType.Runt:
                            Model[index] = (new TriggerRuntModel(), Model[index].Pathway);
                            break;
                        case TriggerType.Window:
                            Model[index] = (new TriggerWindowModel(), Model[index].Pathway);
                            break;
                        case TriggerType.TimeOut:
                            Model[index] = (new TriggerTimeOutModel(), Model[index].Pathway);
                            break;
                        default:
                            return false;
                    }
                }
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Defines the <see cref="TrigPathwayPrsnt" />.
    /// </summary>
    public class TrigPathwayPrsnt : MulticastPrsnt<ITriggerView>, ITriggerPrsnt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrigPathwayPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView"/>.</param>
        /// <param name="tpi">The tpi<see cref="TriggerPathwayInfo"/>.</param>
        internal TrigPathwayPrsnt(IDsoPrsnt idp, ITriggerView view, TriggerPathwayInfo tpi) : base(idp)
        {
            Model = tpi;
            LoadEvent();
        }
        /// <summary>
        /// 重载参数
        /// </summary>
        public virtual void LoadEvent()
        {
            if (Model != null)
            {
                Model.PropertyChanged += OnPropertyChanged;
            }
        }

        /// <summary>
        /// 切换类型，注销事件
        /// </summary>
        public virtual void DisposeEvent()
        {
            if (Model != null)
            {
                Model.PropertyChanged -= OnPropertyChanged;
            }
        }

        /// <summary>
        /// Gets or sets the Counts.
        /// </summary>
        public Int32 EventCounts
        { 
            get => Model.EventCounts;
            set
            {
                Model.EventCounts = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
         }

    /// <summary>
    /// Gets or sets the DelayType.
    /// </summary>
    public DelayOpt DelayType
        {
            get => Model.DelayType;
            set
            {
                Model.DelayType = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets or sets the DurationByps.
        /// </summary>
        public Int64 DurationByps
        { get => Model.DurationByps;
            set
            {
                Model.DurationByps = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets the MaxCounts.
        /// </summary>
        public Int64 MaxEventCounts => Model.MaxEventCounts;

        /// <summary>
        /// Gets the MaxDuration.
        /// </summary>
        public Int64 MaxDuration => Model.MaxDuration;

        /// <summary>
        /// Gets the MinCounts.
        /// </summary>
        public Int64 MinEventCounts => Model.MinEventCounts;

        /// <summary>
        /// Gets the MinDuration.
        /// </summary>
        public Int64 MinDuration => Model.MinDuration;

        /// <summary>
        /// Gets the Model.
        /// </summary>
        private protected override TriggerPathwayInfo Model { get; }
    }
}
