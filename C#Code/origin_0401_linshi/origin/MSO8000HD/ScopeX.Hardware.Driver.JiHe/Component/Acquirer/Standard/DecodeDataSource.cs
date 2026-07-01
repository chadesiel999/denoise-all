using ScopeX.ComModel;
using System;

namespace ScopeX.Hardware.Driver
{
    public sealed class DecodeDataSource
    {
        public static Boolean NeedTakeNewData = false;

        private DecodeDataSource()
        {
            AnalogDataSources = new DeocodeDataSourcePacket[ChannelIdExt.BusChnlNum];
            for (Int32 i = 0; i < ChannelIdExt.BusChnlNum; i++)
            {
                AnalogDataSources[i] = new ComModel.DeocodeDataSourcePacket();
                AnalogDataSources[i].ChannelDataSource = new Byte[0];
                AnalogDataSources[i].ChannelLowDataSource = new Byte[AnalogDataSources[i].ChannelDataSource.Length];
                AnalogDataSources[i].Channels = new ComModel.ChannelId[0];
            }
            ReferenceDataSource = new DeocodeDataSourcePacket[ChannelIdExt.MaxRChId - ChannelIdExt.MinRChId + 1];
            DiffDataSource = new DeocodeDataSourcePacket[ChannelIdExt.MaxRChId - ChannelIdExt.MinRChId + 1];

            LADataSource = Constants.PRODUCT switch
            {
                ProductType.JiHe_MSO7000X => new DeocodeDataSourcePacket() { Channels = new ChannelId[0] },
                ProductType.JiHe_UPO7000L => null,
                _ => new DeocodeDataSourcePacket() { Channels = new ChannelId[0] }
            };
        }
        public ComModel.DeocodeDataSourcePacket[] AnalogDataSources;
        public ComModel.DeocodeDataSourcePacket[] ReferenceDataSource;
        public ComModel.DeocodeDataSourcePacket? LADataSource;
        public ComModel.DeocodeDataSourcePacket[] DiffDataSource;
        public static DecodeDataSource Instance { get; } = new DecodeDataSource();


        public Boolean RemoveReferenceData(ChannelId id)
        {
            if (ReferenceDataSource[id - ChannelIdExt.MinRChId].Channels[0] == id)
            {
                ReferenceDataSource[id - ChannelIdExt.MinRChId].Channels = new ChannelId[1];
                ReferenceDataSource[id - ChannelIdExt.MinRChId].ChannelDataSource = null;
                ReferenceDataSource[id - ChannelIdExt.MinRChId].ChannelLowDataSource = null;
                ReferenceDataSource[id - ChannelIdExt.MinRChId].PerChannelDataLength = 0;
                ReferenceDataSource[id - ChannelIdExt.MinRChId].TimeStamp = 0;
                ReferenceDataSource[id - ChannelIdExt.MinRChId].InterwovenBitCount = 0;
                ReferenceDataSource[id - ChannelIdExt.MinRChId].MaxByteCount = 0;
                ReferenceDataSource[id - ChannelIdExt.MinRChId].SampleRate = 0;
                ReferenceDataSource[id - ChannelIdExt.MinRChId].HasData = false;
                ReferenceDataSource[id - ChannelIdExt.MinRChId].TriggerIndex = 0;
            }
            return false;
        }

    }
}
