using System;
using System.Collections.Concurrent;

namespace ScopeX.ComModel
{
    public enum FalshOpreation
    {
        None = 0,
        Read = 1,
        Write = 2,
    }

    public enum FalshOpreationResult
    {
        None = 0,
        Success = 1,
        Fail = 2
    }

    public class DsoInfos
    {
        public ProductInfos ProductInfos;

        public OptionsInfos OptionsInfos;

        public CaliDatasInfos CaliDatasInfos;

        public DsoInfos()
        {
            ProductInfos = new();
            OptionsInfos = new();
            CaliDatasInfos = new();
        }
    }

    public class ProductInfos
    {
        public volatile FalshOpreation SyncOperation = FalshOpreation.Read;

        public volatile FalshOpreationResult SyncResult = FalshOpreationResult.None;

        private String? _ProductModel = null;
        /// <summary>
        /// 产品型号
        /// </summary>
        public String ProductModel
        {
            get => String.IsNullOrEmpty(_ProductModel) ? "NULL" : _ProductModel;
            set
            {
                if (_ProductModel != value)
                {
                    _ProductModel = value;
                }
            }
        }

        private String? _SerialNumber = null;
        /// <summary>
        /// SN
        /// </summary>
        public String SerialNumber
        {
            get => String.IsNullOrEmpty(_SerialNumber) ? "NULL" : _SerialNumber;
            set
            {
                if (_SerialNumber != value)
                {
                    _SerialNumber = value;
                }
            }
        }

        private String? _ChannelInfo = null;
        /// <summary>
        /// 渠道信息
        /// </summary>
        public String ChannelInfo
        {
            get => String.IsNullOrEmpty(_ChannelInfo) ? "NULL" : _ChannelInfo;
            set
            {
                if (_ChannelInfo != value)
                {
                    _ChannelInfo = value;
                }
            }
        }

        private DateTime _ProductionDate = DateTime.Now;
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime ProductionDate
        {
            get => _ProductionDate;
            set
            {
                if (_ProductionDate != value)
                {
                    _ProductionDate = value;
                }
            }
        }

        private String? _OtherInfo = null;
        /// <summary>
        /// 备注
        /// </summary>
        public String OtherInfo
        {
            get => String.IsNullOrEmpty(_OtherInfo) ? "NULL" : _OtherInfo;
            set
            {
                if (_OtherInfo != value)
                {
                    _OtherInfo = value;
                }
            }
        }

        private String? _ProtocolVersion = null;
        /// <summary>
        /// 协议版本号
        /// </summary>
        public String ProtocolVersion
        {
            get => String.IsNullOrEmpty(_ProtocolVersion) ? "NULL" : _ProtocolVersion;
            set
            {
                if (_ProtocolVersion != value)
                {
                    _ProtocolVersion = value;
                }
            }
        }

        private String? _HardVersion = null;
        /// <summary>
        /// 协议版本号
        /// </summary>
        public String HardVersion
        {
            get => String.IsNullOrEmpty(_HardVersion) ? "NULL" : _HardVersion;
            set
            {
                if (_HardVersion != value)
                {
                    _HardVersion = value;
                }
            }
        }

        public void InitInfos()
        {
            _SerialNumber = null;
            _ProtocolVersion = null;
            _HardVersion = null;
            _OtherInfo = null;
            _ChannelInfo = null;
            _ProductModel = null;
            _ProductionDate = DateTime.MinValue;
        }

        public void CloneTo(ProductInfos? source)
        {
            if (source == null)
            {
                return;
            }
            source.SerialNumber = SerialNumber;
            source.ProtocolVersion = ProtocolVersion;
            source.HardVersion = HardVersion;
            source.OtherInfo = OtherInfo;
            source.ChannelInfo = ChannelInfo;
            source.ProductModel = ProductModel;
            source.ProductionDate = ProductionDate;
            source.SyncOperation = SyncOperation;
            source.SyncResult = SyncResult;
        }
    }

    public class OptionsInfos
    {
        public volatile FalshOpreation SyncOperation = FalshOpreation.Read;

        public volatile FalshOpreationResult SyncResult = FalshOpreationResult.None;

        public ConcurrentDictionary<String, Boolean>? AllOptions = new();

        public Double _TrialRemainingTimeByHour = Constants.DEFAULT_REMAININGTIME_BYHOUR;
        public Double TrialRemainingTimeByHour
        {
            get => _TrialRemainingTimeByHour;
            set
            {
                if (value != _TrialRemainingTimeByHour)
                {
                    _TrialRemainingTimeByHour = value;
                }
            }
        }

        public void InitInfos()
        {
            AllOptions?.Clear();
            _TrialRemainingTimeByHour = Constants.DEFAULT_REMAININGTIME_BYHOUR;
        }

        public void ResetTrialRemainingTime(Boolean isOverTime = false)
        {
            _TrialRemainingTimeByHour = isOverTime ? 0 : Constants.DEFAULT_REMAININGTIME_BYHOUR;
        }

        public void CloneTo(OptionsInfos target)
        {
            if (target == null)
            {
                return;
            }
            if (AllOptions != null)
            {
                if (target.AllOptions != null)
                {
                    foreach (var item in AllOptions)
                    {
                        target.AllOptions[item.Key] = item.Value;
                    }
                }
                else
                {
                    target.AllOptions = new();
                    foreach (var item in AllOptions)
                    {
                        target.AllOptions[item.Key] = item.Value;
                    }
                }
            }
            target.TrialRemainingTimeByHour = TrialRemainingTimeByHour;
            target.SyncResult = SyncResult;
            target.SyncOperation = SyncOperation;
        }
    }

    public class CaliDatasInfos
    {
        public volatile FalshOpreation SyncOperation = FalshOpreation.None;

        public volatile FalshOpreationResult SyncResult = FalshOpreationResult.None;
    }
}
