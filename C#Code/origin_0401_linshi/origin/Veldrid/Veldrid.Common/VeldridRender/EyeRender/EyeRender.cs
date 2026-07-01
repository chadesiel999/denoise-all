using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid.Common.VeldridRender.ImageRender;

namespace Veldrid.Common.VeldridRender.EyeRender
{
    internal class EyeRender : BaseVeldridRender
    {
        private UInt32 _FixWidth;
        private UInt32 _FixHeight;
        private Boolean _SizeChanged = true;
        [AllowNull]
        private Texture _Texture;
        private bool _PointSampler;
        EyeSpriteBatch spriteBatch;


        public EyeRender(IVeldridContent control,uint width = 4096, uint height = 512, bool pointSampler = true) : base(control)
        {
            Width = width;
            Height = height;
            _FixWidth = width;
            _FixHeight = height;
            _PointSampler = pointSampler;
        }
        public override void CreateResources()
        {
            base.CreateResources();
            _Texture = ResourceFactory.CreateTexture(TextureDescription.Texture2D(Width, Height, 1, 1, PixelFormat.R32_Float, TextureUsage.Sampled | TextureUsage.RenderTarget));
            spriteBatch = new EyeSpriteBatch(GraphicsDevice, MainSwapchainBuffer.OutputDescription, CreateShader("EyeRender"), _PointSampler ? GraphicsDevice.PointSampler: GraphicsDevice.LinearSampler, blendState: BlendStateDescription.SingleAlphaBlend);
        }
        internal override void PreDraw()
        {
            if (spriteBatch == null || IsDisposed) return;
            base.PreDraw();
            if (WindowSizeState || _SizeChanged)
            {
                spriteBatch.Begin();
                spriteBatch.ViewMatrix = Matrix4x4.CreateOrthographic(MainSwapchainBuffer.Width, MainSwapchainBuffer.Height, 0.01f, -100f);
                spriteBatch.Draw(_Texture,
                    new RectangleF(GraphicsDevice.MainSwapchain.Framebuffer.Width / -2, GraphicsDevice.MainSwapchain.Framebuffer.Height / -2, GraphicsDevice.MainSwapchain.Framebuffer.Width, GraphicsDevice.MainSwapchain.Framebuffer.Height),
                    new RectangleF(0, 0, FixWidth, FixHeight),
                    Color.Empty,
                    0,
                    Vector2.Zero,
                    SpriteOptions.None, 1);
                spriteBatch.End();
                WindowSizeState = false;
                _SizeChanged = false;
            }
        }
        internal override void DrawData()
        {
            if (!Visibily || IsDisposed || spriteBatch == null || _Texture.IsDisposed) return;
            CommandList.SetFramebuffer(MainSwapchainBuffer);
            spriteBatch.DrawBatch(CommandList);
        }
        public Single Max { get => spriteBatch.MaxValue; set => spriteBatch.MaxValue = value; }
        public Single Min { get => spriteBatch.MinValue; set => spriteBatch.MinValue = value; }
        public void SetData(IntPtr dataptr, UInt32 sizeInBytes, UInt32 width = 4096, UInt32 height = 512)
        {
            if (sizeInBytes == 0 || dataptr == IntPtr.Zero || _Texture.IsDisposed)
            {
                Visibily = false;
                return;
            }
            Visibily = true;
            lock (_Locker)
            {
                FixHeight = height;
                FixWidth = width;
                GraphicsDevice.UpdateTexture(_Texture, dataptr, sizeInBytes, 0, 0, 0, width, height, 1, 0, 0);
            }
        }
        public unsafe void SetData(float[,] data)
        {
            if (data == null || data.Length == 0 || _Texture.IsDisposed)
            {
                Visibily = false;
                return;
            }
            Visibily = true;
            lock (_Locker)
            {
                SetData((IntPtr)Unsafe.AsPointer(ref data[0, 0]), (UInt32)(data.Length * Unsafe.SizeOf<Single>()), (UInt32)data.GetLength(0), (UInt32)data.GetLength(1));
            }
        }
        public override void DisposeResources()
        {
            lock (_Locker)
            {
                base.DisposeResources();
                _Texture?.Dispose();
                spriteBatch?.Dispose();
            }
        }

        public uint Width { get; }
        public uint Height { get; }
        public uint FixWidth
        {
            get => _FixWidth;
            set
            {
                if (_FixWidth != value)
                {
                    _FixWidth = value;
                    _SizeChanged = true;
                }
            }
        }
        public uint FixHeight
        {
            get => _FixHeight;
            set
            {
                if (_FixHeight != value)
                {
                    _FixHeight = value;
                    _SizeChanged = true;
                }
            }
        }
    }
}
