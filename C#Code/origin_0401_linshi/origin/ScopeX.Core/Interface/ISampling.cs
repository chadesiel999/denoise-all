using ScopeX.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    public interface ISampling
    {       
        Double PosIndexBymDiv
        {
            get;
            set;
        }

        public Double PosIdxPerDiv
        {
            get;
        }

        void ResetPosIndex();

        (Double PositionByus, Double PosIndex, Double Scale, Double PosIdxPerDiv) GetCurrentTmbInfo();

        Int32 ScaleIndex
        {
            get;
            set;
        }

        Prefix Prefix
        {
            get;
        }

        String Unit
        {
            get;
            set;
        }
    }

    public interface  ITimebasePrsnt : ISampling, IPresenter<ITimebaseView>
    {
        
    }
}
