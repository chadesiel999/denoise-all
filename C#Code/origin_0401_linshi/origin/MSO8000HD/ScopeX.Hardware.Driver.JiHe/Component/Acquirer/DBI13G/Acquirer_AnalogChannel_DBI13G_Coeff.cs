using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;

namespace ScopeX.Hardware.Driver
{
    public partial class Acquirer_AnalogChanel_DBI13G : AbstractAcquirer_AnalogChannel
    {
        private Boolean bFirstTimes = true;
        private UInt32 ProcBdInterpolationNum = 1;
        private List<Int32> lastChannelYScaleIndex = new();

            public static readonly Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>> AcqBoardCoefficientsTablesSendDefine = new Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>>()
            {

             //2倍插值
             [DbiCoefficientsTablesType.InterpolationCoefficients_2fold] = new List<DBI_CoefTableSendItem>()
             {
                 //CH1
                 new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.LowPass, ChannelID = ChannelId.C1, SubbandIndex = 0, FPGAIndex = 0, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\m1_ch1_sub1_interp2_ts.txt" },
                 new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.LowPass, ChannelID = ChannelId.C1, SubbandIndex = 1, FPGAIndex = 1, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\m1_ch1_sub2_interp2_ts.txt" },
                 new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.LowPass, ChannelID = ChannelId.C1, SubbandIndex = 2, FPGAIndex = 2, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\m1_ch1_sub3_interp2_ts.txt" },
                 new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.LowPass, ChannelID = ChannelId.C1, SubbandIndex = 3, FPGAIndex = 3, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\m1_ch1_sub4_interp2_ts.txt" },
             },

                //第一级插值
                [DbiCoefficientsTablesType.Level1_InterpolationCoefficients] = new List<DBI_CoefTableSendItem>()
             {
                 //CH1
                 new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.LowPass, ChannelID = ChannelId.C1, SubbandIndex = 0, FPGAIndex = 0, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level1_m1_ch1_sub1_interp4_mode1_low.txt" },
                 new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.LowPass, ChannelID = ChannelId.C1, SubbandIndex = 1, FPGAIndex = 1, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level1_m1_ch1_sub1_interp4_mode1_low.txt" },
                 new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.LowPass, ChannelID = ChannelId.C1, SubbandIndex = 2, FPGAIndex = 2, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level1_m1_ch1_sub1_interp4_mode1_low.txt" },
                 new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.LowPass, ChannelID = ChannelId.C1, SubbandIndex = 3, FPGAIndex = 3, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level1_m1_ch1_sub1_interp4_mode1_low.txt" },
             },
                //第一级本振
                [DbiCoefficientsTablesType.Level1_LocalOscillatorCoefficients] = new List<DBI_CoefTableSendItem>()
            {
                //BandMode.Full
                //Ch1
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 1, FPGAIndex = 1, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level1_LOC_Ch1_Sub_2.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 2, FPGAIndex = 2, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level1_LOC_Ch1_Sub_3.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 3, FPGAIndex = 3, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level1_LOC_Ch1_Sub_4.txt" },
            },
            //第一级抗镜像
            [DbiCoefficientsTablesType.Level1_AntiImageCoefficients] = new List<DBI_CoefTableSendItem>()
            {
               //BandMode.Full
               //Ch1
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 },
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 1,FPGAIndex=1 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level1_m1_ch1_sub2_anti.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 2,FPGAIndex=2 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level1_m1_ch1_sub3_anti.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 3,FPGAIndex=3 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level1_m1_ch1_sub4_anti.txt"},
            },

            //第二级插值
            [DbiCoefficientsTablesType.Level2_InterpolationCoefficients] = new List<DBI_CoefTableSendItem>()
            {
                //CH1
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.LowPass, ChannelID = ChannelId.C1, SubbandIndex = 0, FPGAIndex = 0, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level2_m1_ch1_sub1_interp4.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.LowPass, ChannelID = ChannelId.C1, SubbandIndex = 1, FPGAIndex = 1, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level2_m1_ch1_sub2_interp4.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.LowPass, ChannelID = ChannelId.C1, SubbandIndex = 2, FPGAIndex = 2, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level2_m1_ch1_sub3_interp4.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.LowPass, ChannelID = ChannelId.C1, SubbandIndex = 3, FPGAIndex = 3, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level2_m1_ch1_sub4_interp4.txt" },
            },
            //第二级本振
            [DbiCoefficientsTablesType.Level2_LocalOscillatorCoefficients] = new List<DBI_CoefTableSendItem>()
            {
                //BandMode.Full
                //Ch1
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 1, FPGAIndex = 1, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level2_LOC_Ch1_Sub_2.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 2, FPGAIndex = 2, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level2_LOC_Ch1_Sub_3.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 3, FPGAIndex = 3, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level2_LOC_Ch1_Sub_4.txt" },
            },
            //第二级抗镜像
            [DbiCoefficientsTablesType.Level2_AntiImageCoefficients] = new List<DBI_CoefTableSendItem>()
            {
               //BandMode.Full
               //Ch1
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 },
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 1,FPGAIndex=1 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level2_m1_ch1_sub2_anti.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 2,FPGAIndex=2 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level2_m1_ch1_sub3_anti.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 3,FPGAIndex=3 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level2_m1_ch1_sub4_anti.txt"},
            },


                //TS模式下子带幅频补偿，这个没有区分垂直档位 
            [DbiCoefficientsTablesType.Sub_AmpCoefficientFile] = new List<DBI_CoefTableSendItem>()
            {
               //BandMode.Full
               //Ch1
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level2_m1_ch1_sub2_anti.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 1,FPGAIndex=1 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level2_m1_ch1_sub2_anti.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 2,FPGAIndex=2 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level2_m1_ch1_sub3_anti.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 3,FPGAIndex=3 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level2_m1_ch1_sub4_anti.txt"},
            },


                //插值
                [DbiCoefficientsTablesType.InterpolationCoefficients] = new List<DBI_CoefTableSendItem>()
            {
                //CH1
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.LowPass, ChannelID = ChannelId.C1, SubbandIndex = 0, FPGAIndex = 0, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub1_interp4.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.Other, ChannelID = ChannelId.C1, SubbandIndex = 1, FPGAIndex = 1, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub2_interp4.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.Other, ChannelID = ChannelId.C1, SubbandIndex = 2, FPGAIndex = 2, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub3_interp4.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.Other, ChannelID = ChannelId.C1, SubbandIndex = 3, FPGAIndex = 3, DataFileName = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub4_interp4.txt" },

                ////CH2
                //new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.LowPass, ChannelID = ChannelId.C2, SubbandIndex = 0, FPGAIndex = 3, DataFileName = $@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_sub1_interp4.txt" },
                //new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.Other, ChannelID = ChannelId.C2, SubbandIndex = 1, FPGAIndex = 4, DataFileName = $@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_sub2_interp4.txt" },
                //new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.Other, ChannelID = ChannelId.C2, SubbandIndex = 2, FPGAIndex = 5, DataFileName = $@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_sub3_interp4.txt" },

                ////CH3
                //new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.LowPass, ChannelID = ChannelId.C3, SubbandIndex = 0, FPGAIndex = 6, DataFileName = $@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_sub1_interp4.txt" },
                //new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.Other, ChannelID = ChannelId.C3, SubbandIndex = 1, FPGAIndex = 7, DataFileName = $@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_sub2_interp4.txt" },
                //new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.Other, ChannelID = ChannelId.C3, SubbandIndex = 2, FPGAIndex = 8, DataFileName = $@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_sub3_interp4.txt" },

                ////CH4
                //new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.LowPass, ChannelID = ChannelId.C4, SubbandIndex = 0, FPGAIndex = 9, DataFileName = $@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_sub1_interp4.txt" },
                //new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.Other, ChannelID = ChannelId.C4, SubbandIndex = 1, FPGAIndex = 10, DataFileName = $@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_sub2_interp4.txt" },
                //new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.Other, ChannelID = ChannelId.C4, SubbandIndex = 2, FPGAIndex = 11, DataFileName = $@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_sub3_interp4.txt" },
            },
            [DbiCoefficientsTablesType.LocalOscillatorCoefficients] = new List<DBI_CoefTableSendItem>()
            {
                //BandMode.Full
                //Ch1
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 1, FPGAIndex = 1,ChnlScaleIndex=AnaChnlScaleIndex.Lv10m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_2_10mv.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 2, FPGAIndex = 2,ChnlScaleIndex=AnaChnlScaleIndex.Lv10m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_3_10mv.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 3, FPGAIndex = 2,ChnlScaleIndex=AnaChnlScaleIndex.Lv10m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_4_10mv.txt" },                //Ch1
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 1, FPGAIndex = 1,ChnlScaleIndex=AnaChnlScaleIndex.Lv20m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_2_20mv.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 2, FPGAIndex = 2,ChnlScaleIndex=AnaChnlScaleIndex.Lv20m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_3_20mv.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 3, FPGAIndex = 2,ChnlScaleIndex=AnaChnlScaleIndex.Lv20m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_4_20mv.txt" },                //Ch1
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 1, FPGAIndex = 1,ChnlScaleIndex=AnaChnlScaleIndex.Lv50m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_2_50mv.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 2, FPGAIndex = 2,ChnlScaleIndex=AnaChnlScaleIndex.Lv50m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_3_50mv.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 3, FPGAIndex = 2,ChnlScaleIndex=AnaChnlScaleIndex.Lv50m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_4_50mv.txt" },                //Ch1
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 1, FPGAIndex = 1,ChnlScaleIndex=AnaChnlScaleIndex.Lv100m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_2_100mv.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 2, FPGAIndex = 2,ChnlScaleIndex=AnaChnlScaleIndex.Lv100m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_3_100mv.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 3, FPGAIndex = 2,ChnlScaleIndex=AnaChnlScaleIndex.Lv100m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_4_100mv.txt" },                //Ch1
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 1, FPGAIndex = 1,ChnlScaleIndex=AnaChnlScaleIndex.Lv200m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_2_200mv.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 2, FPGAIndex = 2,ChnlScaleIndex=AnaChnlScaleIndex.Lv200m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_3_200mv.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 3, FPGAIndex = 2,ChnlScaleIndex=AnaChnlScaleIndex.Lv200m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_4_200mv.txt" },                //Ch1
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 1, FPGAIndex = 1,ChnlScaleIndex=AnaChnlScaleIndex.Lv500m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_2_500mv.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 2, FPGAIndex = 2,ChnlScaleIndex=AnaChnlScaleIndex.Lv500m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_3_500mv.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 3, FPGAIndex = 2,ChnlScaleIndex=AnaChnlScaleIndex.Lv500m, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_4_500mv.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 1, FPGAIndex = 1,ChnlScaleIndex=AnaChnlScaleIndex.Lv50, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_2_20mv.txt" },                //开机丢点校正使用
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 2, FPGAIndex = 2,ChnlScaleIndex=AnaChnlScaleIndex.Lv50, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_3_20mv.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 3, FPGAIndex = 2,ChnlScaleIndex=AnaChnlScaleIndex.Lv50, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_4_20mv.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 1, FPGAIndex = 1,ChnlScaleIndex=AnaChnlScaleIndex.Lv100, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_2_20mv.txt" },                //开机丢点校正使用
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 2, FPGAIndex = 2,ChnlScaleIndex=AnaChnlScaleIndex.Lv100, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_3_20mv.txt" },
                new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C1, SubbandIndex = 3, FPGAIndex = 2,ChnlScaleIndex=AnaChnlScaleIndex.Lv100, DataFileName = $@".\dbi_all_coe\LOC_Ch1_Sub_4_20mv.txt" },
                //Ch2
                //new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C2, SubbandIndex = 1, FPGAIndex = 4, DataFileName = $@".\dbi_all_coe\LOC_Ch2_Sub_2.txt" },
                //new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C2, SubbandIndex = 2, FPGAIndex = 5, DataFileName = $@".\dbi_all_coe\LOC_Ch2_Sub_3.txt" },

                ////Ch3
                //new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C3, SubbandIndex = 1, FPGAIndex = 7, DataFileName =  $@".\dbi_all_coe\LOC_Ch3_Sub_2.txt" },
                //new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C3, SubbandIndex = 2, FPGAIndex = 8, DataFileName =  $@".\dbi_all_coe\LOC_Ch3_Sub_3.txt" },

                ////Ch4
                //new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C4, SubbandIndex = 1, FPGAIndex = 10, DataFileName = $@".\dbi_all_coe\LOC_Ch4_Sub_2.txt" },
                //new() { BandMode = BandMode.Full, FilterbandMode = FilterbandMode.NotCare, ChannelID = ChannelId.C4, SubbandIndex = 2, FPGAIndex = 11, DataFileName = $@".\dbi_all_coe\LOC_Ch4_Sub_3.txt" },
            },
            [DbiCoefficientsTablesType.AntiImageCoefficients] = new List<DBI_CoefTableSendItem>()
            {
               //BandMode.Full
               //Ch1
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 },
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 1,FPGAIndex=1 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub2_anti.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 2,FPGAIndex=2 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub3_anti.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 3,FPGAIndex=3 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub4_anti.txt"},
               //Ch2
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 0,FPGAIndex=3 , DataFileName=""},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 1,FPGAIndex=4 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_sub2_anti.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 2,FPGAIndex=5 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_sub3_anti.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 3,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_sub4_anti.txt"},
               //Ch3
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=4 },
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 1,FPGAIndex=7 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_sub2_anti.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 2,FPGAIndex=8 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_sub3_anti.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 3,FPGAIndex=7 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_sub4_anti.txt"},
               //Ch4
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 0,FPGAIndex=7 , DataFileName=""},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 1,FPGAIndex=10, DataFileName=$@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_sub2_anti.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 2,FPGAIndex=11, DataFileName=$@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_sub3_anti.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 3,FPGAIndex=4 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_sub4_anti.txt"},
            },
            [DbiCoefficientsTablesType.FractionaryDelayCoefficients] = new List<DBI_CoefTableSendItem>()
            {
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 , DataFileName=""},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 1,FPGAIndex=1, DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub2_olp_fra.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 2,FPGAIndex=2, DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub3_olp_fra.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 3,FPGAIndex=3, DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub4_olp_fra.txt"},

               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 0,FPGAIndex=3 , DataFileName=""},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 1,FPGAIndex=4 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_sub2_olp_fra.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 2,FPGAIndex=5 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_sub3_olp_fra.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 3,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_sub4_olp_fra.txt"},
              
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 0,FPGAIndex=4 , DataFileName=""},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 1,FPGAIndex=7, DataFileName=$@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_sub2_olp_fra.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 2,FPGAIndex=8, DataFileName=$@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_sub3_olp_fra.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 3,FPGAIndex=7, DataFileName=$@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_sub4_olp_fra.txt"},

               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 0,FPGAIndex=7 , DataFileName=""},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 1,FPGAIndex=10, DataFileName=$@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_sub2_olp_fra.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 2,FPGAIndex=11, DataFileName=$@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_sub3_olp_fra.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 3,FPGAIndex=4 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_sub4_olp_fra.txt"},
            },
            [DbiCoefficientsTablesType.OverlapPhaseFreqDelayCoefficients] = new List<DBI_CoefTableSendItem>()
            {
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 , DataFileName=""},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 1,FPGAIndex=1 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub2_olp_phs.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 2,FPGAIndex=2 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub3_olp_phs.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 3,FPGAIndex=3 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub4_olp_phs.txt"},

               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 0,FPGAIndex=3 , DataFileName=""},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 1,FPGAIndex=4 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_sub2_olp_phs.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 2,FPGAIndex=5 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_sub3_olp_phs.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 3,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_sub4_olp_phs.txt"},

               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=4 , DataFileName=""},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 1,FPGAIndex=7 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_sub2_olp_phs.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 2,FPGAIndex=8 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_sub3_olp_phs.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 3,FPGAIndex=7 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_sub4_olp_phs.txt"},

               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 0,FPGAIndex=7 , DataFileName=""},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 1,FPGAIndex=10, DataFileName=$@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_sub2_olp_phs.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 2,FPGAIndex=11, DataFileName=$@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_sub3_olp_phs.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 3,FPGAIndex=4 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_sub4_olp_phs.txt"},
            },
            [DbiCoefficientsTablesType.TiAdc] = new List<DBI_CoefTableSendItem>()
            {
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub1_Qn.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 1,FPGAIndex=1 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub2_Qn.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 2,FPGAIndex=2 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub3_Qn.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 3,FPGAIndex=3 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_sub4_Qn.txt"},

               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 0,FPGAIndex=3 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_sub1_Qn.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 1,FPGAIndex=4 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_sub2_Qn.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 2,FPGAIndex=5 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_sub3_Qn.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 3,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_sub4_Qn.txt"},

               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 0,FPGAIndex=6 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_sub1_Qn.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 1,FPGAIndex=7 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_sub2_Qn.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 2,FPGAIndex=8 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_sub3_Qn.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 3,FPGAIndex=7 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_sub4_Qn.txt"},


               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 0,FPGAIndex=9 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_sub1_Qn.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 1,FPGAIndex=10, DataFileName=$@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_sub2_Qn.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 2,FPGAIndex=11, DataFileName=$@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_sub3_Qn.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 3,FPGAIndex=4 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_sub4_Qn.txt"},
            },

            ////========处理板的系数
            [DbiCoefficientsTablesType.AmpFreqCoefficients] = new List<DBI_CoefTableSendItem>()         //HTF 1031  DBI_DEBUG//0000
            {
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 ,ChnlScaleIndex=AnaChnlScaleIndex.Lv10m,  DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_fre_cali_10mv.txt" },
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 ,ChnlScaleIndex=AnaChnlScaleIndex.Lv20m,  DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_fre_cali_20mv.txt" },
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 ,ChnlScaleIndex=AnaChnlScaleIndex.Lv50m,  DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_fre_cali_50mv.txt" },
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 ,ChnlScaleIndex=AnaChnlScaleIndex.Lv100m, DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_fre_cali_100mv.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 ,ChnlScaleIndex=AnaChnlScaleIndex.Lv200m, DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_fre_cali_200mv.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 ,ChnlScaleIndex=AnaChnlScaleIndex.Lv500m, DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_fre_cali_500mv.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 ,ChnlScaleIndex=AnaChnlScaleIndex.Lv50, DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_fre_cali_20mv.txt"},         //校正使用
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 ,ChnlScaleIndex=AnaChnlScaleIndex.Lv100, DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_fre_cali_20mv.txt"},         //校正使用
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_ap_fre_cali.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_ap_fre_cali.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_ap_fre_cali.txt"},

               //new (){ BandMode= BandMode.Other, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_fre_cali.txt"},
               //new (){ BandMode= BandMode.Other, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_ap_fre_cali.txt"},
               //new (){ BandMode= BandMode.Other, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_ap_fre_cali.txt"},
               //new (){ BandMode= BandMode.Other, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_ap_fre_cali.txt"},
            },
            [DbiCoefficientsTablesType.PhaseFreqCoefficients] = new List<DBI_CoefTableSendItem>()//0000
            {
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 ,ChnlScaleIndex=AnaChnlScaleIndex.Lv10m,  DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_phs_cali_10mv.txt" },
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 ,ChnlScaleIndex=AnaChnlScaleIndex.Lv20m,  DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_phs_cali_20mv.txt" },
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 ,ChnlScaleIndex=AnaChnlScaleIndex.Lv50m,  DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_phs_cali_50mv.txt" },
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 ,ChnlScaleIndex=AnaChnlScaleIndex.Lv100m, DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_phs_cali_100mv.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 ,ChnlScaleIndex=AnaChnlScaleIndex.Lv200m, DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_phs_cali_200mv.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 ,ChnlScaleIndex=AnaChnlScaleIndex.Lv500m, DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_phs_cali_500mv.txt"},
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 ,ChnlScaleIndex=AnaChnlScaleIndex.Lv50, DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_phs_cali_20mv.txt"}, //校正使用
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 ,ChnlScaleIndex=AnaChnlScaleIndex.Lv100, DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\m1_ch1_ap_phs_cali_20mv.txt"}, //校正使用
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch2_coe_gen\m1_ch2_ap_phs_cali.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch3_coe_gen\m1_ch3_ap_phs_cali.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch4_coe_gen\m1_ch4_ap_phs_cali.txt"},
            },
            [DbiCoefficientsTablesType.MultiRadioInterpolationCoefficients] = new List<DBI_CoefTableSendItem>()
            {
               new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\quantized_80Gsps_DBI_319_449rders_max1000_20_50.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C2, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\quantized_80Gsps_DBI_319_449rders_max1000_20_50.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\quantized_80Gsps_DBI_319_449rders_max1000_20_50.txt"},
               //new (){ BandMode= BandMode.Full, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C4, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\quantized_80Gsps_DBI_319_449rders_max1000_20_50.txt"},

               //new (){ BandMode= BandMode.Other, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C1, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\quantized_80Gsps_DBI_319_449rders_max1000_20_50.txt"},
               //new (){ BandMode= BandMode.Other, FilterbandMode= FilterbandMode.NotCare, ChannelID=ChannelId.C3, SubbandIndex= 0,FPGAIndex=0 , DataFileName=$@".\CaliData\CoeFiles\dbi_ch1_coe_gen\quantized_80Gsps_DBI_319_449rders_max1000_20_50.txt"},

            },
        };

        internal override Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>> AcqBdCoefTablesSendDefine
        {
            get => AcqBoardCoefficientsTablesSendDefine;
        }

        #region 幅频特性系数表定义
        Dictionary<string, AmpCoefficientFileInfo>? AmpFreqCoefficientsDefine = new()
        {
            ["C1_10_Full"]  = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C1_10mV_13G.txt" },
            ["C1_20_Full"]  = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C1_20mV_13G.txt" },
            ["C1_50_Full"]  = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C1_50mV_13G.txt" },
            ["C1_100_Full"] = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C1_100mV_13G.txt" },
            ["C1_200_Full"] = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C1_200mV_13G.txt" },
            ["C1_500_Full"] = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C1_500mV_13G.txt" },

            ["C2_10_Full"]  = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C2_10mV_13G.txt" },
            ["C2_20_Full"]  = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C2_20mV_13G.txt" },
            ["C2_50_Full"]  = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C2_50mV_13G.txt" },
            ["C2_100_Full"] = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C2_100mV_13G.txt" },
            ["C2_200_Full"] = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C2_200mV_13G.txt" },
            ["C2_500_Full"] = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C2_500mV_13G.txt" },

            ["C3_10_Full"]  = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C3_10mV_13G.txt" },
            ["C3_20_Full"]  = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C3_20mV_13G.txt" },
            ["C3_50_Full"]  = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C3_50mV_13G.txt" },
            ["C3_100_Full"] = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C3_100mV_13G.txt" },
            ["C3_200_Full"] = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C3_200mV_13G.txt" },
            ["C3_500_Full"] = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C3_500mV_13G.txt" },

            ["C4_10_Full"]  = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C4_10mV_13G.txt" },
            ["C4_20_Full"]  = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C4_20mV_13G.txt" },
            ["C4_50_Full"]  = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C4_50mV_13G.txt" },
            ["C4_100_Full"] = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C4_100mV_13G.txt" },
            ["C4_200_Full"] = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C4_200mV_13G.txt" },
            ["C4_500_Full"] = new AmpCoefficientFileInfo() { FileName = $@".\dbi_all_coe\Afc_C4_500mV_13G.txt" },
        };
        internal override Dictionary<string, AmpCoefficientFileInfo>? ChannelPerScaleAmpFreqCoefficientsDefine => AmpFreqCoefficientsDefine;
        #endregion

        private void AddNewSendCoeff(DbiCoefficientsTablesType coeffType, BandMode bandMode, ChannelId channelId)
         {
            if (!CaliDataManager.DataChangedDbiCoefficientsTablesType_Running.ContainsKey(coeffType))
                CaliDataManager.DataChangedDbiCoefficientsTablesType_Running.Add(coeffType, new List<DBI_CoefTableSendItem>());
            if (!AcqBoardCoefficientsTablesSendDefine.ContainsKey(coeffType))
                return;
            List<DBI_CoefTableSendItem> currList = AcqBoardCoefficientsTablesSendDefine[coeffType];
            foreach (DBI_CoefTableSendItem item in currList)
            {
                if (item.BandMode == bandMode && item.ChannelID == channelId)
                    CaliDataManager.DataChangedDbiCoefficientsTablesType_Running[coeffType].Add(item);
            }
        }

        private UInt32 _LastRecvDbiCoefficientsCnt = 0;

        private void CheckCoefficientSend()
        {
            var msg = Hd.UIMessage;
            if((msg.Command == 0x10000)|| (msg.Command == 0x40000) || (msg.Command == 0x200))
            //if ((((msg.Command & 0x10000) == 0x10000)|| ((msg.Command & 0x40000) == 0x40000)|| ((msg.Command & 0x200) == 0x200))&&(msg.Command!=0xffffffffffffffff))
            {

            }
            else
            {
                Hd.LocalCommands |= (long)HdCmd.CaliDataChanged;
                if (!CaliDataManager.DataChangedCaliDataType.Contains(CaliDataType.DbiCoefficientsTables))
                    CaliDataManager.DataChangedCaliDataType.Add(CaliDataType.DbiCoefficientsTables);
            }

            //Hd.LocalCommands |= (long)HdCmd.CaliDataChanged;
            //if (!CaliDataManager.DataChangedCaliDataType.Contains(CaliDataType.DbiCoefficientsTables))
            //    CaliDataManager.DataChangedCaliDataType.Add(CaliDataType.DbiCoefficientsTables);

            if (bFirstTimes || _ChnlActiveChanged)
            {
                BandMode bandMode = BandMode.Full;
                lock (CaliDataManager.DbiDataChangedLocker)
                {
                    //采集板
                    foreach (ChannelId chnlId in ChannelIdExt.GetAnalogs())
                    {


                        //====InterpolationCoefficients
                        AddNewSendCoeff(DbiCoefficientsTablesType.InterpolationCoefficients_2fold, bandMode, chnlId);
                        //====InterpolationCoefficients
                        //AddNewSendCoeff(DbiCoefficientsTablesType.Level1_InterpolationCoefficients, bandMode, chnlId);
                        //====InterpolationCoefficients
                        AddNewSendCoeff(DbiCoefficientsTablesType.Level2_InterpolationCoefficients, bandMode, chnlId);

                        //====InterpolationCoefficients
                        AddNewSendCoeff(DbiCoefficientsTablesType.InterpolationCoefficients, bandMode, chnlId);
                        //====本振
                        AddNewSendCoeff(DbiCoefficientsTablesType.LocalOscillatorCoefficients, bandMode, chnlId);       //HTF   1031   DBI_DEBUG
                        //====AntiImageCoefficients
                        AddNewSendCoeff(DbiCoefficientsTablesType.AntiImageCoefficients, bandMode, chnlId);             //HTF   1031   DBI_DEBUG
                        //====FractionaryDelayCoefficients
                        AddNewSendCoeff(DbiCoefficientsTablesType.FractionaryDelayCoefficients, bandMode, chnlId);
                        //====OverlapPhaseFreqDelayCoefficients
                        AddNewSendCoeff(DbiCoefficientsTablesType.OverlapPhaseFreqDelayCoefficients, bandMode, chnlId);
                        //=====TiAdc
                        AddNewSendCoeff(DbiCoefficientsTablesType.TiAdc, bandMode, chnlId);

                    }

                    foreach (ChannelId chnlId in ChannelIdExt.GetAnalogs())
                    {
                        //AmpFreqCoefficients
                        if (!CaliDataManager.DataChangedDbiCoefficientsTablesType.ContainsKey(DbiCoefficientsTablesType.AmpFreqCoefficients))
                            AddNewSendCoeff(DbiCoefficientsTablesType.AmpFreqCoefficients, bandMode, chnlId);
                        //PhaseFreqCoefficients
                        if (!CaliDataManager.DataChangedDbiCoefficientsTablesType.ContainsKey(DbiCoefficientsTablesType.PhaseFreqCoefficients))
                            AddNewSendCoeff(DbiCoefficientsTablesType.PhaseFreqCoefficients, bandMode, chnlId);
                    }

                    //MultiRadioInterpolationCoefficients,与通道无关
                    if (!CaliDataManager.DataChangedDbiCoefficientsTablesType.ContainsKey(DbiCoefficientsTablesType.MultiRadioInterpolationCoefficients))
                        AddNewSendCoeff(DbiCoefficientsTablesType.MultiRadioInterpolationCoefficients, bandMode, 0);
                }
                bFirstTimes = false;
            }

            if (_LastRecvDbiCoefficientsCnt != HdSpecial.RecvDbiCoefficientsCnt)
            {
                BandMode bandMode = BandMode.Full;
                lock (CaliDataManager.DbiDataChangedLocker)
                {
                    //采集板
                    foreach (ChannelId chnlId in ChannelIdExt.GetAnalogs())
                    {
                        // Tool工具指定的下发类型
                        AddNewSendCoeff(CaliDataManager.LastChangedDataType, bandMode, chnlId);
                    }
                }
                _LastRecvDbiCoefficientsCnt = HdSpecial.RecvDbiCoefficientsCnt;
            }
        }

        private void CheckAmpleCoefficientSend()
        {
            for (int channelIndex = 0; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            {
                if (Hd.UIMessage!.Analog![channelIndex].Active)
                {
                    if (channelIndex < lastChannelYScaleIndex.Count && Hd.UIMessage!.Analog![channelIndex].ScaleIndex != lastChannelYScaleIndex[channelIndex])
                    {
                        bool bAdded = false;
                        if (!CaliDataManager.DataChangedDbiCoefficientsTablesType_Running.ContainsKey(DbiCoefficientsTablesType.AmpFreqCoefficients))
                        {
                            //只标明需要发送，具体发送由发送程序决定
                            List<DBI_CoefTableSendItem> currList = new List<DBI_CoefTableSendItem>();
                            var item = new DBI_CoefTableSendItem();
                            item.ChnlScaleIndex = (AnaChnlScaleIndex)Hd.UIMessage!.Analog![channelIndex].ScaleIndex;
                            currList.Add(item);
                            CaliDataManager.DataChangedDbiCoefficientsTablesType_Running.Add(DbiCoefficientsTablesType.AmpFreqCoefficients, currList);
                            bAdded = true;
                        }
                        else if (CaliDataManager.DataChangedDbiCoefficientsTablesType_Running[DbiCoefficientsTablesType.AmpFreqCoefficients].Count == 0)
                        {
                            CaliDataManager.DataChangedDbiCoefficientsTablesType_Running[DbiCoefficientsTablesType.AmpFreqCoefficients].Add(new DBI_CoefTableSendItem());
                            bAdded = true;
                            Hd.LocalCommands |= (long)HdCmd.CaliDataChanged;
                        }
                        if (bAdded)
                        {
                            Hd.LocalCommands |= (long)HdCmd.CaliDataChanged;
                            if (!CaliDataManager.DataChangedCaliDataType.Contains(CaliDataType.DbiCoefficientsTables))
                                CaliDataManager.DataChangedCaliDataType.Add(CaliDataType.DbiCoefficientsTables);
                        }
                        lastChannelYScaleIndex[channelIndex] = Hd.UIMessage!.Analog![channelIndex].ScaleIndex;
                    }
                }
            }
        }

        #region acq board

        #endregion
    }
}
