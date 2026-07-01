using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Veldrid.Common.VeldridRender.LineRender
{

    internal unsafe class WaterfallRender : BaseVeldridRender
    {
        Pipeline linePipline;
        Pipeline pointPipline;


        DeviceBuffer _VertexBuffer;
        DeviceBuffer _LineInfoBuffer;

        ResourceLayout _ShardLayout;
        ResourceSet _ShardSet;
        Shader[] _Shards;

        private int currentindex = 0;
        private LineInfo lineInfo = new LineInfo();
        public WaterfallRender(IVeldridContent control,int chDataLenght=10000,int maxFrameCount = 30) : base(control)
        {
            matrix4 = Matrix4x4.CreateLookAt(new Vector3(0, -2, 0), Vector3.Zero, Vector3.UnitY);
            lineInfo.FrameCount = maxFrameCount;
            lineInfo.FrameLenght = chDataLenght;
        }
        public override void CreateResources()
        {
            _Shards = CreateShader("WaterfallRender");
            _VertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<float>() * FrameLenght * MaxFrameCount), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(new VertexElementDescription("Position", VertexElementFormat.Float1, VertexElementSemantic.TextureCoordinate));
            _LineInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<LineInfo>()), BufferUsage.UniformBuffer| BufferUsage.Dynamic));
            _ShardLayout = ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("LineInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
            _ShardSet = ResourceFactory.CreateResourceSet(new ResourceSetDescription(_ShardLayout,  _LineInfoBuffer));
            pointPipline = CreatePipLine(PrimitiveTopology.PointList, _ShardLayout, vertexLayout, _Shards, BlendStateDescription.SingleAlphaBlend);
            if (GraphicsDevice.BackendType == GraphicsBackend.Direct3D11)
            {

                linePipline = CreatePipLine(PrimitiveTopology.LineStrip, _ShardLayout, vertexLayout, new Shader[] { _Shards[0], _Shards[1], GetOtherShader("ThreeDimensionsRenderLine.geometry.hlsl") }, BlendStateDescription.SingleAlphaBlend);
            }
            else
            {
                linePipline = CreatePipLine(PrimitiveTopology.LineStrip, _ShardLayout, vertexLayout, _Shards, BlendStateDescription.SingleAlphaBlend);
            }
        }
        public int MaxFrameCount => (int)lineInfo.FrameCount;
        public int TotalFrameCount { get=>(int)lineInfo.TotalFrameCount; private set=>lineInfo.TotalFrameCount =value; } 
        public float Brightness
        {
            get=>lineInfo.Brightness;
            set
            {
                if (lineInfo.Brightness != value)
                {
                    lineInfo.Brightness = value;
                }
            }
        }
        public PrimitiveTopology PrimitiveTopology { get; set; } = PrimitiveTopology.LineStrip;
        public int FrameLenght=>(int)lineInfo.FrameLenght;
        public RgbaFloat MinColor { get => lineInfo.MinColor;set=>lineInfo.MinColor = value; }
        public RgbaFloat MiddleColor { get => lineInfo.MiddleColor; set => lineInfo.MiddleColor = value; }
        public RgbaFloat LastColor { get => lineInfo.LastColor; set => lineInfo.LastColor = value; }
        public float MinValue { get => lineInfo.MinValue;set=>lineInfo.MinValue = value; }
        public float MiddleValue { get => lineInfo.MiddleValue; set => lineInfo.MiddleValue = value; }
        public float LastValue { get => lineInfo.LastValue; set => lineInfo.LastValue = value; }

        public void SetData(float[] data)
        {
            if (data == null || data.Length == 0 || data.Length != FrameLenght || _VertexBuffer == null || _VertexBuffer.IsDisposed || IsDisposed) return;
            lock (_Locker)
            {
                GraphicsDevice.UpdateBuffer(_VertexBuffer, (uint)(currentindex * FrameLenght * Unsafe.SizeOf<float>()), data);
            }
            currentindex++;
            TotalFrameCount++;
            if (TotalFrameCount > MaxFrameCount) TotalFrameCount = MaxFrameCount;
            if (currentindex >= MaxFrameCount) currentindex = 0;
        }
        Matrix4x4 matrix4;
        internal override void DrawData()
        {
            if (TotalFrameCount == 0 || _VertexBuffer.IsDisposed||_LineInfoBuffer.IsDisposed || IsDisposed) return;
            Pipeline pipeline = PrimitiveTopology == PrimitiveTopology.LineStrip ? linePipline : pointPipline;

            CommandList.SetFramebuffer(MainSwapchainBuffer);
            CommandList.SetVertexBuffer(0,_VertexBuffer);
            CommandList.SetPipeline(pipeline);
            CommandList.SetGraphicsResourceSet(0, _ShardSet);
            lineInfo.FrameIndex = currentindex;
            CommandList.UpdateBuffer(_LineInfoBuffer, 0, lineInfo);
            CommandList.Draw((uint)(TotalFrameCount * FrameLenght), 1, 0, 0);


        }

        public override void DisposeResources()
        {
            lock (_Locker)
            {
                base.DisposeResources();
                _VertexBuffer?.Dispose();
                _LineInfoBuffer?.Dispose();
                _ShardLayout?.Dispose();
                _ShardSet?.Dispose();
                linePipline?.Dispose();
                pointPipline?.Dispose();
            }
        }
        [StructLayout(LayoutKind.Sequential,Pack =1)]

        private struct LineInfo
        {
            public RgbaFloat MinColor;
            public RgbaFloat MiddleColor;
            public RgbaFloat LastColor;
            public float MinValue;
            public float MiddleValue;
            public float LastValue;
            public float Brightness;

            public float TotalFrameCount;
            public float FrameLenght;
            public float FrameIndex;
            public float FrameCount;
        }

    }
}
