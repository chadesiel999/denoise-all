using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Common.VeldridRender.ImageRender;
using Vulkan;

namespace Veldrid.Common.VeldridRender.LineRender
{

    internal class InfiniteAfterglowRender : SpriteBatch<TextureWrapper>
    {
        private readonly GraphicsDevice _device;
        private readonly DeviceBuffer _vertexBuffer;
        private readonly Sampler _sampler;
        private Pipeline _pipeline;
        private readonly ResourceLayout[] _resourceLayouts;
        private bool colourTemperature = false;
        private Dictionary<Texture, (DeviceBuffer buffer, DeviceBuffer[] colorBuffer, ResourceSet set, ResourceSet textureSet, ResourceSet colorSet)> _buffers;
        public Boolean ColourTemperature
        {
            get => colourTemperature;
            set
            {
                if (value != colourTemperature)
                {
                    colourTemperature = value;
                    Color_PropertyChanged(null, EventArgs.Empty);
                }
            }
        }
        public ColourTemperatureInfo Color1 { get; } = new ColourTemperatureInfo();
        public ColourTemperatureInfo Color2 { get; } = new ColourTemperatureInfo();
        public ColourTemperatureInfo Color3 { get; } = new ColourTemperatureInfo();
        public ColourTemperatureInfo Color4 { get; } = new ColourTemperatureInfo();
        private object locker = new object();
        private bool upcolor = true;

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
        public InfiniteAfterglowRender(GraphicsDevice device,
            OutputDescription outputDescription,
            Shader[] shaders,
            Sampler? sampler = null,
            FaceCullMode cullMode = FaceCullMode.None,
            BlendStateDescription? blendState = null,
            DepthStencilStateDescription? depthStencil = null) : base()
        {
            _device = device;
            Color1.Transparency = 0;
            Color2.Transparency = 0.4f;
            Color3.Transparency = 0.7f;
            Color4.Transparency = 0.95f;
            Color1.Color = System.Drawing.Color.Empty;
            Color2.Color = System.Drawing.Color.Green;
            Color3.Color = System.Drawing.Color.Blue;
            Color4.Color = System.Drawing.Color.Red;
            Color1.PropertyChanged += Color_PropertyChanged;
            Color2.PropertyChanged += Color_PropertyChanged;
            Color3.PropertyChanged += Color_PropertyChanged;
            Color4.PropertyChanged += Color_PropertyChanged;
            _sampler = sampler ?? device.LinearSampler;
            _vertexBuffer = CreateVertexBuffer(device);

            _resourceLayouts = CreateResourceLayouts(device);
            _buffers = new();

            var bs = blendState ?? BlendStateDescription.SingleAlphaBlend;
            var ds = depthStencil ?? new(
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.LessEqual);
            _pipeline = CreatePipeline(device, outputDescription, cullMode, bs, ds, shaders, _resourceLayouts);
        }
        private void Color_PropertyChanged(object sender, EventArgs e)
        {
            lock (locker)
            {
                upcolor = true;
            }
        }

        private DeviceBuffer[] CreateColorBuffer(GraphicsDevice device)
        {
            DeviceBuffer[] buffers = new DeviceBuffer[2];
            buffers[0] = device.ResourceFactory.CreateBuffer(new BufferDescription((uint)Marshal.SizeOf<Vector4>(), BufferUsage.UniformBuffer));
            device.UpdateBuffer(buffers[0], 0, new Vector4(Convert.ToSingle(ColourTemperature)));
            buffers[1] = device.ResourceFactory.CreateBuffer(new BufferDescription((UInt32)Marshal.SizeOf<Vector4>()*5, BufferUsage.UniformBuffer));
            device.UpdateBuffer(buffers[1], 0, GetInfiniteInfos());
            return buffers;
        }
        private Vector4[] GetInfiniteInfos()
        {
            Vector4[] vector4s = new Vector4[5];
            vector4s[0] = new Vector4(Color1.Transparency, Color2.Transparency, Color3.Transparency, Color4.Transparency);
            vector4s[1] = Color1.GetColorInfo().Color;
            vector4s[2] = Color2.GetColorInfo().Color;
            vector4s[3] = Color3.GetColorInfo().Color;
            vector4s[4] = Color4.GetColorInfo().Color;
            return vector4s;
        }
        /// <summary>
        /// Draw this branch into a <see cref="CommandList"/>.
        /// Call this after calling <see cref="SpriteBatch{TTexture}.End"/>
        /// </summary>
        /// <param name="commandList"></param>
        public unsafe void DrawBatch(CommandList commandList)
        {
            var matrixSize = (int)Marshal.SizeOf<Matrix4x4>();
            commandList.SetPipeline(_pipeline);
            foreach (var item in _batcher)
            {
                var texture = item.Key;
                var group = item.Value;

                var pair = GetBuffer(texture, 1);
                var mapped = _device.Map(pair.buffer, MapMode.Write);
                var val = group.GetSpan().ToArray();
                Unsafe.Write(mapped.Data.ToPointer(), ViewMatrix);

                for (int i = 0; i < val.Length; i++)
                {
                    Unsafe.Copy((void*)(mapped.Data.ToInt64() + matrixSize + Marshal.SizeOf<BatchItem>() * i), ref val[i]);
                }
                _device.Unmap(pair.buffer);
                lock (locker)
                {
                    if (upcolor)
                    {
                        _device.UpdateBuffer(pair.colorBuffer[0], 0, new Vector4(Convert.ToSingle(ColourTemperature)));
                        _device.UpdateBuffer(pair.colorBuffer[1], 0, GetInfiniteInfos());
                        upcolor = false;
                    }
                }

                commandList.SetVertexBuffer(0, _vertexBuffer);
                commandList.SetGraphicsResourceSet(0, pair.set);
                commandList.SetGraphicsResourceSet(1, pair.textureSet);
                commandList.SetGraphicsResourceSet(2, pair.colorSet);
                commandList.Draw(4, (uint)group.Count, 0, 0);
            }
        }

        internal (DeviceBuffer buffer, DeviceBuffer[] colorBuffer, ResourceSet set, ResourceSet textureSet, ResourceSet colorSet) GetBuffer(Texture t, int count)
        {
            var texture = t as Texture;
            var structSize = (uint)Marshal.SizeOf<BatchItem>();
            var size = ((count + 63) & (~63)) * structSize;
            var bci = new BufferDescription((uint)size, BufferUsage.StructuredBufferReadOnly | BufferUsage.Dynamic, structSize);

            if (!_buffers.TryGetValue(texture, out var pair))
            {
                pair.buffer = _device.ResourceFactory.CreateBuffer(bci);
                pair.colorBuffer = CreateColorBuffer(_device);
                pair.set = _device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_resourceLayouts[0], pair.buffer));
                pair.textureSet = _device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_resourceLayouts[1], texture, _sampler));
                pair.colorSet = _device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_resourceLayouts[2], pair.colorBuffer));
                _buffers[texture] = pair;
            }
            else if (size > pair.buffer.SizeInBytes)
            {
                pair.set.Dispose();
                pair.buffer.Dispose();
                pair.colorBuffer[0].Dispose();
                pair.colorBuffer[1].Dispose();

                pair.buffer = _device.ResourceFactory.CreateBuffer(bci);
                pair.colorBuffer = CreateColorBuffer(_device);
                pair.set = _device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_resourceLayouts[0], pair.buffer));
                pair.textureSet = _device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_resourceLayouts[1], texture, _sampler));
                pair.colorSet = _device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_resourceLayouts[2], pair.colorBuffer));
                _buffers[texture] = pair;
            }

            return pair;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _vertexBuffer.Dispose();
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
                new( 0,  0),
                new( 1,  0),
                new( 0,  1),
                new( 1,  1)
            };

            device.UpdateBuffer(buffer, 0, vertices);

            return buffer;
        }

        private static ResourceLayout[] CreateResourceLayouts(GraphicsDevice device)
        {
            var layouts = new ResourceLayout[3];

            var elements = new ResourceLayoutElementDescription[]
            {
                new("Items", ResourceKind.StructuredBufferReadOnly, ShaderStages.Vertex),
            };
            var rld = new ResourceLayoutDescription(elements);

            layouts[0] = device.ResourceFactory.CreateResourceLayout(rld);

            elements = new ResourceLayoutElementDescription[]
            {
                new("Tex", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new("Sampler", ResourceKind.Sampler, ShaderStages.Fragment)
            };
            rld = new ResourceLayoutDescription(elements);

            layouts[1] = device.ResourceFactory.CreateResourceLayout(rld);
            var status = new ResourceLayoutElementDescription[]
            {
                        new ResourceLayoutElementDescription("InfiniteInfo", ResourceKind.UniformBuffer, ShaderStages.Fragment),
                        new ResourceLayoutElementDescription("Colors",ResourceKind.UniformBuffer, ShaderStages.Fragment),
            };
            layouts[2] = device.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(status));
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
                RasterizerState = new(
                    cullMode: cullMode,
                    fillMode: PolygonFillMode.Solid,
                    frontFace: FrontFace.Clockwise,
                    depthClipEnabled: true,
                    scissorTestEnabled: false),
                PrimitiveTopology = PrimitiveTopology.TriangleStrip,
                ResourceLayouts = layouts,
                ShaderSet = new(
                    vertexLayouts: new[] { vertexLayout },
                    shaders: shaders,
                    specializations: new[] { new SpecializationConstant(0, device.IsClipSpaceYInverted) }),
                Outputs = outputDescription,
            };

            return device.ResourceFactory.CreateGraphicsPipeline(pipelineDescription);
        }


    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct InfiniteInfos
    {
        public ColorInfo Color1;
        public ColorInfo Color2;
        public ColorInfo Color3;
        public ColorInfo Color4;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ColorInfo
    {
        public float Transparency;
        public Vector4 Color;
        public ColorInfo()
        {
            Transparency = 0;
            Color = Vector4.Zero;
        }
        public ColorInfo(float transparency,Vector4 color)
        {
            Transparency = transparency;
            Color = color;
        }
    }
    public class ColourTemperatureInfo
    {
        internal event EventHandler PropertyChanged;
        private float transparency;
        private System.Drawing.Color color;
        private void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        public float Transparency 
        {
            get => transparency;
            set
            {
                if (value != transparency)
                {
                    transparency = value;
                    OnPropertyChanged();
                }
            }
        }
        public System.Drawing.Color Color 
        {
            get => color;
            set
            {
                if (value != color)
                {
                    color = value;
                    OnPropertyChanged();
                }
            }
        }
        internal ColorInfo GetColorInfo()
        {
            return new ColorInfo(Transparency, new System.Numerics.Vector4(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f));
        }
    }
}
