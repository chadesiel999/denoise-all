using ScopeX.ComModel;

namespace ScopeX.Core
{
    internal class PlatformManager
    {
        public static readonly PlatformManager Default = new();

        public IPlatform Platform { get; set; }

        public PlatformManager()
        {
            switch (Constants.PRODUCT)
            {
                case ProductType.JiHe_MSO7000X:
                    {
                        Platform = new MSO7000X();
                    }
                    break;
                case ProductType.JiHe_MSO8000X:
                    {
                        Platform = new MSO8000X();
                    }
                    break;
                case ProductType.JiHe_UPO7000L:
                    {
                        Platform = new UPO7000L();
                    }
                    break;
                case ProductType.JiHe_MSO7000HD:
                    {
                        Platform = new MSO7000HD();
                    }
                    break;
                default:
                    {
                        Platform = new MSO7000X();
                    }
                    break;
            }
        }
    }
}
