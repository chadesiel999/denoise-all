// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/3/29</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;

    /// <summary>
    /// Defines the <see cref="PassFailInfo" />.
    /// </summary>
    public class PassFailInfo
    {
        //scpi查询PF测试中的标题(英文)
        /// <summary>
        /// Defines the titles.
        /// </summary>
        public readonly List<String> Titles= new List<String> { "Test Status", "Total Waveform", "Total Violations", "Testing Time", "Number of Hits", "Hits Per Segment:"};

        //失败的波形数：当前、总计
        /// <summary>
        /// Defines the FailWfms.
        /// </summary>
        public readonly Int64[] FailWfms;

        //当前测试失败波形中的违例点
        /// <summary>
        /// Defines the Hits.
        /// </summary>
        internal List< List<(Double x, Double y)>> HitsBuffer;

        public IReadOnlyList<IReadOnlyList<(Double x, Double y)>> Hits => HitsBuffer.AsReadOnly();

        //违例点的同步锁
        /// <summary>
        /// Defines the HitsLocker.
        /// </summary>
        //internal readonly Object HitsLocker;

        //已测试时间
        /// <summary>
        /// Defines the RunningTime.
        /// </summary>
        public readonly Int64[] RunningTime;

        //测试失败波形中各分段中发生违例的样点数：当前、总计
        /// <summary>
        /// Defines the SegHits.
        /// </summary>
        public readonly Int64[,] SegHits;

        //实时测量运行时间
        /// <summary>
        /// Defines the Timer.
        /// </summary>
        public readonly Stopwatch Timer;

        //测试失败波形中发生违例的样点数：当前、总计
        /// <summary>
        /// Defines the TotalHits.
        /// </summary>
        public readonly Int64[] TotalHits;

        //已测试波形数：当前、总计
        /// <summary>
        /// Defines the TotalWfms.
        /// </summary>
        public readonly Int64[] TotalWfms;

        //已测试次数中失败次数，未使用
        /// <summary>
        /// Defines the FailedTests.
        /// </summary>
        public Int32 FailedTests;

        //已测试次数，未使用
        /// <summary>
        /// Defines the TotalTests.
        /// </summary>
        public Int32 TotalTests;
        public Int64 FailedTimestamp = 0;
        /// <summary>
        /// Initializes a new instance of the <see cref="PassFailInfo"/> class.
        /// </summary>
        public PassFailInfo()
        {
            TotalWfms = new Int64[2];
            FailWfms = new Int64[2];

            TotalHits = new Int64[2];
            SegHits = new Int64[8, 2];
            RunningTime = new Int64[2];

            Timer = new Stopwatch();

            //HitsLocker = new Object();
            HitsBuffer = new List< List<(Double x, Double y)>>();
        }

        /// <summary>
        /// The Reset.
        /// </summary>
        public void Reset()
        {
            TotalWfms[0] = 0;
            FailWfms[0] = 0;
            TotalHits[0] = 0;
            TotalHits[1] = 0;
            for (Int32 i = 0; i < 8; i++)
            {
                SegHits[i, 0] = 0;
                SegHits[i, 1] = 0;
            }
            RunningTime[0] = 0;
            FailedTimestamp = 0;
            //lock (HitsLocker)
            //{
            //    _Hits.Clear();
            //}
        }
    }
}
