using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;

namespace ScopeX.Hardware.Driver
{


    public partial class AbstractAcquirer_RadioFrequency : AbstractAcquirer
    {
#if FrequencyDomain
        protected static Action? ConfigFunc = null;
        internal static void Config() { ConfigFunc?.Invoke(); }

        protected List<List<ushort>> AllChannelWaveData = new List<List<ushort>>();

        protected object AllChannelWaveDataLock = new object();
        /// <summary>
        /// 正在采集的采集参数
        /// </summary>
        internal readonly AcquireAttribute AcquingParameters = new AcquireAttribute();
        /// <summary>
        /// 已经采集的数据的采集参数。在读回数据后赋值
        /// </summary>
        internal readonly AcquireAttribute AcquedParameters = new AcquireAttribute();


        internal override void Init()
        {

        }
        internal override void InitAcq()
        {
            AcqedDataPool.RFData.TimestampInitedAcq = DateTime.Now.Ticks;
        }
        internal override void PostProcess(List<ReadInfo> readInfoList, CancellationToken? softResetToken)
        {

        }
        internal override bool ReadAcqData(List<ReadInfo> readInfoList, out double samplingRateByus, CancellationToken? softResetToken)
        {
            samplingRateByus = 1.0;
            return false;
        }

        public virtual bool TryTakeIQWave([NotNullWhen(true)] out List<Double> bufferI, [NotNullWhen(true)] out List<Double> bufferQ,
            [NotNullWhen(true)] out Double sampleIntervalByps)
        {
            bufferI = new List<Double>();
            bufferQ = new List<Double>();
            sampleIntervalByps = 0.05;
            bool result = false;
            AcqedDataPool.RFData.TimestampAcquired = DateTime.Now.Ticks;
            if (bDataVaild)
            {
                Monitor.Enter(RFData.UpdateDataLock);
                bufferI.AddRange(AcqedDataPool.RFData.DataI);
                bufferQ.AddRange(AcqedDataPool.RFData.DataQ);
                result = true;
                Monitor.Exit(RFData.UpdateDataLock);
            }
            return result;
        }

        public virtual bool TryTakeIQFFTWave([NotNullWhen(true)] out List<Double> bufferFFTI, [NotNullWhen(true)] out List<Double> bufferFFTQ,
            [NotNullWhen(true)] out Double centerFreq, [NotNullWhen(true)] out Double RBW)
        {
            bufferFFTI = new List<Double>();
            bufferFFTQ = new List<Double>();
            centerFreq = 500;
            RBW = 1000;
            bool result = false;
            AcqedDataPool.RFData.TimestampAcquired = DateTime.Now.Ticks;
            if (bDataVaild)
            {
                Monitor.Enter(RFData.UpdateDataLock);
                bufferFFTI.AddRange(AcqedDataPool.RFData.FFTDataI);
                bufferFFTQ.AddRange(AcqedDataPool.RFData.FFTDataQ);
                result = true;
                Monitor.Exit(RFData.UpdateDataLock);
            }
            return result;
        }



        #region Tables



        //private static readonly Dictionary<Int32, (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain)> _AnalogRFSpanScaleTable = new Dictionary<Int32, (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain)>{
        //    {0 ,(            0, 0,      1,             1,1)},
        //    {1 ,(        8_000,126,2000000,       10_000,0.3944)},//0418,RBW修改为10Hz,抽取比修改为2000000
        //    //{1 ,(        6_250,127,2560000,       7_812.5,0.6776)},
        //    {2 ,(       25_000,118,640000 ,        31_250,0.6776)},
        //    {3 ,(       62_500,110,256000 ,        78_125,0.4441)},
        //    {4 ,(      250_000,102,64000  ,       312_500,0.4441)},
        //    {5 ,(      625_000,94,25600  ,       781_250,0.5821)},
        //    {6 ,(    2_500_000,86,6400   ,     3_125_000,0.5821)},
        //    {7 ,(    6_250_000,78,2560   ,     7_812_500,0.7629)},
        //    {8 ,(   25_000_000,70,640    ,    31_250_000,0.7629)},
        //    {9 ,(   50_000_000,68,320    ,    62_500_000,0.7629)},
        //    {10,(  100_000_000,65,160    ,   125_000_000,0.7629)},
        //    {11,(  250_000_000,6,64    ,   312_500_000,1)},
        //    {12,(  500_000_000,4,32     ,   625_000_000,1)},
        //    {13,(  1000_000_000,1,16     ,   1250_000_000,1)},
        //    {14,(2_000_000_000,0,8      , 2_500_000_000,1)},
        //    {15,(8_000_000_000, 128,1      ,20_000_000_000,1)},
        //};

        private static readonly Dictionary<Int32, (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain)> _20GAnalogRFSpanScaleTable = new Dictionary<Int32, (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain)>{
            {0 ,(             0,       0,     5000000,                 1,          1)},
            {1 ,(       320_000/4,     126,    250000,            320_000/4,     0.3944)},//80k
            {2 ,(       640_000/4,     124,    125000,            640_000/4,     0.3944)},
            {3 ,(     1_000_000/4,     118,    80000 ,          1_000_000/4,     0.6776)},
            {4 ,(     2_000_000/4,     116,    40000 ,          2_000_000/4,     0.6776)},
            {5 ,(     2_500_000/4,     110,    32000 ,          2_500_000/4,     0.4441)},
            {6 ,(     5_000_000/4,     108,    16000 ,          5_000_000/4,     0.4441)},
            {7 ,(    10_000_000/4,     102,    8000  ,         10_000_000/4,     0.4441)},
            {8 ,(    20_000_000/4,     100,    4000  ,         20_000_000/4,     0.4441)},
            {9 ,(    25_000_000/4,     94 ,    3200  ,         25_000_000/4,     0.5821)},
            {10,(    50_000_000/4,     92 ,    1600  ,         50_000_000/4,     0.5821)},
            {11,(   100_000_000/4,     86 ,    800   ,        100_000_000/4,     0.5821)},
            {12,(   200_000_000/4,     84 ,    400   ,        200_000_000/4,     0.5821)},
            {13,(   250_000_000/4,     78 ,    320   ,        250_000_000/4,     0.7629)},
            {14,(   500_000_000/4,     76 ,    160   ,        500_000_000/4,     0.7629)},
            {15,( 1_000_000_000/4,     70 ,    80    ,      1_000_000_000/4,     0.7629)},
            {16,( 2_000_000_000/4,     68 ,    40    ,      2_000_000_000/4,     0.7629)},
            {17,( 2_500_000_000/4,     14 ,    32    ,      2_500_000_000/4,     1     )},
            {18,( 5_000_000_000/4,     12 ,    16    ,      5_000_000_000/4,     1     )},
            {19,(10_000_000_000/4,     6  ,    8     ,     10_000_000_000/4,     1     )},
            {20,(20_000_000_000/4,     4  ,    4     ,     20_000_000_000/4,     1     )},
            {21,(6_000_000_000,     0  ,    1     ,        20_000_000_000,     1     )},
        };

        private static readonly Dictionary<Int32, (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain)> _AnalogRFSpanScaleTable = new Dictionary<Int32, (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain)>{
           {0 ,(             0,       0,         1,                  1,          1)},
            {1 ,(       320_000,     126,    250000,            320_000,     0.3944)},//80k
            {2 ,(       640_000,     124,    125000,            640_000,     0.3944)},
            {3 ,(     1_000_000,     118,    80000 ,          1_000_000,     0.6776)},
            {4 ,(     2_000_000,     116,    40000 ,          2_000_000,     0.6776)},
            {5 ,(     2_500_000,     110,    32000 ,          2_500_000,     0.4441)},
            {6 ,(     5_000_000,     108,    16000 ,          5_000_000,     0.4441)},
            {7 ,(    10_000_000,     102,    8000  ,         10_000_000,     0.4441)},
            {8 ,(    20_000_000,     100,    4000  ,         20_000_000,     0.4441)},
            {9 ,(    25_000_000,     94 ,    3200  ,         25_000_000,     0.5821)},
            {10,(    50_000_000,     92 ,    1600  ,         50_000_000,     0.5821)},
            {11,(   100_000_000,     86 ,    800   ,        100_000_000,     0.5821)},
            {12,(   200_000_000,     84 ,    400   ,        200_000_000,     0.5821)},
            {13,(   250_000_000,     78 ,    320   ,        250_000_000,     0.7629)},
            {14,(   500_000_000,     76 ,    160   ,        500_000_000,     0.7629)},
            {15,( 1_000_000_000,     70 ,    80    ,      1_000_000_000,     0.7629)},
            {16,( 2_000_000_000,     68 ,    40    ,      2_000_000_000,     0.7629)},
            {17,( 2_500_000_000,     14 ,    32    ,      2_500_000_000,     1     )},
            {18,( 5_000_000_000,     12 ,    16    ,      5_000_000_000,     1     )},
            {19,(10_000_000_000,     6  ,    8     ,     10_000_000_000,     1     )},
            {20,(20_000_000_000,     4  ,    4     ,     20_000_000_000,     1     )},
            {21,(20_000_000_000    ,     0  ,    1     ,       80_000_000_000,     1     )},
        };

        //private static readonly Dictionary<Int32, (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain)> _AnalogRFSpanScaleTable = new Dictionary<Int32, (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain)>{
        //   {0 ,(             0,       0,         1,                  1,          1)},
        //    {1 ,(       320_000,     126,    250000,            320_000,     0.3944)},//80k
        //    {2 ,(       640_000,     124,    125000,            640_000,     0.3944)},
        //    {3 ,(     1_000_000,     118,    80000 ,          1_000_000,     0.6776)},
        //    {4 ,(     2_000_000,     116,    40000 ,          2_000_000,     0.6776)},
        //    {5 ,(     2_500_000,     110,    32000 ,          2_500_000,     0.4441)},
        //    {6 ,(     5_000_000,     108,    16000 ,          5_000_000,     0.4441)},
        //    {7 ,(    10_000_000,     102,    8000  ,         10_000_000,     0.4441)},
        //    {8 ,(    20_000_000,     100,    4000  ,         20_000_000,     0.4441)},
        //    {9 ,(    25_000_000,     94 ,    3200  ,         25_000_000,     0.5821)},
        //    {10,(    50_000_000,     92 ,    1600  ,         50_000_000,     0.5821)},
        //    {11,(   100_000_000,     86 ,    800   ,        100_000_000,     0.5821)},
        //    {12,(   200_000_000,     84 ,    400   ,        200_000_000,     0.5821)},
        //    {13,(   250_000_000,     78 ,    320   ,        250_000_000,     0.7629)},
        //    {14,(   500_000_000,     76 ,    160   ,        500_000_000,     0.7629)},
        //    {15,( 1_000_000_000,     70 ,    80    ,      1_000_000_000,     0.7629)},
        //    {16,( 2_000_000_000,     68 ,    40    ,      2_000_000_000,     0.7629)},
        //    {17,( 2_500_000_000,     14 ,    32    ,      2_500_000_000,     1     )},
        //    {18,( 5_000_000_000,     12 ,    16    ,      5_000_000_000,     1     )},
        //    {19,(10_000_000_000,     6  ,    8     ,     10_000_000_000,     1     )},
        //    {20,(20_000_000_000,     4  ,    4     ,     20_000_000_000,     1     )},
        //    {21,(20_000_000_000    ,     0  ,    1     ,       80_000_000_000,     1     )},
        //};

        public static readonly Dictionary<RFWindowType, Double> WindowGainTable = new Dictionary<RFWindowType, Double>{
            {RFWindowType.Rectangle ,0.89},
            {RFWindowType.Hann ,1.44},
            {RFWindowType.Hamming ,1.3},
            {RFWindowType.Blackman ,1.9},
            {RFWindowType.Flattop ,3.77},
            {RFWindowType.Kaiser ,2.23},
            {RFWindowType.Gaussian ,1.4468},
        };

        //public static (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain) GetRFHDScale(Int64 value, ChannelId id)
        //{
        //    if (id == ChannelId.RF)
        //    {
        //        return Get10GRFHDScale(value);
        //    }
        //    return GetRFHDScale(value);
        //}

        public static (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain) GetRFHDScale(Int64 value, ChannelId id)
        {
            Int32 bitwidth = Hd.UIMessage?.Precision?.AnaChnlBitWidth ?? 12;
            if (bitwidth == 14)
            {
                return Get5GRFHDScale(value);
            }
            else if (bitwidth == 13)
            {
                return Get10GRFHDScale(value);
            }
            return GetRFHDScale(value);
        }

        public static Double GetRFTranslateSampleRate(Int64 value, ChannelId id)
        {
            if (id == ChannelId.RF)
            {
                return Get10GRFTranslateSampleRate(value);
            }
            return GetRFTranslateSampleRate(value);
        }


        public static (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain) Get20GRFHDScale(Int64 value)
        {
            for (int i = 0; i < _20GAnalogRFSpanScaleTable.Count - 1; i++)
            {
                if (value > _20GAnalogRFSpanScaleTable[i].Span && value <= _20GAnalogRFSpanScaleTable[i + 1].Span)
                {
                    return _20GAnalogRFSpanScaleTable[i + 1];
                }
            }
            return _20GAnalogRFSpanScaleTable[_20GAnalogRFSpanScaleTable.Count - 1];
        }

        public static (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain) GetRFHDScale(Int64 value)
        {
            if (Hd.UIMessage?.Timebase?.TmbScale < 0.020)
            {
                for (int i = 0; i < _AnalogRFSpanScaleTable.Count - 1; i++)
                {
                    if (value > _AnalogRFSpanScaleTable[i].Span && value <= _AnalogRFSpanScaleTable[i + 1].Span)
                    {
                        return _AnalogRFSpanScaleTable[i + 1];
                    }
                }
                return _AnalogRFSpanScaleTable[_AnalogRFSpanScaleTable.Count - 1];
            }
            else
            {
                for (int i = 0; i < _20GAnalogRFSpanScaleTable.Count - 1; i++)
            {
                if (value > _20GAnalogRFSpanScaleTable[i].Span && value <= _20GAnalogRFSpanScaleTable[i + 1].Span)
                {
                    return _20GAnalogRFSpanScaleTable[i + 1];
                }
            }
           
                return _20GAnalogRFSpanScaleTable[_20GAnalogRFSpanScaleTable.Count - 1];
            }
            
        }

        public static Double GetRFTranslateSampleRate(Int64 value)
        {
            for (int i = 0; i < _AnalogRFSpanScaleTable.Count - 1; i++)
            {
                if (value > _AnalogRFSpanScaleTable[i].Span && value <= _AnalogRFSpanScaleTable[i + 1].Span)
                {
                    return _AnalogRFSpanScaleTable[i + 1].SampleRate;
                }
            }
            return _AnalogRFSpanScaleTable[_AnalogRFSpanScaleTable.Count - 1].SampleRate;
        }

        //private static readonly Dictionary<Int32, (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain)> _10GRFSpanScaleTable = new Dictionary<Int32, (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain)>{
        //    {0 ,(            0 / 2, 0,       1,             1,1)},
        //    {1 ,(        8_000 / 2,126,2000000,        10_000 / 2,0.3944)},//0418,RBW修改为10Hz,抽取比修改为2000000
        //    {2 ,(       25_000 / 2,118,640000 ,        31_250 / 2,0.6776)},
        //    {3 ,(       62_500 / 2,110,256000 ,        78_125 / 2,0.4441)},
        //    {4 ,(      250_000 / 2,102,64000  ,       312_500 / 2,0.4441)},
        //    {5 ,(      625_000 / 2,94,25600  ,       781_250 / 2,0.5821)},
        //    {6 ,(    2_500_000 / 2,86,6400   ,     3_125_000 / 2,0.5821)},
        //    {7 ,(    6_250_000 / 2,78,2560   ,     7_812_500 / 2,0.7629)},
        //    {8 ,(   25_000_000 / 2,70,640     ,    31_250_000 / 2,0.7629)},
        //    {9 ,(   50_000_000 / 2,68,320     ,    62_500_000 / 2,0.7629)},
        //    {10,(  100_000_000 / 2,65,160     ,   125_000_000 / 2,0.7629)},
        //    {11,(  250_000_000 / 2,6,64     ,   312_500_000 / 2,1)},
        //    {12,(  500_000_000 / 2,4,32      ,   625_000_000 / 2,1)},
        //    {13,(  1_000_000_000 / 2,1,16      , 1250_000_000 / 2,1)},
        //    {14,(2_000_000_000 / 2,0,8       , 2_500_000_000 / 2,1)},
        //    {15,(8_000_000_000 / 2, 128,1       ,20_000_000_000 / 2,1)},
        //};

        private static readonly Dictionary<Int32, (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain)> _10GRFSpanScaleTable = new Dictionary<Int32, (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain)>{
            {0 ,(             0,       0,         1,                  1,          1)},
            {1 ,(       320_000,     126,    250000,            320_000,     0.3944)},//80k
            {2 ,(       640_000,     124,    125000,            640_000,     0.3944)},
            {3 ,(     1_000_000,     118,    80000 ,          1_000_000,     0.6776)},
            {4 ,(     2_000_000,     116,    40000 ,          2_000_000,     0.6776)},
            {5 ,(     2_500_000,     110,    32000 ,          2_500_000,     0.4441)},
            {6 ,(     5_000_000,     108,    16000 ,          5_000_000,     0.4441)},
            {7 ,(    10_000_000,     102,    8000  ,         10_000_000,     0.4441)},
            {8 ,(    20_000_000,     100,    4000  ,         20_000_000,     0.4441)},
            {9 ,(    25_000_000,     94 ,    3200  ,         25_000_000,     0.5821)},
            {10,(    50_000_000,     92 ,    1600  ,         50_000_000,     0.5821)},
            {11,(   100_000_000,     86 ,    800   ,        100_000_000,     0.5821)},
            {12,(   200_000_000,     84 ,    400   ,        200_000_000,     0.5821)},
            {13,(   250_000_000,     78 ,    320   ,        250_000_000,     0.7629)},
            {14,(   500_000_000,     76 ,    160   ,        500_000_000,     0.7629)},
            {15,( 1_000_000_000,     70 ,    80    ,      1_000_000_000,     0.7629)},
            {16,( 2_000_000_000,     68 ,    40    ,      2_000_000_000,     0.7629)},
            {17,( 2_500_000_000,     14 ,    32    ,      2_500_000_000,     1     )},
            {18,( 5_000_000_000,     12 ,    16    ,      5_000_000_000,     1     )},
            {19,(10_000_000_000,     6  ,    8     ,     10_000_000_000,     1     )},
            {20,(20_000_000_000,     4  ,    4     ,     20_000_000_000,     1     )},
            {21,(20_000_000_000    ,     0  ,    1     ,       80_000_000_000,     1     )},
        };

        private static readonly Dictionary<Int32, (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain)> _5GRFSpanScaleTable = new Dictionary<Int32, (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain)>{
            {0 ,(             0,       0,         1,                  1,          1)},
            {1 ,(       320_000/4 /4,     126,    250000,            320_000/4 /4,     0.3944)},//80k
            {2 ,(       640_000/4 /4,     124,    125000,            640_000/4 /4,     0.3944)},
            {3 ,(     1_000_000/4 /4,     118,    80000 ,          1_000_000/4 /4,     0.6776)},
            {4 ,(     2_000_000/4 /4,     116,    40000 ,          2_000_000/4 /4,     0.6776)},
            {5 ,(     2_500_000/4 /4,     110,    32000 ,          2_500_000/4 /4,     0.4441)},
            {6 ,(     5_000_000/4 /4,     108,    16000 ,          5_000_000/4 /4,     0.4441)},
            {7 ,(    10_000_000/4 /4,     102,    8000  ,         10_000_000/4 /4,     0.4441)},
            {8 ,(    20_000_000/4 /4,     100,    4000  ,         20_000_000/4 /4,     0.4441)},
            {9 ,(    25_000_000/4 /4,     94 ,    3200  ,         25_000_000/4 /4,     0.5821)},
            {10,(    50_000_000/4 /4,     92 ,    1600  ,         50_000_000/4 /4,     0.5821)},
            {11,(   100_000_000/4 /4,     86 ,    800   ,        100_000_000/4 /4,     0.5821)},
            {12,(   200_000_000/4 /4,     84 ,    400   ,        200_000_000/4 /4,     0.5821)},
            {13,(   250_000_000/4 /4,     78 ,    320   ,        250_000_000/4 /4,     0.7629)},
            {14,(   500_000_000/4 /4,     76 ,    160   ,        500_000_000/4 /4,     0.7629)},
            {15,( 1_000_000_000/4 /4,     70 ,    80    ,      1_000_000_000/4 /4,     0.7629)},
            {16,( 2_000_000_000/4 /4,     68 ,    40    ,      2_000_000_000/4 /4,     0.7629)},
            {17,( 2_500_000_000/4 /4,     14 ,    32    ,      2_500_000_000/4 /4,     1     )},
            {18,( 5_000_000_000/4 /4,     12 ,    16    ,      5_000_000_000/4 /4,     1     )},
            {19,(10_000_000_000/4 /4,     6  ,    8     ,     10_000_000_000/4 /4,     1     )},
            {20,(20_000_000_000/4 /4,     4  ,    4     ,     20_000_000_000/4 /4,     1     )},
            {21,(8_000_000_000    /4,     0  ,    1     ,       20_000_000_000 /4,     1     )},
        };

        public static (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain) Get5GRFHDScale(Int64 value)
        {
            for (int i = 0; i < _5GRFSpanScaleTable.Count - 1; i++)
            {
                if (value > _5GRFSpanScaleTable[i].Span && value <= _5GRFSpanScaleTable[i + 1].Span)
                {
                    return _5GRFSpanScaleTable[i + 1];
                }
            }
            return _5GRFSpanScaleTable[_5GRFSpanScaleTable.Count - 1];
        }

        public static (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain) Get10GRFHDScale(Int64 value)
        {
            for (int i = 0; i < _10GRFSpanScaleTable.Count - 1; i++)
            {
                if (value > _10GRFSpanScaleTable[i].Span && value <= _10GRFSpanScaleTable[i + 1].Span)
                {
                    return _10GRFSpanScaleTable[i + 1];
                }
            }
            return _10GRFSpanScaleTable[_10GRFSpanScaleTable.Count - 1];
        }

        public static Double Get10GRFTranslateSampleRate(Int64 value)
        {
            for (int i = 0; i < _10GRFSpanScaleTable.Count - 1; i++)
            {
                if (value > _10GRFSpanScaleTable[i].Span && value <= _10GRFSpanScaleTable[i + 1].Span)
                {
                    return _10GRFSpanScaleTable[i + 1].SampleRate;
                }
            }
            return _10GRFSpanScaleTable[_10GRFSpanScaleTable.Count - 1].SampleRate;
        }

        public static (Int64 Span, Int32 Key, Int32 Ext, Double SampleRate, Double DDCGain) ExtGetRFSpanScaleTable(Double threoryExt)
        {
            Int64 samplerate = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquedParameters?.PerDataByfs_AtDdr == 12500 ? 80_000000000 : 20_000000000;

            var table = _AnalogRFSpanScaleTable;
            if (samplerate == 80_000000000)
            {
                table = _10GRFSpanScaleTable;
            }
            for (int i = table.Count - 1; i > 0; i--)
            {
                if (threoryExt > table[i].Ext && threoryExt <= table[i - 1].Ext + 0.001)
                {
                    return table[i - 1];
                }
            }
            return table[table.Count - 1];
        }

        internal static List<Int64> GetSpanListForTimeFreq(Int64 freqSpan)
        {
            if (Hd.UIMessage?.MultiDomain?.SynchronizationEnable ?? false)

                return _AnalogRFSpanScaleTable.Values.Select(o => (Int64)o.SampleRate).Where(o => o >= freqSpan).ToList();
            return _AnalogRFSpanScaleTable.Values.Select(o => (Int64)o.SampleRate).Where(o => o != 1).ToList();
        }


        internal static Int64 GetValidMinSpanFreq(Double maxExtramNum)
        {
            var spans = _AnalogRFSpanScaleTable.Values.Where(o => o.Ext >= maxExtramNum).Select(o => o.Span);
            return spans.Max();
        }


        #endregion
        #region GetWindowCoefficient

        public static IEnumerable<Double> GetGaussianWindow(Int32 length, Double alpha = 2.5)
        {
            Double sigma = (length - 1) / (2 * alpha);

            var w = new Double[length];

            return w.Select(o => Math.Pow(Math.E, -Math.Pow(o, 2) / (2 * Math.Pow(sigma, 2))));
        }
        public static IEnumerable<Double> GetKaiserWindow(Int32 length)
        {
            for (int i = 0; i < length; i++)
            {
                double beta = 7;
                double x = beta * Math.Sqrt(1 - Math.Pow(1 - 2 * i / (length - 1), 2));
                yield return I0function(x) / I0function(beta);
            }
        }
        public static Double I0function(Double x)
        {
            double i0 = 0;
            for (int i = 0; i < 25; i++)
            {
                double fact = Fact(i);
                i0 += Math.Pow(x, 2 * i) / Math.Pow(4, x) / Math.Pow(fact, 2);
            }
            return i0;
        }
        private static double Fact(int n)
        {
            if (n == 0)
                return 1;

            double y = n;
            for (double m = n - 1; m > 0; m--)
                y *= m;

            return y;
        }
        #endregion
        #region GetWindowCoefficient
        public static string GetWindowCoefficient()
        {
            Int32 length = Hd.UIMessage?.RadioFrequency?[(Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId)].FFTLength ?? 1024;
            RFWindowType windowType = Hd.UIMessage?.RadioFrequency?[(Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId)].Window ?? RFWindowType.Hann;

            var v = GetWindowCoefficient(length, windowType);
            StringBuilder sw = new StringBuilder();
            foreach (var d in v)
                sw.AppendLine(d.ToString());
            return sw.ToString();
        }
        public static IEnumerable<Double> GetWindowCoefficient(Int32 length, RFWindowType type = RFWindowType.Rectangle)
        {
            var w = new Double[length];
            for (Int32 i = 0; i < length; i++)
                w[i] = 2 * Math.PI * i / (length - 1);

            switch (type)
            {
                default:
                    return Enumerable.Repeat(1.0d, length);
                case RFWindowType.Hamming:
                    return w.Select(o => 0.54 - 0.46 * Math.Cos(o));
                case RFWindowType.Hann:
                    return w.Select(o => 0.5 - 0.5 * Math.Cos(o));
                case RFWindowType.Blackman:
                    return w.Select(o => 0.42 - 0.5 * Math.Cos(o) + 0.08 * Math.Cos(2 * o));
                case RFWindowType.Flattop:
                    Double fta0 = 0.215578995;
                    Double fta1 = 0.41663158;
                    Double fta2 = 0.277263158;
                    Double fta3 = 0.083578947;
                    Double fta4 = 0.006947368;
                    return w.Select(o =>
                        fta0 - fta1 * Math.Cos(o) +
                            fta2 * Math.Cos(2 * o) -
                            fta3 * Math.Cos(3 * o) +
                            fta4 * Math.Cos(4 * o));
                case RFWindowType.Kaiser:
                    return GetKaiserWindow(length);
                case RFWindowType.Gaussian:
                    return GetGaussianWindow(length);

            }
        }
        #endregion
#endif
    }

}
