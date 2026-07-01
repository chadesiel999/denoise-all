using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Tools
{
    internal class ChannelZIndex
    {
        public static readonly ChannelZIndex Default = new();

        private  ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();

        private  List<ChannelId> _ChannelZIndexList = new List<ChannelId>();
        private ChannelZIndex()
        {

        }
        public List<ChannelId> ChannelZIndexList
        {
            get
            {
                rwl.EnterReadLock();
                try
                {
                    return _ChannelZIndexList.ToList();
                }
                finally
                {
                    rwl.ExitReadLock();
                }
            }
        }

        public void Prepend(ChannelId id)
        {
            rwl.EnterWriteLock();
            try
            {
                if (_ChannelZIndexList.Contains(id))
                {
                    _ChannelZIndexList.Remove(id);
                }
                _ChannelZIndexList = _ChannelZIndexList.Prepend(id).ToList();
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }
        public void Add(ChannelId id)
        {
            rwl.EnterWriteLock();
            try
            {
                if (_ChannelZIndexList.Contains(id))
                {
                    _ChannelZIndexList.Remove(id);
                }
                _ChannelZIndexList.Add(id);
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }
        public void Remove(ChannelId id)
        {
            rwl.EnterWriteLock();
            try
            {
                if (_ChannelZIndexList.Contains(id))
                {
                    _ChannelZIndexList.Remove(id);
                }
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }
    }
}
