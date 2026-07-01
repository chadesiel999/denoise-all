using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid.Common.Tools;

namespace Veldrid.Common.VeldridRender.LineRender
{
    internal unsafe class RuntimeRender : BaseVeldridRender
    {
        #region PipLine
        Pipeline pointPipline;
        Pipeline linePipline;
        Pipeline spritepipline;
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

        #region FrameBuffer

        private float brightness = 100;
        private float sampleRate = 1.0f;
        private Color color;
        private float horizontalOffset;
        private float verticalOffset;
        List<float> tempdata = new List<float>();

        //VeldridSpriteBatch mainspriteBatch;

        #endregion


        private UInt32 cacheLenght = 1000;

        public RuntimeRender(IVeldridContent control) : base(control)
        {
        }
        public override void CreateResources()
        {
            base.CreateResources();
            vertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<float>() * cacheLenght), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            vertexLayout = new VertexLayoutDescription(new VertexElementDescription("in_Position", VertexElementFormat.Float1, VertexElementSemantic.TextureCoordinate));
            shaders = CreateShader("RuntimeRender");
            ResourceLayoutElementDescription[] resourceLayoutElementDescriptions =
 {
                new ResourceLayoutElementDescription("ProView", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("LineInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            };
            proviewBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>() * 2, BufferUsage.Dynamic | BufferUsage.UniformBuffer));
            lineInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Vector4>() * 4), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            ResourceLayoutDescription resourceLayoutDescription = new ResourceLayoutDescription(resourceLayoutElementDescriptions);
            sharedLayout = ResourceFactory.CreateResourceLayout(resourceLayoutDescription);
            BindableResource[] bindableResources = new BindableResource[] { proviewBuffer, lineInfoBuffer };
            ResourceSetDescription resourceSetDescription = new ResourceSetDescription(sharedLayout, bindableResources);
            sharedSet = ResourceFactory.CreateResourceSet(resourceSetDescription);
            this[nameof(Range), nameof(Margin), nameof(Brightness), nameof(Color), nameof(HorizontalOffset), nameof(SampleRate), nameof(VerticalOffset)] = true;

            CreatePipLine();
        }
        private void CreatePipLine()
        {
            linePipline = CreatePipLine(PrimitiveTopology.LineStrip, sharedLayout, vertexLayout, shaders, BlendStateDescription.SingleAlphaBlend);
            pointPipline = CreatePipLine(PrimitiveTopology.PointList, sharedLayout, vertexLayout, shaders, BlendStateDescription.SingleAlphaBlend);

        }

        internal override void PreDraw()
        {
            if (proviewBuffer.IsDisposed || lineInfoBuffer.IsDisposed || IsDisposed) return;
            if (this[nameof(Margin), nameof(Range)] || WindowSizeState)
            {
                CommandList.UpdateBuffer(proviewBuffer, 0, Camera.OrthographicMatrix);
                CommandList.UpdateBuffer(proviewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>(), Camera.GetLineMatrix(Margin, Range));
                CommandList.UpdateBuffer(lineInfoBuffer, (uint)Unsafe.SizeOf<RgbaFloat>(), (Vector4)Range);
                this[nameof(Range), nameof(Margin)] = false;
                if (WindowSizeState) WindowSizeState = false;
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
            if (vertexBuffer.IsDisposed ||IsDisposed || proviewBuffer.IsDisposed ||lineInfoBuffer.IsDisposed) return;
            Pipeline pipeline = PrimitiveTopology == PrimitiveTopology.LineStrip ? linePipline : pointPipline;
            CommandList.SetFramebuffer(MainSwapchainBuffer);
            CommandList.SetVertexBuffer(0, vertexBuffer);
            CommandList.SetPipeline(pipeline);
            CommandList.SetGraphicsResourceSet(0, sharedSet);
            CommandList.Draw((UInt32)tempdata.Count);
        }
        private void UpdateBrightness()
        {
            CommandList.UpdateBuffer(lineInfoBuffer, (uint)(Unsafe.SizeOf<Vector4>() * 2 + Unsafe.SizeOf<float>() * 2), Brightness);
        }
        private void UpdateSampleRate()
        {
            CommandList.UpdateBuffer(lineInfoBuffer, (uint)(Unsafe.SizeOf<Vector4>() * 2 + Unsafe.SizeOf<float>() * 3), SampleRate);
        }
        private void UpdateColor()
        {
            CommandList.UpdateBuffer(lineInfoBuffer, 0, Color.ColorConverToVect4());
        }
        private void UpdateHorizontalOffset()
        {
            CommandList.UpdateBuffer(lineInfoBuffer, (uint)(Unsafe.SizeOf<RgbaFloat>() + Unsafe.SizeOf<Vector4>() + Unsafe.SizeOf<float>()), HorizontalOffset);
        }
        private void UpdateVerticalOffset()
        {
            CommandList.UpdateBuffer(lineInfoBuffer, (uint)(Unsafe.SizeOf<RgbaFloat>() + Unsafe.SizeOf<Vector4>()), VerticalOffset);
        }

        public UInt32 CacheLenght 
        { 
            get => cacheLenght;
            set
            {
                if (value == 0 || value == cacheLenght || IsDisposed) return;
                cacheLenght = value;
                vertexBuffer?.Dispose();
                vertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription(cacheLenght * 4, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
                tempdata.Clear();
            }
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
        public void SetData(Single data)
        {
            lock (_Locker)
            {
                if (vertexBuffer == null || vertexBuffer.IsDisposed || IsDisposed) return;
                tempdata.Add(data);
                if (tempdata.Count > CacheLenght) tempdata.RemoveAt(0);
                GraphicsDevice.UpdateBuffer(vertexBuffer, 0, tempdata.ToArray());
            }
        }
        public void ClearCache()
        {
            tempdata.Clear();
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
                if (IsDisposed) return;
                base.DisposeResources();
                vertexBuffer?.Dispose();
                tempdata.Clear();
                lineInfoBuffer?.Dispose();
                linePipline?.Dispose();
                pointPipline?.Dispose();
                proviewBuffer?.Dispose();
                sharedLayout?.Dispose();
                sharedSet?.Dispose();
                spritepipline?.Dispose();
                IsDisposed = false;
            }
        }

    }
}
