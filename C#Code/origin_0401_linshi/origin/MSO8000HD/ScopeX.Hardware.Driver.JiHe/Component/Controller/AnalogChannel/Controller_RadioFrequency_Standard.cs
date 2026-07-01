using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal class Controller_RadioFrequency_Standard : AbstractController_RadioFrequency
    {
        public Controller_RadioFrequency_Standard()
        {
            _CtrlConfig = CtrlRadioFrequency_Standard.CtrlConfig;
            _CtrlConfigWindow = ConfigWindow;
            _CtrlSpan = Span;
            _CtrlConfig += FFTOutType;
            _CtrlSTFTSource = STFTSource;
            ConfigChannelInfomation();
            STFTSource();
            FFTOutType();
            ConfigWindow();
        }

        public static ChannelId CurrentRFChannel = ChannelId.RF1;
        public static Double RBWHardware = 1024;
        public static Int64 SpanHardware = 1024;
        public static RFWindowType WindowType ;
        public static Int32 FFTLength = 1024;

        protected static void ConfigChannelInfomation()
        {
            //ChannelId CurrentRFChannel = Hd.UIMessage?.RadioFrequency?[(Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId)].Source ?? ChannelId.C1;
            //RBWHardware = Hd.UIMessage?.RadioFrequency?[Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId].RBW ?? 1024;
            //SpanHardware = (AbstractAcquirer_RadioFrequency.GetRFHDScale(Hd.UIMessage?.RadioFrequency?[Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId].Span ?? 1024, CurrentRFChannel)).Span;
            //WindowType = Hd.UIMessage?.RadioFrequency?[(Int32)(CurrentRFChannel - ChannelIdExt.MinRFChId)].Window ?? RFWindowType.Rectangle;
        }


        protected static void ConfigWindow()
        {
            Int32 CurrentRFChannel = Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId;
            Int32 length = (Int32)(Hd.UIMessage?.MultiDomain?.FFTLength ?? 1024);
            RFWindowType windowType = Hd.UIMessage?.MultiDomain?.WindowType ?? RFWindowType.Hann;
            if (WindowType == windowType && FFTLength == length)
            {
                return;
            }
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

        public static void Span()
        {
            Int64 span;
            Int32 channelid = (Int32)(Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId);
            ChannelId source = Hd.UIMessage?.RadioFrequency?[channelid].Source ?? ChannelId.C1;
            SpanHardware = (AbstractAcquirer_RadioFrequency.GetRFHDScale(Hd.UIMessage?.RadioFrequency?[channelid].Span ?? 1024, source)).Span;
            //HdIO.WriteReg(ProcBdReg.W.MDO_STFT_STFTDataSelect, 0x0008);
            FFTOutType();
        }

        public static void STFTSource()
        {
            //HdIO.WriteReg(ProcBdReg.W.MDO_STFT_DataChoose, 0b1011);
        }

        public static void FFTOutType()
        {
            Int32 CurrentRFChannel = Controller_RadioFrequency_Standard.CurrentRFChannel - ChannelIdExt.MinRFChId;
            Boolean AVTON = Hd.UIMessage?.RadioFrequency?[CurrentRFChannel].AVTON ?? false;
            Boolean PVTON = Hd.UIMessage?.RadioFrequency?[CurrentRFChannel].PVTON ?? false;
            if (AVTON || PVTON)
            {
                //HdIO.WriteReg(ProcBdReg.W.MDO_STFT_STFTDataSelect, 0x0001);
            }
            else
            {
                //HdIO.WriteReg(ProcBdReg.W.MDO_STFT_STFTDataSelect, 0x0002);
            }

        }
    }
}
