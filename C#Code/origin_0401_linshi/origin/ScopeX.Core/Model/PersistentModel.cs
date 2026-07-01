using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    internal class PersistentModel<T>
    {
        public PersistentModel(Int32 maxPersistentCount, Int32 frameNumber = 10)
        {
            _PersistentData = new List<T[]>();
            _MaxPersistentCount = maxPersistentCount;
            _MaxFrameNumber = frameNumber;
        }

        private List<T[]> _PersistentData;
        private static readonly Object _Locker = new Object();

        private Int32 _MaxPersistentCount;
        private Int32 _MinPersistentCount = 1;


        private Int32 _DataLength = -1;
        private Int32 _MaxFrameNumber = 1;
        /// <summary>
        /// 存储帧数
        /// </summary>
        public Int32 MaxFrameNumber
        {
            get { return _MaxFrameNumber; }
            set {
                Int32 result = value > _MaxPersistentCount ? _MaxPersistentCount : value;
                result = value < _MinPersistentCount ? _MinPersistentCount : value;

                if (result != _MaxFrameNumber)
                {
                    _MaxFrameNumber = result;
                    _CurrentFrame = 0;

                    lock (_Locker)
                    { 
                        if (_PersistentData.Count > _MaxFrameNumber)
                        {
                            Int32 count = _PersistentData.Count - _MaxFrameNumber;
                            for (Int32 i = 0; i < count; i++)
                            {
                                _PersistentData.RemoveAt(0);
                            }
                        }
                    }
                }
            }
        }
        
        private Int32 _CurrentFrame = 0;
        /// <summary>
        /// 当前帧
        /// </summary>
        public Int32 CurrentFrame
        {
            get { return _CurrentFrame; }
            set
            {
                Int32 result = value > MaxFrameNumber - 1 ? MaxFrameNumber - 1 : value;
                result = value < 0 ? 0 : value;

                if (result != _CurrentFrame)
                {
                    _CurrentFrame = result;
                }
            }
        }

        public Boolean AddFrames(T[,] data, [NotNullWhen(true)] out T[,]? allData)
        {
            if (data.GetLength(1) <= 0)
            {
                allData = null;
                return false;
            }
            if (_DataLength != data.GetLength(1))
            {
                Reset();
                _DataLength = data.GetLength(1);
            }
            
            List<T[]> frames = new();

            for (Int32 i = 0; i < data.GetLength(0); i++)
            {
                frames.Add(new T[data.GetLength(1)]);
                for (Int32 j = 0; j < data.GetLength(1); j++)
                {
                    frames[i][j] = data[i, j];
                }
            }
            
            lock (_Locker)
            {
                _PersistentData.AddRange(frames);

                if (_PersistentData.Count > _MaxFrameNumber)
                {
                    Int32 count = _PersistentData.Count - _MaxFrameNumber;
                    for (Int32 i = 0; i < count; i++)
                    {
                        _PersistentData.RemoveAt(0);
                    }
                }
            }
            allData = GetPersistentData();
            return allData == null ? false : true;
        }

        public Boolean AddFrames(T[] data, [NotNullWhen(true)] out T[,]? allData)
        {
            if (data.Length <= 0)
            {
                allData = null;
                return false;
            }
            if (_DataLength != data.Length)
            {
                Reset();
                _DataLength = data.Length;
            }

            T[] frames = new T[data.Length];

            for (Int32 j = 0; j < data.Length; j++)
            {
                frames[j] = data[j];
            }
            
            lock (_Locker)
            {
                _PersistentData.Add(frames);

                if (_PersistentData.Count > _MaxFrameNumber)
                {
                    Int32 count = _PersistentData.Count - _MaxFrameNumber;
                    for (Int32 i = 0; i < count; i++)
                    {
                        _PersistentData.RemoveAt(0);
                    }
                }
            }
            allData = GetPersistentData();
            return allData == null ? false : true;
        }

        public T[,]? GetPersistentData()
        {
            if (_PersistentData.Count == 0 || _DataLength == -1)
            {
                return null;
            }

            T[,] persistent;
            T[][] data;

            lock (_Locker)
            {
                data = new T[_PersistentData.Count][];
                _PersistentData.CopyTo(data);
                persistent = new T[_PersistentData.Count, _DataLength];
            }

            for (Int32 i = 0; i < data.Length; i++)
            {
                for (Int32 j = 0; j < _DataLength; j++)
                {
                    persistent[i, j] = data[i][j];
                }
            }

            return persistent;
        }

        public void Reset()
        {
            lock (_Locker)
            {
                _PersistentData.Clear();
                _CurrentFrame = 0;
            }
        }

    }
}
