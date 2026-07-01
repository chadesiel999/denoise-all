namespace ScopeX.Hardware.Driver
{
    public static partial class Hd
    {
        /// <summary>`
        /// 频率计
        /// </summary>
        public static AbstractAcquirer_Cymometer? Cymometer
        {
            get => CurrProduct?.Acquirer_Cymometer;
        }
    }
}
