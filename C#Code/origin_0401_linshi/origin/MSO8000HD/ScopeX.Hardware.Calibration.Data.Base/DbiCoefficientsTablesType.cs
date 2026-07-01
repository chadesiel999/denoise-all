using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
namespace ScopeX.Hardware.Calibration.Data.Base
{
    public enum DbiCoefficientsTablesType
    {

        /// <summary>
        /// acqboard [/*0.625G/1.25G/2.5G=2*/,/*低通=2*/,/*Data*/]
        /// </summary>
        InterpolationCoefficients_2fold,
        /// <summary>
        /// acqboard [/*低通、带通=2*/,/*Data*/]
        /// </summary>
        Level1_InterpolationCoefficients,
        /// <summary>
        /// acqboard [,/*channel*/,/*subBand*/,/*data*/]
        /// </summary>
        Level1_LocalOscillatorCoefficients,
        /// <summary>
        /// acqboard [/*20G/16G*/,/*channel*/,/*subBand*/,/*data*/]
        /// </summary>
        Level1_AntiImageCoefficients,
        /// <summary>
        /// acqboard [/*低通、带通=2*/,/*Data*/]
        /// </summary>
        Level2_InterpolationCoefficients,
        /// <summary>
        /// acqboard [,/*channel*/,/*subBand*/,/*data*/]
        /// </summary>
        Level2_LocalOscillatorCoefficients,
        /// <summary>
        /// acqboard [/*20G/16G*/,/*channel*/,/*subBand*/,/*data*/]
        /// </summary>
        Level2_AntiImageCoefficients,
        /// <summary>
        /// acqboard [/*20G/16G*/,/*channel*/,/*subBand*/,/*data*/]
        /// </summary>
        Sub_AmpCoefficientFile,
        /// <summary>
        /// acqboard [/*20G/16G=2*/,/*低通、带通=2*/,/*Data*/]
        /// </summary>
        InterpolationCoefficients,

        /// <summary>
        /// acqboard [/*20G/16G*/,/*channel*/,/*subBand*/,/*data*/]
        /// </summary>
        LocalOscillatorCoefficients,

        /// <summary>
        /// acqboard [/*20G/16G*/,/*channel*/,/*subBand*/,/*data*/]
        /// </summary>
        AntiImageCoefficients,

        /// <summary>
        /// acqboard [/*20G/16G*/,/*channel*/,/*subBand*/,/*data*/]
        /// </summary>
        FractionaryDelayCoefficients,

        /// <summary>
        /// [/*20G/16G*/,/*channel*/,/*subBand*/,/*data*/]
        /// </summary>
        OverlapPhaseFreqDelayCoefficients,

        /// <summary>
        /// acqboard [/*20G/16G*/,/*channel*/,/*subBand*/,/*data*/]
        /// </summary>
        TiAdc,

        /// <summary>
        /// proc board [/*20G/16G=2*/,/*通道=4*/,/*data*/]
        /// </summary>
        AmpFreqCoefficients,

        /// <summary>
        /// proc board [/*20G/16G=2*/,/*通道=4*/,/*data*/]
        /// </summary>
        PhaseFreqCoefficients,

        /// <summary>
        /// proc board [/*20G/16G=2*/,/*data*/]
        /// </summary>
        MultiRadioInterpolationCoefficients,
        ProcTiAdc
    }
    public enum BandMode
    {
        Full = 0,
        NotCare=0,
        Other=1
    }
    public enum FilterbandMode
    {
        LowPass = 0,
        NotCare=0,
        Other = 1,
    }
    public class DBI_CoefTableSendItem
    {
        public BandMode BandMode
        {
            get;
            init;
        }
        public ChannelId ChannelID
        {
            get;
            init;
        }
        public int SubbandIndex
        {
            get;
            init;
        }
        public FilterbandMode FilterbandMode
        {
            get;
            init;
        }
        public int FPGAIndex
        {
            get;
            set;
        }
        
        public AnaChnlScaleIndex ChnlScaleIndex
        {
            get;
            set;
        }
        public string DataFileName
        {
            get;
            init;
        } = "";
        public bool Equal(DBI_CoefTableSendItem other)
        {
            if (BandMode != other.BandMode)
                return false;
            if (ChannelID != other.ChannelID)
                return false;
            if (SubbandIndex != other.SubbandIndex)
                return false;
            if (FilterbandMode != other.FilterbandMode)
                return false;
            return true;
        }
    }
}
