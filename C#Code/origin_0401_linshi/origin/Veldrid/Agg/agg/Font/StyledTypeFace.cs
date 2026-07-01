using MatterHackers.Agg.Image;
using MatterHackers.Agg.Transform;
using MatterHackers.Agg.VertexSource;

//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
//
// C# port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007-2011
//
// Permission to copy, use, modify, sell and distribute this software
// is granted provided this copyright notice appears in all copies.
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
//
// Class StyledTypeFace.cs
//
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace MatterHackers.Agg.Font
{
	public class GlyphWithUnderline : VertexSourceLegacySupport
	{
		private IVertexSource underline;
		private IVertexSource glyph;

		public GlyphWithUnderline(IVertexSource glyph, int advanceForCharacter, int Underline_position, int Underline_thickness)
		{
			underline = new RoundedRect(new RectangleDouble(0, Underline_position, advanceForCharacter, Underline_position + Underline_thickness), 0);
			this.glyph = glyph;
		}

		public override IEnumerable<VertexData> Vertices()
		{
			// return all the data for the glyph
			foreach (VertexData vertexData in glyph.Vertices())
			{
				if (ShapePath.is_stop(vertexData.command))
				{
					break;
				}
				yield return vertexData;
			}

			// then the underline
			foreach (VertexData vertexData in underline.Vertices())
			{
				yield return vertexData;
			}
		}
	}

	public class StyledTypeFaceImageCache
	{
		private static StyledTypeFaceImageCache instance;

        // Keys: TypeFace, Color, FontSize, Character
        private Dictionary<TypeFace, Dictionary<Color, Dictionary<double, Dictionary<char, ImageBuffer>>>> typeFaceImageCache = new Dictionary<TypeFace, Dictionary<Color, Dictionary<double, Dictionary<char, ImageBuffer>>>>();

		// private so you can't use it by accident (it is a singleton)
		private StyledTypeFaceImageCache()
		{
		}

		public static Dictionary<char, ImageBuffer> GetCorrectCache(TypeFace typeFace, Color color, double emSizeInPoints)
		{
			lock (typeFace)
			{
				// TODO: check if the cache is getting too big and if so prune it (or just delete it and start over).

				Dictionary<Color, Dictionary<double, Dictionary<char, ImageBuffer>>> foundTypeFaceColor;
				if (!Instance.typeFaceImageCache.TryGetValue(typeFace, out foundTypeFaceColor))
				{
					// add in the type face
					foundTypeFaceColor = new Dictionary<Color, Dictionary<double, Dictionary<char, ImageBuffer>>>();
					Instance.typeFaceImageCache.Add(typeFace, foundTypeFaceColor);
				}

				Dictionary<double, Dictionary<char, ImageBuffer>> foundTypeFaceSizes;
				if (!foundTypeFaceColor.TryGetValue(color, out foundTypeFaceSizes))
				{
					// add in the type face
					foundTypeFaceSizes = new Dictionary<double, Dictionary<char, ImageBuffer>>();
					foundTypeFaceColor.Add(color, foundTypeFaceSizes);
				}

				Dictionary<char, ImageBuffer> foundTypeFaceSize;
				if (!foundTypeFaceSizes.TryGetValue(emSizeInPoints, out foundTypeFaceSize))
				{
					// add in the point size
					foundTypeFaceSize = new Dictionary<char, ImageBuffer>();
					foundTypeFaceSizes.Add(emSizeInPoints, foundTypeFaceSize);
				}

				return foundTypeFaceSize;
			}
		}

		private static StyledTypeFaceImageCache Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new StyledTypeFaceImageCache();
				}

				return instance;
			}
		}
	}

    public class StyledTypeFace
    {
        public TypeFace TypeFace { get; private set; }

        private const int PointsPerInch = 72;
        private const int PixelsPerInch = 96;

        private bool flattenCurves = true;
        private double fontSize;

        public StyledTypeFace(TypeFace typeFace, double emSizeInPoints, bool underline = false, bool flattenCurves = true)
        {
            this.TypeFace = typeFace;
            fontSize = emSizeInPoints;
            DoUnderline = underline;
            FlattenCurves = flattenCurves;
        }
        public double CurrentEmScaling => EmSizeInPixels / TypeFace.UnitsPerEm;
        public double FontSize 
        {
            get => fontSize;
            set
            {
                if (value != fontSize)
                {
                    fontSize = value;
                }
            }
        }

        public bool DoUnderline { get; set; }

        /// <summary>
        /// <para>If true the font will have it's curves flattened to the current point size when retrieved.</para>
        /// <para>You may want to disable this so you can flatten the curve after other transforms have been applied,</para>
        /// <para>such as skewing or scaling.  Rotation and Translation will not alter how a curve is flattened.</para>
        /// </summary>
        public bool FlattenCurves
        {
            get => flattenCurves;
            set => flattenCurves = value;
        }

        /// <summary>
        /// Sets the Em size for the font in pixels.
        /// </summary>
        public double EmSizeInPixels => fontSize / PointsPerInch * PixelsPerInch;

        /// <summary>
        /// Sets the Em size for the font assuming there are 72 points per inch and there are 96 pixels per inch.
        /// </summary>
        public double EmSizeInPoints => EmSizeInPixels / PixelsPerInch * PointsPerInch;

        public double AscentInPixels => TypeFace.Ascent * CurrentEmScaling;

        public double DescentInPixels => TypeFace.Descent * CurrentEmScaling;

        public double XHeightInPixels => TypeFace.X_height * CurrentEmScaling;

        public double CapHeightInPixels => TypeFace.Cap_height * CurrentEmScaling;

        public RectangleDouble BoundingBoxInPixels
        {
            get
            {
                RectangleDouble pixelBounds = new RectangleDouble(TypeFace.BoundingBox);
                pixelBounds *= CurrentEmScaling;
                return pixelBounds;
            }
        }

        public double UnderlineThicknessInPixels => TypeFace.Underline_thickness * CurrentEmScaling;

        public double UnderlinePositionInPixels => TypeFace.Underline_position * CurrentEmScaling;

        public ImageBuffer GetImageForCharacter(char character, double xFraction, double yFraction, Color color)
        {
            if (xFraction > 1 || xFraction < 0 || yFraction > 1 || yFraction < 0)
            {
                throw new ArgumentException("The x and y fractions must both be between 0 and 1.");
            }


            var characterImageCache = StyledTypeFaceImageCache.GetCorrectCache(this.TypeFace, color, EmSizeInPixels);
            characterImageCache.TryGetValue(character, out ImageBuffer imageForCharacter);
            if (imageForCharacter != null)
            {
                return imageForCharacter;
            }

            IVertexSource glyphForCharacter = GetGlyphForCharacter(character, 1);
            if (glyphForCharacter == null)
            {
                return null;
            }

            var bounds = glyphForCharacter.GetBounds();

            var charImage = new ImageBuffer(
                Math.Max((int)(bounds.Right + .5), 1) + 1,
                Math.Max((int)Math.Ceiling(EmSizeInPixels + (-DescentInPixels) + .5), 1) + 1,
                32,
                new BlenderPreMultBGRA());

            var graphics = charImage.NewGraphics2D();
            graphics.Render(glyphForCharacter, xFraction, yFraction + (-DescentInPixels) + 1, color);
            characterImageCache[character] = charImage;

            return charImage;
        }

        public IVertexSource GetGlyphForCharacter(char character, double resolutionScale = 1)
        {
            // scale it to the correct size.
            IVertexSource sourceGlyph = TypeFace.GetGlyphForCharacter(character);
            if (sourceGlyph != null)
            {
                if (DoUnderline)
                {
                    sourceGlyph = new GlyphWithUnderline(sourceGlyph, TypeFace.GetAdvanceForCharacter(character), TypeFace.Underline_position, TypeFace.Underline_thickness);
                }

                var glyphTransform = Affine.NewIdentity();
                glyphTransform *= Affine.NewScaling(CurrentEmScaling);
                IVertexSource characterGlyph = new VertexSourceApplyTransform(sourceGlyph, glyphTransform);

                if (FlattenCurves)
                {
                    characterGlyph = new FlattenCurves(characterGlyph)
                    {
                        ResolutionScale = resolutionScale
                    };
                }

                return characterGlyph;
            }

            return null;
        }

        public double GetAdvanceForCharacter(string line, int characterIndex)
        {
            if (characterIndex < line.Length - 1)
            {
                // pass the next char so the typeFaceStyle can do kerning if it needs to.
                return GetAdvanceForCharacter(line[characterIndex], line[characterIndex + 1]);
            }
            else
            {
                return GetAdvanceForCharacter(line[characterIndex]);
            }
        }

        public double GetAdvanceForCharacter(char character, char nextCharacterToKernWith)
        {
            return TypeFace.GetAdvanceForCharacter(character, nextCharacterToKernWith) * CurrentEmScaling;
        }

        public double GetAdvanceForCharacter(char character)
        {
            return TypeFace.GetAdvanceForCharacter(character) * CurrentEmScaling;
        }
    }
}