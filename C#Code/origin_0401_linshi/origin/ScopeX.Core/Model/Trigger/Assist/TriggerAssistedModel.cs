using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    internal class TriggerAssistedModel : INotifyPropertyChanged
    {
        public TriggerAssistedModel(LocationAssistedModel locationAssisted, VisualTriggerModel visualTrigger)
        {
            LocationAssisted = locationAssisted;
            VisualTrigger = visualTrigger;
        }

        protected readonly VisualTriggerModel VisualTrigger;
        protected readonly LocationAssistedModel LocationAssisted;

        private Boolean _Enabled = false;
        public Boolean Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled != value)
                {
                    _Enabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean VisualTriggerEnabled
        {
            get => VisualTrigger.Enabled;
            set
            {
                if (VisualTrigger.Enabled != value)
                {
                    VisualTrigger.Enabled = value;
                    if (VisualTrigger.Enabled)
                    {
                        LocationAssisted.Enabled = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        public Boolean LocationAssistedEnabled
        {
            get => LocationAssisted.Enabled;
            set
            {
                if (LocationAssisted.Enabled != value)
                {
                    LocationAssisted.Enabled = value;
                    if (LocationAssisted.Enabled)
                    {
                        VisualTrigger.Enabled = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        protected PropertyChangedEventHandler? _PropertyChanged;

        public virtual event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
                TriggerShareParameter.Default.PropertyChanged += value;
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
                TriggerShareParameter.Default.PropertyChanged -= value;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
