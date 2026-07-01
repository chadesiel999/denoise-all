using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Common.VeldridRender.ImageRender;
using Veldrid.SPIRV;

namespace Veldrid.Common.VeldridRender
{

    class UPOSpriteBatch : SpriteBatch<TextureWrapper>
    {
        private readonly GraphicsDevice _device;
        private readonly DeviceBuffer _vertexBuffer;
        private readonly Sampler _sampler;
        private Pipeline _pipeline;
        private readonly ResourceLayout[] _resourceLayouts;
        private readonly ResourceLayout _ColorLayout;
        private readonly ResourceSet _ColorSet;
        private Dictionary<Texture, (DeviceBuffer buffer, ResourceSet set, ResourceSet textureSet)> _buffers;

        public ResourceLayout[] ResourceLayouts => _resourceLayouts;
        /// <summary>
        /// Creates a new instance of <see cref="VeldridSpriteBatch"/>.
        /// </summary>
        /// <param name="device">The graphics device to create resources.</param>
        /// <param name="outputDescription">The output description of target framebuffer.</param>
        /// <param name="shaders">The shaders to use to render. Uses <seealso cref="LoadDefaultShaders(GraphicsDevice)"/> for default.</param>
        /// <param name="sampler">The samppler used to sample.</param>
        /// <param name="cullMode">Controls which face will be culled. By default the sprite are rendered with forward normal, negatives scales can flips that normal.</param>
        /// <param name="blendState">The blend state description for creating the pipeline.</param>
        /// <param name="depthStencil">The depth stencil state description for creating the pipeline.</param>
        public UPOSpriteBatch(GraphicsDevice device,
            OutputDescription outputDescription,
            Shader[] shaders,
            DeviceBuffer chinfobuffer,
            Sampler? sampler = null,
            FaceCullMode cullMode = FaceCullMode.None,
            BlendStateDescription? blendState = null,
            DepthStencilStateDescription? depthStencil = null) : base()
        {
            _sampler = sampler ?? device.Aniso4xSampler;
            _device = device;
            _vertexBuffer = CreateVertexBuffer(device);
            _resourceLayouts = CreateResourceLayouts(device);
            _ColorLayout = device.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ChInfoBuffer", ResourceKind.UniformBuffer, ShaderStages.Fragment)));
            _ColorSet = device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_ColorLayout,chinfobuffer));
            _buffers = new Dictionary<Texture, (DeviceBuffer buffer, ResourceSet set, ResourceSet textureSet)>();

            var bs = blendState ?? BlendStateDescription.SingleAlphaBlend;
            var ds = depthStencil ?? DepthStencilStateDescription.DepthOnlyLessEqual;
            _pipeline = CreatePipeline(device, outputDescription, cullMode, bs, ds, shaders, new ResourceLayout[] { _resourceLayouts[0], _resourceLayouts[1], _ColorLayout });
        }

        /// <summary>
        /// Draw this branch into a <see cref="CommandList"/>.
        /// Call this after calling <see cref="SpriteBatch{TTexture}.End"/>
        /// </summary>
        /// <param name="commandList"></param>
        public unsafe void DrawBatch(CommandList commandList)
        {
            DrawBatch(commandList, _pipeline);
        }

        public unsafe void DrawBatch(CommandList commandList, Pipeline pipeline)
        {
            var matrixSize = (int)Marshal.SizeOf<Matrix4x4>();
            commandList.SetPipeline(pipeline);
            foreach (var item in _batcher)
            {
                var texture = item.Key;
                var group = item.Value;

                var pair = GetBuffer(texture, group.Count + matrixSize);
                if (pair.buffer.IsDisposed) continue;
                var val = group.GetSpan();
                var mapped = _device.Map(pair.buffer, MapMode.Write);
                Unsafe.Write(mapped.Data.ToPointer(), ViewMatrix);
                fixed (void* ptr = &val[0])
                {
                    Buffer.MemoryCopy(ptr, (byte*)mapped.Data.ToPointer() + matrixSize, val.Length * Marshal.SizeOf<BatchItem>(), val.Length * Marshal.SizeOf<BatchItem>());
                }

                if (pair.buffer.IsDisposed) continue;
                _device.Unmap(pair.buffer);

                commandList.SetVertexBuffer(0, _vertexBuffer);
                commandList.SetGraphicsResourceSet(0, pair.set);
                commandList.SetGraphicsResourceSet(1, pair.textureSet);
                commandList.SetGraphicsResourceSet(2, _ColorSet);
                commandList.Draw(4, (uint)group.Count, 0, 0);
            }
        }
        internal (DeviceBuffer buffer, ResourceSet set, ResourceSet textureSet) GetBuffer(Texture t, int count)
        {
            var structSize = (uint)Marshal.SizeOf<BatchItem>();
            var size = (uint)(Math.Ceiling(count / 128f + 1) * 256 * structSize);
            var bci = new BufferDescription((uint)(size), BufferUsage.StructuredBufferReadOnly | BufferUsage.Dynamic, structSize);

            if (!_buffers.TryGetValue(t, out var pair))
            {
                pair.buffer = _device.ResourceFactory.CreateBuffer(bci);
                pair.set = _device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_resourceLayouts[0], pair.buffer));
                pair.textureSet = _device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_resourceLayouts[1], t, _sampler));
                _buffers[t] = pair;
            }
            else if (size > pair.buffer.SizeInBytes)
            {
                pair.set.Dispose();
                pair.buffer.Dispose();

                pair.buffer = _device.ResourceFactory.CreateBuffer(bci);
                pair.set = _device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_resourceLayouts[0], pair.buffer));
                pair.textureSet = _device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_resourceLayouts[1], t, _sampler));
                _buffers[t] = pair;
            }

            return pair;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _ColorLayout?.Dispose();
                _ColorSet?.Dispose();
                _vertexBuffer.Dispose();
                _pipeline?.Dispose();
                for (int i = 0; i < _resourceLayouts.Length; i++)
                {
                    _resourceLayouts[i]?.Dispose();
                }
                _buffers.Keys.ToList().ForEach(x => x?.Dispose());
                _buffers.Values.ToList().ForEach(x =>
                {
                    x.textureSet?.Dispose();
                    x.buffer?.Dispose();
                    x.set?.Dispose();
                });
            }
        }

        private static DeviceBuffer CreateVertexBuffer(GraphicsDevice device)
        {
            var bd = new BufferDescription()
            {
                SizeInBytes = 4 * (uint)Marshal.SizeOf<Vector2>(),
                Usage = BufferUsage.VertexBuffer,
            };

            var buffer = device.ResourceFactory.CreateBuffer(bd);

            var vertices = new Vector2[]
            {
                new Vector2( 0,  0),
                new Vector2( 1,  0),
                new Vector2( 0,  1),
                new Vector2( 1,  1)
            };

            device.UpdateBuffer(buffer, 0, vertices);

            return buffer;
        }

        private static ResourceLayout[] CreateResourceLayouts(GraphicsDevice device)
        {
            var layouts = new ResourceLayout[2];

            var elements = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("Items", ResourceKind.StructuredBufferReadOnly, ShaderStages.Vertex),
            };
            var rld = new ResourceLayoutDescription(elements);

            layouts[0] = device.ResourceFactory.CreateResourceLayout(rld);

            elements = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("Tex", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("Sampler", ResourceKind.Sampler, ShaderStages.Fragment)
            };
            rld = new ResourceLayoutDescription(elements);

            layouts[1] = device.ResourceFactory.CreateResourceLayout(rld);
            return layouts;
        }

        private static Pipeline CreatePipeline(GraphicsDevice device,
            OutputDescription outputDescription,
            FaceCullMode cullMode,
            BlendStateDescription blendState,
            DepthStencilStateDescription depthStencil,
            Shader[] shaders,
            params ResourceLayout[] layouts)
        {
            var vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));

            var pipelineDescription = new GraphicsPipelineDescription
            {
                BlendState = blendState,
                DepthStencilState = depthStencil,
                RasterizerState = RasterizerStateDescription.Default,
                PrimitiveTopology = PrimitiveTopology.TriangleStrip,
                ResourceLayouts = layouts,
                ShaderSet = new ShaderSetDescription(
                    vertexLayouts: new[] { vertexLayout },
                    shaders: shaders,
                    specializations: new[] { new SpecializationConstant(0, device.IsClipSpaceYInverted) }),
                Outputs = outputDescription,
            };

            return device.ResourceFactory.CreateGraphicsPipeline(pipelineDescription);
        }


    }
}
