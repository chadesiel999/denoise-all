using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    /// <summary>
    /// Close协议解码的Model类
    /// </summary>
    internal class CloseDecodeModel : ProtocolModel
    {
        private DecodeResultData _DecodeResult = new DecodeResultData();
        public CloseDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id,ComModel.SerialProtocolType.Close, isTrigDecode)
        {
            this._EventInfos.Add(new ComModel.ProtocolEventInfo()
            {
                Index = 0,
                StartTimeByPs = Double.NaN,
            });
            _DecodeResult.Name = "Close";
            _DecodeResult.DecodeViewInfos = new IDecodeViewInfo[0];
        }
        internal override void ParsingData(ref CancellationToken token)
        {
            if ((DecodePackets.Count == 1 && DecodePackets[0].Name != _DecodeResult.Name) || DecodePackets.Count != 1)
            {
                var tempbuffer = GetDecodeBuffer();
                tempbuffer.Clear();
                tempbuffer.Add(_DecodeResult);
                ChangeBuffer();
            }
        }
        public override IReadOnlyList<String> EventInfoTitles => new List<String>() { "Index", "Start Time" };
    }
}
