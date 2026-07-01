using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    public class DecodePrsnt : ChannelPrsnt
    {
        private Boolean _LoggersState => DecoderLogger.State;
        private protected override DecodeModel Model
        {
            get;
        }

        public DecodePrsnt(ChannelId id, IDsoPrsnt idp, ITimebasePrsnt tmbprsnt) : base(idp, null)
        {
            Model = (DecodeModel)DsoModel.Default.GetChannel(id);

            Model.PropertyChanged += OnPropertyChanged;

            Sampling = (TimebasePrsnt)tmbprsnt;
            Label = id.ToString();

            if (_LoggersState) 
            {
               
            }
        }

        public Boolean EventEnable
        {
            get => Model.EventEnable;
            set => Model.EventEnable = value;
        }
        #region 通道属性

        public Double PositionBymV
        {
            get => Model.Conditioning.Position;
            //set => Model.Conditioning.Position = value;
        }

        public Double ScaleBymV
        {
            get => Model.Conditioning.Scale;
            set => Model.Conditioning.Scale = value;
        }

        public Double MaxScale => Model.Conditioning.MaxScale;

        public Double MinScale => Model.Conditioning.MinScale;


        public override TimebasePrsnt Sampling
        {
            get;
        }


        public override Double PosIndexBymDiv
        {
            get => base.PosIndexBymDiv;
            set
            {
                base.PosIndexBymDiv = value;
                //Hardware.HdCmdFactory.Push(HdCmd.ChnlPosition);
            }
        }

        public override Int32 ScaleIndex
        {
            get => base.ScaleIndex;
            set
            {
                base.ScaleIndex = value;

                //DsoModel.Default.GetTrigger().LeapPosIndex();

                //Hardware.HdCmdFactory.Push(HdCmd.ChnlScaleIndex);
            }
        }

        public override Int32 ScaleTick
        {
            get => base.ScaleTick;
            set
            {
                base.ScaleTick = value;

                //DsoModel.Default.GetTrigger().LeapPosIndex();
            }
        }

        #endregion 通道属性

        #region 解码属性

        public SerialProtocolType ProtocolType
        {
            get => Model.ProtocolType;
            set
            {
                if (PlatformManager.Default.Platform.OptionProtocols.ContainsKey(value))
                {
                    var option = PlatformManager.Default.Platform.OptionProtocols[value];
                    if (!OptionsManager.Default.GetOptionAvailable(option))
                    {
                        WeakTip.Default.Write("Decode", MsgTipId.PurchaseOptions, duration: 4);
                        return;
                    }
                }

                if (Model.ProtocolType == value) return;
                ProtocolPrsnt.GetDecodeChPrsnt(Id, Dso, null, value);
                Model.ProtocolType = value;
                if(TriggerPrsnt.Type== TriggerType.Serial)
                {
                    Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
                }
                Model.NotifyProtolTypeChanged();

            }
        }

        //public Boolean IsEventListsVisiable
        //{
        //    get => Model.IsEventListsVisiable;
        //    set => Model.IsEventListsVisiable = value;
        //}

        public DecodeDisplayMode Format
        {
            get => Model.Format;
            set => Model.Format = value;
        }

        public ProtocolPrsnt DecodeChPrsnt => ProtocolPrsnt.GetCurrentChannelDecodePrsnt(Id, Dso);

        #endregion 解码属性

    }
}
