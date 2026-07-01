using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Data.Base;
namespace ScopeX.Hardware.Driver
{
    internal static partial class CoefficientsTableSender_Standard
    {
        #region Interpolation
        internal static void Send_InterpolationCoefficientsToAcqBoardByDMAMode(CoefficientsTableType coefficientsTableType)
        {
            Hd.CurrProduct?.AcqBd?.SendCoefficientsByDMAMode_Interpolation(coefficientsTableType);
        }
        internal static void Send_InterpolationCoefficientsToAcqBoardByRegisterMode(CoefficientsTableType coefficientsTableType,bool bForce)
        {
            Hd.CurrProduct?.AcqBd?.SendCoefficientsByRegisterMode_Interpolation(coefficientsTableType, bForce);
        }
        #endregion Interpolation

        #region 
        internal static void Send_AdcINLCoefficientsToAcqBoardByRefisterMode(CoefficientsTableType coefficientsTableType, bool bForce)
        {
            Hd.CurrProduct?.AcqBd?.ConfigAdcINLCoefficients(coefficientsTableType,bForce);
        }
        #endregion
    }
}
