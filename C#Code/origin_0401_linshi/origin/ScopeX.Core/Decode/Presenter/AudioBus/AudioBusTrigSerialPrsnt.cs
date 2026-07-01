using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class AudioBusTrigSerialPrsnt : TrigSerialPrsnt
    {
        private protected override AudioBusTrigSerialModel Model
        {
            get;
        }

        public AudioBusTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.AudioBus, view)
        {
            Model = (AudioBusTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.AudioBus);
            LoadEvent();
        }

        /// <summary>
        /// 重载参数
        /// </summary>
        public override void LoadEvent()
        {
            if (Model != null)
            {
                Model.PropertyChanged += OnPropertyChanged;
            }
        }

        /// <summary>
        /// 切换类型，注销事件
        /// </summary>
        public override void DisposeEvent()
        {
            if (Model != null)
            {
                Model.PropertyChanged -= OnPropertyChanged;
            }
        }

        public ProtocolAudioBus.Condition Condition { get => Model.Condition; set => Model.Condition = value.Clamp(); }
        public ProtocolAudioBus.Condition4TDM ConditionTDM { get => Model.ConditionTDM; set => Model.ConditionTDM = value.Clamp(); }

        private String _ConditionName = nameof(Condition);

        public new String ConditionName
        {
            get { return _ConditionName; }
            set { _ConditionName = value; OnPropertyChanged(null, new System.ComponentModel.PropertyChangedEventArgs(nameof(ConditionName))); }
        }

        public ProtocolAudioBus.DataRelation DataRelation
        {
            get => Model.DataRelation;
            set => Model.DataRelation = value.Clamp();
        }
        public UInt32 MaxData => Model.MaxData;
        public UInt32 MinData => Model.MinData;
        public UInt32 Data
        {
            get => Model.Data;
            set => Model.Data = Math.Clamp(value, MinData, MaxData);
        }

        public UInt32 MinTDMChannelID => Model.MinTDMChannelID;
        public UInt32 MaxTDMChannelID => Model.MaxTDMChannelID;

        /// <summary>
        /// TDM时选择的音频通道
        /// </summary>
        public UInt32 TDMChannelID
        {
            get => Model.TDMChannelID;
            set { Model.TDMChannelID = Math.Clamp(value, MinTDMChannelID, MaxTDMChannelID); }
        }

        public ProtocolAudioBus.SoundChannel SoundChannel
        {
            get => Model.SoundChannel;
            set { Model.SoundChannel = value; }
        }
    }
}
