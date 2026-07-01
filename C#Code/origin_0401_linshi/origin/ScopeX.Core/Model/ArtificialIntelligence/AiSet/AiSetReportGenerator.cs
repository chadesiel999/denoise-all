using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    internal sealed class AiSetReportGenerator
    {
        private static readonly HttpClient _LlmHttpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(45)
        };

        private readonly ArtificialIntelligenceModel _model;
        private readonly List<IAiSetReportTemplate> _templates;

        public AiSetReportGenerator(ArtificialIntelligenceModel model)
        {
            _model = model;
            _templates = new List<IAiSetReportTemplate>
            {
                new SineWaveSimulationReportTemplate(),
                new SquareWaveSimulationReportTemplate(),
                new TriangleWaveSimulationReportTemplate(),
                new Qam16SimulationReportTemplate()
            };
        }

        internal String GenerateReportJson(UInt32 requestId, UInt32 executedId)
        {
            String requestedSignalType = ResolveSignalType();
            IAiSetReportTemplate template = ResolveTemplate(requestedSignalType);
            String effectiveSignalType = template.SignalTypeKey;

            var context = template.BuildContext(new AiSetReportRequest(requestId, executedId, effectiveSignalType), _model);
            String prompt = template.BuildPrompt(context, effectiveSignalType);
            String report = GenerateAiSetReportWithLlm(prompt, context);

            return JsonSerializer.Serialize(new
            {
                status = true,
                requestId,
                executedRequestId = executedId,
                signalType = effectiveSignalType,
                generatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                context,
                prompt,
                report
            });
        }

        private IAiSetReportTemplate ResolveTemplate(String signalType)
        {
            foreach (var template in _templates)
            {
                if (template.CanHandle(signalType))
                    return template;
            }

            // 暂未适配的信号类型统一降级到16QAM模板，保证报告链路可用
            foreach (var template in _templates)
            {
                if (String.Equals(template.SignalTypeKey, "16QAM", StringComparison.OrdinalIgnoreCase))
                    return template;
            }

            return _templates[0];
        }

        private String ResolveSignalType()
        {
            try
            {
                ChannelId source = DsoModel.Default.VectorAnalysisModel.Source;
                String signalType = _model.GetSignalType(source);
                if (!String.IsNullOrWhiteSpace(signalType))
                    return signalType;
            }
            catch
            {
                // ignore and use fallback
            }
            return "16QAM";
        }

        private static String GenerateAiSetReportWithLlm(String prompt, Object reportContext)
        {
            // 如需切换到Gemini，可修改以下三项（baseUrl/apiKey/model）到对应值。
            String llmBaseUrl = "https://api.deepseek.com";
            String llmApiKey = "sk-fad431829c3d4c9db3d32989660d1dcb";
            String llmModel = "deepseek-chat";

            try
            {
                String llmUrl = $"{llmBaseUrl.TrimEnd('/')}/chat/completions";
                using var request = new HttpRequestMessage(HttpMethod.Post, llmUrl);
                if (!String.IsNullOrWhiteSpace(llmApiKey))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", llmApiKey);
                }

                var payload = new
                {
                    model = llmModel,
                    messages = new[]
                    {
                        new { role = "system", content = "你是一名专业射频与数字调制分析工程师。" },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.2
                };

                String content = JsonSerializer.Serialize(payload);
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
                var response = _LlmHttpClient.SendAsync(request).GetAwaiter().GetResult();
                String respText = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    String detail = String.IsNullOrWhiteSpace(respText)
                        ? String.Empty
                        : $" 响应：{respText}";
                    return BuildFallbackReport(reportContext, $"LLM调用失败，HTTP {(Int32)response.StatusCode} {response.ReasonPhrase ?? String.Empty}.{detail}");
                }

                if (TryExtractLlmContent(respText, out String llmContent))
                    return llmContent;

                return BuildFallbackReport(reportContext, "LLM响应格式未识别，已返回本地降级报告。");
            }
            catch (TaskCanceledException ex)
            {
                return BuildFallbackReport(reportContext, $"LLM调用超时：超过{_LlmHttpClient.Timeout.TotalSeconds:0}秒未完成。{ex.Message}");
            }
            catch (Exception ex)
            {
                return BuildFallbackReport(reportContext, $"LLM调用异常：{ex.Message}");
            }
        }

        private static Boolean TryExtractLlmContent(String responseText, out String content)
        {
            content = String.Empty;
            if (String.IsNullOrWhiteSpace(responseText))
                return false;

            try
            {
                using var doc = JsonDocument.Parse(responseText);
                var root = doc.RootElement;

                if (root.TryGetProperty("choices", out var choices)
                    && choices.ValueKind == JsonValueKind.Array
                    && choices.GetArrayLength() > 0)
                {
                    var first = choices[0];
                    if (first.TryGetProperty("message", out var message)
                        && message.TryGetProperty("content", out var messageContent))
                    {
                        content = messageContent.GetString() ?? String.Empty;
                        return !String.IsNullOrWhiteSpace(content);
                    }
                    if (first.TryGetProperty("text", out var textNode))
                    {
                        content = textNode.GetString() ?? String.Empty;
                        return !String.IsNullOrWhiteSpace(content);
                    }
                }

                if (root.TryGetProperty("content", out var contentNode))
                {
                    content = contentNode.GetString() ?? String.Empty;
                    return !String.IsNullOrWhiteSpace(content);
                }
                if (root.TryGetProperty("report", out var reportNode))
                {
                    content = reportNode.GetString() ?? String.Empty;
                    return !String.IsNullOrWhiteSpace(content);
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        private static String BuildFallbackReport(Object reportContext, String reason)
        {
            return $"【自动测试报告（降级）】{Environment.NewLine}" +
                   $"{reason}{Environment.NewLine}" +
                   $"已收集AiSet与矢量分析上下文，可用于人工复核。{Environment.NewLine}" +
                   $"上下文摘要：{JsonSerializer.Serialize(reportContext)}";
        }
    }

    internal readonly record struct AiSetReportRequest(UInt32 RequestId, UInt32 ExecutedId, String SignalType);

    internal interface IAiSetReportTemplate
    {
        String SignalTypeKey { get; }
        Boolean CanHandle(String signalType);
        Object BuildContext(AiSetReportRequest request, ArtificialIntelligenceModel model);
        String BuildPrompt(Object reportContext, String signalType);
    }

    internal sealed class Qam16SimulationReportTemplate : IAiSetReportTemplate
    {
        public String SignalTypeKey => "16QAM";

        public Boolean CanHandle(String signalType)
            => !String.IsNullOrWhiteSpace(signalType)
               && signalType.Replace(" ", String.Empty).Contains("16QAM", StringComparison.OrdinalIgnoreCase);

        public Object BuildContext(AiSetReportRequest request, ArtificialIntelligenceModel model)
        {
            return new
            {
                requestId = request.RequestId,
                executedId = request.ExecutedId,
                signalType = SignalTypeKey,
                aiSet = new
                {
                    enable = true,
                    signalRecognitionEnable = true,
                    windowsEnable = true,
                    paramsEnable = true,
                    tips = new[] { "仿真提示1：参数已自动识别", "仿真提示2：链路连通性测试中" },
                    aiSetInfos = new[] { "仿真信息：当前为写死测试数据" }
                },
                vectorAnalysis = new
                {
                    enable = true,
                    source = "C1",
                    template = "QAM16",
                    graphType = "Constellation",
                    format = "QAM",
                    symbolRateBaud = 10_000_000.0,
                    rollOffFactor = 0.35,
                    measureFilter = "RRC",
                    refFilter = "RRC",
                    filterPara = 0.35,
                    carryFreqHz = 2_400_000_000.0,
                    carryFreqErrorHz = 120.0,
                    frequencyDetect = true,
                    equalizerEnabled = true,
                    sampleRateHz = 1_000_000_000.0,
                    verticalScale = 0.5,
                    verticalOffset = 0.0,
                    metrics = new Dictionary<String, Object?>
                    {
                        ["EVMRms"] = new { value = 1.85, mean = 1.80, max = 2.10, min = 1.50 },
                        ["SNR"] = new { value = 31.20, mean = 31.00, max = 31.60, min = 30.50 },
                        ["FreqErrorHz"] = new { value = 120.0, mean = 115.0, max = 135.0, min = 98.0 }
                    }
                }
            };
        }

        public String BuildPrompt(Object reportContext, String signalType)
        {
            String contextJson = JsonSerializer.Serialize(reportContext, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            StringBuilder sb = new();
            sb.AppendLine("你是示波器与矢量信号分析专家。");
            sb.AppendLine($"请基于以下JSON生成“{signalType}信号测试报告”。");
            sb.AppendLine("要求：");
            sb.AppendLine("1. 使用中文输出，结构包含：测试概况、关键参数、关键指标、结果判读、风险与建议。");
            sb.AppendLine("2. 对EVM、SNR、频偏等指标进行可读性解读；缺失项请明确写“未获取”。");
            sb.AppendLine("3. 输出要简洁专业，适合直接写入自动化测试日志。");
            sb.AppendLine();
            sb.AppendLine("输入数据：");
            sb.AppendLine(contextJson);
            return sb.ToString();
        }
    }

    internal abstract class AnalogWaveSimulationReportTemplateBase : IAiSetReportTemplate
    {
        private readonly String[] _signalAliases;

        protected AnalogWaveSimulationReportTemplateBase(String signalTypeKey, params String[] signalAliases)
        {
            SignalTypeKey = signalTypeKey;
            _signalAliases = signalAliases ?? Array.Empty<String>();
        }

        public String SignalTypeKey { get; }

        public Boolean CanHandle(String signalType)
        {
            if (String.IsNullOrWhiteSpace(signalType))
                return false;

            String normalized = signalType.Replace(" ", String.Empty);
            foreach (String alias in _signalAliases)
            {
                if (normalized.Contains(alias, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        protected abstract Dictionary<String, Object?> BuildMeasuredMetrics(IReadOnlyDictionary<String, Double?> snapshotMetrics);

        private static ChannelId ResolveSnapshotSource()
        {
            MeasureModel meas = DsoModel.Default.Meas;
            if (meas.SnapshotSource.IsAnalog()
                && DsoModel.Default.TryGetChannel(meas.SnapshotSource, out var selectedChannel)
                && selectedChannel.Active)
            {
                return meas.SnapshotSource;
            }

            ChannelId vectorSource = DsoModel.Default.VectorAnalysisModel.Source;
            if (vectorSource.IsAnalog()
                && DsoModel.Default.TryGetChannel(vectorSource, out var vectorChannel)
                && vectorChannel.Active)
            {
                return vectorSource;
            }

            ChannelModel? firstActiveAnalog = DsoModel.Default.Channels.FirstOrDefault(ch => ch.Id.IsAnalog() && ch.Active);
            return firstActiveAnalog?.Id ?? ChannelId.C1;
        }

        private static IReadOnlyDictionary<String, Double?> TryGetSnapshotMetrics(ChannelId source, out String measurementStatus)
        {
            var metrics = new Dictionary<String, Double?>(StringComparer.OrdinalIgnoreCase);
            MeasureModel meas = DsoModel.Default.Meas;
            ChannelId oldSource = meas.SnapshotSource;
            try
            {
                meas.SnapshotSource = source;
                if (!meas.CalcSnapshotAllResult())
                {
                    measurementStatus = "未获取：快照计算失败或通道未激活。";
                    return metrics;
                }

                String[] names = meas.SnapShotTableNames;
                List<String> values = meas.SnapShotDataTable;
                Int32 count = Math.Min(names.Length, values.Count);
                for (Int32 i = 0; i < count; i++)
                {
                    String key = names[i];
                    String raw = values[i];
                    if (Double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out Double parsed))
                    {
                        metrics[key] = parsed >= Double.MaxValue * 0.5 ? null : parsed;
                    }
                    else
                    {
                        metrics[key] = null;
                    }
                }

                measurementStatus = "已获取：来自参数测量快照结果。";
                return metrics;
            }
            catch (Exception ex)
            {
                measurementStatus = $"未获取：测量异常（{ex.Message}）。";
                return metrics;
            }
            finally
            {
                meas.SnapshotSource = oldSource;
            }
        }

        protected static Object BuildMetricNode(IReadOnlyDictionary<String, Double?> snapshotMetrics, params String[] candidateKeys)
        {
            foreach (String key in candidateKeys)
            {
                if (snapshotMetrics.TryGetValue(key, out Double? value))
                {
                    return new { value, sourceKey = key };
                }
            }
            return new { value = (Double?)null, sourceKey = candidateKeys.Length > 0 ? candidateKeys[0] : String.Empty };
        }

        public Object BuildContext(AiSetReportRequest request, ArtificialIntelligenceModel model)
        {
            ChannelId measureSource = ResolveSnapshotSource();
            IReadOnlyDictionary<String, Double?> snapshotMetrics = TryGetSnapshotMetrics(measureSource, out String measurementStatus);

            return new
            {
                requestId = request.RequestId,
                executedId = request.ExecutedId,
                signalType = SignalTypeKey,
                aiSet = new
                {
                    enable = true,
                    signalRecognitionEnable = true,
                    windowsEnable = true,
                    paramsEnable = true,
                    tips = new[] { "仿真提示1：当前参数为写死测试数据", "仿真提示2：后续可切换为实时测量值" },
                    aiSetInfos = new[] { $"仿真信息：当前信号类型为{SignalTypeKey}" }
                },
                waveformAnalysis = new
                {
                    enable = true,
                    source = measureSource.ToString(),
                    template = SignalTypeKey,
                    sampleRateHz = 1_000_000_000.0,
                    timeScaleS = 1e-6,
                    verticalScaleV = 1.0,
                    verticalOffsetV = 0.0,
                    measurementStatus,
                    metrics = BuildMeasuredMetrics(snapshotMetrics),
                    metricNotes = new[]
                    {
                        "备注：系统可扩展测量参数包括 频率、周期、幅值、峰峰值、最大值、最小值、均值、均方根、占空比、上升时间、下降时间、过冲、脉宽、延迟、相位、抖动 等。"
                    }
                }
            };
        }

        public String BuildPrompt(Object reportContext, String signalType)
        {
            String contextJson = JsonSerializer.Serialize(reportContext, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            StringBuilder sb = new();
            sb.AppendLine("你是示波器波形分析专家。");
            sb.AppendLine($"请基于以下JSON生成“{signalType}信号测试报告”。");
            sb.AppendLine("要求：");
            sb.AppendLine("1. 使用中文输出，结构包含：测试概况、关键参数、关键指标、结果判读、风险与建议。");
            sb.AppendLine("2. 对频率、幅值、峰峰值、均方根、占空比等指标进行可读性解读；缺失项请明确写“未获取”。");
            sb.AppendLine("3. 输出要简洁专业，适合直接写入自动化测试日志。");
            sb.AppendLine();
            sb.AppendLine("输入数据：");
            sb.AppendLine(contextJson);
            return sb.ToString();
        }
    }

    internal sealed class SineWaveSimulationReportTemplate : AnalogWaveSimulationReportTemplateBase
    {
        public SineWaveSimulationReportTemplate()
            : base("正弦波", "正弦波", "sine", "sin")
        {
        }

        protected override Dictionary<String, Object?> BuildMeasuredMetrics(IReadOnlyDictionary<String, Double?> snapshotMetrics)
        {
            return new Dictionary<String, Object?>
            {
                ["FrequencyHz"] = BuildMetricNode(snapshotMetrics, "Freq"),
                ["PeriodS"] = BuildMetricNode(snapshotMetrics, "Period"),
                ["AmplitudeV"] = BuildMetricNode(snapshotMetrics, "Amplitude"),
                ["PeakToPeakV"] = BuildMetricNode(snapshotMetrics, "Pk2Pk"),
                ["RmsV"] = BuildMetricNode(snapshotMetrics, "RMS"),
                ["AverageV"] = BuildMetricNode(snapshotMetrics, "Average")
            };
        }
    }

    internal sealed class SquareWaveSimulationReportTemplate : AnalogWaveSimulationReportTemplateBase
    {
        public SquareWaveSimulationReportTemplate()
            : base("方波", "方波", "square", "pulse")
        {
        }

        protected override Dictionary<String, Object?> BuildMeasuredMetrics(IReadOnlyDictionary<String, Double?> snapshotMetrics)
        {
            return new Dictionary<String, Object?>
            {
                ["FrequencyHz"] = BuildMetricNode(snapshotMetrics, "Freq"),
                ["PeriodS"] = BuildMetricNode(snapshotMetrics, "Period"),
                ["AmplitudeV"] = BuildMetricNode(snapshotMetrics, "Amplitude"),
                ["PeakToPeakV"] = BuildMetricNode(snapshotMetrics, "Pk2Pk"),
                ["DutyCyclePercent"] = BuildMetricNode(snapshotMetrics, "Duty"),
                ["PositivePulseWidthS"] = BuildMetricNode(snapshotMetrics, "PWidth"),
                ["NegativePulseWidthS"] = BuildMetricNode(snapshotMetrics, "NWidth"),
                ["RiseTimeS"] = BuildMetricNode(snapshotMetrics, "Rise"),
                ["FallTimeS"] = BuildMetricNode(snapshotMetrics, "Fall")
            };
        }
    }

    internal sealed class TriangleWaveSimulationReportTemplate : AnalogWaveSimulationReportTemplateBase
    {
        public TriangleWaveSimulationReportTemplate()
            : base("三角波", "三角波", "triangle", "tri")
        {
        }

        protected override Dictionary<String, Object?> BuildMeasuredMetrics(IReadOnlyDictionary<String, Double?> snapshotMetrics)
        {
            return new Dictionary<String, Object?>
            {
                ["FrequencyHz"] = BuildMetricNode(snapshotMetrics, "Freq"),
                ["PeriodS"] = BuildMetricNode(snapshotMetrics, "Period"),
                ["AmplitudeV"] = BuildMetricNode(snapshotMetrics, "Amplitude"),
                ["PeakToPeakV"] = BuildMetricNode(snapshotMetrics, "Pk2Pk"),
                ["RmsV"] = BuildMetricNode(snapshotMetrics, "RMS"),
                ["RiseTimeS"] = BuildMetricNode(snapshotMetrics, "Rise"),
                ["FallTimeS"] = BuildMetricNode(snapshotMetrics, "Fall"),
                ["CycleAverageV"] = BuildMetricNode(snapshotMetrics, "CycAverage"),
                ["CycleRmsV"] = BuildMetricNode(snapshotMetrics, "CycRMS")
            };
        }
    }
}
