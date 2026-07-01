using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using System;
using System.ComponentModel;
using System.Drawing;

namespace ScopeX.Core
{
    public class ExceptionCapturePrsnt : MulticastPrsnt<IExceptionCaptureView>, IExceptionCapturePrsnt, IBadge
    {
        private protected override ExceptionCaptureModel Model
        {
            get;
        }


        #region IBadge接口

        public Boolean Active
        {
            get => Model.Active;
            set => Model.Active = value;
        }

        public ChannelId Id => Model.Id;

        public ChannelType Type => Model.Type;

        public String Name => Model.Name;

        public Color DrawColor
        {
            get => Model.DrawColor;
            set => Model.DrawColor = value;
        }

        #endregion

        public ExceptionCapturePrsnt(IDsoPrsnt idp, IExceptionCaptureView? view) : base(idp)
        {
            Model = DsoModel.Default.ExceptionCapture;
            Model.PropertyChanged += OnPropertyChanged;

            if (view is not null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

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
                if (Model.CaptureExceptionEnable != value)
                {
                    Model.CaptureExceptionEnable = value;
                    Hardware.HdCmdFactory.Push(HdCmd.ExceptionCapture);
                }
            }
        }

        public TemplateTriggerSourceEnum TemplateTriggerSource
        {
            get => Model.TemplateTriggerSource;
            set
            {
                if (Model.TemplateTriggerSource != value)
                {
                    Model.TemplateTriggerSource = value;
                    Hardware.HdCmdFactory.Push(HdCmd.ExceptionCapture);
                }
            }
        }

        public int CaptureExceptionFrameLength
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
            Hardware.HdCmdFactory.Push(HdCmd.ExceptionCapture);
        }

        public ExceptionViewMode ExceptionViewMode
        {
            get => Model.ExceptionViewMode;
            set => Model.ExceptionViewMode = value;
        }

        public Int32 MaxAnormlFrameId => Model.MaxAnormlFrameId;
        public Int32 MinAnormlFrameId => Model.MinAnormlFrameId;

        public Int32 AnormalFrameID
        {
            get => Model.AnormalFrameID;
            set => Model.AnormalFrameID = value;
        }

        public UInt32 AnormalFrameCount
        {
            get => Model.AnormalFrameCount;
            set => Model.AnormalFrameCount = value;
        }


        public void ExportCaptureWave2File()
        {
            Model.Export2FileCnt++;
            Hardware.HdCmdFactory.Push(HdCmd.ExceptionCapture);
        }

        public Boolean StudyStatus => Model.StudyStatus;

        public Boolean ClearFlag
        {
            get => Model.ClearFlag;
            set => Model.ClearFlag = value;
        }

        public void WfmStudy()
        {
            //学习之前将触发电平复位（波形中间）
            if (TriggerPrsnt.GetCurrentTrigger() is TrigEdgePrsnt tedp)
            {
                tedp.SetPosIndexCenter();
            }
            if (TriggerPrsnt.GetCurrentTrigger() is TrigSingleSrcPrsnt tssp)
            {
                tssp.SetPosIndexCenter();
            }
            Hd.TrySetExcuteAction(ChannelType.EmdProcess, nameof(WfmStudy));
        }

        public void ExcuteExCapture()
        {
            Hd.TrySetExcuteAction(ChannelType.EmdProcess, nameof(ExcuteExCapture));
        }
    }
}
