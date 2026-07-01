using ScopeX.ComModel;
using ScopeX.Core.Hardware;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ScopeX.Core
{
    public class SaveDataSoure
    {
        private readonly DsoPrsnt _Oscilloscope;
        public SaveDataSoure(DsoPrsnt dso)
        {
            _Oscilloscope = dso;
            var segDotsLength = 0LU;
            Hd.TryGetData(ChannelType.Analog, $"{AnalogParamEnum.SaveDataSegementDotsLength}_", out Object? data);
            if (data != null && data is UInt64)
                segDotsLength = (UInt64)data;
            _SegementLength = (Int64)segDotsLength!;
        }

        private Int64 _SegementLength = 0L;
        private Int64 GetSegementLength() => _SegementLength > TotalDots ? TotalDots : _SegementLength;


        private Int64 TotalDots => _Oscilloscope.Timebase.AcqedStorageWaveDotsCnt;

        public Int32 GetTotalSegementCount()
        {
            var length = _SegementLength > TotalDots ? TotalDots : _SegementLength;
            return (Int32)(TotalDots / length) * DataSources.Count + 1;
        }

        private Int32 _CompletedSegementCount = 0;
        public Int32 CompletedSegementCount
        {
            get => _CompletedSegementCount;
            private set
            {
                Volatile.Write(ref _CompletedSegementCount, value);
            }
        }

        public List<ChannelId> DataSources { get; set; } = new List<ChannelId>() { ChannelId.C1 };

        public void ClearCompletedSegementCount() => CompletedSegementCount = 0;

        public String SaveFileName
        {
            get
            {
                var appenddatetime = _Oscilloscope.File.IfAppendDatetime ? FilePrsnt.GetDateTimeString() : "";
                var fullname = _Oscilloscope.File.WfmPath + "\\" + _Oscilloscope.File.LongStroageFileName + appenddatetime + "." + _Oscilloscope.File.LongWfmFormat.GetAlias();

                return fullname;
            }
        }

        public Boolean IsPrepareSuccess()
        {
            //因文本格式下文件较大，故只检测在最大存储深度下是否支持文本格式
            if (_Oscilloscope.Timebase.StorageWaveDotsCnt > 501_000_000)//500M
            {
                if (!CheckIsSupportTxtFormat(_Oscilloscope.File.WfmPath))
                {
                    _Oscilloscope.File.LongWfmFormat = WfmFormat.Binary;
                }
            }

            //再检测磁盘剩余空间是否足够
            //根据存储深度大致估算大小
            var needspace = CalculateFileSize(TotalDots * DataSources.Count, _Oscilloscope.File.LongWfmFormat.GetAlias());
            var sufficient = IsDiskSpaceSufficient(SaveFileName, needspace);

            return sufficient;
        }


        public Int32 SaveScreenData(ChannelId source, Int64 segmentStartIndex, Int64 SumTime, CancellationToken? token)
        {
            if (!IsPrepareSuccess())
            {
                CompletedSegementCount = GetTotalSegementCount();
                return 0;
            }
            var res = 0;
            DsoPrsnt.KeyBoardLockEnable = true;
            Dispatcher.Cancel();
            UpdateVuTask.Cancel();

            TriggerModel.State = SysState.Stop;
            if (HdMsgFactory.TryMake(HdCmdFactory.Command, out var msg))
            {
                Hd.Execute(msg!);
                Thread.Sleep(100);
            }

            try
            {
                var appenddatetime = _Oscilloscope.File.IfAppendDatetime ? FilePrsnt.GetDateTimeString() : "";
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string folderPath = Path.Combine(documentsPath, @"U2 DSO\WaveForm");
                var fullname = folderPath + appenddatetime + $"_{source}" + ".bin";
                res = TrySaveScreenDataSourec(source, segmentStartIndex, SumTime, GetSegementLength(), fullname, "bin", token);
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
            }
            finally
            {
                DsoPrsnt.KeyBoardLockEnable = false;
            }
            return res;
        }
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="token"></param>
        /// <returns>-1 --> 未知错误，0 -- > 保存失败，1 -- > 保存成功，2 --> 保存超时</returns>
        public Int32 Run(CancellationToken? token)
        {
            if (!IsPrepareSuccess())
            {
                CompletedSegementCount = GetTotalSegementCount();
                return 0;
            }

            var res = 0;
            DsoPrsnt.KeyBoardLockEnable = true;
            //Dispatcher.Cancel();
            ////清除数据
            //Dispatcher.SoftReset();
            //Dispatcher.DoClear();
            //UpdateVuTask.Cancel();

            //清除数据
            //Dispatcher.SoftReset();
            //Dispatcher.DoClear();
            //Thread.Sleep(100);
            Dispatcher.Cancel();
            UpdateVuTask.Cancel();

            TriggerModel.State = SysState.Stop;
            if (HdMsgFactory.TryMake(HdCmdFactory.Command, out var msg))
            {
                Hd.Execute(msg!);
                Thread.Sleep(100);
            }
            try
            {
                foreach (var source in DataSources)
                {
                    var appenddatetime = _Oscilloscope.File.IfAppendDatetime ? FilePrsnt.GetDateTimeString() : "";
                    var fullname = _Oscilloscope.File.WfmPath + "\\" + _Oscilloscope.File.LongStroageFileName + appenddatetime + $"_{source}" + "." + _Oscilloscope.File.LongWfmFormat.GetAlias();
                    res = TrySaveDdrDataSourec(source, GetSegementLength(), fullname, _Oscilloscope.File.LongWfmFormat.GetAlias(), token);
                }
                if (res == 1)
                {
                    _Oscilloscope.File.LongStroageFileName = MakeDefaultFileName(_Oscilloscope.File.WfmPath, FilePrsnt.GetWfmFileExtName(_Oscilloscope.File.LongWfmFormat), $"_C");//自动增加
                }
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
            }
            finally
            {
                CompletedSegementCount = GetTotalSegementCount();
                DsoPrsnt.KeyBoardLockEnable = false;
            }

            try
            {
                Dispatcher.Run();
                UpdateVuTask.Run();
                DsoPrsnt.KeyBoardLockEnable = false;//键盘锁
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(e, EventBus.LogLevel.Error));
            }
            finally
            {
                DsoPrsnt.KeyBoardLockEnable = false;//键盘锁
            }

            return res;
        }

        public String MakeDefaultFileName(String path, String ext, String middleName = "")
        {
            var result = new DirectoryInfo(path)
                .GetFiles($"*{ext}", SearchOption.TopDirectoryOnly)
                .Where(x => Regex.IsMatch(x.Name, $"^{_Oscilloscope.File.DefaultPrefixName}[0-9]{"{3}"}{middleName}[0-3]{ext}$", RegexOptions.IgnoreCase));
            var filename = $"{_Oscilloscope.File.DefaultPrefixName}{result.Count():D3}";
            var filepath = Path.Combine(path, $"{filename}{ext}");
            Int32 index = 0;
            while (File.Exists(filepath))
            {
                filename = $"{_Oscilloscope.File.DefaultPrefixName}{result.Count() + index:D3}";
                index++;
                filepath = Path.Combine(path, $"{filename}{ext}");
            }
            filename = $"{_Oscilloscope.File.DefaultPrefixName}{result.Count() + index:D3}";
            return filename;
        }


        //private Boolean TryTakeSegmentWave(ChannelId channel, String fileName, Int32 segmentStartIndex, Int32 segmentCnt, CancellationToken? softResetToken, Boolean b4SourceData = false)
        //{
        //    var res = false;
        //    if (File.Exists(fileName))
        //    {
        //        File.Delete(fileName);
        //    }
        //    var mask = nameof(DataRole.SourceData);
        //    var start = segmentStartIndex;//读起始时间（点数）
        //    WfmPkgInfo? wfmpkg = null;
        //    FileStream? fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);
        //    UInt16[,] waveData = null;
        //    WfmSampleInfo wfmSampleInfo = new();
        //    Double SecondByps = 0;
        //    do
        //    {
        //        wfmpkg = new WfmPkgInfo(segmentCnt, Double.MaxValue, start);
        //        var readinfo = new ReadInfo(AcqDataType.AnalogChannel, new() { channel }, wfmpkg, mask);
        //        res = Hd.AnalogChannel.TryTakeSegmentWave(channel, readinfo, segmentStartIndex, segmentCnt, out waveData, out wfmSampleInfo, out SecondByps, softResetToken, b4SourceData);
        //        if (!res)
        //        {
        //            try
        //            {
        //                fs?.Close();
        //            }
        //            catch (Exception e)
        //            {
        //                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($" save data：{e.Message}", EventBus.LogLevel.Debug));
        //            }
        //            //CompletedSegementCount = GetTotalSegementCount();
        //            return res;
        //        }
        //    } while (!(softResetToken?.IsCancellationRequested ?? false));
        //    return res;
        //}
        private Int32 TrySaveScreenDataSourec(ChannelId channel, Int64 startIndex, Int64 sumTime, Int64 SegementLength, String fileName, String format, CancellationToken? softResetToken = null)
        {
            var ans = 0;
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            var mask = nameof(DataRole.SourceData);
            var start = startIndex;//读起始时间（点数）
            WfmPkgInfo? wfmpkg = null;
            FileStream? fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);

            do
            {
                wfmpkg = new WfmPkgInfo(SegementLength, sumTime, start);
                var readinfo = new ReadInfo(AcqDataType.AnalogChannel, new() { channel }, wfmpkg, mask);


                ans = Hd.AnalogChannel!.TrySaveSourceWave(channel, readinfo!, fs, format, softResetToken);
                if (ans == 0)
                {
                    try
                    {
                        fs?.Close();
                    }
                    catch (Exception e)
                    {
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"LongStorage save data：{e.Message}", EventBus.LogLevel.Debug));
                    }
                    //CompletedSegementCount = GetTotalSegementCount();
                    return 0;
                }
                else if (ans == -1)
                {
                    try
                    {
                        fs?.Close();
                    }
                    catch (Exception e)
                    {
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"LongStorage save data：{e.Message}", EventBus.LogLevel.Debug));
                    }
                    //CompletedSegementCount = GetTotalSegementCount();
                    return -1;
                }
                else//ans==1
                {
                    fs.Flush();
                    start += SegementLength;
                    CompletedSegementCount++;
                }
                Thread.Sleep(200);
            }
            while (start < TotalDots && (!(softResetToken?.IsCancellationRequested ?? false)));

            try
            {
                fs?.Close();
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"LongStorage save data：{e.Message}", EventBus.LogLevel.Debug));
            }

            //CompletedSegementCount = GetTotalSegementCount();
            return softResetToken?.IsCancellationRequested ?? false ? 2 : ans;
        }

        /// <summary>
        /// 读取深存储数据
        /// </summary>
        /// <param name="channel">通道</param>
        /// <param name="fileName">文件名</param>
        /// <param name="softResetToken"></param>
        /// <returns>-1 --> 未知错误，0 -- > 保存失败，1 -- > 保存成功，2 --> 保存超时</returns>
        private Int32 TrySaveDdrDataSourec(ChannelId channel, Int64 SegementLength, String fileName, String format, CancellationToken? softResetToken = null)
        {
            var ans = 0;
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            var mask = nameof(DataRole.SourceData);
            var start = 0L;//读起始时间（点数）
            WfmPkgInfo? wfmpkg = null;
            FileStream? fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);

            do
            {
                wfmpkg = new WfmPkgInfo(SegementLength, Double.MaxValue, start);
                var readinfo = new ReadInfo(AcqDataType.AnalogChannel, new() { channel }, wfmpkg, mask);

                ans = Hd.AnalogChannel!.TrySaveSourceWave(channel, readinfo!, fs, format, softResetToken);
                if (ans == 0)
                {
                    try
                    {
                        fs?.Close();
                    }
                    catch (Exception e)
                    {
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"LongStorage save data：{e.Message}", EventBus.LogLevel.Debug));
                    }
                    //CompletedSegementCount = GetTotalSegementCount();
                    return 0;
                }
                else if (ans == -1)
                {
                    try
                    {
                        fs?.Close();
                    }
                    catch (Exception e)
                    {
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"LongStorage save data：{e.Message}", EventBus.LogLevel.Debug));
                    }
                    //CompletedSegementCount = GetTotalSegementCount();
                    return -1;
                }
                else//ans==1
                {
                    fs.Flush();
                    start += SegementLength;
                    CompletedSegementCount++;
                }
                Thread.Sleep(200);
            }
            while (start < TotalDots && (!(softResetToken?.IsCancellationRequested ?? false)));

            try
            {
                fs?.Close();
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"LongStorage save data：{e.Message}", EventBus.LogLevel.Debug));
            }

            //CompletedSegementCount = GetTotalSegementCount();
            return softResetToken?.IsCancellationRequested ?? false ? 2 : ans;
        }

        /// <summary>
        /// 判断是否支持文本格式
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True：支持，false：不支持</returns>
        public Boolean CheckIsSupportTxtFormat(String path)
        {
            var root = Path.GetPathRoot(path);
            if (root == null)
                return false;

            var driveinfo = new DriveInfo(root);
            if (driveinfo == null)
                return false;

            //文件系统和最大文件大小：
            //FAT32：最大单个文件大小为4GB。
            //exFAT：理论上最大单个文件大小可以非常大（16EB），但实际大小可能受到操作系统或U盘制造商的限制，通常可以处理大于4GB的文件。
            //NTFS：最大单个文件大小为16TB，但U盘通常不使用NTFS文件系统。
            var isallowtxt = true;
            switch (driveinfo.DriveFormat.ToUpper())
            {
                case "FAT32":
                    isallowtxt = false;
                    break;
                case "EXFAT":
                // exFAT理论上可以支持非常大的文件，但这里我们不设置具体限制
                case "NTFS"://16TB
                default:
                    isallowtxt = true;
                    break;
            }
            return isallowtxt;
        }

        /// <summary>
        /// 计算文件最终占用磁盘空间大小
        /// </summary>
        /// <param name="fileSizeBytes">实际字节数</param>
        /// <param name="forMat">保存格式</param>
        /// <param name="separator">当保存格式为文本时的分割符，这里默认分隔符为“,\r\n”，因此Dirver在保存时也必须保存一致，否则将导致计算不准确</param>
        /// <returns></returns>
        private Double CalculateFileSize(Int64 fileSizeBytes, String forMat, String separatorByTxt = ",\r\n")
        {
            //簇大小：4KB（4,096字节）
            var clustersizebytes = 4096;
            // 计算占用簇的数量
            var numclusters = 0L;
            // 计算实际占用磁盘空间
            var actualdiskspace = 0D;
            //添加分割符后的字节数
            var filesizebyteswithseparator = 0L;
            switch (forMat)
            {
                case "txt":
                    //1、字节值转换成文本表示：
                    //每个字节转换成 0 到 255 的数字，最大长度为 3 个字符（例如，255）。
                    //加上分隔符 ",\r\n"，分隔符的长度为 3 个字符。
                    //计算单个字节的文本长度：

                    //2、一个字节最大可能被转换成 3 个字符（例如 255）。
                    //加上分隔符，总共 6 个字符（3 + 3）。

                    //3、计算总长度：
                    //例如对于 25000 个字节，每个字节会被转换成 6 个字符（包括分隔符）。
                    //最后一个字节后不需要分隔符，所以实际字符数为(25000 * 6) - 3。

                    // Assuming UTF-8 encoding
                    Encoding encoding = Encoding.Default;
                    // Calculate size of separator in bytes
                    var separatorSize = encoding.GetByteCount(separatorByTxt);

                    //由于保存数据时是分多次读取
                    var count = fileSizeBytes / GetSegementLength();

                    filesizebyteswithseparator = fileSizeBytes * (3 + separatorSize);

                    break;
                case "bin":
                default:
                    filesizebyteswithseparator = fileSizeBytes;
                    break;
            }
            numclusters = (Int64)Math.Ceiling((Double)filesizebyteswithseparator / clustersizebytes);
            actualdiskspace = (Double)(numclusters * clustersizebytes) / (1 * 1024 * 1024);//MB
            return actualdiskspace + 100D;//实际预留100MB的空间
        }

        /// <summary>
        /// 检查磁盘空间是否足够
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="requiredFreeSpace">需要空间</param>
        /// <returns></returns>
        private Boolean IsDiskSpaceSufficient(String filePath, Double requiredFreeSpace)
        {
            try
            {
                // 获取文件所在磁盘根目录的路径
                var driveroot = Path.GetPathRoot(filePath);

                if (driveroot == null)
                {
                    return true;
                }

                // 获取磁盘信息
                DriveInfo driveInfo = new DriveInfo(driveroot);

                // 检查磁盘剩余空间是否足够
                if (driveInfo.AvailableFreeSpace / 1024D / 1024D >= requiredFreeSpace)
                {
                    return true;
                }
                else
                {
                    WeakTip.Default.Write("Save Waveform", MsgTipId.NoMoreDiskSpace);
                    return false;
                }
            }
            catch (Exception e)
            {
                WeakTip.Default.Write("Save Waveform", MsgTipId.CheckDiskSpaceError);
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Debug));
                return false;
            }
        }
    }
}
