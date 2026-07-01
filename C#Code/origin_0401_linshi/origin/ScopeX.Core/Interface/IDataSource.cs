using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    interface IDataSource
    {
        Object? Prepare(Boolean init, ChannelId aid, CancellationToken ct, DataRole dataRole, CancellationToken? softResetToken);

        (Double[,] Buffer, Object Prop)? Read(Object? context);

        WfmPack Process((Double[,] Buffer, Object Prop) pkg, Object? context);

    }

    interface IRFDataSource: IDataSource
    {
        WfmPack ProcessNormal((Double[,] Buffer, Object Prop) pkg, Object? context);
        WfmPack ProcessMaxHold((Double[,] Buffer, Object Prop) pkg, Object? context);
        WfmPack ProcessMinHold((Double[,] Buffer, Object Prop) pkg, Object? context);
        WfmPack ProcessAverage((Double[,] Buffer, Object Prop) pkg, Object? context);
        void Init();
    }
}
