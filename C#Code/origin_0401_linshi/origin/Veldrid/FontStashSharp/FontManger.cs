using FontStashSharp.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Typography.OpenFont;
using Vortice.DXGI;
using Vulkan.Xcb;

namespace FontStashSharp
{
    public  class FontManger
    {
        private ConcurrentDictionary<String,Typeface> _FontCache = new ConcurrentDictionary<String,Typeface>();
        private ConcurrentDictionary<String, String> _FontFileInfos = new ConcurrentDictionary<String, String>();
        private FontManger()
        {
            //InitSystemFontInfo();
        }
        private void LoadFont(string path)
        {
            using (var stream = System.IO.File.Open(path, FileMode.Open))
            {
                Typography.OpenFont.OpenFontReader reader = new OpenFontReader();
                Typeface typeface = reader.Read(stream);
                typeface.Filename = path;
                if (!_FontCache.ContainsKey($"{typeface.Name},{typeface.FontSubFamily}"))
                {
                    _FontCache.TryAdd($"{typeface.FamilyName},{typeface.FontSubFamily}", typeface);
                }
            }
        }

        private Object Locker = new Object();

        private Typeface LoadFontTypeface(String path)
        {
            lock (Locker)
            {
                using (var stream = System.IO.File.Open(path, FileMode.Open))
                {
                    Typography.OpenFont.OpenFontReader reader = new OpenFontReader();
                    Typeface typeface = reader.Read(stream);
                    typeface.Filename = path;
                    return typeface;
                } 
            }
        }
        private void InitSystemFontInfo()
        {
            AddFontDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Fonts));
        }
        public void AddFontDirectory(String directory)
        {
            System.IO.Directory.GetFiles(directory, "*.*")
                .Where(x => x.EndsWith(".ttf") || x.EndsWith(".ttc"))
                .Where(x=>!_FontFileInfos.Values.Contains(x))
                .ToList().
                ForEach(x =>
                {
                    var typeface = LoadFontTypeface(x);
                    String key = $"{typeface.FamilyName},{typeface.FontSubFamily}";
                    if(!_FontFileInfos.TryGetValue(key,out _))
                    {
                        _FontFileInfos.TryAdd(key, typeface.Filename);
                    }
                    typeface = null;
                });

        }
        public FontSystem LoadFromSystemDirectory(String fontName, String fontStyle)
        {
            String key = $"{fontName},{fontStyle}";
            if (_FontCache.TryGetValue(key,out var val))
            {
                FontSystem fontSystem = new FontSystem();
                fontSystem.AddFont(val);
                return fontSystem;
            }
            else if(_FontFileInfos.TryGetValue(key,out var filename))
            {
                var typeface = LoadFontTypeface(filename);
                FontSystem fontSystem = new FontSystem();
                fontSystem.AddFont(typeface);
                _FontCache.TryAdd(key, typeface);
                return fontSystem;
            }
            else
            {
                throw new Exception("Cannot Find Font:" + fontName);
            }
        }

        static FontManger()
        {

        }
        public static FontManger Instance { get; } = new FontManger();
    }
}
