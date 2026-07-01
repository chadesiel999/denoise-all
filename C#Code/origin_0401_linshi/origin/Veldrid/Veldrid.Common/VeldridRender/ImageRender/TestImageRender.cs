using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Text;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common.VeldridRender.ImageRender
{
    internal class TestImageRender : VeldridRender.BaseVeldridRender
    {
        private VeldridSpriteBatch spriteBatch;
        private Texture texture;
        private Shader[] shaders;
        public TestImageRender(IVeldridContent device) : base(device)
        {
            Range = device.GraphicsManger.DefaultLineRange;
            shaders = CreateShader("ImageRender");
            spriteBatch = new VeldridSpriteBatch(GraphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription, shaders, GraphicsDevice.Aniso4xSampler);
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
                    InitTexture();
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
            if (this[nameof(Bitmap), nameof(Range), nameof(Local), nameof(Margin), nameof(Row), nameof(Column)] || WindowSizeState)
            {
                spriteBatch.Begin();
                for (int i = 0; i < 100; i++)
                {
                    PointF pointF = new System.Drawing.PointF(Range.XLenght / 20 * (i % 20) + Range.MinX, Range.MinY + Range.YLenght / 10 * (i / 20));
                    ImagePoint = GetImagePoint(pointF);
                    spriteBatch.ViewMatrix = Matrix4x4.CreateOrthographic(MainSwapchainBuffer.Width, MainSwapchainBuffer.Height, 0.01f, -100f);
                    spriteBatch.Draw(texture, ImagePoint, new RectangleF(3, (i%40) * 30 + 3, 24, 24), Color.Empty, 0,Vector2.Zero,1,1);
                }
                spriteBatch.End();
                this[nameof(Bitmap), nameof(Range), nameof(Local), nameof(Margin), nameof(Row), nameof(Column)] = false;
                WindowSizeState = false;
            }
        }
        public int Row { get => row; set =>Set(ref row,value); }
        public int Column { get => column; set =>Set(ref column,value); }
        public override Vector2 VirtualSize => new Vector2(Bitmap.Width / Rectangle.Width * (Range.MaxX - Range.MinX), bitmap.Height / Rectangle.Height * (Range.MaxY - Range.MinY));
        private PointF local;
        private BitmapData bitmap = new BitmapData();
        private int row;
        private int column;

        public override PointF Local
        {
            get => local;
            set => Set(ref local, value);
        }
        public PointF ImagePoint { get; private set; }
        private PointF GetImagePoint(PointF point)
        {
            float x = Rectangle.Width / (Range.MaxX - Range.MinX) * (point.X - Range.MinX) + Rectangle.Left - 0.5f * GraphicsDevice.MainSwapchain.Framebuffer.Width;
            float y = Rectangle.Height / (Range.MaxY - Range.MinY) * (point.Y - Range.MinY) + Rectangle.Top - 0.5f * GraphicsDevice.MainSwapchain.Framebuffer.Height;
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
