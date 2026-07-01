using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;

namespace ScopeX.Core
{
    public class SpectrumAnalyzer : IJitterAnalyzer
    {
        public Double PJ;
        public Double RJ;

        public Double[] SpectrumSeparation(JitterParameter jitterData,JitterResult result)
        {
            Double[] tie_spectrum = GetTIESpectrum(jitterData.TIEDataAfterInterp.ToList(),jitterData.TIEDataWithoutDDJAfterInterp.ToList(), jitterData.ThresholdFreq / 100, out Double pjFreq, out Double rjFreq);
            tie_spectrum = tie_spectrum.SelectorArray(t => 20 * Math.Log10(t));
            PJ = pjFreq;
            RJ = rjFreq;
            result.PJ = pjFreq;
            result.RJ = rjFreq;
            return tie_spectrum;
        }

        public ScopeX.MathExt.Vector GetSpectrum(JitterParameter jitterData,JitterPrepare prepareData,JitterResult result)
        {
            Double[] ties_pectrum = SpectrumSeparation(jitterData, result);

            return new ScopeX.MathExt.Vector(ties_pectrum.Select(o => o / jitterData.Fs * Constants.S_RELATIVE_TO_PS).ToMatrix(1, ties_pectrum.Length),
                QuantityUnitExt.ToUnitString(QuantityUnit.Hertz),
                /*"ps"*/QuantityUnitExt.ToUnitString(QuantityUnit.Second),
                (jitterData.Fs / 2 / prepareData.AverageUILength) / ties_pectrum.Length,
                /*Constants.DEF_XPOS_IDX*/0);
        }
        public DecompositionParameter GetResult()
        {
            return new DecompositionParameter()
            {
                PJ = PJ,
                RJ = RJ,
                DJ = Double.NaN,
                TJ = Double.NaN,
            };
        }
        /// <summary>
        /// TIE频域分析处理
        /// </summary>
        /// <param name="tieWithoutDDJ">滤除DDJ之后的TIE序列</param>
        /// <param name="pj">周期性抖动</param>
        /// <param name="rj">随机抖动</param>
        /// <returns>滤除DDJ之后的TIE序列的频谱幅度值</returns>
        public static Double[] GetTIESpectrum(List<Double> tieWithDDJ, List<Double> tieWithoutDDJ, Double threshold, out Double pj, out Double rj)
        {
            Int32 fft_length = 1;
            while (fft_length <= tieWithoutDDJ.Count || fft_length <= 256)   //暂定最大64k，fft长度过长可能影响速度？
            {
                fft_length *= 2;
            }
			// 此处可替换为这个标准FFT函数，计算结果为双边FFT幅度值前一半的两倍
            //Double[] TIESingleSidedAmp = TIEWithoutDDJ.SingleSidedFFT(0.01, WindowType.Rectangle, FFTResultOpt.Ampltd, FFTLength, FFTCoordUnit.Vrms).ToArray();
            //if (TIEWithoutDDJ.Count < 1024)
            //{
            //    PJ = 0;
            //    RJ = 0;
            //    return new Double[1];
            //}
            //FFTLength /= 2;
            tieWithDDJ.AddRange(new Double[fft_length - tieWithDDJ.Count]);
            tieWithoutDDJ.AddRange(new Double[fft_length - tieWithoutDDJ.Count]);
  		
            //包含DDJ的抖动频谱用于频谱显示，不用于PJ、RJ计算；下面的FFTAmp为不含DDJ的频谱，用于PJ、RJ的计算
            Double[] tie_withd_dj_single_sided_amp = tieWithDDJ.SingleSidedFFT(0.01, WindowType.Rectangle, FFTResultOpt.Ampltd, fft_length, FFTCoordUnit.Vrms).ToArray();
            
            Complex[] tie_double_sided_result = tieWithoutDDJ.Take(fft_length).Select(o => new Complex(o, 0)).FFT().ToArray();
            var tie_double_sided = tie_double_sided_result.ToDoubleArray();


            Complex[] pj_double_sided_result = new Complex[fft_length];
            Complex[] rj_double_sided_result = new Complex[fft_length];
            var fft_amp_calc = tie_double_sided.Amp.GetMaxMinAvg();
            
            Double FFTAmpMax = fft_amp_calc.Max;
            Double FFTAmpMin = fft_amp_calc.Min;
            for (Int32 i = 0; i < fft_length; i++)
            {
                if (tie_double_sided.Amp[i] > (FFTAmpMax + FFTAmpMin) * threshold)//以单边谱为阈值，其幅度为双边谱的两倍
                {
                    pj_double_sided_result[i] = tie_double_sided_result[i];
                }
                else
                {
                    rj_double_sided_result[i] = tie_double_sided_result[i];
                }
            }

            //对区分好的频谱作IFFT
            Double[] tie_only_pj = pj_double_sided_result.IFFT().Real().Select(o => o / 2).ToArray();  //IFFT结果为理论值的两倍，除以2
            Double[] tie_only_rj = rj_double_sided_result.IFFT().Real().Select(o => o / 2).ToArray();


            //PJ是峰峰值,RJ是均方根值
            var pj_calc = tie_only_pj.GetMaxMinAvg();
            pj = pj_calc.Max - pj_calc.Min;

            Double rj_ave = tie_only_rj.Average();
            rj = 0;
            for (Int32 i = 0; i < fft_length; i++)
            {
                rj += Math.Pow(tie_only_rj[i], 2);
            }
            rj = Math.Sqrt(rj / fft_length);

            return tie_withd_dj_single_sided_amp;
        }
        public void Dispose()
        {
        }
    }
}
