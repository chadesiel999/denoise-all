using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid.Common.Tools;

namespace Veldrid.Common.VeldridRender.LineRender
{

    internal unsafe class AxisInstructionsRender : BaseVeldridRender
    {
        #region PipLine
        [AllowNull]
        Pipeline currentPipline;
        Vector3[] _Vertex = new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(0,0,0),
            new Vector3(0,1,0),
            new Vector3(0,0,0),
            new Vector3(0,0,-1),
        };
        #endregion

        VertexLayoutDescription vertexLayout;
        DeviceBuffer _VertexBuffer;
        DeviceBuffer _ProjViewBuffer;
        DeviceBuffer _LineInfoBuffer;

        ResourceLayout _ShardLayout;
        ResourceSet _ShardSet;
        Shader[] _Shards;
        public AxisInstructionsRender(IVeldridContent control) : base(control)
        {
        }
        public override void CreateResources()
        {
            _Shards = CreateShader("ThreeDimLineRender");
            _VertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Vector3>() * 6), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            GraphicsDevice.UpdateBuffer(_VertexBuffer, 0, _Vertex);
            vertexLayout = new VertexLayoutDescription(new VertexElementDescription("Position", VertexElementFormat.Float3, VertexElementSemantic.TextureCoordinate));
            _ProjViewBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Matrix4x4>() * 3), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _LineInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Vector4>()), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _ShardLayout = ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ProjView", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("LineInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
            _ShardSet = ResourceFactory.CreateResourceSet(new ResourceSetDescription(_ShardLayout, _ProjViewBuffer, _LineInfoBuffer));

            currentPipline = CreatePipLine(PrimitiveTopology.LineList, _ShardLayout, vertexLayout, _Shards, BlendStateDescription.SingleAlphaBlend);
        }
        public RgbaFloat XAxisColor { get; set; } = RgbaFloat.Red;
        public RgbaFloat YAxisColor { get; set; } = RgbaFloat.Green;
        public RgbaFloat ZAxisColor { get; set; } = RgbaFloat.Blue;

        internal override void PreDraw()
        {
            base.PreDraw();
            if(WindowSizeState)
            {
                Vector3 motionDir = Vector3.UnitY * 10 + Vector3.UnitX * 10 * Camera.AspectRatio;
                Quaternion lookRotation = Quaternion.CreateFromYawPitchRoll(0, 0, 0);
                Vector3 lookDir = Vector3.Transform(-Vector3.UnitZ, lookRotation);
                Vector3 poisiton = Vector3.UnitZ * 20 + motionDir;
                Matrix4x4 matrix = Matrix4x4.CreateLookAt(poisiton, poisiton + lookDir, Vector3.UnitY);
                GraphicsDevice.UpdateBuffer(_ProjViewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>(), Matrix4x4.CreatePerspectiveFieldOfView(1, Camera.AspectRatio, 0.1f, 100f));
                CommandList.UpdateBuffer(_ProjViewBuffer, 0, matrix);
                WindowSizeState = false;
            }
        }
        internal override void DrawData()
        {
            if (IsDisposed || _ProjViewBuffer.IsDisposed || _ProjViewBuffer.IsDisposed || _LineInfoBuffer.IsDisposed) return;
            CommandList.UpdateBuffer(_ProjViewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>() * 2, Camera.ModelMatrix);
            CommandList.SetVertexBuffer(0, _VertexBuffer);
            CommandList.SetPipeline(currentPipline);
            CommandList.SetGraphicsResourceSet(0, _ShardSet);
            CommandList.UpdateBuffer(_LineInfoBuffer, 0, XAxisColor);
            CommandList.Draw(2);
            CommandList.UpdateBuffer(_LineInfoBuffer, 0, YAxisColor);
            CommandList.Draw(2, 1, 2, 0);
            CommandList.UpdateBuffer(_LineInfoBuffer, 0, ZAxisColor);
            CommandList.Draw(2, 1, 4, 0);
        }

        public override void DisposeResources()
        {
            lock (_Locker)
            {
                base.DisposeResources();
                currentPipline?.Dispose();
                _VertexBuffer?.Dispose();
                _ProjViewBuffer?.Dispose();
                _LineInfoBuffer?.Dispose();
                _ShardLayout?.Dispose();
                _ShardSet?.Dispose();
            }
        }
    }
}
