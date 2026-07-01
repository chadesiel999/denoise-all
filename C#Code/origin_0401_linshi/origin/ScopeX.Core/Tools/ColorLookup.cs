using System;
using System.Collections.Generic;
using System.Drawing;
using ScopeX.ComModel;

namespace ScopeX.Core.Tools
{
    public class ColorInfo
    {
        public String Id
        {
            get;
        }

        public Int32 Index
        {
            get;
            set;
        } = 0;

        public Color Default
        {
            get;
        }

        public Color Custom
        {
            get;
            set;
        }

        public ColorInfo(String id, Color defaultColor)
        {
            Id = id;
            Default = defaultColor;
            Custom = defaultColor;
        }
    }

    internal class ColorLookup
    {
        private Dictionary<String, ColorInfo> Table
        {
            get;
        } = new();

        public Color this[String index]
        {
            get
            {
                if (Table.TryGetValue(index, out var color))
                {
                    if (color.Index == 0)
                    {
                        return color.Default;
                    }
                    else
                    {
                        return color.Custom;
                    }
                }
                return Color.White;
            }
            set
            {
                if (Table.TryGetValue(index, out var color))
                {
                    color.Index = 1;
                    color.Custom = value;
                }
            }
        }

        public IReadOnlyCollection<ColorInfo> Colors => Table.Values;

        public Int32 Count => Table.Count;

        public void Reset()
        {
            foreach (var (_, color) in Table)
            {
                color.Index = 0;
            }
        }

        public Boolean SetAlpha(String id, Int32 alpha)//调节颜色表中所有颜色的透明度
        {
            if (Table.ContainsKey(id))
            {
                Table[id].Index = 1;
                Table[id].Custom = Color.FromArgb(alpha, Table[id].Custom);
                return true;
            }
            return false;
        }

        public void SetAllAlpha(String[] idChain, Int32 alpha)//调节颜色表中所有颜色的透明度，ID容器待确定，此处用数组代替
        {
            foreach (var id in idChain)
            {
                SetAlpha(id, alpha);
            }
        }

        //!!!Notice: The order of these default colors corresponds to enum ChannelId
        private static readonly Dictionary<ChannelId, Color> _DefaultColor = new()
        {
            //C1~C4
            [ChannelId.C1] = Color.FromArgb(237, 163, 3),
            [ChannelId.C2] = Color.FromArgb(0, 171, 209),
            [ChannelId.C3] = Color.FromArgb(3, 171, 3),
            [ChannelId.C4] = Color.FromArgb(204, 0, 153),

            //C5~C8
            [ChannelId.C5] = Color.Blue,
            [ChannelId.C6] = Color.OrangeRed,
            [ChannelId.C7] = Color.Olive,
            [ChannelId.C8] = Color.BlueViolet,

            //M1~M4
            [ChannelId.M1] = Color.FromArgb(150, 14, 75),
            [ChannelId.M2] = Color.FromArgb(0, 70, 140),
            [ChannelId.M3] = Color.FromArgb(106, 47, 47),
            [ChannelId.M4] = Color.FromArgb(83, 37, 116),

            //M5~M8
            [ChannelId.M5] = Color.FromArgb(126, 27, 72),
            [ChannelId.M6] = Color.FromArgb(7, 96, 136),
            [ChannelId.M7] = Color.FromArgb(106, 47, 47),
            [ChannelId.M8] = Color.FromArgb(83, 37, 116),

            //R1~R4
            [ChannelId.R1] = Color.FromArgb(121, 112, 32),
            [ChannelId.R2] = Color.FromArgb(134, 67, 19),
            [ChannelId.R3] = Color.FromArgb(20, 133, 129),
            [ChannelId.R4] = Color.FromArgb(43, 110, 43),

            //R5~R8
            [ChannelId.R5] = Color.DarkGray,
            [ChannelId.R6] = Color.Chocolate,
            [ChannelId.R7] = Color.Wheat,
            [ChannelId.R8] = Color.DarkGreen,

            //B1~B2
            [ChannelId.B1] = Color.FromArgb(88, 132, 21),
            [ChannelId.B2] = Color.FromArgb(138, 61, 2),

            //D0~D7
            [ChannelId.D0] = Color.FromArgb(7, 96, 136),
            [ChannelId.D1] = Color.DarkSlateGray,
            [ChannelId.D2] = Color.Peru,
            [ChannelId.D3] = Color.Navy,

            [ChannelId.D4] = Color.FromArgb(0, 115, 153),
            [ChannelId.D5] = Color.DarkSlateGray,
            [ChannelId.D6] = Color.Peru,
            [ChannelId.D7] = Color.Navy,

            //D8~D15
            [ChannelId.D8] = Color.FromArgb(0, 115, 153),
            [ChannelId.D9] = Color.DarkSlateGray,
            [ChannelId.D10] = Color.Peru,
            [ChannelId.D11] = Color.Navy,

            [ChannelId.D12] = Color.FromArgb(0, 115, 153),
            [ChannelId.D13] = Color.DarkSlateGray,
            [ChannelId.D14] = Color.Peru,
            [ChannelId.D15] = Color.Navy,

            //P1~P8: Only hold color place, not use.
            [ChannelId.P1] = Color.Pink,
            [ChannelId.P2] = Color.Blue,
            [ChannelId.P3] = Color.DarkRed,
            [ChannelId.P4] = Color.Indigo,

            [ChannelId.P5] = Color.Pink,
            [ChannelId.P6] = Color.Blue,
            [ChannelId.P7] = Color.DarkRed,
            [ChannelId.P8] = Color.Indigo,

            //AWG1~AWG4
            [ChannelId.AWG1] = Color.FromArgb(77, 77, 77),
            [ChannelId.AWG2] = Color.FromArgb(77, 77, 77),
            [ChannelId.AWG3] = Color.FromArgb(77, 77, 77),
            [ChannelId.AWG4] = Color.FromArgb(77, 77, 77),

            //DVM
            [ChannelId.DVM] = Color.Gray,

            //Ext
            [ChannelId.Ext] = Color.OrangeRed,
            //Ext5
            [ChannelId.Ext5] = Color.OrangeRed,
            //AC
            [ChannelId.AC] = Color.OrangeRed,

            //[ChannelId.AC] = Color.OrangeRed,
            //RF1~RF4~RF
            [ChannelId.RF1] = Color.SteelBlue,
            [ChannelId.RF2] = Color.Yellow,
            [ChannelId.RF3] = Color.Lime,
            [ChannelId.RF4] = Color.Magenta,
            [ChannelId.RF] = Color.Yellow,

            //AVT1~AVT4~AVT
            [ChannelId.AVT1] = Color.Cyan,
            [ChannelId.AVT2] = Color.Yellow,
            [ChannelId.AVT3] = Color.Lime,
            [ChannelId.AVT4] = Color.Magenta,
            [ChannelId.AVT] = Color.Yellow,

            //PVT1~PVT4~PVT
            [ChannelId.PVT1] = Color.Cyan,
            [ChannelId.PVT2] = Color.Yellow,
            [ChannelId.PVT3] = Color.Lime,
            [ChannelId.PVT4] = Color.Magenta,
            [ChannelId.PVT] = Color.Yellow,

            //PVF1~PVF4~PVF
            [ChannelId.PVF1] = Color.Cyan,
            [ChannelId.PVF2] = Color.Yellow,
            [ChannelId.PVF3] = Color.Lime,
            [ChannelId.PVF4] = Color.Magenta,
            [ChannelId.PVF] = Color.Yellow,

            //PGD1~PGD4~PGD
            [ChannelId.PGD1] = Color.Cyan,
            [ChannelId.PGD2] = Color.Yellow,
            [ChannelId.PGD3] = Color.Lime,
            [ChannelId.PGD4] = Color.Magenta,
            [ChannelId.PGD] = Color.Yellow,

            //FVT1~FVT4~FVT
            [ChannelId.FVT1] = Color.Cyan,
            [ChannelId.FVT2] = Color.Yellow,
            [ChannelId.FVT3] = Color.Lime,
            [ChannelId.FVT4] = Color.Magenta,
            [ChannelId.FVT] = Color.Yellow,
        };

        public static Color MakeRandColor()
        {
            Random rrand = new();
            Random grand = new();
            Random brand = new();

            Int32 red;
            Int32 green;
            Int32 blue;

            //  为了在黑色背景上显示，尽量生成浅色
            Int32 deltagb;
            Int32 deltarb;

            do
            {
                red = rrand.Next(255);
                green = grand.Next(255);
                blue = brand.Next(255);
                if (red < 120 && green < 120 && blue < 120)
                {
                    continue;
                }
                deltagb = Math.Abs(green - blue);
                deltarb = Math.Abs(red - blue);
                if (deltagb > 70 || deltarb > 70)
                {
                    break;
                }
            } while (true);

            return Color.FromArgb(red, green, blue);
        }

        private void Init(ChannelId[] idChain)//ID容器待确定，此处用数组代替
        {
            for (Int32 i = 0; i < idChain.Length; i++)//根据ID串中所有的ID进行赋初值
            {
                //if (i < _DefaultColor.Length)
                //{
                //    Table.Add(idChain[i], new ColorInfo(idChain[i], _DefaultColor[i]));//从预设颜色表里取值
                //}
                //else
                //{
                //    Table.Add(idChain[i], new ColorInfo(idChain[i], MakeRandColor()));//随机生成颜色
                //}
                var name = idChain[i].ToString();
                if (_DefaultColor.ContainsKey(idChain[i]))
                {
                    Table.Add(name, new ColorInfo(name, _DefaultColor[idChain[i]]));
                }
                else
                {
                    Table.Add(name, new ColorInfo(name, MakeRandColor()));//随机生成颜色
                }
            }
        }

        public ColorLookup(ChannelId[] idChain)
        {
            Init(idChain);
        }

        public static readonly ColorLookup Default = new(Enum.GetValues<ChannelId>());
    }
}
