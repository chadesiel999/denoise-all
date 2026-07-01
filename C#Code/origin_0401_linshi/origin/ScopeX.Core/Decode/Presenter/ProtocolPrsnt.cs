using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    /// <summary>
    /// 所有通道解码Prsnt的基类
    /// </summary>
    public abstract class ProtocolPrsnt :MulticastPrsnt<IProtocolView>, IProtocolPrsnt
    {
        /// <summary>
        /// 当前系统中使用的通道解码Prsnt
        /// </summary>
        private static Dictionary<ChannelId, ProtocolPrsnt> _Currents = new Dictionary<ChannelId, ProtocolPrsnt>();

        /// <summary>
        /// 当前使用的Prsnt
        /// </summary>
        private static ProtocolPrsnt? _TrigDecodeCurrent;

        public ProtocolPrsnt(IDsoPrsnt dso, IProtocolView? view) : base(dso)
        {
            if (view != null)
            {
                view.Presenter = this;
                TryAddView(view);
            }
        }


        /// <summary>
        /// 当前的协议触发类型
        /// </summary>
        public static SerialProtocolType CurrentType => TriggerSerialShareParameter.Default.ProtocolType;


        public IReadOnlyList<DecodeResultData> DecodePackets => GetDecodePackets();

        public IReadOnlyList<String> EventInfoTitles => Model.EventInfoTitles;

        public ChannelId[] ActivedChannels => Model.ActivedChannels;

        /// <summary>
        /// 是否为触发解码
        /// </summary>
        //public Boolean IsTrigger => Model.IsTrigger;
        public Boolean IsTrigger
        {
            get => Model.IsTrigger;
            set => Model.IsTrigger = value;
        }

        public IReadOnlyList<ComModel.ProtocolEventInfo> ProtocolEvents => Model.ProtocolEvents;

        /// <summary>
        /// 协议类型
        /// </summary>
        public ComModel.SerialProtocolType ProtocolType
        {
            get => Model.ProtocolType;
            set => Model.ProtocolType = value.Clamp();
        }



        public Int32 SelectedEventIndex { get; set; } = -1;

        private IReadOnlyList<DecodeResultData> GetDecodePackets()
        {
            if (Model == null || Model.DecodePackets == null)
            {
                return new List<DecodeResultData>();
            }
            else
            {

                //}
            }
            return Model.DecodePackets;
        }


        /// <summary>
        /// Model
        /// </summary>
        private abstract protected override ProtocolModel Model
        {
            get;
        }


        /// <summary>
        /// 获取系统中指定通道当前的解码Prsnt
        /// </summary>
        /// <param name="id">指定的通道</param>
        public static ProtocolPrsnt GetCurrentChannelDecodePrsnt(ChannelId id, IDsoPrsnt idp)
        {
            if (_Currents.TryGetValue(id, out ProtocolPrsnt? prsnt) && prsnt != null)
                return prsnt;
            else
            {
                /*prsnt = new CloseDecodePrsnt(id, idp, null);
                _Currents[id] = prsnt;
                return prsnt;*/
                var am = DsoModel.Default.Channels.Where(c => c.Id.IsDecode() && c.Id == id).FirstOrDefault();
                if (am != null && am is DecodeModel dm)
                {
                    var temp = DecodeTools.GetChDecodePrsnt(id, idp, dm.ProtocolType, null);
                    _Currents[id] = temp;
                    return temp;
                }
                else
                {
                    prsnt = new CloseDecodePrsnt(id, idp, null);
                    _Currents[id] = prsnt;
                    return prsnt;
                }
            }
        }

        /// <summary>
        /// 获取通道的协议解码Prsnt
        /// </summary>
        /// <param name="id">通道</param>
        /// <param name="view">View</param>
        /// <param name="serialType">协议类型</param>
        public static ProtocolPrsnt GetDecodeChPrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, SerialProtocolType serialType = SerialProtocolType.Close)
        {
            IEnumerable<IProtocolView> views = new List<IProtocolView>();

            if (_Currents.TryGetValue(id, out ProtocolPrsnt? prsnt))
            {
                if (prsnt != null)
                {
                    views = prsnt.GetViewList();

                    if (prsnt.ProtocolType != serialType)
                    {
                        //prsnt.Dispose();
                        //((DecodeModel)DsoModel.Default.GetChannel(id)).ProtocolType = serialType;
                        prsnt = DecodeTools.GetChDecodePrsnt(id, idp, serialType, view);

                        if (view != null && (views.Count() > 0))
                        {
                            views.ToList().ForEach(x =>
                            {
                                if (!x.Equals(view))
                                    prsnt.TryAddView(x);
                            });
                        }
                        else
                            prsnt.AddViewList(views);

                        _Currents[id] = prsnt;
                        return prsnt;
                    }
                    else
                    {
                        if (view != null && !views.Contains(view))
                            prsnt.TryAddView(view);

                        return prsnt;
                    }
                }
                else
                {
                    prsnt = DecodeTools.GetChDecodePrsnt(id, idp, serialType, view);
                    _Currents[id] = prsnt;
                    return prsnt;
                }
            }
            else
            {
                prsnt = DecodeTools.GetChDecodePrsnt(id, idp, serialType, view);
                _Currents[id] = prsnt;
                return prsnt;
            }
        }

        /// <summary>
        /// 获取指定协议的Prsnt
        /// </summary>
        /// <param name="serialProtocolType"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public static ProtocolPrsnt GetTrigDecodePrsnt(IDsoPrsnt idp, SerialProtocolType serialProtocolType, IProtocolView? view)
        {
            var current = TriggerSerialShareParameter.Default.Source;

            if (current != ChannelId.None)
            {
                var decode = DsoPrsnt.DefaultDsoPrsnt.GetAllChnls().Where(x => x.Id >= ChannelIdExt.MinBChId && x.Id <= ChannelIdExt.MaxBChId && x.Id == current)?.Cast<DecodePrsnt>()?.FirstOrDefault();


                if (decode != null)
                {
                    return decode.DecodeChPrsnt;
                }
            }

            return serialProtocolType switch
            {
                SerialProtocolType.Close => new CloseDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.RS232 => new RS232DecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.I2C => new I2CDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.SPI => new SPIDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.SENT => new SENTDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.JTAG => new JTAGDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.CAN => new CANDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.ARINC429 => new ARINC429DecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.CAN_FD => new CANFDDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.AudioBus => new AudioBusDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.FlexRay => new FlexRayDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.LIN => new LINDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.MIL => new MILDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.PCIe => new PCIeDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.SATA => new SATADecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.SPMI => new SPMIDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.USB => new USBDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.Ethernet => new EthernetDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.NRZ => new NRZDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.SMBus => new SMBusDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.Common_8b10b => new D8B10BDecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.Mlt3 => new Mlt3DecodePrsnt(ChannelId.B1, idp, view, true),
                SerialProtocolType.CXPI => new CXPIDecodePrsnt(ChannelId.B1, idp, view, true),
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// 获取指定协议的的解码Prsnt
        /// </summary>
        /// <param name="serialType">协议类型</param>
        /// <param name="view">View</param>
        public static ProtocolPrsnt GetTrigSerialDecodePrsnt(IDsoPrsnt idp, SerialProtocolType serialType, IProtocolView? view)
        {
            if (_TrigDecodeCurrent == null)
                _TrigDecodeCurrent = GetTrigDecodePrsnt(idp, serialType, view);
            else
            {
                if (_TrigDecodeCurrent.ProtocolType != serialType)
                {
                    _TrigDecodeCurrent.Dispose();
                    _TrigDecodeCurrent = GetTrigDecodePrsnt(idp, serialType, view);
                }
            }
            if (view != null)
            {
                _TrigDecodeCurrent.TryAddView(view);
            }
            return _TrigDecodeCurrent;
        }

        /// <summary>
        /// 获取系统中所有通道中的协议解码Prsnt
        /// </summary>
        public List<IProtocolPrsnt> GetChannlesDecodePrsnt()
        {
            return ChannelIdExt.GetDecodes().Select(x => GetCurrentChannelDecodePrsnt(x, Dso)).Cast<IProtocolPrsnt>().ToList();
        }

        /// <summary>
        /// 获取系统中当前的触发中的解码Prsnt
        /// </summary>
        public IProtocolPrsnt GetDecodePrsnt(ChannelId id)
        {
            //SerialProtocolType serialtype = TriggerSerialShareParameter.Default.ProtocolType;

            //if (TriggerPrsnt.Type != TriggerType.Serial)
            //{
            //    if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(TriggerSerialShareParameter.Default.Source, out var prsnt) && prsnt is DecodePrsnt decode && decode != null)
            //    {
            //        serialtype = decode.ProtocolType;
            //    }
            //}

            //return GetTrigSerialDecodePrsnt(Dso, serialtype, null);

            if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(id, out var prsnt)&& prsnt is DecodePrsnt dp&&dp!=null)
            {
                return dp.DecodeChPrsnt;
            }
            return null;
        }

        public abstract List<ChannelId> GetDecodeSource();

        //internal void DecodePacketData(List<WfmPack> wfmPacks) => Model?.DecodePacketData(wfmPacks);
    }
}
