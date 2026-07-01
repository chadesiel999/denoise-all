using FontStashSharp.Interfaces;
using MatterHackers.Agg;
using MatterHackers.Agg.Font;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.VertexSource;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using Typography.OpenFont;

namespace Veldrid.FontStashSharp.Typography
{
    internal class OpenFontSource : IFontSource
    {
        private TypeFace _faceType;
        private TypeFacePrinter _facePrinter;
        public OpenFontSource(Typeface typeface)
        {
            _faceType = new TypeFace();
            _faceType.LoadFormTypeFace(typeface);
            _facePrinter = new TypeFacePrinter("",new StyledTypeFace(_faceType,12),new MatterHackers.VectorMath.Vector2(0,1), Justification.Left, Baseline.Text);
        }
        public string FontName => _faceType.Typeface.Name;

        public string FontStyle => _faceType.Typeface.FontSubFamily;

        public void Dispose()
        {
            _facePrinter = null;
        }

        public int? GetGlyphId(int codepoint)
        {
           return _faceType.Typeface.GetGlyphIndex(codepoint);
        }

        public int GetGlyphKernAdvance(int previousGlyphId, int glyphId, float fontSize)
        {
            return 0;
            try
            {
                _facePrinter.TypeFaceStyle.FontSize = fontSize;
                return (int)_facePrinter.TypeFaceStyle.GetAdvanceForCharacter((char)previousGlyphId, (char)glyphId);
            }catch
            {
                return 0;
            }
        }

        public void GetGlyphMetrics(int glyphId, float fontSize, out double advance, out double x0, out double y0, out double x1, out double y1)
        {
            _facePrinter.Text = new string((char)glyphId, 1);
            _facePrinter.TypeFaceStyle.FontSize = fontSize;
            advance = _facePrinter.TypeFaceStyle.GetAdvanceForCharacter((char)glyphId);
            
            x0 = _facePrinter.LocalBounds.Left;
            x1 = _facePrinter.LocalBounds.Right;
            y1 = _facePrinter.TypeFaceStyle.BoundingBoxInPixels.Height;
            y0 = 0;
        }

        public void GetMetricsForSize(float fontSize, out double ascent, out double descent, out double lineHeight)
        {
            _facePrinter.TypeFaceStyle.FontSize = fontSize;
            ascent = (_facePrinter.TypeFaceStyle.AscentInPixels);
            descent = _facePrinter.TypeFaceStyle.DescentInPixels;
            lineHeight = _facePrinter.TypeFaceStyle.BoundingBoxInPixels.Height;
        }

        public void RasterizeGlyphBitmap(int glyphId, float fontSize, byte[] buffer, int startIndex, int outWidth, int outHeight, int outStride)
        {
            _facePrinter.Text = new string((char)glyphId, 1);
            _facePrinter.TypeFaceStyle.FontSize = fontSize;
            _facePrinter.DrawFromHintedCache = false;
            var size = _facePrinter.GetSize();
            _facePrinter.Origin = new MatterHackers.VectorMath.Vector2(0, outHeight - size.Y);
            using (ImageBuffer imageBuffer = new ImageBuffer(outWidth, outHeight,8,new blenderGrayFromRed(1)))
            {
                var g = imageBuffer.NewGraphics2D();
                g.DrawString(_facePrinter, color: Color.Red);
                imageBuffer.FlipY();
                var tempbuffer = imageBuffer.GetBuffer();
                Unsafe.CopyBlock(ref buffer[startIndex], ref tempbuffer[0], (uint)(outWidth * outHeight));

            }
        }
    }
}
