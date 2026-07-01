using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Veldrid.Common.VeldridRender.ImageRender
{
    public struct BatchItem
    {
        public BatchItem(Vector2 textureSize, RectangleF destinationRectangle, RectangleF sourceRectangle, Color color,
                         float rotation, Vector2 origin, float layerDepth, RectangleF scissor, SpriteOptions options)
        {
            var sourceSize = new Vector2(sourceRectangle.Width, sourceRectangle.Height) / textureSize;
            var pos = new Vector2(sourceRectangle.X, sourceRectangle.Y) / textureSize;

            UV = CreateFlip(options) * Matrix4x4.CreateScale(new Vector3(sourceSize, 1)) * Matrix4x4.CreateTranslation(new Vector3(pos, 0));
            Color = ToVector(color);
            Model =
                Matrix4x4.CreateScale(new Vector3(destinationRectangle.Width, destinationRectangle.Height, 0)) *
                Matrix4x4.CreateTranslation(new Vector3(-origin, 0)) *
                Matrix4x4.CreateRotationZ(rotation) *
                Matrix4x4.CreateTranslation(new Vector3(destinationRectangle.X, destinationRectangle.Y, layerDepth));
            Projection = Matrix4x4.Identity;
            Scissor = scissor;
        }

        public BatchItem(Vector2 textureSize, PointF position, RectangleF sourceRectangle, Color color, float rotation,
                         Vector2 origin, Vector2 scale, float layerDepth, RectangleF scissor, SpriteOptions options)
            : this(textureSize, new RectangleF(position.X, position.Y, sourceRectangle.Width * scale.X, sourceRectangle.Height * scale.Y),
                    sourceRectangle,
                    color,
                    rotation,
                    origin,
                    layerDepth,
                    scissor,
                    options)
        {
        }

        public Matrix4x4 UV { get; set; }
        public Vector4 Color { get; set; }
        public Matrix4x4 Model { get; set; }
        public Matrix4x4 Projection { get; set; }
        public RectangleF Scissor { get; set; }

        private static Vector4 ToVector(Color color) => new Vector4(color.R, color.G, color.B, color.A);

        private static Matrix4x4 CreateFlip(SpriteOptions options)
        {
            if (options == SpriteOptions.None)
                return Matrix4x4.Identity;

            var flipX = options.HasFlag(SpriteOptions.FlipHorizontally);
            var flipY = options.HasFlag(SpriteOptions.FlipVertically);

            return Matrix4x4.CreateScale(flipX ? -1 : 1, flipY ? -1 : 1, 1) * Matrix4x4.CreateTranslation(flipX ? 1 : 0, flipY ? 1 : 0, 0);
        }
    }
}
