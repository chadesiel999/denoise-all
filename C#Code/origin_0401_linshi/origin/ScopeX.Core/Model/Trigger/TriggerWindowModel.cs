// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;

    /// <summary>
    /// Defines the <see cref="TriggerWindowModel" />.
    /// 两个比较电平，超出或进入设定电平范围（大于或小于一定时间）
    /// </summary>
    internal class TriggerWindowModel : TriggerMultiLevelModel
    {
        /// <summary>
        /// Defines the _TimeCondition.
        /// </summary>
        private WindowTimeCondition _TimeCondition = WindowTimeCondition.OnEnter;

        /// <summary>
        /// Defines the _PosCondition.
        /// </summary>
        private WindowRange _PosCondition = WindowRange.Inside;

        /// <summary>
        /// Gets or sets the LevelCompCondition.
        /// </summary>
        public WindowRange PosCondition
        {
            get => _PosCondition;
            set
            {
                if (_PosCondition != value)
                {
                    _PosCondition = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the Name.
        /// </summary>
        public override String Name => TriggerType.Window.ToString();

        /// <summary>
        /// Gets or sets the TimeCondition.
        /// </summary>
        public WindowTimeCondition TimeCondition
        {
            get => _TimeCondition;
            set
            {
                if (_TimeCondition != value)
                {
                    _TimeCondition = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
