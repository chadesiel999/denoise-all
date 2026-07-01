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
using Vulkan;

namespace Veldrid.Common.VeldridRender.LineRender
{
    internal unsafe class DigitalRender : BaseVeldridRender
    {
        #region PipLine
        [AllowNull]
        Pipeline _LinePipeline;
        Pipeline _PointsPipline;
        #endregion

        #region vertex
        [AllowNull]
        DeviceBuffer vertexBuffer;
        VertexLayoutDescription vertexLayout;
        #endregion

        #region Uniform

        /// <summary>
        /// vec4 linerange
        /// float VerticalOffset
        /// float HorizontalOffset
        /// float SampleRate
        /// </summary>
        [AllowNull]
        DeviceBuffer lineInfoBuffer;
        /// <summary>
        /// vec4 color
        /// float Brightness
        /// </summary>
        [AllowNull]
        DeviceBuffer _ColorInfoBuffer;

        [AllowNull]
        DeviceBuffer proviewBuffer;
        [AllowNull]
        ResourceLayout sharedLayout;
        [AllowNull]
        ResourceSet sharedSet;
        #endregion 
        public DigitalRender(IVeldridContent control, int maxChDataCount = 2000, int chCount = 16) : base(control)
        {
            MaxCHDataCount = maxChDataCount;
            ChCount = chCount;
            DigitalCh = new List<DigitalChConfig>();
            for (int i = 0; i < chCount; i++)
            {
                DigitalCh.Add(new DigitalChConfig(i)
                {
                    DrawLength = maxChDataCount,
                });
            }
            //DigitalCh = Enumerable.Range(0, chCount).Select(x => new DigitalChConfig(x)
            //{
            //    DrawLength = maxChDataCount,
            //}).ToList().AsReadOnly();
        }

        public List<DigitalChConfig> DigitalCh { get; }
        public int ChCount { get; }
        public override void CreateResources()
        {
            base.CreateResources();
            vertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<float>() * MaxCHDataCount * ChCount), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            vertexLayout = new VertexLayoutDescription(new VertexElementDescription("in_Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float1));

            ResourceLayoutElementDescription[] resourceLayoutElementDescriptions =
            {
                new ResourceLayoutElementDescription("ProView", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("LineInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("ColorInfo", ResourceKind.UniformBuffer, ShaderStages.Fragment),
            };
            proviewBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>() * 2, BufferUsage.Dynamic | BufferUsage.UniformBuffer));
            lineInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Vector4>() * 3), BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            _ColorInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Vector4>() * 2), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            ResourceLayoutDescription resourceLayoutDescription = new ResourceLayoutDescription(resourceLayoutElementDescriptions);
            sharedLayout = ResourceFactory.CreateResourceLayout(resourceLayoutDescription);
            BindableResource[] bindableResources = new BindableResource[] { proviewBuffer, lineInfoBuffer, _ColorInfoBuffer };
            ResourceSetDescription resourceSetDescription = new ResourceSetDescription(sharedLayout, bindableResources);
            sharedSet = ResourceFactory.CreateResourceSet(resourceSetDescription);
            var tempshaders = CreateShader("DigitalRender");
            switch (GraphicsDevice.BackendType)
            {
                case GraphicsBackend.Direct3D11:
                    {
                        var pointshader = (GetOtherShader("DigitalRenderPoint.geometry.hlsl", ShaderStages.Geometry, "main"));
                        var lineshader = (GetOtherShader("DigitalRenderLine.geometry.hlsl", ShaderStages.Geometry, "main"));
                        _PointsPipline = CreatePipLine(PrimitiveTopology.LineStrip, sharedLayout, vertexLayout, new Shader[] { tempshaders[0], tempshaders[1], pointshader });
                        _LinePipeline = CreatePipLine(PrimitiveTopology.LineStrip, sharedLayout, vertexLayout, new Shader[] { tempshaders[0], tempshaders[1], lineshader });
                    }
                    break;
                case GraphicsBackend.OpenGL:
                case GraphicsBackend.OpenGLES:
                case GraphicsBackend.Vulkan:
                    {
                        var pointshader = (GetOtherShader("DigitalRenderPoint.geom", ShaderStages.Geometry, "main"));
                        var lineshader = (GetOtherShader("DigitalRenderLine.geom", ShaderStages.Geometry, "main"));
                        _PointsPipline = CreatePipLine(PrimitiveTopology.LineStrip, sharedLayout, vertexLayout, new Shader[] { tempshaders[0], tempshaders[1], pointshader });
                        _LinePipeline = CreatePipLine(PrimitiveTopology.LineStrip, sharedLayout, vertexLayout, new Shader[] { tempshaders[0], tempshaders[1], lineshader });
                    }
                    break;
                case GraphicsBackend.Metal:
                    _PointsPipline = CreatePipLine(PrimitiveTopology.PointList, sharedLayout, vertexLayout, tempshaders);
                    _LinePipeline = CreatePipLine(PrimitiveTopology.LineStrip, sharedLayout, vertexLayout, tempshaders);
                    break;

            }
            this[nameof(Range), nameof(Margin),nameof(ValueScale)] = true;
        }
        private void UpdateBrightness()
        {
            if (CommandList == null || _ColorInfoBuffer == null) return;
            CommandList.UpdateBuffer(_ColorInfoBuffer, (uint)(Unsafe.SizeOf<Vector4>()), Brightness);
        }
        private void UpdateSampleRate()
        {
            if (CommandList == null || lineInfoBuffer == null) return;
            CommandList.UpdateBuffer(lineInfoBuffer, (uint)(Unsafe.SizeOf<Vector4>() + Unsafe.SizeOf<float>() * 2), SampleRate);
        }
        public PrimitiveTopology PrimitiveTopology { get; set; } = PrimitiveTopology.LineStrip;
        public int MaxCHDataCount { get; private set; }
        public float SampleRate { get => sampleRate; set => Set(ref sampleRate, value); }
        public float HorizontalOffset { get => horizontalOffset; set => Set(ref horizontalOffset, value); }
        private float brightness = 100f;
        private float sampleRate = 1;
        private float horizontalOffset;


        public float ValueScale
        {
            get => valueScale; set =>Set(ref valueScale,value);
        }
        public float Brightness
        {
            get => brightness;
            set
            {
                float temp = Math.Clamp(value, 0.0f, 100.0f);
                Set(ref brightness, temp);
            }
        }
        public int ChdataCount { get; set; }
        internal override void DrawData()
        {
            if (ChdataCount == 0 || !Visibily || DigitalCh.All(x => !x.Visibily) || IsDisposed || vertexBuffer.IsDisposed || lineInfoBuffer.IsDisposed || _ColorInfoBuffer.IsDisposed) return;
            //if (needupdate && _TempData != null)
            //{
            //    CommandList.UpdateBuffer(vertexBuffer, 0, _TempData);
            //    needupdate = false;
            //}
            Pipeline pipeline = PrimitiveTopology == PrimitiveTopology.LineStrip ? _LinePipeline : _PointsPipline;
            CommandList.SetFramebuffer(MainSwapchainBuffer);
            CommandList.SetVertexBuffer(0, vertexBuffer);
            CommandList.SetPipeline(pipeline);
            foreach (var val in DigitalCh.Where(x => x.Visibily).OrderBy(x => x.ZIndex))
            {
                uint startaddr = (uint)(val.Index * ChdataCount + val.SkipCount);
                uint count = (uint)(Math.Min(ChdataCount - val.SkipCount, val.DrawLength));
                CommandList.SetGraphicsResourceSet(0, sharedSet);
                CommandList.UpdateBuffer(lineInfoBuffer, (uint)(Unsafe.SizeOf<Vector4>()), val.VerticalOffset);
                CommandList.UpdateBuffer(lineInfoBuffer, (uint)(Unsafe.SizeOf<Vector4>() + Unsafe.SizeOf<float>() * 4), val.VerticalPos);
                CommandList.UpdateBuffer(_ColorInfoBuffer, 0, val.Color);
                CommandList.Draw(count, 1, startaddr, 0);
            }
        }
        [AllowNull]
        //float[] _TempData = new float[0];
        private float valueScale=1;

        public void SetData(float[] data)
        {
            if (data == null || data.Length > MaxCHDataCount * ChCount || vertexBuffer ==null || vertexBuffer.IsDisposed) return;
            lock (_Locker)
            {
                GraphicsDevice.UpdateBuffer(vertexBuffer, 0, data);
            }
        }
        public override void DisposeResources()
        {
            lock (_Locker)
            {
                vertexBuffer?.Dispose();
                lineInfoBuffer?.Dispose();
                sharedLayout?.Dispose();
                sharedSet?.Dispose();
                _PointsPipline?.Dispose();
                _LinePipeline?.Dispose();
                _ColorInfoBuffer?.Dispose();
                base.DisposeResources();
            }

        }
        internal override void PreDraw()
        {
            if (IsDisposed || proviewBuffer.IsDisposed || lineInfoBuffer.IsDisposed) return;
            base.PreDraw();
            if (this[nameof(Range), nameof(Margin)] || WindowSizeState)
            {
                CommandList.UpdateBuffer(proviewBuffer, 0, Camera.OrthographicMatrix);
                CommandList.UpdateBuffer(proviewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>(), Camera.GetLineMatrix(Margin, Range));
                CommandList.UpdateBuffer(lineInfoBuffer, 0, (Vector4)Range);
                this[nameof(Range), nameof(Margin)] = false;
                if (WindowSizeState) WindowSizeState = false;
            }
            if (this[nameof(Brightness)])
            {
                UpdateBrightness();
                this[nameof(Brightness)] = false;
            }
            if (this[nameof(SampleRate)])
            {
                UpdateSampleRate();
                this[nameof(SampleRate)] = false;
            }
            if (this[nameof(HorizontalOffset)])
            {
                CommandList.UpdateBuffer(lineInfoBuffer, (uint)(Unsafe.SizeOf<Vector4>() + Unsafe.SizeOf<float>()), HorizontalOffset);
                this[nameof(HorizontalOffset)] = false;
            }
            if (this[nameof(ValueScale)])
            {
                CommandList.UpdateBuffer(lineInfoBuffer, (uint)(Unsafe.SizeOf<Vector4>() + Unsafe.SizeOf<float>()*3), ValueScale);
                this[nameof(ValueScale)] = false;
            }
        }
    }

    internal unsafe class DigitalChConfig
    {

        public unsafe DigitalChConfig(int index)
        {
            this.Index = index;
        }
        public int SkipCount { get; set; } = 0;
        public int DrawLength { get; set; }
        public float VerticalOffset { get; set; }
        public float VerticalPos { get; set; }

        public RgbaFloat Color { get; set; }
        public Boolean Visibily { get; set; }
        public int Index { get; }
        public int ZIndex { get; set; }
    }
}
