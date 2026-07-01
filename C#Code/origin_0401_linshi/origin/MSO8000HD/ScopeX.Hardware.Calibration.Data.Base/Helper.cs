using System;
using System.Runtime.InteropServices;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class Helper
    {
        #region Marshal
        internal static byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structObj, buffer, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        internal static Byte[] StructToBytes<T>(T source) where T : struct
        {
            Int32 size = Marshal.SizeOf<T>();

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(source, ptr, false);
            Byte[] result = new Byte[size];
            Marshal.Copy(ptr, result, 0, size);
            Marshal.FreeHGlobal(ptr);

            return result;
        }

        internal static T? BytesToStruct<T>(Byte[] source, Int32 startId) where T : struct
        {
            Int32 structlen = Marshal.SizeOf<T>();
            if (startId >= source.Length || startId + structlen > source.Length)
                return null;

            IntPtr buffer = Marshal.AllocHGlobal(structlen);
            Marshal.Copy(source, startId, buffer, structlen);
            T result = Marshal.PtrToStructure<T>(buffer);
            Marshal.FreeHGlobal(buffer);

            return result;
        }

        internal static Object? BytesToStruct(Byte[] source, Int32 startId, Type strcutType)
        {
            Int32 structsize = Marshal.SizeOf(strcutType);
            IntPtr buff = Marshal.AllocHGlobal(structsize);
            Marshal.Copy(source, startId, buff, structsize);
            Object? result = Marshal.PtrToStructure(buff, strcutType);
            Marshal.FreeHGlobal(buff);
            return result;
        }

        internal static T? BytesToStruct<T>(byte[] bytes, int startIndex, Type strcutType)
        {
            int size = Marshal.SizeOf(strcutType);
            T? data;
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, startIndex, buffer, size);
                data = (T?)Marshal.PtrToStructure(buffer, strcutType);
            }
            catch
            {
                data = (T?)Activator.CreateInstance(strcutType, new object[] { });
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
            return data;
        }
        #endregion

        public static ICaliData? GetICaliData(CaliDataType caliDataType)
        {
            return caliDataType switch
            {
                CaliDataType.AiAnalogParams => AiAnalogChannelParams.Default,
                CaliDataType.AutoCalibration => AutoCaliParams.Default,
                CaliDataType.PhyChannel => ChannelParams.Default,
                CaliDataType.PhyChannelModel2 => ChannelParamsModel2.Default,

                CaliDataType.DbiAnalogParams => DbiAnalogParams.Default,
                CaliDataType.DbiLocalOscillators => DbiLocalOscillators.Default,
                CaliDataType.DbiCoefficientsTables => DbiCoefficientsTables.Default,

                CaliDataType.TiAdc_SyncSampleClock => TiAdc_SyncSampleClock.Default,

                CaliDataType.CoefficientsTables => CoefficientsTables.Default,
                CaliDataType.Misc => MiscData.Default,

                CaliDataType.AWG => AWGCaliData.Default,

                CaliDataType.TiadcPhaseOffsetGainParams => TiadcPhaseOffsetGainParams.Default,
                CaliDataType.AnalogParams => AnalogChannelParams.Default,
                CaliDataType.CoefficientsParams => CoefficientsParams.Default,

                _ => null
            };
        }
    }
}
