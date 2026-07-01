namespace ScopeX.Visa32
{
    //using NationalInstruments.Visa;
    using ScopeX.Visa32;
    using System.Data;
    using System.Diagnostics;
    using System.IO.MemoryMappedFiles;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using static ScopeX.Visa32.Common;

    public class Visa32Stub
    {
        public Visa32Stub (int maxWaitMilliseconds) 
        {
            Process[] processes = Process.GetProcesses();
            string visa32ServerProcessName = "ScopeX.Visa32Entry";
            bool bNeedCreateVisa32ServerProcess = true;
            foreach (Process process in processes)
            {
                if (process.ProcessName == visa32ServerProcessName)
                {
                    bNeedCreateVisa32ServerProcess = false;
                    break;
                }
            }
            if (bNeedCreateVisa32ServerProcess)
            {
                ProcessStartInfo startInfo = new();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = "ScopeX.Visa32Entry.exe";
                //设置启动动作,确保以管理员身份运行
                startInfo.Verb = "runas";
                try
                {
                    Process.Start(startInfo);
                }
                catch
                {
                    return;
                }
            }
            Thread.Sleep(1000);

            c_serverRecvedMappedFile = MemoryMappedFile.OpenExisting(Common.ServerRecvedMappedFileName);
            c_serverSendMappedFile = MemoryMappedFile.OpenExisting(Common.ServerSendMappedFileName);
            serverRecvedMutex = Mutex.OpenExisting(Common.MutexName_ServerRecv);
            serverSendMutex = Mutex.OpenExisting(Common.MutexName_ServerSend);

            serverRecvAccessor = c_serverRecvedMappedFile.CreateViewAccessor(0, 10240);
            serverSendAccessor = c_serverSendMappedFile.CreateViewAccessor(0, 10240);
            MaxWaitMilliseconds = maxWaitMilliseconds;
        }
        MemoryMappedFile c_serverRecvedMappedFile;
        MemoryMappedFile c_serverSendMappedFile;

        Mutex serverRecvedMutex;
        Mutex serverSendMutex;
        MemoryMappedViewAccessor serverRecvAccessor;
        MemoryMappedViewAccessor serverSendAccessor;

        int MaxWaitMilliseconds = 50;
        private bool CheckSpecialValue(Mutex mutex, MemoryMappedViewAccessor accessor,Int32 pos,Byte eqValue,Int32 overMilliseconds)
        {
            if (!mutex.WaitOne(overMilliseconds))
                return false;
            Stopwatch stopwatch = Stopwatch.StartNew();
            byte mark = accessor.ReadByte(0);
            mutex.ReleaseMutex();
            while (mark != eqValue && stopwatch.ElapsedMilliseconds < overMilliseconds)
            {
                if (!mutex.WaitOne(overMilliseconds))
                    return false;
                mark = accessor.ReadByte(0);
                mutex.ReleaseMutex();
            }
            return (mark == eqValue);
        }
        private bool WaitAndWrite(Mutex mutex, MemoryMappedViewAccessor accessor, Int32 pos, Byte eqValue, Int32 overMilliseconds, byte[] buffer)
        {
            if (CheckSpecialValue(mutex,accessor, pos, eqValue, overMilliseconds))
            {
                if (!mutex.WaitOne(overMilliseconds))
                    return false;
                accessor.WriteArray<byte>(0, buffer, 0, buffer.Length);
                mutex.ReleaseMutex();
                return true;
            }
            return false;
        }
        private void EmptyReadShareMemory()
        {
            serverSendMutex.WaitOne();
            byte functionType = (byte)Common.FuncTypes.viNone;
            serverSendAccessor.Write<byte>(0, ref functionType);//置空
            serverSendMutex.ReleaseMutex();
        }
        public int viClose(Int32 vi)
        {
            //=========write
            Int32 writeParamCount = 1;
            MemoryStream memorystream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memorystream);
            binaryWriter.Write((byte)Common.FuncTypes.viClose);//mark
            binaryWriter.Write(writeParamCount);//param count

            binaryWriter.Write(sizeof(Int32));//param 1 byte count of vi
            //以上是header
            binaryWriter.Write(vi);
            binaryWriter.Flush();

            byte[] buffer = memorystream.ToArray();
            if (!WaitAndWrite(serverRecvedMutex, serverRecvAccessor,0, (byte)Common.FuncTypes.viNone, MaxWaitMilliseconds, buffer))
            {
                return (Int32)visa32.VI_ERROR_CLOSING_FAILED;
            }
            //============read
            if (!CheckSpecialValue(serverSendMutex, serverSendAccessor, 0, (byte)Common.FuncTypes.viClose, MaxWaitMilliseconds))
            {
                return (Int32)visa32.VI_ERROR_CLOSING_FAILED;
            }
            byte[] returnBytes = new byte[Marshal.SizeOf<viStructResponse_WithOneReturn>()];
            serverSendMutex.WaitOne();
            serverSendAccessor.ReadArray<byte>(0, returnBytes, 0, returnBytes.Length);
            serverSendMutex.ReleaseMutex();
            viStructResponse_WithOneReturn o_viStructResponse_WithOneReturn = Common.Helper.BytesToStruct<viStructResponse_WithOneReturn>(returnBytes, 0, typeof(viStructResponse_WithOneReturn));

            if (o_viStructResponse_WithOneReturn .ParamCount!= 1)
            {
                return visa32.VI_ERROR_CONN_LOST;
            }

            Int32 returnValue = o_viStructResponse_WithOneReturn.ReturnValue;
            EmptyReadShareMemory();
            return returnValue;
        }
        public Int32 viOpenDefaultRM(out Int32 sesn)
        {
            //write
            Int32 writeParamCount = 0;
            MemoryStream memorystream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memorystream);
            binaryWriter.Write((byte)Common.FuncTypes.viOpenDefaultRM);//mark
            binaryWriter.Write(writeParamCount);//param count
            binaryWriter.Flush();

            byte[] buffer = memorystream.ToArray();
            if (!WaitAndWrite(serverRecvedMutex, serverRecvAccessor, 0, (byte)Common.FuncTypes.viNone, MaxWaitMilliseconds, buffer))
            {
                sesn = -1;
                return (Int32)visa32.VI_ERROR_CLOSING_FAILED;
            }

            //=========read
            if (!CheckSpecialValue(serverSendMutex, serverSendAccessor, 0, (byte)Common.FuncTypes.viOpenDefaultRM, MaxWaitMilliseconds))
            {
                sesn = -1;
                return (Int32)visa32.VI_ERROR_CLOSING_FAILED;
            }

            serverSendMutex.WaitOne();
            Int32 paramStart = 0;
            Int32 paramCount = 0;
            paramStart += 1;
            paramCount = serverSendAccessor.ReadInt32(paramStart);
            paramStart += sizeof(Int32);//count
            if (paramCount != 2)
            {
                sesn=-1;
                serverSendMutex.ReleaseMutex();
                return (Int32)visa32.VI_ERROR_CONN_LOST;
            }
            paramStart += 2*sizeof(Int32);//2 Int32 param bytes

            Int32 returnValue = serverSendAccessor.ReadInt32(paramStart);
            paramStart += sizeof(Int32);
            sesn= serverSendAccessor.ReadInt32(paramStart);

            byte functionType = (byte)Common.FuncTypes.viNone;
            serverSendAccessor.Write<byte>(0, ref functionType);//置空
            serverSendMutex.ReleaseMutex();
            return returnValue;
        }
        public int viOpen(Int32 sesn, string viDesc, Int32 mode, Int32 timeout, out Int32 vi)
        {
            //==============write
            Int32 writeParamCount = 4;
            byte[] viDesc_Byte = Encoding.UTF8.GetBytes(viDesc);

            MemoryStream memorystream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memorystream);

            viStructRequestHeader_viOpen struct_ViOpen = new viStructRequestHeader_viOpen();
            struct_ViOpen.FuncType = (byte)Common.FuncTypes.viOpen;
            struct_ViOpen.ParamCount = writeParamCount;
            struct_ViOpen.P1_Session_Bytes = sizeof(Int32);
            struct_ViOpen.P2_viDesc_Bytes = (Int32)viDesc_Byte.Length;
            struct_ViOpen.P3_Mode_Bytes = sizeof(Int32);
            struct_ViOpen.P4_Timeout_Bytes = sizeof(Int32);
            byte[] headerBytes = Common.Helper.StructToBytes(struct_ViOpen);
            //viStruct_viOpen h= Common.Helper.BytesToStruct<viStruct_viOpen>(header,0, typeof(viStruct_viOpen));
            binaryWriter.Write(headerBytes);
            
            binaryWriter.Write(sesn);
            binaryWriter.Write(viDesc_Byte);
            binaryWriter.Write(mode); 
            binaryWriter.Write(timeout);
            binaryWriter.Flush();

            byte[] buffer = memorystream.ToArray();
            if (!WaitAndWrite(serverRecvedMutex, serverRecvAccessor, 0, (byte)Common.FuncTypes.viNone, MaxWaitMilliseconds, buffer))
            {
                vi = -1;
                return (Int32)visa32.VI_ERROR_CLOSING_FAILED;
            }

            //=========read
            if (!CheckSpecialValue(serverSendMutex, serverSendAccessor, 0, (byte)Common.FuncTypes.viOpen, MaxWaitMilliseconds))
            {
                vi = -1;
                return (Int32)visa32.VI_ERROR_CLOSING_FAILED;
            }
            byte[] returnBytes=new byte[Marshal.SizeOf<viStructResponse_viOpen>()];
            serverSendMutex.WaitOne();
            serverSendAccessor.ReadArray<byte>(0, returnBytes, 0, returnBytes.Length);
            serverSendMutex.ReleaseMutex();
            viStructResponse_viOpen o_viStructResponse_ViOpen = Common.Helper.BytesToStruct< viStructResponse_viOpen>(returnBytes,0,typeof(viStructResponse_viOpen));
            if (o_viStructResponse_ViOpen.ParamCount!=2)
            {
                vi = -1;
                return (Int32)visa32.VI_ERROR_CONN_LOST;
            }
            vi = o_viStructResponse_ViOpen.Vi;
            EmptyReadShareMemory();
            return o_viStructResponse_ViOpen.ReturnValue;
        }
        public int viPrintf(int vi, string writeFmt)
        {
            byte[] writeFmt_Bytes = Encoding.UTF8.GetBytes(writeFmt);
            //==============write
            Int32 writeParamCount = 2;
            MemoryStream memorystream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memorystream);
            binaryWriter.Write((byte)Common.FuncTypes.viPrintf);//mark
            binaryWriter.Write(writeParamCount);//param count
            binaryWriter.Write(sizeof(Int32));//vi
            binaryWriter.Write((Int32)writeFmt_Bytes.Length);//writeFmt
            //==content
            binaryWriter.Write(vi);
            binaryWriter.Write(writeFmt_Bytes);
            binaryWriter.Flush();

            byte[] buffer = memorystream.ToArray();
            if (!WaitAndWrite(serverRecvedMutex, serverRecvAccessor, 0, (byte)Common.FuncTypes.viNone, MaxWaitMilliseconds, buffer))
            {
                return (Int32)visa32.VI_ERROR_CLOSING_FAILED;
            }

            //=========read
            if (!CheckSpecialValue(serverSendMutex, serverSendAccessor, 0, (byte)Common.FuncTypes.viPrintf, MaxWaitMilliseconds))
            {
                return (Int32)visa32.VI_ERROR_CLOSING_FAILED;
            }

            Int32 paramStart = 0;
            Int32 paramCount = 0;
            serverSendMutex.WaitOne();
            paramStart += 1;
            paramCount = serverSendAccessor.ReadInt32(paramStart);
            paramStart += sizeof(Int32);//count
            if (paramCount != 1)
            {
                vi = -1;
                serverSendMutex.ReleaseMutex();
                return (Int32)visa32.VI_ERROR_CONN_LOST;
            }
            paramStart += sizeof(Int32);//vi's length

            Int32 returnValue = serverSendAccessor.ReadInt32(paramStart);

            byte functionType = (byte)Common.FuncTypes.viNone;
            serverSendAccessor.Write<byte>(0, ref functionType);//置空
            serverSendMutex.ReleaseMutex();
            return returnValue;
        }
        public int viWrite(int vi, byte[] in_buffer, int in_count, out int retCount)
        {
            //==============write
            Int32 writeParamCount = 2;
            MemoryStream memorystream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memorystream);
            binaryWriter.Write((byte)Common.FuncTypes.viWrite);//mark
            binaryWriter.Write(writeParamCount);//param count

            binaryWriter.Write(sizeof(Int32));//vi
            binaryWriter.Write((Int32)(in_buffer.Length));//vi
            //==content
            binaryWriter.Write(vi);
            //byte[] writeFmt_Byte = Encoding.UTF8.GetBytes(writeFmt);
            binaryWriter.Write(in_buffer);
            binaryWriter.Flush();

            byte[] buffer = memorystream.ToArray();
            if (!WaitAndWrite(serverRecvedMutex, serverRecvAccessor, 0, (byte)Common.FuncTypes.viNone, MaxWaitMilliseconds, buffer))
            {
                retCount = 0;
                return (Int32)visa32.VI_ERROR_CLOSING_FAILED;
            }

            //=========read
            if (!CheckSpecialValue(serverSendMutex, serverSendAccessor, 0, (byte)Common.FuncTypes.viWrite, MaxWaitMilliseconds))
            {
                retCount = 0;
                return (Int32)visa32.VI_ERROR_CLOSING_FAILED;
            }

            Int32 paramStart = 0;
            Int32 paramCount = 0;

            serverSendMutex.WaitOne();
            paramStart += 1;
            paramCount = serverSendAccessor.ReadInt32(paramStart);
            paramStart += sizeof(Int32);//count
            if (paramCount != 2)
            {
                retCount = 0;
                serverSendMutex.ReleaseMutex();
                return (Int32)visa32.VI_ERROR_CONN_LOST;
            }
            paramStart += sizeof(Int32);//returnValue's length
            paramStart += sizeof(Int32);//retCount's length

            Int32 returnValue = serverSendAccessor.ReadInt32(paramStart);
            paramStart += sizeof(Int32);//retCount
            retCount = serverSendAccessor.ReadInt32(paramStart);

            byte functionType = (byte)Common.FuncTypes.viNone;
            serverSendAccessor.Write<byte>(0, ref functionType);//置空
            serverSendMutex.ReleaseMutex();
            return returnValue;
        }
        public int viRead(int vi, byte[] in_buffer, int in_count, out int retCount)
        {
            viStructRequest_viRead o_viStructRequest_viRead = new();
            o_viStructRequest_viRead.FuncType = (byte)Common.FuncTypes.viRead;
            o_viStructRequest_viRead.ParamCount = 2;
            o_viStructRequest_viRead.P1_vi_Bytes = sizeof(Int32);
            o_viStructRequest_viRead.P2_InCount_Bytes = sizeof(Int32);
            o_viStructRequest_viRead.Vi=vi;
            o_viStructRequest_viRead.InCount=in_count;

            byte[] buffer = Common.Helper.StructToBytes(o_viStructRequest_viRead);
            if (!WaitAndWrite(serverRecvedMutex, serverRecvAccessor, 0, (byte)Common.FuncTypes.viNone, MaxWaitMilliseconds, buffer))
            {
                retCount = 0;
                return (Int32)visa32.VI_ERROR_CLOSING_FAILED;
            }

            //=========read
            if (!CheckSpecialValue(serverSendMutex, serverSendAccessor, 0, (byte)Common.FuncTypes.viRead, MaxWaitMilliseconds))
            {
                retCount = 0;
                return (Int32)visa32.VI_ERROR_CLOSING_FAILED;
            }

            Int32 paramStart = 0;
            Int32 paramCount = 0;
            serverSendMutex.WaitOne();

            paramStart += 1;
            paramCount = serverSendAccessor.ReadInt32(paramStart);
            paramStart += sizeof(Int32);//count
            if (paramCount != 2)
            {
                retCount = 0;
                serverSendMutex.ReleaseMutex();
                return (Int32)visa32.VI_ERROR_CONN_LOST;
            }
            paramStart += sizeof(Int32);//returnValue's length
            retCount = serverSendAccessor.ReadInt32(paramStart);
            paramStart += sizeof(Int32);//buffer's length
            //以上是header部分
            Int32 returnValue = serverSendAccessor.ReadInt32(paramStart);
            paramStart += sizeof(Int32);//returnValue's length

            serverSendAccessor.ReadArray<byte>(paramStart, in_buffer, 0, retCount);

            byte functionType = (byte)Common.FuncTypes.viNone;
            serverSendAccessor.Write<byte>(0, ref functionType);//置空
            serverSendMutex.ReleaseMutex();
            return returnValue;
        }
        public List<string> GetAllExistsResource()
        {
            List<string> returnValue = new List<string>();

            //==============write
            Int32 writeParamCount = 0;
            MemoryStream memorystream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memorystream);
            binaryWriter.Write((byte)Common.FuncTypes.GetAllExistsResource);//mark
            binaryWriter.Write(writeParamCount);//param count

            //==content
            binaryWriter.Flush();

            byte[] buffer = memorystream.ToArray();
            if (!WaitAndWrite(serverRecvedMutex, serverRecvAccessor, 0, (byte)Common.FuncTypes.viNone, MaxWaitMilliseconds, buffer))
            {
                return returnValue;
            }

            //=========read
            if (!CheckSpecialValue(serverSendMutex, serverSendAccessor, 0, (byte)Common.FuncTypes.GetAllExistsResource, 5*MaxWaitMilliseconds))
            {
                return returnValue;
            }

            Int32 paramStart = 0;
            Int32 paramCount = 0;

            serverSendMutex.WaitOne();
            paramStart += 1;
            paramCount = serverSendAccessor.ReadInt32(paramStart);
            paramStart += sizeof(Int32);//count
            List<Int32> stringListLength = new List<Int32>();
            while (paramCount>0)
            {
                stringListLength.Add(serverSendAccessor.ReadInt32(paramStart));
                paramStart += sizeof(Int32);//string bytes
                paramCount--;
            }
            foreach (Int32 length in stringListLength)
            {
                byte[] string_buf = new byte[length];
                serverSendAccessor.ReadArray<byte>(paramStart, string_buf, 0, length);
                paramStart += length;
                returnValue.Add(System.Text.Encoding.UTF8.GetString(string_buf));
            }
            byte functionType = (byte)Common.FuncTypes.viNone;
            serverSendAccessor.Write<byte>(0, ref functionType);//置空

            serverSendMutex.ReleaseMutex();
            return returnValue;
        }
    }
}