namespace ScopeX.Hardware.Driver
{
    public static partial class Hd
    {
        public static AbstractAcquirer_DPX? Dpx { get => CurrProduct?.Acquirer_DPX; }
    }
}