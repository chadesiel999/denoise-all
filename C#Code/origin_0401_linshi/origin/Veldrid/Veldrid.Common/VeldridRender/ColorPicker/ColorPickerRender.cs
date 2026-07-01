using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Veldrid.Common.VeldridRender
{
    internal class ColorPickerRender : BaseVeldridRender
    {
        private DeviceBuffer _VertexBuffer;
        private DeviceBuffer _ProjViewBuffer;
        private ResourceLayout _ShardLayout;
        private ResourceSet _ShardSet;
        private Pipeline _Pipeline;
        public ColorPickerRender(IVeldridContent control) : base(control)
        {
        }
        public override void CreateResources()
        {
            
            _VertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription(PositionColor.SizeBytes * 6, BufferUsage.VertexBuffer));
            UpdateVertex();
            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("in_Position", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate),
                new VertexElementDescription("in_Color", VertexElementFormat.Float4, VertexElementSemantic.TextureCoordinate));
            _ProjViewBuffer = ResourceFactory.CreateBuffer(new BufferDescription((UInt32)(Unsafe.SizeOf<Matrix4x4>() * 2), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _ShardLayout = ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(new ResourceLayoutElementDescription("ProjView", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
            _ShardSet = ResourceFactory.CreateResourceSet(new ResourceSetDescription(_ShardLayout, _ProjViewBuffer));
            _Pipeline = CreatePipLine(PrimitiveTopology.TriangleStrip, _ShardLayout, vertexLayout, CreateShader("ColorPickerRender"));
        }
        public override void DisposeResources()
        {
            lock (_Locker)
            {
                _VertexBuffer?.Dispose();
                _ProjViewBuffer?.Dispose();
                _ShardLayout?.Dispose();
                _Pipeline?.Dispose();
                _ShardSet?.Dispose();
            }
        }
        internal override void PreDraw()
        {
            if (_ProjViewBuffer.IsDisposed) return;
            base.PreDraw();
            if (WindowSizeState)
            {
                CommandList.UpdateBuffer(_ProjViewBuffer, 0, Camera.OrthographicMatrix);
                CommandList.UpdateBuffer(_ProjViewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>(),Matrix4x4.Identity);
                UpdateVertex();
                WindowSizeState = false;
            }
        }
        private void UpdateVertex()
        {
            float hei = 0.4f;
            float wid = 20f;
            PositionColor[] positions = new PositionColor[6]
            {
                new PositionColor(new Vector2(0,MainSwapchainBuffer.Height*(0.5f-hei/2)),RgbaFloat.Red),
                new PositionColor(new Vector2(wid,MainSwapchainBuffer.Height*(0.5f-hei/2)),RgbaFloat.Red),
                new PositionColor(new Vector2(0,MainSwapchainBuffer.Height/2),RgbaFloat.Blue),
                new PositionColor(new Vector2(wid,MainSwapchainBuffer.Height/2),RgbaFloat.Blue),
                new PositionColor(new Vector2(0,MainSwapchainBuffer.Height*(0.5f+hei/2)),RgbaFloat.Green),
                new PositionColor(new Vector2(wid,MainSwapchainBuffer.Height*(0.5f+hei/2)),RgbaFloat.Green),
            };
            GraphicsDevice.UpdateBuffer(_VertexBuffer, 0, positions);
        }
        internal override void DrawData()
        {
            CommandList.SetFramebuffer(MainSwapchainBuffer);
            CommandList.SetVertexBuffer(0,_VertexBuffer);
            CommandList.SetPipeline(_Pipeline);
            CommandList.SetGraphicsResourceSet(0, _ShardSet);
            CommandList.Draw(6);
        }
        struct PositionColor
        {
            public static UInt32 SizeBytes = 32;
            public Vector2 Position;
            public RgbaFloat Color;
            public PositionColor(Vector2 position,RgbaFloat color)
            {
                Position = position;
                Color = color;
            }
        }
    }
}
