using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;

namespace ScopeX.Core
{

    internal class SettingModel : INotifyPropertyChanged
    {
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

        private AuxInputType _AuxInputSignal = AuxInputType.Close;
        public AuxInputType AuxInputSignal
        {
            get => _AuxInputSignal;
            set
            {
                if (_AuxInputSignal != value)
                {
                    _AuxInputSignal = value;
                    OnPropertyChanged();
                }
                Hd.SetAuxInputMux(_AuxInputSignal);
            }
        }
        private EdgeSlope _AuxInPolarity = EdgeSlope.Rise;
        public EdgeSlope AuxInPolarity
        {
            get => _AuxInPolarity;
            set
            {
                if (_AuxInPolarity != value)
                {
                    _AuxInPolarity = value;
                    OnPropertyChanged();
                }
                Hd.SetAuxIutputPolarity(_AuxInPolarity);

            }
        }
        private AuxOutputType _AuxOutputSignal = AuxOutputType.Close;
        public AuxOutputType AuxOutputSignal
        {
            get => _AuxOutputSignal;
            set
            {
                if (_AuxOutputSignal != value)
                {
                    _AuxOutputSignal = value;
                    OnPropertyChanged();
                }
                Hd.SetAuxOutputMux(_AuxOutputSignal);
                if (_AuxOutputSignal == AuxOutputType.Other)
                    Hd.SetAuxOutputSignal(DsoModel.Default.PassFail.Results.FailWfms[0] == 0);
            }
        }

        private EdgeSlope _AuxOutPolarity = EdgeSlope.Rise;
        public EdgeSlope AuxOutPolarity
        {
            get => _AuxOutPolarity;
            set
            {
                if (_AuxOutPolarity != value)
                {
                    _AuxOutPolarity = value;
                    OnPropertyChanged();
                }
                Hd.SetAuxOutputPolarity(_AuxOutPolarity);

            }
        }
    }
}
