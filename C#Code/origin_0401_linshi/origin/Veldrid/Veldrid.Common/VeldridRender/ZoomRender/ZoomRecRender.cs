using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid.Common.Tools;

namespace Veldrid.Common.VeldridRender
{
    internal class ZoomRecRender : BaseVeldridRender
    {
        private float unitsPerPxX;//每个像素占据的单位
        private float unitsPerPxY;
        Boolean needreDraw = true;
        [AllowNull]
        Pipeline linepipeline;
        [AllowNull]
        Pipeline _PointPipeline;
        [AllowNull]
        Pipeline backpipline;
        #region FrameBuffer;
        [AllowNull]
        Framebuffer frameBuffer;
        [AllowNull]
        Texture textureBuffer;
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
        DeviceBuffer _ColorInfoBuffer;
        [AllowNull]
        ResourceLayout sharedLayout;
        [AllowNull]
        ResourceSet sharedSet;
        #endregion 
        [AllowNull]
        Veldrid.Common.VeldridRender.ImageRender.VeldridSpriteBatch spriteBatch;
        private RectangleF rect;
        private Vector2[] _Back = new Vector2[0];
        private Vector2[] _Lines = new Vector2[0];
        private Vector2[] _Points = new Vector2[0];
        public ZoomRecRender(IVeldridContent control) : base(control)
        {
        }
        public override void CreateResources()
        {
            base.CreateResources();
            vertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Vector2>() * 16), BufferUsage.VertexBuffer));

            vertexLayout = new VertexLayoutDescription(new VertexElementDescription("in_Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));
            shaders = CreateShader("ZoomRecRender");
            ResourceLayoutElementDescription[] resourceLayoutElementDescriptions =
            {
                new ResourceLayoutElementDescription("ProView", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("LineInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("ColorInfo", ResourceKind.UniformBuffer, ShaderStages.Fragment),
            };
            proviewBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>() * 2, BufferUsage.Dynamic | BufferUsage.UniformBuffer));
            lineInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Vector4>()), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _ColorInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Vector4>()), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            ResourceLayoutDescription resourceLayoutDescription = new ResourceLayoutDescription(resourceLayoutElementDescriptions);
            sharedLayout = ResourceFactory.CreateResourceLayout(resourceLayoutDescription);
            BindableResource[] bindableResources = new BindableResource[] { proviewBuffer, lineInfoBuffer, _ColorInfoBuffer };
            ResourceSetDescription resourceSetDescription = new ResourceSetDescription(sharedLayout, bindableResources);
            sharedSet = ResourceFactory.CreateResourceSet(resourceSetDescription);
            spriteBatch = new ImageRender.VeldridSpriteBatch(GraphicsDevice, MainSwapchainBuffer.OutputDescription, CreateShader("ImageRender"), GraphicsDevice.PointSampler, FaceCullMode.None, BlendStateDescription.SingleAlphaBlend);
            CreateFrameBuffer();
            backpipline = CreatePipLine(PrimitiveTopology.TriangleStrip, sharedLayout, vertexLayout, shaders, BlendStateDescription.SingleAlphaBlend, true, frameBuffer.OutputDescription, frontFace: FrontFace.CounterClockwise);
            if (GraphicsDevice.BackendType == GraphicsBackend.Direct3D11)
            {
                var lineshader = GetOtherShader("ZoomRecRenderLine.Geometry.hlsl");
                linepipeline = CreatePipLine(PrimitiveTopology.LineStrip, sharedLayout, vertexLayout, new Shader[] { shaders[0], shaders[1], lineshader }, BlendStateDescription.SingleAlphaBlend, frontFace: FrontFace.CounterClockwise);

                var pointshader = GetOtherShader("ZoomRecRenderPoint.Geometry.hlsl");
                _PointPipeline = CreatePipLine(PrimitiveTopology.PointList, sharedLayout, vertexLayout, new Shader[] { shaders[0], shaders[1], pointshader }, BlendStateDescription.SingleAlphaBlend, frontFace: FrontFace.CounterClockwise);
            }
            else
            {
                linepipeline = CreatePipLine(PrimitiveTopology.LineStrip, sharedLayout, vertexLayout, shaders, BlendStateDescription.SingleAlphaBlend, frontFace: FrontFace.CounterClockwise);

                _PointPipeline = CreatePipLine(PrimitiveTopology.PointList, sharedLayout, vertexLayout, shaders, BlendStateDescription.SingleAlphaBlend, frontFace: FrontFace.CounterClockwise);
            }
            WindowSizeState = true;
            needreDraw = true;
        }
        private void CreateFrameBuffer()
        {
            var texturedesc = TextureDescription.Texture2D(MainSwapchainBuffer.Width, MainSwapchainBuffer.Height, 1, 1, MainSwapchainBuffer.ColorTargets[0].Target.Format, TextureUsage.RenderTarget | TextureUsage.Sampled);
            if (frameBuffer == null || frameBuffer.Width != MainSwapchainBuffer.Width || frameBuffer.Height != MainSwapchainBuffer.Height)
            {
                textureBuffer?.Dispose();
                textureBuffer = ResourceFactory.CreateTexture(texturedesc);
                frameBuffer?.Dispose();
                frameBuffer = ResourceFactory.CreateFramebuffer(new FramebufferDescription(null, textureBuffer));
                needreDraw = true;
            }
        }
        private void GetVectors(out Vector2[] back, out Vector2[] lines, out Vector2[] points)
        {
            RectangleF temprect = rect;
            List<Vector2> vector2s = new List<Vector2>();
            float left = temprect.X - Range.MinX;
            float right = Range.MaxX - (temprect.X + temprect.Width);
            if (left == 0)
            {
                temprect.X += unitsPerPxX;
                temprect.Width -= unitsPerPxX;
            }
            if (right == 0)
            {
                temprect.Width -= unitsPerPxX;
            }
            float top = temprect.Y - Range.MinY;
            float bottom = Range.MaxY - (temprect.Y + rect.Height);
            if (top == 0)
            {
                temprect.Y += unitsPerPxY;
                temprect.Height -= unitsPerPxY;
            }
            if (bottom == 0)
            {
                temprect.Height -= unitsPerPxY;
            }
            if (left > 0)
            {
                vector2s.Add(new Vector2(Range.MinX, Range.MinY));
                vector2s.Add(new Vector2(temprect.X, Range.MinY));
                vector2s.Add(new Vector2(Range.MinX, Range.MaxY));
                vector2s.Add(new Vector2(temprect.X, Range.MaxY));
            }
            if (top > 0)
            {
                vector2s.Add(new Vector2(temprect.X, Range.MinY));
                vector2s.Add(new Vector2(Range.MaxX, Range.MinY));
                vector2s.Add(new Vector2(temprect.X, temprect.Y));
                vector2s.Add(new Vector2(Range.MaxX, temprect.Y));
            }
            if (right > 0)
            {

                vector2s.Add(new Vector2(temprect.X + temprect.Width, temprect.Y));
                vector2s.Add(new Vector2(Range.MaxX, temprect.Y));
                vector2s.Add(new Vector2(temprect.X + temprect.Width, Range.MaxY));
                vector2s.Add(new Vector2(Range.MaxX, Range.MaxY));
            }
            if (bottom > 0)
            {
                vector2s.Add(new Vector2(temprect.X, temprect.Y + temprect.Height));
                vector2s.Add(new Vector2(temprect.X + temprect.Width, temprect.Y + temprect.Height));
                vector2s.Add(new Vector2(temprect.X, Range.MaxY));
                vector2s.Add(new Vector2(temprect.X + temprect.Width, Range.MaxY));
            }
            back = vector2s.ToArray();
            lines = new Vector2[5];
            lines[0] = new Vector2(temprect.X, temprect.Y);
            lines[1] = new Vector2(temprect.X + temprect.Width, temprect.Y);
            lines[2] = new Vector2(temprect.X + temprect.Width, temprect.Y + temprect.Height);
            lines[3] = new Vector2(temprect.X, temprect.Y + temprect.Height);
            lines[4] = lines[0];
            points = new Vector2[8];
            Array.Copy(lines, points, 4);
            points[4] = new Vector2(temprect.X + temprect.Width / 2, temprect.Y);
            points[5] = new Vector2(temprect.X + temprect.Width, temprect.Y + temprect.Height / 2);
            points[6] = new Vector2(temprect.X + temprect.Width / 2, temprect.Y + temprect.Height);
            points[7] = new Vector2(temprect.X, temprect.Y + temprect.Height / 2);
        }

        internal override void DrawData()
        {
            if (!Visibily || IsDisposed || vertexBuffer.IsDisposed || _ColorInfoBuffer.IsDisposed || proviewBuffer.IsDisposed || lineInfoBuffer.IsDisposed) return;

            CommandList.SetFramebuffer(frameBuffer);
            CommandList.ClearColorTarget(0, RgbaFloat.Clear);
            if (_Back.Length < 1 || _Lines.Length < 1 || _Points.Length < 1 || needreDraw)
            {
                GetVectors(out _Back, out _Lines, out _Points);
                needreDraw = false;
            }
            if (BackColor.A > 0 && _Back.Length > 0)
            {
                CommandList.UpdateBuffer(vertexBuffer, 0, _Back);
                CommandList.SetVertexBuffer(0, vertexBuffer);

                CommandList.UpdateBuffer(_ColorInfoBuffer, 0, BackColor.ColorConverToRGBA());
                CommandList.SetPipeline(backpipline);
                CommandList.SetGraphicsResourceSet(0, sharedSet);
                for (uint i = 0; i < _Back.Length / 4; i++)
                {
                    CommandList.Draw(4, 1, i * 4, 0);
                }
            }
            if (BorderColor.A > 0)
            {
                CommandList.UpdateBuffer(vertexBuffer, 0, _Lines);
                CommandList.SetPipeline(linepipeline);
                CommandList.UpdateBuffer(_ColorInfoBuffer, 0, BorderColor.ColorConverToRGBA());
                CommandList.SetGraphicsResourceSet(0, sharedSet);

                CommandList.Draw((UInt32)_Lines.Length);


                CommandList.UpdateBuffer(vertexBuffer, 0, _Points);
                CommandList.SetPipeline(_PointPipeline);
                CommandList.SetGraphicsResourceSet(0, sharedSet);

                CommandList.Draw((UInt32)_Points.Length);

            }
            spriteBatch.Begin();
            spriteBatch.ViewMatrix = Matrix4x4.CreateOrthographic(MainSwapchainBuffer.Width, MainSwapchainBuffer.Height, -1, -100f);
            spriteBatch.Draw(textureBuffer, new RectangleF(MainSwapchainBuffer.Width / -2, MainSwapchainBuffer.Height / -2, MainSwapchainBuffer.Width, MainSwapchainBuffer.Height), Color.Empty, 2);
            spriteBatch.End();

            CommandList.SetFramebuffer(MainSwapchainBuffer);
            spriteBatch.DrawBatch(CommandList);
        }
        internal override void PreDraw()
        {
            if (IsDisposed || proviewBuffer.IsDisposed || lineInfoBuffer.IsDisposed) return;
            base.PreDraw();
            if (WindowSizeState)
            {
                CreateFrameBuffer();
            }
            if (this[nameof(Range), nameof(Margin)] || WindowSizeState)
            {
                CommandList.UpdateBuffer(proviewBuffer, 0, Camera.OrthographicMatrix);
                CommandList.UpdateBuffer(proviewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>(), Camera.GetLineMatrix(Margin, Range));

                unitsPerPxX = Range.XLenght / this.Rectangle.Width;
                unitsPerPxY = Range.YLenght / this.Rectangle.Height;

                CommandList.UpdateBuffer(lineInfoBuffer, 0, 4 * unitsPerPxX / Range.XLenght);
                CommandList.UpdateBuffer(lineInfoBuffer, 4, 4 * unitsPerPxY / Range.YLenght);

                if (this[nameof(Range)])
                {
                    needreDraw = true;
                }
                this[nameof(Range), nameof(Margin)] = false;
                if (WindowSizeState)
                {
                    WindowSizeState = false;
                }
            }
        }
        public Color BackColor { get; set; }
        public Color BorderColor { get; set; }

        public RectangleF Rect
        {
            get => rect;
            set
            {
                if (((Int32) rect.X) != ((Int32)value.X) || ((Int32)rect.Width) != ((Int32)value.Width) 
                    || ((Int32)rect.Y) != ((Int32)value.Y) || ((Int32)rect.Height) != ((Int32)value.Height))
                {
                    rect = value;
                    needreDraw = true;
                }
            }
        }
        public override void DisposeResources()
        {
            lock (_Locker)
            {
                base.DisposeResources();
                spriteBatch?.Dispose();
                vertexBuffer?.Dispose();
                lineInfoBuffer?.Dispose();
                proviewBuffer?.Dispose();
                sharedLayout?.Dispose();
                sharedSet?.Dispose();
                linepipeline?.Dispose();
                backpipline?.Dispose();
                frameBuffer?.Dispose();
                textureBuffer?.Dispose();
                _PointPipeline?.Dispose();
                _ColorInfoBuffer?.Dispose();
            }
        }
    }

    public enum Boundary { Top, Bottom, Left, Right, Body, Out };
}
