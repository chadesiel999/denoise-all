using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core.Model.ArtificialIntelligence.AiSet
{
    internal class CommunicationSignal : BaseSignal
    {
        public CommunicationSignal() { }

        public CommunicationSignal(String type) 
        {
            base.type = type;
        }

        public override void AiSet()
        {
            DsoPrsnt.DefaultDsoPrsnt.VectorAnalysis.Source = ResolveAnalogSource();
            DsoModel.Default.ArtificialIntelligence.CurAiSetEnable = true;
            DsoPrsnt.DefaultDsoPrsnt.VectorAnalysis.GenerateDigtalPrsnt.CurGraphType = VsaGraphType.Constellation;
            DsoPrsnt.DefaultDsoPrsnt.VectorAnalysis.GenerateDigtalPrsnt.EqualizerEnabled = true;

            VsaFormatOpt format = NormalizedType switch
            {
                "BPSK" => VsaFormatOpt.BPSK,
                "QPSK" => VsaFormatOpt.QPSK,
                "8PSK" => VsaFormatOpt.PSK8,
                "PSK8" => VsaFormatOpt.PSK8,
                "16QAM" => VsaFormatOpt.QAM16,
                "32QAM" => VsaFormatOpt.QAM32,
                "64QAM" => VsaFormatOpt.QAM64,
                "128QAM" => VsaFormatOpt.QAM128,
                _ => VsaFormatOpt.QAM16
            };
            DsoPrsnt.DefaultDsoPrsnt.VectorAnalysis.GenerateDigtalPrsnt.FormatOpt = format;

            Thread.Sleep(300);
            var curGraph = DsoPrsnt.DefaultDsoPrsnt.VectorAnalysis.GenerateDigtalPrsnt.GetCurVsaGraphPrsnt;
            if (curGraph != null)
                curGraph.Enabled = true;
            DsoPrsnt.DefaultDsoPrsnt.VectorAnalysis.Enabled = true;
            AppendAiTip($"识别为{NormalizedType}，已切换到矢量分析星座图并设置调制格式为{format}");
        }
    }
}
