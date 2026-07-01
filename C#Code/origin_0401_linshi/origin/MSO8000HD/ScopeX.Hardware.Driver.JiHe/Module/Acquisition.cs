using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Channels;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;

namespace ScopeX.Hardware.Driver
{
    internal record PerChannelAcqParams
    {
        public UInt64 AcqBdExtractNum;
        public UInt32 AcqBdInterpolationNum;
        public UInt32 ProcBdInterpolationNum;
        public ulong DdrPointPerDataByfs;
        public ulong DmaPointPerDataByfs;
    }
    internal class AcquireAttribute
    {
        #region 通道带宽模式切换

        /// <summary>
        /// bit8 :Full BandWidth=0,Other BandWidth=1;bit7~bit0:channel 1~channel8 的激活状态
        /// </summary>
        internal Int32 CurrChBWModeAndActiveState = 0;

        /// <summary>
        /// bit8 :Full BandWidth=0,Other BandWidth=1;bit7~bit0:channel 1~channel8 的激活状态
        /// </summary>
        internal Int32 OldChBWModeAndActiveState = -1;

        /// <summary>
        /// 通道使能状态：独热码，每个bit对应一个通道的使能状态
        /// </summary>
        internal UInt32 ChnlActiveState = 0;

        internal BandMode BandMode;

        internal Int32 ActiveChnlCnt = 0;
        #endregion

        /// <summary>
        /// 当前采集属性对应的DDR写入参数
        /// </summary>
        internal WriteParams WriteParams { get; set; }

        /// <summary>
        /// 存储模式：长存储，普通存储
        /// </summary>
        internal AnaChnlStorageMode AcqStorageMode { get; set; }

        internal bool bIsLongStorageMode
        {
            get
            {
                //只有当能访问到是Scan档，并且不暂停才是Fifo模式，其他均为DDR模式
                bool isScan = Hd.UIMessage?.Timebase?.IsScan ?? false;
                if (isScan)
                {
                    if (!Hd.UIMessage?.bAcquireStopped ?? false)
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 硬件存储器（DDR或FIFO）中缓存数据的采样间隔
        /// </summary>
        internal double PerDataByfs_AtDdr { get; set; }
        public double PerDataByfs_AfterPostProcess { get; set; }

        /// <summary>
        /// DMA读取到数据的采样间隔
        /// </summary>
        internal double PerDataByfs_AtDMA { get; set; }
        #region 硬件端的前抽、后抽和插值
        /// <summary>
        /// 前抽：从ADC到FPGA存储器（DDR或者FIFO）的抽取倍率
        /// </summary>
        internal UInt64 ExtractNumFromAdc { get; set; } = 1;

        /// <summary>
        /// 后抽：从FPGA中的存储器（DDR或者FIFO）到DMA buffer的抽取倍率
        /// </summary>
        internal UInt64 ExtramNumToDMA { get; set; } = 1;

        /// <summary>
        /// 前插：从FPGA中的存储器（DDR或者FIFO）到DMA buffer的插值倍率（目前只有DBI模式下在使用，插值倍率受子带个数决定）
        /// </summary>
        internal UInt32 InterplotNumFromADC { get; set; } = 1;

        /// <summary>
        /// 后插：从FPGA中的存储器（DDR或者FIFO）到DMA buffer的插值倍率
        /// </summary>
        internal UInt32 InterplotNumToDMA { get; set; } = 1;
        #endregion

        /// <summary>
        /// 硬件的存储深度
        /// </summary>
        internal Int64 HardwareStorageWaveDotsCnt { get; set; }

        /// <summary>
        /// 硬件读回来的数据点数（非字节数）
        /// </summary>
        internal UInt32 DmaReadDotsCnt { get; set; } = 1;

        internal UInt32 DmaBytsCnt { get; set; } = 1;

        /// <summary>
        /// 设置的触发时间
        /// </summary>
        internal Double SettingTrigPositionByfs { get; set; }

        internal HdMessage? HdMessage { get; set; }

        internal AdcInterleaveMode AdcInterleaveMode { get; set; }
        /// <summary>
        /// 多少个10GADC拼合的，10G=1,20G=2;
        /// </summary>
        internal UInt32 AcqBoardAdcMegerCount { get; set; } = 2;
        /// <summary>
        /// 处理板拼合采集几个板的数据形成一个通道的数据。目前只有HB8G项目在40G模式下是2，在20G模式下是1.
        /// 其与PerDataByfs_AtDdr、HardwareExtractNum的计算有关。HB8G项目在40G模式下，PerDataByfs_AtDdr与HardwareExtractNum存在2倍关系
        /// </summary>
        internal UInt32 ProcBoardMergeAcqBoardRoadCount
        {
            get;
            set;
        } = 1;
        internal UInt32 AcqAdcMergeRoadCount
        {
            get;
            set;
        } = 80;

        internal long Scan2ExtractNum_Total
        {
            get;
            set;
        } = 1;
        internal Int32 Scan2ExtractNum_Base
        {
            get;
            set;
        } = 1;
        internal Int32 Scan2ExtractNum_Multiple
        {
            get;
            set;
        } = 1;
        internal DateTime CreateDateTime
        {
            get;
            set;
        }
        internal void CloneTo(AcquireAttribute dest)
        {
            dest.AcqStorageMode = this.AcqStorageMode;
            dest.ExtractNumFromAdc = this.ExtractNumFromAdc;
            dest.PerDataByfs_AtDdr = this.PerDataByfs_AtDdr;
            dest.PerDataByfs_AtDMA = this.PerDataByfs_AtDMA;
            dest.AdcInterleaveMode = this.AdcInterleaveMode;
            dest.SettingTrigPositionByfs = this.SettingTrigPositionByfs;
            dest.CurrChBWModeAndActiveState = this.CurrChBWModeAndActiveState;
            dest.OldChBWModeAndActiveState = this.OldChBWModeAndActiveState;
            dest.HardwareStorageWaveDotsCnt = this.HardwareStorageWaveDotsCnt;

            dest.Scan2ExtractNum_Base = this.Scan2ExtractNum_Base;
            dest.Scan2ExtractNum_Multiple = this.Scan2ExtractNum_Multiple;
            dest.Scan2ExtractNum_Total = this.Scan2ExtractNum_Total;
            dest.FrameNo = this.FrameNo;
            dest.AcqBoardAdcMegerCount = this.AcqBoardAdcMegerCount;
            dest.AcqAdcMergeRoadCount = this.AcqAdcMergeRoadCount;

            if (this.HdMessage != null)
            {
                dest.HdMessage = this.HdMessage with { };
            }
            dest.WriteParams = DeepCopy.CopyByXml(this.WriteParams);
        }
        internal ushort FrameNo = 0;
    }
    internal enum ChannelBandWidthMode
    {
        /// <summary>
        /// 全带宽模式
        /// </summary>
        Full,
        /// <summary>
        /// 对一般的情况，是降带宽模式，对多域项目来讲，是4G带宽
        /// </summary>
        Mode2,
        /// <summary>
        /// 目前仅仅对多域有效，指射频通道
        /// </summary>
        Mode3
    }
    internal class Acquisition
    {
        internal static AcquireAttribute CurrDataAcquireAttribute = new AcquireAttribute();
        public static HdMessage? AcqedDataMsg;

        private static Dictionary<AcqDataType, AbstractAcquirer> AcqDataTypeAcquirer = new Dictionary<AcqDataType, AbstractAcquirer>();
        internal static void CreateAcquirer(AbstractAcquirer?[] usedAcquirerList)
        {
            allAcquirer.AddRange(usedAcquirerList);
            foreach (var v in allAcquirer)
            {
                if (v != null)
                {
                    AcqDataTypeAcquirer.TryAdd(v.DataType, v);
                }
            }
        }
        private static List<AbstractAcquirer?> allAcquirer = new List<AbstractAcquirer?>();
        internal static void Init()
        {
            foreach (AbstractAcquirer? acquirer in allAcquirer)
            {
                acquirer?.Init();
            }
        }
        internal static long AcqReset_TimeStamp = 0;
        internal static long AcqFull_TimeStamp = 0;
        private static long LastReaded_TimeStamp = 0;

        private static Boolean bFastNeedReInit = false;
        internal static Boolean bScanFirstTimeRead = true;

        /// <summary>
        /// 停止采集
        /// </summary>
        internal static void AcqStop()
        {
            HdIO.WriteReg((uint)PcieBdReg.W.Xdma_XdmaIrqReset, 0);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Upo_SubWinXStartPos, 1);
            HdIO.WriteReg(ProcBdReg.W.RST_CTRL_SysReset, 1);//ProcBoard复位
        }

        private static void AcqReset()
        {
            /*System Reset Start*/
            HdIO.WriteReg((uint)PcieBdReg.W.AnalogChCtrl_ProbeLed, 0);//pcie test
            HdIO.WriteReg((uint)PcieBdReg.W.Xdma_XdmaIrqReset, 0);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Upo_SubWinXStartPos, 1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DdrDataSel, 1);

            HdIO.WriteReg(ProcBdReg.W.RST_CTRL_SysReset, 1);//ProcBoard复位
           

            AcqReset_TimeStamp = DateTime.Now.Ticks;

            //Upo/Dso Ctrl DDR
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.LSCtrl_ReadHold, 0);
            Hd.CurrProduct?.Acquirer_DPX?.ClearParamChangedAtStop();

            ScanRunningNewDataPerChannelExistsDotCount = 0;
            ScanPerChannelInDdrTotalDotCount_AlreadyDisplay = 0;
            ScanPerChannelInDdrDotCount_NotDisplay = 0;
            bFastNeedReInit = Hd.UIMessage!.Display!.IsFast;
            bScanFirstTimeRead = Hd.UIMessage!.Timebase!.IsScan && (!Hd.UIMessage.bAcquireStopped);

            /*System Reset Finish*/
            HdIO.WriteReg((uint)PcieBdReg.W.Xdma_XdmaIrqReset, 1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Upo_SubWinXStartPos, 0);
            //HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFifoProgFullThresh, 8000); //ti_cross_fifo_full  HTF20250806
            //HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectInt,1);//延迟丢点通道选择
            //HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayNum,0);
            ////HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayNum, 0);
            ////HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayNum, 5);
            //HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectInt, 0);
            //HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectFarrow, 1);//通道选择
            //UInt32 data = (UInt32)Math.Round(0.2 * 65536);
            //HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFracNumberDlyL16, data & 0xffff);//low 
            //HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFracNumberDlyH1, data >> 16 & 0xffff);//high
            //HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayEn, 1);

            //#region channel sync 
            //int[]? dataArray = Misc.ReadCaliCoefDataFronmFile($@"{AppDomain.CurrentDomain.BaseDirectory}CaliData\CoeFiles\farrow_10x32_ploy_5ps.txt");

            //if (dataArray != null)
            //{
            //    int dataCount = dataArray.Length;

            //    for (uint i = 0; i < dataCount; i++)
            //    {
            //        uint data = (uint)dataArray[i];
            //        HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, 1);//farrow enable      // reg_farrow_filter_en,reg_int_delay_en             
            //        HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWen, 0);//coef write enable    // reg_farrow_factor_wen           
            //        if (i == 108)
            //        {
            //            ;
            //        }
            //        HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWa, ((uint)(i / 108) << 7) + i % 108);//coef address         //reg_farrow_factor_wa                         
            //        //HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWa, i);//coef address         //reg_farrow_factor_wa                         
            //        HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWdL16, data & 0xffff);//coef data Low16bit       //reg_farrow_factor_wd_l16        
            //        HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWdH3, (data >> 16) & 0x7);//coef data High3bit  //reg_farrow_factor_wd_h3
            //        HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWen, 1);//coef write enable    //reg_farrow_factor_wen           
            //        HdIO.DelayByUs(10);
            //    }
            //    HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWen, 0);//coef write enable    //reg_farrow_factor_wen
            //}
            //#endregion

            UInt32 Cali_m_en =0;
            if (Cali_m_en != 0)
            {
                AcqBdNo acqBd = AcqBdNo.B1;
                //UInt32 ctrllineindex = 0;
                UInt32[] AcqFpgaAddrMarkTable = { 201, 202, 203, 204, 205, 0};//0 1 2 3 4 5
                //UInt32[] Shifttimes = { 0, 0,0, 0, 0, 0 };//0 1 2 3 4 5   - 左移  B6
                UInt32[] Shifttimes = { 5, 5,5,5,5, 0 };//0 1 2 3 4 5   - 左移  B7
                for (Int32 ctrllineindex = 0; ctrllineindex < 6; ctrllineindex++)
                {
                     ctrllineindex =5 ;
                    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Io_Delay_Addr, acqBd, ((UInt32)ctrllineindex) | 0x80);            //先发地址，0x80=代表是控制线
                    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Io_Delay_Value, acqBd, AcqFpgaAddrMarkTable[ctrllineindex]);
                    //UInt32 aaa = (UInt32)((ctrlLineBestTapValue[ctrllineindex].Start + ctrlLineBestTapValue[ctrllineindex].Width / 2) * scanTapStep + ScanTap_Start);
                    //if ( acqBd == AcqBdNo.B6)
                    //{
                    //    aaa = 200;
                    //}
                    //if (acqBd == AcqBdNo.B7)
                    //{
                    //    aaa = 200;
                    //}
                    //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Io_Delay_Value, acqBd, aaa);
                    Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Io_Delay_Value, 500);
                    HdIO.Sleep(5);
                    Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Io_Delay_Ld_En, 0x1);
                    HdIO.Sleep(1);
                    Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Io_Delay_Ld_En, 0x0);
                    HdIO.Sleep(1);
                    //ShiftTimes
                    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Pattern_Shift_Value, acqBd, Shifttimes[ctrllineindex]);
                    HdIO.Sleep(1);
                    //神仙都难看懂的代码和逻辑
                    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Pattern_Shift_Addr, acqBd, ((UInt32)ctrllineindex));     //通过控制地址的第7位拉高来生效数据
                    HdIO.Sleep(1);
                    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Pattern_Shift_Addr, acqBd, ((UInt32)ctrllineindex) | 0x80);     //通过控制地址的第7位拉高来生效数据
                    HdIO.Sleep(1);
                    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Pattern_Shift_Addr, acqBd, ((UInt32)ctrllineindex));     //通过控制地址的第7位拉高来生效数据
                    HdIO.Sleep(1);
                }
            }

            //Int32[] chnlList = { 0, 1, 2, 3 };
            //// UInt32[] int_delay = { 0,2,4,6 };
            //// UInt32[] delay = { 0,10000,20000,30000 };
            //UInt32[] int_delay = { 0, 63, 43, 0 };
            //double[] delay = { 0, 0000.0, 15000.0, 0 };
            //int kkk = 0;
            //foreach (Int32 chnlid in chnlList)
            //{
            //    //整数丢点
            //    HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectInt, 0b1u << chnlid);//延迟丢点通道选择
            //    HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayNum, int_delay[kkk]);
            //    HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectInt, 0);
            //    //分数延迟

            //    //UInt32 farrowDelay = ~(UInt32)(65535 * (delay[kkk] / 25000)) + 1;
            //    UInt32 farrowDelay = (UInt32)(65535 * (delay[kkk] / 25000)) + 1;
            //    HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectFarrow, 0b1u << chnlid);//通道选择
            //    HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFracNumberDlyL16, farrowDelay);//low 
            //    HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFracNumberDlyH1, farrowDelay >> 16);//high
            //    Thread.Sleep(10);
            //    HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, 1);//打开farrow使能
            //    HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectFarrow, 0);

            //    kkk += 1;

            //}
            //HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, 1);//打开farrow使能

            //HdIO.Sleep(1);
            //HdIO.WriteReg(ProcBdReg.W.Exttrig_SoftRst, 0);
            //HdIO.Sleep(1);
            //HdIO.WriteReg(ProcBdReg.W.Exttrig_SoftRst, 1);
            //HdIO.Sleep(1);
            //HdIO.WriteReg(ProcBdReg.W.Exttrig_SoftRst, 0);

            //HdIO.WriteReg(ProcBdReg.W.RST_CTRL_SysReset, 0);
            if (bScanFirstTimeRead && Hd.UIMessage!.Timebase!.IsScan)
            {
                Thread.Sleep(1);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_reg_reserve18, 0xff);
                
                //Thread.Sleep(300);
            }

            if (Hd.UIMessage!.Timebase!.IsScan)
            {
                //HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve10, 0xfe);
            }
            else
            {
               // HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve10, 0x00);
            }
            HdIO.WriteReg(ProcBdReg.W.RST_CTRL_SysReset, 0);
            //HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve22, 2500);
            //var abc = HdIO.ReadReg(ProcBdReg.R.reverse_pro_reverse_rd_reg_0);
            // HdIO.Sleep(20);

            //if (bScanFirstTimeRead && Hd.UIMessage!.Timebase!.IsScan)
            //{
            //    Thread.Sleep(300);
            //}
        }

   
        private static void StartingGunPang(Boolean bAcqReset)
        {
            if (bAcqReset)
            {
                bool bNeedReset = true;
                if (Hd.UIMessage!.bAcquireStopped)
                {
                    bNeedReset = false;
                }
                //else if (Hd.UIMessage!.Timebase!.IsScan)
                //    bNeedReset = false;
                //else if (Hd.UIMessage.Timebase.AcqLength == AnaChnlStorageMode.Fast)
                //    bNeedReset = false;

                //if (!(Hd.UIMessage!.Timebase!.IsScan || Hd.UIMessage.Timebase.AcqLength== AnaChnlStorageMode.Fast) || !Hd.UIMessage!.bAcquireStopped)
                if (bNeedReset)
                {
                    AcqReset();
                }
            }
            
            if (Hd.UIMessage!.Timebase!.IsScan)
            {
                foreach (List<ushort> channelData in AcqedDataPool.AnalogChData.AllChannelData)
                {
                    channelData.Clear();
                }
                //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, 28 * 1024 * 8 * 4);//DMA数据量
            }
        }
        internal static Boolean bReadOldData
        {
            get;
            private set;
        } = true;
        static bool is_write1 = true;
        static bool is_write2 = true;
        internal static Double LastDotOffset = 0;
        internal static void InitAcq(bool bForce)
        {
            if (bForce)
            {               
                //DoAmplitudeTemperatureCompensate();
                foreach (AbstractAcquirer? acquirer in allAcquirer)
                {
                    acquirer?.InitAcq();
                }
                ArtificialIntelligenceProcess.Default.Run();
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Adc_MonitorAdcValid, 0xf);   //no use htf0902

                HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_2, 0b0010_0001 ) ;//单板调试模式   mux  sub1-4:0x1-4  0:sub1234 all   10:1     80：子带 81：拼合
                HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve13, 0b1101);    //reg_sync_fifo_data_mode   0x07
                //HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve10, 0b0100);    //reg_debug_single_mode

                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Test_TestDataAcq, 0x00 ); //acq_test 
                                                                                           //        Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_3, 0x01);//ACQ_FD_power_en 1：ON, 0: OFF
                var clk_locked = HdIO.ReadReg(ProcBdReg.R.reverse_pro_reverse_rd_reg_1);
  //              var clk_10MHz_locked = HdIO.ReadReg(ProcBdReg.R.reverse_pro_reverse_rd_reg_0);
                //发令枪
                Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 0x00);

                Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.LSCtrl_acq_ddr_test_en, 0x00);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1Level1,100);
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1Level2, 32000);
                Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, 1);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SchmittGate_Ch1, 3200);
                
                //thj
                //HdIO.WriteReg(ProcBdReg.W.Dsp_DspEnPro, 0);//处理板DSP使能           
                HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve15, 100);//2U处理版dsp找点范围
                HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve18, 2000);//dsp_search_smt_gate
                HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve21, 1000);//interp_search_smt_gate
                HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve26, 1000);//插值找点范围
                Int32 dsp_rd_num = -750;//-700
                Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Trig_DDR_Segment_Offset, (UInt32)dsp_rd_num);
                Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Trig_Extra_DDR_Rd_Offset, (UInt32)dsp_rd_num);
                UInt16 trigger = 0;
                if (Hd.CurrDebugVarints.bEnable_DigitTrigger)
                    trigger = 1;
                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_DigitalTrigEn_Pro, 0);
                HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve12, 0x8192); //reg_sycnfifo_full
                HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_6, 0x00);//trig_source_sel
                //HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve27, 0); //interp_pre_dis_num
                HdIO.WriteReg(ProcBdReg.W.TIADC_Enable, 0);

                HdIO.WriteReg(0X404F4, 6000);//gate 3000
                HdIO.WriteReg(ProcBdReg.W.DBI_TsAfcEn, 0);
                //CtrlAnalogChannel_DBI20G.ConfigSampleFreq(ChannelId.C1,0);
                HdIO.WriteReg(ProcBdReg.W.DBI_DbiLevelOutSel,0x0000);    //3:acq data    0:normal
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_reg_reserve23, 1);//fft_cnt

                //TS校准用
                //CtrlAnalogChannel_DBI20G.ConfigSampleFreq(ChannelId.C1);
                //HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh1, 0);
                //HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh2, 0);
                //HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh3, 0);
                //HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh4, 0);
                //CtrlAnalogChannel_DBI20G.ConfigLocalFreq(_DefaultLocalFreqByHz);
                //String cmd = GetSubbandFreq(0 + 1, (Int32)(0));

                Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.Cymometer_GateTimeReset, 10000);

                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DigZoom_Gainch1_H, 2048);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DigZoom_Gainch1_L, 2048);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DigZoom_Gainch2_H, 2048);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DigZoom_Gainch2_L, 2048);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DigZoom_Gainch3_H, 2048);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DigZoom_Gainch3_L, 2048);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DigZoom_Gainch4_H, 2048);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DigZoom_Gainch4_L, 2048);

                //cij_test_0924
                //        UInt32 aaa = (UInt32)((ctrlLineBestTapValue[ctrllineindex].Start + ctrlLineBestTapValue[ctrllineindex].Width / 2) * scanTapStep + ScanTap_Start);
                //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Io_Delay_Addr, AcqBdNo.B0, ((UInt32)2) | 0x80);
                //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.BoardSync_Io_Delay_Value, AcqBdNo.B0, 300);

                //HdIO.Sleep(5);
                //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Io_Delay_Ld_En, 0x1);
                //HdIO.Sleep(1);
                //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.BoardSync_Io_Delay_Ld_En, 0x0);
                //----HTF tmp
                //----DBI fifo深度

                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_reg_reserve18, 0xff);

                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Adc_MonitorAdcValid, 0x0);     //处理版DSP使能发往采集板
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_Reverse0, 0x00);

                HdIO.WriteReg(ProcBdReg.W.DBI_AcqProgFullThreshProCh1, 15000);
                HdIO.WriteReg(ProcBdReg.W.DBI_AcqProgFullThreshProCh2, 15000);
                HdIO.WriteReg(ProcBdReg.W.DBI_AcqProgFullThreshProCh3, 15000);
                HdIO.WriteReg(ProcBdReg.W.DBI_AcqProgFullThreshProCh4, 15000);
                //----DBI sub_process_en
                //HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh1, 0xa);
                //HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh2, 0xe);
                //HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh3, 0xe);
                //HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh4, 0xe);

                //HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiCntCap64Ch2, 0x9f);
                //HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiCntGap64Ch2, 0x10);
                HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiCntCap64Ch2, 0x0);
                HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiCntCap64Ch3, 0x0);
                HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiCntCap64Ch4, 0x0);

                if (Hd.UIMessage!.Timebase!.TmbScale < 0.02)
                {
                    HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiCntGap64Ch3, 0x0);
                }
                else
                {
                    HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiCntGap64Ch3, 0x1);
                }

                //Hd.CurrDebugVarints.bEnable_Dsp = false;
                //Hd.CurrDebugVarints.bEnable_Dsp_Pro = false;

                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Trig_TrigBoardModeSelect, AcqBdNo.B0, 0x01);
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Trig_TrigBoardModeSelect, AcqBdNo.B1, 0x03);
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Trig_TrigBoardModeSelect, AcqBdNo.B2, 0x03);
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Trig_TrigBoardModeSelect, AcqBdNo.B3, 0x03);

                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrigSourceSel1Pro, 0);

                //----DBI discard
                //HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh1, 0);
                //HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh2, 0);
                //HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh3, 0);
                //HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh4, 0);

                Hd.CurrProduct.AcqBd.WriteReg(AcqBdReg.W.anormdetect_Reverse2, AcqBdNo.B1,0x9f);
                Hd.CurrProduct.AcqBd.WriteReg(AcqBdReg.W.anormdetect_Reverse3, AcqBdNo.B1,0x10);
                HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiLoRstCh3, 0x01);
                //----DBI pro_process_en
                //HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiModuleEn, 0x00);    //00子带   04拼合

                //CoefficientsTableSender_DBI.SendCoefficientsTables();

                //string tmp = "Com[1]=33013560";
                //SendCmd((ChannelId)0, tmp);

                const int INTERP_COE_NUM = 2672;
                //const int INTERP_COE_NUM = 667;
                const int LO_COE_NUM = 800;
                //const int LO_COE_NUM = 160;
                int a;
                if (true)
                    a = 1;
                else
                {
                    ////// acq1
                    // ////interp4
                    int dataCount = INTERP_COE_NUM;
                    List<int> coelist = new List<int>();
                    using (StreamReader fs_dbi = new StreamReader("C:\\Users\\syy1\\Desktop\\coe\\m1_ch1_sub1_interp4.txt"))
                    //using (StreamReader fs_dbi = new StreamReader("C:\\Users\\syy1\\Desktop\\coe\\interp_allpass.txt"))
                    { 
                        string line_dbi;
                        while ((line_dbi=fs_dbi.ReadLine())!=null)
                        {
                            line_dbi = line_dbi.Trim();
                            if (string.IsNullOrEmpty(line_dbi))
                            {
                                continue;
                            }
                            if (int.TryParse(line_dbi, out int number_dbi))
                            {
                                coelist.Add(number_dbi);
                            }
                            else
                            {
                                ;
                            }
                        }
                    }
                        int[] dataArray =coelist.ToArray();
                    //int[] dataArray = { 0, 0, 0, 0, 0, 0, 0, 1, 2, 2, 3, 3, 3, 3, 2, 1, 0, -1, -2, -2, -2, -2, -1, 0, 2, 3, 3, 3, 2, 1, -1, -3, -4, -4, -4, -2, 0, 2, 4, 5, 5, 4, 2, -1, -3, -6, -7, -7, -5, -2, 2, 5, 8, 9, 8, 5, 1, -4, -8, -11, -11, -9, -5, 1, 7, 11, 14, 13, 9, 3, -4, -11, -16, -17, -15, -9, -1, 8, 16, 20, 21, 16, 8, -3, -14, -22, -26, -24, -16, -4, 9, 21, 29, 31, 26, 14, -1, -17, -30, -36, -35, -26, -10, 9, 27, 39, 44, 39, 24, 4, -19, -39, -50, -51, -40, -19, 7, 32, 52, 61, 56, 38, 11, -21, -49, -67, -71, -59, -33, 2, 38, 67, 82, 79, 58, 22, -20, -60, -88, -96, -84, -51, -5, 44, 85, 108, 108, 83, 38, -18, -72, -112, -128, -115, -76, -17, 47, 104, 140, 144, 116, 61, -11, -84, -140, -166, -156, -109, -35, 49, 126, 177, 189, 159, 91, 0, -95, -172, -213, -206, -152, -61, 47, 149, 221, 244, 213, 132, 17, -105, -209, -268, -268, -207, -96, 41, 174, 272, 311, 281, 185, 43, -113, -250, -334, -345, -277, -144, 28, 199, 331, 392, 365, 254, 81, -117, -296, -412, -439, -366, -207, 6, 224, 400, 490, 471, 343, 133, -116, -347, -507, -556, -480, -291, -29, 249, 481, 610, 605, 460, 205, -106, -405, -621, -703, -627, -405, -81, 273, 578, 761, 777, 615, 306, -85, -471, -764, -893, -822, -560, -158, 295, 699, 958, 1007, 827, 450, -46, -552, -952, -1150, -1092, -780, -275, 315, 858, 1228, 1332, 1133, 665, 21, -657, -1217, -1523, -1492, -1115, -462, 331, 1089, 1636, 1833, 1617, 1014, 143, -813, -1637, -2131, -2160, -1690, -795, 344, 1481, 2353, 2739, 2514, 1683, 391, -1099, -2459, -3361, -3554, -2927, -1543, 353, 2376, 4070, 5000, 4852, 3513, 1122, -1932, -5049, -7519, -8640, -7849, -4841, 357, 7357, 15449, 23697, 31072, 36617, 39590, 39590, 36617, 31072, 23697, 15449, 7357, 357, -4841, -7849, -8640, -7519, -5049, -1932, 1122, 3513, 4852, 5000, 4070, 2376, 353, -1543, -2927, -3554, -3361, -2459, -1099, 391, 1683, 2514, 2739, 2353, 1481, 344, -795, -1690, -2160, -2131, -1637, -813, 143, 1014, 1617, 1833, 1636, 1089, 331, -462, -1115, -1492, -1523, -1217, -657, 21, 665, 1133, 1332, 1228, 858, 315, -275, -780, -1092, -1150, -952, -552, -46, 450, 827, 1007, 958, 699, 295, -158, -560, -822, -893, -764, -471, -85, 306, 615, 777, 761, 578, 273, -81, -405, -627, -703, -621, -405, -106, 205, 460, 605, 610, 481, 249, -29, -291, -480, -556, -507, -347, -116, 133, 343, 471, 490, 400, 224, 6, -207, -366, -439, -412, -296, -117, 81, 254, 365, 392, 331, 199, 28, -144, -277, -345, -334, -250, -113, 43, 185, 281, 311, 272, 174, 41, -96, -207, -268, -268, -209, -105, 17, 132, 213, 244, 221, 149, 47, -61, -152, -206, -213, -172, -95, 0, 91, 159, 189, 177, 126, 49, -35, -109, -156, -166, -140, -84, -11, 61, 116, 144, 140, 104, 47, -17, -76, -115, -128, -112, -72, -18, 38, 83, 108, 108, 85, 44, -5, -51, -84, -96, -88, -60, -20, 22, 58, 79, 82, 67, 38, 2, -33, -59, -71, -67, -49, -21, 11, 38, 56, 61, 52, 32, 7, -19, -40, -51, -50, -39, -19, 4, 24, 39, 44, 39, 27, 9, -10, -26, -35, -36, -30, -17, -1, 14, 26, 31, 29, 21, 9, -4, -16, -24, -26, -22, -14, -3, 8, 16, 21, 20, 16, 8, -1, -9, -15, -17, -16, -11, -4, 3, 9, 13, 14, 11, 7, 1, -5, -9, -11, -11, -8, -4, 1, 5, 8, 9, 8, 5, 2, -2, -5, -7, -7, -6, -3, -1, 2, 4, 5, 5, 4, 2, 0, -2, -4, -4, -4, -3, -1, 1, 2, 3, 3, 3, 2, 0, -1, -2, -2, -2, -2, -1, 0, 1, 2, 3, 3, 3, 3, 2, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    //同一个参数可以发向不同的FPGA，如插值系数，与通道无关

                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh1, 0);//数据类型
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh1, 0x01);//数据类型
                    for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                    {
                        //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                        UInt32 data = (UInt32)dataArray[dataIndex];
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh1, 0);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh1, (UInt32)dataIndex);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh1, data & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh1, (data >> 16) & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh1, 1);
                    }
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh1, 0);

                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh1, 0);//数据类型

                    //acq2
                    //interp4
                    int dataCount_acq2 = INTERP_COE_NUM;
                    coelist.Clear();
                    using (StreamReader fs_dbi = new StreamReader("C:\\Users\\syy1\\Desktop\\coe\\m1_ch1_sub2_interp4.txt"))
                    //using (StreamReader fs_dbi = new StreamReader("C:\\Users\\syy1\\Desktop\\coe\\interp_allpass.txt"))
                    {
                        string line_dbi;
                        while ((line_dbi = fs_dbi.ReadLine()) != null)
                        {
                            line_dbi = line_dbi.Trim();
                            if (string.IsNullOrEmpty(line_dbi))
                            {
                                continue;
                            }
                            if (int.TryParse(line_dbi, out int number_dbi))
                            {
                                coelist.Add(number_dbi);
                            }
                            else
                            {
                                ;
                            }
                        }
                    }
                    int[] dataArray_acq2 = coelist.ToArray();
                    //int[] dataArray_acq2 = { -27, -18, -22, -26, -28, -29, -29, -27, -24, -22, -19, -17, -15, -14, -15, -15, -17, -18, -18, -19, -19, -19, -19, -19, -20, -22, -24, -26, -28, -29, -29, -28, -26, -23, -20, -17, -15, -15, -17, -20, -25, -30, -35, -37, -38, -35, -30, -24, -16, -10, -6, -4, -7, -12, -20, -28, -35, -39, -40, -36, -28, -17, -5, 6, 14, 17, 14, 6, -5, -17, -27, -33, -34, -28, -15, 1, 19, 35, 46, 51, 47, 37, 21, 4, -10, -19, -19, -10, 7, 30, 55, 78, 93, 99, 94, 79, 58, 35, 15, 3, 2, 15, 38, 70, 104, 134, 154, 161, 153, 132, 103, 71, 44, 27, 27, 43, 75, 116, 160, 199, 224, 232, 220, 191, 150, 107, 70, 48, 47, 68, 109, 162, 218, 266, 297, 305, 286, 246, 192, 134, 85, 56, 53, 80, 131, 198, 268, 327, 364, 370, 343, 288, 216, 140, 76, 37, 33, 66, 130, 213, 299, 371, 414, 417, 379, 306, 211, 112, 29, -20, -26, 14, 93, 196, 301, 388, 437, 436, 383, 287, 164, 38, -67, -130, -137, -88, 10, 136, 265, 368, 424, 418, 347, 223, 66, -93, -224, -302, -311, -249, -128, 27, 184, 309, 373, 360, 267, 108, -89, -287, -450, -545, -554, -475, -324, -133, 59, 210, 285, 264, 144, -57, -302, -548, -747, -861, -869, -767, -577, -338, -101, 84, 173, 141, -12, -263, -568, -872, -1115, -1251, -1253, -1120, -878, -577, -280, -52, 56, 13, -181, -497, -878, -1253, -1552, -1713, -1706, -1528, -1215, -830, -452, -163, -28, -85, -334, -734, -1216, -1688, -2059, -2253, -2229, -1988, -1574, -1068, -575, -198, -24, -99, -423, -945, -1570, -2181, -2657, -2898, -2849, -2511, -1943, -1254, -582, -68, 172, 72, -369, -1082, -1940, -2778, -3428, -3751, -3664, -3166, -2335, -1325, -333, 434, 801, 664, 12, -1062, -2371, -3669, -4690, -5206, -5069, -4250, -2856, -1118, 640, 2058, 2804, 2642, 1489, -555, -3203, -6007, -8423, -9889, -9917, -8177, -4568, 754, 7372, 14658, 21843, 28128, 32784, 35260, 35260, 32784, 28128, 21843, 14658, 7372, 754, -4568, -8177, -9917, -9889, -8423, -6007, -3203, -555, 1489, 2642, 2804, 2058, 640, -1118, -2856, -4250, -5069, -5206, -4690, -3669, -2371, -1062, 12, 664, 801, 434, -333, -1325, -2335, -3166, -3664, -3751, -3428, -2778, -1940, -1082, -369, 72, 172, -68, -582, -1254, -1943, -2511, -2849, -2898, -2657, -2181, -1570, -945, -423, -99, -24, -198, -575, -1068, -1574, -1988, -2229, -2253, -2059, -1688, -1216, -734, -334, -85, -28, -163, -452, -830, -1215, -1528, -1706, -1713, -1552, -1253, -878, -497, -181, 13, 56, -52, -280, -577, -878, -1120, -1253, -1251, -1115, -872, -568, -263, -12, 141, 173, 84, -101, -338, -577, -767, -869, -861, -747, -548, -302, -57, 144, 264, 285, 210, 59, -133, -324, -475, -554, -545, -450, -287, -89, 108, 267, 360, 373, 309, 184, 27, -128, -249, -311, -302, -224, -93, 66, 223, 347, 418, 424, 368, 265, 136, 10, -88, -137, -130, -67, 38, 164, 287, 383, 436, 437, 388, 301, 196, 93, 14, -26, -20, 29, 112, 211, 306, 379, 417, 414, 371, 299, 213, 130, 66, 33, 37, 76, 140, 216, 288, 343, 370, 364, 327, 268, 198, 131, 80, 53, 56, 85, 134, 192, 246, 286, 305, 297, 266, 218, 162, 109, 68, 47, 48, 70, 107, 150, 191, 220, 232, 224, 199, 160, 116, 75, 43, 27, 27, 44, 71, 103, 132, 153, 161, 154, 134, 104, 70, 38, 15, 2, 3, 15, 35, 58, 79, 94, 99, 93, 78, 55, 30, 7, -10, -19, -19, -10, 4, 21, 37, 47, 51, 46, 35, 19, 1, -15, -28, -34, -33, -27, -17, -5, 6, 14, 17, 14, 6, -5, -17, -28, -36, -40, -39, -35, -28, -20, -12, -7, -4, -6, -10, -16, -24, -30, -35, -38, -37, -35, -30, -25, -20, -17, -15, -15, -17, -20, -23, -26, -28, -29, -29, -28, -26, -24, -22, -20, -19, -19, -19, -19, -19, -18, -18, -17, -15, -15, -14, -15, -17, -19, -22, -24, -27, -29, -29, -28, -26, -22, -18, -27 };
                    //同一个参数可以发向不同的FPGA，如插值系数，与通道无关
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh2, 0);//数据类型
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh2, 0x01);//数据类型
                    for (int dataIndex = 0; dataIndex < dataCount_acq2; dataIndex++)
                    {
                        //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                        UInt32 data = (UInt32)dataArray_acq2[dataIndex];
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh2, 0);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh2, (UInt32)dataIndex);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh2, data & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh2, (data >> 16) & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh2, 1);
                    }
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh2, 0);

                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh2, 0);//数据类型
                                                                                                   //需要做打开、关闭控制
                                                                                                   //lo
                    int dataCount_lo_acq2 = LO_COE_NUM;
                    coelist.Clear();
                    using (StreamReader fs_dbi = new StreamReader("C:\\Users\\syy1\\Desktop\\coe\\m1_ch1_sub2_lo.txt"))
                    {
                        string line_dbi;
                        while ((line_dbi = fs_dbi.ReadLine()) != null)
                        {
                            line_dbi = line_dbi.Trim();
                            if (string.IsNullOrEmpty(line_dbi))
                            {
                                continue;
                            }
                            if (int.TryParse(line_dbi, out int number_dbi))
                            {
                                coelist.Add(number_dbi);
                            }
                            else
                            {
                                ;
                            }
                        }
                    }
                    int[] dataArray1_lo_acq2 = coelist.ToArray();
                    //int[] dataArray1_lo_acq2 = { -42873, -65468, -38189, 18183, 60703, 56979, 9848, -44785, -65301, -36069, 20640, 61626, 55664, 7296, -46629, -65032, -33893, 23067, 62454, 54263, 4734, -48401, -64664, -31665, 25457, 63185, 52778, 2164, -50099, -64196, -29388, 27808, 63820, 51212, -409, -51719, -63628, -27065, 30117, 64355, 49567, -2982, -53259, -62963, -24701, 32379, 64792, 47846, -5550, -54717, -62201, -22299, 34591, 65129, 46051, -8109, -56091, -61342, -19862, 36749, 65365, 44185, -10656, -57379, -60390, -17395, 38851, 65500, 42250, -13187, -58578, -59344, -14901, 40893, 65535, 40251, -15697, -59686, -58206, -12384, 42873, 65468, 38189, -18183, -60703, -56979, -9848, 44785, 65301, 36069, -20640, -61626, -55664, -7296, 46629, 65032, 33893, -23067, -62454, -54263, -4734, 48401, 64664, 31665, -25457, -63185, -52778, -2164, 50099, 64196, 29388, -27808, -63820, -51212, 409, 51719, 63628, 27065, -30117, -64355, -49567, 2982, 53259, 62963, 24701, -32379, -64792, -47846, 5550, 54717, 62201, 22299, -34591, -65129, -46051, 8109, 56091, 61342, 19862, -36749, -65365, -44185, 10656, 57379, 60390, 17395, -38851, -65500, -42250, 13187, 58578, 59344, 14901, -40893, -65535, -40251, 15697, 59686, 58206, 12384 };
                    //同一个参数可以发向不同的FPGA，如插值系数，与通道无关
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh2, 0);//数据类型
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh2, 0x02);//数据类型
                    for (int dataIndex = 0; dataIndex < dataCount_lo_acq2; dataIndex++)
                    {
                        //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                        UInt32 data = (UInt32)dataArray1_lo_acq2[dataIndex];
                        //UInt32 data = (UInt32)0;
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh2, 0);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh2,(UInt32)dataIndex);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh2, data & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh2, (data >> 16) & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh2, 1);
                    }
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh2, 0);

                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh2, 0);//数据类型
                                                                                                   //需要做打开、关闭控制


                    int dataCount_anti_acq2 = 272;
                    coelist.Clear();
                    using (StreamReader fs_dbi = new StreamReader("C:\\Users\\syy1\\Desktop\\coe\\m1_ch1_sub2_anti.txt"))
                    //using (StreamReader fs_dbi = new StreamReader("C:\\Users\\syy1\\Desktop\\coe\\interp_allpass.txt"))
                    {
                        string line_dbi;
                        while ((line_dbi = fs_dbi.ReadLine()) != null)
                        {
                            line_dbi = line_dbi.Trim();
                            if (string.IsNullOrEmpty(line_dbi))
                            {
                                continue;
                            }
                            if (int.TryParse(line_dbi, out int number_dbi))
                            {
                                coelist.Add(number_dbi);
                            }
                            else
                            {
                                ;
                            }
                        }
                    }
                    int[] dataArray1_anti_acq2 = coelist.ToArray();
                    //int[] dataArray1_anti_acq2 = { -957, 4124, -1197, -907, 398, 1225, 1276, 828, 328, 100, 198, 442, 573, 437, 66, -351, -602, -582, -354, -91, 42, 4, -111, -144, -1, 276, 526, 578, 372, 6, -323, -449, -346, -137, 3, -24, -167, -261, -162, 138, 483, 657, 528, 153, -261, -484, -429, -194, 8, 20, -146, -308, -266, 49, 487, 778, 717, 299, -234, -593, -600, -325, -23, 65, -97, -331, -358, -35, 512, 951, 976, 515, -191, -746, -860, -550, -113, 108, -24, -331, -453, -137, 544, 1192, 1355, 848, -93, -948, -1257, -931, -298, 139, 91, -302, -569, -280, 583, 1552, 1962, 1418, 116, -1252, -1934, -1630, -691, 142, 292, -210, -732, -529, 644, 2233, 3178, 2635, 626, -1861, -3460, -3331, -1753, 52, 791, 96, -1110, -1209, 860, 4553, 7681, 7608, 3030, -4794, -12266, -15220, -11306, -1591, 9678, 17144, 17144, 9678, -1591, -11306, -15220, -12266, -4794, 3030, 7608, 7681, 4553, 860, -1209, -1110, 96, 791, 52, -1753, -3331, -3460, -1861, 626, 2635, 3178, 2233, 644, -529, -732, -210, 292, 142, -691, -1630, -1934, -1252, 116, 1418, 1962, 1552, 583, -280, -569, -302, 91, 139, -298, -931, -1257, -948, -93, 848, 1355, 1192, 544, -137, -453, -331, -24, 108, -113, -550, -860, -746, -191, 515, 976, 951, 512, -35, -358, -331, -97, 65, -23, -325, -600, -593, -234, 299, 717, 778, 487, 49, -266, -308, -146, 20, 8, -194, -429, -484, -261, 153, 528, 657, 483, 138, -162, -261, -167, -24, 3, -137, -346, -449, -323, 6, 372, 578, 526, 276, -1, -144, -111, 4, 42, -91, -354, -582, -602, -351, 66, 437, 573, 442, 198, 100, 328, 828, 1276, 1225, 398, -907, -1197, 4124, -957 };
                    //同一个参数可以发向不同的FPGA，如插值系数，与通道无关
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh2, 0);//数据类型
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh2, 0x04);//数据类型
                    for (int dataIndex = 0; dataIndex < dataCount_anti_acq2; dataIndex++)
                    {
                        //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                        UInt32 data = (UInt32)dataArray1_anti_acq2[dataIndex];
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh2, 0);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh2, (UInt32)dataIndex);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh2, data & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh2, (data >> 16) & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh2, 1);
                    }
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh2, 0);

                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh2, 0);//数据类型//需要做打开、关闭控制

                    //acq3
                    //interp4
                    int dataCount_acq3 = INTERP_COE_NUM;
                    coelist.Clear();
                    using (StreamReader fs_dbi = new StreamReader("C:\\Users\\syy1\\Desktop\\coe\\m1_ch1_sub3_interp4.txt"))
                    {
                        string line_dbi;
                        while ((line_dbi = fs_dbi.ReadLine()) != null)
                        {
                            line_dbi = line_dbi.Trim();
                            if (string.IsNullOrEmpty(line_dbi))
                            {
                                continue;
                            }
                            if (int.TryParse(line_dbi, out int number_dbi))
                            {
                                coelist.Add(number_dbi);
                            }
                            else
                            {
                                ;
                            }
                        }
                    }
                    int[] dataArray_acq3 = coelist.ToArray();
                    //int[] dataArray_acq3 = { -27, -18, -22, -26, -28, -29, -29, -27, -24, -22, -19, -17, -15, -14, -15, -15, -17, -18, -18, -19, -19, -19, -19, -19, -20, -22, -24, -26, -28, -29, -29, -28, -26, -23, -20, -17, -15, -15, -17, -20, -25, -30, -35, -37, -38, -35, -30, -24, -16, -10, -6, -4, -7, -12, -20, -28, -35, -39, -40, -36, -28, -17, -5, 6, 14, 17, 14, 6, -5, -17, -27, -33, -34, -28, -15, 1, 19, 35, 46, 51, 47, 37, 21, 4, -10, -19, -19, -10, 7, 30, 55, 78, 93, 99, 94, 79, 58, 35, 15, 3, 2, 15, 38, 70, 104, 134, 154, 161, 153, 132, 103, 71, 44, 27, 27, 43, 75, 116, 160, 199, 224, 232, 220, 191, 150, 107, 70, 48, 47, 68, 109, 162, 218, 266, 297, 305, 286, 246, 192, 134, 85, 56, 53, 80, 131, 198, 268, 327, 364, 370, 343, 288, 216, 140, 76, 37, 33, 66, 130, 213, 299, 371, 414, 417, 379, 306, 211, 112, 29, -20, -26, 14, 93, 196, 301, 388, 437, 436, 383, 287, 164, 38, -67, -130, -137, -88, 10, 136, 265, 368, 424, 418, 347, 223, 66, -93, -224, -302, -311, -249, -128, 27, 184, 309, 373, 360, 267, 108, -89, -287, -450, -545, -554, -475, -324, -133, 59, 210, 285, 264, 144, -57, -302, -548, -747, -861, -869, -767, -577, -338, -101, 84, 173, 141, -12, -263, -568, -872, -1115, -1251, -1253, -1120, -878, -577, -280, -52, 56, 13, -181, -497, -878, -1253, -1552, -1713, -1706, -1528, -1215, -830, -452, -163, -28, -85, -334, -734, -1216, -1688, -2059, -2253, -2229, -1988, -1574, -1068, -575, -198, -24, -99, -423, -945, -1570, -2181, -2657, -2898, -2849, -2511, -1943, -1254, -582, -68, 172, 72, -369, -1082, -1940, -2778, -3428, -3751, -3664, -3166, -2335, -1325, -333, 434, 801, 664, 12, -1062, -2371, -3669, -4690, -5206, -5069, -4250, -2856, -1118, 640, 2058, 2804, 2642, 1489, -555, -3203, -6007, -8423, -9889, -9917, -8177, -4568, 754, 7372, 14658, 21843, 28128, 32784, 35260, 35260, 32784, 28128, 21843, 14658, 7372, 754, -4568, -8177, -9917, -9889, -8423, -6007, -3203, -555, 1489, 2642, 2804, 2058, 640, -1118, -2856, -4250, -5069, -5206, -4690, -3669, -2371, -1062, 12, 664, 801, 434, -333, -1325, -2335, -3166, -3664, -3751, -3428, -2778, -1940, -1082, -369, 72, 172, -68, -582, -1254, -1943, -2511, -2849, -2898, -2657, -2181, -1570, -945, -423, -99, -24, -198, -575, -1068, -1574, -1988, -2229, -2253, -2059, -1688, -1216, -734, -334, -85, -28, -163, -452, -830, -1215, -1528, -1706, -1713, -1552, -1253, -878, -497, -181, 13, 56, -52, -280, -577, -878, -1120, -1253, -1251, -1115, -872, -568, -263, -12, 141, 173, 84, -101, -338, -577, -767, -869, -861, -747, -548, -302, -57, 144, 264, 285, 210, 59, -133, -324, -475, -554, -545, -450, -287, -89, 108, 267, 360, 373, 309, 184, 27, -128, -249, -311, -302, -224, -93, 66, 223, 347, 418, 424, 368, 265, 136, 10, -88, -137, -130, -67, 38, 164, 287, 383, 436, 437, 388, 301, 196, 93, 14, -26, -20, 29, 112, 211, 306, 379, 417, 414, 371, 299, 213, 130, 66, 33, 37, 76, 140, 216, 288, 343, 370, 364, 327, 268, 198, 131, 80, 53, 56, 85, 134, 192, 246, 286, 305, 297, 266, 218, 162, 109, 68, 47, 48, 70, 107, 150, 191, 220, 232, 224, 199, 160, 116, 75, 43, 27, 27, 44, 71, 103, 132, 153, 161, 154, 134, 104, 70, 38, 15, 2, 3, 15, 35, 58, 79, 94, 99, 93, 78, 55, 30, 7, -10, -19, -19, -10, 4, 21, 37, 47, 51, 46, 35, 19, 1, -15, -28, -34, -33, -27, -17, -5, 6, 14, 17, 14, 6, -5, -17, -28, -36, -40, -39, -35, -28, -20, -12, -7, -4, -6, -10, -16, -24, -30, -35, -38, -37, -35, -30, -25, -20, -17, -15, -15, -17, -20, -23, -26, -28, -29, -29, -28, -26, -24, -22, -20, -19, -19, -19, -19, -19, -18, -18, -17, -15, -15, -14, -15, -17, -19, -22, -24, -27, -29, -29, -28, -26, -22, -18, -27 };
                    //同一个参数可以发向不同的FPGA，如插值系数，与通道无关
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh3, 0);//数据类型
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh3, 0x01);//数据类型
                    for (int dataIndex = 0; dataIndex < dataCount_acq3; dataIndex++)
                    {
                        //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                        UInt32 data = (UInt32)dataArray_acq3[dataIndex];
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh3, 0);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh3, (UInt32)dataIndex);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh3, data & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh3, (data >> 16) & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh3, 1);
                    }
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh3, 0);

                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh3, 0);//数据类型
                                                                                                   //需要做打开、关闭控制
                                                                                                   //lo
                    int dataCount_lo_acq3 = LO_COE_NUM;
                    coelist.Clear();
                    using (StreamReader fs_dbi = new StreamReader("C:\\Users\\syy1\\Desktop\\coe\\m1_ch1_sub3_lo.txt"))
                    {
                        string line_dbi;
                        while ((line_dbi = fs_dbi.ReadLine()) != null)
                        {
                            line_dbi = line_dbi.Trim();
                            if (string.IsNullOrEmpty(line_dbi))
                            {
                                continue;
                            }
                            if (int.TryParse(line_dbi, out int number_dbi))
                            {
                                coelist.Add(number_dbi);
                            }
                            else
                            {
                                ;
                            }
                        }
                    }
                    int[] dataArray1_lo_acq3 = coelist.ToArray();
                    //int[] dataArray1_lo_acq3 = { -64760, -51343, -4944, 44632, 65536, 44340, -5340, -51589, -64698, -36244, 15492, 57277, 62267, 27257, -25263, -61554, -58302, -17598, 34412, 64315, 52903, 7505, -42713, -65493, -46200, 2772, 49963, 65058, 38360, -12980, -55982, -63021, -29575, 22870, 60623, 59433, 20063, -32196, -63771, -54380, -10056, 40729, 65349, 47989, -199, -48259, -65318, -40417, 10448, 54601, 63679, 31849, -20441, -59599, -60471, -22497, 29930, 63129, 55774, 12590, -38682, -65105, -49705, -2374, 46481, 65477, 42411, -7900, -53136, -64238, -34073, 17980, 58483, 61416, 24896, -27618, -62390, -57082, -15106, 36575, 64760, 51343, 4944, -44632, -65536, -44340, 5340, 51589, 64698, 36244, -15492, -57277, -62267, -27257, 25263, 61554, 58302, 17598, -34412, -64315, -52903, -7505, 42713, 65493, 46200, -2772, -49963, -65058, -38360, 12980, 55982, 63021, 29575, -22870, -60623, -59433, -20063, 32196, 63771, 54380, 10056, -40729, -65349, -47989, 199, 48259, 65318, 40417, -10448, -54601, -63679, -31849, 20441, 59599, 60471, 22497, -29930, -63129, -55774, -12590, 38682, 65105, 49705, 2374, -46481, -65477, -42411, 7900, 53136, 64238, 34073, -17980, -58483, -61416, -24896, 27618, 62390, 57082, 15106, -36575 };
                    // int[] dataArray = { 65535, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    //同一个参数可以发向不同的FPGA，如插值系数，与通道无关
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh3, 0);//数据类型
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh3, 0x02);//数据类型
                    for (int dataIndex = 0; dataIndex < dataCount_lo_acq3; dataIndex++)
                    {
                        //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                        UInt32 data = (UInt32)dataArray1_lo_acq3[dataIndex];
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh3, 0);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh3, (UInt32)dataIndex);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh3, data & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh3, (data >> 16) & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh3, 1);
                    }
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh3, 0);

                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh3, 0);//数据类型
                                                                                                   //需要做打开、关闭控制


                    int dataCount_anti_acq3 = 272;
                    coelist.Clear();
                    using (StreamReader fs_dbi = new StreamReader("C:\\Users\\syy1\\Desktop\\coe\\m1_ch1_sub3_anti.txt"))
                    {
                        string line_dbi;
                        while ((line_dbi = fs_dbi.ReadLine()) != null)
                        {
                            line_dbi = line_dbi.Trim();
                            if (string.IsNullOrEmpty(line_dbi))
                            {
                                continue;
                            }
                            if (int.TryParse(line_dbi, out int number_dbi))
                            {
                                coelist.Add(number_dbi);
                            }
                            else
                            {
                                ;
                            }
                        }
                    }
                    int[] dataArray1_anti_acq3 = coelist.ToArray();
                    //int[] dataArray1_anti_acq3 = { 576, 3486, -1392, 2446, 1596, -283, -937, -302, 129, -119, -226, 258, 640, 245, -501, -655, -83, 443, 353, -32, -135, 32, 18, -251, -320, 82, 526, 420, -171, -563, -336, 166, 346, 136, -42, 34, 84, -153, -418, -240, 326, 644, 283, -380, -608, -214, 259, 309, 78, 15, 144, 61, -345, -567, -145, 585, 778, 153, -604, -672, -104, 348, 271, 43, 117, 274, -13, -619, -739, 25, 935, 920, -53, -885, -721, 43, 423, 200, 41, 315, 435, -198, -1048, -922, 353, 1434, 1043, -401, -1244, -723, 238, 454, 85, 139, 695, 620, -625, -1749, -1089, 996, 2213, 1116, -1005, -1760, -672, 490, 393, -70, 506, 1479, 802, -1674, -3143, -1216, 2490, 3798, 1129, -2372, -2838, -565, 890, 99, -244, 2035, 4031, 962, -6019, -8689, -1285, 10272, 12779, 1075, -13787, -15298, -416, 15643, 15643, -416, -15298, -13787, 1075, 12779, 10272, -1285, -8689, -6019, 962, 4031, 2035, -244, 99, 890, -565, -2838, -2372, 1129, 3798, 2490, -1216, -3143, -1674, 802, 1479, 506, -70, 393, 490, -672, -1760, -1005, 1116, 2213, 996, -1089, -1749, -625, 620, 695, 139, 85, 454, 238, -723, -1244, -401, 1043, 1434, 353, -922, -1048, -198, 435, 315, 41, 200, 423, 43, -721, -885, -53, 920, 935, 25, -739, -619, -13, 274, 117, 43, 271, 348, -104, -672, -604, 153, 778, 585, -145, -567, -345, 61, 144, 15, 78, 309, 259, -214, -608, -380, 283, 644, 326, -240, -418, -153, 84, 34, -42, 136, 346, 166, -336, -563, -171, 420, 526, 82, -320, -251, 18, 32, -135, -32, 353, 443, -83, -655, -501, 245, 640, 258, -226, -119, 129, -302, -937, -283, 1596, 2446, -1392, 3486, 576 };
                    //同一个参数可以发向不同的FPGA，如插值系数，与通道无关
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh3, 0);//数据类型
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh3, 0x04);//数据类型
                    for (int dataIndex = 0; dataIndex < dataCount_anti_acq3; dataIndex++)
                    {
                        //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                        UInt32 data = (UInt32)dataArray1_anti_acq3[dataIndex];
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh3, 0);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh3, (UInt32)dataIndex);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh3, data & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh3, (data >> 16) & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh3, 1);
                    }
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh3, 0);

                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh3, 0);//数据类型

                    //acq4
                    //interp4
                    int dataCount_acq4 = INTERP_COE_NUM;
                    coelist.Clear();
                    using (StreamReader fs_dbi = new StreamReader("C:\\Users\\syy1\\Desktop\\coe\\m1_ch1_sub4_interp4.txt"))
                    {
                        string line_dbi;
                        while ((line_dbi = fs_dbi.ReadLine()) != null)
                        {
                            line_dbi = line_dbi.Trim();
                            if (string.IsNullOrEmpty(line_dbi))
                            {
                                continue;
                            }
                            if (int.TryParse(line_dbi, out int number_dbi))
                            {
                                coelist.Add(number_dbi);
                            }
                            else
                            {
                                ;
                            }
                        }
                    }
                    int[] dataArray_acq4 = coelist.ToArray();
                    //int[] dataArray_acq4 = { -27, 5, -1, -8, -12, -13, -10, -5, 1, 6, 8, 8, 5, 2, -2, -5, -7, -6, -4, 0, 5, 10, 15, 18, 20, 20, 18, 14, 9, 5, 2, 1, 2, 6, 12, 18, 24, 28, 29, 26, 20, 11, 1, -8, -15, -19, -18, -14, -7, 1, 8, 11, 11, 5, -5, -18, -33, -46, -55, -58, -55, -47, -35, -21, -8, 0, 3, -2, -12, -27, -42, -54, -61, -60, -50, -32, -9, 15, 37, 52, 59, 56, 44, 27, 9, -6, -13, -10, 4, 27, 56, 86, 112, 127, 129, 118, 95, 65, 33, 5, -12, -16, -6, 16, 45, 75, 97, 105, 97, 72, 32, -17, -66, -108, -135, -144, -132, -105, -68, -31, -2, 10, 2, -28, -74, -129, -183, -224, -245, -240, -209, -157, -93, -29, 23, 55, 59, 37, -5, -59, -109, -144, -152, -129, -74, 5, 96, 184, 255, 295, 300, 269, 211, 138, 68, 17, -2, 16, 71, 153, 247, 333, 394, 414, 389, 318, 214, 94, -22, -112, -162, -164, -119, -42, 50, 132, 183, 186, 133, 29, -112, -268, -411, -518, -569, -556, -485, -369, -236, -112, -26, 3, -34, -129, -266, -415, -545, -625, -634, -564, -422, -229, -17, 178, 323, 394, 383, 296, 158, 4, -127, -199, -188, -85, 100, 338, 590, 811, 961, 1013, 956, 804, 586, 347, 135, -6, -47, 21, 184, 408, 641, 831, 930, 905, 749, 478, 133, -230, -551, -775, -868, -819, -648, -400, -136, 77, 182, 141, -53, -376, -778, -1186, -1525, -1730, -1759, -1602, -1288, -873, -438, -65, 171, 227, 93, -198, -581, -966, -1258, -1376, -1272, -936, -408, 234, 888, 1443, 1806, 1921, 1777, 1417, 929, 430, 43, -128, -20, 375, 1004, 1762, 2510, 3101, 3413, 3368, 2957, 2240, 1339, 416, -355, -828, -913, -597, 48, 875, 1686, 2271, 2450, 2109, 1228, -104, -1707, -3331, -4707, -5595, -5838, -5395, -4363, -2968, -1530, -413, 46, -396, -1831, -4165, -7112, -10224, -12951, -14721, -15034, -13552, -10162, -5016, 1472, 8668, 15799, 22054, 26697, 29169, 29169, 26697, 22054, 15799, 8668, 1472, -5016, -10162, -13552, -15034, -14721, -12951, -10224, -7112, -4165, -1831, -396, 46, -413, -1530, -2968, -4363, -5395, -5838, -5595, -4707, -3331, -1707, -104, 1228, 2109, 2450, 2271, 1686, 875, 48, -597, -913, -828, -355, 416, 1339, 2240, 2957, 3368, 3413, 3101, 2510, 1762, 1004, 375, -20, -128, 43, 430, 929, 1417, 1777, 1921, 1806, 1443, 888, 234, -408, -936, -1272, -1376, -1258, -966, -581, -198, 93, 227, 171, -65, -438, -873, -1288, -1602, -1759, -1730, -1525, -1186, -778, -376, -53, 141, 182, 77, -136, -400, -648, -819, -868, -775, -551, -230, 133, 478, 749, 905, 930, 831, 641, 408, 184, 21, -47, -6, 135, 347, 586, 804, 956, 1013, 961, 811, 590, 338, 100, -85, -188, -199, -127, 4, 158, 296, 383, 394, 323, 178, -17, -229, -422, -564, -634, -625, -545, -415, -266, -129, -34, 3, -26, -112, -236, -369, -485, -556, -569, -518, -411, -268, -112, 29, 133, 186, 183, 132, 50, -42, -119, -164, -162, -112, -22, 94, 214, 318, 389, 414, 394, 333, 247, 153, 71, 16, -2, 17, 68, 138, 211, 269, 300, 295, 255, 184, 96, 5, -74, -129, -152, -144, -109, -59, -5, 37, 59, 55, 23, -29, -93, -157, -209, -240, -245, -224, -183, -129, -74, -28, 2, 10, -2, -31, -68, -105, -132, -144, -135, -108, -66, -17, 32, 72, 97, 105, 97, 75, 45, 16, -6, -16, -12, 5, 33, 65, 95, 118, 129, 127, 112, 86, 56, 27, 4, -10, -13, -6, 9, 27, 44, 56, 59, 52, 37, 15, -9, -32, -50, -60, -61, -54, -42, -27, -12, -2, 3, 0, -8, -21, -35, -47, -55, -58, -55, -46, -33, -18, -5, 5, 11, 11, 8, 1, -7, -14, -18, -19, -15, -8, 1, 11, 20, 26, 29, 28, 24, 18, 12, 6, 2, 1, 2, 5, 9, 14, 18, 20, 20, 18, 15, 10, 5, 0, -4, -6, -7, -5, -2, 2, 5, 8, 8, 6, 1, -5, -10, -13, -12, -8, -1, 5, -27 };
                    //同一个参数可以发向不同的FPGA，如插值系数，与通道无关
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh4, 0);//数据类型
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh4, 0x01);//数据类型
                    for (int dataIndex = 0; dataIndex < dataCount_acq4; dataIndex++)
                    {
                        //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                        UInt32 data = (UInt32)dataArray_acq4[dataIndex];
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh4, 0);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh4, (UInt32)dataIndex);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh4, data & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh4, (data >> 16) & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh4, 1);
                    }
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh4, 0);

                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh4, 0);//数据类型
                                                                                                   //需要做打开、关闭控制
                                                                                                   //lo
                    int dataCount_lo_acq4 = LO_COE_NUM;
                    coelist.Clear();
                    using (StreamReader fs_dbi = new StreamReader("C:\\Users\\syy1\\Desktop\\coe\\m1_ch1_sub4_lo.txt"))
                    {
                        string line_dbi;
                        while ((line_dbi = fs_dbi.ReadLine()) != null)
                        {
                            line_dbi = line_dbi.Trim();
                            if (string.IsNullOrEmpty(line_dbi))
                            {
                                continue;
                            }
                            if (int.TryParse(line_dbi, out int number_dbi))
                            {
                                coelist.Add(number_dbi);
                            }
                            else
                            {
                                ;
                            }
                        }
                    }
                    int[] dataArray1_lo_acq4 = coelist.ToArray();
                    //int[] dataArray1_lo_acq4 = { 35991, -34670, -65021, -19773, 48464, 60353, 2071, -58619, -51154, 15787, 64373, 38114, -32460, -65293, -22211, 46695, 61310, 4641, -57424, -52723, 13278, 63841, 40177, -30199, -65464, -24615, 44853, 62171, 7204, -56139, -54210, 10748, 63210, 42179, -27893, -65534, -26980, 42943, 62937, 9756, -54769, -55615, 8202, 62482, 44116, -25543, -65503, -29304, 40966, 63606, 12292, -53313, -56933, 5643, 61657, 45984, -23154, -65371, -31583, 38926, 64177, 14810, -51776, -58163, 3075, 60738, 47782, -20729, -65139, -33813, 36826, 64649, 17305, -50159, -59304, 502, 59725, 49506, -18272, -64806, -35991, 34670, 65021, 19773, -48464, -60353, -2071, 58619, 51154, -15787, -64373, -38114, 32460, 65293, 22211, -46695, -61310, -4641, 57424, 52723, -13278, -63841, -40177, 30199, 65464, 24615, -44853, -62171, -7204, 56139, 54210, -10748, -63210, -42179, 27893, 65534, 26980, -42943, -62937, -9756, 54769, 55615, -8202, -62482, -44116, 25543, 65503, 29304, -40966, -63606, -12292, 53313, 56933, -5643, -61657, -45984, 23154, 65371, 31583, -38926, -64177, -14810, 51776, 58163, -3075, -60738, -47782, 20729, 65139, 33813, -36826, -64649, -17305, 50159, 59304, -502, -59725, -49506, 18272, 64806 };
                    //同一个参数可以发向不同的FPGA，如插值系数，与通道无关
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh4, 0);//数据类型
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh4, 0x02);//数据类型
                    for (int dataIndex = 0; dataIndex < dataCount_lo_acq4; dataIndex++)
                    {
                        //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                        UInt32 data = (UInt32)dataArray1_lo_acq4[dataIndex];
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh4, 0);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh4, (UInt32)dataIndex);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh4, data & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh4, (data >> 16) & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh4, 1);
                    }
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh4, 0);

                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh4, 0);//数据类型
                                                                                                   //需要做打开、关闭控制


                    int dataCount_anti_acq4 = 272;
                    coelist.Clear();
                    using (StreamReader fs_dbi = new StreamReader("C:\\Users\\syy1\\Desktop\\coe\\m1_ch1_sub4_anti.txt"))
                    {
                        string line_dbi;
                        while ((line_dbi = fs_dbi.ReadLine()) != null)
                        {
                            line_dbi = line_dbi.Trim();
                            if (string.IsNullOrEmpty(line_dbi))
                            {
                                continue;
                            }
                            if (int.TryParse(line_dbi, out int number_dbi))
                            {
                                coelist.Add(number_dbi);
                            }
                            else
                            {
                                ;
                            }
                        }
                    }
                    int[] dataArray1_anti_acq4 = coelist.ToArray();
                    //int[] dataArray1_anti_acq4 = { -2224, 387, -874, -143, 1004, 735, -213, -272, 376, 554, 158, -29, 136, 195, 131, 222, 266, 7, -143, 195, 480, 138, -298, -62, 435, 334, -139, -172, 166, 233, 63, 59, 82, -96, -75, 308, 359, -195, -431, 173, 610, 85, -495, -158, 422, 281, -156, -129, 57, -17, 39, 316, 120, -456, -299, 548, 591, -392, -718, 199, 750, 68, -542, -159, 281, 116, 18, 169, -127, -507, 65, 848, 234, -971, -580, 887, 881, -569, -933, 215, 737, 48, -307, -26, -144, -279, 458, 814, -447, -1342, 126, 1676, 413, -1634, -902, 1245, 1123, -651, -899, 154, 281, -8, 536, 379, -1208, -1190, 1443, 2183, -1084, -2953, 249, 3165, 722, -2661, -1336, 1611, 1183, -455, -75, -188, -1786, -222, 3861, 1964, -5379, -4865, 5622, 8337, -4146, -11486, 996, 13406, 3286, -13443, -7773, 11439, 11439, -7773, -13443, 3286, 13406, 996, -11486, -4146, 8337, 5622, -4865, -5379, 1964, 3861, -222, -1786, -188, -75, -455, 1183, 1611, -1336, -2661, 722, 3165, 249, -2953, -1084, 2183, 1443, -1190, -1208, 379, 536, -8, 281, 154, -899, -651, 1123, 1245, -902, -1634, 413, 1676, 126, -1342, -447, 814, 458, -279, -144, -26, -307, 48, 737, 215, -933, -569, 881, 887, -580, -971, 234, 848, 65, -507, -127, 169, 18, 116, 281, -159, -542, 68, 750, 199, -718, -392, 591, 548, -299, -456, 120, 316, 39, -17, 57, -129, -156, 281, 422, -158, -495, 85, 610, 173, -431, -195, 359, 308, -75, -96, 82, 59, 63, 233, 166, -172, -139, 334, 435, -62, -298, 138, 480, 195, -143, 7, 266, 222, 131, 195, 136, -29, 158, 554, 376, -272, -213, 735, 1004, -143, -874, 387, -2224 };
                    //同一个参数可以发向不同的FPGA，如插值系数，与通道无关
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh4, 0);//数据类型
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh4, 0x04);//数据类型
                    for (int dataIndex = 0; dataIndex < dataCount_anti_acq4; dataIndex++)
                    {
                        //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                        UInt32 data = (UInt32)dataArray1_anti_acq4[dataIndex];
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh4, 0);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh4, (UInt32)dataIndex);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh4, data & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh4, (data >> 16) & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh4, 1);
                    }
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh4, 0);

                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh4, 0);//数据类型

                    //amp
                    int dataCount_pro3 = 800;
                    coelist.Clear();
                    using (StreamReader fs_dbi = new StreamReader("C:\\Users\\syy1\\Desktop\\coe\\m1_ch1_ap_fre_cali.txt"))
                    {
                        string line_dbi;
                        while ((line_dbi = fs_dbi.ReadLine()) != null)
                        {
                            line_dbi = line_dbi.Trim();
                            if (string.IsNullOrEmpty(line_dbi))
                            {
                                continue;
                            }
                            if (int.TryParse(line_dbi, out int number_dbi))
                            {
                                coelist.Add(number_dbi);
                            }
                            else
                            {
                                ;
                            }
                        }
                    }
                    int[] dataArray_pro3 = coelist.ToArray();
                    //int[] dataArray_pro3 = { -26, 60, 51, -84, -134, 25, 211, 203, 42, -107, -199, -210, -39, 210, 187, -127, -190, 241, 530, 143, -333, -104, 365, 40, -837, -1024, -305, 281, 279, 323, 608, 509, 72, 171, 720, 507, -568, -981, -158, 390, -368, -1155, -572, 559, 679, -41, -386, -150, 73, 215, 438, 355, -189, -441, 15, 343, -104, -551, -185, 377, 280, -48, 108, 330, -3, -392, -182, 151, -33, -268, 44, 387, 119, -265, -116, 146, -44, -250, 2, 200, -106, -319, 51, 388, 187, 18, 268, 286, -197, -290, 334, 562, -158, -643, -90, 444, 54, -362, 36, 374, -90, -379, 233, 580, -209, -828, -105, 722, 207, -678, -423, 279, 85, -368, 102, 702, 178, -605, -207, 592, 227, -662, -434, 464, 382, -436, -426, 335, 448, -107, -140, 313, 181, -404, -366, 172, 175, -251, -147, 333, 257, -202, -137, 275, 158, -298, -258, 85, -30, -339, -103, 339, 218, -126, 55, 383, 116, -318, -148, 198, -35, -379, -69, 376, 125, -329, -132, 280, 101, -241, -14, 290, -33, -401, -66, 350, 51, -327, 34, 455, 44, -527, -261, 300, 196, -125, 128, 401, -56, -532, -139, 421, 158, -336, -109, 285, -40, -475, -105, 457, 255, -171, 62, 405, 39, -393, -29, 457, 155, -300, -14, 388, 56, -334, 47, 430, -70, -612, -171, 450, 113, -493, -215, 352, 77, -515, -319, 232, 112, -298, -84, 375, 268, 14, 341, 740, 463, -20, -57, -95, -529, -617, 33, 381, -227, -706, -184, 375, 39, -333, 141, 599, 134, -412, -98, 325, -6, -363, 20, 367, -49, -398, 12, 299, -193, -475, 142, 584, 66, -361, 105, 428, -172, -616, -96, 318, -160, -437, 205, 546, -173, -602, 171, 774, 122, -540, -71, 432, -89, -564, 18, 585, 131, -344, 68, 365, -215, -535, 144, 550, -142, -617, 39, 567, 50, -324, 252, 547, -169, -556, 108, 403, -390, -735, 150, 653, -135, -656, 32, 448, -273, -619, 223, 647, -220, -758, -15, 526, 15, -168, 545, 604, -412, -714, 148, 280, -708, -736, 561, 811, -553, -1032, 258, 1029, 321, 27, 637, 196, -1257, -1193, 334, 373, -1170, -1110, 734, 967, -692, -718, 1157, 1194, -863, -1046, 866, 651, -1744, -1563, 1270, 1383, -1511, -1466, 2113, 2648, -1060, -2138, 1096, 1638, -2300, -3147, 1146, 2173, -2806, -4567, 832, 3871, -635, -3515, 836, 3191, -2090, -4790, 1308, 4556, -2379, -5786, 3893, 10197, -908, -9755, 8060, 35841, 35841, 8060, -9755, -908, 10197, 3893, -5786, -2379, 4556, 1308, -4790, -2090, 3191, 836, -3515, -635, 3871, 832, -4567, -2806, 2173, 1146, -3147, -2300, 1638, 1096, -2138, -1060, 2648, 2113, -1466, -1511, 1383, 1270, -1563, -1744, 651, 866, -1046, -863, 1194, 1157, -718, -692, 967, 734, -1110, -1170, 373, 334, -1193, -1257, 196, 637, 27, 321, 1029, 258, -1032, -553, 811, 561, -736, -708, 280, 148, -714, -412, 604, 545, -168, 15, 526, -15, -758, -220, 647, 223, -619, -273, 448, 32, -656, -135, 653, 150, -735, -390, 403, 108, -556, -169, 547, 252, -324, 50, 567, 39, -617, -142, 550, 144, -535, -215, 365, 68, -344, 131, 585, 18, -564, -89, 432, -71, -540, 122, 774, 171, -602, -173, 546, 205, -437, -160, 318, -96, -616, -172, 428, 105, -361, 66, 584, 142, -475, -193, 299, 12, -398, -49, 367, 20, -363, -6, 325, -98, -412, 134, 599, 141, -333, 39, 375, -184, -706, -227, 381, 33, -617, -529, -95, -57, -20, 463, 740, 341, 14, 268, 375, -84, -298, 112, 232, -319, -515, 77, 352, -215, -493, 113, 450, -171, -612, -70, 430, 47, -334, 56, 388, -14, -300, 155, 457, -29, -393, 39, 405, 62, -171, 255, 457, -105, -475, -40, 285, -109, -336, 158, 421, -139, -532, -56, 401, 128, -125, 196, 300, -261, -527, 44, 455, 34, -327, 51, 350, -66, -401, -33, 290, -14, -241, 101, 280, -132, -329, 125, 376, -69, -379, -35, 198, -148, -318, 116, 383, 55, -126, 218, 339, -103, -339, -30, 85, -258, -298, 158, 275, -137, -202, 257, 333, -147, -251, 175, 172, -366, -404, 181, 313, -140, -107, 448, 335, -426, -436, 382, 464, -434, -662, 227, 592, -207, -605, 178, 702, 102, -368, 85, 279, -423, -678, 207, 722, -105, -828, -209, 580, 233, -379, -90, 374, 36, -362, 54, 444, -90, -643, -158, 562, 334, -290, -197, 286, 268, 18, 187, 388, 51, -319, -106, 200, 2, -250, -44, 146, -116, -265, 119, 387, 44, -268, -33, 151, -182, -392, -3, 330, 108, -48, 280, 377, -185, -551, -104, 343, 15, -441, -189, 355, 438, 215, 73, -150, -386, -41, 679, 559, -572, -1155, -368, 390, -158, -981, -568, 507, 720, 171, 72, 509, 608, 323, 279, 281, -305, -1024, -837, 40, 365, -104, -333, 143, 530, 241, -190, -127, 187, 210, -39, -210, -199, -107, 42, 203, 211, 25, -134, -84, 51, 60, -26 };
                    // int[] dataArray = { 65535, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    //同一个参数可以发向不同的FPGA，如插值系数，与通道无关
                    HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, 0);
                    HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, 1);
                    for (int dataIndex = 0; dataIndex < dataCount_pro3; dataIndex++)
                    {
                        //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                        UInt32 data = (UInt32)dataArray_pro3[dataIndex];
                        HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWen, 0);
                        HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWa, (UInt32)dataIndex);
                        HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWdLow, data & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWdHigh, (data >> 16) & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWen, 1);
                    }
                    HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWen, 0);
                    //需要做打开、关闭控制
                    HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, 0);


                    //phs
                    int dataCount_pro_phs3 = 1500;
                    int[] dataArray_pro_phs3 = { 61723, -127194, 65536, 127194, -61723, 61644, -127059, 65536, 127059, -61644, 61666, -126980, 65536, 126980, -61666, 61735, -126903, 65536, 126903, -61735, 61704, -126668, 65536, 126668, -61704, 61726, -126433, 65536, 126433, -61726, 61731, -126136, 65536, 126136, -61731, 61716, -125761, 65536, 125761, -61716, 61729, -125365, 65536, 125365, -61729, 61771, -124951, 65536, 124951, -61771, 61720, -124395, 65536, 124395, -61720, 61710, -123819, 65536, 123819, -61710, 61641, -123119, 65536, 123119, -61641, 61630, -122442, 65536, 122442, -61630, 61546, -121595, 65536, 121595, -61546, 61540, -120789, 65536, 120789, -61540, 61595, -119968, 65536, 119968, -61595, 61516, -118977, 65536, 118977, -61516, 61532, -117918, 65536, 117918, -61532, 61378, -116595, 65536, 116595, -61378, 61251, -115206, 65536, 115206, -61251, 55049, -109506, 65536, 109506, -55049, 62136, -112618, 65536, 112618, -62136, 62477, -111965, 65536, 111965, -62477, 62702, -111292, 65536, 111292, -62702, 62886, -110549, 65536, 110549, -62886, 62977, -109685, 65536, 109685, -62977, 63012, -108760, 65536, 108760, -63012, 63024, -107810, 65536, 107810, -63024, 63023, -106841, 65536, 106841, -63023, 63026, -105855, 65536, 105855, -63026, 63030, -104852, 65536, 104852, -63030, 63036, -103832, 65536, 103832, -63036, 63047, -102806, 65536, 102806, -63047, 63051, -101764, 65536, 101764, -63051, 63050, -100701, 65536, 100701, -63050, 63044, -99614, 65536, 99614, -63044, 63038, -98503, 65536, 98503, -63038, 63036, -97377, 65536, 97377, -63036, 63026, -96231, 65536, 96231, -63026, 63002, -95053, 65536, 95053, -63002, 62980, -93842, 65536, 93842, -62980, 62974, -92623, 65536, 92623, -62974, 62961, -91379, 65536, 91379, -62961, 62945, -90109, 65536, 90109, -62945, 62907, -88807, 65536, 88807, -62907, 62821, -87436, 65536, 87436, -62821, 62692, -85958, 65536, 85958, -62692, 62632, -84421, 65536, 84421, -62632, 62778, -83014, 65536, 83014, -62778, 63003, -81791, 65536, 81791, -63003, 63171, -80618, 65536, 80618, -63171, 63287, -79456, 65536, 79456, -63287, 63357, -78298, 65536, 78298, -63357, 63385, -77124, 65536, 77124, -63385, 63391, -75928, 65536, 75928, -63391, 63391, -74717, 65536, 74717, -63391, 63395, -73503, 65536, 73503, -63395, 63403, -72288, 65536, 72288, -63403, 63411, -71070, 65536, 71070, -63411, 63417, -69847, 65536, 69847, -63417, 63422, -68618, 65536, 68618, -63422, 63430, -67383, 65536, 67383, -63430, 63441, -66145, 65536, 66145, -63441, 63449, -64905, 65536, 64905, -63449, 63447, -63656, 65536, 63656, -63447, 63441, -62392, 65536, 62392, -63441, 63438, -61118, 65536, 61118, -63438, 63442, -59839, 65536, 59839, -63442, 63451, -58560, 65536, 58560, -63451, 63463, -57280, 65536, 57280, -63463, 63472, -55998, 65536, 55998, -63472, 63471, -54706, 65536, 54706, -63471, 63466, -53405, 65536, 53405, -63466, 63459, -52093, 65536, 52093, -63459, 63452, -50771, 65536, 50771, -63452, 63443, -49439, 65536, 49439, -63443, 63420, -48089, 65536, 48089, -63420, 63385, -46705, 65536, 46705, -63385, 63353, -45296, 65536, 45296, -63353, 63323, -43867, 65536, 43867, -63323, 63286, -42421, 65536, 42421, -63286, 63238, -40935, 65536, 40935, -63238, 63237, -39417, 65536, 39417, -63237, 63321, -37943, 65536, 37943, -63321, 63444, -36563, 65536, 36563, -63444, 63551, -35255, 65536, 35255, -63551, 63629, -33989, 65536, 33989, -63629, 63681, -32747, 65536, 32747, -63681, 63713, -31520, 65536, 31520, -63713, 63735, -30303, 65536, 30303, -63735, 63752, -29092, 65536, 29092, -63752, 63767, -27889, 65536, 27889, -63767, 63779, -26692, 65536, 26692, -63779, 63789, -25499, 65536, 25499, -63789, 63802, -24309, 65536, 24309, -63802, 63820, -23127, 65536, 23127, -63820, 63838, -21954, 65536, 21954, -63838, 63848, -20785, 65536, 20785, -63848, 63839, -19612, 65536, 19612, -63839, 63797, -18420, 65536, 18420, -63797, 63702, -17184, 65536, 17184, -63702, 63517, -15870, 65536, 15870, -63517, 63173, -14403, 65536, 14403, -63173, 62744, -12565, 65536, 12565, -62744, 62217, -10477, 65536, 10477, -62217, 62468, -8047, 65536, 8047, -62468, 62717, -6388, 65536, 6388, -62717, 62510, -4444, 65536, 4444, -62510, 62993, -2341, 65536, 2341, -62993, 63469, -895, 65536, 895, -63469, 63738, 361, 65536, -361, -63738, 63889, 1536, 65536, -1536, -63889, 63970, 2658, 65536, -2658, -63970, 64010, 3744, 65536, -3744, -64010, 64027, 4812, 65536, -4812, -64027, 64030, 5871, 65536, -5871, -64030, 64026, 6929, 65536, -6929, -64026, 64019, 7989, 65536, -7989, -64019, 64011, 9052, 65536, -9052, -64011, 64004, 10119, 65536, -10119, -64004, 63999, 11188, 65536, -11188, -63999, 63996, 12260, 65536, -12260, -63996, 63993, 13331, 65536, -13331, -63993, 63991, 14402, 65536, -14402, -63991, 63991, 15473, 65536, -15473, -63991, 63992, 16542, 65536, -16542, -63992, 63993, 17612, 65536, -17612, -63993, 63992, 18680, 65536, -18680, -63992, 63992, 19746, 65536, -19746, -63992, 63992, 20809, 65536, -20809, -63992, 63993, 21872, 65536, -21872, -63993, 63994, 22934, 65536, -22934, -63994, 63993, 23994, 65536, -23994, -63993, 63992, 25052, 65536, -25052, -63992, 63992, 26107, 65536, -26107, -63992, 63992, 27161, 65536, -27161, -63992, 63993, 28214, 65536, -28214, -63993, 63992, 29266, 65536, -29266, -63992, 63990, 30313, 65536, -30313, -63990, 63989, 31358, 65536, -31358, -63989, 63989, 32401, 65536, -32401, -63989, 63989, 33443, 65536, -33443, -63989, 63990, 34482, 65536, -34482, -63990, 63989, 35519, 65536, -35519, -63989, 63989, 36553, 65536, -36553, -63989, 63989, 37584, 65536, -37584, -63989, 63990, 38613, 65536, -38613, -63990, 63991, 39640, 65536, -39640, -63991, 63992, 40665, 65536, -40665, -63992, 63992, 41687, 65536, -41687, -63992, 63991, 42705, 65536, -42705, -63991, 63991, 43720, 65536, -43720, -63991, 63991, 44733, 65536, -44733, -63991, 63992, 45743, 65536, -45743, -63992, 63991, 46750, 65536, -46750, -63991, 63990, 47752, 65536, -47752, -63990, 63989, 48750, 65536, -48750, -63989, 63989, 49745, 65536, -49745, -63989, 63989, 50738, 65536, -50738, -63989, 63989, 51727, 65536, -51727, -63989, 63989, 52713, 65536, -52713, -63989, 63989, 53694, 65536, -53694, -63989, 63989, 54671, 65536, -54671, -63989, 63989, 55645, 65536, -55645, -63989, 63991, 56617, 65536, -56617, -63991, 63992, 57585, 65536, -57585, -63992, 63991, 58548, 65536, -58548, -63991, 63991, 59507, 65536, -59507, -63991, 63991, 60461, 65536, -60461, -63991, 63991, 61411, 65536, -61411, -63991, 63992, 62359, 65536, -62359, -63992, 63991, 63301, 65536, -63301, -63991, 63989, 64238, 65536, -64238, -63989, 63989, 65169, 65536, -65169, -63989, 63989, 66097, 65536, -66097, -63989, 63989, 67021, 65536, -67021, -63989, 63989, 67940, 65536, -67940, -63989, 63989, 68855, 65536, -68855, -63989, 63988, 69764, 65536, -69764, -63988, 63988, 70668, 65536, -70668, -63988, 63989, 71567, 65536, -71567, -63989, 63990, 72463, 65536, -72463, -63990, 63991, 73355, 65536, -73355, -63991, 63990, 74240, 65536, -74240, -63990, 63989, 75119, 65536, -75119, -63989, 63988, 75993, 65536, -75993, -63988, 63989, 76862, 65536, -76862, -63989, 63989, 77726, 65536, -77726, -63989, 63990, 78585, 65536, -78585, -63990, 63990, 79438, 65536, -79438, -63990, 63989, 80286, 65536, -80286, -63989, 63989, 81127, 65536, -81127, -63989, 63990, 81964, 65536, -81964, -63990, 63992, 82796, 65536, -82796, -63992, 63993, 83623, 65536, -83623, -63993, 63993, 84443, 65536, -84443, -63993, 63992, 85256, 65536, -85256, -63992, 63992, 86064, 65536, -86064, -63992, 63993, 86866, 65536, -86866, -63993, 63993, 87663, 65536, -87663, -63993, 63993, 88453, 65536, -88453, -63993, 63991, 89236, 65536, -89236, -63991, 63990, 90012, 65536, -90012, -63990, 63990, 90783, 65536, -90783, -63990, 63991, 91548, 65536, -91548, -63991, 63991, 92307, 65536, -92307, -63991, 63991, 93059, 65536, -93059, -63991, 63991, 93804, 65536, -93804, -63991, 63991, 94543, 65536, -94543, -63991, 63992, 95276, 65536, -95276, -63992, 63993, 96003, 65536, -96003, -63993, 63994, 96724, 65536, -96724, -63994, 63994, 97438, 65536, -97438, -63994, 63993, 98143, 65536, -98143, -63993, 63993, 98843, 65536, -98843, -63993, 63994, 99536, 65536, -99536, -63994, 63994, 100223, 65536, -100223, -63994, 63994, 100901, 65536, -100901, -63994, 63992, 101572, 65536, -101572, -63992, 63991, 102236, 65536, -102236, -63991, 63992, 102893, 65536, -102893, -63992, 63992, 103543, 65536, -103543, -63992, 63992, 104187, 65536, -104187, -63992, 63992, 104823, 65536, -104823, -63992, 63992, 105452, 65536, -105452, -63992, 63992, 106073, 65536, -106073, -63992, 63993, 106687, 65536, -106687, -63993, 63994, 107295, 65536, -107295, -63994, 63995, 107896, 65536, -107896, -63995, 63995, 108489, 65536, -108489, -63995, 63994, 109073, 65536, -109073, -63994, 63994, 109650, 65536, -109650, -63994, 63995, 110220, 65536, -110220, -63995, 63995, 110783, 65536, -110783, -63995, 63995, 111337, 65536, -111337, -63995, 63993, 111882, 65536, -111882, -63993, 63992, 112419, 65536, -112419, -63992, 63992, 112950, 65536, -112950, -63992, 63993, 113473, 65536, -113473, -63993, 63993, 113989, 65536, -113989, -63993, 63993, 114496, 65536, -114496, -63993, 63992, 114995, 65536, -114995, -63992, 63992, 115486, 65536, -115486, -63992, 63993, 115970, 65536, -115970, -63993, 63995, 116447, 65536, -116447, -63995, 63996, 116916, 65536, -116916, -63996, 63995, 117376, 65536, -117376, -63995, 63995, 117826, 65536, -117826, -63995, 63995, 118269, 65536, -118269, -63995, 63995, 118705, 65536, -118705, -63995, 63996, 119133, 65536, -119133, -63996, 63995, 119551, 65536, -119551, -63995, 63994, 119960, 65536, -119960, -63994, 63993, 120361, 65536, -120361, -63993, 63993, 120754, 65536, -120754, -63993, 63993, 121139, 65536, -121139, -63993, 63994, 121517, 65536, -121517, -63994, 63993, 121885, 65536, -121885, -63993, 63993, 122245, 65536, -122245, -63993, 63993, 122596, 65536, -122596, -63993, 63994, 122940, 65536, -122940, -63994, 63995, 123276, 65536, -123276, -63995, 63996, 123603, 65536, -123603, -63996, 63996, 123921, 65536, -123921, -63996, 63995, 124229, 65536, -124229, -63995, 63995, 124529, 65536, -124529, -63995, 63996, 124822, 65536, -124822, -63996, 63996, 125106, 65536, -125106, -63996, 63995, 125380, 65536, -125380, -63995, 63994, 125644, 65536, -125644, -63994, 63993, 125900, 65536, -125900, -63993, 63993, 126149, 65536, -126149, -63993, 63994, 126389, 65536, -126389, -63994, 63994, 126620, 65536, -126620, -63994, 63994, 126842, 65536, -126842, -63994, 63993, 127055, 65536, -127055, -63993, 63993, 127260, 65536, -127260, -63993, 63994, 127456, 65536, -127456, -63994, 63996, 127645, 65536, -127645, -63996, 63996, 127824, 65536, -127824, -63996, 63996, 127994, 65536, -127994, -63996, 63995, 128153, 65536, -128153, -63995, 63995, 128305, 65536, -128305, -63995, 63996, 128449, 65536, -128449, -63996, 63996, 128583, 65536, -128583, -63996, 63996, 128708, 65536, -128708, -63996, 63994, 128822, 65536, -128822, -63994, 63993, 128929, 65536, -128929, -63993, 63993, 129027, 65536, -129027, -63993, 63994, 129117, 65536, -129117, -63994, 63994, 129198, 65536, -129198, -63994, 63994, 129269, 65536, -129269, -63994, 63993, 129331, 65536, -129331, -63993, 63994, 129385, 65536, -129385, -63994, 63994, 129430, 65536, -129430, -63994, 63994, 129467, 65536, -129467, -63994, 63993, 129492, 65536, -129492, -63993, 63990, 129507, 65536, -129507, -63990, 63987, 129512, 65536, -129512, -63987 };
                    // int[] dataArray = { 65535, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    //同一个参数可以发向不同的FPGA，如插值系数，与通道无关
                    HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, 0);
                    HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, 2);
                    for (int dataIndex = 0; dataIndex < dataCount_pro_phs3; dataIndex++)
                    {
                        //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                        UInt32 data = (UInt32)dataArray_pro_phs3[dataIndex];
                        HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWen, 0);
                        HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWa, (UInt32)dataIndex);
                        HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWdLow, data & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWdHigh, (data >> 16) & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWen, 1);
                    }
                    HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWen, 0);
                    //需要做打开、关闭控制
                    HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, 0);


                    //multi
                    int dataCount_pro_multi = 3000;
                    int[] dataArray_pro_multi = { 65535, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    // int[] dataArray = { 65535, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    //同一个参数可以发向不同的FPGA，如插值系数，与通道无关
                    Int32[]? coefficientsDataL1 = Misc.ReadCaliCoefDataFronmFile(".\\CaliData\\CoeFiles\\MSO8000_Interpolation_1st_max10.txt");

                    Int32[]? coefficientsDataL2 = Misc.ReadCaliCoefDataFronmFile(".\\CaliData\\CoeFiles\\MSO8000_Interpolation_2nd_max10.txt");


                    //Int32[]? coefficientsDataL1 = Misc.ReadCaliCoefDataFronmFile(".\\CaliData\\CoeFiles\\MSO8000_Interpolation_1st_max10.txt");

                    //Int32[]? coefficientsDataL2 = Misc.ReadCaliCoefDataFronmFile(".\\CaliData\\CoeFiles\\MSO8000_Interpolation_2nd_max10.txt");

                    List<Int32> coefficientsDataTable = new List<Int32>();
                    coefficientsDataTable.AddRange(coefficientsDataL1);
                    coefficientsDataTable.AddRange(coefficientsDataL2);
                    Int32[]? coefficientsData = coefficientsDataTable.ToArray();

                    //for (int j = 0; j < coefficientsData.Length; j++)
                    //{
                    //    UInt32 Coeaddr = 0;
                    //    if (j < coefficientsDataL1.Length)
                    //        Coeaddr = (UInt32)j;
                    //    else
                    //        Coeaddr = (UInt32)(j | 0x8000);

                    //    UInt32 CoeData = (UInt32)coefficientsData[j];

                    //    //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];

                    //    HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWen, 0);
                    //    HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve11, (UInt32)Coeaddr);
                    //    HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWdLow, CoeData & 0xffff);
                    //    HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWdHigh, (CoeData >> 16) & 0xffff);
                    //    HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWen, 1);
                    //}
                    //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWen, 0);

                }
                //----HTF_END

                //HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, 0x13);
                //HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_ConfigData,  0x8u << 22);
                //HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_TransStart, 1);
                //HdIO.WaitForSpiTransfer(1, 4);
                //HdIO.Sleep(2);//什么道理，要10ms
                //HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_TransStart, 0);
                //HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, (UInt16)0);

                //HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, 0x13);
                //HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_ConfigData,  0x8u << 21);
                //HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_TransStart, 1);
                //HdIO.WaitForSpiTransfer(1, 4);
                //HdIO.Sleep(2);//什么道理，要10ms
                //HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_TransStart, 0);
                //HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, (UInt16)0);



                //HdIO.WriteReg(ProcBdReg.W.Exttrig_ExttrigSyncIoDelayEnVtc, 0x01);
                //HdIO.WriteReg(ProcBdReg.W.Exttrig_ExttrigSyncIoDelayNum, 0);
                //HdIO.WriteReg(ProcBdReg.W.Exttrig_ExttrigSyncIoDelayEnVtc, 0x00);
                //HdIO.WriteReg(ProcBdReg.W.Exttrig_ExttrigSyncIoDelayLoad, 0x00);
                //HdIO.WriteReg(ProcBdReg.W.Exttrig_ExttrigSyncIoDelayLoad, 0x01);
                //HdIO.WriteReg(ProcBdReg.W.Exttrig_ExttrigSyncIoDelayLoad, 0x00);
                //HdIO.WriteReg(ProcBdReg.W.Exttrig_ExttrigSyncIoDelayEnVtc, 0x01);
                //UInt16 trigext_delayvalue = (ushort)HdIO.ReadReg(ProcBdReg.R.Exttrig_ExttrigSyncIoDelayValueRead);
                //HdIO.WriteReg(ProcBdReg.W.Exttrig_SoftRst, 0x0);
                //Thread.Sleep(1);
                //HdIO.WriteReg(ProcBdReg.W.Exttrig_SoftRst, 0x3);
                //Thread.Sleep(1);
                //HdIO.WriteReg(ProcBdReg.W.Exttrig_SoftRst, 0x0);


                //HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve13, 0x01);     //reg_sync_fifo_data_mode
                HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve14, 0x8192);    //reg_sync_fifo_deepl
                HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve25, 1);
                HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve22, 5000);  //auto trig max cnt
                HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve19, 0x0000);
                HdIO.WriteReg(ProcBdReg.W.Decoder_B2SignalSourceL, 0x0001);//gehaipeng,decamation
                HdIO.WriteReg(ProcBdReg.W.Decoder_B2SignalSourceM, 0x0001);
                HdIO.WriteReg(ProcBdReg.W.Decoder_B2SignalSourceH, 0x0000);
				//UInt16 trigext_delayvalue = (ushort)HdIO.ReadReg(ProcBdReg.R.Exttrig_ExttrigSyncIoDelayValueRead);
                HdIO.WriteReg(ProcBdReg.W.Exttrig_DelayEn, 0x1);
                //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_3, 0x01);//Close FD

                StartingGunPang(bForce);
            }
        }
        internal static void CreateAcquireAttribute()
        {   
            foreach (AbstractAcquirer? acquirer in allAcquirer)
            {
                acquirer?.CreateAcquireAttribute();
            }
        }

        internal static DateTime lastSimulateScanTime = DateTime.Now;
        private static List<ReadInfo> lastReadInfos = new List<ReadInfo>();
        private static bool CheckNeedReread(Boolean bForceReadData, Boolean bNeedReset, List<ReadInfo> readInfos)
        {
            if ((!Hd.UIMessage!.bAcquireStopped) || bForceReadData || bNeedReset || (AcqReset_TimeStamp > LastReaded_TimeStamp) || (lastReadInfos.Count != readInfos.Count))
            {
                return true;
            }

            for (int i = 0; i < readInfos.Count; i++)
            {
                if (!lastReadInfos[i].Equals(readInfos[i]))
                {
                    return true;
                }
            }
            return false;
        }
        
        
        /// <summary>
        /// 查询采集数据是否满
        /// </summary>
        /// <param name="bForceReadData"></param>
        /// <param name="bNeedReset"></param>
        /// <returns>true:采集器满;false:采集器不满;</returns>
        internal static Boolean AcquireQuery(Boolean bForceReadData, Boolean bNeedReset)
        {
            Boolean isacqok = false;

            /*判断是模拟数据还是真实数据*/
            if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen || Hd.BPowerOff)
            {
                /*模拟数据*/
                if ((Hd.UIMessage?.Timebase?.IsScan ?? false))
                {
                    //模拟Scan
                    Double xdiv = Hd.UIMessage!.Timebase.TmbScale / 1000;
                    if ((DateTime.Now - lastSimulateScanTime).TotalMilliseconds < xdiv)
                    {
                        Hd.bAcqedNewData = false;
                        isacqok = false;
                    }
                    else
                    {
                        lastSimulateScanTime = DateTime.Now;
                        Hd.bAcqedNewData = true;
                        isacqok = true;
                    }
                }
                else
                {
                    isacqok = true;
                }
            }
            else
            {
                /*真实数据*/

                if (bNeedReset/* && !Hd.UIMessage!.Display!.IsFast*/)
                {
                    foreach (var acquirer in AcqDataTypeAcquirer)
                    {
                        acquirer.Value.Reset();
                    }
                    InitAcq(true);
                    return false;
                }

                bReadOldData = (bForceReadData && !bNeedReset) || Hd.UIMessage!.bAcquireStopped;

                /*DSO/UPO*/
                if(!Hd.UIMessage!.Display!.IsFast)
                {
                    /*DSO*/
                    if (bReadOldData)
                        isacqok = AcqDataTypeAcquirer[0].AcqFulled;
                    else
                    {
                        isacqok = AbstractController_Misc.AcqIsFulled();
                        if (isacqok)//????
                            Acquisition.CurrDataAcquireAttribute.CloneTo(Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquedParameters ?? new AcquireAttribute());
                    }
                        
           //         isacqok = true;

                    //读当前帧号，更新CurrDataAcquireAttribute
                    if (isacqok)
                    {
                        if (!Hd.UIMessage!.Timebase!.IsScan || Hd.UIMessage.bAcquireStopped)
                        {
                            //DDR数据,应该拉软暂停
                            HdIO.WriteReg(ProcBdReg.W.DataPath_SoftStopPro, 1U);
                            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_SoftStopAcq, 1U);
                        }

                        UInt16 frameno = (ushort)HdIO.ReadReg(ProcBdReg.R.LSCtrl_LockedFrameNoPro);
                        //Debug.WriteLine($"LSCtrl_LockedFrameNoPro:{frameNo}");
                        var acqAttr = Hd.CurrProduct.Acquirer_AnalogChannel!.FrameNo_AcquireAttribute_Get(frameno);
                        if (acqAttr != null)
                            CurrDataAcquireAttribute = acqAttr;//????

                        AcqedDataMsg = CurrDataAcquireAttribute.HdMessage!;
                    }
                }
                else
                {
                    /*UPO*/
                    if (bReadOldData)
                        isacqok = true;
                    else
                        isacqok = AbstractController_Misc.AcqIsFulled();
                    //读当前帧号，更新CurrDataAcquireAttribute
                    if (isacqok)
                    {
                        //pull up softStop
                        if (bReadOldData)
                        {
                            HdIO.WriteReg(ProcBdReg.W.DataPath_SoftStopPro, 1U);
                            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_SoftStopAcq, 1U);
                        }

                        UInt16 frameno = (ushort)HdIO.ReadReg(ProcBdReg.R.LSCtrl_LockedFrameNoPro);
                        if (CurrDataAcquireAttribute.FrameNo != frameno)
                        {
                            var acqattr = Hd.CurrProduct.Acquirer_AnalogChannel!.FrameNo_AcquireAttribute_Get(frameno);
                            if (acqattr != null)
                                CurrDataAcquireAttribute = acqattr;

                            //UPO need resend the params
                            if (Hd.UIMessage!.Display!.IsFast && bReadOldData)
                            {
                                Hd.CurrProduct?.Acquirer_DPX?.InitGenerateParams(true);
                                IsUPODataSync = false;
                            }
                        }

                        AcqedDataMsg = CurrDataAcquireAttribute.HdMessage!;
                    }
                }
                Hd.CurrProduct?.Acquirer_AnalogChannel?.ExistsReadParamsEnsureCapacity(bNeedReset);
            }
            return isacqok;
        }

        internal static Boolean IsReadUPOAtStop = false;
        internal static Boolean IsUPODataSync = true;
        internal static void Acquire(List<ReadInfo> readInfos, ref Dictionary<AcqDataType, Double> HardwareSampeInterval, CancellationToken? softResetToken)
        {
            SystemMonitor.Default.SystemTemperatureProcess();

            HardwareSampeInterval.Clear();
            var datatypegroup = readInfos.GroupBy(m => m.DataType);
            foreach (var v in datatypegroup)
            {
                if (AcqDataTypeAcquirer.ContainsKey(v.Key))
                {
                    AcqDataTypeAcquirer[v.Key].ReadAcqData(v.ToList(), out Double samplingratebyus, softResetToken);

                    AcqDataTypeAcquirer[v.Key].AcqFulled = true;
                    HardwareSampeInterval.TryAdd(v.Key, samplingratebyus);
                    AcqDataTypeAcquirer[v.Key].PostProcess(v.ToList(), softResetToken);
                }
                EmdProcess.Default.UpdateAbnormalData();
            }
        }
        internal static Int32 ScanRunningNewDataPerChannelExistsDotCount = 0;
        internal static Int64 ScanPerChannelInDdrTotalDotCount_AlreadyDisplay = 0;
        internal static Int64 ScanPerChannelInDdrDotCount_NotDisplay = 0;

        internal static void AcquireReset()
        {
            //pull down softStop
            if (!Hd.UIMessage.bAcquireStopped)
            {
                Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_SoftStopAcq, 0U);
                HdIO.WriteReg(ProcBdReg.W.DataPath_SoftStopPro, 0U);
            }

            if (Hd.UIMessage!.Display!.IsFast && !IsUPODataSync)
            {
                //UPO param restore
                CurrDataAcquireAttribute = new AcquireAttribute();
                Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.CloneTo(CurrDataAcquireAttribute);
                Hd.CurrProduct?.Acquirer_DPX?.InitGenerateParams(false);
            }

            // default force reset, some condition not force reset
            Boolean isforceacqreset = true;
            if(Hd.UIMessage?.Timebase?.IsScan ?? true)
            {
                isforceacqreset = false;
            }
            else if(Hd.UIMessage!.Trigger!.Mode == TriggerMode.OneShot)
            {
                isforceacqreset = false;
            }
            else if(Hd.UIMessage.bAcquireStopped)
            {
                isforceacqreset = false;
            }
            else if (Hd.UIMessage!.Display!.IsFast && IsUPODataSync)
            {
                isforceacqreset = false;
            }
            IsUPODataSync = true;

            InitAcq(isforceacqreset);
        }

    }
}
