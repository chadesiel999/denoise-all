using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public interface ICoefficientsTable
    {
        public Int32[] this[int channelIndex]
        {
            get;
        }

        public Int32 this[int channelIndex, int index]
        {
            get;
            set;
        }
        public byte[] SerializeByFpgaFormat(int channelID);
    }
}
