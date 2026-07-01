using FontStashSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid.Common.Tools;

namespace Veldrid.Common.VeldridRender
{
    internal class DecodeRender : BaseVeldridRender
    {
        UInt32 _StripCount = 0;
        private uint _MaxCount;
        private DecodeInfo[] _DecodeInfo = new DecodeInfo[0];
        [AllowNull]
        Pipeline _LinePipeline;
        [AllowNull]
        Pipeline _StripPipeline;
        [AllowNull]
        ResourceLayout _ShardLayout;
        [AllowNull]
        ResourceSet _ShardSet;
        [AllowNull]
        DeviceBuffer _VertexBuffer;
        [AllowNull]
        DeviceBuffer _ProjViewBuffer;
        [AllowNull]
        DeviceBuffer _LineInfoBuffer;
        private LineInfo _LineInfo;
        public DecodeRender(IVeldridContent control,int maxCount = 2000) : base(control)
        {
            _MaxCount = (uint)maxCount;
            Brightness = 100;
        }
        public override void CreateResources()
        {
            base.CreateResources();
            _VertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription(DecodeInfo.SizeInBytes * _MaxCount, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            _ProjViewBuffer = ResourceFactory.CreateBuffer(new BufferDescription((UInt32)Unsafe.SizeOf<Matrix4x4>() * 2, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _LineInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription(LineInfo.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("in_Position", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate),
                new VertexElementDescription("in_Size", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate),
                new VertexElementDescription("in_Polygon", VertexElementFormat.Float1, VertexElementSemantic.TextureCoordinate),
                new VertexElementDescription("in_Color", VertexElementFormat.Float4, VertexElementSemantic.TextureCoordinate));
            var shaders = CreateShader("DecodeRender");
            _ShardLayout = ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ProjView", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("LineInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
            _ShardSet = ResourceFactory.CreateResourceSet(new ResourceSetDescription(_ShardLayout, _ProjViewBuffer, _LineInfoBuffer));
            switch(GraphicsDevice.BackendType)
            {
                case GraphicsBackend.Direct3D11:
                    {
                        var lineshader = GetOtherShader("DecodeRenderLine.Geometry.hlsl");
                        _LinePipeline = CreatePipLine(PrimitiveTopology.PointList, _ShardLayout, vertexLayout, new Shader[] { shaders[0], shaders[1], lineshader },BlendStateDescription.SingleAdditiveBlend);
                        var stripshader = GetOtherShader("DecodeRenderStrip.Geometry.hlsl");
                        _StripPipeline = CreatePipLine(PrimitiveTopology.PointList, _ShardLayout, vertexLayout, new Shader[] { shaders[0], shaders[1], stripshader },BlendStateDescription.SingleAdditiveBlend,frontFace: FrontFace.CounterClockwise);
                    }
                    break;
                default:
                    throw new NotSupportedException(GraphicsDevice.BackendType.ToString());
            }
            WindowSizeState = true;
            this[nameof(Brightness)] = true;
        }
        internal override void PreDraw()
        {
            if (_ProjViewBuffer.IsDisposed || _LineInfoBuffer.IsDisposed) return;
            base.PreDraw();
            if (this[nameof(Range), nameof(Margin)] || WindowSizeState)
            {
                CommandList.UpdateBuffer(_ProjViewBuffer, 0, Camera.OrthographicMatrix);
                CommandList.UpdateBuffer(_ProjViewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>(), Camera.GetLineMatrix(Margin, Range));
                CommandList.UpdateBuffer(_LineInfoBuffer, 0, (Vector4)Range);
                this[nameof(Range), nameof(Margin)] = false;
                if (WindowSizeState) WindowSizeState = false;
                PerPixelSize = new Vector2(Range.XLenght, Range.YLenght) / new Vector2(Rectangle.Width,Rectangle.Height);
                
            }
            if (this[nameof(Brightness),nameof(VerticalOffset),nameof(HorizontalOffset),nameof(Slop)])
            {
                _LineInfo.Range = Range;
                CommandList.UpdateBuffer(_LineInfoBuffer, 0, _LineInfo);
                this[nameof(Brightness),nameof(VerticalOffset),nameof(HorizontalOffset),nameof(Slop)] = false;
            }
        }
        public Vector2 PerPixelSize { get; private set; }
        public override void DisposeResources()
        {
            lock (_Locker)
            {
                base.DisposeResources();
                _LineInfoBuffer?.Dispose();
                _LinePipeline?.Dispose();
                _ProjViewBuffer?.Dispose();
                _ShardLayout?.Dispose();
                _ShardSet?.Dispose();
                _StripPipeline?.Dispose();
                _VertexBuffer?.Dispose();
            }
        }
        public float Slop { get => _LineInfo.Slop; set => Set(ref _LineInfo.Slop, value); }
        public float Brightness { get => _LineInfo.Brightness; set => Set(ref _LineInfo.Brightness, value); }
        public float VerticalOffset { get => _LineInfo.VerticalOffset; set => Set(ref _LineInfo.VerticalOffset, value); }
        public float HorizontalOffset { get => _LineInfo.HorizontalOffset; set => Set(ref _LineInfo.HorizontalOffset, value); }
        public void UpdateDecodeinfos(DecodeInfo[] decodeInfos)
        {
            lock (_Locker)
            {
                if (decodeInfos == null || _VertexBuffer.IsDisposed ||IsDisposed) return;
                _DecodeInfo = decodeInfos;
                if (_DecodeInfo.Length == 0) return;
                if (_DecodeInfo.Length > _MaxCount)
                {
                    _VertexBuffer?.Dispose();
                    _MaxCount = (UInt32)_DecodeInfo.Length * 2;
                    _VertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription(DecodeInfo.SizeInBytes * _MaxCount, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
                }
                CommandList.UpdateBuffer(_VertexBuffer, 0, _DecodeInfo);
                _StripCount = 0;
                for (int index = 0; index < _DecodeInfo.Length; index++)
                {
                    if (_DecodeInfo[index].Polygon >= 3)
                    {
                        _StripCount = (UInt32)index;
                        break;
                    }
                }
            }
        }

        internal override void DrawData()
        {
            if (_DecodeInfo.Length == 0 || !Visibily || _VertexBuffer.IsDisposed || _LineInfoBuffer.IsDisposed || IsDisposed) return;

            CommandList.SetFramebuffer(MainSwapchainBuffer);
            CommandList.SetVertexBuffer(0, _VertexBuffer);
            if(_StripCount>0)
            {
                CommandList.SetPipeline(_StripPipeline);
            }
            else
            {
                CommandList.SetPipeline(_LinePipeline);
            }
            CommandList.SetGraphicsResourceSet(0, _ShardSet);

            if(_StripCount>0)
            {
                CommandList.Draw(_StripCount, 1, 0, 0);
            }
            else
            {
                CommandList.Draw((UInt32)_DecodeInfo.Length, 1, 0, 0);
            }
            if(_StripCount < _DecodeInfo.Length && _StripCount>0)
            {
                CommandList.SetPipeline(_LinePipeline);
                CommandList.SetGraphicsResourceSet(0, _ShardSet);
                CommandList.Draw((UInt32)_DecodeInfo.Length - _StripCount, 1, _StripCount, 0);
            }
        }
        private struct LineInfo
        {
            public static uint SizeInBytes = (uint)Unsafe.SizeOf<float>() * 8;
            public Vector4 Range;
            public float VerticalOffset;
            public float HorizontalOffset;
            public float Brightness;
            public float Slop;
        }
        public enum Polygon
        {
            LineFill=0,
            RectangleFill=1,
            HexagonFill=2,
            Line=3,
            Rectangle=4,
            Hexagon=5,
        }
        public struct DecodeInfo
        {
            public static uint SizeInBytes = (uint)Unsafe.SizeOf<float>()*9;
            public Vector2 Position;
            public Vector2 Size;
            /// <summary>
            /// <see cref="DecodeRender.Polygon"/>
            /// </summary>
            public float Polygon;
            public RgbaFloat Color;
            public override Boolean Equals(object obj)
            {
                if(obj ==null) return false;
                if(obj is DecodeInfo info)
                {
                    return info.Polygon== Polygon&& info.Size == Size && info.Color == Color && info.Position == Position;
                }
                else return false;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
}
