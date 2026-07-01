using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Veldrid.Common.Tools;

namespace Veldrid.Common.VeldridRender.LineRender
{
    internal class MathPlotRender : BaseVeldridRender
    {
        #region PipLine
        private Pipeline _PointPipline;
        private Pipeline _LinePipline;
        #endregion

        #region vertex
        [AllowNull]
        private DeviceBuffer _VertexBuffer;
        private VertexLayoutDescription _VertexLayout;
        #endregion

        #region Uniform
        [AllowNull]
        private Shader[] shaders;
        /// <summary>
        /// rgbafloat color
        /// vec4 linerange
        /// float VerticalOffset
        /// float HorizontalOffset
        /// float Brightness
        /// float spare
        /// </summary>
        [AllowNull]
        private DeviceBuffer _LineInfoBuffer;

        [AllowNull]
        private DeviceBuffer _ProviewBuffer;
        [AllowNull]
        private ResourceLayout _SharedLayout;
        [AllowNull]
        private ResourceSet _SharedSet;
        #endregion

        #region FrameBuffer

        
        private float sampleRate = 1.0f;
        private Color color;
        private float horizontalOffset;
        private float verticalOffset;

        //VeldridSpriteBatch mainspriteBatch;

        #endregion

        public MathPlotRender(IVeldridContent control) : base(control)
        {
        }
        public override void CreateResources()
        {
            base.CreateResources();
            _VertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<float>() * 1000), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            _VertexLayout = new VertexLayoutDescription(elements: new VertexElementDescription("in_Position", VertexElementFormat.Float1, VertexElementSemantic.TextureCoordinate));
            shaders = CreateShader("MathPlotRender");
            ResourceLayoutElementDescription[] resourceLayoutElementDescriptions =
 {
                new ResourceLayoutElementDescription("ProView", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("LineInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            };
            _ProviewBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>() * 2, BufferUsage.Dynamic | BufferUsage.UniformBuffer));
            _LineInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Vector4>() * 3), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            ResourceLayoutDescription resourceLayoutDescription = new ResourceLayoutDescription(resourceLayoutElementDescriptions);
            _SharedLayout = ResourceFactory.CreateResourceLayout(resourceLayoutDescription);
            BindableResource[] bindableResources = new BindableResource[] { _ProviewBuffer, _LineInfoBuffer };
            ResourceSetDescription resourceSetDescription = new ResourceSetDescription(_SharedLayout, bindableResources);
            _SharedSet = ResourceFactory.CreateResourceSet(resourceSetDescription);
            this[nameof(Range), nameof(Margin), nameof(Brightness), nameof(Color), nameof(HorizontalOffset), nameof(SampleRate), nameof(VerticalOffset)] = true;

            CreatePipLine();
        }
        private void CreatePipLine()
        {
            _LinePipline = CreatePipLine(PrimitiveTopology.LineStrip, _SharedLayout, _VertexLayout, shaders, BlendStateDescription.SingleAlphaBlend);
            _PointPipline = CreatePipLine(PrimitiveTopology.PointList, _SharedLayout, _VertexLayout, shaders, BlendStateDescription.SingleAlphaBlend);
        }


        internal override void PreDraw()
        {
            if (IsDisposed || _ProviewBuffer.IsDisposed || _LineInfoBuffer.IsDisposed) return;
            if (this[nameof(Margin), nameof(Range)] || WindowSizeState)
            {
                CommandList.UpdateBuffer(_ProviewBuffer, 0, Camera.OrthographicMatrix);
                CommandList.UpdateBuffer(_ProviewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>(), Camera.GetLineMatrix(Margin, Range));
                CommandList.UpdateBuffer(_LineInfoBuffer, (uint)Unsafe.SizeOf<RgbaFloat>(), (Vector4)Range);
                this[nameof(Range), nameof(Margin)] = false;
                if (WindowSizeState)
                {
                    WindowSizeState = false;
                }
            }
            if (this[nameof(HorizontalOffset)])
            {
                UpdateHorizontalOffset();
                this[nameof(HorizontalOffset)] = false;
            }
            if (this[nameof(Brightness)])
            {
                UpdateBrightness();
                this[nameof(Brightness)] = false;
            }
            if (this[nameof(Color)])
            {
                UpdateColor();
                this[nameof(Color)] = false;
            }
            if (this[nameof(SampleRate)])
            {
                UpdateSampleRate();
                this[nameof(SampleRate)] = false;
            }
            if (this[nameof(VerticalOffset)])
            {
                UpdateVerticalOffset();
                this[nameof(VerticalOffset)] = false;
            }
            base.PreDraw();
        }
        internal override void DrawData()
        {
            if (datalenght == 0 || _VertexBuffer.IsDisposed || IsDisposed || _ProviewBuffer.IsDisposed || _LineInfoBuffer.IsDisposed)
            {
                return;
            }

            Pipeline pipeline = PrimitiveTopology == PrimitiveTopology.LineStrip ? _LinePipline : _PointPipline;
            CommandList.SetFramebuffer(MainSwapchainBuffer);
            CommandList.SetVertexBuffer(0, _VertexBuffer);
            CommandList.SetPipeline(pipeline);
            CommandList.SetGraphicsResourceSet(0, _SharedSet);
            CommandList.Draw(datalenght);
        }
        private void UpdateBrightness()
        {
            CommandList.UpdateBuffer(_LineInfoBuffer, (uint)(Unsafe.SizeOf<Vector4>() * 2 + Unsafe.SizeOf<float>() * 2), Brightness);
        }
        private void UpdateSampleRate()
        {
            CommandList.UpdateBuffer(_LineInfoBuffer, (uint)(Unsafe.SizeOf<Vector4>() * 2 + Unsafe.SizeOf<float>() * 3), SampleRate);
        }
        private void UpdateColor()
        {
            CommandList.UpdateBuffer(_LineInfoBuffer, 0, Color.ColorConverToVect4());
        }
        private void UpdateHorizontalOffset()
        {
            CommandList.UpdateBuffer(_LineInfoBuffer, (uint)(Unsafe.SizeOf<RgbaFloat>() + Unsafe.SizeOf<Vector4>() + Unsafe.SizeOf<float>()), HorizontalOffset);
        }
        private void UpdateVerticalOffset()
        {
            CommandList.UpdateBuffer(_LineInfoBuffer, (uint)(Unsafe.SizeOf<RgbaFloat>() + Unsafe.SizeOf<Vector4>()), VerticalOffset);
        }

        private float _Brightness = 100;
        public float Brightness
        {
            get => _Brightness;
            set
            {
                float temp = Math.Clamp(value, 0.0f, 100.0f);
                Set(ref _Brightness, temp);
            }
        }
        private UInt32 datalenght = 0;
        public void SetDatas(Single[] datas)
        {
            lock (_Locker)
            {
                if (_VertexBuffer == null || _VertexBuffer.IsDisposed || IsDisposed)
                {
                    return;
                }

                datalenght = (UInt32)datas.Length;
                if (datalenght == 0)
                {
                    return;
                }

                if (datalenght > _VertexBuffer.SizeInBytes / 4)
                {
                    _VertexBuffer?.Dispose();
                    _VertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<float>() * datalenght), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
                }                
                GraphicsDevice.UpdateBuffer(_VertexBuffer, 0, datas);
            }
        }
        public float SampleRate
        {
            get => sampleRate;
            set
            {
                Set(ref sampleRate, value);
            }
        }
        public Color Color
        {
            get => color;
            set
            {
                Set(ref color, value);
            }
        }
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
        public PrimitiveTopology PrimitiveTopology { get; set; } = PrimitiveTopology.LineStrip;
        public override void DisposeResources()
        {
            lock (_Locker)
            {
                if (IsDisposed)
                {
                    return;
                }

                base.DisposeResources();
                _VertexBuffer?.Dispose();

                _LineInfoBuffer?.Dispose();
                _LinePipline?.Dispose();
                _PointPipline?.Dispose();
                _ProviewBuffer?.Dispose();
                _SharedLayout?.Dispose();
                _SharedSet?.Dispose();
                IsDisposed = false;
            }
        }

    }
}
