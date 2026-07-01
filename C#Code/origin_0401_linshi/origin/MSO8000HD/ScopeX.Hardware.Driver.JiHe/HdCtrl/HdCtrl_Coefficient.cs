using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    //#if Coefficient
    internal static class HdCtrl_Coefficient
    {
        #region AcqBd
        internal static void Send2AcqBoardByRegister(UInt32 dataType, AcqBdNo acqBdNo, Int32[] dataArray, Int32 dataCount)
        {
            //同一个参数可以发向不同的FPGA，如插值系数，与通道无关
            //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorSelect, acqBdNo, 0);//数据类型
            //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorSelect, acqBdNo, dataType);//数据类型
            //for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
            //{
            //    //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
            //    UInt32 data = (UInt32)dataArray[dataIndex];
            //    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWen, acqBdNo, 0);
            //    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWa, acqBdNo, (UInt32)dataIndex);
            //    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWdLow, acqBdNo, data & 0xffff);
            //    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWdHigh, acqBdNo, (data >> 16) & 0xffff);
            //    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWen, acqBdNo, 1);
            //}
            //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWen, acqBdNo, 0);

            //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorSelect, acqBdNo, 0);//数据类型
            if (dataType == 0x02)
            {
                ;
            }
            //同一个参数可以发向不同的FPGA，如插值系数，与通道无关
            switch (acqBdNo)//Hd.currProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorSelect, acqBdNo, 0);//数据类型
            {
                case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh1, 0); break;
                case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh2, 0); break;
                case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh3, 0); break;
                case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh4, 0); break;
            }
            switch (acqBdNo)//Hd.currProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorSelect, acqBdNo, dataType);//数据类型
            {
                case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh1, dataType); break;
                case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh2, dataType); break;
                case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh3, dataType); break;
                case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh4, dataType); break;
            }

            for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
            {
                UInt32 data = (UInt32)dataArray[dataIndex];
                if ((UInt32)dataIndex > 490 && (UInt32)dataIndex < 500)
                {
                    ;
                }
                switch (acqBdNo)//Hd.currProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWen, acqBdNo, 0);
                {
                    case AcqBdNo.B0:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh1, 0);
                        break;
                    case AcqBdNo.B1:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh2, 0);
                        break;
                    case AcqBdNo.B2:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh3, 0);
                        break;
                    case AcqBdNo.B3:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh4, 0);
                        break;
                }
                switch (acqBdNo)//Hd.currProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWa, acqBdNo, (UInt32)dataIndex);
                {
                    case AcqBdNo.B0:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh1, (UInt32)dataIndex);
                        break;
                    case AcqBdNo.B1:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh2, (UInt32)dataIndex);
                        break;
                    case AcqBdNo.B2:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh3, (UInt32)dataIndex);
                        break;
                    case AcqBdNo.B3:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh4, (UInt32)dataIndex);
                        break;
                }
                switch (acqBdNo)//Hd.currProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWdLow, acqBdNo, data & 0xffff);
                {
                    case AcqBdNo.B0:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh1, data & 0xffff);
                        break;
                    case AcqBdNo.B1:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh2, data & 0xffff);
                        break;
                    case AcqBdNo.B2:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh3, data & 0xffff);
                        break;
                    case AcqBdNo.B3:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh4, data & 0xffff);
                        break;
                }
                switch (acqBdNo)//Hd.currProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWdHigh, acqBdNo, (data >> 16) & 0xffff);
                {
                    case AcqBdNo.B0:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh1, (data >> 16) & 0xffff);
                        break;
                    case AcqBdNo.B1:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh2, (data >> 16) & 0xffff);
                        break;
                    case AcqBdNo.B2:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh3, (data >> 16) & 0xffff);
                        break;
                    case AcqBdNo.B3:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh4, (data >> 16) & 0xffff);
                        break;
                }
                switch (acqBdNo)//Hd.currProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWen, acqBdNo, 1);
                {
                    case AcqBdNo.B0:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh1, 1);
                        break;
                    case AcqBdNo.B1:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh2, 1);
                        break;
                    case AcqBdNo.B2:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh3, 1);
                        break;
                    case AcqBdNo.B3:
                        HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh4, 1);
                        break;
                }
            }
            switch (acqBdNo)//Hd.currProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWen, acqBdNo, 0);
            {
                case AcqBdNo.B0:
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh1, 0);
                    break;
                case AcqBdNo.B1:
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh2, 0);
                    break;
                case AcqBdNo.B2:
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh3, 0);
                    break;
                case AcqBdNo.B3:
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh4, 0);
                    break;
            }
            switch (acqBdNo)//Hd.currProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorSelect, acqBdNo, 0);//数据类型
            {
                case AcqBdNo.B0:
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh1, 0);
                    break;
                case AcqBdNo.B1:
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh2, 0);
                    break;
                case AcqBdNo.B2:
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh3, 0);
                    break;
                case AcqBdNo.B3:
                    HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh4, 0);
                    break;
            }
        }//????
        internal static void Send2ProcBoardByRegister(UInt32 dataType, Int32[] dataArray, Int32 dataCount, ChannelId chnlId)
        {
            HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, dataType);
            for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
            {
                UInt32 data = (UInt32)dataArray[dataIndex];
                HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWen, 0);
                HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWa, (UInt32)dataIndex);
                HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWdLow, (UInt32)data & 0xffff);
                HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWdHigh, (UInt32)(data >> 16) & 0xffff);
                HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWen, 1);
            }
            HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWen, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, 0);
        }//????
        internal static void SendMultiRadioInterpolationByRegister(UInt32 dataType, Int32[] dataArray, Int32 dataCount, ChannelId chnlId)
        {
            //HdIO.WriteReg(ProcBdReg.W.DBI_DBI_pro_fifo_depth_in, 1500);//12288

            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWen, 0);
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiResetDsp, 0);
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiResetDsp, 1);
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiResetDsp, 0);
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiInterpRate, 0x0105);
            for (int i = 0; i < dataArray.Length; i++)
            {
                //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWen, 0);
                UInt32 addr = 0;
                if (i < 500)
                    addr = (UInt32)i;
                else
                    addr = (UInt32)(0x0800) | (uint)(i - 500);

                //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWa, (UInt32)addr);

                Int32 data = dataArray[i];
                //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWdLow, (UInt32)data & 0xffff);
                //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWdHigh, (UInt32)(data >> 16) & 0xffff);

                //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWen, 1);
            }
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWen, 0);
        }//????
        internal static Boolean Send2AcqBoardByDMA(UInt32 dataType, AcqBdNo acqBdNo, Byte[] dmaWriteData)
        {
            //HdIO.WriteReg(S6BdReg.W.DBI_DBI_FACTOR_SELSECT_S6, (UInt32)((1 + (int)acqBdNo)));

            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_DMA_RST_ACQ, acqBdNo, 0);
            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_DMA_RST_ACQ, acqBdNo, 1);
            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_DMA_RST_ACQ, acqBdNo, 0);

            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_DBI_FACTOR_SELSECT_ACQ, acqBdNo, dataType);
            HdIO.DelayByUs(500);
            Boolean writeok = HdIO.DMAWrite(0, dmaWriteData, (UInt32)dmaWriteData.Length);
            HdIO.DelayByUs(500);
            return writeok;
            //return false;
        }//????

        internal static void SendTiadcByRegister(UInt32 dataType, AcqBdNo acqBdNo, Int32[] dataArray, Int32 dataCount)
        {
            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.TIADC_Enable, acqBdNo, 0);

            for (int dataIndex = 0; dataIndex < dataCount / 2; dataIndex++)
            {
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.TIADC_WriteEnable, acqBdNo, 0);
                //地址
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.TIADC_WriteAddress, acqBdNo, (UInt32)dataIndex);

                //实部
                //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex * 2, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                UInt32 data = (UInt32)dataArray[dataIndex * 2]; //

                //data = 4096;
                Hd.CurrProduct!.AcqBd!.WriteReg((UInt32)AcqBdReg.W.TIADC_FactorTableWriteData_r, acqBdNo, (UInt32)data & 0xffff);
                //虚部
                //data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex * 2 + 1, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                //data = 0;
                data = (UInt32)dataArray[dataIndex * 2 + 1]; //
                Hd.CurrProduct!.AcqBd!.WriteReg((UInt32)AcqBdReg.W.TIADC_FactorTableWriteData_i, acqBdNo, (UInt32)data & 0xffff);
                HdIO.DelayByUs(10);
                Hd.CurrProduct!.AcqBd!.WriteReg((UInt32)AcqBdReg.W.TIADC_WriteEnable, acqBdNo, 1);
            }
            Hd.CurrProduct!.AcqBd!.WriteReg((UInt32)AcqBdReg.W.TIADC_WriteEnable, acqBdNo, 0);

            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.TIADC_Enable, acqBdNo, Hd.CurrDebugVarints.bEnable_CorrectTiAdc ? 1U : 0);
        }
        #endregion


        internal static void SendMultiRadioInterpolationByRegister(UInt32 dataType, Int32[] dataArray, Int32 dataCount)
        {
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWen, 0);
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiResetDsp, 0);
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiResetDsp, 1);
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiResetDsp, 0);
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiInterpRate, 0x0105);
            for (int i = 0; i < dataArray.Length; i++)
            {
                //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWen, 0);
                UInt32 addr = 0;
                if (i < 500)
                    addr = (UInt32)i;
                else
                    addr = (UInt32)(0x0800) | (uint)(i - 500);

                //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWa, (UInt32)addr);

                Int32 data = dataArray[i];
                //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWdLow, (UInt32)data & 0xffff);
                //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWdHigh, (UInt32)(data >> 16) & 0xffff);

                //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWen, 1);
            }
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWen, 0);
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiInterEn, Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation ? 1U : 0);
        }//????

        internal static void Send2ProcBoardByRegister(UInt32 dataType, Int32[] dataArray, Int32 dataCount)
        {
            HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, dataType);
            for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
            {
                UInt32 data = (UInt32)dataArray[dataIndex];
                HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWen, 0);
                HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWa, (UInt32)dataIndex);
                HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWdLow, (UInt32)data & 0xffff);
                HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWdHigh, (UInt32)(data >> 16) & 0xffff);
                HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWen, 1);
            }
            HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWen, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, 0);
        }
        internal static void SendPFCByRegister(UInt32 dataType, Int32[] dataArray, Int32 dataCount, ChannelId chnlId)
        {
            //HdIO.WriteReg(ProcBdReg.W.DBI_PfcCaliEn, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, dataType);
            HdIO.WriteReg(ProcBdReg.W.DBI_DBI_FACTOR_SELSECT_PRO, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_DBI_FACTOR_SELSECT_PRO, 0x1u << (Int32)chnlId);
            for (int dataIndex = 0; dataIndex < dataCount / 2; dataIndex++)
            {
                HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWen, 0);
                //地址
                HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWa, (UInt32)dataIndex);
                //实部
                UInt32 data = (UInt32)dataArray[dataIndex * 2]; //
                                                                //data = 4096;
                HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWdHigh, (UInt32)data & 0xffff);
                //虚部
                //data = 0;
                data = (UInt32)dataArray[dataIndex * 2 + 1]; //
                HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWdLow, (UInt32)data & 0xffff);

                HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWen, 1);
            }
            HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorWen, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, 0);
        }//????
        internal static Boolean Send2ProcBoardByDMA(UInt32 dataType, Byte[] dmaWriteData)
        {
            //HdIO.WriteReg(S6BdReg.W.DBI_DBI_FACTOR_SELSECT_S6, 0U);

            HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_PRO, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_PRO, 1U);
            HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_PRO, 0);
            HdIO.DelayByUs(500);
            HdIO.WriteReg(ProcBdReg.W.DBI_DBI_FACTOR_SELSECT_PRO, dataType);
            HdIO.DelayByUs(500);
            return HdIO.DMAWrite(0, dmaWriteData, (UInt32)dmaWriteData.Length);

            //return false;
        }//????

        internal static void SendProcTiadcByRegister(UInt32 dataType, Int32[] dataArray, Int32 dataCount, ChannelId chnlId)
        {
            HdIO.WriteReg(ProcBdReg.W.TIADC_Enable, 0);
            //HdIO.WriteReg(ProcBdReg.W.TIADC_TiSelect, 0x1u << (Int32)chnlId);//根据通道进行选择,独热码形式 chnl1》0b1；chnl2》0b10
            for (int dataIndex = 0; dataIndex < dataCount / 2; dataIndex++)
            {
                HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnable, 0);
                //地址
                HdIO.WriteReg(ProcBdReg.W.TIADC_WriteAddress, (UInt32)dataIndex);
                //实部
                //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex * 2, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                UInt32 data = (UInt32)dataArray[dataIndex * 2]; //

                //data = 4096;
                HdIO.WriteReg(ProcBdReg.W.TIADC_FactorTableWriteData_r, (UInt32)data & 0x3fff);
                //虚部
                //data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex * 2 + 1, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                //data = 0;
                data = (UInt32)dataArray[dataIndex * 2 + 1]; //
                HdIO.WriteReg(ProcBdReg.W.TIADC_FactorTableWriteData_i, (UInt32)data & 0x3fff);
                HdIO.DelayByUs(10);
                HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnable, (UInt32)chnlId);
            }
            HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnable, 1);
            HdIO.WriteReg(ProcBdReg.W.TIADC_Enable, 1);
            //HdIO.WriteReg(ProcBdReg.W.TIADC_Enable, Hd.CurrDebugVarints.bEnable_ProcCorrectTiAdc ? 1U : 0);
        }//????

        internal static void Send2AcqBoardByRegisterProcTi(UInt32 dataType, AcqBdNo acqBdNo, Int32[] dataArray, Int32 dataCount)
        {
            //同一个参数可以发向不同的FPGA，如插值系数，与通道无关
            switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorSelect, acqBdNo, 0);//数据类型
            {
                case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh1, dataType); break;
                case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh2, dataType); break;
                case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh3, dataType); break;
                case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh4, dataType); break;
            }
            //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorSelect, acqBdNo, dataType);//数据类型
            for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
            {
                UInt32 data = (UInt32)dataArray[dataIndex];
                switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWen, acqBdNo, 0);
                {
                    case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh1, 0); break;
                    case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh2, 0); break;
                    case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh3, 0); break;
                    case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh4, 0); break;
                }
                switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWa, acqBdNo, (UInt32)dataIndex);
                {
                    case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh1, (UInt32)dataIndex); break;
                    case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh2, (UInt32)dataIndex); break;
                    case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh3, (UInt32)dataIndex); break;
                    case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWaProCh4, (UInt32)dataIndex); break;
                }
                switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWdLow, acqBdNo, data & 0xffff);
                {
                    case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh1, data & 0xffff); break;
                    case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh2, data & 0xffff); break;
                    case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh3, data & 0xffff); break;
                    case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdLowProCh4, data & 0xffff); break;
                }
                switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWdHigh, acqBdNo, (data >> 16) & 0xffff);
                {
                    case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh1, (data >> 16) & 0xffff); break;
                    case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh2, (data >> 16) & 0xffff); break;
                    case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh3, (data >> 16) & 0xffff); break;
                    case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWdHighProCh4, (data >> 16) & 0xffff); break;
                }
                switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWen, acqBdNo, 1);
                {
                    case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh1, 1); break;
                    case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh2, 1); break;
                    case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh3, 1); break;
                    case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh4, 1); break;
                }
            }
            switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorWen, acqBdNo, 0);
            {
                case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh1, 0); break;
                case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh2, 0); break;
                case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh3, 0); break;
                case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorWenProCh4, 0); break;
            }
            switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_AcqFactorSelect, acqBdNo, 0);//数据类型
            {
                case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh1, 0); break;
                case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh2, 0); break;
                case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh3, 0); break;
                case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_AcqFactorSelectProCh4, 0); break;
            }

        }

        internal static Boolean Send2AcqBoardByDMAProcTi(UInt32 dataType, AcqBdNo acqBdNo, Byte[] dmaWriteData)
        {
            //HdIO.WriteReg(S6BdReg.W.DBI_DBI_FACTOR_SELSECT_S6, (UInt32)((1 + (int)acqBdNo)));

            switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_DMA_RST_ACQ, acqBdNo, 0);
            {
                case AcqBdNo.B11: HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_ACQProCh1, 0); break;
                case AcqBdNo.B12: HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_ACQProCh2, 0); break;
                case AcqBdNo.B7: HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_ACQProCh3, 0); break;
                case AcqBdNo.B10: HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_ACQProCh4, 0); break;
            }
            switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_DMA_RST_ACQ, acqBdNo, 1);
            {
                case AcqBdNo.B11: HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_ACQProCh1, 1); break;
                case AcqBdNo.B12: HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_ACQProCh2, 1); break;
                case AcqBdNo.B7: HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_ACQProCh3, 1); break;
                case AcqBdNo.B10: HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_ACQProCh4, 1); break;
            }
            switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_DMA_RST_ACQ, acqBdNo, 0);
            {
                case AcqBdNo.B11: HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_ACQProCh1, 0); break;
                case AcqBdNo.B12: HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_ACQProCh2, 0); break;
                case AcqBdNo.B7: HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_ACQProCh3, 0); break;
                case AcqBdNo.B10: HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_ACQProCh4, 0); break;
            }

            switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DBI_DBI_FACTOR_SELSECT_ACQ, acqBdNo, dataType);
            {
                case AcqBdNo.B11: HdIO.WriteReg(ProcBdReg.W.DBI_DBI_FACTOR_SELSECT_ACQProCh1, dataType); break;
                case AcqBdNo.B12: HdIO.WriteReg(ProcBdReg.W.DBI_DBI_FACTOR_SELSECT_ACQProCh2, dataType); break;
                case AcqBdNo.B7: HdIO.WriteReg(ProcBdReg.W.DBI_DBI_FACTOR_SELSECT_ACQProCh3, dataType); break;
                case AcqBdNo.B10: HdIO.WriteReg(ProcBdReg.W.DBI_DBI_FACTOR_SELSECT_ACQProCh4, dataType); break;
            }
            HdIO.DelayByUs(500);
            Boolean writeok = HdIO.DMAWrite(0, dmaWriteData, (UInt32)dmaWriteData.Length);
            HdIO.DelayByUs(500);
            return writeok;
            //return false;
        }//????

        internal static void SendTiadcByRegisterProcTi(AcqBdNo acqBdNo, Int32[] dataArray, Int32 dataCount)
        {
            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.TIADC_Enable, acqBdNo, 0);
            //switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.TIADC_Enable, acqBdNo, 0);
            //{
            //    case AcqBdNo.B11: HdIO.WriteReg(ProcBdReg.W.TIADC_EnableProCh1, 0); break;
            //    case AcqBdNo.B12: HdIO.WriteReg(ProcBdReg.W.TIADC_EnableProCh2, 0); break;
            //    case AcqBdNo.B7: HdIO.WriteReg(ProcBdReg.W.TIADC_EnableProCh3, 0); break;
            //    case AcqBdNo.B10: HdIO.WriteReg(ProcBdReg.W.TIADC_EnableProCh4, 0); break;
            //}

            for (int dataIndex = 0; dataIndex < dataCount / 2; dataIndex++)
            {
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.TIADC_WriteEnable, acqBdNo, 0);
                //switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.TIADC_WriteEnable, acqBdNo, 0);
                //{
                //    case AcqBdNo.B11: HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnableProCh1, 0); break;
                //    case AcqBdNo.B12: HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnableProCh2, 0); break;
                //    case AcqBdNo.B7: HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnableProCh3, 0); break;
                //    case AcqBdNo.B10: HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnableProCh4, 0); break;
                //}

                //地址
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.TIADC_WriteAddress, acqBdNo, (UInt32)dataIndex);
                //switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.TIADC_WriteAddress, acqBdNo, (UInt32)dataIndex);
                //{
                //    case AcqBdNo.B11: HdIO.WriteReg(ProcBdReg.W.TIADC_WriteAddressProCh1, (UInt32)dataIndex); break;
                //    case AcqBdNo.B12: HdIO.WriteReg(ProcBdReg.W.TIADC_WriteAddressProCh2, (UInt32)dataIndex); break;
                //    case AcqBdNo.B7: HdIO.WriteReg(ProcBdReg.W.TIADC_WriteAddressProCh3, (UInt32)dataIndex); break;
                //    case AcqBdNo.B10: HdIO.WriteReg(ProcBdReg.W.TIADC_WriteAddressProCh4, (UInt32)dataIndex); break;
                //}

                //实部
                //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex * 2, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                UInt32 data = (UInt32)dataArray[dataIndex * 2]; //

                //  data = 0;

                Hd.CurrProduct!.AcqBd!.WriteReg((UInt32)AcqBdReg.W.TIADC_FactorTableWriteData_r, acqBdNo, (UInt32)data & 0xffff);
                //switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg((UInt32)AcqBdReg.W.TIADC_FactorTableWriteData_r, acqBdNo, (UInt32)data & 0xffff);
                //{
                //    case AcqBdNo.B11: HdIO.WriteReg(ProcBdReg.W.TIADC_FactorTableWriteData_rProCh1, (UInt32)data & 0xffff); break;
                //    case AcqBdNo.B12: HdIO.WriteReg(ProcBdReg.W.TIADC_FactorTableWriteData_rProCh2, (UInt32)data & 0xffff); break;
                //    case AcqBdNo.B7: HdIO.WriteReg(ProcBdReg.W.TIADC_FactorTableWriteData_rProCh3, (UInt32)data & 0xffff); break;
                //    case AcqBdNo.B10: HdIO.WriteReg(ProcBdReg.W.TIADC_FactorTableWriteData_rProCh4, (UInt32)data & 0xffff); break;
                //}

                //虚部
                //data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex * 2 + 1, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                //  data = 0;
                data = (UInt32)dataArray[dataIndex * 2 + 1]; //

                Hd.CurrProduct!.AcqBd!.WriteReg((UInt32)AcqBdReg.W.TIADC_FactorTableWriteData_i, acqBdNo, (UInt32)data & 0xffff);
                //switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg((UInt32)AcqBdReg.W.TIADC_FactorTableWriteData_i, acqBdNo, (UInt32)data & 0xffff);
                //{
                //    case AcqBdNo.B11: HdIO.WriteReg(ProcBdReg.W.TIADC_FactorTableWriteData_iProCh1, (UInt32)data & 0xffff); break;
                //    case AcqBdNo.B12: HdIO.WriteReg(ProcBdReg.W.TIADC_FactorTableWriteData_iProCh2, (UInt32)data & 0xffff); break;
                //    case AcqBdNo.B7: HdIO.WriteReg(ProcBdReg.W.TIADC_FactorTableWriteData_iProCh3, (UInt32)data & 0xffff); break;
                //    case AcqBdNo.B10: HdIO.WriteReg(ProcBdReg.W.TIADC_FactorTableWriteData_iProCh4, (UInt32)data & 0xffff); break;
                //}

                HdIO.DelayByUs(10);
                Hd.CurrProduct!.AcqBd!.WriteReg((UInt32)AcqBdReg.W.TIADC_WriteEnable, acqBdNo, 1);
                //switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg((UInt32)AcqBdReg.W.TIADC_WriteEnable, acqBdNo, 1);
                //{
                //    case AcqBdNo.B11: HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnableProCh1, 1); break;
                //    case AcqBdNo.B12: HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnableProCh2, 1); break;
                //    case AcqBdNo.B7: HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnableProCh3, 1); break;
                //    case AcqBdNo.B10: HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnableProCh4, 1); break;
                //}
            }
            Hd.CurrProduct!.AcqBd!.WriteReg((UInt32)AcqBdReg.W.TIADC_WriteEnable, acqBdNo, 0);
            //switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg((UInt32)AcqBdReg.W.TIADC_WriteEnable, acqBdNo, 0);
            //{
            //    case AcqBdNo.B11: HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnableProCh1, 0); break;
            //    case AcqBdNo.B12: HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnableProCh2, 0); break;
            //    case AcqBdNo.B7: HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnableProCh3, 0); break;
            //    case AcqBdNo.B10: HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnableProCh4, 0); break;
            //}

            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.TIADC_Enable, acqBdNo, Hd.CurrDebugVarints.bEnable_CorrectTiAdc ? 1U : 0);
            //switch (acqBdNo)//Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.TIADC_Enable, acqBdNo, Hd.CurrDebugVarints[DebugBooleanEnum.bEnable_CorrectTiAdc] ? 1U : 0);
            //{
            //    case AcqBdNo.B11: HdIO.WriteReg(ProcBdReg.W.TIADC_EnableProCh1, Hd.CurrDebugVarints.bEnable_CorrectTiAdc ? 1U : 0); break;
            //    case AcqBdNo.B12: HdIO.WriteReg(ProcBdReg.W.TIADC_EnableProCh2, Hd.CurrDebugVarints.bEnable_CorrectTiAdc ? 1U : 0); break;
            //    case AcqBdNo.B7: HdIO.WriteReg(ProcBdReg.W.TIADC_EnableProCh3, Hd.CurrDebugVarints.bEnable_CorrectTiAdc ? 1U : 0); break;
            //    case AcqBdNo.B10: HdIO.WriteReg(ProcBdReg.W.TIADC_EnableProCh4, Hd.CurrDebugVarints.bEnable_CorrectTiAdc ? 1U : 0); break;
            //}
        }//????

        internal static void SendMultiRadioInterpolationByRegisterProcTi(Int32[] dataArray, Int32 dataCount, ChannelId chnlId)
        {
            //HdIO.WriteReg(ProcBdReg.W.DBI_DBI_pro_fifo_depth_in, 1500);//12288

            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWen, 0);
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiResetDsp, 0);
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiResetDsp, 1);
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiResetDsp, 0);
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiInterpRate, 0x0105);
            for (int i = 0; i < dataArray.Length; i++)
            {
                //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWen, 0);
                UInt32 addr = 0;
                if (i < 500)
                    addr = (UInt32)i;
                else
                    addr = (UInt32)(0x0800) | (uint)(i - 500);

                //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWa, (UInt32)addr);

                Int32 data = dataArray[i];
                //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWdLow, (UInt32)data & 0xffff);
                //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWdHigh, (UInt32)(data >> 16) & 0xffff);

                //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWen, 1);
            }
            //HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiFactorWen, 0);
        }//????

        internal static void SendAFCByRegisterProcTi(Int32[] dataArray, Int32 dataCount, ChannelId chnlId)
        {
            //HdIO.WriteReg(ProcBdReg.W.Fir_FirSelect, 0x1u << (Int32)chnlId);
            for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
            {
                UInt32 data = (UInt32)dataArray[dataIndex];
                //HdIO.WriteReg(ProcBdReg.W.Fir_FactorTableWriteEnable, 0);
                //HdIO.WriteReg(ProcBdReg.W.Fir_FactorTableWriteAddr, (UInt32)dataIndex);
                //HdIO.WriteReg(ProcBdReg.W.Fir_FactorTableWriteData_L, (UInt32)data & 0xffff);
                //HdIO.WriteReg(ProcBdReg.W.Fir_FactorTableWriteData_H, (UInt32)(data >> 16) & 0xffff);
                //HdIO.WriteReg(ProcBdReg.W.Fir_FactorTableWriteEnable, 1);
            }
            //HdIO.WriteReg(ProcBdReg.W.Fir_FactorTableWriteEnable, 0);
            //HdIO.WriteReg(ProcBdReg.W.Fir_Enable, 1);

        }//????
        internal static void SendPFCByRegisterProcTi(Int32[] dataArray, Int32 dataCount, ChannelId chnlId)
        {
            //HdIO.WriteReg(ProcBdReg.W.IIRfilter_IIRSelect, 0x1u << (Int32)chnlId);
            for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
            {
                //HdIO.WriteReg(ProcBdReg.W.IIRfilter_iir_factor_wen, 0);
                //地址
                //HdIO.WriteReg(ProcBdReg.W.IIRfilter_iir_factor_addr, (UInt32)dataIndex);

                UInt32 data = (UInt32)dataArray[dataIndex];

                //HdIO.WriteReg(ProcBdReg.W.IIRfilter_iir_factor_data_L16, (UInt32)data & 0xffff);

                //HdIO.WriteReg(ProcBdReg.W.IIRfilter_iir_factor_data_H16, (UInt32)data >> 16 & 0xffff);

                //HdIO.WriteReg(ProcBdReg.W.IIRfilter_iir_factor_wen, 1);
            }
            //HdIO.WriteReg(ProcBdReg.W.IIRfilter_iir_factor_wen, 0);
            //HdIO.WriteReg(ProcBdReg.W.IIRfilter_iir_filter_en, 1);
        }//????
        internal static Boolean Send2ProcBoardByDMAProcTi(UInt32 dataType, Byte[] dmaWriteData)
        {
            //HdIO.WriteReg(S6BdReg.W.DBI_DBI_FACTOR_SELSECT_S6, 0U);

            HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_PRO, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_PRO, 1U);
            HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_PRO, 0);
            HdIO.DelayByUs(500);
            HdIO.WriteReg(ProcBdReg.W.DBI_DBI_FACTOR_SELSECT_PRO, dataType);
            HdIO.DelayByUs(500);
            return HdIO.DMAWrite(0, dmaWriteData, (UInt32)dmaWriteData.Length);
            //return new();
        }//????

        internal static void SendProcTiadcByRegisterProcTi(Int32[] dataArray, Int32 dataCount, ChannelId chnlId)
        {
            HdIO.WriteReg(ProcBdReg.W.TIADC_Enable, 0);
            //HdIO.WriteReg(ProcBdReg.W.TIADC_TiSelect, 0x1u << (Int32)chnlId);//根据通道进行选择,独热码形式 chnl1》0b1；chnl2》0b10
            for (int dataIndex = 0; dataIndex < dataCount / 2; dataIndex++)
            {
                HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnable, 0);
                //地址
                HdIO.WriteReg(ProcBdReg.W.TIADC_WriteAddress, (UInt32)dataIndex);
                //实部
                //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex * 2, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                UInt32 data = (UInt32)dataArray[dataIndex * 2];

                //data = 4096;
                HdIO.WriteReg(ProcBdReg.W.TIADC_FactorTableWriteData_r, (UInt32)data & 0xffff);
                //虚部
                //data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex * 2 + 1, (int)defineItem.BandMode, (int)defineItem.ChannelID, defineItem.SubbandIndex, (int)defineItem.FilterbandMode];
                //data = 0;
                data = (UInt32)dataArray[dataIndex * 2 + 1];
                HdIO.WriteReg(ProcBdReg.W.TIADC_FactorTableWriteData_i, (UInt32)data & 0xffff);
                HdIO.DelayByUs(10);
                //        HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnable, 0x1u << (Int32)chnlId);
                //       HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnable, 1);

                //HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnable, 0x0f);
            }
            HdIO.WriteReg(ProcBdReg.W.TIADC_WriteEnable, 0);
            HdIO.WriteReg(ProcBdReg.W.TIADC_Enable, 0);
            //HdIO.WriteReg(ProcBdReg.W.TIADC_Enable, Hd.CurrDebugVarints.bEnable_ProcCorrectTiAdc ? 1U : 0);
        }//????
    }
}
