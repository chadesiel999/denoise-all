using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    public class ArtificialIntelligencePrsnt : MulticastPrsnt<IArtificialIntelligenceView>, IArtificialIntelligencePrsnt
    {
        public ArtificialIntelligencePrsnt(IDsoPrsnt idp, IArtificialIntelligenceView? view) : base(idp)
        {
            Model = DsoModel.Default.ArtificialIntelligence;
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;
                TryAddView(view);
            }
        }

        private protected override ArtificialIntelligenceModel Model
        {
            get;
        }

        public AiModeEnum AiMode
        { 
            get => Model.AiMode;
            set
            {
                Model.AiMode = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public ChannelId AiSetChnlId
        { 
            get => Model.AiSetChnlId;
            set => Model.AiSetChnlId = value;
        }

        public void ActionAiSet()
        {
            // Left-click AiSet should execute immediately and reset cooldown in continuous mode.
            Model.RequestAiSet(AiSetActionType.Full, resetContinuousCooldown: true);
        }

        public void ActionAiSet(String signalTypeOverride)
        {
            Model.RequestAiSet(AiSetActionType.Full, resetContinuousCooldown: true, signalTypeOverride: signalTypeOverride);
        }

        public void ActionAiSetByScpi(String? signalTypeOverride = null)
        {
            Model.RequestAiSetFromScpi(signalTypeOverride);
        }

        public String GetAiSetScpiStatusJson()
        {
            return Model.GetAiSetScpiStatusJson();
        }

        public void ActionAiSetIdentifyOnly()
        {
            Model.RequestAiSet(AiSetActionType.IdentifyOnly);
        }

        public void ActionAiSetAutoScaleOnly()
        {
            Model.RequestAiSet(AiSetActionType.AutoScaleOnly);
        }

        public void ActionAiSetTimebaseOnly()
        {
            Model.RequestAiSet(AiSetActionType.TimebaseOnly);
        }

        public void ActionAiSetAcqResourceAdaptive()
        {
            Model.RequestAiSet(AiSetActionType.AcqResourceAdaptive);
        }

        public void ActionAiSetRestoreLast()
        {
            Model.RequestAiSet(AiSetActionType.RestoreLast);
        }

        public Boolean CanRestoreAiSet()
        {
            return Model.CanRestoreAiSet();
        }

        public Boolean ContinuousAiSetEnabled
        {
            get => Model.ContinuousAiSetEnabled;
            set => Model.ContinuousAiSetEnabled = value;
        }

        public void ActionModelBuild()
        {
            Model.ModelBuildCnt++;
        }

        public Boolean ModelLearning
        {
            get => Model.ModelLearning;
            set => Model.ModelLearning = value;
        }

        public Boolean CurAiSetEnable
        { 
            get => Model.CurAiSetEnable;
            set => Model.CurAiSetEnable = value;
        }

        public Boolean CurAiSignalRecognitionEnable
        {
            get => Model.CurAiSignalRecognitionEnable;
            set => Model.CurAiSignalRecognitionEnable = value;
        }

        public Boolean CurAiWindowsEnable
        {
            get => Model.CurAiWindowsEnable;
            set => Model.CurAiWindowsEnable = value;
        }

        public Boolean CurAiParamsEnable
        {
            get => Model.CurAiParamsEnable;
            set => Model.CurAiParamsEnable = value;
        }


        public ChannelId ReCfgDbiChnlId
        {
            get => Model.ReCfgDbiChnlId;
            set => Model.ReCfgDbiChnlId = value;
        }

        public Boolean ReconfigurableDBIEnable
        {
            get => Model.ReconfigurableDBIEnable;
            set
            {
                Model.ReconfigurableDBIEnable = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public ChannelId AIUnionChnlId
        {
            get => Model.AIUnionChnlId;
            set => Model.AIUnionChnlId = value;
        }

        public Boolean ReconfigDbiUnionEnable
        {
            get => Model.ReconfigDbiUnionEnable;
            set
            {
                Model.AutoFilterEnable = value;
                Model.ReconfigurableDBIEnable = value;
                Model.ReconfigDbiUnionEnable = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public SubbandCtrlMethod CurSubbandCtrlMethod
        {
            get => Model.CurSubbandCtrlMethod;
            set
            {
                Model.CurSubbandCtrlMethod = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public List<Int32> SubbandTable
        {
            get => Model.SubbandTable;
        }

        public UInt32 SubbandsEnable
        {
            get => Model.SubbandsEnable;
            set
            {
                if (Model.SubbandsEnable != value)
                {
                    Model.SubbandsEnable = value;
                    Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
                }
            }
        }

        public Int32 PrecisionSubbandId
        {
            get => Model.PrecisionSubbandId;
            set => Model.PrecisionSubbandId = value;
        }

        public UInt32 CurSubbandBaseNoise
        {
            get => Model.CurSubbandBaseNoise;
            set
            {
                Model.CurSubbandBaseNoise = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public UInt64 CurLocalFreq
        {
            get => Model.CurLocalFreq;
            set
            {
                Model.CurLocalFreq = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public UInt64 CurBandFreqLimit
        {
            get => Model.CurBandFreqLimit;
            set
            {
                Model.CurBandFreqLimit = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public UInt64 LeftFreqByHz
        {
            get => Model.LeftFreqByHz;
            set
            { 
                Model.LeftFreqByHz = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public UInt64 RightFreqByHz
        {
            get => Model.RightFreqByHz;
            set
            {
                Model.RightFreqByHz = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Boolean CaptureExceptionUnionEnable
        {
            get => Model.CaptureExceptionUnionEnable;
            set
            {
                Model.TemplateBuildCnt++;
                Model.CaptureExceptionEnable = value;
                Model.CaptureExceptionUnionEnable = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Boolean AutoFilterEnable
        {
            get => Model.AutoFilterEnable;
            set
            {
                Model.AutoFilterEnable = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Boolean IterFilterEnable
        {
            get => Model.IterFilterEnable;
            set
            {
                Model.IterFilterEnable = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public UInt32 CriticalFreq
        {
            get => Model.CriticalFreq;
            set
            {
                Model.CriticalFreq = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Boolean AutoCfgAnaChnlBitWidthEnable
        {
            get => Model.AutoCfgAnaChnlBitWidthEnable;
            set
            {
                Model.AutoCfgAnaChnlBitWidthEnable = value;
                //Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Int32 AnaChnlBitWidth
        {
            get => Model.AnaChnlBitWidth;
            set
            {
                Model.AnaChnlBitWidth = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Int32[] AnaChnlBitWidthDefine
        {
            get => Model.AnaChnlBitWidthDefine.ToArray();
        }

        #region 智能降噪
        public ChannelId NoiseReductionChnlId
        {
            get => Model.NoiseReductionChnlId;
            set => Model.NoiseReductionChnlId = value;
        }

        public Boolean CurAINoiseReductionEnable
        {
            get => Model.CurAINoiseReductionEnable;
            set
            {
                Model.CurAINoiseReductionEnable = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public void AverageEnable(Boolean value) 
        {
            Model.AverageEnable = value;
            Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
        }

        public NoiseRedutionMethod CurNoiseRedutionMethod
        {
            get => Model.CurNoiseRedutionMethod;
            set => Model.CurNoiseRedutionMethod = value;
        }

        public Int32 MaxAverageCount
        { 
            get => Model.MaxAverageCount;
            set => Model.MaxAverageCount = value;
        }

        public void ResetAverageNoiseRedution()
        {
            Model.ResetAverageNoiseRedution();
        }
        #endregion

        #region 异常捕获模块
        public ChannelId ExceptionCaptureChnlId
        {
            get => Model.ExceptionCaptureChnlId;
            set => Model.ExceptionCaptureChnlId = value;
        }

        public Boolean CaptureExceptionEnable
        {
            get => Model.CaptureExceptionEnable;
            set
            {
                Model.CaptureExceptionEnable = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public TemplateTriggerSourceEnum TemplateTriggerSource
        {
            get => Model.TemplateTriggerSource;
            set
            {
                Model.TemplateTriggerSource = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Int32 CaptureExceptionFrameLength
        {
            get => Model.CaptureExceptionFrameLength;
            set
            {
                Model.CaptureExceptionFrameLength = value;
                BuildTemplate();
            }
        }

        public void BuildTemplate()
        {
            Model.TemplateBuildCnt++;
            Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
        }

        public ExceptionViewMode CurExceptionViewMode
        {
            get => Model.CurExceptionViewMode;
            set => Model.CurExceptionViewMode = value;
        }

        public UInt32 MaxAnormlFrameId => Model.MaxAnormlFrameId;
        public UInt32 MinAnormlFrameId => Model.MinAnormlFrameId;

        public UInt32 CurAnormlFrameId
        {
            get => Model.CurAnormlFrameId;
            set => Model.CurAnormlFrameId = value;
        }

        public void ExportCaptureWave2File()
        {
            Model.Export2FileCnt++;
            Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
        }
        #endregion

        public ChannelId TemplateTriggerChnlId
        {
            get => Model.TemplateTriggerChnlId;
            set => Model.TemplateTriggerChnlId = value;
        }

        public Boolean FramworkDetectEnable
        {
            get => Model.FramworkDetectEnable;
            set
            {
                Model.FramworkDetectEnable = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Boolean MainEnable
        {
            get => Model.MainEnable;
        }

        public Boolean AiSetEnable
        {
            get => Model.AiSetEnable;
            set => Model.AiSetEnable = value;
        }

        public String[] AiSetInfo
        {
            get => Model.AiSetInfo;
        }

        public String[] AiTipInfo
        {
            get => Model.AiTipInfo;
        }

        public Int64 AiTipVersion
        {
            get => Model.AiTipVersion;
        }

        

        public TemplateSourceEnum TemplateSource
        {
            get => Model.TemplateSource;
            set
            {
                Model.TemplateSource = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public UInt32 FrameIdForTrig
        {
            get => Model.FrameIdForTrig;
            set
            {
                Model.FrameIdForTrig = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public UInt32 TemplateOffset
        {
            get => Model.TemplateOffset;
            set
            {
                Model.TemplateOffset = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Int32 UserDefinePosStart
        {
            get => Model.UserDefinePosStart;
            set
            {
                Model.UserDefinePosStart = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public UInt32 FrameTrigDataLen
        {
            get => Model.FrameTrigDataLen;
            set
            { 
                Model.FrameTrigDataLen = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public void SendTemplateTrigger()
        {
            Model.TemplateTriggerSendCnt++;
            Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
        }
    }
}
