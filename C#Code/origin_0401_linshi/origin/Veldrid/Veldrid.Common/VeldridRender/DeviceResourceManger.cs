using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veldrid.Common.VeldridRender
{

    internal class DeviceResourceManger : IDisposable
    {
        private Dictionary<string, IDisposable> resource = new Dictionary<string, IDisposable>();
        private bool disposedValue;

        public IDisposable this[string name]
        {
            get
            {
                if (resource.TryGetValue(name, out var res))
                {
                    return res;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                resource[name] = value;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    resource.Values.ToList().ForEach(x => x.Dispose());
                    resource.Clear();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~DeviceResourceManger()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
