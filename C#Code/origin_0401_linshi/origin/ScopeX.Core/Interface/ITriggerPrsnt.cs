using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    public interface ITriggerPrsnt : IPresenter<ITriggerView>
    {
        //IEnumerable<ITriggerView> Views
        //{
        //    get;
        //}

        //Boolean TryAddView(ITriggerView vu);

        //Boolean TryRemoveView(ITriggerView vu);

        //Double PosIndex
        //{
        //    get;
        //    set;
        //}

        //void ResetPosIndex();

        void LoadEvent();

        void DisposeEvent();
    }
}
