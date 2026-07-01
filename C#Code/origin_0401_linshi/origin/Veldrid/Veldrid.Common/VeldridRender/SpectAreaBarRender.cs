using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid.Common.Tools;

namespace Veldrid.Common.VeldridRender
{
    internal unsafe class SpectAreaBarRender : BaseVeldridRender
    {
        private Boolean _ClearCache = true;
        private Vector2* _Dataptr;
        private Framebuffer _Framebuffer;
        private Texture _FrameTextureBuffer;
        private float _Brightness = 0;
        private ImageRender.VeldridSpriteBatch _StashRenderer;
        #region PipLine
        [AllowNull]
        Pipeline linepipline;
        Pipeline tripipline;
        #endregion

        #region vertex
        [AllowNull]
        DeviceBuffer vertexBuffer;
        VertexLayoutDescription vertexLayout;
        #endregion

        #region Uniform
        [AllowNull]
        Shader[] shaders;
        /// <summary>
        /// rgbafloat color
        /// vec4 linerange
        /// float VerticalOffset
        /// float HorizontalOffset
        /// float Brightness
        /// float spare
        /// </summary>
        [AllowNull]
        DeviceBuffer lineInfoBuffer;

        [AllowNull]
        DeviceBuffer proviewBuffer;
        [AllowNull]
        ResourceLayout sharedLayout;
        [AllowNull]
        ResourceSet sharedSet;
        #endregion 
        private BlendStateDescription _CacheBlend = new BlendStateDescription()
        {
            AlphaToCoverageEnabled = false,
            BlendFactor = RgbaFloat.Clear,
            AttachmentStates = new BlendAttachmentDescription[]
            {
                new BlendAttachmentDescription()
                {
                    BlendEnabled = true,
                    SourceColorFactor = BlendFactor.SourceAlpha,
                    DestinationColorFactor = BlendFactor.One,
                    ColorFunction = BlendFunction.Maximum,
                    SourceAlphaFactor = BlendFactor.SourceAlpha,
                    DestinationAlphaFactor = BlendFactor.One,
                    AlphaFunction = BlendFunction.Add,
                }
            }
        };
        public SpectAreaBarRender(IVeldridContent control, int xbarCount = 50, int yBarCount = 50) : base(control)
        {
            if (xbarCount <= 0 || yBarCount <= 0) throw new ArgumentOutOfRangeException();
            XBarCount = xbarCount;
            YBarCount = yBarCount;
            _Dataptr = (Vector2*)Marshal.AllocHGlobal(((xbarCount + yBarCount) * 4 + 5) * Unsafe.SizeOf<Vector2>()).ToPointer();
        }
        public override void CreateResources()
        {
            base.CreateResources();
            vertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Vector2>() * (((XBarCount + YBarCount) * 4 + 5))), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            vertexLayout = new VertexLayoutDescription(new VertexElementDescription("in_Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));
            shaders = CreateShader("DataRender");
            ResourceLayoutElementDescription[] resourceLayoutElementDescriptions =
            {
                new ResourceLayoutElementDescription("ProView", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("LineInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            };
            proviewBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>() * 2, BufferUsage.Dynamic | BufferUsage.UniformBuffer));
            lineInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<RgbaFloat>() + Unsafe.SizeOf<Vector4>() * 2), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            ResourceLayoutDescription resourceLayoutDescription = new ResourceLayoutDescription(resourceLayoutElementDescriptions);
            sharedLayout = ResourceFactory.CreateResourceLayout(resourceLayoutDescription);
            BindableResource[] bindableResources = new BindableResource[] { proviewBuffer, lineInfoBuffer };
            ResourceSetDescription resourceSetDescription = new ResourceSetDescription(sharedLayout, bindableResources);
            sharedSet = ResourceFactory.CreateResourceSet(resourceSetDescription);
            this[nameof(Range), nameof(Margin)] = true;
            linepipline = CreatePipLine(PrimitiveTopology.LineStrip, sharedLayout, vertexLayout, shaders, frontFace: FrontFace.Clockwise);
            tripipline = CreatePipLine(PrimitiveTopology.TriangleStrip, sharedLayout, vertexLayout, shaders, frontFace: FrontFace.Clockwise);
            CreateBuffer();
            _StashRenderer = new ImageRender.VeldridSpriteBatch(GraphicsDevice, MainSwapchainBuffer.OutputDescription, CreateShader("ImageRender"), blendState: _CacheBlend);
        }
        private void CreateBuffer()
        {
            var texturedesc = TextureDescription.Texture2D(MainSwapchainBuffer.Width, MainSwapchainBuffer.Height, 1, 1, MainSwapchainBuffer.ColorTargets[0].Target.Format, TextureUsage.RenderTarget | TextureUsage.Sampled);
            if (_Framebuffer == null || _Framebuffer.Width != MainSwapchainBuffer.Width || _Framebuffer.Height != MainSwapchainBuffer.Height)
            {
                _FrameTextureBuffer?.Dispose();
                _FrameTextureBuffer = ResourceFactory.CreateTexture(texturedesc);
                _Framebuffer?.Dispose();
                _Framebuffer = ResourceFactory.CreateFramebuffer(new FramebufferDescription(null, _FrameTextureBuffer));
            }
        }
        internal override void PreDraw()
        {
            if (WindowSizeState)
            {
                CreateBuffer();
            }
            if (this[nameof(Margin), nameof(Range)] || WindowSizeState)
            {
                CommandList.UpdateBuffer(proviewBuffer, 0, Camera.OrthographicMatrix);
                CommandList.UpdateBuffer(proviewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>(), Camera.GetLineMatrix(Margin, Range));
                CommandList.UpdateBuffer(lineInfoBuffer, (uint)Unsafe.SizeOf<RgbaFloat>(), (Vector4)Range);
                this[nameof(Range), nameof(Margin)] = false;
                if (WindowSizeState) WindowSizeState = false;
            }
            if (this[nameof(Brightness)])
            {
                UpdateBrightness();
                this[nameof(Brightness)] = false;
            }
            if (this[nameof(VerticalOffset)])
            {
                UpdateVerticalOffset();
                this[nameof(VerticalOffset)] = false;
            }
            if (this[nameof(HorizontalOffset)])
            {
                UpdateHorizontalOffset();
                this[nameof(HorizontalOffset)] = false;
            }
        }

        internal unsafe override void DrawData()
        {
            CommandList.SetFramebuffer(MainSwapchainBuffer);
            CommandList.UpdateBuffer(vertexBuffer, 0, (IntPtr)_Dataptr, vertexBuffer.SizeInBytes);
            CommandList.SetVertexBuffer(0, vertexBuffer);
            if (RectColor != Color.Transparent)
            {
                CommandList.SetPipeline(linepipline);
                CommandList.SetGraphicsResourceSet(0, sharedSet);
                UpdateColor(RectColor.ColorConverToRGBA());
                CommandList.Draw(5);
            }
            if (BarColor != Color.Transparent)
            {
                if (UseCache)
                {
                    CommandList.SetFramebuffer(_Framebuffer);
                    if (_ClearCache)
                    {
                        CommandList.ClearColorTarget(0, RgbaFloat.Clear);
                        _ClearCache = false;
                    }
                    CommandList.SetPipeline(tripipline);
                    CommandList.SetGraphicsResourceSet(0, sharedSet);
                    UpdateColor(BarColor.ColorConverToRGBA());
                    for (uint i = 0; i < XBarCount + YBarCount; i++)
                    {
                        CommandList.Draw(4, 1, i * 4 + 5, 0);
                    }
                    _StashRenderer.Begin();
                    _StashRenderer.ViewMatrix = Matrix4x4.CreateOrthographic(MainSwapchainBuffer.Width, MainSwapchainBuffer.Height, 0.01f, -100f);
                    _StashRenderer.Draw(_FrameTextureBuffer, new RectangleF(MainSwapchainBuffer.Width / -2, MainSwapchainBuffer.Height / -2, MainSwapchainBuffer.Width, MainSwapchainBuffer.Height), Color.Empty, 1);
                    _StashRenderer.End();

                    CommandList.SetFramebuffer(MainSwapchainBuffer);
                    _StashRenderer.DrawBatch(CommandList);
                }
                else
                {
                    CommandList.SetPipeline(tripipline);
                    CommandList.SetGraphicsResourceSet(0, sharedSet);
                    UpdateColor(BarColor.ColorConverToRGBA());
                    for (uint i = 0; i < XBarCount + YBarCount; i++)
                    {
                        CommandList.Draw(4, 1, i * 4 + 5, 0);
                    }
                }
            }
        }
        private float horizontalOffset;
        private float verticalOffset;
        private bool useCache;

        public float HorizontalOffset
        {
            get => horizontalOffset;
            set
            {
                Set(ref horizontalOffset, value);
            }
        }
        public float VerticalOffset
        {
            get => verticalOffset;
            set
            {
                Set(ref verticalOffset, value);

            }
        }
        private void UpdateHorizontalOffset()
        {
            CommandList.UpdateBuffer(lineInfoBuffer, (uint)(Unsafe.SizeOf<RgbaFloat>() + Unsafe.SizeOf<Vector4>() + Unsafe.SizeOf<float>()), HorizontalOffset);
        }
        private void UpdateVerticalOffset()
        {
            CommandList.UpdateBuffer(lineInfoBuffer, (uint)(Unsafe.SizeOf<RgbaFloat>() + Unsafe.SizeOf<Vector4>()), VerticalOffset);
        }
        private void UpdateColor(RgbaFloat color)
        {
            CommandList.UpdateBuffer(lineInfoBuffer, 0, color);
        }
        private void UpdateBrightness()
        {
            CommandList.UpdateBuffer(lineInfoBuffer, (uint)(Unsafe.SizeOf<Vector4>() * 2 + Unsafe.SizeOf<float>() * 2), Brightness);
        }
        public float Brightness
        {
            get => _Brightness;
            set
            {
                float temp = Math.Clamp(value, 0.0f, 100.0f);
                Set(ref _Brightness, temp);
            }
        }
        public unsafe override void DisposeResources()
        {
            base.DisposeResources();
            vertexBuffer?.Dispose();
            Marshal.FreeHGlobal((IntPtr)_Dataptr);
            _Framebuffer?.Dispose();
            _FrameTextureBuffer?.Dispose();
            _StashRenderer?.Dispose();
            sharedLayout?.Dispose();
            sharedSet?.Dispose();
            proviewBuffer?.Dispose();
            lineInfoBuffer?.Dispose();
            linepipline?.Dispose();
            tripipline?.Dispose();
        }
        public int XBarCount { get; }
        public int YBarCount { get; }
        public unsafe void SetRectData(Vector2[] rectData)
        {
            fixed (Vector2* ptr = &rectData[0])
            {
                Buffer.MemoryCopy(ptr, _Dataptr, Math.Min(5, rectData.Length) * Unsafe.SizeOf<Vector2>(), Math.Min(5, rectData.Length) * Unsafe.SizeOf<Vector2>());
            }
        }
        public unsafe void SetBarData(Vector2[] barData)
        {
            fixed (Vector2* ptr = &barData[0])
            {
                Buffer.MemoryCopy(ptr, _Dataptr + 5, Math.Min((XBarCount + YBarCount) * 4, barData.Length) * Unsafe.SizeOf<Vector2>(), Math.Min((XBarCount + YBarCount) * 4, barData.Length) * Unsafe.SizeOf<Vector2>());
            }
        }

        public Color RectColor { get; set; }

        public Color BarColor { get; set; }
        public Boolean UseCache 
        { 
            get => useCache;
            set
            {
                if (useCache != value)
                {
                    useCache = value;
                    _ClearCache = true;
                }
            }
        }
        public void ClearCache()
        {
            _ClearCache = true;
        }
    }
}
