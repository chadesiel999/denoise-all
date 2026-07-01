using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    public class AbstractAcquirer_DPX : AbstractAcquirer
    {
        internal override AcqDataType DataType { get => AcqDataType.DPX; }
        internal DpxAcqParameters? CurrDpxAcqParameters = null;
        protected static Action? ConfigFunc = null;
        internal static void Config() { ConfigFunc?.Invoke(); }
        protected List<ChannelId> IncludeChannels = new List<ChannelId>();
        protected byte[] DMA_Data = new byte[Constants.UPO_HEIGHT * Constants.UPO_WIDTH];
        protected UInt32 _MainWinMaxHitTimes;
        protected UInt32 _MainWinMinHitTimes;
        protected UInt32 _SubWinMaxHitTimes;
        protected UInt32 _SubWinMinHitTimes;
        protected double _MainWinRadioOfSoftWaveSampleDivDpxWaveSample;
        protected double _SubWinRadioOfSoftWaveSampleDivDpxWaveSample;
        internal override void Init()
        {

        }
        internal override void InitAcq()
        {

        }
        internal virtual void InitGenerateParams(bool bUseOldData)
        {

        }
        protected Boolean bParamNotChangedAtStop = true;
        internal Boolean bInterpolateNumGT100 = false;
        internal UInt32 ColumnValidCount = 1250;
        protected int AtReadPicIndex = 0;
        protected int AtBackPicIndex = 0;
        internal Boolean bValid = false;
        internal virtual void ClearParamChangedAtStop() { bParamNotChangedAtStop = true; }
        internal virtual Boolean IsParamNotChangedAtStop() => true;

        internal override void PostProcess(List<ReadInfo> readInfoList, CancellationToken? softResetToken)
        {

        }
        internal UInt32 ReadedDataColumnValidCount = 1250;
        internal override bool ReadAcqData(List<ReadInfo> readInfoList, out double samplingRateByus, CancellationToken? softResetToken)
        {
            samplingRateByus = 1.0;
            if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen)
            {
                return true;
            }
            return false;
        }
        Bitmap? tempBitmap = null;
        /// <summary>
        /// 供Core调用的对外接口函数
        /// </summary>
        /// <param name="outputData">dpx概率数据，[宽,高]</param>
        /// <param name="channelID">物理通道编号</param>
        /// <param name="FrameCount">本次映射的帧数。FrameCount>=MaxHitCount.用作归一化处理。 </param>
        /// <param name="MaxHitCount">本次映射的最大灰度值。MaxHitCount<=FrameCount。用作归一化处理</param>
        /// <returns></returns>
        public virtual bool TryTakeWave(out Byte[]? outputData,out WfmSampleInfo wfmSampleInfo,out List<ChannelId> includeChannels,out UInt32 MainWinMaxHitTimes, out UInt32 MainWinMinHitTimes, out UInt32 SubWinMaxHitTimes, out UInt32 SubWinMinHitTimes, out double MainWinRadioOfSoftWaveSampleDivDpxWaveSample, out double SubWinRadioOfSoftWaveSampleDivDpxWaveSample)
        {
            wfmSampleInfo = new WfmSampleInfo();
            wfmSampleInfo.HdMessage = Acquisition.AcqedDataMsg! with { };
            MainWinMaxHitTimes = 255;
            MainWinMinHitTimes = 1;
            SubWinMaxHitTimes = 255;
            SubWinMinHitTimes = 1;
            MainWinRadioOfSoftWaveSampleDivDpxWaveSample = ReadedDataColumnValidCount / 1250.0;
            SubWinRadioOfSoftWaveSampleDivDpxWaveSample = 1.0;
            if (!(Hd.UIMessage!.Display?.IsFast ?? false) || (Hd.CurrProduct?.Acquirer_DPX?.bInterpolateNumGT100 ?? false) || (!bValid) || (!AcqFulled))
            {
                includeChannels=new List<ChannelId>();
                outputData = new byte[0];
            }
            else
            {
                outputData = AcqedDataPool.DpxData.DMAData;
                if (Hd.CurrDebugVarints.bEnable_SaveUpoPictureAtDriver)
                {
                    Bitmap bit = new Bitmap(1250, 240);
                    for (int rowIndex = 0; rowIndex < 240; rowIndex++)
                    {
                        for (int colIndex = 0; colIndex < 1250; colIndex++)
                        {
                            bit.SetPixel(colIndex, rowIndex, Color.FromArgb(0, AcqedDataPool.DpxData.DMAData[rowIndex * 1250 + colIndex], 0));
                        }
                    }
                    bit.Save($@"C:\JiHe_MSO7000X\pic\AtBack_{AtBackPicIndex}.bmp");
                    AtBackPicIndex++;
                }
                if (Hd.CurrDebugVarints.bEnable_SaveUpoPictureAtDriver)
                {
                    if (tempBitmap == null)
                        tempBitmap = new Bitmap(1250, 240);
                    for (int rowIndex = 0; rowIndex < 240; rowIndex++)
                    {
                        for (int colIndex = 0; colIndex < 1250; colIndex++)
                        {
                            if (AcqedDataPool.DpxData.DMAData[rowIndex * 1250 + colIndex] != 0)
                                tempBitmap.SetPixel(colIndex, rowIndex, Color.FromArgb(0, AcqedDataPool.DpxData.DMAData[rowIndex * 1250 + colIndex], 0));
                        }
                    }
                    tempBitmap.Save($@"C:\JiHe_MSO7000X\pic\778899.bmp");
                }
                includeChannels = IncludeChannels;
            }
            return true;
        }
    }
}
