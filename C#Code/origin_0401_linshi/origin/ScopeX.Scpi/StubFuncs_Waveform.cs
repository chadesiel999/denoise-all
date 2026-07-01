using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Hardware;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        private enum WaveMode
        {
            [Description("屏幕数据")]
            Normal,
            [Description("深存储数据")]
            Raw,
        }

        private enum WaveFormat
        {
            [Description("单字节数据")]
            BYTe,
            [Description("以科学计数形式返回各波形点的实际电压值")]
            ASCII,
            [Description("双字节数据")]
            WORD,
            [Description("四字节数据")]
            DWORD,
        }
        /// <summary>
        /// 欲读取波形起始点
        /// </summary>
        private static Int64 _ReadWaveStart = 0;
        /// <summary>
        /// 欲读取的波形总点数
        /// </summary>
        private static Int64 _ReadWavePoints = 25000;
        /// <summary>
        /// 欲读取的波形数据源
        /// </summary>
        private static ChannelId _WaveChnl = ChannelId.C1;
        /// <summary>
        /// 欲读取波形的格式
        /// </summary>
        private static WaveFormat _WaveFormat = WaveFormat.BYTe;
        /// <summary>
        /// 欲读取波形数据来源类型
        /// </summary>
        private static WaveMode _WaveMode = WaveMode.Normal;
        /// <summary>
        /// Get wave max points
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private static Int64 GetMaxWavePoints(WaveMode mode)
        {
            var point = mode == WaveMode.Normal ? Presenter.Timebase.TakeViewWaveDotsCnt : Presenter.Timebase.StorageWaveDotsCnt;
            return point;
        }

        /// <summary>
        /// Get Wave Max start
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private static Int64 GetMaxWaveStart(WaveMode mode) => GetMaxWavePoints(mode) - 1L;

        /// <summary>
        /// 设置欲读取的波形的起始地址
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_WavStart(SCPICommandProcessFuncParam analyResult)
        {
            //todo 目前只有 正常模式、后续加入RAW和MAX模式
            if (analyResult.Params != null && analyResult.Params.Count > 0 && Int64.TryParse(encodingBytes(analyResult.Params[0]), out var point))
            {
                if (point >= 0 && point <= GetMaxWaveStart(_WaveMode))
                {
                    _ReadWaveStart = point;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 查询欲读取的波形的起始地址
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_WavStart(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            sendMessage.SendData = Encoding.UTF8.GetBytes($"{_ReadWaveStart}");
            sendMessage.UsingScientificNotation = false;
            return true;
        }

        //================= 波形数据 ===============================================================================================
        /// <summary>
        /// 查询波形数据读取的通道源
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_WavSource(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;

            string outputString = "";
            if (scpiTagObj.ParamList != null && scpiTagObj.ParamList.Count > 0)
            {
                //需要解决ParamList定义的连续顺序与枚举Int值不连续的问题
                if (_WaveChnl is ChannelId channelId)
                {
                    //22.2.22
                    if (channelId <= ChannelId.C4)
                    {
                        outputString = scpiTagObj.ParamList[channelId - ChannelId.C1];
                    }
                    else
                    {
                        var index = scpiTagObj.ParamList.FindLastIndex(s => s.ToUpper() == channelId.ToString().ToUpper());
                        if (index >= 0)
                        {
                            outputString = scpiTagObj.ParamList[index];
                        }
                    }
                    sendMessage.SendData = Encoding.UTF8.GetBytes(outputString);
                    return true;
                }
                else
                {
                    var findindex = scpiTagObj.ParamList.FindIndex(param => param.ToUpper() == _WaveChnl.ToString()!.ToUpper());
                    if (findindex != -1)
                    {
                        outputString = scpiTagObj.ParamList[findindex];
                        sendMessage.SendData = Encoding.UTF8.GetBytes(outputString);
                        return true;
                    }
                    else
                    {
                        var enumIndex = (Int32)_WaveChnl;
                        var okParamListIndex = 0;
                        bool bFound = false;
                        foreach (int enumValue in Enum.GetValues(_WaveChnl.GetType()))
                        {
                            if (enumValue == enumIndex)
                            {
                                bFound = true;
                                break;
                            }
                            okParamListIndex++;
                        }

                        if (bFound && okParamListIndex < scpiTagObj.ParamList.Count)
                        {
                            outputString = scpiTagObj.ParamList[okParamListIndex];
                            sendMessage.SendData = Encoding.UTF8.GetBytes(outputString);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置波形数据读取的通道源
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_WavSource(SCPICommandProcessFuncParam analyResult)
        {
            if (analyResult.Params != null && analyResult.Params.Count > 0)
            {
                if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                    return false;

                var propertyInfo = ((Type)scpiTagObj.PrsntObj).GetProperty(scpiTagObj.PropertyName);
                ChannelId setValue = (ChannelId)ConvertObject(Encoding.UTF8.GetString(analyResult.Params[0]), propertyInfo.PropertyType, scpiTagObj.ParamList);
                if (setValue <= ChannelId.M8)
                {
                    _WaveChnl = setValue;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 查询波形数据的返回格式
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_WavFormat(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj)
                return false;
            string outputString = "";
            if (scpiTagObj.ParamList != null && scpiTagObj.ParamList.Count > 0)
            {
                outputString = scpiTagObj.ParamList[(Int32)_WaveFormat];
                sendMessage.SendData = Encoding.UTF8.GetBytes(outputString);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置波形数据模式
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_WavFormat(SCPICommandProcessFuncParam analyResult)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj)
                return false;
            if (scpiTagObj.ParamList == null || scpiTagObj.ParamList.Count == 0)
            {
                return false;
            }
            if (analyResult.Params != null && analyResult.Params.Count > 0)
            {
                var format = encodingBytes(analyResult.Params[0]).Trim();
                var index = scpiTagObj.ParamList.FindIndex(parm => shortCMD(parm) == format || parm.ToUpper() == format.ToUpper());
                if (index == -1)
                {
                    return false;
                }

                _WaveFormat = (WaveFormat)index;

                return true;
            }
            return false;
        }

        /// <summary>
        /// 查询波形数据模式
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_WavMode(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj)
                return false;
            string outputString = "";
            if (scpiTagObj.ParamList != null && scpiTagObj.ParamList.Count > 0)
            {
                outputString = scpiTagObj.ParamList[(Int32)_WaveMode];
                sendMessage.SendData = Encoding.UTF8.GetBytes(outputString);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置波形数据模式
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_WavMode(SCPICommandProcessFuncParam analyResult)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj)
                return false;
            if (scpiTagObj.ParamList == null || scpiTagObj.ParamList.Count == 0)
            {
                return false;
            }
            if (analyResult.Params != null && analyResult.Params.Count > 0)
            {
                var format = encodingBytes(analyResult.Params[0]).Trim();
                var index = scpiTagObj.ParamList.FindIndex(parm => shortCMD(parm) == format || parm.ToUpper() == format.ToUpper());
                if (index == -1)
                {
                    return false;
                }
                _WaveMode = (WaveMode)index;

                return true;
            }
            return false;
        }
        /// <summary>
        /// 查询欲读取的波形点数
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_WavPoint(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            sendMessage.SendData = Encoding.UTF8.GetBytes($"{_ReadWavePoints}");
            sendMessage.UsingScientificNotation = false;
            return true;
        }
        /// <summary>
        /// 设置欲读取的波形点数
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_WavPoint(SCPICommandProcessFuncParam analyResult)
        {
            //todo 目前只有 正常模式、后续加入RAW和MAX模式
            if (analyResult.Params != null && analyResult.Params.Count > 0 && Int64.TryParse(encodingBytes(analyResult.Params[0]), out var point))
            {
                _ReadWavePoints = Math.Clamp(point, 0L, GetMaxWavePoints(_WaveMode));
                return true;
            }

            return false;
        }

        public static bool scpiQuy_WavData(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            return _WaveMode == WaveMode.Normal
                ? scpiQuy_WavNormalData(analyResult, ref sendMessage)
                : scpiQuy_WavRawData(analyResult, ref sendMessage);
        }

        /// <summary>
        /// 深存储数据单次读取长度
        /// </summary>
        private static Int64 ReadMaxPoints => Constants.PRODUCT switch
        {
            ProductType.JiHe_MSO7000X => 5 * 1000 * 1000L,
            ProductType.JiHe_UPO7000L => 5 * 1000 * 1000L,
            _ => 5 * 1000 * 1000L
        };

        /// <summary>
        /// 读取深存储数据 分多次读取 且只支持读模拟通道
        /// 当前支持单次最大读取为5M数据+30个字节的包头
        /// ADC为大于8Bit时必须采用双字节格式，如未采用双字节模式 则蒋返回失败
        /// 多次读取时 第一次从start（默认为0）位置开始读取
        /// 下次读取时先查寻start是否为-1，若为-1表示深存储数据已经读取完成，否则循环读取，直至为-1
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_WavRawData(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            #region 准备工作

            if (!_WaveChnl.IsAnalog())
                return false;
            CheckWaveFormat();

            var storagepoints = Presenter.Timebase.StorageWaveDotsCnt;
            var mask = nameof(DataRole.SourceData);
            WfmPkgInfo? wfmPkg;
            ReadInfo readInfo;
            //1、存储深度小于5M 则一次性读完
            if (storagepoints < ReadMaxPoints)
            {
                var start = 0L;//读起始时间（点数）
                wfmPkg = new WfmPkgInfo(Presenter.Timebase.StorageWaveDotsCnt, double.MaxValue, start);
                readInfo = new ReadInfo(AcqDataType.AnalogChannel, new() { _WaveChnl }, wfmPkg, mask);
                _ReadWaveStart = -1;

            }
            else //大存储深度下 则需要分多次读取
            {
                //先判断读起始位置
                _ReadWaveStart = _ReadWaveStart >= storagepoints ? 0 : _ReadWaveStart;

                //计算读取长度 读起始位置 + 读取长度 > 存储深度 ？
                var readlength = _ReadWaveStart + ReadMaxPoints > storagepoints ? storagepoints - _ReadWaveStart : ReadMaxPoints;


                wfmPkg = new WfmPkgInfo(readlength, double.MaxValue, _ReadWaveStart);
                readInfo = new ReadInfo(AcqDataType.AnalogChannel, new() { _WaveChnl }, wfmPkg, mask);

                //读取完成之后重新更新读起始位置 下一次读起始位置 = 上一次读起始位置 + 读长度
                _ReadWaveStart += readlength;

                //判断是否读完
                _ReadWaveStart = _ReadWaveStart >= storagepoints ? -1 : _ReadWaveStart;

            }
            var ans = ExportHdFuncs.TryTakeSourceData(_WaveChnl, readInfo, out var wavedata, null);
            if (!ans)
            {
                return false;
            }
            else
            {
                // 预分配足够的空间，避免 List<byte> 动态扩容
                var bytecount = (_WaveFormat == WaveFormat.BYTe) ? wavedata.Count : wavedata.Count * 2;
                var sendbuffer = new List<Byte>(bytecount);

                foreach (var adcValue in wavedata)
                {
                    if (_WaveFormat == WaveFormat.BYTe)
                    {
                        // BYTE 格式：单字节数据，仅低 8 位有效
                        sendbuffer.Add((Byte)(adcValue & 0xFF));
                    }
                    else if (_WaveFormat == WaveFormat.WORD)
                    {
                        // DWORD 格式：双字节数据
                        sendbuffer.Add((Byte)(adcValue & 0xFF));       // 低字节
                        sendbuffer.Add((Byte)((adcValue >> 8) & 0xFF)); // 高字节
                    }
                }

                sendMessage.IsDataBlock = true;
                sendMessage.SendData = sendbuffer.ToArray();
                return true;
            }
            #endregion
        }

        /// <summary>
        /// 读取屏幕显示数据 一次性读完
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_WavNormalData(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            #region 准备
            var chnlId = _WaveChnl;
            var channel = allChnls.FirstOrDefault(chnl => chnl.Id == chnlId);
            if (channel == null)
            {
                return false;
            }
            WfmPack? wfmPack;
            if (channel is AnalogPrsnt analogPrsnt)
            {
                wfmPack = analogPrsnt.Pack;
                //wfmPack = analogPrsnt.VuDatabase.Current.Buffer;
            }
            else if (channel is MathPrsnt mathPrsnt)
            {
                wfmPack = mathPrsnt.Pack;
            }
            else
            {
                return false;
            }

            //mathPrsnt.Pack可能为null
            if (wfmPack == null)
            {
                sendMessage.IsDataBlock = true;
                sendMessage.SendData = new Byte[0];
                return true;
            }
            var wavePoints = _ReadWavePoints > wfmPack.Length ? wfmPack.Length : _ReadWavePoints;
            //时基
            var scaleByus = Presenter.Timebase.ScaleByus;
            //位移
            var postion = Presenter.Timebase.PositionByus;

            //采样率
            var sampling = 1 / wfmPack.Properties.SampInterval; // 22/2/28 去除除以1000，此前实测值比实际值大1000倍

            //起始时间 单位s
            var leftTime = (postion - scaleByus * 5) / 1000000;

            CheckWaveFormat();
            var format = _WaveFormat;

            #endregion 准备

            #region 数据处理
            if (format == WaveFormat.BYTe || format == WaveFormat.WORD)
            {
                // 获取 ADC 数据
                if (!Presenter.ChannelAdcDatas.TryGetValue(chnlId, out var data))
                {
                    return false; // 如果获取数据失败，直接返回
                }

                // 计算读取的起始位置和长度
                var bufferLength = data.GetLength(1);
                _ReadWaveStart = Math.Min(_ReadWaveStart, bufferLength - 1);
                wavePoints = Math.Min(wavePoints, bufferLength - _ReadWaveStart);

                // 根据格式确定每个波形点的字节数
                var bytespersample = (format == WaveFormat.BYTe) ? 1 : 2;

                // 创建 sendbuffer
                var sendbuffer = new byte[wavePoints * bytespersample];

                // 复制数据到 sendbuffer
                for (var i = 0; i < wavePoints; i++)
                {
                    UInt16 adcValue = data[0, _ReadWaveStart + i]; // 获取 ADC 值

                    if (format == WaveFormat.BYTe)
                    {
                        // BYTE 格式：单字节数据，仅低 8 位有效
                        sendbuffer[i] = (byte)(adcValue & 0xFF);
                    }
                    else if (format == WaveFormat.WORD)
                    {
                        // DWORD 格式：双字节数据，低 8 位有效，高 8 位为 0（8 位 ADC）
                        // 或者低 12 位有效，高 4 位为 0（12 位 ADC）
                        sendbuffer[2 * i] = (byte)(adcValue & 0xFF);       // 低字节
                        sendbuffer[2 * i + 1] = (byte)((adcValue >> 8) & 0xFF); // 高字节
                    }
                }

                // 设置发送消息并返回
                sendMessage.IsDataBlock = true;
                sendMessage.SendData = sendbuffer;
                return true;
            }
            else if (format == WaveFormat.ASCII)
            {
                // 定义缩放比例
                var scale = 1000.0;

                // 计算读取的起始位置和长度
                var bufferLength = wfmPack.Length;
                _ReadWaveStart = Math.Min(_ReadWaveStart, bufferLength - 1);
                wavePoints = Math.Min(wavePoints, bufferLength - _ReadWaveStart);

                // 创建 sendbuffer 并复制数据
                var sendbuffer = new Double[wavePoints];
                for (var i = 0; i < wavePoints; i++)
                {
                    sendbuffer[i] = wfmPack.Buffer[0, _ReadWaveStart + i] / scale; // 缩放数据
                }

                // 将数据转换为 ASCII 字符串
                var result = String.Join(",", sendbuffer.Select(x => x.ToString("E5", CultureInfo.InvariantCulture)));

                // 设置发送消息并返回
                sendMessage.IsDataBlock = true;
                sendMessage.SendData = Encoding.UTF8.GetBytes(result);
                return true;
            }

            #endregion  数据处理

            return false;
        }
        /// <summary>
        /// 读取波形数据<br>返回格式与当前选择的波形数据返回格式有关<br><br> BYTE 格式： 读取的数据格式为 头+采样率+开始时间(s)+波形数据点。 头为#NXXXXXX 的形式， #为规定的头标志符， N 表示后面含有 N 个字节，以 ASCII 字符的形式描述波形数据点的长度，结束符用于表示通讯的终止。例如，一次读取的数据为#9000001000XXXX 表示 9 个字节描述数据的长度， 000001000 表示波形数据的长度，即1000 字节。 <br><br>ASCii 格式：采样率+开始时间(s)+点数(整型)+直接以科学计数形式返回波形中每一点的实际电压值， 各值之间以“,”        隔开
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_SourceWavData(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            //参数格式：channelid(from 1->4),points,starttime by second,frameno(from 1..)
            string paramStr = ConvertParamToString(analyResult);
            string[] paramList = paramStr.Split(',');
            if (paramList.Length < 3)
                return false;
            if (!Int32.TryParse(paramList[0], out Int32 intChannelID))
                return false;
            if ((intChannelID - 1) < 0 || intChannelID > ChannelIdExt.AnaChnlNum)
                return false;
            ChannelId channelId = (ChannelId)(intChannelID - 1);
            if (!Int32.TryParse(paramList[1], out Int32 points))
                return false;
            if (!Int32.TryParse(paramList[2], out Int32 startDotIndex))
                return false;
            Int32 frameIndex = 0;
            if (paramList.Length > 3)
            {
                if (Int32.TryParse(paramList[3], out Int32 _frameIndex))
                    frameIndex = _frameIndex;
            }
            WfmSampleInfo wfmSampleInfo = new WfmSampleInfo();
            if (!ExportHdFuncs.TryReadSourceData(channelId, points, startDotIndex, frameIndex, out ushort[] waveData, wfmSampleInfo, null))
                return false;
            byte[] sendData = new byte[waveData.Length * 2];
            for (int i = 0; i < waveData.Length; i++)
                Array.Copy(BitConverter.GetBytes(waveData[i]), 0, sendData, i * 2, 2);

            sendMessage.IsDataBlock = true;
            sendMessage.SendData = sendData;
            return true;
        }

        private static void CheckWaveFormat()
        {
            if (!_WaveChnl.IsAnalog())
            {
                _WaveFormat = WaveFormat.ASCII;
            }

            if (_WaveMode == WaveMode.Raw)
            {
                if (_WaveFormat == WaveFormat.ASCII)
                {
                    _WaveFormat = WaveFormat.BYTe;
                }
                if (Constants.ADC_BITS > 9)
                {
                    _WaveFormat = WaveFormat.WORD;
                }
            }
        }

        private static string ToShortScientNum(double num)
        {
            return num.ToString("0.00000E+000");
        }
    }
}
//================= 共7个方法 =
