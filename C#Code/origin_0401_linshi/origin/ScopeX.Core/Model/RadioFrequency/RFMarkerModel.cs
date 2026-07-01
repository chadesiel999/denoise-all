using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ScopeX.ComModel;
using ScopeX.Core.Model.RadioFrequency;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    internal class MarkerModel : INotifyPropertyChanged
    {
        private List<MarkerItemModel> _Items = new List<MarkerItemModel>();
        public List<MarkerItemModel> Items
        {
            get => _Items;
            set
            {
                if (_Items != value)
                {
                    _Items = value;
                }
            }
        }

        public MarkerModel()
        {
        }


        public void Run()
        {
            var markeritems = Items.Where(x => x.AtuoMarkerActive);
            if (markeritems.Count()>0)
            {
                foreach (var item in markeritems)
                {
                    item.AutoMarker.Run();
                }
            }
        }



        public Boolean AddMarker(ChannelId source, out MarkerItemModel model, ChannelId id = ChannelId.MAKER1)
        {
            var item = _Items.FirstOrDefault(x => x.Id == id);
            if (item==null)
            {
                model = new MarkerItemModel(source,id);
                _Items.Add(model);
                return true;
            }
            model = item;
            return false;
        }

        public Boolean TryRemoveItem(ChannelId id)
        {
            var item = _Items.FirstOrDefault(x => x.Id == id);
            if(item!=null)
            {
                _Items.Remove(item);
                return true;
            }

            return false;
        }


        #region INotifyPropertyChanged
        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
