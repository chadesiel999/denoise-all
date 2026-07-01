using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    [Obsolete]
    [Serializable]
    public record ProductBaseInfo
    {
        public String SN { get; init; } = "";
        public String Model { get; init; } = "";
        public String Manufacturers { get; init; } = "";
        public UInt32 FunCode { get; init; } 
        public static Byte[] Serialize(ProductBaseInfo source)
        {
            List<Byte> result = new List<Byte>();
            result.AddRange(Encoding.UTF8.GetBytes(source.SN.Trim()));
            Int32 appendCount = 0+64- result.Count;
            if (appendCount > 0)
                result.AddRange(new Byte[appendCount]);
            result.AddRange(Encoding.UTF8.GetBytes(source.Model.Trim()));
            appendCount = 0 + 64 +64 - result.Count;
            if (appendCount > 0)
                result.AddRange(new Byte[appendCount]);
            result.AddRange(Encoding.UTF8.GetBytes(source.Manufacturers.Trim()));
            appendCount = 0 + 64 + 64 +64 - result.Count;
            if (appendCount > 0)
                result.AddRange(new Byte[appendCount]);
            result.AddRange(Misc.StructToBytes(source.FunCode));
            return result.ToArray();
        }
        public static ProductBaseInfo Deserialize(Byte[] content)
        {
            return new ProductBaseInfo()
            {
                SN = Encoding.UTF8.GetString(content, 0, 64).Trim('\0'),
                Model = Encoding.UTF8.GetString(content, 0 + 64, 64).Trim('\0'),
                Manufacturers = Encoding.UTF8.GetString(content, 0 + 64 + 64, 64).Trim('\0'),
                FunCode = Misc.BytesToStruct<UInt32>(content, 0 + 64 + 64 + 64, typeof(UInt32)),
            };
        }
    }
}
