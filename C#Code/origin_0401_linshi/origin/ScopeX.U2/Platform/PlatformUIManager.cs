using ScopeX.ComModel;

namespace ScopeX.U2
{
    internal class PlatformUIManager
    {
        public static readonly PlatformUIManager Default = new();

        public IPlatformUI Platform { get; set; }

        public PlatformUIManager()
        {
            switch (Constants.PRODUCT)
            {
                case ProductType.JiHe_MSO7000X:
                    {
                        Platform = new UiSpecialMSO7000X();
                    }
                    break;
                case ProductType.JiHe_MSO8000X:
                    {
                        Platform = new UiSpecialMSO8000X();
                    }
                    break;
                case ProductType.JiHe_UPO7000L:
                    {
                        Platform = new UiSpecialUPO7000L();
                    }
                    break;
                default:
                    {
                        Platform = new UiSpecialMSO7000X();
                    }
                    break;
            }
        }
    }
}
