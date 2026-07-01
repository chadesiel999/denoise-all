using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    public class ReferencePrsnt : ChannelPrsnt
    {
        private protected override ReferenceModel Model
        {
            get;
        }

        private ReferencePrsnt(IDsoPrsnt idp, ReferenceModel rm) : base(idp, null)
        {
            Model = rm;
            Model.PropertyChanged += OnPropertyChanged;
            Model.Sampling.PropertyChanged += OnPropertyChanged;

            Sampling = new SamplingPrsnt(Model.Sampling);

            Model.Conditioning.Prompter = WeakTip.Default;
            Model.Sampling.Prompter = WeakTip.Default;
            TriggerIndex = -(Int64)((Sampling.PositionByus / (Sampling.Scale * Constants.VIS_XDIVS_NUM) - 0.5) * Pack!.Length);
        }

        public String FullFileName => Model.FullFileName;

        public Int64 TriggerIndex { get; init; }

        /// <summary>
        /// Vertical zero position, its unit is usually 'mV'.
        /// </summary>
        public Double Position => Model.Conditioning.Position;

        ///// <summary>
        ///// Vertical scale value, its unit is usually 'mV'.
        ///// </summary>
        //public Double Scale
        //{
        //    get => Model.Conditioning.ScaleBymV;
        //    set
        //    {
        //        Model.Conditioning.Scale = value;
        //        Dispatcher.SoftReset();
        //    }
        //}

        #region 幅度细调

        public Double ScaleBymV
        {
            get => Model.Conditioning.ScaleBymV;
            set
            {
                //设置当前原本的档位
                var oldscale = Model.Conditioning.ScaleBymV;
                var oldposindex = Model.Conditioning.PosIndex;
                Model.ScaleBymV = value;
                UpdateHCursorPosByScale(oldscale, oldposindex, Ylevel_SelectStatus);
            }
        }

        /// <summary>
        /// 设置垂直挡位幅度
        /// </summary>
        /// <param name="saleValue"></param>
        public void SetScaleValueBymV(Int32 saleValue)
        {
            Double oldscale = Model.Conditioning.ScaleBymV;
            var oldposindex = Model.Conditioning.PosIndex;
            Model.SetScaleValueBymV((Int32)saleValue);
            UpdateHCursorPosByScale(oldscale, oldposindex, Ylevel_SelectStatus);
        }

        private Boolean _Ylevel_SelectStatus = false;

        /// <summary>
        /// 幅度细调选中状态
        /// </summary>
        public Boolean Ylevel_SelectStatus
        {
            get { return Model.Ylevel_SelectStatus; }
            set
            {
                if (Model.Ylevel_SelectStatus != value)
                {
                    FineStatusChanged = true;
                    //设置当前原本的档位
                    var oldscale = Model.Conditioning.ScaleBymV;
                    var oldposindex = Model.Conditioning.PosIndex;
                    Model.Ylevel_SelectStatus = value;
                    if (FineStatusChanged)
                    {
                        UpdateHCursorPosByScale(oldscale, oldposindex, Ylevel_SelectStatus);
                        FineStatusChanged = false;
                    }
                }
            }
        }

        #endregion



        public Double MaxScale => Model.Conditioning.MaxScale;

        public Double MinScale => Model.Conditioning.MinScale;


        public override SamplingPrsnt Sampling
        {
            get;
        }

        /// <summary>
        /// <Remark>更改人：彭博 创建日期：2023/12/04 16:41:00 原因：当关闭信号源时，校验信源的选择状态</Remark>
        /// </summary>
        public override Boolean Active
        {
            get => base.Active;
            set
            {
                if (!Constants.ENABLE_Ref)
                {
                    WeakTip.Default.Write("Ref", MsgTipId.FunctionDisabled);
                    base.Active = false;
                    return;
                }
                if (value != base.Active && !value)
                {
                    if (Decode.DecodeDataHelper.Instance.ReferenceDataSource != null && Core.Decode.DecodeDataHelper.Instance.ReferenceDataSource.Length > Id - ChannelIdExt.MinRChId)
                    {
                        Decode.DecodeDataHelper.Instance.ReferenceDataSource[Id - ChannelIdExt.MinRChId].HasData = false;
                    }
                }
                base.Active = value;
                if (!_ReloadFile && !value)
                {
                    SetMeasureSource();
                }
            }
        }

        private Boolean _ReloadFile = false;

        public static Boolean TryReadSVG(ChannelId id, IDsoPrsnt idp, String fullpath, ref ReferencePrsnt? rprsnt)
        {
            if (!Constants.ENABLE_Ref)
            {
                WeakTip.Default.Write("Ref", MsgTipId.FunctionDisabled);
                return false;
            }
            var rm = rprsnt?.Model;
            try
            {
                if (ReferenceModel.TryReadCSV(id, fullpath, ref rm))
                {
                    if (rprsnt == null)
                    {
                        //Add a new reference channel model
                        DsoModel.Default.AddChannel(id, rm!);
                        //Create reference channel presenter
                        rprsnt = new(idp, rm!);
                    }
                    rprsnt._ReloadFile = true;
                    rprsnt.Active = false;
                    rprsnt._ReloadFile = false;
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
        public static Boolean TryRead(ChannelId id, IDsoPrsnt idp, String fullpath, ref ReferencePrsnt? rprsnt)
        {
            if (!Constants.ENABLE_Ref)
            {
                WeakTip.Default.Write("Ref", MsgTipId.FunctionDisabled);
                return false;
            }
            var rm = rprsnt?.Model;
            try
            {
                if (ReferenceModel.TryRead(id, fullpath, ref rm))
                {
                    if (rprsnt == null)
                    {
                        //Add a new reference channel model
                        DsoModel.Default.AddChannel(id, rm!);
                        //Create reference channel presenter
                        rprsnt = new(idp, rm!);
                    }

                    rprsnt._ReloadFile = true;
                    rprsnt.Active = false;
                    rprsnt._ReloadFile = false;

                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
        public static Boolean TryAddRefPrsnt(ChannelId id, String fullFileName, ref ReferencePrsnt? rprsnt)
        {
            if (File.Exists(fullFileName))
            {
                FileInfo fileinfo = new FileInfo(fullFileName);

                if (fileinfo.Extension == "." + WfmFormat.Binary.GetAlias())
                {
                    if (ReferencePrsnt.TryRead(id, DsoPrsnt.DefaultDsoPrsnt, fullFileName, ref rprsnt))
                    {
                        DsoPrsnt.DefaultDsoPrsnt.AddChannel(id, rprsnt!);
                        DsoPrsnt.FocusId = rprsnt!.Id;
                        rprsnt.Active = true;
                        return true;
                    }
                }

                if (fileinfo.Extension == "." + WfmFormat.CSV.GetAlias())
                {
                    if (ReferencePrsnt.TryReadSVG(id, DsoPrsnt.DefaultDsoPrsnt, fullFileName, ref rprsnt))
                    {
                        DsoPrsnt.DefaultDsoPrsnt.AddChannel(id, rprsnt!);
                        DsoPrsnt.FocusId = rprsnt!.Id;
                        rprsnt.Active = true;
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置参考通道参数测量item
        /// </summary>
        /// <Remark>更改人：彭博 创建日期：2023/12/04 16:41:00 原因：当关闭信号源时，校验信源的选择状态</Remark>
        public void SetMeasureSource()
        {
            if (null != DsoPrsnt.DefaultDsoPrsnt)
            {
                //获取每一项参数
                foreach (var item in DsoPrsnt.DefaultDsoPrsnt?.Measure.SelectedItems)
                {
                    ////获取所有开启的通道编号
                    //var srcs = DsoPrsnt.DefaultDsoPrsnt.FindIdentities(c => c.Id.IsAnalog() || (c.Active && (c.Id.IsReference() /*|| c.Id.IsMath()*/)));
                    ////判断当前信通道是否存在
                    //if (!srcs.Contains(item.Source))
                    //{
                    //    //若不存在，就默认第一个通道
                    //    item.Source = srcs.Count > 0 ? srcs[0] : (ChannelId)0;
                    //}
                    if (item.Source == this.Id || (item.Source2nd == this.Id && item.IsSource2ndActive))
                    {
                        item.Active = false;
                    }
                }
            }
        }
    }
}
