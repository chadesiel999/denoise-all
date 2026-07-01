using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Decode;
using ScopeX.Core.Sda;

namespace ScopeX.Core
{
    public interface IView
    {
        void UpdateView(Object? prsnt, String propertyName);
    }

    public interface IView<T> : IView
    {
        T Presenter
        {
            get;
            set;
        }
    }

    public interface IBadgeView : IView<IBadge>
    { }

    public interface IChnlView : IBadgeView
    { }

    public interface IMathView : IChnlView
    {
        MathType Mode
        {
            get;
        }
    }

    public interface IMultiDomainView : IView<IMultiDomainPrsnt>
    { }

    public interface IWfmGenView : IBadgeView
    {
        //new IWfmGenPrsnt Presenter
        //{
        //    get;
        //    set;
        //}
    }

    public interface IVoltmeterView : IBadgeView
    {
        //new IVoltmeterPrsnt Presenter
        //{
        //    get;
        //    set;
        //}
    }


    //public interface ICymometerView : IView<ICymometerPrsnt>
    //{ }

    public interface ICymometerView : IBadgeView
    {
        //new ICymometerPrsnt Presenter
        //{
        //    get;
        //    set;
        //}
    }

    public interface ITimebaseView : IView<ITimebasePrsnt>
    { }

    public interface ITriggerView : IView<ITriggerPrsnt>
    { }
   
    public interface ITriggerSerialView : ITriggerView
    {

        /// <summary>
        /// 由于使用了单例，但通道可能在运行中发生改变，因此需要重新加载通道选项
        /// </summary>
        public void ReLoadSource();
    }

    public interface IProtocolView : IView<IProtocolPrsnt>
    {
        ChannelId Id { get; set; }
        /// <summary>
        /// 由于使用了单例，但通道可能在运行中发生改变，因此需要重新加载通道选项
        /// </summary>
        void ReLoadSource();
        /// <summary>
        /// 统一重载
        /// </summary>
        void Reload();

        /// <summary>
        /// 模拟通道修改探头单位通知方法
        /// </summary>
        void UpdateThresholdUnit();
    }

    public interface IDisplayView : IView<IDisplayPrsnt>
    { }

    public interface ICursorView : IView<ICursorPrsnt>
    { }
    public interface IMarkerView : IView<IMarkerPrsnt>
    { }

    

    public interface IMarkerItemView :IView<IMarkerItemPrsnt>
    { }

    public interface IMeasView : IView<IMeasPrsnt>
    { }

    public interface IPrintView : IView<IPrintPrsnt>
    { }

    public interface IFileView : IView<IFilePrsnt>
    { }

    public interface IXYView : IView<IXYPrsnt>
    { }

 //   public interface IRadioFrequencyView : IView<IRadioFrequencyPrsnt>
	//{ }
	
    public interface IFilterView : IView<IFilterPrsnt>
    { }

    public interface ILocationAssistedView : IView<ILocationAssistedPrsnt>
    { }
    public interface ITriggerAssistedView : IView<ITriggerAssistedPrsnt>
    { }
    public interface IVisualTriggerView : IView<IVisualTriggerPrsnt>
    { }

    public interface IAreaHistogramView : IView<IAreaHistogramPrsnt>
    { }
    public interface ISettingView : IView<ISettingPrsnt>
    { }
    public interface ILANView : IView<ILANPrsnt>
    { }
    public interface ISystemCheckView : IView<ISystemCheckPrsnt>
    { }

    public interface ISdaView : IView<ISdaPrsnt>
    { }

    public interface IEyeView : IView<IEyePrsnt>
    { }

    public interface ITempView : IView<ITempPrsnt>
    { }
	public interface IArtificialIntelligenceView : IView<IArtificialIntelligencePrsnt>
    { }

    public interface IExceptionCaptureView : IView<IExceptionCapturePrsnt>
    { }
}

namespace ScopeX.Core
{
    public interface IPassFailView : IView<IPassFailPrsnt>
    { }

    public interface IPageInfoView:IView<IPageInfoPrsnt>
    { }
}

namespace ScopeX.Core.Jitter
{
    public interface IJitterView :IView<IJitterPrsnt>
    { }
}

namespace ScopeX.Core.PowerAnalysis
{
    public interface IPwrAnalysisView : /*IView<IPwrAnalysisPrsnt>*/IBadgeView
    { }

    public interface IPwrOptionView : IView<IPwrOptionPrsnt>
    {
        PowerAnalysisOpt Mode
        {
            get;
        }
    }
}

namespace ScopeX.Core
{
    public interface ISearchView : IView<ISearchPrsnt>
    { }

    public interface ISearchItemView : IView<ISearchItemPrsnt>
    {
        Int64 ID { get;  }
        ComModel.SearchType Type
        {
            get;
        }
    }
}


namespace ScopeX.Core.Sda
{
    public interface ISdaView : IView<ISdaPrsnt>
    { }

    public interface IJitterView : IView<IJitterPrsnt>
    { }

    public interface IEyeView : IView<IEyePrsnt>
    { }
}

namespace ScopeX.Core
{
    public interface IVsaView : IView<IVsaPrsnt>
    { }

    public interface IVsaGenerateDigtalView : IView<IVsaVsaGenerateDigtalPrsnt>
    { }
}
