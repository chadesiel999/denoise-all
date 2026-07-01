using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Veldrid.Sdl2;
using Veldrid.Common.Tools;
using System.Diagnostics;

namespace Veldrid.Common
{
    internal class GraphicsManger:IDisposable
    {
        private LineRange _Range = new LineRange(0, 10000, -4000, 4000);
        internal void SetLineRange(int minX,int maxX,int minY,int maxY)
        {
            _Range.MinX = minX;
            _Range.MinY = minY;
            _Range.MaxX = maxX;
            _Range.MaxY = maxY;
        }
        private GraphicsDevice graphicsDevice;
        private Sdl2.Sdl2Window window;
        private bool disposedValue;
        public Boolean IsDisposed => disposedValue;

        public Padding DefaultPadding { get; } = new Padding();
        public LineRange DefaultLineRange =>_Range;
        internal static Vector2 CursorFixedSize  => new Vector2(46,56);
        public GraphicsDevice Device => graphicsDevice;
        public ResourceFactory ResourceFactory => Device.ResourceFactory;
        public GraphicsBackend Backend { get; }
        public Vector2 DPIScale { get; }
        public Veldrid.Sdl2.Sdl2Window Window => window;
        public ShaderManger ShaderManger { get; private set; }
        internal CommandList CommandList { get; private set; }
        public Boolean IsExists => Window != null ? Window.Exists : false;
        public Camera Camera { get; private set; }
        internal GraphicsManger(GraphicsBackend backend = GraphicsBackend.Direct3D11)
        {
            Backend = backend;
            GraphicsDeviceOptions options = new GraphicsDeviceOptions(false, null, true, ResourceBindingModel.Default, true, true, true);
            window = new Sdl2Window("VeldridContent", 0, 0, 200, 200,  SDL_WindowFlags.Hidden | SDL_WindowFlags.OpenGL | SDL_WindowFlags.Borderless, false);
            Sdl2Native.SDL_Init(SDLInitFlags.GameController);
            graphicsDevice = StartupUtilities.VeldridWindow.CreateGraphicsDevice(window, options,Backend);
            window.BorderVisible = false;
            window.Visible = false;
            CommandList = Device.ResourceFactory.CreateCommandList();
            GLSLManger.Default.Init();
            ShaderManger = new ShaderManger(Device);
            Camera = new Camera(Device,window);
        }

        internal void SetWindowSize()
        {
            if(Window ==null)
            {
                return;
            }
            Device.ResizeMainWindow((uint)window.Width, (uint)window.Height);
            Camera.WindowResized(window.Width, window.Height);
        }
        internal void Close()
        {
            Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CommandList?.Dispose();
                    window?.ClearEventHandle();
                    window?.Close();
                    ShaderManger?.Dispose();
                    Device?.WaitForIdle();
                    Device?.Dispose();
                    
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~GraphicsManger()
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
