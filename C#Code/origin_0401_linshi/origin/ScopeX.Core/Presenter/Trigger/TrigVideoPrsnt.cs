// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;

    /// <summary>
    /// Defines the <see cref="TrigVideoPrsnt" />.
    /// </summary>
    public class TrigVideoPrsnt : TrigSingleSrcPrsnt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrigVideoPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView?"/>.</param>
        /// <param name="mco">The mco<see cref="ModelCreateOptions"/>.</param>
        public TrigVideoPrsnt(IDsoPrsnt idp, ITriggerView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp, view)
        {
            _Model = mco switch
            {
                ModelCreateOptions.Dependant => (TriggerVideoModel)DsoModel.Default.GetTriggerModel(TriggerType.Video),
                ModelCreateOptions.Standalone => new(),
                _ => null,
            };
            LoadEvent();
        }

        /// <summary>
        /// 重载参数
        /// </summary>
        public override void LoadEvent()
        {
            if (_Model != null && Source != null)
            {
                KeyLed.Default.SetTriggerSrc(Source!.Value);
                _Model.PropertyChanged += OnPropertyChanged;
            }
        }

        /// <summary>
        /// 切换类型，注销事件
        /// </summary>
        public override void DisposeEvent()
        {
            if (_Model != null && Source != null)
            {
                _Model.PropertyChanged -= OnPropertyChanged;
            }
        }

        /// <summary>
        /// Gets or sets the Field.
        /// </summary>
        public Int16 Field
        {
            get => Model.Field;
            set
            {
                Model.Field = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        //public Int16 SpecLine
        //{
        //    get => Model.SpecifiedLine;
        //    set => Model.SpecifiedLine = value;
        //}
        /// <summary>
        /// Gets or sets the Line.
        /// </summary>
        public Int16 Line
        {
            get => Model.Line;
            set
            {
                Model.Line = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets the MaxField.
        /// </summary>
        public Int16 MaxField => Model.MaxField;

        /// <summary>
        /// Gets the MaxLine.
        /// </summary>
        public Int16 MaxLine => Model.MaxLine;



        /// <summary>
        /// Gets the MinField.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int16 MinField => 1;

        /// <summary>
        /// Gets the MinLine.
        /// </summary>
        public Int16 MinLine => Model.MinLine;

        /// <summary>
        /// Gets or sets the Polarity.
        /// </summary>
        public VideoPolarity Polarity
        {
            get => Model.Polarity;
            set
            {
                Model.Polarity = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        //public String Name => Model.Name;

        /// <summary>
        /// Gets or sets the Standard.
        /// </summary>
        public VideoStandard Standard
        {
            get => Model.Standard;
            set
            {
                Model.Standard = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets or sets the Sync.
        /// </summary>
        public VideoSync Sync
        {
            get => Model.Sync;
            set
            {
                Model.Sync = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets the Model.
        /// </summary>
        private protected override TriggerVideoModel Model => (TriggerVideoModel)_Model!;
        public void ScpiResetLine()
        {
            Line = MinLine;
        }
    }
}
