using ScopeX.ComModel;
using ScopeX.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ScopeX.U2
{
    public partial class AiExportPage : UserControl
    {
        // 定义配置文件保存路径
        private readonly string _configFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user_signal_types.txt");

        private readonly List<string> _supportedSignalTypes = new List<string>
        {
            "正弦信号", "三角波信号", "方波信号",
            "BPSK", "QPSK", "PSK8", "QAM16", "QAM32", "QAM64", "QAM128",
            "调幅信号", "线性调频信号", "正弦调频信号", "脉冲调制雷达信号"
        };
        // 样式标志
        public bool StylizeFlag { get; set; } = true;

        // Presenter 属性
        public FilePrsnt Presenter { get; set; }

        public AiExportPage()
        {
            InitializeComponent(); // 现在这里恢复调用 Designer.cs 中的方法

            // 初始化备选源
            cmbTag.Items.Clear();
            foreach (var type in _supportedSignalTypes) cmbTag.Items.Add(type);

            // 运行时加载上一次保存的列表
            LoadUserSignalTypes();

            // 初始化默认数据
            if (cmbChannel.Items.Count > 0) cmbChannel.SelectedIndex = 0;
            if (cmbTag.Items.Count > 0) cmbTag.SelectedIndex = 0;
            if (cmbUsage.Items.Count > 0) cmbUsage.SelectedIndex = 0;
            if (cmbFormat.Items.Count > 0) cmbFormat.SelectedIndex = 0;
            if (cmbtype.Items.Count > 0) cmbtype.SelectedIndex = 0;
        }

        public void UpdateView(object presenter, string propertyName)
        {
            // 预留更新接口
        }

        #region 属性封装
        public ChannelId SelectedChannel
        {
            get
            {
                switch (cmbChannel.SelectedIndex)
                {
                    case 0: return ChannelId.C1;
                    case 1: return ChannelId.C2;
                    case 2: return ChannelId.C3;
                    case 3: return ChannelId.C4;
                    default: return ChannelId.C1;
                }
            }
        }

        public string BaseFileName => txtFileName.Text.Trim();
        public string AuthorName => txtAuthor.Text.Trim();
        public int CaptureCount => (int)numCount.Value;
        public string LabelTag => cmbTag.Text;
        public int IntervalMs => (int)numInterval.Value;
        public string UsageType
        {
            get
            {
                string val = cmbUsage.Text;
                if (val.Contains(" ")) return val.Split(' ')[0];
                return val;
            }
        }
        public string UserNote => txtNote.Text;
        public int ExportFormatIndex => cmbFormat.SelectedIndex;
        #endregion

        #region 按钮事件
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(BaseFileName))
            {
                MessageBox.Show("请输入文件前缀名称！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "请选择数据集保存根目录";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string savePath = System.IO.Path.Combine(fbd.SelectedPath, UsageType);
                    btnStart.Enabled = false;
                    btnStart.Text = "采集运行中...";

                    PerformDataCollection(
                        savePath, BaseFileName, AuthorName, CaptureCount,
                        LabelTag, SelectedChannel, null, IntervalMs, UsageType, UserNote, ExportFormatIndex
                    );
                }
            }
        }
        private void btnaddtype_Click(object sender, EventArgs e)
        {
            string selectedType = cmbTag.Text.Trim();

            if (!_supportedSignalTypes.Contains(selectedType))
            {
                MessageBox.Show("不支持的类型！", "提示");
                return;
            }

            bool exists = false;
            foreach (var item in cmbtype.Items)
            {
                if (item.ToString() == selectedType) { exists = true; break; }
            }

            if (!exists)
            {
                cmbtype.Items.Add(selectedType);
                cmbtype.SelectedIndex = cmbtype.Items.Count - 1;

                SaveUserSignalTypes();
            }
        }

        private void btndeletetype_Click(object sender, EventArgs e)
        {
            if (cmbtype.SelectedIndex != -1)
            {
                cmbtype.Items.RemoveAt(cmbtype.SelectedIndex);
                if (cmbtype.Items.Count > 0) cmbtype.SelectedIndex = 0;

                SaveUserSignalTypes();
            }
        }
        #endregion

        #region 业务核心逻辑
        public class AiDatasetMetadata
        {
            public string Version { get; set; } = "1.0";
            public string CreatedAt { get; set; }
            public string Author { get; set; }
            public string FileName { get; set; }
            public double SamplingRate { get; set; }
            public int NumPoints { get; set; }
            public double AmpMax { get; set; }
            public double AmpMin { get; set; }
            public string SignalType { get; set; }
            public double? NoisePower { get; set; }
            public double? Snr { get; set; }
            public double TriggerIndex { get; set; }
            public Dictionary<string, double> Features { get; set; } = new Dictionary<string, double>();
            public Dictionary<string, object> ExtendedInfo { get; set; } = new Dictionary<string, object>();
        }

        internal async void PerformDataCollection(string folder, string baseName, string author, int count, string label, ChannelId channel, object measPresenter, int intervalMs, string usage, string note, int formatIndex)
        {
            if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);

            string readmePath = System.IO.Path.Combine(folder, "README.txt");
            if (!System.IO.File.Exists(readmePath))
            {
                System.IO.File.WriteAllText(readmePath, $"AI Dataset Created at {DateTime.Now}\r\nAuthor: {author}\r\nNote: {note}");
            }

            await Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        double realSampleRate;
                        double trigIdx;
                        float[] realData = GetWaveformData(channel, out realSampleRate, out trigIdx);

                        if (realData.Length == 0) continue;

                        // 修复：生成文件名时不带后缀，防止 .bin.bin
                        string fileName = $"{baseName}_{channel}_{label}_{i:D4}";
                        string fullPathNoExt = System.IO.Path.Combine(folder, fileName);

                        var metadata = new AiDatasetMetadata
                        {
                            Version = "1.0",
                            CreatedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.ffffff"),
                            Author = author,
                            FileName = fileName + ".bin", // 元数据里记录完整文件名
                            SamplingRate = (long)Math.Round(realSampleRate),
                            NumPoints = realData.Length,
                            AmpMax = realData.Max(),
                            AmpMin = realData.Min(),
                            SignalType = label,
                            TriggerIndex = trigIdx
                        };

                        PopulateHardwareInfo(channel, metadata.ExtendedInfo);
                        metadata.ExtendedInfo["Usage"] = usage;
                        metadata.ExtendedInfo["UserNote"] = note;

                        //SaveSingleFrame(fullPathNoExt, realData, metadata);
                        if (formatIndex == 0)
                        {
                            SaveSingleFrame(fullPathNoExt, realData, metadata);
                        }
                        else
                        {
                            SaveAsTxt(fullPathNoExt, realData, metadata);
                        }

                        int wait = intervalMs > 10 ? intervalMs : 10;
                        System.Threading.Thread.Sleep(wait);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Frame {i} failed: {ex.Message}");
                    }
                }
            });

            if (InvokeRequired)
            {
                Invoke(new Action(() => {
                    btnStart.Enabled = true;
                    btnStart.Text = "开始采集导出";
                    MessageBox.Show($"采集完成！\r\n已保存 {count} 组数据。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                btnStart.Enabled = true;
                btnStart.Text = "开始采集导出";
                MessageBox.Show($"采集完成！\r\n已保存 {count} 组数据。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private float[] GetWaveformData(ChannelId channelId, out double currentSampleRate, out double triggerIndex)
        {
            triggerIndex = 0.0;
            var pack = GetChannelDataFromGlobal(channelId, out currentSampleRate);
            if (pack == null || pack.Buffer == null) return new float[0];

            try
            {
                if (pack.Properties != null) triggerIndex = pack.Properties.TmbPosition.Index;
                int length = pack.Buffer.GetLength(1);
                float[] result = new float[length];
                for (int j = 0; j < length; j++) result[j] = (float)pack.Buffer[0, j];
                return result;
            }
            catch { return new float[0]; }
        }

        private WfmPack GetChannelDataFromGlobal(ChannelId id, out double sampleRate)
        {
            sampleRate = 0.0;
            try
            {
                var dsoModelType = Type.GetType("ScopeX.Core.DsoModel, ScopeX.Core") ?? Type.GetType("ScopeX.Core.DsoModel");
                if (dsoModelType == null) return null;

                var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
                object dsoModelInstance = null;
                var defaultProp = dsoModelType.GetProperty("Default", flags);
                if (defaultProp != null) dsoModelInstance = defaultProp.GetValue(null);
                else
                {
                    var defaultField = dsoModelType.GetField("Default", flags);
                    if (defaultField != null) dsoModelInstance = defaultField.GetValue(null);
                }
                if (dsoModelInstance == null) return null;

                var getChannelMethod = dsoModelType.GetMethod("GetChannel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(ChannelId) }, null);
                if (getChannelMethod == null) return null;
                var channelObj = getChannelMethod.Invoke(dsoModelInstance, new object[] { id });
                if (channelObj == null) return null;

                // 获取采样率
                try
                {
                    var samplingProp = SafeGetPropValue(channelObj, "Sampling");
                    if (samplingProp != null)
                    {
                        var rateProp = SafeGetPropInfo(samplingProp.GetType(), "AnalogSamplingRate");
                        if (rateProp != null) sampleRate = Convert.ToDouble(rateProp.GetValue(samplingProp));
                    }
                }
                catch { }

                var packProp = SafeGetPropInfo(channelObj.GetType(), "Pack");
                if (packProp == null) return null;
                return packProp.GetValue(channelObj) as WfmPack;
            }
            catch { return null; }
        }

        private void SaveSingleFrame(string pathNoExt, float[] data, AiDatasetMetadata meta)
        {
            using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Open(pathNoExt + ".bin", System.IO.FileMode.Create)))
            {
                foreach (var p in data) writer.Write(p);
            }
            var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            System.IO.File.WriteAllText(pathNoExt + ".json", JsonSerializer.Serialize(meta, options));
        }

        private void SaveAsTxt(string pathNoExt, float[] data, AiDatasetMetadata meta)
        {
            string txtPath = pathNoExt + ".txt";
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(txtPath))
            {
                // 1. 写入 Header (元数据)
                sw.WriteLine("=== Header ===");
                sw.WriteLine($"Version: {meta.Version}");
                sw.WriteLine($"CreatedAt: {meta.CreatedAt}");
                sw.WriteLine($"Author: {meta.Author}");
                sw.WriteLine($"FileName: {meta.FileName}");
                sw.WriteLine($"Label: {meta.SignalType}");
                sw.WriteLine($"SamplingRate: {meta.SamplingRate}");
                sw.WriteLine($"NumPoints: {meta.NumPoints}");
                sw.WriteLine($"AmpMax: {meta.AmpMax}");
                sw.WriteLine($"AmpMin: {meta.AmpMin}");
                sw.WriteLine($"SignalType: {meta.SignalType}");
                sw.WriteLine($"NoisePower: {(meta.NoisePower.HasValue ? meta.NoisePower.Value.ToString() : "null")}");
                sw.WriteLine($"Snr: {(meta.Snr.HasValue ? meta.Snr.Value.ToString() : "null")}");
                sw.WriteLine($"TriggerIndex: {meta.TriggerIndex}");

                // 写入特征值 (Features)
                if (meta.Features != null)
                {
                    foreach (var kvp in meta.Features)
                        sw.WriteLine($"Feature_{kvp.Key}: {kvp.Value}");
                }

                // 写入扩展信息 (Hardware Info 等)
                if (meta.ExtendedInfo != null)
                {
                    foreach (var kvp in meta.ExtendedInfo)
                    {
                        // 过滤掉 UserNote，避免和底部的 Note 重复
                        if (kvp.Key != "UserNote")
                            sw.WriteLine($"Info_{kvp.Key}: {kvp.Value}");
                    }
                }
                // 单独写入 Note
                string note = meta.ExtendedInfo.ContainsKey("UserNote") ? meta.ExtendedInfo["UserNote"].ToString() : "无";
                sw.WriteLine($"Note: {note}");

                sw.WriteLine("=== Data ===");

                // 2. 写入数据 (每行一个点，保留6位小数)
                for (int i = 0; i < data.Length; i++)
                {
                    sw.WriteLine(data[i].ToString("F6"));
                }
            }
        }
        private void PopulateHardwareInfo(ChannelId id, Dictionary<string, object> info)
        {
            try
            {
                var dsoModelType = Type.GetType("ScopeX.Core.DsoModel, ScopeX.Core") ?? Type.GetType("ScopeX.Core.DsoModel");
                if (dsoModelType == null) return;
                // ... 省略重复的获取 Instance 代码 (与 GetChannelDataFromGlobal 类似) ...
                // 为了简洁，这里直接复用反射逻辑，实际开发中建议提取公共方法
                var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
                object dsoInstance = dsoModelType.GetProperty("Default", flags)?.GetValue(null) ?? dsoModelType.GetField("Default", flags)?.GetValue(null);
                if (dsoInstance == null) return;

                var getChMethod = dsoModelType.GetMethod("GetChannel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(ChannelId) }, null);
                var chObj = getChMethod?.Invoke(dsoInstance, new object[] { id });
                if (chObj == null) return;

                var condObj = SafeGetPropValue(chObj, "Conditioning");
                if (condObj != null)
                {
                    var scaleVal = SafeGetPropValue(condObj, "Scale");
                    if (scaleVal != null)
                    {
                        info["VerticalScale_Val"] = scaleVal;
                        var pfx = SafeGetPropValue(condObj, "Prefix");
                        var unit = SafeGetPropValue(condObj, "Unit");
                        info["VerticalScale_Desc"] = $"{scaleVal} {pfx} {unit}";
                    }
                    var bias = SafeGetPropValue(condObj, "BiasByuV");
                    if (bias != null) info["VerticalOffset_uV"] = bias;
                }

                var tbObj = SafeGetPropValue(dsoInstance, "Timebase");
                if (tbObj != null)
                {
                    var tbScale = SafeGetPropValue(tbObj, "Scale");
                    if (tbScale != null) info["TimebaseScale"] = tbScale;
                }
            }
            catch { }
        }

        private object SafeGetPropValue(object target, string propName)
        {
            if (target == null) return null;
            return SafeGetPropInfo(target.GetType(), propName)?.GetValue(target);
        }

        private PropertyInfo SafeGetPropInfo(Type type, string propName)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(p => p.Name == propName);
        }
        #endregion

        #region 新增功能实现逻辑

        // 加载配置
        private void LoadUserSignalTypes()
        {
            cmbtype.Items.Clear();

            if (System.IO.File.Exists(_configFilePath))
            {
                string[] savedTypes = System.IO.File.ReadAllLines(_configFilePath);
                foreach (var t in savedTypes)
                {
                    if (!string.IsNullOrWhiteSpace(t))
                        cmbtype.Items.Add(t.Trim());
                }
            }

            // 如果文件不存在或内容为空，加载初始默认项
            if (cmbtype.Items.Count == 0)
            {
                cmbtype.Items.Add("正弦信号");
                cmbtype.Items.Add("三角波信号");
                cmbtype.Items.Add("方波信号");
            }
        }

        // 保存配置
        private void SaveUserSignalTypes()
        {
            var currentItems = cmbtype.Items
                .Cast<object>()
                .Select(item => item?.ToString()?.Trim() ?? String.Empty)
                .Where(item => !String.IsNullOrWhiteSpace(item))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            lock (ScopeX.Core.ArtificialIntelligenceModel.ConfigFileLock)
            {
                System.IO.File.WriteAllLines(_configFilePath, currentItems);
            }

            System.Diagnostics.Debug.WriteLine("AI配置已成功保存到磁盘。");
        }
        #endregion

    }
}