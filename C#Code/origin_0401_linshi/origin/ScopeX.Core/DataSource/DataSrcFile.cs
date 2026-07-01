using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;
using ScopeX.MathExt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core.DataSource
{
    internal class DataSrcFile : IDataSource
    {
        private DataRole _DataRole;
        private String _FileName;
        public Object? Prepare(Boolean init, ChannelId aid, CancellationToken ct, DataRole dataRole, CancellationToken? softResetToken = null)
        {
            _DataRole = dataRole;
            return null;
        }
        public (Double[,], Object)? Read(Object? arg)
        {
            return (new Double[0,0],new Object());
        }

        public WfmPack Process((Double[,] Buffer, Object Prop) pkg, Object? arg)
        {
            if(_DataRole == DataRole.Zoom)
            {
                return null;
            }
            try
            {
                if (String.IsNullOrEmpty(_FileName) == false &&  File.Exists(_FileName))
                {
                    using System.IO.MemoryStream memorystream = new(System.IO.File.ReadAllBytes(_FileName));
                    return BinaryConvert.Deserialize<WfmPack>(memorystream);
                }
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
            }
             
            return null;
        }

        public DataSrcFile(String fileName)
        {
            _FileName = fileName;
        }
    }
}
