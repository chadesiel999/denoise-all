using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ScopeX.ComModel;
using ScopeX.Core.Decode;

namespace ScopeX.Core.Tools
{
    internal class TriggerTools
    {
        internal static ConcurrentDictionary<TriggerType, TriggerPrsnt> TriggerPrsnts = new ConcurrentDictionary<TriggerType, TriggerPrsnt>();
        internal static ConcurrentDictionary<SerialProtocolType, TrigSerialPrsnt> TrigSerialPrsnts = new ConcurrentDictionary<SerialProtocolType, TrigSerialPrsnt>();

        internal static TriggerPrsnt MakeTrigPrsnt(IDsoPrsnt idp, TriggerType tt)
        {
            if(!TriggerPrsnts.ContainsKey(TriggerType.Serial))
            {
                TriggerPrsnts.TryAdd(TriggerType.Serial, new CloseTrigSerialPrsnt(idp));
            }

            TriggerPrsnt prsnt;

            if (TriggerTools.TriggerPrsnts.ContainsKey(tt))
            {
                prsnt = TriggerTools.TriggerPrsnts[tt];
                prsnt.LoadEvent();
            }
            else
            {
                prsnt = tt switch
                {
                    TriggerType.Edge => new TrigEdgePrsnt(idp),
                    TriggerType.PulseWidth => new TrigWidthPrsnt(idp),
                    TriggerType.Video => new TrigVideoPrsnt(idp),
                    TriggerType.Pattern => new TrigPatPrsnt(idp),
                    TriggerType.State => new TrigStatePrsnt(idp),
                    TriggerType.Delay => new TrigDelayPrsnt(idp),
                    TriggerType.TimeOut => new TrigTimeOutPrsnt(idp),
                    TriggerType.SustainTime => new TrigSustainTimePrsnt(idp),
                    TriggerType.SetupHold => new TrigSetupHoldPrsnt(idp),
                    TriggerType.NEdge => new TrigNEdgePrsnt(idp),
                    TriggerType.Runt => new TrigRuntPrsnt(idp),
                    TriggerType.Transition => new TrigTransPrsnt(idp),
                    TriggerType.Glitch => new TrigGlitchPrsnt(idp),
                    TriggerType.Window => new TrigWindowPrsnt(idp),
                    TriggerType.Interval => new TrigIntervalPrsnt(idp),
                    TriggerType.MultiQulified => new TrigMultiQualifiedPrsnt(idp),
                    TriggerType.Serial => TriggerTools.TrigSerialPrsnts[SerialProtocolType.Close],

                    _ => throw new NotImplementedException(),
                };
                TriggerTools.TriggerPrsnts.TryAdd(tt, prsnt);
            }

            return prsnt;

            //return tt switch
            //{
            //    TriggerType.Edge => new TrigEdgePrsnt(idp),
            //    TriggerType.PulseWidth => new TrigWidthPrsnt(idp),
            //    TriggerType.Video => new TrigVideoPrsnt(idp),
            //    TriggerType.Pattern => new TrigPatPrsnt(idp),
            //    TriggerType.State => new TrigStatePrsnt(idp),
            //    TriggerType.Delay => new TrigDelayPrsnt(idp),
            //    TriggerType.TimeOut => new TrigTimeOutPrsnt(idp),
            //    TriggerType.SustainTime => new TrigSustainTimePrsnt(idp),
            //    TriggerType.SetupHold => new TrigSetupHoldPrsnt(idp),
            //    TriggerType.NEdge => new TrigNEdgePrsnt(idp),
            //    TriggerType.Runt => new TrigRuntPrsnt(idp),
            //    TriggerType.Transition => new TrigTransPrsnt(idp),
            //    TriggerType.Glitch => new TrigGlitchPrsnt(idp),
            //    TriggerType.Window => new TrigWindowPrsnt(idp),
            //    TriggerType.Interval => new TrigIntervalPrsnt(idp),
            //    TriggerType.MultiQulified => new TrigMultiQualifiedPrsnt(idp),
            //    TriggerType.Serial => new CloseTrigSerialPrsnt(idp),
            //    _ => throw new NotImplementedException(),
            //};
        }

        internal static TrigSerialPrsnt GetSerialPrsnt(IDsoPrsnt idp, ChannelId id, ITriggerSerialView? view)
        {
            var serialtype = SerialProtocolType.Close;
            if (id != ChannelId.None)
            {
                if (idp.TryGetChannel(id, out var p) && p is DecodePrsnt decodep && decodep.Active)
                {
                    serialtype = decodep.ProtocolType;
                }
            }

            TrigSerialPrsnt prsnt;

            if (TriggerTools.TrigSerialPrsnts.ContainsKey(serialtype))
            {
                prsnt = TriggerTools.TrigSerialPrsnts[serialtype];
                prsnt.LoadEvent();
            }
            else
            {
                prsnt = serialtype switch
                {
                    SerialProtocolType.Close => new CloseTrigSerialPrsnt(idp, view),
                    SerialProtocolType.RS232 => new RS232TrigSerialPrsnt(idp, view),
                    SerialProtocolType.I2C => new I2CTrigSerialPrsnt(idp, view),
                    SerialProtocolType.SPI => new SPITrigSerialPrsnt(idp, view),
                    SerialProtocolType.SENT => new SENTTrigSerialPrsnt(idp, view),
                    SerialProtocolType.JTAG => new JTAGTrigSerialPrsnt(idp, view),
                    SerialProtocolType.CAN => new CANTrigSerialPrsnt(idp, view),
                    SerialProtocolType.CAN_FD => new CANFDTrigSerialPrsnt(idp, view),
                    SerialProtocolType.ARINC429 => new ARINC429TrigSerialPrsnt(idp, view),
                    SerialProtocolType.LIN => new LINTrigSerialPrsnt(idp, view),
                    SerialProtocolType.SPMI => new SPMITrigSerialPrsnt(idp, view),
                    SerialProtocolType.FlexRay => new FlexRayTrigSerialPrsnt(idp, view),
                    SerialProtocolType.AudioBus => new AudioBusTrigSerialPrsnt(idp, view),
                    SerialProtocolType.Ethernet => new EthernetTrigSerialPrsnt(idp, view),
                    SerialProtocolType.PCIe => new PCIeTrigSerialPrsnt(idp, view),
                    SerialProtocolType.USB => new USBTrigSerialPrsnt(idp, view),
                    SerialProtocolType.SATA => new SATATrigSerialPrsnt(idp, view),
                    SerialProtocolType.MIL => new MILTrigSerialPrsnt(idp, view),
                    SerialProtocolType.NRZ => new NRZTrigSerialPrsnt(idp, view),
                    SerialProtocolType.CPHY => new CPHYTrigSerialPrsnt(idp, view),
                    SerialProtocolType.Manchester => new ManchesterTrigSerialPrsnt(idp, view),
                    SerialProtocolType.I3C => new I3CTrigSerialPrsnt(idp, view),
                    SerialProtocolType.PSI5 => new PSI5TrigSerialPrsnt(idp, view),
                    SerialProtocolType.SMBus => new SMBusTrigSerialPrsnt(idp, view),
                    SerialProtocolType.Common_8b10b => new D8B10BTrigSerialPrsnt(idp, view),
                    SerialProtocolType.Mlt3 => new Mlt3TrigSerialPrsnt(idp, view),
                    SerialProtocolType.CXPI => new CXPITrigSerialPrsnt(idp, view),
                    _ => throw new NotImplementedException(),
                };
                TriggerTools.TrigSerialPrsnts.TryAdd(serialtype, prsnt);
            }

            prsnt.Source = id;
            return prsnt;
        }

        internal static void TryRemoveTriggerView(ITriggerView view)
        {
            if(TriggerPrsnts.Count>0)
            {
                foreach(var prsnt in TriggerPrsnts.Values)
                {
                    if(prsnt!=null)
                    {
                        prsnt.TryRemoveView(view);
                    }
                }
            }

            if (TrigSerialPrsnts.Count > 0)
            {
                foreach (var prsnt in TrigSerialPrsnts.Values)
                {
                    if (prsnt != null)
                    {
                        prsnt.TryRemoveView(view);
                    }
                }
            }
        }
    }
}
