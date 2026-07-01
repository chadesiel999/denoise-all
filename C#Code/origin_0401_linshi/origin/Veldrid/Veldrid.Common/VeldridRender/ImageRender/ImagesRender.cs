using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using Veldrid.Common.Plot;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common.VeldridRender.ImageRender
{
    internal class ImagesRender : BaseVeldridRender
    {
        private bool cursorinfochanged = false;
        private VeldridSpriteBatch spriteBatch;
        private Texture texture;
        private Shader[] shaders;
        SizeF cursorSize;
        SizeF gridSize;
        int rowcount;
        int colcount;
        public ImagesRender(IVeldridContent control,BitmapData bitmap, SizeF cursorsize,SizeF gridsize) : base(control)
        {
            if (bitmap.Width == 0 || bitmap.Height == 0 || bitmap.Data == null || bitmap.Data.Length == 0)
            {
                throw new Exception($"{nameof(bitmap)}中内容不能为空");
            }
            if (cursorsize.Width <= 0 || cursorsize.Height <= 0) throw new Exception($"图标尺寸不能为0");
            Bitmap = bitmap;
            this.cursorSize = cursorsize;
            gridSize = gridsize;
            Range = control.GraphicsManger.DefaultLineRange;
            shaders = CreateShader("ImageRender");
            spriteBatch = new VeldridSpriteBatch(GraphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription, shaders, GraphicsDevice.Aniso4xSampler);
            rowcount = (int)(Bitmap.Width / gridSize.Width);
            colcount = (int)(Bitmap.Height / gridSize.Height);
            ImageInfos.ItemAddEvent += ImageInfos_ItemAddEvent;
        }
        internal override void DrawData()
        {
            if (!Visibily || ImageInfos.All(x=>!x.Visibily)) return;
            spriteBatch.DrawBatch(CommandList);
        }
        public override void CreateResources()
        {
            base.CreateResources();
            InitTexture();
        }
        public override void DisposeResources()
        {
            spriteBatch?.Dispose();
            texture?.Dispose();
            ImageInfos?.Dispose();
            base.DisposeResources();
        }

        private void ImageInfos_ItemAddEvent(object sender, ImageInfo e)
        {
            e.PropertyChanged += (_, args) =>
            {
                cursorinfochanged = true;
            };
        }
        internal override void PreDraw()
        {
            base.PreDraw();
            if (this[nameof(Range), nameof(Margin)] || WindowSizeState || cursorinfochanged)
            {
                spriteBatch.Begin();
                ImageInfos.ToList().OrderBy(x => x.ZIndex).ToList().ForEach(x =>
                {
                    if (!x.Visibily || x.ImageIndex <0 || x.ImageIndex>=rowcount*colcount) return;
                    var point = GetImagePoint(x.Position,x.VerticalAlignment,x.HorizontalAlignment);
                    var srcpoint = new PointF(x.ImageIndex % rowcount * gridSize.Width + (gridSize.Width - cursorSize.Width) / 2, x.ImageIndex / rowcount * gridSize.Height + (gridSize.Height - cursorSize.Height) / 2);
                    spriteBatch.ViewMatrix = Matrix4x4.CreateOrthographic(MainSwapchainBuffer.Width, MainSwapchainBuffer.Height, 0.01f, -100f);
                    spriteBatch.Draw(texture, point,new RectangleF(srcpoint,cursorSize), Color.Empty, 0,Vector2.Zero,Vector2.One,1);
                });
                spriteBatch.End();
                this[ nameof(Range), nameof(Margin)] = false;
                WindowSizeState = false;
                cursorinfochanged = false;
            }
        }

        internal PointF GetImagePoint(Vector2 point,VerticalAlignment vertical,HorizontalAlignment horizontal)
        {
            float x = Rectangle.Width / (Range.MaxX - Range.MinX) * (point.X - Range.MinX) + Rectangle.Left - 0.5f * GraphicsDevice.MainSwapchain.Framebuffer.Width;
            float y = Rectangle.Height / (Range.MaxY - Range.MinY) * (Range.MaxY - point.Y) + Rectangle.Top - 0.5f * GraphicsDevice.MainSwapchain.Framebuffer.Height;
            switch (vertical)
            {
                case VerticalAlignment.Top:
                default:
                    break;
                case VerticalAlignment.Bottom:
                    y -= cursorSize.Height;
                    break;
                case VerticalAlignment.Center:
                    y -= cursorSize.Height / 2;
                    break;
            }
            switch (horizontal)
            {
                case HorizontalAlignment.Left:
                default:
                    break;
                case HorizontalAlignment.Center:
                    x -= cursorSize.Width / 2;
                    break;
                case HorizontalAlignment.Right:
                    x -= cursorSize.Width;
                    break;
            }
            return new PointF(x, y);
        }
        public VeldridCollection<ImageInfo> ImageInfos { get; } = new VeldridCollection<ImageInfo>();
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
        public BitmapData Bitmap { get; }
        
    }
    internal class ImageInfo : BaseProperty
    {
        private int imageIndex;
        private Vector2 position = Vector2.Zero;
        private TextRender.HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
        private TextRender.VerticalAlignment verticalAlignment = VerticalAlignment.Center;
        private int zIndex;
        private bool visibily = true;

        public int ImageIndex { get => imageIndex; set => Set(ref imageIndex, value); }
        public Vector2 Position { get => position; set => Set(ref position, value); }
        public HorizontalAlignment HorizontalAlignment { get => horizontalAlignment; set => Set(ref horizontalAlignment, value); }
        public VerticalAlignment VerticalAlignment { get => verticalAlignment; set => Set(ref verticalAlignment, value); }

        public Boolean Visibily { get => visibily; set =>Set(ref visibily,value); }
        public int ZIndex { get => zIndex; set =>Set(ref zIndex,value); }
    }
}
