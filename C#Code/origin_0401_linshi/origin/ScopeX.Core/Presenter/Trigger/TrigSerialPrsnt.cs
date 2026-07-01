using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    /// <summary>
    /// 协议触发
    /// </summary>
    public abstract class TrigSerialPrsnt : TriggerPrsnt, ITriggerSerialPrsnt
    {
        public TrigSerialPrsnt(IDsoPrsnt idp, SerialProtocolType tt = SerialProtocolType.Close, ITriggerSerialView? view = null) : base(idp, view)
        {
            TrigSerialModel = (TriggerSerialModel)DsoModel.Default.GetTriggerModel(TriggerType.Serial);

            LoadEvent();

            ///但在协议触发中切换协议类型时需要同时将解码的类型一起修改了，否则会导致触发中解码的子控件无法正常加载
            ///在Model不进行同步，只是在Prsnt中进行同步
            //if (view is IProtocolView decodeView && decodeView != null)
            //    ProtocolPrsnt.GetTrigSerialDecodePrsnt(idp, tt, decodeView);
            //else
            //    ProtocolPrsnt.GetTrigSerialDecodePrsnt(idp, tt, null);
        }

        /// <summary>
        /// 重载参数
        /// </summary>
        public override void LoadEvent()
        {
            if(TrigSerialModel!=null)
            {
                KeyLed.Default.SetTriggerSrc(ChannelId.Ext);
                TrigSerialModel.PropertyChanged += OnPropertyChanged;
            }
        }

        /// <summary>
        /// 切换类型，注销事件
        /// </summary>
        public override void DisposeEvent()
        {
            if (TrigSerialModel != null)
            {
                TrigSerialModel.PropertyChanged -= OnPropertyChanged;
            }
        }

        private protected TriggerSerialModel TrigSerialModel
        {
            get;
            set;
        }

        public override Double PosIndex
        {
            get;
            set;
        }

        public ChannelId Source
        {
            get => TrigSerialModel.Source;
            set => TrigSerialModel.Source = value;
        }

        public SerialProtocolType SerialType
        {
            get => TrigSerialModel.SerialType;
            set => TrigSerialModel.SerialType = value;
        }

        public virtual String ConditionName { get; } = String.Empty;

        public override void ResetPosIndex()
        {

        }

        public SerialProtocolType GetSerialTypeByChannelId(ChannelId id, Boolean needactive)
        {
            var serialtype = SerialProtocolType.Close;
            if (TrigSerialModel!=null)
            {
                serialtype= TrigSerialModel.GetSerialTypeByChannelId(id,needactive);
            }

            return serialtype;
        }

        public Boolean TryAddView(ITriggerSerialView vu)
        {
            return base.TryAddView(vu);
        }

        public Boolean TryRemoveView(ITriggerSerialView vu)
        {
            return base.TryRemoveView(vu);
        }

        List<ITriggerSerialView> IPresenter<ITriggerSerialView>.GetViewList()
        {
            return base.GetViewList().Where(x => x is ITriggerSerialView).Cast<ITriggerSerialView>().ToList();
        }
    }
}
