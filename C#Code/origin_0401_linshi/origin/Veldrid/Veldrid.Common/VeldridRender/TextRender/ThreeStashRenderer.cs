using FontStashSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vulkan;
using System.Runtime.CompilerServices;
using Veldrid.Sdl2;
using System.Numerics;
using System.Drawing;

namespace Veldrid.Common.VeldridRender.ImageRender
{
    
    internal class ThreeStashRenderer : IFontStashRenderer2,IDisposable
    {
        GraphicsDevice graphicsDevice;
        private const int MAX_SPRITES = 256;
        private const int MAX_VERTICES = MAX_SPRITES * 4;
        private const int MAX_INDICES = MAX_SPRITES * 6;
        private VertexPositionColorTexture[] _vertexData = new VertexPositionColorTexture[MAX_VERTICES];
        private Shader[] shaders;
        private CommandList _commandList;
        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _indexBuffer;
        private DeviceBuffer _matrixBuffer;
        private ResourceLayout _matrixLayout;
        private ResourceSet _matrixSet;
        private ResourceLayout _textureLayout;
        private ResourceSet _textureSet;
        private Pipeline _pipeline;
        private System.Numerics.Matrix4x4 Matrix { get; set; }
        private ushort[] _indexData;
        private Veldrid.Texture _lastTexture;
        private int _vertexIndex = 0;
        public ThreeStashRenderer(GraphicsDevice device,CommandList commandList, Shader[] textshader)
        {
            graphicsDevice = device;
            _commandList = commandList;
            Matrix = System.Numerics.Matrix4x4.CreateOrthographicOffCenter(0, 1024, 1024, 0, 0, -1);
            _indexData = GenerateIndexArray();
            _vertexBuffer = graphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription((UInt32)(Unsafe.SizeOf<VertexPositionColorTexture>() * _vertexData.Length), BufferUsage.VertexBuffer));
            _indexBuffer = graphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription((UInt32)(Unsafe.SizeOf<short>() * _indexData.Length), BufferUsage.IndexBuffer));

            shaders = textshader;
            var vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("a_position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                new VertexElementDescription("a_color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
                new VertexElementDescription("a_texCoords0", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));
            _matrixBuffer = graphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription((UInt32)(Unsafe.SizeOf<System.Numerics.Matrix4x4>()), BufferUsage.UniformBuffer));


            _matrixLayout = graphicsDevice.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(new ResourceLayoutElementDescription("a_MatrixTransform", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
            _matrixSet = graphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_matrixLayout, _matrixBuffer));


            _textureLayout = graphicsDevice.ResourceFactory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Texture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("TextureSampler", ResourceKind.Sampler, ShaderStages.Fragment)
                    ));
           
            texture2DManager = new StashTextureManger(graphicsDevice);
            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();

            pipelineDescription.BlendState = BlendStateDescription.SingleAlphaBlend;
            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.GreaterEqual);
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);
            pipelineDescription.PrimitiveTopology =  PrimitiveTopology.TriangleList;
            pipelineDescription.ResourceLayouts = new ResourceLayout[] {_matrixLayout,_textureLayout };
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] {vertexLayout },
                shaders: shaders,
                specializations: new[] { new SpecializationConstant(0,graphicsDevice.IsClipSpaceYInverted) });
            pipelineDescription.Outputs = graphicsDevice.SwapchainFramebuffer.OutputDescription;
            _pipeline = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(pipelineDescription);

        }
        
        private ushort[] GenerateIndexArray()
        {
            ushort[] result = new ushort[MAX_INDICES];
            for (int i = 0, j = 0; i < MAX_INDICES; i += 6, j += 4)
            {
                result[i] = (ushort)(j);
                result[i + 1] = (ushort)(j + 1);
                result[i + 2] = (ushort)(j + 2);
                result[i + 3] = (ushort)(j + 3);
                result[i + 4] = (ushort)(j + 2);
                result[i + 5] = (ushort)(j + 1);
            }
            return result;
        }
        public void Begin()
        {
        }
        private ITexture2DManager texture2DManager;
        private bool disposedValue;
        internal Texture Texture => _lastTexture;
        ITexture2DManager IFontStashRenderer2.TextureManager => texture2DManager;

        unsafe void IFontStashRenderer2.DrawQuad(Veldrid.Texture texture, ref VertexPositionColorTexture topLeft, ref VertexPositionColorTexture topRight, ref VertexPositionColorTexture bottomLeft, ref VertexPositionColorTexture bottomRight)
        {
            if (_lastTexture != texture)
            {
                FlushBuffer();
            }

            _vertexData[_vertexIndex++] = topLeft;
            _vertexData[_vertexIndex++] = topRight;
            _vertexData[_vertexIndex++] = bottomLeft;
            _vertexData[_vertexIndex++] = bottomRight;
            _lastTexture = texture;
        }
        private unsafe void FlushBuffer()
        {
            if(_vertexIndex ==0|| _lastTexture ==null)
            {
                return;
            }

            if(_textureSet ==null)
            {
                _textureSet = graphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_textureLayout, (Texture)_lastTexture, graphicsDevice.LinearSampler));
            }
            //

            uint count = (UInt32)_vertexIndex * 6 / 4;
            for(int i=0;i<_vertexData.Length;i++)
            {
                _vertexData[i].Position.X *= 0.01f;
                _vertexData[i].Position.Y *= 0.01f;
            }
            _commandList.UpdateBuffer(_matrixBuffer, 0, Matrix);
            _commandList.UpdateBuffer(_indexBuffer, 0, _indexData);
            _commandList.UpdateBuffer(_vertexBuffer, 0, _vertexData);
            _commandList.SetPipeline(_pipeline);
            _commandList.SetVertexBuffer(0, _vertexBuffer);
            _commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            _commandList.SetGraphicsResourceSet(0, _matrixSet);
            _commandList.SetGraphicsResourceSet(1, _textureSet);
            _commandList.DrawIndexed(count, 1, 0, 0, 0);
            _vertexIndex = 0;
        }
        public SizeF BackColorFixedSize { get; set; } = SizeF.Empty;
        public void End()
        {
            Matrix =Matrix4x4.CreateOrthographicOffCenter(0,graphicsDevice.MainSwapchain.Framebuffer.Width,graphicsDevice.MainSwapchain.Framebuffer.Height,0,0,-1);
            FlushBuffer();
        }
        public void End(Matrix4x4 matrix)
        {
            Matrix= matrix;
            FlushBuffer();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _indexData = new UInt16[0];
                    _vertexData = new VertexPositionColorTexture[0];
                    //foreach (var val in shaders) val?.Dispose();
                    _vertexBuffer?.Dispose();
                    _indexBuffer?.Dispose();
                    (_lastTexture as Texture)?.Dispose();
                    _matrixBuffer?.Dispose();
                    _matrixSet?.Dispose();
                    _pipeline?.Dispose();
                    _textureLayout?.Dispose();
                    _textureSet?.Dispose();
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~StashRenderer()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
