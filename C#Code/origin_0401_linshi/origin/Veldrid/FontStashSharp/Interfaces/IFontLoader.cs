using System;
using Typography.OpenFont;

namespace FontStashSharp.Interfaces
{
	public interface IFontSource: IDisposable
	{
		/// <summary>
		/// Returns font metrics for the specified font size
		/// </summary>
		/// <param name="fontSize"></param>
		/// <param name="ascent"></param>
		/// <param name="descent"></param>
		/// <param name="lineHeight"></param>
		void GetMetricsForSize(float fontSize, out double ascent, out double descent, out double lineHeight);

		/// <summary>
		/// Returns Id of a glyph corresponding to a codepoint
		/// Null if the codepoint can't be rasterized
		/// </summary>
		/// <param name="codepoint"></param>
		/// <returns></returns>
		int? GetGlyphId(int codepoint);

		/// <summary>
		/// Returns glyph metrics
		/// </summary>
		/// <param name="glyphId"></param>
		/// <param name="fontSize"></param>
		/// <param name="advance"></param>
		/// <param name="x0"></param>
		/// <param name="y0"></param>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		void GetGlyphMetrics(int glyphId, float fontSize, out double advance, out double x0, out double y0, out double x1, out double y1);

		/// <summary>
		/// Renders a glyph 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="fontSize"></param>
		/// <param name="startIndex"></param>
		/// <param name="outWidth"></param>
		/// <param name="outHeight"></param>
		/// <param name="outStride"></param>
		void RasterizeGlyphBitmap(int glyphId, float fontSize, byte[] buffer, int startIndex, int outWidth, int outHeight, int outStride);

		/// <summary>
		/// Returns kerning
		/// </summary>
		/// <param name="previousGlyphId"></param>
		/// <param name="glyphId"></param>
		/// <param name="fontSize"></param>
		/// <returns></returns>
		int GetGlyphKernAdvance(int previousGlyphId, int glyphId, float fontSize);
		public String FontName { get; }
		public String FontStyle { get; }
	}

	/// <summary>
	/// Font Rasterization Service
	/// </summary>
	public interface IFontLoader
	{
		IFontSource Load(Typeface typeface);
	}
}
