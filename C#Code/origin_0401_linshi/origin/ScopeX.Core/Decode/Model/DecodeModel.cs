using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Core.Decode;
using ScopeX.Core.Tools;
using ScopeX.ComModel;
using NPOI.SS.Formula;
using System.Runtime.CompilerServices;
using NPOI.SS.Formula.Atp;
using System.Threading;

namespace ScopeX.Core.Decode
{
    internal class DecodeModel : ChannelModel
    {
        /// <summary>
        /// 在拖动波形位置时加上防抖
        /// </summary>
        private System.Timers.Timer _DecodeDebouncTimer = new System.Timers.Timer(500);
        private CancellationTokenSource _TokenSource = new CancellationTokenSource();
        internal sealed class ConditioningModel : VertAxisModel
        {
            public ConditioningModel() : base("Conditioning")
            {
                base.InitialScale = (0, 0.5);

                Prefix = Prefix.Milli;
                Unit = "V";

                PosMaxIndex = Constants.MAX_YPOS_IDX;
                PosMinIndex = Constants.MIN_YPOS_IDX;
                PosIndex = Constants.DEF_YPOS_IDX;

            }
        }

        public DecodeModel(ChannelId id, Color color, Boolean active, TimebaseModel tmb)
            : base(ChannelType.Decode, id, color)
        {
            Active = active;

            _DecodeDebouncTimer.AutoReset = false;
            _DecodeDebouncTimer.Enabled = false;
            _DecodeDebouncTimer.Stop();
            _DecodeDebouncTimer.Elapsed += (_, _) => DecodePacketData();
            Conditioning = new ConditioningModel();
            Sampling = tmb;
            Sampling.PropertyChanged += (sender, args) =>
            {
                try
                {
                    _DecodeDebouncTimer.Stop();
                    _DecodeDebouncTimer.Start();
                }
                catch (Exception ex)
                {

                }
            };
            Label = "";
            DecodeProtocolShareParameter.Default.PropertyChanged += (sender, args) =>
            {
                if (sender is ProtocolModel protocol && protocol == _CurrentDecodeModel)
                {
                    DecodePacketData();
                }
            };

        }


        public override ConditioningModel Conditioning
        {
            get;
        }


        public override TimebaseModel Sampling
        {
            get;
        }


        #region 属性

        public SerialProtocolType ProtocolType
        {
            get => _CurrentDecodeModel == null ? SerialProtocolType.Close : _CurrentDecodeModel.ProtocolType;
            set
            {
                if (_CurrentDecodeModel == null || _CurrentDecodeModel.ProtocolType != value)
                {
                    _CurrentDecodeModel = GetChDecodeModel(value);
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _EventEnable = false;
        public Boolean EventEnable
        {
            get => _EventEnable;
            set
            {
                if (_EventEnable != value)
                {
                    _EventEnable = value;
                    OnPropertyChanged();
                }
            }
        }
        public void NotifyProtolTypeChanged()
        {
            OnPropertyChanged(nameof(ProtocolType));
            //if (_CurrentDecodeModel != null)
            //    _CurrentDecodeModel.NotifyProtocolTypeChanged();
        }


        public DecodeDisplayMode _Format = DecodeDisplayMode.Hex;
        public DecodeDisplayMode Format
        {
            get => _Format;
            set
            {
                if (value != _Format)
                {
                    _Format = value;
                    OnPropertyChanged();
                }
            }
        }
        public override void ClearBuffer()
        {
            _TokenSource?.Cancel();
            if (_CurrentDecodeModel == null)
            {
                return;
            }
            _CurrentDecodeModel.CancelFlag = true;
        }

        public void DecodePacketData()
        {
            if (!Active) return;
            if (_CurrentDecodeModel == null)
            {

            }
            else
            {
                _TokenSource?.Cancel();
                _CurrentDecodeModel.CancelFlag = true;
                var chs = _CurrentDecodeModel.ActivedChannels;
                _TokenSource = new CancellationTokenSource();
                _CurrentDecodeModel.CancelFlag = false;
                if (chs.Length > 0)
                {

                    var wfminfos = chs.Select(selector: x =>
                    {
                        ProtocolModel.WfmInfo wfmInfo = new ProtocolModel.WfmInfo();
                        ChannelModel ch = DsoModel.Default.GetChannel(x);
                        wfmInfo.ScaleByPs = ch.Sampling.Scale * 1E6;
                        wfmInfo.PosIndex = ch.Sampling.PosIndex;
                        wfmInfo.Start = ch.VuDatabase.Current?.Start ?? 0;
                        return wfmInfo;
                    }).ToArray();
                    _CurrentDecodeModel.DecodePacketData(wfminfos, ref Unsafe.AsRef(_TokenSource.Token));
                }
            }
        }

        public void ClearPacketData()
        {
            _CurrentDecodeModel?.ClearData();
        }

        public IReadOnlyList<DecodeResultData> DecodePackets => _CurrentDecodeModel == null ? new List<DecodeResultData>() : _CurrentDecodeModel.DecodePackets;

        #endregion 属性

        #region 方法

        /// <summary>
        /// 当前所有被初始化出来的Model
        /// </summary>
        private List<ProtocolModel> _DecodeModels { get; } = new List<ProtocolModel>();


        /// <summary>
        /// 缓存当前通道解码使用的Model
        /// </summary>
        private ProtocolModel? _CurrentDecodeModel = null;

        /// <summary>
        /// 获取当前的解码Model
        /// </summary>
        /// <returns></returns>
        public ProtocolModel GetChDecodeModel()
        {
            if (_CurrentDecodeModel != null)
                return _CurrentDecodeModel;

            if (_DecodeModels.Count > 0)
            {
                _CurrentDecodeModel = _DecodeModels.First();
                Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
                return _CurrentDecodeModel;
            }

            _CurrentDecodeModel = DecodeTools.GetChannelDecodeModel(Id, SerialProtocolType.Close);
            _DecodeModels.Add(_CurrentDecodeModel);
            Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
            return _CurrentDecodeModel;
        }

        /// <summary>
        /// 获取通道解码中指定通道中的Model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="SerialProtocolType"></param>
        /// <returns></returns>
        public ProtocolModel GetChDecodeModel(SerialProtocolType SerialProtocolType)
        {

            if (_CurrentDecodeModel != null && _CurrentDecodeModel.ProtocolType == SerialProtocolType) //判断通道中当前Decode Model是否满足要求
                return _CurrentDecodeModel;

            _CurrentDecodeModel = _DecodeModels.FirstOrDefault(x => x.ProtocolType == SerialProtocolType);
            if (_CurrentDecodeModel == null)
            {
                _CurrentDecodeModel = DecodeTools.GetChannelDecodeModel(Id, SerialProtocolType);
                _DecodeModels.Add(_CurrentDecodeModel);
            }
            Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
            return _CurrentDecodeModel;
        }

        #endregion 方法

    }
}
