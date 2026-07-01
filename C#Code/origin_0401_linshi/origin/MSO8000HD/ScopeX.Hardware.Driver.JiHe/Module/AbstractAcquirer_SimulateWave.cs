using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 模拟模拟通道波形数据
    /// </summary>
    internal class AbstractAcquirer_SimulateWave : AbstractAcquirer
    {
        internal enum SimulateWaveType
        {
            Sine,
            Square
        }
        internal static void SimulateAcqDataGenerator(ref ushort[] dataList, Int64 startIndex, Int32 adcBits, SimulateWaveType waveType, Int32 dataLength, Int64 samplingRate, Int32 frequencyByHz, Int32 amplitude, Int32 offset, Int32 phaseByHz)
        {
            Int32 adcHalfMax = (Int32)(Math.Pow(2, adcBits) / 2);
            if (waveType == SimulateWaveType.Sine)
            {
                double per = frequencyByHz * 1.0 / samplingRate;
                double initPhase = phaseByHz * 1.0 / frequencyByHz;
                double random = new Random().NextDouble();
                for (int i = 0; i < dataLength; i++)
                {
                    dataList[i + startIndex] = (ushort)(Math.Abs(amplitude * Math.Sin(i * 2 * Math.PI * per + initPhase) / 2 + offset * random + adcHalfMax));
                }
            }
            else if (waveType == SimulateWaveType.Sine)
            {
                int width = (int)(samplingRate / frequencyByHz / 2);
                int phase_dot = width * (phaseByHz % frequencyByHz) / frequencyByHz;
                for (int i = 0; i < dataLength; i++)
                {
                    if (((i + phase_dot) % width) == 0)
                        dataList[i + startIndex] = (ushort)(amplitude / 2 + offset + adcHalfMax);
                    else
                        dataList[i + startIndex] = (ushort)(-amplitude / 2 + offset + adcHalfMax);
                }
            }
        }
    }
}
