using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public struct DbiAnalogChannelItem_Common
    {
        public DbiAnalogChannelItem_Common()
        {
            Data = new Int64[DbiAnalogParams_Common.MaxItemCount];
        }
        private Int64[] Data;
        public Int64 this[int which]
        {
            get { return Data[(int)which]; }
            set { Data[(int)which] = value; }
        }
    }
    #region 
    public class ItemDefine
    {
        public String ItemName { get; set; }
        public int ItemIndex { get; set; }
    }
    public class AnalogChannelItemsDefine
    {
        public List<ItemDefine> AnalogChannelItems { get; set; }
        public List<String> Constrain { get; set; }
    }
    #endregion
    #region 
    public class PerAnalogChannelItemNameAndValue
    {
        public int ItemIndex { get; set; }=0;
        public Int64 ItemValue { get; set; } = 0;
    }
    public class AnalogChannelOneRecord
    {
        public String KeyStr { get; set; }
        public List<PerAnalogChannelItemNameAndValue> AnalogChannelItemNameAndValues { get; set; }
    }
    public class AnalogChannelAllDefault
    {
        public List<AnalogChannelOneRecord> AnalogChannelAllDefaultRecords { get; set; }
    }
    #endregion
}
