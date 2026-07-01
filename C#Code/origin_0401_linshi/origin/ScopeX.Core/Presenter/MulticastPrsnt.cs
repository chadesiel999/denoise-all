using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ScopeX.Core
{
    public abstract class MulticastPrsnt<V> : IDisposable, IBroadcaster
        where V : IView
    {
        public MulticastPrsnt(IDsoPrsnt idp)
        {
            Dso = idp;
        }

        private protected abstract INotifyPropertyChanged? Model
        {
            get;
        }

        private readonly List<V> _Views = new();

        public List<V> GetViewList()
        {
            List<V> vus;
            lock ((_Views as ICollection).SyncRoot)
            {
                vus = _Views.ToList();
            }

            return vus;
        }

        public Boolean TryAddView(V vu)
        {
            Boolean res = false;
            lock ((_Views as ICollection).SyncRoot)
            {
                if (!_Views.Contains(vu))
                {
                    _Views.Add(vu);
                    res = true;
                }
            }
            return res;
        }

        public void AddViewList(IEnumerable<V> vus)
        {
            lock ((_Views as ICollection).SyncRoot)
            {
                foreach (V vu in vus)
                {
                    if (!_Views.Contains(vu))
                    {
                        _Views.Add(vu);
                    }
                }
            }
        }

        public Boolean TryRemoveView(V vu)
        {
            Boolean res = false;
            lock ((_Views as ICollection).SyncRoot)
            {
                //var idx = _Views.IndexOf(vu);
                //if (idx >= 0)
                //{
                //    _Views.RemoveAt(idx);
                //    res = true;
                //}

                res = _Views.Remove(vu);
            }
            return res;
        }

        public void RemoveViewList(IEnumerable<V> vus)
        {
            lock ((_Views as ICollection).SyncRoot)
            {
                foreach (V vu in vus)
                {
                    //var idx = _Views.IndexOf(vu);
                    //if (idx >= 0)
                    //{
                    //    _Views.RemoveAt(idx);
                    //}

                    _Views.Remove(vu);
                }
            }
        }

        public Boolean FrozenVu
        {
            get;
            set;
        } = false;

        public event EventHandler<CustomEventArg>? PublisherChanged;

        protected void OnRaiseCustomEvent(CustomEventArg e)
        {
            PublisherChanged?.Invoke(this, e);
        }

        public IDsoPrsnt Dso
        {
            get;
        }

        protected internal void OnPropertyChanged(Object? sender, PropertyChangedEventArgs e)
        {
            OnRaiseCustomEvent(new CustomEventArg(e.PropertyName ?? ""));
            if (!FrozenVu)
            {
                //lock ((_Views as ICollection).SyncRoot)
                //{
                //    //foreach (var vu in GetViewList())
                //    try
                //    {
                //        foreach (var vu in _Views)
                //        {
                //            vu.UpdateView(this, e.PropertyName ?? "");
                //        }
                //    }
                //    catch
                //    {
                //        throw;
                //    }
                //}


                List<V> vus = GetViewList();
                if (vus.Count > 0)
                {
#if DEBUG
                    try
                    {
                        for (Int32 i = vus.Count - 1; i >= 0; i--)
                        {
                            vus[i].UpdateView(this, e.PropertyName ?? "");
                        }
                    }
                    catch
                    {
                        throw;
                    }
#else
                    for (Int32 i = vus.Count - 1; i >= 0; i--)
                    {
                        vus[i].UpdateView(this, e.PropertyName ?? "");
                    }
#endif
                }
                else
                {
                    Dso?.View?.UpdateView(this, e.PropertyName ?? "");
                }
            }
        }

        #region Dispose
        private Boolean _Disposed = false;

        public void Dispose()
        {
            Dispose(true);
            //标记gc不在调用析构函数
            GC.SuppressFinalize(this);
        }

        ~MulticastPrsnt()
        {
            Dispose(false);
        }

        private protected virtual void Dispose(Boolean disposing)
        {
            //如果已经被回收，就中断执行
            if (_Disposed)
            {
                return;
            }

            if (disposing)
            {
                //TODO:释放本对象中管理的托管资源
                _Views.Clear();
                INotifyPropertyChanged? m = Model;
                if (m != null)
                {
                    m.PropertyChanged -= OnPropertyChanged;
                }
            }

            //TODO:释放非托管资源
            _Disposed = true;
        }
        #endregion
    }
}
