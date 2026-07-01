using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Model.ArtificialIntelligence.AiSet
{
    internal class SimpleSignal : BaseSignal
    {

        public SimpleSignal() { }

        public SimpleSignal(String type) 
        {
            base.type = type;
        }

        public override void AiSet()
        {
            DsoModel.Default.ArtificialIntelligence.CurAiSetEnable = true;
            DsoModel.Default.MultiDomain.Active = false;
            DsoPrsnt.DefaultDsoPrsnt.VectorAnalysis.Enabled = false;

            switch (NormalizedType)
            {
                case "Sine":
                    //DsoPrsnt.DefaultDsoPrsnt.ArtificialIntelligence.CurNoiseRedutionMethod = NoiseRedutionMethod.Average;
                    //DsoPrsnt.DefaultDsoPrsnt.ArtificialIntelligence.CurAINoiseReductionEnable = true;
                    //AppendAiTip("识别为正弦信号，已启用平均降噪并聚焦时域观察");
                    break;
                case "Square":
                    //DsoPrsnt.DefaultDsoPrsnt.ArtificialIntelligence.CurNoiseRedutionMethod = NoiseRedutionMethod.Close;
                    //DsoPrsnt.DefaultDsoPrsnt.ArtificialIntelligence.CurAINoiseReductionEnable = false;
                    //AppendAiTip("识别为方波信号，已关闭降噪以保留边沿细节");
                    break;
                case "Tri":
                    //DsoPrsnt.DefaultDsoPrsnt.ArtificialIntelligence.CurNoiseRedutionMethod = NoiseRedutionMethod.TimeDomainFilter;
                    //DsoPrsnt.DefaultDsoPrsnt.ArtificialIntelligence.CurAINoiseReductionEnable = true;
                    //AppendAiTip("识别为三角波信号，已启用时域滤波以稳定斜率观测");
                    break;
                default:
                    //DsoPrsnt.DefaultDsoPrsnt.ArtificialIntelligence.CurNoiseRedutionMethod = NoiseRedutionMethod.Close;
                    //DsoPrsnt.DefaultDsoPrsnt.ArtificialIntelligence.CurAINoiseReductionEnable = false;
                    //AppendAiTip($"未命中预设动作，按基础模拟信号策略处理：{NormalizedType}");
                    break;
            }
        }
    }
}
