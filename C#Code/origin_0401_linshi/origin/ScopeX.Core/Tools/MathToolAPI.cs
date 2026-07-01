using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Tools
{
    public class MathToolAPI
    {

        /// <summary>
        /// 将C/C++ dll获取到的数组指针转换为数组
        /// </summary>
        /// <param name="ptr">指针对象</param>
        /// <param name="length">数组长度</param>
        /// <returns></returns>
        internal static Double[] ConvertIntPtrToDoubleArray(IntPtr ptr, Int32 length)
        {
            Double[] result = new Double[length];
            Marshal.Copy(ptr, result, 0, length);
            var rst = ReleaseDoubleArray(ptr);
            return result;
        }

        /// <summary>
        /// 将C/C++ dll获取到的数组指针转换为数组
        /// </summary>
        /// <param name="ptr">指针对象</param>
        /// <param name="length">数组长度</param>
        /// <returns></returns>
        internal static Complex[] ConvertIntPtrToComplexDoubleArray(IntPtr ptr, Int32 length)
        {
            Complex[] result = new Complex[length];
            length = 2 * length;
            Double[] data = new Double[length];
            Marshal.Copy(ptr, data, 0, length);
            for (Int32 i = 0; i < length/2; i++)
            {
                // Real part is i*2, imaginary part is i*2+1
                result[i] = new Complex(data[2 * i], data[2 * i + 1]);
            }
            var rst = ReleaseComplexDoubleArray(ptr);
            return result;
        }


        /// <summary>
        /// 滤波器系数处理转换信号
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="num"></param>
        /// <param name="den"></param>
        /// <param name="samplelength"></param>
        /// <param name="filterlength"></param>
        /// <returns></returns>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Filter(Double[] sample, Double[] num, Double[] den, Int32 samplelength, Int32 filterlength);


        /// <summary>
        /// Fir滤波器系数处理转换信号
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="num"></param>
        /// <param name="samplelength"></param>
        /// <param name="filterlength"></param>
        /// <returns></returns>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr FirFilterToSignal(Double[] sample, Double[] num, Int32 samplelength, Int32 filterlength);

        /// <summary>
        /// Iir滤波器系数处理转换信号
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="num"></param>
        /// <param name="den"></param>
        /// <param name="samplelength"></param>
        /// <param name="filterlength"></param>
        /// <returns></returns>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr IirFilterToSignal(Double[] sample, Double[] num, Double[] den, Int32 samplelength, Int32 filterlength);

        /// <summary>
        /// 基于窗函数的FIR滤波器接口
        /// </summary>
        /// <param name="order">阶数>0</param>
        /// <param name="Filtertype">滤波器类型（low 1,high 2,bandpass 3,bandstop 4）</param>
        /// <param name="windowtype">窗口函数类型(Rectangle 1,Hann 2,Hamming 3,Blackman 4,Flattop 5,Kaiser 6,Gaussian 7)</param>
        /// <param name="wc1">(0,1)</param>
        /// <param name="wc2">(0,1)</param>
        /// <returns>系数结果数组</returns>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateFirByWindow(Int32 order, Int32 filterType, Int32 windowType, Double wc1, Double wc2);

        /// <summary>
        /// 基于FS的FIR滤波器接口
        /// </summary>
        /// <param name="order">阶数>0</param>
        /// <param name="Filtertype">滤波器类型（low 1,high 2,bandpass 3,bandstop 4）</param>
        /// <param name="windowtype">窗口函数类型(Rectangle 1,Hann 2,Hamming 3,Blackman 4,Flattop 5,Kaiser 6,Gaussian 7)</param>
        /// <param name="lowpass">(0,1)</param>
        /// <param name="lowstop">(0,1)</param>
        /// <param name="highpass">(0,1)</param>
        /// <param name="highstop">(0,1)</param>
        /// <returns>系数结果数组指针</returns>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateFirByFS(Int32 order, Int32 Filtertype, Int32 windowtype, Double pass, Double stop);


        /// <summary>
        /// IIR巴特沃斯滤波器
        /// </summary>
        /// <param name="n">阶数</param>
        /// <param name="Wn">截止频率</param>
        /// <param name="type">滤波器类型（low 1,high 2,bandpass 3,bandstop 4）</param>
        /// <param name="analog">是否模拟滤波器（0or1）</param>
        /// <param name="ab">结果分母</param>
        /// <param name="bb">结果分子</param>
        /// <returns></returns>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Butterworth(Int32 n, Double[] Wn, Int32 type, Int32 analog, Double[] ab, Double[] bb);

        /// <summary>
        /// IIR切比雪夫Ⅰ型滤波器
        /// </summary>
        /// <param name="n">阶数</param>
        /// <param name="r">通带波纹</param>
        /// <param name="Wn">截止频率</param>
        /// <param name="type">滤波器类型（low 1,high 2,bandpass 3,bandstop 4）</param>
        /// <param name="analog">是否模拟滤波器（0or1）</param>
        /// <param name="ab">结果分母</param>
        /// <param name="bb">结果分子</param>
        /// <returns></returns>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Chebyshv1(Int32 n, Double r, Double[] Wn, Int32 type, Int32 analog, Double[] ab, Double[] bb);

        /// <summary>
        /// IIR切比雪夫Ⅱ型滤波器
        /// </summary>
        /// <param name="n">阶数</param>
        /// <param name="r">通带波纹</param>
        /// <param name="Wn">截止频率</param>
        /// <param name="type">滤波器类型（low 1,high 2,bandpass 3,bandstop 4）</param>
        /// <param name="analog">是否模拟滤波器（0or1）</param>
        /// <param name="ab">结果分母</param>
        /// <param name="bb">结果分子</param>
        /// <returns></returns>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Chebyshv2(Int32 n, Double r, Double[] Wn, Int32 type, Int32 analog, Double[] ab, Double[] bb);

        /// <summary>
        /// IIR贝塞尔滤波器
        /// </summary>
        /// <param name="n">阶数</param>
        /// <param name="Wn">截止频率</param>
        /// <param name="type">滤波器类型（low 1,high 2,bandpass 3,bandstop 4）</param>
        /// <param name="ab">结果分母</param>
        /// <param name="bb">结果分子</param>
        /// <returns></returns>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Bessel(Int32 n, Double[] Wn, Int32 type, Double[] ab, Double[] bb);

        /// <summary>
        /// IIR切比雪夫Ⅱ型滤波器
        /// </summary>
        /// <param name="n">阶数</param>
        /// <param name="rp">通带波纹</param>
        /// <param name="rs">阻带波纹</param>
        /// <param name="Wn">截止频率</param>
        /// <param name="type">滤波器类型（low 1,high 2,bandpass 3,bandstop 4）</param>
        /// <param name="analog">是否模拟滤波器（0or1）</param>
        /// <param name="ab">结果分母</param>
        /// <param name="bb">结果分子</param>
        /// <returns></returns>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Elliptic(
            Int32 n,
            Double rp, 
            Double rs,
            [MarshalAs(UnmanagedType.LPArray)] Double[] Wn, 
            Int32 type,
            Int32 analog,
            [MarshalAs(UnmanagedType.LPArray)] Double[] ab,
            [MarshalAs(UnmanagedType.LPArray)] Double[] bb);

        /// <summary>
        /// FIR雷米兹滤波器
        /// </summary>
        /// <param name="order">阶数</param>
        /// <param name="filtertype">滤波器类型</param>
        /// <param name="lowpass">低导通频率（0，1）</param>
        /// <param name="lowstop">低截止频率（0，1）</param>
        /// <param name="highpass">高导通频率（0，1）</param>
        /// <param name="highstop">高截止频率（0，1）</param>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateFirByRemez(Int32 order, Int32 filtertype, Double pass, Double stop);

        /// <summary>
        /// IIR最小阶滤波器
        /// </summary>
        /// <param name="type">滤波器类型（巴特沃斯0，切比雪夫Ⅰ1，切比雪夫Ⅱ2，椭圆3，贝塞尔暂不支持）</param>
        /// <param name="w">频率数组</param>
        /// <param name="nw">频率数组长度</param>
        /// <param name="rs">截止波纹</param>
        /// <param name="order">阶数结果</param>
        /// <param name="wn">滤波器归一化频率结果</param>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void FilterMinimumOrder(Int32 type, Double[] w, Int32 nw, Double rp, Double rs,out Int32 order, Double[] wn);

        /// <summary>
        /// FIR雷米兹滤波器(最小阶)
        /// </summary>
        /// <param name="filtertype">滤波器类型</param>
        /// <param name="lowpass">低导通频率（0，1）</param>
        /// <param name="lowstop">低截止频率（0，1）</param>
        /// <param name="highpass">高导通频率（0，1）</param>
        /// <param name="highstop">高截止频率（0，1）</param>
        /// <param name="dev">最大允许偏差或波纹数组，
        /// 低通和高通长度为2（一个是响应频带，另一个是阻止频带)，
        /// 带通长度3（两个阻止频带和一个响应频带)，
        /// 带阻长度3（两个响应频带一个阻止频带）</param>
        /// <param name="ndev">长度</param>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateFirByRemezord(Int32 filtertype, Double lowpass, Double lowstop, Double highpass, Double highstop, Double[] dev,Int32 ndev,out Int32 order);


        /// <summary>
        /// 求特征值
        /// </summary>
        /// <param name="num">多项式系数</param>
        /// <param name="n">多项式阶数</param>
        /// <returns></returns>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Roots(Double[] num, Int32 n);

        /// <summary>
        /// FFT(自动补零)
        /// </summary>
        /// <param name="ptr">指针</param>
        /// <returns></returns>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void FFText(Double[] arr, Int32 length,Int32 n, Double[] real,Double[] imag);

        /// <summary>
        /// 释放doule数组资源
        /// </summary>
        /// <param name="ptr">指针</param>
        /// <returns></returns>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Int32 ReleaseDoubleArray(IntPtr ptr);

        /// <summary>
        /// 释放doule数组资源
        /// </summary>
        /// <param name="ptr">指针</param>
        /// <returns></returns>
        [DllImport("ScopeMath.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Int32 ReleaseComplexDoubleArray(IntPtr ptr);
    }
}
