using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
namespace ScopeX.Hardware.Driver
{
    internal partial class ProductFactory
    {
        public static OscilloscopeProduct CreateProduct(ProductType productType)
        {
            MethodInfo[] methodInfos = typeof(ProductFactory).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            foreach (MethodInfo m in methodInfos)
            {
                if (m.Name == "CreateProduct_" + productType.ToString())
                {
                    OscilloscopeProduct? result = (OscilloscopeProduct?)m.Invoke(new ProductFactory(), null);
                    if (result != null)
                    {
                        result.ProductType = productType;
                        return result;
                    }
                    throw new NotSupportedException($"Product {productType} not defined!!!");
                }
            }
            throw new NotSupportedException($"Product {productType} not defined!!!");
        }
    }
}
