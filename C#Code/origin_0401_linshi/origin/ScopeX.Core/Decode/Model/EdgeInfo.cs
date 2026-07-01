using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    //下沉到comModle ljw 
    //public enum Edge
    //{
    //    Falling,
    //    Rise,
    //    None,
    //}

    public abstract class BaseEdgeInfo
    {
        public abstract UInt32 LevelCount { get; }
        public Boolean IsRoot => Parent == null;
        public Boolean IsLast => Child == null;
        [AllowNull] public BaseEdgeInfo Parent { get; private protected set; }
        [AllowNull] public BaseEdgeInfo Child { get; private protected set; }
        public UInt32 EdgeIndex { get; private protected set; }
        public Int32 StartIndex { get; private protected set; }
        public Int32 EndIndex { get; private protected set; }
        public Int32 Length => EndIndex - StartIndex + 1;
        public Edge Edge { get; private protected set; }
        internal BaseEdgeInfo? GetEdgeInfoAt(Int32 edgeindex)
        {
            if (edgeindex == EdgeIndex)
                return this;
            if (edgeindex < 0)
                return null;
            var tempinfo = this;
            if (edgeindex > EdgeIndex)
            {
                tempinfo = this.Child;
                while (tempinfo != null)
                {
                    if (tempinfo.EdgeIndex == edgeindex)
                        return tempinfo;
                    tempinfo = tempinfo.Child;
                }
            }
            else if (edgeindex < EdgeIndex)
            {
                tempinfo = this.Parent;
                while (tempinfo != null)
                {
                    if (tempinfo.EdgeIndex == edgeindex)
                        return tempinfo;
                    tempinfo = tempinfo.Parent;
                }
            }
            return null;
        }
        internal BaseEdgeInfo? GetEdgeInfoByIndex(Int32 index)
        {
            if (index >= StartIndex && index <= EndIndex)
                return this;
            if (index < 0)
                return null;
            var tempinfo = this;
            if (index > tempinfo.EndIndex)
            {
                tempinfo = this.Child;
                //var starttime = DateTime.Now;
                var starttime = ComModel.TimeSpanUtility.GetTimestampSpan();
                while (tempinfo != null)
                {
                    if (index >= tempinfo.StartIndex && index <= tempinfo.EndIndex)
                        return tempinfo;
                    tempinfo = tempinfo.Child;
                    if (( /*DateTime.Now*/ComModel.TimeSpanUtility.GetTimestampSpan() - starttime).TotalMilliseconds > 2000)
                    {
                        return null;
                    }
                }
            }
            else if (index < tempinfo.StartIndex)
            {
                tempinfo = this.Parent;
                //var starttime = DateTime.Now;
                var starttime = ComModel.TimeSpanUtility.GetTimestampSpan();
                while (tempinfo != null)
                {
                    if (index >= tempinfo.StartIndex && index <= tempinfo.EndIndex)
                        return tempinfo;
                    tempinfo = tempinfo.Parent;
                    if (( /*DateTime.Now*/ComModel.TimeSpanUtility.GetTimestampSpan() - starttime).TotalMilliseconds > 2000)
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        internal BaseEdgeInfo? GetNextEdgeInfoByIndex(Int32 index)
        {
            //if (index >= StartIndex && index <= EndIndex) return this;
            if (index < 0)
                return null;
            var tempinfo = this.Child;
            if (tempinfo == null)
            {
                return null;
            }
            //var starttime = DateTime.Now;
            var starttime = ComModel.TimeSpanUtility.GetTimestampSpan();
            if (( /*DateTime.Now*/ComModel.TimeSpanUtility.GetTimestampSpan() - starttime).TotalMilliseconds > 2000)
            {
                return null;
            }
            if (tempinfo.Child != null)
            {
                tempinfo = tempinfo.Child;
                return tempinfo;
            }




            return null;
        }
    }


    public sealed class TwoLevelEdgeInfo : BaseEdgeInfo
    {
        internal static TwoLevelEdgeInfo CreateRoot(Int32 current)
        {
            TwoLevelEdgeInfo info = new TwoLevelEdgeInfo();
            info.CurrentLevel = current == 1;
            info.Edge = Edge.None;
            return info;
        }
        public override UInt32 LevelCount => 2;
        public Boolean CurrentLevel { get; private set; }
        internal TwoLevelEdgeInfo AddChild(Int32 startindex, Int32 current)
        {
            TwoLevelEdgeInfo info = new TwoLevelEdgeInfo();
            info.Parent = this;
            this.Child = info;
            info.EdgeIndex = this.EdgeIndex + 1;
            info.StartIndex = startindex;
            info.CurrentLevel = current==1;
            info.Edge = info.CurrentLevel ? Edge.Rise : Edge.Falling;
            this.EndIndex = startindex - 1;
            return info;
        }
        internal void SetNodeEndIndex(Int32 endIndex)
        {
            if (endIndex > 0)
            {
                this.EndIndex = endIndex;
            }
        }

        public TwoLevelEdgeInfo? GetNextNode(Boolean status)
        {
            var node = this;
            while (node?.Child != null)
            {
                if (node.Child is TwoLevelEdgeInfo three && three.CurrentLevel == status)
                    return three;
                node = node.Child as TwoLevelEdgeInfo;
            }
            return null;
        }
        public TwoLevelEdgeInfo? GetLastNode(Boolean status)
        {
            var node = this;
            while (node?.Parent != null)
            {
                if (node.Parent is TwoLevelEdgeInfo three && three.CurrentLevel == status)
                    return three;
                node = node.Parent as TwoLevelEdgeInfo;
            }
            return null;
        }
    }
    public sealed class ThreeLevelEdgeInfo : BaseEdgeInfo
    {
        internal static ThreeLevelEdgeInfo CreateRoot(Status startlevel)
        {
            ThreeLevelEdgeInfo info = new ThreeLevelEdgeInfo();
            info.CurrentLevel = startlevel;
            info.Edge = Edge.None;
            return info;
        }

        public static Status ConverToStatus(Boolean hilevel, Boolean lowlevel)
        {
            return (Status)(((hilevel ? 1 : 0) << 1) | (lowlevel ? 1 : 0));
        }
        public static Status ConverToStatusExtend(Byte level)
        {
            Status threeStatus = Status.None;
            threeStatus = level switch
            {
                0b00 => Status.Low,
                0b01 => Status.Middle,
                0b11 => Status.High,
                _ => Status.High,
            };
            return threeStatus;
        }

        public enum Status : Byte
        {
            High = 0b11,
            Middle = 0b01,
            Low = 0b00,
            None = 0b10,
        }
        public ThreeLevelEdgeInfo? GetNextNode(Status status)
        {
            var node = this;
            while (node?.Child != null)
            {
                if (node.Child is ThreeLevelEdgeInfo three && three.CurrentLevel == status)
                    return three;
                node = node.Child as ThreeLevelEdgeInfo;
            }
            return null;
        }
        public ThreeLevelEdgeInfo? GetLastNode(Status status)
        {
            var node = this;
            while (node?.Parent != null)
            {
                if (node.Parent is ThreeLevelEdgeInfo three && three.CurrentLevel == status)
                    return three;
                node = node.Parent as ThreeLevelEdgeInfo;
            }
            return null;
        }
        public override UInt32 LevelCount => 3;
        public Status CurrentLevel { get; private set; } = Status.None;
        internal void SetNodeEndIndex(Int32 endIndex)
        {
            if (endIndex > 0)
            {
                this.EndIndex = endIndex;
            }
        }
        public ThreeLevelEdgeInfo AddChild(Int32 startindex, Status currentlevel)
        {
            ThreeLevelEdgeInfo info = new ThreeLevelEdgeInfo();
            info.Parent = this;
            this.Child = info;
            switch (currentlevel)
            {
                case Status.High:
                    info.Edge = Edge.Rise;
                    break;
                case Status.Middle:
                    if (this.CurrentLevel == Status.Low)
                        info.Edge = Edge.Rise;
                    else if (this.CurrentLevel == Status.High)
                        info.Edge = Edge.Falling;
                    else
                        info.Edge = Edge.None;
                    break;
                case Status.Low:
                    info.Edge = Edge.Falling;
                    break;
                default:
                    info.Edge = Edge.None;
                    break;

            }
            info.EdgeIndex = this.EdgeIndex + 1;
            info.StartIndex = startindex;
            info.CurrentLevel = currentlevel;
            this.EndIndex = startindex - 1;
            return info;
        }
    }
}
