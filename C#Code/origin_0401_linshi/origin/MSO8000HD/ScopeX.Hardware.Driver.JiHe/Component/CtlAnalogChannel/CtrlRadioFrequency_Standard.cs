using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    public class CtrlRadioFrequency_Standard
    {
        public static void CtrlConfig()
        {
            MD8G_RFCH_SendASCII_Config();//配置射频通道
            CtrlConfigSTFT();
            CtrlEnableSTFT();
            CtrlDDS();
            CtrlDDCDecimation();
            CtrlFreTransLength();
        }

        #region TeacherJin 通道控制
        //根据中心频率和span得到发往射频通道的模式值,混频频率,滤波器带宽和衰减倍数,均用字符串发送。
        //模式值2代表直通,3代表混频,混频频率用四位十进制字符串表示，单位是MHz，“1000”表示混频频率为1000MHz。
        //滤波器带宽分为四档，1代表100MHz带宽，2代表300MHz带宽，3代表500MHz带宽，4代表1000MHz带宽。
        //衰减倍数步进为3，00代表衰减倍数为0，03代表衰减倍数为3dB。
        //例：Com：1500：3：4：06@,表示混频频率1500MHz，1000MHz滤波器，衰减6dB。
        public static void MD8G_RFCH_SendASCII_Config()
        {
            ChannelId source = Hd.UIMessage?.RadioFrequency?[(Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId)].Source ?? ChannelId.C1;
            if (source == ChannelId.RF)
            {
                char Mode = '2';
                char Filter_Bandwidth = '4';
                double fchardware = (Hd.UIMessage?.RadioFrequency?[(Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId)].CenterFrequency ?? 500);
                var span = Hd.UIMessage?.RadioFrequency?[(Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId)].Span ?? 1000;
                var reflevel = Hd.UIMessage?.RadioFrequency?[(Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId)].ReferenceLevel ?? 0;
                if (fchardware > 500_000_000)
                {
                    Mode = '3';
                }

                if (span <= 100_000_000)
                {
                    Filter_Bandwidth = '1';
                    span = 100_000_000;
                }
                else if (span <= 300_000_000)
                {
                    Filter_Bandwidth = '2';
                    span = 300_000_000;
                }
                else if (span <= 500_000_000)
                {
                    Filter_Bandwidth = '3';
                    span = 500_000_000;
                }
                else
                    span = 1_000_000_000;
                fchardware += span / 2;
                fchardware /= 1000_000;//Hz->MHz
                string CentreFreq = ((fchardware < 1000) ? "0" : "") + fchardware.ToString("f0");
                if (Mode == '2')
                {
                    CentreFreq = "3000";
                    Filter_Bandwidth = '4';
                }
                Hd.CurrProduct?.AcqBd?.MiscFunc($"SendRadioAdcGain_{Mode.ToString()}{Filter_Bandwidth.ToString()}");

                List<double> RefLevel0Attenuation = Get0RefLevelAttenuation(Mode, Filter_Bandwidth);
                string SendToChnlAttenuation = GetSendAttenuation(RefLevel0Attenuation[0] + reflevel);
                MD8G_RFCH_SendASCII(CentreFreq, Mode, Filter_Bandwidth, SendToChnlAttenuation);
            }
        }
        public static Dictionary<char, Dictionary<char, List<Double>>> RefLevel0Attenuation = new Dictionary<char, Dictionary<char, List<Double>>>()
        {
            ['2'] = new Dictionary<char, List<Double>>()
            {
                ['1'] = new List<Double>()
                {
                   24,0
                },
                ['2'] = new List<Double>()
                {
                   24,0
                },
                ['3'] = new List<Double>()
                {
                    24,0
                },
                ['4'] = new List<Double>()
                {
                    24,0
                },
            },
            ['3'] = new Dictionary<char, List<Double>>()
            {
                ['1'] = new List<Double>()
                {
                   21,15000
                },
                ['2'] = new List<Double>()
                {
                   21,10500
                },
                ['3'] = new List<Double>()
                {
                   21,10500
                },
                ['4'] = new List<Double>()
                {
                   21,1000
                },
            },

        };
        public static List<double> Get0RefLevelAttenuation(char mode, char filter_bandwidth)
        {
            return RefLevel0Attenuation[mode][filter_bandwidth];
        }
        public static string GetSendAttenuation(double attenuation)
        {
            if (attenuation < 0)
                attenuation = 0;
            if (attenuation > 99)
                attenuation = 99;
            if (attenuation >= 10)
            {
                return attenuation.ToString("f0");
            }
            else
            {
                return "0" + attenuation.ToString("f0");
            }
        }
        public static void MD8G_RFCH_SendASCII(string Fre_LO, char Mode, char Filter_Bandwidth, string attenuation)
        {
            //                           C   O    M    :  0,0,0,0   :   0   :   0   :   0,0    @      //本振频率：直通/混频：输出滤波器选择：衰减倍率         
            Int32[] CMD_array_ASCII = { 67, 111, 109, 58, 0, 0, 0, 0, 58, 0, 58, 0, 58, 0, 0, 64 };  // ASCII指令初始化
            CMD_array_ASCII[4] = (Int32)(Fre_LO[0]);
            CMD_array_ASCII[5] = (Int32)(Fre_LO[1]);
            CMD_array_ASCII[6] = (Int32)(Fre_LO[2]);
            CMD_array_ASCII[7] = (Int32)(Fre_LO[3]);
            CMD_array_ASCII[9] = (Int32)(Mode);
            CMD_array_ASCII[11] = (Int32)(Filter_Bandwidth);
            CMD_array_ASCII[13] = (Int32)(attenuation[0]);
            CMD_array_ASCII[14] = (Int32)(attenuation[1]);

            MD8G_SendASCII((UInt32)PcieBdReg.W.DBI_LO_DBI_bin_en_ch1, (UInt32)PcieBdReg.W.DBI_LO_DBI_bin_wen_ch1, (UInt32)PcieBdReg.W.DBI_LO_DBI_bin_data_ch1, CMD_array_ASCII);//DXF

        }
        public static void MD8G_SendASCII(UInt32 DBI_BIN_EN, UInt32 DBI_BIN_WEN, UInt32 DBI_BIN_DATA, int[] CMD_array_ASCII)
        {
            int k;
            HdIO.WriteReg(DBI_BIN_EN, 0);
            for (k = 0; k < 16; k++)
            {
                HdIO.WriteReg(DBI_BIN_WEN, 0);
                HdIO.WriteReg(DBI_BIN_DATA, (UInt32)CMD_array_ASCII[k]);
                HdIO.WriteReg(DBI_BIN_WEN, 1);

            }
            HdIO.WriteReg(DBI_BIN_WEN, 0);
            HdIO.WriteReg(DBI_BIN_EN, 1);
            Thread.Sleep(100);
            HdIO.WriteReg(DBI_BIN_EN, 0);

        }
        #endregion
        public static void CtrlConfigSTFT()
        {
            double number_log = (double)(Hd.UIMessage?.MultiDomain?.FFTLength ?? 1024);
            
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_NFFT, (uint)Math.Log(number_log,2));
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_NFFT, 14);
            //HdIO.WriteReg(ProcBdReg.W.stft_FFT_Param_NFFT, 10);
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Step, 255);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_STFT_Step, (uint)(Hd.UIMessage?.MultiDomain?.STFTStep ?? 1024));
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_fifo_full_thresh, 1024);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_stft_fifo_full_thresh, (uint)(Hd.UIMessage?.MultiDomain?.FFTLength ?? 1024) - 2);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_frefifo_empty_thresh, 0x0);
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_PointNum, 1023);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Param_PointNum, (uint)(Hd.UIMessage?.MultiDomain?.FFTLength ?? 1024) - 1);

            // HdIO.WriteReg(ProcBdReg.W.MDO_STFT_FFTParamDir, 1);
            // HdIO.WriteReg(ProcBdReg.W.MDO_STFT_FFTConfigStart, 0);
            // HdIO.WriteReg(ProcBdReg.W.MDO_STFT_FFTConfigStart, 1);
           
        }


        public static void CtrlEnableSTFT()
        {
            //HdIO.Sleep(2000);
            //HdIO.WriteReg(ProcBdReg.W.MDO_STFT_STFTCalcStart, 0);
            //HdIO.WriteReg(ProcBdReg.W.stft_STFT_Calc_Start, 1);

            //Int32 channelid = (Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId);
            var isChannelActive = Hd.UIMessage?.MultiDomain?.Active ?? false;
            if (isChannelActive == true)
            {
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Config_Start, 0);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_FFT_Config_Start, 1);
            }
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
        public static void CtrlDDS()
        {
            //double fs_sys = 20_000_000_000;
            //double fs_sys = 60_000_000_000;//lhc-----------------------------------------------------------------------------------------------------------------
            var sampleinterval = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquedParameters?.PerDataByfs_AtDdr ?? 50000;

            double fs_sys = 4e15 / sampleinterval / Math.Pow(2,(int)Hd.UIMessage?.Precision?.AnaChnlBitWidth - 12);  //mry
            //double fchardware = fs_sys - (Hd.UIMessage?.RadioFrequency?[(Int32)(CurrentRFChannel-ChannelIdExt.MinRFChId)].CenterFrequency ?? 500);
            ////double fchan = 250_000_000;
            //double nph = 31;
            //Int32 ph_step_inside_chan_out_range = (Int32)(Math.Floor(fchardware / fs_sys * Math.Pow(2,nph)));
            var span = Hd.UIMessage?.MultiDomain?.SpanByHz ?? 1000;
            double fchardware = -(Hd.UIMessage?.MultiDomain?.CenterFreqByHz ?? 500);
            ChannelId source = Hd.UIMessage?.MultiDomain?.Source ?? ChannelId.C1;
            if ((source == ChannelId.C1 || source == ChannelId.C2 || source == ChannelId.C3 || source == ChannelId.C4) && source == ChannelId.RF)
            {
                //fs_sys = 10_000_000_000;

                fs_sys = Constants.SAMPLING_RATE;
                if (fchardware < -500_000_000)
                {

                    fchardware = -GetValidSpan(span) / 2;
                }
            }
            //double fchan = fs_sys / 8;
            double fchan = fs_sys ;
            double nph = 28;
            fchardware = fchardware + fs_sys;
            UInt32 ph_step_inside_chan_out_range = (UInt32)(Math.Round(fchardware / fchan * Math.Pow(2, nph)));
            double ph_inside_chan_out_range = fchardware / fs_sys * Math.Pow(2, nph);
            //double important_freq_centre = ph_step_inside_chan_out_range * fchan / Math.Pow(2, nph);

            //double ph_step_between_chan_out_range_double = fchardware * 2 / fs_sys * Math.Pow(2, nph);
            //double ph_step_between_chan_out_range_double_seq;
            UInt32 ph_to_FPGA;
            UInt32 ph_step_to_FPGA;
            ph_step_to_FPGA = (UInt32)(ph_step_inside_chan_out_range % Math.Pow(2, nph));
            //double ph_step_between_chan_out_range_double_seq = Math.Floor(ph_step_between_chan_out_range_double.* (0:79));
            //double ph_step_between_chan_in_range = ph_step_between_chan_out_range_double_seq % Math.Pow(2, nph);
            //double ph_to_FPGA = [ph_step_between_chan_in_range ph_step_inside_chan_in_range];

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_DDSWen, 0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 0);
            for (int i = 0; i < 1; i++)
            {
                ph_to_FPGA = (UInt32)(Math.Round(ph_inside_chan_out_range * i) % Math.Pow(2, nph));

                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_l, ph_to_FPGA);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_h, ph_to_FPGA >> 16); 
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_DDSAddr, (UInt16)ph_step_inside_chan_out_range);  //新结构中这个寄存器改为步进低16比特
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_DDSWen, (UInt16)(ph_step_inside_chan_out_range >> 16)); //新结构中这个寄存器改为步进高16比特
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_addr, (uint)i);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 1);
                HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 0);
            }

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_DDSDataL, ph_step_to_FPGA); 
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_DDSDataH, ph_step_to_FPGA >> 16); 
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_l, ph_step_to_FPGA);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_data_h, ph_step_to_FPGA >> 16);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_addr, 1);

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_wr_en, 0);

            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ph_finish, 0);
        }


        public static void CtrlDDCDecimation()
        {
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_en, 0x1);//lhc----------------------------------------------------------------------
            var span = Hd.UIMessage?.MultiDomain?.SpanByHz ?? 1000;
            ChannelId source = Hd.UIMessage?.MultiDomain?.Source ?? ChannelId.C1;
            var key = AbstractAcquirer_RadioFrequency.GetRFHDScale(span, source);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_CIC_rst, 0x1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_CIC_rst, 0x0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_dec_switch, (UInt16)key.Key);
            //HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_dec_switch, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_FreDeciMode, 25);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_SerialCIC_Config_Tvalid, 0x1);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_fifo_thresh, 16382);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_SerialCIC_Config_Tvalid, 0x0);
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_ddc_en, 0x0);//lhc----------------------------------------------------------------------
        }

        public static void CtrlFreTransLength()
        {
            HdIO.WriteReg(ProcBdReg.W.MDO_DDC_fd_frefifo_full_thresh, (uint)(Hd.UIMessage?.MultiDomain?.FFTLength ?? 1024) - 2);
        }


        #region GetWindowCoefficient

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
                    return w.Select(o => 0.42 - 0.5 * Math.Cos(o) + 0.08 * Math.Cos(2*o));
                case RFWindowType.Flattop:
                    Double fta0 = 0.21557895;
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
                double x = beta * Math.Sqrt(1 - Math.Pow(1 - (double)2 * i / (length - 1), 2));
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
    }
}
