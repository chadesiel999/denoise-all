// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/20</date>

namespace ScopeX.Core
{
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using ScopeX.ComModel;
    using ScopeX.Controls.Common.Helper;
    using ScopeX.Core.Tools;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;


    //public enum SaveStatus
    //{
    //    Success,
    //    Fail,
    //    Cancel,
    //}
    public enum WfmFormat
    {
        [Alias("bin")]
        Binary,

        [Alias("txt")]
        Text,

        [Alias("mat")]
        Matlab,

        [Alias("xlsx")]
        Excel,

        [Alias("csv")]
        CSV,

        [Alias("tsv")]
        TSV,

        [Alias("dat")]
        DAT,

        [Alias("bsv")]
        BSV,
        //[Alias("wfm")]
        //WFM,

        //[Alias("h5")]
        //HDF5,
    }

    public enum PicFormat
    {
        Bmp,

        Tiff,

        Gif,

        Png,

        Jpeg,
    }

    public enum PicArea
    {
        FullScreen = 0,

        Window = 1,

        Application = 2
    }

    public enum PicColor
    {
        Standard = 0,
        BlackWhite = 1,
        Reverse = 2
    }

    public enum TxtFormat
    {
        ASCII,
        GB2312,
        UTF8,
        UTF32,
        Unicode,
    }
    public enum SaveStatus
    {
        Success,
        Fail,
        Cancel
    }

    public class FilePrsnt : MulticastPrsnt<IFileView>, IFilePrsnt
    {
        public FilePrsnt(IDsoPrsnt idp, IFileView? view, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            Model = mco switch
            {
                ModelCreateOptions.Dependant => DsoModel.Default.File,
                ModelCreateOptions.Standalone => new(),
                _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
            };
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public static Func<PicFormat, PicArea, PicColor, Boolean, MemoryStream>? GetImageStreamHandler { get; set; }

        public static Func<String, String, ChannelId, Boolean>? SaveLabNoteBookHandler { get; set; }

        public String DefaultPrefixName => Model.DefaultPrefixName;

        public String FileName { get => Model.FileName; set => Model.FileName = value; }

        public String LongStroageFileName { get => Model.LongStroageFileName; set => Model.LongStroageFileName = value; }

        public static Boolean IsTimestamp { get; set; } = true;
        public Boolean IfAppendDatetime { get => Model.IfAppendDatetime; set => Model.IfAppendDatetime = value; }

        public Boolean IsDefaultSetting { get => Model.IsDefaultSetting; set => Model.IsDefaultSetting = value; }

        public PicFormat PicFormat { get => Model.PicFormat; set => Model.PicFormat = value; }

        public String PicPath
        {
            get => Model.PicPath;
            set
            {
                Model.PicPath = value;
                FileName = MakeDefaultFileName(PicPath, FilePrsnt.GetPicFileExtName(PicFormat));

            }
        }

        public PicArea PicRegion { get => Model.PicRegion; set => Model.PicRegion = value; }

        public String SettingLoadFullPath { get => Model.SettingLoadFullPath; set => Model.SettingLoadFullPath = value; }

        public String SettingSavePath
        {
            get => Model.SettingSavePath;
            set
            {
                Model.SettingSavePath = value;
                FileName = MakeDefaultFileName(SettingSavePath, ".set");
            }
        }

        public WfmFormat LongWfmFormat
        {
            get => Model.LongWfmFormat;
            set => Model.LongWfmFormat = value;
        }

        public WfmFormat WfmFormat
        {
            get => Model.WfmFormat;
            set
            {
                Model.WfmFormat = value;
                if (value == WfmFormat.Binary && !Model.WfmSource.IsAnalog())
                {
                    Model.WfmSource = ChannelId.C1;
                }
            }
        }

        public String WfmPath
        {
            get => Model.WfmPath;
            set
            {
                Model.WfmPath = value;
                FileName = MakeDefaultFileName(WfmPath, FilePrsnt.GetWfmFileExtName(WfmFormat));
            }
        }

        public ChannelId WfmSource
        {
            get => Model.WfmSource;
            set => Model.WfmSource = value;
        }

        public PicColor PicColor { get => Model.PicColor; set => Model.PicColor = value; }

        public TxtFormat WfmTxtFormat { get => Model.WfmTxtFormat; set => Model.WfmTxtFormat = value; }

        private protected override FileModel Model { get; }

        public static String GetDateTimeString()
        {
            return "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        public static String? GetImageBase64String(Boolean needCloseWeakTip = false)
        {
            var ms = GetImageStreamHandler?.Invoke(PicFormat.Jpeg, PicArea.Application, PicColor.Standard, needCloseWeakTip);
            if (ms is not null)
            {
                try
                {
                    var b64string = Convert.ToBase64String(ms.ToArray());
                    ms.Close();
                    return b64string;
                }
                catch (Exception e)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs("Failed to convert image to Base64 String: " + e.ToString(), EventBus.LogLevel.Error));
                }
            }
            return null;
        }

        public Byte[]? GetImageByteArray(Boolean needCloseWeakTip = false)
        {
            var ms = GetImageStreamHandler?.Invoke(PicFormat, PicRegion, PicColor, needCloseWeakTip);
            if (ms is not null)
            {
                try
                {
                    var bytearray = ms.ToArray();
                    ms.Close();
                    return bytearray;
                }
                catch (Exception e)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs("Failed to convert image to Byte array: " + e.ToString(), EventBus.LogLevel.Error));
                }
            }
            return null;
        }

        public static Encoding GetEncoding(TxtFormat format)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                return format switch
                {
                    TxtFormat.ASCII => Encoding.ASCII,
                    TxtFormat.UTF8 => Encoding.UTF8,
                    TxtFormat.UTF32 => Encoding.UTF32,
                    TxtFormat.GB2312 => Encoding.GetEncoding("GB2312"),
                    TxtFormat.Unicode => Encoding.Unicode,
                    _ => Encoding.Default
                };
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
                return Encoding.Default;
            }
        }

        public static String GetWfmFileExtName(WfmFormat wf)
        {
            return wf switch
            {
                WfmFormat.Binary => ".bin",
                WfmFormat.Text => ".txt",
                WfmFormat.Matlab => ".mat",
                WfmFormat.Excel => ".xlsx",
                WfmFormat.CSV => ".csv",
                WfmFormat.TSV => ".tsv",
                WfmFormat.DAT => ".dat",
                WfmFormat.BSV => ".bsv",
                _ => ".bin"
            };
        }

        public static String GetPicFileExtName(PicFormat pf)
        {
            return pf switch
            {
                PicFormat.Bmp => ".bmp",
                PicFormat.Gif => ".gif",
                PicFormat.Jpeg => ".jpeg",
                PicFormat.Png => ".png",
                PicFormat.Tiff => ".tiff",
                _ => ".dat"
            };
        }

        //public static String[] FileInfo = new String[]
        //{
        //    "Model",
        //    "Channel",
        //    "Horizontal Units",
        //    "Vertical Units",
        //    "Sample Interval",
        //    "Horizontal Scale",
        //    "Horizontal Position",
        //    "Vertical Scale",
        //    "Vertical Position",
        //    "Record Length"
        //};

        public static String[] FileInfo = new String[]
        {
            "Channel",
            "Time Base",
            "Amplitude",
            "Sample Rate",
            "Record Length",
            "Horizontal Scale",
            "Horizontal Position",
            "Vertical Scale",
            "Vertical Position",
            "Sample Interval",
            "Probe Ratio",
        };

        public static Boolean DeleteFile(String fullfilename)
        {
            try
            {
                File.Delete(fullfilename);
            }
            catch
            {
                return false;
            }

            return true;
        }

        #region Setting
        public static Boolean LoadDefSetting()
        {
            //Load Factory *.set
            return LoadSetting(Constants.SET_DEF_PATH + "\\" + Constants.FACTORY_SET_NAME + ".set");
        }

        public static Boolean LoadSetting(String fullpath, Boolean isCallingByVu = false)
        {
            try
            {
                if (!File.Exists(fullpath))
                {
                    WeakTip.Default.Write(nameof(LoadSetting), MsgTipId.ReadingFailed);
                    return false;
                }

                using MemoryStream memorystream = new(File.ReadAllBytes(fullpath));
                var setting = BinaryConvert.Deserialize<SysSettings>(memorystream);
                if (setting is null)
                {
                    return false;
                }

                setting.OnDeserialized();
                if (isCallingByVu)
                {
                    setting.LoadFunction();
                }
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
#if DEBUG
                throw;
#else                
                return false;
#endif
            }

            return true;
        }

        public static Boolean SaveSetting(String fullpath)
        {
            var path = Path.GetDirectoryName(fullpath);
            var file = Path.GetFileName(fullpath);

            if (String.IsNullOrEmpty(file))
            {
                return false;
            }

            if (path?.Length == 0)
            {
                path = Directory.GetCurrentDirectory();
            }

            return SaveSetting(path!, file, false, true);
        }

        public static Int32 SaveSettingEx(String path, String name, Boolean postfix, Boolean dumb = false)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    if (!Directory.Exists(path))
                    {
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Path does not exist: '{path}'", EventBus.LogLevel.Error));
                        return 0;
                    }
                }

                String fullfilename = path + "\\" + name + (postfix ? GetDateTimeString() : "") + ".set";
                if (File.Exists(fullfilename))
                {
                    if (!dumb && !StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.FileExisted, MessageType.Warning))
                    {
                        return 2;
                    }
                    if (!DeleteFile(fullfilename))
                    {
                        WeakTip.Default.Write("File", MsgTipId.FileOccupied);
                        return 0;
                    }
                }

                SysSettings settings = new();
                settings.OnSerializing();

                using var fs = new FileStream(fullfilename, FileMode.Create, FileAccess.Write);
                BinaryConvert.Serialize(settings, fs);
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
#if DEBUG
                throw;
#else               
                return false;
#endif
            }

            return 1;
        }

        public static Boolean SaveSetting(String path, String name, Boolean postfix, Boolean dumb = false)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    if (!Directory.Exists(path))
                    {
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Path does not exist: '{path}'", EventBus.LogLevel.Error));
                        return false;
                    }
                }

                String fullfilename = path + "\\" + name + (postfix ? GetDateTimeString() : "") + ".set";
                if (File.Exists(fullfilename))
                {
                    if (!dumb && !StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.FileExisted, MessageType.Warning))
                    {
                        return false;
                    }
                    if (!DeleteFile(fullfilename))
                    {
                        WeakTip.Default.Write("File", MsgTipId.FileOccupied);
                        return false;
                    }
                }

                SysSettings settings = new();
                settings.OnSerializing();

                using var fs = new FileStream(fullfilename, FileMode.Create, FileAccess.Write);
                BinaryConvert.Serialize(settings, fs);
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
#if DEBUG
                throw;
#else               
                return false;
#endif
            }

            return true;
        }
        #endregion

        #region Waveform

        public static Boolean LoadWaveform(String fullpath, ChannelId id, DsoPrsnt dso)
        {
            ReferencePrsnt? rprsnt = null;
            if (ReferencePrsnt.TryRead(id, dso, fullpath, ref rprsnt))
            {
                //Add a new reference channel presenter
                dso.AddChannel(id, rprsnt!);

                DsoPrsnt.FocusId = rprsnt!.Id;
                return true;
            }
            return false;
        }

        public static Boolean SaveWaveform(String fullpath, WfmFormat wf, ChannelId id, TxtFormat wfmtxtfotmat = TxtFormat.UTF8, Boolean IsCheckSameName = true)
        {
            var path = Path.GetDirectoryName(fullpath);
            var file = Path.GetFileName(fullpath);

            if (String.IsNullOrEmpty(file))
            {
                return false;
            }

            if (path?.Length == 0)
            {
                path = Directory.GetCurrentDirectory();
            }

            return SaveWaveform(path!, file, wf, id, false, wfmtxtfotmat, IsCheckSameName);
        }

        public Boolean SaveWaveform(Boolean IsCheckSameName = true)
        {
            return SaveWaveform(WfmPath, FileName, WfmFormat, WfmSource, IfAppendDatetime, WfmTxtFormat, IsCheckSameName);
        }

        /// <summary>
        /// Save wavrform extension
        /// </summary>
        /// <param name="path">path</param>
        /// <param name="name">name</param>
        /// <param name="wf">wave format</param>
        /// <param name="id">source</param>
        /// <param name="postfix">postfix</param>
        /// <param name="wfmtxtfotmat"> wfm txt format</param>
        /// <param name="IsCheckSameName">check same name</param>
        /// <returns>0-->false,1-->true,2-->unsave</returns>
        public static Int32 SaveWaveformEx(String path, String name, WfmFormat wf, ChannelId id, Boolean postfix, TxtFormat wfmtxtfotmat = TxtFormat.UTF8, Boolean IsCheckSameName = true)
        {
            if (DsoModel.Default.TryGetChannel(id, out var cm) && cm.Pack is not null)
            {
                //if (cm.Pack.Properties.Version != "U2.0")
                //    return false;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    if (!Directory.Exists(path))
                    {
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Path does not exist: '{path}'", EventBus.LogLevel.Error));
                        return 0;
                    }
                }
                String appenddatetime = postfix ? GetDateTimeString() : "";
                String fullfilename = path + "\\" + name + appenddatetime + "." + wf.GetAlias();
                if (File.Exists(fullfilename))
                {
                    if (IsCheckSameName && !StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.FileExisted, MessageType.Warning))
                    {
                        return 2;
                    }
                    if (!DeleteFile(fullfilename))
                    {
                        WeakTip.Default.Write("File", MsgTipId.FileOccupied);
                        return 0;
                    }
                }
                DsoModel.Default.File.FileName = name + appenddatetime;
                using var fs = new FileStream(fullfilename, FileMode.OpenOrCreate, FileAccess.Write);
                switch (wf)
                {
                    case WfmFormat.Binary:
                        return FilePrsnt.SaveWaveByBin(fs, cm.Pack) ? 1 : 0;
                    case WfmFormat.Text:
                        return FilePrsnt.SaveWaveByTxt(fs, cm.Pack, wfmtxtfotmat) ? 1 : 0;
                    case WfmFormat.Matlab:
                        return FilePrsnt.SaveWaveByMatlab(fs, cm.Pack, fullfilename) ? 1 : 0;
                    case WfmFormat.Excel:
                        return FilePrsnt.SaveWaveByExcel(fs, cm.Pack) ? 1 : 0;
                    case WfmFormat.CSV:
                        return FilePrsnt.SaveWaveByCsv(fs, cm.Pack) ? 1 : 0;
                    case WfmFormat.TSV:
                        return FilePrsnt.SaveWaveByTsv(fs, cm.Pack) ? 1 : 0;
                    case WfmFormat.DAT:
                        return FilePrsnt.SaveWaveByRaw(fs, cm.Pack) ? 1 : 0;
                    //case WfmFormat.WFM:
                    //case WfmFormat.HDF5:
                    default:
                        WeakTip.Default.Write("WfmSave", MsgTipId.UnSupportedFormat);
                        return 0;
                }
            }
            return 0;
        }

        public static Int32 SaveAllWaveformByCsv(String path, String name, Boolean postfix, TxtFormat wfmtxtfotmat = TxtFormat.UTF8, Boolean IsCheckSameName = true, params ChannelId[] sources)
        {
            var wf = WfmFormat.CSV;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                if (!Directory.Exists(path))
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Path does not exist: '{path}'", EventBus.LogLevel.Error));
                    return 0;
                }
            }
            String appenddatetime = postfix ? GetDateTimeString() : "";
            String fullfilename = path + "\\" + name + appenddatetime + "." + wf.GetAlias();
            if (File.Exists(fullfilename))
            {
                if (IsCheckSameName && !StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.FileExisted, MessageType.Warning))
                {
                    return 2;
                }
                if (!DeleteFile(fullfilename))
                {
                    WeakTip.Default.Write("File", MsgTipId.FileOccupied);
                    return 0;
                }
            }
            var packs = new List<WfmPack>();
            foreach (var source in sources)
            {
                if (DsoModel.Default.TryGetChannel(source, out var cm) && cm.Active && cm.Pack is not null)
                {
                    packs.Add(cm.Pack);
                }
            }
            DsoModel.Default.File.FileName = name + appenddatetime;
            using var fs = new FileStream(fullfilename, FileMode.OpenOrCreate, FileAccess.Write);
            return FilePrsnt.SaveWaveByCsv(fs, packs.ToArray()) ? 1 : 0;
        }


        public static Boolean SaveWaveform(String path, String name, WfmFormat wf, ChannelId id, Boolean postfix, TxtFormat wfmtxtfotmat = TxtFormat.UTF8, Boolean IsCheckSameName = true)
        {
            if (DsoModel.Default.TryGetChannel(id, out var cm) && cm.Pack is not null)
            {
                //if (cm.Pack.Properties.Version != "U2.0")
                //    return false;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    if (!Directory.Exists(path))
                    {
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Path does not exist: '{path}'", EventBus.LogLevel.Error));
                        return false;
                    }
                }
                String appenddatetime = postfix ? GetDateTimeString() : "";
                String fullfilename = path + "\\" + name + appenddatetime + "." + wf.GetAlias();
                if (File.Exists(fullfilename))
                {
                    if (IsCheckSameName && !StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.FileExisted, MessageType.Warning))
                    {
                        return false;
                    }
                    if (!DeleteFile(fullfilename))
                    {
                        WeakTip.Default.Write("File", MsgTipId.FileOccupied);
                        return false;
                    }
                }
                DsoModel.Default.File.FileName = name + appenddatetime;
                using var fs = new FileStream(fullfilename, FileMode.OpenOrCreate, FileAccess.Write);
                switch (wf)
                {
                    case WfmFormat.Binary:
                        return FilePrsnt.SaveWaveByBin(fs, cm.Pack);
                    case WfmFormat.Text:
                        return FilePrsnt.SaveWaveByTxt(fs, cm.Pack, wfmtxtfotmat);
                    case WfmFormat.Matlab:
                        return FilePrsnt.SaveWaveByMatlab(fs, cm.Pack, fullfilename);
                    case WfmFormat.Excel:
                        return FilePrsnt.SaveWaveByExcel(fs, cm.Pack);
                    case WfmFormat.CSV:
                        return FilePrsnt.SaveWaveByCsv(fs, cm.Pack);
                    case WfmFormat.TSV:
                        return FilePrsnt.SaveWaveByTsv(fs, cm.Pack);
                    case WfmFormat.DAT:
                        return FilePrsnt.SaveWaveByRaw(fs, cm.Pack);
                    //case WfmFormat.WFM:
                    //case WfmFormat.HDF5:
                    default:
                        WeakTip.Default.Write("WfmSave", MsgTipId.UnSupportedFormat);
                        return false;
                }
            }
            return false;
        }

        public static Boolean SaveWaveByBin(Stream stm, WfmPack pkg)
        {
            try
            {
                BinaryConvert.Serialize(pkg, stm);
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
#if DEBUG
                throw;
#else
                
                return false;
#endif
            }

            return true;
        }
        public static Boolean SaveWaveByCSV(Stream stm, WfmPack pkg, Func<Double, Double[], String> cbfunc, TxtFormat wfmtxtformat = TxtFormat.UTF8)
        {
            try
            {
                using StreamWriter sw = new(stm, GetEncoding(wfmtxtformat));

                Int32 length = pkg.Buffer.GetLength(1);
                Int32 wfmcnt = pkg.Buffer.GetLength(0);

                Double sp = pkg.Properties.TmbScale.Value * Constants.VIS_XDIVS_NUM / length;

                Double pos0 = pkg.Properties.TmbPosition.Index / Constants.MAX_XPOS_IDX * pkg.Properties.TmbScale.Value * Constants.VIS_XDIVS_NUM;

                Double time;
                Double[] ampls = new Double[wfmcnt];
                String text = FileInfo[0] + $":{DsoModel.Default.File.WfmSource}";
                sw.WriteLine(text);
                text = FileInfo[1] + $":{SIHelper.ValueChangeToSI(pkg.Properties.TmbScale.Value / 1E6, 0, "s")}";
                sw.WriteLine(text);
                text = FileInfo[2] + $":{SIHelper.ValueChangeToSI(pkg.Properties.ChnlScale.Value / 1E3, 0, "V")}";
                sw.WriteLine(text);
                text = FileInfo[3] + $":{SIHelper.ValueChangeToSI(DsoModel.Default.Timebase.AnalogSamplingRate, 2, "Sa/s")}";
                sw.WriteLine(text);
                text = FileInfo[4] + $":{DsoModel.Default.Timebase.StorageWaveDotsCnt}";
                sw.WriteLine(text);
                text = FileInfo[5] + $":{pkg.Properties.TmbScale.Value}" + $":{pkg.Properties.TmbScale.Index}";
                sw.WriteLine(text);
                text = FileInfo[6] + $":{pkg.Properties.TmbPosition.Value}" + $":{pkg.Properties.TmbPosition.Index}";
                sw.WriteLine(text);
                text = FileInfo[7] + $":{pkg.Properties.ChnlScale.Value}" + $":{pkg.Properties.ChnlScale.Index}";
                sw.WriteLine(text);
                text = FileInfo[8] + $":{pkg.Properties.ChnlPosition.Value}" + $":{pkg.Properties.ChnlPosition.Index}";
                sw.WriteLine(text);
                text = FileInfo[9] + $":{pkg.Properties.SampInterval}";
                sw.WriteLine(text);
                text = FileInfo[10] + $":{pkg.Properties.ProbeInfo?.Gain ?? 1}" + $":{pkg.Properties.ProbeInfo?.UnitRatio ?? 1}";
                sw.WriteLine(text);
                String header;
                //!!!Patch: digital channel is different from analog channel
                if (!pkg.Properties.Name.Contains("D"))
                {
                    var headertmp = $"Y-axis({pkg.Properties.ChnlUnit.Prefix.ToPfxString()}{pkg.Properties.ChnlUnit.Name})";
                    header = headertmp;
                    for (Int32 i = 1; i < wfmcnt; i++)
                    {
                        header += ", " + headertmp;
                    }
                }
                else
                {
                    header = $"Y-axis({ChannelIdExt.MinDChId}...{ChannelIdExt.MaxDChId})";
                }
                header = $"X-axis({pkg.Properties.TmbUnit.Prefix.ToPfxString()}{pkg.Properties.TmbUnit.Name}), " + header;
                sw.WriteLine(header);
                for (Int32 j = 0; j < length; j++)
                {
                    time = (j * sp - pos0) + pkg.Properties.TrigErrorTime;
                    for (Int32 i = 0; i < wfmcnt; i++)
                    {
                        ampls[i] = pkg.Buffer[i, j];
                    }

                    sw.WriteLine(cbfunc(time, ampls));
                }
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
#if DEBUG
                throw;
#else                
                return false;
#endif
            }

            return true;
        }

        public static Boolean SaveWaveByCSV(Stream stm, Func<Double, Double[], String> cbfunc, TxtFormat wfmtxtformat = TxtFormat.UTF8, params WfmPack[] pkgs)
        {
            List<String> stringlist = new List<String>();

            foreach (var item in pkgs)
            {
                Int32 index = 0;

                WfmPack pkg = item;
                if (pkg is null)
                {
                    continue;
                }
                try
                {
                    Int32 length = pkg.Buffer.GetLength(1);
                    Int32 wfmcnt = pkg.Buffer.GetLength(0);

                    Double sp = pkg.Properties.TmbScale.Value * Constants.VIS_XDIVS_NUM / length;

                    Double pos0 = pkg.Properties.TmbPosition.Index / Constants.MAX_XPOS_IDX * pkg.Properties.TmbScale.Value * Constants.VIS_XDIVS_NUM;

                    Double time;
                    Double[] ampls = new Double[wfmcnt];
                    String text = FileInfo[0] + $":{pkg.Properties.Name}";
                    if (stringlist.Count < 1)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[0] = stringlist[0] + ",," + text;
                    }
                    text = FileInfo[1] + $":{SIHelper.ValueChangeToSI(pkg.Properties.TmbScale.Value / 1E6, 0, "s")}";
                    if (stringlist.Count < 2)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[1] = stringlist[1] + ",," + text;
                    }
                    text = FileInfo[2] + $":{SIHelper.ValueChangeToSI(pkg.Properties.ChnlScale.Value / 1E3, 0, "V")}";
                    if (stringlist.Count < 3)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[2] = stringlist[2] + ",," + text;
                    }
                    text = FileInfo[3] + $":{SIHelper.ValueChangeToSI(DsoModel.Default.Timebase.AnalogSamplingRate, 2, "Sa/s")}";
                    if (stringlist.Count < 4)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[3] = stringlist[3] + ",," + text;
                    }
                    text = FileInfo[4] + $":{DsoModel.Default.Timebase.StorageWaveDotsCnt}";
                    if (stringlist.Count < 5)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[4] = stringlist[4] + ",," + text;
                    }
                    text = FileInfo[5] + $":{pkg.Properties.TmbScale.Value}" + $":{pkg.Properties.TmbScale.Index}";
                    if (stringlist.Count < 6)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[5] = stringlist[5] + ",," + text;
                    }
                    text = FileInfo[6] + $":{pkg.Properties.TmbPosition.Value}" + $":{pkg.Properties.TmbPosition.Index}";
                    if (stringlist.Count < 7)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[6] = stringlist[6] + ",," + text;
                    }
                    text = FileInfo[7] + $":{pkg.Properties.ChnlScale.Value}" + $":{pkg.Properties.ChnlScale.Index}";
                    if (stringlist.Count < 8)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[7] = stringlist[7] + ",," + text;
                    }
                    text = FileInfo[8] + $":{pkg.Properties.ChnlPosition.Value}" + $":{pkg.Properties.ChnlPosition.Index}";
                    if (stringlist.Count < 9)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[8] = stringlist[8] + ",," + text;
                    }
                    text = FileInfo[9] + $":{pkg.Properties.SampInterval}";
                    if (stringlist.Count < 10)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[9] = stringlist[9] + ",," + text;
                    }
                    text = FileInfo[10] + $":{pkg.Properties.ProbeInfo?.Gain ?? 1}" + $":{pkg.Properties.ProbeInfo?.UnitRatio ?? 1}";
                    if (stringlist.Count < 11)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[10] = stringlist[10] + ",," + text;
                    }

                    String header;
                    //!!!Patch: digital channel is different from analog channel
                    if (!pkg.Properties.Name.Contains("D"))
                    {
                        var headertmp = $"Y-axis({pkg.Properties.ChnlUnit.Prefix.ToPfxString()}{pkg.Properties.ChnlUnit.Name})";
                        header = headertmp;
                        for (Int32 i = 1; i < wfmcnt; i++)
                        {
                            header += ", " + headertmp;
                        }
                    }
                    else
                    {
                        header = $"Y-axis({ChannelIdExt.MinDChId}...{ChannelIdExt.MaxDChId})";
                    }
                    header = $"X-axis({pkg.Properties.TmbUnit.Prefix.ToPfxString()}{pkg.Properties.TmbUnit.Name}), " + header;
                    if (stringlist.Count < 12)
                    {
                        stringlist.Add(header);
                    }
                    else
                    {
                        stringlist[11] = stringlist[11] + "," + header;
                    }
                    index = 12;
                    for (Int32 j = 0; j < length; j++)
                    {
                        time = (j * sp - pos0) + pkg.Properties.TrigErrorTime;
                        for (Int32 i = 0; i < wfmcnt; i++)
                        {
                            ampls[i] = pkg.Buffer[i, j];
                        }
                        if (stringlist.Count < index + 1)
                        {
                            stringlist.Add(cbfunc(time, ampls));
                        }
                        else
                        {
                            stringlist[index] = stringlist[index] + "," + cbfunc(time, ampls);
                        }
                        index++;
                    }
                }
                catch (Exception e)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
                    return false;
                }
            }
            try
            {
                using StreamWriter sw = new(stm, GetEncoding(wfmtxtformat));
                foreach (var item in stringlist)
                {
                    sw.WriteLine(item);
                }
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
                return false;
            }
            return true;
        }
        public static Boolean SavePassFailWaveform(String path, String name, ChannelId id, Boolean postfix, TxtFormat wfmtxtfotmat = TxtFormat.UTF8)
        {
            if (DsoModel.Default.TryGetChannel(id, out var cm) && cm.Pack is not null)
            {
                //if (cm.Pack.Properties.Version != "U2.0")
                //    return false;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    if (!Directory.Exists(path))
                    {
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Path does not exist: '{path}'", EventBus.LogLevel.Error));
                        return false;
                    }
                }
                String appenddatetime = postfix ? GetDateTimeString() : "";
                String fullfilename = path + "\\" + name + appenddatetime + ".csv";
                if (File.Exists(fullfilename))
                {
                    if (!StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.FileExisted, MessageType.Warning))
                    {
                        return false;
                    }
                    if (!DeleteFile(fullfilename))
                    {
                        WeakTip.Default.Write("File", MsgTipId.FileOccupied);
                        return false;
                    }
                }
                DsoModel.Default.File.FileName = name + appenddatetime;
                using var fs = new FileStream(fullfilename, FileMode.OpenOrCreate, FileAccess.Write);

                return SavePassFailWaveByCsv(fs, cm.Pack, id);

            }
            return false;
        }
        public static Boolean SavePassFailWaveByCsv(Stream stm, WfmPack pkg, ChannelId id)
        {
            List<WfmPack> list = new List<WfmPack>();
            list.Add(pkg);
            var ids = ChannelIdExt.GetAnalogs();
            foreach (var item in ids)
            {
                if (id == item)
                {
                    continue;
                }
                if (DsoModel.Default.TryGetChannel(item, out var cm) && cm.Pack is not null && cm.Active)
                {
                    list.Add(cm.Pack);
                }
            }
            return SavePassFailWaveByCSV(stm, list, (x, y) => x.ToString("E") + "," + String.Join(",", y.Select(o => o.ToString("G"))));
        }
        public static Boolean SavePassFailWaveByCSV(Stream stm, List<WfmPack> wfmPackList, Func<Double, Double[], String> cbfunc, TxtFormat wfmtxtformat = TxtFormat.UTF8)
        {
            List<string> stringlist = new List<string>();

            foreach (var item in wfmPackList)
            {
                Int32 index = 0;

                WfmPack pkg = item;
                if (pkg is null)
                {
                    continue;
                }
                try
                {
                    Int32 length = pkg.Buffer.GetLength(1);
                    Int32 wfmcnt = pkg.Buffer.GetLength(0);

                    Double sp = pkg.Properties.TmbScale.Value * Constants.VIS_XDIVS_NUM / length;

                    Double pos0 = pkg.Properties.TmbPosition.Index / Constants.MAX_XPOS_IDX * pkg.Properties.TmbScale.Value * Constants.VIS_XDIVS_NUM;

                    Double time;
                    Double[] ampls = new Double[wfmcnt];
                    String text = FileInfo[0] + $":{pkg.Properties.Name}";
                    if (stringlist.Count < 1)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[0] = stringlist[0] + ",," + text;
                    }
                    text = FileInfo[1] + $":{SIHelper.ValueChangeToSI(pkg.Properties.TmbScale.Value / 1E6, 0, "s")}";
                    if (stringlist.Count < 2)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[1] = stringlist[1] + ",," + text;
                    }
                    text = FileInfo[2] + $":{SIHelper.ValueChangeToSI(pkg.Properties.ChnlScale.Value / 1E3, 0, "V")}";
                    if (stringlist.Count < 3)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[2] = stringlist[2] + ",," + text;
                    }
                    text = FileInfo[3] + $":{SIHelper.ValueChangeToSI(DsoModel.Default.Timebase.AnalogSamplingRate, 2, "Sa/s")}";
                    if (stringlist.Count < 4)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[3] = stringlist[3] + ",," + text;
                    }
                    text = FileInfo[4] + $":{DsoModel.Default.Timebase.StorageWaveDotsCnt}";
                    if (stringlist.Count < 5)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[4] = stringlist[4] + ",," + text;
                    }
                    text = FileInfo[5] + $":{pkg.Properties.TmbScale.Value}" + $":{pkg.Properties.TmbScale.Index}";
                    if (stringlist.Count < 6)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[5] = stringlist[5] + ",," + text;
                    }
                    text = FileInfo[6] + $":{pkg.Properties.TmbPosition.Value}" + $":{pkg.Properties.TmbPosition.Index}";
                    if (stringlist.Count < 7)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[6] = stringlist[6] + ",," + text;
                    }
                    text = FileInfo[7] + $":{pkg.Properties.ChnlScale.Value}" + $":{pkg.Properties.ChnlScale.Index}";
                    if (stringlist.Count < 8)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[7] = stringlist[7] + ",," + text;
                    }
                    text = FileInfo[8] + $":{pkg.Properties.ChnlPosition.Value}" + $":{pkg.Properties.ChnlPosition.Index}";
                    if (stringlist.Count < 9)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[8] = stringlist[8] + ",," + text;
                    }
                    text = FileInfo[9] + $":{pkg.Properties.SampInterval}";
                    if (stringlist.Count < 10)
                    {
                        stringlist.Add(text);
                    }
                    else
                    {
                        stringlist[9] = stringlist[9] + ",," + text;
                    }

                    String header;
                    //!!!Patch: digital channel is different from analog channel
                    if (!pkg.Properties.Name.Contains("D"))
                    {
                        var headertmp = $"Y-axis({pkg.Properties.ChnlUnit.Prefix.ToPfxString()}{pkg.Properties.ChnlUnit.Name})";
                        header = headertmp;
                        for (Int32 i = 1; i < wfmcnt; i++)
                        {
                            header += ", " + headertmp;
                        }
                    }
                    else
                    {
                        header = $"Y-axis({ChannelIdExt.MinDChId}...{ChannelIdExt.MaxDChId})";
                    }
                    header = $"X-axis({pkg.Properties.TmbUnit.Prefix.ToPfxString()}{pkg.Properties.TmbUnit.Name}), " + header;
                    if (stringlist.Count < 11)
                    {
                        stringlist.Add(header);
                    }
                    else
                    {
                        stringlist[10] = stringlist[10] + "," + header;
                    }
                    index = 11;
                    for (Int32 j = 0; j < length; j++)
                    {
                        time = (j * sp - pos0) + pkg.Properties.TrigErrorTime;
                        for (Int32 i = 0; i < wfmcnt; i++)
                        {
                            ampls[i] = pkg.Buffer[i, j];
                        }
                        if (stringlist.Count < index + 1)
                        {
                            stringlist.Add(cbfunc(time, ampls));
                        }
                        else
                        {
                            stringlist[index] = stringlist[index] + "," + cbfunc(time, ampls);
                        }
                        index++;
                    }
                }
                catch (Exception e)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
                    return false;
                }
            }
            try
            {
                using StreamWriter sw = new(stm, GetEncoding(wfmtxtformat));
                foreach (var item in stringlist)
                {
                    sw.WriteLine(item);
                }
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
                return false;
            }
            return true;
        }
        public static Boolean SaveWaveByText(Stream stm, WfmPack pkg, Func<Double, Double[], String> cbfunc, TxtFormat wfmtxtformat = TxtFormat.UTF8)
        {
            try
            {
                using StreamWriter sw = new(stm, GetEncoding(wfmtxtformat));

                Int32 length = pkg.Buffer.GetLength(1);
                Int32 wfmcnt = pkg.Buffer.GetLength(0);

                Double sp = pkg.Properties.TmbScale.Value * Constants.VIS_XDIVS_NUM / length;

                Double pos0 = pkg.Properties.TmbPosition.Index / Constants.MAX_XPOS_IDX * pkg.Properties.TmbScale.Value * Constants.VIS_XDIVS_NUM;

                Double time;
                Double[] ampls = new Double[wfmcnt];
                String text = FileInfo[0] + $":{DsoModel.Default.File.WfmSource}";
                sw.WriteLine(text);
                text = FileInfo[1] + $":{SIHelper.ValueChangeToSI(pkg.Properties.TmbScale.Value / 1E6, 0, "s")}";
                sw.WriteLine(text);
                text = FileInfo[2] + $":{SIHelper.ValueChangeToSI(pkg.Properties.ChnlScale.Value / 1E3, 0, "V")}";
                sw.WriteLine(text);
                text = FileInfo[3] + $":{SIHelper.ValueChangeToSI(DsoModel.Default.Timebase.AnalogSamplingRate, 2, "Sa/s")}";
                sw.WriteLine(text);
                text = FileInfo[4] + $":{DsoModel.Default.Timebase.StorageWaveDotsCnt}";
                sw.WriteLine(text);
                String header;
                //!!!Patch: digital channel is different from analog channel
                if (!pkg.Properties.Name.Contains("D"))
                {
                    header = $"Y-axis({pkg.Properties.ChnlUnit.Prefix.ToPfxString()}{pkg.Properties.ChnlUnit.Name})";
                    for (Int32 i = 1; i < wfmcnt; i++)
                    {
                        header = header + ", " + header;
                    }
                }
                else
                {
                    header = $"Y-axis({ChannelIdExt.MinDChId}...{ChannelIdExt.MaxDChId})";
                }
                header = $"X-axis({pkg.Properties.TmbUnit.Prefix.ToPfxString()}{pkg.Properties.TmbUnit.Name}), " + header;
                sw.WriteLine(header);
                for (Int32 j = 0; j < length; j++)
                {
                    time = (j * sp - pos0) + pkg.Properties.TrigErrorTime;
                    for (Int32 i = 0; i < wfmcnt; i++)
                    {
                        ampls[i] = pkg.Buffer[i, j];
                    }

                    sw.WriteLine(cbfunc(time, ampls));
                }
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
#if DEBUG
                throw;
#else                
                return false;
#endif
            }

            return true;
        }

        public static Boolean SaveWaveByRaw(Stream stm, WfmPack pkg)
        {
            if (pkg.Properties.Name.Contains('D'))
            {
                return SaveWaveByText(stm, pkg, (x, y) =>
                    x.ToString("E") + ", "
                    + String.Join(", ", y.Select(o => ((UInt16)o).ToString("X4"))));
            }
            else
            {
                var adc0 = pkg.Properties.ChnlPosition.Index / Constants.IDX_PER_YDIV * Constants.ERES_SAMPS_PER_YDIV + Constants.ERES_MAX_ADC_RES / 2;

                //!!!  Adaptation required, "/*o.ToString("N6") + */" and "+ rd.Next(1,16)"   is to increase the precision after conversion.
                var rd = new Random();
                return SaveWaveByText(stm, pkg, (x, y) =>
                    x.ToString("E") + ", "
                    + String.Join(", ", y.Select(o => /*o.ToString("N6") + */"(" + (((Int32)Math.Round((o - pkg.Properties.ChnlBias) * Constants.ERES_SAMPS_PER_YDIV / pkg.Properties.ChnlScale.Value + adc0) + rd.Next(1, 16))).ToString("X4") + ")")));
            }
        }

        public static Boolean SaveWaveByTsv(Stream stm, WfmPack pkg)
        {
            return SaveWaveByText(stm, pkg, (x, y) => x.ToString("E") + "\t" + String.Join("\t", y.Select(o => o.ToString("N6"))));
        }

        public static Boolean SaveWaveByTxt(Stream stm, WfmPack pkg, TxtFormat fotmat)
        {
            return SaveWaveByText(stm, pkg, (x, y) => x.ToString("E") + "," + String.Join(",", y.Select(o => o.ToString("G"))), fotmat);
        }

        public static Boolean SaveWaveByCsv(Stream stm, params WfmPack[] pkgs)
        {
            return SaveWaveByCSV(stm, (x, y) => x.ToString("E") + "," + String.Join(",", y.Select(o => o.ToString("G"))), pkgs: pkgs);
        }

        public static Boolean SaveWaveByCsv(Stream stm, WfmPack pkg)
        {
            return SaveWaveByCSV(stm, pkg, (x, y) => x.ToString("E") + "," + String.Join(",", y.Select(o => o.ToString("G"))));
        }

        public static Boolean SaveWaveByMatlab(Stream stm, WfmPack pkg, String path)
        {
            return SaveWaveByText(stm, pkg, (x, y) => x.ToString("E") + " " + String.Join(" ", y.Select(o => o.ToString("G"))));
            //return MatlabAux.Instance.SaveWaveByMatlab(pkg, path);
        }

        public static Boolean SaveWaveByExcel(Stream stm, WfmPack pkg)
        {
            try
            {
                XSSFWorkbook workbook = new();
                ISheet sheet = workbook.CreateSheet("Sheet1");

                Int32 length = pkg.Buffer.GetLength(1);
                Int32 wfmcnt = pkg.Buffer.GetLength(0);

                Double sp = pkg.Properties.TmbScale.Value * Constants.VIS_XDIVS_NUM / length;

                //Double pos0 = pkg.Properties.TmbPosition.Index;
                Double pos0 = pkg.Properties.TmbPosition.Index / Constants.MAX_XPOS_IDX * pkg.Properties.TmbScale.Value * Constants.VIS_XDIVS_NUM;//???????
                
                Double time;
                var info = (XSSFRow)sheet.CreateRow(0);
                info.CreateCell(0).SetCellValue(FileInfo[0]);
                info.CreateCell(1).SetCellValue($"{DsoModel.Default.File.WfmSource}");
                info = (XSSFRow)sheet.CreateRow(1);
                info.CreateCell(0).SetCellValue(FileInfo[1]);
                info.CreateCell(1).SetCellValue($"{SIHelper.ValueChangeToSI(pkg.Properties.TmbScale.Value / 1E6, 0, "s")}");
                info = (XSSFRow)sheet.CreateRow(2);
                info.CreateCell(0).SetCellValue(FileInfo[2]);
                info.CreateCell(1).SetCellValue($"{SIHelper.ValueChangeToSI(pkg.Properties.ChnlScale.Value / 1E3, 0, "V")}");
                info = (XSSFRow)sheet.CreateRow(3);
                info.CreateCell(0).SetCellValue(FileInfo[3]);
                info.CreateCell(1).SetCellValue($"{SIHelper.ValueChangeToSI(DsoModel.Default.Timebase.AnalogSamplingRate, 2, "Sa/s")}");
                info = (XSSFRow)sheet.CreateRow(4);
                info.CreateCell(0).SetCellValue(FileInfo[4]);
                info.CreateCell(1).SetCellValue($"{DsoModel.Default.Timebase.StorageWaveDotsCnt}");

                //info = (XSSFRow)sheet.CreateRow(5);
                //info.CreateCell(0).SetCellValue("");
                //info.CreateCell(1).SetCellValue("");

                IRow head = sheet.CreateRow(5);
                head.CreateCell(0).SetCellValue($"X-axis({pkg.Properties.TmbUnit.Prefix.ToPfxString()}{pkg.Properties.TmbUnit.Name})");
                for (Int32 i = 0; i < wfmcnt; i++)
                {
                    head.CreateCell(i + 1).SetCellValue($"Y-axis({pkg.Properties.ChnlUnit.Prefix.ToPfxString()}{pkg.Properties.ChnlUnit.Name})");
                }

                for (Int32 j = 0; j < length; j++)
                {
                    var row = (XSSFRow)sheet.CreateRow(j + 6);

                    //time = (j - pos0) * sp + pkg.Properties.TrigErrorTime;
                    time = (j * sp - pos0) + pkg.Properties.TrigErrorTime;//??????
                    row.CreateCell(0).SetCellValue(time);

                    for (Int32 i = 0; i < wfmcnt; i++)
                    {
                        row.CreateCell(i + 1).SetCellValue(pkg.Buffer[i, j]);
                        //if (row.GetCell(i) == null)
                        //{
                        //    row.CreateCell(i).SetCellValue(data);
                        //}
                        //else
                        //{
                        //    row.GetCell(i).SetCellValue(data);
                        //}
                    }
                }

                workbook.Write(stm);
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
#if DEBUG
                throw;
#else                
                return false;
#endif
            }

            return true;


        }
        #endregion

        public static Boolean SaveImage(String fullpath, PicFormat pf, PicArea region, PicColor color = PicColor.Standard, Boolean IsCheckSameName = true, Boolean needCloseWeakTip = false)
        {
            var path = Path.GetDirectoryName(fullpath);
            var file = Path.GetFileName(fullpath);

            if (String.IsNullOrEmpty(file))
            {
                return false;
            }

            if (path?.Length == 0)
            {
                path = Directory.GetCurrentDirectory();
            }
            return SaveImage(path!, file, pf, region, false, color, IsCheckSameName, needCloseWeakTip);
        }

        public Boolean SaveImage(Boolean IsCheckSameName = true, Boolean needCloseWeakTip = false)
        {
            return SaveImage(PicPath, FileName, PicFormat, PicRegion, IfAppendDatetime, PicColor, IsCheckSameName, needCloseWeakTip);
        }

        /// <summary>
        /// SaveImage extension
        /// </summary>
        /// <param name="path">path</param>
        /// <param name="name">name</param>
        /// <param name="pf">picture format</param>
        /// <param name="region">picture area</param>
        /// <param name="postfix">postfix</param>
        /// <param name="color">picture color</param>
        /// <param name="IsCheckSameName">check same name</param>
        /// <param name="needCloseWeakTip">close weaktip</param>
        /// <returns>0-->false,1-->true,2-->unsave</returns>
        public static Int32 SaveImageEx(String path, String name, PicFormat pf, PicArea region, Boolean postfix, PicColor color = PicColor.Standard, Boolean IsCheckSameName = true, Boolean needCloseWeakTip = false)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                if (!Directory.Exists(path))
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Path does not exist: '{path}'", EventBus.LogLevel.Error));
                    return 0;
                }
            }

            String fullfilename = path + "\\" + name + (postfix ? GetDateTimeString() : "") + GetPicFileExtName(pf);
            if (File.Exists(fullfilename))
            {
                if (IsCheckSameName && !StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.FileExisted, MessageType.Warning))
                {
                    return 2;
                }
                if (!DeleteFile(fullfilename))
                {
                    WeakTip.Default.Write("File", MsgTipId.FileOccupied);
                    return 0;
                }
            }

            var ms = GetImageStreamHandler?.Invoke(pf, region, color, needCloseWeakTip);
            if (ms is not null)
            {
                try
                {
                    using FileStream fs = new(fullfilename, FileMode.Create, FileAccess.Write);
                    ms.WriteTo(fs);
                    ms.Close();
                    return 1;
                }
                catch (Exception e)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs("Failed to save full screen: " + e.ToString(), EventBus.LogLevel.Error));
                }
            }
            return 0;
        }

        public static Boolean SaveImage(String path, String name, PicFormat pf, PicArea region, Boolean postfix, PicColor color = PicColor.Standard, Boolean IsCheckSameName = true, Boolean needCloseWeakTip = false)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                if (!Directory.Exists(path))
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Path does not exist: '{path}'", EventBus.LogLevel.Error));
                    return false;
                }
            }

            String fullfilename = path + "\\" + name + (postfix ? GetDateTimeString() : "") + GetPicFileExtName(pf);
            if (File.Exists(fullfilename))
            {
                if (IsCheckSameName && !StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.FileExisted, MessageType.Warning))
                {
                    return false;
                }
                if (!DeleteFile(fullfilename))
                {
                    WeakTip.Default.Write("File", MsgTipId.FileOccupied);
                    return false;
                }
            }

            var ms = GetImageStreamHandler?.Invoke(pf, region, color, needCloseWeakTip);
            if (ms is not null)
            {
                try
                {
                    using FileStream fs = new(fullfilename, FileMode.Create, FileAccess.Write);
                    ms.WriteTo(fs);
                    ms.Close();
                    return true;
                }
                catch (Exception e)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs("Failed to save full screen: " + e.ToString(), EventBus.LogLevel.Error));
                }

            }
            return false;
        }
        public static Boolean SaveImage(Bitmap bitmap, String path, String name, PicFormat pf, Boolean postfix, Boolean IsCheckSameName = true, Boolean needCloseWeakTip = false)
        {
            if (bitmap is null)
                return false;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                if (!Directory.Exists(path))
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Path does not exist: '{path}'", EventBus.LogLevel.Error));
                    return false;
                }
            }

            String fullfilename = path + "\\" + name + (postfix ? GetDateTimeString() : "") + GetPicFileExtName(pf);
            if (File.Exists(fullfilename))
            {
                if (IsCheckSameName && !StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.FileExisted, MessageType.Warning))
                {
                    return false;
                }
                if (!DeleteFile(fullfilename))
                {
                    WeakTip.Default.Write("File", MsgTipId.FileOccupied);
                    return false;
                }
            }
            try
            {
                MemoryStream ms = new();
                bitmap.Save(ms, GetImageFormat(pf));
                bitmap.Dispose();
                using FileStream fs = new(fullfilename, FileMode.Create, FileAccess.Write);
                ms.WriteTo(fs);
                ms.Close();
                return true;
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs("Failed to save full screen: " + e.ToString(), EventBus.LogLevel.Error));
            }
            return false;
        }
        private static System.Drawing.Imaging.ImageFormat GetImageFormat(PicFormat pf) => pf switch
        {
            PicFormat.Gif => System.Drawing.Imaging.ImageFormat.Gif,
            PicFormat.Jpeg => System.Drawing.Imaging.ImageFormat.Jpeg,
            PicFormat.Png => System.Drawing.Imaging.ImageFormat.Png,
            PicFormat.Tiff => System.Drawing.Imaging.ImageFormat.Tiff,
            _ => System.Drawing.Imaging.ImageFormat.Bmp,
        };
        public static Boolean? SaveLabNoteBook(String LabNotePath, String LabNoteName, ChannelId channelId)
        {
            return SaveLabNoteBookHandler?.Invoke(LabNotePath, LabNoteName, channelId);
        }

        //public String MakeDefaultFileName(String path, String ext, String middleName = "")
        //{
        //    var result = new DirectoryInfo(path)
        //        .GetFiles($"*{ext}", SearchOption.TopDirectoryOnly)
        //        .Where(x => Regex.IsMatch(x.Name, $"^{DefaultPrefixName}{middleName}[0-9]{"{3}"}{ext}$", RegexOptions.IgnoreCase));

        //    return $"{DefaultPrefixName}{middleName}{result.Count():D3}";
        //}

        public String MakeDefaultFileName(String path, String ext, String middleName = "")
        {
            var result = new DirectoryInfo(path)
                .GetFiles($"*{ext}", SearchOption.TopDirectoryOnly)
                .Where(x => Regex.IsMatch(x.Name, $"^{DefaultPrefixName}{middleName}[0-9]{"{3}"}{ext}$", RegexOptions.IgnoreCase));
            var filename = $"{DefaultPrefixName}{middleName}{result.Count():D3}";
            var filepath = Path.Combine(path, $"{filename}{ext}");
            Int32 index = 0;
            while (File.Exists(filepath))
            {
                index++;
                filename = $"{DefaultPrefixName}{middleName}{result.Count() + index:D3}";
                filepath = Path.Combine(path, $"{filename}{ext}");
            }
            return filename;
        }

        public static Boolean SaveToText(String path, String name, Func<StreamWriter, Boolean> writer)
        {
            Boolean res = false;
            try
            {
                String fullfilename = path + "\\" + Path.GetFileNameWithoutExtension(name) + ".txt";
                using var fs = new FileStream(fullfilename, FileMode.Create, FileAccess.Write);
                using StreamWriter sw = new(fs, Encoding.UTF8);
                res = writer(sw);
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
                //#if DEBUG
                //                throw;
                //#else                
                return false;
                //#endif
            }

            return res;
        }

        public static Boolean SaveToText(String path, String name, IEnumerable<Double> buffer)
        {
            try
            {
                String fullfilename = path + "\\" + name + ".txt";
                using var fs = new FileStream(fullfilename, FileMode.OpenOrCreate, FileAccess.Write);
                using StreamWriter sw = new(fs, Encoding.UTF8);
                foreach (var d in buffer)
                {
                    sw.WriteLine(d);
                }

            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
#if DEBUG
                throw;
#else                
                return false;
#endif
            }

            return true;
        }

        public static Boolean LoadFromText(String fullpath, Func<StreamReader, Boolean> reader)
        {
            var res = false;
            try
            {
                using var fs = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
                using StreamReader sr = new(fs, Encoding.UTF8);
                res = reader(sr);
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
                //#if DEBUG
                //                throw;
                //#else
                return false;
                //#endif
            }

            return res;
        }

        public static Boolean LoadFromText(String fullpath, out List<Double> buffer)
        {
            buffer = new List<Double>();
            try
            {
                using var fs = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
                using StreamReader sr = new(fs, Encoding.UTF8);
                String? line;
                while ((line = sr.ReadLine()) != null)
                {
                    buffer.Add(Double.Parse(line));
                }
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
#if DEBUG
                throw;
#else
                return false;
#endif
            }

            return true;
        }
    }
}
