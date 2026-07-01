using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Veldrid.Common.VeldridRender.LineRender
{
    internal unsafe class ThressDimensionsRender : BaseVeldridRender
    {
        Pipeline linePipline;
        Pipeline pointPipline;


        DeviceBuffer _VertexBuffer;
        DeviceBuffer _ProjViewBuffer;
        DeviceBuffer _LineInfoBuffer;

        ResourceLayout _ShardLayout;
        ResourceSet _ShardSet;
        Shader[] _Shards;

        private int currentindex = 0;
        private LineInfo lineInfo = new LineInfo();
        public ThressDimensionsRender(IVeldridContent control, int chDataLenght = 10000, int maxFrameCount = 30) : base(control)
        {
            lineInfo.Brightness = 100;
            lineInfo.FrameCount = maxFrameCount;
            lineInfo.FrameLenght = chDataLenght;
            Max = 1000;
            Min = -1000;
        }
        public override void CreateResources()
        {
            _Shards = CreateShader("ThreeDimensionsRender");
            _VertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<float>() * FrameLenght * MaxFrameCount), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(new VertexElementDescription("Position", VertexElementFormat.Float1, VertexElementSemantic.TextureCoordinate));
            _ProjViewBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Matrix4x4>() * 3), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _LineInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<LineInfo>()), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _ShardLayout = ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ProjView", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("LineInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
            _ShardSet = ResourceFactory.CreateResourceSet(new ResourceSetDescription(_ShardLayout, _ProjViewBuffer, _LineInfoBuffer));

            pointPipline = CreatePipLine(PrimitiveTopology.PointList, _ShardLayout, vertexLayout, _Shards, BlendStateDescription.SingleAlphaBlend);
            if (GraphicsDevice.BackendType == GraphicsBackend.Direct3D11)
            {

                linePipline = CreatePipLine(PrimitiveTopology.LineStrip, _ShardLayout, vertexLayout, new Shader[] { _Shards[0], _Shards[1],GetOtherShader("ThreeDimensionsRenderLine.geometry.hlsl") }, BlendStateDescription.SingleAlphaBlend);
            }
            else
            {
                linePipline = CreatePipLine(PrimitiveTopology.LineStrip, _ShardLayout, vertexLayout, _Shards, BlendStateDescription.SingleAlphaBlend);
            }
        }

        public int MaxFrameCount => (int)lineInfo.FrameCount;
        public int TotalFrameCount { get; private set; } = 0;
        public float Brightness
        {
            get => lineInfo.Brightness;
            set
            {
                if (lineInfo.Brightness != value)
                {
                    lineInfo.Brightness = value;
                }
            }
        }
        public PrimitiveTopology PrimitiveTopology { get; set; } = PrimitiveTopology.LineStrip;
        public int FrameLenght => (int)lineInfo.FrameLenght;
        public RgbaFloat MinColor { get => lineInfo.MinColor; set => lineInfo.MinColor = value; }
        public RgbaFloat MiddleColor { get => lineInfo.MiddleColor; set => lineInfo.MiddleColor = value; }
        public RgbaFloat LastColor { get => lineInfo.LastColor; set => lineInfo.LastColor = value; }
        public float MinValue { get => lineInfo.MinValue; set => lineInfo.MinValue = value; }
        public float MiddleValue { get => lineInfo.MiddleValue; set => lineInfo.MiddleValue = value; }
        public float LastValue { get => lineInfo.LastValue; set => lineInfo.LastValue = value; }

        public void SetData(float[] data)
        {
            if (data == null || data.Length == 0 || data.Length != FrameLenght || _VertexBuffer ==null ||_VertexBuffer.IsDisposed) return;
            lock (_Locker)
            {
                GraphicsDevice.UpdateBuffer(_VertexBuffer, (uint)(currentindex * FrameLenght * Unsafe.SizeOf<float>()), data);
            }
            currentindex++;
            TotalFrameCount++;
            if (TotalFrameCount > MaxFrameCount) TotalFrameCount = MaxFrameCount;
            if (currentindex >= MaxFrameCount) currentindex = 0;
        }
        internal override void DrawData()
        {
            if (TotalFrameCount == 0 || _ProjViewBuffer.IsDisposed || _LineInfoBuffer.IsDisposed || _VertexBuffer.IsDisposed) return;
            Pipeline pipeline = PrimitiveTopology == PrimitiveTopology.LineStrip ? linePipline : pointPipline;

            CommandList.UpdateBuffer(_ProjViewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>(), Camera.ProjectionMatrix);
            CommandList.UpdateBuffer(_ProjViewBuffer, 0, Camera.ViewMatrix);

            CommandList.UpdateBuffer(_ProjViewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>() * 2, Camera.ModelMatrix);

            CommandList.SetFramebuffer(MainSwapchainBuffer);
            CommandList.SetVertexBuffer(0, _VertexBuffer);
            CommandList.SetPipeline(pipeline);
            CommandList.SetGraphicsResourceSet(0, _ShardSet);
            lineInfo.TotalFrameCount = TotalFrameCount;
            lineInfo.FrameIndex = currentindex;

            if(TotalFrameCount<MaxFrameCount)              
            {
                lineInfo.First = 0;
                CommandList.UpdateBuffer(_LineInfoBuffer, 0, lineInfo);
                CommandList.Draw((uint)(TotalFrameCount*FrameLenght), 1, 0, 0);
            }
            else
            {
                lineInfo.First = 1;
                CommandList.UpdateBuffer(_LineInfoBuffer, 0, lineInfo);
                CommandList.Draw((uint)((TotalFrameCount - currentindex) * FrameLenght), 1, (uint)(currentindex * FrameLenght), 0);
                lineInfo.First = 0;
                CommandList.UpdateBuffer(_LineInfoBuffer, 0, lineInfo);
                CommandList.Draw((uint)(currentindex * FrameLenght), 1, 0, 0);
            }

        }

        private void UpdateFrameIndex(float index)
        {
            CommandList.UpdateBuffer(_LineInfoBuffer, (uint)(Unsafe.SizeOf<Vector4>() * 4 + Unsafe.SizeOf<float>() * 2), index);
        }
        public float Max
        { 
            get => lineInfo.Max;
            set
            {
                if(value!=lineInfo.Max)
                {
                    lineInfo.Max = value;
                }
            } 
        }
        public float Min 
        {
            get => lineInfo.Min; 
            set
            {
                if(value!=lineInfo.Min)
                {
                    lineInfo.Min = value;
                }
            }
        }

        public override void DisposeResources()
        {
            lock (_Locker)
            {
                base.DisposeResources();
                _VertexBuffer?.Dispose();
                _LineInfoBuffer?.Dispose();
                _ProjViewBuffer?.Dispose();
                _ShardLayout?.Dispose();
                _ShardSet?.Dispose();
                linePipline?.Dispose();
                pointPipline?.Dispose();
            }
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]

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

            public float Max;
            public float Min;
            public float First;
            public float Spare2;
        }

    }
}
