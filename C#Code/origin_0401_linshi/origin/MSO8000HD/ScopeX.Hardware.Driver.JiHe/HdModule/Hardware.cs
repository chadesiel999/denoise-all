//https://www.cnblogs.com/tianjiuyi/p/11095964.html
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Driver.Module;
using ScopeX.Hardware.Driver.Registers.SendManage;
using ScopeX.Updater.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{

    /// <summary>
    /// 硬件工作模式：目前分作“更新模式”和“非更新模式(正常模式)”
    /// 
    /// 便于板卡初始化时做区别，避免更新模式进行不必要的校准灯操作
    /// </summary>
    public enum HardwareWorkMode
    {
        HdWorkOnUpdater,
        HdWorkOnNormal,
    }

    /// <summary>
    /// 1、开机调用顺序为：Open，Initialize，AutoCaliPowerOn。程序退出时，要调用Close.
    /// 2、硬件响应及要数，必须在同一个线程中。
    /// 3、本DLL是基于 非独立PCIE模式。整个工程FPGA分为Acquire FPGA,Process FPGA[K7],PCIE[K7] FPGA 。共3个FPGA工程块。
    /// 4、采集的流程为
    ///     A、Execute（如果有硬件改变，将自动执行初始化Fifo,否则不执行。为慢时基、单次、正常触发而设计)；
    ///     B、AcqWave（返回是否读到数据，Fifo满，将数据读到软件端。长存储的数据采用乒乓方式）；
    ///     C、特殊的数据读取（主要是长存储下的）
    /// 5、本DLL适用于XXXX 型号工程。
    /// </summary>
    public static partial class Hd
    {
        static Hd()
        {
            CtrlAnalogChannel_JiHe2d5G.baseObj1.HighVoltageWarningEvent += BaseObj1_HighVoltageWarningEvent;
        }

        private static void BaseObj1_HighVoltageWarningEvent(Int32 obj)
        {
            HighVoltageWarningEvent?.Invoke(obj);
        }
        #region FlashUpdater

        private static IFpgaFlashUpdater? _FlashUpdater;
        internal static IFpgaFlashUpdater? FlashUpdater
        {
            get
            {
                if (_FlashUpdater == null)
                {
                    _FlashUpdater = FpgaFlashUpdaterFactory.FindUpdater(CurrProductType, new List<Int32>() { Constants.BOAED_INDEX });
                    _FlashUpdater!.BindLogFunc(FlashUpdateLog);
                }
                return _FlashUpdater;
            }
        }
        internal static void FlashUpdateLog(String msg)
        {
            Hd.SysLogger?.Invoke($"Driver-DMA:{msg}!", "Info");
        }

        public static Boolean WriteUSBTMCSN(String sn)
        {
            if (FlashUpdater == null)
            {
                return false;
            }
            return FlashUpdater.WriteUSBTMCSN(sn);
        }

        internal static Boolean WritingCaliDataToFlash(CaliDataType type, Byte[] buffer)
        {
            var flash = FpgaFlashUpdaterFactory.FindUpdater(CurrProductType, new List<Int32>() { Constants.PCIE_BOARD_INDEX });
            if (flash == null)
            {
                return false;
            }
            return type switch
            {
                CaliDataType.AWG => flash.AwgCaliDataWrite(buffer),
                CaliDataType.Misc => flash.MiscCaliDataWrite(buffer),
                CaliDataType.CoefficientsParams => flash.DspCaliDataWrite(buffer),
                CaliDataType.AnalogParams => flash.ChannelCaliDataWrite(buffer),
                //CaliDataType.TiadcPhaseOffsetGainParams => flash.TiadcCaliDataWrite(buffer),
                _ => false,
            };
        }

        internal static Boolean ReadingCaliDataFromFlash(CaliDataType type, out Byte[] buffer)
        {
            buffer = new Byte[0];
            var flash = FpgaFlashUpdaterFactory.FindUpdater(CurrProductType, new List<Int32>() { Constants.PCIE_BOARD_INDEX });
            if (flash == null)
            {
                return false;
            }

            return type switch
            {
                CaliDataType.AWG => flash.AwgCaliDataRead(out buffer),
                CaliDataType.Misc => flash.MiscCaliDataRead(out buffer),
                CaliDataType.CoefficientsParams => flash.DspCaliDataRead(out buffer),
                CaliDataType.AnalogParams => flash.ChannelCaliDataRead(out buffer),
                //CaliDataType.TiadcPhaseOffsetGainParams => flash.TiadcCaliDataRead(out buffer),
                _ => false,
            };
        }

        /// <summary>
        /// 需要获取Flash数据时 请将此标志置为True
        /// </summary>
        public static volatile Boolean ReadlashCaliDataFlag = false;

        /// <summary>
        /// 需要重新写入Flash数据时 请将此标志置为True
        /// </summary>
        public static volatile Boolean UpdateFlashCaliDataFlag = false;

        /// <summary>
        /// 供Core.Dispatcher调用
        /// </summary>
        /// <param name="type">校准数据类型</param>
        public static void UpdateFlashCaliData(UInt32 type)
        {
            //if (Enum.IsDefined(typeof(CaliDataType), (int)type))
            //{
            //    CaliDataType calidatatype = (CaliDataType)type;
            //    if (ReadlashCaliDataFlag)
            //    {
            //    var buffer = new byte[0];
            //    try
            //    {
            //        var res = ReadingCaliDataFromFlash(calidatatype, out buffer);
            //            res &= CalibrationStore.Default.SetCaliData(calidatatype, buffer);
            //            if (!res)
            //            {
            //                var calidata = CalibrationStore.Default.GetCaliData(calidatatype, true);
            //                res = WritingCaliDataToFlash(calidatatype, calidata);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //        if (buffer.Length > 0)
            //        {
            //            var jsonstring = Encoding.Default.GetString(buffer);
            //            Hd.SysLogger?.Invoke($"Read data error: {jsonstring}  ", "Info");
            //        }

            //        Hd.SysLogger?.Invoke($"Read {calidatatype} data error {ex.Message}", "Info");
            //    }
            //    finally
            //    {
            //            ReadlashCaliDataFlag = false;
            //    }
            //    return;
            //    }

            //    if (UpdateFlashCaliDataFlag)
            //    {
            //        try
            //        {
            //            var calidata = CalibrationStore.Default.GetCaliData(calidatatype);
            //            var res = WritingCaliDataToFlash(calidatatype, calidata);
            //            if (!res)
            //            {
            //                Helper.GetICaliData(calidatatype)?.SaveToFile();//后续可能取消这个步骤
            //            }
            //            else
            //            {
            //                //如果是模拟通道校准数据保存成功 自校正数据需要重新Copy一份并保存
            //                if (calidatatype == CaliDataType.PhyChannel || calidatatype == CaliDataType.AnalogParams)
            //                {
            //                    AutoCaliParams.Default?.SaveToFile();
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            Hd.SysLogger?.Invoke($"Update {calidatatype} data error {ex.Message}", "Error");
            //        }
            //        finally
            //        {
            //            UpdateFlashCaliDataFlag = false;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 初始化
        /// Dispather
        /// </summary>
        public static void UpdateDsoInfo(DsoInfos? current)
        {
            //MSO80000HD 选件暂缓
            if (current == null)
            {
                return;
            }
            //current.OptionsInfos.SyncOperation = FalshOpreation.None;
            //current.OptionsInfos.SyncResult = FalshOpreationResult.Fail;
            //return;

            var productinfo = new ProductInfo();
            var optionsinfo = new OptionsInfoBase();
            var bok = false;
            //读写产品信息
            if (current.ProductInfos.SyncOperation == FalshOpreation.Read)//读
            {
                bok = FlashUpdater?.ReadProductInfo(out productinfo) ?? false;
                if (bok)
                {
                    current.ProductInfos.SerialNumber = productinfo.SerialNumber;
                    current.ProductInfos.ChannelInfo = productinfo.ChannelInfo;
                    current.ProductInfos.ProductModel = productinfo.ProductModel;
                    current.ProductInfos.ProtocolVersion = productinfo.ProtocolVersion;
                    current.ProductInfos.HardVersion = productinfo.HardVersion;
                    current.ProductInfos.OtherInfo = productinfo.OtherInfo;
                    current.ProductInfos.ProductionDate = productinfo.ProductionDate;
                }
            }
            else if (current.ProductInfos.SyncOperation == FalshOpreation.Write)//写读
            {
                productinfo.ProductionDate = current.ProductInfos.ProductionDate;
                productinfo.ProductModel = current.ProductInfos.ProductModel;
                productinfo.ChannelInfo = current.ProductInfos.ChannelInfo;
                productinfo.OtherInfo = current.ProductInfos.OtherInfo;
                productinfo.ProtocolVersion = current.ProductInfos.ProtocolVersion;
                productinfo.HardVersion = current.ProductInfos.HardVersion;
                productinfo.SerialNumber = current.ProductInfos.SerialNumber;

                bok = FlashUpdater?.WriteProductInfo(productinfo) ?? false;
            }

            current.ProductInfos.SyncResult = bok ? FalshOpreationResult.Success : FalshOpreationResult.Fail;
            current.ProductInfos.SyncOperation = FalshOpreation.None;

            bok = false;
            //读写选件信息
            if (current.OptionsInfos.SyncOperation == FalshOpreation.Read)
            {
                bok = FlashUpdater?.ReadOptionsInfo(out optionsinfo) ?? false;
                if (bok)
                {
                    if (optionsinfo.AllOptions != null && current.OptionsInfos.AllOptions != null)
                    {
                        foreach (var item in optionsinfo.AllOptions)
                        {
                            if (current.OptionsInfos.AllOptions.ContainsKey(item.Key))
                            {
                                current.OptionsInfos.AllOptions[item.Key] = item.Value;
                            }
                        }
                    }
                    current.OptionsInfos.AllOptions = optionsinfo.AllOptions;
                    current.OptionsInfos.TrialRemainingTimeByHour = optionsinfo.TrialRemainingTimeByHour;
                }
            }
            else if (current.OptionsInfos.SyncOperation == FalshOpreation.Write)
            {
                optionsinfo.AllOptions = current.OptionsInfos.AllOptions;
                optionsinfo.TrialRemainingTimeByHour = current.OptionsInfos.TrialRemainingTimeByHour;

                bok = FlashUpdater?.WriteOptionsInfo(optionsinfo) ?? false;
            }

            current.OptionsInfos.SyncOperation = FalshOpreation.None;
            current.OptionsInfos.SyncResult = bok && current.OptionsInfos.AllOptions != null ? FalshOpreationResult.Success : FalshOpreationResult.Fail;
        }

        #endregion

        public static event Action<Int32> HighVoltageWarningEvent;

#pragma warning disable CS8618
        internal static OscilloscopeProduct CurrProduct;
        internal static HdMessage? OldMessage = null;
        internal static ProductType CurrProductType = ProductType.Base;
        internal static HardwareWorkMode HardwareWorkMode = HardwareWorkMode.HdWorkOnNormal;//默认正常模式

        /// <summary>
        /// 加载硬件定义
        /// </summary>
        /// <param name="productType"></param>
        /// <param name="productComponents"></param>
        public static void Load(ProductType productType = ProductType.Base, String productComponents = "")
        {
            CurrProductType = productType;
            if (CurrProduct == null)
            {
                var pcomponents = productComponents.Split(',');
                //todo: 添加相应组件

                CurrProduct = ProductFactory.CreateProduct(CurrProductType);

                CurrProduct.InitMatlabDlls();
            }
        }

        /// <summary>
        /// 打开硬件
        /// </summary>
        /// <param name="deviceInfo">PCIE通信没有用到，为Usb等通信预留。目前没有使用，可填任意值</param>
        /// <param name="attachHardware">使用真实硬件还是模拟硬件。模拟硬件只能产生模拟波形，其他的读入数皆为0.缺省为true</param>
        /// <param name="isPrintDebugInformation">目前没有使用，缺省为false</param>
        /// <returns></returns>
        public static Boolean Open(String deviceInfo, Boolean attachHardware = true, Boolean isPrintDebugInformation = false)
        {
            Hd.ConstructorCommandTable();
            bPrintDebugInformation = isPrintDebugInformation;
            if (HdIO.CurrDriver != null)
            {
                HdIO.CurrDriver.Close();
            }
            bAttachHardware = attachHardware;

            HdIO.CurrDriver = DriverFactory(DriverTypes.DCCardPcie, deviceInfo);
            if (bAttachHardware)
            {
                if (HdIO.CurrDriver == null)
                {
                    ComModel.ErrorCode.ErrorType = ErrorType.S_Driver_Is_Null_0003;
                    return false;
                }
                if (HdIO.CurrDriver.Open(deviceInfo))
                {
                    HdIO.QueryPerWriteRegUsedNs();
                    // 空读
                    Byte[] datatmp = new Byte[16 * 1024 * 1024 * 2];// 快传一次最大16M采样点，每个采样点2byte
                    Hd.CurrProduct?.AcqBd?.SwitchDataPathAndPcieReset_SetDataLength(DMAReadDataTypes.AnalogChannelDdr, (UInt32)datatmp.Length);
                    HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                    HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);
                    HdIO.DMARead((UInt32)datatmp.Length, ref datatmp);
                    HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                    return true;
                }
            }
            return false;
        }

        //保存命令中需要DMA访问的操作
        private static List<Action> CmdDelayedDmaTasks = new List<Action>();

        //收集命令中需要DMA访问的操作
        public static void PushCmdDelayedDmaTask(Action action)
        {
            CmdDelayedDmaTasks.Add(action);
        }

        //执行命令中需要DMA访问的操作
        public static void ExecuteCmdDelayedDmaTask()
        {
            CmdDelayedDmaTasks.ForEach(action =>
            {
                action?.Invoke();
            });
            CmdDelayedDmaTasks.Clear();
        }

        public static void PowerDown()
        {
            Hd.CurrProduct?.PcieBd?.PowerDown();
            HdIO.WriteReg(PcieBdReg.W.PowerManager_ExtTrig_Power, 0x0);
        }
        /// <summary>
        /// 退出程序前，必须调用。
        /// </summary>
        public static void Close()
        {
            if (HdIO.CurrDriver != null)
                HdIO.CurrDriver.Close();
            //if (AWG.CurrUsb30Driver != null)
            //{
            //    AWG.CurrUsb30Driver.Close();
            //}
            HdIO.SaveLogToDisk?.Invoke();

            HdIO.CurrDriver = null;
        }
        /// <summary>
        /// (String message, String errorLevel)
        /// </summary>
        internal static Action<String, String>? SysLogger;

        /// <summary>
        /// 错误信息框委托
        /// </summary>
        internal static Action<Int32>? ErrorMsgbox;

        /// <summary>
        /// 硬件系统基本初始化。执行此操作后，硬件处于缺省的正确的工作状态。系统上电时执行一次
        /// </summary>
        /// <returns></returns>
        public static Boolean Initialize(HdMessage? hdMessage, Action<String, String>? sysLogger = null, Action<Int32>? errorMsgbox = null)
        {
            SysLogger = sysLogger;
            ErrorMsgbox = errorMsgbox;

            if (Hd.BPowerOff)
            {
                Hd.CurrProduct!.PcieBd!.PowerDown();
                ComModel.ErrorCode.ErrorType = ErrorType.S_PowerOff_0006;
                return false;
            }
            UIMessage = hdMessage;
            Acquisition.AcqedDataMsg = hdMessage! with { };

            HdIO.Init?.Invoke();
            CaliDataManager.LoadAllFromFile();

            CurrProduct?.AcqBd?.ClearSendHistory();
            CurrProduct?.Acquirer_AnalogChannel?.CreateAcquireAttribute();//为后续的配置打下基础

            if (bAttachHardware)//在纯软件调试时，调用下面的函数需要近30秒的时间，不可忍受。但实际带硬件时，其启动又是支持的。
                DoAllBoardInit();
            Hd.CurrDebugVarints.InitDebugVariants();
            #region 发送所有校准数据
            //由于在初始化采集板时，自动使用了有关IDelay、ADC同步窗口串和ADC的Phase_Gain，故这些不需要重新初始化，特别是不能初始化ADCPhase_Gain
            CaliDataManager.DataChangedCaliDataType.AddRange(new CaliDataType[] { CaliDataType.PhyChannel, CaliDataType.CoefficientsTables, CaliDataType.TiadcPhaseOffsetGainParams, CaliDataType.TiAdc_SyncSampleClock });
            foreach (var coefficientstabletype in Hd.CurrProduct!.HardwareConfig!.LocalCoefficientsTableMeanings.Keys)
                CaliDataManager.DataChangedCoefficientsTableType.Add(coefficientstabletype);
            Hd.CurrProduct?.AcqBd?.SendCoefficientsByRegisterMode_ADCTI(true);//?????
            Hd.CurrProduct?.AcqBd?.SendCoefficientsByRegisterMode_IFC(true);//?????
            HdIO.Sleep(100);

            #region channel sync 
            int[]? dataArray = Misc.ReadCaliCoefDataFronmFile($@"{AppDomain.CurrentDomain.BaseDirectory}CaliData\CoeFiles\coe_farrow.txt");

            if (dataArray != null)
            { 
                int dataCount = dataArray.Length;

                for (uint i = 0; i < dataCount; i++)
                {
                    uint data = (uint)dataArray[i];
                    HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, 1);//farrow enable      // reg_farrow_filter_en,reg_int_delay_en             
                    HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWen, 0);//coef write enable    // reg_farrow_factor_wen           
                    //HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWa, i);//coef address         //reg_farrow_factor_wa                         
                    HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWa, ((uint)(i / 108) << 7) + i % 108);//coef address         //reg_farrow_factor_wa htf 20250806                        
                    HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWdL16, data & 0xffff);//coef data Low16bit       //reg_farrow_factor_wd_l16        
                    HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWdH3, (data >> 16) & 0x7);//coef data High3bit  //reg_farrow_factor_wd_h3
                    HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWen, 1);//coef write enable    //reg_farrow_factor_wen           
                    HdIO.DelayByUs(10);
                }
                HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWen, 0);//coef write enable    //reg_farrow_factor_wen
            }
            #endregion

            //CaliDataManager.CheckDataChanged();
            //AbstractController_Misc.CaliDataChanged();
            #endregion

            EmdProcess.Default.Init();
            ArtificialIntelligenceProcess.Default.Init();

            AbstractController_Misc.AfterFirstInitAction();
            Execute(hdMessage!);

            //先强制Adc自校准
            Adc5200AutoCaliManager.Default.ExecForceCali();

            //Hd.CurrProduct?.Acquirer_AnalogChannel?.AutoCaliAtInit(Hd.UIMessage);//????


            Thread.Sleep(100);
            HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_2, 0x0);
            HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve10, 0x0);    //reg_debug_single_mode
            HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve11, 0x30);   //reg_syncfifo_epmty
            HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve12, 0x8192); //reg_sycnfifo_full
            HdIO.WriteReg(ProcBdReg.W. reverse_reg_reserve13, 0xf);    //reg_sync_fifo_data_mode
            HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve14, 8192);    //reg_sync_fifo_data_mode
            HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFifoProgFullThresh, 8000); //ti_cross_fifo_full  8192  htf 20250806
            HdIO.WriteReg(ProcBdReg.W.ChannelSync_FifoProgFull, 512); //ti_cross_fifo_full


            Thread.Sleep(100);


            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_0, 0);//lry0508
            HdIO.Sleep(100);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_0, 1);//lry0508
            HdIO.Sleep(100);

            #region GT初始化
            //B7与处理板GT
            HdIO.WriteReg(ProcBdReg.W.GtConfig_GtProSet, 0x0);
            HdIO.WriteReg(ProcBdReg.W.GtConfig_Gt1ProSet, 0x0);
            Thread.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.GtConfig_GtProSet, 0x21);
            HdIO.WriteReg(ProcBdReg.W.GtConfig_Gt1ProSet, 0x003f);
            Thread.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.GtConfig_GtProSet, 0x0);
            HdIO.WriteReg(ProcBdReg.W.GtConfig_Gt1ProSet, 0x0);

            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.GtConfig_Gt, AcqBdNo.B0, 0x00);
            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.GtConfig_Gt, AcqBdNo.B1, 0x00);
            Thread.Sleep(1);
            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.GtConfig_Gt, AcqBdNo.B0, 0x21);
            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.GtConfig_Gt, AcqBdNo.B1, 0x21);
            Thread.Sleep(1);
            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.GtConfig_Gt, AcqBdNo.B0, 0x00);
            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.GtConfig_Gt, AcqBdNo.B1, 0x00);

            //回读状态，等待2S才能读回状态
            Thread.Sleep(2000);
            //单板测试模式
            HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_2, 0x00);


            //回读状态，若状态有误重新复位再次检测
            UInt32 progtstatus = HdIO.ReadReg(ProcBdReg.R.GtConfig_RoGtProStatus);
            UInt32 progt2status = HdIO.ReadReg(ProcBdReg.R.GtConfig_RoGt2ProStatus);
            UInt32 progt3status = HdIO.ReadReg(ProcBdReg.R.GtConfig_RoGt3ProStatus);
            UInt32 progt4status = HdIO.ReadReg(ProcBdReg.R.GtConfig_RoGt4ProStatus);
            UInt32 progt5status = HdIO.ReadReg(ProcBdReg.R.GtConfig_RoGt5ProStatus);
            UInt32 progt6status = HdIO.ReadReg(ProcBdReg.R.GtConfig_RoGt6ProStatus);
            UInt32 progt7status = HdIO.ReadReg(ProcBdReg.R.GtConfig_RoGt7ProStatus);
            UInt32 progt1status = HdIO.ReadReg(ProcBdReg.R.GtConfig_RoGt1ProStatus);

            Thread.Sleep(2000);
            //if ((progtstatus== 0x7fff) && (progt1status == 0x7fff) &&(progt2status==0x7fff)&& (progt3status == 0x7fff) && (progt4status == 0x7fff) && (progt5status == 0x7fff)  && (progt7status == 0x7fff))
            //if ((progtstatus == 0x7fff) && (progt1status == 0x7fff) && (progt3status == 0x7fff) && (progt4status == 0x7fff))
            if ((progtstatus == 0x7fff))
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.GtConfig_Gt, AcqBdNo.B0, 0x10);
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.GtConfig_Gt, AcqBdNo.B1, 0x10);
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.GtConfig_Gt, AcqBdNo.B2, 0x10);
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.GtConfig_Gt, AcqBdNo.B3, 0x10);
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.GtConfig_Gt, AcqBdNo.B4, 0x10);
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.GtConfig_Gt, AcqBdNo.B5, 0x10);
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.GtConfig_Gt, AcqBdNo.B6, 0x10);
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.GtConfig_Gt, AcqBdNo.B7, 0x10);
            }
            UInt32 acqgtstatus = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.GtConfig_Gt, AcqBdNo.B1);
            acqgtstatus = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.GtConfig_Gt, AcqBdNo.B1);
            UInt32 acqgt1status = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.GtConfig_Gt, AcqBdNo.B0);
            acqgt1status = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.GtConfig_Gt, AcqBdNo.B0);

            //gt ldh
            HdIO.WriteReg(ProcBdReg.W.Decoder_B1Enable, 0x0);

            Thread.Sleep(1);

            HdIO.WriteReg(ProcBdReg.W.Decoder_B1Enable, 0x01);

            Thread.Sleep(1);

            HdIO.WriteReg(ProcBdReg.W.Decoder_B1Enable, 0x0);

            //pcie
            HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, 0x0);

            Thread.Sleep(1);

            HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, 0x01);

            Thread.Sleep(1);

            HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, 0x0);

            //回读状态，等待10S才能读回状态
            Thread.Sleep(10000);

            UInt32 o_status = HdIO.ReadReg(PcieBdReg.R.AnalogChCtrl_ReadProbeStatus);

            Thread.Sleep(2000);

            if ((o_status == 0x0001))
            {
                HdIO.WriteReg(ProcBdReg.W.Decoder_B2Enable, 0x01);
            }


            #endregion

            CtrlAnalogChannel_JiHe2d5G.AnalogChannelMiscSetAll();

            #region 外触发初始化
            HdIO.WriteReg(PcieBdReg.W.PowerManager_ExtTrig_Power, 0x1);
            Thread.Sleep(1);

            //GT复位初始化

            HdIO.WriteReg(ProcBdReg.W.Exttrig_SoftRst, 0x0);
            Thread.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.Exttrig_SoftRst, 0x3);
            Thread.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.Exttrig_SoftRst, 0x0);

            //FIFO复位初始化
            Thread.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.Exttrig_GtFifoRst, 0x0);
            Thread.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.Exttrig_GtFifoRst, 0x1);
            Thread.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.Exttrig_GtFifoRst, 0x0);
          

            Thread.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.Exttrig_DelayEn, 0x0);
            Thread.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.Exttrig_DelayEn, 0x1);
            //HdIO.WriteReg(ProcBdReg.W.Exttrig_ExttrigSyncIoDelayEnVtc, 0x01);
            //HdIO.WriteReg(ProcBdReg.W.Exttrig_ExttrigSyncIoDelayNum, 0x0);
            //HdIO.WriteReg(ProcBdReg.W.Exttrig_ExttrigSyncIoDelayEnVtc, 0x00);
            //HdIO.WriteReg(ProcBdReg.W.Exttrig_ExttrigSyncIoDelayLoad, 0x00);
            //HdIO.WriteReg(ProcBdReg.W.Exttrig_ExttrigSyncIoDelayLoad, 0x01);
            //HdIO.WriteReg(ProcBdReg.W.Exttrig_ExttrigSyncIoDelayLoad, 0x00);
            //HdIO.WriteReg(ProcBdReg.W.Exttrig_ExttrigSyncIoDelayEnVtc, 0x01);

            //HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_4, 0x0);
            //HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_4, 0x1);
            #endregion 外触发初始化

            Thread.Sleep(1000);
            HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve21, 0x3);

            // 2023/07/23 开机第一次采集之前 进行温漂自校正 HChen添加
            //Hd.Calibration.TemperatureOffsetCalibration();

            //TS_init
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(0x40828, 0b00);  //不抽
            HdIO.WriteReg(ProcBdReg.W.DBI_DbiSampleMode, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_SelectSub, 0xf);
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, 0x8000);  //8 4 C
            HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_0, 0x8000);//8



            if (Constants.BOARD_ATTACHED)
            {
                ////自动校准TiAdc
                ////cij_0512_Change
                //Hd.Calibration?.TiAdcAutoCali_Exec();

                ////自动校准通道间同步
                //Hd.CurrProduct?.Acquirer_AnalogChannel?.ChnlSyncDiscardDotsEx();
				
				
				
            if (CurrProduct?.AutoCaliAtInit != null)
                CurrProduct?.AutoCaliAtInit();
            }



            //硬件初始化完成后，统一状态，并且强制Adc自校准一次
            Execute(hdMessage!);
   //         SetAuxInputMux(AuxInputType.Close);//AuxInput默认状态是Close
            Adc5200AutoCaliManager.Default.ExecForceCali();
            UpdateTempInfo();
            //打开失锁检查
            Hd.CurrDebugVarints.BEnable_Reverse0 = true;
            Hd.Calibration.CaliStatus = false;
            return true;
        }

        private static void LoadCaliData()
        {
            var bOK = LoadFlashCaliData();
            CaliDataManager.LoadAllFromFile(!bOK);
        }

        private static Boolean LoadFlashCaliData()
        {
            var bOK = true;
            Stopwatch stopwatch = Stopwatch.StartNew(); stopwatch.Start();
            foreach (var type in CaliDataManager.FlashCaliTypes)
            {
                var res = false;
                try
                {
                    res = ReadingCaliDataFromFlash(type, out var buffer);
                    res &= CalibrationStore.Default.SetCaliData(type, buffer);
                    Hd.SysLogger?.Invoke($"#### 加载校准数据{type} {stopwatch.ElapsedMilliseconds} {buffer.Length}", "Info"); stopwatch.Restart();
                }
                catch (Exception ex)
                {
                    Hd.SysLogger?.Invoke($"Read flash calidata error {ex.Message}", "Error");
                    res = false;
                    Hd.SysLogger?.Invoke($"#### 加载校准数据{type} {stopwatch.ElapsedMilliseconds} Fail", "Info"); stopwatch.Restart();
                }
                if (!res && Constants.ENABLE_CALIBDATA_REWRITE_AT_LOADFAIL == true)//没读到数据或者读到的数据反序列化失败 重新从本地读取一份写入Flash
                {
                    var calidata = CalibrationStore.Default.GetCaliData(type, true);
                    res = WritingCaliDataToFlash(type, calidata);
                    Hd.SysLogger?.Invoke($"#### 重写校准数据{type} {stopwatch.ElapsedMilliseconds} {calidata.Length} {res}", "Info"); stopwatch.Restart();
                }
                bOK &= res;
            }
            return bOK;
        }

        internal static Int64 LocalCommands = 0;
        /// <summary>
        /// 执行上电后的自动校准。是否执行由调用者决定。
        /// </summary>
        /// <param name="hdMessage"></param>
        /// <returns></returns>
        public static Boolean AutoCaliPowerOn(HdMessage hdMessage)
        {
            return false;
        }
        internal static Boolean LocalExecute(HdMessage hdMessage)
        {
            HdMessage newmessage = hdMessage with { Command = 0UL };
            return Execute(newmessage);
        }
        internal static Boolean BPowerOff = false;
        private static Int32 OldPowerOffKeyPressed = 0;
        internal static Boolean BFirstPower = true;
        /// <summary>
        /// 硬件响应。
        /// </summary>
        /// <param name="hdMessage">系统参数</param>
        /// <returns>是否执行过新的硬件命令</returns>
        public static Boolean Execute(HdMessage hdMessage)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            #region Power控制
            if ((hdMessage.ComboBits & 0x02) != (OldPowerOffKeyPressed & 0x02))
            {
                if ((OldPowerOffKeyPressed & 0x02) != 0)
                    BPowerOff = false;
                else
                    BPowerOff = true;
                HdMessage newhdmessage = hdMessage with { Command = BPowerOff ? 0U : 0xffff_ffff_ffff_ffffUL };
                BFirstPower = false;
                OldPowerOffKeyPressed = hdMessage.ComboBits & 0x02;
                Initialize(newhdmessage);
            }
            #endregion

            UIMessage = hdMessage with { };

            UInt64 currcommands = hdMessage.Command | (UInt64)LocalCommands;
            if ((hdMessage.Command & ((UInt64)HdCmd.Run)) != 0)
            {
                if (!(currcommands==0x0500000000000200))
                    if (!hdMessage.bAcquireStopped)
                        currcommands |= 0xffff_ffff_ffff_ffffUL;
            }
            LocalCommands = 0;
            if (currcommands == 0)
                return false;
            List<Action> executelist = new List<Action>();
            waitMillisecondList.Clear();
            #region 获取需要执行的Action
            if (currcommands != (UInt64)HdCmd.None)
            {
                if (currcommands != ~0UL)
                {
                    foreach (HdCmd cmd in Enum.GetValues(typeof(HdCmd)))
                    {
                        if ((((UInt64)cmd) & currcommands) != 0)
                        {
                            HdCmd hdcmd = (HdCmd)cmd;
                            if (cmdTable.ContainsKey(hdcmd))
                            {
                                foreach (Action action in cmdTable[hdcmd])
                                {
                                    if (!executelist.Contains(action))
                                        executelist.Add(action);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<HdCmd, Action[]> kvp in cmdTable)
                    {
                        foreach (Action action in kvp.Value)
                        {
                            if (!executelist.Contains(action))
                                executelist.Add(action);
                        }
                    }
                }
            }
            #endregion
            if (!hdMessage.bAcquireStopped && executelist.Count > 0)
                Acquisition.CreateAcquireAttribute();
            if (CaliDataManager.CheckDataChanged())
                LocalCommands |= (Int64)HdCmd.CaliDataChanged;

            #region 二次命令,解决系数重发，通道信号，这些处理在CreateAcquireAttribute 中处理
            currcommands = (UInt64)LocalCommands;
            LocalCommands = 0;
            if (currcommands != 0)
            {
                foreach (HdCmd cmd in Enum.GetValues(typeof(HdCmd)))
                {
                    if ((((UInt64)cmd) & currcommands) != 0)
                    {
                        HdCmd hdcmd = (HdCmd)cmd;
                        if (cmdTable.ContainsKey(hdcmd))
                        {
                            foreach (Action action in cmdTable[hdcmd])
                            {
                                if (!executelist.Contains(action))
                                    executelist.Add(action);
                            }
                        }
                    }
                }
            }
            #endregion
            if (executelist.Count > 0 && !IsOnlyControlAWGCmd(executelist) && !UIMessage.bAcquireStopped)
            {
                Acquisition.AcqStop();
            }
            #region 执行每个Action
            foreach (Action action in executelist)
            {
                try
                {
                    action();
                }
                    catch (Exception e)
                {
                    Hd.SysLogger?.Invoke($"==!!!!Driver!!!!== 调试阶段异常捕获： Hd.Execute 异常[ {e.Message},{e.StackTrace}],Action Name=[{action.Method.Name}]", "Info");
                }
            }
            #endregion
            #region 延时处理
            if (waitMillisecondList.Count > 0)
            {
                for (Int32 i = 0; i < waitMillisecondList.Count; i++)
                {
                    if (waitMillisecondList[i].Key.IsRunning)
                    {
                        if (waitMillisecondList[i].Key.ElapsedMilliseconds > waitMillisecondList[i].Value)
                            waitMillisecondList[i].Key.Stop();
                    }
                }
            }
            #endregion

            if (executelist.Count > 0 && !IsOnlyControlAWGCmd(executelist))
            {
                Acquisition.InitAcq(!UIMessage.bAcquireStopped);
                if (UIMessage!.Timebase!.IsScan && !UIMessage.bAcquireStopped)
                {
                    Acquisition.ScanRunningNewDataPerChannelExistsDotCount = 0;
                    Acquisition.ScanPerChannelInDdrTotalDotCount_AlreadyDisplay = 0;
                    Acquisition.ScanPerChannelInDdrDotCount_NotDisplay = 0;
                }
            }
            OldMessage = UIMessage with { };
            stopwatch.Stop();
            return executelist.Count > 0;
        }
        /// <summary>
        /// 判断是否仅控制AWG指令
        /// </summary>
        /// <param name="executelist">执行指令集合</param>
        /// <returns></returns>
        private static Boolean IsOnlyControlAWGCmd(List<Action> executelist)
        {
            if (executelist.Count == 1)
            {
                Action action = executelist[0];
                Action action1 = cmdTable[HdCmd.AWGConfig][0];
                if (action == action1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        internal static UInt32 trigState = 0;
        public static UInt32 TrigState
        {
            get => trigState;
        }
        internal static UInt32 _RegionTrigStatus = 0;
        public static UInt32 RegionTrigStatus { get => _RegionTrigStatus; }

        private static Boolean BAcqStatisticsRunning = false;
        private static Int64 AcqStatisticsTimes_Trigged = 0;
        private static Int64 AcqStatisticsTimes_AcqWaveCalled = 0;
        private static Int64 AcqStatisticsTimes_AcqWaveOk = 0;
        private static Stopwatch AcqStatisticsStopwatch = new Stopwatch();
        internal static Boolean AcqStatisticsRunning
        {
            get => BAcqStatisticsRunning;
            set
            {
                BAcqStatisticsRunning = value;
                if (value)
                {
                    AcqStatisticsTimes_Trigged = 0;
                    AcqStatisticsTimes_AcqWaveCalled = 0;
                    AcqStatisticsTimes_AcqWaveOk = 0;
                    AcqStatisticsStopwatch.Restart();
                }
                else
                    AcqStatisticsStopwatch.Stop();
            }
        }
        internal static String GetAcqStatisticsInfo()
        {
            return $"bRunning={BAcqStatisticsRunning},ElapsedMilliseconds={AcqStatisticsStopwatch.ElapsedMilliseconds},TriggedTimes={AcqStatisticsTimes_Trigged},AcqWaveCalledTimes={AcqStatisticsTimes_AcqWaveCalled},AcqWaveOkTimes={AcqStatisticsTimes_AcqWaveOk}";
        }

        private static UInt32 _LastTrigState = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>只要主要的FIFO满，就返回true;是否真实存在数据，需要相应的采集器的读取函数确定</returns>
        public static Boolean AcqWave(Boolean bStoped, Boolean bNeedReset, List<ReadInfo> readInfoList, ref Dictionary<AcqDataType, Double> HardwareSampeIntervalByus, CancellationToken? softResetToken = null)
        {
            //获取系统温度
            SystemMonitor.Default.SystemTemperatureProcess();

            bAcqedNewData = Acquisition.AcquireQuery(bStoped, bNeedReset);

            //更新触发状态
            Hd.trigState = AbstractController_Misc.ReadTrigStatus();

            if (_LastTrigState != Hd.trigState)
            {
                Trace.WriteLine($"Trig State Changed {_LastTrigState} -> {Hd.trigState}");
                _LastTrigState = Hd.trigState;
            }

            if (bAcqedNewData)
            {
                Acquisition.Acquire(readInfoList, ref HardwareSampeIntervalByus, softResetToken);
                Acquisition.AcquireReset();
            }
            else
            { 
                
            }
            if (BAcqStatisticsRunning)
            {
                AcqStatisticsTimes_AcqWaveCalled++;
                if ((Hd.TrigState & 0x4) != 0)
                    AcqStatisticsTimes_Trigged++;
                if (bAcqedNewData)
                    AcqStatisticsTimes_AcqWaveOk++;
            }
            UpdateTempInfo();
            CfgFansSpeed();

            return bAcqedNewData;
        }


        /// <summary>
        /// 波形获取器
        /// </summary>
        public static AbstractAcquirer_AnalogChannel? AnalogChannel { get => CurrProduct?.Acquirer_AnalogChannel; }
        /// <summary>
        /// 温度传感器
        /// </summary>
        public static AbstractAcquirer_Temperaturer? Temperaturer { get => CurrProduct?.Acquirer_Temperaturer; }

        public static readonly Cali Calibration = new Cali();

        public static SystemMonitor SystemMonitor { get => SystemMonitor.Default; }
        public static Boolean Ext10MHzLocked
        {
            get
            {//cij_0805
                return (HdIO.ReadReg(ProcBdReg.R.reverse_pro_reverse_rd_reg_0)) != 0;
                //return ((HdIO.ReadReg(ProcBdReg.R.ext_10m_status_of_clock) & 0x8000) != 0) && ((HdIO.ReadReg(PcieBdReg.R.SysMon_Pcie_Pll_Locked) & 0x07) == 0x07);
            }
        }
        public static void DoForceTrigger()
        {
            ConditionManager.TriggerCtrlEn = true;
            RegSendManager.Default.Send((UInt32)ProcBdReg.W.TrigCtrl_DigitalTrigEn_Pro);
            ConditionManager.TriggerCtrlEn = false;
            RegSendManager.Default.Send((UInt32)ProcBdReg.W.TrigCtrl_DigitalTrigEn_Pro);
            ConditionManager.TriggerCtrlEn = true;
            RegSendManager.Default.Send((UInt32)ProcBdReg.W.TrigCtrl_DigitalTrigEn_Pro);
        }
        public static ProductConfig ProductConfig => ProductConfig.Defalut;
        public static void SetAuxInputMux(AuxInputType auxInputType)
        {
            switch (auxInputType)
            {
                case AuxInputType.Trigger:
                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_aux_in_setting, 0x00);
                    break;
                case AuxInputType.Sync_AWG:
                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_aux_in_setting, 0x01);
                    break;
                case AuxInputType.Close:
                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_aux_in_setting, 0x02);
                    break;
            }
        }
        public static void SetAuxIutputPolarity(EdgeSlope auxInPolarity)
        {
            HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_aux_in_setting_polarity, (UInt32)auxInPolarity);
        }
        public static void SetAuxOutputMux(AuxOutputType auxOutputType)
        {
            switch (auxOutputType)
            {
                case AuxOutputType.Close:
                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_aux_out_setting, 0x03);
                    break;
                case AuxOutputType.Trigger:
                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_aux_out_setting, 0x00);
                    break;
                case AuxOutputType.Sync_AWG:
                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_aux_out_setting, 0x02);
                    break;
                case AuxOutputType.Other:
                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_aux_out_setting, 0x01);
                    break;
            }
        }
        public static void SetAuxOutputSignal(Boolean bHigh)
        {
            HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_AuxOutOther, bHigh ? 0x01U : 0x0U);
        }
        public static void SetAuxOutputPolarity(EdgeSlope edgeSlope)
        {
            switch (edgeSlope)
            {
                case EdgeSlope.Rise:
                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_TrigOutputPolaritySelect, 0x00);
                    break;
                case EdgeSlope.Fall:
                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_TrigOutputPolaritySelect, 0x01);
                    break;
                case EdgeSlope.Both:
                    break;
            }
        }
        public static void SetPassFailWarn()
        {

            HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_ProPassFailWarn, 1);
            System.Threading.Thread.Sleep(10);
            HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_ProPassFailWarn, 0);

        }
        public static UInt32 ConditionFilter = 0;
        public static void SetConditionFilter(Int32 channelIndex, Boolean enable)
        {
            if (channelIndex > 3)
                return;
            if (enable)
            {
                UInt32 data = 1U << (channelIndex + 1);
                ConditionFilter = ConditionFilter | data;
            }
            else
            {
                UInt32 data = 28;
                if (channelIndex == 1)
                {
                    data = 26;
                }
                else if (channelIndex == 2)
                {
                    data = 22;
                }
                else if (channelIndex == 3)
                {
                    data = 14;
                }

                ConditionFilter = ConditionFilter & data;
            }
            ConditionFilter = ConditionFilter | (Hd.CurrDebugVarints.bEnable_AdcConditionFilter ? 1U : 0);

            //currProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ConditionFilter_Enable, ConditionFilter);
        }

        public static void ConfigEResFIRCoefficients(Double EnhancedBits, Boolean bEnableMode)
        {
            return;
        }

        public static List<Int64> GetSpanListForTimeFreq(Int64 freqSpan) => AbstractAcquirer_RadioFrequency.GetSpanListForTimeFreq(freqSpan);

        public static (double, double) GetValidRect(double startPosByns, double lengthByns)
        {
            return ArtificialIntelligenceProcess.Default.MultiDomainProcess.GetValidRect(startPosByns, lengthByns);
        }

        public static long GetValidMaxSpanFreq(double maxExtramNum)
        {
            return ArtificialIntelligenceProcess.Default.MultiDomainProcess.GetValidMaxSpanFreq(maxExtramNum);
        }

        public static double GetAcutalFFTLength()
        {
            return ArtificialIntelligenceProcess.Default.MultiDomainProcess.GetAcutalFFTLength();
        }

        public static void OuterLedCtrl(Dictionary<OuterPannelLEDType, (Boolean enable, Byte BlueOrLight, Byte Green, Byte Red)> define)
        {
        }
        public static Dictionary<String, UInt64> Read_MiscDataFromFPGA()
        {
            Dictionary<String, UInt64> readback = new Dictionary<String, UInt64>();
            Read_EdgeCounter("EdgeCounter", ref readback);
            Read_DVM("DVM", ref readback);
            Read_AcqWaveStatistics("AcqWaveStatistics", ref readback);
            return readback;
        }

        /// <summary>
        /// 获取通道和外触发接口的高压警告信息
        /// </summary>
        public static Int32 GetChannelAndEXTTriggerHighVoltageWarningInfo() => CtrlAnalogChannel_JiHe2d5G.baseObj1?.HighVoltageWarning ?? 0;

        public static void ResetHighVoltageWarning(IEnumerable<ChannelId> channelIds) => CtrlAnalogChannel_JiHe2d5G.baseObj1?.ResetHighVoltageWarning(channelIds);

        public static void ResetAllHighVoltageWarning() => CtrlAnalogChannel_JiHe2d5G.baseObj1?.ResetHighVoltageWarning(null);

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="acquirerType">采集器的种类</param>
        /// <param name="paramInfos">需要获取数据的参数</param>
        /// <param name="paramsData">返回的数据</param>
        /// <returns>函数内部执行情况</returns>
        public static String TryGetData(ChannelType acquirerType, Object paramInfos, out Object? paramsData)
        {
            switch (acquirerType)
            {
                case ChannelType.Analog:
                    if (AnalogChannel == null)
                    {
                        paramsData = null;
                        return "AnalogChannel is null";
                    }
                    return AnalogChannel.TryGetData(paramInfos, out paramsData);
                case ChannelType.RadioFrequency:
                    return ArtificialIntelligenceProcess.Default.MultiDomainProcess.TryGetData(paramInfos, out paramsData);
                case ChannelType.ReconfigDBI:
                    return ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.TryGetData(paramInfos, out paramsData);
                case ChannelType.EmdProcess:
                    return EmdProcess.Default.TryGetData(paramInfos, out paramsData);
            }

            paramsData = null;
            return $"{acquirerType} not support!";
        }



        /// <summary>
        /// 提供给外部调用，只能获取纯软的参数
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="paramType"></param>
        /// <param name="paramInfo"></param>
        /// <returns></returns>
        public static Object? GetParamters(String moduleName, String paramType, Object? paramInfo)
        {
         

            if (moduleName == "System")
            {
                if (paramType == "FansName")
                    return (Object)GetFansName();
                if (paramType == "TempInfo")
                    return (Object)GetTempInfo();
            }

            return $"{Hd.CurrProductType} not support {moduleName}:{paramType}!";
        }

        private static String[] GetFansName()
        {
            return _FansCtrlDefine.Keys.ToArray();
        }

        private static Dictionary<String, Double> _TempInfo = new();

        /// <summary>
        /// 从FPGA读取温度，并缓存在_TempInfo中
        /// </summary>
        private static void UpdateTempInfo()
        {
            HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 0);

            //_TempInfo["PCIe"] = ReadPcieTemp();
            //_TempInfo["PROC"] = ReadProcTemp();

            if (CurrProduct?.AcqBd != null)
            {
                _TempInfo = ReadTemperature();
                //foreach (AcqBdNo acqbd in CurrProduct.AcqBd.ExistsDefines)
                //{
                //    _TempInfo[$"{acqbd}_FPGA"] = ReadAcqFpga(acqbd);
                //    _TempInfo[$"{acqbd}_ADC0"] = ReadAcqAdc(acqbd, 0);
                //    _TempInfo[$"{acqbd}_ADC1"] = ReadAcqAdc(acqbd, 1);
                //    _TempInfo[$"{acqbd}_PCB0"] = ReadAcqPcbTemp(acqbd, 0);
                //    _TempInfo[$"{acqbd}_PCB1"] = ReadAcqPcbTemp(acqbd, 1);
                //}
            }

            //HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 1);
        }

        private static Dictionary<String, Double> ReadTemperature()
        {
            Dictionary<String, Double> keyValuePairs = new Dictionary<String, Double>();

            //StringBuilder stringbuilder = new StringBuilder();
            #region Pcie board
            String boardName = "Pcie:";
            HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 0);
            //HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 1);
            Thread.Sleep(5);
            UInt32 data = 0;
            Double temprature = 0.0;

            data = HdIO.ReadReg(PcieBdReg.R.SysMon_Sysmon_Temperature);
            temprature = (data * 501.3743 / 1024) - 273.15;
            //stringbuilder.AppendLine($"{boardName}SysMon_pcie_fpga_temp={temprature.ToString("0.00")}℃");
            keyValuePairs.Add($"{boardName}_fpga", temprature);

            //data = HdIO.ReadReg(PcieBdReg.R.SysMon_Ct_1820_Driver_Temp_Data);
            //temprature = data * 0.0625;
            ////stringbuilder.AppendLine($"{boardName}SysMon_Ct_1820_Driver_Temp_Data={temprature.ToString("0.00")}℃");
            //keyValuePairs.Add($"{boardName}SysMon_Ct_1820_Driver_Temp_Data", temprature);
            //data = HdIO.ReadReg(PcieBdReg.R.SysMon_Ct_1820_Driver_Temp_Data_pro);
            //temprature = data * 0.0625;
            ////stringbuilder.AppendLine($"{boardName}SysMon_Ct_1820_Driver_Temp_Data_pro={temprature.ToString("0.00")}℃");
            //keyValuePairs.Add($"{boardName}SysMon_Ct_1820_Driver_Temp_Data_pro", temprature);
            ////HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 0);
            #endregion
            #region S6Board

            #endregion
            #region ProcessBoard
            boardName = "Pro:";
            HdIO.WriteReg(ProcBdReg.W.SysMon_Sysmon_Rst, 0);
            //HdIO.WriteReg(ProcBdReg.W.SysMon_pro_sysmon_rst, 1);
            Thread.Sleep(5);
            data = HdIO.ReadReg(ProcBdReg.R.SysMon_Sysmon_Temperature);
            data &= 0xfff;
            temprature = (int)data;
            temprature = (temprature * 503.975 / 1024) - 273.15;
            //stringbuilder.AppendLine($"{boardName}SysMon_pro_fpga_temp={temprature.ToString("0.00")}℃");
            keyValuePairs.Add($"{boardName}_fpga", temprature);
          
            #endregion
            //stringbuilder.AppendLine("======================================");
            #region AcqBoard
            for (var acqBdIndex = 0; acqBdIndex < Hd.CurrProduct!.AcqBd!.ExistsDefines.Count; acqBdIndex++)
            {
                if (Hd.CurrProduct!.AcqBd!.ExistsDefines[acqBdIndex].ISENABLE)
                {
                    AcqBdNo acbdno = (AcqBdNo)acqBdIndex;
                    boardName = "Acq_" + acbdno.ToString() + ":";

                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_fpga_temp, acbdno);
                    data &= 0x3ff;
                    temprature = (Int32)data;
                    temprature = (temprature * 501.3743 / 1024) - 273.6777;
                    //stringbuilder.AppendLine($"{boardName}SysMon_acq_fpga_temp={temprature.ToString("0.00")}℃");
                    keyValuePairs.Add($"{boardName}_fpga", temprature);

                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_fpga_vccaux, acbdno);
                    data &= 0x3ff;
                    temprature = (Int32)data;
                   
                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_adc1, acbdno);
                    data &= 0x7FF;
                    temprature = (Int32)data * 0.125;
                    //stringbuilder.AppendLine($"{boardName}SysMon_acq_adc1_temperature={temprature.ToString("0.00")}℃");
                    keyValuePairs.Add($"{boardName}adc1", temprature);
                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_adc2, acbdno);
                    data &= 0x7FF;
                    temprature = (Int32)data * 0.125;
                    //stringbuilder.AppendLine($"{boardName}SysMon_acq_adc2_temperature={temprature.ToString("0.00")}℃");
                    keyValuePairs.Add($"{boardName}adc2", temprature);
                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_pcb1, acbdno);
                    data &= 0x7FF;
                    temprature = (int)data * 0.125;
                    //stringbuilder.AppendLine($"{boardName}SysMon_acq_pcb1_temperature={temprature.ToString("0.00")}℃");
                    keyValuePairs.Add($"{boardName}pcb1", temprature);
                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_pcb2, acbdno);
                    data &= 0x7FF;
                    temprature = (Int32)data * 0.125;
                    //stringbuilder.AppendLine($"{boardName}SysMon_acq_pcb2_temperature={temprature.ToString("0.00")}℃");
                    keyValuePairs.Add($"{boardName}pcb2", temprature);
                }
            }
            #endregion
            return keyValuePairs;
        }

        //private static Double ReadPcieTemp()
        //{
        //    UInt32 tmpdata = HdIO.ReadReg(PcieBdReg.R.SysMon_Sysmon_Temperature);
        //    Double pcie_temprature = tmpdata & 0xfff;
        //    pcie_temprature = (pcie_temprature * 501.3743) / 1024 - 273.6777;
        //    return pcie_temprature;
        //}

        //private static Double ReadProcTemp()
        //{
        //    UInt32 data = HdIO.ReadReg(ProcBdReg.R.SysMon_pro_fpga_temp);
        //    Double temprature = data & 0xfff;
        //    temprature = (temprature * 503.975) / 4096 - 273.15;
        //    return temprature;
        //}

        //private static Double ReadAcqFpga(AcqBdNo acqBd)
        //{
        //    UInt32 data = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.SysMon_acq_fpga_temp, acqBd) ?? 0;
        //    Double temprature = data & 0x3ff;
        //    temprature = (temprature * 501.3743) / 1024 - 273.6777;
        //    return temprature;
        //}


        private static Dictionary<Int32, AcqBdReg.R> _AdcTempRegDefine = new()
        {
            [0] = AcqBdReg.R.SysMon_acq_temp_adc1,
            [1] = AcqBdReg.R.SysMon_acq_temp_adc2,
        };

        private static Double ReadAcqAdc(AcqBdNo acqbd, Int32 adcId)
        {
            if (_AdcTempRegDefine.ContainsKey(adcId) && Hd.CurrProduct?.AcqBd != null)
            {
                UInt32 data = Hd.CurrProduct.AcqBd.ReadReg(_AdcTempRegDefine[adcId], acqbd);
                Double temprature = data & 0x3ff;
                return temprature * 0.125;
            }
            return 0.0;
        }

        private static Dictionary<Int32, AcqBdReg.R> _PcbTempRegDefine = new()
        {
            [0] = AcqBdReg.R.SysMon_acq_temp_pcb1,
            [1] = AcqBdReg.R.SysMon_acq_temp_pcb2,
        };

        private static Double ReadAcqPcbTemp(AcqBdNo acqbd, Int32 pcbId)
        {
            if (_PcbTempRegDefine.ContainsKey(pcbId) && Hd.CurrProduct?.AcqBd != null)
            {
                UInt32 data = Hd.CurrProduct.AcqBd.ReadReg(_PcbTempRegDefine[pcbId], acqbd);
                Double temprature = ComplementConvert(data) * 0.125;
                return temprature;
            }
            return 0.0;
        }

        private static int ComplementConvert(UInt32 data)
        {
            data &= 0x7ff;
            int mark = 1;
            if ((data & 0x400) != 0)
            {
                mark = -1;
                data &= 0x3ff;
                data = ~data;
                data &= 0x3ff;
                data = +1;
            }
            //补码运算
            int temprature = (int)data;
            temprature *= mark;
            return temprature;
        }

        private static Dictionary<String, Double> GetTempInfo()
        {
            return _TempInfo;
        }


        private static Dictionary<String, Action<Int32>> _FansCtrlDefine = new()
        {
            ["机箱风扇"] = CtrlCaseFanSpeed,
            //["采集板风扇"] = CtrlAcqBdFanSpeed,
        };
        private static void CtrlCaseFanSpeed(Int32 speed)
        {
            //HdIO.WriteReg(PcieBdReg.W.FanCtrlPwm_RegFanCtrlPwmRst, 0x02);//复位控制 FanCtrlPwm_RegFanCtrlPwmRst
            //HdIO.WriteReg(PcieBdReg.W.FanCtrlPwm_RegPwmThreshold, (UInt32)(speed * 100));//风扇转速控制 10%:0X03E8 ，20%:0X07D0， 30%：0x0BB8， 40%：0x0FA0，50%：0x1388，60%：0x1770，70%：0x1B58，80%：0x1F40，90%：0x2328, 100%：0x2710
            HdIO.WriteReg(PcieBdReg.W.FanCtrlPwm_RegPwmDutyCycleStaicPcie, 0x01);//上位机单片机风扇控制使能，上位机接入有效位至1
            HdIO.WriteReg(PcieBdReg.W.FanCtrlPwm_RegPwmCtrlTxEnPcie, (UInt32)(speed));//上位机风扇转速控制，0~100
            HdIO.WriteReg(PcieBdReg.W.FanCtrlPwm_RegPwmThresholdPcie, 0x01);//uart发送使能状态,有效位至1
            HdIO.Sleep(10);
            HdIO.WriteReg(PcieBdReg.W.FanCtrlPwm_RegPwmThresholdPcie, 0x00);//uart发送使能状态,有效位至1
        }
        //private static void CtrlAcqBdFanSpeed(Int32 speed)
        //{
        //    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.FanCtrlPwm_RegFanCtrlPwmRstAcq, 0x02);//复位控制 FanCtrlPwm_RegFanCtrlPwmRst
        //    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.FanCtrlPwm_RegPwmThresholdAcq, (UInt32)(speed * 100 * 0.85));//风扇转速控制 10%:0X03E8 ，20%:0X07D0， 30%：0x0BB8， 40%：0x0FA0，50%：0x1388，60%：0x1770，70%：0x1B58，80%：0x1F40，90%：0x2328, 100%：0x2710
        //}
        internal static void CfgFansSpeed()
        {
            var systemctrl = UIMessage?.System;
            if (systemctrl != null)
            {
                foreach (String fanname in _FansCtrlDefine.Keys)
                {
                    if (systemctrl.FansSpeed.ContainsKey(fanname))
                    {
                        _FansCtrlDefine[fanname].Invoke(systemctrl.FansSpeed[fanname]);
                    }
                }
            }
        }

        public static void TrySetExcuteAction(ChannelType acquirerType, Object paramInfos)
        {
            switch (acquirerType)
            {
                case ChannelType.Analog:
                    return;
                case ChannelType.EmdProcess:
                    if (paramInfos.ToString() == "WfmStudy")
                    {
                        EmdProcess.Default.WfmStudy();
                    }
                    if (paramInfos.ToString() == "ExcuteExCapture")
                    {
                        EmdProcess.Default.ExcuteExCapture();
                    }
                    return;
            }
        }
    }
    public enum AnalogParamEnum
    {
        WaveByteSize,
        AdcWaveData,
        StorageDotsCnt,
        TriggerAddrStart,
        SaveDataSegementDotsLength,
        AdcInterleaveMode,
    }
}
