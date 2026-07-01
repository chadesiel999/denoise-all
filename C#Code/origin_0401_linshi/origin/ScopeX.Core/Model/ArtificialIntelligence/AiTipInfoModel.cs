using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Model.ArtificialIntelligence
{
    internal class AiTipInfoModel
    {
        private AiTipInfoModel()
        { 
            
        }

        internal static AiTipInfoModel Default = new();

        private const Int32 _TipInfoBuffSize = 2;

        private List<String>[] _TipInfoBuff = new List<String>[_TipInfoBuffSize];
        private Object _LockObject = new();

        private Int32 _ReadBuffId = 0;
        private Int32 _WriteBuffId = 1;

        internal String[] TipInfo
        {
            get
            {
                lock (_LockObject)
                {
                    return _TipInfoBuff[_ReadBuffId].ToArray();
                }
            }
        }

        internal void UpdateTipInfo()
        { 

            lock ( _LockObject)
            {
                Int32 tmp = _WriteBuffId;
                _WriteBuffId = _ReadBuffId;
                _ReadBuffId = tmp;
            }
        }
    }
}
