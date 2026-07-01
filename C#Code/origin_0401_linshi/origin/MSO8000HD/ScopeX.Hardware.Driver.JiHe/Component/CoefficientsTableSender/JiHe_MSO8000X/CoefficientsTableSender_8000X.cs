using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Data.Base;

namespace ScopeX.Hardware.Driver
{
    internal static partial class CoefficientsTableSender_8000X
    {
        #region Interpolation
        internal static void Send_InterpolationCoefficientsToAcqBoardByDMAMode(CoefficientsTableType coefficientsTableType)
        {
            Hd.CurrProduct?.AcqBd?.SendCoefficientsByDMAMode_Interpolation(coefficientsTableType);
        }
        internal static void Send_InterpolationCoefficientsToAcqBoardByRegisterMode(CoefficientsTableType coefficientsTableType, bool bForce)
        {
            Hd.CurrProduct?.AcqBd?.SendCoefficientsByRegisterMode_Interpolation(coefficientsTableType, bForce);
        }
        #endregion Interpolation

        #region IFC，为TiAdc、AFC和PFC融合生成的
        internal static void Send_IFCCoefficientsToAcqBoardByRegisterMode(bool bForce)
        {
     //       Hd.CurrProduct?.AcqBd?.SendCoefficientsByRegisterMode_ADCTI(bForce);//?????
            Hd.CurrProduct?.AcqBd?.SendCoefficientsByRegisterMode_IFC(bForce);
        }
        #endregion IFC
        #region AFC
        /// <summary>
        /// 2022.07.01改为每个通道每个幅度档都有不同的系数的方式
        /// </summary>
        /// <param name="coefficientsTableType"></param>
        /// <param name="bForce"></param>
        internal static void SendCoefficients_Afc(CoefficientsTableType coefficientsTableType, bool bForce)
        {
            //Hd.CurrProduct?.Acquirer_AnalogChannel?.SendCoefficients_Afc(coefficientsTableType, bForce);
        }
        #endregion AFC
    }
}
