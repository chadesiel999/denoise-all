using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid.Common.Tools;

namespace Veldrid.Common
{
    public class VeldridCollection<T> : ICollection<T>,IDisposable where T : class
    {
        internal event EventHandler<T> ItemAddEvent;
        internal event EventHandler ItemRemoveEvent;
        private List<T> _list = new List<T>();
        private bool disposedValue;

        public int Count =>_list.Count;

        public bool IsReadOnly => false;
        public T this[int index] { get => _list[index]; set => _list[index] = value; }

        public void Add(T item)
        {
            if(!_list.Contains(item))
            { 
                _list.Add(item);
                ItemAddEvent?.Invoke(this, item);
            }
        }

        public void Clear()
        {
            _list.ForEach(x => x.ClearEventHandle());
            _list.Clear();
            ItemRemoveEvent?.Invoke(this, EventArgs.Empty);
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public bool Remove(T item)
        {
            Boolean result = _list.Remove(item);
            item?.ClearEventHandle();
            if (result)ItemRemoveEvent?.Invoke(this, EventArgs.Empty);
            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        public List<T> ToList() => _list;
        public T[] ToArray() => _list.ToArray();

        public void ForEach(Action<T> action) => _list.ForEach(action);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Clear();
                    this.ClearEventHandle();
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~VeldridCollection()
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
