using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid.Common.Tools;

namespace Veldrid.Common.VeldridRender.LineRender
{
    internal unsafe class ThreeDimLineRender : BaseVeldridRender
    {
        #region PipLine
        Dictionary<PrimitiveTopology, Pipeline> _PipLines = new Dictionary<PrimitiveTopology, Pipeline>();
        [AllowNull]
        Pipeline currentPipline;
        PrimitiveTopology currentPrimitive;
        #endregion

        VertexLayoutDescription vertexLayout;
        DeviceBuffer _VertexBuffer;
        DeviceBuffer _ProjViewBuffer;
        DeviceBuffer _LineInfoBuffer;

        ResourceLayout _ShardLayout;
        ResourceSet _ShardSet;
        Shader[] _Shards;
        public ThreeDimLineRender(IVeldridContent control,int maxDataCount = 1000) : base(control)
        {
            MaxDataCount= maxDataCount;
        }
        public DataRenderConfig[] DataRenderConfigs { get; set; } = new DataRenderConfig[0];
        public override void CreateResources()
        {
            _Shards = CreateShader("ThreeDimLineRender");
            _VertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Vector3>() * MaxDataCount), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            vertexLayout = new VertexLayoutDescription(new VertexElementDescription("Position", VertexElementFormat.Float3, VertexElementSemantic.TextureCoordinate));
            _ProjViewBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Matrix4x4>() * 3), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _LineInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Vector4>()), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _ShardLayout = ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ProjView", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("LineInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
            _ShardSet = ResourceFactory.CreateResourceSet(new ResourceSetDescription(_ShardLayout, _ProjViewBuffer, _LineInfoBuffer));

        }
        public void WriteData(int offset, Vector3[] datas)
        {
            lock (_Locker)
            {
                if (IsDisposed || _VertexBuffer.IsDisposed) return;
                GraphicsDevice.UpdateBuffer(_VertexBuffer, (uint)(offset * Unsafe.SizeOf<Vector3>()), datas);
            }
        }

        private Pipeline GetPipeline(PrimitiveTopology primitiveTopology)
        {
            if (currentPipline != null && currentPrimitive == primitiveTopology)
            {
                return currentPipline;
            }
            if (_PipLines.TryGetValue(primitiveTopology, out Pipeline pipeline))
            {
                currentPipline = pipeline;
                currentPrimitive = primitiveTopology;
                return pipeline;
            }
            else
            {
                GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
                pipelineDescription.BlendState = BlendStateDescription.SingleAlphaBlend;
                pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                    depthTestEnabled: true,
                    depthWriteEnabled: true,
                    comparisonKind: ComparisonKind.LessEqual);
                pipelineDescription.RasterizerState = new RasterizerStateDescription(
                    cullMode: FaceCullMode.Back,
                    fillMode: PolygonFillMode.Solid,
                    frontFace: FrontFace.Clockwise,
                    depthClipEnabled: true,
                    scissorTestEnabled: false);
                pipelineDescription.PrimitiveTopology = primitiveTopology;
                pipelineDescription.ResourceLayouts = new ResourceLayout[] { _ShardLayout };
                pipelineDescription.ShaderSet = new ShaderSetDescription(
                    vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                    shaders: _Shards);
                pipelineDescription.Outputs = GraphicsDevice.SwapchainFramebuffer.OutputDescription;
                pipeline = ResourceFactory.CreateGraphicsPipeline(pipelineDescription);
                _PipLines[primitiveTopology] = pipeline;
                currentPipline = pipeline;
                currentPrimitive = primitiveTopology;
                return pipeline;
            }
        }
        internal override void DrawData()
        {
            if (DataRenderConfigs.Length == 0 || _VertexBuffer.IsDisposed || _ProjViewBuffer.IsDisposed || _LineInfoBuffer.IsDisposed || IsDisposed) return;
            CommandList.UpdateBuffer(_ProjViewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>(), Camera.ProjectionMatrix);
            CommandList.UpdateBuffer(_ProjViewBuffer, 0, Camera.ViewMatrix);
            CommandList.UpdateBuffer(_ProjViewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>() * 2,Camera.ModelMatrix);
            CommandList.SetVertexBuffer(0, _VertexBuffer);
            uint offset = 0;
            for (int i = 0; i < DataRenderConfigs.Length; i++)
            {
                if (DataRenderConfigs[i].DataLenght == 0)
                {
                    offset += DataRenderConfigs[i].FixedDataLenght;
                    continue;
                }
                var pipline = GetPipeline(DataRenderConfigs[i].Primitive);
                CommandList.SetPipeline(pipline);
                CommandList.SetGraphicsResourceSet(0, _ShardSet);

                uint start = 0;
                foreach (var p in DataRenderConfigs[i].PointConfigs)
                {
                    CommandList.UpdateBuffer(_LineInfoBuffer, 0, p.Color.ColorConverToRGBA());
                    if (p.PointCounts != null && p.PointCounts.Length > 0)
                    {
                        foreach (var count in p.PointCounts)
                        {
                            if (count.Visibily) CommandList.Draw(count.PointCount, 1, start + offset, 0);
                            start += count.FixedPointCount;
                        }
                    }
                    else
                    {
                        CommandList.Draw((uint)DataRenderConfigs[i].DataLenght, 1, offset, 0);
                    }
                }
                offset += (uint)DataRenderConfigs[i].FixedDataLenght;
            }
        }
        public int MaxDataCount { get; }

        public override void DisposeResources()
        {
            lock (_Locker)
            {
                base.DisposeResources();
                _PipLines.Values.ToList().ForEach(x => x.Dispose());
                _PipLines.Clear();
                _VertexBuffer?.Dispose();
                _ProjViewBuffer?.Dispose();
                _LineInfoBuffer?.Dispose();
                _ShardLayout?.Dispose();
                _ShardSet?.Dispose();
            }
        }
    }
}
