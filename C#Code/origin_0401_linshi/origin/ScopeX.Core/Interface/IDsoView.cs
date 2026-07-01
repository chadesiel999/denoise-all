using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    public interface IDsoView
    {
        IDsoPrsnt Presenter
        {
            get;
            set;
        }

        IBadgeView GetBadge(ChannelId id);

        ITimebaseView TimebaseVu
        {
            get;
        }

        IArtificialIntelligenceView ArtificialIntelligenceView
        {
            get;
        }

        ITriggerView TriggerVu
        {
            get;
        }

        //IMeasView MeasVu
        //{
        //    get;
        //}

        //ICursorView CursorVu
        //{
        //    get;
        //}

        //IMarkerView MarkerVu
        //{
        //    get;
        //}

        //ICymometerView CymometerVu
        //{
        //    get;
        //}

        //IVoltmeterView VoltmeterVu
        //{
        //    get;
        //}

        void UpdateView(Object sender, String propertyName);
    }
}
