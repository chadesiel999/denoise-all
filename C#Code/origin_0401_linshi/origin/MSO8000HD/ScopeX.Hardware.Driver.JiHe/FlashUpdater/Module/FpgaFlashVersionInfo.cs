using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    [Obsolete]
    [Serializable]
    public record FpgaFlashVersionInfo
    {
        public Int32 MaxNum
        {
            get;
            set;
        } = 1;
        public Int32 SubNum
        {
            get;
            set;
        } = 0;
        public Int32 MinNum
        {
            get;
            set;
        } = 0;

        public String FullVersionStr
        {
            get => $"{MaxNum}.{SubNum}.{MinNum}";
        }
        public String Designer
        {
            get;
            set;
        } = "";
        public String Comment
        {
            get;
            set;
        } = "";
        public DateTime BuildTime
        {
            get;
            set;
        }
        public DateTime WriteTime
        {
            get;
            set;
        }
        public SoftwareVersionInfo SoftwareVersionInfo { get; init; }
        public FpgaFlashVersionInfo(SoftwareVersionInfo softwareVersion)
        {
            SoftwareVersionInfo = softwareVersion;
        }
        public String PrintInfoStr
        {
            get => $"[{FullVersionStr}],[{Designer}],[{Comment}],[{BuildTime}]" +
                $",[{SoftwareVersionInfo.MaxSoftVersion}],[{SoftwareVersionInfo.MinSoftVersion}]";
        }
        public static Byte[] Serialize(FpgaFlashVersionInfo source)
        {
            List<Byte> result = new List<Byte>();
            result.AddRange(Misc.StructToBytes(source.MaxNum));
            Int32 appendCount = 0 + 4 - result.Count;
            if (appendCount > 0)
                result.AddRange(new Byte[appendCount]);

            result.AddRange(Misc.StructToBytes(source.SubNum));
            appendCount = 0 + 4 + 4 - result.Count;
            if (appendCount > 0)
                result.AddRange(new Byte[appendCount]);
            
            result.AddRange(Misc.StructToBytes(source.MinNum));
            appendCount = 0 + 4 + 4 + 4 - result.Count;
            if (appendCount > 0)
                result.AddRange(new Byte[appendCount]);

            result.AddRange(Encoding.UTF8.GetBytes(source.Designer.Trim()));
            appendCount = 0 + 4 + 4 + 4 +16 - result.Count;
            if (appendCount > 0)
                result.AddRange(new Byte[appendCount]);

            result.AddRange(Encoding.UTF8.GetBytes(source.Comment.Trim()));
            appendCount = 0 + 4 + 4 + 4 + 16 + 256 - result.Count;
            if (appendCount > 0)
                result.AddRange(new Byte[appendCount]);

            result.AddRange(Misc.StructToBytes(source.BuildTime.ToBinary()));
            appendCount = 0 + 4 + 4 + 4 + 16 + 256 + sizeof(Int64)- result.Count;
            if (appendCount > 0)
                result.AddRange(new Byte[appendCount]);

            result.AddRange(Misc.StructToBytes(source.WriteTime.ToBinary()));
            appendCount = 0 + 4 + 4 + 4 + 16 + 256 + sizeof(Int64)+ sizeof(Int64) - result.Count;
            if (appendCount > 0)
                result.AddRange(new Byte[appendCount]);
            result.AddRange(Encoding.UTF8.GetBytes(source.SoftwareVersionInfo.MaxSoftVersion.Trim()));
            appendCount = 0 + 4 + 4 + 4 + 16 + 256 + sizeof(Int64)+ sizeof(Int64)+64 - result.Count;
            if (appendCount > 0)
                result.AddRange(new Byte[appendCount]);
            result.AddRange(Encoding.UTF8.GetBytes(source.SoftwareVersionInfo.MinSoftVersion.Trim()));
            appendCount = 0 + 4 + 4 + 4 + 16 + 256 + sizeof(Int64)+ sizeof(Int64)+ 64 +64 - result.Count;
            if (appendCount > 0)
                result.AddRange(new Byte[appendCount]);
            return result.ToArray();
        }
        public static FpgaFlashVersionInfo Deserialize(Byte[] content)
        {
            SoftwareVersionInfo softwareVersionInfo = new ();
            softwareVersionInfo.MaxSoftVersion = Encoding.UTF8.GetString(content, 4 + 4 + 4 + 16 + 256 + sizeof(Int64) + sizeof(Int64), 64).Trim('\0');
            softwareVersionInfo.MinSoftVersion = Encoding.UTF8.GetString(content, 4 + 4 + 4 + 16 + 256 + sizeof(Int64)+ sizeof(Int64)+64, 64).Trim('\0');
            return new FpgaFlashVersionInfo(softwareVersionInfo)
            {
                MaxNum = Misc.BytesToStruct<Int32>(content, 0, typeof(Int32)),
                SubNum = Misc.BytesToStruct<Int32>(content, 4, typeof(Int32)),
                MinNum = Misc.BytesToStruct<Int32>(content, 4 + 4, typeof(Int32)),
                Designer = Encoding.UTF8.GetString(content, 4 + 4 + 4, 16).Trim('\0'),
                Comment = Encoding.UTF8.GetString(content, 4 + 4 + 4 + 16, 256).Trim('\0'),
                BuildTime =DateTime.FromBinary(Misc.BytesToStruct<Int64>(content, 4 + 4 + 4 + 16 + 256, typeof(Int64))),
                WriteTime = DateTime.FromBinary(Misc.BytesToStruct<Int64>(content, 4 + 4 + 4 + 16 + 256 + sizeof(Int64), typeof(Int64))),
            };
        }
    }
}
