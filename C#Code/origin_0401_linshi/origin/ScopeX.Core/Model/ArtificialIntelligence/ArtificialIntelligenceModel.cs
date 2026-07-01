using AutoTimebase_Selector;
using MathWorks.MATLAB.NET.Arrays;
using NPOI.Util;
using PreProcess;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;
using ScopeX.MathExt;
using STFT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    public class ArtificialIntelligenceModel : INotifyPropertyChanged
    {
        public ArtificialIntelligenceModel() 
        {
            foreach (ChannelId chnlid in ChannelIdExt.GetAnalogs())
            {
                _ExceptionViewGraphTable.Add(chnlid, new AiGraphModel($"AbnormalData({chnlid})", DrawMethod.Plot));
                _NoiseReductionViewGraphTable.Add(chnlid, new AiGraphModel($"NoiseReduction({chnlid})", DrawMethod.Plot));
            }
            _communicator = new NeuralNetworkCommunicator();
            try
            {
                _communicator.InitializeConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化连接失败: {ex.Message}");
            }

            _aiSetReportGenerator = new AiSetReportGenerator(this);
        }

        private NeuralNetworkCommunicator _communicator;

        private AiModeEnum _AiMode = AiModeEnum.Manual;
        internal AiModeEnum AiMode
        {
            get => _AiMode;
            set
            {
                if (_AiMode != value)
                {
                    _AiMode = value;
                    if (_AiMode == AiModeEnum.Auto)
                    {
                        IterFilterEnable = true;
                        ReconfigurableDBIEnable = true;
                    }
                    else
                    {
                        IterFilterEnable = false;
                        ReconfigurableDBIEnable = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private Object _LockObject = new();
        private Object _ScreenLockObject = new();
        public record AnchnlDataInfo(UInt16[] data, Double sampleInterval);
        private Dictionary<ChannelId, AnchnlDataInfo> _AnchnlTable = new();
        private Dictionary<ChannelId, AnchnlDataInfo> _ScreenAnchnlTable = new();
        private Int64 _UpdateDataCsvSeq = 0;

        internal void UpdateData(ChannelId chnlId, UInt16[] data, Double sampleInterval)
        {
            if (data == null || data.Length == 0)
            {
                return;
            }

            UInt16[] dataCopy = data.ToArray();
            lock (_LockObject)
            {
                _AnchnlTable[chnlId] = new AnchnlDataInfo(dataCopy, sampleInterval);
            }

            //将新的数据保存为 CSV（异步，避免阻塞采集线程）
            //Task.Run(() =>
            //{
            //    try
            //    {
            //SaveUpdateDataAsCsv(chnlId, dataCopy, sampleInterval);  //分类采数
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.WriteLine($"UpdateData CSV保存失败: ch={chnlId}, len={dataCopy.Length}, ex={ex}");
            //    }
            //});
        }
        internal void UpdateScreenData(ChannelId chnlId, UInt16[] data, Double sampleInterval)
        {
            if (data == null || data.Length == 0)
            {
                return;
            }

            UInt16[] dataCopy = data.ToArray();
            lock (_ScreenLockObject)
            {
                _ScreenAnchnlTable[chnlId] = new AnchnlDataInfo(dataCopy, sampleInterval);
            }
        }

        SignalPreProcess signalPreProcess = new();
        private void SaveUpdateDataAsCsv(ChannelId chnlId, UInt16[] data, Double sampleInterval)
        {

            ushort[] buffer = data;
            double[] bufferDouble = Array.ConvertAll(buffer, o => (double)o);
            MWNumericArray tmpData = new(bufferDouble);
            MWArray[] res = signalPreProcess.PreProcess(1, tmpData, DsoModel.Default.Timebase.AnalogSamplingRate);
            MWNumericArray tmp = (MWNumericArray)res[0];
            double[] tmp1 = (double[])tmp.ToVector(MWArrayComponent.Real);

            // 默认落盘目录：我的文档\ScopeX\AI\UpdateDataCsv\<ChannelId>\
            String rootDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "ScopeX",
                "AI",
                "UpdateDataCsv",
                chnlId.ToString());
            Directory.CreateDirectory(rootDir);

            Int64 seq = Interlocked.Increment(ref _UpdateDataCsvSeq);
            String fileName = $"UpdateData_{chnlId}_{DateTime.Now:yyyyMMdd_HHmmss_fff}_{seq}.csv";
            String filePath = Path.Combine(rootDir, fileName);

            using (var writer = new StreamWriter(filePath, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
            {
                // 统一用 InvariantCulture，避免小数点受本地化影响
                writer.WriteLine("Index,Time,Value");
                for (Int32 i = 0; i < tmp1.Length; i++)
                {
                    Double t = i * sampleInterval;
                    writer.WriteLine(
                        $"{tmp1[i]}");
                }
            }

            //String fileName_time = $"UpdateDataTime_{chnlId}_{DateTime.Now:yyyyMMdd_HHmmss_fff}_{seq}.csv";
            //String filePath_time = Path.Combine(rootDir, fileName_time);

            //using (var writer = new StreamWriter(filePath_time, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
            //{
            //    // 统一用 InvariantCulture，避免小数点受本地化影响
            //    writer.WriteLine("Index,Time,Value");
            //    for (Int32 i = 0; i < data.Length; i++)
            //    {
            //        Double t = i * sampleInterval;
            //        writer.WriteLine(
            //            $"{data[i]}");
            //    }
            //}
        }

        public AnchnlDataInfo? GetData(ChannelId chnlId)
        {
            lock (_LockObject)
            {
                if (_AnchnlTable.ContainsKey(chnlId))
                {
                    return _AnchnlTable[chnlId];
                }
                return null;
            }
        }

        public AnchnlDataInfo? GetScreenData(ChannelId chnlId)
        {
            lock (_LockObject)
            {
                if (_ScreenAnchnlTable.ContainsKey(chnlId))
                {
                    return _ScreenAnchnlTable[chnlId];
                }
                return null;
            }
        }

        private UInt32 _ExcuteAiSetCnt = 0;
        internal UInt32 AiSetCnt = 0;
        internal AiSetActionType PendingAiSetAction = AiSetActionType.Full;
        internal String? PendingAiSetSignalType = null;
        private UInt32 _LastScpiAiSetRequestId = 0;
        private UInt32 _LastScpiReportRequestId = 0;
        private String _LastScpiReportJson = String.Empty;
        private UInt32 _LastExecutedFullAiSetRequestId = 0;
        private Boolean _HasQueuedFullAiSetWhileRunning = false;
        private readonly AiSetReportGenerator _aiSetReportGenerator;
        private readonly Object _AiSetStateLock = new();
        private readonly Object _AiSetTipRunLock = new();
        private Int32 _AiSetWorkerRunning = 0;

        internal UInt32 ModelBuildCnt = 0;
        internal Boolean ModelLearning = false;
        private Boolean _ContinuousAiSetEnabled = false;
        private DateTime _LastContinuousAiSetRequestTime = DateTime.MinValue;
        private static readonly TimeSpan _ContinuousAiSetInterval = TimeSpan.FromSeconds(5);

        internal UInt32 _ModelBuildCnt = 0;
        private readonly Object _SignalTypeLock = new();
        private readonly Dictionary<ChannelId, (String SignalType, DateTime UpdatedAt)> _SignalTypeCache = new();
        private static readonly TimeSpan _SignalTypeCacheWindow = TimeSpan.FromMilliseconds(300);


        #region 智能图形图表
        private ChannelId _AiSetChnlId = ChannelId.C1;
        internal ChannelId AiSetChnlId
        {
            get => _AiSetChnlId;
            set
            {
                if (_AiSetChnlId != value)
                {
                    _AiSetChnlId = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<ChannelId, Boolean> _AiSetEnableTable = new();
        private Boolean GetAiSetEnable(ChannelId chnlId) => _AiSetEnableTable.ContainsKey(chnlId) ? _AiSetEnableTable[chnlId] : false;
        internal Boolean CurAiSetEnable
        { 
            get => GetAiSetEnable(_AiSetChnlId);
            set
            {
                if (GetAiSetEnable(_AiSetChnlId) != value)
                {
                    _AiSetEnableTable[_AiSetChnlId] = value;
                    CurAiSignalRecognitionEnable = value;
                    CurAiWindowsEnable = value;
                    CurAiParamsEnable = value;
                    if (!value) 
                    {
                        foreach (var item in DsoModel.Default.Meas.SelectedItems)
                        {
                            item.Active = false;
                        }
                    }
                    UpdateMainEnable();
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<ChannelId, Boolean> _AiSignalRecognitionEnableTable = new();
        private Boolean GetAiSignalRecognitionEnable(ChannelId chnlId) => _AiSignalRecognitionEnableTable.ContainsKey(chnlId) ? _AiSignalRecognitionEnableTable[chnlId] : false;
        internal Boolean CurAiSignalRecognitionEnable
        {
            get => GetAiSignalRecognitionEnable(_AiSetChnlId);
            set
            {
                if (GetAiSignalRecognitionEnable(_AiSetChnlId) != value)
                {
                    _AiSignalRecognitionEnableTable[_AiSetChnlId] = value;
                    UpdateMainEnable();
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<ChannelId, Boolean> _AiWindowsEnableTable = new();
        private Boolean GetAiWindowsEnable(ChannelId chnlId) => _AiWindowsEnableTable.ContainsKey(chnlId) ? _AiWindowsEnableTable[chnlId] : false;
        internal Boolean CurAiWindowsEnable
        {
            get => GetAiWindowsEnable(_AiSetChnlId);
            set
            {
                if (GetAiWindowsEnable(_AiSetChnlId) != value)
                {
                    _AiWindowsEnableTable[_AiSetChnlId] = value;
                    UpdateMainEnable();
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<ChannelId, Boolean> _AiParamsEnableTable = new();
        private Boolean GetAiParamsEnable(ChannelId chnlId) => _AiParamsEnableTable.ContainsKey(chnlId) ? _AiParamsEnableTable[chnlId] : false;
        internal Boolean CurAiParamsEnable
        {
            get => GetAiParamsEnable(_AiSetChnlId);
            set
            {
                if (GetAiParamsEnable(_AiSetChnlId) != value)
                {
                    _AiParamsEnableTable[_AiSetChnlId] = value;
                    UpdateMainEnable();
                    OnPropertyChanged();
                }
            }
        }

        //private List<String> UpdateSmartChartInfos()
        //{
        //    List<String> infos = new List<String>();
        //    foreach (ChannelId chnlid in _AiSetEnableTable.Keys)
        //    {
        //        if (GetAiSetEnable(chnlid))
        //        {
        //            infos.Add($"{chnlid}的智能图形图表功能已打开");
        //        }
        //    }

        //    var smartChannels = _AiSignalRecognitionEnableTable.Keys
        //        .Union(_AiParamsEnableTable.Keys)
        //        .Distinct()
        //        .ToArray();
        //    foreach (ChannelId chnlid in smartChannels)
        //    {
        //        String sigtype = "";
        //        if (GetAiSignalRecognitionEnable(chnlid))
        //        {
        //            sigtype = GetSignalType(chnlid);
        //            infos.Add($"{chnlid}的信号：{sigtype}");
        //        }
        //        if(GetAiParamsEnable(chnlid))
        //        {
        //            if (String.IsNullOrEmpty(sigtype))
        //                sigtype = GetSignalType(chnlid);
        //            DsoModel.Default.IntelligentChartManager.SwitchMeasureParameters(chnlid, sigtype);
        //            AppendSignalParameterInfos(infos, chnlid, sigtype);
        //        }
        //    }
        //    return infos;
        //}

        public static readonly object ConfigFileLock = new object();
        private HashSet<String> _cachedAllowedTypes = new HashSet<String>(StringComparer.OrdinalIgnoreCase);
        private DateTime _lastConfigReadTimeUtc = DateTime.MinValue;
        private Int64 _lastConfigLength = -1;

        private static readonly Dictionary<String, String> _signalTypeAliasMap = new(StringComparer.OrdinalIgnoreCase)
 {
     { "正弦信号", "Sine" },
     { "三角波信号", "Tri" },
     { "方波信号", "Square" },
     { "PSK8", "8PSK" },
     { "QAM16", "16QAM" },
     { "QAM32", "32QAM" },
     { "QAM64", "64QAM" },
     { "QAM128", "128QAM" },
     { "调幅信号", "AM" },
     { "线性调频信号", "LFM" },
     { "正弦调频信号", "SFM" },
     { "脉冲调制雷达信号", "Pulse" },
 };
        private List<String> UpdateSmartChartInfos()
        {
            List<String> infos = new List<String>();

            String configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user_signal_types.txt");
            var allowedTypes = GetAllowedTypes(configPath, out Boolean isFilterActive);

            var smartChannels = _AiSetEnableTable.Keys
                .Union(_AiSignalRecognitionEnableTable.Keys)
                .Union(_AiParamsEnableTable.Keys)
                .Distinct()
                .ToArray();

            foreach (ChannelId chnlid in smartChannels)
            {
                if (GetAiSetEnable(chnlid))
                    infos.Add($"{chnlid}的智能图形图表功能已打开");

                Boolean signalRecognitionEnabled = GetAiSignalRecognitionEnable(chnlid);
                Boolean paramsEnabled = GetAiParamsEnable(chnlid);
                if (!signalRecognitionEnabled && !paramsEnabled)
                    continue;

                String rawType = GetSignalType(chnlid);
                String normalizedType = NormalizeSignalType(rawType);
                if (String.IsNullOrWhiteSpace(normalizedType))
                    normalizedType = rawType?.Trim() ?? String.Empty;

                Boolean typeAllowed = !isFilterActive || allowedTypes.Contains(normalizedType);
                String displayType = typeAllowed ? normalizedType : "新类型信号";//"未知或未添加类型";

                if (signalRecognitionEnabled)
                    infos.Add($"{chnlid}的信号：{displayType}");

                if (paramsEnabled && typeAllowed && !String.IsNullOrWhiteSpace(normalizedType))
                {
                    DsoModel.Default.IntelligentChartManager.SwitchMeasureParameters(chnlid, normalizedType);
                    AppendSignalParameterInfos(infos, chnlid, normalizedType);
                }
            }

            return infos;
        }

        private HashSet<String> GetAllowedTypes(String configPath, out Boolean isFilterActive)
        {
            lock (ConfigFileLock)
            {
                if (!File.Exists(configPath))
                {
                    isFilterActive = false;
                    _lastConfigReadTimeUtc = DateTime.MinValue;
                    _lastConfigLength = -1;
                    _cachedAllowedTypes.Clear();
                    return _cachedAllowedTypes;
                }

                isFilterActive = true;
                DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(configPath);
                Int64 length = new FileInfo(configPath).Length;
                if (lastWriteTimeUtc == _lastConfigReadTimeUtc && length == _lastConfigLength)
                    return _cachedAllowedTypes;

                String[] lines = File.ReadAllLines(configPath);
                _cachedAllowedTypes = lines
                    .Select(NormalizeSignalType)
                    .Where(t => !String.IsNullOrWhiteSpace(t))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
                _lastConfigReadTimeUtc = lastWriteTimeUtc;
                _lastConfigLength = length;
                Debug.WriteLine("AI配置已更新，已重新加载文件。");
                return _cachedAllowedTypes;
            }
        }

        private static String NormalizeSignalType(String? signalType)
        {
            if (String.IsNullOrWhiteSpace(signalType))
                return String.Empty;

            String trimmed = signalType.Trim();
            return _signalTypeAliasMap.TryGetValue(trimmed, out String mapped) ? mapped : trimmed;
        }

        private void AppendSignalParameterInfos(List<String> infos, ChannelId channelId, String signalType)
        {
            Double amplitude = DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("Pk2Pk", channelId) ?? Double.NaN;
            Double frequency = DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("Freq", channelId) ?? Double.NaN;
            AppendParameterInfo(infos, "信号幅度", amplitude, Prefix.Milli, QuantityUnit.VoltagePeakPeak);
            AppendParameterInfo(infos, "信号频率", frequency, Prefix.Empty, QuantityUnit.Hertz);

            if (String.Equals(signalType, "Square", StringComparison.OrdinalIgnoreCase))
            {
                Double duty = DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("Duty", channelId) ?? Double.NaN;
                Double pulseWidth = DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("PWidth", channelId) ?? Double.NaN;
                Double dutyPercent = duty;
                if (Double.IsFinite(dutyPercent) && Math.Abs(dutyPercent) <= 1.0)
                    dutyPercent *= 100.0;

                AppendParameterInfo(infos, "占空比", dutyPercent, Prefix.Empty, QuantityUnit.Percent);
                AppendParameterInfo(infos, "脉冲宽度", pulseWidth, Prefix.Empty, QuantityUnit.Second);
            }

            if (String.Equals(signalType, "Tri", StringComparison.OrdinalIgnoreCase))
            {
                Double riseTime = DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("Rise", channelId) ?? Double.NaN;
                AppendParameterInfo(infos, "上升时间", riseTime, Prefix.Empty, QuantityUnit.Second);
            }

            if (IsRadarSignal(signalType))
            {
                var vectorAnalysisModel = DsoModel.Default.VectorAnalysisModel;
                var (carrierFreq, symbolRate, evm, snr) = vectorAnalysisModel.EstimateCarrierAndSymbolRate();
                AppendParameterInfo(infos, "符号速率", symbolRate, Prefix.Empty, QuantityUnit.Hertz);
                AppendParameterInfo(infos, "载波频率", carrierFreq, Prefix.Empty, QuantityUnit.Hertz);
                AppendParameterInfo(infos, "EVM", evm, Prefix.Empty, QuantityUnit.Percent);
                AppendParameterInfo(infos, "SNR", snr, Prefix.Empty, QuantityUnit.Decibel);
            }
        }

        private static Boolean IsRadarSignal(String signalType)
        {
            return String.Equals(signalType, "BPSK", StringComparison.OrdinalIgnoreCase)
                || String.Equals(signalType, "QPSK", StringComparison.OrdinalIgnoreCase)
                || String.Equals(signalType, "8PSK", StringComparison.OrdinalIgnoreCase)
                || String.Equals(signalType, "16QAM", StringComparison.OrdinalIgnoreCase)
                || String.Equals(signalType, "32QAM", StringComparison.OrdinalIgnoreCase)
                || String.Equals(signalType, "64QAM", StringComparison.OrdinalIgnoreCase)
                || String.Equals(signalType, "128QAM", StringComparison.OrdinalIgnoreCase);
        }

        private static void AppendParameterInfo(List<String> infos, String name, Double value, Prefix prefix, QuantityUnit unit)
        {
            if (!Double.IsFinite(value))
                return;
            infos.Add($"{name}为{new Quantity(value, prefix, unit).ToString("##0.000", true)}");
        }

        internal String GetSignalType(ChannelId chnlid, Boolean forceRefresh = false)
        {
            lock (_SignalTypeLock)
            {
                if (!forceRefresh &&
                    _SignalTypeCache.TryGetValue(chnlid, out var cache) &&
                    (DateTime.Now - cache.UpdatedAt) <= _SignalTypeCacheWindow)
                {
                    return cache.SignalType;
                }
            }

            String signalType = DsoModel.Default.IntelligentChartManager.GetMatchType(new double[0, 0]);
            lock (_SignalTypeLock)
            {
                _SignalTypeCache[chnlid] = (signalType, DateTime.Now);
            }
            return signalType;
        }
             
        #endregion

        #region 精度提升
        private ChannelId _ReCfgDbiChnlId = ChannelId.C1;
        internal ChannelId ReCfgDbiChnlId
        {
            get { return _ReCfgDbiChnlId; }
            set
            {
                if (value != _ReCfgDbiChnlId)
                {
                    _ReCfgDbiChnlId = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<ChannelId, Boolean> _IterFilterEnableTable = new();
        internal Boolean GetIterFilterEnable(ChannelId chnlId)
        {
            if (_IterFilterEnableTable.ContainsKey(chnlId))
                return _IterFilterEnableTable[chnlId];

            return false;
        }

        internal Boolean IterFilterEnable
        {
            get => GetIterFilterEnable(_ReCfgDbiChnlId);
            set
            {
                if (GetIterFilterEnable(_ReCfgDbiChnlId) == value)
                {
                    return;
                }

                _IterFilterEnableTable[_ReCfgDbiChnlId] = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<ChannelId, Boolean> _ReconfigurableDBIEnableTable = new();
        internal Boolean GetRecfgDbiEnable(ChannelId chnlId)
        {
            if (_ReconfigurableDBIEnableTable.ContainsKey(chnlId))
                return _ReconfigurableDBIEnableTable[chnlId];

            return false;
        }

        internal Boolean ReconfigurableDBIEnable
        {
            get => GetRecfgDbiEnable(_ReCfgDbiChnlId);
            set
            {
                if (GetRecfgDbiEnable(_ReCfgDbiChnlId) == value)
                {
                    return;
                }

                _ReconfigurableDBIEnableTable[_ReCfgDbiChnlId] = value;
                UpdateMainEnable();
                OnPropertyChanged();
            }
        }

        private Dictionary<ChannelId, Boolean> _AutoFilterEnableTable = new();
        internal Boolean GetAutoFilterEnable(ChannelId chnlId)
        { 
            if (_AutoFilterEnableTable.ContainsKey(chnlId))
                return _AutoFilterEnableTable[chnlId];
            return false;
        }

        internal Boolean AutoFilterEnable
        {
            get => GetAutoFilterEnable(_ReCfgDbiChnlId);
            set
            {
                if (GetAutoFilterEnable(_ReCfgDbiChnlId) != value)
                {
                    _AutoFilterEnableTable[_ReCfgDbiChnlId] = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<ChannelId, AutoFilterMode> _AutoFilterModeTable = new();
        internal AutoFilterMode GetAutoFilterMode(ChannelId chnlId)
        {
            if (_AutoFilterModeTable.ContainsKey(chnlId))
                return _AutoFilterModeTable[chnlId];

            return AutoFilterMode.Closed;
        }

        internal AutoFilterMode AutoFilterMode
        {
            get => GetAutoFilterMode(_ReCfgDbiChnlId);
            set
            {
                if (GetAutoFilterMode(_ReCfgDbiChnlId) == value)
                {
                    return;
                }

                _AutoFilterModeTable[_ReCfgDbiChnlId] = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<ChannelId, SubbandCtrlMethod> _SubbandCtrlMethodTable = new();
        internal SubbandCtrlMethod GetSubbandCtrlMethod(ChannelId chnlId)
        { 
            return _SubbandCtrlMethodTable.ContainsKey(chnlId) ? _SubbandCtrlMethodTable[chnlId] : SubbandCtrlMethod.UserManual;
        }
        internal SubbandCtrlMethod CurSubbandCtrlMethod
        {
            get => GetSubbandCtrlMethod(_ReCfgDbiChnlId);
            set
            {
                if (GetSubbandCtrlMethod(_ReCfgDbiChnlId) != value)
                {
                    _SubbandCtrlMethodTable[_ReCfgDbiChnlId] = value;
                    UpdateMainEnable();
                    OnPropertyChanged();
                }
            }
        }

        private List<Int32> _SubbandTable = new List<Int32>();
        private Object _SubbandTableLock = new Object();
        internal void UpdateSubbandTable()
        {
            lock (_SubbandTableLock)
            {
                _SubbandTable.Clear();
                // 数组应该从Driver获取
                _SubbandTable.AddRange(new Int32[] { 1, 2, 3, 4 });
            }
        }

        internal List<Int32> SubbandTable
        {
            get
            {
                lock (_SubbandTableLock)
                { 
                    return _SubbandTable;
                }
            }
        }

        private UInt32 _SubbandsEnable = 0b1111;
        internal UInt32 SubbandsEnable
        {
            get => _SubbandsEnable;
            set
            {
                if (_SubbandsEnable != value)
                {
                    _SubbandsEnable = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _PrecisionSubbandId = 0;
        internal Int32 PrecisionSubbandId
        {
            get => _PrecisionSubbandId;
            set
            {
                if (_PrecisionSubbandId != value)
                {
                    _PrecisionSubbandId = value;
                    OnPropertyChanged();
                }
            }
        }

        internal Int32[] AnaChnlBitWidthDefine = new Int32[] { 12 };

        internal void InitAnaChnlBitWidthDefine()
        {
            Hd.TryGetData(ChannelType.ReconfigDBI, nameof(AnaChnlBitWidthDefine), out Object? data);
            if (data != null && data is Int32[])
            {
                AnaChnlBitWidthDefine = (Int32[])data;
            }
        }

        private Boolean _AutoCfgAnaChnlBitWidthEnable = false;
        internal Boolean AutoCfgAnaChnlBitWidthEnable
        {
            get => _AutoCfgAnaChnlBitWidthEnable;
            set
            {
                if (_AutoCfgAnaChnlBitWidthEnable != value)
                {
                    _AutoCfgAnaChnlBitWidthEnable = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _AnaChnlBitWidth = 12;
        internal Int32 AnaChnlBitWidth
        {
            get => _AnaChnlBitWidth;
            set
            {
                if (_AnaChnlBitWidth != value)
                {
                    _AnaChnlBitWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 子带底噪
        private Dictionary<ChannelId, Dictionary<Int32, UInt32>> _SubbandBaseNoiseTable = new();
        private readonly Dictionary<Int32, UInt32> _DefaultSubbandBaseNoiseTable = new()
        {
            [0] = 22,
            [1] = 21,
            [2] = 21,
            [3] = 24,
        };
        private const UInt32 _DefaultBaseNoiseValue = 20;

        internal Dictionary<Int32, UInt32> GetSubbandBaseNoise(ChannelId chnlId)
        {
            Dictionary<Int32, UInt32> basenoise = new();
            foreach (Int32 subbandid in _DefaultSubbandBaseNoiseTable.Keys)
            {
                if (_SubbandBaseNoiseTable.ContainsKey(chnlId) && _SubbandBaseNoiseTable[chnlId].ContainsKey(subbandid))
                {
                    basenoise[subbandid] = _SubbandBaseNoiseTable[chnlId][subbandid];
                }
                else
                {
                    basenoise[subbandid] = _DefaultSubbandBaseNoiseTable[subbandid];
                }
            }

            return basenoise;
        }

        internal UInt32 GetSubbandBaseNoise(ChannelId chnlId, Int32 subbandId)
        {
            if (_SubbandBaseNoiseTable.ContainsKey(chnlId) && _SubbandBaseNoiseTable[chnlId].ContainsKey(subbandId))
                return _SubbandBaseNoiseTable[chnlId][subbandId];

            if (_DefaultSubbandBaseNoiseTable.ContainsKey(subbandId))
                return _DefaultSubbandBaseNoiseTable[subbandId];

            return _DefaultBaseNoiseValue;
        }

        internal UInt32 CurSubbandBaseNoise
        {
            get => GetSubbandBaseNoise(_ReCfgDbiChnlId, _PrecisionSubbandId);
            set
            {
                if (GetSubbandBaseNoise(_ReCfgDbiChnlId, _PrecisionSubbandId) != value)
                {
                    if (!_SubbandBaseNoiseTable.ContainsKey(_ReCfgDbiChnlId))
                        _SubbandBaseNoiseTable[_ReCfgDbiChnlId] = new Dictionary<Int32, UInt32>();
                    _SubbandBaseNoiseTable[_ReCfgDbiChnlId][_PrecisionSubbandId] = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region 本振值设定
        private const UInt64 _DefaultPrecisionLoValue = 404; //有四个默认值，跟着子带选择走
        private Dictionary<ChannelId, UInt64> _PrecisionLoValueTable = new();

        private Dictionary<Int32, UInt64> _DefaultSubbandLocalFreqTable = new()
        {
            [0] = 0,
            [1] = 10_000_000_000,
            [2] = 15_000_000_000,
            [3] = 22_500_000_000,
        };
        private Dictionary<ChannelId, Dictionary<Int32, UInt64>> _SubbandLocalFreqTable = new();

        internal UInt64 GetSubbandLocalFreq(ChannelId chnlId, Int32 subbandId)
        {
            if (_SubbandLocalFreqTable.ContainsKey(chnlId) && _SubbandLocalFreqTable[chnlId].ContainsKey(subbandId))
                return _SubbandLocalFreqTable[chnlId][subbandId];

            if (_DefaultSubbandLocalFreqTable.ContainsKey(subbandId))
                return _DefaultSubbandLocalFreqTable[subbandId];

            return _DefaultPrecisionLoValue;
        }

        internal Dictionary<Int32, UInt64> GetSubbandLocalFreq(ChannelId chnlId)
        {
            Dictionary<Int32, UInt64> localfreq = new();
            foreach (Int32 subbandid in _DefaultSubbandLocalFreqTable.Keys)
            {
                if (_SubbandLocalFreqTable.ContainsKey(chnlId) && _SubbandLocalFreqTable[chnlId].ContainsKey(subbandid))
                {
                    localfreq[subbandid] = _SubbandLocalFreqTable[chnlId][subbandid];
                }
                else
                {
                    localfreq[subbandid] = _DefaultSubbandLocalFreqTable[subbandid];
                }
            }

            return localfreq;
        }

        internal UInt64 CurLocalFreq
        {
            get => GetSubbandLocalFreq(_ReCfgDbiChnlId, _PrecisionSubbandId);
            set
            {
                if (GetSubbandLocalFreq(_ReCfgDbiChnlId, _PrecisionSubbandId) != value)
                {
                    if (!_SubbandLocalFreqTable.ContainsKey(_ReCfgDbiChnlId))
                        _SubbandLocalFreqTable[_ReCfgDbiChnlId] = new Dictionary<Int32, UInt64>();
                    _SubbandLocalFreqTable[_ReCfgDbiChnlId][_PrecisionSubbandId] = value;
                    OnPropertyChanged();
                }
            }
        }

        internal UInt64 GetLoValue(ChannelId chnlId)
        {
            if (_PrecisionLoValueTable.ContainsKey(chnlId))
                return _PrecisionLoValueTable[chnlId];
            return _DefaultPrecisionLoValue;
        }
        internal UInt64 PrecisionLoValue
        {
            get => GetLoValue(_ReCfgDbiChnlId);
            set
            {
                if (GetLoValue(_ReCfgDbiChnlId) != value)
                {
                    _PrecisionLoValueTable[_ReCfgDbiChnlId] = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region 带宽限制设定
        private const UInt64 _DefaultBandFreqLimitValue = 404; 
        private Dictionary<ChannelId, UInt64> _PrecisionBandFreqLimitTable = new();

        private Dictionary<Int32, UInt64> _DefaultBandFreqLimitTable = new()
        {
            [0] = 8_000_000_000,
            [1] = 8_000_000_000,
            [2] = 8_000_000_000,
            [3] = 8_000_000_000,
        };
        private Dictionary<ChannelId, Dictionary<Int32, UInt64>> _BandFreqLimitTable = new();

        internal UInt64 GetBandFreqLimit(ChannelId chnlId, Int32 subbandId)
        {
            if (_BandFreqLimitTable.ContainsKey(chnlId) && _BandFreqLimitTable[chnlId].ContainsKey(subbandId))
                return _BandFreqLimitTable[chnlId][subbandId];

            if (_DefaultBandFreqLimitTable.ContainsKey(subbandId))
                return _DefaultBandFreqLimitTable[subbandId];

            return _DefaultBandFreqLimitValue;
        }

        //internal Dictionary<Int32, UInt64> GetBandFreqLimit(ChannelId chnlId)
        //{
   
        //    if (_BandFreqLimitTable.ContainsKey(chnlId))
        //        return _BandFreqLimitTable[chnlId].Copy();

        //    return _DefaultBandFreqLimitTable.Copy();
        //}

        internal Dictionary<Int32, UInt64> GetBandFreqLimit(ChannelId chnlId)
        {
            Dictionary<Int32, UInt64> bandfreqLimit = new();
            foreach (Int32 subbandid in _DefaultBandFreqLimitTable.Keys)
            {
                if (_BandFreqLimitTable.ContainsKey(chnlId) && _BandFreqLimitTable[chnlId].ContainsKey(subbandid))
                {
                    bandfreqLimit[subbandid] = _BandFreqLimitTable[chnlId][subbandid];
                }
                else
                {
                    bandfreqLimit[subbandid] = _DefaultBandFreqLimitTable[subbandid];
                }
            }

            return bandfreqLimit;
        }

        internal UInt64 CurBandFreqLimit
        {
            get => GetBandFreqLimit(_ReCfgDbiChnlId, _PrecisionSubbandId);
            set
            {
                if (GetBandFreqLimit(_ReCfgDbiChnlId, _PrecisionSubbandId) != value)
                {
                    if (!_BandFreqLimitTable.ContainsKey(_ReCfgDbiChnlId))
                        _BandFreqLimitTable[_ReCfgDbiChnlId] = new Dictionary<Int32, UInt64>();
                    _BandFreqLimitTable[_ReCfgDbiChnlId][_PrecisionSubbandId] = value;
                    OnPropertyChanged();
                }
            }
        }

        internal UInt64 GetBandFreq(ChannelId chnlId)
        {
            if (_PrecisionBandFreqLimitTable.ContainsKey(chnlId))
                return _PrecisionBandFreqLimitTable[chnlId];
            return _DefaultBandFreqLimitValue;
        }
        internal UInt64 PrecisionBandFreq
        {
            get => GetBandFreq(_ReCfgDbiChnlId);
            set
            {
                if (GetBandFreq(_ReCfgDbiChnlId) != value)
                {
                    _PrecisionBandFreqLimitTable[_ReCfgDbiChnlId] = value;
                    OnPropertyChanged();
                }
            }
        }

        //internal UInt64 PrecisionBandFreqLimit
        //{
        //    get => GetBandFreqLimit(_ReCfgDbiChnlId);
        //    set
        //    {
        //        if (GetBandFreqLimit(_ReCfgDbiChnlId) != value)
        //        {
        //            _PrecisionBandFreqLimitTable[_ReCfgDbiChnlId] = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}
        #endregion

        #region 抗镜像范围
        private const Int32 _DefaultAntImageSubbandId = 0;
        private readonly Dictionary<Int32, AntImageFreq> _DefaultAntImageFreqRange = new()
        {
            [0] = new AntImageFreq(0,               6_000_000_000),
            [1] = new AntImageFreq(6_000_000_000,  9_500_000_000),
            [2] = new AntImageFreq(9_500_000_000, 14_500_000_000),
            [3] = new AntImageFreq(14_500_000_000, 20_000_000_000),
        };

        private Dictionary<ChannelId, Dictionary<Int32, AntImageFreq>> _AntImageFreqByHz = new();

        internal Dictionary<Int32, AntImageFreq> GetAntImageFreq(ChannelId chnlId)
        {
            if (_AntImageFreqByHz.ContainsKey(chnlId))
            { 
                return _AntImageFreqByHz[chnlId].Copy();
            }
            return _DefaultAntImageFreqRange.Copy();
        }

        internal AntImageFreq GetAntImageFreq(ChannelId chnlId, Int32 subbandId)
        {
            if (_AntImageFreqByHz.ContainsKey(chnlId) && _AntImageFreqByHz[chnlId].ContainsKey(subbandId))
            {
                return _AntImageFreqByHz[chnlId][subbandId];
            }

            if (_DefaultAntImageFreqRange.ContainsKey(subbandId))
                return _DefaultAntImageFreqRange[subbandId];

            return _DefaultAntImageFreqRange[_DefaultAntImageSubbandId];
        }

        internal UInt64 LeftFreqByHz
        { 
            get => GetAntImageFreq(_ReCfgDbiChnlId, _PrecisionSubbandId).LeftFreqByHz;
            set
            {
                if (GetAntImageFreq(_ReCfgDbiChnlId, _PrecisionSubbandId).LeftFreqByHz != value)
                {
                    if (!_AntImageFreqByHz.ContainsKey(_ReCfgDbiChnlId))
                    {
                        _AntImageFreqByHz[_ReCfgDbiChnlId] = new();
                    }

                    _AntImageFreqByHz[_ReCfgDbiChnlId][_PrecisionSubbandId] = new AntImageFreq(value, RightFreqByHz);

                    OnPropertyChanged();
                }
            }
        }

        internal UInt64 RightFreqByHz
        { 
            get => GetAntImageFreq(_ReCfgDbiChnlId, _PrecisionSubbandId).RightFreqByHz;
            set
            {
                if (GetAntImageFreq(_ReCfgDbiChnlId, _PrecisionSubbandId).RightFreqByHz != value)
                {
                    if (!_AntImageFreqByHz.ContainsKey(_ReCfgDbiChnlId))
                    {
                        _AntImageFreqByHz[_ReCfgDbiChnlId] = new();
                    }

                    _AntImageFreqByHz[_ReCfgDbiChnlId][_PrecisionSubbandId] = new AntImageFreq(LeftFreqByHz, value);

                    OnPropertyChanged();
                }
            }
        }

        #endregion

        /// <summary>
        /// 单位：MHz
        /// </summary>
        private UInt32 _DefaultCriticalFreq = 1000;
        private Dictionary<ChannelId, UInt32> _CriticalFreqTable = new();
        internal UInt32 GetCriticalFreq(ChannelId chnlId)
        { 
            if (_CriticalFreqTable.ContainsKey(chnlId))
                return _CriticalFreqTable[chnlId];
            return _DefaultCriticalFreq;
        }

        internal UInt32 CriticalFreq
        {
            get => GetCriticalFreq(_ReCfgDbiChnlId);
            set
            {
                if (GetCriticalFreq(_ReCfgDbiChnlId) != value)
                {
                    _CriticalFreqTable[_ReCfgDbiChnlId] = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<String> UpdateRecfgDbiInfos()
        {
            List<String> infos = new();
            foreach (ChannelId chnlid in ChannelIdExt.GetAnalogs())
            {
                if (GetRecfgDbiEnable(chnlid))
                {
                    infos.Add($"{chnlid}的可重构DBI功能已打开");
                    var ret = Hd.TryGetData(ChannelType.ReconfigDBI, chnlid, out object? data);
                    if (data != null)
                    {
                        if (data is Double)
                        {
                            infos.Add($"{chnlid}的信号频率为{new Quantity((double)data, Prefix.Empty, QuantityUnit.Hertz).ToString("##0.000", true)}");
                        }
                        if (data is Double[,])
                        {
                            for (int i = 0; i < ((Double[,])data).GetLength(1); i++)
                            {
                                infos.Add($"{chnlid}的信号频率为{new Quantity(((Double[,])data)[0, i], Prefix.Empty, QuantityUnit.Hertz).ToString("##0.000", true)}");
                            }
                        }
                    }
                }
            }

            return infos;
        }
        #endregion

        #region 智能降噪
        private ChannelId _NoiseReductionChnlId = ChannelId.C1;
        internal ChannelId NoiseReductionChnlId
        {
            get => _NoiseReductionChnlId;
            set
            {
                if (_NoiseReductionChnlId != value)
                {
                    _NoiseReductionChnlId = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<ChannelId, Boolean> _AINoiseReductionEnableTable = new();
        internal Boolean GetAINoiseReductionEnable(ChannelId chnlId)
        {
            if (_AINoiseReductionEnableTable.ContainsKey(chnlId))
                return _AINoiseReductionEnableTable[chnlId];
            return false;
        }
        internal Boolean CurAINoiseReductionEnable
        {
            get => GetAINoiseReductionEnable(_NoiseReductionChnlId);
            set
            {
                if (GetAINoiseReductionEnable(_NoiseReductionChnlId) != value)
                {
                    _AINoiseReductionEnableTable[_NoiseReductionChnlId] = value;
                    UpdateMainEnable();
                    UpdateNoiseReductionViewEnableState(_NoiseReductionChnlId);
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<ChannelId, NoiseRedutionMethod> _NoiseRedutionMethodTable = new();
        internal NoiseRedutionMethod GetNoiseRedutionMethod(ChannelId chnlId)
        { 
            if (_NoiseRedutionMethodTable.ContainsKey(chnlId))
                return _NoiseRedutionMethodTable[chnlId];
            return NoiseRedutionMethod.Close;
        }

        internal NoiseRedutionMethod CurNoiseRedutionMethod
        {
            get => GetNoiseRedutionMethod(_NoiseReductionChnlId);
            set
            {
                if (GetNoiseRedutionMethod(_NoiseReductionChnlId) != value)
                {
                    _NoiseRedutionMethodTable[_NoiseReductionChnlId] = value;
                    UpdateNoiseReductionViewEnableState(_NoiseReductionChnlId);
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateNoiseReductionViewEnableState(ChannelId chnlId)
        {
            NoiseRedutionMethod method = GetNoiseRedutionMethod(chnlId);
            Boolean enableView = GetAINoiseReductionEnable(chnlId)
                && method != NoiseRedutionMethod.Close
                && method != NoiseRedutionMethod.AdaptiveFilter;
            SetAiViewEnable(chnlId, enableView, _NoiseReductionViewGraphTable);
        }

        private Dictionary<ChannelId, IntelligentNoiseReductionModel> _NoiseReductionModelTable = new();
        private IntelligentNoiseReductionModel GetIntelligentNoiseReductionModel(ChannelId chnlId)
        {
            if (!_NoiseReductionModelTable.ContainsKey(chnlId))
            {
                _NoiseReductionModelTable.Add(chnlId, new IntelligentNoiseReductionModel());
            }
            return _NoiseReductionModelTable[chnlId];
        }

        internal Int32 MaxAverageCount
        { 
            get => GetIntelligentNoiseReductionModel(_NoiseReductionChnlId).MaxAverageCount;
            set
            {
                if (GetIntelligentNoiseReductionModel(_NoiseReductionChnlId).MaxAverageCount != value)
                {
                    GetIntelligentNoiseReductionModel(_NoiseReductionChnlId).MaxAverageCount = value;
                    OnPropertyChanged();
                }
            }
        }

        internal void ResetAverageNoiseRedution()
        {
            GetIntelligentNoiseReductionModel(_NoiseReductionChnlId).UserResetCnt++;
        }


        public static String DefaultModelPath = @"AI/TensorModels/denoise_model_0202.onnx";
        // 训练数据累积相关字段
        private List<UInt16> _trainingDataBuffer = new List<UInt16>();
        private Int32 _trainingDataTargetSize = 10 * 1024 * 1024; // 默认10M

        /// <summary>
        /// 设置训练数据目标大小（单位：M）
        /// </summary>
        internal Int32 TrainingDataTargetSizeMB
        {
            get => _trainingDataTargetSize / (1024 * 1024);
            set
            {
                if (value > 0)
                {
                    _trainingDataTargetSize = value * 1024 * 1024;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 获取当前已累积的训练数据大小（单位：M）
        /// </summary>
        internal Double CurrentTrainingDataSizeMB => _trainingDataBuffer.Count / (1024.0 * 1024.0);

        /// <summary>
        /// 清空训练数据缓冲区（取消当前数据收集）
        /// </summary>
        internal void ClearTrainingDataBuffer()
        {
            _trainingDataBuffer.Clear();
            _ModelBuildCnt = ModelBuildCnt; // 更新计数器，停止当前收集
            Debug.WriteLine("训练数据缓冲区已清空");
        }

        /// <summary>
        /// 强制发送当前累积的训练数据（即使未达到目标大小）
        /// </summary>
        internal void ForceSendTrainingData()
        {
            if (_trainingDataBuffer.Count > 0)
            {
                _communicator.SendTrainingData(_trainingDataBuffer.ToArray());
                Debug.WriteLine($"强制发送训练数据: {CurrentTrainingDataSizeMB:F2}M");
                _trainingDataBuffer.Clear();
                _ModelBuildCnt = ModelBuildCnt; // 更新计数器，表示本次收集完成
            }
        }

        private void RunNoiseRedution()
        {

            String modelPath = DefaultModelPath;
            if (ModelLearning)
            {
                if (ModelBuildCnt != _ModelBuildCnt)
                {
                    var pkg = GetData(0);
                    if (pkg != null)
                    {
                        // 将新数据添加到缓冲区
                        _trainingDataBuffer.AddRange(pkg.data);

                        // 检查是否达到目标大小
                        if (_trainingDataBuffer.Count >= _trainingDataTargetSize)
                        {
                            // 发送累积的数据
                            _communicator.SendTrainingData(_trainingDataBuffer.ToArray());

                            // 清空缓冲区
                            _trainingDataBuffer.Clear();

                            Debug.WriteLine($"已发送 {_trainingDataTargetSize / (1024 * 1024)}M 训练数据");

                            // 只在发送完数据后才更新计数器，表示本次收集完成
                            _ModelBuildCnt = ModelBuildCnt;
                        }
                        else
                        {
                            Debug.WriteLine($"累积训练数据: {_trainingDataBuffer.Count / (1024 * 1024)}M / {_trainingDataTargetSize / (1024 * 1024)}M");
                        }
                    }
                }
                if (_communicator.NewModelPath != null && _communicator.NewModelPath != "")
                {
                    modelPath = _communicator.NewModelPath;
                }
            }

            foreach (ChannelId chnlid in _NoiseRedutionMethodTable.Keys)
            {
                if (!_NoiseReductionModelTable.ContainsKey(chnlid))
                {
                    _NoiseReductionModelTable[chnlid] = new IntelligentNoiseReductionModel();
                }

                List<Double> sourcedata = new List<Double>();
                Double sampleinterval = 0;
                lock (Acquisition.Locker)
                {
                    if (DsoModel.Default.TryGetChannel(chnlid, out var chnlmodel) && chnlmodel?.Pack != null)
                    {
                        sourcedata.AddRange(chnlmodel.Pack.Buffer.Cast<Double>());
                        sampleinterval = chnlmodel.Pack.Properties.SampInterval;
                    }
                }
                var resultdata = _NoiseReductionModelTable[chnlid].Run(sourcedata, _NoiseRedutionMethodTable[chnlid], modelPath);
                if (resultdata.Count > 0 && _NoiseReductionViewGraphTable.ContainsKey(chnlid))
                {
                    OccupierBuffer.Default.Provide(_NoiseReductionViewGraphTable[chnlid].Formula, new Vector(resultdata, "s", "v", sampleinterval));
                    //降噪数据放到MathVecBuffer，M7;用于后续FFT
                    MathVecBuffer.Default.Provide("M7", new Vector(resultdata, "s", "v", sampleinterval));
                }
            }
        }

        private Dictionary<ChannelId, AiGraphModel> _NoiseReductionViewGraphTable = new();
        #endregion

        #region 异常捕获模块

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
        #endregion

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
                UpdateMainEnable();
                OnPropertyChanged();
            }
        }
        #endregion

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
                    TemplateBuildCnt++;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region 模板数据长度
        private const Int32 _DefaultCaptureExceptionFrameLength = 4000;
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
        #endregion

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
        #endregion

        #region 显示模式
        private Dictionary<ChannelId, ExceptionViewMode> _ExceptionViewModeTable = new();
        internal ExceptionViewMode GetExceptionViewMode(ChannelId chnlId)
        { 
            if (_ExceptionViewModeTable.ContainsKey(chnlId))
                return _ExceptionViewModeTable[chnlId];
            return ExceptionViewMode.None;
        }

        internal ExceptionViewMode CurExceptionViewMode
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

        private Dictionary<ChannelId, AiGraphModel> _ExceptionViewGraphTable = new();

        #endregion

        #region 当前显示的异常波形帧号
        private const UInt32 _DefaultAnormalFrameId = 0;
        internal UInt32 MaxAnormlFrameId = 256;
        internal UInt32 MinAnormlFrameId = 0;

        private Dictionary<ChannelId, UInt32> _AnormlFrameIdTable = new();

        internal UInt32 GetFrameId(ChannelId chnlId)
        { 
            if (_AnormlFrameIdTable.ContainsKey(chnlId))
                return _AnormlFrameIdTable[chnlId];
            return _DefaultAnormalFrameId;
        }

        internal UInt32 CurAnormlFrameId
        {
            get => GetFrameId(_ExceptionCaptureChnlId);
            set
            {
                if (value < MinAnormlFrameId)
                    value = MinAnormlFrameId;
                if (value > MaxAnormlFrameId)
                    value = MaxAnormlFrameId;

                if (GetFrameId(_ExceptionCaptureChnlId) != value)
                {
                    _AnormlFrameIdTable[_ExceptionCaptureChnlId] = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

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
        #endregion

        private void SetAiViewEnable(ChannelId chnlId, Boolean enableState, Dictionary<ChannelId, AiGraphModel> aiGraphTable)
        {
            if (enableState)
            {
                foreach (var mathid in ChannelIdExt.GetIRMaths())
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
            //List<UInt32> viewframes = new List<UInt32>();
            //ExceptionViewMode viewmode = GetExceptionViewMode(chnlId);
            //switch (viewmode)
            //{
            //    case ExceptionViewMode.Single:
            //        viewframes.Add(CurAnormlFrameId);
            //        break;
            //    case ExceptionViewMode.All:
            //        for (UInt32 i = 0; i < capturedCnt; i++)
            //        {
            //            viewframes.Add(i);
            //        }
            //        break;
            //}

            //if (viewmode != ExceptionViewMode.None && _ExceptionViewGraphTable.ContainsKey(chnlId))
            //{
            //    ExceptionData exceptionparam = new ExceptionData(chnlId, viewframes);
            //    var datainfo = Hd.TryGetData(ChannelType.EmdProcess, exceptionparam, out Object? data);
            //    if (data != null && data is Dictionary<UInt32, List<UInt16>>)
            //    {
            //        Int32 symBaudRate = 1000_000;
            //        Dictionary<UInt32, List<UInt16>> datatable = (Dictionary<UInt32, List<UInt16>>)data;
            //        if (datatable.Count == 0)
            //        {
            //            Trace.WriteLine($"[UpdateAbnormalDagaGraph]datatable count is 0");
            //            return;
            //        }
                        

            //        Int32 rowCount = datatable.Keys.Count;
            //        Int32 columnCount = datatable.Values.Select(o => o.Count).Max();
            //        if (columnCount == 0)
            //        {
            //            Trace.WriteLine($"[UpdateAbnormalDagaGraph]columnCount is 0");
            //            return;
            //        }

            //        Double[,] dataarray = new Double[rowCount, columnCount];

            //        Int32 row = 0;
            //        foreach (var listdata in datatable.Values)
            //        {
            //            for (Int32 id = 0; id < listdata.Count; id++)
            //            {
            //                dataarray[row, id] = listdata[id];
            //            }
            //            row++;
            //        }

            //        OccupierBuffer.Default.Provide(_ExceptionViewGraphTable[chnlId].Formula, new Vector(dataarray, "", "", 1.0 / (symBaudRate), 1.0));
            //    }
            //}
        }//????
        #endregion

        #region 模板触发
        private ChannelId _TemplateTriggerChnlId = ChannelId.C1;
        internal ChannelId TemplateTriggerChnlId
        {
            get { return _TemplateTriggerChnlId; }
            set
            {
                if (value != _TemplateTriggerChnlId)
                {
                    _TemplateTriggerChnlId = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<ChannelId, Boolean> _TemplateTriggerEnableTable = new();

        internal Boolean GetTemplateTriggerEnable(ChannelId chnlId)
        { 
            if (_TemplateTriggerEnableTable.ContainsKey(chnlId))
                return _TemplateTriggerEnableTable[chnlId];

            return false;
        }

        internal Boolean FramworkDetectEnable
        {
            get => GetTemplateTriggerEnable(_TemplateTriggerChnlId);
            set
            {
                if (GetTemplateTriggerEnable(_TemplateTriggerChnlId) == value)
                {
                    return;
                }
                _TemplateTriggerEnableTable[_TemplateTriggerChnlId] = value;
                UpdateMainEnable();
                OnPropertyChanged();
            }
        }

        private const TemplateSourceEnum _DefaultTemplateSourceEnum = TemplateSourceEnum.Outside;
        private Dictionary<ChannelId, TemplateSourceEnum> TemplateSourceTable = new();

        internal TemplateSourceEnum GetTemplateSource(ChannelId chnlId)
        { 
            if (TemplateSourceTable.ContainsKey(chnlId))
                return TemplateSourceTable[chnlId];

            return _DefaultTemplateSourceEnum;
        }

        internal TemplateSourceEnum TemplateSource
        {
            get => GetTemplateSource(_TemplateTriggerChnlId);
            set
            {
                if (GetTemplateSource(_TemplateTriggerChnlId) == value)
                    return;

                TemplateSourceTable[_TemplateTriggerChnlId] = value;
                OnPropertyChanged();
            }
        }

        private const UInt32 _DefaultTemplateOffset = 50;
        private Dictionary<ChannelId, UInt32> _TemplateOffsetTable = new();

        internal UInt32 GetTemplateOffset(ChannelId chnlId)
        { 
            if (_TemplateOffsetTable.ContainsKey(chnlId))
                return _TemplateOffsetTable[chnlId];

            return _DefaultTemplateOffset;
        }

        internal UInt32 TemplateOffset
        {
            get => GetTemplateOffset(_TemplateTriggerChnlId);
            set
            {
                if (GetTemplateOffset(_TemplateTriggerChnlId) == value)
                    return;
                _TemplateOffsetTable[_TemplateTriggerChnlId] = value;
                OnPropertyChanged();
            }
        }

        private const Int32 _DefaultPosStart = 0;
        private Dictionary<ChannelId, Int32> UserDefinePosStartTable = new();

        internal Int32 GetUserDefinePosStart(ChannelId chnlId)
        { 
            if (UserDefinePosStartTable.ContainsKey(chnlId))
                return UserDefinePosStartTable[chnlId];

            return _DefaultPosStart;
        }

        internal Int32 UserDefinePosStart
        {
            get => GetUserDefinePosStart(_TemplateTriggerChnlId);
            set
            {
                if (GetUserDefinePosStart(_TemplateTriggerChnlId) == value)
                    return;
                UserDefinePosStartTable[_TemplateTriggerChnlId] = value;
                OnPropertyChanged();
            }
        }

        private const UInt32 _DefaultFrameTrigDataLen = 400;
        private Dictionary<ChannelId, UInt32> _FrameTrigDataLenTable = new();
        internal UInt32 GetFrameTrigDataLen(ChannelId chnlId)
        { 
            if (_FrameTrigDataLenTable.ContainsKey(chnlId))
                return _FrameTrigDataLenTable[chnlId];
            return _DefaultFrameTrigDataLen;
        }
        internal UInt32 FrameTrigDataLen
        {
            get => GetFrameTrigDataLen(_TemplateTriggerChnlId);
            set
            { 
                if (GetFrameTrigDataLen(_TemplateTriggerChnlId) == value)
                    return;

                _FrameTrigDataLenTable[_TemplateTriggerChnlId] = value;
                OnPropertyChanged();
            }
        }

        private const UInt32 _DefaultFrameIdForTrig = 0;
        private Dictionary<ChannelId, UInt32> _FrameIdForTrigTable = new();

        internal UInt32 GetFrameIdForTrig(ChannelId chnlId)
        { 
            if (_FrameIdForTrigTable.ContainsKey(chnlId))
                return _FrameIdForTrigTable[chnlId];

            return _DefaultFrameIdForTrig;
        }

        internal UInt32 FrameIdForTrig
        {
            get => GetFrameIdForTrig(_TemplateTriggerChnlId);
            set
            {
                if (GetFrameIdForTrig(_TemplateTriggerChnlId) == value)
                    return;
                _FrameIdForTrigTable[_TemplateTriggerChnlId] = value;
                OnPropertyChanged();
            }
        }

        private const Int32 _DefaultTemplateTriggerSendCnt = 0;
        private Dictionary<ChannelId, Int32> _TemplateTriggerSendCntTable = new();

        internal Int32 GetTemplateTriggerSendCnt(ChannelId chnlId)
        { 
            if (_TemplateTriggerSendCntTable.ContainsKey(chnlId))
                return _TemplateTriggerSendCntTable[chnlId];
            return _DefaultTemplateTriggerSendCnt;
        }

        internal Int32 TemplateTriggerSendCnt
        {
            get => GetTemplateTriggerSendCnt(_TemplateTriggerChnlId);
            set => _TemplateTriggerSendCntTable[_TemplateTriggerChnlId] = value;
        }

        private List<String> UpdateTemplateInfos()
        {
            List<String> infos = new List<String>();
            foreach (ChannelId chnlid in ChannelIdExt.GetAnalogs())
            {
                if (GetTemplateTriggerEnable(chnlid))
                {
                    infos.Add($"{chnlid}的模板检测功能已打开");
                }
            }
            return infos;
        }
        #endregion

        private bool _MainEnable = false;
        public bool MainEnable
        {
            get => _MainEnable;
            private set
            {
                if (_MainEnable != value)
                {
                    _MainEnable = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool _AiSetEnable = false;
        public bool AiSetEnable
        {
            get => _AiSetEnable;
            set
            {
                if (_AiSetEnable != value)
                {
                    _AiSetEnable = value;
                    if (!value)
                        _AiSetInfo.Clear();
                    OnPropertyChanged();
                }
            }
        }

        public bool AverageEnable
        {
            get;
            set;
        }

        private ChannelId _AIUnionChnlId = ChannelId.C1;
        public ChannelId AIUnionChnlId
        {
            get { return _AIUnionChnlId; }
            set
            {
                if (value != _AIUnionChnlId)
                {
                    _AIUnionChnlId = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<ChannelId, Boolean> _CaptureExceptionUnionEnableTable = new();
        internal Boolean GetCaptureExceptionUnionEnable(ChannelId chnlId)
        {
            if (_CaptureExceptionUnionEnableTable.ContainsKey(chnlId))
                return _CaptureExceptionUnionEnableTable[chnlId];

            return false;
        }
        public Boolean CaptureExceptionUnionEnable
        {
            get => GetCaptureExceptionUnionEnable(_AIUnionChnlId);
            set
            {
                if (GetCaptureExceptionUnionEnable(_AIUnionChnlId) == value)
                {
                    return;
                }
                _CaptureExceptionUnionEnableTable[_AIUnionChnlId] = value;
                UpdateMainEnable();
                OnPropertyChanged();
            }
        }

        private Dictionary<ChannelId, Boolean> _ReconfigDbiUnionEnableTable = new();
        internal Boolean GetReconfigDbiUnionEnable(ChannelId chnlId)
        {
            if (_ReconfigDbiUnionEnableTable.ContainsKey(chnlId))
                return _ReconfigDbiUnionEnableTable[chnlId];

            return false;
        }
        public bool ReconfigDbiUnionEnable
        {
            get => GetReconfigDbiUnionEnable(_AIUnionChnlId);
            set
            {
                if (GetReconfigDbiUnionEnable(_AIUnionChnlId) == value)
                {
                    return;
                }
                _ReconfigDbiUnionEnableTable[_AIUnionChnlId] = value;
                UpdateMainEnable();
                OnPropertyChanged();
            }
        }


        private void UpdateMainEnable()
        {
            foreach (Boolean aiset in _AiSetEnableTable.Values)
            {
                if (aiset)
                {
                    MainEnable = true;
                    return;
                }
            }

            foreach (Boolean sigrecog in _AiSignalRecognitionEnableTable.Values)
            {
                if (sigrecog)
                {
                    MainEnable = true;
                    return;
                }
            }

            foreach (Boolean aiwindow in _AiWindowsEnableTable.Values)
            {
                if (aiwindow)
                {
                    MainEnable = true;
                    return;
                }
            }

            foreach (Boolean aiparam in _AiParamsEnableTable.Values)
            {
                if (aiparam)
                {
                    MainEnable = true;
                    return;
                }
            }

            foreach (Boolean recfgdbi in _ReconfigurableDBIEnableTable.Values)
            {
                if (recfgdbi)
                {
                    MainEnable = true;
                    return;
                }
            }

            foreach (Boolean iterFilter in _ReconfigurableDBIEnableTable.Values)
            {
                if (iterFilter)
                {
                    MainEnable = true;
                    return;
                }
            }

            foreach (Boolean capexe in _CaptureExceptionEnableTable.Values)
            {
                if (capexe)
                {
                    MainEnable = true;
                    return;
                }
            }

            foreach (bool detectfram in _TemplateTriggerEnableTable.Values)
            {
                if (detectfram)
                {
                    MainEnable = true;
                    return;
                }
            }

            foreach (Boolean capuni in _CaptureExceptionUnionEnableTable.Values)
            {
                if(capuni)
                {
                    MainEnable = true;
                    return;
                }
            }
            foreach (Boolean reuni in _ReconfigDbiUnionEnableTable.Values)
            {
                if (reuni)
                {
                    MainEnable = true;
                    return;
                }
            }

            if (CurSubbandCtrlMethod == SubbandCtrlMethod.BitWidthAdaptive)
            {
                MainEnable = true;
                return;
            }
            if (CurAINoiseReductionEnable == true && CurNoiseRedutionMethod == NoiseRedutionMethod.AdaptiveFilter) 
            {
                MainEnable = true;
                return;
            }

            MainEnable = false;
        }

        private Object _AiTipInfoLock = new Object();
        private List<String> _AiTipInfo = new List<String>();
        private List<String> _AiSetTipInfo = new List<String>();
        private Int64 _AiTipVersion = 0;

        public void AppenAiTipInfo(String info)
        {
            if (String.IsNullOrWhiteSpace(info))
                return;
            lock (_AiTipInfoLock)
            {
                _AiSetTipInfo.Add(info);
            }
        }

        public void ClearAiTipInfo()
        {
            lock (_AiTipInfoLock)
            {
                _AiSetTipInfo.Clear();
            }
        }

        internal String[] AiTipInfo
        {
            get
            {
                lock (_AiTipInfoLock)
                {
                    return _AiTipInfo.ToArray();
                }
            }
        }

        internal Int64 AiTipVersion => Interlocked.Read(ref _AiTipVersion);

        private Object _AiSetInfoLock = new Object();
        private List<String> _AiSetInfo = new List<String>();
        internal String[] AiSetInfo
        {
            get
            {
                lock (_AiSetInfoLock)
                {
                    return _AiSetInfo.ToArray();
                }
            }
        }

        public void AppendAiSetInfo(String info) 
        {
            _AiSetInfo.Add(info);
        }

        public void AppendAiSetInfos(String[] infos)
        {
            _AiSetInfo.AddRange(infos);
        }

        private void UpdateAiTipInfo()
        {
            var smartinfos = UpdateSmartChartInfos();
            var recfgdbiinfos = UpdateRecfgDbiInfos();
            var abnormalinfos = UpdateAbnormalInfos();
            var templateinfos = UpdateTemplateInfos();

            RunNoiseRedution();            

            List<String> aiSetInfos;
            lock (_AiTipInfoLock)
            {
                aiSetInfos = _AiSetTipInfo.ToList();
            }

            List<String> mergedInfos = new();
            mergedInfos.AddRange(smartinfos);
            mergedInfos.AddRange(recfgdbiinfos);
            mergedInfos.AddRange(abnormalinfos);
            mergedInfos.AddRange(templateinfos);
            mergedInfos.AddRange(aiSetInfos);

            lock (_AiTipInfoLock)
            {
                if (_AiTipInfo.SequenceEqual(mergedInfos))
                    return;

                _AiTipInfo.Clear();
                _AiTipInfo.AddRange(mergedInfos);
                Interlocked.Increment(ref _AiTipVersion);
            }
        }


        STFTLib sTFTLib = new();

        public Double GetFreq(ChannelId chnlId)
        {
            var pkg = GetData(chnlId);
            if (pkg != null)
            {
                MWNumericArray ndata = new(pkg.data.Select(o => (double)o - 8192).ToArray());
                MWNumericArray tmpData = new(new double[pkg.data.Length]);
                MWArray[] res_stft_ada = sTFTLib.STFT(2, ndata, tmpData, 2048, 0, 0, 2, 0);
                MWNumericArray dataI_stft_ada = (MWNumericArray)res_stft_ada[0];
                MWNumericArray dataQ_stft_ada = (MWNumericArray)res_stft_ada[1];
                double[] dataI_stft_ada_array = (double[])dataI_stft_ada.ToVector(MWArrayComponent.Real);
                double[] dataQ_stft_ada_array = (double[])dataQ_stft_ada.ToVector(MWArrayComponent.Real);

                List<double> ampdata = new List<double>();

                for (int i = 0; i < dataI_stft_ada_array.Length; i++)
                {
                    System.Numerics.Complex complex = new System.Numerics.Complex(dataI_stft_ada_array[i], dataQ_stft_ada_array[i]);
                    var amp = complex.Magnitude;
                    ampdata.Add(amp);
                }
            ;
                RescaleAmp(ampdata);
                Double max_value = Double.MinValue;
                Double min_value = Double.MaxValue;
                (min_value, max_value) = GetDataMax(ampdata, 512, 2048);
                return (max_value + min_value) * DsoModel.Default.Timebase.AnalogSamplingRate / 2048 / 2;
            }
            return 0;
        }

        private double[] timebaseScale = new double[] {2e-11, 5e-11, 1e-10, 2e-10, 5e-10, 1e-9, 2e-9, 5e-9,
                                            1e-8, 2e-8, 5e-8, 1e-7, 2e-7, 5e-7, 1e-6, 2e-6, 5e-6, 1e-5, 2e-5, 5e-5,
                                            1e-4, 2e-4, 5e-4, 1e-3, 2e-3, 5e-3, 1e-2, 2e-2, 5e-2, 1e-1, 2e-1, 5e-1, 1};                                                                                                                                                                                                            
        TimebaseSelector selector = new();
        public void SetTimeBaseScale(ChannelId chnlId, String sigtype)
        {
            var pkg = GetData(chnlId);
            if (pkg != null)
            {
                MWNumericArray data = new(pkg.data.Select(o => (double)o).ToArray());
                MWNumericArray scales = new(timebaseScale);
                MWArray[] res = selector.AutoTimebase_Selector(1, data, 1 / pkg.sampleInterval, sigtype, scales);
                MWNumericArray tmp1 = (MWNumericArray)res[0];
                double[] tmp2 = (double[])tmp1.ToVector(MWArrayComponent.Real);
                double scale = tmp2[0];
                DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleByus = 1e6 * timebaseScale[(int)scale];
            }
        }

        public (Double, Double) GetDataMax(List<double> data, int data_row, int data_col)
        {
            int target_row = data_row;
            int target_col = (int)(data_col * 0.4);
            Double[,] temp = new double[target_row, target_col];
            Double max_value = Double.MinValue;
            Double min_value = Double.MaxValue;
            double maxval = Double.MinValue;

            for (int i = 1; i < target_row; i++)
            {

                for (int j = 0; j < target_col; j++)
                {
                    temp[i, j] = data[i * data_col + j];
                    if (temp[i, j] > maxval)
                    {
                        maxval = temp[i, j];
                    }
                }
            }

            for (int i = 1; i < target_row; i++)
            {
                //double maxval = temp[i, 0];
                //for (int j = 0; j < target_col; j++)
                //{
                //    if (temp[i,j]>maxval)
                //    {
                //        maxval = temp[i,j];
                //    }
                //}
                double y = maxval - 20;
                List<int> indices = new List<int>();
                for (int j = 0; j < target_col; j++)
                {
                    if (temp[i, j] > y)
                    {
                        indices.Add(j);
                    }
                }
                if (indices.Count == 0)
                    continue;
                int rowMin = indices[0];
                int rowMax = indices[0];
                foreach (int index in indices)
                {
                    if (index < rowMin)
                        rowMin = index;
                    if (index > rowMax)
                        rowMax = index;
                }
                if (rowMin < min_value)
                    min_value = rowMin;
                if (rowMax > max_value)
                    max_value = rowMax;
            }
            return (min_value, max_value);

        }


        private static Boolean RescaleAmp(List<double> pkg)
        {
            if (pkg == null)
                return false;
            Double unitDiff_test = -106.99;
            for (Int32 i = 0; i < pkg.Count; i++)
            {
                //Double y = pkg[i];
                if (!Double.IsNaN(pkg[i]))
                {
                    pkg[i] = 20 * Math.Log10(pkg[i]);
                    pkg[i] = pkg[i] + unitDiff_test;
                    //y = y / condition.AmpScale * Constants.IDX_PER_YDIV + condition.PosIndex;
                    //pkg[i] = ValidateVuSamples(pkg[i]);
                }
                //vubuf[i, index] = y;
            }
            return true;
        }

        /// <summary>
        /// 异步任务运行在主线程中，不需要担心耗时操作，但需要考虑跨线程的问题
        /// </summary>
        internal void Run()
        {
            try
            {
                lock (_AiSetTipRunLock)
                {
                    UpdateAiTipInfo();
                }
                TryRunContinuousAiSet();
                Thread.Sleep(20);
            }
            catch (Exception e) 
            {
            }
        }

        internal Boolean ContinuousAiSetEnabled
        {
            get => _ContinuousAiSetEnabled;
            set
            {
                if (_ContinuousAiSetEnabled == value)
                    return;
                _ContinuousAiSetEnabled = value;
                _LastContinuousAiSetRequestTime = DateTime.MinValue;
                DsoModel.Default.ArtificialIntelligence.AppenAiTipInfo(value ? "连续AiSet已开启" : "连续AiSet已关闭");
            }
        }

        private void TryRunContinuousAiSet()
        {
            if (!_ContinuousAiSetEnabled)
                return;
            if (Interlocked.CompareExchange(ref _AiSetWorkerRunning, 0, 0) != 0)
                return;
            if (DateTime.Now - _LastContinuousAiSetRequestTime < _ContinuousAiSetInterval)
                return;

            _LastContinuousAiSetRequestTime = DateTime.Now;
            RequestAiSet(AiSetActionType.Full);
        }

        internal UInt32 RequestAiSet(AiSetActionType actionType, Boolean resetContinuousCooldown = false, String? signalTypeOverride = null)
        {
            UInt32 requestId;
            lock (_AiSetStateLock)
            {
                if (resetContinuousCooldown && _ContinuousAiSetEnabled)
                    _LastContinuousAiSetRequestTime = DateTime.Now;

                Boolean workerRunning = Interlocked.CompareExchange(ref _AiSetWorkerRunning, 0, 0) != 0;
                if (actionType == AiSetActionType.Full && workerRunning)
                {
                    // Latest-wins: 执行中连续左键点击仅保留一次后续完整AiSet
                    if (!_HasQueuedFullAiSetWhileRunning)
                    {
                        PendingAiSetAction = AiSetActionType.Full;
                        PendingAiSetSignalType = signalTypeOverride;
                        AiSetCnt++;
                        _HasQueuedFullAiSetWhileRunning = true;
                    }
                    requestId = AiSetCnt;
                }
                else
                {
                    PendingAiSetAction = actionType;
                    PendingAiSetSignalType = signalTypeOverride;
                    AiSetCnt++;
                    requestId = AiSetCnt;
                }
            }
            EnsureAiSetWorker();
            return requestId;
        }

        internal void RequestAiSetFromScpi(String? signalTypeOverride = null)
        {
            UInt32 requestId = RequestAiSet(AiSetActionType.Full, resetContinuousCooldown: true, signalTypeOverride: signalTypeOverride);
            lock (_AiSetStateLock)
            {
                _LastScpiAiSetRequestId = requestId;
            }
        }

        internal String GetAiSetScpiStatusJson()
        {
            Boolean status;
            UInt32 requestId;
            UInt32 executedId;
            lock (_AiSetStateLock)
            {
                executedId = _ExcuteAiSetCnt;
                if (_LastScpiAiSetRequestId > 0)
                {
                    requestId = _LastScpiAiSetRequestId;
                    status = _LastExecutedFullAiSetRequestId >= _LastScpiAiSetRequestId;
                }
                else
                {
                    // 兼容UI左键直接触发AiSet：取最近一次已执行完成的完整AiSet请求
                    requestId = _LastExecutedFullAiSetRequestId;
                    status = _LastExecutedFullAiSetRequestId > 0;
                }

                if (status && _LastScpiReportRequestId == requestId && !String.IsNullOrWhiteSpace(_LastScpiReportJson))
                {
                    return _LastScpiReportJson;
                }
            }

            if (!status)
            {
                return JsonSerializer.Serialize(new
                {
                    status = false,
                    requestId,
                    executedRequestId = executedId
                });
            }

            String reportJson = _aiSetReportGenerator.GenerateReportJson(requestId, executedId);

            lock (_AiSetStateLock)
            {
                _LastScpiReportRequestId = requestId;
                _LastScpiReportJson = reportJson;
            }
            return reportJson;
        }

        private void EnsureAiSetWorker()
        {
            if (Interlocked.CompareExchange(ref _AiSetWorkerRunning, 1, 0) != 0)
                return;

            _ = Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        UInt32 targetCount;
                        AiSetActionType action;
                        String? signalTypeOverride;
                        lock (_AiSetStateLock)
                        {
                            if (AiSetCnt == _ExcuteAiSetCnt)
                                break;
                            targetCount = AiSetCnt;
                            action = PendingAiSetAction;
                            signalTypeOverride = PendingAiSetSignalType;
                        }

                        var channelId = GetSigValidChannel();
                        if (channelId != null)
                        {
                            lock (_AiSetTipRunLock)
                            {
                                AiSetProcess.Default.Execute((ChannelId)channelId, action, signalTypeOverride);
                            }
                        }

                        lock (_AiSetStateLock)
                        {
                            if (_ExcuteAiSetCnt < targetCount)
                                _ExcuteAiSetCnt = targetCount;
                            if (action == AiSetActionType.Full && _LastExecutedFullAiSetRequestId < targetCount)
                            {
                                _LastExecutedFullAiSetRequestId = targetCount;
                            }
                            if (action == AiSetActionType.Full && _ExcuteAiSetCnt >= AiSetCnt)
                            {
                                _HasQueuedFullAiSetWhileRunning = false;
                            }
                            PendingAiSetAction = AiSetActionType.Full;
                            PendingAiSetSignalType = null;
                        }
                    }
                }
                finally
                {
                    Interlocked.Exchange(ref _AiSetWorkerRunning, 0);
                    lock (_AiSetStateLock)
                    {
                        _HasQueuedFullAiSetWhileRunning = false;
                    }
                    if (AiSetCnt != _ExcuteAiSetCnt)
                        EnsureAiSetWorker();
                }
            });
        }

        public Boolean CanRestoreAiSet()
        {
            var channelId = GetSigValidChannel();
            if (channelId == null)
                return false;
            return AiSetProcess.Default.CanRestore((ChannelId)channelId);
        }

        private ChannelId? GetSigValidChannel()
        {
            ChannelId? validchnlid = null;
            foreach (ChannelId chnlid in ChannelIdExt.GetAnalogs())
            {
                ChannelModel m = DsoModel.Default.Channels.Where(c => c.Id == chnlid).First();
                if (m.Active)
                {
                    var pkg = GetData(chnlid);
                    if (pkg != null)
                    {
                        validchnlid = chnlid;
                    }
                }
            }
            return ChannelId.C1;
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

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
