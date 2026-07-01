// // ******************************************************************
// //       /\ /|       @File         ProbeMcuUpdater.cs
// //       \ V/        @Brief
// //       | "")       @Author        lijinwen, ghz005@uni-trend.com.cn
// //       /  |        @Creation      2024-06-17
// //      /  \\        @Modified      2024-06-18
// //    *(__\_\
// // ******************************************************************
namespace ScopeX.Hardware.Driver;

using ScopeX.ComModel;
using ScopeX.Updater.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using static ScopeX.Hardware.Driver.CtrlAnalogChannel_JiHe2d5G;
using static ScopeX.Hardware.Driver.McuComPortUpdater;

public class ProbeMcuUpdater
{
    //分段发送23.6.17 ljw
    private const int SPAN_SIZE = 2 * 1024;
    private static McuComPortUpdater? _mcuComPortUpdater;
    private static Action<string>? _debugWriter;
    static UpdateItem _updateItem;
    #region 更新相关

    private static void WriteLog(string msg)
    {
        _debugWriter?.Invoke($"探头更新：{msg}");
    }
    //打开串口
    private static bool OpenComPort_Step1()
    {
        _mcuComPortUpdater = CtrlAnalogChannel_JiHe2d5G.baseObj1;
        string boardName = _updateItem.BoardName.Trim();
        string comPortNum = boardName.Substring(boardName.Length - 1, 1);

        bool bOpened = _mcuComPortUpdater.Open($"COM{comPortNum}");
        switch (bOpened)
        {
            case false:
                WriteLog($"updaterItemType={_updateItem.Type},BoardName={_updateItem.BoardName}对用的Mcu通信接口还没有对应实现！");
                return false;
            default:
                return McuComPortWorkStateCheck();
        }
    }
    private static bool BoardAndProbeSupportPortUpdate_Step2()
    {
        if (_mcuComPortUpdater == null)
        {
            WriteLog("mcuComPortUpdater is null");
            return false;
        }
        if (!_mcuComPortUpdater.McuUpdate_IsRunAtApp(2500))
        {
            WriteLog($"updaterItemType={_updateItem.Type},BoardName={_updateItem.BoardName} 通道板未成功进入BOOT");
            WriteLog(!_mcuComPortUpdater.McuUpdate_IsRunAtApp(2500) ? $"updaterItemType={_updateItem.Type},BoardName={_updateItem.BoardName} 通道板未工作在APP，可能通道板未上电" : $"updaterItemType={_updateItem.Type},BoardName={_updateItem.BoardName} 通道板目前工作在APP");
            _mcuComPortUpdater.Close();
            return false;
        }
        var sendData = new List<byte>();
        //通道底板是否支持探头自动更新
        _mcuComPortUpdater.SendData(true, (byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x7B_Request_ProbeUpdateSupport_Chlboard, sendData);
        if (_mcuComPortUpdater.ReadSpecailMessage((byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x7B_Request_ProbeUpdateSupport_Chlboard, 2000, true, out var result))
        {
            if (result.Value.dataLength == 0 || result.Value.Data[0] != 1)
            {
                WriteLog("CMD【0x7B】通道板版本不支持更新探头");
                return false;
            }
            else
            {
                WriteLog("CMD【0x7B】通道底板支持探头更新");
            }
        }
        else
        {
            WriteLog("CMD【0x7B】错误，可能通道板版本不支持更新探头");
            return false;
        }
        //探头后端是否支持自动更新
        _mcuComPortUpdater.SendData(true, (byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x7C_Request_ProbeUpdateSupport_Backend, sendData);
        if (_mcuComPortUpdater.ReadSpecailMessage((byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x7C_Request_ProbeUpdateSupport_Backend, 2000, true, out result))
        {
            if (result.Value.dataLength == 0 || result.Value.Data[0] != 1)
            {
                WriteLog("CMD【0x7C】错误，可能探头不支持更新");
                return false;
            }
            else
            {
                WriteLog("CMD【0x7C】探头后端支持探头更新");
            }
        }
        else
        {
            WriteLog("CMD【0x7C】错误，可能探头不支持更新");
            return false;
        }

        return true;
    }
    private static bool McuComPortWorkStateCheck()
    {
        return _mcuComPortUpdater is { Connected: true };
    }

    private static bool SendDataToBoardStage1_Step3(List<byte> sendAllData, int fillCount = 0)
    {
        //发送数据到通道
        if (_mcuComPortUpdater == null || !McuComPortWorkStateCheck())
        {
            return false;
        }
        ////////////////////////////////////////////////////////////////////////////////
        /// 发送

        int errorTimes = 0;//分包发送失败次数
        int tmpSendTimes = 0;//分包发送多少次
        int alwarySendCount = 0;//已发送字节
        while (alwarySendCount < sendAllData.Count && errorTimes < 100)
        {
            Debug.WriteLine($"L588: SendTimes:{tmpSendTimes}");

            var sendData = new List<byte>();

            #region 装载本次发送的字节 每次最大发送 SPAN_SIZE = 2K
            int nowCount = ((sendAllData.Count - alwarySendCount) > SPAN_SIZE) ? SPAN_SIZE : (sendAllData.Count - alwarySendCount);
            for (int i = 0; i < nowCount; i++)
            {
                sendData.Add(sendAllData[alwarySendCount + i]);
            }
            #endregion

            #region 追加四个字节告诉通道底板存放偏移 WriteAtAddress
            sendData.Add((byte)((alwarySendCount & 0x00_00_00_ff) >> 0));
            sendData.Add((byte)((alwarySendCount & 0x00_00_ff_00) >> 8));
            sendData.Add((byte)((alwarySendCount & 0x00_ff_00_00) >> 16));
            sendData.Add((byte)((alwarySendCount & 0xff_00_00_00) >> 24));
            #endregion

            #region 追加四个字节告诉通道第八版本次校验 CRC Code
            UInt32 crcCode = CRC32.GetCRC32Code(sendData);
            sendData.Add((byte)((crcCode & 0x00_00_00_ff) >> 0));
            sendData.Add((byte)((crcCode & 0x00_00_ff_00) >> 8));
            sendData.Add((byte)((crcCode & 0x00_ff_00_00) >> 16));
            sendData.Add((byte)((crcCode & 0xff_00_00_00) >> 24));
            #endregion

            _mcuComPortUpdater.ClearSpecialReceiveQueue((byte)Updater_ReqScopeXommands.CMD0x71_Request_Diff_UpdateSend_Stage1);
            _mcuComPortUpdater.SendData(true, (byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x71_Request_Diff_UpdateSend_Stage1, sendData);
            tmpSendTimes++;

            int maxWaitMilliseconds = (int)((sendData.Count + 8) * 1000.0 / (115200 / 10) * 2) * 10;
            if (_mcuComPortUpdater.ReadSpecailMessage((byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x71_Request_Diff_UpdateSend_Stage1, maxWaitMilliseconds, true, out var result))
            {
                if (result.Value.dataLength >= 1)
                {
                    if (result.Value.Data[0] == 0x00)
                    {
                        alwarySendCount += nowCount;
                        continue;
                    }
                    else
                    {
                        WriteLog("CMD【0x71】探头更新 error:result.Value.Data[0] != 0x00");
                    }
                }
                else
                {
                    WriteLog("CMD【0x71】探头更新 error:result.Value.dataLength == 0x00");
                }
            }
            else
            {
                WriteLog("CMD【0x71】探头更新 error: ReadSpecailMessage failed");
            }
            errorTimes++;
        }

        if (errorTimes >= 100)
        {
            //发送给失败
            WriteLog("CMD【0x71】数据传输都通道底板失败");
            return false;
        }
        else
        {
            //发送给失败
            WriteLog("CMD【0x71】数据传输到通道底板");
            return true;
        }
    }

    private static bool SendDataVerifyStage1_Step3(List<byte> srcData, out UInt32 srcDataCrc)
    {
        var sendData = new List<byte>();

        #region 附加整个数据长度 totalBytes 

        sendData.Add((byte)((srcData.Count & 0x00_00_00_ff) >> 0));
        sendData.Add((byte)((srcData.Count & 0x00_00_ff_00) >> 8));
        sendData.Add((byte)((srcData.Count & 0x00_ff_00_00) >> 16));
        sendData.Add((byte)((srcData.Count & 0xff_00_00_00) >> 24));

        #endregion

        #region 附加整个数据校验 CRC Code 

        srcDataCrc = CRC32.GetCRC32Code(srcData);
        sendData.Add((byte)((srcDataCrc & 0x00_00_00_ff) >> 0));
        sendData.Add((byte)((srcDataCrc & 0x00_00_ff_00) >> 8));
        sendData.Add((byte)((srcDataCrc & 0x00_ff_00_00) >> 16));
        sendData.Add((byte)((srcDataCrc & 0xff_00_00_00) >> 24));

        #endregion


        _mcuComPortUpdater.SendData(true, (byte)Updater_ReqScopeXommands.CMD0x72_Request_Diff_UpdateVerify_Stage1, sendData);
        if (_mcuComPortUpdater.ReadSpecailMessage((byte)Updater_ReqScopeXommands.CMD0x72_Request_Diff_UpdateVerify_Stage1, 5000, true, out var result))
        {
            int readbackLength = result.Value.Data[3];
            readbackLength <<= 8;
            readbackLength |= result.Value.Data[2];
            readbackLength <<= 8;
            readbackLength |= result.Value.Data[1];
            readbackLength <<= 8;
            readbackLength |= result.Value.Data[0];

            UInt32 readbackCrcCode = 0;
            readbackCrcCode |= result.Value.Data[4 + 3];
            readbackCrcCode <<= 8;
            readbackCrcCode |= result.Value.Data[4 + 2];
            readbackCrcCode <<= 8;
            readbackCrcCode |= result.Value.Data[4 + 1];
            readbackCrcCode <<= 8;
            readbackCrcCode |= result.Value.Data[4 + 0];

            if (readbackLength != srcData.Count || readbackCrcCode != srcDataCrc)
            {
                WriteLog("CMD【0x72】探头更新失败，通道底板数据验证失败 ");
                return false;
            }
            else
            {
                WriteLog("CMD【0x72】更新数据传到通道底板验证成功 ");
                return true;
            }
        }
        else
        {
            WriteLog("CMD【0x72】探头更新失败，通道底板数据验证失败 ");
            return false;
        }
    }

    private static bool ChangeProbeToBoot_Step4()
    {
        if (_mcuComPortUpdater == null || !McuComPortWorkStateCheck())
        {
            WriteLog("错误，串口打开失败");
            return false;
        }
        var sendData = new List<byte>();

        //触发探头运行模式切换,切换后探头上下文丢失，不检测返回信息，直接等待5秒后继续
        _mcuComPortUpdater.SendData(true, (byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x73_Request_Diff_UpdateStart, sendData);
        WriteLog("CMD【0x73】 指示探头进入BOOT模式");
        //if (_mcuComPortUpdater.ReadSpecailMessage((byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x73_Request_Diff_UpdateStart, 2000, true, out var result1))
        //{
        //    if (result1.Value.dataLength == 0 || result1.Value.Data[0] != 1)
        //    {
        //        WriteLog("CMD【0x73】 指示探头进入BOOT模式,失败,返回值错误");
        //        return false;
        //    }
        //    else
        //    {
        //        WriteLog("CMD【0x73】 指示探头进入BOOT模式");
        //    }
        //}
        //else
        //{
        //    WriteLog("CMD【0x73】 指示探头进入BOOT模式,失败");
        //    return false;
        //}

        //等待5秒查询探头运行模式
        Thread.Sleep(5000);
        if (!Probe_IsRunAtBoot(2000))
        {
            WriteLog("CMD【0x74】 探头开始更新,失败,探头未进入BOOT");
            return false;
        }
        else
        {
            WriteLog("CMD【0x74】 探头开始更新,探头运行在boot ");
        }

        //初始化探头BOOT中的更新变量，【重复一次，考虑是从APP1切换到BOOT】
        _mcuComPortUpdater.SendData(true, (byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x73_Request_Diff_UpdateStart, sendData);
        if (_mcuComPortUpdater.ReadSpecailMessage((byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x73_Request_Diff_UpdateStart, 2000, true, out var result2))
        {
            if (result2.Value.dataLength == 0 || result2.Value.Data[0] != 1)
            {
                WriteLog("CMD【0x73】 指示探头开始更新,失败,返回值错误");
                return false;
            }
            else
            {
                WriteLog("CMD【0x73】 指示探头-开始更新");
            }
        }
        else
        {
            WriteLog("CMD【0x73】 指示探头开始更新,失败");
            return false;
        }
        return true;
    }

    public static bool Probe_IsRunAtBoot(int maxWaitMicroseconds)
    {
        if (_mcuComPortUpdater == null || !McuComPortWorkStateCheck())
        {
            return false;
        }
        for (int i = 0; i < maxWaitMicroseconds / 50; i++)
        {
            _mcuComPortUpdater.SendData(true, (byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x74_Request_Diff_QueryRunAt, null);
            if (_mcuComPortUpdater.ReadSpecailMessage((byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x74_Request_Diff_QueryRunAt, 1000, true, out var readbackResult))
            {
                if (readbackResult.Value.Data[0] == McuComPortUpdater.RUNNING_AT_BOOT)
                {
                    return true;
                }
            }
            else
                Thread.Sleep(50);
        }
        return false;
    }

    /// <summary>
    /// Step5:数据准备 0x75 0x76 0x77
    /// </summary>
    /// <returns></returns>
    private static bool SendDataToProbeEPROM_Step5(int dataRealLen, UInt32 dataRealCrc)
    {
        if (_mcuComPortUpdater == null || !McuComPortWorkStateCheck())
        {
            WriteLog("错误，串口打开失败");
            return false;
        }
        var sendData = new List<byte>();
        sendData.Add((byte)((dataRealLen & 0x00_00_00_ff) >> 0));
        sendData.Add((byte)((dataRealLen & 0x00_00_ff_00) >> 8));
        sendData.Add((byte)((dataRealLen & 0x00_ff_00_00) >> 16));
        sendData.Add((byte)((dataRealLen & 0xff_00_00_00) >> 24));
        sendData.Add((byte)((dataRealCrc & 0x00_00_00_ff) >> 0));
        sendData.Add((byte)((dataRealCrc & 0x00_00_ff_00) >> 8));
        sendData.Add((byte)((dataRealCrc & 0x00_ff_00_00) >> 16));
        sendData.Add((byte)((dataRealCrc & 0xff_00_00_00) >> 24));


        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// 特别注意：等待时间按照1K数据进行预估；因为探头后端【【资源有限】】，通道底板每次传输1K，然后通道底板挪动数据到EEPROM，再分小块读取做对比验证写入是否正确
        var blockLen = 1024;
        var blockNum = (dataRealLen + (blockLen - 1)) / blockLen;//不足1K按1K计

        _mcuComPortUpdater.SendData(true, (byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x75_Request_Diff_UpdateSend_Stage2, sendData);
        {//数据转到探头EEPROM
            if (_mcuComPortUpdater.ReadSpecailMessage((byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x75_Request_Diff_UpdateSend_Stage2,
                    //预计等待时间 4s/K
                    blockNum * 4 * 1000, true, out var result))
            {
                if (result.Value.dataLength == 0 || result.Value.Data[0] != 1)
                {
                    WriteLog("CMD【0x75】 更新包通道板发送到探头,失败,返回值错误");
                    return false;
                }
                else
                {
                    WriteLog("CMD【0x75】 更新包通道板发送到探头,成功");
                }
            }
            else
            {
                WriteLog("CMD【0x75】 更新包通道板发送到探头,失败");
                return false;
            }
        }

        _mcuComPortUpdater.SendData(true, (byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x76_Request_Diff_UpdateVerify_Stage2, sendData);
        {//探头EEPROM数据校验
            if (!_mcuComPortUpdater.ReadSpecailMessage((byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x76_Request_Diff_UpdateVerify_Stage2,
                    //预计等待时间 1s/K,+5(因为通道底板等待探头blockNum秒)
                    (blockNum +5)* 1 * 1000, true, out var result))
            {
                WriteLog("CMD【0x76】 探头数据校验,失败");
                return false;
            }
            if (result.Value.dataLength != 8)
            {
                WriteLog("CMD【0x76】 探头数据校验,失败,返回值错误");
                return false;
            }

            int readbackLength = result.Value.Data[3];
            readbackLength <<= 8;
            readbackLength |= result.Value.Data[2];
            readbackLength <<= 8;
            readbackLength |= result.Value.Data[1];
            readbackLength <<= 8;
            readbackLength |= result.Value.Data[0];

            UInt32 readbackCrcCode = 0;
            readbackCrcCode |= result.Value.Data[4 + 3];
            readbackCrcCode <<= 8;
            readbackCrcCode |= result.Value.Data[4 + 2];
            readbackCrcCode <<= 8;
            readbackCrcCode |= result.Value.Data[4 + 1];
            readbackCrcCode <<= 8;
            readbackCrcCode |= result.Value.Data[4 + 0];

            if (readbackLength != dataRealLen || readbackCrcCode != dataRealCrc)
            {
                WriteLog("CMD【0x76】探头更新失败，探头接收数据验证失败 ");
                return false;
            }
            else
            {
                WriteLog("CMD【0x76】探头数据发送与校验,成功 ");
                return true;
            }
        }
    }
    private static bool SendDataToProbeFlash_Step6(int dataRealLen, UInt32 dataRealCrc)
    {
        if (_mcuComPortUpdater == null || !McuComPortWorkStateCheck())
        {
            WriteLog("错误，串口打开失败");
            return false;
        }
        var sendData = new List<byte>();
        sendData.Add((byte)((dataRealLen & 0x00_00_00_ff) >> 0));
        sendData.Add((byte)((dataRealLen & 0x00_00_ff_00) >> 8));
        sendData.Add((byte)((dataRealLen & 0x00_ff_00_00) >> 16));
        sendData.Add((byte)((dataRealLen & 0xff_00_00_00) >> 24));
        sendData.Add((byte)((dataRealCrc & 0x00_00_00_ff) >> 0));
        sendData.Add((byte)((dataRealCrc & 0x00_00_ff_00) >> 8));
        sendData.Add((byte)((dataRealCrc & 0x00_ff_00_00) >> 16));
        sendData.Add((byte)((dataRealCrc & 0xff_00_00_00) >> 24));

        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// 特别注意：等待时间按照1K数据进行预估；因为探头后端【【资源有限】】，通道底板每次传输1K，然后通道底板挪动数据到EEPROM，再分小块读取做对比验证写入是否正确
        var blockLen = 1024;
        var blockNum = (dataRealLen + (blockLen - 1)) / blockLen;//不足1K按1K计

        _mcuComPortUpdater.SendData(true, (byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x78_Request_Diff_UpdateSwitch, sendData);
        if (_mcuComPortUpdater.ReadSpecailMessage((byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x78_Request_Diff_UpdateSwitch,
                //等待15秒 因为通道底板等待10秒(探头后端有FLASH操作)
                15*1*1000, true, out var result))
        {
            if (result.Value.dataLength == 0 || result.Value.Data[0] != 1)
            {
                WriteLog("CMD【0x78】 发送差分探头MCU程序部署指令,失败,返回值错误");
                return false;
            }
            else
            {
                WriteLog("CMD【0x78】 发送差分探头MCU程序部署指令成功");
            }
        }
        else
        {
            WriteLog("CMD【0x78】 发送差分探头MCU程序部署指令,失败");
            return false;
        }

        _mcuComPortUpdater.SendData(true, (byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x79_Request_Diff_QueryVerify3, sendData);
        if (_mcuComPortUpdater.ReadSpecailMessage((byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x79_Request_Diff_QueryVerify3,
                //等待两秒
                2*1 * 1000, true, out result))
        {
            if (result.Value.dataLength == 0 || result.Value.Data[0] != 1)
            {
                WriteLog("CMD【0x79】 发送差分探头MCU程序切换校验查询,失败,返回值错误");
                return false;
            }
            else
            {
                WriteLog("CMD【0x79】 发送差分探头MCU程序切换校验成功");
            }
        }
        else
        {
            WriteLog("CMD【0x79】 发送差分探头MCU程序切换校验查询,失败");
            return false;
        }


        return true;

    }

    #endregion

    public static bool DoUpdate(UpdateItem item, Action<string>? debuger)
    {
        #region step0: 基础准备  
        _debugWriter = debuger;
        _updateItem  = item;

        var lastPaddingLen = 0;                         //尾部填充长度
        var dataToWrite = new List<byte>();             //要写入的数据        
        var dataRealLen = _updateItem.Content.Length;   //实际数据商都
        UInt32 dataRealCrc = 0;

        if (dataRealLen <= 0)
        {
            WriteLog("错误,固件数据包长度为0");
            return false;
        }
        else
        {
            dataToWrite.AddRange(_updateItem.Content);
            var padding = lastPaddingLen = (SPAN_SIZE - (dataRealLen % SPAN_SIZE));
            while (padding -- > 0)
            {
                dataToWrite.Add(0xFF);
            }
        }
        #endregion

        #region step1: 打开串口
        
        if (!OpenComPort_Step1())
        {
            WriteLog("错误,串口打开失败");
            return false;
        }
        else
        {
            WriteLog("串口打开成功");
            Thread.Sleep(500);
        }

        #endregion

        #region step2: 更新准备 查询是否支持探头升级，通道底板(0x7B)、探头后端(0像C)；预计耗时6s

        if (!BoardAndProbeSupportPortUpdate_Step2())
        {
            _mcuComPortUpdater?.Close();
            return false;
        }

        #endregion

        #region Step3: 更新准备 切换探头Boot(0x73)并查询(0x74) 重置底板和探头内部关于探头升级升级的变量

        if (!ChangeProbeToBoot_Step4())
        {
            _mcuComPortUpdater?.Close();
            return false;
        }

        Thread.Sleep(100);

        #endregion

        #region Step4: 阶段1 发送数据到通道板(0x71)并验证验证(0x72)


        if (!SendDataToBoardStage1_Step3(dataToWrite, lastPaddingLen))
        {
            _mcuComPortUpdater?.Close();
            return false;
        }

        if (!SendDataVerifyStage1_Step3(_updateItem.Content.ToList(), out dataRealCrc))
        {
            _mcuComPortUpdater?.Close();
            return false;
        }
        
        Thread.Sleep(100);

        #endregion


        #region Step5: 阶段2 数据转移到探头缓冲区：转移(0x75) 验证（0x76) 验证查询(0x77)

        if (!SendDataToProbeEPROM_Step5(dataRealLen, dataRealCrc))
        {
            _mcuComPortUpdater?.Close();
            return false;
        }

        #endregion

        #region Step6: 阶段3 数据转移到探头目标区：转移(0x78) 验证(0x79)

        if (!SendDataToProbeFlash_Step6(dataRealLen, dataRealCrc))
        {
            _mcuComPortUpdater?.Close();
            return false;
        }

        #endregion

        #region Setp7: 完成处理

        if (_mcuComPortUpdater == null || !McuComPortWorkStateCheck())
        {
            _mcuComPortUpdater?.Close();
            WriteLog("错误，串口打开失败");
            return false;
        }

        _mcuComPortUpdater.SendData(true, (byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x7A_Request_Diff_UpdateFinished, new List<byte>());
        if (_mcuComPortUpdater.ReadSpecailMessage((byte)McuComPortUpdater.Updater_ReqScopeXommands.CMD0x7A_Request_Diff_UpdateFinished,
                (5+2)*1*1000, true, out var result))
        {
            if (result.Value.dataLength == 0 || result.Value.Data[0] != 1)
            {
                _mcuComPortUpdater?.Close();
                WriteLog("CMD【0x7A】 发送差分探头MCU程序更新完成成指令,失败,返回值错误");
                return false;
            }
            else
            {
                WriteLog("CMD【0x7A】 发送差分探头MCU程序更新完成成指令成功");
            }
        }
        else
        {
            _mcuComPortUpdater?.Close();
            WriteLog("CMD【0x7A】 发送差分探头MCU程序更新完成成指令,失败");
            return false;
        }

        #endregion

        _mcuComPortUpdater?.Close();
        WriteLog("探头更新成功");

        return true;
    }
}
