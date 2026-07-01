using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    public delegate Object? PrepareHandler(Boolean init, ChannelId id, CancellationToken ct, DataRole dataRole = DataRole.View, CancellationToken? softResetToken = null);
    public delegate (T Buffer, Object Prop)? ReadHandler<T>(Object? arg);
    public delegate (T Buffer, T2 Buffer2, Object Prop)? ReadHandler<T, T2>(Object? arg);
    public delegate WfmPack ProcessHandler<T>((T Buffer, Object Prop) pkg, Object? arg);



    internal class ChannelShareParameter : INotifyPropertyChanged
    {
        private ChannelId _FocusId = ChannelId.C1;
        public ChannelId FocusId
        {
            get => _FocusId;
            set
            {
                if (_FocusId != value)
                {
                    _FocusId = value;

                    OnPropertyChanged();
                    KeyLed.Default.SetFocusChannel(value);
                }
            }
        }

        #region INotifyPropertyChanged
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
        #endregion

        private ChannelShareParameter()
        { }

        public static readonly ChannelShareParameter Default = new();
    }

    internal abstract class ChannelModel : INotifyPropertyChanged
    {
        public static ChannelId FocusId
        {
            get => ChannelShareParameter.Default.FocusId;
            set => ChannelShareParameter.Default.FocusId = value;
        }

        public ChannelType Type
        {
            get;
            init;
        }

        public ChannelId Id
        {
            get;
            init;
        }

        public String Name
        {
            get;
        }

        private Color _DrawColor;
        public Color DrawColor
        {
            get => _DrawColor;
            set
            {
                if (_DrawColor != value)
                {
                    _DrawColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _Active = false;
        public virtual Boolean Active
        {
            get => _Active;
            set
            {
                if (_Active != value)
                {
                    if (Id.IsDecode())
                    {
                        if (value && FunctionLimit.DecodeFunctionLimit(DsoPrsnt.DefaultDsoPrsnt?.MutexFunctionFlag ?? false) == false)
                        {
                            return;
                        }
                    }

                    #region 抖动相关

                    //if (DsoPrsnt.DefaultDsoPrsnt != null && DsoPrsnt.DefaultDsoPrsnt.Jitter.Active && DsoPrsnt.DefaultDsoPrsnt.Jitter.Source == this.Id && !value)
                    if ((DsoModel.Default?.JitterModel.Active ?? false) && DsoModel.Default.JitterModel.Source == this.Id && !value)
                    {
                        if (!DsoPrsnt.IsDsoClosing && !StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.JitterSourceClose, MessageType.Warning))
                        {
                            OnPropertyChanged();
                            return;
                        }
                        else
                        {
                            DsoModel.Default.JitterModel.Active = false;
                        }
                    }

                    #endregion


                    _Active = value;

                    AdcInterleaveProcessor.Default.Process();

                    OnPropertyChanged();

                    if (!value)
                    {
                        if (Id.IsAnalog())
                        {
                            ChannelZIndex.Default.Add(Id);

                            foreach (var item in DsoModel.Default.VisualTrigger.SelectedItems)
                            {
                                if (item.Source == Id)
                                {
                                    item.Enabled = false;
                                }
                            }
                        }
                        else
                        {
                            ChannelZIndex.Default.Remove(Id);
                        }
                        if (DsoPrsnt.FocusId == Id)
                        {
                            DsoPrsnt.DefaultDsoPrsnt?.MoveFocusId();
                        }
                        else if (Id.IsAnalog() || Id.IsReference() || Id.IsBaseMath())
                        {
                            DsoPrsnt.DefaultDsoPrsnt?.SetCursorSyncSource();
                        }
                    }

                    if (DsoModel.Default == null || Conditioning == null || KeyLed.Default == null)
                    {
                        return;
                    }
                    Dispatcher.SoftReset();
                    if (Id.IsDecode())
                    {
                        Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
                    }
                    if (Id.IsAnalog() && this is AnalogModel analogmodel)
                    {
                        //Model层 幅度细调开启与否只能依据内部字段判断--待进一步优化
                        KeyLed.Default.SetAnalogChannelConfig(Id, value, analogmodel.Conditioning.Coupling, analogmodel.Conditioning.Bandwidth, analogmodel.Conditioning.ScaleBymVAdd != 0);
                    }
                    else if (Id.IsAWG())
                    {
                        KeyLed.Default.SetOtherChannelState(ControlChannelType.AWG, DsoModel.Default.Channels.Where(x => x.Id.IsAWG()).Select(x => x.Active));
                    }
                    else if (Id.IsDecode())
                    {
                        KeyLed.Default.SetOtherChannelState(ControlChannelType.Bus, DsoModel.Default.DecodeChnls.Select(x => x.Active));
                    }
                    else if (Id.IsDigital())
                    {
                        KeyLed.Default.SetOtherChannelState(ControlChannelType.Digital, DsoModel.Default.Channels.Where(x => x.Id.IsDigital()).Select(x => x.Active));
                    }
                    else if (Id.IsMath())
                    {
                        KeyLed.Default.SetOtherChannelState(ControlChannelType.Math, DsoModel.Default.MathChnls.Select(x => x.Active));
                    }
                    else if (Id.IsReference())
                    {
                        KeyLed.Default.SetOtherChannelState(ControlChannelType.Ref, DsoModel.Default.Channels.Where(x => x.Id.IsReference()).Select(x => x.Active));
                        if (_Active == false)
                        {
                            var mathlist = DsoModel.Default.MathChnls.Where((a) => { return a.Active; }).ToList();
                            foreach (var item in mathlist)
                            {
                                if (item.Args is MathFftArg fftmath && fftmath.Source == Id)
                                {
                                    fftmath.Source = ChannelId.C1;
                                }
                                else if (item.Args is MathEResArg eresmath && eresmath.Source == Id)
                                {
                                    eresmath.Source = ChannelId.C1;
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                    else
                    {

                    }


                }
            }
        }

        private String _Label = "";
        public String Label
        {
            get => _Label;
            set
            {
                if (_Label != value)
                {
                    _Label = value;
                    OnPropertyChanged();
                }
            }
        }
        private Boolean _LabelVisibility = false;
        public Boolean LabelVisibility
        {
            get => _LabelVisibility;
            set
            {
                if (_LabelVisibility != value)
                {
                    _LabelVisibility = value;
                    OnPropertyChanged();
                }

            }
        }

        private Int64? _WindowId;
        public Int64? WindowId
        {
            get => _WindowId;
            set
            {
                if (_WindowId != value)
                {
                    _WindowId = value;
                    OnPropertyChanged();
                }
            }
        }

        public abstract VertAxisModel Conditioning
        {
            get;
        }

        public abstract SamplingModel Sampling
        {
            get;
        }

        /// <summary>
        /// work线程内部使用的pack
        /// </summary>
        public WfmPack? Pack
        {
            get;
            protected set;
        }

        /// <summary>
        /// 多线程同步的缓存，线程间互斥
        /// </summary>
        private WfmPack? _PackForLock;

        /// <summary>
        /// 用于updateVu线程更新Vu的pack
        /// </summary>
        public WfmPack? PackForVu;
        public WfmPack? ZoomPack
        {
            get;
            protected set;
        }
        /// <summary>
        /// 多线程同步的缓存，线程间互斥
        /// </summary>
        private WfmPack? _ZoomPackForLock;

        /// <summary>
        /// 用于updateVu线程更新Vu的pack
        /// </summary>
        public WfmPack? ZoomPackForVu;
        internal void CopyToPackLock()
        {
            _PackForLock = Pack;
            _ZoomPackForLock = ZoomPack;
        }

        internal void CopyFormPackLock()
        {
            PackForVu = _PackForLock;
            ZoomPackForVu = _ZoomPackForLock;
        }

        public WfmVuDatabase VuDatabase
        {
            get;
        } = new();
        public WfmVuDatabase ZoomVuDatabase
        {
            get;
        } = new();
        #region INotifyPropertyChanged
        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
                Conditioning.PropertyChanged += value;
                ChannelShareParameter.Default.PropertyChanged += value;
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
                Conditioning.PropertyChanged -= value;
                ChannelShareParameter.Default.PropertyChanged -= value;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public ReadHandler<Double[,]>? ReadSamples
        {
            get;
            set;
        }// = static (param) => Array.Empty<Double>();

        public ReadHandler<Double[,], Double[,]>? ReadIQSamples
        {
            get;
            set;
        }// = static (param) => Array.Empty<Double>();

        public PrepareHandler? PrepareSamples
        {
            get;
            set;
        } //= static (id, token) => null;

        public ProcessHandler<Double[,]>? ProcessSamples
        {
            get;
            set;
        } //= static (buf, param) => new WfmPack(buf, 0, buf.Length, new());

        public Func<ChannelModel, Int32, WfmVuBaseParam?, (WfmVuBlock?, WfmVuBlock?)?>? MakeVuSamples
        {
            get;
            set;
        }

        //public Func<Boolean>? Init
        //{
        //    get;
        //    set;
        //}

        private Boolean _IsAsyncPack = false;
        private Boolean _NeedAsyncPackFlag = true;
        public event Action<ChannelModel> FirstAsyncPackComed;

        public virtual Boolean Take(Boolean init, CancellationToken ct, CancellationToken? softResetToken = null)
        {
            Boolean result = false;
            _IsAsyncPack = true;
            var args = PrepareSamples?.Invoke(init, Id, ct, DataRole.View, softResetToken);
            var pkg = ReadSamples?.Invoke(args);
            if (pkg is not null)
            {
                Pack = ProcessSamples?.Invoke(pkg.Value, args);
                if (_IsAsyncPack && _NeedAsyncPackFlag)
                {
                    //触发事件
                    FirstAsyncPackComed?.Invoke(this);
                    _NeedAsyncPackFlag = false;
                }
                result = true;
            }
            if (Sampling is TimebaseModel timebasemodel && timebasemodel.IsZoom)
            {
                var zoomargs = PrepareSamples?.Invoke(init, Id, ct, DataRole.Zoom, softResetToken);
                var zoompkg = ReadSamples?.Invoke(zoomargs);
                if (zoompkg is not null)
                {
                    ZoomPack = ProcessSamples?.Invoke(zoompkg.Value, zoomargs);
                    result = true;
                }
                else
                {
                    ZoomPack = null;
                }
            }

            return result;
        }

        private Semaphore semaphore = new Semaphore(1, 1);

        public virtual void ClearBuffer()
        {
            semaphore.WaitOne();
            Pack = null;
            semaphore.Release();
            ZoomPack = null;
        }

        public WfmPack? DeepClonePack()
        {
            WfmPack? pack = null;
            semaphore.WaitOne();
            if (Pack is not null)
            {
                Double[,] buffer = new Double[Pack.Buffer.GetLength(0), Pack.Buffer.GetLength(1)];
                if (buffer.GetLength(0) > 0 && buffer.GetLength(1) >= Pack.Buffer.GetLength(1) && Pack.Buffer.GetLength(0) > 0 && Pack.Buffer.GetLength(1) > 0)
                {
                    Unsafe.CopyBlock(ref Unsafe.As<Double, Byte>(ref buffer[0, 0]), ref Unsafe.As<Double, Byte>(ref Pack.Buffer[0, 0]), (UInt32)(Unsafe.SizeOf<Double>() * Pack.Buffer.GetLength(1)));
                }
                Int32 offset = Pack.Offset;
                Int32 length = Pack.Length;
                WfmProperties properties = new WfmProperties(Pack.Properties.Name)
                {
                    ProcessFlag = Pack.Properties.ProcessFlag,
                    ChnlScale = Pack.Properties.ChnlScale,
                    ChnlPosition = Pack.Properties.ChnlPosition,
                    ChnlUnit = Pack.Properties.ChnlUnit,
                    ChnlBias = Pack.Properties.ChnlBias,
                    TmbScale = Pack.Properties.TmbScale,
                    TmbPosition = Pack.Properties.TmbPosition,
                    TmbUnit = Pack.Properties.TmbUnit,
                    VuStartIndex = Pack.Properties.VuStartIndex,
                    SampInterval = Pack.Properties.SampInterval,
                    DpxCorrection = Pack.Properties.DpxCorrection,
                    TrigErrorTime = Pack.Properties.TrigErrorTime,
                    DrawMethod = Pack.Properties.DrawMethod
                };

                pack = new WfmPack(buffer, offset, length, properties);
            }
            semaphore.Release();
            return pack;
        }

        protected void SetAsyncPackMark()
        {
            _IsAsyncPack = false;
            _NeedAsyncPackFlag = true;
        }

        public ChannelModel(ChannelType type, ChannelId id, Color color)
        {
            Type = type;
            Id = id;
            Name = id.ToString();
            DrawColor = color;
        }
    }
}
