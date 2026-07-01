using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid.Common;
using Veldrid.Common.VeldridRender;
using Veldrid.Sdl2;
using Veldrid.Common.Tools;

namespace Veldrid.Common.Plot
{
    public abstract class BaseRender:IRender
    {
        private Camera _camera;
        private Sdl2Window _window;
        [AllowNull]
        protected CommandList CommandList { get; }
        private protected GraphicsDevice GraphicsDevice { get; }
        private protected abstract BaseVeldridRender Renderer { get; }

        public BaseRender(IVeldridContent control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }
            _camera = control.GraphicsManger.Camera;
            CommandList = control.GraphicsManger.CommandList;
            GraphicsDevice = control.GraphicsManger.Device;
            _window= control.GraphicsManger.Window;
        }
        protected void SetCursor(SDL_SystemCursor cursor)=>_window.Cursor = cursor;
        public Boolean Visibily { get=>Renderer.Visibily; set=>Renderer.Visibily =value; }
        public int ZIndex { get; set; } = 0;
        internal virtual Boolean Skip => !Visibily;
        Boolean IRender.Skip => Skip;
        public virtual Padding Margin
        {
            get => Renderer.Margin;
            set
            {
                Renderer.Margin = value;
                (this as IRender).Children.ForEach(x => x.Margin = value);
            }
        }
        Camera IRender.Camera => _camera;
        Vector2 IRender.WindowSize => new Vector2(GraphicsDevice.MainSwapchain.Framebuffer.Width, GraphicsDevice.MainSwapchain.Framebuffer.Height);
        public RectangleF Rectangle => Renderer.Rectangle;
        public virtual LineRange LineRange
        {
            get => Renderer.Range;
            set
            {
                Renderer.Range = value;
                (this as IRender).Children.ForEach(x => x.Range = value);
            }
        }
        public virtual void Draw()
        {
            if (!Visibily) return;
            Renderer?.Draw();
            (this as IRender).Children.ForEach(x => x.Draw());
        }
        public virtual void CursorDraw()
        {
            //if (!Visibily) return;
            Renderer?.Draw();
            (this as IRender).Children.ForEach(x => x.Draw());
        }
        List<BaseVeldridRender> IRender.Children { get;  } = new List<BaseVeldridRender>();


        [AllowNull]
        public Object Tag { get; set; }
        protected virtual void SetWindowSizeState(Boolean state)
        {
            Renderer.WindowSizeState = state;
            (this as IRender).Children.ForEach(x => x.WindowSizeState = state);

        }
        bool IRender.WindowSizeState
        {
            get => Renderer.WindowSizeState;
            set
            {
                SetWindowSizeState(value);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.ClearEventHandle();
                    Renderer?.DisposeResources();
                    (this as IRender).Children.ForEach((x) => x.DisposeResources());
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~BaseRenderer()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool disposedValue;
    }
}
