using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.MathExt;
using ScopeX.Measure;

namespace ScopeX.Hardware.Driver
{
    internal class MultiDomainProcess : AbstractAcquirer_RadioFrequency
    {
        internal static byte[] DMAData = new byte[16 * 8 * 1024];
        internal static byte[] DMADataXi = new byte[5 * 1024 * 1024];//lhc
        internal static byte[] DMADataCu = new byte[10 * 1024 * 1024];
        internal static byte[] DMATotalData = new byte[16 * 1024 * 1024];

        private byte[] IQDMAData = new byte[4 * 16 * 8 * 1024];
        bool bDDR_SoftwareEmulate = true;
        int cnt = 0;
        int test = 0;
        List<List<double>> roughspecdata = new List<List<double>>();
        //List<Double> spectrogramdata = new List<Double>();

        Double Tstart = 0;
        Double Tstop = 0;
        Double CenterFreq = 0;
        Double Span = 0;
        Double SamplingRateByus = 1;
        private UInt32 FifoMaxDepth = 16 * 1024;
        public static uint _test = 0;
        public static uint Params = 0;
        internal bool bDataVaild;

        Double samplingratehardware = 1;
        Double rbwhardware = 1000;
        Int64 spanhardware = 8_000_000_000;

        Int64 spansync = 8_000_000_000;
        Int64 fftlengthsync = 8192;
        Int64 spanparamtuning = 8_000_000_000;
        Int64 centerfreqparamtuning = 4_000_000_000;

        Int64 stateparamtuning = 0;

        Int64 fft_length = 8192;

        Int64 fft_length_real = 8192;

        Int32 auto_num = 0;

        Double auto_span;
        Double auto_centerfreq;

        internal MultiDomainProcess()
        {

        }

        /// <summary>
        /// 开机初始化需要配置的内容
        /// </summary>
        internal void Init()
        {
            //Controller_RadioFrequency_Standard();

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_RF_select, 0x0);//处理板粗时频开关
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_enable_set, 0);
            Int32 channelid = (Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId);
            var isChannelActive = Hd.UIMessage?.RadioFrequency?[channelid].Active ?? false;
            if (isChannelActive != true)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_en, 0x0);
                return;
            }
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_rst, 0x1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_rst, 0x0);

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_data_select, 0x0002);//DDR读取之前先将数据类型选为FFT后的re和im数据
        }

        /// <summary>
        /// 配置DDR，执行FFT或者STFT，读IQ数据，并解析
        /// </summary>
        internal void Run()
        {
            if (Hd.UIMessage?.MultiDomain?.Active == true)
            {
                try
                {
                    ReadAcqData();
                }
                catch (Exception e) 
                {
                    ;
                }
                
                var tmp = Hd.UIMessage?.MultiDomain?.SpanForTimeFreq ?? 0;
                tmp = Hd.UIMessage?.MultiDomain?.TimeScaleForTimeFreq ?? 0;
            }
            else
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_enable_set, 0);//选择多域路径
            }
        }

        /// <summary>
        /// 用户配置发生变化时执行
        /// </summary>
        internal void PropertyChanged()
        {
            CtrlRadioFrequency_Standard.MD8G_RFCH_SendASCII_Config();//配置射频通道
            CtrlRadioFrequency_Standard.CtrlConfigSTFT();
            CtrlRadioFrequency_Standard.CtrlEnableSTFT();
            CtrlRadioFrequency_Standard.CtrlDDS();
            CtrlRadioFrequency_Standard.CtrlDDCDecimation();
            CtrlRadioFrequency_Standard.CtrlFreTransLength();
            Controller_MultiDomain_Standard.ConfigWindow();
        }
        void configure_window(Int32 FFTLength, RFWindowType WindowType)
        {
            //return;
            List<double> coefficient = CtrlRadioFrequency_Standard.GetWindowCoefficient(FFTLength, WindowType).ToList();
            for (int i = 0; i < coefficient.Count; i++)
            {
                byte[] floatBytes = BitConverter.GetBytes((float)coefficient[i]);
                UInt16 low = (UInt16)(floatBytes[1] << 8 & 0xff00 | floatBytes[0] & 0x00ff);
                UInt16 high = (UInt16)(floatBytes[3] << 8 & 0xff00 | floatBytes[2] & 0x00ff);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_coefficient_datain_h16, high);//暂时是写死的矩形窗lhc
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_coefficient_datain_l16, low); //暂时是写死的矩形窗lhc
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_Coefficient_Data_WREN, 0);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_Coefficient_Data_WREN, 1);

            }
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_Coefficient_Data_WREN, 0);
        }


        /// <summary>
        /// 获取各种解析后的数据
        /// </summary>
        /// <param name="param"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal String TryGetData(Object param, out Object? data)
        {
            if (param is MultiDomainDataTypeEnum)
            {
                switch ((MultiDomainDataTypeEnum)param)
                {
                    case MultiDomainDataTypeEnum.IFFT:
                        //data = new List<Double>() { 1, 2, 3, 4, 5 };
                        data = AcqedDataPool.RFData.FFTDataI;
                        return String.Empty;

                    case MultiDomainDataTypeEnum.QFFT:
                        data = AcqedDataPool.RFData.FFTDataQ;
                        return String.Empty;

                    case MultiDomainDataTypeEnum.AmpVSTime:
                        data = AcqedDataPool.RFData.AmpVSTimeData;
                        return String.Empty;

                    case MultiDomainDataTypeEnum.PhaseVSTime:
                        data = AcqedDataPool.RFData.PhaseVSTimeData;
                        return String.Empty;

                    case MultiDomainDataTypeEnum.Spectrogram:
                        data = AcqedDataPool.RFData.SpectrogramData;
                        return String.Empty;

                    case MultiDomainDataTypeEnum.WfmParams:
                        data = new WfmMdInfo
                        {
                            SampleRateHardware = samplingratehardware,
                            RBWHardware = rbwhardware,
                            SpanHardware = spanhardware,

                            SpanSync = spansync,
                            FFTLengthSync = fftlengthsync,
                            SpanParamTuning = spanparamtuning,
                            CenterFreqParamTuning = centerfreqparamtuning,
                            StateParamTuning = stateparamtuning,
                        };
                        return String.Empty;
                }
            }
            data = null;
            return String.Empty;
        }

        private UInt32 _RoughSpecExcutedCnt = 0;
        internal bool ReadAcqData()
        {
           // HdIO.WriteReg(PcieBdReg.W.Xdma_FifoProgFullThreshH, 0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_en, 0x1);

            HdIO.WriteReg(PcieBdReg.W.Xdma_FifoProgFullThreshL, 1 * 1024);//wby 251225 

            HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, 0x3);
            HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkmux_select, 0x3);
            Int32 channelid = (Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId);
            Boolean RoughSpecON = Hd.UIMessage?.RadioFrequency?[channelid].RoughSpecON ?? false;
            //Boolean RoughSpecON = _RoughSpecExcutedCnt == (Hd.UIMessage?.MultiDomain?.RoughSpecCnt ?? 0);
            //_RoughSpecExcutedCnt = Hd.UIMessage?.MultiDomain?.RoughSpecCnt ?? 0;

            //var isChannelActive = Hd.UIMessage?.RadioFrequency?[channelid].Active ?? false;
            //if (isChannelActive != true)
            //{
            //    SamplingRateByus = 1.0;
            //    return false;
            //}

            Boolean IsSynchronizationEnable = Hd.UIMessage?.MultiDomain?.SynchronizationEnable ?? false;    // 多域同步
            Boolean IsParameterTuningEnable = Hd.UIMessage?.MultiDomain?.ParameterTuningEnable ?? false;    // 自适应参数调整

            //ChannelId source = Hd.UIMessage?.RadioFrequency?[channelid].Source ?? ChannelId.C1;
            ChannelId source = Hd.UIMessage?.MultiDomain?.Source ?? ChannelId.C1;
            UInt32 DDCDiscardNum = 512;
            //Int32 channelDotCount = Hd.UIMessage?.RadioFrequency?[channelid].FFTLength ?? Constants.CHNL_DATA_NUM;
            Int32 channelDotCount = (int)(Hd.UIMessage?.MultiDomain?.FFTLength ?? Constants.CHNL_DATA_NUM);
            channelDotCount = 10240;
            RFWindowType windowType = Hd.UIMessage?.MultiDomain?.WindowType ?? RFWindowType.Hann;
            Int32 fftLength = (int)(Hd.UIMessage?.MultiDomain?.FFTLength ?? 1024);
            Int64 stftLength = Hd.UIMessage?.MultiDomain?.STFTLength ?? 1024;
            Int64 stftstep = Hd.UIMessage?.MultiDomain?.STFTStep ?? 1024;
            Int64 stftTimes = (stftLength < fftLength) ? 1 : (stftLength - fftLength) / stftstep + 1;
            Int64 span = (long)(Hd.UIMessage?.MultiDomain?.SpanByHz ?? 8_000_000_000);
            Int64 centerFrequency = Hd.UIMessage?.MultiDomain?.CenterFreqByHz ?? 10_000_000_000;
            Int64 spanHardware = Controller_RadioFrequency_Standard.SpanHardware;

            Double SampleRateHardware = AbstractAcquirer_RadioFrequency.GetRFTranslateSampleRate(span, source);

            //Boolean AVTON = Hd.UIMessage?.MultiDomain?.FigureActive[MultiDomainFigureEnum.PhaseVsTime] ?? false;
            Boolean AVTON = Hd.UIMessage?.MultiDomain?.AVTON ?? false;
            Boolean PVTON = Hd.UIMessage?.MultiDomain?.PVTON ?? false;
            Boolean FVTON = Hd.UIMessage?.MultiDomain?.FVTON ?? false;
            var scale = AbstractAcquirer_RadioFrequency.GetRFHDScale(span, source);
            HdMessage.AnalogOptions analogParameters = Hd.UIMessage!.Analog![(int)GetAnalogChnlId[source]];
            bool ret = false;
            bool DisplayRoughSpec = false;
            double wh = GetWindowCoeComp(fftLength, windowType);
            int frame_num = 512;
            int points_num = 2048;



            Double zoomstart = Hd.UIMessage?.MultiDomain?.ZoomStart ?? -25;
            Double zoomlength = Hd.UIMessage?.MultiDomain?.ZoomLength ?? 50;
            if (IsParameterTuningEnable)
            {
                if (auto_num == 20)
                {
                    ReadFrameData_Auto();
                    stateparamtuning++;

                    #region 解析
                    List<Double> ffti = new();
                    List<Double> fftq = new();
                    List<double> ampdata = new List<double>();

                    for (int i = 0; i < frame_num; i++)
                    {
                        for (int dotIndex = points_num * i; dotIndex < points_num * (i + 1); dotIndex++)
                        {
                            byte[] bytes = new byte[4];
                            byte[] bytesi = new byte[4];
                            {
                                bytes[0] = DMATotalData[8 * (dotIndex) + 0];
                                bytes[1] = DMATotalData[8 * (dotIndex) + 1];
                                bytes[2] = DMATotalData[8 * (dotIndex) + 2];
                                bytes[3] = DMATotalData[8 * (dotIndex) + 3];

                                bytesi[0] = DMATotalData[8 * (dotIndex) + 4];
                                bytesi[1] = DMATotalData[8 * (dotIndex) + 5];
                                bytesi[2] = DMATotalData[8 * (dotIndex) + 6];
                                bytesi[3] = DMATotalData[8 * (dotIndex) + 7];
                            }

                            float result = BitConverter.ToSingle(bytes, 0);
                            float resulti = BitConverter.ToSingle(bytesi, 0);

                            ffti.Add(resulti);
                            fftq.Add(result);

                            Complex complex = new Complex(result, resulti);
                            var amp = complex.Magnitude;
                            ampdata.Add(amp);
                        }
                    }
                    //SaveDataToFile(ffti, "D:\\data\\ffti.txt");
                    //SaveDataToFile(fftq, "D:\\data\\fftq.txt");
                    RescaleAmp(ampdata);
                    Double max_value = Double.MinValue;
                    Double min_value = Double.MaxValue;
                    if (ampdata.Count == frame_num * points_num)
                    {
                        (min_value, max_value) = GetDataMax(ampdata, frame_num, points_num);
                    }
                    if (((int)(max_value - min_value) * 80_000_000_000 / 2048) > 0 && ((int)(max_value - min_value) * 80_000_000_000 / 2048) < 20_000_000_000)
                    {
                        spanparamtuning = (int)(max_value - min_value) * 80_000_000_000 / 2048;
                        centerfreqparamtuning = (int)(max_value + min_value) * 80_000_000_000 / 2048 / 2;
                    }
                    //if (((int)(max_value - min_value) * 20_000_000_000 / 2048) > 0 && ((int)(max_value - min_value) * 20_000_000_000 / 2048) < 8_000_000_000)
                    //{
                    //    spanparamtuning = (int)(max_value - min_value) * 20_000_000_000 / 2048;
                    //    centerfreqparamtuning = (int)(max_value + min_value) * 20_000_000_000 / 2048 / 2;
                    //}//20Gsps
                    #endregion
                    auto_num = 0;
                }
            }

            if (Hd.UIMessage?.MultiDomain?.SpecON ?? false)
            {
                ReadFrameDataTVF();
                List<Double> ffti = new();
                List<Double> fftq = new();
                AcqedDataPool.RFData.SpectrogramData.Clear();

                for (int i = 0; i < 512; i++)
                {
                    List<Double> specdata = new();
                    List<Double> front_specdata = new();
                    List<Double> after_specdata = new();

                    for (int dotIndex = 512 * i; dotIndex < 512 * (i + 1); dotIndex++)
                    {
                        byte[] bytes = new byte[4];
                        byte[] bytesi = new byte[4];
                        {
                            bytes[0] = DMATotalData[8 * (dotIndex) + 0];
                            bytes[1] = DMATotalData[8 * (dotIndex) + 1];
                            bytes[2] = DMATotalData[8 * (dotIndex) + 2];
                            bytes[3] = DMATotalData[8 * (dotIndex) + 3];

                            bytesi[0] = DMATotalData[8 * (dotIndex) + 4];
                            bytesi[1] = DMATotalData[8 * (dotIndex) + 5];
                            bytesi[2] = DMATotalData[8 * (dotIndex) + 6];
                            bytesi[3] = DMATotalData[8 * (dotIndex) + 7];
                        }

                        float result = BitConverter.ToSingle(bytes, 0);
                        float resulti = BitConverter.ToSingle(bytesi, 0);

                        ffti.Add(resulti);
                        fftq.Add(result);

                        Complex complex = new Complex(result, resulti);
                        var amp = complex.Magnitude;

                        AcqedDataPool.RFData.SpectrogramData.Add(amp);
                        //specdata.Add(amp);
                    }
                    //front_specdata = specdata.GetRange(0, (int)(0.5 * specdata.Count));
                    //after_specdata = specdata.GetRange((int)(0.5 * specdata.Count), specdata.Count - (int)(0.5 * specdata.Count));
                    //after_specdata.AddRange(front_specdata);
                    //AcqedDataPool.RFData.SpectrogramData.AddRange(specdata);
                }
            }

            int ExecuteCnt = RoughSpecON ? 2 : 1;
            for (int m = 0; m < ExecuteCnt; m++)
            {
                if (!RoughSpecON)
                {
                    //ReadFrameDataXi(stftTimes, fftLength);
                    cnt = 0;
                    //0508,改成800m是为了调试把这段旁路  spanHardware == 20_000_000_000
                    ret = ReadFrameDataFromNormal(stftTimes, fftLength);
                    //ReadFrameDataTVF();
                    if (!ret)
                    {
                        SamplingRateByus = 1.0;
                        return ret;
                    }
                }
                else
                {
                    if (cnt == 0)
                    {
                        AcqedDataPool.RFData.RoughSpecData.Clear();
                        AcqedDataPool.RFData.FineSpecData.Clear();
                    }
                    ReadFrameDataCu();
                }

                if (DisplayRoughSpec)
                {
                    ReadFrameDataXi(stftTimes, fftLength);
                    //DisplayRoughSpec = false;
                }
                HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkdemux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DataPath_acq_linkmux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
                HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkmux_select, 0x0);

                bDataVaild = false;
                bool bOk = false;
                if (Hd.UIMessage == null)
                {
                    SamplingRateByus = 1.0;
                    return false;
                }
                AcquingParameters.CloneTo(AcquedParameters);
                if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen)
                {
                    SamplingRateByus = 1.0;
                    return AcqAnalogChannelSimulateWaveform();
                }
                #region 数据解析  
                #region 数据拆分  

                //数据解析
                Monitor.Enter(RFData.UpdateDataLock);

                #endregion

                //int channelDotCount =10000;
                List<UInt64> dataTmp = new List<UInt64>();
                AcqedDataPool.RFData.Data.Clear();
                AcqedDataPool.RFData.PhaseVSFrequencyData.Clear();
                AcqedDataPool.RFData.PhaseVSTimeData.Clear();
                AcqedDataPool.RFData.AmpVSTimeData.Clear();

                AcqedDataPool.RFData.FFTDataI.Clear();
                AcqedDataPool.RFData.FFTDataQ.Clear();

                ////数据解析并存入文件

                AcqedDataPool.RFData.WfmSampleInfo.SampleIntervalByus = 1 / AbstractAcquirer_RadioFrequency.GetRFTranslateSampleRate(span, source);

                if (!true)//IQ 
                {
                    List<Double> dataI = new List<Double>();
                    List<Double> dataQ = new List<Double>();
                    for (int dotIndex = 0; dotIndex < channelDotCount; dotIndex++)
                    {
                        byte[] byteI = new byte[4];
                        byte[] byteQ = new byte[4];
                        for (int i = 0; i < byteI.Length; i++)
                        {
                            byteI[i] = DMATotalData[8 * dotIndex + i];
                            byteQ[i] = DMATotalData[8 * dotIndex + i + 4];
                        }
                        double floatI = BitConverter.ToSingle(byteI, 0);
                        double floatQ = BitConverter.ToSingle(byteQ, 0);
                        floatI = floatI / scale.DDCGain;
                        floatQ = floatQ / scale.DDCGain;
                        dataI.Add(floatI);
                        dataQ.Add(floatQ);
                    }
                    if (dataI != null && dataQ != null)
                    {
                        AcqedDataPool.RFData.Data.AddRange(dataI);
                        AcqedDataPool.RFData.Data.AddRange(dataQ);
                        bDataVaild = true;
                    }
                }
                else
                {
                    Double real_result;
                    Double real_resulti;
                    #region 解析原始I、Q数据，计算幅度域，相位域
                    if (PVTON || AVTON || FVTON)
                    {
                        channelDotCount = IQDMAData.Length / 8;
                        if (channelDotCount > (Hd.UIMessage?.MultiDomain?.FFTLength ?? 1024))
                        {
                            //channelDotCount = (Int32)(Hd.UIMessage?.MultiDomain?.FFTLength ?? 1024);
                            channelDotCount = (Int32)actualfftlength;
                        }
                        List<double> pvtdata = new List<double>();
                        List<double> avtdata = new List<double>();
                        HdMessage.AnalogOptions AnalogParameters = Hd.UIMessage!.Analog![(int)GetAnalogChnlId[source]];
                        double AnalogADCReflevel = GetAnalogADCRef[(int)GetAnalogChnlId[source], AnalogParameters.ScaleIndex];
                        for (int dotIndex = 0; dotIndex < channelDotCount; dotIndex++)
                        {
                            byte[] bytes = new byte[4];
                            byte[] bytesi = new byte[4];
                            {
                                bytes[0] = IQDMAData[8 * dotIndex + 0];
                                bytes[1] = IQDMAData[8 * dotIndex + 1];
                                bytes[2] = IQDMAData[8 * dotIndex + 2];
                                bytes[3] = IQDMAData[8 * dotIndex + 3];

                                bytesi[0] = IQDMAData[8 * dotIndex + 4];
                                bytesi[1] = IQDMAData[8 * dotIndex + 5];
                                bytesi[2] = IQDMAData[8 * dotIndex + 6];
                                bytesi[3] = IQDMAData[8 * dotIndex + 7];
                            }
                            float result = BitConverter.ToSingle(bytes, 0);
                            float resulti = BitConverter.ToSingle(bytesi, 0);



                            Complex complex = new Complex(result, resulti);
                            var amp = complex.Magnitude;
                            var phase = GetPhase(result, resulti);
                            #region 将幅度值转换为模拟电压值,单位V
                            amp = amp / scale.DDCGain;
                            amp = amp / 3200;
                            amp = amp * analogParameters.Scale * 10 / 1000;
                            amp = amp * 2;
                            #endregion
                            avtdata.Add(amp);
                            pvtdata.Add(phase);
                        }


                        AcqedDataPool.RFData.AmpVSTimeData.AddRange(avtdata);
                        AcqedDataPool.RFData.PhaseVSTimeData.AddRange(pvtdata);

                        bDataVaild = true;

                    }
                    #endregion
                    #region 解析多帧FFT数据，计算频域，时频域，相频域
                    channelDotCount = DMATotalData.Length / 8;
                    if (channelDotCount > fftLength)
                    {
                        if (RoughSpecON && !DisplayRoughSpec)
                        //if (IsParameterTuningEnable)
                        {
                            channelDotCount = 2048;
                            stftTimes = 512;
                        }
                        else if (DisplayRoughSpec)
                        {
                            channelDotCount = 512;
                            stftTimes = 512;
                        }
                        else
                        {
                            channelDotCount = fftLength;
                        }
                    }
                    {
                        List<double> ampdata = new List<double>();
                        List<double> phasedata = new List<double>();

                        List<Double> ffti = new();
                        List<Double> fftq = new();


                        //List<List<double>> roughspecdata = new List<List<double>>();

                        double Freq = 0;
                        double amp_pro;
                        for (int frame = 0; frame < stftTimes; frame++)
                        {
                            for (int dotIndex = channelDotCount * frame; dotIndex < channelDotCount * (frame + 1); dotIndex++)
                            {
                                byte[] bytes = new byte[4];
                                byte[] bytesi = new byte[4];
                                {
                                    bytes[0] = DMATotalData[8 * (dotIndex) + 0];//2
                                    bytes[1] = DMATotalData[8 * (dotIndex) + 1];//3
                                    bytes[2] = DMATotalData[8 * (dotIndex) + 2];//0
                                    bytes[3] = DMATotalData[8 * (dotIndex) + 3];//1

                                    bytesi[0] = DMATotalData[8 * (dotIndex) + 4];//6
                                    bytesi[1] = DMATotalData[8 * (dotIndex) + 5];//7
                                    bytesi[2] = DMATotalData[8 * (dotIndex) + 6];//4
                                    bytesi[3] = DMATotalData[8 * (dotIndex) + 7];//5
                                }

                                float result = BitConverter.ToSingle(bytes, 0);
                                float resulti = BitConverter.ToSingle(bytesi, 0);



                                DataPostProcess(result, scale.DDCGain, out real_result, wh, Freq);//将解析后的数据进行处理，包括定标，校准
                                DataPostProcess(resulti, scale.DDCGain, out real_resulti, wh, Freq);//将解析后的数据进行处理，包括定标，校准

                                ffti.Add(real_resulti);
                                fftq.Add(real_result);



                                Complex complex = new Complex(result, resulti);
                                var amp = complex.Magnitude;
                                var phase = GetPhase(result, resulti);
                                if (centerFrequency > 500_000_000)
                                    Freq = 500_000_000 - ((double)(dotIndex % channelDotCount) - (double)(dotIndex % channelDotCount > fftLength / 2 ? fftLength : 0)) * scale.SampleRate / fftLength;
                                else
                                    Freq = ((double)(dotIndex % channelDotCount) - (double)(dotIndex % channelDotCount > fftLength / 2 ? fftLength : 0)) * scale.SampleRate / fftLength + 500_000_000;
                                DataPostProcess(amp, scale.DDCGain, out amp_pro, wh, Freq);//将解析后的数据进行处理，包括定标，校准
                                ampdata.Add(amp_pro);
                                phasedata.Add(phase);
                            }

                            // 细时频！！！！！！！！！！
                            if (DisplayRoughSpec)
                            {
                                AcqedDataPool.RFData.FineSpecData.AddRange(ampdata);
                            }
                            {
                                List<double> datapre = new List<double>();
                                List<double> dataend = new List<double>();
                                if (source != ChannelId.RF ||
                                (source == ChannelId.RF && centerFrequency <= 500_000_000))
                                {
                                    for (int i = 0; i < (ampdata.Count / 2); i++)
                                    {
                                        datapre.Add(ampdata[i]);
                                        dataend.Add(ampdata[ampdata.Count / 2 + i]);
                                    }
                                }
                                else
                                {
                                    dataend.Add(ampdata[ampdata.Count - 1]);
                                    for (int i = 0; i < (ampdata.Count / 2); i++)
                                    {
                                        dataend.Add(ampdata[ampdata.Count / 2 - i - 1]);
                                        datapre.Add(ampdata[ampdata.Count - i - 1]);
                                    }
                                    datapre.RemoveAt(ampdata.Count / 2 - 1);
                                }
                                ampdata.Clear();
                                ampdata.AddRange(dataend);
                                ampdata.AddRange(datapre);

                                if (RoughSpecON == true && AcqedDataPool.RFData.RoughSpecData.Count <= 262144)
                                {
                                    //List<double> copy = dataend.Select(item => item).ToList();
                                    int datapre_lenght = datapre.Count / 2;
                                    AcqedDataPool.RFData.RoughSpecData.AddRange(datapre.GetRange(0, datapre_lenght));

                                }

                                //roughspecdata = AcqedDataPool.RFData.RoughSpecData;
                                //if (RoughSpecON == true && AcqedDataPool.RFData.RoughSpecData.Count == 262144)
                                ////{ test++; }
                                //for (int k = 0; k < 3; k++)
                                //{
                                //    TryExecuteTFDetectRF();
                                //}
                                Boolean cutSuccessed = false;
                                if (spanHardware == 20_000_000_000)
                                {
                                    //cutSuccessed = FFTDataCutE(ampdata, fftLength, centerFrequency, span, spanHardware, SampleRateHardware, out List<Double>? dataAfterCut);
                                    ////cutSuccessed = FFTDataCut(ampdata, fftLength, span, spanHardware, out List<Double>? dataAfterCut);
                                    //if (cutSuccessed && dataAfterCut != null)
                                    //{
                                    //AcqedDataPool.RFData.Data.AddRange(dataAfterCut);
                                    //bDataVaild = true;

                                    ////AcqedDataPool.RFData.Data.AddRange(datapre);
                                    ////SaveDataToFile(dataAfterCut);
                                    //}
                                }
                                else
                                {
                                    ////cutSuccessed = FFTDataCut(ampdata, fftLength, span, spanHardware, out List<Double>? dataAfterCut);
                                    //cutSuccessed = FFTDataCutE(ampdata, fftLength, centerFrequency, span, spanHardware, SampleRateHardware, out List<Double>? dataAfterCut);
                                    //if (cutSuccessed && dataAfterCut != null)
                                    //{
                                    //    AcqedDataPool.RFData.Data.AddRange(dataAfterCut);
                                    //    bDataVaild = true;
                                    //    //SaveDataToFile(dataAfterCut);
                                    //}
                                }
                                ampdata.Clear();
                                datapre.Clear();
                                dataend.Clear();
                            }
                            {
                                List<double> datapre = new List<double>();
                                List<double> dataend = new List<double>();
                                if (source != ChannelId.RF ||
                                (source == ChannelId.RF && centerFrequency <= 500_000_000))
                                {
                                    for (int i = 0; i < (phasedata.Count / 2); i++)
                                    {
                                        datapre.Add(phasedata[i]);
                                        dataend.Add(phasedata[phasedata.Count / 2 + i]);
                                    }
                                }
                                else
                                {
                                    dataend.Add(phasedata[phasedata.Count - 1]);
                                    for (int i = 0; i < (phasedata.Count / 2); i++)
                                    {
                                        dataend.Add(phasedata[phasedata.Count / 2 - i - 1]);
                                        datapre.Add(phasedata[phasedata.Count - i - 1]);
                                    }
                                    datapre.RemoveAt(phasedata.Count / 2 - 1);
                                }
                                phasedata.Clear();
                                phasedata.AddRange(dataend);
                                phasedata.AddRange(datapre);
                                Boolean cutSuccessed = false;
                                if (spanHardware == 20_000_000_000)//8_000_000_000
                                {
                                    cutSuccessed = FFTDataCutE(datapre, fftLength, centerFrequency, span, spanHardware, SampleRateHardware, out List<Double>? dataAfterCut);
                                    //cutSuccessed = DataPostProcess(dataAfterCut, scale.DDCGain, out List<Double>? dataAfterProcess);
                                    if (cutSuccessed && dataAfterCut != null)
                                    {
                                        AcqedDataPool.RFData.PhaseVSFrequencyData.AddRange(dataAfterCut);
                                        bDataVaild = true;
                                        //SaveDataToFile(dataAfterCut);
                                    }
                                }
                                else
                                {
                                    cutSuccessed = FFTDataCut(phasedata, fftLength, span, spanHardware, out List<Double>? dataAfterCut);
                                    //cutSuccessed = DataPostProcess(dataAfterCut, scale.DDCGain, out List<Double>? dataAfterProcess);
                                    if (cutSuccessed && dataAfterCut != null)
                                    {
                                        AcqedDataPool.RFData.PhaseVSFrequencyData.AddRange(dataAfterCut);
                                        bDataVaild = true;
                                        //SaveDataToFile(dataAfterCut);
                                    }
                                }
                                phasedata.Clear();
                                datapre.Clear();
                                dataend.Clear();
                            }
                        }


                        if (RoughSpecON == true && AcqedDataPool.RFData.RoughSpecData.Count == 262144 && !DisplayRoughSpec)
                        {
                            SaveDataToFile(ffti, "d:\\data\\ffti.txt");
                            SaveDataToFile(fftq, "d:\\data\\fftq.txt");
                            //{ test++; }
                            for (int k = 0; k < 1; k++)
                            {
                                //DisplayRoughSpec = TryExecuteTFDetectRF();
                            }
                        }

                        if (DisplayRoughSpec == true && AcqedDataPool.RFData.FineSpecData.Count == 262144)
                        {
                            //SaveDataToFile(ffti, "c:\\matlab\\ffti.txt");
                            //SaveDataToFile(fftq, "c:\\matlab\\fftq.txt");
                            for (int k = 0; k < 1; k++)
                            {
                                //DisplayRoughSpec = TryExecuteFineSpec();
                            }
                            DisplayRoughSpec = false;
                        }


                        // 截断数据
                        List<Double> aftercutfftq = new();
                        List<Double> aftercutffti = new();
                        List<Double> Beforecutfftq = new();
                        List<Double> Beforecutffti = new();
                        //mry test
                        Beforecutfftq = fftq.GetRange(0, fftq.Count);
                        Beforecutffti = ffti.GetRange(0, ffti.Count);

                        List<Double> frontfftq = new();
                        List<Double> frontffti = new();
                        Double CutCoefficient = 1 - (span / 2 / SampleRateHardware);
                        samplingratehardware = SampleRateHardware;
                        rbwhardware = (Double)(GetRFTranslateSampleRate(span, source) / fftLength);
                        //CutCoefficient = 1;
                        //mry



                        frontfftq = fftq.GetRange(0, (int)(CutCoefficient * fftq.Count));
                        frontffti = ffti.GetRange(0, (int)(CutCoefficient * fftq.Count));
                        aftercutfftq = fftq.GetRange((int)(CutCoefficient * fftq.Count), fftq.Count - (int)(CutCoefficient * fftq.Count));
                        aftercutffti = ffti.GetRange((int)(CutCoefficient * fftq.Count), fftq.Count - (int)(CutCoefficient * fftq.Count));
                        //frontfftq = fftq.GetRange(0, (int)(0.875 * fftq.Count));
                        //frontffti = ffti.GetRange(0, (int)(0.875 * fftq.Count));
                        //aftercutfftq = fftq.GetRange((int)(0.875 * fftq.Count), fftq.Count - (int)(0.875 * fftq.Count));
                        //aftercutffti = ffti.GetRange((int)(0.875 * fftq.Count), fftq.Count - (int)(0.875 * fftq.Count));
                        aftercutfftq.AddRange(frontfftq);
                        aftercutffti.AddRange(frontffti);

                        AcqedDataPool.RFData.FFTDataI.AddRange(aftercutffti);
                        AcqedDataPool.RFData.FFTDataQ.AddRange(aftercutfftq);

                        //AcqedDataPool.RFData.FFTDataI.AddRange(ffti);
                        //AcqedDataPool.RFData.FFTDataQ.AddRange(fftq);

                        //if (RoughSpecON == true)
                        //    for (int i = 0; i < 2; i++)
                        //    {
                        //        SaveData();
                        //    }
                    }
                    #endregion
                }


                Monitor.Exit(RFData.UpdateDataLock);
                #endregion
            }

            //if (Hd.UIMessage?.MultiDomain?.SpecON ?? false)
            //{
            //    ReadFrameDataTVF();
            //    List<Double> ffti = new();
            //    List<Double> fftq = new();
            //    AcqedDataPool.RFData.SpectrogramData.Clear();

            //    for (int i = 0; i < 512; i++)
            //    {
            //        for (int dotIndex = 512 * i; dotIndex < 512 * (i + 1); dotIndex++)
            //        {
            //            byte[] bytes = new byte[4];
            //            byte[] bytesi = new byte[4];
            //            {
            //                bytes[0] = DMATotalData[8 * (dotIndex) + 2];
            //                bytes[1] = DMATotalData[8 * (dotIndex) + 3];
            //                bytes[2] = DMATotalData[8 * (dotIndex) + 0];
            //                bytes[3] = DMATotalData[8 * (dotIndex) + 1];

            //                bytesi[0] = DMATotalData[8 * (dotIndex) + 6];
            //                bytesi[1] = DMATotalData[8 * (dotIndex) + 7];
            //                bytesi[2] = DMATotalData[8 * (dotIndex) + 4];
            //                bytesi[3] = DMATotalData[8 * (dotIndex) + 5];
            //            }

            //            float result = BitConverter.ToSingle(bytes, 0);
            //            float resulti = BitConverter.ToSingle(bytesi, 0);

            //            ffti.Add(resulti);
            //            fftq.Add(result);

            //            Complex complex = new Complex(result, resulti);
            //            var amp = complex.Magnitude;
            //            AcqedDataPool.RFData.SpectrogramData.Add(amp);
            //        }
            //    }
            //    //SaveDataToFile(ffti, "d:\\data\\ffti.txt");
            //    //SaveDataToFile(fftq, "d:\\data\\fftq.txt");
            //}


            SamplingRateByus = 1.0;

            #region 注释
            //if (IsParameterTuningEnable)
            //{
            //    if (auto_num == 50)
            //    {
            //        ReadFrameData_Auto();
            //        stateparamtuning++;

            //        #region 解析
            //        List<Double> ffti = new();
            //        List<Double> fftq = new();
            //        List<double> ampdata = new List<double>();

            //        for (int i = 0; i < frame_num; i++)
            //        {
            //            for (int dotIndex = points_num * i; dotIndex < points_num * (i + 1); dotIndex++)
            //            {
            //                byte[] bytes = new byte[4];
            //                byte[] bytesi = new byte[4];
            //                {
            //                    bytes[0] = DMATotalData[8 * (dotIndex) + 2];
            //                    bytes[1] = DMATotalData[8 * (dotIndex) + 3];
            //                    bytes[2] = DMATotalData[8 * (dotIndex) + 0];
            //                    bytes[3] = DMATotalData[8 * (dotIndex) + 1];

            //                    bytesi[0] = DMATotalData[8 * (dotIndex) + 6];
            //                    bytesi[1] = DMATotalData[8 * (dotIndex) + 7];
            //                    bytesi[2] = DMATotalData[8 * (dotIndex) + 4];
            //                    bytesi[3] = DMATotalData[8 * (dotIndex) + 5];
            //                }

            //                float result = BitConverter.ToSingle(bytes, 0);
            //                float resulti = BitConverter.ToSingle(bytesi, 0);

            //                ffti.Add(resulti);
            //                fftq.Add(result);

            //                Complex complex = new Complex(result, resulti);
            //                var amp = complex.Magnitude;
            //                ampdata.Add(amp);
            //            }
            //        }
            //        RescaleAmp(ampdata);
            //        Double max_value = Double.MinValue;
            //        Double min_value = Double.MaxValue;
            //        if (ampdata.Count == frame_num * points_num)
            //        {
            //            (min_value, max_value) = GetDataMax(ampdata, frame_num, points_num);
            //        }
            //        if (((int)(max_value - min_value) * 20_000_000_000 / 2048) > 0 && ((int)(max_value - min_value) * 20_000_000_000 / 2048) < 8_000_000_000)
            //        {
            //            spanparamtuning = (int)(max_value - min_value) * 20_000_000_000 / 2048;
            //            centerfreqparamtuning = (int)(max_value + min_value) * 20_000_000_000 / 2048 / 2;
            //        }
            //        #endregion
            //        auto_num = 0;
            //    }
            //}
            #endregion
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_en, 0);

            return true;

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
        public (Double, Double) GetDataMax(List<double> data, int data_row, int data_col)
        {
            int target_row = data_row;
            int target_col = (int)(data_col * 0.25);//wby0311
            Double[,] temp = new double[target_row, target_col];
            Double max_value = Double.MinValue;
            Double min_value = Double.MaxValue;
            double maxval = Double.MinValue;

            for (int i = 1; i < target_row; i++)
            {

                for (int j = 1; j < target_col; j++)
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
                double y = maxval - 15;
                List<int> indices = new List<int>();
                for (int j = 1; j < target_col; j++)
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

        void sleep_time(int Ext, uint FifoDepth)
        {
            if (Ext < 81)//401,wby260317
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, FifoDepth - 10);
                HdIO.Sleep(15 * (int)FifoDepth / 1024);
            }
            else if (Ext > 80 && Ext < 401)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, FifoDepth);
                HdIO.Sleep(20 * (int)FifoDepth / 1024);
            }
            else if (Ext > 400 && Ext < 801)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, FifoDepth - 1);
                HdIO.Sleep(50 * (int)FifoDepth / 1024);
            }
            else if (Ext > 800 && Ext < 1601)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, FifoDepth - 1);
                HdIO.Sleep(70 * (int)FifoDepth / 1024);
            }
            else if (Ext > 1600 && Ext < 4001)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, FifoDepth - 1);
                HdIO.Sleep(180 * (int)FifoDepth / 1024);
            }
            else if (Ext > 4000 && Ext < 8001)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, FifoDepth - 1);
                HdIO.Sleep(320 * (int)FifoDepth / 1024);
            }
            else if (Ext > 8000 && Ext < 40001)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, FifoDepth - 1);
                HdIO.Sleep(400 * (int)FifoDepth / 1024);
            }
            else if (Ext > 40000 && Ext < 80001)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, FifoDepth - 1);
                HdIO.Sleep(800 * (int)FifoDepth / 1024);
            }
            else if (Ext > 80000 && Ext < 125001)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, FifoDepth - 1);
                HdIO.Sleep(1250 * (int)FifoDepth / 1024);
            }
            else if (Ext > 125000)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, FifoDepth - 1);
                HdIO.Sleep(2500 * (int)FifoDepth / 1024);
            }
            return;
        }

        //void para_mixing()
        //{
        //    double fs_sys = Constants.SAMPLING_RATE / Math.Pow(2, (int)Hd.UIMessage?.Precision?.AnaChnlBitWidth - 12);
        //    var span = Hd.UIMessage?.MultiDomain?.SpanByHz ?? 1000;
        //    double fchardware = -(Hd.UIMessage?.MultiDomain?.CenterFreqByHz ?? 500);
        //    ChannelId source = Hd.UIMessage?.MultiDomain?.Source ?? ChannelId.C1;
        //    if ((source == ChannelId.C1 || source == ChannelId.C2 || source == ChannelId.C3 || source == ChannelId.C4) && source == ChannelId.RF)
        //    {
        //        fs_sys = Constants.SAMPLING_RATE;
        //        if (fchardware < -500_000_000)
        //        {
        //            fchardware = -GetValidSpan(span) / 2;
        //        }
        //    }
        //    double fchan = fs_sys / 80;
        //    double nph = 28;
        //    fchardware = fchardware + fs_sys;
        //    double ph_step_inside_chan_out_range = (double)(Math.Round(fchardware / fchan * Math.Pow(2, nph)));
        //    double ph_inside_chan_out_range = fchardware / fs_sys * Math.Pow(2, nph);
        //    UInt32 ph_to_FPGA;
        //    UInt32 ph_step_to_FPGA;
        //    ph_step_to_FPGA = (UInt32)(ph_step_inside_chan_out_range % Math.Pow(2, nph));
        //    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_Write20, 1);//DDC_80_en
        //    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_Write19, 0);//ph_finish
        //    for (int i = 0; i < 80; i++)
        //    {
        //        ph_to_FPGA = (UInt32)(Math.Round(ph_inside_chan_out_range * i) % Math.Pow(2, nph));

        //        Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_Write16, ph_to_FPGA);
        //        Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_Write15, ph_to_FPGA >> 16);
        //        Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_Write17, (uint)i);
        //        Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_Write18, 1);
        //        Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_Write18, 0);
        //    }
        //    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_Write16, ph_step_to_FPGA);
        //    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_Write15, ph_step_to_FPGA >> 16);
        //    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_Write17, 80);

        //    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_Write18, 1);
        //    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_Write18, 0);

        //    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_Write19, 1);
        //    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_Write19, 0);

        //    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 0);
        //    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_DDC_80_en, 1);
        //    for (int i = 0; i < 1; i++)
        //    {
        //        ph_to_FPGA = (UInt32)(Math.Round(ph_inside_chan_out_range * i) % Math.Pow(2, nph));

        //        HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_l, 0);
        //        HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_h, 0);
        //        HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_addr, (uint)i);
        //        HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 1);
        //        HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 0);
        //    }

        //    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_l, 0);
        //    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_h, 0);
        //    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_addr, 1);

        //    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 1);
        //    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 0);

        //    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 1);
        //    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 0);

        //    return;
        //}

        private double GetWindowCoeComp(Int32 fftLength, RFWindowType windowType)
        {
            List<double> ws = AbstractAcquirer_RadioFrequency.GetWindowCoefficient(fftLength, windowType).Select((x) => x * x).ToList();
            double wh = 0;
            for (int j = 0; j < ws.Count; j++)
            {
                wh += ws[j];
            }
            return wh;
        }

        internal Double ValidLength = 0;

        private Int64 _LastSpan = 0;

        private Double _LastSatrtPos = 0;
        private Double _LastLength = 0;
        private Int32 _UnchangedCnt = 0;

        private Int32 _SpanUnEqualCnt = 0;

        Int32 actualextnum;
        Double actualfftlength = 8192;

        internal Double GetAcutalFFTLength()
        {
            return actualfftlength;
        }

        internal (Double, Double) GetValidRect(Double startPosByns, Double lengthByns)
        {
            Int32 holdcnt = 1;
            if (startPosByns.Equals(_LastSatrtPos) && lengthByns.Equals(_LastLength))
            {
                if (_UnchangedCnt < holdcnt)
                    _UnchangedCnt++;
            }
            else
            {
                _UnchangedCnt = 0;
                _LastSatrtPos = startPosByns;
                _LastLength = lengthByns;
            }

            if (_UnchangedCnt < holdcnt)
            {
                return (startPosByns, lengthByns);
            }


            Int32 fftLength = (int)(Hd.UIMessage?.MultiDomain?.FFTLength ?? 1024);

            var scalebyus = Hd.UIMessage?.Timebase?.TmbScale ?? 0;
            var sumtimebyus = scalebyus * Constants.VIS_XDIVS_NUM;

            var trigpos = Hd.UIMessage?.Timebase?.TmbPosition ?? 0;

            var theorydotcnt = lengthByns * 1e6 / (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquedParameters?.PerDataByfs_AtDdr ?? 50_000);
            var theoryextramnum = theorydotcnt / (fftLength == 0 ? 8192 : fftLength);

            actualextnum = ExtGetRFSpanScaleTable(theoryextramnum).Ext;
            spansync = ExtGetRFSpanScaleTable(actualextnum).Span;
            actualfftlength = Math.Ceiling(theorydotcnt / actualextnum);
            var actualdotcnt = actualextnum * actualfftlength;
            //Int32 minrealdots = 0;
            var span = Hd.UIMessage?.MultiDomain?.SpanByHz ?? 1000;
            if (span != spansync)
            {
                if (_SpanUnEqualCnt < 10)
                {
                    _SpanUnEqualCnt++;
                    span = spansync;
                }
            }
            else
            {
                _SpanUnEqualCnt = 0;
            }

            ChannelId source = Hd.UIMessage?.MultiDomain?.Source ?? ChannelId.C1;
            var key = AbstractAcquirer_RadioFrequency.GetRFHDScale(span, source);
            if (key.Span != _LastSpan)
            {
                //var key_old = AbstractAcquirer_RadioFrequency.GetRFHDScale((Int64)_LastSpan, source);
                //minrealdots = key_old.Ext * fftLength / key.Ext + 1;
                _LastSpan = key.Span;
                actualdotcnt = key.Ext * actualfftlength;
                //actualdotcnt = key.Ext * minrealdots;

            }

            Double fre_rate = ((Hd.UIMessage?.MultiDomain?.SpanForTimeFreq ?? 0) / key.Span);
            var span_AVF = (span * fre_rate > 20_000_000_000) ? 20_000_000_000 : span * fre_rate;
            var key_AVF = AbstractAcquirer_RadioFrequency.GetRFHDScale((Int64)span_AVF, source);

            if (Hd.UIMessage?.MultiDomain?.SpecON ?? false)
            {
                TVFtotallenth = (UInt32)(actualfftlength * key.Ext / key_AVF.Ext) > 4600 ? (UInt32)(actualfftlength * key.Ext / key_AVF.Ext) : 4600;
                actualdotcnt = TVFtotallenth * key_AVF.Ext;
            }
            Double lengthByns_real = actualdotcnt * ((Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquedParameters?.PerDataByfs_AtDdr ?? 50_000)) / 1e6;

            var minposbyns = (trigpos - Constants.DEF_XPOS_IDX) / Constants.IDX_PER_XDIV * scalebyus * 1e3;
            var maxposbyns = minposbyns + sumtimebyus * 1e3;

            if (sumtimebyus * 1e3 < lengthByns_real)
            {
                lengthByns_real = sumtimebyus * 1e3;
            }

            Double startPosByns_real = startPosByns;
            if (startPosByns + lengthByns_real > maxposbyns)
            {
                startPosByns_real = maxposbyns - lengthByns_real;
            }

            return (startPosByns_real, lengthByns_real);
        }

        internal Int64 GetValidMaxSpanFreq(Double maxExtramNum)
        {
            return AbstractAcquirer_RadioFrequency.GetValidMinSpanFreq(maxExtramNum);
        }

        private UInt16 _Key = 0;
        private bool ReadFrameDataFromNormal(Int64 stftTimes, Int64 fftlength)
        {
            if (DMAData != null)
            {
                Array.Clear(DMAData, 0, DMAData.Length);
                //DMAData.Initialize();
            }
            if (IQDMAData != null)
            {
                Array.Clear(IQDMAData, 0, IQDMAData.Length);
                //DMAData.Initialize();
            }
            //if (DMATotalData != null)
            //{
            //    Array.Clear(DMATotalData, 0, DMATotalData.Length);
            //    //DMAData.Initialize();
            //}
            if (Hd.UIMessage?.MultiDomain?.ParameterTuningEnable ?? false)
            {
                auto_num++;
            }
            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.Cymometer_CymometerReset, 1);
            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.Cymometer_CymometerReset, 0);
            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.Cymometer_GateTimeReset, 200);
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_RF_select, 0x4);//处理板粗时频开关
            Int32 channel = (Int32)(Hd.UIMessage?.MultiDomain?.Source ?? ChannelId.C1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_RF_select, (uint)1 << (12 + channel));//0：普通时频 1：粗时频使能 2：细时频使能 
            HdIO.Sleep(1);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.anormdetect_emd_rd_en, 0);//选择多域路径
            HdIO.Sleep(1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_enable_set, 0);//选择多域路径
            HdIO.Sleep(1);
            //HdIO.WriteReg(ProcBdReg.W.LSCtrl_DDREnablePro, 1);  wby0108
            Int32 channelid = (Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId);
            bool retVal;
            Boolean AVTON = Hd.UIMessage?.MultiDomain?.AVTON ?? false;
            Boolean PVTON = Hd.UIMessage?.MultiDomain?.PVTON ?? false;
            Boolean FVTON = Hd.UIMessage?.MultiDomain?.FVTON ?? false;

            //para_mixing();
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_DDC_80_en, 0);


            Int32 DestinationIndex = 0;
            Int32 DestinationIndex1 = 0;
            Int64 totalLength = stftTimes * fftlength;
            UInt32 FifoDepth = (totalLength > 16 * 1024) ? 16 * 1024 : (uint)totalLength;
            ChannelId trigchid = (ChannelId)Hd.CurrProduct?.Ctrl_Trigger?.CurrentTrigSource();
            //AcqBdNo trigacqbd = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(Acquirer_AnalogChanel_DBI13G.DbiActuallActiveState, trigchid)?.FirstOrDefault()?.AcqBdNo ?? AcqBdNo.B7;

            Boolean IsSynchronizationEnable = Hd.UIMessage?.MultiDomain?.SynchronizationEnable ?? false;    // 多域同步
            Double zoomstart = Hd.UIMessage?.MultiDomain?.ZoomStart ?? -25;
            Double zoomlength = Hd.UIMessage?.MultiDomain?.ZoomLength ?? 50;
            Double zoomlength_sync;
            if (IsSynchronizationEnable)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_zero_num_l, (uint)actualfftlength);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_zero_num_h, (uint)actualfftlength >> 16);
            }
            else
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_zero_num_l, (uint)fftlength);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_zero_num_h, (uint)fftlength >> 16);
            }

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDRPcUiRstAcq, 1);
            //HdIO.Sleep(1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDRPcUiRstAcq, 0);
            //HdIO.Sleep(1);


            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_frefifo_empty_thresh, 0);

            HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x1);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.RST_CTRL_SysReset, 1);
            HdIO.Sleep(1);
            HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x0);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.RST_CTRL_SysReset, 0);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_rst, 0x1);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_rst, 0x0);
            HdIO.Sleep(1);

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_fifo_before_full_thresh, 16384);



            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_l, 0);//打开粗时频时关闭混频
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_h, 0);//
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_addr, 0);
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 1);
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 0);

            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_l, 0);//打开粗时频时关闭混频
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_h, 0);//
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_addr, 1);
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 1);
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 0);

            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 1);
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 0);
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_dec_switch, 0X10);
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_badpoint_num, 0);



            //var test = (uint)Math.Log2(FifoDepth);
            HdIO.WriteReg(PcieBdReg.W.FifoCtrl_ProgEmpty, 2);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_PointNum, FifoDepth - 1);//1023  必须减去1
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Step, FifoDepth - 1);//1023  必须减去1
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Step, 16 -1);//1023  必须减去1
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_NFFT, (uint)Math.Log2(FifoDepth));//10
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_fifo_full_thresh, FifoDepth - 2);//1022
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_frefifo_full_thresh, FifoDepth - 2);//1022
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, FifoDepth - 2);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_ddr_fifo_wen_s, 0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_data_select, 0x0002);//DDR读取之前先将数据类型选为FFT后的re和im数据，4为幅值相位信息
            var span = Hd.UIMessage?.MultiDomain?.SpanByHz ?? 1000;
            ChannelId source = Hd.UIMessage?.MultiDomain?.Source ?? ChannelId.C1;
            var key = AbstractAcquirer_RadioFrequency.GetRFHDScale(span, source);
            if (!IsSynchronizationEnable)
            {
                _LastSpan = key.Span;
            }

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_badpoint_num, 200);

            HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkmux_select, (UInt32)DMAReadSourceMuxType.FreqDomain);
            HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, (UInt32)DMAReadSourceMuxType.FreqDomain);//写pcie选择

            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, FifoDepth);

            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_SoftStopAcq, 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_ReadEnable, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_ReadEnable, 1);
            ReadParams tmpparams = new ReadParams()
            { };

            //Int64 fft_length = 1024;

            if (IsSynchronizationEnable)
            {
                tmpparams = new ReadParams()
                {
                    DdrReadStartDotPosition = _LastSatrtPos * 20 - 109 * key.Ext,
                    TotalExtractNum = 1,
                    Interpolate_Num_Double = 1,
                };

                var key_sync = ExtGetRFSpanScaleTable(actualextnum);

                if ((UInt16)key_sync.Key != _Key)
                {
                    _Key = (UInt16)key_sync.Key;
                    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_dec_switch, _Key);
                }
                //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_dec_switch, (UInt16)key_sync.Key);

                //spansync = key_sync.Span;
                fftlengthsync = fft_length;
            }

            else
            {
                tmpparams = new ReadParams()
                {
                    DdrReadStartDotPosition = Acquisition.LastDotOffset,
                    TotalExtractNum = 1,
                    Interpolate_Num_Double = 1,
                };
            }

            HdCtrl_AnalogDDR.ConfigRead(tmpparams, AcquedParameters);



            //HdIO.WriteReg(ProcBdReg.W.Dsp_DspEnPro, 0x0);
            //HdIO.WriteReg(ProcBdReg.W.Interpolate_InterpEnPro, 0x0);
            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TIADC_Enable, 0);
            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.Interpolate_EnableAcq, 0);
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);//
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);//wby251111

            sleep_time(key.Ext, FifoDepth);
            //Thread.Sleep(400);

            spanhardware = key.Span;

            retVal = false;
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Config_Start, 0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Config_Start, 1);
            HdIO.Sleep(1);
            //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, 8 * FifoDepth);//8=8bit
            //HdIO.WriteReg(PcieBdReg.W.ReadTotalBytes, 4 * FifoDepth);
            //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, 64 * 1015);//单位为bit
            HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, 64 * (FifoDepth));//单位为bit//+4是因为PCIE有点问题lhc//实际发的是Xdma_DataNum 251209wby
            HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x0);
            HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x1);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Calc_Start, 0);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Calc_Start, 1);

            //if (key.Ext > 80000)
            //    HdIO.Sleep(1500);
            //else
            HdIO.Sleep(1);


            //HdIO.Sleep(10);


            Int64 readTimes = totalLength / FifoDepth + ((totalLength > FifoDepth) ? 1 : 0);
            for (int i = 0; i < readTimes; i++)
            {
                if (HdIO.CheckRegisterValue((uint)PcieBdReg.R.Xdma_XdmaWrFinish, 1, 0x01, 1000))
                {
                    //HdIO.Sleep(1000);//251124 wby
                    retVal = HdIO.DMARead(8 * FifoDepth, ref DMAData);
                    //if (retVal == false)
                    //{
                    //    return true;
                    //}
                    HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);//251112wby
                    //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_ReadEnable, 0);
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_SoftStopAcq, 0);
                }
                else
                    ;
                Array.Copy(DMAData, 0, DMATotalData, 8 * DestinationIndex, 8 * FifoDepth);
                //HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkmux_select, 0);
                //HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, 0);//写pcie选择
                if (!(AVTON || PVTON || FVTON))
                {
                    HdIO.WriteReg(ProcBdReg.W. DataPath_pro_linkmux_select, 0);
                    HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, 0);//写pcie选择
                    
                }
                DestinationIndex += (int)FifoDepth;
                if (i == readTimes - 2 && totalLength > FifoDepth)
                {
                    FifoDepth = (uint)totalLength % FifoDepth;
                    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_ScaleSCH, FifoDepth);//fft之后fifo深度配置
                    //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, FifoDepth);//PCIE FIFO深度
                }
                //HdIO.WriteReg(ProcBdReg.W.MDO_STFT_FFTTimes, 0);//------------------------------------------------------------------------------------------------lhc
                //HdIO.WriteReg(ProcBdReg.W.MDO_STFT_FFTTimes, 1);//------------------------------------------------------------------------------------------------lhc
            }
            if (AVTON || PVTON || FVTON)//如果幅度域相位域开关被打开，读取原始I、Q数据
            {
                //HdIO.Sleep(10);
                //HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x0);
                //HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x1);

                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_data_select, 0x0001);//wby0320

                Int64 IQLength = Hd.UIMessage?.MultiDomain?.STFTLength ?? 1024;
                if (IQLength > FifoMaxDepth)
                    IQLength = FifoMaxDepth;
                //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, (UInt32)IQLength - 2);//PCIE FIFO深度

                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_fifo_full_thresh, FifoDepth - 2);

                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);//
           

                HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, 64 * (FifoDepth) + 1);//实际发的是Xdma_DataNum 251209wby

                HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x1);
                HdIO.Sleep(1);
                HdIO.WriteReg(ProcBdReg.W.reverse_reg_pro_ssd_reserve0, 1);
                HdIO.Sleep(1);
                HdIO.WriteReg(ProcBdReg.W.reverse_reg_pro_ssd_reserve0, 0);
                HdIO.Sleep(1);
                HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x0);
                HdIO.Sleep(1);

                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);//wby0227

                HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkmux_select, 3);
                HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, 3);//写pcie选择
                HdIO.WriteReg(PcieBdReg.W.FifoCtrl_ProgEmpty, 3);


                HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x0);
                HdIO.Sleep(1);
                HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x1);
                HdIO.Sleep(1);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_data_select, 0x4001);//0x8001测试数据 0x4001读调制域 0x0001选调制域，但不读 
                HdIO.Sleep(1);
                retVal = HdIO.DMARead(8 * FifoDepth, ref DMAData);//(乘以2是因为前面8 * FifoDepth是FFT结果，后面8 * FifoDepth是混频后的时域结果)
                //retVal = true;
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);//wby0227

                HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkmux_select, 0);
                HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, 0);//写pcie选择
                Array.Copy(DMAData, 0, IQDMAData, 8 * DestinationIndex1, 8 * FifoDepth);
                DestinationIndex1 += (int)FifoDepth;
            }

            HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, 3);//写pcie选择,必须发3，时域才能读取上来 0227 lww


            HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkdemux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
            //HdIO.Sleep(1000);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DataPath_acq_linkmux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_enable_set, 0);

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_DDC_80_en, 0);
            return true;
        }

        private bool ReadFrameData_Auto()
        {


            UInt32 FifoDepth = 2048;
            if (DMADataCu != null)
            {
                Array.Clear(DMADataCu, 0, DMADataCu.Length);
                //DMAData.Initialize();
            }
            if (DMATotalData != null)
            {
                Array.Clear(DMATotalData, 0, DMATotalData.Length);
                //DMAData.Initialize();
            }
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_7, 30);//DataPath_AfifoPreP2SFullThresh


            bool retVal;
            Int32 DestinationIndex1 = 0;


            Int32 channel = (Int32)(Hd.UIMessage?.MultiDomain?.Source ?? ChannelId.C1);
            if (channel == 0)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_RF_select, 0x1014);//0x1014丢点关闭 0x1004丢点打开wby
                //HdIO.WriteReg(ProcBdReg.W.Debug_SingleModePro, 0b0000_0000_0000_0110);//通道开关，高位决定是否开单板  wby0108
            }
            if (channel == 1)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_RF_select, 0x2004);
                //HdIO.WriteReg(ProcBdReg.W.Debug_SingleModePro, 0b1000_0000_0000_1101);//通道开关，高位决定是否开单板  wby0108
            }
            if (channel == 2)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_RF_select, 0x4004);
                //HdIO.WriteReg(ProcBdReg.W.Debug_SingleModePro, 0b1000_0000_0000_1011);//通道开关，高位决定是否开单板  wby0108
            }
            if (channel == 3)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_RF_select, 0x8004);
                //HdIO.WriteReg(ProcBdReg.W.Debug_SingleModePro, 0b1000_0000_0000_0111);//通道开关，高位决定是否开单板  wby0108
            }
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_StorageMode, 0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_badpoint_num, 0);

            HdIO.WriteReg(ProcBdReg.W.DataPath_SyncFifoEmptyThreshold, 0);//8=8bit0717

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_rf_points, 256);//对并行8路数据计数,wby>=256//开dbi丢点，多加89
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_rf_dly_num, (UInt32)(Hd.UIMessage?.MultiDomain?.TimeStep ?? 1));//延迟数，单位为64ns
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_fifo_before_full_thresh, 2048 - 2);
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_RF_select, 0x4);//处理板粗时频开关
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_enable_set, 0x4);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_7, 30); //DataPath_AfifoPreP2SFullThresh wby251113

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, FifoDepth);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_dec_switch, 0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_frefifo_full_thresh, FifoDepth - 300);//1022

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_zero_num_l, (uint)0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_zero_num_h, (uint)2048 * 512 >> 16);

            HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x1);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.RST_CTRL_SysReset, 1);
            HdIO.Sleep(1);
            HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x0);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_rst, 0x1);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_rst, 0x0);
            HdIO.Sleep(1);
            //RFWindowType windowType = Hd.UIMessage?.MultiDomain?.WindowType ?? RFWindowType.Hann;
            //configure_window(2048, windowType);

            HdIO.WriteReg(ProcBdReg.W.RST_CTRL_SysReset, 0);



            HdIO.WriteReg(PcieBdReg.W.FifoCtrl_ProgEmpty, 2);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_PointNum, FifoDepth - 1);//1023  必须减去1
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Step, FifoDepth - 1);//1023  必须减去1
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Step, 16 -1);//1023  必须减去1
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_NFFT, (uint)Math.Log2(FifoDepth));//10
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_fifo_full_thresh, FifoDepth - 2);//1022
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_frefifo_full_thresh, FifoDepth - 2);//1022

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_data_select, 0x0002);//DDR读取之前先将数据类型选为FFT后的re和im数据

            HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkmux_select, (UInt32)DMAReadSourceMuxType.FreqDomain);
            HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, (UInt32)DMAReadSourceMuxType.FreqDomain);//写pcie选择


            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_l, 0);//打开粗时频时关闭混频
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_h, 0);//
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_addr, 0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 0);

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_l, 0);//打开粗时频时关闭混频
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_h, 0);//
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_addr, 1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 0);

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_dec_switch, 0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_badpoint_num, 0);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DBI_IntDelayNum, 0);//处理板dbi丢点值
            HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh1, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh2, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh3, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh4, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DBI_auto_init_delay_num, 0);//采集板dbi丢点值
            HdIO.WriteReg(ProcBdReg.W.DBI_auto_init_delay_numProCh1, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_auto_init_delay_numProCh2, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_auto_init_delay_numProCh3, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_auto_init_delay_numProCh4, 0);

            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);//wby

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Config_Start, 0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Config_Start, 1);
            HdIO.Sleep(1);
            //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, 8 * FifoDepth);//8=8bit
            //HdIO.WriteReg(PcieBdReg.W.ReadTotalBytes, 4 * FifoDepth);
            //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, 64 * 1015);//单位为bit
            HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, 64 * (FifoDepth) * 512);//单位为bit//+4是因为PCIE有点问题lhc//实际发的是Xdma_DataNum 251209wby
            HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x0);
            HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x1);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Calc_Start, 0);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Calc_Start, 1);
            //if (key.Ext > 80000)
            //    HdIO.Sleep(1500);
            //else
            HdIO.Sleep(1);


            retVal = HdIO.DMARead(8 * FifoDepth * 512, ref DMADataCu);//(541=512+29)29为路径上的固定残留
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
            Array.Copy(DMADataCu, 0, DMATotalData, 8 * DestinationIndex1, 8 * FifoDepth * 512);


            //发混频


            var span = Hd.UIMessage?.MultiDomain?.SpanByHz ?? 1000;
            var CenterFreq = Hd.UIMessage?.MultiDomain?.CenterFreqByHz ?? 1000;
            ChannelId source = Hd.UIMessage?.MultiDomain?.Source ?? ChannelId.C1;
            double fs_sys = 20_000_000_000;
            double fchardware = -(CenterFreq);

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_badpoint_num, 0);
            //var span = Hd.UIMessage?.RadioFrequency?[(Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId)].Span ?? 1000;

            var key = AbstractAcquirer_RadioFrequency.GetRFHDScale(span, source);
            double fchan = fs_sys;
            double nph = 28;
            fchardware = fchardware + fs_sys;
            UInt32 ph_step_inside_chan_out_range = (UInt32)(Math.Round(fchardware / fchan * Math.Pow(2, nph)));
            double ph_inside_chan_out_range = fchardware / fs_sys * Math.Pow(2, nph);
            UInt32 ph_to_FPGA;
            UInt32 ph_step_to_FPGA;
            ph_step_to_FPGA = (UInt32)(ph_step_inside_chan_out_range % Math.Pow(2, nph));
            HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x1);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.reverse_reg_pro_ssd_reserve0, 1);
            HdIO.Sleep(1);
            HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x0);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.reverse_reg_pro_ssd_reserve0, 0);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 0);
            for (int i = 0; i < 1; i++)
            {
                ph_to_FPGA = (UInt32)(Math.Round(ph_inside_chan_out_range * i) % Math.Pow(2, nph));

                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_l, ph_to_FPGA);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_h, ph_to_FPGA >> 16);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_addr, (uint)i);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 1);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 0);
            }
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_l, ph_step_to_FPGA);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_h, ph_step_to_FPGA >> 16);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_addr, 1);

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 0);

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 0);
            //发抽取
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_en, 0x1);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_CIC_rst, 0x1);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_CIC_rst, 0x0);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_dec_switch, (UInt16)key.Key);//
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_SerialCIC_Config_Tvalid, 0x1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, 16382);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_SerialCIC_Config_Tvalid, 0x0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_badpoint_num, 200);

            //Int32 fftLength = (int)(Hd.UIMessage?.MultiDomain?.FFTLength ?? 1024);

            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_rst, 0x1);
            //HdIO.Sleep(1);
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_rst, 0x0);
            //HdIO.Sleep(1);

            //configure_window(fftLength, windowType);

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_RF_select, 0);//处理板粗时频开关
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_enable_set, 0);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_StorageMode, 1);

            return true;
        }

        UInt32 TVFtotallenth = 8192;
        private bool ReadFrameDataTVF()
        {
            if (DMADataXi != null)
            {
                Array.Clear(DMADataXi, 0, DMADataXi.Length);
            }
            if (DMATotalData != null)
            {
                Array.Clear(DMATotalData, 0, DMATotalData.Length);
            }
            bool retVal;

            Int32 DestinationIndex = 0;
            UInt32 FifoDepth = 512;
            UInt32 FreFifoDepth = 512;
            UInt32 FFTStep = 512;
            UInt32 frame_num = 512;


            Int32 fftLength = (int)(Hd.UIMessage?.MultiDomain?.FFTLength ?? 1024);

            var tmp = Hd.UIMessage?.MultiDomain?.SpanForTimeFreq ?? 0;
            tmp = Hd.UIMessage?.MultiDomain?.TimeScaleForTimeFreq ?? 0;

            Boolean IsSynchronizationEnable = Hd.UIMessage?.MultiDomain?.SynchronizationEnable ?? false;    // 多域同步
            Double zoomstart = Hd.UIMessage?.MultiDomain?.ZoomStart ?? -25;

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_RF_select, 0x0);//0：普通时频 1：粗时频使能 2：细时频使能 
            HdIO.Sleep(1);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.anormdetect_emd_rd_en, 0);//选择多域路径
            HdIO.Sleep(1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_enable_set, 0);//选择多域路径
            HdIO.Sleep(1);
            //HdIO.WriteReg(ProcBdReg.W.LSCtrl_DDREnablePro, 1);  wby0108   因为表里没了，所以要注释掉

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_zero_num_l, (uint)TVFtotallenth);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_zero_num_h, (uint)TVFtotallenth >> 16);

            HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x1);
            HdIO.Sleep(10);
            //HdIO.WriteReg(ProcBdReg.W.RST_CTRL_SysReset, 1);
            HdIO.Sleep(10);
            HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x0);
            HdIO.Sleep(10);
            //HdIO.WriteReg(ProcBdReg.W.RST_CTRL_SysReset, 0);
            HdIO.Sleep(10);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_rst, 0x1);
            HdIO.Sleep(10);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_rst, 0x0);

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_fifo_before_full_thresh, 16382);


            var span = Hd.UIMessage?.MultiDomain?.SpanByHz ?? 1000;
            ChannelId source = Hd.UIMessage?.MultiDomain?.Source ?? ChannelId.C1;
            var key = AbstractAcquirer_RadioFrequency.GetRFHDScale(span, source);
            Double fre_rate = ((Hd.UIMessage?.MultiDomain?.SpanForTimeFreq ?? 0) / key.Span);
            var span_AVF = (span * fre_rate > 20_000_000_000) ? 20_000_000_000 : span * fre_rate;
            var key_AVF = AbstractAcquirer_RadioFrequency.GetRFHDScale((Int64)span_AVF, source);
            if (IsSynchronizationEnable)
            {
                //TVFtotallenth = (UInt32)(actualfftlength * key.Ext / key_AVF.Ext) > 4600 ? (UInt32)(actualfftlength * key.Ext / key_AVF.Ext) : 4600;
                FFTStep = (TVFtotallenth - FifoDepth) / (frame_num - 1);

            }
            if (!IsSynchronizationEnable)
            {
                FFTStep = (UInt32)((Hd.UIMessage?.MultiDomain?.TimeScaleForTimeFreq * 1e9 ?? 0) * 20 / key_AVF.Ext);
                TVFtotallenth = FFTStep * 511 + 512;
            }

            if (TVFtotallenth > 16382)
            {
                FreFifoDepth = 15000;
            }
            else
            {
                FreFifoDepth = TVFtotallenth;
            }
            //发窗
            //RFWindowType windowType = Hd.UIMessage?.MultiDomain?.WindowType ?? RFWindowType.Hann;
            //configure_window((int)FifoDepth, windowType);

            //发STFT配置
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_PointNum, FifoDepth - 1);//必须减去1
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Step, FFTStep - 1);//必须减去1
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_NFFT, (uint)Math.Log2(FifoDepth));
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_fifo_full_thresh, FifoDepth - 2);//test
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_frefifo_full_thresh, FreFifoDepth);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, FifoDepth - 2);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_badpoint_num, 200);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_data_select, 0x0002);//DDR读取之前先将数据类型选为FFT后的re和im数据
            HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkmux_select, (UInt32)DMAReadSourceMuxType.FreqDomain);
            HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, (UInt32)DMAReadSourceMuxType.FreqDomain);//写pcie选择

            //发抽取

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_dec_switch, (UInt16)key_AVF.Key);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_CIC_rst, 0x1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_CIC_rst, 0x0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_dec_switch, (UInt16)key_AVF.Key);
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_dec_switch, 0x08);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_FreDeciMode, 25);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_SerialCIC_Config_Tvalid, 0x1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_SerialCIC_Config_Tvalid, 0x0);



            //发读起始地址
            ReadParams tmpparams = new ReadParams()
            { };


            if (IsSynchronizationEnable)
            {
                tmpparams = new ReadParams()
                {
                    DdrReadStartDotPosition = zoomstart * 20 - 109 * key.Ext,
                    TotalExtractNum = 1,
                    Interpolate_Num_Double = 1,
                };
            }
            else
            {
                tmpparams = new ReadParams()
                {
                    DdrReadStartDotPosition = Acquisition.LastDotOffset,
                    TotalExtractNum = 1,
                    Interpolate_Num_Double = 1,
                };
            }

            HdCtrl_AnalogDDR.ConfigRead(tmpparams, AcquedParameters);

            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);//wby20260204
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);//wby20260204

            sleep_time(key.Ext, (uint)fftLength);


            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Config_Start, 0);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Config_Start, 1);
            HdIO.Sleep(1);
            HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, 64 * (FifoDepth) * 512);//实际发的是Xdma_DataNum 251209wby
            HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x0);
            HdIO.Sleep(1);
            HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x1);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Calc_Start, 0);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Calc_Start, 1);
            HdIO.Sleep(1);




            Int64 readTimes = 1;
            for (int i = 0; i < readTimes; i++)
            {
                retVal = HdIO.DMARead(8 * FifoDepth * 512, ref DMADataXi);
                Array.Copy(DMADataXi, 0, DMATotalData, 8 * DestinationIndex, 8 * FifoDepth * 512);
                DestinationIndex += (int)FifoDepth;
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);//wby20260107
            }

            //发抽取

            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_dec_switch, (UInt16)key_AVF.Key);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_CIC_rst, 0x1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_CIC_rst, 0x0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_dec_switch, (UInt16)key.Key);
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_dec_switch, 0x08);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_FreDeciMode, 25);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_SerialCIC_Config_Tvalid, 0x1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_SerialCIC_Config_Tvalid, 0x0);

            HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkdemux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DataPath_acq_linkmux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);

            return true;
        }

        private bool ReadFrameDataCu()
        {
            if (cnt > 0)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_RF_select, 0x0);//处理板粗时频开关
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_enable_set, 1);
                return false;
            }
            if (DMADataCu != null)
            {
                Array.Clear(DMADataCu, 0, DMADataCu.Length);
                //DMAData.Initialize();
            }
            if (DMATotalData != null)
            {
                Array.Clear(DMATotalData, 0, DMATotalData.Length);
                //DMAData.Initialize();
            }
            UInt32 ddr_addr_L = 0;
            UInt32 ddr_addr_H = 0;
            UInt32 readTimes = 1;
            UInt32 FifoDepth1 = 2048;//1024
            Int64 totalLength = 2048;//1024
            bool retVal;
            Int32 DestinationIndex1 = 0;

            if (cnt == 0)
            {

                HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x1);
                HdIO.Sleep(10);
                HdIO.WriteReg(ProcBdReg.W.reverse_reg_pro_ssd_reserve0, 1);
                HdIO.Sleep(10);
                HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x0);
                HdIO.Sleep(10);
                HdIO.WriteReg(ProcBdReg.W.reverse_reg_pro_ssd_reserve0, 0);
                HdIO.Sleep(10);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_rst, 0x1);
                HdIO.Sleep(10);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_rst, 0x0);

                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_l, 0);//打开粗时频时关闭混频
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_h, 0);//
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_addr, 0);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 1);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 0);

                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_l, 0);//打开粗时频时关闭混频
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_h, 0);//
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_addr, 1);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 1);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 0);

                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 1);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 0);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_dec_switch, 0);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_badpoint_num, 0);
                HdIO.Sleep(10);
                //HdIO.WriteReg(ProcBdReg.W.FifoCtrl_AcqWriteEnable, 0);  wby0108
                HdIO.Sleep(10);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_ddr_fifo_wen_s, 1);
                //HdIO.WriteReg(ProcBdReg.W.FifoCtrl_AcqWriteEnable, 1);//写使能，ADC到FIFO的第一步   wby0108
                HdIO.Sleep(200);
                bool finish = Cu_Write_Finish();
                if (!finish)
                {
                    return false;
                }
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_ddr_fifo_wen_s, 0);

                ChannelId trigchid = (ChannelId)Hd.CurrProduct?.Ctrl_Trigger?.CurrentTrigSource();
                //AcqBdNo trigacqbd = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(Acquirer_AnalogChanel_DBI13G.DbiActuallActiveState, trigchid)?.FirstOrDefault()?.AcqBdNo ?? AcqBdNo.B7;
                AcqBdNo trigacqbd = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(1, trigchid)?.FirstOrDefault()?.AcqBdNo ?? AcqBdNo.B7;
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_PointNum, 2047);//1023  必须减去1
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Step, 2047);//1023  必须减去1
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_NFFT, 11);//10
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_fifo_full_thresh, 2046);//1022
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_frefifo_full_thresh, 2046);//1022
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, 2560);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_data_select, 0x0002);//DDR读取之前先将数据类型选为FFT后的re和im数据
                //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_data_select, 0x0001);//DDR读取之前先将数据类型选为FFT后的re和im数据

                ddr_addr_L = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.LSCtrl_DDRTrigAddrReadL, trigacqbd) & 0xFFFF ?? 0;
                ddr_addr_H = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.LSCtrl_DDRTrigAddrReadH, trigacqbd) & 0xFFFF ?? 0;
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_segmentlenth_l, 16384);//writeParams.WaveAddrSum
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_segmentlenth_l, _test % 65536);//writeParams.WaveAddrSum
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_segmentlenth_h, _test / 65536);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_fft_points, 2048);//1024
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_offset, 0);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_segment_num, 512);

                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_init_addr_l, 0);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_init_addr_h, 0);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_rd_lenth_l, 0);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_rd_lenth_h, 8);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_DDR_per_lenth, 5);//每个地址去DDR读多少次，粗时频为5次，细时频为5N次，N为抽取率
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DBI_IntDelayNum, 0);//采集板dbi丢点值
                HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh1, 0);
                HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh2, 0);
                HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh3, 0);
                HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh4, 0);
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DBI_auto_init_delay_num, 0);//采集板dbi丢点值
                HdIO.WriteReg(ProcBdReg.W.DBI_auto_init_delay_numProCh1, 0);
                HdIO.WriteReg(ProcBdReg.W.DBI_auto_init_delay_numProCh2, 0);
                HdIO.WriteReg(ProcBdReg.W.DBI_auto_init_delay_numProCh3, 0);
                HdIO.WriteReg(ProcBdReg.W.DBI_auto_init_delay_numProCh4, 0);
                HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkmux_select, (UInt32)DMAReadSourceMuxType.FreqDomain);
                HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, (UInt32)DMAReadSourceMuxType.FreqDomain);//写pcie选择

                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDRPcUiRstAcq, 1);
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDRPcUiRstAcq, 0);

                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);//wby20251111
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_RF_select, 0x1);//0：普通时频 1：粗时频使能 2：细时频使能 
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_enable_set, 3);
                //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, 128);//单位为32kByte
                HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, 512 * 64 * 2048);//单位为bit(541=512+29)29为路径上的固定残留//实际发的是Xdma_DataNum 251209wby


                //发窗
                {
                    RFWindowType WindowType;
                    Int32 FFTLength;
                    Int32 CurrentRFChannel = Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId;
                    Int32 length = 2048;
                    RFWindowType windowType = Hd.UIMessage?.MultiDomain?.WindowType ?? RFWindowType.Hann;
                    WindowType = windowType;
                    FFTLength = length;
                    //Thread.Sleep(3000);

                    List<double> coefficient = CtrlRadioFrequency_Standard.GetWindowCoefficient(length, windowType).ToList();
                    for (int i = 0; i < coefficient.Count; i++)
                    {
                        //UInt32 value = (UInt32)(coefficient[i]);
                        //UInt32 value = BitConverter.ToUInt32(BitConverter.GetBytes(3.255));
                        byte[] floatBytes = BitConverter.GetBytes((float)coefficient[i]);
                        // byte[] floatBytes = BitConverter.GetBytes(3.255f);

                        UInt16 low = (UInt16)(floatBytes[1] << 8 & 0xff00 | floatBytes[0] & 0x00ff);
                        UInt16 high = (UInt16)(floatBytes[3] << 8 & 0xff00 | floatBytes[2] & 0x00ff);
                        //UInt16 valueH = (UInt16)(value >> 16 & (0xffff));
                        //UInt16 valueL = (UInt16)(value & (0xffff));

                        //HdIO.WriteReg(ProcBdReg.W.MDO_STFT_CoefficientDataInH16, high);//暂时是写死的矩形窗lhc
                        //HdIO.WriteReg(ProcBdReg.W.MDO_STFT_CoefficientDataInL16, low); //暂时是写死的矩形窗lhc
                        HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_coefficient_datain_h16, high);//暂时是写死的矩形窗lhc
                        HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_coefficient_datain_l16, low); //暂时是写死的矩形窗lhc
                        HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_Coefficient_Data_WREN, 0);
                        HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_Coefficient_Data_WREN, 1);

                    }
                    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_Coefficient_Data_WREN, 0);
                }

                retVal = false;

                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Config_Start, 0);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Config_Start, 1);
                HdIO.Sleep(10);
                HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x0);
                HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x1);
                HdIO.Sleep(10);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Calc_Start, 0);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Calc_Start, 1);
                HdIO.Sleep(30);
                //retVal = CheckRegValue((UInt32)AcqBdReg.R.reverse_Read0, 7, 10000);
                //if (!retVal)
                //{
                //    return retVal;
                //}
                //retVal = CheckRegValue((UInt32)AcqBdReg.R.reverse_Read0, 7, 10000);
                Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_SoftStopAcq, 1);
                HdIO.Sleep(10);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_enable_set, 1);
                HdIO.Sleep(10);
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);
                HdIO.Sleep(10);
                Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_SoftStopAcq, 0);
                UInt32 ddr_discard_num = 0;
                UInt32 ddr_discard_num_offset = 0;
                UInt32 ddr_discard_num_sum = 0;
                Boolean ddr_discard_num_vaild = HdIO.CheckRegisterValue((uint)AcqBdReg.R.LSCtrl_DDRReadTrigDiscardNumflag, (UInt32)trigacqbd, 0x1, 1);
                if (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.HdMessage?.Timebase?.SegmentActive == 1)
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_TrigDiscardMode, 0);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDRTrigDiscardNumValid, 0x0);
                }
                else
                {
                    if (ddr_discard_num_vaild)
                    {
                        ddr_discard_num = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.LSCtrl_ReadTrigDiscardNum, trigacqbd) & 0xFFFF ?? 0;
                        ddr_discard_num_offset = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.LSCtrl_DDRReadTrigDiscardNumOffset, trigacqbd) & 0xFFFF ?? 0;
                        ddr_discard_num_sum = ddr_discard_num + ddr_discard_num_offset;
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_TrigDiscardMode, (Hd.UIMessage?.bAcquireStopped ?? false) ? 0U : 1U);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_TrigDiscardNum, ddr_discard_num_sum);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDRTrigDiscardNumValid, 0x0);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDRTrigDiscardNumValid, 0x1);
                    }
                    else
                    {
                        ddr_discard_num = 0;
                        ddr_discard_num_offset = 0;
                        ddr_discard_num_sum = 0;
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_TrigDiscardMode, 0);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_TrigDiscardNum, ddr_discard_num_sum);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDRTrigDiscardNumValid, 0x0);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDRTrigDiscardNumValid, 0x1);
                    }
                }
            }
            //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_ReadFromFIFO_Num, 1024);//dma从fifo读取的数据量

            for (int i = 0; i < readTimes; i++)//多次读取短时傅里叶变换产生的多帧FFT数据，由于fifo深度只有16K，所以需要多次读取
            {
                //retVal = CheckRegValue((UInt32)PcieBdReg.R.FifoCtrl_FullFlagRead, 1, 1000);
                //retVal = CheckRegValue((UInt32)PcieBdReg.R.Xdma_ReadFifoProgFull, 1, 1000);
                retVal = CheckRegValue((UInt32)PcieBdReg.R.Xdma_XdmaWrFinish, 1, 1000);
                if (!retVal)
                    return retVal;
                //HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x0);
                //HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x1);
                retVal = HdIO.DMARead(8 * FifoDepth1 * 512, ref DMADataCu);//(541=512+29)29为路径上的固定残留
                if (retVal)
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_enable_set, 1);
                    cnt++;
                }
                Array.Copy(DMADataCu, 0, DMATotalData, 8 * DestinationIndex1, 8 * FifoDepth1 * 512);
                DestinationIndex1 += (int)FifoDepth1;
                if (i == readTimes - 2 && (totalLength % FifoDepth1 != 0))
                {
                    FifoDepth1 = (uint)totalLength % FifoDepth1;
                    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_ScaleSCH, FifoDepth1);//fft之后fifo深度配置
                   // HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, FifoDepth1);//PCIE FIFO深度
                }
            }


            return true;
        }

        private bool ReadFrameDataXi(Int64 stftTimes, Int64 fftlength)
        {
            if (DMADataXi != null)
            {
                Array.Clear(DMADataXi, 0, DMADataXi.Length);
                //DMAData.Initialize();
            }
            if (DMATotalData != null)
            {
                Array.Clear(DMATotalData, 0, DMATotalData.Length);
                //DMAData.Initialize();
            }

            Int32 channelid = (Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId);
            bool retVal;
            Boolean AVTON = Hd.UIMessage?.MultiDomain?.AVTON ?? false;
            Boolean PVTON = Hd.UIMessage?.MultiDomain?.PVTON ?? false;
            Boolean FVTON = Hd.UIMessage?.MultiDomain?.FVTON ?? false;

            Int32 DestinationIndex = 0;
            Int32 DestinationIndex1 = 0;
            //Int64 totalLength = stftTimes * fftlength;
            Int64 totalLength = fftlength;
            UInt32 FifoDepth = (totalLength > 8 * 1024) ? 8 * 1024 : (uint)totalLength;

            ChannelId trigchid = (ChannelId)Hd.CurrProduct?.Ctrl_Trigger?.CurrentTrigSource();
            //AcqBdNo trigacqbd = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(Acquirer_AnalogChanel_DBI13G.DbiActuallActiveState, trigchid)?.FirstOrDefault()?.AcqBdNo ?? AcqBdNo.B7;
            AcqBdNo trigacqbd = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(1, trigchid)?.FirstOrDefault()?.AcqBdNo ?? AcqBdNo.B7;


            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_ddr_fifo_wen_s, 0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_RF_select, 0x2);//0：普通时频 1：粗时频使能 2：细时频使能 
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);

            HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x1);
            HdIO.Sleep(10);
            HdIO.WriteReg(ProcBdReg.W.reverse_reg_pro_ssd_reserve0, 1);
            HdIO.Sleep(10);
            HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x0);
            HdIO.Sleep(10);
            HdIO.WriteReg(ProcBdReg.W.reverse_reg_pro_ssd_reserve0, 0);
            HdIO.Sleep(10);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_rst, 0x1);
            HdIO.Sleep(10);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_rst, 0x0);



            //发混频
            double fs_sys = 20_000_000_000;
            var span = (Int64)Span * 4;
            double fchardware = -(CenterFreq);
            ChannelId source = Hd.UIMessage?.RadioFrequency?[(Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId)].Source ?? ChannelId.C1;
            if (source == ChannelId.RF)
            {
                fs_sys = 80_000_000_000;
                if (fchardware < -500_000_000)
                {

                    fchardware = -GetValidSpan(Span) / 2;
                }
            }
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_badpoint_num, 0);
            //var span = Hd.UIMessage?.RadioFrequency?[(Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId)].Span ?? 1000;

            var key = AbstractAcquirer_RadioFrequency.GetRFHDScale(span, source);
            double fchan = fs_sys;
            double nph = 28;
            fchardware = fchardware + fs_sys;
            UInt32 ph_step_inside_chan_out_range = (UInt32)(Math.Round(fchardware / fchan * Math.Pow(2, nph)));
            double ph_inside_chan_out_range = fchardware / fs_sys * Math.Pow(2, nph);
            UInt32 ph_to_FPGA;
            UInt32 ph_step_to_FPGA;
            ph_step_to_FPGA = (UInt32)(ph_step_inside_chan_out_range % Math.Pow(2, nph));
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 0);
            for (int i = 0; i < 1; i++)
            {
                ph_to_FPGA = (UInt32)(Math.Round(ph_inside_chan_out_range * i) % Math.Pow(2, nph));

                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_l, ph_to_FPGA);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_h, ph_to_FPGA >> 16);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_addr, (uint)i);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 1);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 0);
            }
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_l, ph_step_to_FPGA);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_h, ph_step_to_FPGA >> 16);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_addr, 1);

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 0);

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 0);

            //发窗
            {
                RFWindowType WindowType;
                Int32 FFTLength;
                Int32 CurrentRFChannel = Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId;
                Int32 length = 512;
                RFWindowType windowType = Hd.UIMessage?.MultiDomain?.WindowType ?? RFWindowType.Hann;
                WindowType = windowType;
                FFTLength = length;
                //Thread.Sleep(3000);

                List<double> coefficient = CtrlRadioFrequency_Standard.GetWindowCoefficient(length, windowType).ToList();
                for (int i = 0; i < coefficient.Count; i++)
                {
                    //UInt32 value = (UInt32)(coefficient[i]);
                    //UInt32 value = BitConverter.ToUInt32(BitConverter.GetBytes(3.255));
                    byte[] floatBytes = BitConverter.GetBytes((float)coefficient[i]);
                    // byte[] floatBytes = BitConverter.GetBytes(3.255f);

                    UInt16 low = (UInt16)(floatBytes[1] << 8 & 0xff00 | floatBytes[0] & 0x00ff);
                    UInt16 high = (UInt16)(floatBytes[3] << 8 & 0xff00 | floatBytes[2] & 0x00ff);
                    //UInt16 valueH = (UInt16)(value >> 16 & (0xffff));
                    //UInt16 valueL = (UInt16)(value & (0xffff));

                    //HdIO.WriteReg(ProcBdReg.W.MDO_STFT_CoefficientDataInH16, high);//暂时是写死的矩形窗lhc
                    //HdIO.WriteReg(ProcBdReg.W.MDO_STFT_CoefficientDataInL16, low); //暂时是写死的矩形窗lhc
                    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_coefficient_datain_h16, high);//暂时是写死的矩形窗lhc
                    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_coefficient_datain_l16, low); //暂时是写死的矩形窗lhc
                    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_Coefficient_Data_WREN, 0);
                    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_Coefficient_Data_WREN, 1);

                }
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_Coefficient_Data_WREN, 0);
            }

            //发STFT配置
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_PointNum, 511);//必须减去1
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Step, 511);//必须减去1
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_NFFT, 9);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_fifo_full_thresh, 510);//test
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_frefifo_full_thresh, 510);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, 510);

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_data_select, 0x0002);//DDR读取之前先将数据类型选为FFT后的re和im数据
            HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkmux_select, (UInt32)DMAReadSourceMuxType.FreqDomain);
            HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, (UInt32)DMAReadSourceMuxType.FreqDomain);//写pcie选择
            //发读起始地址
            double T = Tstart * 512;
            double T1 = Tstop * 512;
            UInt32 start = (UInt32)(Params * Tstart) / 16;
            UInt32 stop = (UInt32)(Params * Tstop) / 16;
            UInt32 step = (stop - start) * 16 / 512;
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_segmentlenth_l, step % 65536);//writeParams.WaveAddrSum
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_segmentlenth_h, step / 65536);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_fft_points, 512);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_offset, 0);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_segment_num, 512);

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_init_addr_l, start % 65536);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_init_addr_h, start / 65536);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_DDR_per_lenth, 5 * (uint)key.Ext);//每个地址去DDR读多少次，粗时频为5次，细时频为5N次，N为抽取率
            //发抽取
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_dec_switch, (UInt16)key.Key);//
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DBI_IntDelayNum, 0);//采集板dbi丢点值
            HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh1, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh2, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh3, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh4, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DBI_auto_init_delay_num, 0);//采集板dbi丢点值
            HdIO.WriteReg(ProcBdReg.W.DBI_auto_init_delay_numProCh1, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_auto_init_delay_numProCh2, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_auto_init_delay_numProCh3, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_auto_init_delay_numProCh4, 0);

            HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, 64 * 512 * 512);//单位为bit//实际发的是Xdma_DataNum 251209wby
            HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x0);
            HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x1);

            //发读使能
            HdIO.Sleep(10);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_RF_select, 0x2);//0：普通时频 1：粗时频使能 2：细时频使能
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_enable_set, 3);

            HdIO.Sleep(50);
            retVal = false;
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Config_Start, 0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Config_Start, 1);
            HdIO.Sleep(10);
            HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x0);
            HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x1);
            HdIO.Sleep(10);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Calc_Start, 0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Calc_Start, 1);
            while (true)
            {
                var value = Hd.CurrProduct?.AcqBd?.ReadFromAllFpga(AcqBdReg.R.MDO_DDC_fd_rough_finish, 7);
                int ValidBitsCnt = 0;
                if (value != null)
                {
                    foreach (UInt32 return_value in value.Values)
                    {
                        if (return_value == 7)
                        {
                            ValidBitsCnt += 1;
                        }
                    }
                    if (ValidBitsCnt > 0)
                    {
                        break;
                    }
                }
            }

            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_SoftStopAcq, 1);
            HdIO.Sleep(10);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_fd_enable_set, 1);
            HdIO.Sleep(10);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_SoftStopAcq, 0);
            UInt32 ddr_discard_num = 0;
            UInt32 ddr_discard_num_offset = 0;
            UInt32 ddr_discard_num_sum = 0;
            Boolean ddr_discard_num_vaild = HdIO.CheckRegisterValue((uint)AcqBdReg.R.LSCtrl_DDRReadTrigDiscardNumflag, (UInt32)trigacqbd, 0x1, 1);
            if (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.HdMessage?.Timebase?.SegmentActive == 1)
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_TrigDiscardMode, 0);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDRTrigDiscardNumValid, 0x0);
            }
            else
            {
                if (ddr_discard_num_vaild)
                {
                    ddr_discard_num = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.LSCtrl_ReadTrigDiscardNum, trigacqbd) & 0xFFFF ?? 0;
                    ddr_discard_num_offset = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.LSCtrl_DDRReadTrigDiscardNumOffset, trigacqbd) & 0xFFFF ?? 0;
                    ddr_discard_num_sum = ddr_discard_num + ddr_discard_num_offset;
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_TrigDiscardMode, (Hd.UIMessage?.bAcquireStopped ?? false) ? 0U : 1U);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_TrigDiscardNum, ddr_discard_num_sum);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDRTrigDiscardNumValid, 0x0);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDRTrigDiscardNumValid, 0x1);
                }
                else
                {
                    ddr_discard_num = 0;
                    ddr_discard_num_offset = 0;
                    ddr_discard_num_sum = 0;
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_TrigDiscardMode, 0);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_TrigDiscardNum, ddr_discard_num_sum);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDRTrigDiscardNumValid, 0x0);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDRTrigDiscardNumValid, 0x1);
                }
            }

            Int64 readTimes = totalLength / FifoDepth + ((totalLength > FifoDepth) ? 1 : 0);
            for (int i = 0; i < readTimes; i++)
            {
                retVal = HdIO.DMARead(8 * 512 * 512, ref DMADataXi);
                if (retVal)
                {
                    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_RF_select, 0x0);//0：普通时频 1：粗时频使能 2：细时频使能 
                }
                Array.Copy(DMADataXi, 0, DMATotalData, 8 * DestinationIndex, 8 * 512 * 512);
                DestinationIndex += (int)FifoDepth;
                if (i == readTimes - 2 && totalLength > FifoDepth)
                {
                    FifoDepth = (uint)totalLength % FifoDepth;
                    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_ScaleSCH, FifoDepth);//fft之后fifo深度配置
                   // HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, FifoDepth);//PCIE FIFO深度//wby
                }
                //HdIO.WriteReg(ProcBdReg.W.MDO_STFT_FFTTimes, 0);//------------------------------------------------------------------------------------------------lhc
                //HdIO.WriteReg(ProcBdReg.W.MDO_STFT_FFTTimes, 1);//------------------------------------------------------------------------------------------------lhc
            }
            if (AVTON || PVTON || FVTON)//如果幅度域相位域开关被打开，读取原始I、Q数据
            {
                //HdIO.Sleep(10);
                //HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x0);
                //HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x1);
                Int64 IQLength = Hd.UIMessage?.MultiDomain?.STFTLength ?? 1024;
                if (IQLength > FifoMaxDepth)
                    IQLength = FifoMaxDepth;
                //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, (UInt32)IQLength - 2);//PCIE FIFO深度
                HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x0);
                HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, 0x1);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_data_select, 0x0001);
                //retVal = CheckRegValue((UInt32)PcieBdReg.R.FifoCtrl_FullFlagRead, 1, 1000);
                //if (!retVal)
                //    return retVal;
                retVal = HdIO.DMARead(8 * FifoDepth, ref DMAData);//(乘以2是因为前面8 * FifoDepth是FFT结果，后面8 * FifoDepth是混频后的时域结果)
                Array.Copy(DMAData, 0, IQDMAData, 8 * DestinationIndex1, 8 * FifoDepth);
                DestinationIndex1 += (int)FifoDepth;
            }
            HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkdemux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DataPath_acq_linkmux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);

            return true;
        }

        protected virtual bool AcqAnalogChannelSimulateWaveform()
        {
            Monitor.Enter(AllChannelWaveDataLock);
            AllChannelWaveData.Clear();
            int Length = Constants.CHNL_DATA_NUM;
            double SampIntByns = Hd.UIMessage?.Timebase?.TmbScale * Constants.VIS_XDIVS_NUM * 1000 / Constants.CHNL_DATA_NUM ?? 0.5;
            var cycles = Length * (SampIntByns * 1E-9) * (Constants.AWG_SIN_FRQ_DEF * 1E-6);
            double NoiseByPercent = 0.05;
            ArbWfmType[] allChannelArbWfmType = { ArbWfmType.Sinc, ArbWfmType.Square, ArbWfmType.Ramp, ArbWfmType.Haversine, ArbWfmType.Sinc, ArbWfmType.Square, ArbWfmType.Ramp, ArbWfmType.Haversine };
            for (int channelID = 0; channelID < ChannelIdExt.AnaChnlNum; channelID++)
            {
                double anaChannelPosition = 0;// Constants.IDX_PER_YDIV * 5;// Hd.UIMessage?.Analog?[channelID].PositionIndex ?? 0;
                double amplitude = Constants.IDX_PER_YDIV * 4;// (Hd.UIMessage?.Analog?[channelID].Scale ?? 0) * 6;
                ArbWfmType arbWfmType = allChannelArbWfmType[channelID];
                IEnumerable<Double> y = arbWfmType switch
                {
                    ArbWfmType.Pulse or ArbWfmType.Square => Generator.Rectangular(anaChannelPosition, amplitude, cycles / Length, Length, 0.05, NoiseByPercent, 0.1),
                    ArbWfmType.DC => Generator.DirectCurrent(anaChannelPosition, amplitude, Length, 0.05),
                    ArbWfmType.Haversine => Generator.Haversine(anaChannelPosition, amplitude, cycles / Length, Length, NoiseByPercent, 0.05),
                    _ => Generator.Sinc(anaChannelPosition, amplitude, cycles / Length, Length, NoiseByPercent, 0.5, 0),
                };
                var pos0 = (Hd.UIMessage?.Analog?[channelID].PositionIndex ?? 0) / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
                y = y.Select((o) => o /*(Hd.UIMessage?.Analog?[channelID].Scale ?? 100)*/ / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + pos0);
                Double[] data = y.ToArray();// .ToRowVector();
                AllChannelWaveData.Add(new List<ushort>());
                for (int i = 0; i < Length; i++)
                    AllChannelWaveData[channelID].Add((ushort)data[i]);
            }
            AcquedParameters.PerDataByfs_AfterPostProcess = (long)(Hd.UIMessage?.Timebase?.TmbScale * Constants.VIS_XDIVS_NUM * 1000_000_000 / Constants.CHNL_DATA_NUM ?? 50 * 1000);
            Monitor.Exit(AllChannelWaveDataLock);
            return true;
        }

        //public static RFWindowType WindowType;
        //public static Int32 FFTLength = 1024;

        //protected static void ConfigWindow()
        //{
        //    Int32 CurrentRFChannel = Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId;
        //    Int32 length = (Int32)(Hd.UIMessage?.MultiDomain?.FFTLength ?? 1024);
        //    RFWindowType windowType = Hd.UIMessage?.MultiDomain?.WindowType ?? RFWindowType.Hann;
        //    if (WindowType == windowType && FFTLength == length)
        //    {
        //        return;
        //    }
        //    WindowType = windowType;
        //    FFTLength = length;
        //    //Thread.Sleep(3000);

        //    List<double> coefficient = CtrlRadioFrequency_Standard.GetWindowCoefficient(length, windowType).ToList();
        //    for (int i = 0; i < coefficient.Count; i++)
        //    {
        //        //UInt32 value = (UInt32)(coefficient[i]);
        //        //UInt32 value = BitConverter.ToUInt32(BitConverter.GetBytes(3.255));
        //        byte[] floatBytes = BitConverter.GetBytes((float)coefficient[i]);
        //        // byte[] floatBytes = BitConverter.GetBytes(3.255f);

        //        UInt16 low = (UInt16)(floatBytes[1] << 8 & 0xff00 | floatBytes[0] & 0x00ff);
        //        UInt16 high = (UInt16)(floatBytes[3] << 8 & 0xff00 | floatBytes[2] & 0x00ff);
        //        //UInt16 valueH = (UInt16)(value >> 16 & (0xffff));
        //        //UInt16 valueL = (UInt16)(value & (0xffff));

        //        //HdIO.WriteReg(ProcBdReg.W.MDO_STFT_CoefficientDataInH16, high);//暂时是写死的矩形窗lhc
        //        //HdIO.WriteReg(ProcBdReg.W.MDO_STFT_CoefficientDataInL16, low); //暂时是写死的矩形窗lhc
        //        HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_coefficient_datain_h16, high);//暂时是写死的矩形窗lhc
        //        HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_coefficient_datain_l16, low); //暂时是写死的矩形窗lhc
        //        HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_Coefficient_Data_WREN, 0);
        //        HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_Coefficient_Data_WREN, 1);

        //    }
        //    HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_Coefficient_Data_WREN, 0);
        //}

        private Double GetPhase(Double i, Double q)
        {
            Double phase = 0;
            if (i > 0 && q > 0)
                phase = Math.Atan(q / i) / Math.PI * 180;
            else if (i > 0 && q < 0)
                phase = Math.Atan(q / i) / Math.PI * 180;
            else if (i < 0 && q > 0)
                phase = Math.Atan(q / i) / Math.PI * 180 + 180;
            else if (i < 0 && q < 0)
                phase = Math.Atan(q / i) / Math.PI * 180 - 180;
            return phase;
        }

        private Boolean DataPostProcess(double originalData, double DDCgain, out double dataAfterProcess, double wh, double Freq)
        {
            double ADCReflevel = 0;
            double AnalogADCReflevel = 0;
            double RFComp;
            if (originalData == null)
            {
                dataAfterProcess = 0;
                return false;
            }
            Int32 channelid = (Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId);
            ChannelId source = Hd.UIMessage?.MultiDomain?.Source ?? ChannelId.C1;
            RFWindowType windowType = Hd.UIMessage?.MultiDomain?.WindowType ?? RFWindowType.Hann;
            Int32 fftLength = (int)(Hd.UIMessage?.MultiDomain?.FFTLength ?? 1024);
            Int64 centerfrequency = (long)(Hd.UIMessage?.MultiDomain?.CenterFreqByHz ?? 1024);
            Int64 span = (long)(Hd.UIMessage?.MultiDomain?.SpanByHz ?? 1024);

            Double w = AbstractAcquirer_RadioFrequency.WindowGainTable[windowType];
            var reflevel = Hd.UIMessage?.RadioFrequency?[channelid].ReferenceLevel ?? 0;
            if (centerfrequency > 500_000_000)
                ADCReflevel = 0.54;
            else if (centerfrequency <= 500_000_000 && span == 1000_000_000)
                ADCReflevel = 0.4;
            else
                ADCReflevel = 0.4;
            HdMessage.AnalogOptions analogParameters = Hd.UIMessage!.Analog![(int)GetAnalogChnlId[source]];
            AnalogADCReflevel = GetAnalogADCRef[(int)GetAnalogChnlId[source], analogParameters.ScaleIndex];
            //将量化值转换为模拟电压值                                                             射频通道         模拟通道，下同
            //originalData = originalData / fftLength / Math.Pow(2, 11) * ((source == ChannelId.RF) ? ADCReflevel : AnalogADCReflevel);
            originalData = originalData / fftLength / 1800;
            originalData = originalData * ((source == ChannelId.RF) ? 1 : (analogParameters.Scale * 10 / 1000));
            //补偿DDC模块的衰减
            originalData = originalData / DDCgain;
            //补偿滤波器滤去的另一半信号的能量并转换为有效值
            originalData = originalData * 2 / Math.Sqrt(2);
            //求功率
            originalData = originalData / Math.Sqrt(50);
            //对经过不同窗函数的数据进行修正
            originalData = originalData * Math.Sqrt(fftLength / wh);
            //转换为功率谱，单位dB
            //originalData = 10 * Math.Log10(originalData);
            //originalData = originalData + 30 + ((source == ChannelId.RF) ? reflevel : 0);
            //if (originalData > -45)
            //{
            //    RFComp = GetRFCompensationCoe(Freq, centerfrequency);//该函数是为了补偿射频通道0-1GHz带宽内滤波器平坦度不佳带来的影响
            //    originalData = originalData + ((source == ChannelId.RF) ? RFComp : 0);
            //}
            dataAfterProcess = originalData;
            return true;
        }

        private double GetRFCompensationCoe(double Freq, Int64 centerFreq)
        {
            Int32 channelid = (Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId);
            if ((Hd.UIMessage?.RadioFrequency?[channelid].Source ?? ChannelId.C1) != ChannelId.RF)
                return 0;
            double CompA, CompB;
            uint mode = 0;
            uint FreqSegment;
            double RealSigFreq = (centerFreq - 500_000_000 + Freq) / 1000000000;
            if (RealSigFreq < 1.5)
                RealSigFreq = 1.5;
            if (RealSigFreq > 7.5)
                RealSigFreq = 7.5;
            if (Freq < 0)
                Freq = 0;
            if (Freq > 1_000_000_000)
                Freq = 1_000_000_000;
            if (centerFreq > 2_000_000_000)
                CompA = (-1) * (Fkb[(uint)(RealSigFreq - 1.5), 0] * (RealSigFreq - 1.5 - (uint)(RealSigFreq - 1.5)) + Fkb[(uint)(RealSigFreq - 1.5), 1]);
            //CompA = 0;
            else
                CompA = 0;
            if (centerFreq > 500000000)
            {
                mode = 1;
                Freq = 1000000000 - Freq;
            }
            Freq /= 1000000;
            FreqSegment = (uint)Freq / 100;
            if (FreqSegment > 9)
                FreqSegment = 9;
            CompB = (-1) * (kb[mode, FreqSegment, 0] * Freq + kb[mode, FreqSegment, 1]);
            return CompA + CompB;
        }

        private bool Cu_Write_Finish()
        {
            var readBack = Hd.CurrProduct?.AcqBd?.ReadFromAllFpga(AcqBdReg.R.LSCtrl_DDRDataUpdateFlag, 0x1);
            if (readBack == null)
                return false;

            List<AcqBdNo> checkAcqBdList = new();
            foreach (ChannelId chnlid in ChannelIdExt.GetAnalogs())
            {
                //var adcuseinfo = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(Acquirer_AnalogChanel_DBI13G.DbiActuallActiveState, chnlid);
                var adcuseinfo = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(1, chnlid);
                if (adcuseinfo != null)
                {
                    checkAcqBdList.AddRange(adcuseinfo.Select(o => o.AcqBdNo));
                }
            }
            //checkAcqBdList = new List<AcqBdNo> { AcqBdNo.B7};
            checkAcqBdList = new List<AcqBdNo> { AcqBdNo.B10 };//lhc----------------------------------------------------------------------
            foreach (AcqBdNo acqBd in checkAcqBdList)
            {
                if (readBack.ContainsKey(acqBd) && readBack[acqBd] != 1)
                {
                    return false;
                }
            }
            return true;
        }

        private Boolean FFTDataCut(List<Double> originalData, Int32 fftLength, Int64 spanSoft, Int64 spanHardware, out List<Double>? dataAfterCut)
        {
            if (originalData.Count != fftLength | originalData.Count == 0 | spanSoft > spanHardware)
            {
                dataAfterCut = null;
                return false;
            }

            dataAfterCut = new List<double>();

            var start = fftLength / 2 - Math.Ceiling(fftLength * 0.4 * spanSoft / spanHardware);
            var length = Math.Ceiling(fftLength * 0.4 * spanSoft / spanHardware) * 2;
            originalData.RemoveRange(0, (int)start);
            originalData.RemoveRange((int)length, (int)(fftLength - length - start));
            dataAfterCut = originalData;

            return true;
        }

        private Boolean FFTDataCutE(List<Double> originalData, Int32 fftLength, Int64 centerFrenquency, Int64 spanSoft, Int64 spanHardware, Double SampleRateHardware, out List<Double>? dataAfterCut)
        {
            if (originalData.Count != fftLength)
            {
                dataAfterCut = null;
                return false;
            }

            dataAfterCut = new List<double>();

            List<Double> frontdata = new();
            List<Double> aftercutdata = new();
            dataAfterCut = new List<double>();
            frontdata = originalData.GetRange(0, (int)(0.25 * originalData.Count));
            //aftercutdata = originalData.GetRange((int)(0.875 * fftLength), fftLength - (int)(0.875 * fftLength));
            //aftercutdata.AddRange(frontdata);

            var length = Math.Ceiling(fftLength * 0.41 * spanSoft / spanHardware);
            var start = Math.Floor(fftLength * 0.4 * (centerFrenquency - spanSoft / 2) / 20_000_000_000);//8_000_000_000

            Double CutfftCoefficient = 0.5 - (spanSoft / 2.0 / SampleRateHardware);
            originalData.RemoveRange(0, (int)(CutfftCoefficient * originalData.Count));

            dataAfterCut = originalData;

            return true;
        }

        private bool CheckRegValue(UInt32 regAddr, UInt32 expectValue, Int32 overTimeByMs)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < overTimeByMs)
            {
                if (HdIO.ReadReg(regAddr) == expectValue)
                    return true;
            }
            return false;
        }

        private static double GetValidSpan(double span)
        {
            if (span <= 100_000_000)
            {
                return 100_000_000;
            }
            else if (span <= 300_000_000)
            {
                return 300_000_000;
            }
            else if (span <= 500_000_000)
            {
                return 500_000_000;
            }
            else
                return 1_000_000_000;
        }

        private void SaveDataToFile(List<Double> data, string file_name)
        {
            StreamWriter sw = new StreamWriter(@file_name, false);
            for (int i = 0; i < data.Count; i++)
            {
                sw.WriteLine(data[i]);
            }
            sw.Close();
        }

        Dictionary<ChannelId, ChannelId> GetAnalogChnlId = new Dictionary<ChannelId, ChannelId>()
        {
            { ChannelId.C1,ChannelId.C1 },
            { ChannelId.C2,ChannelId.C2 },
            { ChannelId.C3,ChannelId.C3 },
            { ChannelId.C4,ChannelId.C4 },
            { ChannelId.RF, ChannelId.C4 },
        };

        double[,] GetAnalogADCRef = //获取不同通道不同档位下ADC满量程电压的校准值,用来定标频域FFT的功率谱
        {
            { 0,0,0,0,0.42,0.41 ,0.41,0.25,0.415,0.51,0.52},
            { 0,0,0,0,0.42,0.42 ,0.4,0.41,0.41,0.415,0.41},
            { 0,0,0,0,0.41,0.41 ,0.42,0.52,0.41,0.41,0.42},
            { 0,0,0,0,0.41,0.36 ,0.41,0.41,0.4,0.41,0.38},
        };

        double[,,] kb =
        {
                // k  b  
            {
                { 0.23,  -28.2 },
                { 0.034,  -8.6 },
                { 0.016,  -5 },
                { 0,  0.35 },
                { -0.007,  2.5 },
                { -0.003,  0.5 },
                { 0,  -1.45 },
                { -0.01,  5.6 },
                { -0.016,  10.8 },
                { -0.016,  12.4 },
            },
            {
                { 0.007,  -2.1 },
                { 0.007,  -2.2 },
                { 0.0125,  -3.05 },
                { 0,  0.05 },
                { -0.004,  1.45 },
                { 0,  -0.65 },
                { 0.003,  -2.3 },
                { -0.007,  4.7 },
                { -0.02,  15.1 },
                { -0.019,  14 },
            },
        };

        double[,] Fkb =
        {
                { -2.4,  0 },
                { -2.6,  -2.4 },
                { -2,  -5 },
                { -0.7,  -7 },
                { -4.5,  -7.7 },
                { -3.1,  -12.2 },
                { 0,  -15.3 },
        };
    }

    public enum MultiDomainDataTypeEnum
    {
        IFFT,
        QFFT,
        AmpVSTime,
        PhaseVSTime,
        Spectrogram,
        WfmParams,
    }
}
