using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    public interface ISettingPrsnt
    {
        public AuxInputType AuxInputSignal
        {
            get; 
            set;
        }
        public EdgeSlope AuxInPolarity
        {
            get;
            set;
        }
        public AuxOutputType AuxOutputSignal
        {
            get;
            set;
        }
        public EdgeSlope AuxOutPolarity
        {
            get;
            set;
        }
    }
}
