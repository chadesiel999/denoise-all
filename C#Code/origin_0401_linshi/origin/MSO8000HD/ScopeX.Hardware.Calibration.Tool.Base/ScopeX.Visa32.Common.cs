using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Visa32
{
    public class Common
    {
        public const string ServerRecvedMappedFileName = "ScopeXVisaRecvedMappedFile";
        public const string ServerSendMappedFileName = "ScopeXVisaSendMappedFile";
        public const string MutexName_ServerRecv = "recvedMappedFileMutex";
        public const string MutexName_ServerSend = "sendMappedFileMutex";
        public enum FuncTypes
        {
            viNone=0,
            viClose=1<<0,
            viOpenDefaultRM=1<<1,
            viOpen=1<<2,
            viPrintf=1<<3,
            viWrite=1<<4,
            viRead=1<<5,
            GetAllExistsResource=1<<6,
            CloseServer=1<<7,
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct viStructRequestHeader_viOpen
        {
            public Byte FuncType ;
            public Int32 ParamCount ;
            public Int32 P1_Session_Bytes ;
            public Int32 P2_viDesc_Bytes;
            public Int32 P3_Mode_Bytes;
            public Int32 P4_Timeout_Bytes;
        }
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct viStructRequest_viRead
        {
            public Byte FuncType;
            public Int32 ParamCount;
            public Int32 P1_vi_Bytes;
            public Int32 P2_InCount_Bytes;
            public Int32 Vi;
            public Int32 InCount;
        }
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct viStructResponseHeader_viRead
        {
            public Byte FuncType;
            public Int32 ParamCount;
            public Int32 P1_ReturnValue_Bytes;
            public Int32 P2_Return_Bytes;
            public Int32 ReturnValue;
        }
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct viStructResponse_viOpen
        {
            public Byte FuncType;
            public Int32 ParamCount;
            public Int32 P1_ReturnValue_Bytes;
            public Int32 P2_Vi_Bytes;

            public Int32 ReturnValue;
            public Int32 Vi;
        }
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct viStructResponse_viOpenDefaultRM
        {
            public Byte FuncType;
            public Int32 ParamCount;
            public Int32 P1_ReturnValue_Bytes;
            public Int32 P2_Session_Bytes;

            public Int32 ReturnValue;
            public Int32 Session;
        }
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct viStructResponse_WithOneReturn
        {
            public Byte FuncType;
            public Int32 ParamCount;
            public Int32 P1_ReturnValue_Bytes;
            public Int32 ReturnValue;
        }
        public static class Helper
        {
            #region Marshal
            public static byte[] StructToBytes(object structObj)
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


            public static T? BytesToStruct<T>(byte[] bytes, int startIndex, Type strcutType)
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
        }
    }
}
