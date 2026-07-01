//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ScopeX.Core.Model
//{
//    /// <summary>
//    /// Defines the Digital down conversion, input with  cos（wt+Btt）
//    /// </summary>
//    internal class DdcModel
//    {

//        private Double _CarierFrequency;
//        private Double f_rider;
//        private Double f_signalsampling;
//        private Double SamplingPoints;
//        public static (Double[] I, Double[] Q) DDC(Double _f_carrie, Double _f_rider, Double _f_signalsampling, Double _SamplingPoints, Double[] buffer)
//        {

//        }

//        public static Boolean DDC(Double _f_carrie, Double _f_rider, Double _f_signalsampling, Double _SamplingPoints=10000, Double[] buffer, out Double[] I, out Double[] Q)
//        {

//        }
//        public static Boolean DDC(Double _f_carrie, Double _f_rider, Double _f_signalsampling, Double[] buffer, out Double[] I, out Double[] Q)
//        {
//            DDC(_f_carrie, _f_rider, _f_signalsampling, 10000, buffer, out I, out Q);
//        }

//        public void FrequencyAndSamplingPoints(Double _f_carrie, Double _f_rider, Double _f_signalsampling, Double _SamplingPoints)
//        {
//            _CarierFrequency = _f_carrie;
//            f_rider = _f_rider;
//            f_signalsampling = _f_signalsampling;
//            SamplingPoints = _SamplingPoints;
//        }


//        public Double[] SignalSamplingT(Double _f_signalsampling, Double _SamplingPoints)
//        {

//            f_signalsampling = _f_signalsampling;
//            SamplingPoints = _SamplingPoints;
//            Double[] t = new Double[(Int32)SamplingPoints];

//            for (Int32 k = 0; k < SamplingPoints; k++)
//            {

//                t[k] = k * (1 / f_signalsampling);
//            }
//            return t;

//        }
//        public Double[] SignalSamplingF(Double _f_signalsampling, Double _SamplingPoints)
//        {
//            f_signalsampling = _f_signalsampling;
//            SamplingPoints = _SamplingPoints;
//            Double[] f = new Double[(Int32)SamplingPoints];

//            for (Int32 k = 0; k < SamplingPoints; k++)
//            {
//                f[k] = k * (f_signalsampling / SamplingPoints);
//            }
//            return f;

//        }

//        public Double[] SignalInput(Double[] array)  //cos（wt+Btt）
//        {
//            Double[] Sinput = new Double[(Int32)SamplingPoints];
//            for (Int32 k = 0; k < SamplingPoints; k++)
//            {
//                Sinput[k] = Math.Cos(2 * Math.PI * _CarierFrequency * array[k] + 2 * Math.PI * f_rider * array[k] * array[k]);
//            }
//            return Sinput;
//        }

//        public Double[] CosCarryInput(Double[] array)  //cos(wt)
//        {
//            Double[] CCinput = new Double[(Int32)SamplingPoints];
//            for (Int32 k = 0; k < SamplingPoints; k++)
//            {
//                CCinput[k] = Math.Cos(2 * Math.PI * _CarierFrequency * array[k]);
//            }
//            return CCinput;
//        }

//        public Double[] SinCarryInput(Double[] array)  //sin(wt)
//        {
//            Double[] SCinput = new Double[(Int32)SamplingPoints];
//            for (Int32 k = 0; k < SamplingPoints; k++)
//            {
//                SCinput[k] = Math.Sin(2 * Math.PI * _CarierFrequency * array[k]);
//            }
//            return SCinput;
//        }

//        /// <summary>
//        /// I xxxxxxxxxx
//        /// </summary>
//        /// <param name="array">ssss</param>
//        /// <param name="array1">zzzzzz</param>
//        /// <returns></returns>
//        public Double[] IchannelSignal(Double[] array, Double[] array1) //I
//        {
//            Double[] ISinput1 = new Double[(Int32)SamplingPoints];//
//            for (Int32 k = 0; k < SamplingPoints; k++)
//            {
//                ISinput1[k] = array[k] * array1[k];
//            }
//            return ISinput1;
//        }

//        public Double[] QchannelSignal(Double[] array, Double[] array1)  //Q
//        {
//            Double[] QSinput1 = new Double[(Int32)SamplingPoints];
//            for (Int32 k = 0; k < SamplingPoints; k++)
//            {
//                QSinput1[k] = array[k] * (-array1[k]);
//            }
//            return QSinput1;
//        }

//    }
//}

//Write a method can find the peak of a list of data ,Can you?