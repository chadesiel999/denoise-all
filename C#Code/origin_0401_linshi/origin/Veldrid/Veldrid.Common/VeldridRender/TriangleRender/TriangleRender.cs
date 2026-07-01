using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid.Common.Tools;

namespace Veldrid.Common.VeldridRender
{
    internal unsafe class TriangleRender : BaseVeldridRender
    {
        private UInt32 _MaxCount = 1000;
        private UInt32 _DataLenght = 0;
        private DeviceBuffer _VertexBuffer;
        private DeviceBuffer _ProjViewBuffer;
        private DeviceBuffer _LineInfoBuffer;
        private DeviceBuffer _ColorInfoBuffer;
        private ResourceLayout _ShardLayout;
        private ResourceSet _ShardSet;
        private Pipeline _Pipeline;
        private Color color;
        private float horizontalOffset;
        private float verticalOffset;
        private float width;
        private float top;

        public TriangleRender(IVeldridContent control, UInt32 maxCount = 100) : base(control)
        {
            if (maxCount <= 0) throw new ArgumentOutOfRangeException(nameof(maxCount));
            _MaxCount = maxCount;
        }
        public override void CreateResources()
        {
            base.CreateResources();
            _VertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription(_MaxCount * (uint)sizeof(Vertex), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
    new VertexElementDescription[]
    {
        new VertexElementDescription("in_Position", VertexElementFormat.Float1, VertexElementSemantic.TextureCoordinate),
        new VertexElementDescription("in_isKey", VertexElementFormat.Int1, VertexElementSemantic.TextureCoordinate) // 假设 in_isKey 用于控制某些特定的顶点行为
    }
);
            _ProjViewBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>() * 2, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _LineInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<Vector4>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _ColorInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<Vector4>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _ShardLayout = ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ProjView", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("LineInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("ColorInfo", ResourceKind.UniformBuffer, ShaderStages.Fragment)));
            _ShardSet = ResourceFactory.CreateResourceSet(new ResourceSetDescription(_ShardLayout, _ProjViewBuffer, _LineInfoBuffer, _ColorInfoBuffer));
            var shaders = CreateShader("TriangleRender");
            if(GraphicsDevice.BackendType == GraphicsBackend.Direct3D11)
            {
                var shader = GetOtherShader("TriangleRender.Geometry.hlsl");
                _Pipeline = CreatePipLine(PrimitiveTopology.PointList, _ShardLayout, vertexLayout, new Shader[] { shaders[0], shaders[1], shader });
            }
            else
            {
                _Pipeline = CreatePipLine(PrimitiveTopology.PointList, _ShardLayout, vertexLayout, shaders);
            }
        }
        public override void DisposeResources()
        {
            base.DisposeResources();
            _VertexBuffer?.Dispose();
            _ShardSet?.Dispose();
            _ShardLayout?.Dispose();
            _LineInfoBuffer?.Dispose();
            _ColorInfoBuffer?.Dispose();
            _Pipeline?.Dispose();
            _ProjViewBuffer?.Dispose();
        }
        public void SetData(float[] data)
        {
            lock (_Locker)
            {
                if (data == null || data.Length == 0 || _VertexBuffer == null || _VertexBuffer.IsDisposed || IsDisposed)
                {
                    _DataLenght = 0;
                    return;
                }
                _DataLenght = (uint)data.Length;

                _MaxCount = _DataLenght * 2;
                _VertexBuffer?.Dispose();
                _VertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription(_MaxCount * (uint)sizeof(Vertex), BufferUsage.VertexBuffer | BufferUsage.Dynamic));

                Vertex[] vertex = new Vertex[_DataLenght];
                for (int i = 0; i < _DataLenght; i++)
                {
                    vertex[i] = new Vertex()
                    {
                        Position = data[ i],
                        IsKey = 0
                    };
                }

                GraphicsDevice.UpdateBuffer(_VertexBuffer, 0, vertex);
            }
        }

        private struct Vertex
        {
            public float Position;  // 对应 GLSL 中的 in_Position
            public int IsKey;      // 对应 GLSL 中的 in_iskey
        }

        public void SetData((Double[,] positions, Boolean[] iskeys) data)
        {
            lock (_Locker)
            {
                if (data.iskeys == null || data.iskeys.Length == 0 || _VertexBuffer == null || _VertexBuffer.IsDisposed || IsDisposed)
                {
                    _DataLenght = 0;
                    return;
                }
                _DataLenght = (uint)data.iskeys.Length;
                if (_DataLenght > _MaxCount)
                {
                    _MaxCount = _DataLenght * 2;
                    _VertexBuffer?.Dispose();
                    _VertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription(_MaxCount * (uint)sizeof(Vertex), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
                }

                Vertex[] vertex = new Vertex[data.iskeys.Length];
                for (int i = 0,l= data.iskeys.Length; i < l; i++)
                {
                    vertex[i] = new Vertex()
                    {
                        Position= (float)data.positions[0,i],
                        IsKey = data.iskeys[i] ? 1 : 0
                    };
                }

                GraphicsDevice.UpdateBuffer(_VertexBuffer, 0, vertex);
                //GraphicsDevice.UpdateBuffer(_VertexBuffer, 0, data.Cast<Double>().Select(x => (Single)x).ToArray());
            }
        }
        internal override void PreDraw()
        {
            if (IsDisposed || _ProjViewBuffer.IsDisposed || _LineInfoBuffer.IsDisposed) return;
            base.PreDraw();
            if (this[nameof(Range), nameof(Margin)] | WindowSizeState)
            {
                CommandList.UpdateBuffer(_ProjViewBuffer, 0, Camera.OrthographicMatrix);
                CommandList.UpdateBuffer(_ProjViewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>(), Camera.GetLineMatrix(Margin, Range));
                CommandList.UpdateBuffer(_LineInfoBuffer, 0, Range.MaxY);
                CommandList.UpdateBuffer(_LineInfoBuffer, 4 * 2, Camera.AspectRatio);
                this[nameof(Range), nameof(Margin)] = false;
                WindowSizeState = false;
            }
        }
        internal override void DrawData()
        {
            if (!Visibily || _DataLenght == 0 || IsDisposed || _VertexBuffer.IsDisposed|| _ProjViewBuffer.IsDisposed || _LineInfoBuffer.IsDisposed) return;
            CommandList.SetFramebuffer(MainSwapchainBuffer);
            CommandList.SetVertexBuffer(0, _VertexBuffer);
            CommandList.SetPipeline(_Pipeline);
            CommandList.SetGraphicsResourceSet(0, _ShardSet);
            CommandList.Draw(_DataLenght);
        }

        public UInt32 DataLengh { get => _DataLenght; }
        public float Width
        {
            get => width;
            set
            {
                if (value == width) return;
                width = value;
                lock (_Locker)
                {
                    if (IsDisposed || _LineInfoBuffer.IsDisposed) return;
                    GraphicsDevice.UpdateBuffer(_LineInfoBuffer, 4, value);
                    GraphicsDevice.UpdateBuffer(_LineInfoBuffer, 4 * 2, Camera.AspectRatio);
                }
            }
        }
        public Color Color
        {
            get => color;
            set
            {
                if (color.R != value.R || color.B != value.B || color.G != value.G)
                {
                    var val = value.ColorConverToRGBA();
                    color = value;
                    lock (_Locker)
                    {
                        if (_ColorInfoBuffer.IsDisposed || IsDisposed) return;
                        GraphicsDevice.UpdateBuffer(_ColorInfoBuffer, 0, (IntPtr)(&val), (UInt32)Unsafe.SizeOf<float>() * 3);
                    }
                }
            }
        }
        public Single HorizontalOffset
        {
            get => horizontalOffset;
            set
            {
                if (value == horizontalOffset) return;
                horizontalOffset = value;
                lock (_Locker)
                {
                    if (_LineInfoBuffer.IsDisposed || IsDisposed) return;
                    GraphicsDevice.UpdateBuffer(_LineInfoBuffer, 4 * 3, value);
                }
            }
        }
        public float Brightness
        {
            get => color.A / 255f * 100;
            set
            {
                lock (_Locker)
                {
                    var temp = color.A / 255f * 100;
                    if (temp != value)
                    {
                        color = Color.FromArgb((Byte)(value / 100f * 255), color);
                        if (IsDisposed || _ColorInfoBuffer.IsDisposed) return;
                        GraphicsDevice.UpdateBuffer(_ColorInfoBuffer, (UInt32)Unsafe.SizeOf<float>() * 3, value / 100);
                    }
                }
            }
        }
    }
}
