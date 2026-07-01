using Microsoft.Office.Interop.Excel;
using NationalInstruments.Visa;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.Hardware.Calibration.Data.Base;
using System.Threading.Channels;
using System.Threading;
using System.Reflection;
using MathWorks.MATLAB.NET.Arrays;
using ScopeX.MathExt;
using ScopeX.Hardware.Calibration.Tool;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageDbiDiscardDots : UserControl, IMainFormTabPage
    {
        public TabPageDbiDiscardDots()
        {
            InitializeComponent();
            Init();
            _LocalCoe = new MtlabDll(_LocalCoeDllName, _LocalCoeTypeName, new() { [_LocalCoeMethodName] = _LocalCoeSignature });
            _CutPts = new MtlabDll(_CutptsDllName, _CutptsTypeName, new() { [_CutptsMethodName] = _CutPtsSignature });
        }

        private DiscardDotsCfg _Cfg = new();

        #region 常量定义
        private const Int32 _SubbandId = 0;
        private const Int32 _DiscardBeforeId = 1;
        private const Int32 _DiscardAfterId = 2;
        private const Int32 _CalcDiscardId = 3;
        private const Int32 _InitPhaseId = 4;
        private const Int32 _CalcInitPhaseId = 5;
        private const Int32 _SignalFreqId = 6;
        private const Int32 _PhaseDiffId = 7;
        private const Int32 _LocalFreqId = 8;
        private const Int32 _CalcLocalCoe = 9;

        private const String _SampleFreqByGHz = "SampleFreqByGHz";
        private const String _ParallelRoadCnt = "ParallelRoadCount";
        private const String _SubBnadCnt = "SubBandCount";
        private const String _CoeLength = "CoeLength";

        private const String _CfgFileName = "DbiDiscardDotsCfg.txt";

        private const String _CutptsDllName = "cali_discard_num.dll";
        private const String _CutptsTypeName = "cali_discard_num.Class1";
        private const String _CutptsMethodName = "cali_discard_num";

        private const String _LocalCoeDllName = "MatlabGenerateOverlapBandSync_LoCoe.dll";
        private const String _LocalCoeTypeName = "MatlabGenerateOverlapBandSync_LoCoe.Class1";
        private const String _LocalCoeMethodName = "MatlabGenerateOverlapBandSync_LoCoe";

        private const String _PhaseDiffFileName = "PhaseDiff.txt";
        #endregion

        #region 内部变量定义
        private Int32 _CurBandMode = 0;
        private Int32 _CurChnlId = 0;
        private Int32 _CurYscale = (Int32)AnaChnlScaleIndex.Lv1;

        private Type[] _CutPtsSignature = { typeof(MWCharArray), typeof(MWCharArray), typeof(MWCharArray), typeof(MWCharArray), typeof(MWCharArray) };
        private Type[] _LocalCoeSignature = { typeof(MWCharArray), typeof(MWCharArray), typeof(MWCharArray), typeof(MWCharArray), typeof(MWCharArray) };

        private MtlabDll _CutPts;
        private MtlabDll _LocalCoe;
        #endregion

        private void UpdateDgvSubband()
        {
            DgvSubband.RowCount = _Cfg.GetCfg(_SubBnadCnt);
            for (Int32 subbandId = 0; subbandId < DgvSubband.RowCount; subbandId++)
            {
                DgvSubband.Rows[subbandId].Cells[_SubbandId].Value = subbandId + 1;

                DgvSubband.Rows[subbandId].Cells[_SignalFreqId].Value = _Cfg.GetFreqSettingStr(subbandId);
                DgvSubband.Rows[subbandId].Cells[_LocalFreqId].Value = _Cfg.GetLocalFreq(subbandId);

                DgvSubband.Rows[subbandId].Cells[_DiscardBeforeId].Value = DbiAnalogParams.Default[_CurBandMode, _CurChnlId, _CurYscale, subbandId].DiscardDotsBefore;
                DgvSubband.Rows[subbandId].Cells[_DiscardAfterId].Value = DbiAnalogParams.Default[_CurBandMode, _CurChnlId, _CurYscale, subbandId].DiscardDotsAfter;
            }
        }

        private void Init()
        {
            _Cfg.SetCfg(_SampleFreqByGHz, 80);
            _Cfg.SetCfg(_ParallelRoadCnt, 80);
            _Cfg.SetCfg(_SubBnadCnt, 4);
            _Cfg.SetCfg(_CoeLength, 800);

            _Cfg.SetFreqSetting(0, new List<Int32>() { });
            _Cfg.SetFreqSetting(1, new List<Int32>() { 5850, 5860, 5870, 5880, 5890, 5900, 5910, 5920, 5930, 5940, 5950, 5960, 5970, 5980, 5990, 6000, 6010, 6020, 6030, 6040, 6050, 6060, 6070, 6080, 6090, 6100, 6110, 6120, 6130, 6140, 6150, 6160, 6170, 6180, 6190, 6200, 6210, 6220, 6230, 6240, 6250, 6260, 6270, 6280, 6290, 6300 });
            _Cfg.SetFreqSetting(2, new List<Int32>() { 9300, 9310, 9320, 9330, 9340, 9350, 9360, 9370, 9380, 9390, 9400, 9410, 9420, 9430, 9440, 9450, 9460, 9470, 9480, 9490, 9500, 9510, 9520, 9530, 9540, 9550, 9560, 9570, 9580, 9590, 9600, 9610, 9620, 9630, 9640, 9650, 9660, 9670, 9680, 9690, 9700 });
            _Cfg.SetFreqSetting(3, new List<Int32>() { 14300, 14310, 14320, 14330, 14340, 14350, 14360, 14370, 14380, 14390, 14400, 14410, 14420, 14430, 14440, 14450, 14460, 14470, 14480, 14490, 14500, 14510, 14520, 14530, 14540, 14550, 14560, 14570, 14580, 14590, 14600, 14610, 14620, 14630, 14640, 14650, 14660, 14670, 14680, 14690, 14700 });

            _Cfg.SetLocalFreq(0, 0);
            _Cfg.SetLocalFreq(1, 10);
            _Cfg.SetLocalFreq(2, 15);
            _Cfg.SetLocalFreq(3, 22.5);

            CbxChnlSelect.SelectedIndex = _CurChnlId;

            RefreshData();
        }

        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            ProductType.B21_JinHui_PXI,
            ProductType.JiHe_MSO7000X,
            ProductType.B21_HB8G,
            ProductType.B21_HD4G,
            ProductType.B21_DBI20G,
            ProductType.B21_DBI16G,
            ProductType.B21_MD8G,
            ProductType.B24_9Y8G,
            ProductType.B24_ReCfg8G,
            ProductType.B24_ST8G,
            ProductType.B21_HR1G,
            ProductType.B21_MS2G,
            ProductType.B23_USB,
            ProductType.B23_DBI13G,
            ProductType.B24_AI20G,
            ProductType.B24_XunXin40G,
            ProductType.ForTest,
        };

        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;

        /// <summary>
        /// 将内存中的参数刷新到界面
        /// </summary>
        public void RefreshData()
        {
            TxbSampleFreq.Text = _Cfg.GetCfg(_SampleFreqByGHz).ToString();
            TxbParallelRoads.Text = _Cfg.GetCfg(_ParallelRoadCnt).ToString();
            NudSubbandCnt.Value = _Cfg.GetCfg(_SubBnadCnt);
            TbxCoeLength.Text = _Cfg.GetCfg(_CoeLength).ToString();

            UpdateDgvSubband();
        }

        /// <summary>
        /// 将界面的设置更新到内存
        /// </summary>
        private void UpdateParam()
        {
            _Cfg.SetCfg(_SampleFreqByGHz, Convert.ToInt32(TxbSampleFreq.Text));
            _Cfg.SetCfg(_ParallelRoadCnt, Convert.ToInt32(TxbParallelRoads.Text));
            _Cfg.SetCfg(_SubBnadCnt, Convert.ToInt32(NudSubbandCnt.Value));
            _Cfg.SetCfg(_CoeLength, Convert.ToInt32(TbxCoeLength.Text));

            for (Int32 subbandId = 0; subbandId < DgvSubband.RowCount; subbandId++)
            {
                DbiAnalogChannelSubbandItem originItem = DbiAnalogParams.Default[_CurBandMode, _CurChnlId, _CurYscale, subbandId];
                DbiAnalogChannelSubbandItem newItem = new DbiAnalogChannelSubbandItem()
                {
                    AnalogChannelGain = originItem.AnalogChannelGain,
                    IntDiscardDots = originItem.IntDiscardDots,
                    SubbandGain = originItem.SubbandGain,
                    BiasPreceding = originItem.BiasPreceding,
                    BiasPreceding_3Div = originItem.BiasPreceding_3Div,
                    OffsetPosterior = originItem.OffsetPosterior,
                    OffsetPosterior_3Div = originItem.OffsetPosterior_3Div,
                    Gain_FineByAdc1ByTenThousand = originItem.Gain_FineByAdc1ByTenThousand,
                    Gain_FineByAdc2ByTenThousand = originItem.Gain_FineByAdc2ByTenThousand,
                    Gain_FineByFpgaThousand = originItem.Gain_FineByFpgaThousand,
                    Reserved1 = originItem.Reserved1,
                    Reserved2 = originItem.Reserved2,
                    DiscardDotsAfter = Convert.ToInt32(DgvSubband.Rows[subbandId].Cells[_DiscardAfterId].Value),
                    DiscardDotsBefore = Convert.ToInt32(DgvSubband.Rows[subbandId].Cells[_DiscardBeforeId].Value),
                };
                DbiAnalogParams.Default[_CurBandMode, _CurChnlId, _CurYscale, subbandId] = newItem;
                _Cfg.SetLocalFreq(subbandId, Convert.ToDouble(DgvSubband.Rows[subbandId].Cells[_LocalFreqId].Value));

                string tmp = null;
                if (DgvSubband.Rows[subbandId].Cells[_SignalFreqId]?.Value!=null)
                {
                    tmp = DgvSubband.Rows[subbandId].Cells[_SignalFreqId].Value.ToString();
                } 
                if (!String.IsNullOrEmpty(tmp))
                    _Cfg.SetFreqSetting(subbandId, tmp);
            }
        }

        private IInstrumentSession? currInstrument = null;
        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
        }

        private Boolean _SignalSourceIsOk = false;
        private IInstrumentSession? sgInstrumentSession = null;//信号源仪器
        private bool CheckPrepareOk()
        {
            if (MessageBox.Show("该任务是用于本振同步,请做好如下准备:\r\n1、确认信号源[" + GetSignalSourceAddress() + "]已经连接到示波器的通道" + (_CurChnlId + 1) + "，并打开输出，设置好相应的幅度；\r\n2、示波器软件运行中，并设置好扫频的幅度档位和时基。\r\n你确认要执行该任务吗？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return false;
            sgInstrumentSession = new VISASession(GetSignalSourceAddress() ?? "", 20);

            if (!sgInstrumentSession.Open(null))
            {
                MessageBox.Show("对应的仪器不能打开！");
                return false;
            }
            return true;
        }

        private String? GetSignalSourceAddress()
        {
            if (CbxVisaResource.SelectedIndex < CbxVisaResource.Items.Count)
                return CbxVisaResource.SelectedItem.ToString();
            return null;
        }

        private Boolean SetSignalSourceFreq(Int32 freqByMHz)
        {
            return sgInstrumentSession?.WriteString($"SOUR1:FREQ {freqByMHz * 1e6}") ?? false;
        }

        /// <summary>
        /// 1、修改信号源的频率
        /// </summary>
        private Dictionary<Int32, SinFitResult> GetSubbandPhase(Int32 freqByMHz, IEnumerable<Int32> subbandIdList)
        {
            Dictionary<Int32, SinFitResult> waveOffsetGainPhasesDict = new();

            List<UInt16[]>? allWaveData = InstrumentInteract.Factory_WaveData_Channel(currInstrument, 6_000, 4);
            if (allWaveData == null)
            {
                MessageBox.Show("示波器异常，数据读取错误，请检查！");
                return waveOffsetGainPhasesDict;
            }

            Double sampFreqPerSubbandByMsps = 1000d * Double.Parse(TxbSampleFreq.Text);

            foreach (Int32 subbandId in subbandIdList)
            {
                if (subbandId < allWaveData.Count)
                {
                    waveOffsetGainPhasesDict[subbandId] = SinFitClass.SinFit(allWaveData[subbandId].Take(6000).Select(o => (Double)o).ToArray(), sampFreqPerSubbandByMsps, freqByMHz) ?? new SinFitResult(0, 0, 0);
                }
            }
            return waveOffsetGainPhasesDict;
        }

        private Dictionary<Int32, Dictionary<Int32, Double>> LoadPhaseDiffDictionary()
        {
            Dictionary<Int32, Dictionary<Int32, Double>> ans = new();
            String filename = $"C{_CurChnlId + 1}_{_PhaseDiffFileName}";
            if (File.Exists(filename))
            {
                StreamReader sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    var tmp = sr.ReadLine();
                    if (tmp == null || !tmp.Contains(','))
                        continue;
                    var phasediffinfo = tmp.Split(',');
                    if (phasediffinfo == null || phasediffinfo.Length < 3)
                        continue;
                    Int32 subbandid = Int32.Parse(phasediffinfo[0]);
                    Int32 freq = Int32.Parse(phasediffinfo[1]);
                    Double phasediff = Double.Parse(phasediffinfo[2]);
                    if (!ans.ContainsKey(subbandid))
                        ans[subbandid] = new();
                    ans[subbandid][freq] = phasediff;
                }
                sr.Close();
            }

            return ans;
        }

        private void CalcDiscardDots(Int32 subbandId)
        {
            if (subbandId < 1 || currInstrument == null || sgInstrumentSession == null)
            {
                MessageBox.Show($"以下存在异常：\n子带编号：{subbandId}\n示波器地址:{currInstrument?.Address ?? "NULL"}\n信号源地址:{sgInstrumentSession?.Address ?? "NULL"}");
                return;
            }

            List<Int32> freqSettingList = _Cfg.GetFreqSettingList(subbandId);
            if (freqSettingList.Count == 0)
            {
                return;
            }

            Dictionary<Int32, Dictionary<Int32, Double>> lastphasediff = LoadPhaseDiffDictionary();
            if (!lastphasediff.ContainsKey(subbandId))
            {
                MessageBox.Show($"相位差扫频文件中不包含子带{subbandId}的数据，请重新保存！");
                return;
            }
            foreach (Int32 signalFreq in freqSettingList)
            {
                if (!lastphasediff[subbandId].ContainsKey(signalFreq))
                {
                    MessageBox.Show($"相位差扫频文件中不包含子带{subbandId}的{signalFreq}MHz，请重新保存！");
                    return;
                }
            }

            // 键值对：频率（MHz）-当前子带与上一子带的相位差序列
            Dictionary<Int32, List<Double>> phaseDiff = new();

            foreach (Int32 signalFreq in freqSettingList)
            {
                if (!SetSignalSourceFreq(signalFreq))
                {
                    MessageBox.Show("信号源异常，请检查!");
                    return;
                }
                Thread.Sleep(1000);

                phaseDiff[signalFreq] = new();
                for (Int32 i = 0; i < 10; i++)
                {
                    Dictionary<Int32, SinFitResult> phaseDic = GetSubbandPhase(signalFreq, new Int32[] { subbandId - 1, subbandId });
                    if (phaseDic.ContainsKey(subbandId) && phaseDic.ContainsKey(subbandId - 1))
                    {
                        phaseDiff[signalFreq].Add(phaseDic[subbandId - 1].Phase - phaseDic[subbandId].Phase);
                    }
                }

            }
            List<String> info = new List<String>();

            // 键值对：频率（MHz）-当前子带与上一子带的平均相位差
            Dictionary<Int32, Double> phaseDiffAverage = new();
            foreach (Int32 signalFreq in phaseDiff.Keys)
            {
                Double sinAverage = phaseDiff[signalFreq].Select(o => Math.Sin(o)).Average();
                Double cosAverage = phaseDiff[signalFreq].Select(o => Math.Cos(o)).Average();
                phaseDiffAverage[signalFreq] = Math.Atan2(sinAverage, cosAverage);
                info.Add(String.Join(",", phaseDiff[signalFreq].Select(o => o.ToString("0.000"))));
            }
            info.Add($"Average Phase:{String.Join(",", phaseDiffAverage.Values.Select(o => o.ToString("0.000")))}");
            RtbInfo.Text = String.Join("\n", info);

            Dictionary<Int32, Double> phaseDiffAverageDelta = new();
            foreach (Int32 signalFreq in phaseDiffAverage.Keys)
            {
                phaseDiffAverageDelta[signalFreq] = phaseDiffAverage[signalFreq] - lastphasediff[subbandId][signalFreq];
            }
            String diffAvg = String.Join(",", phaseDiffAverageDelta.Values.Select(o => o.ToString("0.000")));
            DgvSubband.Rows[subbandId].Cells[_PhaseDiffId].Value = diffAvg;

            Double[] cutpts = CalcCutPtsMatlabDll(subbandId, phaseDiffAverageDelta);
            for (Int32 i = 0; i < _Cfg.GetCfg(_SubBnadCnt); i++)
            {

                DbiAnalogChannelSubbandItem originItem = DbiAnalogParams.Default[_CurBandMode, _CurChnlId, _CurYscale, i];
                DbiAnalogChannelSubbandItem newItem = new DbiAnalogChannelSubbandItem()
                {
                    AnalogChannelGain = originItem.AnalogChannelGain,
                    IntDiscardDots = originItem.IntDiscardDots,
                    SubbandGain = originItem.SubbandGain,
                    BiasPreceding = originItem.BiasPreceding,
                    BiasPreceding_3Div = originItem.BiasPreceding_3Div,
                    OffsetPosterior = originItem.OffsetPosterior,
                    OffsetPosterior_3Div = originItem.OffsetPosterior_3Div,
                    Gain_FineByAdc1ByTenThousand = originItem.Gain_FineByAdc1ByTenThousand,
                    Gain_FineByAdc2ByTenThousand = originItem.Gain_FineByAdc2ByTenThousand,
                    Gain_FineByFpgaThousand = originItem.Gain_FineByFpgaThousand,
                    Reserved1 = originItem.Reserved1,
                    Reserved2 = originItem.Reserved2,
                    DiscardDotsAfter = Convert.ToInt32(cutpts[i + _Cfg.GetCfg(_SubBnadCnt)]),
                    DiscardDotsBefore = Convert.ToInt32(cutpts[i]),
                };
                DbiAnalogParams.Default[_CurBandMode, _CurChnlId, _CurYscale, i] = newItem;
            }

            UpdateDgvSubband();

            bool bResult = InstrumentInteract.CaliData_Send(currInstrument, CaliDataType.DbiAnalogParams);
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subbandId"></param>
        /// <param name="phaseDiffTable">频率（MHz）</param>
        private Double[] CalcCutPtsMatlabDll(Int32 subbandId, Dictionary<Int32, Double> phaseDiffTable)
        {
            String freqStr = String.Join(",", phaseDiffTable.Keys);
            String phase = String.Join(",", phaseDiffTable.Values.Select(o => o.ToString("0.000")));
            //String cfgparam = $"{freqStr} {phase} {_Cfg.GetCfg(_SampleFreqByGHz)} {_Cfg.GetCfg(_ParallelRoadCnt)} {_Cfg.GetCfg(_SubBnadCnt)} {subbandId}";
            String cfgparam = $"{freqStr} {phase} {_Cfg.GetCfg(_SampleFreqByGHz)} {subbandId+1}";

            List<Int32> discardDots = new();

            for (Int32 i = 0; i < _Cfg.GetCfg(_SubBnadCnt); i++)
            {
                discardDots.Add(DbiAnalogParams.Default[_CurBandMode, _CurChnlId, _CurYscale, i].DiscardDotsBefore);
            }

            for (Int32 i = 0; i < _Cfg.GetCfg(_SubBnadCnt); i++)
            {
                discardDots.Add(DbiAnalogParams.Default[_CurBandMode, _CurChnlId, _CurYscale, i].DiscardDotsAfter);
            }

            String paramstr = $"{cfgparam} {String.Join(",", discardDots)}";

            String filepath = "0";
            String filename = "0";
            String DebugSaveFilePath = "0";
            String DebugSaveFilePrefixName = "0";
            MWArray[] paramArray = new MWArray[]
            {
                new MWCharArray(filepath.ToCharArray()),
                new MWCharArray(filename.ToCharArray()),
                new MWCharArray(DebugSaveFilePath.ToCharArray()),
                new MWCharArray(DebugSaveFilePrefixName.ToCharArray()),
                new MWCharArray(paramstr.ToCharArray()),
            };

            var ans = _CutPts.Excute(_CutptsMethodName, paramArray);

            if (ans is not MWNumericArray)
            {
                return new Double[0];
            }

            var cutpts = (Double[]?)((ans as MWNumericArray)?.ToVector(MWArrayComponent.Real));
            if (cutpts == null)
                return new Double[0];
            return cutpts;
        }

        Dictionary<Int32, Int32> _LocalCoeFreq = new()
        {
            [1] = 6000,
            [2] = 9500,
            [3] = 14500,
        };

        private void CalcLocalInitPhase(Int32 subbandId)
        {
            if (subbandId < 1 || currInstrument == null)
            {
                return;
            }

            List<Double> phasediff = new();

            Int32 signalFreq = 6000;
            if (_LocalCoeFreq.ContainsKey(subbandId))
                signalFreq = _LocalCoeFreq[subbandId];

            if (!SetSignalSourceFreq(signalFreq))
            {
                MessageBox.Show("信号源异常，请检查!");
                return;
            }
            Thread.Sleep(1000);

            for (Int32 i = 0; i < 50; i++)
            {
                Dictionary<Int32, SinFitResult> phaseDic = GetSubbandPhase(signalFreq, new Int32[] { subbandId - 1, subbandId });
                if (phaseDic.ContainsKey(subbandId) && phaseDic.ContainsKey(subbandId - 1))
                {
                    phasediff.Add(phaseDic[subbandId - 1].Phase - phaseDic[subbandId].Phase);
                }
            }
            if (phasediff.Count == 0) phasediff.Add(0.0);

            RtbInfo.Text = String.Join(", ", phasediff.Select(o => o.ToString("0.000")));

            Double phasesin = phasediff.Select(o => Math.Sin(o)).Average();
            Double phasecos = phasediff.Select(o => Math.Cos(o)).Average();

            Double phase = Math.Atan2(phasesin, phasecos);
            DgvSubband.Rows[subbandId].Cells[_InitPhaseId].Value = phase;
        }

        private void CalcLocalCoe(Int32 subbandId, Double phaseDiff)
        {
            String cfgparam = $"{_Cfg.GetCfg(_SampleFreqByGHz)} {phaseDiff.ToString("0.000")} {_Cfg.GetLocalFreq(subbandId)} 16 {_Cfg.GetCfg(_CoeLength)}";
            String filepath = "0";
            String filename = "0";
            String DebugSaveFilePath = "0";
            String DebugSaveFilePrefixName = "0";
            MWArray[] paramArray = new MWArray[]
            {
                new MWCharArray(filepath.ToCharArray()),
                new MWCharArray(filename.ToCharArray()),
                new MWCharArray(DebugSaveFilePath.ToCharArray()),
                new MWCharArray(DebugSaveFilePrefixName.ToCharArray()),
                new MWCharArray(cfgparam.ToCharArray()),
            };

            var tmp = _LocalCoe.Excute(_LocalCoeMethodName, paramArray);
            if (tmp == null)
            {
                MessageBox.Show($"{_LocalCoeDllName} is initting or error.\n Init Info:{_LocalCoe.InitInfo}");
                return;
            }

            if (tmp is not MWNumericArray)
            {
                MessageBox.Show($"{_LocalCoeDllName} return error {tmp.GetType()}!");
                return;
            }

            var ans = (Double[]?)((tmp as MWNumericArray)?.ToVector(MWArrayComponent.Real));
            if (ans == null)
            {
                MessageBox.Show($"{_LocalCoeDllName} return error {tmp.GetType()}!");
                return;
            }

            RtbInfo.Text = String.Join("\n", ans);


            SaveAndSendCoe(ans, DbiCoefficientsTablesType.LocalOscillatorCoefficients, (ChannelId)_CurChnlId, subbandId);




            for (Int32 i = 0; i < ans.Length; i++)
            {
                //DbiCoefficientsTables.Default[DbiCoefficientsTablesType.LocalOscillatorCoefficients, i, 0, _CurChnlId, subbandId, 0] = (Int32)ans[i];
                DbiCoefficientsTables.Default[DbiCoefficientsTablesType.LocalOscillatorCoefficients, i, 0, _CurChnlId, subbandId, 0] = (Int32)ans[i];
            }

            bool bResult = InstrumentInteract.DbiCoefficientsTable_SaveToFile(currInstrument, DbiCoefficientsTablesType.LocalOscillatorCoefficients, 0, _CurChnlId, subbandId, 0);
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }
        private static void SaveAndSendCoe(Double[] coeData, DbiCoefficientsTablesType coeType, ChannelId chnlId, Int32 subbandId)
        {

            String dirName = "C:\\Users\\Admin\\Desktop\\origin_0316\\origin\\ScopeX.U2\\bin\\Debug\\net6.0-windows\\dbi_all_coe\\";
            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);
            String fileName = "C:\\Users\\Admin\\Desktop\\origin_0316\\origin\\ScopeX.U2\\bin\\Debug\\net6.0-windows\\dbi_all_coe\\LOC_Ch1_Sub_" + Convert.ToString(subbandId+1)+"_500mv.txt";
            var bExists = File.Exists(fileName);
            if (bExists)
            {
                File.Delete(fileName);
            }
            FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate,FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            for (Int32 i = 0; i < coeData.Length; i++)
            {
                sw.WriteLine(coeData[i]);
            }
            sw.Flush();
            sw.Close();

            Thread.Sleep(10);
        }
        private void SendParam(Boolean isSaveToFile = false)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }

            bool bResult = isSaveToFile ? InstrumentInteract.CaliData_SaveToFile(this.currInstrument, CaliDataType.DbiAnalogParams) : InstrumentInteract.CaliData_Send(this.currInstrument, CaliDataType.DbiAnalogParams);
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void DgvSubband_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 表头，不处理
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex < 0 || e.ColumnIndex >= DgvSubband.Columns.Count)
            {
                return;
            }

            switch (e.ColumnIndex)
            {
                case _CalcDiscardId:
                    CalcDiscardDots(e.RowIndex);
                    break;

                case _CalcInitPhaseId:
                    CalcLocalInitPhase(e.RowIndex);
                    break;

                case _CalcLocalCoe:
                    CalcLocalCoe(e.RowIndex, Convert.ToDouble(DgvSubband.Rows[e.RowIndex].Cells[_InitPhaseId].Value));
                    break;
            }
        }

        private void BtnRefreshSingleAddress_Click(object sender, EventArgs e)
        {
            CbxVisaResource.Items.Clear();
            using (var rm = new ResourceManager())
            {
                try
                {
                    string filter = "?*";
                    IEnumerable<string> resources = rm.Find(filter);
                    foreach (string s in resources)
                    {
                        CbxVisaResource.Items.Add(s);
                    }
                    CbxVisaResource.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void NudSubbandCnt_ValueChanged(object sender, EventArgs e)
        {
            _Cfg.SetCfg(_SubBnadCnt, (Int32)NudSubbandCnt.Value);
            UpdateDgvSubband();
        }

        private void BtnLoadCfg_Click(object sender, EventArgs e)
        {
            var cfg = DiscardDotsCfg.LoadCfg(_CfgFileName);
            if (cfg != null)
            {
                _Cfg = cfg;
                RefreshData();
            }
        }

        private void BtnSaveCfg_Click(object sender, EventArgs e)
        {
            UpdateParam();
            _Cfg.SaveCfg(_CfgFileName);
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            GetCaliDataFormOrigin();

            UpdateParam();

            SendParam();
        }

        private void GetCaliDataFormOrigin()
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }

            bool bResult = InstrumentInteract.CaliData_Get(this.currInstrument, CaliDataType.DbiAnalogParams);
            if (!bResult)
                MessageBox.Show("仪器连接错误！");
        }

        private void CbxChnlSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            _CurChnlId = CbxChnlSelect.SelectedIndex;
            RefreshData();
        }

        private void DgvSubband_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            GetCaliDataFormOrigin();
            UpdateParam();
            if (ChkAutoSend.Checked)
                SendParam();
        }

        private void BtnLoadFromFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            bool bResult = InstrumentInteract.CaliData_LoadFromFile(currInstrument, CaliDataType.DbiAnalogParams);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void BtnSaveToFile_Click(object sender, EventArgs e)
        {
            GetCaliDataFormOrigin();
            UpdateParam();
            SendParam(true);
            RefreshData();
        }

        private void BtnSlectSourceAddress_Click(object sender, EventArgs e)
        {
            if (_SignalSourceIsOk)
            {
                BtnRefreshSingleAddress.Enabled = true;
                BtnSlectSourceAddress.Text = "选择";
                _SignalSourceIsOk = false;
                sgInstrumentSession = null;
                CbxVisaResource.Enabled = true;
            }
            else
            {
                _SignalSourceIsOk = CheckPrepareOk();
                if (_SignalSourceIsOk)
                {
                    BtnRefreshSingleAddress.Enabled = false;
                    BtnSlectSourceAddress.Text = "取消";
                    CbxVisaResource.Enabled = false;
                }
            }
        }

        private Dictionary<Int32, List<Double>> CalcPhaseDiffTable(Int32 subbandId)
        {
            Dictionary<Int32, List<Double>> phaseDiff = new();
            if (subbandId < 1 || currInstrument == null || sgInstrumentSession == null)
            {
                MessageBox.Show("请检查如下情况：\n示波器是否连接\n信号源是否连接");
                return phaseDiff;
            }

            List<Int32> freqSettingList = _Cfg.GetFreqSettingList(subbandId);
            if (freqSettingList.Count == 0)
            {
                return phaseDiff;
            }

            foreach (Int32 signalFreq in freqSettingList)
            {
                if (!SetSignalSourceFreq(signalFreq))
                {
                    MessageBox.Show($"信号源异常，设置{signalFreq}MHz失败!");
                    return phaseDiff;
                }
                Thread.Sleep(1000);

                phaseDiff[signalFreq] = new();
                for (Int32 i = 0; i < 10; i++)
                {
                    Dictionary<Int32, SinFitResult> phaseDic = GetSubbandPhase(signalFreq, new Int32[] { subbandId - 1, subbandId });
                    if (phaseDic.ContainsKey(subbandId) && phaseDic.ContainsKey(subbandId - 1))
                    {
                        phaseDiff[signalFreq].Add(phaseDic[subbandId - 1].Phase - phaseDic[subbandId].Phase);
                    }
                }
            }
            return phaseDiff;
        }

        private void BtnSavePhaseDiff_Click(object sender, EventArgs e)
        {
            if (currInstrument == null || sgInstrumentSession == null)
            {
                MessageBox.Show("请检查如下情况：\n示波器是否连接\n信号源是否连接");
                return;
            }

            List<Int32> olptable = _Cfg.GetOlpTable();
            Dictionary<Int32, Dictionary<Int32, Double>> originphasediff = new();
            List<String> info = new List<String>();

            foreach (Int32 olpid in olptable)
            {
                info.Add($"subband id = {olpid}:");
                Dictionary<Int32, List<Double>> phasedifflist = CalcPhaseDiffTable(olpid);
                foreach (Int32 freq in phasedifflist.Keys)
                {
                    Double sinAverage = phasedifflist[freq].Select(o => Math.Sin(o)).Average();
                    Double cosAverage = phasedifflist[freq].Select(o => Math.Cos(o)).Average();
                    if (!originphasediff.ContainsKey(olpid))
                    {
                        originphasediff[olpid] = new();
                    }
                    originphasediff[olpid][freq] = Math.Atan2(sinAverage, cosAverage);

                    info.Add(String.Join(",", phasedifflist[freq].Select(o => o.ToString("0.000"))));
                }
            }
            RtbInfo.Text = String.Join("\n", info);

            SavePhaseDiff(originphasediff);
        }

        private void SavePhaseDiff(Dictionary<Int32, Dictionary<Int32, Double>> phaseDiffTable)
        {
            String filename = $"C{_CurChnlId + 1}_{_PhaseDiffFileName}";
            StreamWriter sw = new StreamWriter(filename);
            foreach (Int32 subbandid in phaseDiffTable.Keys)
            {
                foreach (Int32 freq in phaseDiffTable[subbandid].Keys)
                {
                    sw.WriteLine($"{subbandid},{freq},{phaseDiffTable[subbandid][freq]}");
                }
            }
            sw.Flush();
            sw.Close();
            MessageBox.Show($"C{_CurChnlId + 1}相位差文件保存成功！");
        }

        private void BtnInitPhaseDiffFile_Click(object sender, EventArgs e)
        {
            List<Int32> olptable = _Cfg.GetOlpTable();
            Dictionary<Int32, Dictionary<Int32, Double>> phasedifftable = new();
            foreach (Int32 olpid in olptable)
            {
                List<Int32> freqSettingList = _Cfg.GetFreqSettingList(olpid);
                phasedifftable[olpid] = new();
                foreach (Int32 freq in freqSettingList)
                {
                    phasedifftable[olpid][freq] = 0;
                }
            }
            SavePhaseDiff(phasedifftable);
        }
    }

    public class DiscardDotsCfg
    {
        private Dictionary<String, Int32> _CfgTable = new();
        private Dictionary<Int32, List<Int32>> _FreqSettingTable = new();
        private Dictionary<Int32, Double> _LocalFreq = new();

        public String? GetFreqSettingStr(Int32 subbandId)
        {
            if (_FreqSettingTable.ContainsKey(subbandId))
                return String.Join(",", _FreqSettingTable[subbandId]);
            return null;
        }

        public List<Int32> GetFreqSettingList(Int32 subbandId)
        {
            if (_FreqSettingTable.ContainsKey(subbandId))
                return _FreqSettingTable[subbandId];
            return new List<Int32>();
        }

        public List<Int32> GetSubbandTable()
        {
            return _LocalFreq.Keys.ToList();
        }

        public List<Int32> GetOlpTable()
        {
            return _FreqSettingTable.Keys.ToList();
        }

        public void SetFreqSetting(Int32 subbandId, String freqStr)
        {
            String[] ferqTable = freqStr.Split(',');
            _FreqSettingTable[subbandId] = ferqTable.Select(o => Convert.ToInt32(o.Trim())).ToList();
        }

        public void SetFreqSetting(Int32 subbandId, IEnumerable<Int32> freqList)
        {
            _FreqSettingTable[subbandId] = freqList.ToList();
        }

        public Int32 GetCfg(String cfgType)
        {
            if (_CfgTable.ContainsKey(cfgType))
                return _CfgTable[cfgType];
            return 0;
        }

        public void SetCfg(String cfgType, Int32 cfgValue)
        {
            _CfgTable[cfgType] = cfgValue;
        }

        public Double GetLocalFreq(Int32 subbandId)
        {
            if (_LocalFreq.ContainsKey(subbandId))
                return _LocalFreq[subbandId];
            return 0;
        }

        public void SetLocalFreq(Int32 subbandId, Double localFreq)
        {
            _LocalFreq[subbandId] = localFreq;
        }

        public void SaveCfg(String fileName)
        {
            StreamWriter sw = new StreamWriter(fileName);
            foreach (String cfgType in _CfgTable.Keys)
            {
                sw.WriteLine($"{cfgType}={_CfgTable[cfgType]}");
            }

            foreach (Int32 subband in _FreqSettingTable.Keys)
            {
                sw.WriteLine($"{subband}:{String.Join(",", _FreqSettingTable[subband])}");
            }

            foreach (Int32 subband in _LocalFreq.Keys)
            {
                sw.WriteLine($"{subband}-{_LocalFreq[subband]}");
            }
            sw.Flush();
            sw.Close();
        }

        public static DiscardDotsCfg? LoadCfg(String fileName)
        {
            if (File.Exists(fileName))
            {
                DiscardDotsCfg cfg = new();
                StreamReader sr = new StreamReader(fileName);

                while (!sr.EndOfStream)
                {
                    String? tmpStr = sr.ReadLine();
                    if (String.IsNullOrEmpty(tmpStr))
                        continue;

                    String[] tmpTable = tmpStr.Split('=');
                    if (tmpTable.Length == 2)
                    {
                        cfg.SetCfg(tmpTable[0].Trim(), Convert.ToInt32(tmpTable[1].Trim()));
                        continue;
                    }

                    String[] freqTable = tmpStr.Split(":");
                    if (freqTable.Length == 2)
                    {
                        cfg.SetFreqSetting(Convert.ToInt32(freqTable[0].Trim()), freqTable[1].Trim());
                        continue;
                    }

                    String[] local = tmpStr.Split("-");
                    if (local.Length == 2)
                    {
                        cfg.SetLocalFreq(Convert.ToInt32(local[0].Trim()), Convert.ToDouble(local[1].Trim()));
                        continue;
                    }
                }
                sr.Close();
                return cfg;
            }
            return null;
        }
    }

    public static class SinFitClass
    {
        /// <summary>
        /// 拟合公式：A*cos(w*tn)+B*sin(w*tn)+C
        /// </summary>
        /// <param name="source">源数组</param>
        /// <param name="sourceSampleRateByMHz">采样率</param>
        /// <param name="sourceFreqByMHz">信号频率</param>
        /// <returns></returns>
        public static SinFitResult? SinFit(Double[] source, Double sourceSampleRateByMHz, Double sourceFreqByMHz)
        {
            Int32 len = source.Length;
            if (len == 0 || sourceSampleRateByMHz.Equals(0.0) || sourceFreqByMHz.Equals(0.0))
                return null;

            Double[] cos_data = new Double[len];
            Double[] sin_data = new Double[len];

            Double ratio = 2 * Math.PI * sourceFreqByMHz / sourceSampleRateByMHz;
            for (Int32 i = 0; i < len; i++)
            {
                cos_data[i] = Math.Cos(ratio * i);
                sin_data[i] = Math.Sin(ratio * i);
            }

            Double cos_sum = cos_data.Sum();
            Double sin_sum = sin_data.Sum();
            Double cos_avg = cos_sum / len;
            Double sin_avg = sin_sum / len;
            Double cos_cos_dp = cos_data.DotProd(cos_data);
            Double sin_sin_dp = sin_data.DotProd(sin_data);
            Double cos_sin_dp = cos_data.DotProd(sin_data);

            Double src_avg = source.Average();
            Double src_cos_dp = source.DotProd(cos_data);
            Double src_sin_dp = source.DotProd(sin_data);

            Double An = (src_cos_dp - src_avg * cos_sum) / (cos_sin_dp - sin_avg * cos_sum) - (src_sin_dp - src_avg * sin_sum) / (sin_sin_dp - sin_avg * sin_sum);
            Double Ad = (cos_cos_dp - cos_avg * cos_sum) / (cos_sin_dp - sin_avg * cos_sum) - (cos_sin_dp - cos_avg * sin_sum) / (sin_sin_dp - sin_avg * sin_sum);
            Double A = An / Ad;

            Double Bn = (src_cos_dp - src_avg * cos_sum) / (cos_cos_dp - cos_avg * cos_sum) - (src_sin_dp - src_avg * sin_sum) / (cos_sin_dp - cos_avg * sin_sum);
            Double Bd = (cos_sin_dp - sin_avg * cos_sum) / (cos_cos_dp - cos_avg * cos_sum) - (sin_sin_dp - sin_avg * sin_sum) / (cos_sin_dp - cos_avg * sin_sum);
            Double B = Bn / Bd;

            Double C = src_avg - A * cos_avg - B * sin_avg;

            return new SinFitResult(C, Math.Sqrt(A * A + B * B), Math.Atan2(A, B));
        }
    }

    public record SinFitResult(Double Offset, Double Gain, Double Phase);
}
