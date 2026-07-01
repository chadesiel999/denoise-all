namespace ScopeX.Hardware.Driver
{
    public static partial class Hd
    {
        /// <summary>
        /// 解码器
        /// </summary>
        public static AbstractAcquirer_Decoder? Decoder { get => CurrProduct?.Acquirer_Decoder; }
    }
}
