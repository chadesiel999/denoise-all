using EventBus;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;

namespace ScopeX.Core
{
    /// <summary>
    /// 系统选件升级管理类
    /// 1、流程: 读取SN-->读取试用时间-->读取选件激活状态-->初始化选件信息
    /// 2、先加载所有选件初始信息，包括选件名、选件编码、激活状态，并根据设备序列号生成激活码，
    ///    配件编码作为密钥，对机器序列号（SN码）进行加密，加密后的串号即是激活码
    ///    生成的激活码根据IT部门提供的内部加密规则再次进行加密
    /// 3、读取指定位置所存放的license信息，包括选件名、选件编码，激活码
    /// 4、软件生成的激活码与license中进行比对，若信息一致则表示该选件已激活
    /// 5、激活流程：选取lic-->比对序列号、获取激活码-->与软件生成的激活码比对-->一致则写入flash激活成功，否则激活失败
    /// </summary>
    public class OptionsManager
    {
        protected const Double UPDATE_INTERVAL_MINUTES = 5.0;
        private const Int32 TimeOutByms = 500;

        /// <summary>
        /// License文件后缀名
        /// </summary>
        private const String LicenseFileExtension = ".lic";

        private const Int32 ActiveCodeLength = 16;

        //private String? _SerialNumber;
        /// <summary>
        /// 示波器唯一序列号
        /// </summary>
        public String SerialNumber
        {
            get => DsoModel.Default.DsoInfo.ProductInfos.SerialNumber;
            set
            {
                var sndata = value.Replace("\0", "").Trim();
                if (String.IsNullOrWhiteSpace(sndata))
                {
                    return;
                }
                if (DsoModel.Default.DsoInfo.ProductInfos.SerialNumber != sndata)
                {
                    DsoModel.Default.DsoInfoBackup.ProductInfos.SerialNumber = sndata;

                    DsoModel.Default.DsoInfoBackup.ProductInfos.SyncResult = FalshOpreationResult.None;
                    DsoModel.Default.DsoInfoBackup.ProductInfos.SyncOperation = FalshOpreation.Write;

                    DsoModel.Default.DsoInfoBackup.OptionsInfos.SyncResult = FalshOpreationResult.None;
                    DsoModel.Default.DsoInfoBackup.OptionsInfos.SyncOperation = FalshOpreation.Write;
                    IsActiveOption = 0;
                }
                //烧写相同序列号 只重置试用时间
                ResetRemainingTime();
                Dispatcher.NeedUpdateDsoInfo();
            }
        }

        public void InitDsoInfo()
        {
            ////to do:此处重新烧写序列号后 需要重新初始化选件信息和重置试用时间
            InitAllOption();
            InitOptionsInfo();
        }

        public Boolean Is2GHz => String.IsNullOrWhiteSpace(SerialNumber) || SerialNumber.Length < 5 || SerialNumber.Substring(1, 4) == PlatformManager.Default.Platform.GetSNPrefix();

        /// <summary>
        /// 选件信息
        /// </summary>
        /// <param name="Name">选件名</param>
        /// <param name="Activecode">选件激活码</param>
        /// <param name="Activecode">选件激活码</param>
        public record OptionInfo(String Name, String Activecode, String Description);

        /// <summary>
        /// 根据当前序列号所生成的选项信息表，与从falsh中读上来的进行比对
        /// 包括选件名、选件编码、选件激活码
        /// </summary>
        private ConcurrentDictionary<OptionType, OptionInfo> AllOptionInfo = new ConcurrentDictionary<OptionType, OptionInfo>();

        /// <summary>
        /// 激活码再次加密规则，由IT部门提供
        /// </summary>
        private IReadOnlyDictionary<String, String> ReplaceString = new Dictionary<String, String>()
        {
            ["I"] = "7",
            ["O"] = "5",
            ["L"] = "9",
            ["o"] = "3",
            ["/"] = "1",
            ["."] = "8"
        };

        public static OptionsManager Default { get; } = new OptionsManager();

        // 获取用户的 AppData\Local 目录路径
        private String AppDataLocalPath
        {
            get
            {
                var dirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Dso Scope");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                return Path.Combine(dirPath, "dso.bin");
            }
        }

        internal Double CumulativeUseTime
        {
            get => ReadBinaryFile();
            set => WriteBinaryFile(value);
        }

        private OptionsManager()
        {
            InitOptionsInfo();
        }

        #region 更新试用时间

        /// <summary>
        /// 更新剩余时间，以5分钟为间隔，每次更新减少5分钟
        /// </summary>
        /// <returns></returns>
        internal void UpdateRemainingTime()
        {
            var time = Math.Round(UPDATE_INTERVAL_MINUTES / 60, 2);
            CumulativeUseTime += time;
        }

        /// <summary>
        /// 重置试用期
        /// </summary>
        /// <param name="isOverTime">结束还是重置,True：结束，False：重置</param>
        internal void ResetRemainingTime(Boolean isOverTime = false)
        {
            CumulativeUseTime = 0;
            DsoModel.Default.DsoInfoBackup.OptionsInfos.ResetTrialRemainingTime(isOverTime);
            DsoModel.Default.DsoInfoBackup.OptionsInfos.SyncOperation = FalshOpreation.Write;
            IsActiveOption = 0;

            if (GetRemainingTimeByHour() > 0)
            {
                SysRunTimeMangager.Default.Run();
            }
        }

        /// <summary>
        /// 重置试用期
        /// </summary>
        /// <param name="isOverTime">结束还是重置,True：结束，False：重置</param>
        public void ResetTime(Boolean isOverTime)
        {
            ResetRemainingTime(isOverTime);
            Dispatcher.NeedUpdateDsoInfo();
        }


        // 将数字以二进制格式写入文件
        private void WriteBinaryFile(Double content)
        {
            try
            {
                using (FileStream fs = new FileStream(AppDataLocalPath, FileMode.OpenOrCreate, FileAccess.Write))
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(content);
                }
            }
            catch
            {

            }
        }

        // 从二进制文件中读取数字
        private Double ReadBinaryFile()
        {
            try
            {
                using (FileStream fs = new FileStream(AppDataLocalPath, FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    // 读取并返回文件中的数字
                    var content = reader.ReadDouble();
                    return content;
                }
            }
            catch
            {
                return 0;
            }
        }


        #endregion

        /// <summary>
        /// 初始化选件信息、选件激活状态并通过软件生成激活码
        /// </summary>
        internal void InitAllOption()
        {
            AllOptionInfo = new();
            var options = Enum.GetValues<OptionType>()
                .Where(o => o.GetProductTypes().Contains($"{PlatformManager.Default.Platform.ProductType}"))
                .Select(o => new KeyValuePair<OptionType, (String ModelName, String Description)>(o, EnumEx.GetOptionDescription(o))).ToList();
            foreach (var option in options)
            {
                #region 生成激活码 通过选件编码对序列号进行加密

                //编码和加密方式必须保密！编码作为密钥不能超过16个字符
                //利用密码加密函数crypt()进行加密，假设SN号为：AUTS01220200002，可选配件为A1256
                //参数key为：”AUTS01220200002”。
                //参数salt为：”$1$A1256$”
                //函数返回结果为：$1$A1256$n3MMgHE3bJRTIBo66fXrb0，即激活码为：”n3MMgHE3bJRTIBo66fXrb0”

                var key = SerialNumber;
                var optionname = option.Value.ModelName; //$"$1${option.Value.ModelName}$";//加密函数已做处理，此处直接传入选件名即可
                var activecode = PlatformManager.Default.Platform.GetOptionActiveCode(key, optionname);
                var lastindex = activecode!.LastIndexOf("$");
                if (lastindex == -1)
                {
                    var msg = $"Opint is not exist,please check option name";
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(msg, LogLevel.Warn));
                    //throw new Exception(msg);
                }
                else
                    activecode = activecode.Substring(lastindex + 1, ActiveCodeLength);

                //得到的激活码再次加密
                foreach (var str in ReplaceString.ToList())
                {
                    activecode = activecode.Replace(str.Key, str.Value);
                }

                #endregion

                var info = new OptionInfo(option.Value.ModelName, activecode, option.Value.Description);
                AllOptionInfo.TryAdd(option.Key, info);
                //TryAdd($"{option.Key}", false);
            }
        }

        private void InitOptionsInfo()
        {
            var options = Enum.GetValues<OptionType>()
                .Where(o => o.GetProductTypes().Contains($"{PlatformManager.Default.Platform.ProductType}"))
                .Select(o => new KeyValuePair<OptionType, (String ModelName, String Description)>(o, EnumEx.GetOptionDescription(o))).ToList();
            foreach (var option in options)
            {
                TryAdd($"{option.Key}", false);
            }
        }

        /// <summary>
        /// 添加选件信息
        /// </summary>
        /// <param name="optionType">选件名</param>
        /// <param name="active">激活状态</param>
        /// <returns>false：选件名过长或选件数量超过额定数量，true：添加成功</returns>
        private Boolean TryAdd(String optionType, Boolean active)
        {
            if (DsoModel.Default.DsoInfo.OptionsInfos.AllOptions == null || DsoModel.Default.DsoInfo.OptionsInfos.AllOptions.Count > Constants.OPTIONS_MAX_COUNT)
            {
                return false;
            }
            if (!DsoModel.Default.DsoInfo.OptionsInfos.AllOptions.ContainsKey(optionType))
            {
                var type = optionType.Length > Constants.OPTIONNAME_MAX_LENGTH ? optionType.Substring(0, Constants.OPTIONNAME_MAX_LENGTH) : optionType;
                DsoModel.Default.DsoInfo.OptionsInfos.AllOptions.TryAdd(type, active);
            }
            else
            {
                DsoModel.Default.DsoInfo.OptionsInfos.AllOptions[optionType] = active;
            }

            DsoModel.Default.DsoInfo.OptionsInfos.CloneTo(DsoModel.Default.DsoInfoBackup.OptionsInfos);

            return true;
        }

        /// <summary>
        /// 是否是激活选件 用于弱提示
        /// 0-->无提示，1-->移除，2-->激活
        /// </summary>
        public Int32 IsActiveOption { get; set; } = 0;

        #region 获取产品信息或者选件信息 使用DsoInfo

        /// <summary>
        /// 获取选件激活状态（包含套装选件）
        /// </summary>
        /// <param name="option">选件名</param>
        /// <returns>True：已激活；false：未激活</returns>
        public Boolean GetOptionActive(OptionType option)
        {
            Boolean active;
            if (TryGetActive($"{option}", out active))
            {
                if ((Int32)option >= (Int32)OptionType.Jitter && TryGetActive($"{OptionType.BND}", out Boolean bndActive))
                {
                    active |= bndActive;
                }
            }

            return active;
        }

        /// <summary>
        /// 通过激活状态和剩余试用时间，获取选件是否可用，
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public Boolean GetOptionAvailable(OptionType option)
        {
            return GetOptionActive(option) || DsoModel.Default.DsoInfo.OptionsInfos.TrialRemainingTimeByHour > 0;
        }

        /// <summary>
        /// 获取选件激活信息（排除套装选件）
        /// </summary>
        /// <param name="option">选件名</param>
        /// <returns></returns>
        public Boolean GetOpitonActiveEx(OptionType option)
        {
            return TryGetActive($"{option}", out var active) && active;
        }

        //避免重复更新
        internal volatile Boolean AllActiveUpdateStatus = false;

        /// <summary>
        /// 更新完成标志
        /// </summary>
        public volatile Boolean UpdateCompleteFlag = false;

        /// <summary>
        /// 所有选件是否激活
        /// </summary>
        /// <returns></returns>
        public Boolean IsAllActive()
        {
            if (DsoModel.Default.DsoInfo.OptionsInfos.AllOptions == null)
            {
                return false;
            }
            var bndactive = GetOpitonActiveEx(OptionType.BND);
            if (bndactive)//套装已激活
            {
                var active = true;
                var options = AllOptionInfo.Where(option => option.Key < OptionType.Jitter).ToList();//非2G  非7000L
                if (Is2GHz)
                {
                    var index = options.FindIndex(x => x.Key == OptionType.BW10T20);
                    if (index >= 0 && options.Count > 0)
                    {
                        options.RemoveAt(index);
                    }
                }

                foreach (var item in options)
                {
                    active &= GetOpitonActiveEx(item.Key);
                }

                return active;
            }
            else//套装未激活
            {
                var unactive = DsoModel.Default.DsoInfo.OptionsInfos.AllOptions.Where(option => option.Key != $"{OptionType.BND}" && option.Value == false).ToList();//非2G  非7000L
                if (Is2GHz)
                {
                    var index = unactive.FindIndex(x => x.Key == $"{OptionType.BW10T20}");
                    if (index >= 0 && unactive.Count > 0)
                        unactive.RemoveAt(index);
                }

                return unactive.Count == 0;
            }
        }

        /// <summary>
        /// 获取剩余使用时间
        /// </summary>
        /// <returns></returns>
        public Double GetRemainingTimeByHour() => DsoModel.Default.DsoInfo.OptionsInfos.TrialRemainingTimeByHour - CumulativeUseTime;

        /// <summary>
        /// 获取选件的激活状态
        /// </summary>
        /// <param name="optionType">选件名</param>
        /// <param name="active">选件的激活状态</param>
        /// <returns>true：成功获取，false：未找到选件</returns>
        internal Boolean TryGetActive(String optionType, out Boolean active)
        {
            active = false;
            return DsoModel.Default.DsoInfo.OptionsInfos.AllOptions != null && DsoModel.Default.DsoInfo.OptionsInfos.AllOptions.TryGetValue(optionType, out active);
        }

        //internal void TryGetOrAddActive(String optionType, out Boolean active)
        //{
        //    active = false;
        //    if (DsoModel.Default.DsoInfo.OptionsInfos.AllOptions == null)
        //    {
        //        return;
        //    }
        //    active = DsoModel.Default.DsoInfo.OptionsInfos.AllOptions.GetOrAdd(optionType, false);
        //}

        #endregion 获取产品信息或者选件信息 使用DsoInfo

        #region 更新产品信息或者选件信息 使用DsoInfoBackup

        /// <summary>
        /// 移除选件许可信息
        /// </summary>
        /// <param name="option">选件类型</param>
        /// <returns></returns>
        public Boolean RemoveLicense(OptionType option)
        {
            var bok = TrySetActive($"{option}", false);
            DsoModel.Default.DsoInfoBackup.OptionsInfos.SyncResult = FalshOpreationResult.None;
            DsoModel.Default.DsoInfoBackup.OptionsInfos.SyncOperation = FalshOpreation.Write;
            Dispatcher.NeedUpdateDsoInfo();

            return bok;
        }

        ///// <summary>
        ///// 一键清除许可信息，提供给测试用
        ///// </summary>
        //public void ClearLicenseInfo()
        //{
        //    AllOptionInfo.Clear();
        //    InitAllOption();
        //    InitOptionsInfo();
        //    WeakTip.Default.Write(nameof(OptionsManager), MsgTipId.LicenseRemoveSuccess, duration: 5);
        //}

        /// <summary>
        /// 激活单个选件
        /// </summary>
        /// <param name="licensepath">License路径</param>
        /// <returns>激活结果 false：激活失败；true：激活成功</returns>
        public Boolean ActiveOption(String licensepath, out Boolean updateView)
        {
            updateView = false;
            OptionType type = OptionType.BW10T20;
            var bok = false;
            if (!File.Exists(licensepath))
            {

                return false;
            }

            var activecode = GetActiveCode(licensepath, true);
            if (String.IsNullOrEmpty(activecode))
            {
                return false;
            }
            ///License文件中可能会包含分隔符“-”，在计算和比对的时候 需要先移除分隔符
            var newactivecode = activecode.Replace("-", "");
            foreach (var option in AllOptionInfo)
            {
                bok = ComparingActiveCode(option.Value.Activecode, newactivecode);
                if (bok)
                {
                    type = option.Key;
                    if (TryGetActive($"{option.Key}", out Boolean active))
                    {
                        if (bok == active)
                        {
                            WeakTip.Default.Write(nameof(OptionsManager), MsgTipId.OptionActived, duration: 5);
                        }
                        else
                        {
                            TrySetActive($"{option.Key}", bok);
                            DsoModel.Default.DsoInfoBackup.OptionsInfos.SyncResult = FalshOpreationResult.None;
                            DsoModel.Default.DsoInfoBackup.OptionsInfos.SyncOperation = FalshOpreation.Write;
                            Dispatcher.NeedUpdateDsoInfo();
                            updateView = true;
                        }

                    }
                    break;
                }
            }
            if (!bok)
            {
                WeakTip.Default.Write(nameof(OptionsManager), MsgTipId.OptionActiveFail, duration: 5);
            }
            return bok;
        }

        private String? GetActiveCode(String path, Boolean ShowTips = false)
        {
            String? activationcode = null;
            try
            {
                Stream stream = new FileStream(path, FileMode.Open);
                var xmldoc = new XmlDocument();
                xmldoc.Load(stream);
                foreach (XmlNode Node in xmldoc!.DocumentElement!.ChildNodes)
                {
                    if (Node.Name == "License")
                    {
                        var childnodes = Node.ChildNodes.Cast<XmlNode>().ToArray();

                        activationcode = childnodes.FirstOrDefault(n => n.Name == "ActivationCode")!.InnerText; //只关心激活码

                        return activationcode; //因为已经约定好一个.lic文件只会有一个“License”节点，直接return
                    }
                }
            }
            catch
            {
                activationcode = null;
                //WeakTip.Default.Write("License", MsgTipId.LicenseFileFormatError, duration: 5);
                if (!ShowTips)
                {
                    WeakTip.Default.Write(nameof(OptionsManager), MsgTipId.LicenseFileFormatError, duration: 5);
                    //StrongTip.Default.Show(MsgTipId.Error, MsgTipId.LicenseFileFormatError, MessageType.Error);
                    //EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(ex.Message, EventBus.LogLevel.Error));
                }
            }

            return activationcode;
        }


        /// <summary>
        /// 激活码比对
        /// </summary>
        /// <param name="fromSoft">软件生成的激活码</param>
        /// <param name="activeCode">License或注册表中读取的激活码</param>
        /// <param name="count">比对位数，默认全部比对</param>
        /// <returns>false：未激活；true：已激活</returns>
        private Boolean ComparingActiveCode(String fromSoft, String? activeCode, Int32 count = 0)
        {
            if (String.IsNullOrEmpty(activeCode) || count < 0)
            {
                return false;
            }
            if (count == 0) //比较全部
            {
                return fromSoft == activeCode;

            }
            else if (count <= fromSoft.Length && count <= activeCode.Length) //比较前count位
            {
                return fromSoft.Substring(0, count) == activeCode.Substring(0, count);
            }
            return false;
        }



        /// <summary>
        /// 移除选件
        /// 暂未开放移除选件功能 需要移除选件时修改其激活状态即可
        /// </summary>
        /// <param name="optionType">选件名</param>
        /// <returns>true：成功移除，false：未找到选件</returns>
        private Boolean TryRemove(String optionType)
        {
            return DsoModel.Default.DsoInfoBackup.OptionsInfos.AllOptions != null && DsoModel.Default.DsoInfoBackup.OptionsInfos.AllOptions.TryRemove(optionType, out Boolean active);
        }

        /// <summary>
        /// 设置选件的激活状态
        /// </summary>
        /// <param name="optionType">选件名</param>
        /// <param name="active">新的激活状态</param>
        /// <returns>true：成功设置，false：未找到选件</returns>
        internal Boolean TrySetActive(String optionType, Boolean active)
        {
            if (DsoModel.Default.DsoInfoBackup.OptionsInfos.AllOptions?.ContainsKey(optionType) == true)
            {
                DsoModel.Default.DsoInfoBackup.OptionsInfos.AllOptions[optionType] = active;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置所有选件的激活状态
        /// </summary>
        /// <param name="active">新的激活状态</param>
        internal void SetAllActive(Boolean active)
        {
            if (DsoModel.Default.DsoInfoBackup.OptionsInfos.AllOptions == null)
            {
                return;
            }

            foreach (var option in DsoModel.Default.DsoInfoBackup.OptionsInfos.AllOptions.Keys.ToList())
            {
                DsoModel.Default.DsoInfoBackup.OptionsInfos.AllOptions[option] = active;
            }
        }

        #endregion 更新产品信息或者选件信息 使用DsoInfoBackup
    }
}
