using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid.Common.Tools;

namespace Veldrid.Common.Plot
{
    public abstract class BaseProperty:IDisposable
    {
        private bool disposedValue;

        public event EventHandler<String> PropertyChanged;
        protected void Set<T>(ref T field, T value, [CallerMemberName] String propertyName = "")
        {
            if(typeof(T).IsValueType && field.ToString()!=value.ToString())
            {
                field = value;
                OnPropertyChanged(this, propertyName);
            }
            else if (!Object.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(this, propertyName);
            }
        }
        protected void OnPropertyChanged(object sender, string name) => PropertyChanged?.Invoke(sender, name);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.ClearEventHandle();
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~BaseProperty()
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
