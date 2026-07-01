using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Common;
using Veldrid.Common.Tools;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.ImageRender;

namespace Veldrid.Common.VeldridRender
{
    internal class UPORender : BaseVeldridRender
    {
        private Boolean _Needupdateinfo = true;
        private Boolean _NeedCampute = false;
        private ChInfo _ChInfo;
        private Int32 _Width;
        private Int32 _Height;
        private Texture frameTexture;
        private Texture frameTexture2;
        private UPOSpriteBatch spriteBatch;
        private DeviceBuffer _ChInfoBuffer;
        private UPOCompute _UPOCompute;
        [StructLayout(LayoutKind.Sequential, Pack =1)]
        struct ChInfo
        {
            public int MaxValue;
            public int MinValue;
            public int ChOnCount;
            //当启用1个通道时表示启用的通道序号，从0开始，当启用两个通道时表示第一个启用的通道需要
            public int ChIndex1;
            //当启用两个通道时表示第一个启用的通道需要
            public int ChIndex2;

            public int Brightness;
            public Vector2 Spare1;
        }
        
        public unsafe void SetData(ref byte data ,uint datalen)
        {
            lock (_Locker)
            {
                if (frameTexture == null || frameTexture.IsDisposed || IsDisposed) return;
                if (datalen == 0)
                {
                    GraphicsDevice.UpdateTexture(frameTexture, new byte[_Width * _Height], 1, 1, 0, (uint)_Width, (uint)_Height, 1, 0, 0);
                }
                else
                {
                    GraphicsDevice.UpdateTexture(frameTexture, (IntPtr)Unsafe.AsPointer(ref data),datalen,  1, 1, 0, (uint)_Width, (uint)_Height,1, 0, 0);
                }
                _NeedCampute = true;
            }
        }
        public UPORender(IVeldridContent control, int width =1000,int height = 200,int chcount =4) : base(control)
        {
            _Colors = new Color[4];
            _Width = width;
            _Height = height;
            drawHeigth = height;
            drawWidth = width;
            _SizeChanged = true;
            Brightness = 10;
        }

        public override void CreateResources()
        {
            base.CreateResources();
            Shader[] shaders = new Shader[2];
            String path1 = Environment.CurrentDirectory + "\\UPORender.vert";
            String path2 = Environment.CurrentDirectory + "\\UPORender.frag";
            if (File.Exists(path2) && File.Exists(path1))
            {
                shaders[0] = GetLocalFileShader(path1);
                shaders[1] = GetLocalFileShader(path2, stages: ShaderStages.Fragment);
            }
            else
            {
                shaders = CreateShader("UPORender");
            }
            _ChInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((UInt32)(Unsafe.SizeOf<ChInfo>()+Unsafe.SizeOf<Vector4>() * 4), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            spriteBatch = new UPOSpriteBatch(GraphicsDevice, MainSwapchainBuffer.OutputDescription,
                shaders,_ChInfoBuffer, GraphicsDevice.LinearSampler, FaceCullMode.None,BlendStateDescription.SingleAlphaBlend);
            var texturedesc = TextureDescription.Texture2D((uint)_Width+2, (uint)_Height+2, 1, 1, PixelFormat.R8_UNorm, TextureUsage.Storage | TextureUsage.Sampled);
            frameTexture = ResourceFactory.CreateTexture(texturedesc);
            texturedesc.Format = PixelFormat.R32_G32_B32_A32_Float;
            frameTexture2 = ResourceFactory.CreateTexture(texturedesc);
            _UPOCompute = new UPOCompute(GraphicsDevice, GetOtherShader("UPORender.comp", ShaderStages.Compute),new[] { frameTexture,frameTexture2 }, _ChInfoBuffer);

        }
        public int Brightness
        {
            get => _ChInfo.Brightness;
            set
            {
                var val = Math.Clamp(value, 0, 100);
                if(val!=_ChInfo.Brightness)
                {
                    _ChInfo.Brightness = val;
                    _Needupdateinfo = true;
                }
            }
        }
        private Boolean _SizeChanged = false;
        private int drawWidth;

        public int DrawWidth
        {
            get { return drawWidth; }
            set 
            {
                if (drawWidth != value)
                {
                    drawWidth = value;
                    _SizeChanged = true;
                }
            }
        }
        private int drawHeigth;

        public int DrawHeigth
        {
            get { return drawHeigth; }
            set 
            {
                if (drawHeigth != value)
                {
                    drawHeigth = value;
                    _SizeChanged = true;
                }
            }
        }

        public Int32 MaxValue
        {
            get => _ChInfo.MaxValue;
            set
            {
                if(_ChInfo.MaxValue!=value)
                {
                    _Needupdateinfo = true;
                    _ChInfo.MaxValue = value;
                }
            }
        }
        public Int32 MinValue
        {
            get=>_ChInfo.MinValue;
            set
            {
                if(_ChInfo.MinValue!=value)
                {
                    _Needupdateinfo = true;
                    _ChInfo.MinValue = value;
                }
            }
        }
        public Int32 ChOnCount
        {
            get => _ChInfo.ChOnCount;
            set
            {
                if(_ChInfo.ChOnCount!=value)
                {
                    _Needupdateinfo = true;
                    _ChInfo.ChOnCount = value;
                }
            }
        }
        public Int32 ChIndex1
        {
            get => _ChInfo.ChIndex1;
            set
            {
                if(_ChInfo.ChIndex1!=value)
                {
                    _Needupdateinfo = true;
                    _ChInfo.ChIndex1 = value;
                }
            }
        }
        public Int32 ChIndex2
        {
            get => _ChInfo.ChIndex2;
            set
            {
                if( _ChInfo.ChIndex2!=value)
                {
                    _Needupdateinfo = true;
                    _ChInfo.ChIndex2 = value;
                }
            }
        }
        private Color[] _Colors = new Color[0];
        public Color[] Colors
        {
            get=>_Colors;
            set
            { 
                if (value == null || value.Length ==0 || value.Length !=_Colors.Length) return;
                Boolean state = false;
                for(int i=0;i<_Colors.Length;i++)
                {
                    if (value[i] != _Colors[i])
                    {
                        state = true;
                        break;
                    }
                }
                _Colors = value;
                if(state)
                {
                    GraphicsDevice.UpdateBuffer(_ChInfoBuffer, (UInt32)Unsafe.SizeOf<ChInfo>(), _Colors.Select(x => x.ColorConverToVect4()).Take(4).ToArray());
                    _NeedCampute = true;
                }
            }
        }
        internal override void DrawData()
        {
            if (IsDisposed || spriteBatch == null) return;
            CommandList.SetFramebuffer(GraphicsDevice.MainSwapchain.Framebuffer);
            spriteBatch.DrawBatch(CommandList);
        }
        internal override void PreDraw()
        {
            if (IsDisposed || spriteBatch == null) return;
            base.PreDraw();
            if(_Needupdateinfo)
            {
                _Needupdateinfo = false;
                GraphicsDevice.UpdateBuffer(_ChInfoBuffer, 0, _ChInfo);
                if(_NeedCampute)
                {
                    _NeedCampute = false;
                    _UPOCompute?.DoCompute(CommandList);
                }
            }
            if (_NeedCampute)
            {
                _NeedCampute = false;
                _UPOCompute?.DoCompute(CommandList);
            }
            if (WindowSizeState ||_SizeChanged)
            {
                spriteBatch.Begin();
                spriteBatch.ViewMatrix = Matrix4x4.CreateOrthographic(GraphicsDevice.MainSwapchain.Framebuffer.Width, GraphicsDevice.MainSwapchain.Framebuffer.Height, 0.01f, -100f);
                spriteBatch.Draw(frameTexture2,
                    new RectangleF(GraphicsDevice.MainSwapchain.Framebuffer.Width / -2, GraphicsDevice.MainSwapchain.Framebuffer.Height / -2, MainSwapchainBuffer.Width, MainSwapchainBuffer.Height),
                    new RectangleF(1, 1, DrawWidth, DrawHeigth),
                    Color.Empty,
                    0,
                    Vector2.Zero,
                    SpriteOptions.None, 0);
                spriteBatch.End();
                _SizeChanged = false;
                WindowSizeState= false;
            }
        }

        public override void DisposeResources()
        {
            lock (_Locker)
            {
                base.DisposeResources();
                _UPOCompute?.Dispose();
                _ChInfoBuffer?.Dispose();
                spriteBatch.Dispose();
                spriteBatch = null;
                frameTexture?.Dispose();
                frameTexture2?.Dispose();
            }
        }
    }

    class UPOCompute:IDisposable
    {
        private readonly UInt32 _Width;
        private readonly UInt32 _Height;
        private ResourceLayout _Layout;
        private ResourceSet _ResourceSet;
        private Pipeline _Pipeline;
        public UPOCompute(GraphicsDevice device,Shader shader,Texture[] texture,DeviceBuffer chBuffer)
        {
            _Width = texture[0].Width;
            _Height = texture[0].Height;
            _Layout = device.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Tex", ResourceKind.TextureReadWrite, ShaderStages.Compute),
                new ResourceLayoutElementDescription("OutTex", ResourceKind.TextureReadWrite, ShaderStages.Compute),
                new ResourceLayoutElementDescription("ChInfoBuffer", ResourceKind.UniformBuffer, ShaderStages.Compute)));
            _ResourceSet = device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(_Layout, texture[0], texture[1], chBuffer));
            _Pipeline = device.ResourceFactory.CreateComputePipeline(new ComputePipelineDescription(shader, _Layout, 25, 40, 1));
        }

        public void DoCompute(CommandList command)
        {
            command.SetPipeline( _Pipeline );
            command.SetComputeResourceSet(0, _ResourceSet);
            command.Dispatch(_Width / 25, _Height / 40, 1);
        }
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _ResourceSet?.Dispose();
                    _Layout?.Dispose();
                    _Pipeline?.Dispose();
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~UPOCompute()
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
