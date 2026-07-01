namespace ScopeX.Hardware.Driver
{
    public static partial class Hd
    {
        public static AbstractAcquirer_LA? LA { get => CurrProduct?.Acquirer_LA; }
    }
}