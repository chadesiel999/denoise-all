using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Veldrid.Windows.ImageRendering;
using Microsoft.VisualBasic.ApplicationServices;

namespace Veldrid.Windows.LinesRenderering
{
    /*
     * 绘制流程为
     * 1、将单帧波形绘制到frameTexture中，此时忽略像素点亮度叠加功能
     * 2、将单帧波形绘制到mutliframeTexture中，此时需要开启像素点亮度叠加功能
     * 3、将mutliframeTexture绘制到主显示缓存中
     * 4、因此，在第2步中完成无限余晖效果，在第3步完成色温效果
     */

    internal class LinesRenderer_Double : BaseVeldridPlot<float>
    {
        private readonly string OffsetBufferName = "OffsetBuffer";
        private readonly string ColorLayoutName = "ColorLayout";
        private readonly string ColorSetName = "ColorSet";
        private object locker = new object();
        VertexLayoutDescription vertexLayout;
        private Boolean clearCache = false;
        DeviceBuffer offsetBuffer;
        ResourceSet colorSet;
        Veldrid.Framebuffer framebuffer;
        Veldrid.Texture frameTexture;

        Framebuffer mutliframebuffer;
        Texture mutliframeTexture;

        private InfiniteAfterglowRender infiniterender;
        private VeldridSpriteBatch spriteBatch;
        public uint LineCount { get; private set; } = 10_000;
        public LinesRenderer_Double(IVeldridControl device, uint lineCount = 10000) : base(device)
        {
            LineCount = lineCount;
            if (LineCount == 0)

            {
                LineCount = 2;
            }
            Points = new float[lineCount];
            

            infiniterender = new InfiniteAfterglowRender(
                device.GraphicsManger.Device,
                GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
                device.GraphicsManger.ShaderManger.GetShaders("InfiniteAfterglow"),
                GraphicsDevice.PointSampler,
                FaceCullMode.None,
                BlendStateDescription.SingleAlphaBlend);
        }
        protected override void OnPointCountChanged(uint count)
        {
            LineCount = count;
            if (LineCount == 0)

            {
                LineCount = 2;
            }
            base.OnPointCountChanged(count);
        }
        private BlendStateDescription _CacheBlend = new BlendStateDescription()
        {
            AlphaToCoverageEnabled =false,
            BlendFactor = RgbaFloat.Clear,
            AttachmentStates = new BlendAttachmentDescription[] 
            {
                new BlendAttachmentDescription()
                {
                    BlendEnabled = true,
                    SourceColorFactor = BlendFactor.SourceAlpha,
                    DestinationColorFactor = BlendFactor.One,
                    ColorFunction = BlendFunction.Maximum,
                    SourceAlphaFactor = BlendFactor.SourceAlpha,
                    DestinationAlphaFactor = BlendFactor.One,
                    AlphaFunction = BlendFunction.Add,
                }
            }
        };
        private BlendStateDescription _NomalBlend = new BlendStateDescription()
        {
            AlphaToCoverageEnabled = false,
            BlendFactor = RgbaFloat.Clear,
            AttachmentStates = new BlendAttachmentDescription[]
            {
                new BlendAttachmentDescription()
                {
                    AlphaFunction = BlendFunction.Maximum,
                    ColorFunction = BlendFunction.Maximum,
                    BlendEnabled = true,
                    SourceAlphaFactor= BlendFactor.Zero,
                    DestinationAlphaFactor = BlendFactor.DestinationAlpha,
                    SourceColorFactor= BlendFactor.Zero,
                    DestinationColorFactor = BlendFactor.DestinationColor,
                }
            }
        };

        public override void CreateResources()
        {
            base.CreateResources();

            offsetBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<LinesOffsetInfo>(), BufferUsage.UniformBuffer));
            SetGraphicsResource(OffsetBufferName, offsetBuffer);

            CreateVertexBuffer(Points);
            CreateShader("LineSingle");
            vertexLayout = new VertexLayoutDescription(new VertexElementDescription("in_Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float1));
            var elments = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("in_Color", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("in_WindowsInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("in_LinesOffsetInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            };

            ResourceLayout colorLayout = ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(elments));
            colorSet = ResourceFactory.CreateResourceSet(new ResourceSetDescription(colorLayout, GetGraphicsResource<DeviceBuffer>(LineColorBufferName), GetGraphicsResource<DeviceBuffer>(WindowSizeBufferName), offsetBuffer));
            SetGraphicsResource(ColorLayoutName, colorLayout);
            SetGraphicsResource(ColorSetName, colorSet);
            CreatePipLine(lineType,colorLayout , vertexLayout , Shaders,_NomalBlend);




            GraphicsDevice.UpdateBuffer(offsetBuffer, 0, new LinesOffsetInfo(SampleRate));
        }


        internal override void PreDraw()
        {
            if (frameTexture == null || frameTexture.Width != GraphicsDevice.SwapchainFramebuffer.Width || frameTexture.Height != GraphicsDevice.SwapchainFramebuffer.Height)
            {
                frameTexture?.Dispose();
                var texturedesc = TextureDescription.Texture2D(GraphicsDevice.SwapchainFramebuffer.Width, GraphicsDevice.SwapchainFramebuffer.Height, 1, 1, GraphicsDevice.SwapchainFramebuffer.ColorTargets[0].Target.Format, TextureUsage.RenderTarget | TextureUsage.Sampled);
                frameTexture = ResourceFactory.CreateTexture(texturedesc);
                framebuffer?.Dispose();
                framebuffer = ResourceFactory.CreateFramebuffer(new FramebufferDescription(null, frameTexture));

                mutliframeTexture?.Dispose();
                mutliframeTexture = ResourceFactory.CreateTexture(texturedesc);
                mutliframebuffer?.Dispose();
                mutliframebuffer = ResourceFactory.CreateFramebuffer(new FramebufferDescription(null, mutliframeTexture));

                if(spriteBatch ==null)
                {
                    spriteBatch = new VeldridSpriteBatch(GraphicsDevice,
                        mutliframebuffer.OutputDescription,
                        VeldridControl.GraphicsManger.ShaderManger.GetShaders("ImageRender"),
                        GraphicsDevice.PointSampler,
                        FaceCullMode.None,
                        _CacheBlend);
                }
            }
            base.PreDraw();
            if (PointCountChanged)
            {
                CreateVertexBuffer(Points);
                GraphicsDevice.UpdateBuffer(offsetBuffer, 0, new LinesOffsetInfo(SampleRate));
                PointCountChanged = false;
            }

        }


        internal override void DrawData()
        {
            base.DrawData();
            if (this[nameof(LineType)])
            {
                CreatePipLine(LineType, GetGraphicsResource<ResourceLayout>(ColorLayoutName), vertexLayout, Shaders, _NomalBlend);
                this[nameof(LineType)] = false;
                this[nameof(LineColor)] = true;
            }

            if (this[nameof(SampleRate), nameof(Brightness)])
            {
                GraphicsDevice.UpdateBuffer(offsetBuffer, 0, new LinesOffsetInfo(SampleRate, Brightness));
                this[nameof(SampleRate), nameof(Brightness)] = false;
            }
            if (this[nameof(Points)])
            {
                GraphicsDevice.UpdateBuffer(VertexBuffer, 0, Points);
                this[nameof(Points)] = false;
            }

            bool tempclearcache = clearCache || !UseCache;
            UInt32 tempframecount = FramePointCount;
            Int32 count = (Int32)MathF.Ceiling(LineCount / (float)tempframecount);

            for (uint index = 0; index < count; index++)
            {
                if (index != 0) CommandList.Begin();
                CommandList.SetFramebuffer(framebuffer);
                CommandList.ClearColorTarget(0, RgbaFloat.Clear);
                CommandList.SetVertexBuffer(0, VertexBuffer);
                CommandList.SetPipeline(Pipeline);
                CommandList.SetGraphicsResourceSet(0, colorSet);
                if (index < count - 1)
                {
                    CommandList.Draw(tempframecount, 1, tempframecount * index, 0);
                }
                else
                {
                    CommandList.Draw(LineCount - tempframecount * index, 1, tempframecount * index, 0);
                }
                CommandList.End();
                GraphicsDevice.SubmitCommands(CommandList);


                spriteBatch.Begin();
                spriteBatch.ViewMatrix = Matrix4x4.CreateOrthographic(WindowSize.Width, WindowSize.Height, 0.01f, -100f);
                spriteBatch.Draw(frameTexture, new RectangleF(frameTexture.Width / -2, frameTexture.Height / -2, frameTexture.Width, frameTexture.Height), Color.Empty, 2);
                spriteBatch.End();

                CommandList.Begin();
                CommandList.SetFramebuffer(mutliframebuffer);
                if(tempclearcache)
                {
                    CommandList.ClearColorTarget(0, RgbaFloat.Clear);
                    clearCache = false;
                }
                spriteBatch.DrawBatch(CommandList);
                CommandList.End();
                GraphicsDevice.SubmitCommands(CommandList);

            }


            infiniterender.Begin();
            infiniterender.ViewMatrix = spriteBatch.ViewMatrix;
            infiniterender.Draw(mutliframeTexture, new RectangleF(mutliframeTexture.Width / -2, mutliframeTexture.Height / -2, mutliframeTexture.Width, mutliframeTexture.Height), Color.Empty, 2);
            infiniterender.End();
            
            CommandList.Begin();
            CommandList.SetFramebuffer(GraphicsDevice.MainSwapchain.Framebuffer);
            infiniterender.DrawBatch(CommandList);
            CommandList.End();
            GraphicsDevice.SubmitCommands(CommandList);

        }
        public Texture LinesCacheBuffer => mutliframeTexture;
        public void ClearCache()
        {
            lock (locker)
            {
                clearCache = true;
            }
        }

        private PrimitiveTopology lineType = PrimitiveTopology.LineStrip;

        public PrimitiveTopology LineType
        {
            get { return lineType; }
            set { Set(ref lineType, value); }
        }
        public UInt32 FramePointCount { get; set; } = 10000;
        private float sampleRate = 1;
        private bool useCache = false;

        public float SampleRate
        {
            get => sampleRate;
            set
            {
                if (value <= 0) return;
                Set(ref sampleRate, value);
            }
        }

        private Single brightness =100;

        public Single Brightness
        {
            get { return brightness; }
            set {Set(ref brightness,value); }
        }


        public Boolean UseCache 
        { 
            get => useCache;
            set
            {
                if (value != useCache)
                {
                    this[nameof(LineType)] = true;
                    useCache = value;
                    if(value)
                    {
                        clearCache = true;
                    }
                }
            }
        }
        public override void DisposeResources()
        {
            base.DisposeResources();
            framebuffer?.Dispose();
            frameTexture?.Dispose();
            mutliframebuffer?.Dispose();
            mutliframeTexture?.Dispose();
            spriteBatch?.Dispose();
            infiniterender?.Dispose();
        }
    }
}
