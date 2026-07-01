using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Common.Tools;

namespace Veldrid.Common.VeldridRender
{
    internal abstract class BaseVeldridRender
    {
        protected Object _Locker = new Object();
        public Boolean IsDisposed { get; protected set; } = false;
        protected CommandList CommandList { get; }
        private ShaderManger ShaderManger { get; }
        protected Framebuffer MainSwapchainBuffer => GraphicsDevice.MainSwapchain.Framebuffer;
        internal Camera Camera { get; }
        public Boolean WindowSizeState { get; set; } = true;
        private Dictionary<string, bool> propertyChangedDictionary = new Dictionary<string, bool>();
        internal GraphicsDevice GraphicsDevice { get; }
        internal Vector2 WindowSize => new Vector2(GraphicsDevice.MainSwapchain.Framebuffer.Width, GraphicsDevice.MainSwapchain.Framebuffer.Height);
        private protected ResourceFactory ResourceFactory => GraphicsDevice.ResourceFactory;
        private bool TryGetPropertyState(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return false;
            }
            if (propertyChangedDictionary.TryGetValue(propertyName, out var result))
            {
                return result;
            }
            propertyChangedDictionary[propertyName] = false;
            return false;
        }
        public virtual bool Contains(PointF point)
        {
            if (!Visibily) return false;
            RectangleF rectangle = new RectangleF(Local, new SizeF(VirtualSize.X, VirtualSize.Y));
            return rectangle.Contains(point);
        }
        /// <summary>
        /// 元素在虚拟坐标系中的绝对位置，
        /// 此参数的设置对波形绘制等无效，设置波形的位置请使用<see cref="Margin"/>
        /// </summary>
        public virtual PointF Local
        {
            get;
            set;
        }
        public virtual System.Numerics.Vector2 VirtualSize { get; }

        internal protected Boolean this[params string[] propertyNames]
        {
            get
            {
                if (propertyNames == null || propertyNames.Length == 0)
                {
                    return false;
                }
                if (propertyNames.Length == 1)
                {
                    return TryGetPropertyState(propertyNames[0]);
                }
                return propertyNames.Select(x => TryGetPropertyState(x)).Any(x => x);
            }
            set
            {
                if (propertyNames == null || propertyNames.Length == 0)
                {
                    return;
                }
                foreach (var name in propertyNames.Where(x => !string.IsNullOrEmpty(x)))
                {
                    propertyChangedDictionary[name] = value;
                }
            }
        }
        private Padding margin = new Padding();

        public Padding Margin
        {
            get { return margin; }
            set { Set(ref margin, value); }
        }
        public RectangleF Rectangle => new RectangleF(Margin.Left, Margin.Top, MainSwapchainBuffer.Width - Margin.Left - Margin.Right, MainSwapchainBuffer.Height - Margin.Top - Margin.Bottom);
        private LineRange range = new LineRange(0, 10000, -5000, 5000);

        public LineRange Range
        {
            get { return range; }
            set { Set(ref range, value); }
        }
        public BaseVeldridRender(IVeldridContent control)
        {
            GraphicsDevice = control.GraphicsManger.Device;
            CommandList = control.GraphicsManger.CommandList;
            Range = control.GraphicsManger.DefaultLineRange;
            Margin = control.GraphicsManger.DefaultPadding;
            Camera = control.GraphicsManger.Camera;
            ShaderManger = control.GraphicsManger.ShaderManger;
        }
        protected void Set<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (object.Equals(field, value))
            {
                return;
            }
            field = value;
            this[propertyName] = true;
            Update(propertyName, value);
        }
        public virtual void CreateResources()
        {
            IsDisposed = false;
        }
        internal virtual void PreDraw()
        {

        }
        public void Draw()
        {
            lock (_Locker)
            {
                if (Visibily && !IsDisposed)
                {
                    PreDraw();
                    DrawData();
                    PosDraw();
                }
            }
        }
        internal virtual void DrawData()
        {
        }
        internal virtual void PosDraw()
        {

        }
        public virtual void DisposeResources()
        {
            this.ClearEventHandle();
            IsDisposed = true;
        }
        public virtual void Update(string propertyName, object value)
        {
        }
        private protected DeviceBuffer CreateVertexBuffer<T>(T[] value) where T : unmanaged
        {
            var buffer = GraphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription((uint)(value.Length * Unsafe.SizeOf<T>()), BufferUsage.VertexBuffer));
            GraphicsDevice.UpdateBuffer(buffer, 0, value);
            return buffer;
        }
        private protected DeviceBuffer CreateIndexBuffer(ushort[] index)
        {
            var buffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(index.Length * Unsafe.SizeOf<ushort>()), BufferUsage.IndexBuffer));
            GraphicsDevice.UpdateBuffer(buffer, 0, index);
            return buffer;
        }
        private protected DeviceBuffer CreateIndexBuffer(uint[] index)
        {
            var buffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(index.Length * Unsafe.SizeOf<uint>()), BufferUsage.IndexBuffer));
            GraphicsDevice.UpdateBuffer(buffer, 0, index);
            return buffer;
        }
        private protected Shader[] CreateShader(string name)
        {
            return ShaderManger.GetShaders(name);
        }
        private protected Shader GetLocalFileShader(string path, string entrypoint = "main", ShaderStages stages = ShaderStages.Vertex) => ShaderManger.GetLocalFileShader(path, entrypoint, stages);
        private protected Shader GetOtherShader(string name, ShaderStages stages = ShaderStages.Geometry, string entrypoints = "main")
        {
            return ShaderManger.GetOtherShader(name, stages, entrypoints);
        }
        private protected Pipeline CreatePipLine(PrimitiveTopology primitiveTopology,
            ResourceLayout layout,
            VertexLayoutDescription vertexLayout,
            Shader[] shaders,
            BlendStateDescription? blend = null,
            bool depthTestEnabled = true,
            OutputDescription? outputDescription = null,
            FrontFace frontFace = FrontFace.Clockwise)
        {
            return CreatePipLine(primitiveTopology, new ResourceLayout[] { layout }, new VertexLayoutDescription[] { vertexLayout }, shaders, blend, depthTestEnabled, outputDescription, frontFace);
        }
        private protected Pipeline CreatePipLine(PrimitiveTopology primitiveTopology,
            ResourceLayout[] layouts,
            VertexLayoutDescription[] vertexLayouts,
            Shader[] shaders,
            BlendStateDescription? blend = null,
            bool depthTestEnabled = true,
            OutputDescription? outputDescription = null,
            FrontFace frontFace = FrontFace.Clockwise)
        {
            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = blend ?? BlendStateDescription.SingleAlphaBlend;
            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: depthTestEnabled,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.LessEqual);
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: frontFace,
                depthClipEnabled: true,
                scissorTestEnabled: false);
            pipelineDescription.PrimitiveTopology = primitiveTopology;
            pipelineDescription.ResourceLayouts = layouts;
            pipelineDescription.Outputs = outputDescription ?? GraphicsDevice.SwapchainFramebuffer.OutputDescription;
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: vertexLayouts,
                shaders: shaders,
                specializations: new[] { new SpecializationConstant(0, GraphicsDevice.IsClipSpaceYInverted) });
            return ResourceFactory.CreateGraphicsPipeline(pipelineDescription);
        }

        public bool Visibily { get; set; } = true;

        private protected struct WindowsInfo
        {
        }
    }


}
