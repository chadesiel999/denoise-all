using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool
{
    public class MatlabSourceCodePrePosProcessor_Test:IMatlabSourceCodePrePosProcessor
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


            Array source1 = new double[1000];
            Array source2 = new double[1000];
            Array imaginary = new double[1000];
            double random = new Random().NextDouble();

            for (int i = 0; i < 1000; i++)
            {
                source1.SetValue(random * i, i);
                source2.SetValue(i, i);
                imaginary.SetValue(0, i);
            }
            matLabApp?.PutFullMatrix("input_data", "base", source1, imaginary);
            //matLabApp.PutFullMatrix("source2", "base", source2, imaginary);
        }
    }
}
