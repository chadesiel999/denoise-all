using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    public static class BitHelper
    {
        /// <summary>
        /// 获取数据有效字节数。eg: 0xFF 1位 ；  0x1000  2位；  0x0F3A 2位
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static Int32 GetByteCount(Int64 data) => (Int32)Math.Ceiling(data.ToString("X16").TrimStart('0').Length / 2d);

        /// <summary>
        /// 获取数据的有效比特位数。
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static Int32 GetEffectiveDigitCount(Int64 val) => Convert.ToString(val, 2).Length;


        /// <summary>
        /// 获取指定字节数掩码,0有效
        /// </summary>
        /// <param name="n">掩码字节数</param>
        /// <returns></returns>
        public static UInt64 GetMask0(Int32 n)
        {
            // 创建掩码，将第 n 位全部置为1
            UInt64 mask = (1UL << (n * 8)) - 1UL;
            return ~mask;
        }

        /// <summary>
        /// 获取指定字节数掩码,1有效
        /// </summary>
        /// <param name="n">掩码字节数</param>
        /// <returns></returns>
        public static UInt64 GetMask1(Int32 n)
        {
            // 创建掩码，将第 n 位全部置为1
            UInt64 mask = (1UL << (n * 8)) - 1UL;
            return mask;
        }

        /// <summary>
        /// 获取数据掩码，0有效
        /// </summary>
        /// <param name="data">要获取掩码的数据</param>
        /// <returns></returns>
        public static UInt64 GetBitsMask(Int64 data)
        {
            var bitnumbers = GetByteCount(data);
            return GetMask0(bitnumbers);
        }

        /// <summary>
        /// 获取按4位(16进制)为一个有效掩码的方法，0为该位有效
        /// </summary>
        /// <author>
        /// zxl
        /// </author>
        /// <param name="data">要获取掩码的数据</param>
        /// <returns>按4位(16进制)为一个有效掩码</returns>
        public static UInt64 GetBitsMaskByHex(Int64 data)
        {
            UInt64 mask = 0X_FF_FF_FF_FF_FF_FF_FF_FF;
            int bit_count = GetEffectiveDigitCount(data);
            int mask_bits_count = bit_count % 4 == 0 ? (bit_count / 4) : (bit_count / 4) + 1;
            mask = mask << (mask_bits_count == 1 ? (mask_bits_count + 1) * 4 : (mask_bits_count) * 4);
            return mask;
        }

        /// <summary>
        /// 更改控件字节数时，根据字节数保留数据位数
        /// </summary>
        /// <author>
        /// zxl
        /// </author>
        /// <param name="data">根据数据字节来展示的数据</param>
        /// <returns>固定数据</returns>
        public static UInt64 GetRealDataFromByteCount(UInt64 data,Int32 byte_count)
        {
            UInt64 mask = 0X_FF_FF_FF_FF_FF_FF_FF_FF;
            mask >>= (8 - byte_count) * 8;
            return (data & mask);
        }
    }
}
