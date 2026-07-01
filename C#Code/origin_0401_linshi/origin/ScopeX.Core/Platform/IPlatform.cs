using ScopeX.ComModel;
using ScopeX.Core.Decode;
using ScopeX.Core.Model.Jitter.Eye;
using System;
using System.Collections.Generic;
using System.Threading;
using static NPOI.HSSF.Util.HSSFColor;

namespace ScopeX.Core
{
    internal interface IPlatform
    {
        public ProductType ProductType { get; }

        public AnaChnlTimebaseIndex GetTimebaseMinIndex(AdcInterleaveMode adcInterleaveMode);
        public IReadOnlyList<(Int32 Index, String Name)> GetBandWidthNames(Boolean is2G, AnaChnlCoupling anaChnlCoupling, Boolean active, Boolean isForcedRead);
        public IReadOnlyList<KeyValuePair<String, Int32>> GetAnaChnlLengthSource(List<KeyValuePair<String, Int32>> source, Double timeScale, AdcInterleaveMode adcInterleaveMode);

        /// <summary>
        /// 获取当前条件下的偏置范围
        /// </summary>
        /// <param name="coupling">耦合方式</param>
        /// <param name="scale">垂直挡位</param>
        /// <returns></returns>
        public (Int32 MaxUv, Int32 MinUv) GetBiasRange(AnaChnlCoupling coupling, AnaChnlScaleIndex scale);

        /// <summary>
        /// 做LA功能的互斥操作
        /// </summary>
        /// <returns></returns>
        public Boolean DoLAMutex();

        public void SetBandWidthByInterleaveMode(AdcInterleaveMode adcInterleaveMode);

        /// <summary>
        /// added by hjli 
        /// 
        /// 适配不同型号对边沿触发与指示灯的联动
        /// </summary>
        public void SetEdgeTriggerLed(EdgeSlope slope);
        public void TriggerTypeChanged(TriggerType type);
        /// <summary>
        /// 获取DDr的分段数
        /// </summary>
        /// <param name="storageWaveDotsCnt">存储深度</param>
        /// <param name="adcInterleaveMode">交织模式</param>
        /// <returns></returns>
        public Int32 GetDDrMaxFrameCount(Int64 storageWaveDotsCnt, AdcInterleaveMode adcInterleaveMode);

        /// <summary>
        /// 获取该平台通道支持的耦合模式
        /// </summary>
        /// <param name="serailNumber"></param>
        /// <param name="storageMode"></param>
        /// <returns></returns>
        public IEnumerable<AnaChnlCoupling> GetSupportedCouplings(String serailNumber, AnaChnlStorageMode storageMode);

        /// <summary>
        /// 加载每个平台初始化的设置
        /// </summary>
        public void LoadOriginSetting();

        /// <summary>
        /// 获取UI绘制波形的点数
        /// </summary>
        public Int64 GetViewWaveDotsCnt(AdcInterleaveMode adcInterleaveMode);

        public String GetSNPrefix();
        /// <summary>
        /// 设置产品型号，如果设置，则产品型号不会根据序列号进行变化。不设置，则根据序列号变化。
        /// </summary>
        /// <param name="productModel"></param>
        public void SetProductModel(String productModel);
        /// <summary>
        /// 获取产品型号
        /// </summary>
        /// <returns></returns>
        public String GetProductModel();

        /// <summary>
        /// 支持选装的协议（目前几个项目是一样的，将来可能会有不同）
        /// </summary>
        /// <returns></returns>
        public IDictionary<SerialProtocolType, OptionType> OptionProtocols { get; }

        /// <summary>
        /// 获取选件激活码
        /// </summary>
        /// <param name="sn">序列号</param>
        /// <param name="name">选件名</param>
        /// <returns></returns>
        public String? GetOptionActiveCode(String sn, String name);

        /// <summary>
        /// Autoset专用 是否显示弱提示 
        /// </summary>
        public Boolean ShowWeakTip { get; }

        /// <summary>
        /// Autoset专用 是否默认高阻
        /// </summary>
        public Boolean DefaultHighImpedance { get; }

        /// <summary>
        /// 是否包含数字通道
        /// </summary>
        public Boolean IncludeDigitalChnl { get; }

        /// <summary>
        /// 是否能获取或设置屏幕亮度
        /// </summary>
        public Boolean EnableGetOrSetScreenBrightness { get; }

        /// <summary>
        /// 抖动眼图最大数据量
        /// </summary>
        public Int32 JitterMaxDataLength { get; }

        /// <summary>
        /// 是否需要带宽限制
        /// </summary>
        public Boolean LimitBandwidth { get; }

        /// <summary>
        /// 是否需要检查带宽
        /// </summary>
        public Boolean NeedCheckBandwidth { get; }

        /// <summary>
        /// 解码数据处理
        /// </summary>
        /// <param name="id">通道ID</param>
        /// <param name="bitCount">bitCount</param>
        /// <param name="datasource">数据源</param>
        /// <param name="root">父节点</param>
        /// <param name="tempnode">子节点</param>
        /// <param name="nodeindex">节点索引</param>
        /// <param name="token"></param>
        public void ProcessDecodeData(ChannelId id, Int32 bitCount, ref DeocodeDataSourcePacket datasource, ref TwoLevelEdgeInfo root, ref TwoLevelEdgeInfo tempnode, ref Int32 nodeindex, CancellationToken token);

        void ProbePrompt(ref Int32 readCount, String sn);

        /// <summary>
        /// Cursor多功能旋钮按键是否需要差异化处理
        /// </summary>
        Boolean KeyEnumCursor();

        /// <summary>
        /// Jitter互斥检测
        /// </summary>
        /// <param name="forceClose">是否强制关闭互斥功能</param>
        /// <returns></returns>
        Boolean JitterFunctionLimit(Boolean forceClose);

        IEnumerable<ChannelId> GetTriggerSource();

        /// <summary>
        /// 获取通道延迟点数
        /// </summary>
        /// <param name="uiPoint"></param>
        /// <param name="channelDelay"></param>
        /// <returns></returns>
        Int32 GetChannelDelayPoint(Double uiPoint, Double channelDelay);

        void GetEyeGraphParams(ref FastEyeParams fastEyeParams, Double averageUILength, Int32 dataHeight, Int32 dataWidth, Double[] interpolated_data, Double[] eye_ref_edges_array, out Double[,] matrix, out Double[] levhist);
    }
}
