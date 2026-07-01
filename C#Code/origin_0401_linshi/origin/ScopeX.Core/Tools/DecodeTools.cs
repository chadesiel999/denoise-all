using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Decode;

namespace ScopeX.Core.Tools
{
    internal class DecodeTools
    {
        internal static ConcurrentDictionary<ChannelId,ConcurrentDictionary<SerialProtocolType, ProtocolPrsnt>> ChannelProtocolPrsnts = new ConcurrentDictionary<ChannelId, ConcurrentDictionary<SerialProtocolType, ProtocolPrsnt>>();
        internal static ConcurrentDictionary<ChannelId,ConcurrentDictionary<SerialProtocolType, ProtocolModel>> ChannelProtocolModels = new ConcurrentDictionary<ChannelId, ConcurrentDictionary<SerialProtocolType, ProtocolModel>>();


        /// <summary>
        /// 获取指定通道、指定协议的Prsnt
        /// </summary>
        /// <param name="id"></param>
        /// <param name="view"></param>
        /// <param name="SerialProtocolType"></param>
        /// <returns></returns>
        internal static ProtocolPrsnt GetChDecodePrsnt(ChannelId id, IDsoPrsnt idp, SerialProtocolType type, IProtocolView? view)
        {
            ProtocolPrsnt prsnt;
            if(!ChannelProtocolPrsnts.ContainsKey(id))
            {
                ChannelProtocolPrsnts.TryAdd(id, new ConcurrentDictionary<SerialProtocolType, ProtocolPrsnt>());
            }
            if (!ChannelProtocolPrsnts[id].ContainsKey(type))
            {
                prsnt = type switch
                {
                    SerialProtocolType.Close => new CloseDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.I2C => new I2CDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.RS232 => new RS232DecodePrsnt(id, idp, view, false),
                    SerialProtocolType.SPI => new SPIDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.SENT => new SENTDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.JTAG => new JTAGDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.CAN => new CANDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.ARINC429 => new ARINC429DecodePrsnt(id, idp, view, false),
                    SerialProtocolType.AudioBus => new AudioBusDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.CAN_FD => new CANFDDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.FlexRay => new FlexRayDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.LIN => new LINDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.MIL => new MILDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.PCIe => new PCIeDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.SATA => new SATADecodePrsnt(id, idp, view, false),
                    SerialProtocolType.SPMI => new SPMIDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.USB => new USBDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.Ethernet => new EthernetDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.NRZ => new NRZDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.CPHY => new CPHYDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.Manchester => new ManchesterDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.I3C => new I3CDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.PSI5 => new PSI5DecodePrsnt(id, idp, view, false),
                    SerialProtocolType.SMBus => new SMBusDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.Common_8b10b => new D8B10BDecodePrsnt(id, idp, view, false),
                    SerialProtocolType.Mlt3 => new Mlt3DecodePrsnt(id, idp, view, false),
                    SerialProtocolType.CXPI => new CXPIDecodePrsnt(id, idp, view, false),
                    _ => throw new NotSupportedException(),
                };
                ChannelProtocolPrsnts[id].TryAdd(type,prsnt);
            }
            else
            {
                prsnt = ChannelProtocolPrsnts[id][type];
                if(prsnt!=null&& view!=null)
                {
                    prsnt!.TryAddView(view!);
                }
            }

            return prsnt;
        }



        /// <summary>
        /// 获取指定通道、指定协议的Prsnt
        /// </summary>
        /// <param name="id"></param>
        /// <param name="view"></param>
        /// <param name="SerialProtocolType"></param>
        /// <returns></returns>
        internal static ProtocolModel GetChannelDecodeModel(ChannelId id,SerialProtocolType type)
        {
            ProtocolModel model;
            if (!ChannelProtocolModels.ContainsKey(id))
            {
                ChannelProtocolModels.TryAdd(id, new ConcurrentDictionary<SerialProtocolType, ProtocolModel>());
            }
            if (!ChannelProtocolModels[id].ContainsKey(type))
            {
                model = type switch
                {
                    SerialProtocolType.Close => new CloseDecodeModel(id,false),
                    SerialProtocolType.RS232 => new RS232DecodeModel(id,false),
                    SerialProtocolType.I2C => new I2CDecodeModel(id,false),
                    SerialProtocolType.SPI => new SPIDecodeModel(id,false),
                    SerialProtocolType.SENT => new SENTDecodeModelCPP(id,false),
                    SerialProtocolType.JTAG => new JTAGDecodeModel(id,false),
                    SerialProtocolType.CAN => new CANDecodeModelCPP(id,false),
                    SerialProtocolType.ARINC429 => new ARINC429DecodeModelCPP(id,false),
                    SerialProtocolType.AudioBus => new AudioBusDecodeModelCPP(id,false),
                    SerialProtocolType.CAN_FD => new CANFDDecodeModelCPP(id,false),
                    SerialProtocolType.FlexRay => new FlexRayDecodeModelCPP(id,false),
                    SerialProtocolType.LIN => new LINDecodeModelCPP(id,false),
                    SerialProtocolType.MIL => new MILDecodeModelCPP(id,false),
                    SerialProtocolType.PCIe => new PCIeDecodeModel(id,false),
                    SerialProtocolType.SATA => new SATADecodeModel(id,false),
                    SerialProtocolType.SPMI => new SPMIDecodeModel(id,false),
                    SerialProtocolType.USB => new USBDecodeModelCPP(id,false),
                    SerialProtocolType.Ethernet => new EthernetDecodeModel(id,false),
                    SerialProtocolType.NRZ => new NRZDecodeModel(id,false),
                    SerialProtocolType.CPHY => new CPHYDecodeModelCPP(id,false),
                    SerialProtocolType.Manchester => new ManchesterDecodeModel(id, false),
                    SerialProtocolType.I3C => new I3CDecodeModel(id, false),
                    SerialProtocolType.PSI5 => new PSI5DecodeModel(id, false),
                    SerialProtocolType.SMBus => new SMBusDecodeModel(id, false),
                    SerialProtocolType.Common_8b10b => new D8B10BDecodeModel(id, false),
                    SerialProtocolType.Mlt3 => new Mlt3DecodeModel(id, false),
                    SerialProtocolType.CXPI => new CXPIDecodeModel(id, false),
                    _ => throw new NotSupportedException(),
                };
                ChannelProtocolModels[id].TryAdd(type, model);
            }
            else
            {
                model = ChannelProtocolModels[id][type];
            }

            return model;
        }
    }
}
