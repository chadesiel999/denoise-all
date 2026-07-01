using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid.Common.Tools;
using Veldrid.Common.VeldridRender.ImageRender;
using Vulkan;
using Vulkan.Xcb;

namespace Veldrid.Common.VeldridRender
{
    internal unsafe class BarRender : BaseVeldridRender
    {
        public UInt32 TextureWidth => 4000;
        public UInt32 TextureHeight => 1000;
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
        private Texture _Texture;
        private Framebuffer _FrameBuffer;
        private VeldridSpriteBatch spriteBatch;
        #region Uniform
        [AllowNull]
        Shader[] shaders;
        [AllowNull]
        DeviceBuffer lineInfoBuffer;
        DeviceBuffer _ColorInfoBuffer;

        [AllowNull]
        DeviceBuffer proviewBuffer;
        [AllowNull]
        ResourceLayout sharedLayout;
        [AllowNull]
        ResourceSet sharedSet;
        #endregion 

        public Matrix4x4 Orth { get; private set; }
        public Matrix4x4 View { get; private set; }
        public BarRender(IVeldridContent control, int maxDataCount = 10000, int plotCount = 1) : base(control)
        {
            if (plotCount <= 0 || maxDataCount <= 0) throw new ArgumentOutOfRangeException();
            MaxDataCount = maxDataCount;
            PlotCount = plotCount;
            BarRenders = Enumerable.Range(0, plotCount).Select(x => new BarRenderConfig(x, maxDataCount)
            {
                SetBuffer = (args) =>
                {
                    lock (_Locker)
                    {
                        if (vertexBuffer == null || vertexBuffer.IsDisposed) return;
                        GraphicsDevice.UpdateBuffer(vertexBuffer, (UInt32)(x * maxDataCount * Unsafe.SizeOf<Single>()), args);
                    }
                }
            }).ToList().AsReadOnly();
            Orth = Camera.GetOrthographicMatrix(TextureWidth, TextureHeight);
        }


        public float Brightness
        {
            get => brightness;
            set
            {
                if (value != brightness)
                {
                    lock (_Locker)
                    {
                        if (_ColorInfoBuffer.IsDisposed) return;
                        brightness = value;
                        GraphicsDevice.UpdateBuffer(_ColorInfoBuffer, 4 * 3, value / 100f);
                    }
                }
            }
        }
        public int PlotCount { get; }
        public IReadOnlyList<BarRenderConfig> BarRenders { get; }
        private float brightness;

        public override void CreateResources()
        {
            base.CreateResources();
            _Texture?.Dispose();
            _FrameBuffer?.Dispose();
            _Texture = ResourceFactory.CreateTexture(TextureDescription.Texture2D(TextureWidth, TextureHeight, 1, 1, MainSwapchainBuffer.ColorTargets[0].Target.Format, TextureUsage.RenderTarget | TextureUsage.Sampled));
            _FrameBuffer = ResourceFactory.CreateFramebuffer(new FramebufferDescription(null, _Texture));
            vertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Single>() * MaxDataCount * PlotCount), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            vertexLayout = new VertexLayoutDescription(new VertexElementDescription("in_Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float1));
            shaders = CreateShader("BarRender");
            ResourceLayoutElementDescription[] resourceLayoutElementDescriptions =
            {
                new ResourceLayoutElementDescription("ProView", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("LineInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("ColorInfo", ResourceKind.UniformBuffer, ShaderStages.Fragment),
            };
            proviewBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>() * 2, BufferUsage.Dynamic | BufferUsage.UniformBuffer));
            lineInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription(LineInfoStruct.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _ColorInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((UInt32)Unsafe.SizeOf<Vector4>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            ResourceLayoutDescription resourceLayoutDescription = new ResourceLayoutDescription(resourceLayoutElementDescriptions);
            sharedLayout = ResourceFactory.CreateResourceLayout(resourceLayoutDescription);
            BindableResource[] bindableResources = new BindableResource[] { proviewBuffer, lineInfoBuffer, _ColorInfoBuffer };
            ResourceSetDescription resourceSetDescription = new ResourceSetDescription(sharedLayout, bindableResources);
            sharedSet = ResourceFactory.CreateResourceSet(resourceSetDescription);
            this[nameof(Range), nameof(Margin)] = true;
            if (GraphicsDevice.BackendType == GraphicsBackend.Direct3D11)
            {
                Shader tri = GetOtherShader("BarRenderTriangle.Geometry.hlsl");
                tripipline = CreatePipLine(PrimitiveTopology.PointList, sharedLayout, vertexLayout, new Shader[] { shaders[0], shaders[1], tri }, frontFace: FrontFace.Clockwise);
                Shader line = GetOtherShader("BarRenderLines.Geometry.hlsl");
                linepipline = CreatePipLine(PrimitiveTopology.PointList, sharedLayout, vertexLayout, new Shader[] { shaders[0], shaders[1], line });
            }
            else
            {
                linepipline = CreatePipLine(PrimitiveTopology.LineStrip, sharedLayout, vertexLayout, shaders, frontFace: FrontFace.Clockwise);
                tripipline = CreatePipLine(PrimitiveTopology.TriangleStrip, sharedLayout, vertexLayout, shaders, frontFace: FrontFace.Clockwise);
            }
            spriteBatch = new VeldridSpriteBatch(GraphicsDevice, MainSwapchainBuffer.OutputDescription, CreateShader("ImageRender"), GraphicsDevice.PointSampler);
        }
        public BlendStateDescription BlendState { get; set; } = BlendStateDescription.SingleAlphaBlend;
        public int MaxDataCount { get; private set; }
        internal unsafe override void DrawData()
        {
            if (!Visibily || !BarRenders.All(x => x.Visibily) || vertexBuffer.IsDisposed || _ColorInfoBuffer.IsDisposed || lineInfoBuffer.IsDisposed) return;
            CommandList.SetFramebuffer(_FrameBuffer);
            CommandList.ClearColorTarget(0, RgbaFloat.Clear);
            CommandList.SetVertexBuffer(0, vertexBuffer);
            if (BarRenders.Count(x => x.Visibily && x.DataLength > 0 && x.Color.A > 0) > 0)
            {
                CommandList.SetPipeline(tripipline);
                CommandList.SetGraphicsResourceSet(0, sharedSet);

                foreach (var bar in BarRenders.Where(x => x.Visibily && x.DataLength > 0 && x.Color.A > 0).OrderBy(x => x.ZIndex))
                {
                    var color = bar.Color;
                    CommandList.UpdateBuffer(_ColorInfoBuffer, 0, (IntPtr)(&color), 4 * 3);
                    CommandList.UpdateBuffer(lineInfoBuffer, 0, bar.LineInfo);
                    CommandList.Draw((UInt32)bar.DataLength);
                }
            }
            if (BarRenders.Count(x => x.Visibily && x.DataLength > 0 && x.BorderColor.A > 0) > 0)
            {
                CommandList.SetPipeline(linepipline);
                CommandList.SetGraphicsResourceSet(0, sharedSet);
                foreach (var bar in BarRenders.Where(x => x.Visibily && x.BorderColor.A > 0 && x.DataLength > 0).OrderBy(x => x.Visibily))
                {
                    var color = bar.BorderColor;
                    CommandList.UpdateBuffer(_ColorInfoBuffer, 0, (IntPtr)(&color), 4 * 3);
                    CommandList.UpdateBuffer(lineInfoBuffer, 0, bar.LineInfo);

                    CommandList.Draw((UInt32)bar.DataLength);
                }
            }
            CommandList.SetFramebuffer(MainSwapchainBuffer);
            spriteBatch.DrawBatch(CommandList);
        }

        public override void DisposeResources()
        {
            lock (_Locker)
            {
                foreach (var val in BarRenders)
                {
                    val.SetBuffer = null;
                }
                vertexBuffer?.Dispose();
                lineInfoBuffer?.Dispose();
                sharedLayout?.Dispose();
                sharedSet?.Dispose();
                linepipline?.Dispose();
                tripipline?.Dispose();
                _ColorInfoBuffer?.Dispose();
                base.DisposeResources();
            }
        }

        public Padding FixedMargin => new Padding(Margin.Left / MainSwapchainBuffer.Width * TextureWidth, Margin.Top / MainSwapchainBuffer.Height * TextureHeight, Margin.Right / MainSwapchainBuffer.Width * TextureWidth, Margin.Bottom / MainSwapchainBuffer.Height * TextureHeight);
        internal override void PreDraw()
        {
            if (proviewBuffer.IsDisposed || lineInfoBuffer.IsDisposed) return;
            base.PreDraw();
            if (this[nameof(Range), nameof(Margin)] || WindowSizeState)
            {
                CommandList.UpdateBuffer(proviewBuffer, 0, Orth);
                View = Camera.GetLineMatrix(_Texture.Width, _Texture.Height, FixedMargin, Range, GraphicsDevice.IsClipSpaceYInverted);
                CommandList.UpdateBuffer(proviewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>(), View);
                CommandList.UpdateBuffer(lineInfoBuffer, (uint)Unsafe.SizeOf<RgbaFloat>(), (Vector4)Range);
                spriteBatch.Begin();
                spriteBatch.ViewMatrix = Matrix4x4.CreateOrthographic(MainSwapchainBuffer.Width, MainSwapchainBuffer.Height, 0.01f, -100f);
                spriteBatch.Draw(_Texture, new RectangleF(MainSwapchainBuffer.Width / -2, MainSwapchainBuffer.Height / -2, MainSwapchainBuffer.Width, MainSwapchainBuffer.Height),
                    new RectangleF(0, 0, _Texture.Width, _Texture.Height), Color.Empty, 0);
                spriteBatch.End();
                this[nameof(Range), nameof(Margin)] = false;
                if (WindowSizeState) WindowSizeState = false;
            }
            if (this[nameof(Brightness)])
            {
                CommandList.UpdateBuffer(lineInfoBuffer, (uint)(Unsafe.SizeOf<RgbaFloat>() * 2 + Unsafe.SizeOf<float>() * 2), Brightness);
                this[nameof(Brightness)] = false;
            }
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct LineInfoStruct
        {
            public static UInt32 SizeInBytes = 4 * 8;
            public float Start;
            public float BaseValue;
            public float AbsBaseValue;
            public float Width;
            public float AbsWidth;
            public float YValueOffset;
            public float XValueOffset;
            public float Orientation;
        }
        public unsafe class BarRenderConfig
        {
            private int maxdatalen;
            private float verticalOffset;
            private float horizontalOffset;
            private LineInfoStruct _LineInfo;
            private float xValueOffset;
            private float yValueOffset;

            internal LineInfoStruct LineInfo => _LineInfo;

            internal BarRenderConfig(int index, int maxDataLen)
            {
                maxdatalen = maxDataLen;
                Index = index;
            }
            [AllowNull]
            internal Action<Single[]> SetBuffer { get; set; }
            public int Index { get; }
            public float Start { get => _LineInfo.Start; set => _LineInfo.Start = value; }
            public float BaseValue { get => _LineInfo.BaseValue; set => _LineInfo.BaseValue = value; }
            public float AbsBaseValue { get => _LineInfo.AbsBaseValue; set => _LineInfo.AbsBaseValue = value; }
            public float Width { get => _LineInfo.Width; set => _LineInfo.Width = value; }
            public float AbsWidth { get => _LineInfo.AbsWidth; set => _LineInfo.AbsWidth = value; }
            public Plot.Orientation Orientation { get => (Plot.Orientation)_LineInfo.Orientation; set => _LineInfo.Orientation = (Int32)value; }
            public float VerticalOffset
            {
                get => verticalOffset;
                set
                {
                    if (value != verticalOffset)
                    {
                        verticalOffset = value;
                        _LineInfo.YValueOffset = YValueOffset + value;
                    }
                }
            }
            public float HorizontalOffset
            {
                get => horizontalOffset;
                set
                {
                    if (value == horizontalOffset) return;
                    horizontalOffset = value;
                    _LineInfo.XValueOffset = XValueOffset + value;
                }
            }
            public int DrawIndex { get; set; } = -1;
            public int DrawLength { get; set; } = -1;
            internal int DataLength { get; private set; } = 0;
            public Single[] Data
            {
                set
                {
                    if (value.Length == 0)
                    {
                        DataLength = 0;
                        return;
                    }
                    if (value.Length > maxdatalen)
                    {
                        throw new ArgumentException();
                    }
                    DataLength = value.Length;
                    SetBuffer?.Invoke(value);
                }
            }
            public RgbaFloat Color { get; set; }
            public RgbaFloat BorderColor { get; set; } = RgbaFloat.Clear;
            public float XValueOffset
            {
                get => xValueOffset;
                set
                {
                    if (value == xValueOffset) return;
                    xValueOffset = value;
                    _LineInfo.XValueOffset = xValueOffset + horizontalOffset;
                }
            }
            public float YValueOffset
            {
                get => yValueOffset;
                set
                {
                    if (value == yValueOffset) return;
                    yValueOffset = value;
                    _LineInfo.YValueOffset = yValueOffset + verticalOffset;
                }
            }
            public bool Visibily { get; set; } = true;
            public int ZIndex { get; set; }


        }
    }

}
