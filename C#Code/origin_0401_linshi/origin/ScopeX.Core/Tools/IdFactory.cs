using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Tools
{
    public static class IdFactory
    {
        //private static readonly Int64 _WorkerId = 0;
        //private static readonly Int32 _WorkerIdShift = _SequenceBits; //机器码数据左移位数，就是后面计数器占用的位数

        private static readonly Int64 _Epoch = 687888001020L; //唯一时间，这是一个避免重复的随机量，自行设定不要大于当前时间戳

        private static Int64 _Sequence = 0L;
        private static readonly Int32 _SequenceBits = 10; //计数器位数，10个字节用来保存计数码
        private static readonly Int64 _SequenceMask = -1L ^ -1L << _SequenceBits; //一微秒内可以产生计数，如果达到该值则等到下一微妙在进行生成

        private static readonly Int32 _IdBits = 4; //机器码字节数。4个字节用来保存机器码(定义为Int64类型会出现，最大偏移64位，所以左移64位没有意义)
                                                   //private static readonly Int64 _MaxId = -1L ^ -1L << _IdBits; //最大机器ID


        private static readonly Int32 _TimestampShift = _SequenceBits + _IdBits; //时间戳左移动位数就是机器码和计数器总字节数

        private static Int64 _LastTimestamp = -1L;

        private static readonly Object _Locker = new Object();

        public static Int64 NextId
        {
            get
            {
                lock (_Locker)
                {
                    Int64 nowtimestamp = GetNowTime();
                    if (nowtimestamp < _LastTimestamp)
                    { //如果当前时间戳比上一次生成ID时时间戳还小，抛出异常，因为不能保证现在生成的ID之前没有生成过
                        throw new Exception(String.Format("Clock moved backwards. Refusing to generate id for {0} milliseconds",
                            _LastTimestamp - nowtimestamp));
                    }

                    if (_LastTimestamp == nowtimestamp)
                    { //同一微妙中生成ID
                        IdFactory._Sequence = (IdFactory._Sequence + 1) & IdFactory._SequenceMask; //用&运算计算该微秒内产生的计数是否已经到达上限
                        if (IdFactory._Sequence == 0)
                        {
                            //一微妙内产生的ID计数已达上限，等待下一微妙
                            nowtimestamp = GetNextTime(_LastTimestamp);
                        }
                    }
                    else
                    { //不同微秒生成ID
                        IdFactory._Sequence = 0; //计数清0
                    }

                    _LastTimestamp = nowtimestamp; //把当前时间戳保存为最后生成ID的时间戳
                    Int64 nextid = (nowtimestamp - _Epoch << _TimestampShift) /*| IdFactory._WorkerId << IdFactory._WorkerIdShift */ | IdFactory._Sequence;
                    return nextid;
                }
            }
        }

        /// <summary>
        /// 获取下一微秒时间戳
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        private static Int64 GetNextTime(Int64 lastTimestamp)
        {
            Int64 timestamp = GetNowTime();
            while (timestamp <= lastTimestamp)
            {
                timestamp = GetNowTime();
            }
            return timestamp;
        }

        /// <summary>
        /// 生成当前时间戳
        /// </summary>
        /// <returns></returns>
        private static Int64 GetNowTime()
        {
            //return (Int64)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            //return Stopwatch.GetTimestamp();
            return ComModel.TimeSpanUtility.GetTimesLongByms();
        }
    }
}
