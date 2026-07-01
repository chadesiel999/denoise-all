using ScopeX.Core.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    public interface IChnlPrsnt : IPresenter<IChnlView>, IBadge/*, IBroadcaster*/
    {        
        String Label
        {
            get;
            set;
        }

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

        Int32 ScaleIndex
        {
            get;
            set;
        }

        String Unit
        {
            get;
            set;
        }

        Prefix Prefix
        {
            get;
        }

        ISampling Sampling
        {
            get;
        }

        WfmPack? Pack
        {
            get;
        }

        Int64? WindowId
        {
            get;
            set;
        }

        WfmVuDatabase VuDatabase
        {
            get;
        }

        //ReadHandler<Double[]> ReadCallback
        //{
        //    get;
        //    set;
        //} 

        //PrepareHandler PrepareCallback
        //{
        //    get;
        //    set;
        //}

        //ProcessHandler<Double[]> ProcessCallback
        //{
        //    get;
        //    set;
        //}

        //Func<Boolean>? Init
        //{
        //    get;
        //    set;
        //}

        //Boolean Take(Double[] buf, CancellationToken ct);
    }
};