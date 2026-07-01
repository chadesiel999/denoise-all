using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core.Model.ArtificialIntelligence.AiSet
{
    internal class RadarSignal : BaseSignal
    {
        public override void AiSet()
        {
            DsoModel.Default.MultiDomain.Active = true;
            DsoModel.Default.ArtificialIntelligence.CurAiSetEnable = true;
            DsoModel.Default.MultiDomain.ParameterTuningEnable = true;
            Thread.Sleep(100);

            switch (NormalizedType)
            {
                case "SFM":
                case "LFM":
                    DsoModel.Default.MultiDomain.CurFigureType = MultiDomainFigureEnum.AmpleVsFreq;
                    DsoModel.Default.MultiDomain.CurFigureEnable = true;
                    Thread.Sleep(100);
                    DsoModel.Default.MultiDomain.CurFigureType = MultiDomainFigureEnum.Spectrogram;
                    DsoModel.Default.MultiDomain.CurFigureEnable = true;
                    Thread.Sleep(100);
                    DsoModel.Default.MultiDomain.TimeScaleForTimeFreq = 50e-9;
                    Thread.Sleep(300);
                    DsoPrsnt.DefaultDsoPrsnt.MultiDomain.ThreeDimensionalEnable = true;
                    //AppendAiTip("识别为SFM，已切换幅频图并打开三维时频观察");
                    break;
                case "AM":

                    break;
                case "Pulse":

                    break;
                default:
                    DsoModel.Default.MultiDomain.CurFigureType = MultiDomainFigureEnum.AmpleVsFreq;
                    DsoModel.Default.MultiDomain.CurFigureEnable = true;
                    AppendAiTip($"未命中预设动作，按通信信号基础策略处理：{NormalizedType}");
                    break;
            }
        }
    }
}
