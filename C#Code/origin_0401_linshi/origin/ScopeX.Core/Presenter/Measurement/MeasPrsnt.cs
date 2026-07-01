using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.Measure;

namespace ScopeX.Core
{
    public class MeasPrsnt : MulticastPrsnt<IView>, IMeasPrsnt
    {
        private protected override MeasureModel Model
        {
            get;
        }

        public MeasPrsnt(IDsoPrsnt idp, IMeasView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            Model = mco switch
            {
                ModelCreateOptions.Dependant => DsoModel.Default.Meas,
                ModelCreateOptions.Standalone => new(),
                _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
            };
            Model.PropertyChanged += OnPropertyChanged;

            var mip = new MeasItemPrsnt[Length];
            for (Int32 i = 0; i < Length; i++)
            {
                mip[i] = new MeasItemPrsnt(Model.SelectedItems[i], this);
                LastChangedItem = mip[i];
            }
            SelectedItems = Array.AsReadOnly(mip);

            if (view is not null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public Boolean IsStatActive
        {
            get => Model.IsStatActive;
            set => Model.IsStatActive = value;
        }

        public Boolean Active
        {
            get => Model.Active;
            set
            {
                if (!Constants.ENABLE_Measure && value)
                {
                    WeakTip.Default.Write("Measure", MsgTipId.FunctionDisabled);
                    Model.Active = false;
                    return;
                }
                if (value && !IsAnyActive())
                {
                    if (SelectedItems[0].Source.IsReference())//如果是参考通道需要检查参考通道是否还在
                    {
                        if (!DsoPrsnt.DefaultDsoPrsnt.GetAllChnls().Any(x => x.Active && x.Id == SelectedItems[0].Source))
                        {
                            SelectedItems[0].Source = ChannelId.C1;//默认切换到C1
                        }
                    }
                    if (SelectedItems[0].Source.IsMeasure())
                    {
                        SelectedItems[0].Source = ChannelId.C1;//默认切换到C1
                    }
                    SelectedItems[0].MeasureType = MeasureType.Single;
                    SelectedItems[0].Active = true;
                }
                if (!value && Model.Active != value && IsAnyActive())
                {
                    var measitemids = Model.SelectedItems.Where(x => x.Active).Select(x => x.Id);
                    var activemchs = DsoPrsnt.DefaultDsoPrsnt.TryGetRange(c => c.Active && c.Id.IsMath());
                    if (activemchs.Any())
                    {
                        var meas = activemchs.Where(x =>
                        {
                            if (x is MathPrsnt math && math.Args.Occupier == null)
                            {
                                if (math.Args is MathHistArg hist)
                                {
                                    return measitemids.Contains(hist.Source);
                                }
                                if (math.Args is MathTrackArg track)
                                {
                                    return measitemids.Contains(track.Source);
                                }
                                if (math.Args is MathTrendArg trend)
                                {
                                    return measitemids.Contains(trend.Source);
                                }
                            }

                            return false;
                        }).ToList();
                        meas.ForEach(x => x.Active = false);
                    }
                }
                Model.Active = value;
            }
        }

        public MeasureGate Strobe
        {
            get => Model.Strobe;
            set
            {
                Model.Strobe = value;
                //!!!Properties Interaction REF#CursorPrsnt.cs
                if (Model.Strobe == MeasureGate.Cursor)
                {
                    DsoModel.Default.Cursors.Active = true;
                    DsoModel.Default.Cursors.Type = CursorType.Vertical;
                    DsoModel.Default.Cursors.HCursor.Source = DsoModel.Default.Cursors.VCursor.Source = DsoPrsnt.FocusId;
                    KeyLed.Default.SetLed(LedEnum.LedCursor, DsoModel.Default.Cursors.Active);
                    var id = DsoModel.Default.Cursors.Type == CursorType.Horizontal ? DsoModel.Default.Cursors.HCursor.SelectedIndex : DsoModel.Default.Cursors.VCursor.SelectedIndex;
                    KeyLed.Default.SetLed(LedEnum.LedMultipupose, DsoModel.Default.Cursors.Active && id != -1);
                }
                else
                {
                    DsoPrsnt.DefaultDsoPrsnt.Cursor.Active = false;
                }
            }
        }

        public Boolean StopMeasure
        {
            get => Model.StopMeasure;
            set
            {
                Model.StopMeasure = value;
            }
        }

        public Int32 Indicator
        {
            get => Model.Indicator;
            set => Model.Indicator = value;
        }

        public Boolean SnapshotActive
        {
            get => Model.SnapshotActive;
            set
            {
                if (value && !Constants.ENABLE_Measure)
                {
                    WeakTip.Default.Write("Measure", MsgTipId.FunctionDisabled);
                    Model.SnapshotActive = false;
                    return;
                }
                Model.SnapshotActive = value;
            }
        }

        public ChannelId SnapshotSource
        {
            get => Model.SnapshotSource;
            set => Model.SnapshotSource = value;
        }

        /// <summary>
        /// 参数测量清除标志 
        /// 临时解决方案 2023.08.50 HChen
        /// </summary>
        public Boolean ClearFlag { get; set; } = false;
        public Boolean ClearHisFlag { get; set; } = false;
        public Boolean ClearStrongFlag { get; set; } = false;

        public Color SnapshotColor => DsoModel.Default.GetChannel(SnapshotSource).DrawColor;

        //public Boolean SnapshotSrcActive => DsoModel.Default.GetChannel(SnapshotSource).Active;

        public Dictionary<String, String> SnapShotResult => Model.SnapShotResult;

        public ImmutableDictionary<String, Func<Double?, Double?, Double?>> ExtCalcName => Model.ExtCalcName;

        public ReadOnlyDictionary<String, String>? ScpiNameTable
        {
            get;
            set;
        }

        //protected readonly MeasItemPrsnt[] SelectedItems;

        public readonly IList<MeasItemPrsnt> SelectedItems;


        public MeasItemPrsnt this[Int32 index] => SelectedItems[index];

        public MeasItemPrsnt LastChangedItem = null;

        public Int32 Length => Model.SelectedItems.Length;

        public void SetAllActive(Boolean active)
        {
            for (Int32 i = 0; i < Length; i++)
            {
                SelectedItems[i].Active = active;
                Model.Calc.ClearStat(i);
            }
        }

        //private void SetAllVisiable(Boolean visiable)
        //{
        //    for (Int32 i = 0; i < Length; i++)
        //    {
        //        SelectedItems[i].Visiable = visiable;
        //    }
        //}

        public Boolean IsAllActive() => SelectedItems.All((item) => item.Active);

        public Boolean IsAnyActive() => SelectedItems.Any((item) => item.Active);

        public Double? GetOrCalcResult(String name, ChannelId source, ChannelId destination = ChannelId.C2) => Model.Calc.GetResultOrCalc(name, source, destination);

        public (List<(Double, Double)>, Double? @top, Double? @base) CalcPwrSwtichLossParas(ChannelId source) => Model.Calc.CalcPwrSwtichLossParas(source);

        public Double? CalcResultNow(String name, ChannelId source, ChannelId destination = ChannelId.C2) => Model.Calc.ForceGetResultOrCalc(name, source, destination);

        /// <summary>
        /// Added by lihuijun
        /// </summary>
        /// <param name="name"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        /// <remarks>
        /// 扩展测量计算，测量结果携带单位
        /// </remarks>
        public (Double Result, Prefix Prefix, String UnitTxt)? CalcResultNowWithUnit(String name, ChannelId source, ChannelId destination = ChannelId.C2)
            => Model.Calc.ForceGetResultOrCalcWithUnit(name, source, destination);

        /// <summary>
        /// 参数快照
        /// </summary>
        /// <param name="name"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public String CalcResultStringNow(String name, ChannelId source, ChannelId destination = ChannelId.C2)
        {
            var value = Model.Calc.ForceGetResultOrCalc(name, source, destination, Model.Strobe);
            return MeasureProc.GetPfxUnitString(name, source, value);
        }
        public String CalcResultStringNow(Int32 index, ChannelId source, ChannelId destination = ChannelId.C2) => MeasureProc.GetPfxUnitString(Model.SelectedItems[index].Name, source, GetResult(index));

        public String CalcResultStringNow(Double value, Prefix prefix, String unit) => MeasureProc.GetPfxUnitString(value, prefix, unit);

        public static (Prefix Prefix, String Unit) GetPfxUnitString(String name, ChannelId source) => MeasureProc.GetPfxUnitString(name, source);

        public static String GetPfxUnitString(String name, ChannelId source, Double? value) => MeasureProc.GetPfxUnitString(name, source, value);

        public (List<Double>? x, List<Double>? y) GetTrack(String name, ChannelId source, ChannelId destination = ChannelId.C2, MeasureGate strobe = MeasureGate.Screen) => Model.Calc.GetTrack(name, source, destination, strobe);

        //public Double? GetResult(Int32 index) => Model.Calc.GetResult(Model.SelectedItems[index]);

        public Double? GetResult(Int32 index)
        {
            var res = Model.Calc.GetResult(Model.SelectedItems[index]);
            //if (res is null)
            //{
            //    return GetResult(0, MathBinaryType.Add, 1);
            //}
            return res;
        }

        public Boolean GetIndicatorStates(String name) => Model.Calc.GetIndicatorStates(name);

        public (Prefix Prefix, String Unit) GetPfxUnitString(Int32 index) => Model.Calc.GetPfxUnitString(index);

        public Double? GetStatAverage(Int32 index) => Model.Calc.StatBuffer[index].Average;

        public Double? GetStatMax(Int32 index) => Model.Calc.StatBuffer[index].Max;

        public Double? GetStatMin(Int32 index) => Model.Calc.StatBuffer[index].Min;

        public Double? GetStatStddev(Int32 index) => Model.Calc.StatBuffer[index].Stddev;

        public Int32 GetStatCount(Int32 index) => Model.Calc.StatBuffer[index].Count;

        public Boolean CalcSnapshotAllResult() => Model.CalcSnapshotAllResult();

        public void ResetStat(Int32 index)
        {
            var maths = DsoModel.Default.MathChnls.Where(x => x.Args is MathTrendArg arg && arg.Source - ChannelId.P1 == index).ToList();
            maths?.ForEach(x => x.ClearFlag = true);
            Model.Calc.StatBuffer[index].Clear();
        }

        public void ResetAllStats()
        {           
            Model.Calc.ClearAllStat();
        }

        public (List<Double>? x, List<Double>? y) GetTrack(Int32 index, MeasureGate strobe = MeasureGate.Screen) => Model.Calc.GetTrack(Model.SelectedItems[index], strobe);

        public List<String> SnapShotDataTable => Model.SnapShotDataTable;

        #region Indicator
        public (List<Double>?, List<Double>?) GetIndicator(Int32 index) => Model.Calc.GetIndicator(Model.SelectedItems[index]);
        #endregion

        //public Double? GetResult(Int32 index1, MathBinaryType mbt, Int32 index2)
        //{
        //    return mbt switch
        //    {
        //        MathBinaryType.Add => Model.Calc.GetResult(Model.SelectedItems[index1]) + Model.Calc.GetResult(Model.SelectedItems[index2]),
        //        MathBinaryType.Subtract => Model.Calc.GetResult(Model.SelectedItems[index1]) - Model.Calc.GetResult(Model.SelectedItems[index2]),
        //        MathBinaryType.Multiply => Model.Calc.GetResult(Model.SelectedItems[index1]) * Model.Calc.GetResult(Model.SelectedItems[index2]),
        //        MathBinaryType.Divide => Model.Calc.GetResult(Model.SelectedItems[index1]) / Model.Calc.GetResult(Model.SelectedItems[index2]),
        //        _ => null,
        //    };
        //}

    }
}
