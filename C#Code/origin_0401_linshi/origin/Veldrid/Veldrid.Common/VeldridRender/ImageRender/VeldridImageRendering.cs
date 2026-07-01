using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using Veldrid.Common.VeldridRender.LineRender;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common.VeldridRender.ImageRender
{
    internal class VeldridImageRendering : VeldridRender.BaseVeldridRender
    {
        private VeldridSpriteBatch spriteBatch;
        private Texture texture;
        private Shader[] shaders;
        public VeldridImageRendering(IVeldridContent device) : base(device)
        {
            Range = device.GraphicsManger.DefaultLineRange;
            shaders = CreateShader("ImageRender");
            spriteBatch = new VeldridSpriteBatch(GraphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription, shaders, GraphicsDevice.PointSampler);
        }

        public BitmapData Bitmap
        {
            get => bitmap;
            set
            {
                if (bitmap != value)
                {
                    this[nameof(Bitmap)] = true;
                    bitmap = value;
                    if (bitmap.Height != 0 && bitmap.Width != 0)
                    {
                        InitTexture();
                    }
                }
            }
        }
        private void InitTexture()
        {
            if (texture == null)
            {
                texture = ResourceFactory.CreateTexture(new TextureDescription((uint)Bitmap.Width, (uint)Bitmap.Height, 1, 1, 1, PixelFormat.B8_G8_R8_A8_UNorm_SRgb, TextureUsage.Sampled, TextureType.Texture2D));
            }
            else if (texture.Height != Bitmap.Height || texture.Width != Bitmap.Width)
            {
                texture?.Dispose();
                texture = ResourceFactory.CreateTexture(new TextureDescription((uint)Bitmap.Width, (uint)Bitmap.Height, 1, 1, 1, PixelFormat.B8_G8_R8_A8_UNorm_SRgb, TextureUsage.Sampled, TextureType.Texture2D));
            }
            GraphicsDevice.UpdateTexture(texture, Bitmap.Data, 0, 0, 0, texture.Width, texture.Height, 1, 0, 0);
        }

        public override void CreateResources()
        {
            base.CreateResources();
            if (Bitmap.Height != 0 && Bitmap.Width != 0)
            {
                InitTexture();
            }

        }
        internal override void PreDraw()
        {
            if (texture == null)
            {
                return;
            }
            base.PreDraw();
            if (this[nameof(Bitmap), nameof(Range), nameof(Local), nameof(Margin)] || WindowSizeState)
            {
                spriteBatch.Begin();
                ImagePoint = GetImagePoint(Local);
                spriteBatch.ViewMatrix = Matrix4x4.CreateOrthographic(MainSwapchainBuffer.Width, MainSwapchainBuffer.Height, 0.01f, -100f);
                spriteBatch.Draw(texture, new RectangleF(ImagePoint, new SizeF(texture.Width, texture.Height)), Color.Empty, 2);
                spriteBatch.End();
                this[nameof(Bitmap), nameof(Range), nameof(Local), nameof(Margin)] = false;
                WindowSizeState = false;
            }
        }
        public override Vector2 VirtualSize => new Vector2(Bitmap.Width / Rectangle.Width * (Range.MaxX - Range.MinX), bitmap.Height / Rectangle.Height * (Range.MaxY - Range.MinY));
        private PointF local = new PointF(-1f, -1f);
        private BitmapData bitmap = new BitmapData();

        public override PointF Local
        {
            get => local;
            set => Set(ref local, value);
        }
        public PointF ImagePoint { get; private set; }
        private PointF GetImagePoint(PointF point)
        {
            float x = Rectangle.Width / (Range.MaxX - Range.MinX) * (point.X - Range.MinX) + Rectangle.Left - 0.5f * GraphicsDevice.MainSwapchain.Framebuffer.Width;
            float y = Rectangle.Height / (Range.MaxY - Range.MinY) * (Range.MaxY - point.Y) + Rectangle.Top - 0.5f * GraphicsDevice.MainSwapchain.Framebuffer.Height;
            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                default:
                    break;
                case VerticalAlignment.Bottom:
                    y -= bitmap.Height;
                    break;
                case VerticalAlignment.Center:
                    y -= bitmap.Height / 2;
                    break;
            }
            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                default:
                    break;
                case HorizontalAlignment.Center:
                    x -= bitmap.Width / 2;
                    break;
                case HorizontalAlignment.Right:
                    x -= bitmap.Width;
                    break;
            }
            return new PointF(x, y);
        }


        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Top;
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
        internal override void DrawData()
        {
            if (texture == null) return;
            spriteBatch.DrawBatch(CommandList);
        }

        public override void DisposeResources()
        {
            base.DisposeResources();
            texture?.Dispose();
            spriteBatch?.Dispose();
            Bitmap?.Dispose();
        }
    }
}
