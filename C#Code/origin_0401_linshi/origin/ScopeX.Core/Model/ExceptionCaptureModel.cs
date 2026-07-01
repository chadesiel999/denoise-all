using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using ScopeX.MathExt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using static NPOI.HSSF.Util.HSSFColor;

namespace ScopeX.Core
{
    internal class ExceptionCaptureModel : INotifyPropertyChanged
    {
        public ExceptionCaptureModel(ChannelId id)
        {
            foreach (ChannelId chnlid in ChannelIdExt.GetAnalogs())
            {
                _ExceptionViewGraphTable.Add(chnlid, new ExceptionGraphModel($"AbnormalData({chnlid})", DrawMethod.Plot));
            }
            Id = id;
            Name = id.ToString();
        }

        internal ChannelType Type => ChannelType.EmdProcess;
        internal ChannelId Id
        {
            get;
        }

        internal String Name
        {
            get;
        }

        private Boolean _Active = false;
        internal Boolean Active
        {
            get { return _Active; }
            set
            {
                if (value != _Active)
                {
                    _Active = value;
                    if (_Active == false)
                    {
                        ChannelIdExt.GetAnalogs().ToList().ForEach(chnlid =>
                        {
                            _CaptureExceptionEnableTable[chnlid] = false;
                            _ExceptionViewGraphTable[chnlid].Enabled = false;
                            _ExceptionViewModeTable[chnlid] = ExceptionViewMode.None;
                        });
                        OnPropertyChanged(nameof(CaptureExceptionEnable));
                    }
                    OnPropertyChanged();
                    Hardware.HdCmdFactory.Push(HdCmd.ExceptionCapture);
                }
            }
        }

        private Color _DrawColor;

        internal Color DrawColor
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

        private Boolean _ClearFlag = true;
        public Boolean ClearFlag
        {
            get => _ClearFlag;
            set
            {
                if (_ClearFlag != value)
                {
                    _ClearFlag = value;
                }
            }
        }


        #region 通道选择

        private ChannelId _ExceptionCaptureChnlId = ChannelId.C1;
        public ChannelId ExceptionCaptureChnlId
        {
            get { return _ExceptionCaptureChnlId; }
            set
            {
                if (value != _ExceptionCaptureChnlId)
                {
                    _ExceptionCaptureChnlId = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion 通道选择

        #region 开关控制

        private Dictionary<ChannelId, Boolean> _CaptureExceptionEnableTable = new();

        internal Boolean GetCaptureExceptionEnable(ChannelId chnlId)
        {
            if (_CaptureExceptionEnableTable.ContainsKey(chnlId))
                return _CaptureExceptionEnableTable[chnlId];

            return false;
        }

        internal Boolean CaptureExceptionEnable
        {
            get => GetCaptureExceptionEnable(_ExceptionCaptureChnlId);
            set
            {
                if (GetCaptureExceptionEnable(_ExceptionCaptureChnlId) == value)
                {
                    return;
                }

                _CaptureExceptionEnableTable[_ExceptionCaptureChnlId] = value;
                OnPropertyChanged();
            }
        }

        #endregion 开关控制

        #region 模板类型选择

        private Dictionary<ChannelId, TemplateTriggerSourceEnum> _TemplateTriggerSourceTable = new();
        internal TemplateTriggerSourceEnum GetTemplateTriggerSourceEnum(ChannelId chnlId)
        {
            if (_TemplateTriggerSourceTable.ContainsKey(chnlId))
                return _TemplateTriggerSourceTable[chnlId];
            return TemplateTriggerSourceEnum.Origin;
        }

        internal TemplateTriggerSourceEnum TemplateTriggerSource
        {
            get => GetTemplateTriggerSourceEnum(_ExceptionCaptureChnlId);
            set
            {
                if (GetTemplateTriggerSourceEnum(_ExceptionCaptureChnlId) != value)
                {
                    _TemplateTriggerSourceTable[_ExceptionCaptureChnlId] = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion 模板类型选择

        #region 模板数据长度

        private const Int32 _DefaultCaptureExceptionFrameLength = 25;
        private Dictionary<ChannelId, Int32> _CaptureExceptionFrameLengthTable = new();

        internal Int32 GetCaptureExceptionFrameLength(ChannelId chnlId)
        {
            if (_CaptureExceptionFrameLengthTable.ContainsKey(chnlId))
                return _CaptureExceptionFrameLengthTable[chnlId];
            return _DefaultCaptureExceptionFrameLength;
        }

        internal Int32 CaptureExceptionFrameLength
        {
            get => GetCaptureExceptionFrameLength(_ExceptionCaptureChnlId);
            set
            {
                if (GetCaptureExceptionFrameLength(_ExceptionCaptureChnlId) != value)
                {
                    _CaptureExceptionFrameLengthTable[_ExceptionCaptureChnlId] = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion 模板数据长度

        #region 模板下发

        private const Int32 _DefaultTemplateBuildCnt = 0;
        private Dictionary<ChannelId, Int32> _BuildTemplateCntTable = new();

        internal Int32 GetTemplateBuildCnt(ChannelId chnlId)
        {
            if (_BuildTemplateCntTable.ContainsKey(chnlId))
                return _BuildTemplateCntTable[chnlId];

            return _DefaultTemplateBuildCnt;
        }

        internal Int32 TemplateBuildCnt
        {
            get => GetTemplateBuildCnt(_ExceptionCaptureChnlId);
            set => _BuildTemplateCntTable[_ExceptionCaptureChnlId] = value;
        }

        #endregion 模板下发

        #region 显示模式

        private Dictionary<ChannelId, ExceptionViewMode> _ExceptionViewModeTable = new();
        internal ExceptionViewMode GetExceptionViewMode(ChannelId chnlId)
        {
            if (_ExceptionViewModeTable.ContainsKey(chnlId))
                return _ExceptionViewModeTable[chnlId];
            return ExceptionViewMode.None;
        }

        internal ExceptionViewMode ExceptionViewMode
        {
            get => GetExceptionViewMode(_ExceptionCaptureChnlId);
            set
            {
                if (GetExceptionViewMode(_ExceptionCaptureChnlId) != value)
                {
                    _ExceptionViewModeTable[_ExceptionCaptureChnlId] = value;
                    SetAiViewEnable(_ExceptionCaptureChnlId, value != ExceptionViewMode.None, _ExceptionViewGraphTable);
                    OnPropertyChanged();
                }
            }
        }
        private Dictionary<ChannelId, ExceptionGraphModel> _ExceptionViewGraphTable = new();


        #endregion 显示模式

        #region 总异常波形帧数

        private const UInt32 DefaultAnormalFrameCount = 0;

        private Dictionary<ChannelId, UInt32> _AnormlFrameCountTable = new();

        private UInt32 GetFrameCount(ChannelId chnlId)
        {
            if (_AnormlFrameCountTable.ContainsKey(chnlId))
                return _AnormlFrameCountTable[chnlId];
            return DefaultAnormalFrameCount;
        }

        internal UInt32 AnormalFrameCount
        {
            get => GetFrameCount(_ExceptionCaptureChnlId);
            set
            {
                if (GetFrameCount(_ExceptionCaptureChnlId) != value)
                {
                    _AnormlFrameCountTable[_ExceptionCaptureChnlId] = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion 总异常波形帧数

        #region 当前显示的异常波形帧号

        private const Int32 DefaultAnormalFrameId = 0;
        internal Int32 MaxAnormlFrameId = 256;
        internal Int32 MinAnormlFrameId = 0;

        private Dictionary<ChannelId, Int32> _AnormlFrameIdTable = new();

        internal Int32 GetFrameId(ChannelId chnlId)
        {
            if (_AnormlFrameIdTable.ContainsKey(chnlId))
                return _AnormlFrameIdTable[chnlId];
            return DefaultAnormalFrameId;
        }

        internal Int32 AnormalFrameID
        {
            get => GetFrameId(_ExceptionCaptureChnlId);
            set
            {
                value = Math.Clamp(value, MinAnormlFrameId, (Int32)AnormalFrameCount - 1 < 0 ? 0 : (Int32)AnormalFrameCount - 1);

                if (GetFrameId(_ExceptionCaptureChnlId) != value)
                {
                    _AnormlFrameIdTable[_ExceptionCaptureChnlId] = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 异步任务运行在主线程中，不需要担心耗时操作，但需要考虑跨线程的问题
        /// </summary>
        internal void Run()
        {
            UpdateExceptionCapture();
            Thread.Sleep(20);
        }

        #endregion 当前显示的异常波形帧号

        #region 异常数据导出

        private const Int32 _DefaultExport2FileCnt = 0;
        private Dictionary<ChannelId, Int32> _Export2FileCntTable = new();

        internal Int32 GetExport2FileCnt(ChannelId chnlId)
        {
            if (_Export2FileCntTable.ContainsKey(chnlId))
                return _Export2FileCntTable[chnlId];
            return _DefaultExport2FileCnt;
        }

        internal Int32 Export2FileCnt
        {
            get => GetExport2FileCnt(_ExceptionCaptureChnlId);
            set => _Export2FileCntTable[_ExceptionCaptureChnlId] = value;
        }

        #endregion 异常数据导出

        private void SetAiViewEnable(ChannelId chnlId, Boolean enableState, Dictionary<ChannelId, ExceptionGraphModel> aiGraphTable)
        {
            if (enableState)
            {
                foreach (var mathid in ChannelIdExt.GetCEMaths())
                {
                    if (DsoModel.Default.TryGetChannel(mathid, out var mathmodel) && (mathmodel != null) && (mathmodel is MathModel))
                    {
                        if (((MathModel)mathmodel).Args?.Occupier == null && aiGraphTable.ContainsKey(chnlId))
                        {
                            aiGraphTable[chnlId].MathChannelId = mathid;
                            ((MathModel)mathmodel).GetOrMakeArg?.Invoke(MathType.Custom);
                            aiGraphTable[chnlId].Enabled = enableState;
                            return;
                        }
                    }
                }
            }
            else
            {
                if (aiGraphTable.ContainsKey(chnlId))
                {
                    aiGraphTable[chnlId].Enabled = enableState;
                }
            }
        }

        internal Boolean StudyStatus
        {
            get
            {
                var value = false;
                var info = Hd.TryGetData(ChannelType.EmdProcess, "StudySatus", out Object? cnt);
                if (cnt != null && cnt is Boolean)
                {
                    value = (Boolean)cnt;
                }
                return value;
            }
        }

        private void UpdateExceptionCapture()
        {
            //foreach (ChannelId chnlid in ChannelIdExt.GetAnalogs())
            {
                if (Active)
                {
                    UInt32 capturedcnt = 0;
                    var cntinfo = Hd.TryGetData(ChannelType.EmdProcess, ChannelId.C1, out Object? cnt);
                    if (cnt != null && cnt is UInt32)
                    {
                        capturedcnt = (UInt32)cnt;
                        AnormalFrameCount = ClearFlag ? 0 : capturedcnt;
                        UpdateAbnormalDagaGraph(ChannelId.C1, capturedcnt);
                    }
                    else
                        throw new Exception("程序异常，请排查！！！！！！");
                }
            }
        }

        private List<String> UpdateAbnormalInfos()
        {
            List<String> infos = new List<String>();
            foreach (ChannelId chnlid in ChannelIdExt.GetAnalogs())
            {
                if (GetCaptureExceptionEnable(chnlid))
                {
                    infos.Add($"{chnlid}的异常捕获功能已打开");
                    UInt32 capturedcnt = 0;
                    var cntinfo = Hd.TryGetData(ChannelType.EmdProcess, chnlid, out Object? cnt);
                    if (cnt != null && cnt is UInt32)
                    {
                        capturedcnt = (UInt32)cnt;
                        infos.Add($"{chnlid}已捕获到{capturedcnt}幅异常波形");
                    }
                    else
                    {
                        infos.Add($"程序异常");
                    }

                    UpdateAbnormalDagaGraph(chnlid, capturedcnt);
                }
            }
            return infos;
        }

        private void UpdateAbnormalDagaGraph(ChannelId chnlId, UInt32 capturedCnt)
        {
            List<UInt32> viewframes = new List<UInt32>();
            ExceptionViewMode viewmode = GetExceptionViewMode(chnlId);
            switch (viewmode)
            {
                case ExceptionViewMode.Single:
                    viewframes.Add((UInt32)AnormalFrameID);
                    break;
                case ExceptionViewMode.All:
                    for (UInt32 i = 0; i < 25; i++)
                    {
                        viewframes.Add(i);
                    }
                    break;
            }

            if (viewmode != ExceptionViewMode.None && _ExceptionViewGraphTable.ContainsKey(chnlId))
            {
                ExceptionData exceptionparam = new ExceptionData(chnlId, viewframes);
                var datainfo = Hd.TryGetData(ChannelType.EmdProcess, exceptionparam, out Object? data);
                if (data != null && data is Dictionary<UInt32, List<UInt16>>)
                {
                    Int32 symBaudRate = 1000_000;
                    Dictionary<UInt32, List<UInt16>> datatable = (Dictionary<UInt32, List<UInt16>>)data;
                    if (datatable.Count == 0)
                    {
                        Trace.WriteLine($"[UpdateAbnormalDagaGraph]datatable count is 0");
                        MathVecBuffer.Default.Provide(_ExceptionViewGraphTable[chnlId].Formula, new Vector());
                        return;
                    }


                    Int32 rowCount = datatable.Keys.Count;
                    Int32 columnCount = datatable.Values.Select(o => o.Count).Max();
                    if (columnCount == 0)
                    {
                        Trace.WriteLine($"[UpdateAbnormalDagaGraph]columnCount is 0");
                        return;
                    }

                    Double[,] dataarray = new Double[rowCount, columnCount];

                    var analogmode = DsoModel.Default.GetChannel(ChannelId.C1) as AnalogModel;

                    var chnlbias = analogmode!.Conditioning.BiasByuV / 1000;
                    var scale = analogmode!.Conditioning.ScaleBymV / 1000;
                    var posidxperdiv = analogmode!.Conditioning.PosIdxPerDiv;
                    var posdndex = analogmode!.Conditioning.PosIndex;
                    var posscale = posidxperdiv / scale;
                    var pos0adc = (posdndex / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2);
                    Int32 row = 0;
                    //量化公式pkg.Buffer[i, j] = (pkg.Buffer[i, j] - ctx.Pos0ByAdc) / Constants.SAMPS_PER_YDIV * ctx.Properties.ChnlScale.Value + ctx.Properties.ChnlBias;
                    foreach (var listdata in datatable.Values)
                    {
                        for (Int32 id = 0; id < listdata.Count; id++)
                        {

                            Double y = listdata[id];
                            if (!Double.IsNaN(y))
                            {
                                y = (y - pos0adc) / Constants.SAMPS_PER_YDIV * scale + chnlbias;
                                y = y * 1.0;
                                y = ValidateVuSamples(y);
                            }
                            dataarray[row, id] = y;
                        }
                        row++;
                    }

                    //var max = dataarray.Cast<Double>().Max();

                    MathVecBuffer.Default.Provide(_ExceptionViewGraphTable[chnlId].Formula, new Vector(ClearFlag ? new Double[rowCount, columnCount] : dataarray, analogmode.Sampling.Unit, analogmode.PackForVu.Properties.ChnlUnit.Name, 5E-11, 1.0));
                }
            }
        }

        private static Double ValidateVuSamples(Double y)
        {
            if (y > Constants.MAX_YPOS_IDX)
            {
                y = Constants.MAX_YPOS_IDX;
            }
            else if (y < Constants.MIN_YPOS_IDX)
            {
                y = Constants.MIN_YPOS_IDX;
            }

            return y;
        }

        #region INotifyPropertyChanged接口实现

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

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged接口实现
    }
}
