using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Common;

namespace Veldrid.Common.VeldridRender
{
    internal class ZoomRectRender : BaseVeldridRender
    {
        private DeviceBuffer windowSizeBuffer;
        private ResourceSet windowSizeSet;
        ResourceLayout windowsizeLayout;
        VertexLayoutDescription vertexLayout;
        private Vector2[] _Point =
        {
            new Vector2(-1f,1f),
            new Vector2(1f,1f),
            new Vector2(-1f,-1f),
            new Vector2(1f,-1f),
        };
        public ZoomRectRender(IVeldridContent control) : base(control)
        {
        }

        public override void CreateResources()
        {
            base.CreateResources();
            CreateVertexBuffer(_Point);
            CreateShader("ZoomRectRender");
            vertexLayout = new VertexLayoutDescription(
               new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));

            windowSizeBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<Uniform>(), BufferUsage.UniformBuffer));

            windowsizeLayout = ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(new ResourceLayoutElementDescription("in_WindowsInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
            windowSizeSet = ResourceFactory.CreateResourceSet(new ResourceSetDescription(windowsizeLayout, windowSizeBuffer));

            Padding padding = GetPixFormUnit();

            GraphicsDevice.UpdateBuffer(windowSizeBuffer, 0, new Uniform(padding, Color, BorderColor, WindowSize));

            CreatePipLine(PrimitiveTopology.TriangleStrip, windowsizeLayout, vertexLayout, Shaders);

        }

        private Padding GetPixFormUnit()
        {
            float minx = (Rect.X - Range.MinX) / (Range.MaxX - Range.MinX) * MainSwapchainBuffer.Width;
            float maxx = (Rect.X + Rect.Width - Range.MinX) / (Range.MaxX - Range.MinX) * MainSwapchainBuffer.Width;
            float maxy = (Range.MaxY -Rect.Y) / Range.YLenght * MainSwapchainBuffer.Height;
            float miny = (Range.MaxY - Rect.Y - Rect.Height) / Range.YLenght * MainSwapchainBuffer.Height;
            return new Padding(minx, maxx, miny, maxy);
        }

        internal override void DrawData()
        {
            base.DrawData();
            if (this[nameof(WindowSize)] || this[nameof(Color)] || this[nameof(BorderColor)] || this[nameof(Rect)])
            {
                Padding padding = GetPixFormUnit();
                GraphicsDevice.UpdateBuffer(windowSizeBuffer, 0, new Uniform(padding, Color, BorderColor, WindowSize));
                this[nameof(WindowSize)] = false;
                this[nameof(Color)] = false;
                this[nameof(BorderColor)] = false;
                this[nameof(Rect)] = false;
            }
            CommandList.SetVertexBuffer(0, VertexBuffer);
            CommandList.SetPipeline(Pipeline);
            CommandList.SetGraphicsResourceSet(0, windowSizeSet);
            CommandList.Draw((uint)_Point.Length);
            CommandList.End();
            GraphicsDevice.SubmitCommands(CommandList);
        }

        private Color _Color = Color.Transparent;
        public Color Color
        {
            get
            {
                return _Color;
            }
            set
            {
                Set(ref _Color, value);
            }
        }
        private Color _BorderColor = Color.Transparent;
        public Color BorderColor
        {
            get
            {
                return _BorderColor;
            }
            set
            {
                Set(ref _BorderColor, value);
            }
        }
        private RectangleF _Rect = new RectangleF(1,1,1,1);
        public RectangleF Rect
        {
            get
            {
                return _Rect;
            }
            set
            {
                Set(ref _Rect, value);
            }
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Uniform
        {
            public Vector4 Local;
            public Vector4 Color;
            public Vector4 BorderColor;
            public Vector4 WindowSize;
            public Uniform(Padding padding, Color color, Color bordecolor, SizeF windowSize)
            {
                Local = new Vector4(padding.Left, padding.Top, padding.Right, padding.Bottom);
                Color = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
                BorderColor = new Vector4(bordecolor.R / 255f, bordecolor.G / 255f, bordecolor.B / 255f, bordecolor.A / 255f);
                WindowSize = new Vector4(windowSize.Width, windowSize.Height, 0, 0);
            }
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Padding
        {
            public float Left;
            public float Top;
            public float Right;
            public float Bottom;

            public Padding(float left = 0, float top = 0, float right = 0, float bottom = 0)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
            public Padding(float all = 0) : this(all, all, all, all)
            {

            }
        }
    }
}
