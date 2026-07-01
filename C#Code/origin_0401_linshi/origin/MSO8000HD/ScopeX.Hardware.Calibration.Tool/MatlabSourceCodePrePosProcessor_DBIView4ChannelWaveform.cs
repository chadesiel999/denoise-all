using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.Hardware;
using ScopeX.Hardware.Calibration.Tool.Utilities;
namespace ScopeX.Hardware.Calibration.Tool;


public class MatlabSourceCodePrePosProcessor_DBIView4ChannelWaveform : IMatlabSourceCodePrePosProcessor
{
    void IMatlabSourceCodePrePosProcessor.GetOutputData(MLApp.MLApp? matLabApp, IInstrumentSession? currInstrumentSession)
    {
        //int length = 100;
        //Array buffer=new Double[length];

        //Array pi = new Double[length];
        //Array gain_error_rel = new double[8];
        //Array gain_error_im = new double[8];
        //Array time_error_rel = new double[8];
        //Array time_error_im = new double[8];
        //Array offset_error_rel = new double[8];
        //Array offset_error_im = new double[8];

        //matLabApp.GetFullMatrix("result", "base", ref buffer, ref pi);//如果Matlab环境中只有实数部分的数值，如此返回会出现运行时异常
        //matLabApp.GetFullMatrix("gerror", "base", ref gain_error_rel, ref gain_error_im);//如果Matlab环境中只有实数部分的数值，如此返回会出现运行时异常
        //matLabApp.GetFullMatrix("dterror", "base", ref time_error_rel, ref time_error_im);//如果Matlab环境中只有实数部分的数值，如此返回会出现运行时异常
        //matLabApp.GetFullMatrix("oerror", "base", ref offset_error_rel, ref offset_error_im);//如果Matlab环境中只有实数部分的数值，如此返回会出现运行时异常


        //List<double> gainList = new List<double>(gain_error_rel as double[]);
        //List<double> phaseList = new List<double>(time_error_rel as double[]);
        //List<double> offsetList = new List<double>(offset_error_rel as double[]);
        //Array dataout = new short[1000];

        //Array imaginary = new short[1000];

        //matLabApp.GetFullMatrix("dataout", "base", ref dataout, ref imaginary);//如果Matlab环境中只有实数部分的数值，如此返回会出现运行时异常
    }
    void IMatlabSourceCodePrePosProcessor.PutInputData(MLApp.MLApp? matLabApp, IInstrumentSession? currInstrumentSession)
    {
        //Array lo_addr = new Double[1];
        //Array lo_addr_2 = new Double[1];
        //Array lo_addr_3 = new Double[1];

        //Array dlo_phase_intial = new Double[1];
        //Array discard_sub1 = new Double[2];
        //Array discard_sub2 = new Double[2];
        //discard_sub1.SetValue(100, 0);
        //discard_sub1.SetValue(200, 1);

        //discard_sub2.SetValue(7788, 0);
        //discard_sub2.SetValue(9999, 1);

        //lo_addr.SetValue(222, 0);
        //lo_addr_2.SetValue(333, 0);
        //lo_addr_3.SetValue(4444, 0);

        //dlo_phase_intial.SetValue(6666, 0);
        //Array addr = new Double[1];
        //Array addr_2 = new Double[1];
        //Array addr_3 = new Double[1];

        //Array addr_var = new Double[2];

        //matLabApp.PutFullMatrix("LO_ADDR", "base", lo_addr, addr); // 模数本振同步计数值
        //matLabApp.PutFullMatrix("LO_ADDR_2", "base", lo_addr_2, addr_2); // 模数本振同步计数值
        //matLabApp.PutFullMatrix("LO_ADDR_3", "base", lo_addr_3, addr_3); // 模数本振同步计数值

        //matLabApp.PutFullMatrix("sub1_discard_num", "base", discard_sub1, addr_var); // 第一子带同步丢点值
        //matLabApp.PutFullMatrix("sub2_discard_num", "base", discard_sub2, addr_var); // 第二子带同步丢点值
        //matLabApp.PutFullMatrix("dlo_phase_intial", "base", dlo_phase_intial, addr); // 第二子带同步丢点值
        //int length = 1000;
        //Array buffer = new Double[length];
        //short[] srcBuffer=new short[100];
        //for (int index = 0; index < length; index++)
        //{
        //    buffer.SetValue(srcBuffer[index], index);
        //}
        //String name = $"source{0 + 1}";
        //Array pi = new Double[length];
        //matLabApp.PutFullMatrix(name, "base", buffer, pi);


        //string readBack = InstrumentInteract.Factory_ReadbackFPGAWritedRegisterValue(currInstrumentSession); //测试
        Array lo_addr_1 = new Double[1];
        Array lo_addr_2 = new Double[1];
        Array lo_addr_3 = new Double[1];
        Array lo_addr_4 = new Double[1];
        lo_addr_1.SetValue(0,0);
        lo_addr_2.SetValue(0, 0);
        lo_addr_3.SetValue(0, 0);
        lo_addr_4.SetValue(0, 0);

        List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(currInstrumentSession,6_000);
        if (allChannelData == null)
        {
            return;
        }
        Array source1 = new double[allChannelData[0].Length];
        Array source2 = new double[allChannelData[0].Length];
        Array source3 = new double[allChannelData[0].Length];
        Array source4 = new double[allChannelData[0].Length];

        Array imaginary = new double[allChannelData[0].Length];
        Array lo_imaginary = new Double[1];
        lo_imaginary.SetValue(0, 0);
        for (int i = 0; i < allChannelData[0].Length; i++)
        {
            source1.SetValue((double)allChannelData[0][i], i);
            source2.SetValue((double)allChannelData[1][i], i);
            source3.SetValue((double)allChannelData[2][i], i);
            source4.SetValue((double)allChannelData[3][i], i);

            imaginary.SetValue(0, i);
        }
        //double lo_addr_tmp2 = HdIO.ReadReg(ProcBdReg.R.Acq_Pro_Main_State);

        lo_addr_1.SetValue(0, 0);
        lo_addr_2.SetValue(0, 0);
        lo_addr_3.SetValue(0, 0);
        lo_addr_4.SetValue(0, 0);
        matLabApp?.PutFullMatrix("ch1_data", "base", source1, imaginary);
        matLabApp?.PutFullMatrix("ch2_data", "base", source2, imaginary);
        matLabApp?.PutFullMatrix("ch3_data", "base", source3, imaginary);
        matLabApp?.PutFullMatrix("ch4_data", "base", source4, imaginary);

        matLabApp?.PutFullMatrix("lo_addr_1", "base", lo_addr_1, lo_imaginary);
        matLabApp?.PutFullMatrix("lo_addr_2", "base", lo_addr_2, lo_imaginary);
        matLabApp?.PutFullMatrix("lo_addr_3", "base", lo_addr_3, lo_imaginary);
        matLabApp?.PutFullMatrix("lo_addr_4", "base", lo_addr_4, lo_imaginary);
    }
}
