using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    public class HistoryData
    {
        private List<Double[]> _Data = new List<Double[]>();
        private static Object _Locker = new Object(); 
        public HistoryData(UInt32 maxPkgCount = 300)
        {
            _MaxPkgCount = maxPkgCount;
        }

        /// <summary>
        /// 每个包的长度
        /// </summary>
        private UInt64 _PkgLength = 0;
        private UInt64 PkgLength 
        { 
            get { return _PkgLength; }
            set {
                if (_PkgLength != value)
                {
                    lock (_Locker)
                        _Data.Clear();
                }
                _PkgLength = value; 
            }
        }


        private UInt64 _MaxPkgCount;
        public UInt64 MaxPkgCount
        {
            get { return _MaxPkgCount; }
            set {
                lock (_Locker)
                    if (value != _MaxPkgCount)
                    {
                        if ((value < _MaxPkgCount) && ((UInt64)_Data.Count > _MaxPkgCount))
                        {
                            Int64 outCount = (Int64)((UInt64)_Data.Count - _MaxPkgCount);
                            for (Int64 i = 0; i < outCount; i++)
                                _Data.RemoveAt(0);
                        }
                        _MaxPkgCount = value;
                    }
            }
        }

        /// <summary>
        /// 当前包数量
        /// </summary>
        private UInt64 _PkgCount = 0;
        public UInt64 PkgCount
        {
            get {
                lock (_Locker)
                    return _PkgCount;
            }
        }

        public void AddData(IEnumerable<Double[]> buffers)
        {
            if (buffers != null)
            {
                if ((UInt64)buffers.ToList()[0].Length != _PkgLength)
                    PkgLength = (UInt64)buffers.ToList()[0].Length;
                lock (_Locker)
                {
                    _Data.AddRange(buffers);
                    if ((UInt64)_Data.Count > _MaxPkgCount)
                    {
                        Int64 outCount = (Int64)((UInt64)_Data.Count - _MaxPkgCount);
                        for (Int64 i = 0; i < outCount; i++)
                            _Data.RemoveAt(0);
                    }
                    _PkgCount =(UInt64)_Data.Count;
                }
            }
            else
                return;
        }

        public List<Double[]> GetList()
        {
            List<Double[]> lsit = new List<Double[]>();
            lock (_Locker)
                for (Int32 i = 0; i < _Data.Count; i++)
                {
                    lsit.Add(_Data[i]);
                }
            return lsit;
        }
        public Double[,] GetData()
        {
            lock (_Locker)
                if (_Data.Count > 0)
                {
                    Double[,] data = new Double[_Data.Count, _Data[0].Length];
                    for (Int32 i = 0; i < data.GetLength(0); i++)
                        for (Int32 j = 0; j < data.GetLength(1); j++)
                        {
                            data[i, j] = _Data[i][j];
                        }
                    return data;
                }
            return new Double[0, 0];
        }

        public IEnumerable<Double[]> GetEnumerableData()
        {
            lock (_Locker)
                return _Data;
        }

        public void Clear()
        {
            lock (_Locker)
               _Data.Clear();
        }
    }
}
