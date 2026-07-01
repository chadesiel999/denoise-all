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
    internal class StashTextureManger : ITexture2DManager
    {
        Texture lastTexure;
        private GraphicsDevice _Graphics;
        public StashTextureManger(GraphicsDevice graphics)
        {
            _Graphics = graphics;
        }
        public Veldrid.Texture CreateTexture(int width, int height)
        {
            if (lastTexure == null || lastTexure.Width != width || lastTexure.Height != height)
            {
                lastTexure?.Dispose();
                lastTexure = _Graphics.ResourceFactory.CreateTexture(new TextureDescription((uint)width, (uint)height, 1, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Sampled, TextureType.Texture2D, TextureSampleCount.Count1));
            }
            return lastTexure;
        }

        public Size GetTextureSize(Veldrid.Texture texture)
        {
            var t = (Texture)texture;
            return new Size((Int32)t.Width, (Int32)t.Height);
        }

        public void SetTextureData(Veldrid.Texture texture, Rectangle bounds, byte[] data)
        {
            if(texture is Texture t)
            {
                _Graphics.UpdateTexture(t, data, (UInt32)bounds.X, (UInt32)bounds.Y, 0, (UInt32)bounds.Width, (UInt32)bounds.Height, 1, 0, 0);
            }
        }
    }
    
    internal class StashRenderer : IFontStashRenderer2,IDisposable
    {
        GraphicsDevice graphicsDevice;
        private const int MAX_SPRITES = 256;
        private const int MAX_VERTICES = MAX_SPRITES * 4;
        private const int MAX_INDICES = MAX_SPRITES * 6;
        private VertexPositionColorTexture[] _vertexData = new VertexPositionColorTexture[MAX_VERTICES];
        private Shader[] shaders;
        private CommandList _commandList;
        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _backColorBuffer;
        private DeviceBuffer _indexBuffer;
        private DeviceBuffer _matrixBuffer;
        private ResourceLayout _matrixLayout;
        private ResourceLayout _backColotLayout;
        private ResourceSet _matrixSet;
        private ResourceSet _backColorSet;
        private ResourceLayout _textureLayout;
        private ResourceSet _textureSet;
        private Pipeline _pipeline;
        private Pipeline _backColorPipline;
        private System.Numerics.Matrix4x4 Matrix { get; set; }
        private ushort[] _indexData;
        private Veldrid.Texture _lastTexture;
        private int _vertexIndex = 0;
        public StashRenderer(GraphicsDevice device,CommandList commandList, Shader[] textshader, Shader[] backColorshader,BlendStateDescription textBlend, BlendStateDescription backblendState)
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

            _backColorBuffer = graphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<RgbaFloat>(), BufferUsage.UniformBuffer));
            _backColotLayout = graphicsDevice.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(new ResourceLayoutElementDescription("a_color", ResourceKind.UniformBuffer, ShaderStages.Fragment)));
            _backColorSet = graphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_backColotLayout, _backColorBuffer));
            _textureLayout = graphicsDevice.ResourceFactory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Texture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("TextureSampler", ResourceKind.Sampler, ShaderStages.Fragment)
                    ));
           
            texture2DManager = new StashTextureManger(graphicsDevice);
            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();

            pipelineDescription.BlendState = textBlend;
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
            _backColorPipline = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(new GraphicsPipelineDescription()
            {
                BlendState = backblendState,
                DepthStencilState = new DepthStencilStateDescription()
                {
                    DepthTestEnabled= true,
                    DepthWriteEnabled= true,
                    DepthComparison = ComparisonKind.LessEqual
                },
                RasterizerState = new RasterizerStateDescription()
                {
                    CullMode = FaceCullMode.Back,
                    FillMode = PolygonFillMode.Solid,
                    FrontFace = FrontFace.Clockwise,
                    DepthClipEnabled= true,
                    ScissorTestEnabled = false,
                },
                PrimitiveTopology= PrimitiveTopology.TriangleStrip,
                ResourceLayouts = new ResourceLayout[] {_matrixLayout,_backColotLayout },
                ShaderSet = new ShaderSetDescription()
                {
                    Shaders = backColorshader,
                    Specializations = new[] {new SpecializationConstant(0,graphicsDevice.IsClipSpaceYInverted) },
                    VertexLayouts = new VertexLayoutDescription[] { vertexLayout},
                },
                Outputs = graphicsDevice.SwapchainFramebuffer.OutputDescription,
            });

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
        public RgbaFloat BackColor { get; set; } = RgbaFloat.Clear;
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
            if (BackColor.A > 0)
            {
                _commandList.UpdateBuffer(_backColorBuffer, 0, BackColor);
                var rectdata = _indexData.Take((int)count).Distinct().Select(x => _vertexData[x]);
                RectangleF rec = new RectangleF();
                rec.X = rectdata.Min(x => x.Position.X) - 2;
                rec.Y = rectdata.Min(y => y.Position.Y) - 2;
                rec.Width = rectdata.Max(x => x.Position.X) - rec.X + 2;
                rec.Height = rectdata.Max(y => y.Position.Y) - rec.Y + 2;
                if (BackColorFixedSize.Width > 0 && BackColorFixedSize.Height > 0)
                {
                    rec.Width = BackColorFixedSize.Width;
                    rec.Height = BackColorFixedSize.Height;
                }
                VertexPositionColorTexture[] temp = new VertexPositionColorTexture[4];
                temp[0].Position = new Vector3(rec.X, rec.Y, _vertexData[0].Position.Z);
                temp[1].Position = new Vector3(rec.X + rec.Width, rec.Y, _vertexData[0].Position.Z);
                temp[2].Position = new Vector3(rec.X, rec.Y + rec.Height, _vertexData[0].Position.Z);
                temp[3].Position = new Vector3(rec.X + rec.Width, rec.Y + rec.Height, _vertexData[0].Position.Z);
                _commandList.UpdateBuffer(_vertexBuffer, 0, temp);
                //_commandList.UpdateBuffer(_indexBuffer, 0, backcolorindex);
                _commandList.SetPipeline(_backColorPipline);
                _commandList.SetVertexBuffer(0, _vertexBuffer);
                //_commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
                _commandList.SetGraphicsResourceSet(0, _matrixSet);
                _commandList.SetGraphicsResourceSet(1, _backColorSet);

                _commandList.Draw(4);

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
                    _backColorBuffer?.Dispose();
                    _backColorPipline?.Dispose();
                    _backColorSet?.Dispose();
                    _backColotLayout?.Dispose();
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
