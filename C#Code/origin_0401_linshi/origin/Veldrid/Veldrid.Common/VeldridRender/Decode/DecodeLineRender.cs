using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Veldrid.Common.VeldridRender
{
    internal class DecodeLineRender : BaseVeldridRender
    {
        [AllowNull]
        private Pipeline _Pipeline;
        [AllowNull]
        private ResourceSet _ResourceSet;
        [AllowNull]
        private ResourceLayout _ResourceLayout;
        [AllowNull]
        private DeviceBuffer _DecodeDataInfoBuffer;
        [AllowNull]
        private DeviceBuffer _ProjViewBuffer;
        [AllowNull]
        private DeviceBuffer _DataBuffer;
        private DecodeDataInfo _DecodeDataInfo;
        public DecodeLineRender(IVeldridContent control,UInt32 maxBufferlen = 128*1024*1024) : base(control)
        {
            MaxBufferLength = maxBufferlen;
        }
        public override void CreateResources()
        {
            base.CreateResources();
            _DataBuffer = ResourceFactory.CreateBuffer(new BufferDescription(MaxBufferLength, BufferUsage.StructuredBufferReadWrite,4));
            _ProjViewBuffer = ResourceFactory.CreateBuffer(new BufferDescription((UInt32)Unsafe.SizeOf<Matrix4x4>() * 2, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _DecodeDataInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((UInt32)Unsafe.SizeOf<DecodeDataInfo>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _ResourceLayout = ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ProjView", ResourceKind.UniformBuffer, ShaderStages.Geometry),
                new ResourceLayoutElementDescription("DataBuffer", ResourceKind.StructuredBufferReadOnly, ShaderStages.Geometry),
                new ResourceLayoutElementDescription("DecodeDataInfo", ResourceKind.UniformBuffer, ShaderStages.Geometry)));
            _ResourceSet = ResourceFactory.CreateResourceSet(new ResourceSetDescription(_ResourceLayout, _ProjViewBuffer, _DataBuffer, _DecodeDataInfoBuffer));
            Shader[] shaders = new Shader[3];
            var temp = CreateShader("DecodeLineRender");
            shaders[0] = temp[0];
            shaders[1] = temp[1];
            shaders[2] = GetOtherShader("DecodeLineRender.Geometry.hlsl", ShaderStages.Geometry);
            _Pipeline = CreatePipLine(PrimitiveTopology.PointList,
                new[] { _ResourceLayout },
                new VertexLayoutDescription[0],
                shaders);
        }
        public override void DisposeResources()
        {
            base.DisposeResources();
            _DataBuffer?.Dispose();
            _ProjViewBuffer?.Dispose();
            _DecodeDataInfoBuffer?.Dispose();
            _ResourceLayout?.Dispose();
            _ResourceSet?.Dispose();
            _Pipeline?.Dispose();
        }
        public UInt32 MaxBufferLength { get; }
        public UInt32 BufferLength { get; private set; }
        public void SetData<T>(ref T data,UInt32 sizeInBytes) where T:unmanaged
        {
            if (_DataBuffer == null || _DataBuffer.IsDisposed)
            {
                BufferLength = sizeInBytes;
                return;
            }
            if (sizeInBytes > _DataBuffer.SizeInBytes) return;
            BufferLength = sizeInBytes;
            GraphicsDevice.UpdateBuffer(_DataBuffer,0,ref data,sizeInBytes);
        }
        internal override void PreDraw()
        {
            base.PreDraw();
            if (this[nameof(Range), nameof(Margin)] || WindowSizeState)
            {
                CommandList.UpdateBuffer(_ProjViewBuffer, 0, Camera.OrthographicMatrix);
                CommandList.UpdateBuffer(_ProjViewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>(), Camera.GetLineMatrix(Margin, Range));
                this[nameof(Range), nameof(Margin)] = false;
                if (WindowSizeState) WindowSizeState = false;
            }
        }
        internal override void DrawData()
        {
            if (!Visibily || 
                PerChannelDataLength == 0 || 
                ChannelCount == 0 ||
                BufferLength==0||
                _Pipeline.IsDisposed ||
                _ProjViewBuffer.IsDisposed ||
                _ResourceLayout.IsDisposed ||
                _ResourceSet.IsDisposed ||
                _DecodeDataInfoBuffer.IsDisposed||
                _DataBuffer.IsDisposed) return;
            GraphicsDevice.UpdateBuffer(_DecodeDataInfoBuffer, 0, _DecodeDataInfo);
            CommandList.SetFramebuffer(MainSwapchainBuffer);
            CommandList.SetPipeline(_Pipeline);
            CommandList.SetGraphicsResourceSet(0, _ResourceSet);
            CommandList.Draw(BufferLength / 4);
        }
        public UInt32 ChannelCount { get => _DecodeDataInfo.ChannelCount; set => _DecodeDataInfo.ChannelCount = value; }
        public UInt32 PerChannelDataLength { get => _DecodeDataInfo.PerChannelDataLength; set => _DecodeDataInfo.PerChannelDataLength = value; }
        public Int32 InterwovenBitCount { get => _DecodeDataInfo.InterwovenBitCount;set=> _DecodeDataInfo.InterwovenBitCount = value; }
        public Single DataInterval { get => _DecodeDataInfo.DataInterval; set => _DecodeDataInfo.DataInterval = value; }
        public Int32 TriggerIndex { get => _DecodeDataInfo.TriggerIndex;set=> _DecodeDataInfo.TriggerIndex = value; }
        public Single Position { get => _DecodeDataInfo.Position; set => _DecodeDataInfo.Position = value; }
        public Int32 Chindex { get => _DecodeDataInfo.Chindex; set => _DecodeDataInfo.Chindex = value; }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct DecodeDataInfo
        {
            public UInt32 ChannelCount;
            public UInt32 PerChannelDataLength;
            public Int32 InterwovenBitCount;
            public Single DataInterval;
            public Int32 TriggerIndex;
            public Single Position;
            public Int32 Chindex;
            public Single Spare;
        }
    }
}
