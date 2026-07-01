using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Reflection.Metadata;
using ScopeX.Hardware.Calibration.Data.Base;
using System.Drawing;
using ScopeX.Hardware.Driver.Registers.SendManage;
namespace ScopeX.Hardware.Driver
{
    public class Acquirer_DPX_Standard : AbstractAcquirer_DPX
    {
        /// <summary>
        /// 余辉参数与波形暂留时间的定义表
        /// </summary>
        private readonly Dictionary<UInt32,Int32> _WaveDuration = new Dictionary<UInt32, Int32>
        {
            {0, 0 },   //0-Close;
            {1, 50 },   //1-50ms;
            {2, 100 },  //2-100ms;
            {3, 200 },  //3-200ms;
            {4, 500 },  //4-500ms;
            {5, 1000 }, //5-1000ms
            {6, 2000 }, //6-2000ms
            {7, 5000 },//7-5000ms
            {8, 10000 },//8-10000ms
            {9, 20000 },//0-20000ms
            {10, -1 },  //10-无限;
        };

        /// <summary>
        /// 余辉类型与余辉参数的定义表
        /// </summary>
        private readonly Dictionary<WfmPersist, UInt32> _PersistenceType = new Dictionary<WfmPersist, UInt32> 
        {
            { WfmPersist.Close , 0},
            { WfmPersist.Auto , 5},
            { WfmPersist.Infinity , 10},
        };

        private HdMessage? _LastAcqMsg;
        public Acquirer_DPX_Standard()
        {
            ConfigFunc = ourConfig;
        }
        private Int64 _WrittedTimestamp = 0;
        internal override void SetWrittedTimeStamp()
        {
            _WrittedTimestamp = Acquisition.AcqFull_TimeStamp;
        }
        internal override void Init()
        {

        }
        private Int32 CalcChannelLevel(HdMessage compareHdMessage)
        {
            IncludeChannels.Clear();
            Boolean[] bchannelneeddisplay = new Boolean[ChannelIdExt.AnaChnlNum];
            Int32 displaycount = 0;
            for (Int32 i = 0; i < ChannelIdExt.AnaChnlNum; i++)
            {
                bchannelneeddisplay[i] = Hd.UIMessage!.Analog![i]!.Active && compareHdMessage.Analog![i]!.Active;
                if (bchannelneeddisplay[i])
                    displaycount++;
            }
            if (displaycount == 0)
            {
                bValid = false;
                return -1;
            }
            bValid = true;
            Int32 channellevels = 0;
            Int32 displayindex = 0;
            Int32 topchnlid = 0;
            for (Int32 i = ChannelIdExt.AnaChnlNum - 1; i >= 0; i--)
            {

                Int32 chnliD = (Hd.UIMessage!.Display!.AnalogZIndex >> i * 4) & 0x03;
                if (bchannelneeddisplay[chnliD])
                {
                    topchnlid = chnliD;
                    channellevels |= (chnliD << (displayindex * 2));
                    IncludeChannels.Add((ChannelId)chnliD);
                    displayindex++;
                }
            }
            for (Int32 j = displayindex; j < 4; j++)
            {
                channellevels |= (topchnlid << (j * 2));
            }
            IncludeChannels.Reverse();
            return channellevels;
        }
        internal override Boolean IsParamNotChangedAtStop()
        {
            if (_LastAcqMsg == null)
                return true;
            if (Hd.UIMessage!.Timebase!.TmbScaleIndex != _LastAcqMsg!.Timebase!.TmbScaleIndex || Hd.UIMessage.Timebase.TmbPositionIndex != _LastAcqMsg.Timebase.TmbPositionIndex)
            {
                bParamNotChangedAtStop = false;
                return false;
            }
            for (Int32 channelID = 0; channelID < ChannelIdExt.AnaChnlNum; channelID++)
            {
                if (!Hd.UIMessage!.Analog![channelID].Equals(_LastAcqMsg!.Analog![channelID]))
                {
                    bParamNotChangedAtStop = false;
                    return false;
                }
            }
            return true;
        }

        const UInt32 BESTL2MAXEXTRACTNUM = 1000;
        private (UInt32 L1, UInt32 L2) SplitL1L2ExtractNum(AdcInterleaveMode interleaveMode, UInt32 totalExtractNum)
        {
            if (totalExtractNum <= BESTL2MAXEXTRACTNUM)
                return (1, totalExtractNum);
            UInt32 mergeroads = interleaveMode switch
            {
                AdcInterleaveMode.Mode2To1 => 16,
                AdcInterleaveMode.Mode4To1 => 32,
                _ => 8,
            };
            UInt32[] baseextractnum = AbstractAcquirer_AnalogChannel.PreExtractGapModeList.Keys.ToArray().OrderByDescending(x => x).ToArray();
            foreach (UInt32 value in baseextractnum)
            {
                if (value <= mergeroads)
                {
                    if ((totalExtractNum % value) == 0)
                        return (value, totalExtractNum / value);
                }
            }
            return (1, totalExtractNum);
        }
        internal override void InitGenerateParams(Boolean bUseOldData)
        {
            Boolean isfast = Hd.UIMessage!.Display!.IsFast;

            if (isfast)
            {
                var uiinterleavemode = bUseOldData ? Acquisition.AcqedDataMsg!.Timebase!.InterleaveMode : Hd.UIMessage!.Timebase!.InterleaveMode;
                UInt32 uineedmindotscount = uiinterleavemode switch
                {
                    AdcInterleaveMode.Mode2To1 => 100_000U,
                    AdcInterleaveMode.Mode1To1 => 50_000U,
                    _ => 50_000U,
                } ;

                Int32 channellevels = CalcChannelLevel(bUseOldData ? Acquisition.AcqedDataMsg! : Hd.UIMessage!);
                if (!bValid)
                    return;
                AcquireAttribute curracquireattribute = Acquisition.CurrDataAcquireAttribute;// bUseOldData ? Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquedParameters : Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters;
                //AcquireAttribute? acquireAttribute = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquedParameters;

                Double uineedtotaltimebyus = Hd.UIMessage!.Timebase!.TmbScale * Constants.VIS_XDIVS_NUM;
                ReadParams readparam = Hd.CurrProduct!.Acquirer_AnalogChannel!.CalcDdrReadParams(
                    DdrData4What.Upo,
                    Acquisition.CurrDataAcquireAttribute,
                    //bUseOldData ? Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquedParameters : Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters,
                    Hd.UIMessage.Timebase.TmbPosition,
                    uineedtotaltimebyus,
                    uineedmindotscount,
                    _WrittedTimestamp
                    );

                if(readparam.PerChannelRecvDotsCount <=0 || readparam.bInterpolateNumGT100)
                {
                    if (!Hd.UIMessage.bAcquireStopped)
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Dpo_DpxEnable, isfast ? 1U : 0);//暂时关闭乒乓
                    else
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Dpo_DpxEnable, 0);
                    bValid = false;
                    return;
                }
                if (readparam.PerChannelRecvDotsCount*readparam.Interpolate_Num_Double/readparam.UPO_Level1ExtractNum/readparam.UPO_Level2ExtractNum<1)
                {
                    bValid = false;
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Dpo_DpxEnable, 0);
                    return;
                }
                HdMessage channelinfousemessage = Hd.UIMessage!.bAcquireStopped ? /*bUseOldData ? */Acquisition.AcqedDataMsg! : Hd.UIMessage!;
                for (int channelindex = 0; channelindex < ChannelIdExt.AnaChnlNum; channelindex++)
                {
                    readparam.Ddr_PosGainFine[channelindex] = channelinfousemessage.Analog![channelindex].Scale / Hd.UIMessage!.Analog![channelindex].Scale;
                    readparam.Ddr_PosOffset[channelindex] = (short)(channelinfousemessage.Analog![channelindex].PositionIndex * Constants.SAMPS_PER_YDIV / Constants.IDX_PER_YDIV + Constants.MAX_ADC_RES / 2);
                    bool bneedinvertprocessoffsetdelta = (Hd.UIMessage!.Analog![channelindex].IsInverted, channelinfousemessage.Analog[channelindex].IsInverted) switch
                    {
                        (false, false) => false,
                        (true, true) => false,
                        (_, _) => true,
                    };
                    readparam.Ddr_PosOffsetDelta[channelindex] = (short)((Hd.UIMessage!.Analog![channelindex].PositionIndex - channelinfousemessage.Analog![channelindex].PositionIndex) * Constants.SAMPS_PER_YDIV / Constants.IDX_PER_YDIV);
                    if (Hd.UIMessage!.bAcquireStopped && bneedinvertprocessoffsetdelta)
                        readparam.Ddr_PosOffsetDelta[channelindex] += (short)(2 * channelinfousemessage.Analog![channelindex].PositionIndex * Constants.SAMPS_PER_YDIV / Constants.IDX_PER_YDIV);

                    readparam.Ddr_PosInvert[channelindex] = Hd.UIMessage!.Analog![channelindex].IsInverted;//反相目前是由Core层操作
                    //readParam.Ddr_PosInvert[channelIndex] = Hd.UIMessage!.Analog![channelIndex].IsInverted != channelInfoUseMessage.Analog![channelIndex].IsInverted;

                }
                bInterpolateNumGT100 = readparam.bInterpolateNumGT100;
                ColumnValidCount = readparam.UPO_ResultColoumnNum;
                Int32 uimainwinextractnum = (Int32)(uineedtotaltimebyus / readparam.UPO_ResultColoumnNum / readparam.SampleIntervalByUs);
                if (uimainwinextractnum <= 0)
                    uimainwinextractnum = 1;
                CurrDpxAcqParameters = new DpxAcqParameters()
                {
                    UIMainWinExtractNum = uimainwinextractnum,
                    DdrReadParam = readparam,
                };

                Int32 mainwinxstartpos = 0;
                Int32 mainwinxendpos = (Int32)(readparam.UPO_ResultColoumnNum - 1);
                if (bUseOldData)
                {
                    double uitheorystarttimebyus = Hd.UIMessage.Timebase.TmbPosition - Hd.UIMessage.Timebase.TmbScale * Constants.VIS_XDIVS_NUM / 2;
                    double realstarttimebyus = Acquisition.AcqedDataMsg!.Timebase!.TmbPosition - Acquisition.AcqedDataMsg.Timebase.TmbScale * Constants.VIS_XDIVS_NUM / 2 -
                                                readparam.StartTimeByus;

                    mainwinxstartpos = (Int32)((realstarttimebyus - uitheorystarttimebyus) * readparam.UPO_ResultColoumnNum / (Hd.UIMessage.Timebase.TmbScale * Constants.VIS_XDIVS_NUM));
                    if (mainwinxstartpos < 0)
                        mainwinxstartpos = 0;
                    mainwinxendpos = (Int32)(mainwinxstartpos + readparam.PerChannelRecvDotsCount / CurrDpxAcqParameters.UIMainWinExtractNum - 1);

                    if (mainwinxendpos > (readparam.UPO_ResultColoumnNum - 1))
                        mainwinxendpos = (Int32)(readparam.UPO_ResultColoumnNum - 1);
                }
                else
                {
                    mainwinxstartpos = 0;
                    mainwinxendpos = (Int32)ColumnValidCount-1;
                }
                channelinfousemessage = bUseOldData ? Acquisition.AcqedDataMsg! : Hd.UIMessage!;
                //拆分两级抽取数
                //UInt32 totalExtractNum = (uint)(CurrDpxAcqParameters!.UIMainWinExtractNum * CurrDpxAcqParameters!.DdrReadParam!.TotalExtractNum);
                (UInt32 L1ExtractNum, UInt32 L2ExtractNum) l1l2extractnum = (readparam.UPO_Level1ExtractNum,readparam.UPO_Level2ExtractNum) ;// SplitL1L2ExtractNum(channelInfoUseMessage.Timebase!.InterleaveMode, totalExtractNum);
                // 通用DDR读取设置
                readparam.TotalExtractNum = l1l2extractnum.L1ExtractNum;
                ConfigRead(readparam);

                #region Upo特殊的设置
                AcqedDataPool.DpxData.TimestampInitedAcq = DateTime.Now.Ticks;
                HdIO.WriteReg(ProcBdReg.W.UPO_UpoChPriority, (UInt32)channellevels);
                HdIO.WriteReg(ProcBdReg.W.UPO_UpoVectorEn, 1 - (uint)Hd.UIMessage.Display.DrawMode);
                HdIO.WriteReg(ProcBdReg.W.UPO_UpoPersistenceType, _PersistenceType[Hd.UIMessage.Display.Persist]);
                Int32 activedchannels = Hd.CurrProduct.Acquirer_AnalogChannel.AcquingParameters.CurrChBWModeAndActiveState & 0xff;
                HdIO.WriteReg(ProcBdReg.W.UPO_UpoOpenedCh, (UInt32)activedchannels);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Upo_WinMode, 0);//Message没有传下来
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Upo_XYMode_Source, 0);//Message没有传下来
                HdIO.WriteReg(ProcBdReg.W.UPO_UpoStartMwin, (UInt32)mainwinxstartpos);
                HdIO.WriteReg(ProcBdReg.W.UPO_UpoEndMwin, (UInt32)mainwinxendpos);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Upo_SubWinXStartPos, 0);////运行态，永远是0
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Upo_SubWinXEndPos, 0);//运行态，永远是Constants.UPO_WIDTH

                //UPO抽取倍数，是指插值之后
                HdIO.WriteReg(ProcBdReg.W.UPO_UpoBaseMwinL, (UInt32)(l1l2extractnum.L2ExtractNum & 0xffff));
                HdIO.WriteReg(ProcBdReg.W.UPO_UpoBaseMwinH, (UInt32)((l1l2extractnum.L2ExtractNum >> 16) & 0xffff));

                #endregion
                _PicDebugString = $"bReadOldData={Acquisition.bReadOldData},FrameNo={curracquireattribute.FrameNo},PerDataByps_AtDdr={curracquireattribute.PerDataByfs_AtDdr / 1e-3}ps,UI_XScale_Byus={Hd.UIMessage.Timebase.TmbScale}us,Data_XScale_Byus={curracquireattribute.HdMessage.Timebase.TmbScale}us, L1L2ExtractNum=[{l1l2extractnum.L1ExtractNum},{l1l2extractnum.L2ExtractNum}],StoreLen={curracquireattribute.HardwareStorageWaveDotsCnt},Offset={readparam.DdrReadStartDotPosition}";
            }
            isfast &= bInterpolateNumGT100 == false;
        }

        internal override void InitAcq()
        {
            InitGenerateParams(bUseOldData: false);
        }
        List<Byte[]> _HistoryUpo = new List<Byte[]>();
        List<String> _HistoryUpoInfo = new List<String>();
        List<DateTime> _HistoryCreateTime = new List<DateTime>();
        List<UInt16> _HistoryReadbackFrameNo = new List<UInt16>();
        List<Boolean> _HistorybReadUPOAtStop = new List<Boolean>();
        Boolean _IsTraceUpoData = false;
        Boolean _IsSaveTraceUpoData = false;
        private String _PicDebugString = "";
        private Boolean _RedrawFlag = false;
        
        internal override Boolean ReadAcqData(List<ReadInfo> readInfoList, out Double samplingRateByus, CancellationToken? softResetToken)
        {
            samplingRateByus = 1.0;

            //是否读老数据
            if(Acquisition.bReadOldData)
            {
                if(IsParamNotChangedAtStop())
                    return true;
                else
                {
                    InitGenerateParams(true);
                    //重绘，则画图拉低
                    HdIO.WriteReg(ProcBdReg.W.UPO_UpoPlotStart, 1);
                    _RedrawFlag = true;

                    //temp 等待full
                    Int32 waitTimeMax = 1000;
                    while (!AbstractController_Misc.AcqIsFulled())
                    {
                        Thread.Sleep(1);
                        waitTimeMax--;
                        if (waitTimeMax == 0)
                            break;
                    }

                    //如果重绘，则画图拉低
                    if (_RedrawFlag)
                        HdIO.WriteReg(ProcBdReg.W.UPO_UpoPlotStart, 0);
                }
            }

            if (Acquisition.AcqedDataMsg!.Timebase!.IsScan && (!Hd.UIMessage.bAcquireStopped))
                return false;
            Boolean isfast = (Hd.UIMessage?.Display?.IsFast ?? false);

            AcquireAttribute? acquireattribute = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquedParameters;
            if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen || acquireattribute == null)
                return true;
            if (isfast && bValid)
            {
                UInt32 dpxtotalbytes = (UInt32)(Constants.UPO_HEIGHT * Constants.UPO_WIDTH);
                Hd.CurrProduct!.AcqBd!.SwitchDataPathAndPcieReset_SetDataLength(DMAReadDataTypes.Dpx, dpxtotalbytes);

                AcqedDataPool.DpxData.DMAData = new Byte[dpxtotalbytes];
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);
                //Thread.Sleep(5000);
                bDataVaild = HdIO.DMARead(dpxtotalbytes, ref AcqedDataPool.DpxData.DMAData);
                if(bDataVaild)
                {
                    _LastAcqMsg = Hd.UIMessage;
                }

                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                ReadedDataColumnValidCount = ColumnValidCount;

                //other
                if (Hd.CurrDebugVarints.bEnable_SaveUpoPictureAtDriver)
                {
                    try
                    {
                        Bitmap bit = new Bitmap(Constants.UPO_WIDTH, Constants.UPO_HEIGHT);
                        for (Int32 rowindex = 0; rowindex < Constants.UPO_HEIGHT; rowindex++)
                        {
                            for (Int32 colindex = 0; colindex < Constants.UPO_WIDTH; colindex++)
                            {
                                bit.SetPixel(colindex, rowindex, Color.FromArgb(0, AcqedDataPool.DpxData.DMAData[rowindex * Constants.UPO_WIDTH + colindex], 0));
                            }
                        }
                        bit.Save($"AtRead_{AtReadPicIndex}.bmp");
                        AtReadPicIndex++;
                    }
                    catch(Exception ex)
                    { 
                    }
                }
                if (_IsTraceUpoData)
                {
                    if (_HistoryUpo.Count < 500)
                    {
                        _HistoryUpo.Add(AcqedDataPool.DpxData.DMAData);
                        _HistoryUpoInfo.Add(_PicDebugString);
                        _HistoryCreateTime.Add(Acquisition.CurrDataAcquireAttribute.CreateDateTime);
                        ushort frameno = 0;//(ushort)Hd.CurrProduct.AcqBd!.ReadReg(AcqBdReg.R.LSCtrl_LockedFrameNo, AcqBdNo.B0);
                        _HistoryReadbackFrameNo.Add(frameno);
                        _HistorybReadUPOAtStop.Add(Acquisition.IsReadUPOAtStop);
                    }
                    else
                    {
                        _IsTraceUpoData = false;
                        _IsSaveTraceUpoData = true;
                    }
                }
                if (_IsSaveTraceUpoData)
                {

                    for (Int32 fileIndex = 0; fileIndex < _HistoryUpo.Count; fileIndex++)
                    {
                        Byte[] upo = _HistoryUpo[fileIndex];
                        Bitmap bit = new Bitmap(Constants.UPO_WIDTH, Constants.UPO_HEIGHT);
                        Graphics graphics = Graphics.FromImage(bit);

                        for (Int32 rowIndex = 0; rowIndex < Constants.UPO_HEIGHT; rowIndex++)
                        {
                            for (Int32 colIndex = 0; colIndex < Constants.UPO_WIDTH; colIndex++)
                            {
                                bit.SetPixel(colIndex, rowIndex, Color.FromArgb(0, upo[rowIndex * Constants.UPO_WIDTH + colIndex], 0));
                            }
                        }
                        graphics.DrawString(_HistoryUpoInfo[fileIndex], new Font("Arial", 10.5f), new SolidBrush(Color.Red), new Point(10, 10));
                        graphics.DrawString($"CreateDateTime={_HistoryCreateTime[fileIndex]}", new Font("Arial", 10.5f), new SolidBrush(Color.Red), new Point(10, 30));
                        graphics.DrawString($"ReadbackFrameNo={_HistoryReadbackFrameNo[fileIndex]}", new Font("Arial", 10.5f), new SolidBrush(Color.Red), new Point(10, 50));
                        graphics.DrawString($"historybReadUPOAtStop={_HistorybReadUPOAtStop[fileIndex]}", new Font("Arial", 10.5f), new SolidBrush(Color.Red), new Point(10, 70));
                        bit.Save($@"C:\JiHe_MSO7000X\pic\AtRead_{fileIndex.ToString().PadLeft(3, '0')}.bmp");
                    }
                    _IsSaveTraceUpoData = false;
                    _HistoryUpo.Clear();
                    _HistoryUpoInfo.Clear();
                    _HistoryCreateTime.Clear();
                    _HistoryReadbackFrameNo.Clear();
                    _HistorybReadUPOAtStop.Clear();
                    _IsTraceUpoData = false;
                }
                return true;
            }
            bDataVaild = false;
            return false;
        }

        /// <summary>
        /// 配置DDR的读参数和使能
        /// </summary>
        /// <param name="readParams"></param>
        internal void ConfigRead(ReadParams readParams)
        {
            var define = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave()!;

            #region 插值配置下发
            if (readParams.Interpolate_Num_Double > 1)
            {
                ConditionManager.InterpEn = true;
                RegSendManager.Default.Send((UInt32)AcqBdReg.W.Interpolate_EnableAcq);
                RegSendManager.Default.Send((UInt32)ProcBdReg.W.Interpolate_InterpEnPro);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Interpolate_Ratio, Interp_JiHe_MSO8000X.GetValideValue((Int32)readParams.Interpolate_Num_Double));//4Byte,最低Byte最多能发A(10倍率，只能2，5，A)，2位为0，34位最多20倍率（2，5，10，20）；
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Interpolate_RemaimderNun, (UInt32)readParams.Interpolate_DiscardDotNum);
                HdIO.WriteReg(ProcBdReg.W.Interpolate_Ratio, Interp_JiHe_MSO8000X.GetValideValue((Int32)readParams.Interpolate_Num_Double));//4Byte,最低Byte最多能发A(10倍率，只能2，5，A)，2位为0，34位最多20倍率（2，5，10，20）；
            }
            else
            {
                ConditionManager.InterpEn = false;
                RegSendManager.Default.Send((UInt32)AcqBdReg.W.Interpolate_EnableAcq);
                RegSendManager.Default.Send((UInt32)ProcBdReg.W.Interpolate_InterpEnPro);
            }
            #endregion 插值配置下发

            #region 后抽系数下发
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PosGapValuelL16, readParams.TotalExtractNum & 0xffff);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PosGapValueH16, (readParams.TotalExtractNum >> 16) & 0xffff);

            HdIO.WriteReg(ProcBdReg.W.Decimation_PosGapValuelL16, readParams.TotalExtractNum & 0xffff); //处理板后抽 zwj
            HdIO.WriteReg(ProcBdReg.W.Decimation_PosGapValueH16, (readParams.TotalExtractNum >> 16) & 0xffff); //处理板后抽 zwj
            #endregion

            //触发
            UInt32 offset = (UInt32)(readParams.DdrReadStartDotPosition);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_Offset_L, AcqBdReg.W.LSCtrl_Offset_H, offset);
            return;
        }

        /// <summary>
        /// 根据系统的设置参数而进行的配置
        /// </summary>
        internal static void ourConfig()
        {
            bool isfast = (Hd.UIMessage?.Display?.IsFast ?? false);
            HdIO.WriteReg(ProcBdReg.W.UPO_ProUpoEn, isfast ? 1U : 0);//打开UPO
        }
    }
}
