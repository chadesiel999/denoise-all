using ScopeX.ComModel;
using ScopeX.MathExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ScopeX.Core
{

    public static class VSA
    {
        public class PhaseDiscParam
        {
            public Complex CarrierSynSign;

            public double PhaseErr;
        }

        public enum ModFormat
        {
            QPSK,
            MQAM
        }

        public static Complex[] CarrierSyncDD(Complex[] data, double c1, double c2, double k0, int M, VsaFormatOpt formatOpt)
        {
            int num = data.Length;
            double[] array = new double[num];
            array[0] = 0.0;
            double[] array2 = new double[num];
            Complex[] array3 = new Complex[num];
            double[] array4 = new double[num];
            Complex[] array5 = new Complex[num];
            for (int i = 0; i < num - 1; i++)
            {
                array5[i] = Complex.Multiply(right: new Complex(Math.Cos(0.0 - array[i]), Math.Sin(0.0 - array[i])), left: data[i]);
                PhaseDiscParam phaseDiscParam = new PhaseDiscParam();
                phaseDiscParam = DDPhaseDiscQam(array5[i], M, formatOpt);
                array3[i] = phaseDiscParam.CarrierSynSign;
                array4[i] = phaseDiscParam.PhaseErr;
                array2[i + 1] = LoopFilter(array2[i], array4, i, c1, c2);
                array[i + 1] = array[i] + k0 * array2[i];
            }

            return array5;
        }

        public static Complex[] GetStandardConstellation(VsaFormatOpt formatOpt, int M)
        {
            Complex[] array = new Complex[M];
            switch (formatOpt)
            {
                case VsaFormatOpt.BPSK:
                    array[0] = new Complex(1.0, 0.0);
                    array[1] = new Complex(-1.0, 0.0);
                    return array;
                case VsaFormatOpt.QPSK:
                    array[0] = new Complex(1.0, 0.0);
                    array[1] = new Complex(0.0, 1.0);
                    array[2] = new Complex(-1.0, 0.0);
                    array[3] = new Complex(0.0, -1.0);
                    return array;
                case VsaFormatOpt.PSK8:
                    array[0] = new Complex(1.0, 0.0);
                    array[1] = new Complex(0.7071, 0.7071);
                    array[2] = new Complex(0.0, 1.0);
                    array[3] = new Complex(-0.7071, 0.7071);
                    array[4] = new Complex(-1.0, 0.0);
                    array[5] = new Complex(-0.7071, -0.7071);
                    array[6] = new Complex(0.0, -1.0);
                    array[7] = new Complex(0.7071, -0.7071);
                    return array;
                case VsaFormatOpt.QAM32:
                    {
                        int[] array3 = new int[6] { -5, -3, -1, 1, 3, 5 };
                        int num3 = 0;
                        for (int l = 0; l < 6; l++)
                        {
                            for (int m = 0; m < 6; m++)
                            {
                                int value = array3[l] * array3[m];
                                if (Math.Abs(value) != 25)
                                {
                                    array[num3] = new Complex(array3[l], array3[m]);
                                    num3++;
                                }
                            }
                        }

                        return array;
                    }
                case VsaFormatOpt.QAM128:
                    {
                        int[] array4 = new int[12]
                        {
                -11, -9, -7, -5, -3, -1, 1, 3, 5, 7,
                9, 11
                        };
                        int num4 = 0;
                        for (int n = 0; n < 12; n++)
                        {
                            for (int num5 = 0; num5 < 12; num5++)
                            {
                                int value2 = array4[n] * array4[num5];
                                if (Math.Abs(value2) != 121 && Math.Abs(value2) != 81 && Math.Abs(value2) != 99)
                                {
                                    array[num4] = new Complex(array4[n], array4[num5]);
                                    num4++;
                                }
                            }
                        }

                        return array;
                    }
                default:
                    {
                        double num = Math.Sqrt(M);
                        int[] array2 = new int[(int)num];
                        for (int i = 0; (double)i < num; i++)
                        {
                            array2[i] = 2 * (i - (int)num / 2) + 1;
                        }

                        int num2 = 0;
                        for (int j = 0; (double)j < num; j++)
                        {
                            for (int k = 0; (double)k < num; k++)
                            {
                                array[num2] = new Complex(array2[j], array2[k]);
                                num2++;
                            }
                        }

                        return array;
                    }
            }
        }

        public static PhaseDiscParam DDPhaseDiscQam(Complex dataIn, int M, VsaFormatOpt formatOpt)
        {
            PhaseDiscParam phaseDiscParam = new PhaseDiscParam();
            if (dataIn == 0)
            {
                return phaseDiscParam;
            }

            Complex[] array = new Complex[M];
            array = GetStandardConstellation(formatOpt, M);
            double[] array2 = new double[M];
            for (int i = 0; i < M; i++)
            {
                array2[i] = Complex.Abs(dataIn - array[i]);
            }

            double num = array2.Min();
            for (int j = 0; j < M; j++)
            {
                if (array2[j] == num)
                {
                    phaseDiscParam.CarrierSynSign = array[j];
                    break;
                }
            }

            phaseDiscParam.PhaseErr = Complex.Abs(dataIn) * (dataIn / phaseDiscParam.CarrierSynSign).Imaginary / Math.Sqrt(2.0);
            return phaseDiscParam;
        }

        public static double LoopFilter(double filterLastOut, double[] DataIn, int dataInTime, double c1, double c2)
        {
            if (dataInTime == 0)
            {
                return filterLastOut + c1 * DataIn[dataInTime] + c2 * DataIn[dataInTime];
            }

            return filterLastOut + c1 * (DataIn[dataInTime] - DataIn[dataInTime - 1]) + c2 * DataIn[dataInTime];
        }

        public static double StateSwitchControl(Complex theoryPosion, Complex actualPosion, double crisis, double state_cnt)
        {
            double num = 0.0;
            double magnitude = (actualPosion - theoryPosion).Magnitude;
            state_cnt = ((magnitude < crisis) ? state_cnt : (state_cnt + 1.0));
            if (magnitude < crisis)
            {
                num = state_cnt + 1.0;
            }

            num = state_cnt;
            return state_cnt;
        }

        public static bool MixFreqUp(IEnumerable<double> IData, IEnumerable<double> QData, double sampleRate, double freqCarrier, double freqErrCarrier, double phaseErrCarrier, out IEnumerable<double> modSignal)
        {
            if (IData.Count() != QData.Count() || sampleRate.Equals(0.0) || IData == null || QData == null)
            {
                modSignal = new double[0];
                return false;
            }

            IEnumerable<Complex> first = IData.Zip(QData, (double i, double q) => new Complex(i, q));
            IEnumerable<double> second = from o in Enumerable.Range(0, IData.Count())
                                         select (double)o / sampleRate;
            modSignal = first.Zip(second, (Complex s, double t) => Complex.Multiply(s, Complex.Exp(Complex.ImaginaryOne * 2.0 * Math.PI * (freqCarrier + freqErrCarrier) * t)).Real);
            return true;
        }

        public static bool MixFreqDown(IEnumerable<double> modSignal, double sampleRate, double freqCarrier, out IEnumerable<Complex> dmodSignal)
        {
            if (sampleRate == 0.0 || modSignal == null)
            {
                dmodSignal = new Complex[0];
                return false;
            }

            IEnumerable<double> second = from o in Enumerable.Range(0, modSignal.Count())
                                         select (double)o / sampleRate;
            IEnumerable<double> first = modSignal.Zip(second, (double s, double t) => s * Math.Cos(Math.PI * 2.0 * freqCarrier * t));
            IEnumerable<double> second2 = modSignal.Zip(second, (double s, double t) => s * Math.Cos(Math.PI * 2.0 * freqCarrier * t + Math.PI / 2.0));
            dmodSignal = first.Zip(second2, (double i, double q) => new Complex(i, q));
            return true;
        }

        public static double[,]? GetEyeDiagramVSA(IEnumerable<double> dataIn, int symbolNumPlay, int sps, uint width, uint high)
        {
            int num = dataIn.Count();
            int pointsPerSymbol = symbolNumPlay * sps;
            IEnumerable<double> source = Enumerable.Range(0, num).Select((Func<int, double>)((int o) => o % pointsPerSymbol));
            double eyeTimeRegMax = pointsPerSymbol;
            double dataInMax = dataIn.Max();
            if (pointsPerSymbol >= num || pointsPerSymbol == 0 || dataIn == null || dataInMax == 0.0 || eyeTimeRegMax == 0.0)
            {
                return null;
            }

            IEnumerable<double> source2 = source.Select((double o) => o / eyeTimeRegMax * (double)width);
            double eyeTimeAverage = source2.Average();
            IEnumerable<double> source3 = source2.Select((double o) => o - eyeTimeAverage);
            IEnumerable<double> source4 = dataIn.Select((double o) => o / dataInMax * (double)high);
            double[,] array = new double[2, num];
            Buffer.BlockCopy(source3.ToArray(), 0, array, 0, num * 8);
            Buffer.BlockCopy(source4.ToArray(), 0, array, num * 8, num * 8);
            return array;
        }

        public static bool hPrefilter(double beta, int sps, int span, out double[] h)
        {
            int num = sps * span;
            int num2 = num / 2;
            double[] array = new double[num + 1];
            double[] array2 = new double[num + 1];
            double[] array3 = new double[num + 1];
            double[] array4 = new double[num + 1];
            double[] array5 = new double[num + 1];
            double[] array6 = new double[num + 1];
            double[] array7 = new double[num + 1];
            if (beta == 0.0 || sps == 0 || span == 0)
            {
                h = null;
                return false;
            }

            for (int i = 0; i < num + 1; i++)
            {
                array[i] = (double)(i - num2) * 1.0 / (double)sps;
                array5[i] = Math.Cos(Math.PI * 2.0 * array[i]);
            }

            for (int j = 0; j < num + 1; j++)
            {
                array2[j] = 1.0 - Math.Pow(2.0 * beta * array[j], 2.0);
                if (Math.Abs(array2[j]) > Math.Sqrt(2.2204E-16))
                {
                    double num3 = ((array[j] == 0.0) ? 1.0 : (Math.Sin(Math.PI * array[j]) / (Math.PI * array[j])));
                    array3[j] = num3 * Math.Cos(Math.PI * beta * array[j]) / array2[j];
                }
                else
                {
                    array3[j] = beta * Math.Sin(Math.PI / (2.0 * beta)) / 2.0;
                }
            }

            for (int k = 0; k < num + 1; k++)
            {
                array4[k] = array3[k] * array5[k];
            }

            if ((num + 1) / 2 != 0)
            {
                for (int l = 0; l < num + 1; l++)
                {
                    if (l < (num + 2) / 2)
                    {
                        array6[l] = 0.42 - 0.5 * Math.Cos(Math.PI * 2.0 * (double)l / (double)num) + 0.08 * Math.Cos(Math.PI * 4.0 * (double)l / (double)num);
                    }
                    else
                    {
                        array6[l] = array6[num - l];
                    }
                }
            }
            else
            {
                for (int m = 0; m < num + 1; m++)
                {
                    if (m < (num + 1) / 2)
                    {
                        array6[m] = 0.42 - 0.5 * Math.Cos(Math.PI * 2.0 * (double)m / (double)num) + 0.08 * Math.Cos(Math.PI * 4.0 * (double)m / (double)num);
                        continue;
                    }

                    if (m == (num + 1) / 2)
                    {
                        array6[m] = array6[m - 1];
                    }

                    array6[m] = array6[num + 1 - m];
                }
            }

            for (int n = 0; n < num + 1; n++)
            {
                array7[n] = array4[n] * array6[n];
            }

            double num4 = 0.0;
            for (int num5 = 0; num5 < num + 1; num5++)
            {
                num4 = Math.Pow(array7[num5], 2.0) + num4;
            }

            h = new double[num + 1];
            for (int num6 = 0; num6 < num + 1; num6++)
            {
                h[num6] = array7[num6] / Math.Sqrt(num4);
            }

            return true;
        }

        public static bool LoopFilter(double bandWidth, double kesi, double sps, double beta, int M, VsaPllUseScne vsaPllUseScne, out double[]? coeff)
        {
            if (bandWidth > 0.1 || beta < 0.0 || beta > 1.0 || sps.Equals(0.0) || kesi.Equals(0.0) || kesi < 0.0 || kesi > 1.0)
            {
                coeff = null;
                return false;
            }

            coeff = new double[2];
            if (1 == 0)
            {
            }

            double num = vsaPllUseScne switch
            {
                VsaPllUseScne.MPSKsymbolsyn => 2.0 * Math.Sin(Math.PI * beta / 2.0) / (1.0 - Math.Pow(beta, 2.0) / 4.0),
                VsaPllUseScne.MQAMsymbolsyn => 2.0 * Math.Pow(Math.Log2(M), 2.0) * Math.Sin(Math.PI * beta / 2.0) / (1.0 - Math.Pow(beta, 2.0) / 4.0),
                VsaPllUseScne.carriersyn => 1.0,
                _ => 2.0 * Math.Sin(Math.PI * beta / 2.0) / (1.0 - Math.Pow(beta, 2.0) / 4.0),
            };
            if (1 == 0)
            {
            }

            double num2 = num;
            double num3 = bandWidth / sps / (kesi + 1.0 / (kesi * 4.0));
            double num4 = 4.0 * kesi * num3 / (1.0 + 2.0 * kesi * num3 + Math.Pow(num3, 2.0)) / num2;
            double num5 = 4.0 * Math.Pow(num3, 2.0) / (1.0 + 2.0 * kesi * num3 + Math.Pow(num3, 2.0)) / num2;
            coeff[0] = num4;
            coeff[1] = num5;
            return true;
        }

        public static bool ReSample(double[] dataIn, double fsIn, double fsOut, double[,] farrowCoeff, out double[]? signalItplOut)
        {
            if (fsIn == 0.0)
            {
                signalItplOut = null;
                return false;
            }

            int num = dataIn.Length;
            int num2 = farrowCoeff.GetLength(1) / 2;
            double num3 = fsOut / fsIn;
            int num4 = (int)Math.Floor((double)num * num3);
            double[] array = new double[num4];
            double[] array2 = new double[num4];
            double[] array3 = new double[num4];
            for (int i = 0; i < num4; i++)
            {
                array[i] = (double)i / num3;
                array2[i] = Math.Floor(array[i]);
                array3[i] = array[i] - array2[i];
            }

            double[,] signalItplIn = SignalItplIn(num4, num, array2, num2, dataIn, farrowCoeff.GetLength(1));
            double[] array4 = Itpl(num4, array3, farrowCoeff, signalItplIn);
            int num5 = (int)((double)array4.Length - (double)num2 * num3 + 1.0);
            int num6 = (int)((double)num2 * num3 - 1.0);
            signalItplOut = array4.Skip(num6).Take(num5 - num6).ToArray();
            return true;
        }

        private static double[,] SignalItplIn(int signalOutLength, int datainlength, double[] itplBasePosion, int filtDly, double[] dataIn, int filterOrder)
        {
            int[] array = new int[signalOutLength];
            int[] array2 = new int[signalOutLength];
            double[,] array3 = new double[signalOutLength, filterOrder];
            for (int i = 0; i < signalOutLength; i++)
            {
                array[i] = (int)(itplBasePosion[i] + 1.0 - (double)filtDly);
                array2[i] = (int)(itplBasePosion[i] + 1.0 + (double)filtDly - 1.0);
                if (array[i] < 0 || array2[i] >= datainlength)
                {
                    continue;
                }

                for (int j = 0; j < filterOrder; j++)
                {
                    if (array[i] + j < datainlength)
                    {
                        array3[i, j] = dataIn[array[i] + j];
                    }
                }
            }

            return array3;
        }

        private static double[] Itpl(int signalOutLength, double[] itplFracDelay, double[,] farrowCoeff, double[,] signalItplIn)
        {
            int num = farrowCoeff.GetLength(0) - 1;
            double[] array = new double[signalOutLength];
            double[] array2 = new double[farrowCoeff.GetLength(1)];
            for (int i = 0; i < signalOutLength; i++)
            {
                double num2 = itplFracDelay[i];
                if (num2.Equals(0.0))
                {
                    num2 += 1E-07;
                }

                for (int j = 0; j < farrowCoeff.GetLength(1); j++)
                {
                    array2[j] = farrowCoeff[num, j];
                }

                for (int k = 0; k < farrowCoeff.GetLength(1); k++)
                {
                    for (int l = 1; l < num + 1; l++)
                    {
                        array2[k] += Math.Pow(num2, l) * farrowCoeff[num - l, k];
                    }
                }

                for (int m = 0; m < farrowCoeff.GetLength(1) - 1; m++)
                {
                    array[i] += signalItplIn[i, m] * array2[m];
                }
            }

            return array;
        }

        public static IEnumerable<double> RootRaisedCosFilter(double beta, int span, int sps)
        {
            int num = span * sps + 1;
            if ((num % 2).Equals(0) || beta < 0.0 || beta > 1.0)
            {
                return new double[0];
            }

            double[] t = (from i in Enumerable.Range(0, num)
                          select (0.0 - (double)(span * sps / 2) + (double)i) / (double)sps).ToArray();
            IEnumerable<double> h = CalculFilterCoeff(beta, sps, t);
            return NormalEnergy(h);
        }

        public static IEnumerable<double> RaisedCosFilter(double beta, int span, int sps)
        {
            IEnumerable<double> enumerable = VSA.RootRaisedCosFilter(beta, span, sps);
            return enumerable.Convolve(enumerable);
        }

        public static IEnumerable<double> CalculFilterCoeff(double beta, int sps, double[] t)
        {
            double[] array = new double[t.Length];
            for (int i = 0; i < t.Length; i++)
            {
                if (Math.Abs(4.0 * beta * t[i]).Equals(1.0))
                {
                    array[i] = 1.0 / (Math.PI * 2.0 * (double)sps) * (Math.PI * (beta + 1.0) * Math.Sin(Math.PI * (beta + 1.0) / (4.0 * beta)) - 4.0 * beta * Math.Sin(Math.PI * (beta - 1.0) / (4.0 * beta)) + Math.PI * (beta - 1.0) * Math.Cos(Math.PI * (beta - 1.0) / (4.0 * beta)));
                }
                else if (t[i].Equals(0.0))
                {
                    array[i] = -1.0 / (Math.PI * (double)sps) * (Math.PI * (beta - 1.0) - 4.0 * beta);
                }
                else
                {
                    array[i] = -4.0 * beta / (double)sps * (Math.Cos((1.0 + beta) * Math.PI * t[i]) + Math.Sin((1.0 - beta) * Math.PI * t[i]) / (4.0 * beta * t[i])) / (Math.PI * (Math.Pow(4.0 * beta * t[i], 2.0) - 1.0));
                }
            }

            return array;
        }

        public static IEnumerable<double> NormalEnergy(IEnumerable<double> h)
        {
            double filterEnergy = Math.Sqrt(h.Select((double h) => Math.Pow(h, 2.0)).Sum());
            return filterEnergy.Equals(0.0) ? h : h.Select((double o) => o / filterEnergy);
        }

        public static IEnumerable<double> NormalAmpli(IEnumerable<double> h)
        {
            double hMax = h.Max();
            return hMax.Equals(0.0) ? h : h.Select((double o) => o / hMax);
        }

        public static int? SignalStableStartPoint(int rmsAccNum, IEnumerable<double> dataIn, int StableNum, double ErrPrecision)
        {
            if ((dataIn.Count() * rmsAccNum * StableNum).Equals(0))
            {
                return null;
            }

            int row = dataIn.Count() / rmsAccNum;
            double[,] datIn = dataIn.ToMatrix(row, rmsAccNum);
            double[] datIn2 = datIn.Rms();
            IEnumerable<double> source = Diff(datIn2);
            double[] datIn3 = source.Select((double o) => Math.Abs(o)).ToArray();
            List<int> list = datIn3.FindPrecisnRangeIdex(ErrPrecision, CompareLess, StableNum);
            if (StableNum > list.Count() - 1)
            {
                return null;
            }

            return list[StableNum] * rmsAccNum;
        }

        public static double[] Rms(this double[,] datIn)
        {
            int length = datIn.GetLength(0);
            int length2 = datIn.GetLength(1);
            double[] array = new double[length2];
            double[] array2 = new double[length];
            for (int i = 0; i < length; i++)
            {
                Buffer.BlockCopy(datIn, i * length2 * 8, array, 0, length2 * 8);
                array2[i] = Math.Sqrt(array.Sum((double o) => Math.Pow(o, 2.0)) / (double)length2);
            }

            return array2;
        }

        public static IEnumerable<double> Diff(IEnumerable<double> datIn)
        {
            if (datIn.Count() < 1)
            {
                return Enumerable.Empty<double>();
            }

            IEnumerable<double> first = datIn.Skip(1).Take(datIn.Count());
            IEnumerable<double> second = datIn.Skip(0).Take(datIn.Count() - 1);
            return first.Zip(second, (double o, double w) => o - w);
        }

        public static List<int> FindPrecisnRangeIdex(this double[] datIn, double dataPrecision, Func<double[], double, double[]> condFunc, int stableNum)
        {
            double[] dataIn = condFunc(datIn, dataPrecision);
            List<int> list = NoZeroIdex(dataIn);
            return (list.Count() < stableNum) ? list.Take(stableNum).ToList() : list;
        }

        public static List<int> NoZeroIdex(double[] dataIn)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < dataIn.Length; i++)
            {
                if (!dataIn[i].Equals(0.0))
                {
                    list.Add(i);
                }
            }

            return list;
        }

        public static double[] CompareLess(double[] dataIn, double compareValue)
        {
            IEnumerable<double> source = dataIn.Select((double o) => o - compareValue);
            return source.Select((double o) => (1.0 - (double)Math.Sign(o)) / 2.0).ToArray();
        }

        public static (Complex[] y, double[] Pte) SymSynPreFilter(int pSPS, double c1, double c2, Complex[,] data, Complex[,] dataNoPrefilter, ModFormat modFormat)
        {
            Complex[] item;
            double[] item2;
            if ((pSPS + 2) % 2 != 0)
            {
                item = null;
                item2 = null;
                return (y: item, Pte: item2);
            }

            double num = 0.5;
            double num2 = num;
            double[,] pInterpFilterCoeff = new double[3, 4]
            {
            { 0.0, 0.0, 1.0, 0.0 },
            {
                0.0 - num2,
                1.0 + num2,
                0.0 - (1.0 - num2),
                0.0 - num2
            },
            {
                num2,
                0.0 - num2,
                0.0 - num2,
                num2
            }
            };
            int[] array = new int[2] { 11, 10 };
            int length = data.GetLength(0);
            double num3 = Math.Round((double)(length * array[0]) / ((double)array[1] * (double)pSPS), 0);
            double pLoopFilterState = 0.0;
            double pLoopPreviousInput = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            double[,] array2 = new double[1, pSPS];
            double num6 = 0.0;
            double pNCOCounter = 0.0;
            double[,] array3 = new double[length, 1];
            Complex[,] pInterpFilterState = new Complex[3, 1];
            Complex[,] pInterpFilterState2 = new Complex[3, 1];
            Complex[,] array4 = new Complex[1, pSPS];
            Complex[,] array5 = new Complex[(int)num3, 1];
            int length2 = array5.GetLength(0);
            for (int i = 0; i < length; i++)
            {
                if (num5 == (double)length2 && num4 == 1.0)
                {
                    break;
                }

                num5 += num4;
                array3[i, 0] = num6;
                Complex complex;
                (complex, pInterpFilterState) = InterpFilter(data[i, 0], pInterpFilterState, pInterpFilterCoeff, num6);
                Complex complex2;
                (complex2, pInterpFilterState2) = InterpFilter(dataNoPrefilter[i, 0], pInterpFilterState2, pInterpFilterCoeff, num6);
                if (num4 == 1.0)
                {
                    array5[(int)num5 - 1, 0] = complex2;
                }

                if (num5 > (double)length2)
                {
                    break;
                }

                double timeError = GardnerTED(complex, num4, array2, array4);
                double[,] array6 = new double[1, pSPS];
                int j;
                for (j = 0; j < array2.GetLength(1) - 1; j++)
                {
                    array6[0, j] = array2[0, j + 1];
                }

                array6[0, j] = num4;
                double num7 = 0.0;
                for (j = 0; j < array6.GetLength(1); j++)
                {
                    num7 += array6[0, j];
                }

                double num8 = num7;
                double num9 = num8;
                if (num9 == 1.0)
                {
                    for (j = 0; j < array4.GetLength(1) - 1; j++)
                    {
                        array4[0, j] = array4[0, j + 1];
                    }

                    array4[0, j] = complex;
                }
                else
                {
                    for (j = 0; j < array4.GetLength(1) - 2; j++)
                    {
                        array4[0, j] = array4[0, j + 2];
                    }

                    array4[0, j] = 0;
                    array4[0, j + 1] = complex;
                }

                (double v, double pLoopFilterState, double pLoopPreviousInput) tuple3 = LoopFilter(timeError, pLoopPreviousInput, pLoopFilterState, c1, c2);
                double item3 = tuple3.v;
                pLoopFilterState = tuple3.pLoopFilterState;
                pLoopPreviousInput = tuple3.pLoopPreviousInput;
                (double pMu, double pNCOCounter, double[,] pStrobeHistory, double pStrobe) tuple4 = InterpControl(item3, pSPS, array2, num4, pNCOCounter, num6);
                num6 = tuple4.pMu;
                pNCOCounter = tuple4.pNCOCounter;
                array2 = tuple4.pStrobeHistory;
                num4 = tuple4.pStrobe;
            }

            item = new Complex[(int)num5];
            for (int k = 0; (double)k < num5; k++)
            {
                item[k] = array5[k, 0];
            }

            item2 = new double[array3.Length];
            for (int l = 0; l < array3.Length; l++)
            {
                item2[l] = array3[l, 0];
            }

            return (y: item, Pte: item2);
        }

        public static (double v, double pLoopFilterState, double pLoopPreviousInput) LoopFilter(double timeError, double pLoopPreviousInput, double pLoopFilterState, double c1, double c2)
        {
            double num = pLoopPreviousInput + pLoopFilterState;
            double item = timeError * c1 + num;
            pLoopFilterState = num;
            pLoopPreviousInput = timeError * c2;
            return (v: item, pLoopFilterState: pLoopFilterState, pLoopPreviousInput: pLoopPreviousInput);
        }

        public static (Complex y, Complex[,] pInterpFilterState) InterpFilter(Complex dataOne, Complex[,] pInterpFilterState, double[,] pInterpFilterCoeff, double pMu)
        {
            Complex[,] array = new Complex[4, 1]
            {
            { dataOne },
            { pInterpFilterState[0, 0] },
            { pInterpFilterState[1, 0] },
            { pInterpFilterState[2, 0] }
            };
            Complex[,] array2 = new Complex[3, 1];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    array2[i, 0] += pInterpFilterCoeff[i, j] * array[j, 0];
                }
            }

            Complex item = array2[0, 0] + pMu * array2[1, 0] + pMu * pMu * array2[2, 0];
            pInterpFilterState[0, 0] = array[0, 0];
            pInterpFilterState[1, 0] = array[1, 0];
            pInterpFilterState[2, 0] = array[2, 0];
            return (y: item, pInterpFilterState: pInterpFilterState);
        }

        public static (double pMu, double pNCOCounter, double[,] pStrobeHistory, double pStrobe) InterpControl(double v, int pSPS, double[,] pStrobeHistory, double pStrobe, double pNCOCounter, double pMu)
        {
            double num = v + 1.0 / (double)pSPS;
            double[,] array = new double[1, pSPS];
            int i;
            for (i = 0; i < pStrobeHistory.GetLength(1) - 1; i++)
            {
                array[0, i] = pStrobeHistory[0, i + 1];
            }

            array[0, i] = pStrobe;
            pStrobe = ((!(pNCOCounter < num)) ? 0.0 : 1.0);
            if (pStrobe == 1.0)
            {
                pMu = pNCOCounter / num;
            }

            pNCOCounter = (pNCOCounter - num + 1.0) % 1.0;
            return (pMu: pMu, pNCOCounter: pNCOCounter, pStrobeHistory: array, pStrobe: pStrobe);
        }

        public static double GardnerTED(Complex x, double pStrobe, double[,] pStrobeHistory, Complex[,] pTEDBuffer)
        {
            if (pStrobe != 1.0)
            {
                return 0.0;
            }

            double[] array = new double[pStrobeHistory.GetLength(1) - 1];
            for (int i = 0; i < pStrobeHistory.GetLength(1) - 1; i++)
            {
                array[i] = pStrobeHistory[0, i + 1];
            }

            if (!array.All((double element) => element == 0.0))
            {
                return 0.0;
            }

            Complex complex = pTEDBuffer[0, pTEDBuffer.GetLength(1) / 2];
            Complex complex2 = (pTEDBuffer[0, 0] + x) / 2.0;
            return (complex.Real - complex2.Real) * (pTEDBuffer[0, 0].Real - x.Real) + (complex.Imaginary - complex2.Imaginary) * (pTEDBuffer[0, 0].Imaginary - x.Imaginary);
        }
    }
}
