using FontStashSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Typography.OpenFont;

namespace Veldrid.FontStashSharp.Typography
{
    internal class OpenFontLoader : IFontLoader
    {
        public IFontSource Load(Typeface data)
        {
            return new OpenFontSource(data);
        }
    }
}
