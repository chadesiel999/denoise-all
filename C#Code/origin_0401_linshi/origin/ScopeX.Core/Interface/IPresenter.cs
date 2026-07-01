using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Core.Jitter;
using ScopeX.Core.Presenter.RadioFrequency;

namespace ScopeX.Core
{
    public interface IPresenter<V>
    {
        //IEnumerable<V> GetViewList();
        List<V> GetViewList();


        Boolean TryAddView(V vu);


        Boolean TryRemoveView(V vu);
        
    }

    public interface IDisplayPrsnt : IPresenter<IDisplayView>
    { }

    public interface ICursorPrsnt : IPresenter<ICursorView>
    { }

    public interface IMarkerPrsnt : IPresenter<IMarkerView>
    { }

    public interface IMarkerItemPrsnt :IPresenter<IMarkerItemView>
    { }

    public interface IPrintPrsnt : IPresenter<IPrintView>
    { }

    public interface IMultiDomainPrsnt : IPresenter<IMultiDomainView>
    { }

    public interface IXYPrsnt : IPresenter<IXYView>
    { }

    public interface IFilterPrsnt : IPresenter<IFilterView>
    { }

    public interface ILANPrsnt : IPresenter<ILANView>
    { }

    public interface ILocationAssistedPrsnt : IPresenter<ILocationAssistedView>
    { }
    public interface ITriggerAssistedPrsnt : IPresenter<ITriggerAssistedView>
    { }
    public interface IVisualTriggerPrsnt : IPresenter<IVisualTriggerView>
    { }

    public interface IAreaHistogramPrsnt : IPresenter<IAreaHistogramView>
    { }
    //public interface IRadioFrequencyPrsnt : IPresenter<IRadioFrequencyView>
    //{
    //}

    public interface ICymometerPrsnt : IPresenter<IView>, IBadge
    { }

    public interface IVoltmeterPrsnt : IPresenter<IView>, IBadge
    { }

    public interface IMeasPrsnt : IPresenter<IView>
    { }

    public interface ISdaPrsnt : IPresenter<ISdaView>
    { }

    public interface IJitterPrsnt : IPresenter<IJitterView>, IBadge
    { }

    public interface IEyePrsnt : IPresenter<IEyeView>
    { }

    public interface ISystemCheckPrsnt : IPresenter<ISystemCheckView>
    { }
    public interface IArtificialIntelligencePrsnt : IPresenter<IArtificialIntelligenceView>
    { }
    public interface ITempPrsnt : IPresenter<ITempView>{ }

    public interface IExceptionCapturePrsnt : IPresenter<IExceptionCaptureView>, IBadge
    {
    }
}

namespace ScopeX.Core
{
    public interface IPassFailPrsnt : IPresenter<IPassFailView>
    { }

    public interface IPageInfoPrsnt :IPresenter<IPageInfoView>
    { }
}

namespace ScopeX.Core.PowerAnalysis
{
    public interface IPwrAnalysisPrsnt : IPresenter<IPwrAnalysisView>,IBadge
    { }

    public interface IPwrOptionPrsnt : IPresenter<IPwrOptionView>
    { }
}


namespace ScopeX.Core
{
    public interface ISearchPrsnt : IPresenter<ISearchView>
    { }

    public interface ISearchItemPrsnt : IPresenter<ISearchItemView>
    { }
}


namespace ScopeX.Core.Sda
{
    public interface ISdaPrsnt : IPresenter<ISdaView>
    { }

    public interface IJitterPrsnt : IPresenter<IJitterView>
    { }

    public interface IEyePrsnt : IPresenter<IEyeView>
    { }
    
}

namespace ScopeX.Core
{
    public interface IVsaPrsnt : IPresenter<IVsaView>
    { }
    public interface IVsaVsaGenerateDigtalPrsnt : IPresenter<IVsaGenerateDigtalView>
    { }
    
}

