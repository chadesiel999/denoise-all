using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Veldrid.Common.Tools;
using Veldrid.Common.VeldridRender.ImageRender;

namespace Veldrid.Common.VeldridRender.LineRender
{
    internal class AnalogRender : BaseVeldridRender
    {
        private bool clearCache = false;
        private float colourTemperatureBrightness = 1f / 65535;
        private LineInfoStruct _LineInfo;
        #region PipLine
        Pipeline crossPipline;
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

        Framebuffer[] frameBuffer;
        Texture[] frameTexture;

        Framebuffer mutliFrameBuffer;
        Texture mutliTexture;
        private Color color;

        VeldridSpriteBatch spriteBatch;
        //VeldridSpriteBatch mainspriteBatch;

        #endregion


        private BlendStateDescription _CacheBlend = new BlendStateDescription()
        {
            AlphaToCoverageEnabled = false,
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
        private float valueScale = 1;
        private Boolean _UpdateLineInfo = false;

        public AnalogRender(IVeldridContent control, uint maxframeLenght = 10000, uint maxFrameCount = 1) : base(control)
        {
            Datas = new float[maxframeLenght * maxFrameCount];
            frameBuffer = new Framebuffer[maxFrameCount];
            frameTexture = new Texture[maxFrameCount];
            this._MaxFrameCount = maxFrameCount;
            this._MaxFrameLength = maxframeLenght;
            //DrawLength = maxframeLenght;
            _LineInfo.ValueScale = 1;
            _UpdateLineInfo = true;
        }
        public override void CreateResources()
        {
            base.CreateResources();
            vertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<float>() * MaxFrameCount * MaxFrameLength), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            vertexLayout = new VertexLayoutDescription(new VertexElementDescription("in_Position", VertexElementFormat.Float1, VertexElementSemantic.TextureCoordinate));
            shaders = CreateShader("AnalogRender");
            ResourceLayoutElementDescription[] resourceLayoutElementDescriptions ={
                new ResourceLayoutElementDescription("ProView", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("LineInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            };
            proviewBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>() * 2, BufferUsage.Dynamic | BufferUsage.UniformBuffer));
            lineInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<LineInfoStruct>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            ResourceLayoutDescription resourceLayoutDescription = new ResourceLayoutDescription(resourceLayoutElementDescriptions);
            sharedLayout = ResourceFactory.CreateResourceLayout(resourceLayoutDescription);
            BindableResource[] bindableResources = new BindableResource[] { proviewBuffer, lineInfoBuffer };
            ResourceSetDescription resourceSetDescription = new ResourceSetDescription(sharedLayout, bindableResources);
            sharedSet = ResourceFactory.CreateResourceSet(resourceSetDescription);
            spriteBatch = new VeldridSpriteBatch(GraphicsDevice, MainSwapchainBuffer.OutputDescription, CreateShader("ImageRender"), GraphicsDevice.PointSampler, FaceCullMode.None, _CacheBlend);
            //mainspriteBatch = new VeldridSpriteBatch(GraphicsDevice, MainSwapchainBuffer.OutputDescription, CreateShader("ImageRender"), GraphicsDevice.Aniso4xSampler, FaceCullMode.None, BlendStateDescription.SingleAlphaBlend);
            this[nameof(Range), nameof(Margin), nameof(Brightness), nameof(Color), nameof(HorizontalOffset), nameof(SampleRate), nameof(VerticalOffset), nameof(ValueScale)] = true;
            CreateFrameBuffer();

            CreatePipLine();
        }
        private void CreatePipLine()
        {
            linePipline = CreatePipLine(PrimitiveTopology.LineStrip, sharedLayout, vertexLayout, shaders, _NomalBlend);
            var shader = GetOtherShader("AnalogPointToCross.geometry.hlsl", ShaderStages.Geometry);
            crossPipline = CreatePipLine(PrimitiveTopology.PointList, sharedLayout, vertexLayout, new Shader[] { shaders[0], shaders[1], shader }, _NomalBlend);
            pointPipline = CreatePipLine(PrimitiveTopology.PointList, sharedLayout, vertexLayout, shaders, _NomalBlend);
            var tempvertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));
            spritepipline = CreatePipLine(PrimitiveTopology.TriangleStrip, spriteBatch.ResourceLayouts, new[] { tempvertexLayout }, CreateShader("ImageRender"), BlendStateDescription.SingleAlphaBlend);
        }
        private void CreateFrameBuffer()
        {
            var texturedesc = TextureDescription.Texture2D(MainSwapchainBuffer.Width, MainSwapchainBuffer.Height, 1, 1, MainSwapchainBuffer.ColorTargets[0].Target.Format, TextureUsage.RenderTarget | TextureUsage.Sampled);
            for (int i = 0; i < MaxFrameCount; i++)
            {
                if (frameBuffer[i] == null || frameBuffer[i].Width != MainSwapchainBuffer.Width || frameBuffer[i].Height != MainSwapchainBuffer.Height)
                {
                    frameTexture[i]?.Dispose();
                    frameBuffer[i]?.Dispose();
                    //if (MainSwapchainBuffer.ColorTargets[0].Target.IsDisposed) break;
                    frameTexture[i] = ResourceFactory.CreateTexture(texturedesc);
                    frameBuffer[i] = ResourceFactory.CreateFramebuffer(new FramebufferDescription(null, frameTexture[i]));
                }
            }
            if (mutliFrameBuffer == null || mutliFrameBuffer.Width != MainSwapchainBuffer.Width || mutliFrameBuffer.Height != MainSwapchainBuffer.Height)
            {
                mutliTexture?.Dispose();
                mutliFrameBuffer?.Dispose();
                //if (MainSwapchainBuffer.ColorTargets[0].Target.IsDisposed) return;
                mutliTexture = ResourceFactory.CreateTexture(texturedesc);
                mutliFrameBuffer = ResourceFactory.CreateFramebuffer(new FramebufferDescription(null, mutliTexture));
            }
        }
        public StackConfig StackConfig { get; } = new StackConfig();

        internal unsafe override void PreDraw()
        {
            if (proviewBuffer.IsDisposed || lineInfoBuffer.IsDisposed) return;
            if (WindowSizeState)
            {
                CreateFrameBuffer();
            }
            if (this[nameof(Margin), nameof(Range)] || WindowSizeState)
            {
                CommandList.UpdateBuffer(proviewBuffer, 0, Camera.OrthographicMatrix);
                CommandList.UpdateBuffer(proviewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>(), Camera.GetLineMatrix(Margin, Range));
                _LineInfo.LineRange = Range;
                _LineInfo.CrossSize = new Vector2(10f / MainSwapchainBuffer.Width, 10f / MainSwapchainBuffer.Height);
                _UpdateLineInfo = true;
                this[nameof(Range), nameof(Margin)] = false;
                if (WindowSizeState) WindowSizeState = false;
            }
            if (_UpdateLineInfo)
            {
                fixed (void* ptr = &_LineInfo)
                {
                    CommandList.UpdateBuffer(lineInfoBuffer, 0, (IntPtr)(ptr), (uint)Unsafe.SizeOf<LineInfoStruct>());
                    _UpdateLineInfo = false;
                }
            }

            base.PreDraw();
        }
        internal override void DrawData()
        {
            if (DrawLength == 0 || vertexBuffer.IsDisposed || proviewBuffer.IsDisposed || lineInfoBuffer.IsDisposed) return;
            var frmaelenght = DrawLength;
            var framecount = _DataCount / frmaelenght;
            Pipeline pipeline = PrimitiveTopology == PrimitiveTopology.LineStrip ? linePipline : (SampleRate > 0.1 ? pointPipline : crossPipline);
            //if (framecount == 0)
            //{
            //    CommandList.SetFramebuffer(MainSwapchainBuffer);
            //    CommandList.SetVertexBuffer(0, vertexBuffer);
            //    CommandList.SetPipeline(pipeline);
            //    CommandList.SetGraphicsResourceSet(0, sharedSet);
            //    CommandList.Draw((UInt32)_DataCount);
            //    return;
            //}
            bool tempclearcache = clearCache || !UseCache;
            Matrix4x4 matrix = Matrix4x4.CreateOrthographic(MainSwapchainBuffer.Width, MainSwapchainBuffer.Height, 0.01f, -100f);
            if (spriteBatch.BeginCalled)
            {
                spriteBatch.End();
            }
            if ((framecount == 1 || framecount == 0) && !UseCache)
            {
                if (mutliFrameBuffer == null || mutliFrameBuffer.IsDisposed) return;
                frmaelenght = Math.Min(frmaelenght, _DataCount);
                CommandList.SetFramebuffer(mutliFrameBuffer);
                CommandList.ClearColorTarget(0, RgbaFloat.Clear);
                CommandList.SetVertexBuffer(0, vertexBuffer);
                CommandList.SetPipeline(pipeline);
                CommandList.SetGraphicsResourceSet(0, sharedSet);
                CommandList.Draw((uint)frmaelenght, 1, (uint)SkipCount, 0);
                spriteBatch.Begin();
                spriteBatch.ViewMatrix = matrix;
                spriteBatch.Draw(mutliTexture, new RectangleF(MainSwapchainBuffer.Width / -2, MainSwapchainBuffer.Height / -2, MainSwapchainBuffer.Width, MainSwapchainBuffer.Height),
                    new RectangleF(0, 0, mutliFrameBuffer.Width, mutliFrameBuffer.Height), Color.Empty, 0);
                spriteBatch.End();
                CommandList.SetFramebuffer(MainSwapchainBuffer);
                spriteBatch.DrawBatch(CommandList, spritepipline);
            }
            else
            {
                if (framecount == 1 || framecount == 0)
                {
                    spriteBatch.Begin();
                    spriteBatch.ViewMatrix = matrix;

                    if (frameBuffer[0] == null || frameBuffer[0].IsDisposed) return;
                    frmaelenght = Math.Min(frmaelenght, _DataCount);

                    CommandList.SetFramebuffer(frameBuffer[0]);
                    CommandList.ClearColorTarget(0, RgbaFloat.Clear);
                    CommandList.SetVertexBuffer(0, vertexBuffer);
                    CommandList.SetPipeline(pipeline);
                    CommandList.SetGraphicsResourceSet(0, sharedSet);
                    CommandList.Draw((uint)frmaelenght, 1, (uint)SkipCount, 0);


                    spriteBatch.Draw(frameTexture[0], new RectangleF(frameTexture[0].Width / -2, frameTexture[0].Height / -2, frameTexture[0].Width, frameTexture[0].Height), Color.Empty, 2);
                    spriteBatch.End();
                    CommandList.SetFramebuffer(mutliFrameBuffer);
                    if (tempclearcache)
                    {
                        CommandList.ClearColorTarget(0, RgbaFloat.Clear);
                        clearCache = false;
                    }
                    spriteBatch.DrawBatch(CommandList);

                    spriteBatch.Begin();
                    spriteBatch.ViewMatrix = matrix;
                    spriteBatch.Draw(mutliTexture, new RectangleF(mutliFrameBuffer.Width / -2, mutliFrameBuffer.Height / -2, mutliFrameBuffer.Width, mutliFrameBuffer.Height), Color.Empty, 2);
                    spriteBatch.End();
                    CommandList.SetFramebuffer(MainSwapchainBuffer);
                    spriteBatch.DrawBatch(CommandList, spritepipline);
                }
                else
                {
                    spriteBatch.Begin();
                    spriteBatch.ViewMatrix = matrix;
                    CommandList.SetVertexBuffer(0, vertexBuffer);
                    CommandList.SetPipeline(pipeline);
                    //var drawcount = framecount > frameBuffer.Length ? frameBuffer.Length : framecount;
                    for (int i = 0; i < framecount; i++)
                    {
                        if (frameBuffer[i] == null || frameBuffer[i].IsDisposed) return;
                        //System.Diagnostics.Debug.WriteLine($"frameindex:{i}");
                        CommandList.SetFramebuffer(frameBuffer[i]);
                        CommandList.ClearColorTarget(0, RgbaFloat.Clear);
                        CommandList.SetGraphicsResourceSet(0, sharedSet);
                        CommandList.Draw((uint)frmaelenght, 1, (uint)(i * frmaelenght + SkipCount), 0);
                        switch (StackConfig.StackingMode)
                        {
                            case StackingMode.Offset:
                                spriteBatch.Draw(frameTexture[i], new RectangleF(frameTexture[i].Width / -2 + StackConfig.XOffset * i, frameTexture[i].Height / -2 - StackConfig.YOffset * i, frameTexture[i].Width, frameTexture[i].Height), Color.Empty, 1);
                                break;
                            case StackingMode.Stitching:
                                spriteBatch.Draw(frameTexture[i], new RectangleF(frameTexture[i].Width / -2 + frameTexture[i].Width / framecount * i, frameTexture[i].Height / -2, frameTexture[i].Width / framecount, frameTexture[i].Height), Color.Empty, 1);
                                break;
                            case StackingMode.Auto:
                                spriteBatch.Draw(frameTexture[i], new RectangleF(frameTexture[i].Width / -2, frameTexture[i].Height / -2, frameTexture[i].Width, frameTexture[i].Height), Color.Empty, 1);
                                break;
                        }
                    }
                    spriteBatch.End();
                    CommandList.SetFramebuffer(MainSwapchainBuffer);
                    spriteBatch.DrawBatch(CommandList, spritepipline);
                }
            }
        }

        public float Brightness
        {
            get => _LineInfo.Brightness;
            set
            {
                float temp = Math.Clamp(value, 0.0f, 100.0f);
                if (_LineInfo.Brightness != temp)
                {
                    _LineInfo.Brightness = temp;
                    _UpdateLineInfo = true;
                }
            }
        }
        //private Int32 _DataCount = 0;

        private uint _MaxFrameCount;
        /// <summary>
        /// 最大帧数
        /// </summary>
        public uint MaxFrameCount
        {
            get => _MaxFrameCount;
            set
            {
                if (value != _MaxFrameCount && value > 0)
                {
                    lock (_Locker)
                    {
                        if (IsDisposed) return;
                        _MaxFrameCount = value;
                        vertexBuffer?.Dispose();
                        vertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<float>() * value * MaxFrameLength), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
                        var texturedesc = TextureDescription.Texture2D(MainSwapchainBuffer.Width, MainSwapchainBuffer.Height, 1, 1, MainSwapchainBuffer.ColorTargets[0].Target.Format, TextureUsage.RenderTarget | TextureUsage.Sampled);

                        for (int i = 0; i < frameBuffer.Length; i++)
                        {
                            frameBuffer[i]?.Dispose();
                            frameTexture[i]?.Dispose();
                        }
                        frameBuffer = new Framebuffer[value];
                        frameTexture = new Texture[value];

                        for (int i = 0; i < value; i++)
                        {

                            if (frameBuffer[i] == null || frameBuffer[i].Width != MainSwapchainBuffer.Width || frameBuffer[i].Height != MainSwapchainBuffer.Height)
                            {
                                frameTexture[i] = ResourceFactory.CreateTexture(texturedesc);
                                frameBuffer[i] = ResourceFactory.CreateFramebuffer(new FramebufferDescription(null, frameTexture[i]));
                            }
                        }
                    }
                }
            }
        }

        private uint _MaxFrameLength;
        /// <summary>
        /// 最大帧长度
        /// </summary>
        public uint MaxFrameLength
        {
            get => _MaxFrameLength;
            private set
            {
                if (value != _MaxFrameLength && value > 0)
                {
                    _MaxFrameLength = value;
                  //  vertexBuffer?.Dispose();
                  //  vertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<float>() * value * MaxFrameCount), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
                }
            }
        }

        /// <summary>
        /// 跳过的点数
        /// </summary>
        public uint SkipCount { get; set; } = 0;

        /// <summary>
        /// 绘制长度
        /// </summary>
        public Int32 DrawLength { get; set; }

        ///// <summary>
        ///// 帧长度
        ///// </summary>
        //public uint FrameLength { get; set; }
        private Int32 _DataCount = 0;
        public float[] Datas
        {
            set
            {
                if (vertexBuffer == null || value == null || value.Length == 0)
                {
                    _DataCount = 0;
                    return;
                }
                _DataCount = value.Length;
                lock (_Locker)
                {
                    CheckBufferLength((uint)(Unsafe.SizeOf<float>() * value.Length));
                    GraphicsDevice.UpdateBuffer(vertexBuffer, 0, value);
                }
            }
        }

        private void CheckBufferLength(uint length)
        {
            if (vertexBuffer?.SizeInBytes < length)
            {
                vertexBuffer?.Dispose();
                vertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription(length, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
                MaxFrameLength = length / MaxFrameCount;
            }
        }

        public Boolean UseCache { get; set; } = false;
        public void ClearCache()
        {
            clearCache = true;
        }

        public float SampleRate
        {
            get => _LineInfo.SampleRate;
            set
            {
                if (_LineInfo.SampleRate != value)
                {
                    _LineInfo.SampleRate = value;
                    _UpdateLineInfo = true;
                }
            }
        }

        public Color Color
        {
            get => color;
            set
            {
                if (color != value)
                {
                    _UpdateLineInfo = true;
                    color = value;
                    _LineInfo.Color = value.ColorConverToVect4();
                }
            }
        }
        public float HorizontalOffset
        {
            get => _LineInfo.HorizontalOffset;
            set
            {
                if (_LineInfo.HorizontalOffset != value)
                {
                    _UpdateLineInfo = true;
                    _LineInfo.HorizontalOffset = value;
                }
            }
        }
        public float VerticalOffset
        {
            get => _LineInfo.VerticalOffset;
            set
            {
                if (_LineInfo.VerticalOffset != value)
                {
                    _UpdateLineInfo = true;
                    _LineInfo.VerticalOffset = value;
                }
            }
        }
        public float ValueScale
        {
            get => _LineInfo.ValueScale;
            set
            {
                if (_LineInfo.ValueScale != value)
                {
                    _UpdateLineInfo = true;
                    _LineInfo.ValueScale = value;
                }
            }
        }
        public PrimitiveTopology PrimitiveTopology { get; set; } = PrimitiveTopology.LineStrip;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct LineInfoStruct
        {
            [MarshalAs(UnmanagedType.LPStruct, IidParameterIndex = 0)]
            public Vector4 Color;
            [MarshalAs(UnmanagedType.LPStruct, IidParameterIndex = 1)]
            public Vector4 LineRange;
            [MarshalAs(UnmanagedType.R4, IidParameterIndex = 2)]
            public float VerticalOffset;
            [MarshalAs(UnmanagedType.R4, IidParameterIndex = 3)]
            public float HorizontalOffset;
            [MarshalAs(UnmanagedType.R4, IidParameterIndex = 4)]
            public float Brightness;
            [MarshalAs(UnmanagedType.R4, IidParameterIndex = 5)]
            public float SampleRate;
            [MarshalAs(UnmanagedType.R4, IidParameterIndex = 6)]
            public float ValueScale;
            [MarshalAs(UnmanagedType.LPStruct, IidParameterIndex = 7)]
            public Vector2 CrossSize;
            [MarshalAs(UnmanagedType.R4, IidParameterIndex = 8)]
            public float Spare;
        }
        public override void DisposeResources()
        {
            lock (_Locker)
            {
                if (IsDisposed) return;
                base.DisposeResources();
                spriteBatch?.Dispose();
                vertexBuffer?.Dispose();
                for (int i = 0; i < MaxFrameCount; i++)
                {
                    frameBuffer[i]?.Dispose();
                    frameTexture[i]?.Dispose();
                }
                crossPipline?.Dispose();
                lineInfoBuffer?.Dispose();
                linePipline?.Dispose();
                pointPipline?.Dispose();
                mutliFrameBuffer?.Dispose();
                mutliTexture?.Dispose();
                proviewBuffer?.Dispose();
                sharedLayout?.Dispose();
                sharedSet?.Dispose();
                spritepipline?.Dispose();
                IsDisposed = false;
            }
            //mainspriteBatch?.Dispose();
        }

    }
    /// <summary>
    /// 堆叠方式
    /// </summary>
    public enum StackingMode
    {
        /// <summary>
        /// 自动
        /// </summary>
        Auto,
        /// <summary>
        /// 拼接
        /// </summary>
        Stitching,
        /// <summary>
        /// 偏移
        /// </summary>
        Offset,
    }
    public class StackConfig
    {
        internal StackConfig()
        {

        }
        /// <summary>
        /// 堆叠方式参数
        /// 当<see cref="StackingMode"/>的值为<see cref="StackingMode.Offset"/>时
        /// 参数<see cref="XOffset"/>和<see cref="YOffset"/>有效
        /// </summary>
        public StackingMode StackingMode { get; set; } = StackingMode.Auto;
        public float XOffset { get; set; } = 5;
        public float YOffset { get; set; } = 5;
    }
}
