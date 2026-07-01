using EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Decode;
using ScopeX.Core.Presenter;

namespace ScopeX.U2
{
    internal class DecodeApp
    {
        public static DecodeApp Default
        {
            get;
            internal set;
        }

        public Action<Boolean> UpdateEventState { get; set; }

        private PageInfoPrsnt _PageInfo;
        public PageInfoPrsnt PageInfo
        {
            get
            {
                if(_PageInfo==null)
                {
                    _PageInfo = new PageInfoPrsnt(DsoPrsnt.DefaultDsoPrsnt);
                }

                return _PageInfo;
            }
        }
        public Dictionary<ChannelId, Dictionary<SerialProtocolType,IProtocolView>> ChannelsView { get; private set; } = new Dictionary<ChannelId, Dictionary<SerialProtocolType,IProtocolView>>();
        public Dictionary<SerialProtocolType,IProtocolView> TriggerDecodeViews { get; private set; } = new Dictionary<SerialProtocolType,IProtocolView>();
        public Dictionary<SerialProtocolType, ITriggerSerialView> TriggerViews { get; private set; } = new Dictionary<SerialProtocolType, ITriggerSerialView>();
        private readonly Dictionary<ChannelId, DecodePrsnt> _Presenters;

        public DecodeApp(IEnumerable<IChnlPrsnt> prsnts)
        {
            DecodeViews = DllLoader<Protocol.DecodeView>.LoadDecodeView();

            _Presenters = prsnts?.OfType<DecodePrsnt>().ToDictionary((dp) => dp.Id);
            ChannelIdExt.GetDecodes().ToList().ForEach(x =>
            {
                Dictionary<SerialProtocolType, IProtocolView> dic = new Dictionary<SerialProtocolType, IProtocolView>();
                DecodeViews.ForEach(y =>
                {
                    dic[y.ProtocolType] = y.ProtocolView;
                    dic[y.ProtocolType].Id = x;
                });
                ChannelsView[x] = dic;
            });
            DecodeViews.ForEach(x =>
            {
                TriggerDecodeViews[x.ProtocolType] = x.ProtocolView;
                TriggerViews[x.ProtocolType] = x.TriggerSerialView;
            });
        }

        public List<Protocol.DecodeView> DecodeViews
        {
            get;
            private set;
        }

        public Boolean TryGetPresenter(ChannelId id,out DecodePrsnt prsnt)
        {
            if (_Presenters.TryGetValue(id, out prsnt))
            {
                return true;
            }

            return false;
        }

        public Boolean IsEventOpen => !DecodeEventTableForm.Default.IsCanShow;

        public void ShowEventLists(ChannelId id)
        {
            if (DecodeEventTableForm.Default.IsCanShow)
            {
                EventBroker.Instance.GetEvent<ScopeX.Controls.Common.Structs.FormEventArgs>().Publish(this, new() { Current = DecodeEventTableForm.Default, Type = ScopeX.Controls.Common.Structs.FormType.InfoForm });
            }
            DecodeEventTableForm.Default.LoadSourceEventInfos(id);
        }

        public void HideEventLists()
        {
            if(!DecodeEventTableForm.Default.IsClosed)
            {
                DecodeEventTableForm.Default?.Close();
            }
        }
        public DecodePrsnt GetPresenter(ChannelId id)
        {
            if (_Presenters.TryGetValue(id, out DecodePrsnt prsnt))
            {
                return prsnt;
            }

            return prsnt;
        }

        public DecodeForm MakeForm(ChannelId id)
        {
            DecodeForm form = new DecodeForm()
            {
                Id= id,
                DecodePresenter = GetPresenter(id),
                Anchor = AnchorStyles.Bottom,
                Text = GetPresenter(id).Name,
            };
            form.DecodePresenter.TryAddView(form);
            return form;
        }
    }
}
