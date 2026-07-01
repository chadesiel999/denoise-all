using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace ScopeX.Hardware.Driver
{
    internal class EmdProcess
    {
        public static readonly EmdProcess Default = new EmdProcess();

        internal EmdProcess()
        {
            foreach (ChannelId chnlid in ChannelIdExt.GetAnalogs())
            {
                _OldEnableTable.Add(chnlid, false);
                _SendCnt.Add(chnlid, 0);
                _ExportCnt.Add(chnlid, 0);

                _CapturedException.Add(chnlid, 0);
                _AnormalData.Add(chnlid, new Dictionary<UInt32, List<UInt16>>());
            } 
        }

        private Boolean _EnableChanged = false;

        private readonly Dictionary<ChannelId, Boolean> _OldEnableTable = new();
        private readonly Dictionary<ChannelId, Int32> _SendCnt = new();
        private readonly Dictionary<ChannelId, Int32> _ExportCnt = new();

        private readonly Dictionary<ChannelId, UInt32> _CapturedException = new();
        private readonly Dictionary<ChannelId, Dictionary<UInt32, List<UInt16>>> _AnormalData = new();

        #region 开关状态控制
        internal void ChangeExceptionCaptureState()
        {
            if (Hd.UIMessage?.AiTable == null)
                return;

            foreach (ChannelId chnlid in Hd.UIMessage.AiTable.Keys)
            {
                var captureexception = Hd.UIMessage.AiTable[chnlid].CaptureException;
                if (captureexception == null)
                    continue;

                CtrlTemplateState(chnlid, captureexception.Enable);
                if (!_OldEnableTable[chnlid] && captureexception.Enable)
                {
                    _AnormalData[chnlid].Clear();
                    _EnableChanged = true;
                }
                _OldEnableTable[chnlid] = captureexception.Enable;
            }
        }
        #endregion

        #region 模板数据下发
        private void SendTemplateFromSampleData(ChannelId chnlId, Int32 dataLength)
        {
            AcqBdNo[]? acqbdnos = GetAcqBdNoList(chnlId);
            if (acqbdnos == null)
                return;

            Int32 frameparalen = dataLength / 80;
            Int32 framecnt = 4000 / (frameparalen + 1);

            double tmbScale = Hd.UIMessage?.Timebase?.TmbScale ?? 1.0;
            double tmbPosition = Hd.UIMessage?.Timebase?.TmbPosition ?? 0;
            double perDataByfs_AtStorage = (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.PerDataByfs_AtDMA ?? 50_000_000); //20G sample,50_000_000
            Int64 depth = (Int64)((tmbScale * Constants.VIS_XDIVS_NUM / 2 - tmbPosition) * 1_000_000_000 / perDataByfs_AtStorage);
            Int32 trigpos = (Int32)depth;

            if (AcqedDataPool.AnalogChData.AllChannelData.Count > (Int32)chnlId)
            {
                if (AcqedDataPool.AnalogChData.AllChannelData[(Int32)chnlId].Count > trigpos)
                {
                    UInt32[] imfdatainner = AcqedDataPool.AnalogChData.AllChannelData[(Int32)chnlId].Skip(trigpos).Select(o => (UInt32)o).Take(dataLength).ToArray();
                    UInt32[] resdatainner = new UInt32[imfdatainner.Length];
                    for (Int32 i = 0; i < resdatainner.Length; i++)
                    {
                        resdatainner[i] = 1;
                       // imfdatainner[i] = (2049 + (uint)i) * 16;
                    }
                    foreach (AcqBdNo acq in acqbdnos)
                    {
                        SendEmdCoefficient(imfdatainner, resdatainner, acq, framecnt, frameparalen);
                    }
                }
            }
        }

        private void SendTemplate(ChannelId chnlId, UInt32[]? resData, UInt32[]? imfData)
        {
            AcqBdNo[]? acqbdnos = GetAcqBdNoList(chnlId);
            if (acqbdnos == null || resData == null || imfData == null)
                return;

            Int32 dataLength = Math.Min(resData.Length, imfData.Length);
            Int32 frameparalen = dataLength / 80;
            Int32 framecnt = 4000 / (frameparalen + 1);

            foreach (AcqBdNo acq in acqbdnos)
            {
                SendEmdCoefficient(imfData, resData, acq, framecnt, frameparalen);
            }
        }

        private void SendTemplateFromFile(ChannelId chnlId)
        { 
            String filename = $"./EmdData/UserDefine_{chnlId}.txt";
            (UInt32[]? imfdata, UInt32[]? resdata) = ImportData(filename);
            SendTemplate(chnlId, resdata, imfdata);
        }

        internal void TyrSendTemplate2FPGA()
        {
            if (Hd.UIMessage?.AiTable == null)
                return;

            foreach (ChannelId chnlid in Hd.UIMessage.AiTable.Keys)
            {
                var captureexception = Hd.UIMessage.AiTable[chnlid].CaptureException;
                if (captureexception == null)
                    continue;

                if (_SendCnt.ContainsKey(chnlid) && _SendCnt[chnlid] == captureexception.SendTemplateCnt)
                    continue;

                switch (captureexception.SourceType)
                {
                    case TemplateTriggerSourceEnum.Origin:
                        SendTemplateFromSampleData(chnlid, captureexception.FrameLength);
                        break;
                    case TemplateTriggerSourceEnum.Inner:
                        SendTemplate(chnlid, captureexception.ResData?.ToArray(), captureexception.ImfData?.ToArray());
                        break;
                    case TemplateTriggerSourceEnum.File:
                        SendTemplateFromFile(chnlid);
                        break;
                }
                _SendCnt[chnlid] = captureexception.SendTemplateCnt;
                _AnormalData[chnlid].Clear();
            }
        }
        #endregion

        #region 异常数据读取
        internal void UpdateAbnormalData()
        {
            if (Hd.UIMessage?.AiTable == null)
                return;
            var chnlid = ChannelId.C1;
            //foreach (ChannelId chnlid in Hd.UIMessage.AiTable.Keys)
            {
                var captureexception = Hd.UIMessage.AiTable[chnlid].CaptureException;
                if (captureexception == null || (!captureexception.Enable))
                    return;

                AcqBdNo[]? acqbdnos = GetAcqBdNoList(chnlid);
                if (acqbdnos == null || acqbdnos.Length < 1)
                    return;
                UInt32 tmpcnt = 0;
                tmpcnt = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.anormdetect_wr_frame, acqbdnos[0]) ?? 0;
                if ((_CapturedException.ContainsKey(chnlid) && _CapturedException[chnlid] == tmpcnt))
                {
                    return;
                }
                _EnableChanged = false;
                _CapturedException[chnlid] = tmpcnt;
                HdIO.WriteReg(ProcBdReg.W.DataPath_ProDataMux, 04);
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);
                //TryReadAbnormalData(chnlid, tmpcnt, captureexception.FrameLength);
                int length = 7168;
                if (captureexception.FrameLength == 25)
                {
                    length = 43008;
                } 
                else if (captureexception.FrameLength == 50) 
                {
                    length = 22528;
                }
                else if (captureexception.FrameLength == 100)
                {
                    length = 12288;
                }
                else if (captureexception.FrameLength == 200)
                {
                    length = 7168;
                }

                if (Hd.UIMessage!.Timebase!.TmbScale < 0.02)
                {
                    TryReadAbnormalData(chnlid, tmpcnt, length * 4);
                }
                else
                {
                    TryReadAbnormalData(chnlid, tmpcnt, length);
                }

                //TryReadAbnormalData(chnlid, tmpcnt, 7168*4);//0->7168 1->12288 2->22528 3->43008

            }
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.anormdetect_emd_rd_en, 0);
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
        }

        private void TryReadAbnormalData(ChannelId chnlId, List<UInt16> frameIds, Int32 Emdlength)
        {
            AcqBdNo[]? tempacqbdno = GetAcqBdNoList(chnlId);
            if (tempacqbdno == null || tempacqbdno.Length < 1)
                return;
            Int32 AnormDataLength = Emdlength + 80;
            foreach (var frameid in frameIds)
            {
                _AnormalData[chnlId][frameid] = ReadAnormData(frameid, tempacqbdno[0], AnormDataLength);
            }
        }

        private void TryReadAbnormalData(ChannelId chnlId, UInt32 frameCnt, Int32 frameLength)
        {
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreRst, 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_emd_detec_en, 0x0002);
            //HdIO.WriteReg(ProcBdReg.W.DataPath_ProDataMux, 4);
            //HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreRst, 0);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_emd_rd_en, 0x0000);
            HdIO.DelayByUs(10);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_emd_rd_en, 0x0002);
            HdIO.DelayByUs(10);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_emd_rd_en, 0x0000);
            HdIO.DelayByUs(10);
           // Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_emd_rd_en, 0x0001);
            //HdIO.WriteReg((uint)PcieBdReg.W.Xdma_XdmaIrqReset, 0);
            //HdIO.WriteReg((uint)PcieBdReg.W.Xdma_XdmaIrqReset, 1);



            Int32 AnormDataLength = frameLength;

            //Int32 sumbytescnt = (Int32)(frameCnt * AnormDataLength *8);//总共需要读取的字节数
            //Int32 sumbytescnt = (Int32)(1 * AnormDataLength *8);//总共需要读取的字节数
            // Int32 sumbytescnt = (Int32)(16000);
            Int32 sumbytescnt = (Int32)((frameCnt) * AnormDataLength * 2 * 2 * 2);//总共需要读取的字节数
                                                                        //Int32 sumbytescnt = (Int32)(frameCnt * AnormDataLength *4);//总共需要读取的字节数

            var _DmaBuff = new Byte[sumbytescnt];
            
            
            var bok = Hd.AnalogChannel?.ReadDMAAnorm(DMAReadDataTypes.MeasureHist, (UInt32)sumbytescnt, _DmaBuff);

            if (bok == false)
            {
                return;
            }
            // ------------------------
            // 1) Byte → UInt16 解码（每2字节一个值，右移4bit）
            // ------------------------
            int u16Count = _DmaBuff.Length / 2;
            var u16Buffer = new ushort[u16Count];

            for (int i = 0, j = 0; i < _DmaBuff.Length; i += 2, j++)
            {
                u16Buffer[j] = (ushort)((_DmaBuff[i] | (_DmaBuff[i + 1] << 8)));
            }

            // ------------------------
            // 2) 每 16 个取前 8 个（直接写入 newBuffer）
            //    也就是 size/2
            // ------------------------
            //int packedCount = u16Count / 2;
            //var newBuffer = new ushort[packedCount];

            //for (int i = 0, j = 0; i < u16Count; i += 16, j += 8)
            //{
            //    // Copy 前 8 个
            //    Buffer.BlockCopy(u16Buffer, i * 2, newBuffer, j * 2, 16); // 8 * 2 bytes
            //}

            // ------------------------
            // 3) 重新按 frameLength 分帧
            // ------------------------
            var newCount = u16Count / 4;
            var newBuffer = new ushort[newCount];
            for (int i = 0, j = 0; i < u16Count; i += 4, j += 1)
            {
                newBuffer[j] = u16Buffer[i];
                //newBuffer[j + 1] = u16Buffer[i + 1];
            }
            Int32[] discardbefore = new Int32[4];
            Int32[] discardafter = new Int32[4];

            Int32 yscaleId = (Int32)AnaChnlScaleIndex.Lv1;
            for (Int32 subid = 0; subid < 4; subid++)
            {
                discardbefore[subid] = DbiAnalogParams.Default[0, 0, yscaleId, subid].DiscardDotsBefore;
                discardafter[subid] = DbiAnalogParams.Default[0, 0, yscaleId, subid].DiscardDotsAfter;
            }

            int frames = newBuffer.Length / frameLength;

            for (int i = 0; i < frames; i++)
            {
                var frame = new ushort[frameLength];
                Buffer.BlockCopy(newBuffer, i * frameLength * 2, frame, 0, frameLength * 2);
                if (Hd.UIMessage!.Timebase!.TmbScale < 0.02)
                {
                    if (i == 0)
                        _AnormalData[chnlId][(UInt32)i] = frame.Skip(1024 * 4 - 800 - discardafter.Sum()).Take(frameLength-2048*4).ToList();
                    else
                        _AnormalData[chnlId][(UInt32)i] = frame.Skip(1024 * 4).Take(frameLength - 2048 * 4).ToList();
                }
                else
                {
                    _AnormalData[chnlId][(UInt32)i] = frame.ToList();
                }
            }


            //else
            //    _AnormalData.Add(chnlId, new Dictionary<UInt32, List<UInt16>> { { frameid, buffer.ToList() } });


            //sumdata.AddRange(datadata);


            //Int32 bytescnt = sumbytescnt;// 一次读取DMA的字节数
            //Byte[] tmpbuff = new Byte[bytescnt];
            //HdIO.WriteReg(ProcBdReg.W.RST_CTRL_SysReset, 1);//采集数据复位
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SerialTrigEnable, 0);//采集数据复位
            //HdIO.WriteReg(ProcBdReg.W.LSCtrl_DDREnablePro, 0);//采集数据复位
            ////HdIO.WriteReg(ProcBdReg.W.DataPath_SoftfifoFullThreshold, 10000);//采集数据复位
            ////HdIO.WriteReg(ProcBdReg.W.Debug_SingleModePro, 0b0000_1111_1111_0111);//采集数据复位
            ////HdIO.WriteReg(AcqBdReg.W.DataPath_AfifoPreP2SFullThresh,20);//采集数据复位
            ////.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_AfifoPreP2SFullThresh, 100);
            //HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x1);
            //HdIO.WriteReg(PcieBdReg.W.Xdma_DataNum, (UInt32)bytescnt * 8);

            //HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x0);
            //HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x1);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DataPath_acq_linkmux_select, 0);
            //HdIO.WriteReg((uint)PcieBdReg.W.ReadFromAcqOrDpo, 0);
            //HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, (UInt32)DMAReadDataTypes.MeasureHist);

            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.anormdetect_emd_rd_en, 0x0001);
            //HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x0);
            //HdIO.WriteReg(ProcBdReg.W.RST_CTRL_SysReset, 0);//采集数据复位
            //List<UInt16> sumdata = new();
            //for (Int32 byteid = 0; byteid < sumbytescnt; byteid += bytescnt)
            //{
            //    // 配路径
            //    Thread.Sleep(1);
            //    // 检查可读标志，确保数据已经送到了PCIE存储区
            //    var pciefull = (HdIO.ReadReg(PcieBdReg.R.Xdma_XdmaWrFinish) & 0x01) == 1;
            //    // DMA读取
            //    if (HdIO.CheckRegisterValue((uint)PcieBdReg.R.Xdma_XdmaWrFinish, 1, 0x01, 1000))
            //    {
            //        bool readok = HdIO.DMARead((UInt32)bytescnt, ref tmpbuff);
            //        if (readok)
            //        {
            //            // 解析
            //            UInt16[] datadata = Hd.CurrProduct.Acquirer_AnalogChannel?.ParseDMAData((Int32)chnlId, tmpbuff.Take(bytescnt).ToArray()).ToArray() ?? new UInt16[0];
            //            sumdata.AddRange(datadata);
            //        }
            //    }

            //}

            //for (UInt32 frameid = 0; frameid < frameCnt; frameid++)
            //{
            //    if (_AnormalData[chnlId].ContainsKey(frameid))
            //    {
            //        for (Int32 dotid = 0; dotid < AnormDataLength; dotid++)
            //        {
            //            var sumdataid = frameid * AnormDataLength + dotid;
            //            if (sumdataid >= sumdata.Count)
            //                break;
            //            if (dotid < _AnormalData[chnlId][frameid].Count)
            //                _AnormalData[chnlId][frameid][dotid] = sumdata[(Int32)sumdataid];
            //            else
            //                _AnormalData[chnlId][frameid].Add(sumdata[(Int32)sumdataid]);
            //        }
            //    }
            //    else
            //    {
            //        Int32 dotstartid = (Int32)frameid * AnormDataLength;
            //        if (dotstartid + AnormDataLength <= sumdata.Count)
            //        {
            //            _AnormalData[chnlId].Add(frameid, sumdata.Skip(dotstartid).Take(AnormDataLength).ToList());
            //        }
            //    }
            //}

            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.anormdetect_emd_rd_en, 0);
            //HdIO.WriteReg(ProcBdReg.W.LSCtrl_DDREnablePro, 0);//采集数据复位
        }
        #endregion

        #region 异常数据导出
        internal void TryExport2File()
        {
            if (Hd.UIMessage?.AiTable == null)
                return;

            foreach (ChannelId chnlid in Hd.UIMessage.AiTable.Keys)
            {
                var captureexception = Hd.UIMessage.AiTable[chnlid].CaptureException;
                if (captureexception == null)
                    continue;

                if (_ExportCnt.ContainsKey(chnlid) && _ExportCnt[chnlid] == captureexception.Export2FileCnt)
                    continue;

                Export2File(chnlid);
                _ExportCnt[chnlid] = captureexception.Export2FileCnt;
            }
        }

        private void Export2File(ChannelId chnlId)
        {
            if (_AnormalData.ContainsKey(chnlId))
            {
                foreach (UInt32 frameid in _AnormalData[chnlId].Keys)
                {
                    StreamWriter sw = new StreamWriter($"AnormalWave_{chnlId}_{frameid}.txt", false);
                    for (Int32 i = 0; i < _AnormalData[chnlId][frameid].Count; i++)
                    {
                        sw.WriteLine(_AnormalData[chnlId][frameid][i]);
                    }
                    sw.Flush();
                    sw.Close();
                }
            }
        }
        #endregion

        internal void PropertiesChanged()
        {
            Config();
            TyrSendTemplate2FPGA();
            ChangeExceptionCaptureState();
            TryExport2File();
        }

        internal void Config()
        {
            var active = Hd.UIMessage!.AiTable[ChannelId.C1]!.CaptureException!.Enable;
            _CapturedException[ChannelId.C1] = 0;

            if (!active)
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreRst, 1);
            }
        }//fre

        internal void Init()
        {
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.anormdetect_trig_sign_data_delay_rst_n, 0);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.anormdetect_trig_sign_data_delay, 13);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.anormdetect_trig_sign_data_delay_rst_n, 1);

            ConfigCoefs();
        }

        internal void ConfigCoefs()
        {
            //频域异常检测下发系数，暂时写着，后面只需要开机下发一次
            List<UInt32> coelist1 = new List<UInt32>(); //第一部分coe
            var filename = Path.Combine($@"{AppDomain.CurrentDomain.BaseDirectory}CaliData\CoeFiles\", "abnormal_COE.txt");
            using (StreamReader fs_dbi1 = new StreamReader(filename))
            {
                string line_dbi1;
                while ((line_dbi1 = fs_dbi1.ReadLine()) != null)
                {
                    line_dbi1 = line_dbi1.Trim();
                    if (string.IsNullOrEmpty(line_dbi1))
                    {
                        continue;
                    }
                    var number_dbi = Convert.ToUInt32(line_dbi1, 2);
                    coelist1.Add(number_dbi);

                }
            }
            uint[] dataArray1 = coelist1.ToArray();

            List<UInt32> coelist2 = new List<UInt32>(); //第二部分ejw
            filename = Path.Combine($@"{AppDomain.CurrentDomain.BaseDirectory}CaliData\CoeFiles\", "abnormal_EJW.txt");
            using (StreamReader fs_dbi2 = new StreamReader(filename))
            {
                string line_dbi2;
                while ((line_dbi2 = fs_dbi2.ReadLine()) != null)
                {
                    line_dbi2 = line_dbi2.Trim();
                    if (string.IsNullOrEmpty(line_dbi2))
                    {
                        continue;
                    }
                    var number_dbi = Convert.ToUInt32(line_dbi2, 2);
                    coelist2.Add(number_dbi);

                }
            }
            UInt32[] dataArray2 = coelist2.ToArray();
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreRst, 1);//开机时先将异常检测的模块复位，防止开机时模块工作功耗高
            //HdIO.WriteReg((uint)AcqBdReg.W.anormdetect_TimeFreInitial, 0x0004);//开机时下发系数的复位拉高
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreInitial, 0x0004);//whj

            for (int dataIndex1 = 0; dataIndex1 < 512; dataIndex1++)
            {
                //HdIO.WriteReg((uint)AcqBdReg.W.anormdetect_TimeFreInitial, 0x0000);//0000
                //HdIO.WriteReg((uint)AcqBdReg.W.anormdetect_TimeFreFirCoe, dataArray1[dataIndex1] & 0xffff);
                //HdIO.WriteReg((uint)AcqBdReg.W.anormdetect_TimeFreInitial, 0x0002);//0002

                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreInitial, 0x0000);//whj
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreFirCoe, dataArray1[dataIndex1] & 0xffff);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreInitial, 0x0002);

            }

            for (int dataIndex2 = 0; dataIndex2 < 128; dataIndex2++)
            {
                //HdIO.WriteReg((uint)AcqBdReg.W.anormdetect_TimeFreInitial, 0x0000);//0000
                //HdIO.WriteReg((uint)AcqBdReg.W.anormdetect_TimeFreEjw, dataArray2[dataIndex2] & 0xffff);
                //HdIO.WriteReg((uint)AcqBdReg.W.anormdetect_TimeFreInitial, 0x0001);//0001

                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreInitial, 0x0000);//whj
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreEjw, dataArray2[dataIndex2] & 0xffff);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreInitial, 0x0001);

            }
            //HdIO.WriteReg((uint)AcqBdReg.W.anormdetect_TimeFreInitial, 0x0000);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreInitial, 0x0000);//whj
        }//fre

        internal String TryGetData(Object param, out Object? data)
        {
            if (param is ChannelId && _CapturedException.ContainsKey((ChannelId)param))
            {
                data = 0;
                if (_AnormalData.ContainsKey((ChannelId)param))
                {
                    data = _CapturedException[(ChannelId)param];
                    System.Diagnostics.Debug.WriteLine(data);
                }
                return "";
            }
            if (param is ExceptionData)
            {
                ExceptionData tmp = (ExceptionData)param;
                data = TryGetAbnormalData(tmp.chnlId, tmp.frameIds);
                return "";
            }
            if (param.ToString() == "StudySatus")
            {
                data = GetWfmStudyStatus();
                return "";
            }
            data = null;
            return "";
        }

        internal Dictionary<UInt32, List<UInt16>> TryGetAbnormalData(ChannelId chnlId, List<UInt32> frameIds)
        {
            Dictionary<UInt32, List<UInt16>> retdata = new();

            if (_AnormalData.ContainsKey(chnlId))
            {
                foreach (UInt32 frameid in frameIds)
                {
                    if (_AnormalData[chnlId].ContainsKey(frameid))
                    {
                        retdata.Add(frameid, _AnormalData[chnlId][frameid].ToList());
                    }
                }
            }

            return retdata;
        }

        internal List<List<UInt16>>? TryGetAbnormalData(ChannelId chnlId)
        {
            if ((Hd.UIMessage?.AiTable == null) || (Hd.UIMessage.AiTable.ContainsKey(chnlId) == false))
                return null;

            var capturexceptionunion = Hd.UIMessage.AiTable[chnlId].AIUnion;
            if (_AnormalData.ContainsKey(chnlId) && _AnormalData[chnlId].Count > 0)
            {
                List<List<UInt16>> data = new();
                foreach (UInt32 frame in _AnormalData[chnlId].Keys)
                {
                    data.Add(_AnormalData[chnlId][frame].Select(o => o).ToList());
                }
                return data.Count > 0 ? data : null;
            }

            return null;
        }

        internal void WfmStudy()
        {
            _CapturedException[ChannelId.C1] = 0;

            //FPGA同学填写，控制寄存器
            Thread.Sleep((int)(Hd.Cymometer?.GateTime ?? 1));
            var vds = Hd.CurrProduct.Acquirer_AnalogChannel?.SubbandEnergyTable.Where(o => o.Value).Select(o => o.Key).ToArray() ?? new int[] { 0, };
            var sf = ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.GetSignalFreq();
            //Double signalFrequencyHz = ConfigBaseNoise(2);
            //var freq = CalculateTetectFrequency(signalFrequencyHz * 1E-6);
            //Int32 currentSubBandIndex = GetSubBandIndex(signalFrequencyHz * 1E-9);
            AcqBdNo[]? acqbdnos = GetAcqBdNoList(ChannelId.C1);
            if (acqbdnos != null && acqbdnos.Length > 0)
            {
                for (Int32 i = 0; i < acqbdnos.Length; i++)
                {
                    if (vds.Length > 0 && vds.Contains(i))
                    {
                        Double signalFrequency = (sf[i].FreqMax + sf[i].FreqMin) / 2;
                        if (i == 0)
                        {
                            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.anormdetect_TimeFreTetectFrequency, acqbdnos[i], CalculateTetectFrequency((signalFrequency) * 1E-6));
                        }
                        else if (i == 1) 
                        {
                            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.anormdetect_TimeFreTetectFrequency, acqbdnos[i], CalculateTetectFrequency((10E9 - signalFrequency) * 1E-6));
                        }
                        else if (i == 2) 
                        {
                            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.anormdetect_TimeFreTetectFrequency, acqbdnos[i], CalculateTetectFrequency((15E9 - signalFrequency) * 1E-6));
                        } 
                        else 
                        {
                            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.anormdetect_TimeFreTetectFrequency, acqbdnos[i], CalculateTetectFrequency((22.5E9 - signalFrequency) * 1E-6));
                        }

                    }
                    else 
                    {
                        Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.anormdetect_TimeFreTetectFrequency, acqbdnos[i], 1);
                    }
                    
                }
            }
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreTetectFrequency, 14);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreThresh, 0x3232);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_Reverse2, 0);//time_fre_storage_depth
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_Reverse3, 0);//time_fre_storage_location
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreRstCntHigh, 0x1000);//学习时间，以3.2ns为单位
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreRstCntLow, 0x0000);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreRst, 1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_emd_detec_en, 0x00);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreRst, 0);
            foreach (var kv in _AnormalData)
            {
                kv.Value.Clear();
            }
        }

        internal void ExcuteExCapture()
        {
            _CapturedException[ChannelId.C1] = 0;

            //FPGA同学填写，控制寄存器
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreRst, 1);
            var captureexception = Hd.UIMessage.AiTable[ChannelId.C1].CaptureException;
            if (captureexception.FrameLength == 25)
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_Reverse2, 3);
            }
            else if (captureexception.FrameLength == 50)
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_Reverse2, 2);
            }
            else if (captureexception.FrameLength == 100)
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_Reverse2, 1);
            }
            else if (captureexception.FrameLength == 200)
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_Reverse2, 0);
            }
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_Reverse2, 0);//time_fre_storage_depth
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_Reverse3, 1);//time_fre_storage_location
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_Reverse4, 64);//64
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_Reverse1, 32+16);//test
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_emd_detec_en, 0x01);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_TimeFreRst, 0);
        }

        /// <summary>
        /// //起始检测频率，频率小于78.125MHz发1，否则发（f-78.125）/156.25的商+2
        /// </summary>
        /// <returns></returns>
        internal uint CalculateTetectFrequency()
        {
            Thread.Sleep((int)(Hd.Cymometer?.GateTime ?? 1));
            var frequency = ConfigBaseNoise(0) * 1E-6;//Hd.Cymometer?.GetFrequencyByHz() * 1E-6 ?? 0.0;v   
            return CalculateTetectFrequency(frequency);
        }

        private uint CalculateTetectFrequency(Double frequency)
        {
            //判断频率所在子带，然后每个（本振频率 - 信号频率）以中心频率为准
            //var tmp = testc;

            if (frequency < 78.125 + 156.25 * 3)
            {
                return 1;
            }
            else
            {
                double quotient = (frequency - 78.125) / 156.25; //tmp
                return (uint)Math.Floor(quotient) + 2 - 3;
            }
        }

        private Int32 GetSubBandIndex(Double frequencyGHz)
        {
            if (frequencyGHz < 6.0)
                return 0;
            if (frequencyGHz < 9.5)
                return 1;
            if (frequencyGHz < 14.5)
                return 2;
            return 3;
        }

        private Double ConfigBaseNoise(Int32 subbandid)
        {
            var aimsg = Hd.UIMessage?.AiTable;
            if (aimsg != null)
            {
                UInt32 fpgaactivestate = Hd.CurrProduct?.AnalogAcquireModel?.GetActuallActiveState(0x1u << (Int32)subbandid) ?? 0xf;
                var adcuseinfo = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(fpgaactivestate, ChannelId.C1, subbandid);
                if (adcuseinfo != null)
                {
                    Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.SenceFFT_sence_fft_threshold_l16, adcuseinfo.AcqBdNo, 0);                        //    //
                    Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.SenceFFT_sence_fft_threshold_h16, adcuseinfo.AcqBdNo, 80);     //

                    UInt32? tmpdata = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_out_valid, adcuseinfo.AcqBdNo);

                    if (tmpdata == 1)
                    {
                        UInt32 max = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_freg_max, adcuseinfo.AcqBdNo);
                        UInt32 min = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_freg_min, adcuseinfo.AcqBdNo);
                        Double freqMax = max * 20 * 1e9 / 4096;
                        Double freqMin = min * 20 * 1e9 / 4096;
                        Double freq = (freqMax + freqMin) / 2;
                        return freq;
                    }
                }
            }
            return 0;
        }

        private Double LeftFreqBand(Double localFreq, UInt32 fftid)
        {
            return localFreq - fftid * 20 * 1e9 / 4096;
        }

        private Double RightFreqBand(Double localFreq, UInt32 fftid)
        {
            return localFreq + fftid * 20 * 1e9 / 4096;
        }

        internal Boolean GetWfmStudyStatus()
        {
            //FPGA同学填写，控制寄存器
            static Boolean IsSubBandDone(UInt32? regValue) => ((regValue ?? 0u) & 0x01u) == 0x01u;

            Boolean WaitSubBandDone(AcqBdNo acqBdNo)
            {
                var sw = Stopwatch.StartNew();
                Boolean status = IsSubBandDone(Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.AnormTemplate_ReverseRd0, acqBdNo));
                while (!status && sw.ElapsedMilliseconds < 2000)
                {
                    Thread.Sleep(50);
                    status = IsSubBandDone(Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.AnormTemplate_ReverseRd0, acqBdNo));
                }

                return status;
            }

            var statusB0 = WaitSubBandDone(AcqBdNo.B0);
            var statusB1 = WaitSubBandDone(AcqBdNo.B1);
            var statusB2 = WaitSubBandDone(AcqBdNo.B2);
            var statusB3 = WaitSubBandDone(AcqBdNo.B3);

            // 四个子带都完成才返回成功
            //return statusB0 && statusB1 && statusB2 && statusB3;
            return statusB0 && statusB2;
        }

        #region FPGA同学填写，控制寄存器
        /// <summary>
        /// 下发模板系数
        /// </summary>
        /// <param name="imfData"></param>
        /// <param name="resData"></param>
        /// <param name="acqBd"></param>
        /// <param name="FrameMax"></param>
        /// <param name="EmdParalength"></param>
        private void SendEmdCoefficient(UInt32[] imfData, UInt32[] resData, AcqBdNo acqBd, Int32 FrameMax, Int32 EmdParalength)
        {
            Hd.CurrProduct.AcqBd?.WriteReg(AcqBdReg.W.anormdetect_emd_detec_en, acqBd, 0);
            Hd.CurrProduct.AcqBd?.WriteReg(AcqBdReg.W.anormdetect_BasicHysteresis, acqBd, 100);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_emd_frame_max, (uint)FrameMax);
            Hd.CurrProduct.AcqBd?.WriteReg(AcqBdReg.W.anormdetect_ParseLenParallel, acqBd, (uint)EmdParalength);
            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_trig_sign_data_delay_rst_n, acqBd, 0);
            //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_trig_sign_data_delay, acqBd, 13);
            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_trig_sign_data_delay_rst_n, acqBd, 1);
            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_emd_source_sel, acqBd, 1);
            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_cnt_tresh, acqBd, 1);
            //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_single_tresh, acqBd, 400);
            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_single_tresh, acqBd, 300);//800
            //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_total_treshl16, AcqBdReg.W.anormdetect_total_treshh16, acqBd, (uint)(9000 * EmdParalength*2));
            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_total_treshl16, AcqBdReg.W.anormdetect_total_treshh16, acqBd, (uint)(1500 * EmdParalength * 80 * 2));
            Hd.CurrProduct.AcqBd?.WriteReg(AcqBdReg.W.anormdetect_emd_detec_en, acqBd, 0b01);
            for (Int32 i = 0; i < imfData.Length && i < resData.Length; i++)
            {
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_emd_data_soft_valid, acqBd, 0);
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_emd_imf_sum_data_soft, acqBd, imfData[i] & 0xffff);
                //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_emd_imf_sum_data_soft, acqBd, 0x5a5a & 0xffff);
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_emd_res_data_soft, acqBd, resData[i] & 0xffff);
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_emd_data_soft_valid, acqBd, 1);
            }
            //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_emd_source_sel, acqBd, 0);
            //Hd.CurrProduct.AcqBd?.WriteReg(AcqBdReg.W.anormdetect_emd_detec_en, acqBd, 0);
        }

        /// <summary>
        /// 控制异常检测的开关
        /// </summary>
        /// <param name="chnlId"></param>
        /// <param name="enable"></param>
        private void CtrlTemplateState(ChannelId chnlId, Boolean enable)
        {
            var activechnls = Hd.CurrProduct?.Acquirer_AnalogChannel?.AnalogAcquireModel?.DeafultChannelState ?? 0x1;
            var adcinfo = Hd.CurrProduct?.Acquirer_AnalogChannel?.AnalogAcquireModel?.GetAdcUsedInfo(activechnls, chnlId);
            if (adcinfo == null || adcinfo.Length < 1)
                return;

            if (enable)
            {
                var emdstorefinish = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.anormdetect_emd_store_finish, adcinfo[0].AcqBdNo);
                if (emdstorefinish == 1)
                {
                    Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.anormdetect_emd_detec_en, adcinfo[0].AcqBdNo, 0b11);
                    _CapturedException[chnlId] = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.anormdetect_wr_frame, adcinfo[0].AcqBdNo) ?? 0;
                }
                else
                {
                    //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.anormdetect_emd_detec_en, adcinfo[0].AcqBdNo, 0b01);
                }
                //ZYXin emd_anorm_rd
                //var emdstorefinish = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.anormdetect_emd_store_finish, AcqBdNo.B1);
                //if (emdstorefinish == 1)
                //{
                //    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_emd_detec_en, AcqBdNo.B1, 0b11);
                //    //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_cnt_tresh, acqBd, 1);
                //    //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_single_tresh, acqBd, 1);
                //    //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_total_treshL16, AcqBdReg.W.anormdetect_total_treshH16, acqBd, 1);
                //    //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_trig_sign_data_delay, acqBd, 1);
                //    //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_trig_sign_data_delay_rst_n, acqBd, 1);
                //    //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_trig_cmp1_level_l, AcqBdReg.W.anormdetect_trig_cmp1_level_h, acqBd, 1);
                //    //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_trig_edge_sel, acqBd, 1);
                //}
            }
            else
            {
                //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.anormdetect_emd_detec_en, adcinfo[0].AcqBdNo, 0b01);
                _CapturedException[chnlId] = 0;
            }
        }

        /// <summary>
        /// 读取异常数据
        /// </summary>
        /// <param name="frameid"></param>
        /// <param name="acqBd"></param>
        /// <param name="datalength"></param>
        /// <returns></returns>
        private List<UInt16> ReadAnormData(UInt32 frameid, AcqBdNo acqBd, Int32 datalength)
        {
            List<UInt16> data = new List<UInt16>();
            HdIO.WriteReg(ProcBdReg.W.RST_CTRL_SysReset, 1);//采集数据复位
            //HdIO.WriteReg(ProcBdReg.W.RST_CTRL_SysReset, 0);//采集数据复位
            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_emd_rd_en, acqBd, 0);
            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_rd_frame, acqBd, frameid);
            HdIO.WriteReg(ProcBdReg.W.RST_CTRL_SysReset, 0);//采集数据复位
            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_emd_rd_en, acqBd, 0b01);
            HdIO.Sleep(1);
            for (Int32 i = 0; i < datalength; i++)
            {
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_emd_rd_en, acqBd, 0b11);
                var anorm_data = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.anormdetect_rd_anorm_data, acqBd);
                if (anorm_data != null)
                {
                    data.Add((UInt16)(anorm_data ?? 0));
                }
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_emd_rd_en, acqBd, 0b01);
            }

            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.anormdetect_emd_rd_en, acqBd, 0);

            return data;
        }
        #endregion

        private void SendTemplate(ChannelId chnlId, CaptureExceptionRecord captureexceptionmsg)
        {
            AcqBdNo[]? acqbdnos = GetAcqBdNoList(chnlId);
            if (acqbdnos == null)
                return;

            // todo
            Int32 Emd_Length = captureexceptionmsg.FrameLength;
            Int32 EmdParalength = Emd_Length / 80;
            Int32 FrameMax = 4000 / (EmdParalength + 1);
            // 帧数 : captureexceptionmsg.FrameCnt
            // 帧长度 : captureexceptionmsg.FrameLength

            if (captureexceptionmsg.ImfData != null && captureexceptionmsg.ResData != null)
            {
                foreach (AcqBdNo acq in acqbdnos)
                {
                    SendEmdCoefficient(captureexceptionmsg.ImfData.ToArray(), captureexceptionmsg.ResData.ToArray(), acq, FrameMax, EmdParalength);
                }
                return;
            }

            #region 调试代码
            #region 先检测是否存在用户自定义的数据文件，存在就下发
            String userfilename = $"./EmdData/UserDefine_{chnlId}.txt";
            (UInt32[]? imfdata, UInt32[]? resdata) = ImportData(userfilename);
            if (imfdata != null && resdata != null)
            {
                foreach (AcqBdNo acq in acqbdnos)
                {
                    SendEmdCoefficient(imfdata, resdata, acq, FrameMax, EmdParalength);
                }
                return;
            }
            #endregion

            #region 没有用户自定义的数据文件，内部产生
            double tmbScale = Hd.UIMessage?.Timebase?.TmbScale ?? 1.0;
            double tmbPosition = Hd.UIMessage?.Timebase?.TmbPosition ?? 0;
            double perDataByfs_AtStorage = (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.PerDataByfs_AtDMA ?? 50_000_000); //20G sample,50_000_000
            Int64 depth = (Int64)((tmbScale * Constants.VIS_XDIVS_NUM / 2 - tmbPosition) * 1_000_000_000 / perDataByfs_AtStorage);
            Int32 trigpos = (Int32)depth;

            if (AcqedDataPool.AnalogChData.AllChannelData.Count > (Int32)chnlId)
            {
                if (AcqedDataPool.AnalogChData.AllChannelData[(Int32)chnlId].Count > trigpos)
                {
                    UInt32[] imfdatainner = AcqedDataPool.AnalogChData.AllChannelData[(Int32)chnlId].Skip(trigpos).Select(o => (UInt32)o).Take(Emd_Length).ToArray();
                    UInt32[] resdatainner = new UInt32[imfdatainner.Length];
                    for (Int32 i = 0; i < resdatainner.Length; i++)
                        resdatainner[i] = 1;

                    foreach (AcqBdNo acq in acqbdnos)
                    {
                        SendEmdCoefficient(imfdatainner, resdatainner, acq, FrameMax, EmdParalength);
                    }
                }
            }
            #endregion
            #endregion
        }


        private void ExportData(String fileName, UInt32[] imfData, UInt32[] resData)
        {
            StreamWriter sw = new StreamWriter(fileName);
            for (Int32 i = 0; i < imfData.Length && i < resData.Length; i++)
            {
                sw.WriteLine($"{imfData[i]}, {resData[i]}");
            }
            sw.Flush();
            sw.Close();
        }

        private (UInt32[]?, UInt32[]?) ImportData(String fileName)
        {
            if (File.Exists(fileName))
            {
                StreamReader sr = new StreamReader(fileName);
                String datastr = sr.ReadToEnd();
                String[] datainfo = datastr.Split("\r\n");
                List<UInt32> imf = new List<UInt32>();
                List<UInt32> res = new List<UInt32>();
                for (Int32 i = 0; i < datainfo.Length; i++)
                {
                    String[] lineinfo = datainfo[i].Split(",");
                    if (lineinfo.Length < 2)
                        continue;
                    imf.Add(UInt32.Parse(lineinfo[0].Replace(" ", "")));
                    res.Add(UInt32.Parse(lineinfo[1].Replace(" ", "")));
                }
                return (imf.ToArray(), res.ToArray());
            }
            return (null, null);
        }

        private void SendDataToMatlab(ChannelId chnlid, UInt32 frameid, List<UInt32> data)
        {
            Matlab.Default.PutData(data.ToArray(), $"{chnlid}_{frameid}");
            Matlab.Default.ExcuteCode($"{chnlid}_{frameid}_matlab = cell2mat({chnlid}_{frameid});");
            Matlab.Default.ExcuteCode($"figure\nplot({chnlid}_{frameid}_matlab)\ntitle('{chnlid}_{frameid}')");
        }

        private AcqBdNo[]? GetAcqBdNoList(ChannelId chnlId)
        {
            var activechnls = Hd.CurrProduct?.Acquirer_AnalogChannel?.AnalogAcquireModel?.DeafultChannelState ?? 0x1;
            var adcinfos = Hd.CurrProduct?.Acquirer_AnalogChannel?.AnalogAcquireModel?.GetAdcUsedInfo(activechnls, chnlId);

            return adcinfos?.Select(o => o.AcqBdNo).ToArray();
        }
    }
}
